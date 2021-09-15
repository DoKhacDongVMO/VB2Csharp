using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic


/// <summary>
/// 催行可否照会／入力のDAクラス
/// </summary>
public partial class SaikouKahiInput_DA : DataAccessorBase
{

    #region  定数／変数 

    public enum accessType : int
    {
        getSaikouKahiInput,                   // 一覧結果取得検索
        getUsingFlgCrs,                       // 使用中フラグチェック用検索
        getSaikouKakuteiInput,                // 催行確定区分チェック用検索
        getUsingFlgData                      // 使用中フラグ対象取得用
    }

    private string tejimaiSumi = "Y";      // 手仕舞い済

    public partial struct USINGFLG_PARAMKEY
    {
        public const string WHEREIND = "WHEREIN";
    }



    #endregion

    #region  SELECT処理 

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessSaikouKahiInputTehai(accessType type, Hashtable paramInfoList = null)
    {

        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case accessType.getSaikouKahiInput:
                {
                    // 一覧結果取得検索
                    sqlString = getSaikouKahiInput(paramInfoList);
                    break;
                }

            case accessType.getUsingFlgCrs:
                {
                    // 使用中フラグチェック用検索
                    sqlString = getUsingFlgCrs(paramInfoList);
                    break;
                }

            case accessType.getSaikouKakuteiInput:
                {
                    // 催行確定区分チェック用検索
                    sqlString = getSaikouKakuteiInput(paramInfoList);
                    break;
                }

            case accessType.getUsingFlgData:
                {
                    // 使用中フラグ対象用検索
                    sqlString = getUsingFlgData(paramInfoList);
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
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getSaikouKahiInput(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SELECT句
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ");  // 出発日
        sqlString.AppendLine(",BASIC.CRS_CD ");                                              // コースコード
        sqlString.AppendLine(",BASIC.CRS_NAME ");                                            // コース名
        sqlString.AppendLine(",BASIC.YOBI_CD ");                                             // 曜日コード
        sqlString.AppendLine(",0 AS CHK_FLG ");                                              // 変更フラグ
        sqlString.AppendLine(",Case WHEN BASIC.SAIKOU_KAKUTEI_KBN = ' ' THEN ");
        sqlString.AppendLine("'' ");
        sqlString.AppendLine("   WHEN BASIC.SAIKOU_KAKUTEI_KBN = " + " '" + FixedCd.SaikouKakuteiKbn.Saikou + "' " + " THEN ");
        sqlString.AppendLine("'" + FixedCd.Saikoukahi.Saikou + "' ");
        sqlString.AppendLine("   WHEN BASIC.SAIKOU_KAKUTEI_KBN = " + " '" + FixedCd.SaikouKakuteiKbn.Tyushi + "' " + " THEN ");
        sqlString.AppendLine("'" + FixedCd.Saikoukahi.Tyushi + "' ");
        sqlString.AppendLine("   Else ' ' END AS SAIKOU_KAKUTEI_KBN ");                      // 催行確定区分
        sqlString.AppendLine(",BASIC.TEJIMAI_KBN ");                                         // 手仕舞区分
        sqlString.AppendLine(",BASIC.MIN_SAIKOU_NINZU ");                                    // 最小催行人数
        sqlString.AppendLine(",CRSLEDGERBASIC.KUSEKI_NUM_KEI ");                             // 空席数
        sqlString.AppendLine(",CRSLEDGERBASIC.YOYAKU_NUM_KEI ");                             // 予約数
        sqlString.AppendLine(",CRSLEDGERBASIC.ROOM_ZANSU_SOKEI ");                           // 部屋残数総計
        sqlString.AppendLine(",CRSLEDGERBASIC.YOYAKU_ALREADY_ROOM_NUM ");                    // 予約済ＲＯＯＭ数
        sqlString.AppendLine(",BASIC.BUS_RESERVE_CD ");                                      // バス指定コード
        sqlString.AppendLine(",CRSLEDGERBASIC.BUS_COUNT_FLG ");                              // 台数カウントフラグ
        sqlString.AppendLine(",BASIC.MARU_ZOU_MANAGEMENT_KBN ");                             // 〇増管理区分
        sqlString.AppendLine(",BASIC.TEJIMAI_CONTACT_KBN ");                                 // 手仕舞連絡区分
        sqlString.AppendLine(",BASIC.UNKYU_KBN ");                                           // 運休区分
        sqlString.AppendLine(",'詳細' AS MORE_DETAIL ");                                     // 詳細ボタン
        sqlString.AppendLine(",' ' AS USING_FLG ");                                          // 使用中フラグ
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ");                                    // 定期・企画区分
        sqlString.AppendLine(",BASIC.HOUJIN_GAIKYAKU_KBN ");                                 // 邦人／外客区分
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ");                                    // 変更可否区分
        // FROM句
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ");
        sqlString.AppendLine(",(SELECT SUM(NVL(CRSBASIC.KUSEKI_NUM_TEISEKI,0) + NVL(CRSBASIC.KUSEKI_NUM_SUB_SEAT,0)) AS KUSEKI_NUM_KEI ,SUM(NVL(CRSBASIC.YOYAKU_NUM_TEISEKI, 0) + NVL(CRSBASIC.YOYAKU_NUM_SUB_SEAT, 0)) AS YOYAKU_NUM_KEI ");
        sqlString.AppendLine(",SUM(NVL(CRSBASIC.ROOM_ZANSU_SOKEI, 0)) AS ROOM_ZANSU_SOKEI ,SUM(NVL(CRSBASIC.YOYAKU_ALREADY_ROOM_NUM, 0)) AS YOYAKU_ALREADY_ROOM_NUM ");
        sqlString.AppendLine(",COUNT(CRSBASIC.BUS_COUNT_FLG) AS BUS_COUNT_FLG ,CRSBASIC.SYUPT_DAY AS SYUPT_DAY ,CRSBASIC.CRS_CD AS CRS_CD ");
        sqlString.AppendLine(" FROM T_CRS_LEDGER_BASIC CRSBASIC ");
        sqlString.AppendLine(" WHERE ");
        if ((Conversions.ToString(paramList["SYUPT_DAY_FROM"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["SYUPT_DAY_TO"]) ?? "") == (string.Empty ?? ""))
        {
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY_FROM"], OracleDbType.Decimal, 8, 0));
        }
        else if ((Conversions.ToString(paramList["SYUPT_DAY_FROM"]) ?? "") == (string.Empty ?? "") && (Conversions.ToString(paramList["SYUPT_DAY_TO"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY_TO"], OracleDbType.Decimal, 8, 0));
        }
        else
        {
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY between " + setParam("SYUPT_DAY_FROM", paramList["SYUPT_DAY_FROM"], OracleDbType.Decimal, 8, 0));
            sqlString.AppendLine(" AND " + setParam("SYUPT_DAY_TO", paramList["SYUPT_DAY_TO"], OracleDbType.Decimal, 8, 0));
        }

        if ((Conversions.ToString(paramList["CRS_CD"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            // sqlString.AppendLine(" ( ")
            sqlString.AppendLine(" CRSBASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
            // sqlString.AppendLine(" OR ")
            // sqlString.AppendLine(" CRSBASIC.BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            // sqlString.AppendLine(" ) ")
        }

        if ((Conversions.ToString(paramList["CRS_NAME_KANA"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            sqlString.AppendLine(" CRSBASIC.CRS_NAME_KANA LIKE '%'||" + setParam("CRS_NAME_KANA", paramList["CRS_NAME_KANA"], OracleDbType.Varchar2) + "||'%'");
        }

        if ((Conversions.ToString(paramList["BUS_RESERVE_CD"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            sqlString.AppendLine(" CRSBASIC.BUS_RESERVE_CD = " + setParam("BUS_RESERVE_CD", paramList["BUS_RESERVE_CD"], OracleDbType.Char));
        }

        switch (Conversions.ToString(paramList["SAIKOU_KAKUTEI_KBN"]) ?? "")
        {
            case var @case when @case == (string.Empty ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" (CRSBASIC.SAIKOU_KAKUTEI_KBN <> '" + FixedCd.SaikouKakuteiKbn.Haishi + "'");
                    sqlString.AppendLine(" OR ");
                    sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN Is NULL) ");
                    break;
                }

            case var case1 when case1 == FixedCd.SaikouKakuteiKbn.Saikou:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
                    break;
                }

            case var case2 when case2 == FixedCd.SaikouKakuteiKbn.Tyushi:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
                    break;
                }

            default:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" NVL(CRSBASIC.SAIKOU_KAKUTEI_KBN, '*') NOT IN ('" + FixedCd.SaikouKakuteiKbn.Tyushi + "', '" + FixedCd.SaikouKakuteiKbn.Haishi + "', '" + FixedCd.SaikouKakuteiKbn.Saikou + "') ");
                    break;
                }
        }

        switch (Conversions.ToString(paramList["TEJIMAI_KBN"]) ?? "")
        {
            case var case3 when case3 == (string.Empty ?? ""):
                {
                    break;
                }

            case var case4 when case4 == (tejimaiSumi ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" CRSBASIC.TEJIMAI_KBN = " + setParam("TEJIMAI_KBN", paramList["TEJIMAI_KBN"], OracleDbType.Char));
                    break;
                }

            case var case5 when case5 == (Conversions.ToString(FixedCd.TejimaiKbn.Mi) ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" CRSBASIC.TEJIMAI_KBN IS NULL ");
                    break;
                }
        }

        if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN IN (" + setParam("HOUJIN_KBN", paramList["HOUJIN_KBN"], OracleDbType.Char) + "," + setParam("GAIKYAKU_KBN", paramList["GAIKYAKU_KBN"], OracleDbType.Char) + ")");
        }
        else if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") == (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN = " + setParam("GAIKYAKU_KBN", paramList["GAIKYAKU_KBN"], OracleDbType.Char));
        }
        else if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") == (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN = " + setParam("HOUJIN_KBN", paramList["HOUJIN_KBN"], OracleDbType.Char));
        }

        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" (CRSBASIC.MARU_ZOU_MANAGEMENT_KBN = '" + string.Empty + "'");
        sqlString.AppendLine(" OR ");
        sqlString.AppendLine(" CRSBASIC.MARU_ZOU_MANAGEMENT_KBN Is NULL) ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(CRSBASIC.DELETE_DAY,0) = 0 ");
        sqlString.AppendLine(" AND ");
        // sqlString.AppendLine(" (( CRSBASIC.CRS_CD = CRSBASIC.BUS_RESERVE_CD And ")
        sqlString.AppendLine("CRSBASIC.CRS_KIND IN ('" + FixedCd.CrsKindType.higaeri + "','" + FixedCd.CrsKindType.stay + "','" + FixedCd.CrsKindType.rcourse + "')");
        // sqlString.AppendLine(" ) OR ")
        // sqlString.AppendLine(" CRSBASIC.CRS_CD <> CRSBASIC.BUS_RESERVE_CD ")
        // sqlString.AppendLine(" ) ")
        // If CType(paramList.Item("BUS_RESERVE_CD"), String) <> String.Empty Then
        // sqlString.AppendLine(" AND CRSBASIC.BUS_COUNT_FLG = '" & FixedCd.BusCountFlg.Count & "'")
        // End If
        sqlString.AppendLine(" AND NVL(CRSBASIC.DELETE_DAY,0) = 0 ");
        sqlString.AppendLine(" GROUP BY CRSBASIC.SYUPT_DAY , CRSBASIC.CRS_CD) CRSLEDGERBASIC ");
        // WHERE句
        sqlString.AppendLine(" WHERE ");
        if ((Conversions.ToString(paramList["SYUPT_DAY_FROM"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["SYUPT_DAY_TO"]) ?? "") == (string.Empty ?? ""))
        {
            sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY_FROM"], OracleDbType.Decimal, 8, 0));
        }
        else if ((Conversions.ToString(paramList["SYUPT_DAY_FROM"]) ?? "") == (string.Empty ?? "") && (Conversions.ToString(paramList["SYUPT_DAY_TO"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY_TO"], OracleDbType.Decimal, 8, 0));
        }
        else
        {
            sqlString.AppendLine(" BASIC.SYUPT_DAY between " + setParam("SYUPT_DAY_FROM", paramList["SYUPT_DAY_FROM"], OracleDbType.Decimal, 8, 0));
            sqlString.AppendLine(" AND " + setParam("SYUPT_DAY_TO", paramList["SYUPT_DAY_TO"], OracleDbType.Decimal, 8, 0));
        }

        if ((Conversions.ToString(paramList["CRS_CD"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            // sqlString.AppendLine(" ( ")
            sqlString.AppendLine(" BASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
            // sqlString.AppendLine(" OR ( ")
            // sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            // sqlString.AppendLine(" AND ")
            // sqlString.AppendLine(" BASIC.CRS_CD <> " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            // sqlString.AppendLine(" )) ")
        }

        if ((Conversions.ToString(paramList["CRS_NAME_KANA"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            sqlString.AppendLine(" BASIC.CRS_NAME_KANA  LIKE '%'||" + setParam("CRS_NAME_KANA", paramList["CRS_NAME_KANA"], OracleDbType.Varchar2) + "||'%'");
        }

        if ((Conversions.ToString(paramList["BUS_RESERVE_CD"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND ");
            sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " + setParam("BUS_RESERVE_CD", paramList["BUS_RESERVE_CD"], OracleDbType.Char));
        }

        switch (Conversions.ToString(paramList["SAIKOU_KAKUTEI_KBN"]) ?? "")
        {
            case var case6 when case6 == (string.Empty ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" (BASIC.SAIKOU_KAKUTEI_KBN <> '" + FixedCd.SaikouKakuteiKbn.Haishi + "'");
                    sqlString.AppendLine(" OR ");
                    sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN IS NULL) ");
                    break;
                }

            case var case7 when case7 == FixedCd.SaikouKakuteiKbn.Saikou:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
                    break;
                }

            case var case8 when case8 == FixedCd.SaikouKakuteiKbn.Tyushi:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
                    break;
                }

            default:
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') NOT IN ('" + FixedCd.SaikouKakuteiKbn.Tyushi + "', '" + FixedCd.SaikouKakuteiKbn.Haishi + "', '" + FixedCd.SaikouKakuteiKbn.Saikou + "') ");
                    break;
                }
        }

        switch (Conversions.ToString(paramList["TEJIMAI_KBN"]) ?? "")
        {
            case var case9 when case9 == (string.Empty ?? ""):
                {
                    break;
                }

            case var case10 when case10 == (tejimaiSumi ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" BASIC.TEJIMAI_KBN = " + setParam("TEJIMAI_KBN", paramList["TEJIMAI_KBN"], OracleDbType.Char));
                    break;
                }

            case var case11 when case11 == (Conversions.ToString(FixedCd.TejimaiKbn.Mi) ?? ""):
                {
                    sqlString.AppendLine(" AND ");
                    sqlString.AppendLine(" BASIC.TEJIMAI_KBN IS NULL ");
                    break;
                }
        }

        if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN IN (" + setParam("HOUJIN_KBN", paramList["HOUJIN_KBN"], OracleDbType.Char) + "," + setParam("GAIKYAKU_KBN", paramList["GAIKYAKU_KBN"], OracleDbType.Char) + ")");
        }
        else if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") == (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") != (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN = " + setParam("GAIKYAKU_KBN", paramList["GAIKYAKU_KBN"], OracleDbType.Char));
        }
        else if ((Conversions.ToString(paramList["HOUJIN_KBN"]) ?? "") != (string.Empty ?? "") && (Conversions.ToString(paramList["GAIKYAKU_KBN"]) ?? "") == (string.Empty ?? ""))
        {
            sqlString.AppendLine(" AND");
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN = " + setParam("HOUJIN_KBN", paramList["HOUJIN_KBN"], OracleDbType.Char));
        }

        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" (BASIC.MARU_ZOU_MANAGEMENT_KBN = '" + string.Empty + "'");
        sqlString.AppendLine(" OR ");
        sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN Is NULL) ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ");
        sqlString.AppendLine(" AND ");
        // sqlString.AppendLine(" (( BASIC.CRS_CD = BASIC.BUS_RESERVE_CD And")
        sqlString.AppendLine(" BASIC.CRS_KIND IN ('" + FixedCd.CrsKindType.higaeri + "','" + FixedCd.CrsKindType.stay + "','" + FixedCd.CrsKindType.rcourse + "')");
        // sqlString.AppendLine(" ) OR ")
        // sqlString.AppendLine(" BASIC.CRS_CD <> BASIC.BUS_RESERVE_CD ")
        // sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = CRSLEDGERBASIC.SYUPT_DAY ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.CRS_CD = CRSLEDGERBASIC.CRS_CD ");
        // GROUP BY句
        sqlString.AppendLine(" GROUP BY ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY ,BASIC.CRS_CD ,BASIC.CRS_NAME ,BASIC.YOBI_CD ,BASIC.SAIKOU_KAKUTEI_KBN ");
        sqlString.AppendLine(" ,BASIC.TEJIMAI_KBN ,BASIC.MIN_SAIKOU_NINZU ,CRSLEDGERBASIC.KUSEKI_NUM_KEI ,CRSLEDGERBASIC.YOYAKU_NUM_KEI ");
        sqlString.AppendLine(" ,CRSLEDGERBASIC.ROOM_ZANSU_SOKEI ,CRSLEDGERBASIC.YOYAKU_ALREADY_ROOM_NUM ,BASIC.BUS_RESERVE_CD ,CRSLEDGERBASIC.BUS_COUNT_FLG ");
        sqlString.AppendLine(" ,BASIC.MARU_ZOU_MANAGEMENT_KBN ,BASIC.TEJIMAI_CONTACT_KBN ,BASIC.UNKYU_KBN ,BASIC.TEIKI_KIKAKU_KBN ");
        sqlString.AppendLine(" ,BASIC.HOUJIN_GAIKYAKU_KBN ");
        // ORDER BY句
        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY ");
        sqlString.AppendLine(" ,BASIC.CRS_CD ");
        return sqlString.ToString();
    }

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getUsingFlgCrs(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SELECT句
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY ");   // 出発日
        sqlString.AppendLine(",BASIC.CRS_CD ");      // コースコード
        sqlString.AppendLine(",BASIC.GOUSYA ");      // 号車
        // FROM句
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ");
        // WHERE句
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" And ");
        sqlString.AppendLine(" BASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        sqlString.AppendLine(" And ");
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ");
        // ORDER BY句
        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine(" BASIC.GOUSYA ");
        return sqlString.ToString();
    }

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getSaikouKakuteiInput(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SELECT句
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ");  // 出発日
        sqlString.AppendLine(",BASIC.CRS_CD ");                                              // コースコード
        sqlString.AppendLine(",BASIC.SAIKOU_KAKUTEI_KBN ");                                  // 催行確定区分
        sqlString.AppendLine(",BASIC.TEJIMAI_KBN ");                                         // 手仕舞区分
        sqlString.AppendLine(",BASIC.BUS_RESERVE_CD ");                                      // バス指定コード
        // FROM句
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ");
        // WHERE句
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " + setParam("BUS_RESERVE_CD", paramList["BUS_RESERVE_CD"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.CRS_CD <> " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.TEJIMAI_KBN = '" + FixedCd.TejimaiKbn.Zumi + "'");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL( BASIC.DELETE_DAY,0) = 0 ");
        return sqlString.ToString();
    }

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getUsingFlgData(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SELECT句
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine("   SYUPT_DAY ");                   // 出発日
        sqlString.AppendLine(" , CRS_CD ");                      // コースコード
        sqlString.AppendLine(" , GOUSYA ");                      // 号車
        sqlString.AppendLine(" , BUS_RESERVE_CD ");              // バス指定コード
        sqlString.AppendLine(" , SYSTEM_UPDATE_DAY ");           // システム更新日
        sqlString.AppendLine(" , SYSTEM_UPDATE_PGMID ");         // システム更新PGMID
        sqlString.AppendLine(" , SYSTEM_UPDATE_PERSON_CD ");     // システム更新者
        sqlString.AppendLine(" , USING_FLG ");                   // 使用中フラグ

        // FROM句
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC ");

        // WHERE句
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" (SYUPT_DAY, CRS_CD) IN (");
        sqlString.AppendLine("  " + Conversions.ToString(paramList[USINGFLG_PARAMKEY.WHEREIND]));
        sqlString.AppendLine(" )");
        // 削除済みは除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(DELETE_DAY, 0) = 0 ");
        // ○増は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" MARU_ZOU_MANAGEMENT_KBN IS NULL ");
        // 定期は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" TEIKI_KIKAKU_KBN = '" + TeikiKikakuKbn.Kikaku + "' ");
        // 廃止は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(SAIKOU_KAKUTEI_KBN, '*') <> '" + FixedCd.SaikouKakuteiKbn.Haishi + "'");

        // ORDER BY句
        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine("   SYUPT_DAY, CRS_CD, GOUSYA ");
        return sqlString.ToString();
    }

    #endregion

    #region  UPDATE処理 

    /// <summary>
    /// DB接続用
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public int updateSaikouKahi(Hashtable paramInfoList)
    {
        OracleTransaction oracleTransaction = default;
        int returnValue = 0;
        string sqlString = string.Empty;
        try
        {

            // トランザクション開始
            oracleTransaction = callBeginTransaction();
            for (int i = 1; i <= 2; i++)
            {
                switch (i)
                {
                    case 1:  // コース台帳（基本）
                        {
                            sqlString = executeUpdateBasicData(paramInfoList);
                            break;
                        }

                    case 2:  // 催行決定コース履歴
                        {
                            sqlString = executeInsertCrsHistoryData(paramInfoList);
                            break;
                        }
                }

                returnValue += execNonQuery(oracleTransaction, sqlString);
            }

            if (returnValue > 0)
            {
                // コミット
                callCommitTransaction(oracleTransaction);
            }
            else
            {
                // ロールバック
                callRollbackTransaction(oracleTransaction);
            }
        }
        catch (Exception ex)
        {
            callRollbackTransaction(oracleTransaction);
            throw;
        }
        finally
        {
            oracleTransaction.Dispose();
        }

        return returnValue;
    }

    /// <summary>
    /// 使用中フラグ更新
    /// </summary>
    /// <param name="selectData"></param>
    /// <param name="systemupdatepgmid"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable executeUsingFlgCrs(DataTable selectData, string systemupdatepgmid)
    {
        var totalValue = new DataTable();
        OracleTransaction trn = default;
        bool returnValue = false;
        try
        {
            totalValue.Columns.Add("USING_FLG");             // 使用中フラグ

            // トランザクション開始
            trn = callBeginTransaction();
            foreach (DataRow row in selectData.Rows)
            {
                string syuptsday = Conversions.ToString(row("SYUPT_DAY"));                   // 出発日
                string crscd = Conversions.ToString(row("CRS_CD"));                          // コースコード
                string gousya = Conversions.ToString(row("GOUSYA"));                         // 号車
                returnValue = CommonCheckUtil.setUsingFlg_Crs(syuptsday, crscd, gousya, systemupdatepgmid, trn, true);
                DataRow row2 = totalValue.NewRow;
                if (returnValue == true)
                {
                    row2("USING_FLG") = FixedCd.UsingFlg.Use;
                }
                else
                {
                    row2("USING_FLG") = string.Empty;
                }

                totalValue.Rows.Add(row2);
            }

            // コミット
            callCommitTransaction(trn);
        }
        catch (Exception ex)
        {
            // ロールバック
            callRollbackTransaction(trn);
            throw;
        }
        finally
        {
            trn.Dispose();
        }

        return totalValue;
    }

    /// <summary>
    /// コース台帳（基本）：データ更新用
    /// </summary>
    /// <param name="paramList">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeUpdateBasicData(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ");
        sqlString.AppendLine(" SET ");
        sqlString.AppendLine(" SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
        sqlString.AppendLine(",SAIKOU_DAY = " + setParam("SAIKOU_DAY", paramList["SAIKOU_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(",TEJIMAI_KBN = " + setParam("TEJIMAI_KBN", paramList["TEJIMAI_KBN"], OracleDbType.Char));
        sqlString.AppendLine(",TEJIMAI_DAY = " + setParam("TEJIMAI_DAY", paramList["TEJIMAI_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " + setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date));
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " + setParam("SYSTEM_UPDATE_PGMID", paramList["SYSTEM_UPDATE_PGMID"], OracleDbType.Char));
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " + setParam("SYSTEM_UPDATE_PERSON_CD", paramList["SYSTEM_UPDATE_PERSON_CD"], OracleDbType.Varchar2));
        // WHERE句
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        // 削除済みは除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(DELETE_DAY, 0) = 0 ");
        // ○増は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" MARU_ZOU_MANAGEMENT_KBN IS NULL ");
        // 定期は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" TEIKI_KIKAKU_KBN = '" + TeikiKikakuKbn.Kikaku + "' ");
        // 廃止は除く
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(SAIKOU_KAKUTEI_KBN, '*') <> '" + FixedCd.SaikouKakuteiKbn.Haishi + "'");
        return sqlString.ToString();
    }

    /// <summary>
    /// 催行決定コース履歴：データ登録用
    /// </summary>
    /// <param name="paramList">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeInsertCrsHistoryData(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // INSERT
        sqlString.AppendLine(" INSERT INTO T_SAIKOU_DECISION_CRS_HISTORY ");
        sqlString.AppendLine(" ( ");
        sqlString.AppendLine(" CRS_CD ");
        sqlString.AppendLine(",CRS_NAME ");
        sqlString.AppendLine(",SYUPT_DAY ");
        sqlString.AppendLine(",SAIKOU_KAKUTEI_KBN ");
        sqlString.AppendLine(",TEJIMAI_KBN ");
        sqlString.AppendLine(",TEJIMAI_CONTACT_KBN ");
        sqlString.AppendLine(",MARU_ZOU_MANAGEMENT_KBN ");
        sqlString.AppendLine(",UNKYU_KBN ");
        sqlString.AppendLine(",DELETE_DAY ");
        sqlString.AppendLine(",SYSTEM_ENTRY_DAY ");
        sqlString.AppendLine(",SYSTEM_ENTRY_PERSON_CD ");
        sqlString.AppendLine(",SYSTEM_ENTRY_PGMID ");
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ");
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ");
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ");
        sqlString.AppendLine(" ) ");
        // VALUE
        sqlString.AppendLine(" VALUES ");
        sqlString.AppendLine(" ( ");
        sqlString.AppendLine(setParam("CRS_CD", Conversions.ToString(paramList["CRS_CD"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("CRS_NAME", Conversions.ToString(paramList["CRS_NAME"]), OracleDbType.Varchar2));
        sqlString.AppendLine("," + setParam("SYUPT_DAY", Conversions.ToDecimal(paramList["SYUPT_DAY"]), OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine("," + setParam("SAIKOU_KAKUTEI_KBN", Conversions.ToString(paramList["SAIKOU_KAKUTEI_KBN"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("TEJIMAI_KBN", Conversions.ToString(paramList["TEJIMAI_KBN"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("TEJIMAI_CONTACT_KBN", Conversions.ToString(paramList["TEJIMAI_CONTACT_KBN"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("MARU_ZOU_MANAGEMENT_KBN", Conversions.ToString(paramList["MARU_ZOU_MANAGEMENT_KBN"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("UNKYU_KBN", Conversions.ToString(paramList["UNKYU_KBN"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("DELETE_DAY", Conversions.ToDecimal(paramList["DELETE_DAY"]), OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine("," + setParam("SYSTEM_ENTRY_DAY", Conversions.ToDate(paramList["SYSTEM_ENTRY_DAY"]), OracleDbType.Date));
        sqlString.AppendLine("," + setParam("SYSTEM_ENTRY_PERSON_CD", Conversions.ToString(paramList["SYSTEM_ENTRY_PERSON_CD"]), OracleDbType.Varchar2));
        sqlString.AppendLine("," + setParam("SYSTEM_ENTRY_PGMID", Conversions.ToString(paramList["SYSTEM_ENTRY_PGMID"]), OracleDbType.Char));
        sqlString.AppendLine("," + setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date));
        sqlString.AppendLine("," + setParam("SYSTEM_UPDATE_PERSON_CD", Conversions.ToString(paramList["SYSTEM_UPDATE_PERSON_CD"]), OracleDbType.Varchar2));
        sqlString.AppendLine("," + setParam("SYSTEM_UPDATE_PGMID", Conversions.ToString(paramList["SYSTEM_UPDATE_PGMID"]), OracleDbType.Char));
        sqlString.AppendLine(" ) ");
        return sqlString.ToString();
    }

    #endregion


}