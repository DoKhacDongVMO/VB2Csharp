Imports System.Text

''' <summary>
''' リクエスト仕入登録・照会	のDAクラス
''' </summary>
Public Class S03_0801DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

#Region " SELECT処理 "

    ''' <summary>
    ''' 検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTable(ByVal param As S03_0801DASelectParam) As DataTable
        Dim reqInfoEtt As New WtRequestInfoEntity
        Dim basicEtt As New CrsLedgerBasicEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("       CASE STATE ")
        sb.AppendLine("         WHEN '1' THEN 1 ")
        sb.AppendLine("         ELSE 0 ")
        sb.AppendLine("       END AS STATE ")                                                                               '状態
        sb.AppendLine("     , CASE CANCEL_FLG ")
        sb.AppendLine("         WHEN '1' THEN 1 ")
        sb.AppendLine("         ELSE 0 ")
        sb.AppendLine("       END AS CANCEL_FLG ")                                                                          'キャンセルフラグ
        sb.AppendLine("     , TO_CHAR(TO_DATE(ENTRY_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS RESERVATION_DATE_STR ")                '登録日
        sb.AppendLine("     , TO_CHAR(TO_DATE(SYUPT_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS SYUPT_DAY_STR ")                       '出発日
        sb.AppendLine("     , CRS_CD AS CRS_CD ")                                                                           'コースコード
        sb.AppendLine("     , CRS_NAME AS CRS_NAME ")                                                                       'コース名
        sb.AppendLine("     , TO_CHAR(GOUSYA) AS GOUSYA ")                                                                  '号車
        sb.AppendLine("     , MANAGEMENT_KBN || ',' || TO_CHAR(MANAGEMENT_NO, 'FM000,000,000') AS REQUEST_NO ")             '管理区分 + 管理ＮＯ
        sb.AppendLine("     , NAME AS NAME ")                                                                               '名前
        sb.AppendLine("     , NINZU AS YOYAKU_NUM ")                                                                        '人数
        sb.AppendLine("     , TEL_NO AS TEL_NO ")                                                                           '電話番号
        sb.AppendLine("     , TO_CHAR(TO_DATE(UPDATE_DAY, 'YYYYMMDD'), 'YY/MM/DD') AS UPDATE_DAY_STR ")                     '更新日
        sb.AppendLine("     , MEMO AS MEMO ")                                                                               '内容
        sb.AppendLine("     , TO_CHAR(MEMO_UPDATE_DAY, 'YYYY/MM/DD HH24:MI') AS MEMO_UPDATE_DAY ")                          'システム更新日
        sb.AppendLine("     , MEMO_UPDATE_PERSON AS MEMO_UPDATE_PERSON_CD ")                                                'システム更新者コード
        sb.AppendLine("     , MANAGEMENT_KBN AS MANAGEMENT_KBN_HIDDEN ")                                                    '管理区分
        sb.AppendLine("     , MANAGEMENT_NO AS MANAGEMENT_NO_HIDDEN ")                                                      '管理ＮＯ
        'FROM句
        sb.AppendLine("FROM P03_0801")
        sb.AppendLine("WHERE 1 = 1 ")
        '予約受付日/出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            If param.Kijyun = True Then
                sb.AppendLine("  AND ")
                sb.AppendLine("  ENTRY_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, reqInfoEtt.entryDay))
            ElseIf param.Kijyun = False Then
                sb.AppendLine("  AND ")
                sb.AppendLine("  SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEtt.syuptDay))
            End If
        End If
        '予約受付日/出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            If param.Kijyun = True Then
                sb.AppendLine("  AND ")
                sb.AppendLine("  ENTRY_DAY <= ").Append(setSelectParam(param.SyuptDayTo, reqInfoEtt.entryDay))
            ElseIf param.Kijyun = False Then
                sb.AppendLine("  AND ")
                sb.AppendLine("  SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEtt.syuptDay))
            End If
        End If
        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEtt.crsCd))
        End If
        '邦人／外客区分
        If param.CrsJapanese = True AndAlso param.CrsForeign = False Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  HOUJIN_GAIKYAKU_KBN = ").Append(setSelectParam(HoujinGaikyakuKbnType.Houjin, basicEtt.houjinGaikyakuKbn))
        ElseIf param.CrsJapanese = False AndAlso param.CrsForeign = True Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  HOUJIN_GAIKYAKU_KBN = ").Append(setSelectParam(HoujinGaikyakuKbnType.Gaikyaku, basicEtt.houjinGaikyakuKbn))
        End If
        If param.CrsKbnHiru = True OrElse param.CrsKbnYoru = True OrElse param.CrsKbnDay = True OrElse param.CrsKbnStay = True OrElse param.CrsKbnR = True Then
            'コース種別/コース区分１
            sb.AppendLine(" AND ")
            sb.AppendLine(" ( ")
            sb.AppendLine("  1 <> 1 ")
            '定期（昼） = TRUEの場合
            If param.CrsKbnHiru = True Then
                sb.AppendLine("  OR ")
                sb.AppendLine("  (")
                sb.AppendLine("   CRS_KIND = ").Append(setSelectParam(CStr(CrsKindType.hatoBusTeiki), basicEtt.crsKind))
                sb.AppendLine("    AND ")
                sb.AppendLine("   CRS_KBN_1 = ").Append(setSelectParam(CStr(CrsKbn1Type.noon), basicEtt.crsKbn1))
                sb.AppendLine("  )")
            End If
            '定期（夜）＝TRUEの場合
            If param.CrsKbnYoru = True Then
                sb.AppendLine("  OR ")
                sb.AppendLine("  (")
                sb.AppendLine("   CRS_KIND = ").Append(setSelectParam(CStr(CrsKindType.hatoBusTeiki), basicEtt.crsKind))
                sb.AppendLine("    AND ")
                sb.AppendLine("   CRS_KBN_1 = ").Append(setSelectParam(CStr(CrsKbn1Type.night), basicEtt.crsKbn1))
                sb.AppendLine("  )")
            End If
            '企画（日帰り）＝TRUEの場合
            If param.CrsKbnDay = True Then
                sb.AppendLine("  OR ")
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CStr(CrsKindType.higaeri), basicEtt.crsKind))
            End If
            '企画（宿泊）＝TRUEの場合
            If param.CrsKbnStay = True Then
                sb.AppendLine("  OR ")
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CStr(CrsKindType.stay), basicEtt.crsKind))
            End If
            '企画（Ｒコース）＝TRUEの場合
            If param.CrsKbnR = True Then
                sb.AppendLine("  OR ")
                sb.AppendLine("  CRS_KIND = ").Append(setSelectParam(CStr(CrsKindType.rcourse), basicEtt.crsKind))
            End If
            sb.AppendLine(" ) ")
        End If
        '状態
        If param.SiireMachiFlg = True AndAlso param.SiireZumiFlg = False Then
            sb.AppendLine(" AND ")
            sb.AppendLine(" STATE <> ").Append(setSelectParam(1.ToString(), reqInfoEtt.state))
        ElseIf param.SiireMachiFlg = False AndAlso param.SiireZumiFlg = True Then
            sb.AppendLine(" AND ")
            sb.AppendLine(" STATE = ").Append(setSelectParam(1.ToString(), reqInfoEtt.state))
        End If
        'キャンセルフラグ
        If param.DeleteFlg = False Then
            sb.AppendLine(" AND ")
            sb.AppendLine(" CANCEL_FLG IS NULL ")
        End If
        'ORDER句
        sb.AppendLine(" ORDER BY ")
        If param.Kijyun = True Then
            '登録日
            sb.AppendLine("  ENTRY_DAY ")
            '管理区分
            sb.AppendLine("  , MANAGEMENT_KBN ")
            '管理ＮＯ
            sb.AppendLine("  , MANAGEMENT_NO ")
            '枝番
            sb.AppendLine("  , MEMO_UPDATE_DAY DESC ")
        ElseIf param.Kijyun = False Then
            '出発日
            sb.AppendLine("  SYUPT_DAY ")
            'コースコード
            sb.AppendLine("  , CRS_CD ")
            '号車
            sb.AppendLine("  , GOUSYA ")
            '管理区分
            sb.AppendLine("  , MANAGEMENT_KBN ")
            '管理ＮＯ
            sb.AppendLine("  , MANAGEMENT_NO ")
            '枝番
            sb.AppendLine("  , MEMO_UPDATE_DAY DESC ")
        End If

        Return MyBase.getDataTable(sb.ToString)
    End Function
#End Region

#Region " UPDATE処理 "
    ''' <summary>
    ''' 更新処理を呼び出す
    ''' </summary>
    ''' <param name="lsUpdateData"></param>
    ''' <returns>Integer</returns>
    Public Function updateTable(ByVal lsUpdateData As List(Of S03_0801DAUpdateParam)) As Integer
        Dim returnValue As Integer = 0
        Dim sqlString As String = String.Empty
        Dim con As OracleConnection = Nothing

        'コネクション開始
        con = openCon()

        Try
            For Each item As S03_0801DAUpdateParam In lsUpdateData
                sqlString = getUpdateQuery(item)

                returnValue += execNonQuery(con, sqlString)
            Next
        Catch ex As Exception
            Throw
        Finally
            If con IsNot Nothing Then
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                If con IsNot Nothing Then
                    con.Dispose()
                End If
            End If
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' UPDATE用DBアクセス
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>Integer</returns>
    Public Function getUpdateQuery(ByVal param As S03_0801DAUpdateParam) As String
        Dim reqInfoEtt As New WtRequestInfoEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'UPDATE
        sb.AppendLine(" UPDATE T_WT_REQUEST_INFO ")
        sb.AppendLine(" SET ")
        '状態
        If param.SiireFlg = True Then
            sb.AppendLine("  STATE = ").Append(setUpdateParam(1.ToString(), reqInfoEtt.state) + ", ")
        End If
        'キャンセルフラグ
        If param.DeleteFlg = True Then
            sb.AppendLine("  CANCEL_FLG = ").Append(setUpdateParam(1.ToString(), reqInfoEtt.cancelFlg) + ", ")
        Else
            sb.AppendLine("  CANCEL_FLG = NULL, ")
        End If
        '削除日
        If param.DeleteFlg = True Then
            sb.AppendLine("  DELETE_DAY = " & setParam("DELETE_DAY", CommonDateUtil.getSystemTime.ToString(CommonFormatType.dateFormatyyyyMMdd), OracleDbType.Decimal, 8, 0) + ", ")
        Else
            sb.AppendLine("  DELETE_DAY = 0, ")
        End If
        'システム更新ＰＧＭＩＤ
        sb.AppendLine("  SYSTEM_UPDATE_PGMID = ").Append(setUpdateParam(param.SystemUpdatePgmid, reqInfoEtt.systemUpdatePgmid) + ", ")
        'システム更新者コード
        sb.AppendLine("  SYSTEM_UPDATE_PERSON_CD = ").Append(setUpdateParam(param.SystemUpdatePersonCd, reqInfoEtt.systemUpdatePersonCd) + ", ")
        'システム更新日
        sb.AppendLine("  SYSTEM_UPDATE_DAY = ").Append(setUpdateParam(param.SystemUpdateDay, reqInfoEtt.systemUpdateDay))
        'WHERE句
        sb.AppendLine(" WHERE ")
        '管理区分
        sb.AppendLine(" MANAGEMENT_KBN = ").Append(setUpdateParam(param.ManagementKbn, reqInfoEtt.managementKbn))
        '管理ＮＯ
        sb.AppendLine(" AND MANAGEMENT_NO = ").Append(setUpdateParam(param.ManagementNo, reqInfoEtt.managementNo))

        Return sb.ToString

    End Function

    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function

    Public Function setUpdateParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, False)
    End Function

    Private Function setParamEx(ByVal value As Object, ByVal ent As IEntityKoumokuType, ByVal selFlg As Boolean) As String
        ParamNum += 1
        If selFlg = True AndAlso TypeOf ent Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType)
        Else
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function

    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
