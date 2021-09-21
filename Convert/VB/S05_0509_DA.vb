Imports System.Text

''' <summary>
''' 窓口日報出力のDAクラス
''' </summary>
Public Class S05_0509_DA
    Inherits DataAccessorBase

#Region "SELECT処理"

    ''' <summary>
    ''' 1検索条件に一致するデータを取得
    ''' ユーザIDを指定し、検索を行った場合
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),ユーザID,邦人/外客区分</param>
    ''' <returns></returns>
    Public Function getSearchData(param As Hashtable) As DataTable

        Dim sqlString As StringBuilder = New StringBuilder
        sqlString.AppendLine(" SELECT DISTINCT  ")
        sqlString.AppendLine("   T_SEISAN_INFO.ENTRY_PERSON_CD,  ")
        sqlString.AppendLine("   M_USER.USER_NAME,  ")
        sqlString.AppendLine("   TO_CHAR(TO_DATE(LPAD(T_SEISAN_INFO.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') AS SIGNON_TIME ")

        sqlString.AppendLine(" FROM T_SEISAN_INFO ")
        sqlString.AppendLine(" INNER JOIN M_USER ")
        sqlString.AppendLine("    ON M_USER.COMPANY_CD = '0001' ")
        sqlString.AppendLine("   AND M_USER.USER_ID    = T_SEISAN_INFO.ENTRY_PERSON_CD ")

        sqlString.AppendLine(" WHERE T_SEISAN_INFO.COMPANY_CD = '00' ")
        sqlString.AppendLine("   AND T_SEISAN_INFO.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("   AND T_SEISAN_INFO.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND T_SEISAN_INFO.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND T_SEISAN_INFO.DELETE_DAY = 0  ")
        sqlString.AppendLine("   AND NVL(T_SEISAN_INFO.URIAGE_KBN, ' ') <> 'V'  ")
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("   AND T_SEISAN_INFO.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If

        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine("   TO_CHAR(TO_DATE(LPAD(T_SEISAN_INFO.SIGNON_TIME, 6,'0'),'HH24MISS'), 'HH24:MI:SS') ")

        'sql実行
        Dim result As New DataTable
        Try
            result = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, "S05_0401", ex.Message)
            createFactoryMsg.messageDisp("E90_046", ex.Message)
        End Try
        'リターン
        Return result
    End Function
#End Region

#Region "共通"
    ''' <summary>
    ''' 1検索条件に一致するデータを取得
    ''' ユーザIDを指定し、検索を行った場合
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),ユーザID,邦人/外客区分</param>
    ''' <returns></returns>
    Public Function getEigyosyoName(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder
        sqlString.AppendLine("  SELECT EM.EIGYOSYO_NAME_PRINT_1 ")
        sqlString.AppendLine("  FROM M_EIGYOSYO EM ")
        sqlString.AppendLine("  WHERE EM.COMPANY_CD = '00' ")
        sqlString.AppendLine("    AND EM.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))

        Dim result As New DataTable
        Try
            result = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, "S05_0401", ex.Message)
            createFactoryMsg.messageDisp("E90_046", ex.Message)
        End Try
        'リターン
        Return result
    End Function
#End Region

#Region "窓口日報 P05_0501"

#Region "窓口日報"
    ''' <summary>
    ''' 窓口日報
    ''' 2-3プレビューOR印刷(ユーザIDの指定なしorあり　かつ　明細貢＝OFF)
    ''' 精算情報内訳より
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),出力対象月日(To),ユーザID,邦人/外客区分,サインオン時間</param>
    ''' <returns></returns>
    Public Function getPrintOffUtiwake(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder
        '借方
        sqlString.AppendLine(" SELECT  ")
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '110' THEN U.KINGAKU ELSE 0 END) Genkin ,  ") '現金
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '120' THEN U.KINGAKU ELSE 0 END) HurikomiEtcEigyosyo ,  ") '振込（営業所）
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '130' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenter ,  ") '予約センター入金
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '130' AND U.HURIKOMI_KBN = 'G' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterBankKei ,  ") '予約センター入金(銀行)計
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '130' AND U.HURIKOMI_KBN = 'Y' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterYubinKyokuKei ,  ") '予約センター入金(郵便局)計
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '130' AND U.HURIKOMI_KBN = 'K' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterConveniKei ,  ") '予約センター入金(コンビニ)計
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '140' THEN U.KINGAKU ELSE 0 END) SensyaKen ,  ") '船車券
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '150' THEN U.KINGAKU ELSE 0 END) BC ,  ") 'B/C
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '160' THEN U.KINGAKU ELSE 0 END) CreditCard ,  ") 'クレジットカード
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '170' THEN U.KINGAKU ELSE 0 END) OnlineCredit ,  ") 'ノーサインクレジット
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '830' THEN U.KINGAKU ELSE 0 END) WaribikiKingaku ,  ") '割引
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '180' THEN U.KINGAKU ELSE 0 END) Sonota ,  ") 'その他
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '190' THEN U.KINGAKU ELSE 0 END) OpenKen ,  ") 'オープン券
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '840' THEN U.KINGAKU ELSE 0 END) ToriatukaiFee ,  ") '取扱手数料
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '200' THEN U.KINGAKU ELSE 0 END) KariukekinSeisan ,  ") '仮受金精算
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '210' THEN U.KINGAKU ELSE 0 END) MaeukekinSeisan ,  ") '前受金精算
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '220' THEN U.KINGAKU ELSE 0 END) Misyu ,  ") '未収
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '230' THEN U.KINGAKU ELSE 0 END) Karibaraikin ,  ") '仮払金
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '240' THEN U.KINGAKU ELSE 0 END) MisyuSubKen ,  ") '未収(補助券)
        '貸方
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '811' THEN U.KINGAKU ELSE 0 END) CancelRyou ,  ") 'キャンセル料
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '821' THEN U.KINGAKU ELSE 0 END) ModosiFee ,  ") '払戻手数料
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '111' THEN U.KINGAKU ELSE 0 END) GenkinModosi ,  ") '現金払戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '121' THEN U.KINGAKU ELSE 0 END) HurikomiEtcEigyosyoReturn ,  ") '振込等営業所戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '131' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterReturn ,  ") '振込等予約センター戻
        'sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '131' AND U.HURIKOMI_KBN = 'G' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterReturnBank ,  ") '予約センター入金(銀行)計
        'sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '131' AND U.HURIKOMI_KBN = 'Y' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterReturnYubinKyoku ,  ") '予約センター入金(郵便局)計
        'sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '131' AND U.HURIKOMI_KBN = 'K' THEN U.KINGAKU ELSE 0 END) HurikomiEtcYoyakuCenterReturnConveni ,  ") '予約センター入金(コンビニ)計
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '141' THEN U.KINGAKU ELSE 0 END) SensyaKenReturn ,  ") '船車券戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '151' THEN U.KINGAKU ELSE 0 END) BCReturn ,  ") 'Ｂ／Ｃ戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '161' THEN U.KINGAKU ELSE 0 END) CreditCardModosi ,  ") 'クレジットカード払戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '171' THEN U.KINGAKU ELSE 0 END) OnlineCreditReturn ,  ") 'オンラインクレジット戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '831' THEN U.KINGAKU ELSE 0 END) WaribikiKingakuReturn ,  ") '割引金額戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '181' THEN U.KINGAKU ELSE 0 END) SonotaReturn ,  ") 'その他戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '841' THEN U.KINGAKU ELSE 0 END) ToriatukaiFeeReturn ,  ") '取扱手数料戻
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '201' THEN U.KINGAKU ELSE 0 END) Kariukekin ,  ") '仮受金
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '211' THEN U.KINGAKU ELSE 0 END) Maeukekin ,  ") '前受金
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '221' THEN U.KINGAKU ELSE 0 END) MisyuRecoveryKei ,  ") '未収回収
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '231' THEN U.KINGAKU ELSE 0 END) KaribaraikinSeisan ,  ") '仮払金精算
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '241' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryTokyo ,  ") '未収(補助券)回収(東京)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '242' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryHotel ,  ") '未収(補助券)回収(ホテル)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '243' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryHamamatsucho ,  ") '未収(補助券)回収(浜松町)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '244' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryShinjuku ,  ") '未収(補助券)回収(新宿)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '245' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryIkebukuro ,  ") '未収(補助券)回収(池袋)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '246' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryTotyo ,  ") '未収(補助券)回収(都庁)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '247' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoverySalesKi ,  ") '未収(補助券)回収(営業企)
        sqlString.AppendLine(" 　SUM(CASE WHEN U.SEISAN_KOUMOKU_CD = '248' THEN U.KINGAKU ELSE 0 END) MisyuSubKenRecoveryYoyakuC  ") '未収(補助券)回収(予約C)

        sqlString.AppendLine(" FROM T_SEISAN_INFO SI ")
        sqlString.AppendLine(" INNER JOIN T_SEISAN_INFO_UTIWAKE U ")
        sqlString.AppendLine("    ON SI.SEISAN_INFO_SEQ = U.SEISAN_INFO_SEQ ")
        sqlString.AppendLine(" LEFT JOIN T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("    ON SI.CRS_CD    = T_CRS_LEDGER_BASIC.CRS_CD ")
        sqlString.AppendLine("   AND SI.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
        sqlString.AppendLine("   AND SI.GOUSYA    = T_CRS_LEDGER_BASIC.GOUSYA ")

        sqlString.AppendLine(" WHERE SI.COMPANY_CD = '00' ")
        sqlString.AppendLine("   AND SI.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("   AND SI.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.DELETE_DAY = 0  ")
        sqlString.AppendLine("   AND NVL(SI.URIAGE_KBN, ' ') <> 'V'  ")
        If Not String.IsNullOrWhiteSpace(CType(param("CrsKbn"), String)) Then
            sqlString.AppendLine("   AND (T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IS NULL ")
            sqlString.AppendLine("    OR  T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_GAIKYAKU_KBN", param.Item("CrsKbn"), OracleDbType.Char, 1) & ") ")
        End If
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("   AND SI.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If
        Dim checkoutItemSignonTime As List(Of String) = CType(param("SignonTime"), List(Of String))
        If checkoutItemSignonTime.Count > 0 Then
            sqlString.AppendLine("   AND SI.SIGNON_TIME IN (")
            For i As Integer = 0 To checkoutItemSignonTime.Count - 1
                If i > 0 Then
                    sqlString.AppendLine(",")
                End If
                sqlString.AppendLine(setParam("SIGNON_TIME_" & i, checkoutItemSignonTime(i), OracleDbType.Int32, 6))
            Next
            sqlString.AppendLine(") ")
        End If

        'sql実行
        Dim result As DataTable = MyBase.getDataTable(sqlString.ToString)
        'リターン
        Return result
    End Function

    ''' <summary>
    ''' 窓口日報
    ''' 2-3プレビューOR印刷(ユーザIDの指定なしorあり　かつ　明細貢＝OFF)
    ''' 精算情報より
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),出力対象月日(To),ユーザID,邦人/外客区分,サインオン時間</param>
    ''' <returns></returns>
    Public Function getPrintOffSeisanInfo(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder

        sqlString.AppendLine(" SELECT  ")
        sqlString.AppendLine("   SUM(COUPON_URIAGE) CouponUriage, ") 'クーポン売上
        sqlString.AppendLine("   SUM(COUPON_REFUND) CouponModosi, ") 'クーポン払戻
        sqlString.AppendLine("   SUM(OTHER_URIAGE_SYOHIN_URIAGE) SonotaSyohinUriage, ") 'その他商品売上
        sqlString.AppendLine("   SUM(OTHER_URIAGE_SYOHIN_MODOSI) SonotaSyohinModosi  ") 'その他商品払戻

        sqlString.AppendLine(" FROM T_SEISAN_INFO SI ")
        sqlString.AppendLine(" LEFT JOIN T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("    ON SI.CRS_CD    = T_CRS_LEDGER_BASIC.CRS_CD ")
        sqlString.AppendLine("   AND SI.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
        sqlString.AppendLine("   AND SI.GOUSYA    = T_CRS_LEDGER_BASIC.GOUSYA ")

        sqlString.AppendLine(" WHERE SI.COMPANY_CD = '00' ")
        sqlString.AppendLine("   AND SI.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("   AND SI.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.DELETE_DAY = 0  ")
        sqlString.AppendLine("   AND NVL(SI.URIAGE_KBN, ' ') <> 'V'  ")
        If Not String.IsNullOrWhiteSpace(CType(param("CrsKbn"), String)) Then
            sqlString.AppendLine("   AND (T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IS NULL ")
            sqlString.AppendLine("    OR  T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_GAIKYAKU_KBN", param.Item("CrsKbn"), OracleDbType.Char, 1) & ") ")
        End If
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("   AND SI.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If
        Dim checkoutItemSignonTime As List(Of String) = CType(param("SignonTime"), List(Of String))
        If checkoutItemSignonTime.Count > 0 Then
            sqlString.AppendLine("   AND SI.SIGNON_TIME IN (")
            For i As Integer = 0 To checkoutItemSignonTime.Count - 1
                If i > 0 Then
                    sqlString.AppendLine(",")
                End If
                sqlString.AppendLine(setParam("SIGNON_TIME_" & i, checkoutItemSignonTime(i), OracleDbType.Int32, 6))
            Next
            sqlString.AppendLine(") ")
        End If

        'sql実行
        Dim result As DataTable = MyBase.getDataTable(sqlString.ToString)
        'リターン
        Return result
    End Function
#End Region

#Region "窓口日報(内訳)"
    ''' <summary>
    ''' 窓口日報
    ''' 4-5プレビューOR印刷(ユーザIDの指定なしorあり　かつ　明細貢＝ON)
    ''' 詳細頁を出力する = ONの場合
    ''' 精算情報内訳より
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),ユーザID,邦人/外客区分</param>
    ''' <returns></returns>
    Public Function getPrintONUtiwake(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder
        '借方
        sqlString.AppendLine(" SELECT   ")
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenDayRegularNoon ,  ") '船車券（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenDayRegularNight ,  ")　'船車券（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenDayKiR ,  ")　'船車券（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenDayKi ,  ")　'船車券（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenOutsideRegularNoon ,  ") '船車券（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenOutsideRegularNight ,  ")　'船車券（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenOutsideKiR ,  ")　'船車券（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenOutsideKi ,  ")　'船車券（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD IN ('140','141','150','151') AND SI.SEISAN_KBN = '40'                                                                         THEN U.KINGAKU * (CASE WHEN U.SEISAN_KOUMOKU_CD IN ('141','151') THEN -1 ELSE 1 END) ELSE 0 END ) SensyaKenOther ,  ") '船車券（他）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '190' THEN U.KINGAKU ELSE 0 END ) OpenKen ,  ")　'オープン券
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNoon ,  ") '割引（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNoonSyotai ,  ") '割引（日定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNoonFamily ,  ") '割引（日定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNight ,  ") '割引（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNightSyotai ,  ") '割引（日定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayRegularNightFamily ,  ") '割引（日定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiDayKiR ,  ") '割引（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayKiRSyotai ,  ") '割引（日企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayKiRFamily ,  ") '割引（日企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiDayKi ,  ") '割引（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayKiSyotai ,  ") '割引（日企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiDayKiFamily ,  ") '割引（日企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNoon ,  ") '割引（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNoonSyotai ,  ") '割引（外定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNoonFamily ,  ") '割引（外定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNight ,  ") '割引（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNightSyotai ,  ") '割引（外定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideRegularNightFamily ,  ") '割引（外定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKiR ,  ") '割引（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKiRSyotai ,  ") '割引（外企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKiRFamily ,  ") '割引（外企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKi ,  ") '割引（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKiSyotai ,  ") '割引（外企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiOutsideKiFamily ,  ") '割引（外企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiCapital ,  ") '割引（キャピタル）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiCapitalSyotai ,  ") '割引（キャピタル）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '830' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiCapitalFamily ,  ") '割引（キャピタル）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeDayRegularNoon ,  ") '取扱手数料（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeDayRegularNight ,  ") '取扱手数料（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeDayKiR ,  ") '取扱手数料（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeDayKi ,  ") '取扱手数料（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeOutsideRegularNoon ,  ") '取扱手数料（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeOutsideRegularNight ,  ") '取扱手数料（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeOutsideKiR ,  ") '取扱手数料（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeOutsideKi ,  ") '取扱手数料（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '13'                                                                         THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeCapital ,  ") '取扱手数料（キャピタル）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '840' AND SI.SEISAN_KBN = '40'                                                                         THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeOther ,  ") '取扱手数料（他）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '160'                                                 AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) CreditCardJapanese ,  ") 'クレジットカード（日本語）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '160'                                                 AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) CreditCardGaikokugo ,  ") 'クレジットカード（外国語）

        '貸方
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '821' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ModosiFeeDayRegularNoon ,  ") '払戻手数料（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '821' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ModosiFeeDayRegularNight ,  ") '払戻手数料（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '811' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) CancelRyouDayKiR ,  ") 'キャンセル料（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '811' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) CancelRyouDayKi ,  ") 'キャンセル料（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '821' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ModosiFeeOutsideRegularNoon ,  ") '払戻手数料（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '821' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ModosiFeeOutsideRegularNight ,  ") '払戻手数料（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '811' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) CancelRyouOutsideKiR ,  ") 'キャンセル料（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '811' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) CancelRyouOutsideKi ,  ") 'キャンセル料（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '821' AND SI.SEISAN_KBN = '13'                                                                         THEN U.KINGAKU ELSE 0 END ) ModosiFeeCapital ,  ") '払戻手数料（キャピタル）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNoon ,  ") '割引戻（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNoonSyotai ,  ") '割引戻（日定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNoonFamily ,  ") '割引戻（日定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNight ,  ") '割引戻（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNightSyotai ,  ") '割引戻（日定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayRegularNightFamily ,  ") '割引戻（日定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKiR ,  ") '割引戻（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKiRSyotai ,  ") '割引戻（日企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKiRFamily ,  ") '割引戻（日企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKi ,  ") '割引戻（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKiSyotai ,  ") '割引戻（日企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnDayKiFamily ,  ") '割引戻（日企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNoon ,  ") '割引戻（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNoonSyotai ,  ") '割引戻（外定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNoonFamily ,  ") '割引戻（外定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNight ,  ") '割引戻（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNightSyotai ,  ") '割引戻（外定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideRegularNightFamily ,  ") '割引戻（外定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKiR ,  ") '割引戻（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKiRSyotai ,  ") '割引戻（外企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKiRFamily ,  ") '割引戻（外企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKi ,  ") '割引戻（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKiSyotai ,  ") '割引戻（外企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnOutsideKiFamily ,  ") '割引戻（外企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN U.KINGAKU ELSE 0 END ) WaribikiReturnCapital ,  ") '割引戻（キャピタル）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnCapitalSyotai ,  ") '割引戻（キャピタル）招待
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '831' AND SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN U.KINGAKU ELSE 0 END ) WaribikiReturnCapitalFamily ,  ") '割引戻（キャピタル）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnDayRegularNoon ,  ") '取扱手数料戻（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnDayRegularNight ,  ") '取扱手数料戻（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnDayKiR ,  ") '取扱手数料戻（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnDayKi ,  ") '取扱手数料戻（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnOutsideRegularNoon ,  ") '取扱手数料戻（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnOutsideRegularNight ,  ") '取扱手数料戻（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnOutsideKiR ,  ") '取扱手数料戻（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnOutsideKi ,  ") '取扱手数料戻（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '13'                                                                         THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnCapital ,  ") '取扱手数料戻（キャピタル）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '841' AND SI.SEISAN_KBN = '40'                                                                         THEN U.KINGAKU ELSE 0 END ) ToriatukaiFeeReturnOther ,  ") '取扱手数料戻（他）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '161'                                                 AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' THEN U.KINGAKU ELSE 0 END ) CreditCardModosiJapanese ,  ") 'クレジットカード払戻（日本語）
        sqlString.AppendLine("   SUM ( CASE WHEN U.SEISAN_KOUMOKU_CD = '161'                                                 AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' THEN U.KINGAKU ELSE 0 END ) CreditCardModosiGaikokugo   ") 'クレジットカード払戻（外国語）

        sqlString.AppendLine(" FROM T_SEISAN_INFO SI ")
        sqlString.AppendLine(" INNER JOIN T_SEISAN_INFO_UTIWAKE U ")
        sqlString.AppendLine("    ON SI.SEISAN_INFO_SEQ = U.SEISAN_INFO_SEQ ")
        sqlString.AppendLine(" LEFT JOIN T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("    ON SI.CRS_CD    = T_CRS_LEDGER_BASIC.CRS_CD ")
        sqlString.AppendLine("   AND SI.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
        sqlString.AppendLine("   AND SI.GOUSYA    = T_CRS_LEDGER_BASIC.GOUSYA ")
        sqlString.AppendLine(" LEFT JOIN (")
        sqlString.AppendLine("   SELECT")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine("     , CASE WHEN COUNT(*) > 0 THEN '1' ELSE '0' END AS WARIBIKI_TYPE_KBN_1")
        sqlString.AppendLine("   FROM")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE")
        sqlString.AppendLine("   WHERE")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE.WARIBIKI_CD = '002'") '招待優待
        sqlString.AppendLine("   GROUP BY")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine(" ) WARIBIKI_1")
        sqlString.AppendLine("    ON WARIBIKI_1.EIGYOSYO_KBN   = SUBSTR(SI.KENNO,1,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.TICKETTYPE_CD  = SUBSTR(SI.KENNO,2,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.MOKUTEKI       = SUBSTR(SI.KENNO,3,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.ISSUE_YEARLY   = SUBSTR(SI.KENNO,4,2)")
        sqlString.AppendLine("   AND WARIBIKI_1.SEQ_1          = SUBSTR(SI.KENNO,6,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.SEQ_2          = SUBSTR(SI.KENNO,7,4)")
        sqlString.AppendLine("   AND SI.WARIBIKI_TYPE          = '2'")
        sqlString.AppendLine(" LEFT JOIN (")
        sqlString.AppendLine("   SELECT")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine("     , CASE WHEN COUNT(*) > 0 THEN '1' ELSE '0' END AS WARIBIKI_TYPE_KBN_2")
        sqlString.AppendLine("   FROM")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE")
        sqlString.AppendLine("   WHERE")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE.WARIBIKI_CD = '004'") 'ファミリーチケット
        sqlString.AppendLine("   GROUP BY")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine(" ) WARIBIKI_2")
        sqlString.AppendLine("    ON WARIBIKI_2.EIGYOSYO_KBN   = SUBSTR(SI.KENNO,1,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.TICKETTYPE_CD  = SUBSTR(SI.KENNO,2,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.MOKUTEKI       = SUBSTR(SI.KENNO,3,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.ISSUE_YEARLY   = SUBSTR(SI.KENNO,4,2)")
        sqlString.AppendLine("   AND WARIBIKI_2.SEQ_1          = SUBSTR(SI.KENNO,6,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.SEQ_2          = SUBSTR(SI.KENNO,7,4)")
        sqlString.AppendLine("   AND SI.WARIBIKI_TYPE          = '2'")

        sqlString.AppendLine(" WHERE SI.COMPANY_CD = '00' ")
        sqlString.AppendLine("   AND SI.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("   AND SI.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.DELETE_DAY = 0  ")
        sqlString.AppendLine("   AND NVL(SI.URIAGE_KBN, ' ') <> 'V'  ")
        If Not String.IsNullOrWhiteSpace(CType(param("CrsKbn"), String)) Then
            sqlString.AppendLine("   AND (T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IS NULL ")
            sqlString.AppendLine("    OR  T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_GAIKYAKU_KBN", param.Item("CrsKbn"), OracleDbType.Char, 1) & ") ")
        End If
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("   AND SI.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If
        Dim checkoutItemSignonTime As List(Of String) = CType(param("SignonTime"), List(Of String))
        If checkoutItemSignonTime.Count > 0 Then
            sqlString.AppendLine("   AND SI.SIGNON_TIME IN (")
            For i As Integer = 0 To checkoutItemSignonTime.Count - 1
                If i > 0 Then
                    sqlString.AppendLine(",")
                End If
                sqlString.AppendLine(setParam("SIGNON_TIME_" & i, checkoutItemSignonTime(i), OracleDbType.Int32, 6))
            Next
            sqlString.AppendLine(") ")
        End If

        'sql実行
        Dim result As DataTable = MyBase.getDataTable(sqlString.ToString)
        'リターン
        Return result
    End Function

    ''' <summary>
    ''' 窓口日報
    ''' 4-5プレビューOR印刷(ユーザIDの指定なしorあり　かつ　明細貢＝ON)
    ''' 詳細頁を出力する = ONの場合
    ''' 精算情報より
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),ユーザID,邦人/外客区分</param>
    ''' <returns></returns>
    Public Function getPrintONSeisanInfo(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder
        '借方
        sqlString.AppendLine(" SELECT   ")
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNoon ,  ") 'クーポン払戻（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNoonSyotai ,  ") 'クーポン払戻（日定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNoonFamily ,  ") 'クーポン払戻（日定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNight ,  ") 'クーポン払戻（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNightSyotai ,  ") 'クーポン払戻（日定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayRegularNightFamily ,  ") 'クーポン払戻（日定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKiR ,  ") 'クーポン払戻（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKiRSyotai ,  ") 'クーポン払戻（日企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKiRFamily ,  ") 'クーポン払戻（日企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKi ,  ") 'クーポン払戻（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKiSyotai ,  ") 'クーポン払戻（日企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiDayKiFamily ,  ") 'クーポン払戻（日企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNoon ,  ") 'クーポン払戻（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNoonSyotai ,  ") 'クーポン払戻（外定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNoonFamily ,  ") 'クーポン払戻（外定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNight ,  ") 'クーポン払戻（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNightSyotai ,  ") 'クーポン払戻（外定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideRegularNightFamily ,  ") 'クーポン払戻（外定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKiR ,  ") 'クーポン払戻（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKiRSyotai ,  ") 'クーポン払戻（外企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKiRFamily ,  ") 'クーポン払戻（外企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKi ,  ") 'クーポン払戻（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKiSyotai ,  ") 'クーポン払戻（外企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiOutsideKiFamily ,  ") 'クーポン払戻（外企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_REFUND ELSE 0 END ) CouponModosiCapital ,  ") 'クーポン払戻（ホテル）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiCapitalSyotai ,  ") 'クーポン払戻（ホテル）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_REFUND ELSE 0 END ) CouponModosiCapitalFamily ,  ") 'クーポン払戻（ホテル）ファミリ
        '貸方
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNoon ,  ") 'クーポン売上払戻（日定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNoonSyotai ,  ") 'クーポン売上払戻（日定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNoonFamily ,  ") 'クーポン売上払戻（日定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNight ,  ") 'クーポン売上払戻（日定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNightSyotai ,  ") 'クーポン売上払戻（日定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayRegularNightFamily ,  ") 'クーポン売上払戻（日定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKiR ,  ") 'クーポン売上払戻（日企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKiRSyotai ,  ") 'クーポン売上払戻（日企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKiRFamily ,  ") 'クーポン売上払戻（日企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKi ,  ") 'クーポン売上払戻（日企）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKiSyotai ,  ") 'クーポン売上払戻（日企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'H' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageDayKiFamily ,  ") 'クーポン売上払戻（日企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNoon ,  ") 'クーポン売上払戻（外定昼）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNoonSyotai ,  ") 'クーポン売上払戻（外定昼）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '1' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNoonFamily ,  ") 'クーポン売上払戻（外定昼）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNight ,  ") 'クーポン売上払戻（外定夜）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNightSyotai ,  ") 'クーポン売上払戻（外定夜）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '11' AND SI.CRS_KBN_1 = '2' AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideRegularNightFamily ,  ") 'クーポン売上払戻（外定夜）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKiR ,  ") 'クーポン売上払戻（外企Ｒ）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKiRSyotai ,  ") 'クーポン売上払戻（外企Ｒ）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '12'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKiRFamily ,  ") 'クーポン売上払戻（外企Ｒ）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKi ,  ") 'クーポン売上払戻（外企）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKiSyotai ,  ") 'クーポン売上払戻（外企）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '20'                        AND T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = 'G' AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageOutsideKiFamily ,  ") 'クーポン売上払戻（外企）ファミリ
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') <> '2'                              THEN COUPON_URIAGE ELSE 0 END ) CouponUriageCapital ,  ") 'クーポン売上払戻（ホテル）
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_1 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageCapitalSyotai ,  ") 'クーポン売上払戻（ホテル）招待
        sqlString.AppendLine("   SUM ( CASE WHEN SI.SEISAN_KBN = '13'                                                                         AND NVL(SI.WARIBIKI_TYPE,' ') = '2' AND WARIBIKI_TYPE_KBN_2 = '1' THEN COUPON_URIAGE ELSE 0 END ) CouponUriageCapitalFamily    ") 'クーポン売上払戻（ホテル）ファミリ

        sqlString.AppendLine(" FROM T_SEISAN_INFO SI ")
        sqlString.AppendLine(" LEFT JOIN T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine("    ON SI.CRS_CD    = T_CRS_LEDGER_BASIC.CRS_CD ")
        sqlString.AppendLine("   AND SI.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
        sqlString.AppendLine("   AND SI.GOUSYA    = T_CRS_LEDGER_BASIC.GOUSYA ")
        sqlString.AppendLine(" LEFT JOIN (")
        sqlString.AppendLine("   SELECT")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine("     , CASE WHEN COUNT(*) > 0 THEN '1' ELSE '0' END AS WARIBIKI_TYPE_KBN_1")
        sqlString.AppendLine("   FROM")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE")
        sqlString.AppendLine("   WHERE")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE.WARIBIKI_CD = '002'") '招待優待
        sqlString.AppendLine("   GROUP BY")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine(" ) WARIBIKI_1")
        sqlString.AppendLine("    ON WARIBIKI_1.EIGYOSYO_KBN   = SUBSTR(SI.KENNO,1,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.TICKETTYPE_CD  = SUBSTR(SI.KENNO,2,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.MOKUTEKI       = SUBSTR(SI.KENNO,3,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.ISSUE_YEARLY   = SUBSTR(SI.KENNO,4,2)")
        sqlString.AppendLine("   AND WARIBIKI_1.SEQ_1          = SUBSTR(SI.KENNO,6,1)")
        sqlString.AppendLine("   AND WARIBIKI_1.SEQ_2          = SUBSTR(SI.KENNO,7,4)")
        sqlString.AppendLine("   AND SI.WARIBIKI_TYPE          = '2'")
        sqlString.AppendLine(" LEFT JOIN (")
        sqlString.AppendLine("   SELECT")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine("     , CASE WHEN COUNT(*) > 0 THEN '1' ELSE '0' END AS WARIBIKI_TYPE_KBN_2")
        sqlString.AppendLine("   FROM")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE")
        sqlString.AppendLine("   WHERE")
        sqlString.AppendLine("     T_HAKKEN_INFO_CHARGE.WARIBIKI_CD = '004'") 'ファミリーチケット
        sqlString.AppendLine("   GROUP BY")
        sqlString.AppendLine("       T_HAKKEN_INFO_CHARGE.EIGYOSYO_KBN")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.TICKETTYPE_CD")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.MOKUTEKI")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.ISSUE_YEARLY")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_1")
        sqlString.AppendLine("     , T_HAKKEN_INFO_CHARGE.SEQ_2")
        sqlString.AppendLine(" ) WARIBIKI_2")
        sqlString.AppendLine("    ON WARIBIKI_2.EIGYOSYO_KBN   = SUBSTR(SI.KENNO,1,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.TICKETTYPE_CD  = SUBSTR(SI.KENNO,2,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.MOKUTEKI       = SUBSTR(SI.KENNO,3,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.ISSUE_YEARLY   = SUBSTR(SI.KENNO,4,2)")
        sqlString.AppendLine("   AND WARIBIKI_2.SEQ_1          = SUBSTR(SI.KENNO,6,1)")
        sqlString.AppendLine("   AND WARIBIKI_2.SEQ_2          = SUBSTR(SI.KENNO,7,4)")
        sqlString.AppendLine("   AND SI.WARIBIKI_TYPE          = '2'")

        sqlString.AppendLine(" WHERE SI.COMPANY_CD = '00' ")
        sqlString.AppendLine("   AND SI.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("   AND SI.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("   AND SI.DELETE_DAY = 0  ")
        sqlString.AppendLine("   AND NVL(SI.URIAGE_KBN, ' ') <> 'V'  ")
        If Not String.IsNullOrWhiteSpace(CType(param("CrsKbn"), String)) Then
            sqlString.AppendLine("   AND (T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN IS NULL ")
            sqlString.AppendLine("    OR  T_CRS_LEDGER_BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_GAIKYAKU_KBN", param.Item("CrsKbn"), OracleDbType.Char, 1) & ") ")
        End If
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("   AND SI.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If
        Dim checkoutItemSignonTime As List(Of String) = CType(param("SignonTime"), List(Of String))
        If checkoutItemSignonTime.Count > 0 Then
            sqlString.AppendLine("   AND SI.SIGNON_TIME IN (")
            For i As Integer = 0 To checkoutItemSignonTime.Count - 1
                If i > 0 Then
                    sqlString.AppendLine(",")
                End If
                sqlString.AppendLine(setParam("SIGNON_TIME_" & i, checkoutItemSignonTime(i), OracleDbType.Int32, 6))
            Next
            sqlString.AppendLine(") ")
        End If

        'sql実行
        Dim result As DataTable = MyBase.getDataTable(sqlString.ToString)
        'リターン
        Return result
    End Function
#End Region
#End Region

#Region "窓口日報（その他商品） P05_0502"
    ''' <summary>
    ''' 窓口日報（その他商品）
    ''' 6-7「ユーザIDの指定あり」または「ユーザIDの指定なし」
    ''' </summary>
    ''' <param name="param">営業所コード,出力対象月日(From),出力対象月日(To),ユーザID,サインオン時間</param>
    ''' <returns></returns>
    Public Function getSonotaPrintAriNasi(param As Hashtable) As DataTable
        Dim sqlString As StringBuilder = New StringBuilder

        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine("   SUS.KOTEIBU_OUT_LINE_NO, ")
        sqlString.AppendLine("   SUS.TAISYAKU_KBN, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_NAME, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_CD_1, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_CD_2, ")
        sqlString.AppendLine("   ABS(SUM(NVL(SI.OTHER_URIAGE_SYOHIN_URIAGE,0) - NVL(SI.OTHER_URIAGE_SYOHIN_MODOSI,0))) AS SYUKEI_KINGAKU ")
        sqlString.AppendLine(" FROM M_SONOTA_URIAGE_SYOHIN SUS ")
        sqlString.AppendLine(" LEFT JOIN T_SEISAN_INFO SI ")
        sqlString.AppendLine("   ON SUS.OTHER_URIAGE_SYOHIN_CD_1 = SI.OTHER_URIAGE_SYOHIN_CD_1 ")
        sqlString.AppendLine("  AND SUS.OTHER_URIAGE_SYOHIN_CD_2 = SI.OTHER_URIAGE_SYOHIN_CD_2 ")
        sqlString.AppendLine("  AND SI.COMPANY_CD = '00' ")
        sqlString.AppendLine("  AND SI.EIGYOSYO_CD = " & setParam("EIGYOSYO_CD", param.Item("EigyosyoCd"), OracleDbType.Char, 2))
        sqlString.AppendLine("  AND SI.CREATE_DAY >= " & setParam("CREATE_DAY_FROM", param.Item("OutTaisyoMonDayFrom"), OracleDbType.Int32, 8))
        sqlString.AppendLine("  AND SI.CREATE_DAY <= " & setParam("CREATE_DAY_TO", param.Item("OutTaisyoMonDayTo"), OracleDbType.Int32, 8))
        sqlString.AppendLine("  AND SI.DELETE_DAY = 0  ")
        If Not String.IsNullOrWhiteSpace(CType(param("UserID"), String)) Then
            sqlString.AppendLine("  AND SI.ENTRY_PERSON_CD = " & setParam("ENTRY_PERSON_CD", param.Item("UserID"), OracleDbType.Varchar2, 20))
        End If
        Dim checkoutItemSignonTime As List(Of String) = CType(param("SignonTime"), List(Of String))
        If checkoutItemSignonTime.Count > 0 Then
            sqlString.AppendLine("  AND SI.SIGNON_TIME IN (")
            For i As Integer = 0 To checkoutItemSignonTime.Count - 1
                If i > 0 Then
                    sqlString.AppendLine(",")
                End If
                sqlString.AppendLine(setParam("SIGNON_TIME_" & i, checkoutItemSignonTime(i), OracleDbType.Int32, 6))
            Next
            sqlString.AppendLine(") ")
        End If

        sqlString.AppendLine(" WHERE SUS.KOTEIBU_OUT_LINE_NO > 0 ")
        sqlString.AppendLine("   AND NVL(SUS.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" GROUP BY ")
        sqlString.AppendLine("   SUS.KOTEIBU_OUT_LINE_NO, ")
        sqlString.AppendLine("   SUS.TAISYAKU_KBN, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_NAME, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_CD_1, ")
        sqlString.AppendLine("   SUS.OTHER_URIAGE_SYOHIN_CD_2  ")

        sqlString.AppendLine(" ORDER BY SUS.KOTEIBU_OUT_LINE_NO, ")
        sqlString.AppendLine("          SUS.TAISYAKU_KBN ")

        'sql実行
        Dim result As DataTable = MyBase.getDataTable(sqlString.ToString)
        'リターン
        Return result
    End Function
#End Region
End Class
