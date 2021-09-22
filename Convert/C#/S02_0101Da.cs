using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

/// <summary>
/// コース一覧照会のDAクラス
/// </summary>
public partial class S02_0101Da : Hatobus.ReservationManagementSystem.Common.DataAccessorBase
{
    #region  定数／変数 
    /// <summary>
    /// 運行区分：運休（廃止）
    /// </summary>
    private const string UNKOU_KBN_HAISI = "X";

    /// <summary>
    /// 乗車定員（空席表示に「バスの空席数」判断用）
    /// </summary>
    private const int JYOSYA_CAPACITY_BUS = 999;

    /// <summary>
    /// 検索方法
    /// </summary>
    public enum AccessType : int
    {
        courseByHeaderKey,   // 一覧結果取得検索
        courseByPrimaryKey,  // キー重複チェック
        courseByCheck       // 存在チェック
    }

    // コース台帳（基本）エンティティ
    private readonly CrsLedgerBasicEntity ClsCrsDaityoEntity = new CrsLedgerBasicEntity();
    // 場所マスタエンティティ
    private readonly PlaceMasterEntity ClsPlaceMasteroEntity = new PlaceMasterEntity();
    // コースコントロール情報エンティティ
    private readonly CrsControlInfoEntity ClsCrsContorolInfoEntity = new CrsControlInfoEntity();

    #endregion

    #region  SELECT処理 

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable AccessCourseDaityo(AccessType type, Hashtable paramInfoList = null)
    {
        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case AccessType.courseByCheck:
                {
                    // 一覧結果取得検索
                    sqlString = GetCourseCdCheck(paramInfoList);
                    break;
                }

            case AccessType.courseByHeaderKey:
                {
                    // 一覧結果取得検索
                    sqlString = GetCourseDaityo(paramInfoList);
                    break;
                }

            case AccessType.courseByPrimaryKey:
                {
                    // キー重複チェック
                    sqlString = GetCourseDataByPrimaryKey(paramInfoList);
                    break;
                }

            default:
                {
                    // 該当処理なし
                    return returnValue;
                }
        }

