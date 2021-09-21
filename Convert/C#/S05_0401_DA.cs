using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

public partial class S05_0401_DA : DataAccessorBase
{

    #region 定数・変数

    public enum accessType : int
    {
        getEigyousyoName,             // 営業所名取得
        getGridData                  // 一覧結果取得検索
    }

    // 精算情報エンティティ
    private SeisanInfoEntity clsSeisanInfoEntity = new SeisanInfoEntity();

    #endregion

    #region SELECT処理

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessS05_0401(accessType type, Hashtable paramInfoList = null)
    {
        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case accessType.getEigyousyoName:
                {
                    // 営業所名取得
                    sqlString = getEigyousyoName(paramInfoList);
                    break;
                }

            case accessType.getGridData:
                {
                    // 一覧結果取得検索
                    sqlString = getGridData(paramInfoList);
                    break;
                }

            default:
                {
                    // 該当処理なし
                    return returnValue;
                }
        }

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
    /// 営業所名取得
    /// </summary>
    /// <param name="param">会社コードと営業所コード</param>
    /// <returns></returns>
    public string getEigyousyoName(Hashtable param)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT EIGYOSYO_NAME_1");
        sql.AppendLine("   FROM M_EIGYOSYO");
        sql.AppendLine("  WHERE COMPANY_CD=" + setParam("COMPANY_CD", param["CompanyCd"], OracleDbType.Char, 2));
        sql.AppendLine("    AND EIGYOSYO_CD=" + setParam("EIGYOSYO_CD", param["EigyousyoCd"], OracleDbType.Char, 2));
        return sql.ToString();
    }

    /// <summary>
    /// 一覧情報取得
    /// </summary>
    /// <param name="param">検索条件</param>
    /// <returns></returns>
    public string getGridData(Hashtable param)
    {
        var sql = new StringBuilder();
        // SQL生成
        sql.AppendLine(" SELECT ");
        sql.AppendLine("        SSN.SEISAN_INFO_SEQ ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_1 ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_2 ");
        sql.AppendLine("       ,LPAD(' ',4)  AS SYOHIN_CD ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_H_G ");
        sql.AppendLine("       ,SUS.OTHER_URIAGE_SYOHIN_NAME AS SYOHIN_NAME ");
        sql.AppendLine("       ,SSN.SEISAN_KBN ");
        sql.AppendLine("       ,SSN.KENNO ");
        sql.AppendLine("       ,SSN.CREATE_DAY ");
        sql.AppendLine("       ,TO_CHAR(TO_DATE(LPAD(SSN.CREATE_TIME, 6,'0'),'HH24:MI:SS'),'HH24:MI') AS TIME ");
        sql.AppendLine("       ,SSN.ENTRY_PERSON_CD ");
        sql.AppendLine("       ,SSN.SIGNON_TIME ");
        sql.AppendLine("       ,CASE WHEN SSN.URIAGE_KBN='" + FixedCd.UriageKbnType.HaraiModoshi + "' THEN ");
        sql.AppendLine("             SSN.OTHER_URIAGE_SYOHIN_MODOSI ");
        sql.AppendLine("        ELSE SSN.OTHER_URIAGE_SYOHIN_URIAGE ");
        sql.AppendLine("         END AS TOTAL ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_BIKO || OTHER_URIAGE_SYOHIN_BIKO_2 AS BIKO ");
        sql.AppendLine("       ,LPAD(' ',8) AS URIAGEINPUT ");
        sql.AppendLine("   FROM ");
        sql.AppendLine("        T_SEISAN_INFO SSN");
        sql.AppendLine("        INNER JOIN M_SONOTA_URIAGE_SYOHIN SUS ");
        sql.AppendLine("           ON SUS.OTHER_URIAGE_SYOHIN_CD_1 = SSN.OTHER_URIAGE_SYOHIN_CD_1 ");
        sql.AppendLine("          AND SUS.OTHER_URIAGE_SYOHIN_CD_2 = SSN.OTHER_URIAGE_SYOHIN_CD_2 ");
        sql.AppendLine("          AND SUS.HOUJIN_GAIKYAKU_KBN = SSN.OTHER_URIAGE_SYOHIN_H_G ");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("  WHERE ");
            // 会社コード
            sql.AppendLine("        SSN.COMPANY_CD = '" + UserInfoManagement.companyCd + "' ");
            // 必須項目：営業所コード
            sql.AppendLine("    AND SSN.EIGYOSYO_CD = " + setParam(withBlock.eigyosyoCd.PhysicsName, param(withBlock.eigyosyoCd.PhysicsName), OracleDbType.Char));
            // 必須項目：作成日
            sql.AppendLine("    AND SSN.CREATE_DAY = " + setParam(withBlock.createDay.PhysicsName, param(withBlock.createDay.PhysicsName), OracleDbType.Decimal, 8, 0));
            // 時刻From
            if (!string.IsNullOrEmpty(Conversions.ToString(param["TimeFrom"])))
            {
                sql.AppendLine(" AND SSN.CREATE_TIME >= " + setParam("TimeFrom", param["TimeFrom"], OracleDbType.Decimal, 6, 0));
            }
            // 時刻To
            if (!string.IsNullOrEmpty(Conversions.ToString(param["TimeTo"])))
            {
                sql.AppendLine(" AND SSN.CREATE_TIME <= " + setParam("TimeTo", param["TimeTo"], OracleDbType.Decimal, 6, 0));
            }
            // 商品コード
            if (!string.IsNullOrEmpty(Conversions.ToString(param(withBlock.otherUriageSyohinCd1.PhysicsName))) && !string.IsNullOrEmpty(Conversions.ToString(param(withBlock.otherUriageSyohinCd2.PhysicsName))))
            {
                sql.AppendLine(" AND SSN.OTHER_URIAGE_SYOHIN_CD_1 = " + setParam(withBlock.otherUriageSyohinCd1.PhysicsName, param(withBlock.otherUriageSyohinCd1.PhysicsName), OracleDbType.Char));
                sql.AppendLine(" AND SSN.OTHER_URIAGE_SYOHIN_CD_2 = " + setParam(withBlock.otherUriageSyohinCd2.PhysicsName, param(withBlock.otherUriageSyohinCd2.PhysicsName), OracleDbType.Char));
            }
            // ユーザーID
            if (!string.IsNullOrEmpty(Conversions.ToString(param(withBlock.entryPersonCd.PhysicsName))))
            {
                sql.AppendLine(" AND SSN.ENTRY_PERSON_CD = " + setParam(withBlock.entryPersonCd.PhysicsName, param(withBlock.entryPersonCd.PhysicsName), OracleDbType.Char));
            }
        }
        // 並び順
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("     SSN.OTHER_URIAGE_SYOHIN_CD_1 ASC ");
        sql.AppendLine("   , SSN.OTHER_URIAGE_SYOHIN_CD_2 ASC ");
        sql.AppendLine("   , SSN.CREATE_TIME ASC ");
        return sql.ToString();
    }
    #endregion

}