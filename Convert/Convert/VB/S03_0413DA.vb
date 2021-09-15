Imports System.Text

''' <summary>
''' 運休連絡のDAクラス
''' </summary>
Public Class S03_0413DA
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
    Public Function selectDataTable(ByVal param As S03_0413DASelectParam) As DataTable
        Dim crsLedgerBasic As New CrsLedgerBasicEntity
        Dim yoyakuInfoBasic As New YoyakuInfoBasicEntity

        'SQL文字列
        Dim sb As New StringBuilder

        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT  ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BASIC.SYUPT_DAY) AS SYUPT_DAY_STR ")                                    '出発日(表示用)
        sb.AppendLine("    , BASIC.SYUPT_DAY AS SYUPT_DAY ")                                                      '出発日
        sb.AppendLine("    , BASIC.CRS_CD AS CRS_CD ")                                                            'コースコード
        sb.AppendLine("    , BASIC.CRS_NAME AS CRS_NAME ")                                                        'コース名
        sb.AppendLine("    , BASIC.GOUSYA AS GOUSYA ")                                                            '号車

        sb.AppendLine("    , CASE WHEN BASIC.TEIKI_KIKAKU_KBN = '1' THEN TO_YYYYMMDD_FC(BASIC.UNKYU_DECIDED_DAY) ") '運休決定日
        sb.AppendLine("      WHEN BASIC.TEIKI_KIKAKU_KBN = '2' THEN TO_YYYYMMDD_FC(BASIC.SAIKOU_DAY) ")             '催行日
        sb.AppendLine("      END AS UNKYU_DECIDE_DAY ")

        sb.AppendLine("    , INFO.YOYAKU_KBN AS YOYAKU_KBN ")                                                     '予約区分
        sb.AppendLine("    , INFO.YOYAKU_NO AS YOYAKU_NO ")                                                       '予約Ｎｏ
        sb.AppendLine("    , CONCAT(INFO.YOYAKU_KBN, INFO.YOYAKU_NO) AS YOYAKU_NO_STR ")                          '予約Ｎｏ（表示用）
        sb.AppendLine("    , CONCAT(INFO.SURNAME, INFO.NAME) AS YOYAKU_NAME ")                                    '予約者名
        sb.AppendLine("    , TO_CHAR(CHARGE.YOYAKU_NUM, '999,999,999,999') AS YOYAKU_NUM ")                       '予約人数
        sb.AppendLine("    , TO_YYYYMMDD_FC(INFO.UNKYU_CONTACT_DAY) AS  UNKYU_CONTACT_DAY")                       '運休連絡日

        'FROM句
        sb.AppendLine("FROM T_CRS_LEDGER_BASIC BASIC ")
        sb.AppendLine("INNER JOIN T_YOYAKU_INFO_BASIC INFO ")
        sb.AppendLine("    ON BASIC.SYUPT_DAY = INFO.SYUPT_DAY ")
        sb.AppendLine("    AND BASIC.CRS_CD = INFO.CRS_CD ")
        sb.AppendLine("    AND BASIC.GOUSYA = INFO.GOUSYA ")
        sb.AppendLine("INNER JOIN ")
        sb.AppendLine("    (SELECT  ")
        sb.AppendLine("        YOYAKU_KBN ")
        sb.AppendLine("        , YOYAKU_NO ")
        sb.AppendLine("        , SUM(CHARGE_APPLICATION_NINZU) AS YOYAKU_NUM ")
        sb.AppendLine("     FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
        sb.AppendLine("     GROUP BY YOYAKU_KBN, YOYAKU_NO")
        sb.AppendLine("    ) CHARGE ")
        sb.AppendLine("    ON INFO.YOYAKU_KBN = CHARGE.YOYAKU_KBN ")
        sb.AppendLine("    AND INFO.YOYAKU_NO = CHARGE.YOYAKU_NO  ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("    NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'0') <> 'M' ")
        sb.AppendLine("    AND (BASIC.UNKYU_KBN = 'Y' OR BASIC.SAIKOU_KAKUTEI_KBN = 'N') ")
        sb.AppendLine("    AND NVL(BASIC.DELETE_DAY, 0) = 0 ")
        sb.AppendLine("    AND (BASIC.YOYAKU_NUM_TEISEKI <> 0 OR BASIC.YOYAKU_NUM_SUB_SEAT <> 0) ")
        sb.AppendLine("    AND INFO.YOYAKU_KBN >= '0' AND INFO.YOYAKU_KBN <= '9' ")
        sb.AppendLine("    AND INFO.CANCEL_FLG IS NULL ")
        sb.AppendLine("    AND NVL(INFO.DELETE_DAY, 0) = 0 ")
        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.Append("    AND BASIC.SYUPT_DAY >= ").AppendLine(setSelectParam(param.SyuptDayFrom, crsLedgerBasic.syuptDay))
        End If
        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.Append("    AND BASIC.SYUPT_DAY <= ").AppendLine(setSelectParam(param.SyuptDayTo, crsLedgerBasic.syuptDay))
        End If
        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.Append("    AND BASIC.CRS_CD = ").AppendLine(setSelectParam(param.CrsCd, crsLedgerBasic.crsCd))
        End If
        'コースコード
        If Not param.Gousya Is Nothing Then
            sb.Append("    AND BASIC.GOUSYA = ").AppendLine(setSelectParam(param.Gousya, crsLedgerBasic.gousya))
        End If
        '邦人／外客区分
        If param.CrsJapanese = True OrElse param.CrsForeign = True Then
            sb.AppendLine("    AND (1 <> 1 ")
            If param.CrsJapanese = True Then
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = ").AppendLine(setSelectParam("H", crsLedgerBasic.houjinGaikyakuKbn))
            End If
            If param.CrsForeign = True Then
                sb.Append("         OR BASIC.HOUJIN_GAIKYAKU_KBN = ").AppendLine(setSelectParam("G", crsLedgerBasic.houjinGaikyakuKbn))
            End If
            sb.AppendLine("        )")
        End If
        '定期
        If param.CrsTeiki = True Then
            sb.Append("    AND BASIC.TEIKI_KIKAKU_KBN = ").AppendLine(setSelectParam("1", crsLedgerBasic.teikiKikakuKbn))
            If param.CrsKbnHiru = True OrElse param.CrsKbnYoru = True OrElse param.CrsKbnKougai = True Then
                sb.AppendLine("    AND (1 <> 1 ")
                '定期（昼）
                If param.CrsKbnHiru = True Then
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind))
                    sb.Append("             AND BASIC.CRS_KBN_1 = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKbn1))
                    sb.AppendLine("             ) ")
                End If
                '定期（夜）
                If param.CrsKbnYoru = True Then
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind))
                    sb.Append("             AND BASIC.CRS_KBN_1 = ").AppendLine(setSelectParam("2", crsLedgerBasic.crsKbn1))
                    sb.AppendLine("             ) ")
                End If
                '定期（郊外）
                If param.CrsKbnKougai = True Then
                    sb.Append("         OR (BASIC.CRS_KIND = ").AppendLine(setSelectParam("1", crsLedgerBasic.crsKind))
                    sb.Append("             AND BASIC.CRS_KBN_2 = ").AppendLine(setSelectParam("2", crsLedgerBasic.crsKbn2))
                    sb.AppendLine("             ) ")
                End If
                sb.AppendLine("        )")
            End If
        End If
        '企画
        If param.CrsKikaku = True Then
            sb.Append("    AND BASIC.TEIKI_KIKAKU_KBN = ").AppendLine(setSelectParam("2", crsLedgerBasic.teikiKikakuKbn))
            If param.CrsKindDay = True OrElse param.CrsKindStay = True OrElse param.CrsKindR = True Then
                sb.AppendLine("    AND (1 <> 1 ")
                '企画（日帰り）
                If param.CrsKindDay = True Then
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("4", crsLedgerBasic.crsKind))
                End If
                '企画（宿泊）
                If param.CrsKindStay = True Then
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("5", crsLedgerBasic.crsKind))
                End If
                '企画（Ｒコース）
                If param.CrsKindR = True Then
                    sb.Append("         OR BASIC.CRS_KIND = ").AppendLine(setSelectParam("6", crsLedgerBasic.crsKind))
                End If
                sb.AppendLine("        )")
            End If
        End If
        '連絡済含む
        If param.ContactIncluding = False Then
            sb.Append("    AND INFO.UNKYU_CONTACT_DAY = ").AppendLine(setSelectParam("0", yoyakuInfoBasic.unkyuContactDay))
        End If
        'ORDER句
        sb.AppendLine(" ORDER BY BASIC.SYUPT_DAY")
        sb.AppendLine("         , BASIC.CRS_CD")
        sb.AppendLine("         , BASIC.GOUSYA")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
    Public Function setSelectParam(ByVal value As Object, ByVal ent As IEntityKoumokuType) As String
        Return setParamEx(value, ent, True)
    End Function
    Private Function setParamEx(ByVal value As Object, ByVal ent As IEntityKoumokuType, ByVal selFlg As Boolean) As String
        ParamNum += 1
        If selFlg = True AndAlso TypeOf ent Is EntityKoumoku_MojiType Then
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType)
        Else
            Return MyBase.setParam(ParamNum.ToString, value, ent.DBType, ent.IntegerBu, ent.DecimalBu)
        End If
    End Function
#End Region

#Region " パラメータ "
    Public Class S03_0413DASelectParam
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
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer?
        ''' <summary>
        ''' 日本語
        ''' </summary>
        Public Property CrsJapanese As Boolean
        ''' <summary>
        ''' 外国語
        ''' </summary>
        Public Property CrsForeign As Boolean
        ''' <summary>
        ''' 定期
        ''' </summary>
        Public Property CrsTeiki As Boolean
        ''' <summary>
        ''' 定期（昼）
        ''' </summary>
        Public Property CrsKbnHiru As Boolean
        ''' <summary>
        ''' 定期（夜）
        ''' </summary>
        Public Property CrsKbnYoru As Boolean
        ''' <summary>
        ''' 定期（郊外）
        ''' </summary>
        Public Property CrsKbnKougai As Boolean
        ''' <summary>
        ''' 企画
        ''' </summary>
        Public Property CrsKikaku As Boolean
        ''' <summary>
        ''' 企画（日帰り）
        ''' </summary>
        Public Property CrsKindDay As Boolean
        ''' <summary>
        ''' 企画（宿泊）
        ''' </summary>
        Public Property CrsKindStay As Boolean
        ''' <summary>
        ''' 企画（Ｒコース）
        ''' </summary>
        Public Property CrsKindR As Boolean
        ''' <summary>
        ''' 連絡済含む
        ''' </summary>
        Public Property ContactIncluding As Boolean
    End Class
#End Region
End Class
