Imports System.Text

Public Class Request_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

    Public Function selectDataTable(ByVal param As Request_DASelectParam) As DataTable
        Dim sb As New StringBuilder
        Dim basicEnt As New CrsLedgerBasicEntity
        Dim reqestEnt As New TWtRequestInfoEntity

        'パラメータクリア
        clear()

        sb.AppendLine("SELECT ")
        sb.AppendLine("    REQ.ENTRY_DAY AS ENTRY_DAY ")


        sb.AppendLine("FROM ")
        sb.AppendLine("  T_WT_REQUEST_INFO REQ ")
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_BASIC CLB ")
        sb.AppendLine("  ON REQ.SYUPT_DAY = CLB.SYUPT_DAY ")
        sb.AppendLine("  AND REQ.CRS_CD = CLB.CRS_CD ")
        sb.AppendLine("  AND REQ.GOUSYA = CLB.GOUSYA ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("  1 = 1 ")
        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  REQ.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, reqestEnt.SyuptDay))
        End If
        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  REQ.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, reqestEnt.SyuptDay))
        End If



        Return MyBase.getDataTable(sb.ToString)
    End Function

    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function

    Public Function setUpdateParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, False)
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

    Public Class Request_DASelectParam
        ''' <summary>
        ''' 出発日FROM
        ''' </summary>
        Public Property SyuptDayFrom As Integer?
        ''' <summary>
        ''' 出発日TO
        ''' </summary>
        Public Property SyuptDayTo As Integer?


    End Class


End Class
