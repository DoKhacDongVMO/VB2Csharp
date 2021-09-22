Imports System.Text

''' <summary>
''' S02_0604Da 不乗・差額証明書
''' </summary>
''' <remarks>
''' </remarks>
Public Class P02_0604Da
    Inherits DataAccessorBase

#Region "定数／変数"
    ''' <summary>
    ''' 金額ゼロ
    ''' </summary>
    Private Const KingakuZero As Integer = 0
    ''' <summary>
    ''' 1行目
    ''' </summary>
    Private Const OneLine As Integer = 1
    ''' <summary>
    ''' 削除日ゼロ
    ''' </summary>
    Private Const DeleteDayZero As Integer = 0

    Private Const Kbnno As Integer = 1
#End Region

#Region "検索"

    ''' <summary>
    ''' 不乗証明テーブル 取得
    ''' 予約区分と予約Noを指定して取得
    ''' </summary>
    ''' <param name="yoyakuKbn"></param>
    ''' <param name="yoyakuNo"></param>
    ''' <returns></returns>
    Public Function getFujyoProofHakkenCount(ByVal yoyakuKbn As String,
                                       ByVal yoyakuNo As Integer) As DataTable

        Dim fujyoProofEntity As New TFujyoProofEntity '不乗証明エンティティ

        ' parameter clear
        MyBase.paramClear()

        Dim prmYoyakuKbn As String = MyBase.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim, fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu)
        Dim prmYoyakuNo As String = MyBase.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu)

        Try
            Dim sb As New StringBuilder()

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     HAKKEN_COUNT ")
            sb.AppendLine(" FROM  ")
            sb.AppendLine("     T_FUJYO_PROOF ")
            sb.AppendLine(" WHERE 1=1 ")
            sb.AppendLine($"     AND YOYAKU_KBN = {prmYoyakuKbn} ")
            sb.AppendLine($"     AND YOYAKU_NO = {prmYoyakuNo} ")
            sb.AppendLine($"     AND ROWNUM = {OneLine} ")
            sb.AppendLine($"     AND NVL(DELETE_DAY, 0) = {DeleteDayZero} ")
            sb.AppendLine(" ORDER BY ")
            sb.AppendLine("     HAKKEN_COUNT DESC ")

            Return getDataTable(sb.ToString())

        Catch
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 不乗証明、コース台帳（基本）テーブル 取得
    ''' 予約区分と予約Noと発券回数を指定して取得
    ''' </summary>
    ''' <param name="yoyakuKbn"></param>
    ''' <param name="yoyakuNo"></param>
    ''' <returns></returns>
    Public Function getFujyoProof(ByVal yoyakuKbn As String,
                                       ByVal yoyakuNo As Integer,
                                       ByVal hakkenCount As Integer) As DataTable

        Dim fujyoProofEntity As New TFujyoProofEntity '不乗証明エンティティ

        ' parameter clear
        MyBase.paramClear()

        Dim prmYoyakuKbn As String = MyBase.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim, fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu)
        Dim prmYoyakuNo As String = MyBase.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu)
        Dim prmHakkenCount As String = MyBase.setParam(fujyoProofEntity.hakkenCount.PhysicsName, hakkenCount, fujyoProofEntity.hakkenCount.DBType, fujyoProofEntity.hakkenCount.IntegerBu)

        Try
            Dim sb As New StringBuilder()

            sb.AppendLine(" SELECT ")
            sb.AppendLine("   ADDRESS ")
            sb.AppendLine("   , T_FUJYO_PROOF.AGENT_CD ")
            sb.AppendLine("   , T_FUJYO_PROOF.AGENT_NM ")
            sb.AppendLine("   , T_FUJYO_PROOF.CRS_CD ")
            sb.AppendLine("   , T_FUJYO_PROOF.ADDRESS ")
            sb.AppendLine("   , T_CRS_LEDGER_BASIC.CRS_NAME ")
            sb.AppendLine("   , T_FUJYO_PROOF.HAKKEN_COUNT ")
            sb.AppendLine("   , T_FUJYO_PROOF.KEN_KBN ")
            sb.AppendLine("   , NVL(T_FUJYO_PROOF.MODOSI_KINGAKU ,0) MODOSI_KINGAKU ")
            sb.AppendLine("   , T_FUJYO_PROOF.PER_YEN_KBN ")
            sb.AppendLine("   , NVL(T_FUJYO_PROOF.MODOSI_FEE ,0) MODOSI_FEE ")
            sb.AppendLine("   , T_FUJYO_PROOF.FEE_PER ")
            sb.AppendLine("   , T_FUJYO_PROOF.RIYUU_WORDING ")
            sb.AppendLine("   , T_FUJYO_PROOF.SENSYA_KEN_TASYA_ISSUE_DAY ")
            sb.AppendLine("   , T_FUJYO_PROOF.SENSYA_KEN_TASYA_KENNO_COACH ")
            sb.AppendLine("   , T_FUJYO_PROOF.SYUPT_DAY ")
            sb.AppendLine("   , T_FUJYO_PROOF.YOYAKU_KBN ")
            sb.AppendLine("   , T_FUJYO_PROOF.YOYAKU_NO  ")
            sb.AppendLine("   , T_FUJYO_PROOF.UPDATE_PERSON_CD  ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("   T_FUJYO_PROOF ")
            sb.AppendLine(" INNER JOIN ")
            sb.AppendLine("   T_CRS_LEDGER_BASIC ")
            sb.AppendLine(" ON ")
            sb.AppendLine("   T_FUJYO_PROOF.CRS_CD = T_CRS_LEDGER_BASIC.CRS_CD ")
            sb.AppendLine("   AND   T_FUJYO_PROOF.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ")
            sb.AppendLine("   AND   T_FUJYO_PROOF.GOUSYA = T_CRS_LEDGER_BASIC.GOUSYA ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine($"   YOYAKU_KBN = {prmYoyakuKbn} ")
            sb.AppendLine($"   and YOYAKU_NO = {prmYoyakuNo} ")
            sb.AppendLine($"   and HAKKEN_COUNT = {hakkenCount} ")

            Return getDataTable(sb.ToString())

        Catch
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 不乗証明、コース台帳（基本）テーブル 取得
    ''' 予約区分と予約Noと発券回数を指定して取得
    ''' </summary>
    ''' <param name="yoyakuKbn"></param>
    ''' <param name="yoyakuNo"></param>
    ''' <returns></returns>
    Public Function getFujyoProofCharge(ByVal yoyakuKbn As String,
                                       ByVal yoyakuNo As Integer,
                                       ByVal hakkenCount As Integer) As DataTable

        Dim fujyoProofChargeEntity As New TFujyoProofChargeEntity '不乗証明エンティティ

        ' parameter clear
        MyBase.paramClear()

        Dim prmKbnno As String = MyBase.setParam(fujyoProofChargeEntity.kbnNo.PhysicsName, Kbnno, fujyoProofChargeEntity.kbnNo.DBType, fujyoProofChargeEntity.kbnNo.IntegerBu)
        Dim prmYoyakuKbn As String = MyBase.setParam(fujyoProofChargeEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim, fujyoProofChargeEntity.yoyakuKbn.DBType, fujyoProofChargeEntity.yoyakuKbn.IntegerBu)
        Dim prmYoyakuNo As String = MyBase.setParam(fujyoProofChargeEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofChargeEntity.yoyakuNo.DBType, fujyoProofChargeEntity.yoyakuNo.IntegerBu)
        Dim prmHakkenCount As String = MyBase.setParam(fujyoProofChargeEntity.hakkenCount.PhysicsName, hakkenCount, fujyoProofChargeEntity.hakkenCount.DBType, fujyoProofChargeEntity.hakkenCount.IntegerBu)

        Try

            Dim sb As New StringBuilder()

            sb.AppendLine(" SELECT ")
            sb.AppendLine("   T_FUJYO_PROOF_CHARGE.CHARGE_KBN_JININ_CD ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JININ_SEQ ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.HAKKEN_COUNT ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_KINGAKU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_NINZU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_TANKA ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_KINGAKU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_NINZU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_TANKA ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_KINGAKU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_NINZU ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_TANKA ")
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.CHARGE_KBN  ")
            sb.AppendLine("   , M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_NAME  ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("   T_FUJYO_PROOF_CHARGE  ")
            sb.AppendLine(" LEFT OUTER JOIN  ")
            sb.AppendLine("   M_CHARGE_JININ_KBN  ")
            sb.AppendLine(" ON ")
            sb.AppendLine("    T_FUJYO_PROOF_CHARGE.CHARGE_KBN_JININ_CD = M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_CD ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine($"   YOYAKU_KBN = {prmYoyakuKbn}  ")
            sb.AppendLine($"   and YOYAKU_NO = {prmYoyakuNo}  ")
            sb.AppendLine($"   and KBN_NO = {prmKbnno}  ")
            sb.AppendLine($"   and HAKKEN_COUNT = {prmHakkenCount}  ")
            sb.AppendLine(" ORDER BY ")
            sb.AppendLine("   YOYAKU_KBN ")
            sb.AppendLine("   , YOYAKU_NO ")
            sb.AppendLine("   , KBN_NO ")
            sb.AppendLine("   , CHARGE_KBN_JININ_CD ")
            sb.AppendLine("   , JININ_SEQ ")

            Return getDataTable(sb.ToString())

        Catch
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 不乗証明テーブル 取得
    ''' 予約区分と予約Noを指定して取得
    ''' </summary>
    ''' <returns></returns>
    Public Function getEigyosyoMaster() As DataTable

        ' parameter clear
        MyBase.paramClear()

        Dim companyCd As String = UserInfoManagement.companyCd
        Dim eigyosyoCd As String = UserInfoManagement.eigyosyoCd

        Try
            Dim sb As New StringBuilder()

            sb.AppendLine(" SELECT ")
            sb.AppendLine("   EIGYOSYO_NAME_1  ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("   M_EIGYOSYO  ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine($"   COMPANY_CD = {companyCd}  ")
            sb.AppendLine($"   AND EIGYOSYO_CD = {eigyosyoCd} ")

            Return getDataTable(sb.ToString())

        Catch
            Throw
        End Try

    End Function

#End Region

#Region "登録／更新／削除"

    ''' <summary>
    ''' 不乗証明、予約情報（基本）テーブルの更新
    ''' </summary>
    ''' <param name="yoyakuInfoBasicEntity"></param>
    ''' <param name="fujyoProofEntity"></param>
    ''' <returns></returns>
    Public Function executeFujyoProof(ByVal yoyakuInfoBasicEntity As YoyakuInfoBasicEntity,
                                      ByVal fujyoProofEntity As TFujyoProofEntity) As Boolean

        Dim oraTran As OracleTransaction = Nothing

        Try
            'トランザクション開始
            oraTran = MyBase.callBeginTransaction()

            Dim updateCount As Integer = MyBase.execNonQuery(oraTran, createSqlUpdateYoyakuInfoBasic(yoyakuInfoBasicEntity).ToString)

            If updateCount < 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oraTran)
            End If

            updateCount = MyBase.execNonQuery(oraTran, createSqlUpdateFujyoProof(fujyoProofEntity).ToString)

            If updateCount < 0 Then

                ' ロールバック
                MyBase.callRollbackTransaction(oraTran)
            Else
                'コミット
                MyBase.callCommitTransaction(oraTran)
            End If

        Catch

            ' ロールバック
            MyBase.callRollbackTransaction(oraTran)

            Throw
        Finally
            ' トランザクションのはき
            oraTran.Dispose()
        End Try

        Return True
    End Function

#Region "メソッド"

    ''' <summary>
    ''' 予約情報（基本）のInsert文
    ''' </summary>
    ''' <param name="yoyakuInfoBasicEntity"></param>
    ''' <returns>不乗証明発行フラグをBLANKに更新</returns>
    Private Function createSqlUpdateYoyakuInfoBasic(ByVal yoyakuInfoBasicEntity As YoyakuInfoBasicEntity) As String

        ' parameter clear
        MyBase.paramClear()

        Dim sb As New StringBuilder()

        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC  ")
        sb.AppendLine(" SET ")
        sb.AppendLine("   UPDATE_DAY = " + MyBase.setParam(yoyakuInfoBasicEntity.updateDay.PhysicsName, yoyakuInfoBasicEntity.updateDay.Value, yoyakuInfoBasicEntity.updateDay.DBType, yoyakuInfoBasicEntity.updateDay.IntegerBu, yoyakuInfoBasicEntity.updateDay.DecimalBu))
        sb.AppendLine("   , UPDATE_PERSON_CD = " + MyBase.setParam(yoyakuInfoBasicEntity.updatePersonCd.PhysicsName, yoyakuInfoBasicEntity.updatePersonCd.Value, yoyakuInfoBasicEntity.updatePersonCd.DBType, yoyakuInfoBasicEntity.updatePersonCd.IntegerBu, yoyakuInfoBasicEntity.updatePersonCd.DecimalBu))
        sb.AppendLine("   , UPDATE_PGMID = " + MyBase.setParam(yoyakuInfoBasicEntity.updatePgmid.PhysicsName, yoyakuInfoBasicEntity.updatePgmid.Value, yoyakuInfoBasicEntity.updatePgmid.DBType, yoyakuInfoBasicEntity.updatePgmid.IntegerBu, yoyakuInfoBasicEntity.updatePgmid.DecimalBu))
        sb.AppendLine("   , UPDATE_TIME = " + MyBase.setParam(yoyakuInfoBasicEntity.updateTime.PhysicsName, yoyakuInfoBasicEntity.updateTime.Value, yoyakuInfoBasicEntity.updateTime.DBType, yoyakuInfoBasicEntity.updateTime.IntegerBu, yoyakuInfoBasicEntity.updateTime.DecimalBu))
        sb.AppendLine("   , FUJYO_PROOF_ISSUE_FLG = " + MyBase.setParam(yoyakuInfoBasicEntity.fujyoProofIssueFlg.PhysicsName, yoyakuInfoBasicEntity.fujyoProofIssueFlg.Value, yoyakuInfoBasicEntity.fujyoProofIssueFlg.DBType, yoyakuInfoBasicEntity.fujyoProofIssueFlg.IntegerBu, yoyakuInfoBasicEntity.fujyoProofIssueFlg.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_PGMID = " + MyBase.setParam(yoyakuInfoBasicEntity.systemUpdatePgmid.PhysicsName, yoyakuInfoBasicEntity.systemUpdatePgmid.Value, yoyakuInfoBasicEntity.systemUpdatePgmid.DBType, yoyakuInfoBasicEntity.systemUpdatePgmid.IntegerBu, yoyakuInfoBasicEntity.systemUpdatePgmid.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(yoyakuInfoBasicEntity.systemUpdatePersonCd.PhysicsName, yoyakuInfoBasicEntity.systemUpdatePersonCd.Value, yoyakuInfoBasicEntity.systemUpdatePersonCd.DBType, yoyakuInfoBasicEntity.systemUpdatePersonCd.IntegerBu, yoyakuInfoBasicEntity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_DAY = " + MyBase.setParam(yoyakuInfoBasicEntity.systemUpdateDay.PhysicsName, yoyakuInfoBasicEntity.systemUpdateDay.Value, yoyakuInfoBasicEntity.systemUpdateDay.DBType, yoyakuInfoBasicEntity.systemUpdateDay.IntegerBu, yoyakuInfoBasicEntity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("   YOYAKU_KBN = " + MyBase.setParam(yoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, yoyakuInfoBasicEntity.yoyakuKbn.Value, yoyakuInfoBasicEntity.yoyakuKbn.DBType, yoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, yoyakuInfoBasicEntity.yoyakuKbn.DecimalBu))
        sb.AppendLine("   AND YOYAKU_NO = " + MyBase.setParam(yoyakuInfoBasicEntity.yoyakuNo.PhysicsName, yoyakuInfoBasicEntity.yoyakuNo.Value, yoyakuInfoBasicEntity.yoyakuNo.DBType, yoyakuInfoBasicEntity.yoyakuNo.IntegerBu, yoyakuInfoBasicEntity.yoyakuNo.DecimalBu))

        Return sb.ToString
    End Function

    ''' <summary>
    ''' 不乗証明のUpdate文
    ''' </summary>
    ''' <param name="fujyoProofEntity"></param>
    ''' <returns>削除日に日付を格納</returns>
    Private Function createSqlUpdateFujyoProof(ByVal fujyoProofEntity As TFujyoProofEntity) As String

        Dim sb As New StringBuilder()

        sb.AppendLine(" UPDATE T_FUJYO_PROOF  ")
        sb.AppendLine(" SET ")
        sb.AppendLine("   UPDATE_DAY = " + MyBase.setParam(fujyoProofEntity.updateDay.PhysicsName, fujyoProofEntity.updateDay.Value, fujyoProofEntity.updateDay.DBType, fujyoProofEntity.updateDay.IntegerBu, fujyoProofEntity.updateDay.DecimalBu))
        sb.AppendLine("   , UPDATE_PERSON_CD = " + MyBase.setParam(fujyoProofEntity.updatePersonCd.PhysicsName, fujyoProofEntity.updatePersonCd.Value, fujyoProofEntity.updatePersonCd.DBType, fujyoProofEntity.updatePersonCd.IntegerBu, fujyoProofEntity.updatePersonCd.DecimalBu))
        sb.AppendLine("   , UPDATE_PGMID = " + MyBase.setParam(fujyoProofEntity.updatePgmid.PhysicsName, fujyoProofEntity.updatePgmid.Value, fujyoProofEntity.updatePgmid.DBType, fujyoProofEntity.updatePgmid.IntegerBu, fujyoProofEntity.updatePgmid.DecimalBu))
        sb.AppendLine("   , UPDATE_TIME = " + MyBase.setParam(fujyoProofEntity.updateTime.PhysicsName, fujyoProofEntity.updateTime.Value, fujyoProofEntity.updateTime.DBType, fujyoProofEntity.updateTime.IntegerBu, fujyoProofEntity.updateTime.DecimalBu))
        sb.AppendLine("   , HAKKEN_DAY = " + MyBase.setParam(fujyoProofEntity.hakkenDay.PhysicsName, fujyoProofEntity.hakkenDay.Value, fujyoProofEntity.hakkenDay.DBType, fujyoProofEntity.hakkenDay.IntegerBu, fujyoProofEntity.hakkenDay.DecimalBu))
        sb.AppendLine("   , HAKKEN_TIME = " + MyBase.setParam(fujyoProofEntity.hakkenTime.PhysicsName, fujyoProofEntity.hakkenTime.Value, fujyoProofEntity.hakkenTime.DBType, fujyoProofEntity.hakkenTime.IntegerBu, fujyoProofEntity.hakkenTime.DecimalBu))
        sb.AppendLine("   , COMPANY_CD = " + MyBase.setParam(fujyoProofEntity.companyCd.PhysicsName, fujyoProofEntity.companyCd.Value, fujyoProofEntity.companyCd.DBType, fujyoProofEntity.companyCd.IntegerBu, fujyoProofEntity.companyCd.DecimalBu))
        sb.AppendLine("   , EIGYOSYO_CD = " + MyBase.setParam(fujyoProofEntity.eigyosyoCd.PhysicsName, fujyoProofEntity.eigyosyoCd.Value, fujyoProofEntity.eigyosyoCd.DBType, fujyoProofEntity.eigyosyoCd.IntegerBu, fujyoProofEntity.eigyosyoCd.DecimalBu))
        sb.AppendLine("   , SIGNON_TIME = " + MyBase.setParam(fujyoProofEntity.signonTime.PhysicsName, fujyoProofEntity.signonTime.Value, fujyoProofEntity.signonTime.DBType, fujyoProofEntity.signonTime.IntegerBu, fujyoProofEntity.signonTime.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_PGMID = " + MyBase.setParam(fujyoProofEntity.systemUpdatePersonCd.PhysicsName, fujyoProofEntity.systemUpdatePersonCd.Value, fujyoProofEntity.systemUpdatePersonCd.DBType, fujyoProofEntity.systemUpdatePersonCd.IntegerBu, fujyoProofEntity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_PERSON_CD = " + MyBase.setParam(fujyoProofEntity.systemUpdatePersonCd.PhysicsName, fujyoProofEntity.systemUpdatePersonCd.Value, fujyoProofEntity.systemUpdatePersonCd.DBType, fujyoProofEntity.systemUpdatePersonCd.IntegerBu, fujyoProofEntity.systemUpdatePersonCd.DecimalBu))
        sb.AppendLine("   , SYSTEM_UPDATE_DAY = " + MyBase.setParam(fujyoProofEntity.systemUpdateDay.PhysicsName, fujyoProofEntity.systemUpdateDay.Value, fujyoProofEntity.systemUpdateDay.DBType, fujyoProofEntity.systemUpdateDay.IntegerBu, fujyoProofEntity.systemUpdateDay.DecimalBu))
        sb.AppendLine(" WHERE ")
        sb.AppendLine("   YOYAKU_KBN = " + MyBase.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, fujyoProofEntity.yoyakuKbn.Value, fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu, fujyoProofEntity.yoyakuKbn.DecimalBu))
        sb.AppendLine("   AND YOYAKU_NO = " + MyBase.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, fujyoProofEntity.yoyakuNo.Value, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu, fujyoProofEntity.yoyakuNo.DecimalBu))
        sb.AppendLine("   AND HAKKEN_COUNT = " + MyBase.setParam(fujyoProofEntity.hakkenCount.PhysicsName, fujyoProofEntity.hakkenCount.Value, fujyoProofEntity.hakkenCount.DBType, fujyoProofEntity.hakkenCount.IntegerBu, fujyoProofEntity.hakkenCount.DecimalBu))

        Return sb.ToString
    End Function

#End Region

#End Region

End Class