#End Region

#Region " パラメータ "

    Public Class S03_0801DASelectParam
        ''' <summary>
        ''' 出発日FROM
        ''' </summary>
        Public Property SyuptDayFrom As Integer?
        ''' <summary>
        ''' 出発日TO
        ''' </summary>
        Public Property SyuptDayTo As Integer?
        ''' <summary>
        ''' コースコード
        ''' </summary>
        Public Property CrsCd As String
        ''' <summary>
        ''' 日本語
        ''' </summary>
        Public Property CrsJapanese As Boolean
        ''' <summary>
        ''' 外国語
        ''' </summary>
        Public Property CrsForeign As Boolean
        ''' <summary>
        ''' 定期（昼）
        ''' </summary>
        Public Property CrsKbnHiru As Boolean
        ''' <summary>
        ''' 定期（夜）
        ''' </summary>
        Public Property CrsKbnYoru As Boolean
        ''' <summary>
        ''' 企画（日帰り）
        ''' </summary>
        Public Property CrsKbnDay As Boolean
        ''' <summary>
        ''' 企画（宿泊）
        ''' </summary>
        Public Property CrsKbnStay As Boolean
        ''' <summary>
        ''' 企画（Ｒコース）
        ''' </summary>
        Public Property CrsKbnR As Boolean
        ''' <summary>
        ''' 仕入れ待ち
        ''' </summary>
        Public Property SiireMachiFlg As Boolean
        ''' <summary>
        ''' 仕入れ済み
        ''' </summary>
        Public Property SiireZumiFlg As Boolean
        ''' <summary>
        ''' 削除含む
        ''' </summary>
        Public Property DeleteFlg As Boolean
        ''' <summary>
        ''' 基準
        ''' </summary>
        Public Property Kijyun As Boolean
    End Class

    Public Class S03_0801DAUpdateParam
        ''' <summary>
        ''' 管理区分
        ''' </summary>
        Public Property ManagementKbn As String
        ''' <summary>
        ''' 管理ＮＯ
        ''' </summary>
        Public Property ManagementNo As Integer
        ''' <summary>
        ''' 仕入
        ''' </summary>
        Public Property SiireFlg As Boolean
        ''' <summary>
        ''' 削除
        ''' </summary>
        Public Property DeleteFlg As Boolean
        ''' <summary>
        ''' システム更新ＰＧＭＩＤ
        ''' </summary>
        Public Property SystemUpdatePgmid As String
        ''' <summary>
        ''' システム更新者コード
        ''' </summary>
        Public Property SystemUpdatePersonCd As String
        ''' <summary>
        ''' システム更新日
        ''' </summary>
        Public Property SystemUpdateDay As Date
    End Class
#End Region
End Class