Imports System.Text

''' <summary>
''' コース一覧照会（増発）のDAクラス
''' </summary>
Public Class CrsItiranZouhatsu_DA
    Inherits Hatobus.ReservationManagementSystem.Common.DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getCrsItiranZouhatsu                  '一覧結果取得検索
    End Enum

    Private Const YSet As String = "Y"

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessCrsItiranZouhatsu(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getCrsItiranZouhatsu
                '一覧結果取得検索
                sqlString = getCrsItiranZouhatsu(paramInfoList)
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
    Protected Overloads Function getCrsItiranZouhatsu(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" " & CommonKyushuUtil.setSQLDateFormat("SYUPT_DAY", CType(FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD, String), "CRS") & " AS SYUPT_DAY") ' 出発日
        sqlString.AppendLine(" , CRS.CRS_CD ")                   ' コースコード
        sqlString.AppendLine(" , CRS.CRS_NAME ")                 ' コース名
        sqlString.AppendLine(" , CRS.GOUSYA ")                   ' 号車
        sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_1 ")        ' 配車経由コード1
        sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_2 ")        ' 配車経由コード2
        sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_3 ")        ' 配車経由コード3
        sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_4 ")        ' 配車経由コード4
        sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_5 ")        ' 配車経由コード5
        sqlString.AppendLine(" , PLC1.PLACE_NAME_1 ")            ' 場所名1
        sqlString.AppendLine(" , PLC2.PLACE_NAME_1 AS PLACE_NAME_2 ")            ' 場所名2
        sqlString.AppendLine(" , PLC3.PLACE_NAME_1 AS PLACE_NAME_3 ")            ' 場所名3
        sqlString.AppendLine(" , PLC4.PLACE_NAME_1 AS PLACE_NAME_4 ")            ' 場所名4
        sqlString.AppendLine(" , PLC5.PLACE_NAME_1 AS PLACE_NAME_5 ")            ' 場所名5
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") ' 出発時間1
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_2") & " AS SYUPT_TIME_2 ") ' 出発時間2
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_3") & " AS SYUPT_TIME_3 ") ' 出発時間3
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_4") & " AS SYUPT_TIME_4 ") ' 出発時間4
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_5") & " AS SYUPT_TIME_5 ") ' 出発時間5
        sqlString.AppendLine(" , CRS.UNKYU_KBN ")                ' 運休区分
        sqlString.AppendLine(" , CD1.CODE_NAME AS UNKYU_KBN_NAME ")  '運休区分(名称)
        sqlString.AppendLine(" , CD2.CODE_NAME AS SAIKOU_KAKUTEI_KBN_NAME ")  '催行確定区分(名称)
        sqlString.AppendLine(" , CRS.TEJIMAI_KBN ")              ' 手仕舞区分
        sqlString.AppendLine(" , CRS.KUSEKI_NUM_TEISEKI + CRS.KUSEKI_NUM_SUB_SEAT AS KUSEKI_NUM_TEISEKI ")       ' 空席数
        sqlString.AppendLine(" , CRS.YOYAKU_NUM_TEISEKI + CRS.YOYAKU_NUM_SUB_SEAT AS YOYAKU_NUM_TEISEKI ")       ' 予約数定席
        sqlString.AppendLine(" , CRS.BLOCK_KAKUHO_NUM + CRS.EI_BLOCK_REGULAR + CRS.EI_BLOCK_HO AS BLOCK_KAKUHO_NUM ")         ' ブロック確保数
        sqlString.AppendLine(" , CRS.KUSEKI_KAKUHO_NUM ")        ' 空席確保数
        sqlString.AppendLine(" , CRS.CANCEL_WAIT_NINZU ")        ' キャンセル待ち人数
        sqlString.AppendLine(" , CRS.ROOM_ZANSU_SOKEI ")         ' 部屋残数総計
        sqlString.AppendLine(" , CRS.ROOM_ZANSU_ONE_ROOM ")      ' 部屋残数１人部屋
        sqlString.AppendLine(" , CRS.ROOM_ZANSU_TWO_ROOM ")      ' 部屋残数２人部屋
        sqlString.AppendLine(" , CRS.ROOM_ZANSU_THREE_ROOM ")    ' 部屋残数３人部屋
        sqlString.AppendLine(" , CRS.CRS_BLOCK_ONE_1R ")         ' コースブロック１名１Ｒ
        sqlString.AppendLine(" , CRS.CRS_BLOCK_TWO_1R ")         ' コースブロック２名１Ｒ
        sqlString.AppendLine(" , CRS.CRS_BLOCK_THREE_1R ")       ' コースブロック３名１Ｒ
        sqlString.AppendLine(" , CRS.CAR_NO ")                   ' 車番
        sqlString.AppendLine(" , CRS.CAR_TYPE_CD ")              ' 車種コード
        sqlString.AppendLine(" , CRS.SUB_SEAT_OK_KBN ")          ' 補助席可区分
        sqlString.AppendLine(" , CRS.USING_FLG ")                ' 使用中フラグ
        sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_PGMID ")      ' システム更新PGM-ID
        sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_PERSON_CD ")  ' システム更新者コード
        sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_DAY ")        ' システム更新日
        sqlString.AppendLine(" , 0 AS SOSU ")                    ' 総数(加算用)
        sqlString.AppendLine(" , LPAD(' ',6) AS JYOUKYOU ")      ' 状況
        sqlString.AppendLine(" , CRS.TEIKI_KIKAKU_KBN ")         ' 定期＿企画区分
        sqlString.AppendLine(" , LPAD(' ',4) AS ZOUHATSU ")      ' 増発
        sqlString.AppendLine(" , CRS.YOYAKU_STOP_FLG ")          ' 予約停止フラグ
        sqlString.AppendLine(" , CRS.BUS_RESERVE_CD ")           ' バス指定コード
        sqlString.AppendLine(" , ZIMG.USING_FLG AS ZIMG_USING_FLG ")                ' 使用中フラグ
        sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_PGMID AS ZIMG_SYSTEM_UPDATE_PGMID ")      ' システム更新PGM-ID
        sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_PERSON_CD AS ZIMG_SYSTEM_UPDATE_PERSON_CD ")  ' システム更新者コード
        sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_DAY AS ZIMG_SYSTEM_UPDATE_DAY ")        ' システム更新日
        sqlString.AppendLine(" , CRS.CAPACITY_REGULAR ")           ' 定員定
        sqlString.AppendLine(" , CRS.CAPACITY_HO_1KAI ")           ' 定員補
        sqlString.AppendLine(" , CRS.HOUJIN_GAIKYAKU_KBN ")        ' 邦人・外客区分
        sqlString.AppendLine(" , CRS.ZOUHATSUMOTO_GOUSYA ")        ' 増発元号車
        sqlString.AppendLine(" ," & CommonKyushuUtil.setSQLDateFormat("ZOUHATSU_DAY", CType(FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD, String), "CRS") & " AS ZOUHATSU_DAY ") ' 増発日
        sqlString.AppendLine(" , USR.USER_NAME AS ZOUHATSU_ENTRY_PERSON_CD ")   ' 増発実施者

        ' FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS ") ' コース台帳（基本）
        sqlString.AppendLine("INNER JOIN T_ZASEKI_IMAGE ZIMG ")
        sqlString.AppendLine("  ON     CRS.SYUPT_DAY = ZIMG.SYUPT_DAY ")
        sqlString.AppendLine("     AND CRS.BUS_RESERVE_CD = ZIMG.BUS_RESERVE_CD ")
        sqlString.AppendLine("     AND CRS.GOUSYA = ZIMG.GOUSYA ") ' 座席イメージ（バス情報）
        sqlString.AppendLine("LEFT JOIN M_PLACE PLC1 ON CRS.HAISYA_KEIYU_CD_1 = PLC1.PLACE_CD ") ' 場所マスタ
        sqlString.AppendLine("LEFT JOIN M_PLACE PLC2 ON CRS.HAISYA_KEIYU_CD_2 = PLC2.PLACE_CD ") ' 場所マスタ
        sqlString.AppendLine("LEFT JOIN M_PLACE PLC3 ON CRS.HAISYA_KEIYU_CD_3 = PLC3.PLACE_CD ") ' 場所マスタ
        sqlString.AppendLine("LEFT JOIN M_PLACE PLC4 ON CRS.HAISYA_KEIYU_CD_4 = PLC4.PLACE_CD ") ' 場所マスタ
        sqlString.AppendLine("LEFT JOIN M_PLACE PLC5 ON CRS.HAISYA_KEIYU_CD_5 = PLC5.PLACE_CD ") ' 場所マスタ
        sqlString.AppendLine("LEFT JOIN M_CODE  CD1  ON CRS.UNKYU_KBN = CD1.CODE_VALUE AND CD1.CODE_BUNRUI = " & CType(FixedCd.CodeBunrui.unkyu, String)) ' コードマスタ(運休区分(名称))
        sqlString.AppendLine("LEFT JOIN M_CODE  CD2  ON CRS.SAIKOU_KAKUTEI_KBN = CD2.CODE_VALUE AND CD2.CODE_BUNRUI = " & CType(FixedCd.CodeBunrui.saikou, String)) ' コードマスタ(催行確定区分(名称))
        sqlString.AppendLine("LEFT JOIN M_USER  USR  ON USR.COMPANY_CD = '0001' AND CRS.ZOUHATSU_ENTRY_PERSON_CD = USR.USER_ID ") ' 利用者情報

        ' WHERE句
        ' 出発日Fromは必須
        ' 出発日
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" CRS.SYUPT_DAY BETWEEN " & setParam("SyuptDayFrom", paramList.Item("SyuptDayFrom"), OracleDbType.Decimal, 8, 0) & " AND " & setParam("SyuptDayTo", paramList.Item("SyuptDayTo"), OracleDbType.Decimal, 8, 0))
        ' コースコードは必須
        ' コースコード
        sqlString.AppendLine(" AND CRS.CRS_CD = " & setParam("CrsCd", paramList.Item("CrsCd"), OracleDbType.Char))
        ' 号車
        If Not String.IsNullOrEmpty(CType(paramList.Item("Gousya"), String)) Then
            sqlString.AppendLine(" AND CRS.GOUSYA = " & setParam("Gousya", paramList.Item("Gousya"), OracleDbType.Decimal, 3, 0))
        End If
        ' 空席数
        If Not String.IsNullOrEmpty(CType(paramList.Item("KusekiNum"), String)) Then
            sqlString.AppendLine(" AND CRS.KUSEKI_NUM_TEISEKI <= " & setParam("KusekiNum", paramList.Item("KusekiNum"), OracleDbType.Decimal, 3, 0))
        End If
        ' WT有
        If Not String.IsNullOrEmpty(CType(paramList.Item("WT"), String)) Then
            sqlString.AppendLine(" AND CRS.CANCEL_WAIT_NINZU > 0 ")
            'Else
            '    sqlString.AppendLine(" AND CRS.CANCEL_WAIT_NINZU = 0 ")
        End If
        ' 出発時間
        If Not String.IsNullOrEmpty(CType(paramList.Item("SyuptTimeFrom"), String)) OrElse Not String.IsNullOrEmpty(CType(paramList.Item("SyuptTimeTo"), String)) Then
            sqlString.AppendLine(" AND CRS.SYUPT_TIME_1 BETWEEN " & setParam("SyuptTimeFrom", paramList.Item("SyuptTimeFrom"), OracleDbType.Decimal, 4, 0) & " AND " & setParam("SyuptTimeTo", paramList.Item("SyuptTimeTo"), OracleDbType.Decimal, 4, 0))
        End If

        ' 邦人／外客区分保持用
        Dim houjinGaikyakuKbn As String = ""
        ' 日本語
        If Not String.IsNullOrEmpty(CType(paramList.Item("CrsKind3Japanese"), String)) Then
            houjinGaikyakuKbn = houjinGaikyakuKbn & "'" & CType(FixedCd.HoujinGaikyakuKbnType.Houjin, String) & "'" & ","
        End If
        ' 外国語
        If Not String.IsNullOrEmpty(CType(paramList.Item("CrsKind3Gaikokugo"), String)) Then
            houjinGaikyakuKbn = houjinGaikyakuKbn & "'" & CType(FixedCd.HoujinGaikyakuKbnType.Gaikyaku, String) & "'" & ","
        End If
        ' 邦人／外客区分
        If Not String.IsNullOrEmpty(houjinGaikyakuKbn) Then
            ' 末尾カンマ削除
            houjinGaikyakuKbn = Left(houjinGaikyakuKbn, Len(houjinGaikyakuKbn) - 1)
            sqlString.AppendLine(" AND CRS.HOUJIN_GAIKYAKU_KBN IN (" & houjinGaikyakuKbn & ") ")
        Else
            sqlString.AppendLine(" AND CRS.HOUJIN_GAIKYAKU_KBN = CRS.HOUJIN_GAIKYAKU_KBN ")
        End If
        sqlString.AppendLine(" AND (CRS.MARU_ZOU_MANAGEMENT_KBN = ' ' OR CRS.MARU_ZOU_MANAGEMENT_KBN IS NULL) ")
        sqlString.AppendLine(" AND (CRS.UNKYU_KBN <> '" & CType(FixedCd.UnkyuKbn.Haishi, String) & "' OR CRS.UNKYU_KBN IS NULL) ")
        sqlString.AppendLine(" AND (CRS.SAIKOU_KAKUTEI_KBN <> '" & CType(FixedCd.SaikouKakuteiKbn.Haishi, String) & "' OR CRS.SAIKOU_KAKUTEI_KBN IS NULL) ")
        sqlString.AppendLine(" AND (CRS.YOYAKU_NG_FLG = ' ' OR CRS.YOYAKU_NG_FLG IS NULL) ")
        sqlString.AppendLine(" AND NVL(CRS.DELETE_DAY,0) = 0 ")
        'sqlString.AppendLine(" AND CRS.CRS_CD = CRS.BUS_RESERVE_CD ")
        ' ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" CRS.SYUPT_DAY ")
        sqlString.AppendLine(",CRS.SYUPT_TIME_1 ")
        sqlString.AppendLine(",CRS.CRS_CD ")
        sqlString.AppendLine(",CRS.GOUSYA ")

        Return sqlString.ToString
    End Function

#End Region

End Class
