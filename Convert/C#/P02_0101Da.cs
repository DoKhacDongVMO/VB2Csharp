using System.Text;

/// <summary>
/// キャンセル発生通知用Da
/// </summary>
/// <remarks>
/// </remarks>
public partial class P02_0101Da : DataAccessorBase
{

    #region 定数/変数

    private const string CompanyCd = "0001";    // 会社コード
    private const string ManagementKbn = "W";   // 管理区分

    #endregion

    #region 検索

    /// <summary>
    /// コース台帳（基本）テーブル取得
    /// コースコードと出発日と号車を指定して取得
    /// </summary>
    /// <param name="yoyakuKbn"></param>
    /// <param name="yoyakuNo"></param>
    /// <returns></returns>
    public DataTable getCrsLedgerBasic(string yoyakuKbn, int yoyakuNo)
    {
        var sbSQL = new StringBuilder();
        var yoyakuEntity = new YoyakuInfoBasicEntity(); // 予約情報（基本）エンティティ

        // parameter clear
        base.paramClear();
        string prmYOYAKUKBN = base.setParam(yoyakuEntity.yoyakuKbn.PhysicsName, yoyakuKbn.Trim(), yoyakuEntity.yoyakuKbn.DBType, yoyakuEntity.yoyakuKbn.IntegerBu);
        string prmYOYAKUNO = base.setParam(yoyakuEntity.yoyakuNo.PhysicsName, yoyakuNo, yoyakuEntity.yoyakuNo.DBType, yoyakuEntity.yoyakuNo.IntegerBu);
        try
        {
            sbSQL.AppendLine($" SELECT  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.CRS_KIND  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.SYUPT_DAY  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.RETURN_DAY  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.CRS_CD   ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.CRS_NAME   ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.GOUSYA GOUSYA  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.KUSEKI_NUM_TEISEKI ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.KUSEKI_NUM_SUB_SEAT  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.WT_KAKUHO_SEAT_NUM WT_KAKUHO_SEAT  ");
            sbSQL.AppendLine($"     ,T_CRS_LEDGER_BASIC.ROOM_ZANSU_SOKEI || ' （1 R : ' || ");
            sbSQL.AppendLine($" CASE  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.CRS_BLOCK_ONE_1R <> 0 THEN  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.ROOM_ZANSU_ONE_ROOM  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.ROOM_ZANSU_ONE_ROOM IS NULL THEN  ");
            sbSQL.AppendLine($"     0  ");
            sbSQL.AppendLine($" END  ");
            sbSQL.AppendLine($" || '、2 R : ' ||  ");
            sbSQL.AppendLine($" CASE  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.CRS_BLOCK_TWO_1R <> 0 THEN  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.ROOM_ZANSU_TWO_ROOM  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.ROOM_ZANSU_TWO_ROOM IS NULL THEN  ");
            sbSQL.AppendLine($"     0  ");
            sbSQL.AppendLine($" END  ");
            sbSQL.AppendLine($"     || '、3 R : ' ||  ");
            sbSQL.AppendLine($" CASE  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.CRS_BLOCK_THREE_1R <> 0 THEN  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.ROOM_ZANSU_THREE_ROOM  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.ROOM_ZANSU_THREE_ROOM IS NULL THEN  ");
            sbSQL.AppendLine($"     0  ");
            sbSQL.AppendLine($" END  ");
            sbSQL.AppendLine($"     || '、4 R : ' ||  ");
            sbSQL.AppendLine($" CASE  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.CRS_BLOCK_FOUR_1R <> 0 THEN  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.ROOM_ZANSU_FOUR_ROOM  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.ROOM_ZANSU_FOUR_ROOM IS NULL THEN  ");
            sbSQL.AppendLine($"     0  ");
            sbSQL.AppendLine($" END  ");
            sbSQL.AppendLine($"     || '、5 R : ' ||  ");
            sbSQL.AppendLine($" CASE  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.CRS_BLOCK_FIVE_1R <> 0 THEN  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.ROOM_ZANSU_FIVE_ROOM  ");
            sbSQL.AppendLine($" WHEN T_CRS_LEDGER_BASIC.ROOM_ZANSU_FIVE_ROOM IS NULL THEN  ");
            sbSQL.AppendLine($"     0  ");
            sbSQL.AppendLine($" END  ");
            sbSQL.AppendLine($"  || '）' ROOM_ZANSU");
            sbSQL.AppendLine($"     ,T_YOYAKU_INFO_BASIC.TEIKI_KIKAKU_KBN  ");
            sbSQL.AppendLine($" FROM  ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC ");
            sbSQL.AppendLine($" INNER JOIN ");
            sbSQL.AppendLine($"     T_YOYAKU_INFO_BASIC ");
            sbSQL.AppendLine($" ON ");
            sbSQL.AppendLine($"     T_CRS_LEDGER_BASIC.CRS_CD = T_YOYAKU_INFO_BASIC.CRS_CD ");
            sbSQL.AppendLine($"     AND T_CRS_LEDGER_BASIC.SYUPT_DAY = T_YOYAKU_INFO_BASIC.SYUPT_DAY ");
            sbSQL.AppendLine($"     AND T_CRS_LEDGER_BASIC.GOUSYA = T_YOYAKU_INFO_BASIC.GOUSYA ");
            sbSQL.AppendLine($" WHERE 1=1  ");
            sbSQL.AppendLine($"     AND T_YOYAKU_INFO_BASIC.YOYAKU_KBN = {prmYOYAKUKBN} ");
            sbSQL.AppendLine($"     AND T_YOYAKU_INFO_BASIC.YOYAKU_NO = {prmYOYAKUNO} ");
            return getDataTable(sbSQL.ToString());
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// 予約情報（基本）テーブル取得
    /// 予約区分と予約Noを指定して取得
    /// </summary>
    /// <returns></returns>
    public DataTable getYoyakuInfoBasic(int syuptDay, string crsCd)
    {
        var sbSQL = new StringBuilder();
        var wtRequestEntity = new WtRequestInfoEntity(); // 予約情報（基本）エンティティ
        var userMasterEntity = new UserMasterEntity();    // 利用者情報エンティティ

        // parameter clear
        base.paramClear();
        string prmSyuptDay = base.setParam(wtRequestEntity.syuptDay.PhysicsName, syuptDay, wtRequestEntity.syuptDay.DBType, wtRequestEntity.syuptDay.IntegerBu);
        string prmCrsCd = base.setParam(wtRequestEntity.crsCd.PhysicsName, crsCd, wtRequestEntity.crsCd.DBType, wtRequestEntity.crsCd.IntegerBu);
        string prmState = base.setParam(wtRequestEntity.state.PhysicsName, FixedCdYoyaku.YoyakuState.hakken, wtRequestEntity.state.DBType, wtRequestEntity.state.IntegerBu);
        string prmManagementKbn = base.setParam(wtRequestEntity.managementKbn.PhysicsName, ManagementKbn, wtRequestEntity.managementKbn.DBType, wtRequestEntity.managementKbn.IntegerBu);
        string prmCompanyCd = base.setParam(userMasterEntity.companyCd.PhysicsName, CompanyCd, userMasterEntity.companyCd.DBType, userMasterEntity.companyCd.IntegerBu);
        // Dim prmStay As Integer = FixedCd.CrsKindType.stay.GetHashCode

        try
        {
            sbSQL.AppendLine($"  SELECT  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.MANAGEMENT_KBN || LPAD(T_WT_REQUEST_INFO.MANAGEMENT_NO, 9, '0') WT_MANAGEMENT_NO,  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN.NINZU,  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.SYSTEM_ENTRY_DAY UKETUKE,  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.SUB_SEAT_WAIT_FLG,  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.MOTO_YOYAKU_KBN || LPAD(T_WT_REQUEST_INFO.MOTO_YOYAKU_NO, 9, '0') MOTO_YOYAKU_NO,  ");
            sbSQL.AppendLine($"      M_USER.USER_NAME ");
            sbSQL.AppendLine($"  FROM  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO  ");
            sbSQL.AppendLine($"  INNER JOIN  ");
            sbSQL.AppendLine($"      (SELECT   ");
            sbSQL.AppendLine($"            MANAGEMENT_KBN   ");
            sbSQL.AppendLine($"           ,MANAGEMENT_NO   ");
            sbSQL.AppendLine($"           ,SUM(CHARGE_APPLICATION_NINZU) NINZU   ");
            sbSQL.AppendLine($"       FROM   ");
            sbSQL.AppendLine($"           T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN   ");
            sbSQL.AppendLine($"       GROUP BY   ");
            sbSQL.AppendLine($"            MANAGEMENT_KBN   ");
            sbSQL.AppendLine($"           ,MANAGEMENT_NO) T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN   ");
            sbSQL.AppendLine($"  ON  ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.MANAGEMENT_KBN = T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN .MANAGEMENT_KBN  ");
            sbSQL.AppendLine($"      AND T_WT_REQUEST_INFO.MANAGEMENT_NO = T_WT_REQUEST_INFO_CRS_CHARGE_CHARGE_KBN .MANAGEMENT_NO  ");
            sbSQL.AppendLine($" LEFT OUTER JOIN  ");
            sbSQL.AppendLine($"     M_USER ");
            sbSQL.AppendLine($"     ON ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.SYSTEM_ENTRY_PERSON_CD = M_USER.USER_ID ");
            sbSQL.AppendLine($"  WHERE 1=1  ");
            sbSQL.AppendLine($"      AND T_WT_REQUEST_INFO.SYUPT_DAY = {prmSyuptDay}  ");
            sbSQL.AppendLine($"      AND T_WT_REQUEST_INFO.CRS_CD = {prmCrsCd} ");
            sbSQL.AppendLine($"      AND NVL(T_WT_REQUEST_INFO.STATE, ' ') = {prmState} ");
            sbSQL.AppendLine($"      AND T_WT_REQUEST_INFO.MANAGEMENT_KBN = {prmManagementKbn} ");
            sbSQL.AppendLine($"      AND M_USER.COMPANY_CD = {prmCompanyCd} ");
            sbSQL.AppendLine($"  ORDER BY ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.SYUPT_DAY, ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.CRS_CD, ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.STATE, ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.ENTRY_DAY, ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.ENTRY_TIME, ");
            sbSQL.AppendLine($"      T_WT_REQUEST_INFO.MANAGEMENT_NO ");
            return getDataTable(sbSQL.ToString());
        }
        catch
        {
            throw;
        }
    }

    #endregion

}