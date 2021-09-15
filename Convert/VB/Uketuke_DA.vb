Imports System.Text


''' <summary>
''' コース台帳一括修正（受付開始日･予約停止･キャンセル料区分）のDAクラス
''' </summary>
Public Class Uketuke_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getUketuke                   '一覧結果取得検索
        executeInsertUketuke         '登録
        executeUpdateUketuke         '更新
        'executeReturnUketuke         '戻し
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessUketukeTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getUketuke
                '一覧結果取得検索
                sqlString = getUketuke(paramInfoList)
            Case Else
                '該当処理なし
                Return returnValue
        End Select

        Try
            returnValue = getDataTable(sqlString)
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getUketuke(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")  '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                              'コースコード
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")                               '曜日コード
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 ")                   '乗車地コード
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") '出発時間
        sqlString.AppendLine(",BASIC.GOUSYA ")                                              '号車
        sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN ")                             '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")                    '催行確定区分
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ")                                    '定期・企画区分
        'sqlString.AppendLine(",NVL(BASIC.UKETUKE_START_KAGETUMAE,'1') AS UKETUKE_START_KAGETUMAE ")                         '受付開始ヶ月前
        sqlString.AppendLine(",TRIM(NVL(BASIC.UKETUKE_START_KAGETUMAE,'1')) AS UKETUKE_START_KAGETUMAE ") '受付開始ヶ月前
        sqlString.AppendLine(",NVL(BASIC.UKETUKE_START_BI,'') AS UKETUKE_START_BI ")        '受付開始日
        sqlString.AppendLine(",Case WHEN NVL(BASIC.UKETUKE_START_DAY,0) = 0 THEN ")
        sqlString.AppendLine("'' ")
        sqlString.AppendLine("   Else TO_CHAR(TO_DATE(BASIC.UKETUKE_START_DAY),'yyyy/MM/dd') END AS UKETUKE_START_DAY ")    '受付開始日
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_STOP_FLG,'0') AS YOYAKU_STOP_FLG ")         '予約停止フラグ
        sqlString.AppendLine(",NVL(BASIC.CANCEL_RYOU_KBN,'') AS CANCEL_RYOU_KBN ")          'キャンセル料区分
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                                   'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")                             'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")                                 'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")                      '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                                    '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                                         '更新区分
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & FixedCd.CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & FixedCd.CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & FixedCd.CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.HAISYA_KEIYU_CD_1 = " & setParam("HAISYA_KEIYU_CD_1", paramList.Item("HAISYA_KEIYU_CD_1"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        If CType(paramList.Item("UNKYU_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.UNKYU_KBN = " & setParam("UNKYU_KBN", paramList.Item("UNKYU_KBN"), OracleDbType.Char))
        End If
        If CType(paramList.Item("SAIKOU_KAKUTEI_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
        End If
        If CType(paramList.Item("MARU_ZOU_MANAGEMENT_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN = " & setParam("MARU_ZOU_MANAGEMENT_KBN", paramList.Item("MARU_ZOU_MANAGEMENT_KBN"), OracleDbType.Char))
        End If
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.YOBI_CD = " & setParam("YOBI_CD", paramList.Item("YOBI_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")
        sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ")
        sqlString.AppendLine(" ,BASIC.GOUSYA ")

        Return sqlString.ToString
    End Function
#End Region

#Region " UPDATE処理 "

    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUketukeTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateUketuke
                    sqlString = executeUpdateBasicData(paramInfoList)
                    'Case accessType.executeReturnUketuke
                    '    sqlString = executeReturnBasicData(paramInfoList)
            End Select

            returnValue += execNonQuery(oracleTransaction, sqlString)

            If returnValue > 0 Then
                'コミット
                Call callCommitTransaction(oracleTransaction)
            Else
                'ロールバック
                Call callRollbackTransaction(oracleTransaction)
            End If

        Catch ex As Exception
            Call callRollbackTransaction(oracleTransaction)
            Throw

        Finally
            Call oracleTransaction.Dispose()
        End Try

        Return returnValue

    End Function

    '''' <summary>
    '''' 使用中フラグ更新
    '''' </summary>
    '''' <param name="selectOldData"></param>
    '''' <param name="systemupdatepgmid"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function executeUsingFlgCrs(ByVal selectOldData As DataTable, ByVal systemupdatepgmid As String) As DataTable

    '    Dim totalValue As New DataTable
    '    Dim trn As OracleTransaction = Nothing
    '    Dim returnValue As Boolean = False

    '    Try

    '        totalValue.Columns.Add("USING_FLG")             '使用中フラグ

    '        'トランザクション開始
    '        trn = callBeginTransaction()

    '        For Each row As DataRow In selectOldData.Rows
    '            Dim syuptsday As String = Replace(CType(row("SYUPT_DAY"), String), "/", "") '出発日
    '            Dim crscd As String = CType(row("CRS_CD"), String)                          'コースコード
    '            Dim gousya As String = CType(row("GOUSYA"), String)                         '号車

    '            returnValue = CommonCheckUtil.setUsingFlg_Crs(syuptsday, crscd, gousya, systemupdatepgmid, trn, True)

    '            Dim row2 As DataRow = totalValue.NewRow
    '            If returnValue = True Then
    '                row2("USING_FLG") = FixedCd.UsingFlg.Use
    '            Else
    '                row2("USING_FLG") = String.Empty
    '            End If

    '            totalValue.Rows.Add(row2)
    '        Next

    '        'コミット
    '        Call callCommitTransaction(trn)

    '    Catch ex As Exception
    '        'ロールバック
    '        Call callRollbackTransaction(trn)
    '        Throw

    '    Finally
    '        Call trn.Dispose()
    '    End Try

    '    Return totalValue

    'End Function

    ''' <summary>
    ''' コース台帳（基本）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBasicData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" UKETUKE_START_KAGETUMAE = " & setParam("UKETUKE_START_KAGETUMAE", paramList.Item("UKETUKE_START_KAGETUMAE"), OracleDbType.Char))
        sqlString.AppendLine(",UKETUKE_START_BI = " & setParam("UKETUKE_START_BI", paramList.Item("UKETUKE_START_BI"), OracleDbType.Decimal, 2, 0))
        sqlString.AppendLine(",UKETUKE_START_DAY = " & setParam("UKETUKE_START_DAY", paramList.Item("UKETUKE_START_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(",YOYAKU_STOP_FLG = " & setParam("YOYAKU_STOP_FLG", paramList.Item("YOYAKU_STOP_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",CANCEL_RYOU_KBN = " & setParam("CANCEL_RYOU_KBN", paramList.Item("CANCEL_RYOU_KBN"), OracleDbType.Char))
        'sqlString.AppendLine(",USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    '''' <summary>
    '''' コース台帳（基本）：データ戻し用
    '''' </summary>
    '''' <param name="paramList">SQL引数</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function executeReturnBasicData(ByVal paramList As Hashtable) As String

    '    Dim sqlString As New StringBuilder
    '    paramClear()

    '    'UPDATE
    '    sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
    '    sqlString.AppendLine(" SET ")
    '    sqlString.AppendLine("USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", paramList.Item("SYSTEM_UPDATE_DAY"), OracleDbType.Date))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
    '    'WHERE句
    '    sqlString.AppendLine(" WHERE ")
    '    sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
    '    sqlString.AppendLine(" AND ")
    '    sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
    '    sqlString.AppendLine(" AND ")
    '    sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

    '    Return sqlString.ToString

    'End Function

#End Region


End Class
