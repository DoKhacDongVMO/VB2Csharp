Imports System.Text


''' <summary>
''' コース台帳一括修正（バス指定・乗車定員・受付限定人数）のDAクラス
''' </summary>
Public Class BusReserveCd_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getBusReserveCd                         '一覧結果取得検索
        executeInsertBusReserveCd               '登録
        executeUpdateBusReserveCd               '更新（バス指定コード変更時）
        'executeReturnBusReserveCd               '戻し
        getCrsLedgerBasic2                      'コース台帳（基本）件数検索 //乗せ合わせチェック
        getCrsLedgerBasic3                      'コース台帳（基本）検索
        getCrsLedgerBasic4                      '座席イメージ（座席情報）検索
        getNoseawaseZaseki                      '座席イメージ（バス情報）検索
        'executeUpdateBusReserveCdZaseki        '更新(座席イメージ_使用フラグ:乗せ合わせ先)
        'executeUpdateBusReserveCdSaki           '更新(コース台帳（基本）:乗せ合わせ先)
        executeUpdateBusReserveCdSakiZaseki     '更新(座席イメージ（バス情報）:乗せ合わせ先)
        'getZasekiImage                         '座席イメージ（バス情報）取得
        getZasekiImageInfo                      '座席イメージ（座席情報）取得
        executeUpdateJosyaChange                '乗車定員更新
        'executeUpdateBusReserveCdMotoZasekiData '座席イメージ（バス情報）元データ更新
        executeUpdateZasekiInfo                 '座席イメージ（座席情報）
        executeUpdateZasekiMotoInfo             '座席イメージ（座席情報）元データ
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessBusReserveCdTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getBusReserveCd
                '一覧結果取得検索
                sqlString = getBusReserveCd(paramInfoList)
            Case accessType.getCrsLedgerBasic2
                sqlString = getCrsLedgerBasic2(paramInfoList)
            Case accessType.getCrsLedgerBasic3
                sqlString = getCrsLedgerBasic3(paramInfoList)
            Case accessType.getCrsLedgerBasic4
                sqlString = getCrsLedgerBasic4(paramInfoList)
            Case accessType.getNoseawaseZaseki
                sqlString = getNoseawaseZaseki(paramInfoList)
            'Case accessType.getZasekiImage
            '    sqlString = getZasekiImage(paramInfoList)
            Case accessType.getZasekiImageInfo
                sqlString = getZasekiImageInfo(paramInfoList)
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
    Protected Overloads Function getBusReserveCd(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")          '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                                      'コースコード
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")                                       '曜日コード
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 ")                           '乗車地コード
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") '出発時間
        sqlString.AppendLine(",BASIC.GOUSYA ")                                                      '号車
        sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN ")                                     '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")                            '催行確定区分
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ")                                            '定期・企画区分
        sqlString.AppendLine(",NVL(BASIC.BUS_RESERVE_CD,'') AS BUS_RESERVE_CD ")                    'バス指定コード
        sqlString.AppendLine(",NVL(BASIC.UKETUKE_GENTEI_NINZU,'0') AS UKETUKE_GENTEI_NINZU ")       '受付限定人数
        sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,'0') AS JYOSYA_CAPACITY ")                 '乗車定員
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,'0') AS YOYAKU_NUM_TEISEKI ")           '予約数定席
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,'0') AS YOYAKU_NUM_SUB_SEAT ")         '予約数補助席
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_REGULAR,'0') AS EI_BLOCK_REGULAR ")               '営ブロック定
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_HO,'0') AS EI_BLOCK_HO ")                         '営ブロック補
        sqlString.AppendLine(",NVL(BASIC.KUSEKI_KAKUHO_NUM,'0') AS KUSEKI_KAKUHO_NUM ")             '空席確保数
        sqlString.AppendLine(",NVL(BASIC.BLOCK_KAKUHO_NUM,'0') AS BLOCK_KAKUHO_NUM ")               'ブロック確保数
        sqlString.AppendLine(",NVL(BASIC.SUB_SEAT_OK_KBN,'') AS SUB_SEAT_OK_KBN ")                  '補助席可区分
        sqlString.AppendLine(",0 AS TOTAL_SEKI ")                                                   '合計席数
        sqlString.AppendLine(",0 AS TOTAL_SUB_SEAT ")                                               '合計補助席数
        sqlString.AppendLine(",NVL(BASIC.CRS_KIND,'') AS CRS_KIND ")                                'コース種別
        sqlString.AppendLine(",NVL(BASIC.TEIINSEI_FLG,'') AS TEIINSEI_FLG ")                        '定員制フラグ
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_CAPACITY,'0') AS CRS_BLOCK_CAPACITY ")           'コースブロック定員
        sqlString.AppendLine(",NVL(BASIC.CAR_TYPE_CD,'') AS CAR_TYPE_CD ")                          '車種コード
        sqlString.AppendLine(",NVL(ZASEKI.BLOCK_KAKUHO_NUM,0) AS Z_BLOCK_KAKUHO_NUM ")              'ブロック確保数
        sqlString.AppendLine(",NVL(ZASEKI.SUB_SEAT_OK_KBN,'') AS Z_SUB_SEAT_OK_KBN ")               '補助席可区分
        sqlString.AppendLine(",'' AS Z_ARRAY_STATE_REGULAR ")                                       '配列状態定
        sqlString.AppendLine(",'' AS Z_ARRAY_STATE_HO ")                                            '配列状態補
        sqlString.AppendLine(",'' AS Z_ARRAY_STATE_1KAI ")                                          '配列状態１Ｆ
        sqlString.AppendLine(",ZASEKI.SYSTEM_ENTRY_DAY AS Z_SYSTEM_ENTRY_DAY ")                     '座席イメージ_システム登録日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                                           'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")                                     'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")                                         'システム更 新ＰＧＭＩＤ
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")                              '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                                            '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                                                 '更新区分
        sqlString.AppendLine(",BASIC.YOYAKU_STOP_FLG ")                                             '予約停止フラグ
        sqlString.AppendLine(",BASIC.UNKYU_KBN AS W_UNKYU_KBN ")                                    '運休区分(コース台帳（基本）)
        sqlString.AppendLine(",BASIC.SAIKOU_KAKUTEI_KBN AS W_SAIKOU_KAKUTEI_KBN ")                  '催行確定区分(コース台帳（基本）)
        sqlString.AppendLine(",NVL(BASIC.BUS_RESERVE_CD,'') AS W_BUS_RESERVE_CD ")                  'バス指定コード(チェック用)
        sqlString.AppendLine(",' ' AS BUS_RESERVE_CD_KAHI_KBN ")                                    'バス指定可否区分
        sqlString.AppendLine(",' ' AS JYOSYA_CAPACITY_KAHI_KBN ")                                   '乗車定員可否区分
        sqlString.AppendLine(",0 AS TEISEKIREGULAR ")                                               '空席再計算_定席数（定）
        sqlString.AppendLine(",0 AS TEISEKIHO ")                                                    '空席再計算_定席数（補）
        sqlString.AppendLine(",0 AS KUSEKIREGULAR ")                                                '空席再計算_空席数（定）
        sqlString.AppendLine(",0 AS KUSEKIHO ")                                                     '空席再計算_空席数（補）
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & FixedCd.CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & FixedCd.CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & FixedCd.CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("LEFT JOIN T_ZASEKI_IMAGE ZASEKI ON BASIC.BUS_RESERVE_CD = ZASEKI.BUS_RESERVE_CD AND BASIC.SYUPT_DAY = ZASEKI.SYUPT_DAY AND BASIC.GOUSYA = ZASEKI.GOUSYA")
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
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getCrsLedgerBasic2(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" COUNT(SYUPT_DAY) AS COUNT")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getCrsLedgerBasic3(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" SYUPT_DAY ")                     '出発日
        sqlString.AppendLine(",CRS_CD ")                        'コースコード
        sqlString.AppendLine(",GOUSYA ")                        '号車
        sqlString.AppendLine(",BUS_RESERVE_CD ")                'バス指定コード
        sqlString.AppendLine(",CAR_TYPE_CD ")                   '車種コード
        sqlString.AppendLine(",JYOSYA_CAPACITY ")               '乗車定員
        sqlString.AppendLine(",UNKYU_KBN ")                     '運休区分
        sqlString.AppendLine(",SAIKOU_KAKUTEI_KBN ")            '催行確定区分
        sqlString.AppendLine(",USING_FLG ")                     '使用中フラグ
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ")             'システム更新日
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ")       'システム更新者コード
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ")           'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",YOYAKU_KANOU_NUM ")              '予約可能数
        sqlString.AppendLine(",YOYAKU_NUM_TEISEKI ")            '予約数定席
        sqlString.AppendLine(",YOYAKU_NUM_SUB_SEAT ")           '予約数補助席

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getCrsLedgerBasic4(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" COUNT(SYUPT_DAY) AS COUNT")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_ZASEKI_IMAGE_INFO ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" YOYAKU_STATUS IN ('" & FixedCd.YoyakuStatus.hakkenzumi & "','" & FixedCd.YoyakuStatus.Zasekishitei & "') ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getNoseawaseZaseki(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" SYUPT_DAY ")                 '出発日
        sqlString.AppendLine(",'' AS CRS_CD ")              'コースコード
        sqlString.AppendLine(",GOUSYA ")                    '号車
        sqlString.AppendLine(",BUS_RESERVE_CD ")            'バス指定コード
        sqlString.AppendLine(",BLOCK_KAKUHO_NUM ")          'ブロック確保数
        sqlString.AppendLine(",EI_BLOCK_HO ")               '営ブロック補
        sqlString.AppendLine(",EI_BLOCK_REGULAR ")          '営ブロック定
        sqlString.AppendLine(",KUSEKI_KAKUHO_NUM ")         '空席確保数
        sqlString.AppendLine(",KUSEKI_NUM_TEISEKI ")        '空席数定席
        sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT ")       '空席数補助席
        sqlString.AppendLine(",SUB_SEAT_OK_KBN ")           '補助席可区分
        sqlString.AppendLine(",CAR_TYPE_CD ")               '車種コード
        sqlString.AppendLine(",TEIKI_KIKAKU_KBN ")          '定期企画区分
        sqlString.AppendLine(",CAPACITY_REGULAR ")          '定員定
        sqlString.AppendLine(",CAPACITY_HO_1KAI")           '定員補1F
        sqlString.AppendLine(",USING_FLG ")                 '使用中フラグ
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ")   '更新者コード
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ")       '更新ＰＧＭＩＤ
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ")         '更新日
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_ZASEKI_IMAGE ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    '''' <summary>
    '''' 座席イメージ(バス情報)取得
    '''' </summary>
    '''' <param name="paramList"></param>
    'Public Function getZasekiImage(paramList As Hashtable) As String

    '    Dim sqlString As New StringBuilder
    '    paramClear()

    '    'SELECT句
    '    sqlString.AppendLine(" SELECT")
    '    sqlString.AppendLine(" SUB_SEAT_OK_KBN ")       ' 補助席可区分
    '    sqlString.AppendLine(" , TEIKI_KIKAKU_KBN ")    ' 定期企画区分
    '    sqlString.AppendLine(" , BLOCK_KAKUHO_NUM ")    ' ブロック確保数
    '    sqlString.AppendLine(" , EI_BLOCK_REGULAR ")    ' 営ブロック定
    '    sqlString.AppendLine(" , EI_BLOCK_HO ")         ' 営ブロック補
    '    sqlString.AppendLine(" , KUSEKI_KAKUHO_NUM ")   ' 空席確保数
    '    sqlString.AppendLine(" , CAPACITY_REGULAR ")    ' 定員定
    '    sqlString.AppendLine(" , CAPACITY_HO_1KAI")     ' 定員補1F
    '    'FROM句
    '    sqlString.AppendLine(" FROM ")
    '    sqlString.AppendLine("   T_ZASEKI_IMAGE ")
    '    'WHERE句
    '    sqlString.AppendLine(" WHERE ")
    '    sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
    '    sqlString.AppendLine(" AND BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
    '    sqlString.AppendLine(" AND GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
    '    sqlString.AppendLine(" AND NVL(DELETE_DAY, 0) = 0 ")

    '    Return sqlString.ToString
    'End Function

    ''' <summary>
    ''' 座席イメージ(座席情報)取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Protected Overloads Function getZasekiImageInfo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("  INFO.SYUPT_DAY ")               ' 出発日
        sqlString.AppendLine("  , INFO.BUS_RESERVE_CD ")        ' バス指定コード
        sqlString.AppendLine("  , INFO.GOUSYA ")                ' 号車
        sqlString.AppendLine("  , INFO.ZASEKI_KAI ")            ' 階
        sqlString.AppendLine("  , INFO.ZASEKI_LINE ")           ' 行
        sqlString.AppendLine("  , INFO.ZASEKI_COL ")            ' 列
        sqlString.AppendLine("  , INFO.ZASEKI_KIND ")           ' 座席種別
        sqlString.AppendLine("  , INFO.ZASEKI_KBN ")            ' 座席区分
        sqlString.AppendLine("  , INFO.BLOCK_KBN ")             ' ブロック区分
        sqlString.AppendLine("  , INFO.EIGYOSYO_BLOCK_KBN ")    ' 営業所ブロック区分
        sqlString.AppendLine("  , INFO.EIGYOSYO_CD ")           ' 営業所コード
        sqlString.AppendLine("  , INFO.YOYAKU_FLG ")            ' 予約フラグ
        sqlString.AppendLine("  , INFO.YOYAKU_STATUS ")         ' 予約状態
        sqlString.AppendLine("  , INFO.GROUP_NO ")              ' 予約グループNo
        sqlString.AppendLine("  , INFO.ZASEKI_STATE")           ' 座席状態
        sqlString.AppendLine("  , INFO.YOYAKU_KBN")             ' 予約区分
        sqlString.AppendLine("  , INFO.YOYAKU_NO")              ' 予約番号
        sqlString.AppendLine("  , INFO.NLGSQN")                 ' 連番
        sqlString.AppendLine("  , INFO.LADIES_SEAT_FLG ")       ' 女性席フラグ
        sqlString.AppendLine("  , INFO.DELETE_DAY ")            ' 削除日
        'FROM句
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("  T_ZASEKI_IMAGE_INFO INFO ")
        'sqlString.AppendLine("  INNER JOIN T_ZASEKI_IMAGE_INFO INFO ")
        'sqlString.AppendLine("    ON IMAGE.BUS_RESERVE_CD = INFO.BUS_RESERVE_CD ")
        'sqlString.AppendLine("    AND IMAGE.SYUPT_DAY = INFO.SYUPT_DAY ")
        'sqlString.AppendLine("    AND IMAGE.GOUSYA = INFO.GOUSYA ")
        'sqlString.AppendLine("    AND INFO.ZASEKI_KIND <> '" & FixedCd.ZasekiKind.dummySeat & "' ")
        'sqlString.AppendLine("    AND INFO.ZASEKI_KBN = '" & FixedCd.ZasekiKbnType.zaseki & "' ")
        'WHERE句
        sqlString.AppendLine("WHERE ")
        sqlString.AppendLine(" INFO.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND INFO.BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND INFO.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

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
    Public Function executeBusReserveCdTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable, ByVal BusReserveCdFlg As Boolean, ByVal JyosyaCapacityFlg As Boolean) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateBusReserveCd
                    For i As Integer = 1 To 2
                        'バス指定コードを更新、更新後乗せ合わせ先の使用中フラグを初期化
                        If i = 1 Then
                            sqlString = executeUpdateBasicData(paramInfoList)
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                        Else
                            sqlString = executeUpdateBusReserveCdSakiData(paramInfoList)
                        End If
                    Next
                    'Case accessType.executeReturnBusReserveCd
                    '    sqlString = executeReturnBasicData(paramInfoList)
                    'Case accessType.executeUpdateBusReserveCdZaseki
                    '    sqlString = executeUpdateZasekiData(paramInfoList)
                'Case accessType.executeUpdateBusReserveCdSaki
                '    sqlString = executeUpdateBusReserveCdSakiData(paramInfoList)
                Case accessType.executeUpdateBusReserveCdSakiZaseki
                    '座席イメージ（バス情報）の更新
                    For i As Integer = 1 To 2
                        '移動先を更新して、移動元の座席イメージに削除日を設定
                        If i = 1 Then
                            sqlString = executeUpdateBusReserveCdSakiZasekiData(paramInfoList, BusReserveCdFlg, JyosyaCapacityFlg)
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                        Else
                            sqlString = executeUpdateBusReserveCdMotoZasekiData(paramInfoList)
                        End If
                    Next
                Case accessType.executeUpdateJosyaChange
                    sqlString = executeUpdateJosyaChange(paramInfoList, BusReserveCdFlg, JyosyaCapacityFlg)
                'Case accessType.executeUpdateBusReserveCdMotoZasekiData
                '    sqlString = executeUpdateBusReserveCdMotoZasekiData(paramInfoList)
                Case accessType.executeUpdateZasekiInfo
                    sqlString = executeUpdateZasekiInfo(paramInfoList)
                Case accessType.executeUpdateZasekiMotoInfo
                    sqlString = executeUpdateZasekiMotoInfo(paramInfoList)
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

    '''' <summary>
    '''' 使用中フラグ更新
    '''' </summary>
    '''' <param name="paramInfoList"></param>
    '''' <param name="systemupdatepgmid"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function executeUsingFlgCrs2(ByVal paramInfoList As Hashtable, ByVal systemupdatepgmid As String) As DataTable

    '    Dim totalValue As New DataTable
    '    Dim trn As OracleTransaction = Nothing
    '    Dim returnValue As Boolean = False

    '    Try

    '        totalValue.Columns.Add("USING_FLG")             '使用中フラグ

    '        'トランザクション開始
    '        trn = callBeginTransaction()

    '        Dim syuptsday As String = CType(paramInfoList.Item("SYUPT_DAY"), String)   '出発日
    '        Dim crscd As String = CType(paramInfoList.Item("BUS_RESERVE_CD"), String)  'コースコード
    '        Dim gousya As String = CType(paramInfoList.Item("GOUSYA"), String)         '号車

    '        returnValue = CommonCheckUtil.setUsingFlg_Crs(syuptsday, crscd, gousya, systemupdatepgmid, trn, True)

    '        Dim row2 As DataRow = totalValue.NewRow
    '        If returnValue = True Then
    '            row2("USING_FLG") = FixedCd.UsingFlg.Use
    '        Else
    '            row2("USING_FLG") = String.Empty
    '        End If

    '        totalValue.Rows.Add(row2)

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

    '''' <summary>
    '''' 座席イメージ：使用フラグ更新
    '''' </summary>
    '''' <param name="paramList">SQL引数</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function executeUpdateZasekiData(ByVal paramList As Hashtable) As String

    '    Dim sqlString As New StringBuilder
    '    paramClear()

    '    'UPDATE
    '    sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE ")
    '    sqlString.AppendLine(" SET ")
    '    sqlString.AppendLine("USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", paramList.Item("SYSTEM_UPDATE_DAY"), OracleDbType.Date))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
    '    sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
    '    'WHERE句
    '    sqlString.AppendLine(" WHERE ")
    '    sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
    '    sqlString.AppendLine(" AND ")
    '    sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
    '    sqlString.AppendLine(" AND ")
    '    sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

    '    Return sqlString.ToString

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
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        'sqlString.AppendLine(",JYOSYA_CAPACITY = " & setParam("JYOSYA_CAPACITY", paramList.Item("JYOSYA_CAPACITY"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",UKETUKE_GENTEI_NINZU = " & setParam("UKETUKE_GENTEI_NINZU", paramList.Item("UKETUKE_GENTEI_NINZU"), OracleDbType.Decimal, 1, 0))
        sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM_2", paramList.Item("YOYAKU_KANOU_NUM_2"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI_2", paramList.Item("KUSEKI_NUM_TEISEKI_2"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT_2", paramList.Item("KUSEKI_NUM_SUB_SEAT_2"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",YOYAKU_NUM_TEISEKI = " & setParam("YOYAKU_NUM_TEISEKI", paramList.Item("YOYAKU_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",YOYAKU_NUM_SUB_SEAT = " & setParam("YOYAKU_NUM_SUB_SEAT", paramList.Item("YOYAKU_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
        'If BusReserveCdFlg = True Then
        sqlString.AppendLine(",BLOCK_KAKUHO_NUM = " & setParam("BLOCK_KAKUHO_NUM", paramList.Item("BLOCK_KAKUHO_NUM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",EI_BLOCK_HO = " & setParam("EI_BLOCK_HO", paramList.Item("EI_BLOCK_HO"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",EI_BLOCK_REGULAR = " & setParam("EI_BLOCK_REGULAR", paramList.Item("EI_BLOCK_REGULAR"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",SUB_SEAT_OK_KBN = " & setParam("SUB_SEAT_OK_KBN", paramList.Item("SUB_SEAT_OK_KBN"), OracleDbType.Char))
        sqlString.AppendLine(",KUSEKI_KAKUHO_NUM = " & setParam("KUSEKI_KAKUHO_NUM", paramList.Item("KUSEKI_KAKUHO_NUM"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",BUS_COUNT_FLG = " & setParam("BUS_COUNT_FLG", paramList.Item("BUS_COUNT_FLG"), OracleDbType.Char))
        'End If
        'sqlString.AppendLine(",USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" (BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" OR CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char) & ")")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（基本）：乗せ合わせ先データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBusReserveCdSakiData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        'sqlString.AppendLine(" JYOSYA_CAPACITY = " & setParam("JYOSYA_CAPACITY", paramList.Item("JYOSYA_CAPACITY"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM", paramList.Item("YOYAKU_KANOU_NUM"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT", paramList.Item("KUSEKI_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine("USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        'sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        'sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        'sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 座席イメージ：乗せ合わせ先データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBusReserveCdSakiZasekiData(ByVal paramList As Hashtable, ByVal BusReserveCdFlg As Boolean, ByVal JyosyaCapacityFlg As Boolean) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT", paramList.Item("KUSEKI_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
        If BusReserveCdFlg = True Then
            sqlString.AppendLine(",USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        End If
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
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

    ''' <summary>
    ''' コース台帳（基本）：乗車定員・受付限定人数データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateJosyaChange(ByVal paramList As Hashtable, ByVal BusReserveCdFlg As Boolean, ByVal JyosyaCapacityFlg As Boolean) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("UKETUKE_GENTEI_NINZU = " & setParam("UKETUKE_GENTEI_NINZU", paramList.Item("UKETUKE_GENTEI_NINZU"), OracleDbType.Decimal, 1, 0))
        If BusReserveCdFlg = True OrElse JyosyaCapacityFlg = True Then
            sqlString.AppendLine(",JYOSYA_CAPACITY = " & setParam("JYOSYA_CAPACITY", paramList.Item("JYOSYA_CAPACITY"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM", paramList.Item("YOYAKU_KANOU_NUM"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT", paramList.Item("KUSEKI_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
        End If
        If BusReserveCdFlg = True Then
            sqlString.AppendLine(",BUS_COUNT_FLG = " & setParam("BUS_COUNT_FLG", paramList.Item("BUS_COUNT_FLG"), OracleDbType.Char))
        End If
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
    ''' 座席イメージ：乗せ合わせ元データ更新用（使用しないようになるため削除日を設定）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateBusReserveCdMotoZasekiData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("DELETE_DAY = " & setParam("DELETE_DAY", CommonDateUtil.getSystemTime.ToString("yyyyMMdd"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 座席イメージ（座席情報）：予約情報更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateZasekiInfo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE_INFO ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("YOYAKU_FLG = " & setParam("YOYAKU_FLG", paramList.Item("YOYAKU_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",YOYAKU_STATUS = " & setParam("YOYAKU_STATUS", paramList.Item("YOYAKU_STATUS"), OracleDbType.Char))
        sqlString.AppendLine(",GROUP_NO = " & setParam("GROUP_NO", paramList.Item("GROUP_NO"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ZASEKI_STATE = " & setParam("ZASEKI_STATE", paramList.Item("ZASEKI_STATE"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",YOYAKU_KBN = " & setParam("YOYAKU_KBN", paramList.Item("YOYAKU_KBN"), OracleDbType.Char))
        sqlString.AppendLine(",YOYAKU_NO = " & setParam("YOYAKU_NO", paramList.Item("YOYAKU_NO"), OracleDbType.Decimal, 9, 0))
        sqlString.AppendLine(",NLGSQN = " & setParam("NLGSQN", paramList.Item("NLGSQN"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",LADIES_SEAT_FLG = " & setParam("LADIES_SEAT_FLG", paramList.Item("LADIES_SEAT_FLG"), OracleDbType.Char))

        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" ZASEKI_KAI = " & setParam("ZASEKI_KAI", paramList.Item("ZASEKI_KAI"), OracleDbType.Decimal, 1, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" ZASEKI_LINE = " & setParam("ZASEKI_LINE", paramList.Item("ZASEKI_LINE"), OracleDbType.Decimal, 2, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" ZASEKI_COL = " & setParam("ZASEKI_COL", paramList.Item("ZASEKI_COL"), OracleDbType.Decimal, 2, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 座席イメージ（座席情報）：元データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateZasekiMotoInfo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE_INFO ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine("DELETE_DAY = " & setParam("DELETE_DAY", CommonDateUtil.getSystemTime.ToString("yyyyMMdd"), OracleDbType.Decimal, 8, 0))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 座席イメージ：乗車定員更新
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateJousyaZasekiData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_ZASEKI_IMAGE ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam("KUSEKI_NUM_SUB_SEAT", paramList.Item("KUSEKI_NUM_SUB_SEAT"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

#End Region


End Class
