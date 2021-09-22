Imports System.Text

''' <summary>
''' インターネット予約トランザクション管理）のDAクラス
''' <remarks>
''' Author:2018/10/26//QuangTD
''' </remarks>
''' </summary>
Public Class IntYoyakuTranManagement_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private clsIntYoyakuTranManagementEntity As New TIntYoyakuTranManagementEntity()
    Private clsTYoyakuInfoBasicEntity As New TYoyakuInfoBasicEntity()

    ''' <summary>
    ''' DAアクセスタイプ
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum accessType As Integer
        getIntYoyakuTranManagement
        getIntYoyakuTranLog
        getIntYoyakuTranLogDetail
        getChargeApplicationNinzu
        executeUpdateIntYoyokuTranLogDetail
    End Enum
#End Region

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessIntYoyakuTranManagement(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getIntYoyakuTranManagement
                sqlString = getIntYoyakuTranManagement(paramInfoList)
            Case accessType.getIntYoyakuTranLog
                sqlString = getIntYoyakuTranLog(paramInfoList)
            Case accessType.getIntYoyakuTranLogDetail
                sqlString = getIntYoyakuTranLogDetail(paramInfoList)
            Case accessType.getChargeApplicationNinzu
                sqlString = getChargeApplicationNinzu(paramInfoList)
            Case Else
                Return returnValue
        End Select
        Try
            returnValue = getDataTable(sqlString)
        Catch ex As Exception
            Throw
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' SUM(料金適用人数) SELECT
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <remarks></remarks>
    Private Function getChargeApplicationNinzu(paramInfoList As Hashtable) As String
        Dim sqlString As New StringBuilder
        With clsIntYoyakuTranManagementEntity
            'SELECT
            sqlString.AppendLine(" SELECT ")
            sqlString.AppendLine("    SUM(T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN.CHARGE_APPLICATION_NINZU) AS TOTAL_CHARGE_APPLICATION_NINZU,")
            sqlString.AppendLine("    M_CHARGE_JININ_KBN.SHUYAKU_CHARGE_KBN_CD")
            'FROM
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT")
            sqlString.AppendLine(" LEFT OUTER JOIN")
            sqlString.AppendLine("    T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
            sqlString.AppendLine(" ON ")
            sqlString.AppendLine("    (T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO = T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN.YOYAKU_NO")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN = T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN.YOYAKU_KBN)")
            sqlString.AppendLine(" LEFT OUTER JOIN")
            sqlString.AppendLine("    M_CHARGE_JININ_KBN")
            sqlString.AppendLine(" ON ")
            sqlString.AppendLine("    T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN.CHARGE_KBN_JININ_CD = M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_CD")
            'WHERE 
            If Not (paramInfoList Is Nothing) AndAlso paramInfoList.Count > 0 Then
                sqlString.AppendLine(" WHERE ")
                If Not String.IsNullOrEmpty(CType(paramInfoList.Item("tranKey"), String)) Then
                    sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY =  CAST( " & setParam("tranKey", paramInfoList.Item("tranKey"), .tranKey.DBType, .tranKey.IntegerBu, .tranKey.DecimalBu) & " AS Char(46))")
                End If
                If Not String.IsNullOrEmpty(CType(paramInfoList.Item("tranKbn"), String)) Then
                    sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN =  CAST( " & setParam("tranKbn", paramInfoList.Item("tranKbn"), .tranKbn.DBType, .tranKbn.IntegerBu, .tranKbn.DecimalBu) & " AS Char(1))")
                End If
            End If
            sqlString.AppendLine("GROUP BY ")
            sqlString.AppendLine("    M_CHARGE_JININ_KBN.SHUYAKU_CHARGE_KBN_CD")
            Return sqlString.ToString()
        End With
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <remarks></remarks>
    Private Function getIntYoyakuTranLogDetail(paramInfoList As Hashtable) As String
        Dim sqlString As New StringBuilder
        With clsIntYoyakuTranManagementEntity
            'SELECT
            sqlString.AppendLine(" SELECT")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_FLG,")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_DAY,'yyyyMMdd'),'yy/MM/dd') AS OPERATION_TAISHO_DAY,")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(CASE")
            sqlString.AppendLine("    WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_TIME < 100000 THEN LPAD(T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_TIME, 6, '0')")
            sqlString.AppendLine("    ELSE TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_TIME)")
            sqlString.AppendLine("    END,'HH24miss'),'HH24:mi') AS OPERATION_TAISHO_TIME,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_PERSON,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.INVALID_FLG,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_TIME,")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY,'yyyyMMdd'),'yy/MM/dd') || ' ' ||TO_CHAR(TO_DATE(CASE")
            sqlString.AppendLine("    WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_TIME < 100000 THEN LPAD(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_TIME, 6, '0')")
            sqlString.AppendLine("    ELSE TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_TIME)")
            sqlString.AppendLine("    END,'HH24miss'),'HH24:mi') AS COL_PROCESS_UKETUKE_DATE,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY AS CRS_SYUPT_DAY_YOBI,")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY,'yyyyMMdd'),'yy/MM/dd') AS CRS_SYUPT_DAY,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.GOUSYA,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,")
            sqlString.AppendLine("    REPLACE(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN||TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,'000000000'),' ','') AS COL_YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_NAME,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.MOSHIKOMI_YOYAKU_NINZU_KEI,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_TEL_1,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_TEL_2,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.AGENT_CD,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.AGENT_TANTOSYA,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.SEIKI_CHARGE_ALL_GAKU,0),'0,000,000'),' ','') AS SEIKI_CHARGE_ALL_GAKU,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.CANCEL_RYOU_KEI,0),'0,000,000'),' ','') AS CANCEL_RYOU,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.WARIBIKI_ALL_GAKU,0),'0,000,000'),' ','') AS WARIBIKI_KINGAKU,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_URIAGE,0),'0,000,000'),' ','') AS TORIATUKAI_FEE_URIAGE,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_CANCEL,0),'0,000,000'),' ','') AS TORIATUKAI_FEE_CANCEL,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("    T_TRAN_MANAGEMENT_SETTLEMENT_INFO.ORDER_ID,")
            sqlString.AppendLine("    T_TRAN_MANAGEMENT_SETTLEMENT_INFO.GMO_CAPTURE_YMD,")
            sqlString.AppendLine("    T_TRAN_MANAGEMENT_SETTLEMENT_INFO.GMO_CAPTURE_TIME,")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_NAME,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.AGENT_NM,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.AGENT_TEL_NO_1,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.AGENT_TEL_NO_2,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.AGENT_TEL_NO_3,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.ADD_CHARGE_MAEBARAI_KEI,0),'0,000,000'),' ','') AS ADD_CHARGE_MAEBARAI_KEI,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_URIAGE,0) + NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_CANCEL,0),'0,000,000'),' ','') AS TORIATUKAI_FEE_SAGAKU,")
            sqlString.AppendLine("    REPLACE(TO_CHAR(NVL(T_YOYAKU_INFO_BASIC.WARIBIKI_ALL_GAKU,0),'0,000,000'),' ','') AS WARIBIKI_ALL_GAKU,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.CRS_KIND,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.YOBI_1,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.YOBI_2,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY, ")
            sqlString.AppendLine("    REPLACE(TO_CHAR((NVL(T_YOYAKU_INFO_BASIC.SEIKI_CHARGE_ALL_GAKU, 0)")
            sqlString.AppendLine("    + NVL(T_YOYAKU_INFO_BASIC.ADD_CHARGE_MAEBARAI_KEI, 0)")
            sqlString.AppendLine("    - NVL(T_YOYAKU_INFO_BASIC.WARIBIKI_ALL_GAKU, 0)")
            sqlString.AppendLine("    + NVL(T_YOYAKU_INFO_BASIC.CANCEL_RYOU_KEI, 0)")
            sqlString.AppendLine("    - NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_URIAGE, 0)")
            sqlString.AppendLine("    - NVL(T_YOYAKU_INFO_BASIC.TORIATUKAI_FEE_CANCEL, 0)),'0,000,000'),' ','') AS COL_TOTAL, ")
            '1 予約
            sqlString.AppendLine("CASE")
            sqlString.AppendLine("    WHEN (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN='A'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN   ='B'")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY NOT LIKE '%02%'")
            sqlString.AppendLine("    AND (T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY NOT LIKE '%D2%'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN='C'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN='D'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN='E')")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY NOT LIKE '%01%')")
            sqlString.AppendLine("    THEN ''")
            sqlString.AppendLine("    ELSE '○'")
            sqlString.AppendLine("  END AS COL_YOYAKU,")
            sqlString.AppendLine("    COL_TORIHIKI_ENTRY,")
            sqlString.AppendLine("    COL_SOKUJI_SETTLEMENT,")
            sqlString.AppendLine("    COL_SETTLEMENT_CANCEL,")
            '5 入金
            sqlString.AppendLine(" CASE")
            sqlString.AppendLine("    WHEN (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <> 'A' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <> 'B' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <> 'H' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <> 'I' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <> 'K' )")
            sqlString.AppendLine("    THEN ''")
            sqlString.AppendLine("    WHEN (T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A05' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A06' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B05' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B06' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'H04' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'I05' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'J03' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A16' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A17' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'K16' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'K17') ")
            sqlString.AppendLine("    THEN '○'")
            sqlString.AppendLine("    WHEN (T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A04' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A54' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A56' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A51' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,1' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B04' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B54' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B56' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'B51' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,3' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'H03' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'H53' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'H51' ")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,5' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'I04' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'J02' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A15' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'A65' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,12' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,13' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'K15' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS = 'K65' ")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY = 'RST,20') ")
            sqlString.AppendLine("    THEN '×'")
            sqlString.AppendLine("    END AS COL_NYUUKIN,")
            sqlString.AppendLine("    '結果' AS COL_TORIATUKAI_NAIYO,")
            '6 決済取消
            sqlString.AppendLine("  CASE")
            sqlString.AppendLine("    WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB1'")
            sqlString.AppendLine("    AND COL_TORIHIKI_ENTRY                                 = ''")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A62'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A61'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='KB1'")
            sqlString.AppendLine("    AND COL_TORIHIKI_ENTRY                                 = ''")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K62'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K61'")
            sqlString.AppendLine("    THEN ''")
            sqlString.AppendLine("    WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A06'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='AB1'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B06'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='BB1'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='C03'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='A01:A02:A03:A04:A54:A56:A517'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='D03'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='DB1'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A17'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K17'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='L03'")
            sqlString.AppendLine("    THEN '○'")
            sqlString.AppendLine("    WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A05'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='AB2'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B05'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='BB2'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='C02'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='CB2'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='D02'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='DB2'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A16'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K16'")
            sqlString.AppendLine("    OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='L02'")
            sqlString.AppendLine("    THEN '×'")
            sqlString.AppendLine("  END AS COL_MAIL,")
            sqlString.AppendLine("  CASE")
            sqlString.AppendLine("    WHEN COL_TORIHIKI_ENTRY   = '○'")
            sqlString.AppendLine("    AND COL_SOKUJI_SETTLEMENT = '○'")
            sqlString.AppendLine("    AND COL_SETTLEMENT_CANCEL = '○'")
            sqlString.AppendLine("    THEN '0'")
            sqlString.AppendLine("    WHEN (COL_TORIHIKI_ENTRY                               <> '○'")
            sqlString.AppendLine("    OR COL_SOKUJI_SETTLEMENT                               <> '○')")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS <> 'J01'")
            sqlString.AppendLine("    THEN '0'")
            sqlString.AppendLine("    ELSE REPLACE(TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.SEIKI_CHARGE_ALL_GAKU,'0,000,000'),' ','')")
            sqlString.AppendLine("  END AS TXT_KI_SETTLEMENT_KINGAKU")
            'FROM
            sqlString.AppendLine(" FROM")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT ")
            sqlString.AppendLine(" LEFT OUTER JOIN")
            sqlString.AppendLine("   (SELECT")
            '2 取引登録
            sqlString.AppendLine("     CASE")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN          <>'A'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'B'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'H'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'I'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'K'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A11'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A12'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A62'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A61'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB1'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K11'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K12'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K62'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K61'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='KB1'")
            sqlString.AppendLine("       THEN ''")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A01'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B01'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='H01'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='I01'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A13'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K13'")
            sqlString.AppendLine("       THEN '×'")
            sqlString.AppendLine("       ELSE '○'")
            sqlString.AppendLine("     END AS COL_TORIHIKI_ENTRY,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN")
            sqlString.AppendLine("   FROM T_INT_YOYAKU_TRAN_MANAGEMENT")
            sqlString.AppendLine("   ) TORIHIKI_ENTRY")
            sqlString.AppendLine(" ON (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY = TORIHIKI_ENTRY.TRAN_KEY")
            sqlString.AppendLine(" AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN = TORIHIKI_ENTRY.TRAN_KBN)")
            sqlString.AppendLine(" LEFT OUTER JOIN")
            sqlString.AppendLine("   (SELECT")
            '3 即時決済
            sqlString.AppendLine("     CASE")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN<>'A'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <>'B'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <>'H'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <>'I'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN <>'K'")
            sqlString.AppendLine("       THEN ''")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS＞='A04'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS＜ ='A06'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A54'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A56'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='A01:A02:A03:A04:A54:A56:A51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS＞  ='B04'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS＜ ='B06'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B54'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B56'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='B01:B02:B03:B04:B54:B56:B51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H03'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H04'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H53'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='H01:H02:H03:H53:H51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='I04'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='I05'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A15'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A16'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A17'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A65'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A04:A54:A56:A512'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A04:A54:A56:A513'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K15'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K16'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K17'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K65'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A53:A52:A510'")
            sqlString.AppendLine("       THEN '○'")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS ='A03'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A53'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A52'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='A01:A02:A03:A53:A52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='A01:A02:A03:A53:A52:A51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B03'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B53'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B52'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='B01:B02:B03:B53:B52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='B51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='B01:B02:B03:B53:B52:B51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H02'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='H51'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY ='H01:H02:H52:H51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='I03'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A14'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='AD4'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='A64'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A04:A54:A56:A514'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A04:A54:A56:A515'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K14'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS   ='K64'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.PROCESS_STATUS_HISTORY  ='A01:A02:A03:A53:A52:A511'")
            sqlString.AppendLine("       THEN '×'")
            sqlString.AppendLine("       ELSE ''")
            sqlString.AppendLine("     END AS COL_SOKUJI_SETTLEMENT,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN")
            sqlString.AppendLine("   FROM T_INT_YOYAKU_TRAN_MANAGEMENT")
            sqlString.AppendLine("   ) SOKUJI_SETTLEMENT")
            sqlString.AppendLine(" ON (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY = SOKUJI_SETTLEMENT.TRAN_KEY")
            sqlString.AppendLine(" AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN = SOKUJI_SETTLEMENT.TRAN_KBN)")
            sqlString.AppendLine(" LEFT OUTER JOIN")
            sqlString.AppendLine("   (SELECT")
            '4 決済取消
            sqlString.AppendLine("     CASE")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN          <>'A'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'B'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'H'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'I'")
            sqlString.AppendLine("       AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'K'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB1'")
            sqlString.AppendLine("       AND T1.COL_TORIHIKI_ENTRY                            = ''")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='KB1'")
            sqlString.AppendLine("       AND T1.COL_TORIHIKI_ENTRY                            = ''")
            sqlString.AppendLine("       THEN ''")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB2'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='AB1'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='BB2'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='BB1'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='H51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='I51'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A63'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K63'")
            sqlString.AppendLine("       THEN '○'")
            sqlString.AppendLine("       WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB3'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='BB3'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='H52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='I52'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A64'")
            sqlString.AppendLine("       OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K64'")
            sqlString.AppendLine("       THEN '×'")
            sqlString.AppendLine("     END AS COL_SETTLEMENT_CANCEL,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("     T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN")
            sqlString.AppendLine("   FROM T_INT_YOYAKU_TRAN_MANAGEMENT")
            sqlString.AppendLine("   LEFT OUTER JOIN")
            sqlString.AppendLine("     (SELECT")
            sqlString.AppendLine("       CASE")
            sqlString.AppendLine("         WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN          <>'A'")
            sqlString.AppendLine("         AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'B'")
            sqlString.AppendLine("         AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'H'")
            sqlString.AppendLine("         AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'I'")
            sqlString.AppendLine("         AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN           <>'K'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A11'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A12'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A62'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A61'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='AB1'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K11'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K12'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K62'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='K61'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='KB1'")
            sqlString.AppendLine("         THEN ''")
            sqlString.AppendLine("         WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS='A01'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='B01'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='H01'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='I01'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='A13'")
            sqlString.AppendLine("         OR T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS  ='K13'")
            sqlString.AppendLine("         THEN '×'")
            sqlString.AppendLine("         ELSE '○'")
            sqlString.AppendLine("       END AS COL_TORIHIKI_ENTRY,")
            sqlString.AppendLine("       T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("       T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN")
            sqlString.AppendLine("     FROM T_INT_YOYAKU_TRAN_MANAGEMENT")
            sqlString.AppendLine("     ) T1")
            sqlString.AppendLine("   ON (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY                     = T1.TRAN_KEY")
            sqlString.AppendLine("   AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN                     = T1.TRAN_KBN)")
            sqlString.AppendLine("   ) SETTLEMENT_CANCEL ON (T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY = SETTLEMENT_CANCEL.TRAN_KEY")
            sqlString.AppendLine(" AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN                       = SETTLEMENT_CANCEL.TRAN_KBN)")
            sqlString.AppendLine(" LEFT OUTER JOIN ")
            sqlString.AppendLine("    T_TRAN_MANAGEMENT_SETTLEMENT_INFO ")
            sqlString.AppendLine(" ON")
            sqlString.AppendLine("    (T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN = T_TRAN_MANAGEMENT_SETTLEMENT_INFO.YOYAKU_KBN")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO = T_TRAN_MANAGEMENT_SETTLEMENT_INFO.YOYAKU_NO) ")
            sqlString.AppendLine(" INNER JOIN")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC ")
            sqlString.AppendLine(" ON")
            sqlString.AppendLine("    (T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN = T_YOYAKU_INFO_BASIC.YOYAKU_KBN")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO = T_YOYAKU_INFO_BASIC.YOYAKU_NO)")
            sqlString.AppendLine(" INNER JOIN")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine(" ON")
            sqlString.AppendLine("    (T_YOYAKU_INFO_BASIC.CRS_CD = T_CRS_LEDGER_BASIC.CRS_CD")
            sqlString.AppendLine("    AND T_YOYAKU_INFO_BASIC.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY")
            sqlString.AppendLine("    AND T_YOYAKU_INFO_BASIC.GOUSYA = T_CRS_LEDGER_BASIC.GOUSYA)")
            'WHERE 
            If Not (paramInfoList Is Nothing) AndAlso paramInfoList.Count > 0 Then
                sqlString.AppendLine(" WHERE 1 = 1")
                If Not String.IsNullOrEmpty(CType(paramInfoList.Item("tranKey"), String)) Then
                    sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY =  CAST( " & setParam("tranKey", paramInfoList.Item("tranKey"), .tranKey.DBType, .tranKey.IntegerBu, .tranKey.DecimalBu) & " AS Char(46))")
                End If
                If Not String.IsNullOrEmpty(CType(paramInfoList.Item("tranKbn"), String)) Then
                    sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN =  CAST( " & setParam("tranKbn", paramInfoList.Item("tranKbn"), .tranKbn.DBType, .tranKbn.IntegerBu, .tranKbn.DecimalBu) & " AS Char(1))")
                End If
            End If
            Return sqlString.ToString()
        End With
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <remarks></remarks>
    Private Function getIntYoyakuTranLog(paramInfoList As Hashtable) As String
        Dim sqlString As New StringBuilder
        With clsIntYoyakuTranManagementEntity
            'SELECT
            sqlString.AppendLine("SELECT")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY,'yyyyMMdd'),'yy/MM/dd') || ' ' ||TO_CHAR(TO_DATE(LPAD(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_TIME, 6, '0'),'HH24miss'),'HH24:mi') AS COL_PROCESS_UKETUKE_DATE,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.LATEST_PROCESS_STATUS,")
            sqlString.AppendLine("    CASE ")
            sqlString.AppendLine("        WHEN T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_FLG = 'Y' THEN 1 ")
            sqlString.AppendLine("        ELSE 0 ")
            sqlString.AppendLine("    END AS OPERATION_TAISHO_FLG,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,")
            sqlString.AppendLine("    REPLACE(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN||','||TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,'000,000,000'),' ','') AS COL_YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_NAME,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.MOSHIKOMI_YOYAKU_NINZU_KEI,")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY,'yyMMdd'),'yy/MM/dd') AS CRS_SYUPT_DAY,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.GOUSYA,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.INVALID_FLG,")
            sqlString.AppendLine("    CASE ")
            sqlString.AppendLine("         WHEN TRIM(T_YOYAKU_INFO_BASIC.AGENT_CD) IS NOT NULL THEN 'Y'")
            sqlString.AppendLine("         ELSE ''")
            sqlString.AppendLine("    END AS AGENT_CD,")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC.CANCEL_FLG ")
            sqlString.AppendLine("FROM")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT ")
            sqlString.AppendLine("INNER JOIN")
            sqlString.AppendLine("    T_YOYAKU_INFO_BASIC ")
            sqlString.AppendLine("ON")
            sqlString.AppendLine("    (T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN = T_YOYAKU_INFO_BASIC.YOYAKU_KBN")
            sqlString.AppendLine("    And T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO = T_YOYAKU_INFO_BASIC.YOYAKU_NO) ")
            sqlString.AppendLine("WHERE 1 = 1")
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("createDay"), String)) Then
                'インターネット予約トランザクション管理．作成日 = 画面．処理受付日
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY = " & setParam("createDay", paramInfoList.Item("createDay"), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("crsSyuptDay"), String)) Then
                'インターネット予約トランザクション管理．出発日 = 画面．出発日
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY = " & setParam("crsSyuptDay", paramInfoList.Item("crsSyuptDay"), .CrsSyuptDay.DBType, .CrsSyuptDay.IntegerBu, .CrsSyuptDay.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("crsCd"), String)) Then
                'インターネット予約トランザクション管理．コースコード = 画面．コースコード
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD = " & setParam("crsCd", CType(paramInfoList.Item("crsCd"), String), .CrsCd.DBType, .CrsCd.IntegerBu, .CrsCd.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("yoyakuNo"), String)) Then
                'インターネット予約トランザクション管理．予約ＮＯ = 画面．「予約番号」の1桁目以外
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO = " & setParam("yoyakuNo", CType(paramInfoList.Item("yoyakuNo"), String), .YoyakuNo.DBType, .YoyakuNo.IntegerBu, .YoyakuNo.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("yoyakuKbn"), String)) Then
                'インターネット予約トランザクション管理．予約区分 = 画面．「予約番号」の1桁目
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN = " & setParam("yoyakuKbn", CType(paramInfoList.Item("yoyakuKbn"), String), .YoyakuKbn.DBType, .YoyakuKbn.IntegerBu, .YoyakuKbn.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("yykmksName"), String)) Then
                'インターネット予約トランザクション管理.申込者名 Like 画面.名前%
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_NAME LIKE " & setParam("yykmksName", paramInfoList.Item("yykmksName"), .YykmksName.DBType) & "||'%'")
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("agentTelNo"), String)) Then
                '予約情報（基本）．業者電話番号 LIKE 画面．電話番号%
                sqlString.AppendLine("    AND T_YOYAKU_INFO_BASIC.AGENT_TEL_NO LIKE " & setParam("agentTelNo", paramInfoList.Item("agentTelNo"), clsTYoyakuInfoBasicEntity.AgentTelNo.DBType) & "||'%'")
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("agentCd"), String)) Then
                '予約情報（基本）．業者コード = 画面．業者コード
                sqlString.AppendLine("    AND T_YOYAKU_INFO_BASIC.AGENT_CD = " & setParam("agentCd", CType(paramInfoList.Item("agentCd"), String), .AgentCd.DBType, .AgentCd.IntegerBu, .AgentCd.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("chkTaiouAlreadyWith"), String)) Then
                'WHEN  「画面．対応済み含む」にチェックの場合
                'インターネット予約トランザクション管理.運用対処フラグ = 'Y'
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_FLG = 'Y'")
                'ElseIf String.IsNullOrEmpty(CType(paramInfoList.Item("chkTaiouAlreadyWith"), String)) And String.IsNullOrEmpty(CType(paramInfoList.Item("chkTaiouHuyoWith"), String)) Then
                '    'WHEN 「画面.対応済み含む」「画面対応不要含む」のどちらにも選択が無い場合
                '    'Nvl(インターネット予約トランザクション管理.無効フラグ, ' ') ≠ 'Y'
                '    'かつ、NVL(インターネット予約トランザクション管理.運用対処フラグ, ' ') ≠ 'Y'
                '    sqlString.AppendLine("    AND NVL(T_INT_YOYAKU_TRAN_MANAGEMENT.INVALID_FLG,' ') <> 'Y'")
                '    sqlString.AppendLine("    AND NVL(T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_FLG,' ') <> 'Y'")
            ElseIf Not String.IsNullOrEmpty(CType(paramInfoList.Item("chkTaiouHuyoWith"), String)) Then
                'WHEN 「画面．対応不要含む」にチェックの場合
                'インターネット予約トランザクション管理.無効フラグ = 'Y'
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.INVALID_FLG = 'Y'")
            End If
            Return sqlString.ToString()
        End With
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <remarks></remarks>
    Private Function getIntYoyakuTranManagement(paramInfoList As Hashtable) As String
        Dim sqlString As New StringBuilder
        With clsIntYoyakuTranManagementEntity
            'SELECT
            sqlString.AppendLine("SELECT DISTINCT")
            sqlString.AppendLine("    TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY,'yyyyMMdd'),'yyyy/MM/dd') AS CREATE_DAY,")
            sqlString.AppendLine("    REPLACE(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN||TO_CHAR(T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,'000000000'),' ','') AS COL_YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_NAME,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_TEL_1,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_EMAIL,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD,")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_NAME,")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_KIND,")
            sqlString.AppendLine("    CASE")
            sqlString.AppendLine("      WHEN NVL(T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY,0) <> 0 THEN")
            sqlString.AppendLine("          TO_CHAR(TO_DATE(T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY,'yyyyMMdd'),'yyyy/MM/dd') ")
            sqlString.AppendLine("    END AS CRS_SYUPT_DAY")
            'FROM
            sqlString.AppendLine("FROM")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT ")
            sqlString.AppendLine("INNER JOIN")
            sqlString.AppendLine("    T_INT_YOYAKU_MAIL_SENDING_ERROR_LOG ")
            sqlString.AppendLine("ON")
            sqlString.AppendLine("    T_INT_YOYAKU_MAIL_SENDING_ERROR_LOG.UNIQ_KEY LIKE TRIM(T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_EMAIL) || '%' ")
            sqlString.AppendLine("    AND SUBSTR(TO_CHAR(T_INT_YOYAKU_MAIL_SENDING_ERROR_LOG.SYSTEM_ENTRY_DAY,'YYYYMMDD'),0,8) = T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY ")
            sqlString.AppendLine("LEFT OUTER JOIN")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine("ON")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_CD = T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD")
            sqlString.AppendLine("    AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_SYUPT_DAY")
            sqlString.AppendLine("    AND T_CRS_LEDGER_BASIC.GOUSYA = T_INT_YOYAKU_TRAN_MANAGEMENT.GOUSYA")
            'WHERE
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_EMAIL IS NOT NULL ")
            sqlString.AppendLine("    AND TRIM(T_INT_YOYAKU_MAIL_SENDING_ERROR_LOG.OPERATION_TAISHO_FLG) IS NULL ")
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("createDayFrom"), String)) Then
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY >= " & setParam("createDayFrom", paramInfoList.Item("createDayFrom"), .createDay.DBType, .createDay.IntegerBu, .createDay.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("createDayTo"), String)) Then
                sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.CREATE_DAY <= " & setParam("createDayTo", paramInfoList.Item("createDayTo"), .createDay.DBType, .createDay.IntegerBu, .createDay.DecimalBu))
            End If
            'ORDER
            sqlString.AppendLine(" ORDER BY")
            sqlString.AppendLine("    CREATE_DAY,")
            sqlString.AppendLine("    COL_YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_KBN,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YOYAKU_NO,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_NAME,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_TEL_1,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.YYKMKS_EMAIL,")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.CRS_CD,")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_NAME,")
            sqlString.AppendLine("    T_CRS_LEDGER_BASIC.CRS_KIND,")
            sqlString.AppendLine("    CRS_SYUPT_DAY")
            Return sqlString.ToString()
        End With
    End Function

