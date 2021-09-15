Imports System.Text

''' <summary>
''' 手仕舞い情報登録のDAクラス
''' </summary>
Public Class S03_0411DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

#Region "SELECT処理 "
    ''' <summary>
    ''' 検索処理を呼び出す（宛先情報）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectTCeaControl(ByVal param As TCeaDAParam) As DataTable
        Dim controlEtt As New TCeaControlEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("     CRS_CD")                                                        'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD")                                            '仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME")                                               '仕入先名
        sb.AppendLine("     , TEL1 ")                                                         'TEL1
        sb.AppendLine("     , TEL1_1 ")                                                       'TEL1_1
        sb.AppendLine("     , TEL1_2 ")                                                       'TEL1_2
        sb.AppendLine("     , TEL1_3 ")                                                       'TEL1_3
        sb.AppendLine("     , FAX1 ")                                                         'FAX1
        sb.AppendLine("     , FAX1_1 ")                                                       'FAX1_1
        sb.AppendLine("     , FAX1_2 ")                                                       'FAX1_2
        sb.AppendLine("     , FAX1_3 ")                                                       'FAX1_3
        sb.AppendLine("     , FAX2 ")                                                         'FAX2
        sb.AppendLine("     , FAX2_1 ")                                                       'FAX2_1
        sb.AppendLine("     , FAX2_2 ")                                                       'FAX2_2
        sb.AppendLine("     , FAX2_3 ")                                                       'FAX2_3
        sb.AppendLine("     , MAIL ")                                                         'メールアドレス
        sb.AppendLine("     , SEND_KIND ")                                                    '通知方法
        sb.AppendLine("     , SEND_YMDT ")                                                    '最終送信日時
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ")                                            'システム更新日
        'FROM句
        sb.AppendLine("FROM T_CEA_CONTROL")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, controlEtt.CrsCd))
        End If
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo))
        End If
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, controlEtt.Daily))

        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 検索処理を呼び出す (手仕舞い連絡表　他コース)
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectTCeaOthercrs(ByVal param As TCeaDAParam) As DataTable
        Dim othercrsEtt As New TCeaOthercrsEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("     CRS_CD")                                                        'コースコード
        sb.AppendLine("     , DAILY ")                                                                  '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , OTHER_CRS_CD")                                                  '他コースコード
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ")                                            'システム更新日
        'FROM句
        sb.AppendLine("FROM T_CEA_OTHERCRS")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, othercrsEtt.CrsCd))
        End If
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo))
        End If
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, othercrsEtt.Daily))
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 検索処理を呼び出す （コース情報）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectTCeaCrsInfo(ByVal param As TCeaDAParam) As DataTable
        Dim crsInfoEtt As New TCeaCrsInfoEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("      CRS_CD")                                                        'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , CRS_INFO_NO")                                                   'コース情報№
        sb.AppendLine("     , CRS_INFO")                                                      'コース情報
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ")                                            'システム更新日
        'FROM句
        sb.AppendLine("FROM T_CEA_CRS_INFO")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, crsInfoEtt.CrsCd))
        End If
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo))
        End If
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, crsInfoEtt.Daily))
        'ソート条件（Order By）
        sb.AppendLine("  ORDER BY CRS_INFO_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 検索処理を呼び出す（通信欄）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectTCeaReaders(ByVal param As TCeaDAParam) As DataTable
        Dim readersEtt As New TCeaReadersEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("     CRS_CD")                                                        'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , READERS_COL_NO")                                                '通信欄№
        sb.AppendLine("     , READERS_COL")                                                   '通信欄
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ")                                            'システム更新日
        'FROM句
        sb.AppendLine("FROM T_CEA_READERS")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, readersEtt.CrsCd))
        End If
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo))
        End If
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, readersEtt.Daily))
        'ソート条件（Order By）
        sb.AppendLine("  ORDER BY READERS_COL_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 検索処理を呼び出す（特記事項）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectTCeaNoteworthy(ByVal param As TCeaDAParam) As DataTable
        Dim noteworthyEtt As New TCeaNoteworthyEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("     CRS_CD")                                                        'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , NOTEWORTHY_NO")                                                 '特記事項№
        sb.AppendLine("     , NOTEWORTHY")                                                    '特記事項
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY ")                                            'システム更新日
        'FROM句
        sb.AppendLine("FROM T_CEA_NOTEWORTHY")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, noteworthyEtt.CrsCd))
        End If
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, noteworthyEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, noteworthyEtt.SiireSakiNo))
        End If
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, noteworthyEtt.Daily))
        'ソート条件（Order By）
        sb.AppendLine("  ORDER BY NOTEWORTHY_NO")
        Return MyBase.getDataTable(sb.ToString)
    End Function
    ''' <summary>
    ''' 検索処理を呼び出す（手仕舞い表にデータがない場合）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectMSiireSaki(ByVal param As TCeaDAParam) As DataTable
        Dim mSiireSakiEtt As New MSiireSakiEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("     SIIRE_SAKI_CD")                                                   '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD")                                            '仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME")                                               '仕入先名
        sb.AppendLine("     , TELNO_1 ")                                                      '電話番号１
        sb.AppendLine("     , TELNO_1_1 ")                                                    '電話番号１－１
        sb.AppendLine("     , TELNO_1_2 ")                                                    '電話番号１－２
        sb.AppendLine("     , TELNO_1_3 ")                                                    '電話番号１－３
        sb.AppendLine("     , FAX_1 ")                                                        'FAX1
        sb.AppendLine("     , FAX_1_1 ")                                                      'FAX1_1
        sb.AppendLine("     , FAX_1_2 ")                                                      'FAX1_2
        sb.AppendLine("     , FAX_1_3 ")                                                      'FAX1_3
        sb.AppendLine("     , FAX_2 ")                                                        'FAX2
        sb.AppendLine("     , FAX_2_1 ")                                                      'FAX2_1
        sb.AppendLine("     , FAX_2_2 ")                                                      'FAX2_2
        sb.AppendLine("     , FAX_2_3 ")                                                      'FAX2_3
        sb.AppendLine("     , MAIL ")                                                         'メールアドレス
        'FROM句
        sb.AppendLine("FROM M_SIIRE_SAKI")
        'WHERE
        sb.AppendLine("WHERE 1 = 1 ")
        '仕入先コード
        If Not param.SiireSakiCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, mSiireSakiEtt.SiireSakiCd))
        End If
        '仕入先枝番
        If Not param.SiireSakiNo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, mSiireSakiEtt.SiireSakiNo))
        End If
        sb.AppendLine(" AND DELETE_DATE IS NULL")
        Return MyBase.getDataTable(sb.ToString)
    End Function

    '' <summary>
    '' 検索処理を呼び出す コース台帳（基本）
    '' </summary>
    '' <param name="param"></param>
    '' <returns>DataTable</returns>
    Public Function selectLedgerBasic(ByVal param As TCeaLedgerBasicParam) As DataTable
        Dim ledgerBasicEtt As New TCrsLedgerBasicEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine("   SELECT ")
        sb.AppendLine("   CRS_CD ")
        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC")
        'WHERE
        sb.AppendLine(" WHERE 1 = 1 ")
        'コースコード
        If Not param.CrsCd Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, ledgerBasicEtt.crsCd))
        End If
        '出発日
        sb.AppendLine("  AND ")
        sb.AppendLine("  SYUPT_DAY = ").Append(setDataParam(param.SyuptDay, ledgerBasicEtt.syuptDay))
        Return MyBase.getDataTable(sb.ToString)
    End Function


#End Region

#Region "DELETE/INSERT 処理 "
    ''' <summary>
    ''' 削除/登録 処理
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' 
    Public Function executeInsertDelete(ByVal paramDA As TCeaDAParam, ByVal paramTCeaControl As TCeaControlParam,
                                  ByVal listParamTCeaOthercrs As List(Of TCeaOthercrsParam), ByVal listParamTCeaCrsInfo As List(Of TCeaCrsInfoParam),
                                  ByVal listParamTCeaReaders As List(Of TCeaReadersParam), ByVal listParamTCeaNoteworthy As List(Of TCeaNoteworthyParam)) As Integer
        Dim oraTran As OracleTransaction = Nothing
        'SQL文字列
        Try
            'トランザクション開始
            oraTran = callBeginTransaction()
            '実行
            '削除（手仕舞い連絡表）
            Dim sqlStringDeteleTCeaControl As String = executeDeleteTCeaControl(paramDA)
            execNonQuery(oraTran, sqlStringDeteleTCeaControl)
            '登録（手仕舞い連絡表）
            Dim sqlStringTCeaControl As String = executeInsertTCeaControl(paramTCeaControl)
            execNonQuery(oraTran, sqlStringTCeaControl)
            '※他コース情報のコースコードに入力がない場合はDA定義No8をスキップする
            If (listParamTCeaOthercrs.Count <> 0) Then
                '実行
                '削除（手仕舞い連絡表 他コース）
                Dim sqlStringDeteleTCeaOthercrs As String = executeDeleteTCeaOthercrs(paramDA)
                execNonQuery(oraTran, sqlStringDeteleTCeaOthercrs)
                For Each paramTCeaOthercrs As TCeaOthercrsParam In listParamTCeaOthercrs
                    '登録（手仕舞い連絡表　他コース）
                    Dim sqlStringTCeaOthercrs As String = executeInsertTCeaOthercrs(paramTCeaOthercrs)
                    execNonQuery(oraTran, sqlStringTCeaOthercrs)
                Next
            End If
            '実行
            '削除（手仕舞い連絡表 コース情報）
            Dim sqlStringDeteleTCeaCrsInfo As String = executeDeleteTCeaCrsInfo(paramDA)
            execNonQuery(oraTran, sqlStringDeteleTCeaCrsInfo)
            For Each paramTCeaCrsInfo As TCeaCrsInfoParam In listParamTCeaCrsInfo
                '登録（手仕舞い連絡表　コース情報）
                Dim sqlStringTCeaCrsInfo As String = executeInsertTCeaCrsInfo(paramTCeaCrsInfo)
                execNonQuery(oraTran, sqlStringTCeaCrsInfo)
            Next

            '実行
            '削除（手仕舞い連絡表 通信欄）
            Dim sqlStringDeteleTCeaReaders As String = executeDeleteTCeaReaders(paramDA)
            execNonQuery(oraTran, sqlStringDeteleTCeaReaders)
            For Each paramTCeaReaders As TCeaReadersParam In listParamTCeaReaders
                '登録（手仕舞い連絡表　通信欄）
                Dim sqlStringTCeaReaders As String = executeInsertTCeaReaders(paramTCeaReaders)
                execNonQuery(oraTran, sqlStringTCeaReaders)
            Next
            '実行
            '削除（手仕舞い連絡表 特記事項）
            Dim sqlStringDeteleTCeaNoteworthy As String = executeDeleteTCeaNoteworthy(paramDA)
            execNonQuery(oraTran, sqlStringDeteleTCeaNoteworthy)
            For Each paramTCeaNoteworthy As TCeaNoteworthyParam In listParamTCeaNoteworthy
                '登録（手仕舞い連絡表　特記事項）
                Dim sqlStringTCeaNoteworthy As String = executeInsertTCeaNoteworthy(paramTCeaNoteworthy)
                execNonQuery(oraTran, sqlStringTCeaNoteworthy)
            Next
            'コミット
            callCommitTransaction(oraTran)
            'リターンreturnValue
            Return 1
        Catch ex As OracleException
            '失敗時
            callRollbackTransaction(oraTran)
            'リターンreturnValue
            Return 0
        Catch ex As Exception
            '失敗時
            callRollbackTransaction(oraTran)
            'リターンreturnValue
            Return 0
        End Try

    End Function
#End Region

#Region "DELETE処理 "
    ''' <summary>
    ''' データ削除用 （手仕舞い連絡表）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTCeaControl(ByVal param As TCeaDAParam) As String
        Dim controlEtt As New TCeaControlEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_CEA_CONTROL ")
        'WHERE
        sb.AppendLine("WHERE ")
        'コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, controlEtt.CrsCd))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo))
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, controlEtt.Daily))
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ削除用 （手仕舞い連絡表 他コース）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTCeaOthercrs(ByVal param As TCeaDAParam) As String
        Dim othercrsEtt As New TCeaOthercrsEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_CEA_OTHERCRS ")
        'WHERE
        sb.AppendLine("WHERE ")
        'コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, othercrsEtt.CrsCd))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo))
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, othercrsEtt.Daily))
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ削除用 （手仕舞い連絡表 コース情報）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTCeaCrsInfo(ByVal param As TCeaDAParam) As String
        Dim crsInfoEtt As New TCeaCrsInfoEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_CEA_CRS_INFO ")
        'WHERE
        sb.AppendLine("WHERE ")
        'コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, crsInfoEtt.CrsCd))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo))
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, crsInfoEtt.Daily))
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ削除用 （手仕舞い連絡表 通信欄）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTCeaReaders(ByVal param As TCeaDAParam) As String
        Dim readersEtt As New TCeaReadersEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_CEA_READERS ")
        'WHERE
        sb.AppendLine("WHERE")
        'コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, readersEtt.CrsCd))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo))
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, readersEtt.Daily))
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ削除用 （手仕舞い連絡表 特記事項）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteTCeaNoteworthy(ByVal param As TCeaDAParam) As String
        Dim noteworthy As New TCeaNoteworthyEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        sb.AppendLine("DELETE FROM T_CEA_NOTEWORTHY ")
        'WHERE
        sb.AppendLine("WHERE")
        'コースコード
        sb.AppendLine("  CRS_CD = ").Append(setDataParam(param.CrsCd, noteworthy.CrsCd))
        '仕入先コード
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_CD = ").Append(setDataParam(param.SiireSakiCd, noteworthy.SiireSakiCd))
        '仕入先枝番
        sb.AppendLine("  AND ")
        sb.AppendLine("  SIIRE_SAKI_NO = ").Append(setDataParam(param.SiireSakiNo, noteworthy.SiireSakiNo))
        '日次
        sb.AppendLine("  AND ")
        sb.AppendLine("  DAILY = ").Append(setDataParam(param.Daily, noteworthy.Daily))
        Return sb.ToString
    End Function
#End Region

#Region "INSERT処理 "
    ''' <summary>
    ''' データ登録用 （手仕舞い連絡表）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertTCeaControl(ByVal param As TCeaControlParam) As String
        Dim controlEtt As New TCeaControlEntity
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_CEA_CONTROL")
        sb.AppendLine("            (CRS_CD")                                                  'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , SIIRE_SAKI_KIND_CD")                                            '仕入先種別
        sb.AppendLine("     , SIIRE_SAKI_NAME")                                               '仕入先名
        sb.AppendLine("     , TEL1 ")                                                         'TEL1
        sb.AppendLine("     , TEL1_1 ")                                                       'TEL1_1
        sb.AppendLine("     , TEL1_2 ")                                                       'TEL1_2
        sb.AppendLine("     , TEL1_3 ")                                                       'TEL1_3
        sb.AppendLine("     , FAX1 ")                                                         'FAX1
        sb.AppendLine("     , FAX1_1 ")                                                       'FAX1_1
        sb.AppendLine("     , FAX1_2 ")                                                       'FAX1_2
        sb.AppendLine("     , FAX1_3 ")                                                       'FAX1_3
        sb.AppendLine("     , FAX2 ")                                                         'FAX2
        sb.AppendLine("     , FAX2_1 ")                                                       'FAX2_1
        sb.AppendLine("     , FAX2_2 ")                                                       'FAX2_2
        sb.AppendLine("     , FAX2_3 ")                                                       'FAX2_3
        sb.AppendLine("     , MAIL ")                                                         'メールアドレス
        sb.AppendLine("     , SEND_KIND ")                                                    '通知方法
        sb.AppendLine("     , SEND_YMDT ")                                                    '最終送信日時
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY)")                                            'システム更新日
        'INSERT 登録
        sb.AppendLine("     VALUES")
        sb.AppendLine("            (" & setDataParam(param.CrsCd, controlEtt.CrsCd))
        sb.AppendLine("            ," & setDataParam(param.Daily, controlEtt.Daily))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiCd, controlEtt.SiireSakiCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiNo, controlEtt.SiireSakiNo))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiKindCd, controlEtt.SiireSakiKindCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiName, controlEtt.SiireSakiName))
        sb.AppendLine("            ," & setDataParam(param.Tel1, controlEtt.Tel1))
        sb.AppendLine("            ," & setDataParam(param.Tel11, controlEtt.Tel11))
        sb.AppendLine("            ," & setDataParam(param.Tel12, controlEtt.Tel12))
        sb.AppendLine("            ," & setDataParam(param.Tel13, controlEtt.Tel13))
        sb.AppendLine("            ," & setDataParam(param.Fax1, controlEtt.Fax1))
        sb.AppendLine("            ," & setDataParam(param.Fax11, controlEtt.Fax11))
        sb.AppendLine("            ," & setDataParam(param.Fax12, controlEtt.Fax12))
        sb.AppendLine("            ," & setDataParam(param.Fax13, controlEtt.Fax13))
        sb.AppendLine("            ," & setDataParam(param.Fax2, controlEtt.Fax2))
        sb.AppendLine("            ," & setDataParam(param.Fax21, controlEtt.Fax21))
        sb.AppendLine("            ," & setDataParam(param.Fax22, controlEtt.Fax22))
        sb.AppendLine("            ," & setDataParam(param.Fax23, controlEtt.Fax23))
        sb.AppendLine("            ," & setDataParam(param.Mail, controlEtt.Mail))
        sb.AppendLine("            ," & setDataParam(param.SendKind, controlEtt.SendKind))
        sb.AppendLine("            ," & setDataParam(param.SendYmdt, controlEtt.SendYmdt))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPgmid, controlEtt.SystemEntryPgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPersonCd, controlEtt.SystemEntryPersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryDay, controlEtt.SystemEntryDay))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePgmid, controlEtt.SystemUpdatePgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePersonCd, controlEtt.SystemUpdatePersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdateDay, controlEtt.SystemUpdateDay))
        sb.AppendLine("            )")
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ登録用 （手仕舞い連絡表　他コース）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeInsertTCeaOthercrs(ByVal param As TCeaOthercrsParam) As String
        Dim othercrsEtt As New TCeaOthercrsEntity
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_CEA_OTHERCRS")
        sb.AppendLine("            (CRS_CD")                                                  'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , OTHER_CRS_CD")                                                  '他コースコード
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY) ")                                            'システム更新日
        'INSERT 登録
        sb.AppendLine("     VALUES")
        sb.AppendLine("            (" & setDataParam(param.CrsCd, othercrsEtt.CrsCd))
        sb.AppendLine("            ," & setDataParam(param.Daily, othercrsEtt.Daily))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiCd, othercrsEtt.SiireSakiCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiNo, othercrsEtt.SiireSakiNo))
        sb.AppendLine("            ," & setDataParam(param.OtherCrsCd, othercrsEtt.OtherCrsCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPgmid, othercrsEtt.SystemEntryPgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPersonCd, othercrsEtt.SystemEntryPersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryDay, othercrsEtt.SystemEntryDay))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePgmid, othercrsEtt.SystemUpdatePgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePersonCd, othercrsEtt.SystemUpdatePersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdateDay, othercrsEtt.SystemUpdateDay)) '更新日DATE
        sb.AppendLine("            )")
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ登録用 （手仕舞い連絡表　コース情報）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertTCeaCrsInfo(ByVal param As TCeaCrsInfoParam) As String
        Dim crsInfoEtt As New TCeaCrsInfoEntity
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_CEA_CRS_INFO")
        sb.AppendLine("            (CRS_CD")                                                  'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , CRS_INFO_NO")                                                   'コース情報№
        sb.AppendLine("     , CRS_INFO")                                                      'コース情報
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )")                                            'システム更新日
        'INSERT 登録
        sb.AppendLine("     VALUES")
        sb.AppendLine("            (" & setDataParam(param.CrsCd, crsInfoEtt.CrsCd))
        sb.AppendLine("            ," & setDataParam(param.Daily, crsInfoEtt.Daily))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiCd, crsInfoEtt.SiireSakiCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiNo, crsInfoEtt.SiireSakiNo))
        sb.AppendLine("            ," & setDataParam(param.CrsInfoNo, crsInfoEtt.CrsInfoNo))
        sb.AppendLine("            ," & setDataParam(param.CrsInfo, crsInfoEtt.CrsInfo))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPgmid, crsInfoEtt.SystemEntryPgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPersonCd, crsInfoEtt.SystemEntryPersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryDay, crsInfoEtt.SystemEntryDay))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePgmid, crsInfoEtt.SystemUpdatePgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePersonCd, crsInfoEtt.SystemUpdatePersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdateDay, crsInfoEtt.SystemUpdateDay)) '更新日DATE
        sb.AppendLine("            )")
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ登録用 （手仕舞い連絡表　通信欄）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertTCeaReaders(ByVal param As TCeaReadersParam) As String
        Dim readersEtt As New TCeaReadersEntity
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_CEA_READERS")
        sb.AppendLine("            (CRS_CD")                                                  'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , READERS_COL_NO")                                                '通信欄№
        sb.AppendLine("     , READERS_COL")                                                   '通信欄
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )")                                           'システム更新日
        'INSERT 登録
        sb.AppendLine("     VALUES")
        sb.AppendLine("            (" & setDataParam(param.CrsCd, readersEtt.CrsCd))
        sb.AppendLine("            ," & setDataParam(param.Daily, readersEtt.Daily))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiCd, readersEtt.SiireSakiCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiNo, readersEtt.SiireSakiNo))
        sb.AppendLine("            ," & setDataParam(param.ReadersColNo, readersEtt.ReadersColNo))
        sb.AppendLine("            ," & setDataParam(param.ReadersCol, readersEtt.ReadersCol))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPgmid, readersEtt.SystemEntryPgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPersonCd, readersEtt.SystemEntryPersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryDay, readersEtt.SystemEntryDay))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePgmid, readersEtt.SystemUpdatePgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePersonCd, readersEtt.SystemUpdatePersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdateDay, readersEtt.SystemUpdateDay)) '更新日DATE
        sb.AppendLine("            )")
        Return sb.ToString
    End Function

    ''' <summary>
    ''' データ登録用 （手仕舞い連絡表　特記事項）
    ''' </summary>
    ''' <param name="param">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertTCeaNoteworthy(ByVal param As TCeaNoteworthyParam) As String
        Dim noteworthyEtt As New TCeaNoteworthyEntity
        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO T_CEA_NOTEWORTHY")
        sb.AppendLine("            (CRS_CD")                                                  'コースコード
        sb.AppendLine("     , DAILY ")                                                        '日次
        sb.AppendLine("     , SIIRE_SAKI_CD")                                                 '仕入先コード
        sb.AppendLine("     , SIIRE_SAKI_NO")                                                 '仕入先枝番
        sb.AppendLine("     , NOTEWORTHY_NO")                                                 '特記事項№
        sb.AppendLine("     , NOTEWORTHY")                                                    '特記事項
        sb.AppendLine("     , SYSTEM_ENTRY_PGMID ")                                           'システム登録ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_ENTRY_PERSON_CD ")                                       'システム登録者コード
        sb.AppendLine("     , SYSTEM_ENTRY_DAY ")                                             'システム登録日
        sb.AppendLine("     , SYSTEM_UPDATE_PGMID ")                                          'システム更新ＰＧＭＩＤ
        sb.AppendLine("     , SYSTEM_UPDATE_PERSON_CD ")                                      'システム更新者コード
        sb.AppendLine("     , SYSTEM_UPDATE_DAY )")                                            'システム更新日
        'INSERT 登録
        sb.AppendLine("     VALUES")
        sb.AppendLine("            (" & setDataParam(param.CrsCd, noteworthyEtt.CrsCd))
        sb.AppendLine("            ," & setDataParam(param.Daily, noteworthyEtt.Daily))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiCd, noteworthyEtt.SiireSakiCd))
        sb.AppendLine("            ," & setDataParam(param.SiireSakiNo, noteworthyEtt.SiireSakiNo))
        sb.AppendLine("            ," & setDataParam(param.NoteworthyNo, noteworthyEtt.NoteworthyNo))
        sb.AppendLine("            ," & setDataParam(param.Noteworthy, noteworthyEtt.Noteworthy))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPgmid, noteworthyEtt.SystemEntryPgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryPersonCd, noteworthyEtt.SystemEntryPersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemEntryDay, noteworthyEtt.SystemEntryDay))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePgmid, noteworthyEtt.SystemUpdatePgmid))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdatePersonCd, noteworthyEtt.SystemUpdatePersonCd))
        sb.AppendLine("            ," & setDataParam(param.SystemUpdateDay, noteworthyEtt.SystemUpdateDay)) '更新日DATE
        sb.AppendLine("            )")
        Return sb.ToString
    End Function

    Public Function setDataParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function

    Private Function setParamEx(ByVal value As Object, ByVal ent As IEntityKoumokuType, ByVal selFlg As Boolean) As String
        ParamNum += 1
        If selFlg = True AndAlso TypeOf ent Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType)
        Else
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function
    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
#End Region

#Region " パラメータ "

    Public Class TCeaDAParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
    End Class

    Public Class TCeaControlParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 仕入先種別
        ''' </summary>
        Public Property SiireSakiKindCd As String
        ''' <summary>
        ''' 仕入先名
        ''' </summary>
        Public Property SiireSakiName As String
        ''' <summary>
        ''' TEL1
        ''' </summary>
        Public Property Tel1 As String
        ''' <summary>
        ''' TEL1_1
        ''' </summary>
        Public Property Tel11 As String
        ''' <summary>
        ''' TEL1_2
        ''' </summary>
        Public Property Tel12 As String
        ''' <summary>
        ''' TEL1_3
        ''' </summary>
        Public Property Tel13 As String
        ''' <summary>
        ''' FAX1
        ''' </summary>
        Public Property Fax1 As String
        ''' <summary>
        ''' FAX1_1
        ''' </summary>
        Public Property Fax11 As String
        ''' <summary>
        ''' FAX1_2
        ''' </summary>
        Public Property Fax12 As String
        ''' <summary>
        ''' FAX1_3
        ''' </summary>
        Public Property Fax13 As String
        ''' <summary>
        ''' FAX2
        ''' </summary>
        Public Property Fax2 As String
        ''' <summary>
        ''' FAX2_1
        ''' </summary>
        Public Property Fax21 As String
        ''' <summary>
        ''' FAX2_2
        ''' </summary>
        Public Property Fax22 As String
        ''' <summary>
        ''' FAX2_3
        ''' </summary>
        Public Property Fax23 As String
        ''' <summary>
        ''' メールアドレス
        ''' </summary>
        Public Property Mail As String
        ''' <summary>
        ''' 通知方法
        ''' </summary>
        Public Property SendKind As String
        ''' <summary>
        ''' 最終送信日時
        ''' </summary>
        Public Property SendYmdt As Date?
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class

    Public Class TCeaOthercrsParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 他コースコード
        ''' </summary>
        Public Property OtherCrsCd As String
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class

    Public Class TCeaCrsInfoParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' コース情報№
        ''' </summary>
        Public Property CrsInfoNo As Integer
        ''' <summary>
        ''' コース情報
        ''' </summary>
        Public Property CrsInfo As String
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class

    Public Class TCeaReadersParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 通信欄№
        ''' </summary>
        Public Property ReadersColNo As Integer
        ''' <summary>
        ''' 通信欄
        ''' </summary>
        Public Property ReadersCol As String
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class

    Public Class TCeaNoteworthyParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日次
        ''' </summary>
        Public Property Daily As Integer
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' 仕入先枝番
        ''' </summary>
        Public Property SiireSakiNo As String
        ''' <summary>
        ''' 特記事項№
        ''' </summary>
        Public Property NoteworthyNo As Integer
        ''' <summary>
        ''' 特記事項
        ''' </summary>
        Public Property Noteworthy As String
        ''' <summary>
        ''' システム登録ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemEntryPgmid As String
        ''' <summary>
        ''' システム登録者コード
        ''' </summary>
        Public Property SystemEntryPersonCd As String
        ''' <summary>
        ''' システム登録日
        ''' </summary>
        Public Property SystemEntryDay As Date
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class

    Public Class TCeaLedgerBasicParam
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String

        ''' <summary>
        ''' 出発日
        ''' </summary>
        Public Property SyuptDay As String
    End Class
#End Region

End Class