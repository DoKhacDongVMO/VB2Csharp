using System.Linq;
using System.Text;

/// <summary>
/// 催行決定/中止連絡のDAクラス
/// </summary>
public partial class S03_0406DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;

    /// <summary>
    /// 通知種別＝'1'（催行決定）
    /// </summary>
    private const string NoticeTypeSaikouDecided = "1";

    /// <summary>
    /// 通知種別＝'2'（催行中止）
    /// </summary>
    private const string NoticeTypeSaikouStop = "2";
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 検索処理を呼び出す（予約番号の入力がない場合）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableWithoutYoyakuNo(S03_0406DASelectParam param)
    {

        // 照会テーブルEntityを作成
        var crsLedgerBasic = new CrsLedgerBasicEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();
        sb.AppendLine("WITH  CALC_PLACE_NUM AS ( ");
        sb.AppendLine("    SELECT ");
        sb.AppendLine("        SYUPT_DAY ");
        sb.AppendLine("       ,CRS_CD ");
        sb.AppendLine("       ,COUNT(KOSHAKASHO_CD) AS KOSHAKASHO_NUM ");
        sb.AppendLine("    FROM ( ");
        sb.AppendLine("          SELECT DISTINCT ");
        sb.AppendLine("                  KOSHAKASHO.SYUPT_DAY        AS SYUPT_DAY ");
        sb.AppendLine("                , KOSHAKASHO.CRS_CD           AS CRS_CD ");
        sb.AppendLine("                , KOSHAKASHO.KOSHAKASHO_CD    AS KOSHAKASHO_CD ");
        sb.AppendLine("          FROM ");
        sb.AppendLine("              T_CRS_LEDGER_KOSHAKASHO KOSHAKASHO ");
        sb.AppendLine("          INNER JOIN ");
        sb.AppendLine("              ( ");
        sb.AppendLine("                  SELECT ");
        sb.AppendLine("                      SIIRE_SAKI_CD ");
        sb.AppendLine("                    , SIIRE_SAKI_NO ");
        sb.AppendLine("                    , SIIRE_SAKI_KIND_CD ");
        sb.AppendLine("                    , DELETE_DATE ");
        sb.AppendLine("                  FROM ");
        sb.AppendLine("                      M_SIIRE_SAKI ");
        sb.AppendLine("                 WHERE ");
        sb.AppendLine("                     DELETE_DATE IS NULL");
        sb.AppendLine("               ) SIIRE ");
        sb.AppendLine("          ON KOSHAKASHO.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("          AND KOSHAKASHO.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("    WHERE ");
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '");
        // 50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '");
        // 99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '");
        // 35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ");
        sb.AppendLine("        AND DELETE_DAY IS NULL");
        sb.AppendLine("    UNION");
        sb.AppendLine("    SELECT DISTINCT ");
        sb.AppendLine("          HOTEL.SYUPT_DAY       AS SYUPT_DAY ");
        sb.AppendLine("        , HOTEL.CRS_CD          AS CRS_CD ");
        sb.AppendLine("        , HOTEL.SIIRE_SAKI_CD   AS KOSHAKASHO_CD ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("        T_CRS_LEDGER_HOTEL HOTEL ");
        sb.AppendLine("    INNER JOIN ");
        sb.AppendLine("    ( ");
        sb.AppendLine("       SELECT ");
        sb.AppendLine("             SIIRE_SAKI_CD ");
        sb.AppendLine("          , SIIRE_SAKI_NO ");
        sb.AppendLine("          , SIIRE_SAKI_KIND_CD ");
        sb.AppendLine("          , DELETE_DATE ");
        sb.AppendLine("      FROM ");
        sb.AppendLine("           M_SIIRE_SAKI ");
        sb.AppendLine("      WHERE ");
        sb.AppendLine("           DELETE_DATE IS NULL");
        sb.AppendLine("      ) SIIRE ");
        sb.AppendLine("        ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("        AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("    WHERE ");
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '");
        // 50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '");
        // 99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '");
        // 35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ");
        sb.AppendLine("    AND DELETE_DAY IS NULL");
        sb.AppendLine("    ) ");
        sb.AppendLine("    GROUP BY ");
        sb.AppendLine("        SYUPT_DAY");
        sb.AppendLine("      , CRS_CD");
        sb.AppendLine(" )");


        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("      TO_YYYYMMDD_FC(BASIC.SYUPT_DAY)                                     AS SYUPT_DAY_STR ");     // 出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY                                                     AS SYUPT_DAY ");         // 出発日
        sb.AppendLine("    , BASIC.CRS_CD                                                        AS CRS_CD ");            // コースコード
        sb.AppendLine("    , MAX(BASIC.CRS_NAME)                                                 AS CRS_NAME ");          // コース名
        sb.AppendLine("    , SUM(BASIC.YOYAKU_NUM_TEISEKI + BASIC.YOYAKU_NUM_SUB_SEAT)           AS YOYAKU_NUM ");        // 予約数定席 + 予約数補助席
        sb.AppendLine("    , MAX(CALC_PLACE_NUM.KOSHAKASHO_NUM)                                  AS GET_OUT_PLACE_NUM "); // 降車ヶ所コード
        sb.AppendLine("    , TO_YYYYMMDD_FC(MAX(BASIC.SAIKOU_DAY))                               AS SAIKOU_DAY ");        // 催行日
        sb.AppendLine("    , MAX(BASIC.BUS_RESERVE_CD)                                           AS BUS_RESERVE_CD  ");    // バス指定コード
        sb.AppendLine("    , COUNT(BASIC.GOUSYA)                                                 AS BUS_NUM  ");           // 号車

        // FROM句
        sb.AppendLine("FROM ");

        // コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC ");
        sb.AppendLine("    INNER JOIN  CALC_PLACE_NUM ");
        // 結合条件
        sb.AppendLine("    ON  BASIC.SYUPT_DAY = CALC_PLACE_NUM.SYUPT_DAY ");
        sb.AppendLine("    AND BASIC.CRS_CD = CALC_PLACE_NUM.CRS_CD ");

        // WHERE句
        sb.AppendLine("WHERE 1 = 1 ");

        // 定期・企画区分 =　2
        sb.AppendLine(" AND BASIC.TEIKI_KIKAKU_KBN = '2' ");
        // ○増管理区分 ≠M
        sb.AppendLine(" AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> 'M' ");
        // 削除日 = 0
        sb.AppendLine(" AND NVL(BASIC.DELETE_DAY, 0) = 0 ");

        // 催行可否登録日FROM
        if (param.SaikouEntryDayFrom is object)
        {
            sb.Append("    AND BASIC.SAIKOU_DAY >= ").AppendLine(setSelectParam(param.SaikouEntryDayFrom, crsLedgerBasic.saikouDay));
        }
        // 催行可否登録日TO
        if (param.SaikouEntryDayTo is object)
        {
            sb.Append("    AND BASIC.SAIKOU_DAY <= ").AppendLine(setSelectParam(param.SaikouEntryDayTo, crsLedgerBasic.saikouDay));
        }

        // 通知種別が１の場合
        if (string.Compare(param.NoticeKind, NoticeTypeSaikouDecided) == 0)
        {
            // 'Y'：催行決定
            sb.Append("    AND BASIC.SAIKOU_KAKUTEI_KBN = '").Append(FixedCd.SaikouKakuteiKbn.Saikou).AppendLine("' --'Y'：催行決定");
        }

        // 通知種別が２の場合
        if (string.Compare(param.NoticeKind, NoticeTypeSaikouStop) == 0)
        {
            // 'N'：催行中止
            sb.Append("    AND BASIC.SAIKOU_KAKUTEI_KBN = '").Append(FixedCd.SaikouKakuteiKbn.Tyushi).AppendLine("' --'N'：催行中止");
        }

        // コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            sb.Append("    AND BASIC.CRS_CD = ").AppendLine(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd));
        }

        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.Append("    AND BASIC.SYUPT_DAY >= ").AppendLine(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay));
        }
        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.Append("    AND BASIC.SYUPT_DAY <= ").AppendLine(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay));
        }

        // 邦人／外客区分
        if (param.CrsJapanese == true || param.CrsForeign == true)
        {
            sb.AppendLine("    AND (1 <> 1 ");
            if (param.CrsJapanese == true)
            {
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = '").Append(FixedCd.HoujinGaikyakuKbnType.Houjin).AppendLine("' --'H'：邦人");
            }

            if (param.CrsForeign == true)
            {
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = '").Append(FixedCd.HoujinGaikyakuKbnType.Gaikyaku).AppendLine("' --'G'：外客");
            }

            sb.AppendLine("        )");
        }

        // コース種別
        if (param.CrsDay == true || param.CrsStay == true || param.CrsRcourse == true)
        {
            sb.AppendLine("    AND (1 <> 1 ");
            // 企画（日帰り）
            if (param.CrsDay == true)
            {
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Higaeri).AppendLine("' --'4'：企画日帰り");
            }
            // 企画（宿泊）
            if (param.CrsStay == true)
            {
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Stay).AppendLine("' --'5'：企画宿泊");
            }
            // 企画（Ｒコース）
            if (param.CrsRcourse == true)
            {
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Kikaku).AppendLine("' --'6'：Rコース");
            }

            sb.AppendLine("        )");
        }

        // バス指定コード
        if (!string.IsNullOrEmpty(param.BusReserveCd))
        {
            sb.Append("    AND BASIC.BUS_RESERVE_CD = ").AppendLine(setSelectParam(param.BusReserveCd, crsLedgerBasic.busReserveCd));
        }
        // ソート条件
        sb.AppendLine("GROUP BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        // ソート条件
        sb.AppendLine("ORDER BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 検索処理を呼び出す（予約番号の入力がある場合）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableWithYoyakuNo(S03_0406DASelectParam param)
    {

        // 照会テーブルEntityを作成
        var yoyakuInfoBasic = new YoyakuInfoBasicEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();
        sb.AppendLine("WITH  CALC_PLACE_NUM AS ( ");
        sb.AppendLine("    SELECT ");
        sb.AppendLine("        SYUPT_DAY ");
        sb.AppendLine("       ,CRS_CD ");
        sb.AppendLine("       ,COUNT(KOSHAKASHO_CD) AS KOSHAKASHO_NUM ");
        sb.AppendLine("    FROM ( ");
        sb.AppendLine("          SELECT DISTINCT ");
        sb.AppendLine("                  KOSHAKASHO.SYUPT_DAY        AS SYUPT_DAY ");
        sb.AppendLine("                , KOSHAKASHO.CRS_CD           AS CRS_CD ");
        sb.AppendLine("                , KOSHAKASHO.KOSHAKASHO_CD    AS KOSHAKASHO_CD ");
        sb.AppendLine("          FROM ");
        sb.AppendLine("              T_CRS_LEDGER_KOSHAKASHO KOSHAKASHO ");
        sb.AppendLine("          INNER JOIN ");
        sb.AppendLine("              ( ");
        sb.AppendLine("                  SELECT ");
        sb.AppendLine("                      SIIRE_SAKI_CD ");
        sb.AppendLine("                    , SIIRE_SAKI_NO ");
        sb.AppendLine("                    , SIIRE_SAKI_KIND_CD ");
        sb.AppendLine("                    , DELETE_DATE ");
        sb.AppendLine("                  FROM ");
        sb.AppendLine("                      M_SIIRE_SAKI ");
        sb.AppendLine("                 WHERE ");
        sb.AppendLine("                     DELETE_DATE IS NULL");
        sb.AppendLine("               ) SIIRE ");
        sb.AppendLine("          ON KOSHAKASHO.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("          AND KOSHAKASHO.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("    WHERE ");
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '");
        // 50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '");
        // 99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '");
        // 35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ");
        sb.AppendLine("        AND DELETE_DAY IS NULL");
        sb.AppendLine("    UNION");
        sb.AppendLine("    SELECT DISTINCT ");
        sb.AppendLine("          HOTEL.SYUPT_DAY       AS SYUPT_DAY ");
        sb.AppendLine("        , HOTEL.CRS_CD          AS CRS_CD ");
        sb.AppendLine("        , HOTEL.SIIRE_SAKI_CD   AS KOSHAKASHO_CD ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("        T_CRS_LEDGER_HOTEL HOTEL ");
        sb.AppendLine("    INNER JOIN ");
        sb.AppendLine("    ( ");
        sb.AppendLine("       SELECT ");
        sb.AppendLine("             SIIRE_SAKI_CD ");
        sb.AppendLine("          , SIIRE_SAKI_NO ");
        sb.AppendLine("          , SIIRE_SAKI_KIND_CD ");
        sb.AppendLine("          , DELETE_DATE ");
        sb.AppendLine("      FROM ");
        sb.AppendLine("           M_SIIRE_SAKI ");
        sb.AppendLine("      WHERE ");
        sb.AppendLine("           DELETE_DATE IS NULL");
        sb.AppendLine("      ) SIIRE ");
        sb.AppendLine("        ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("        AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("    WHERE ");
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '");
        // 50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '");
        // 99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '");
        // 35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ");
        sb.AppendLine("    AND DELETE_DAY IS NULL");
        sb.AppendLine("    ) ");
        sb.AppendLine("    GROUP BY ");
        sb.AppendLine("        SYUPT_DAY");
        sb.AppendLine("      , CRS_CD");
        sb.AppendLine(" )");

        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("      TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ");      // 出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY                 AS SYUPT_DAY ");          // 出発日
        sb.AppendLine("    , BASIC.CRS_CD                    AS CRS_CD ");             // コースコード
        sb.AppendLine("    , MAX(BASIC.CRS_NAME)                  AS CRS_NAME ");           // コース名
        sb.AppendLine("    , SUM(BASIC.YOYAKU_NUM_TEISEKI + BASIC.YOYAKU_NUM_SUB_SEAT)          AS YOYAKU_NUM ");        // 予約数定席 + 予約数補助席
        sb.AppendLine("    , MAX(CALC_PLACE_NUM.KOSHAKASHO_NUM)    AS GET_OUT_PLACE_NUM ");
        sb.AppendLine("    , TO_YYYYMMDD_FC(MAX(BASIC.SAIKOU_DAY))                AS SAIKOU_DAY ");         // 催行日
        sb.AppendLine("    , MAX(BASIC.BUS_RESERVE_CD)            AS BUS_RESERVE_CD  ");     // バス指定コード
        sb.AppendLine("    , COUNT(BASIC.GOUSYA)              AS BUS_NUM  ");            // 号車
        sb.AppendLine("    , MAX(BASIC.SYUPT_TIME_1)              AS SYUPT_TIME ");         // 出発時間1
        sb.AppendLine("    , MAX(BASIC.SAIKOU_KAKUTEI_KBN)        AS SAIKOU_KAKUTEI_KBN "); // 催行確定区分
        sb.AppendLine("    , MAX(BASIC.TEIKI_KIKAKU_KBN)          AS TEIKI_KIKAKU_KBN ");   // 定期/企画区分
        sb.AppendLine("    , MAX(YOYAKU.CANCEL_FLG)               AS CANCEL_FLG ");         // キャンセルフラグ

        // FROM句
        sb.AppendLine("FROM ");

        // 予約情報(基本) YOYAKU
        sb.AppendLine("    T_YOYAKU_INFO_BASIC YOYAKU");
        // 内部結合
        sb.AppendLine("INNER JOIN  ");
        // コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC ");
        // 結合条件
        sb.AppendLine("    ON  YOYAKU.SYUPT_DAY = BASIC.SYUPT_DAY ");
        sb.AppendLine("    AND YOYAKU.CRS_CD = BASIC.CRS_CD ");
        // 内部結合
        sb.AppendLine("INNER JOIN ");
        sb.AppendLine("    CALC_PLACE_NUM ");
        // 結合条件
        sb.AppendLine("    ON  BASIC.SYUPT_DAY = CALC_PLACE_NUM.SYUPT_DAY ");
        sb.AppendLine("    AND BASIC.CRS_CD = CALC_PLACE_NUM.CRS_CD ");

        // WHERE句
        sb.AppendLine("WHERE 1 = 1 ");
        sb.AppendLine(" AND NVL(BASIC.DELETE_DAY, 0) = 0 ");
        if (!string.IsNullOrEmpty(param.YoyakuKbn))
        {
            // 予約区分
            sb.Append("    AND YOYAKU.YOYAKU_KBN = ").AppendLine(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn));
            // 予約ＮＯ
            sb.Append("    AND YOYAKU.YOYAKU_NO = ").AppendLine(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo));
        }

        // 集計条件
        sb.AppendLine("    GROUP BY ");
        sb.AppendLine("          BASIC.SYUPT_DAY ");
        sb.AppendLine("        , BASIC.CRS_CD ");

        // ソート条件
        sb.AppendLine("ORDER BY ");
        sb.AppendLine("      BASIC.SYUPT_DAY ");
        sb.AppendLine("    , BASIC.CRS_CD ");
        return base.getDataTable(sb.ToString());
    }

    private void clear()
    {
        base.paramClear();
        ParamNum = 0;
    }

    public string setSelectParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, true);
    }

    private string setParamEx(object value, IEntityKoumokuType ent, bool selFlg)
    {
        ParamNum += 1;
        if (selFlg == true && ent is EntityKoumoku_MojiType)
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType);
        }
        else
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType, ent.IntegerBu, ent.DecimalBu);
        }
    }
    #endregion

    #region  パラメータ 
    public partial class S03_0406DASelectParam
    {
        /// <summary>
        /// 催行可否登録日FROM
        /// </summary>
        public int? SaikouEntryDayFrom { get; set; }
        /// <summary>
        /// 催行可否登録日TO
        /// </summary>
        public int? SaikouEntryDayTo { get; set; }
        /// <summary>
        /// 通知種別
        /// </summary>
        public string NoticeKind { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? SyuptDayFrom { get; set; }
        /// <summary>
        /// 出発日TO
        /// </summary>
        public int? SyuptDayTo { get; set; }
        /// <summary>
        /// 日本語
        /// </summary>
        public bool CrsJapanese { get; set; }
        /// <summary>
        /// 外国語
        /// </summary>
        public bool CrsForeign { get; set; }
        /// <summary>
        /// 企画（日帰り）
        /// </summary>
        public bool CrsDay { get; set; }
        /// <summary>
        /// 企画（宿泊）
        /// </summary>
        public bool CrsStay { get; set; }
        /// <summary>
        /// 企画（Ｒコース）
        /// </summary>
        public bool CrsRcourse { get; set; }
        /// <summary>
        /// バス指定コード
        /// </summary>
        public string BusReserveCd { get; set; }
        /// <summary>
        /// 予約区分
        /// </summary>
        public string YoyakuKbn { get; set; }
        /// <summary>
        /// 予約ＮＯ
        /// </summary>
        public int YoyakuNo { get; set; }
    }
    #endregion
}