Imports System.Reflection
Imports System.Text
Imports Hatobus.ReservationManagementSystem.Zaseki

''' <summary>
''' 予約の変更DA
''' </summary>
Public Class S02_0104Da
    Inherits DataAccessorBase

#Region "定数/変数"

    ''' <summary>
    ''' 使用中チェックリトライ回数：100
    ''' </summary>
    Private Const UsingCheckRetryNum As Integer = 100
    ''' <summary>
    ''' ゼロ
    ''' </summary>
    Private Const Zero As Integer = 0
    ''' <summary>
    ''' 架空車種：XX
    ''' </summary>
    Private Const KakuShashu As String = "XX"

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 予約コース情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約コース情報取得</returns>
    Public Function getYoyakuCrsInfo(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuCrsInfo As DataTable

        Try
            Dim query As String = Me.createYoyakuCrsInfoSql(entity)
            yoyakuCrsInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuCrsInfo
    End Function

    ''' <summary>
    ''' 予約料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約料金区分一覧</returns>
    Public Function getYoyakuChargeKbnList(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuChargeKbnList As DataTable

        Try

            Dim query As String = Me.createYoyakuChargeKbnListSql(entity)
            yoyakuChargeKbnList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuChargeKbnList
    End Function

    ''' <summary>
    ''' 予約料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約料金区分一覧</returns>
    Public Function getYoyakuChargeList(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuChargeList As DataTable

        Try
            Dim query As String = Me.createYoyakuChargeListSql(entity)
            yoyakuChargeList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuChargeList
    End Function

    ''' <summary>
    ''' コース情報取得
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース情報一覧</returns>
    Public Function getCrsInfoList(entity As CrsLedgerBasicEntity) As DataTable

        Dim crsInfoList As DataTable

        Try
            Dim query As String = Me.createCrsInfoSql(entity)
            crsInfoList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return crsInfoList
    End Function

    ''' <summary>
    ''' 料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>料金区分一覧</returns>
    Public Function getChargeKbnList(entity As CrsLedgerChargeEntity) As DataTable

        Dim chargeKbnList As DataTable

        Try

            Dim query As String = Me.createChargeKbnListSql(entity)
            chargeKbnList = MyBase.getDataTable(query)
        Catch ex As Exception
            Throw ex
        End Try

        Return chargeKbnList
    End Function

    ''' <summary>
    ''' 空席情報取得
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>空席情報</returns>
    Public Function getCrsLedgerBasicKusekiNumData(entity As CrsLedgerBasicEntity) As DataTable

        Dim kusekiData As DataTable = Nothing

        Try

            Dim query As String = Me.createCrsLedgerBasicKusekiNumDataSql(entity)
            kusekiData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return kusekiData
    End Function

    ''' <summary>
    ''' 予約の変更処理
    ''' 定期用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="registEntity">変更予約情報Entity</param>
    ''' <returns>座席変更ステータス</returns>
    Public Function updateYoyakuInfoForTeiki(z0003Result As Z0003_Result, registEntity As ChangeYoyakuInfoEntity, z0003param As Z0003_Param) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim crsLedgerZasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席情報更新
            If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    If z0003Result.CarTypeCd <> KakuShashu Then

                        ' コース台帳座席情報更新SQL作成
                        Dim kusekiTeiseki As Integer = 0
                        Dim zasekiUpdateQuery As String = Me.createCrsZasekiDataForTeiki(z0003Result, crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                        If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)
                        If updateCount <= 0 Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        For Each row As DataRow In sharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    Else
                        ' コース台帳座席更新
                        Dim kusekiTeiseki As Integer = 0
                        updateCount = Me.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, oracleTransaction, kusekiTeiseki)
                        If updateCount <= 0 Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        For Each row As DataRow In sharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    End If
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        Dim kusekiTeiseki As Integer = 0
                        updateCount = Me.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki)
                        If updateCount <= 0 Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        For Each row As DataRow In sharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    End If
                End If
            Else
                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    ' コース台帳座席更新
                    Dim kusekiTeiseki As Integer = 0
                    updateCount = Me.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, oracleTransaction, kusekiTeiseki)
                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiUpdateFailure
                    End If

                    For Each row As DataRow In sharedZasekiData.Rows

                        If Me.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) = False Then
                            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            Continue For
                        End If

                        ' 共用バス座席情報更新SQL作成
                        Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                        updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If
                    Next
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        ' コース台帳座席更新
                        Dim kusekiTeiseki As Integer = 0
                        updateCount = Me.updateKakuZasekiSu(crsLedgerZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki)
                        If updateCount <= 0 Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        For Each row As DataRow In sharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, registEntity.NewCrsLedgerBasicEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, registEntity.NewCrsLedgerBasicEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    End If
                End If
            End If

            ' 変更履歴登録
            Dim historyRec As OutputYoyakuChangeHistoryParam = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList)
            If historyRec Is Nothing Then

                Return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure
            End If

            ' 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq

            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName)
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName)

            ' 予約情報更新
            Dim updateStatus As String = Me.updateYoyakuInfo(registEntity, oracleTransaction)
            If updateStatus <> S02_0104.UpdateStatusSucess Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return updateStatus
            End If

            ' ピックアップルート台帳 （ホテル）
            If registEntity.PickupRouteLedgerHotelList IsNot Nothing Then

                For Each entity As PickupRouteLedgerHotelEntity In registEntity.PickupRouteLedgerHotelList

                    ' ピックアップルート台帳 （ホテル）人数更新SQL作成
                    Dim pickupHotelQuery As String = Me.createPickupRouteLedgerHotelUpdateSql(entity)
                    updateCount = MyBase.execNonQuery(oracleTransaction, pickupHotelQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure
                    End If
                Next
            End If

            ' 旧座席情報取得
            Dim oldCrsInfoEntity As New CrsLedgerBasicEntity()
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya

            Dim oldCrsLedgerZasekiData As DataTable = Nothing
            Dim oldSharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldCrsLedgerZasekiData, oldSharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席確定処理実施
            Dim zasekiKakuteiResult As Z0001_Result = Me.getCommonZasekiJidoData(z0003param, CInt(registEntity.YoyakuInfoBasicEntity.groupNo.Value))

            ' 前回確保していた座席数を加減
            If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then
                '出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                    If z0003Result.OldCarTypeCd <> KakuShashu Then

                        ' コース台帳座席情報更新SQL作成
                        Dim query As String = Me.createCrsZasekiKaihoDateForTeiki(z0003Result, oldCrsLedgerZasekiData, oldCrsInfoEntity)
                        updateCount = MyBase.execNonQuery(oracleTransaction, query)

                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createShearedZasekiKaihoDateForTeiki(row, oldCrsInfoEntity)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    Else
                        ' コース台帳座席更新(旧座席数解放)
                        Dim kusekiTeiseki As Integer = 0
                        updateCount = Me.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction, kusekiTeiseki)
                        If updateCount <= 0 Then
                            ' 座席解放処理にて、更新件数が0件の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    End If
                Else
                    ' コース台帳座席更新(旧座席数解放)
                    Dim kusekiTeiseki As Integer = 0
                    updateCount = Me.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction, kusekiTeiseki)
                    If updateCount <= 0 Then
                        ' 座席解放処理にて、更新件数が0件の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                    End If

                    For Each row As DataRow In oldSharedZasekiData.Rows

                        If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            Continue For
                        End If

                        ' 共用バス座席情報更新SQL作成
                        Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki)
                        updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If
                    Next
                End If
            Else
                ' 予約人数の変更の場合
                Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu

                If zasekiKagenSu < Zero Then
                    ' 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                    If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                        ' コース台帳座席更新SQL作成
                        Dim query As String = Me.createCrsKakuShashuZasekiData(oldCrsLedgerZasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result)
                        updateCount = MyBase.execNonQuery(oracleTransaction, query)
                        If updateCount <= 0 Then
                            ' 座席解放処理にて、更新件数が0件の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        ' 自コースを元に、加減する RefでEntityをもらう？
                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createShearedZasekiKaihoDateForTeiki(row, oldCrsInfoEntity)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    Else
                        ' コース台帳座席更新(旧座席数解放)
                        Dim kusekiTeiseki As Integer = 0
                        updateCount = Me.updateKakuZasekiSu(oldCrsLedgerZasekiData, oldCrsInfoEntity, zasekiKagenSu, oracleTransaction, kusekiTeiseki)
                        If updateCount <= 0 Then
                            ' 座席解放処理にて、更新件数が0件の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用バス座席情報更新SQL作成
                            Dim sharedZasekiQuery As String = Me.createSharedCrsZasekiDataUpdateSqlForTeiki(row, oldCrsInfoEntity, kusekiTeiseki)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedZasekiQuery)
                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiUpdateFailure
                            End If
                        Next
                    End If
                End If
            End If

            ' コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        ' 不要行削除
        Me.deleteCrsChargeChargeKbnForShukuhakuNashi(registEntity.YoyakuInfoCrsChargeChargeKbnList(0))

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約の変更処理
    ''' 企画（日帰り）用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="registEntity">変更予約情報Entity</param>
    ''' <returns>座席変更ステータス</returns>
    Public Function updateZasekiInfoForKikakuHigaeri(z0003Result As Z0003_Result, registEntity As ChangeYoyakuInfoEntity, z0003param As Z0003_Param) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席情報更新
            Dim zasekiUpdateQuery As String = ""
            If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    If z0003Result.CarTypeCd <> KakuShashu Then

                        zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0003Result, zasekiData, registEntity.NewCrsLedgerBasicEntity)
                        If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)
                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    Else
                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                End If
            Else
                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    ' 席区分取得
                    Dim seatKbn As String = z0003Result.SeatKbn
                    ' コース台帳座席更新
                    updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction)

                    If updateCount <= Zero Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiUpdateFailure
                    End If

                    ' 共用バス更新
                    Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction)
                    If status <> S02_0104.UpdateStatusSucess Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return status
                    End If
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                End If
            End If

            ' 変更履歴登録
            Dim historyRec As OutputYoyakuChangeHistoryParam = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList)
            If historyRec Is Nothing Then

                Return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure
            End If

            ' 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq

            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName)
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName)

            ' 予約情報更新
            Dim updateStatus As String = Me.updateYoyakuInfo(registEntity, oracleTransaction)
            If updateStatus <> S02_0104.UpdateStatusSucess Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return updateStatus
            End If

            ' ピックアップルート台帳 （ホテル）
            If registEntity.PickupRouteLedgerHotelList IsNot Nothing Then

                For Each entity As PickupRouteLedgerHotelEntity In registEntity.PickupRouteLedgerHotelList

                    ' ピックアップルート台帳 （ホテル）人数更新SQL作成
                    Dim pickupHotelQuery As String = Me.createPickupRouteLedgerHotelUpdateSql(entity)
                    updateCount = MyBase.execNonQuery(oracleTransaction, pickupHotelQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure
                    End If
                Next
            End If

            ' 旧座席情報取得
            Dim oldCrsInfoEntity As New CrsLedgerBasicEntity()
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya

            Dim oldZasekiData As DataTable = Nothing
            Dim oldSharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldZasekiData, oldSharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席確定処理実施
            Dim zasekiKakuteiResult As Z0001_Result = Me.getCommonZasekiJidoData(z0003param, CInt(registEntity.YoyakuInfoBasicEntity.groupNo.Value))

            ' 前回確保していた座席数を加減
            If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then
                '出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                    If z0003Result.OldCarTypeCd <> KakuShashu Then
                        'If True Then

                        ' 座席確定DataTableをコピーし、旧座席数などを新側の変数に代入する
                        ' コピーする内容は以下
                        ' WK7YST(座席加減数・定席) = WK7YTQ(旧座席加減数・定席)
                        ' WK7YSH(座席加減数・補助席) = WK7YHQ(旧座席加減数・補助)
                        ' WK7KTS(空席数・定席) = WK7KTQ(旧空席数・定席)
                        ' WK7KHJ(空席数・補助席) = WK7KHQ(旧空席数・補助席)
                        ' WK7801(補助 801 調整数) = WK780Q(旧補助 801 調整数)
                        Dim copyZasekiKakuteiResult As New Z0003_Result()
                        copyZasekiKakuteiResult.ZasekiKagenTeiseki = z0003Result.OldZasekiKagenTeiseki
                        copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.OldZasekiKagenSub
                        copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.OldKusekiNumTeiseki
                        copyZasekiKakuteiResult.KusekiNumSub = z0003Result.OldKusekiNumSub
                        copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.OldSubCyoseiSeatNum

                        Dim zasekiKaihoUpdateQuery = Me.createCrsZasekiDataForKikaku(copyZasekiKakuteiResult, oldZasekiData, oldCrsInfoEntity)
                        If String.IsNullOrEmpty(zasekiKaihoUpdateQuery) = True Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        updateCount = MyBase.execNonQuery(oracleTransaction, zasekiKaihoUpdateQuery)
                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        Dim sharedBusCrsQuery As String = ""
                        ' 共用コース座席更新
                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用コース座席更新SQL作成
                            sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                            End If
                        Next
                    Else
                        ' コース台帳座席更新(旧座席数解放)
                        updateCount = Me.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction)
                    End If
                Else
                    ' コース台帳座席更新(旧座席数解放)
                    updateCount = Me.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction)
                End If
            Else
                ' 予約人数変更の場合

                Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                If zasekiKagenSu < Zero Then
                    ' 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                    Dim copyZasekiKakuteiResult As New Z0003_Result()
                    copyZasekiKakuteiResult.ZasekiKagenTeiseki = zasekiKagenSu
                    copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.ZasekiKagenSub1F
                    copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.KusekiNumTeiseki
                    copyZasekiKakuteiResult.KusekiNumSub = z0003Result.KusekiNumSub
                    copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.SubCyoseiSeatNum

                    updateCount = Me.updateKakuShashuZaseki(zasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result.SeatKbn, oracleTransaction)
                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                    End If

                    Dim sharedBusCrsQuery As String = ""
                    ' 共用コース座席更新
                    For Each row As DataRow In oldSharedZasekiData.Rows

                        If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            Continue For
                        End If

                        ' 共用コース座席更新SQL作成
                        sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity)
                        updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If
                    Next
                End If
            End If

            ' コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw ex
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        ' 不要な行削除
        Me.deleteCrsChargeChargeKbnForShukuhakuNashi(registEntity.YoyakuInfoCrsChargeChargeKbnList(0))

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約の変更処理
    ''' 企画（宿泊）用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="registEntity">変更予約情報Entity</param>
    ''' <returns>座席変更ステータス</returns>
    Public Function updateZasekiInfoForKikakuShukuhaku(z0003Result As Z0003_Result, registEntity As ChangeYoyakuInfoEntity, z0003param As Z0003_Param) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(registEntity.NewCrsLedgerBasicEntity, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席情報更新
            Dim zasekiUpdateQuery As String = ""
            If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    If z0003Result.CarTypeCd <> KakuShashu Then

                        ' コース台帳座席情報更新SQL作成
                        zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0003Result, zasekiData, registEntity.NewCrsLedgerBasicEntity)
                        If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusKusekiNothing
                        End If

                        ' 共用バス座席更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    Else
                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                End If
            Else
                If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

                    ' 席区分取得
                    Dim seatKbn As String = z0003Result.SeatKbn
                    ' コース台帳座席更新
                    updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, seatKbn, oracleTransaction)

                    If updateCount <= Zero Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiUpdateFailure
                    End If

                    ' 共用バス更新
                    Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.NewYoyakuNinzu, z0003Result, oracleTransaction)
                    If status <> S02_0104.UpdateStatusSucess Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return status
                    End If
                Else

                    Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                    If zasekiKagenSu > Zero Then
                        ' 変更後の人数が多い場合、架空車種の座席数を変更

                        ' 席区分取得
                        Dim seatKbn As String = z0003Result.SeatKbn
                        ' コース台帳座席更新
                        updateCount = Me.updateKakuShashuZaseki(zasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, seatKbn, oracleTransaction)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiUpdateFailure
                        End If

                        ' 共用バス更新
                        Dim status As String = Me.updateSharedBusCrsZaseki(sharedZasekiData, registEntity.NewCrsLedgerBasicEntity, zasekiKagenSu, z0003Result, oracleTransaction)
                        If status <> S02_0104.UpdateStatusSucess Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return status
                        End If
                    End If
                End If
            End If

            ' 部屋残数更新
            If String.IsNullOrEmpty(registEntity.ZasekiInfoEntity.AibeyaFlag) = True Then

                ' 部屋残数更新SQL作成
                Dim query As String = Me.createCrsRoomAibeyaNashiUpdateSql(zasekiData, registEntity.NewCrsLedgerBasicEntity, registEntity.ZasekiInfoEntity, registEntity.YoyakuChangeKbn)
                If String.IsNullOrEmpty(query) = False Then

                    updateCount = MyBase.execNonQuery(oracleTransaction, query)
                    If updateCount <= Zero Then
                        ' 部屋残数の更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusRoomKakuhoUpdateFailure
                    End If
                End If
            Else
                ' 変更後が相部屋の場合

                Dim query As String = Me.createAibeyaRoomZansuSql(zasekiData, registEntity.ZasekiInfoEntity, registEntity.NewCrsLedgerBasicEntity)
                If String.IsNullOrEmpty(query) = False Then

                    updateCount = MyBase.execNonQuery(oracleTransaction, query)
                    If updateCount <= Zero Then
                        ' 部屋残数の更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusRoomKakuhoUpdateFailure
                    End If
                End If
            End If

            ' 変更履歴登録
            Dim historyRec As OutputYoyakuChangeHistoryParam = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction, registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoCrsChargeChargeKbnList)
            If historyRec Is Nothing Then

                Return CommonRegistYoyaku.UpdateStatusYoyakuChangeHistoryFailure
            End If

            ' 変更履歴最終日、変更履歴最終ＳＥＱ設定
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay
            registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq

            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName)
            registEntity.YoyakuInfoBasicPhysicsNameList.Add(registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName)

            ' 予約情報更新
            Dim updateStatus As String = Me.updateYoyakuInfo(registEntity, oracleTransaction)
            If updateStatus <> S02_0104.UpdateStatusSucess Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return updateStatus
            End If

            ' ピックアップルート台帳 （ホテル）
            If registEntity.PickupRouteLedgerHotelList IsNot Nothing Then

                For Each entity As PickupRouteLedgerHotelEntity In registEntity.PickupRouteLedgerHotelList

                    ' ピックアップルート台帳 （ホテル）人数更新SQL作成
                    Dim pickupHotelQuery As String = Me.createPickupRouteLedgerHotelUpdateSql(entity)
                    updateCount = MyBase.execNonQuery(oracleTransaction, pickupHotelQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure
                    End If
                Next
            End If

            ' 旧座席情報取得
            Dim oldCrsInfoEntity As New CrsLedgerBasicEntity()
            oldCrsInfoEntity.crsCd.Value = registEntity.ZasekiInfoEntity.OldCrsCd
            oldCrsInfoEntity.syuptDay.Value = registEntity.ZasekiInfoEntity.OldSyuptDay
            oldCrsInfoEntity.gousya.Value = registEntity.ZasekiInfoEntity.OldGousya

            Dim oldZasekiData As DataTable = Nothing
            Dim oldSharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(oldCrsInfoEntity, oracleTransaction, oldZasekiData, oldSharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusUsing
            End If

            ' 座席確定処理実施
            Dim zasekiKakuteiResult As Z0001_Result = Me.getCommonZasekiJidoData(z0003param, CInt(registEntity.YoyakuInfoBasicEntity.groupNo.Value))

            ' 前回確保していた座席数を加減
            If registEntity.YoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then
                '出発日、号車の変更の場合

                oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                If z0003Result.Status = Z0003_Result.Z0003_Result_Status.OK Then

                    If z0003Result.OldCarTypeCd <> KakuShashu Then
                        'If True Then

                        ' 座席確定DataTableをコピーし、旧座席数などを新側の変数に代入する
                        ' コピーする内容は以下
                        ' WK7YST(座席加減数・定席) = WK7YTQ(旧座席加減数・定席)
                        ' WK7YSH(座席加減数・補助席) = WK7YHQ(旧座席加減数・補助)
                        ' WK7KTS(空席数・定席) = WK7KTQ(旧空席数・定席)
                        ' WK7KHJ(空席数・補助席) = WK7KHQ(旧空席数・補助席)
                        ' WK7801(補助 801 調整数) = WK780Q(旧補助 801 調整数)
                        Dim copyZasekiKakuteiResult As New Z0003_Result()
                        copyZasekiKakuteiResult.ZasekiKagenTeiseki = z0003Result.OldZasekiKagenTeiseki
                        copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.OldZasekiKagenSub
                        copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.OldKusekiNumTeiseki
                        copyZasekiKakuteiResult.KusekiNumSub = z0003Result.OldKusekiNumSub
                        copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.OldSubCyoseiSeatNum

                        Dim zasekiKaihoUpdateQuery = Me.createCrsZasekiDataForKikaku(copyZasekiKakuteiResult, oldZasekiData, oldCrsInfoEntity)
                        If String.IsNullOrEmpty(zasekiKaihoUpdateQuery) = True Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        updateCount = MyBase.execNonQuery(oracleTransaction, zasekiKaihoUpdateQuery)
                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If

                        Dim sharedBusCrsQuery As String = ""
                        ' 共用コース座席更新
                        For Each row As DataRow In oldSharedZasekiData.Rows

                            If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                                Continue For
                            End If

                            ' 共用コース座席更新SQL作成
                            sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity)
                            updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                            If updateCount <= Zero Then
                                ' 座席データの更新件数が0件以下の場合、処理終了

                                ' ロールバック
                                MyBase.callRollbackTransaction(oracleTransaction)
                                Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                            End If
                        Next
                    Else
                        ' コース台帳座席更新(旧座席数解放)
                        updateCount = Me.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction)
                    End If
                Else
                    ' コース台帳座席更新(旧座席数解放)
                    updateCount = Me.updateKakuZasekiSu(oldZasekiData, oldCrsInfoEntity, (registEntity.OldYoyakuNinzu * -1), oracleTransaction)
                End If
            Else
                ' 予約人数の変更の場合

                Dim zasekiKagenSu As Integer = registEntity.NewYoyakuNinzu - registEntity.OldYoyakuNinzu
                If zasekiKagenSu < Zero Then
                    ' 変更後の人数が多い場合、解放処理なし

                    oldCrsInfoEntity.systemUpdateDay.Value = registEntity.ZasekiInfoEntity.ShoriDate
                    oldCrsInfoEntity.systemUpdatePersonCd.Value = registEntity.ZasekiInfoEntity.PersonCd
                    oldCrsInfoEntity.systemUpdatePgmid.Value = registEntity.ZasekiInfoEntity.Pgmid

                    Dim copyZasekiKakuteiResult As New Z0003_Result()
                    copyZasekiKakuteiResult.ZasekiKagenTeiseki = zasekiKagenSu
                    copyZasekiKakuteiResult.ZasekiKagenSub1F = z0003Result.ZasekiKagenSub1F
                    copyZasekiKakuteiResult.KusekiNumTeiseki = z0003Result.KusekiNumTeiseki
                    copyZasekiKakuteiResult.KusekiNumSub = z0003Result.KusekiNumSub
                    copyZasekiKakuteiResult.SubCyoseiSeatNum = z0003Result.SubCyoseiSeatNum

                    updateCount = Me.updateKakuShashuZaseki(zasekiData, oldCrsInfoEntity, zasekiKagenSu, z0003Result.SeatKbn, oracleTransaction)
                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                    End If

                    Dim sharedBusCrsQuery As String = ""
                    ' 共用コース座席更新
                    For Each row As DataRow In oldSharedZasekiData.Rows

                        If Me.isSharedBusCrsEqualCheck(row, oldCrsInfoEntity) = False Then
                            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                            Continue For
                        End If

                        ' 共用コース座席更新SQL作成
                        sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(copyZasekiKakuteiResult, row, oldCrsInfoEntity)
                        updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                        If updateCount <= Zero Then
                            ' 座席データの更新件数が0件以下の場合、処理終了

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return S02_0104.UpdateStatusZasekiKaihoUpdateFailure
                        End If
                    Next
                End If
            End If

            ' 部屋数解放SQL作成
            Dim roomiKaihoQuery As String = Me.createZasekiKaihoSql(oldZasekiData, registEntity, oldCrsInfoEntity)
            If String.IsNullOrEmpty(roomiKaihoQuery) = False Then

                updateCount = MyBase.execNonQuery(oracleTransaction, roomiKaihoQuery)

                If updateCount <= Zero Then
                    ' 部屋残数解放の更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return S02_0104.UpdateStatusRoomKaihoUpdateFailure
                End If
            End If

            ' コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw ex
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        ' 不要な行削除
        Me.deleteCrsChargeChargeKbnForShukuhakuAri(registEntity.YoyakuInfoCrsChargeChargeKbnList(0))

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約情報（ピックアップ）情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報（ピックアップ）情報</returns>
    Public Function getYoyakuInfoPickupList(entity As YoyakuInfoBasicEntity) As DataTable

        Dim pickupList As DataTable

        Try
            Dim query As String = Me.createYoyakuPickupInfoSql(entity)
            pickupList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return pickupList
    End Function

    ''' <summary>
    ''' 予約コース情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約コース情報取得SQL</returns>
    Private Function createYoyakuCrsInfoSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CLB.SYUPT_DAY ")
        sb.AppendLine("     ,CLB.CRS_CD ")
        sb.AppendLine("     ,CLB.CRS_NAME ")
        sb.AppendLine("     ,CLB.GOUSYA ")
        sb.AppendLine("     ,CLB.CRS_KIND ")
        sb.AppendLine("     ,CLB.CRS_KBN_1 ")
        sb.AppendLine("     ,CLB.CRS_KBN_2 ")
        sb.AppendLine("     ,CLB.TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     ,CLB.HOUJIN_GAIKYAKU_KBN ")
        sb.AppendLine("     ,CLB.SAIKOU_KAKUTEI_KBN ")
        sb.AppendLine("     ,CLB.UNKYU_KBN ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_1 AS JYOCHACHI_CD_1 ")
        sb.AppendLine("     ,PL1.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_1 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_2 AS JYOCHACHI_CD_2 ")
        sb.AppendLine("     ,PL2.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_2 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_3 AS JYOCHACHI_CD_3 ")
        sb.AppendLine("     ,PL3.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_3 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_4 AS JYOCHACHI_CD_4 ")
        sb.AppendLine("     ,PL4.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_4 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_5 AS JYOCHACHI_CD_5 ")
        sb.AppendLine("     ,PL5.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_5 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_1 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_2 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_3 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_4 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_5 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_1 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_2 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_3 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_4 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_5 ")
        sb.AppendLine("     ,CLB.JYOSEI_SENYO_SEAT_FLG ")
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ")
        sb.AppendLine("     ,CLB.ONE_SANKA_FLG ")
        sb.AppendLine("     ,CLB.JYOSYA_CAPACITY ")
        sb.AppendLine("     ,CLB.TEIINSEI_FLG ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_ONE_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_TWO_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_THREE_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_FOUR_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_FIVE_1R ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_ONE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_TWO_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_THREE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FOUR_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FIVE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_SOKEI ")
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ")
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ")
        sb.AppendLine("     ,CLB.USING_FLG ")
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_1 ")
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_2 ")
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_3 ")
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_4 ")
        sb.AppendLine("     ,YIB.ROOMING_BETU_NINZU_5 ")
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_1 ")
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_2 ")
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_3 ")
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_4 ")
        sb.AppendLine("     ,YIB.JYOSYA_NINZU_5 ")
        sb.AppendLine("     ,YIB.INFANT_NINZU ")
        sb.AppendLine("     ,YIB.JYOSEI_SENYO ")
        sb.AppendLine("     ,YIB.AIBEYA_FLG ")
        sb.AppendLine("     ,YIB.YOYAKU_ZASEKI_GET_KBN ")
        sb.AppendLine("     ,YIB.TOBI_SEAT_FLG ")
        sb.AppendLine("     ,YIB.YOYAKU_ZASEKI_KBN ")
        sb.AppendLine("     ,YIB.GROUP_NO ")
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ")
        sb.AppendLine("     ,YIB.HAKKEN_DAY ")
        sb.AppendLine("     ,YIB.HAKKEN_NAIYO ")
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ")
        sb.AppendLine("     ,YIB.SEISAN_HOHO ")
        sb.AppendLine("     ,YIB.AGENT_CD ")
        sb.AppendLine("     ,YIB.AGENT_NM ")
        sb.AppendLine("     ,YIB.AGENT_TEL_NO ")
        sb.AppendLine("     ,YIB.ADD_CHARGE_MAEBARAI_KEI ")
        sb.AppendLine("     ,YIB.WARIBIKI_ALL_GAKU ")
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_CANCEL ")
        sb.AppendLine("     ,YIB.YYKMKS ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CLB.CRS_CD = YIB.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.SYUPT_DAY = YIB.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.GOUSYA = YIB.GOUSYA ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL1 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL1.PLACE_CD = CLB.HAISYA_KEIYU_CD_1 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL2 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL2.PLACE_CD = CLB.HAISYA_KEIYU_CD_2 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL3 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL3.PLACE_CD = CLB.HAISYA_KEIYU_CD_3 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL4 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL4.PLACE_CD = CLB.HAISYA_KEIYU_CD_4 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL5 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL5.PLACE_CD = CLB.HAISYA_KEIYU_CD_5 ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIB.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIB.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約料金区分一覧取得SQL</returns>
    Private Function createYoyakuChargeKbnListSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CCK.KBN_NO AS KBN_NO_URA ")
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD_URA ")
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     ,CJK.SEX_BETU  ")
        sb.AppendLine("     ,CCK.CHARGE_KBN ")
        sb.AppendLine("     ,NVL(MCK.CHARGE_NAME, '-') AS KBN_NO ")
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ")
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ")
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ")
        sb.AppendLine("     ,CCK.CARRIAGE ")
        sb.AppendLine("     ,CCK.CHARGE_APPLICATION_NINZU_1 AS YOYAKU_NINZU ")
        sb.AppendLine("     ,CCK.TANKA_1 AS CHARGE ")
        sb.AppendLine("     ,CLC.CHARGE_SUB_SEAT ")
        sb.AppendLine("     ,CLC.CARRIAGE_SUB_SEAT ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine(" INNER JOIN ")
        sb.AppendLine("     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN CCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CCK.YOYAKU_KBN = YIB.YOYAKU_KBN ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CCK.YOYAKU_NO = YIB.YOYAKU_NO ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CLC  ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CLC.CRS_CD = YIB.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.SYUPT_DAY = YIB.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.GOUSYA = YIB.GOUSYA ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.KBN_NO = CCK.KBN_NO ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CHARGE_KBN MCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     MCK.CHARGE_KBN = CCK.CHARGE_KBN ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIB.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIB.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約料金区分一覧取得SQL</returns>
    Private Function createYoyakuChargeListSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT  ")
        sb.AppendLine("      CLC.CHARGE_KBN  ")
        sb.AppendLine("     ,CLC.KBN_NO  ")
        sb.AppendLine("     ,MCK.CHARGE_NAME  ")
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG  ")
        sb.AppendLine("     ,MCK.STAY_ADD_FLG  ")
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG  ")
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD  ")
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME  ")
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME_RK  ")
        sb.AppendLine("     ,CJK.SEX_BETU  ")
        sb.AppendLine("     ,CCK.CHARGE  ")
        sb.AppendLine("     ,CCK.CHARGE_SUB_SEAT  ")
        sb.AppendLine("     ,CCK.CARRIAGE  ")
        sb.AppendLine("     ,CCK.CARRIAGE_SUB_SEAT  ")
        sb.AppendLine("     ,CCK.CHARGE_1  ")
        sb.AppendLine("     ,CCK.CHARGE_2  ")
        sb.AppendLine("     ,CCK.CHARGE_3  ")
        sb.AppendLine("     ,CCK.CHARGE_4  ")
        sb.AppendLine("     ,CCK.CHARGE_5  ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_1 ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_2 ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_3 ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_4 ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU_5 ")
        sb.AppendLine("     ,YCK.CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,YCK.TANKA_1 ")
        sb.AppendLine("     ,YCK.TANKA_2 ")
        sb.AppendLine("     ,YCK.TANKA_3 ")
        sb.AppendLine("     ,YCK.TANKA_4 ")
        sb.AppendLine("     ,YCK.TANKA_5 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU_1 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU_2 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU_3 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU_4 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU_5 ")
        sb.AppendLine("     ,YCK.CANCEL_NINZU ")
        sb.AppendLine(" FROM  ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE CLC  ")
        sb.AppendLine(" LEFT OUTER JOIN  ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK  ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     CCK.CRS_CD = CLC.CRS_CD  ")
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CCK.SYUPT_DAY = CLC.SYUPT_DAY  ")
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CCK.GOUSYA = CLC.GOUSYA  ")
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CCK.KBN_NO = CLC.KBN_NO  ")
        sb.AppendLine(" LEFT OUTER JOIN  ")
        sb.AppendLine("     M_CHARGE_KBN MCK  ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     MCK.CHARGE_KBN = CLC.CHARGE_KBN  ")
        sb.AppendLine(" LEFT OUTER JOIN  ")
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK  ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     (SELECT ")
        sb.AppendLine("          KBN_NO ")
        sb.AppendLine("         ,CHARGE_KBN_JININ_CD ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_1 ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_2 ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_3 ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_4 ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU_5 ")
        sb.AppendLine("         ,CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("         ,TANKA_1 ")
        sb.AppendLine("         ,TANKA_2 ")
        sb.AppendLine("         ,TANKA_3 ")
        sb.AppendLine("         ,TANKA_4 ")
        sb.AppendLine("         ,TANKA_5 ")
        sb.AppendLine("         ,CANCEL_NINZU_1 ")
        sb.AppendLine("         ,CANCEL_NINZU_2 ")
        sb.AppendLine("         ,CANCEL_NINZU_3 ")
        sb.AppendLine("         ,CANCEL_NINZU_4 ")
        sb.AppendLine("         ,CANCEL_NINZU_5 ")
        sb.AppendLine("         ,CANCEL_NINZU ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("         T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine("     LEFT OUTER JOIN ")
        sb.AppendLine("         T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN CCK ")
        sb.AppendLine("     ON ")
        sb.AppendLine("         YIB.YOYAKU_KBN = CCK.YOYAKU_KBN ")
        sb.AppendLine("         AND ")
        sb.AppendLine("         YIB.YOYAKU_NO =CCK.YOYAKU_NO ")
        sb.AppendLine("         WHERE ")
        sb.AppendLine("         YIB.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("         AND ")
        sb.AppendLine("         YIB.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("     ) YCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YCK.KBN_NO = CLC.KBN_NO  ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     YCK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" WHERE  ")
        sb.AppendLine("     CLC.CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CLC.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CLC.GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" ORDER BY CLC.KBN_NO, CCK.CHARGE_KBN_JININ_CD  ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース情報取得SQL</returns>
    Private Function createCrsInfoSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CLB.SYUPT_DAY ")
        sb.AppendLine("     ,CLB.CRS_CD ")
        sb.AppendLine("     ,CLB.CRS_NAME ")
        sb.AppendLine("     ,CLB.GOUSYA ")
        sb.AppendLine("     ,CLB.CRS_KIND ")
        sb.AppendLine("     ,CLB.CRS_KBN_1 ")
        sb.AppendLine("     ,CLB.CRS_KBN_2 ")
        sb.AppendLine("     ,CLB.TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     ,CLB.HOUJIN_GAIKYAKU_KBN ")
        sb.AppendLine("     ,CLB.SAIKOU_KAKUTEI_KBN ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_1 AS JYOCHACHI_CD_1 ")
        sb.AppendLine("     ,PL1.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_1 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_2 AS JYOCHACHI_CD_2 ")
        sb.AppendLine("     ,PL2.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_2 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_3 AS JYOCHACHI_CD_3 ")
        sb.AppendLine("     ,PL3.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_3 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_4 AS JYOCHACHI_CD_4 ")
        sb.AppendLine("     ,PL4.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_4 ")
        sb.AppendLine("     ,CLB.HAISYA_KEIYU_CD_5 AS JYOCHACHI_CD_5 ")
        sb.AppendLine("     ,PL5.PLACE_NAME_SHORT AS PLACE_NAME_SHORT_5 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_1 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_2 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_3 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_4 ")
        sb.AppendLine("     ,CLB.SYUGO_TIME_5 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_1 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_2 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_3 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_4 ")
        sb.AppendLine("     ,CLB.SYUPT_TIME_5 ")
        sb.AppendLine("     ,CLB.JYOSEI_SENYO_SEAT_FLG ")
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ")
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ")
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.YOYAKU_NUM_SUB_SEAT ")
        sb.AppendLine("     ,CLB.HURIKOMI_NG_FLG ")
        sb.AppendLine("     ,CLB.ONE_SANKA_FLG ")
        sb.AppendLine("     ,CLB.JYOSYA_CAPACITY ")
        sb.AppendLine("     ,CLB.TEIINSEI_FLG ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_ONE_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_TWO_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_THREE_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_FOUR_1R ")
        sb.AppendLine("     ,CLB.CRS_BLOCK_FIVE_1R ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_ONE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_TWO_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_THREE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FOUR_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_FIVE_ROOM ")
        sb.AppendLine("     ,CLB.ROOM_ZANSU_SOKEI ")
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ")
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ")
        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL1 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL1.PLACE_CD = CLB.HAISYA_KEIYU_CD_1 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL2 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL2.PLACE_CD = CLB.HAISYA_KEIYU_CD_2 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL3 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL3.PLACE_CD = CLB.HAISYA_KEIYU_CD_3 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL4 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL4.PLACE_CD = CLB.HAISYA_KEIYU_CD_4 ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PLACE PL5 ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     PL5.PLACE_CD = CLB.HAISYA_KEIYU_CD_5 ")
        sb.AppendLine(" WHERE  ")
        sb.AppendLine("     CLB.CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND  ")
        sb.AppendLine("     CLB.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))

        If entity.gousya.Value IsNot Nothing AndAlso entity.gousya.Value > 0 Then

            sb.AppendLine("     AND  ")
            sb.AppendLine("     CLB.GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        End If

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity
    ''' </param>
    ''' <returns>料金区分一覧取得SQL</returns>
    Private Function createChargeKbnListSql(entity As CrsLedgerChargeEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CLC.CHARGE_KBN ")
        sb.AppendLine("     ,CLC.KBN_NO ")
        sb.AppendLine("     ,NVL(MCK.CHARGE_NAME, '-') AS CHARGE_NAME ")
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ")
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ")
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ")
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME ")
        sb.AppendLine("     ,CJK.CHARGE_KBN_JININ_NAME_RK ")
        sb.AppendLine("     ,CJK.SEX_BETU ")
        sb.AppendLine("     ,CCK.CHARGE ")
        sb.AppendLine("     ,CCK.CHARGE_SUB_SEAT ")
        sb.AppendLine("     ,CCK.CARRIAGE ")
        sb.AppendLine("     ,CCK.CARRIAGE_SUB_SEAT ")
        sb.AppendLine("     ,CCK.CHARGE_1 ")
        sb.AppendLine("     ,CCK.CHARGE_2 ")
        sb.AppendLine("     ,CCK.CHARGE_3 ")
        sb.AppendLine("     ,CCK.CHARGE_4 ")
        sb.AppendLine("     ,CCK.CHARGE_5 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_1 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_2 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_3 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_4 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU_5 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_1 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_2 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_3 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_4 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(7, 0)) AS TANKA_5 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_1 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_2 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_3 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_4 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU_5 ")
        sb.AppendLine("     ,CAST(NULL AS NUMBER(3, 0)) AS CANCEL_NINZU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE CLC ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CCK.CRS_CD = CLC.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CCK.SYUPT_DAY = CLC.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CCK.GOUSYA = CLC.GOUSYA ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CCK.KBN_NO = CLC.KBN_NO ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CHARGE_KBN MCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     MCK.CHARGE_KBN = CLC.CHARGE_KBN ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CHARGE_JININ_KBN CJK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CJK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CLC.CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" ORDER BY ")
        sb.AppendLine("     CLC.KBN_NO, CCK.CHARGE_KBN_JININ_CD ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳空席数取得SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳空席数取得SQL</returns>
    Private Function createCrsLedgerBasicKusekiNumDataSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ")
        sb.AppendLine("     ,YOYAKU_NUM_TEISEKI ")
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ")
        sb.AppendLine("     ,CRS_BLOCK_CAPACITY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳座席情報取得
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="oracleTransaction">トランザクション</param>
    ''' <returns>コース台帳座席情報</returns>
    Private Function getCrsLenderZasekiDataForTeiki(entity As CrsLedgerBasicEntity, oracleTransaction As OracleTransaction) As DataTable

        Dim zasekiData As DataTable = Nothing
        Dim idx As Integer = 1

        Dim query As String = Me.createCrsLedgerZasekiSql(entity)

        While idx <= UsingCheckRetryNum

            zasekiData = MyBase.getDataTable(oracleTransaction, query)

            If zasekiData.Rows.Count > Zero Then

                Exit While
            End If

            ' 座席データがない場合、リトライ
            idx = idx + 1
        End While

        Return zasekiData
    End Function

    ''' <summary>
    ''' コース使用中チェック
    ''' 企画用
    ''' </summary>
    ''' <param name="entity"></param>
    ''' <param name="oracleTransaction">トランザクション</param>
    ''' <param name="zasekiData">自コース座席情報</param>
    ''' <param name="sharedZasekiData">共用コース座席情報</param>
    ''' <returns>検証結果</returns>
    Private Function isCrsLenderZasekiDataForKikaku(entity As CrsLedgerBasicEntity, oracleTransaction As OracleTransaction, ByRef zasekiData As DataTable, ByRef sharedZasekiData As DataTable) As Boolean

        Dim isValid As Boolean = False
        Dim idx As Integer = 1
        Dim busReserveCd As String = ""
        Dim sharedCrsQuery As String = ""

        While idx <= UsingCheckRetryNum

            '　自コースの座席情報取得SQL作成
            Dim crsQuery As String = Me.createCrsLedgerZasekiSql(entity)

            ' 自コースの使用中チェック
            zasekiData = MyBase.getDataTable(oracleTransaction, crsQuery)
            If zasekiData.Rows.Count <= 0 Then

                ' 自コースが使用中の場合、リトライ
                idx = idx + 1
                Continue While
            End If

            ' 共用コースの使用中チェック
            ' バス指定コード取得
            busReserveCd = zasekiData.Rows(0)("BUS_RESERVE_CD").ToString()
            ' 共用コースの座席情報取得SQL作成
            sharedCrsQuery = Me.createSharedBusCrsZasekiSql(entity, busReserveCd)
            ' 共用コース取得
            sharedZasekiData = MyBase.getDataTable(oracleTransaction, sharedCrsQuery)

            If sharedZasekiData.Rows.Count <= 0 Then
                ' 共用コースがない場合、処理終了

                isValid = True
                Exit While
            End If

            For Each sharedRow As DataRow In sharedZasekiData.Rows

                If sharedRow("USING_FLG") IsNot Nothing AndAlso String.IsNullOrEmpty(sharedRow("USING_FLG").ToString()) = False Then
                    ' 共用コースが使用中の場合、リトライ
                    idx = idx + 1
                    Continue While
                End If
            Next

            isValid = True
            Exit While
        End While

        Return isValid
    End Function

    ''' <summary>
    ''' コース台帳座席情報取得SQL作成
    ''' 悲観ロック用
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席情報取得SQL</returns>
    Private Function createCrsLedgerZasekiSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YOYAKU_NUM_TEISEKI ")
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ")
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ")
        sb.AppendLine("     ,JYOSYA_CAPACITY ")
        sb.AppendLine("     ,CAPACITY_REGULAR ")
        sb.AppendLine("     ,CAPACITY_HO_1KAI ")
        sb.AppendLine("     ,EI_BLOCK_REGULAR ")
        sb.AppendLine("     ,EI_BLOCK_HO ")
        sb.AppendLine("     ,BLOCK_KAKUHO_NUM ")
        sb.AppendLine("     ,KUSEKI_KAKUHO_NUM ")
        sb.AppendLine("     ,BUS_RESERVE_CD ")
        sb.AppendLine("     ,TEIINSEI_FLG ")
        sb.AppendLine("     ,CRS_BLOCK_ONE_1R ")
        sb.AppendLine("     ,CRS_BLOCK_TWO_1R ")
        sb.AppendLine("     ,CRS_BLOCK_THREE_1R ")
        sb.AppendLine("     ,CRS_BLOCK_FOUR_1R ")
        sb.AppendLine("     ,CRS_BLOCK_FIVE_1R ")
        sb.AppendLine("     ,CRS_BLOCK_ROOM_NUM ")
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM ")
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM ")
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM ")
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM ")
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM ")
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI ")
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI ")
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_MALE ")
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM ")
        sb.AppendLine("     ,USING_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     USING_FLG IS NULL ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" FOR UPDATE WAIT 10 ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 共用バスコース座席情報取得SQL作成
    ''' 悲観ロック用
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="busReserveCd">バス指定コード</param>
    ''' <returns>共用バスコース座席情報取得SQL</returns>
    Private Function createSharedBusCrsZasekiSql(entity As CrsLedgerBasicEntity, busReserveCd As String) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CRS_CD ")
        sb.AppendLine("     ,SYUPT_DAY ")
        sb.AppendLine("     ,GOUSYA ")
        sb.AppendLine("     ,YOYAKU_NUM_TEISEKI ")
        sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT ")
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT ")
        sb.AppendLine("     ,JYOSYA_CAPACITY ")
        sb.AppendLine("     ,CAPACITY_REGULAR ")
        sb.AppendLine("     ,CAPACITY_HO_1KAI ")
        sb.AppendLine("     ,EI_BLOCK_REGULAR ")
        sb.AppendLine("     ,EI_BLOCK_HO ")
        sb.AppendLine("     ,BLOCK_KAKUHO_NUM ")
        sb.AppendLine("     ,KUSEKI_KAKUHO_NUM ")
        sb.AppendLine("     ,BUS_RESERVE_CD ")
        sb.AppendLine("     ,USING_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     BUS_RESERVE_CD = " + MyBase.setParam("BUS_RESERVE_CD", busReserveCd, entity.busReserveCd.DBType, entity.busReserveCd.IntegerBu, entity.busReserveCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" FOR UPDATE WAIT 10 ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 架空車種座席更新
    ''' </summary>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="zasekiKagenSu">座席数</param>
    ''' <param name="seatKbn">席区分</param>
    ''' <param name="oracleTransaction">Oracleトランザクション</param>
    ''' <returns>更新件数</returns>
    Private Function updateKakuShashuZaseki(crsLedgerZasekiData As DataTable, entity As CrsLedgerBasicEntity, zasekiKagenSu As Integer, seatKbn As String, oracleTransaction As OracleTransaction) As Integer

        Dim updateEntity As New CrsLedgerBasicEntity()
        updateEntity.crsCd.Value = entity.crsCd.Value
        updateEntity.syuptDay.Value = entity.syuptDay.Value
        updateEntity.gousya.Value = entity.gousya.Value
        updateEntity.systemUpdateDay.Value = entity.systemUpdateDay.Value
        updateEntity.systemUpdatePersonCd.Value = entity.systemUpdatePersonCd.Value
        updateEntity.systemUpdatePgmid.Value = entity.systemUpdatePgmid.Value

        If String.IsNullOrEmpty(seatKbn) = True Then

            Dim yoyakuNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString())
            Dim kusekiNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString())

            updateEntity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu
            updateEntity.kusekiNumTeiseki.Value = kusekiNumTeiseki - zasekiKagenSu
        Else

            Dim yoyakuNumSubSeat As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_SUB_SEAT").ToString())
            Dim kusekiNumSubSeat As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT").ToString())

            updateEntity.yoyakuNumSubSeat.Value = yoyakuNumSubSeat + zasekiKagenSu
            updateEntity.kusekiNumSubSeat.Value = kusekiNumSubSeat - zasekiKagenSu
        End If

        ' 架空車種座席更新SQL作成
        Dim query As String = Me.createKakuShashuZasekiSql(updateEntity, seatKbn)
        Dim updateCount As Integer = MyBase.execNonQuery(oracleTransaction, query)

        Return updateCount
    End Function

    ''' <summary>
    ''' 架空車種座席更新SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="seatKbn">席区分</param>
    ''' <returns>架空車種座席更新SQL</returns>
    Private Function createKakuShashuZasekiSql(entity As CrsLedgerBasicEntity, seatKbn As String) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")

        If String.IsNullOrEmpty(seatKbn) = True Then

            sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))
            sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        Else

            sb.AppendLine("      YOYAKU_NUM_SUB_SEAT = " + MyBase.setParam(entity.yoyakuNumSubSeat.PhysicsName, entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu))
            sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        End If

        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 共用バス座席更新
    ''' </summary>
    ''' <param name="sharedZasekiData">共用バス座席情報</param>
    ''' <param name="newCrsEntity">コース台帳（基本）Entity</param>
    ''' <param name="z0003Result">共通処理「バス座席自動設定」</param>
    ''' <param name="oracleTransaction">Oracleトランザクション</param>
    ''' <returns>更新ステータス</returns>
    Private Function updateSharedBusCrsZaseki(sharedZasekiData As DataTable, newCrsEntity As CrsLedgerBasicEntity, z0003Result As Z0003_Result, oracleTransaction As OracleTransaction) As String

        For Each sharedRow As DataRow In sharedZasekiData.Rows

            If Me.isSharedBusCrsEqualCheck(sharedRow, newCrsEntity) = False Then
                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                Continue For
            End If

            ' 空席数ワーク
            Dim kusekiWork As Integer = 0

            ' 空席数定席
            Dim kusekiSuTeiseki As Integer = Integer.Parse(sharedRow("KUSEKI_NUM_TEISEKI").ToString())
            ' 空席数ワーク = コース台帳（基本）.共通処理「NRV710R バス座席自動設定（予約変更）」.座席加減数・定席
            kusekiWork = kusekiSuTeiseki - z0003Result.ZasekiKagenTeiseki

            If kusekiWork >= z0003Result.KusekiNumTeiseki Then
                ' 空席数ワークが 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・定席

                kusekiSuTeiseki = z0003Result.KusekiNumTeiseki
            Else

                kusekiSuTeiseki = kusekiWork
            End If

            ' 空席数補助席
            Dim kusekiSuSubSeat As Integer = Integer.Parse(sharedRow("KUSEKI_NUM_SUB_SEAT").ToString())
            ' 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席
            kusekiWork = kusekiSuSubSeat - z0003Result.KusekiNumSub
            ' 空席数ワーク = 空席数ワーク - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
            kusekiWork = kusekiWork - z0003Result.SubCyoseiSeatNum

            If kusekiWork >= z0003Result.KusekiNumSub Then
                ' 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席

                kusekiSuSubSeat = z0003Result.KusekiNumSub
            Else

                kusekiSuSubSeat = kusekiWork
            End If

            ' 空席数定席、空席数補助席が0を下回る場合、0にする
            If kusekiSuTeiseki < CommonRegistYoyaku.ZERO Then

                kusekiSuTeiseki = CommonRegistYoyaku.ZERO
            End If

            If kusekiSuSubSeat < CommonRegistYoyaku.ZERO Then

                kusekiSuSubSeat = CommonRegistYoyaku.ZERO
            End If

            ' WHERE条件設定
            Dim sharedEntity As New CrsLedgerBasicEntity()
            sharedEntity.crsCd.Value = sharedRow("CRS_CD").ToString()
            sharedEntity.syuptDay.Value = Integer.Parse(sharedRow("SYUPT_DAY").ToString())
            sharedEntity.gousya.Value = Integer.Parse(sharedRow("GOUSYA").ToString())
            sharedEntity.systemUpdateDay.Value = newCrsEntity.systemUpdateDay.Value
            sharedEntity.systemUpdatePersonCd.Value = newCrsEntity.systemUpdatePersonCd.Value
            sharedEntity.systemUpdatePgmid.Value = newCrsEntity.systemUpdatePgmid.Value

            Dim query As String = Me.createSharedCrsZasekiUpdateSql(kusekiSuTeiseki, kusekiSuSubSeat, sharedEntity)
            Dim updateCount = MyBase.execNonQuery(oracleTransaction, query)

            If updateCount <= Zero Then
                ' 座席データの更新件数が0件以下の場合、処理終了

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusZasekiUpdateFailure
            End If
        Next

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 共用バス座席更新
    ''' </summary>
    ''' <param name="sharedZasekiData">共用バス座席情報</param>
    ''' <param name="newCrsEntity">コース台帳（基本）Entity</param>
    ''' <param name="zasekiKagenSu">座席加減数</param>
    ''' <param name="z0003Result">共通処理「バス座席自動設定」</param>
    ''' <param name="oracleTransaction">Oracleトランザクション</param>
    ''' <returns>更新ステータス</returns>
    Private Function updateSharedBusCrsZaseki(sharedZasekiData As DataTable, newCrsEntity As CrsLedgerBasicEntity, zasekiKagenSu As Integer, z0003Result As Z0003_Result, oracleTransaction As OracleTransaction) As String

        For Each sharedRow As DataRow In sharedZasekiData.Rows

            If Me.isSharedBusCrsEqualCheck(sharedRow, newCrsEntity) = False Then
                ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                Continue For
            End If

            ' 空席数ワーク
            Dim kusekiWork As Integer = 0

            ' 空席数定席
            Dim kusekiSuTeiseki As Integer = Integer.Parse(sharedRow("KUSEKI_NUM_TEISEKI").ToString())
            ' 空席数ワーク = コース台帳（基本）.空席数定席 - 座席加減数(新予約人数 - 旧予約人数)
            kusekiWork = kusekiSuTeiseki - zasekiKagenSu

            If kusekiWork >= z0003Result.KusekiNumTeiseki Then
                ' 空席数ワークが 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・定席

                kusekiSuTeiseki = z0003Result.KusekiNumTeiseki
            Else

                kusekiSuTeiseki = kusekiWork
            End If

            ' 空席数補助席
            Dim kusekiSuSubSeat As Integer = Integer.Parse(sharedRow("KUSEKI_NUM_SUB_SEAT").ToString())
            ' 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席
            kusekiWork = kusekiSuSubSeat - z0003Result.KusekiNumSub
            ' 空席数ワーク = 空席数ワーク - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
            kusekiWork = kusekiWork - z0003Result.SubCyoseiSeatNum

            If kusekiWork >= z0003Result.KusekiNumSub Then
                ' 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数補助席

                kusekiSuSubSeat = z0003Result.KusekiNumSub
            Else

                kusekiSuSubSeat = kusekiWork
            End If

            ' 空席数定席、空席数補助席が0を下回る場合、0にする
            If kusekiSuTeiseki < CommonRegistYoyaku.ZERO Then

                kusekiSuTeiseki = CommonRegistYoyaku.ZERO
            End If

            If kusekiSuSubSeat < CommonRegistYoyaku.ZERO Then

                kusekiSuSubSeat = CommonRegistYoyaku.ZERO
            End If

            ' WHERE条件設定
            Dim sharedEntity As New CrsLedgerBasicEntity()
            sharedEntity.crsCd.Value = sharedRow("CRS_CD").ToString()
            sharedEntity.syuptDay.Value = Integer.Parse(sharedRow("SYUPT_DAY").ToString())
            sharedEntity.gousya.Value = Integer.Parse(sharedRow("GOUSYA").ToString())
            sharedEntity.systemUpdateDay.Value = newCrsEntity.systemUpdateDay.Value
            sharedEntity.systemUpdatePersonCd.Value = newCrsEntity.systemUpdatePersonCd.Value
            sharedEntity.systemUpdatePgmid.Value = newCrsEntity.systemUpdatePgmid.Value

            Dim query As String = Me.createSharedCrsZasekiUpdateSql(kusekiSuTeiseki, kusekiSuSubSeat, sharedEntity)
            Dim updateCount = MyBase.execNonQuery(oracleTransaction, query)

            If updateCount <= CommonRegistYoyaku.ZERO Then
                ' 座席データの更新件数が0件以下の場合、処理終了

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusZasekiUpdateFailure
            End If
        Next

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳座席更新
    ''' </summary>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">座席更新の検索条件</param>
    ''' <param name="zasekiKagenSu">座席加減数</param>
    ''' <param name="oracleTransaction">オラクルトランザクション</param>
    ''' <param name="kusekiTeiseki">空席数定席</param>
    ''' <returns>更新件数</returns>
    Private Function updateKakuZasekiSu(crsLedgerZasekiData As DataTable, entity As CrsLedgerBasicEntity, zasekiKagenSu As Integer, oracleTransaction As OracleTransaction, Optional ByRef kusekiTeiseki As Integer = 0) As Integer

        ' コース台帳座席更新SQL作成
        Dim query = Me.createCrsKakuShashuZasekiData(crsLedgerZasekiData, entity, zasekiKagenSu, kusekiTeiseki)

        Dim updateCount As Integer = 0

        If String.IsNullOrEmpty(query) = True Then

            Return updateCount
        End If

        updateCount = MyBase.execNonQuery(oracleTransaction, query)

        Return updateCount
    End Function

    ''' <summary>
    ''' コース台帳座席更新SQL作成
    ''' </summary>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">座席更新の検索条件</param>
    ''' <param name="zasekiKagenSu">座席加減数</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsKakuShashuZasekiData(crsLedgerZasekiData As DataTable, entity As CrsLedgerBasicEntity, zasekiKagenSu As Integer, ByRef kusekiTeiseki As Integer) As String

        Dim yoyakuNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString())
        Dim kusekiNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString())

        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu
        kusekiTeiseki = kusekiNumTeiseki - zasekiKagenSu
        entity.kusekiNumTeiseki.Value = kusekiTeiseki

        If entity.kusekiNumTeiseki.Value < Zero Then
            ' 空席数・定席が0を下回る場合
            Return ""
        End If

        ' コース台帳架空車種座席更新SQL作成
        Dim query As String = Me.createCrsKakuShashuZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' コース台帳座席更新SQL作成
    ''' </summary>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">座席更新の検索条件</param>
    ''' <param name="zasekiKagenSu">座席加減数</param>
    ''' <param name="z0003Result">座席確定データ</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsKakuShashuZasekiData(crsLedgerZasekiData As DataTable, ByRef entity As CrsLedgerBasicEntity, zasekiKagenSu As Integer, z0003Result As Z0003_Result) As String

        Dim yoyakuNumTeiseki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim kusekiNumTeiseki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))

        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + zasekiKagenSu
        entity.kusekiNumTeiseki.Value = kusekiNumTeiseki - zasekiKagenSu

        Dim kusekiNumSubSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"))

        ' 空席数ワーク = コース台帳（基本）.空席数補助席 - 共通処理「NRV710R バス座席自動設定（予約変更）」.補助801調整数
        Dim kusekiWork As Integer = kusekiNumSubSeat - z0003Result.OldSubCyoseiSeatNum

        If kusekiWork >= z0003Result.OldKusekiNumSub Then
            ' 空席数ワークが共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・補助席以上の場合
            ' 共通処理「NRV710R バス座席自動設定（予約変更）」.空席数・補助席を設定
            entity.kusekiNumSubSeat.Value = z0003Result.OldKusekiNumSub
        Else
            ' 上記以外、空席数ワークを設定
            entity.kusekiNumSubSeat.Value = kusekiWork
        End If

        Dim query As String = Me.createCrsKakuZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' コース台帳架空車種座席更新SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsKakuShashuZasekiUpdateSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳架空車種座席更新SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsKakuZasekiUpdateSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳座席情報更新SQL作成
    ''' 定期用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="kusekiTeisaki">空席数定席</param>
    ''' <returns>コース台帳座席情報更新SQL</returns>
    Private Function createCrsZasekiDataForTeiki(z0003Result As Z0003_Result, crsLedgerZasekiData As DataTable, entity As CrsLedgerBasicEntity, ByRef kusekiTeisaki As Integer) As String

        Dim crsYoyakuSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim crsKusekiSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))

        ' 予約数定席
        ' コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim yoyakuSuTeisaki As Integer = crsYoyakuSuTeisaki + z0003Result.ZasekiKagenTeiseki

        ' 空席数定席算出
        ' コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        kusekiTeisaki = Me.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.ZasekiKagenTeiseki, z0003Result.KusekiNumTeiseki)

        ' 空席数定席マイナスチェック
        If kusekiTeisaki < CommonRegistYoyaku.ZERO Then
            ' 空席数定席が'0'を下回る場合、エラーとして更新処理終了
            Return ""
        End If

        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki
        entity.kusekiNumTeiseki.Value = kusekiTeisaki

        ' コース台帳座席更新SQL作成
        Dim query As String = Me.createCrsZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' 共用バス座席情報更新SQL作成
    ''' </summary>
    ''' <param name="sharedRow">共用座席情報</param>
    ''' <param name="ledgerBasicEntity">コース台帳（基本）</param>
    ''' <param name="kusekiTeisaki">空席数定席</param>
    ''' <returns>共用バス座席情報更新SQL</returns>
    Private Function createSharedCrsZasekiDataUpdateSqlForTeiki(sharedRow As DataRow, ledgerBasicEntity As CrsLedgerBasicEntity, kusekiTeisaki As Integer) As String

        Dim entity As New CrsLedgerBasicEntity()
        entity.kusekiNumTeiseki.Value = kusekiTeisaki
        entity.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"))
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"))
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"))

        entity.systemUpdatePgmid.Value = ledgerBasicEntity.systemUpdatePgmid.Value
        entity.systemUpdatePersonCd.Value = ledgerBasicEntity.systemUpdatePersonCd.Value
        entity.systemUpdateDay.Value = ledgerBasicEntity.systemUpdateDay.Value

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 共用バス座席情報更新SQL作成
    ''' </summary>
    ''' <param name="sharedRow"></param>
    ''' <param name="ledgerBasicEntity"></param>
    ''' <returns></returns>
    Private Function createShearedZasekiKaihoDateForTeiki(sharedRow As DataRow, ledgerBasicEntity As CrsLedgerBasicEntity) As String

        Dim entity As New CrsLedgerBasicEntity()
        entity.kusekiNumTeiseki.Value = ledgerBasicEntity.kusekiNumTeiseki.Value
        entity.kusekiNumSubSeat.Value = ledgerBasicEntity.kusekiNumSubSeat.Value
        entity.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"))
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"))
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"))
        entity.systemUpdatePgmid.Value = ledgerBasicEntity.systemUpdatePgmid.Value
        entity.systemUpdatePersonCd.Value = ledgerBasicEntity.systemUpdatePersonCd.Value
        entity.systemUpdateDay.Value = ledgerBasicEntity.systemUpdateDay.Value

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳座席情報更新SQL作成
    ''' 定期用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    ''' <param name="oldCrsLedgerZasekiData">旧コース台帳座席情報</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席情報更新SQL</returns>
    Private Function createCrsZasekiKaihoDateForTeiki(z0003Result As Z0003_Result, oldCrsLedgerZasekiData As DataTable, ByRef entity As CrsLedgerBasicEntity) As String

        Dim crsYoyakuSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim crsKusekiSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))
        Dim crsKusekiSuSuSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(oldCrsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"))

        ' 予約数定席
        ' コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.旧座席加減数・定席
        Dim yoyakuSuTeisaki As Integer = crsYoyakuSuTeisaki + z0003Result.OldZasekiKagenTeiseki
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki

        ' 空席数定席算出
        ' コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.旧座席加減数・定席, 共通処理「バス座席自動設定処理」.旧空席数・定席
        Dim kusekiSuTeisaki As Integer = Me.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.OldZasekiKagenTeiseki, z0003Result.OldKusekiNumTeiseki)
        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki

        ' 空席数補助席算出
        '空席数ワーク = コース台帳(基本).空席数補助席 - 共通処理「バス座席自動設定処理」.旧補助 801 調整数
        Dim kusekiWork As Integer = crsKusekiSuSuSeat - z0003Result.OldSubCyoseiSeatNum
        If kusekiWork >= z0003Result.OldKusekiNumSub Then
            ' 空席数ワークが共通処理「バス座席自動設定処理」.旧空席数・補助席以上の場合、
            ' 共通処理「バス座席自動設定処理」.旧空席数・補助席を設定
            entity.kusekiNumSubSeat.Value = z0003Result.OldKusekiNumSub
        Else
            ' 上記以外は、空席数ワークを設定
            entity.kusekiNumSubSeat.Value = kusekiWork
        End If

        If entity.kusekiNumSubSeat.Value < CommonRegistYoyaku.ZERO Then
            ' 空席数補助席が0を下回る場合、強制的に0にする
            entity.kusekiNumSubSeat.Value = CommonRegistYoyaku.ZERO
        End If

        Dim query As String = Me.createCrsZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' 空席数定席算出
    ''' </summary>
    ''' <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    ''' <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・定席</param>
    ''' <param name="comKusekiSu">共通処理「バス座席自動設定処理」.空席数・定席</param>
    ''' <returns>空席数定席</returns>
    Private Function calcKuusekiSu(crsKusekiSu As Integer, comZasekiKagenSu As Integer, comKusekiSu As Integer) As Integer

        ' コース台帳(基本).空席数定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim kusekiSu As Integer = crsKusekiSu - comZasekiKagenSu

        If kusekiSu >= comKusekiSu Then
            ' 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席」が共通処理「バス座席自動設定処理」.空席数・定席以上の場合
            ' 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席とする
            kusekiSu = comKusekiSu
        End If

        Return kusekiSu
    End Function

    ''' <summary>
    ''' コース台帳座席更新SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsZasekiUpdateSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam(entity.yoyakuNumTeiseki.PhysicsName, entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))

        If entity.yoyakuNumSubSeat.Value IsNot Nothing Then

            sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT = " + MyBase.setParam(entity.yoyakuNumSubSeat.PhysicsName, entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu))
        End If

        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))

        If entity.kusekiNumSubSeat.Value IsNot Nothing Then

            sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        End If

        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' INSERT SQL作成
    ''' </summary>
    ''' <param name="entity">登録するテーブルEntity</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>INSERT SQL</returns>
    Private Function createInsertSql(Of T)(entity As T, tableName As String) As String

        MyBase.paramClear()

        Dim type As Type = GetType(T)
        Dim properties() As PropertyInfo = type.GetProperties()


        Dim insertSql As New StringBuilder()
        Dim valueSql As New StringBuilder()

        Dim idx As Integer = 0
        Dim comma As String = ""
        Dim physicsName As String = ""

        For Each prop As PropertyInfo In properties

            If idx = 0 Then

                comma = ""
            End If

            If prop.PropertyType Is GetType(EntityKoumoku_YmdType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).PhysicsName

                Dim value As Date? = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).Value

                If value IsNot Nothing Then

                    insertSql.AppendLine(comma + physicsName)
                    valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).Value,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).DBType))
                End If

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_NumberType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).PhysicsName
                Dim value As Integer? = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value

                If value IsNot Nothing Then

                    insertSql.AppendLine(comma + physicsName)
                    valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DBType,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).IntegerBu,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DecimalBu))
                End If

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_Number_DecimalType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).PhysicsName

                insertSql.AppendLine(comma + physicsName)
                valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).Value,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DBType,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DecimalBu))

            Else

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).PhysicsName
                Dim value As String = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value

                insertSql.AppendLine(comma + physicsName)
                valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).DBType,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).IntegerBu))
            End If

            comma = ","
            idx = idx + 1
        Next

        ' INSERT文作成
        Dim sb As New StringBuilder()
        sb.AppendLine(String.Format(" INSERT INTO {0} ", tableName))
        sb.AppendLine(" ( ")
        sb.AppendLine(insertSql.ToString())
        sb.AppendLine(") VALUES ( ")
        sb.AppendLine(valueSql.ToString())
        sb.AppendLine(" ) ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' UPDATE SQL作成
    ''' </summary>
    ''' <typeparam name="T">テーブルEntity</typeparam>
    ''' <param name="entity">更新するテーブルEntity</param>
    ''' <param name="physicsNameList">更新するカラム名一覧</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>UPDATE SQL</returns>
    Private Function createUpdateSql(Of T)(entity As T, physicsNameList As List(Of String), tableName As String) As String

        MyBase.paramClear()

        Dim type As Type = GetType(T)
        Dim properties() As PropertyInfo = type.GetProperties()

        Dim idx As Integer = 0
        Dim comma As String = ""

        Dim physicsName As String = ""
        Dim updatePhysicsName As String = ""

        Dim valueSql As New StringBuilder()
        Dim format As String = "{0}{1} = {2}"

        For Each prop As PropertyInfo In properties

            If idx = 0 Then

                comma = ""
            End If

            If prop.PropertyType Is GetType(EntityKoumoku_YmdType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).PhysicsName

                If physicsNameList.Any(Function(x) x = physicsName) = False Then
                    ' 更新物理名リストにない場合、次レコードへ
                    Continue For
                End If

                valueSql.AppendLine(String.Format(format,
                                                  comma,
                                                  physicsName,
                                                  MyBase.setParam(physicsName,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).Value,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).DBType)))

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_NumberType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).PhysicsName

                If physicsNameList.Any(Function(x) x = physicsName) = False Then
                    ' 更新物理名リストにない場合、次レコードへ
                    Continue For
                End If

                valueSql.AppendLine(String.Format(format,
                                                  comma,
                                                  physicsName,
                                                  MyBase.setParam(physicsName,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DBType,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).IntegerBu,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DecimalBu)))

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_Number_DecimalType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).PhysicsName

                If physicsNameList.Any(Function(x) x = physicsName) = False Then
                    ' 更新物理名リストにない場合、次レコードへ
                    Continue For
                End If

                valueSql.AppendLine(String.Format(format,
                                                  comma,
                                                  physicsName,
                                                  MyBase.setParam(physicsName,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).Value,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DBType,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DecimalBu)))
            Else

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).PhysicsName

                If physicsNameList.Any(Function(x) x = physicsName) = False Then
                    ' 更新物理名リストにない場合、次レコードへ
                    Continue For
                End If

                valueSql.AppendLine(String.Format(format,
                                                  comma,
                                                  physicsName,
                                                  MyBase.setParam(physicsName,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).DBType,
                                                                  DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).IntegerBu)))
            End If

            comma = ","
            idx = idx + 1
        Next

        ' UPDATE文作成
        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine(String.Format("     {0} ", tableName))
        sb.AppendLine(" SET ")
        sb.AppendLine(valueSql.ToString())

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報２取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報２Entity</param>
    ''' <returns>予約情報２取得SQL</returns>
    Private Function createYoyakuInfo2Sql(entity As YoyakuInfo2Entity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      DELETE_DAY ")
        sb.AppendLine("     ,ENTRY_DAY ")
        sb.AppendLine("     ,ENTRY_PERSON_CD ")
        sb.AppendLine("     ,ENTRY_PGMID ")
        sb.AppendLine("     ,ENTRY_TIME ")
        sb.AppendLine("     ,OUT_DAY ")
        sb.AppendLine("     ,OUT_PERSON_CD ")
        sb.AppendLine("     ,OUT_PGMID ")
        sb.AppendLine("     ,OUT_TIME ")
        sb.AppendLine("     ,UPDATE_DAY ")
        sb.AppendLine("     ,UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UPDATE_PGMID ")
        sb.AppendLine("     ,UPDATE_TIME ")
        sb.AppendLine("     ,WARIBIKI_JIDO_CREATE_FLG ")
        sb.AppendLine("     ,YEAR ")
        sb.AppendLine("     ,YOBI_DAY_1 ")
        sb.AppendLine("     ,YOBI_DAY_2 ")
        sb.AppendLine("     ,YOBI_DAY_3 ")
        sb.AppendLine("     ,YOBI_FLG_1 ")
        sb.AppendLine("     ,YOBI_FLG_10 ")
        sb.AppendLine("     ,YOBI_FLG_2 ")
        sb.AppendLine("     ,YOBI_FLG_3 ")
        sb.AppendLine("     ,YOBI_FLG_4 ")
        sb.AppendLine("     ,YOBI_FLG_5 ")
        sb.AppendLine("     ,YOBI_FLG_6 ")
        sb.AppendLine("     ,YOBI_FLG_7 ")
        sb.AppendLine("     ,YOBI_FLG_8 ")
        sb.AppendLine("     ,YOBI_FLG_9 ")
        sb.AppendLine("     ,YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_2  ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（コース料金_料金区分）取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（コース料金_料金区分）Entity</param>
    ''' <returns>予約情報（コース料金_料金区分）取得SQL</returns>
    Private Function createYoyakuInfoCrsChargeChargeKbn(entity As YoyakuInfoCrsChargeChargeKbnEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")
        sb.AppendLine("     ,KBN_NO ")
        sb.AppendLine("     ,CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     ,CHARGE_KBN ")
        sb.AppendLine("     ,CARRIAGE ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_1 ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_2 ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_3 ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_4 ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU_5 ")
        sb.AppendLine("     ,CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,TANKA_1 ")
        sb.AppendLine("     ,TANKA_2 ")
        sb.AppendLine("     ,TANKA_3 ")
        sb.AppendLine("     ,TANKA_4 ")
        sb.AppendLine("     ,TANKA_5 ")
        sb.AppendLine("     ,CANCEL_NINZU_1 ")
        sb.AppendLine("     ,CANCEL_NINZU_2 ")
        sb.AppendLine("     ,CANCEL_NINZU_3 ")
        sb.AppendLine("     ,CANCEL_NINZU_4 ")
        sb.AppendLine("     ,CANCEL_NINZU_5 ")
        sb.AppendLine("     ,CANCEL_NINZU ")
        sb.AppendLine("     ,ENTRY_DAY ")
        sb.AppendLine("     ,ENTRY_PERSON_CD ")
        sb.AppendLine("     ,ENTRY_PGMID ")
        sb.AppendLine("     ,ENTRY_TIME ")
        sb.AppendLine("     ,UPDATE_DAY ")
        sb.AppendLine("     ,UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UPDATE_PGMID ")
        sb.AppendLine("     ,UPDATE_TIME ")
        sb.AppendLine("     ,DELETE_DAY ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN  ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報UPDATE SQL作成
    ''' </summary>
    ''' <param name="entity">対象Entity</param>
    ''' <param name="physicsNameList">更新するカラム名一覧</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>予約情報UPDATE SQL</returns>
    Private Function createYoyakuInfoUpdateSql(Of T)(entity As T, physicsNameList As List(Of String), tableName As String) As String

        Dim valueQuery As String = Me.createUpdateSql(Of T)(entity, physicsNameList, tableName)

        Dim type As Type = GetType(T)

        Dim yoyakuKbnProp As PropertyInfo = type.GetProperty("yoyakuKbn")
        Dim yoyakuNoProp As PropertyInfo = type.GetProperty("yoyakuNo")

        Dim sb As New StringBuilder()
        sb.AppendLine(valueQuery)
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(DirectCast(yoyakuKbnProp.GetValue(entity, Nothing), EntityKoumoku_MojiType).PhysicsName,
                                                             DirectCast(yoyakuKbnProp.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value,
                                                             DirectCast(yoyakuKbnProp.GetValue(entity, Nothing), EntityKoumoku_MojiType).DBType,
                                                             DirectCast(yoyakuKbnProp.GetValue(entity, Nothing), EntityKoumoku_MojiType).IntegerBu,
                                                             DirectCast(yoyakuKbnProp.GetValue(entity, Nothing), EntityKoumoku_MojiType).DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(DirectCast(yoyakuNoProp.GetValue(entity, Nothing), EntityKoumoku_NumberType).PhysicsName,
                                                            DirectCast(yoyakuNoProp.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value,
                                                            DirectCast(yoyakuNoProp.GetValue(entity, Nothing), EntityKoumoku_NumberType).DBType,
                                                            DirectCast(yoyakuNoProp.GetValue(entity, Nothing), EntityKoumoku_NumberType).IntegerBu,
                                                            DirectCast(yoyakuNoProp.GetValue(entity, Nothing), EntityKoumoku_NumberType).DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳座席情報更新SQL作成
    ''' 企画用
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」情報</param>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>コース台帳座席情報更新SQL</returns>
    Private Function createCrsZasekiDataForKikaku(z0003Result As Z0003_Result, crsLedgerZasekiData As DataTable, entity As CrsLedgerBasicEntity) As String

        Dim crsYoyakuSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim crsKusekiSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))

        ' 予約数定席
        ' コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim yoyakuSuTeisaki As Integer = crsYoyakuSuTeisaki + z0003Result.ZasekiKagenTeiseki
        ' 空席数定席算出
        ' コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        Dim kusekiSuTeisaki As Integer = Me.calcKuusekiSu(crsKusekiSuTeisaki, z0003Result.ZasekiKagenTeiseki, z0003Result.KusekiNumTeiseki)
        ' 空席数補助席算出
        ' コース台帳(基本).空席数定席,共通処理「バス座席自動設定処理」.座席加減数・定席,共通処理「バス座席自動設定処理」.補助調整空席,共通処理「バス座席自動設定処理」.空席数/補助・1F
        Dim kusekiSuSubSeat As Integer = Me.calcKusekiSuSubSeatForKikaku(crsKusekiSuTeisaki,
                                                                         z0003Result.ZasekiKagenTeiseki,
                                                                         z0003Result.SubCyoseiSeatNum,
                                                                         z0003Result.ZasekiKagenSub1F)

        If kusekiSuTeisaki < CommonRegistYoyaku.ZERO OrElse kusekiSuSubSeat < CommonRegistYoyaku.ZERO Then
            ' 空席数定席、空席数補助席が'0'を下回る場合、エラーとして更新処理終了

            Return ""
        End If

        ' 予約数定席
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki

        ' 予約数補助席
        If String.IsNullOrEmpty(z0003Result.SeatKbn) = False Then
            ' 共通処理「バス座席自動設定処理」.席区分が空以外の場合
            ' 予約数補助席を設定(コース台帳（基本）.予約数・補助席 + 共通処理「バス座席自動設定処理」.座席加減数・補助席)

            entity.yoyakuNumSubSeat.Value = z0003Result.ZasekiKagenSub1F
        End If

        ' 空席数定席
        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki
        ' 空席数補助席
        entity.kusekiNumSubSeat.Value = kusekiSuSubSeat

        ' コース台帳座席更新SQL作成
        Dim query As String = Me.createCrsZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' 空席数補助席算出
    ''' </summary>
    ''' <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    ''' <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・定席</param>
    ''' <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    ''' <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    ''' <returns></returns>
    Private Function calcKusekiSuSubSeatForKikaku(crsKusekiSu As Integer, comZasekiKagenSu As Integer, comSubChoseiKusekiSu As Integer, comKusekiSuSubSeat1F As Integer) As Integer

        Dim kusekiSu As Integer = crsKusekiSu - comZasekiKagenSu - comSubChoseiKusekiSu

        If kusekiSu >= comKusekiSuSubSeat1F Then
            ' 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席 - 共通処理「バス座席自動設定処理」.補助調整空席」が共通処理「バス座席自動設定処理」.空席数/補助・1F以上の場合
            kusekiSu = comKusekiSuSubSeat1F
        End If

        Return kusekiSu
    End Function

    ''' <summary>
    ''' 自コースと共用コースのコースコード、号車、出発日が同一チェック
    ''' </summary>
    ''' <param name="sharedRow">共用コースRow</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>検証結果</returns>
    Private Function isSharedBusCrsEqualCheck(sharedRow As DataRow, entity As CrsLedgerBasicEntity) As Boolean

        Dim csharedCrsCd As String = sharedRow("CRS_CD").ToString()
        Dim csharedGousya As Integer = Integer.Parse(sharedRow("GOUSYA").ToString())
        Dim csharedSyuptDay As Integer = Integer.Parse(sharedRow("SYUPT_DAY").ToString())

        If csharedCrsCd = entity.crsCd.Value AndAlso csharedGousya = entity.gousya.Value AndAlso csharedSyuptDay = entity.syuptDay.Value Then

            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 共用コース座席更新SQL作成
    ''' </summary>
    ''' <param name="z0003Result">共通処理「バス座席自動設定処理」</param>
    ''' <param name="sharedRow">共用コース座席情報</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>共用コース座席更新SQL</returns>
    Private Function createSharedBusCrsZasekiUpdateSql(z0003Result As Z0003_Result, sharedRow As DataRow, entity As CrsLedgerBasicEntity) As String

        Dim kusekiTeisaki As Integer = 0
        Dim kusekiSubSeat As Integer = 0

        Dim jyosyaCapacity As Integer = Integer.Parse(sharedRow("JYOSYA_CAPACITY").ToString())
        Dim capacityRegular As Integer = Integer.Parse(sharedRow("CAPACITY_REGULAR").ToString())
        Dim capacityHo1kai As Integer = Integer.Parse(sharedRow("CAPACITY_HO_1KAI").ToString())

        If jyosyaCapacity >= capacityRegular + capacityHo1kai Then
            ' コース台帳(基本).乗車定員が「コース台帳(基本).定員定 + コース台帳(基本).定員補1F」以上の場合

            ' 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席とする
            kusekiTeisaki = z0003Result.KusekiNumTeiseki
            ' 空席数補助席を共通処理「バス座席自動設定処理」.空席数・補助席とする
            kusekiSubSeat = z0003Result.KusekiNumSub
        Else
            ' それ以外の場合

            ' 営BLK算出
            Dim eiBlockNum As Integer = Me.calcEiBlockNum(sharedRow)

            If jyosyaCapacity >= capacityRegular Then
                ' コース台帳(基本).乗車定員がコース台帳(基本).定員定以上の場合

                If capacityRegular - eiBlockNum <= z0003Result.KusekiNumTeiseki Then
                    ' 「コース台帳(基本).定員定 - 営BLK算出結果」が共通処理「バス座席自動設定処理」.空席数・定席以下の場合

                    ' 空席数・定席を「コース台帳(基本).定員定 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum
                Else
                    ' それ以外の場合

                    ' 空席数・定席を共通処理「バス座席自動設定処理」.空席数・定席とする
                    kusekiTeisaki = z0003Result.KusekiNumTeiseki
                End If

                Dim eiBlockHo As Integer = Integer.Parse(sharedRow("EI_BLOCK_HO").ToString())
                Dim yoyaokuNumSubSeat As Integer = Integer.Parse(sharedRow("YOYAKU_NUM_SUB_SEAT").ToString())

                If capacityRegular - eiBlockHo - yoyaokuNumSubSeat <= z0003Result.KusekiNumSub Then
                    ' 「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」が
                    ' 共通処理「バス座席自動設定処理」.空席数・補助席以下の場合

                    ' 空席数・補助席を「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」とする
                    kusekiSubSeat = capacityRegular - eiBlockHo - yoyaokuNumSubSeat
                Else
                    ' それ以外の場合

                    ' 空席数・補助席をを共通処理「バス座席自動設定処理」空席数・補助席とする
                    kusekiSubSeat = z0003Result.KusekiNumSub
                End If
            Else
                ' それ以外の場合、

                If jyosyaCapacity - eiBlockNum <= z0003Result.KusekiNumTeiseki Then
                    ' 「コース台帳(基本).乗車定員 - 営BLK算出結果」が共通処理「バス座席自動設定処理」空席数・定席以下の場合

                    ' 空席数・定席を「コース台帳(基本).乗車定員 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum
                Else
                    ' それ以外の場合

                    ' 空席数・定席を共通処理「バス座席自動設定処理」空席数・定席とする
                    kusekiTeisaki = z0003Result.KusekiNumTeiseki
                End If

                ' 空席数・補助席は'0'とする
                kusekiSubSeat = CommonRegistYoyaku.ZERO
            End If
        End If

        ' WHERE条件設定
        Dim sharedEntity As New CrsLedgerBasicEntity()
        sharedEntity.crsCd.Value = entity.crsCd.Value
        sharedEntity.syuptDay.Value = entity.syuptDay.Value
        sharedEntity.gousya.Value = entity.gousya.Value
        sharedEntity.systemUpdateDay.Value = entity.systemUpdateDay.Value
        sharedEntity.systemUpdatePersonCd.Value = entity.systemUpdatePersonCd.Value
        sharedEntity.systemUpdatePgmid.Value = entity.systemUpdatePgmid.Value

        ' 更新SQL作成
        Dim query As String = Me.createSharedCrsZasekiUpdateSql(kusekiTeisaki, kusekiSubSeat, sharedEntity)

        Return query
    End Function

    ''' <summary>
    ''' 営BLK算出
    ''' </summary>
    ''' <param name="sharedRow">共用コース座席情報</param>
    ''' <returns>営BLK</returns>
    Private Function calcEiBlockNum(sharedRow As DataRow) As Integer

        Dim eiBlockRegular As Integer = Integer.Parse(sharedRow("EI_BLOCK_REGULAR").ToString())
        Dim blockKakuhoNum As Integer = Integer.Parse(sharedRow("BLOCK_KAKUHO_NUM").ToString())
        Dim yoyakuNumTeisaki As Integer = Integer.Parse(sharedRow("YOYAKU_NUM_TEISEKI").ToString())

        ' 営BLK = 「コース台帳(基本).営ブロック定 + コース台帳(基本).ブロック確保数 + コース台帳(基本).空席確保数 + コース台帳(基本).予約数・定席
        Dim eiBlk As Integer = eiBlockRegular + blockKakuhoNum + yoyakuNumTeisaki

        Return eiBlk
    End Function

    ''' <summary>
    ''' 共用コース座席更新SQL作成
    ''' </summary>
    ''' <param name="kusekiTeisaki">空席数定席</param>
    ''' <param name="kusekiSubSeat">空席数補助席</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>共用コース座席更新SQL</returns>
    Private Function createSharedCrsZasekiUpdateSql(kusekiTeisaki As Integer, kusekiSubSeat As Integer, entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, kusekiTeisaki, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, kusekiSubSeat, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報更新
    ''' </summary>
    ''' <param name="registEntity">変更予約情報Entity</param>
    ''' <param name="oracleTransaction">オラクルトランザクション</param>
    ''' <returns>更新ステータス</returns>
    Private Function updateYoyakuInfo(registEntity As ChangeYoyakuInfoEntity, oracleTransaction As OracleTransaction) As String

        Dim updateCount As Integer = 0

        ' 予約情報（基本）
        Dim yoyakuInfoQuery As String = Me.createYoyakuInfoUpdateSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
        updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

        If updateCount <= 0 Then

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Return S02_0104.UpdateStatusYoyakuUpdateFailure
        End If

        ' 予約情報（コース料金）
        Dim crsChargeQuery As String = Me.createYoyakuInfoUpdateSql(Of YoyakuInfoCrsChargeEntity)(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE")
        updateCount = MyBase.execNonQuery(oracleTransaction, crsChargeQuery)

        If updateCount <= 0 Then

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Return S02_0104.UpdateStatusYoyakuCrsChargeUpdateFailure
        End If

        ' 予約情報（コース料金_料金区分）
        Dim chargeKbnQuery As String = Me.createYoyakuInfoCrsChargeChargeKbn(registEntity.YoyakuInfoCrsChargeChargeKbnList(0))
        Dim chargeKbnData As DataTable = MyBase.getDataTable(chargeKbnQuery)

        Dim chargeKbnRegistQuery As String = ""
        For Each entity As YoyakuInfoCrsChargeChargeKbnEntity In registEntity.YoyakuInfoCrsChargeChargeKbnList

            Dim chargeKbnRow() As DataRow = chargeKbnData.Select(String.Format("KBN_NO = {0} AND CHARGE_KBN_JININ_CD = '{1}'", entity.kbnNo.Value, entity.chargeKbnJininCd.Value))

            If chargeKbnRow.Length > Zero Then

                Dim valueQuery = Me.createUpdateSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")

                Dim whereQuery As New StringBuilder()
                whereQuery.AppendLine(valueQuery)
                whereQuery.AppendLine(" WHERE ")
                whereQuery.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName,
                                                                     entity.yoyakuKbn.Value,
                                                                     entity.yoyakuKbn.DBType,
                                                                     entity.yoyakuKbn.IntegerBu,
                                                                     entity.yoyakuKbn.DecimalBu))
                whereQuery.AppendLine("     AND ")
                whereQuery.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName,
                                                                    entity.yoyakuNo.Value,
                                                                    entity.yoyakuNo.DBType,
                                                                    entity.yoyakuNo.IntegerBu,
                                                                    entity.yoyakuNo.DecimalBu))
                whereQuery.AppendLine("     AND ")
                whereQuery.AppendLine("     KBN_NO = " + MyBase.setParam(entity.kbnNo.PhysicsName,
                                                                         entity.kbnNo.Value,
                                                                         entity.kbnNo.DBType,
                                                                         entity.kbnNo.IntegerBu,
                                                                         entity.kbnNo.DecimalBu))
                whereQuery.AppendLine("     AND ")
                whereQuery.AppendLine("     CHARGE_KBN_JININ_CD = " + MyBase.setParam(entity.chargeKbnJininCd.PhysicsName,
                                                                                      entity.chargeKbnJininCd.Value,
                                                                                      entity.chargeKbnJininCd.DBType,
                                                                                      entity.chargeKbnJininCd.IntegerBu,
                                                                                      entity.chargeKbnJininCd.DecimalBu))

                chargeKbnRegistQuery = whereQuery.ToString()
            Else

                chargeKbnRegistQuery = Me.createInsertSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
            End If

            updateCount = MyBase.execNonQuery(oracleTransaction, chargeKbnRegistQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return S02_0104.UpdateStatusYoyakuChargeKbnUpdateFailure
            End If
        Next

        ' 予約情報２
        Dim yoyakuInfo2Query As String

        Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
        Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

        If yoyakuInfo2.Rows.Count > 0 Then

            yoyakuInfo2Query = Me.createYoyakuInfoUpdateSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2")
        Else

            yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
        End If

        updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)

        If updateCount <= 0 Then

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Return S02_0104.UpdateStatusYoyakuInfo2UpdateFailure
        End If

        Return S02_0104.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 相部屋部屋残数更新SQL作成
    ''' </summary>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="zasekiInfoEntity">座席情報Entity</param>
    ''' <param name="newCrsLedgerBasicEntity">コース台帳（基本）Entity</param>
    ''' <returns>相部屋部屋残数更新SQL</returns>
    Private Function createAibeyaRoomZansuSql(zasekiData As DataTable, zasekiInfoEntity As ZasekiInfoEntity, newCrsLedgerBasicEntity As CrsLedgerBasicEntity) As String

        Dim teiinseiFlag As String = zasekiData.Rows(0)("TEIINSEI_FLG").ToString()
        If String.IsNullOrEmpty(teiinseiFlag) = False Then
            ' 定員制の場合、ルームの残数管理は行わない
            Return ""
        End If

        Dim shoriDate As Integer = CInt(zasekiInfoEntity.ShoriDate.ToString("yyyyMMdd"))
        If zasekiInfoEntity.SyuptDay = shoriDate Then
            ' 出発日当日の場合、ルームの残数管理は行わない
            Return ""
        End If

        ' ROOM MAX定員数取得
        Dim roomMaxCap As Double = Me.getRoomMaxTein(newCrsLedgerBasicEntity)

        ' 男性
        Dim aibeyaManNinzu As Double = Double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE").ToString())
        Dim w1rom1 As Double = Math.Ceiling(aibeyaManNinzu / roomMaxCap)

        Dim yoyakuManNinzu As Double = Double.Parse(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value.ToString())
        aibeyaManNinzu = aibeyaManNinzu + yoyakuManNinzu

        Dim w1rom2 As Double = Math.Ceiling(aibeyaManNinzu / roomMaxCap)

        Dim manKagenRoomSu As Integer = CInt(w1rom2 - w1rom1)

        ' 女性
        Dim aibeyaWoManNinzu As Double = Double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI").ToString())
        w1rom1 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap)

        Dim yoyakuWomanNinzu As Double = Double.Parse(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value.ToString())
        aibeyaWoManNinzu = aibeyaWoManNinzu + yoyakuWomanNinzu

        w1rom2 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap)

        Dim womanKagenRoomSu As Integer = CInt(w1rom2 - w1rom1)

        ' 相部屋予約人数男性
        newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value = CInt(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE")) + CInt(aibeyaManNinzu)
        ' 相部屋予約人数女性
        newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value = CInt(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI")) + CInt(aibeyaWoManNinzu)
        ' 部屋残数総計
        newCrsLedgerBasicEntity.roomZansuSokei.Value = Integer.Parse(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI").ToString()) - manKagenRoomSu - womanKagenRoomSu
        ' 予約済ＲＯＯＭ数
        newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value = Integer.Parse(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM").ToString()) + manKagenRoomSu + womanKagenRoomSu
        ' 部屋残数１人部屋 ～ 部屋残数５人部屋
        newCrsLedgerBasicEntity.roomZansuOneRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM")
        newCrsLedgerBasicEntity.roomZansuTwoRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM")
        newCrsLedgerBasicEntity.roomZansuThreeRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM")
        newCrsLedgerBasicEntity.roomZansuFourRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM")
        newCrsLedgerBasicEntity.roomZansuFiveRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM")

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      AIBEYA_YOYAKU_NINZU_MALE =  " + MyBase.setParam(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.PhysicsName, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.Value, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.DBType, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.IntegerBu, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuMale.DecimalBu))
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI =  " + MyBase.setParam(newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.PhysicsName, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.Value, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.DBType, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.IntegerBu, newCrsLedgerBasicEntity.aibeyaYoyakuNinzuJyosei.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuOneRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuOneRoom.Value, newCrsLedgerBasicEntity.roomZansuOneRoom.DBType, newCrsLedgerBasicEntity.roomZansuOneRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuOneRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuTwoRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuTwoRoom.Value, newCrsLedgerBasicEntity.roomZansuTwoRoom.DBType, newCrsLedgerBasicEntity.roomZansuTwoRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuTwoRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuThreeRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuThreeRoom.Value, newCrsLedgerBasicEntity.roomZansuThreeRoom.DBType, newCrsLedgerBasicEntity.roomZansuThreeRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuThreeRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuFourRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFourRoom.Value, newCrsLedgerBasicEntity.roomZansuFourRoom.DBType, newCrsLedgerBasicEntity.roomZansuFourRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFourRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuFiveRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFiveRoom.Value, newCrsLedgerBasicEntity.roomZansuFiveRoom.DBType, newCrsLedgerBasicEntity.roomZansuFiveRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFiveRoom.DecimalBu))
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + MyBase.setParam(newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.PhysicsName, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DBType, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.IntegerBu, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuSokei.PhysicsName, newCrsLedgerBasicEntity.roomZansuSokei.Value, newCrsLedgerBasicEntity.roomZansuSokei.DBType, newCrsLedgerBasicEntity.roomZansuSokei.IntegerBu, newCrsLedgerBasicEntity.roomZansuSokei.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(newCrsLedgerBasicEntity.crsCd.PhysicsName, newCrsLedgerBasicEntity.crsCd.Value, newCrsLedgerBasicEntity.crsCd.DBType, newCrsLedgerBasicEntity.crsCd.IntegerBu, newCrsLedgerBasicEntity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA =  " + MyBase.setParam(newCrsLedgerBasicEntity.syuptDay.PhysicsName, newCrsLedgerBasicEntity.syuptDay.Value, newCrsLedgerBasicEntity.syuptDay.DBType, newCrsLedgerBasicEntity.syuptDay.IntegerBu, newCrsLedgerBasicEntity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY =  " + MyBase.setParam(newCrsLedgerBasicEntity.gousya.PhysicsName, newCrsLedgerBasicEntity.gousya.Value, newCrsLedgerBasicEntity.gousya.DBType, newCrsLedgerBasicEntity.gousya.IntegerBu, newCrsLedgerBasicEntity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' ROOM MAX定員数取得
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>ROOM MAX定員数</returns>
    Private Function getRoomMaxTein(entity As CrsLedgerBasicEntity) As Double

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      MSS.SIIRE_SAKI_CD ")
        sb.AppendLine("     ,MSS.SIIRE_SAKI_NO ")
        sb.AppendLine("     ,MSS.ROOM_MAX_CAPACITY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_HOTEL CLH ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_SIIRE_SAKI MSS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     MSS.SIIRE_SAKI_CD = CLH.SIIRE_SAKI_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     MSS.SIIRE_SAKI_NO = CLH.SIIRE_SAKI_EDABAN ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Dim roomMaxInfo As DataTable = MyBase.getDataTable(sb.ToString())

        Const roomMaxCap As Double = 99

        If roomMaxInfo.Rows.Count <= 0 Then

            Return roomMaxCap
        End If

        Dim roomMax As Double = 0
        Double.TryParse(roomMaxInfo.Rows(0)("ROOM_MAX_CAPACITY").ToString(), roomMax)

        If roomMax < roomMaxCap Then

            Return roomMax
        End If

        Return roomMaxCap
    End Function

    ''' <summary>
    ''' 部屋残数算出
    ''' </summary>
    ''' <param name="zasekiData"></param>
    ''' <param name="manKagenSu">男性加減数</param>
    ''' <param name="womanKagenSu">女性加減数</param>
    ''' <param name="crsBlockColName">コースブロックカラム名</param>
    ''' <param name="roomZansuColName">部屋残数カラム名</param>
    ''' <returns>部屋残数</returns>
    Private Function calcRoomZansuAibeyaAri(zasekiData As DataTable, manKagenSu As Integer, womanKagenSu As Integer, crsBlockColName As String, roomZansuColName As String) As Integer

        Dim crsBlockRoomSu As Integer = Zero
        Dim crsRoomZanSu As Integer = Zero

        Integer.TryParse(zasekiData.Rows(0)(crsBlockColName).ToString(), crsBlockRoomSu)
        Integer.TryParse(zasekiData.Rows(0)(roomZansuColName).ToString(), crsRoomZanSu)

        If crsBlockRoomSu = Zero Then

            Return crsRoomZanSu
        End If

        Dim roomZansu As Integer = crsRoomZanSu - manKagenSu - womanKagenSu

        Return roomZansu
    End Function

    ''' <summary>
    ''' 部屋残数一覧取得
    ''' </summary>
    ''' <param name="zasekiInfoEntity">座席情報Entity</param>
    ''' <param name="yoyakuChangeKbn">予約変更区分</param>
    ''' <returns>部屋残数一覧</returns>
    Private Function getRoomZansu(zasekiInfoEntity As ZasekiInfoEntity, yoyakuChangeKbn As String) As List(Of Integer)

        Dim list As New List(Of Integer)

        If yoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

            list.Add(zasekiInfoEntity.Room1Num)
            list.Add(zasekiInfoEntity.Room2Num)
            list.Add(zasekiInfoEntity.Room3Num)
            list.Add(zasekiInfoEntity.Room4Num)
            list.Add(zasekiInfoEntity.Room5Num)
        Else

            If String.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) = True AndAlso String.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) = True Then

                list.Add(Me.getDifRoomNum(zasekiInfoEntity.Room1Num, zasekiInfoEntity.OldRoom1Num))
                list.Add(Me.getDifRoomNum(zasekiInfoEntity.Room2Num, zasekiInfoEntity.OldRoom2Num))
                list.Add(Me.getDifRoomNum(zasekiInfoEntity.Room3Num, zasekiInfoEntity.OldRoom3Num))
                list.Add(Me.getDifRoomNum(zasekiInfoEntity.Room4Num, zasekiInfoEntity.OldRoom4Num))
                list.Add(Me.getDifRoomNum(zasekiInfoEntity.Room5Num, zasekiInfoEntity.OldRoom5Num))
            Else

                list.Add(zasekiInfoEntity.Room1Num)
                list.Add(zasekiInfoEntity.Room2Num)
                list.Add(zasekiInfoEntity.Room3Num)
                list.Add(zasekiInfoEntity.Room4Num)
                list.Add(zasekiInfoEntity.Room5Num)
            End If
        End If

        Return list
    End Function

    ''' <summary>
    ''' 部屋差分数取得
    ''' </summary>
    ''' <param name="newRoomNum">新部屋数</param>
    ''' <param name="oldRoomNum">旧部屋数</param>
    ''' <returns></returns>
    Private Function getDifRoomNum(newRoomNum As Integer, oldRoomNum As Integer) As Integer

        Dim roomNum As Integer = newRoomNum - oldRoomNum

        If roomNum < Zero Then

            roomNum = Zero
        End If

        Return roomNum
    End Function

    ''' <summary>
    ''' 部屋残数更新SQL作成
    ''' </summary>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="newCrsLedgerBasicEntity">新コース台帳（基本）Entity</param>
    ''' <param name="zasekiInfoEntity">座席情報Entity</param>
    ''' <param name="yoyakuChangeKbn">予約変更区分</param>
    ''' <returns>部屋残数更新SQL</returns>
    Private Function createCrsRoomAibeyaNashiUpdateSql(zasekiData As DataTable, newCrsLedgerBasicEntity As CrsLedgerBasicEntity, zasekiInfoEntity As ZasekiInfoEntity, yoyakuChangeKbn As String) As String

        Dim teiinseiFlag As String = zasekiData.Rows(0)("TEIINSEI_FLG").ToString()
        If String.IsNullOrEmpty(teiinseiFlag) = False Then
            ' 定員制の場合、ルームの残数管理は行わない
            Return ""
        End If

        Dim shoriDate As Integer = CInt(zasekiInfoEntity.ShoriDate.ToString("yyyyMMdd"))
        If zasekiInfoEntity.SyuptDay = shoriDate Then
            ' 出発日当日の場合、ルームの残数管理は行わない
            Return ""
        End If

        ' 部屋残数一覧取得
        Dim roomList As List(Of Integer) = Me.getRoomZansu(zasekiInfoEntity, yoyakuChangeKbn)

        newCrsLedgerBasicEntity.roomZansuOneRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, roomList(0), "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM")
        newCrsLedgerBasicEntity.roomZansuTwoRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, roomList(1), "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM")
        newCrsLedgerBasicEntity.roomZansuThreeRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, roomList(2), "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM")
        newCrsLedgerBasicEntity.roomZansuFourRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, roomList(3), "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM")
        newCrsLedgerBasicEntity.roomZansuFiveRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, roomList(4), "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM")

        ' 予約部屋数総数算出
        Dim totalRoomsu As Integer = 0
        For Each roomNum As Integer In roomList

            totalRoomsu = totalRoomsu + roomNum
        Next
        newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value = Integer.Parse(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM").ToString()) + totalRoomsu
        newCrsLedgerBasicEntity.roomZansuSokei.Value = Integer.Parse(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI").ToString()) - totalRoomsu

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      ROOM_ZANSU_ONE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuOneRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuOneRoom.Value, newCrsLedgerBasicEntity.roomZansuOneRoom.DBType, newCrsLedgerBasicEntity.roomZansuOneRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuOneRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuTwoRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuTwoRoom.Value, newCrsLedgerBasicEntity.roomZansuTwoRoom.DBType, newCrsLedgerBasicEntity.roomZansuTwoRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuTwoRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuThreeRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuThreeRoom.Value, newCrsLedgerBasicEntity.roomZansuThreeRoom.DBType, newCrsLedgerBasicEntity.roomZansuThreeRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuThreeRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuFourRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFourRoom.Value, newCrsLedgerBasicEntity.roomZansuFourRoom.DBType, newCrsLedgerBasicEntity.roomZansuFourRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFourRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuFiveRoom.PhysicsName, newCrsLedgerBasicEntity.roomZansuFiveRoom.Value, newCrsLedgerBasicEntity.roomZansuFiveRoom.DBType, newCrsLedgerBasicEntity.roomZansuFiveRoom.IntegerBu, newCrsLedgerBasicEntity.roomZansuFiveRoom.DecimalBu))
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + MyBase.setParam(newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.PhysicsName, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.Value, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DBType, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.IntegerBu, newCrsLedgerBasicEntity.yoyakuAlreadyRoomNum.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + MyBase.setParam(newCrsLedgerBasicEntity.roomZansuSokei.PhysicsName, newCrsLedgerBasicEntity.roomZansuSokei.Value, newCrsLedgerBasicEntity.roomZansuSokei.DBType, newCrsLedgerBasicEntity.roomZansuSokei.IntegerBu, newCrsLedgerBasicEntity.roomZansuSokei.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(newCrsLedgerBasicEntity.crsCd.PhysicsName, newCrsLedgerBasicEntity.crsCd.Value, newCrsLedgerBasicEntity.crsCd.DBType, newCrsLedgerBasicEntity.crsCd.IntegerBu, newCrsLedgerBasicEntity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY =  " + MyBase.setParam(newCrsLedgerBasicEntity.syuptDay.PhysicsName, newCrsLedgerBasicEntity.syuptDay.Value, newCrsLedgerBasicEntity.syuptDay.DBType, newCrsLedgerBasicEntity.syuptDay.IntegerBu, newCrsLedgerBasicEntity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA =  " + MyBase.setParam(newCrsLedgerBasicEntity.gousya.PhysicsName, newCrsLedgerBasicEntity.gousya.Value, newCrsLedgerBasicEntity.gousya.DBType, newCrsLedgerBasicEntity.gousya.IntegerBu, newCrsLedgerBasicEntity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 部屋残数算出
    ''' </summary>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="roomingBetuNinzu">予約部屋数</param>
    ''' <param name="crsBlockColName">コースブロックカラム名</param>
    ''' <param name="roomZansuColName">部屋残数カラム名</param>
    ''' <returns>部屋残数</returns>
    Private Function calcCrsRoomAibeyaNashi(zasekiData As DataTable, roomingBetuNinzu As Integer, crsBlockColName As String, roomZansuColName As String) As Integer

        Dim crsBlockRoomSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName))
        Dim crsRoomZanSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(roomZansuColName))

        If crsBlockRoomSu = 0 Then

            Return crsRoomZanSu
        End If

        Dim roomZansu As Integer = crsRoomZanSu - roomingBetuNinzu

        Return roomZansu
    End Function

    ''' <summary>
    ''' 旧部屋残数一覧取得
    ''' </summary>
    ''' <param name="zasekiInfoEntity">座席情報Entity</param>
    ''' <param name="yoyakuChangeKbn">予約変更区分</param>
    ''' <returns>部屋残数一覧</returns>
    Private Function getOldRoomZansu(zasekiInfoEntity As ZasekiInfoEntity, yoyakuChangeKbn As String) As List(Of Integer)

        Dim list As New List(Of Integer)

        If yoyakuChangeKbn = S02_0104.YoyakuChangeKbnGosyaSyuptDayHenko Then

            list.Add(zasekiInfoEntity.OldRoom1Num)
            list.Add(zasekiInfoEntity.OldRoom2Num)
            list.Add(zasekiInfoEntity.OldRoom3Num)
            list.Add(zasekiInfoEntity.OldRoom4Num)
            list.Add(zasekiInfoEntity.OldRoom5Num)
        Else

            If String.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) = True AndAlso String.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) = True Then

                list.Add(Me.getOldDifRoomNum(zasekiInfoEntity.Room1Num, zasekiInfoEntity.OldRoom1Num))
                list.Add(Me.getOldDifRoomNum(zasekiInfoEntity.Room2Num, zasekiInfoEntity.OldRoom2Num))
                list.Add(Me.getOldDifRoomNum(zasekiInfoEntity.Room3Num, zasekiInfoEntity.OldRoom3Num))
                list.Add(Me.getOldDifRoomNum(zasekiInfoEntity.Room4Num, zasekiInfoEntity.OldRoom4Num))
                list.Add(Me.getOldDifRoomNum(zasekiInfoEntity.Room5Num, zasekiInfoEntity.OldRoom5Num))

            ElseIf String.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) = True AndAlso String.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) = False Then

                list.Add(zasekiInfoEntity.OldRoom1Num)
                list.Add(zasekiInfoEntity.OldRoom2Num)
                list.Add(zasekiInfoEntity.OldRoom3Num)
                list.Add(zasekiInfoEntity.OldRoom4Num)
                list.Add(zasekiInfoEntity.OldRoom5Num)

            ElseIf String.IsNullOrEmpty(zasekiInfoEntity.OldAibeyaFlag) = False AndAlso String.IsNullOrEmpty(zasekiInfoEntity.AibeyaFlag) = True Then

                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
            Else

                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
                list.Add(Zero)
            End If
        End If

        Return list
    End Function

    ''' <summary>
    ''' 旧部屋差分数取得
    ''' </summary>
    ''' <param name="newRoomNum">新部屋数</param>
    ''' <param name="oldRoomNum">旧部屋数</param>
    ''' <returns></returns>
    Private Function getOldDifRoomNum(newRoomNum As Integer, oldRoomNum As Integer) As Integer

        Dim roomNum As Integer = oldRoomNum - newRoomNum

        If roomNum < Zero Then

            roomNum = Zero
        End If

        Return roomNum
    End Function

    ''' <summary>
    ''' 相部屋仕様人数取得
    ''' </summary>
    ''' <param name="oldAibeyaYoyakuNinzu">旧相部屋人数</param>
    ''' <param name="newAibeyaYoyakuNinzu">新相部屋人数</param>
    ''' <returns>相部屋仕様人数</returns>
    Private Function getAibeyaUsingNinzu(oldAibeyaYoyakuNinzu As Integer, newAibeyaYoyakuNinzu As Integer) As Integer

        Dim ninzu As Integer = oldAibeyaYoyakuNinzu - newAibeyaYoyakuNinzu

        If ninzu < Zero Then

            ninzu = Zero
        End If

        Return ninzu
    End Function

    ''' <summary>
    ''' 解放する部屋数算出
    ''' </summary>
    ''' <param name="oldZasekiData">旧座席情報</param>
    ''' <param name="colName">相部屋のカラム名</param>
    ''' <param name="aibeyaYoyakuNinzu">相部屋の予約人数</param>
    ''' <param name="roomMaxCap">ROOM MAX定員数</param>
    ''' <param name="aibeyaYoyakuNum">相部屋人数</param>
    ''' <returns>解放部屋数</returns>
    Private Function getAibeyaKaihoNum(oldZasekiData As DataTable, colName As String, aibeyaYoyakuNinzu As Integer, roomMaxCap As Double, ByRef aibeyaYoyakuNum As Integer) As Integer

        Dim aibeyaNinzu As Double = Double.Parse(oldZasekiData.Rows(0)(colName).ToString())
        Dim w1rom1 As Double = Math.Ceiling(aibeyaNinzu / roomMaxCap)

        aibeyaNinzu = aibeyaNinzu - aibeyaYoyakuNinzu
        aibeyaYoyakuNum = CInt(aibeyaNinzu)

        Dim w1rom2 As Double = Math.Ceiling(aibeyaNinzu / roomMaxCap)

        Dim kaihoRoomSu As Integer = CInt(w1rom2 - w1rom1)

        Return kaihoRoomSu
    End Function

    ''' <summary>
    ''' 座席解放SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>座席解放SQL</returns>
    Private Function createZasekiKaihoSql(entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      AIBEYA_YOYAKU_NINZU_MALE =  " + MyBase.setParam(entity.aibeyaYoyakuNinzuMale.PhysicsName, entity.aibeyaYoyakuNinzuMale.Value, entity.aibeyaYoyakuNinzuMale.DBType, entity.aibeyaYoyakuNinzuMale.IntegerBu, entity.aibeyaYoyakuNinzuMale.DecimalBu))
        sb.AppendLine("     ,AIBEYA_YOYAKU_NINZU_JYOSEI =  " + MyBase.setParam(entity.aibeyaYoyakuNinzuJyosei.PhysicsName, entity.aibeyaYoyakuNinzuJyosei.Value, entity.aibeyaYoyakuNinzuJyosei.DBType, entity.aibeyaYoyakuNinzuJyosei.IntegerBu, entity.aibeyaYoyakuNinzuJyosei.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_ONE_ROOM =  " + MyBase.setParam(entity.roomZansuOneRoom.PhysicsName, entity.roomZansuOneRoom.Value, entity.roomZansuOneRoom.DBType, entity.roomZansuOneRoom.IntegerBu, entity.roomZansuOneRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_TWO_ROOM =  " + MyBase.setParam(entity.roomZansuTwoRoom.PhysicsName, entity.roomZansuTwoRoom.Value, entity.roomZansuTwoRoom.DBType, entity.roomZansuTwoRoom.IntegerBu, entity.roomZansuTwoRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_THREE_ROOM =  " + MyBase.setParam(entity.roomZansuThreeRoom.PhysicsName, entity.roomZansuThreeRoom.Value, entity.roomZansuThreeRoom.DBType, entity.roomZansuThreeRoom.IntegerBu, entity.roomZansuThreeRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FOUR_ROOM =  " + MyBase.setParam(entity.roomZansuFourRoom.PhysicsName, entity.roomZansuFourRoom.Value, entity.roomZansuFourRoom.DBType, entity.roomZansuFourRoom.IntegerBu, entity.roomZansuFourRoom.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_FIVE_ROOM =  " + MyBase.setParam(entity.roomZansuFiveRoom.PhysicsName, entity.roomZansuFiveRoom.Value, entity.roomZansuFiveRoom.DBType, entity.roomZansuFiveRoom.IntegerBu, entity.roomZansuFiveRoom.DecimalBu))
        sb.AppendLine("     ,YOYAKU_ALREADY_ROOM_NUM = " + MyBase.setParam(entity.yoyakuAlreadyRoomNum.PhysicsName, entity.yoyakuAlreadyRoomNum.Value, entity.yoyakuAlreadyRoomNum.DBType, entity.yoyakuAlreadyRoomNum.IntegerBu, entity.yoyakuAlreadyRoomNum.DecimalBu))
        sb.AppendLine("     ,ROOM_ZANSU_SOKEI = " + MyBase.setParam(entity.roomZansuSokei.PhysicsName, entity.roomZansuSokei.Value, entity.roomZansuSokei.DBType, entity.roomZansuSokei.IntegerBu, entity.roomZansuSokei.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY =  " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA =  " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 部屋数解放SQL作成
    ''' </summary>
    ''' <param name="oldZasekiData">旧コース台帳座席情報</param>
    ''' <param name="registEntity">変更予約情報Entity</param>
    ''' <param name="oldCrsInfoEntity">旧コース台帳（基本）Entity</param>
    ''' <returns>座席解放SQL</returns>
    Private Function createZasekiKaihoSql(oldZasekiData As DataTable, registEntity As ChangeYoyakuInfoEntity, oldCrsInfoEntity As CrsLedgerBasicEntity) As String

        Dim shoriDate As Integer = CInt(registEntity.ZasekiInfoEntity.ShoriDate.ToString("yyyyMMdd"))
        If oldCrsInfoEntity.syuptDay.Value = shoriDate Then
            ' 出発日が当日の場合、ルームの残数管理は行わない
            Return ""
        End If

        Dim teiinseFlag As String = oldZasekiData.Rows(0)("TEIINSEI_FLG").ToString()
        If String.IsNullOrEmpty(teiinseFlag) = False Then
            ' 旧コースの定員制フラグが空以外の場合、ルームの残数管理は行わない
            Return ""
        End If

        ' 旧部屋残数一覧取得
        Dim oldRoomList As List(Of Integer) = Me.getOldRoomZansu(registEntity.ZasekiInfoEntity, registEntity.YoyakuChangeKbn)
        ' 合計旧部屋残数
        Dim totalRoomsu As Integer = 0
        For Each roomNum As Integer In oldRoomList

            totalRoomsu = totalRoomsu + roomNum
        Next

        ' 男性
        Dim aibeyaYoyakuNinzuMale As Integer = Me.getAibeyaUsingNinzu(registEntity.ZasekiInfoEntity.OldAibeyaYoyakuNinzuMale, registEntity.ZasekiInfoEntity.AibeyaYoyakuNinzuMale)
        ' 女性
        Dim aibeyaYoyakuNinzuJyosei As Integer = Me.getAibeyaUsingNinzu(registEntity.ZasekiInfoEntity.OldAibeyaYoyakuNinzuJyosei, registEntity.ZasekiInfoEntity.AibeyaYoyakuNinzuJyosei)

        ' ROOM MAX定員数取得
        Dim roomMaxCap As Double = Me.getRoomMaxTein(oldCrsInfoEntity)
        ' 相部屋男性
        Dim aibeyaManNinzu As Integer = Zero
        Dim kaihoRoomNumMan As Integer = Me.getAibeyaKaihoNum(oldZasekiData, "AIBEYA_YOYAKU_NINZU_MALE", aibeyaYoyakuNinzuMale, roomMaxCap, aibeyaManNinzu)
        ' 相部屋女性
        Dim aibeyaWomanNinzu As Integer = Zero
        Dim kaihoRoomNumWoman As Integer = Me.getAibeyaKaihoNum(oldZasekiData, "AIBEYA_YOYAKU_NINZU_JYOSEI", aibeyaYoyakuNinzuJyosei, roomMaxCap, aibeyaWomanNinzu)

        ' Entity設定
        ' 相部屋予約人数男性
        oldCrsInfoEntity.aibeyaYoyakuNinzuMale.Value = aibeyaManNinzu
        ' 相部屋予約人数女性
        oldCrsInfoEntity.aibeyaYoyakuNinzuJyosei.Value = aibeyaWomanNinzu
        ' 予約済ＲＯＯＭ数
        oldCrsInfoEntity.yoyakuAlreadyRoomNum.Value = CInt(oldZasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) - totalRoomsu - kaihoRoomNumMan - kaihoRoomNumWoman
        ' 部屋残数総計
        oldCrsInfoEntity.roomZansuSokei.Value = CInt(oldZasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) + totalRoomsu + kaihoRoomNumMan + kaihoRoomNumWoman

        ' 部屋残数１人部屋 ～ 部屋残数５人部屋(相部屋)
        oldCrsInfoEntity.roomZansuOneRoom.Value = Me.calcRoomZansuAibeyaAri(oldZasekiData, (kaihoRoomNumMan * -1), (kaihoRoomNumWoman * -1), "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM")
        oldCrsInfoEntity.roomZansuTwoRoom.Value = Me.calcRoomZansuAibeyaAri(oldZasekiData, (kaihoRoomNumMan * -1), (kaihoRoomNumWoman * -1), "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM")
        oldCrsInfoEntity.roomZansuThreeRoom.Value = Me.calcRoomZansuAibeyaAri(oldZasekiData, (kaihoRoomNumMan * -1), (kaihoRoomNumWoman * -1), "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM")
        oldCrsInfoEntity.roomZansuFourRoom.Value = Me.calcRoomZansuAibeyaAri(oldZasekiData, (kaihoRoomNumMan * -1), (kaihoRoomNumWoman * -1), "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM")
        oldCrsInfoEntity.roomZansuFiveRoom.Value = Me.calcRoomZansuAibeyaAri(oldZasekiData, (kaihoRoomNumMan * -1), (kaihoRoomNumWoman * -1), "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM")

        ' 部屋残数１人部屋 ～ 部屋残数５人部屋
        oldCrsInfoEntity.roomZansuOneRoom.Value = Me.calcCrsRoomAibeyaNashi(oldZasekiData, (oldRoomList(0) * -1), oldCrsInfoEntity.roomZansuOneRoom.Value, "CRS_BLOCK_ONE_1R")
        oldCrsInfoEntity.roomZansuTwoRoom.Value = Me.calcCrsRoomAibeyaNashi(oldZasekiData, (oldRoomList(1) * -1), oldCrsInfoEntity.roomZansuTwoRoom.Value, "CRS_BLOCK_TWO_1R")
        oldCrsInfoEntity.roomZansuThreeRoom.Value = Me.calcCrsRoomAibeyaNashi(oldZasekiData, (oldRoomList(2) * -1), oldCrsInfoEntity.roomZansuThreeRoom.Value, "CRS_BLOCK_THREE_1R")
        oldCrsInfoEntity.roomZansuFourRoom.Value = Me.calcCrsRoomAibeyaNashi(oldZasekiData, (oldRoomList(3) * -1), oldCrsInfoEntity.roomZansuFourRoom.Value, "CRS_BLOCK_FOUR_1R")
        oldCrsInfoEntity.roomZansuFiveRoom.Value = Me.calcCrsRoomAibeyaNashi(oldZasekiData, (oldRoomList(4) * -1), oldCrsInfoEntity.roomZansuFiveRoom.Value, "CRS_BLOCK_FIVE_1R")

        Dim zasekiKaihoQuery As String = Me.createZasekiKaihoSql(oldCrsInfoEntity)

        Return zasekiKaihoQuery
    End Function

    ''' <summary>
    ''' 部屋残数算出
    ''' </summary>
    ''' <param name="zasekiData">座席情報</param>
    ''' <param name="roomingBetuNinzu">部屋別人数</param>
    ''' <param name="roomZansu">部屋残数</param>
    ''' <param name="crsBlockColName">コースブロックカラム名</param>
    ''' <returns>部屋残数</returns>
    Private Function calcCrsRoomAibeyaNashi(zasekiData As DataTable, roomingBetuNinzu As Integer, roomZansu As Integer?, crsBlockColName As String) As Integer

        Dim crsBlockRoomSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName))
        Dim crsRoomZanSu As Integer = CInt(roomZansu)

        If crsBlockRoomSu = 0 Then
            ' コースブロック管理されていない場合
            Return crsRoomZanSu
        End If

        Dim roomsu As Integer = crsRoomZanSu - roomingBetuNinzu

        Return roomsu
    End Function

    ''' <summary>
    ''' 不要行削除
    ''' （宿泊なし）
    ''' </summary>
    ''' <param name="entity">予約情報（コース料金_料金区分）</param>
    ''' <returns></returns>
    Private Function deleteCrsChargeChargeKbnForShukuhakuNashi(entity As YoyakuInfoCrsChargeChargeKbnEntity) As Boolean

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim sb As New StringBuilder()
            sb.AppendLine("DELETE FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
            sb.AppendLine("WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
            sb.AppendLine("AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_1 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_1 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU = 0 ")

            updateCount = MyBase.execNonQuery(oracleTransaction, sb.ToString())

            ' コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Return False
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 不要行削除
    ''' （宿泊あり）
    ''' </summary>
    ''' <param name="entity">予約情報（コース料金_料金区分）</param>
    ''' <returns></returns>
    Private Function deleteCrsChargeChargeKbnForShukuhakuAri(entity As YoyakuInfoCrsChargeChargeKbnEntity) As Boolean

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim sb As New StringBuilder()
            sb.AppendLine("DELETE FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
            sb.AppendLine("WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
            sb.AppendLine("AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_1 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_2 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_3 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_4 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU_5 = 0 ")
            sb.AppendLine("AND CHARGE_APPLICATION_NINZU = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_1 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_2 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_3 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_4 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU_5 = 0 ")
            sb.AppendLine("AND CANCEL_NINZU = 0 ")

            updateCount = MyBase.execNonQuery(oracleTransaction, sb.ToString())

            ' コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Return False
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' バス座席自動設定処理
    ''' </summary>
    ''' <param name="z0003Param"></param>
    ''' <param name="groupNo">グループNO</param>
    ''' <returns>座席自動配置（予約変更）結果</returns>
    Private Function getCommonZasekiJidoData(z0003Param As Z0003_Param, groupNo As Integer) As Z0001_Result

        Dim param As New Z0001_Param()
        param.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_40
        param.CrsCd = z0003Param.CrsCd
        param.SyuptDay = z0003Param.SyuptDay
        param.Gousya = z0003Param.Gousya
        param.BusReserveCd = z0003Param.BusReserveCd
        param.Ninzu = z0003Param.Ninzu
        param.JyoseiSenyoSeatFlg = z0003Param.JyoseiSenyoSeatFlg
        param.ZasekiReserveKbn = z0003Param.ZasekiReserveKbn
        param.YoyakuKbn = z0003Param.YoyakuKbn
        param.YoyakuNo = z0003Param.YoyakuNO
        param.GroupNo = groupNo

        Dim z0001 As New Z0001()
        Dim result As Z0001_Result = z0001.Execute(param)

        Return result
    End Function

    ''' <summary>
    ''' 予約ピックアップ情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約ピックアップ情報取得SQL</returns>
    Private Function createYoyakuPickupInfoSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YIP.YOYAKU_KBN ")
        sb.AppendLine("     ,YIP.YOYAKU_NO ")
        sb.AppendLine("     ,YIP.PICKUP_HOTEL_CD ")
        sb.AppendLine("     ,YIP.PICKUP_ROUTE_CD ")
        sb.AppendLine("     ,PRL.CRS_JYOSYA_TI ")
        sb.AppendLine("     ,RLH.SYUPT_TIME ")
        sb.AppendLine("     ,YIP.NINZU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_PICKUP YIP ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_PICKUP_HOTEL MPH ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     MPH.PICKUP_HOTEL_CD = YIP.PICKUP_HOTEL_CD ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER PRL ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     PRL.PICKUP_ROUTE_CD = YIP.PICKUP_ROUTE_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     PRL.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER_HOTEL RLH ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     RLH.PICKUP_ROUTE_CD = YIP.PICKUP_ROUTE_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     RLH.PICKUP_HOTEL_CD = YIP.PICKUP_HOTEL_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     RLH.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIP.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIP.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function


    ''' <summary>
    ''' ピックアップルート台帳 （ホテル）人数更新SQL作成
    ''' </summary>
    ''' <param name="entity">ピックアップルート台帳 （ホテル）Entity</param>
    ''' <returns>ピックアップルート台帳 （ホテル）人数更新SQL</returns>
    Private Function createPickupRouteLedgerHotelUpdateSql(entity As PickupRouteLedgerHotelEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_PICKUP_ROUTE_LEDGER_HOTEL ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      NINZU = NINZU + " + MyBase.setParam(entity.ninzu.PhysicsName, entity.ninzu.Value, entity.ninzu.DBType, entity.ninzu.IntegerBu, entity.ninzu.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     PICKUP_ROUTE_CD = " + MyBase.setParam(entity.pickupRouteCd.PhysicsName, entity.pickupRouteCd.Value, entity.pickupRouteCd.DBType, entity.pickupRouteCd.IntegerBu, entity.pickupRouteCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     PICKUP_HOTEL_CD = " + MyBase.setParam(entity.pickupHotelCd.PhysicsName, entity.pickupHotelCd.Value, entity.pickupHotelCd.DBType, entity.pickupHotelCd.IntegerBu, entity.pickupHotelCd.DecimalBu))

        Return sb.ToString()
    End Function

#End Region

End Class
