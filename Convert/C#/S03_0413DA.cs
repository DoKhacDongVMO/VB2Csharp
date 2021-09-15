using System.Text;

/// <summary>
/// 運休連絡のDAクラス
/// </summary>
public partial class S03_0413DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTable(S03_0413DASelectParam param)
    {
        var crsLedgerBasic = new CrsLedgerBasicEntity();
        var yoyakuInfoBasic = new YoyakuInfoBasicEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // SELECT句
        sb.AppendLine("SELECT  ");
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ");                                    // 出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY AS SYUPT_DAY ");                                                      // 出発日
        sb.AppendLine("    , BASIC.CRS_CD AS CRS_CD ");                                                            // コースコード
        sb.AppendLine("    , BASIC.CRS_NAME AS CRS_NAME ");                                                        // コース名
        sb.AppendLine("    , BASIC.GOUSYA AS GOUSYA ");                                                            // 号車
        sb.AppendLine("    , CASE WHEN BASIC.TEIKI_KIKAKU_KBN = '1' THEN TO_YYYYMMDD_FC(BASIC.UNKYU_DECIDED_DAY) "); // 運休決定日
        sb.AppendLine("      WHEN BASIC.TEIKI_KIKAKU_KBN = '2' THEN TO_YYYYMMDD_FC(BASIC.SAIKOU_DAY) ");             // 催行日
        sb.AppendLine("      END AS UNKYU_DECIDE_DAY ");
        sb.AppendLine("    , INFO.YOYAKU_KBN AS YOYAKU_KBN ");                                                     // 予約区分
        sb.AppendLine("    , INFO.YOYAKU_NO AS YOYAKU_NO ");                                                       // 予約Ｎｏ
        sb.AppendLine("    , CONCAT(INFO.YOYAKU_KBN, INFO.YOYAKU_NO) AS YOYAKU_NO_STR ");                          // 予約Ｎｏ（表示用）
        sb.AppendLine("    , CONCAT(INFO.SURNAME, INFO.NAME) AS YOYAKU_NAME ");                                    // 予約者名
        sb.AppendLine("    , TO_CHAR(CHARGE.YOYAKU_NUM, '999,999,999,999') AS YOYAKU_NUM ");                       // 予約人数
        sb.AppendLine("    , TO_YYYYMMDD_FC(INFO.UNKYU_CONTACT_DAY) AS  UNKYU_CONTACT_DAY");                       // 運休連絡日

        // FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC ");
        sb.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC INFO ");
        sb.AppendLine("    ON BASIC.SYUPT_DAY = INFO.SYUPT_DAY ");
        sb.AppendLine("    AND BASIC.CRS_CD = INFO.CRS_CD ");
        sb.AppendLine("    AND BASIC.GOUSYA = INFO.GOUSYA ");
        sb.AppendLine("INNER JOIN ");
        sb.AppendLine("    (SELECT  ");
        sb.AppendLine("        YOYAKU_KBN ");
        sb.AppendLine("        , YOYAKU_NO ");
        sb.AppendLine("        , SUM(CHARGE_APPLICATION_NINZU) AS YOYAKU_NUM ");
        sb.AppendLine("     FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
        sb.AppendLine("     GROUP BY YOYAKU_KBN, YOYAKU_NO");
        sb.AppendLine("    ) CHARGE ");
        sb.AppendLine("    ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ");
        sb.AppendLine("    AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO  ");
        sb.AppendLine("WHERE ");
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0') <> 'M' ");
        sb.AppendLine("    AND (BASIC.UNKYU_KBN = 'Y' OR BASIC.SAIKOU_KAKUTEI_KBN = 'N') ");
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0 ");
        sb.AppendLine("    AND (BASIC.YOYAKU_NUM_TEISEKI <> 0 OR BASIC.YOYAKU_NUM_SUB_SEAT <> 0) ");
        sb.AppendLine("    AND INFO.YOYAKU_KBN >= '0' AND INFO.YOYAKU_KBN <= '9' ");
        sb.AppendLine("    AND INFO.CANCEL_FLG IS NULL ");
        sb.AppendLine("    AND NVL(INFO.DELETE_DAY, 0) = 0 ");
        // 出発日FROM
        if (param.SyuptDayFrom is object)
        {
            sb.Append("    AND BASIC.SYUPT_DAY >= ").AppendLine(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay));
        }
        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.Append("    AND BASIC.SYUPT_DAY <= ").AppendLine(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay));
        }
        // コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            sb.Append("    AND BASIC.CRS_CD = ").AppendLine(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd));
        }
        // コースコード
        if (param.Gousya is object)
        {
            sb.Append("    AND BASIC.GOUSYA = ").AppendLine(setSelectParam(param.Gousya, crsLedgerBasic.gousya));
        }
        // 邦人／外客区分
        if (param.CrsJapanese == true || param.CrsForeign == true)
        {
            sb.AppendLine("    AND (1 <> 1 ");
            if (param.CrsJapanese == true)
            {
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = ").AppendLine(setSelectParam("H", crsLedgerBasic.houjinGaikyakuKbn));
            }

            if (param.CrsForeign == true)
            {
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = ").AppendLine(setSelectParam("G", crsLedgerBasic.houjinGaikyakuKbn));
            }

            sb.AppendLine("        )");
        }
        // 定期
        if (param.CrsTeiki == true)
        {
            sb.Append("    AND BASIC.TEIKI_KIKAKU_KBN = ").AppendLine(setSelectParam("1", crsLedgerBasic.teikiKikakuKbn));
            if (param.CrsKbnHiru == true || param.CrsKbnYoru == true || param.CrsKbnKougai == true)
            {
                sb.AppendLine("    AND (1 <> 1 ");
                // 定期（昼）
                if (param.CrsKbnHiru == true)
                {
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind));
                    sb.Append("             AND BASIC.CRS_KBN_1 = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKbn1));
                    sb.AppendLine("             ) ");
                }
                // 定期（夜）
                if (param.CrsKbnYoru == true)
                {
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind));
                    sb.Append("             AND BASIC.CRS_KBN_1 = ").AppendLine(setSelectParam("2", crsLedgerBasic.crsKbn1));
                    sb.AppendLine("             ) ");
                }
                // 定期（郊外）
                if (param.CrsKbnKougai == true)
                {
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind));
                    sb.Append("             AND BASIC.CRS_KBN_2 = ").AppendLine(setSelectParam("2", crsLedgerBasic.crsKbn2));
                    sb.AppendLine("             ) ");
                }

                sb.AppendLine("        )");
            }
        }
        // 企画
        if (param.CrsKikaku == true)
        {
            sb.Append("    AND BASIC.TEIKI_KIKAKU_KBN = ").AppendLine(setSelectParam("2", crsLedgerBasic.teikiKikakuKbn));
            if (param.CrsKindDay == true || param.CrsKindStay == true || param.CrsKindR == true)
            {
                sb.AppendLine("    AND (1 <> 1 ");
                // 企画（日帰り）
                if (param.CrsKindDay == true)
                {
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("4", crsLedgerBasic.crsKind));
                }
                // 企画（宿泊）
                if (param.CrsKindStay == true)
                {
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("5", crsLedgerBasic.crsKind));
                }
                // 企画（Ｒコース）
                if (param.CrsKindR == true)
                {
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("6", crsLedgerBasic.crsKind));
                }

                sb.AppendLine("        )");
            }
        }
        // 連絡済含む
        if (param.ContactIncluding == false)
        {
            sb.Append("    AND INFO.UNKYU_CONTACT_DAY = ").AppendLine(setSelectParam("0", yoyakuInfoBasic.unkyuContactDay));
        }
        // ORDER句
        sb.AppendLine(" ORDER BY BASIC.SYUPT_DAY");
        sb.AppendLine("         , BASIC.CRS_CD");
        sb.AppendLine("         , BASIC.GOUSYA");
        return base.getDataTable(sb.ToString());
    }

    private void clear()
    {
        base.paramClear();
        ParamNum = 0;
    }

    public string setSelectParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, true);
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
    #endregion

    #region  パラメータ 
    public partial class S03_0413DASelectParam
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
        /// <summary>
        /// 日本語
        /// </summary>
        public bool CrsJapanese { get; set; }
        /// <summary>
        /// 外国語
        /// </summary>
        public bool CrsForeign { get; set; }
        /// <summary>
        /// 定期
        /// </summary>
        public bool CrsTeiki { get; set; }
        /// <summary>
        /// 定期（昼）
        /// </summary>
        public bool CrsKbnHiru { get; set; }
        /// <summary>
        /// 定期（夜）
        /// </summary>
        public bool CrsKbnYoru { get; set; }
        /// <summary>
        /// 定期（郊外）
        /// </summary>
        public bool CrsKbnKougai { get; set; }
        /// <summary>
        /// 企画
        /// </summary>
        public bool CrsKikaku { get; set; }
        /// <summary>
        /// 企画（日帰り）
        /// </summary>
        public bool CrsKindDay { get; set; }
        /// <summary>
        /// 企画（宿泊）
        /// </summary>
        public bool CrsKindStay { get; set; }
        /// <summary>
        /// 企画（Ｒコース）
        /// </summary>
        public bool CrsKindR { get; set; }
        /// <summary>
        /// 連絡済含む
        /// </summary>
        public bool ContactIncluding { get; set; }
    }
    #endregion
}