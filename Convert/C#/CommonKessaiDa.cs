using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

/// <summary>
/// 
/// </summary>
public partial class CommonKessaiDa : DataAccessorBase
{

    #region メソッド

    /// <summary>
    /// 予約情報（基本）取得
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>予約情報（基本）</returns>
    public DataTable getYoyakuInfoBasic(string yoyakuKbn, int yoyakuNo)
    {
        DataTable yoyakuInfo;
        try
        {
            string query = createYoyakuInfoBasicSql(yoyakuKbn, yoyakuNo);
            yoyakuInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuInfo;
    }

    /// <summary>
    /// 予約情報（基本）取得SQL作成
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>予約情報（基本）取得SQL</returns>
    private string createYoyakuInfoBasicSql(string yoyakuKbn, int yoyakuNo)
    {
        var entity = new YoyakuInfoBasicEntity();
        entity.yoyakuKbn.Value = yoyakuKbn;
        entity.yoyakuNo.Value = yoyakuNo;
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT * FROM T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（決済）取得
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>予約情報（決済）</returns>
    public DataTable getYoyakuInfoKessai(string yoyakuKbn, int yoyakuNo)
    {
        DataTable kessaiInfo;
        try
        {
            string query = createYoyakuInfoKessaiSql(yoyakuKbn, yoyakuNo);
            kessaiInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return kessaiInfo;
    }

    /// <summary>
    /// 予約情報（決済）取得SQL作成
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>予約情報（決済）取得SQL</returns>
    private string createYoyakuInfoKessaiSql(string yoyakuKbn, int yoyakuNo)
    {
        var entity = new TYoyakuInfoSettlementEntity();
        entity.YoyakuKbn.Value = yoyakuKbn;
        entity.YoyakuNo.Value = yoyakuNo;
        entity.SettlementProcessResultCd.Value = "0";
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT * FROM T_YOYAKU_INFO_SETTLEMENT ");
        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO = " + this.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo));
        sb.AppendLine(" AND SETTLEMENT_PROCESS_RESULT_CD = " + this.prepareParam(entity.SettlementProcessResultCd.PhysicsName, entity.SettlementProcessResultCd.Value, entity.SettlementProcessResultCd));
        sb.AppendLine(" AND NVL(GMO_OSORI_CANCEL_DAY, 0) = 0 ");
        sb.AppendLine(" AND NVL(DELETE_DAY, 0) = 0 ");
        return sb.ToString();
    }

    /// <summary>
    /// 前回決済金額取得
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>前回決済金額</returns>
    public int getBfKessaiAmount(string yoyakuKbn, int yoyakuNo)
    {
        int amount = 0;
        try
        {
            string query = createBfKessaiAmount(yoyakuKbn, yoyakuNo);
            DataTable bfAmountTable = base.getDataTable(query);
            if (bfAmountTable.Rows.Count > 0)
            {
                amount = int.Parse(bfAmountTable.Rows(0)("KINGAKU").ToString());
            }
            else
            {
                amount = 0;
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        return amount;
    }

    /// <summary>
    /// 前回決済金額取得SQL作成
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>前回決済金額取得SQL</returns>
    private string createBfKessaiAmount(string yoyakuKbn, int yoyakuNo)
    {
        var entity = new TNyuukinInfoEntity();
        entity.YoyakuKbn.Value = yoyakuKbn;
        entity.YoyakuNo.Value = yoyakuNo;
        entity.SettlementModuleKbn.Value = "G";
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT  ");
        sb.AppendLine("      YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        sb.AppendLine("     ,SUM(NYUKINGAKU) - SUM(HENKINGAKU) AS KINGAKU ");
        sb.AppendLine(" FROM ( ");
        sb.AppendLine("     SELECT ");
        sb.AppendLine("         YOYAKU_KBN ");
        sb.AppendLine("         ,YOYAKU_NO ");
        sb.AppendLine("         ,SETTLEMENT_MODULE_KBN ");
        sb.AppendLine("         ,HAKKEN_HURIKOMI_KBN ");
        sb.AppendLine("         ,CASE HAKKEN_HURIKOMI_KBN WHEN '7' THEN SUM(NYUUKIN_GAKU_2) ELSE 0 END AS NYUKINGAKU ");
        sb.AppendLine("         ,CASE HAKKEN_HURIKOMI_KBN WHEN '8' THEN SUM(NYUUKIN_GAKU_2) ELSE 0 END AS HENKINGAKU ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("         T_NYUUKIN_INFO ");
        sb.AppendLine("     WHERE ");
        sb.AppendLine("         YOYAKU_KBN = " + this.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn));
        sb.AppendLine("         AND ");
        sb.AppendLine("         YOYAKU_NO = " + this.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo));
        sb.AppendLine("         AND ");
        sb.AppendLine("         SETTLEMENT_MODULE_KBN = " + this.prepareParam(entity.SettlementModuleKbn.PhysicsName, entity.SettlementModuleKbn.Value, entity.SettlementModuleKbn));
        sb.AppendLine("         AND ");
        sb.AppendLine("         HAKKEN_HURIKOMI_KBN IN('7', '8') ");
        sb.AppendLine("     GROUP BY  ");
        sb.AppendLine("          YOYAKU_KBN ");
        sb.AppendLine("         ,YOYAKU_NO ");
        sb.AppendLine("         ,SETTLEMENT_MODULE_KBN ");
        sb.AppendLine("         ,HAKKEN_HURIKOMI_KBN) NYUHENKIN ");
        sb.AppendLine(" GROUP BY ");
        sb.AppendLine("      YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（決済）情報登録
    /// </summary>
    /// <param name="entity">予約情報（決済）</param>
    /// <returns>登録結果</returns>
    public bool registYoyakuInfoKessai(TYoyakuInfoSettlementEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            string query = createInsertSql<TYoyakuInfoSettlementEntity>(entity, "T_YOYAKU_INFO_SETTLEMENT");
            updateCount = base.execNonQuery(oracleTransaction, query);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return false;
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

        return true;
    }

    /// <summary>
    /// 最大値SEQ取得
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>最大値SEQ</returns>
    public int getNyuhenkinSeq(string yoyakuKbn, int yoyakuNo)
    {
        int nyukinSeq = 0;
        try
        {
            string query = createNyuhenkinSeqSql(yoyakuKbn, yoyakuNo);
            DataTable seqDataTable = base.getDataTable(query);
            nyukinSeq = int.Parse(seqDataTable.Rows(0)("SEQ").ToString());
        }
        catch (Exception ex)
        {
            throw;
        }

        return nyukinSeq;
    }

    /// <summary>
    /// 最大値SEQ取得SQL作成
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <returns>最大値SEQ取得SQL</returns>
    private string createNyuhenkinSeqSql(string yoyakuKbn, int yoyakuNo)
    {
        base.paramClear();
        var entity = new TNyuukinInfoEntity();
        entity.YoyakuKbn.Value = yoyakuKbn;
        entity.YoyakuNo.Value = yoyakuNo;
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT (NVL(MAX(SEQ), 0) + 1) As SEQ FROM T_NYUUKIN_INFO ");
        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO = " + this.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（決済）更新
    /// </summary>
    /// <param name="entity">予約情報（決済）</param>
    /// <returns>更新結果</returns>
    public bool updateYoyakuInfoKessai(TYoyakuInfoSettlementEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            string query = createYoyakuInfoKessaiUpdateSql(entity);
            updateCount = base.execNonQuery(oracleTransaction, query);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return false;
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

        return true;
    }

    /// <summary>
    /// 予約情報（決済）更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報（決済）</param>
    /// <returns>予約情報（決済）更新SQL</returns>
    private string createYoyakuInfoKessaiUpdateSql(TYoyakuInfoSettlementEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_SETTLEMENT SET ");
        sb.AppendLine("  GMO_OSORI_CANCEL_DAY = " + this.prepareParam(entity.GmoOsoriCancelDay.PhysicsName, entity.GmoOsoriCancelDay.Value, entity.GmoOsoriCancelDay));
        sb.AppendLine(" ,GMO_OSORI_CANCEL_TIME = " + this.prepareParam(entity.GmoOsoriCancelTime.PhysicsName, entity.GmoOsoriCancelTime.Value, entity.GmoOsoriCancelTime));
        sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.UpdateDay.PhysicsName, entity.UpdateDay.Value, entity.UpdateDay));
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.UpdatePersonCd.PhysicsName, entity.UpdatePersonCd.Value, entity.UpdatePersonCd));
        sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.UpdatePgmid.PhysicsName, entity.UpdatePgmid.Value, entity.UpdatePgmid));
        sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.UpdateTime.PhysicsName, entity.UpdateTime.Value, entity.UpdateTime));
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.SystemUpdatePgmid.PhysicsName, entity.SystemUpdatePgmid.Value, entity.SystemUpdatePgmid));
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.SystemUpdatePersonCd.PhysicsName, entity.SystemUpdatePersonCd.Value, entity.SystemUpdatePersonCd));
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.SystemUpdateDay.PhysicsName, entity.SystemUpdateDay.Value, entity.SystemUpdateDay));
        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO =  " + this.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo));
        sb.AppendLine(" AND ORDER_ID =  " + this.prepareParam(entity.OrderId.PhysicsName, entity.OrderId.Value, entity.OrderId));
        return sb.ToString();
    }

    /// <summary>
    /// 予約関連登録
    /// </summary>
    /// <param name="registEntity">登録用Entity</param>
    /// <param name="registDate">登録日付</param>
    /// <param name="formId">画面ID</param>
    /// <param name="torikeshiFlg">取消フラグ</param>
    /// <returns>更新ステータス</returns>
    public string registYoyakuInfo(KessaiRegistEntity registEntity, DateTime registDate, string formId, bool torikeshiFlg)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 入返金情報
            string nyukinQuery = this.createInsertSql<TNyuukinInfoEntity>(registEntity.NyuukinInfoEntity, "T_NYUUKIN_INFO");
            updateCount = base.execNonQuery(oracleTransaction, nyukinQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonKessai.RegistKessaiStatusNyukinFailure;
            }

            // 変更履歴情報
            OutputYoyakuChangeHistoryParam historyRec = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, default);
            if (historyRec is null)
            {
                return CommonKessai.RegistKessaiStatusHistoryFailure;
            }

            if (registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value == historyRec.changeHistoryLastSeq)
            {
                // 変更履歴が登録されていない場合T_YOYAKU_INFO_SETTLEMENT

                string basicQuery = "";
                if (torikeshiFlg == true)
                {
                    // 取消の場合

                    basicQuery = this.createYoyakuInfoBasicUpdateSqlForTorikeshi(registEntity.YoyakuInfoBasicEntity, true);
                    updateCount = base.execNonQuery(oracleTransaction, basicQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure;
                    }
                }
                else
                {
                    // 売上の場合

                    basicQuery = this.createYoyakuInfoBasicUpdateSqlForUriage(registEntity.YoyakuInfoBasicEntity, false);
                    updateCount = base.execNonQuery(oracleTransaction, basicQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure;
                    }
                }
            }
            else
            {
                // 変更履歴が登録されている場合

                // 予約情報（基本）
                registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay;
                registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq;
                registEntity.YoyakuInfoBasicEntity.updateDay.Value = Conversions.ToInteger(registDate.ToString("yyyyMMdd"));
                registEntity.YoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId;
                registEntity.YoyakuInfoBasicEntity.updatePgmid.Value = formId;
                registEntity.YoyakuInfoBasicEntity.updateTime.Value = Conversions.ToInteger(registDate.ToString("HHmmss"));
                registEntity.YoyakuInfoBasicEntity.systemUpdateDay.Value = registDate;
                registEntity.YoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId;
                registEntity.YoyakuInfoBasicEntity.systemUpdatePgmid.Value = formId;

                // 予約情報（基本）
                string basicQuery = "";
                if (torikeshiFlg == true)
                {
                    // 取消の場合

                    basicQuery = this.createYoyakuInfoBasicUpdateSqlForTorikeshi(registEntity.YoyakuInfoBasicEntity, false);
                    updateCount = base.execNonQuery(oracleTransaction, basicQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure;
                    }
                }
                else
                {
                    // 売上の場合

                    basicQuery = this.createYoyakuInfoBasicUpdateSqlForUriage(registEntity.YoyakuInfoBasicEntity, true);
                    updateCount = base.execNonQuery(oracleTransaction, basicQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure;
                    }
                }

                // 予約情報２
                string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
                DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
                string yoyakuInfo2Query;
                if (yoyakuInfo2.Rows.Count > 0)
                {
                    yoyakuInfo2Query = this.createYoyakuInfo2UpdateSql(registEntity.YoyakuInfo2Entity);
                }
                else
                {
                    yoyakuInfo2Query = this.createInsertSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2");
                }

                updateCount = base.execNonQuery(oracleTransaction, yoyakuInfo2Query);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonKessai.RegistKessaiStatusYoyakuInfo2Failure;
                }

                // 予約情報（コース料金）
                string yoyakuCrsChargeQuery = this.createYoyakuInfoCrsChargeUpdateSql(registEntity.YoyakuInfoCrsChargeEntity);
                updateCount = base.execNonQuery(oracleTransaction, yoyakuCrsChargeQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeFailure;
                }

                // 予約情報（コース料金_料金区分）
                string yoyakuCrsChargeChargeKbnQuery = this.createYoyakuInfoCrsChargeChargeKbnUpdateSql(registEntity.YoyakuInfoCrsChargeChargeKbnEntity);
                updateCount = base.execNonQuery(oracleTransaction, yoyakuCrsChargeChargeKbnQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeChargeKbnFailure;
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

        return CommonKessai.RegistKessaiStatusSucess;
    }

    /// <summary>
    /// 予約情報（基本）更新SQL作成
    /// 売上用
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <param name="isUpdateFlg">更新フラグ</param>
    /// <returns>予約情報（基本）更新SQL</returns>
    private string createYoyakuInfoBasicUpdateSqlForUriage(YoyakuInfoBasicEntity entity, bool isUpdateFlg)
    {
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC SET ");
        sb.AppendLine("  SEISAN_HOHO = " + this.prepareParam(entity.seisanHoho.PhysicsName, entity.seisanHoho.Value, entity.seisanHoho));
        sb.AppendLine(" ,NYUKINGAKU_SOKEI = " + this.prepareParam(entity.nyukingakuSokei.PhysicsName, entity.nyukingakuSokei.Value, entity.nyukingakuSokei));
        sb.AppendLine(" ,LAST_NYUUKIN_DAY = " + this.prepareParam(entity.lastNyuukinDay.PhysicsName, entity.lastNyuukinDay.Value, entity.lastNyuukinDay));
        sb.AppendLine(" ,FURIKOMIYOSHI_YOHI_FLG = " + this.prepareParam(entity.furikomiyoshiYohiFlg.PhysicsName, entity.furikomiyoshiYohiFlg.Value, entity.furikomiyoshiYohiFlg));
        sb.AppendLine(" ,NYUUKIN_SITUATION_KBN = " + this.prepareParam(entity.nyuukinSituationKbn.PhysicsName, entity.nyuukinSituationKbn.Value, entity.nyuukinSituationKbn));
        if (isUpdateFlg == true)
        {
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_DAY = " + this.prepareParam(entity.changeHistoryLastDay.PhysicsName, entity.changeHistoryLastDay.Value, entity.changeHistoryLastDay));
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_SEQ = " + this.prepareParam(entity.changeHistoryLastSeq.PhysicsName, entity.changeHistoryLastSeq.Value, entity.changeHistoryLastSeq));
            sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
            sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
            sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
            sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
            sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
            sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
            sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        }

        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO =  " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（基本）更新SQL作成
    /// 取消用
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <param name="isUpdateFlg">更新フラグ</param>
    /// <returns>予約情報（基本）更新SQL</returns>
    private string createYoyakuInfoBasicUpdateSqlForTorikeshi(YoyakuInfoBasicEntity entity, bool isUpdateFlg)
    {
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC SET ");
        sb.AppendLine("  NYUKINGAKU_SOKEI = " + this.prepareParam(entity.nyukingakuSokei.PhysicsName, entity.nyukingakuSokei.Value, entity.nyukingakuSokei));
        sb.AppendLine(" ,LAST_HENKIN_DAY = " + this.prepareParam(entity.lastHenkinDay.PhysicsName, entity.lastHenkinDay.Value, entity.lastHenkinDay));
        sb.AppendLine(" ,NYUUKIN_SITUATION_KBN = " + this.prepareParam(entity.nyuukinSituationKbn.PhysicsName, entity.nyuukinSituationKbn.Value, entity.nyuukinSituationKbn));
        if (isUpdateFlg == true)
        {
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_DAY = " + this.prepareParam(entity.changeHistoryLastDay.PhysicsName, entity.changeHistoryLastDay.Value, entity.changeHistoryLastDay));
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_SEQ = " + this.prepareParam(entity.changeHistoryLastSeq.PhysicsName, entity.changeHistoryLastSeq.Value, entity.changeHistoryLastSeq));
            sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
            sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
            sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
            sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
            sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
            sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
            sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        }

        sb.AppendLine(" WHERE YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine(" AND YOYAKU_NO =  " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }


    /// <summary>
    /// 予約情報２取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報２Entity</param>
    /// <returns>予約情報２取得SQL</returns>
    private string createYoyakuInfo2Sql(YoyakuInfo2Entity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      DELETE_DAY ");
        sb.AppendLine("     ,ENTRY_DAY ");
        sb.AppendLine("     ,ENTRY_PERSON_CD ");
        sb.AppendLine("     ,ENTRY_PGMID ");
        sb.AppendLine("     ,ENTRY_TIME ");
        sb.AppendLine("     ,OUT_DAY ");
        sb.AppendLine("     ,OUT_PERSON_CD ");
        sb.AppendLine("     ,OUT_PGMID ");
        sb.AppendLine("     ,OUT_TIME ");
        sb.AppendLine("     ,UPDATE_DAY ");
        sb.AppendLine("     ,UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UPDATE_PGMID ");
        sb.AppendLine("     ,UPDATE_TIME ");
        sb.AppendLine("     ,WARIBIKI_JIDO_CREATE_FLG ");
        sb.AppendLine("     ,YEAR ");
        sb.AppendLine("     ,YOBI_DAY_1 ");
        sb.AppendLine("     ,YOBI_DAY_2 ");
        sb.AppendLine("     ,YOBI_DAY_3 ");
        sb.AppendLine("     ,YOBI_FLG_1 ");
        sb.AppendLine("     ,YOBI_FLG_10 ");
        sb.AppendLine("     ,YOBI_FLG_2 ");
        sb.AppendLine("     ,YOBI_FLG_3 ");
        sb.AppendLine("     ,YOBI_FLG_4 ");
        sb.AppendLine("     ,YOBI_FLG_5 ");
        sb.AppendLine("     ,YOBI_FLG_6 ");
        sb.AppendLine("     ,YOBI_FLG_7 ");
        sb.AppendLine("     ,YOBI_FLG_8 ");
        sb.AppendLine("     ,YOBI_FLG_9 ");
        sb.AppendLine("     ,YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_2  ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報２更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報２Entity</param>
    /// <returns>予約情報２更新SQL</returns>
    private string createYoyakuInfo2UpdateSql(YoyakuInfo2Entity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_2 SET ");
        sb.AppendLine("  OUT_DAY = " + this.prepareParam(entity.outDay.PhysicsName, entity.outDay.Value, entity.outDay));
        sb.AppendLine(" ,OUT_PERSON_CD = " + this.prepareParam(entity.outPersonCd.PhysicsName, entity.outPersonCd.Value, entity.outPersonCd));
        sb.AppendLine(" ,OUT_PGMID = " + this.prepareParam(entity.outPgmid.PhysicsName, entity.outPgmid.Value, entity.outPgmid));
        sb.AppendLine(" ,OUT_TIME = " + this.prepareParam(entity.outTime.PhysicsName, entity.outTime.Value, entity.outTime));
        sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
        sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
        sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（コース料金）更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報（コース料金）Entity</param>
    /// <returns>予約情報（コース料金）更新SQL</returns>
    private string createYoyakuInfoCrsChargeUpdateSql(YoyakuInfoCrsChargeEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_CRS_CHARGE SET ");
        sb.AppendLine("  CANCEL_RYOU = " + this.prepareParam(entity.cancelRyou.PhysicsName, entity.cancelRyou.Value, entity.cancelRyou));
        sb.AppendLine(" ,CANCEL_PER = " + this.prepareParam(entity.cancelPer.PhysicsName, entity.cancelPer.Value, entity.cancelPer));
        sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
        sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
        sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報（コース料金_料金区分）更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報（コース料金_料金区分）Entity</param>
    /// <returns>予約情報（コース料金_料金区分）更新SQL</returns>
    private string createYoyakuInfoCrsChargeChargeKbnUpdateSql(YoyakuInfoCrsChargeChargeKbnEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN SET ");
        sb.AppendLine("  CANCEL_NINZU_1 = " + this.prepareParam(entity.cancelNinzu1.PhysicsName, entity.cancelNinzu1.Value, entity.cancelNinzu1));
        sb.AppendLine(" ,CANCEL_NINZU_2 = " + this.prepareParam(entity.cancelNinzu2.PhysicsName, entity.cancelNinzu2.Value, entity.cancelNinzu2));
        sb.AppendLine(" ,CANCEL_NINZU_3 = " + this.prepareParam(entity.cancelNinzu3.PhysicsName, entity.cancelNinzu3.Value, entity.cancelNinzu3));
        sb.AppendLine(" ,CANCEL_NINZU_4 = " + this.prepareParam(entity.cancelNinzu4.PhysicsName, entity.cancelNinzu4.Value, entity.cancelNinzu4));
        sb.AppendLine(" ,CANCEL_NINZU_5 = " + this.prepareParam(entity.cancelNinzu5.PhysicsName, entity.cancelNinzu5.Value, entity.cancelNinzu5));
        sb.AppendLine(" ,CANCEL_NINZU = " + this.prepareParam(entity.cancelNinzu.PhysicsName, entity.cancelNinzu.Value, entity.cancelNinzu));
        sb.AppendLine(" ,UPDATE_DAY = " + this.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay));
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + this.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd));
        sb.AppendLine(" ,UPDATE_PGMID = " + this.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid));
        sb.AppendLine(" ,UPDATE_TIME = " + this.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime));
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + this.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid));
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + this.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd));
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + this.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + this.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + this.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo));
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