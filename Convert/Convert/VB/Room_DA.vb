Imports System.Text


''' <summary>
''' コース台帳一括修正（ルーム数等）のDAクラス
''' </summary>
Public Class Room_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getRoom                   '一覧結果取得検索
        executeInsertRoom         '登録
        executeUpdateRoom         '更新
        'executeReturnRoom         '戻し
    End Enum

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessRoomTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getRoom
                '一覧結果取得検索
                sqlString = getRoom(paramInfoList)
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
    Protected Overloads Function getRoom(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")      '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                                  'コースコード
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")                                   '曜日コード
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 ")                       '乗車地コード
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") '出発時間
        sqlString.AppendLine(",BASIC.GOUSYA ")                                                  '号車
        sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN ")                                 '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")                        '催行確定区分
        sqlString.AppendLine(",'' AS TEIKI_KIKAKU_KBN ")                                        '定期・企画区分
        sqlString.AppendLine(",NVL(BASIC.ONE_SANKA_FLG,'0') AS ONE_SANKA_FLG ")                 '１名参加フラグ
        sqlString.AppendLine(",NVL(BASIC.AIBEYA_USE_FLG,'0') AS AIBEYA_USE_FLG ")               '相部屋使用フラグ
        sqlString.AppendLine(",NVL(BASIC.TEIINSEI_FLG,'0') AS TEIINSEI_FLG ")                   '定員制フラグ
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,0) AS YOYAKU_NUM_TEISEKI ")         '予約数定席
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,0) AS YOYAKU_NUM_SUB_SEAT ")       '予約数補助席
        sqlString.AppendLine(",TO_CHAR(NVL(BASIC.CRS_BLOCK_CAPACITY,0),'99,999') CRS_BLOCK_CAPACITY ")         'コースブロック定員
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_ROOM_NUM,0) AS CRS_BLOCK_ROOM_NUM ")         'コースブロックルーム数
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_ONE_1R,0) AS CRS_BLOCK_ONE_1R ")             'コースブロック１名１R
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_TWO_1R,0) AS CRS_BLOCK_TWO_1R ")             'コースブロック２名１R
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_THREE_1R,0) AS CRS_BLOCK_THREE_1R ")         'コースブロック３名１R
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_FOUR_1R,0) AS CRS_BLOCK_FOUR_1R ")           'コースブロック４名１R
        sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_FIVE_1R,0) AS CRS_BLOCK_FIVE_1R ")           'コースブロック５名１R
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_SOKEI,0) AS ROOM_ZANSU_SOKEI ")             '部屋残数総計
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_ONE_ROOM,0) AS ROOM_ZANSU_ONE_ROOM ")       '部屋残数１人部屋
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_TWO_ROOM,0) AS ROOM_ZANSU_TWO_ROOM ")       '部屋残数２人部屋
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_THREE_ROOM,0) AS ROOM_ZANSU_THREE_ROOM ")   '部屋残数３人部屋
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_FOUR_ROOM,0) AS ROOM_ZANSU_FOUR_ROOM ")     '部屋残数４人部屋
        sqlString.AppendLine(",NVL(BASIC.ROOM_ZANSU_FIVE_ROOM,0) AS ROOM_ZANSU_FIVE_ROOM ")     '部屋残数５人部屋
        sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,0) AS JYOSYA_CAPACITY ")               '乗車定員
        sqlString.AppendLine(",NVL(BASIC.CAPACITY_HO_1KAI,0) AS CAPACITY_HO_1KAI ")             '定員補１階
        sqlString.AppendLine(",NVL(BASIC.CAPACITY_REGULAR,0) AS CAPACITY_REGULAR ")             '定員定
        sqlString.AppendLine(",NVL(BASIC.YOYAKU_KANOU_NUM,0) AS YOYAKU_KANOU_NUM ")             '予約可能数
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                                       'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")                                 'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")                                     'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",NVL(ADDINFO.UKETOME_KBN_ONE_1R,'0') AS UKETOME_KBN_ONE_1R ")     '受止区分１名１Ｒ
        sqlString.AppendLine(",NVL(ADDINFO.UKETOME_KBN_TWO_1R,'0') AS UKETOME_KBN_TWO_1R ")     '受止区分２名１Ｒ
        sqlString.AppendLine(",NVL(ADDINFO.UKETOME_KBN_THREE_1R,'0') AS UKETOME_KBN_THREE_1R ") '受止区分３名１Ｒ
        sqlString.AppendLine(",NVL(ADDINFO.UKETOME_KBN_FOUR_1R,'0') AS UKETOME_KBN_FOUR_1R ")   '受止区分４名１Ｒ
        sqlString.AppendLine(",NVL(ADDINFO.UKETOME_KBN_FIVE_1R,'0') AS UKETOME_KBN_FIVE_1R ")   '受止区分５名１Ｒ
        sqlString.AppendLine(",0 AS ROOMING_TOTAL ")                                            '予約済ルーム数
        sqlString.AppendLine(",YOYAKUINFOBASIC.ROOMING_BETU_NINZU_1 ")                          'SUM（ＲＯＯＭＩＮＧ別人数１）
        sqlString.AppendLine(",YOYAKUINFOBASIC.ROOMING_BETU_NINZU_2 ")                          'SUM（ＲＯＯＭＩＮＧ別人数２）
        sqlString.AppendLine(",YOYAKUINFOBASIC.ROOMING_BETU_NINZU_3 ")                          'SUM（ＲＯＯＭＩＮＧ別人数３）
        sqlString.AppendLine(",YOYAKUINFOBASIC.ROOMING_BETU_NINZU_4 ")                          'SUM（ＲＯＯＭＩＮＧ別人数４）
        sqlString.AppendLine(",YOYAKUINFOBASIC.ROOMING_BETU_NINZU_5 ")                          'SUM（ＲＯＯＭＩＮＧ別人数５）
        'sqlString.AppendLine(",NVL(HOTEL.SIIRE_SAKI_CD,'') AS SIIRE_SAKI_CD ")                  '仕入先コード
        'sqlString.AppendLine(",NVL(HOTEL.SIIRE_SAKI_EDABAN,'') AS SIIRE_SAKI_EDABAN ")          '仕入先枝番
        sqlString.AppendLine(",HOTELBASIC.ROOM_MAX_CAPACITY ")                                  'MIN（ROOMMAX定員）
        sqlString.AppendLine(",NVL(BASIC.AIBEYA_YOYAKU_NINZU_MALE,0) AS AIBEYA_YOYAKU_NINZU_MALE ")             '相部屋予約人数男性
        sqlString.AppendLine(",NVL(BASIC.AIBEYA_YOYAKU_NINZU_JYOSEI,0) AS AIBEYA_YOYAKU_NINZU_JYOSEI ")         '相部屋予約人数女性
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")                          '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                                        '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                                             '更新区分
        sqlString.AppendLine(",NVL(BASIC.BLOCK_KAKUHO_NUM,0) AS BLOCK_KAKUHO_NUM ")             'ブロック確保数
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_REGULAR,0) AS EI_BLOCK_REGULAR ")             '営ブロック定
        sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_HO,0) AS EI_BLOCK_HO ")                       '営ブロック補
        sqlString.AppendLine(",NVL(BASIC.KUSEKI_KAKUHO_NUM,0) AS KUSEKI_KAKUHO_NUM ")           '空席確保数

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & FixedCd.CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & FixedCd.CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & FixedCd.CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_ADD_INFO ADDINFO ON BASIC.SYUPT_DAY = ADDINFO.SYUPT_DAY AND BASIC.CRS_CD = ADDINFO.CRS_CD AND BASIC.GOUSYA = ADDINFO.GOUSYA ")
        'sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_HOTEL HOTEL ON BASIC.SYUPT_DAY = HOTEL.SYUPT_DAY AND BASIC.CRS_CD = HOTEL.CRS_CD AND BASIC.GOUSYA = HOTEL.GOUSYA ")
        'SUM（ＲＯＯＭＩＮＧ別人数１）～SUM（ＲＯＯＭＩＮＧ別人数５）
        sqlString.AppendLine(",(SELECT SUM(NVL(INFOBASIC.ROOMING_BETU_NINZU_1,0)) AS ROOMING_BETU_NINZU_1 ,SUM(NVL(INFOBASIC.ROOMING_BETU_NINZU_2, 0)) AS ROOMING_BETU_NINZU_2 ")
        sqlString.AppendLine(",SUM(NVL(INFOBASIC.ROOMING_BETU_NINZU_3, 0)) AS ROOMING_BETU_NINZU_3 ,SUM(NVL(INFOBASIC.ROOMING_BETU_NINZU_4, 0)) AS ROOMING_BETU_NINZU_4 ")
        sqlString.AppendLine(",SUM(NVL(INFOBASIC.ROOMING_BETU_NINZU_5, 0)) AS ROOMING_BETU_NINZU_5 ,BASIC.SYUPT_DAY AS SYUPT_DAY ,BASIC.CRS_CD AS CRS_CD ,BASIC.GOUSYA AS GOUSYA ")
        sqlString.AppendLine(" FROM T_CRS_LEDGER_BASIC BASIC LEFT JOIN T_YOYAKU_INFO_BASIC INFOBASIC ON BASIC.SYUPT_DAY = INFOBASIC.SYUPT_DAY AND BASIC.CRS_CD = INFOBASIC.CRS_CD AND BASIC.GOUSYA = INFOBASIC.GOUSYA ")
        sqlString.AppendLine(" AND (INFOBASIC.CANCEL_FLG = '' OR INFOBASIC.CANCEL_FLG IS NULL) AND (INFOBASIC.AIBEYA_FLG = '' OR INFOBASIC.AIBEYA_FLG IS NULL) ")
        sqlString.AppendLine(" WHERE BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        If Not String.IsNullOrEmpty(CType(paramList.Item("GOUSYA"), String)) Then
            sqlString.AppendLine(" AND BASIC.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        End If
        sqlString.AppendLine(" GROUP BY BASIC.SYUPT_DAY ,BASIC.CRS_CD, BASIC.GOUSYA) YOYAKUINFOBASIC ")
        'MIN（ROOMMAX定員）
        sqlString.AppendLine(",(SELECT MIN(NVL(ROOM_MAX_CAPACITY,0)) AS ROOM_MAX_CAPACITY ,BASIC.SYUPT_DAY AS SYUPT_DAY ,BASIC.CRS_CD AS CRS_CD ,BASIC.GOUSYA AS GOUSYA ")
        sqlString.AppendLine(" FROM M_SIIRE_SAKI SIIRESAKI LEFT JOIN T_CRS_LEDGER_HOTEL HOTEL ON SIIRESAKI.SIIRE_SAKI_CD = HOTEL.SIIRE_SAKI_CD AND SIIRESAKI.SIIRE_SAKI_NO = HOTEL.SIIRE_SAKI_EDABAN ")
        sqlString.AppendLine(" LEFT JOIN T_CRS_LEDGER_BASIC BASIC ON HOTEL.SYUPT_DAY = BASIC.SYUPT_DAY AND HOTEL.CRS_CD = BASIC.CRS_CD AND HOTEL.GOUSYA = BASIC.GOUSYA ")
        sqlString.AppendLine(" WHERE BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        If Not String.IsNullOrEmpty(CType(paramList.Item("GOUSYA"), String)) Then
            sqlString.AppendLine(" AND BASIC.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        End If
        sqlString.AppendLine(" GROUP BY BASIC.SYUPT_DAY ,BASIC.CRS_CD, BASIC.GOUSYA) HOTELBASIC ")
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
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = YOYAKUINFOBASIC.SYUPT_DAY ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = YOYAKUINFOBASIC.CRS_CD ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = YOYAKUINFOBASIC.GOUSYA ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")
        sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ")
        sqlString.AppendLine(" ,BASIC.GOUSYA ")

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
    Public Function executeRoomTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateRoom
                    For i As Integer = 1 To 2
                        Select Case i
                            Case 1  'コース台帳（基本）
                                sqlString = executeUpdateBasicData(paramInfoList)
                            Case 2  'コース台帳（付加情報）
                                sqlString = executeUpdateAddInfoData(paramInfoList)
                        End Select

                        returnValue += execNonQuery(oracleTransaction, sqlString)
                    Next
                    'Case accessType.executeReturnRoom
                    '    sqlString = executeReturnBasicData(paramInfoList)

                    '    returnValue += execNonQuery(oracleTransaction, sqlString)
            End Select

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
        sqlString.AppendLine(" ROOM_ZANSU_SOKEI = " & setParam("ROOM_ZANSU_SOKEI", paramList.Item("ROOM_ZANSU_SOKEI"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ROOM_ZANSU_ONE_ROOM = " & setParam("ROOM_ZANSU_ONE_ROOM", paramList.Item("ROOM_ZANSU_ONE_ROOM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ROOM_ZANSU_TWO_ROOM = " & setParam("ROOM_ZANSU_TWO_ROOM", paramList.Item("ROOM_ZANSU_TWO_ROOM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ROOM_ZANSU_THREE_ROOM = " & setParam("ROOM_ZANSU_THREE_ROOM", paramList.Item("ROOM_ZANSU_THREE_ROOM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ROOM_ZANSU_FOUR_ROOM = " & setParam("ROOM_ZANSU_FOUR_ROOM", paramList.Item("ROOM_ZANSU_FOUR_ROOM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",ROOM_ZANSU_FIVE_ROOM = " & setParam("ROOM_ZANSU_FIVE_ROOM", paramList.Item("ROOM_ZANSU_FIVE_ROOM"), OracleDbType.Decimal, 3, 0))
        If CType(paramList.Item("CRS_BLOCK_CAPACITY"), Integer) <> 0 Then
            sqlString.AppendLine(",JYOSYA_CAPACITY = " & setParam("JYOSYA_CAPACITY", paramList.Item("JYOSYA_CAPACITY"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",YOYAKU_KANOU_NUM = " & setParam("YOYAKU_KANOU_NUM", paramList.Item("YOYAKU_KANOU_NUM"), OracleDbType.Decimal, 3, 0))
            sqlString.AppendLine(",KUSEKI_NUM_TEISEKI = " & setParam("KUSEKI_NUM_TEISEKI", paramList.Item("KUSEKI_NUM_TEISEKI"), OracleDbType.Decimal, 3, 0))
        End If
        sqlString.AppendLine(",ONE_SANKA_FLG = " & setParam("ONE_SANKA_FLG", paramList.Item("ONE_SANKA_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",AIBEYA_USE_FLG = " & setParam("AIBEYA_USE_FLG", paramList.Item("AIBEYA_USE_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",TEIINSEI_FLG = " & setParam("TEIINSEI_FLG", paramList.Item("TEIINSEI_FLG"), OracleDbType.Char))
        sqlString.AppendLine(",CRS_BLOCK_CAPACITY = " & setParam("CRS_BLOCK_CAPACITY", paramList.Item("CRS_BLOCK_CAPACITY"), OracleDbType.Decimal, 5, 0))
        sqlString.AppendLine(",CRS_BLOCK_ROOM_NUM = " & setParam("CRS_BLOCK_ROOM_NUM", paramList.Item("CRS_BLOCK_ROOM_NUM"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",CRS_BLOCK_ONE_1R = " & setParam("CRS_BLOCK_ONE_1R", paramList.Item("CRS_BLOCK_ONE_1R"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",CRS_BLOCK_TWO_1R = " & setParam("CRS_BLOCK_TWO_1R", paramList.Item("CRS_BLOCK_TWO_1R"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",CRS_BLOCK_THREE_1R = " & setParam("CRS_BLOCK_THREE_1R", paramList.Item("CRS_BLOCK_THREE_1R"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",CRS_BLOCK_FOUR_1R = " & setParam("CRS_BLOCK_FOUR_1R", paramList.Item("CRS_BLOCK_FOUR_1R"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(",CRS_BLOCK_FIVE_1R = " & setParam("CRS_BLOCK_FIVE_1R", paramList.Item("CRS_BLOCK_FIVE_1R"), OracleDbType.Decimal, 3, 0))
        'sqlString.AppendLine(",USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
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
    ''' コース台帳（付加情報）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeUpdateAddInfoData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_ADD_INFO ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" UKETOME_KBN_ONE_1R = " & setParam("UKETOME_KBN_ONE_1R", paramList.Item("UKETOME_KBN_ONE_1R"), OracleDbType.Char))
        sqlString.AppendLine(",UKETOME_KBN_TWO_1R = " & setParam("UKETOME_KBN_TWO_1R", paramList.Item("UKETOME_KBN_TWO_1R"), OracleDbType.Char))
        sqlString.AppendLine(",UKETOME_KBN_THREE_1R = " & setParam("UKETOME_KBN_THREE_1R", paramList.Item("UKETOME_KBN_THREE_1R"), OracleDbType.Char))
        sqlString.AppendLine(",UKETOME_KBN_FOUR_1R = " & setParam("UKETOME_KBN_FOUR_1R", paramList.Item("UKETOME_KBN_FOUR_1R"), OracleDbType.Char))
        sqlString.AppendLine(",UKETOME_KBN_FIVE_1R = " & setParam("UKETOME_KBN_FIVE_1R", paramList.Item("UKETOME_KBN_FIVE_1R"), OracleDbType.Char))
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

#End Region


End Class
