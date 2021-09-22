using Hatobus.ReservationManagementSystem.Zaseki;

/// <summary>
/// 予約の変更DA
/// </summary>
public partial class S02_0104Da : DataAccessorBase
{
	#region 定数/変数

    /// <summary>
    /// 使用中チェックリトライ回数：100
    /// </summary>
    private const int UsingCheckRetryNum = 100;
    /// <summary>
    /// ゼロ
    /// </summary>
    private const int Zero = 0;
    /// <summary>
    /// 架空車種：XX
    /// </summary>
    private const string KakuShashu = "XX";

    #endregion
	
	#region メソッド
	/// <summary>
/// 予約コース情報取得
/// </summary>
/// <param name="entity">予約情報（基本）Entity</param>
/// <returns>予約コース情報取得</returns>
    public DataTable getYoyakuCrsInfo(YoyakuInfoBasicEntity entity)
    {
        DataTable yoyakuCrsInfo;
        try
        {
            string query = createYoyakuCrsInfoSql(entity);
            yoyakuCrsInfo = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return yoyakuCrsInfo;
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
    /// コース情報取得
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース情報一覧</returns>
    public DataTable getCrsInfoList(CrsLedgerBasicEntity entity)
    {
        DataTable crsInfoList;
        try
        {
            string query = this.createCrsInfoSql(entity);
            crsInfoList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return crsInfoList;
    }

    /// <summary>
    /// 料金区分一覧取得
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
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
            throw ex;
        }

        return chargeKbnList;
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
    /// 予約の変更処理
    /// 定期用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="registEntity">変更予約情報Entity</param>
    /// <returns>座席変更ステータス</returns>
    public string updateYoyakuInfoForTeiki(Z0003_Result z0003Result, ChangeYoyakuInfoEntity registEntity, Z0003_Param z0003param)
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
            if (this.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席情報更新
            if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
            {
                if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
                {
                    if (z0003Result.CarTypeCd != KakuShashu)
                    {

                        // コース台帳座席情報更新SQL作成
                        int kusekiTeiseki = 0;
                        string zasekiUpdateQuery = this.createCrsZasekiDataForTeiki(z0003Result, crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                        if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                        if (updateCount <= 0)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        foreach (DataRow row in sharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                    else
                    {
                        // コース台帳座席更新
                        int kusekiTeiseki = 0;
                        updateCount = this.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, oracleTransaction, kusekiTeiseki);
                        if (updateCount <= 0)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        foreach (DataRow row in sharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                }
                else
                {
                    int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                    if (zasekiKagenSu > Zero)
                    {
                        // 変更後の人数が多い場合、架空車種の座席数を変更

                        int kusekiTeiseki = 0;
                        updateCount = this.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki);
                        if (updateCount <= 0)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        foreach (DataRow row in sharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                }
            }
            else if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {

                // コース台帳座席更新
                int kusekiTeiseki = 0;
                updateCount = this.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, oracleTransaction, kusekiTeiseki);
                if (updateCount <= 0)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return S02_0104.UpdateStatusZasekiUpdateFailure;
                }

                foreach (DataRow row in sharedZasekiData.Rows)
                {
                    if (this.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) == false)
                    {
                        // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        continue;
                    }

                    // 共用バス座席情報更新SQL作成
                    string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                    updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                    if (updateCount <= Zero)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiUpdateFailure;
                    }
                }
            }
            else
            {
                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu > Zero)
                {
                    // 変更後の人数が多い場合、架空車種の座席数を変更

                    // コース台帳座席更新
                    int kusekiTeiseki = 0;
                    updateCount = this.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiUpdateFailure;
                    }

                    foreach (DataRow row in sharedZasekiData.Rows)
                    {
                        if (this.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) == false)
                        {
                            // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            continue;
                        }

                        // 共用バス座席情報更新SQL作成
                        string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki);
                        updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }
                    }
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

            // 予約情報更新
            string updateStatus = this.updateYoyakuInfo(registEntity, oracleTransaction);
            if (updateStatus != S02_0104.UpdateStatusSucess)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return updateStatus;
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

            // 旧座席情報取得
            var oldCrsInfoEntity = new CrsLedgerBasicEntity();
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd;
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay;
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya;
            DataTable oldCrsLedgerZasekiData = default;
            DataTable oldSharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldCrsLedgerZasekiData, oldSharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席確定処理実施
            Z0001_Result zasekiKakuteiResult = this.getCommonZasekiJidoData(z0003param, registEntity.YoyakuInfoBasicEntity.groupNo.Value);

            // 前回確保していた座席数を加減
            if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {
                // 出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
                {
                    if (z0003Result.OldCarTypeCd != KakuShashu)
                    {

                        // コース台帳座席情報更新SQL作成
                        string query = this.createCrsZasekiKaihoDateForTeiki(z0003Result, oldCrsLedgerZasekiData, oldCrsInfoEntity);
                        updateCount = base.execNonQuery(oracleTransaction, query);
                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createShearedZasekiKaihoDateForTeiki(row, oldCrsInfoEntity);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                    else
                    {
                        // コース台帳座席更新(旧座席数解放)
                        int kusekiTeiseki = 0;
                        updateCount = this.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction, kusekiTeiseki);
                        if (updateCount <= 0)
                        {
                            // 座席解放処理にて、更新件数が0件の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                }
                else
                {
                    // コース台帳座席更新(旧座席数解放)
                    int kusekiTeiseki = 0;
                    updateCount = this.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction, kusekiTeiseki);
                    if (updateCount <= 0)
                    {
                        // 座席解放処理にて、更新件数が0件の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                    }

                    foreach (DataRow row in oldSharedZasekiData.Rows)
                    {
                        if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                        {
                            // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            continue;
                        }

                        // 共用バス座席情報更新SQL作成
                        string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki);
                        updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }
                    }
                }
            }
            else
            {
                // 予約人数の変更の場合
                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu < Zero)
                {
                    // 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                    if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
                    {

                        // コース台帳座席更新SQL作成
                        string query = this.createCrsKakuShashuZasekiData(oldCrsLedgerZasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result);
                        updateCount = base.execNonQuery(oracleTransaction, query);
                        if (updateCount <= 0)
                        {
                            // 座席解放処理にて、更新件数が0件の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        // 自コースを元に、加減する RefでEntityをもらう？
                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createShearedZasekiKaihoDateForTeiki(row, oldCrsInfoEntity);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
                    }
                    else
                    {
                        // コース台帳座席更新(旧座席数解放)
                        int kusekiTeiseki = 0;
                        updateCount = this.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki);
                        if (updateCount <= 0)
                        {
                            // 座席解放処理にて、更新件数が0件の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用バス座席情報更新SQL作成
                            string sharedZasekiQuery = this.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki);
                            updateCount = base.execNonQuery(oracleTransaction, sharedZasekiQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiUpdateFailure;
                            }
                        }
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

        // 不要行削除
        this.deleteCrsChargeChargeKbnForShukuhakuNashi(registEntity.YoyakuInfoCrsChargeChargeKbnList(0));
        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約の変更処理
    /// 企画（日帰り）用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="registEntity">変更予約情報Entity</param>
    /// <returns>座席変更ステータス</returns>
    public string updateZasekiInfoForKikakuHigaeri(Z0003_Result z0003Result, ChangeYoyakuInfoEntity registEntity, Z0003_Param z0003param)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 座席情報取得
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席情報更新
            string zasekiUpdateQuery = "";
            if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
            {
                if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
                {
                    if (z0003Result.CarTypeCd != KakuShashu)
                    {
                        zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0003Result, zasekiData, registEntity.NewCrsLedgerBasicEntity);
                        if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                    else
                    {
                        // 席区分取得
                        string seatKbn = z0003Result.SeatKbn;
                        // コース台帳座席更新
                        updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        // 共用バス更新
                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                }
                else
                {
                    int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                    if (zasekiKagenSu > Zero)
                    {
                        // 変更後の人数が多い場合、架空車種の座席数を変更

                        // 席区分取得
                        string seatKbn = z0003Result.SeatKbn;
                        // コース台帳座席更新
                        updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        // 共用バス更新
                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                }
            }
            else if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {

                // 席区分取得
                string seatKbn = z0003Result.SeatKbn;
                // コース台帳座席更新
                updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction);
                if (updateCount <= Zero)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return S02_0104.UpdateStatusZasekiUpdateFailure;
                }

                // 共用バス更新
                string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction);
                if (status != S02_0104.UpdateStatusSucess)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return status;
                }
            }
            else
            {
                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu > Zero)
                {
                    // 変更後の人数が多い場合、架空車種の座席数を変更

                    // 席区分取得
                    string seatKbn = z0003Result.SeatKbn;
                    // コース台帳座席更新
                    updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction);
                    if (updateCount <= Zero)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiUpdateFailure;
                    }

