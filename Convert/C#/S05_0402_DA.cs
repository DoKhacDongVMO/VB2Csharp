using System;
using System.Collections.Generic;
using System.Reflection;
using static System.String;
using System.Text;
using Hatobus.ReservationManagementSystem.Yoyaku;
using Oracle.ManagedDataAccess.Client;

public partial class S05_0402_DA : DataAccessorBase
{

    #region 定数・変数

    public enum accessType : int
    {
        getSeisanKomoku,                 // 精算情報
        getSeisanInfo,                   // 精算情報
        getSeisanInfoAll,                // 精算情報(全Field)
        getSeisanUriage,                 // 精算情報売上
        getSeisanKaisyu,                 // 精算情報内訳（回収先）
        getOtherUriageSyohin            // その他売上商品マスタ
    }

    // ''' <summary>
    // ''' 精算区分コード(その他商品売上)
    // ''' </summary>
    // Private Const strSeisankbn As String = "40" ':精算区分(その他商品売上)

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
    public DataTable accessS05_0402(accessType type, P05_0401ParamData paramInfoList)
    {
        // SQL文字列
        string sqlString = Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case accessType.getOtherUriageSyohin:
                {
                    // その他売上商品マスタ
                    sqlString = getOtherUriageSyohin(paramInfoList);
                    break;
                }

            case accessType.getSeisanKomoku:
                {
                    // 精算項目
                    sqlString = getSeisanKomoku(paramInfoList);
                    break;
                }

            case accessType.getSeisanInfoAll:
                {
                    // 精算情報(全項目)
                    sqlString = getSeisanInfoAll(paramInfoList);
                    break;
                }

            case accessType.getSeisanInfo:
                {
                    // 精算情報
                    sqlString = getSeisanInfo(paramInfoList);
                    break;
                }

            case accessType.getSeisanUriage:
                {
                    // 精算情報内訳
                    sqlString = getSeisanUriage(paramInfoList);
                    break;
                }

            case accessType.getSeisanKaisyu:
                {
                    // 精算情報内訳（回収先）
                    sqlString = getSeisanKaisyu(paramInfoList);
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
    /// 精算項目マスタ取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public string getSeisanKomoku(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        // SQL生成

        sql.AppendLine(" SELECT ");
        sql.AppendLine("        SK.SEISAN_KOUMOKU_CD ");         // 精算項目コード
        sql.AppendLine("       ,SK.SEISAN_KOUMOKU_NAME ");       // 精算項目名
        sql.AppendLine("       ,SK.TAISYAKU_KBN ");              // 貸借区分
        sql.AppendLine("   FROM M_SEISAN_KOUMOKU SK");
        sql.AppendLine("  WHERE SK.SONOTA_SYOHIN_FLG = 1 ");     // その他商品フラグ(0:OFF 1:ON)
        sql.AppendLine("  ORDER BY SK.SEISAN_KOUMOKU_CD ");
        return sql.ToString();
    }

    /// <summary>
    /// 精算情報内訳（回収先）取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public string getSeisanKaisyu(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        // SQL生成
        sql.AppendLine(" SELECT ");
        sql.AppendLine("     SI.RECOVERY_SAKI_DAY ");                            // 回収先日
        sql.AppendLine("    ,SI.RECOVERY_SAKI_YOYAKU_KBN ");                     // 回収先予約区分
        sql.AppendLine("    ,SI.RECOVERY_SAKI_YOYAKU_NO ");                      // 回収先予約ＮＯ
        sql.AppendLine("    ,SI.RECOVERY_SAKI_TYPE ");                           // 回収先種類
        sql.AppendLine("    ,SI.OTHER_URIAGE_NAME_BF ");                         // 他売上名前
        sql.AppendLine("    ,SI.OTHER_URIAGE_AITESAKI ");                        // 他売上相手先
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_BIKO ");                     // 他売上商品備考
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_BIKO_2 ");                   // 他売上商品備考２
        sql.AppendLine("    ,SI.OTHER_URIAGE_SYOHIN_H_G ");                      // 他売上商品邦人／外客区分
        sql.AppendLine("    ,YIB.SURNAME_KJ || YIB.NAME_KJ AS SEIMEI");       // 姓（漢字）＋名（漢字）
        sql.AppendLine("    ,SI.CRS_CD ");                                       // コースコード
        // コース名略称
        sql.AppendLine("    ,(SELECT DISTINCT NVL(CRS_NAME_RK,CRS_NAME) FROM T_CRS_LEDGER_BASIC WHERE CRS_CD = SI.CRS_CD AND SYUPT_DAY = SI.RECOVERY_SAKI_DAY ) AS CRS_NAME_RK ");
        sql.AppendLine("    ,SUS.OTHER_URIAGE_SYOHIN_NAME AS KAISYU_OTHER_URIAGE_SYOHIN_NAME");      // 回収先:他売上商品名
        sql.AppendLine("    ,SIA.OTHER_URIAGE_SYOHIN_BIKO AS KAISYU_OTHER_URIAGE_SYOHIN_BIKO");      // 回収先:他売上商品備考
        sql.AppendLine("    ,SIA.OTHER_URIAGE_SYOHIN_BIKO_2 AS KAISYU_OTHER_URIAGE_SYOHIN_BIKO_2");  // 回収先:他売上商品備考２
        sql.AppendLine("    ,SUS.OTHER_URIAGE_SYOHIN_NAME ");                    // 他売上商品名
        sql.AppendLine(" FROM   ");
        sql.AppendLine("     T_SEISAN_INFO SI ");                            // 精算情報
        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO SIA  ");                    // 精算情報A(回収先)
        sql.AppendLine("   ON SI.RECOVERY_SAKI_YOYAKU_KBN = SIA.YOYAKU_KBN ");
        sql.AppendLine("  AND SI.RECOVERY_SAKI_YOYAKU_NO = SIA.YOYAKU_NO ");
        sql.AppendLine("  AND SI.RECOVERY_SAKI_SEQ = SIA.SEISAN_INFO_SEQ ");
        sql.AppendLine(" LEFT JOIN  T_YOYAKU_INFO_BASIC YIB ");              // 予約情報（基本）
        sql.AppendLine("   ON SI.RECOVERY_SAKI_YOYAKU_KBN = YIB.YOYAKU_KBN ");
        sql.AppendLine("  AND SI.RECOVERY_SAKI_YOYAKU_NO = YIB.YOYAKU_NO ");
        sql.AppendLine(" LEFT JOIN  M_SONOTA_URIAGE_SYOHIN SUS ");           // その他売上商品マスタ
        sql.AppendLine("   ON SI.OTHER_URIAGE_SYOHIN_CD_1 = SUS.OTHER_URIAGE_SYOHIN_CD_1 ");
        sql.AppendLine("  AND SI.OTHER_URIAGE_SYOHIN_CD_2 = SUS.OTHER_URIAGE_SYOHIN_CD_2 ");
        sql.AppendLine("  AND SI.OTHER_URIAGE_SYOHIN_H_G = SUS.HOUJIN_GAIKYAKU_KBN ");
        sql.AppendLine(" WHERE ");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("      SI.SEISAN_KBN = '" + FixedCd.SeisanKbn.SonotaSyohinUriage + "' "); // 精算区分 = 40(その他商品売上)
            sql.AppendLine("  AND SI.KENNO = " + setParam(withBlock.kenno.PhysicsName, param.KenNO, OracleDbType.Char));                            // 券番
            sql.AppendLine("  AND SI.SEISAN_INFO_SEQ = " + setParam(withBlock.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char));   // 精算情報SEQ
            // sql.AppendLine("  AND SI.SEQ = '" & setParam(.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char) & "'")
        }

