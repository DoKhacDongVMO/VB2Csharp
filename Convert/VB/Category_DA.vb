Imports System.Text


''' <summary>
''' コース台帳一括修正（カテゴリ）のDAクラス
''' </summary>
Public Class Category_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getCategory                   '一覧結果取得検索
        executeInsertCategory         '登録
        executeUpdateCategory         '更新
        'executeReturnCategory         '戻し
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessCategoryTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getCategory
                '一覧結果取得検索
                sqlString = getCategory(paramInfoList)
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
    Protected Overloads Function getCategory(ByVal paramList As Hashtable) As String

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
        sqlString.AppendLine(",BASIC.UNKYU_KBN ")                                           '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")                    '催行確定区分
        sqlString.AppendLine(",BASIC.CATEGORY_CD_1 ")                                       'カテゴリー1
        sqlString.AppendLine(",BASIC.CATEGORY_CD_2 ")                                       'カテゴリー2
        sqlString.AppendLine(",BASIC.CATEGORY_CD_3 ")                                       'カテゴリー3
        sqlString.AppendLine(",BASIC.CATEGORY_CD_4 ")                                       'カテゴリー4
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
    Public Function executeCategoryTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateCategory
                    sqlString = executeUpdateBasicData(paramInfoList)
                    'Case accessType.executeReturnCategory
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
        sqlString.AppendLine(" CATEGORY_CD_1 = " & setParam("CATEGORY_CD_1", paramList.Item("CATEGORY_CD_1"), OracleDbType.Char))
        sqlString.AppendLine(",CATEGORY_CD_2 = " & setParam("CATEGORY_CD_2", paramList.Item("CATEGORY_CD_2"), OracleDbType.Char))
        sqlString.AppendLine(",CATEGORY_CD_3 = " & setParam("CATEGORY_CD_3", paramList.Item("CATEGORY_CD_3"), OracleDbType.Char))
        sqlString.AppendLine(",CATEGORY_CD_4 = " & setParam("CATEGORY_CD_4", paramList.Item("CATEGORY_CD_4"), OracleDbType.Char))
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
