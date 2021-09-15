Imports System.Text

Public Class UsingFlg_DA
    Inherits DataAccessorBase

#Region "変数／定数(Public)"
    Public Structure LOCK_TARGET
        Public basic As Boolean
        'Public zaseki As Boolean
        'Public yoyaku As Boolean
    End Structure

    Public Structure USING_CHECK
        Public basic As Boolean
        Public busreserve As Boolean
    End Structure


    Public Enum LOCK_MODE
        USE
        REJECT
    End Enum

#End Region

#Region "変数／定数(Private)"
    Private baseTbl As DataTable = Nothing    '対象データテーブル(コンストラクタで作成)
    Private lockTbl As DataTable = Nothing    '対象データテーブル(ロック)

    Private pUserId As String = UserInfoManagement.userId
    Private pPgmId As String
    Private pSysDate As Date
#End Region

#Region "列挙体"
    ''' <summary>
    ''' テーブルモード
    ''' </summary>
    Public Enum MODE_TYPE
        BASIC
        BUS
    End Enum

    Private Enum accessType As Integer
        getUsingFlgList                    '使用中フラグリスト取得
    End Enum

    ''' <summary>
    ''' 取得モード
    ''' </summary>
    Private Enum SELECT_MODE
        STANDARD
        UPDATEERROR
    End Enum

    ''' <summary>
    ''' 更新モード
    ''' </summary>
    Private Enum UPDATE_MODE
        USE
        UNUSE_NOUPDATE
        UNUSE_UPDATE
    End Enum
#End Region

#Region "構造体"
    ''' <summary>
    ''' カラムキー
    ''' </summary>
    Private Structure ColumnKeys
        Public key1 As String
        Public key2 As String
        Public key3 As String
    End Structure

    ''' <summary>
    ''' カラムID
    ''' </summary>
    Private Structure KEY_COLUMNID
        Public Const CRS_CD As String = "CRS_CD"
        Public Const GOUSYA As String = "GOUSYA"
        Public Const SYUPT_DAY As String = "SYUPT_DAY"
        Public Const BUS_RESERVE_CD As String = "BUS_RESERVE_CD"
        Public Const USING_FLG As String = "USING_FLG"
    End Structure
#End Region

#Region "コンストラクタ"
    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="dt">対象のDataTable</param>
    Public Sub New(ByVal dt As DataTable, ByVal pgmId As String)
        baseTbl = dt.Copy
        pPgmId = pgmId
    End Sub
#End Region

#Region "公開メソッド"
    Public Function executeUsingFlg(ByVal locktype As LOCK_TARGET, ByVal chkFlg As USING_CHECK, ByVal lockMode As LOCK_MODE) As Integer
        Dim tran As OracleTransaction = Nothing
        Dim retValue As Integer = 0

        If LOCK_MODE.USE.Equals(lockMode) Then
            'ロック処理
            If locktype.basic = True Then
                '[条件作成]
                Dim listWhere As List(Of String) = getWhereStr(baseTbl, MODE_TYPE.BASIC)
                If listWhere.Count = 0 Then
                    '条件無しは失敗と見なす
                    Return -1
                Else
                    Try
                        'トランザクション開始
                        tran = callBeginTransaction()
                        '対象データ取得(状況取得)
                        lockTbl = getUsingListData(listWhere, tran, SELECT_MODE.STANDARD, chkFlg)
                        'ロック処理
                        retValue = setUsingFlg(lockTbl, locktype, chkFlg, lockMode, tran)
                        'コミット
                        Call callCommitTransaction(tran)
                    Catch ex As Exception
                        'ロールバック
                        Call callRollbackTransaction(tran)
                        Throw
                    Finally
                        Call tran.Dispose()
                    End Try
                End If
            End If
        Else
            'ロック解除処理
            If locktype.basic = True Then
                If lockTbl Is Nothing Then
                    'ロック対象無しはエラー
                    Return -1
                Else
                    Try
                        'トランザクション開始
                        tran = callBeginTransaction()
                        'ロック解除処理
                        retValue = setUsingFlg(lockTbl, locktype, chkFlg, lockMode, tran)
                        'コミット
                        Call callCommitTransaction(tran)
                        lockTbl = Nothing
                    Catch ex As Exception
                        'ロールバック
                        Call callRollbackTransaction(tran)
                        Throw
                    Finally
                        Call tran.Dispose()
                    End Try
                End If
            End If
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' 使用中フラグの項目を更新する
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <returns></returns>
    Public Function setUsingFlgColumnData(ByRef prmDt As DataTable, ByVal prmType As MODE_TYPE, Optional columnNm As String = KEY_COLUMNID.USING_FLG) As Boolean
        Dim colUsingFlg As String = String.Empty
        Dim strSelect As String = String.Empty

        'DataTable or カラム無し
        If prmDt Is Nothing OrElse prmDt.Columns.Contains(columnNm) = False Then
            Return False
        End If
        'ロックテーブル無

        '[カラムチェック]
        If Not lockTbl Is Nothing AndAlso chkExistsKeys(prmDt, prmType) = True Then
            'ロック用テーブルに存在するもののみを更新とする
            For Each row As DataRow In lockTbl.Rows
                strSelect = getKeyStr(row, prmType)
                For Each dr In prmDt.Select(strSelect)
                    dr(columnNm) = row(KEY_COLUMNID.USING_FLG)
                Next
            Next
        Else
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' 使用中フラグ情報テーブル
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property UsingFlgTable As DataTable
        Get
            Return lockTbl
        End Get
    End Property

#End Region

#Region "Private変数"
    ''' <summary>
    ''' カラムの存在チェック
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <param name="prmType"></param>
    ''' <returns></returns>
    Private Function chkExistsKeys(ByVal prmDt As DataTable, ByVal prmType As MODE_TYPE) As Boolean
        Dim keys As ColumnKeys = getKey(prmType)
        If prmDt.Columns.Contains(keys.key1) = False OrElse
           prmDt.Columns.Contains(keys.key2) = False OrElse
           prmDt.Columns.Contains(keys.key3) = False Then
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' WHERE条件作成用
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <param name="prmType"></param>
    ''' <returns></returns>
    Private Function getWhereStr(ByVal prmDt As DataTable, ByVal prmType As MODE_TYPE) As List(Of String)
        Dim keyStr As New Text.StringBuilder
        Dim keys As ColumnKeys = getKey(prmType)
        Dim tmpTbl As DataTable
        Dim list As New List(Of String)
        Dim rowIdx As Integer = 0

        '[カラムチェック]
        If chkExistsKeys(prmDt, prmType) = True Then
            '[WHERE作成]
            'IN句(1000件以上対応)
            tmpTbl = New DataView(prmDt).ToTable(True, {keys.key1, keys.key2, keys.key3})

            For Each dr As DataRow In tmpTbl.Rows
                If String.Empty.Equals(keyStr.ToString) = False Then
                    keyStr.Append(",")
                End If
                keyStr.AppendLine(String.Format("('{0}',{1},{2})", dr(keys.key1).ToString, dr(keys.key2).ToString, dr(keys.key3).ToString))
                rowIdx = rowIdx + 1
                If rowIdx = 1000 Then
                    list.Add(keyStr.ToString)
                    keyStr.Clear()
                    rowIdx = 0
                End If
            Next
            If keyStr.Length > 0 Then
                list.Add(keyStr.ToString)
            End If
        End If

        Return list
    End Function

    ''' <summary>
    ''' WHERE条件作成用
    ''' </summary>
    ''' <param name="prmRow"></param>
    ''' <param name="prmType"></param>
    ''' <returns></returns>
    Private Function getKeyStr(ByVal prmRow As DataRow, ByVal prmType As MODE_TYPE) As String
        Dim keys As ColumnKeys = getKey(prmType)
        Return String.Format(keys.key1 & "='{0}' AND " & keys.key2 & "={1} AND " & keys.key3 & "={2}", prmRow(keys.key1).ToString, prmRow(keys.key2).ToString, prmRow(keys.key3).ToString)
    End Function

    ''' <summary>
    ''' 必要カラムキー取得
    ''' </summary>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    Private Function getKey(ByVal mode As MODE_TYPE) As ColumnKeys
        Dim keys As New ColumnKeys

        If MODE_TYPE.BASIC.Equals(mode) Then
            keys.key1 = KEY_COLUMNID.CRS_CD
        Else
            keys.key1 = KEY_COLUMNID.BUS_RESERVE_CD
        End If
        keys.key2 = KEY_COLUMNID.GOUSYA
        keys.key3 = KEY_COLUMNID.SYUPT_DAY

        Return keys
    End Function

    ''' <summary>
    ''' 使用中フラグデータ取得
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <param name="prmType"></param>
    ''' <returns></returns>
    Private Function getUsingFlgData(ByVal prmDt As DataTable, ByVal prmType As String) As DataTable
        Dim tmpDt As DataTable
        Dim dr As DataRow()
        tmpDt = prmDt.Clone

        If prmType Is Nothing Then
            dr = prmDt.Select(String.Format("USING_FLG IS NULL", prmType))
        Else
            dr = prmDt.Select(String.Format("USING_FLG = '{0}'", prmType))
        End If
        If dr.Count > 0 Then
            tmpDt = dr.CopyToDataTable
        Else
            tmpDt = tmpDt.Clone
        End If
        Return tmpDt

    End Function

#End Region























    Private Function getUsingListData(ByVal paramInfoList As List(Of String), ByVal tran As OracleTransaction, ByVal mode As SELECT_MODE, ByVal chkMode As USING_CHECK) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        Dim tmpTable As DataTable = Nothing

        '戻り値
        Dim returnValue As DataTable = Nothing
        Try
            For Each strWhere As String In paramInfoList
                tmpTable = getDataTable(tran, getUsingListSql(strWhere, mode, chkMode))

                If returnValue Is Nothing Then
                    returnValue = tmpTable
                ElseIf Not tmpTable Is Nothing Then
                    returnValue.Merge(tmpTable)
                End If
            Next
        Catch ex As Exception
            Throw
        End Try

        Return returnValue
    End Function

    Private Function getUsingListSql(ByVal strWhereIn As String, ByVal mode As SELECT_MODE, ByVal chkMode As USING_CHECK) As String
        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine("    BASIC.CRS_CD ")                         'コースコード
        sqlString.AppendLine("   ,BASIC.GOUSYA ")                         '号車
        sqlString.AppendLine("   ,BASIC.SYUPT_DAY ")                      '出発日
        sqlString.AppendLine("   ,BASIC.BUS_RESERVE_CD ")                 'バス指定コード
        '使用中フラグ
        If chkMode.basic = True Andalso chkMode.busreserve = True Then
            sqlString.AppendLine("   ,CASE WHEN BASIC.USING_FLG IS NULL AND SUB.BUS_USING_FLG IS NULL THEN NULL ELSE 'Y' END USING_FLG ")                 'バス指定コード
        ElseIf chkMode.busreserve = True Then
            sqlString.AppendLine("   ,SUB.BUS_USING_FLG AS USING_FLG ")   '使用中フラグ(バス指定コード) ※1階層までなのには注意
        Else
            sqlString.AppendLine("   ,BASIC.USING_FLG AS USING_FLG ")     '使用中フラグ(台帳基本)
        End If
        sqlString.AppendLine("   ,BASIC.USING_FLG AS BASIC_USING_FLG ")   '使用中フラグ(台帳基本)
        sqlString.AppendLine("   ,SUB.BUS_USING_FLG ")                    '使用中フラグ(バス指定コード)
        sqlString.AppendLine("   ,BASIC.SYSTEM_UPDATE_PGMID ")            'システム更新ＰＧＭＩＤ
        sqlString.AppendLine("   ,BASIC.SYSTEM_UPDATE_PERSON_CD ")        'システム更新者コード
        sqlString.AppendLine("   ,BASIC.SYSTEM_UPDATE_DAY ")              'システム更新日

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("   T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("   LEFT JOIN ")
        sqlString.AppendLine("   ( ")
        sqlString.AppendLine("     SELECT")
        sqlString.AppendLine("        CASE WHEN COUNT(BUS_RESERVE_CD) > 0 THEN 'Y' ELSE NULL END BUS_USING_FLG")
        sqlString.AppendLine("       ,BUS_RESERVE_CD")
        sqlString.AppendLine("       ,GOUSYA")
        sqlString.AppendLine("       ,SYUPT_DAY")
        sqlString.AppendLine("     FROM")
        sqlString.AppendLine("       T_CRS_LEDGER_BASIC")
        sqlString.AppendLine("     WHERE")
        sqlString.AppendLine("       (BUS_RESERVE_CD, GOUSYA, SYUPT_DAY)")
        sqlString.AppendLine("       IN")
        sqlString.AppendLine("       (")
        sqlString.AppendLine("        " & strWhereIn)
        sqlString.AppendLine("       )")
        sqlString.AppendLine("       AND")
        sqlString.AppendLine("       USING_FLG ='Y'")
        sqlString.AppendLine("     GROUP BY")
        sqlString.AppendLine("       BUS_RESERVE_CD,GOUSYA,SYUPT_DAY")
        sqlString.AppendLine("   ) SUB ")
        sqlString.AppendLine("   ON ")
        sqlString.AppendLine("     BASIC.BUS_RESERVE_CD = SUB.BUS_RESERVE_CD")
        sqlString.AppendLine("     AND")
        sqlString.AppendLine("     BASIC.GOUSYA = SUB.GOUSYA")
        sqlString.AppendLine("     AND")
        sqlString.AppendLine("     BASIC.SYUPT_DAY = SUB.SYUPT_DAY")
        sqlString.AppendLine("     AND")
        sqlString.AppendLine("     NVL(BASIC.DELETE_DAY, 0) = 0")

        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine("   (BASIC.CRS_CD, BASIC.GOUSYA, BASIC.SYUPT_DAY)")
        sqlString.AppendLine("   IN")
        sqlString.AppendLine("   (")
        sqlString.AppendLine("      " & strWhereIn)
        sqlString.AppendLine("   )")
        sqlString.AppendLine("     AND")
        sqlString.AppendLine("     NVL(BASIC.DELETE_DAY, 0) = 0")
        If SELECT_MODE.UPDATEERROR.Equals(mode) Then
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   BASIC.USING_FLG = {0}", setParam("USING_FLG", UsingFlg.Use)))
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine("   (")
            sqlString.AppendLine(String.Format("     BASIC.SYSTEM_UPDATE_PGMID <> {0}", setParam("SYSTEM_UPDATE_PGMID", pPgmId, OracleDbType.Varchar2)))
            sqlString.AppendLine("     OR ")
            sqlString.AppendLine(String.Format("     BASIC.SYSTEM_UPDATE_PERSON_CD <> {0}", setParam("SYSTEM_UPDATE_PERSON_CD", pUserId, OracleDbType.Varchar2)))
            sqlString.AppendLine("     OR ")
            sqlString.AppendLine(String.Format("     BASIC.SYSTEM_UPDATE_DAY <> {0}", setParam("SYSTEM_UPDATE_DAY", pSysDate, OracleDbType.Date)))
            sqlString.AppendLine("   )")
        End If

        Return sqlString.ToString
    End Function

    Private Function updateListData(ByVal paramInfoList As List(Of String), ByVal tran As OracleTransaction, ByVal mode As UPDATE_MODE) As Integer
        Return updateListData(paramInfoList, tran, mode, Nothing)
    End Function

    Private Function updateListData(ByVal paramInfoList As List(Of String), ByVal tran As OracleTransaction, ByVal mode As UPDATE_MODE, ByVal paramInfoList2 As List(Of String)) As Integer
        'SQL文字列
        Dim sqlString As String = String.Empty
        Dim returnValue As Integer = 0

        '戻り値
        Try
            If paramInfoList2 Is Nothing Then
                For Each strWhere As String In paramInfoList
                    returnValue += execNonQuery(tran, updateListDataSql(strWhere, mode, String.Empty))
                Next
            Else
                For listIdx As Integer = 0 To paramInfoList.Count - 1
                    returnValue += execNonQuery(tran, updateListDataSql(paramInfoList(listIdx), mode, paramInfoList2(listIdx)))
                Next
            End If
        Catch ex As Exception
            Throw
        End Try

        Return returnValue
    End Function

    Private Function updateListDataSql(ByVal strWhereIn As String, ByVal mode As UPDATE_MODE, ByVal strSelect As String) As String
        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC MAIN")
        sqlString.AppendLine("   SET")
        If UPDATE_MODE.USE.Equals(mode) Then
            sqlString.AppendLine(String.Format("      USING_FLG = {0}", setParam("USING_FLG", UsingFlg.Use)))       '使用中フラグ
            sqlString.AppendLine(String.Format("     ,SYSTEM_UPDATE_PGMID = {0}", setParam("SYSTEM_UPDATE_PGMID", pPgmId, OracleDbType.Varchar2)))            'システム更新ＰＧＭＩＤ
            sqlString.AppendLine(String.Format("     ,SYSTEM_UPDATE_PERSON_CD = {0}", setParam("SYSTEM_UPDATE_PERSON_CD", pUserId, OracleDbType.Varchar2)))   'システム更新者コード
            sqlString.AppendLine(String.Format("     ,SYSTEM_UPDATE_DAY = {0}", setParam("SYSTEM_UPDATE_DAY", pSysDate, OracleDbType.Date)))   'システム更新日
        ElseIf UPDATE_MODE.UNUSE_NOUPDATE.Equals(mode) Then
            sqlString.AppendLine("      ( ")
            sqlString.AppendLine("        USING_FLG ")                  '使用中フラグ
            sqlString.AppendLine("       ,SYSTEM_UPDATE_PGMID ")        'システム更新ＰＧＭＩＤ
            sqlString.AppendLine("       ,SYSTEM_UPDATE_PERSON_CD ")    'システム更新者コード
            sqlString.AppendLine("       ,SYSTEM_UPDATE_DAY ")          'システム更新日
            sqlString.AppendLine("      ) = ")
            sqlString.AppendLine("      ( ")
            sqlString.AppendLine("        SELECT ")
            sqlString.AppendLine("            NULL ")                       '使用中フラグ
            sqlString.AppendLine("          , SYSTEM_UPDATE_PGMID ")        'システム更新ＰＧＭＩＤ
            sqlString.AppendLine("          , SYSTEM_UPDATE_PERSON_CD ")    'システム更新者コード
            sqlString.AppendLine("          , TO_DATE(SYSTEM_UPDATE_DAY, 'YYYY/MM/DD HH24:MI:SS') ")          'システム更新日
            sqlString.AppendLine("        FROM ")
            sqlString.AppendLine("          ( ")
            sqlString.AppendLine(strSelect)
            sqlString.AppendLine("          ) SUB ")
            sqlString.AppendLine("        WHERE ")
            sqlString.AppendLine("            SUB.CRS_CD = MAIN.CRS_CD ")
            sqlString.AppendLine("            AND ")
            sqlString.AppendLine("            SUB.SYUPT_DAY = MAIN.SYUPT_DAY ")
            sqlString.AppendLine("            AND ")
            sqlString.AppendLine("            SUB.GOUSYA = MAIN.GOUSYA ")
            sqlString.AppendLine("      ) ")
        ElseIf UPDATE_MODE.UNUSE_UPDATE.Equals(mode) Then
            sqlString.AppendLine("      USING_FLG = NULL")       '使用中フラグ
        End If
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine("   (CRS_CD, GOUSYA, SYUPT_DAY)")
        sqlString.AppendLine("   IN")
        sqlString.AppendLine("   (")
        sqlString.AppendLine("      " & strWhereIn)
        sqlString.AppendLine("   )")
        If UPDATE_MODE.USE.Equals(mode) Then
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   USING_FLG IS NULL"))
        ElseIf UPDATE_MODE.UNUSE_NOUPDATE.Equals(mode) Then
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   USING_FLG = {0}", setParam("USING_FLG", UsingFlg.Use)))
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   SYSTEM_UPDATE_DAY <= {0}", setParam("SYSTEM_UPDATE_DAY", pSysDate, OracleDbType.Date)))
        ElseIf UPDATE_MODE.UNUSE_UPDATE.Equals(mode) Then
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   USING_FLG = {0}", setParam("USING_FLG", UsingFlg.Use)))
            sqlString.AppendLine("   AND ")
            sqlString.AppendLine(String.Format("   SYSTEM_UPDATE_DAY > {0}", setParam("SYSTEM_UPDATE_DAY", pSysDate, OracleDbType.Date)))
        End If

        Return sqlString.ToString
    End Function


    ''' <summary>
    ''' 使用中フラグデータ取得
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <param name="locktype"></param>
    ''' <param name="lockMode"></param>
    ''' <param name="tran"></param>
    ''' <returns></returns>
    Private Function setUsingFlg(ByVal prmDt As DataTable, ByVal locktype As LOCK_TARGET, ByVal chkFlg As USING_CHECK, ByVal lockMode As LOCK_MODE, ByVal tran As OracleTransaction) As Integer
        Dim updateTable As DataTable = Nothing
        Dim listWhere As List(Of String) = Nothing
        Dim listSelect As List(Of String) = Nothing
        Dim updCnt As Integer = 0
        Dim updateErrorTable As DataTable = Nothing

        If LOCK_MODE.USE.Equals(lockMode) Then
            pSysDate = CommonDateUtil.getSystemTime
            If locktype.basic = True Then
                '更新対象取得
                updateTable = getUsingFlgData(prmDt, UsingFlg.Unused)
                listWhere = getWhereStr(updateTable, MODE_TYPE.BASIC)
                updCnt = updateListData(listWhere, tran, UPDATE_MODE.USE)
                If updCnt <> updateTable.Rows.Count Then
                    updateErrorTable = getUsingListData(listWhere, tran, SELECT_MODE.UPDATEERROR, chkFlg)
                    'prmDtにUsingFlgを反映

                End If
            End If
        Else
            If locktype.basic = True Then
                '更新対象取得
                updateTable = getUsingFlgData(prmDt, UsingFlg.Unused)
                listWhere = getWhereStr(updateTable, MODE_TYPE.BASIC)
                listSelect = getSelectStr(updateTable, MODE_TYPE.BASIC)
                '解除(更新レコード)
                updCnt = updateListData(listWhere, tran, UPDATE_MODE.UNUSE_UPDATE)
                '解除(未更新レコード)
                updCnt += updateListData(listWhere, tran, UPDATE_MODE.UNUSE_NOUPDATE, listSelect)
            End If
        End If

        Return updCnt
    End Function

    ''' <summary>
    ''' WHERE条件作成用
    ''' </summary>
    ''' <param name="prmDt"></param>
    ''' <param name="prmType"></param>
    ''' <returns></returns>
    Private Function getSelectStr(ByVal prmDt As DataTable, ByVal prmType As MODE_TYPE) As List(Of String)
        Dim keyStr As New Text.StringBuilder
        Dim list As New List(Of String)
        Dim rowIdx As Integer = 0
        Dim key1 As String = String.Empty

        If MODE_TYPE.BASIC.Equals(prmType) Then
            key1 = KEY_COLUMNID.CRS_CD
        Else
            key1 = KEY_COLUMNID.BUS_RESERVE_CD
        End If

        '[SELECT句作成]
        For Each dr As DataRow In prmDt.Rows
            If String.Empty.Equals(keyStr.ToString) = False Then
                keyStr.AppendLine("UNION ALL")
            End If
            keyStr.AppendLine(String.Format("            SELECT '{0}' AS {1}, {2} AS GOUSYA, {3} AS SYUPT_DAY, '{4}' AS SYSTEM_UPDATE_PGMID, '{5}' AS SYSTEM_UPDATE_PERSON_CD, '{6}' AS SYSTEM_UPDATE_DAY FROM dual", dr(key1).ToString, key1, dr("GOUSYA").ToString, dr("SYUPT_DAY").ToString, dr("SYSTEM_UPDATE_PGMID").ToString, dr("SYSTEM_UPDATE_PERSON_CD").ToString, dr("SYSTEM_UPDATE_DAY").ToString))
            rowIdx = rowIdx + 1
            If rowIdx = 1000 Then
                list.Add(keyStr.ToString)
                keyStr.Clear()
                rowIdx = 0
            End If
        Next
        If keyStr.Length > 0 Then
            list.Add(keyStr.ToString)
        End If

        Return list
    End Function

End Class
