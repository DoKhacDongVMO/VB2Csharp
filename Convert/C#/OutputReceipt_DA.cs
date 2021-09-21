using System;
using System.Collections;
using System.Text;

public partial class OutputReceipt_DA : DataAccessorBase
{

    #region  定数／変数 

    // 精算情報
    private TReceiptEntity tReceiptEntity = new TReceiptEntity();
    // 予約情報（基本）
    private TCrsLedgerBasicEntity tcrsLedgerBasic = new TCrsLedgerBasicEntity();

    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 領収書明明細表
    /// </summary>
    /// <returns></returns>
    public StringBuilder getSubQueryReceiptSchedule(Hashtable paramInfo)
    {
        var commandText = new StringBuilder();
        commandText.AppendLine("SELECT DISTINCT ");
        commandText.AppendLine("    T_RECEIPT.ENTRY_PERSON_CD AS ENTRY_PERSON_CD ");
        commandText.AppendLine("  , M_USER.USER_NAME AS USER_NAME ");
        commandText.AppendLine("  , TO_CHAR(TO_DATE(LPAD(T_RECEIPT.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ");
        commandText.AppendLine("FROM T_RECEIPT ");
        commandText.AppendLine("INNER JOIN M_USER ");
        commandText.AppendLine("   ON M_USER.COMPANY_CD = '0001' ");
        commandText.AppendLine("  AND M_USER.USER_ID = T_RECEIPT.ENTRY_PERSON_CD ");
        // commandText.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC ")
        // commandText.AppendLine("    ON T_YOYAKU_INFO_BASIC.YOYAKU_KBN = T_RECEIPT.YOYAKU_KBN")
        // commandText.AppendLine("    AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = T_RECEIPT.YOYAKU_NO")
        // commandText.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC ")
        // commandText.AppendLine("    ON T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD")
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY")
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA")

        commandText.AppendLine("WHERE T_RECEIPT.COMPANY_CD = '00' ");
        {
            var withBlock = tReceiptEntity;
            commandText.AppendLine("    AND T_RECEIPT.EIGYOSYO_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD)), withBlock.eigyosyoCd.DBType, withBlock.eigyosyoCd.IntegerBu, withBlock.eigyosyoCd.DecimalBu));
            commandText.AppendLine("    AND TRUNC(T_RECEIPT.ENTRY_DAY) = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY)), withBlock.EntryDay.DBType, withBlock.EntryDay.IntegerBu, withBlock.EntryDay.DecimalBu));
            commandText.AppendLine("    AND T_RECEIPT.ENTRY_PERSON_CD = " + setParam(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID), paramInfo(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID)), withBlock.EntryPersonCd.DBType, withBlock.EntryPersonCd.IntegerBu, withBlock.EntryPersonCd.DecimalBu));
        }
        // With tcrsLedgerBasic
        // commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IN " & paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.HOUJIN_GAIKYAKU_KBN)).ToString())
        // End With
        commandText.AppendLine("ORDER BY SIGNON_TIME");
        return commandText;
    }

    /// <summary>
    /// get data for Grids _ TODO
    /// </summary>
    /// <param name="paramInfo"></param>
    /// <returns></returns>
    public DataTable getTableReceiptSchedule(Hashtable paramInfo)
    {
        var resultDataTable = new DataTable();
        try
        {
            string sqlString = getSubQueryReceiptSchedule(paramInfo).ToString();
            // SQL実施 get 領収書明細表 for Grid _ TODO
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