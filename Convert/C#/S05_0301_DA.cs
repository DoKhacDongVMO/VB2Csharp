using System;
using System.Collections.Generic;
using System.Text;

public partial class S05_0301_DA : DataAccessorBase
{

    #region 定数・変数
    private int ParamNum = 0;
    #endregion

    #region SELECT処理

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessS05_0301(S05_0301SelectParamData paramInfoList = null)
    {
        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;

        // 現払登録対象コース照会
        sqlString = getGridData(paramInfoList);
        try
        {
            returnValue = getDataTable(sqlString);
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// 一覧情報取得
    /// </summary>
    /// <param name="param">検索条件</param>
    /// <returns></returns>
    public string getGridData(S05_0301SelectParamData param)
    {
        var crsLedgerBasic = new TCrsLedgerBasicEntity();
        var genbaraiEntry = new TGenbaraiEntryEntity();
        var sql = new StringBuilder();
        // パラメータクリア
        clear();
        // SQL生成
        sql.AppendLine(" SELECT ");
        sql.AppendLine("        E.SYUPT_DAY "); // 出発日
        sql.AppendLine("       ,E.RETURN_DAY "); // 帰着日
        sql.AppendLine("       ,E.CRS_CD "); // コースコード
        sql.AppendLine("       ,E.CRS_NAME "); // コース名
        sql.AppendLine("       ,E.GOUSYA "); // 号車
        sql.AppendLine("       ,E.WITHDRAWAL_EIGYOSYO_CD "); // 出金営業所コード
        sql.AppendLine("       ,ME1.EIGYOSYO_NAME_1 AS WITHDRAWAL_EIGYOSHO_NAME "); // 出金営業所
        sql.AppendLine("       ,E.RETURN_YOTEI_EIGYOSYO_CD "); // 帰着予定営業所コード
        sql.AppendLine("       ,ME2.EIGYOSYO_NAME_1 AS RETURN_YOTEI_EIGYOSYO_NAME "); // 帰着予定営業所
        sql.AppendLine("       ,E.JUNBIKIN "); // 準備金
        sql.AppendLine("       ,E.SIHARAI_KINGAKU "); // 支払金額
        sql.AppendLine("       ,E.BALANCE "); // 残額
        sql.AppendLine("       ,E.RECEIPT_GENKIN "); // 受取現金
        sql.AppendLine("       ,(E.BALANCE + E.RECEIPT_GENKIN) AS SASHIHIKI_KAHUSIOKU "); // 差引過不足
        sql.AppendLine("       ,E.KAKUTEI_DAY "); // 確定日
        sql.AppendLine("       ,E.ENTRY_PERSON "); // 登録者
        sql.AppendLine("       ,E.KAKUNIN_DATE_1 "); // 確定日１
        sql.AppendLine("       ,E.KAKUNIN_PERSON_1 "); // 確認者１
        sql.AppendLine("       ,E.KAKUNIN_PERSON_CD_1 "); // 確認者１コード
        sql.AppendLine("       ,E.KAKUNIN_DATE_2 "); // 確定日２
        sql.AppendLine("       ,E.KAKUNIN_PERSON_2 "); // 確認者２
        sql.AppendLine("       ,E.KAKUNIN_PERSON_CD_2 "); // 確認者２コード
        sql.AppendLine("       ,E.TENJYOIN_CD "); // 添乗員コード
        sql.AppendLine("       ,E.TENJYOIN_NAME "); // 添乗員名
        sql.AppendLine(" FROM ");
        sql.AppendLine("     ( ");
        sql.AppendLine("      SELECT  ");
        sql.AppendLine("             TCLB.SYUPT_DAY "); // 出発日
        sql.AppendLine("            ,TCLB.RETURN_DAY "); // 帰着日
        sql.AppendLine("            ,TCLB.CRS_CD "); // コースコード
        sql.AppendLine("            ,TCLB.CRS_NAME_RK AS CRS_NAME "); // コース名
        sql.AppendLine("            ,TCLB.GOUSYA "); // 号車
        sql.AppendLine("            ,CASE WHEN TGE.WITHDRAWAL_EIGYOSYO_CD IS NULL THEN MP.EIGYOSYO_CD ELSE TGE.WITHDRAWAL_EIGYOSYO_CD END AS WITHDRAWAL_EIGYOSYO_CD "); // 出金営業所コード
        sql.AppendLine("            ,TGE.RETURN_YOTEI_EIGYOSYO_CD "); // 帰着予定営業所コード
        sql.AppendLine("            ,NVL((A.JUNBIKIN + A.DELIVERY_KINGAKU),0) AS JUNBIKIN "); // 準備金 + 受渡金額
        sql.AppendLine("            ,NVL(A.SHISHUTSU_KINGAKU,0) AS SIHARAI_KINGAKU "); // 支払金額
        sql.AppendLine("            ,NVL((A.JUNBIKIN + A.DELIVERY_KINGAKU - A.SHISHUTSU_KINGAKU),0) AS BALANCE "); // 準備金 + 受渡金額 - 支払金額
        sql.AppendLine("            ,NVL(A.SHUNYU_KINGAKU,0) AS RECEIPT_GENKIN "); // 受取現金
        sql.AppendLine("            ,TGE.KAKUTEI_DAY "); // 確定日
        sql.AppendLine("            ,(SELECT USER_NAME FROM M_USER WHERE USER_ID = TGE.KAKUTEI_PERSON_CD AND COMPANY_CD = '0001') AS ENTRY_PERSON "); // 登録者
        sql.AppendLine("            ,TGE.KAKUNIN_DATE_1 "); // 確認日１
        sql.AppendLine("            ,TGE.KAKUNIN_PERSON_CD_1 "); // 確認者１コード
        sql.AppendLine("            ,(SELECT USER_NAME FROM M_USER WHERE USER_ID = TGE.KAKUNIN_PERSON_CD_1 AND COMPANY_CD = '0001') AS KAKUNIN_PERSON_1 "); // 確認者１
        sql.AppendLine("            ,TGE.KAKUNIN_DATE_2 "); // 確認日２
        sql.AppendLine("            ,TGE.KAKUNIN_PERSON_CD_2 "); // 確認者２コード
        sql.AppendLine("            ,(SELECT USER_NAME FROM M_USER WHERE USER_ID = TGE.KAKUNIN_PERSON_CD_2 AND COMPANY_CD = '0001') AS KAKUNIN_PERSON_2 "); // 確認者２
        sql.AppendLine("            ,CASE WHEN TGE.TENJYOIN_NAME IS NULL THEN TCLB.TENJYOIN_CD ELSE NULL END AS TENJYOIN_CD "); // 添乗員コード
        sql.AppendLine("            ,CASE WHEN TGE.TENJYOIN_NAME IS NOT NULL THEN TGE.TENJYOIN_NAME ELSE MT.TENJYOIN_NAME END AS TENJYOIN_NAME "); // 添乗員名
        sql.AppendLine("      FROM ");
        sql.AppendLine("          T_CRS_LEDGER_BASIC TCLB "); // コース台帳（基本）
        sql.AppendLine("      LEFT OUTER JOIN ");
        sql.AppendLine("          T_GENBARAI_ENTRY TGE "); // 現払登録
        sql.AppendLine("       ON  TCLB.CRS_CD = TGE.CRS_CD ");
        sql.AppendLine("       AND TCLB.SYUPT_DAY = TGE.SYUPT_DAY ");
        sql.AppendLine("       AND TCLB.GOUSYA = TGE.GOUSYA ");
        sql.AppendLine("       AND NVL(TGE.DELETE_DAY, 0) = 0 ");
        sql.AppendLine("      LEFT OUTER JOIN ");
        sql.AppendLine("          ( ");
        sql.AppendLine("           SELECT  ");
        sql.AppendLine("                  TGEA.CRS_CD "); // コースコード
        sql.AppendLine("                 ,TGEA.SYUPT_DAY "); // 出発日
        sql.AppendLine("                 ,TGEA.GOUSYA "); // 号車
        sql.AppendLine("                 ,TGEA.TENJYOIN_CD "); // 添乗員コード
        sql.AppendLine("                 ,TGEA.TENJYOIN_NAME "); // 添乗員名
        sql.AppendLine("                 ,B.JUNBIKIN "); // 準備金
        sql.AppendLine("                 ,C.SHISHUTSU_KINGAKU "); // 支出金額 支払金額
        sql.AppendLine("                 ,C.SHUNYU_KINGAKU "); // 収入金額
        sql.AppendLine("                 ,E.DELIVERY_KINGAKU "); // 受渡金額
        sql.AppendLine("           FROM ");
        sql.AppendLine("               T_GENBARAI_ENTRY TGEA"); // 現払登録
        sql.AppendLine("           LEFT OUTER JOIN ");
        sql.AppendLine("               ( ");
        sql.AppendLine("                SELECT  ");
        sql.AppendLine("                       TGWH.CRS_CD "); // コースコード
        sql.AppendLine("                      ,TGWH.SYUPT_DAY "); // 出発日
        sql.AppendLine("                      ,TGWH.GOUSYA "); // 号車
        sql.AppendLine("                      ,SUM(NVL(TGWH.WITHDRAWAL_GAKU, 0)) AS JUNBIKIN "); // 準備金
        sql.AppendLine("                FROM ");
        sql.AppendLine("                    T_GENBARAI_WITHDRAWAL_HISTORY TGWH"); // 現払出金履歴
        sql.AppendLine("                WHERE ");
        sql.AppendLine("                     NVL(TGWH.DELETE_DAY, 0) = 0 "); // 削除日
        sql.AppendLine("                GROUP BY ");
        sql.AppendLine("                         TGWH.CRS_CD ");
        sql.AppendLine("                        ,TGWH.SYUPT_DAY ");
        sql.AppendLine("                        ,TGWH.GOUSYA ");
        sql.AppendLine("               ) B ");
        sql.AppendLine("            ON  TGEA.CRS_CD = B.CRS_CD ");
        sql.AppendLine("            AND TGEA.SYUPT_DAY = B.SYUPT_DAY ");
        sql.AppendLine("            AND TGEA.GOUSYA = B.GOUSYA ");
        sql.AppendLine("           LEFT OUTER JOIN ");
        sql.AppendLine("               ( ");
        sql.AppendLine("                SELECT  ");
        sql.AppendLine("                       TCKGU.CRS_CD "); // コースコード
        sql.AppendLine("                      ,TCKGU.SYUPT_DAY "); // 出発日
        sql.AppendLine("                      ,TCKGU.GOUSYA "); // 号車
        sql.AppendLine("                      ,SUM(NVL(TCKGU.SHISHUTSU_KINGAKU, 0)) AS SHISHUTSU_KINGAKU "); // 支出金額
        sql.AppendLine("                      ,SUM(NVL(TCKGU.SHUNYU_KINGAKU, 0)) AS SHUNYU_KINGAKU "); // 収入金額
        sql.AppendLine("                FROM ");
        sql.AppendLine("                    T_COST_KAKUTEI_GENKIN_UTIWAKE TCKGU"); // 原価確定現金内訳
        sql.AppendLine("                WHERE ");
        sql.AppendLine("                     NVL(TCKGU.DELETE_DAY, 0) = 0 "); // 削除日
        sql.AppendLine("                 AND TCKGU.SIWAKE_KBN <> '11' "); // 原価確定現金内訳.仕訳区分<>'11'
        sql.AppendLine("                GROUP BY ");
        sql.AppendLine("                         TCKGU.CRS_CD ");
        sql.AppendLine("                        ,TCKGU.SYUPT_DAY ");
        sql.AppendLine("                        ,TCKGU.GOUSYA ");
        sql.AppendLine("               ) C ");
        sql.AppendLine("            ON  TGEA.CRS_CD = C.CRS_CD ");
        sql.AppendLine("            AND TGEA.SYUPT_DAY = C.SYUPT_DAY ");
        sql.AppendLine("            AND TGEA.GOUSYA = C.GOUSYA ");
        sql.AppendLine("           LEFT OUTER JOIN ");
        sql.AppendLine("               ( ");
        sql.AppendLine("                Select  ");
        sql.AppendLine("                       TTJ.CRS_CD "); // コースコード
        sql.AppendLine("                      ,TTJ.SYUPT_DAY "); // 出発日
        sql.AppendLine("                      ,TTJ.GOUSYA "); // 号車
        sql.AppendLine("                      ,TTJ.DELIVERY_KINGAKU "); // 受渡金額
        sql.AppendLine("                FROM ");
        sql.AppendLine("                    T_TENJYO_JUNBIKIN TTJ"); // 添乗準備金
        sql.AppendLine("                WHERE ");
        sql.AppendLine("                     TTJ.INPUT_KAKUTEI_FLG = '1' "); // 入力確定フラグ='1'
        sql.AppendLine("                 AND NVL(TTJ.DELETE_DAY, 0) = 0 "); // 削除日
        sql.AppendLine("               ) E ");
        sql.AppendLine("              ON  TGEA.CRS_CD = E.CRS_CD ");
        sql.AppendLine("              AND TGEA.SYUPT_DAY = E.SYUPT_DAY ");
        sql.AppendLine("              AND TGEA.GOUSYA = E.GOUSYA ");
        sql.AppendLine("           WHERE ");
        sql.AppendLine("                NVL(TGEA.DELETE_DAY, 0) = 0 ");
        sql.AppendLine("          ) A ");
        sql.AppendLine("       ON  TCLB.CRS_CD = A.CRS_CD ");
        sql.AppendLine("       AND TCLB.SYUPT_DAY = A.SYUPT_DAY ");
        sql.AppendLine("       AND TCLB.GOUSYA = A.GOUSYA ");
        sql.AppendLine("      LEFT OUTER JOIN ");
        sql.AppendLine("          M_TENJYOIN MT "); // 添乗員マスタ
        sql.AppendLine("       ON TCLB.TENJYOIN_CD = MT.TENJYOIN_CD ");
        sql.AppendLine("       AND NVL(MT.DELETE_DAY, 0) = 0 ");
        sql.AppendLine("      LEFT OUTER JOIN ");
        sql.AppendLine("          M_PLACE MP "); // 場所マスタ
        sql.AppendLine("       ON TCLB.HAISYA_KEIYU_CD_1 = MP.PLACE_CD ");
        sql.AppendLine("       AND MP.DELETE_DATE IS NULL ");
        sql.AppendLine("      WHERE ");
        sql.AppendLine("           1 = 1 ");
        sql.AppendLine("       AND NVL(TCLB.DELETE_DAY, 0) = 0 ");
        // かつ コース台帳（基本）.コースコード = パラメータ.コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            // ※コースコードフル桁入力時は他の条件は無視する
            sql.AppendLine(" AND TCLB.CRS_CD = ").Append(setSelectParam(param.CrsCd, crsLedgerBasic.CrsCd));
        }
        else
        {
            // かつ コース台帳（基本）.出発日 >= パラメータ.出発日From
            if (param.SyuptdayFrom.HasValue)
            {
                sql.AppendLine(" AND TCLB.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptdayFrom, crsLedgerBasic.SyuptDay));
            }
            // かつ コース台帳（基本）.出発日 <= パラメータ.出発日To
            if (param.SyuptdayTo.HasValue)
            {
                sql.AppendLine(" AND TCLB.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptdayTo, crsLedgerBasic.SyuptDay));
            }
            // かつ コース台帳（基本）.号車 = パラメータ.号車
            if (param.Gousya.HasValue)
            {
                sql.AppendLine(" AND TCLB.GOUSYA = ").Append(setSelectParam(param.Gousya, crsLedgerBasic.Gousya));
            }
            // パラメータ.未確定のみ != 空白
            if (!string.IsNullOrEmpty(param.UnsettledOnly))
            {
                sql.AppendLine(" AND TGE.KAKUTEI_PERSON_CD IS NULL ");
            }
            // パラメータ.出金の無いコースを含む = 空白
            if (string.IsNullOrEmpty(param.WithdrawalNasiCrsWith))
            {
                sql.AppendLine(" AND NVL(A.JUNBIKIN,0) <> 0 ");
            }
            // パラメータ.出金営業所コード
            if (!string.IsNullOrEmpty(param.WithdrawalEigyosyoCd))
            {
                sql.AppendLine(" AND TGE.WITHDRAWAL_EIGYOSYO_CD = ").Append(setSelectParam(param.WithdrawalEigyosyoCd, genbaraiEntry.WithdrawalEigyosyoCd));
            }
            // パラメータ.帰着予定営業所コード
            if (!string.IsNullOrEmpty(param.ReturnYoteiEigyosyoCd))
            {
                sql.AppendLine(" AND TGE.RETURN_YOTEI_EIGYOSYO_CD = ").Append(setSelectParam(param.ReturnYoteiEigyosyoCd, genbaraiEntry.ReturnYoteiEigyosyoCd));
            }
            // パラメータ.邦人外客区分 != 空白
            if (!string.IsNullOrEmpty(param.HoujinGaikyakuKbn))
            {
                sql.AppendLine(" AND TCLB.HOUJIN_GAIKYAKU_KBN = ").Append(setSelectParam(param.HoujinGaikyakuKbn, crsLedgerBasic.HoujinGaikyakuKbn));
            }
            // パラメータ.昼夜区分 != 空白
            if (!string.IsNullOrEmpty(param.TeikiKbn))
            {
                sql.AppendLine(" AND TCLB.CRS_KBN_1 = ").Append(setSelectParam(param.TeikiKbn, crsLedgerBasic.CrsKbn1));
            }
            // パラメータ.定期企画区分 != 空白
            if (!string.IsNullOrEmpty(param.TeikiKikakuKbn))
            {
                sql.AppendLine(" AND TCLB.TEIKI_KIKAKU_KBN = ").Append(setSelectParam(param.TeikiKikakuKbn, crsLedgerBasic.TeikiKikakuKbn));
            }
        }

        sql.AppendLine("     ) E");
        sql.AppendLine(" LEFT OUTER JOIN ");
        sql.AppendLine("     M_EIGYOSYO ME1 "); // 営業所マスタ
        sql.AppendLine("  ON E.WITHDRAWAL_EIGYOSYO_CD = ME1.EIGYOSYO_CD ");
        sql.AppendLine("  AND ME1.COMPANY_CD = '00' ");
        sql.AppendLine("  AND ME1.DELETE_DATE IS NULL ");
        sql.AppendLine(" LEFT OUTER JOIN ");
        sql.AppendLine("     M_EIGYOSYO ME2 "); // 営業所マスタ
        sql.AppendLine("  ON E.RETURN_YOTEI_EIGYOSYO_CD = ME2.EIGYOSYO_CD ");
        sql.AppendLine("  AND ME2.COMPANY_CD = '00' ");
        sql.AppendLine("  AND ME2.DELETE_DATE IS NULL ");
        sql.AppendLine("  ORDER BY ");
        sql.AppendLine("     E.SYUPT_DAY "); // 出発日
        sql.AppendLine("    ,E.CRS_CD "); // コースコード
        sql.AppendLine("    ,E.GOUSYA "); // 号車
        return sql.ToString();
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

    /// <summary>
    /// 更新処理を呼び出す
    /// </summary>
    /// <param name="updateData"></param>
    /// <returns>Integer</returns>
    public int updateCheckoutPayment(List<S05_0301UpdateParamData> updateData)
    {
        int returnValue = 0;
        string sqlString = string.Empty;
        OracleConnection con = default;

        // コネクション開始
        con = openCon();
        try
        {
            for (int row = 0, loopTo = updateData.Count - 1; row <= loopTo; row++)
            {
                sqlString = getUpdateQuery(updateData[row]);
                returnValue += execNonQuery(con, sqlString);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            if (con is object)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        return returnValue;
    }

    /// <summary>
    /// 現払登録：データ更新用
    /// </summary>
    /// <param name="paramList">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string getUpdateQuery(S05_0301UpdateParamData paramList)
    {
        var sqlString = new StringBuilder();
        // パラメータクリア
        clear();
        {
            var withBlock = new TGenbaraiEntryEntity();
            // UPDATE
            sqlString.AppendLine(" UPDATE T_GENBARAI_ENTRY ");
            sqlString.AppendLine(" SET ");
            if (paramList.NoUpdateKakunin1 == false)
            {
                // パラメータ.確認日１
                sqlString.AppendLine(" KAKUNIN_DATE_1 = ").Append(setUpdateParam(paramList.KakuninDate1, withBlock.KakuninDate1));
                // パラメータ.確認者１コード
                sqlString.AppendLine(" ,KAKUNIN_PERSON_CD_1 = ").Append(setUpdateParam(paramList.KakuninPersonCd1, withBlock.KakuninPersonCd1));
            }
            else if (paramList.NoUpdateKakunin2 == false)
            {
                // パラメータ.確認日２
                sqlString.AppendLine(" KAKUNIN_DATE_2 = ").Append(setUpdateParam(paramList.KakuninDate2, withBlock.KakuninDate2));
                // パラメータ.確認者２コード
                sqlString.AppendLine(" ,KAKUNIN_PERSON_CD_2 = ").Append(setUpdateParam(paramList.KakuninPersonCd2, withBlock.KakuninPersonCd2));
            }
            // システム更新ＰＧＭＩＤ
            sqlString.AppendLine(" ,SYSTEM_UPDATE_PGMID = ").Append(setUpdateParam(paramList.SystemUpdatePgmid, withBlock.SystemUpdatePgmid));
            // システム更新者コード
            sqlString.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = ").Append(setUpdateParam(paramList.SystemUpdatePersonCd, withBlock.SystemUpdatePersonCd));
            // システム更新日
            sqlString.AppendLine(" ,SYSTEM_UPDATE_DAY = ").Append(setUpdateParam(paramList.SystemUpdateDay, withBlock.SystemUpdateDay));
            // WHERE句
            sqlString.AppendLine(" WHERE ");
            sqlString.AppendLine("     SYUPT_DAY = ").Append(setUpdateParam(paramList.Syuptday, withBlock.SyuptDay));
            sqlString.AppendLine("  AND ");
            sqlString.AppendLine("     CRS_CD = ").Append(setUpdateParam(paramList.CrsCd, withBlock.CrsCd));
            sqlString.AppendLine("  AND ");
            sqlString.AppendLine("     GOUSYA = ").Append(setUpdateParam(paramList.Gousya, withBlock.Gousya));
            return sqlString.ToString();
        }
    }
    #endregion

    #region  パラメータ 
    public partial class S05_0301SelectParamData
    {
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? SyuptdayFrom { get; set; }
        /// <summary>
        /// 出発日TO
        /// </summary>
        public int? SyuptdayTo { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// '号車
        /// </summary>
        public int? Gousya { get; set; }
        /// <summary>
        /// 未確定のみ
        /// </summary>
        public string UnsettledOnly { get; set; }
        /// <summary>
        /// 出金の無いコースを含む
        /// </summary>
        public string WithdrawalNasiCrsWith { get; set; }
        /// <summary>
        /// 出金営業所コード
        /// </summary>
        public string WithdrawalEigyosyoCd { get; set; }
        /// <summary>
        /// 帰着予定営業所コード
        /// </summary>
        public string ReturnYoteiEigyosyoCd { get; set; }
        /// <summary>
        /// 邦人／外客区分
        /// </summary>
        public string HoujinGaikyakuKbn { get; set; }
        /// <summary>
        /// 昼夜区分
        /// </summary>
        public string TeikiKbn { get; set; }
        /// <summary>
        /// 定期・企画区分
        /// </summary>
        public string TeikiKikakuKbn { get; set; }
    }

    public partial class S05_0301UpdateParamData
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 出発日FROM
        /// </summary>
        public int? Syuptday { get; set; }
        /// <summary>
        /// '号車
        /// </summary>
        public int? Gousya { get; set; }
        /// <summary>
        /// '確認日１
        /// </summary>
        public int? KakuninDate1 { get; set; }
        /// <summary>
        /// '確認者１コード
        /// </summary>
        public string KakuninPersonCd1 { get; set; }
        /// <summary>
        /// '確認日２
        /// </summary>
        public int? KakuninDate2 { get; set; }
        /// <summary>
        /// '確認者２コード
        /// </summary>
        public string KakuninPersonCd2 { get; set; }
        /// <summary>
        /// '更新対象外 確認1
        /// </summary>
        public bool NoUpdateKakunin1 { get; set; }
        /// <summary>
        /// '更新対象外 確認2
        /// </summary>
        public bool NoUpdateKakunin2 { get; set; }
        /// <summary>
        /// システム更新ＰＧＭＩＤ
        /// </summary>
        public string SystemUpdatePgmid { get; set; }
        /// <summary>
        /// システム更新者コード
        /// </summary>
        public string SystemUpdatePersonCd { get; set; }
        /// <summary>
        /// システム更新日
        /// </summary>
        public DateTime SystemUpdateDay { get; set; }
    }
    #endregion

}