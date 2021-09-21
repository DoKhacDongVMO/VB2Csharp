using System;
using System.Text;

/// <summary>
/// ＡＧＥＮＴ請求書（キャンセル料）Da
/// </summary>
public partial class P05_0604Da : DataAccessorBase
{

    #region メソッド

    /// <summary>
    /// 請求書NO取得
    /// </summary>
    /// <returns>請求書NO</returns>
    public DataTable getSeikyushoNo()
    {
        DataTable seikyushoNoData;
        try
        {
            string query = createSeikyushoNoSql();
            seikyushoNoData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return seikyushoNoData;
    }

    /// <summary>
    /// 代理店情報取得S
    /// </summary>
    /// <param name="entity">代理店マスタEntity</param>
    /// <returns>代理店情報</returns>
    public DataTable getAgetnInfoSql(MAgentEntity entity)
    {
        DataTable agentInfo;
        try
        {
            string query = createAgetnInfoSql(entity);
            agentInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return agentInfo;
    }

    /// <summary>
    /// 発行元情報取得
    /// </summary>
    /// <param name="entity">コードマスタEntity</param>
    /// <returns>発行元情報</returns>
    public DataTable getIssueInfoSql(MCodeEntity entity)
    {
        DataTable issueInfo;
        try
        {
            string query = createIssueInfoSql(entity);
            issueInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return issueInfo;
    }

    /// <summary>
    /// 振込口座情報取得
    /// </summary>
    /// <param name="entity">振込口座マスタEntity</param>
    /// <returns>振込口座情報</returns>
    public DataTable getHurikomiInfo(MHurikomiKozaEntity entity)
    {
        DataTable hurikomiKozaInfo;
        try
        {
            string query = createHurikomiInfoSql(entity);
            hurikomiKozaInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return hurikomiKozaInfo;
    }







    /// <summary>
    /// 請求書NO取得SQL作成
    /// </summary>
    /// <returns>請求書NO取得SQL</returns>
    private string createSeikyushoNoSql()
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.Append("SELECT HTBS.AGENT_SEIKYU_SEQ.NEXTVAL AS SEIKYUSYO_NO FROM DUAL");
        return sb.ToString();
    }

    /// <summary>
    /// 代理店情報取得SQL作成
    /// </summary>
    /// <param name="entity">代理店マスタEntity</param>
    /// <returns>代理店情報取得SQL</returns>
    private string createAgetnInfoSql(MAgentEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine("SELECT ");
        sb.AppendLine("     AGENT_CD ");
        sb.AppendLine("    ,YUBIN_NO ");
        sb.AppendLine("    ,ADDRESS_1 ");
        sb.AppendLine("    ,ADDRESS_2 ");
        sb.AppendLine("    ,ADDRESS_3 ");
        sb.AppendLine("    ,AGENT_NAME ");
        sb.AppendLine("    ,COMPANY_NAME ");
        sb.AppendLine("    ,BRANCH_NAME ");
        sb.AppendLine("    ,SEIKYU_SAKI_CD ");
        sb.AppendLine("FROM ");
        sb.AppendLine("    M_AGENT ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("    AGENT_CD = " + base.setParam(entity.AgentCd.PhysicsName, entity.AgentCd.Value, entity.AgentCd.DBType, entity.AgentCd.IntegerBu, entity.AgentCd.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 発行元情報取得SQL作成
    /// </summary>
    /// <param name="entity">コードマスタEntity</param>
    /// <returns>発行元情報取得SQL</returns>
    private string createIssueInfoSql(MCodeEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine("SELECT ");
        sb.AppendLine("     CODE_VALUE ");
        sb.AppendLine("    ,CODE_NAME ");
        sb.AppendLine("    ,NAIYO_1 ");
        sb.AppendLine("FROM ");
        sb.AppendLine("    M_CODE ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("    CODE_BUNRUI = " + base.setParam(entity.CodeBunrui.PhysicsName, entity.CodeBunrui.Value, entity.CodeBunrui.DBType, entity.CodeBunrui.IntegerBu, entity.CodeBunrui.DecimalBu));
        sb.AppendLine("    AND ");
        sb.AppendLine("    CODE_VALUE IN('102', '103', '104', '105', '106') ");
        return sb.ToString();
    }

    /// <summary>
    /// 振込口座情報取得SQL作成
    /// </summary>
    /// <param name="entity">振込口座マスタEntity</param>
    /// <returns>振込口座情報取得SQL</returns>
    private string createHurikomiInfoSql(MHurikomiKozaEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine("SELECT ");
        sb.AppendLine("     HURIKOMI_SAKI_BANK_NAME ");
        sb.AppendLine("    ,HURIKOMI_SAKI_BRANCH_NAME ");
        sb.AppendLine("    ,YOKIN_EVENT ");
        sb.AppendLine("    ,CASE YOKIN_EVENT WHEN '1' THEN '普通' ");
        sb.AppendLine("    　                 WHEN '2' THEN '当座' ");
        sb.AppendLine("    　ELSE '他' END AS YOKIN_EVENT_NAME ");
        sb.AppendLine("    ,KOZA_NO ");
        sb.AppendLine("    ,HURIKOMI_SAKI_KOZA_NAME ");
        sb.AppendLine("FROM ");
        sb.AppendLine("    M_HURIKOMI_KOZA ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("    HURIKOMI_SEIKYUSYO_FOR_FLG = " + base.setParam(entity.HurikomiSeikyusyoForFlg.PhysicsName, entity.HurikomiSeikyusyoForFlg.Value, entity.HurikomiSeikyusyoForFlg.DBType, entity.HurikomiSeikyusyoForFlg.IntegerBu, entity.HurikomiSeikyusyoForFlg.DecimalBu));
        return sb.ToString();
    }

    #endregion

}