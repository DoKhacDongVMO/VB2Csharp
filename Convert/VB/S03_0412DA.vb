Imports System.Text
''' <summary>
''' 手仕舞い連絡のDAクラス
''' </summary>
Public Class S03_0412DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
    '手仕舞区分 = 'Y'
    Private Const TEJIMAI_KBN_SUMI As String = "Y"
#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' 検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTable(ByVal param As S03_0412DASelectParam) As DataTable
        Dim sb As New StringBuilder
        Dim basicEnt As New CrsLedgerBasicEntity

        'パラメータクリア
        clear()

        sb.AppendLine("SELECT")
        sb.AppendLine(" CASE WHEN KOSHA.SEND_YMDT IS NOT NULL THEN ")
        sb.AppendLine(" 1  ")
        sb.AppendLine("  Else 0 END AS OUTPUT_FLG,  ")
        sb.AppendLine(" CASE WHEN KOSHA.SEND_YMDT IS NOT NULL  AND KOSHA.SIIRE_SAKI_KIND_CD = '30' THEN   ")
        sb.AppendLine("  1 ")
        sb.AppendLine("  Else 0 END AS KAGAMI_FLG, ")
        sb.AppendLine("  TO_YYYYMMDD_FC(BASIC.SYUPT_DAY)  AS SYUPT_DAY_STR,  ")
        sb.AppendLine("  BASIC.SYUPT_DAY AS SYUPT_DAY,  ")
        sb.AppendLine("  BASIC.CRS_CD AS CRS_CD, ")
        sb.AppendLine("  KOSHA.DAILY AS DAILY, ")
        sb.AppendLine("  KOSHA.KOSHAKASHO_CD AS SIIRE_SAKI_CD, ")
        sb.AppendLine("  KOSHA.KOSHAKASHO_EDABAN AS SIIRE_SAKI_NO, ")
        sb.AppendLine("  KOSHA.SIIRE_SAKI_NAME AS SIIRE_SAKI_NAME,  ")
        sb.AppendLine("  KOSHA.CODE_NAME AS SIIRE_SAKI_KIND, ")
        sb.AppendLine("  KOSHA.NOTIFICATION_HOHO, ")

        sb.AppendLine("  CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL　THEN  ")
        sb.AppendLine("  KOSHA.MAIL  ")
        sb.AppendLine("  ELSE KOSHA.T_CEA_CONTROL_MAIL END AS MAIL,  ")
        sb.AppendLine("  KOSHA.FAX_1, ")
        sb.AppendLine("  CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL THEN ")
        sb.AppendLine("  CASE WHEN KOSHA.FAX_1_STR = '--' THEN  ")
        sb.AppendLine("  ''  ")
        sb.AppendLine("  ELSE KOSHA.FAX_1_STR END  ")
        sb.AppendLine("  ELSE CONCAT(CONCAT(CONCAT(CONCAT(KOSHA.FAX1_1,'-'),KOSHA.FAX1_2),'-'),KOSHA.FAX1_3) END AS FAX_1_STR  ")
        sb.AppendLine("  ,FAX_2  ")
        sb.AppendLine("  ,CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL THEN  ")
        sb.AppendLine("  CASE WHEN KOSHA.FAX_2_STR = '--' THEN  ")
        sb.AppendLine("  ''  ")
        sb.AppendLine("  ELSE KOSHA.FAX_2_STR  END  ")
        sb.AppendLine("  ELSE CONCAT(CONCAT(CONCAT(CONCAT(KOSHA.FAX2_1,'-'),KOSHA.FAX2_2),'-'),KOSHA.FAX2_3) END AS FAX_2_STR  ")
        sb.AppendLine("  , TO_CHAR(KOSHA.SEND_YMDT, 'yyyy/mm/dd hh24:mi:ss')  AS LAST_SEND_DAY  ")
        sb.AppendLine("  ,KOSHA.ROOM_MAX_CAPACITY  ")
        sb.AppendLine("  ,KOSHA.SIIRE_SAKI_KIND_CD  ")

        sb.AppendLine("  ,CASE WHEN KOSHA.SYSTEM_ENTRY_DAY IS NOT NULL THEN  ")
        sb.AppendLine("  1 ")
        sb.AppendLine("  Else 0 END AS ALLOW_EDIT ")

        sb.AppendLine("  FROM T_CRS_LEDGER_BASIC BASIC ")
        sb.AppendLine("  INNER JOIN (SELECT ")
        sb.AppendLine(" T_CRS_LEDGER_KOSHAKASHO.SYUPT_DAY ")
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.CRS_CD   ")
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_CD  ")
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_EDABAN  ")
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.DAILY  ")
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.RIYOU_DAY  ")
        sb.AppendLine(" ,CODE_SK.CODE_NAME  ")
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_NAME  ")
        sb.AppendLine(" ,SIIRE.NOTIFICATION_HOHO  ")
        sb.AppendLine(" ,SIIRE.MAIL  ")
        sb.AppendLine(" ,SIIRE.FAX_1  ")
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_1_1,'-'),SIIRE.FAX_1_2),'-'),SIIRE.FAX_1_3) AS FAX_1_STR  ")
        sb.AppendLine(" ,SIIRE.FAX_2 ")
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_2_1,'-'),SIIRE.FAX_2_2),'-'),SIIRE.FAX_2_3) AS FAX_2_STR  ")
        sb.AppendLine(" ,SIIRE.ROOM_MAX_CAPACITY  ")
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_KIND_CD  ")
        sb.AppendLine(" ,CONTROL.SEND_YMDT ")
        sb.AppendLine(",CONTROL.MAIL AS T_CEA_CONTROL_MAIL ")
        sb.AppendLine(",CONTROL.FAX1 ")
        sb.AppendLine(",CONTROL.FAX2 ")
        sb.AppendLine(",CONTROL.FAX1_1 ")
        sb.AppendLine(",CONTROL.FAX1_2 ")
        sb.AppendLine(" ,CONTROL.FAX1_3 ")
        sb.AppendLine(",CONTROL.FAX2_1 ")
        sb.AppendLine(",CONTROL.FAX2_2 ")
        sb.AppendLine(" ,CONTROL.FAX2_3 ")
        sb.AppendLine(" ,CONTROL.SYSTEM_ENTRY_DAY ")

        sb.AppendLine("  FROM T_CRS_LEDGER_KOSHAKASHO ")
        sb.AppendLine("  LEFT JOIN (SELECT ")
        sb.AppendLine(" SIIRE_SAKI_CD  ")
        sb.AppendLine(" ,SIIRE_SAKI_NO  ")
        sb.AppendLine(" ,SIIRE_SAKI_KIND_CD  ")
        sb.AppendLine(" ,SIIRE_SAKI_NAME  ")
        sb.AppendLine(" ,NOTIFICATION_HOHO  ")
        sb.AppendLine(" ,MAIL  ")
        sb.AppendLine(" ,FAX_1  ")
        sb.AppendLine(" ,FAX_1_1  ")
        sb.AppendLine(" ,FAX_1_2  ")
        sb.AppendLine(" ,FAX_1_3  ")
        sb.AppendLine(" ,FAX_2  ")
        sb.AppendLine(" ,FAX_2_1  ")
        sb.AppendLine(" ,FAX_2_2  ")
        sb.AppendLine(" ,FAX_2_3  ")
        sb.AppendLine(" ,ROOM_MAX_CAPACITY  ")

        sb.AppendLine("  FROM M_SIIRE_SAKI ")
        sb.AppendLine("  WHERE DELETE_DATE IS NULL AND SIIRE_SAKI_KIND_CD = ").Append(SuppliersKind_Koshakasho).Append(" ) SIIRE ")
        sb.AppendLine("  ON KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("  AND KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("  LEFT JOIN M_CODE CODE_SK ")
        sb.AppendLine("  On SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        sb.AppendLine("  And CODE_SK.CODE_BUNRUI = ").Append(CodeBunrui.siireKind)
        sb.AppendLine("  And CODE_SK.DELETE_DATE Is NULL ")
        sb.AppendLine("  LEFT JOIN T_CEA_CONTROL CONTROL  ")
        sb.AppendLine("  ON T_CRS_LEDGER_KOSHAKASHO.CRS_CD = CONTROL.CRS_CD  ")
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.DAILY = CONTROL.DAILY ")
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_CD = CONTROL.SIIRE_SAKI_CD ")
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_EDABAN = CONTROL.SIIRE_SAKI_NO ")

        sb.AppendLine("  UNION ALL ")
        sb.AppendLine("  SELECT ")
        sb.AppendLine("  T_CRS_LEDGER_HOTEL.SYUPT_DAY	 ")
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.CRS_CD	 ")
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.SIIRE_SAKI_CD As KOSHAKASHO_CD	 ")
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.SIIRE_SAKI_EDABAN As KOSHAKASHO_EDABAN	 ")
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.DAILY	 ")
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.RIYOU_DAY	 ")
        sb.AppendLine(" ,CODE_SK.CODE_NAME	 ")
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_NAME	 ")
        sb.AppendLine(" ,SIIRE.NOTIFICATION_HOHO	 ")
        sb.AppendLine(" ,SIIRE.MAIL	 ")
        sb.AppendLine(" ,SIIRE.FAX_1	 ")
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_1_1,'-'),SIIRE.FAX_1_2),'-'),SIIRE.FAX_1_3) AS FAX_1_STR	 ")
        sb.AppendLine(" ,SIIRE.FAX_2	 ")
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_2_1,'-'),SIIRE.FAX_2_2),'-'),SIIRE.FAX_2_3) AS FAX_2_STR	 ")
        sb.AppendLine(" ,SIIRE.ROOM_MAX_CAPACITY	 ")
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_KIND_CD AS SIIRE_SAKI_KIND_CD ")
        sb.AppendLine(" ,CONTROL.SEND_YMDT ")
        sb.AppendLine(",CONTROL.MAIL AS T_CEA_CONTROL_MAIL ")
        sb.AppendLine(",CONTROL.FAX1 ")
        sb.AppendLine(",CONTROL.FAX2 ")
        sb.AppendLine(",CONTROL.FAX1_1 ")
        sb.AppendLine(",CONTROL.FAX1_2 ")
        sb.AppendLine(" ,CONTROL.FAX1_3 ")
        sb.AppendLine(",CONTROL.FAX2_1 ")
        sb.AppendLine(",CONTROL.FAX2_2 ")
        sb.AppendLine(" ,CONTROL.FAX2_3 ")
        sb.AppendLine(" ,CONTROL.SYSTEM_ENTRY_DAY ")

        sb.AppendLine("  FROM T_CRS_LEDGER_HOTEL ")
        sb.AppendLine("  LEFT JOIN (SELECT ")
        sb.AppendLine("  SIIRE_SAKI_CD AS SIIRE_CD")
        sb.AppendLine(" ,SIIRE_SAKI_NO")
        sb.AppendLine(" ,SIIRE_SAKI_KIND_CD")
        sb.AppendLine(" ,SIIRE_SAKI_NAME")
        sb.AppendLine(" ,NOTIFICATION_HOHO")
        sb.AppendLine(" ,MAIL")
        sb.AppendLine(" ,FAX_1")
        sb.AppendLine(" ,FAX_1_1")
        sb.AppendLine(" ,FAX_1_2")
        sb.AppendLine(" ,FAX_1_3")
        sb.AppendLine(" ,FAX_2")
        sb.AppendLine(" ,FAX_2_1")
        sb.AppendLine(" ,FAX_2_2")
        sb.AppendLine(" ,FAX_2_3")
        sb.AppendLine(" ,ROOM_MAX_CAPACITY")

        sb.AppendLine("  FROM M_SIIRE_SAKI ")
        sb.AppendLine("  WHERE DELETE_DATE IS NULL AND SIIRE_SAKI_KIND_CD = ").Append(SuppliersKind_Stay).Append(" ) SIIRE ")
        sb.AppendLine("  ON SIIRE_SAKI_CD = SIIRE.SIIRE_CD ")
        sb.AppendLine("  AND SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("  LEFT JOIN M_CODE CODE_SK ")
        sb.AppendLine("  ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ")
        sb.AppendLine("  AND CODE_SK.CODE_BUNRUI = ").Append(CodeBunrui.siireKind)
        sb.AppendLine("  AND CODE_SK.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN T_CEA_CONTROL CONTROL ")
        sb.AppendLine("  ON T_CRS_LEDGER_HOTEL.CRS_CD = CONTROL.CRS_CD ")
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.DAILY = CONTROL.DAILY ")
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.SIIRE_SAKI_CD = CONTROL.SIIRE_SAKI_CD ")
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.SIIRE_SAKI_EDABAN = CONTROL.SIIRE_SAKI_NO ")
        sb.AppendLine("  )KOSHA ")
        sb.AppendLine("  ON BASIC.SYUPT_DAY = KOSHA.SYUPT_DAY ")
        sb.AppendLine("  AND BASIC.CRS_CD = KOSHA.CRS_CD ")
        sb.AppendLine("WHERE ")
        '出発日FROM
        sb.AppendLine("  BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay))

        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay))
        End If
        sb.AppendLine(" AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd))
        sb.AppendLine("  AND BASIC.TEIKI_KIKAKU_KBN = '" & Teiki_KikakuKbnType.kikakuTravel & "'")
        sb.AppendLine(" AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'*') <> '" & MaruzouKanriKbn.Maruzou & "'")
        sb.AppendLine(" AND BASIC.SAIKOU_KAKUTEI_KBN = '" & SaikouKakuteiKbn.Saikou & "'")
        sb.AppendLine("  AND NVL(BASIC.DELETE_DAY,0) = 0 ")
        sb.AppendLine("  AND BASIC.TEJIMAI_KBN = '" & TEJIMAI_KBN_SUMI & "'")
        sb.AppendLine("  ORDER BY ")
        sb.AppendLine("  BASIC.SYUPT_DAY")
        sb.AppendLine(" ,KOSHA.DAILY")
        sb.AppendLine(" ,KOSHA.KOSHAKASHO_CD")
        sb.AppendLine(" ,KOSHA.KOSHAKASHO_EDABAN")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function

    Private Function setParamEx(ByVal value As Object, ByVal ent As IEntityKoumokuType, ByVal selFlg As Boolean) As String
        ParamNum += 1
        If selFlg = True AndAlso TypeOf ent Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType)
        Else
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function
    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
#End Region


#Region " UPDATE処理 "

    ''' <summary>
    ''' 更新処理を呼び出す
    ''' </summary>
    ''' <param name="UpdateListData"></param>
    ''' <returns>Integer</returns>
    Public Function updateTable(ByVal UpdateListData As List(Of S03_0412DAUpdateParam)) As Integer
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Dim con As OracleConnection = Nothing

        'コネクション開始
        con = openCon()

        Try
            Dim sizeData As Integer = UpdateListData.Count
            For row As Integer = 0 To sizeData - 1
                sqlString = getUpdateQuery(UpdateListData(row))

                returnValue += execNonQuery(con, sqlString)
            Next
        Catch ex As Exception
            Throw
        Finally
            If con IsNot Nothing Then
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                If con IsNot Nothing Then
                    con.Dispose()
                End If
            End If
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' UPDATE用DBアクセス
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>Integer</returns>
    Private Function getUpdateQuery(ByVal param As S03_0412DAUpdateParam) As String
        Dim tCeaControlEtt As New TCeaControlEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'UPDATE
        sb.AppendLine(" UPDATE T_CEA_CONTROL ")
        sb.AppendLine(" SET ")
        '通知方法
        sb.AppendLine(" SEND_KIND = ").Append(setUpdateParam(param.ContactWay, tCeaControlEtt.SendKind))
        '最終送信日時
        sb.AppendLine(" ,SEND_YMDT  = TO_DATE( '" & param.LastSendDay & "', 'YYYY/MM/DD HH24:MI:SS')")
        '最終編集出発日
        sb.AppendLine(" ,LAST_SYUPT_DAY  = ").Append(setUpdateParam(param.SyuptDay, tCeaControlEtt.LastSyuptDay))
        'システム更新日
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY  = ").Append(setUpdateParam(param.SystemUpdateDay, tCeaControlEtt.SystemUpdateDay))
        'システム更新ＰＧＭＩＤ
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID  = ").Append(setUpdateParam(param.SystemUpdatePgmid, tCeaControlEtt.SystemUpdatePgmid))
        'システム更新者コード
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD  = ").Append(setUpdateParam(param.SystemUpdatePersonCd, tCeaControlEtt.SystemUpdatePersonCd))

        'WHERE句
        sb.AppendLine(" WHERE ")
        'コースコード
        sb.AppendLine(" CRS_CD = ").Append(setUpdateParam(param.CrsCd, tCeaControlEtt.CrsCd))
        sb.AppendLine(" AND DAILY = ").Append(setUpdateParam(param.Daily, tCeaControlEtt.Daily))
        sb.AppendLine(" AND SIIRE_SAKI_CD = ").Append(setUpdateParam(param.SiireSakiCd, tCeaControlEtt.SiireSakiCd))
        sb.AppendLine(" AND SIIRE_SAKI_NO = ").Append(setUpdateParam(param.SiireSakiNo, tCeaControlEtt.SiireSakiNo))

        Return sb.ToString

    End Function

    Public Function setUpdateParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, False)
    End Function

#End Region
#Region " パラメータ "
    Public Class S03_0412DASelectParam
        ''' <summary>
        ''' 出発日FROM
        ''' </summary>
        Public Property SyuptDayFrom As Integer?
        ''' <summary>
        ''' 出発日TO
        ''' </summary>
        Public Property SyuptDayTo As Integer?
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
    End Class

    Public Class S03_0412DAUpdateParam
        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As Integer
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 通知方法
        ''' </summary>
        Public Property ContactWay As Integer

        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As String

        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String

        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 最終送信日時
        ''' </summary>
        Public Property LastSendDay As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String

    End Class
#End Region
End Class
