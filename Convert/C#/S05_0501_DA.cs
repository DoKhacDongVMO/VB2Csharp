using System;
using System.Collections;
using System.Linq;
using System.Text;

public partial class S05_0501_DA : DataAccessorBase
{

    #region 定数・変数

    public enum accessType : int
    {
        getEigyousyoName,             // 営業所名取得
        getNippoKingaku,              // 日報金額取得
        getGridData                  // 一覧結果取得検索
    }

    // 精算情報エンティティ
    private SeisanInfoEntity clsSeisanInfoEntity = new SeisanInfoEntity();
    // 内訳情報（クレジット会社）エンティティ
    private TUtiwakeInfoCreditCompanyEntity clsUtiwakeInfoCreditCompanyEntity = new TUtiwakeInfoCreditCompanyEntity();

    #endregion

    #region SELECT処理

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessS05_0501(accessType type, Hashtable paramInfoList = null)
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

            case accessType.getNippoKingaku:
                {
                    // 日報金額取得
                    sqlString = getNippoKingaku(paramInfoList);
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
    /// 日報金額取得
    /// </summary>
    /// <param name="param">検索条件</param>
    /// <returns></returns>
    public string getNippoKingaku(Hashtable param)
    {
        var sql = new StringBuilder();
        // SQL生成
        sql.AppendLine(" SELECT ");
        sql.AppendLine("        SSN.COMPANY_CD ");                        // 精算情報.会社コード
        sql.AppendLine("       ,SSN.EIGYOSYO_CD ");                       // 精算情報.営業所コード
        sql.AppendLine("       ,SSN.CREATE_DAY ");                        // 精算情報.作成日
        sql.AppendLine("       ,SUM(SIUA.KINGAKU) AS NCREDITKINGAKU  ");  // SUM(精算情報内訳.金額) AS 日報クレジット金額
        sql.AppendLine("       ,SUM(SIUB.KINGAKU) AS NCREDITMODOSHI ");    // SUM(精算情報内訳.金額) AS 日報クレジット払戻
        sql.AppendLine("   FROM ");
        sql.AppendLine("        T_SEISAN_INFO SSN");  // 精算情報
        sql.AppendLine("        LEFT JOIN T_SEISAN_INFO_UTIWAKE SIUA ");  // 精算情報内訳A
        sql.AppendLine("           ON SIUA.SEISAN_INFO_SEQ = SSN.SEISAN_INFO_SEQ ");
        sql.AppendLine("          AND SIUA.SEISAN_KOUMOKU_CD = '" + FixedCd.SeisanItemCd.credit_card + "' ");
        sql.AppendLine("        LEFT JOIN T_SEISAN_INFO_UTIWAKE SIUB ");  // 精算情報内訳B
        sql.AppendLine("           ON SIUB.SEISAN_INFO_SEQ = SSN.SEISAN_INFO_SEQ ");
        sql.AppendLine("          AND SIUB.SEISAN_KOUMOKU_CD = '" + FixedCd.SeisanItemCd.credit_card_modosi + "' ");
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine("  WHERE ");
            // ノーサイン区分
            sql.AppendLine("        NVL(SSN.NOSIGN_KBN,' ') = ' ' ");
            // 会社コード
            sql.AppendLine("    AND SSN.COMPANY_CD = " + setParam(withBlock.companyCd.PhysicsName, param(withBlock.companyCd.PhysicsName), OracleDbType.Char));
            // 必須項目：営業所コード
            sql.AppendLine("    AND SSN.EIGYOSYO_CD = " + setParam(withBlock.eigyosyoCd.PhysicsName, param(withBlock.eigyosyoCd.PhysicsName), OracleDbType.Char));
            // 必須項目：作成日
            sql.AppendLine("    AND SSN.CREATE_DAY = " + setParam(withBlock.createDay.PhysicsName, param(withBlock.createDay.PhysicsName), OracleDbType.Decimal, 8, 0));
        }
        // グループ化
        sql.AppendLine(" GROUP BY ");
        sql.AppendLine("     SSN.COMPANY_CD ");   // 精算情報.会社コード
        sql.AppendLine("   , SSN.EIGYOSYO_CD ");  // 精算情報.営業所コード
        sql.AppendLine("   , SSN.CREATE_DAY ");   // 精算情報.作成日
        // 並び順
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("     SSN.COMPANY_CD ASC ");   // 精算情報.会社コード 昇順
        sql.AppendLine("   , SSN.EIGYOSYO_CD ASC ");  // 精算情報.営業所コード 昇順
        sql.AppendLine("   , SSN.CREATE_DAY ASC ");   // 精算情報.作成日 昇順
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
        {
            var withBlock = clsSeisanInfoEntity;
            sql.AppendLine(" SELECT ");
            sql.AppendLine("        KMC.CREDIT_CD AS CODE ");    // 科目・クレジット会社対応マスタ.クレジット会社コード AS コード
            sql.AppendLine("       ,KMC.CREDIT_NAME AS NAME ");  // 科目・クレジット会社対応マスタ.クレジット会社名称 AS 名称
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.URIAGE_KENSU,0), '" + CommonFormatType.numberFormatComma + "') AS URIAGE_KENSU ");                // 内訳情報（クレジット会社）.売上件数
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.URIAGE_KINGAKU,0), '" + CommonFormatType.numberFormatCommaLargeDigit + "') AS URIAGE_KINGAKU ");  // 内訳情報（クレジット会社）.売上金額
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.RETURN_KENSU,0), '" + CommonFormatType.numberFormatComma + "') AS RETURN_KENSU ");                // 内訳情報（クレジット会社）.戻し件数
            sql.AppendLine("       ,TO_CHAR(NVL(UIC.RETURN_KINGAKU,0), '" + CommonFormatType.numberFormatCommaLargeDigit + "') AS RETURN_KINGAKU ");  // 内訳情報（クレジット会社）.戻し金額
            sql.AppendLine("       ,NVL(UIC.COMPANY_CD," + setParam(withBlock.companyCd.PhysicsName, param(withBlock.companyCd.PhysicsName), OracleDbType.Char) + ") AS COMPANY_CD ");           // 内訳情報（クレジット会社）.会社コード
            sql.AppendLine("       ,NVL(UIC.EIGYOSYO_CD, " + setParam(withBlock.eigyosyoCd.PhysicsName, param(withBlock.eigyosyoCd.PhysicsName), OracleDbType.Char) + ") AS EIGYOSYO_CD ");          // 内訳情報（クレジット会社）.営業所コード
            sql.AppendLine("       ,NVL(UIC.CREATE_DAY, " + setParam(withBlock.createDay.PhysicsName, param(withBlock.createDay.PhysicsName), OracleDbType.Decimal, 8, 0) + ") AS CREATE_DAY ");           // 内訳情報（クレジット会社）.作成日
            sql.AppendLine("   FROM ");
            sql.AppendLine("        M_KAMOKU_CREDIT KMC");  // 科目・クレジット会社対応マスタ
            sql.AppendLine("        LEFT JOIN T_UTIWAKE_INFO_CREDIT_COMPANY UIC ");  // 内訳情報（クレジット会社）
            sql.AppendLine("           ON UIC.CREDIT_COMPANY_CD = KMC.CREDIT_CD ");
            // 会社コード
            sql.AppendLine("          AND UIC.COMPANY_CD = " + setParam(withBlock.companyCd.PhysicsName, param(withBlock.companyCd.PhysicsName), OracleDbType.Char));
            // 必須項目：営業所コード
            sql.AppendLine("          AND UIC.EIGYOSYO_CD = " + setParam(withBlock.eigyosyoCd.PhysicsName, param(withBlock.eigyosyoCd.PhysicsName), OracleDbType.Char));
            // 必須項目：作成日
            sql.AppendLine("          AND UIC.CREATE_DAY = " + setParam(withBlock.createDay.PhysicsName, param(withBlock.createDay.PhysicsName), OracleDbType.Decimal, 8, 0));
            sql.AppendLine("  WHERE ");
            // 削除日
            sql.AppendLine("        NVL(KMC.DELETE_DATE,'0') = '0' ");
        }
        // 並び順
        sql.AppendLine(" ORDER BY ");
        sql.AppendLine("     KMC.CREDIT_CD ASC ");  // 科目・クレジット会社対応マスタ.クレジット会社コード 昇順
        return sql.ToString();
    }
    #endregion

    #region 更新処理

    /// <summary>
    /// 内訳情報(クレジット会社)更新
    /// </summary>
    /// <param name="updateInfoList"></param>
    /// <returns></returns>
    public int executeTUtiwakeInfoCreditCompany(DataTable updateInfoList)
    {
        OracleTransaction oracleTransaction = default;
        int returnValue = 0;
        string sqlString = string.Empty;
        try
        {
            // トランザクション開始
            oracleTransaction = callBeginTransaction();
            if (updateInfoList is object && updateInfoList.Rows.Count > 0)
            {
                // 更新対象情報有り
                for (int i = 0, loopTo = updateInfoList.Rows.Count - 1; i <= loopTo; i++)
                {
                    sqlString = executeUpdateTUtiwakeInfoCreditCompany(i, updateInfoList);
                    if (execNonQuery(oracleTransaction, sqlString) > 0)
                    {
                        returnValue = 1;
                        // パラメータの初期化[更新(前回)のパラメータをクリア]
                        paramClear();
                    }
                    else
                    {
                        returnValue = 0;
                        break;
                    }
                }
            }

            if (returnValue == 1)
            {
                // 一括登録成功
                // コミット
                callCommitTransaction(oracleTransaction);
            }
            else
            {
                // 一括登録失敗
                // ロールバック
                callRollbackTransaction(oracleTransaction);
            }
        }
        catch (Exception ex)
        {
            callRollbackTransaction(oracleTransaction);
            throw;
        }
        finally
        {
            oracleTransaction.Dispose();
        }

        return returnValue;
    }

    /// <summary>
    /// 内訳情報(クレジット会社)更新用
    /// </summary>
    /// <param name="targetRowIndex"></param>
    /// <param name="updateInfoList"></param>
    /// <returns></returns>
    private string executeUpdateTUtiwakeInfoCreditCompany(int targetRowIndex, DataTable updateInfoList)
    {
        var sqlString = new StringBuilder();
        DateTime sysDate = CommonDateUtil.getSystemTime;
        {
            var withBlock = clsUtiwakeInfoCreditCompanyEntity;
            sqlString.AppendLine(" MERGE INTO T_UTIWAKE_INFO_CREDIT_COMPANY ");
            sqlString.AppendLine(" USING DUAL ON (T_UTIWAKE_INFO_CREDIT_COMPANY.COMPANY_CD = " + setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), withBlock.CompanyCd.DBType, withBlock.CompanyCd.IntegerBu, withBlock.CompanyCd.DecimalBu));
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.EIGYOSYO_CD = " + setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), withBlock.EigyosyoCd.DBType, withBlock.EigyosyoCd.IntegerBu, withBlock.EigyosyoCd.DecimalBu));
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.CREATE_DAY = " + setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), withBlock.CreateDay.DBType, withBlock.CreateDay.IntegerBu, withBlock.CreateDay.DecimalBu));
            sqlString.AppendLine(" 　　　　　AND  T_UTIWAKE_INFO_CREDIT_COMPANY.CREDIT_COMPANY_CD = " + setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), withBlock.CreditCompanyCd.DBType, withBlock.CreditCompanyCd.IntegerBu, withBlock.CreditCompanyCd.DecimalBu) + ") ");
            sqlString.AppendLine(" WHEN MATCHED THEN ");
            sqlString.AppendLine("   UPDATE SET ");
            sqlString.AppendLine("     URIAGE_KENSU = " + setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), withBlock.UriageKensu.DBType, withBlock.UriageKensu.IntegerBu, withBlock.UriageKensu.DecimalBu));
            sqlString.AppendLine("    ,URIAGE_KINGAKU = " + setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), withBlock.UriageKingaku.DBType, withBlock.UriageKingaku.IntegerBu, withBlock.UriageKingaku.DecimalBu));
            sqlString.AppendLine("    ,RETURN_KENSU = " + setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), withBlock.ReturnKensu.DBType, withBlock.ReturnKensu.IntegerBu, withBlock.ReturnKensu.DecimalBu));
            sqlString.AppendLine("    ,RETURN_KINGAKU = " + setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), withBlock.UriageKingaku.DBType, withBlock.UriageKingaku.IntegerBu, withBlock.UriageKingaku.DecimalBu));
            sqlString.AppendLine("    ,UPDATE_PERSON_CD = " + setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), withBlock.UpdatePersonCd.DBType, withBlock.UpdatePersonCd.IntegerBu, withBlock.UpdatePersonCd.DecimalBu));
            sqlString.AppendLine("    ,UPDATE_PGMID = " + setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), withBlock.UpdatePgmid.DBType, withBlock.UpdatePgmid.IntegerBu, withBlock.UpdatePgmid.DecimalBu));
            sqlString.AppendLine("    ,UPDATE_DAY = " + setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), withBlock.UpdateDay.DBType, withBlock.UpdateDay.IntegerBu, withBlock.UpdateDay.DecimalBu));
            sqlString.AppendLine("    ,UPDATE_TIME = " + setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), withBlock.UpdateTime.DBType, withBlock.UpdateTime.IntegerBu, withBlock.UpdateTime.DecimalBu));
            sqlString.AppendLine("    ,SYSTEM_UPDATE_DAY = SYSDATE ");
            sqlString.AppendLine("    ,SYSTEM_UPDATE_PGMID = " + setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), withBlock.SystemUpdatePgmid.DBType, withBlock.SystemUpdatePgmid.IntegerBu, withBlock.SystemUpdatePgmid.DecimalBu));
            sqlString.AppendLine("    ,SYSTEM_UPDATE_PERSON_CD = " + setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), withBlock.SystemUpdatePersonCd.DBType, withBlock.SystemUpdatePersonCd.IntegerBu, withBlock.SystemUpdatePersonCd.DecimalBu));
            sqlString.AppendLine(" WHEN NOT MATCHED THEN ");
            sqlString.AppendLine("   INSERT( ");
            sqlString.AppendLine("      COMPANY_CD ");
            sqlString.AppendLine("     ,EIGYOSYO_CD ");
            sqlString.AppendLine("     ,CREATE_DAY ");
            sqlString.AppendLine("     ,CREDIT_COMPANY_CD ");
            sqlString.AppendLine("     ,URIAGE_KENSU ");
            sqlString.AppendLine("     ,URIAGE_KINGAKU ");
            sqlString.AppendLine("     ,RETURN_KENSU ");
            sqlString.AppendLine("     ,RETURN_KINGAKU ");
            sqlString.AppendLine("     ,ENTRY_PERSON_CD ");
            sqlString.AppendLine("     ,ENTRY_PGMID ");
            sqlString.AppendLine("     ,ENTRY_DAY ");
            sqlString.AppendLine("     ,ENTRY_TIME ");
            sqlString.AppendLine("     ,UPDATE_PERSON_CD ");
            sqlString.AppendLine("     ,UPDATE_PGMID ");
            sqlString.AppendLine("     ,UPDATE_DAY ");
            sqlString.AppendLine("     ,UPDATE_TIME ");
            sqlString.AppendLine("     ,SYSTEM_ENTRY_DAY ");
            sqlString.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
            sqlString.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
            sqlString.AppendLine("     ,SYSTEM_UPDATE_DAY ");
            sqlString.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
            sqlString.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD) ");
            sqlString.AppendLine("   VALUES( ");
            sqlString.AppendLine("      " + setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), withBlock.CompanyCd.DBType, withBlock.CompanyCd.IntegerBu, withBlock.CompanyCd.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), withBlock.EigyosyoCd.DBType, withBlock.EigyosyoCd.IntegerBu, withBlock.EigyosyoCd.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), withBlock.CreateDay.DBType, withBlock.CreateDay.IntegerBu, withBlock.CreateDay.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), withBlock.CreditCompanyCd.DBType, withBlock.CreditCompanyCd.IntegerBu, withBlock.CreditCompanyCd.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), withBlock.UriageKensu.DBType, withBlock.UriageKensu.IntegerBu, withBlock.UriageKensu.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), withBlock.UriageKingaku.DBType, withBlock.UriageKingaku.IntegerBu, withBlock.UriageKingaku.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), withBlock.ReturnKensu.DBType, withBlock.ReturnKensu.IntegerBu, withBlock.ReturnKensu.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), withBlock.UriageKingaku.DBType, withBlock.UriageKingaku.IntegerBu, withBlock.UriageKingaku.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), withBlock.UpdatePersonCd.DBType, withBlock.UpdatePersonCd.IntegerBu, withBlock.UpdatePersonCd.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), withBlock.UpdatePgmid.DBType, withBlock.UpdatePgmid.IntegerBu, withBlock.UpdatePgmid.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), withBlock.UpdateDay.DBType, withBlock.UpdateDay.IntegerBu, withBlock.UpdateDay.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), withBlock.UpdateTime.DBType, withBlock.UpdateTime.IntegerBu, withBlock.UpdateTime.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), withBlock.UpdatePersonCd.DBType, withBlock.UpdatePersonCd.IntegerBu, withBlock.UpdatePersonCd.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), withBlock.UpdatePgmid.DBType, withBlock.UpdatePgmid.IntegerBu, withBlock.UpdatePgmid.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updateDay, sysDate.ToString(CommonFormatType.dateFormatyyyyMMdd), withBlock.UpdateDay.DBType, withBlock.UpdateDay.IntegerBu, withBlock.UpdateDay.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.updateTime, sysDate.ToString("HHmmss"), withBlock.UpdateTime.DBType, withBlock.UpdateTime.IntegerBu, withBlock.UpdateTime.DecimalBu));
            sqlString.AppendLine("     ,SYSDATE ");
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), withBlock.SystemUpdatePgmid.DBType, withBlock.SystemUpdatePgmid.IntegerBu, withBlock.SystemUpdatePgmid.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), withBlock.SystemUpdatePersonCd.DBType, withBlock.SystemUpdatePersonCd.IntegerBu, withBlock.SystemUpdatePersonCd.DecimalBu));
            sqlString.AppendLine("     ,SYSDATE ");
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), withBlock.SystemUpdatePgmid.DBType, withBlock.SystemUpdatePgmid.IntegerBu, withBlock.SystemUpdatePgmid.DecimalBu));
            sqlString.AppendLine("     ," + setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), withBlock.SystemUpdatePersonCd.DBType, withBlock.SystemUpdatePersonCd.IntegerBu, withBlock.SystemUpdatePersonCd.DecimalBu) + " )");

            // 'UPDATE
            // sqlString.AppendLine(" UPDATE T_UTIWAKE_INFO_CREDIT_COMPANY ")
            // sqlString.AppendLine(" SET ")
            // sqlString.AppendLine(" URIAGE_KENSU = " & setParam(S05_0501.EditedDataTableColName.uriageKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKensu), .UriageKensu.DBType, .UriageKensu.IntegerBu, .UriageKensu.DecimalBu))
            // sqlString.AppendLine(",URIAGE_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.uriageKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.uriageKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            // sqlString.AppendLine(",RETURN_KENSU = " & setParam(S05_0501.EditedDataTableColName.returnKensu, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKensu), .ReturnKensu.DBType, .ReturnKensu.IntegerBu, .ReturnKensu.DecimalBu))
            // sqlString.AppendLine(",RETURN_KINGAKU = " & setParam(S05_0501.EditedDataTableColName.returnKingaku, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.returnKingaku), .UriageKingaku.DBType, .UriageKingaku.IntegerBu, .UriageKingaku.DecimalBu))
            // sqlString.AppendLine(",UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.updatePersonCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePersonCd), .UpdatePersonCd.DBType, .UpdatePersonCd.IntegerBu, .UpdatePersonCd.DecimalBu))
            // sqlString.AppendLine(",UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.updatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.updatePgmid), .UpdatePgmid.DBType, .UpdatePgmid.IntegerBu, .UpdatePgmid.DecimalBu))
            // sqlString.AppendLine(",UPDATE_DAY = TO_NUMBER(TO_CHAR(SYSDATE,'" & CommonFormatType.dateFormatyyyyMMdd & "')) ")
            // sqlString.AppendLine(",UPDATE_TIME = TO_NUMBER(TO_CHAR(SYSDATE,'HH24MISS')) ")
            // sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(S05_0501.EditedDataTableColName.systemUpdateDay, CommonDateUtil.getSystemTime, OracleDbType.Date))
            // sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePgmid, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePgmid), .SystemUpdatePgmid.DBType, .SystemUpdatePgmid.IntegerBu, .SystemUpdatePgmid.DecimalBu))
            // sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(S05_0501.EditedDataTableColName.systemUpdatePerson_cd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.systemUpdatePerson_cd), .SystemUpdatePersonCd.DBType, .SystemUpdatePersonCd.IntegerBu, .SystemUpdatePersonCd.DecimalBu))
            // 'WHERE句
            // sqlString.AppendLine(" WHERE ")
            // sqlString.AppendLine(" COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.companyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.companyCd), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            // sqlString.AppendLine(" AND ")
            // sqlString.AppendLine(" EIGYOSYO_CD = " & setParam(S05_0501.EditedDataTableColName.eigyosyoCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.eigyosyoCd), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            // sqlString.AppendLine(" AND ")
            // sqlString.AppendLine(" CREATE_DAY = " & setParam(S05_0501.EditedDataTableColName.createDay, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.createDay), .CreateDay.DBType, .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            // sqlString.AppendLine(" AND ")
            // sqlString.AppendLine(" CREDIT_COMPANY_CD = " & setParam(S05_0501.EditedDataTableColName.creditCompanyCd, updateInfoList.Rows(targetRowIndex).Item(S05_0501.EditedDataTableColName.creditCompanyCd), .CreditCompanyCd.DBType, .CreditCompanyCd.IntegerBu, .CreditCompanyCd.DecimalBu))

            return sqlString.ToString();
        }
    }
    #endregion

}