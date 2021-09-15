using System.Text;

public partial class Request_DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    public DataTable selectDataTable(Request_DASelectParam param)
    {
        var sb = new StringBuilder();
        var basicEnt = new CrsLedgerBasicEntity();
        var reqestEnt = new TWtRequestInfoEntity();

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT ");
        sb.AppendLine("    REQ.ENTRY_DAY AS ENTRY_DAY ");
        sb.AppendLine("FROM ");
        sb.AppendLine("  T_WT_REQUEST_INFO REQ ");
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_BASIC CLB ");
        sb.AppendLine("  ON REQ.SYUPT_DAY = CLB.SYUPT_DAY ");
        sb.AppendLine("  AND REQ.CRS_CD = CLB.CRS_CD ");
        sb.AppendLine("  AND REQ.GOUSYA = CLB.GOUSYA ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("  1 = 1 ");
        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  REQ.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, reqestEnt.SyuptDay));
        }
        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  REQ.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, reqestEnt.SyuptDay));
        }

        return base.getDataTable(sb.ToString());
    }

    public string setSelectParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, true);
    }

    public string setUpdateParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, false);
    }

    private string setParamEx(object value, IEntityKoumokuType ent, bool selFlg)
    {
        ParamNum += 1;
        if (selFlg == true && ent is EntityKoumoku_MojiType)
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType);
        }
        else
        {
            return base.setParam(ParamNum.ToString(), value, ent.DBType, ent.IntegerBu, ent.DecimalBu);
        }
    }

    private void clear()
    {
        base.paramClear();
        ParamNum = 0;
    }

    public partial class Request_DASelectParam
    {
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? SyuptDayFrom { get; set; }
        /// <summary>
        /// 出発日TO
        /// </summary>
        public int? SyuptDayTo { get; set; }
    }
}