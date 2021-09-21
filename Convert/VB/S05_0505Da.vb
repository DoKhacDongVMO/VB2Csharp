Imports System.Reflection
Imports System.Text

''' <summary>
''' 窓口日報修正Da
''' </summary>
Public Class S05_0505Da
    Inherits DataAccessorBase

#Region "メソッド"

    ''' <summary>
    ''' 予約情報取得
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>予約情報</returns>
    Public Function getYoyakuInfo(entity As SeisanInfoEntity) As DataTable

        Dim yoyakuInfo As DataTable

        Try
            Dim query As String = Me.createYoyakuInfoSql(entity)
            yoyakuInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return yoyakuInfo
    End Function

    ''' <summary>
    ''' 予約情報取得SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>予約情報取得SQL</returns>
    Private Function createYoyakuInfoSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim yoyakuKbn As String = Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn)
        Dim yoyakuNo As String = Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo)
        Dim seq As String = Me.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq)

        Dim sb As New StringBuilder()
        sb.AppendLine($" SELECT ")
        sb.AppendLine($"      YIB.SURNAME || YIB.NAME AS YOYAKU_NANME ")
        sb.AppendLine($"      ,YIB.YOYAKU_KBN ")
        sb.AppendLine($"     ,YIB.YOYAKU_NO ")
        sb.AppendLine($"     ,YIB.TEIKI_KIKAKU_KBN ")
        sb.AppendLine($"     ,YIB.CRS_KIND ")
        sb.AppendLine($"     ,YIB.SYUPT_DAY ")
        sb.AppendLine($"     ,YIB.GOUSYA ")
        sb.AppendLine($"     ,YIB.CRS_CD ")
        sb.AppendLine($"     ,CLB.CRS_NAME ")
        sb.AppendLine($"     ,SIS.ADULT ")
        sb.AppendLine($"     ,SIS.JUNIOR ")
        sb.AppendLine($"     ,SIS.CHILD ")
        sb.AppendLine($"     ,YIB.UPDATE_TIME ")
        sb.AppendLine($"     ,TSI.URIAGE_KBN ")
        sb.AppendLine($"     ,CASE TSI.URIAGE_KBN WHEN 'H' THEN '【払戻】' ")
        sb.AppendLine($"                          WHEN 'V' THEN '【ＶＯＩＤ】' ")
        sb.AppendLine($"      ELSE '【売上】' END AS URIAGE_KBN_NAME ")
        sb.AppendLine($"     ,TSI.WARIBIKI_TYPE ")
        sb.AppendLine($"     ,MWT.WARIBIKI_TYPE_NAME_ABB ")
        sb.AppendLine($"     ,TSI.OTHER_URIAGE_SYOHIN_BIKO ")
        sb.AppendLine($"     ,TSI.TANTOSYA_CD ")
        sb.AppendLine($"     ,MUS.USER_NAME ")
        sb.AppendLine($" FROM ")
        sb.AppendLine($"     T_SEISAN_INFO TSI ")
        sb.AppendLine($" INNER JOIN ")
        sb.AppendLine($"     T_YOYAKU_INFO_BASIC YIB ")
        sb.AppendLine($" ON ")
        sb.AppendLine($"     TSI.YOYAKU_KBN = YIB.YOYAKU_KBN ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     TSI.YOYAKU_NO = YIB.YOYAKU_NO ")
        sb.AppendLine($" LEFT OUTER JOIN ")
        sb.AppendLine($"     T_CRS_LEDGER_BASIC CLB ")
        sb.AppendLine($" ON ")
        sb.AppendLine($"     YIB.CRS_CD = CLB.CRS_CD ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     YIB.SYUPT_DAY = CLB.SYUPT_DAY ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     YIB.GOUSYA = CLB.GOUSYA ")
        sb.AppendLine($" LEFT JOIN ")
        sb.AppendLine($"     (SELECT ")
        sb.AppendLine($"          SIS.SEQ ")
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '10' THEN SIS.SANKA_NINZU ELSE 0 END) AS ADULT ")
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '20' THEN SIS.SANKA_NINZU ELSE 0 END) AS JUNIOR ")
        sb.AppendLine($"         ,SUM(CASE WHEN CJK.SHUYAKU_CHARGE_KBN_CD = '30' THEN SIS.SANKA_NINZU ELSE 0 END) AS CHILD ")
        sb.AppendLine($"     FROM ")
        sb.AppendLine($"         T_SEISAN_INFO_SANKA_NINZU SIS ")
        sb.AppendLine($"     INNER JOIN ")
        sb.AppendLine($"         M_CHARGE_JININ_KBN CJK ")
        sb.AppendLine($"     ON ")
        sb.AppendLine($"         SIS.CHARGE_KBN_JININ_CD = CJK.CHARGE_KBN_JININ_CD ")
        sb.AppendLine($"     WHERE ")
        sb.AppendLine($"         SIS.YOYAKU_KBN = {yoyakuKbn}")
        sb.AppendLine($"         AND ")
        sb.AppendLine($"         SIS.YOYAKU_NO = {yoyakuNo}")
        sb.AppendLine($"         AND ")
        sb.AppendLine($"         SIS.SEQ = {seq}")
        sb.AppendLine($"     GROUP BY ")
        sb.AppendLine($"          SIS.YOYAKU_KBN ")
        sb.AppendLine($"         ,SIS.YOYAKU_NO ")
        sb.AppendLine($"         ,SIS.SEQ) SIS ")
        sb.AppendLine($" ON ")
        sb.AppendLine($"     TSI.SEQ = SIS.SEQ ")
        sb.AppendLine($" LEFT OUTER JOIN ")
        sb.AppendLine($"     M_WARIBIKI_TYPE MWT ")
        sb.AppendLine($" ON ")
        sb.AppendLine($"     TSI.WARIBIKI_TYPE = MWT.WARIBIKI_TYPE ")
        sb.AppendLine($" LEFT OUTER JOIN ")
        sb.AppendLine($"     M_USER MUS ")
        sb.AppendLine($" ON ")
        sb.AppendLine($"     MUS.COMPANY_CD = '0001' ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     TSI.TANTOSYA_CD = MUS.USER_ID ")
        sb.AppendLine($" WHERE ")
        sb.AppendLine($"     TSI.YOYAKU_KBN = {yoyakuKbn} ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     TSI.YOYAKU_NO = {yoyakuNo} ")
        sb.AppendLine($"     AND ")
        sb.AppendLine($"     TSI.SEQ = {seq} ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 割引一覧取得
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>割引一覧</returns>
    Public Function getWaribikiList(entity As SeisanInfoEntity) As DataTable

        Dim waribikiList As DataTable

        Try
            Dim query As String = Me.createWaribikiListSql(entity)
            waribikiList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return waribikiList
    End Function

    ''' <summary>
    ''' 割引一覧取得SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>割引一覧取得SQL</returns>
    Private Function createWaribikiListSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim yoyakuKbn As String = Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn)
        Dim yoyakuNo As String = Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo)
        Dim seq As String = Me.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq)
        Dim syuptDay As String = Me.prepareParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay)

        Dim sb As New StringBuilder()
        'sb.AppendLine($" SELECT ")
        'sb.AppendLine($"      YIW.KBN_NO ")
        'sb.AppendLine($"     ,YIC.CHARGE_KBN ")
        'sb.AppendLine($"     ,MCK.CHARGE_NAME ")
        'sb.AppendLine($"     ,YIW.CHARGE_KBN_JININ_CD ")
        'sb.AppendLine($"     ,MCJ.CHARGE_KBN_JININ_NAME ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_CD ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_RIYUU ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_USE_FLG ")
        'sb.AppendLine($"     ,CASE YIW.WARIBIKI_KBN WHEN '1' THEN '%' ")
        'sb.AppendLine($"                        WHEN '2' THEN '\' ")
        'sb.AppendLine($"      END AS WARIBIKI_KBN  ")
        'sb.AppendLine($"     ,DECODE(YIW.WARIBIKI_KBN, '1', YIW.WARIBIKI_PER, YIW.WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ")
        'sb.AppendLine($"     ,CASE WHEN NVL(YIW.CARRIAGE_WARIBIKI_FLG, '0') = '1' OR NVL(YIW.YOYAKU_WARIBIKI_FLG, '0') = '1' THEN '予約時' ")
        'sb.AppendLine($"      ELSE '当日' END AS KBN ")
        'sb.AppendLine($"     ,(YIW.WARIBIKI_APPLICATION_NINZU_1 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_2 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_3 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_4 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_APPLICATION_NINZU_5) AS WARIBIKI_NINZU ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_1 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_2 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_3 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_4 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU_5 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_APPLICATION_NINZU  ")
        'sb.AppendLine($"     ,(YIW.WARIBIKI_TANKA_1 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_TANKA_2 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_TANKA_3 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_TANKA_4 + ")
        'sb.AppendLine($"      YIW.WARIBIKI_TANKA_5) AS WARIBIKI_TOTAL_GAKU ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_1 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_2 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_3 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_4 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_TANKA_5 ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_BIKO ")
        'sb.AppendLine($"     ,MWC.CHANGE_KANOU_FLG ")
        'sb.AppendLine($"     ,YIW.CARRIAGE_WARIBIKI_FLG ")
        'sb.AppendLine($"     ,YIW.YOYAKU_WARIBIKI_FLG ")
        'sb.AppendLine($"     ,YIW.WARIBIKI_USE_FLG ")
        'sb.AppendLine($" FROM ")
        'sb.AppendLine($"     T_SEISAN_INFO TSI ")
        'sb.AppendLine($" INNER JOIN ")
        'sb.AppendLine($"     T_SEISAN_INFO_SANKA_NINZU SIS ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     TSI.SEISAN_INFO_SEQ = SIS.SEISAN_INFO_SEQ ")
        'sb.AppendLine($" INNER JOIN ")
        'sb.AppendLine($"     T_YOYAKU_INFO_WARIBIKI YIW ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     TSI.YOYAKU_KBN = YIW.YOYAKU_KBN ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     TSI.YOYAKU_NO = YIW.YOYAKU_NO ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     SIS.KBN_NO = YIW.KBN_NO ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     SIS.CHARGE_KBN_JININ_CD = YIW.CHARGE_KBN_JININ_CD ")
        'sb.AppendLine($" INNER JOIN ")
        'sb.AppendLine($"     M_WARIBIKI_CD MWC ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     YIW.WARIBIKI_CD = MWC.WARIBIKI_CD ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     TSI.CRS_KIND = MWC.CRS_KIND ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     {syuptDay} BETWEEN MWC.APPLICATION_DAY_FROM AND MWC.APPLICATION_DAY_TO ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     TSI.WARIBIKI_TYPE = MWC.WARIBIKI_TYPE_KBN ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     NVL(MWC.DELETE_DAY, 0) = 0 ")
        'sb.AppendLine($" LEFT OUTER JOIN ")
        'sb.AppendLine($"     T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YIC ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     YIW.YOYAKU_KBN = YIC.YOYAKU_KBN ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     YIW.YOYAKU_NO = YIC.YOYAKU_NO ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     YIW.KBN_NO = YIC.KBN_NO ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     YIW.CHARGE_KBN_JININ_CD = YIC.CHARGE_KBN_JININ_CD ")
        'sb.AppendLine($" LEFT OUTER JOIN ")
        'sb.AppendLine($"     M_CHARGE_KBN MCK ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     YIC.CHARGE_KBN = MCK.CHARGE_KBN ")
        'sb.AppendLine($" LEFT OUTER JOIN ")
        'sb.AppendLine($"     M_CHARGE_JININ_KBN MCJ ")
        'sb.AppendLine($" ON ")
        'sb.AppendLine($"     YIW.CHARGE_KBN_JININ_CD = MCJ.CHARGE_KBN_JININ_CD ")
        'sb.AppendLine($" WHERE ")
        'sb.AppendLine($"     TSI.YOYAKU_KBN = {yoyakuKbn} ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     TSI.YOYAKU_NO = {yoyakuNo} ")
        'sb.AppendLine($"     AND ")
        'sb.AppendLine($"     TSI.SEQ = {seq} ")
        'sb.AppendLine($" ORDER BY ")
        'sb.AppendLine($"      YIW.WARIBIKI_CD ")
        'sb.AppendLine($"     ,YIW.KBN_NO ")
        'sb.AppendLine($"     ,YIW.CHARGE_KBN_JININ_CD ")
        sb.AppendLine(" SELECT")
        sb.AppendLine($"      YW.KBN_NO ")
        sb.AppendLine($"     ,YCC.CHARGE_KBN ")
        sb.AppendLine($"     ,MC.CHARGE_NAME ")
        sb.AppendLine($"     ,YW.CHARGE_KBN_JININ_CD ")
        sb.AppendLine($"     ,MCJ.CHARGE_KBN_JININ_NAME ")
        sb.AppendLine($"     ,YW.WARIBIKI_CD ")
        sb.AppendLine($"     ,YW.WARIBIKI_RIYUU ")
        sb.AppendLine($"     ,YW.WARIBIKI_USE_FLG ")
        sb.AppendLine($"     ,CASE YW.WARIBIKI_KBN WHEN '1' THEN '%' ")
        sb.AppendLine($"                        WHEN '2' THEN '\' ")
        sb.AppendLine($"      END AS WARIBIKI_KBN  ")
        sb.AppendLine($"     ,DECODE(YW.WARIBIKI_KBN, '1', YW.WARIBIKI_PER, YW.WARIBIKI_KINGAKU) AS WARIBIKI_KINGAKU ")
        sb.AppendLine($"     ,CASE WHEN NVL(YW.CARRIAGE_WARIBIKI_FLG, '0') = '1' OR NVL(YW.YOYAKU_WARIBIKI_FLG, '0') = '1' THEN '予約時' ")
        sb.AppendLine($"      ELSE '当日' END AS KBN ")
        sb.AppendLine($"     ,(NVL(YW.WARIBIKI_APPLICATION_NINZU_1,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_2,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_3,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_4,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_APPLICATION_NINZU_5,0)) AS WARIBIKI_NINZU ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_1,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_2,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_3,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_4,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU_5,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_APPLICATION_NINZU,0)  ")
        sb.AppendLine($"     ,(NVL(YW.WARIBIKI_TANKA_1,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_2,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_3,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_4,0) + ")
        sb.AppendLine($"       NVL(YW.WARIBIKI_TANKA_5,0)) AS WARIBIKI_TOTAL_GAKU ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_1,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_2,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_3,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_4,0) ")
        sb.AppendLine($"     ,NVL(YW.WARIBIKI_TANKA_5,0) ")
        sb.AppendLine($"     ,YW.WARIBIKI_BIKO ")
        sb.AppendLine($"     ,MWC.CHANGE_KANOU_FLG ")
        sb.AppendLine($"     ,YW.CARRIAGE_WARIBIKI_FLG ")
        sb.AppendLine($"     ,YW.YOYAKU_WARIBIKI_FLG ")
        sb.AppendLine($"     ,YW.WARIBIKI_USE_FLG ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_YOYAKU_INFO_WARIBIKI YW ")
        sb.AppendLine("     INNER JOIN T_YOYAKU_INFO_BASIC TYIB ")
        sb.AppendLine("         ON TYIB.YOYAKU_KBN = YW.YOYAKU_KBN ")
        sb.AppendLine("        AND TYIB.YOYAKU_NO = YW.YOYAKU_NO ")
        sb.AppendLine("     INNER JOIN T_CRS_LEDGER_BASIC TCLB ")
        sb.AppendLine("         ON TCLB.CRS_CD = TYIB.CRS_CD ")
        sb.AppendLine("        AND TCLB.SYUPT_DAY = TYIB.SYUPT_DAY ")
        sb.AppendLine("        AND TCLB.GOUSYA = TYIB.GOUSYA ")
        sb.AppendLine("     LEFT OUTER JOIN T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN YCC ")
        sb.AppendLine("         ON YW.YOYAKU_KBN = YCC.YOYAKU_KBN ")
        sb.AppendLine("        AND YW.YOYAKU_NO = YCC.YOYAKU_NO ")
        sb.AppendLine("        AND YW.KBN_NO       = YCC.KBN_NO ")
        sb.AppendLine("        AND YW.CHARGE_KBN_JININ_CD = YCC.CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     LEFT OUTER JOIN M_CHARGE_KBN MC ")
        sb.AppendLine("         ON YCC.CHARGE_KBN = MC.CHARGE_KBN ")
        sb.AppendLine("     LEFT OUTER JOIN M_CHARGE_JININ_KBN MCJ ")
        sb.AppendLine("         ON YW.CHARGE_KBN_JININ_CD = MCJ.CHARGE_KBN_JININ_CD ")
        sb.AppendLine("     LEFT OUTER JOIN M_WARIBIKI_CD MWC ")
        sb.AppendLine("         ON YW.WARIBIKI_CD = MWC.WARIBIKI_CD ")
        sb.AppendLine("        AND MWC.CRS_KIND  = TCLB.CRS_KIND ")
        sb.AppendLine("        AND MWC.HOUJIN_GAIKYAKU_KBN = TCLB.HOUJIN_GAIKYAKU_KBN ")
        sb.AppendLine(" WHERE 1 = 1 ")
        sb.AppendLine($"     AND YW.YOYAKU_KBN = {yoyakuKbn} ")
        sb.AppendLine($"     AND YW.YOYAKU_NO  = {yoyakuNo} ")
        sb.AppendLine($"     AND RTRIM(MC.DELETE_DATE) IS NULL ") 'CHAR型
        sb.AppendLine($"     AND RTRIM(MCJ.DELETE_DAY) IS NULL ") 'CHAR型
        sb.AppendLine($"     AND NVL(MWC.DELETE_DAY, 0) = 0 ") 'NUMBER型

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' クーポン払戻売上取得
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>クーポン払戻売上</returns>
    Public Function getCouponInfo(entity As SeisanInfoEntity) As DataTable

        Dim couponInfo As DataTable

        Try
            Dim query As String = Me.createCouponInfoSql(entity)
            couponInfo = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return couponInfo
    End Function

    ''' <summary>
    ''' クーポン払戻売上取得SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>クーポン払戻売上取得SQL</returns>
    Private Function createCouponInfoSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim yoyakuKbn As String = Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn)
        Dim yoyakuNo As String = Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo)
        Dim seq As String = Me.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq)

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      SEISAN_INFO_SEQ ")
        sb.AppendLine("     ,COUPON_REFUND ")
        sb.AppendLine("     ,COUPON_URIAGE ")
        sb.AppendLine("     ,OTHER_URIAGE_SYOHIN_BIKO ")
        sb.AppendLine("     ,URIAGE_KBN ")
        sb.AppendLine("     ,SEISAN_KBN ")
        sb.AppendLine("     ,KENNO ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     T_SEISAN_INFO ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine($"     YOYAKU_KBN = {yoyakuKbn} ")
        sb.AppendLine("     AND ")
        sb.AppendLine($"     YOYAKU_NO = {yoyakuNo} ")
        sb.AppendLine("     AND ")
        sb.AppendLine($"     SEQ = {seq} ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 精算項目内訳取得
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>精算項目内訳</returns>
    Public Function getSeisanInfoUtiwake(entity As SeisanInfoEntity) As DataTable

        Dim seisanInfoUtiwake As DataTable

        Try
            Dim query As String = Me.createSeisanInfoUtiwakeSql(entity)
            seisanInfoUtiwake = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return seisanInfoUtiwake
    End Function

    ''' <summary>
    ''' 精算項目内訳取得SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>精算項目内訳取得SQL</returns>
    Private Function createSeisanInfoUtiwakeSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim seisanInfoSeq As String = Me.prepareParam(entity.seisanInfoSeq.PhysicsName, entity.seisanInfoSeq.Value, entity.seisanInfoSeq)

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      MSK.SEISAN_KOUMOKU_CD ")
        sb.AppendLine("     ,MSK.SEISAN_KOUMOKU_NAME ")
        sb.AppendLine("     ,SIU.BIKO ")
        sb.AppendLine("     ,SIU.ISSUE_COMPANY_CD ")
        sb.AppendLine("     ,SIU.KINGAKU ")
        sb.AppendLine("     ,SIU.HURIKOMI_KBN ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_SEISAN_KOUMOKU MSK ")
        sb.AppendLine(" LEFT OUTER JOIN ")
        sb.AppendLine("     (SELECT ")
        sb.AppendLine("         * ")
        sb.AppendLine("     FROM ")
        sb.AppendLine("         T_SEISAN_INFO_UTIWAKE SIU ")
        sb.AppendLine("     WHERE ")
        sb.AppendLine($"         SIU.SEISAN_INFO_SEQ = {seisanInfoSeq} ")
        sb.AppendLine("     ) SIU ")
        sb.AppendLine(" ON ")
        sb.AppendLine("     MSK.SEISAN_KOUMOKU_CD = SIU.SEISAN_KOUMOKU_CD ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 補助券発行会社取得
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>補助券発行会社一覧</returns>
    Public Function getSubKenIssueCompanyList(entity As SeisanInfoEntity) As DataTable

        Dim subKenIssueCompanyList As DataTable

        Try
            Dim query As String = Me.createSubKenIssueCompanyListSql(entity)
            subKenIssueCompanyList = MyBase.getDataTable(query)
        Catch ex As Exception

            Throw
        End Try

        Return subKenIssueCompanyList
    End Function

    ''' <summary>
    ''' 補助券発行会社取得SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>補助券発行会社取得SQL</returns>
    Private Function createSubKenIssueCompanyListSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim syuptDay As String = Me.prepareParam(entity.syuptDay.PhysicsName, entity.syuptDay.Value, entity.syuptDay)

        Dim sb As New StringBuilder()
        sb.AppendLine(" SELECT ")
        sb.AppendLine("      ISSUE_COMPANY_CD ")
        sb.AppendLine("     ,ISSUE_COMPANY_NAME ")
        sb.AppendLine(" FROM ")
        sb.AppendLine("     M_SUB_KEN_ISSUE_COMPANY ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     NVL(DELETE_DAY, 0) = 0 ")
        sb.AppendLine("     AND ")
        sb.AppendLine($"     TO_DATE(TO_CHAR({syuptDay}), 'yyyyMMdd') BETWEEN NVL(KEIYAKU_START_DATE, TO_DATE('0001/01/01')) AND NVL(KEIYAKU_END_DATE, TO_DATE('9999/12/31')) ")
        sb.AppendLine(" ORDER BY ")
        sb.AppendLine("     ISSUE_COMPANY_CD ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 精算情報登録
    ''' </summary>
    ''' <param name="registEntity">精算情報登録用Entity</param>
    ''' <returns>登録結果</returns>
    Public Function registSeisanInfo(registEntity As RegistSeisanInfoEntity) As String

        Dim oracleTransaction As OracleTransaction = Nothing
        Dim updateCount As Integer = 0

        Try
            'トランザクション開始
            oracleTransaction = MyBase.callBeginTransaction()

            ' 精算情報更新
            Dim seisanInfoSql As String = Me.createSeisanInfoUpdateSql(registEntity.SeisanInfoEntity)
            updateCount = MyBase.execNonQuery(oracleTransaction, seisanInfoSql)
            If updateCount <= 0 Then

                Return S05_0505.RegistStatusSeisanInfoFailure
            End If

            ' 精算情報内訳削除
            Dim seisanInfoUtiwakeDeleteSql As String = Me.createSeisanInfoUtiwakeDeleteSql(registEntity.SeisanInfoUtiwakeEntity)
            MyBase.execNonQuery(oracleTransaction, seisanInfoUtiwakeDeleteSql)

            ' 精算情報内訳追加
            For Each entity As SeisanInfoUtiwakeEntity In registEntity.SeisanInfoUtiwakeList

                Dim  utiwakeInsertSql As String = Me.createInsertSql(Of SeisanInfoUtiwakeEntity)(entity, "T_SEISAN_INFO_UTIWAKE")
                updateCount = MyBase.execNonQuery(oracleTransaction, utiwakeInsertSql)
                If updateCount <= 0 Then

                    Return S05_0505.RegistStatusSeisanInfoUtiwakeFailure
                End If
            Next

            'コミット
            MyBase.callCommitTransaction(oracleTransaction)

        Catch ex As Exception

            ' ロールバック
            MyBase.callRollbackTransaction(oracleTransaction)
            Throw
        Finally
            ' トランザクションの破棄
            oracleTransaction.Dispose()
        End Try

        Return S05_0505.RegistStatusSucess
    End Function

    ''' <summary>
    ''' 精算情報更新SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報Entity</param>
    ''' <returns>精算情報更新SQL</returns>
    Private Function createSeisanInfoUpdateSql(entity As SeisanInfoEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" UPDATE ")
        sb.AppendLine("     T_SEISAN_INFO ")
        sb.AppendLine(" SET ")
        sb.AppendLine("      NOSIGN_KBN = " + Me.prepareParam(entity.nosignKbn.PhysicsName, entity.nosignKbn.Value, entity.nosignKbn))
        sb.AppendLine("     ,UPDATE_DAY = " + Me.prepareParam(entity.updateDay.PhysicsName, entity.updateDay.Value, entity.updateDay))
        sb.AppendLine("     ,UPDATE_PERSON_CD = " + Me.prepareParam(entity.updatePersonCd.PhysicsName, entity.updatePersonCd.Value, entity.updatePersonCd))
        sb.AppendLine("     ,UPDATE_PGMID = " + Me.prepareParam(entity.updatePgmid.PhysicsName, entity.updatePgmid.Value, entity.updatePgmid))
        sb.AppendLine("     ,UPDATE_TIME = " + Me.prepareParam(entity.updateTime.PhysicsName, entity.updateTime.Value, entity.updateTime))
        sb.AppendLine("     ,SYSTEM_UPDATE_PGMID = " + Me.prepareParam(entity.systemUpdatePgmid.PhysicsName, entity.systemUpdatePgmid.Value, entity.systemUpdatePgmid))
        sb.AppendLine("     ,SYSTEM_UPDATE_PERSON_CD = " + Me.prepareParam(entity.systemUpdatePersonCd.PhysicsName, entity.systemUpdatePersonCd.Value, entity.systemUpdatePersonCd))
        sb.AppendLine("     ,SYSTEM_UPDATE_DAY = " + Me.prepareParam(entity.systemUpdateDay.PhysicsName, entity.systemUpdateDay.Value, entity.systemUpdateDay))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("     YOYAKU_KBN = " + Me.prepareParam(entity.yoyakuKbn.PhysicsName, entity.yoyakuKbn.Value, entity.yoyakuKbn))
        sb.AppendLine("     AND ")
        sb.AppendLine("     YOYAKU_NO = " + Me.prepareParam(entity.yoyakuNo.PhysicsName, entity.yoyakuNo.Value, entity.yoyakuNo))
        sb.AppendLine("     AND ")
        sb.AppendLine("     SEQ = " + Me.prepareParam(entity.seq.PhysicsName, entity.seq.Value, entity.seq))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' 精算情報内訳削除SQL作成
    ''' </summary>
    ''' <param name="entity">精算情報内訳Entity</param>
    ''' <returns>精算情報内訳削除SQL</returns>
    Private Function createSeisanInfoUtiwakeDeleteSql(entity As SeisanInfoUtiwakeEntity) As String

        MyBase.paramClear()

        Dim sb As New StringBuilder()
        sb.AppendLine(" DELETE FROM T_SEISAN_INFO_UTIWAKE ")
        sb.AppendLine(" WHERE SEISAN_INFO_SEQ = " + Me.prepareParam(entity.seisanInfoSeq.PhysicsName, entity.seisanInfoSeq.Value, entity.seisanInfoSeq))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' INSERT SQL作成
    ''' </summary>
    ''' <param name="entity">登録するテーブルEntity</param>
    ''' <param name="tableName">テーブル名</param>
    ''' <returns>INSERT SQL</returns>
    Private Function createInsertSql(Of T)(entity As T, tableName As String) As String

        MyBase.paramClear()

        Dim type As Type = GetType(T)
        Dim properties() As PropertyInfo = type.GetProperties()

        Dim insertSql As New StringBuilder()
        Dim valueSql As New StringBuilder()

        Dim idx As Integer = 0
        Dim comma As String = ""
        Dim physicsName As String = ""

        For Each prop As PropertyInfo In properties

            If idx = 0 Then

                comma = ""
            End If

            If prop.PropertyType Is GetType(EntityKoumoku_YmdType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).PhysicsName

                Dim value As Date? = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).Value

                If value IsNot Nothing Then

                    insertSql.AppendLine(comma + physicsName)
                    valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).Value,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_YmdType).DBType))
                End If

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_NumberType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).PhysicsName
                Dim value As Integer? = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value

                If value IsNot Nothing Then

                    insertSql.AppendLine(comma + physicsName)
                    valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).Value,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DBType,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).IntegerBu,
                                                                DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_NumberType).DecimalBu))
                End If

            ElseIf prop.PropertyType Is GetType(EntityKoumoku_Number_DecimalType) Then

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).PhysicsName

                insertSql.AppendLine(comma + physicsName)
                valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).Value,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DBType,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).IntegerBu,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_Number_DecimalType).DecimalBu))

            Else

                physicsName = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).PhysicsName
                Dim value As String = DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value

                insertSql.AppendLine(comma + physicsName)
                valueSql.AppendLine(comma + MyBase.setParam(physicsName,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).Value,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).DBType,
                                                            DirectCast(prop.GetValue(entity, Nothing), EntityKoumoku_MojiType).IntegerBu))
            End If

            comma = ","
            idx = idx + 1
        Next

        ' INSERT文作成
        Dim sb As New StringBuilder()
        sb.AppendLine(String.Format(" INSERT INTO {0} ", tableName))
        sb.AppendLine(" ( ")
        sb.AppendLine(insertSql.ToString())
        sb.AppendLine(") VALUES ( ")
        sb.AppendLine(valueSql.ToString())
        sb.AppendLine(" ) ")

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' パラメータの用意
    ''' </summary>
    ''' <param name="name">パラメータ名（重複不可）</param>
    ''' <param name="value">パラメータ</param>
    ''' <param name="entKoumoku">エンティティの項目</param>
    Private Function prepareParam(ByVal name As String,
                                  ByVal value As Object,
                                  ByVal entKoumoku As IEntityKoumokuType) As String

        Return MyBase.setParam(name,
                               value,
                               entKoumoku.DBType,
                               entKoumoku.IntegerBu,
                               entKoumoku.DecimalBu)
    End Function

#End Region

End Class
