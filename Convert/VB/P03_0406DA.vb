Imports System.Text
Imports Hatobus.ReservationManagementSystem.Master
''' <summary>
''' 催行決定・中止連絡（仕入先／直販客・代理店）のDAクラス
''' </summary>
Public Class P03_0406DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
    ''' <summary>
    ''' 通知先連絡データ
    ''' </summary>
    Private Enum NoticeDt As Integer
        '予約区分
        YOYAKU_KBN
        '予約No
        YOYAKU_NO
        '業者コード
        AGENT_CD
    End Enum

#End Region

#Region " SELECT処理 "
    ''' <summary>
    ''' コース台帳（基本）検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableTCrsLedgerBasic(ByVal param As DataTable) As DataTable
        Dim sb As New StringBuilder
        Dim YoyakuNo As String = If(param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString).ToString, String.Empty)
        Dim YoyakuKbn As String = If(param.Rows(0).Field(Of String)(NoticeDt.YOYAKU_KBN.ToString), String.Empty)

        'パラメータクリア
        clear()
        sb.AppendLine("  SELECT  ")
        sb.AppendLine("  TCLB.CRS_NAME AS CRS_NAME, ")
        sb.AppendLine("  MP1.PLACE_NAME_1 AS SYUGO_PLACE1, ")
        sb.AppendLine("  MP2.PLACE_NAME_1 AS SYUGO_PLACE2, ")
        sb.AppendLine("  MP3.PLACE_NAME_1 AS SYUGO_PLACE3, ")
        sb.AppendLine("  MP4.PLACE_NAME_1 AS SYUGO_PLACE4, ")
        sb.AppendLine("  MP5.PLACE_NAME_1 AS SYUGO_PLACE5, ")
        sb.AppendLine("  MPC.PLACE_NAME_1 AS SYUGO_PLACE_CARRIER, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_1) AS SYUGO_TIME1, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_2) AS SYUGO_TIME2, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_3) AS SYUGO_TIME3, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_4) AS SYUGO_TIME4, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_5) AS SYUGO_TIME5, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUGO_TIME_CARRIER) AS SYUGO_TIME_CARRIER, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_1) AS SYUPT_TIME1, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_2) AS SYUPT_TIME2, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_3) AS SYUPT_TIME3, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_4) AS SYUPT_TIME4, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_5) AS SYUPT_TIME5, ")
        sb.AppendLine("  TO_HHMM_FC(TCLB.SYUPT_TIME_CARRIER) AS SYUPT_TIME_CARRIER, ")
        sb.AppendLine("  TCLB.SYUPT_JI_CARRIER_KBN AS SYUPT_JI_CARRIER_KBN, ")
        sb.AppendLine("  TYIB.JYOSYA_NINZU_1 AS JYOSYA_NINZU_1, ")
        sb.AppendLine("  TYIB.JYOSYA_NINZU_2 AS JYOSYA_NINZU_2, ")
        sb.AppendLine("  TYIB.JYOSYA_NINZU_3 AS JYOSYA_NINZU_3, ")
        sb.AppendLine("  TYIB.JYOSYA_NINZU_4 AS JYOSYA_NINZU_4, ")
        sb.AppendLine("  TYIB.JYOSYA_NINZU_5 AS JYOSYA_NINZU_5, ")
        sb.AppendLine("  YOYAKU_NINZU.CHARGE_APPLICATION_NINZU AS NINZU")
        sb.AppendLine("  FROM T_YOYAKU_INFO_BASIC TYIB ")
        sb.AppendLine("  INNER JOIN  ")
        sb.AppendLine("  T_CRS_LEDGER_BASIC TCLB ON TYIB.CRS_CD = TCLB.CRS_CD ")
        sb.AppendLine("  AND TCLB.SYUPT_DAY = TYIB.SYUPT_DAY ")
        sb.AppendLine("  AND TCLB.GOUSYA = TYIB.GOUSYA ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MP1 ON TCLB.HAISYA_KEIYU_CD_1 = MP1.PLACE_CD ")
        sb.AppendLine("  AND MP1.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MP2 ON TCLB.HAISYA_KEIYU_CD_2 = MP2.PLACE_CD ")
        sb.AppendLine("  AND MP2.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MP3 ON TCLB.HAISYA_KEIYU_CD_3 = MP3.PLACE_CD ")
        sb.AppendLine("  AND MP3.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MP4 ON TCLB.HAISYA_KEIYU_CD_4 = MP4.PLACE_CD ")
        sb.AppendLine("  AND MP4.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MP5 ON TCLB.HAISYA_KEIYU_CD_5 = MP5.PLACE_CD ")
        sb.AppendLine("  AND MP5.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN  ")
        sb.AppendLine("  M_PLACE MPC ON TCLB.SYUGO_PLACE_CD_CARRIER = MPC.PLACE_CD ")
        sb.AppendLine("  AND MPC.DELETE_DATE IS NULL ")
        sb.AppendLine("  LEFT JOIN (SELECT SUM(CHARGE_APPLICATION_NINZU) AS CHARGE_APPLICATION_NINZU, ")
        sb.AppendLine("  YOYAKU_KBN, ")
        sb.AppendLine("  YOYAKU_NO ")
        sb.AppendLine("  FROM T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ")
        sb.AppendLine("  WHERE YOYAKU_KBN = '" & YoyakuKbn & "'")
        sb.AppendLine("  AND YOYAKU_NO = '" & YoyakuNo & "'")
        sb.AppendLine("  GROUP BY YOYAKU_KBN,YOYAKU_NO)  YOYAKU_NINZU  ")
        sb.AppendLine("  ON YOYAKU_NINZU.YOYAKU_KBN = TYIB.YOYAKU_KBN ")
        sb.AppendLine("  AND YOYAKU_NINZU.YOYAKU_NO = TYIB.YOYAKU_NO ")
        sb.AppendLine("  WHERE ")
        sb.AppendLine("  TYIB.YOYAKU_KBN = '" & YoyakuKbn & "'")
        sb.AppendLine("  AND TYIB.YOYAKU_NO = '" & YoyakuNo & "'")


        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 予約情報検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableTYoyakuInfoBasic(ByVal param As DataTable) As DataTable
        Dim sb As New StringBuilder
        Dim YoyakuNo As String = If(param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString).ToString, String.Empty)
        Dim YoyakuKbn As String = If(param.Rows(0).Field(Of String)(NoticeDt.YOYAKU_KBN.ToString), String.Empty)

        'パラメータクリア
        clear()
        sb.AppendLine("SELECT  ")
        sb.AppendLine("YUBIN_NO, ")
        sb.AppendLine("ADDRESS_1, ")
        sb.AppendLine("ADDRESS_2, ")
        sb.AppendLine("ADDRESS_3 ")
        sb.AppendLine("FROM T_YOYAKU_INFO_BASIC ")
        sb.AppendLine("WHERE YOYAKU_KBN = '" & YoyakuKbn & "'")
        sb.AppendLine("AND YOYAKU_NO = '" & YoyakuNo & "'")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 代理店マスタ検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectDataTableAgent(ByVal param As DataTable) As DataTable
        Dim sb As New StringBuilder
        Dim AgentCd As String = If(param.Rows(0).Field(Of String)(NoticeDt.AGENT_CD.ToString), String.Empty)

        'パラメータクリア
        clear()
        sb.AppendLine("SELECT YUBIN_NO,  ")
        sb.AppendLine("FAX_1 || '-' || FAX_2 || '-' || FAX_3 AS FAXSTR ")
        sb.AppendLine("FROM M_AGENT ")
        sb.AppendLine("WHERE AGENT_CD = '" & AgentCd & "'")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 代理店マスタ検索処理を呼び出す
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns>DataTable</returns>
    Public Function selectUsedayStr(ByVal param As DataTable) As DataTable
        Dim sb As New StringBuilder
        Dim YoyakuNo As String = If(param.Rows(0).Item(NoticeDt.YOYAKU_NO.ToString).ToString, String.Empty)
        Dim YoyakuKbn As String = If(param.Rows(0).Field(Of String)(NoticeDt.YOYAKU_KBN.ToString), String.Empty)

        'パラメータクリア
        clear()
        sb.AppendLine("SELECT TCL.USEDAY_STR AS USEDAY_STR ")
        sb.AppendLine("FROM T_YOYAKU_INFO_BASIC TYIB ")
        sb.AppendLine("LEFT JOIN ")
        sb.AppendLine("  (SELECT TO_YYYYMMDD_FC(RIYOU_DAY) AS USEDAY_STR, ")
        sb.AppendLine("    CRS_CD, ")
        sb.AppendLine("    SYUPT_DAY, ")
        sb.AppendLine("    GOUSYA ")
        sb.AppendLine("  FROM T_CRS_LEDGER_HOTEL ")
        sb.AppendLine("  UNION ")
        sb.AppendLine("  SELECT TO_YYYYMMDD_FC(RIYOU_DAY) AS USEDAY_STR, ")
        sb.AppendLine("    CRS_CD, ")
        sb.AppendLine("    SYUPT_DAY, ")
        sb.AppendLine("    GOUSYA ")
        sb.AppendLine("  FROM T_CRS_LEDGER_KOSHAKASHO ")
        sb.AppendLine("  ) TCL ")
        sb.AppendLine("ON TCL.CRS_CD         = TYIB.CRS_CD ")
        sb.AppendLine("AND TCL.SYUPT_DAY     = TYIB.SYUPT_DAY ")
        sb.AppendLine("AND TCL.GOUSYA        = TYIB.GOUSYA ")
        sb.AppendLine("WHERE TYIB.YOYAKU_KBN = '" & YoyakuKbn & "'")
        sb.AppendLine("AND TYIB.YOYAKU_NO    = '" & YoyakuNo & "'")
        sb.AppendLine("GROUP BY USEDAY_STR")
        sb.AppendLine("ORDER BY USEDAY_STR")

        Return MyBase.getDataTable(sb.ToString)
    End Function

    ''' <summary>
    ''' 通知内容取得
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function selectMCode(ByVal param As P03_0406DAGetMCodeParam) As DataTable
        Dim code As New MCodeEntity
        'SQL文字列
        Dim sb As New StringBuilder
        'パラメータクリア
        clear()
        'SELECT句
        sb.AppendLine(" SELECT ")
        sb.AppendLine(" CODE_VALUE ")
        sb.AppendLine("   , NAIYO_1")
        'FROM句
        sb.AppendLine(" FROM ")
        sb.AppendLine("   M_CODE ")
        'WHERE句
        sb.AppendLine("WHERE ")
        'コード分類
        sb.AppendLine("CODE_BUNRUI = ").Append(setSelectParam(param.CodeBunrui, code.CodeBunrui))
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
    Public Class P03_0406DAGetMCodeParam
        ''' <summary>
        ''' コード分類
        ''' </summary>
        Public Property CodeBunrui As String
    End Class
#End Region
End Class
