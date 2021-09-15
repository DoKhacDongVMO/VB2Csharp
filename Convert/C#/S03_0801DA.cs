using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// リクエスト仕入登録・照会	のDAクラス
/// </summary>
public partial class S03_0801DA : DataAccessorBase
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
    public DataTable selectDataTable(S03_0801DASelectParam param)
    {
        var reqInfoEtt = new WtRequestInfoEntity();
        var basicEtt = new CrsLedgerBasicEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // SELECT句
        sb.AppendLine("SELECT ");
        sb.AppendLine("       CASE STATE ");
        sb.AppendLine("         WHEN '1' THEN 1 ");
        sb.AppendLine("         ELSE 0 ");
        sb.AppendLine("       END AS STATE ");                                                                               // 状態
        sb.AppendLine("     , CASE CANCEL_FLG ");
        sb.AppendLine("         WHEN '1' THEN 1 ");
        sb.AppendLine("         ELSE 0 ");
        sb.AppendLine("       END AS CANCEL_FLG ");                                                                          // キャンセルフラグ
        sb.AppendLine("     , TO_CHAR(TO_DATE(ENTRY_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS RESERVATION_DATE_STR ");                // 登録日
        sb.AppendLine("     , TO_CHAR(TO_DATE(SYUPT_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS SYUPT_DAY_STR ");                       // 出発日
        sb.AppendLine("     , CRS_CD AS CRS_CD ");                                                                           // コースコード
        sb.AppendLine("     , CRS_NAME AS CRS_NAME ");                                                                       // コース名
        sb.AppendLine("     , TO_CHAR(GOUSYA) AS GOUSYA ");                                                                  // 号車
        sb.AppendLine("     , MANAGEMENT_KBN || ',' || TO_CHAR(MANAGEMENT_NO, 'FM000,000,000') AS REQUEST_NO ");             // 管理区分 + 管理ＮＯ
        sb.AppendLine("     , NAME AS NAME ");                                                                               // 名前
        sb.AppendLine("     , NINZU AS YOYAKU_NUM ");                                                                        // 人数
        sb.AppendLine("     , TEL_NO AS TEL_NO ");                                                                           // 電話番号
        sb.AppendLine("     , TO_CHAR(TO_DATE(UPDATE_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS UPDATE_DAY_STR ");                     // 更新日
        sb.AppendLine("     , MEMO AS MEMO ");                                                                               // 内容
        sb.AppendLine("     , TO_CHAR(MEMO_UPDATE_DAY, 'YYYY/MM/DD HH24:MI') AS MEMO_UPDATE_DAY ");                          // システム更新日
        sb.AppendLine("     , MEMO_UPDATE_PERSON AS MEMO_UPDATE_PERSON_CD ");                                                // システム更新者コード
        sb.AppendLine("     , MANAGEMENT_KBN AS MANAGEMENT_KBN_HIDDEN ");                                                    // 管理区分
        sb.AppendLine("     , MANAGEMENT_NO AS MANAGEMENT_NO_HIDDEN ");                                                      // 管理ＮＯ
        // FROM句
        sb.AppendLine("FROM P03_0801");
        sb.AppendLine("WHERE 1 = 1 ");
        // 予約受付日/出発日FROM
        if (param.SyuptDayFrom is object)
        {
            if (param.Kijyun == true)
            {
                sb.AppendLine("  AND ");
                sb.AppendLine("  ENTRY_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, reqInfoEtt.entryDay));
            }
            else if (param.Kijyun == false)
            {
                sb.AppendLine("  AND ");
                sb.AppendLine("  SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEtt.syuptDay));
            }
        }
        // 予約受付日/出発日TO
        if (param.SyuptDayTo is object)
        {
            if (param.Kijyun == true)
            {
                sb.AppendLine("  AND ");
                sb.AppendLine("  ENTRY_DAY <= ").Append(setSelectParam(param.SyuptDayTo, reqInfoEtt.entryDay));
            }
            else if (param.Kijyun == false)
            {
                sb.AppendLine("  AND ");
                sb.AppendLine("  SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEtt.syuptDay));
            }
        }
        // コースコード
        if (!string.IsNullOrEmpty(param.CrsCd))
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEtt.crsCd));
        }
        // 邦人／外客区分
        if (param.CrsJapanese == true && param.CrsForeign == false)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  HOUJIN_GAIKYAKU_KBN = ").Append(setSelectParam(HoujinGaikyakuKbnType.Houjin, basicEtt.houjinGaikyakuKbn));
        }
        else if (param.CrsJapanese == false && param.CrsForeign == true)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  HOUJIN_GAIKYAKU_KBN = ").Append(setSelectParam(HoujinGaikyakuKbnType.Gaikyaku, basicEtt.houjinGaikyakuKbn));
        }

        if (param.CrsKbnHiru == true || param.CrsKbnYoru == true || param.CrsKbnDay == true || param.CrsKbnStay == true || param.CrsKbnR == true)
        {
            // コース種別/コース区分１
            sb.AppendLine(" AND ");
            sb.AppendLine(" ( ");
            sb.AppendLine("  1 <> 1 ");
            // 定期（昼） = TRUEの場合
            if (param.CrsKbnHiru == true)
            {
                sb.AppendLine("  OR ");
                sb.AppendLine("  (");
                sb.AppendLine("   CRS_KIND = ").Append(setSelectParam(CrsKindType.hatoBusTeiki, basicEtt.crsKind));
                sb.AppendLine("    AND ");
                sb.AppendLine("   CRS_KBN_1 = ").Append(setSelectParam(CrsKbn1Type.noon, basicEtt.crsKbn1));
                sb.AppendLine("  )");
            }
            // 定期（夜）＝TRUEの場合
            if (param.CrsKbnYoru == true)
            {
                sb.AppendLine("  OR ");
                sb.AppendLine("  (");
                sb.AppendLine("   CRS_KIND = ").Append(setSelectParam(CrsKindType.hatoBusTeiki, basicEtt.crsKind));
                sb.AppendLine("    AND ");
                sb.AppendLine("   CRS_KBN_1 = ").Append(setSelectParam(CrsKbn1Type.night, basicEtt.crsKbn1));
                sb.AppendLine("  )");
            }
            // 企画（日帰り）＝TRUEの場合
            if (param.CrsKbnDay == true)
            {
                sb.AppendLine("  OR ");
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CrsKindType.higaeri, basicEtt.crsKind));
            }
            // 企画（宿泊）＝TRUEの場合
            if (param.CrsKbnStay == true)
            {
                sb.AppendLine("  OR ");
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CrsKindType.stay, basicEtt.crsKind));
            }
            // 企画（Ｒコース）＝TRUEの場合
            if (param.CrsKbnR == true)
            {
                sb.AppendLine("  OR ");
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CrsKindType.rcourse, basicEtt.crsKind));
            }

            sb.AppendLine(" ) ");
        }
        // 状態
        if (param.SiireMachiFlg == true && param.SiireZumiFlg == false)
        {
            sb.AppendLine(" AND ");
            sb.AppendLine(" STATE <> ").Append(setSelectParam(1.ToString(), reqInfoEtt.state));
        }
        else if (param.SiireMachiFlg == false && param.SiireZumiFlg == true)
        {
            sb.AppendLine(" AND ");
            sb.AppendLine(" STATE = ").Append(setSelectParam(1.ToString(), reqInfoEtt.state));
        }
        // キャンセルフラグ
        if (param.DeleteFlg == false)
        {
            sb.AppendLine(" AND ");
            sb.AppendLine(" CANCEL_FLG IS NULL ");
        }
        // ORDER句
        sb.AppendLine(" ORDER BY ");
        if (param.Kijyun == true)
        {
            // 登録日
            sb.AppendLine("  ENTRY_DAY ");
            // 管理区分
            sb.AppendLine("  , MANAGEMENT_KBN ");
            // 管理ＮＯ
            sb.AppendLine("  , MANAGEMENT_NO ");
            // 枝番
            sb.AppendLine("  , MEMO_UPDATE_DAY DESC ");
        }
        else if (param.Kijyun == false)
        {
            // 出発日
            sb.AppendLine("  SYUPT_DAY ");
            // コースコード
            sb.AppendLine("  , CRS_CD ");
            // 号車
            sb.AppendLine("  , GOUSYA ");
            // 管理区分
            sb.AppendLine("  , MANAGEMENT_KBN ");
            // 管理ＮＯ
            sb.AppendLine("  , MANAGEMENT_NO ");
            // 枝番
            sb.AppendLine("  , MEMO_UPDATE_DAY DESC ");
        }

        return base.getDataTable(sb.ToString());
    }
    #endregion

    #region  UPDATE処理 
    /// <summary>
    /// 更新処理を呼び出す
    /// </summary>
    /// <param name="lsUpdateData"></param>
    /// <returns>Integer</returns>
    public int updateTable(List<S03_0801DAUpdateParam> lsUpdateData)
    {
        int returnValue = 0;
        string sqlString = string.Empty;
        OracleConnection con = default;

        // コネクション開始
        con = openCon();
        try
        {
            foreach (S03_0801DAUpdateParam item in lsUpdateData)
            {
                sqlString = getUpdateQuery(item);
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

                if (con is object)
                {
                    con.Dispose();
                }
            }
        }

        return returnValue;
    }

    /// <summary>
    /// UPDATE用DBアクセス
    /// </summary>
    /// <param name="param"></param>
    /// <returns>Integer</returns>
    public string getUpdateQuery(S03_0801DAUpdateParam param)
    {
        var reqInfoEtt = new WtRequestInfoEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // UPDATE
        sb.AppendLine(" UPDATE T_WT_REQUEST_INFO ");
        sb.AppendLine(" SET ");
        // 状態
        if (param.SiireFlg == true)
        {
            sb.AppendLine("  STATE = ").Append(setUpdateParam(1.ToString(), reqInfoEtt.state) + ", ");
        }
        // キャンセルフラグ
        if (param.DeleteFlg == true)
        {
            sb.AppendLine("  CANCEL_FLG = ").Append(setUpdateParam(1.ToString(), reqInfoEtt.cancelFlg) + ", ");
        }
        else
        {
            sb.AppendLine("  CANCEL_FLG = NULL, ");
        }
        // 削除日
        if (param.DeleteFlg == true)
        {
            sb.AppendLine("  DELETE_DAY = " + (setParam("DELETE_DAY", CommonDateUtil.getSystemTime.ToString(CommonFormatType.dateFormatyyyyMMdd), OracleDbType.Decimal, 8, 0) + ", "));
        }
        else
        {
            sb.AppendLine("  DELETE_DAY = 0, ");
        }
        // システム更新ＰＧＭＩＤ
        sb.AppendLine("  SYSTEM_UPDATE_PGMID = ").Append(setUpdateParam(param.SystemUpdatePgmid, reqInfoEtt.systemUpdatePgmid) + ", ");
        // システム更新者コード
        sb.AppendLine("  SYSTEM_UPDATE_PERSON_CD = ").Append(setUpdateParam(param.SystemUpdatePersonCd, reqInfoEtt.systemUpdatePersonCd) + ", ");
        // システム更新日
        sb.AppendLine("  SYSTEM_UPDATE_DAY = ").Append(setUpdateParam(param.SystemUpdateDay, reqInfoEtt.systemUpdateDay));
        // WHERE句
        sb.AppendLine(" WHERE ");
        // 管理区分
        sb.AppendLine(" MANAGEMENT_KBN = ").Append(setUpdateParam(param.ManagementKbn, reqInfoEtt.managementKbn));
        // 管理ＮＯ
        sb.AppendLine(" AND MANAGEMENT_NO = ").Append(setUpdateParam(param.ManagementNo, reqInfoEtt.managementNo));
        return sb.ToString();
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
    #endregion

    #region  パラメータ 

    public partial class S03_0801DASelectParam
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
        /// 日本語
        /// </summary>
        public bool CrsJapanese { get; set; }
        /// <summary>
        /// 外国語
        /// </summary>
        public bool CrsForeign { get; set; }
        /// <summary>
        /// 定期（昼）
        /// </summary>
        public bool CrsKbnHiru { get; set; }
        /// <summary>
        /// 定期（夜）
        /// </summary>
        public bool CrsKbnYoru { get; set; }
        /// <summary>
        /// 企画（日帰り）
        /// </summary>
        public bool CrsKbnDay { get; set; }
        /// <summary>
        /// 企画（宿泊）
        /// </summary>
        public bool CrsKbnStay { get; set; }
        /// <summary>
        /// 企画（Ｒコース）
        /// </summary>
        public bool CrsKbnR { get; set; }
        /// <summary>
        /// 仕入れ待ち
        /// </summary>
        public bool SiireMachiFlg { get; set; }
        /// <summary>
        /// 仕入れ済み
        /// </summary>
        public bool SiireZumiFlg { get; set; }
        /// <summary>
        /// 削除含む
        /// </summary>
        public bool DeleteFlg { get; set; }
        /// <summary>
        /// 基準
        /// </summary>
        public bool Kijyun { get; set; }
    }

    public partial class S03_0801DAUpdateParam
    {
        /// <summary>
        /// 管理区分
        /// </summary>
        public string ManagementKbn { get; set; }
        /// <summary>
        /// 管理ＮＯ
        /// </summary>
        public int ManagementNo { get; set; }
        /// <summary>
        /// 仕入
        /// </summary>
        public bool SiireFlg { get; set; }
        /// <summary>
        /// 削除
        /// </summary>
        public bool DeleteFlg { get; set; }
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