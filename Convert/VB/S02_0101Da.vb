Imports System.Text

''' <summary>
''' コース一覧照会のDAクラス
''' </summary>
Public Class S02_0101Da
    Inherits Hatobus.ReservationManagementSystem.Common.DataAccessorBase
#Region " 定数／変数 "
    ''' <summary>
    ''' 運行区分：運休（廃止）
    ''' </summary>
    Private Const UNKOU_KBN_HAISI = "X"

    ''' <summary>
    ''' 乗車定員（空席表示に「バスの空席数」判断用）
    ''' </summary>
    Private Const JYOSYA_CAPACITY_BUS As Integer = 999

    ''' <summary>
    ''' 検索方法
    ''' </summary>
    Public Enum AccessType As Integer
        courseByHeaderKey   '一覧結果取得検索
        courseByPrimaryKey  'キー重複チェック
        courseByCheck       '存在チェック
    End Enum

    'コース台帳（基本）エンティティ
    Private ReadOnly ClsCrsDaityoEntity As New CrsLedgerBasicEntity()
    '場所マスタエンティティ
    Private ReadOnly ClsPlaceMasteroEntity As New PlaceMasterEntity()
    'コースコントロール情報エンティティ
    Private ReadOnly ClsCrsContorolInfoEntity As New CrsControlInfoEntity()

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function AccessCourseDaityo(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case AccessType.courseByCheck
                '一覧結果取得検索
                sqlString = GetCourseCdCheck(paramInfoList)
            Case AccessType.courseByHeaderKey
                '一覧結果取得検索
                sqlString = GetCourseDaityo(paramInfoList)
            Case AccessType.courseByPrimaryKey
                'キー重複チェック
                sqlString = GetCourseDataByPrimaryKey(paramInfoList)
            Case Else
                '該当処理なし
                Return returnValue
        End Select

        Try
            returnValue = getDataTable(sqlString)
        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' コース一覧照会検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function GetCourseDaityo(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        Dim s02_0101 As New S02_0101

        With clsCrsDaityoEntity
            'SELECT句
            sqlString.AppendLine("SELECT ")
            sqlString.AppendLine("  DISTINCT ")
            sqlString.AppendLine("  '' AS YoyakuButton,")                                   '予約ボタン
            sqlString.AppendLine("  '' AS TojituHakken,")                                   '当日発券ボタン
            '出発日
            sqlString.AppendLine(" SUBSTR(XX.SYUPT_DAY,1,4)||'/'||SUBSTR(XX.SYUPT_DAY,5,2)||'/'||")
            sqlString.AppendLine(" SUBSTR(XX.SYUPT_DAY,7,2) AS SYUPT_DAY_HEN,")
            '出発時間
            sqlString.AppendLine(" CASE XX.SYUPT_TIME WHEN 0 THEN '' ELSE ")
            sqlString.AppendLine("  SUBSTR(LPAD(XX.SYUPT_TIME,4,'0'), 1, 2)||':'||")
            sqlString.AppendLine("  SUBSTR(LPAD(XX.SYUPT_TIME,4,'0'), 3, 2) END AS SYUPT_TIME_HEN,")

            sqlString.AppendLine("  XX.CRS_CD,")                                            'コースコード
            sqlString.AppendLine("  NVL(XX.CRS_NAME_RK,XX.CRS_NAME) AS CRS_NAME,")          'コース名称
            sqlString.AppendLine("  ''  AS SITUATION,")                                     '状態
            sqlString.AppendLine("  PLACE.PLACE_NAME_SHORT || KEIYUKBN AS HAISYA_KEIYU,")   '配車経由
            sqlString.AppendLine("  XX.GOUSYA,")                                            '号車
            '空席数定席（乗車定員・定員が999で設定されている場合の表示方法はバスの空席数）
            sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " & JYOSYA_CAPACITY_BUS & " THEN ")
            sqlString.AppendLine("      NVL(ZSK.KUSEKI_NUM_TEISEKI,0) ")
            sqlString.AppendLine("    ELSE ")
            sqlString.AppendLine("      NVL(XX.KUSEKI_NUM_TEISEKI,0)")
            sqlString.AppendLine("  END AS KUSEKI_NUM_TEISEKI,")                            '空席数定席
            '空席数補助席（乗車定員・定員が999で設定されている場合の表示方法はバスの空席数）
            sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " & JYOSYA_CAPACITY_BUS & " THEN ")
            sqlString.AppendLine("      NVL(ZSK.KUSEKI_NUM_SUB_SEAT,0) ")
            sqlString.AppendLine("    ELSE ")
            sqlString.AppendLine("      NVL(XX.KUSEKI_NUM_SUB_SEAT,0) ")
            sqlString.AppendLine("  END AS KUSEKI_NUM_SUB_SEAT,")                           '空席数補助席

            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_TEISEKI,0) + ")                       '予約数定席
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_SUB_SEAT,0) + ")                      '予約数補助席
            sqlString.AppendLine("  NVL(XX.BLOCK_KAKUHO_NUM,0) + ")                         'ブロック確保数
            sqlString.AppendLine("  NVL(XX.KUSEKI_KAKUHO_NUM,0) AS ALLNUM_UTIWAKE,")        '=総数内訳
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_TEISEKI,0) + ")
            sqlString.AppendLine("  NVL(XX.YOYAKU_NUM_SUB_SEAT,0) AS YOYAKU_NUM_TEISEKI,")  '予約数定席
            sqlString.AppendLine("  NVL(XX.BLOCK_KAKUHO_NUM,0) AS BLOCK_KAKUHO_NUM,")       'ブロック確保数
            sqlString.AppendLine("  NVL(XX.KUSEKI_KAKUHO_NUM,0) AS KUSEKI_KAKUHO_NUM,")     '空席確保数
            sqlString.AppendLine("  NVL(XX.CANCEL_WAIT_NINZU,0) AS CANCEL_WAIT_NINZU,")     'WT件数
            sqlString.AppendLine("  0 AS REQUEST_KENSU ,")                                  'リクエスト件数       TODO 二次対応
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_ONE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_ONE_1R,")     '部屋残数１人部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_TWO_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_TWO_1R,")     '部屋残数２人部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_THREE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_THREE_1R,") '部屋残数３名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_FOUR_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_FOUR_1R,")   '部屋残数４名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_FIVE_ROOM,NVL(ROOM_ZANSU_SOKEI,0)) AS ROOM_ZANSU_FIVE_1R,")   '部屋残数５名部屋
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI,")       '部屋残数総計
            sqlString.AppendLine("  CODEBUS.CODE_NAME  AS BUS_TYPE,")                       'バスタイプ
            sqlString.AppendLine("  XX.HAISYA_KEIYU_CD,")                                   '配車経由コード
            sqlString.AppendLine("  XX.GUIDE_GENGO, ")                                      'ガイド言語
            sqlString.AppendLine("  CODE1.CODE_NAME AS CATEGORY1, ")                        'コード名称1（カテゴリ）
            sqlString.AppendLine("  CODE2.CODE_NAME AS CATEGORY2, ")                        'コード名称2（カテゴリ）
            sqlString.AppendLine("  CODE3.CODE_NAME AS CATEGORY3, ")                        'コード名称3（カテゴリ）
            sqlString.AppendLine("  CODE4.CODE_NAME AS CATEGORY4, ")                        'コード名称4（カテゴリ）
            sqlString.AppendLine("  XX.ONE_SANKA_FLG, ")                                    '１名参加フラグ
            sqlString.AppendLine("  XX.TEIKI_KIKAKU_KBN, ")                                 '定期・企画区分
            sqlString.AppendLine("  XX.HOUJIN_GAIKYAKU_KBN, ")                              '邦人/ 外客区分  
            sqlString.AppendLine("  XX.CRS_KBN_1, ")                                        'コース区分１  /昼 / 夜 
            sqlString.AppendLine("  XX.CRS_KBN_2, ")                                        'コース区分２  /都内 / 郊外
            sqlString.AppendLine("  XX.CRS_KIND, ")                                         'コース種別  /はとバス定期 / ｷｬﾋﾟﾀﾙ / 企画日帰り / 企画宿泊 / 企画都内Rコース
            sqlString.AppendLine("  XX.CAR_TYPE_CD, ")                                      '車種コード
            sqlString.AppendLine("  XX.TEJIMAI_KBN, ")                                      '手仕舞区分
            sqlString.AppendLine("  XX.YOYAKU_STOP_FLG, ")                                  '予約停止フラグ
            sqlString.AppendLine("  XX.UNKYU_KBN, ")                                        '運休区分    /運休/廃止
            sqlString.AppendLine("  XX.SAIKOU_KAKUTEI_KBN, ")                               '催行確定区分
            sqlString.AppendLine("  XX.UKETUKE_START_DAY, ")                                '受付開始日
            sqlString.AppendLine("  XX.SYUPT_DAY,")                                         '出発日
            sqlString.AppendLine("  XX.SYUPT_TIME,")                                        '出発時間
            sqlString.AppendLine("  XX.TEIINSEI_FLG, ")                                     '定員制フラグ
            sqlString.AppendLine("  NVL(XX.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI ")       '部屋残数総計

            'FROM句
            sqlString.AppendLine("FROM")
            sqlString.AppendLine("  (SELECT ")
            sqlString.AppendLine("      SYUPT_TIME_1 AS SYUPT_TIME,")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_1 AS HAISYA_KEIYU_CD,")
            sqlString.AppendLine("      '' AS KEIYUKBN,")                                   '乗り場/経由区分（ ’’ =出発地）
            sqlString.AppendLine("      SYUPT_DAY,")
            sqlString.AppendLine("      GOUSYA,")
            sqlString.AppendLine("      CRS_CD,")
            sqlString.AppendLine("      CRS_NAME_RK,")
            sqlString.AppendLine("      CRS_NAME,")
            sqlString.AppendLine("      UKETUKE_START_DAY,")
            sqlString.AppendLine("      JYOSYA_CAPACITY,")
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,")
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,")
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,")
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,")
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,")
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,")
            sqlString.AppendLine("      GUIDE_GENGO,")
            sqlString.AppendLine("      ONE_SANKA_FLG,")
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,")
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,")
            sqlString.AppendLine("      CRS_KBN_1,")
            sqlString.AppendLine("      CRS_KBN_2,")
            sqlString.AppendLine("      CRS_KIND,")
            sqlString.AppendLine("      CAR_TYPE_CD,")
            sqlString.AppendLine("      BUS_RESERVE_CD,")
            sqlString.AppendLine("      HOMEN_CD,")
            sqlString.AppendLine("      TEJIMAI_KBN,")
            sqlString.AppendLine("      YOYAKU_NG_FLG,")
            sqlString.AppendLine("      YOYAKU_STOP_FLG,")
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,")
            sqlString.AppendLine("      TEIINSEI_FLG,")
            sqlString.AppendLine("      UNKYU_KBN,")
            sqlString.AppendLine("      CATEGORY_CD_1,")
            sqlString.AppendLine("      CATEGORY_CD_2,")
            sqlString.AppendLine("      CATEGORY_CD_3,")
            sqlString.AppendLine("      CATEGORY_CD_4")
            sqlString.AppendLine("    FROM ")
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC")
            sqlString.AppendLine("    WHERE ")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_1 IS Not NULL")         '配車経由地コード1が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ")             '削除日＝0
            sqlString.AppendLine("   UNION ALL")
            sqlString.AppendLine("   SELECT ")
            sqlString.AppendLine("      SYUPT_TIME_2 AS SYUPT_TIME,")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_2 AS HAISYA_KEIYU_CD,")
            sqlString.AppendLine("      '*' AS KEIYUKBN,")                                  '乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,")
            sqlString.AppendLine("      GOUSYA,")
            sqlString.AppendLine("      CRS_CD,")
            sqlString.AppendLine("      CRS_NAME_RK,")
            sqlString.AppendLine("      CRS_NAME,")
            sqlString.AppendLine("      UKETUKE_START_DAY,")
            sqlString.AppendLine("      JYOSYA_CAPACITY,")
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,")
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,")
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,")
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,")
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,")
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,")
            sqlString.AppendLine("      GUIDE_GENGO,")
            sqlString.AppendLine("      ONE_SANKA_FLG,")
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,")
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,")
            sqlString.AppendLine("      CRS_KBN_1,")
            sqlString.AppendLine("      CRS_KBN_2,")
            sqlString.AppendLine("      CRS_KIND,")
            sqlString.AppendLine("      CAR_TYPE_CD,")
            sqlString.AppendLine("      BUS_RESERVE_CD,")
            sqlString.AppendLine("      HOMEN_CD,")
            sqlString.AppendLine("      TEJIMAI_KBN,")
            sqlString.AppendLine("      YOYAKU_NG_FLG,")
            sqlString.AppendLine("      YOYAKU_STOP_FLG,")
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,")
            sqlString.AppendLine("      TEIINSEI_FLG,")
            sqlString.AppendLine("      UNKYU_KBN,")
            sqlString.AppendLine("      CATEGORY_CD_1,")
            sqlString.AppendLine("      CATEGORY_CD_2,")
            sqlString.AppendLine("      CATEGORY_CD_3,")
            sqlString.AppendLine("      CATEGORY_CD_4")
            sqlString.AppendLine("    FROM ")
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC")
            sqlString.AppendLine("    WHERE ")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_2 IS Not NULL")         '配車経由地コード2が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ")             '削除日＝0
            sqlString.AppendLine("   UNION ALL")
            sqlString.AppendLine("   SELECT ")
            sqlString.AppendLine("      SYUPT_TIME_3 AS SYUPT_TIME,")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_3 AS HAISYA_KEIYU_CD,")
            sqlString.AppendLine("      '*' AS KEIYUKBN,")                                  '乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,")
            sqlString.AppendLine("      GOUSYA,")
            sqlString.AppendLine("      CRS_CD,")
            sqlString.AppendLine("      CRS_NAME_RK,")
            sqlString.AppendLine("      CRS_NAME,")
            sqlString.AppendLine("      UKETUKE_START_DAY,")
            sqlString.AppendLine("      JYOSYA_CAPACITY,")
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,")
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,")
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,")
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,")
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,")
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,")
            sqlString.AppendLine("      GUIDE_GENGO,")
            sqlString.AppendLine("      ONE_SANKA_FLG,")
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,")
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,")
            sqlString.AppendLine("      CRS_KBN_1,")
            sqlString.AppendLine("      CRS_KBN_2,")
            sqlString.AppendLine("      CRS_KIND,")
            sqlString.AppendLine("      CAR_TYPE_CD,")
            sqlString.AppendLine("      BUS_RESERVE_CD,")
            sqlString.AppendLine("      HOMEN_CD,")
            sqlString.AppendLine("      TEJIMAI_KBN,")
            sqlString.AppendLine("      YOYAKU_NG_FLG,")
            sqlString.AppendLine("      YOYAKU_STOP_FLG,")
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,")
            sqlString.AppendLine("      TEIINSEI_FLG,")
            sqlString.AppendLine("      UNKYU_KBN,")
            sqlString.AppendLine("      CATEGORY_CD_1,")
            sqlString.AppendLine("      CATEGORY_CD_2,")
            sqlString.AppendLine("      CATEGORY_CD_3,")
            sqlString.AppendLine("      CATEGORY_CD_4")
            sqlString.AppendLine("    FROM ")
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC")
            sqlString.AppendLine("    WHERE ")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_3 IS Not NULL")         '配車経由地コード3が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ")             '削除日＝0
            sqlString.AppendLine("   UNION ALL")
            sqlString.AppendLine("   SELECT ")
            sqlString.AppendLine("      SYUPT_TIME_4 AS SYUPT_TIME,")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_4 AS HAISYA_KEIYU_CD,")
            sqlString.AppendLine("      '*' AS KEIYUKBN,")                                  '乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,")
            sqlString.AppendLine("      GOUSYA,")
            sqlString.AppendLine("      CRS_CD,")
            sqlString.AppendLine("      CRS_NAME_RK,")
            sqlString.AppendLine("      CRS_NAME,")
            sqlString.AppendLine("      UKETUKE_START_DAY,")
            sqlString.AppendLine("      JYOSYA_CAPACITY,")
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,")
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,")
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,")
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,")
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,")
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,")
            sqlString.AppendLine("      GUIDE_GENGO,")
            sqlString.AppendLine("      ONE_SANKA_FLG,")
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,")
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,")
            sqlString.AppendLine("      CRS_KBN_1,")
            sqlString.AppendLine("      CRS_KBN_2,")
            sqlString.AppendLine("      CRS_KIND,")
            sqlString.AppendLine("      CAR_TYPE_CD,")
            sqlString.AppendLine("      BUS_RESERVE_CD,")
            sqlString.AppendLine("      HOMEN_CD,")
            sqlString.AppendLine("      TEJIMAI_KBN,")
            sqlString.AppendLine("      YOYAKU_NG_FLG,")
            sqlString.AppendLine("      YOYAKU_STOP_FLG,")
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,")
            sqlString.AppendLine("      TEIINSEI_FLG,")
            sqlString.AppendLine("      UNKYU_KBN,")
            sqlString.AppendLine("      CATEGORY_CD_1,")
            sqlString.AppendLine("      CATEGORY_CD_2,")
            sqlString.AppendLine("      CATEGORY_CD_3,")
            sqlString.AppendLine("      CATEGORY_CD_4")
            sqlString.AppendLine("    FROM ")
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC")
            sqlString.AppendLine("    WHERE ")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_4 IS Not NULL")         '配車経由地コード4が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ")             '削除日＝0
            sqlString.AppendLine("   UNION ALL")
            sqlString.AppendLine("   SELECT ")
            sqlString.AppendLine("      SYUPT_TIME_5 AS SYUPT_TIME,")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_5 AS HAISYA_KEIYU_CD,")
            sqlString.AppendLine("      '*' AS KEIYUKBN,")                                  '乗り場/経由区分（ ’*’ =経由地）
            sqlString.AppendLine("      SYUPT_DAY,")
            sqlString.AppendLine("      GOUSYA,")
            sqlString.AppendLine("      CRS_CD,")
            sqlString.AppendLine("      CRS_NAME_RK,")
            sqlString.AppendLine("      CRS_NAME,")
            sqlString.AppendLine("      UKETUKE_START_DAY,")
            sqlString.AppendLine("      JYOSYA_CAPACITY,")
            sqlString.AppendLine("      YOYAKU_NUM_TEISEKI,")
            sqlString.AppendLine("      YOYAKU_NUM_SUB_SEAT,")
            sqlString.AppendLine("      BLOCK_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_KAKUHO_NUM,")
            sqlString.AppendLine("      KUSEKI_NUM_TEISEKI,")
            sqlString.AppendLine("      KUSEKI_NUM_SUB_SEAT,")
            sqlString.AppendLine("      CANCEL_WAIT_NINZU,")
            sqlString.AppendLine("      ROOM_ZANSU_ONE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_TWO_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_THREE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FOUR_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_FIVE_ROOM,")
            sqlString.AppendLine("      ROOM_ZANSU_SOKEI,")
            sqlString.AppendLine("      GUIDE_GENGO,")
            sqlString.AppendLine("      ONE_SANKA_FLG,")
            sqlString.AppendLine("      TEIKI_KIKAKU_KBN,")
            sqlString.AppendLine("      HOUJIN_GAIKYAKU_KBN,")
            sqlString.AppendLine("      CRS_KBN_1,")
            sqlString.AppendLine("      CRS_KBN_2,")
            sqlString.AppendLine("      CRS_KIND,")
            sqlString.AppendLine("      CAR_TYPE_CD,")
            sqlString.AppendLine("      BUS_RESERVE_CD,")
            sqlString.AppendLine("      HOMEN_CD,")
            sqlString.AppendLine("      TEJIMAI_KBN,")
            sqlString.AppendLine("      YOYAKU_NG_FLG,")
            sqlString.AppendLine("      YOYAKU_STOP_FLG,")
            sqlString.AppendLine("      SAIKOU_KAKUTEI_KBN,")
            sqlString.AppendLine("      TEIINSEI_FLG,")
            sqlString.AppendLine("      UNKYU_KBN,")
            sqlString.AppendLine("      CATEGORY_CD_1,")
            sqlString.AppendLine("      CATEGORY_CD_2,")
            sqlString.AppendLine("      CATEGORY_CD_3,")
            sqlString.AppendLine("      CATEGORY_CD_4")
            sqlString.AppendLine("    FROM ")
            sqlString.AppendLine("      T_CRS_LEDGER_BASIC")
            sqlString.AppendLine("    WHERE ")
            sqlString.AppendLine("      HAISYA_KEIYU_CD_5 IS Not NULL")  '配車経由地コード5が入っているか
            sqlString.AppendLine("    AND NVL(DELETE_DAY, 0) = 0 ")      '削除日＝0
            sqlString.AppendLine("  ) XX ")
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE1  ON   RTRIM(CODE1.CODE_BUNRUI) = '" & FixedCdYoyaku.CodeBunruiTypeCategory & "'")
            sqlString.AppendLine("  AND RTRIM(CODE1.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_1) ")
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE2  ON   RTRIM(CODE2.CODE_BUNRUI) = '" & FixedCdYoyaku.CodeBunruiTypeCategory & "'")
            sqlString.AppendLine("  AND RTRIM(CODE2.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_2) ")
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE3  ON   RTRIM(CODE3.CODE_BUNRUI) = '" & FixedCdYoyaku.CodeBunruiTypeCategory & "'")
            sqlString.AppendLine("  AND RTRIM(CODE3.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_3) ")
            sqlString.AppendLine("LEFT JOIN  M_CODE CODE4  ON   RTRIM(CODE4.CODE_BUNRUI) = '" & FixedCdYoyaku.CodeBunruiTypeCategory & "'")
            sqlString.AppendLine("  AND RTRIM(CODE4.CODE_VALUE) = RTRIM(XX.CATEGORY_CD_4) ")
            sqlString.AppendLine("LEFT JOIN  M_PLACE PLACE  ON  RTRIM(PLACE.PLACE_CD) = RTRIM(XX.HAISYA_KEIYU_CD) ")
            sqlString.AppendLine("LEFT JOIN  M_CAR_KIND CAR_KIND  ON  RTRIM(CAR_KIND.CAR_CD) = RTRIM(XX.CAR_TYPE_CD) ")
            sqlString.AppendLine("LEFT JOIN  M_CODE CODEBUS  ON   RTRIM(CODEBUS.CODE_BUNRUI) = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.busType) & "'")
            sqlString.AppendLine("  AND RTRIM(CODEBUS.CODE_VALUE) = RTRIM(CAR_KIND.BUS_TYPE) ")
            sqlString.AppendLine("LEFT JOIN  T_ZASEKI_IMAGE ZSK ON  RTRIM(ZSK.BUS_RESERVE_CD) = RTRIM(XX.BUS_RESERVE_CD) ")
            sqlString.AppendLine("  AND RTRIM(ZSK.GOUSYA) = RTRIM(XX.GOUSYA) ")
            sqlString.AppendLine("  AND RTRIM(ZSK.SYUPT_DAY) = RTRIM(XX.SYUPT_DAY) ")
            '【フリーワード指定時】
            If Not String.IsNullOrEmpty(paramList.Item("FreeWord").ToString.Trim) Then
                'コースマスタ
                sqlString.AppendLine("LEFT JOIN  T_COURSE_MST CRS_M ON  RTRIM(CRS_M.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(CRS_M.TEIKI_KIKAKU_KBN) = RTRIM(XX.TEIKI_KIKAKU_KBN) ")
                'コースマスタ（発着）ダイヤ[定期]
                sqlString.AppendLine("LEFT JOIN T_COURSE_DIA DIA1 ON  RTRIM(DIA1.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(DIA1.TEIKI_KIKAKU_KBN) = 1 AND DIA1.LINE_NO >= 1 AND DIA1.LINE_NO <= 15")
                '                 ダイヤ[企画]
                sqlString.AppendLine("LEFT JOIN T_COURSE_DIA DIA2 ON  RTRIM(DIA2.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(DIA2.TEIKI_KIKAKU_KBN) = 2 AND DIA1.LINE_NO >= 1 AND DIA1.LINE_NO <= 4")

                '○増管理　食事詳細
                sqlString.AppendLine("LEFT JOIN  T_CIB_MEAL_DETAIL CIB_MD ON RTRIM(CIB_MD.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(CIB_MD.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ")
                sqlString.AppendLine("  AND RTRIM(CIB_MD.COACH_NO) = RTRIM(XX.GOUSYA) ")

                '○増管理　宿泊詳細	T_CIB_HOTEL_DETAIL
                sqlString.AppendLine("LEFT JOIN  T_CIB_HOTEL_DETAIL CIB_HD ON RTRIM(CIB_HD.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(CIB_HD.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ")
                sqlString.AppendLine("  AND RTRIM(CIB_HD.COACH_NO) = RTRIM(XX.GOUSYA) ")

                '○増管理　降車ヶ所（ホテル込み）	T_CIB_KOUSYA_PLACE
                sqlString.AppendLine("LEFT JOIN  T_CIB_KOUSYA_PLACE CIB_KP ON RTRIM(CIB_KP.CRS_CD) = RTRIM(XX.CRS_CD) ")
                sqlString.AppendLine("  AND RTRIM(CIB_KP.SYUPT_DATE) = RTRIM(XX.SYUPT_DAY) ")
                sqlString.AppendLine("  AND RTRIM(CIB_KP.COACH_NO) = RTRIM(XX.GOUSYA) ")
            End If
            'WHERE句
            sqlString.AppendLine("WHERE ")
            sqlString.AppendLine("      NVL(XX.UNKYU_KBN,' ') != '" & UNKOU_KBN_HAISI & "' ")    '運行区分＝ 'X'：運休(廃止)
            sqlString.AppendLine("  AND XX.YOYAKU_NG_FLG IS NULL ")                              '予約不可フラグ = ブランク 
            sqlString.AppendLine("  AND NVL(XX.SAIKOU_KAKUTEI_KBN, ' ') != 'X' ")                '催行確定区分 != 'X'：廃止

            '   通常受付開始日に値がない場合、コースを表示しないが、
            '   本社と予約センターのリーダー以上→表示
            If UserInfoManagement.eigyosyoKbn = FixedCd.EigyosyoKbn.SystemPromote Or
              (UserInfoManagement.eigyosyoKbn = FixedCd.EigyosyoKbn.ReserveCenter
                ) Then
                s02_0101.UketukeStartDayChkFlg = False
            Else
                sqlString.AppendLine("  AND NVL(RTRIM(XX.UKETUKE_START_DAY),0) > 0 ")  'かつ コース台帳（基本）.受付開始日= 0   
            End If

            If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                ''パラメータ（SIngleコースコード）が設定されている場合
                'If Not String.IsNullOrEmpty(paramList.Item("SingleCrsCd").ToString.Trim) Then
                '    sqlString.AppendLine("AND  XX.CRS_CD !='" & paramList.Item("SingleCrsCd").ToString.Trim & "' ")               'SIngleコースコード
                'End If
                ''パラメータ（Twinコースコード）が設定されている場合
                'If Not String.IsNullOrEmpty(paramList.Item("TwinCrsCd").ToString.Trim) Then
                '    sqlString.AppendLine("AND  XX.CRS_CD !='" & paramList.Item("TwinCrsCd").ToString.Trim & "' ")                  'Twinコースコード
                'End If

                'パラメータ（出発日：開始）が設定されている場合
                If Not String.IsNullOrEmpty(paramList.Item("SyuptDayFrom").ToString.Trim) AndAlso IsNumeric(paramList.Item("SyuptDayFrom")) Then
                    sqlString.AppendLine("AND  XX.SYUPT_DAY >=" & CInt(paramList.Item("SyuptDayFrom")) & " ")    '出発日:開始
                End If
                'パラメータ（出発日：終了）が設定されている場合
                If Not String.IsNullOrEmpty(paramList.Item("SyuptDayTo").ToString.Trim) AndAlso IsNumeric(paramList.Item("SyuptDayFrom")) Then
                    sqlString.AppendLine("AND  XX.SYUPT_DAY <=" & CInt(paramList.Item("SyuptDayTo")) & " ")      '出発日:終了
                End If
                ''   【出発年月From,Toとも当日で指定された場合】
                'If paramList.Item("SyuptDayFrom").ToString.Trim = paramList.Item("SyuptDayTo").ToString.Trim Then
                '    sqlString.AppendLine("AND XX.SYUPT_TIME >=" & Now().Hour * 100 + Now().Minute & " ")                   '出発時間
                'End If

                '《予約メニュー》からのパラメータ：検索可能コース ("1":すべて、"2":定期旅行）が "2":定期旅行 の場合に、コース台帳.定期企画区分＝1(:定期)のみを検索する。
                If paramList.Item("SearchKanouCrs").ToString = S02_0101.SearchCrs_Teiki Then
                    sqlString.AppendLine("AND  XX.TEIKI_KIKAKU_KBN ='" & TeikiKikakuKbn.Teiki & "' ")
                End If
                '【コースコード指定時】

                If CBool(paramList.Item("CrsCdAssigne")) = True Then                         'コースコードFull桁指定時
                    sqlString.AppendLine("AND  XX.CRS_CD ='" & paramList.Item("CrsCd").ToString.Trim & "' ")               'コースコード
                Else
                    If Not String.IsNullOrEmpty(paramList.Item("CrsCd").ToString.Trim) Then
                        sqlString.AppendLine("AND  XX.CRS_CD LIKE '" & Trim(paramList.Item("CrsCd").ToString.Trim) & "%' ")
                    End If
                End If

                '【言語指定時】    'TODO:ガイド言語（分類コード決定後要修正）
                'コース台帳（基本）.ガイド言語 = 画面項目 : 言語（コード）
                If Not String.IsNullOrEmpty(paramList.Item("GuidoGengo").ToString.Trim) Then
                    sqlString.AppendLine("AND  XX.GUIDE_GENGO = '" & Trim(paramList.Item("GuidoGengo").ToString.Trim) & "' ")
                End If

                '【フリーワード指定時】　　※複数ワード（文言をスペースで区切った場合）はAND検索とする。
                ' ’FreeWord’ をスペース区切りで分割して配列に格納する
                If Not String.IsNullOrEmpty(paramList.Item("FreeWord").ToString.Trim) Then
                    Dim strArrayWord As String() = paramList.Item("FreeWord").ToString.Trim.Replace("　", " ").Split(" "c)
                    Dim idxForWord As Integer = 1
                    For Each strWord As String In strArrayWord
                        Dim freeWord As String = MyBase.setParam("FREE_WORD" & idxForWord.ToString, "%" & Trim(strWord) & "%", OracleDbType.Varchar2, 50)
                        'コース名   コースマスタ（基本）.コース名
                        sqlString.AppendLine("AND  ( XX.CRS_NAME LIKE " & freeWord & " ")
                        '目的       コース台帳（基本）.カテゴリコード１～４
                        sqlString.AppendLine(" OR    CODE1.CODE_NAME LIKE " & freeWord & " ")
                        sqlString.AppendLine(" OR    CODE2.CODE_NAME LIKE " & freeWord & " ")
                        sqlString.AppendLine(" OR    CODE3.CODE_NAME LIKE " & freeWord & " ")
                        sqlString.AppendLine(" OR    CODE4.CODE_NAME LIKE " & freeWord & " ")
                        '期間               コースマスタ（基本）.期間（文章）
                        sqlString.AppendLine(" OR    CRS_M.KIKAN_BUNSYO LIKE " & freeWord & " ")
                        'コースのポイント   コースマスタ（基本）.全体見所情報
                        sqlString.AppendLine(" OR    CRS_M.ZENTAI_MIDOKORO_INFO LIKE " & freeWord & " ")
                        'エリア
                        '   方面コード
                        '       (コースが「企画」で、コース種別２が「日帰り(1)」の場合は
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '" & TeikiKikakuKbn.Kikaku & "' ")
                        sqlString.AppendLine("   AND XX.HOMEN_CD LIKE '%" & "4" & Trim(strWord) & "%' )")
                        '                         コース種別２が「宿泊(2)」の場合は
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '" & TeikiKikakuKbn.Kikaku & "' ")
                        sqlString.AppendLine("   AND XX.HOMEN_CD LIKE '%" & "5" & Trim(strWord) & "%' )")

                        '   ダイヤ[定期].配車経由地１～５
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '1' ")
                        sqlString.AppendLine("   AND (   (DIA1.HAISYA_KEIYU_TI_1 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_1) = 1) ")
                        sqlString.AppendLine("        OR (DIA1.HAISYA_KEIYU_TI_2 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_2) = 1) ")
                        sqlString.AppendLine("        OR (DIA1.HAISYA_KEIYU_TI_3 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_3) = 1) ")
                        sqlString.AppendLine("       ) ")
                        sqlString.AppendLine("      )")
                        '   ダイヤ[企画].配車経由地１～３
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '2' ")
                        sqlString.AppendLine("   AND (   (DIA2.HAISYA_KEIYU_TI_1 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_1) = 1) ")
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_2 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_2) = 1) ")
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_3 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_3) = 1) ")
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_4 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_4) = 1) ")
                        sqlString.AppendLine("        OR (DIA2.HAISYA_KEIYU_TI_5 LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.HAISYA_KEIYU_TI_5) = 1) ")
                        sqlString.AppendLine("       ) ")
                        sqlString.AppendLine("      )")

                        '   ダイヤ[定期].ダイヤ.終了場所
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '1' ")
                        sqlString.AppendLine("   AND (   (DIA1.END_PLACE LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA1.END_PLACE) = 1) ")
                        sqlString.AppendLine("       ) ")
                        sqlString.AppendLine("      )")

                        '   ダイヤ[企画].ダイヤ.終了場所
                        sqlString.AppendLine(" OR   (XX.TEIKI_KIKAKU_KBN = '2' ")
                        sqlString.AppendLine("   AND (   (DIA2.END_PLACE LIKE " & freeWord & "  AND ( SELECT CRS_JYOSYA_PLACE_FLG FROM M_PLACE WHERE PLACE_CD = DIA2.END_PLACE) = 1) ")
                        sqlString.AppendLine("       ) ")
                        sqlString.AppendLine("      )")

                        '   出発時キャリア区分が "2":その他 の場合は 出発時間・キャリア が対象
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN <> '1' ")
                        sqlString.AppendLine("   AND CRS_M.SYUPT_TIME_CARRIER LIKE " & freeWord & "  )")
                        '   出発時キャリア区分が "1":バス の場合は 
                        '    ①発場所コード・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ")
                        sqlString.AppendLine("   AND CRS_M.SYUPT_PLACE_CD_CARRIER LIKE " & freeWord & "  )")
                        '    ②出発場所・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ")
                        sqlString.AppendLine("   AND CRS_M.SYUPT_PLACE_CARRIER LIKE " & freeWord & "  )")
                        '    ③到着時間・キャリア
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ")
                        sqlString.AppendLine("   AND CRS_M.TTYAK_TIME_CARRIER LIKE " & freeWord & "  )")
                        '    ④到着場所コード・キャリアが対象
                        sqlString.AppendLine(" OR   (CRS_M.SYUPT_CARRIER_KBN = '1' ")
                        sqlString.AppendLine("   AND CRS_M.TTYAK_CD_CARRIER LIKE " & freeWord & "  )")

                        '行程
                        '   降車ヶ所（ホテル込み）.行程種別		
                        sqlString.AppendLine(" OR   (CIB_KP.WORK_KIND = '1' AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" & FixedCdYoyaku.CodeBunruiTypeitIneraryKind & "' AND CODE_NAME LIKE " & freeWord & "  ) >= 1) ")
                        '   降車ヶ所（ホテル込み）.日次
                        If strWord = "日次" Then
                            sqlString.AppendLine(" OR    CIB_KP.DAILY > 0 ")
                        End If
                        '   降車ヶ所（ホテル込み）.種別
                        If strWord = "0" Or strWord = "仕入先マスタ" Or strWord = "仕入先" Then
                            sqlString.AppendLine(" OR   CIB_KP.KIND ＝ 0 ")
                        End If
                        If strWord = "1" Or strWord = "車窓マスタ" Or strWord = "車窓" Then
                            sqlString.AppendLine(" OR   CIB_KP.KIND ＝ 1 ")
                        End If
                        If strWord = "2" Or strWord = "場所マスタ" Or strWord = "場所" Then
                            sqlString.AppendLine(" OR   (CIB_KP.KIND ＝2 ")
                        End If
                        '   降車ヶ所（ホテル込み）.降車ヶ所(種別毎)
                        '                          種別=0:仕入先マスタ  
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 0 ")
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_SIIRE_SAKI WHERE SIIRE_SAKI_NAME LIKE " & freeWord & "  ) >= 1) ")
                        '                          種別=1:車窓マスタ （コード分類：64）
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 1 ")
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" & FixedCdYoyaku.CodeBunruiTypeCarWindowView & "' AND CODE_NAME LIKE " & freeWord & "  ) >= 1) ")
                        '                          種別=2:場所マスタ
                        sqlString.AppendLine(" OR   (CIB_KP.KIND ＝ 2 ")
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_PLACE WHERE CRS_JYOSYA_PLACE_FLG = 1 AND PLACE_NAME_1 LIKE " & freeWord & "  ) >= 1) ")
                        '   降車ヶ所（ホテル込み）.降車ヶ所枝番
                        '   降車ヶ所（ホテル込み）.備考
                        sqlString.AppendLine(" OR   (CIB_KP.BIKO LIKE " & freeWord & "  )")

                        '食事詳細
                        '   食事種別
                        If strWord = "朝食" Then
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" & MealKindType.breakfast & "' ) ")
                        ElseIf strWord = "昼食" Then
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" & MealKindType.lunch & "' ) ")
                        ElseIf strWord = "夕食" Then
                            sqlString.AppendLine(" OR   (CIB_MD.MEAL_KIND = '" & MealKindType.dinner & "' ) ")
                        End If
                        '   内容_バイキング
                        If strWord = "バイキング" Then
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_VAIKING = '1' ) ")
                        End If
                        '   内容_会席料理
                        If strWord = "会席料理" Or strWord = "会席" Then
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_KAISEKI = '1' ) ")
                        End If
                        '   内容_弁当
                        If strWord = "弁当" Then
                            sqlString.AppendLine(" OR   (CIB_MD.NAIYO_BENTO = '1' ) ")
                        End If
                        '   内容_その他
                        sqlString.AppendLine(" OR   (CIB_MD.NAIYO_OTHER LIKE " & freeWord & "  )")
                        '   内容_備考
                        sqlString.AppendLine(" OR   (CIB_MD.BIKO LIKE " & freeWord & "  )")

                        '宿泊
                        '   宿泊詳細.和室有無
                        If strWord = "和室有無" Or strWord = "和室" Then
                            sqlString.AppendLine(" OR   (CIB_HD.WASITU_FLG = 1 ) ")
                        End If
                        '   宿泊詳細.洋室有無
                        If strWord = "洋室有無" Or strWord = "洋室" Then
                            sqlString.AppendLine(" OR   (CIB_HD.YOSITU_FLG = 1 ) ")
                        End If
                        '   宿泊詳細.和洋室有無
                        If strWord = "和洋室有無" Or strWord = "和洋室" Then
                            sqlString.AppendLine(" OR   (CIB_HD.WAYOSITU_FLG = 1 ) ")
                        End If

                        '   宿泊詳細.設備１～６
                        sqlString.AppendLine(" OR   ((   CIB_HD.SETUBI_1 >= 1 OR CIB_HD.SETUBI_2 >= 1  OR CIB_HD.SETUBI_3 >= 1 ") '
                        sqlString.AppendLine("        OR CIB_HD.SETUBI_4 >= 1 OR CIB_HD.SETUBI_5 >= 1  OR CIB_HD.SETUBI_5 >= 1 ) ")
                        sqlString.AppendLine("       AND (SELECT COUNT(*) FROM M_CODE WHERE CODE_BUNRUI = '" & FixedCdYoyaku.CodeBunruiTypeHotelFacility & "' AND CODE_NAME LIKE " & freeWord & "  ) >= 1) ")
                        '   宿泊詳細.備考
                        sqlString.AppendLine(" OR   (CIB_HD.BIKO LIKE " & freeWord & "  )")

                        sqlString.AppendLine("     ) ")
                        idxForWord += 1
                    Next strWord
                End If
                '【乗り場指定時】						
                If Not String.IsNullOrEmpty(paramList.Item("NoribaCd").ToString.Trim) Then
                    sqlString.AppendLine("AND  XX.HAISYA_KEIYU_CD = '" & Trim(paramList.Item("NoribaCd").ToString.Trim) & "' ")    '乗り場
                End If

                '【バスタイプ指定時】						
                If Not String.IsNullOrEmpty(CType(paramList.Item("BusType"), String)) Then
                    sqlString.AppendLine("AND  CODEBUS.CODE_VALUE = '" & Trim(paramList.Item("BusType").ToString.Trim) & "' ")
                End If

                '【空席数指定時】						
                If Not String.IsNullOrEmpty(paramList.Item("KusekiNum").ToString.Trim) AndAlso IsNumeric(paramList.Item("KusekiNum").ToString.Trim) Then
                    sqlString.AppendLine("AND (  ")
                    sqlString.AppendLine("  CASE XX.JYOSYA_CAPACITY WHEN " & JYOSYA_CAPACITY_BUS)
                    sqlString.AppendLine("    THEN ")
                    sqlString.AppendLine("      (NVL(ZSK.KUSEKI_NUM_TEISEKI,0) + NVL(ZSK.KUSEKI_NUM_SUB_SEAT,0)) ")
                    sqlString.AppendLine("    ELSE ")
                    sqlString.AppendLine("      (NVL(XX.KUSEKI_NUM_TEISEKI,0) + NVL(XX.KUSEKI_NUM_SUB_SEAT,0)) ")
                    sqlString.AppendLine("  END) >= " & Convert.ToInt32(paramList.Item("KusekiNum")) & " ")         '空席数
                End If

                '【宿泊一人参加指定時】						
                'コース台帳（基本）.１名参加フラグ ≠ブランク						
                If Not String.IsNullOrEmpty(paramList.Item("Stay1NinSanka").ToString.Trim) AndAlso CType(paramList.Item("Stay1NinSanka"), Boolean) = True Then
                    sqlString.AppendLine("AND RTRIM(XX.ONE_SANKA_FLG) IS NOT NULL ")  ' '                                 '１名参加フラグ
                End If

                '【予約可能指定時】
                If Not String.IsNullOrEmpty(paramList.Item("YoyakuKanouOnly").ToString.Trim) AndAlso CType(paramList.Item("YoyakuKanouOnly"), Boolean) = True Then
                    Dim intStartDay As Integer = CInt(CommonDateUtil.getSystemTime().ToString("yyyyMMdd")) 'システム日付（YYYYMMDD）
                    sqlString.AppendLine("AND ( NVL(XX.TEJIMAI_KBN,' ') != '" & S02_0101.KbnValueY & "' ")                ' 手仕舞区分 ≠'Y'
                    sqlString.AppendLine("AND   NVL(XX.YOYAKU_STOP_FLG,' ') = ' ' ")                                                   '予約停止フラグ = ブランク
                    sqlString.AppendLine("AND   NVL(XX.UNKYU_KBN,' ') != '" & S02_0101.KbnValueY & "' ")   '運休区分 ≠ 'Y'
                    sqlString.AppendLine("AND   NVL(XX.SAIKOU_KAKUTEI_KBN,' ') != '" & S02_0101.KbnValueN & "' ")  '催行確定区分 ≠ 'N'
                    sqlString.AppendLine("AND   XX.SYUPT_DAY >='" & paramList.Item("SyuptDayFrom").ToString.Trim & "' ")                                     '出発日:開始'受付開始日 <= システム日付 
                    sqlString.AppendLine("AND   NVL(RTRIM(XX.UKETUKE_START_DAY),0) <= " & intStartDay & " ")                                   '受付開始日 <= システム日付
                    sqlString.AppendLine("AND (NVL(XX.KUSEKI_NUM_TEISEKI,0) + NVL(XX.KUSEKI_NUM_SUB_SEAT,0)) > 0 ")                               '空席数
                    sqlString.AppendLine("AND ( NVL(XX.CRS_KIND,' ') != '" & FixedCdYoyaku.CrsKind.kikakuStay & "'")  '{      コース種別 ≠'5'
                    sqlString.AppendLine(" OR ( NVL(XX.CRS_KIND,' ') = '" & FixedCdYoyaku.CrsKind.kikakuStay & "' ")  ' または コース種別 = '5' かつ
                    sqlString.AppendLine(" And ( RTRIM(XX.TEIINSEI_FLG) IS NOT NULL ")                              '    (  定員制フラグ ≠ ブランク または
                    sqlString.AppendLine("    OR NVL(XX.ROOM_ZANSU_SOKEI,0) <> 0 ))) ")                                      '       部屋残数総計 ≠ 0             )
                    sqlString.AppendLine(")")                                                                               '}
                End If
                If CBool(paramList.Item("CrsCdAssigne")) = True Then
                    'コースコードFull桁指定時、【日本語・外国語コース、コース区分指定】は無視
                    sqlString.AppendLine(" ")
                Else
                    '【日本語,外国語コース指定時】（日本語・外国語コース両方指定時、または両方未指定時は、抽出条件無し）
                    If CType(paramList.Item("JapaneseCrs"), Boolean) <> CType(paramList.Item("GaikokugoCrs"), Boolean) Then
                        '【日本語コースのみ指定時】
                        If Not String.IsNullOrEmpty(paramList.Item("JapaneseCrs").ToString.Trim) AndAlso CType(paramList.Item("JapaneseCrs"), Boolean) = True Then
                            sqlString.AppendLine("AND   NVL(XX.HOUJIN_GAIKYAKU_KBN,' ') =  '" & FixedCd.HoujinGaikyakuKbnType.Houjin & "' ")   '日本語外国語区分 = 日本語
                        End If
                        '【外国語コースのみ指定時】
                        If Not String.IsNullOrEmpty(paramList.Item("GaikokugoCrs").ToString.Trim) AndAlso CType(paramList.Item("GaikokugoCrs"), Boolean) = True Then
                            sqlString.AppendLine("AND   NVL(XX.HOUJIN_GAIKYAKU_KBN,' ')  =  '" & FixedCd.HoujinGaikyakuKbnType.Gaikyaku & "' ")  '日本語外国語区分 = 外国語
                        End If
                    End If
                    '【コース区分指定時】（全てのコース区分指定時、または全て未指定時は、抽出条件無し）
                    If (CType(paramList.Item("TeikiNoon"), Boolean) = True And CType(paramList.Item("TeikiNight"), Boolean) = True And CType(paramList.Item("TeikiKogai"), Boolean) = True And
                        CType(paramList.Item("KikakuDayTrip"), Boolean) = True And CType(paramList.Item("KikakuStay"), Boolean) = True And
                        CType(paramList.Item("KikakuTonaiR"), Boolean) = True And CType(paramList.Item("Capital"), Boolean) = True) Or
                       (CType(paramList.Item("TeikiNoon"), Boolean) = False And CType(paramList.Item("TeikiNight"), Boolean) = False And CType(paramList.Item("TeikiKogai"), Boolean) = False And
                        CType(paramList.Item("KikakuDayTrip"), Boolean) = False And CType(paramList.Item("KikakuStay"), Boolean) = False And
                        CType(paramList.Item("KikakuTonaiR"), Boolean) = False And CType(paramList.Item("Capital"), Boolean) = False) Then
                        sqlString.AppendLine(" ")
                    Else
                        sqlString.AppendLine("AND ( 1 = 2 ")
                        '【定期(昼) 指定時】						
                        '（　　　コース種別 = '1'						
                        'かつコース区分１ = '1'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("TeikiNoon").ToString.Trim) AndAlso CType(paramList.Item("TeikiNoon"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.teiki & "' ) ") 'コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_1,' ') =  '" & Common.FixedCd.CrsKbn1Type.noon & "' ")     'コース区分1 =昼
                            sqlString.AppendLine(" AND  NVL(XX.CRS_KBN_2,' ') =  '" & Common.FixedCd.crsKbn2Type.tonai & "' ) ")  'コース区分2 =都内
                        End If
                        '【定期(夜) 指定時】						
                        '（　　　コース種別 = '1'						
                        'かつコース区分１ = '2'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("TeikiNight").ToString.Trim) AndAlso CType(paramList.Item("TeikiNight"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.teiki & "' ) ") 'コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_1,' ') =  '" & Common.FixedCd.CrsKbn1Type.night & "' ")    'コース区分1 =夜
                            sqlString.AppendLine(" AND  NVL(XX.CRS_KBN_2,' ') =  '" & Common.FixedCd.crsKbn2Type.tonai & "' ) ")  'コース区分2 =都内
                        End If
                        '【定期(郊外) 指定時】						
                        '（　　　定期契約区分 = '1'						
                        'かつコース区分２ = '2'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("TeikiKogai").ToString.Trim) AndAlso CType(paramList.Item("TeikiKogai"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.teiki & "' ) ") 'コース種別 = はとバス定期
                            sqlString.AppendLine("AND ( NVL(XX.CRS_KBN_2,' ') =  '" & Common.FixedCd.crsKbn2Type.suburbs & "' ) ") 'コース区分2 =郊外
                        End If
                        '【企画(日帰り) 指定時】						
                        '（　　　コース種別 = '4'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("KikakuDayTrip").ToString.Trim) AndAlso CType(paramList.Item("KikakuDayTrip"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.kikakuHigaeri & "' ) ") 'コース種別 = 企画（日帰り）
                        End If
                        '【企画(宿泊) 指定時】						
                        '（　　　コース種別 = '5'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("KikakuStay").ToString.Trim) AndAlso CType(paramList.Item("KikakuStay"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.kikakuStay & "' ) ") 'コース種別 = 企画（宿泊）
                        End If
                        '【企画(都内R) コース 指定時】						
                        '（　　　コース種別 = '6'　　）						
                        If Not String.IsNullOrEmpty(paramList.Item("KikakuTonaiR").ToString.Trim) AndAlso CType(paramList.Item("KikakuTonaiR"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.kikakuTonaiR & "' ) ") 'コース種別 = 企画（都内R）
                        End If
                        '【キャピタル 指定時】						
                        If Not String.IsNullOrEmpty(paramList.Item("Capital").ToString.Trim) AndAlso CType(paramList.Item("Capital"), Boolean) = True Then
                            sqlString.AppendLine("OR ( NVL(XX.CRS_KIND,' ') =  '" & FixedCdYoyaku.CrsKind.capital & "' ) ") 'コース種別 = キャピタル
                        End If
                        sqlString.AppendLine(")")
                    End If
                End If
            End If

            'ORDER BY句
            sqlString.AppendLine("ORDER BY  ")
            sqlString.AppendLine("  XX.SYUPT_DAY, XX.CRS_CD, XX.SYUPT_TIME, XX.GOUSYA ")       '出発日、コースコード、出発時間（すべて昇順）

            Return sqlString.ToString

        End With
    End Function

    ''' <summary>
    ''' 重複チェック用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function GetCourseDataByPrimaryKey(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        With ClsCrsDaityoEntity

            sqlString.AppendLine("SELECT ")
            sqlString.AppendLine(" CRS.CRS_CD ")
            sqlString.AppendLine(",NVL(CRS.CRS_NAME_RK,CRS_NAME) AS CRS_NAME_RK ")
            sqlString.AppendLine(",CRS.TEIKI_KIKAKU_KBN ")

            'FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS ")

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     CRS.CRS_CD LIKE '" & paramList.Item("CRS_CD").ToString.Trim & "%'")

            Return sqlString.ToString

        End With

    End Function

    ''' <summary>
    ''' 重複チェック用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function GetCourseCdCheck(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        With ClsCrsDaityoEntity

            sqlString.AppendLine("SELECT ")
            sqlString.AppendLine(" CRS.CRS_CD ")
            sqlString.AppendLine(",CRS.GOUSYA ")

            'FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS ")

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     CRS.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
            Return sqlString.ToString

        End With

    End Function

    ''' <summary>
    ''' 車種マスタデータ取得（バスタイプコンボボックス設定用）
    ''' </summary>
    ''' <param name="nullRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCarKindMasterDataCodeData(Optional ByVal nullRecord As Boolean = False, Optional ByVal addWhere As String = "") As DataTable

        Dim resultDataTable As New DataTable
        Dim strSQL As String = ""

        Try

            '空レコード挿入要否に従い、空行挿入
            If nullRecord = True Then
                strSQL &= "SELECT ' ' AS CODE_VALUE,'' AS CODE_NAME FROM DUAL UNION "
            End If
            strSQL &= "SELECT RTRIM(CAR_CD) AS CODE_VALUE, CAR_NAME FROM M_CAR_KIND"
            strSQL &= " WHERE DELETE_DATE IS NULL"
            If Not addWhere.Equals(String.Empty) Then
                strSQL &= (" AND " & addWhere)
            End If
            strSQL &= " ORDER BY CODE_VALUE"

            resultDataTable = MyBase.getDataTable(strSQL)

        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable

    End Function

    ''' <summary>
    ''' 場所マスタデータ取得（バスタイプコンボボックス設定用）
    ''' </summary>
    ''' <param name="nullRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlaceMasterDataCodeData(Optional ByVal nullRecord As Boolean = False, Optional ByVal addWhere As String = "") As DataTable

        Dim resultDataTable As New DataTable
        Dim strSQL As String = ""

        Try

            '空レコード挿入要否に従い、空行挿入
            If nullRecord = True Then
                strSQL &= "SELECT ' ' AS CODE_VALUE,'' AS CODE_NAME FROM DUAL UNION "
            End If
            strSQL &= "SELECT RTRIM(PLACE_CD) AS CODE_VALUE, PLACE_NAME_1 FROM M_PLACE"
            strSQL &= " WHERE DELETE_DATE IS NULL"
            If Not addWhere.Equals(String.Empty) Then
                strSQL &= (" AND " & addWhere)
            End If
            strSQL &= " ORDER BY CODE_VALUE"

            resultDataTable = MyBase.getDataTable(strSQL)

        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable

    End Function

    ''' <summary>
    ''' 場所マスタデータ取得用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlaceMasterData(ByVal paramList As Hashtable) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        Try
            With ClsPlaceMasteroEntity
                sqlString.AppendLine("SELECT ")
                sqlString.AppendLine("  PLACE_CD,")
                sqlString.AppendLine("  PLACE_NAME_1,")
                sqlString.AppendLine(" PLACE_NAME_2,")
                sqlString.AppendLine(" PLACE_NAME_RK,")
                sqlString.AppendLine(" PLACE_NAME_SHORT,")
                sqlString.AppendLine(" CRS_JYOSYA_PLACE_FLG,")
                sqlString.AppendLine(" ROSEN_KEIYUTI_FLG,")
                sqlString.AppendLine(" CRS_SYUGO_PLACE_FLG,")
                sqlString.AppendLine(" CARRIER_SYUPT_TTYAK_PLACE,")
                sqlString.AppendLine(" COUPON_STAY_TIIKI,")
                sqlString.AppendLine(" COMPANY_CD,")
                sqlString.AppendLine(" EIGYOSYO_CD,")
                sqlString.AppendLine(" HAISYA_KEIYU_TI_SEQ,")
                sqlString.AppendLine(" HSSYA_ALR_HAISYA_TIKU_CD,")
                sqlString.AppendLine(" HSSYA_ALR_HAISYA_TI_CD,")
                sqlString.AppendLine(" T_WEB_PLACE_CD,")
                sqlString.AppendLine(" K_WEB_PLACE_CD ")
                'FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine("M_PLACE ")
                'WHERE句
                sqlString.AppendLine(" WHERE ")
                sqlString.AppendLine("     PLACE_CD = " & setParam("PLACE_CD", paramList.Item("PLACE_CD"), .PlaceCd.DBType, .PlaceCd.IntegerBu, .PlaceCd.DecimalBu))

                resultDataTable = MyBase.getDataTable(sqlString.ToString)

            End With

        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable

    End Function

    ''' <summary>
    ''' コース情報・追加情報取得
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function GetCrsControlInfo(ByVal paramList As Hashtable) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        Try
            'パラメータ設定
            With ClsCrsContorolInfoEntity
                'ＫＥＹ１
                setParam(.key1.PhysicsName, paramList.Item(.key1.PhysicsName), .key1.DBType, .key1.IntegerBu, .key1.DecimalBu)
                'ＫＥＹ２
                setParam(.key2.PhysicsName, paramList.Item(.key2.PhysicsName), .key2.DBType, .key2.IntegerBu, .key2.DecimalBu)
                sqlString.AppendLine("SELECT ")
                sqlString.AppendLine(" BIKO, ")
                sqlString.AppendLine(" DELETE_DAY, ")
                sqlString.AppendLine(" ENTRY_DAY, ")
                sqlString.AppendLine(" ENTRY_PERSON_CD, ")
                sqlString.AppendLine(" ENTRY_PGMID, ")
                sqlString.AppendLine(" ENTRY_TIME, ")
                sqlString.AppendLine(" FUZOKU_INFO_1, ")
                sqlString.AppendLine(" FUZOKU_INFO_2, ")
                sqlString.AppendLine(" FUZOKU_INFO_3, ")
                sqlString.AppendLine(" FUZOKU_INFO_4, ")
                sqlString.AppendLine(" KEY_1, ")
                sqlString.AppendLine(" KEY_2, ")
                sqlString.AppendLine(" KEY_3, ")
                sqlString.AppendLine(" UPDATE_DAY, ")
                sqlString.AppendLine(" UPDATE_PERSON_CD, ")
                sqlString.AppendLine(" UPDATE_PGMID, ")
                sqlString.AppendLine(" UPDATE_TIME, ")
                sqlString.AppendLine(" SYSTEM_ENTRY_PGMID, ")
                sqlString.AppendLine(" SYSTEM_ENTRY_PERSON_CD, ")
                sqlString.AppendLine(" SYSTEM_ENTRY_DAY, ")
                sqlString.AppendLine(" SYSTEM_UPDATE_PGMID, ")
                sqlString.AppendLine(" SYSTEM_UPDATE_PERSON_CD, ")
                sqlString.AppendLine(" SYSTEM_UPDATE_DAY ")
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" T_CRS_CONTROL_INFO ")
                'WHERE句
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine(" WHERE KEY_1 = :" & .key1.PhysicsName)
                    sqlString.AppendLine("   AND KEY_2 = :" & .key2.PhysicsName)
                End If
            End With

            resultDataTable = MyBase.getDataTable(sqlString.ToString())

        Catch ex As Oracle.ManagedDataAccess.Client.OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return resultDataTable
    End Function
#End Region

End Class
