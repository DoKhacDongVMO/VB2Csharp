Imports System.Text
''' <summary>
''' 未収回収先売上検索_DA (S05_0508)
''' </summary>
Public Class S05_0508_DA
    Inherits DataAccessorBase

    '予約情報（基本）エンティティ
    Private clsTYoyakuInfoBasicEntity As New TYoyakuInfoBasicEntity()
    '精算情報エンティティ
    Private clsTSeisanInfoEntity As New TSeisanInfoEntity()

    ''' <summary>
    ''' 定期・企画一覧データ取得
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    Public Function getTeikiKikakuList(ByVal paramList As Hashtable) As DataTable
        'sql生成
        Dim sql As New StringBuilder

        sql.AppendLine(" SELECT ")
        sql.AppendLine("  CASE WHEN SEISAN_INF.SYUPT_DAY IS NOT NULL THEN TO_CHAR(TO_DATE(SEISAN_INF.SYUPT_DAY,'" & CommonFormatType.dateFormatYYYYMMDDbySlash & "'),'" & CommonFormatType.dateFormatYYYYMMDDbySlash & "') END SYUPT_DAY ")
        sql.AppendLine(" ,SEISAN_INF.YOYAKU_NO ")
        sql.AppendLine(" ,Y_BASIC.SURNAME || ' ' || Y_BASIC.NAME AS SIMEI ")
        sql.AppendLine(" ,(SELECT SUM(CHARGE_APPLICATION_NINZU) ")
        sql.AppendLine("   FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN Y_CHARGE ")
        sql.AppendLine("   WHERE ")
        sql.AppendLine("     Y_CHARGE.YOYAKU_KBN = Y_BASIC.YOYAKU_KBN ")
        sql.AppendLine("     AND ")
        sql.AppendLine("     Y_CHARGE.YOYAKU_NO = Y_BASIC.YOYAKU_NO ")
        sql.AppendLine("    ) AS CHARGE_APPLICATION_NINZU_SUM ")
        sql.AppendLine(" ,SEISAN_INF.CRS_CD ")
        sql.AppendLine(" ,CRS_BASIC.CRS_NAME ")
        sql.AppendLine(" ,TO_CHAR(SEISAN_U.KINGAKU, '" & CommonFormatType.numberFormatComma & "') AS KINGAKU ")
        sql.AppendLine(" ,SEISAN_INF.YOYAKU_KBN ")
        sql.AppendLine(" ,SEISAN_INF.SEQ ")
        sql.AppendLine(" ,SEISAN_INF.KENNO ")
        sql.AppendLine(" ,SEISAN_INF.SEISAN_INFO_SEQ ")
        sql.AppendLine("   FROM ")
        sql.AppendLine("     T_SEISAN_INFO SEISAN_INF ")
        sql.AppendLine("       INNER JOIN T_YOYAKU_INFO_BASIC Y_BASIC ")
        sql.AppendLine("       ON (SEISAN_INF.YOYAKU_KBN = Y_BASIC.YOYAKU_KBN) ")
        sql.AppendLine("       AND (SEISAN_INF.YOYAKU_NO = Y_BASIC.YOYAKU_NO) ")
        sql.AppendLine("       INNER JOIN T_CRS_LEDGER_BASIC CRS_BASIC ")
        sql.AppendLine("       ON (SEISAN_INF.CRS_CD = CRS_BASIC.CRS_CD) ")
        sql.AppendLine("       AND (SEISAN_INF.SYUPT_DAY = CRS_BASIC.SYUPT_DAY) ")
        sql.AppendLine("       AND (SEISAN_INF.GOUSYA = CRS_BASIC.GOUSYA) ")
        sql.AppendLine("       INNER JOIN T_SEISAN_INFO_UTIWAKE SEISAN_U ")
        sql.AppendLine("       ON (SEISAN_INF.SEISAN_INFO_SEQ = SEISAN_U.SEISAN_INFO_SEQ) ")
        sql.AppendLine("       AND (SEISAN_U.SEISAN_KOUMOKU_CD = '" & SeisanItemCd.misyu & "') ")  '･･･精算情報内訳．精算項目コード：未集金を対象とする
        sql.AppendLine("       AND (SEISAN_U.KINGAKU <> 0) ")                                      '･･･精算情報内訳．金額：0ではない
        sql.AppendLine("   WHERE ")
        '精算情報．削除日：0[未設定]である
        sql.AppendLine("     NVL(SEISAN_INF.DELETE_DAY , 0 ) = 0 ")
        '精算情報．売上区分：V[ボイド]ではない
        sql.AppendLine("     AND NVL(SEISAN_INF.URIAGE_KBN , ' ') <> '" & UriageKbnType.Void & "' ")
        '■予約情報（基本）
        With clsTYoyakuInfoBasicEntity
            'パラメータ（定期・企画区分）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("TeikiKikakuKbn"), String)) Then
                '予約情報.定期・企画区分
                sql.AppendLine("    AND Y_BASIC.TEIKI_KIKAKU_KBN = " & setParam("TeikiKikakuKbn", paramList.Item("TeikiKikakuKbn"), .TeikiKikakuKbn.DBType, .TeikiKikakuKbn.IntegerBu, .TeikiKikakuKbn.DecimalBu))
            End If
            'パラメータ（予約番号）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("YoyakuNo"), String)) Then
                '予約情報.予約区分 + 予約情報.予約番号 = 画面．予約番号
                sql.AppendLine("    AND Y_BASIC.YOYAKU_KBN || Y_BASIC.YOYAKU_NO = " & setParam("YoyakuNo", paramList.Item("YoyakuNo"), .YoyakuKbn.DBType, .YoyakuKbn.IntegerBu + .YoyakuNo.IntegerBu, .YoyakuKbn.DecimalBu + .YoyakuNo.DecimalBu))
            End If
            'パラメータ日付（開始）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("fromDay"), String)) Then
                '予約情報.出発日 = 画面．日付(From)
                sql.AppendLine("    AND Y_BASIC.SYUPT_DAY >= " & setParam("fromDay", paramList.Item("fromDay"), .SyuptDay.DBType, .SyuptDay.IntegerBu + .SyuptDay.IntegerBu, .SyuptDay.DecimalBu))
            End If
            'パラメータ日付（終了）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("toDay"), String)) Then
                '予約情報.出発日 = 画面．日付(To)
                sql.AppendLine("    AND Y_BASIC.SYUPT_DAY <= " & setParam("toDay", paramList.Item("toDay"), .SyuptDay.DBType, .SyuptDay.IntegerBu + .SyuptDay.IntegerBu, .SyuptDay.DecimalBu))
            End If

        End With
        '■精算情報
        With clsTSeisanInfoEntity
            'パラメータ（会社コード[営業所コード]）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("CompanyCd"), String)) Then
                '予約情報.定期・企画区分
                sql.AppendLine("    AND SEISAN_INF.COMPANY_CD = " & setParam("CompanyCd", paramList.Item("CompanyCd"), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            End If
            'パラメータ（営業所コード）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("EigyosyoCd"), String)) Then
                '予約情報.定期・企画区分
                sql.AppendLine("    AND SEISAN_INF.EIGYOSYO_CD = " & setParam("EigyosyoCd", paramList.Item("EigyosyoCd"), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            End If
        End With
        sql.AppendLine(" ORDER BY  ")
        sql.AppendLine("   SEISAN_INF.SYUPT_DAY ")
        sql.AppendLine("  ,SEISAN_INF.YOYAKU_KBN ")
        sql.AppendLine("  ,SEISAN_INF.YOYAKU_NO ")

        Dim result As DataTable = MyBase.getDataTable(sql.ToString)

        Return result

    End Function

    ''' <summary>
    ''' クーポン券一覧データ取得
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    Public Function getCouponKenList(ByVal paramList As Hashtable) As DataTable

        '★★★★★★★★★★★★★★★★★★★★★★
        ' クーポン券はPH2実装予定
        '★★★★★★★★★★★★★★★★★★★★★★
        'sql生成
        Dim sql As New StringBuilder

        sql.AppendLine(" SELECT ")
        sql.AppendLine(" ANY_TABLE.ANY_ITEM1 ")
        sql.AppendLine(",ANY_TABLE.ANY_ITEM2 ")
        sql.AppendLine(" FROM ")
        sql.AppendLine(" ANY_TABLE ")

        Dim result As DataTable = MyBase.getDataTable(sql.ToString)

        Return result

    End Function

    ''' <summary>
    ''' その他商品一覧データ取得
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    Public Function getSonotaSyohinList(ByVal paramList As Hashtable) As DataTable
        'sql生成
        Dim sql As New StringBuilder

        sql.AppendLine(" SELECT ")
        sql.AppendLine(" CASE WHEN SEISAN_INF.CREATE_DAY IS NOT NULL THEN TO_CHAR(TO_DATE(SEISAN_INF.CREATE_DAY,'" & CommonFormatType.dateFormatYYYYMMDDbySlash & "'),'" & CommonFormatType.dateFormatYYYYMMDDbySlash & "') END CREATE_DAY ")
        sql.AppendLine(" ,M_SONOTA.OTHER_URIAGE_SYOHIN_NAME ")
        sql.AppendLine(" ,SEISAN_INF.OTHER_URIAGE_AITESAKI ")
        sql.AppendLine(" ,SEISAN_INF.OTHER_URIAGE_SYOHIN_BIKO || SEISAN_INF.OTHER_URIAGE_SYOHIN_BIKO_2 AS OTHER_URIAGE_SYOHIN_BIKO ")
        sql.AppendLine(" ,TO_CHAR(SEISAN_U.KINGAKU, '" & CommonFormatType.numberFormatComma & "') AS KINGAKU ")
        sql.AppendLine(" ,SEISAN_INF.YOYAKU_KBN ")
        sql.AppendLine(" ,SEISAN_INF.YOYAKU_NO ")
        sql.AppendLine(" ,SEISAN_INF.SEISAN_INFO_SEQ ")
        sql.AppendLine(" ,SEISAN_INF.SEQ ")
        sql.AppendLine(" ,SEISAN_INF.KENNO ")
        sql.AppendLine(" FROM ")
        sql.AppendLine(" T_SEISAN_INFO SEISAN_INF ")
        sql.AppendLine("     INNER JOIN T_SEISAN_INFO_UTIWAKE SEISAN_U ")
        sql.AppendLine("     ON (SEISAN_INF.SEISAN_INFO_SEQ = SEISAN_U.SEISAN_INFO_SEQ) ")
        sql.AppendLine("     AND (SEISAN_U.SEISAN_KOUMOKU_CD = '" & SeisanItemCd.misyu & "') ")  '･･･精算情報内訳．精算項目コード：未集金を対象とする
        sql.AppendLine("     AND (SEISAN_U.KINGAKU <> 0) ")                                      '･･･精算情報内訳．金額：0はない
        sql.AppendLine("     INNER JOIN M_SONOTA_URIAGE_SYOHIN M_SONOTA ")
        sql.AppendLine("     ON (SEISAN_INF.OTHER_URIAGE_SYOHIN_H_G = M_SONOTA.HOUJIN_GAIKYAKU_KBN) ")
        sql.AppendLine("     AND (SEISAN_INF.OTHER_URIAGE_SYOHIN_CD_1 = M_SONOTA.OTHER_URIAGE_SYOHIN_CD_1) ")
        sql.AppendLine("     AND (SEISAN_INF.OTHER_URIAGE_SYOHIN_CD_2 = M_SONOTA.OTHER_URIAGE_SYOHIN_CD_2) ")
        sql.AppendLine("     AND (M_SONOTA.HOUJIN_GAIKYAKU_KBN = '" & HoujinGaikyakuKbnType.Houjin & "') ")
        sql.AppendLine(" WHERE ")
        sql.AppendLine("  NVL(SEISAN_INF.DELETE_DAY , 0 ) = 0 ")
        '■精算情報
        With clsTSeisanInfoEntity
            'パラメータ（予約番号）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("YoyakuNo"), String)) Then
                '精算情報.予約区分 + 予約情報.予約番号 = 画面．予約番号
                sql.AppendLine("    AND SEISAN_INF.YOYAKU_KBN || SEISAN_INF.YOYAKU_NO = " & setParam("YoyakuNo", paramList.Item("YoyakuNo"), .YoyakuKbn.DBType, .YoyakuKbn.IntegerBu + .YoyakuNo.IntegerBu, .YoyakuKbn.DecimalBu + .YoyakuNo.DecimalBu))
            End If
            'パラメータ日付（開始）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("fromDay"), String)) Then
                '精算情報.作成日 = 画面．日付(From)
                sql.AppendLine("    AND SEISAN_INF.CREATE_DAY >= " & setParam("fromDay", paramList.Item("fromDay"), .CreateDay.DBType, .CreateDay.IntegerBu + .CreateDay.IntegerBu, .CreateDay.DecimalBu))
            End If
            'パラメータ日付（終了）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("toDay"), String)) Then
                '精算情報.作成日 = 画面．日付(To)
                sql.AppendLine("    AND SEISAN_INF.CREATE_DAY <= " & setParam("toDay", paramList.Item("toDay"), .CreateDay.DBType, .CreateDay.IntegerBu + .SyuptDay.IntegerBu, .CreateDay.DecimalBu))
            End If
            'パラメータ（会社コード[営業所コード]）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("CompanyCd"), String)) Then
                '予約情報.定期・企画区分
                sql.AppendLine("    AND SEISAN_INF.COMPANY_CD = " & setParam("CompanyCd", paramList.Item("CompanyCd"), .CompanyCd.DBType, .CompanyCd.IntegerBu, .CompanyCd.DecimalBu))
            End If
            'パラメータ（営業所コード）が設定されている場合
            If Not String.IsNullOrEmpty(CType(paramList.Item("EigyosyoCd"), String)) Then
                '予約情報.定期・企画区分
                sql.AppendLine("    AND SEISAN_INF.EIGYOSYO_CD = " & setParam("EigyosyoCd", paramList.Item("EigyosyoCd"), .EigyosyoCd.DBType, .EigyosyoCd.IntegerBu, .EigyosyoCd.DecimalBu))
            End If
        End With
        sql.AppendLine(" ORDER BY  ")
        sql.AppendLine("   SEISAN_INF.CREATE_DAY ")
        sql.AppendLine("  ,SEISAN_INF.OTHER_URIAGE_SYOHIN_CD_1 ")
        sql.AppendLine("  ,SEISAN_INF.OTHER_URIAGE_SYOHIN_CD_2 ")

        Dim result As DataTable = MyBase.getDataTable(sql.ToString)

        Return result

    End Function

End Class
