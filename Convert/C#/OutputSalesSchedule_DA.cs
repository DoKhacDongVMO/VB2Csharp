using System.Collections;
using System.Text;

public partial class OutputSalesSchedule_DA : DataAccessorBase
{

    #region  定数／変数 

    // 精算情報
    private TSeisanInfoEntity tSeisanInfoEntity = new TSeisanInfoEntity();
    // 予約情報（基本）
    private TCrsLedgerBasicEntity tcrsLedgerBasic = new TCrsLedgerBasicEntity();

    #endregion

    #region  SELECT処理 
    /// <summary>
    /// get sql 売上明細表
    /// </summary>
    /// <param name="paramInfo"></param>
    /// <returns></returns>
    private StringBuilder getSubQuerySalesSchedule(Hashtable paramInfo)
    {
        var resultCommandTxt = new StringBuilder();
        resultCommandTxt.AppendLine("SELECT DISTINCT ");
        resultCommandTxt.AppendLine("    T_SEISAN_INFO.ENTRY_PERSON_CD");
        resultCommandTxt.AppendLine("  , M_USER.USER_NAME");
        resultCommandTxt.AppendLine("  , TO_CHAR(TO_DATE(LPAD(T_SEISAN_INFO.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ");
        resultCommandTxt.AppendLine("FROM T_SEISAN_INFO ");
        resultCommandTxt.AppendLine("INNER JOIN M_USER ");
        resultCommandTxt.AppendLine("   ON M_USER.COMPANY_CD = '0001' ");
        resultCommandTxt.AppendLine("  AND M_USER.USER_ID = T_SEISAN_INFO.ENTRY_PERSON_CD ");
        // resultCommandTxt.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC ON T_YOYAKU_INFO_BASIC.YOYAKU_KBN = T_SEISAN_INFO.YOYAKU_KBN")
        // resultCommandTxt.AppendLine("   AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = T_SEISAN_INFO.YOYAKU_NO")
        // resultCommandTxt.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC ON T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD")
        // resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY")
        // resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA")
        resultCommandTxt.AppendLine("WHERE T_SEISAN_INFO.COMPANY_CD = '00'");
        {
            var withBlock = tSeisanInfoEntity;
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.EIGYOSYO_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.EIGYOSYO_CD), paramInfo(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.EIGYOSYO_CD)), withBlock.EigyosyoCd.DBType, withBlock.EigyosyoCd.IntegerBu, withBlock.EigyosyoCd.DecimalBu));
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.CREATE_DAY = " + setParam(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.ENTRY_DAY), paramInfo(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.ENTRY_DAY)), withBlock.CreateDay.DBType, withBlock.CreateDay.IntegerBu, withBlock.CreateDay.DecimalBu));
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.ENTRY_PERSON_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.USER_ID), paramInfo(CommonKyushuUtil.getEnumValue(typeof(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.USER_ID)), withBlock.EntryPersonCd.DBType, withBlock.EntryPersonCd.IntegerBu, withBlock.EntryPersonCd.DecimalBu));
        }
        // With tcrsLedgerBasic
        // resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IN " & paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.HOUJIN_GAIKYAKU_KBN)).ToString())
        // End With
        {
            var withBlock1 = tSeisanInfoEntity;
        }

        resultCommandTxt.AppendLine("ORDER BY SIGNON_TIME");
        return resultCommandTxt;
    }

    /// <summary>
    /// 売上明細表
    /// </summary>
    /// <returns></returns>
    public DataTable getSalesSchedule(Hashtable paramInfo)
    {
        var resultDataTable = new DataTable();
        var sqlString = getSubQuerySalesSchedule(paramInfo);
        resultDataTable = getDataTable(sqlString.ToString());
        return resultDataTable;
    }
}
#endregion
