''' <summary>
''' 連携済仕入原価
''' </summary>
''' <remarks>
''' Author:2018/10/06//PhucNH
''' </remarks>
Public Class RenShiireGenka_DA  '連携済仕入原価_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private rensiireGenkaEntity As New TRenSiireGenkaEntity()
#End Region

    ''' <summary>
    ''' 連携済仕入原価テーブルにデータを数える
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function countRenShiireGenkaByTargetDay(ByVal paramList As Hashtable) As Integer

        Dim resultdatatable As New DataTable
        Dim strsql As String = ""

        Try
            strsql &= "SELECT COUNT(1) "
            strsql &= "FROM T_REN_SIIRE_GENKA "
            With rensiireGenkaEntity
                strsql &= "WHERE KAIKEI_YM = " & setParam("KaikeiYm", paramList.Item("KaikeiYm"), .kaikeiYm.DBType, .kaikeiYm.IntegerBu, .kaikeiYm.DecimalBu)
            End With
            resultdatatable = getDataTable(strsql)
        Catch ex As Exception
            Throw
        End Try

        If resultdatatable.Rows.Count > 0 Then
            Return CInt(resultdatatable.Rows.Item(0).Item(0))
        Else
            Return 0
        End If

    End Function

End Class
