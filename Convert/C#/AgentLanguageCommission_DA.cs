/// <summary>
/// 代理店言語別コミッションマスタ
/// </summary>
/// <remarks>
/// Author:2018/10/06//PhucNH
/// </remarks>
using System;
using System.Collections;

public partial class AgentLanguageCommission_DA : DataAccessorBase  // 代理店言語別コミッションマスタ_DA
{

    #region  定数／変数 
    private MAgentLanguageCommissionEntity agentLanguageCommissionEntity = new MAgentLanguageCommissionEntity();
    #endregion

    /// <summary>
    /// 代理店言語別コミッションマスタでコミッションを取得する
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable getComValue(Hashtable paramList)
    {
        var resultdatatable = new DataTable();
        string strsql = "";
        try
        {
            strsql += "SELECT COM ";
            strsql += "FROM M_AGENT_LANGUAGE_COMMISSION ";
            {
                var withBlock = agentLanguageCommissionEntity;
                strsql += "WHERE AGENT_CD = " + setParam("AgentCd", paramList["AgentCd"], withBlock.agentCd.DBType, withBlock.agentCd.IntegerBu, withBlock.agentCd.DecimalBu);
                strsql += "        AND TEIKI_KIKAKU_KBN =  " + setParam("TeikiKikakuKbn", paramList["TeikiKikakuKbn"], withBlock.teikiKikakuKbn.DBType, withBlock.teikiKikakuKbn.IntegerBu, withBlock.teikiKikakuKbn.DecimalBu);
                strsql += "        AND LANGUAGE_KBN = " + setParam("LanguageKbn", paramList["LanguageKbn"], withBlock.languageKbn.DBType, withBlock.languageKbn.IntegerBu, withBlock.languageKbn.DecimalBu);
            }

            resultdatatable = getDataTable(strsql);
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultdatatable;
    }
}