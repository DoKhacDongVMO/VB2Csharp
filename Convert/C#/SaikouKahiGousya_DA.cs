using System;
using System.Collections;
using System.Text;


/// <summary>
/// 催行可否照会（号車情報一覧）のDAクラス
/// </summary>
public partial class SaikouKahiGousya_DA : DataAccessorBase
{

    #region  定数／変数 

    public enum accessType : int
    {
        getSaikouKahiGousya             // 一覧結果取得検索
    }

    #endregion

    #region  SELECT処理 

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessSaikouKahiGousya(Hashtable paramInfoList)
    {

        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        sqlString = getSaikouKahiGousya(paramInfoList);
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
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getSaikouKahiGousya(Hashtable paramList)
    {
        var sqlString = new StringBuilder();
        paramClear();

        // SELECT句
        sqlString.AppendLine(" SELECT ");
        sqlString.AppendLine(" BASIC.GOUSYA ");                                                      // 号車
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,0) + NVL(BASIC.YOYAKU_NUM_SUB_SEAT,0) AS YOYAKU_NUM_KEI ");  // 予約数
        sqlString.AppendLine(",NVL(BASIC.KUSEKI_NUM_TEISEKI,0) + NVL(BASIC.KUSEKI_NUM_SUB_SEAT,0) AS KUSEKI_NUM_KEI ");  // 空席数
        sqlString.AppendLine(",'   ' AS YOYAKU_ALREADY_ROOM_NUM");                                   // 予約済ＲＯＯＭ数
        sqlString.AppendLine(",'   ' AS ROOM_ZANSU_SOKEI");                                          // 部屋残数総計
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_ALREADY_ROOM_NUM,0) AS YOYAKU_ALREADY_ROOM_NUM2");   // 予約済ＲＯＯＭ数（日帰り表示用）
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI2");                 // 部屋残数総計（日帰り表示用）
        sqlString.AppendLine(",NVL(YOYAKUINFOBASIC.YOYAKU_COUNT, 0) AS YOYAKU_COUNT");               // 空席数
        sqlString.AppendLine(",NVL(YOYAKUINFOBASIC2.HAKKEN_NAIYO, 0) AS HAKKEN_NAIYO");              // 予約数
        sqlString.AppendLine(",BASIC.CRS_KIND ");                                                    // コース種別
        // FROM句
        sqlString.AppendLine(" FROM ");
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ");
        // count（予約No）
        sqlString.AppendLine(" LEFT JOIN (SELECT COUNT(INFOBASIC.YOYAKU_NO) AS YOYAKU_COUNT ");
        sqlString.AppendLine(",INFOBASIC.SYUPT_DAY AS SYUPT_DAY ");
        sqlString.AppendLine(",INFOBASIC.CRS_CD AS CRS_CD ");
        sqlString.AppendLine(",INFOBASIC.GOUSYA AS GOUSYA ");
        sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC INFOBASIC ");
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" INFOBASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" INFOBASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(INFOBASIC.DELETE_DAY,0) = 0 ");
        sqlString.AppendLine(" GROUP BY INFOBASIC.SYUPT_DAY ,INFOBASIC.CRS_CD ,INFOBASIC.GOUSYA ");
        sqlString.AppendLine(" ORDER BY INFOBASIC.SYUPT_DAY ,INFOBASIC.CRS_CD ,INFOBASIC.GOUSYA ");
        sqlString.AppendLine(" ) YOYAKUINFOBASIC ");
        sqlString.AppendLine(" ON ");
        sqlString.AppendLine(" BASIC.CRS_CD = YOYAKUINFOBASIC.CRS_CD");
        sqlString.AppendLine(" AND");
        sqlString.AppendLine(" BASIC.GOUSYA = YOYAKUINFOBASIC.GOUSYA");
        sqlString.AppendLine(" AND");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = YOYAKUINFOBASIC.SYUPT_DAY");
        // count（発券内容）
        sqlString.AppendLine(" LEFT JOIN (SELECT COUNT(INFOBASIC2.HAKKEN_NAIYO) AS HAKKEN_NAIYO ");
        sqlString.AppendLine(",INFOBASIC2.SYUPT_DAY AS SYUPT_DAY ");
        sqlString.AppendLine(",INFOBASIC2.CRS_CD AS CRS_CD ");
        sqlString.AppendLine(",INFOBASIC2.GOUSYA AS GOUSYA ");
        sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC INFOBASIC2 ");
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" INFOBASIC2.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" INFOBASIC2.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" INFOBASIC2.HAKKEN_NAIYO IN('" + FixedCd.HakkenNaiyo.Zenkin + "','" + FixedCd.HakkenNaiyo.Zankin + "') ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(INFOBASIC2.DELETE_DAY,0) = 0 ");
        sqlString.AppendLine(" GROUP BY INFOBASIC2.SYUPT_DAY ,INFOBASIC2.CRS_CD ,INFOBASIC2.GOUSYA ");
        sqlString.AppendLine(" ORDER BY INFOBASIC2.SYUPT_DAY ,INFOBASIC2.CRS_CD ,INFOBASIC2.GOUSYA ");
        sqlString.AppendLine(" ) YOYAKUINFOBASIC2 ");
        sqlString.AppendLine(" ON ");
        sqlString.AppendLine(" BASIC.CRS_CD = YOYAKUINFOBASIC2.CRS_CD");
        sqlString.AppendLine(" AND");
        sqlString.AppendLine(" BASIC.GOUSYA = YOYAKUINFOBASIC2.GOUSYA");
        sqlString.AppendLine(" AND");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = YOYAKUINFOBASIC2.SYUPT_DAY");
        // WHERE句
        sqlString.AppendLine(" WHERE ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN = " + setParam("HOUJIN_GAIKYAKU_KBN", paramList["HOUJIN_GAIKYAKU_KBN"], OracleDbType.Char));
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" (BASIC.MARU_ZOU_MANAGEMENT_KBN = '" + string.Empty + "'");
        sqlString.AppendLine(" OR ");
        sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN IS NULL) ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(BASIC.SAIKOU_KAKUTEI_KBN,'*') NOT IN('" + FixedCd.SaikouKakuteiKbn.Haishi + "') ");
        sqlString.AppendLine(" AND ");
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ");
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.SYUPT_DAY = YOYAKUINFOBASIC.SYUPT_DAY ")
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.CRS_CD = YOYAKUINFOBASIC.CRS_CD ")
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.GOUSYA = YOYAKUINFOBASIC.GOUSYA ")
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.SYUPT_DAY = YOYAKUINFOBASIC2.SYUPT_DAY ")
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.CRS_CD = YOYAKUINFOBASIC2.CRS_CD ")
        // sqlString.AppendLine(" AND ")
        // sqlString.AppendLine(" BASIC.GOUSYA = YOYAKUINFOBASIC2.GOUSYA ")
        // ORDER BY句
        sqlString.AppendLine(" ORDER BY ");
        sqlString.AppendLine(" BASIC.SYUPT_DAY ");
        sqlString.AppendLine(",BASIC.CRS_CD ");
        sqlString.AppendLine(",BASIC.GOUSYA ");
        return sqlString.ToString();
    }
    #endregion

}