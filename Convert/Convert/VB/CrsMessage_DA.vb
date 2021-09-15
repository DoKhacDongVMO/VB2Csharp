Imports System.Text

''' <summary>
''' コース台帳一括修正（メッセージ）のDAクラス
''' </summary>
Public Class CrsMessage_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Private Edaban As String = "※"                                     '枝番判定

    Public Enum accessType As Integer
        getCrsMessage                      '一覧結果取得検索
        executeInsertCrsMessage            '登録
        executeUpdateCrsMessage            '更新
        executeDeleteCrsMessage            '削除
    End Enum

#End Region

#Region " マスタテーブル読み込み "

    ''' <summary>
    ''' コードマスタより名称データを取得する
    ''' </summary>
    ''' <param name="cdBunrui"></param>
    ''' <param name="cdValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCdMaster(ByVal cdBunrui As CdBunruiType, ByVal cdValue As String) As String

        Dim resultDataTable As New DataTable  'resultDataTable
        Dim sqlString As New StringBuilder
        Dim retStr As String  'retStr

        Try
            sqlString.AppendLine("SELECT CODE_NAME FROM M_CODE")
            sqlString.AppendLine(" WHERE")
            sqlString.AppendLine(" CODE_BUNRUI = " & setParam("CODE_BUNRUI", CommonType_MojiColValue.CdBunruiType_Value(cdBunrui), OracleDbType.Varchar2, 3))
            sqlString.AppendLine(" AND CODE_VALUE = " & setParam("CODE_VALUE", cdValue, OracleDbType.Varchar2, 15))
            sqlString.AppendLine(" AND DELETE_DATE IS NULL")

            resultDataTable = MyBase.getDataTable(sqlString.ToString)
        Catch ex As Exception
            Throw
        End Try

        retStr = ""
        If resultDataTable.Rows.Count > 0 Then
            If IsDBNull(resultDataTable(0).Item(0)) = False Then
                retStr = CStr(resultDataTable(0).Item(0))
            End If
        End If

        Return retStr

    End Function

