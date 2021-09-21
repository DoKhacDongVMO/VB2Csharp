''' <summary>
''' AGENT請求書シーケンス
''' </summary>
''' <remarks>
''' Author:2018/10/06//PhucNH
''' </remarks>
Public Class AgentSeikyuSeq_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

#End Region

    ''' <summary>
    ''' AGENT請求書シーケンスから　請求書No　を取得する
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getInvoiceNo() As String

        Dim resultdatatable As New DataTable
        Dim strsql As String = ""

        Try
            strsql &= "SELECT AGENT_SEIKYU_SEQ.nextval FROM dual "
            resultdatatable = getDataTable(strsql)
        Catch ex As Exception
            Throw
        End Try

        Return resultdatatable.Rows(0).Item(0).ToString
    End Function

End Class
