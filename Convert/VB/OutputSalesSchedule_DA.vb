Imports System.Text

Public Class OutputSalesSchedule_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    '精算情報
    Private tSeisanInfoEntity As New TSeisanInfoEntity()
    '予約情報（基本）
    Private tcrsLedgerBasic As New TCrsLedgerBasicEntity()

#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' get sql 売上明細表
    ''' </summary>
    ''' <param name="paramInfo"></param>
    ''' <returns></returns>
    Private Function getSubQuerySalesSchedule(ByVal paramInfo As Hashtable) As StringBuilder
        Dim resultCommandTxt As StringBuilder = New StringBuilder
        resultCommandTxt.AppendLine("SELECT DISTINCT ")
        resultCommandTxt.AppendLine("    T_SEISAN_INFO.ENTRY_PERSON_CD")
        resultCommandTxt.AppendLine("  , M_USER.USER_NAME")
        resultCommandTxt.AppendLine("  , TO_CHAR(TO_DATE(LPAD(T_SEISAN_INFO.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ")
        resultCommandTxt.AppendLine("FROM T_SEISAN_INFO ")
        resultCommandTxt.AppendLine("INNER JOIN M_USER ")
        resultCommandTxt.AppendLine("   ON M_USER.COMPANY_CD = '0001' ")
        resultCommandTxt.AppendLine("  AND M_USER.USER_ID = T_SEISAN_INFO.ENTRY_PERSON_CD ")
        'resultCommandTxt.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC ON T_YOYAKU_INFO_BASIC.YOYAKU_KBN = T_SEISAN_INFO.YOYAKU_KBN")
        'resultCommandTxt.AppendLine("   AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = T_SEISAN_INFO.YOYAKU_NO")
        'resultCommandTxt.AppendLine("INNER JOIN T_CRS_LEDGER_BASIC ON T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD")
        'resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY")
        'resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA")
        resultCommandTxt.AppendLine("WHERE T_SEISAN_INFO.COMPANY_CD = '00'")
        With tSeisanInfoEntity
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.EIGYOSYO_CD = " & setParam(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.EIGYOSYO_CD), paramInfo(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.EIGYOSYO_CD)), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.CREATE_DAY = " & setParam(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.ENTRY_DAY), paramInfo(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.ENTRY_DAY)), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            resultCommandTxt.AppendLine("   AND T_SEISAN_INFO.ENTRY_PERSON_CD = " & setParam(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.USER_ID), paramInfo(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.USER_ID)), .EntryPersonCd.DBType, .EntryPersonCd.IntegerBu, .EntryPersonCd.DecimalBu))
        End With
        'With tcrsLedgerBasic
        '    resultCommandTxt.AppendLine("   AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IN " & paramInfo.Item(CommonKyushuUtil.getEnumValue(GetType(UriageMeisaiHyoShutsuryokuSuru), UriageMeisaiHyoShutsuryokuSuru.HOUJIN_GAIKYAKU_KBN)).ToString())
        'End With
        With tSeisanInfoEntity
        End With
        resultCommandTxt.AppendLine("ORDER BY SIGNON_TIME")

        Return resultCommandTxt
    End Function

    ''' <summary>
    ''' 売上明細表
    ''' </summary>
    ''' <returns></returns>
    Public Function getSalesSchedule(ByVal paramInfo As Hashtable) As DataTable
        Dim resultDataTable As DataTable = New DataTable
        Dim sqlString As StringBuilder = getSubQuerySalesSchedule(paramInfo)

        resultDataTable = getDataTable(sqlString.ToString())
        Return resultDataTable
    End Function
End Class
#End Region
