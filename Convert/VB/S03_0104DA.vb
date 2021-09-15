Imports System.Text

Public Class S03_0104DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

    Public Function selectDataTable(ByVal param As S03_0104DASelectParam) As DataTable
        Dim sb As New StringBuilder
        Dim basicEnt As New CrsLedgerBasicEntity
        Dim groupEnt As New CrsLedgerOptionGroupEntity
        Dim siireEnt As New MSiireSakiEntity

        'パラメータクリア
        clear()

        sb.AppendLine("SELECT ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BSC.SYUPT_DAY) AS SYUPT_DAY_STR ")
        sb.AppendLine("  , BSC.SYUPT_DAY AS SYUPT_DAY ")
        sb.AppendLine("  , TO_HHMM_FC(BSC.SYUPT_TIME_1) AS SYUPT_TIME_STR ")
        sb.AppendLine("  , BSC.CRS_CD ")
        sb.AppendLine("  , BSC.GOUSYA ")
        sb.AppendLine("  , OPTG.OPTION_GROUP_NM AS OPTION_GROUP_NM ")
        sb.AppendLine("  , OPT.OPTIONAL_NAME AS OPTION_NM ")
        sb.AppendLine("  , SUM(NVL(YOPT.ADD_CHARGE_APPLICATION_NINZU, 0)) AS OPTION_NUM ")
        sb.AppendLine("  , NVL(BSC.YOYAKU_NUM_TEISEKI, 0) + NVL(BSC.YOYAKU_NUM_SUB_SEAT, 0) AS YOYAKU_NUM ")
        sb.AppendLine("  , COUNT(YOPT.ADD_CHARGE_APPLICATION_NINZU) OPTION_KENSU ")
        sb.AppendLine("  , OPT.HANBAI_TANKA AS HANBAI_TANKA ")
        sb.AppendLine("  , SUM( ")
        sb.AppendLine("    NVL(YOPT.ADD_CHARGE_APPLICATION_NINZU, 0) * NVL(YOPT.ADD_CHARGE_TANKA, 0) ")
        sb.AppendLine("  ) AS TOTAL_COST ")
        sb.AppendLine("  , NULL AS MEMO ")
        sb.AppendLine("  , SIIRE.SIIRE_SAKI_NAME ")
        sb.AppendLine("  , YOPT.GROUP_NO ")
        sb.AppendLine("  , YOPT.LINE_NO ")
        sb.AppendLine("FROM ")
        sb.AppendLine("  T_CRS_LEDGER_BASIC BSC ")
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_OPTION_GROUP OPTG ")
        sb.AppendLine("    ON BSC.CRS_CD = OPTG.CRS_CD ")
        sb.AppendLine("    AND BSC.SYUPT_DAY = OPTG.SYUPT_DAY ")
        sb.AppendLine("    AND BSC.GOUSYA = OPTG.GOUSYA ")
        sb.AppendLine("  INNER JOIN T_CRS_LEDGER_OPTION OPT ")
        sb.AppendLine("    ON OPTG.CRS_CD = OPT.CRS_CD ")
        sb.AppendLine("    AND OPTG.SYUPT_DAY = OPT.SYUPT_DAY ")
        sb.AppendLine("    AND OPTG.GOUSYA = OPT.GOUSYA ")
        sb.AppendLine("    AND OPTG.GROUP_NO = OPT.GROUP_NO ")
        sb.AppendLine("  LEFT JOIN M_SIIRE_SAKI SIIRE ")
        sb.AppendLine("    ON SIIRE.SIIRE_SAKI_CD = OPT.SIIRE_SAKI_CD ")
        sb.AppendLine("    AND SIIRE.SIIRE_SAKI_NO = OPT.SIIRE_SAKI_EDABAN ")
        sb.AppendLine("    AND SIIRE.DELETE_DATE IS NULL ")
        sb.AppendLine("  INNER JOIN T_YOYAKU_INFO_BASIC YBSC ")
        sb.AppendLine("    ON BSC.CRS_CD = YBSC.CRS_CD ")
        sb.AppendLine("    AND BSC.SYUPT_DAY = YBSC.SYUPT_DAY ")
        sb.AppendLine("    AND BSC.GOUSYA = YBSC.GOUSYA                  --予約のみ(WT,R除くなら) ")
        sb.AppendLine("    AND YBSC.YOYAKU_KBN BETWEEN 0 AND 9           --キャンセル除く ")
        sb.AppendLine("    AND YBSC.CANCEL_FLG IS NULL                   --削除日 ")
        sb.AppendLine("    AND YBSC.DELETE_DAY = 0 ")
        sb.AppendLine("  INNER JOIN T_YOYAKU_INFO_OPTION YOPT ")
        sb.AppendLine("    ON YBSC.YOYAKU_KBN = YOPT.YOYAKU_KBN ")
        sb.AppendLine("    AND YBSC.YOYAKU_NO = YOPT.YOYAKU_NO ")
        sb.AppendLine("    AND OPT.GROUP_NO = YOPT.GROUP_NO ")
        sb.AppendLine("    AND OPT.LINE_NO = YOPT.LINE_NO                --削除日 ")
        sb.AppendLine("    AND YOPT.DELETE_DAY = 0 ")
        sb.AppendLine("WHERE ")
        sb.AppendLine("  1 = 1 ")
        '出発日FROM
        If Not param.SyuptDayFrom Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay))
        End If
        '出発日TO
        If Not param.SyuptDayTo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay))
        End If
        'コースコード
        If String.IsNullOrEmpty(param.CrsCd) = False Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd))
        End If
        '出発時間FROM
        If Not param.SyuptTimeFrom Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.SYUPT_TIME_1 >= ").Append(setSelectParam(param.SyuptTimeFrom, basicEnt.syuptTime1))
        End If
        '出発時間TO
        If Not param.SyuptTimeTo Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.SYUPT_TIME_1 <= ").Append(setSelectParam(param.SyuptTimeTo, basicEnt.syuptTime1))
        End If
        '号車
        If Not param.Gousya Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.GOUSYA >= ").Append(setSelectParam(param.Gousya, basicEnt.gousya))
        End If
        '仕入先コード
        If String.IsNullOrEmpty(param.SiireSakiCd) = False Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE.SIIRE_SAKI_CD = ").Append(setSelectParam(param.SiireSakiCd, siireEnt.SiireSakiCd))
            sb.AppendLine("  AND ")
            sb.AppendLine("  SIIRE.SIIRE_SAKI_NO = ").Append(setSelectParam(param.SiireSakiCd, siireEnt.SiireSakiNo))
        End If
        'グループ名称
        If String.IsNullOrEmpty(param.GroupName) = False Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  OPTG.OPTION_GROUP_NM LIKE = ").Append(setSelectParam("%" & param.GroupName & "%", groupEnt.optionGroupNm))
        End If
        sb.AppendLine("  AND HTBS.GET_CRS_STATUS_FC(BSC.TEIKI_KIKAKU_KBN,BSC.UNKYU_KBN,BSC.SAIKOU_KAKUTEI_KBN) = 1 ")
        sb.AppendLine("  AND BSC.MARU_ZOU_MANAGEMENT_KBN IS NULL ")
        sb.AppendLine("  AND BSC.DELETE_DAY = 0 ")
        sb.AppendLine("  AND BSC.OPTION_FLG = 'Y' ")
        sb.AppendLine("GROUP BY ")
        sb.AppendLine("    BSC.SYUPT_DAY ")
        sb.AppendLine("  , BSC.SYUPT_TIME_1 ")
        sb.AppendLine("  , BSC.CRS_CD ")
        sb.AppendLine("  , BSC.GOUSYA ")
        sb.AppendLine("  , BSC.YOYAKU_NUM_TEISEKI ")
        sb.AppendLine("  , BSC.YOYAKU_NUM_SUB_SEAT ")
        sb.AppendLine("  , OPTG.OPTION_GROUP_NM ")
        sb.AppendLine("  , OPT.OPTIONAL_NAME ")
        sb.AppendLine("  , OPT.HANBAI_TANKA ")
        sb.AppendLine("  , YOPT.GROUP_NO ")
        sb.AppendLine("  , YOPT.LINE_NO ")
        sb.AppendLine("  , SIIRE.SIIRE_SAKI_NAME ")
        sb.AppendLine("ORDER BY ")
        sb.AppendLine("    BSC.SYUPT_DAY ")
        sb.AppendLine("  , BSC.CRS_CD ")
        sb.AppendLine("  , BSC.SYUPT_TIME_1 ")
        sb.AppendLine("  , YOPT.GROUP_NO ")
        sb.AppendLine("  , YOPT.LINE_NO ")

        Return MyBase.getDataTable(sb.ToString)
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

    Public Class S03_0104DASelectParam
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
        ''' 出発時間FROM
        ''' </summary>
        Public Property SyuptTimeFrom As Integer?
        ''' <summary>
        ''' 出発時間TO
        ''' </summary>
        Public Property SyuptTimeTo As Integer?
        ''' <summary>
        ''' 号車
        ''' </summary>
        Public Property Gousya As Integer?
        ''' <summary>
        ''' 仕入先コード
        ''' </summary>
        Public Property SiireSakiCd As String
        ''' <summary>
        ''' グループ名称
        ''' </summary>
        Public Property GroupName As String
    End Class


End Class