                    // 共用バス更新
                    string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction);
                    if (status != S02_0104.UpdateStatusSucess)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return status;
                    }
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

            // 予約情報更新
            string updateStatus = this.updateYoyakuInfo(registEntity, oracleTransaction);
            if (updateStatus != S02_0104.UpdateStatusSucess)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return updateStatus;
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

            // 旧座席情報取得
            var oldCrsInfoEntity = new CrsLedgerBasicEntity();
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd;
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay;
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya;
            DataTable oldZasekiData = default;
            DataTable oldSharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldZasekiData, oldSharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席確定処理実施
            Z0001_Result zasekiKakuteiResult = this.getCommonZasekiJidoData(z0003param, registEntity.YoyakuInfoBasicEntity.groupNo.Value);

            // 前回確保していた座席数を加減
            if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {
                // 出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
                {
                    if (z0003Result.OldCarTypeCd != KakuShashu)
                    {
                        // If True Then

                        // 座席確定DataTableをコピーし、旧座席数などを新側の変数に代入する
                        // コピーする内容は以下
                        // WK7YST(座席加減数・定席) = WK7YTQ(旧座席加減数・定席)
                        // WK7YSH(座席加減数・補助席) = WK7YHQ(旧座席加減数・補助)
                        // WK7KTS(空席数・定席) = WK7KTQ(旧空席数・定席)
                        // WK7KHJ(空席数・補助席) = WK7KHQ(旧空席数・補助席)
                        // WK7801(補助 801 調整数) = WK780Q(旧補助 801 調整数)
                        var copyZasekiKakuteiResult = new Z0003_Result();
                        copyZasekiKakuteiResult.ZasekiKagenTeiseki = z0003Result.OldZasekiKagenTeiseki;
                        copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.OldZasekiKagenSub;
                        copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.OldKusekiNumTeiseki;
                        copyZasekiKakuteiResult.KusekiNumSub = z0003Result.OldKusekiNumSub;
                        copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.OldSubCyoseiSeatNum;
                        var zasekiKaihoUpdateQuery = this.createCrsZasekiDataForKikaku(copyZasekiKakuteiResult, oldZasekiData, oldCrsInfoEntity);
                        if (string.IsNullOrEmpty(zasekiKaihoUpdateQuery) == true)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        updateCount = base.execNonQuery(oracleTransaction, zasekiKaihoUpdateQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        string sharedBusCrsQuery = "";
                        // 共用コース座席更新
                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用コース座席更新SQL作成
                            sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity);
                            updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                            }
                        }
                    }
                    else
                    {
                        // コース台帳座席更新(旧座席数解放)
                        updateCount = this.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction);
                    }
                }
                else
                {
                    // コース台帳座席更新(旧座席数解放)
                    updateCount = this.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction);
                }
            }
            else
            {
                // 予約人数変更の場合

                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu < Zero)
                {
                    // 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                    var copyZasekiKakuteiResult = new Z0003_Result();
                    copyZasekiKakuteiResult.ZasekiKagenTeiseki = zasekiKagenSu;
                    copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.ZasekiKagenSub1F;
                    copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.KusekiNumTeiseki;
                    copyZasekiKakuteiResult.KusekiNumSub = z0003Result.KusekiNumSub;
                    copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.SubCyoseiSeatNum;
                    updateCount = this.updateKakuShashuZaseki(zasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result.SeatKbn, oracleTransaction);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                    }

                    string sharedBusCrsQuery = "";
                    // 共用コース座席更新
                    foreach (DataRow row in oldSharedZasekiData.Rows)
                    {
                        if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                        {
                            // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            continue;
                        }

                        // 共用コース座席更新SQL作成
                        sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity);
                        updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }
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
            throw ex;
        }
        finally
        {
            // トランザクションの破棄
            oracleTransaction.Dispose();
        }

        // 不要な行削除
        this.deleteCrsChargeChargeKbnForShukuhakuNashi(registEntity.YoyakuInfoCrsChargeChargeKbnList(0));
        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約の変更処理
    /// 企画（宿泊）用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    /// <param name="registEntity">変更予約情報Entity</param>
    /// <returns>座席変更ステータス</returns>
    public string updateZasekiInfoForKikakuShukuhaku(Z0003_Result z0003Result, ChangeYoyakuInfoEntity registEntity, Z0003_Param z0003param)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();

            // 座席情報取得
            DataTable zasekiData = default;
            DataTable sharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, zasekiData, sharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席情報更新
            string zasekiUpdateQuery = "";
            if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
            {
                if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
                {
                    if (z0003Result.CarTypeCd != KakuShashu)
                    {

                        // コース台帳座席情報更新SQL作成
                        zasekiUpdateQuery = this.createCrsZasekiDataForKikaku(z0003Result, zasekiData, registEntity.NewCrsLedgerBasicEntity);
                        if (string.IsNullOrEmpty(zasekiUpdateQuery) == true)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        updateCount = base.execNonQuery(oracleTransaction, zasekiUpdateQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusKusekiNothing;
                        }

                        // 共用バス座席更新
                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                    else
                    {
                        // 席区分取得
                        string seatKbn = z0003Result.SeatKbn;
                        // コース台帳座席更新
                        updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        // 共用バス更新
                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                }
                else
                {
                    int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                    if (zasekiKagenSu > Zero)
                    {
                        // 変更後の人数が多い場合、架空車種の座席数を変更

                        // 席区分取得
                        string seatKbn = z0003Result.SeatKbn;
                        // コース台帳座席更新
                        updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiUpdateFailure;
                        }

                        // 共用バス更新
                        string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction);
                        if (status != S02_0104.UpdateStatusSucess)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return status;
                        }
                    }
                }
            }
            else if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {

                // 席区分取得
                string seatKbn = z0003Result.SeatKbn;
                // コース台帳座席更新
                updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction);
                if (updateCount <= Zero)
                {
                    // 座席データの更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return S02_0104.UpdateStatusZasekiUpdateFailure;
                }

                // 共用バス更新
                string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction);
                if (status != S02_0104.UpdateStatusSucess)
                {

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return status;
                }
            }
            else
            {
                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu > Zero)
                {
                    // 変更後の人数が多い場合、架空車種の座席数を変更

                    // 席区分取得
                    string seatKbn = z0003Result.SeatKbn;
                    // コース台帳座席更新
                    updateCount = this.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction);
                    if (updateCount <= Zero)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiUpdateFailure;
                    }

                    // 共用バス更新
                    string status = this.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction);
                    if (status != S02_0104.UpdateStatusSucess)
                    {

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return status;
                    }
                }
            }

            // 部屋残数更新
            if (string.IsNullOrEmpty(registEntity.ZasekiInfoEntity.AibeyaFlag) == true)
            {

                // 部屋残数更新SQL作成
                string query = this.createCrsRoomAibeyaNashiUpdateSql(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.ZasekiInfoEntity, registEntity.YoyakuChangeKbn);
                if (string.IsNullOrEmpty(query) == false)
                {
                    updateCount = base.execNonQuery(oracleTransaction, query);
                    if (updateCount <= Zero)
                    {
                        // 部屋残数の更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusRoomKakuhoUpdateFailure;
                    }
                }
            }
            else
            {
                // 変更後が相部屋の場合

                string query = this.createAibeyaRoomZansuSql(zasekiData, registEntity.ZasekiInfoEntity, registEntity.NewCrsLedgerBasicEntity);
                if (string.IsNullOrEmpty(query) == false)
                {
                    updateCount = base.execNonQuery(oracleTransaction, query);
                    if (updateCount <= Zero)
                    {
                        // 部屋残数の更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusRoomKakuhoUpdateFailure;
                    }
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

            // 予約情報更新
            string updateStatus = this.updateYoyakuInfo(registEntity, oracleTransaction);
            if (updateStatus != S02_0104.UpdateStatusSucess)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return updateStatus;
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

            // 旧座席情報取得
            var oldCrsInfoEntity = new CrsLedgerBasicEntity();
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd;
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay;
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya;
            DataTable oldZasekiData = default;
            DataTable oldSharedZasekiData = default;

            // 自コース、共用コースの使用中チェック
            if (this.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldZasekiData, oldSharedZasekiData) == false)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusUsing;
            }

            // 座席確定処理実施
            Z0001_Result zasekiKakuteiResult = this.getCommonZasekiJidoData(z0003param, registEntity.YoyakuInfoBasicEntity.groupNo.Value);

            // 前回確保していた座席数を加減
            if (registEntity.YoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
            {
                // 出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                if (z0003Result.Status == Z0003_Result.Z0003_Result_Status.OK)
                {
                    if (z0003Result.OldCarTypeCd != KakuShashu)
                    {
                        // If True Then

                        // 座席確定DataTableをコピーし、旧座席数などを新側の変数に代入する
                        // コピーする内容は以下
                        // WK7YST(座席加減数・定席) = WK7YTQ(旧座席加減数・定席)
                        // WK7YSH(座席加減数・補助席) = WK7YHQ(旧座席加減数・補助)
                        // WK7KTS(空席数・定席) = WK7KTQ(旧空席数・定席)
                        // WK7KHJ(空席数・補助席) = WK7KHQ(旧空席数・補助席)
                        // WK7801(補助 801 調整数) = WK780Q(旧補助 801 調整数)
                        var copyZasekiKakuteiResult = new Z0003_Result();
                        copyZasekiKakuteiResult.ZasekiKagenTeiseki = z0003Result.OldZasekiKagenTeiseki;
                        copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.OldZasekiKagenSub;
                        copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.OldKusekiNumTeiseki;
                        copyZasekiKakuteiResult.KusekiNumSub = z0003Result.OldKusekiNumSub;
                        copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.OldSubCyoseiSeatNum;
                        var zasekiKaihoUpdateQuery = this.createCrsZasekiDataForKikaku(copyZasekiKakuteiResult, oldZasekiData, oldCrsInfoEntity);
                        if (string.IsNullOrEmpty(zasekiKaihoUpdateQuery) == true)
                        {

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        updateCount = base.execNonQuery(oracleTransaction, zasekiKaihoUpdateQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }

                        string sharedBusCrsQuery = "";
                        // 共用コース座席更新
                        foreach (DataRow row in oldSharedZasekiData.Rows)
                        {
                            if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                            {
                                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                continue;
                            }

                            // 共用コース座席更新SQL作成
                            sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity);
                            updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                            if (updateCount <= Zero)
                            {
                                // 座席データの更新件数が0件以下の場合、処理終了

                                // ロールバック
                                base.callRollbackTransaction(oracleTransaction);
                                return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                            }
                        }
                    }
                    else
                    {
                        // コース台帳座席更新(旧座席数解放)
                        updateCount = this.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction);
                    }
                }
                else
                {
                    // コース台帳座席更新(旧座席数解放)
                    updateCount = this.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, registEntity.OldYoyakuNinzu * -1, oracleTransaction);
                }
            }
            else
            {
                // 予約人数の変更の場合

                int zasekiKagenSu = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu;
                if (zasekiKagenSu < Zero)
                {
                    // 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate;
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd;
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid;
                    var copyZasekiKakuteiResult = new Z0003_Result();
                    copyZasekiKakuteiResult.ZasekiKagenTeiseki = zasekiKagenSu;
                    copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.ZasekiKagenSub1F;
                    copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.KusekiNumTeiseki;
                    copyZasekiKakuteiResult.KusekiNumSub = z0003Result.KusekiNumSub;
                    copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.SubCyoseiSeatNum;
                    updateCount = this.updateKakuShashuZaseki(zasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result.SeatKbn, oracleTransaction);
                    if (updateCount <= 0)
                    {
                        // 座席データの更新件数が0件以下の場合、処理終了

                        // ロールバック
                        base.callRollbackTransaction(oracleTransaction);
                        return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                    }

                    string sharedBusCrsQuery = "";
                    // 共用コース座席更新
                    foreach (DataRow row in oldSharedZasekiData.Rows)
                    {
                        if (this.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) == false)
                        {
                            // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            continue;
                        }

                        // 共用コース座席更新SQL作成
                        sharedBusCrsQuery = this.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity);
                        updateCount = base.execNonQuery(oracleTransaction, sharedBusCrsQuery);
                        if (updateCount <= Zero)
                        {
                            // 座席データの更新件数が0件以下の場合、処理終了

                            // ロールバック
                            base.callRollbackTransaction(oracleTransaction);
                            return S02_0104.UpdateStatusZasekiKaihoUpdateFailure;
                        }
                    }
                }
            }

            // 部屋数解放SQL作成
            string roomiKaihoQuery = this.createZasekiKaihoSql(oldZasekiData, registEntity, oldCrsInfoEntity);
            if (string.IsNullOrEmpty(roomiKaihoQuery) == false)
            {
                updateCount = base.execNonQuery(oracleTransaction, roomiKaihoQuery);
                if (updateCount <= Zero)
                {
                    // 部屋残数解放の更新件数が0件以下の場合、処理終了

                    // ロールバック
                    base.callRollbackTransaction(oracleTransaction);
                    return S02_0104.UpdateStatusRoomKaihoUpdateFailure;
                }
            }

            // コミット
            base.callCommitTransaction(oracleTransaction);
        }
        catch (Exception ex)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            throw ex;
        }
        finally
        {
            // トランザクションの破棄
            oracleTransaction.Dispose();
        }

        // 不要な行削除
        this.deleteCrsChargeChargeKbnForShukuhakuAri(registEntity.YoyakuInfoCrsChargeChargeKbnList(0));
        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// 予約情報（ピックアップ）情報取得
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約情報（ピックアップ）情報</returns>
    public DataTable getYoyakuInfoPickupList(YoyakuInfoBasicEntity entity)
    {
        DataTable pickupList;
        try
        {
            string query = this.createYoyakuPickupInfoSql(entity);
            pickupList = base.getDataTable(query);
        }
        catch (Exception ex)
        {
            throw;
        }

        return pickupList;
    }

    /// <summary>
    /// 予約コース情報取得SQL作成
    /// </summary>
    /// <param name="entity">予約情報（基本）Entity</param>
    /// <returns>予約コース情報取得SQL</returns>
    private string createYoyakuCrsInfoSql(YoyakuInfoBasicEntity entity)
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
        sb.AppendLine("     ,CLB.UNKYU_KBN ");
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
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ");
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
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ");
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ");
        sb.AppendLine("     ,CLB.USING_FLG ");
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
        sb.AppendLine("     ,YIB.JYOSEI_SENYO ");
        sb.AppendLine("     ,YIB.AIBEYA_FLG ");
        sb.AppendLine("     ,YIB.YOYAKU_ZASEKI_GET_KBN ");
        sb.AppendLine("     ,YIB.TOBI_SEAT_FLG ");
        sb.AppendLine("     ,YIB.YOYAKU_ZASEKI_KBN ");
        sb.AppendLine("     ,YIB.GROUP_NO ");
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ");
        sb.AppendLine("     ,YIB.HAKKEN_DAY ");
        sb.AppendLine("     ,YIB.HAKKEN_NAIYO ");
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ");
        sb.AppendLine("     ,YIB.SEISAN_HOHO ");
        sb.AppendLine("     ,YIB.AGENT_CD ");
        sb.AppendLine("     ,YIB.AGENT_NM ");
        sb.AppendLine("     ,YIB.AGENT_TEL_NO ");
        sb.AppendLine("     ,YIB.ADD_CHARGE_MAEBARAI_KEI ");
        sb.AppendLine("     ,YIB.WARIBIKI_ALL_GAKU ");
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_CANCEL ");
        sb.AppendLine("     ,YIB.YYKMKS ");
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
    /// コース情報取得SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース情報取得SQL</returns>
    private string createCrsInfoSql(CrsLedgerBasicEntity entity)
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
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ");
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ");
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ");
        sb.AppendLine("     ,CLB.YOYAKU_NUM_SUB_SEAT ");
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
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ");
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ");
        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ");
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
        sb.AppendLine("     AND  ");
        sb.AppendLine("     CLB.SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        if (entity.gousya.Value is object && entity.gousya.Value > 0)
        {
            sb.AppendLine("     AND  ");
            sb.AppendLine("     CLB.GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 料金区分一覧取得SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity
    /// </param>
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
    /// コース台帳座席情報取得
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <param name="oracleTransaction">トランザクション</param>
    /// <returns>コース台帳座席情報</returns>
    private DataTable getCrsLenderZasekiDataForTeiki(CrsLedgerBasicEntity entity, OracleTransaction oracleTransaction)
    {
        DataTable zasekiData = default;
        int idx = 1;
        string query = createCrsLedgerZasekiSql(entity);
        while (idx <= UsingCheckRetryNum)
        {
            zasekiData = base.getDataTable(oracleTransaction, query);
            if (zasekiData.Rows.Count > Zero)
            {
                break;
            }

            // 座席データがない場合、リトライ
            idx = idx + 1;
        }

        return zasekiData;
    }

    /// <summary>
    /// コース使用中チェック
    /// 企画用
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="oracleTransaction">トランザクション</param>
    /// <param name="zasekiData">自コース座席情報</param>
    /// <param name="sharedZasekiData">共用コース座席情報</param>
    /// <returns>検証結果</returns>
    private bool isCrsLenderZasekiDataForKikaku(CrsLedgerBasicEntity entity, OracleTransaction oracleTransaction, ref DataTable zasekiData, ref DataTable sharedZasekiData)
    {
        bool isValid = false;
        int idx = 1;
        string busReserveCd = "";
        string sharedCrsQuery = "";
        while (idx <= UsingCheckRetryNum)
        {

            // 自コースの座席情報取得SQL作成
            string crsQuery = createCrsLedgerZasekiSql(entity);

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
            sharedCrsQuery = createSharedBusCrsZasekiSql(entity, busReserveCd);
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

            isValid = true;
            break;
        }

        return isValid;
    }

    /// <summary>
    /// コース台帳座席情報取得SQL作成
    /// 悲観ロック用
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席情報取得SQL</returns>
    private string createCrsLedgerZasekiSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
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
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" FOR UPDATE WAIT 10 ");
        return sb.ToString();
    }

    /// <summary>
    /// 共用バスコース座席情報取得SQL作成
    /// 悲観ロック用
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <param name="busReserveCd">バス指定コード</param>
    /// <returns>共用バスコース座席情報取得SQL</returns>
    private string createSharedBusCrsZasekiSql(CrsLedgerBasicEntity entity, string busReserveCd)
    {
        base.paramClear();
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
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        sb.AppendLine(" FOR UPDATE WAIT 10 ");
        return sb.ToString();
    }

    /// <summary>
    /// 架空車種座席更新
    /// </summary>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <param name="zasekiKagenSu">座席数</param>
    /// <param name="seatKbn">席区分</param>
    /// <param name="oracleTransaction">Oracleトランザクション</param>
    /// <returns>更新件数</returns>
    private int updateKakuShashuZaseki(DataTable crsLedgerZasekiData, CrsLedgerBasicEntity entity, int zasekiKagenSu, string seatKbn, OracleTransaction oracleTransaction)
    {
        var updateEntity = new CrsLedgerBasicEntity();
        updateEntity.crsCd.Value = entity.crsCd.Value;
        updateEntity.syuptDay.Value = entity.syuptDay.Value;
        updateEntity.gousya.Value = entity.gousya.Value;
        updateEntity.systemUpdateDay.Value = entity.systemUpdateDay.Value;
        updateEntity.systemUpdatePersonCd.Value = entity.systemUpdatePersonCd.Value;
        updateEntity.systemUpdatePgmid.Value = entity.systemUpdatePgmid.Value;
        if (string.IsNullOrEmpty(seatKbn) == true)
        {
            int yoyakuNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString());
            int kusekiNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString());
            updateEntity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu;
            updateEntity.kusekiNumTeiseki.Value = kusekiNumTeiseki - zasekiKagenSu;
        }
        else
        {
            int yoyakuNumSubSeat = int.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_SUB_SEAT").ToString());
            int kusekiNumSubSeat = int.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT").ToString());
            updateEntity.yoyakuNumSubSeat.Value = yoyakuNumSubSeat + zasekiKagenSu;
            updateEntity.kusekiNumSubSeat.Value = kusekiNumSubSeat - zasekiKagenSu;
        }

        // 架空車種座席更新SQL作成
        string query = createKakuShashuZasekiSql(updateEntity, seatKbn);
        int updateCount = base.execNonQuery(oracleTransaction, query);
        return updateCount;
    }

    /// <summary>
    /// 架空車種座席更新SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <param name="seatKbn">席区分</param>
    /// <returns>架空車種座席更新SQL</returns>
    private string createKakuShashuZasekiSql(CrsLedgerBasicEntity entity, string seatKbn)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        if (string.IsNullOrEmpty(seatKbn) == true)
        {
            sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
            sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        }
        else
        {
            sb.AppendLine("      YOYAKU_NUM_SUB_SEAT = " + base.setParam(entity.yoyakuNumSubSeat.PhysicsName, entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu));
            sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        }

        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 共用バス座席更新
    /// </summary>
    /// <param name="sharedZasekiData">共用バス座席情報</param>
    /// <param name="newCrsEntity">コース台帳（基本）Entity</param>
    /// <param name="z0003Result">共通処理「バス座席自動設定」</param>
    /// <param name="oracleTransaction">Oracleトランザクション</param>
    /// <returns>更新ステータス</returns>
    private string updateSharedBusCrsZaseki(DataTable sharedZasekiData, CrsLedgerBasicEntity newCrsEntity, Z0003_Result z0003Result, OracleTransaction oracleTransaction)
    {
        foreach (DataRow sharedRow in sharedZasekiData.Rows)
        {
            if (this.isSharedBusCrsEqualCheck(sharedRow, newCrsEntity) == false)
            {
                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                continue;
            }

            // 空席数ワーク
            int kusekiWork = 0;

            // 空席数定席
            int kusekiSuTeiseki = int.Parse(sharedRow("KUSEKI_NUM_TEISEKI").ToString());
            // 空席数ワーク = コース台帳（基本）.共通処理「NRV710R バス座席自動設定（予約変更）」.座席加減数・定席
            kusekiWork = kusekiSuTeiseki - z0003Result.ZasekiKagenTeiseki;
            if (kusekiWork >= z0003Result.KusekiNumTeiseki)
            {
                // 空席数ワークが 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・定席

                kusekiSuTeiseki = z0003Result.KusekiNumTeiseki;
            }
            else
            {
                kusekiSuTeiseki = kusekiWork;
            }

            // 空席数補助席
            int kusekiSuSubSeat = int.Parse(sharedRow("KUSEKI_NUM_SUB_SEAT").ToString());
            // 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席
            kusekiWork = kusekiSuSubSeat - z0003Result.KusekiNumSub;
            // 空席数ワーク = 空席数ワーク - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
            kusekiWork = kusekiWork - z0003Result.SubCyoseiSeatNum;
            if (kusekiWork >= z0003Result.KusekiNumSub)
            {
                // 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席

                kusekiSuSubSeat = z0003Result.KusekiNumSub;
            }
            else
            {
                kusekiSuSubSeat = kusekiWork;
            }

            // 空席数定席、空席数補助席が0を下回る場合、0にする
            if (kusekiSuTeiseki < CommonRegistYoyaku.ZERO)
            {
                kusekiSuTeiseki = CommonRegistYoyaku.ZERO;
            }

            if (kusekiSuSubSeat < CommonRegistYoyaku.ZERO)
            {
                kusekiSuSubSeat = CommonRegistYoyaku.ZERO;
            }

            // WHERE条件設定
            var sharedEntity = new CrsLedgerBasicEntity();
            sharedEntity.crsCd.Value = sharedRow("CRS_CD").ToString();
            sharedEntity.syuptDay.Value = int.Parse(sharedRow("SYUPT_DAY").ToString());
            sharedEntity.gousya.Value = int.Parse(sharedRow("GOUSYA").ToString());
            sharedEntity.systemUpdateDay.Value = newCrsEntity.systemUpdateDay.Value;
            sharedEntity.systemUpdatePersonCd.Value = newCrsEntity.systemUpdatePersonCd.Value;
            sharedEntity.systemUpdatePgmid.Value = newCrsEntity.systemUpdatePgmid.Value;
            string query = this.createSharedCrsZasekiUpdateSql(kusekiSuTeiseki, kusekiSuSubSeat, sharedEntity);
            var updateCount = base.execNonQuery(oracleTransaction, query);
            if (updateCount <= Zero)
            {
                // 座席データの更新件数が0件以下の場合、処理終了

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusZasekiUpdateFailure;
            }
        }

        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// 共用バス座席更新
    /// </summary>
    /// <param name="sharedZasekiData">共用バス座席情報</param>
    /// <param name="newCrsEntity">コース台帳（基本）Entity</param>
    /// <param name="zasekiKagenSu">座席加減数</param>
    /// <param name="z0003Result">共通処理「バス座席自動設定」</param>
    /// <param name="oracleTransaction">Oracleトランザクション</param>
    /// <returns>更新ステータス</returns>
    private string updateSharedBusCrsZaseki(DataTable sharedZasekiData, CrsLedgerBasicEntity newCrsEntity, int zasekiKagenSu, Z0003_Result z0003Result, OracleTransaction oracleTransaction)
    {
        foreach (DataRow sharedRow in sharedZasekiData.Rows)
        {
            if (this.isSharedBusCrsEqualCheck(sharedRow, newCrsEntity) == false)
            {
                // 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                continue;
            }

            // 空席数ワーク
            int kusekiWork = 0;

            // 空席数定席
            int kusekiSuTeiseki = int.Parse(sharedRow("KUSEKI_NUM_TEISEKI").ToString());
            // 空席数ワーク = コース台帳（基本）.空席数定席 - 座席加減数(新予約人数 - 旧予約人数)
            kusekiWork = kusekiSuTeiseki - zasekiKagenSu;
            if (kusekiWork >= z0003Result.KusekiNumTeiseki)
            {
                // 空席数ワークが 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・定席

                kusekiSuTeiseki = z0003Result.KusekiNumTeiseki;
            }
            else
            {
                kusekiSuTeiseki = kusekiWork;
            }

            // 空席数補助席
            int kusekiSuSubSeat = int.Parse(sharedRow("KUSEKI_NUM_SUB_SEAT").ToString());
            // 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席
            kusekiWork = kusekiSuSubSeat - z0003Result.KusekiNumSub;
            // 空席数ワーク = 空席数ワーク - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
            kusekiWork = kusekiWork - z0003Result.SubCyoseiSeatNum;
            if (kusekiWork >= z0003Result.KusekiNumSub)
            {
                // 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席

                kusekiSuSubSeat = z0003Result.KusekiNumSub;
            }
            else
            {
                kusekiSuSubSeat = kusekiWork;
            }

            // 空席数定席、空席数補助席が0を下回る場合、0にする
            if (kusekiSuTeiseki < CommonRegistYoyaku.ZERO)
            {
                kusekiSuTeiseki = CommonRegistYoyaku.ZERO;
            }

            if (kusekiSuSubSeat < CommonRegistYoyaku.ZERO)
            {
                kusekiSuSubSeat = CommonRegistYoyaku.ZERO;
            }

            // WHERE条件設定
            var sharedEntity = new CrsLedgerBasicEntity();
            sharedEntity.crsCd.Value = sharedRow("CRS_CD").ToString();
            sharedEntity.syuptDay.Value = int.Parse(sharedRow("SYUPT_DAY").ToString());
            sharedEntity.gousya.Value = int.Parse(sharedRow("GOUSYA").ToString());
            sharedEntity.systemUpdateDay.Value = newCrsEntity.systemUpdateDay.Value;
            sharedEntity.systemUpdatePersonCd.Value = newCrsEntity.systemUpdatePersonCd.Value;
            sharedEntity.systemUpdatePgmid.Value = newCrsEntity.systemUpdatePgmid.Value;
            string query = this.createSharedCrsZasekiUpdateSql(kusekiSuTeiseki, kusekiSuSubSeat, sharedEntity);
            var updateCount = base.execNonQuery(oracleTransaction, query);
            if (updateCount <= CommonRegistYoyaku.ZERO)
            {
                // 座席データの更新件数が0件以下の場合、処理終了

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusZasekiUpdateFailure;
            }
        }

        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// コース台帳座席更新
    /// </summary>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">座席更新の検索条件</param>
    /// <param name="zasekiKagenSu">座席加減数</param>
    /// <param name="oracleTransaction">オラクルトランザクション</param>
    /// <param name="kusekiTeiseki">空席数定席</param>
    /// <returns>更新件数</returns>
    private int updateKakuZasekiSu(DataTable crsLedgerZasekiData, CrsLedgerBasicEntity entity, int zasekiKagenSu, OracleTransaction oracleTransaction, [Optional, DefaultParameterValue(0)] ref int kusekiTeiseki)
    {

        // コース台帳座席更新SQL作成
        string query = this.createCrsKakuShashuZasekiData(crsLedgerZasekiData, entity, zasekiKagenSu, kusekiTeiseki);
        int updateCount = 0;
        if (string.IsNullOrEmpty(query) == true)
        {
            return updateCount;
        }

        updateCount = base.execNonQuery(oracleTransaction, query);
        return updateCount;
    }

    /// <summary>
    /// コース台帳座席更新SQL作成
    /// </summary>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">座席更新の検索条件</param>
    /// <param name="zasekiKagenSu">座席加減数</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsKakuShashuZasekiData(DataTable crsLedgerZasekiData, CrsLedgerBasicEntity entity, int zasekiKagenSu, ref int kusekiTeiseki)
    {
        int yoyakuNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString());
        int kusekiNumTeiseki = int.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString());
        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu;
        kusekiTeiseki = kusekiNumTeiseki - zasekiKagenSu;
        entity.kusekiNumTeiseki.Value = kusekiTeiseki;
        if (entity.kusekiNumTeiseki.Value < Zero)
        {
            // 空席数・定席が0を下回る場合
            return "";
        }

        // コース台帳架空車種座席更新SQL作成
        string query = createCrsKakuShashuZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// コース台帳座席更新SQL作成
    /// </summary>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">座席更新の検索条件</param>
    /// <param name="zasekiKagenSu">座席加減数</param>
    /// <param name="z0003Result">座席確定データ</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsKakuShashuZasekiData(DataTable crsLedgerZasekiData, ref CrsLedgerBasicEntity entity, int zasekiKagenSu, Z0003_Result z0003Result)
    {
        int yoyakuNumTeiseki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int kusekiNumTeiseki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));
        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu;
        entity.kusekiNumTeiseki.Value = kusekiNumTeiseki - zasekiKagenSu;
        int kusekiNumSubSeat = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"));

        // 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
        int kusekiWork = kusekiNumSubSeat - z0003Result.OldSubCyoseiSeatNum;
        if (kusekiWork >= z0003Result.OldKusekiNumSub)
        {
            // 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・補助席以上の場合
            // 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・補助席を設定
            entity.kusekiNumSubSeat.Value = z0003Result.OldKusekiNumSub;
        }
        else
        {
            // 上記以外、空席数ワークを設定
            entity.kusekiNumSubSeat.Value = kusekiWork;
        }

        string query = createCrsKakuZasekiUpdateSql(entity);
        return query;
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
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳架空車種座席更新SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席更新SQL</returns>
    private string createCrsKakuZasekiUpdateSql(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳座席情報更新SQL作成
    /// 定期用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <param name="kusekiTeisaki">空席数定席</param>
    /// <returns>コース台帳座席情報更新SQL</returns>
    private string createCrsZasekiDataForTeiki(Z0003_Result z0003Result, DataTable crsLedgerZasekiData, CrsLedgerBasicEntity entity, ref int kusekiTeisaki)
    {
        int crsYoyakuSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int crsKusekiSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));

        // 予約数定席
        // コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        int yoyakuSuTeisaki = crsYoyakuSuTeisaki + z0003Result.ZasekiKagenTeiseki;

        // 空席数定席算出
        // コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        kusekiTeisaki = this.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.ZasekiKagenTeiseki, z0003Result.KusekiNumTeiseki);

        // 空席数定席マイナスチェック
        if (kusekiTeisaki < CommonRegistYoyaku.ZERO)
        {
            // 空席数定席が'0'を下回る場合、エラーとして更新処理終了
            return "";
        }

        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki;
        entity.kusekiNumTeiseki.Value = kusekiTeisaki;

        // コース台帳座席更新SQL作成
        string query = createCrsZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// 共用バス座席情報更新SQL作成
    /// </summary>
    /// <param name="sharedRow">共用座席情報</param>
    /// <param name="ledgerBasicEntity">コース台帳（基本）</param>
    /// <param name="kusekiTeisaki">空席数定席</param>
    /// <returns>共用バス座席情報更新SQL</returns>
    private string createSharedCrsZasekiDataUpdateSqlForTeiki(DataRow sharedRow, CrsLedgerBasicEntity ledgerBasicEntity, int kusekiTeisaki)
    {
        var entity = new CrsLedgerBasicEntity();
        entity.kusekiNumTeiseki.Value = kusekiTeisaki;
        entity.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"));
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"));
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"));
        entity.systemUpdatePgmid.Value = ledgerBasicEntity.systemUpdatePgmid.Value;
        entity.systemUpdatePersonCd.Value = ledgerBasicEntity.systemUpdatePersonCd.Value;
        entity.systemUpdateDay.Value = ledgerBasicEntity.systemUpdateDay.Value;
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 共用バス座席情報更新SQL作成
    /// </summary>
    /// <param name="sharedRow"></param>
    /// <param name="ledgerBasicEntity"></param>
    /// <returns></returns>
    private string createShearedZasekiKaihoDateForTeiki(DataRow sharedRow, CrsLedgerBasicEntity ledgerBasicEntity)
    {
        var entity = new CrsLedgerBasicEntity();
        entity.kusekiNumTeiseki.Value = ledgerBasicEntity.kusekiNumTeiseki.Value;
        entity.kusekiNumSubSeat.Value = ledgerBasicEntity.kusekiNumSubSeat.Value;
        entity.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"));
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"));
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"));
        entity.systemUpdatePgmid.Value = ledgerBasicEntity.systemUpdatePgmid.Value;
        entity.systemUpdatePersonCd.Value = ledgerBasicEntity.systemUpdatePersonCd.Value;
        entity.systemUpdateDay.Value = ledgerBasicEntity.systemUpdateDay.Value;
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳座席情報更新SQL作成
    /// 定期用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    /// <param name="oldCrsLedgerZasekiData">旧コース台帳座席情報</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席情報更新SQL</returns>
    private string createCrsZasekiKaihoDateForTeiki(Z0003_Result z0003Result, DataTable oldCrsLedgerZasekiData, ref CrsLedgerBasicEntity entity)
    {
        int crsYoyakuSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int crsKusekiSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));
        int crsKusekiSuSuSeat = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"));

        // 予約数定席
        // コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.旧座席加減数・定席
        int yoyakuSuTeisaki = crsYoyakuSuTeisaki + z0003Result.OldZasekiKagenTeiseki;
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki;

        // 空席数定席算出
        // コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.旧座席加減数・定席, 共通処理「バス座席自動設定処理」.旧空席数・定席
        int kusekiSuTeisaki = this.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.OldZasekiKagenTeiseki, z0003Result.OldKusekiNumTeiseki);
        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki;

        // 空席数補助席算出
        // 空席数ワーク = コース台帳(基本).空席数補助席 - 共通処理「バス座席自動設定処理」.旧補助 801 調整数
        int kusekiWork = crsKusekiSuSuSeat - z0003Result.OldSubCyoseiSeatNum;
        if (kusekiWork >= z0003Result.OldKusekiNumSub)
        {
            // 空席数ワークが共通処理「バス座席自動設定処理」.旧空席数・補助席以上の場合、
            // 共通処理「バス座席自動設定処理」.旧空席数・補助席を設定
            entity.kusekiNumSubSeat.Value = z0003Result.OldKusekiNumSub;
        }
        else
        {
            // 上記以外は、空席数ワークを設定
            entity.kusekiNumSubSeat.Value = kusekiWork;
        }

        if (entity.kusekiNumSubSeat.Value < CommonRegistYoyaku.ZERO)
        {
            // 空席数補助席が0を下回る場合、強制的に0にする
            entity.kusekiNumSubSeat.Value = CommonRegistYoyaku.ZERO;
        }

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

        // コース台帳(基本).空席数定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席
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
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + base.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu));
        if (entity.yoyakuNumSubSeat.Value is object)
        {
            sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT = " + base.setParam(entity.yoyakuNumSubSeat.PhysicsName, entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu));
        }

        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        if (entity.kusekiNumSubSeat.Value is object)
        {
            sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        }

        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
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
/// 予約情報（コース料金_料金区分）取得SQL作成
/// </summary>
/// <param name="entity">予約情報（コース料金_料金区分）Entity</param>
/// <returns>予約情報（コース料金_料金区分）取得SQL</returns>
    private string createYoyakuInfoCrsChargeChargeKbn(YoyakuInfoCrsChargeChargeKbnEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" SELECT ");
        sb.AppendLine("      YOYAKU_KBN ");
        sb.AppendLine("     ,YOYAKU_NO ");
        sb.AppendLine("     ,KBN_NO ");
        sb.AppendLine("     ,CHARGE_KBN_JININ_CD ");
        sb.AppendLine("     ,CHARGE_KBN ");
        sb.AppendLine("     ,CARRIAGE ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_1 ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_2 ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_3 ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_4 ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_5 ");
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU ");
        sb.AppendLine("     ,TANKA_1 ");
        sb.AppendLine("     ,TANKA_2 ");
        sb.AppendLine("     ,TANKA_3 ");
        sb.AppendLine("     ,TANKA_4 ");
        sb.AppendLine("     ,TANKA_5 ");
        sb.AppendLine("     ,CANCEL_NINZU_1 ");
        sb.AppendLine("     ,CANCEL_NINZU_2 ");
        sb.AppendLine("     ,CANCEL_NINZU_3 ");
        sb.AppendLine("     ,CANCEL_NINZU_4 ");
        sb.AppendLine("     ,CANCEL_NINZU_5 ");
        sb.AppendLine("     ,CANCEL_NINZU ");
        sb.AppendLine("     ,ENTRY_DAY ");
        sb.AppendLine("     ,ENTRY_PERSON_CD ");
        sb.AppendLine("     ,ENTRY_PGMID ");
        sb.AppendLine("     ,ENTRY_TIME ");
        sb.AppendLine("     ,UPDATE_DAY ");
        sb.AppendLine("     ,UPDATE_PERSON_CD ");
        sb.AppendLine("     ,UPDATE_PGMID ");
        sb.AppendLine("     ,UPDATE_TIME ");
        sb.AppendLine("     ,DELETE_DAY ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ");
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ");
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ");
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ");
        sb.AppendLine(" FROM ");
        sb.AppendLine("     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN  ");
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報UPDATE SQL作成
    /// </summary>
    /// <param name="entity">対象Entity</param>
    /// <param name="physicsNameList">更新するカラム名一覧</param>
    /// <param name="tableName">テーブル名</param>
    /// <returns>予約情報UPDATE SQL</returns>
    private string createYoyakuInfoUpdateSql<T>(T entity, List<string> physicsNameList, string tableName)
    {
        string valueQuery = this.createUpdateSql<T>(entity, physicsNameList, tableName);
        var type = typeof(T);
        var yoyakuKbnProp = type.GetProperty("yoyakuKbn");
        var yoyakuNoProp = type.GetProperty("yoyakuNo");
        var sb = new StringBuilder();
        sb.AppendLine(valueQuery);
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     YOYAKU_KBN = " + base.setParam(((EntityKoumoku_MojiType)yoyakuKbnProp.GetValue(entity, null)).PhysicsName, ((EntityKoumoku_MojiType)yoyakuKbnProp.GetValue(entity, null)).Value, ((EntityKoumoku_MojiType)yoyakuKbnProp.GetValue(entity, null)).DBType, ((EntityKoumoku_MojiType)yoyakuKbnProp.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_MojiType)yoyakuKbnProp.GetValue(entity, null)).DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     YOYAKU_NO = " + base.setParam(((EntityKoumoku_NumberType)yoyakuNoProp.GetValue(entity, null)).PhysicsName, ((EntityKoumoku_NumberType)yoyakuNoProp.GetValue(entity, null)).Value, ((EntityKoumoku_NumberType)yoyakuNoProp.GetValue(entity, null)).DBType, ((EntityKoumoku_NumberType)yoyakuNoProp.GetValue(entity, null)).IntegerBu, ((EntityKoumoku_NumberType)yoyakuNoProp.GetValue(entity, null)).DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// コース台帳座席情報更新SQL作成
    /// 企画用
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    /// <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>コース台帳座席情報更新SQL</returns>
    private string createCrsZasekiDataForKikaku(Z0003_Result z0003Result, DataTable crsLedgerZasekiData, CrsLedgerBasicEntity entity)
    {
        int crsYoyakuSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"));
        int crsKusekiSuTeisaki = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"));

        // 予約数定席
        // コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        int yoyakuSuTeisaki = crsYoyakuSuTeisaki + z0003Result.ZasekiKagenTeiseki;
        // 空席数定席算出
        // コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        int kusekiSuTeisaki = this.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.ZasekiKagenTeiseki, z0003Result.KusekiNumTeiseki);
        // 空席数補助席算出
        // コース台帳(基本).空席数定席,共通処理「バス座席自動設定処理」.座席加減数・定席,共通処理「バス座席自動設定処理」.補助調整空席,共通処理「バス座席自動設定処理」.空席数/補助・1F
        int kusekiSuSubSeat = this.calcKusekiSuSubSeatForKikaku(crsKusekiSuTeisaki, z0003Result.ZasekiKagenTeiseki, z0003Result.SubCyoseiSeatNum, z0003Result.ZasekiKagenSub1F);
        if (kusekiSuTeisaki < CommonRegistYoyaku.ZERO || kusekiSuSubSeat < CommonRegistYoyaku.ZERO)
        {
            // 空席数定席、空席数補助席が'0'を下回る場合、エラーとして更新処理終了

            return "";
        }

        // 予約数定席
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki;

        // 予約数補助席
        if (string.IsNullOrEmpty(z0003Result.SeatKbn) == false)
        {
            // 共通処理「バス座席自動設定処理」.席区分が空以外の場合
            // 予約数補助席を設定(コース台帳（基本）.予約数・補助席 + 共通処理「バス座席自動設定処理」.座席加減数・補助席)

            entity.yoyakuNumSubSeat.Value = z0003Result.ZasekiKagenSub1F;
        }

        // 空席数定席
        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki;
        // 空席数補助席
        entity.kusekiNumSubSeat.Value = kusekiSuSubSeat;

        // コース台帳座席更新SQL作成
        string query = this.createCrsZasekiUpdateSql(entity);
        return query;
    }

    /// <summary>
    /// 空席数補助席算出
    /// </summary>
    /// <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    /// <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・定席</param>
    /// <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    /// <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    /// <returns></returns>
    private int calcKusekiSuSubSeatForKikaku(int crsKusekiSu, int comZasekiKagenSu, int comSubChoseiKusekiSu, int comKusekiSuSubSeat1F)
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
    /// 自コースと共用コースのコースコード、号車、出発日が同一チェック
    /// </summary>
    /// <param name="sharedRow">共用コースRow</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>検証結果</returns>
    private bool isSharedBusCrsEqualCheck(DataRow sharedRow, CrsLedgerBasicEntity entity)
    {
        string csharedCrsCd = sharedRow("CRS_CD").ToString();
        int csharedGousya = int.Parse(sharedRow("GOUSYA").ToString());
        int csharedSyuptDay = int.Parse(sharedRow("SYUPT_DAY").ToString());
        if (csharedCrsCd == entity.crsCd.Value && csharedGousya == entity.gousya.Value && csharedSyuptDay == entity.syuptDay.Value)
        {

            // 自コースと共用コースのコースコード、号車、出発日が同一の場合
            return false;
        }

        return true;
    }

    /// <summary>
    /// 共用コース座席更新SQL作成
    /// </summary>
    /// <param name="z0003Result">共通処理「バス座席自動設定処理」</param>
    /// <param name="sharedRow">共用コース座席情報</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>共用コース座席更新SQL</returns>
    private string createSharedBusCrsZasekiUpdateSql(Z0003_Result z0003Result, DataRow sharedRow, CrsLedgerBasicEntity entity)
    {
        int kusekiTeisaki = 0;
        int kusekiSubSeat = 0;
        int jyosyaCapacity = int.Parse(sharedRow("JYOSYA_CAPACITY").ToString());
        int capacityRegular = int.Parse(sharedRow("CAPACITY_REGULAR").ToString());
        int capacityHo1kai = int.Parse(sharedRow("CAPACITY_HO_1KAI").ToString());
        if (jyosyaCapacity >= capacityRegular + capacityHo1kai)
        {
            // コース台帳(基本).乗車定員が「コース台帳(基本).定員定 + コース台帳(基本).定員補1F」以上の場合

            // 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席とする
            kusekiTeisaki = z0003Result.KusekiNumTeiseki;
            // 空席数補助席を共通処理「バス座席自動設定処理」.空席数・補助席とする
            kusekiSubSeat = z0003Result.KusekiNumSub;
        }
        else
        {
            // それ以外の場合

            // 営BLK算出
            int eiBlockNum = calcEiBlockNum(sharedRow);
            if (jyosyaCapacity >= capacityRegular)
            {
                // コース台帳(基本).乗車定員がコース台帳(基本).定員定以上の場合

                if (capacityRegular - eiBlockNum <= z0003Result.KusekiNumTeiseki)
                {
                    // 「コース台帳(基本).定員定 - 営BLK算出結果」が共通処理「バス座席自動設定処理」.空席数・定席以下の場合

                    // 空席数・定席を「コース台帳(基本).定員定 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum;
                }
                else
                {
                    // それ以外の場合

                    // 空席数・定席を共通処理「バス座席自動設定処理」.空席数・定席とする
                    kusekiTeisaki = z0003Result.KusekiNumTeiseki;
                }

                int eiBlockHo = int.Parse(sharedRow("EI_BLOCK_HO").ToString());
                int yoyaokuNumSubSeat = int.Parse(sharedRow("YOYAKU_NUM_SUB_SEAT").ToString());
                if (capacityRegular - eiBlockHo - yoyaokuNumSubSeat <= z0003Result.KusekiNumSub)
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
                    kusekiSubSeat = z0003Result.KusekiNumSub;
                }
            }
            else
            {
                // それ以外の場合、

                if (jyosyaCapacity - eiBlockNum <= z0003Result.KusekiNumTeiseki)
                {
                    // 「コース台帳(基本).乗車定員 - 営BLK算出結果」が共通処理「バス座席自動設定処理」空席数・定席以下の場合

                    // 空席数・定席を「コース台帳(基本).乗車定員 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum;
                }
                else
                {
                    // それ以外の場合

                    // 空席数・定席を共通処理「バス座席自動設定処理」空席数・定席とする
                    kusekiTeisaki = z0003Result.KusekiNumTeiseki;
                }

                // 空席数・補助席は'0'とする
                kusekiSubSeat = CommonRegistYoyaku.ZERO;
            }
        }

        // WHERE条件設定
        var sharedEntity = new CrsLedgerBasicEntity();
        sharedEntity.crsCd.Value = entity.crsCd.Value;
        sharedEntity.syuptDay.Value = entity.syuptDay.Value;
        sharedEntity.gousya.Value = entity.gousya.Value;
        sharedEntity.systemUpdateDay.Value = entity.systemUpdateDay.Value;
        sharedEntity.systemUpdatePersonCd.Value = entity.systemUpdatePersonCd.Value;
        sharedEntity.systemUpdatePgmid.Value = entity.systemUpdatePgmid.Value;

        // 更新SQL作成
        string query = createSharedCrsZasekiUpdateSql(kusekiTeisaki, kusekiSubSeat, sharedEntity);
        return query;
    }

    /// <summary>
    /// 営BLK算出
    /// </summary>
    /// <param name="sharedRow">共用コース座席情報</param>
    /// <returns>営BLK</returns>
    private int calcEiBlockNum(DataRow sharedRow)
    {
        int eiBlockRegular = int.Parse(sharedRow("EI_BLOCK_REGULAR").ToString());
        int blockKakuhoNum = int.Parse(sharedRow("BLOCK_KAKUHO_NUM").ToString());
        int yoyakuNumTeisaki = int.Parse(sharedRow("YOYAKU_NUM_TEISEKI").ToString());

        // 営BLK = 「コース台帳(基本).営ブロック定 + コース台帳(基本).ブロック確保数 + コース台帳(基本).空席確保数 + コース台帳(基本).予約数・定席
        int eiBlk = eiBlockRegular + blockKakuhoNum + yoyakuNumTeisaki;
        return eiBlk;
    }

    /// <summary>
    /// 共用コース座席更新SQL作成
    /// </summary>
    /// <param name="kusekiTeisaki">空席数定席</param>
    /// <param name="kusekiSubSeat">空席数補助席</param>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>共用コース座席更新SQL</returns>
    private string createSharedCrsZasekiUpdateSql(int kusekiTeisaki, int kusekiSubSeat, CrsLedgerBasicEntity entity)
    {
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + base.setParam(entity.kusekiNumTeiseki.PhysicsName, kusekiTeisaki, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu));
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + base.setParam(entity.kusekiNumSubSeat.PhysicsName, kusekiSubSeat, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + base.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + base.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + base.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 予約情報更新
    /// </summary>
    /// <param name="registEntity">変更予約情報Entity</param>
    /// <param name="oracleTransaction">オラクルトランザクション</param>
    /// <returns>更新ステータス</returns>
    private string updateYoyakuInfo(ChangeYoyakuInfoEntity registEntity, OracleTransaction oracleTransaction)
    {
        int updateCount = 0;

        // 予約情報（基本）
        string yoyakuInfoQuery = this.createYoyakuInfoUpdateSql<YoyakuInfoBasicEntity>(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC");
        updateCount = base.execNonQuery(oracleTransaction, yoyakuInfoQuery);
        if (updateCount <= 0)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            return S02_0104.UpdateStatusYoyakuUpdateFailure;
        }

        // 予約情報（コース料金）
        string crsChargeQuery = this.createYoyakuInfoUpdateSql<YoyakuInfoCrsChargeEntity>(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE");
        updateCount = base.execNonQuery(oracleTransaction, crsChargeQuery);
        if (updateCount <= 0)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            return S02_0104.UpdateStatusYoyakuCrsChargeUpdateFailure;
        }

        // 予約情報（コース料金_料金区分）
        string chargeKbnQuery = this.createYoyakuInfoCrsChargeChargeKbn(registEntity.YoyakuInfoCrsChargeChargeKbnList(0));
        DataTable chargeKbnData = base.getDataTable(chargeKbnQuery);
        string chargeKbnRegistQuery = "";
        foreach (YoyakuInfoCrsChargeChargeKbnEntity entity in registEntity.YoyakuInfoCrsChargeChargeKbnList)
        {
            DataRow[] chargeKbnRow = chargeKbnData.Select(string.Format("KBN_NO = {0} AND CHARGE_KBN_JININ_CD = '{1}'", entity.kbnNo.Value, entity.chargeKbnJininCd.Value));
            if (chargeKbnRow.Length > Zero)
            {
                var valueQuery = this.createUpdateSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
                var whereQuery = new StringBuilder();
                whereQuery.AppendLine(valueQuery);
                whereQuery.AppendLine(" WHERE ");
                whereQuery.AppendLine("     YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
                whereQuery.AppendLine("     AND ");
                whereQuery.AppendLine("     YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
                whereQuery.AppendLine("     AND ");
                whereQuery.AppendLine("     KBN_NO = " + base.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu));
                whereQuery.AppendLine("     AND ");
                whereQuery.AppendLine("     CHARGE_KBN_JININ_CD = " + base.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu));
                chargeKbnRegistQuery = whereQuery.ToString();
            }
            else
            {
                chargeKbnRegistQuery = this.createInsertSql<YoyakuInfoCrsChargeChargeKbnEntity>(entity, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN");
            }

            updateCount = base.execNonQuery(oracleTransaction, chargeKbnRegistQuery);
            if (updateCount <= 0)
            {

                // ロールバック
                base.callRollbackTransaction(oracleTransaction);
                return S02_0104.UpdateStatusYoyakuChargeKbnUpdateFailure;
            }
        }

        // 予約情報２
        string yoyakuInfo2Query;
        string yoyaku2Query = this.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity);
        DataTable yoyakuInfo2 = base.getDataTable(yoyaku2Query);
        if (yoyakuInfo2.Rows.Count > 0)
        {
            yoyakuInfo2Query = this.createYoyakuInfoUpdateSql<YoyakuInfo2Entity>(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2");
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
            return S02_0104.UpdateStatusYoyakuInfo2UpdateFailure;
        }

        return S02_0104.UpdateStatusSucess;
    }

    /// <summary>
    /// 相部屋部屋残数更新SQL作成
    /// </summary>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="zasekiInfoEntity">座席情報Entity</param>
    /// <param name="newCrsLedgerBasicEntity">コース台帳（基本）Entity</param>
    /// <returns>相部屋部屋残数更新SQL</returns>
    private string createAibeyaRoomZansuSql(DataTable zasekiData, ZasekiInfoEntity zasekiInfoEntity, CrsLedgerBasicEntity newCrsLedgerBasicEntity)
    {
        string teiinseiFlag = zasekiData.Rows(0)("TEIINSEI_FLG").ToString();
        if (string.IsNullOrEmpty(teiinseiFlag) == false)
        {
            // 定員制の場合、ルームの残数管理は行わない
            return "";
        }

        int shoriDate = zasekiInfoEntity.ShoriDate.ToString("yyyyMMdd");
        if (zasekiInfoEntity.SyuptDay == shoriDate)
        {
            // 出発日当日の場合、ルームの残数管理は行わない
            return "";
        }

        // ROOM MAX定員数取得
        double roomMaxCap = getRoomMaxTein(newCrsLedgerBasicEntity);

        // 男性
        double aibeyaManNinzu = double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE").ToString());
        double w1rom1 = Math.Ceiling(aibeyaManNinzu / roomMaxCap);
        double yoyakuManNinzu = double.Parse(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value.ToString());
        aibeyaManNinzu = aibeyaManNinzu + yoyakuManNinzu;
        double w1rom2 = Math.Ceiling(aibeyaManNinzu / roomMaxCap);
        int manKagenRoomSu = (int)Math.Round(w1rom2 - w1rom1);

        // 女性
        double aibeyaWoManNinzu = double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI").ToString());
        w1rom1 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap);
        double yoyakuWomanNinzu = double.Parse(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value.ToString());
        aibeyaWoManNinzu = aibeyaWoManNinzu + yoyakuWomanNinzu;
        w1rom2 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap);
        int womanKagenRoomSu = (int)Math.Round(w1rom2 - w1rom1);

        // 相部屋予約人数男性
        newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value = zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE") + (int)Math.Round(aibeyaManNinzu);
        // 相部屋予約人数女性
        newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value = zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI") + (int)Math.Round(aibeyaWoManNinzu);
        // 部屋残数総計
        newCrsLedgerBasicEntity.roomZansuSokei.Value = int.Parse(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI").ToString()) - manKagenRoomSu - womanKagenRoomSu;
        // 予約済ＲＯＯＭ数
        newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value = int.Parse(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM").ToString()) + manKagenRoomSu + womanKagenRoomSu;
        // 部屋残数１人部屋 ～ 部屋残数５人部屋
        newCrsLedgerBasicEntity.roomZansuOneRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM");
        newCrsLedgerBasicEntity.roomZansuTwoRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM");
        newCrsLedgerBasicEntity.roomZansuThreeRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM");
        newCrsLedgerBasicEntity.roomZansuFourRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM");
        newCrsLedgerBasicEntity.roomZansuFiveRoom.Value = calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM");
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      AIBEYA_YOYAKU_NINZU_MALE =  " + base.setParam(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.PhysicsName, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.DBType, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.IntegerBu, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.DecimalBu));
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI =  " + base.setParam(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.PhysicsName, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.DBType, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.IntegerBu, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuOneRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuOneRoom.Value, newCrsLedgerBasicEntity.roomZansuOneRoom.DBType, newCrsLedgerBasicEntity.roomZansuOneRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuOneRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuTwoRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuTwoRoom.Value, newCrsLedgerBasicEntity.roomZansuTwoRoom.DBType, newCrsLedgerBasicEntity.roomZansuTwoRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuTwoRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuThreeRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuThreeRoom.Value, newCrsLedgerBasicEntity.roomZansuThreeRoom.DBType, newCrsLedgerBasicEntity.roomZansuThreeRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuThreeRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuFourRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFourRoom.Value, newCrsLedgerBasicEntity.roomZansuFourRoom.DBType, newCrsLedgerBasicEntity.roomZansuFourRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFourRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuFiveRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFiveRoom.Value, newCrsLedgerBasicEntity.roomZansuFiveRoom.DBType, newCrsLedgerBasicEntity.roomZansuFiveRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFiveRoom.DecimalBu));
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + base.setParam(newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.PhysicsName, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DBType, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.IntegerBu, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + base.setParam(newCrsLedgerBasicEntity.roomZansuSokei.PhysicsName, newCrsLedgerBasicEntity.roomZansuSokei.Value, newCrsLedgerBasicEntity.roomZansuSokei.DBType, newCrsLedgerBasicEntity.roomZansuSokei.IntegerBu, newCrsLedgerBasicEntity.roomZansuSokei.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(newCrsLedgerBasicEntity.crsCd.PhysicsName, newCrsLedgerBasicEntity.crsCd.Value, newCrsLedgerBasicEntity.crsCd.DBType, newCrsLedgerBasicEntity.crsCd.IntegerBu, newCrsLedgerBasicEntity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA =  " + base.setParam(newCrsLedgerBasicEntity.syuptDay.PhysicsName, newCrsLedgerBasicEntity.syuptDay.Value, newCrsLedgerBasicEntity.syuptDay.DBType, newCrsLedgerBasicEntity.syuptDay.IntegerBu, newCrsLedgerBasicEntity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY =  " + base.setParam(newCrsLedgerBasicEntity.gousya.PhysicsName, newCrsLedgerBasicEntity.gousya.Value, newCrsLedgerBasicEntity.gousya.DBType, newCrsLedgerBasicEntity.gousya.IntegerBu, newCrsLedgerBasicEntity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// ROOM MAX定員数取得
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>ROOM MAX定員数</returns>
    private double getRoomMaxTein(CrsLedgerBasicEntity entity)
    {
        base.paramClear();
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
        sb.AppendLine("     CRS_CD = " + base.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY = " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA = " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        DataTable roomMaxInfo = base.getDataTable(sb.ToString());
        const double roomMaxCap = 99d;
        if (roomMaxInfo.Rows.Count <= 0)
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
    /// <returns>部屋残数</returns>
    private int calcRoomZansuAibeyaAri(DataTable zasekiData, int manKagenSu, int womanKagenSu, string crsBlockColName, string roomZansuColName)
    {
        int crsBlockRoomSu = Zero;
        int crsRoomZanSu = Zero;
        int.TryParse(zasekiData.Rows(0)(crsBlockColName).ToString(), crsBlockRoomSu);
        int.TryParse(zasekiData.Rows(0)(roomZansuColName).ToString(), crsRoomZanSu);
        if (crsBlockRoomSu == Zero)
        {
            return crsRoomZanSu;
        }

        int roomZansu = crsRoomZanSu - manKagenSu - womanKagenSu;
        return roomZansu;
    }

    /// <summary>
    /// 部屋残数一覧取得
    /// </summary>
    /// <param name="zasekiInfoEntity">座席情報Entity</param>
    /// <param name="yoyakuChangeKbn">予約変更区分</param>
    /// <returns>部屋残数一覧</returns>
    private List<int> getRoomZansu(ZasekiInfoEntity zasekiInfoEntity, string yoyakuChangeKbn)
    {
        var list = new List<int>();
        if (yoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
        {
            list.Add(zasekiInfoEntity.Room1Num);
            list.Add(zasekiInfoEntity.Room2Num);
            list.Add(zasekiInfoEntity.Room3Num);
            list.Add(zasekiInfoEntity.Room4Num);
            list.Add(zasekiInfoEntity.Room5Num);
        }
        else if (string.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) == true && string.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) == true)
        {
            list.Add(this.getDifRoomNum(zasekiInfoEntity.Room1Num, zasekiInfoEntity.OldRoom1Num));
            list.Add(this.getDifRoomNum(zasekiInfoEntity.Room2Num, zasekiInfoEntity.OldRoom2Num));
            list.Add(this.getDifRoomNum(zasekiInfoEntity.Room3Num, zasekiInfoEntity.OldRoom3Num));
            list.Add(this.getDifRoomNum(zasekiInfoEntity.Room4Num, zasekiInfoEntity.OldRoom4Num));
            list.Add(this.getDifRoomNum(zasekiInfoEntity.Room5Num, zasekiInfoEntity.OldRoom5Num));
        }
        else
        {
            list.Add(zasekiInfoEntity.Room1Num);
            list.Add(zasekiInfoEntity.Room2Num);
            list.Add(zasekiInfoEntity.Room3Num);
            list.Add(zasekiInfoEntity.Room4Num);
            list.Add(zasekiInfoEntity.Room5Num);
        }

        return list;
    }

    /// <summary>
    /// 部屋差分数取得
    /// </summary>
    /// <param name="newRoomNum">新部屋数</param>
    /// <param name="oldRoomNum">旧部屋数</param>
    /// <returns></returns>
    private int getDifRoomNum(int newRoomNum, int oldRoomNum)
    {
        int roomNum = newRoomNum - oldRoomNum;
        if (roomNum < Zero)
        {
            roomNum = Zero;
        }

        return roomNum;
    }

    /// <summary>
    /// 部屋残数更新SQL作成
    /// </summary>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="newCrsLedgerBasicEntity">新コース台帳（基本）Entity</param>
    /// <param name="zasekiInfoEntity">座席情報Entity</param>
    /// <param name="yoyakuChangeKbn">予約変更区分</param>
    /// <returns>部屋残数更新SQL</returns>
    private string createCrsRoomAibeyaNashiUpdateSql(DataTable zasekiData, CrsLedgerBasicEntity newCrsLedgerBasicEntity, ZasekiInfoEntity zasekiInfoEntity, string yoyakuChangeKbn)
    {
        string teiinseiFlag = zasekiData.Rows(0)("TEIINSEI_FLG").ToString();
        if (string.IsNullOrEmpty(teiinseiFlag) == false)
        {
            // 定員制の場合、ルームの残数管理は行わない
            return "";
        }

        int shoriDate = zasekiInfoEntity.ShoriDate.ToString("yyyyMMdd");
        if (zasekiInfoEntity.SyuptDay == shoriDate)
        {
            // 出発日当日の場合、ルームの残数管理は行わない
            return "";
        }

        // 部屋残数一覧取得
        var roomList = getRoomZansu(zasekiInfoEntity, yoyakuChangeKbn);
        newCrsLedgerBasicEntity.roomZansuOneRoom.Value = calcCrsRoomAibeyaNashi(zasekiData, roomList[0], "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM");
        newCrsLedgerBasicEntity.roomZansuTwoRoom.Value = calcCrsRoomAibeyaNashi(zasekiData, roomList[1], "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM");
        newCrsLedgerBasicEntity.roomZansuThreeRoom.Value = calcCrsRoomAibeyaNashi(zasekiData, roomList[2], "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM");
        newCrsLedgerBasicEntity.roomZansuFourRoom.Value = calcCrsRoomAibeyaNashi(zasekiData, roomList[3], "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM");
        newCrsLedgerBasicEntity.roomZansuFiveRoom.Value = calcCrsRoomAibeyaNashi(zasekiData, roomList[4], "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM");

        // 予約部屋数総数算出
        int totalRoomsu = 0;
        foreach (int roomNum in roomList)
            totalRoomsu = totalRoomsu + roomNum;
        newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value = int.Parse(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM").ToString()) + totalRoomsu;
        newCrsLedgerBasicEntity.roomZansuSokei.Value = int.Parse(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI").ToString()) - totalRoomsu;
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE ");
        sb.AppendLine("     T_CRS_LEDGER_BASIC ");
        sb.AppendLine(" SET ");
        sb.AppendLine("      ROOM_ZANSU_ONE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuOneRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuOneRoom.Value, newCrsLedgerBasicEntity.roomZansuOneRoom.DBType, newCrsLedgerBasicEntity.roomZansuOneRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuOneRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuTwoRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuTwoRoom.Value, newCrsLedgerBasicEntity.roomZansuTwoRoom.DBType, newCrsLedgerBasicEntity.roomZansuTwoRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuTwoRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuThreeRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuThreeRoom.Value, newCrsLedgerBasicEntity.roomZansuThreeRoom.DBType, newCrsLedgerBasicEntity.roomZansuThreeRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuThreeRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuFourRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFourRoom.Value, newCrsLedgerBasicEntity.roomZansuFourRoom.DBType, newCrsLedgerBasicEntity.roomZansuFourRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFourRoom.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + base.setParam(newCrsLedgerBasicEntity.roomZansuFiveRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFiveRoom.Value, newCrsLedgerBasicEntity.roomZansuFiveRoom.DBType, newCrsLedgerBasicEntity.roomZansuFiveRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFiveRoom.DecimalBu));
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + base.setParam(newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.PhysicsName, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DBType, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.IntegerBu, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DecimalBu));
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + base.setParam(newCrsLedgerBasicEntity.roomZansuSokei.PhysicsName, newCrsLedgerBasicEntity.roomZansuSokei.Value, newCrsLedgerBasicEntity.roomZansuSokei.DBType, newCrsLedgerBasicEntity.roomZansuSokei.IntegerBu, newCrsLedgerBasicEntity.roomZansuSokei.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("     CRS_CD = " + base.setParam(newCrsLedgerBasicEntity.crsCd.PhysicsName, newCrsLedgerBasicEntity.crsCd.Value, newCrsLedgerBasicEntity.crsCd.DBType, newCrsLedgerBasicEntity.crsCd.IntegerBu, newCrsLedgerBasicEntity.crsCd.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     SYUPT_DAY =  " + base.setParam(newCrsLedgerBasicEntity.syuptDay.PhysicsName, newCrsLedgerBasicEntity.syuptDay.Value, newCrsLedgerBasicEntity.syuptDay.DBType, newCrsLedgerBasicEntity.syuptDay.IntegerBu, newCrsLedgerBasicEntity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA =  " + base.setParam(newCrsLedgerBasicEntity.gousya.PhysicsName, newCrsLedgerBasicEntity.gousya.Value, newCrsLedgerBasicEntity.gousya.DBType, newCrsLedgerBasicEntity.gousya.IntegerBu, newCrsLedgerBasicEntity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 部屋残数算出
    /// </summary>
    /// <param name="zasekiData">コース台帳座席情報</param>
    /// <param name="roomingBetuNinzu">予約部屋数</param>
    /// <param name="crsBlockColName">コースブロックカラム名</param>
    /// <param name="roomZansuColName">部屋残数カラム名</param>
    /// <returns>部屋残数</returns>
    private int calcCrsRoomAibeyaNashi(DataTable zasekiData, int roomingBetuNinzu, string crsBlockColName, string roomZansuColName)
    {
        int crsBlockRoomSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName));
        int crsRoomZanSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(roomZansuColName));
        if (crsBlockRoomSu == 0)
        {
            return crsRoomZanSu;
        }

        int roomZansu = crsRoomZanSu - roomingBetuNinzu;
        return roomZansu;
    }

    /// <summary>
    /// 旧部屋残数一覧取得
    /// </summary>
    /// <param name="zasekiInfoEntity">座席情報Entity</param>
    /// <param name="yoyakuChangeKbn">予約変更区分</param>
    /// <returns>部屋残数一覧</returns>
    private List<int> getOldRoomZansu(ZasekiInfoEntity zasekiInfoEntity, string yoyakuChangeKbn)
    {
        var list = new List<int>();
        if (yoyakuChangeKbn == S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko)
        {
            list.Add(zasekiInfoEntity.OldRoom1Num);
            list.Add(zasekiInfoEntity.OldRoom2Num);
            list.Add(zasekiInfoEntity.OldRoom3Num);
            list.Add(zasekiInfoEntity.OldRoom4Num);
            list.Add(zasekiInfoEntity.OldRoom5Num);
        }
        else if (string.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) == true && string.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) == true)
        {
            list.Add(this.getOldDifRoomNum(zasekiInfoEntity.Room1Num, zasekiInfoEntity.OldRoom1Num));
            list.Add(this.getOldDifRoomNum(zasekiInfoEntity.Room2Num, zasekiInfoEntity.OldRoom2Num));
            list.Add(this.getOldDifRoomNum(zasekiInfoEntity.Room3Num, zasekiInfoEntity.OldRoom3Num));
            list.Add(this.getOldDifRoomNum(zasekiInfoEntity.Room4Num, zasekiInfoEntity.OldRoom4Num));
            list.Add(this.getOldDifRoomNum(zasekiInfoEntity.Room5Num, zasekiInfoEntity.OldRoom5Num));
        }
        else if (string.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) == true && string.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) == false)
        {
            list.Add(zasekiInfoEntity.OldRoom1Num);
            list.Add(zasekiInfoEntity.OldRoom2Num);
            list.Add(zasekiInfoEntity.OldRoom3Num);
            list.Add(zasekiInfoEntity.OldRoom4Num);
            list.Add(zasekiInfoEntity.OldRoom5Num);
        }
        else if (string.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) == false && string.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) == true)
        {
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
        }
        else
        {
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
            list.Add(Zero);
        }

        return list;
    }

    /// <summary>
    /// 旧部屋差分数取得
    /// </summary>
    /// <param name="newRoomNum">新部屋数</param>
    /// <param name="oldRoomNum">旧部屋数</param>
    /// <returns></returns>
    private int getOldDifRoomNum(int newRoomNum, int oldRoomNum)
    {
        int roomNum = oldRoomNum - newRoomNum;
        if (roomNum < Zero)
        {
            roomNum = Zero;
        }

        return roomNum;
    }

    /// <summary>
    /// 相部屋仕様人数取得
    /// </summary>
    /// <param name="oldAibeyaYoyakuNinzu">旧相部屋人数</param>
    /// <param name="newAibeyaYoyakuNinzu">新相部屋人数</param>
    /// <returns>相部屋仕様人数</returns>
    private int getAibeyaUsingNinzu(int oldAibeyaYoyakuNinzu, int newAibeyaYoyakuNinzu)
    {
        int ninzu = oldAibeyaYoyakuNinzu - newAibeyaYoyakuNinzu;
        if (ninzu < Zero)
        {
            ninzu = Zero;
        }

        return ninzu;
    }

    /// <summary>
    /// 解放する部屋数算出
    /// </summary>
    /// <param name="oldZasekiData">旧座席情報</param>
    /// <param name="colName">相部屋のカラム名</param>
    /// <param name="aibeyaYoyakuNinzu">相部屋の予約人数</param>
    /// <param name="roomMaxCap">ROOM MAX定員数</param>
    /// <param name="aibeyaYoyakuNum">相部屋人数</param>
    /// <returns>解放部屋数</returns>
    private int getAibeyaKaihoNum(DataTable oldZasekiData, string colName, int aibeyaYoyakuNinzu, double roomMaxCap, ref int aibeyaYoyakuNum)
    {
        double aibeyaNinzu = double.Parse(oldZasekiData.Rows(0)(colName).ToString());
        double w1rom1 = Math.Ceiling(aibeyaNinzu / roomMaxCap);
        aibeyaNinzu = aibeyaNinzu - aibeyaYoyakuNinzu;
        aibeyaYoyakuNum = (int)Math.Round(aibeyaNinzu);
        double w1rom2 = Math.Ceiling(aibeyaNinzu / roomMaxCap);
        int kaihoRoomSu = (int)Math.Round(w1rom2 - w1rom1);
        return kaihoRoomSu;
    }

    /// <summary>
    /// 座席解放SQL作成
    /// </summary>
    /// <param name="entity">コース台帳（基本）Entity</param>
    /// <returns>座席解放SQL</returns>
    private string createZasekiKaihoSql(CrsLedgerBasicEntity entity)
    {
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
        sb.AppendLine("     SYUPT_DAY =  " + base.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu));
        sb.AppendLine("     AND ");
        sb.AppendLine("     GOUSYA =  " + base.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 部屋数解放SQL作成
    /// </summary>
    /// <param name="oldZasekiData">旧コース台帳座席情報</param>
    /// <param name="registEntity">変更予約情報Entity</param>
    /// <param name="oldCrsInfoEntity">旧コース台帳（基本）Entity</param>
    /// <returns>座席解放SQL</returns>
    private string createZasekiKaihoSql(DataTable oldZasekiData, ChangeYoyakuInfoEntity registEntity, CrsLedgerBasicEntity oldCrsInfoEntity)
    {
        int shoriDate = registEntity.ZasekiInfoEntity.ShoriDate.ToString("yyyyMMdd");
        if (oldCrsInfoEntity.syuptDay.Value == shoriDate)
        {
            // 出発日が当日の場合、ルームの残数管理は行わない
            return "";
        }

        string teiinseFlag = oldZasekiData.Rows(0)("TEIINSEI_FLG").ToString();
        if (string.IsNullOrEmpty(teiinseFlag) == false)
        {
            // 旧コースの定員制フラグが空以外の場合、ルームの残数管理は行わない
            return "";
        }

        // 旧部屋残数一覧取得
        var oldRoomList = this.getOldRoomZansu(registEntity.ZasekiInfoEntity, registEntity.YoyakuChangeKbn);
        // 合計旧部屋残数
        int totalRoomsu = 0;
        foreach (int roomNum in oldRoomList)
            totalRoomsu = totalRoomsu + roomNum;

        // 男性
        int aibeyaYoyakuNinzuMale = this.getAibeyaUsingNinzu(registEntity.ZasekiInfoEntity.OldAibeyaYoyakuNinzuMale, registEntity.ZasekiInfoEntity.AibeyaYoyakuNinzuMale);
        // 女性
        int aibeyaYoyakuNinzuJyosei = this.getAibeyaUsingNinzu(registEntity.ZasekiInfoEntity.OldAibeyaYoyakuNinzuJyosei, registEntity.ZasekiInfoEntity.AibeyaYoyakuNinzuJyosei);

        // ROOM MAX定員数取得
        double roomMaxCap = getRoomMaxTein(oldCrsInfoEntity);
        // 相部屋男性
        int aibeyaManNinzu = Zero;
        int kaihoRoomNumMan = getAibeyaKaihoNum(oldZasekiData, "AIBEYA_YOYAKU_NINZU_MALE", aibeyaYoyakuNinzuMale, roomMaxCap, ref aibeyaManNinzu);
        // 相部屋女性
        int aibeyaWomanNinzu = Zero;
        int kaihoRoomNumWoman = getAibeyaKaihoNum(oldZasekiData, "AIBEYA_YOYAKU_NINZU_JYOSEI", aibeyaYoyakuNinzuJyosei, roomMaxCap, ref aibeyaWomanNinzu);

        // Entity設定
        // 相部屋予約人数男性
        oldCrsInfoEntity.aibeyaYoyakuNinzuMale.Value = aibeyaManNinzu;
        // 相部屋予約人数女性
        oldCrsInfoEntity.aibeyaYoyakuNinzuJyosei.Value = aibeyaWomanNinzu;
        // 予約済ＲＯＯＭ数
        oldCrsInfoEntity.yoyakuAlreadyRoomNum.Value = oldZasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM") - totalRoomsu - kaihoRoomNumMan - kaihoRoomNumWoman;
        // 部屋残数総計
        oldCrsInfoEntity.roomZansuSokei.Value = oldZasekiData.Rows(0)("ROOM_ZANSU_SOKEI") + totalRoomsu + kaihoRoomNumMan + kaihoRoomNumWoman;

        // 部屋残数１人部屋 ～ 部屋残数５人部屋(相部屋)
        oldCrsInfoEntity.roomZansuOneRoom.Value = calcRoomZansuAibeyaAri(oldZasekiData, kaihoRoomNumMan * -1, kaihoRoomNumWoman * -1, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM");
        oldCrsInfoEntity.roomZansuTwoRoom.Value = calcRoomZansuAibeyaAri(oldZasekiData, kaihoRoomNumMan * -1, kaihoRoomNumWoman * -1, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM");
        oldCrsInfoEntity.roomZansuThreeRoom.Value = calcRoomZansuAibeyaAri(oldZasekiData, kaihoRoomNumMan * -1, kaihoRoomNumWoman * -1, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM");
        oldCrsInfoEntity.roomZansuFourRoom.Value = calcRoomZansuAibeyaAri(oldZasekiData, kaihoRoomNumMan * -1, kaihoRoomNumWoman * -1, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM");
        oldCrsInfoEntity.roomZansuFiveRoom.Value = calcRoomZansuAibeyaAri(oldZasekiData, kaihoRoomNumMan * -1, kaihoRoomNumWoman * -1, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM");

        // 部屋残数１人部屋 ～ 部屋残数５人部屋
        oldCrsInfoEntity.roomZansuOneRoom.Value = this.calcCrsRoomAibeyaNashi(oldZasekiData, oldRoomList[0] * -1, oldCrsInfoEntity.roomZansuOneRoom.Value, "CRS_BLOCK_ONE_1R");
        oldCrsInfoEntity.roomZansuTwoRoom.Value = this.calcCrsRoomAibeyaNashi(oldZasekiData, oldRoomList[1] * -1, oldCrsInfoEntity.roomZansuTwoRoom.Value, "CRS_BLOCK_TWO_1R");
        oldCrsInfoEntity.roomZansuThreeRoom.Value = this.calcCrsRoomAibeyaNashi(oldZasekiData, oldRoomList[2] * -1, oldCrsInfoEntity.roomZansuThreeRoom.Value, "CRS_BLOCK_THREE_1R");
        oldCrsInfoEntity.roomZansuFourRoom.Value = this.calcCrsRoomAibeyaNashi(oldZasekiData, oldRoomList[3] * -1, oldCrsInfoEntity.roomZansuFourRoom.Value, "CRS_BLOCK_FOUR_1R");
        oldCrsInfoEntity.roomZansuFiveRoom.Value = this.calcCrsRoomAibeyaNashi(oldZasekiData, oldRoomList[4] * -1, oldCrsInfoEntity.roomZansuFiveRoom.Value, "CRS_BLOCK_FIVE_1R");
        string zasekiKaihoQuery = createZasekiKaihoSql(oldCrsInfoEntity);
        return zasekiKaihoQuery;
    }

    /// <summary>
    /// 部屋残数算出
    /// </summary>
    /// <param name="zasekiData">座席情報</param>
    /// <param name="roomingBetuNinzu">部屋別人数</param>
    /// <param name="roomZansu">部屋残数</param>
    /// <param name="crsBlockColName">コースブロックカラム名</param>
    /// <returns>部屋残数</returns>
    private int calcCrsRoomAibeyaNashi(DataTable zasekiData, int roomingBetuNinzu, int? roomZansu, string crsBlockColName)
    {
        int crsBlockRoomSu = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName));
        int crsRoomZanSu = (int)roomZansu;
        if (crsBlockRoomSu == 0)
        {
            // コースブロック管理されていない場合
            return crsRoomZanSu;
        }

        int roomsu = crsRoomZanSu - roomingBetuNinzu;
        return roomsu;
    }

    /// <summary>
    /// 不要行削除
    /// （宿泊なし）
    /// </summary>
    /// <param name="entity">予約情報（コース料金_料金区分）</param>
    /// <returns></returns>
    private bool deleteCrsChargeChargeKbnForShukuhakuNashi(YoyakuInfoCrsChargeChargeKbnEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            var sb = new StringBuilder();
            sb.AppendLine("DELETE FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
            sb.AppendLine("WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
            sb.AppendLine("AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_1 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_1 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU = 0 ");
            updateCount = base.execNonQuery(oracleTransaction, sb.ToString());

            // コミット
            base.callCommitTransaction(oracleTransaction);
        }
        catch (Exception ex)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            return false;
        }
        finally
        {
            // トランザクションの破棄
            oracleTransaction.Dispose();
        }

        return true;
    }

    /// <summary>
    /// 不要行削除
    /// （宿泊あり）
    /// </summary>
    /// <param name="entity">予約情報（コース料金_料金区分）</param>
    /// <returns></returns>
    private bool deleteCrsChargeChargeKbnForShukuhakuAri(YoyakuInfoCrsChargeChargeKbnEntity entity)
    {
        OracleTransaction oracleTransaction = default;
        int updateCount = 0;
        try
        {
            // トランザクション開始
            oracleTransaction = base.callBeginTransaction();
            var sb = new StringBuilder();
            sb.AppendLine("DELETE FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ");
            sb.AppendLine("WHERE YOYAKU_KBN = " + base.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu));
            sb.AppendLine("AND YOYAKU_NO = " + base.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu));
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_1 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_2 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_3 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_4 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_5 = 0 ");
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_1 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_2 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_3 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_4 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU_5 = 0 ");
            sb.AppendLine("AND CANCEL_NINZU = 0 ");
            updateCount = base.execNonQuery(oracleTransaction, sb.ToString());

            // コミット
            base.callCommitTransaction(oracleTransaction);
        }
        catch (Exception ex)
        {

            // ロールバック
            base.callRollbackTransaction(oracleTransaction);
            return false;
        }
        finally
        {
            // トランザクションの破棄
            oracleTransaction.Dispose();
        }

        return true;
    }

    /// <summary>
    /// バス座席自動設定処理
    /// </summary>
    /// <param name="z0003Param"></param>
    /// <param name="groupNo">グループNO</param>
    /// <returns>座席自動配置（予約変更）結果</returns>
    private Z0001_Result getCommonZasekiJidoData(Z0003_Param z0003Param, int groupNo)
    {
        var param = new Z0001_Param();
        param.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_40;
        param.CrsCd = z0003Param.CrsCd;
        param.SyuptDay = z0003Param.SyuptDay;
        param.Gousya = z0003Param.Gousya;
        param.BusReserveCd = z0003Param.BusReserveCd;
        param.Ninzu = z0003Param.Ninzu;
        param.JyoseiSenyoSeatFlg = z0003Param.JyoseiSenyoSeatFlg;
        param.ZasekiReserveKbn = z0003Param.ZasekiReserveKbn;
        param.YoyakuKbn = z0003Param.YoyakuKbn;
        param.YoyakuNo = z0003Param.YoyakuNO;
        param.GroupNo = groupNo;
        var z0001 = new Z0001();
        Z0001_Result result = z0001.Execute(param);
        return result;
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
        sb.AppendLine("     ,YIP.PICKUP_ROUTE_CD ");
        sb.AppendLine("     ,PRL.CRS_JYOSYA_TI ");
        sb.AppendLine("     ,RLH.SYUPT_TIME ");
        sb.AppendLine("     ,YIP.NINZU ");
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