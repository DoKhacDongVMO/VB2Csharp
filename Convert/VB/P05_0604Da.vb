Imports System.Text

''' <summary>
''' ＡＧＥＮＴ請求書（キャンセル料）Da
''' </summary>
Public Class P05_0604Da
    Inherits DataAccessorBase

#Region "メソッド"

    ''' <summary>
    ''' 請求書NO取得
    ''' </summary>
    ''' <returns>請求書NO</returns>
    Public Function getSeikyushoNo() As DataTable

        Dim seikyushoNoData As DataTable

        Try
            Dim query = Me.createSeikyushoNoSql()
            seikyushoNoData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return seikyushoNoData
    End Function

    ''' <summary>
    ''' 代理店情報取得S
    ''' </summary>
    ''' <param name="entity">代理店マスタEntity</param>
    ''' <returns>代理店情報</returns>
    Public Function getAgetnInfoSql(entity As MAgentEntity) As DataTable

        Dim agentInfo As DataTable

        Try
            Dim query As String = Me.createAgetnInfoSql(entity)
            agentInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return agentInfo
    End Function

    ''' <summary>
    ''' 発行元情報取得
    ''' </summary>
    ''' <param name="entity">コードマスタEntity</param>
    ''' <returns>発行元情報</returns>
    Public Function getIssueInfoSql(entity As MCodeEntity) As DataTable

        Dim issueInfo As DataTable

        Try
            Dim query As String = Me.createIssueInfoSql(entity)
            issueInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return issueInfo
    End Function

    ''' <summary>
    ''' 振込口座情報取得
    ''' </summary>
    ''' <param name="entity">振込口座マスタEntity</param>
    ''' <returns>振込口座情報</returns>
    Public Function getHurikomiInfo(entity As MHurikomiKozaEntity) As DataTable

        Dim hurikomiKozaInfo As DataTable

        Try
            Dim query As String = Me.createHurikomiInfoSql(entity)
            hurikomiKozaInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return hurikomiKozaInfo
    End Function







    ''' <summary>
    ''' 請求書NO取得SQL作成
    ''' </summary>
    ''' <returns>請求書NO取得SQL</returns>
    Private Function createSeikyushoNoSql() As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()

        sb.Append("SELECT HTBS.AGENT_SEIKYU_SEQ.NEXTVAL AS SEIKYUSYO_NO FROM DUAL")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 代理店情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">代理店マスタEntity</param>
    ''' <returns>代理店情報取得SQL</returns>
    Private Function createAgetnInfoSql(entity As MAgentEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()

        sb.AppendLine("SELECT ")
        sb.AppendLine("     AGENT_CD ")
        sb.AppendLine("    ,YUBIN_NO ")
        sb.AppendLine("    ,ADDRESS_1 ")
        sb.AppendLine("    ,ADDRESS_2 ")
        sb.AppendLine("    ,ADDRESS_3 ")
        sb.AppendLine("    ,AGENT_NAME ")
        sb.AppendLine("    ,COMPANY_NAME ")
        sb.AppendLine("    ,BRANCH_NAME ")
        sb.AppendLine("    ,SEIKYU_SAKI_CD ")
        sb.AppendLine("FROM ")
        sb.AppendLine("    M_AGENT ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("    AGENT_CD = " + MyBase.setParam(entity.AgentCd.PhysicsName, entity.AgentCd.Value, entity.AgentCd.DBType, entity.AgentCd.IntegerBu, entity.AgentCd.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 発行元情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">コードマスタEntity</param>
    ''' <returns>発行元情報取得SQL</returns>
    Private Function createIssueInfoSql(entity As MCodeEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()

        sb.AppendLine("SELECT ")
        sb.AppendLine("     CODE_VALUE ")
        sb.AppendLine("    ,CODE_NAME ")
        sb.AppendLine("    ,NAIYO_1 ")
        sb.AppendLine("FROM ")
        sb.AppendLine("    M_CODE ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("    CODE_BUNRUI = " + MyBase.setParam(entity.CodeBunrui.PhysicsName, entity.CodeBunrui.Value, entity.CodeBunrui.DBType, entity.CodeBunrui.IntegerBu, entity.CodeBunrui.DecimalBu))
        sb.AppendLine("    AND ")
        sb.AppendLine("    CODE_VALUE IN('102', '103', '104', '105', '106') ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 振込口座情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">振込口座マスタEntity</param>
    ''' <returns>振込口座情報取得SQL</returns>
    Private Function createHurikomiInfoSql(entity As MHurikomiKozaEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()

        sb.AppendLine("SELECT ")
        sb.AppendLine("     HURIKOMI_SAKI_BANK_NAME ")
        sb.AppendLine("    ,HURIKOMI_SAKI_BRANCH_NAME ")
        sb.AppendLine("    ,YOKIN_EVENT ")
        sb.AppendLine("    ,CASE YOKIN_EVENT WHEN '1' THEN '普通' ")
        sb.AppendLine("    　                 WHEN '2' THEN '当座' ")
        sb.AppendLine("    　ELSE '他' END AS YOKIN_EVENT_NAME ")
        sb.AppendLine("    ,KOZA_NO ")
        sb.AppendLine("    ,HURIKOMI_SAKI_KOZA_NAME ")
        sb.AppendLine("FROM ")
        sb.AppendLine("    M_HURIKOMI_KOZA ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("    HURIKOMI_SEIKYUSYO_FOR_FLG = " + MyBase.setParam(entity.HurikomiSeikyusyoForFlg.PhysicsName, entity.HurikomiSeikyusyoForFlg.Value, entity.HurikomiSeikyusyoForFlg.DBType, entity.HurikomiSeikyusyoForFlg.IntegerBu, entity.HurikomiSeikyusyoForFlg.DecimalBu))

        Return sb.ToString()
    End Function

#End Region

End Class
