using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// 手仕舞い連絡のDAクラス
/// </summary>
public partial class S03_0412DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    // 手仕舞区分 = 'Y'
    private const string TEJIMAI_KBN_SUMI = "Y";
    #endregion

    #region  SELECT処理 
    /// <summary>
    /// 検索処理を呼び出す
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectDataTable(S03_0412DASelectParam param)
    {
        var sb = new StringBuilder();
        var basicEnt = new CrsLedgerBasicEntity();

        // パラメータクリア
        clear();
        sb.AppendLine("SELECT");
        sb.AppendLine(" CASE WHEN KOSHA.SEND_YMDT IS NOT NULL THEN ");
        sb.AppendLine(" 1  ");
        sb.AppendLine("  Else 0 END AS OUTPUT_FLG,  ");
        sb.AppendLine(" CASE WHEN KOSHA.SEND_YMDT IS NOT NULL  AND KOSHA.SIIRE_SAKI_KIND_CD = '30' THEN   ");
        sb.AppendLine("  1 ");
        sb.AppendLine("  Else 0 END AS KAGAMI_FLG, ");
        sb.AppendLine("  TO_YYYYMMDD_FC(BASIC.SYUPT_DAY)  AS SYUPT_DAY_STR,  ");
        sb.AppendLine("  BASIC.SYUPT_DAY AS SYUPT_DAY,  ");
        sb.AppendLine("  BASIC.CRS_CD AS CRS_CD, ");
        sb.AppendLine("  KOSHA.DAILY AS DAILY, ");
        sb.AppendLine("  KOSHA.KOSHAKASHO_CD AS SIIRE_SAKI_CD, ");
        sb.AppendLine("  KOSHA.KOSHAKASHO_EDABAN AS SIIRE_SAKI_NO, ");
        sb.AppendLine("  KOSHA.SIIRE_SAKI_NAME AS SIIRE_SAKI_NAME,  ");
        sb.AppendLine("  KOSHA.CODE_NAME AS SIIRE_SAKI_KIND, ");
        sb.AppendLine("  KOSHA.NOTIFICATION_HOHO, ");
        sb.AppendLine("  CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL　THEN  ");
        sb.AppendLine("  KOSHA.MAIL  ");
        sb.AppendLine("  ELSE KOSHA.T_CEA_CONTROL_MAIL END AS MAIL,  ");
        sb.AppendLine("  KOSHA.FAX_1, ");
        sb.AppendLine("  CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL THEN ");
        sb.AppendLine("  CASE WHEN KOSHA.FAX_1_STR = '--' THEN  ");
        sb.AppendLine("  ''  ");
        sb.AppendLine("  ELSE KOSHA.FAX_1_STR END  ");
        sb.AppendLine("  ELSE CONCAT(CONCAT(CONCAT(CONCAT(KOSHA.FAX1_1,'-'),KOSHA.FAX1_2),'-'),KOSHA.FAX1_3) END AS FAX_1_STR  ");
        sb.AppendLine("  ,FAX_2  ");
        sb.AppendLine("  ,CASE WHEN KOSHA.T_CEA_CONTROL_MAIL IS NULL  AND KOSHA.FAX1 IS NULL AND KOSHA.FAX2 IS NULL THEN  ");
        sb.AppendLine("  CASE WHEN KOSHA.FAX_2_STR = '--' THEN  ");
        sb.AppendLine("  ''  ");
        sb.AppendLine("  ELSE KOSHA.FAX_2_STR  END  ");
        sb.AppendLine("  ELSE CONCAT(CONCAT(CONCAT(CONCAT(KOSHA.FAX2_1,'-'),KOSHA.FAX2_2),'-'),KOSHA.FAX2_3) END AS FAX_2_STR  ");
        sb.AppendLine("  , TO_CHAR(KOSHA.SEND_YMDT, 'yyyy/mm/dd hh24:mi:ss')  AS LAST_SEND_DAY  ");
        sb.AppendLine("  ,KOSHA.ROOM_MAX_CAPACITY  ");
        sb.AppendLine("  ,KOSHA.SIIRE_SAKI_KIND_CD  ");
        sb.AppendLine("  ,CASE WHEN KOSHA.SYSTEM_ENTRY_DAY IS NOT NULL THEN  ");
        sb.AppendLine("  1 ");
        sb.AppendLine("  Else 0 END AS ALLOW_EDIT ");
        sb.AppendLine("  FROM T_CRS_LEDGER_BASIC BASIC ");
        sb.AppendLine("  INNER JOIN (SELECT ");
        sb.AppendLine(" T_CRS_LEDGER_KOSHAKASHO.SYUPT_DAY ");
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.CRS_CD   ");
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_CD  ");
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_EDABAN  ");
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.DAILY  ");
        sb.AppendLine(" ,T_CRS_LEDGER_KOSHAKASHO.RIYOU_DAY  ");
        sb.AppendLine(" ,CODE_SK.CODE_NAME  ");
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_NAME  ");
        sb.AppendLine(" ,SIIRE.NOTIFICATION_HOHO  ");
        sb.AppendLine(" ,SIIRE.MAIL  ");
        sb.AppendLine(" ,SIIRE.FAX_1  ");
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_1_1,'-'),SIIRE.FAX_1_2),'-'),SIIRE.FAX_1_3) AS FAX_1_STR  ");
        sb.AppendLine(" ,SIIRE.FAX_2 ");
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_2_1,'-'),SIIRE.FAX_2_2),'-'),SIIRE.FAX_2_3) AS FAX_2_STR  ");
        sb.AppendLine(" ,SIIRE.ROOM_MAX_CAPACITY  ");
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_KIND_CD  ");
        sb.AppendLine(" ,CONTROL.SEND_YMDT ");
        sb.AppendLine(",CONTROL.MAIL AS T_CEA_CONTROL_MAIL ");
        sb.AppendLine(",CONTROL.FAX1 ");
        sb.AppendLine(",CONTROL.FAX2 ");
        sb.AppendLine(",CONTROL.FAX1_1 ");
        sb.AppendLine(",CONTROL.FAX1_2 ");
        sb.AppendLine(" ,CONTROL.FAX1_3 ");
        sb.AppendLine(",CONTROL.FAX2_1 ");
        sb.AppendLine(",CONTROL.FAX2_2 ");
        sb.AppendLine(" ,CONTROL.FAX2_3 ");
        sb.AppendLine(" ,CONTROL.SYSTEM_ENTRY_DAY ");
        sb.AppendLine("  FROM T_CRS_LEDGER_KOSHAKASHO ");
        sb.AppendLine("  LEFT JOIN (SELECT ");
        sb.AppendLine(" SIIRE_SAKI_CD  ");
        sb.AppendLine(" ,SIIRE_SAKI_NO  ");
        sb.AppendLine(" ,SIIRE_SAKI_KIND_CD  ");
        sb.AppendLine(" ,SIIRE_SAKI_NAME  ");
        sb.AppendLine(" ,NOTIFICATION_HOHO  ");
        sb.AppendLine(" ,MAIL  ");
        sb.AppendLine(" ,FAX_1  ");
        sb.AppendLine(" ,FAX_1_1  ");
        sb.AppendLine(" ,FAX_1_2  ");
        sb.AppendLine(" ,FAX_1_3  ");
        sb.AppendLine(" ,FAX_2  ");
        sb.AppendLine(" ,FAX_2_1  ");
        sb.AppendLine(" ,FAX_2_2  ");
        sb.AppendLine(" ,FAX_2_3  ");
        sb.AppendLine(" ,ROOM_MAX_CAPACITY  ");
        sb.AppendLine("  FROM M_SIIRE_SAKI ");
        sb.AppendLine("  WHERE DELETE_DATE IS NULL AND SIIRE_SAKI_KIND_CD = ").Append(SuppliersKind_Koshakasho).Append(" ) SIIRE ");
        sb.AppendLine("  ON KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ");
        sb.AppendLine("  AND KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("  LEFT JOIN M_CODE CODE_SK ");
        sb.AppendLine("  On SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ");
        sb.AppendLine("  And CODE_SK.CODE_BUNRUI = ").Append(CodeBunrui.siireKind);
        sb.AppendLine("  And CODE_SK.DELETE_DATE Is NULL ");
        sb.AppendLine("  LEFT JOIN T_CEA_CONTROL CONTROL  ");
        sb.AppendLine("  ON T_CRS_LEDGER_KOSHAKASHO.CRS_CD = CONTROL.CRS_CD  ");
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.DAILY = CONTROL.DAILY ");
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_CD = CONTROL.SIIRE_SAKI_CD ");
        sb.AppendLine("  AND T_CRS_LEDGER_KOSHAKASHO.KOSHAKASHO_EDABAN = CONTROL.SIIRE_SAKI_NO ");
        sb.AppendLine("  UNION ALL ");
        sb.AppendLine("  SELECT ");
        sb.AppendLine("  T_CRS_LEDGER_HOTEL.SYUPT_DAY	 ");
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.CRS_CD	 ");
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.SIIRE_SAKI_CD As KOSHAKASHO_CD	 ");
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.SIIRE_SAKI_EDABAN As KOSHAKASHO_EDABAN	 ");
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.DAILY	 ");
        sb.AppendLine(" ,T_CRS_LEDGER_HOTEL.RIYOU_DAY	 ");
        sb.AppendLine(" ,CODE_SK.CODE_NAME	 ");
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_NAME	 ");
        sb.AppendLine(" ,SIIRE.NOTIFICATION_HOHO	 ");
        sb.AppendLine(" ,SIIRE.MAIL	 ");
        sb.AppendLine(" ,SIIRE.FAX_1	 ");
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_1_1,'-'),SIIRE.FAX_1_2),'-'),SIIRE.FAX_1_3) AS FAX_1_STR	 ");
        sb.AppendLine(" ,SIIRE.FAX_2	 ");
        sb.AppendLine(" ,CONCAT(CONCAT(CONCAT(CONCAT(SIIRE.FAX_2_1,'-'),SIIRE.FAX_2_2),'-'),SIIRE.FAX_2_3) AS FAX_2_STR	 ");
        sb.AppendLine(" ,SIIRE.ROOM_MAX_CAPACITY	 ");
        sb.AppendLine(" ,SIIRE.SIIRE_SAKI_KIND_CD AS SIIRE_SAKI_KIND_CD ");
        sb.AppendLine(" ,CONTROL.SEND_YMDT ");
        sb.AppendLine(",CONTROL.MAIL AS T_CEA_CONTROL_MAIL ");
        sb.AppendLine(",CONTROL.FAX1 ");
        sb.AppendLine(",CONTROL.FAX2 ");
        sb.AppendLine(",CONTROL.FAX1_1 ");
        sb.AppendLine(",CONTROL.FAX1_2 ");
        sb.AppendLine(" ,CONTROL.FAX1_3 ");
        sb.AppendLine(",CONTROL.FAX2_1 ");
        sb.AppendLine(",CONTROL.FAX2_2 ");
        sb.AppendLine(" ,CONTROL.FAX2_3 ");
        sb.AppendLine(" ,CONTROL.SYSTEM_ENTRY_DAY ");
        sb.AppendLine("  FROM T_CRS_LEDGER_HOTEL ");
        sb.AppendLine("  LEFT JOIN (SELECT ");
        sb.AppendLine("  SIIRE_SAKI_CD AS SIIRE_CD");
        sb.AppendLine(" ,SIIRE_SAKI_NO");
        sb.AppendLine(" ,SIIRE_SAKI_KIND_CD");
        sb.AppendLine(" ,SIIRE_SAKI_NAME");
        sb.AppendLine(" ,NOTIFICATION_HOHO");
        sb.AppendLine(" ,MAIL");
        sb.AppendLine(" ,FAX_1");
        sb.AppendLine(" ,FAX_1_1");
        sb.AppendLine(" ,FAX_1_2");
        sb.AppendLine(" ,FAX_1_3");
        sb.AppendLine(" ,FAX_2");
        sb.AppendLine(" ,FAX_2_1");
        sb.AppendLine(" ,FAX_2_2");
        sb.AppendLine(" ,FAX_2_3");
        sb.AppendLine(" ,ROOM_MAX_CAPACITY");
        sb.AppendLine("  FROM M_SIIRE_SAKI ");
        sb.AppendLine("  WHERE DELETE_DATE IS NULL AND SIIRE_SAKI_KIND_CD = ").Append(SuppliersKind_Stay).Append(" ) SIIRE ");
        sb.AppendLine("  ON SIIRE_SAKI_CD = SIIRE.SIIRE_CD ");
        sb.AppendLine("  AND SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ");
        sb.AppendLine("  LEFT JOIN M_CODE CODE_SK ");
        sb.AppendLine("  ON SIIRE.SIIRE_SAKI_KIND_CD = CODE_SK.CODE_VALUE ");
        sb.AppendLine("  AND CODE_SK.CODE_BUNRUI = ").Append(CodeBunrui.siireKind);
        sb.AppendLine("  AND CODE_SK.DELETE_DATE IS NULL ");
        sb.AppendLine("  LEFT JOIN T_CEA_CONTROL CONTROL ");
        sb.AppendLine("  ON T_CRS_LEDGER_HOTEL.CRS_CD = CONTROL.CRS_CD ");
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.DAILY = CONTROL.DAILY ");
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.SIIRE_SAKI_CD = CONTROL.SIIRE_SAKI_CD ");
        sb.AppendLine("  AND T_CRS_LEDGER_HOTEL.SIIRE_SAKI_EDABAN = CONTROL.SIIRE_SAKI_NO ");
        sb.AppendLine("  )KOSHA ");
        sb.AppendLine("  ON BASIC.SYUPT_DAY = KOSHA.SYUPT_DAY ");
        sb.AppendLine("  AND BASIC.CRS_CD = KOSHA.CRS_CD ");
        sb.AppendLine("WHERE ");
        // 出発日FROM
        sb.AppendLine("  BASIC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay));

        // 出発日TO
        if (param.SyuptDayTo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  BASIC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay));
        }

        sb.AppendLine(" AND BASIC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd));
        sb.AppendLine("  AND BASIC.TEIKI_KIKAKU_KBN = '" + Teiki_KikakuKbnType.kikakuTravel + "'");
        sb.AppendLine(" AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'*') <> '" + MaruzouKanriKbn.Maruzou + "'");
        sb.AppendLine(" AND BASIC.SAIKOU_KAKUTEI_KBN = '" + SaikouKakuteiKbn.Saikou + "'");
        sb.AppendLine("  AND NVL(BASIC.DELETE_DAY,0) = 0 ");
        sb.AppendLine("  AND BASIC.TEJIMAI_KBN = '" + TEJIMAI_KBN_SUMI + "'");
        sb.AppendLine("  ORDER BY ");
        sb.AppendLine("  BASIC.SYUPT_DAY");
        sb.AppendLine(" ,KOSHA.DAILY");
        sb.AppendLine(" ,KOSHA.KOSHAKASHO_CD");
        sb.AppendLine(" ,KOSHA.KOSHAKASHO_EDABAN");
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


    #region  UPDATE処理 

    /// <summary>
    /// 更新処理を呼び出す
    /// </summary>
    /// <param name="UpdateListData"></param>
    /// <returns>Integer</returns>
    public int updateTable(List<S03_0412DAUpdateParam> UpdateListData)
    {
        int returnValue = 0;
        string sqlString = string.Empty;
        OracleConnection con = default;

        // コネクション開始
        con = openCon();
        try
        {
            int sizeData = UpdateListData.Count;
            for (int row = 0, loopTo = sizeData - 1; row <= loopTo; row++)
            {
                sqlString = getUpdateQuery(UpdateListData[row]);
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
    private string getUpdateQuery(S03_0412DAUpdateParam param)
    {
        var tCeaControlEtt = new TCeaControlEntity();

        // SQL文字列
        var sb = new StringBuilder();

        // パラメータクリア
        clear();

        // UPDATE
        sb.AppendLine(" UPDATE T_CEA_CONTROL ");
        sb.AppendLine(" SET ");
        // 通知方法
        sb.AppendLine(" SEND_KIND = ").Append(setUpdateParam(param.ContactWay, tCeaControlEtt.SendKind));
        // 最終送信日時
        sb.AppendLine(" ,SEND_YMDT  = TO_DATE( '" + param.LastSendDay + "', 'YYYY/MM/DD HH24:MI:SS')");
        // 最終編集出発日
        sb.AppendLine(" ,LAST_SYUPT_DAY  = ").Append(setUpdateParam(param.SyuptDay, tCeaControlEtt.LastSyuptDay));
        // システム更新日
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY  = ").Append(setUpdateParam(param.SystemUpdateDay, tCeaControlEtt.SystemUpdateDay));
        // システム更新ＰＧＭＩＤ
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID  = ").Append(setUpdateParam(param.SystemUpdatePgmid, tCeaControlEtt.SystemUpdatePgmid));
        // システム更新者コード
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD  = ").Append(setUpdateParam(param.SystemUpdatePersonCd, tCeaControlEtt.SystemUpdatePersonCd));

        // WHERE句
        sb.AppendLine(" WHERE ");
        // コースコード
        sb.AppendLine(" CRS_CD = ").Append(setUpdateParam(param.CrsCd, tCeaControlEtt.CrsCd));
        sb.AppendLine(" AND DAILY = ").Append(setUpdateParam(param.Daily, tCeaControlEtt.Daily));
        sb.AppendLine(" AND SIIRE_SAKI_CD = ").Append(setUpdateParam(param.SiireSakiCd, tCeaControlEtt.SiireSakiCd));
        sb.AppendLine(" AND SIIRE_SAKI_NO = ").Append(setUpdateParam(param.SiireSakiNo, tCeaControlEtt.SiireSakiNo));
        return sb.ToString();
    }

    public string setUpdateParam(object value, IEntityKoumokuType ent)
    {
        return setParamEx(value, ent, false);
    }

    #endregion
    #region  パラメータ 
    public partial class S03_0412DASelectParam
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
    }

    public partial class S03_0412DAUpdateParam
    {
        /// <summary>
        /// 出発日
        /// </summary>
        public int SyuptDay { get; set; }
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 通知方法
        /// </summary>
        public int ContactWay { get; set; }

        /// <summary>
        /// 日次
        /// </summary>
        public string Daily { get; set; }

        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }

        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 最終送信日時
        /// </summary>
        public string LastSendDay { get; set; }
        /// <summary>
        /// システム更新日
        /// </summary>
        public DateTime SystemUpdateDay { get; set; }
        /// <summary>
        /// システム更新ＰＧＭＩＤ
        /// </summary>
        public string SystemUpdatePgmid { get; set; }
        /// <summary>
        /// システム更新者コード
        /// </summary>
        public string SystemUpdatePersonCd { get; set; }
    }
    #endregion
}