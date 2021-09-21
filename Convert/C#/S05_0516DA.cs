using System.Text;

/// <summary>
/// 発売報告書及び発券明細書データ取得のDAクラス
/// </summary>
public partial class S05_0516DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 検索 
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public DataTable selectP05_0509Details(S05_0516Param param)
    {
        var tCouponSensyaKenEntity = new TCouponSensyaKenEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("SELECT");
        sb.AppendLine("  DETAILS.COMPANY_CD_TASYA,");
        sb.AppendLine("  DETAILS.COMPANY_NAME_TASYA,");
        sb.AppendLine("  DETAILS.RELEASE_YEAR,");
        sb.AppendLine("  DETAILS.RELEASE_MONTH,");
        sb.AppendLine("  DETAILS.TICKETTYPE_CD,");
        sb.AppendLine("  DETAILS.TICKET_TYPE_NAME,");
        sb.AppendLine("  DETAILS.PRODUCT_NAME,");
        sb.AppendLine("  DETAILS.HAKKEN_EIGYOSYO_CD,");
        sb.AppendLine("  DETAILS.EIGYOSYO_NAME,");
        sb.AppendLine("  DETAILS.KENNO,");
        sb.AppendLine("  DETAILS.TICKET_AMOUNT,");
        sb.AppendLine("  DETAILS.COMMISSION,");
        sb.AppendLine("  DETAILS.FEE,");
        sb.AppendLine("  DETAILS.AMOUNT_OF_PAYMENT,");
        sb.AppendLine("  DETAILS.BIKO_4");
        sb.AppendLine("FROM");
        sb.AppendLine("  (( SELECT");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_NAME_TASYA,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,4) AS RELEASE_YEAR,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,5,2) AS RELEASE_MONTH,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.TICKETTYPE_CD,");
        sb.AppendLine("      '船車券' AS TICKET_TYPE_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.ROSEN_NAME AS PRODUCT_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD ,");
        sb.AppendLine("      M_EIGYOSYO.EIGYOSYO_NAME_1 AS EIGYOSYO_NAME,");
        sb.AppendLine("      T_SEISAN_INFO.KENNO  AS KENNO,");
        sb.AppendLine("      NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) AS TICKET_AMOUNT,");
        sb.AppendLine("      M_TASYA.COMMISSION,");
        sb.AppendLine("      TRUNC(((NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) * NVL(M_TASYA.COMMISSION,0))/100)) AS FEE,");
        sb.AppendLine("      (NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) -");
        sb.AppendLine("       TRUNC((NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) * NVL(M_TASYA.COMMISSION,0))/100)) AS AMOUNT_OF_PAYMENT,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.BIKO_4");
        sb.AppendLine("    FROM");
        sb.AppendLine("      T_COUPON_SENSYA_KEN");
        sb.AppendLine("        INNER JOIN T_SEISAN_INFO");
        sb.AppendLine("          ON (T_COUPON_SENSYA_KEN.EIGYOSYO_KBN ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.TICKETTYPE_CD ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.MOKUTEKI ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.ISSUE_YEARLY,2,'0') ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.SEQ_1 ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.SEQ_2,4,'0')) = T_SEISAN_INFO.KENNO");
        sb.AppendLine("            AND T_COUPON_SENSYA_KEN.SEQ_3 = T_SEISAN_INFO.SEQ");
        sb.AppendLine("            AND NVL(T_SEISAN_INFO.DELETE_DAY,0) = 0");
        sb.AppendLine("        INNER JOIN M_EIGYOSYO");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = M_EIGYOSYO.EIGYOSYO_CD");
        sb.AppendLine("          AND M_EIGYOSYO.COMPANY_CD = '00'");
        sb.AppendLine("          AND M_EIGYOSYO.DELETE_DATE IS NULL");
        sb.AppendLine("        LEFT JOIN M_TASYA");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = M_TASYA.COMPANY_CD_TASYA AND NVL(M_TASYA.DELETE_DAY,0) = 0");
        sb.AppendLine("   WHERE");
        sb.AppendLine("       NVL(T_COUPON_SENSYA_KEN.DELETE_DAY, 0) = 0");
        sb.AppendLine("       AND NOT (T_COUPON_SENSYA_KEN.KIND='T'");                                   // --'T'：高速バス
        sb.AppendLine("       AND T_COUPON_SENSYA_KEN.DATA_KBN='2'");                             // --'2'：払戻し
        sb.AppendLine("       AND NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) != 0");
        sb.AppendLine("       )");
        // -------- 画面で「月別出力」が指定された場合、以下の条件を追加----------------------------------
        if (param.OutputMonthly == true)
        {
            sb.AppendLine("       AND SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,6) = ").Append(setSelectParam(param.OutputTaisyoYm.Value, tCouponSensyaKenEntity.HakkenDay));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「営業所」が指定された場合、以下の条件を追加------------------------------------
        if (!string.IsNullOrEmpty(param.EigyosyoCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = ").Append(setSelectParam(param.EigyosyoCd, tCouponSensyaKenEntity.HakkenEigyosyoCd));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「運輸機関」が指定された場合、以下の条件を追加----------------------------------
        if (!string.IsNullOrEmpty(param.TasyaCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = ").Append(setSelectParam(param.TasyaCd, tCouponSensyaKenEntity.CompanyCdTasya));
        }
        // -----------------------------------------------------------------------------------------------
        sb.AppendLine("  )");
        sb.AppendLine("  UNION ALL");
        sb.AppendLine("  ( Select");                                                     // --高速バス払戻時その他分調整データ
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_NAME_TASYA,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,4) AS RELEASE_YEAR,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,5,2) AS RELEASE_MONTH,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.TICKETTYPE_CD,");
        sb.AppendLine("      '船車券' AS TICKET_TYPE_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.ROSEN_NAME AS PRODUCT_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD,");
        sb.AppendLine("      M_EIGYOSYO.EIGYOSYO_NAME_1 AS EIGYOSYO_NAME,");
        sb.AppendLine("      T_SEISAN_INFO.KENNO  AS KENNO,");
        sb.AppendLine("      (NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) - NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0)) AS TICKET_AMOUNT,");
        sb.AppendLine("      M_TASYA.COMMISSION,");
        sb.AppendLine("      TRUNC((( (NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL, 0) - NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0))");
        sb.AppendLine("                * NVL(M_TASYA.COMMISSION,0))/100)) AS FEE, ");
        sb.AppendLine("      ( (NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) - NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0))");
        sb.AppendLine("          -");
        sb.AppendLine("      TRUNC(( (NVL(T_COUPON_SENSYA_KEN.NYUKINGAKU_KEI_TTL,0) - NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0))");
        sb.AppendLine("           * NVL(M_TASYA.COMMISSION,0))/100)) AS AMOUNT_OF_PAYMENT, ");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.BIKO_4");
        sb.AppendLine("    FROM");
        sb.AppendLine("      T_COUPON_SENSYA_KEN");
        sb.AppendLine("        INNER JOIN T_SEISAN_INFO");
        sb.AppendLine("          ON (T_COUPON_SENSYA_KEN.EIGYOSYO_KBN ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.TICKETTYPE_CD ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.MOKUTEKI ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.ISSUE_YEARLY,2,'0') ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.SEQ_1 ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.SEQ_2,4,'0')) = T_SEISAN_INFO.KENNO");
        sb.AppendLine("            AND T_COUPON_SENSYA_KEN.SEQ_3 = T_SEISAN_INFO.SEQ");
        sb.AppendLine("            AND NVL(T_SEISAN_INFO.DELETE_DAY,0) = 0");
        sb.AppendLine("        INNER JOIN M_EIGYOSYO");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = M_EIGYOSYO.EIGYOSYO_CD ");
        sb.AppendLine("          AND M_EIGYOSYO.COMPANY_CD = '00'");
        sb.AppendLine("          AND M_EIGYOSYO.DELETE_DATE IS NULL");
        sb.AppendLine("        LEFT JOIN M_TASYA");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = M_TASYA.COMPANY_CD_TASYA AND NVL(M_TASYA.DELETE_DAY,0) = 0");
        sb.AppendLine("   WHERE");
        sb.AppendLine("     NVL(T_COUPON_SENSYA_KEN.DELETE_DAY, 0) = 0");
        sb.AppendLine("     AND T_COUPON_SENSYA_KEN.KIND='T'");
        sb.AppendLine("     AND T_COUPON_SENSYA_KEN.DATA_KBN='2'");
        sb.AppendLine("     AND NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) != 0");
        // -------- 画面で「月別出力」が指定された場合、以下の条件を追加----------------------------------
        if (param.OutputMonthly == true)
        {
            sb.AppendLine("       AND SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,6) = ").Append(setSelectParam(param.OutputTaisyoYm.Value, tCouponSensyaKenEntity.HakkenDay));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「営業所」が指定された場合、以下の条件を追加------------------------------------
        if (!string.IsNullOrEmpty(param.EigyosyoCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = ").Append(setSelectParam(param.EigyosyoCd, tCouponSensyaKenEntity.HakkenEigyosyoCd));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「運輸機関」が指定された場合、以下の条件を追加----------------------------------
        if (!string.IsNullOrEmpty(param.TasyaCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = ").Append(setSelectParam(param.TasyaCd, tCouponSensyaKenEntity.CompanyCdTasya));
        }
        // -----------------------------------------------------------------------------------------------
        sb.AppendLine("  )");
        sb.AppendLine("  UNION ALL");
        sb.AppendLine(" ( SELECT ");                                                               // --高速バス払戻時その他分データ
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.COMPANY_NAME_TASYA,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,4) AS RELEASE_YEAR,");
        sb.AppendLine("      SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,5,2) AS RELEASE_MONTH,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.TICKETTYPE_CD,");
        sb.AppendLine("      '船車券' AS TICKET_TYPE_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.ROSEN_NAME AS PRODUCT_NAME,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD,");
        sb.AppendLine("      M_EIGYOSYO.EIGYOSYO_NAME_1 AS EIGYOSYO_NAME,");
        sb.AppendLine("      T_SEISAN_INFO.KENNO  AS KENNO,");
        sb.AppendLine("      NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) AS TICKET_AMOUNT,");
        sb.AppendLine("      M_TASYA.COMMISSION,");
        sb.AppendLine("      TRUNC(((NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) * NVL(M_TASYA.COMMISSION,0))/100)) AS FEE,");
        sb.AppendLine("      (NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) -");
        sb.AppendLine("       TRUNC((NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) * NVL(M_TASYA.COMMISSION,0))/100)) AS AMOUNT_OF_PAYMENT,");
        sb.AppendLine("      T_COUPON_SENSYA_KEN.BIKO_4");
        sb.AppendLine("    FROM");
        sb.AppendLine("      T_COUPON_SENSYA_KEN");
        sb.AppendLine("        INNER JOIN T_SEISAN_INFO");
        sb.AppendLine("          ON (T_COUPON_SENSYA_KEN.EIGYOSYO_KBN ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.TICKETTYPE_CD ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.MOKUTEKI ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.ISSUE_YEARLY,2,'0') ||");
        sb.AppendLine("              T_COUPON_SENSYA_KEN.SEQ_1 ||");
        sb.AppendLine("              LPAD(T_COUPON_SENSYA_KEN.SEQ_2,4,'0')) = T_SEISAN_INFO.KENNO");
        sb.AppendLine("            AND T_COUPON_SENSYA_KEN.SEQ_3 = T_SEISAN_INFO.SEQ");
        sb.AppendLine("            AND NVL(T_SEISAN_INFO.DELETE_DAY,0) = 0");
        sb.AppendLine("        INNER JOIN M_EIGYOSYO");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = M_EIGYOSYO.EIGYOSYO_CD");
        sb.AppendLine("          AND M_EIGYOSYO.COMPANY_CD = '00'");
        sb.AppendLine("          AND M_EIGYOSYO.DELETE_DATE IS NULL");
        sb.AppendLine("        LEFT JOIN M_TASYA");
        sb.AppendLine("          ON T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = M_TASYA.COMPANY_CD_TASYA AND NVL(M_TASYA.DELETE_DAY,0) = 0");
        sb.AppendLine("   WHERE");
        sb.AppendLine("     NVL(T_COUPON_SENSYA_KEN.DELETE_DAY, 0) = 0");
        sb.AppendLine("     AND T_COUPON_SENSYA_KEN.KIND='T'");
        sb.AppendLine("     AND T_COUPON_SENSYA_KEN.DATA_KBN='2'");
        sb.AppendLine("     AND NVL(T_COUPON_SENSYA_KEN.SONOTA_CHARGE,0) != 0");
        // -------- 画面で「月別出力」が指定された場合、以下の条件を追加----------------------------------
        if (param.OutputMonthly == true)
        {
            sb.AppendLine("       AND SUBSTR(T_COUPON_SENSYA_KEN.HAKKEN_DAY,1,6) = ").Append(setSelectParam(param.OutputTaisyoYm.Value, tCouponSensyaKenEntity.HakkenDay));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「営業所」が指定された場合、以下の条件を追加------------------------------------
        if (!string.IsNullOrEmpty(param.EigyosyoCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.HAKKEN_EIGYOSYO_CD = ").Append(setSelectParam(param.EigyosyoCd, tCouponSensyaKenEntity.HakkenEigyosyoCd));
        }
        // -----------------------------------------------------------------------------------------------
        // -------- 画面で「運輸機関」が指定された場合、以下の条件を追加----------------------------------
        if (!string.IsNullOrEmpty(param.TasyaCd))
        {
            sb.AppendLine("       AND T_COUPON_SENSYA_KEN.COMPANY_CD_TASYA = ").Append(setSelectParam(param.TasyaCd, tCouponSensyaKenEntity.CompanyCdTasya));
        }
        // -----------------------------------------------------------------------------------------------
        sb.AppendLine("  )");
        sb.AppendLine(" ) DETAILS");
        sb.AppendLine("   ORDER BY");
        sb.AppendLine("     DETAILS.COMPANY_CD_TASYA,");
        sb.AppendLine("     DETAILS.HAKKEN_EIGYOSYO_CD,");
        sb.AppendLine("     DETAILS.PRODUCT_NAME,");
        sb.AppendLine("     DETAILS.KENNO,");
        sb.AppendLine("     DETAILS.TICKET_AMOUNT DESC");
        return base.getDataTable(sb.ToString());
    }

    public DataTable selectHakkoMoto()
    {
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("SELECT");
        sb.AppendLine("  NAIYO_1 AS ADDRESS,");
        sb.AppendLine("  NAIYO_2 AS DEPARTMENT,");
        sb.AppendLine("  NAIYO_3 AS TEL");
        sb.AppendLine("FROM");
        sb.AppendLine("  M_CODE");
        sb.AppendLine("WHERE");
        sb.AppendLine("  CODE_BUNRUI='400' AND CODE_VALUE='107' AND DELETE_DATE IS NULL");
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
    public partial class S05_0516Param
    {
        /// <summary>
        /// 営業所用
        /// </summary>
        public bool EigyoSyo { get; set; }

        /// <summary>
        /// 運輸機関発売報告書用
        /// </summary>
        public bool UnyuKikanHoukokuSyo { get; set; }

        /// <summary>
        /// 月別出力
        /// </summary>
        public bool OutputMonthly { get; set; }

        /// <summary>
        /// 出力対象年月
        /// </summary>
        public int? OutputTaisyoYm { get; set; }

        /// <summary>
        /// 日別出力
        /// </summary>
        public bool OutputDaily { get; set; }

        /// <summary>
        /// 出力対象日
        /// </summary>
        public int? OutputTaisyoYmd { get; set; }

        /// <summary>
        /// 船車券
        /// </summary>
        public bool SensyaKen { get; set; }

        /// <summary>
        /// 宿泊券
        /// </summary>
        public bool StayKen { get; set; }

        /// <summary>
        /// 観光券
        /// </summary>
        public bool KankoKen { get; set; }

        /// <summary>
        /// 旅行参加券
        /// </summary>
        public bool TravelSankaKen { get; set; }

        /// <summary>
        /// 発券日順
        /// </summary>
        public bool HakkenDayOrder { get; set; }

        /// <summary>
        /// 利用日順
        /// </summary>
        public bool RiyoDayOrder { get; set; }

        /// <summary>
        /// 営業所コード 参照 営業所名
        /// </summary>
        public string EigyosyoCd { get; set; }

        /// <summary>
        /// 運輸機関コード 参照 運輸機関名
        /// </summary>
        public string TasyaCd { get; set; }
    }
    #endregion

}