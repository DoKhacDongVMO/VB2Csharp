using Hatobus.ReservationManagementSystem.Zaseki;

/// <summary>
/// 予約登録DA
/// </summary>
public partial class S02_0103Da : DataAccessorBase
{

    #region 定数/変数

    /// <summary>
    /// 使用中チェックリトライ回数：100
    /// </summary>
    private const int UsingCheckRetryNum = 100;
	#endregion
	
	#region メソッド
	public DataTable getCodeMasterData(string codeBunrui, bool nullRecord = false)
    {
        base.paramClear();
        DataTable codeMasterData = default;
        try
        {
            string query = this.createCodeMasterSql(codeBunrui, nullRecord);
            codeMasterData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return codeMasterData;
    }

    /// <summary>
    /// コース情報取得
    /// </summary>
    /// <param name="entity">パラメータ</param>
    /// <returns>コース情報</returns>
    public DataTable getCrsLedgerBasicData(CrsLedgerBasicEntity entity)
    {
        DataTable crsInfo = default;
        try
        {
            // コース台帳(基本)SQL文作成
            string query = this.createCrsLedgerBasicSql(entity);
            crsInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return crsInfo;
    }

    /// <summary>
    /// コース台帳(基本)の使用中フラグ取得
    /// </summary>
    /// <param name="paramInfoList">パラメータ群</param>
    /// <returns>コース台帳(基本)の使用中フラグ</returns>
    public DataTable getCrsLedgerBasicUsingData(Hashtable paramInfoList)
    {
        DataTable usingData = default;
        try
        {
            string query = this.createUsingFlagSql(paramInfoList);
            usingData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return usingData;
    }

    /// <summary>
    /// コース台帳座席更新処理
    /// 定期用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">コース台帳座席更新データ</param>
    /// <param name="registEntity">登録予約情報Entity</param>
    /// <returns>座席変更ステータス</returns>
    public string updateZasekiInfoForTeiki(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 座席情報取得
            DataTable crsLedgerZasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                int kusekiTeisaki = 0;
                int kusekiSubSeat = 0;
                zasekiUpdateQuery = this.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {
                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 仮予約情報登録
            string yoyakuInfoDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_BASIC");
            base.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery);
            string yoyakuInfoQuery = this.createInsertSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }
	public string updateTorikeshiZasekiInfoForTeiki(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 座席情報取得
            DataTable crsLedgerZasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                int kusekiTeisaki = 0;
                int kusekiSubSeat = 0;
                zasekiUpdateQuery = this.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {
                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 仮予約情報論削
            string yoyakuInfoQuery = this.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// コース台帳座席更新処理
    /// 企画(日帰り)用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">コース台帳座席更新データ</param>
    /// <param name="registEntity">登録予約情報Entity</param>
    /// <returns></returns>
    public string updateZasekiInfoForKikakuHigaeri(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 仮予約情報登録
            string yoyakuInfoDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_BASIC");
            base.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery);
            string yoyakuInfoQuery = this.createInsertSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// コース台帳取消座席更新処理
    /// 企画(日帰り)用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">コース台帳座席更新データ</param>
    /// <param name="registEntity">登録予約情報Entity</param>
    /// <returns></returns>
    public string updateTorikeshiZasekiInfoForKikakuHigaeri(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 仮予約情報論削
            string yoyakuInfoQuery = this.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// コース台帳座席更新処理
    /// 企画(宿泊)用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">コース台帳座席更新データ</param>
    /// <param name="registEntity">登録予約情報Entity</param>
    /// <param name="crsInfoBasicEntity">コース台帳（Entity）</param>
    /// <returns>更新ステータス</returns>
    public string updateZasekiInfoForKikakuShukuhaku(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity, CrsLedgerBasicEntity crsInfoBasicEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 部屋数更新
            string roomZansuUpdateQuery = this.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, false);
            if (string.IsNullOrEmpty(roomZansuUpdateQuery) == false)
            {
                updateCount = base.execNonQuery(oracleTransaction, roomZansuUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 仮予約情報登録
            string yoyakuInfoDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_BASIC");
            base.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery);
            string yoyakuInfoQuery = this.createInsertSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }
	public string updateTorikeshiZasekiInfoForKikakuShukuhaku(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity, CrsLedgerBasicEntity crsInfoBasicEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 部屋数更新
            string roomZansuUpdateQuery = this.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, true);
            updateCount = base.execNonQuery(oracleTransaction, roomZansuUpdateQuery);
            if (updateCount <= 0)
            {
                // 座席データの更新件数が0件以下の場合、処理終了

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
            }

            // ROOM別人数1～5を初期化
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu1.Value = CommonRegistYoyaku.ZERO;
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu2.Value = CommonRegistYoyaku.ZERO;
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu3.Value = CommonRegistYoyaku.ZERO;
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu4.Value = CommonRegistYoyaku.ZERO;
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu5.Value = CommonRegistYoyaku.ZERO;

            // 仮予約情報論削
            string yoyakuInfoQuery = this.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// 空席情報取得
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>空席情報</returns>
    public DataTable getCrsLedgerBasicKusekiNumData(CrsLedgerBasicEntity entity)
    {
        DataTable kusekiData = default;
        try
        {
            string query = this.createCrsLedgerBasicKusekiNumDataSql(entity);
            kusekiData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return kusekiData;
    }

    /// <summary>
    /// 料金区分一覧取得
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>料金区分一覧</returns>
    public DataTable getChargeKbnList(CrsLedgerChargeEntity entity)
    {
        DataTable chargeKbnList;
        try
        {
            string query = this.createChargeKbnListSql(entity);
            chargeKbnList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return chargeKbnList;
    }

    /// <summary>
    /// コース台帳（コース情報）取得
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>コース台帳（コース情報）</returns>
    public DataTable getCrsInfo(CrsLedgerCrsInfoEntity entity)
    {
        DataTable crsInfo = default;
        try
        {
            string query = this.createCrsInfoSql(entity);
            crsInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return crsInfo;
    }

    /// <summary>
    /// メッセージ情報取得
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>メッセージ情報</returns>
    public DataTable getCrsInfoMessage(CrsLedgerMessageEntity entity)
    {
        DataTable messageData = default;
        try
        {
            string query = this.createCrsInfoMessageSql(entity);
            messageData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return messageData;
    }

    /// <summary>
    /// 予約区分、予約番号取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約区分、予約番号</returns>
    public DataTable getYoyakuIno(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuData = default;
        try
        {
            string query = this.createYoyakuInfoSql(entity);
            yoyakuData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuData;
    }

    /// <summary>
    /// 割引マスタ情報取得
    /// </summary>
    /// <param name="entity">割引マスタ</param>
    /// <returns>割引マスタ情報</returns>
    public DataTable getWaribikiMasterData(WaribikiCdMasterEntity entity)
    {
        DataTable waribikiData = default;
        try
        {
            string query = this.createWaribikiMasterDataSql(entity);
            waribikiData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return waribikiData;
    }

    /// <summary>
    /// オプション必須選択一覧取得
    /// </summary>
    /// <param name="entity">コース台帳（オプショングループ）Entity</param>
    /// <returns>オプション必須選択一覧</returns>
    public DataTable getRequiredSelectionList(CrsLedgerOptionGroupEntity entity)
    {
        base.paramClear();
        DataTable requiredSelectionList = default;
        try
        {
            string query = this.createRequiredSelectionListSql(entity);
            requiredSelectionList = this.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return requiredSelectionList;
    }

    /// <summary>
    /// オプション任意選択一覧取得
    /// </summary>
    /// <param name="entity">コース台帳（オプショングループ）Entity</param>
    /// <returns>オプション任意選択一覧</returns>
    public DataTable getAnySelectionList(CrsLedgerOptionGroupEntity entity)
    {
        base.paramClear();
        DataTable anySelectionList = default;
        try
        {
            string query = this.createAnySelectionListSql(entity);
            anySelectionList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return anySelectionList;
    }

    /// <summary>
    /// 送付物マスタ一覧取得
    /// </summary>
    /// <returns>送付物マスタ一覧</returns>
    public DataTable getSoufubutsuList()
    {
        DataTable soufubutsuList = default;
        try
        {
            string query = this.createSoufubutsuListSql();
            soufubutsuList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return soufubutsuList;
    }

    /// <summary>
    /// 予約情報登録
    /// </summary>
    /// <param name="registEntity">登録用Entity</param>
    /// <returns>処理結果</returns>
    public string registYoyakuInfo(RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 予約情報（基本）更新
            string basicQuery = this.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            updateCount = base.execNonQuery(oracleTransaction, basicQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
            }

            // 予約情報（コース料金）追加
            string crsChargeQuery = this.createInsertSql<YoyakuInfoCrsChargeEntity>(registEntity.YoyakuInfoCrsChargeEntity, "T_YOYAKU_INFO_CRS_CHARGE");
            updateCount = base.execNonQuery(oracleTransaction, crsChargeQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure;
            }

            // 予約情報（コース料金_料金区分）
            string chargeKbnQuery = "";
            foreach (YoyakuInfoCrsChargeChargeKbnEntity entity in registEntity.YoyakuInfoCrsChargeChargeKbnList)
            {
                chargeKbnQuery = this.createInsertSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
                updateCount = base.execNonQuery(oracleTransaction, chargeKbnQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure;
                }
            }

            // 予約情報（ピックアップ）
            if (registEntity.YoyakuInfoPickupList is object)
            {
                string pickupDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_PICKUP");
                base.execNonQuery(oracleTransaction, pickupDeleteQuery);
                string pickupInsertQuery = "";
                foreach (YoyakuInfoPickupEntity entity in registEntity.YoyakuInfoPickupList)
                {
                    pickupInsertQuery = this.createInsertSql<YoyakuInfoPickupEntity>(entity, "T_YOYAKU_INFO_PICKUP");
                    updateCount = base.execNonQuery(oracleTransaction, pickupInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuPickupInfoFailure;
                    }
                }
            }

            // ピックアップルート台帳 （ホテル）
            if (registEntity.PickupRouteLedgerHotelList is object)
            {
                foreach (PickupRouteLedgerHotelEntity entity in registEntity.PickupRouteLedgerHotelList)
                {

                    // ピックアップルート台帳 （ホテル）人数更新SQL作成
                    string pickupHotelQuery = this.createPickupRouteLedgerHotelUpdateSql(entity);
                    updateCount = base.execNonQuery(oracleTransaction, pickupHotelQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure;
                    }
                }
            }

            // 予約情報（割引）
            if (registEntity.YoyakuInfoWaribikiList is object)
            {
                string waribikiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_WARIBIKI");
                base.execNonQuery(oracleTransaction, waribikiDeleteQuery);
                string waribikiInsertQuery = "";
                foreach (YoyakuInfoWaribikiEntity entity in registEntity.YoyakuInfoWaribikiList)
                {
                    waribikiInsertQuery = this.createInsertSql<YoyakuInfoWaribikiEntity>(entity, "T_YOYAKU_INFO_WARIBIKI");
                    updateCount = base.execNonQuery(oracleTransaction, waribikiInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuWaribikiUpdateFailure;
                    }
                }
            }

            // 予約情報（振込）
            if (registEntity.YoyakuInfoHurikomiEntity is object)
            {
                string hurikomiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_HURIKOMI");
                base.execNonQuery(oracleTransaction, hurikomiDeleteQuery);
                var hurikomiInsertQuery = this.createInsertSql<YoyakuInfoHurikomiEntity>(registEntity.YoyakuInfoHurikomiEntity, "T_YOYAKU_INFO_HURIKOMI");
                updateCount = base.execNonQuery(oracleTransaction, hurikomiInsertQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuFurikomiUpdateFailure;
                }
            }

            // 予約情報（名簿）
            if (registEntity.YoyakuInfoMeiboList is object)
            {
                string meiboDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_MEIBO");
                base.execNonQuery(oracleTransaction, meiboDeleteQuery);
                string meiboInsertQuery = "";
                foreach (YoyakuInfoMeiboEntity entity in registEntity.YoyakuInfoMeiboList)
                {
                    meiboInsertQuery = this.createInsertSql<YoyakuInfoMeiboEntity>(entity, "T_YOYAKU_INFO_MEIBO");
                    updateCount = base.execNonQuery(oracleTransaction, meiboInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuMeiboUpdateFailure;
                    }
                }
            }

            // 予約情報（送付物）
            if (registEntity.YoyakuInfoSofubutsuList is object)
            {
                string sofubutsuDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_SOFUBUTSU");
                base.execNonQuery(oracleTransaction, sofubutsuDeleteQuery);
                string sofubutsuInsertQuery = "";
                foreach (YoyakuInfoSofubutsuEntity entity in registEntity.YoyakuInfoSofubutsuList)
                {
                    sofubutsuInsertQuery = this.createInsertSql<YoyakuInfoSofubutsuEntity>(entity, "T_YOYAKU_INFO_SOFUBUTSU");
                    updateCount = base.execNonQuery(oracleTransaction, sofubutsuInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuSofubutsuUpdateFailure;
                    }
                }
            }

            // 予約情報（オプション）
            if (registEntity.YoyakuInfoOptionList is object)
            {
                string optionDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_OPTION");
                base.execNonQuery(oracleTransaction, optionDeleteQuery);
                string optionInsertQuery = "";
                foreach (YoyakuInfoOptionEntity entity in registEntity.YoyakuInfoOptionList)
                {
                    optionInsertQuery = this.createInsertSql<YoyakuInfoOptionEntity>(entity, "T_YOYAKU_INFO_OPTION");
                    updateCount = base.execNonQuery(oracleTransaction, optionInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuOptionUpdateFailure;
                    }
                }
            }

            // 予約情報（送付先）
            if (registEntity.YoyakuInfoSofusakiList is object)
            {
                string sofusakiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_SOFUSAKI");
                base.execNonQuery(oracleTransaction, sofusakiDeleteQuery);
                string sofusakiInsertQuery = "";
                foreach (YoyakuInfoSofusakiEntity entity in registEntity.YoyakuInfoSofusakiList)
                {
                    sofusakiInsertQuery = this.createInsertSql<YoyakuInfoSofusakiEntity>(entity, "T_YOYAKU_INFO_SOFUSAKI");
                    updateCount = base.execNonQuery(oracleTransaction, sofusakiInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuSofusakiUpdateFailure;
                    }
                }
            }

            // WT_リクエスト情報更新
            if (registEntity.WtRequestInfoEntity is object)
            {
                string wtRequestUpdateQuery = this.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity);
                updateCount = base.execNonQuery(oracleTransaction, wtRequestUpdateQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure;
                }

                // コース台帳（基本）（WTキャンセル人数更新）
                string wtCancelNinzuQuery = this.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction);
                updateCount = base.execNonQuery(oracleTransaction, wtCancelNinzuQuery);
            }

            if (updateCount == 0)
            {
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約情報更新
    /// </summary>
    /// <param name="registEntity">登録用Entity</param>
    /// <returns>処理結果</returns>
    public string updateYoyakuInfo(RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 変更履歴登録
            OutputYoyakuChangeHistoryParam historyRec = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList);
            if (historyRec is null)
            {
                return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure;
            }

            // 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay;
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq;
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName);
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName);

            // 予約情報（基本）
            string yoyakuInfoQuery = this.createUpdateSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            var yoyakuInfoBuilder = new StringBuilder();
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery);
            yoyakuInfoBuilder.AppendLine("WHERE");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu));
            yoyakuInfoBuilder.AppendLine("    AND ");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
            }

            // 予約情報２
            string yoyakuInfo2Query;
            string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
            DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
            if (yoyakuInfo2.Rows.Count > 0)
            {
                string yoyakuInfo2UpdateQuery = this.createUpdateSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2");
                var yoyakuInfo2Builder = new StringBuilder();
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery);
                yoyakuInfo2Builder.AppendLine("WHERE");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu));
                yoyakuInfo2Builder.AppendLine("    AND ");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu));
                yoyakuInfo2Query = yoyakuInfo2Builder.ToString();
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
                return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure;
            }

            // 予約情報（ピックアップ）
            if (registEntity.YoyakuInfoPickupList is object)
            {
                string pickupDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_PICKUP");
                base.execNonQuery(oracleTransaction, pickupDeleteQuery);
                string pickupInsertQuery = "";
                foreach (YoyakuInfoPickupEntity entity in registEntity.YoyakuInfoPickupList)
                {
                    pickupInsertQuery = this.createInsertSql<YoyakuInfoPickupEntity>(entity, "T_YOYAKU_INFO_PICKUP");
                    updateCount = base.execNonQuery(oracleTransaction, pickupInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuPickupInfoFailure;
                    }
                }
            }

            // ピックアップルート台帳 （ホテル）
            if (registEntity.PickupRouteLedgerHotelList is object)
            {
                foreach (PickupRouteLedgerHotelEntity entity in registEntity.PickupRouteLedgerHotelList)
                {
                    if (entity.ninzu.Value != CommonRegistYoyaku.ZERO) // 2019/06/05 今回人数と前回人数に相違がある場合に更新
                    {
                        // ピックアップルート台帳 （ホテル）人数更新SQL作成
                        string pickupHotelQuery = this.createPickupRouteLedgerHotelUpdateSql(entity);
                        updateCount = base.execNonQuery(oracleTransaction, pickupHotelQuery);
                        if (updateCount <= 0)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure;
                        }
                    }
                }
            }

            // 予約情報（割引）
            if (registEntity.YoyakuInfoWaribikiList is object)
            {
                string waribikiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_WARIBIKI");
                base.execNonQuery(oracleTransaction, waribikiDeleteQuery);
                string waribikiInsertQuery = "";
                foreach (YoyakuInfoWaribikiEntity entity in registEntity.YoyakuInfoWaribikiList)
                {
                    waribikiInsertQuery = this.createInsertSql<YoyakuInfoWaribikiEntity>(entity, "T_YOYAKU_INFO_WARIBIKI");
                    updateCount = base.execNonQuery(oracleTransaction, waribikiInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuWaribikiUpdateFailure;
                    }
                }
            }

            // 予約情報（振込）
            if (registEntity.YoyakuInfoHurikomiEntity is object)
            {
                string hurikomiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_HURIKOMI");
                base.execNonQuery(oracleTransaction, hurikomiDeleteQuery);
                var hurikomiInsertQuery = this.createInsertSql<YoyakuInfoHurikomiEntity>(registEntity.YoyakuInfoHurikomiEntity, "T_YOYAKU_INFO_HURIKOMI");
                updateCount = base.execNonQuery(oracleTransaction, hurikomiInsertQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuFurikomiUpdateFailure;
                }
            }

            // 予約情報（名簿）
            if (registEntity.YoyakuInfoMeiboList is object)
            {
                string meiboDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_MEIBO");
                base.execNonQuery(oracleTransaction, meiboDeleteQuery);
                string meiboInsertQuery = "";
                foreach (YoyakuInfoMeiboEntity entity in registEntity.YoyakuInfoMeiboList)
                {
                    meiboInsertQuery = this.createInsertSql<YoyakuInfoMeiboEntity>(entity, "T_YOYAKU_INFO_MEIBO");
                    updateCount = base.execNonQuery(oracleTransaction, meiboInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuMeiboUpdateFailure;
                    }
                }
            }

            // 予約情報（送付物）
            if (registEntity.YoyakuInfoSofubutsuList is object)
            {
                string sofubutsuDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_SOFUBUTSU");
                base.execNonQuery(oracleTransaction, sofubutsuDeleteQuery);
                string sofubutsuInsertQuery = "";
                foreach (YoyakuInfoSofubutsuEntity entity in registEntity.YoyakuInfoSofubutsuList)
                {
                    sofubutsuInsertQuery = this.createInsertSql<YoyakuInfoSofubutsuEntity>(entity, "T_YOYAKU_INFO_SOFUBUTSU");
                    updateCount = base.execNonQuery(oracleTransaction, sofubutsuInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuSofubutsuUpdateFailure;
                    }
                }
            }

            // 予約情報（オプション）
            if (registEntity.YoyakuInfoOptionList is object)
            {
                string optionDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_OPTION");
                base.execNonQuery(oracleTransaction, optionDeleteQuery);
                string optionInsertQuery = "";
                foreach (YoyakuInfoOptionEntity entity in registEntity.YoyakuInfoOptionList)
                {
                    optionInsertQuery = this.createInsertSql<YoyakuInfoOptionEntity>(entity, "T_YOYAKU_INFO_OPTION");
                    updateCount = base.execNonQuery(oracleTransaction, optionInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuOptionUpdateFailure;
                    }
                }
            }

            // 予約情報（送付先）
            if (registEntity.YoyakuInfoSofusakiList is object)
            {
                string sofusakiDeleteQuery = this.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, "T_YOYAKU_INFO_SOFUSAKI");
                base.execNonQuery(oracleTransaction, sofusakiDeleteQuery);
                string sofusakiInsertQuery = "";
                foreach (YoyakuInfoSofusakiEntity entity in registEntity.YoyakuInfoSofusakiList)
                {
                    sofusakiInsertQuery = this.createInsertSql<YoyakuInfoSofusakiEntity>(entity, "T_YOYAKU_INFO_SOFUSAKI");
                    updateCount = base.execNonQuery(oracleTransaction, sofusakiInsertQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusYoyakuSofusakiUpdateFailure;
                    }
                }
            }

            // WT_リクエスト情報更新
            if (registEntity.WtRequestInfoEntity is object)
            {
                string wtRequestUpdateQuery = this.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity);
                updateCount = base.execNonQuery(oracleTransaction, wtRequestUpdateQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure;
                }

                // コース台帳（基本）（WTキャンセル人数更新）
                string wtCancelNinzuQuery = this.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction);
                updateCount = base.execNonQuery(oracleTransaction, wtCancelNinzuQuery);
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約情報データ取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報データ</returns>
    public DataTable getYoyakuInfoData(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuInfoData;
        try
        {
            string query = this.createYoyakuInfoDataSql(entity);
            yoyakuInfoData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuInfoData;
    }

    /// <summary>
    /// 予約料金区分一覧取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約料金区分一覧</returns>
    public DataTable getYoyakuChargeKbnList(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuChargeKbnList;
        try
        {
            string query = this.createYoyakuChargeKbnListSql(entity);
            yoyakuChargeKbnList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuChargeKbnList;
    }

    /// <summary>
    /// 予約メモ情報一覧取得
    /// </summary>
    /// <param name="entity">予約情報（メモ）Entity</param>
    /// <returns>予約メモ情報一覧</returns>
    public DataTable getYoyakuMemoInfoList(YoyakuInfoMemoEntity entity)
    {
        DataTable memoInfoList;
        try
        {
            string query = this.createYoyakuMemoInfoListSql(entity);
            memoInfoList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return memoInfoList;
    }
	public DataTable getYoyakuWaribikiList(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuWaribikiList;
        try
        {
            string query = this.createYoyakuWaribikiListSql(entity);
            yoyakuWaribikiList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuWaribikiList;
    }

    /// <summary>
    /// 予約振込情報取得
    /// </summary>
    /// <param name="entity">予約情報（振込）Entity</param>
    /// <returns>予約振込情報</returns>
    public DataTable getYoyakuHurikomiInfo(YoyakuInfoHurikomiEntity entity)
    {
        DataTable yoyakuFurikomiInfo;
        try
        {
            string query = this.createYoyakuHurikomiInfoSql(entity);
            yoyakuFurikomiInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuFurikomiInfo;
    }

    /// <summary>
    /// 予約名簿一覧取得
    /// </summary>
    /// <param name="entity">予約情報（名簿）Entity</param>
    /// <returns>予約名簿一覧</returns>
    public DataTable getYoyakuMeiboList(YoyakuInfoMeiboEntity entity)
    {
        DataTable yoyakuMeiboList;
        try
        {
            string query = this.createYoyakuMeiboListSql(entity);
            yoyakuMeiboList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuMeiboList;
    }

    /// <summary>
    /// 予約送付物一覧取得
    /// </summary>
    /// <param name="entity">予約情報（送付物）Entity</param>
    /// <returns>予約送付物一覧</returns>
    public DataTable getYoyakuSofubutsuList(YoyakuInfoSofubutsuEntity entity)
    {
        DataTable yoyakuSofubutsuList;
        try
        {
            string query = this.createYoyakuSofubutsuListSql(entity);
            yoyakuSofubutsuList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuSofubutsuList;
    }

    /// <summary>
    /// 予約送付先一覧取得
    /// </summary>
    /// <param name="entity">予約情報（送付先）Entity</param>
    /// <returns>予約送付先一覧</returns>
    public DataTable getYoyakuSofusakiList(YoyakuInfoSofusakiEntity entity)
    {
        DataTable yoyakuSofusakiList;
        try
        {
            string query = this.createYoyakuSofusakiListSql(entity);
            yoyakuSofusakiList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuSofusakiList;
    }

    /// <summary>
    /// 入返金一覧取得
    /// </summary>
    /// <param name="entity">入返金情報Entity</param>
    /// <returns>入返金一覧</returns>
    public DataTable getNyuhenkinList(NyuukinInfoEntity entity)
    {
        DataTable nyuhenkinList;
        try
        {
            string query = this.createNyuhenkinListSql(entity);
            nyuhenkinList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return nyuhenkinList;
    }

    /// <summary>
    /// 予約オプション一覧取得
    /// </summary>
    /// <param name="crsEntity">コース台帳（オプション）Entity</param>
    /// <param name="yoyakuentity">予約情報（オプション）Entity</param>
    /// <returns>予約オプション一覧</returns>
    public DataTable getYoyakuOptionList(CrsLedgerOptionGroupEntity crsEntity, YoyakuInfoOptionEntity yoyakuentity)
    {
        base.paramClear();
        DataTable yoyakuOptionList;
        try
        {
            string query = this.createYoyakuOptionListSql(crsEntity, yoyakuentity);
            yoyakuOptionList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuOptionList;
    }

    /// <summary>
    /// ホリデーT管理（コースコード）情報取得
    /// </summary>
    /// <param name="entity">ホリデーＴ管理（コースコード）Entity</param>
    /// <returns>ホリデーT管理（コースコード）情報</returns>
    public DataTable getHolidayCrs(HolidayCrsCdEntity entity)
    {
        DataTable holidayCrsInfo;
        try
        {
            string query = this.createHolidayCrsSql(entity);
            holidayCrsInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return holidayCrsInfo;
    }

    /// <summary>
    /// ホリデーT管理（適用日）情報取得
    /// </summary>
    /// <param name="entity">ホリデーＴ管理（適用日）Entity</param>
    /// <returns>ホリデーT管理（適用日）情報</returns>
    public DataTable getHolidayApplicationDay(HolidayApplicationDayEntity entity)
    {
        DataTable holidayApplicationDayInfo;
        try
        {
            string query = this.createHolidayApplicationDaySql(entity);
            holidayApplicationDayInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return holidayApplicationDayInfo;
    }

    /// <summary>
    /// 予約料金区分一覧取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約料金区分一覧</returns>
    public DataTable getYoyakuChargeList(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuChargeList;
        try
        {
            string query = this.createYoyakuChargeListSql(entity);
            yoyakuChargeList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuChargeList;
    }

    /// <summary>
    /// 予約情報使用中フラグ更新
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>更新件数</returns>
    public int updateYoyakuUsingFlag(YoyakuInfoBasicEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            string query = this.createYoyakuUsingFlagUpdateSql(entity);
            updateCount = base.execNonQuery(oracleTransaction, query);

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

        return updateCount;
    }

    /// <summary>
    /// 予約情報使用中フラグ更新
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>更新件数</returns>
    public int updateYoyakuUsingFlagOn(YoyakuInfoBasicEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            string query = this.createYoyakuUsingFlagOnUpdateSql(entity);
            updateCount = base.execNonQuery(oracleTransaction, query);

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

        return updateCount;
    }

    /// <summary>
    /// 予約情報使用中フラグ取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報使用中フラグ</returns>
    public DataTable getYoyakuInfoUsingFlag(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuInfo;
        try
        {
            string query = this.createYoyakuUsingFlagSql(entity);
            yoyakuInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuInfo;
    }

    /// <summary>
    /// 予約座席情報キャンセル
    /// 定期用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">座席情報検索条件群</param>
    /// <param name="registEntity">予約登録データ</param>
    /// <returns>処理結果</returns>
    public string cancelYoyakuInfoForTeiseki(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 座席情報取得
            DataTable crsLedgerZasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                int kusekiTeisaki = 0;
                int kusekiSubSeat = 0;

                // コース台帳座席情報更新SQL作成
                zasekiUpdateQuery = this.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {
                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 変更履歴登録
            OutputYoyakuChangeHistoryParam historyRec = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList);
            if (historyRec is null)
            {
                return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure;
            }

            // 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay;
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq;
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName);
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName);

            // 予約情報（基本）
            string yoyakuInfoQuery = this.createUpdateSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            var yoyakuInfoBuilder = new StringBuilder();
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery);
            yoyakuInfoBuilder.AppendLine("WHERE");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu));
            yoyakuInfoBuilder.AppendLine("    AND ");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
            }

            // 予約情報（コース料金）
            string crsChargeQuery = this.createUpdateSql<YoyakuInfoCrsChargeEntity>(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE");
            var crsChargeBuilder = new StringBuilder();
            crsChargeBuilder.AppendLine(crsChargeQuery);
            crsChargeBuilder.AppendLine("WHERE");
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu));
            crsChargeBuilder.AppendLine("    AND ");
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, crsChargeBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure;
            }

            // 予約情報（コース料金_料金区分）
            string chargeKbnQuery = "";
            foreach (YoyakuInfoCrsChargeChargeKbnEntity entity in registEntity.YoyakuInfoCrsChargeChargeKbnList)
            {
                chargeKbnQuery = this.createUpdateSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
                var chargeKbnBuilder = new StringBuilder();
                chargeKbnBuilder.AppendLine(chargeKbnQuery);
                chargeKbnBuilder.AppendLine("WHERE");
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    KBN_NO = " + base.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + base.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu));
                updateCount = base.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString());
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure;
                }
            }

            // 予約情報２
            string yoyakuInfo2Query;
            string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
            DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
            if (yoyakuInfo2.Rows.Count > 0)
            {
                string yoyakuInfo2UpdateQuery = this.createUpdateSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2");
                var yoyakuInfo2Builder = new StringBuilder();
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery);
                yoyakuInfo2Builder.AppendLine("WHERE");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu));
                yoyakuInfo2Builder.AppendLine("    AND ");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu));
                yoyakuInfo2Query = yoyakuInfo2Builder.ToString();
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
                return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure;
            }

            // ピックアップルート台帳 （ホテル）
            if (registEntity.PickupRouteLedgerHotelList is object)
            {
                foreach (PickupRouteLedgerHotelEntity entity in registEntity.PickupRouteLedgerHotelList)
                {

                    // ピックアップルート台帳 （ホテル）人数更新SQL作成
                    string pickupHotelQuery = this.createPickupRouteLedgerHotelUpdateSql(entity);
                    updateCount = base.execNonQuery(oracleTransaction, pickupHotelQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure;
                    }
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約座席情報キャンセル
    /// 企画（日帰り）用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">座席情報検索条件群</param>
    /// <param name="registEntity">予約登録データ</param>
    /// <returns>処理結果</returns>
    public string cancelYoyakuInfoForKikakuHigaeri(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 変更履歴登録
            OutputYoyakuChangeHistoryParam historyRec = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList);
            if (historyRec is null)
            {
                return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure;
            }

            // 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay;
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq;
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName);
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName);

            // 予約情報（基本）
            string yoyakuInfoQuery = this.createUpdateSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            var yoyakuInfoBuilder = new StringBuilder();
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery);
            yoyakuInfoBuilder.AppendLine("WHERE");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu));
            yoyakuInfoBuilder.AppendLine("    AND ");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
            }

            // 予約情報（コース料金）
            string crsChargeQuery = this.createUpdateSql<YoyakuInfoCrsChargeEntity>(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE");
            var crsChargeBuilder = new StringBuilder();
            crsChargeBuilder.AppendLine(crsChargeQuery);
            crsChargeBuilder.AppendLine("WHERE");
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu));
            crsChargeBuilder.AppendLine("    AND ");
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, crsChargeBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure;
            }

            // 予約情報（コース料金_料金区分）
            string chargeKbnQuery = "";
            foreach (YoyakuInfoCrsChargeChargeKbnEntity entity in registEntity.YoyakuInfoCrsChargeChargeKbnList)
            {
                chargeKbnQuery = this.createUpdateSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
                var chargeKbnBuilder = new StringBuilder();
                chargeKbnBuilder.AppendLine(chargeKbnQuery);
                chargeKbnBuilder.AppendLine("WHERE");
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    KBN_NO = " + base.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + base.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu));
                updateCount = base.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString());
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure;
                }
            }

            // 予約情報２
            string yoyakuInfo2Query;
            string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
            DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
            if (yoyakuInfo2.Rows.Count > 0)
            {
                string yoyakuInfo2UpdateQuery = this.createUpdateSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2");
                var yoyakuInfo2Builder = new StringBuilder();
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery);
                yoyakuInfo2Builder.AppendLine("WHERE");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu));
                yoyakuInfo2Builder.AppendLine("    AND ");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu));
                yoyakuInfo2Query = yoyakuInfo2Builder.ToString();
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
                return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure;
            }

            // ピックアップルート台帳 （ホテル）
            if (registEntity.PickupRouteLedgerHotelList is object)
            {
                foreach (PickupRouteLedgerHotelEntity entity in registEntity.PickupRouteLedgerHotelList)
                {

                    // ピックアップルート台帳 （ホテル）人数更新SQL作成
                    string pickupHotelQuery = this.createPickupRouteLedgerHotelUpdateSql(entity);
                    updateCount = base.execNonQuery(oracleTransaction, pickupHotelQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure;
                    }
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約座席情報キャンセル
    /// 企画（宿泊）用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="crsZasekiData">座席情報検索条件群</param>
    /// <param name="registEntity">予約登録データ</param>
    /// <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    /// <returns>処理結果</returns>
    public string cancelYoyakuInfoForKikakuShukuhaku(Z0001_Result z0001Result, Hashtable crsZasekiData, RegistYoyakuInfoEntity registEntity, CrsLedgerBasicEntity crsInfoBasicEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusUsing;
            }

            string zasekiUpdateQuery = "";
            // コース台帳座席数更新
            if (z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK)
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData);
                if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusKusekiNothing;
                }

                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }

                string sharedBusCrsQuery = "";
                // 共用コース座席更新
                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, crsZasekiData) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用コース座席更新SQL作成
                    sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData);
                    updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                // 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = this.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData);
                updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure;
                }
            }

            // 部屋数更新
            string roomZansuUpdateQuery = this.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, true);
            if (string.IsNullOrEmpty(roomZansuUpdateQuery) == true)
            {
            }
            // ロールバック⇒SQL無しの場合は更新処理自体行わない（現行ASも同様）
            // MyBase.callRollbackTransaction(oracleTransaction)
            // Return CommonRegistYoyaku.UpdateStatusRoomStateFailure
            else
            {
                // SQLが生成された場合のみ処理実行
                updateCount = base.execNonQuery(oracleTransaction, roomZansuUpdateQuery);
                if (updateCount <= 0)
                {
                    // ROOM数の更新件数が0件以下の場合、処理終了
                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusRoomStateFailure;
                }
            }

            // 変更履歴登録
            OutputYoyakuChangeHistoryParam historyRec = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList);
            if (historyRec is null)
            {
                return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure;
            }

            // 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay;
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq;
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName);
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName);

            // 予約情報（基本）
            string yoyakuInfoQuery = this.createUpdateSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
            var yoyakuInfoBuilder = new StringBuilder();
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery);
            yoyakuInfoBuilder.AppendLine("WHERE");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu));
            yoyakuInfoBuilder.AppendLine("    AND ");
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure;
            }

            // 予約情報（コース料金）
            string crsChargeQuery = this.createUpdateSql<YoyakuInfoCrsChargeEntity>(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE");
            var crsChargeBuilder = new StringBuilder();
            crsChargeBuilder.AppendLine(crsChargeQuery);
            crsChargeBuilder.AppendLine("WHERE");
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu));
            crsChargeBuilder.AppendLine("    AND ");
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu));
            updateCount = base.execNonQuery(oracleTransaction, crsChargeBuilder.ToString());
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure;
            }

            // 予約情報（コース料金_料金区分）
            string chargeKbnQuery = "";
            foreach (YoyakuInfoCrsChargeChargeKbnEntity entity in registEntity.YoyakuInfoCrsChargeChargeKbnList)
            {
                chargeKbnQuery = this.createUpdateSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
                var chargeKbnBuilder = new StringBuilder();
                chargeKbnBuilder.AppendLine(chargeKbnQuery);
                chargeKbnBuilder.AppendLine("WHERE");
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    KBN_NO = " + base.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu));
                chargeKbnBuilder.AppendLine("    AND ");
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + base.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu));
                updateCount = base.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString());
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure;
                }
            }

            // 予約情報２
            string yoyakuInfo2Query;
            string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
            DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
            if (yoyakuInfo2.Rows.Count > 0)
            {
                string yoyakuInfo2UpdateQuery = this.createUpdateSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2");
                var yoyakuInfo2Builder = new StringBuilder();
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery);
                yoyakuInfo2Builder.AppendLine("WHERE");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu));
                yoyakuInfo2Builder.AppendLine("    AND ");
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + base.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu));
                yoyakuInfo2Query = yoyakuInfo2Builder.ToString();
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
                return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure;
            }

            // ピックアップルート台帳 （ホテル）
            if (registEntity.PickupRouteLedgerHotelList is object)
            {
                foreach (PickupRouteLedgerHotelEntity entity in registEntity.PickupRouteLedgerHotelList)
                {

                    // ピックアップルート台帳 （ホテル）人数更新SQL作成
                    string pickupHotelQuery = this.createPickupRouteLedgerHotelUpdateSql(entity);
                    updateCount = base.execNonQuery(oracleTransaction, pickupHotelQuery);
                    if (updateCount <= 0)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure;
                    }
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// WT_リクエスト情報キャンセル
    /// </summary>
    /// <param name="registEntity">予約登録データ</param>
    /// <returns>処理結果</returns>
    public string cancelWtRequest(RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // WT_リクエスト情報更新
            if (registEntity.WtRequestInfoEntity is object)
            {
                string wtRequestUpdateQuery = this.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity);
                updateCount = base.execNonQuery(oracleTransaction, wtRequestUpdateQuery);
                if (updateCount <= 0)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure;
                }
            }

            // コース台帳（基本）（WTキャンセル人数更新）
            string wtCancelNinzuQuery = this.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction);
            updateCount = base.execNonQuery(oracleTransaction, wtCancelNinzuQuery);
            if (updateCount == 0)
            {
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// WT情報登録
    /// </summary>
    /// <param name="registEntity">登録予約情報Entity</param>
    /// <returns>更新ステータス</returns>
    public string updateWtRequestInfo(RegistYoyakuInfoEntity registEntity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // WT_リクエスト情報
            if (string.IsNullOrEmpty(registEntity.WtRequestInfoEntity.managementKbn.Value) == true && registEntity.WtRequestInfoEntity.managementNo.Value is null)
            {

                // WT管理番号採番
                var param = new numberingWtRequestNoParam();
                param.crsCd = registEntity.WtRequestInfoEntity.crsCd.Value;
                param.managementKind = "W";
                YoyakuBizCommon.numberingWtRequestNo(param);
                registEntity.WtRequestInfoEntity.managementKbn.Value = param.managementKbn;
                registEntity.WtRequestInfoEntity.managementNo.Value = param.managementNo;
                string wtRequestQuery = this.createInsertSql<WtRequestInfoEntity>(registEntity.WtRequestInfoEntity, "T_WT_REQUEST_INFO");
                updateCount = base.execNonQuery(oracleTransaction, wtRequestQuery);
                if (updateCount == 0)
                {
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoFailure;
                }
            }
            else
            {
                string query = this.createUpdateSql<WtRequestInfoEntity>(registEntity.WtRequestInfoEntity, registEntity.WtRequestInfoPhysicsNameList, "T_WT_REQUEST_INFO");
                var sb = new StringBuilder();
                sb.Append(query);
                sb.AppendLine(" WHERE ");
                sb.AppendLine("     MANAGEMENT_KBN = " + base.setParam(registEntity.WtRequestInfoEntity.managementKbn.PhysicsName, registEntity.WtRequestInfoEntity.managementKbn.Value, registEntity.WtRequestInfoEntity.managementKbn.DBType, registEntity.WtRequestInfoEntity.managementKbn.IntegerBu, registEntity.WtRequestInfoEntity.managementKbn.DecimalBu));
                sb.AppendLine("     AND ");
                sb.AppendLine("     MANAGEMENT_NO = " + base.setParam(registEntity.WtRequestInfoEntity.managementNo.PhysicsName, registEntity.WtRequestInfoEntity.managementNo.Value, registEntity.WtRequestInfoEntity.managementNo.DBType, registEntity.WtRequestInfoEntity.managementNo.IntegerBu, registEntity.WtRequestInfoEntity.managementNo.DecimalBu));
                updateCount = base.execNonQuery(oracleTransaction, sb.ToString());
                if (updateCount == 0)
                {
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoFailure;
                }
            }

            // WT_リクエスト情報（コース料金_料金区分）
            string wtRequestChargeKbnDeleteQuery = this.createDeleteSql(registEntity.WtRequestInfoEntity.managementKbn.Value, registEntity.WtRequestInfoEntity.managementNo.Value, "T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN");
            updateCount = base.execNonQuery(oracleTransaction, wtRequestChargeKbnDeleteQuery);
            string wtRequestChargeKbnQuery = "";
            foreach (WtRequestInfoCrsChargeChargeKbnEntity entity in registEntity.WtRequestInfoCrsChargeChargeKbnList)
            {
                entity.managementKbn.Value = registEntity.WtRequestInfoEntity.managementKbn.Value;
                entity.managementNo.Value = registEntity.WtRequestInfoEntity.managementNo.Value;
                wtRequestChargeKbnQuery = this.createInsertSql<WtRequestInfoCrsChargeChargeKbnEntity>(entity, "T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN");
                updateCount = base.execNonQuery(oracleTransaction, wtRequestChargeKbnQuery);
                if (updateCount == 0)
                {
                    base.callRollbackTransaction(oracleTransaction);
                    return CommonRegistYoyaku.UpdateStatusWtRequestInfoCrsChargeChargeKbnFailure;
                }
            }

            // コース台帳（基本）（WTキャンセル人数更新）
            string wtCancelNinzuQuery = this.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction);
            updateCount = base.execNonQuery(oracleTransaction, wtCancelNinzuQuery);
            if (updateCount == 0)
            {
                base.callRollbackTransaction(oracleTransaction);
                return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure;
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

        return CommonRegistYoyaku.UpdateStatusSucess;
    }

    /// <summary>
    /// WTリクエストコース情報取得
    /// </summary>
    /// <param name="entity">WT_リクエスト情報entity</param>
    /// <returns>WTリクエストコース情報</returns>
    public DataTable getCrsWtRequestInfo(WtRequestInfoEntity entity)
    {
        DataTable wtRequestInfo;
        try
        {
            string query = this.createCrsWtRequestInfoSql(entity);
            wtRequestInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return wtRequestInfo;
    }

    /// <summary>
    /// WTリクエスト料金区分一覧取得
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WTリクエスト料金区分一覧</returns>
    public DataTable getWtRequestChargeList(WtRequestInfoEntity entity)
    {
        DataTable WtRequestChargeList;
        try
        {
            string query = this.createWtRequestChargeListSql(entity);
            WtRequestChargeList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return WtRequestChargeList;
    }

    /// <summary>
    /// WTリクエスト料金区分一覧取得
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WTリクエスト料金区分一覧</returns>
    public DataTable getWtRequestChargeKbnList(WtRequestInfoEntity entity)
    {
        DataTable wtRequestChargeList;
        try
        {
            string query = this.createWtRequestChargeKbnList(entity);
            wtRequestChargeList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return wtRequestChargeList;
    }

    /// <summary>
    /// WT_リクエスト情報（メモ）取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報（メモ）Entity</param>
    /// <returns>WT_リクエスト情報（メモ）取得SQL</returns>
    public DataTable getWtRequestMemoInfoList(WtRequestInfoMemoEntity entity)
    {
        DataTable wtRequestMemoInfo;
        try
        {
            string query = this.createWtRequestMemoInfoListSql(entity);
            wtRequestMemoInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return wtRequestMemoInfo;
    }

    /// <summary>
    /// WT_リクエスト情報取得
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WT_リクエスト情報</returns>
    public DataTable getWtRequestInfo(WtRequestInfoEntity entity)
    {
        DataTable wtRequestInfo;
        try
        {
            string query = this.createWtRequestInfoSql(entity);
            wtRequestInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return wtRequestInfo;
    }

    /// <summary>
    /// 複写予約情報取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>複写予約情報</returns>
    public DataTable getReproductionYoyakuData(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuData;
        try
        {
            string query = this.createReproductionYoyakuDataSql(entity);
            yoyakuData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuData;
    }

    /// <summary>
    /// 複写WTリクエスト情報取得
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>複写WTリクエスト情報</returns>
    public DataTable geteReproductionWtRequestData(WtRequestInfoEntity entity)
    {
        DataTable yoyakuData;
        try
        {
            string query = this.createReproductionWtRequestDataSql(entity);
            yoyakuData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuData;
    }

    /// <summary>
    /// 代理店情報取得
    /// </summary>
    /// <param name="entity">代理店マスタEntity</param>
    /// <returns>代理店情報</returns>>
    public DataTable getAgentMaster(MAgentEntity entity)
    {
        DataTable agentMasterData;
        try
        {
            string query = this.createAgentMasterSql(entity);
            agentMasterData = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return agentMasterData;
    }

    /// <summary>
    /// 予約ピックアップ情報取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約ピックアップ情報</returns>
    public DataTable getYoyakuPickupList(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuPickupList;
        try
        {
            string query = this.createYoyakuPickupInfoSql(entity);
            yoyakuPickupList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuPickupList;
    }

    /// <summary>
    /// その他情報取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>その他情報</returns>
    public DataTable getSonotaInfo(YoyakuInfoBasicEntity entity)
    {
        DataTable sonotaInfo;
        try
        {
            string query = this.createSonotaInfoSql(entity);
            sonotaInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return sonotaInfo;
    }

    /// <summary>
    /// その他情報取得
    /// </summary>
    /// <param name="entity">WTリクエストEntity</param>
    /// <returns>その他情報</returns>
    public DataTable getWtSonotaInfo(WtRequestInfoEntity entity)
    {
        DataTable sonotaInfo;
        try
        {
            string query = this.createWtSonotaInfoSql(entity);
            sonotaInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return sonotaInfo;
    }

    /// <summary>
    /// 利用者情報取得
    /// </summary>
    /// <param name="nullRecord">空行レコード挿入フラグ</param>
    /// <returns>利用者情報</returns>
    public DataTable getUserMaster(bool nullRecord)
    {
        DataTable sonotaInfo;
        try
        {
            string query = this.createUserMasterSql(nullRecord);
            sonotaInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return sonotaInfo;
    }

    /// <summary>
    /// コース情報取得SQL作成
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>コース情報取得SQL</returns>
    private string createCrsLedgerBasicSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLB.SYUPT_DAY ");
        sb.AppendLine("     ,CLB.CRS_CD ");
        sb.AppendLine("     ,CLB.CRS_NAME ");
        sb.AppendLine("     ,CLB.GOUSYA ");
        sb.AppendLine("     ,CLB.CRS_KIND ");
        sb.AppendLine("     ,CLB.CRS_KBN_1 ");
        sb.AppendLine("     ,CLB.CRS_KBN_2 ");
        sb.AppendLine("     ,CLB.TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     ,CLB.HOUJIN_GAIKYAKU_KBN ");
        sb.AppendLine("     ,CLB.SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     ,CLB.SAIKOU_DAY ");
        sb.AppendLine("     ,CLB.UNKYU_CONTACT_DAY ");
        sb.AppendLine("     ,CLB.RETURN_DAY ");
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ");
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_1 AS JYOCHACHI_CD_1 ");
        sb.AppendLine("     ,PL1.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_1 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_2 AS JYOCHACHI_CD_2 ");
        sb.AppendLine("     ,PL2.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_2 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_3 AS JYOCHACHI_CD_3 ");
        sb.AppendLine("     ,PL3.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_3 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_4 AS JYOCHACHI_CD_4 ");
        sb.AppendLine("     ,PL4.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_4 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_5 AS JYOCHACHI_CD_5 ");
        sb.AppendLine("     ,PL5.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_5 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_5 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_5 ");
        sb.AppendLine("     ,CLB.JYOSEI_SENYO_SEAT_FLG ");
        sb.AppendLine("     ,CLB.TYUIJIKOU ");
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ");
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ");
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ");
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ");
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ");
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,CLB.HURIKOMI_NG_FLG ");
        sb.AppendLine("     ,CLB.ONE_SANKA_FLG ");
        sb.AppendLine("     ,CLB.JYOSYA_CAPACITY ");
        sb.AppendLine("     ,CLB.TEIINSEI_FLG ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_ONE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_TWO_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_THREE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FOUR_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FIVE_1R ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_ONE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_TWO_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_THREE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FOUR_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FIVE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_SOKEI ");
        sb.AppendLine("     ,CLB.CANCEL_RYOU_KBN ");
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ");
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ");
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ");
        sb.AppendLine("     ,CLB.TEJIMAI_DAY ");
        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ");
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ");
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ");
        sb.AppendLine("     ,CLB.AGE_FLG ");
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ");
        sb.AppendLine("     ,CLB.TEL_FLG ");
        sb.AppendLine("     ,CLB.ADDRESS_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_STOP_FLG ");
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ");
        sb.AppendLine("     ,CLB.USING_FLG ");
        sb.AppendLine("     ,CLB.TOJITU_KOKUCHI_FLG");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL1 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL1.PLACE_CD = CLB.HAISYA_KEIYU_CD_1 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL2 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL2.PLACE_CD = CLB.HAISYA_KEIYU_CD_2 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL3 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL3.PLACE_CD = CLB.HAISYA_KEIYU_CD_3 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL4 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL4.PLACE_CD = CLB.HAISYA_KEIYU_CD_4 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL5 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL5.PLACE_CD = CLB.HAISYA_KEIYU_CD_5 ");
        sb.AppendLine(" WHERE  ");
        sb.AppendLine("     CLB.CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }
	
	private string createUsingFlagSql(Hashtable paramInfoList)
    {
        base.paramClear();
        var entity = new CrsLedgerBasicEntity();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      USING_FLG ");
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" WHERE  ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", paramInfoList["CRS_CD"], entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", paramInfoList["SYUPT_DAY"], entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        if (string.IsNullOrEmpty(paramInfoList["GOUSYA"].ToString()) == false)
        {
            sb.AppendLine("     AND ");
            sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", paramInfoList["GOUSYA"], entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 後泊部屋数更新SQL作成
    /// </summary>
    /// <param name="atohakuCrsCd">後泊コースコード</param>
    /// <param name="defRoomsu">>差分部屋数</param>
    /// <param name="paramInfoList">パラメータ群</param>
    /// <returns>後泊部屋数更新SQL</returns>
    private string createAtohakuRoomsuUpdateSql(string atohakuCrsCd, int defRoomsu, Hashtable paramInfoList)
    {
        base.paramClear();
        var entity = new CrsLedgerBasicEntity();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = YOYAKU_NUM_TEISEKI + " + base.setParam("KUSEKI_NUM_TEISEKI", defRoomsu, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = KUSEKI_NUM_TEISEKI - " + base.setParam("KUSEKI_NUM_TEISEKI", defRoomsu, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam("SYSTEM_UPDATE_PGMID", paramInfoList["SYSTEM_UPDATE_PGMID"], entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam("SYSTEM_UPDATE_PERSON_CD", paramInfoList["SYSTEM_UPDATE_PERSON_CD"], entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam("SYSTEM_UPDATE_DAY", paramInfoList["SYSTEM_UPDATE_DAY"], entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", atohakuCrsCd, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", paramInfoList["SYUPT_DAY"], entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース使用中チェック
    /// 企画用
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <param name="oracleTransaction">トランザクション</param>
    /// <param name="zasekiData">自コース座席情報</param>
    /// <param name="sharedZasekiData">共用コース座席情報</param>
    /// <returns>検証結果</returns>
    private bool isCrsLenderZasekiDataForKikaku(Hashtable paramInfoList, OracleTransaction oracleTransaction, ref DataTable zasekiData, ref DataTable sharedZasekiData)
    {
        bool isValid = false;
        int idx = 1;
        string busReserveCd = "";
        string sharedCrsQuery = "";
        while (idx <= UsingCheckRetryNum)
        {

            // 自コースの座席情報取得SQL作成
            string crsQuery = createCrsLedgerZasekiSql(paramInfoList);

            // 自コースの使用中チェック
            zasekiData = base.getDataTable(oracleTransaction, crsQuery);
            if (zasekiData.Rows.Count <= 0)
            {

                // 自コースが使用中の場合、リトライ
                idx = idx + 1;
                continue;
            }

            // 共用コースの使用中チェック
            // バス指定コード取得
            busReserveCd = zasekiData.Rows(0)("BUS_RESERVE_CD").ToString();
            // 共用コースの座席情報取得SQL作成
            sharedCrsQuery = createSharedBusCrsZasekiSql(paramInfoList, busReserveCd);
            // 共用コース取得
            sharedZasekiData = base.getDataTable(oracleTransaction, sharedCrsQuery);
            if (sharedZasekiData.Rows.Count <= 0)
            {
                // 共用コースがない場合、処理終了

                isValid = true;
                break;
            }

            foreach (DataRow sharedRow in sharedZasekiData.Rows)
            {
                if (sharedRow("USING_FLG") is object && string.IsNullOrEmpty(sharedRow("USING_FLG").ToString()) == false)
                {
                    // 共用コースが使用中の場合、リトライ
                    idx = idx + 1;
                    continue;
                }
            }

            // 共用コースが使用中でない場合、処理終了
            isValid = true;
            break;
        }

        return isValid;
    }

    /// <summary>
    /// コース台帳座席情報取得SQL作成
    /// 悲観ロック用
    /// </summary>
    /// <param name="paramInfoList">パラメータ群</param>
    /// <returns>コース台帳座席情報取得SQL</returns>
    private string createCrsLedgerZasekiSql(Hashtable paramInfoList)
    {
        base.paramClear();
        var entity = new CrsLedgerBasicEntity();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ");
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,JYOSYA_CAPACITY ");
        sb.AppendLine("     ,CAPACITY_REGULAR ");
        sb.AppendLine("     ,CAPACITY_HO_1KAI ");
        sb.AppendLine("     ,EI_BLOCK_REGULAR ");
        sb.AppendLine("     ,EI_BLOCK_HO ");
        sb.AppendLine("     ,BLOCK_KAKUHO_NUM ");
        sb.AppendLine("     ,KUSEKI_KAKUHO_NUM ");
        sb.AppendLine("     ,BUS_RESERVE_CD ");
        sb.AppendLine("     ,TEIINSEI_FLG ");
        sb.AppendLine("     ,CRS_BLOCK_ONE_1R ");
        sb.AppendLine("     ,CRS_BLOCK_TWO_1R ");
        sb.AppendLine("     ,CRS_BLOCK_THREE_1R ");
        sb.AppendLine("     ,CRS_BLOCK_FOUR_1R ");
        sb.AppendLine("     ,CRS_BLOCK_FIVE_1R ");
        sb.AppendLine("     ,CRS_BLOCK_ROOM_NUM ");
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM ");
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM ");
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM ");
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM ");
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM ");
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI ");
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI ");
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_MALE ");
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM ");
        sb.AppendLine("     ,USING_FLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     USING_FLG IS NULL ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", paramInfoList["CRS_CD"], entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", paramInfoList["SYUPT_DAY"], entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", paramInfoList["GOUSYA"], entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" FOR UPDATE WAIT 10 ");
        return sb.ToString();
    }

    /// <summary>
    /// 共用バスコース座席情報取得SQL作成
    /// 悲観ロック用
    /// </summary>
    /// <param name="paramInfoList">パラメータ群</param>
    /// <param name="busReserveCd">バス指定コード</param>
    /// <returns>共用バスコース座席情報取得SQL</returns>
    private string createSharedBusCrsZasekiSql(Hashtable paramInfoList, string busReserveCd)
    {
        base.paramClear();
        var entity = new CrsLedgerBasicEntity();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CRS_CD ");
        sb.AppendLine("     ,SYUPT_DAY ");
        sb.AppendLine("     ,GOUSYA ");
        sb.AppendLine("     ,YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ");
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,JYOSYA_CAPACITY ");
        sb.AppendLine("     ,CAPACITY_REGULAR ");
        sb.AppendLine("     ,CAPACITY_HO_1KAI ");
        sb.AppendLine("     ,EI_BLOCK_REGULAR ");
        sb.AppendLine("     ,EI_BLOCK_HO ");
        sb.AppendLine("     ,BLOCK_KAKUHO_NUM ");
        sb.AppendLine("     ,KUSEKI_KAKUHO_NUM ");
        sb.AppendLine("     ,BUS_RESERVE_CD ");
        sb.AppendLine("     ,USING_FLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     BUS_RESERVE_CD = " + base.setParam("BUS_RESERVE_CD", busReserveCd, entity.busReserveCd.DBType, entity.busReserveCd.IntegerBu, entity.busReserveCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", paramInfoList["SYUPT_DAY"], entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", paramInfoList["GOUSYA"], entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" FOR UPDATE WAIT 10 ");
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳座席情報更新SQL作成
    /// 定期用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」情報</param>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="crsZasekiData">座席更新の検索条件</param>
    /// <param name="kusekiTeisaki">空席数定席</param>
    /// <param name="kusekiSubSeat">空席数補助席</param>
    /// <returns>コース台帳座席情報更新SQL</returns>
    private string createCrsZasekiDataForTeiki(Z0001_Result z0001Result, DataTable crsLedgerZasekiData, Hashtable crsZasekiData, ref int kusekiTeisaki, ref int kusekiSubSeat)
    {
        int crsYoyakuSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int crsKusekiSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));
        int crsKusekiSuSubSeat = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"));

        // 予約数定席
        // コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        int yoyakuSuTeisaki = crsYoyakuSuTeisaki + z0001Result.ZasekiKagenTeiseki;

        // 空席数定席算出
        // コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        kusekiTeisaki = this.calcKuusekiSu(crsKusekiSuTeisaki, z0001Result.ZasekiKagenTeiseki, z0001Result.KusekiNumTeiseki);

        // 空席数定席マイナスチェック
        if (kusekiTeisaki < CommonRegistYoyaku.ZERO)
        {
            // 空席数定席が'0'を下回る場合、エラーとして更新処理終了
            return "";
        }

        // 空席数補助席算出
        // コース台帳(基本).空席数補助席、共通処理「バス座席自動設定処理」.補助調整空席、共通処理「バス座席自動設定処理」.空席数/補助・1F
        kusekiSubSeat = this.calKusekiSuSubSeatForTeiki(crsKusekiSuSubSeat, z0001Result.SubCyoseiSeatNum, z0001Result.ZasekiKagenSub1F);
        var entity = new CrsLedgerBasicEntity();
        entity.crsCd.Value = crsZasekiData["CRS_CD"].ToString();
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["GOUSYA"]);
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["SYUPT_DAY"]);
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki;
        entity.kusekiNumTeiseki.Value = kusekiTeisaki;
        entity.kusekiNumSubSeat.Value = kusekiSubSeat;
        entity.systemUpdatePgmid.Value = crsZasekiData["SYSTEM_UPDATE_PGMID"].ToString();
        entity.systemUpdatePersonCd.Value = crsZasekiData["SYSTEM_UPDATE_PERSON_CD"].ToString();
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData["SYSTEM_UPDATE_DAY"].ToString());

        // コース台帳座席更新SQL作成
        string query = createCrsZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// 共用コース座席更新SQL作成
    /// 定席用
    /// </summary>
    /// <param name="sharedRow">共用コース座席情報</param>
    /// <param name="crsZasekiData">コース台帳座席更新データ</param>
    /// <param name="kusekiTeisaki">空席数定席</param>
    /// <param name="kusekiSubSeat">空席数補助席</param>
    /// <returns>共用コース座席更新SQL</returns>
    private string createSharedCrsZasekiUpdateSqlForTeiseki(DataRow sharedRow, Hashtable crsZasekiData, int kusekiTeisaki, int kusekiSubSeat)
    {
        var entiy = new CrsLedgerBasicEntity();
        entiy.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"));
        entiy.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"));
        entiy.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"));
        entiy.kusekiNumTeiseki.Value = kusekiTeisaki;
        entiy.kusekiNumSubSeat.Value = kusekiSubSeat;
        string query = createSharedCrsZasekiUpdateSql(crsZasekiData, entiy);
        return query;
    }

    /// <summary>
    /// コース台帳座席情報更新SQL作成
    /// 企画用
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」情報</param>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="crsZasekiData">座席更新の検索条件</param>
    /// <returns>コース台帳座席情報更新SQL</returns>
    private string createCrsZasekiDataForKikaku(Z0001_Result z0001Result, DataTable crsLedgerZasekiData, Hashtable crsZasekiData)
    {
        int crsYoyakuSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int crsKusekiSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));
        int crsKusekiSuSubSeat = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"));

        // 予約数定席
        // コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        int yoyakuSuTeisaki = crsYoyakuSuTeisaki + z0001Result.ZasekiKagenTeiseki;
        // 空席数定席算出
        // コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        int kusekiSuTeisaki = this.calcKuusekiSu(crsKusekiSuTeisaki, z0001Result.ZasekiKagenTeiseki, z0001Result.KusekiNumTeiseki);
        // 空席数補助席算出
        // コース台帳(基本).空席数定席,共通処理「バス座席自動設定処理」.座席加減数・補助席,共通処理「バス座席自動設定処理」.補助調整空席,共通処理「バス座席自動設定処理」.空席数/補助・1F
        int kusekiSuSubSeat = this.calKusekiSuSubSeatForKikaku(crsKusekiSuSubSeat, z0001Result.ZasekiKagenSub1F, z0001Result.SubCyoseiSeatNum, z0001Result.KusekiNumSub1F);
        if (kusekiSuTeisaki < CommonRegistYoyaku.ZERO || kusekiSuSubSeat < CommonRegistYoyaku.ZERO)
        {
            // 空席数定席、空席数補助席が'0'を下回る場合、エラーとして更新処理終了

            return "";
        }

        var entity = new CrsLedgerBasicEntity();
        entity.crsCd.Value = crsZasekiData["CRS_CD"].ToString();
        entity.gousya.Value = int.Parse(crsZasekiData["GOUSYA"].ToString());
        entity.syuptDay.Value = int.Parse(crsZasekiData["SYUPT_DAY"].ToString());
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki;

        // 予約数補助席
        if (string.IsNullOrEmpty(z0001Result.SeatKbn) == false)
        {
            // 共通処理「バス座席自動設定処理」.席区分が空以外の場合
            // 予約数補助席を設定(コース台帳（基本）.予約数・補助席 + 共通処理「バス座席自動設定処理」.座席加減数・補助席)
            int crsYoyakuSuSubSeat = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_SUB_SEAT"));
            entity.yoyakuNumSubSeat.Value = crsYoyakuSuSubSeat + z0001Result.ZasekiKagenSub1F;
        }

        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki;
        entity.kusekiNumSubSeat.Value = kusekiSuSubSeat;
        entity.systemUpdatePgmid.Value = crsZasekiData["SYSTEM_UPDATE_PGMID"].ToString();
        entity.systemUpdatePersonCd.Value = crsZasekiData["SYSTEM_UPDATE_PERSON_CD"].ToString();
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData["SYSTEM_UPDATE_DAY"].ToString());

        // コース台帳座席更新SQL作成
        string query = createCrsZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// 空席数定席算出
    /// </summary>
    /// <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    /// <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・定席</param>
    /// <param name="comKusekiSu">共通処理「バス座席自動設定処理」.空席数・定席</param>
    /// <returns>空席数定席</returns>
    private int calcKuusekiSu(int crsKusekiSu, int comZasekiKagenSu, int comKusekiSu)
    {

        // コース台帳(基本).空席数定席 - >共通処理「バス座席自動設定処理」.座席加減数・定席
        int kusekiSu = crsKusekiSu - comZasekiKagenSu;
        if (kusekiSu >= comKusekiSu)
        {
            // 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席」が共通処理「バス座席自動設定処理」.空席数・定席以上の場合
            // 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席とする
            kusekiSu = comKusekiSu;
        }

        return kusekiSu;
    }

    /// <summary>
    /// 空席数補助席算出
    /// </summary>
    /// <param name="crsKusekiSu">コース台帳(基本).空席数補助席</param>
    /// <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    /// <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    /// <returns>空席数補助席</returns>
    private int calKusekiSuSubSeatForTeiki(int crsKusekiSu, int comSubChoseiKusekiSu, int comKusekiSuSubSeat1F)
    {

        // コース台帳(基本).空席数定席 - 共通処理「バス座席自動設定処理」.補助調整空席
        int kusekiSu = crsKusekiSu - comSubChoseiKusekiSu;
        if (kusekiSu >= comKusekiSuSubSeat1F)
        {
            // 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.補助調整空席」が共通処理「バス座席自動設定処理」.空席数・定席以上の場合
            kusekiSu = comKusekiSuSubSeat1F;
        }

        if (kusekiSu < CommonRegistYoyaku.ZERO)
        {
            // 空席数補助席の算出結果が0を下回る場合、0とする
            kusekiSu = CommonRegistYoyaku.ZERO;
        }

        return kusekiSu;
    }

    /// <summary>
    /// 空席数補助席算出
    /// </summary>
    /// <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    /// <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・補助席</param>
    /// <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    /// <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    /// <returns></returns>
    private int calKusekiSuSubSeatForKikaku(int crsKusekiSu, int comZasekiKagenSu, int comSubChoseiKusekiSu, int comKusekiSuSubSeat1F)
    {
        int kusekiSu = crsKusekiSu - comZasekiKagenSu - comSubChoseiKusekiSu;
        if (kusekiSu >= comKusekiSuSubSeat1F)
        {
            // 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席 - 共通処理「バス座席自動設定処理」.補助調整空席」が共通処理「バス座席自動設定処理」.空席数/補助・1F以上の場合
            kusekiSu = comKusekiSuSubSeat1F;
        }

        return kusekiSu;
    }

    /// <summary>
    /// コース台帳座席更新SQL作成
    /// </summary>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="crsZasekiData">座席更新の検索条件</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsKakuShashuZasekiData(DataTable crsLedgerZasekiData, Hashtable crsZasekiData)
    {
        int yoyakuNum = int.Parse(crsZasekiData["YOYAKU_NINZU"].ToString());
        int yoyakuNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString());
        int kusekiNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString());
        var entity = new CrsLedgerBasicEntity();
        entity.crsCd.Value = crsZasekiData["CRS_CD"].ToString();
        entity.gousya.Value = int.Parse(crsZasekiData["GOUSYA"].ToString());
        entity.syuptDay.Value = int.Parse(crsZasekiData["SYUPT_DAY"].ToString());
        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + yoyakuNum;
        entity.kusekiNumTeiseki.Value = kusekiNumTeiseki - yoyakuNum;
        entity.systemUpdatePgmid.Value = crsZasekiData["SYSTEM_UPDATE_PGMID"].ToString();
        entity.systemUpdatePersonCd.Value = crsZasekiData["SYSTEM_UPDATE_PERSON_CD"].ToString();
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData["SYSTEM_UPDATE_DAY"].ToString());

        // コース台帳架空車種座席更新SQL作成
        string query = createCrsKakuShashuZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// コース台帳座席更新SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsZasekiUpdateSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam("YOYAKU_NUM_TEISEKI", entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        if (entity.yoyakuNumSubSeat.Value is object)
        {
            sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT = " + base.setParam("YOYAKU_NUM_SUB_SEAT", entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu));
        }

        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam("KUSEKI_NUM_TEISEKI", entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam("KUSEKI_NUM_SUB_SEAT", entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam("SYSTEM_UPDATE_PGMID", entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam("SYSTEM_UPDATE_PERSON_CD", entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam("SYSTEM_UPDATE_DAY", entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳架空車種座席更新SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsKakuShashuZasekiUpdateSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam("YOYAKU_NUM_TEISEKI", entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam("KUSEKI_NUM_TEISEKI", entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam("SYSTEM_UPDATE_PGMID", entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam("SYSTEM_UPDATE_PERSON_CD", entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam("SYSTEM_UPDATE_DAY", entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 自コースと共用コースのコースコード、号車、出発日が同一チェック
    /// </summary>
    /// <param name="sharedRow">共用コースRow</param>
    /// <param name="crsZasekiData">座席更新の検索条件作成</param>
    /// <returns>検証結果</returns>
    private bool isSharedBusCrsEqualCheck(DataRow sharedRow, Hashtable crsZasekiData)
    {
        string csharedCrsCd = sharedRow("CRS_CD").ToString();
        int csharedGousya = int.Parse(sharedRow("GOUSYA").ToString());
        int csharedSyuptDay = int.Parse(sharedRow("SYUPT_DAY").ToString());
        string crsCd = crsZasekiData["CRS_CD"].ToString();
        int gousya = int.Parse(crsZasekiData["GOUSYA"].ToString());
        int syuptDay = int.Parse(crsZasekiData["SYUPT_DAY"].ToString());
        if ((csharedCrsCd ?? "") == (crsCd ?? "") && csharedGousya == gousya && csharedSyuptDay == syuptDay)
        {

            // 自コースと共用コースのコースコード、号車、出発日が同一の場合
            return false;
        }

        return true;
    }

    /// <summary>
    /// 共用コース座席更新SQL作成
    /// </summary>
    /// <param name="z0001Result">共通処理「バス座席自動設定処理」</param>
    /// <param name="sharedRow">共用コース座席情報</param>
    /// <param name="crsZasekiData">座席更新の検索条件</param>
    /// <returns>共用コース座席更新SQL</returns>
    private string createSharedBusCrsZasekiUpdateSql(Z0001_Result z0001Result, DataRow sharedRow, Hashtable crsZasekiData)
    {
        int kusekiTeisaki = 0;
        int kusekiSubSeat = 0;
        int jyosyaCapacity = CommonRegistYoyaku.convertObjectToInteger(sharedRow("JYOSYA_CAPACITY"));
        int capacityRegular = CommonRegistYoyaku.convertObjectToInteger(sharedRow("CAPACITY_REGULAR"));
        int capacityHo1kai = CommonRegistYoyaku.convertObjectToInteger(sharedRow("CAPACITY_HO_1KAI"));
        if (jyosyaCapacity >= capacityRegular + capacityHo1kai)
        {
            // コース台帳(基本).乗車定員が「コース台帳(基本).定員定 + コース台帳(基本).定員補1F」以上の場合

            // 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席
            kusekiTeisaki = z0001Result.KusekiNumTeiseki;
            // 空席数補助席を共通処理「バス座席自動設定処理」.空席数・補助席
            kusekiSubSeat = z0001Result.KusekiNumSub1F;
        }
        else
        {
            // それ以外の場合

            // 営BLK算出
            int eiBlockNum = calEiBlockNum(sharedRow);
            if (jyosyaCapacity >= capacityRegular)
            {
                // コース台帳(基本).乗車定員がコース台帳(基本).定員定以上の場合

                if (capacityRegular - eiBlockNum <= z0001Result.KusekiNumTeiseki)
                {
                    // 「コース台帳(基本).定員定 - 営BLK算出結果」が共通処理「バス座席自動設定処理」.空席数・定席以下の場合

                    // 空席数・定席を「コース台帳(基本).定員定 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum;
                }
                else
                {
                    // それ以外の場合

                    // 空席数・定席を共通処理「バス座席自動設定処理」.空席数・定席とする
                    kusekiTeisaki = z0001Result.KusekiNumTeiseki;
                }

                int eiBlockHo = CommonRegistYoyaku.convertObjectToInteger(sharedRow("EI_BLOCK_HO"));
                int yoyaokuNumSubSeat = CommonRegistYoyaku.convertObjectToInteger(sharedRow("YOYAKU_NUM_SUB_SEAT"));
                if (capacityRegular - eiBlockHo - yoyaokuNumSubSeat <= z0001Result.KusekiNumSub1F)
                {
                    // 「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」が
                    // 共通処理「バス座席自動設定処理」.空席数・補助席以下の場合

                    // 空席数・補助席を「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」とする
                    kusekiSubSeat = capacityRegular - eiBlockHo - yoyaokuNumSubSeat;
                }
                else
                {
                    // それ以外の場合

                    // 空席数・補助席をを共通処理「バス座席自動設定処理」空席数・補助席とする
                    kusekiSubSeat = z0001Result.KusekiNumSub1F;
                }
            }
            else
            {
                // それ以外の場合、

                if (jyosyaCapacity - eiBlockNum <= z0001Result.KusekiNumTeiseki)
                {
                    // 「コース台帳(基本).乗車定員 - 営BLK算出結果」が共通処理「バス座席自動設定処理」空席数・定席以下の場合

                    // 空席数・定席を「コース台帳(基本).乗車定員 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum;
                }
                else
                {
                    // それ以外の場合

                    // 空席数・定席を共通処理「バス座席自動設定処理」空席数・定席とする
                    kusekiTeisaki = z0001Result.KusekiNumTeiseki;
                }

                // 空席数・補助席は'0'とする
                kusekiSubSeat = CommonRegistYoyaku.ZERO;
            }
        }

        var entiy = new CrsLedgerBasicEntity();
        entiy.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"));
        entiy.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"));
        entiy.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"));
        entiy.kusekiNumTeiseki.Value = kusekiTeisaki;
        entiy.kusekiNumSubSeat.Value = kusekiSubSeat;

        // 更新SQL作成
        string query = createSharedCrsZasekiUpdateSql(crsZasekiData, entiy);
        return query;
    }

    /// <summary>
    /// 営BLK算出
    /// </summary>
    /// <param name="sharedRow">共用コース座席情報</param>
    /// <returns>営BLK</returns>
    private int calEiBlockNum(DataRow sharedRow)
    {
        int eiBlockRegular = CommonRegistYoyaku.convertObjectToInteger(sharedRow("EI_BLOCK_REGULAR"));
        int blockKakuhoNum = CommonRegistYoyaku.convertObjectToInteger(sharedRow("BLOCK_KAKUHO_NUM"));
        int yoyakuNumTeisaki = CommonRegistYoyaku.convertObjectToInteger(sharedRow("YOYAKU_NUM_TEISEKI"));

        // 営BLK = 「コース台帳(基本).営ブロック定 + コース台帳(基本).ブロック確保数 + コース台帳(基本).空席確保数 + コース台帳(基本).予約数・定席
        int eiBlk = eiBlockRegular + blockKakuhoNum + yoyakuNumTeisaki;
        return eiBlk;
    }

    /// <summary>
    /// 共用コース座席更新SQL作成
    /// </summary>
    /// <param name="crsZasekiData">座席更新の検索条件</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>共用コース座席更新SQL</returns>
    private string createSharedCrsZasekiUpdateSql(Hashtable crsZasekiData, CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, crsZasekiData["SYSTEM_UPDATE_PGMID"], entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, crsZasekiData["SYSTEM_UPDATE_PERSON_CD"], entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, crsZasekiData["SYSTEM_UPDATE_DAY"], entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳空席数取得SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳空席数取得SQL</returns>
    private string createCrsLedgerBasicKusekiNumDataSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ");
        sb.AppendLine("     ,CRS_BLOCK_CAPACITY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
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
    /// 予約情報（基本）UPDATE SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <param name="physicsNameList">更新するカラム名一覧</param>
    /// <param name="tableName">テーブル名</param>
    /// <returns>予約情報（基本）UPDATE SQL</returns>
    private string createYoyakuInfoBasicUpdateSql(YoyakuInfoBasicEntity entity, List<string> physicsNameList, string tableName)
    {
        string valueQuery = createUpdateSql<YoyakuInfoBasicEntity>(entity, physicsNameList, tableName);
        var sb = new StringBuilder();
        sb.AppendLine(valueQuery);
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// UPDATE SQL作成
    /// </summary>
    /// <typeparam name="T">テーブルEntity</typeparam>
    /// <param name="entity">更新するテーブルEntity</param>
    /// <param name="physicsNameList">更新するカラム名一覧</param>
    /// <param name="tableName">テーブル名</param>
    /// <returns>UPDATE SQL</returns>
    private string createUpdateSql<T>(T entity, List<string> physicsNameList, string tableName)
    {
        base.paramClear();
        var type = typeof(T);
        var properties = type.GetProperties();
        int idx = 0;
        string comma = "";
        string physicsName = "";
        string updatePhysicsName = "";
        var valueSql = new StringBuilder();
        string format = "{0}{1} = {2}";
        foreach (PropertyInfo prop in properties)
        {
            if (idx == 0)
            {
                comma = "";
            }

            if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_YmdType)))
            {
                physicsName = ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).PhysicsName;
                if (physicsNameList.Any(x => (x ?? "") == (physicsName ?? "")) == false)
                {
                    // 更新物理名リストにない場合、次レコードへ
                    continue;
                }

                valueSql.AppendLine(string.Format(format, comma, physicsName, base.setParam(physicsName, ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_YmdType)prop.GetValue(entity, null)).DBType)));
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_NumberType)))
            {
                physicsName = ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).PhysicsName;
                if (physicsNameList.Any(x => (x ?? "") == (physicsName ?? "")) == false)
                {
                    // 更新物理名リストにない場合、次レコードへ
                    continue;
                }

                valueSql.AppendLine(string.Format(format, comma, physicsName, base.setParam(physicsName, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_NumberType)prop.GetValue(entity, null)).DecimalBu)));
            }
            else if (ReferenceEquals(prop.PropertyType, typeof(EntityKoumoku_Number_DecimalType)))
            {
                physicsName = ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).PhysicsName;
                if (physicsNameList.Any(x => (x ?? "") == (physicsName ?? "")) == false)
                {
                    // 更新物理名リストにない場合、次レコードへ
                    continue;
                }

                valueSql.AppendLine(string.Format(format, comma, physicsName, base.setParam(physicsName, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_Number_DecimalType)prop.GetValue(entity, null)).DecimalBu)));
            }
            else
            {
                physicsName = ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).PhysicsName;
                if (physicsNameList.Any(x => (x ?? "") == (physicsName ?? "")) == false)
                {
                    // 更新物理名リストにない場合、次レコードへ
                    continue;
                }

                valueSql.AppendLine(string.Format(format, comma, physicsName, base.setParam(physicsName, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).Value, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).DBType, ((EntityKoumoku_MojiType)prop.GetValue(entity, null)).IntegerBu)));
            }

            comma = ",";
            idx = idx + 1;
        }

        // UPDATE文作成
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine(string.Format("     {0} ", tableName));
        sb.AppendLine(" SET ");
        sb.AppendLine(valueSql.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// 料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>料金区分一覧取得SQL</returns>
    private string createChargeKbnListSql(CrsLedgerChargeEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLC.CHARGE_KBN ");
        sb.AppendLine("     ,CLC.KBN_NO ");
        sb.AppendLine("     ,NVL(MCK.CHARGE_NAME, '-') AS CHARGE_NAME ");
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ");
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ");
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME_RK ");
        sb.AppendLine("     ,CJK.SEX_BETU ");
        sb.AppendLine("     ,CCK.CHARGE ");
        sb.AppendLine("     ,CCK.CHARGE_SUB_SEAT ");
        sb.AppendLine("     ,CCK.CARRIAGE ");
        sb.AppendLine("     ,CCK.CARRIAGE_SUB_SEAT ");
        sb.AppendLine("     ,CCK.CHARGE_1 ");
        sb.AppendLine("     ,CCK.CHARGE_2 ");
        sb.AppendLine("     ,CCK.CHARGE_3 ");
        sb.AppendLine("     ,CCK.CHARGE_4 ");
        sb.AppendLine("     ,CCK.CHARGE_5 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_1 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_2 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_3 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_4 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_5 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_1 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_2 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_3 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_4 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_5 ");
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE CLC ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CCK.CRS_CD = CLC.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CCK.SYUPT_DAY = CLC.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CCK.GOUSYA = CLC.GOUSYA ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CCK.KBN_NO = CLC.KBN_NO ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_KBN MCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MCK.CHARGE_KBN = CLC.CHARGE_KBN ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CLC.CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" ORDER BY ");
        sb.AppendLine("     CLC.KBN_NO, CCK.CHARGE_KBN_JININ_CD ");
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳（コース情報）取得SQL作成
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>コース台帳（コース情報）取得SQL</returns>
    private string createCrsInfoSql(CrsLedgerCrsInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      LINE_NO ");
        sb.AppendLine("     ,CRS_JOKEN ");
        sb.AppendLine(" FROM T_CRS_LEDGER_CRS_INFO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     DELETE_DAY = 0 ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CRS_CD =  " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" ORDER BY ");
        sb.AppendLine("     LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// メッセージ情報取得SQL作成
    /// </summary>
    /// <param name="entity">パラメータ群</param>
    /// <returns>メッセージ情報取得SQL</returns>
    private string createCrsInfoMessageSql(CrsLedgerMessageEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      '' AS MESSAGE_CHECK_FLG ");
        sb.AppendLine("     ,LINE_NO ");
        sb.AppendLine("     ,MESSAGE ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_MESSAGE ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" ORDER BY ");
        sb.AppendLine("     LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// コード分類の内容を取得SQL作成
    /// </summary>
    /// <param name="codeBunrui">コード分類</param>
    /// <param name="nullRecord">空行レコード挿入フラグ</param>
    /// <returns>コード分類の内容を取得SQL</returns>
    private string createCodeMasterSql(string codeBunrui, bool nullRecord)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT * FROM ( ");
        if (nullRecord == true)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("      ' ' AS CODE_VALUE ");
            sb.AppendLine("     ,' ' AS CODE_NAME ");
            sb.AppendLine("     ,'0' AS NAIYO_1 ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     DUAL ");
            sb.AppendLine(" UNION ");
        }

        sb.AppendLine(" SELECT ");
        sb.AppendLine("       CODE_VALUE ");
        sb.AppendLine("      ,CODE_NAME ");
        sb.AppendLine("      ,NAIYO_1 ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_CODE ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CODE_BUNRUI = :BUNRUI ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     NVL(DELETE_DATE, 0) = 0 ");
        sb.AppendLine(" ) M_CODE ");
        sb.AppendLine(" ORDER BY TO_NUMBER (NAIYO_1), CODE_VALUE ");
        base.setParam("BUNRUI", codeBunrui);
        return sb.ToString();
    }

    /// <summary>
    /// 予約区分、予約番号取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約区分、予約番号取得SQL</returns>
    private string createYoyakuInfoSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        sb.AppendLine("     ,CRS_CD ");
        sb.AppendLine("     ,SYUPT_DAY ");
        sb.AppendLine("     ,GOUSYA ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 割引マスタ情報取得SQL作成
    /// </summary>
    /// <param name="entity">割引コードマスタ</param>
    /// <returns>割引マスタ情報取得SQL</returns>
    private string createWaribikiMasterDataSql(WaribikiCdMasterEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      APPLICATION_DAY_FROM ");
        sb.AppendLine("     ,APPLICATION_DAY_TO ");
        sb.AppendLine("     ,BIKO ");
        sb.AppendLine("     ,CARRIAGE_WARIBIKI_FLG ");
        sb.AppendLine("     ,CRS_KIND ");
        sb.AppendLine("     ,USE_KBN ");
        sb.AppendLine("     ,WARIBIKI_APPLICATION_NINZU ");
        sb.AppendLine("     ,WARIBIKI_CD ");
        sb.AppendLine("     ,CASE WARIBIKI_KBN WHEN '1' THEN '%' ");
        sb.AppendLine(@"                        WHEN '2' THEN '\' ");
        sb.AppendLine("      END AS WARIBIKI_KBN ");
        sb.AppendLine("     ,DECODE(WARIBIKI_KBN, '1', WARIBIKI_PER, WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ");
        sb.AppendLine("     ,CASE WHEN NVL(CARRIAGE_WARIBIKI_FLG, '0') <> '1' AND NVL(YOYAKU_WARIBIKI_FLG, '0') <> '1' THEN '当日' ");
        sb.AppendLine("      ELSE '予約時' END AS KBN ");
        sb.AppendLine("     ,WARIBIKI_NAME ");
        sb.AppendLine("     ,WARIBIKI_NAME_KANA ");
        sb.AppendLine("     ,WARIBIKI_TYPE_KBN ");
        sb.AppendLine("     ,YOYAKU_WARIBIKI_FLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_WARIBIKI_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     NVL(DELETE_DAY, 0) = 0 ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CRS_KIND = " + base.setParam(entity.crsKind.PhysicsName, entity.crsKind.Value, entity.crsKind.DBType, entity.crsKind.IntegerBu, entity.crsKind.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     " + base.setParam(entity.applicationDayFrom.PhysicsName, entity.applicationDayFrom.Value, entity.applicationDayFrom.DBType, entity.applicationDayFrom.IntegerBu, entity.applicationDayFrom.DecimalBu) + " BETWEEN APPLICATION_DAY_FROM AND DECODE(NVL(APPLICATION_DAY_TO, 0), 0, 99999999, APPLICATION_DAY_TO) ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     HOUJIN_GAIKYAKU_KBN = " + base.setParam(entity.houjinGaikyakuKbn.PhysicsName, entity.houjinGaikyakuKbn.Value, entity.houjinGaikyakuKbn.DBType, entity.houjinGaikyakuKbn.IntegerBu, entity.houjinGaikyakuKbn.DecimalBu));
        sb.AppendLine(" ORDER BY WARIBIKI_CD ");
        return sb.ToString();
    }

    /// <summary>
    /// オプション必須選択一覧SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（オプショングループ）Entity</param>
    /// <returns>オプション必須選択一覧SQL</returns>
    private string createRequiredSelectionListSql(CrsLedgerOptionGroupEntity entity)
    {
        string mainQuery = createOptionListSql();
        var whereQuery = new StringBuilder();
        whereQuery.AppendLine(" WHERE ");
        whereQuery.AppendLine("     COG.CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.SYUPT_DAY = " + base.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.GOUSYA = " + base.setParam("GOUSYA", entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.REQUIRED_KBN = " + base.setParam("REQUIRED_KBN", entity.requiredKbn.Value, entity.requiredKbn.DBType, entity.requiredKbn.IntegerBu, entity.requiredKbn.DecimalBu));
        whereQuery.AppendLine(" ORDER BY COG.GROUP_NO, CLO.LINE_NO ");
        string query = mainQuery + whereQuery.ToString();
        return query;
    }

    /// <summary>
    /// オプション任意選択一覧SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（オプショングループ）Entity</param>
    /// <returns>オプション任意選択一覧SQL</returns>
    private string createAnySelectionListSql(CrsLedgerOptionGroupEntity entity)
    {
        string mainQuery = createOptionListSql();
        var whereQuery = new StringBuilder();
        whereQuery.AppendLine(" WHERE ");
        whereQuery.AppendLine("     COG.CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.SYUPT_DAY = " + base.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.GOUSYA = " + base.setParam("GOUSYA", entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        whereQuery.AppendLine("     AND ");
        whereQuery.AppendLine("     COG.REQUIRED_KBN = " + base.setParam("REQUIRED_KBN", entity.requiredKbn.Value, entity.requiredKbn.DBType, entity.requiredKbn.IntegerBu, entity.requiredKbn.DecimalBu));
        whereQuery.AppendLine(" ORDER BY COG.GROUP_NO, CLO.LINE_NO ");
        string query = mainQuery + whereQuery.ToString();
        return query;
    }

    /// <summary>
    /// オプション一覧取得SQL作成
    /// </summary>
    /// <returns>オプション一覧取得SQL</returns>
    private string createOptionListSql()
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLO.PAYMENT_HOHO ");
        sb.AppendLine("     ,CASE CLO.PAYMENT_HOHO WHEN '1' THEN '事前' ELSE '当日' END AS PAYMENT_HOHO_NM ");
        sb.AppendLine("     ,COG.REQUIRED_KBN ");
        sb.AppendLine("     ,COG.GROUP_NO ");
        sb.AppendLine("     ,COG.OPTION_GROUP_NM ");
        sb.AppendLine("     ,COG.TANKA_KBN ");
        sb.AppendLine("     ,CASE COG.TANKA_KBN ");
        sb.AppendLine("          WHEN '1' THEN '人数単価' ");
        sb.AppendLine("          WHEN '2' THEN 'ルーム単価' ");
        sb.AppendLine("      ELSE '数量単価' END AS TANKA_KBN_NM ");
        sb.AppendLine("     ,CLO.LINE_NO ");
        sb.AppendLine("     ,CLO.OPTIONAL_NAME ");
        // sb.AppendLine("     ,''  AS ADD_CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,TRIM(TO_CHAR(CLO.HANBAI_TANKA, '9,999,999')) AS HANBAI_TANKA ");
        sb.AppendLine("     ,CLO.TAX_KBN ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_OPTION_GROUP COG ");
        sb.AppendLine(" INNER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_OPTION CLO ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     COG.CRS_CD = CLO.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.SYUPT_DAY = CLO.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.GOUSYA = CLO.GOUSYA ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.GROUP_NO = CLO.GROUP_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// 送付物マスタ取得SQL作成
    /// </summary>
    /// <returns>送付物マスタ取得SQL</returns>
    private string createSoufubutsuListSql()
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      SOFUBUTSU_CD ");
        sb.AppendLine("     ,SOFUBUTSU_NM ");
        sb.AppendLine("     ,LINE_NO ");
        sb.AppendLine("     ,'' AS OUTPUT_DATE ");
        // sb.AppendLine("     ,'' AS BIKO ")
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_SOFUBUTSU ");
        sb.AppendLine(" ORDER BY LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// DELETE SQL作成
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約NO</param>
    /// <param name="tableName">テーブル名</param>
    /// <returns>DELETE SQL</returns>
    private string createDeleteSql(string yoyakuKbn, int? yoyakuNo, string tableName)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(string.Format(" DELETE FROM {0}", tableName));
        if (yoyakuKbn == "W")
        {
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     MANAGEMENT_KBN =  " + base.setParam("MANAGEMENT_KBN", yoyakuKbn, OracleDbType.Char, 1, 0));
            sb.AppendLine(" AND ");
            sb.AppendLine("     MANAGEMENT_NO =  " + base.setParam("MANAGEMENT_NO", yoyakuNo, OracleDbType.Decimal, 9, 0));
        }
        else
        {
            sb.AppendLine(" WHERE ");
            sb.AppendLine("     YOYAKU_KBN =  " + base.setParam("YOYAKU_KBN", yoyakuKbn, OracleDbType.Char, 1, 0));
            sb.AppendLine(" AND ");
            sb.AppendLine("     YOYAKU_NO =  " + base.setParam("YOYAKU_NO", yoyakuNo, OracleDbType.Decimal, 9, 0));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 予約情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報取得SQL</returns>
    private string createYoyakuInfoDataSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLB.SYUPT_DAY ");
        sb.AppendLine("     ,CLB.CRS_CD ");
        sb.AppendLine("     ,CLB.CRS_NAME ");
        sb.AppendLine("     ,CLB.GOUSYA ");
        sb.AppendLine("     ,CLB.CRS_KIND ");
        sb.AppendLine("     ,CLB.CRS_KBN_1 ");
        sb.AppendLine("     ,CLB.CRS_KBN_2 ");
        sb.AppendLine("     ,CLB.TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     ,CLB.HOUJIN_GAIKYAKU_KBN ");
        sb.AppendLine("     ,CLB.SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     ,CLB.SAIKOU_DAY ");
        sb.AppendLine("     ,CLB.UNKYU_CONTACT_DAY ");
        sb.AppendLine("     ,CLB.RETURN_DAY ");
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ");
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_1 AS JYOCHACHI_CD_1 ");
        sb.AppendLine("     ,PL1.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_1 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_2 AS JYOCHACHI_CD_2 ");
        sb.AppendLine("     ,PL2.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_2 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_3 AS JYOCHACHI_CD_3 ");
        sb.AppendLine("     ,PL3.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_3 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_4 AS JYOCHACHI_CD_4 ");
        sb.AppendLine("     ,PL4.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_4 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_5 AS JYOCHACHI_CD_5 ");
        sb.AppendLine("     ,PL5.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_5 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_5 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_5 ");
        sb.AppendLine("     ,CLB.JYOSEI_SENYO_SEAT_FLG ");
        sb.AppendLine("     ,CLB.TYUIJIKOU ");
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ");
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ");
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ");
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ");
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ");
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,CLB.HURIKOMI_NG_FLG ");
        sb.AppendLine("     ,CLB.ONE_SANKA_FLG ");
        sb.AppendLine("     ,CLB.JYOSYA_CAPACITY ");
        sb.AppendLine("     ,CLB.TEIINSEI_FLG ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_ONE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_TWO_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_THREE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FOUR_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FIVE_1R ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_ONE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_TWO_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_THREE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FOUR_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FIVE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_SOKEI ");
        sb.AppendLine("     ,CLB.CANCEL_RYOU_KBN ");
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ");
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ");
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ");
        sb.AppendLine("     ,CLB.TEJIMAI_DAY ");
        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ");
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ");
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ");
        sb.AppendLine("     ,CLB.AGE_FLG ");
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ");
        sb.AppendLine("     ,CLB.TEL_FLG ");
        sb.AppendLine("     ,CLB.ADDRESS_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_STOP_FLG ");
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ");
        sb.AppendLine("     ,CLB.USING_FLG ");
        sb.AppendLine("     ,CLB.ZASEKI_HIHYOJI_FLG");
        sb.AppendLine("     ,CLB.TOJITU_KOKUCHI_FLG");
        sb.AppendLine("     ,YIB.YOYAKU_KBN ");
        sb.AppendLine("     ,YIB.YOYAKU_NO ");
        sb.AppendLine("     ,YIB.SEIKI_CHARGE_ALL_GAKU ");
        sb.AppendLine("     ,YIB.WARIBIKI_ALL_GAKU ");
        sb.AppendLine("     ,YIB.ZASEKI ");
        sb.AppendLine("     ,YIB.SEISAN_HOHO ");
        sb.AppendLine("     ,SEI.CODE_NAME AS SEISAN_HOHO_NM ");
        sb.AppendLine("     ,YIB.NYUKINGAKU_SOKEI ");
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ");
        sb.AppendLine("     ,YIB.SUB_SEAT_WAIT_FLG ");
        sb.AppendLine("     ,YIB.MOTO_YOYAKU_KBN ");
        sb.AppendLine("     ,YIB.MOTO_YOYAKU_NO ");
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_1 ");
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_2 ");
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_3 ");
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_4 ");
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_5 ");
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_1 ");
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_2 ");
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_3 ");
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_4 ");
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_5 ");
        sb.AppendLine("     ,YIB.INFANT_NINZU ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_1 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_2 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_3 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_4 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_5 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_6 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_7 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_8 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_9 ");
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_10 ");
        sb.AppendLine("     ,YIB.MEDIA_CD ");
        sb.AppendLine("     ,YIB.AGENT_CD ");
        sb.AppendLine("     ,YIB.AGENT_NAME_KANA ");
        sb.AppendLine("     ,YIB.AGENT_NM ");
        sb.AppendLine("     ,YIB.AGENT_SEISAN_KBN ");
        sb.AppendLine("     ,YIB.AGENT_TEL_NO ");
        sb.AppendLine("     ,YIB.AGENT_TANTOSYA ");
        sb.AppendLine("     ,YIB.TOURS_NO ");
        sb.AppendLine("     ,YIB.KOKUSEKI ");
        sb.AppendLine("     ,YIB.SEX_BETU ");
        sb.AppendLine("     ,YIB.SURNAME ");
        sb.AppendLine("     ,YIB.NAME ");
        sb.AppendLine("     ,YIB.SURNAME_KJ ");
        sb.AppendLine("     ,YIB.NAME_KJ ");
        sb.AppendLine("     ,YIB.TEL_NO_1 ");
        sb.AppendLine("     ,YIB.TEL_NO_2 ");
        sb.AppendLine("     ,YIB.MAIL_ADDRESS ");
        sb.AppendLine("     ,YIB.MAIL_SENDING_KBN ");
        sb.AppendLine("     ,YIB.YUBIN_NO ");
        sb.AppendLine("     ,YIB.ADDRESS_1 ");
        sb.AppendLine("     ,YIB.ADDRESS_2 ");
        sb.AppendLine("     ,YIB.ADDRESS_3 ");
        sb.AppendLine("     ,YIB.MOSHIKOMI_HOTEL_FLG ");
        sb.AppendLine("     ,YIB.YYKMKS ");
        sb.AppendLine("     ,YIB.FURIKOMIYOSHI_YOHI_FLG ");
        sb.AppendLine("     ,YIB.SEIKYUSYO_YOHI_FLG ");
        sb.AppendLine("     ,YIB.RELATION_YOYAKU_KBN ");
        sb.AppendLine("     ,YIB.RELATION_YOYAKU_NO ");
        sb.AppendLine("     ,YIB.ADD_CHARGE_MAEBARAI_KEI ");
        sb.AppendLine("     ,YIB.ADD_CHARGE_TOJITU_PAYMENT_KEI ");
        sb.AppendLine("     ,YIB.ENTRY_DAY ");
        sb.AppendLine("     ,YIB.YOYAKU_JI_AGENT_CD ");
        sb.AppendLine("     ,YIB.YOYAKU_JI_AGENT_NAME ");
        sb.AppendLine("     ,YIB.JYOSEI_SENYO ");
        sb.AppendLine("     ,YIB.AIBEYA_FLG ");
        sb.AppendLine("     ,YIB.HAKKEN_NAIYO ");
        sb.AppendLine("     ,YIB.CANCEL_FLG ");
        sb.AppendLine("     ,YIB.STATE ");
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_URIAGE ");
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_CANCEL "); // 
        sb.AppendLine("     ,YIB.YOYAKU_UKETUKE_KBN ");
        sb.AppendLine("     ,YIB.NYUUKIN_SITUATION_KBN ");
        sb.AppendLine("     ,YIB.CANCEL_FLG ");
        sb.AppendLine("     ,YIB.DELETE_DAY ");
        sb.AppendLine("     ,YIB.USING_FLG ");
        sb.AppendLine("     , NVL(IYTM.OPERATION_TAISHO_FLG, '') AS OPETAISHOFLG ");
        sb.AppendLine("     , NVL(IYTM.INVALID_FLG, '') AS INVFLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CLB.CRS_CD = YIB.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.SYUPT_DAY = YIB.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.GOUSYA = YIB.GOUSYA ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL1 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL1.PLACE_CD = CLB.HAISYA_KEIYU_CD_1 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL2 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL2.PLACE_CD = CLB.HAISYA_KEIYU_CD_2 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL3 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL3.PLACE_CD = CLB.HAISYA_KEIYU_CD_3 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL4 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL4.PLACE_CD = CLB.HAISYA_KEIYU_CD_4 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL5 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL5.PLACE_CD = CLB.HAISYA_KEIYU_CD_5 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE SEI ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     SEI.CODE_BUNRUI = '027' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     SEI.CODE_VALUE = YIB.SEISAN_HOHO ");
        sb.AppendLine("     LEFT OUTER JOIN T_YOYAKU_INFO_PICKUP YP ");                 // 予約情報（ピックアップ）
        sb.AppendLine("         ON  YP.YOYAKU_KBN = YIB.YOYAKU_KBN ");
        sb.AppendLine("         AND YP.YOYAKU_NO = YIB.YOYAKU_NO ");
        sb.AppendLine("     LEFT OUTER JOIN M_PICKUP_HOTEL PH ");                       // ピックアップホテルマスタ
        sb.AppendLine("         ON PH.PICKUP_HOTEL_CD = YP.PICKUP_HOTEL_CD ");
        sb.AppendLine("     LEFT OUTER JOIN ");
        sb.AppendLine("         T_INT_YOYAKU_TRAN_MANAGEMENT IYTM ");
        sb.AppendLine("     ON ");
        sb.AppendLine("         YIB.YOYAKU_KBN = IYTM.YOYAKU_KBN ");
        sb.AppendLine("         AND ");
        sb.AppendLine("         YIB.YOYAKU_NO = IYTM.YOYAKU_NO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIB.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIB.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約料金区分一覧取得SQL</returns>
    private string createYoyakuChargeKbnListSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CCK.KBN_NO AS KBN_NO_URA ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD_URA ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     ,CJK.SEX_BETU  ");
        sb.AppendLine("     ,CCK.CHARGE_KBN ");
        sb.AppendLine("     ,NVL(MCK.CHARGE_NAME, '-') AS KBN_NO ");
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ");
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ");
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ");
        sb.AppendLine("     ,CCK.CARRIAGE ");
        sb.AppendLine("     ,CCK.CHARGE_APPLICATION_NINZU_1 AS YOYAKU_NINZU ");
        sb.AppendLine("     ,CCK.TANKA_1 AS CHARGE ");
        sb.AppendLine("     ,CLC.CHARGE_SUB_SEAT ");
        sb.AppendLine("     ,CLC.CARRIAGE_SUB_SEAT ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine(" INNER JOIN ");
        sb.AppendLine("     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN CCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CCK.YOYAKU_KBN = YIB.YOYAKU_KBN ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CCK.YOYAKU_NO = YIB.YOYAKU_NO ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CLC  ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CLC.CRS_CD = YIB.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.SYUPT_DAY = YIB.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.GOUSYA = YIB.GOUSYA ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.KBN_NO = CCK.KBN_NO ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_KBN MCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MCK.CHARGE_KBN = CCK.CHARGE_KBN ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIB.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIB.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// メモ情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（メモ）Entity</param>
    /// <returns>メモ情報取得SQL</returns>
    private string createYoyakuMemoInfoListSql(YoyakuInfoMemoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YIM.SYSTEM_UPDATE_DAY ");
        sb.AppendLine("     ,YIM.MEMO_KBN ");
        sb.AppendLine("     ,KBN.CODE_NAME AS MEMO_KBN_NM ");
        sb.AppendLine("     ,YIM.MEMO_BUNRUI ");
        sb.AppendLine("     ,BUN.CODE_NAME AS MEMO_BUNRUI_NM ");
        sb.AppendLine("     ,YIM.NAIYO ");
        sb.AppendLine("     ,YIM.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SER.USER_NAME ");
        sb.AppendLine("     ,YIM.EDABAN ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_MEMO YIM ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER SER ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     SER.COMPANY_CD = '0001' ");
        sb.AppendLine(" AND ");
        sb.AppendLine("     SER.USER_ID = YIM.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE KBN ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     KBN.CODE_BUNRUI = '004' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     KBN.CODE_VALUE = YIM.MEMO_KBN ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE BUN ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     BUN.CODE_BUNRUI = '005' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     BUN.CODE_VALUE = YIM.MEMO_BUNRUI ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     NVL(YIM.DELETE_DATE, 0) = 0 ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIM.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIM.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine(" ORDER BY YIM.SYSTEM_UPDATE_DAY, YIM.EDABAN ");
        return sb.ToString();
    }

    /// <summary>
    /// 予約割引一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（割引）Entity</param>
    /// <returns>予約割引一覧取得SQL</returns>
    private string createYoyakuWaribikiListSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YIW.WARIBIKI_CD ");
        sb.AppendLine("     ,YIW.WARIBIKI_CD AS WARIBIKI_NAME ");
        sb.AppendLine("     ,CAST(YIW.KBN_NO AS VARCHAR2(3)) AS CHARGE_KBN ");
        sb.AppendLine("     ,YIW.CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     ,DECODE(YIW.WARIBIKI_KBN, '1', YIW.WARIBIKI_PER, YIW.WARIBIKI_KINGAKU) As WARIBIKI_KINGAKU ");
        sb.AppendLine("     ,CASE YIW.WARIBIKI_KBN WHEN '1' THEN '%' ");
        sb.AppendLine(@"                            WHEN '2' THEN '\' ");
        sb.AppendLine("      END AS WARIBIKI_KBN ");
        sb.AppendLine("     ,CASE WHEN NVL(YIW.CARRIAGE_WARIBIKI_FLG, '0') <> '1' AND NVL(YIW.YOYAKU_WARIBIKI_FLG, '0') <> '1' THEN '当日'  ");
        sb.AppendLine("      ELSE '予約時' END AS KBN ");
        sb.AppendLine("     ,NVL(YIW.WARIBIKI_APPLICATION_NINZU_1, 0) +  ");
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_2, 0) +  ");
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_3, 0) +  ");
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_4, 0) +  ");
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_5, 0) AS WARIBIKI_NINZU ");
        sb.AppendLine("     ,(NVL(YIW.WARIBIKI_TANKA_1, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_1, 0)) +  ");
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_2, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_2, 0)) +  ");
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_3, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_3, 0)) +  ");
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_4, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_4, 0)) +  ");
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_5, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_5, 0)) AS WARIBIKI_TOTAL_GAKU ");
        sb.AppendLine("     ,YIW.WARIBIKI_BIKO ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU ");
        sb.AppendLine("     ,YIW.CARRIAGE_WARIBIKI_FLG ");
        sb.AppendLine("     ,YIW.WARIBIKI_USE_FLG ");
        sb.AppendLine("     ,YIW.YOYAKU_WARIBIKI_FLG ");
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_1 ");
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_2 ");
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_3 ");
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_4 ");
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_5 ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_1 ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_2 ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_3 ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_4 ");
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_5 ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine(" INNER JOIN ");
        sb.AppendLine("     T_YOYAKU_INFO_WARIBIKI YIW ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YIW.YOYAKU_KBN = YIB.YOYAKU_KBN ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIW.YOYAKU_NO = YIB.YOYAKU_NO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIB.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIB.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約振込情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（振込）Entity</param>
    /// <returns>予約振込情報取得SQL</returns>
    private string createYoyakuHurikomiInfoSql(YoyakuInfoHurikomiEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YUBIN_NO_HURIKOMI_NIN ");
        sb.AppendLine("     ,ADDRESS_1_HURIKOMI_NIN ");
        sb.AppendLine("     ,ADDRESS_2_HURIKOMI_NIN ");
        sb.AppendLine("     ,ADDRESS_3_HURIKOMI_NIN ");
        sb.AppendLine("     ,NM_1_HURIKOMI_NIN ");
        sb.AppendLine("     ,NM_2_HURIKOMI_NIN ");
        sb.AppendLine("     ,DOUJOU_FLG_SOFU_SAKI ");
        sb.AppendLine("     ,YUBIN_NO_SOFU_SAKI ");
        sb.AppendLine("     ,ADDRESS_1_SOFU_SAKI ");
        sb.AppendLine("     ,ADDRESS_2_SOFU_SAKI ");
        sb.AppendLine("     ,ADDRESS_3_SOFU_SAKI ");
        sb.AppendLine("     ,NM_1_SOFU_SAKI ");
        sb.AppendLine("     ,NM_2_SOFU_SAKI ");
        sb.AppendLine("     ,DOUJOU_FLG_HURIKOMI_NIN ");
        sb.AppendLine("     ,SEIKYU_GAKU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_HURIKOMI ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約名簿一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（名簿）Entity</param>
    /// <returns>予約名簿一覧取得SQL</returns>
    private string createYoyakuMeiboListSql(YoyakuInfoMeiboEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      SURNAME ");
        sb.AppendLine("     ,NAME ");
        sb.AppendLine("     ,SEX_BETU ");
        sb.AppendLine("     ,TO_DATE(BIRTHDAY,'YYYYMMDD') AS BIRTHDAY ");
        sb.AppendLine("     ,TRUNC((TO_CHAR(SYSDATE, 'YYYYMMDD') - TO_CHAR(TO_DATE(BIRTHDAY, 'YYYY/MM/DD'), 'YYYYMMDD'))/10000) AS AGE ");
        sb.AppendLine("     ,TEL_NO_1　AS TEL_NO ");
        sb.AppendLine("     ,YUBIN_NO ");
        sb.AppendLine("     ,ADDRESS_1 ");
        sb.AppendLine("     ,ADDRESS_2 ");
        sb.AppendLine("     ,ADDRESS_3 ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_MEIBO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約送付物一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（送付物）Entity</param>
    /// <returns>予約送付物一覧取得SQL</returns>
    private string createYoyakuSofubutsuListSql(YoyakuInfoSofubutsuEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      MSO.SOFUBUTSU_CD ");
        sb.AppendLine("     ,MSO.SOFUBUTSU_NM ");
        sb.AppendLine("     ,CASE MSO.SOFUBUTSU_CD  ");
        sb.AppendLine("           WHEN '01' THEN (SELECT TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME) FROM T_SEIKYUSYO_ISSUE ");
        sb.AppendLine("                          WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                            AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                     ) ");
        sb.AppendLine("           WHEN '02' THEN (SELECT OUTDATE FROM (SELECT TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME) AS OUTDATE FROM T_FURIKOMIHYO_CREATE_HISTORY ");
        sb.AppendLine("                                                   WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                                                     AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                                 ORDER BY UPDATE_DAY DESC, UPDATE_TIME DESC) WHERE rownum <= 1) ");
        sb.AppendLine("           WHEN '03' THEN (SELECT OUTDATE FROM (SELECT (TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME)) AS OUTDATE FROM T_RECEIPT ");
        sb.AppendLine("                                                 WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                                                   AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                                 ORDER BY PRINT_NLGSQN DESC) WHERE rownum <= 1) ");
        sb.AppendLine("           WHEN '04' THEN (SELECT OUTDATE FROM (SELECT (TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME)) AS OUTDATE FROM T_ZASEKI_RESERVE_KEN_HIKIKAESHO_OUT ");
        sb.AppendLine("                                                 WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                                                   AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                                 ORDER BY PROCESS_DAY DESC) WHERE rownum <= 1) ");
        sb.AppendLine("           WHEN '05' THEN (SELECT CASE NVL(YOYAKU_KAKUNIN_DAY, 0) WHEN 0 THEN '' ELSE TO_CHAR(TO_DATE(TO_CHAR(YOYAKU_KAKUNIN_DAY), 'yyyyMMdd'), 'yyyy/MM/dd') END FROM T_YOYAKU_INFO_BASIC ");
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                              AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                     ) ");
        sb.AppendLine("           WHEN '06' THEN (SELECT CASE NVL(SAIKOU_KAKUTEI_GUIDE_OUT_DAY, 0) WHEN 0 THEN '' ELSE TO_CHAR(TO_DATE(TO_CHAR(SAIKOU_KAKUTEI_GUIDE_OUT_DAY), 'yyyyMMdd'), 'yyyy/MM/dd') END FROM T_YOYAKU_INFO_BASIC ");
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                              AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                     ) ");
        sb.AppendLine("           WHEN '07' THEN (SELECT TO_CHAR(SYSTEM_UPDATE_DAY) FROM T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT ");
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("                              AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("                                     ) ");
        sb.AppendLine("      END AS OUTPUT_DATE ");
        sb.AppendLine("     ,(SELECT BIKO FROM T_YOYAKU_INFO_SOFUBUTSU YIS ");
        sb.AppendLine("       WHERE YIS.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("         AND YIS.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("         AND YIS.SOFUBUTSU_CD = MSO.SOFUBUTSU_CD ");
        sb.AppendLine("      ) AS BIKO  ");
        sb.AppendLine("     ,MSO.LINE_NO ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_SOFUBUTSU MSO ");
        sb.AppendLine(" ORDER BY MSO.LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// 予約送付先一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（送付先）Entity</param>
    /// <returns>予約送付先一覧取得SQL</returns>
    private string createYoyakuSofusakiListSql(YoyakuInfoSofusakiEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YIS.AGENT_CD ");
        sb.AppendLine("     ,AGT.COMPANY_NAME ");
        sb.AppendLine("     ,AGT.BRANCH_NAME ");
        sb.AppendLine("     ,YIS.NAME_BF ");
        sb.AppendLine("     ,YIS.YUBIN_NO ");
        sb.AppendLine("     ,YIS.ADDRESS_1 ");
        sb.AppendLine("     ,YIS.ADDRESS_2 ");
        sb.AppendLine("     ,YIS.ADDRESS_3 ");
        sb.AppendLine("     ,YIS.NM_1 ");
        sb.AppendLine("     ,YIS.NM_2 ");
        sb.AppendLine("     ,YIS.SOFU_BUSU ");
        sb.AppendLine("     ,YIS.LINE_NO ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_SOFUSAKI YIS ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_AGENT AGT ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     AGT.AGENT_CD = YIS.AGENT_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIS.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIS.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine(" ORDER BY YIS.LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// 入返金一覧取得SQL作成
    /// </summary>
    /// <param name="entity">入返金情報Entity</param>
    /// <returns>入返金一覧取得SQL</returns>
    private string createNyuhenkinListSql(NyuukinInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      PROCESS_DATE ");
        sb.AppendLine("     ,CASE HAKKEN_HURIKOMI_KBN WHEN '3' THEN NYUUKIN_GAKU_1 ");
        sb.AppendLine("                               WHEN '7' THEN NYUUKIN_GAKU_2 ");
        sb.AppendLine("      END AS NYUKIN ");
        sb.AppendLine("     ,CASE HAKKEN_HURIKOMI_KBN WHEN '4' THEN NYUUKIN_GAKU_1 ");
        sb.AppendLine("                               WHEN '8' THEN NYUUKIN_GAKU_2 ");
        sb.AppendLine("      END AS HENKIN ");
        sb.AppendLine("     ,HURIKOMI_SAKI_KOZA_NAME ");
        sb.AppendLine("     ,CASE WHEN HAKKEN_HURIKOMI_KBN = '3' OR HAKKEN_HURIKOMI_KBN = '7' ");
        sb.AppendLine("                THEN CASE HURIKOMI_KBN WHEN 'G' THEN '銀行' ");
        sb.AppendLine("                                       WHEN 'Y' THEN '郵便局' ");
        sb.AppendLine("                                       WHEN 'K' THEN 'コンビニ' ");
        sb.AppendLine("                     END ");
        sb.AppendLine("           WHEN HAKKEN_HURIKOMI_KBN = '4' OR HAKKEN_HURIKOMI_KBN = '8' ");
        sb.AppendLine("                THEN CASE HURIKOMI_KBN WHEN 'G' THEN '振込' ");
        sb.AppendLine("                                       WHEN 'Y' THEN '書留' ");
        sb.AppendLine("                                       WHEN 'K' THEN '窓口' ");
        sb.AppendLine("                     END ");
        sb.AppendLine("      END AS HURIKOMI_KBN ");
        sb.AppendLine("     ,NICOS_UKETUKE_NO ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_NYUUKIN_INFO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine(" ORDER BY SEQ ");
        return sb.ToString();
    }

    /// <summary>
    /// 予約オプション一覧取得SQL作成
    /// </summary>
    /// <param name="crsEntity">コース台帳（オプション）Entity</param>
    /// <param name="yoyakuentity">予約情報（オプション）Entity</param>
    /// <returns>予約オプション一覧取得SQL</returns>
    private string createYoyakuOptionListSql(CrsLedgerOptionGroupEntity crsEntity, YoyakuInfoOptionEntity yoyakuentity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLO.PAYMENT_HOHO ");
        sb.AppendLine("     ,CASE CLO.PAYMENT_HOHO WHEN '1' THEN '事前' ELSE '当日' END AS PAYMENT_HOHO_NM ");
        sb.AppendLine("     ,COG.REQUIRED_KBN ");
        sb.AppendLine("     ,COG.GROUP_NO ");
        sb.AppendLine("     ,COG.OPTION_GROUP_NM ");
        sb.AppendLine("     ,COG.TANKA_KBN ");
        sb.AppendLine("     ,CASE COG.TANKA_KBN ");
        sb.AppendLine("          WHEN '1' THEN '人数単価' ");
        sb.AppendLine("          WHEN '2' THEN 'ルーム単価' ");
        sb.AppendLine("      ELSE '数量単価' END AS TANKA_KBN_NM ");
        sb.AppendLine("     ,CLO.LINE_NO ");
        sb.AppendLine("     ,CLO.OPTIONAL_NAME ");
        sb.AppendLine("     ,TRIM(TO_CHAR(CLO.HANBAI_TANKA, '9,999,999')) AS HANBAI_TANKA ");
        sb.AppendLine("     ,CLO.TAX_KBN ");
        sb.AppendLine("     ,YIO.ADD_CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("     ,YIO.EXISTS_YOYAKU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_OPTION_GROUP COG ");
        sb.AppendLine(" INNER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_OPTION CLO ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     COG.CRS_CD = CLO.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.SYUPT_DAY = CLO.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.GOUSYA = CLO.GOUSYA ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.GROUP_NO = CLO.GROUP_NO ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     (SELECT ");
        sb.AppendLine("          YOYAKU_KBN ");
        sb.AppendLine("         ,YOYAKU_NO ");
        sb.AppendLine("         ,GROUP_NO ");
        sb.AppendLine("         ,LINE_NO ");
        sb.AppendLine("         ,ADD_CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("         ,'1' AS EXISTS_YOYAKU ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("         T_YOYAKU_INFO_OPTION ");
        sb.AppendLine("     WHERE ");
        sb.AppendLine("         YOYAKU_KBN = " + base.setParam(yoyakuentity.yoyakuKbn.PhysicsName, yoyakuentity.yoyakuKbn.Value, yoyakuentity.yoyakuKbn.DBType, yoyakuentity.yoyakuKbn.IntegerBu, yoyakuentity.yoyakuKbn.DecimalBu));
        sb.AppendLine("         AND ");
        sb.AppendLine("         YOYAKU_NO = " + base.setParam(yoyakuentity.yoyakuNo.PhysicsName, yoyakuentity.yoyakuNo.Value, yoyakuentity.yoyakuNo.DBType, yoyakuentity.yoyakuNo.IntegerBu, yoyakuentity.yoyakuNo.DecimalBu));
        sb.AppendLine("     ) YIO ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YIO.GROUP_NO = COG.GROUP_NO ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIO.LINE_NO = CLO.LINE_NO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     COG.CRS_CD = " + base.setParam("CRS_CD", crsEntity.crsCd.Value, crsEntity.crsCd.DBType, crsEntity.crsCd.IntegerBu, crsEntity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.SYUPT_DAY = " + base.setParam("SYUPT_DAY", crsEntity.syuptDay.Value, crsEntity.syuptDay.DBType, crsEntity.syuptDay.IntegerBu, crsEntity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.GOUSYA = " + base.setParam("GOUSYA", crsEntity.gousya.Value, crsEntity.gousya.DBType, crsEntity.gousya.IntegerBu, crsEntity.gousya.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     COG.REQUIRED_KBN = " + base.setParam("REQUIRED_KBN", crsEntity.requiredKbn.Value, crsEntity.requiredKbn.DBType, crsEntity.requiredKbn.IntegerBu, crsEntity.requiredKbn.DecimalBu));
        sb.AppendLine(" ORDER BY COG.REQUIRED_KBN, COG.GROUP_NO, CLO.LINE_NO ");
        return sb.ToString();
    }

    /// <summary>
    /// ホリデーT管理（コースコード）取得SQL作成
    /// </summary>
    /// <param name="entity">ホリデーT管理（コースコード）Entity</param>
    /// <returns>ホリデーT管理（コースコード）取得SQL</returns>
    private string createHolidayCrsSql(HolidayCrsCdEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CRS_CD ");
        sb.AppendLine("     ,DELETE_DAY ");
        sb.AppendLine("     ,ENTRY_DAY ");
        sb.AppendLine("     ,ENTRY_PERSON_CD ");
        sb.AppendLine("     ,ENTRY_PGMID ");
        sb.AppendLine("     ,ENTRY_TIME ");
        sb.AppendLine("     ,UPDATE_DAY ");
        sb.AppendLine("     ,UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UPDATE_PGMID ");
        sb.AppendLine("     ,UPDATE_TIME ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_HOLIDAY_CRS_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        return sb.ToString();
    }
	
	private string createHolidayApplicationDaySql(HolidayApplicationDayEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      APPLICATION_DAY_FROM ");
        sb.AppendLine("     ,APPLICATION_DAY_TO ");
        sb.AppendLine("     ,DELETE_DAY ");
        sb.AppendLine("     ,ENTRY_DAY ");
        sb.AppendLine("     ,ENTRY_PERSON_CD ");
        sb.AppendLine("     ,ENTRY_PGMID ");
        sb.AppendLine("     ,ENTRY_TIME ");
        sb.AppendLine("     ,UPDATE_DAY ");
        sb.AppendLine("     ,UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UPDATE_PGMID ");
        sb.AppendLine("     ,UPDATE_TIME ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_HOLIDAY_APPLICATION_DAY ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     APPLICATION_DAY_FROM <= " + base.setParam(entity.applicationDayFrom.PhysicsName, entity.applicationDayFrom.Value, entity.applicationDayFrom.DBType, entity.applicationDayFrom.IntegerBu, entity.applicationDayFrom.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     APPLICATION_DAY_TO >= " + base.setParam(entity.applicationDayTo.PhysicsName, entity.applicationDayTo.Value, entity.applicationDayTo.DBType, entity.applicationDayTo.IntegerBu, entity.applicationDayTo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約料金区分一覧取得SQL</returns>
    private string createYoyakuChargeListSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT  ");
        sb.AppendLine("      CLC.CHARGE_KBN  ");
        sb.AppendLine("     ,CLC.KBN_NO  ");
        sb.AppendLine("     ,MCK.CHARGE_NAME  ");
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG  ");
        sb.AppendLine("     ,MCK.STAY_ADD_FLG  ");
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG  ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD  ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME  ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME_RK  ");
        sb.AppendLine("     ,CJK.SEX_BETU  ");
        sb.AppendLine("     ,CCK.CHARGE  ");
        sb.AppendLine("     ,CCK.CHARGE_SUB_SEAT  ");
        sb.AppendLine("     ,CCK.CARRIAGE  ");
        sb.AppendLine("     ,CCK.CARRIAGE_SUB_SEAT  ");
        sb.AppendLine("     ,CCK.CHARGE_1  ");
        sb.AppendLine("     ,CCK.CHARGE_2  ");
        sb.AppendLine("     ,CCK.CHARGE_3  ");
        sb.AppendLine("     ,CCK.CHARGE_4  ");
        sb.AppendLine("     ,CCK.CHARGE_5  ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("     ,YCK.TANKA_1 ");
        sb.AppendLine("     ,YCK.TANKA_2 ");
        sb.AppendLine("     ,YCK.TANKA_3 ");
        sb.AppendLine("     ,YCK.TANKA_4 ");
        sb.AppendLine("     ,YCK.TANKA_5 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU_1 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU_2 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU_3 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU_4 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU_5 ");
        sb.AppendLine("     ,YCK.CANCEL_NINZU ");
        sb.AppendLine(" FROM  ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE CLC  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     CCK.CRS_CD = CLC.CRS_CD  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.SYUPT_DAY = CLC.SYUPT_DAY  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.GOUSYA = CLC.GOUSYA  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.KBN_NO = CLC.KBN_NO  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     M_CHARGE_KBN MCK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     MCK.CHARGE_KBN = CLC.CHARGE_KBN  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     (SELECT ");
        sb.AppendLine("          KBN_NO ");
        sb.AppendLine("         ,CHARGE_KBN_JININ_CD ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("         ,TANKA_1 ");
        sb.AppendLine("         ,TANKA_2 ");
        sb.AppendLine("         ,TANKA_3 ");
        sb.AppendLine("         ,TANKA_4 ");
        sb.AppendLine("         ,TANKA_5 ");
        sb.AppendLine("         ,CANCEL_NINZU_1 ");
        sb.AppendLine("         ,CANCEL_NINZU_2 ");
        sb.AppendLine("         ,CANCEL_NINZU_3 ");
        sb.AppendLine("         ,CANCEL_NINZU_4 ");
        sb.AppendLine("         ,CANCEL_NINZU_5 ");
        sb.AppendLine("         ,CANCEL_NINZU ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("         T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine("     LEFT OUTER JOIN ");
        sb.AppendLine("         T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN CCK ");
        sb.AppendLine("     ON ");
        sb.AppendLine("         YIB.YOYAKU_KBN = CCK.YOYAKU_KBN ");
        sb.AppendLine("         AND ");
        sb.AppendLine("         YIB.YOYAKU_NO =CCK.YOYAKU_NO ");
        sb.AppendLine("         WHERE ");
        sb.AppendLine("         YIB.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("         AND ");
        sb.AppendLine("         YIB.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        sb.AppendLine("     ) YCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YCK.KBN_NO = CLC.KBN_NO  ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     YCK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" WHERE  ");
        sb.AppendLine("     CLC.CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CLC.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CLC.GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" ORDER BY CLC.KBN_NO, CCK.CHARGE_KBN_JININ_CD  ");
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報使用中フラグ更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報使用中フラグ更新SQ</returns>
    private string createYoyakuUsingFlagUpdateSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("     USING_FLG = '' ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報使用中フラグ更新SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報使用中フラグ更新SQ</returns>
    private string createYoyakuUsingFlagOnUpdateSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("     USING_FLG = 'Y' ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報使用中フラグ取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報使用中フラグ取得SQL</returns>
    private string createYoyakuUsingFlagSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("     USING_FLG ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
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
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 部屋残数更新SQL作成
    /// </summary>
    /// <param name="crsZasekiData">コース情報検索条件群</param>
    /// <param name="zasekiData">座席情報</param>
    /// <param name="yoyakuEntity">予約情報（基本）Entity</param>
    /// <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    /// <param name="isTorikeshi">取消フラグ</param>
    /// <returns>部屋残数更新SQL</returns>
    private string createCrsRoomUpdateSql(Hashtable crsZasekiData, DataTable zasekiData, YoyakuInfoBasicEntity yoyakuEntity, CrsLedgerBasicEntity crsInfoBasicEntity, bool isTorikeshi)
    {
        int syuptDay = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["SYUPT_DAY"]);
        var shoriDt = DateTime.Parse(crsZasekiData["SYSTEM_UPDATE_DAY"].ToString());
        int shoribi = CommonRegistYoyaku.convertObjectToInteger(shoriDt.ToString("yyyyMMdd"));
        string query = CommonRegistYoyaku.ValueEmpty;

        // 出発日がと当日の場合
        if (syuptDay == shoribi)
        {
            return query;
        }

        // 定員制フラグが空以外の場合
        if (zasekiData is object && string.IsNullOrEmpty(zasekiData.Rows(0)("TEIINSEI_FLG").ToString()) == false)
        {
            return query;
        }

        if (string.IsNullOrEmpty(yoyakuEntity.aibeyaFlg.Value) == true)
        {
            // 相部屋以外の場合

            query = createCrsRoomAibeyaNashiUpdateSql(crsZasekiData, zasekiData, yoyakuEntity, isTorikeshi);
        }
        else
        {
            // 相部屋の場合

            query = createCrsRoomAibeyaAriUpdateSql(crsZasekiData, zasekiData, yoyakuEntity, crsInfoBasicEntity, isTorikeshi);
        }

        return query;
    }

    /// <summary>
    /// 部屋残数更新SQL作成
    /// </summary>
    /// <param name="crsZasekiData">コース台帳検索条件群</param>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="yoyakuEntity">予約情報（基本）Entity</param>
    /// <param name="isTorikeshi">取消フラグ</param>
    /// <returns>部屋残数更新SQL</returns>
    private string createCrsRoomAibeyaNashiUpdateSql(Hashtable crsZasekiData, DataTable zasekiData, YoyakuInfoBasicEntity yoyakuEntity, bool isTorikeshi)
    {
        var entity = new CrsLedgerBasicEntity();
        entity.crsCd.Value = crsZasekiData["CRS_CD"].ToString();
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["SYUPT_DAY"]);
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["GOUSYA"]);
        entity.roomZansuOneRoom.Value = this.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu1.Value, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi);
        entity.roomZansuTwoRoom.Value = this.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu2.Value, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi);
        entity.roomZansuThreeRoom.Value = this.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu3.Value, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi);
        entity.roomZansuFourRoom.Value = this.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu4.Value, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi);
        entity.roomZansuFiveRoom.Value = this.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu5.Value, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi);

        // 予約部屋数総数算出
        int totalRoomsu = calcTotalYoyakuRoomsu(yoyakuEntity);
        if (isTorikeshi)
        {
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) - totalRoomsu;
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) + totalRoomsu;
        }
        else
        {
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) + totalRoomsu;
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) - totalRoomsu;
        }

        // 部屋残数算出
        entity.roomZansuOneRoom.Value = this.calcRoomZansu(zasekiData, "CRS_BLOCK_ONE_1R", entity.roomZansuOneRoom.Value, entity.roomZansuSokei.Value);
        entity.roomZansuTwoRoom.Value = this.calcRoomZansu(zasekiData, "CRS_BLOCK_TWO_1R", entity.roomZansuTwoRoom.Value, entity.roomZansuSokei.Value);
        entity.roomZansuThreeRoom.Value = this.calcRoomZansu(zasekiData, "CRS_BLOCK_THREE_1R", entity.roomZansuThreeRoom.Value, entity.roomZansuSokei.Value);
        entity.roomZansuFourRoom.Value = this.calcRoomZansu(zasekiData, "CRS_BLOCK_FOUR_1R", entity.roomZansuFourRoom.Value, entity.roomZansuSokei.Value);
        entity.roomZansuFiveRoom.Value = this.calcRoomZansu(zasekiData, "CRS_BLOCK_FIVE_1R", entity.roomZansuFiveRoom.Value, entity.roomZansuSokei.Value);
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      ROOM_ZANSU_ONE_ROOM =  " + base.setParam(entity.roomZansuOneRoom.PhysicsName, entity.roomZansuOneRoom.Value, entity.roomZansuOneRoom.DBType, entity.roomZansuOneRoom.IntegerBu, entity.roomZansuOneRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + base.setParam(entity.roomZansuTwoRoom.PhysicsName, entity.roomZansuTwoRoom.Value, entity.roomZansuTwoRoom.DBType, entity.roomZansuTwoRoom.IntegerBu, entity.roomZansuTwoRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + base.setParam(entity.roomZansuThreeRoom.PhysicsName, entity.roomZansuThreeRoom.Value, entity.roomZansuThreeRoom.DBType, entity.roomZansuThreeRoom.IntegerBu, entity.roomZansuThreeRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + base.setParam(entity.roomZansuFourRoom.PhysicsName, entity.roomZansuFourRoom.Value, entity.roomZansuFourRoom.DBType, entity.roomZansuFourRoom.IntegerBu, entity.roomZansuFourRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + base.setParam(entity.roomZansuFiveRoom.PhysicsName, entity.roomZansuFiveRoom.Value, entity.roomZansuFiveRoom.DBType, entity.roomZansuFiveRoom.IntegerBu, entity.roomZansuFiveRoom.DecimalBu));
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + base.setParam(entity.yoyakuAlreadyRoomNum.PhysicsName, entity.yoyakuAlreadyRoomNum.Value, entity.yoyakuAlreadyRoomNum.DBType, entity.yoyakuAlreadyRoomNum.IntegerBu, entity.yoyakuAlreadyRoomNum.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + base.setParam(entity.roomZansuSokei.PhysicsName, entity.roomZansuSokei.Value, entity.roomZansuSokei.DBType, entity.roomZansuSokei.IntegerBu, entity.roomZansuSokei.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY =  " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA =  " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 部屋残数算出
    /// </summary>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="roomingBetuNinzu">予約部屋数</param>
    /// <param name="crsBlockColName">コースブロックカラム名</param>
    /// <param name="roomZansuColName">部屋残数カラム名</param>
    /// <param name="isTorikeshi">取消フラグ</param>
    /// <returns>部屋残数</returns>
    private int calcCrsRoomAibeyaNashi(DataTable zasekiData, int? roomingBetuNinzu, string crsBlockColName, string roomZansuColName, bool isTorikeshi)
    {
        int crsBlockRoomSu = CommonRegistYoyaku.ZERO;
        int crsRoomZanSu = CommonRegistYoyaku.ZERO;
        int.TryParse(zasekiData.Rows(0)(crsBlockColName).ToString(), crsBlockRoomSu);
        int.TryParse(zasekiData.Rows(0)(roomZansuColName).ToString(), crsRoomZanSu);
        if (crsBlockRoomSu == CommonRegistYoyaku.ZERO)
        {
            // コースブロック管理されてない部屋残数は、変動なし
            return crsRoomZanSu;
        }

        int fugoHanten = -1;
        if (isTorikeshi)
        {
            fugoHanten = 1;
        }

        int yoyakuRoomsu = CommonRegistYoyaku.convertObjectToInteger(roomingBetuNinzu);
        int roomZansu = crsRoomZanSu + yoyakuRoomsu * fugoHanten;
        return roomZansu;
    }

    /// <summary>
    /// 予約部屋数総数算出
    /// </summary>
    /// <param name="yoyakuEntity">予約情報（基本）Entity</param>
    /// <returns>予約部屋数総数</returns>
    private int calcTotalYoyakuRoomsu(YoyakuInfoBasicEntity yoyakuEntity)
    {
        int yoyakuRoom1 = 0;
        int yoyakuRoom2 = 0;
        int yoyakuRoom3 = 0;
        int yoyakuRoom4 = 0;
        int yoyakuRoom5 = 0;
        int.TryParse(yoyakuEntity.roomingBetuNinzu1.Value.ToString(), yoyakuRoom1);
        int.TryParse(yoyakuEntity.roomingBetuNinzu2.Value.ToString(), yoyakuRoom2);
        int.TryParse(yoyakuEntity.roomingBetuNinzu3.Value.ToString(), yoyakuRoom3);
        int.TryParse(yoyakuEntity.roomingBetuNinzu4.Value.ToString(), yoyakuRoom4);
        int.TryParse(yoyakuEntity.roomingBetuNinzu5.Value.ToString(), yoyakuRoom5);
        int totalYoyakuRoom = yoyakuRoom1 + yoyakuRoom2 + yoyakuRoom3 + yoyakuRoom4 + yoyakuRoom5;
        return totalYoyakuRoom;
    }

    /// <summary>
    /// 部屋残数算出
    /// </summary>
    /// <param name="zasekiData">座席情報</param>
    /// <param name="crsBlockColName">コースブロックカラム名</param>
    /// <param name="roomZansu">部屋残数</param>
    /// <param name="roomZansuSoukei">部屋残数総計</param>
    /// <returns></returns>
    private int calcRoomZansu(DataTable zasekiData, string crsBlockColName, int? roomZansu, int? roomZansuSoukei)
    {

        // コースブロック数
        int crsBlockRoomSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName));
        if (crsBlockRoomSu == CommonRegistYoyaku.ZERO)
        {
            // コースブロック管理されてない部屋残数は、特になし
            return (int)roomZansu;
        }

        roomZansu = CommonRegistYoyaku.convertObjectToInteger(roomZansu);
        roomZansuSoukei = CommonRegistYoyaku.convertObjectToInteger(roomZansuSoukei);
        if (roomZansu > roomZansuSoukei == true)
        {
            // 部屋残数が部屋残数総計を超えたい場合、部屋残数を部屋残数総計にする
            roomZansu = roomZansuSoukei;
        }

        return (int)roomZansu;
    }

    /// <summary>
    /// 部屋残数更新SQL作成
    /// </summary>
    /// <param name="crsZasekiData">コース台帳検索条件群</param>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="yoyakuEntity">予約情報（基本）Entity</param>
    /// <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    /// <param name="isTorikeshi">取消フラグ</param>
    /// <returns>部屋残数更新SQL</returns>
    private string createCrsRoomAibeyaAriUpdateSql(Hashtable crsZasekiData, DataTable zasekiData, YoyakuInfoBasicEntity yoyakuEntity, CrsLedgerBasicEntity crsInfoBasicEntity, bool isTorikeshi)
    {

        // ROOM MAX定員数取得
        double roomMaxCap = getRoomMaxTein(crsZasekiData);

        // 男性
        double aibeyaManNinzu = double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE").ToString());
        double w1rom1 = Math.Ceiling(aibeyaManNinzu / roomMaxCap);
        double yoyakuManNinzu = double.Parse(crsInfoBasicEntity.aibeyaYoyakuNinzuMale.Value.ToString());
        aibeyaManNinzu = aibeyaManNinzu + yoyakuManNinzu;
        double w1rom2 = Math.Ceiling(aibeyaManNinzu / roomMaxCap);
        int manKagenRoomSu = (int)Math.Round(w1rom2 - w1rom1);

        // 女性
        double aibeyaWoManNinzu = double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI").ToString());
        w1rom1 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap);
        double yoyakuWomanNinzu = double.Parse(crsInfoBasicEntity.aibeyaYoyakuNinzuJyosei.Value.ToString());
        aibeyaWoManNinzu = aibeyaWoManNinzu + yoyakuWomanNinzu;
        w1rom2 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap);
        int womanKagenRoomSu = (int)Math.Round(w1rom2 - w1rom1);
        var entity = new CrsLedgerBasicEntity();
        entity.crsCd.Value = crsZasekiData["CRS_CD"].ToString();
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["SYUPT_DAY"]);
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData["GOUSYA"]);
        if (isTorikeshi)
        {
            entity.aibeyaYoyakuNinzuMale.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE")) - CommonRegistYoyaku.convertObjectToInteger(aibeyaManNinzu);
            entity.aibeyaYoyakuNinzuJyosei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI")) - CommonRegistYoyaku.convertObjectToInteger(aibeyaWoManNinzu);
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) + manKagenRoomSu + womanKagenRoomSu;
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) - manKagenRoomSu - womanKagenRoomSu;
            entity.roomZansuOneRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi);
            entity.roomZansuTwoRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi);
            entity.roomZansuThreeRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi);
            entity.roomZansuFourRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi);
            entity.roomZansuFiveRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi);
        }
        else
        {
            entity.aibeyaYoyakuNinzuMale.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE")) + CommonRegistYoyaku.convertObjectToInteger(aibeyaManNinzu);
            entity.aibeyaYoyakuNinzuJyosei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI")) + CommonRegistYoyaku.convertObjectToInteger(aibeyaWoManNinzu);
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) - manKagenRoomSu - womanKagenRoomSu;
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) + manKagenRoomSu + womanKagenRoomSu;
            entity.roomZansuOneRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi);
            entity.roomZansuTwoRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi);
            entity.roomZansuThreeRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi);
            entity.roomZansuFourRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi);
            entity.roomZansuFiveRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi);
        }

        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      AIBEYA_YOYAKU_NINZU_MALE =  " + base.setParam(entity.aibeyaYoyakuNinzuMale.PhysicsName, entity.aibeyaYoyakuNinzuMale.Value, entity.aibeyaYoyakuNinzuMale.DBType, entity.aibeyaYoyakuNinzuMale.IntegerBu, entity.aibeyaYoyakuNinzuMale.DecimalBu));
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI =  " + base.setParam(entity.aibeyaYoyakuNinzuJyosei.PhysicsName, entity.aibeyaYoyakuNinzuJyosei.Value, entity.aibeyaYoyakuNinzuJyosei.DBType, entity.aibeyaYoyakuNinzuJyosei.IntegerBu, entity.aibeyaYoyakuNinzuJyosei.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM =  " + base.setParam(entity.roomZansuOneRoom.PhysicsName, entity.roomZansuOneRoom.Value, entity.roomZansuOneRoom.DBType, entity.roomZansuOneRoom.IntegerBu, entity.roomZansuOneRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + base.setParam(entity.roomZansuTwoRoom.PhysicsName, entity.roomZansuTwoRoom.Value, entity.roomZansuTwoRoom.DBType, entity.roomZansuTwoRoom.IntegerBu, entity.roomZansuTwoRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + base.setParam(entity.roomZansuThreeRoom.PhysicsName, entity.roomZansuThreeRoom.Value, entity.roomZansuThreeRoom.DBType, entity.roomZansuThreeRoom.IntegerBu, entity.roomZansuThreeRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + base.setParam(entity.roomZansuFourRoom.PhysicsName, entity.roomZansuFourRoom.Value, entity.roomZansuFourRoom.DBType, entity.roomZansuFourRoom.IntegerBu, entity.roomZansuFourRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + base.setParam(entity.roomZansuFiveRoom.PhysicsName, entity.roomZansuFiveRoom.Value, entity.roomZansuFiveRoom.DBType, entity.roomZansuFiveRoom.IntegerBu, entity.roomZansuFiveRoom.DecimalBu));
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + base.setParam(entity.yoyakuAlreadyRoomNum.PhysicsName, entity.yoyakuAlreadyRoomNum.Value, entity.yoyakuAlreadyRoomNum.DBType, entity.yoyakuAlreadyRoomNum.IntegerBu, entity.yoyakuAlreadyRoomNum.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + base.setParam(entity.roomZansuSokei.PhysicsName, entity.roomZansuSokei.Value, entity.roomZansuSokei.DBType, entity.roomZansuSokei.IntegerBu, entity.roomZansuSokei.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA =  " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY =  " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// ROOM MAX定員数取得
    /// </summary>
    /// <param name="crsZasekiData">コース台帳検索条件群</param>
    /// <returns>ROOM MAX定員数</returns>
    private double getRoomMaxTein(Hashtable crsZasekiData)
    {
        base.paramClear();
        var entity = new CrsLedgerBasicEntity();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      MSS.SIIRE_SAKI_CD ");
        sb.AppendLine("     ,MSS.SIIRE_SAKI_NO ");
        sb.AppendLine("     ,MSS.ROOM_MAX_CAPACITY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_CRS_LEDGER_HOTEL CLH ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_SIIRE_SAKI MSS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MSS.SIIRE_SAKI_CD = CLH.SIIRE_SAKI_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     MSS.SIIRE_SAKI_NO = CLH.SIIRE_SAKI_EDABAN ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam("CRS_CD", crsZasekiData["CRS_CD"], entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam("SYUPT_DAY", crsZasekiData["SYUPT_DAY"], entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam("GOUSYA", crsZasekiData["GOUSYA"], entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        DataTable roomMaxInfo = base.getDataTable(sb.ToString());
        const double roomMaxCap = 99d;
        if (roomMaxInfo.Rows.Count <= CommonRegistYoyaku.ZERO)
        {
            return roomMaxCap;
        }

        double roomMax = 0d;
        double.TryParse(roomMaxInfo.Rows(0)("ROOM_MAX_CAPACITY").ToString(), roomMax);
        if (roomMax < roomMaxCap)
        {
            return roomMax;
        }

        return roomMaxCap;
    }

    /// <summary>
    /// 部屋残数算出
    /// </summary>
    /// <param name="zasekiData"></param>
    /// <param name="manKagenSu">男性加減数</param>
    /// <param name="womanKagenSu">女性加減数</param>
    /// <param name="crsBlockColName">コースブロックカラム名</param>
    /// <param name="roomZansuColName">部屋残数カラム名</param>
    /// <param name="isTorikeshi">取消フラグ</param>
    /// <returns>部屋残数</returns>
    private int calcRoomZansuAibeyaAri(DataTable zasekiData, int manKagenSu, int womanKagenSu, string crsBlockColName, string roomZansuColName, bool isTorikeshi)
    {
        int crsBlockRoomSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName));
        int crsRoomZanSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(roomZansuColName));
        if (crsBlockRoomSu == CommonRegistYoyaku.ZERO)
        {
            // コースブロック管理されていない場合、部屋残数は変動なし
            return crsRoomZanSu;
        }

        int fugoHanten = -1;
        if (isTorikeshi)
        {
            fugoHanten = 1;
        }

        int roomZansu = crsRoomZanSu + manKagenSu * fugoHanten + womanKagenSu * fugoHanten;
        return roomZansu;
    }

    /// <summary>
    /// WTリクエストコース情報取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WTリクエストコース情報取得SQL</returns>
    private string createCrsWtRequestInfoSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CLB.SYUPT_DAY ");
        sb.AppendLine("     ,CLB.CRS_CD ");
        sb.AppendLine("     ,CLB.CRS_NAME ");
        sb.AppendLine("     ,CLB.GOUSYA ");
        sb.AppendLine("     ,CLB.CRS_KIND ");
        sb.AppendLine("     ,CLB.CRS_KBN_1 ");
        sb.AppendLine("     ,CLB.CRS_KBN_2 ");
        sb.AppendLine("     ,CLB.TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     ,CLB.HOUJIN_GAIKYAKU_KBN ");
        sb.AppendLine("     ,CLB.SAIKOU_KAKUTEI_KBN ");
        sb.AppendLine("     ,CLB.RETURN_DAY ");
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ");
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_1 AS JYOCHACHI_CD_1 ");
        sb.AppendLine("     ,PL1.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_1 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_2 AS JYOCHACHI_CD_2 ");
        sb.AppendLine("     ,PL2.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_2 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_3 AS JYOCHACHI_CD_3 ");
        sb.AppendLine("     ,PL3.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_3 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_4 AS JYOCHACHI_CD_4 ");
        sb.AppendLine("     ,PL4.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_4 ");
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_5 AS JYOCHACHI_CD_5 ");
        sb.AppendLine("     ,PL5.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_5 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUGO_TIME_5 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_1 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_2 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_3 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_4 ");
        sb.AppendLine("     ,CLB.SYUPT_TIME_5 ");
        sb.AppendLine("     ,CLB.JYOSEI_SENYO_SEAT_FLG ");
        sb.AppendLine("     ,CLB.TYUIJIKOU ");
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ");
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ");
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ");
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ");
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ");
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,CLB.HURIKOMI_NG_FLG ");
        sb.AppendLine("     ,CLB.ONE_SANKA_FLG ");
        sb.AppendLine("     ,CLB.JYOSYA_CAPACITY ");
        sb.AppendLine("     ,CLB.TEIINSEI_FLG ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_ONE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_TWO_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_THREE_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FOUR_1R ");
        sb.AppendLine("     ,CLB.CRS_BLOCK_FIVE_1R ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_ONE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_TWO_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_THREE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FOUR_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FIVE_ROOM ");
        sb.AppendLine("     ,CLB.ROOM_ZANSU_SOKEI ");
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ");
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ");
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ");
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ");
        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ");
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ");
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ");
        sb.AppendLine("     ,CLB.AGE_FLG ");
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ");
        sb.AppendLine("     ,CLB.TEL_FLG ");
        sb.AppendLine("     ,CLB.ADDRESS_FLG ");
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ");
        sb.AppendLine("     ,CLB.USING_FLG ");
        sb.AppendLine("     ,WRI.MANAGEMENT_KBN ");
        sb.AppendLine("     ,WRI.MANAGEMENT_NO ");
        sb.AppendLine("     ,WRI.SEIKI_CHARGE_ALL_GAKU ");
        sb.AppendLine("     ,WRI.WARIBIKI_ALL_GAKU ");
        sb.AppendLine("     ,WRI.ZASEKI ");
        sb.AppendLine("     ,WRI.NYUKINGAKU_SOKEI ");
        sb.AppendLine("     ,WRI.CANCEL_RYOU_KEI ");
        sb.AppendLine("     ,WRI.SUB_SEAT_WAIT_FLG ");
        sb.AppendLine("     ,WRI.MOTO_YOYAKU_KBN ");
        sb.AppendLine("     ,WRI.MOTO_YOYAKU_NO ");
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_1 ");
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_2 ");
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_3 ");
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_4 ");
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_5 ");
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_1 ");
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_2 ");
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_3 ");
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_4 ");
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_5 ");
        sb.AppendLine("     ,WRI.INFANT_NINZU ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_1 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_2 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_3 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_4 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_5 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_6 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_7 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_8 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_9 ");
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_10 ");
        sb.AppendLine("     ,WRI.MEDIA_CD ");
        sb.AppendLine("     ,WRI.AGENT_CD ");
        sb.AppendLine("     ,WRI.AGENT_NAME_KANA ");
        sb.AppendLine("     ,WRI.AGENT_NM ");
        sb.AppendLine("     ,WRI.AGENT_SEISAN_KBN ");
        sb.AppendLine("     ,WRI.AGENT_TEL_NO ");
        sb.AppendLine("     ,WRI.AGENT_TANTOSYA ");
        sb.AppendLine("     ,WRI.TOURS_NO ");
        sb.AppendLine("     ,WRI.KOKUSEKI ");
        sb.AppendLine("     ,WRI.SEX_BETU ");
        sb.AppendLine("     ,WRI.SURNAME ");
        sb.AppendLine("     ,WRI.NAME ");
        sb.AppendLine("     ,WRI.SURNAME_KJ ");
        sb.AppendLine("     ,WRI.NAME_KJ ");
        sb.AppendLine("     ,WRI.TEL_NO_1 ");
        sb.AppendLine("     ,WRI.TEL_NO_2 ");
        sb.AppendLine("     ,WRI.MAIL_ADDRESS ");
        sb.AppendLine("     ,WRI.MAIL_SENDING_KBN ");
        sb.AppendLine("     ,WRI.YUBIN_NO ");
        sb.AppendLine("     ,WRI.ADDRESS_1 ");
        sb.AppendLine("     ,WRI.ADDRESS_2 ");
        sb.AppendLine("     ,WRI.ADDRESS_3 ");
        sb.AppendLine("     ,WRI.MOSHIKOMI_HOTEL_FLG ");
        sb.AppendLine("     ,WRI.YYKMKS ");
        sb.AppendLine("     ,WRI.SEISAN_HOHO ");
        sb.AppendLine("     ,SEI.CODE_NAME AS SEISAN_HOHO_NM ");
        sb.AppendLine("     ,WRI.FURIKOMIYOSHI_YOHI_FLG ");
        sb.AppendLine("     ,WRI.SEIKYUSYO_YOHI_FLG ");
        sb.AppendLine("     ,WRI.RELATION_YOYAKU_KBN ");
        sb.AppendLine("     ,WRI.RELATION_YOYAKU_NO ");
        sb.AppendLine("     ,WRI.ADD_CHARGE_MAEBARAI_KEI ");
        sb.AppendLine("     ,WRI.ADD_CHARGE_TOJITU_PAYMENT_KEI ");
        sb.AppendLine("     ,WRI.ENTRY_DAY ");
        sb.AppendLine("     ,WRI.YOYAKU_JI_AGENT_CD ");
        sb.AppendLine("     ,WRI.YOYAKU_JI_AGENT_NAME ");
        sb.AppendLine("     ,WRI.JYOSEI_SENYO ");
        sb.AppendLine("     ,WRI.AIBEYA_FLG ");
        sb.AppendLine("     ,WRI.CANCEL_FLG ");
        sb.AppendLine("     ,WRI.HAKKEN_NAIYO ");
        sb.AppendLine("     ,WRI.CANCEL_FLG ");
        sb.AppendLine("     ,WRI.STATE ");
        sb.AppendLine("     ,WRI.TORIATUKAI_FEE_URIAGE ");
        sb.AppendLine("     ,WRI.TORIATUKAI_FEE_CANCEL ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CLB.CRS_CD = WRI.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.SYUPT_DAY = WRI.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLB.GOUSYA = WRI.GOUSYA ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL1 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL1.PLACE_CD = CLB.HAISYA_KEIYU_CD_1 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL2 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL2.PLACE_CD = CLB.HAISYA_KEIYU_CD_2 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL3 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL3.PLACE_CD = CLB.HAISYA_KEIYU_CD_3 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL4 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL4.PLACE_CD = CLB.HAISYA_KEIYU_CD_4 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PLACE PL5 ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     PL5.PLACE_CD = CLB.HAISYA_KEIYU_CD_5 ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE SEI ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     SEI.CODE_BUNRUI = '027' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     SEI.CODE_VALUE = WRI.SEISAN_HOHO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     WRI.MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     WRI.MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// WTリクエスト料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WTリクエスト料金区分一覧取得SQL</returns>
    private string createWtRequestChargeListSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT  ");
        sb.AppendLine("      CLC.CHARGE_KBN  ");
        sb.AppendLine("     ,CLC.KBN_NO  ");
        sb.AppendLine("     ,MCK.CHARGE_NAME  ");
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG  ");
        sb.AppendLine("     ,MCK.STAY_ADD_FLG  ");
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG  ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD  ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME  ");
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME_RK  ");
        sb.AppendLine("     ,CJK.SEX_BETU  ");
        sb.AppendLine("     ,CCK.CHARGE  ");
        sb.AppendLine("     ,CCK.CHARGE_SUB_SEAT  ");
        sb.AppendLine("     ,CCK.CARRIAGE  ");
        sb.AppendLine("     ,CCK.CARRIAGE_SUB_SEAT  ");
        sb.AppendLine("     ,CCK.CHARGE_1  ");
        sb.AppendLine("     ,CCK.CHARGE_2  ");
        sb.AppendLine("     ,CCK.CHARGE_3  ");
        sb.AppendLine("     ,CCK.CHARGE_4  ");
        sb.AppendLine("     ,CCK.CHARGE_5  ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("     ,WCK.TANKA_1 ");
        sb.AppendLine("     ,WCK.TANKA_2 ");
        sb.AppendLine("     ,WCK.TANKA_3 ");
        sb.AppendLine("     ,WCK.TANKA_4 ");
        sb.AppendLine("     ,WCK.TANKA_5 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU_1 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU_2 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU_3 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU_4 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU_5 ");
        sb.AppendLine("     ,WCK.CANCEL_NINZU ");
        sb.AppendLine(" FROM  ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE CLC  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     CCK.CRS_CD = CLC.CRS_CD  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.SYUPT_DAY = CLC.SYUPT_DAY  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.GOUSYA = CLC.GOUSYA  ");
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CCK.KBN_NO = CLC.KBN_NO  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     M_CHARGE_KBN MCK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     MCK.CHARGE_KBN = CLC.CHARGE_KBN  ");
        sb.AppendLine(" LEFT OUTER JOIN  ");
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK  ");
        sb.AppendLine(" ON  ");
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     (SELECT ");
        sb.AppendLine("          KBN_NO ");
        sb.AppendLine("         ,CHARGE_KBN_JININ_CD ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("         ,TANKA_1 ");
        sb.AppendLine("         ,TANKA_2 ");
        sb.AppendLine("         ,TANKA_3 ");
        sb.AppendLine("         ,TANKA_4 ");
        sb.AppendLine("         ,TANKA_5 ");
        sb.AppendLine("         ,CANCEL_NINZU_1 ");
        sb.AppendLine("         ,CANCEL_NINZU_2 ");
        sb.AppendLine("         ,CANCEL_NINZU_3 ");
        sb.AppendLine("         ,CANCEL_NINZU_4 ");
        sb.AppendLine("         ,CANCEL_NINZU_5 ");
        sb.AppendLine("         ,CANCEL_NINZU ");
        sb.AppendLine("     FROM ");
        sb.AppendLine("         T_WT_REQUEST_INFO WRI ");
        sb.AppendLine("     LEFT OUTER JOIN ");
        sb.AppendLine("         T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN CCK ");
        sb.AppendLine("     ON ");
        sb.AppendLine("         WRI.MANAGEMENT_KBN = CCK.MANAGEMENT_KBN ");
        sb.AppendLine("         AND ");
        sb.AppendLine("         WRI.MANAGEMENT_NO = CCK.MANAGEMENT_NO ");
        sb.AppendLine("         WHERE ");
        sb.AppendLine("         WRI.MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("         AND ");
        sb.AppendLine("         WRI.MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        sb.AppendLine("     ) WCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     WCK.KBN_NO = CLC.KBN_NO  ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     WCK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" WHERE  ");
        sb.AppendLine("     CLC.CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CLC.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CLC.GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" ORDER BY CLC.KBN_NO, CCK.CHARGE_KBN_JININ_CD  ");
        return sb.ToString();
    }

    /// <summary>
    /// WTリクエスト料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WTリクエスト料金区分一覧取得SQL</returns>
    private string createWtRequestChargeKbnList(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      CCK.KBN_NO AS KBN_NO_URA ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD_URA ");
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     ,CJK.SEX_BETU  ");
        sb.AppendLine("     ,CCK.CHARGE_KBN ");
        sb.AppendLine("     ,CCK.KBN_NO AS KBN_NO ");
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ");
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ");
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ");
        sb.AppendLine("     ,CCK.CARRIAGE ");
        sb.AppendLine("     ,CCK.CHARGE_APPLICATION_NINZU_1 AS YOYAKU_NINZU ");
        sb.AppendLine("     ,CCK.TANKA_1 AS CHARGE ");
        sb.AppendLine("     ,CLC.CHARGE_SUB_SEAT ");
        sb.AppendLine("     ,CLC.CARRIAGE_SUB_SEAT ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ");
        sb.AppendLine(" INNER JOIN ");
        sb.AppendLine("     T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN CCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CCK.MANAGEMENT_KBN = WRI.MANAGEMENT_KBN ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CCK.MANAGEMENT_NO = WRI.MANAGEMENT_NO ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CLC  ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CLC.CRS_CD = WRI.CRS_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.SYUPT_DAY = WRI.SYUPT_DAY ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.GOUSYA = WRI.GOUSYA ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.KBN_NO = CCK.KBN_NO ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     CLC.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_KBN MCK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MCK.CHARGE_KBN = CCK.CHARGE_KBN ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("         WRI.MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("         AND ");
        sb.AppendLine("         WRI.MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// WT_リクエスト情報（メモ）取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報（メモ）Entity</param>
    /// <returns>WT_リクエスト情報（メモ）取得SQL</returns>
    private string createWtRequestMemoInfoListSql(WtRequestInfoMemoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      WIM.SYSTEM_UPDATE_DAY ");
        sb.AppendLine("     ,WIM.MEMO_KBN ");
        sb.AppendLine("     ,KBN.CODE_NAME AS MEMO_KBN_NM ");
        sb.AppendLine("     ,WIM.MEMO_BUNRUI ");
        sb.AppendLine("     ,BUN.CODE_NAME AS MEMO_BUNRUI_NM ");
        sb.AppendLine("     ,WIM.NAIYO ");
        sb.AppendLine("     ,WIM.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SER.USER_NAME ");
        sb.AppendLine("     ,WIM.EDABAN ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO_MEMO WIM ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER SER ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     SER.COMPANY_CD = '0001' ");
        sb.AppendLine(" AND ");
        sb.AppendLine("     SER.USER_ID = WIM.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE KBN ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     KBN.CODE_BUNRUI = '004' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     KBN.CODE_VALUE = WIM.MEMO_KBN ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_CODE BUN ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     BUN.CODE_BUNRUI = '005' ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     BUN.CODE_VALUE = WIM.MEMO_BUNRUI ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     NVL(WIM.DELETE_DATE, 0) = 0 ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     WIM.MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     WIM.MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        sb.AppendLine(" ORDER BY WIM.SYSTEM_UPDATE_DAY, WIM.EDABAN ");
        return sb.ToString();
    }

    /// <summary>
    /// WT_リクエスト情報取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WT_リクエスト情報取得SQL</returns>
    private string createWtRequestInfoSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      MANAGEMENT_KBN ");
        sb.AppendLine("     ,MANAGEMENT_NO ");
        sb.AppendLine("     ,ADDRESS_1 ");
        sb.AppendLine("     ,ADDRESS_2 ");
        sb.AppendLine("     ,ADDRESS_3 ");
        sb.AppendLine("     ,MOSHIKOMI_HOTEL_FLG ");
        sb.AppendLine("     ,ADD_CHARGE_MAEBARAI_KEI ");
        sb.AppendLine("     ,ADD_CHARGE_TOJITU_PAYMENT_KEI ");
        sb.AppendLine("     ,AF_STAY_YOYAKU_SGL_NUM ");
        sb.AppendLine("     ,AF_STAY_YOYAKU_TWN_NUM ");
        sb.AppendLine("     ,AGENT_CD ");
        sb.AppendLine("     ,AGENT_NAME_KANA ");
        sb.AppendLine("     ,AGENT_NM ");
        sb.AppendLine("     ,AGENT_TANTOSYA ");
        sb.AppendLine("     ,AGENT_TEL_NO ");
        sb.AppendLine("     ,AGENT_TEL_NO_1 ");
        sb.AppendLine("     ,AGENT_TEL_NO_2 ");
        sb.AppendLine("     ,AGENT_TEL_NO_3 ");
        sb.AppendLine("     ,AGENT_YOYAKU_CD ");
        sb.AppendLine("     ,AGENT_SEISAN_KBN ");
        sb.AppendLine("     ,AIBEYA_FLG ");
        sb.AppendLine("     ,BIRTHDAY ");
        sb.AppendLine("     ,CANCEL_FLG ");
        sb.AppendLine("     ,CANCEL_RYOU_KEI ");
        sb.AppendLine("     ,CHANGE_HISTORY_LAST_DAY ");
        sb.AppendLine("     ,CHANGE_HISTORY_LAST_SEQ ");
        sb.AppendLine("     ,CHECKIN_FLG_1 ");
        sb.AppendLine("     ,CHECKIN_FLG_2 ");
        sb.AppendLine("     ,CHECKIN_FLG_3 ");
        sb.AppendLine("     ,CHECKIN_FLG_4 ");
        sb.AppendLine("     ,CHECKIN_FLG_5 ");
        sb.AppendLine("     ,CHECKIN_NINZU_1 ");
        sb.AppendLine("     ,CHECKIN_NINZU_2 ");
        sb.AppendLine("     ,CHECKIN_NINZU_3 ");
        sb.AppendLine("     ,CHECKIN_NINZU_4 ");
        sb.AppendLine("     ,CHECKIN_NINZU_5 ");
        sb.AppendLine("     ,INFANT_NINZU ");
        sb.AppendLine("     ,CRS_CD ");
        sb.AppendLine("     ,CRS_KBN_1 ");
        sb.AppendLine("     ,CRS_KBN_2 ");
        sb.AppendLine("     ,CRS_KIND ");
        sb.AppendLine("     ,DELETE_DAY ");
        sb.AppendLine("     ,ENTRY_DAY ");
        sb.AppendLine("     ,ENTRY_PERSON_CD ");
        sb.AppendLine("     ,ENTRY_PGMID ");
        sb.AppendLine("     ,ENTRY_TIME ");
        sb.AppendLine("     ,FUJYO_PROOF_ISSUE_FLG ");
        sb.AppendLine("     ,FURIKOMIYOSHI_YOHI_FLG ");
        sb.AppendLine("     ,GOUSYA ");
        sb.AppendLine("     ,GROUP_NO ");
        sb.AppendLine("     ,HAKKEN_DAY ");
        sb.AppendLine("     ,HAKKEN_EIGYOSYO_CD ");
        sb.AppendLine("     ,HAKKEN_KINGAKU ");
        sb.AppendLine("     ,HAKKEN_NAIYO ");
        sb.AppendLine("     ,HAKKEN_TANTOSYA_CD ");
        sb.AppendLine("     ,ITINERARY_TABLE_PRINT_ALREADY ");
        sb.AppendLine("     ,ITINERARY_TABLE_PRINT_DAY ");
        sb.AppendLine("     ,JYOCHACHI_CD_1 ");
        sb.AppendLine("     ,JYOCHACHI_CD_2 ");
        sb.AppendLine("     ,JYOCHACHI_CD_3 ");
        sb.AppendLine("     ,JYOCHACHI_CD_4 ");
        sb.AppendLine("     ,JYOCHACHI_CD_5 ");
        sb.AppendLine("     ,JYOSEI_SENYO ");
        sb.AppendLine("     ,JYOSYA_NINZU_1 ");
        sb.AppendLine("     ,JYOSYA_NINZU_2 ");
        sb.AppendLine("     ,JYOSYA_NINZU_3 ");
        sb.AppendLine("     ,JYOSYA_NINZU_4 ");
        sb.AppendLine("     ,JYOSYA_NINZU_5 ");
        sb.AppendLine("     ,KOKUSEKI ");
        sb.AppendLine("     ,LAST_HENKIN_DAY ");
        sb.AppendLine("     ,LAST_NYUUKIN_DAY ");
        sb.AppendLine("     ,LOCAL_TEL_NO ");
        sb.AppendLine("     ,LOST_DAY ");
        sb.AppendLine("     ,LOST_FLG ");
        sb.AppendLine("     ,MAIL_ADDRESS ");
        sb.AppendLine("     ,MAIL_SENDING_KBN ");
        sb.AppendLine("     ,MEDIA_CD ");
        sb.AppendLine("     ,MEDIA_NAME ");
        sb.AppendLine("     ,MEIBO_SEQ ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_1 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_2 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_3 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_4 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_5 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_6 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_7 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_8 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_9 ");
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_10 ");
        sb.AppendLine("     ,SURNAME ");
        sb.AppendLine("     ,NAME ");
        sb.AppendLine("     ,SURNAME_KJ ");
        sb.AppendLine("     ,NAME_KJ ");
        sb.AppendLine("     ,NO_SHOW_FLG ");
        sb.AppendLine("     ,NYUKINGAKU_SOKEI ");
        sb.AppendLine("     ,NYUUKIN_SITUATION_KBN ");
        sb.AppendLine("     ,OLD_GOUSYA ");
        sb.AppendLine("     ,OLD_ZASEKI ");
        sb.AppendLine("     ,RECEIPT_ISSUE_FLG ");
        sb.AppendLine("     ,RELATION_YOYAKU_KBN ");
        sb.AppendLine("     ,RELATION_YOYAKU_NO ");
        sb.AppendLine("     ,RETURN_DAY ");
        sb.AppendLine("     ,ROOMING_BETU_NINZU_1 ");
        sb.AppendLine("     ,ROOMING_BETU_NINZU_2 ");
        sb.AppendLine("     ,ROOMING_BETU_NINZU_3 ");
        sb.AppendLine("     ,ROOMING_BETU_NINZU_4 ");
        sb.AppendLine("     ,ROOMING_BETU_NINZU_5 ");
        sb.AppendLine("     ,SAIKOU_KAKUTEI_GUIDE_OUT_DAY ");
        sb.AppendLine("     ,SEIKI_CHARGE_ALL_GAKU ");
        sb.AppendLine("     ,SEIKYUSYO_YOHI_FLG ");
        sb.AppendLine("     ,SEISAN_HOHO ");
        sb.AppendLine("     ,SEX_BETU ");
        sb.AppendLine("     ,SIHARAI_HOHO ");
        sb.AppendLine("     ,SONOTA_NYUUKIN_HENKIN ");
        sb.AppendLine("     ,SPONSORSHIP_KEIYAKU_KBN ");
        sb.AppendLine("     ,STATE ");
        sb.AppendLine("     ,SYUGO_TIME_1 ");
        sb.AppendLine("     ,SYUGO_TIME_2 ");
        sb.AppendLine("     ,SYUGO_TIME_3 ");
        sb.AppendLine("     ,SYUGO_TIME_4 ");
        sb.AppendLine("     ,SYUGO_TIME_5 ");
        sb.AppendLine("     ,SYUPT_DAY ");
        sb.AppendLine("     ,SYUPT_TIME_1 ");
        sb.AppendLine("     ,SYUPT_TIME_2 ");
        sb.AppendLine("     ,SYUPT_TIME_3 ");
        sb.AppendLine("     ,SYUPT_TIME_4 ");
        sb.AppendLine("     ,SYUPT_TIME_5 ");
        sb.AppendLine("     ,TASYA_KENNO_KINGAKU ");
        sb.AppendLine("     ,TASYA_YOYAKU_NO ");
        sb.AppendLine("     ,TEIKI_KEIYAKU_KBN ");
        sb.AppendLine("     ,TEIKI_KIKAKU_KBN ");
        sb.AppendLine("     ,TEL_NO_1 ");
        sb.AppendLine("     ,TEL_NO_1_1 ");
        sb.AppendLine("     ,TEL_NO_1_2 ");
        sb.AppendLine("     ,TEL_NO_1_3 ");
        sb.AppendLine("     ,TEL_NO_2 ");
        sb.AppendLine("     ,TEL_NO_2_1 ");
        sb.AppendLine("     ,TEL_NO_2_2 ");
        sb.AppendLine("     ,TEL_NO_2_3 ");
        sb.AppendLine("     ,TOBI_SEAT_FLG ");
        sb.AppendLine("     ,TOMONOKAI_NO ");
        sb.AppendLine("     ,TORIATUKAI_FEE_CANCEL ");
        sb.AppendLine("     ,TORIATUKAI_FEE_SAGAKU ");
        sb.AppendLine("     ,TORIATUKAI_FEE_URIAGE ");
        sb.AppendLine("     ,TOURS_NO ");
        sb.AppendLine("     ,UNKYU_CONTACT_DAY ");
        sb.AppendLine("     ,UPDATE_DAY ");
        sb.AppendLine("     ,UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UPDATE_PGMID ");
        sb.AppendLine("     ,UPDATE_TIME ");
        sb.AppendLine("     ,USING_FLG ");
        sb.AppendLine("     ,WARIBIKI_ALL_GAKU ");
        sb.AppendLine("     ,YEAR ");
        sb.AppendLine("     ,YOBI_1 ");
        sb.AppendLine("     ,YOBI_2 ");
        sb.AppendLine("     ,YOBI_3 ");
        sb.AppendLine("     ,YOBI_4 ");
        sb.AppendLine("     ,YOBI_5 ");
        sb.AppendLine("     ,YOBI_6 ");
        sb.AppendLine("     ,YOBI_7 ");
        sb.AppendLine("     ,YOYAKU_JI_AGENT_CD ");
        sb.AppendLine("     ,YOYAKU_JI_AGENT_NAME ");
        sb.AppendLine("     ,YOYAKU_KAKUNIN_DAY ");
        sb.AppendLine("     ,YOYAKU_UKETUKE_KBN ");
        sb.AppendLine("     ,YOYAKU_ZASEKI_GET_KBN ");
        sb.AppendLine("     ,YOYAKU_ZASEKI_KBN ");
        sb.AppendLine("     ,YUBIN_NO ");
        sb.AppendLine("     ,YYKMKS ");
        sb.AppendLine("     ,ZASEKI ");
        sb.AppendLine("     ,ZASEKI_CHANGE_UMU ");
        sb.AppendLine("     ,ZASEKI_RESERVE_YOYAKU_FLG ");
        sb.AppendLine("     ,SUB_SEAT_WAIT_FLG ");
        sb.AppendLine("     ,MOTO_YOYAKU_KBN ");
        sb.AppendLine("     ,MOTO_YOYAKU_NO ");
        sb.AppendLine("     ,ENTRY_SECTION_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO  ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     MOTO_YOYAKU_KBN = " + base.setParam(entity.motoYoyakuKbn.PhysicsName, entity.motoYoyakuKbn.Value, entity.motoYoyakuKbn.DBType, entity.motoYoyakuKbn.IntegerBu, entity.motoYoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     MOTO_YOYAKU_NO = " + base.setParam(entity.motoYoyakuNo.PhysicsName, entity.motoYoyakuNo.Value, entity.motoYoyakuNo.DBType, entity.motoYoyakuNo.IntegerBu, entity.motoYoyakuNo.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     NVL(STATE, ' ') = ' ' ");
        return sb.ToString();
    }
	
	 private string createWtCancelNinzuUpdateSql(CrsLedgerBasicEntity entity, OracleTransaction oracleTransaction)
    {
        var wtRequestEntity = new WtRequestInfoEntity();
        wtRequestEntity.crsCd.Value = entity.crsCd.Value;
        wtRequestEntity.syuptDay.Value = entity.syuptDay.Value;
        wtRequestEntity.gousya.Value = entity.gousya.Value;
        string wtCancelQuery = createWtCancelNinzuSql(wtRequestEntity);
        DataTable wtCancelInfo = base.getDataTable(oracleTransaction, wtCancelQuery);
        // WT人数設定
        entity.cancelWaitNinzu.Value = CommonRegistYoyaku.convertObjectToInteger(wtCancelInfo.Rows(0)("CANCEL_WAIT_NINZU"));
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("     CANCEL_WAIT_NINZU = " + base.setParam(entity.cancelWaitNinzu.PhysicsName, entity.cancelWaitNinzu.Value, entity.cancelWaitNinzu.DBType, entity.cancelWaitNinzu.IntegerBu, entity.cancelWaitNinzu.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// WT人数取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WT人数取得SQL</returns>
    private string createWtCancelNinzuSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("     COUNT(0) AS CANCEL_WAIT_NINZU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO  ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     NVL(STATE, ' ') = ' ' ");
        return sb.ToString();
    }

    /// <summary>
    /// 複写予約情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>複写予約情報取得SQL</returns>
    private string createReproductionYoyakuDataSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        sb.AppendLine("     ,SURNAME ");
        sb.AppendLine("     ,NAME ");
        sb.AppendLine("     ,SURNAME_KJ ");
        sb.AppendLine("     ,NAME_KJ ");
        sb.AppendLine("     ,TEL_NO_1 ");
        sb.AppendLine("     ,TEL_NO_2 ");
        sb.AppendLine("     ,YUBIN_NO ");
        sb.AppendLine("     ,ADDRESS_1 ");
        sb.AppendLine("     ,ADDRESS_2 ");
        sb.AppendLine("     ,ADDRESS_3 ");
        sb.AppendLine("     ,YYKMKS ");
        sb.AppendLine("     ,AGENT_CD ");
        sb.AppendLine("     ,AGENT_NAME_KANA ");
        sb.AppendLine("     ,AGENT_NM ");
        sb.AppendLine("     ,AGENT_TEL_NO ");
        sb.AppendLine("     ,AGENT_TANTOSYA ");
        sb.AppendLine("     ,AGENT_SEISAN_KBN ");
        sb.AppendLine("     ,TOURS_NO ");
        sb.AppendLine("     ,KOKUSEKI ");
        sb.AppendLine("     ,SEX_BETU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 複写WTリクエスト情報取得SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>複写WTリクエスト情報取得SQL</returns>
    private string createReproductionWtRequestDataSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      MANAGEMENT_KBN ");
        sb.AppendLine("     ,MANAGEMENT_NO ");
        sb.AppendLine("     ,SURNAME ");
        sb.AppendLine("     ,NAME ");
        sb.AppendLine("     ,SURNAME_KJ ");
        sb.AppendLine("     ,NAME_KJ ");
        sb.AppendLine("     ,TEL_NO_1 ");
        sb.AppendLine("     ,TEL_NO_2 ");
        sb.AppendLine("     ,YUBIN_NO ");
        sb.AppendLine("     ,ADDRESS_1 ");
        sb.AppendLine("     ,ADDRESS_2 ");
        sb.AppendLine("     ,ADDRESS_3 ");
        sb.AppendLine("     ,YYKMKS ");
        sb.AppendLine("     ,AGENT_CD ");
        sb.AppendLine("     ,AGENT_NAME_KANA ");
        sb.AppendLine("     ,AGENT_NM ");
        sb.AppendLine("     ,AGENT_TEL_NO ");
        sb.AppendLine("     ,AGENT_TANTOSYA ");
        sb.AppendLine("     ,AGENT_SEISAN_KBN ");
        sb.AppendLine("     ,TOURS_NO ");
        sb.AppendLine("     ,KOKUSEKI ");
        sb.AppendLine("     ,SEX_BETU ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 代理店情報取得SQL作成
    /// </summary>
    /// <param name="entity">代理店マスタEntity</param>
    /// <returns>代理店情報取得SQL</returns>
    private string createAgentMasterSql(MAgentEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      * ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_AGENT ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     AGENT_CD = " + base.setParam(entity.agentCd.PhysicsName, entity.agentCd.Value, entity.agentCd.DBType, entity.agentCd.IntegerBu, entity.agentCd.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約ピックアップ情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約ピックアップ情報取得SQL</returns>
    private string createYoyakuPickupInfoSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YIP.YOYAKU_KBN ");
        sb.AppendLine("     ,YIP.YOYAKU_NO ");
        sb.AppendLine("     ,YIP.PICKUP_HOTEL_CD ");
        sb.AppendLine("     ,MPH.RK ");
        sb.AppendLine("     ,MPH.HOTEL_NAME_JYOSYA_TI ");
        sb.AppendLine("     ,YIP.PICKUP_ROUTE_CD ");
        sb.AppendLine("     ,MPH.SYUGO_PLACE ");
        sb.AppendLine("     ,SUBSTR(LPAD(RLH.SYUPT_TIME, 4,'0'), 1, 2)||':'|| SUBSTR(LPAD(RLH.SYUPT_TIME, 4,'0'), 3) AS SYUPT_TIME ");
        sb.AppendLine("     ,YIP.NINZU ");
        sb.AppendLine("     ,PRL.CRS_JYOSYA_TI ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_PICKUP YIP ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_PICKUP_HOTEL MPH ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     MPH.PICKUP_HOTEL_CD = YIP.PICKUP_HOTEL_CD ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER PRL ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     PRL.PICKUP_ROUTE_CD = YIP.PICKUP_ROUTE_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     PRL.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER_HOTEL RLH ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     RLH.PICKUP_ROUTE_CD = YIP.PICKUP_ROUTE_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     RLH.PICKUP_HOTEL_CD = YIP.PICKUP_HOTEL_CD ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     RLH.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIP.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIP.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// その他情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>その他情報取得SQL</returns>
    private string createSonotaInfoSql(YoyakuInfoBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YIB.SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,YIB.SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,EUS.USER_NAME AS ENTRY_USER_NAME ");
        sb.AppendLine("     ,YIB.SYSTEM_UPDATE_DAY ");
        sb.AppendLine("     ,YIB.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UUS.USER_NAME AS UPDATE_USER_NAME ");
        sb.AppendLine("     ,YIB.HAKKEN_DAY ");
        sb.AppendLine("     ,YIB.HAKKEN_TANTOSYA_CD ");
        sb.AppendLine("     ,HUS.USER_NAME AS HAKKEN_USER_NAME ");
        sb.AppendLine("     ,YIB.LAST_NYUUKIN_DAY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER EUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YIB.SYSTEM_ENTRY_PERSON_CD = EUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     EUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER UUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YIB.SYSTEM_UPDATE_PERSON_CD = UUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     UUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER HUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     YIB.HAKKEN_TANTOSYA_CD = HUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     HUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YIB.YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YIB.YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// その他情報取得SQL作成
    /// </summary>
    /// <param name="entity">WTリクエストEntity</param>
    /// <returns>その他情報取得SQL</returns>
    private string createWtSonotaInfoSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      WRI.SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,WRI.SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,EUS.USER_NAME AS ENTRY_USER_NAME ");
        sb.AppendLine("     ,WRI.SYSTEM_UPDATE_DAY ");
        sb.AppendLine("     ,WRI.SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UUS.USER_NAME AS UPDATE_USER_NAME ");
        sb.AppendLine("     ,WRI.HAKKEN_DAY ");
        sb.AppendLine("     ,WRI.HAKKEN_TANTOSYA_CD ");
        sb.AppendLine("     ,HUS.USER_NAME AS HAKKEN_USER_NAME ");
        sb.AppendLine("     ,WRI.LAST_NYUUKIN_DAY ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER EUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     WRI. SYSTEM_ENTRY_PERSON_CD = EUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     EUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER UUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     WRI. SYSTEM_UPDATE_PERSON_CD = UUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     UUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" LEFT OUTER JOIN ");
        sb.AppendLine("     M_USER HUS ");
        sb.AppendLine(" ON ");
        sb.AppendLine("     WRI. HAKKEN_TANTOSYA_CD = HUS.USER_ID ");
        sb.AppendLine("     AND ");
        sb.AppendLine("     HUS.COMPANY_CD = '0001' ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     WRI.MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     WRI.MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 利用者情報取得SQL作成
    /// </summary>
    /// <param name="nullRecord">空行レコード挿入フラグ</param>
    /// <returns>利用者情報取得SQL</returns>
    private string createUserMasterSql(bool nullRecord)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT * FROM ( ");
        if (nullRecord == true)
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("      ' ' AS CODE_VALUE ");
            sb.AppendLine("     ,' ' AS CODE_NAME ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     DUAL ");
            sb.AppendLine(" UNION ");
        }

        sb.AppendLine(" SELECT ");
        sb.AppendLine("       USER_ID AS CODE_VALUE ");
        sb.AppendLine("      ,USER_NAME AS CODE_NAME ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     M_USER ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     COMPANY_CD = '0001' ");
        sb.AppendLine(" ) M_USER ");
        // sb.AppendLine(" ORDER BY CODE_VALUE ")

        return sb.ToString();
    }

    /// <summary>
    /// WT_リクエスト情報更新SQL作成
    /// </summary>
    /// <param name="entity">WT_リクエスト情報Entity</param>
    /// <returns>WT_リクエスト状態更新SQL</returns>
    private string createWtRequestStateUpdateSql(WtRequestInfoEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_WT_REQUEST_INFO ");
        sb.AppendLine(" SET ");
        sb.AppendLine("     UPDATE_DAY = " + base.setParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay.DBType, entity.updateDay.IntegerBu, entity.updateDay.DecimalBu));
        sb.AppendLine("     ,UPDATE_PERSON_CD = " + base.setParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd.DBType, entity.updatePersonCd.IntegerBu, entity.updatePersonCd.DecimalBu));
        sb.AppendLine("     ,UPDATE_PGMID = " + base.setParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid.DBType, entity.updatePgmid.IntegerBu, entity.updatePgmid.DecimalBu));
        sb.AppendLine("     ,UPDATE_TIME = " + base.setParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime.DBType, entity.updateTime.IntegerBu, entity.updateTime.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = TO_DATE(" + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu) + ",'yyyy/mm/dd hh24:mi:ss')");
        sb.AppendLine("     ,STATE = " + base.setParam(entity.state.PhysicsName, entity.state.Value, entity.state.DBType, entity.state.IntegerBu, entity.state.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     MANAGEMENT_KBN = " + base.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     MANAGEMENT_NO = " + base.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// ピックアップルート台帳 （ホテル）人数更新SQL作成
    /// </summary>
    /// <param name="entity">ピックアップルート台帳 （ホテル）Entity</param>
    /// <returns>ピックアップルート台帳 （ホテル）人数更新SQL</returns>
    private string createPickupRouteLedgerHotelUpdateSql(PickupRouteLedgerHotelEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER_HOTEL ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      NINZU = NINZU + " + base.setParam(entity.ninzu.PhysicsName, entity.ninzu.Value, entity.ninzu.DBType, entity.ninzu.IntegerBu, entity.ninzu.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     PICKUP_ROUTE_CD = " + base.setParam(entity.pickupRouteCd.PhysicsName, entity.pickupRouteCd.Value, entity.pickupRouteCd.DBType, entity.pickupRouteCd.IntegerBu, entity.pickupRouteCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     PICKUP_HOTEL_CD = " + base.setParam(entity.pickupHotelCd.PhysicsName, entity.pickupHotelCd.Value, entity.pickupHotelCd.DBType, entity.pickupHotelCd.IntegerBu, entity.pickupHotelCd.DecimalBu));
        return sb.ToString();
    }
	#endregion
}

