Imports System.Text

Public Class S03_0224DA
    Inherits DataAccessorBase

#Region " 定数／変数 "
    Private ParamNum As Integer = 0
#End Region

    Public Function selectDataTable(ByVal param As S03_0224DASelectParam) As DataTable
        Dim sb As New StringBuilder
        Dim basicEnt As New CrsLedgerBasicEntity
        Dim groupEnt As New CrsLedgerOptionGroupEntity
        Dim siireEnt As New MSiireSakiEntity

        'パラメータクリア
        clear()

        sb.AppendLine(" SELECT ")
        sb.AppendLine("   TO_YYYYMMDD_FC(BSC.SYUPT_DAY) AS SYUPT_DAY_STR, ")
        sb.AppendLine("   BSC.CRS_CD, ")
        sb.AppendLine("   BSC.CRS_NAME, ")
        sb.AppendLine("   BSC.GOUSYA, ")
        sb.AppendLine("   TO_HHMM_FC(BSC.SYUPT_TIME_1) AS SYUPT_TIME1, ")
        sb.AppendLine("   CASE BSC.TEIKI_KIKAKU_KBN ")
        sb.AppendLine("      WHEN '1' THEN ")
        sb.AppendLine("        DECODE(BSC.UNKYU_KBN, 'Y', '運休', '廃止') ")
        sb.AppendLine("      WHEN '2' THEN ")
        sb.AppendLine("        DECODE(BSC.UNKYU_KBN, 'Y', '催行決定', 'N', '催行中止', '廃止') ")
        sb.AppendLine("    END AS UNKYU_SAIKOU, ")
        sb.AppendLine("    BSC.BUS_RESERVE_CD, ")
        sb.AppendLine("    BSC.CAR_TYPE_CD, ")
        sb.AppendLine("    BSC.CAPACITY_REGULAR + BSC.CAPACITY_HO_1KAI AS CAPACITY_NUM, ")
        sb.AppendLine("    BSC.JYOSYA_CAPACITY, ")
        sb.AppendLine("    BSC.KUSEKI_NUM_TEISEKI + BSC.KUSEKI_NUM_SUB_SEAT AS KUSEKI_NUM, ")
        sb.AppendLine("    BSC.YOYAKU_NUM_TEISEKI + BSC.YOYAKU_NUM_SUB_SEAT AS YOYAKU_NUM, ")
        sb.AppendLine("    BSC.EI_BLOCK_REGULAR + BSC.EI_BLOCK_HO AS BLOCK_NUM, ")
        sb.AppendLine("    BSC.KUSEKI_KAKUHO_NUM, ")
        sb.AppendLine("    TO_YYYYMMDD_FC(BSC.UKETUKE_START_DAY) AS UKETUKE_START_DAY, ")
        sb.AppendLine("    DECODE(BSC.ZASEKI_RESERVE_KBN, '1', '縦', '横') AS ZASEKI_RESERVE_KBN, ")
        sb.AppendLine("    BSC.UKETUKE_GENTEI_NINZU, ")
        sb.AppendLine("    BSC.BUS_COUNT_FLG, ")
        sb.AppendLine("    DECODE(BSC.SUB_SEAT_OK_KBN, 'Y', '1', '0') AS SUB_SEAT_OK_KBN, ")
        sb.AppendLine("    DECODE(BSC.YOYAKU_STOP_FLG, 'Y', '1', '0') AS YOYAKU_STOP_FLG, ")
        sb.AppendLine("    DECODE(BSC.JYOSEI_SENYO_SEAT_FLG, 'Y', '1', '0') AS JYOSEI_SENYO_SEAT_FLG, ")
        sb.AppendLine("    DECODE(BSC.MEDIA_CHECK_FLG, 'Y', '1', '0') AS MEDIA_CHECK_FLG, ")
        sb.AppendLine("    BSC.CANCEL_RYOU_KBN, ")
        sb.AppendLine("    DECODE(BSC.ONE_SANKA_FLG, 'Y', '1', '0') AS ONE_SANKA_FLG, ")
        sb.AppendLine("    DECODE(BSC.AIBEYA_USE_FLG, 'Y', '1', '0') AS AIBEYA_USE_FLG, ")
        sb.AppendLine("    DECODE(BSC.TEIINSEI_FLG, 'Y', '1', '0') AS TEIINSEI_FLG, ")
        sb.AppendLine("    BSC.CRS_BLOCK_CAPACITY, ")
        sb.AppendLine("    BSC.CRS_BLOCK_ROOM_NUM, ")
        sb.AppendLine("    BSC.CRS_BLOCK_ONE_1R, ")
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_ONE_1R, 'Y', '1', '0') AS UKETOME_KBN_ONE_1R, ")
        sb.AppendLine("    BSC.CRS_BLOCK_TWO_1R, ")
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_TWO_1R, 'Y', '1', '0') AS UKETOME_KBN_TWO_1R, ")
        sb.AppendLine("    BSC.CRS_BLOCK_THREE_1R, ")
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_THREE_1R, 'Y', '1', '0') AS UKETOME_KBN_THREE_1R, ")
        sb.AppendLine("    BSC.CRS_BLOCK_FOUR_1R, ")
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_FOUR_1R, 'Y', '1', '0') AS UKETOME_KBN_FOUR_1R, ")
        sb.AppendLine("    BSC.CRS_BLOCK_FIVE_1R, ")
        sb.AppendLine("    DECODE(ADINFO.UKETOME_KBN_FIVE_1R, 'Y', '1', '0') AS UKETOME_KBN_FIVE_1R, ")
        sb.AppendLine("    CASE ")
        sb.AppendLine("      WHEN BSC.DELETE_DAY <> 0 THEN '1' ")
        sb.AppendLine("      ELSE '0' ")
        sb.AppendLine("    END DELETE_DAY ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("    T_CRS_LEDGER_BASIC BSC ")
        sb.AppendLine("    INNER JOIN T_CRS_LEDGER_ADD_INFO ADINFO ")
        sb.AppendLine("      ON BSC.CRS_CD = ADINFO.CRS_CD ")
        sb.AppendLine("      AND BSC.SYUPT_DAY = ADINFO.SYUPT_DAY ")
        sb.AppendLine("      AND BSC.GOUSYA = ADINFO.GOUSYA ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("  1 = 1 ")
        '出発日FROM
        sb.AppendLine("  AND ")
        sb.AppendLine("  BSC.SYUPT_DAY >= ").Append(setSelectParam(param.SyuptDayFrom, basicEnt.syuptDay))
        '出発日TO
        sb.AppendLine("  AND ")
        sb.AppendLine("  BSC.SYUPT_DAY <= ").Append(setSelectParam(param.SyuptDayTo, basicEnt.syuptDay))
        'コースコード
        If String.IsNullOrEmpty(param.CrsCd) = False Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.CRS_CD = ").Append(setSelectParam(param.CrsCd, basicEnt.crsCd))
        End If
        '号車
        If Not param.Gousya Is Nothing Then
            sb.AppendLine("  AND ")
            sb.AppendLine("  BSC.GOUSYA = ").Append(setSelectParam(param.Gousya, basicEnt.gousya))
        End If
        sb.AppendLine(" ORDER BY ")
        sb.AppendLine("   BSC.SYUPT_DAY, ")
        sb.AppendLine("   BSC.CRS_CD, ")
        sb.AppendLine("   BSC.SYUPT_TIME_1, ")
        sb.AppendLine("   BSC.GOUSYA ")

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

    Public Class S03_0224DASelectParam
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
    End Class


End Class
