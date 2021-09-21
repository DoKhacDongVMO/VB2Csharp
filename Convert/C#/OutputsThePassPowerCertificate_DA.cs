using System;
using System.Collections;
using System.Text;

public partial class OutputsThePassPowerCertificate_DA : DataAccessorBase
{

    #region  定数／変数 

    // 不乗証明
    private TFujyoProofEntity tFujyoProofEntity = new TFujyoProofEntity();
    // コース台帳（基本）
    private TCrsLedgerBasicEntity tCrsLedgerBasicEntity = new TCrsLedgerBasicEntity();
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 不乗証明明細表
    /// </summary>
    /// <returns></returns>
    public StringBuilder getSubQueryScandalProofSchedule(Hashtable paramInfo)
    {
        var commandText = new StringBuilder();
        string a = "";
        commandText.AppendLine("SELECT DISTINCT ");
        commandText.AppendLine("    T_FUJYO_PROOF.ENTRY_PERSON_CD AS ENTRY_PERSON_CD ");
        commandText.AppendLine("  , M_USER.USER_NAME AS USER_NAME ");
        commandText.AppendLine("  , TO_CHAR(TO_DATE(LPAD(T_FUJYO_PROOF.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ");
        commandText.AppendLine("FROM T_FUJYO_PROOF ");
        commandText.AppendLine("INNER JOIN M_USER ");
        commandText.AppendLine("   ON M_USER.COMPANY_CD = '0001' ");
        commandText.AppendLine("  AND M_USER.USER_ID = T_FUJYO_PROOF.ENTRY_PERSON_CD ");
        // commandText.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC ")
        // commandText.AppendLine("    ON T_YOYAKU_INFO_BASIC.YOYAKU_KBN = T_FUJYO_PROOF.YOYAKU_KBN")
        // commandText.AppendLine("    AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = T_FUJYO_PROOF.YOYAKU_NO")
        // commandText.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC ")
        // commandText.AppendLine("    ON T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD")
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY")
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA")

        commandText.AppendLine("WHERE T_FUJYO_PROOF.COMPANY_CD = '00' ");
        {
            var withBlock = tFujyoProofEntity;
            commandText.AppendLine("    AND T_FUJYO_PROOF.EIGYOSYO_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD)), withBlock.eigyosyoCd.DBType, withBlock.eigyosyoCd.IntegerBu, withBlock.eigyosyoCd.DecimalBu));
            commandText.AppendLine("    AND TRUNC(T_FUJYO_PROOF.ENTRY_DAY) = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY)), withBlock.EntryDay.DBType, withBlock.EntryDay.IntegerBu, withBlock.EntryDay.DecimalBu));
            commandText.AppendLine("    AND T_FUJYO_PROOF.ENTRY_PERSON_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID)), withBlock.EntryPersonCd.DBType, withBlock.EntryPersonCd.IntegerBu, withBlock.EntryPersonCd.DecimalBu));
        }
        // With tCrsLedgerBasicEntity
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IN " & paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.HOUJIN_GAIKYAKU_KBN)).ToString())
        // End With
        commandText.AppendLine("ORDER BY SIGNON_TIME");
        return commandText;
    }

    /// <summary>
    /// get data for Grids
    /// </summary>
    /// <param name="paramInfo"></param>
    /// <returns></returns>
    public DataTable getTableScandalProofSchedule(Hashtable paramInfo)
    {
        var resultDataTable = new DataTable();
        try
        {
            string sqlString = getSubQueryScandalProofSchedule(paramInfo).ToString();
            // SQL実施 get 不乗証明明細表 for Grid
            resultDataTable = getDataTable(sqlString.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return resultDataTable;
    }
    #endregion

}