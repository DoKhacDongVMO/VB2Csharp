using System.Text;
using Hatobus.ReservationManagementSystem.Master;
/// <summary>
/// 催行決定・中止連絡（仕入先／直販客・代理店）のDAクラス
/// </summary>
public partial class P03_0406DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    /// <summary>
    /// 通知先連絡データ
    /// </summary>
    private enum NoticeDt : int
    {
        // 予約区分
        YOYAKU_KBN,
        // 予約No
        YOYAKU_NO,
        // 業者コード
        AGENT_CD
    }

    #endregion

    #region  SELECT処理 
    /// <summary>
    /// コース台帳（基本）検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableTCrsLedgerBasic(DataTable param)
    {
        var sb = new StringBuilder();
        string YoyakuNo = param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString()).ToString ?? string.Empty;
        string YoyakuKbn = param.Rows(0).Field<string>(NoticeDt.YOYAKU_KBN.ToString()) ?? string.Empty;

        // パラメータクリア
        clear();
        sb.AppendLine("  SELECT  ");
        sb.AppendLine("  TCLB.CRS_NAME AS CRS_NAME, ");
        sb.AppendLine("  MP1.PLACE_NAME_1 AS SYUGO_PLACE1, ");
        sb.AppendLine("  MP2.PLACE_NAME_1 AS SYUGO_PLACE2, ");
        sb.AppendLine("  MP3.PLACE_NAME_1 AS SYUGO_PLACE3, ");
        sb.AppendLine("  MP4.PLACE_NAME_1 AS SYUGO_PLACE4, ");
        sb.AppendLine("  MP5.PLACE_NAME_1 AS SYUGO_PLACE5, ");
        sb.AppendLine("  MPC.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_1) AS SYUGO_TIME1, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_2) AS SYUGO_TIME2, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_3) AS SYUGO_TIME3, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_4) AS SYUGO_TIME4, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_5) AS SYUGO_TIME5, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_1) AS SYUPT_TIME1, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_2) AS SYUPT_TIME2, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_3) AS SYUPT_TIME3, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_4) AS SYUPT_TIME4, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_5) AS SYUPT_TIME5, ");
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER, ");
        sb.AppendLine("  TCLB.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN, ");
        sb.AppendLine("  TYIB.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1, ");
        sb.AppendLine("  TYIB.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2, ");
        sb.AppendLine("  TYIB.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3, ");
        sb.AppendLine("  TYIB.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4, ");
        sb.AppendLine("  TYIB.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5, ");
        sb.AppendLine("  YOYAKU_NINZU.CHARGE_APPLICATION_NINZU AS NINZU");
        sb.AppendLine("  FROM T_YOYAKU_INFO_BASIC TYIB ");
        sb.AppendLine("  INNER JOIN  ");
        sb.AppendLine("  T_CRS_LEDGER_BASIC TCLB ON TYIB.CRS_CD = TCLB.CRS_CD ");
        sb.AppendLine("  AND TCLB.SYUPT_DAY = TYIB.SYUPT_DAY ");
        sb.AppendLine("  AND TCLB.GOUSYA = TYIB.GOUSYA ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MP1 ON TCLB.HAISYA_KEIYU_CD_1 = MP1.PLACE_CD ");
        sb.AppendLine("  AND MP1.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MP2 ON TCLB.HAISYA_KEIYU_CD_2 = MP2.PLACE_CD ");
        sb.AppendLine("  AND MP2.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MP3 ON TCLB.HAISYA_KEIYU_CD_3 = MP3.PLACE_CD ");
        sb.AppendLine("  AND MP3.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MP4 ON TCLB.HAISYA_KEIYU_CD_4 = MP4.PLACE_CD ");
        sb.AppendLine("  AND MP4.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MP5 ON TCLB.HAISYA_KEIYU_CD_5 = MP5.PLACE_CD ");
        sb.AppendLine("  AND MP5.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN  ");
        sb.AppendLine("  M_PLACE MPC ON TCLB.SYUGO_PLACE_CD_CARRIER = MPC.PLACE_CD ");
        sb.AppendLine("  AND MPC.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN (SELECT SUM(CHARGE_APPLICATION_NINZU) AS CHARGE_APPLICATION_NINZU, ");
        sb.AppendLine("  YOYAKU_KBN, ");
        sb.AppendLine("  YOYAKU_NO ");
        sb.AppendLine("  FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
        sb.AppendLine("  WHERE YOYAKU_KBN = '" + YoyakuKbn + "'");
        sb.AppendLine("  AND YOYAKU_NO = '" + YoyakuNo + "'");
        sb.AppendLine("  GROUP BY YOYAKU_KBN,YOYAKU_NO)  YOYAKU_NINZU  ");
        sb.AppendLine("  ON YOYAKU_NINZU.YOYAKU_KBN = TYIB.YOYAKU_KBN ");
        sb.AppendLine("  AND YOYAKU_NINZU.YOYAKU_NO = TYIB.YOYAKU_NO ");
        sb.AppendLine("  WHERE ");
        sb.AppendLine("  TYIB.YOYAKU_KBN = '" + YoyakuKbn + "'");
        sb.AppendLine("  AND TYIB.YOYAKU_NO = '" + YoyakuNo + "'");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 予約情報検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableTYoyakuInfoBasic(DataTable param)
    {
        var sb = new StringBuilder();
        string YoyakuNo = param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString()).ToString ?? string.Empty;
        string YoyakuKbn = param.Rows(0).Field<string>(NoticeDt.YOYAKU_KBN.ToString()) ?? string.Empty;

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT  ");
        sb.AppendLine("YUBIN_NO, ");
        sb.AppendLine("ADDRESS_1, ");
        sb.AppendLine("ADDRESS_2, ");
        sb.AppendLine("ADDRESS_3 ");
        sb.AppendLine("FROM T_YOYAKU_INFO_BASIC ");
        sb.AppendLine("WHERE YOYAKU_KBN = '" + YoyakuKbn + "'");
        sb.AppendLine("AND YOYAKU_NO = '" + YoyakuNo + "'");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 代理店マスタ検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTableAgent(DataTable param)
    {
        var sb = new StringBuilder();
        string AgentCd = param.Rows(0).Field<string>(NoticeDt.AGENT_CD.ToString()) ?? string.Empty;

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT YUBIN_NO,  ");
        sb.AppendLine("FAX_1 || '-' || FAX_2 || '-' || FAX_3 AS FAXSTR ");
        sb.AppendLine("FROM M_AGENT ");
        sb.AppendLine("WHERE AGENT_CD = '" + AgentCd + "'");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 代理店マスタ検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectUsedayStr(DataTable param)
    {
        var sb = new StringBuilder();
        string YoyakuNo = param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString()).ToString ?? string.Empty;
        string YoyakuKbn = param.Rows(0).Field<string>(NoticeDt.YOYAKU_KBN.ToString()) ?? string.Empty;

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT TCL.USEDAY_STR AS USEDAY_STR ");
        sb.AppendLine("FROM T_YOYAKU_INFO_BASIC TYIB ");
        sb.AppendLine("LEFT JOIN ");
        sb.AppendLine("  (SELECT TO_YYYYMMDD_FC(RIYOU_DAY) AS USEDAY_STR, ");
        sb.AppendLine("    CRS_CD, ");
        sb.AppendLine("    SYUPT_DAY, ");
        sb.AppendLine("    GOUSYA ");
        sb.AppendLine("  FROM T_CRS_LEDGER_HOTEL ");
        sb.AppendLine("  UNION ");
        sb.AppendLine("  SELECT TO_YYYYMMDD_FC(RIYOU_DAY) AS USEDAY_STR, ");
        sb.AppendLine("    CRS_CD, ");
        sb.AppendLine("    SYUPT_DAY, ");
        sb.AppendLine("    GOUSYA ");
        sb.AppendLine("  FROM T_CRS_LEDGER_KOSHAKASHO ");
        sb.AppendLine("  ) TCL ");
        sb.AppendLine("ON TCL.CRS_CD         = TYIB.CRS_CD ");
        sb.AppendLine("AND TCL.SYUPT_DAY     = TYIB.SYUPT_DAY ");
        sb.AppendLine("AND TCL.GOUSYA        = TYIB.GOUSYA ");
        sb.AppendLine("WHERE TYIB.YOYAKU_KBN = '" + YoyakuKbn + "'");
        sb.AppendLine("AND TYIB.YOYAKU_NO    = '" + YoyakuNo + "'");
        sb.AppendLine("GROUP BY USEDAY_STR");
        sb.AppendLine("ORDER BY USEDAY_STR");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 通知内容取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public DataTable selectMCode(P03_0406DAGetMCodeParam param)
    {
        var code = new MCodeEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine(" SELECT ");
        sb.AppendLine(" CODE_VALUE ");
        sb.AppendLine("   , NAIYO_1");
        // FROM句
        sb.AppendLine(" FROM ");
        sb.AppendLine("   M_CODE ");
        // WHERE句
        sb.AppendLine("WHERE ");
        // コード分類
        sb.AppendLine("CODE_BUNRUI = ").Append(setSelectParam(param.CodeBunrui, code.CodeBunrui));
        return base.getDataTable(sb.ToString());
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

    private void clear()
    {
        base.paramClear();
        ParamNum = 0;
    }

    #endregion
    #region  パラメータ 
    public partial class P03_0406DAGetMCodeParam
    {
        /// <summary>
        /// コード分類
        /// </summary>
        public string CodeBunrui { get; set; }
    }
    #endregion
}