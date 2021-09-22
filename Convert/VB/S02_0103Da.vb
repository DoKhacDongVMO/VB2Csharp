Imports System.Reflection
Imports System.Text
Imports Hatobus.ReservationManagementSystem.Zaseki

''' <summary>
''' 予約登録DA
''' </summary>
Public Class S02_0103Da
    Inherits DataAccessorBase

#Region "定数/変数"

    ''' <summary>
    ''' 使用中チェックリトライ回数：100
    ''' </summary>
    Private Const UsingCheckRetryNum As Integer = 100

#End Region

#Region "メソッド"

    ''' <summary>
    ''' コード分類の内容を取得
    ''' </summary>
    ''' <param name="codeBunrui">コード分類</param>
    ''' <param name="nullRecord">空行レコード挿入フラグ</param>
    ''' <returns></returns>
    Public Function getCodeMasterData(codeBunrui As String, Optional nullRecord As Boolean = False) As DataTable

        MyBase.paramClear()

        Dim codeMasterData As DataTable = Nothing

        Try

            Dim query As String = Me.createCodeMasterSql(codeBunrui, nullRecord)
            codeMasterData = MyBase.getDataTable(query)

        Catch ex As Exception
            Throw
        End Try

        Return codeMasterData
    End Function

    ''' <summary>
    ''' コース情報取得
    ''' </summary>
    ''' <param name="entity">パラメータ</param>
    ''' <returns>コース情報</returns>
    Public Function getCrsLedgerBasicData(entity As CrsLedgerBasicEntity) As DataTable

        Dim crsInfo As DataTable = Nothing

        Try
            ' コース台帳(基本)SQL文作成
            Dim query As String = Me.createCrsLedgerBasicSql(entity)
            crsInfo = MyBase.getDataTable(query)
        Catch ex As Exception
            Throw
        End Try

        Return crsInfo
    End Function

    ''' <summary>
    ''' コース台帳(基本)の使用中フラグ取得
    ''' </summary>
    ''' <param name="paramInfoList">パラメータ群</param>
    ''' <returns>コース台帳(基本)の使用中フラグ</returns>
    Public Function getCrsLedgerBasicUsingData(paramInfoList As Hashtable) As DataTable

        Dim usingData As DataTable = Nothing

        Try

            Dim query As String = Me.createUsingFlagSql(paramInfoList)
            usingData = MyBase.getDataTable(query)

        Catch ex As Exception
            Throw
        End Try

        Return usingData
    End Function

    ''' <summary>
    ''' コース台帳座席更新処理
    ''' 定期用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <returns>座席変更ステータス</returns>
    Public Function updateZasekiInfoForTeiki(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim crsLedgerZasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                Dim kusekiTeisaki As Integer = 0
                Dim kusekiSubSeat As Integer = 0

                zasekiUpdateQuery = Me.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then
                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)
                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 仮予約情報登録
            Dim yoyakuInfoDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_BASIC")
            MyBase.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery)

            Dim yoyakuInfoQuery As String = Me.createInsertSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC")

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳取消座席更新処理
    ''' 定期用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <returns>座席変更ステータス</returns>
    Public Function updateTorikeshiZasekiInfoForTeiki(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim crsLedgerZasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                Dim kusekiTeisaki As Integer = 0
                Dim kusekiSubSeat As Integer = 0

                zasekiUpdateQuery = Me.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat)

                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then
                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)
                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 仮予約情報論削
            Dim yoyakuInfoQuery As String = Me.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳座席更新処理
    ''' 企画(日帰り)用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <returns></returns>
    Public Function updateZasekiInfoForKikakuHigaeri(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 仮予約情報登録
            Dim yoyakuInfoDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_BASIC")
            MyBase.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery)

            Dim yoyakuInfoQuery As String = Me.createInsertSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC")

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳取消座席更新処理
    ''' 企画(日帰り)用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <returns></returns>
    Public Function updateTorikeshiZasekiInfoForKikakuHigaeri(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 仮予約情報論削
            Dim yoyakuInfoQuery As String = Me.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳座席更新処理
    ''' 企画(宿泊)用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <param name="crsInfoBasicEntity">コース台帳（Entity）</param>
    ''' <returns>更新ステータス</returns>
    Public Function updateZasekiInfoForKikakuShukuhaku(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity, crsInfoBasicEntity As CrsLedgerBasicEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 部屋数更新
            Dim roomZansuUpdateQuery As String = Me.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, False)
            If String.IsNullOrEmpty(roomZansuUpdateQuery) = False Then

                updateCount = MyBase.execNonQuery(oracleTransaction, roomZansuUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 仮予約情報登録
            Dim yoyakuInfoDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_BASIC")
            MyBase.execNonQuery(oracleTransaction, yoyakuInfoDeleteQuery)

            Dim yoyakuInfoQuery As String = Me.createInsertSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, "T_YOYAKU_INFO_BASIC")

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' コース台帳取消座席更新処理
    ''' 企画(宿泊)用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <param name="crsInfoBasicEntity">コース台帳（Entity）</param>
    ''' <returns>更新ステータス</returns>
    Public Function updateTorikeshiZasekiInfoForKikakuShukuhaku(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity, crsInfoBasicEntity As CrsLedgerBasicEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 部屋数更新
            Dim roomZansuUpdateQuery As String = Me.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, True)
            updateCount = MyBase.execNonQuery(oracleTransaction, roomZansuUpdateQuery)

            If updateCount <= 0 Then
                ' 座席データの更新件数が0件以下の場合、処理終了

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
            End If

            ' ROOM別人数1～5を初期化
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu1.Value = CommonRegistYoyaku.ZERO
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu2.Value = CommonRegistYoyaku.ZERO
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu3.Value = CommonRegistYoyaku.ZERO
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu4.Value = CommonRegistYoyaku.ZERO
            registEntity.YoyakuInfoBasicEntity.roomingBetuNinzu5.Value = CommonRegistYoyaku.ZERO

            ' 仮予約情報論削
            Dim yoyakuInfoQuery As String = Me.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
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
    ''' 料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>料金区分一覧</returns>
    Public Function getChargeKbnList(entity As CrsLedgerChargeEntity) As DataTable

        Dim chargeKbnList As DataTable

        Try
            Dim query As String = Me.createChargeKbnListSql(entity)
            chargeKbnList = MyBase.getDataTable(query)
        Catch ex As Exception
            Throw
        End Try

        Return chargeKbnList
    End Function

    ''' <summary>
    ''' コース台帳（コース情報）取得
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>コース台帳（コース情報）</returns>
    Public Function getCrsInfo(entity As CrsLedgerCrsInfoEntity) As DataTable

        Dim crsInfo As DataTable = Nothing

        Try

            Dim query As String = Me.createCrsInfoSql(entity)
            crsInfo = MyBase.getDataTable(query)

        Catch ex As Exception
            Throw
        End Try

        Return crsInfo
    End Function

    ''' <summary>
    ''' メッセージ情報取得
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>メッセージ情報</returns>
    Public Function getCrsInfoMessage(entity As CrsLedgerMessageEntity) As DataTable

        Dim messageData As DataTable = Nothing

        Try

            Dim query As String = Me.createCrsInfoMessageSql(entity)
            messageData = MyBase.getDataTable(query)

        Catch ex As Exception
            Throw
        End Try

        Return messageData
    End Function

    ''' <summary>
    ''' 予約区分、予約番号取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約区分、予約番号</returns>
    Public Function getYoyakuIno(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuData As DataTable = Nothing

        Try

            Dim query As String = Me.createYoyakuInfoSql(entity)
            yoyakuData = MyBase.getDataTable(query)

        Catch ex As Exception

            Throw
        End Try

        Return yoyakuData
    End Function

    ''' <summary>
    ''' 割引マスタ情報取得
    ''' </summary>
    ''' <param name="entity">割引マスタ</param>
    ''' <returns>割引マスタ情報</returns>
    Public Function getWaribikiMasterData(entity As WaribikiCdMasterEntity) As DataTable

        Dim waribikiData As DataTable = Nothing

        Try

            Dim query As String = Me.createWaribikiMasterDataSql(entity)
            waribikiData = MyBase.getDataTable(query)

        Catch ex As Exception

            Throw
        End Try

        Return waribikiData
    End Function

    ''' <summary>
    ''' オプション必須選択一覧取得
    ''' </summary>
    ''' <param name="entity">コース台帳（オプショングループ）Entity</param>
    ''' <returns>オプション必須選択一覧</returns>
    Public Function getRequiredSelectionList(entity As CrsLedgerOptionGroupEntity) As DataTable

        MyBase.paramClear()

        Dim requiredSelectionList As DataTable = Nothing

        Try
            Dim query As String = Me.createRequiredSelectionListSql(entity)
            requiredSelectionList = Me.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return requiredSelectionList
    End Function

    ''' <summary>
    ''' オプション任意選択一覧取得
    ''' </summary>
    ''' <param name="entity">コース台帳（オプショングループ）Entity</param>
    ''' <returns>オプション任意選択一覧</returns>
    Public Function getAnySelectionList(entity As CrsLedgerOptionGroupEntity) As DataTable

        MyBase.paramClear()

        Dim anySelectionList As DataTable = Nothing

        Try

            Dim query As String = Me.createAnySelectionListSql(entity)
            anySelectionList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return anySelectionList
    End Function

    ''' <summary>
    ''' 送付物マスタ一覧取得
    ''' </summary>
    ''' <returns>送付物マスタ一覧</returns>
    Public Function getSoufubutsuList() As DataTable

        Dim soufubutsuList As DataTable = Nothing

        Try

            Dim query As String = Me.createSoufubutsuListSql()
            soufubutsuList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return soufubutsuList
    End Function

    ''' <summary>
    ''' 予約情報登録
    ''' </summary>
    ''' <param name="registEntity">登録用Entity</param>
    ''' <returns>処理結果</returns>
    Public Function registYoyakuInfo(registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 予約情報（基本）更新
            Dim basicQuery As String = Me.createYoyakuInfoBasicUpdateSql(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            updateCount = MyBase.execNonQuery(oracleTransaction, basicQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            ' 予約情報（コース料金）追加
            Dim crsChargeQuery As String = Me.createInsertSql(Of YoyakuInfoCrsChargeEntity)(registEntity.YoyakuInfoCrsChargeEntity, "T_YOYAKU_INFO_CRS_CHARGE")
            updateCount = MyBase.execNonQuery(oracleTransaction, crsChargeQuery)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure
            End If

            ' 予約情報（コース料金_料金区分）
            Dim chargeKbnQuery As String = ""
            For Each entity As YoyakuInfoCrsChargeChargeKbnEntity In registEntity.YoyakuInfoCrsChargeChargeKbnList

                chargeKbnQuery = Me.createInsertSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
                updateCount = MyBase.execNonQuery(oracleTransaction, chargeKbnQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure
                End If
            Next

            ' 予約情報（ピックアップ）
            If registEntity.YoyakuInfoPickupList IsNot Nothing Then

                Dim pickupDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_PICKUP")
                MyBase.execNonQuery(oracleTransaction, pickupDeleteQuery)

                Dim pickupInsertQuery As String = ""
                For Each entity As YoyakuInfoPickupEntity In registEntity.YoyakuInfoPickupList

                    pickupInsertQuery = Me.createInsertSql(Of YoyakuInfoPickupEntity)(entity, "T_YOYAKU_INFO_PICKUP")
                    updateCount = MyBase.execNonQuery(oracleTransaction, pickupInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuPickupInfoFailure
                    End If
                Next
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

            ' 予約情報（割引）
            If registEntity.YoyakuInfoWaribikiList IsNot Nothing Then

                Dim waribikiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_WARIBIKI")
                MyBase.execNonQuery(oracleTransaction, waribikiDeleteQuery)

                Dim waribikiInsertQuery As String = ""
                For Each entity As YoyakuInfoWaribikiEntity In registEntity.YoyakuInfoWaribikiList

                    waribikiInsertQuery = Me.createInsertSql(Of YoyakuInfoWaribikiEntity)(entity, "T_YOYAKU_INFO_WARIBIKI")
                    updateCount = MyBase.execNonQuery(oracleTransaction, waribikiInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuWaribikiUpdateFailure
                    End If
                Next
            End If

            '予約情報（振込）
            If registEntity.YoyakuInfoHurikomiEntity IsNot Nothing Then

                Dim hurikomiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_HURIKOMI")
                MyBase.execNonQuery(oracleTransaction, hurikomiDeleteQuery)

                Dim hurikomiInsertQuery = Me.createInsertSql(Of YoyakuInfoHurikomiEntity)(registEntity.YoyakuInfoHurikomiEntity, "T_YOYAKU_INFO_HURIKOMI")
                updateCount = MyBase.execNonQuery(oracleTransaction, hurikomiInsertQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuFurikomiUpdateFailure
                End If
            End If

            ' 予約情報（名簿）
            If registEntity.YoyakuInfoMeiboList IsNot Nothing Then

                Dim meiboDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                    registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                    "T_YOYAKU_INFO_MEIBO")
                MyBase.execNonQuery(oracleTransaction, meiboDeleteQuery)

                Dim meiboInsertQuery As String = ""
                For Each entity As YoyakuInfoMeiboEntity In registEntity.YoyakuInfoMeiboList

                    meiboInsertQuery = Me.createInsertSql(Of YoyakuInfoMeiboEntity)(entity, "T_YOYAKU_INFO_MEIBO")
                    updateCount = MyBase.execNonQuery(oracleTransaction, meiboInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuMeiboUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（送付物）
            If registEntity.YoyakuInfoSofubutsuList IsNot Nothing Then

                Dim sofubutsuDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                        registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                        "T_YOYAKU_INFO_SOFUBUTSU")
                MyBase.execNonQuery(oracleTransaction, sofubutsuDeleteQuery)

                Dim sofubutsuInsertQuery As String = ""
                For Each entity As YoyakuInfoSofubutsuEntity In registEntity.YoyakuInfoSofubutsuList

                    sofubutsuInsertQuery = Me.createInsertSql(Of YoyakuInfoSofubutsuEntity)(entity, "T_YOYAKU_INFO_SOFUBUTSU")
                    updateCount = MyBase.execNonQuery(oracleTransaction, sofubutsuInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuSofubutsuUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（オプション）
            If registEntity.YoyakuInfoOptionList IsNot Nothing Then

                Dim optionDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_OPTION")
                MyBase.execNonQuery(oracleTransaction, optionDeleteQuery)

                Dim optionInsertQuery As String = ""
                For Each entity As YoyakuInfoOptionEntity In registEntity.YoyakuInfoOptionList

                    optionInsertQuery = Me.createInsertSql(Of YoyakuInfoOptionEntity)(entity, "T_YOYAKU_INFO_OPTION")
                    updateCount = MyBase.execNonQuery(oracleTransaction, optionInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuOptionUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（送付先）
            If registEntity.YoyakuInfoSofusakiList IsNot Nothing Then

                Dim sofusakiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_SOFUSAKI")
                MyBase.execNonQuery(oracleTransaction, sofusakiDeleteQuery)

                Dim sofusakiInsertQuery As String = ""
                For Each entity As YoyakuInfoSofusakiEntity In registEntity.YoyakuInfoSofusakiList

                    sofusakiInsertQuery = Me.createInsertSql(Of YoyakuInfoSofusakiEntity)(entity, "T_YOYAKU_INFO_SOFUSAKI")
                    updateCount = MyBase.execNonQuery(oracleTransaction, sofusakiInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuSofusakiUpdateFailure
                    End If
                Next
            End If

            ' WT_リクエスト情報更新
            If registEntity.WtRequestInfoEntity IsNot Nothing Then

                Dim wtRequestUpdateQuery As String = Me.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity)

                updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestUpdateQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure
                End If

                ' コース台帳（基本）（WTキャンセル人数更新）
                Dim wtCancelNinzuQuery As String = Me.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction)
                updateCount = MyBase.execNonQuery(oracleTransaction, wtCancelNinzuQuery)

            End If

            If updateCount = 0 Then

                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約情報更新
    ''' </summary>
    ''' <param name="registEntity">登録用Entity</param>
    ''' <returns>処理結果</returns>
    Public Function updateYoyakuInfo(registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

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

            ' 予約情報（基本）
            Dim yoyakuInfoQuery As String = Me.createUpdateSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            Dim yoyakuInfoBuilder As New StringBuilder()
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery)
            yoyakuInfoBuilder.AppendLine("WHERE")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu))
            yoyakuInfoBuilder.AppendLine("    AND ")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            ' 予約情報２
            Dim yoyakuInfo2Query As String

            Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
            Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

            If yoyakuInfo2.Rows.Count > 0 Then

                Dim yoyakuInfo2UpdateQuery As String = Me.createUpdateSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2")
                Dim yoyakuInfo2Builder As New StringBuilder()
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery)
                yoyakuInfo2Builder.AppendLine("WHERE")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu))
                yoyakuInfo2Builder.AppendLine("    AND ")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu))

                yoyakuInfo2Query = yoyakuInfo2Builder.ToString()
            Else

                yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
            End If

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure
            End If

            ' 予約情報（ピックアップ）
            If registEntity.YoyakuInfoPickupList IsNot Nothing Then

                Dim pickupDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_PICKUP")
                MyBase.execNonQuery(oracleTransaction, pickupDeleteQuery)

                Dim pickupInsertQuery As String = ""
                For Each entity As YoyakuInfoPickupEntity In registEntity.YoyakuInfoPickupList

                    pickupInsertQuery = Me.createInsertSql(Of YoyakuInfoPickupEntity)(entity, "T_YOYAKU_INFO_PICKUP")
                    updateCount = MyBase.execNonQuery(oracleTransaction, pickupInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuPickupInfoFailure
                    End If
                Next
            End If

            ' ピックアップルート台帳 （ホテル）
            If registEntity.PickupRouteLedgerHotelList IsNot Nothing Then

                For Each entity As PickupRouteLedgerHotelEntity In registEntity.PickupRouteLedgerHotelList

                    If entity.ninzu.Value <> CommonRegistYoyaku.ZERO Then '2019/06/05 今回人数と前回人数に相違がある場合に更新
                        ' ピックアップルート台帳 （ホテル）人数更新SQL作成
                        Dim pickupHotelQuery As String = Me.createPickupRouteLedgerHotelUpdateSql(entity)
                        updateCount = MyBase.execNonQuery(oracleTransaction, pickupHotelQuery)

                        If updateCount <= 0 Then

                            ' ロールバック
                            MyBase.callRollbackTransaction(oracleTransaction)
                            Return CommonRegistYoyaku.UpdateStatusPickupRouteLedgerHotelFailure
                        End If
                    End If
                Next
            End If

            ' 予約情報（割引）
            If registEntity.YoyakuInfoWaribikiList IsNot Nothing Then

                Dim waribikiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_WARIBIKI")
                MyBase.execNonQuery(oracleTransaction, waribikiDeleteQuery)

                Dim waribikiInsertQuery As String = ""
                For Each entity As YoyakuInfoWaribikiEntity In registEntity.YoyakuInfoWaribikiList

                    waribikiInsertQuery = Me.createInsertSql(Of YoyakuInfoWaribikiEntity)(entity, "T_YOYAKU_INFO_WARIBIKI")
                    updateCount = MyBase.execNonQuery(oracleTransaction, waribikiInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuWaribikiUpdateFailure
                    End If
                Next
            End If

            '予約情報（振込）
            If registEntity.YoyakuInfoHurikomiEntity IsNot Nothing Then

                Dim hurikomiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_HURIKOMI")
                MyBase.execNonQuery(oracleTransaction, hurikomiDeleteQuery)

                Dim hurikomiInsertQuery = Me.createInsertSql(Of YoyakuInfoHurikomiEntity)(registEntity.YoyakuInfoHurikomiEntity, "T_YOYAKU_INFO_HURIKOMI")
                updateCount = MyBase.execNonQuery(oracleTransaction, hurikomiInsertQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuFurikomiUpdateFailure
                End If
            End If

            ' 予約情報（名簿）
            If registEntity.YoyakuInfoMeiboList IsNot Nothing Then

                Dim meiboDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                    registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                    "T_YOYAKU_INFO_MEIBO")
                MyBase.execNonQuery(oracleTransaction, meiboDeleteQuery)

                Dim meiboInsertQuery As String = ""
                For Each entity As YoyakuInfoMeiboEntity In registEntity.YoyakuInfoMeiboList

                    meiboInsertQuery = Me.createInsertSql(Of YoyakuInfoMeiboEntity)(entity, "T_YOYAKU_INFO_MEIBO")
                    updateCount = MyBase.execNonQuery(oracleTransaction, meiboInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuMeiboUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（送付物）
            If registEntity.YoyakuInfoSofubutsuList IsNot Nothing Then

                Dim sofubutsuDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                        registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                        "T_YOYAKU_INFO_SOFUBUTSU")
                MyBase.execNonQuery(oracleTransaction, sofubutsuDeleteQuery)

                Dim sofubutsuInsertQuery As String = ""
                For Each entity As YoyakuInfoSofubutsuEntity In registEntity.YoyakuInfoSofubutsuList

                    sofubutsuInsertQuery = Me.createInsertSql(Of YoyakuInfoSofubutsuEntity)(entity, "T_YOYAKU_INFO_SOFUBUTSU")
                    updateCount = MyBase.execNonQuery(oracleTransaction, sofubutsuInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuSofubutsuUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（オプション）
            If registEntity.YoyakuInfoOptionList IsNot Nothing Then

                Dim optionDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                     registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                     "T_YOYAKU_INFO_OPTION")
                MyBase.execNonQuery(oracleTransaction, optionDeleteQuery)

                Dim optionInsertQuery As String = ""
                For Each entity As YoyakuInfoOptionEntity In registEntity.YoyakuInfoOptionList

                    optionInsertQuery = Me.createInsertSql(Of YoyakuInfoOptionEntity)(entity, "T_YOYAKU_INFO_OPTION")
                    updateCount = MyBase.execNonQuery(oracleTransaction, optionInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuOptionUpdateFailure
                    End If
                Next
            End If

            ' 予約情報（送付先）
            If registEntity.YoyakuInfoSofusakiList IsNot Nothing Then

                Dim sofusakiDeleteQuery As String = Me.createDeleteSql(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value,
                                                                       registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value,
                                                                       "T_YOYAKU_INFO_SOFUSAKI")
                MyBase.execNonQuery(oracleTransaction, sofusakiDeleteQuery)

                Dim sofusakiInsertQuery As String = ""
                For Each entity As YoyakuInfoSofusakiEntity In registEntity.YoyakuInfoSofusakiList

                    sofusakiInsertQuery = Me.createInsertSql(Of YoyakuInfoSofusakiEntity)(entity, "T_YOYAKU_INFO_SOFUSAKI")
                    updateCount = MyBase.execNonQuery(oracleTransaction, sofusakiInsertQuery)

                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusYoyakuSofusakiUpdateFailure
                    End If
                Next
            End If

            ' WT_リクエスト情報更新
            If registEntity.WtRequestInfoEntity IsNot Nothing Then

                Dim wtRequestUpdateQuery As String = Me.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity)

                updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestUpdateQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure
                End If

                ' コース台帳（基本）（WTキャンセル人数更新）
                Dim wtCancelNinzuQuery As String = Me.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction)
                updateCount = MyBase.execNonQuery(oracleTransaction, wtCancelNinzuQuery)

            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約情報データ取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報データ</returns>
    Public Function getYoyakuInfoData(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuInfoData As DataTable

        Try

            Dim query As String = Me.createYoyakuInfoDataSql(entity)
            yoyakuInfoData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuInfoData
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
    ''' 予約メモ情報一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（メモ）Entity</param>
    ''' <returns>予約メモ情報一覧</returns>
    Public Function getYoyakuMemoInfoList(entity As YoyakuInfoMemoEntity) As DataTable

        Dim memoInfoList As DataTable

        Try

            Dim query As String = Me.createYoyakuMemoInfoListSql(entity)
            memoInfoList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return memoInfoList
    End Function

    ''' <summary>
    ''' 予約割引一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（割引）Entity</param>
    ''' <returns>予約割引一覧</returns>
    Public Function getYoyakuWaribikiList(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuWaribikiList As DataTable

        Try

            Dim query As String = Me.createYoyakuWaribikiListSql(entity)
            yoyakuWaribikiList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuWaribikiList
    End Function

    ''' <summary>
    ''' 予約振込情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（振込）Entity</param>
    ''' <returns>予約振込情報</returns>
    Public Function getYoyakuHurikomiInfo(entity As YoyakuInfoHurikomiEntity) As DataTable

        Dim yoyakuFurikomiInfo As DataTable

        Try

            Dim query As String = Me.createYoyakuHurikomiInfoSql(entity)
            yoyakuFurikomiInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuFurikomiInfo
    End Function

    ''' <summary>
    ''' 予約名簿一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（名簿）Entity</param>
    ''' <returns>予約名簿一覧</returns>
    Public Function getYoyakuMeiboList(entity As YoyakuInfoMeiboEntity) As DataTable

        Dim yoyakuMeiboList As DataTable

        Try
            Dim query As String = Me.createYoyakuMeiboListSql(entity)
            yoyakuMeiboList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuMeiboList
    End Function

    ''' <summary>
    ''' 予約送付物一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（送付物）Entity</param>
    ''' <returns>予約送付物一覧</returns>
    Public Function getYoyakuSofubutsuList(entity As YoyakuInfoSofubutsuEntity) As DataTable

        Dim yoyakuSofubutsuList As DataTable

        Try
            Dim query As String = Me.createYoyakuSofubutsuListSql(entity)
            yoyakuSofubutsuList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuSofubutsuList
    End Function

    ''' <summary>
    ''' 予約送付先一覧取得
    ''' </summary>
    ''' <param name="entity">予約情報（送付先）Entity</param>
    ''' <returns>予約送付先一覧</returns>
    Public Function getYoyakuSofusakiList(entity As YoyakuInfoSofusakiEntity) As DataTable

        Dim yoyakuSofusakiList As DataTable

        Try
            Dim query As String = Me.createYoyakuSofusakiListSql(entity)
            yoyakuSofusakiList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuSofusakiList
    End Function

    ''' <summary>
    ''' 入返金一覧取得
    ''' </summary>
    ''' <param name="entity">入返金情報Entity</param>
    ''' <returns>入返金一覧</returns>
    Public Function getNyuhenkinList(entity As NyuukinInfoEntity) As DataTable

        Dim nyuhenkinList As DataTable

        Try
            Dim query As String = Me.createNyuhenkinListSql(entity)
            nyuhenkinList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return nyuhenkinList
    End Function

    ''' <summary>
    ''' 予約オプション一覧取得
    ''' </summary>
    ''' <param name="crsEntity">コース台帳（オプション）Entity</param>
    ''' <param name="yoyakuentity">予約情報（オプション）Entity</param>
    ''' <returns>予約オプション一覧</returns>
    Public Function getYoyakuOptionList(crsEntity As CrsLedgerOptionGroupEntity, yoyakuentity As YoyakuInfoOptionEntity) As DataTable

        MyBase.paramClear()

        Dim yoyakuOptionList As DataTable

        Try
            Dim query As String = Me.createYoyakuOptionListSql(crsEntity, yoyakuentity)
            yoyakuOptionList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuOptionList
    End Function

    ''' <summary>
    ''' ホリデーT管理（コースコード）情報取得
    ''' </summary>
    ''' <param name="entity">ホリデーＴ管理（コースコード）Entity</param>
    ''' <returns>ホリデーT管理（コースコード）情報</returns>
    Public Function getHolidayCrs(entity As HolidayCrsCdEntity) As DataTable

        Dim holidayCrsInfo As DataTable

        Try
            Dim query As String = Me.createHolidayCrsSql(entity)
            holidayCrsInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return holidayCrsInfo
    End Function

    ''' <summary>
    ''' ホリデーT管理（適用日）情報取得
    ''' </summary>
    ''' <param name="entity">ホリデーＴ管理（適用日）Entity</param>
    ''' <returns>ホリデーT管理（適用日）情報</returns>
    Public Function getHolidayApplicationDay(entity As HolidayApplicationDayEntity) As DataTable

        Dim holidayApplicationDayInfo As DataTable

        Try
            Dim query As String = Me.createHolidayApplicationDaySql(entity)
            holidayApplicationDayInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return holidayApplicationDayInfo
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
    ''' 予約情報使用中フラグ更新
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>更新件数</returns>
    Public Function updateYoyakuUsingFlag(entity As YoyakuInfoBasicEntity) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim query As String = Me.createYoyakuUsingFlagUpdateSql(entity)
            updateCount = MyBase.execNonQuery(oracleTransaction, query)

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return updateCount
    End Function

    ''' <summary>
    ''' 予約情報使用中フラグ更新
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>更新件数</returns>
    Public Function updateYoyakuUsingFlagOn(entity As YoyakuInfoBasicEntity) As Integer

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim query As String = Me.createYoyakuUsingFlagOnUpdateSql(entity)
            updateCount = MyBase.execNonQuery(oracleTransaction, query)

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return updateCount
    End Function

    ''' <summary>
    ''' 予約情報使用中フラグ取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報使用中フラグ</returns>
    Public Function getYoyakuInfoUsingFlag(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuInfo As DataTable

        Try
            Dim query As String = Me.createYoyakuUsingFlagSql(entity)
            yoyakuInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuInfo
    End Function

    ''' <summary>
    ''' 予約座席情報キャンセル
    ''' 定期用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">座席情報検索条件群</param>
    ''' <param name="registEntity">予約登録データ</param>
    ''' <returns>処理結果</returns>
    Public Function cancelYoyakuInfoForTeiseki(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 座席情報取得
            Dim crsLedgerZasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, crsLedgerZasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                Dim kusekiTeisaki As Integer = 0
                Dim kusekiSubSeat As Integer = 0

                ' コース台帳座席情報更新SQL作成
                zasekiUpdateQuery = Me.createCrsZasekiDataForTeiki(z0001Result, crsLedgerZasekiData, crsZasekiData, kusekiTeisaki, kusekiSubSeat)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then
                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)
                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedCrsZasekiUpdateSqlForTeiseki(row, crsZasekiData, kusekiTeisaki, kusekiSubSeat)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(crsLedgerZasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
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

            ' 予約情報（基本）
            Dim yoyakuInfoQuery As String = Me.createUpdateSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            Dim yoyakuInfoBuilder As New StringBuilder()
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery)
            yoyakuInfoBuilder.AppendLine("WHERE")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu))
            yoyakuInfoBuilder.AppendLine("    AND ")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            ' 予約情報（コース料金）
            Dim crsChargeQuery As String = Me.createUpdateSql(Of YoyakuInfoCrsChargeEntity)(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE")
            Dim crsChargeBuilder As New StringBuilder()
            crsChargeBuilder.AppendLine(crsChargeQuery)
            crsChargeBuilder.AppendLine("WHERE")
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu))
            crsChargeBuilder.AppendLine("    AND ")
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, crsChargeBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure
            End If

            ' 予約情報（コース料金_料金区分）
            Dim chargeKbnQuery As String = ""
            For Each entity As YoyakuInfoCrsChargeChargeKbnEntity In registEntity.YoyakuInfoCrsChargeChargeKbnList

                chargeKbnQuery = Me.createUpdateSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
                Dim chargeKbnBuilder As New StringBuilder()
                chargeKbnBuilder.AppendLine(chargeKbnQuery)
                chargeKbnBuilder.AppendLine("WHERE")
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    KBN_NO = " + MyBase.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + MyBase.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu))

                updateCount = MyBase.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString())

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure
                End If
            Next

            ' 予約情報２
            Dim yoyakuInfo2Query As String

            Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
            Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

            If yoyakuInfo2.Rows.Count > 0 Then

                Dim yoyakuInfo2UpdateQuery As String = Me.createUpdateSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2")
                Dim yoyakuInfo2Builder As New StringBuilder()
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery)
                yoyakuInfo2Builder.AppendLine("WHERE")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu))
                yoyakuInfo2Builder.AppendLine("    AND ")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu))

                yoyakuInfo2Query = yoyakuInfo2Builder.ToString()
            Else

                yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
            End If

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure
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

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約座席情報キャンセル
    ''' 企画（日帰り）用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">座席情報検索条件群</param>
    ''' <param name="registEntity">予約登録データ</param>
    ''' <returns>処理結果</returns>
    Public Function cancelYoyakuInfoForKikakuHigaeri(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
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

            ' 予約情報（基本）
            Dim yoyakuInfoQuery As String = Me.createUpdateSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            Dim yoyakuInfoBuilder As New StringBuilder()
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery)
            yoyakuInfoBuilder.AppendLine("WHERE")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu))
            yoyakuInfoBuilder.AppendLine("    AND ")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            ' 予約情報（コース料金）
            Dim crsChargeQuery As String = Me.createUpdateSql(Of YoyakuInfoCrsChargeEntity)(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE")
            Dim crsChargeBuilder As New StringBuilder()
            crsChargeBuilder.AppendLine(crsChargeQuery)
            crsChargeBuilder.AppendLine("WHERE")
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu))
            crsChargeBuilder.AppendLine("    AND ")
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, crsChargeBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure
            End If

            ' 予約情報（コース料金_料金区分）
            Dim chargeKbnQuery As String = ""
            For Each entity As YoyakuInfoCrsChargeChargeKbnEntity In registEntity.YoyakuInfoCrsChargeChargeKbnList

                chargeKbnQuery = Me.createUpdateSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
                Dim chargeKbnBuilder As New StringBuilder()
                chargeKbnBuilder.AppendLine(chargeKbnQuery)
                chargeKbnBuilder.AppendLine("WHERE")
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    KBN_NO = " + MyBase.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + MyBase.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu))

                updateCount = MyBase.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString())

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure
                End If
            Next

            ' 予約情報２
            Dim yoyakuInfo2Query As String

            Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
            Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

            If yoyakuInfo2.Rows.Count > 0 Then

                Dim yoyakuInfo2UpdateQuery As String = Me.createUpdateSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2")
                Dim yoyakuInfo2Builder As New StringBuilder()
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery)
                yoyakuInfo2Builder.AppendLine("WHERE")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu))
                yoyakuInfo2Builder.AppendLine("    AND ")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu))

                yoyakuInfo2Query = yoyakuInfo2Builder.ToString()
            Else

                yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
            End If

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure
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

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' 予約座席情報キャンセル
    ''' 企画（宿泊）用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」データ</param>
    ''' <param name="crsZasekiData">座席情報検索条件群</param>
    ''' <param name="registEntity">予約登録データ</param>
    ''' <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    ''' <returns>処理結果</returns>
    Public Function cancelYoyakuInfoForKikakuShukuhaku(z0001Result As Z0001_Result, crsZasekiData As Hashtable, registEntity As RegistYoyakuInfoEntity, crsInfoBasicEntity As CrsLedgerBasicEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim zasekiData As DataTable = Nothing
            Dim sharedZasekiData As DataTable = Nothing

            ' 自コース、共用コースの使用中チェック
            If Me.isCrsLenderZasekiDataForKikaku(crsZasekiData, oracleTransaction, zasekiData, sharedZasekiData) = False Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusUsing
            End If

            Dim zasekiUpdateQuery As String = ""
            ' コース台帳座席数更新
            If z0001Result.Status = Z0001_Result.Z0001_Result_Status.OK Then
                ' 共通処理「バス座席自動設定処理」.ステータスが「00」の場合
                zasekiUpdateQuery = Me.createCrsZasekiDataForKikaku(z0001Result, zasekiData, crsZasekiData)
                If String.IsNullOrEmpty(zasekiUpdateQuery) = True Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusKusekiNothing
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If

                Dim sharedBusCrsQuery As String = ""
                ' 共用コース座席更新
                For Each row As DataRow In sharedZasekiData.Rows

                    If Me.isSharedBusCrsEqualCheck(row, crsZasekiData) = False Then
                        ' 自コースと共用コースのコースコード、号車、出発日が同一の場合、次レコードへ
                        Continue For
                    End If

                    ' 共用コース座席更新SQL作成
                    sharedBusCrsQuery = Me.createSharedBusCrsZasekiUpdateSql(z0001Result, row, crsZasekiData)
                    updateCount = MyBase.execNonQuery(oracleTransaction, sharedBusCrsQuery)

                    If updateCount <= 0 Then
                        ' 座席データの更新件数が0件以下の場合、処理終了

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                    End If
                Next
            Else
                ' 共通処理「バス座席自動設定処理」.ステータスが「10」の場合
                zasekiUpdateQuery = Me.createCrsKakuShashuZasekiData(zasekiData, crsZasekiData)
                updateCount = MyBase.execNonQuery(oracleTransaction, zasekiUpdateQuery)

                If updateCount <= 0 Then
                    ' 座席データの更新件数が0件以下の場合、処理終了

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure
                End If
            End If

            ' 部屋数更新
            Dim roomZansuUpdateQuery As String = Me.createCrsRoomUpdateSql(crsZasekiData, zasekiData, registEntity.YoyakuInfoBasicEntity, crsInfoBasicEntity, True)

            If String.IsNullOrEmpty(roomZansuUpdateQuery) = True Then
                ' ロールバック⇒SQL無しの場合は更新処理自体行わない（現行ASも同様）
                'MyBase.callRollbackTransaction(oracleTransaction)
                'Return CommonRegistYoyaku.UpdateStatusRoomStateFailure
            Else
                ' SQLが生成された場合のみ処理実行
                updateCount = MyBase.execNonQuery(oracleTransaction, roomZansuUpdateQuery)

                If updateCount <= 0 Then
                    ' ROOM数の更新件数が0件以下の場合、処理終了
                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusRoomStateFailure
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

            ' 予約情報（基本）
            Dim yoyakuInfoQuery As String = Me.createUpdateSql(Of YoyakuInfoBasicEntity)(registEntity.YoyakuInfoBasicEntity, registEntity.YoyakuInfoBasicPhysicsNameList, "T_YOYAKU_INFO_BASIC")
            Dim yoyakuInfoBuilder As New StringBuilder()
            yoyakuInfoBuilder.AppendLine(yoyakuInfoQuery)
            yoyakuInfoBuilder.AppendLine("WHERE")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.Value, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuKbn.DecimalBu))
            yoyakuInfoBuilder.AppendLine("    AND ")
            yoyakuInfoBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoBasicEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoBasicEntity.yoyakuNo.Value, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DBType, registEntity.YoyakuInfoBasicEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoBasicEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfoBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure
            End If

            ' 予約情報（コース料金）
            Dim crsChargeQuery As String = Me.createUpdateSql(Of YoyakuInfoCrsChargeEntity)(registEntity.YoyakuInfoCrsChargeEntity, registEntity.YoyakuInfoCrsChargePhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE")
            Dim crsChargeBuilder As New StringBuilder()
            crsChargeBuilder.AppendLine(crsChargeQuery)
            crsChargeBuilder.AppendLine("WHERE")
            crsChargeBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuKbn.DecimalBu))
            crsChargeBuilder.AppendLine("    AND ")
            crsChargeBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.Value, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DBType, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.IntegerBu, registEntity.YoyakuInfoCrsChargeEntity.yoyakuNo.DecimalBu))

            updateCount = MyBase.execNonQuery(oracleTransaction, crsChargeBuilder.ToString())

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyakuCrsChargeUpdateFailure
            End If

            ' 予約情報（コース料金_料金区分）
            Dim chargeKbnQuery As String = ""
            For Each entity As YoyakuInfoCrsChargeChargeKbnEntity In registEntity.YoyakuInfoCrsChargeChargeKbnList

                chargeKbnQuery = Me.createUpdateSql(Of YoyakuInfoCrsChargeChargeKbnEntity)(entity, registEntity.YoyakuInfoCrsChargeChargeKbnPhysicsNameList, "T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN")
                Dim chargeKbnBuilder As New StringBuilder()
                chargeKbnBuilder.AppendLine(chargeKbnQuery)
                chargeKbnBuilder.AppendLine("WHERE")
                chargeKbnBuilder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    KBN_NO = " + MyBase.setParam(entity.kbnNo.PhysicsName, entity.kbnNo.Value, entity.kbnNo.DBType, entity.kbnNo.IntegerBu, entity.kbnNo.DecimalBu))
                chargeKbnBuilder.AppendLine("    AND ")
                chargeKbnBuilder.AppendLine("    CHARGE_KBN_JININ_CD = " + MyBase.setParam(entity.chargeKbnJininCd.PhysicsName, entity.chargeKbnJininCd.Value, entity.chargeKbnJininCd.DBType, entity.chargeKbnJininCd.IntegerBu, entity.chargeKbnJininCd.DecimalBu))

                updateCount = MyBase.execNonQuery(oracleTransaction, chargeKbnBuilder.ToString())

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusYoyakuChargeKbnUpdateFailure
                End If
            Next

            ' 予約情報２
            Dim yoyakuInfo2Query As String

            Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
            Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

            If yoyakuInfo2.Rows.Count > 0 Then

                Dim yoyakuInfo2UpdateQuery As String = Me.createUpdateSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, registEntity.YoyakuInfo2PhysicsNameList, "T_YOYAKU_INFO_2")
                Dim yoyakuInfo2Builder As New StringBuilder()
                yoyakuInfo2Builder.AppendLine(yoyakuInfo2UpdateQuery)
                yoyakuInfo2Builder.AppendLine("WHERE")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_KBN = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuKbn.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuKbn.Value, registEntity.YoyakuInfo2Entity.yoyakuKbn.DBType, registEntity.YoyakuInfo2Entity.yoyakuKbn.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuKbn.DecimalBu))
                yoyakuInfo2Builder.AppendLine("    AND ")
                yoyakuInfo2Builder.AppendLine("    YOYAKU_NO = " + MyBase.setParam(registEntity.YoyakuInfo2Entity.yoyakuNo.PhysicsName, registEntity.YoyakuInfo2Entity.yoyakuNo.Value, registEntity.YoyakuInfo2Entity.yoyakuNo.DBType, registEntity.YoyakuInfo2Entity.yoyakuNo.IntegerBu, registEntity.YoyakuInfo2Entity.yoyakuNo.DecimalBu))

                yoyakuInfo2Query = yoyakuInfo2Builder.ToString()
            Else

                yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
            End If

            updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusYoyaku2UpdateFailure
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

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' WT_リクエスト情報キャンセル
    ''' </summary>
    ''' <param name="registEntity">予約登録データ</param>
    ''' <returns>処理結果</returns>
    Public Function cancelWtRequest(registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' WT_リクエスト情報更新
            If registEntity.WtRequestInfoEntity IsNot Nothing Then

                Dim wtRequestUpdateQuery As String = Me.createWtRequestStateUpdateSql(registEntity.WtRequestInfoEntity)

                updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestUpdateQuery)

                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoStateFailure
                End If
            End If

            ' コース台帳（基本）（WTキャンセル人数更新）
            Dim wtCancelNinzuQuery As String = Me.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction)
            updateCount = MyBase.execNonQuery(oracleTransaction, wtCancelNinzuQuery)

            If updateCount = 0 Then

                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure
            End If

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' WT情報登録
    ''' </summary>
    ''' <param name="registEntity">登録予約情報Entity</param>
    ''' <returns>更新ステータス</returns>
    Public Function updateWtRequestInfo(registEntity As RegistYoyakuInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' WT_リクエスト情報
            If String.IsNullOrEmpty(registEntity.WtRequestInfoEntity.managementKbn.Value) = True AndAlso registEntity.WtRequestInfoEntity.managementNo.Value Is Nothing Then

                ' WT管理番号採番
                Dim param As New numberingWtRequestNoParam()
                param.crsCd = registEntity.WtRequestInfoEntity.crsCd.Value
                param.managementKind = "W"

                YoyakuBizCommon.numberingWtRequestNo(param)

                registEntity.WtRequestInfoEntity.managementKbn.Value = param.managementKbn
                registEntity.WtRequestInfoEntity.managementNo.Value = param.managementNo

                Dim wtRequestQuery As String = Me.createInsertSql(Of WtRequestInfoEntity)(registEntity.WtRequestInfoEntity, "T_WT_REQUEST_INFO")
                updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestQuery)

                If updateCount = 0 Then

                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoFailure
                End If
            Else

                Dim query As String = Me.createUpdateSql(Of WtRequestInfoEntity)(registEntity.WtRequestInfoEntity, registEntity.WtRequestInfoPhysicsNameList, "T_WT_REQUEST_INFO")

                Dim sb As New StringBuilder()
                sb.Append(query)
                sb.AppendLine(" WHERE ")
                sb.AppendLine("     MANAGEMENT_KBN = " + MyBase.setParam(registEntity.WtRequestInfoEntity.managementKbn.PhysicsName,
                                                                         registEntity.WtRequestInfoEntity.managementKbn.Value,
                                                                         registEntity.WtRequestInfoEntity.managementKbn.DBType,
                                                                         registEntity.WtRequestInfoEntity.managementKbn.IntegerBu,
                                                                         registEntity.WtRequestInfoEntity.managementKbn.DecimalBu))
                sb.AppendLine("     AND ")
                sb.AppendLine("     MANAGEMENT_NO = " + MyBase.setParam(registEntity.WtRequestInfoEntity.managementNo.PhysicsName,
                                                                        registEntity.WtRequestInfoEntity.managementNo.Value,
                                                                        registEntity.WtRequestInfoEntity.managementNo.DBType,
                                                                        registEntity.WtRequestInfoEntity.managementNo.IntegerBu,
                                                                        registEntity.WtRequestInfoEntity.managementNo.DecimalBu))

                updateCount = MyBase.execNonQuery(oracleTransaction, sb.ToString())

                If updateCount = 0 Then

                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoFailure
                End If
            End If

            ' WT_リクエスト情報（コース料金_料金区分）
            Dim wtRequestChargeKbnDeleteQuery As String = Me.createDeleteSql(registEntity.WtRequestInfoEntity.managementKbn.Value,
                                                                             registEntity.WtRequestInfoEntity.managementNo.Value,
                                                                             "T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN")
            updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestChargeKbnDeleteQuery)

            Dim wtRequestChargeKbnQuery As String = ""
            For Each entity As WtRequestInfoCrsChargeChargeKbnEntity In registEntity.WtRequestInfoCrsChargeChargeKbnList

                entity.managementKbn.Value = registEntity.WtRequestInfoEntity.managementKbn.Value
                entity.managementNo.Value = registEntity.WtRequestInfoEntity.managementNo.Value

                wtRequestChargeKbnQuery = Me.createInsertSql(Of WtRequestInfoCrsChargeChargeKbnEntity)(entity, "T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN")
                updateCount = MyBase.execNonQuery(oracleTransaction, wtRequestChargeKbnQuery)

                If updateCount = 0 Then

                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonRegistYoyaku.UpdateStatusWtRequestInfoCrsChargeChargeKbnFailure
                End If
            Next

            ' コース台帳（基本）（WTキャンセル人数更新）
            Dim wtCancelNinzuQuery As String = Me.createWtCancelNinzuUpdateSql(registEntity.CrsLedgerBasicEntity, oracleTransaction)
            updateCount = MyBase.execNonQuery(oracleTransaction, wtCancelNinzuQuery)

            If updateCount = 0 Then

                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonRegistYoyaku.UpdateStatusWtCancelNinzuFailure
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

        Return CommonRegistYoyaku.UpdateStatusSucess
    End Function

    ''' <summary>
    ''' WTリクエストコース情報取得
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報entity</param>
    ''' <returns>WTリクエストコース情報</returns>
    Public Function getCrsWtRequestInfo(entity As WtRequestInfoEntity) As DataTable

        Dim wtRequestInfo As DataTable

        Try

            Dim query As String = Me.createCrsWtRequestInfoSql(entity)
            wtRequestInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return wtRequestInfo
    End Function

    ''' <summary>
    ''' WTリクエスト料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WTリクエスト料金区分一覧</returns>
    Public Function getWtRequestChargeList(entity As WtRequestInfoEntity) As DataTable

        Dim WtRequestChargeList As DataTable

        Try

            Dim query As String = Me.createWtRequestChargeListSql(entity)
            WtRequestChargeList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return WtRequestChargeList
    End Function

    ''' <summary>
    ''' WTリクエスト料金区分一覧取得
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WTリクエスト料金区分一覧</returns>
    Public Function getWtRequestChargeKbnList(entity As WtRequestInfoEntity) As DataTable

        Dim wtRequestChargeList As DataTable

        Try

            Dim query As String = Me.createWtRequestChargeKbnList(entity)
            wtRequestChargeList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return wtRequestChargeList
    End Function

    ''' <summary>
    ''' WT_リクエスト情報（メモ）取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報（メモ）Entity</param>
    ''' <returns>WT_リクエスト情報（メモ）取得SQL</returns>
    Public Function getWtRequestMemoInfoList(entity As WtRequestInfoMemoEntity) As DataTable

        Dim wtRequestMemoInfo As DataTable

        Try
            Dim query As String = Me.createWtRequestMemoInfoListSql(entity)
            wtRequestMemoInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return wtRequestMemoInfo
    End Function

    ''' <summary>
    ''' WT_リクエスト情報取得
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WT_リクエスト情報</returns>
    Public Function getWtRequestInfo(entity As WtRequestInfoEntity) As DataTable

        Dim wtRequestInfo As DataTable

        Try

            Dim query As String = Me.createWtRequestInfoSql(entity)
            wtRequestInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return wtRequestInfo
    End Function

    ''' <summary>
    ''' 複写予約情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>複写予約情報</returns>
    Public Function getReproductionYoyakuData(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuData As DataTable

        Try
            Dim query As String = Me.createReproductionYoyakuDataSql(entity)
            yoyakuData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuData
    End Function

    ''' <summary>
    ''' 複写WTリクエスト情報取得
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>複写WTリクエスト情報</returns>
    Public Function geteReproductionWtRequestData(entity As WtRequestInfoEntity) As DataTable

        Dim yoyakuData As DataTable

        Try
            Dim query As String = Me.createReproductionWtRequestDataSql(entity)
            yoyakuData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuData
    End Function

    ''' <summary>
    ''' 代理店情報取得
    ''' </summary>
    ''' <param name="entity">代理店マスタEntity</param>
    ''' <returns>代理店情報</returns>>
    Public Function getAgentMaster(entity As MAgentEntity) As DataTable

        Dim agentMasterData As DataTable

        Try
            Dim query As String = Me.createAgentMasterSql(entity)
            agentMasterData = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return agentMasterData
    End Function

    ''' <summary>
    ''' 予約ピックアップ情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約ピックアップ情報</returns>
    Public Function getYoyakuPickupList(entity As YoyakuInfoBasicEntity) As DataTable

        Dim yoyakuPickupList As DataTable

        Try
            Dim query As String = Me.createYoyakuPickupInfoSql(entity)
            yoyakuPickupList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuPickupList
    End Function

    ''' <summary>
    ''' その他情報取得
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>その他情報</returns>
    Public Function getSonotaInfo(entity As YoyakuInfoBasicEntity) As DataTable

        Dim sonotaInfo As DataTable

        Try
            Dim query As String = Me.createSonotaInfoSql(entity)
            sonotaInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return sonotaInfo
    End Function

    ''' <summary>
    ''' その他情報取得
    ''' </summary>
    ''' <param name="entity">WTリクエストEntity</param>
    ''' <returns>その他情報</returns>
    Public Function getWtSonotaInfo(entity As WtRequestInfoEntity) As DataTable

        Dim sonotaInfo As DataTable

        Try
            Dim query As String = Me.createWtSonotaInfoSql(entity)
            sonotaInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return sonotaInfo
    End Function

    ''' <summary>
    ''' 利用者情報取得
    ''' </summary>
    ''' <param name="nullRecord">空行レコード挿入フラグ</param>
    ''' <returns>利用者情報</returns>
    Public Function getUserMaster(nullRecord As Boolean) As DataTable

        Dim sonotaInfo As DataTable

        Try
            Dim query As String = Me.createUserMasterSql(nullRecord)
            sonotaInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return sonotaInfo
    End Function

    ''' <summary>
    ''' コース情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>コース情報取得SQL</returns>
    Private Function createCrsLedgerBasicSql(entity As CrsLedgerBasicEntity) As String

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
        sb.AppendLine("     ,CLB.SAIKOU_DAY ")
        sb.AppendLine("     ,CLB.UNKYU_CONTACT_DAY ")
        sb.AppendLine("     ,CLB.RETURN_DAY ")
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ")
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ")
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
        sb.AppendLine("     ,CLB.TYUIJIKOU ")
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ")
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ")
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ")
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ")
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ")
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ")
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
        sb.AppendLine("     ,CLB.CANCEL_RYOU_KBN ")
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ")
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ")
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ")
        sb.AppendLine("     ,CLB.TEJIMAI_DAY ")

        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ")
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ")
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ")
        sb.AppendLine("     ,CLB.AGE_FLG ")
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ")
        sb.AppendLine("     ,CLB.TEL_FLG ")
        sb.AppendLine("     ,CLB.ADDRESS_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ")

        sb.AppendLine("     ,CLB.YOYAKU_STOP_FLG ")
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ")
        sb.AppendLine("     ,CLB.USING_FLG ")

        sb.AppendLine("     ,CLB.TOJITU_KOKUCHI_FLG")

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
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 使用中フラグ取得SQL作成
    ''' </summary>
    ''' <param name="paramInfoList">パラメータ群</param>
    ''' <returns>使用中フラグ取得SQL</returns>
    Private Function createUsingFlagSql(paramInfoList As Hashtable) As String

        MyBase.paramClear()

        Dim entity As New CrsLedgerBasicEntity()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      USING_FLG ")
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" WHERE  ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", paramInfoList.Item("CRS_CD"), entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", paramInfoList.Item("SYUPT_DAY"), entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))

        If String.IsNullOrEmpty(paramInfoList.Item("GOUSYA").ToString()) = False Then
            sb.AppendLine("     AND ")
            sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", paramInfoList.Item("GOUSYA"), entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        End If

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 後泊部屋数更新SQL作成
    ''' </summary>
    ''' <param name="atohakuCrsCd">後泊コースコード</param>
    ''' <param name="defRoomsu">>差分部屋数</param>
    ''' <param name="paramInfoList">パラメータ群</param>
    ''' <returns>後泊部屋数更新SQL</returns>
    Private Function createAtohakuRoomsuUpdateSql(atohakuCrsCd As String, defRoomsu As Integer, paramInfoList As Hashtable) As String

        MyBase.paramClear()

        Dim entity As New CrsLedgerBasicEntity()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = YOYAKU_NUM_TEISEKI + " + MyBase.setParam("KUSEKI_NUM_TEISEKI", defRoomsu, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = KUSEKI_NUM_TEISEKI - " + MyBase.setParam("KUSEKI_NUM_TEISEKI", defRoomsu, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam("SYSTEM_UPDATE_PGMID", paramInfoList.Item("SYSTEM_UPDATE_PGMID"), entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam("SYSTEM_UPDATE_PERSON_CD", paramInfoList.Item("SYSTEM_UPDATE_PERSON_CD"), entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam("SYSTEM_UPDATE_DAY", paramInfoList.Item("SYSTEM_UPDATE_DAY"), entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", atohakuCrsCd, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", paramInfoList.Item("SYUPT_DAY"), entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース使用中チェック
    ''' 企画用
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <param name="oracleTransaction">トランザクション</param>
    ''' <param name="zasekiData">自コース座席情報</param>
    ''' <param name="sharedZasekiData">共用コース座席情報</param>
    ''' <returns>検証結果</returns>
    Private Function isCrsLenderZasekiDataForKikaku(paramInfoList As Hashtable, oracleTransaction As OracleTransaction, ByRef zasekiData As DataTable, ByRef sharedZasekiData As DataTable) As Boolean

        Dim isValid As Boolean = False
        Dim idx As Integer = 1
        Dim busReserveCd As String = ""
        Dim sharedCrsQuery As String = ""

        While idx <= UsingCheckRetryNum

            '　自コースの座席情報取得SQL作成
            Dim crsQuery As String = Me.createCrsLedgerZasekiSql(paramInfoList)

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
            sharedCrsQuery = Me.createSharedBusCrsZasekiSql(paramInfoList, busReserveCd)
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

            ' 共用コースが使用中でない場合、処理終了
            isValid = True
            Exit While
        End While

        Return isValid
    End Function

    ''' <summary>
    ''' コース台帳座席情報取得SQL作成
    ''' 悲観ロック用
    ''' </summary>
    ''' <param name="paramInfoList">パラメータ群</param>
    ''' <returns>コース台帳座席情報取得SQL</returns>
    Private Function createCrsLedgerZasekiSql(paramInfoList As Hashtable) As String

        MyBase.paramClear()

        Dim entity As New CrsLedgerBasicEntity()

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
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", paramInfoList.Item("CRS_CD"), entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", paramInfoList.Item("SYUPT_DAY"), entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", paramInfoList.Item("GOUSYA"), entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" FOR UPDATE WAIT 10 ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 共用バスコース座席情報取得SQL作成
    ''' 悲観ロック用
    ''' </summary>
    ''' <param name="paramInfoList">パラメータ群</param>
    ''' <param name="busReserveCd">バス指定コード</param>
    ''' <returns>共用バスコース座席情報取得SQL</returns>
    Private Function createSharedBusCrsZasekiSql(paramInfoList As Hashtable, busReserveCd As String) As String

        MyBase.paramClear()

        Dim entity As New CrsLedgerBasicEntity()

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
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", paramInfoList.Item("SYUPT_DAY"), entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", paramInfoList.Item("GOUSYA"), entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" FOR UPDATE WAIT 10 ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コース台帳座席情報更新SQL作成
    ''' 定期用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」情報</param>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="crsZasekiData">座席更新の検索条件</param>
    ''' <param name="kusekiTeisaki">空席数定席</param>
    ''' <param name="kusekiSubSeat">空席数補助席</param>
    ''' <returns>コース台帳座席情報更新SQL</returns>
    Private Function createCrsZasekiDataForTeiki(z0001Result As Z0001_Result, crsLedgerZasekiData As DataTable, crsZasekiData As Hashtable, ByRef kusekiTeisaki As Integer, ByRef kusekiSubSeat As Integer) As String

        Dim crsYoyakuSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim crsKusekiSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))
        Dim crsKusekiSuSubSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"))

        ' 予約数定席
        ' コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim yoyakuSuTeisaki As Integer = crsYoyakuSuTeisaki + z0001Result.ZasekiKagenTeiseki

        ' 空席数定席算出
        ' コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        kusekiTeisaki = Me.calcKuusekiSu(crsKusekiSuTeisaki, z0001Result.ZasekiKagenTeiseki, z0001Result.KusekiNumTeiseki)

        ' 空席数定席マイナスチェック
        If kusekiTeisaki < CommonRegistYoyaku.ZERO Then
            ' 空席数定席が'0'を下回る場合、エラーとして更新処理終了
            Return ""
        End If

        ' 空席数補助席算出
        ' コース台帳(基本).空席数補助席、共通処理「バス座席自動設定処理」.補助調整空席、共通処理「バス座席自動設定処理」.空席数/補助・1F
        kusekiSubSeat = Me.calKusekiSuSubSeatForTeiki(crsKusekiSuSubSeat, z0001Result.SubCyoseiSeatNum, z0001Result.ZasekiKagenSub1F)

        Dim entity As New CrsLedgerBasicEntity()

        entity.crsCd.Value = crsZasekiData.Item("CRS_CD").ToString()
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("GOUSYA"))
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("SYUPT_DAY"))
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki
        entity.kusekiNumTeiseki.Value = kusekiTeisaki
        entity.kusekiNumSubSeat.Value = kusekiSubSeat
        entity.systemUpdatePgmid.Value = crsZasekiData.Item("SYSTEM_UPDATE_PGMID").ToString()
        entity.systemUpdatePersonCd.Value = crsZasekiData.Item("SYSTEM_UPDATE_PERSON_CD").ToString()
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData.Item("SYSTEM_UPDATE_DAY").ToString())

        ' コース台帳座席更新SQL作成
        Dim query As String = Me.createCrsZasekiUpdateSql(entity)

        Return query
    End Function

    ''' <summary>
    ''' 共用コース座席更新SQL作成
    ''' 定席用
    ''' </summary>
    ''' <param name="sharedRow">共用コース座席情報</param>
    ''' <param name="crsZasekiData">コース台帳座席更新データ</param>
    ''' <param name="kusekiTeisaki">空席数定席</param>
    ''' <param name="kusekiSubSeat">空席数補助席</param>
    ''' <returns>共用コース座席更新SQL</returns>
    Private Function createSharedCrsZasekiUpdateSqlForTeiseki(sharedRow As DataRow, crsZasekiData As Hashtable, kusekiTeisaki As Integer, kusekiSubSeat As Integer) As String

        Dim entiy As New CrsLedgerBasicEntity()
        entiy.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"))
        entiy.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"))
        entiy.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"))
        entiy.kusekiNumTeiseki.Value = kusekiTeisaki
        entiy.kusekiNumSubSeat.Value = kusekiSubSeat

        Dim query As String = Me.createSharedCrsZasekiUpdateSql(crsZasekiData, entiy)

        Return query
    End Function

    ''' <summary>
    ''' コース台帳座席情報更新SQL作成
    ''' 企画用
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」情報</param>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="crsZasekiData">座席更新の検索条件</param>
    ''' <returns>コース台帳座席情報更新SQL</returns>
    Private Function createCrsZasekiDataForKikaku(z0001Result As Z0001_Result, crsLedgerZasekiData As DataTable, crsZasekiData As Hashtable) As String

        Dim crsYoyakuSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI"))
        Dim crsKusekiSuTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI"))
        Dim crsKusekiSuSubSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_SUB_SEAT"))

        ' 予約数定席
        ' コース台帳（基本）.予約数・定席 + 共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim yoyakuSuTeisaki As Integer = crsYoyakuSuTeisaki + z0001Result.ZasekiKagenTeiseki
        ' 空席数定席算出
        ' コース台帳(基本).空席数定席, 共通処理「バス座席自動設定処理」.座席加減数・定席, 共通処理「バス座席自動設定処理」.空席数・定席
        Dim kusekiSuTeisaki As Integer = Me.calcKuusekiSu(crsKusekiSuTeisaki, z0001Result.ZasekiKagenTeiseki, z0001Result.KusekiNumTeiseki)
        ' 空席数補助席算出
        ' コース台帳(基本).空席数定席,共通処理「バス座席自動設定処理」.座席加減数・補助席,共通処理「バス座席自動設定処理」.補助調整空席,共通処理「バス座席自動設定処理」.空席数/補助・1F
        Dim kusekiSuSubSeat As Integer = Me.calKusekiSuSubSeatForKikaku(crsKusekiSuSubSeat,
                                                                        z0001Result.ZasekiKagenSub1F,
                                                                        z0001Result.SubCyoseiSeatNum,
                                                                        z0001Result.KusekiNumSub1F)

        If kusekiSuTeisaki < CommonRegistYoyaku.ZERO OrElse kusekiSuSubSeat < CommonRegistYoyaku.ZERO Then
            ' 空席数定席、空席数補助席が'0'を下回る場合、エラーとして更新処理終了

            Return ""
        End If

        Dim entity As New CrsLedgerBasicEntity()
        entity.crsCd.Value = crsZasekiData.Item("CRS_CD").ToString()
        entity.gousya.Value = Integer.Parse(crsZasekiData.Item("GOUSYA").ToString())
        entity.syuptDay.Value = Integer.Parse(crsZasekiData.Item("SYUPT_DAY").ToString())
        entity.yoyakuNumTeiseki.Value = yoyakuSuTeisaki

        ' 予約数補助席
        If String.IsNullOrEmpty(z0001Result.SeatKbn) = False Then
            ' 共通処理「バス座席自動設定処理」.席区分が空以外の場合
            ' 予約数補助席を設定(コース台帳（基本）.予約数・補助席 + 共通処理「バス座席自動設定処理」.座席加減数・補助席)
            Dim crsYoyakuSuSubSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_SUB_SEAT"))

            entity.yoyakuNumSubSeat.Value = crsYoyakuSuSubSeat + z0001Result.ZasekiKagenSub1F
        End If

        entity.kusekiNumTeiseki.Value = kusekiSuTeisaki
        entity.kusekiNumSubSeat.Value = kusekiSuSubSeat
        entity.systemUpdatePgmid.Value = crsZasekiData.Item("SYSTEM_UPDATE_PGMID").ToString()
        entity.systemUpdatePersonCd.Value = crsZasekiData.Item("SYSTEM_UPDATE_PERSON_CD").ToString()
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData.Item("SYSTEM_UPDATE_DAY").ToString())

        ' コース台帳座席更新SQL作成
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

        ' コース台帳(基本).空席数定席 - >共通処理「バス座席自動設定処理」.座席加減数・定席
        Dim kusekiSu As Integer = crsKusekiSu - comZasekiKagenSu

        If kusekiSu >= comKusekiSu Then
            ' 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席」が共通処理「バス座席自動設定処理」.空席数・定席以上の場合
            ' 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席とする
            kusekiSu = comKusekiSu
        End If

        Return kusekiSu
    End Function

    ''' <summary>
    ''' 空席数補助席算出
    ''' </summary>
    ''' <param name="crsKusekiSu">コース台帳(基本).空席数補助席</param>
    ''' <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    ''' <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    ''' <returns>空席数補助席</returns>
    Private Function calKusekiSuSubSeatForTeiki(crsKusekiSu As Integer, comSubChoseiKusekiSu As Integer, comKusekiSuSubSeat1F As Integer) As Integer

        ' コース台帳(基本).空席数定席 - 共通処理「バス座席自動設定処理」.補助調整空席
        Dim kusekiSu As Integer = crsKusekiSu - comSubChoseiKusekiSu

        If kusekiSu >= comKusekiSuSubSeat1F Then
            ' 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.補助調整空席」が共通処理「バス座席自動設定処理」.空席数・定席以上の場合
            kusekiSu = comKusekiSuSubSeat1F
        End If

        If kusekiSu < CommonRegistYoyaku.ZERO Then
            ' 空席数補助席の算出結果が0を下回る場合、0とする
            kusekiSu = CommonRegistYoyaku.ZERO
        End If

        Return kusekiSu
    End Function

    ''' <summary>
    ''' 空席数補助席算出
    ''' </summary>
    ''' <param name="crsKusekiSu">コース台帳(基本).空席数定席</param>
    ''' <param name="comZasekiKagenSu">共通処理「バス座席自動設定処理」.座席加減数・補助席</param>
    ''' <param name="comSubChoseiKusekiSu">共通処理「バス座席自動設定処理」.補助調整空席</param>
    ''' <param name="comKusekiSuSubSeat1F">共通処理「バス座席自動設定処理」.空席数/補助・1F</param>
    ''' <returns></returns>
    Private Function calKusekiSuSubSeatForKikaku(crsKusekiSu As Integer, comZasekiKagenSu As Integer, comSubChoseiKusekiSu As Integer, comKusekiSuSubSeat1F As Integer) As Integer

        Dim kusekiSu As Integer = crsKusekiSu - comZasekiKagenSu - comSubChoseiKusekiSu

        If kusekiSu >= comKusekiSuSubSeat1F Then
            ' 「コース台帳(基本).空席数・定席 - 共通処理「バス座席自動設定処理」.座席加減数・定席 - 共通処理「バス座席自動設定処理」.補助調整空席」が共通処理「バス座席自動設定処理」.空席数/補助・1F以上の場合
            kusekiSu = comKusekiSuSubSeat1F
        End If

        Return kusekiSu
    End Function

    ''' <summary>
    ''' コース台帳座席更新SQL作成
    ''' </summary>
    ''' <param name="crsLedgerZasekiData">コース台帳座席情報</param>
    ''' <param name="crsZasekiData">座席更新の検索条件</param>
    ''' <returns>コース台帳座席更新SQL</returns>
    Private Function createCrsKakuShashuZasekiData(crsLedgerZasekiData As DataTable, crsZasekiData As Hashtable) As String

        Dim yoyakuNum As Integer = Integer.Parse(crsZasekiData.Item("YOYAKU_NINZU").ToString())
        Dim yoyakuNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("YOYAKU_NUM_TEISEKI").ToString())
        Dim kusekiNumTeiseki As Integer = Integer.Parse(crsLedgerZasekiData.Rows(0)("KUSEKI_NUM_TEISEKI").ToString())

        Dim entity As New CrsLedgerBasicEntity()

        entity.crsCd.Value = crsZasekiData.Item("CRS_CD").ToString()
        entity.gousya.Value = Integer.Parse(crsZasekiData.Item("GOUSYA").ToString())
        entity.syuptDay.Value = Integer.Parse(crsZasekiData.Item("SYUPT_DAY").ToString())
        entity.yoyakuNumTeiseki.Value = yoyakuNumTeiseki + yoyakuNum
        entity.kusekiNumTeiseki.Value = kusekiNumTeiseki - yoyakuNum
        entity.systemUpdatePgmid.Value = crsZasekiData.Item("SYSTEM_UPDATE_PGMID").ToString()
        entity.systemUpdatePersonCd.Value = crsZasekiData.Item("SYSTEM_UPDATE_PERSON_CD").ToString()
        entity.systemUpdateDay.Value = DateTime.Parse(crsZasekiData.Item("SYSTEM_UPDATE_DAY").ToString())

        ' コース台帳架空車種座席更新SQL作成
        Dim query As String = Me.createCrsKakuShashuZasekiUpdateSql(entity)

        Return query
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
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam("YOYAKU_NUM_TEISEKI", entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))

        If entity.yoyakuNumSubSeat.Value IsNot Nothing Then

            sb.AppendLine("     ,YOYAKU_NUM_SUB_SEAT = " + MyBase.setParam("YOYAKU_NUM_SUB_SEAT", entity.yoyakuNumSubSeat.Value, entity.yoyakuNumSubSeat.DBType, entity.yoyakuNumSubSeat.IntegerBu, entity.yoyakuNumSubSeat.DecimalBu))
        End If

        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam("KUSEKI_NUM_TEISEKI", entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam("KUSEKI_NUM_SUB_SEAT", entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam("SYSTEM_UPDATE_PGMID", entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam("SYSTEM_UPDATE_PERSON_CD", entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam("SYSTEM_UPDATE_DAY", entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
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
        sb.AppendLine("      YOYAKU_NUM_TEISEKI = " + MyBase.setParam("YOYAKU_NUM_TEISEKI", entity.yoyakuNumTeiseki.Value, entity.yoyakuNumTeiseki.DBType, entity.yoyakuNumTeiseki.IntegerBu, entity.yoyakuNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_TEISEKI = " + MyBase.setParam("KUSEKI_NUM_TEISEKI", entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam("SYSTEM_UPDATE_PGMID", entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam("SYSTEM_UPDATE_PERSON_CD", entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam("SYSTEM_UPDATE_DAY", entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", entity.gousya.Value, entity.syuptDay.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 自コースと共用コースのコースコード、号車、出発日が同一チェック
    ''' </summary>
    ''' <param name="sharedRow">共用コースRow</param>
    ''' <param name="crsZasekiData">座席更新の検索条件作成</param>
    ''' <returns>検証結果</returns>
    Private Function isSharedBusCrsEqualCheck(sharedRow As DataRow, crsZasekiData As Hashtable) As Boolean

        Dim csharedCrsCd As String = sharedRow("CRS_CD").ToString()
        Dim csharedGousya As Integer = Integer.Parse(sharedRow("GOUSYA").ToString())
        Dim csharedSyuptDay As Integer = Integer.Parse(sharedRow("SYUPT_DAY").ToString())

        Dim crsCd As String = crsZasekiData.Item("CRS_CD").ToString()
        Dim gousya As Integer = Integer.Parse(crsZasekiData.Item("GOUSYA").ToString())
        Dim syuptDay As Integer = Integer.Parse(crsZasekiData.Item("SYUPT_DAY").ToString())

        If csharedCrsCd = crsCd AndAlso csharedGousya = gousya AndAlso csharedSyuptDay = syuptDay Then

            ' 自コースと共用コースのコースコード、号車、出発日が同一の場合
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 共用コース座席更新SQL作成
    ''' </summary>
    ''' <param name="z0001Result">共通処理「バス座席自動設定処理」</param>
    ''' <param name="sharedRow">共用コース座席情報</param>
    ''' <param name="crsZasekiData">座席更新の検索条件</param>
    ''' <returns>共用コース座席更新SQL</returns>
    Private Function createSharedBusCrsZasekiUpdateSql(z0001Result As Z0001_Result, sharedRow As DataRow, crsZasekiData As Hashtable) As String

        Dim kusekiTeisaki As Integer = 0
        Dim kusekiSubSeat As Integer = 0

        Dim jyosyaCapacity As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("JYOSYA_CAPACITY"))
        Dim capacityRegular As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("CAPACITY_REGULAR"))
        Dim capacityHo1kai As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("CAPACITY_HO_1KAI"))

        If jyosyaCapacity >= capacityRegular + capacityHo1kai Then
            ' コース台帳(基本).乗車定員が「コース台帳(基本).定員定 + コース台帳(基本).定員補1F」以上の場合

            ' 空席数定席を共通処理「バス座席自動設定処理」.空席数・定席
            kusekiTeisaki = z0001Result.KusekiNumTeiseki
            ' 空席数補助席を共通処理「バス座席自動設定処理」.空席数・補助席
            kusekiSubSeat = z0001Result.KusekiNumSub1F
        Else
            ' それ以外の場合

            ' 営BLK算出
            Dim eiBlockNum As Integer = Me.calEiBlockNum(sharedRow)

            If jyosyaCapacity >= capacityRegular Then
                ' コース台帳(基本).乗車定員がコース台帳(基本).定員定以上の場合

                If capacityRegular - eiBlockNum <= z0001Result.KusekiNumTeiseki Then
                    ' 「コース台帳(基本).定員定 - 営BLK算出結果」が共通処理「バス座席自動設定処理」.空席数・定席以下の場合

                    ' 空席数・定席を「コース台帳(基本).定員定 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum
                Else
                    ' それ以外の場合

                    ' 空席数・定席を共通処理「バス座席自動設定処理」.空席数・定席とする
                    kusekiTeisaki = z0001Result.KusekiNumTeiseki
                End If

                Dim eiBlockHo As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("EI_BLOCK_HO"))
                Dim yoyaokuNumSubSeat As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("YOYAKU_NUM_SUB_SEAT"))

                If capacityRegular - eiBlockHo - yoyaokuNumSubSeat <= z0001Result.KusekiNumSub1F Then
                    ' 「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」が
                    ' 共通処理「バス座席自動設定処理」.空席数・補助席以下の場合

                    ' 空席数・補助席を「コース台帳(基本).定員定 - コース台帳(基本).定員定 - コース台帳(基本).営ブロック補 - コース台帳(基本).予約数・補助席」とする
                    kusekiSubSeat = capacityRegular - eiBlockHo - yoyaokuNumSubSeat
                Else
                    ' それ以外の場合

                    ' 空席数・補助席をを共通処理「バス座席自動設定処理」空席数・補助席とする
                    kusekiSubSeat = z0001Result.KusekiNumSub1F
                End If
            Else
                ' それ以外の場合、

                If jyosyaCapacity - eiBlockNum <= z0001Result.KusekiNumTeiseki Then
                    ' 「コース台帳(基本).乗車定員 - 営BLK算出結果」が共通処理「バス座席自動設定処理」空席数・定席以下の場合

                    ' 空席数・定席を「コース台帳(基本).乗車定員 - 営BLK算出結果」とする
                    kusekiTeisaki = capacityRegular - eiBlockNum
                Else
                    ' それ以外の場合

                    ' 空席数・定席を共通処理「バス座席自動設定処理」空席数・定席とする
                    kusekiTeisaki = z0001Result.KusekiNumTeiseki
                End If

                ' 空席数・補助席は'0'とする
                kusekiSubSeat = CommonRegistYoyaku.ZERO
            End If
        End If

        Dim entiy As New CrsLedgerBasicEntity()
        entiy.crsCd.Value = CommonRegistYoyaku.convertObjectToString(sharedRow("CRS_CD"))
        entiy.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("SYUPT_DAY"))
        entiy.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(sharedRow("GOUSYA"))
        entiy.kusekiNumTeiseki.Value = kusekiTeisaki
        entiy.kusekiNumSubSeat.Value = kusekiSubSeat

        ' 更新SQL作成
        Dim query As String = Me.createSharedCrsZasekiUpdateSql(crsZasekiData, entiy)

        Return query
    End Function

    ''' <summary>
    ''' 営BLK算出
    ''' </summary>
    ''' <param name="sharedRow">共用コース座席情報</param>
    ''' <returns>営BLK</returns>
    Private Function calEiBlockNum(sharedRow As DataRow) As Integer

        Dim eiBlockRegular As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("EI_BLOCK_REGULAR"))
        Dim blockKakuhoNum As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("BLOCK_KAKUHO_NUM"))
        Dim yoyakuNumTeisaki As Integer = CommonRegistYoyaku.convertObjectToInteger(sharedRow("YOYAKU_NUM_TEISEKI"))

        ' 営BLK = 「コース台帳(基本).営ブロック定 + コース台帳(基本).ブロック確保数 + コース台帳(基本).空席確保数 + コース台帳(基本).予約数・定席
        Dim eiBlk As Integer = eiBlockRegular + blockKakuhoNum + yoyakuNumTeisaki

        Return eiBlk
    End Function

    ''' <summary>
    ''' 共用コース座席更新SQL作成
    ''' </summary>
    ''' <param name="crsZasekiData">座席更新の検索条件</param>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <returns>共用コース座席更新SQL</returns>
    Private Function createSharedCrsZasekiUpdateSql(crsZasekiData As Hashtable, entity As CrsLedgerBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      KUSEKI_NUM_TEISEKI = " + MyBase.setParam(entity.kusekiNumTeiseki.PhysicsName, entity.kusekiNumTeiseki.Value, entity.kusekiNumTeiseki.DBType, entity.kusekiNumTeiseki.IntegerBu, entity.kusekiNumTeiseki.DecimalBu))
        sb.AppendLine("     ,KUSEKI_NUM_SUB_SEAT = " + MyBase.setParam(entity.kusekiNumSubSeat.PhysicsName, entity.kusekiNumSubSeat.Value, entity.kusekiNumSubSeat.DBType, entity.kusekiNumSubSeat.IntegerBu, entity.kusekiNumSubSeat.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, crsZasekiData.Item("SYSTEM_UPDATE_PGMID"), entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, crsZasekiData.Item("SYSTEM_UPDATE_PERSON_CD"), entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + MyBase.setParam(entity.systemUpdateDay.PhysicsName, crsZasekiData.Item("SYSTEM_UPDATE_DAY"), entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

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
    ''' 予約情報（基本）UPDATE SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <param name="physicsNameList">更新するカラム名一覧</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>予約情報（基本）UPDATE SQL</returns>
    Private Function createYoyakuInfoBasicUpdateSql(entity As YoyakuInfoBasicEntity, physicsNameList As List(Of String), tableName As String) As String

        Dim valueQuery As String = Me.createUpdateSql(Of YoyakuInfoBasicEntity)(entity, physicsNameList, tableName)

        Dim sb As New StringBuilder()
        sb.AppendLine(valueQuery)
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName,
                                                                                             entity.yoyakuKbn.Value,
                                                                                             entity.yoyakuKbn.DBType,
                                                                                             entity.yoyakuKbn.IntegerBu,
                                                                                             entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName,
                                                                                            entity.yoyakuNo.Value,
                                                                                            entity.yoyakuNo.DBType,
                                                                                            entity.yoyakuNo.IntegerBu,
                                                                                            entity.yoyakuNo.DecimalBu))

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
    ''' 料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
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
    ''' コース台帳（コース情報）取得SQL作成
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>コース台帳（コース情報）取得SQL</returns>
    Private Function createCrsInfoSql(entity As CrsLedgerCrsInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      LINE_NO ")
        sb.AppendLine("     ,CRS_JOKEN ")
        sb.AppendLine(" FROM T_CRS_LEDGER_CRS_INFO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     DELETE_DAY = 0 ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CRS_CD =  " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" ORDER BY ")
        sb.AppendLine("     LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' メッセージ情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">パラメータ群</param>
    ''' <returns>メッセージ情報取得SQL</returns>
    Private Function createCrsInfoMessageSql(entity As CrsLedgerMessageEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      '' AS MESSAGE_CHECK_FLG ")
        sb.AppendLine("     ,LINE_NO ")
        sb.AppendLine("     ,MESSAGE ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_MESSAGE ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine(" ORDER BY ")
        sb.AppendLine("     LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' コード分類の内容を取得SQL作成
    ''' </summary>
    ''' <param name="codeBunrui">コード分類</param>
    ''' <param name="nullRecord">空行レコード挿入フラグ</param>
    ''' <returns>コード分類の内容を取得SQL</returns>
    Private Function createCodeMasterSql(codeBunrui As String, nullRecord As Boolean) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT * FROM ( ")
        If nullRecord = True Then
            sb.AppendLine(" SELECT ")
            sb.AppendLine("      ' ' AS CODE_VALUE ")
            sb.AppendLine("     ,' ' AS CODE_NAME ")
            sb.AppendLine("     ,'0' AS NAIYO_1 ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     DUAL ")
            sb.AppendLine(" UNION ")
        End If
        sb.AppendLine(" SELECT ")
        sb.AppendLine("       CODE_VALUE ")
        sb.AppendLine("      ,CODE_NAME ")
        sb.AppendLine("      ,NAIYO_1 ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_CODE ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CODE_BUNRUI = :BUNRUI ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     NVL(DELETE_DATE, 0) = 0 ")
        sb.AppendLine(" ) M_CODE ")
        sb.AppendLine(" ORDER BY TO_NUMBER (NAIYO_1), CODE_VALUE ")

        MyBase.setParam("BUNRUI", codeBunrui)

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約区分、予約番号取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約区分、予約番号取得SQL</returns>
    Private Function createYoyakuInfoSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")
        sb.AppendLine("     ,CRS_CD ")
        sb.AppendLine("     ,SYUPT_DAY ")
        sb.AppendLine("     ,GOUSYA ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 割引マスタ情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">割引コードマスタ</param>
    ''' <returns>割引マスタ情報取得SQL</returns>
    Private Function createWaribikiMasterDataSql(entity As WaribikiCdMasterEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      APPLICATION_DAY_FROM ")
        sb.AppendLine("     ,APPLICATION_DAY_TO ")
        sb.AppendLine("     ,BIKO ")
        sb.AppendLine("     ,CARRIAGE_WARIBIKI_FLG ")
        sb.AppendLine("     ,CRS_KIND ")
        sb.AppendLine("     ,USE_KBN ")
        sb.AppendLine("     ,WARIBIKI_APPLICATION_NINZU ")
        sb.AppendLine("     ,WARIBIKI_CD ")
        sb.AppendLine("     ,CASE WARIBIKI_KBN WHEN '1' THEN '%' ")
        sb.AppendLine("                        WHEN '2' THEN '\' ")
        sb.AppendLine("      END AS WARIBIKI_KBN ")
        sb.AppendLine("     ,DECODE(WARIBIKI_KBN, '1', WARIBIKI_PER, WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ")
        sb.AppendLine("     ,CASE WHEN NVL(CARRIAGE_WARIBIKI_FLG, '0') <> '1' AND NVL(YOYAKU_WARIBIKI_FLG, '0') <> '1' THEN '当日' ")
        sb.AppendLine("      ELSE '予約時' END AS KBN ")
        sb.AppendLine("     ,WARIBIKI_NAME ")
        sb.AppendLine("     ,WARIBIKI_NAME_KANA ")
        sb.AppendLine("     ,WARIBIKI_TYPE_KBN ")
        sb.AppendLine("     ,YOYAKU_WARIBIKI_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_WARIBIKI_CD ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     NVL(DELETE_DAY, 0) = 0 ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CRS_KIND = " + MyBase.setParam(entity.crsKind.PhysicsName, entity.crsKind.Value, entity.crsKind.DBType, entity.crsKind.IntegerBu, entity.crsKind.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     " + MyBase.setParam(entity.applicationDayFrom.PhysicsName, entity.applicationDayFrom.Value, entity.applicationDayFrom.DBType, entity.applicationDayFrom.IntegerBu, entity.applicationDayFrom.DecimalBu) + " BETWEEN APPLICATION_DAY_FROM AND DECODE(NVL(APPLICATION_DAY_TO, 0), 0, 99999999, APPLICATION_DAY_TO) ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     HOUJIN_GAIKYAKU_KBN = " + MyBase.setParam(entity.houjinGaikyakuKbn.PhysicsName, entity.houjinGaikyakuKbn.Value, entity.houjinGaikyakuKbn.DBType, entity.houjinGaikyakuKbn.IntegerBu, entity.houjinGaikyakuKbn.DecimalBu))
        sb.AppendLine(" ORDER BY WARIBIKI_CD ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' オプション必須選択一覧SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（オプショングループ）Entity</param>
    ''' <returns>オプション必須選択一覧SQL</returns>
    Private Function createRequiredSelectionListSql(entity As CrsLedgerOptionGroupEntity) As String

        Dim mainQuery As String = Me.createOptionListSql()

        Dim whereQuery As New StringBuilder()
        whereQuery.AppendLine(" WHERE ")
        whereQuery.AppendLine("     COG.CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.GOUSYA = " + MyBase.setParam("GOUSYA", entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.REQUIRED_KBN = " + MyBase.setParam("REQUIRED_KBN", entity.requiredKbn.Value, entity.requiredKbn.DBType, entity.requiredKbn.IntegerBu, entity.requiredKbn.DecimalBu))
        whereQuery.AppendLine(" ORDER BY COG.GROUP_NO, CLO.LINE_NO ")

        Dim query As String = mainQuery + whereQuery.ToString()

        Return query
    End Function

    ''' <summary>
    ''' オプション任意選択一覧SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（オプショングループ）Entity</param>
    ''' <returns>オプション任意選択一覧SQL</returns>
    Private Function createAnySelectionListSql(entity As CrsLedgerOptionGroupEntity) As String

        Dim mainQuery As String = Me.createOptionListSql()

        Dim whereQuery As New StringBuilder()
        whereQuery.AppendLine(" WHERE ")
        whereQuery.AppendLine("     COG.CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.GOUSYA = " + MyBase.setParam("GOUSYA", entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        whereQuery.AppendLine("     AND ")
        whereQuery.AppendLine("     COG.REQUIRED_KBN = " + MyBase.setParam("REQUIRED_KBN", entity.requiredKbn.Value, entity.requiredKbn.DBType, entity.requiredKbn.IntegerBu, entity.requiredKbn.DecimalBu))
        whereQuery.AppendLine(" ORDER BY COG.GROUP_NO, CLO.LINE_NO ")

        Dim query As String = mainQuery + whereQuery.ToString()

        Return query
    End Function

    ''' <summary>
    ''' オプション一覧取得SQL作成
    ''' </summary>
    ''' <returns>オプション一覧取得SQL</returns>
    Private Function createOptionListSql() As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CLO.PAYMENT_HOHO ")
        sb.AppendLine("     ,CASE CLO.PAYMENT_HOHO WHEN '1' THEN '事前' ELSE '当日' END AS PAYMENT_HOHO_NM ")
        sb.AppendLine("     ,COG.REQUIRED_KBN ")
        sb.AppendLine("     ,COG.GROUP_NO ")
        sb.AppendLine("     ,COG.OPTION_GROUP_NM ")
        sb.AppendLine("     ,COG.TANKA_KBN ")
        sb.AppendLine("     ,CASE COG.TANKA_KBN ")
        sb.AppendLine("          WHEN '1' THEN '人数単価' ")
        sb.AppendLine("          WHEN '2' THEN 'ルーム単価' ")
        sb.AppendLine("      ELSE '数量単価' END AS TANKA_KBN_NM ")
        sb.AppendLine("     ,CLO.LINE_NO ")
        sb.AppendLine("     ,CLO.OPTIONAL_NAME ")
        'sb.AppendLine("     ,''  AS ADD_CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,TRIM(TO_CHAR(CLO.HANBAI_TANKA, '9,999,999')) AS HANBAI_TANKA ")
        sb.AppendLine("     ,CLO.TAX_KBN ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_OPTION_GROUP COG ")
        sb.AppendLine(" INNER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_OPTION CLO ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     COG.CRS_CD = CLO.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.SYUPT_DAY = CLO.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.GOUSYA = CLO.GOUSYA ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.GROUP_NO = CLO.GROUP_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 送付物マスタ取得SQL作成
    ''' </summary>
    ''' <returns>送付物マスタ取得SQL</returns>
    Private Function createSoufubutsuListSql() As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      SOFUBUTSU_CD ")
        sb.AppendLine("     ,SOFUBUTSU_NM ")
        sb.AppendLine("     ,LINE_NO ")
        sb.AppendLine("     ,'' AS OUTPUT_DATE ")
        'sb.AppendLine("     ,'' AS BIKO ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_SOFUBUTSU ")
        sb.AppendLine(" ORDER BY LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' DELETE SQL作成
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>DELETE SQL</returns>
    Private Function createDeleteSql(yoyakuKbn As String, yoyakuNo As Integer?, tableName As String) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(String.Format(" DELETE FROM {0}", tableName))

        If yoyakuKbn = "W" Then

            sb.AppendLine(" WHERE ")
            sb.AppendLine("     MANAGEMENT_KBN =  " + MyBase.setParam("MANAGEMENT_KBN", yoyakuKbn, OracleDbType.Char, 1, 0))
            sb.AppendLine(" AND ")
            sb.AppendLine("     MANAGEMENT_NO =  " + MyBase.setParam("MANAGEMENT_NO", yoyakuNo, OracleDbType.Decimal, 9, 0))
        Else

            sb.AppendLine(" WHERE ")
            sb.AppendLine("     YOYAKU_KBN =  " + MyBase.setParam("YOYAKU_KBN", yoyakuKbn, OracleDbType.Char, 1, 0))
            sb.AppendLine(" AND ")
            sb.AppendLine("     YOYAKU_NO =  " + MyBase.setParam("YOYAKU_NO", yoyakuNo, OracleDbType.Decimal, 9, 0))
        End If

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報取得SQL</returns>
    Private Function createYoyakuInfoDataSql(entity As YoyakuInfoBasicEntity) As String

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
        sb.AppendLine("     ,CLB.SAIKOU_DAY ")
        sb.AppendLine("     ,CLB.UNKYU_CONTACT_DAY ")
        sb.AppendLine("     ,CLB.RETURN_DAY ")
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ")
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ")
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
        sb.AppendLine("     ,CLB.TYUIJIKOU ")
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ")
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ")
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ")
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ")
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ")
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ")
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
        sb.AppendLine("     ,CLB.CANCEL_RYOU_KBN ")
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ")
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ")
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ")
        sb.AppendLine("     ,CLB.TEJIMAI_DAY ")

        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ")
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ")
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ")
        sb.AppendLine("     ,CLB.AGE_FLG ")
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ")
        sb.AppendLine("     ,CLB.TEL_FLG ")
        sb.AppendLine("     ,CLB.ADDRESS_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ")

        sb.AppendLine("     ,CLB.YOYAKU_STOP_FLG ")
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ")
        sb.AppendLine("     ,CLB.USING_FLG ")
        sb.AppendLine("     ,CLB.ZASEKI_HIHYOJI_FLG")
        sb.AppendLine("     ,CLB.TOJITU_KOKUCHI_FLG")
        sb.AppendLine("     ,YIB.YOYAKU_KBN ")
        sb.AppendLine("     ,YIB.YOYAKU_NO ")
        sb.AppendLine("     ,YIB.SEIKI_CHARGE_ALL_GAKU ")
        sb.AppendLine("     ,YIB.WARIBIKI_ALL_GAKU ")
        sb.AppendLine("     ,YIB.ZASEKI ")
        sb.AppendLine("     ,YIB.SEISAN_HOHO ")
        sb.AppendLine("     ,SEI.CODE_NAME AS SEISAN_HOHO_NM ")
        sb.AppendLine("     ,YIB.NYUKINGAKU_SOKEI ")
        sb.AppendLine("     ,YIB.CANCEL_RYOU_KEI ")
        sb.AppendLine("     ,YIB.SUB_SEAT_WAIT_FLG ")
        sb.AppendLine("     ,YIB.MOTO_YOYAKU_KBN ")
        sb.AppendLine("     ,YIB.MOTO_YOYAKU_NO ")
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
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_1 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_2 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_3 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_4 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_5 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_6 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_7 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_8 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_9 ")
        sb.AppendLine("     ,YIB.MESSAGE_CHECK_FLG_10 ")
        sb.AppendLine("     ,YIB.MEDIA_CD ")
        sb.AppendLine("     ,YIB.AGENT_CD ")
        sb.AppendLine("     ,YIB.AGENT_NAME_KANA ")
        sb.AppendLine("     ,YIB.AGENT_NM ")
        sb.AppendLine("     ,YIB.AGENT_SEISAN_KBN ")
        sb.AppendLine("     ,YIB.AGENT_TEL_NO ")
        sb.AppendLine("     ,YIB.AGENT_TANTOSYA ")
        sb.AppendLine("     ,YIB.TOURS_NO ")
        sb.AppendLine("     ,YIB.KOKUSEKI ")
        sb.AppendLine("     ,YIB.SEX_BETU ")
        sb.AppendLine("     ,YIB.SURNAME ")
        sb.AppendLine("     ,YIB.NAME ")
        sb.AppendLine("     ,YIB.SURNAME_KJ ")
        sb.AppendLine("     ,YIB.NAME_KJ ")
        sb.AppendLine("     ,YIB.TEL_NO_1 ")
        sb.AppendLine("     ,YIB.TEL_NO_2 ")
        sb.AppendLine("     ,YIB.MAIL_ADDRESS ")
        sb.AppendLine("     ,YIB.MAIL_SENDING_KBN ")
        sb.AppendLine("     ,YIB.YUBIN_NO ")
        sb.AppendLine("     ,YIB.ADDRESS_1 ")
        sb.AppendLine("     ,YIB.ADDRESS_2 ")
        sb.AppendLine("     ,YIB.ADDRESS_3 ")
        sb.AppendLine("     ,YIB.MOSHIKOMI_HOTEL_FLG ")
        sb.AppendLine("     ,YIB.YYKMKS ")
        sb.AppendLine("     ,YIB.FURIKOMIYOSHI_YOHI_FLG ")
        sb.AppendLine("     ,YIB.SEIKYUSYO_YOHI_FLG ")
        sb.AppendLine("     ,YIB.RELATION_YOYAKU_KBN ")
        sb.AppendLine("     ,YIB.RELATION_YOYAKU_NO ")
        sb.AppendLine("     ,YIB.ADD_CHARGE_MAEBARAI_KEI ")
        sb.AppendLine("     ,YIB.ADD_CHARGE_TOJITU_PAYMENT_KEI ")
        sb.AppendLine("     ,YIB.ENTRY_DAY ")
        sb.AppendLine("     ,YIB.YOYAKU_JI_AGENT_CD ")
        sb.AppendLine("     ,YIB.YOYAKU_JI_AGENT_NAME ")
        sb.AppendLine("     ,YIB.JYOSEI_SENYO ")
        sb.AppendLine("     ,YIB.AIBEYA_FLG ")
        sb.AppendLine("     ,YIB.HAKKEN_NAIYO ")
        sb.AppendLine("     ,YIB.CANCEL_FLG ")
        sb.AppendLine("     ,YIB.STATE ")
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_URIAGE ")
        sb.AppendLine("     ,YIB.TORIATUKAI_FEE_CANCEL ") '
        sb.AppendLine("     ,YIB.YOYAKU_UKETUKE_KBN ")
        sb.AppendLine("     ,YIB.NYUUKIN_SITUATION_KBN ")
        sb.AppendLine("     ,YIB.CANCEL_FLG ")
        sb.AppendLine("     ,YIB.DELETE_DAY ")
        sb.AppendLine("     ,YIB.USING_FLG ")
        sb.AppendLine("     , NVL(IYTM.OPERATION_TAISHO_FLG, '') AS OPETAISHOFLG ")
        sb.AppendLine("     , NVL(IYTM.INVALID_FLG, '') AS INVFLG ")
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
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE SEI ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     SEI.CODE_BUNRUI = '027' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     SEI.CODE_VALUE = YIB.SEISAN_HOHO ")
        sb.AppendLine("     LEFT OUTER JOIN T_YOYAKU_INFO_PICKUP YP ")                 '予約情報（ピックアップ）
        sb.AppendLine("         ON  YP.YOYAKU_KBN = YIB.YOYAKU_KBN ")
        sb.AppendLine("         AND YP.YOYAKU_NO = YIB.YOYAKU_NO ")
        sb.AppendLine("     LEFT OUTER JOIN M_PICKUP_HOTEL PH ")                       'ピックアップホテルマスタ
        sb.AppendLine("         ON PH.PICKUP_HOTEL_CD = YP.PICKUP_HOTEL_CD ")
        sb.AppendLine("     LEFT OUTER JOIN ")
        sb.AppendLine("         T_INT_YOYAKU_TRAN_MANAGEMENT IYTM ")
        sb.AppendLine("     ON ")
        sb.AppendLine("         YIB.YOYAKU_KBN = IYTM.YOYAKU_KBN ")
        sb.AppendLine("         AND ")
        sb.AppendLine("         YIB.YOYAKU_NO = IYTM.YOYAKU_NO ")
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
    ''' メモ情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（メモ）Entity</param>
    ''' <returns>メモ情報取得SQL</returns>
    Private Function createYoyakuMemoInfoListSql(entity As YoyakuInfoMemoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YIM.SYSTEM_UPDATE_DAY ")
        sb.AppendLine("     ,YIM.MEMO_KBN ")
        sb.AppendLine("     ,KBN.CODE_NAME AS MEMO_KBN_NM ")
        sb.AppendLine("     ,YIM.MEMO_BUNRUI ")
        sb.AppendLine("     ,BUN.CODE_NAME AS MEMO_BUNRUI_NM ")
        sb.AppendLine("     ,YIM.NAIYO ")
        sb.AppendLine("     ,YIM.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SER.USER_NAME ")
        sb.AppendLine("     ,YIM.EDABAN ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_MEMO YIM ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER SER ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     SER.COMPANY_CD = '0001' ")
        sb.AppendLine(" AND ")
        sb.AppendLine("     SER.USER_ID = YIM.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE KBN ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     KBN.CODE_BUNRUI = '004' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     KBN.CODE_VALUE = YIM.MEMO_KBN ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE BUN ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     BUN.CODE_BUNRUI = '005' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     BUN.CODE_VALUE = YIM.MEMO_BUNRUI ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     NVL(YIM.DELETE_DATE, 0) = 0 ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIM.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIM.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine(" ORDER BY YIM.SYSTEM_UPDATE_DAY, YIM.EDABAN ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約割引一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（割引）Entity</param>
    ''' <returns>予約割引一覧取得SQL</returns>
    Private Function createYoyakuWaribikiListSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YIW.WARIBIKI_CD ")
        sb.AppendLine("     ,YIW.WARIBIKI_CD AS WARIBIKI_NAME ")
        sb.AppendLine("     ,CAST(YIW.KBN_NO AS VARCHAR2(3)) AS CHARGE_KBN ")
        sb.AppendLine("     ,YIW.CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     ,DECODE(YIW.WARIBIKI_KBN, '1', YIW.WARIBIKI_PER, YIW.WARIBIKI_KINGAKU) As WARIBIKI_KINGAKU ")
        sb.AppendLine("     ,CASE YIW.WARIBIKI_KBN WHEN '1' THEN '%' ")
        sb.AppendLine("                            WHEN '2' THEN '\' ")
        sb.AppendLine("      END AS WARIBIKI_KBN ")
        sb.AppendLine("     ,CASE WHEN NVL(YIW.CARRIAGE_WARIBIKI_FLG, '0') <> '1' AND NVL(YIW.YOYAKU_WARIBIKI_FLG, '0') <> '1' THEN '当日'  ")
        sb.AppendLine("      ELSE '予約時' END AS KBN ")
        sb.AppendLine("     ,NVL(YIW.WARIBIKI_APPLICATION_NINZU_1, 0) +  ")
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_2, 0) +  ")
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_3, 0) +  ")
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_4, 0) +  ")
        sb.AppendLine("      NVL(YIW.WARIBIKI_APPLICATION_NINZU_5, 0) AS WARIBIKI_NINZU ")
        sb.AppendLine("     ,(NVL(YIW.WARIBIKI_TANKA_1, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_1, 0)) +  ")
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_2, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_2, 0)) +  ")
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_3, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_3, 0)) +  ")
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_4, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_4, 0)) +  ")
        sb.AppendLine("      (NVL(YIW.WARIBIKI_TANKA_5, 0) * NVL(YIW.WARIBIKI_APPLICATION_NINZU_5, 0)) AS WARIBIKI_TOTAL_GAKU ")
        sb.AppendLine("     ,YIW.WARIBIKI_BIKO ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU ")
        sb.AppendLine("     ,YIW.CARRIAGE_WARIBIKI_FLG ")
        sb.AppendLine("     ,YIW.WARIBIKI_USE_FLG ")
        sb.AppendLine("     ,YIW.YOYAKU_WARIBIKI_FLG ")
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_1 ")
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_2 ")
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_3 ")
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_4 ")
        sb.AppendLine("     ,YIW.WARIBIKI_TANKA_5 ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_1 ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_2 ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_3 ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_4 ")
        sb.AppendLine("     ,YIW.WARIBIKI_APPLICATION_NINZU_5 ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine(" INNER JOIN ")
        sb.AppendLine("     T_YOYAKU_INFO_WARIBIKI YIW ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YIW.YOYAKU_KBN = YIB.YOYAKU_KBN ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIW.YOYAKU_NO = YIB.YOYAKU_NO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIB.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIB.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約振込情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（振込）Entity</param>
    ''' <returns>予約振込情報取得SQL</returns>
    Private Function createYoyakuHurikomiInfoSql(entity As YoyakuInfoHurikomiEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YUBIN_NO_HURIKOMI_NIN ")
        sb.AppendLine("     ,ADDRESS_1_HURIKOMI_NIN ")
        sb.AppendLine("     ,ADDRESS_2_HURIKOMI_NIN ")
        sb.AppendLine("     ,ADDRESS_3_HURIKOMI_NIN ")
        sb.AppendLine("     ,NM_1_HURIKOMI_NIN ")
        sb.AppendLine("     ,NM_2_HURIKOMI_NIN ")
        sb.AppendLine("     ,DOUJOU_FLG_SOFU_SAKI ")
        sb.AppendLine("     ,YUBIN_NO_SOFU_SAKI ")
        sb.AppendLine("     ,ADDRESS_1_SOFU_SAKI ")
        sb.AppendLine("     ,ADDRESS_2_SOFU_SAKI ")
        sb.AppendLine("     ,ADDRESS_3_SOFU_SAKI ")
        sb.AppendLine("     ,NM_1_SOFU_SAKI ")
        sb.AppendLine("     ,NM_2_SOFU_SAKI ")
        sb.AppendLine("     ,DOUJOU_FLG_HURIKOMI_NIN ")
        sb.AppendLine("     ,SEIKYU_GAKU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_HURIKOMI ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約名簿一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（名簿）Entity</param>
    ''' <returns>予約名簿一覧取得SQL</returns>
    Private Function createYoyakuMeiboListSql(entity As YoyakuInfoMeiboEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      SURNAME ")
        sb.AppendLine("     ,NAME ")
        sb.AppendLine("     ,SEX_BETU ")
        sb.AppendLine("     ,TO_DATE(BIRTHDAY,'YYYYMMDD') AS BIRTHDAY ")
        sb.AppendLine("     ,TRUNC((TO_CHAR(SYSDATE, 'YYYYMMDD') - TO_CHAR(TO_DATE(BIRTHDAY, 'YYYY/MM/DD'), 'YYYYMMDD'))/10000) AS AGE ")
        sb.AppendLine("     ,TEL_NO_1　AS TEL_NO ")
        sb.AppendLine("     ,YUBIN_NO ")
        sb.AppendLine("     ,ADDRESS_1 ")
        sb.AppendLine("     ,ADDRESS_2 ")
        sb.AppendLine("     ,ADDRESS_3 ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_MEIBO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約送付物一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（送付物）Entity</param>
    ''' <returns>予約送付物一覧取得SQL</returns>
    Private Function createYoyakuSofubutsuListSql(entity As YoyakuInfoSofubutsuEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      MSO.SOFUBUTSU_CD ")
        sb.AppendLine("     ,MSO.SOFUBUTSU_NM ")
        sb.AppendLine("     ,CASE MSO.SOFUBUTSU_CD  ")
        sb.AppendLine("           WHEN '01' THEN (SELECT TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME) FROM T_SEIKYUSYO_ISSUE ")
        sb.AppendLine("                          WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                            AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                     ) ")
        sb.AppendLine("           WHEN '02' THEN (SELECT OUTDATE FROM (SELECT TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME) AS OUTDATE FROM T_FURIKOMIHYO_CREATE_HISTORY ")
        sb.AppendLine("                                                   WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                                                     AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                                 ORDER BY UPDATE_DAY DESC, UPDATE_TIME DESC) WHERE rownum <= 1) ")
        sb.AppendLine("           WHEN '03' THEN (SELECT OUTDATE FROM (SELECT (TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME)) AS OUTDATE FROM T_RECEIPT ")
        sb.AppendLine("                                                 WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                                                   AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                                 ORDER BY PRINT_NLGSQN DESC) WHERE rownum <= 1) ")
        sb.AppendLine("           WHEN '04' THEN (SELECT OUTDATE FROM (SELECT (TO_CHAR(UPDATE_DAY) || TO_CHAR(UPDATE_TIME)) AS OUTDATE FROM T_ZASEKI_RESERVE_KEN_HIKIKAESHO_OUT ")
        sb.AppendLine("                                                 WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                                                   AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                                 ORDER BY PROCESS_DAY DESC) WHERE rownum <= 1) ")
        sb.AppendLine("           WHEN '05' THEN (SELECT CASE NVL(YOYAKU_KAKUNIN_DAY, 0) WHEN 0 THEN '' ELSE TO_CHAR(TO_DATE(TO_CHAR(YOYAKU_KAKUNIN_DAY), 'yyyyMMdd'), 'yyyy/MM/dd') END FROM T_YOYAKU_INFO_BASIC ")
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                              AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                     ) ")
        sb.AppendLine("           WHEN '06' THEN (SELECT CASE NVL(SAIKOU_KAKUTEI_GUIDE_OUT_DAY, 0) WHEN 0 THEN '' ELSE TO_CHAR(TO_DATE(TO_CHAR(SAIKOU_KAKUTEI_GUIDE_OUT_DAY), 'yyyyMMdd'), 'yyyy/MM/dd') END FROM T_YOYAKU_INFO_BASIC ")
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                              AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                     ) ")
        sb.AppendLine("           WHEN '07' THEN (SELECT TO_CHAR(SYSTEM_UPDATE_DAY) FROM T_BUS_COMPANY_KAKUTEI_NOTIFICATION_OUT ")
        sb.AppendLine("                            WHERE YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("                              AND YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("                                     ) ")
        sb.AppendLine("      END AS OUTPUT_DATE ")
        sb.AppendLine("     ,(SELECT BIKO FROM T_YOYAKU_INFO_SOFUBUTSU YIS ")
        sb.AppendLine("       WHERE YIS.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("         AND YIS.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine("         AND YIS.SOFUBUTSU_CD = MSO.SOFUBUTSU_CD ")
        sb.AppendLine("      ) AS BIKO  ")
        sb.AppendLine("     ,MSO.LINE_NO ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_SOFUBUTSU MSO ")
        sb.AppendLine(" ORDER BY MSO.LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約送付先一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（送付先）Entity</param>
    ''' <returns>予約送付先一覧取得SQL</returns>
    Private Function createYoyakuSofusakiListSql(entity As YoyakuInfoSofusakiEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YIS.AGENT_CD ")
        sb.AppendLine("     ,AGT.COMPANY_NAME ")
        sb.AppendLine("     ,AGT.BRANCH_NAME ")
        sb.AppendLine("     ,YIS.NAME_BF ")
        sb.AppendLine("     ,YIS.YUBIN_NO ")
        sb.AppendLine("     ,YIS.ADDRESS_1 ")
        sb.AppendLine("     ,YIS.ADDRESS_2 ")
        sb.AppendLine("     ,YIS.ADDRESS_3 ")
        sb.AppendLine("     ,YIS.NM_1 ")
        sb.AppendLine("     ,YIS.NM_2 ")
        sb.AppendLine("     ,YIS.SOFU_BUSU ")
        sb.AppendLine("     ,YIS.LINE_NO ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_SOFUSAKI YIS ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_AGENT AGT ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     AGT.AGENT_CD = YIS.AGENT_CD ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIS.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIS.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine(" ORDER BY YIS.LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 入返金一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">入返金情報Entity</param>
    ''' <returns>入返金一覧取得SQL</returns>
    Private Function createNyuhenkinListSql(entity As NyuukinInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      PROCESS_DATE ")
        sb.AppendLine("     ,CASE HAKKEN_HURIKOMI_KBN WHEN '3' THEN NYUUKIN_GAKU_1 ")
        sb.AppendLine("                               WHEN '7' THEN NYUUKIN_GAKU_2 ")
        sb.AppendLine("      END AS NYUKIN ")
        sb.AppendLine("     ,CASE HAKKEN_HURIKOMI_KBN WHEN '4' THEN NYUUKIN_GAKU_1 ")
        sb.AppendLine("                               WHEN '8' THEN NYUUKIN_GAKU_2 ")
        sb.AppendLine("      END AS HENKIN ")
        sb.AppendLine("     ,HURIKOMI_SAKI_KOZA_NAME ")
        sb.AppendLine("     ,CASE WHEN HAKKEN_HURIKOMI_KBN = '3' OR HAKKEN_HURIKOMI_KBN = '7' ")
        sb.AppendLine("                THEN CASE HURIKOMI_KBN WHEN 'G' THEN '銀行' ")
        sb.AppendLine("                                       WHEN 'Y' THEN '郵便局' ")
        sb.AppendLine("                                       WHEN 'K' THEN 'コンビニ' ")
        sb.AppendLine("                     END ")
        sb.AppendLine("           WHEN HAKKEN_HURIKOMI_KBN = '4' OR HAKKEN_HURIKOMI_KBN = '8' ")
        sb.AppendLine("                THEN CASE HURIKOMI_KBN WHEN 'G' THEN '振込' ")
        sb.AppendLine("                                       WHEN 'Y' THEN '書留' ")
        sb.AppendLine("                                       WHEN 'K' THEN '窓口' ")
        sb.AppendLine("                     END ")
        sb.AppendLine("      END AS HURIKOMI_KBN ")
        sb.AppendLine("     ,NICOS_UKETUKE_NO ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_NYUUKIN_INFO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))
        sb.AppendLine(" ORDER BY SEQ ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約オプション一覧取得SQL作成
    ''' </summary>
    ''' <param name="crsEntity">コース台帳（オプション）Entity</param>
    ''' <param name="yoyakuentity">予約情報（オプション）Entity</param>
    ''' <returns>予約オプション一覧取得SQL</returns>
    Private Function createYoyakuOptionListSql(crsEntity As CrsLedgerOptionGroupEntity, yoyakuentity As YoyakuInfoOptionEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CLO.PAYMENT_HOHO ")
        sb.AppendLine("     ,CASE CLO.PAYMENT_HOHO WHEN '1' THEN '事前' ELSE '当日' END AS PAYMENT_HOHO_NM ")
        sb.AppendLine("     ,COG.REQUIRED_KBN ")
        sb.AppendLine("     ,COG.GROUP_NO ")
        sb.AppendLine("     ,COG.OPTION_GROUP_NM ")
        sb.AppendLine("     ,COG.TANKA_KBN ")
        sb.AppendLine("     ,CASE COG.TANKA_KBN ")
        sb.AppendLine("          WHEN '1' THEN '人数単価' ")
        sb.AppendLine("          WHEN '2' THEN 'ルーム単価' ")
        sb.AppendLine("      ELSE '数量単価' END AS TANKA_KBN_NM ")
        sb.AppendLine("     ,CLO.LINE_NO ")
        sb.AppendLine("     ,CLO.OPTIONAL_NAME ")
        sb.AppendLine("     ,TRIM(TO_CHAR(CLO.HANBAI_TANKA, '9,999,999')) AS HANBAI_TANKA ")
        sb.AppendLine("     ,CLO.TAX_KBN ")
        sb.AppendLine("     ,YIO.ADD_CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,YIO.EXISTS_YOYAKU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_CRS_LEDGER_OPTION_GROUP COG ")
        sb.AppendLine(" INNER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_OPTION CLO ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     COG.CRS_CD = CLO.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.SYUPT_DAY = CLO.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.GOUSYA = CLO.GOUSYA ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.GROUP_NO = CLO.GROUP_NO ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     (SELECT ")
        sb.AppendLine("          YOYAKU_KBN ")
        sb.AppendLine("         ,YOYAKU_NO ")
        sb.AppendLine("         ,GROUP_NO ")
        sb.AppendLine("         ,LINE_NO ")
        sb.AppendLine("         ,ADD_CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("         ,'1' AS EXISTS_YOYAKU ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("         T_YOYAKU_INFO_OPTION ")
        sb.AppendLine("     WHERE ")
        sb.AppendLine("         YOYAKU_KBN = " + MyBase.setParam(yoyakuentity.yoyakuKbn.PhysicsName, yoyakuentity.yoyakuKbn.Value, yoyakuentity.yoyakuKbn.DBType, yoyakuentity.yoyakuKbn.IntegerBu, yoyakuentity.yoyakuKbn.DecimalBu))
        sb.AppendLine("         AND ")
        sb.AppendLine("         YOYAKU_NO = " + MyBase.setParam(yoyakuentity.yoyakuNo.PhysicsName, yoyakuentity.yoyakuNo.Value, yoyakuentity.yoyakuNo.DBType, yoyakuentity.yoyakuNo.IntegerBu, yoyakuentity.yoyakuNo.DecimalBu))
        sb.AppendLine("     ) YIO ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YIO.GROUP_NO = COG.GROUP_NO ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIO.LINE_NO = CLO.LINE_NO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     COG.CRS_CD = " + MyBase.setParam("CRS_CD", crsEntity.crsCd.Value, crsEntity.crsCd.DBType, crsEntity.crsCd.IntegerBu, crsEntity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", crsEntity.syuptDay.Value, crsEntity.syuptDay.DBType, crsEntity.syuptDay.IntegerBu, crsEntity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.GOUSYA = " + MyBase.setParam("GOUSYA", crsEntity.gousya.Value, crsEntity.gousya.DBType, crsEntity.gousya.IntegerBu, crsEntity.gousya.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     COG.REQUIRED_KBN = " + MyBase.setParam("REQUIRED_KBN", crsEntity.requiredKbn.Value, crsEntity.requiredKbn.DBType, crsEntity.requiredKbn.IntegerBu, crsEntity.requiredKbn.DecimalBu))
        sb.AppendLine(" ORDER BY COG.REQUIRED_KBN, COG.GROUP_NO, CLO.LINE_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' ホリデーT管理（コースコード）取得SQL作成
    ''' </summary>
    ''' <param name="entity">ホリデーT管理（コースコード）Entity</param>
    ''' <returns>ホリデーT管理（コースコード）取得SQL</returns>
    Private Function createHolidayCrsSql(entity As HolidayCrsCdEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CRS_CD ")
        sb.AppendLine("     ,DELETE_DAY ")
        sb.AppendLine("     ,ENTRY_DAY ")
        sb.AppendLine("     ,ENTRY_PERSON_CD ")
        sb.AppendLine("     ,ENTRY_PGMID ")
        sb.AppendLine("     ,ENTRY_TIME ")
        sb.AppendLine("     ,UPDATE_DAY ")
        sb.AppendLine("     ,UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UPDATE_PGMID ")
        sb.AppendLine("     ,UPDATE_TIME ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_HOLIDAY_CRS_CD ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' ホリデーT管理（適用日）取得SQL作成
    ''' </summary>
    ''' <param name="entity">ホリデーT管理（適用日）Entity</param>
    ''' <returns>ホリデーT管理（適用日）取得SQL</returns>
    Private Function createHolidayApplicationDaySql(entity As HolidayApplicationDayEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      APPLICATION_DAY_FROM ")
        sb.AppendLine("     ,APPLICATION_DAY_TO ")
        sb.AppendLine("     ,DELETE_DAY ")
        sb.AppendLine("     ,ENTRY_DAY ")
        sb.AppendLine("     ,ENTRY_PERSON_CD ")
        sb.AppendLine("     ,ENTRY_PGMID ")
        sb.AppendLine("     ,ENTRY_TIME ")
        sb.AppendLine("     ,UPDATE_DAY ")
        sb.AppendLine("     ,UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UPDATE_PGMID ")
        sb.AppendLine("     ,UPDATE_TIME ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_HOLIDAY_APPLICATION_DAY ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     APPLICATION_DAY_FROM <= " + MyBase.setParam(entity.applicationDayFrom.PhysicsName, entity.applicationDayFrom.Value, entity.applicationDayFrom.DBType, entity.applicationDayFrom.IntegerBu, entity.applicationDayFrom.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     APPLICATION_DAY_TO >= " + MyBase.setParam(entity.applicationDayTo.PhysicsName, entity.applicationDayTo.Value, entity.applicationDayTo.DBType, entity.applicationDayTo.IntegerBu, entity.applicationDayTo.DecimalBu))

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
    ''' 予約情報使用中フラグ更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報使用中フラグ更新SQ</returns>
    Private Function createYoyakuUsingFlagUpdateSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("     USING_FLG = '' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報使用中フラグ更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報使用中フラグ更新SQ</returns>
    Private Function createYoyakuUsingFlagOnUpdateSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("     USING_FLG = 'Y' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報使用中フラグ取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>予約情報使用中フラグ取得SQL</returns>
    Private Function createYoyakuUsingFlagSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("     USING_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

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
    ''' 部屋残数更新SQL作成
    ''' </summary>
    ''' <param name="crsZasekiData">コース情報検索条件群</param>
    ''' <param name="zasekiData">座席情報</param>
    ''' <param name="yoyakuEntity">予約情報（基本）Entity</param>
    ''' <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    ''' <param name="isTorikeshi">取消フラグ</param>
    ''' <returns>部屋残数更新SQL</returns>
    Private Function createCrsRoomUpdateSql(crsZasekiData As Hashtable, zasekiData As DataTable, yoyakuEntity As YoyakuInfoBasicEntity, crsInfoBasicEntity As CrsLedgerBasicEntity, isTorikeshi As Boolean) As String

        Dim syuptDay As Integer = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("SYUPT_DAY"))

        Dim shoriDt As DateTime = DateTime.Parse(crsZasekiData.Item("SYSTEM_UPDATE_DAY").ToString())
        Dim shoribi As Integer = CommonRegistYoyaku.convertObjectToInteger(shoriDt.ToString("yyyyMMdd"))

        Dim query As String = CommonRegistYoyaku.ValueEmpty

        ' 出発日がと当日の場合
        If syuptDay = shoribi Then

            Return query
        End If

        ' 定員制フラグが空以外の場合
        If zasekiData IsNot Nothing AndAlso String.IsNullOrEmpty(zasekiData.Rows(0)("TEIINSEI_FLG").ToString()) = False Then

            Return query
        End If


        If String.IsNullOrEmpty(yoyakuEntity.aibeyaFlg.Value) = True Then
            ' 相部屋以外の場合

            query = Me.createCrsRoomAibeyaNashiUpdateSql(crsZasekiData, zasekiData, yoyakuEntity, isTorikeshi)
        Else
            ' 相部屋の場合

            query = Me.createCrsRoomAibeyaAriUpdateSql(crsZasekiData, zasekiData, yoyakuEntity, crsInfoBasicEntity, isTorikeshi)
        End If

        Return query
    End Function

    ''' <summary>
    ''' 部屋残数更新SQL作成
    ''' </summary>
    ''' <param name="crsZasekiData">コース台帳検索条件群</param>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="yoyakuEntity">予約情報（基本）Entity</param>
    ''' <param name="isTorikeshi">取消フラグ</param>
    ''' <returns>部屋残数更新SQL</returns>
    Private Function createCrsRoomAibeyaNashiUpdateSql(crsZasekiData As Hashtable, zasekiData As DataTable, yoyakuEntity As YoyakuInfoBasicEntity, isTorikeshi As Boolean) As String

        Dim entity As New CrsLedgerBasicEntity()
        entity.crsCd.Value = crsZasekiData.Item("CRS_CD").ToString()
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("SYUPT_DAY"))
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("GOUSYA"))

        entity.roomZansuOneRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu1.Value, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi)
        entity.roomZansuTwoRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu2.Value, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi)
        entity.roomZansuThreeRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu3.Value, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi)
        entity.roomZansuFourRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu4.Value, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi)
        entity.roomZansuFiveRoom.Value = Me.calcCrsRoomAibeyaNashi(zasekiData, yoyakuEntity.roomingBetuNinzu5.Value, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi)

        ' 予約部屋数総数算出
        Dim totalRoomsu As Integer = Me.calcTotalYoyakuRoomsu(yoyakuEntity)

        If isTorikeshi Then

            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) - totalRoomsu
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) + totalRoomsu
        Else

            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) + totalRoomsu
            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) - totalRoomsu
        End If

        ' 部屋残数算出
        entity.roomZansuOneRoom.Value = Me.calcRoomZansu(zasekiData, "CRS_BLOCK_ONE_1R", entity.roomZansuOneRoom.Value, entity.roomZansuSokei.Value)
        entity.roomZansuTwoRoom.Value = Me.calcRoomZansu(zasekiData, "CRS_BLOCK_TWO_1R", entity.roomZansuTwoRoom.Value, entity.roomZansuSokei.Value)
        entity.roomZansuThreeRoom.Value = Me.calcRoomZansu(zasekiData, "CRS_BLOCK_THREE_1R", entity.roomZansuThreeRoom.Value, entity.roomZansuSokei.Value)
        entity.roomZansuFourRoom.Value = Me.calcRoomZansu(zasekiData, "CRS_BLOCK_FOUR_1R", entity.roomZansuFourRoom.Value, entity.roomZansuSokei.Value)
        entity.roomZansuFiveRoom.Value = Me.calcRoomZansu(zasekiData, "CRS_BLOCK_FIVE_1R", entity.roomZansuFiveRoom.Value, entity.roomZansuSokei.Value)

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      ROOM_ZANSU_ONE_ROOM =  " + MyBase.setParam(entity.roomZansuOneRoom.PhysicsName, entity.roomZansuOneRoom.Value, entity.roomZansuOneRoom.DBType, entity.roomZansuOneRoom.IntegerBu, entity.roomZansuOneRoom.DecimalBu))
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
    ''' 部屋残数算出
    ''' </summary>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="roomingBetuNinzu">予約部屋数</param>
    ''' <param name="crsBlockColName">コースブロックカラム名</param>
    ''' <param name="roomZansuColName">部屋残数カラム名</param>
    ''' <param name="isTorikeshi">取消フラグ</param>
    ''' <returns>部屋残数</returns>
    Private Function calcCrsRoomAibeyaNashi(zasekiData As DataTable, roomingBetuNinzu As Integer?, crsBlockColName As String, roomZansuColName As String, isTorikeshi As Boolean) As Integer

        Dim crsBlockRoomSu As Integer = CommonRegistYoyaku.ZERO
        Dim crsRoomZanSu As Integer = CommonRegistYoyaku.ZERO

        Integer.TryParse(zasekiData.Rows(0)(crsBlockColName).ToString(), crsBlockRoomSu)
        Integer.TryParse(zasekiData.Rows(0)(roomZansuColName).ToString(), crsRoomZanSu)

        If crsBlockRoomSu = CommonRegistYoyaku.ZERO Then
            ' コースブロック管理されてない部屋残数は、変動なし
            Return crsRoomZanSu
        End If

        Dim fugoHanten As Integer = -1
        If isTorikeshi Then

            fugoHanten = 1
        End If

        Dim yoyakuRoomsu As Integer = CommonRegistYoyaku.convertObjectToInteger(roomingBetuNinzu)
        Dim roomZansu As Integer = crsRoomZanSu + (yoyakuRoomsu * fugoHanten)

        Return roomZansu
    End Function

    ''' <summary>
    ''' 予約部屋数総数算出
    ''' </summary>
    ''' <param name="yoyakuEntity">予約情報（基本）Entity</param>
    ''' <returns>予約部屋数総数</returns>
    Private Function calcTotalYoyakuRoomsu(yoyakuEntity As YoyakuInfoBasicEntity) As Integer

        Dim yoyakuRoom1 As Integer = 0
        Dim yoyakuRoom2 As Integer = 0
        Dim yoyakuRoom3 As Integer = 0
        Dim yoyakuRoom4 As Integer = 0
        Dim yoyakuRoom5 As Integer = 0

        Integer.TryParse(yoyakuEntity.roomingBetuNinzu1.Value.ToString(), yoyakuRoom1)
        Integer.TryParse(yoyakuEntity.roomingBetuNinzu2.Value.ToString(), yoyakuRoom2)
        Integer.TryParse(yoyakuEntity.roomingBetuNinzu3.Value.ToString(), yoyakuRoom3)
        Integer.TryParse(yoyakuEntity.roomingBetuNinzu4.Value.ToString(), yoyakuRoom4)
        Integer.TryParse(yoyakuEntity.roomingBetuNinzu5.Value.ToString(), yoyakuRoom5)

        Dim totalYoyakuRoom As Integer = yoyakuRoom1 + yoyakuRoom2 + yoyakuRoom3 + yoyakuRoom4 + yoyakuRoom5

        Return totalYoyakuRoom
    End Function

    ''' <summary>
    ''' 部屋残数算出
    ''' </summary>
    ''' <param name="zasekiData">座席情報</param>
    ''' <param name="crsBlockColName">コースブロックカラム名</param>
    ''' <param name="roomZansu">部屋残数</param>
    ''' <param name="roomZansuSoukei">部屋残数総計</param>
    ''' <returns></returns>
    Private Function calcRoomZansu(zasekiData As DataTable, crsBlockColName As String, roomZansu As Integer?, roomZansuSoukei As Integer?) As Integer

        ' コースブロック数
        Dim crsBlockRoomSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName))

        If crsBlockRoomSu = CommonRegistYoyaku.ZERO Then
            ' コースブロック管理されてない部屋残数は、特になし
            Return CInt(roomZansu)
        End If

        roomZansu = CommonRegistYoyaku.convertObjectToInteger(roomZansu)
        roomZansuSoukei = CommonRegistYoyaku.convertObjectToInteger(roomZansuSoukei)

        If roomZansu > roomZansuSoukei Then
            ' 部屋残数が部屋残数総計を超えたい場合、部屋残数を部屋残数総計にする
            roomZansu = roomZansuSoukei
        End If

        Return CInt(roomZansu)
    End Function

    ''' <summary>
    ''' 部屋残数更新SQL作成
    ''' </summary>
    ''' <param name="crsZasekiData">コース台帳検索条件群</param>
    ''' <param name="zasekiData">コース台帳座席情報</param>
    ''' <param name="yoyakuEntity">予約情報（基本）Entity</param>
    ''' <param name="crsInfoBasicEntity">コース台帳（基本）Entity</param>
    ''' <param name="isTorikeshi">取消フラグ</param>
    ''' <returns>部屋残数更新SQL</returns>
    Private Function createCrsRoomAibeyaAriUpdateSql(crsZasekiData As Hashtable, zasekiData As DataTable, yoyakuEntity As YoyakuInfoBasicEntity, crsInfoBasicEntity As CrsLedgerBasicEntity, isTorikeshi As Boolean) As String

        ' ROOM MAX定員数取得
        Dim roomMaxCap As Double = Me.getRoomMaxTein(crsZasekiData)

        ' 男性
        Dim aibeyaManNinzu As Double = Double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE").ToString())
        Dim w1rom1 As Double = Math.Ceiling(aibeyaManNinzu / roomMaxCap)

        Dim yoyakuManNinzu As Double = Double.Parse(crsInfoBasicEntity.aibeyaYoyakuNinzuMale.Value.ToString())
        aibeyaManNinzu = aibeyaManNinzu + yoyakuManNinzu

        Dim w1rom2 As Double = Math.Ceiling(aibeyaManNinzu / roomMaxCap)

        Dim manKagenRoomSu As Integer = CInt(w1rom2 - w1rom1)

        ' 女性
        Dim aibeyaWoManNinzu As Double = Double.Parse(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI").ToString())
        w1rom1 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap)

        Dim yoyakuWomanNinzu As Double = Double.Parse(crsInfoBasicEntity.aibeyaYoyakuNinzuJyosei.Value.ToString())
        aibeyaWoManNinzu = aibeyaWoManNinzu + yoyakuWomanNinzu

        w1rom2 = Math.Ceiling(aibeyaWoManNinzu / roomMaxCap)

        Dim womanKagenRoomSu As Integer = CInt(w1rom2 - w1rom1)

        Dim entity As New CrsLedgerBasicEntity()
        entity.crsCd.Value = crsZasekiData.Item("CRS_CD").ToString()
        entity.syuptDay.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("SYUPT_DAY"))
        entity.gousya.Value = CommonRegistYoyaku.convertObjectToInteger(crsZasekiData.Item("GOUSYA"))

        If isTorikeshi Then

            entity.aibeyaYoyakuNinzuMale.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE")) - CommonRegistYoyaku.convertObjectToInteger(aibeyaManNinzu)
            entity.aibeyaYoyakuNinzuJyosei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI")) - CommonRegistYoyaku.convertObjectToInteger(aibeyaWoManNinzu)

            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) + manKagenRoomSu + womanKagenRoomSu
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) - manKagenRoomSu - womanKagenRoomSu

            entity.roomZansuOneRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi)
            entity.roomZansuTwoRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi)
            entity.roomZansuThreeRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi)
            entity.roomZansuFourRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi)
            entity.roomZansuFiveRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi)
        Else

            entity.aibeyaYoyakuNinzuMale.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_MALE")) + CommonRegistYoyaku.convertObjectToInteger(aibeyaManNinzu)
            entity.aibeyaYoyakuNinzuJyosei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("AIBEYA_YOYAKU_NINZU_JYOSEI")) + CommonRegistYoyaku.convertObjectToInteger(aibeyaWoManNinzu)

            entity.roomZansuSokei.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("ROOM_ZANSU_SOKEI")) - manKagenRoomSu - womanKagenRoomSu
            entity.yoyakuAlreadyRoomNum.Value = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)("YOYAKU_ALREADY_ROOM_NUM")) + manKagenRoomSu + womanKagenRoomSu

            entity.roomZansuOneRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_ONE_1R", "ROOM_ZANSU_ONE_ROOM", isTorikeshi)
            entity.roomZansuTwoRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_TWO_1R", "ROOM_ZANSU_TWO_ROOM", isTorikeshi)
            entity.roomZansuThreeRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_THREE_1R", "ROOM_ZANSU_THREE_ROOM", isTorikeshi)
            entity.roomZansuFourRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FOUR_1R", "ROOM_ZANSU_FOUR_ROOM", isTorikeshi)
            entity.roomZansuFiveRoom.Value = Me.calcRoomZansuAibeyaAri(zasekiData, manKagenRoomSu, womanKagenRoomSu, "CRS_BLOCK_FIVE_1R", "ROOM_ZANSU_FIVE_ROOM", isTorikeshi)
        End If

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
        sb.AppendLine("     GOUSYA =  " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY =  " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' ROOM MAX定員数取得
    ''' </summary>
    ''' <param name="crsZasekiData">コース台帳検索条件群</param>
    ''' <returns>ROOM MAX定員数</returns>
    Private Function getRoomMaxTein(crsZasekiData As Hashtable) As Double

        MyBase.paramClear()

        Dim entity As New CrsLedgerBasicEntity()

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
        sb.AppendLine("     CRS_CD = " + MyBase.setParam("CRS_CD", crsZasekiData.Item("CRS_CD"), entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam("SYUPT_DAY", crsZasekiData.Item("SYUPT_DAY"), entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam("GOUSYA", crsZasekiData.Item("GOUSYA"), entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Dim roomMaxInfo As DataTable = MyBase.getDataTable(sb.ToString())

        Const roomMaxCap As Double = 99

        If roomMaxInfo.Rows.Count <= CommonRegistYoyaku.ZERO Then

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
    ''' <param name="isTorikeshi">取消フラグ</param>
    ''' <returns>部屋残数</returns>
    Private Function calcRoomZansuAibeyaAri(zasekiData As DataTable, manKagenSu As Integer, womanKagenSu As Integer, crsBlockColName As String, roomZansuColName As String, isTorikeshi As Boolean) As Integer

        Dim crsBlockRoomSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(crsBlockColName))
        Dim crsRoomZanSu As Integer = CommonRegistYoyaku.convertObjectToInteger(zasekiData.Rows(0)(roomZansuColName))

        If crsBlockRoomSu = CommonRegistYoyaku.ZERO Then
            ' コースブロック管理されていない場合、部屋残数は変動なし
            Return crsRoomZanSu
        End If

        Dim fugoHanten As Integer = -1
        If isTorikeshi Then

            fugoHanten = 1
        End If

        Dim roomZansu As Integer = crsRoomZanSu + (manKagenSu * fugoHanten) + (womanKagenSu * fugoHanten)

        Return roomZansu
    End Function

    ''' <summary>
    ''' WTリクエストコース情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WTリクエストコース情報取得SQL</returns>
    Private Function createCrsWtRequestInfoSql(entity As WtRequestInfoEntity) As String

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
        sb.AppendLine("     ,CLB.RETURN_DAY ")
        sb.AppendLine("     ,CLB.CANCEL_WAIT_NINZU ")
        sb.AppendLine("     ,CLB.YOYAKU_NUM_TEISEKI ")
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
        sb.AppendLine("     ,CLB.TYUIJIKOU ")
        sb.AppendLine("     ,CLB.UNDER_KINSI_18OLD ")
        sb.AppendLine("     ,CLB.UWAGI_TYAKUYO ")
        sb.AppendLine("     ,CLB.TIE_TYAKUYO ")
        sb.AppendLine("     ,CLB.MAEURI_KIGEN ")
        sb.AppendLine("     ,CLB.AIBEYA_USE_FLG ")
        sb.AppendLine("     ,CLB.UKETUKE_GENTEI_NINZU ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_TEISEKI ")
        sb.AppendLine("     ,CLB.KUSEKI_NUM_SUB_SEAT ")
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
        sb.AppendLine("     ,CLB.TEJIMAI_KBN ")
        sb.AppendLine("     ,CLB.CANCEL_NG_FLG ")
        sb.AppendLine("     ,CLB.BUS_RESERVE_CD ")
        sb.AppendLine("     ,CLB.ZASEKI_RESERVE_KBN ")

        sb.AppendLine("     ,CLB.PICKUP_KBN_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_MEDIA_INPUT_FLG ")
        sb.AppendLine("     ,CLB.KOKUSEKI_FLG ")
        sb.AppendLine("     ,CLB.SEX_BETU_FLG ")
        sb.AppendLine("     ,CLB.AGE_FLG ")
        sb.AppendLine("     ,CLB.BIRTHDAY_FLG ")
        sb.AppendLine("     ,CLB.TEL_FLG ")
        sb.AppendLine("     ,CLB.ADDRESS_FLG ")
        sb.AppendLine("     ,CLB.YOYAKU_NG_FLG ")

        sb.AppendLine("     ,CLB.USING_FLG ")
        sb.AppendLine("     ,WRI.MANAGEMENT_KBN ")
        sb.AppendLine("     ,WRI.MANAGEMENT_NO ")
        sb.AppendLine("     ,WRI.SEIKI_CHARGE_ALL_GAKU ")
        sb.AppendLine("     ,WRI.WARIBIKI_ALL_GAKU ")
        sb.AppendLine("     ,WRI.ZASEKI ")
        sb.AppendLine("     ,WRI.NYUKINGAKU_SOKEI ")
        sb.AppendLine("     ,WRI.CANCEL_RYOU_KEI ")
        sb.AppendLine("     ,WRI.SUB_SEAT_WAIT_FLG ")
        sb.AppendLine("     ,WRI.MOTO_YOYAKU_KBN ")
        sb.AppendLine("     ,WRI.MOTO_YOYAKU_NO ")
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_1 ")
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_2 ")
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_3 ")
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_4 ")
        sb.AppendLine("     ,WRI.ROOMING_BETU_NINZU_5 ")
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_1 ")
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_2 ")
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_3 ")
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_4 ")
        sb.AppendLine("     ,WRI.JYOSYA_NINZU_5 ")
        sb.AppendLine("     ,WRI.INFANT_NINZU ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_1 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_2 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_3 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_4 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_5 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_6 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_7 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_8 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_9 ")
        sb.AppendLine("     ,WRI.MESSAGE_CHECK_FLG_10 ")
        sb.AppendLine("     ,WRI.MEDIA_CD ")
        sb.AppendLine("     ,WRI.AGENT_CD ")
        sb.AppendLine("     ,WRI.AGENT_NAME_KANA ")
        sb.AppendLine("     ,WRI.AGENT_NM ")
        sb.AppendLine("     ,WRI.AGENT_SEISAN_KBN ")
        sb.AppendLine("     ,WRI.AGENT_TEL_NO ")
        sb.AppendLine("     ,WRI.AGENT_TANTOSYA ")
        sb.AppendLine("     ,WRI.TOURS_NO ")
        sb.AppendLine("     ,WRI.KOKUSEKI ")
        sb.AppendLine("     ,WRI.SEX_BETU ")
        sb.AppendLine("     ,WRI.SURNAME ")
        sb.AppendLine("     ,WRI.NAME ")
        sb.AppendLine("     ,WRI.SURNAME_KJ ")
        sb.AppendLine("     ,WRI.NAME_KJ ")
        sb.AppendLine("     ,WRI.TEL_NO_1 ")
        sb.AppendLine("     ,WRI.TEL_NO_2 ")
        sb.AppendLine("     ,WRI.MAIL_ADDRESS ")
        sb.AppendLine("     ,WRI.MAIL_SENDING_KBN ")
        sb.AppendLine("     ,WRI.YUBIN_NO ")
        sb.AppendLine("     ,WRI.ADDRESS_1 ")
        sb.AppendLine("     ,WRI.ADDRESS_2 ")
        sb.AppendLine("     ,WRI.ADDRESS_3 ")
        sb.AppendLine("     ,WRI.MOSHIKOMI_HOTEL_FLG ")
        sb.AppendLine("     ,WRI.YYKMKS ")
        sb.AppendLine("     ,WRI.SEISAN_HOHO ")
        sb.AppendLine("     ,SEI.CODE_NAME AS SEISAN_HOHO_NM ")
        sb.AppendLine("     ,WRI.FURIKOMIYOSHI_YOHI_FLG ")
        sb.AppendLine("     ,WRI.SEIKYUSYO_YOHI_FLG ")
        sb.AppendLine("     ,WRI.RELATION_YOYAKU_KBN ")
        sb.AppendLine("     ,WRI.RELATION_YOYAKU_NO ")
        sb.AppendLine("     ,WRI.ADD_CHARGE_MAEBARAI_KEI ")
        sb.AppendLine("     ,WRI.ADD_CHARGE_TOJITU_PAYMENT_KEI ")
        sb.AppendLine("     ,WRI.ENTRY_DAY ")
        sb.AppendLine("     ,WRI.YOYAKU_JI_AGENT_CD ")
        sb.AppendLine("     ,WRI.YOYAKU_JI_AGENT_NAME ")
        sb.AppendLine("     ,WRI.JYOSEI_SENYO ")
        sb.AppendLine("     ,WRI.AIBEYA_FLG ")
        sb.AppendLine("     ,WRI.CANCEL_FLG ")
        sb.AppendLine("     ,WRI.HAKKEN_NAIYO ")
        sb.AppendLine("     ,WRI.CANCEL_FLG ")
        sb.AppendLine("     ,WRI.STATE ")
        sb.AppendLine("     ,WRI.TORIATUKAI_FEE_URIAGE ")
        sb.AppendLine("     ,WRI.TORIATUKAI_FEE_CANCEL ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC CLB ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CLB.CRS_CD = WRI.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.SYUPT_DAY = WRI.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLB.GOUSYA = WRI.GOUSYA ")
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
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE SEI ")
        sb.AppendLine(" ON  ")
        sb.AppendLine("     SEI.CODE_BUNRUI = '027' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     SEI.CODE_VALUE = WRI.SEISAN_HOHO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     WRI.MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     WRI.MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WTリクエスト料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WTリクエスト料金区分一覧取得SQL</returns>
    Private Function createWtRequestChargeListSql(entity As WtRequestInfoEntity) As String

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
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_1 ")
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_2 ")
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_3 ")
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_4 ")
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU_5 ")
        sb.AppendLine("     ,WCK.CHARGE_APPLICATION_NINZU ")
        sb.AppendLine("     ,WCK.TANKA_1 ")
        sb.AppendLine("     ,WCK.TANKA_2 ")
        sb.AppendLine("     ,WCK.TANKA_3 ")
        sb.AppendLine("     ,WCK.TANKA_4 ")
        sb.AppendLine("     ,WCK.TANKA_5 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU_1 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU_2 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU_3 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU_4 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU_5 ")
        sb.AppendLine("     ,WCK.CANCEL_NINZU ")
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
        sb.AppendLine("         T_WT_REQUEST_INFO WRI ")
        sb.AppendLine("     LEFT OUTER JOIN ")
        sb.AppendLine("         T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN CCK ")
        sb.AppendLine("     ON ")
        sb.AppendLine("         WRI.MANAGEMENT_KBN = CCK.MANAGEMENT_KBN ")
        sb.AppendLine("         AND ")
        sb.AppendLine("         WRI.MANAGEMENT_NO = CCK.MANAGEMENT_NO ")
        sb.AppendLine("         WHERE ")
        sb.AppendLine("         WRI.MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("         AND ")
        sb.AppendLine("         WRI.MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))
        sb.AppendLine("     ) WCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     WCK.KBN_NO = CLC.KBN_NO  ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     WCK.CHARGE_KBN_JININ_CD = CCK.CHARGE_KBN_JININ_CD ")
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
    ''' WTリクエスト料金区分一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WTリクエスト料金区分一覧取得SQL</returns>
    Private Function createWtRequestChargeKbnList(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      CCK.KBN_NO AS KBN_NO_URA ")
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD_URA ")
        sb.AppendLine("     ,CCK.CHARGE_KBN_JININ_CD CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     ,CJK.SEX_BETU  ")
        sb.AppendLine("     ,CCK.CHARGE_KBN ")
        sb.AppendLine("     ,CCK.KBN_NO AS KBN_NO ")
        sb.AppendLine("     ,MCK.JYOSEI_CHARGE_FLG ")
        sb.AppendLine("     ,MCK.STAY_ADD_FLG ")
        sb.AppendLine("     ,MCK.MEAL_ADD_FLG ")
        sb.AppendLine("     ,CCK.CARRIAGE ")
        sb.AppendLine("     ,CCK.CHARGE_APPLICATION_NINZU_1 AS YOYAKU_NINZU ")
        sb.AppendLine("     ,CCK.TANKA_1 AS CHARGE ")
        sb.AppendLine("     ,CLC.CHARGE_SUB_SEAT ")
        sb.AppendLine("     ,CLC.CARRIAGE_SUB_SEAT ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ")
        sb.AppendLine(" INNER JOIN ")
        sb.AppendLine("     T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN CCK ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CCK.MANAGEMENT_KBN = WRI.MANAGEMENT_KBN ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CCK.MANAGEMENT_NO = WRI.MANAGEMENT_NO ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     T_CRS_LEDGER_CHARGE_CHARGE_KBN CLC  ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     CLC.CRS_CD = WRI.CRS_CD ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.SYUPT_DAY = WRI.SYUPT_DAY ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     CLC.GOUSYA = WRI.GOUSYA ")
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
        sb.AppendLine("         WRI.MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("         AND ")
        sb.AppendLine("         WRI.MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WT_リクエスト情報（メモ）取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報（メモ）Entity</param>
    ''' <returns>WT_リクエスト情報（メモ）取得SQL</returns>
    Private Function createWtRequestMemoInfoListSql(entity As WtRequestInfoMemoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      WIM.SYSTEM_UPDATE_DAY ")
        sb.AppendLine("     ,WIM.MEMO_KBN ")
        sb.AppendLine("     ,KBN.CODE_NAME AS MEMO_KBN_NM ")
        sb.AppendLine("     ,WIM.MEMO_BUNRUI ")
        sb.AppendLine("     ,BUN.CODE_NAME AS MEMO_BUNRUI_NM ")
        sb.AppendLine("     ,WIM.NAIYO ")
        sb.AppendLine("     ,WIM.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SER.USER_NAME ")
        sb.AppendLine("     ,WIM.EDABAN ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO_MEMO WIM ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER SER ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     SER.COMPANY_CD = '0001' ")
        sb.AppendLine(" AND ")
        sb.AppendLine("     SER.USER_ID = WIM.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE KBN ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     KBN.CODE_BUNRUI = '004' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     KBN.CODE_VALUE = WIM.MEMO_KBN ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_CODE BUN ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     BUN.CODE_BUNRUI = '005' ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     BUN.CODE_VALUE = WIM.MEMO_BUNRUI ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     NVL(WIM.DELETE_DATE, 0) = 0 ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     WIM.MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     WIM.MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))
        sb.AppendLine(" ORDER BY WIM.SYSTEM_UPDATE_DAY, WIM.EDABAN ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WT_リクエスト情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WT_リクエスト情報取得SQL</returns>
    Private Function createWtRequestInfoSql(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      MANAGEMENT_KBN ")
        sb.AppendLine("     ,MANAGEMENT_NO ")
        sb.AppendLine("     ,ADDRESS_1 ")
        sb.AppendLine("     ,ADDRESS_2 ")
        sb.AppendLine("     ,ADDRESS_3 ")
        sb.AppendLine("     ,MOSHIKOMI_HOTEL_FLG ")
        sb.AppendLine("     ,ADD_CHARGE_MAEBARAI_KEI ")
        sb.AppendLine("     ,ADD_CHARGE_TOJITU_PAYMENT_KEI ")
        sb.AppendLine("     ,AF_STAY_YOYAKU_SGL_NUM ")
        sb.AppendLine("     ,AF_STAY_YOYAKU_TWN_NUM ")
        sb.AppendLine("     ,AGENT_CD ")
        sb.AppendLine("     ,AGENT_NAME_KANA ")
        sb.AppendLine("     ,AGENT_NM ")
        sb.AppendLine("     ,AGENT_TANTOSYA ")
        sb.AppendLine("     ,AGENT_TEL_NO ")
        sb.AppendLine("     ,AGENT_TEL_NO_1 ")
        sb.AppendLine("     ,AGENT_TEL_NO_2 ")
        sb.AppendLine("     ,AGENT_TEL_NO_3 ")
        sb.AppendLine("     ,AGENT_YOYAKU_CD ")
        sb.AppendLine("     ,AGENT_SEISAN_KBN ")
        sb.AppendLine("     ,AIBEYA_FLG ")
        sb.AppendLine("     ,BIRTHDAY ")
        sb.AppendLine("     ,CANCEL_FLG ")
        sb.AppendLine("     ,CANCEL_RYOU_KEI ")
        sb.AppendLine("     ,CHANGE_HISTORY_LAST_DAY ")
        sb.AppendLine("     ,CHANGE_HISTORY_LAST_SEQ ")
        sb.AppendLine("     ,CHECKIN_FLG_1 ")
        sb.AppendLine("     ,CHECKIN_FLG_2 ")
        sb.AppendLine("     ,CHECKIN_FLG_3 ")
        sb.AppendLine("     ,CHECKIN_FLG_4 ")
        sb.AppendLine("     ,CHECKIN_FLG_5 ")
        sb.AppendLine("     ,CHECKIN_NINZU_1 ")
        sb.AppendLine("     ,CHECKIN_NINZU_2 ")
        sb.AppendLine("     ,CHECKIN_NINZU_3 ")
        sb.AppendLine("     ,CHECKIN_NINZU_4 ")
        sb.AppendLine("     ,CHECKIN_NINZU_5 ")
        sb.AppendLine("     ,INFANT_NINZU ")
        sb.AppendLine("     ,CRS_CD ")
        sb.AppendLine("     ,CRS_KBN_1 ")
        sb.AppendLine("     ,CRS_KBN_2 ")
        sb.AppendLine("     ,CRS_KIND ")
        sb.AppendLine("     ,DELETE_DAY ")
        sb.AppendLine("     ,ENTRY_DAY ")
        sb.AppendLine("     ,ENTRY_PERSON_CD ")
        sb.AppendLine("     ,ENTRY_PGMID ")
        sb.AppendLine("     ,ENTRY_TIME ")
        sb.AppendLine("     ,FUJYO_PROOF_ISSUE_FLG ")
        sb.AppendLine("     ,FURIKOMIYOSHI_YOHI_FLG ")
        sb.AppendLine("     ,GOUSYA ")
        sb.AppendLine("     ,GROUP_NO ")
        sb.AppendLine("     ,HAKKEN_DAY ")
        sb.AppendLine("     ,HAKKEN_EIGYOSYO_CD ")
        sb.AppendLine("     ,HAKKEN_KINGAKU ")
        sb.AppendLine("     ,HAKKEN_NAIYO ")
        sb.AppendLine("     ,HAKKEN_TANTOSYA_CD ")
        sb.AppendLine("     ,ITINERARY_TABLE_PRINT_ALREADY ")
        sb.AppendLine("     ,ITINERARY_TABLE_PRINT_DAY ")
        sb.AppendLine("     ,JYOCHACHI_CD_1 ")
        sb.AppendLine("     ,JYOCHACHI_CD_2 ")
        sb.AppendLine("     ,JYOCHACHI_CD_3 ")
        sb.AppendLine("     ,JYOCHACHI_CD_4 ")
        sb.AppendLine("     ,JYOCHACHI_CD_5 ")
        sb.AppendLine("     ,JYOSEI_SENYO ")
        sb.AppendLine("     ,JYOSYA_NINZU_1 ")
        sb.AppendLine("     ,JYOSYA_NINZU_2 ")
        sb.AppendLine("     ,JYOSYA_NINZU_3 ")
        sb.AppendLine("     ,JYOSYA_NINZU_4 ")
        sb.AppendLine("     ,JYOSYA_NINZU_5 ")
        sb.AppendLine("     ,KOKUSEKI ")
        sb.AppendLine("     ,LAST_HENKIN_DAY ")
        sb.AppendLine("     ,LAST_NYUUKIN_DAY ")
        sb.AppendLine("     ,LOCAL_TEL_NO ")
        sb.AppendLine("     ,LOST_DAY ")
        sb.AppendLine("     ,LOST_FLG ")
        sb.AppendLine("     ,MAIL_ADDRESS ")
        sb.AppendLine("     ,MAIL_SENDING_KBN ")
        sb.AppendLine("     ,MEDIA_CD ")
        sb.AppendLine("     ,MEDIA_NAME ")
        sb.AppendLine("     ,MEIBO_SEQ ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_1 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_2 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_3 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_4 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_5 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_6 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_7 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_8 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_9 ")
        sb.AppendLine("     ,MESSAGE_CHECK_FLG_10 ")
        sb.AppendLine("     ,SURNAME ")
        sb.AppendLine("     ,NAME ")
        sb.AppendLine("     ,SURNAME_KJ ")
        sb.AppendLine("     ,NAME_KJ ")
        sb.AppendLine("     ,NO_SHOW_FLG ")
        sb.AppendLine("     ,NYUKINGAKU_SOKEI ")
        sb.AppendLine("     ,NYUUKIN_SITUATION_KBN ")
        sb.AppendLine("     ,OLD_GOUSYA ")
        sb.AppendLine("     ,OLD_ZASEKI ")
        sb.AppendLine("     ,RECEIPT_ISSUE_FLG ")
        sb.AppendLine("     ,RELATION_YOYAKU_KBN ")
        sb.AppendLine("     ,RELATION_YOYAKU_NO ")
        sb.AppendLine("     ,RETURN_DAY ")
        sb.AppendLine("     ,ROOMING_BETU_NINZU_1 ")
        sb.AppendLine("     ,ROOMING_BETU_NINZU_2 ")
        sb.AppendLine("     ,ROOMING_BETU_NINZU_3 ")
        sb.AppendLine("     ,ROOMING_BETU_NINZU_4 ")
        sb.AppendLine("     ,ROOMING_BETU_NINZU_5 ")
        sb.AppendLine("     ,SAIKOU_KAKUTEI_GUIDE_OUT_DAY ")
        sb.AppendLine("     ,SEIKI_CHARGE_ALL_GAKU ")
        sb.AppendLine("     ,SEIKYUSYO_YOHI_FLG ")
        sb.AppendLine("     ,SEISAN_HOHO ")
        sb.AppendLine("     ,SEX_BETU ")
        sb.AppendLine("     ,SIHARAI_HOHO ")
        sb.AppendLine("     ,SONOTA_NYUUKIN_HENKIN ")
        sb.AppendLine("     ,SPONSORSHIP_KEIYAKU_KBN ")
        sb.AppendLine("     ,STATE ")
        sb.AppendLine("     ,SYUGO_TIME_1 ")
        sb.AppendLine("     ,SYUGO_TIME_2 ")
        sb.AppendLine("     ,SYUGO_TIME_3 ")
        sb.AppendLine("     ,SYUGO_TIME_4 ")
        sb.AppendLine("     ,SYUGO_TIME_5 ")
        sb.AppendLine("     ,SYUPT_DAY ")
        sb.AppendLine("     ,SYUPT_TIME_1 ")
        sb.AppendLine("     ,SYUPT_TIME_2 ")
        sb.AppendLine("     ,SYUPT_TIME_3 ")
        sb.AppendLine("     ,SYUPT_TIME_4 ")
        sb.AppendLine("     ,SYUPT_TIME_5 ")
        sb.AppendLine("     ,TASYA_KENNO_KINGAKU ")
        sb.AppendLine("     ,TASYA_YOYAKU_NO ")
        sb.AppendLine("     ,TEIKI_KEIYAKU_KBN ")
        sb.AppendLine("     ,TEIKI_KIKAKU_KBN ")
        sb.AppendLine("     ,TEL_NO_1 ")
        sb.AppendLine("     ,TEL_NO_1_1 ")
        sb.AppendLine("     ,TEL_NO_1_2 ")
        sb.AppendLine("     ,TEL_NO_1_3 ")
        sb.AppendLine("     ,TEL_NO_2 ")
        sb.AppendLine("     ,TEL_NO_2_1 ")
        sb.AppendLine("     ,TEL_NO_2_2 ")
        sb.AppendLine("     ,TEL_NO_2_3 ")
        sb.AppendLine("     ,TOBI_SEAT_FLG ")
        sb.AppendLine("     ,TOMONOKAI_NO ")
        sb.AppendLine("     ,TORIATUKAI_FEE_CANCEL ")
        sb.AppendLine("     ,TORIATUKAI_FEE_SAGAKU ")
        sb.AppendLine("     ,TORIATUKAI_FEE_URIAGE ")
        sb.AppendLine("     ,TOURS_NO ")
        sb.AppendLine("     ,UNKYU_CONTACT_DAY ")
        sb.AppendLine("     ,UPDATE_DAY ")
        sb.AppendLine("     ,UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UPDATE_PGMID ")
        sb.AppendLine("     ,UPDATE_TIME ")
        sb.AppendLine("     ,USING_FLG ")
        sb.AppendLine("     ,WARIBIKI_ALL_GAKU ")
        sb.AppendLine("     ,YEAR ")
        sb.AppendLine("     ,YOBI_1 ")
        sb.AppendLine("     ,YOBI_2 ")
        sb.AppendLine("     ,YOBI_3 ")
        sb.AppendLine("     ,YOBI_4 ")
        sb.AppendLine("     ,YOBI_5 ")
        sb.AppendLine("     ,YOBI_6 ")
        sb.AppendLine("     ,YOBI_7 ")
        sb.AppendLine("     ,YOYAKU_JI_AGENT_CD ")
        sb.AppendLine("     ,YOYAKU_JI_AGENT_NAME ")
        sb.AppendLine("     ,YOYAKU_KAKUNIN_DAY ")
        sb.AppendLine("     ,YOYAKU_UKETUKE_KBN ")
        sb.AppendLine("     ,YOYAKU_ZASEKI_GET_KBN ")
        sb.AppendLine("     ,YOYAKU_ZASEKI_KBN ")
        sb.AppendLine("     ,YUBIN_NO ")
        sb.AppendLine("     ,YYKMKS ")
        sb.AppendLine("     ,ZASEKI ")
        sb.AppendLine("     ,ZASEKI_CHANGE_UMU ")
        sb.AppendLine("     ,ZASEKI_RESERVE_YOYAKU_FLG ")
        sb.AppendLine("     ,SUB_SEAT_WAIT_FLG ")
        sb.AppendLine("     ,MOTO_YOYAKU_KBN ")
        sb.AppendLine("     ,MOTO_YOYAKU_NO ")
        sb.AppendLine("     ,ENTRY_SECTION_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PGMID ")
        sb.AppendLine("     ,SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID ")
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY  ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO  ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     MOTO_YOYAKU_KBN = " + MyBase.setParam(entity.motoYoyakuKbn.PhysicsName, entity.motoYoyakuKbn.Value, entity.motoYoyakuKbn.DBType, entity.motoYoyakuKbn.IntegerBu, entity.motoYoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     MOTO_YOYAKU_NO = " + MyBase.setParam(entity.motoYoyakuNo.PhysicsName, entity.motoYoyakuNo.Value, entity.motoYoyakuNo.DBType, entity.motoYoyakuNo.IntegerBu, entity.motoYoyakuNo.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     NVL(STATE, ' ') = ' ' ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WTキャンセル待ち人数更新SQL作成
    ''' </summary>
    ''' <param name="entity">コース台帳（基本）Entity</param>
    ''' <param name="oracleTransaction">トランザクション</param>
    ''' <returns>WTキャンセル待ち人数更新SQL</returns>
    Private Function createWtCancelNinzuUpdateSql(entity As CrsLedgerBasicEntity, oracleTransaction As OracleTransaction) As String

        Dim wtRequestEntity As New WtRequestInfoEntity()
        wtRequestEntity.crsCd.Value = entity.crsCd.Value
        wtRequestEntity.syuptDay.Value = entity.syuptDay.Value
        wtRequestEntity.gousya.Value = entity.gousya.Value

        Dim wtCancelQuery As String = Me.createWtCancelNinzuSql(wtRequestEntity)
        Dim wtCancelInfo As DataTable = MyBase.getDataTable(oracleTransaction, wtCancelQuery)
        ' WT人数設定
        entity.cancelWaitNinzu.Value = CommonRegistYoyaku.convertObjectToInteger(wtCancelInfo.Rows(0)("CANCEL_WAIT_NINZU"))

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_CRS_LEDGER_BASIC ")
        sb.AppendLine(" SET ")
        sb.AppendLine("     CANCEL_WAIT_NINZU = " + MyBase.setParam(entity.cancelWaitNinzu.PhysicsName, entity.cancelWaitNinzu.Value, entity.cancelWaitNinzu.DBType, entity.cancelWaitNinzu.IntegerBu, entity.cancelWaitNinzu.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WT人数取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WT人数取得SQL</returns>
    Private Function createWtCancelNinzuSql(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("     COUNT(0) AS CANCEL_WAIT_NINZU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO  ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     CRS_CD = " + MyBase.setParam(entity.crsCd.PhysicsName, entity.crsCd.Value, entity.crsCd.DBType, entity.crsCd.IntegerBu, entity.crsCd.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SYUPT_DAY = " + MyBase.setParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay.DBType, entity.syuptDay.IntegerBu, entity.syuptDay.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     GOUSYA = " + MyBase.setParam(entity.gousya.PhysicsName, entity.gousya.Value, entity.gousya.DBType, entity.gousya.IntegerBu, entity.gousya.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     NVL(STATE, ' ') = ' ' ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 複写予約情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>複写予約情報取得SQL</returns>
    Private Function createReproductionYoyakuDataSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")
        sb.AppendLine("     ,SURNAME ")
        sb.AppendLine("     ,NAME ")
        sb.AppendLine("     ,SURNAME_KJ ")
        sb.AppendLine("     ,NAME_KJ ")
        sb.AppendLine("     ,TEL_NO_1 ")
        sb.AppendLine("     ,TEL_NO_2 ")
        sb.AppendLine("     ,YUBIN_NO ")
        sb.AppendLine("     ,ADDRESS_1 ")
        sb.AppendLine("     ,ADDRESS_2 ")
        sb.AppendLine("     ,ADDRESS_3 ")
        sb.AppendLine("     ,YYKMKS ")
        sb.AppendLine("     ,AGENT_CD ")
        sb.AppendLine("     ,AGENT_NAME_KANA ")
        sb.AppendLine("     ,AGENT_NM ")
        sb.AppendLine("     ,AGENT_TEL_NO ")
        sb.AppendLine("     ,AGENT_TANTOSYA ")
        sb.AppendLine("     ,AGENT_SEISAN_KBN ")
        sb.AppendLine("     ,TOURS_NO ")
        sb.AppendLine("     ,KOKUSEKI ")
        sb.AppendLine("     ,SEX_BETU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 複写WTリクエスト情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>複写WTリクエスト情報取得SQL</returns>
    Private Function createReproductionWtRequestDataSql(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      MANAGEMENT_KBN ")
        sb.AppendLine("     ,MANAGEMENT_NO ")
        sb.AppendLine("     ,SURNAME ")
        sb.AppendLine("     ,NAME ")
        sb.AppendLine("     ,SURNAME_KJ ")
        sb.AppendLine("     ,NAME_KJ ")
        sb.AppendLine("     ,TEL_NO_1 ")
        sb.AppendLine("     ,TEL_NO_2 ")
        sb.AppendLine("     ,YUBIN_NO ")
        sb.AppendLine("     ,ADDRESS_1 ")
        sb.AppendLine("     ,ADDRESS_2 ")
        sb.AppendLine("     ,ADDRESS_3 ")
        sb.AppendLine("     ,YYKMKS ")
        sb.AppendLine("     ,AGENT_CD ")
        sb.AppendLine("     ,AGENT_NAME_KANA ")
        sb.AppendLine("     ,AGENT_NM ")
        sb.AppendLine("     ,AGENT_TEL_NO ")
        sb.AppendLine("     ,AGENT_TANTOSYA ")
        sb.AppendLine("     ,AGENT_SEISAN_KBN ")
        sb.AppendLine("     ,TOURS_NO ")
        sb.AppendLine("     ,KOKUSEKI ")
        sb.AppendLine("     ,SEX_BETU ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 代理店情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">代理店マスタEntity</param>
    ''' <returns>代理店情報取得SQL</returns>
    Private Function createAgentMasterSql(entity As MAgentEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      * ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_AGENT ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     AGENT_CD = " + MyBase.setParam(entity.agentCd.PhysicsName, entity.agentCd.Value, entity.agentCd.DBType, entity.agentCd.IntegerBu, entity.agentCd.DecimalBu))

        Return sb.ToString()
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
        sb.AppendLine("     ,MPH.RK ")
        sb.AppendLine("     ,MPH.HOTEL_NAME_JYOSYA_TI ")
        sb.AppendLine("     ,YIP.PICKUP_ROUTE_CD ")
        sb.AppendLine("     ,MPH.SYUGO_PLACE ")
        sb.AppendLine("     ,SUBSTR(LPAD(RLH.SYUPT_TIME, 4,'0'), 1, 2)||':'|| SUBSTR(LPAD(RLH.SYUPT_TIME, 4,'0'), 3) AS SYUPT_TIME ")
        sb.AppendLine("     ,YIP.NINZU ")
        sb.AppendLine("     ,PRL.CRS_JYOSYA_TI ")
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
    ''' その他情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <returns>その他情報取得SQL</returns>
    Private Function createSonotaInfoSql(entity As YoyakuInfoBasicEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      YIB.SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,YIB.SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,EUS.USER_NAME AS ENTRY_USER_NAME ")
        sb.AppendLine("     ,YIB.SYSTEM_UPDATE_DAY ")
        sb.AppendLine("     ,YIB.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UUS.USER_NAME AS UPDATE_USER_NAME ")
        sb.AppendLine("     ,YIB.HAKKEN_DAY ")
        sb.AppendLine("     ,YIB.HAKKEN_TANTOSYA_CD ")
        sb.AppendLine("     ,HUS.USER_NAME AS HAKKEN_USER_NAME ")
        sb.AppendLine("     ,YIB.LAST_NYUUKIN_DAY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER EUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YIB.SYSTEM_ENTRY_PERSON_CD = EUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     EUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER UUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YIB.SYSTEM_UPDATE_PERSON_CD = UUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     UUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER HUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     YIB.HAKKEN_TANTOSYA_CD = HUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     HUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YIB.YOYAKU_KBN = " + MyBase.setParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn.DBType, entity.yoyakuKbn.IntegerBu, entity.yoyakuKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YIB.YOYAKU_NO = " + MyBase.setParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo.DBType, entity.yoyakuNo.IntegerBu, entity.yoyakuNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' その他情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">WTリクエストEntity</param>
    ''' <returns>その他情報取得SQL</returns>
    Private Function createWtSonotaInfoSql(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      WRI.SYSTEM_ENTRY_DAY ")
        sb.AppendLine("     ,WRI.SYSTEM_ENTRY_PERSON_CD ")
        sb.AppendLine("     ,EUS.USER_NAME AS ENTRY_USER_NAME ")
        sb.AppendLine("     ,WRI.SYSTEM_UPDATE_DAY ")
        sb.AppendLine("     ,WRI.SYSTEM_UPDATE_PERSON_CD ")
        sb.AppendLine("     ,UUS.USER_NAME AS UPDATE_USER_NAME ")
        sb.AppendLine("     ,WRI.HAKKEN_DAY ")
        sb.AppendLine("     ,WRI.HAKKEN_TANTOSYA_CD ")
        sb.AppendLine("     ,HUS.USER_NAME AS HAKKEN_USER_NAME ")
        sb.AppendLine("     ,WRI.LAST_NYUUKIN_DAY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_WT_REQUEST_INFO WRI ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER EUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     WRI. SYSTEM_ENTRY_PERSON_CD = EUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     EUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER UUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     WRI. SYSTEM_UPDATE_PERSON_CD = UUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     UUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     M_USER HUS ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     WRI. HAKKEN_TANTOSYA_CD = HUS.USER_ID ")
        sb.AppendLine("     AND ")
        sb.AppendLine("     HUS.COMPANY_CD = '0001' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     WRI.MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     WRI.MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 利用者情報取得SQL作成
    ''' </summary>
    ''' <param name="nullRecord">空行レコード挿入フラグ</param>
    ''' <returns>利用者情報取得SQL</returns>
    Private Function createUserMasterSql(nullRecord As Boolean) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT * FROM ( ")
        If nullRecord = True Then
            sb.AppendLine(" SELECT ")
            sb.AppendLine("      ' ' AS CODE_VALUE ")
            sb.AppendLine("     ,' ' AS CODE_NAME ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     DUAL ")
            sb.AppendLine(" UNION ")
        End If
        sb.AppendLine(" SELECT ")
        sb.AppendLine("       USER_ID AS CODE_VALUE ")
        sb.AppendLine("      ,USER_NAME AS CODE_NAME ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_USER ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     COMPANY_CD = '0001' ")
        sb.AppendLine(" ) M_USER ")
        'sb.AppendLine(" ORDER BY CODE_VALUE ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' WT_リクエスト情報更新SQL作成
    ''' </summary>
    ''' <param name="entity">WT_リクエスト情報Entity</param>
    ''' <returns>WT_リクエスト状態更新SQL</returns>
    Private Function createWtRequestStateUpdateSql(entity As WtRequestInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_WT_REQUEST_INFO ")
        sb.AppendLine(" SET ")
        sb.AppendLine("     UPDATE_DAY = " + MyBase.setParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay.DBType, entity.updateDay.IntegerBu, entity.updateDay.DecimalBu))
        sb.AppendLine("     ,UPDATE_PERSON_CD = " + MyBase.setParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd.DBType, entity.updatePersonCd.IntegerBu, entity.updatePersonCd.DecimalBu))
        sb.AppendLine("     ,UPDATE_PGMID = " + MyBase.setParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid.DBType, entity.updatePgmid.IntegerBu, entity.updatePgmid.DecimalBu))
        sb.AppendLine("     ,UPDATE_TIME = " + MyBase.setParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime.DBType, entity.updateTime.IntegerBu, entity.updateTime.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + MyBase.setParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid.DBType, entity.systemUpdatePgmid.IntegerBu, entity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd.DBType, entity.systemUpdatePersonCd.IntegerBu, entity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = TO_DATE(" + MyBase.setParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay.DBType, entity.systemUpdateDay.IntegerBu, entity.systemUpdateDay.DecimalBu) + ",'yyyy/mm/dd hh24:mi:ss')")
        sb.AppendLine("     ,STATE = " + MyBase.setParam(entity.state.PhysicsName, entity.state.Value, entity.state.DBType, entity.state.IntegerBu, entity.state.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     MANAGEMENT_KBN = " + MyBase.setParam(entity.managementKbn.PhysicsName, entity.managementKbn.Value, entity.managementKbn.DBType, entity.managementKbn.IntegerBu, entity.managementKbn.DecimalBu))
        sb.AppendLine("     AND ")
        sb.AppendLine("     MANAGEMENT_NO = " + MyBase.setParam(entity.managementNo.PhysicsName, entity.managementNo.Value, entity.managementNo.DBType, entity.managementNo.IntegerBu, entity.managementNo.DecimalBu))

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
