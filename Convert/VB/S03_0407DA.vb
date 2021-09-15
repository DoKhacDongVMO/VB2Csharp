Imports System.Text

''' <summary>
''' 催行決定・中止連絡履歴のDAクラス
''' </summary>
Public Class S03_0407DA
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
    Public Function selectDataTable(ByVal param As S03_0407DASelectParam) As DataTable
        'SQL文字列
        Dim sb As New StringBuilder
        Dim historyEntity As New TSaikouContactHistoryEntity
        'パラメータクリア
        clear()

        'SELECT句
        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_CHAR(HIS.CONTACT_DAY,'YYYY/MM/DD') AS CONTACT_DAY_STR ")                             '送信日時
        sb.AppendLine("    , CASE WHEN HIS.CONTACT_KIND = '1' THEN '催行決定' ")
        sb.AppendLine("      WHEN HIS.CONTACT_KIND = '2' THEN '催行中止' ")
        sb.AppendLine("      ELSE NULL ")
        sb.AppendLine("      END AS CONTACT_CONTENT ")                                                             '通知種別
        sb.AppendLine("    , CASE WHEN HIS.CONTACT_TO = '1' THEN '仕入先' ")
        sb.AppendLine("      WHEN HIS.CONTACT_TO = '2' THEN '代理店' ")
        sb.AppendLine("      WHEN HIS.CONTACT_TO = '3' THEN '予約者' ")
        sb.AppendLine("      ELSE NULL ")
        sb.AppendLine("      END AS CONTACT_TO ")                                                                  '通知先
        sb.AppendLine("    , CASE WHEN HIS.SIIRE_SAKI_CD IS NOT NULL ")
        sb.AppendLine("           THEN CONCAT(HIS.SIIRE_SAKI_CD, HIS.SIIRE_SAKI_EDABAN) ")                         '仕入先コード + 仕入先枝番
        sb.AppendLine("      WHEN HIS.SIIRE_SAKI_CD IS NULL ")
        sb.AppendLine("           AND (HIS.AGENT_CD IS NULL OR (SUBSTR(HIS.AGENT_CD,1,4) IN ('0001','0002'))) ")
        sb.AppendLine("           THEN CONCAT(HIS.YOYAKU_KBN, HIS.YOYAKU_NO) ")                                    '予約区分 + 予約No
        sb.AppendLine("      ELSE HIS.AGENT_CD ")                                                                  '業者コード
        sb.AppendLine("      END AS NUMBER_CODE ")
        sb.AppendLine("    , CASE WHEN HIS.SIIRE_SAKI_CD IS NOT NULL THEN SIIRE.SIIRE_SAKI_NAME ")                 '仕入先名
        sb.AppendLine("      WHEN HIS.SIIRE_SAKI_CD IS NULL ")
        sb.AppendLine("           AND (HIS.AGENT_CD IS NULL OR (SUBSTR(HIS.AGENT_CD,1,4) IN ('0001','0002'))) ")
        sb.AppendLine("           THEN CONCAT(INFO.SURNAME, INFO.NAME)")                                           '姓 + 名
        sb.AppendLine("      ELSE AGENT.AGENT_NAME ")                                                              '代理店名称
        sb.AppendLine("      END AS NAME ")
        sb.AppendLine("    , CASE WHEN HIS.CONTACT_METHOD = '1' THEN 'メール' ")
        sb.AppendLine("      WHEN HIS.CONTACT_METHOD = '2' THEN 'FAX' ")
        sb.AppendLine("      WHEN HIS.CONTACT_METHOD = '3' THEN 'TEL' ")
        sb.AppendLine("      WHEN HIS.CONTACT_METHOD = '4' THEN '郵送' ")
        sb.AppendLine("      ELSE NULL ")
        sb.AppendLine("      END AS CONTACT_METHOD ")                                                              '通知方法
        sb.AppendLine("    , CASE WHEN HIS.CONTACT_METHOD = '1' THEN HIS.MAIL ")                                   'メール
        sb.AppendLine("      WHEN HIS.CONTACT_METHOD = '2 'THEN HIS.FAX ")                                         'FAX
        sb.AppendLine("      WHEN HIS.CONTACT_METHOD = '3' THEN HIS.TEL ")                                         'TEL
        sb.AppendLine("      ELSE NULL ")
        sb.AppendLine("      END AS CONTENT_INFO ")
        sb.AppendLine("    , CASE WHEN HIS.CONTACT_RESULT = '1' THEN '送信成功'")
        sb.AppendLine("      WHEN HIS.CONTACT_RESULT = '2' THEN '送信失敗'")
        sb.AppendLine("      ELSE NULL ")
        sb.AppendLine("      END AS CONTENT_RESULT ")                                                               '通知結果
        sb.AppendLine("    , M_USER.USER_NAME AS CONTENT_PERSON_NAME ")                                             'ユーザ名
        'FROM句
        sb.AppendLine("FROM T_SAIKOU_CONTACT_HISTORY HIS ")
        sb.AppendLine("INNER JOIN (SELECT YOYAKU_KBN ")
        sb.AppendLine("                   , YOYAKU_NO ")
        sb.AppendLine("                   , SURNAME ")
        sb.AppendLine("                   , NAME ")
        sb.AppendLine("            FROM T_YOYAKU_INFO_BASIC)　INFO ")
        sb.AppendLine("            ON HIS.YOYAKU_KBN = INFO.YOYAKU_KBN ")
        sb.AppendLine("            AND HIS.YOYAKU_NO = INFO.YOYAKU_NO ")
        sb.AppendLine("LEFT JOIN (SELECT AGENT_CD ")
        sb.AppendLine("                  , AGENT_NAME ")
        sb.AppendLine("           FROM M_AGENT)　AGENT ")
        sb.AppendLine("           ON HIS.AGENT_CD = AGENT.AGENT_CD ")
        sb.AppendLine("LEFT JOIN (SELECT SIIRE_SAKI_CD ")
        sb.AppendLine("                  , SIIRE_SAKI_NO ")
        sb.AppendLine("                  , SIIRE_SAKI_NAME ")
        sb.AppendLine("           FROM M_SIIRE_SAKI)　SIIRE ")
        sb.AppendLine("           ON HIS.SIIRE_SAKI_CD = SIIRE.SIIRE_SAKI_CD ")
        sb.AppendLine("           AND HIS.SIIRE_SAKI_EDABAN = SIIRE.SIIRE_SAKI_NO ")
        sb.AppendLine("INNER JOIN (SELECT COMPANY_CD ")
        sb.AppendLine("                   , USER_ID ")
        sb.AppendLine("                   , USER_NAME ")
        sb.AppendLine("            FROM M_USER ")
        sb.AppendLine("            WHERE COMPANY_CD = '0001') M_USER　")
        sb.AppendLine("            ON HIS.CONTACT_PERSON_CD = M_USER.USER_ID ")
        'WHERE句
        sb.AppendLine("WHERE 1 = 1")

        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.Append("    AND HIS.SYUPT_DAY >= ").AppendLine(setSelectParam(param.SyuptDayFrom, historyEntity.SyuptDay))
        End If
        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.Append("    AND HIS.SYUPT_DAY <= ").AppendLine(setSelectParam(param.SyuptDayTo, historyEntity.SyuptDay))
        End If
        'コースコード
        If Not String.IsNullOrEmpty(param.CrsCd) Then
            sb.Append("    AND HIS.CRS_CD = ").AppendLine(setSelectParam(param.CrsCd, historyEntity.CrsCd))
        End If
        'ORDER句
        sb.AppendLine("ORDER BY HIS.CONTACT_DAY ")

        Return MyBase.getDataTable(sb.ToString)
    End Function

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

    Private Sub clear()
        MyBase.paramClear()
        ParamNum = 0
    End Sub
#End Region
#Region " パラメータ "
    Public Class S03_0407DASelectParam
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
    End Class
#End Region
End Class
