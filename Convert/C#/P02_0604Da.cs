using System.Text;

/// <summary>
/// S02_0604Da 不乗・差額証明書
/// </summary>
/// <remarks>
/// </remarks>
public partial class P02_0604Da : DataAccessorBase
{

    #region 定数／変数
    /// <summary>
    /// 金額ゼロ
    /// </summary>
    private const int KingakuZero = 0;
    /// <summary>
    /// 1行目
    /// </summary>
    private const int OneLine = 1;
    /// <summary>
    /// 削除日ゼロ
    /// </summary>
    private const int DeleteDayZero = 0;
    private const int Kbnno = 1;
    #endregion

    #region 検索

    /// <summary>
    /// 不乗証明テーブル 取得
    /// 予約区分と予約Noを指定して取得
    /// </summary>
    /// <param name="yoyakuKbn"></param>
    /// <param name="yoyakuNo"></param>
    /// <returns></returns>
    public DataTable getFujyoProofHakkenCount(string yoyakuKbn, int yoyakuNo)
    {
        var fujyoProofEntity = new TFujyoProofEntity(); // 不乗証明エンティティ

        // parameter clear
        base.paramClear();
        string prmYoyakuKbn = base.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim(), fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu);
        string prmYoyakuNo = base.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu);
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     HAKKEN_COUNT ");
            sb.AppendLine(" FROM  ");
            sb.AppendLine("     T_FUJYO_PROOF ");
            sb.AppendLine(" WHERE 1=1 ");
            sb.AppendLine($"     AND YOYAKU_KBN = {prmYoyakuKbn} ");
            sb.AppendLine($"     AND YOYAKU_NO = {prmYoyakuNo} ");
            sb.AppendLine($"     AND ROWNUM = {OneLine} ");
            sb.AppendLine($"     AND NVL(DELETE_DAY, 0) = {DeleteDayZero} ");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("     HAKKEN_COUNT DESC ");
            return getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// 不乗証明、コース台帳（基本）テーブル 取得
    /// 予約区分と予約Noと発券回数を指定して取得
    /// </summary>
    /// <param name="yoyakuKbn"></param>
    /// <param name="yoyakuNo"></param>
    /// <returns></returns>
    public DataTable getFujyoProof(string yoyakuKbn, int yoyakuNo, int hakkenCount)
    {
        var fujyoProofEntity = new TFujyoProofEntity(); // 不乗証明エンティティ

        // parameter clear
        base.paramClear();
        string prmYoyakuKbn = base.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim(), fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu);
        string prmYoyakuNo = base.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu);
        string prmHakkenCount = base.setParam(fujyoProofEntity.hakkenCount.PhysicsName, hakkenCount, fujyoProofEntity.hakkenCount.DBType, fujyoProofEntity.hakkenCount.IntegerBu);
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("   ADDRESS ");
            sb.AppendLine("   , T_FUJYO_PROOF.AGENT_CD ");
            sb.AppendLine("   , T_FUJYO_PROOF.AGENT_NM ");
            sb.AppendLine("   , T_FUJYO_PROOF.CRS_CD ");
            sb.AppendLine("   , T_FUJYO_PROOF.ADDRESS ");
            sb.AppendLine("   , T_CRS_LEDGER_BASIC.CRS_NAME ");
            sb.AppendLine("   , T_FUJYO_PROOF.HAKKEN_COUNT ");
            sb.AppendLine("   , T_FUJYO_PROOF.KEN_KBN ");
            sb.AppendLine("   , NVL(T_FUJYO_PROOF.MODOSI_KINGAKU ,0) MODOSI_KINGAKU ");
            sb.AppendLine("   , T_FUJYO_PROOF.PER_YEN_KBN ");
            sb.AppendLine("   , NVL(T_FUJYO_PROOF.MODOSI_FEE ,0) MODOSI_FEE ");
            sb.AppendLine("   , T_FUJYO_PROOF.FEE_PER ");
            sb.AppendLine("   , T_FUJYO_PROOF.RIYUU_WORDING ");
            sb.AppendLine("   , T_FUJYO_PROOF.SENSYA_KEN_TASYA_ISSUE_DAY ");
            sb.AppendLine("   , T_FUJYO_PROOF.SENSYA_KEN_TASYA_KENNO_COACH ");
            sb.AppendLine("   , T_FUJYO_PROOF.SYUPT_DAY ");
            sb.AppendLine("   , T_FUJYO_PROOF.YOYAKU_KBN ");
            sb.AppendLine("   , T_FUJYO_PROOF.YOYAKU_NO  ");
            sb.AppendLine("   , T_FUJYO_PROOF.UPDATE_PERSON_CD  ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("   T_FUJYO_PROOF ");
            sb.AppendLine(" INNER JOIN ");
            sb.AppendLine("   T_CRS_LEDGER_BASIC ");
            sb.AppendLine(" ON ");
            sb.AppendLine("   T_FUJYO_PROOF.CRS_CD = T_CRS_LEDGER_BASIC.CRS_CD ");
            sb.AppendLine("   AND   T_FUJYO_PROOF.SYUPT_DAY = T_CRS_LEDGER_BASIC.SYUPT_DAY ");
            sb.AppendLine("   AND   T_FUJYO_PROOF.GOUSYA = T_CRS_LEDGER_BASIC.GOUSYA ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"   YOYAKU_KBN = {prmYoyakuKbn} ");
            sb.AppendLine($"   and YOYAKU_NO = {prmYoyakuNo} ");
            sb.AppendLine($"   and HAKKEN_COUNT = {hakkenCount} ");
            return getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// 不乗証明、コース台帳（基本）テーブル 取得
    /// 予約区分と予約Noと発券回数を指定して取得
    /// </summary>
    /// <param name="yoyakuKbn"></param>
    /// <param name="yoyakuNo"></param>
    /// <returns></returns>
    public DataTable getFujyoProofCharge(string yoyakuKbn, int yoyakuNo, int hakkenCount)
    {
        var fujyoProofChargeEntity = new TFujyoProofChargeEntity(); // 不乗証明エンティティ

        // parameter clear
        base.paramClear();
        string prmKbnno = base.setParam(fujyoProofChargeEntity.kbnNo.PhysicsName, Kbnno, fujyoProofChargeEntity.kbnNo.DBType, fujyoProofChargeEntity.kbnNo.IntegerBu);
        string prmYoyakuKbn = base.setParam(fujyoProofChargeEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim(), fujyoProofChargeEntity.yoyakuKbn.DBType, fujyoProofChargeEntity.yoyakuKbn.IntegerBu);
        string prmYoyakuNo = base.setParam(fujyoProofChargeEntity.yoyakuNo.PhysicsName, yoyakuNo, fujyoProofChargeEntity.yoyakuNo.DBType, fujyoProofChargeEntity.yoyakuNo.IntegerBu);
        string prmHakkenCount = base.setParam(fujyoProofChargeEntity.hakkenCount.PhysicsName, hakkenCount, fujyoProofChargeEntity.hakkenCount.DBType, fujyoProofChargeEntity.hakkenCount.IntegerBu);
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("   T_FUJYO_PROOF_CHARGE.CHARGE_KBN_JININ_CD ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JININ_SEQ ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.HAKKEN_COUNT ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_KINGAKU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_NINZU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.FUJYO_TANKA ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_KINGAKU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_NINZU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.JYOSYA_TANKA ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_KINGAKU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_NINZU ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.KENMEN_TANKA ");
            sb.AppendLine("   , T_FUJYO_PROOF_CHARGE.CHARGE_KBN  ");
            sb.AppendLine("   , M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_NAME  ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("   T_FUJYO_PROOF_CHARGE  ");
            sb.AppendLine(" LEFT OUTER JOIN  ");
            sb.AppendLine("   M_CHARGE_JININ_KBN  ");
            sb.AppendLine(" ON ");
            sb.AppendLine("    T_FUJYO_PROOF_CHARGE.CHARGE_KBN_JININ_CD = M_CHARGE_JININ_KBN.CHARGE_KBN_JININ_CD ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"   YOYAKU_KBN = {prmYoyakuKbn}  ");
            sb.AppendLine($"   and YOYAKU_NO = {prmYoyakuNo}  ");
            sb.AppendLine($"   and KBN_NO = {prmKbnno}  ");
            sb.AppendLine($"   and HAKKEN_COUNT = {prmHakkenCount}  ");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("   YOYAKU_KBN ");
            sb.AppendLine("   , YOYAKU_NO ");
            sb.AppendLine("   , KBN_NO ");
            sb.AppendLine("   , CHARGE_KBN_JININ_CD ");
            sb.AppendLine("   , JININ_SEQ ");
            return getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// 不乗証明テーブル 取得
    /// 予約区分と予約Noを指定して取得
    /// </summary>
    /// <returns></returns>
    public DataTable getEigyosyoMaster()
    {

        // parameter clear
        base.paramClear();
        string companyCd = UserInfoManagement.companyCd;
        string eigyosyoCd = UserInfoManagement.eigyosyoCd;
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine("   EIGYOSYO_NAME_1  ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("   M_EIGYOSYO  ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"   COMPANY_CD = {companyCd}  ");
            sb.AppendLine($"   AND EIGYOSYO_CD = {eigyosyoCd} ");
            return getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }

    #endregion

    #region 登録／更新／削除

    /// <summary>
    /// 不乗証明、予約情報（基本）テーブルの更新
    /// </summary>
    /// <param name="yoyakuInfoBasicEntity"></param>
    /// <param name="fujyoProofEntity"></param>
    /// <returns></returns>
    public bool executeFujyoProof(YoyakuInfoBasicEntity yoyakuInfoBasicEntity, TFujyoProofEntity fujyoProofEntity)
    {
        OracleTransaction oraTran = default;
        try
        {
            // トランザクション開始
            oraTran = base.callBeginTransaction();
            int updateCount = base.execNonQuery(oraTran, createSqlUpdateYoyakuInfoBasic(yoyakuInfoBasicEntity).ToString);
            if (updateCount < 0)
            {

                // ロールバック
                base.callRollbackTransaction(oraTran);
            }

            updateCount = base.execNonQuery(oraTran, createSqlUpdateFujyoProof(fujyoProofEntity).ToString);
            if (updateCount < 0)
            {

                // ロールバック
                base.callRollbackTransaction(oraTran);
            }
            else
            {
                // コミット
                base.callCommitTransaction(oraTran);
            }
        }
        catch
        {

            // ロールバック
            base.callRollbackTransaction(oraTran);
            throw;
        }
        finally
        {
            // トランザクションのはき
            oraTran.Dispose();
        }

        return true;
    }

    #region メソッド

    /// <summary>
    /// 予約情報（基本）のInsert文
    /// </summary>
    /// <param name="yoyakuInfoBasicEntity"></param>
    /// <returns>不乗証明発行フラグをBLANKに更新</returns>
    private string createSqlUpdateYoyakuInfoBasic(YoyakuInfoBasicEntity yoyakuInfoBasicEntity)
    {

        // parameter clear
        base.paramClear();
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC  ");
        sb.AppendLine(" SET ");
        sb.AppendLine("   UPDATE_DAY = " + base.setParam(yoyakuInfoBasicEntity.updateDay.PhysicsName, yoyakuInfoBasicEntity.updateDay.Value, yoyakuInfoBasicEntity.updateDay.DBType, yoyakuInfoBasicEntity.updateDay.IntegerBu, yoyakuInfoBasicEntity.updateDay.DecimalBu));
        sb.AppendLine("   , UPDATE_PERSON_CD = " + base.setParam(yoyakuInfoBasicEntity.updatePersonCd.PhysicsName, yoyakuInfoBasicEntity.updatePersonCd.Value, yoyakuInfoBasicEntity.updatePersonCd.DBType, yoyakuInfoBasicEntity.updatePersonCd.IntegerBu, yoyakuInfoBasicEntity.updatePersonCd.DecimalBu));
        sb.AppendLine("   , UPDATE_PGMID = " + base.setParam(yoyakuInfoBasicEntity.updatePgmid.PhysicsName, yoyakuInfoBasicEntity.updatePgmid.Value, yoyakuInfoBasicEntity.updatePgmid.DBType, yoyakuInfoBasicEntity.updatePgmid.IntegerBu, yoyakuInfoBasicEntity.updatePgmid.DecimalBu));
        sb.AppendLine("   , UPDATE_TIME = " + base.setParam(yoyakuInfoBasicEntity.updateTime.PhysicsName, yoyakuInfoBasicEntity.updateTime.Value, yoyakuInfoBasicEntity.updateTime.DBType, yoyakuInfoBasicEntity.updateTime.IntegerBu, yoyakuInfoBasicEntity.updateTime.DecimalBu));
        sb.AppendLine("   , FUJYO_PROOF_ISSUE_FLG = " + base.setParam(yoyakuInfoBasicEntity.fujyoProofIssueFlg.PhysicsName, yoyakuInfoBasicEntity.fujyoProofIssueFlg.Value, yoyakuInfoBasicEntity.fujyoProofIssueFlg.DBType, yoyakuInfoBasicEntity.fujyoProofIssueFlg.IntegerBu, yoyakuInfoBasicEntity.fujyoProofIssueFlg.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_PGMID = " + base.setParam(yoyakuInfoBasicEntity.systemUpdatePgmid.PhysicsName, yoyakuInfoBasicEntity.systemUpdatePgmid.Value, yoyakuInfoBasicEntity.systemUpdatePgmid.DBType, yoyakuInfoBasicEntity.systemUpdatePgmid.IntegerBu, yoyakuInfoBasicEntity.systemUpdatePgmid.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_PERSON_CD = " + base.setParam(yoyakuInfoBasicEntity.systemUpdatePersonCd.PhysicsName, yoyakuInfoBasicEntity.systemUpdatePersonCd.Value, yoyakuInfoBasicEntity.systemUpdatePersonCd.DBType, yoyakuInfoBasicEntity.systemUpdatePersonCd.IntegerBu, yoyakuInfoBasicEntity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_DAY = " + base.setParam(yoyakuInfoBasicEntity.systemUpdateDay.PhysicsName, yoyakuInfoBasicEntity.systemUpdateDay.Value, yoyakuInfoBasicEntity.systemUpdateDay.DBType, yoyakuInfoBasicEntity.systemUpdateDay.IntegerBu, yoyakuInfoBasicEntity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("   YOYAKU_KBN = " + base.setParam(yoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, yoyakuInfoBasicEntity.yoyakuKbn.Value, yoyakuInfoBasicEntity.yoyakuKbn.DBType, yoyakuInfoBasicEntity.yoyakuKbn.IntegerBu, yoyakuInfoBasicEntity.yoyakuKbn.DecimalBu));
        sb.AppendLine("   AND YOYAKU_NO = " + base.setParam(yoyakuInfoBasicEntity.yoyakuNo.PhysicsName, yoyakuInfoBasicEntity.yoyakuNo.Value, yoyakuInfoBasicEntity.yoyakuNo.DBType, yoyakuInfoBasicEntity.yoyakuNo.IntegerBu, yoyakuInfoBasicEntity.yoyakuNo.DecimalBu));
        return sb.ToString();
    }

    /// <summary>
    /// 不乗証明のUpdate文
    /// </summary>
    /// <param name="fujyoProofEntity"></param>
    /// <returns>削除日に日付を格納</returns>
    private string createSqlUpdateFujyoProof(TFujyoProofEntity fujyoProofEntity)
    {
        var sb = new StringBuilder();
        sb.AppendLine(" UPDATE T_FUJYO_PROOF  ");
        sb.AppendLine(" SET ");
        sb.AppendLine("   UPDATE_DAY = " + base.setParam(fujyoProofEntity.updateDay.PhysicsName, fujyoProofEntity.updateDay.Value, fujyoProofEntity.updateDay.DBType, fujyoProofEntity.updateDay.IntegerBu, fujyoProofEntity.updateDay.DecimalBu));
        sb.AppendLine("   , UPDATE_PERSON_CD = " + base.setParam(fujyoProofEntity.updatePersonCd.PhysicsName, fujyoProofEntity.updatePersonCd.Value, fujyoProofEntity.updatePersonCd.DBType, fujyoProofEntity.updatePersonCd.IntegerBu, fujyoProofEntity.updatePersonCd.DecimalBu));
        sb.AppendLine("   , UPDATE_PGMID = " + base.setParam(fujyoProofEntity.updatePgmid.PhysicsName, fujyoProofEntity.updatePgmid.Value, fujyoProofEntity.updatePgmid.DBType, fujyoProofEntity.updatePgmid.IntegerBu, fujyoProofEntity.updatePgmid.DecimalBu));
        sb.AppendLine("   , UPDATE_TIME = " + base.setParam(fujyoProofEntity.updateTime.PhysicsName, fujyoProofEntity.updateTime.Value, fujyoProofEntity.updateTime.DBType, fujyoProofEntity.updateTime.IntegerBu, fujyoProofEntity.updateTime.DecimalBu));
        sb.AppendLine("   , HAKKEN_DAY = " + base.setParam(fujyoProofEntity.hakkenDay.PhysicsName, fujyoProofEntity.hakkenDay.Value, fujyoProofEntity.hakkenDay.DBType, fujyoProofEntity.hakkenDay.IntegerBu, fujyoProofEntity.hakkenDay.DecimalBu));
        sb.AppendLine("   , HAKKEN_TIME = " + base.setParam(fujyoProofEntity.hakkenTime.PhysicsName, fujyoProofEntity.hakkenTime.Value, fujyoProofEntity.hakkenTime.DBType, fujyoProofEntity.hakkenTime.IntegerBu, fujyoProofEntity.hakkenTime.DecimalBu));
        sb.AppendLine("   , COMPANY_CD = " + base.setParam(fujyoProofEntity.companyCd.PhysicsName, fujyoProofEntity.companyCd.Value, fujyoProofEntity.companyCd.DBType, fujyoProofEntity.companyCd.IntegerBu, fujyoProofEntity.companyCd.DecimalBu));
        sb.AppendLine("   , EIGYOSYO_CD = " + base.setParam(fujyoProofEntity.eigyosyoCd.PhysicsName, fujyoProofEntity.eigyosyoCd.Value, fujyoProofEntity.eigyosyoCd.DBType, fujyoProofEntity.eigyosyoCd.IntegerBu, fujyoProofEntity.eigyosyoCd.DecimalBu));
        sb.AppendLine("   , SIGNON_TIME = " + base.setParam(fujyoProofEntity.signonTime.PhysicsName, fujyoProofEntity.signonTime.Value, fujyoProofEntity.signonTime.DBType, fujyoProofEntity.signonTime.IntegerBu, fujyoProofEntity.signonTime.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_PGMID = " + base.setParam(fujyoProofEntity.systemUpdatePersonCd.PhysicsName, fujyoProofEntity.systemUpdatePersonCd.Value, fujyoProofEntity.systemUpdatePersonCd.DBType, fujyoProofEntity.systemUpdatePersonCd.IntegerBu, fujyoProofEntity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_PERSON_CD = " + base.setParam(fujyoProofEntity.systemUpdatePersonCd.PhysicsName, fujyoProofEntity.systemUpdatePersonCd.Value, fujyoProofEntity.systemUpdatePersonCd.DBType, fujyoProofEntity.systemUpdatePersonCd.IntegerBu, fujyoProofEntity.systemUpdatePersonCd.DecimalBu));
        sb.AppendLine("   , SYSTEM_UPDATE_DAY = " + base.setParam(fujyoProofEntity.systemUpdateDay.PhysicsName, fujyoProofEntity.systemUpdateDay.Value, fujyoProofEntity.systemUpdateDay.DBType, fujyoProofEntity.systemUpdateDay.IntegerBu, fujyoProofEntity.systemUpdateDay.DecimalBu));
        sb.AppendLine(" WHERE ");
        sb.AppendLine("   YOYAKU_KBN = " + base.setParam(fujyoProofEntity.yoyakuKbn.PhysicsName, fujyoProofEntity.yoyakuKbn.Value, fujyoProofEntity.yoyakuKbn.DBType, fujyoProofEntity.yoyakuKbn.IntegerBu, fujyoProofEntity.yoyakuKbn.DecimalBu));
        sb.AppendLine("   AND YOYAKU_NO = " + base.setParam(fujyoProofEntity.yoyakuNo.PhysicsName, fujyoProofEntity.yoyakuNo.Value, fujyoProofEntity.yoyakuNo.DBType, fujyoProofEntity.yoyakuNo.IntegerBu, fujyoProofEntity.yoyakuNo.DecimalBu));
        sb.AppendLine("   AND HAKKEN_COUNT = " + base.setParam(fujyoProofEntity.hakkenCount.PhysicsName, fujyoProofEntity.hakkenCount.Value, fujyoProofEntity.hakkenCount.DBType, fujyoProofEntity.hakkenCount.IntegerBu, fujyoProofEntity.hakkenCount.DecimalBu));
        return sb.ToString();
    }

    #endregion

    #endregion

}