Imports System.Text



''' <summary>
''' コース台帳一括修正（車種・台数カウント）のDAクラス
''' </summary>
Public Class CarType_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getCarType                          '一覧結果取得検索
        executeCountUpdateCarType           '登録
        executeUpdateCarType                '更新
        executeReturnCarType                '戻し
        getZasekiImage                      '座席イメージ（バス情報）取得
        getZasekiImageInfo                  '座席イメージ（座席情報）取得
        getLegBasic                         'コース台帳（基本）取得
        getBusReserveCrsLedger              '同一バス指定コードデータ取得
        getCarTypeMst                       '車種マスタ取得
    End Enum

    ''' <summary>
    ''' パラメータキー
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class ParamHashKeys
        ''' <summary>
        ''' バス指定(KEY)
        ''' </summary>
        Public Const BASIC_KEYS As String = "BASIC_KEYS"
        ''' <summary>
        ''' コースコード(KEY)
        ''' </summary>
        Public Const CRS_CD As String = "CRS_CD"

    End Class

    Private comTehai As New TehaiCommon
#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessCarTypeTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getCarType
                '一覧結果取得検索
                sqlString = getCarType(paramInfoList)
            Case accessType.getZasekiImage
                sqlString = getZasekiImage(paramInfoList)
            Case accessType.getZasekiImageInfo
                sqlString = getZasekiImageInfo(paramInfoList)
            Case accessType.getLegBasic
                sqlString = getLegBasic(paramInfoList)
            Case accessType.getBusReserveCrsLedger
                sqlString = getBusReserveCrsLedger(paramInfoList)
            Case accessType.getCarTypeMst
                '一覧結果取得検索
                sqlString = getMstCarType()
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
    Protected Overloads Function getCarType(ByVal paramList As Hashtable) As String

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
        sqlString.AppendLine(",NVL(BASIC.BUS_RESERVE_CD,'') AS BUS_RESERVE_CD ")            'バス指定コード
        sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,'0') AS JYOSYA_CAPACITY ")         '乗車定員
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,'0') AS YOYAKU_NUM_TEISEKI ")   '予約数定席
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,'0') AS YOYAKU_NUM_SUB_SEAT ") '予約数補助席
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_REGULAR,'0') AS EI_BLOCK_REGULAR ")       '営ブロック定
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_HO,'0') AS EI_BLOCK_HO ")                 '営ブロック補
        sqlString.AppendLine(",NVL(BASIC.KUSEKI_KAKUHO_NUM,'0') AS KUSEKI_KAKUHO_NUM ")     '空席確保数
        sqlString.AppendLine(",NVL(BASIC.BLOCK_KAKUHO_NUM,'0') AS BLOCK_KAKUHO_NUM ")       'ブロック確保数
        sqlString.AppendLine(",NVL(BASIC.SUB_SEAT_OK_KBN,'') AS SUB_SEAT_OK_KBN ")          '補助席可区分
        sqlString.AppendLine(",NVL(BASIC.CAR_NO,'') AS CAR_NO ")                            '車番
        sqlString.AppendLine(",NVL(BASIC.CAR_TYPE_CD,'') AS CAR_TYPE_CD ")                  '車種コード
        sqlString.AppendLine(",NVL(BASIC.BUS_COUNT_FLG,'0') AS BUS_COUNT_FLG ")             '台数カウントフラグ
        sqlString.AppendLine(",NVL(ZASEKIIMAGE.KUSEKI_KAKUHO_NUM,0) AS ZASEKI_KUSEKI_KAKUHO_NUM ")  '空席確保数
        sqlString.AppendLine(",NVL(ZASEKIIMAGE.BLOCK_KAKUHO_NUM,0) AS ZASEKI_BLOCK_KAKUHO_NUM ")    'ブロック確保数
        sqlString.AppendLine(",NVL(ZASEKIIMAGE.SUB_SEAT_OK_KBN,'') AS ZASEKI_SUB_SEAT_OK_KBN ")     '補助席可区分
        sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_REGULAR ")                          '配列状態定
        sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_HO ")                               '配列状態補
        sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_1KAI ")                             '配列状態1F
        sqlString.AppendLine(",NVL(CARKIND.CAR_CAPACITY,'') AS CAR_CAPACITY ")              '定員（定）
        sqlString.AppendLine(",NVL(CARKIND.CAR_EMG_CAPACITY,'') AS CAR_EMG_CAPACITY ")      '定員（補・１階）
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                                   'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")                             'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")                                 'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",NVL(BASIC.CAPACITY_REGULAR,'') AS CAPACITY_REGULAR ")        '定員定
        sqlString.AppendLine(",NVL(BASIC.CAPACITY_HO_1KAI,'') AS CAPACITY_HO_1KAI ")        '定員補１階
        sqlString.AppendLine(",NVL(BASIC.CRS_KIND,'') AS CRS_KIND ")                        'コース種別
        sqlString.AppendLine(",NVL(BASIC.TEIINSEI_FLG,'') AS TEIINSEI_FLG ")                '定員制フラグ
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_CAPACITY,'') AS CRS_BLOCK_CAPACITY ")    'コースブロック定員
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")                      '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                                    '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                                         '更新区分
        sqlString.AppendLine(",BASIC.UNKYU_KBN AS UNKYU ")                                  '運休区分（判定用）
        sqlString.AppendLine(",BASIC.SAIKOU_KAKUTEI_KBN AS SAIKOU ")                        '催行確定区分（判定用）
        sqlString.AppendLine(",' ' AS CAR_TYPE_HENKOU_KBN ")                                '車種変更区分
        sqlString.AppendLine(",0 AS TEISEKIREGULAR ")                                       '空席再計算_定席数（定）
        sqlString.AppendLine(",0 AS TEISEKIHO ")                                            '空席再計算_定席数（補）
        sqlString.AppendLine(",0 AS KUSEKIREGULAR ")                                        '空席再計算_空席数（定）
        sqlString.AppendLine(",0 AS KUSEKIHO ")                                             '空席再計算_空席数（補）
        sqlString.AppendLine(",0 AS TEISEKIREGULAR_CARKIND ")                               '車種変更_定席数（定）
        sqlString.AppendLine(",0 AS TEISEKIHO_CARKIND ")                                    '車種変更_定席数（補）
        sqlString.AppendLine(",0 AS KUSEKIREGULAR_CARKIND ")                                '車種変更_空席数（定）
        sqlString.AppendLine(",0 AS KUSEKIHO_CARKIND ")                                     '車種変更_空席数（補）
        sqlString.AppendLine(",LPAD(' ', 256) AS MESSAGE ")                                 'メッセージ
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & FixedCd.CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & FixedCd.CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & FixedCd.CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("LEFT JOIN T_ZASEKI_IMAGE ZASEKIIMAGE ON BASIC.BUS_RESERVE_CD = ZASEKIIMAGE.BUS_RESERVE_CD AND BASIC.SYUPT_DAY = ZASEKIIMAGE.SYUPT_DAY AND BASIC.GOUSYA = ZASEKIIMAGE.GOUSYA ")
        sqlString.AppendLine("LEFT JOIN M_CAR_KIND CARKIND ON BASIC.CAR_TYPE_CD = CARKIND.CAR_CD ")
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

    ''' <summary>
    ''' 座席イメージ(バス情報)取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Public Function getZasekiImage(paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" SUB_SEAT_OK_KBN ")       ' ブロック確保数
        sqlString.AppendLine(" , TEIKI_KIKAKU_KBN ")    ' ブロック確保数
        sqlString.AppendLine(" , BLOCK_KAKUHO_NUM ")    ' ブロック確保数
        sqlString.AppendLine(" , EI_BLOCK_REGULAR ")    ' 営ブロック定
        sqlString.AppendLine(" , EI_BLOCK_HO ")         ' 営ブロック補
        sqlString.AppendLine(" , KUSEKI_KAKUHO_NUM ")   ' 空席確保数
        sqlString.AppendLine(" , CAPACITY_REGULAR ")    ' 定員定
        sqlString.AppendLine(" , CAPACITY_HO_1KAI")     ' 定員補1F
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("   T_ZASEKI_IMAGE ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND NVL(DELETE_DAY, 0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 座席イメージ(座席情報)取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Protected Overloads Function getZasekiImageInfo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  INFO.ZASEKI_KAI ")              ' 階
        sqlString.AppendLine("  , INFO.ZASEKI_LINE ")           ' 行
        sqlString.AppendLine("  , INFO.ZASEKI_COL ")            ' 列
        sqlString.AppendLine("  , INFO.ZASEKI_KIND ")           ' 座席種別
        sqlString.AppendLine("  , INFO.ZASEKI_KBN ")            ' 座席区分
        sqlString.AppendLine("  , INFO.BLOCK_KBN ")             ' ブロック区分
        sqlString.AppendLine("  , INFO.EIGYOSYO_BLOCK_KBN ")    ' 営業所ブロック区分
        sqlString.AppendLine("  , INFO.EIGYOSYO_CD ")           ' 営業所コード
        sqlString.AppendLine("  , INFO.YOYAKU_FLG ")            ' 予約フラグ
        sqlString.AppendLine("  , INFO.LADIES_SEAT_FLG ")       ' 女性席フラグ
        sqlString.AppendLine("  , INFO.ZASEKI_STATE")           ' 座席状態
        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_ZASEKI_IMAGE IMAGE ")
        sqlString.AppendLine("  INNER JOIN T_ZASEKI_IMAGE_INFO INFO ")
        sqlString.AppendLine("    ON IMAGE.BUS_RESERVE_CD = INFO.BUS_RESERVE_CD ")
        sqlString.AppendLine("    AND IMAGE.SYUPT_DAY = INFO.SYUPT_DAY ")
        sqlString.AppendLine("    AND IMAGE.GOUSYA = INFO.GOUSYA ")
        sqlString.AppendLine("    AND INFO.ZASEKI_KIND <> '" & FixedCd.ZasekiKind.dummySeat & "' ")
        sqlString.AppendLine("    AND INFO.ZASEKI_KBN = '" & FixedCd.ZasekiKbnType.zaseki & "' ")
        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine(" IMAGE.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND IMAGE.BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND IMAGE.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' コース台帳（基本）取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Protected Overloads Function getLegBasic(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  SYUPT_DAY ")                ' 出発日
        sqlString.AppendLine("  , CRS_CD ")                 ' コースコード
        sqlString.AppendLine("  , GOUSYA ")                 ' 号車
        sqlString.AppendLine("  , BUS_RESERVE_CD ")         ' バス指定コード
        sqlString.AppendLine("  , CAR_TYPE_CD ")            ' 車種コード
        sqlString.AppendLine("  , ZASEKI_RESERVE_KBN ")     ' 座席指定区分
        sqlString.AppendLine("  , CAPACITY_REGULAR ")       ' 定員定
        sqlString.AppendLine("  , CAPACITY_HO_1KAI ")       ' 定員補１階
        sqlString.AppendLine("  , KUSEKI_NUM_TEISEKI ")     ' 空席数定席
        sqlString.AppendLine("  , KUSEKI_KAKUHO_NUM ")      ' 空席数補助席
        sqlString.AppendLine("  , EI_BLOCK_REGULAR")        ' 営ブロック定
        sqlString.AppendLine("  , EI_BLOCK_HO")             ' 営ブロック補
        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC ")
        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 同一バス指定データ取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Protected Overloads Function getBusReserveCrsLedger(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")                                                       '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                                          'コースコード
        sqlString.AppendLine(",BASIC.GOUSYA ")                                                          '号車
        sqlString.AppendLine(",NVL(BASIC.BUS_RESERVE_CD,'') AS BUS_RESERVE_CD ")                        'バス指定コード
        sqlString.AppendLine(",BASIC.USING_FLG")                                                        '使用中フラグ
        sqlString.AppendLine(",'' AS HENKOU_KAHI_KBN ")                                                 '変更可否区分
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY AS SYSTEM_UPDATE_DAY ")                          'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD AS SYSTEM_UPDATE_PERSON_CD ")              'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID AS SYSTEM_UPDATE_PGMID ")                      'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,'0') AS JYOSYA_CAPACITY ")                     '乗車定員
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,'0') AS YOYAKU_NUM_TEISEKI ")               '予約数定席
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,'0') AS YOYAKU_NUM_SUB_SEAT ")             '予約数補助席

        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_CRS_LEDGER_BASIC BASIC ")

        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine(" (BASIC.SYUPT_DAY, BASIC.GOUSYA, BASIC.BUS_RESERVE_CD) IN (")
        sqlString.AppendLine(" ").Append(paramList(ParamHashKeys.BASIC_KEYS))
        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" AND TRIM(BASIC.CRS_CD) <> " & setParam("CRS_CD", paramList(ParamHashKeys.CRS_CD), OracleDbType.Char))

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 車種マスタデータ取得
    ''' </summary>
    Protected Overloads Function getMstCarType() As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine(" TYPE.CAR_CD ")                                                       '車種コード
        sqlString.AppendLine(",TYPE.CAR_CAPACITY ")                                                 '定員（定）
        sqlString.AppendLine(",TYPE.CAR_EMG_CAPACITY ")                                             '定員（補・1Ｆ）

        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  M_CAR_KIND TYPE ")

        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine(" NVL(DELETE_DATE,0) = 0 ")

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
    Public Function executeCarTypeTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable, ByVal CarTypeChanged As Boolean, ByVal busReserveFlg As Boolean) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                'コース台帳（基本）車種関連の更新
                Case accessType.executeUpdateCarType
                    For i As Integer = 1 To 2
                        If i = 1 Then
                            sqlString = executeUpdateBasicData(paramInfoList, CarTypeChanged)
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                        Else
                            If busReserveFlg = True Then
                                sqlString = executeUpdateBasicReserveData(paramInfoList, CarTypeChanged)
                                returnValue += execNonQuery(oracleTransaction, sqlString)
                            End If
                        End If
                    Next

                'コース台帳（基本）台数カウントフラグの更新
                Case accessType.executeCountUpdateCarType
                    sqlString = executeUpdateCountData(paramInfoList)
                '戻し処理（乗せ合わせ対応）
                Case accessType.executeReturnCarType
                    sqlString = executeReturnBasicData(paramInfoList)
            End Select

            If type <> accessType.executeUpdateCarType Then
                returnValue += execNonQuery(oracleTransaction, sqlString)
            End If

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

    ''' <summary>
    ''' 使用中フラグ更新
    ''' </summary>
    ''' <param name="selectOldData"></param>
    ''' <param name="systemupdatepgmid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUsingFlgCrs(ByVal selectOldData As DataTable, ByVal systemupdatepgmid As String) As DataTable

        Dim totalValue As New DataTable
        Dim trn As OracleTransaction = Nothing
        Dim returnValue As Boolean = False

        Try

            totalValue.Columns.Add("USING_FLG")             '使用中フラグ

            'トランザクション開始
            trn = callBeginTransaction()

            For Each row As DataRow In selectOldData.Rows
                Dim syuptsday As String = Replace(CType(row("SYUPT_DAY"), String), "/", "") '出発日
                Dim crscd As String = CType(row("CRS_CD"), String)                          'コースコード
                Dim gousya As String = CType(row("GOUSYA"), String)                         '号車

                returnValue = CommonCheckUtil.setUsingFlg_Crs(syuptsday, crscd, gousya, systemupdatepgmid, trn, True)

                Dim row2 As DataRow = totalValue.NewRow
                If returnValue = True Then
                    row2("USING_FLG") = FixedCd.UsingFlg.Use
                Else
                    row2("USING_FLG") = String.Empty
                End If

                totalValue.Rows.Add(row2)
            Next

            'コミット
            Call callCommitTransaction(trn)

        Catch ex As Exception
            'ロールバック
            Call callRollbackTransaction(trn)
            Throw

        Finally
            Call trn.Dispose()
        End Try

        Return totalValue

    End Function

    ''' <summary>
    ''' コース台帳（基本）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBasicData(ByVal paramList As Hashtable, ByVal CarTypeChanged As Boolean) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        If CarTypeChanged = True Then
            sqlString.AppendLine(",CAR_TYPE_CD = " & setParam("CAR_TYPE_CD", paramList.Item("CAR_TYPE_CD"), OracleDbType.Char))
            sqlString.AppendLine(",KYOSAI_UNKOU_KBN = " & setParam("KYOSAI_UNKOU_KBN", paramList.Item("KYOSAI_UNKOU_KBN"), OracleDbType.Char))
            sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM", paramList.Item("YOYAKU_KANOU_NUM"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT", paramList.Item("KUSEKI_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",CAPACITY_REGULAR = " & setParam("CAPACITY_REGULAR", paramList.Item("CAPACITY_REGULAR"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",CAPACITY_HO_1KAI = " & setParam("CAPACITY_HO_1KAI", paramList.Item("CAPACITY_HO_1KAI"), OracleDbType.Decimal, 3, 0))
        End If
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（基本）：台数カウントフラグ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateCountData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" BUS_COUNT_FLG = " & setParam("BUS_COUNT_FLG", paramList.Item("BUS_COUNT_FLG"), OracleDbType.Char))
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

    ''' <summary>
    ''' コース台帳（基本）：データ更新用（乗せ合わせコース）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBasicReserveData(ByVal paramList As Hashtable, ByVal CarTypeChanged As Boolean) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        If CarTypeChanged = True Then
            sqlString.AppendLine(",CAR_TYPE_CD = " & setParam("CAR_TYPE_CD", paramList.Item("CAR_TYPE_CD"), OracleDbType.Char))
            sqlString.AppendLine(",KYOSAI_UNKOU_KBN = " & setParam("KYOSAI_UNKOU_KBN", paramList.Item("KYOSAI_UNKOU_KBN"), OracleDbType.Char))
            sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM_2", paramList.Item("YOYAKU_KANOU_NUM_2"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI_2", paramList.Item("KUSEKI_NUM_TEISEKI_2"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT_2", paramList.Item("KUSEKI_NUM_SUB_SEAT_2"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",CAPACITY_REGULAR = " & setParam("CAPACITY_REGULAR", paramList.Item("CAPACITY_REGULAR"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",CAPACITY_HO_1KAI = " & setParam("CAPACITY_HO_1KAI", paramList.Item("CAPACITY_HO_1KAI"), OracleDbType.Decimal, 3, 0))
        End If
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD_2", paramList.Item("CRS_CD_2"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（基本）：データ戻し用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeReturnBasicData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", paramList.Item("SYSTEM_UPDATE_DAY"), OracleDbType.Date))
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

#End Region

#Region "マスタテーブル読み込み"

    ''' <summary>
    ''' 車種マスタデータ取得
    ''' </summary>
    ''' <param name="nullRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCarKindMaster(ByVal clsCarTypeGet_DA As CarType_DA, Optional ByVal nullRecord As Boolean = False, Optional ByVal addWhere As String = "") As DataTable

        Dim resultDataTable As New DataTable
        Dim strSQL As New StringBuilder

        Try
            '空レコード挿入要否に従い、空行挿入
            If nullRecord = True Then
                strSQL.AppendLine("SELECT ' ' AS CODE_VALUE,'' AS CODE_NAME FROM DUAL UNION ")
            End If
            strSQL.AppendLine("SELECT CAR_CD AS CODE_VALUE, CAR_NAME AS CODE_NAME FROM M_CAR_KIND")
            strSQL.AppendLine(" WHERE DELETE_DATE IS NULL")
            If Not addWhere.Equals(String.Empty) Then
                strSQL.AppendLine((" AND " & addWhere))
            End If
            strSQL.AppendLine(" ORDER BY CODE_VALUE")

            resultDataTable = MyBase.getDataTable(strSQL.ToString)
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 共催運行区分マスタより共催運行区分が存在するか確認する
    ''' </summary>
    ''' <param name="kyosaiunkoukbn"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetKyosaiUnkouKbn(ByVal kyosaiunkoukbn As String) As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        Dim KyosaiUnkou As String = String.Empty

        Try
            sqlString.AppendLine("SELECT KYOSAI_UNKOU_KBN FROM M_KYOSAI_UNKOU_KBN")
            sqlString.AppendLine(" WHERE")
            sqlString.AppendLine(" KYOSAI_UNKOU_KBN = " & setParam("KYOSAI_UNKOU_KBN", kyosaiunkoukbn, OracleDbType.Char))
            sqlString.AppendLine(" AND DELETE_DAY IS NULL")

            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        If resultDataTable.Rows.Count > 0 Then
            KyosaiUnkou = resultDataTable(0).Item("KYOSAI_UNKOU_KBN").ToString.Trim
        Else
            KyosaiUnkou = String.Empty
        End If
        Return KyosaiUnkou

    End Function

    '''' <summary>
    '''' 車種変更
    '''' </summary>
    '''' <param name="crsLegBasicDt"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function executeCarKindChange(ByVal crsLegBasicDt As DataTable, ByVal syoriKbn As Integer, ByVal haitiJun As String) As Integer
    '    Dim trn As OracleTransaction = Nothing
    '    Dim returnValue As Integer = 0

    '    Try
    '        'トランザクション開始
    '        trn = callBeginTransaction()

    '        '処理実行
    '        returnValue = comTehai.getChangeCarKind(crsLegBasicDt, syoriKbn, haitiJun, trn)

    '        '正常終了の場合
    '        If returnValue = 0 Then
    '            'コミット
    '            Call callCommitTransaction(trn)
    '        End If

    '    Catch ex As Exception
    '        'ロールバック
    '        Call callRollbackTransaction(trn)
    '        Throw

    '    Finally
    '        Call trn.Dispose()
    '    End Try

    '    Return returnValue

    'End Function
#End Region

End Class