#Region " INSERT/UPDATE処理 "
    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeIntYoyakuTranManagement(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As Integer
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Try
            Select Case type
                Case accessType.executeUpdateIntYoyokuTranLogDetail
                    sqlString = executeUpdateIntYoyokuTranLogDetail(paramInfoList)
            End Select
            'SQL実施
            returnValue = execNonQuery(sqlString)
        Catch ex As Exception
            Throw
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' データ更新用
    ''' </summary>
    ''' <param name="paramInfoList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateIntYoyokuTranLogDetail(ByVal paramInfoList As Hashtable) As String
        Dim sqlString As New StringBuilder
        With clsIntYoyakuTranManagementEntity

            'UPDATE
            sqlString.AppendLine("UPDATE")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT ")
            sqlString.AppendLine("SET")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_FLG	= " & setParam("operationTaishoFlg", paramInfoList.Item("operationTaishoFlg"), .operationTaishoFlg.DBType, .operationTaishoFlg.IntegerBu, .operationTaishoFlg.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.INVALID_FLG	= " & setParam("invalidFlg", paramInfoList.Item("invalidFlg"), .invalidFlg.DBType, .invalidFlg.IntegerBu, .invalidFlg.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_DAY	= " & setParam("operationTaishoDay", paramInfoList.Item("operationTaishoDay"), .operationTaishoDay.DBType, .operationTaishoDay.IntegerBu, .operationTaishoDay.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_TIME = " & setParam("operationTaishoTime", paramInfoList.Item("operationTaishoTime"), .operationTaishoTime.DBType, .operationTaishoTime.IntegerBu, .operationTaishoTime.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_PERSON = " & setParam("operationTaishoPerson", paramInfoList.Item("operationTaishoPerson"), .operationTaishoPerson.DBType, .operationTaishoPerson.IntegerBu, .operationTaishoPerson.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.OPERATION_TAISHO_PGMID = " & setParam("operationTaishoPgmid", paramInfoList.Item("operationTaishoPgmid"), .operationTaishoPgmid.DBType, .operationTaishoPgmid.IntegerBu, .operationTaishoPgmid.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.UPDATE_DAY = " & setParam("updateDay", paramInfoList.Item("updateDay"), .updateDay.DBType, .updateDay.IntegerBu, .updateDay.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.UPDATE_TIME = " & setParam("updateTime", paramInfoList.Item("updateTime"), .updateTime.DBType, .updateTime.IntegerBu, .updateTime.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.UPDATE_PERSON = " & setParam("updatePerson", paramInfoList.Item("updatePerson"), .updatePerson.DBType, .updatePerson.IntegerBu, .updatePerson.DecimalBu) & ",")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.UPDATE_PGMID = " & setParam("updatePgmid", paramInfoList.Item("updatePgmid"), .updatePgmid.DBType, .updatePgmid.IntegerBu, .updatePgmid.DecimalBu))
            'WHERE 
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("    T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KEY =  CAST( " & setParam("tranKey", paramInfoList.Item("tranKey"), .tranKey.DBType, .tranKey.IntegerBu, .tranKey.DecimalBu) & " AS Char(46))")
            sqlString.AppendLine("    AND T_INT_YOYAKU_TRAN_MANAGEMENT.TRAN_KBN =  CAST( " & setParam("tranKbn", paramInfoList.Item("tranKbn"), .tranKbn.DBType, .tranKbn.IntegerBu, .tranKbn.DecimalBu) & " AS Char(1))")
            'End If
        End With
        Return sqlString.ToString
    End Function
#End Region
End Class
