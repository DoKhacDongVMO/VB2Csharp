''' <summary>
''' 代理店言語別コミッションマスタ
''' </summary>
''' <remarks>
''' Author:2018/10/06//PhucNH
''' </remarks>
Public Class AgentLanguageCommission_DA  '代理店言語別コミッションマスタ_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private agentLanguageCommissionEntity As New MAgentLanguageCommissionEntity()
#End Region

    ''' <summary>
    ''' 代理店言語別コミッションマスタでコミッションを取得する
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getComValue(ByVal paramList As Hashtable) As DataTable

        Dim resultdatatable As New DataTable
        Dim strsql As String = ""

        Try
            strsql &= "SELECT COM "
            strsql &= "FROM M_AGENT_LANGUAGE_COMMISSION "
            With agentLanguageCommissionEntity
                strsql &= "WHERE AGENT_CD = " & setParam("AgentCd", paramList.Item("AgentCd"), .agentCd.DBType, .agentCd.IntegerBu, .agentCd.DecimalBu)
                strsql &= "        AND TEIKI_KIKAKU_KBN =  " & setParam("TeikiKikakuKbn", paramList.Item("TeikiKikakuKbn"), .teikiKikakuKbn.DBType, .teikiKikakuKbn.IntegerBu, .teikiKikakuKbn.DecimalBu)
                strsql &= "        AND LANGUAGE_KBN = " & setParam("LanguageKbn", paramList.Item("LanguageKbn"), .languageKbn.DBType, .languageKbn.IntegerBu, .languageKbn.DecimalBu)
            End With
            resultdatatable = getDataTable(strsql)
        Catch ex As Exception
            Throw
        End Try

        Return resultdatatable
    End Function

End Class