        return sql.ToString();
    }

    /// <summary>
    /// 精算情報取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public string getSeisanInfoAll(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("        * ");
        sql.AppendLine("   FROM ");
        sql.AppendLine("        T_SEISAN_INFO SSN");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("  WHERE ");
            // 会社コード
            sql.AppendLine("        COMPANY_CD = " + setParam(withBlock.companyCd.PhysicsName, param.CompanyCd, OracleDbType.Char));
            // 必須項目：営業所コード
            sql.AppendLine("    AND EIGYOSYO_CD = " + setParam(withBlock.eigyosyoCd.PhysicsName, param.EigyosyoCd, OracleDbType.Char));
            // 必須項目：作成日
            sql.AppendLine("    AND CREATE_DAY = " + setParam(withBlock.createDay.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0));
            // 時刻
            // sql.AppendLine("    AND SSN.CREATE_TIME = " & setParam(.createTime.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0))

            // 商品コード
            sql.AppendLine(" AND OTHER_URIAGE_SYOHIN_CD_1 = " + setParam(withBlock.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char));
            sql.AppendLine(" AND OTHER_URIAGE_SYOHIN_CD_2 = " + setParam(withBlock.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char));
        }

        return sql.ToString();
    }


    /// <summary>
    /// 精算情報取得
    /// </summary>
    /// <param name="seisanInfoSeq"></param>
    /// <returns></returns>
    public string getSeisanInfoUtiwakeAll(int seisanInfoSeq)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("        * ");
        sql.AppendLine("   FROM ");
        sql.AppendLine("        T_SEISAN_INFO_UTIWAKE ");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("  WHERE ");
            // 会社コード
            sql.AppendLine("        SEISAN_INFO_SEQ = " + setParam(withBlock.seisanInfoSeq.PhysicsName, seisanInfoSeq, OracleDbType.Int16));
        }

        return sql.ToString();
    }

    /// <summary>
    /// 精算情報取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public string getSeisanInfo(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        // SQL生成

        sql.AppendLine(" SELECT ");
        sql.AppendLine("        SSN.SEISAN_INFO_SEQ ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_1 ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_CD_2 ");
        sql.AppendLine("       ,LPAD(' ',4)  AS SYOHIN_CD ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_H_G ");
        sql.AppendLine("       ,SUS.TAISYAKU_KBN ");                      // その他売上商品マスタ.貸借区分(1:借方、2:貸方)
        sql.AppendLine("       ,SUS.OTHER_URIAGE_SYOHIN_NAME ");          // その他売上商品マスタ.他売上商品名
        sql.AppendLine("       ,SSN.SEISAN_KBN ");
        sql.AppendLine("       ,SSN.KENNO ");
        sql.AppendLine("       ,SSN.CREATE_DAY "); // 作成日	
        sql.AppendLine("       ,TO_CHAR(TO_DATE(LPAD(SSN.CREATE_TIME, 6,'0'),'HH24:MI:SS'),'HH24:MI:SS') AS TIME ");
        sql.AppendLine("       ,SSN.ENTRY_PERSON_CD ");
        sql.AppendLine("       ,SSN.SIGNON_TIME ");
        sql.AppendLine("       ,CASE WHEN SSN.URIAGE_KBN='" + FixedCd.UriageKbnType.HaraiModoshi + "' THEN ");
        sql.AppendLine("             SSN.OTHER_URIAGE_SYOHIN_MODOSI ");
        sql.AppendLine("        ELSE SSN.OTHER_URIAGE_SYOHIN_URIAGE ");
        sql.AppendLine("        END AS TOTAL ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_MODOSI ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_URIAGE ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_QUANTITY "); // 他売上商品数量
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_TANKA "); // 他売上商品単価
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_BIKO ");
        sql.AppendLine("       ,SSN.OTHER_URIAGE_SYOHIN_BIKO_2 ");
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
            sql.AppendLine("        SSN.COMPANY_CD = " + setParam(withBlock.companyCd.PhysicsName, param.CompanyCd, OracleDbType.Char));
            // 必須項目：営業所コード
            sql.AppendLine("    AND SSN.EIGYOSYO_CD = " + setParam(withBlock.eigyosyoCd.PhysicsName, param.EigyosyoCd, OracleDbType.Char));
            // 必須項目：作成日
            sql.AppendLine("    AND SSN.CREATE_DAY = " + setParam(withBlock.createDay.PhysicsName, param.ProcessDate, OracleDbType.Decimal, 8, 0));
            // 精算情報SEQ
            sql.AppendLine("    AND SSN.SEISAN_INFO_SEQ = " + setParam(withBlock.seq.PhysicsName, param.SEQ, OracleDbType.Decimal, 12, 0));
            // 券番
            sql.AppendLine("    AND SSN.KENNO = " + setParam(withBlock.kenno.PhysicsName, param.KenNO, OracleDbType.Char));
            // 商品コード
            sql.AppendLine("    AND SSN.OTHER_URIAGE_SYOHIN_CD_1 = " + setParam(withBlock.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char));
            sql.AppendLine("    AND SSN.OTHER_URIAGE_SYOHIN_CD_2 = " + setParam(withBlock.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char));
        }
        // ' 並び順
        // sql.AppendLine(" ORDER BY ")
        // sql.AppendLine("     SSN.OTHER_URIAGE_SYOHIN_CD_1 ASC ")
        // sql.AppendLine("   , SSN.OTHER_URIAGE_SYOHIN_CD_2 ASC ")
        // sql.AppendLine("   , SSN.CREATE_TIME ASC ")
        return sql.ToString();
    }

    /// <summary>
    /// 精算情報(内訳)取得
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public string getSeisanUriage(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        // SQL生成
        sql.AppendLine(" SELECT");
        sql.AppendLine("     SK.SEISAN_KOUMOKU_CD");
        sql.AppendLine("   , SK.SEISAN_KOUMOKU_NAME");
        sql.AppendLine("   , SK.TAISYAKU_KBN");
        sql.AppendLine("   , SU.BIKO");
        sql.AppendLine("   , SU.HURIKOMI_KBN");
        sql.AppendLine("   , NVL(SUM(SU.KINGAKU),0) AS URIAGE");
        sql.AppendLine("   , SU.ISSUE_COMPANY_CD");
        sql.AppendLine(" FROM");
        sql.AppendLine("   M_SEISAN_KOUMOKU SK");
        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO_UTIWAKE SU");
        sql.AppendLine("   ON SK.SEISAN_KOUMOKU_CD = SU.SEISAN_KOUMOKU_CD");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("   AND SU.SEISAN_KBN = " + setParam(withBlock.seisanKbn.PhysicsName, FixedCd.SeisanKbn.SonotaSyohinUriage.Trim, OracleDbType.Char));
            sql.AppendLine("   AND SU.KENNO = " + setParam(withBlock.kenno.PhysicsName, param.KenNO, OracleDbType.Char));     // 40:精算区分(その他商品売上)
            sql.AppendLine("   AND SU.SEISAN_INFO_SEQ = " + setParam(withBlock.seisanInfoSeq.PhysicsName, param.SEQ, OracleDbType.Char));
        }

        sql.AppendLine(" LEFT JOIN T_SEISAN_INFO SI");
        sql.AppendLine(" ON SU.SEISAN_INFO_SEQ = SI.SEISAN_INFO_SEQ");
        sql.AppendLine(" AND SU.SEISAN_KBN = SI.SEISAN_KBN");
        sql.AppendLine(" AND SU.KENNO = SI.KENNO  ");
        sql.AppendLine(" GROUP BY");
        sql.AppendLine("     SK.SEISAN_KOUMOKU_CD");
        sql.AppendLine("   , SK.SEISAN_KOUMOKU_NAME");
        sql.AppendLine("   , SK.TAISYAKU_KBN");
        sql.AppendLine("   , SU.BIKO");
        sql.AppendLine("   , SU.HURIKOMI_KBN");
        sql.AppendLine("   , SU.ISSUE_COMPANY_CD");
        sql.AppendLine(" ORDER BY");
        sql.AppendLine("     SK.TAISYAKU_KBN ");
        sql.AppendLine("    ,SK.SEISAN_KOUMOKU_CD ");
        return sql.ToString();
    }

    /// <summary>
    /// その他売上商品マスタ取得
    /// </summary>
    /// <param name="param">その他商品コード1,2</param>
    /// <returns></returns>
    public string getOtherUriageSyohin(P05_0401ParamData param)
    {
        var sql = new StringBuilder();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("    HOUJIN_GAIKYAKU_KBN");               // 邦人外客区分
        sql.AppendLine("   ,OTHER_URIAGE_SYOHIN_NAME");          // 他売上商品名
        sql.AppendLine("   ,TAISYAKU_KBN ");                     // その他売上商品マスタ.貸借区分
        sql.AppendLine(" FROM ");
        sql.AppendLine("    M_SONOTA_URIAGE_SYOHIN");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("  WHERE ");
            // その他商品コード1,2
            sql.AppendLine("        OTHER_URIAGE_SYOHIN_CD_1 = " + setParam(withBlock.otherUriageSyohinCd1.PhysicsName, param.SyohinCd1, OracleDbType.Char));
            sql.AppendLine("    AND OTHER_URIAGE_SYOHIN_CD_2 = " + setParam(withBlock.otherUriageSyohinCd2.PhysicsName, param.SyohinCd2, OracleDbType.Char));
        }

        return sql.ToString();
    }

    /// <summary>
    /// SEQ（精算情報）の取得（精算区分（'40'）、券番単位で採番)
    /// </summary>
    /// <param name="kenNo"></param>
    /// <returns></returns>
    public DataTable getMaxSeq(string kenNo)
    {
        base.paramClear();
        var ent = new SeisanInfoEntity();
        var sb = new StringBuilder();

        // パラメータの取得
        string prmKenNo = this.prepareParam("KENNO", kenNo, ent.yoyakuKbn);
        string prmSeisanKbn = this.prepareParam("SEISAN_KBN", FixedCd.SeisanKbn.SonotaSyohinUriage.Trim, ent.seisanKbn);
        try
        {
            sb.AppendLine(" SELECT");
            sb.AppendLine("     NVL(MAX(SEQ), 0) AS SEQ");
            sb.AppendLine(" FROM");
            sb.AppendLine("     T_SEISAN_INFO");
            sb.AppendLine(" WHERE ");
            sb.AppendLine(" ");
            sb.AppendLine($"         KENNO = {prmKenNo}");
            sb.AppendLine($"     AND SEISAN_KBN = {prmSeisanKbn}");
            sb.AppendLine(" ORDER BY");
            sb.AppendLine("     YOYAKU_KBN, YOYAKU_NO");
            return base.getDataTable(sb.ToString());
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region 登録/更新処理

    /// <summary>
    /// 登録用SQLの作成
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nameTable"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    private string createSqlInsert<T>(string nameTable, T entity) where T : class
    {
        var properties = typeof(T).GetProperties();
        var insertSql = new StringBuilder();
        var valueSql = new StringBuilder();
        int idx = 0;
        string comma = "";
        string physicsName = "";
        string param = "";

        // プロパティを取り出し、プロパティの型に応じて登録SQLのカラム名とVALUE部分を生成
        foreach (PropertyInfo prop in properties)
        {
            if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_YmdType)))
            {
                // 日付型の場合
                // プロパティ名取得
                EntityKoumoku_YmdType propYmdType = (EntityKoumoku_YmdType)prop.GetValue(entity);
                physicsName = propYmdType.PhysicsName;
                param = base.setParam(physicsName, propYmdType.Value, propYmdType.DBType);
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_NumberType)))
            {
                // 数字型の場合
                // プロパティ名取得
                EntityKoumoku_NumberType propNumberType = (EntityKoumoku_NumberType)prop.GetValue(entity);
                physicsName = propNumberType.PhysicsName;
                param = this.prepareParam(physicsName, propNumberType.Value, propNumberType);
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_Number_DecimalType)))
            {
                // Decimal型の場合
                // プロパティ名取得
                EntityKoumoku_Number_DecimalType propNumberDecimaltype = (EntityKoumoku_Number_DecimalType)prop.GetValue(entity);
                physicsName = propNumberDecimaltype.PhysicsName;
                param = this.prepareParam(physicsName, propNumberDecimaltype.Value, propNumberDecimaltype);
            }
            else
            {
                // 文字型の場合
                // プロパティ名取得
                EntityKoumoku_MojiType propMojiType = (EntityKoumoku_MojiType)prop.GetValue(entity);
                physicsName = propMojiType.PhysicsName;
                param = this.prepareParam(physicsName, propMojiType.Value, propMojiType);
            }

            // SQL生成
            insertSql.AppendLine($"{comma} {physicsName}");
            valueSql.AppendLine($"{comma} {param}");
            comma = ",";
            idx += 1;
        }


        // INSERT文作成
        var sb = new StringBuilder();
        sb.AppendLine($" INSERT INTO {nameTable}");
        sb.AppendLine(" ( ");
        sb.AppendLine(insertSql.ToString());
        sb.AppendLine(" ) VALUES ( ");
        sb.AppendLine(valueSql.ToString());
        sb.AppendLine(" ) ");
        return sb.ToString();
    }

    /// <summary>
    /// 精算情報の更新SQLの作成 
    /// </summary>
    /// <param name="ent"></param>
    /// <returns></returns>
    private string createSqlUpdateSeisanInfo(SeisanInfoEntity ent)
    {
        // パラメータの設定
        string prmCompanyCd = this.prepareParam("COMPANY_CD", ent.companyCd.Value, ent.companyCd);
        string prmEigyosyoCd = this.prepareParam("EIGYOSYO_CD", ent.eigyosyoCd.Value, ent.eigyosyoCd);
        string prmCrsCd = this.prepareParam("CRS_CD", ent.crsCd.Value, ent.crsCd);
        string prmNosignKbn = this.prepareParam("NOSIGN_KBN", ent.nosignKbn.Value, ent.nosignKbn);
        string prmOtherUriageAitesaki = this.prepareParam("OTHER_URIAGE_AITESAKI", ent.otherUriageAitesaki.Value, ent.otherUriageAitesaki);
        string prmOtherUriageNameBf = this.prepareParam("OTHER_URIAGE_NAME_BF", ent.otherUriageNameBf.Value, ent.otherUriageNameBf);
        string prmOtherUriageSyohinBiko = this.prepareParam("OTHER_URIAGE_SYOHIN_BIKO", ent.otherUriageSyohinBiko.Value, ent.otherUriageSyohinBiko);
        string prmOtherUriageSyohinBiko2 = this.prepareParam("OTHER_URIAGE_SYOHIN_BIKO_2", ent.otherUriageSyohinBiko2.Value, ent.otherUriageSyohinBiko2);
        string prmOtherUriageSyohinQuantity = this.prepareParam("OTHER_URIAGE_SYOHIN_QUANTITY", ent.otherUriageSyohinQuantity.Value, ent.otherUriageSyohinQuantity);
        string prmOtherUriageSyohinTanka = this.prepareParam("OTHER_URIAGE_SYOHIN_TANKA", ent.otherUriageSyohinTanka.Value, ent.otherUriageSyohinTanka);
        string prmOtherUriageSyohinUriage = this.prepareParam("OTHER_URIAGE_SYOHIN_URIAGE", ent.otherUriageSyohinUriage.Value, ent.otherUriageSyohinUriage);
        string prmOtherUriageSyohinModosi = this.prepareParam("OTHER_URIAGE_SYOHIN_MODOSI", ent.otherUriageSyohinModosi.Value, ent.otherUriageSyohinModosi);
        string prmRecoverySakiDay = this.prepareParam("RECOVERY_SAKI_DAY", ent.recoverySakiDay.Value, ent.recoverySakiDay);
        string prmRecoverySakiSeq = this.prepareParam("RECOVERY_SAKI_SEQ", ent.recoverySakiSeq.Value, ent.recoverySakiSeq);
        string prmRecoverySakiType = this.prepareParam("RECOVERY_SAKI_TYPE", ent.recoverySakiType.Value, ent.recoverySakiType);
        string prmRecoverySakiKenno = this.prepareParam("RECOVERY_SAKI_KENNO", ent.recoverySakiKenno.Value, ent.recoverySakiKenno);
        string prmRecoverySakiYoyakuKbn = this.prepareParam("RECOVERY_SAKI_YOYAKU_KBN", ent.recoverySakiYoyakuKbn.Value, ent.recoverySakiYoyakuKbn);
        string prmRecoverySakiYoyakuNo = this.prepareParam("RECOVERY_SAKI_YOYAKU_NO", ent.recoverySakiYoyakuNo.Value, ent.recoverySakiYoyakuNo);
        string prmSeisanKbn = this.prepareParam("SEISAN_KBN", ent.seisanKbn.Value, ent.seisanKbn);
        string prmSignonTime = this.prepareParam("SIGNON_TIME", ent.signonTime.Value, ent.signonTime);
        string prmUpdateDay = this.prepareParam("UPDATE_DAY", ent.updateDay.Value, ent.updateDay);
        string prmUpdatePersonCd = this.prepareParam("UPDATE_PERSON_CD", ent.updatePersonCd.Value, ent.updatePersonCd);
        string prmUpdatePgmid = this.prepareParam("UPDATE_PGMID", ent.updatePgmid.Value, ent.updatePgmid);
        string prmUpdateTime = this.prepareParam("UPDATE_TIME", ent.updateTime.Value, ent.updateTime);
        string prmUriagekBN = this.prepareParam("URIAGE_KBN", ent.uriageKbn.Value, ent.uriageKbn);
        string prmSystemUpdatePgmId = this.prepareParam("SYSTEM_UPDATE_PGMID", ent.systemUpdatePgmid.Value, ent.systemUpdatePgmid);
        string prmSystemUpdatePersonCd = this.prepareParam("SYSTEM_UPDATE_PERSON_CD", ent.systemUpdatePersonCd.Value, ent.systemUpdatePersonCd);
        string prmSystemUpdateDay = this.prepareParam("SYSTEM_UPDATE_DAY", ent.systemUpdateDay.Value, ent.systemUpdateDay);
        string prmKenNo = this.prepareParam("KENNO", ent.kenno.Value, ent.kenno);           // 券番
        string prmSeq = this.prepareParam("SEQ", ent.seq.Value, ent.seq);                   // ＳＥＱ
        var sb = new StringBuilder();
        sb.AppendLine("UPDATE");
        sb.AppendLine("    T_SEISAN_INFO ");
        sb.AppendLine("SET ");
        sb.AppendLine($"     COMPANY_CD = {prmCompanyCd}");                                      // 会社コード
        sb.AppendLine($"    ,EIGYOSYO_CD = {prmEigyosyoCd}");                                    // 営業所コード
        sb.AppendLine($"    ,CRS_CD = {prmCrsCd}");                                              // コースコード
        sb.AppendLine($"    ,NOSIGN_KBN = {prmNosignKbn}");                                      // ノーサイン区分
        sb.AppendLine($"    ,OTHER_URIAGE_AITESAKI = {prmOtherUriageAitesaki}");                 // 他売上相手先
        sb.AppendLine($"    ,OTHER_URIAGE_NAME_BF = {prmOtherUriageNameBf}");                    // 他売上名前
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_BIKO = {prmOtherUriageSyohinBiko}");            // 他売上商品備考 
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_BIKO_2 = {prmOtherUriageSyohinBiko2}");         // 他売上商品備考２
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_QUANTITY = {prmOtherUriageSyohinQuantity}");    // 他売上商品数量
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_TANKA = {prmOtherUriageSyohinTanka}");          // 他売上商品単価
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_URIAGE = {prmOtherUriageSyohinUriage}");        // 他売上商品売上
        sb.AppendLine($"    ,OTHER_URIAGE_SYOHIN_MODOSI = {prmOtherUriageSyohinModosi}");        // 他売上商品払戻
        sb.AppendLine($"    ,RECOVERY_SAKI_DAY = {prmRecoverySakiDay}");                         // 回収先日	
        sb.AppendLine($"    ,RECOVERY_SAKI_KENNO = {prmRecoverySakiKenno}");                     // 回収先券番
        sb.AppendLine($"    ,RECOVERY_SAKI_SEQ = {prmRecoverySakiSeq}");                         // 回収先ＳＥＱ
        sb.AppendLine($"    ,RECOVERY_SAKI_TYPE = {prmRecoverySakiType}");                       // 回収先種類       
        sb.AppendLine($"    ,RECOVERY_SAKI_YOYAKU_KBN = {prmRecoverySakiYoyakuKbn}");            // 回収先予約区分
        sb.AppendLine($"    ,RECOVERY_SAKI_YOYAKU_NO = {prmRecoverySakiYoyakuNo}");              // 回収先予約ＮＯ
        sb.AppendLine($"    ,SIGNON_TIME = {prmSignonTime}");                                    // サインオン時刻
        sb.AppendLine($"    ,UPDATE_DAY = {prmUpdateDay}");                                      // 更新日
        sb.AppendLine($"    ,UPDATE_PERSON_CD = {prmUpdatePersonCd}");                           // 更新者コード
        sb.AppendLine($"    ,UPDATE_PGMID = {prmUpdatePgmid}");                                  // 更新ＰＧＭＩＤ
        sb.AppendLine($"    ,UPDATE_TIME = {prmUpdateTime}");                                    // 更新時刻
        sb.AppendLine($"    ,URIAGE_KBN = {prmUriagekBN}");                                      // 更新時刻
        sb.AppendLine($"    ,SYSTEM_UPDATE_PGMID = {prmSystemUpdatePgmId}");                     // システム更新ＰＧＭＩＤ
        sb.AppendLine($"    ,SYSTEM_UPDATE_PERSON_CD = {prmSystemUpdatePersonCd}");              // システム更新者コード
        sb.AppendLine($"    ,SYSTEM_UPDATE_DAY = {prmSystemUpdateDay}");                         // システム更新日
        sb.AppendLine(" WHERE 1=1");
        sb.AppendLine($" AND SEISAN_KBN = {prmSeisanKbn}");                                      // 精算区分
        sb.AppendLine($" AND KENNO  = {prmKenNo}");                                              // 券番
        sb.AppendLine($" AND SEQ  = {prmSeq}");                                                  // ＳＥＱ
        return sb.ToString();
    }

    /// <summary>
    /// 精算情報内訳の削除SQLの作成
    /// </summary>
    /// <param name="seisanInfoSeq"></param>
    /// 精算情報SEQ
    /// <returns></returns>
    private string createSqlDeleteSeisanInfoUtiwake(int seisanInfoSeq)
    {
        var ent = new SeisanInfoUtiwakeEntity();
        string prmSeisanInfoSeq = this.prepareParam("SEISAN_INFO_SEQ", seisanInfoSeq, ent.seisanInfoSeq);
        var sb = new StringBuilder();
        sb.AppendLine("DELETE");
        sb.AppendLine("    T_SEISAN_INFO_UTIWAKE");
        sb.AppendLine("WHERE 1=1");
        sb.AppendLine($"    And SEISAN_INFO_SEQ  = {prmSeisanInfoSeq}");
        return sb.ToString();
    }

    /// <summary>
    /// その他商品・その他精算の登録
    /// </summary>
    /// <param name="s05_0402Entity"></param>
    /// <returns></returns>
    public bool insertOtherSeisanGroup(S05_0402Entity s05_0402Entity)
    {
        OracleTransaction oraTran = default;
        int returnValue = 0;
        try
        {
            // トランザクション開始
            oraTran = base.callBeginTransaction();
            string sql = "";
            int updateCount = 0; // 更新件数

            // 精算情報
            foreach (SeisanInfoEntity ent in s05_0402Entity.SeisanInfoEntityList)
            {
                base.paramClear();
                string insertSeisanInfoQuery = this.createSqlInsert<SeisanInfoEntity>(CommonHakken.NameTblSeisanInfo, ent);
                updateCount = base.execNonQuery(oraTran, insertSeisanInfoQuery);
                if (updateCount <= 0)
                {
                    base.callRollbackTransaction(oraTran);
                    return false;
                }
            }

            // 精算情報内訳
            foreach (SeisanInfoUtiwakeEntity ent in s05_0402Entity.SeisanInfoUtiwakeEntityList)
            {
                base.paramClear();
                string insertSeisanInfoUtiwakeQuery = this.createSqlInsert<SeisanInfoUtiwakeEntity>(CommonHakken.NameTblSeisanInfoUtiwake, ent);
                updateCount = base.execNonQuery(oraTran, insertSeisanInfoUtiwakeQuery);
                if (updateCount <= 0)
                {
                    base.callRollbackTransaction(oraTran);
                    return false;
                }
            }

            // トランザクションコミット
            base.callCommitTransaction(oraTran);
        }
        catch (Exception ex)
        {
            base.callRollbackTransaction(oraTran);
            throw;
        }
        finally
        {
            oraTran.Dispose();
        }

        return true;
    }

    /// <summary>
    /// その他商品・その他精算の更新
    /// </summary>
    /// <returns></returns>
    public bool updateOtherSeisanGroup(S05_0402Entity s05_0402Entity, P05_0401ParamData param)
    {
        OracleTransaction oraTran = default;
        int updateCount = 0;
        try
        {
            oraTran = base.callBeginTransaction();

            // 精算情報
            foreach (SeisanInfoEntity ent in s05_0402Entity.SeisanInfoEntityList)
            {
                base.paramClear();
                string updateSeisanInfoQuery = createSqlUpdateSeisanInfo(ent);
                updateCount = base.execNonQuery(oraTran, updateSeisanInfoQuery);
                if (updateCount <= 0)
                {
                    base.callRollbackTransaction(oraTran);
                    return false;
                }
            }
            // 精算情報内訳
            string deleteSeisanInfoQuery = createSqlDeleteSeisanInfoUtiwake(param.SEQ);
            // 修正前データ削除
            updateCount = base.execNonQuery(oraTran, deleteSeisanInfoQuery);
            // 新規追加
            List<SeisanInfoUtiwakeEntity> seisanInfoEntityList = s05_0402Entity.SeisanInfoUtiwakeEntityList;
            foreach (SeisanInfoUtiwakeEntity ent in s05_0402Entity.SeisanInfoUtiwakeEntityList)
            {
                base.paramClear();
                string insertSeisanInfoUtiwakeQuery = this.createSqlInsert<SeisanInfoUtiwakeEntity>(CommonHakken.NameTblSeisanInfoUtiwake, ent);
                updateCount = base.execNonQuery(oraTran, insertSeisanInfoUtiwakeQuery);
                if (updateCount <= 0)
                {
                    base.callRollbackTransaction(oraTran);
                    return false;
                }
            }

            base.callCommitTransaction(oraTran);
        }
        catch (Exception ex)
        {
            base.callRollbackTransaction(oraTran);
            throw;
        }
        finally
        {
            oraTran.Dispose();
        }

        return true;
    }

    /// <summary>
    /// パラメータの用意
    /// </summary>
    /// <param name="name">パラメータ名（重複不可）</param>
    /// <param name="value">パラメータ</param>
    /// <param name="entKoumoku">予約情報（基本）の項目</param>
    private string prepareParam(string name, object value, IEntityKoumokuType entKoumoku)
    {
        return base.setParam(name, value, entKoumoku.DBType, entKoumoku.IntegerBu, entKoumoku.DecimalBu);
    }

    #endregion

}