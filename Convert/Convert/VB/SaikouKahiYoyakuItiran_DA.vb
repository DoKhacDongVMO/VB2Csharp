Imports System.Text

''' <summary>
''' 催行可否照会（予約情報一覧）のDAクラス
''' </summary>
Public Class SaikouKahiYoyakuItiran_DA
    Inherits Hatobus.ReservationManagementSystem.Common.DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getSaikouKahiYoyakuItiran                 '一覧結果取得検索
    End Enum

    '予約情報（基本）エンティティ
    Private clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity()

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessSaikouKahiYoyakuItiran(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getSaikouKahiYoyakuItiran
                '一覧結果取得検索
                sqlString = getSaikouKahiYoyakuItiran(paramInfoList)
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
    Protected Overloads Function getSaikouKahiYoyakuItiran(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" DISTINCT ")
        'sqlString.AppendLine("   YIB.YOYAKU_KBN || TO_CHAR(YIB.YOYAKU_NO) AS YOYAKU_NO_DISP")       ' 予約No(表示用)
        sqlString.AppendLine("   YIB.YOYAKU_KBN || ',' || TRIM(TO_CHAR(YIB.YOYAKU_NO,'000,000,000')) AS YOYAKU_NO_DISP")       ' 予約No(表示用)
        sqlString.AppendLine(" , YIB.YOYAKU_KBN ")                       ' 予約区分
        sqlString.AppendLine(" , YIB.YOYAKU_NO ")                        ' 予約ＮＯ
        sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_1 ")             ' ＲＯＯＭＩＮＧ別人数１
        sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_2 ")             ' ＲＯＯＭＩＮＧ別人数２
        sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_3 ")             ' ＲＯＯＭＩＮＧ別人数３
        sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_4 ")             ' ＲＯＯＭＩＮＧ別人数４
        sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_5 ")             ' ＲＯＯＭＩＮＧ別人数５
        sqlString.AppendLine(" , YIB.HAKKEN_NAIYO ")                     ' 発券内容
        sqlString.AppendLine(" , YIB.SEISAN_HOHO ")                      ' 精算方法
        sqlString.AppendLine(" , YIB.SURNAME || YIB.NAME AS YYKMKS ")    ' 姓+名
        sqlString.AppendLine(" , " & CommonKyushuUtil.setSQLDateFormat("ENTRY_DAY", CType(FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD, String), "YIB") & " AS ENTRY_DAY ") ' 登録日
        sqlString.AppendLine(" , ( ")
        sqlString.AppendLine("     SELECT")
        sqlString.AppendLine("       SUM(ADT.CHARGE_APPLICATION_NINZU) ")
        sqlString.AppendLine("     FROM ")
        sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ADT ")
        sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK4 ON ADT.CHARGE_KBN_JININ_CD = CJK4.CHARGE_KBN_JININ_CD ")
        sqlString.AppendLine("     WHERE ")
        With clsYoyakuInfoBasicEntity
            sqlString.AppendLine("          ADT.YOYAKU_NO = YIB.YOYAKU_NO ")
            sqlString.AppendLine("      And ADT.YOYAKU_KBN = YIB.YOYAKU_KBN ")
            ' かつ　予約情報（コース料金_料金区分）.料金区分（人員）コード　＝　大人
            sqlString.AppendLine("      And CJK4.SHUYAKU_CHARGE_KBN_CD = '" & FixedCd.ChargeKbnJininCd.adult & "'")
            sqlString.AppendLine("      GROUP BY CJK4.SHUYAKU_CHARGE_KBN_CD ")
            sqlString.AppendLine(" ) AS NINZU_ADULT ") ' 大人
            sqlString.AppendLine(" , ( ")
            sqlString.AppendLine("     SELECT")
            sqlString.AppendLine("       SUM(MAN.CHARGE_APPLICATION_NINZU) ")
            sqlString.AppendLine("     FROM ")
            sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN MAN ")
            sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK5 ON MAN.CHARGE_KBN_JININ_CD = CJK5.CHARGE_KBN_JININ_CD ")
            sqlString.AppendLine("     WHERE ")
            sqlString.AppendLine("           MAN.YOYAKU_NO = YIB.YOYAKU_NO ")
            sqlString.AppendLine("       And MAN.YOYAKU_KBN = YIB.YOYAKU_KBN")
            sqlString.AppendLine("       And(")
            ' 料金区分（人員）マスタ.集約料金区分コード＝’大人’
            sqlString.AppendLine("               CJK5.SHUYAKU_CHARGE_KBN_CD = '" & FixedCd.ShuyakuChargeKbnType.adult & "'")
            ' かつ　料金区分（人員）マスタ.性別＝’男性’
            sqlString.AppendLine("           And CJK5.SEX_BETU = '" & FixedCd.SexBetu.Man & "'")
            sqlString.AppendLine("       )")
            sqlString.AppendLine("     GROUP BY CJK5.SHUYAKU_CHARGE_KBN_CD")
            sqlString.AppendLine(" ) AS NINZU_MAN") ' 大人(男)(算出用)
            sqlString.AppendLine(" , ( ")
            sqlString.AppendLine("     SELECT")
            sqlString.AppendLine("       SUM(WMN.CHARGE_APPLICATION_NINZU) ")
            sqlString.AppendLine("     FROM ")
            sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN WMN ")
            sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK6 ON WMN.CHARGE_KBN_JININ_CD = CJK6.CHARGE_KBN_JININ_CD ")
            sqlString.AppendLine("     WHERE ")
            sqlString.AppendLine("           WMN.YOYAKU_NO = YIB.YOYAKU_NO ")
            sqlString.AppendLine("       And WMN.YOYAKU_KBN = YIB.YOYAKU_KBN")
            sqlString.AppendLine("       And(")
            ' 料金区分（人員）マスタ.集約料金区分コード＝’大人’
            sqlString.AppendLine("               CJK6.SHUYAKU_CHARGE_KBN_CD = '" & FixedCd.ShuyakuChargeKbnType.adult & "'")
            ' かつ　料金区分（人員）マスタ.性別＝’女性’
            sqlString.AppendLine("           And CJK6.SEX_BETU = '" & FixedCd.SexBetu.Woman & "'")
            sqlString.AppendLine("       )")
            sqlString.AppendLine("     GROUP BY CJK6.SHUYAKU_CHARGE_KBN_CD")
            sqlString.AppendLine(" ) AS NINZU_WOMAN") ' 大人(女)(算出用)
            sqlString.AppendLine(" , ( ")
            sqlString.AppendLine("     SELECT")
            sqlString.AppendLine("       SUM(JNR.CHARGE_APPLICATION_NINZU) ")
            sqlString.AppendLine("     FROM ")
            sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN JNR ")
            sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK2 ON JNR.CHARGE_KBN_JININ_CD = CJK2.CHARGE_KBN_JININ_CD ")
            sqlString.AppendLine("     WHERE ")
            sqlString.AppendLine("           JNR.YOYAKU_NO = YIB.YOYAKU_NO ")
            sqlString.AppendLine("       And JNR.YOYAKU_KBN = YIB.YOYAKU_KBN")
            ' かつ　料金区分（人員）マスタ.集約料金区分　＝　中人
            sqlString.AppendLine("       And CJK2.SHUYAKU_CHARGE_KBN_CD = '" & FixedCd.ShuyakuChargeKbnType.junior & "'")
            sqlString.AppendLine(" GROUP BY ")
            sqlString.AppendLine("   CJK2.SHUYAKU_CHARGE_KBN_CD")
            sqlString.AppendLine(" ) AS NINZU_JUNIOR") ' 中人(算出用)
            sqlString.AppendLine(" , ( ")
            sqlString.AppendLine("     SELECT")
            sqlString.AppendLine("       SUM(CHR.CHARGE_APPLICATION_NINZU) ")
            sqlString.AppendLine("     FROM ")
            sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN CHR ")
            sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK3 ON CHR.CHARGE_KBN_JININ_CD = CJK3.CHARGE_KBN_JININ_CD ")
            sqlString.AppendLine("     WHERE ")
            sqlString.AppendLine("           CHR.YOYAKU_NO = YIB.YOYAKU_NO ")
            sqlString.AppendLine("       And CHR.YOYAKU_KBN = YIB.YOYAKU_KBN")
            ' かつ　料金区分（人員）マスタ.集約料金区分　＝　小人
            sqlString.AppendLine("       And CJK3.SHUYAKU_CHARGE_KBN_CD = '" & FixedCd.ShuyakuChargeKbnType.child & "'")
            sqlString.AppendLine(" GROUP BY ")
            sqlString.AppendLine("   CJK3.SHUYAKU_CHARGE_KBN_CD")
            sqlString.AppendLine(" ) AS NINZU_CHILD") ' 小人(算出用)
            sqlString.AppendLine(" , 0 AS NINZU_SUM ")                       ' 予約合計(算出用)
            sqlString.AppendLine(" ,  LPAD(' ',2) AS HAKKEN_NAIYO_DISP ")         ' 発券内容(表示用)
            sqlString.AppendLine(" ,  LPAD(' ',10) AS SEISAN_HOHO_DISP ")         ' 精算方法(表示用)

            ' FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("T_YOYAKU_INFO_BASIC YIB ") ' 予約情報（基本）
            sqlString.AppendLine("LEFT JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YIC ON YIB.YOYAKU_KBN = YIC.YOYAKU_KBN ")
            sqlString.AppendLine("  And YIB.YOYAKU_NO = YIC.YOYAKU_NO ")  ' 予約情報（コース料金_料金区分）
            sqlString.AppendLine("LEFT JOIN M_CHARGE_JININ_KBN CJK ON YIC.CHARGE_KBN_JININ_CD = CJK.CHARGE_KBN_JININ_CD ")  ' 料金区分（人員）マスタ

            ' WHERE句
            sqlString.AppendLine(" WHERE ")
            ' 予約情報（基本）.出発日　＝　パラメータ.出発日
            sqlString.AppendLine(" YIB.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), OracleDbType.Decimal, 8, 0))
            ' かつ　予約情報（基本）.コースコード　＝　パラメータ.コースコード
            sqlString.AppendLine(" And YIB.CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), OracleDbType.Char))
            ' かつ　予約情報（基本）.号車　＝　パラメータ.号車
            sqlString.AppendLine(" And YIB.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), OracleDbType.Decimal, 3, 0))
            ' かつ 予約情報（基本）.削除日 = 0 (未削除)
            sqlString.AppendLine(" And NVL(YIB.DELETE_DAY,0) = 0 ")
        End With
        ' ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" YIB.YOYAKU_NO ")  ' 予約No(昇順)

        Return sqlString.ToString
    End Function

#End Region

End Class