#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessCrsMessageTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getCrsMessage
                '一覧結果取得検索
                sqlString = getCrsMessage(paramInfoList)
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
    Protected Overloads Function getCrsMessage(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("SYUPT_DAY ")                                      '出発日
        sqlString.AppendLine(",CRS_CD ")                                        'コースコード
        sqlString.AppendLine(",YOBI_CD ")                                       '曜日コード
        sqlString.AppendLine(",HAISYA_KEIYU_CD_1 ")                             '乗車地コード
        sqlString.AppendLine(",SYUPT_TIME_1 ")                                  '出発時間
        sqlString.AppendLine(",GOUSYA ")                                        '号車
        sqlString.AppendLine(",UNKYU_KBN ")                                     '運休区分
        sqlString.AppendLine(",SAIKOU_KAKUTEI_KBN ")                            '催行確定区分
        sqlString.AppendLine(",TEIKI_KIKAKU_KBN ")                              '定期・企画区分
        sqlString.AppendLine(",TO_CHAR(LINE_NO) AS LINE_NO ")                   '行ＮＯ
        sqlString.AppendLine(",CRS_MESSAGE ")                                   'コード
        sqlString.AppendLine(",MESSAGE_KBN ")                                   '注意事項区分/メッセージ区分
        sqlString.AppendLine(",MESSAGE ")                                       '注意事項/メッセージ
        sqlString.AppendLine(",EIBUN_MESSAGE ")                                 '英文メッセージ
        sqlString.AppendLine(",DELETE_DAY ")                                    '削除日
        sqlString.AppendLine(",SYSTEM_ENTRY_DAY ")                              'システム登録日
        sqlString.AppendLine(",SYSTEM_ENTRY_PERSON_CD ")                        'システム登録者コード
        sqlString.AppendLine(",SYSTEM_ENTRY_PGMID ")                            'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ")                             'システム更新日
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ")                       'システム更新者コード
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ")                           'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",SYUPT_DAY2 ")                                    '出発日2
        sqlString.AppendLine(",HAISYA_KEIYU_CD_12 ")                            '乗車地コード2
        sqlString.AppendLine(",GOUSYA2 ")                                       '号車2
        sqlString.AppendLine(",SYUPT_TIME_12 ")                                 '出発時間2
        sqlString.AppendLine(",USING_FLG ")                                     '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                        '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                             '更新区分
        sqlString.AppendLine("FROM( ")
        sqlString.AppendLine("SELECT TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")    '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")                                  'コースコード
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")                   '曜日コード
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 ")       '乗車地コード
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") '出発時間
        sqlString.AppendLine(",BASIC.GOUSYA ")                                  '号車
        sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN ")                 '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")        '催行確定区分
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ")    '定期・企画区分
        sqlString.AppendLine(",0 AS LINE_NO ")                                  '行ＮＯ
        sqlString.AppendLine(",'' AS CRS_MESSAGE ")                             'コード
        sqlString.AppendLine(",NVL(BASIC.TYUIJIKOU_KBN,'') AS MESSAGE_KBN ")    '注意事項区分
        sqlString.AppendLine(",NVL(BASIC.TYUIJIKOU,'') AS MESSAGE ")            '注意事項
        sqlString.AppendLine(",'' AS EIBUN_MESSAGE ")                           '英文メッセージ
        sqlString.AppendLine(",0 AS DELETE_DAY ")                               '削除日
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_DAY ")                        'システム登録日
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_PERSON_CD ")                  'システム登録者コード
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_PGMID ")                      'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                       'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")                 'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")                     'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY2 ") '出発日2
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_12 ")      '乗車地コード2
        sqlString.AppendLine(",BASIC.GOUSYA AS GOUSYA2")                        '号車2
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_12 ") '出発時間2
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")          '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                        '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                             '更新区分
        sqlString.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & FixedCd.CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & FixedCd.CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & FixedCd.CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_MESSAGE MESSAGE ON BASIC.CRS_CD = MESSAGE.CRS_CD And BASIC.GOUSYA = MESSAGE.GOUSYA AND BASIC.SYUPT_DAY = MESSAGE.SYUPT_DAY ")
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

        sqlString.AppendLine("UNION ALL ")

        sqlString.AppendLine("SELECT TO_CHAR(TO_DATE(MESSAGE.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")  '出発日
        sqlString.AppendLine(",MESSAGE.CRS_CD ")                                'コースコード
        sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD ")                   '曜日コード
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 ")       '乗車地コード
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ") '出発時間
        sqlString.AppendLine(",MESSAGE.GOUSYA ")                                '号車
        sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN ")                 '運休区分
        sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN ")        '催行確定区分
        sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ")    '定期・企画区分
        sqlString.AppendLine(",NVL(MESSAGE.LINE_NO,'1') AS LINE_NO ")           '行ＮＯ
        sqlString.AppendLine(",'' AS CRS_MESSAGE ")                             'コード
        sqlString.AppendLine(",NVL(MESSAGE.MESSAGE_KBN,'') AS MESSAGE_KBN ")    'メッセージ区分
        sqlString.AppendLine(",NVL(MESSAGE.MESSAGE,'') AS MESSAGE ")            'メッセージ
        sqlString.AppendLine(",NVL(MESSAGE.EIBUN_MESSAGE,'') AS EIBUN_MESSAGE ")    '英文メッセージ
        sqlString.AppendLine(",NVL(MESSAGE.DELETE_DAY,0) AS DELETE_DAY ")       '削除日
        sqlString.AppendLine(",MESSAGE.SYSTEM_ENTRY_DAY ")                      'システム登録日
        sqlString.AppendLine(",NVL(MESSAGE.SYSTEM_ENTRY_PERSON_CD,'') AS SYSTEM_ENTRY_PERSON_CD ")  'システム登録者コード
        sqlString.AppendLine(",NVL(MESSAGE.SYSTEM_ENTRY_PGMID,'') AS SYSTEM_ENTRY_PGMID ")  'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_DAY ")                     'システム更新日
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_PERSON_CD ")               'システム更新者コード
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_PGMID ")                   'システム更新ＰＧＭＩＤ
        sqlString.AppendLine(",TO_CHAR(TO_DATE(MESSAGE.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY2 ")   '出発日2
        sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_12 ")      '乗車地コード
        sqlString.AppendLine(",MESSAGE.GOUSYA AS GOUSYA2")                      '号車2
        sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_12 ") '出発時間2
        sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG ")          '使用中フラグ
        sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN ")                        '変更可否区分
        sqlString.AppendLine(",' ' AS UPDATE_KBN ")                             '更新区分
        sqlString.AppendLine("FROM T_CRS_LEDGER_MESSAGE MESSAGE ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_BASIC BASIC ON BASIC.CRS_CD = MESSAGE.CRS_CD And BASIC.GOUSYA = MESSAGE.GOUSYA And BASIC.SYUPT_DAY = MESSAGE.SYUPT_DAY ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " & CodeBunrui.yobi & " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " & CodeBunrui.unkyu & " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " & CodeBunrui.saikou & " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ")
        sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ")
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
        sqlString.AppendLine(" YOBI_CD = " & setParam("YOBI_CD", paramList.Item("YOBI_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")

        sqlString.AppendLine(" ) ")
        sqlString.AppendLine("GROUP BY SYUPT_DAY ,CRS_CD ,YOBI_CD ,HAISYA_KEIYU_CD_1 ,SYUPT_TIME_1 ,GOUSYA ,UNKYU_KBN ,SAIKOU_KAKUTEI_KBN ,TEIKI_KIKAKU_KBN ,LINE_NO ,CRS_MESSAGE ,MESSAGE_KBN ,MESSAGE ,EIBUN_MESSAGE ,DELETE_DAY ,SYSTEM_ENTRY_DAY ,SYSTEM_ENTRY_PERSON_CD ,SYSTEM_ENTRY_PGMID ,SYSTEM_UPDATE_DAY ,SYSTEM_UPDATE_PERSON_CD ,SYSTEM_UPDATE_PGMID ,SYUPT_DAY2 ,HAISYA_KEIYU_CD_12 ,GOUSYA2 ,SYUPT_TIME_12 ,USING_FLG ")
        sqlString.AppendLine("ORDER BY SYUPT_DAY  , SYUPT_TIME_1, GOUSYA ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overloads Function getReCrsMessage(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'SELECT句
        sqlString.AppendLine("SELECT ")
        sqlString.AppendLine("SYUPT_DAY ")               '出発日
        sqlString.AppendLine(",CRS_CD ")             'コースコード
        sqlString.AppendLine(",GOUSYA ")             '号車
        sqlString.AppendLine(",TO_CHAR(LINE_NO) AS LINE_NO ")                '行ＮＯ
        sqlString.AppendLine(",MESSAGE_KBN ")                '注意事項区分/メッセージ区分
        sqlString.AppendLine(",MESSAGE ")                '注意事項/メッセージ
        sqlString.AppendLine(",EIBUN_MESSAGE ")              '英文メッセージ
        sqlString.AppendLine(",DELETE_DAY ")             '削除日
        sqlString.AppendLine(",SYSTEM_ENTRY_DAY ")               'システム登録日
        sqlString.AppendLine(",SYSTEM_ENTRY_PERSON_CD ")             'システム登録者コード
        sqlString.AppendLine(",SYSTEM_ENTRY_PGMID ")             'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",SYSTEM_UPDATE_DAY ")              'システム更新日
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD ")               'システム更新者コード
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID ")                'システム更新ＰＧＭＩＤ
        sqlString.AppendLine("FROM ")
        sqlString.AppendLine("( ")
        sqlString.AppendLine("SELECT TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")           '出発日
        sqlString.AppendLine(",BASIC.CRS_CD ")           'コースコード
        sqlString.AppendLine(",BASIC.GOUSYA ")           '号車
        sqlString.AppendLine(",0 AS LINE_NO ")               '行ＮＯ
        sqlString.AppendLine(",NVL(BASIC.TYUIJIKOU_KBN,'') AS MESSAGE_KBN ")             '注意事項区分
        sqlString.AppendLine(",NVL(BASIC.TYUIJIKOU,'') AS MESSAGE ")             '注意事項
        sqlString.AppendLine(",'' AS EIBUN_MESSAGE ")                '英文メッセージ
        sqlString.AppendLine(",0 AS DELETE_DAY ")                '削除日
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_DAY ")             'システム登録日
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_PERSON_CD ")               'システム登録者コード
        sqlString.AppendLine(",BASIC.SYSTEM_ENTRY_PGMID ")               'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")                'システム更新日
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")              'システム更新者コード
        sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")             'システム更新ＰＧＭＩＤ
        sqlString.AppendLine("From T_CRS_LEDGER_BASIC BASIC ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_MESSAGE MESSAGE ON BASIC.CRS_CD = MESSAGE.CRS_CD And BASIC.GOUSYA = MESSAGE.GOUSYA And BASIC.SYUPT_DAY = MESSAGE.SYUPT_DAY ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPTDAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRSCD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")

        sqlString.AppendLine("UNION ALL ")

        sqlString.AppendLine("SELECT TO_CHAR(TO_DATE(MESSAGE.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY ")           '出発日
        sqlString.AppendLine(",MESSAGE.CRS_CD ")           'コースコード
        sqlString.AppendLine(",MESSAGE.GOUSYA ")           '号車
        sqlString.AppendLine(",NVL(MESSAGE.LINE_NO,'1') AS LINE_NO ")                '行ＮＯ
        sqlString.AppendLine(",NVL(MESSAGE.MESSAGE_KBN,'') AS MESSAGE_KBN ")             'メッセージ区分
        sqlString.AppendLine(",NVL(MESSAGE.MESSAGE,'') AS MESSAGE ")             'メッセージ
        sqlString.AppendLine(",NVL(MESSAGE.EIBUN_MESSAGE,'') AS EIBUN_MESSAGE ")             '英文メッセージ
        sqlString.AppendLine(",NVL(MESSAGE.DELETE_DAY,0) AS DELETE_DAY ")              '削除日
        sqlString.AppendLine(",MESSAGE.SYSTEM_ENTRY_DAY ")               'システム登録日
        sqlString.AppendLine(",NVL(MESSAGE.SYSTEM_ENTRY_PERSON_CD,'') AS SYSTEM_ENTRY_PERSON_CD ")               'システム登録者コード
        sqlString.AppendLine(",NVL(MESSAGE.SYSTEM_ENTRY_PGMID,'') AS SYSTEM_ENTRY_PGMID ")               'システム登録ＰＧＭＩＤ
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_DAY ")              'システム更新日
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_PERSON_CD ")                'システム更新者コード
        sqlString.AppendLine(",MESSAGE.SYSTEM_UPDATE_PGMID ")                'システム更新ＰＧＭＩＤ
        sqlString.AppendLine("FROM T_CRS_LEDGER_MESSAGE MESSAGE ")
        sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_BASIC BASIC ON BASIC.CRS_CD = MESSAGE.CRS_CD And BASIC.GOUSYA = MESSAGE.GOUSYA And BASIC.SYUPT_DAY = MESSAGE.SYUPT_DAY ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" BASIC.SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPTDAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.CRS_CD = " & setParam("CRS_CD", paramList.Item("CRSCD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" BASIC.GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")

        sqlString.AppendLine(" ) ")
        sqlString.AppendLine(" WHERE LINE_NO NOT IN (0)")
        sqlString.AppendLine("GROUP BY SYUPT_DAY ,CRS_CD ,GOUSYA ,LINE_NO ,MESSAGE_KBN ,MESSAGE ,EIBUN_MESSAGE ,DELETE_DAY ,SYSTEM_ENTRY_DAY ,SYSTEM_ENTRY_PERSON_CD ,SYSTEM_ENTRY_PGMID ,SYSTEM_UPDATE_DAY ,SYSTEM_UPDATE_PERSON_CD ,SYSTEM_UPDATE_PGMID ")
        sqlString.AppendLine("ORDER BY SYUPT_DAY ,GOUSYA ")

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
    Public Function executeCrsMessageTehai(ByVal type As accessType, ByVal paramInfoList As Hashtable, ByVal sCrsMessage As String) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateCrsMessage
                    For i As Integer = 1 To 3
                        Select Case i
                            Case 1  'コース台帳（基本）
                                sqlString = executeUpdateBasicData(paramInfoList)
                            Case 2  'コース台帳（メッセージ）DELETE
                                If sCrsMessage = Edaban Then
                                Else
                                    sqlString = executeDeleteCrsMessageData(type, paramInfoList)
                                End If
                            Case 3  'コース台帳（メッセージ）
                                If sCrsMessage = Edaban Then
                                Else
                                    sqlString = executeInsertCrsMessageData(paramInfoList)
                                End If
                        End Select
                        If sqlString = String.Empty Then
                        Else
                            returnValue += execNonQuery(oracleTransaction, sqlString)
                        End If
                        sqlString = String.Empty
                    Next
            End Select

            '予約情報を更新
            Dim crsMessageChangeAfter As New Yoyaku.crsMessageChangeAfter
            Call crsMessageChangeAfter.changeCrsMessageAfter(CType(paramInfoList.Item("CRS_CD"), String), CType(paramInfoList.Item("GOUSYA"), String), CType(paramInfoList.Item("SYUPT_DAY"), String), CType(paramInfoList.Item("LINE_NO"), Integer), oracleTransaction)

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
    ''' コース台帳（メッセージ）：データ登録用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeInsertCrsMessageData(ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'INSERT
        sqlString.AppendLine(" INSERT INTO T_CRS_LEDGER_MESSAGE ")
        sqlString.AppendLine(" ( ")
        sqlString.AppendLine(" CRS_CD ")
        sqlString.AppendLine(",SYUPT_DAY ")
        sqlString.AppendLine(",GOUSYA ")
        sqlString.AppendLine(",LINE_NO ")
        sqlString.AppendLine(",MESSAGE_KBN ")
        sqlString.AppendLine(",MESSAGE ")
        sqlString.AppendLine(",EIBUN_MESSAGE ")
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
        sqlString.AppendLine("," & setParam("SYUPT_DAY", CType(paramList.Item("SYUPT_DAY"), Decimal), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine("," & setParam("GOUSYA", CType(paramList.Item("GOUSYA"), Decimal), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine("," & setParam("LINE_NO", CType(paramList.Item("LINE_NO"), Decimal), OracleDbType.Decimal, 2, 0))
        sqlString.AppendLine("," & setParam("MESSAGE_KBN", CType(paramList.Item("MESSAGE_KBN"), String), OracleDbType.Char))
        sqlString.AppendLine("," & setParam("MESSAGE", CType(paramList.Item("MESSAGE"), String), OracleDbType.Varchar2))
        sqlString.AppendLine("," & setParam("EIBUN_MESSAGE", CType(paramList.Item("EIBUN_MESSAGE"), String), OracleDbType.Varchar2))
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

    ''' <summary>
    ''' コース台帳（メッセージ）：データ削除用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function executeDeleteCrsMessageData(ByVal type As accessType, ByVal paramList As Hashtable) As String

        Dim sqlString As New StringBuilder
        paramClear()

        'DELETE
        sqlString.AppendLine(" DELETE ")
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine(" T_CRS_LEDGER_MESSAGE ")
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))
        sqlString.AppendLine(" AND ")
        Select Case type
            Case accessType.executeUpdateCrsMessage
                sqlString.AppendLine(" LINE_NO = " & setParam("LINE_NO", paramList.Item("LINE_NO"), OracleDbType.Decimal, 2, 0))
        End Select

        Return sqlString.ToString

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
        'sqlString.AppendLine(" USING_FLG = " & setParam("USING_FLG", paramList.Item("USING_FLG"), OracleDbType.Char))
        sqlString.AppendLine("SYSTEM_UPDATE_DAY = " & setParam("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime, OracleDbType.Date))
        sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = " & setParam("SYSTEM_UPDATE_PGMID", paramList.Item("SYSTEM_UPDATE_PGMID"), OracleDbType.Char))
        sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = " & setParam("SYSTEM_UPDATE_PERSON_CD", paramList.Item("SYSTEM_UPDATE_PERSON_CD"), OracleDbType.Varchar2))
        If CType(paramList.Item("LINE_NO"), Decimal) = 0 Then
            sqlString.AppendLine(",TYUIJIKOU_KBN = " & setParam("TYUIJIKOU_KBN", paramList.Item("TYUIJIKOU_KBN"), OracleDbType.Char))
            sqlString.AppendLine(",TYUIJIKOU = " & setParam("TYUIJIKOU", paramList.Item("TYUIJIKOU"), OracleDbType.Varchar2))
        End If
        'WHERE句
        sqlString.AppendLine(" WHERE ")
        sqlString.AppendLine(" SYUPT_DAY = " & setParam("SYUPT_DAY", paramList.Item("SYUPT_DAY"), OracleDbType.Decimal, 8, 0))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" CRS_CD = " & setParam("CRS_CD", paramList.Item("CRS_CD"), OracleDbType.Char))
        sqlString.AppendLine(" AND ")
        sqlString.AppendLine(" GOUSYA = " & setParam("GOUSYA", paramList.Item("GOUSYA"), OracleDbType.Decimal, 3, 0))

        Return sqlString.ToString

    End Function

#End Region


End Class
