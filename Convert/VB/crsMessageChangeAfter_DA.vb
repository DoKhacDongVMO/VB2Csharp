Imports System.Text

Public Class CrsMessageChangeAfter_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private yoyakuInfoBasicEntity As New TYoyakuInfoBasicEntity()
    Private yoyakuInfoPickUpEntity As New TYoyakuInfoPickupEntity()
    Private Const notFlg As String = ""
#End Region

    ''' <summary>
    ''' 更新対象の予約情報を取得する
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function getChageYoyakuInfo(ByVal paramInfoList As Hashtable) As DataTable
        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sb As New StringBuilder
        With yoyakuInfoBasicEntity


            'SELECT句
            sb.AppendLine(" SELECT ")
            sb.AppendLine("     YIB.YOYAKU_KBN ")
            sb.AppendLine("   , YIB.YOYAKU_NO ")
            sb.AppendLine("   , YIB.USING_FLG")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ")
            sb.AppendLine(" WHERE 1=1 ")
            sb.AppendLine("    AND NVL(YIB.DELETE_DAY, 0) = 0")
            'sb.AppendLine("    AND NVL(YIB.CANCEL_FLG, 0) = 0")
            sb.AppendLine("    AND YIB.SYSTEM_UPDATE_PERSON_CD != 'BAT_USER'")
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("crsCd"), String)) Then
                sb.AppendLine("    AND YIB.CRS_CD = " & setParam("crsCd", CType(paramInfoList.Item("crsCd"), String), .CrsCd.DBType, .CrsCd.IntegerBu, .CrsCd.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("gousya"), String)) Then
                sb.AppendLine("    AND YIB.GOUSYA = " & setParam("gousya", CType(paramInfoList.Item("gousya"), String), .Gousya.DBType, .Gousya.IntegerBu, .Gousya.DecimalBu))
            End If
            If Not String.IsNullOrEmpty(CType(paramInfoList.Item("syuptDay"), String)) Then
                sb.AppendLine("    AND YIB.SYUPT_DAY = " & setParam("syuptDay", CType(paramInfoList.Item("syuptDay"), String), .SyuptDay.DBType, .SyuptDay.IntegerBu, .SyuptDay.DecimalBu))
            End If
        End With
        Try
            returnValue = getDataTable(sb.ToString)
        Catch ex As Exception
            Throw
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' 予約情報(ピックアップ）更新
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約No</param>
    ''' <returns></returns>
    Public Function updateYoyakuPickUpInfo(ByVal yoyakuKbn As String, ByVal yoyakuNo As String, oracleTransaction As OracleTransaction) As Boolean

        'Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        '戻り値
        Dim returnValue As DataTable = Nothing
        Dim sqlString As New StringBuilder

        Try
            'トランザクション開始
            'oracleTransaction = MyBase.callBeginTransaction()

            ' update
            With yoyakuInfoPickUpEntity
                sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_PICKUP YIP")
                sqlString.AppendLine("    SET YIP.ZUMI_FLG = '' ")
                sqlString.AppendLine(" WHERE YIP.YOYAKU_KBN = " & setParam("yoyakuKbn", yoyakuKbn, .YoyakuKbn.DBType, .YoyakuKbn.IntegerBu, .YoyakuKbn.DecimalBu))
                sqlString.AppendLine(" AND YIP.YOYAKU_NO = " & setParam("yoyakuNo", yoyakuNo, .YoyakuNo.DBType, .YoyakuNo.IntegerBu, .YoyakuNo.DecimalBu))
            End With



            updateCount = MyBase.execNonQuery(oracleTransaction, sqlString.ToString)

            'If updateCount <= 0 Then

            ' ロールバック、コミットはコースメッセージの更新成功と同時に行う
            ' ロールバック
            'MyBase.callRollbackTransaction(oracleTransaction)
            ' Return False
            'End If
            'コミット
            ' MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            'MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return True

    End Function


    ''' <summary>
    ''' 予約情報(ピックアップ）更新
    ''' </summary>
    ''' <param name="yoyakuKbn">予約区分</param>
    ''' <param name="yoyakuNo">予約No</param>
    ''' <param name="gyouNo">行No</param>
    ''' <returns></returns>
    Public Function updateYoyakuBasicInfo(ByVal yoyakuKbn As String, ByVal yoyakuNo As String, gyouNo As Integer, oracleTransaction As OracleTransaction) As Boolean

        'Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Dim sqlString As New StringBuilder

        Try
            'トランザクション開始
            'oracleTransaction = MyBase.callBeginTransaction()

            ' update
            With yoyakuInfoBasicEntity
                sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC ")
                If gyouNo = 1 Then
                    ' メッセージチェックフラグ１を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_1 = '' ")

                ElseIf gyouNo = 2 Then
                    ' メッセージチェックフラグ２を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_2 = '' ")

                ElseIf gyouNo = 3 Then
                    ' メッセージチェックフラグ３を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_3 = '' ")


                ElseIf gyouNo = 4 Then
                    ' メッセージチェックフラグ４を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_4 = '' ")


                ElseIf gyouNo = 5 Then
                    ' メッセージチェックフラグ５を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_5 = '' ")


                ElseIf gyouNo = 6 Then
                    ' メッセージチェックフラグ６を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_6 = '' ")


                ElseIf gyouNo = 7 Then
                    ' メッセージチェックフラグ７を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_7 = '' ")



                ElseIf gyouNo = 8 Then
                    ' メッセージチェックフラグ８を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_8 = '' ")


                ElseIf gyouNo = 9 Then
                    ' メッセージチェックフラグ９を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_9 = '' ")


                ElseIf gyouNo = 10 Then
                    ' メッセージチェックフラグ10を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_10 = '' ")

                Else
                    ' 失敗
                End If
                sqlString.AppendLine(" WHERE YOYAKU_KBN = " & setParam("yoyakuKbn", yoyakuKbn, .YoyakuKbn.DBType, .YoyakuKbn.IntegerBu, .YoyakuKbn.DecimalBu))
                sqlString.AppendLine(" AND YOYAKU_NO = " & setParam("yoyakuNo", yoyakuNo, .YoyakuNo.DBType, .YoyakuNo.IntegerBu, .YoyakuNo.DecimalBu))
            End With

            updateCount = MyBase.execNonQuery(oracleTransaction, sqlString.ToString)


            ' ロールバック、コミットはコースメッセージの更新成功と同時に行う
            If updateCount <= 0 Then

                ' ロールバック
                'MyBase.callRollbackTransaction(oracleTransaction)
                Return False
            End If
            'コミット
            ' MyBase.callCommitTransaction(oracleTransaction)
        Catch ex As Exception

            ' ロールバック
            'MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        End Try

        Return True

    End Function

End Class
