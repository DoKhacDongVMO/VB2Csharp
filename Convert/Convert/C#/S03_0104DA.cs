using System.Text;

public partial class S03_0104DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    public DataTable selectDataTable(S03_0104DASelectParam param)
    {
        var sb = new StringBuilder();
        var basicEnt = new CrsLedgerBasicEntity();
        var groupEnt = new CrsLedgerOptionGroupEntity();
        var siireEnt = new MSiireSakiEntity();

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT ");
        sb.AppendLine("    TO_YYYYMMDD_FC(BSC.SYUPT_DAY) AS SYUPT_DAY_STR ");
        sb.AppendLine("  , BSC.SYUPT_DAY AS SYUPT_DAY ");
        sb.AppendLine("  , TO_HHMM_FC(BSC.SYUPT_TIME_1) AS SYUPT_TIME_STR ");
        sb.AppendLine("  , BSC.CRS_CD ");
        sb.AppendLine("  , BSC.GOUSYA ");
        sb.AppendLine("  , OPTG.OPTION_GROUP_NM AS OPTION_GROUP_NM ");
        sb.AppendLine("  , OPT.OPTIONAL_NAME AS OPTION_NM ");
        sb.AppendLine("  , SUM(NVL(YOPT.ADD_CHARGE_APPLICATION_NINZU, 0)) AS OPTION_NUM ");
        sb.AppendLine("  , NVL(BSC.YOYAKU_NUM_TEISEKI, 0) + NVL(BSC.YOYAKU_NUM_SUB_SEAT, 0) AS YOYAKU_NUM ");
        sb.AppendLine("  , COUNT(YOPT.ADD_CHARGE_APPLICATION_NINZU) OPTION_KENSU ");
        sb.AppendLine("  , OPT.HANBAI_TANKA AS HANBAI_TANKA ");
        sb.AppendLine("  , SUM( ");
        sb.AppendLine("    NVL(YOPT.ADD_CHARGE_APPLICATION_NINZU, 0) * NVL(YOPT.ADD_CHARGE_TANKA, 0) ");
        sb.AppendLine("  ) AS TOTAL_COST ");
        sb.AppendLine("  , NULL AS MEMO ");
        sb.AppendLine("  , SIIRE.SIIRE_SAKI_NAME ");
        sb.AppendLine("  , YOPT.GROUP_NO ");
        sb.AppendLine("  , YOPT.LINE_NO ");
        sb.AppendLine("FROM ");
        sb.AppendLine("  T_CRS_LEDGER_BASIC BSC ");
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_OPTION_GROUP OPTG ");
        sb.AppendLine("    ON BSC.CRS_CD = OPTG.CRS_CD ");
        sb.AppendLine("    AND BSC.SYUPT_DAY = OPTG.SYUPT_DAY ");
        sb.AppendLine("    AND BSC.GOUSYA = OPTG.GOUSYA ");
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_OPTION OPT ");
        sb.AppendLine("    ON OPTG.CRS_CD = OPT.CRS_CD ");
        sb.AppendLine("    AND OPTG.SYUPT_DAY = OPT.SYUPT_DAY ");
        sb.AppendLine("    AND OPTG.GOUSYA = OPT.GOUSYA ");
        sb.AppendLine("    AND OPTG.GROUP_NO = OPT.GROUP_NO ");
        sb.AppendLine("  LEFT JOIN M_SIIRE_SAKI SIIRE ");
        sb.AppendLine("    ON SIIRE.SIIRE_SAKI_CD = OPT.SIIRE_SAKI_CD ");
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_NO = OPT.SIIRE_SAKI_EDABAN ");
        sb.AppendLine("    AND SIIRE.DELETE_DATE IS NULL ");
        sb.AppendLine("  INNER JOIN T_YOYAKU_INFO_BASIC YBSC ");
        sb.AppendLine("    ON BSC.CRS_CD = YBSC.CRS_CD ");
        sb.AppendLine("    AND BSC.SYUPT_DAY = YBSC.SYUPT_DAY ");
        sb.AppendLine("    AND BSC.GOUSYA = YBSC.GOUSYA                  --予約のみ(WT,R除くなら) ");
        sb.AppendLine("    AND YBSC.YOYAKU_KBN BETWEEN 0 AND 9           --キャンセル除く ");
        sb.AppendLine("    AND YBSC.CANCEL_FLG IS NULL                   --削除日 ");
        sb.AppendLine("    AND YBSC.DELETE_DAY = 0 ");
        sb.AppendLine("  INNER JOIN T_YOYAKU_INFO_OPTION YOPT ");
        sb.AppendLine("    ON YBSC.YOYAKU_KBN = YOPT.YOYAKU_KBN ");
        sb.AppendLine("    AND YBSC.YOYAKU_NO = YOPT.YOYAKU_NO ");
        sb.AppendLine("    AND OPT.GROUP_NO = YOPT.GROUP_NO ");
        sb.AppendLine("    AND OPT.LINE_NO = YOPT.LINE_NO                --削除日 ");
        sb.AppendLine("    AND YOPT.DELETE_DAY = 0 ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("  1 = 1 ");
        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay));
        }
        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay));
        }
        // コースコード
        if (string.IsNullOrEmpty(param.CrsCd) == false)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd));
        }
        // 出発時間FROM
        if (param.SyuptTimeFrom is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.SYUPT_TIME_1 >= ").Append(setSelectParam(param.SyuptTimeFrom, basicEnt.syuptTime1));
        }
        // 出発時間TO
        if (param.SyuptTimeTo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.SYUPT_TIME_1 <= ").Append(setSelectParam(param.SyuptTimeTo, basicEnt.syuptTime1));
        }
        // 号車
        if (param.Gousya is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.GOUSYA >= ").Append(setSelectParam(param.Gousya, basicEnt.gousya));
        }
        // 仕入先コード
        if (string.IsNullOrEmpty(param.SiireSakiCd) == false)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE.SIIRE_SAKI_CD = ").Append(setSelectParam(param.SiireSakiCd, siireEnt.SiireSakiCd));
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE.SIIRE_SAKI_NO = ").Append(setSelectParam(param.SiireSakiCd, siireEnt.SiireSakiNo));
        }
        // グループ名称
        if (string.IsNullOrEmpty(param.GroupName) == false)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  OPTG.OPTION_GROUP_NM LIKE = ").Append(setSelectParam("%" + param.GroupName + "%", groupEnt.optionGroupNm));
        }

        sb.AppendLine("  AND HTBS.GET_CRS_STATUS_FC(BSC.TEIKI_KIKAKU_KBN,BSC.UNKYU_KBN,BSC.SAIKOU_KAKUTEI_KBN) = 1 ");
        sb.AppendLine("  AND BSC.MARU_ZOU_MANAGEMENT_KBN IS NULL ");
        sb.AppendLine("  AND BSC.DELETE_DAY = 0 ");
        sb.AppendLine("  AND BSC.OPTION_FLG = 'Y' ");
        sb.AppendLine("GROUP BY ");
        sb.AppendLine("    BSC.SYUPT_DAY ");
        sb.AppendLine("  , BSC.SYUPT_TIME_1 ");
        sb.AppendLine("  , BSC.CRS_CD ");
        sb.AppendLine("  , BSC.GOUSYA ");
        sb.AppendLine("  , BSC.YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("  , BSC.YOYAKU_NUM_SUB_SEAT ");
        sb.AppendLine("  , OPTG.OPTION_GROUP_NM ");
        sb.AppendLine("  , OPT.OPTIONAL_NAME ");
        sb.AppendLine("  , OPT.HANBAI_TANKA ");
        sb.AppendLine("  , YOPT.GROUP_NO ");
        sb.AppendLine("  , YOPT.LINE_NO ");
        sb.AppendLine("  , SIIRE.SIIRE_SAKI_NAME ");
        sb.AppendLine("ORDER BY ");
        sb.AppendLine("    BSC.SYUPT_DAY ");
        sb.AppendLine("  , BSC.CRS_CD ");
        sb.AppendLine("  , BSC.SYUPT_TIME_1 ");
        sb.AppendLine("  , YOPT.GROUP_NO ");
        sb.AppendLine("  , YOPT.LINE_NO ");
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

    public partial class S03_0104DASelectParam
    {
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? SyuptDayFrom { get; set; }
        /// <summary>
        /// 出発日TO
        /// </summary>
        public int? SyuptDayTo { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 出発時間FROM
        /// </summary>
        public int? SyuptTimeFrom { get; set; }
        /// <summary>
        /// 出発時間TO
        /// </summary>
        public int? SyuptTimeTo { get; set; }
        /// <summary>
        /// 号車
        /// </summary>
        public int? Gousya { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// グループ名称
        /// </summary>
        public string GroupName { get; set; }
    }
}