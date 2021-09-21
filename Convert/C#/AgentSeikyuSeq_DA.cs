/// <summary>
/// AGENT請求書シーケンス
/// </summary>
/// <remarks>
/// Author:2018/10/06//PhucNH
/// </remarks>
using System;

public partial class AgentSeikyuSeq_DA : DataAccessorBase
{

    #region  定数／変数 

    #endregion

    /// <summary>
    /// AGENT請求書シーケンスから　請求書No　を取得する
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string getInvoiceNo()
    {
        var resultdatatable = new DataTable();
        string strsql = "";
        try
        {
            strsql += "SELECT AGENT_SEIKYU_SEQ.nextval FROM dual ";
            resultdatatable = getDataTable(strsql);
        }
        catch (Exception ex)
        {
            throw;
        }

        return resultdatatable.Rows(0).Item(0).ToString;
    }
}