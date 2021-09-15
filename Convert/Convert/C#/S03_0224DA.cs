using System.Text;

public partial class S03_0224DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    public DataTable selectDataTable(S03_0224DASelectParam param)
    {
        var sb = new StringBuilder();
        var basicEnt = new CrsLedgerBasicEntity();
        var groupEnt = new CrsLedgerOptionGroupEntity();
        var siireEnt = new MSiireSakiEntity();

        // パラメータクリア
        clear();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("   TO_YYYYMMDD_FC(BSC.SYUPT_DAY) AS SYUPT_DAY_STR, ");
        sb.AppendLine("   BSC.CRS_CD, ");
        sb.AppendLine("   BSC.CRS_NAME, ");
        sb.AppendLine("   BSC.GOUSYA, ");
        sb.AppendLine("   TO_HHMM_FC(BSC.SYUPT_TIME_1) AS SYUPT_TIME1, ");
        sb.AppendLine("   CASE BSC.TEIKI_KIKAKU_KBN ");
        sb.AppendLine("      WHEN '1' THEN ");
        sb.AppendLine("        DECODE(BSC.UNKYU_KBN, 'Y', '運休', '廃止') ");
        sb.AppendLine("      WHEN '2' THEN ");
        sb.AppendLine("        DECODE(BSC.UNKYU_KBN, 'Y', '催行決定', 'N', '催行中止', '廃止') ");
        sb.AppendLine("    END AS UNKYU_SAIKOU, ");
        sb.AppendLine("    BSC.BUS_RESERVE_CD, ");
        sb.AppendLine("    BSC.CAR_TYPE_CD, ");
        sb.AppendLine("    BSC.CAPACITY_REGULAR + BSC.CAPACITY_HO_1KAI AS CAPACITY_NUM, ");
        sb.AppendLine("    BSC.JYOSYA_CAPACITY, ");
        sb.AppendLine("    BSC.KUSEKI_NUM_TEISEKI + BSC.KUSEKI_NUM_SUB_SEAT AS KUSEKI_NUM, ");
        sb.AppendLine("    BSC.YOYAKU_NUM_TEISEKI + BSC.YOYAKU_NUM_SUB_SEAT AS YOYAKU_NUM, ");
        sb.AppendLine("    BSC.EI_BLOCK_REGULAR + BSC.EI_BLOCK_HO AS BLOCK_NUM, ");
        sb.AppendLine("    BSC.KUSEKI_KAKUHO_NUM, ");
        sb.AppendLine("    TO_YYYYMMDD_FC(BSC.UKETUKE_START_DAY) AS UKETUKE_START_DAY, ");
        sb.AppendLine("    DECODE(BSC.ZASEKI_RESERVE_KBN, '1', '縦', '横') AS ZASEKI_RESERVE_KBN, ");
        sb.AppendLine("    BSC.UKETUKE_GENTEI_NINZU, ");
        sb.AppendLine("    BSC.BUS_COUNT_FLG, ");
        sb.AppendLine("    DECODE(BSC.SUB_SEAT_OK_KBN, 'Y', '1', '0') AS SUB_SEAT_OK_KBN, ");
        sb.AppendLine("    DECODE(BSC.YOYAKU_STOP_FLG, 'Y', '1', '0') AS YOYAKU_STOP_FLG, ");
        sb.AppendLine("    DECODE(BSC.JYOSEI_SENYO_SEAT_FLG, 'Y', '1', '0') AS JYOSEI_SENYO_SEAT_FLG, ");
        sb.AppendLine("    DECODE(BSC.MEDIA_CHECK_FLG, 'Y', '1', '0') AS MEDIA_CHECK_FLG, ");
        sb.AppendLine("    BSC.CANCEL_RYOU_KBN, ");
        sb.AppendLine("    DECODE(BSC.ONE_SANKA_FLG, 'Y', '1', '0') AS ONE_SANKA_FLG, ");
        sb.AppendLine("    DECODE(BSC.AIBEYA_USE_FLG, 'Y', '1', '0') AS AIBEYA_USE_FLG, ");
        sb.AppendLine("    DECODE(BSC.TEIINSEI_FLG, 'Y', '1', '0') AS TEIINSEI_FLG, ");
        sb.AppendLine("    BSC.CRS_BLOCK_CAPACITY, ");
        sb.AppendLine("    BSC.CRS_BLOCK_ROOM_NUM, ");
        sb.AppendLine("    BSC.CRS_BLOCK_ONE_1R, ");
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_ONE_1R, 'Y', '1', '0') AS UKETOME_KBN_ONE_1R, ");
        sb.AppendLine("    BSC.CRS_BLOCK_TWO_1R, ");
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_TWO_1R, 'Y', '1', '0') AS UKETOME_KBN_TWO_1R, ");
        sb.AppendLine("    BSC.CRS_BLOCK_THREE_1R, ");
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_THREE_1R, 'Y', '1', '0') AS UKETOME_KBN_THREE_1R, ");
        sb.AppendLine("    BSC.CRS_BLOCK_FOUR_1R, ");
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_FOUR_1R, 'Y', '1', '0') AS UKETOME_KBN_FOUR_1R, ");
        sb.AppendLine("    BSC.CRS_BLOCK_FIVE_1R, ");
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_FIVE_1R, 'Y', '1', '0') AS UKETOME_KBN_FIVE_1R, ");
        sb.AppendLine("    CASE ");
        sb.AppendLine("      WHEN BSC.DELETE_DAY <> 0 THEN '1' ");
        sb.AppendLine("      ELSE '0' ");
        sb.AppendLine("    END DELETE_DAY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("    T_CRS_LEDGER_BASIC BSC ");
        sb.AppendLine("    INNER JOIN T_CRS_LEDGER_ADD_INFO ADINFO ");
        sb.AppendLine("      ON BSC.CRS_CD = ADINFO.CRS_CD ");
        sb.AppendLine("      AND BSC.SYUPT_DAY = ADINFO.SYUPT_DAY ");
        sb.AppendLine("      AND BSC.GOUSYA = ADINFO.GOUSYA ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("  1 = 1 ");
        // 出発日FROM
        sb.AppendLine("  AND ");
        sb.AppendLine("  BSC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay));
        // 出発日TO
        sb.AppendLine("  AND ");
        sb.AppendLine("  BSC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay));
        // コースコード
        if (string.IsNullOrEmpty(param.CrsCd) == false)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd));
        }
        // 号車
        if (param.Gousya is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BSC.GOUSYA = ").Append(setSelectParam(param.Gousya, basicEnt.gousya));
        }

        sb.AppendLine(" ORDER BY ");
        sb.AppendLine("   BSC.SYUPT_DAY, ");
        sb.AppendLine("   BSC.CRS_CD, ");
        sb.AppendLine("   BSC.SYUPT_TIME_1, ");
        sb.AppendLine("   BSC.GOUSYA ");
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

    public partial class S03_0224DASelectParam
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
        /// 号車
        /// </summary>
        public int? Gousya { get; set; }
    }
}