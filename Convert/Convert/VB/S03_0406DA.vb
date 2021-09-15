Imports System.Text

''' <summary>
''' 催行決定/中止連絡のDAクラス
''' </summary>
Public Class S03_0406DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0

    ''' <summary>
    ''' 通知種別＝'1'（催行決定）
    ''' </summary>
    Private Const NoticeTypeSaikouDecided = "1"

    ''' <summary>
    ''' 通知種別＝'2'（催行中止）
    ''' </summary>
    Private Const NoticeTypeSaikouStop = "2"
#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' 検索処理を呼び出す（予約番号の入力がない場合）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableWithoutYoyakuNo(ByVal param As S03_0406DASelectParam) As DataTable

        '照会テーブルEntityを作成
        Dim crsLedgerBasic As New CrsLedgerBasicEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()
        sb.AppendLine("WITH  CALC_PLACE_NUM AS ( ")
        sb.AppendLine("    SELECT ")
        sb.AppendLine("        SYUPT_DAY ")
        sb.AppendLine("       ,CRS_CD ")
        sb.AppendLine("       ,COUNT(KOSHAKASHO_CD) AS KOSHAKASHO_NUM ")
        sb.AppendLine("    FROM ( ")
        sb.AppendLine("          SELECT DISTINCT ")
        sb.AppendLine("                  KOSHAKASHO.SYUPT_DAY        AS SYUPT_DAY ")
        sb.AppendLine("                , KOSHAKASHO.CRS_CD           AS CRS_CD ")
        sb.AppendLine("                , KOSHAKASHO.KOSHAKASHO_CD    AS KOSHAKASHO_CD ")
        sb.AppendLine("          FROM ")
        sb.AppendLine("              T_CRS_LEDGER_KOSHAKASHO KOSHAKASHO ")
        sb.AppendLine("          INNER JOIN ")
        sb.AppendLine("              ( ")
        sb.AppendLine("                  SELECT ")
        sb.AppendLine("                      SIIRE_SAKI_CD ")
        sb.AppendLine("                    , SIIRE_SAKI_NO ")
        sb.AppendLine("                    , SIIRE_SAKI_KIND_CD ")
        sb.AppendLine("                    , DELETE_DATE ")
        sb.AppendLine("                  FROM ")
        sb.AppendLine("                      M_SIIRE_SAKI ")
        sb.AppendLine("                 WHERE ")
        sb.AppendLine("                     DELETE_DATE IS NULL")
        sb.AppendLine("               ) SIIRE ")
        sb.AppendLine("          ON KOSHAKASHO.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("          AND KOSHAKASHO.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("    WHERE ")
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '")
        '50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '")
        '99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '")
        '35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ")
        sb.AppendLine("        AND DELETE_DAY IS NULL")
        sb.AppendLine("    UNION")
        sb.AppendLine("    SELECT DISTINCT ")
        sb.AppendLine("          HOTEL.SYUPT_DAY       AS SYUPT_DAY ")
        sb.AppendLine("        , HOTEL.CRS_CD          AS CRS_CD ")
        sb.AppendLine("        , HOTEL.SIIRE_SAKI_CD   AS KOSHAKASHO_CD ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("        T_CRS_LEDGER_HOTEL HOTEL ")
        sb.AppendLine("    INNER JOIN ")
        sb.AppendLine("    ( ")
        sb.AppendLine("       SELECT ")
        sb.AppendLine("             SIIRE_SAKI_CD ")
        sb.AppendLine("          , SIIRE_SAKI_NO ")
        sb.AppendLine("          , SIIRE_SAKI_KIND_CD ")
        sb.AppendLine("          , DELETE_DATE ")
        sb.AppendLine("      FROM ")
        sb.AppendLine("           M_SIIRE_SAKI ")
        sb.AppendLine("      WHERE ")
        sb.AppendLine("           DELETE_DATE IS NULL")
        sb.AppendLine("      ) SIIRE ")
        sb.AppendLine("        ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("        AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("    WHERE ")
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '")
        '50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '")
        '99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '")
        '35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ")
        sb.AppendLine("    AND DELETE_DAY IS NULL")
        sb.AppendLine("    ) ")
        sb.AppendLine("    GROUP BY ")
        sb.AppendLine("        SYUPT_DAY")
        sb.AppendLine("      , CRS_CD")
        sb.AppendLine(" )")


        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("      TO_YYYYMMDD_FC(BASIC.SYUPT_DAY)                                     AS SYUPT_DAY_STR ")     '出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY                                                     AS SYUPT_DAY ")         '出発日
        sb.AppendLine("    , BASIC.CRS_CD                                                        AS CRS_CD ")            'コースコード
        sb.AppendLine("    , MAX(BASIC.CRS_NAME)                                                 AS CRS_NAME ")          'コース名
        sb.AppendLine("    , SUM(BASIC.YOYAKU_NUM_TEISEKI + BASIC.YOYAKU_NUM_SUB_SEAT)           AS YOYAKU_NUM ")        '予約数定席 + 予約数補助席
        sb.AppendLine("    , MAX(CALC_PLACE_NUM.KOSHAKASHO_NUM)                                  AS GET_OUT_PLACE_NUM ") '降車ヶ所コード
        sb.AppendLine("    , TO_YYYYMMDD_FC(MAX(BASIC.SAIKOU_DAY))                               AS SAIKOU_DAY ")        '催行日
        sb.AppendLine("    , MAX(BASIC.BUS_RESERVE_CD)                                           AS BUS_RESERVE_CD  ")    'バス指定コード
        sb.AppendLine("    , COUNT(BASIC.GOUSYA)                                                 AS BUS_NUM  ")           '号車

        'FROM句
        sb.AppendLine("FROM ")

        'コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC ")

        sb.AppendLine("    INNER JOIN  CALC_PLACE_NUM ")
        '結合条件
        sb.AppendLine("    ON  BASIC.SYUPT_DAY = CALC_PLACE_NUM.SYUPT_DAY ")
        sb.AppendLine("    AND BASIC.CRS_CD = CALC_PLACE_NUM.CRS_CD ")

        'WHERE句
        sb.AppendLine("WHERE 1 = 1 ")

        '定期・企画区分 =　2
        sb.AppendLine(" AND BASIC.TEIKI_KIKAKU_KBN = '2' ")
        '○増管理区分 ≠M
        sb.AppendLine(" AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN, '*') <> 'M' ")
        '削除日 = 0
        sb.AppendLine(" AND NVL(BASIC.DELETE_DAY, 0) = 0 ")

        '催行可否登録日FROM
        If Not param.SaikouEntryDayFrom Is Nothing Then
            sb.Append("    AND BASIC.SAIKOU_DAY >= ").AppendLine(setSelectParam(param.SaikouEntryDayFrom, crsLedgerBasic.saikouDay))
        End If
        '催行可否登録日TO
        If Not param.SaikouEntryDayTo Is Nothing Then
            sb.Append("    AND BASIC.SAIKOU_DAY <= ").AppendLine(setSelectParam(param.SaikouEntryDayTo, crsLedgerBasic.saikouDay))
        End If

        '通知種別が１の場合
        If String.Compare(param.NoticeKind, NoticeTypeSaikouDecided) = 0 Then
            ''Y'：催行決定
            sb.Append("    AND BASIC.SAIKOU_KAKUTEI_KBN = '").Append(FixedCd.SaikouKakuteiKbn.Saikou).AppendLine("' --'Y'：催行決定")
        End If

        '通知種別が２の場合
        If String.Compare(param.NoticeKind, NoticeTypeSaikouStop) = 0 Then
            ''N'：催行中止
            sb.Append("    AND BASIC.SAIKOU_KAKUTEI_KBN = '").Append(FixedCd.SaikouKakuteiKbn.Tyushi).AppendLine("' --'N'：催行中止")
        End If

        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.Append("    AND BASIC.CRS_CD = ").AppendLine(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd))
        End If

        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.Append("    AND BASIC.SYUPT_DAY >= ").AppendLine(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay))
        End If
        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.Append("    AND BASIC.SYUPT_DAY <= ").AppendLine(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay))
        End If

        '邦人／外客区分
        If param.CrsJapanese = True OrElse param.CrsForeign = True Then
            sb.AppendLine("    AND (1 <> 1 ")
            If param.CrsJapanese = True Then
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = '").Append(FixedCd.HoujinGaikyakuKbnType.Houjin).AppendLine("' --'H'：邦人")
            End If
            If param.CrsForeign = True Then
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = '").Append(FixedCd.HoujinGaikyakuKbnType.Gaikyaku).AppendLine("' --'G'：外客")
            End If
            sb.AppendLine("        )")
        End If

        'コース種別
        If param.CrsDay = True OrElse param.CrsStay = True OrElse param.CrsRcourse = True Then
            sb.AppendLine("    AND (1 <> 1 ")
            '企画（日帰り）
            If param.CrsDay = True Then
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Higaeri).AppendLine("' --'4'：企画日帰り")
            End If
            '企画（宿泊）
            If param.CrsStay = True Then
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Stay).AppendLine("' --'5'：企画宿泊")
            End If
            '企画（Ｒコース）
            If param.CrsRcourse = True Then
                sb.Append("         OR BASIC.CRS_KIND = '").Append(FixedCd.CrsKbnType.Kikaku).AppendLine("' --'6'：Rコース")
            End If
            sb.AppendLine("        )")
        End If

        'バス指定コード
        If Not String.IsNullOrEmpty(param.BusReserveCd) Then
            sb.Append("    AND BASIC.BUS_RESERVE_CD = ").AppendLine(setSelectParam(param.BusReserveCd, crsLedgerBasic.busReserveCd))
        End If
        'ソート条件
        sb.AppendLine("GROUP BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")
        'ソート条件
        sb.AppendLine("ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")

        Return MyBase.getDataTable(sb.ToString)

    End Function

    ''' <summary>
    ''' 検索処理を呼び出す（予約番号の入力がある場合）
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableWithYoyakuNo(ByVal param As S03_0406DASelectParam) As DataTable

        '照会テーブルEntityを作成
        Dim yoyakuInfoBasic As New YoyakuInfoBasicEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        sb.AppendLine("WITH  CALC_PLACE_NUM AS ( ")
        sb.AppendLine("    SELECT ")
        sb.AppendLine("        SYUPT_DAY ")
        sb.AppendLine("       ,CRS_CD ")
        sb.AppendLine("       ,COUNT(KOSHAKASHO_CD) AS KOSHAKASHO_NUM ")
        sb.AppendLine("    FROM ( ")
        sb.AppendLine("          SELECT DISTINCT ")
        sb.AppendLine("                  KOSHAKASHO.SYUPT_DAY        AS SYUPT_DAY ")
        sb.AppendLine("                , KOSHAKASHO.CRS_CD           AS CRS_CD ")
        sb.AppendLine("                , KOSHAKASHO.KOSHAKASHO_CD    AS KOSHAKASHO_CD ")
        sb.AppendLine("          FROM ")
        sb.AppendLine("              T_CRS_LEDGER_KOSHAKASHO KOSHAKASHO ")
        sb.AppendLine("          INNER JOIN ")
        sb.AppendLine("              ( ")
        sb.AppendLine("                  SELECT ")
        sb.AppendLine("                      SIIRE_SAKI_CD ")
        sb.AppendLine("                    , SIIRE_SAKI_NO ")
        sb.AppendLine("                    , SIIRE_SAKI_KIND_CD ")
        sb.AppendLine("                    , DELETE_DATE ")
        sb.AppendLine("                  FROM ")
        sb.AppendLine("                      M_SIIRE_SAKI ")
        sb.AppendLine("                 WHERE ")
        sb.AppendLine("                     DELETE_DATE IS NULL")
        sb.AppendLine("               ) SIIRE ")
        sb.AppendLine("          ON KOSHAKASHO.KOSHAKASHO_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("          AND KOSHAKASHO.KOSHAKASHO_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("    WHERE ")
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '")
        '50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '")
        '99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '")
        '35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ")
        sb.AppendLine("        AND DELETE_DAY IS NULL")
        sb.AppendLine("    UNION")
        sb.AppendLine("    SELECT DISTINCT ")
        sb.AppendLine("          HOTEL.SYUPT_DAY       AS SYUPT_DAY ")
        sb.AppendLine("        , HOTEL.CRS_CD          AS CRS_CD ")
        sb.AppendLine("        , HOTEL.SIIRE_SAKI_CD   AS KOSHAKASHO_CD ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("        T_CRS_LEDGER_HOTEL HOTEL ")
        sb.AppendLine("    INNER JOIN ")
        sb.AppendLine("    ( ")
        sb.AppendLine("       SELECT ")
        sb.AppendLine("             SIIRE_SAKI_CD ")
        sb.AppendLine("          , SIIRE_SAKI_NO ")
        sb.AppendLine("          , SIIRE_SAKI_KIND_CD ")
        sb.AppendLine("          , DELETE_DATE ")
        sb.AppendLine("      FROM ")
        sb.AppendLine("           M_SIIRE_SAKI ")
        sb.AppendLine("      WHERE ")
        sb.AppendLine("           DELETE_DATE IS NULL")
        sb.AppendLine("      ) SIIRE ")
        sb.AppendLine("        ON HOTEL.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("        AND HOTEL.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("    WHERE ")
        sb.Append("        SIIRE.SIIRE_SAKI_KIND_CD NOT IN ( '")
        '50（高速・有料道路）
        sb.Append(FixedCd.SuppliersKind_Kosoku_TollRoad).Append("', '")
        '99（その他確定経費）
        sb.Append(FixedCd.SuppliersKind_SonotaKakuteiExpense).Append("', '")
        '35（ホテル（クーポン））
        sb.Append(FixedCd.SuppliersKind_Hotel_Coupon).AppendLine("' ) --高速・有料道路、その他確定経費、ホテル（クーポン）以外 ")
        sb.AppendLine("    AND DELETE_DAY IS NULL")
        sb.AppendLine("    ) ")
        sb.AppendLine("    GROUP BY ")
        sb.AppendLine("        SYUPT_DAY")
        sb.AppendLine("      , CRS_CD")
        sb.AppendLine(" )")

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("      TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ")      '出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY                 AS SYUPT_DAY ")          '出発日
        sb.AppendLine("    , BASIC.CRS_CD                    AS CRS_CD ")             'コースコード
        sb.AppendLine("    , MAX(BASIC.CRS_NAME)                  AS CRS_NAME ")           'コース名
        sb.AppendLine("    , SUM(BASIC.YOYAKU_NUM_TEISEKI + BASIC.YOYAKU_NUM_SUB_SEAT)          AS YOYAKU_NUM ")        '予約数定席 + 予約数補助席
        sb.AppendLine("    , MAX(CALC_PLACE_NUM.KOSHAKASHO_NUM)    AS GET_OUT_PLACE_NUM ")
        sb.AppendLine("    , TO_YYYYMMDD_FC(MAX(BASIC.SAIKOU_DAY))                AS SAIKOU_DAY ")         '催行日
        sb.AppendLine("    , MAX(BASIC.BUS_RESERVE_CD)            AS BUS_RESERVE_CD  ")     'バス指定コード
        sb.AppendLine("    , COUNT(BASIC.GOUSYA)              AS BUS_NUM  ")            '号車
        sb.AppendLine("    , MAX(BASIC.SYUPT_TIME_1)              AS SYUPT_TIME ")         '出発時間1
        sb.AppendLine("    , MAX(BASIC.SAIKOU_KAKUTEI_KBN)        AS SAIKOU_KAKUTEI_KBN ") '催行確定区分
        sb.AppendLine("    , MAX(BASIC.TEIKI_KIKAKU_KBN)          AS TEIKI_KIKAKU_KBN ")   '定期/企画区分
        sb.AppendLine("    , MAX(YOYAKU.CANCEL_FLG)               AS CANCEL_FLG ")         'キャンセルフラグ

        'FROM句
        sb.AppendLine("FROM ")

        '予約情報(基本) YOYAKU
        sb.AppendLine("    T_YOYAKU_INFO_BASIC YOYAKU")
        '内部結合
        sb.AppendLine("INNER JOIN  ")
        'コース台帳(基本) BASIC
        sb.AppendLine("    T_CRS_LEDGER_BASIC BASIC ")
        '結合条件
        sb.AppendLine("    ON  YOYAKU.SYUPT_DAY = BASIC.SYUPT_DAY ")
        sb.AppendLine("    AND YOYAKU.CRS_CD = BASIC.CRS_CD ")
        '内部結合
        sb.AppendLine("INNER JOIN ")
        sb.AppendLine("    CALC_PLACE_NUM ")
        '結合条件
        sb.AppendLine("    ON  BASIC.SYUPT_DAY = CALC_PLACE_NUM.SYUPT_DAY ")
        sb.AppendLine("    AND BASIC.CRS_CD = CALC_PLACE_NUM.CRS_CD ")

        'WHERE句
        sb.AppendLine("WHERE 1 = 1 ")
        sb.AppendLine(" AND NVL(BASIC.DELETE_DAY, 0) = 0 ")
        If Not String.IsNullOrEmpty(param.YoyakuKbn) Then
            '予約区分
            sb.Append("    AND YOYAKU.YOYAKU_KBN = ").AppendLine(setSelectParam(param.YoyakuKbn, yoyakuInfoBasic.yoyakuKbn))
            '予約ＮＯ
            sb.Append("    AND YOYAKU.YOYAKU_NO = ").AppendLine(setSelectParam(param.YoyakuNo, yoyakuInfoBasic.yoyakuNo))
        End If

        '集計条件
        sb.AppendLine("    GROUP BY ")
        sb.AppendLine("          BASIC.SYUPT_DAY ")
        sb.AppendLine("        , BASIC.CRS_CD ")

        'ソート条件
        sb.AppendLine("ORDER BY ")
        sb.AppendLine("      BASIC.SYUPT_DAY ")
        sb.AppendLine("    , BASIC.CRS_CD ")

        Return MyBase.getDataTable(sb.ToString)

    End Function

    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
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
#End Region

#Region " パラメータ "
    Public Class S03_0406DASelectParam
        ''' <summary>
        ''' 催行可否登録日FROM
        ''' </summary>
        Public Property SaikouEntryDayFrom As Integer?
        ''' <summary>
        ''' 催行可否登録日TO
        ''' </summary>
        Public Property SaikouEntryDayTo As Integer?
        ''' <summary>
        ''' 通知種別
        ''' </summary>
        Public Property NoticeKind As String
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 出発日FROM
        ''' </summary>
        Public Property SyuptDayFrom As Integer?
        ''' <summary>
        ''' 出発日TO
        ''' </summary>
        Public Property SyuptDayTo As Integer?
        ''' <summary>
        ''' 日本語
        ''' </summary>
        Public Property CrsJapanese As Boolean
        ''' <summary>
        ''' 外国語
        ''' </summary>
        Public Property CrsForeign As Boolean
        ''' <summary>
        ''' 企画（日帰り）
        ''' </summary>
        Public Property CrsDay As Boolean
        ''' <summary>
        ''' 企画（宿泊）
        ''' </summary>
        Public Property CrsStay As Boolean
        ''' <summary>
        ''' 企画（Ｒコース）
        ''' </summary>
        Public Property CrsRcourse As Boolean
        ''' <summary>
        ''' バス指定コード
        ''' </summary>
        Public Property BusReserveCd As String
        ''' <summary>
        ''' 予約区分
        ''' </summary>
        Public Property YoyakuKbn As String
        ''' <summary>
        ''' 予約ＮＯ
        ''' </summary>
        Public Property YoyakuNo As Integer
    End Class
#End Region
End Class
