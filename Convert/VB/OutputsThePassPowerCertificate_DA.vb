Imports System.Text

Public Class OutputsThePassPowerCertificate_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    '不乗証明
    Private tFujyoProofEntity As New TFujyoProofEntity()
    'コース台帳（基本）
    Private tCrsLedgerBasicEntity As New TCrsLedgerBasicEntity
#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' 不乗証明明細表
    ''' </summary>
    ''' <returns></returns>
    Public Function getSubQueryScandalProofSchedule(ByVal paramInfo As Hashtable) As StringBuilder
        Dim commandText As StringBuilder = New StringBuilder
        Dim a As String = ""
        commandText.AppendLine("SELECT DISTINCT ")
        commandText.AppendLine("    T_FUJYO_PROOF.ENTRY_PERSON_CD AS ENTRY_PERSON_CD ")
        commandText.AppendLine("  , M_USER.USER_NAME AS USER_NAME ")
        commandText.AppendLine("  , TO_CHAR(TO_DATE(LPAD(T_FUJYO_PROOF.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ")
        commandText.AppendLine("FROM T_FUJYO_PROOF ")
        commandText.AppendLine("INNER JOIN M_USER ")
        commandText.AppendLine("   ON M_USER.COMPANY_CD = '0001' ")
        commandText.AppendLine("  AND M_USER.USER_ID = T_FUJYO_PROOF.ENTRY_PERSON_CD ")
        'commandText.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC ")
        'commandText.AppendLine("    ON T_YOYAKU_INFO_BASIC.YOYAKU_KBN = T_FUJYO_PROOF.YOYAKU_KBN")
        'commandText.AppendLine("    AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = T_FUJYO_PROOF.YOYAKU_NO")
        'commandText.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC ")
        'commandText.AppendLine("    ON T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD")
        'commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY")
        'commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA")

        commandText.AppendLine("WHERE T_FUJYO_PROOF.COMPANY_CD = '00' ")
        With tFujyoProofEntity
            commandText.AppendLine("    AND T_FUJYO_PROOF.EIGYOSYO_CD = " & setParam(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD), paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.EIGYOSYO_CD)), .eigyosyoCd.DBType, .eigyosyoCd.IntegerBu, .eigyosyoCd.DecimalBu))
            commandText.AppendLine("    AND TRUNC(T_FUJYO_PROOF.ENTRY_DAY) = " & setParam(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY), paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.ENTRY_DAY)), .EntryDay.DBType, .EntryDay.IntegerBu, .EntryDay.DecimalBu))
            commandText.AppendLine("    AND T_FUJYO_PROOF.ENTRY_PERSON_CD = " & setParam(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID), paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.USER_ID)), .EntryPersonCd.DBType, .EntryPersonCd.IntegerBu, .EntryPersonCd.DecimalBu))
        End With
        'With tCrsLedgerBasicEntity
        '    commandText.AppendLine("    AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IN " & paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchFujoShomeiMeisai_Hyo), ConditionSearchFujoShomeiMeisai_Hyo.HOUJIN_GAIKYAKU_KBN)).ToString())
        'End With
        commandText.AppendLine("ORDER BY SIGNON_TIME")

        Return commandText
    End Function

    ''' <summary>
    ''' get data for Grids
    ''' </summary>
    ''' <param name="paramInfo"></param>
    ''' <returns></returns>
    Public Function getTableScandalProofSchedule(ByVal paramInfo As Hashtable) As DataTable
        Dim resultDataTable As DataTable = New DataTable
        Try
            Dim sqlString As String = getSubQueryScandalProofSchedule(paramInfo).ToString()
            'SQL実施 get 不乗証明明細表 for Grid
            resultDataTable = getDataTable(sqlString.ToString())
        Catch ex As Exception
            Throw ex
        End Try
        Return resultDataTable
    End Function
#End Region

End Class
