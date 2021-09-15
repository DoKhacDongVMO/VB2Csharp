Imports System.Text

''' <summary>
''' コース台帳一括修正（発着情報）のDAクラス
''' </summary>
Public Class StartEnd_DA
    Inherits DataAccessorBase

#Region " 定数／変数 "

    Public Enum accessType As Integer
        getPlaceMaster                    '場所マスタ検索
        getStartEnd                       '一覧結果取得検索
        executeUpdateBusCompanyCd         '更新
        executeReverseBusCompanyCd        '戻し
    End Enum

    Private Const CodeValueSyugoTime As String = "106"  'コードマスタ.コード値（集合時間（出発日－？分））

    Private _comTehai As New TehaiCommon
#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' SELECT用DBアクセス
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="paramInfoList"></param>
    ''' <returns></returns>
    Public Function accessStartEndTehai(ByVal type As accessType, Optional ByVal paramInfoList As Hashtable = Nothing) As DataTable
        'SQL文字列
        Dim sqlString As String = String.Empty
        '戻り値
        Dim returnValue As DataTable = Nothing

        Select Case type
            Case accessType.getPlaceMaster
                ' 場所マスタ検索
                sqlString = getPlaceMaster()
            Case accessType.getStartEnd
                '一覧結果取得検索
                sqlString = getStartEnd(paramInfoList)
            Case Else
                '該当処理なし
                Return returnValue
        End Select

        Try
            returnValue = getDataTable(sqlString)
        Catch ex As Exception
            Throw ex
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' 場所マスタ検索
    ''' </summary>
    Public Function getPlaceMaster() As String

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder

        '空レコード挿入要否に従い、空行挿入
        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" ' ' AS CODE_VALUE ")
        sqlString.AppendLine(",'' AS CODE_NAME ")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("DUAL UNION ")
        sqlString.AppendLine(" SELECT")
        sqlString.AppendLine(" RTRIM(PLACE_CD) AS CODE_VALUE ")
        sqlString.AppendLine(",PLACE_NAME_1 AS CODE_NAME ")
        'FROM句
        sqlString.AppendLine(" FROM ")
        sqlString.AppendLine("M_PLACE ")
        'WHERE句
        sqlString.AppendLine(" WHERE NVL(DELETE_DATE,0) = 0 ")
        'ORDER BY句
        sqlString.AppendLine(" ORDER BY ")
        sqlString.AppendLine(" CODE_VALUE ASC ")

        Return sqlString.ToString
    End Function

    ''' <summary>
    ''' 検索用SELECT
    ''' </summary>
    ''' <param name="paramList"></param>
    ''' <returns>SQL分</returns>
    ''' <remarks></remarks>
    Private Function getStartEnd(ByVal paramList As Hashtable) As String

        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity
        Dim clsCdMasterEntity As New CdMasterEntity

        Dim sqlString As New StringBuilder
        paramClear()

        With clsCrsLedgerBasicEntity

            'パラメータ設定
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            'setParam(.haisyaKeiyuCd1.PhysicsName, paramList.Item(.haisyaKeiyuCd1.PhysicsName), .haisyaKeiyuCd1.DBType, .haisyaKeiyuCd1.IntegerBu, .haisyaKeiyuCd1.DecimalBu)
            If CType(paramList.Item(.syuptJiCarrierKbn.PhysicsName), Integer) = SyuptJiCarrierKbnType.bus Then
                setParam(.haisyaKeiyuCd1.PhysicsName, paramList.Item(.haisyaKeiyuCd1.PhysicsName), .haisyaKeiyuCd1.DBType, .haisyaKeiyuCd1.IntegerBu, .haisyaKeiyuCd1.DecimalBu)
            Else
                setParam(.syuptPlaceCdCarrier.PhysicsName, paramList.Item(.syuptPlaceCdCarrier.PhysicsName), .syuptPlaceCdCarrier.DBType, .syuptPlaceCdCarrier.IntegerBu, .syuptPlaceCdCarrier.DecimalBu)
            End If
            setParam(.unkyuKbn.PhysicsName, paramList.Item(.unkyuKbn.PhysicsName), .unkyuKbn.DBType, .unkyuKbn.IntegerBu, .unkyuKbn.DecimalBu)
            setParam(.saikouKakuteiKbn.PhysicsName, paramList.Item(.saikouKakuteiKbn.PhysicsName), .saikouKakuteiKbn.DBType, .saikouKakuteiKbn.IntegerBu, .saikouKakuteiKbn.DecimalBu)
            setParam(.maruZouManagementKbn.PhysicsName, paramList.Item(.maruZouManagementKbn.PhysicsName), .maruZouManagementKbn.DBType, .maruZouManagementKbn.IntegerBu, .maruZouManagementKbn.DecimalBu)
            setParam(.yobiCd.PhysicsName, paramList.Item(.yobiCd.PhysicsName), .yobiCd.DBType, .yobiCd.IntegerBu, .yobiCd.DecimalBu)
            setParam(clsCdMasterEntity.CdValue.PhysicsName, CodeValueSyugoTime, clsCdMasterEntity.CdValue.DBType, clsCdMasterEntity.CdValue.IntegerBu, clsCdMasterEntity.CdValue.DecimalBu)

            'SELECT句
            sqlString.AppendLine(" SELECT ")
            sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') AS SYUPT_DAY ")
            sqlString.AppendLine(",CODE_YOBI.CODE_NAME AS YOBI")
            sqlString.AppendLine(",TO_CHAR(BASIC.GOUSYA) AS GOUSYA ")
            sqlString.AppendLine(",CODE_UNKYU.CODE_NAME AS UNKYU_KBN")
            sqlString.AppendLine(",CODE_SAIKOU.CODE_NAME AS SAIKOU_KAKUTEI_KBN")
            sqlString.AppendLine(",BASIC.BUS_COMPANY_CD ")
            sqlString.AppendLine(",BUS_COMPANY.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_1 ")
            sqlString.AppendLine(",PLACE_1.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_1 ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_2 ")
            sqlString.AppendLine(",PLACE_2.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_2 ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_3 ")
            sqlString.AppendLine(",PLACE_3.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_3 ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_4 ")
            sqlString.AppendLine(",PLACE_4.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_4 ")
            sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_5 ")
            sqlString.AppendLine(",PLACE_5.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_5 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_1") & " AS SYUGO_TIME_1 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_2") & " AS SYUGO_TIME_2 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_3") & " AS SYUGO_TIME_3 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_4") & " AS SYUGO_TIME_4 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_5") & " AS SYUGO_TIME_5 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") & " AS SYUPT_TIME_1 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_2") & " AS SYUPT_TIME_2 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_3") & " AS SYUPT_TIME_3 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_4") & " AS SYUPT_TIME_4 ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_5") & " AS SYUPT_TIME_5 ")
            sqlString.AppendLine(",BASIC.END_PLACE_CD ")
            sqlString.AppendLine(",END_PLACE.PLACE_NAME_1 AS END_PLACE_NAME ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.END_TIME") & " AS END_TIME ")
            'sqlString.AppendLine(",BASIC.SYUPT_JI_CARRIER_KBN ")
            sqlString.AppendLine(",BASIC.CARRIER_CD ")
            sqlString.AppendLine(",BASIC.CARRIER_EDABAN ")
            sqlString.AppendLine(",CARRIER.SIIRE_SAKI_NAME AS CARRIER_NAME ")
            sqlString.AppendLine(",BASIC.SYUGO_PLACE_CD_CARRIER ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_CARRIER") & " AS SYUGO_TIME_CARRIER ")
            sqlString.AppendLine(",SYUPT_PLACE_CARRIER.PLACE_NAME_1 AS SYUPT_PLACE_CARRIER ")
            sqlString.AppendLine(",BASIC.SYUPT_PLACE_CD_CARRIER ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_CARRIER") & " AS SYUPT_TIME_CARRIER ")
            sqlString.AppendLine(",ARRIVAL_PLACE_CARRIER.PLACE_NAME_1 AS TTYAK_PLACE_CARRIER ")
            sqlString.AppendLine(",BASIC.TTYAK_PLACE_CD_CARRIER ")
            sqlString.AppendLine("," & CommonKyushuUtil.setSQLTime24Format("BASIC.TTYAK_TIME_CARRIER") & " AS TTYAK_TIME_CARRIER ")
            sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID ")
            sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD ")
            sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY ")
            sqlString.AppendLine(",BASIC.USING_FLG ")
            sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ")
            sqlString.AppendLine(",CODE_SYUGO_TIME.NAIYO_1 AS SYUGO_TIME_CALCULATION ")
            'FROM句
            sqlString.AppendLine(" FROM ")
            sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ")
            sqlString.AppendLine("LEFT JOIN M_CODE CODE_YOBI ON CODE_YOBI.CODE_BUNRUI = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.yobi) & "' AND BASIC.YOBI_CD = CODE_YOBI.CODE_VALUE ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE_1 ON BASIC.HAISYA_KEIYU_CD_1 = PLACE_1.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE_2 ON BASIC.HAISYA_KEIYU_CD_2 = PLACE_2.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE_3 ON BASIC.HAISYA_KEIYU_CD_3 = PLACE_3.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE_4 ON BASIC.HAISYA_KEIYU_CD_4 = PLACE_4.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE PLACE_5 ON BASIC.HAISYA_KEIYU_CD_5 = PLACE_5.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE END_PLACE ON BASIC.END_PLACE_CD = END_PLACE.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE SYUPT_PLACE_CARRIER ON BASIC.SYUPT_PLACE_CD_CARRIER = SYUPT_PLACE_CARRIER.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_PLACE ARRIVAL_PLACE_CARRIER ON BASIC.TTYAK_PLACE_CD_CARRIER = ARRIVAL_PLACE_CARRIER.PLACE_CD ")
            sqlString.AppendLine("LEFT JOIN M_CODE CODE_UNKYU ON CODE_UNKYU.CODE_BUNRUI = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.unkyu) & "' AND BASIC.UNKYU_KBN = CODE_UNKYU.CODE_VALUE ")
            sqlString.AppendLine("LEFT JOIN M_CODE CODE_SAIKOU ON CODE_SAIKOU.CODE_BUNRUI = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.saikouKakuteiKbn) & "' AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_SAIKOU.CODE_VALUE ")
            sqlString.AppendLine("LEFT JOIN M_SIIRE_SAKI BUS_COMPANY ON BASIC.BUS_COMPANY_CD = BUS_COMPANY.SIIRE_SAKI_CD || BUS_COMPANY.SIIRE_SAKI_NO ")
            sqlString.AppendLine("LEFT JOIN M_SIIRE_SAKI CARRIER ON BASIC.CARRIER_CD = CARRIER.SIIRE_SAKI_CD AND BASIC.CARRIER_EDABAN = CARRIER.SIIRE_SAKI_NO ")
            sqlString.AppendLine("LEFT JOIN M_CODE CODE_SYUGO_TIME ON CODE_SYUGO_TIME.CODE_BUNRUI = '" & CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.systemSet) & "' AND CODE_SYUGO_TIME.CODE_VALUE = :" & clsCdMasterEntity.CdValue.PhysicsName)
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine(" BASIC.SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.CRS_CD = :" & .crsCd.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.GOUSYA = :" & .gousya.PhysicsName)
            'sqlString.AppendLine(" AND (")
            'sqlString.AppendLine(" BASIC.HAISYA_KEIYU_CD_1 = :" & .haisyaKeiyuCd1.PhysicsName)
            'sqlString.AppendLine("   OR ")
            'sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_2 = :" & .haisyaKeiyuCd1.PhysicsName)
            'sqlString.AppendLine("   OR ")
            'sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_3 = :" & .haisyaKeiyuCd1.PhysicsName)
            'sqlString.AppendLine("   OR ")
            'sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_4 = :" & .haisyaKeiyuCd1.PhysicsName)
            'sqlString.AppendLine("   OR ")
            'sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_5 = :" & .haisyaKeiyuCd1.PhysicsName & ")")
            If CType(paramList.Item(.syuptJiCarrierKbn.PhysicsName), Integer) = SyuptJiCarrierKbnType.bus Then
                If String.IsNullOrEmpty(paramList.Item(.haisyaKeiyuCd1.PhysicsName).ToString()) = False Then
                    sqlString.AppendLine(" AND (")
                    sqlString.AppendLine(" BASIC.HAISYA_KEIYU_CD_1 = :" & .haisyaKeiyuCd1.PhysicsName)
                    sqlString.AppendLine("   OR ")
                    sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_2 = :" & .haisyaKeiyuCd1.PhysicsName)
                    sqlString.AppendLine("   OR ")
                    sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_3 = :" & .haisyaKeiyuCd1.PhysicsName)
                    sqlString.AppendLine("   OR ")
                    sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_4 = :" & .haisyaKeiyuCd1.PhysicsName)
                    sqlString.AppendLine("   OR ")
                    sqlString.AppendLine("   BASIC.HAISYA_KEIYU_CD_5 = :" & .haisyaKeiyuCd1.PhysicsName & ")")
                End If
            Else
                If String.IsNullOrEmpty(paramList.Item(.syuptPlaceCdCarrier.PhysicsName).ToString()) = False Then
                    sqlString.AppendLine(" AND ")
                    sqlString.AppendLine(" BASIC.SYUPT_PLACE_CD_CARRIER = :" & .syuptPlaceCdCarrier.PhysicsName)
                End If
            End If
            sqlString.AppendLine(" AND ")
            '運休区分
            If String.IsNullOrEmpty(paramList.Item(.unkyuKbn.PhysicsName).ToString()) = True Then
                sqlString.AppendLine(" BASIC.UNKYU_KBN IS NULL")
            Else
                sqlString.AppendLine(" BASIC.UNKYU_KBN = :" & .unkyuKbn.PhysicsName)
            End If
            sqlString.AppendLine(" AND ")
            '催行確定区分
            If String.IsNullOrEmpty(paramList.Item(.saikouKakuteiKbn.PhysicsName).ToString()) = True Then
                sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN IS NULL")
            Else
                sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = :" & .saikouKakuteiKbn.PhysicsName)
            End If
            sqlString.AppendLine(" AND ")
            '○増管理区分
            If String.IsNullOrEmpty(paramList.Item(.maruZouManagementKbn.PhysicsName).ToString()) = True Then
                sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN IS NULL")
            Else
                sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN = :" & .maruZouManagementKbn.PhysicsName)
            End If
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" BASIC.YOBI_CD = :" & .yobiCd.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ")
            'ORDER BY句
            sqlString.AppendLine(" ORDER BY ")
            sqlString.AppendLine(" BASIC.SYUPT_DAY ")
            sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ")
            sqlString.AppendLine(" ,BASIC.GOUSYA ")
        End With

        Return sqlString.ToString

    End Function

#End Region


#Region " UPDATE処理 "

    ''' <summary>
    ''' 使用中フラグ更新
    ''' </summary>
    ''' <param name="crsCd"></param>
    ''' <param name="selectData"></param>
    ''' <param name="systemDate"></param>
    ''' <param name="systemUpdatePgmId"></param>
    ''' <returns>Dataset（使用中フラグ設定内容DataTable、予約情報（基本）DataTable）</returns>
    ''' <remarks></remarks>
    Public Function executeUsingFlgCrs(ByVal crsCd As String, ByVal selectData As DataTable, systemDate As Date, ByVal systemUpdatePgmId As String) As DataSet

        Dim returnDataSet As New DataSet
        Dim usingFlgValue As New DataTable
        Dim relationKeyValue As New DataTable
        Dim trn As OracleTransaction = Nothing
        Dim returnResultCrsLedgerBasic As Boolean = False
        Dim returnValue As New DataTable
        Dim returnResultYoyakuInfoBasic As Boolean = False
        Dim syuptDayCompare As String = String.Empty
        Dim syuptDay As String = String.Empty
        Dim gousya As String = String.Empty
        Dim usingFlgYoyaku As Boolean = False
        Dim usingFlgRow As DataRow = usingFlgValue.NewRow
        Dim relationKeyRow As DataRow = relationKeyValue.NewRow
        Dim UsingNow As String = "Y"
        'DBパラメータ
        Dim paramInfoList As New Hashtable
        '予約情報（基本）エンティティ
        Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity()

        Try

            '使用中かどうかを判断するためのデータテーブル
            usingFlgValue.Columns.Add("SYUPT_DAY")             '出発日
            usingFlgValue.Columns.Add("GOUSYA")                '号車
            usingFlgValue.Columns.Add("USING_FLG")             '使用中フラグ

            '予約情報（基本）を保持するためのデータテーブル
            relationKeyValue.Columns.Add("CRS_CD")                     'コースコード
            relationKeyValue.Columns.Add("SYUPT_DAY")                  '出発日
            relationKeyValue.Columns.Add("GOUSYA")                     '号車
            relationKeyValue.Columns.Add("YOYAKU_KBN")                 '予約区分
            relationKeyValue.Columns.Add("YOYAKU_NO")                  '予約NO
            relationKeyValue.Columns.Add("JYOCHACHI_CD_1")             '乗車地コード１
            relationKeyValue.Columns.Add("JYOCHACHI_CD_2")             '乗車地コード２
            relationKeyValue.Columns.Add("JYOCHACHI_CD_3")             '乗車地コード３
            relationKeyValue.Columns.Add("JYOCHACHI_CD_4")             '乗車地コード４
            relationKeyValue.Columns.Add("JYOCHACHI_CD_5")             '乗車地コード５
            relationKeyValue.Columns.Add("JYOSYA_NINZU_1")             '乗車人数１
            relationKeyValue.Columns.Add("JYOSYA_NINZU_2")             '乗車人数２
            relationKeyValue.Columns.Add("JYOSYA_NINZU_3")             '乗車人数３
            relationKeyValue.Columns.Add("JYOSYA_NINZU_4")             '乗車人数４
            relationKeyValue.Columns.Add("JYOSYA_NINZU_5")             '乗車人数５
            relationKeyValue.Columns.Add("USING_FLG_ORG")              '使用中フラグ（既存）
            relationKeyValue.Columns.Add("USING_FLG_TARGET")           '使用中フラグ（今回変更分）
            relationKeyValue.Columns.Add("SYSTEM_UPDATE_DAY")          'システム更新日
            relationKeyValue.Columns.Add("SYSTEM_UPDATE_PERSON_CD")    'システム更新者コード
            relationKeyValue.Columns.Add("SYSTEM_UPDATE_PGMID")        'システム更新PGMID

            'トランザクション開始
            trn = callBeginTransaction()

            For Each row As DataRow In selectData.Rows
                paramInfoList = New Hashtable
                syuptDayCompare = row("SYUPT_DAY").ToString().Substring(0, 4) & "/" & row("SYUPT_DAY").ToString().Substring(4, 2) & "/" & row("SYUPT_DAY").ToString().Substring(6, 2)
                '出発日が過去日の場合は読み飛ばし
                If CType(syuptDayCompare, Date) < systemDate Then
                    Continue For
                End If
                syuptDay = CType(row("SYUPT_DAY"), String)  '出発日
                gousya = CType(row("GOUSYA"), String)       '号車
                usingFlgRow = usingFlgValue.NewRow
                usingFlgRow("SYUPT_DAY") = syuptDayCompare
                usingFlgRow("GOUSYA") = gousya

                ''コース台帳（基本）の使用中フラグチェック
                'returnResultCrsLedgerBasic = CommonCheckUtil.setUsingFlg_Crs(syuptDay, crsCd, gousya, systemUpdatePgmId, trn, False)

                If row("USING_FLG").ToString = UsingNow Then
                    'コース台帳（基本）の使用中フラグが使用中の場合
                    'usingFlgRow("USING_FLG") = String.Empty
                    usingFlgRow("USING_FLG") = UsingNow
                Else
                    'コース台帳（基本）の使用中フラグが使用中でない場合

                    '予約情報（基本）取得
                    With clsYoyakuInfoBasicEntity
                        '出発時間
                        paramInfoList.Add(.syuptDay.PhysicsName, syuptDay)
                        'コースコード
                        paramInfoList.Add(.crsCd.PhysicsName, crsCd)
                        '号車
                        paramInfoList.Add(.gousya.PhysicsName, gousya)
                    End With

                    returnValue = getYoyakuInfoBasic(paramInfoList, trn)

                    '予約情報（基本）の使用中フラグが立っているレコードがあるかをチェック
                    For Each rowYoyaku As DataRow In returnValue.Rows
                        If usingFlgYoyaku = False Then
                            If rowYoyaku("USING_FLG_ORG").ToString().Equals(S03_0213.TargetFlg.NotTarget) = True Then
                                usingFlgRow("USING_FLG") = UsingNow
                                usingFlgYoyaku = True
                            End If
                        End If
                        relationKeyRow = relationKeyValue.NewRow
                        relationKeyRow("CRS_CD") = crsCd
                        relationKeyRow("SYUPT_DAY") = syuptDay
                        relationKeyRow("GOUSYA") = gousya
                        relationKeyRow("YOYAKU_KBN") = rowYoyaku("YOYAKU_KBN").ToString()
                        relationKeyRow("YOYAKU_NO") = rowYoyaku("YOYAKU_NO").ToString()
                        relationKeyRow("JYOCHACHI_CD_1") = rowYoyaku("JYOCHACHI_CD_1").ToString()
                        relationKeyRow("JYOCHACHI_CD_2") = rowYoyaku("JYOCHACHI_CD_2").ToString()
                        relationKeyRow("JYOCHACHI_CD_3") = rowYoyaku("JYOCHACHI_CD_3").ToString()
                        relationKeyRow("JYOCHACHI_CD_4") = rowYoyaku("JYOCHACHI_CD_4").ToString()
                        relationKeyRow("JYOCHACHI_CD_5") = rowYoyaku("JYOCHACHI_CD_5").ToString()
                        relationKeyRow("JYOSYA_NINZU_1") = rowYoyaku("JYOSYA_NINZU_1").ToString()
                        relationKeyRow("JYOSYA_NINZU_2") = rowYoyaku("JYOSYA_NINZU_2").ToString()
                        relationKeyRow("JYOSYA_NINZU_3") = rowYoyaku("JYOSYA_NINZU_3").ToString()
                        relationKeyRow("JYOSYA_NINZU_4") = rowYoyaku("JYOSYA_NINZU_4").ToString()
                        relationKeyRow("JYOSYA_NINZU_5") = rowYoyaku("JYOSYA_NINZU_5").ToString()
                        relationKeyRow("USING_FLG_ORG") = rowYoyaku("USING_FLG_ORG").ToString()
                        relationKeyRow("USING_FLG_TARGET") = String.Empty
                        relationKeyRow("SYSTEM_UPDATE_DAY") = rowYoyaku("SYSTEM_UPDATE_DAY").ToString()
                        relationKeyRow("SYSTEM_UPDATE_PERSON_CD") = rowYoyaku("SYSTEM_UPDATE_PERSON_CD").ToString()
                        relationKeyRow("SYSTEM_UPDATE_PGMID") = rowYoyaku("SYSTEM_UPDATE_PGMID").ToString()
                        relationKeyValue.Rows.Add(relationKeyRow)
                    Next

                    '予約情報（基本）の使用中フラグが1件も立っていない場合
                    If usingFlgYoyaku = False Then
                        ''コース台帳（基本）の使用中フラグ更新
                        'returnResultCrsLedgerBasic = CommonCheckUtil.setUsingFlg_Crs(syuptDay, crsCd, gousya, systemUpdatePgmId, trn, True)
                        For Each rowYoyaku As DataRow In returnValue.Rows
                            '予約情報（基本）の使用中フラグ更新
                            returnResultYoyakuInfoBasic = CommonCheckUtil.setUsingFlg_Yoyaku(rowYoyaku("YOYAKU_KBN").ToString(), rowYoyaku("YOYAKU_NO").ToString(), systemUpdatePgmId, trn, True)
                            'rowYoyaku("USING_FLG_TARGET") = FixedCdTehai.UsingFlg.Use
                        Next
                        If returnResultYoyakuInfoBasic = True Then
                            usingFlgRow("USING_FLG") = String.Empty
                        End If
                    End If
                End If

                usingFlgValue.Rows.Add(usingFlgRow)
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

        returnDataSet.Tables.Add(usingFlgValue)
        returnDataSet.Tables.Add(relationKeyValue)

        Return returnDataSet

    End Function

    ''' <summary>
    ''' 予約情報（基本）取得
    ''' </summary>
    ''' <param name="paramList">検索用パラメータ</param>
    ''' <param name="transaction">Oracleトランザクション</param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function getYoyakuInfoBasic(ByVal paramList As Hashtable, ByVal transaction As OracleTransaction) As DataTable

        Dim resultDataTable As New DataTable
        Dim sqlString As New StringBuilder
        '予約情報（基本）エンティティ
        Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity()
        paramClear()

        Try

            'パラメータ設定
            '予約情報（基本）
            With clsYoyakuInfoBasicEntity
                '出発日
                setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
                'コースコード
                setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
                '号車
                setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)

                sqlString.AppendLine("SELECT ")
                sqlString.AppendLine(" RES_BASE.YOYAKU_KBN ")
                sqlString.AppendLine(",RES_BASE.YOYAKU_NO ")
                sqlString.AppendLine(",RES_BASE.JYOCHACHI_CD_1 ")
                sqlString.AppendLine(",RES_BASE.JYOCHACHI_CD_2 ")
                sqlString.AppendLine(",RES_BASE.JYOCHACHI_CD_3 ")
                sqlString.AppendLine(",RES_BASE.JYOCHACHI_CD_4 ")
                sqlString.AppendLine(",RES_BASE.JYOCHACHI_CD_5 ")
                sqlString.AppendLine(",RES_BASE.JYOSYA_NINZU_1 ")
                sqlString.AppendLine(",RES_BASE.JYOSYA_NINZU_2 ")
                sqlString.AppendLine(",RES_BASE.JYOSYA_NINZU_3 ")
                sqlString.AppendLine(",RES_BASE.JYOSYA_NINZU_4 ")
                sqlString.AppendLine(",RES_BASE.JYOSYA_NINZU_5 ")
                sqlString.AppendLine(",RES_BASE.USING_FLG AS USING_FLG_ORG")
                sqlString.AppendLine(",RES_BASE.SYSTEM_UPDATE_DAY ")
                sqlString.AppendLine(",RES_BASE.SYSTEM_UPDATE_PERSON_CD ")
                sqlString.AppendLine(",RES_BASE.SYSTEM_UPDATE_PGMID ")
                sqlString.AppendLine(" FROM ")
                sqlString.AppendLine(" T_YOYAKU_INFO_BASIC RES_BASE ")

                'WHERE句
                sqlString.AppendLine(" WHERE RES_BASE.SYUPT_DAY = : " & .syuptDay.PhysicsName)
                sqlString.AppendLine(" AND RES_BASE.CRS_CD = : " & .crsCd.PhysicsName)
                sqlString.AppendLine(" AND RES_BASE.GOUSYA = : " & .gousya.PhysicsName)
                sqlString.AppendLine(" AND NVL(RES_BASE.DELETE_DAY,0) = 0 ")
            End With

            resultDataTable = MyBase.getDataTable(transaction, sqlString.ToString())
        Catch ex As Exception
            Throw ex
        End Try

        Return resultDataTable
    End Function

    ''' <summary>
    ''' 更新処理
    ''' </summary>
    ''' <param name="type">アクセスタイプ</param>
    ''' <param name="paramInfoList">更新用パラメータ</param>
    ''' <param name="paramYoyakuInfoData">予約情報（基本）データ</param>
    ''' <param name="initFlg">初期化フラグ</param>
    ''' <param name="syuptJiCarrierKbn">出発時キャリア区分</param>
    ''' <returns>更新処理件数</returns>
    ''' <remarks></remarks>
    Public Function updateStartEndInfo(ByVal type As accessType, ByVal paramInfoList As Hashtable, ByVal paramYoyakuInfoData As DataTable,
                                        Optional ByVal initFlg As Boolean = False,
                                        Optional ByVal syuptJiCarrierKbn As Integer = Nothing) As Integer
        Dim oracleTransaction As OracleTransaction = Nothing
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Dim selectData() As DataRow = Nothing
        Dim whereString As String = String.Empty
        Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity
        Dim paramYoyakuInfoList As Hashtable = Nothing

        Try

            'トランザクション開始
            oracleTransaction = callBeginTransaction()

            Select Case type
                Case accessType.executeUpdateBusCompanyCd
                    For i As Integer = 1 To 2
                        Select Case i
                            Case 1  'コース台帳（ダイヤ）
                                sqlString = executeUpdateCrsLedgerDia(paramInfoList, syuptJiCarrierKbn)
                            Case 2  'コース台帳（基本）
                                sqlString = executeUpdateCrsLedgerBasic(paramInfoList, False, syuptJiCarrierKbn)
                        End Select

                        returnValue += execNonQuery(oracleTransaction, sqlString)
                    Next
                Case accessType.executeReverseBusCompanyCd
                    '予約情報（基本）のデータテーブルから対象データを取得
                    '条件の設定
                    With clsYoyakuInfoBasicEntity
                        whereString = .syuptDay.PhysicsName & " = " & CType(paramInfoList.Item(.syuptDay.PhysicsName), Integer) &
                                " AND " & .crsCd.PhysicsName & " = '" & paramInfoList.Item(.crsCd.PhysicsName).ToString() & "'" &
                                " AND " & .gousya.PhysicsName & " = " & CType(paramInfoList.Item(.gousya.PhysicsName), Integer)
                        '問合せ対象データ取得
                        selectData = paramYoyakuInfoData.Select(whereString)
                        For Each row As DataRow In selectData
                            'パラメータ設定
                            paramYoyakuInfoList = New Hashtable
                            paramYoyakuInfoList.Add(.yoyakuKbn.PhysicsName, row(.yoyakuKbn.PhysicsName).ToString())
                            paramYoyakuInfoList.Add(.yoyakuNo.PhysicsName, CType(row(.yoyakuNo.PhysicsName).ToString, Integer))
                            paramYoyakuInfoList.Add(.systemUpdatePgmid.PhysicsName, row(.systemUpdatePgmid.PhysicsName).ToString())
                            paramYoyakuInfoList.Add(.systemUpdatePersonCd.PhysicsName, row(.systemUpdatePersonCd.PhysicsName).ToString())
                            paramYoyakuInfoList.Add(.systemUpdateDay.PhysicsName, CType(row(.systemUpdateDay.PhysicsName).ToString(), DateTime))
                            '予約情報（基本）更新
                            sqlString = executeUpdateYoyakuInfoBasic(paramYoyakuInfoList)
                            returnValue = execNonQuery(oracleTransaction, sqlString)
                        Next
                    End With

            End Select

            'If initFlg = True Then
            '    'コース台帳（基本）更新
            '    sqlString = executeUpdateCrsLedgerBasic(paramInfoList, True)
            '    returnValue = execNonQuery(oracleTransaction, sqlString)
            'Else
            ''コース台帳（ダイヤ）更新
            'sqlString = executeUpdateCrsLedgerDia(paramInfoList, syuptJiCarrierKbn)
            'returnValue = execNonQuery(oracleTransaction, sqlString)

            'If returnValue > 0 Then
            '    'コース台帳（基本）更新
            '    sqlString = executeUpdateCrsLedgerBasic(paramInfoList, False, syuptJiCarrierKbn)
            '    returnValue = execNonQuery(oracleTransaction, sqlString)
            'End If
            'End If

            'If returnValue > 0 Then
            '    '予約情報（基本）のデータテーブルから対象データを取得
            '    '条件の設定
            '    With clsYoyakuInfoBasicEntity
            '        whereString = .syuptDay.PhysicsName & " = " & CType(paramInfoList.Item(.syuptDay.PhysicsName), Integer) &
            '                " AND " & .crsCd.PhysicsName & " = '" & paramInfoList.Item(.crsCd.PhysicsName).ToString() & "'" &
            '                " AND " & .gousya.PhysicsName & " = " & CType(paramInfoList.Item(.gousya.PhysicsName), Integer)
            '        '問合せ対象データ取得
            '        selectData = paramYoyakuInfoData.Select(whereString)
            '        For Each row As DataRow In selectData
            '            'パラメータ設定
            '            paramYoyakuInfoList = New Hashtable
            '            paramYoyakuInfoList.Add(.yoyakuKbn.PhysicsName, row(.yoyakuKbn.PhysicsName).ToString())
            '            paramYoyakuInfoList.Add(.yoyakuNo.PhysicsName, CType(row(.yoyakuNo.PhysicsName).ToString, Integer))
            '            paramYoyakuInfoList.Add(.systemUpdatePgmid.PhysicsName, row(.systemUpdatePgmid.PhysicsName).ToString())
            '            paramYoyakuInfoList.Add(.systemUpdatePersonCd.PhysicsName, row(.systemUpdatePersonCd.PhysicsName).ToString())
            '            paramYoyakuInfoList.Add(.systemUpdateDay.PhysicsName, CType(row(.systemUpdateDay.PhysicsName).ToString(), DateTime))
            '            '予約情報（基本）更新
            '            sqlString = executeUpdateYoyakuInfoBasic(paramYoyakuInfoList)
            '            returnValue = execNonQuery(oracleTransaction, sqlString)
            '        Next
            '    End With
            'End If

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
    ''' コース台帳（ダイヤ）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <param name="syuptJiCarrierKbn">出発時キャリア区分</param>
    ''' <returns>SQL</returns>
    ''' <remarks></remarks>
    Private Function executeUpdateCrsLedgerDia(ByVal paramList As Hashtable,
                                                ByVal syuptJiCarrierKbn As Integer) As String

        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity
        Dim clsCrsLedgerDiaEntity As New CrsLedgerDiaEntity
        Dim sqlString As New StringBuilder

        paramClear()

        With clsCrsLedgerDiaEntity
            'パラメータ設定
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            If syuptJiCarrierKbn = SyuptJiCarrierKbnType.bus Then
                setParam(.jyochachiCd.PhysicsName, paramList.Item(clsCrsLedgerBasicEntity.haisyaKeiyuCd1.PhysicsName), .jyochachiCd.DBType, .jyochachiCd.IntegerBu, .jyochachiCd.DecimalBu)
                setParam(.syugoTime.PhysicsName, _comTehai.nnvl_int(paramList.Item(clsCrsLedgerBasicEntity.syugoTime1.PhysicsName), 0), .syugoTime.DBType, .syugoTime.IntegerBu, .syugoTime.DecimalBu)
                setParam(.syuptTime.PhysicsName, _comTehai.nnvl_int(paramList.Item(clsCrsLedgerBasicEntity.syuptTime1.PhysicsName), 0), .syuptTime.DBType, .syuptTime.IntegerBu, .syuptTime.DecimalBu)
            Else
                setParam(.jyochachiCd.PhysicsName, paramList.Item(clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.PhysicsName), .jyochachiCd.DBType, .jyochachiCd.IntegerBu, .jyochachiCd.DecimalBu)
                setParam(.syugoTime.PhysicsName, _comTehai.nnvl_int(paramList.Item(clsCrsLedgerBasicEntity.syugoTimeCarrier.PhysicsName), 0), .syugoTime.DBType, .syugoTime.IntegerBu, .syugoTime.DecimalBu)
                setParam(.syuptTime.PhysicsName, _comTehai.nnvl_int(paramList.Item(clsCrsLedgerBasicEntity.syuptTimeCarrier.PhysicsName), 0), .syuptTime.DBType, .syuptTime.IntegerBu, .syuptTime.DecimalBu)
            End If
            setParam(.systemUpdateDay.PhysicsName, paramList.Item(.systemUpdateDay.PhysicsName), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu)
            setParam(.systemUpdatePgmid.PhysicsName, paramList.Item(.systemUpdatePgmid.PhysicsName), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu)
                setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item(.systemUpdatePersonCd.PhysicsName), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu)

                'UPDATE
                sqlString.AppendLine(" UPDATE T_CRS_LEDGER_DIA ")
                sqlString.AppendLine(" SET ")
                sqlString.AppendLine(" JYOCHACHI_CD = :" & .jyochachiCd.PhysicsName)
                sqlString.AppendLine(",SYUGO_TIME = :" & .syugoTime.PhysicsName)
                sqlString.AppendLine(",SYUPT_TIME = :" & .syuptTime.PhysicsName)
                sqlString.AppendLine(",SYSTEM_UPDATE_DAY = :" & .systemUpdateDay.PhysicsName)
                sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = :" & .systemUpdatePgmid.PhysicsName)
                sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = :" & .systemUpdatePersonCd.PhysicsName)
                'WHERE句
                sqlString.AppendLine(" WHERE ")
                sqlString.AppendLine(" SYUPT_DAY = :" & .syuptDay.PhysicsName)
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" CRS_CD = :" & .crsCd.PhysicsName)
                sqlString.AppendLine(" AND ")
                sqlString.AppendLine(" GOUSYA = :" & .gousya.PhysicsName)
        End With
        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' コース台帳（基本）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <param name="initFlg">初期化フラグ</param>
    ''' <param name="syuptJiCarrierKbn">出発時キャリア区分</param>
    ''' <returns>SQL</returns>
    ''' <remarks></remarks>
    Private Function executeUpdateCrsLedgerBasic(ByVal paramList As Hashtable,
                                                    Optional ByVal initFlg As Boolean = False,
                                                    Optional ByVal syuptJiCarrierKbn As Integer = Nothing) As String

        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity
        Dim sqlString As New StringBuilder

        paramClear()

        With clsCrsLedgerBasicEntity
            'パラメータ設定
            setParam(.syuptDay.PhysicsName, paramList.Item(.syuptDay.PhysicsName), .syuptDay.DBType, .syuptDay.IntegerBu, .syuptDay.DecimalBu)
            setParam(.crsCd.PhysicsName, paramList.Item(.crsCd.PhysicsName), .crsCd.DBType, .crsCd.IntegerBu, .crsCd.DecimalBu)
            setParam(.gousya.PhysicsName, paramList.Item(.gousya.PhysicsName), .gousya.DBType, .gousya.IntegerBu, .gousya.DecimalBu)
            setParam(.systemUpdateDay.PhysicsName, paramList.Item(.systemUpdateDay.PhysicsName), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu)
            setParam(.systemUpdatePgmid.PhysicsName, paramList.Item(.systemUpdatePgmid.PhysicsName), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu)
            setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item(.systemUpdatePersonCd.PhysicsName), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu)
            setParam(.syoyoTime.PhysicsName, paramList.Item(.syoyoTime.PhysicsName), .syoyoTime.DBType, .syoyoTime.IntegerBu, .syoyoTime.DecimalBu)
            If initFlg = False Then
                setParam(.haisyaKeiyuCd1.PhysicsName, paramList.Item(.haisyaKeiyuCd1.PhysicsName), .haisyaKeiyuCd1.DBType, .haisyaKeiyuCd1.IntegerBu, .haisyaKeiyuCd1.DecimalBu)
                If syuptJiCarrierKbn = SyuptJiCarrierKbnType.bus Then
                    setParam(.haisyaKeiyuCd2.PhysicsName, paramList.Item(.haisyaKeiyuCd2.PhysicsName), .haisyaKeiyuCd2.DBType, .haisyaKeiyuCd2.IntegerBu, .haisyaKeiyuCd2.DecimalBu)
                    setParam(.haisyaKeiyuCd3.PhysicsName, paramList.Item(.haisyaKeiyuCd3.PhysicsName), .haisyaKeiyuCd3.DBType, .haisyaKeiyuCd3.IntegerBu, .haisyaKeiyuCd3.DecimalBu)
                    setParam(.haisyaKeiyuCd4.PhysicsName, paramList.Item(.haisyaKeiyuCd4.PhysicsName), .haisyaKeiyuCd4.DBType, .haisyaKeiyuCd4.IntegerBu, .haisyaKeiyuCd4.DecimalBu)
                    setParam(.haisyaKeiyuCd5.PhysicsName, paramList.Item(.haisyaKeiyuCd5.PhysicsName), .haisyaKeiyuCd5.DBType, .haisyaKeiyuCd5.IntegerBu, .haisyaKeiyuCd5.DecimalBu)
                    setParam(.syugoTime1.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTime1.PhysicsName), 0), .syugoTime1.DBType, .syugoTime1.IntegerBu, .syugoTime1.DecimalBu)
                    setParam(.syugoTime2.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTime2.PhysicsName), 0), .syugoTime2.DBType, .syugoTime2.IntegerBu, .syugoTime2.DecimalBu)
                    setParam(.syugoTime3.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTime3.PhysicsName), 0), .syugoTime3.DBType, .syugoTime3.IntegerBu, .syugoTime3.DecimalBu)
                    setParam(.syugoTime4.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTime4.PhysicsName), 0), .syugoTime4.DBType, .syugoTime4.IntegerBu, .syugoTime4.DecimalBu)
                    setParam(.syugoTime5.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTime5.PhysicsName), 0), .syugoTime5.DBType, .syugoTime5.IntegerBu, .syugoTime5.DecimalBu)
                    setParam(.syuptTime1.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTime1.PhysicsName), 0), .syuptTime1.DBType, .syuptTime1.IntegerBu, .syuptTime1.DecimalBu)
                    setParam(.syuptTime2.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTime2.PhysicsName), 0), .syuptTime2.DBType, .syuptTime2.IntegerBu, .syuptTime2.DecimalBu)
                    setParam(.syuptTime3.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTime3.PhysicsName), 0), .syuptTime3.DBType, .syuptTime3.IntegerBu, .syuptTime3.DecimalBu)
                    setParam(.syuptTime4.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTime4.PhysicsName), 0), .syuptTime4.DBType, .syuptTime4.IntegerBu, .syuptTime4.DecimalBu)
                    setParam(.syuptTime5.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTime5.PhysicsName), 0), .syuptTime5.DBType, .syuptTime5.IntegerBu, .syuptTime5.DecimalBu)
                    setParam(.endPlaceCd.PhysicsName, paramList.Item(.endPlaceCd.PhysicsName), .endPlaceCd.DBType, .endPlaceCd.IntegerBu, .endPlaceCd.DecimalBu)
                    setParam(.endTime.PhysicsName, _comTehai.nnvl_int(paramList.Item(.endTime.PhysicsName), 0), .endTime.DBType, .endTime.IntegerBu, .endTime.DecimalBu)
                Else
                    setParam(.syugoPlaceCdCarrier.PhysicsName, paramList.Item(.syugoPlaceCdCarrier.PhysicsName), .syugoPlaceCdCarrier.DBType, .syugoPlaceCdCarrier.IntegerBu, .syugoPlaceCdCarrier.DecimalBu)
                    setParam(.syugoTimeCarrier.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syugoTimeCarrier.PhysicsName), 0), .syugoTimeCarrier.DBType, .syugoTimeCarrier.IntegerBu, .syugoTimeCarrier.DecimalBu)
                    setParam(.syuptPlaceCarrier.PhysicsName, paramList.Item(.syuptPlaceCarrier.PhysicsName), .syuptPlaceCarrier.DBType, .syuptPlaceCarrier.IntegerBu, .syuptPlaceCarrier.DecimalBu)
                    setParam(.syuptPlaceCdCarrier.PhysicsName, paramList.Item(.syuptPlaceCdCarrier.PhysicsName), .syuptPlaceCdCarrier.DBType, .syuptPlaceCdCarrier.IntegerBu, .syuptPlaceCdCarrier.DecimalBu)
                    setParam(.syuptTimeCarrier.PhysicsName, _comTehai.nnvl_int(paramList.Item(.syuptTimeCarrier.PhysicsName), 0), .syuptTimeCarrier.DBType, .syuptTimeCarrier.IntegerBu, .syuptTimeCarrier.DecimalBu)
                    setParam(.ttyakPlaceCarrier.PhysicsName, paramList.Item(.ttyakPlaceCarrier.PhysicsName), .ttyakPlaceCarrier.DBType, .ttyakPlaceCarrier.IntegerBu, .ttyakPlaceCarrier.DecimalBu)
                    setParam(.ttyakPlaceCdCarrier.PhysicsName, paramList.Item(.ttyakPlaceCdCarrier.PhysicsName), .ttyakPlaceCdCarrier.DBType, .ttyakPlaceCdCarrier.IntegerBu, .ttyakPlaceCdCarrier.DecimalBu)
                    setParam(.ttyakTimeCarrier.PhysicsName, _comTehai.nnvl_int(paramList.Item(.ttyakTimeCarrier.PhysicsName), 0), .ttyakTimeCarrier.DBType, .ttyakTimeCarrier.IntegerBu, .ttyakTimeCarrier.DecimalBu)
                End If
            End If

            'UPDATE
            sqlString.AppendLine(" UPDATE T_CRS_LEDGER_BASIC ")
            sqlString.AppendLine(" SET ")
            sqlString.AppendLine(" SYSTEM_UPDATE_DAY = :" & .systemUpdateDay.PhysicsName)
            'sqlString.AppendLine(" USING_FLG = NULL")
            If initFlg = False Then
                sqlString.AppendLine(",HAISYA_KEIYU_CD_1 = :" & .haisyaKeiyuCd1.PhysicsName)
                If syuptJiCarrierKbn = SyuptJiCarrierKbnType.bus Then
                    sqlString.AppendLine(",HAISYA_KEIYU_CD_2 = :" & .haisyaKeiyuCd2.PhysicsName)
                    sqlString.AppendLine(",HAISYA_KEIYU_CD_3 = :" & .haisyaKeiyuCd3.PhysicsName)
                    sqlString.AppendLine(",HAISYA_KEIYU_CD_4 = :" & .haisyaKeiyuCd4.PhysicsName)
                    sqlString.AppendLine(",HAISYA_KEIYU_CD_5 = :" & .haisyaKeiyuCd5.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_1 = :" & .syugoTime1.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_2 = :" & .syugoTime2.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_3 = :" & .syugoTime3.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_4 = :" & .syugoTime4.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_5 = :" & .syugoTime5.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_1 = :" & .syuptTime1.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_2 = :" & .syuptTime2.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_3 = :" & .syuptTime3.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_4 = :" & .syuptTime4.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_5 = :" & .syuptTime5.PhysicsName)
                    sqlString.AppendLine(",END_PLACE_CD = :" & .endPlaceCd.PhysicsName)
                    sqlString.AppendLine(",END_TIME = :" & .endTime.PhysicsName)
                    If String.IsNullOrEmpty(paramList.Item(.syoyoTime.PhysicsName).ToString()) = False Then
                        sqlString.AppendLine(",SYOYO_TIME = :" & .syoyoTime.PhysicsName)
                    End If
                Else
                    sqlString.AppendLine(",SYUGO_TIME_1 = :" & .syugoTimeCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUGO_PLACE_CD_CARRIER = :" & .syugoPlaceCdCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUGO_TIME_CARRIER = :" & .syugoTimeCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_1 = :" & .syuptTimeCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUPT_PLACE_CARRIER = :" & .syuptPlaceCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUPT_PLACE_CD_CARRIER = :" & .syuptPlaceCdCarrier.PhysicsName)
                    sqlString.AppendLine(",SYUPT_TIME_CARRIER = :" & .syuptTimeCarrier.PhysicsName)
                    sqlString.AppendLine(",TTYAK_PLACE_CARRIER = :" & .ttyakPlaceCarrier.PhysicsName)
                    sqlString.AppendLine(",TTYAK_PLACE_CD_CARRIER = :" & .ttyakPlaceCdCarrier.PhysicsName)
                    sqlString.AppendLine(",TTYAK_TIME_CARRIER = :" & .ttyakTimeCarrier.PhysicsName)
                End If
            End If
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = :" & .systemUpdatePgmid.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = :" & .systemUpdatePersonCd.PhysicsName)
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine(" SYUPT_DAY = :" & .syuptDay.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" CRS_CD = :" & .crsCd.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" GOUSYA = :" & .gousya.PhysicsName)
        End With
        Return sqlString.ToString

    End Function

    ''' <summary>
    ''' 予約情報（基本）：データ更新用
    ''' </summary>
    ''' <param name="paramList">SQL引数</param>
    ''' <returns>SQL</returns>
    ''' <remarks></remarks>
    Private Function executeUpdateYoyakuInfoBasic(ByVal paramList As Hashtable) As String

        Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity
        Dim sqlString As New StringBuilder

        paramClear()

        With clsYoyakuInfoBasicEntity
            'パラメータ設定
            setParam(.yoyakuKbn.PhysicsName, paramList.Item(.yoyakuKbn.PhysicsName), .yoyakuKbn.DBType, .yoyakuKbn.IntegerBu, .yoyakuKbn.DecimalBu)
            setParam(.yoyakuNo.PhysicsName, paramList.Item(.yoyakuNo.PhysicsName), .yoyakuNo.DBType, .yoyakuNo.IntegerBu, .yoyakuNo.DecimalBu)
            setParam(.systemUpdateDay.PhysicsName, paramList.Item(.systemUpdateDay.PhysicsName), .systemUpdateDay.DBType, .systemUpdateDay.IntegerBu, .systemUpdateDay.DecimalBu)
            setParam(.systemUpdatePgmid.PhysicsName, paramList.Item(.systemUpdatePgmid.PhysicsName), .systemUpdatePgmid.DBType, .systemUpdatePgmid.IntegerBu, .systemUpdatePgmid.DecimalBu)
            setParam(.systemUpdatePersonCd.PhysicsName, paramList.Item(.systemUpdatePersonCd.PhysicsName), .systemUpdatePersonCd.DBType, .systemUpdatePersonCd.IntegerBu, .systemUpdatePersonCd.DecimalBu)

            'UPDATE
            sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC ")
            sqlString.AppendLine(" SET ")
            sqlString.AppendLine(" USING_FLG = NULL")
            sqlString.AppendLine(",SYSTEM_UPDATE_DAY = :" & .systemUpdateDay.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PGMID = :" & .systemUpdatePgmid.PhysicsName)
            sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD = :" & .systemUpdatePersonCd.PhysicsName)
            'WHERE句
            sqlString.AppendLine(" WHERE ")
            sqlString.AppendLine(" YOYAKU_KBN = :" & .yoyakuKbn.PhysicsName)
            sqlString.AppendLine(" AND ")
            sqlString.AppendLine(" YOYAKU_NO = :" & .yoyakuNo.PhysicsName)
        End With
        Return sqlString.ToString

    End Function

#End Region

End Class
