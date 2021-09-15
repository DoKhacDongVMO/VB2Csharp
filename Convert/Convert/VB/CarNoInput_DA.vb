Imports System.Text

''' <summary>
''' 車番入力のDAクラス
''' </summary>
Public Class CarNoInput_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    ' バス会社コード（＝はとバス）
    Private Const SetHatobuscd As String = "400100"

    ' サンライズピック
    Private Const SetSunrize As String = "X"

    ' コース台帳（基本）エンティティ
    Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()

    ' 予約情報（基本）エンティティ
    Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity()

    Public Enum accessType As Integer
        getCarNoInput                   ' 一覧結果取得検索
        getYoyakuCount                  ' 予約情報（基本）    件数取得
        getBusReserveCount              ' バス指定コード      件数取得
        getReturnCrs                    ' コース台帳（基本）  退避
        getReturnyoyaku                 ' 予約情報（基本）    退避
        getCarNoMaster                  ' 車番・車種取得検索
        getKyosaiUnkouKbn               ' 共催運行区分検索
        getLegBasic                     ' コース台帳（基本）取得
    End Enum

    Private comTehai As New TehaiCommon
#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessCarNoInput(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getCarNoInput
                ' 一覧結果取得検索
                sqlString = getCarNoInput(paramInfoList)
            Case accessType.getYoyakuCount
                ' 予約情報（基本） 件数取得
                sqlString = getYoyakuCount(paramInfoList)
            Case accessType.getBusReserveCount
                ' バス指定コード 件数取得
                sqlString = getBusReserveCount(paramInfoList)
            Case accessType.getReturnCrs
                ' コース台帳（基本） 退避
                sqlString = getReturnCrs(paramInfoList)
            Case accessType.getReturnyoyaku
                ' 予約情報（基本） 退避
                sqlString = getReturnyoyaku(paramInfoList)
            Case accessType.getCarNoMaster
                ' 車番・車種取得検索
                sqlString = getCarNoMaster(paramInfoList)
            Case accessType.getKyosaiUnkouKbn
                ' 共催運行区分検索
                sqlString = getKyosaiUnkouKbn(paramInfoList)
            Case accessType.getLegBasic
                ' コース台帳（基本）取得
                sqlString = getLegBasic(paramInfoList)
            Case Else
                ' 該当処理なし
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
    ''' 検索用SELECT（一覧結果を取得する）
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getCarNoInput(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Dim whereFlg As Boolean = True

        Try

            paramClear()

            With clsCrsLedgerBasicEntity

                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" crs.CRS_CD ")                                                    ' コースコード
                sqlString.AppendLine(",crs.CRS_NAME ")                                                  ' コース名
                sqlString.AppendLine(",place.PLACE_NAME_1 As PLACE_NAME_SHORT")                         ' 場所名1
                sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("crs.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") ' 出発時間１
                sqlString.AppendLine(",crs.GOUSYA ")                                                    ' 号車
                sqlString.AppendLine(",crs.CAR_NO ")                                                    ' 車番
                sqlString.AppendLine(",crs.YOYAKU_NUM_TEISEKI ")                                        ' 予約数定席
                sqlString.AppendLine(",crs.YOYAKU_NUM_SUB_SEAT ")                                       ' 予約数補助席
                sqlString.AppendLine(",crs.CAPACITY_REGULAR ")                                          ' 定員定
                sqlString.AppendLine(",crs.CAPACITY_HO_1KAI ")                                          ' 定員補１階
                sqlString.AppendLine(",crs.CAR_TYPE_CD_YOTEI ")                                         ' 車種コード予定
                sqlString.AppendLine(",crs.CAR_TYPE_CD ")                                               ' 車種コード
                sqlString.AppendLine(",LPAD(' ', 256) AS MESSAGE ")                                     ' メッセージ
                sqlString.AppendLine(",crs.SYUPT_DAY ")                                                 ' 出発日
                sqlString.AppendLine(",crs.BUS_RESERVE_CD ")                                            ' バス指定コード
                sqlString.AppendLine(",crs.ZASEKI_RESERVE_KBN ")                                        ' 座席指定区分
                sqlString.AppendLine(",crs.SUB_SEAT_OK_KBN ")                                           ' 補助席可区分
                sqlString.AppendLine(",crs.KYOSAI_UNKOU_KBN ")                                          ' 共催運行区分
                sqlString.AppendLine(",crs.EI_BLOCK_REGULAR ")                                          ' 営ブロック定
                sqlString.AppendLine(",crs.EI_BLOCK_HO ")                                               ' 営ブロック補
                sqlString.AppendLine(",crs.KUSEKI_KAKUHO_NUM ")                                         ' 空席確保数
                sqlString.AppendLine(",crs.BLOCK_KAKUHO_NUM ")                                          ' ブロック確保数
                sqlString.AppendLine(",crs.JYOSYA_CAPACITY ")                                           ' 乗車定員
                sqlString.AppendLine(",LPAD(' ', 1) AS USING_FLG ")                                     ' 使用中フラグ
                sqlString.AppendLine(",crs.KUSEKI_NUM_TEISEKI ")                                        ' 空席数定席
                sqlString.AppendLine(",crs.KUSEKI_NUM_SUB_SEAT ")                                       ' 空席数補助席
                sqlString.AppendLine(",crs.YOYAKU_KANOU_NUM ")                                          ' 予約可能数
                sqlString.AppendLine(",crs.SYSTEM_UPDATE_PGMID ")                                       ' システム更新ＰＧＭＩＤ
                sqlString.AppendLine(",crs.SYSTEM_UPDATE_PERSON_CD ")                                   ' システム更新者コード
                sqlString.AppendLine(",crs.SYSTEM_UPDATE_DAY ")                                         ' システム更新日
                sqlString.AppendLine(",0 AS ZASEKI_CAPACITYREGULAR ")                                   ' バス座席自動設定_定員数_定席
                sqlString.AppendLine(",0 AS ZASEKI_CAPACITYHO1KAI ")                                    ' バス座席自動設定_定員数_補助
                sqlString.AppendLine(",0 AS ZASEKI_KUSEKINUMTEISEKI ")                                  ' バス座席自動設定_空席数_定席
                sqlString.AppendLine(",0 AS ZASEKI_KUSEKINUMSUBSEAT ")                                  ' バス座席自動設定_空席数_補
                sqlString.AppendLine(",0 AS ZASEKI_EIBLOCKREGULAR ")                                    ' バス座席自動設定_営ブロック数_定席
                sqlString.AppendLine(",0 AS ZASEKI_EIBLOCKHO ")                                         ' バス座席自動設定_営ブロック数_補助
                sqlString.AppendLine(",LPAD(' ', 1) AS SABUN ")                                         ' 差分確認
                'FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" T_CRS_LEDGER_BASIC crs ")
                sqlString.AppendLine(" LEFT JOIN M_PLACE place ON ( ")
                sqlString.AppendLine("      place.PLACE_CD = crs.HAISYA_KEIYU_CD_1 ")
                sqlString.AppendLine(" AND (place.DELETE_DATE IS NULL OR place.DELETE_DATE = 0) ) ")
                'WHERE句
                sqlString.AppendLine(" WHERE ")

                '以下、入力値による抽出条件。
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then

                    ' 出発日 ＝ 入力.出発日
                    sqlString.AppendLine(" crs.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))

                    ' かつ　（ 入力.コース種別１ 日本語が選択されている場合  コース台帳（基本）.邦人/ 外客区分 = 日本語
                    '          入力.コース種別１ 外国語が選択されている場合  コース台帳（基本）.邦人/ 外客区分 = 外国語 ）
                    If CType(paramList.Item("SelectCrsKindJapanese"), Integer) = FixedCd.CheckBoxValue.OnValue And
                       CType(paramList.Item("SelectCrsKindGaikokugo"), Integer) = FixedCd.CheckBoxValue.OffValue Then
                        sqlString.AppendLine(" AND crs.HOUJIN_GAIKYAKU_KBN IN ('" & FixedCd.HoujinGaikyakuKbnType.Houjin & "')")

                    ElseIf CType(paramList.Item("SelectCrsKindJapanese"), Integer) = FixedCd.CheckBoxValue.OffValue And
                           CType(paramList.Item("SelectCrsKindGaikokugo"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.HOUJIN_GAIKYAKU_KBN IN ('" & FixedCd.HoujinGaikyakuKbnType.Gaikyaku & "')")

                    ElseIf CType(paramList.Item("SelectCrsKindJapanese"), Integer) = FixedCd.CheckBoxValue.OnValue And
                           CType(paramList.Item("SelectCrsKindGaikokugo"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.HOUJIN_GAIKYAKU_KBN IN ('" & FixedCd.HoujinGaikyakuKbnType.Houjin & "', '" & FixedCd.HoujinGaikyakuKbnType.Gaikyaku & "')")
                    End If

                    ' かつ 入力.コースコードが選択されている場合
                    '      バス指定コード ＝ 入力.コースコード
                    If CType(paramList.Item("SelectCrsCd"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        If Not String.IsNullOrEmpty(CType(paramList.Item("CrsCd"), String)) Then
                            sqlString.AppendLine(" AND (crs.BUS_RESERVE_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
                            sqlString.AppendLine(" OR crs.CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu) & ")")
                        End If
                    End If

                    '      定期（昼）が選択されている場合
                    '      コース種別 ＝ 定期 かつ コース区分1 ＝ 昼
                    If CType(paramList.Item("SelectTeikiNoon"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.CRS_KIND  = '" & FixedCd.TeikiKikakuKbn.Teiki & "'")
                        sqlString.AppendLine(" AND crs.CRS_KBN_1 = '" & FixedCd.CrsKbn1Type.noon & "'")
                    End If

                    '      定期（夜）が選択されている場合
                    '      コース種別1 ＝ 定期 かつ コース区分1 ＝ 夜
                    If CType(paramList.Item("SelectTeikiNight"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.CRS_KIND  = '" & FixedCd.TeikiKikakuKbn.Teiki & "'")
                        sqlString.AppendLine(" AND crs.CRS_KBN_1 = '" & FixedCd.CrsKbn1Type.night & "'")
                    End If

                    '      定期（郊外）が選択されている場合
                    '      コース種別1 ＝ 定期 かつ コース区分2 ＝ 郊外
                    If CType(paramList.Item("SelectTeikiKougai"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.CRS_KIND  = '" & FixedCd.TeikiKikakuKbn.Teiki & "'")
                        sqlString.AppendLine(" AND crs.CRS_KBN_2 = '" & FixedCd.crsKbn2Type.suburbs & "'")
                    End If

                    '      企画が選択されている場合
                    '      コース種別1 ＝ 企画
                    If CType(paramList.Item("SelectKikaku"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND crs.TEIKI_KIKAKU_KBN  = '" & FixedCd.TeikiKikakuKbn.Kikaku & "'")
                    End If

                    ' かつ 入力.号車がブランクでない場合 号車 ＝ 入力.号車
                    If Not String.IsNullOrEmpty(CType(paramList.Item("Gousya"), String)) Then
                        sqlString.AppendLine(" AND crs.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                    End If

                    ' かつ 入力.乗車地がブランクでない場合 配車経由コード1 ＝ 入力.乗車地
                    If Not String.IsNullOrEmpty(CType(paramList.Item("HaisyaKeiyuCd"), String)) Then
                        sqlString.AppendLine(" AND crs.HAISYA_KEIYU_CD_1 = " & setParam(.haisyaKeiyuCd1.PhysicsName, paramList.Item("HaisyaKeiyuCd"), .haisyaKeiyuCd1.DBType, .haisyaKeiyuCd1.IntegerBu, .haisyaKeiyuCd1.DecimalBu))
                    End If

                    ' かつ 入力.車番未入力分がブランクでない場合 車番 ＝ ブランク
                    If CType(paramList.Item("ChkMinyuryoku"), Integer) = FixedCd.CheckBoxValue.OnValue Then
                        sqlString.AppendLine(" AND NVL(crs.CAR_NO,0) = 0 ")
                    End If

                End If

                ' かつ コースコード（１桁目） ≠ サンライズピック ※現行だとX
                sqlString.AppendLine(" AND SUBSTR(crs.CRS_CD, 1, 1) != '" & SetSunrize & "'")
                ' かつ 運休区分 ≠ 運休 または 運休区分 ≠ 廃止
                sqlString.AppendLine(" AND NVL(crs.UNKYU_KBN, '*') NOT IN ('" & FixedCd.UnkyuKbn.Unkyu & "', '" & FixedCd.UnkyuKbn.Haishi & "') ")
                ' かつ 催行確定区分 ≠ 催行中止 または 催行確定区分 ≠ 廃止
                sqlString.AppendLine(" AND NVL(crs.SAIKOU_KAKUTEI_KBN, '*') NOT IN ('" & FixedCd.SaikouKakuteiKbn.Tyushi & "', '" & FixedCd.SaikouKakuteiKbn.Haishi & "') ")
                ' かつ 出発時キャリア区分 ＝ バス
                sqlString.AppendLine(" AND NVL(crs.SYUPT_JI_CARRIER_KBN, '1') = '" & FixedCd.SyuptJiCarrierKbnType.bus & "'")
                ' かつ 予約不可 ≠ 予約不可
                sqlString.AppendLine(" AND NVL(crs.YOYAKU_NG_FLG, '*') != '" & FixedCd.YoyakuNgFlg.Huka & "'")
                ' かつ 削除日 = 0
                sqlString.AppendLine(" AND COALESCE(crs.DELETE_DAY,0) = 0 ")
                ' かつ バス会社コード = ブランク
                sqlString.AppendLine(" AND (RTRIM(crs.BUS_COMPANY_CD) IS NULL ")
                ' または バス会社コード = はとバス
                sqlString.AppendLine(" OR   crs.BUS_COMPANY_CD = '" & SetHatobuscd & "')")

                'ORDER BY句
                sqlString.AppendLine(" ORDER BY ")
                sqlString.AppendLine(" crs.SYUPT_DAY ")
                sqlString.AppendLine(",crs.CRS_CD ")
                sqlString.AppendLine(",crs.SYUPT_TIME_1 ")
                sqlString.AppendLine(",crs.GOUSYA ")

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT（予約情報（基本）の使用中フラグ有りの件数を取得する）
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getYoyakuCount(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Try

            paramClear()

            With clsYoyakuInfoBasicEntity
                'SELECT
                sqlString.AppendLine(" SELECT COUNT(*) AS CNT ")
                'FROM句
                sqlString.AppendLine(" FROM T_YOYAKU_INFO_BASIC ")
                'WHERE句
                sqlString.AppendLine(" WHERE ")

                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine("     T_YOYAKU_INFO_BASIC.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))
                    sqlString.AppendLine(" AND T_YOYAKU_INFO_BASIC.CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
                    sqlString.AppendLine(" AND T_YOYAKU_INFO_BASIC.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                    sqlString.AppendLine(" AND T_YOYAKU_INFO_BASIC.USING_FLG = '" & FixedCd.UsingFlg.Use & "'")
                    sqlString.AppendLine(" AND COALESCE(T_YOYAKU_INFO_BASIC.DELETE_DAY,0) = 0 ")
                End If

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT（乗合いコースの使用中フラグをチェック）
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getBusReserveCount(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Try
            paramClear()

            With clsCrsLedgerBasicEntity

                'SELECT
                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" Gr.SYUPT_DAY ")                                  ' 出発日
                sqlString.AppendLine(",Gr.BUS_RESERVE_CD ")                             ' バス指定コード
                sqlString.AppendLine(",Gr.GOUSYA ")                                     ' 号車
                sqlString.AppendLine(",NVL(SUM(C_USING_FLG), 0) AS C_USING_FLG ")       ' 使用中フラグ件数 コース台帳
                sqlString.AppendLine(",NVL(SUM(Y_USING_FLG), 0) AS Y_USING_FLG ")       ' 使用中フラグ件数 予約情報
                'FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" ( ")
                sqlString.AppendLine("     SELECT ")
                sqlString.AppendLine("     T_CRS_LEDGER_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("    ,T_CRS_LEDGER_BASIC.CRS_CD ")
                sqlString.AppendLine("    ,T_CRS_LEDGER_BASIC.GOUSYA ")
                sqlString.AppendLine("    ,T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ")
                sqlString.AppendLine("    ,T_CRS_LEDGER_BASIC.USING_FLG AS C1_USING_FLG ")
                sqlString.AppendLine("    ,Gr1.C_USING_FLG ")
                sqlString.AppendLine("    ,Gr2.Y_USING_FLG ")
                sqlString.AppendLine("     FROM ")
                sqlString.AppendLine("     T_CRS_LEDGER_BASIC ")

                sqlString.AppendLine("     LEFT OUTER JOIN ")
                sqlString.AppendLine("      (  ")
                sqlString.AppendLine("         SELECT ")
                sqlString.AppendLine("         T_CRS_LEDGER_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("        ,T_CRS_LEDGER_BASIC.GOUSYA ")
                sqlString.AppendLine("        ,T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ")
                sqlString.AppendLine("        ,COUNT(T_CRS_LEDGER_BASIC.USING_FLG) AS C_USING_FLG ")
                sqlString.AppendLine("         FROM ")
                sqlString.AppendLine("         T_CRS_LEDGER_BASIC ")
                sqlString.AppendLine("         WHERE T_CRS_LEDGER_BASIC.USING_FLG = '" & FixedCd.UsingFlg.Use & "'")
                sqlString.AppendLine("          AND  COALESCE(T_CRS_LEDGER_BASIC.DELETE_DAY, 0) = 0 ")
                sqlString.AppendLine("         GROUP BY ")
                sqlString.AppendLine("         T_CRS_LEDGER_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("        ,T_CRS_LEDGER_BASIC.GOUSYA ")
                sqlString.AppendLine("        ,T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ) Gr1 ")
                sqlString.AppendLine("     ON ( ")
                sqlString.AppendLine("         Gr1.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("     AND Gr1.GOUSYA = T_CRS_LEDGER_BASIC.GOUSYA ")
                sqlString.AppendLine("     AND Gr1.BUS_RESERVE_CD = T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ) ")

                sqlString.AppendLine("     LEFT OUTER JOIN ")
                sqlString.AppendLine("      ( ")
                sqlString.AppendLine("         SELECT ")
                sqlString.AppendLine("         T_YOYAKU_INFO_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("        ,T_YOYAKU_INFO_BASIC.GOUSYA ")
                sqlString.AppendLine("        ,T_YOYAKU_INFO_BASIC.CRS_CD ")
                sqlString.AppendLine("        ,COUNT(T_YOYAKU_INFO_BASIC.USING_FLG) AS Y_USING_FLG ")
                sqlString.AppendLine("         FROM ")
                sqlString.AppendLine("         T_YOYAKU_INFO_BASIC ")
                sqlString.AppendLine("         WHERE T_YOYAKU_INFO_BASIC.USING_FLG = '" & FixedCd.UsingFlg.Use & "'")
                sqlString.AppendLine("          AND  COALESCE(T_YOYAKU_INFO_BASIC.DELETE_DAY, 0) = 0 ")
                sqlString.AppendLine("         GROUP BY ")
                sqlString.AppendLine("         T_YOYAKU_INFO_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("        ,T_YOYAKU_INFO_BASIC.GOUSYA ")
                sqlString.AppendLine("        ,T_YOYAKU_INFO_BASIC.CRS_CD ) Gr2 ")
                sqlString.AppendLine("     ON ( ")
                sqlString.AppendLine("         Gr2.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
                sqlString.AppendLine("     AND Gr2.GOUSYA = T_CRS_LEDGER_BASIC.GOUSYA ")
                sqlString.AppendLine("     AND Gr2.CRS_CD = T_CRS_LEDGER_BASIC.CRS_CD ) ")

                sqlString.AppendLine("     WHERE ")
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine("         T_CRS_LEDGER_BASIC.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))
                    sqlString.AppendLine("     AND T_CRS_LEDGER_BASIC.BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item("BusReserveCd"), .busReserveCd.DBType, .busReserveCd.IntegerBu, .busReserveCd.DecimalBu))
                    sqlString.AppendLine("     AND T_CRS_LEDGER_BASIC.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                    sqlString.AppendLine("     AND COALESCE(T_CRS_LEDGER_BASIC.DELETE_DAY,0) = 0 ")
                End If
                sqlString.AppendLine(" ) Gr ")

                'GROUP BY
                sqlString.AppendLine(" GROUP BY ")
                sqlString.AppendLine(" Gr.SYUPT_DAY ")
                sqlString.AppendLine(",Gr.GOUSYA ")
                sqlString.AppendLine(",Gr.BUS_RESERVE_CD ")

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getReturnCrs(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Try
            paramClear()

            With clsCrsLedgerBasicEntity

                'SELECT
                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" T_CRS_LEDGER_BASIC.SYUPT_DAY ")                          ' 出発日
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.CRS_CD ")                             ' コースコード
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.GOUSYA ")                             ' 号車
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ")                     ' バス指定コード
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.CAR_NO ")                             ' 車番
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.USING_FLG ")                          ' 使用中フラグ
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.SYSTEM_UPDATE_PGMID ")                ' システム更新ＰＧＭＩＤ
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.SYSTEM_UPDATE_PERSON_CD ")            ' システム更新者コード
                sqlString.AppendLine(",T_CRS_LEDGER_BASIC.SYSTEM_UPDATE_DAY ")                  ' システム更新日
                'FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" T_CRS_LEDGER_BASIC ")
                'WHERE句
                sqlString.AppendLine(" WHERE ")
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine("     T_CRS_LEDGER_BASIC.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))
                    sqlString.AppendLine(" AND T_CRS_LEDGER_BASIC.BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item("BusReserveCd"), .busReserveCd.DBType, .busReserveCd.IntegerBu, .busReserveCd.DecimalBu))
                    sqlString.AppendLine(" AND T_CRS_LEDGER_BASIC.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                    sqlString.AppendLine(" AND COALESCE(T_CRS_LEDGER_BASIC.DELETE_DAY,0) = 0 ")
                End If

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getReturnyoyaku(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Try

            paramClear()

            With clsYoyakuInfoBasicEntity

                'SELECT
                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" T_YOYAKU_INFO_BASIC.YOYAKU_KBN ")                        ' 予約区分
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.YOYAKU_NO ")                         ' 予約ＮＯ
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.SYUPT_DAY ")                         ' 出発日
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.CRS_CD ")                            ' コースコード
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.GOUSYA ")                            ' 号車
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.USING_FLG ")                         ' 使用中フラグ
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.SYSTEM_UPDATE_PGMID ")               ' システム更新ＰＧＭＩＤ
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.SYSTEM_UPDATE_PERSON_CD ")           ' システム更新者コード
                sqlString.AppendLine(",T_YOYAKU_INFO_BASIC.SYSTEM_UPDATE_DAY ")                 ' システム更新日
                'FROM句
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" T_YOYAKU_INFO_BASIC ")
                'WHERE句
                sqlString.AppendLine(" WHERE ")
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine("     T_YOYAKU_INFO_BASIC.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))
                    sqlString.AppendLine(" AND T_YOYAKU_INFO_BASIC.CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
                    sqlString.AppendLine(" AND T_YOYAKU_INFO_BASIC.GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                    sqlString.AppendLine(" AND COALESCE(T_YOYAKU_INFO_BASIC.DELETE_DAY,0) = 0 ")
                End If

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT（一覧_車番 LostFocus時に車番、車種マスタより、情報を取得する）
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getCarNoMaster(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Dim whereFlg As Boolean = True

        Try

            paramClear()

            With clsCrsLedgerBasicEntity

                'SELECT
                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" M_CAR_NO.CAR_NO ")                                               ' 車番
                sqlString.AppendLine(",M_CAR_NO.APPLICATION_DAY_FROM ")                                 ' 適用日自
                sqlString.AppendLine(",M_CAR_NO.APPLICATION_DAY_TO ")                                   ' 適用日至
                sqlString.AppendLine(",MKIND.CAR_CD ")                                                  ' 車種コード
                sqlString.AppendLine(",MKIND.CAR_CAPACITY ")                                            ' 定員（定）
                sqlString.AppendLine(",MKIND.CAR_EMG_CAPACITY ")                                        ' 定員（補・１階）
                sqlString.AppendLine(",M_CAR_NO.CAR_TYPE_CD ")                                          ' 車種コード
                'FROM句
                sqlString.AppendLine(" FROM M_CAR_NO ")
                sqlString.AppendLine(" LEFT OUTER JOIN ")
                sqlString.AppendLine(" ( ")
                sqlString.AppendLine("   SELECT ")
                sqlString.AppendLine("   M_CAR_KIND.CAR_CD ")
                sqlString.AppendLine("  ,M_CAR_KIND.CAR_CAPACITY ")
                sqlString.AppendLine("  ,M_CAR_KIND.CAR_EMG_CAPACITY ")
                sqlString.AppendLine("   FROM M_CAR_KIND ")
                sqlString.AppendLine("   WHERE ")
                sqlString.AppendLine("         M_CAR_KIND.DELETE_DATE = 0 ")
                sqlString.AppendLine("    OR   M_CAR_KIND.DELETE_DATE IS NULL ")
                sqlString.AppendLine(" ) MKIND ")
                sqlString.AppendLine(" ON M_CAR_NO.CAR_TYPE_CD = MKIND.CAR_CD ")
                'WHERE句
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine(" WHERE M_CAR_NO.CAR_NO = " & setParam(.carNo.PhysicsName, paramList.Item("Carno"), .carNo.DBType, .carNo.IntegerBu, .carNo.DecimalBu))
                    sqlString.AppendLine("  AND  COALESCE(M_CAR_NO.DELETE_DAY,0) = 0")
                Else
                    sqlString.AppendLine(" WHERE COALESCE(M_CAR_NO.DELETE_DAY,0) = 0")
                End If

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 検索用SELECT（共催運行区分マスタより、共催運行区分を取得する）
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getKyosaiUnkouKbn(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Dim whereFlg As Boolean = True

        Try

            paramClear()

            With clsCrsLedgerBasicEntity

                'SELECT
                sqlString.AppendLine(" SELECT ")
                sqlString.AppendLine(" M_KYOSAI_UNKOU_KBN.KYOSAI_UNKOU_KBN ")                           ' 共催運行区分
                'FROM句
                sqlString.AppendLine(" FROM M_KYOSAI_UNKOU_KBN ")
                'WHERE句
                If Not (paramList Is Nothing) AndAlso paramList.Count > 0 Then
                    sqlString.AppendLine(" WHERE M_KYOSAI_UNKOU_KBN.KYOSAI_UNKOU_KBN = " & setParam(.kyosaiUnkouKbn.PhysicsName, paramList.Item("KyosaiUnkouKbn"), .kyosaiUnkouKbn.DBType, .kyosaiUnkouKbn.IntegerBu, .kyosaiUnkouKbn.DecimalBu))
                    sqlString.AppendLine("  AND  COALESCE(M_KYOSAI_UNKOU_KBN.DELETE_DAY,0) = 0 ")
                End If

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' コース台帳（基本）取得
    ''' </summary>
    ''' <param name="paramList"></param>
    Protected Overloads Function getLegBasic(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        Try

            paramClear()

            With clsCrsLedgerBasicEntity

                'SELECT句
                sqlString.AppendLine("SELECT ")
                sqlString.AppendLine("   T_CRS_LEDGER_BASIC.SYUPT_DAY ")            ' 出発日
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.CRS_CD ")               ' コースコード
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.GOUSYA ")               ' 号車
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.BUS_RESERVE_CD ")       ' バス指定コード
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.CAR_TYPE_CD ")          ' 車種コード
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.ZASEKI_RESERVE_KBN ")   ' 座席指定区分
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.CAPACITY_REGULAR ")     ' 定員定
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.CAPACITY_HO_1KAI ")     ' 定員補１階
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.KUSEKI_NUM_TEISEKI ")   ' 空席数定席
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.KUSEKI_KAKUHO_NUM ")    ' 空席数補助席
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.EI_BLOCK_REGULAR")      ' 営ブロック定
                sqlString.AppendLine("  ,T_CRS_LEDGER_BASIC.EI_BLOCK_HO")           ' 営ブロック補
                'FROM句
                sqlString.AppendLine("FROM ")
                sqlString.AppendLine("  T_CRS_LEDGER_BASIC ")
                'WHERE句
                sqlString.AppendLine("WHERE ")
                sqlString.AppendLine("     T_CRS_LEDGER_BASIC.SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))
                sqlString.AppendLine(" AND T_CRS_LEDGER_BASIC.CRS_CD    = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))
                sqlString.AppendLine(" AND T_CRS_LEDGER_BASIC.GOUSYA    = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))
                sqlString.AppendLine(" AND COALESCE(T_CRS_LEDGER_BASIC.DELETE_DAY,0) = 0 ")

                Return sqlString.ToString
            End With

        Catch ex As Exception
            Throw
        End Try

    End Function

#End Region

#Region " UPDATE処理 "

    ''' <summary>
    ''' 使用中フラグ更新
    ''' </summary>
    ''' <param name="selectOldData"></param>
    ''' <param name="selectOldData_Crs"></param>
    ''' <param name="selectOldData_yoyaku"></param>
    ''' <param name="systemupdatepgmid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUsingFlgCrsAndYoyaku(ByVal selectOldData As DataTable, ByVal selectOldData_Crs As DataTable, selectOldData_yoyaku As DataTable, systemupdatepgmid As String) As Boolean

        Dim totalValue1 As New DataTable
        Dim totalValue2 As New DataTable

        Dim trn As OracleTransaction = Nothing
        Dim returnValue As Boolean = False

        Try

            totalValue1.Columns.Add("USING_FLG")             '使用中フラグ
            totalValue2.Columns.Add("USING_FLG")             '使用中フラグ

            'トランザクション開始
            trn = callBeginTransaction()

            For Each row As DataRow In selectOldData.Rows

                ' 使用中フラグをセットしたものが対象とし、更新する

                If CType(row("USING_FLG"), String) = FixedCd.UsingFlg.Use Then

                    ' 乗合いが絡むため、バス指定コードから紐づくコースを対象とする

                    'DataAccessクラス生成
                    Dim dataAccess As New CarNoInput_DA

                    Dim paramList As New Hashtable
                    Dim returnValueCrs As DataTable = Nothing

                    paramList.Add("SyuptDay", CType(row("SYUPT_DAY"), String))                      ' 出発日
                    paramList.Add("BusReserveCd", CType(row("BUS_RESERVE_CD"), String))             ' バス指定コード
                    paramList.Add("Gousya", CType(row("GOUSYA"), String))                           ' 号車

                    returnValueCrs = dataAccess.accessCarNoInput(CarNoInput_DA.accessType.getReturnCrs, paramList)

                    For Each rowCrs As DataRow In returnValueCrs.Rows

                        ' コース台帳（基本）

                        Dim syuptday As String = CType(rowCrs("SYUPT_DAY"), String)                 '出発日
                        Dim crscd As String = CType(rowCrs("CRS_CD"), String)                       'コースコード
                        Dim gousya As String = CType(rowCrs("GOUSYA"), String)                      '号車

                        returnValue = CommonCheckUtil.setUsingFlg_Crs(syuptday, crscd, gousya, systemupdatepgmid, trn, True)

                        Dim row2 As DataRow = totalValue1.NewRow
                        If returnValue = True Then
                            row2("USING_FLG") = FixedCd.UsingFlg.Use
                        Else
                            row2("USING_FLG") = String.Empty
                        End If

                        totalValue1.Rows.Add(row2)

                        ' 戻し退避用 使用中フラグ有りをマーキング
                        For Each rowChk As DataRow In selectOldData_Crs.Rows
                            If syuptday.Equals(rowChk("SYUPT_DAY")) AndAlso
                               crscd.Equals(rowChk("CRS_CD")) AndAlso
                               gousya.Equals(rowChk("GOUSYA")) Then
                                rowChk("USING_FLG_AFTER") = FixedCd.UsingFlg.Use
                            End If
                        Next

                        ' 予約情報（基本）

                        Dim paramList2 As New Hashtable
                        Dim returnValueYoyaku As DataTable = Nothing

                        paramList2.Add("SyuptDay", CType(syuptday, String))                         ' 出発日
                        paramList2.Add("CrsCd", CType(crscd, String))                               ' コースコード
                        paramList2.Add("Gousya", CType(gousya, String))                             ' 号車

                        returnValueYoyaku = dataAccess.accessCarNoInput(CarNoInput_DA.accessType.getReturnyoyaku, paramList2)

                        For Each rowYoyaku As DataRow In returnValueYoyaku.Rows

                            Dim yoyakuKbn As String = CType(rowYoyaku("YOYAKU_KBN"), String)        '予約区分
                            Dim yoyakuNo As String = CType(rowYoyaku("YOYAKU_NO"), String)          '予約ＮＯ

                            returnValue = CommonCheckUtil.setUsingFlg_Yoyaku(yoyakuKbn, yoyakuNo, systemupdatepgmid, trn, True)

                            Dim row3 As DataRow = totalValue2.NewRow
                            If returnValue = True Then
                                row3("USING_FLG") = FixedCd.UsingFlg.Use
                            Else
                                row3("USING_FLG") = String.Empty
                            End If

                            totalValue2.Rows.Add(row3)

                            ' 戻し退避用 使用中フラグ有りをマーキング
                            For Each rowChk2 As DataRow In selectOldData_yoyaku.Rows
                                If yoyakuKbn.Equals(rowChk2("YOYAKU_KBN")) AndAlso
                                   yoyakuNo.Equals(rowChk2("YOYAKU_NO")) Then

                                    rowChk2("USING_FLG_AFTER") = FixedCd.UsingFlg.Use
                                End If
                            Next
                        Next
                    Next
                End If
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

        Return True

    End Function

    ''' <summary>
    ''' 使用中フラグ更新（コース台帳（基本）、予約情報（基本）の使用中フラグを戻す）
    ''' </summary>
    ''' <param name="selectOldData_Crs"></param>
    ''' <param name="selectOldData_yoyaku"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUsingFlgCrsAndYoyakuModoshi(ByVal selectOldData_Crs As DataTable, ByVal selectOldData_yoyaku As DataTable) As Boolean

        Dim trn As OracleTransaction = Nothing
        Dim returnValue As Boolean = False

        Dim getString As String

        Try
            'トランザクション開始
            trn = callBeginTransaction()

            ' 戻し退避用 使用中フラグ有りを対象に戻す
            For Each rowChk As DataRow In selectOldData_Crs.Rows
                If FixedCd.UsingFlg.Use.Equals(rowChk("USING_FLG_AFTER")) Then

                    ' コース台帳（基本）
                    Dim paramList As New Hashtable

                    paramList.Add("SyuptDay", CType(rowChk("SYUPT_DAY"), Decimal))                              ' 出発日
                    paramList.Add("CrsCd", CType(rowChk("CRS_CD"), String))                                     ' コースコード
                    paramList.Add("Gousya", CType(rowChk("GOUSYA"), Decimal))                                   ' 号車
                    paramList.Add("systemUpdatePgmid", CType(rowChk("SYSTEM_UPDATE_PGMID"), String))            ' システム更新ＰＧＭＩＤ
                    paramList.Add("systemUpdatePersonCd", CType(rowChk("SYSTEM_UPDATE_PERSON_CD"), String))     ' システム更新者コード
                    paramList.Add("systemUpdateDay", CType(rowChk("SYSTEM_UPDATE_DAY"), Date))                  ' システム更新日
                    paramList.Add("BusReserveCd", CType(rowChk("BUS_RESERVE_CD"), String))                      ' バス指定コード

                    getString = Nothing
                    getString = executeReturnCrsData(paramList)

                    execNonQuery(trn, getString)

                End If
            Next

            ' 戻し退避用 使用中フラグ有りを対象に戻す
            For Each rowChk2 As DataRow In selectOldData_yoyaku.Rows
                If FixedCd.UsingFlg.Use.Equals(rowChk2("USING_FLG_AFTER")) Then

                    ' 予約情報（基本）
                    Dim paramList2 As New Hashtable

                    paramList2.Add("yoyakuKbn", CType(rowChk2("YOYAKU_KBN"), String))                           ' 予約区分
                    paramList2.Add("yoyakuNo", CType(rowChk2("YOYAKU_NO"), Decimal))                            ' 予約ＮＯ
                    paramList2.Add("systemUpdatePgmid", CType(rowChk2("SYSTEM_UPDATE_PGMID"), String))          ' システム更新ＰＧＭＩＤ
                    paramList2.Add("systemUpdatePersonCd", CType(rowChk2("SYSTEM_UPDATE_PERSON_CD"), String))   ' システム更新者コード
                    paramList2.Add("systemUpdateDay", CType(rowChk2("SYSTEM_UPDATE_DAY"), Date))                ' システム更新日

                    getString = Nothing
                    getString = executeReturnYoyakuData(paramList2)

                    execNonQuery(trn, getString)

                End If
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

        Return True

    End Function


    ''' <summary>
    ''' コース台帳（基本）（使用中フラグ戻し用として、使用する）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeReturnCrsData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        paramClear()

        With clsCrsLedgerBasicEntity

            'UPDATE
            sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine(" SET ")
            ' 使用中フラグ 
            sqlString.AppendLine(" USING_FLG = NULL ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(.systemUpdatePgmid.PhysicsName, paramList.Item("systemUpdatePgmid"), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu))
            ' システム更新者コード
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item("systemUpdatePersonCd"), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu))
            ' システム更新日
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(.systemUpdateDay.PhysicsName, paramList.Item("systemUpdateDay"), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu))

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))     ' 出発日
            sqlString.AppendLine(" AND BUS_RESERVE_CD = " & setParam(.busReserveCd.PhysicsName, paramList.Item("BusReserveCd"), .busReserveCd.DBType, .busReserveCd.IntegerBu, .busReserveCd.DecimalBu))                       ' バス指定コード
            sqlString.AppendLine(" AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))                  ' 号車

            Return sqlString.ToString

        End With

    End Function

    ''' <summary>
    ''' 予約情報（基本）：（使用中フラグ戻し用として、使用する）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeReturnYoyakuData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        paramClear()

        With clsYoyakuInfoBasicEntity

            'UPDATE
            sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC ")
            sqlString.AppendLine(" SET ")
            ' 使用中フラグ 
            sqlString.AppendLine(" USING_FLG = NULL ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(.systemUpdatePgmid.PhysicsName, paramList.Item("systemUpdatePgmid"), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu))
            ' システム更新者コード
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item("systemUpdatePersonCd"), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu))
            ' システム更新日
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(.systemUpdateDay.PhysicsName, paramList.Item("systemUpdateDay"), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu))

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     YOYAKU_KBN = " & setParam(.yoyakuKbn.PhysicsName, paramList.Item("yoyakuKbn"), .yoyakuKbn.DBType, .yoyakuKbn.IntegerBu, .yoyakuKbn.DecimalBu))   ' 予約区分
            sqlString.AppendLine(" AND YOYAKU_NO = " & setParam(.yoyakuNo.PhysicsName, paramList.Item("yoyakuNo"), .yoyakuNo.DBType, .yoyakuNo.IntegerBu, .yoyakuNo.DecimalBu))         ' 予約ＮＯ

            Return sqlString.ToString

        End With

    End Function

    ''' <summary>
    ''' 車番入力（データ更新用）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUpdateCarNoInputData(ByVal paramList As Hashtable, ByVal selectOldData_Crs As DataTable, ByVal selectOldData_yoyaku As DataTable) As Boolean

        Dim trn As OracleTransaction = Nothing

        Dim getString As String

        Dim Upflg As Boolean = False

        Try
            'トランザクション開始
            trn = callBeginTransaction()

            ' コース台帳（基本）
            'For Each rowChk As DataRow In selectOldData_Crs.Rows

            '    ' 出発日、コースコード、号車 が同じ
            '    If paramList.Item("SyuptDay").ToString.Equals(rowChk("SYUPT_DAY")) And
            '       paramList.Item("CrsCd").ToString.Equals(rowChk("CRS_CD")) And
            '       paramList.Item("Gousya").ToString.Equals(rowChk("GOUSYA")) Then

            '        ' かつ 使用中フラグ有りを対象
            '        If FixedCd.UsingFlg.Use.Equals(rowChk("USING_FLG_AFTER")) Then

            '            ' かつ 車番が変更されていない
            '            If paramList.Item("CarNo").ToString.Equals(rowChk("CAR_NO")) Then

            '                ' コース台帳（基本） 戻し

            '                Dim paramList_Return As New Hashtable

            '                paramList_Return.Add("SyuptDay", CType(rowChk("SYUPT_DAY"), Decimal))                              ' 出発日
            '                paramList_Return.Add("CrsCd", CType(rowChk("CRS_CD"), String))                                     ' コースコード
            '                paramList_Return.Add("Gousya", CType(rowChk("GOUSYA"), Decimal))                                   ' 号車
            '                paramList_Return.Add("systemUpdatePgmid", CType(rowChk("SYSTEM_UPDATE_PGMID"), String))            ' システム更新ＰＧＭＩＤ
            '                paramList_Return.Add("systemUpdatePersonCd", CType(rowChk("SYSTEM_UPDATE_PERSON_CD"), String))     ' システム更新者コード
            '                paramList_Return.Add("systemUpdateDay", CType(rowChk("SYSTEM_UPDATE_DAY"), Date))                  ' システム更新日
            '                paramList_Return.Add("BusReserveCd", CType(rowChk("BUS_RESERVE_CD"), String))                      ' バス指定コード

            '                getString = Nothing
            '                getString = executeReturnCrsData(paramList_Return)

            '                execNonQuery(trn, getString)

            '            Else    ' かつ 車番が変更されている

            ' コース台帳（基本） 更新
            getString = Nothing
            getString = executeUpdateCrsData(paramList)
            execNonQuery(trn, getString)
            Upflg = True

            '        End If

            '    End If

            'End If

            'Next

            '' 予約情報（基本）
            'For Each rowChk2 As DataRow In selectOldData_yoyaku.Rows

            '    ' 出発日、コースコード、号車 が同じ
            '    If paramList.Item("SyuptDay").ToString.Equals(rowChk2("SYUPT_DAY")) And
            '       paramList.Item("CrsCd").ToString.Equals(rowChk2("CRS_CD")) And
            '       paramList.Item("Gousya").ToString.Equals(rowChk2("GOUSYA")) Then

            '        ' かつ 使用中フラグ有りを対象
            '        If FixedCd.UsingFlg.Use.Equals(rowChk2("USING_FLG_AFTER")) Then

            '            If Upflg = True Then

            '                getString = Nothing
            '                getString = executeUpdateYoyakuData(paramList)

            '                execNonQuery(trn, getString)

            '            Else

            '                Dim paramList_Return As New Hashtable

            '                paramList_Return.Add("SyuptDay", CType(rowChk2("SYUPT_DAY"), Decimal))                              ' 出発日
            '                paramList_Return.Add("CrsCd", CType(rowChk2("CRS_CD"), String))                                     ' コースコード
            '                paramList_Return.Add("Gousya", CType(rowChk2("GOUSYA"), Decimal))                                   ' 号車
            '                paramList_Return.Add("systemUpdatePgmid", CType(rowChk2("SYSTEM_UPDATE_PGMID"), String))            ' システム更新ＰＧＭＩＤ
            '                paramList_Return.Add("systemUpdatePersonCd", CType(rowChk2("SYSTEM_UPDATE_PERSON_CD"), String))     ' システム更新者コード
            '                paramList_Return.Add("systemUpdateDay", CType(rowChk2("SYSTEM_UPDATE_DAY"), Date))                  ' システム更新日

            '                getString = Nothing
            '                getString = executeUpdateYoyakuData(paramList_Return)

            '                execNonQuery(trn, getString)

            '            End If


            '        End If

            '    End If

            'Next

            'コミット
            Call callCommitTransaction(trn)

        Catch ex As Exception
            'ロールバック
            Call callRollbackTransaction(trn)
            Throw
        Finally
            Call trn.Dispose()
        End Try

        Return True

    End Function

    ''' <summary>
    ''' コース台帳（基本）：更新
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateCrsData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        paramClear()

        With clsCrsLedgerBasicEntity

            'UPDATE
            sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine(" SET ")
            ' 車番
            If String.IsNullOrEmpty(paramList.Item("CarNo").ToString) Then
                sqlString.AppendLine(" CAR_NO = 0 ")
            Else
                sqlString.AppendLine(" CAR_NO = " & setParam(.carNo.PhysicsName, paramList.Item("CarNo"), .carNo.DBType, .carNo.IntegerBu, .carNo.DecimalBu))
            End If
            ' 車種コード
            sqlString.AppendLine(",CAR_TYPE_CD = " & setParam(.carTypeCd.PhysicsName, paramList.Item("carTypeCd"), .carTypeCd.DBType, .carTypeCd.IntegerBu, .carTypeCd.DecimalBu))
            ' 定員定
            sqlString.AppendLine(",CAPACITY_REGULAR = " & setParam(.capacityRegular.PhysicsName, paramList.Item("capacityRegular"), .capacityRegular.DBType, .capacityRegular.IntegerBu, .capacityRegular.DecimalBu))
            ' 定員補１階
            sqlString.AppendLine(",CAPACITY_HO_1KAI = " & setParam(.capacityHo1kai.PhysicsName, paramList.Item("capacityHo1kai"), .capacityHo1kai.DBType, .capacityHo1kai.IntegerBu, .capacityHo1kai.DecimalBu))
            ' 空席数定席
            sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam(.kusekiNumTeiseki.PhysicsName, paramList.Item("kusekiNumTeiseki"), .kusekiNumTeiseki.DBType, .kusekiNumTeiseki.IntegerBu, .kusekiNumTeiseki.DecimalBu))
            ' 空席数補助席
            sqlString.AppendLine(",KUSEKI_NUM_SUB_SEAT = " & setParam(.kusekiNumSubSeat.PhysicsName, paramList.Item("kusekiNumSubSeat"), .kusekiNumSubSeat.DBType, .kusekiNumSubSeat.IntegerBu, .kusekiNumSubSeat.DecimalBu))
            ' 予約可能数
            sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam(.yoyakuKanouNum.PhysicsName, paramList.Item("yoyakuKanouNum"), .yoyakuKanouNum.DBType, .yoyakuKanouNum.IntegerBu, .yoyakuKanouNum.DecimalBu))
            ' 営ブロック定
            sqlString.AppendLine(",EI_BLOCK_REGULAR = " & setParam(.eiBlockRegular.PhysicsName, paramList.Item("eiBlockRegular"), .eiBlockRegular.DBType, .eiBlockRegular.IntegerBu, .eiBlockRegular.DecimalBu))
            ' 営ブロック補
            sqlString.AppendLine(",EI_BLOCK_HO = " & setParam(.eiBlockHo.PhysicsName, paramList.Item("eiBlockHo"), .eiBlockHo.DBType, .eiBlockHo.IntegerBu, .eiBlockHo.DecimalBu))
            ' 共催運行区分
            sqlString.AppendLine(",KYOSAI_UNKOU_KBN = " & setParam(.kyosaiUnkouKbn.PhysicsName, paramList.Item("kyosaiUnkouKbn"), .kyosaiUnkouKbn.DBType, .kyosaiUnkouKbn.IntegerBu, .kyosaiUnkouKbn.DecimalBu))
            ' バス会社コード
            sqlString.AppendLine(",BUS_COMPANY_CD = " & setParam(.busCompanyCd.PhysicsName, SetHatobuscd, .busCompanyCd.DBType, .busCompanyCd.IntegerBu, .busCompanyCd.DecimalBu))
            ' 使用中フラグ 
            sqlString.AppendLine(",USING_FLG = NULL ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(.systemUpdatePgmid.PhysicsName, paramList.Item("systemUpdatePgmid"), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu))
            ' システム更新者コード
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item("systemUpdatePersonCd"), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu))
            ' システム更新日
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(.systemUpdateDay.PhysicsName, paramList.Item("systemUpdateDay"), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu))

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))     ' 出発日
            sqlString.AppendLine(" AND CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))                       ' コースコード
            sqlString.AppendLine(" AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))                  ' 号車

            Return sqlString.ToString

        End With

    End Function

    ''' <summary>
    ''' 予約情報（基本）（データ更新時に予約区分、予約ＮＯが取れない。データ更新用）
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateYoyakuData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder

        paramClear()

        With clsYoyakuInfoBasicEntity

            'UPDATE
            sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC ")
            sqlString.AppendLine(" SET ")
            ' 使用中フラグ 
            sqlString.AppendLine(" USING_FLG = NULL ")
            ' システム更新ＰＧＭＩＤ
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam(.systemUpdatePgmid.PhysicsName, paramList.Item("systemUpdatePgmid"), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu))
            ' システム更新者コード
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item("systemUpdatePersonCd"), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu))
            ' システム更新日
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam(.systemUpdateDay.PhysicsName, paramList.Item("systemUpdateDay"), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu))

            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine("     SYUPT_DAY = " & setParam(.syuptDay.PhysicsName, paramList.Item("SyuptDay"), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu))     ' 出発日
            sqlString.AppendLine(" AND CRS_CD = " & setParam(.crsCd.PhysicsName, paramList.Item("CrsCd"), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu))                       ' コースコード
            sqlString.AppendLine(" AND GOUSYA = " & setParam(.gousya.PhysicsName, paramList.Item("Gousya"), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu))                  ' 号車

            Return sqlString.ToString

        End With

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
