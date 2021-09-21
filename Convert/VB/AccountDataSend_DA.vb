Imports System.Text
''' <summary>
''' 会計データ送信管理
''' </summary>
''' <remarks>
''' Author:2018/10/06//PhucNH
''' </remarks>
Public Class AccountDataSend_DA  '会計データ送信管理_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private accountDataSendEntity As New TAccountsDataSendEntity()
#End Region

    ''' <summary>
    ''' 指定した年月で売掛情報テーブルにデータを取得する
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function countAccountDataSendByTaisoYm(ByVal paramList As Hashtable) As Integer

        Dim resultdatatable As New DataTable
        'sql生成
        Dim sql As New StringBuilder

        Try
            sql.AppendLine("SELECT COUNT(1) ")
            sql.AppendLine("FROM T_ACCOUNTS_DATA_SEND TADS ")
            sql.AppendLine("INNER JOIN ( ")
            sql.AppendLine("        SELECT TAISYO_YM ")
            sql.AppendLine("                ,PROCESS_TANI ")
            sql.AppendLine("                ,MANAGEMENT_SEC ")
            sql.AppendLine("                ,PROCESS_KBN ")
            sql.AppendLine("                ,MAX(SEQ) AS SEQ ")
            sql.AppendLine("        FROM T_ACCOUNTS_DATA_SEND ")
            With accountDataSendEntity
                sql.AppendLine("        WHERE PROCESS_TANI = '0' ")
                sql.AppendLine("            AND MANAGEMENT_SEC IN ('1', '2', '3') ")
                sql.AppendLine("            AND PROCESS_KBN = '1' ")
                sql.AppendLine("            AND TAISYO_YM = " & setParam("TaisoYm", paramList.Item("TaisoYm"), .taisyoYm.DBType, .taisyoYm.IntegerBu, .taisyoYm.DecimalBu))
            End With
            sql.AppendLine("        GROUP BY TAISYO_YM ")
            sql.AppendLine("                ,PROCESS_TANI ")
            sql.AppendLine("                ,MANAGEMENT_SEC ")
            sql.AppendLine("                ,PROCESS_KBN ")
            sql.AppendLine("        ) M ON TADS.TAISYO_YM = M.TAISYO_YM ")
            sql.AppendLine("        AND TADS.PROCESS_TANI = M.PROCESS_TANI ")
            sql.AppendLine("        AND TADS.MANAGEMENT_SEC = M.MANAGEMENT_SEC ")
            sql.AppendLine("        AND TADS.PROCESS_KBN = M.PROCESS_KBN ")
            sql.AppendLine("        AND TADS.SEQ = M.SEQ ")
            sql.AppendLine("        AND TADS.PROCESS_STATUS = '1' ")
            resultdatatable = getDataTable(sql.ToString)
        Catch ex As Exception
            Throw
        End Try

        If resultdatatable.Rows.Count > 0 Then

            Return CInt(resultdatatable.Rows.Item(0).Item(0))
        Else
            Return 0
        End If
    End Function

End Class
