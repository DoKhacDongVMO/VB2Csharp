Imports System.Text


''' <summary>
''' 催行可否照会／入力のDAクラス
''' </summary>
Public Class SaikouKahiInput_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getSaikouKahiInput                   '一覧結果取得検索
        getUsingFlgCrs                       '使用中フラグチェック用検索
        getSaikouKakuteiInput                '催行確定区分チェック用検索
        getUsingFlgData                      '使用中フラグ対象取得用
    End Enum

    Private tejimaiSumi As String = "Y"      '手仕舞い済

    Public Structure USINGFLG_PARAMKEY
        Public Const WHEREIND As String = "WHEREIN"
    End Structure



#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessSaikouKahiInputTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable

        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getSaikouKahiInput
                '一覧結果取得検索
                sqlString = getSaikouKahiInput(paramInfoList)
            Case accessType.getUsingFlgCrs
                '使用中フラグチェック用検索
                sqlString = getUsingFlgCrs(paramInfoList)
            Case accessType.getSaikouKakuteiInput
                '催行確定区分チェック用検索
                sqlString = getSaikouKakuteiInput(paramInfoList)
            Case accessType.getUsingFlgData
                '使用中フラグ対象用検索
                sqlString = getUsingFlgData(paramInfoList)
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
    Protected Overloads Function getSaikouKahiInput(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")  '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                              'コースコード
        sqlString.AppendLine(",BASIC.CRS_NAME ")                                            'コース名
        sqlString.AppendLine(",BASIC.YOBI_CD ")                                             '曜日コード
        sqlString.AppendLine(",0 AS CHK_FLG ")                                              '変更フラグ
        sqlString.AppendLine(",Case WHEN BASIC.SAIKOU_KAKUTEI_KBN = ' ' THEN ")
        sqlString.AppendLine("'' ")
        sqlString.AppendLine("   WHEN BASIC.SAIKOU_KAKUTEI_KBN = " & " '" & FixedCd.SaikouKakuteiKbn.Saikou & "' " & " THEN ")
        sqlString.AppendLine("'" & FixedCd.Saikoukahi.Saikou & "' ")
        sqlString.AppendLine("   WHEN BASIC.SAIKOU_KAKUTEI_KBN = " & " '" & FixedCd.SaikouKakuteiKbn.Tyushi & "' " & " THEN ")
        sqlString.AppendLine("'" & FixedCd.Saikoukahi.Tyushi & "' ")
        sqlString.AppendLine("   Else ' ' END AS SAIKOU_KAKUTEI_KBN ")                      '催行確定区分
        sqlString.AppendLine(",BASIC.TEJIMAI_KBN ")                                         '手仕舞区分
        sqlString.AppendLine(",BASIC.MIN_SAIKOU_NINZU ")                                    '最小催行人数
        sqlString.AppendLine(",CRSLEDGERBASIC.KUSEKI_NUM_KEI ")                             '空席数
        sqlString.AppendLine(",CRSLEDGERBASIC.YOYAKU_NUM_KEI ")                             '予約数
        sqlString.AppendLine(",CRSLEDGERBASIC.ROOM_ZANSU_SOKEI ")                           '部屋残数総計
        sqlString.AppendLine(",CRSLEDGERBASIC.YOYAKU_ALREADY_ROOM_NUM ")                    '予約済ＲＯＯＭ数
        sqlString.AppendLine(",BASIC.BUS_RESERVE_CD ")                                      'バス指定コード
        sqlString.AppendLine(",CRSLEDGERBASIC.BUS_COUNT_FLG ")                              '台数カウントフラグ
        sqlString.AppendLine(",BASIC.MARU_ZOU_MANAGEMENT_KBN ")                             '〇増管理区分
        sqlString.AppendLine(",BASIC.TEJIMAI_CONTACT_KBN ")                                 '手仕舞連絡区分
        sqlString.AppendLine(",BASIC.UNKYU_KBN ")                                           '運休区分
        sqlString.AppendLine(",'詳細' AS MORE_DETAIL ")                                     '詳細ボタン
        sqlString.AppendLine(",' ' AS USING_FLG ")                                          '使用中フラグ
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ")                                    '定期・企画区分
        sqlString.AppendLine(",BASIC.HOUJIN_GAIKYAKU_KBN ")                                 '邦人／外客区分
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                                    '変更可否区分
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine(",(SELECT SUM(NVL(CRSBASIC.KUSEKI_NUM_TEISEKI,0) + NVL(CRSBASIC.KUSEKI_NUM_SUB_SEAT,0)) AS KUSEKI_NUM_KEI ,SUM(NVL(CRSBASIC.YOYAKU_NUM_TEISEKI, 0) + NVL(CRSBASIC.YOYAKU_NUM_SUB_SEAT, 0)) AS YOYAKU_NUM_KEI ")
        sqlString.AppendLine(",SUM(NVL(CRSBASIC.ROOM_ZANSU_SOKEI, 0)) AS ROOM_ZANSU_SOKEI ,SUM(NVL(CRSBASIC.YOYAKU_ALREADY_ROOM_NUM, 0)) AS YOYAKU_ALREADY_ROOM_NUM ")
        sqlString.AppendLine(",COUNT(CRSBASIC.BUS_COUNT_FLG) AS BUS_COUNT_FLG ,CRSBASIC.SYUPT_DAY AS SYUPT_DAY ,CRSBASIC.CRS_CD AS CRS_CD ")
        sqlString.AppendLine(" FROM T_CRS_LEDGER_BASIC CRSBASIC ")
        sqlString.AppendLine(" WHERE ")
        If CType(paramList.Item("SYUPT_DAY_FROM"), String) <> String.Empty AndAlso CType(paramList.Item("SYUPT_DAY_TO"), String) = String.Empty Then
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY_FROM"), OracleDbType.Decimal, 8, 0))
        ElseIf CType(paramList.Item("SYUPT_DAY_FROM"), String) = String.Empty AndAlso CType(paramList.Item("SYUPT_DAY_TO"), String) <> String.Empty Then
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY_TO"), OracleDbType.Decimal, 8, 0))
        Else
            sqlString.AppendLine(" CRSBASIC.SYUPT_DAY between " & setParam("SYUPT_DAY_FROM", paramList.Item("SYUPT_DAY_FROM"), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine(" AND " & setParam("SYUPT_DAY_TO", paramList.Item("SYUPT_DAY_TO"), OracleDbType.Decimal, 8, 0))
        End If
        If CType(paramList.Item("CRS_CD"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" ( ")
            sqlString.AppendLine(" CRSBASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            'sqlString.AppendLine(" OR ")
            'sqlString.AppendLine(" CRSBASIC.BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            'sqlString.AppendLine(" ) ")
        End If
        If CType(paramList.Item("CRS_NAME_KANA"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" CRSBASIC.CRS_NAME_KANA LIKE '%'||" & setParam("CRS_NAME_KANA", paramList.Item("CRS_NAME_KANA"), OracleDbType.Varchar2) & "||'%'")
        End If
        If CType(paramList.Item("BUS_RESERVE_CD"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" CRSBASIC.BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        End If
        Select Case CType(paramList.Item("SAIKOU_KAKUTEI_KBN"), String)
            Case String.Empty
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" (CRSBASIC.SAIKOU_KAKUTEI_KBN <> '" & FixedCd.SaikouKakuteiKbn.Haishi & "'")
                sqlString.AppendLine(" OR ")
                sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN Is NULL) ")
            Case FixedCd.SaikouKakuteiKbn.Saikou
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
            Case FixedCd.SaikouKakuteiKbn.Tyushi
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" CRSBASIC.SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
            Case Else
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" NVL(CRSBASIC.SAIKOU_KAKUTEI_KBN, '*') NOT IN ('" & FixedCd.SaikouKakuteiKbn.Tyushi & "', '" & FixedCd.SaikouKakuteiKbn.Haishi & "', '" & FixedCd.SaikouKakuteiKbn.Saikou & "') ")
        End Select
        Select Case CType(paramList.Item("TEJIMAI_KBN"), String)
            Case String.Empty
            Case tejimaiSumi
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" CRSBASIC.TEJIMAI_KBN = " & setParam("TEJIMAI_KBN", paramList.Item("TEJIMAI_KBN"), OracleDbType.Char))
            Case CType(FixedCd.TejimaiKbn.Mi, String)
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" CRSBASIC.TEJIMAI_KBN IS NULL ")
        End Select
        If CType(paramList.Item("HOUJIN_KBN"), String) <> String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN IN (" & setParam("HOUJIN_KBN", paramList.Item("HOUJIN_KBN"), OracleDbType.Char) & "," & setParam("GAIKYAKU_KBN", paramList.Item("GAIKYAKU_KBN"), OracleDbType.Char) & ")")
        ElseIf CType(paramList.Item("HOUJIN_KBN"), String) = String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("GAIKYAKU_KBN", paramList.Item("GAIKYAKU_KBN"), OracleDbType.Char))
        ElseIf CType(paramList.Item("HOUJIN_KBN"), String) <> String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) = String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" CRSBASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_KBN", paramList.Item("HOUJIN_KBN"), OracleDbType.Char))
        End If
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" (CRSBASIC.MARU_ZOU_MANAGEMENT_KBN = '" & String.Empty & "'")
        sqlString.AppendLine(" OR ")
        sqlString.AppendLine(" CRSBASIC.MARU_ZOU_MANAGEMENT_KBN Is NULL) ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(CRSBASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" AND ")
        'sqlString.AppendLine(" (( CRSBASIC.CRS_CD = CRSBASIC.BUS_RESERVE_CD And ")
        sqlString.AppendLine("CRSBASIC.CRS_KIND IN ('" & FixedCd.CrsKindType.higaeri & "','" & FixedCd.CrsKindType.stay & "','" & FixedCd.CrsKindType.rcourse & "')")
        'sqlString.AppendLine(" ) OR ")
        'sqlString.AppendLine(" CRSBASIC.CRS_CD <> CRSBASIC.BUS_RESERVE_CD ")
        'sqlString.AppendLine(" ) ")
        'If CType(paramList.Item("BUS_RESERVE_CD"), String) <> String.Empty Then
        '    sqlString.AppendLine(" AND CRSBASIC.BUS_COUNT_FLG = '" & FixedCd.BusCountFlg.Count & "'")
        'End If
        sqlString.AppendLine(" AND NVL(CRSBASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" GROUP BY CRSBASIC.SYUPT_DAY , CRSBASIC.CRS_CD) CRSLEDGERBASIC ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        If CType(paramList.Item("SYUPT_DAY_FROM"), String) <> String.Empty AndAlso CType(paramList.Item("SYUPT_DAY_TO"), String) = String.Empty Then
            sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY_FROM"), OracleDbType.Decimal, 8, 0))
        ElseIf CType(paramList.Item("SYUPT_DAY_FROM"), String) = String.Empty AndAlso CType(paramList.Item("SYUPT_DAY_TO"), String) <> String.Empty Then
            sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY_TO"), OracleDbType.Decimal, 8, 0))
        Else
            sqlString.AppendLine(" BASIC.SYUPT_DAY between " & setParam("SYUPT_DAY_FROM", paramList.Item("SYUPT_DAY_FROM"), OracleDbType.Decimal, 8, 0))
            sqlString.AppendLine(" AND " & setParam("SYUPT_DAY_TO", paramList.Item("SYUPT_DAY_TO"), OracleDbType.Decimal, 8, 0))
        End If
        If CType(paramList.Item("CRS_CD"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" ( ")
            sqlString.AppendLine(" BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            'sqlString.AppendLine(" OR ( ")
            'sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            'sqlString.AppendLine(" AND ")
            'sqlString.AppendLine(" BASIC.CRS_CD <> " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
            'sqlString.AppendLine(" )) ")
        End If
        If CType(paramList.Item("CRS_NAME_KANA"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.CRS_NAME_KANA  LIKE '%'||" & setParam("CRS_NAME_KANA", paramList.Item("CRS_NAME_KANA"), OracleDbType.Varchar2) & "||'%'")
        End If
        If CType(paramList.Item("BUS_RESERVE_CD"), String) <> String.Empty Then
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        End If
        Select Case CType(paramList.Item("SAIKOU_KAKUTEI_KBN"), String)
            Case String.Empty
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" (BASIC.SAIKOU_KAKUTEI_KBN <> '" & FixedCd.SaikouKakuteiKbn.Haishi & "'")
                sqlString.AppendLine(" OR ")
                sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN IS NULL) ")
            Case FixedCd.SaikouKakuteiKbn.Saikou
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
            Case FixedCd.SaikouKakuteiKbn.Tyushi
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
            Case Else
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" NVL(BASIC.SAIKOU_KAKUTEI_KBN, '*') NOT IN ('" & FixedCd.SaikouKakuteiKbn.Tyushi & "', '" & FixedCd.SaikouKakuteiKbn.Haishi & "', '" & FixedCd.SaikouKakuteiKbn.Saikou & "') ")
        End Select
        Select Case CType(paramList.Item("TEJIMAI_KBN"), String)
            Case String.Empty
            Case tejimaiSumi
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" BASIC.TEJIMAI_KBN = " & setParam("TEJIMAI_KBN", paramList.Item("TEJIMAI_KBN"), OracleDbType.Char))
            Case CType(FixedCd.TejimaiKbn.Mi, String)
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" BASIC.TEJIMAI_KBN IS NULL ")
        End Select
        If CType(paramList.Item("HOUJIN_KBN"), String) <> String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN IN (" & setParam("HOUJIN_KBN", paramList.Item("HOUJIN_KBN"), OracleDbType.Char) & "," & setParam("GAIKYAKU_KBN", paramList.Item("GAIKYAKU_KBN"), OracleDbType.Char) & ")")
        ElseIf CType(paramList.Item("HOUJIN_KBN"), String) = String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) <> String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("GAIKYAKU_KBN", paramList.Item("GAIKYAKU_KBN"), OracleDbType.Char))
        ElseIf CType(paramList.Item("HOUJIN_KBN"), String) <> String.Empty AndAlso CType(paramList.Item("GAIKYAKU_KBN"), String) = String.Empty Then
            sqlString.AppendLine(" AND")
            sqlString.AppendLine(" BASIC.HOUJIN_GAIKYAKU_KBN = " & setParam("HOUJIN_KBN", paramList.Item("HOUJIN_KBN"), OracleDbType.Char))
        End If
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" (BASIC.MARU_ZOU_MANAGEMENT_KBN = '" & String.Empty & "'")
        sqlString.AppendLine(" OR ")
        sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN Is NULL) ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
        sqlString.AppendLine(" AND ")
        'sqlString.AppendLine(" (( BASIC.CRS_CD = BASIC.BUS_RESERVE_CD And")
        sqlString.AppendLine(" BASIC.CRS_KIND IN ('" & FixedCd.CrsKindType.higaeri & "','" & FixedCd.CrsKindType.stay & "','" & FixedCd.CrsKindType.rcourse & "')")
        'sqlString.AppendLine(" ) OR ")
        'sqlString.AppendLine(" BASIC.CRS_CD <> BASIC.BUS_RESERVE_CD ")
        'sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = CRSLEDGERBASIC.SYUPT_DAY ")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = CRSLEDGERBASIC.CRS_CD ")
        'GROUP BY句
        sqlString.AppendLine(" GROUP BY ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ,BASIC.CRS_CD ,BASIC.CRS_NAME ,BASIC.YOBI_CD ,BASIC.SAIKOU_KAKUTEI_KBN ")
        sqlString.AppendLine(" ,BASIC.TEJIMAI_KBN ,BASIC.MIN_SAIKOU_NINZU ,CRSLEDGERBASIC.KUSEKI_NUM_KEI ,CRSLEDGERBASIC.YOYAKU_NUM_KEI ")
        sqlString.AppendLine(" ,CRSLEDGERBASIC.ROOM_ZANSU_SOKEI ,CRSLEDGERBASIC.YOYAKU_ALREADY_ROOM_NUM ,BASIC.BUS_RESERVE_CD ,CRSLEDGERBASIC.BUS_COUNT_FLG ")
        sqlString.AppendLine(" ,BASIC.MARU_ZOU_MANAGEMENT_KBN ,BASIC.TEJIMAI_CONTACT_KBN ,BASIC.UNKYU_KBN ,BASIC.TEIKI_KIKAKU_KBN ")
        sqlString.AppendLine(" ,BASIC.HOUJIN_GAIKYAKU_KBN ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")
        sqlString.AppendLine(" ,BASIC.CRS_CD ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getUsingFlgCrs(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY ")   '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")      'コースコード
        sqlString.AppendLine(",BASIC.GOUSYA ")      '号車
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" And ")
        sqlString.AppendLine(" BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" And ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" BASIC.GOUSYA ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getSaikouKakuteiInput(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")  '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                              'コースコード
        sqlString.AppendLine(",BASIC.SAIKOU_KAKUTEI_KBN ")                                  '催行確定区分
        sqlString.AppendLine(",BASIC.TEJIMAI_KBN ")                                         '手仕舞区分
        sqlString.AppendLine(",BASIC.BUS_RESERVE_CD ")                                      'バス指定コード
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC BASIC ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.BUS_RESERVE_CD = " & setParam("BUS_RESERVE_CD", paramList.Item("BUS_RESERVE_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD <> " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.TEJIMAI_KBN = '" & FixedCd.TejimaiKbn.Zumi & "'")
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL( BASIC.DELETE_DAY,0) = 0 ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getUsingFlgData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine(" SELECT ")
        sqlString.AppendLine("   SYUPT_DAY ")                   '出発日
        sqlString.AppendLine(" , CRS_CD ")                      'コースコード
        sqlString.AppendLine(" , GOUSYA ")                      '号車
        sqlString.AppendLine(" , BUS_RESERVE_CD ")              'バス指定コード
        sqlString.AppendLine(" , SYSTEM_UPDATE_DAY ")           'システム更新日
        sqlString.AppendLine(" , SYSTEM_UPDATE_PGMID ")         'システム更新PGMID
        sqlString.AppendLine(" , SYSTEM_UPDATE_PERSON_CD ")     'システム更新者
        sqlString.AppendLine(" , USING_FLG ")                   '使用中フラグ

        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_BASIC ")

        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" (SYUPT_DAY, CRS_CD) IN (")
        sqlString.AppendLine("  " & CType(paramList.Item(USINGFLG_PARAMKEY.WHEREIND), String))
        sqlString.AppendLine(" )")
        '削除済みは除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY, 0) = 0 ")
        '○増は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" MARU_ZOU_MANAGEMENT_KBN IS NULL ")
        '定期は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" TEIKI_KIKAKU_KBN = '" & CStr(TeikiKikakuKbn.Kikaku) & "' ")
        '廃止は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(SAIKOU_KAKUTEI_KBN, '*') <> '" & FixedCd.SaikouKakuteiKbn.Haishi & "'")

        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine("   SYUPT_DAY, CRS_CD, GOUSYA ")

        Return sqlString.ToString
    End Function

#End Region

#Region " UPDATE処理 "

    ''' <summary>
    ''' DB接続用
    ''' </summary>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function updateSaikouKahi(ByVal paramInfoList As Hashtable) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            For i As Integer = 1 To 2
                Select Case i
                    Case 1  'コース台帳（基本）
                        sqlString = executeUpdateBasicData(paramInfoList)
                    Case 2  '催行決定コース履歴
                        sqlString = executeInsertCrsHistoryData(paramInfoList)
                End Select

                returnValue += execNonQuery(oracleTransaction, sqlString)
            Next

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
    ''' <param name="selectData"></param>
    ''' <param name="systemupdatepgmid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function executeUsingFlgCrs(ByVal selectData As DataTable, ByVal systemupdatepgmid As String) As DataTable

        Dim totalValue As New DataTable
        Dim trn As OracleTransaction = Nothing
        Dim returnValue As Boolean = False

        Try

            totalValue.Columns.Add("USING_FLG")             '使用中フラグ

            'トランザクション開始
            trn = callBeginTransaction()

            For Each row As DataRow In selectData.Rows
                Dim syuptsday As String = CType(row("SYUPT_DAY"), String)                   '出発日
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
    Private Function executeUpdateBasicData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'UPDATE
        sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
        sqlString.AppendLine(" SET ")
        sqlString.AppendLine(" SAIKOU_KAKUTEI_KBN = " & setParam("SAIKOU_KAKUTEI_KBN", paramList.Item("SAIKOU_KAKUTEI_KBN"), OracleDbType.Char))
        sqlString.AppendLine(",SAIKOU_DAY = " & setParam("SAIKOU_DAY", paramList.Item("SAIKOU_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(",TEJIMAI_KBN = " & setParam("TEJIMAI_KBN", paramList.Item("TEJIMAI_KBN"), OracleDbType.Char))
        sqlString.AppendLine(",TEJIMAI_DAY = " & setParam("TEJIMAI_DAY", paramList.Item("TEJIMAI_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        '削除済みは除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(DELETE_DAY, 0) = 0 ")
        '○増は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" MARU_ZOU_MANAGEMENT_KBN IS NULL ")
        '定期は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" TEIKI_KIKAKU_KBN = '" & CStr(TeikiKikakuKbn.Kikaku) & "' ")
        '廃止は除く
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(SAIKOU_KAKUTEI_KBN, '*') <> '" & FixedCd.SaikouKakuteiKbn.Haishi & "'")

        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 催行決定コース履歴：データ登録用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertCrsHistoryData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'INSERT
        sqlString.AppendLine(" INSERT INTO T_SAIKOU_DECISION_CRS_HISTORY ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(" CRS_CD ")
        sqlString.AppendLine(",CRS_NAME ")
        sqlString.AppendLine(",SYUPT_DAY ")
        sqlString.AppendLine(",SAIKOU_KAKUTEI_KBN ")
        sqlString.AppendLine(",TEJIMAI_KBN ")
        sqlString.AppendLine(",TEJIMAI_CONTACT_KBN ")
        sqlString.AppendLine(",MARU_ZOU_MANAGEMENT_KBN ")
        sqlString.AppendLine(",UNKYU_KBN ")
        sqlString.AppendLine(",DELETE_DAY ")
        sqlString.AppendLine(",SYSTEM_ENTRY_DAY ")
        sqlString.AppendLine(",SYSTEM_ENTRY_PERSON_CD ")
        sqlString.AppendLine(",SYSTEM_ENTRY_PGMID ")
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ")
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ")
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine(" ) ")
        'VALUE
        sqlString.AppendLine(" VALUES ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(setParam("CRS_CD", CType(paramList.Item("CRS_CD"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("CRS_NAME", CType(paramList.Item("CRS_NAME"), String), OracleDbType.Varchar2))
        sqlString.AppendLine("," & setParam("SYUPT_DAY", CType(paramList.Item("SYUPT_DAY"), Decimal), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine("," & setParam("SAIKOU_KAKUTEI_KBN", CType(paramList.Item("SAIKOU_KAKUTEI_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("TEJIMAI_KBN", CType(paramList.Item("TEJIMAI_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("TEJIMAI_CONTACT_KBN", CType(paramList.Item("TEJIMAI_CONTACT_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("MARU_ZOU_MANAGEMENT_KBN", CType(paramList.Item("MARU_ZOU_MANAGEMENT_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("UNKYU_KBN", CType(paramList.Item("UNKYU_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("DELETE_DAY", CType(paramList.Item("DELETE_DAY"), Decimal), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine("," & setParam("SYSTEM_ENTRY_DAY", CType(paramList.Item("SYSTEM_ENTRY_DAY"), Date), OracleDbType.Date))
        sqlString.AppendLine("," & setParam("SYSTEM_ENTRY_PERSON_CD", CType(paramList.Item("SYSTEM_ENTRY_PERSON_CD"), String), OracleDbType.Varchar2))
        sqlString.AppendLine("," & setParam("SYSTEM_ENTRY_PGMID", CType(paramList.Item("SYSTEM_ENTRY_PGMID"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine("," & setParam("SYSTEM_UPDATE_PERSON_CD", CType(paramList.Item("SYSTEM_UPDATE_PERSON_CD"), String), OracleDbType.Varchar2))
        sqlString.AppendLine("," & setParam("SYSTEM_UPDATE_PGMID", CType(paramList.Item("SYSTEM_UPDATE_PGMID"), String), OracleDbType.Char))
        sqlString.AppendLine(" ) ")

        Return sqlString.ToString

    End Function

#End Region


End Class
