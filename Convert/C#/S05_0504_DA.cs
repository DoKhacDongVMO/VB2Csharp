using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

public partial class S05_0504_DA : DataAccessorBase
{

    #region 定数・変数

    public enum accessType : int
    {
        getEigyousyoName,        // 営業所名取得
        getGridData,             // 検索照会取得検索
        getGridDetailData       // 詳細照会取得検索
    }

    // 精算情報エンティティ
    private TSeisanInfoEntity seisanInfoEntity = new TSeisanInfoEntity();

    #endregion

    #region SELECT処理

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessS05_0504(accessType type, Hashtable paramInfoList = null)
    {
        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case accessType.getEigyousyoName:
                {
                    // 営業所名取得
                    sqlString = getEigyousyoName(paramInfoList);
                    break;
                }

            case accessType.getGridData:
                {
                    // 検索照会取得検索
                    sqlString = getGridData(paramInfoList);
                    break;
                }

            case accessType.getGridDetailData:
                {
                    // 詳細照会取得検索
                    sqlString = getGridDetailData(paramInfoList);
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
    /// 営業所名取得
    /// </summary>
    /// <param name="param">会社コードと営業所コード</param>
    /// <returns></returns>
    public string getEigyousyoName(Hashtable param)
    {
        var sqlString = new StringBuilder();
        paramClear();
        sqlString.AppendLine(" SELECT EIGYOSYO_NAME_1");
        sqlString.AppendLine("   FROM M_EIGYOSYO");
        sqlString.AppendLine("  WHERE COMPANY_CD=" + setParam("COMPANY_CD", param["CompanyCd"], OracleDbType.Char, 2));
        sqlString.AppendLine("    AND EIGYOSYO_CD=" + setParam("EIGYOSYO_CD", param["EigyousyoCd"], OracleDbType.Char, 2));
        return sqlString.ToString();
    }

    /// <summary>
    /// 検索照会取得検索
    /// </summary>
    /// <param name="paramInfoList">検索条件</param>
    /// <returns></returns>
    public string getGridData(Hashtable paramInfoList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SQL生成
        // SELECT句
        sqlString.AppendLine(" SELECT DISTINCT");
        sqlString.AppendLine("       T_SEISAN_INFO.ENTRY_PERSON_CD AS ENTRY_PERSON_CD ");
        sqlString.AppendLine("      ,M_USER.USER_NAME AS USER_NAME ");
        sqlString.AppendLine("      ,TO_DATE(CONCAT(LPAD(T_SEISAN_INFO.CREATE_DAY, 8,'0'),LPAD(T_SEISAN_INFO.SIGNON_TIME,6,'0')), '" + CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDDHH24MISS) + "') AS SIGNON_TIME ");
        sqlString.AppendLine("      ,T_SEISAN_INFO.SIGNON_TIME AS SIGNON_TIME2 ");
        // FROM句
        sqlString.AppendLine("FROM T_SEISAN_INFO");
        sqlString.AppendLine("INNER JOIN M_USER ON M_USER.COMPANY_CD = '" + FixedCd.MuserCompanyCdType.hatoBus + "'");
        sqlString.AppendLine("      AND M_USER.USER_ID = T_SEISAN_INFO.ENTRY_PERSON_CD");
        // WHERE句
        {
            var withBlock = seisanInfoEntity;
            sqlString.AppendLine("  WHERE ");
            // 会社コード
            sqlString.AppendLine("        T_SEISAN_INFO.COMPANY_CD = '" + UserInfoManagement.companyCd + "' ");
            // 営業所コード
            sqlString.AppendLine("    AND T_SEISAN_INFO.EIGYOSYO_CD = " + setParam(withBlock.EigyosyoCd.PhysicsName, paramInfoList(withBlock.EigyosyoCd.PhysicsName), OracleDbType.Char));
            // 作成日
            sqlString.AppendLine("    AND T_SEISAN_INFO.CREATE_DAY = " + setParam("CREATE_DAY", paramInfoList["CREATE_DAY"], OracleDbType.Decimal, 8, 0));
            // 売上区分
            sqlString.AppendLine("    AND NVL(T_SEISAN_INFO.URIAGE_KBN, ' ') <> 'V' ");
            // 精算区分
            sqlString.AppendLine("    AND (T_SEISAN_INFO.SEISAN_KBN LIKE '" + 1 + "%'");
            sqlString.AppendLine("     OR  T_SEISAN_INFO.SEISAN_KBN LIKE '" + 2 + "%')");
            // ユーザID
            if (!string.IsNullOrEmpty(paramInfoList(withBlock.EntryPersonCd.PhysicsName).ToString))
            {
                sqlString.AppendLine("    AND T_SEISAN_INFO.ENTRY_PERSON_CD = " + setParam(withBlock.EntryPersonCd.PhysicsName, paramInfoList(withBlock.EntryPersonCd.PhysicsName), OracleDbType.Varchar2));
            }
            // 発券時刻From
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["TimeFrom"])))
            {
                sqlString.AppendLine(" AND T_SEISAN_INFO.CREATE_TIME >= " + setParam("TimeFrom", paramInfoList["TimeFrom"], OracleDbType.Decimal, 6, 0));
            }
            // 発券時刻To
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["TimeTo"])))
            {
                sqlString.AppendLine(" AND T_SEISAN_INFO.CREATE_TIME <= " + setParam("TimeTo", paramInfoList["TimeTo"], OracleDbType.Decimal, 6, 0));
            }
            // 予約区分
            if (!string.IsNullOrEmpty(paramInfoList(withBlock.YoyakuKbn.PhysicsName).ToString))
            {
                sqlString.AppendLine("    AND T_SEISAN_INFO.YOYAKU_KBN = " + setParam(withBlock.YoyakuKbn.PhysicsName, paramInfoList(withBlock.YoyakuKbn.PhysicsName), OracleDbType.Char));
            }
            // 予約NO
            if (Conversions.ToInteger(paramInfoList(withBlock.YoyakuNo.PhysicsName)) == 0)
            {
            }
            else
            {
                sqlString.AppendLine("    AND T_SEISAN_INFO.YOYAKU_NO = " + setParam(withBlock.YoyakuNo.PhysicsName, paramInfoList(withBlock.YoyakuNo.PhysicsName), OracleDbType.Decimal, 9, 0));
            }
        }
        // ORDER BY句
        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine(" TO_DATE(CONCAT(LPAD(T_SEISAN_INFO.CREATE_DAY, 8,'0'),LPAD(T_SEISAN_INFO.SIGNON_TIME,6,'0')), '" + CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDDHH24MISS) + "') ");
        return sqlString.ToString();
    }

    /// <summary>
    /// 詳細照会取得検索
    /// </summary>
    /// <param name="paramInfoList">検索条件</param>
    /// <returns></returns>
    public string getGridDetailData(Hashtable paramInfoList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SQL生成
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine("     '修正' AS MORE_DETAIL ");
        sqlString.AppendLine("   , TO_CHAR(TO_DATE(LPAD(T_SEISAN_INFO.CREATE_TIME,6,'0'),'HH24:MI:SS'),'HH24:MI:SS') AS CREATE_TIME ");
        sqlString.AppendLine("   , M_WARIBIKI_TYPE.WARIBIKI_TYPE_NAME ");
        sqlString.AppendLine("   , T_SEISAN_INFO.CRS_CD ");
        sqlString.AppendLine("   , T_SEISAN_INFO.SYUPT_DAY ");
        sqlString.AppendLine("   , T_SEISAN_INFO.GOUSYA ");
        sqlString.AppendLine("   , T_CRS_LEDGER_BASIC.CRS_NAME ");
        sqlString.AppendLine("   , NVL(WK_NINZU.SANKA_NINZU_DAI,0) AS SANKA_NINZU_DAI ");
        sqlString.AppendLine("   , NVL(WK_NINZU.SANKA_NINZU_SHO,0) AS SANKA_NINZU_SHO ");
        sqlString.AppendLine("   , NVL(T_SEISAN_INFO.COUPON_URIAGE,0) - NVL(T_SEISAN_INFO.COUPON_REFUND,0) AS COUPON ");
        sqlString.AppendLine("   , NVL(WK_UTIWAKE_GENKIN.KIGOU,'  ')  ||NVL(WK_UTIWAKE_SENSYA_KEN.KIGOU,'  ')||NVL(WK_UTIWAKE_CREDIT.KIGOU,'  ')|| ");
        sqlString.AppendLine("     NVL(WK_UTIWAKE_HURIKOMI.KIGOU,'  ')||NVL(WK_UTIWAKE_OTHER.KIGOU,'  ')     ||NVL(WK_UTIWAKE_WARIBIKI.KIGOU,'  ') AS SEISAN_KOUMOKU_CD ");
        sqlString.AppendLine("   , T_SEISAN_INFO.YOYAKU_KBN ");
        sqlString.AppendLine("   , T_SEISAN_INFO.YOYAKU_NO ");
        sqlString.AppendLine("   , T_SEISAN_INFO.SEQ");
        sqlString.AppendLine("   , T_SEISAN_INFO.KENNO ");
        sqlString.AppendLine("   , T_SEISAN_INFO.SEISAN_KBN ");
        sqlString.AppendLine("   , TO_CHAR(T_SEISAN_INFO.YOYAKU_KBN || T_SEISAN_INFO.YOYAKU_NO, '0,000,000,000') AS YOYAKU_NO_DISP");
        sqlString.AppendLine("   , TO_CHAR(TO_DATE(T_SEISAN_INFO.SYUPT_DAY,'YYYY/MM/DD'),'YYYY/MM/DD') AS SYUPT_DAY_DISP  ");
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine("   T_SEISAN_INFO ");
        sqlString.AppendLine("   INNER JOIN T_CRS_LEDGER_BASIC  ");
        sqlString.AppendLine("      ON T_CRS_LEDGER_BASIC.CRS_CD    = T_SEISAN_INFO.CRS_CD  ");
        sqlString.AppendLine("     AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_SEISAN_INFO.SYUPT_DAY  ");
        sqlString.AppendLine("     AND T_CRS_LEDGER_BASIC.GOUSYA    = T_SEISAN_INFO.GOUSYA  ");
        sqlString.AppendLine("   LEFT JOIN M_WARIBIKI_TYPE ");
        sqlString.AppendLine("      ON M_WARIBIKI_TYPE.WARIBIKI_TYPE = T_SEISAN_INFO.WARIBIKI_TYPE ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_SANKA_NINZU.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , SUM(CASE WHEN M_CHARGE_JININ_KBN.SHUYAKU_CHARGE_KBN_CD IN ('10','20') THEN SANKA_NINZU ELSE 0 END) AS SANKA_NINZU_DAI ");
        sqlString.AppendLine("       , SUM(CASE WHEN M_CHARGE_JININ_KBN.SHUYAKU_CHARGE_KBN_CD IN ('30') THEN SANKA_NINZU ELSE 0 END) AS SANKA_NINZU_SHO ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_SANKA_NINZU ");
        sqlString.AppendLine("       INNER JOIN M_CHARGE_JININ_KBN ");
        sqlString.AppendLine("          ON M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_CD = T_SEISAN_INFO_SANKA_NINZU.CHARGE_KBN_JININ_CD ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_SANKA_NINZU.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_NINZU ");
        sqlString.AppendLine("      ON WK_NINZU.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('G ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD IN ('" + FixedCd.SeisanItemCd.genkin + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.genkin_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_GENKIN ");
        sqlString.AppendLine("      ON WK_UTIWAKE_GENKIN.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('P ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD IN ('" + FixedCd.SeisanItemCd.sensya_ken + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.sensya_ken_modosi + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.bc + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.bc_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_SENSYA_KEN ");
        sqlString.AppendLine("      ON WK_UTIWAKE_SENSYA_KEN.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('C ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD IN ('" + FixedCd.SeisanItemCd.credit_card + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.credit_card_modosi + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.online_credit + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.online_credit_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_CREDIT ");
        sqlString.AppendLine("      ON WK_UTIWAKE_CREDIT.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('F ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD IN ('" + FixedCd.SeisanItemCd.hurikomi_eigyosyo + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.hurikomi_eigyosyo_modosi + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.hurikomi_yoyaku_center + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.hurikomi_yoyaku_center_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_HURIKOMI ");
        sqlString.AppendLine("      ON WK_UTIWAKE_HURIKOMI.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('W ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD IN ('" + FixedCd.SeisanItemCd.waribiki_kingaku + "',");
        sqlString.AppendLine("                                                       '" + FixedCd.SeisanItemCd.waribiki_kingaku_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_WARIBIKI ");
        sqlString.AppendLine("      ON WK_UTIWAKE_WARIBIKI.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   LEFT JOIN ( ");
        sqlString.AppendLine("     SELECT ");
        sqlString.AppendLine("         T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("       , MAX('S ') AS KIGOU ");
        sqlString.AppendLine("     FROM ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE ");
        sqlString.AppendLine("     WHERE ");
        sqlString.AppendLine("           T_SEISAN_INFO_UTIWAKE.SEISAN_KOUMOKU_CD NOT IN ('" + FixedCd.SeisanItemCd.genkin + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.genkin_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.sensya_ken + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.sensya_ken_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.bc + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.bc_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.credit_card + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.credit_card_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.online_credit + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.online_credit_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.hurikomi_eigyosyo + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.hurikomi_eigyosyo_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.hurikomi_yoyaku_center + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.hurikomi_yoyaku_center_modosi + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.waribiki_kingaku + "',");
        sqlString.AppendLine("                                                           '" + FixedCd.SeisanItemCd.waribiki_kingaku_modosi + "') ");
        sqlString.AppendLine("       AND T_SEISAN_INFO_UTIWAKE.KINGAKU <> 0 ");
        sqlString.AppendLine("     GROUP BY ");
        sqlString.AppendLine("       T_SEISAN_INFO_UTIWAKE.SEISAN_INFO_SEQ ");
        sqlString.AppendLine("   ) WK_UTIWAKE_OTHER ");
        sqlString.AppendLine("      ON WK_UTIWAKE_OTHER.SEISAN_INFO_SEQ = T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        {
            var withBlock = seisanInfoEntity;
            sqlString.AppendLine(" WHERE ");
            // 売上区分
            sqlString.AppendLine("       NVL(T_SEISAN_INFO.URIAGE_KBN, ' ') <> 'V' ");
            // 精算区分
            sqlString.AppendLine("   AND (T_SEISAN_INFO.SEISAN_KBN LIKE '" + 1 + "%'");
            sqlString.AppendLine("    OR  T_SEISAN_INFO.SEISAN_KBN LIKE '" + 2 + "%')");
            // 会社コード
            sqlString.AppendLine("   AND T_SEISAN_INFO.COMPANY_CD = '" + UserInfoManagement.companyCd + "' ");
            // 営業所コード
            sqlString.AppendLine("   AND T_SEISAN_INFO.EIGYOSYO_CD = " + setParam(withBlock.EigyosyoCd.PhysicsName, paramInfoList(withBlock.EigyosyoCd.PhysicsName), OracleDbType.Char));
            // ユーザID
            sqlString.AppendLine("   AND T_SEISAN_INFO.ENTRY_PERSON_CD = " + setParam(withBlock.EntryPersonCd.PhysicsName, paramInfoList(withBlock.EntryPersonCd.PhysicsName), OracleDbType.Varchar2));
            // 作成日
            sqlString.AppendLine("   AND T_SEISAN_INFO.CREATE_DAY = " + setParam("CREATE_DAY", paramInfoList["CREATE_DAY"], OracleDbType.Decimal, 8, 0));
            // サインオン時刻
            sqlString.AppendLine("   AND T_SEISAN_INFO.SIGNON_TIME = " + setParam(withBlock.SignonTime.PhysicsName, paramInfoList(withBlock.SignonTime.PhysicsName), OracleDbType.Decimal, 6, 0));
            // 発券時刻From
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["TimeFrom"])))
            {
                sqlString.AppendLine("   AND T_SEISAN_INFO.CREATE_TIME >= " + setParam("TimeFrom", paramInfoList["TimeFrom"], OracleDbType.Decimal, 6, 0));
            }
            // 発券時刻To
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["TimeTo"])))
            {
                sqlString.AppendLine("   AND T_SEISAN_INFO.CREATE_TIME <= " + setParam("TimeTo", paramInfoList["TimeTo"], OracleDbType.Decimal, 6, 0));
            }
            // 予約区分
            if (!string.IsNullOrEmpty(paramInfoList(withBlock.YoyakuKbn.PhysicsName).ToString))
            {
                sqlString.AppendLine("   AND T_SEISAN_INFO.YOYAKU_KBN = " + setParam(withBlock.YoyakuKbn.PhysicsName, paramInfoList(withBlock.YoyakuKbn.PhysicsName), OracleDbType.Char));
            }
            // 予約NO
            if (Conversions.ToInteger(paramInfoList(withBlock.YoyakuNo.PhysicsName)) == 0)
            {
            }
            else
            {
                sqlString.AppendLine("   AND T_SEISAN_INFO.YOYAKU_NO = " + setParam(withBlock.YoyakuNo.PhysicsName, paramInfoList(withBlock.YoyakuNo.PhysicsName), OracleDbType.Decimal, 9, 0));
            }
        }

        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine("   T_SEISAN_INFO.CREATE_TIME, T_SEISAN_INFO.SEISAN_INFO_SEQ ");
        return sqlString.ToString();
    }

    #endregion
}