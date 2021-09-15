using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 手仕舞い情報登録のDAクラス
/// </summary>
public partial class S03_0411DA : DataAccessorBase
{

    #region  定数／変数 
    private int ParamNum = 0;
    #endregion

    #region SELECT処理 
    /// <summary>
    /// 検索処理を呼び出す（宛先情報）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectTCeaControl(TCeaDAParam param)
    {
        var controlEtt = new TCeaControlEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("     CRS_CD");                                                        // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD");                                            // 仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME");                                               // 仕入先名
        sb.AppendLine("     , TEL1 ");                                                         // TEL1
        sb.AppendLine("     , TEL1_1 ");                                                       // TEL1_1
        sb.AppendLine("     , TEL1_2 ");                                                       // TEL1_2
        sb.AppendLine("     , TEL1_3 ");                                                       // TEL1_3
        sb.AppendLine("     , FAX1 ");                                                         // FAX1
        sb.AppendLine("     , FAX1_1 ");                                                       // FAX1_1
        sb.AppendLine("     , FAX1_2 ");                                                       // FAX1_2
        sb.AppendLine("     , FAX1_3 ");                                                       // FAX1_3
        sb.AppendLine("     , FAX2 ");                                                         // FAX2
        sb.AppendLine("     , FAX2_1 ");                                                       // FAX2_1
        sb.AppendLine("     , FAX2_2 ");                                                       // FAX2_2
        sb.AppendLine("     , FAX2_3 ");                                                       // FAX2_3
        sb.AppendLine("     , MAIL ");                                                         // メールアドレス
        sb.AppendLine("     , SEND_KIND ");                                                    // 通知方法
        sb.AppendLine("     , SEND_YMDT ");                                                    // 最終送信日時
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ");                                            // システム更新日
        // FROM句
        sb.AppendLine("FROM T_CEA_CONTROL");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, controlEtt.CrsCd));
        }
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo));
        }
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, controlEtt.Daily));
        return base.getDataTable(sb.ToString());
    }
    /// <summary>
    /// 検索処理を呼び出す (手仕舞い連絡表　他コース)
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectTCeaOthercrs(TCeaDAParam param)
    {
        var othercrsEtt = new TCeaOthercrsEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("     CRS_CD");                                                        // コースコード
        sb.AppendLine("     , DAILY ");                                                                  // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , OTHER_CRS_CD");                                                  // 他コースコード
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ");                                            // システム更新日
        // FROM句
        sb.AppendLine("FROM T_CEA_OTHERCRS");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, othercrsEtt.CrsCd));
        }
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo));
        }
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, othercrsEtt.Daily));
        return base.getDataTable(sb.ToString());
    }
    /// <summary>
    /// 検索処理を呼び出す （コース情報）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectTCeaCrsInfo(TCeaDAParam param)
    {
        var crsInfoEtt = new TCeaCrsInfoEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("      CRS_CD");                                                        // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , CRS_INFO_NO");                                                   // コース情報№
        sb.AppendLine("     , CRS_INFO");                                                      // コース情報
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ");                                            // システム更新日
        // FROM句
        sb.AppendLine("FROM T_CEA_CRS_INFO");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, crsInfoEtt.CrsCd));
        }
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo));
        }
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, crsInfoEtt.Daily));
        // ソート条件（Order By）
        sb.AppendLine("  ORDER BY CRS_INFO_NO");
        return base.getDataTable(sb.ToString());
    }
    /// <summary>
    /// 検索処理を呼び出す（通信欄）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectTCeaReaders(TCeaDAParam param)
    {
        var readersEtt = new TCeaReadersEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("     CRS_CD");                                                        // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , READERS_COL_NO");                                                // 通信欄№
        sb.AppendLine("     , READERS_COL");                                                   // 通信欄
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ");                                            // システム更新日
        // FROM句
        sb.AppendLine("FROM T_CEA_READERS");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, readersEtt.CrsCd));
        }
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo));
        }
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, readersEtt.Daily));
        // ソート条件（Order By）
        sb.AppendLine("  ORDER BY READERS_COL_NO");
        return base.getDataTable(sb.ToString());
    }

    /// <summary>
    /// 検索処理を呼び出す（特記事項）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectTCeaNoteworthy(TCeaDAParam param)
    {
        var noteworthyEtt = new TCeaNoteworthyEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("     CRS_CD");                                                        // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , NOTEWORTHY_NO");                                                 // 特記事項№
        sb.AppendLine("     , NOTEWORTHY");                                                    // 特記事項
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ");                                            // システム更新日
        // FROM句
        sb.AppendLine("FROM T_CEA_NOTEWORTHY");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, noteworthyEtt.CrsCd));
        }
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, noteworthyEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, noteworthyEtt.SiireSakiNo));
        }
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, noteworthyEtt.Daily));
        // ソート条件（Order By）
        sb.AppendLine("  ORDER BY NOTEWORTHY_NO");
        return base.getDataTable(sb.ToString());
    }
    /// <summary>
    /// 検索処理を呼び出す（手仕舞い表にデータがない場合）
    /// </summary>
    /// <param name="param"></param>
    /// <returns>DataTable</returns>
    public DataTable selectMSiireSaki(TCeaDAParam param)
    {
        var mSiireSakiEtt = new MSiireSakiEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("     SIIRE_SAKI_CD");                                                   // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD");                                            // 仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME");                                               // 仕入先名
        sb.AppendLine("     , TELNO_1 ");                                                      // 電話番号１
        sb.AppendLine("     , TELNO_1_1 ");                                                    // 電話番号１－１
        sb.AppendLine("     , TELNO_1_2 ");                                                    // 電話番号１－２
        sb.AppendLine("     , TELNO_1_3 ");                                                    // 電話番号１－３
        sb.AppendLine("     , FAX_1 ");                                                        // FAX1
        sb.AppendLine("     , FAX_1_1 ");                                                      // FAX1_1
        sb.AppendLine("     , FAX_1_2 ");                                                      // FAX1_2
        sb.AppendLine("     , FAX_1_3 ");                                                      // FAX1_3
        sb.AppendLine("     , FAX_2 ");                                                        // FAX2
        sb.AppendLine("     , FAX_2_1 ");                                                      // FAX2_1
        sb.AppendLine("     , FAX_2_2 ");                                                      // FAX2_2
        sb.AppendLine("     , FAX_2_3 ");                                                      // FAX2_3
        sb.AppendLine("     , MAIL ");                                                         // メールアドレス
        // FROM句
        sb.AppendLine("FROM M_SIIRE_SAKI");
        // WHERE
        sb.AppendLine("WHERE 1 = 1 ");
        // 仕入先コード
        if (param.SiireSakiCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, mSiireSakiEtt.SiireSakiCd));
        }
        // 仕入先枝番
        if (param.SiireSakiNo is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, mSiireSakiEtt.SiireSakiNo));
        }

        sb.AppendLine(" AND DELETE_DATE IS NULL");
        return base.getDataTable(sb.ToString());
    }

    // ' <summary>
    // ' 検索処理を呼び出す コース台帳（基本）
    // ' </summary>
    // ' <param name="param"></param>
    // ' <returns>DataTable</returns>
    public DataTable selectLedgerBasic(TCeaLedgerBasicParam param)
    {
        var ledgerBasicEtt = new TCrsLedgerBasicEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        // SELECT句
        sb.AppendLine("   SELECT ");
        sb.AppendLine("   CRS_CD ");
        // FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC");
        // WHERE
        sb.AppendLine(" WHERE 1 = 1 ");
        // コースコード
        if (param.CrsCd is object)
        {
            sb.AppendLine("  AND ");
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, ledgerBasicEtt.crsCd));
        }
        // 出発日
        sb.AppendLine("  AND ");
        sb.AppendLine("  SYUPT_DAY = ").Append(setDataParam(param.SyuptDay, ledgerBasicEtt.syuptDay));
        return base.getDataTable(sb.ToString());
    }


    #endregion

    #region DELETE/INSERT 処理 
    /// <summary>
    /// 削除/登録 処理
    /// </summary>
    /// <returns>Boolean</returns>
    public int executeInsertDelete(TCeaDAParam paramDA, TCeaControlParam paramTCeaControl, List<TCeaOthercrsParam> listParamTCeaOthercrs, List<TCeaCrsInfoParam> listParamTCeaCrsInfo, List<TCeaReadersParam> listParamTCeaReaders, List<TCeaNoteworthyParam> listParamTCeaNoteworthy)
    {
        OracleTransaction oraTran = default;
        // SQL文字列
        try
        {
            // トランザクション開始
            oraTran = callBeginTransaction();
            // 実行
            // 削除（手仕舞い連絡表）
            string sqlStringDeteleTCeaControl = executeDeleteTCeaControl(paramDA);
            execNonQuery(oraTran, sqlStringDeteleTCeaControl);
            // 登録（手仕舞い連絡表）
            string sqlStringTCeaControl = executeInsertTCeaControl(paramTCeaControl);
            execNonQuery(oraTran, sqlStringTCeaControl);
            // ※他コース情報のコースコードに入力がない場合はDA定義No8をスキップする
            if (listParamTCeaOthercrs.Count != 0)
            {
                // 実行
                // 削除（手仕舞い連絡表 他コース）
                string sqlStringDeteleTCeaOthercrs = executeDeleteTCeaOthercrs(paramDA);
                execNonQuery(oraTran, sqlStringDeteleTCeaOthercrs);
                foreach (TCeaOthercrsParam paramTCeaOthercrs in listParamTCeaOthercrs)
                {
                    // 登録（手仕舞い連絡表　他コース）
                    string sqlStringTCeaOthercrs = executeInsertTCeaOthercrs(paramTCeaOthercrs);
                    execNonQuery(oraTran, sqlStringTCeaOthercrs);
                }
            }
            // 実行
            // 削除（手仕舞い連絡表 コース情報）
            string sqlStringDeteleTCeaCrsInfo = executeDeleteTCeaCrsInfo(paramDA);
            execNonQuery(oraTran, sqlStringDeteleTCeaCrsInfo);
            foreach (TCeaCrsInfoParam paramTCeaCrsInfo in listParamTCeaCrsInfo)
            {
                // 登録（手仕舞い連絡表　コース情報）
                string sqlStringTCeaCrsInfo = executeInsertTCeaCrsInfo(paramTCeaCrsInfo);
                execNonQuery(oraTran, sqlStringTCeaCrsInfo);
            }

            // 実行
            // 削除（手仕舞い連絡表 通信欄）
            string sqlStringDeteleTCeaReaders = executeDeleteTCeaReaders(paramDA);
            execNonQuery(oraTran, sqlStringDeteleTCeaReaders);
            foreach (TCeaReadersParam paramTCeaReaders in listParamTCeaReaders)
            {
                // 登録（手仕舞い連絡表　通信欄）
                string sqlStringTCeaReaders = executeInsertTCeaReaders(paramTCeaReaders);
                execNonQuery(oraTran, sqlStringTCeaReaders);
            }
            // 実行
            // 削除（手仕舞い連絡表 特記事項）
            string sqlStringDeteleTCeaNoteworthy = executeDeleteTCeaNoteworthy(paramDA);
            execNonQuery(oraTran, sqlStringDeteleTCeaNoteworthy);
            foreach (TCeaNoteworthyParam paramTCeaNoteworthy in listParamTCeaNoteworthy)
            {
                // 登録（手仕舞い連絡表　特記事項）
                string sqlStringTCeaNoteworthy = executeInsertTCeaNoteworthy(paramTCeaNoteworthy);
                execNonQuery(oraTran, sqlStringTCeaNoteworthy);
            }
            // コミット
            callCommitTransaction(oraTran);
            // リターンreturnValue
            return 1;
        }
        catch (OracleException ex)
        {
            // 失敗時
            callRollbackTransaction(oraTran);
            // リターンreturnValue
            return 0;
        }
        catch (Exception ex)
        {
            // 失敗時
            callRollbackTransaction(oraTran);
            // リターンreturnValue
            return 0;
        }
    }
    #endregion

    #region DELETE処理 
    /// <summary>
    /// データ削除用 （手仕舞い連絡表）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeDeleteTCeaControl(TCeaDAParam param)
    {
        var controlEtt = new TCeaControlEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_CEA_CONTROL ");
        // WHERE
        sb.AppendLine("WHERE ");
        // コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, controlEtt.CrsCd));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd));
        // 仕入先枝番
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo));
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, controlEtt.Daily));
        return sb.ToString();
    }

    /// <summary>
    /// データ削除用 （手仕舞い連絡表 他コース）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeDeleteTCeaOthercrs(TCeaDAParam param)
    {
        var othercrsEtt = new TCeaOthercrsEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_CEA_OTHERCRS ");
        // WHERE
        sb.AppendLine("WHERE ");
        // コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, othercrsEtt.CrsCd));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd));
        // 仕入先枝番
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo));
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, othercrsEtt.Daily));
        return sb.ToString();
    }

    /// <summary>
    /// データ削除用 （手仕舞い連絡表 コース情報）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeDeleteTCeaCrsInfo(TCeaDAParam param)
    {
        var crsInfoEtt = new TCeaCrsInfoEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_CEA_CRS_INFO ");
        // WHERE
        sb.AppendLine("WHERE ");
        // コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, crsInfoEtt.CrsCd));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd));
        // 仕入先枝番
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo));
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, crsInfoEtt.Daily));
        return sb.ToString();
    }

    /// <summary>
    /// データ削除用 （手仕舞い連絡表 通信欄）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeDeleteTCeaReaders(TCeaDAParam param)
    {
        var readersEtt = new TCeaReadersEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_CEA_READERS ");
        // WHERE
        sb.AppendLine("WHERE");
        // コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, readersEtt.CrsCd));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd));
        // 仕入先枝番
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo));
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, readersEtt.Daily));
        return sb.ToString();
    }

    /// <summary>
    /// データ削除用 （手仕舞い連絡表 特記事項）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeDeleteTCeaNoteworthy(TCeaDAParam param)
    {
        var noteworthy = new TCeaNoteworthyEntity();
        // SQL文字列
        var sb = new StringBuilder();
        // パラメータクリア
        clear();
        sb.AppendLine("DELETE FROM T_CEA_NOTEWORTHY ");
        // WHERE
        sb.AppendLine("WHERE");
        // コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, noteworthy.CrsCd));
        // 仕入先コード
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, noteworthy.SiireSakiCd));
        // 仕入先枝番
        sb.AppendLine("  AND ");
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, noteworthy.SiireSakiNo));
        // 日次
        sb.AppendLine("  AND ");
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, noteworthy.Daily));
        return sb.ToString();
    }
    #endregion

    #region INSERT処理 
    /// <summary>
    /// データ登録用 （手仕舞い連絡表）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeInsertTCeaControl(TCeaControlParam param)
    {
        var controlEtt = new TCeaControlEntity();
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO T_CEA_CONTROL");
        sb.AppendLine("            (CRS_CD");                                                  // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD");                                            // 仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME");                                               // 仕入先名
        sb.AppendLine("     , TEL1 ");                                                         // TEL1
        sb.AppendLine("     , TEL1_1 ");                                                       // TEL1_1
        sb.AppendLine("     , TEL1_2 ");                                                       // TEL1_2
        sb.AppendLine("     , TEL1_3 ");                                                       // TEL1_3
        sb.AppendLine("     , FAX1 ");                                                         // FAX1
        sb.AppendLine("     , FAX1_1 ");                                                       // FAX1_1
        sb.AppendLine("     , FAX1_2 ");                                                       // FAX1_2
        sb.AppendLine("     , FAX1_3 ");                                                       // FAX1_3
        sb.AppendLine("     , FAX2 ");                                                         // FAX2
        sb.AppendLine("     , FAX2_1 ");                                                       // FAX2_1
        sb.AppendLine("     , FAX2_2 ");                                                       // FAX2_2
        sb.AppendLine("     , FAX2_3 ");                                                       // FAX2_3
        sb.AppendLine("     , MAIL ");                                                         // メールアドレス
        sb.AppendLine("     , SEND_KIND ");                                                    // 通知方法
        sb.AppendLine("     , SEND_YMDT ");                                                    // 最終送信日時
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY)");                                            // システム更新日
        // INSERT 登録
        sb.AppendLine("     VALUES");
        sb.AppendLine("            (" + setDataParam(param.CrsCd, controlEtt.CrsCd));
        sb.AppendLine("            ," + setDataParam(param.Daily, controlEtt.Daily));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiKindCd, controlEtt.SiireSakiKindCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiName, controlEtt.SiireSakiName));
        sb.AppendLine("            ," + setDataParam(param.Tel1, controlEtt.Tel1));
        sb.AppendLine("            ," + setDataParam(param.Tel11, controlEtt.Tel11));
        sb.AppendLine("            ," + setDataParam(param.Tel12, controlEtt.Tel12));
        sb.AppendLine("            ," + setDataParam(param.Tel13, controlEtt.Tel13));
        sb.AppendLine("            ," + setDataParam(param.Fax1, controlEtt.Fax1));
        sb.AppendLine("            ," + setDataParam(param.Fax11, controlEtt.Fax11));
        sb.AppendLine("            ," + setDataParam(param.Fax12, controlEtt.Fax12));
        sb.AppendLine("            ," + setDataParam(param.Fax13, controlEtt.Fax13));
        sb.AppendLine("            ," + setDataParam(param.Fax2, controlEtt.Fax2));
        sb.AppendLine("            ," + setDataParam(param.Fax21, controlEtt.Fax21));
        sb.AppendLine("            ," + setDataParam(param.Fax22, controlEtt.Fax22));
        sb.AppendLine("            ," + setDataParam(param.Fax23, controlEtt.Fax23));
        sb.AppendLine("            ," + setDataParam(param.Mail, controlEtt.Mail));
        sb.AppendLine("            ," + setDataParam(param.SendKind, controlEtt.SendKind));
        sb.AppendLine("            ," + setDataParam(param.SendYmdt, controlEtt.SendYmdt));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPgmid, controlEtt.SystemEntryPgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPersonCd, controlEtt.SystemEntryPersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryDay, controlEtt.SystemEntryDay));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePgmid, controlEtt.SystemUpdatePgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePersonCd, controlEtt.SystemUpdatePersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdateDay, controlEtt.SystemUpdateDay));
        sb.AppendLine("            )");
        return sb.ToString();
    }

    /// <summary>
    /// データ登録用 （手仕舞い連絡表　他コース）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string executeInsertTCeaOthercrs(TCeaOthercrsParam param)
    {
        var othercrsEtt = new TCeaOthercrsEntity();
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO T_CEA_OTHERCRS");
        sb.AppendLine("            (CRS_CD");                                                  // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , OTHER_CRS_CD");                                                  // 他コースコード
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY) ");                                            // システム更新日
        // INSERT 登録
        sb.AppendLine("     VALUES");
        sb.AppendLine("            (" + setDataParam(param.CrsCd, othercrsEtt.CrsCd));
        sb.AppendLine("            ," + setDataParam(param.Daily, othercrsEtt.Daily));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo));
        sb.AppendLine("            ," + setDataParam(param.OtherCrsCd, othercrsEtt.OtherCrsCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPgmid, othercrsEtt.SystemEntryPgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPersonCd, othercrsEtt.SystemEntryPersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryDay, othercrsEtt.SystemEntryDay));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePgmid, othercrsEtt.SystemUpdatePgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePersonCd, othercrsEtt.SystemUpdatePersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdateDay, othercrsEtt.SystemUpdateDay)); // 更新日DATE
        sb.AppendLine("            )");
        return sb.ToString();
    }

    /// <summary>
    /// データ登録用 （手仕舞い連絡表　コース情報）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeInsertTCeaCrsInfo(TCeaCrsInfoParam param)
    {
        var crsInfoEtt = new TCeaCrsInfoEntity();
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO T_CEA_CRS_INFO");
        sb.AppendLine("            (CRS_CD");                                                  // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , CRS_INFO_NO");                                                   // コース情報№
        sb.AppendLine("     , CRS_INFO");                                                      // コース情報
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )");                                            // システム更新日
        // INSERT 登録
        sb.AppendLine("     VALUES");
        sb.AppendLine("            (" + setDataParam(param.CrsCd, crsInfoEtt.CrsCd));
        sb.AppendLine("            ," + setDataParam(param.Daily, crsInfoEtt.Daily));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo));
        sb.AppendLine("            ," + setDataParam(param.CrsInfoNo, crsInfoEtt.CrsInfoNo));
        sb.AppendLine("            ," + setDataParam(param.CrsInfo, crsInfoEtt.CrsInfo));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPgmid, crsInfoEtt.SystemEntryPgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPersonCd, crsInfoEtt.SystemEntryPersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryDay, crsInfoEtt.SystemEntryDay));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePgmid, crsInfoEtt.SystemUpdatePgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePersonCd, crsInfoEtt.SystemUpdatePersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdateDay, crsInfoEtt.SystemUpdateDay)); // 更新日DATE
        sb.AppendLine("            )");
        return sb.ToString();
    }

    /// <summary>
    /// データ登録用 （手仕舞い連絡表　通信欄）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeInsertTCeaReaders(TCeaReadersParam param)
    {
        var readersEtt = new TCeaReadersEntity();
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO T_CEA_READERS");
        sb.AppendLine("            (CRS_CD");                                                  // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , READERS_COL_NO");                                                // 通信欄№
        sb.AppendLine("     , READERS_COL");                                                   // 通信欄
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )");                                           // システム更新日
        // INSERT 登録
        sb.AppendLine("     VALUES");
        sb.AppendLine("            (" + setDataParam(param.CrsCd, readersEtt.CrsCd));
        sb.AppendLine("            ," + setDataParam(param.Daily, readersEtt.Daily));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo));
        sb.AppendLine("            ," + setDataParam(param.ReadersColNo, readersEtt.ReadersColNo));
        sb.AppendLine("            ," + setDataParam(param.ReadersCol, readersEtt.ReadersCol));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPgmid, readersEtt.SystemEntryPgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPersonCd, readersEtt.SystemEntryPersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryDay, readersEtt.SystemEntryDay));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePgmid, readersEtt.SystemUpdatePgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePersonCd, readersEtt.SystemUpdatePersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdateDay, readersEtt.SystemUpdateDay)); // 更新日DATE
        sb.AppendLine("            )");
        return sb.ToString();
    }

    /// <summary>
    /// データ登録用 （手仕舞い連絡表　特記事項）
    /// </summary>
    /// <param name="param">SQL引数</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string executeInsertTCeaNoteworthy(TCeaNoteworthyParam param)
    {
        var noteworthyEtt = new TCeaNoteworthyEntity();
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO T_CEA_NOTEWORTHY");
        sb.AppendLine("            (CRS_CD");                                                  // コースコード
        sb.AppendLine("     , DAILY ");                                                        // 日次
        sb.AppendLine("     , SIIRE_SAKI_CD");                                                 // 仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO");                                                 // 仕入先枝番
        sb.AppendLine("     , NOTEWORTHY_NO");                                                 // 特記事項№
        sb.AppendLine("     , NOTEWORTHY");                                                    // 特記事項
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ");                                           // システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ");                                       // システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ");                                             // システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ");                                          // システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ");                                      // システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )");                                            // システム更新日
        // INSERT 登録
        sb.AppendLine("     VALUES");
        sb.AppendLine("            (" + setDataParam(param.CrsCd, noteworthyEtt.CrsCd));
        sb.AppendLine("            ," + setDataParam(param.Daily, noteworthyEtt.Daily));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiCd, noteworthyEtt.SiireSakiCd));
        sb.AppendLine("            ," + setDataParam(param.SiireSakiNo, noteworthyEtt.SiireSakiNo));
        sb.AppendLine("            ," + setDataParam(param.NoteworthyNo, noteworthyEtt.NoteworthyNo));
        sb.AppendLine("            ," + setDataParam(param.Noteworthy, noteworthyEtt.Noteworthy));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPgmid, noteworthyEtt.SystemEntryPgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryPersonCd, noteworthyEtt.SystemEntryPersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemEntryDay, noteworthyEtt.SystemEntryDay));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePgmid, noteworthyEtt.SystemUpdatePgmid));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdatePersonCd, noteworthyEtt.SystemUpdatePersonCd));
        sb.AppendLine("            ," + setDataParam(param.SystemUpdateDay, noteworthyEtt.SystemUpdateDay)); // 更新日DATE
        sb.AppendLine("            )");
        return sb.ToString();
    }

    public string setDataParam(object value, IEntityKoumokuType ent)
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

    public partial class TCeaDAParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
    }

    public partial class TCeaControlParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 仕入先種別
        /// </summary>
        public string SiireSakiKindCd { get; set; }
        /// <summary>
        /// 仕入先名
        /// </summary>
        public string SiireSakiName { get; set; }
        /// <summary>
        /// TEL1
        /// </summary>
        public string Tel1 { get; set; }
        /// <summary>
        /// TEL1_1
        /// </summary>
        public string Tel11 { get; set; }
        /// <summary>
        /// TEL1_2
        /// </summary>
        public string Tel12 { get; set; }
        /// <summary>
        /// TEL1_3
        /// </summary>
        public string Tel13 { get; set; }
        /// <summary>
        /// FAX1
        /// </summary>
        public string Fax1 { get; set; }
        /// <summary>
        /// FAX1_1
        /// </summary>
        public string Fax11 { get; set; }
        /// <summary>
        /// FAX1_2
        /// </summary>
        public string Fax12 { get; set; }
        /// <summary>
        /// FAX1_3
        /// </summary>
        public string Fax13 { get; set; }
        /// <summary>
        /// FAX2
        /// </summary>
        public string Fax2 { get; set; }
        /// <summary>
        /// FAX2_1
        /// </summary>
        public string Fax21 { get; set; }
        /// <summary>
        /// FAX2_2
        /// </summary>
        public string Fax22 { get; set; }
        /// <summary>
        /// FAX2_3
        /// </summary>
        public string Fax23 { get; set; }
        /// <summary>
        /// メールアドレス
        /// </summary>
        public string Mail { get; set; }
        /// <summary>
        /// 通知方法
        /// </summary>
        public string SendKind { get; set; }
        /// <summary>
        /// 最終送信日時
        /// </summary>
        public DateTime? SendYmdt { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
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

    public partial class TCeaOthercrsParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 他コースコード
        /// </summary>
        public string OtherCrsCd { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
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

    public partial class TCeaCrsInfoParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// コース情報№
        /// </summary>
        public int CrsInfoNo { get; set; }
        /// <summary>
        /// コース情報
        /// </summary>
        public string CrsInfo { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
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

    public partial class TCeaReadersParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 通信欄№
        /// </summary>
        public int ReadersColNo { get; set; }
        /// <summary>
        /// 通信欄
        /// </summary>
        public string ReadersCol { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
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

    public partial class TCeaNoteworthyParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }
        /// <summary>
        /// 日次
        /// </summary>
        public int Daily { get; set; }
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string SiireSakiCd { get; set; }
        /// <summary>
        /// 仕入先枝番
        /// </summary>
        public string SiireSakiNo { get; set; }
        /// <summary>
        /// 特記事項№
        /// </summary>
        public int NoteworthyNo { get; set; }
        /// <summary>
        /// 特記事項
        /// </summary>
        public string Noteworthy { get; set; }
        /// <summary>
        /// システム登録ＰＧＭＩＤ
        /// </summary>
        public string SystemEntryPgmid { get; set; }
        /// <summary>
        /// システム登録者コード
        /// </summary>
        public string SystemEntryPersonCd { get; set; }
        /// <summary>
        /// システム登録日
        /// </summary>
        public DateTime SystemEntryDay { get; set; }
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

    public partial class TCeaLedgerBasicParam
    {
        /// <summary>
        /// コースコード
        /// </summary>
        public string CrsCd { get; set; }

        /// <summary>
        /// 出発日
        /// </summary>
        public string SyuptDay { get; set; }
    }
    #endregion

}