        try
        {
            returnValue = getDataTable(sqlString);
        }
        catch (Oracle.ManagedDataAccess.Client.OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// コース一覧照会検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string GetCourseDaityo(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        var s02_0101 = new S02_0101();
        {
            var withBlock = ClsCrsDaityoEntity;
            // SELECT句
            sqlString.AppendLine("SELECT ");
            sqlString.AppendLine("  DISTINCT ");
            sqlString.AppendLine("  '' AS YoyakuButton,");                                   // 予約ボタン
            sqlString.AppendLine("  '' AS TojituHakken,");                                   // 当日発券ボタン
            // 出発日
            sqlString.AppendLine(" SUBSTR(XX.SYUPT_DAY,1,4)||'/'||SUBSTR(XX.SYUPT_DAY,5,2)||'/'||");
            sqlString.AppendLine(" SUBSTR(XX.SYUPT_DAY,7,2) AS SYUPT_DAY_HEN,");
            // 出発時間
            sqlString.AppendLine(" CASE XX.SYUPT_TIME WHEN 0 THEN '' ELSE ");
            sqlString.AppendLine("  SUBSTR(LPAD(XX.SYUPT_TIME,4,'0'), 1, 2)||':'||");
            sqlString.AppendLine("  SUBSTR(LPAD(XX.SYUPT_TIME,4,'0'), 3, 2) END AS SYUPT_TIME_HEN,");
            sqlString.AppendLine("  XX.CRS_CD,");                                            // コースコード
            sqlString.AppendLine("  NVL(XX.CRS_NAME_RK,XX.CRS_NAME) AS CRS_NAME,");          // コース名称
            sqlString.AppendLine("  ''  AS SITUATION,");                                     // 状態
            sqlString.AppendLine("  PLACE.PLACE_NAME_SHORT || KEIYUKBN AS HAISYA_KEIYU,");   // 配車経由
            sqlString.AppendLine("  XX.GOUSYA,");                                            // 号車
            // 空席数定席（乗車定員・定員が999で設定されている場合の表示方法はバスの空席数）
            sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " + JYOSYA_CAPACITY_BUS + " THEN ");
            sqlString.AppendLine("      NVL(ZSK.KUSEKI_NUM_TEISEKI,0) ");
            sqlString.AppendLine("    ELSE ");
            sqlString.AppendLine("      NVL(XX.KUSEKI_NUM_TEISEKI,0)");
            sqlString.AppendLine("  END AS KUSEKI_NUM_TEISEKI,");                            // 空席数定席
            // 空席数補助席（乗車定員・定員が999で設定されている場合の表示方法はバスの空席数）
            sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " + JYOSYA_CAPACITY_BUS + " THEN ");
            sqlString.AppendLine("      NVL(ZSK.KUSEKI_NUM_SUB_SEAT,0) ");
            sqlString.AppendLine("    ELSE ");
            sqlString.AppendLine("      NVL(XX.KUSEKI_NUM_SUB_SEAT,0) ");
            sqlString.AppendLine("  END AS KUSEKI_NUM_SUB_SEAT,");                           // 空席数補助席
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_TEISEKI,0) + ");                       // 予約数定席
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_SUB_SEAT,0) + ");                      // 予約数補助席
            sqlString.AppendLine("  NVL(XX.BLOCK_KAKUHO_NUM,0) + ");                         // ブロック確保数
            sqlString.AppendLine("  NVL(XX.KUSEKI_KAKUHO_NUM,0) AS ALLNUM_UTIWAKE,");        // =総数内訳
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_TEISEKI,0) + ");
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_SUB_SEAT,0) AS YOYAKU_NUM_TEISEKI,");  // 予約数定席
            sqlString.AppendLine("  NVL(XX.BLOCK_KAKUHO_NUM,0) AS BLOCK_KAKUHO_NUM,");       // ブロック確保数
            sqlString.AppendLine("  NVL(XX.KUSEKI_KAKUHO_NUM,0) AS KUSEKI_KAKUHO_NUM,");     // 空席確保数
            sqlString.AppendLine("  NVL(XX.CANCEL_WAIT_NINZU,0) AS CANCEL_WAIT_NINZU,");     // WT件数
            sqlString.AppendLine("  0 AS REQUEST_KENSU ,");                                  // リクエスト件数       TODO 二次対応
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_ONE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_ONE_1R,");     // 部屋残数１人部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_TWO_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_TWO_1R,");     // 部屋残数２人部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_THREE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_THREE_1R,"); // 部屋残数３名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_FOUR_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_FOUR_1R,");   // 部屋残数４名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_FIVE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_FIVE_1R,");   // 部屋残数５名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI,");       // 部屋残数総計
            sqlString.AppendLine("  CODEBUS.CODE_NAME  AS BUS_TYPE,");                       // バスタイプ
            sqlString.AppendLine("  XX.HAISYA_KEIYU_CD,");                                   // 配車経由コード
            sqlString.AppendLine("  XX.GUIDE_GENGO, ");                                      // ガイド言語
            sqlString.AppendLine("  CODE1.CODE_NAME AS CATEGORY1, ");                        // コード名称1（カテゴリ）
            sqlString.AppendLine("  CODE2.CODE_NAME AS CATEGORY2, ");                        // コード名称2（カテゴリ）
            sqlString.AppendLine("  CODE3.CODE_NAME AS CATEGORY3, ");                        // コード名称3（カテゴリ）
            sqlString.AppendLine("  CODE4.CODE_NAME AS CATEGORY4, ");                        // コード名称4（カテゴリ）
            sqlString.AppendLine("  XX.ONE_SANKA_FLG, ");                                    // １名参加フラグ
            sqlString.AppendLine("  XX.TEIKI_KIKAKU_KBN, ");                                 // 定期・企画区分
            sqlString.AppendLine("  XX.HOUJIN_GAIKYAKU_KBN, ");                              // 邦人/ 外客区分  
            sqlString.AppendLine("  XX.CRS_KBN_1, ");                                        // コース区分１  /昼 / 夜 
            sqlString.AppendLine("  XX.CRS_KBN_2, ");                                        // コース区分２  /都内 / 郊外
            sqlString.AppendLine("  XX.CRS_KIND, ");                                         // コース種別  /はとバス定期 / ｷｬﾋﾟﾀﾙ / 企画日帰り / 企画宿泊 / 企画都内Rコース
            sqlString.AppendLine("  XX.CAR_TYPE_CD, ");                                      // 車種コード
            sqlString.AppendLine("  XX.TEJIMAI_KBN, ");                                      // 手仕舞区分
            sqlString.AppendLine("  XX.YOYAKU_STOP_FLG, ");                                  // 予約停止フラグ
            sqlString.AppendLine("  XX.UNKYU_KBN, ");                                        // 運休区分    /運休/廃止
            sqlString.AppendLine("  XX.SAIKOU_KAKUTEI_KBN, ");                               // 催行確定区分
            sqlString.AppendLine("  XX.UKETUKE_START_DAY, ");                                // 受付開始日
            sqlString.AppendLine("  XX.SYUPT_DAY,");                                         // 出発日
            sqlString.AppendLine("  XX.SYUPT_TIME,");                                        // 出発時間
            sqlString.AppendLine("  XX.TEIINSEI_FLG, ");                                     // 定員制フラグ
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI ");       // 部屋残数総計

            // FROM句
            sqlString.AppendLine("FROM");
            sqlString.AppendLine("  (SELECT ");
            sqlString.AppendLine("      SYUPT_TIME_1 AS SYUPT_TIME,");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_1 AS HAISYA_KEIYU_CD,");
            sqlString.AppendLine("      '' AS KEIYUKBN,");                                   // 乗り場/経由区分（ ’’ =出発地）
            sqlString.AppendLine("      SYUPT_DAY,");
            sqlString.AppendLine("      GOUSYA,");
            sqlString.AppendLine("      CRS_CD,");
            sqlString.AppendLine("      CRS_NAME_RK,");
            sqlString.AppendLine("      CRS_NAME,");
            sqlString.AppendLine("      UKETUKE_START_DAY,");
            sqlString.AppendLine("      JYOSYA_CAPACITY,");
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,");
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,");
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,");
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,");
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,");
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,");
            sqlString.AppendLine("      GUIDE_GENGO,");
            sqlString.AppendLine("      ONE_SANKA_FLG,");
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,");
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,");
            sqlString.AppendLine("      CRS_KBN_1,");
            sqlString.AppendLine("      CRS_KBN_2,");
            sqlString.AppendLine("      CRS_KIND,");
            sqlString.AppendLine("      CAR_TYPE_CD,");
            sqlString.AppendLine("      BUS_RESERVE_CD,");
            sqlString.AppendLine("      HOMEN_CD,");
            sqlString.AppendLine("      TEJIMAI_KBN,");
            sqlString.AppendLine("      YOYAKU_NG_FLG,");
            sqlString.AppendLine("      YOYAKU_STOP_FLG,");
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,");
            sqlString.AppendLine("      TEIINSEI_FLG,");
            sqlString.AppendLine("      UNKYU_KBN,");
            sqlString.AppendLine("      CATEGORY_CD_1,");
            sqlString.AppendLine("      CATEGORY_CD_2,");
            sqlString.AppendLine("      CATEGORY_CD_3,");
            sqlString.AppendLine("      CATEGORY_CD_4");
            sqlString.AppendLine("    FROM ");
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC");
            sqlString.AppendLine("    WHERE ");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_1 IS Not NULL");         // 配車経由地コード1が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ");             // 削除日＝0
            sqlString.AppendLine("   UNION ALL");
            sqlString.AppendLine("   SELECT ");
            sqlString.AppendLine("      SYUPT_TIME_2 AS SYUPT_TIME,");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_2 AS HAISYA_KEIYU_CD,");
            sqlString.AppendLine("      '*' AS KEIYUKBN,");                                  // 乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,");
            sqlString.AppendLine("      GOUSYA,");
            sqlString.AppendLine("      CRS_CD,");
            sqlString.AppendLine("      CRS_NAME_RK,");
            sqlString.AppendLine("      CRS_NAME,");
            sqlString.AppendLine("      UKETUKE_START_DAY,");
            sqlString.AppendLine("      JYOSYA_CAPACITY,");
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,");
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,");
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,");
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,");
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,");
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,");
            sqlString.AppendLine("      GUIDE_GENGO,");
            sqlString.AppendLine("      ONE_SANKA_FLG,");
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,");
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,");
            sqlString.AppendLine("      CRS_KBN_1,");
            sqlString.AppendLine("      CRS_KBN_2,");
            sqlString.AppendLine("      CRS_KIND,");
            sqlString.AppendLine("      CAR_TYPE_CD,");
            sqlString.AppendLine("      BUS_RESERVE_CD,");
            sqlString.AppendLine("      HOMEN_CD,");
            sqlString.AppendLine("      TEJIMAI_KBN,");
            sqlString.AppendLine("      YOYAKU_NG_FLG,");
            sqlString.AppendLine("      YOYAKU_STOP_FLG,");
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,");
            sqlString.AppendLine("      TEIINSEI_FLG,");
            sqlString.AppendLine("      UNKYU_KBN,");
            sqlString.AppendLine("      CATEGORY_CD_1,");
            sqlString.AppendLine("      CATEGORY_CD_2,");
            sqlString.AppendLine("      CATEGORY_CD_3,");
            sqlString.AppendLine("      CATEGORY_CD_4");
            sqlString.AppendLine("    FROM ");
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC");
            sqlString.AppendLine("    WHERE ");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_2 IS Not NULL");         // 配車経由地コード2が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ");             // 削除日＝0
            sqlString.AppendLine("   UNION ALL");
            sqlString.AppendLine("   SELECT ");
            sqlString.AppendLine("      SYUPT_TIME_3 AS SYUPT_TIME,");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_3 AS HAISYA_KEIYU_CD,");
            sqlString.AppendLine("      '*' AS KEIYUKBN,");                                  // 乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,");
            sqlString.AppendLine("      GOUSYA,");
            sqlString.AppendLine("      CRS_CD,");
            sqlString.AppendLine("      CRS_NAME_RK,");
            sqlString.AppendLine("      CRS_NAME,");
            sqlString.AppendLine("      UKETUKE_START_DAY,");
            sqlString.AppendLine("      JYOSYA_CAPACITY,");
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,");
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,");
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,");
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,");
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,");
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,");
            sqlString.AppendLine("      GUIDE_GENGO,");
            sqlString.AppendLine("      ONE_SANKA_FLG,");
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,");
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,");
            sqlString.AppendLine("      CRS_KBN_1,");
            sqlString.AppendLine("      CRS_KBN_2,");
            sqlString.AppendLine("      CRS_KIND,");
            sqlString.AppendLine("      CAR_TYPE_CD,");
            sqlString.AppendLine("      BUS_RESERVE_CD,");
            sqlString.AppendLine("      HOMEN_CD,");
            sqlString.AppendLine("      TEJIMAI_KBN,");
            sqlString.AppendLine("      YOYAKU_NG_FLG,");
            sqlString.AppendLine("      YOYAKU_STOP_FLG,");
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,");
            sqlString.AppendLine("      TEIINSEI_FLG,");
            sqlString.AppendLine("      UNKYU_KBN,");
            sqlString.AppendLine("      CATEGORY_CD_1,");
            sqlString.AppendLine("      CATEGORY_CD_2,");
            sqlString.AppendLine("      CATEGORY_CD_3,");
            sqlString.AppendLine("      CATEGORY_CD_4");
            sqlString.AppendLine("    FROM ");
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC");
            sqlString.AppendLine("    WHERE ");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_3 IS Not NULL");         // 配車経由地コード3が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ");             // 削除日＝0
            sqlString.AppendLine("   UNION ALL");
            sqlString.AppendLine("   SELECT ");
            sqlString.AppendLine("      SYUPT_TIME_4 AS SYUPT_TIME,");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_4 AS HAISYA_KEIYU_CD,");
            sqlString.AppendLine("      '*' AS KEIYUKBN,");                                  // 乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,");
            sqlString.AppendLine("      GOUSYA,");
            sqlString.AppendLine("      CRS_CD,");
            sqlString.AppendLine("      CRS_NAME_RK,");
            sqlString.AppendLine("      CRS_NAME,");
            sqlString.AppendLine("      UKETUKE_START_DAY,");
            sqlString.AppendLine("      JYOSYA_CAPACITY,");
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,");
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,");
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,");
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,");
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,");
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,");
            sqlString.AppendLine("      GUIDE_GENGO,");
            sqlString.AppendLine("      ONE_SANKA_FLG,");
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,");
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,");
            sqlString.AppendLine("      CRS_KBN_1,");
            sqlString.AppendLine("      CRS_KBN_2,");
            sqlString.AppendLine("      CRS_KIND,");
            sqlString.AppendLine("      CAR_TYPE_CD,");
            sqlString.AppendLine("      BUS_RESERVE_CD,");
            sqlString.AppendLine("      HOMEN_CD,");
            sqlString.AppendLine("      TEJIMAI_KBN,");
            sqlString.AppendLine("      YOYAKU_NG_FLG,");
            sqlString.AppendLine("      YOYAKU_STOP_FLG,");
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,");
            sqlString.AppendLine("      TEIINSEI_FLG,");
            sqlString.AppendLine("      UNKYU_KBN,");
            sqlString.AppendLine("      CATEGORY_CD_1,");
            sqlString.AppendLine("      CATEGORY_CD_2,");
            sqlString.AppendLine("      CATEGORY_CD_3,");
            sqlString.AppendLine("      CATEGORY_CD_4");
            sqlString.AppendLine("    FROM ");
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC");
            sqlString.AppendLine("    WHERE ");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_4 IS Not NULL");         // 配車経由地コード4が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ");             // 削除日＝0
            sqlString.AppendLine("   UNION ALL");
            sqlString.AppendLine("   SELECT ");
            sqlString.AppendLine("      SYUPT_TIME_5 AS SYUPT_TIME,");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_5 AS HAISYA_KEIYU_CD,");
            sqlString.AppendLine("      '*' AS KEIYUKBN,");                                  // 乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,");
            sqlString.AppendLine("      GOUSYA,");
            sqlString.AppendLine("      CRS_CD,");
            sqlString.AppendLine("      CRS_NAME_RK,");
            sqlString.AppendLine("      CRS_NAME,");
            sqlString.AppendLine("      UKETUKE_START_DAY,");
            sqlString.AppendLine("      JYOSYA_CAPACITY,");
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,");
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,");
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,");
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,");
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,");
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,");
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,");
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,");
            sqlString.AppendLine("      GUIDE_GENGO,");
            sqlString.AppendLine("      ONE_SANKA_FLG,");
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,");
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,");
            sqlString.AppendLine("      CRS_KBN_1,");
            sqlString.AppendLine("      CRS_KBN_2,");
            sqlString.AppendLine("      CRS_KIND,");
            sqlString.AppendLine("      CAR_TYPE_CD,");
            sqlString.AppendLine("      BUS_RESERVE_CD,");
            sqlString.AppendLine("      HOMEN_CD,");
            sqlString.AppendLine("      TEJIMAI_KBN,");
            sqlString.AppendLine("      YOYAKU_NG_FLG,");
            sqlString.AppendLine("      YOYAKU_STOP_FLG,");
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,");
            sqlString.AppendLine("      TEIINSEI_FLG,");
            sqlString.AppendLine("      UNKYU_KBN,");
            sqlString.AppendLine("      CATEGORY_CD_1,");
            sqlString.AppendLine("      CATEGORY_CD_2,");
            sqlString.AppendLine("      CATEGORY_CD_3,");
            sqlString.AppendLine("      CATEGORY_CD_4");
            sqlString.AppendLine("    FROM ");
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC");
            sqlString.AppendLine("    WHERE ");
            sqlString.AppendLine("      HAISYA_KEIYU_CD_5 IS Not NULL");  // 配車経由地コード5が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ");      // 削除日＝0
            sqlString.AppendLine("  ) XX ");
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE1  ON   RTRIM(CODE1.CODE_BUNRUI) = '" + FixedCdYoyaku.CodeBunruiTypeCategory + "'");
            sqlString.AppendLine("  AND RTRIM(CODE1.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_1) ");
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE2  ON   RTRIM(CODE2.CODE_BUNRUI) = '" + FixedCdYoyaku.CodeBunruiTypeCategory + "'");
            sqlString.AppendLine("  AND RTRIM(CODE2.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_2) ");
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE3  ON   RTRIM(CODE3.CODE_BUNRUI) = '" + FixedCdYoyaku.CodeBunruiTypeCategory + "'");
            sqlString.AppendLine("  AND RTRIM(CODE3.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_3) ");
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE4  ON   RTRIM(CODE4.CODE_BUNRUI) = '" + FixedCdYoyaku.CodeBunruiTypeCategory + "'");
            sqlString.AppendLine("  AND RTRIM(CODE4.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_4) ");
            sqlString.AppendLine("LEFT JOIN  M_PLACE PLACE  ON  RTRIM(PLACE.PLACE_CD) = RTRIM(XX.HAISYA_KEIYU_CD) ");
            sqlString.AppendLine("LEFT JOIN  M_CAR_KIND CAR_KIND  ON  RTRIM(CAR_KIND.CAR_CD) = RTRIM(XX.CAR_TYPE_CD) ");
            sqlString.AppendLine("LEFT JOIN  M_CODE CODEBUS  ON   RTRIM(CODEBUS.CODE_BUNRUI) = '" + CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.busType) + "'");
            sqlString.AppendLine("  AND RTRIM(CODEBUS.CODE_VALUE) = RTRIM(CAR_KIND.BUS_TYPE) ");
            sqlString.AppendLine("LEFT JOIN  T_ZASEKI_IMAGE ZSK ON  RTRIM(ZSK.BUS_RESERVE_CD) = RTRIM(XX.BUS_RESERVE_CD) ");
            sqlString.AppendLine("  AND RTRIM(ZSK.GOUSYA) = RTRIM(XX.GOUSYA) ");
            sqlString.AppendLine("  AND RTRIM(ZSK.SYUPT_DAY) = RTRIM(XX.SYUPT_DAY) ");
            // 【フリーワード指定時】
            if (!string.IsNullOrEmpty(paramList["FreeWord"].ToString().Trim()))
            {
                // コースマスタ
                sqlString.AppendLine("LEFT JOIN  T_COURSE_MST CRS_M ON  RTRIM(CRS_M.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(CRS_M.TEIKI_KIKAKU_KBN) = RTRIM(XX.TEIKI_KIKAKU_KBN) ");
                // コースマスタ（発着）ダイヤ[定期]
                sqlString.AppendLine("LEFT JOIN T_COURSE_DIA DIA1 ON  RTRIM(DIA1.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(DIA1.TEIKI_KIKAKU_KBN) = 1 AND DIA1.LINE_NO >= 1 AND DIA1.LINE_NO <= 15");
                // ダイヤ[企画]
                sqlString.AppendLine("LEFT JOIN T_COURSE_DIA DIA2 ON  RTRIM(DIA2.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(DIA2.TEIKI_KIKAKU_KBN) = 2 AND DIA1.LINE_NO >= 1 AND DIA1.LINE_NO <= 4");

                // ○増管理　食事詳細
                sqlString.AppendLine("LEFT JOIN  T_CIB_MEAL_DETAIL CIB_MD ON RTRIM(CIB_MD.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(CIB_MD.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ");
                sqlString.AppendLine("  AND RTRIM(CIB_MD.COACH_NO) = RTRIM(XX.GOUSYA) ");

                // ○増管理　宿泊詳細	T_CIB_HOTEL_DETAIL
                sqlString.AppendLine("LEFT JOIN  T_CIB_HOTEL_DETAIL CIB_HD ON RTRIM(CIB_HD.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(CIB_HD.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ");
                sqlString.AppendLine("  AND RTRIM(CIB_HD.COACH_NO) = RTRIM(XX.GOUSYA) ");

                // ○増管理　降車ヶ所（ホテル込み）	T_CIB_KOUSYA_PLACE
                sqlString.AppendLine("LEFT JOIN  T_CIB_KOUSYA_PLACE CIB_KP ON RTRIM(CIB_KP.CRS_CD) = RTRIM(XX.CRS_CD) ");
                sqlString.AppendLine("  AND RTRIM(CIB_KP.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ");
                sqlString.AppendLine("  AND RTRIM(CIB_KP.COACH_NO) = RTRIM(XX.GOUSYA) ");
            }
            // WHERE句
            sqlString.AppendLine("WHERE ");
            sqlString.AppendLine("      NVL(XX.UNKYU_KBN,' ') != '" + UNKOU_KBN_HAISI + "' ");    // 運行区分＝ 'X'：運休(廃止)
            sqlString.AppendLine("  AND XX.YOYAKU_NG_FLG IS NULL ");                              // 予約不可フラグ = ブランク 
            sqlString.AppendLine("  AND NVL(XX.SAIKOU_KAKUTEI_KBN, ' ') != 'X' ");                // 催行確定区分 != 'X'：廃止

            // 通常受付開始日に値がない場合、コースを表示しないが、
            // 本社と予約センターのリーダー以上→表示
            if (UserInfoManagement.eigyosyoKbn == FixedCd.EigyosyoKbn.SystemPromote | UserInfoManagement.eigyosyoKbn == FixedCd.EigyosyoKbn.ReserveCenter)
            {
                s02_0101.UketukeStartDayChkFlg = false;
            }
            else
            {
                sqlString.AppendLine("  AND NVL(RTRIM(XX.UKETUKE_START_DAY),0) > 0 ");
            }  // かつ コース台帳（基本）.受付開始日= 0   

            if (paramList is object && paramList.Count > 0)
            {
                // 'パラメータ（SIngleコースコード）が設定されている場合
                // If Not String.IsNullOrEmpty(paramList.Item("SingleCrsCd").ToString.Trim) Then
                // sqlString.AppendLine("AND  XX.CRS_CD !='" & paramList.Item("SingleCrsCd").ToString.Trim & "' ")               'SIngleコースコード
                // End If
                // 'パラメータ（Twinコースコード）が設定されている場合
                // If Not String.IsNullOrEmpty(paramList.Item("TwinCrsCd").ToString.Trim) Then
                // sqlString.AppendLine("AND  XX.CRS_CD !='" & paramList.Item("TwinCrsCd").ToString.Trim & "' ")                  'Twinコースコード
                // End If

                // パラメータ（出発日：開始）が設定されている場合
                if (!string.IsNullOrEmpty(paramList["SyuptDayFrom"].ToString().Trim()) && Information.IsNumeric(paramList["SyuptDayFrom"]))
                {
                    sqlString.AppendLine("AND  XX.SYUPT_DAY >=" + Conversions.ToInteger(paramList["SyuptDayFrom"]) + " ");    // 出発日:開始
                }
                // パラメータ（出発日：終了）が設定されている場合
                if (!string.IsNullOrEmpty(paramList["SyuptDayTo"].ToString().Trim()) && Information.IsNumeric(paramList["SyuptDayFrom"]))
                {
                    sqlString.AppendLine("AND  XX.SYUPT_DAY <=" + Conversions.ToInteger(paramList["SyuptDayTo"]) + " ");      // 出発日:終了
                }
                // '   【出発年月From,Toとも当日で指定された場合】
                // If paramList.Item("SyuptDayFrom").ToString.Trim = paramList.Item("SyuptDayTo").ToString.Trim Then
                // sqlString.AppendLine("AND XX.SYUPT_TIME >=" & Now().Hour * 100 + Now().Minute & " ")                   '出発時間
                // End If

                // 《予約メニュー》からのパラメータ：検索可能コース ("1":すべて、"2":定期旅行）が "2":定期旅行 の場合に、コース台帳.定期企画区分＝1(:定期)のみを検索する。
                if (paramList["SearchKanouCrs"].ToString() == s02_0101.SearchCrs_Teiki)
                {
                    sqlString.AppendLine("AND  XX.TEIKI_KIKAKU_KBN ='" + TeikiKikakuKbn.Teiki + "' ");
                }
                // 【コースコード指定時】

                if (Conversions.ToBoolean(paramList["CrsCdAssigne"]) == true)                         // コースコードFull桁指定時
                {
                    sqlString.AppendLine("AND  XX.CRS_CD ='" + paramList["CrsCd"].ToString().Trim() + "' ");               // コースコード
                }
                else if (!string.IsNullOrEmpty(paramList["CrsCd"].ToString().Trim()))
                {
                    sqlString.AppendLine("AND  XX.CRS_CD LIKE '" + Strings.Trim(paramList["CrsCd"].ToString().Trim()) + "%' ");
                }

                // 【言語指定時】    'TODO:ガイド言語（分類コード決定後要修正）
                // コース台帳（基本）.ガイド言語 = 画面項目 : 言語（コード）
                if (!string.IsNullOrEmpty(paramList["GuidoGengo"].ToString().Trim()))
                {
                    sqlString.AppendLine("AND  XX.GUIDE_GENGO = '" + Strings.Trim(paramList["GuidoGengo"].ToString().Trim()) + "' ");
                }

                // 【フリーワード指定時】　　※複数ワード（文言をスペースで区切った場合）はAND検索とする。
                // ’FreeWord’ をスペース区切りで分割して配列に格納する
                if (!string.IsNullOrEmpty(paramList["FreeWord"].ToString().Trim()))
                {
                    var strArrayWord = paramList["FreeWord"].ToString().Trim().Replace("　", " ").Split(' ');
                    int idxForWord = 1;
                    foreach (string strWord in strArrayWord)
                    {
                        string freeWord = base.setParam("FREE_WORD" + idxForWord.ToString(), "%" + Strings.Trim(strWord) + "%", OracleDbType.Varchar2, 50);
                        // コース名   コースマスタ（基本）.コース名
                        sqlString.AppendLine("AND  ( XX.CRS_NAME LIKE " + freeWord + " ");
                        // 目的       コース台帳（基本）.カテゴリコード１～４
                        sqlString.AppendLine(" OR    CODE1.CODE_NAME LIKE " + freeWord + " ");
                        sqlString.AppendLine(" OR    CODE2.CODE_NAME LIKE " + freeWord + " ");
                        sqlString.AppendLine(" OR    CODE3.CODE_NAME LIKE " + freeWord + " ");
                        sqlString.AppendLine(" OR    CODE4.CODE_NAME LIKE " + freeWord + " ");
                        // 期間               コースマスタ（基本）.期間（文章）
                        sqlString.AppendLine(" OR    CRS_M.KIKAN_BUNSYO LIKE " + freeWord + " ");
                        // コースのポイント   コースマスタ（基本）.全体見所情報
                        sqlString.AppendLine(" OR    CRS_M.ZENTAI_MIDOKORO_INFO LIKE " + freeWord + " ");
                        // エリア
                        // 方面コード
                        // (コースが「企画」で、コース種別２が「日帰り(1)」の場合は
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '" + TeikiKikakuKbn.Kikaku + "' ");
                        sqlString.AppendLine("   AND XX.HOMEN_CD LIKE '%" + "4" + Strings.Trim(strWord) + "%' )");
                        // コース種別２が「宿泊(2)」の場合は
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '" + TeikiKikakuKbn.Kikaku + "' ");
                        sqlString.AppendLine("   AND XX.HOMEN_CD LIKE '%" + "5" + Strings.Trim(strWord) + "%' )");

                        // ダイヤ[定期].配車経由地１～５
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '1' ");
                        sqlString.AppendLine("   AND (   (DIA1.HAISYA_KEIYU_TI_1 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_1) = 1) ");
                        sqlString.AppendLine("        OR (DIA1.HAISYA_KEIYU_TI_2 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_2) = 1) ");
                        sqlString.AppendLine("        OR (DIA1.HAISYA_KEIYU_TI_3 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_3) = 1) ");
                        sqlString.AppendLine("       ) ");
                        sqlString.AppendLine("      )");
                        // ダイヤ[企画].配車経由地１～３
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '2' ");
                        sqlString.AppendLine("   AND (   (DIA2.HAISYA_KEIYU_TI_1 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_1) = 1) ");
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_2 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_2) = 1) ");
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_3 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_3) = 1) ");
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_4 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_4) = 1) ");
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_5 LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_5) = 1) ");
                        sqlString.AppendLine("       ) ");
                        sqlString.AppendLine("      )");

                        // ダイヤ[定期].ダイヤ.終了場所
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '1' ");
                        sqlString.AppendLine("   AND (   (DIA1.END_PLACE LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.END_PLACE) = 1) ");
                        sqlString.AppendLine("       ) ");
                        sqlString.AppendLine("      )");

                        // ダイヤ[企画].ダイヤ.終了場所
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '2' ");
                        sqlString.AppendLine("   AND (   (DIA2.END_PLACE LIKE " + freeWord + "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA2.END_PLACE) = 1) ");
                        sqlString.AppendLine("       ) ");
                        sqlString.AppendLine("      )");

                        // 出発時キャリア区分が "2":その他 の場合は 出発時間・キャリア が対象
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN <> '1' ");
                        sqlString.AppendLine("   AND CRS_M.SYUPT_TIME_CARRIER LIKE " + freeWord + "  )");
                        // 出発時キャリア区分が "1":バス の場合は 
                        // ①発場所コード・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ");
                        sqlString.AppendLine("   AND CRS_M.SYUPT_PLACE_CD_CARRIER LIKE " + freeWord + "  )");
                        // ②出発場所・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ");
                        sqlString.AppendLine("   AND CRS_M.SYUPT_PLACE_CARRIER LIKE " + freeWord + "  )");
                        // ③到着時間・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ");
                        sqlString.AppendLine("   AND CRS_M.TTYAK_TIME_CARRIER LIKE " + freeWord + "  )");
                        // ④到着場所コード・キャリアが対象
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ");
                        sqlString.AppendLine("   AND CRS_M.TTYAK_CD_CARRIER LIKE " + freeWord + "  )");

                        // 行程
                        // 降車ヶ所（ホテル込み）.行程種別		
                        sqlString.AppendLine(" OR   (CIB_KP.WORK_KIND = '1' AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" + FixedCdYoyaku.CodeBunruiTypeitIneraryKind + "' AND CODE_NAME LIKE " + freeWord + "  ) >= 1) ");
                        // 降車ヶ所（ホテル込み）.日次
                        if (strWord == "日次")
                        {
                            sqlString.AppendLine(" OR    CIB_KP.DAILY > 0 ");
                        }
                        // 降車ヶ所（ホテル込み）.種別
                        if (strWord == "0" | strWord == "仕入先マスタ" | strWord == "仕入先")
                        {
                            sqlString.AppendLine(" OR   CIB_KP.KIND ＝ 0 ");
                        }

                        if (strWord == "1" | strWord == "車窓マスタ" | strWord == "車窓")
                        {
                            sqlString.AppendLine(" OR   CIB_KP.KIND ＝ 1 ");
                        }

                        if (strWord == "2" | strWord == "場所マスタ" | strWord == "場所")
                        {
                            sqlString.AppendLine(" OR   (CIB_KP.KIND ＝2 ");
                        }
                        // 降車ヶ所（ホテル込み）.降車ヶ所(種別毎)
                        // 種別=0:仕入先マスタ  
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 0 ");
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_SIIRE_SAKI WHERE SIIRE_SAKI_NAME LIKE " + freeWord + "  ) >= 1) ");
                        // 種別=1:車窓マスタ （コード分類：64）
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 1 ");
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" + FixedCdYoyaku.CodeBunruiTypeCarWindowView + "' AND CODE_NAME LIKE " + freeWord + "  ) >= 1) ");
                        // 種別=2:場所マスタ
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 2 ");
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_PLACE WHERE CRS_JYOSYA_PLACE_FLG = 1 AND PLACE_NAME_1 LIKE " + freeWord + "  ) >= 1) ");
                        // 降車ヶ所（ホテル込み）.降車ヶ所枝番
                        // 降車ヶ所（ホテル込み）.備考
                        sqlString.AppendLine(" OR   (CIB_KP.BIKO LIKE " + freeWord + "  )");

                        // 食事詳細
                        // 食事種別
                        if (strWord == "朝食")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" + MealKindType.breakfast + "' ) ");
                        }
                        else if (strWord == "昼食")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" + MealKindType.lunch + "' ) ");
                        }
                        else if (strWord == "夕食")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" + MealKindType.dinner + "' ) ");
                        }
                        // 内容_バイキング
                        if (strWord == "バイキング")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_VAIKING = '1' ) ");
                        }
                        // 内容_会席料理
                        if (strWord == "会席料理" | strWord == "会席")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_KAISEKI = '1' ) ");
                        }
                        // 内容_弁当
                        if (strWord == "弁当")
                        {
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_BENTO = '1' ) ");
                        }
                        // 内容_その他
                        sqlString.AppendLine(" OR   (CIB_MD.NAIYO_OTHER LIKE " + freeWord + "  )");
                        // 内容_備考
                        sqlString.AppendLine(" OR   (CIB_MD.BIKO LIKE " + freeWord + "  )");

                        // 宿泊
                        // 宿泊詳細.和室有無
                        if (strWord == "和室有無" | strWord == "和室")
                        {
                            sqlString.AppendLine(" OR   (CIB_HD.WASITU_FLG = 1 ) ");
                        }
                        // 宿泊詳細.洋室有無
                        if (strWord == "洋室有無" | strWord == "洋室")
                        {
                            sqlString.AppendLine(" OR   (CIB_HD.YOSITU_FLG = 1 ) ");
                        }
                        // 宿泊詳細.和洋室有無
                        if (strWord == "和洋室有無" | strWord == "和洋室")
                        {
                            sqlString.AppendLine(" OR   (CIB_HD.WAYOSITU_FLG = 1 ) ");
                        }

                        // 宿泊詳細.設備１～６
                        sqlString.AppendLine(" OR   ((   CIB_HD.SETUBI_1 >= 1 OR CIB_HD.SETUBI_2 >= 1  OR CIB_HD.SETUBI_3 >= 1 "); // 
                        sqlString.AppendLine("        OR CIB_HD.SETUBI_4 >= 1 OR CIB_HD.SETUBI_5 >= 1  OR CIB_HD.SETUBI_5 >= 1 ) ");
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" + FixedCdYoyaku.CodeBunruiTypeHotelFacility + "' AND CODE_NAME LIKE " + freeWord + "  ) >= 1) ");
                        // 宿泊詳細.備考
                        sqlString.AppendLine(" OR   (CIB_HD.BIKO LIKE " + freeWord + "  )");
                        sqlString.AppendLine("     ) ");
                        idxForWord += 1;
                    }
                }
                // 【乗り場指定時】						
                if (!string.IsNullOrEmpty(paramList["NoribaCd"].ToString().Trim()))
                {
                    sqlString.AppendLine("AND  XX.HAISYA_KEIYU_CD = '" + Strings.Trim(paramList["NoribaCd"].ToString().Trim()) + "' ");    // 乗り場
                }

                // 【バスタイプ指定時】						
                if (!string.IsNullOrEmpty(Conversions.ToString(paramList["BusType"])))
                {
                    sqlString.AppendLine("AND  CODEBUS.CODE_VALUE = '" + Strings.Trim(paramList["BusType"].ToString().Trim()) + "' ");
                }

                // 【空席数指定時】						
                if (!string.IsNullOrEmpty(paramList["KusekiNum"].ToString().Trim()) && Information.IsNumeric(paramList["KusekiNum"].ToString().Trim()))
                {
                    sqlString.AppendLine("AND (  ");
                    sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " + JYOSYA_CAPACITY_BUS);
                    sqlString.AppendLine("    THEN ");
                    sqlString.AppendLine("      (NVL(ZSK.KUSEKI_NUM_TEISEKI,0) + NVL(ZSK.KUSEKI_NUM_SUB_SEAT,0)) ");
                    sqlString.AppendLine("    ELSE ");
                    sqlString.AppendLine("      (NVL(XX.KUSEKI_NUM_TEISEKI,0) + NVL(XX.KUSEKI_NUM_SUB_SEAT,0)) ");
                    sqlString.AppendLine("  END) >= " + Convert.ToInt32(paramList["KusekiNum"]) + " ");         // 空席数
                }

                // 【宿泊一人参加指定時】						
                // コース台帳（基本）.１名参加フラグ ≠ブランク						
                if (!string.IsNullOrEmpty(paramList["Stay1NinSanka"].ToString().Trim()) && Conversions.ToBoolean(paramList["Stay1NinSanka"]) == true)
                {
                    sqlString.AppendLine("AND RTRIM(XX.ONE_SANKA_FLG) IS NOT NULL ");  // '                                 '１名参加フラグ
                }

                // 【予約可能指定時】
                if (!string.IsNullOrEmpty(paramList["YoyakuKanouOnly"].ToString().Trim()) && Conversions.ToBoolean(paramList["YoyakuKanouOnly"]) == true)
                {
                    int intStartDay = CommonDateUtil.getSystemTime().ToString("yyyyMMdd"); // システム日付（YYYYMMDD）
                    sqlString.AppendLine("AND ( NVL(XX.TEJIMAI_KBN,' ') != '" + s02_0101.KbnValueY + "' ");                // 手仕舞区分 ≠'Y'
                    sqlString.AppendLine("AND   NVL(XX.YOYAKU_STOP_FLG,' ') = ' ' ");                                                   // 予約停止フラグ = ブランク
                    sqlString.AppendLine("AND   NVL(XX.UNKYU_KBN,' ') != '" + s02_0101.KbnValueY + "' ");   // 運休区分 ≠ 'Y'
                    sqlString.AppendLine("AND   NVL(XX.SAIKOU_KAKUTEI_KBN,' ') != '" + s02_0101.KbnValueN + "' ");  // 催行確定区分 ≠ 'N'
                    sqlString.AppendLine("AND   XX.SYUPT_DAY >='" + paramList["SyuptDayFrom"].ToString().Trim() + "' ");                                     // 出発日:開始'受付開始日 <= システム日付 
                    sqlString.AppendLine("AND   NVL(RTRIM(XX.UKETUKE_START_DAY),0) <= " + intStartDay + " ");                                   // 受付開始日 <= システム日付
                    sqlString.AppendLine("AND (NVL(XX.KUSEKI_NUM_TEISEKI,0) + NVL(XX.KUSEKI_NUM_SUB_SEAT,0)) > 0 ");                               // 空席数
                    sqlString.AppendLine("AND ( NVL(XX.CRS_KIND,' ') != '" + FixedCdYoyaku.CrsKind.kikakuStay + "'");  // {      コース種別 ≠'5'
                    sqlString.AppendLine(" OR ( NVL(XX.CRS_KIND,' ') = '" + FixedCdYoyaku.CrsKind.kikakuStay + "' ");  // または コース種別 = '5' かつ
                    sqlString.AppendLine(" And ( RTRIM(XX.TEIINSEI_FLG) IS NOT NULL ");                              // (  定員制フラグ ≠ ブランク または
                    sqlString.AppendLine("    OR NVL(XX.ROOM_ZANSU_SOKEI,0) <> 0 ))) ");                                      // 部屋残数総計 ≠ 0             )
                    sqlString.AppendLine(")");                                                                               // }
                }

                if (Conversions.ToBoolean(paramList["CrsCdAssigne"]) == true)
                {
                    // コースコードFull桁指定時、【日本語・外国語コース、コース区分指定】は無視
                    sqlString.AppendLine(" ");
                }
                else
                {
                    // 【日本語,外国語コース指定時】（日本語・外国語コース両方指定時、または両方未指定時は、抽出条件無し）
                    if (Conversions.ToBoolean(paramList["JapaneseCrs"]) != Conversions.ToBoolean(paramList["GaikokugoCrs"]))
                    {
                        // 【日本語コースのみ指定時】
                        if (!string.IsNullOrEmpty(paramList["JapaneseCrs"].ToString().Trim()) && Conversions.ToBoolean(paramList["JapaneseCrs"]) == true)
                        {
                            sqlString.AppendLine("AND   NVL(XX.HOUJIN_GAIKYAKU_KBN,' ') =  '" + FixedCd.HoujinGaikyakuKbnType.Houjin + "' ");   // 日本語外国語区分 = 日本語
                        }
                        // 【外国語コースのみ指定時】
                        if (!string.IsNullOrEmpty(paramList["GaikokugoCrs"].ToString().Trim()) && Conversions.ToBoolean(paramList["GaikokugoCrs"]) == true)
                        {
                            sqlString.AppendLine("AND   NVL(XX.HOUJIN_GAIKYAKU_KBN,' ')  =  '" + FixedCd.HoujinGaikyakuKbnType.Gaikyaku + "' ");  // 日本語外国語区分 = 外国語
                        }
                    }
                    // 【コース区分指定時】（全てのコース区分指定時、または全て未指定時は、抽出条件無し）
                    if (Conversions.ToBoolean(paramList["TeikiNoon"]) == true & Conversions.ToBoolean(paramList["TeikiNight"]) == true & Conversions.ToBoolean(paramList["TeikiKogai"]) == true & Conversions.ToBoolean(paramList["KikakuDayTrip"]) == true & Conversions.ToBoolean(paramList["KikakuStay"]) == true & Conversions.ToBoolean(paramList["KikakuTonaiR"]) == true & Conversions.ToBoolean(paramList["Capital"]) == true | Conversions.ToBoolean(paramList["TeikiNoon"]) == false & Conversions.ToBoolean(paramList["TeikiNight"]) == false & Conversions.ToBoolean(paramList["TeikiKogai"]) == false & Conversions.ToBoolean(paramList["KikakuDayTrip"]) == false & Conversions.ToBoolean(paramList["KikakuStay"]) == false & Conversions.ToBoolean(paramList["KikakuTonaiR"]) == false & Conversions.ToBoolean(paramList["Capital"]) == false)
                    {
                        sqlString.AppendLine(" ");
                    }
                    else
                    {
                        sqlString.AppendLine("AND ( 1 = 2 ");
                        // 【定期(昼) 指定時】						
                        // （　　　コース種別 = '1'						
                        // かつコース区分１ = '1'　　）						
                        if (!string.IsNullOrEmpty(paramList["TeikiNoon"].ToString().Trim()) && Conversions.ToBoolean(paramList["TeikiNoon"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.teiki + "' ) "); // コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_1,' ') =  '" + Common.FixedCd.CrsKbn1Type.noon + "' ");     // コース区分1 =昼
                            sqlString.AppendLine(" AND  NVL(XX.CRS_KBN_2,' ') =  '" + Common.FixedCd.crsKbn2Type.tonai + "' ) ");  // コース区分2 =都内
                        }
                        // 【定期(夜) 指定時】						
                        // （　　　コース種別 = '1'						
                        // かつコース区分１ = '2'　　）						
                        if (!string.IsNullOrEmpty(paramList["TeikiNight"].ToString().Trim()) && Conversions.ToBoolean(paramList["TeikiNight"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.teiki + "' ) "); // コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_1,' ') =  '" + Common.FixedCd.CrsKbn1Type.night + "' ");    // コース区分1 =夜
                            sqlString.AppendLine(" AND  NVL(XX.CRS_KBN_2,' ') =  '" + Common.FixedCd.crsKbn2Type.tonai + "' ) ");  // コース区分2 =都内
                        }
                        // 【定期(郊外) 指定時】						
                        // （　　　定期契約区分 = '1'						
                        // かつコース区分２ = '2'　　）						
                        if (!string.IsNullOrEmpty(paramList["TeikiKogai"].ToString().Trim()) && Conversions.ToBoolean(paramList["TeikiKogai"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.teiki + "' ) "); // コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_2,' ') =  '" + Common.FixedCd.crsKbn2Type.suburbs + "' ) "); // コース区分2 =郊外
                        }
                        // 【企画(日帰り) 指定時】						
                        // （　　　コース種別 = '4'　　）						
                        if (!string.IsNullOrEmpty(paramList["KikakuDayTrip"].ToString().Trim()) && Conversions.ToBoolean(paramList["KikakuDayTrip"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.kikakuHigaeri + "' ) "); // コース種別 = 企画（日帰り）
                        }
                        // 【企画(宿泊) 指定時】						
                        // （　　　コース種別 = '5'　　）						
                        if (!string.IsNullOrEmpty(paramList["KikakuStay"].ToString().Trim()) && Conversions.ToBoolean(paramList["KikakuStay"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.kikakuStay + "' ) "); // コース種別 = 企画（宿泊）
                        }
                        // 【企画(都内R) コース 指定時】						
                        // （　　　コース種別 = '6'　　）						
                        if (!string.IsNullOrEmpty(paramList["KikakuTonaiR"].ToString().Trim()) && Conversions.ToBoolean(paramList["KikakuTonaiR"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.kikakuTonaiR + "' ) "); // コース種別 = 企画（都内R）
                        }
                        // 【キャピタル 指定時】						
                        if (!string.IsNullOrEmpty(paramList["Capital"].ToString().Trim()) && Conversions.ToBoolean(paramList["Capital"]) == true)
                        {
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" + FixedCdYoyaku.CrsKind.capital + "' ) "); // コース種別 = キャピタル
                        }

                        sqlString.AppendLine(")");
                    }
                }
            }

            // ORDER BY句
            sqlString.AppendLine("ORDER BY  ");
            sqlString.AppendLine("  XX.SYUPT_DAY, XX.CRS_CD, XX.SYUPT_TIME, XX.GOUSYA ");       // 出発日、コースコード、出発時間（すべて昇順）
            return sqlString.ToString();
        }
    }

    /// <summary>
    /// 重複チェック用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string GetCourseDataByPrimaryKey(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        {
            var withBlock = ClsCrsDaityoEntity;
            sqlString.AppendLine("SELECT ");
            sqlString.AppendLine(" CRS.CRS_CD ");
            sqlString.AppendLine(",NVL(CRS.CRS_NAME_RK,CRS_NAME) AS CRS_NAME_RK ");
            sqlString.AppendLine(",CRS.TEIKI_KIKAKU_KBN ");

            // FROM句
            sqlString.AppendLine(" FROM ");
            sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS ");

            // WHERE句
            sqlString.AppendLine(" WHERE ");
            sqlString.AppendLine("     CRS.CRS_CD LIKE '" + paramList["CRS_CD"].ToString().Trim() + "%'");
            return sqlString.ToString();
        }
    }

    /// <summary>
    /// 重複チェック用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string GetCourseCdCheck(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        {
            var withBlock = ClsCrsDaityoEntity;
            sqlString.AppendLine("SELECT ");
            sqlString.AppendLine(" CRS.CRS_CD ");
            sqlString.AppendLine(",CRS.GOUSYA ");

            // FROM句
            sqlString.AppendLine(" FROM ");
            sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS ");

            // WHERE句
            sqlString.AppendLine(" WHERE ");
            sqlString.AppendLine("     CRS.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], withBlock.crsCd.DBType, withBlock.crsCd.IntegerBu, withBlock.crsCd.DecimalBu));
            return sqlString.ToString();
        }
    }

    /// <summary>
    /// 車種マスタデータ取得（バスタイプコンボボックス設定用）
    /// </summary>
    /// <param name="nullRecord"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable GetCarKindMasterDataCodeData(bool nullRecord = false, string addWhere = "")
    {
        var resultDataTable = new DataTable();
        string strSQL = "";
        try
        {

            // 空レコード挿入要否に従い、空行挿入
            if (nullRecord == true)
            {
                strSQL += "SELECT ' ' AS CODE_VALUE,'' AS CODE_NAME FROM DUAL UNION ";
            }

            strSQL += "SELECT RTRIM(CAR_CD) AS CODE_VALUE, CAR_NAME FROM M_CAR_KIND";
            strSQL += " WHERE DELETE_DATE IS NULL";
            if (!addWhere.Equals(string.Empty))
            {
                strSQL += " AND " + addWhere;
            }

            strSQL += " ORDER BY CODE_VALUE";
            resultDataTable = base.getDataTable(strSQL);
        }
        catch (Oracle.ManagedDataAccess.Client.OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultDataTable;
    }

    /// <summary>
    /// 場所マスタデータ取得（バスタイプコンボボックス設定用）
    /// </summary>
    /// <param name="nullRecord"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable GetPlaceMasterDataCodeData(bool nullRecord = false, string addWhere = "")
    {
        var resultDataTable = new DataTable();
        string strSQL = "";
        try
        {

            // 空レコード挿入要否に従い、空行挿入
            if (nullRecord == true)
            {
                strSQL += "SELECT ' ' AS CODE_VALUE,'' AS CODE_NAME FROM DUAL UNION ";
            }

            strSQL += "SELECT RTRIM(PLACE_CD) AS CODE_VALUE, PLACE_NAME_1 FROM M_PLACE";
            strSQL += " WHERE DELETE_DATE IS NULL";
            if (!addWhere.Equals(string.Empty))
            {
                strSQL += " AND " + addWhere;
            }

            strSQL += " ORDER BY CODE_VALUE";
            resultDataTable = base.getDataTable(strSQL);
        }
        catch (Oracle.ManagedDataAccess.Client.OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultDataTable;
    }

    /// <summary>
    /// 場所マスタデータ取得用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable GetPlaceMasterData(Hashtable paramList)
    {
        var resultDataTable = new DataTable();
        var sqlString = new StringBuilder();
        try
        {
            {
                var withBlock = ClsPlaceMasteroEntity;
                sqlString.AppendLine("SELECT ");
                sqlString.AppendLine("  PLACE_CD,");
                sqlString.AppendLine("  PLACE_NAME_1,");
                sqlString.AppendLine(" PLACE_NAME_2,");
                sqlString.AppendLine(" PLACE_NAME_RK,");
                sqlString.AppendLine(" PLACE_NAME_SHORT,");
                sqlString.AppendLine(" CRS_JYOSYA_PLACE_FLG,");
                sqlString.AppendLine(" ROSEN_KEIYUTI_FLG,");
                sqlString.AppendLine(" CRS_SYUGO_PLACE_FLG,");
                sqlString.AppendLine(" CARRIER_SYUPT_TTYAK_PLACE,");
                sqlString.AppendLine(" COUPON_STAY_TIIKI,");
                sqlString.AppendLine(" COMPANY_CD,");
                sqlString.AppendLine(" EIGYOSYO_CD,");
                sqlString.AppendLine(" HAISYA_KEIYU_TI_SEQ,");
                sqlString.AppendLine(" HSSYA_ALR_HAISYA_TIKU_CD,");
                sqlString.AppendLine(" HSSYA_ALR_HAISYA_TI_CD,");
                sqlString.AppendLine(" T_WEB_PLACE_CD,");
                sqlString.AppendLine(" K_WEB_PLACE_CD ");
                // FROM句
                sqlString.AppendLine(" FROM ");
                sqlString.AppendLine("M_PLACE ");
                // WHERE句
                sqlString.AppendLine(" WHERE ");
                sqlString.AppendLine("     PLACE_CD = " + setParam("PLACE_CD", paramList["PLACE_CD"], withBlock.PlaceCd.DBType, withBlock.PlaceCd.IntegerBu, withBlock.PlaceCd.DecimalBu));
                resultDataTable = base.getDataTable(sqlString.ToString());
            }
        }
        catch (Oracle.ManagedDataAccess.Client.OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultDataTable;
    }

    /// <summary>
    /// コース情報・追加情報取得
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns>DataTable</returns>
    /// <remarks></remarks>
    public DataTable GetCrsControlInfo(Hashtable paramList)
    {
        var resultDataTable = new DataTable();
        var sqlString = new StringBuilder();
        try
        {
            // パラメータ設定
            {
                var withBlock = ClsCrsContorolInfoEntity;
                // ＫＥＹ１
                setParam(withBlock.key1.PhysicsName, paramList(withBlock.key1.PhysicsName), withBlock.key1.DBType, withBlock.key1.IntegerBu, withBlock.key1.DecimalBu);
                // ＫＥＹ２
                setParam(withBlock.key2.PhysicsName, paramList(withBlock.key2.PhysicsName), withBlock.key2.DBType, withBlock.key2.IntegerBu, withBlock.key2.DecimalBu);
                sqlString.AppendLine("SELECT ");
                sqlString.AppendLine(" BIKO, ");
                sqlString.AppendLine(" DELETE_DAY, ");
                sqlString.AppendLine(" ENTRY_DAY, ");
                sqlString.AppendLine(" ENTRY_PERSON_CD, ");
                sqlString.AppendLine(" ENTRY_PGMID, ");
                sqlString.AppendLine(" ENTRY_TIME, ");
                sqlString.AppendLine(" FUZOKU_INFO_1, ");
                sqlString.AppendLine(" FUZOKU_INFO_2, ");
                sqlString.AppendLine(" FUZOKU_INFO_3, ");
                sqlString.AppendLine(" FUZOKU_INFO_4, ");
                sqlString.AppendLine(" KEY_1, ");
                sqlString.AppendLine(" KEY_2, ");
                sqlString.AppendLine(" KEY_3, ");
                sqlString.AppendLine(" UPDATE_DAY, ");
                sqlString.AppendLine(" UPDATE_PERSON_CD, ");
                sqlString.AppendLine(" UPDATE_PGMID, ");
                sqlString.AppendLine(" UPDATE_TIME, ");
                sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ");
                sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ");
                sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ");
                sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ");
                sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ");
                sqlString.AppendLine(" SYSTEM_UPDATE_DAY ");
                sqlString.AppendLine(" FROM ");
                sqlString.AppendLine(" T_CRS_CONTROL_INFO ");
                // WHERE句
                if (paramList is object && paramList.Count > 0)
                {
                    sqlString.AppendLine(" WHERE KEY_1 = :" + withBlock.key1.PhysicsName);
                    sqlString.AppendLine("   AND KEY_2 = :" + withBlock.key2.PhysicsName);
                }
            }

            resultDataTable = base.getDataTable(sqlString.ToString());
        }
        catch (Oracle.ManagedDataAccess.Client.OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultDataTable;
    }
    #endregion

}