Imports System.Reflection
Imports System.Text

''' <summary>
''' 
''' </summary>
Public Class CommonKessaiDa
    Inherits DataAccessorBase

#Region "メソッド"

    ''' <summary>
    ''' 予約情報（基本）取得
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>予約情報（基本）</returns>
    Public Function getYoyakuInfoBasic(yoyakuKbn As String, yoyakuNo As Integer) As DataTable

        Dim yoyakuInfo As DataTable

        Try
            Dim query As String = Me.createYoyakuInfoBasicSql(yoyakuKbn, yoyakuNo)
            yoyakuInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuInfo
    End Function

    ''' <summary>
    ''' 予約情報（基本）取得SQL作成
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>予約情報（基本）取得SQL</returns>
    Private Function createYoyakuInfoBasicSql(yoyakuKbn As String, yoyakuNo As Integer) As String

        Dim entity As New YoyakuInfoBasicEntity()
        entity.yoyakuKbn.Value = yoyakuKbn
        entity.yoyakuNo.Value = yoyakuNo

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT * FROM T_YOYAKU_INFO_BASIC ")
        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（決済）取得
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>予約情報（決済）</returns>
    Public Function getYoyakuInfoKessai(yoyakuKbn As String, yoyakuNo As Integer) As DataTable

        Dim kessaiInfo As DataTable

        Try
            Dim query As String = Me.createYoyakuInfoKessaiSql(yoyakuKbn, yoyakuNo)
            kessaiInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return kessaiInfo
    End Function

    ''' <summary>
    ''' 予約情報（決済）取得SQL作成
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>予約情報（決済）取得SQL</returns>
    Private Function createYoyakuInfoKessaiSql(yoyakuKbn As String, yoyakuNo As Integer) As String

        Dim entity As New TYoyakuInfoSettlementEntity()
        entity.YoyakuKbn.Value = yoyakuKbn
        entity.YoyakuNo.Value = yoyakuNo
        entity.SettlementProcessResultCd.Value = "0"

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT * FROM T_YOYAKU_INFO_SETTLEMENT ")
        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO = " + Me.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo))
        sb.AppendLine(" AND SETTLEMENT_PROCESS_RESULT_CD = " + Me.prepareParam(entity.SettlementProcessResultCd.PhysicsName, entity.SettlementProcessResultCd.Value, entity.SettlementProcessResultCd))
        sb.AppendLine(" AND NVL(GMO_OSORI_CANCEL_DAY, 0) = 0 ")
        sb.AppendLine(" AND NVL(DELETE_DAY, 0) = 0 ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 前回決済金額取得
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>前回決済金額</returns>
    Public Function getBfKessaiAmount(yoyakuKbn As String, yoyakuNo As Integer) As Integer

        Dim amount As Integer = 0

        Try

            Dim query As String = Me.createBfKessaiAmount(yoyakuKbn, yoyakuNo)
            Dim bfAmountTable As DataTable = MyBase.getDataTable(query)

            If bfAmountTable.Rows.Count > 0 Then

                amount = Integer.Parse(bfAmountTable.Rows(0)("KINGAKU").ToString())
            Else

                amount = 0
            End If
        Catch ex As Exception

            Throw
        End Try

        Return amount
    End Function

    ''' <summary>
    ''' 前回決済金額取得SQL作成
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>前回決済金額取得SQL</returns>
    Private Function createBfKessaiAmount(yoyakuKbn As String, yoyakuNo As Integer) As String

        Dim entity As New TNyuukinInfoEntity()
        entity.YoyakuKbn.Value = yoyakuKbn
        entity.YoyakuNo.Value = yoyakuNo
        entity.SettlementModuleKbn.Value = "G"

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT  ")
        sb.AppendLine("      YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")
        sb.AppendLine("     ,SUM(NYUKINGAKU) - SUM(HENKINGAKU) AS KINGAKU ")
        sb.AppendLine(" FROM ( ")
        sb.AppendLine("     SELECT ")
        sb.AppendLine("         YOYAKU_KBN ")
        sb.AppendLine("         ,YOYAKU_NO ")
        sb.AppendLine("         ,SETTLEMENT_MODULE_KBN ")
        sb.AppendLine("         ,HAKKEN_HURIKOMI_KBN ")
        sb.AppendLine("         ,CASE HAKKEN_HURIKOMI_KBN WHEN '7' THEN SUM(NYUUKIN_GAKU_2) ELSE 0 END AS NYUKINGAKU ")
        sb.AppendLine("         ,CASE HAKKEN_HURIKOMI_KBN WHEN '8' THEN SUM(NYUUKIN_GAKU_2) ELSE 0 END AS HENKINGAKU ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("         T_NYUUKIN_INFO ")
        sb.AppendLine("     WHERE ")
        sb.AppendLine("         YOYAKU_KBN = " + Me.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn))
        sb.AppendLine("         AND ")
        sb.AppendLine("         YOYAKU_NO = " + Me.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo))
        sb.AppendLine("         AND ")
        sb.AppendLine("         SETTLEMENT_MODULE_KBN = " + Me.prepareParam(entity.SettlementModuleKbn.PhysicsName, entity.SettlementModuleKbn.Value, entity.SettlementModuleKbn))
        sb.AppendLine("         AND ")
        sb.AppendLine("         HAKKEN_HURIKOMI_KBN IN('7', '8') ")
        sb.AppendLine("     GROUP BY  ")
        sb.AppendLine("          YOYAKU_KBN ")
        sb.AppendLine("         ,YOYAKU_NO ")
        sb.AppendLine("         ,SETTLEMENT_MODULE_KBN ")
        sb.AppendLine("         ,HAKKEN_HURIKOMI_KBN) NYUHENKIN ")
        sb.AppendLine(" GROUP BY ")
        sb.AppendLine("      YOYAKU_KBN ")
        sb.AppendLine("     ,YOYAKU_NO ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（決済）情報登録
    ''' </summary>
    ''' <param name="entity">予約情報（決済）</param>
    ''' <returns>登録結果</returns>
    Public Function registYoyakuInfoKessai(entity As TYoyakuInfoSettlementEntity) As Boolean

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim query As String = Me.createInsertSql(Of TYoyakuInfoSettlementEntity)(entity, "T_YOYAKU_INFO_SETTLEMENT")

            updateCount = MyBase.execNonQuery(oracleTransaction, query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return False
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

        Return True
    End Function

    ''' <summary>
    ''' 最大値SEQ取得
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>最大値SEQ</returns>
    Public Function getNyuhenkinSeq(yoyakuKbn As String, yoyakuNo As Integer) As Integer

        Dim nyukinSeq As Integer = 0

        Try

            Dim query As String = Me.createNyuhenkinSeqSql(yoyakuKbn, yoyakuNo)
            Dim seqDataTable As DataTable = MyBase.getDataTable(query)

            nyukinSeq = Integer.Parse(seqDataTable.Rows(0)("SEQ").ToString())

        Catch ex As Exception

            Throw
        End Try

        Return nyukinSeq
    End Function

    ''' <summary>
    ''' 最大値SEQ取得SQL作成
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約NO</param>
    ''' <returns>最大値SEQ取得SQL</returns>
    Private Function createNyuhenkinSeqSql(yoyakuKbn As String, yoyakuNo As Integer) As String

        MyBase.paramClear()

        Dim entity As New TNyuukinInfoEntity()
        entity.YoyakuKbn.Value = yoyakuKbn
        entity.YoyakuNo.Value = yoyakuNo

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT (NVL(MAX(SEQ), 0) + 1) As SEQ FROM T_NYUUKIN_INFO ")
        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO = " + Me.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（決済）更新
    ''' </summary>
    ''' <param name="entity">予約情報（決済）</param>
    ''' <returns>更新結果</returns>
    Public Function updateYoyakuInfoKessai(entity As TYoyakuInfoSettlementEntity) As Boolean

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            Dim query As String = Me.createYoyakuInfoKessaiUpdateSql(entity)

            updateCount = MyBase.execNonQuery(oracleTransaction, query)

            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return False
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

        Return True
    End Function

    ''' <summary>
    ''' 予約情報（決済）更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（決済）</param>
    ''' <returns>予約情報（決済）更新SQL</returns>
    Private Function createYoyakuInfoKessaiUpdateSql(entity As TYoyakuInfoSettlementEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_SETTLEMENT SET ")
        sb.AppendLine("  GMO_OSORI_CANCEL_DAY = " + Me.prepareParam(entity.GmoOsoriCancelDay.PhysicsName, entity.GmoOsoriCancelDay.Value, entity.GmoOsoriCancelDay))
        sb.AppendLine(" ,GMO_OSORI_CANCEL_TIME = " + Me.prepareParam(entity.GmoOsoriCancelTime.PhysicsName, entity.GmoOsoriCancelTime.Value, entity.GmoOsoriCancelTime))
        sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.UpdateDay.PhysicsName, entity.UpdateDay.Value, entity.UpdateDay))
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.UpdatePersonCd.PhysicsName, entity.UpdatePersonCd.Value, entity.UpdatePersonCd))
        sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.UpdatePgmid.PhysicsName, entity.UpdatePgmid.Value, entity.UpdatePgmid))
        sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.UpdateTime.PhysicsName, entity.UpdateTime.Value, entity.UpdateTime))
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.SystemUpdatePgmid.PhysicsName, entity.SystemUpdatePgmid.Value, entity.SystemUpdatePgmid))
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.SystemUpdatePersonCd.PhysicsName, entity.SystemUpdatePersonCd.Value, entity.SystemUpdatePersonCd))
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.SystemUpdateDay.PhysicsName, entity.SystemUpdateDay.Value, entity.SystemUpdateDay))
        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.YoyakuKbn.PhysicsName, entity.YoyakuKbn.Value, entity.YoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO =  " + Me.prepareParam(entity.YoyakuNo.PhysicsName, entity.YoyakuNo.Value, entity.YoyakuNo))
        sb.AppendLine(" AND ORDER_ID =  " + Me.prepareParam(entity.OrderId.PhysicsName, entity.OrderId.Value, entity.OrderId))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約関連登録
    ''' </summary>
    ''' <param name="registEntity">登録用Entity</param>
    ''' <param name="registDate">登録日付</param>
    ''' <param name="formId">画面ID</param>
    ''' <param name="torikeshiFlg">取消フラグ</param>
    ''' <returns>更新ステータス</returns>
    Public Function registYoyakuInfo(registEntity As KessaiRegistEntity, registDate As DateTime, formId As String, torikeshiFlg As Boolean) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 入返金情報
            Dim nyukinQuery As String = Me.createInsertSql(Of TNyuukinInfoEntity)(registEntity.NyuukinInfoEntity, "T_NYUUKIN_INFO")

            updateCount = MyBase.execNonQuery(oracleTransaction, nyukinQuery)
            If updateCount <= 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oracleTransaction)
                Return CommonKessai.RegistKessaiStatusNyukinFailure
            End If

            ' 変更履歴情報
            Dim historyRec As OutputYoyakuChangeHistoryParam = CommonRegistYoyaku.registYoyakuChangeHistory(oracleTransaction,
                                                                                                            registEntity.YoyakuInfoBasicEntity,
                                                                                                            Nothing)
            If historyRec Is Nothing Then

                Return CommonKessai.RegistKessaiStatusHistoryFailure
            End If

            If registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq Then
                ' 変更履歴が登録されていない場合T_YOYAKU_INFO_SETTLEMENT

                Dim basicQuery As String = ""
                If torikeshiFlg = True Then
                    ' 取消の場合

                    basicQuery = Me.createYoyakuInfoBasicUpdateSqlForTorikeshi(registEntity.YoyakuInfoBasicEntity, True)
                    updateCount = MyBase.execNonQuery(oracleTransaction, basicQuery)
                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure
                    End If
                Else
                    ' 売上の場合

                    basicQuery = Me.createYoyakuInfoBasicUpdateSqlForUriage(registEntity.YoyakuInfoBasicEntity, False)
                    updateCount = MyBase.execNonQuery(oracleTransaction, basicQuery)
                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure
                    End If
                End If
            Else
                ' 変更履歴が登録されている場合

                ' 予約情報（基本）
                registEntity.YoyakuInfoBasicEntity.changeHistoryLastDay.Value = historyRec.changeHistoryLastDay
                registEntity.YoyakuInfoBasicEntity.changeHistoryLastSeq.Value = historyRec.changeHistoryLastSeq
                registEntity.YoyakuInfoBasicEntity.updateDay.Value = CInt(registDate.ToString("yyyyMMdd"))
                registEntity.YoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId
                registEntity.YoyakuInfoBasicEntity.updatePgmid.Value = formId
                registEntity.YoyakuInfoBasicEntity.updateTime.Value = CInt(registDate.ToString("HHmmss"))
                registEntity.YoyakuInfoBasicEntity.systemUpdateDay.Value = registDate
                registEntity.YoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId
                registEntity.YoyakuInfoBasicEntity.systemUpdatePgmid.Value = formId

                ' 予約情報（基本）
                Dim basicQuery As String = ""
                If torikeshiFlg = True Then
                    ' 取消の場合

                    basicQuery = Me.createYoyakuInfoBasicUpdateSqlForTorikeshi(registEntity.YoyakuInfoBasicEntity, False)
                    updateCount = MyBase.execNonQuery(oracleTransaction, basicQuery)
                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure
                    End If
                Else
                    ' 売上の場合

                    basicQuery = Me.createYoyakuInfoBasicUpdateSqlForUriage(registEntity.YoyakuInfoBasicEntity, True)
                    updateCount = MyBase.execNonQuery(oracleTransaction, basicQuery)
                    If updateCount <= 0 Then

                        ' ロールバック
                        MyBase.callRollbackTransaction(oracleTransaction)
                        Return CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure
                    End If
                End If

                ' 予約情報２
                Dim yoyaku2Query As String = Me.createYoyakuInfo2Sql(registEntity.YoyakuInfo2Entity)
                Dim yoyakuInfo2 As DataTable = MyBase.getDataTable(yoyaku2Query)

                Dim yoyakuInfo2Query As String
                If yoyakuInfo2.Rows.Count > 0 Then

                    yoyakuInfo2Query = Me.createYoyakuInfo2UpdateSql(registEntity.YoyakuInfo2Entity)
                Else

                    yoyakuInfo2Query = Me.createInsertSql(Of YoyakuInfo2Entity)(registEntity.YoyakuInfo2Entity, "T_YOYAKU_INFO_2")
                End If

                updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuInfo2Query)
                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonKessai.RegistKessaiStatusYoyakuInfo2Failure
                End If

                ' 予約情報（コース料金）
                Dim yoyakuCrsChargeQuery As String = Me.createYoyakuInfoCrsChargeUpdateSql(registEntity.YoyakuInfoCrsChargeEntity)

                updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuCrsChargeQuery)
                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeFailure
                End If

                ' 予約情報（コース料金_料金区分）
                Dim yoyakuCrsChargeChargeKbnQuery As String = Me.createYoyakuInfoCrsChargeChargeKbnUpdateSql(registEntity.YoyakuInfoCrsChargeChargeKbnEntity)

                updateCount = MyBase.execNonQuery(oracleTransaction, yoyakuCrsChargeChargeKbnQuery)
                If updateCount <= 0 Then

                    ' ロールバック
                    MyBase.callRollbackTransaction(oracleTransaction)
                    Return CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeChargeKbnFailure
                End If
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

        Return CommonKessai.RegistKessaiStatusSucess
    End Function

    ''' <summary>
    ''' 予約情報（基本）更新SQL作成
    ''' 売上用
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <param name="isUpdateFlg">更新フラグ</param>
    ''' <returns>予約情報（基本）更新SQL</returns>
    Private Function createYoyakuInfoBasicUpdateSqlForUriage(entity As YoyakuInfoBasicEntity, isUpdateFlg As Boolean) As String

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC SET ")

        sb.AppendLine("  SEISAN_HOHO = " + Me.prepareParam(entity.seisanHoho.PhysicsName, entity.seisanHoho.Value, entity.seisanHoho))
        sb.AppendLine(" ,NYUKINGAKU_SOKEI = " + Me.prepareParam(entity.nyukingakuSokei.PhysicsName, entity.nyukingakuSokei.Value, entity.nyukingakuSokei))
        sb.AppendLine(" ,LAST_NYUUKIN_DAY = " + Me.prepareParam(entity.lastNyuukinDay.PhysicsName, entity.lastNyuukinDay.Value, entity.lastNyuukinDay))
        sb.AppendLine(" ,FURIKOMIYOSHI_YOHI_FLG = " + Me.prepareParam(entity.furikomiyoshiYohiFlg.PhysicsName, entity.furikomiyoshiYohiFlg.Value, entity.furikomiyoshiYohiFlg))
        sb.AppendLine(" ,NYUUKIN_SITUATION_KBN = " + Me.prepareParam(entity.nyuukinSituationKbn.PhysicsName, entity.nyuukinSituationKbn.Value, entity.nyuukinSituationKbn))

        If isUpdateFlg = True Then

            sb.AppendLine(" ,CHANGE_HISTORY_LAST_DAY = " + Me.prepareParam(entity.changeHistoryLastDay.PhysicsName, entity.changeHistoryLastDay.Value, entity.changeHistoryLastDay))
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_SEQ = " + Me.prepareParam(entity.changeHistoryLastSeq.PhysicsName, entity.changeHistoryLastSeq.Value, entity.changeHistoryLastSeq))
            sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
            sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
            sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
            sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
            sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
            sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
            sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))

        End If

        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO =  " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（基本）更新SQL作成
    ''' 取消用
    ''' </summary>
    ''' <param name="entity">予約情報（基本）Entity</param>
    ''' <param name="isUpdateFlg">更新フラグ</param>
    ''' <returns>予約情報（基本）更新SQL</returns>
    Private Function createYoyakuInfoBasicUpdateSqlForTorikeshi(entity As YoyakuInfoBasicEntity, isUpdateFlg As Boolean) As String

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC SET ")

        sb.AppendLine("  NYUKINGAKU_SOKEI = " + Me.prepareParam(entity.nyukingakuSokei.PhysicsName, entity.nyukingakuSokei.Value, entity.nyukingakuSokei))
        sb.AppendLine(" ,LAST_HENKIN_DAY = " + Me.prepareParam(entity.lastHenkinDay.PhysicsName, entity.lastHenkinDay.Value, entity.lastHenkinDay))
        sb.AppendLine(" ,NYUUKIN_SITUATION_KBN = " + Me.prepareParam(entity.nyuukinSituationKbn.PhysicsName, entity.nyuukinSituationKbn.Value, entity.nyuukinSituationKbn))

        If isUpdateFlg = True Then

            sb.AppendLine(" ,CHANGE_HISTORY_LAST_DAY = " + Me.prepareParam(entity.changeHistoryLastDay.PhysicsName, entity.changeHistoryLastDay.Value, entity.changeHistoryLastDay))
            sb.AppendLine(" ,CHANGE_HISTORY_LAST_SEQ = " + Me.prepareParam(entity.changeHistoryLastSeq.PhysicsName, entity.changeHistoryLastSeq.Value, entity.changeHistoryLastSeq))
            sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
            sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
            sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
            sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
            sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
            sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
            sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))

        End If

        sb.AppendLine(" WHERE YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine(" AND YOYAKU_NO =  " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

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
        sb.AppendLine("     YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報２更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報２Entity</param>
    ''' <returns>予約情報２更新SQL</returns>
    Private Function createYoyakuInfo2UpdateSql(entity As YoyakuInfo2Entity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_2 SET ")
        sb.AppendLine("  OUT_DAY = " + Me.prepareParam(entity.outDay.PhysicsName, entity.outDay.Value, entity.outDay))
        sb.AppendLine(" ,OUT_PERSON_CD = " + Me.prepareParam(entity.outPersonCd.PhysicsName, entity.outPersonCd.Value, entity.outPersonCd))
        sb.AppendLine(" ,OUT_PGMID = " + Me.prepareParam(entity.outPgmid.PhysicsName, entity.outPgmid.Value, entity.outPgmid))
        sb.AppendLine(" ,OUT_TIME = " + Me.prepareParam(entity.outTime.PhysicsName, entity.outTime.Value, entity.outTime))
        sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
        sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
        sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（コース料金）更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（コース料金）Entity</param>
    ''' <returns>予約情報（コース料金）更新SQL</returns>
    Private Function createYoyakuInfoCrsChargeUpdateSql(entity As YoyakuInfoCrsChargeEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_CRS_CHARGE SET ")
        sb.AppendLine("  CANCEL_RYOU = " + Me.prepareParam(entity.cancelRyou.PhysicsName, entity.cancelRyou.Value, entity.cancelRyou))
        sb.AppendLine(" ,CANCEL_PER = " + Me.prepareParam(entity.cancelPer.PhysicsName, entity.cancelPer.Value, entity.cancelPer))
        sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
        sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
        sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 予約情報（コース料金_料金区分）更新SQL作成
    ''' </summary>
    ''' <param name="entity">予約情報（コース料金_料金区分）Entity</param>
    ''' <returns>予約情報（コース料金_料金区分）更新SQL</returns>
    Private Function createYoyakuInfoCrsChargeChargeKbnUpdateSql(entity As YoyakuInfoCrsChargeChargeKbnEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN SET ")
        sb.AppendLine("  CANCEL_NINZU_1 = " + Me.prepareParam(entity.cancelNinzu1.PhysicsName, entity.cancelNinzu1.Value, entity.cancelNinzu1))
        sb.AppendLine(" ,CANCEL_NINZU_2 = " + Me.prepareParam(entity.cancelNinzu2.PhysicsName, entity.cancelNinzu2.Value, entity.cancelNinzu2))
        sb.AppendLine(" ,CANCEL_NINZU_3 = " + Me.prepareParam(entity.cancelNinzu3.PhysicsName, entity.cancelNinzu3.Value, entity.cancelNinzu3))
        sb.AppendLine(" ,CANCEL_NINZU_4 = " + Me.prepareParam(entity.cancelNinzu4.PhysicsName, entity.cancelNinzu4.Value, entity.cancelNinzu4))
        sb.AppendLine(" ,CANCEL_NINZU_5 = " + Me.prepareParam(entity.cancelNinzu5.PhysicsName, entity.cancelNinzu5.Value, entity.cancelNinzu5))
        sb.AppendLine(" ,CANCEL_NINZU = " + Me.prepareParam(entity.cancelNinzu.PhysicsName, entity.cancelNinzu.Value, entity.cancelNinzu))
        sb.AppendLine(" ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
        sb.AppendLine(" ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
        sb.AppendLine(" ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
        sb.AppendLine(" ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
        sb.AppendLine(" ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
        sb.AppendLine(" ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
        sb.AppendLine(" ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))

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
    ''' パラメータの用意
    ''' </summary>
    ''' <param name="name">パラメータ名（重複不可）</param>
    ''' <param name="value">パラメータ</param>
    ''' <param name="entKoumoku">エンティティの項目</param>
    Private Function prepareParam(ByVal name As String,
                                  ByVal value As Object,
                                  ByVal entKoumoku As IEntityKoumokuType) As String

        Return MyBase.setParam(name,
                               value,
                               entKoumoku.DBType,
                               entKoumoku.IntegerBu,
                               entKoumoku.DecimalBu)
    End Function

#End Region

End Class
