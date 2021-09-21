using System;
using System.Reflection;
using System.Text;

/// <summary>
/// 窓口日報修正Da
/// </summary>
public partial class S05_0505Da : DataAccessorBase
{

    #region メソッド

    /// <summary>
    /// 予約情報取得
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>予約情報</returns>
    public DataTable getYoyakuInfo(SeisanInfoEntity entity)
    {
        DataTable yoyakuInfo;
        try
        {
            string query = createYoyakuInfoSql(entity);
            yoyakuInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuInfo;
    }

    /// <summary>
    /// 予約情報取得SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>予約情報取得SQL</returns>
    private string createYoyakuInfoSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        string yoyakuKbn = this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn);
        string yoyakuNo = this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo);
        string seq = this.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq);
        var sb = new StringBuilder();
        sb.AppendLine($" SELECT ");
        sb.AppendLine($"      YIB.SURNAME || YIB.NAME AS YOYAKU_NANME ");
        sb.AppendLine($"      ,YIB.YOYAKU_KBN ");
        sb.AppendLine($"     ,YIB.YOYAKU_NO ");
        sb.AppendLine($"     ,YIB.TEIKI_KIKAKU_KBN ");
        sb.AppendLine($"     ,YIB.CRS_KIND ");
        sb.AppendLine($"     ,YIB.SYUPT_DAY ");
        sb.AppendLine($"     ,YIB.GOUSYA ");
        sb.AppendLine($"     ,YIB.CRS_CD ");
        sb.AppendLine($"     ,CLB.CRS_NAME ");
        sb.AppendLine($"     ,SIS.ADULT ");
        sb.AppendLine($"     ,SIS.JUNIOR ");
        sb.AppendLine($"     ,SIS.CHILD ");
        sb.AppendLine($"     ,YIB.UPDATE_TIME ");
        sb.AppendLine($"     ,TSI.URIAGE_KBN ");
        sb.AppendLine($"     ,CASE TSI.URIAGE_KBN WHEN 'H' THEN '【払戻】' ");
        sb.AppendLine($"                          WHEN 'V' THEN '【ＶＯＩＤ】' ");
        sb.AppendLine($"      ELSE '【売上】' END AS URIAGE_KBN_NAME ");
        sb.AppendLine($"     ,TSI.WARIBIKI_TYPE ");
        sb.AppendLine($"     ,MWT.WARIBIKI_TYPE_NAME_ABB ");
        sb.AppendLine($"     ,TSI.OTHER_URIAGE_SYOHIN_BIKO ");
        sb.AppendLine($"     ,TSI.TANTOSYA_CD ");
        sb.AppendLine($"     ,MUS.USER_NAME ");
        sb.AppendLine($" FROM ");
        sb.AppendLine($"     T_SEISAN_INFO TSI ");
        sb.AppendLine($" INNER JOIN ");
        sb.AppendLine($"     T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine($" ON ");
        sb.AppendLine($"     TSI.YOYAKU_KBN = YIB.YOYAKU_KBN ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     TSI.YOYAKU_NO = YIB.YOYAKU_NO ");
        sb.AppendLine($" LEFT OUTER JOIN ");
        sb.AppendLine($"     T_CRS_LEDGER_BASIC CLB ");
        sb.AppendLine($" ON ");
        sb.AppendLine($"     YIB.CRS_CD = CLB.CRS_CD ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     YIB.SYUPT_DAY = CLB.SYUPT_DAY ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     YIB.GOUSYA = CLB.GOUSYA ");
        sb.AppendLine($" LEFT JOIN ");
        sb.AppendLine($"     (SELECT ");
        sb.AppendLine($"          SIS.SEQ ");
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '10' THEN SIS.SANKA_NINZU ELSE 0 END) AS ADULT ");
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '20' THEN SIS.SANKA_NINZU ELSE 0 END) AS JUNIOR ");
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '30' THEN SIS.SANKA_NINZU ELSE 0 END) AS CHILD ");
        sb.AppendLine($"     FROM ");
        sb.AppendLine($"         T_SEISAN_INFO_SANKA_NINZU SIS ");
        sb.AppendLine($"     INNER JOIN ");
        sb.AppendLine($"         M_CHARGE_JININ_KBN CJK ");
        sb.AppendLine($"     ON ");
        sb.AppendLine($"         SIS.CHARGE_KBN_JININ_CD = CJK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine($"     WHERE ");
        sb.AppendLine($"         SIS.YOYAKU_KBN = {yoyakuKbn}");
        sb.AppendLine($"         AND ");
        sb.AppendLine($"         SIS.YOYAKU_NO = {yoyakuNo}");
        sb.AppendLine($"         AND ");
        sb.AppendLine($"         SIS.SEQ = {seq}");
        sb.AppendLine($"     GROUP BY ");
        sb.AppendLine($"          SIS.YOYAKU_KBN ");
        sb.AppendLine($"         ,SIS.YOYAKU_NO ");
        sb.AppendLine($"         ,SIS.SEQ) SIS ");
        sb.AppendLine($" ON ");
        sb.AppendLine($"     TSI.SEQ = SIS.SEQ ");
        sb.AppendLine($" LEFT OUTER JOIN ");
        sb.AppendLine($"     M_WARIBIKI_TYPE MWT ");
        sb.AppendLine($" ON ");
        sb.AppendLine($"     TSI.WARIBIKI_TYPE = MWT.WARIBIKI_TYPE ");
        sb.AppendLine($" LEFT OUTER JOIN ");
        sb.AppendLine($"     M_USER MUS ");
        sb.AppendLine($" ON ");
        sb.AppendLine($"     MUS.COMPANY_CD = '0001' ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     TSI.TANTOSYA_CD = MUS.USER_ID ");
        sb.AppendLine($" WHERE ");
        sb.AppendLine($"     TSI.YOYAKU_KBN = {yoyakuKbn} ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     TSI.YOYAKU_NO = {yoyakuNo} ");
        sb.AppendLine($"     AND ");
        sb.AppendLine($"     TSI.SEQ = {seq} ");
        return sb.ToString();
    }

    /// <summary>
    /// 割引一覧取得
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>割引一覧</returns>
    public DataTable getWaribikiList(SeisanInfoEntity entity)
    {
        DataTable waribikiList;
        try
        {
            string query = createWaribikiListSql(entity);
            waribikiList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return waribikiList;
    }

    /// <summary>
    /// 割引一覧取得SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>割引一覧取得SQL</returns>
    private string createWaribikiListSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        string yoyakuKbn = this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn);
        string yoyakuNo = this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo);
        string seq = this.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq);
        string syuptDay = this.prepareParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay);
        var sb = new StringBuilder();
        // sb.AppendLine($" SELECT ")
        // sb.AppendLine($"      YIW.KBN_NO ")
        // sb.AppendLine($"     ,YIC.CHARGE_KBN ")
        // sb.AppendLine($"     ,MCK.CHARGE_NAME ")
        // sb.AppendLine($"     ,YIW.CHARGE_KBN_JININ_CD ")
        // sb.AppendLine($"     ,MCJ.CHARGE_KBN_JININ_NAME ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_CD ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_RIYUU ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_USE_FLG ")
        // sb.AppendLine($"     ,CASE YIW.WARIBIKI_KBN WHEN '1' THEN '%' ")
        // sb.AppendLine($"                        WHEN '2' THEN '\' ")
        // sb.AppendLine($"      END AS WARIBIKI_KBN  ")
        // sb.AppendLine($"     ,DECODE(YIW.WARIBIKI_KBN, '1', YIW.WARIBIKI_PER, YIW.WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ")
        // sb.AppendLine($"     ,CASE WHEN NVL(YIW.CARRIAGE_WARIBIKI_FLG, '0') = '1' OR NVL(YIW.YOYAKU_WARIBIKI_FLG, '0') = '1' THEN '予約時' ")
        // sb.AppendLine($"      ELSE '当日' END AS KBN ")
        // sb.AppendLine($"     ,(YIW.WARIBIKI_APPLICATION_NINZU_1 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_2 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_3 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_4 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_5) AS WARIBIKI_NINZU ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_1 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_2 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_3 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_4 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_5 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU  ")
        // sb.AppendLine($"     ,(YIW.WARIBIKI_TANKA_1 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_TANKA_2 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_TANKA_3 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_TANKA_4 + ")
        // sb.AppendLine($"      YIW.WARIBIKI_TANKA_5) AS WARIBIKI_TOTAL_GAKU ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_1 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_2 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_3 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_4 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_5 ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_BIKO ")
        // sb.AppendLine($"     ,MWC.CHANGE_KANOU_FLG ")
        // sb.AppendLine($"     ,YIW.CARRIAGE_WARIBIKI_FLG ")
        // sb.AppendLine($"     ,YIW.YOYAKU_WARIBIKI_FLG ")
        // sb.AppendLine($"     ,YIW.WARIBIKI_USE_FLG ")
        // sb.AppendLine($" FROM ")
        // sb.AppendLine($"     T_SEISAN_INFO TSI ")
        // sb.AppendLine($" INNER JOIN ")
        // sb.AppendLine($"     T_SEISAN_INFO_SANKA_NINZU SIS ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     TSI.SEISAN_INFO_SEQ = SIS.SEISAN_INFO_SEQ ")
        // sb.AppendLine($" INNER JOIN ")
        // sb.AppendLine($"     T_YOYAKU_INFO_WARIBIKI YIW ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     TSI.YOYAKU_KBN = YIW.YOYAKU_KBN ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     TSI.YOYAKU_NO = YIW.YOYAKU_NO ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     SIS.KBN_NO = YIW.KBN_NO ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     SIS.CHARGE_KBN_JININ_CD = YIW.CHARGE_KBN_JININ_CD ")
        // sb.AppendLine($" INNER JOIN ")
        // sb.AppendLine($"     M_WARIBIKI_CD MWC ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     YIW.WARIBIKI_CD = MWC.WARIBIKI_CD ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     TSI.CRS_KIND = MWC.CRS_KIND ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     {syuptDay} BETWEEN MWC.APPLICATION_DAY_FROM AND MWC.APPLICATION_DAY_TO ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     TSI.WARIBIKI_TYPE = MWC.WARIBIKI_TYPE_KBN ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     NVL(MWC.DELETE_DAY, 0) = 0 ")
        // sb.AppendLine($" LEFT OUTER JOIN ")
        // sb.AppendLine($"     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YIC ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     YIW.YOYAKU_KBN = YIC.YOYAKU_KBN ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     YIW.YOYAKU_NO = YIC.YOYAKU_NO ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     YIW.KBN_NO = YIC.KBN_NO ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     YIW.CHARGE_KBN_JININ_CD = YIC.CHARGE_KBN_JININ_CD ")
        // sb.AppendLine($" LEFT OUTER JOIN ")
        // sb.AppendLine($"     M_CHARGE_KBN MCK ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     YIC.CHARGE_KBN = MCK.CHARGE_KBN ")
        // sb.AppendLine($" LEFT OUTER JOIN ")
        // sb.AppendLine($"     M_CHARGE_JININ_KBN MCJ ")
        // sb.AppendLine($" ON ")
        // sb.AppendLine($"     YIW.CHARGE_KBN_JININ_CD = MCJ.CHARGE_KBN_JININ_CD ")
        // sb.AppendLine($" WHERE ")
        // sb.AppendLine($"     TSI.YOYAKU_KBN = {yoyakuKbn} ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     TSI.YOYAKU_NO = {yoyakuNo} ")
        // sb.AppendLine($"     AND ")
        // sb.AppendLine($"     TSI.SEQ = {seq} ")
        // sb.AppendLine($" ORDER BY ")
        // sb.AppendLine($"      YIW.WARIBIKI_CD ")
        // sb.AppendLine($"     ,YIW.KBN_NO ")
        // sb.AppendLine($"     ,YIW.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" SELECT");
        sb.AppendLine($"      YW.KBN_NO ");
        sb.AppendLine($"     ,YCC.CHARGE_KBN ");
        sb.AppendLine($"     ,MC.CHARGE_NAME ");
        sb.AppendLine($"     ,YW.CHARGE_KBN_JININ_CD ");
        sb.AppendLine($"     ,MCJ.CHARGE_KBN_JININ_NAME ");
        sb.AppendLine($"     ,YW.WARIBIKI_CD ");
        sb.AppendLine($"     ,YW.WARIBIKI_RIYUU ");
        sb.AppendLine($"     ,YW.WARIBIKI_USE_FLG ");
        sb.AppendLine($"     ,CASE YW.WARIBIKI_KBN WHEN '1' THEN '%' ");
        sb.AppendLine($@"                        WHEN '2' THEN '\' ");
        sb.AppendLine($"      END AS WARIBIKI_KBN  ");
        sb.AppendLine($"     ,DECODE(YW.WARIBIKI_KBN, '1', YW.WARIBIKI_PER, YW.WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ");
        sb.AppendLine($"     ,CASE WHEN NVL(YW.CARRIAGE_WARIBIKI_FLG, '0') = '1' OR NVL(YW.YOYAKU_WARIBIKI_FLG, '0') = '1' THEN '予約時' ");
        sb.AppendLine($"      ELSE '当日' END AS KBN ");
        sb.AppendLine($"     ,(NVL(YW.WARIBIKI_APPLICATION_NINZU_1,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_2,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_3,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_4,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_5,0)) AS WARIBIKI_NINZU ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_1,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_2,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_3,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_4,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_5,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU,0)  ");
        sb.AppendLine($"     ,(NVL(YW.WARIBIKI_TANKA_1,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_2,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_3,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_4,0) + ");
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_5,0)) AS WARIBIKI_TOTAL_GAKU ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_1,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_2,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_3,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_4,0) ");
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_5,0) ");
        sb.AppendLine($"     ,YW.WARIBIKI_BIKO ");
        sb.AppendLine($"     ,MWC.CHANGE_KANOU_FLG ");
        sb.AppendLine($"     ,YW.CARRIAGE_WARIBIKI_FLG ");
        sb.AppendLine($"     ,YW.YOYAKU_WARIBIKI_FLG ");
        sb.AppendLine($"     ,YW.WARIBIKI_USE_FLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_WARIBIKI YW ");
        sb.AppendLine("     INNER JOIN T_YOYAKU_INFO_BASIC TYIB ");
        sb.AppendLine("         ON TYIB.YOYAKU_KBN = YW.YOYAKU_KBN ");
        sb.AppendLine("        AND TYIB.YOYAKU_NO = YW.YOYAKU_NO ");
        sb.AppendLine("     INNER JOIN T_CRS_LEDGER_BASIC TCLB ");
        sb.AppendLine("         ON TCLB.CRS_CD = TYIB.CRS_CD ");
        sb.AppendLine("        AND TCLB.SYUPT_DAY = TYIB.SYUPT_DAY ");
        sb.AppendLine("        AND TCLB.GOUSYA = TYIB.GOUSYA ");
        sb.AppendLine("     LEFT OUTER JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YCC ");
        sb.AppendLine("         ON YW.YOYAKU_KBN = YCC.YOYAKU_KBN ");
        sb.AppendLine("        AND YW.YOYAKU_NO = YCC.YOYAKU_NO ");
        sb.AppendLine("        AND YW.KBN_NO       = YCC.KBN_NO ");
        sb.AppendLine("        AND YW.CHARGE_KBN_JININ_CD = YCC.CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     LEFT OUTER JOIN M_CHARGE_KBN MC ");
        sb.AppendLine("         ON YCC.CHARGE_KBN = MC.CHARGE_KBN ");
        sb.AppendLine("     LEFT OUTER JOIN M_CHARGE_JININ_KBN MCJ ");
        sb.AppendLine("         ON YW.CHARGE_KBN_JININ_CD = MCJ.CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     LEFT OUTER JOIN M_WARIBIKI_CD MWC ");
        sb.AppendLine("         ON YW.WARIBIKI_CD = MWC.WARIBIKI_CD ");
        sb.AppendLine("        AND MWC.CRS_KIND  = TCLB.CRS_KIND ");
        sb.AppendLine("        AND MWC.HOUJIN_GAIKYAKU_KBN = TCLB.HOUJIN_GAIKYAKU_KBN ");
        sb.AppendLine(" WHERE 1 = 1 ");
        sb.AppendLine($"     AND YW.YOYAKU_KBN = {yoyakuKbn} ");
        sb.AppendLine($"     AND YW.YOYAKU_NO  = {yoyakuNo} ");
        sb.AppendLine($"     AND RTRIM(MC.DELETE_DATE) IS NULL "); // CHAR型
        sb.AppendLine($"     AND RTRIM(MCJ.DELETE_DAY) IS NULL "); // CHAR型
        sb.AppendLine($"     AND NVL(MWC.DELETE_DAY, 0) = 0 "); // NUMBER型
        return sb.ToString();
    }

    /// <summary>
    /// クーポン払戻売上取得
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>クーポン払戻売上</returns>
    public DataTable getCouponInfo(SeisanInfoEntity entity)
    {
        DataTable couponInfo;
        try
        {
            string query = createCouponInfoSql(entity);
            couponInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return couponInfo;
    }

    /// <summary>
    /// クーポン払戻売上取得SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>クーポン払戻売上取得SQL</returns>
    private string createCouponInfoSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        string yoyakuKbn = this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn);
        string yoyakuNo = this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo);
        string seq = this.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq);
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      SEISAN_INFO_SEQ ");
        sb.AppendLine("     ,COUPON_REFUND ");
        sb.AppendLine("     ,COUPON_URIAGE ");
        sb.AppendLine("     ,OTHER_URIAGE_SYOHIN_BIKO ");
        sb.AppendLine("     ,URIAGE_KBN ");
        sb.AppendLine("     ,SEISAN_KBN ");
        sb.AppendLine("     ,KENNO ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_SEISAN_INFO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine($"     YOYAKU_KBN = {yoyakuKbn} ");
        sb.AppendLine("     AND ");
        sb.AppendLine($"     YOYAKU_NO = {yoyakuNo} ");
        sb.AppendLine("     AND ");
        sb.AppendLine($"     SEQ = {seq} ");
        return sb.ToString();
    }

    /// <summary>
    /// 精算項目内訳取得
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>精算項目内訳</returns>
    public DataTable getSeisanInfoUtiwake(SeisanInfoEntity entity)
    {
        DataTable seisanInfoUtiwake;
        try
        {
            string query = createSeisanInfoUtiwakeSql(entity);
            seisanInfoUtiwake = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return seisanInfoUtiwake;
    }

    /// <summary>
    /// 精算項目内訳取得SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>精算項目内訳取得SQL</returns>
    private string createSeisanInfoUtiwakeSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        string seisanInfoSeq = this.prepareParam(entity.seisanInfoSeq.PhysicsName, entity.seisanInfoSeq.Value, entity.seisanInfoSeq);
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      MSK.SEISAN_KOUMOKU_CD ");
        sb.AppendLine("     ,MSK.SEISAN_KOUMOKU_NAME ");
        sb.AppendLine("     ,SIU.BIKO ");
        sb.AppendLine("     ,SIU.ISSUE_COMPANY_CD ");
        sb.AppendLine("     ,SIU.KINGAKU ");
        sb.AppendLine("     ,SIU.HURIKOMI_KBN ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_SEISAN_KOUMOKU MSK ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     (SELECT ");
        sb.AppendLine("         * ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("         T_SEISAN_INFO_UTIWAKE SIU ");
        sb.AppendLine("     WHERE ");
        sb.AppendLine($"         SIU.SEISAN_INFO_SEQ = {seisanInfoSeq} ");
        sb.AppendLine("     ) SIU ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MSK.SEISAN_KOUMOKU_CD = SIU.SEISAN_KOUMOKU_CD ");
        return sb.ToString();
    }

    /// <summary>
    /// 補助券発行会社取得
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>補助券発行会社一覧</returns>
    public DataTable getSubKenIssueCompanyList(SeisanInfoEntity entity)
    {
        DataTable subKenIssueCompanyList;
        try
        {
            string query = createSubKenIssueCompanyListSql(entity);
            subKenIssueCompanyList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return subKenIssueCompanyList;
    }

    /// <summary>
    /// 補助券発行会社取得SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>補助券発行会社取得SQL</returns>
    private string createSubKenIssueCompanyListSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        string syuptDay = this.prepareParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay);
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      ISSUE_COMPANY_CD ");
        sb.AppendLine("     ,ISSUE_COMPANY_NAME ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_SUB_KEN_ISSUE_COMPANY ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     NVL(DELETE_DAY, 0) = 0 ");
        sb.AppendLine("     AND ");
        sb.AppendLine($"     TO_DATE(TO_CHAR({syuptDay}), 'yyyyMMdd') BETWEEN NVL(KEIYAKU_START_DATE, TO_DATE('0001/01/01')) AND NVL(KEIYAKU_END_DATE, TO_DATE('9999/12/31')) ");
        sb.AppendLine(" ORDER BY ");
        sb.AppendLine("     ISSUE_COMPANY_CD ");
        return sb.ToString();
    }

    /// <summary>
    /// 精算情報登録
    /// </summary>
    /// <param name="registEntity">精算情報登録用Entity</param>
    /// <returns>登録結果</returns>
    public string registSeisanInfo(RegistSeisanInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 精算情報更新
            string seisanInfoSql = this.createSeisanInfoUpdateSql(registEntity.SeisanInfoEntity);
            updateCount = base.execNonQuery(oracleTransaction, seisanInfoSql);
            if (updateCount <= 0)
            {
                return S05_0505.RegistStatusSeisanInfoFailure;
            }

            // 精算情報内訳削除
            string seisanInfoUtiwakeDeleteSql = this.createSeisanInfoUtiwakeDeleteSql(registEntity.SeisanInfoUtiwakeEntity);
            base.execNonQuery(oracleTransaction, seisanInfoUtiwakeDeleteSql);

            // 精算情報内訳追加
            foreach (SeisanInfoUtiwakeEntity entity in registEntity.SeisanInfoUtiwakeList)
            {
                string utiwakeInsertSql = createInsertSql<SeisanInfoUtiwakeEntity>(entity, "T_SEISAN_INFO_UTIWAKE");
                updateCount = base.execNonQuery(oracleTransaction, utiwakeInsertSql);
                if (updateCount <= 0)
                {
                    return S05_0505.RegistStatusSeisanInfoUtiwakeFailure;
                }
            }

            // コミット
            base.callCommitTransaction(oracleTransaction);
        }
        catch (Exception ex)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            throw;
        }
        finally
        {
            // トランザクションの破棄
            oracleTransaction.Dispose();
        }

        return S05_0505.RegistStatusSucess;
    }

    /// <summary>
    /// 精算情報更新SQL作成
    /// </summary>
    /// <param name="entity">精算情報Entity</param>
    /// <returns>精算情報更新SQL</returns>
    private string createSeisanInfoUpdateSql(SeisanInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_SEISAN_INFO ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      NOSIGN_KBN = " + this.prepareParam(entity.nosignKbn.PhysicsName, entity.nosignKbn.Value, entity.nosignKbn));
        sb.AppendLine("     ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
        sb.AppendLine("     ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
        sb.AppendLine("     ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
        sb.AppendLine("     ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SEQ = " + this.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq));
        return sb.ToString();
    }

    /// <summary>
    /// 精算情報内訳削除SQL作成
    /// </summary>
    /// <param name="entity">精算情報内訳Entity</param>
    /// <returns>精算情報内訳削除SQL</returns>
    private string createSeisanInfoUtiwakeDeleteSql(SeisanInfoUtiwakeEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" DELETE FROM T_SEISAN_INFO_UTIWAKE ");
        sb.AppendLine(" WHERE SEISAN_INFO_SEQ = " + this.prepareParam(entity.seisanInfoSeq.PhysicsName, entity.seisanInfoSeq.Value, entity.seisanInfoSeq));
        return sb.ToString();
    }

    /// <summary>
    /// INSERT SQL作成
    /// </summary>
    /// <param name="entity">登録するテーブルEntity</param>
    /// <param name="tableName">テーブル名</param>
    /// <returns>INSERT SQL</returns>
    private string createInsertSql<T>(T entity, string tableName)
    {
        base.paramClear();
        var type = typeof(T);
        var properties = type.GetProperties();
        var insertSql = new StringBuilder();
        var valueSql = new StringBuilder();
        int idx = 0;
        string comma = "";
        string physicsName = "";
        foreach (PropertyInfo prop in properties)
        {
            if (idx == 0)
            {
                comma = "";
            }

            if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_YmdType)))
            {
                physicsName = ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).PhysicsName;
                DateTime? value = ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).Value;
                if (value is object)
                {
                    insertSql.AppendLine(comma + physicsName);
                    valueSql.AppendLine(comma + base.setParam(physicsName, ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).DBType));
                }
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_NumberType)))
            {
                physicsName = ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).PhysicsName;
                int? value = ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).Value;
                if (value is object)
                {
                    insertSql.AppendLine(comma + physicsName);
                    valueSql.AppendLine(comma + base.setParam(physicsName, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).DecimalBu));
                }
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_Number_DecimalType)))
            {
                physicsName = ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).PhysicsName;
                insertSql.AppendLine(comma + physicsName);
                valueSql.AppendLine(comma + base.setParam(physicsName, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).DecimalBu));
            }
            else
            {
                physicsName = ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).PhysicsName;
                string value = ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).Value;
                insertSql.AppendLine(comma + physicsName);
                valueSql.AppendLine(comma + base.setParam(physicsName, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).IntegerBu));
            }

            comma = ",";
            idx = idx + 1;
        }

        // INSERT文作成
        var sb = new StringBuilder();
        sb.AppendLine(string.Format(" INSERT INTO {0} ", tableName));
        sb.AppendLine(" ( ");
        sb.AppendLine(insertSql.ToString());
        sb.AppendLine(") VALUES ( ");
        sb.AppendLine(valueSql.ToString());
        sb.AppendLine(" ) ");
        return sb.ToString();
    }

    /// <summary>
    /// パラメータの用意
    /// </summary>
    /// <param name="name">パラメータ名（重複不可）</param>
    /// <param name="value">パラメータ</param>
    /// <param name="entKoumoku">エンティティの項目</param>
    private string prepareParam(string name, object value, IEntityKoumokuType entKoumoku)
    {
        return base.setParam(name, value, entKoumoku.DBType, entKoumoku.IntegerBu, entKoumoku.DecimalBu);
    }

    #endregion

}