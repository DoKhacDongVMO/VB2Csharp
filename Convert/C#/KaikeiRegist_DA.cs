using System;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

/// <summary>
/// 会計連携_DA
/// </summary>
/// <remarks>
/// </remarks>
public partial class KaikeiRegist_DA : DataAccessorBase
{

    #region  定数／変数 
    private const int RUN_USER_SIZE = 20;
    #endregion

    #region メソッド
    /// <summary>
    /// 会計データ送信管理検索
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="Proces_Tani"></param>
    /// <param name="Management_Sec"></param>
    /// <param name="Proces_Kbn"></param>
    /// <param name="Proces_Status"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable getKaikeiDataSendingManagement(string TaisyoYM, string Proces_Tani = null, string Management_Sec = null, string Proces_Kbn = null, string Proces_Status = null)
    {
        OracleConnection connection = default;
        string sql = "";
        try
        {
            connection = openCon();
            sql = makeKaikeiDataSendingManagementSQL(TaisyoYM, Proces_Tani, Management_Sec, Proces_Kbn, Proces_Status);
            return this.getDataTable(connection, sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            closeCon(connection);
        }
    }

    /// <summary>
    /// 会計データ送信管理SQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="Proces_Tani"></param>
    /// <param name="Management_Sec"></param>
    /// <param name="Proces_Kbn"></param>
    /// <param name="Proces_Status"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeKaikeiDataSendingManagementSQL(string TaisyoYM, string Proces_Tani, string Management_Sec, string Proces_Kbn, string Proces_Status)
    {
        var sql = new StringBuilder();
        paramClear();
        sql.AppendLine(" SELECT ");
        sql.AppendLine("    SEND.TAISYO_YM ");
        sql.AppendLine("  , SEND.PROCESS_TANI ");
        sql.AppendLine("  , SEND.MANAGEMENT_SEC ");
        sql.AppendLine("  , SEND.SEQ ");
        sql.AppendLine("  , SEND.PROCESS_STATUS ");
        sql.AppendLine("  , SEND.PROCESS_DATE ");
        sql.AppendLine("  , SEND.PROCESS_TIME ");
        sql.AppendLine("  , SEND.UPDATE_USER_ID ");
        sql.AppendLine(" FROM ");
        sql.AppendLine("  T_ACCOUNTS_DATA_SEND SEND ");
        sql.AppendLine("  INNER JOIN ( ");
        sql.AppendLine("   SELECT ");
        sql.AppendLine("       TAISYO_YM ");
        sql.AppendLine("     , PROCESS_TANI ");
        sql.AppendLine("     , MANAGEMENT_SEC ");
        sql.AppendLine("     , PROCESS_KBN ");
        sql.AppendLine("     , MAX(SEQ) AS SEQ ");
        sql.AppendLine("   FROM ");
        sql.AppendLine("      T_ACCOUNTS_DATA_SEND ");
        sql.AppendLine("   WHERE ");
        sql.AppendLine("     TAISYO_YM = " + setParam("TAISYO_YM", TaisyoYM, OracleDbType.Char, 6));
        if (!string.IsNullOrEmpty(Proces_Tani))
        {
            sql.AppendLine(" AND PROCESS_TANI = " + setParam("PROCESS_TANI", Proces_Tani, OracleDbType.Char, 1));
        }

        if (!string.IsNullOrEmpty(Management_Sec))
        {
            sql.AppendLine(" AND MANAGEMENT_SEC = " + setParam("MANAGEMENT_SEC", Management_Sec, OracleDbType.Varchar2, 15));
        }

        if (!string.IsNullOrEmpty(Proces_Kbn))
        {
            sql.AppendLine(" AND PROCESS_KBN = " + setParam("PROCESS_KBN", Proces_Kbn, OracleDbType.Char, 1));
        }

        if (!string.IsNullOrEmpty(Proces_Status))
        {
            sql.AppendLine(" AND PROCESS_STATUS = " + setParam("PROCESS_STATUS", Proces_Status, OracleDbType.Char, 1));
        }

        sql.AppendLine("   GROUP BY ");
        sql.AppendLine("       TAISYO_YM ");
        sql.AppendLine("     , PROCESS_TANI ");
        sql.AppendLine("     , MANAGEMENT_SEC ");
        sql.AppendLine("     , PROCESS_KBN ");
        sql.AppendLine("  ) SENDMAX ");
        sql.AppendLine("  ON ( ");
        sql.AppendLine("         SEND.TAISYO_YM =  SENDMAX.TAISYO_YM ");
        sql.AppendLine("    AND  SEND.PROCESS_TANI =  SENDMAX.PROCESS_TANI ");
        sql.AppendLine("    AND  SEND.MANAGEMENT_SEC =  SENDMAX.MANAGEMENT_SEC ");
        sql.AppendLine("    AND  SEND.PROCESS_KBN =  SENDMAX.PROCESS_KBN ");
        sql.AppendLine("    AND  SEND.SEQ = SENDMAX.SEQ ");
        sql.AppendLine("  )  ");
        return sql.ToString();
    }

    /// <summary>
    /// コース関連売上・原価の最終更新日付チェック
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable chkCourseUriageGenkaLastUpdateDate(string TaisyoYM)
    {
        OracleConnection connection = default;
        string sql = "";
        try
        {
            connection = openCon();
            sql = makeCourseUriageGenkaLastUpdateDateSQL(TaisyoYM);
            return this.getDataTable(connection, sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            closeCon(connection);
        }
    }

    /// <summary>
    /// コース関連売上・原価の最終更新日付チェックSQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeCourseUriageGenkaLastUpdateDateSQL(string TaisyoYM)
    {
        var sql = new StringBuilder();
        paramClear();
        sql.AppendLine(" SELECT");
        sql.AppendLine("   *");
        sql.AppendLine(" FROM (");
        sql.AppendLine("     SELECT");
        sql.AppendLine("         T_COURSE_URIAGE.SUM_YM");
        sql.AppendLine("       , T_COURSE_URIAGE.MANAGEMENT_SEC");
        sql.AppendLine("       , MAX(T_COURSE_URIAGE.UPDATE_DATE) AS URIAGE_LAST_DATE");
        sql.AppendLine("     FROM");
        sql.AppendLine("       T_COURSE_URIAGE");
        sql.AppendLine("     WHERE");
        sql.AppendLine("       T_COURSE_URIAGE.SUM_YM = " + setParam("SUM_YM", TaisyoYM, OracleDbType.Char, 6));
        sql.AppendLine("     GROUP BY");
        sql.AppendLine("         T_COURSE_URIAGE.SUM_YM");
        sql.AppendLine("       , T_COURSE_URIAGE.MANAGEMENT_SEC");
        sql.AppendLine("   ) WK_URIAGE");
        sql.AppendLine("   LEFT JOIN (");
        sql.AppendLine("     SELECT");
        sql.AppendLine("         T_COURSE_GENKA.SUM_YM");
        sql.AppendLine("       , T_COURSE_GENKA.MANAGEMENT_SEC");
        sql.AppendLine("       , MAX(T_COURSE_GENKA.UPDATE_DATE) AS GENKA_LAST_DATE");
        sql.AppendLine("     FROM");
        sql.AppendLine("       T_COURSE_GENKA");
        sql.AppendLine("     WHERE");
        sql.AppendLine("       T_COURSE_GENKA.SUM_YM = " + setParam("SUM_YM", TaisyoYM, OracleDbType.Char, 6));
        sql.AppendLine("     GROUP BY");
        sql.AppendLine("         T_COURSE_GENKA.SUM_YM");
        sql.AppendLine("       , T_COURSE_GENKA.MANAGEMENT_SEC");
        sql.AppendLine("   ) WK_GENKA");
        sql.AppendLine("      ON WK_GENKA.SUM_YM          = WK_URIAGE.SUM_YM");
        sql.AppendLine("     AND WK_GENKA.MANAGEMENT_SEC  = WK_URIAGE.MANAGEMENT_SEC");
        sql.AppendLine(" WHERE");
        sql.AppendLine("   URIAGE_LAST_DATE > GENKA_LAST_DATE");
        return sql.ToString();
    }

    /// <summary>
    /// コース関連売上集計実行
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="Houjin_T_Kbn"></param>
    /// <param name="Houjin_K_Kbn"></param>
    /// <param name="GaikyakuKbn"></param>
    /// <param name="ret">実行結果戻り値</param>
    /// <remarks></remarks>
    public void execCourseUriageSyukei(string TaisyoYM, string Houjin_T_Kbn, string Houjin_K_Kbn, string GaikyakuKbn, ref KaikeiRegistCommon.ProcedureExecResult ret)
    {
        OracleConnection con = default;
        OracleCommand cmd = default;
        try
        {
            ret.retCode = 0;
            ret.runUser = "";
            ret.sqlCode = 0;
            con = openCon();
            cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = KaikeiRegistCommon.PRC_NM_COURSE_URIAGE;
            cmd.Parameters.Add("pYM", OracleDbType.Int32, TaisyoYM, ParameterDirection.Input);
            cmd.Parameters.Add("pHoujin_T_Kbn", OracleDbType.Char, Houjin_T_Kbn, ParameterDirection.Input);
            cmd.Parameters.Add("pHoujin_K_Kbn", OracleDbType.Char, Houjin_K_Kbn, ParameterDirection.Input);
            cmd.Parameters.Add("pGaikyakuKbn", OracleDbType.Char, GaikyakuKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pClient", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(SystemInfoManagement.client), "1", SystemInfoManagement.client), ParameterDirection.Input);
            cmd.Parameters.Add("pUserId", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(UserInfoManagement.userId), "1", UserInfoManagement.userId), ParameterDirection.Input);
            cmd.Parameters.Add("pTanto", OracleDbType.Varchar2, "1", ParameterDirection.Input);
            cmd.Parameters.Add("pRetCode", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add("pRunUser", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.Parameters("pRunUser").Size = RUN_USER_SIZE;
            cmd.Parameters.Add("pSQLCODE", OracleDbType.Int32, ParameterDirection.Output);
            execNonQuery(cmd);
            ret.retCode = Convert.ToInt32(cmd.Parameters("pRetCode").Value.ToString);
            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Running)
            {
                ret.runUser = cmd.Parameters("pRunUser").Value.ToString;
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Error)
            {
                ret.sqlCode = Convert.ToInt32(cmd.Parameters("pSQLCODE").Value.ToString);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (cmd is object)
            {
                cmd.Dispose();
            }

            closeCon(con);
        }
    }

    /// <summary>
    /// コース関連売上原価集計実行
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="Houjin_T_Kbn"></param>
    /// <param name="Houjin_K_Kbn"></param>
    /// <param name="GaikyakuKbn"></param>
    /// <param name="ret">実行結果戻り値</param>
    /// <remarks></remarks>
    public void execCourseGenkaSyukei(string TaisyoYM, string Houjin_T_Kbn, string Houjin_K_Kbn, string GaikyakuKbn, ref KaikeiRegistCommon.ProcedureExecResult ret)
    {
        OracleConnection con = default;
        OracleCommand cmd = default;
        try
        {
            ret.retCode = 0;
            ret.runUser = "";
            ret.sqlCode = 0;
            con = openCon();
            cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = KaikeiRegistCommon.PRC_NM_COURSE_GENKA;
            cmd.Parameters.Add("pYM", OracleDbType.Int32, TaisyoYM, ParameterDirection.Input);
            cmd.Parameters.Add("pHoujin_T_Kbn", OracleDbType.Char, Houjin_T_Kbn, ParameterDirection.Input);
            cmd.Parameters.Add("pHoujin_K_Kbn", OracleDbType.Char, Houjin_K_Kbn, ParameterDirection.Input);
            cmd.Parameters.Add("pGaikyakuKbn", OracleDbType.Char, GaikyakuKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pClient", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(SystemInfoManagement.client), "1", SystemInfoManagement.client), ParameterDirection.Input);
            cmd.Parameters.Add("pUserId", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(UserInfoManagement.userId), "1", UserInfoManagement.userId), ParameterDirection.Input);
            cmd.Parameters.Add("pTanto", OracleDbType.Varchar2, "1", ParameterDirection.Input);
            cmd.Parameters.Add("pRetCode", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add("pRunUser", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.Parameters("pRunUser").Size = RUN_USER_SIZE;
            cmd.Parameters.Add("pSQLCODE", OracleDbType.Int32, ParameterDirection.Output);
            execNonQuery(cmd);
            ret.retCode = Convert.ToInt32(cmd.Parameters("pRetCode").Value.ToString);
            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Running)
            {
                ret.runUser = cmd.Parameters("pRunUser").Value.ToString;
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Error)
            {
                ret.sqlCode = Convert.ToInt32(cmd.Parameters("pSQLCODE").Value.ToString);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (cmd is object)
            {
                cmd.Dispose();
            }

            closeCon(con);
        }
    }

    /// <summary>
    /// 営業所関連売上集計実行
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="ret">実行結果戻り値</param>
    /// <remarks></remarks>
    public void execEigyosyoSyukei(string TaisyoYM, ref KaikeiRegistCommon.ProcedureExecResult ret)
    {
        OracleConnection con = default;
        OracleCommand cmd = default;
        try
        {
            ret.retCode = 0;
            ret.runUser = "";
            ret.sqlCode = 0;
            con = openCon();
            cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = KaikeiRegistCommon.PRC_NM_EIGYOSYO_URIAGE;
            cmd.Parameters.Add("pYM", OracleDbType.Int32, TaisyoYM, ParameterDirection.Input);
            cmd.Parameters.Add("pClient", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(SystemInfoManagement.client), "1", SystemInfoManagement.client), ParameterDirection.Input);
            cmd.Parameters.Add("pUserId", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(UserInfoManagement.userId), "1", UserInfoManagement.userId), ParameterDirection.Input);
            cmd.Parameters.Add("pTanto", OracleDbType.Varchar2, "1", ParameterDirection.Input);
            cmd.Parameters.Add("pRetCode", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add("pRunUser", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.Parameters("pRunUser").Size = RUN_USER_SIZE;
            cmd.Parameters.Add("pSQLCODE", OracleDbType.Int32, ParameterDirection.Output);
            execNonQuery(cmd);
            ret.retCode = Convert.ToInt32(cmd.Parameters("pRetCode").Value.ToString);
            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Running)
            {
                ret.runUser = cmd.Parameters("pRunUser").Value.ToString;
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Error)
            {
                ret.sqlCode = Convert.ToInt32(cmd.Parameters("pSQLCODE").Value.ToString);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (cmd is object)
            {
                cmd.Dispose();
            }

            closeCon(con);
        }
    }

    /// <summary>
    /// 会計連携実行
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="BtnKbn"></param>
    /// <param name="ModeKbn"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <param name="ret">実行結果戻り値</param>
    /// <remarks></remarks>
    public void execKaikeiRenkei(string TaisyoYM, string BtnKbn, string ModeKbn, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn, ref KaikeiRegistCommon.ProcedureExecResult ret)
    {
        OracleTransaction tranBO = default;
        OracleTransaction tranCORE = default;
        try
        {
            tranBO = callBeginTransaction();
            if (BtnKbn == KaikeiRegistCommon.DATA_TYPE_RENKEI)
            {
                tranCORE = callBeginTransaction(GetSetting(tranBO, KaikeiRegistCommon.CODE_VALUE_SSJ_CONNECTION_STRING, 1));
                setLockCORERenkei(tranCORE);
            }

            execKaikeiRenkeiProcedure(tranBO, TaisyoYM, BtnKbn, ModeKbn, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn, ref ret);
            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Running)
            {
                callRollbackTransaction(tranCORE);
                callRollbackTransaction(tranBO);
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Error)
            {
                callRollbackTransaction(tranCORE);
                callRollbackTransaction(tranBO);
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.NormalEnd)
            {
                callCommitTransaction(tranBO);
                if (BtnKbn == KaikeiRegistCommon.DATA_TYPE_RENKEI)
                {
                    tranBO = callBeginTransaction();
                    execCoreRenkei(tranCORE, tranBO, TaisyoYM, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn);
                    callCommitTransaction(tranCORE);
                    callCommitTransaction(tranBO);
                }
            }
        }
        catch (Exception ex)
        {
            callRollbackTransaction(tranCORE);
            callRollbackTransaction(tranBO);
            throw ex;
        }
    }

    /// <summary>
    /// 会計連携プロシージャ実行
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="TaisyoYM"></param>
    /// <param name="BtnKbn"></param>
    /// <param name="ModeKbn"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <param name="ret">実行結果戻り値</param>
    /// <remarks></remarks>
    public void execKaikeiRenkeiProcedure(OracleTransaction tran, string TaisyoYM, string BtnKbn, string ModeKbn, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn, ref KaikeiRegistCommon.ProcedureExecResult ret)
    {
        OracleCommand cmd = default;
        try
        {
            ret.retCode = 0;
            ret.runUser = "";
            ret.sqlCode = 0;
            cmd = tran.Connection.CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = KaikeiRegistCommon.PRC_NM_KAIKEI_RENKEI;
            cmd.Parameters.Add("pYM", OracleDbType.Int32, TaisyoYM, ParameterDirection.Input);
            cmd.Parameters.Add("pBtnKbn", OracleDbType.Char, BtnKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pModeKbn", OracleDbType.Char, ModeKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pEigyosyoKbn", OracleDbType.Char, EigyosyoKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pCourseTeikiKbn", OracleDbType.Char, CourseTeikiKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pCourseKikakuKbn", OracleDbType.Char, CourseKikakuKbn, ParameterDirection.Input);
            cmd.Parameters.Add("pClient", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(SystemInfoManagement.client), "1", SystemInfoManagement.client), ParameterDirection.Input);
            cmd.Parameters.Add("pUserId", OracleDbType.Varchar2, Interaction.IIf(string.IsNullOrEmpty(UserInfoManagement.userId), "1", UserInfoManagement.userId), ParameterDirection.Input);
            cmd.Parameters.Add("pTanto", OracleDbType.Varchar2, "1", ParameterDirection.Input);
            cmd.Parameters.Add("pRetCode", OracleDbType.Int32, ParameterDirection.Output);
            cmd.Parameters.Add("pRunUser", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.Parameters("pRunUser").Size = RUN_USER_SIZE;
            cmd.Parameters.Add("pSQLCODE", OracleDbType.Int32, ParameterDirection.Output);
            execNonQuery(cmd);
            ret.retCode = Convert.ToInt32(cmd.Parameters("pRetCode").Value.ToString);
            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Running)
            {
                ret.runUser = cmd.Parameters("pRunUser").Value.ToString;
            }

            if (ret.retCode == KaikeiRegistCommon.Kaikei_ExecResult.Error)
            {
                ret.sqlCode = Convert.ToInt32(cmd.Parameters("pSQLCODE").Value.ToString);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (cmd is object)
            {
                cmd.Dispose();
            }
        }
    }

    /// <summary>
    /// CORE連携
    /// </summary>
    /// <param name="tranCORE"></param>
    /// <param name="tranBO"></param>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <remarks></remarks>
    public void execCoreRenkei(OracleTransaction tranCORE, OracleTransaction tranBO, string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        DataTable dt;
        string sql = "";
        try
        {
            if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON && CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
            {
                updRenSiireGenka(tranBO, TaisyoYM, 0);
            }
            else if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON && CourseKikakuKbn == KaikeiRegistCommon.CHECK_OFF)
            {
                updRenSiireGenka(tranBO, TaisyoYM, 1);
            }
            else if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_OFF && CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
            {
                updRenSiireGenka(tranBO, TaisyoYM, 2);
            }

            sql = makeGetSSSiwakeInfoSQL(TaisyoYM, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn);
            dt = this.getDataTable(tranBO, sql);
            foreach (DataRow row in dt.Rows)
            {
                sql = makeInsCoreRenkeiSQL(row);
                execNonQuery(tranCORE, sql);
            }

            sql = makeUpdAccountsDataSendSQL(TaisyoYM, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn);
            execNonQuery(tranBO, sql);
        }
        catch (Exception ex)
        {
            sql = makeDelAccountsDataSendSQL(TaisyoYM, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn);
            execNonQuery(tranBO, sql);
            throw ex;
        }
    }

    /// <summary>
    /// SS送信仕分情報取得SQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeGetSSSiwakeInfoSQL(string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        var sql = new StringBuilder();
        string DataKbn = "";
        if (EigyosyoKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(DataKbn))
                DataKbn += ",";
            DataKbn += "'" + KaikeiRegistCommon.DATA_KBN_EIGYOSYO + "'";
        }

        if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(DataKbn))
                DataKbn += ",";
            DataKbn += "'" + KaikeiRegistCommon.DATA_KBN_CRS_URIAGE_TEIKI + "'";
        }

        if (CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(DataKbn))
                DataKbn += ",";
            DataKbn += "'" + KaikeiRegistCommon.DATA_KBN_CRS_URIAGE_KIKAKU + "'";
        }

        paramClear();
        sql.AppendLine(" SELECT  ");
        sql.AppendLine("    COMPANY_CD       AS SW2_KAI_CODE ");
        sql.AppendLine("  , DENPYO_DATE      AS SW2_DATE ");
        sql.AppendLine("  , DENPYO_NO        AS SW2_DEN_NO ");
        sql.AppendLine("  , DENPYO_MEISAI_NO AS SW2_GYO_NO ");
        sql.AppendLine("  , TAISYAKU_KBN     AS SW2_DC_KBN ");
        sql.AppendLine("  , KAMOKU_CD        AS SW2_KMK_CODE ");
        sql.AppendLine("  , KAMOKU_H_CD      AS SW2_HKM_CODE ");
        sql.AppendLine("  , BUMON_CD         AS SW2_BMN_CODE ");
        sql.AppendLine("  , SIWAKE_GAKU      AS SW2_KIN ");
        sql.AppendLine("  , TAX_CD           AS SW2_ZEI_CODE ");
        sql.AppendLine("  , TAX_KBN          AS SW2_ZEI_KBN ");
        sql.AppendLine("  , TAX              AS SW2_ZEI_KIN ");
        sql.AppendLine("  , TEKIYO_1         AS SW2_TEKIYO1 ");
        sql.AppendLine("  , TEKIYO_2         AS SW2_TEKIYO2 ");
        sql.AppendLine("  , DENPYO_GROUP_CD  AS SW2_GRP_CODE ");
        sql.AppendLine("  , SYSTEM_KBN       AS SW2_SYS_KBN ");
        sql.AppendLine("  , '          '     AS SW2_AIT_KMK_CODE ");
        sql.AppendLine("  , AUTHORTY_KBN     AS SW2_SHONIN_KBN ");
        sql.AppendLine("  , DENPYO_KBN       AS SW2_DEN_KBN ");
        sql.AppendLine("  , HAIFU_KBN        AS SW2_HAIFU_KBN ");
        sql.AppendLine("  , AUTHORTY_LEVEL   AS SW2_REC_LEVEL ");
        sql.AppendLine("  , TORIHIKI_KBN     AS SW2_TORI_KBN ");
        sql.AppendLine(" FROM T_SS_SIWAKE_INFO ");
        sql.AppendLine(" WHERE SUM_YM =  " + setParam("SUM_YM", TaisyoYM, OracleDbType.Char, 6));
        sql.AppendLine("   AND DATA_TYPE = '0' ");
        sql.AppendLine("   AND DATA_KBN IN ( " + DataKbn + ")");
        return sql.ToString();
    }

    /// <summary>
    /// CORE連携テーブル登録
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeInsCoreRenkeiSQL(DataRow row)
    {
        var sql = new StringBuilder();
        paramClear();
        sql.AppendLine(" INSERT INTO CMSW2WRK ");
        sql.AppendLine(" ( ");
        sql.AppendLine("    SW2_KAI_CODE ");
        sql.AppendLine("  , SW2_DATE ");
        sql.AppendLine("  , SW2_DEN_NO ");
        sql.AppendLine("  , SW2_GYO_NO ");
        sql.AppendLine("  , SW2_DC_KBN ");
        sql.AppendLine("  , SW2_KMK_CODE ");
        sql.AppendLine("  , SW2_HKM_CODE ");
        sql.AppendLine("  , SW2_BMN_CODE ");
        sql.AppendLine("  , SW2_KIN ");
        sql.AppendLine("  , SW2_ZEI_CODE ");
        sql.AppendLine("  , SW2_ZEI_KBN ");
        sql.AppendLine("  , SW2_ZEI_KIN ");
        sql.AppendLine("  , SW2_TEKIYO1 ");
        sql.AppendLine("  , SW2_TEKIYO2 ");
        sql.AppendLine("  , SW2_GRP_CODE ");
        sql.AppendLine("  , SW2_SYS_KBN ");
        sql.AppendLine("  , SW2_AIT_KMK_CODE ");
        sql.AppendLine("  , SW2_SHONIN_KBN ");
        sql.AppendLine("  , SW2_DEN_KBN ");
        sql.AppendLine("  , SW2_HAIFU_KBN ");
        sql.AppendLine("  , SW2_REC_LEVEL ");
        sql.AppendLine("  , SW2_TORI_KBN ");
        sql.AppendLine(" ) ");
        sql.AppendLine(" VALUES ");
        sql.AppendLine(" ( ");
        sql.AppendLine("   " + setParam("SW2_KAI_CODE", row.Item("SW2_KAI_CODE"), OracleDbType.Char, 5));
        sql.AppendLine("  ," + setParam("SW2_DATE", row.Item("SW2_DATE"), OracleDbType.Date));
        sql.AppendLine("  ," + setParam("SW2_DEN_NO", row.Item("SW2_DEN_NO"), OracleDbType.Char, 8));
        sql.AppendLine("  ," + setParam("SW2_GYO_NO", row.Item("SW2_GYO_NO"), OracleDbType.Int32, 5));
        sql.AppendLine("  ," + setParam("SW2_DC_KBN", row.Item("SW2_DC_KBN"), OracleDbType.Char, 1));
        sql.AppendLine("  ," + setParam("SW2_KMK_CODE", row.Item("SW2_KMK_CODE"), OracleDbType.Char, 10));
        sql.AppendLine("  ," + setParam("SW2_HKM_CODE", row.Item("SW2_HKM_CODE"), OracleDbType.Char, 10));
        sql.AppendLine("  ," + setParam("SW2_BMN_CODE", row.Item("SW2_BMN_CODE"), OracleDbType.Char, 10));
        sql.AppendLine("  ," + setParam("SW2_KIN", row.Item("SW2_KIN"), OracleDbType.Decimal, 21));
        sql.AppendLine("  ," + setParam("SW2_ZEI_CODE", row.Item("SW2_ZEI_CODE"), OracleDbType.Char, 4));
        sql.AppendLine("  ," + setParam("SW2_ZEI_KBN", row.Item("SW2_ZEI_KBN"), OracleDbType.Char, 1));
        sql.AppendLine("  ," + setParam("SW2_ZEI_KIN", row.Item("SW2_ZEI_KIN"), OracleDbType.Decimal, 21));
        sql.AppendLine("  ," + setParam("SW2_TEKIYO1", row.Item("SW2_TEKIYO1"), OracleDbType.Varchar2, 40));
        sql.AppendLine("  ," + setParam("SW2_TEKIYO2", row.Item("SW2_TEKIYO2"), OracleDbType.Varchar2, 40));
        sql.AppendLine("  ," + setParam("SW2_GRP_CODE", row.Item("SW2_GRP_CODE"), OracleDbType.Char, 2));
        sql.AppendLine("  ," + setParam("SW2_SYS_KBN", row.Item("SW2_SYS_KBN"), OracleDbType.Char, 2));
        sql.AppendLine("  ," + setParam("SW2_AIT_KMK_CODE", row.Item("SW2_AIT_KMK_CODE"), OracleDbType.Char, 10));
        sql.AppendLine("  ," + setParam("SW2_SHONIN_KBN", row.Item("SW2_SHONIN_KBN"), OracleDbType.Char, 1));
        sql.AppendLine("  ," + setParam("SW2_DEN_KBN", row.Item("SW2_DEN_KBN"), OracleDbType.Char, 1));
        sql.AppendLine("  ," + setParam("SW2_HAIFU_KBN", row.Item("SW2_HAIFU_KBN"), OracleDbType.Char, 1));
        sql.AppendLine("  ," + setParam("SW2_REC_LEVEL", row.Item("SW2_REC_LEVEL"), OracleDbType.Int16, 1));
        sql.AppendLine("  ," + setParam("SW2_TORI_KBN", row.Item("SW2_TORI_KBN"), OracleDbType.Char, 1));
        sql.AppendLine(" ) ");
        return sql.ToString();
    }

    /// <summary>
    /// 会計データ送信管理更新SQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeUpdAccountsDataSendSQL(string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        var sql = new StringBuilder();
        string SyukeiTani = "";
        paramClear();
        sql.AppendLine(" UPDATE T_ACCOUNTS_DATA_SEND ");
        sql.AppendLine(" SET ");
        sql.AppendLine("     PROCESS_STATUS = '1' ");
        sql.AppendLine("   , PROCESS_DATE = TO_CHAR(SYSDATE,'YYYYMMDD')");
        sql.AppendLine("   , PROCESS_TIME = TO_CHAR(SYSDATE,'HH24MISS')");
        sql.AppendLine("   , PROCESS_TANTO = " + setParam("PROCESS_TANTO", "1", OracleDbType.Varchar2));
        sql.AppendLine("   , UPDATE_DATE = SYSDATE ");
        sql.AppendLine("   , UPDATE_USER_ID = " + setParam("UPDATE_USER_ID", Interaction.IIf(string.IsNullOrEmpty(UserInfoManagement.userId), "1", UserInfoManagement.userId), OracleDbType.Varchar2));
        sql.AppendLine("   , UPDATE_CLIENT = " + setParam("UPDATE_CLIENT", Interaction.IIf(string.IsNullOrEmpty(SystemInfoManagement.client), "1", SystemInfoManagement.client), OracleDbType.Varchar2));
        sql.AppendLine(" WHERE ");
        sql.AppendLine("      TAISYO_YM = " + setParam("TAISYO_YM", TaisyoYM, OracleDbType.Char, 6));
        sql.AppendLine("  AND PROCESS_KBN = " + setParam("PROCESS_KBN", KaikeiRegistCommon.Kaikei_ProcessKbn.SSJSEND, OracleDbType.Char, 1));
        sql.AppendLine("  AND PROCESS_STATUS = " + setParam("PROCESS_STATUS", KaikeiRegistCommon.Kaikei_ProcessStatus.PROCESSING, OracleDbType.Char, 1));
        if (EigyosyoKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.EIGYOSYO + "'";
        }

        if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.COURSE_SSJSEND_TEIKI + "'";
        }

        if (CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.COURSE_SSJSEND_KIKAKU + "'";
        }

        sql.AppendLine("  AND MANAGEMENT_SEC IN (" + SyukeiTani + ")");
        return sql.ToString();
    }

    /// <summary>
    /// 会計データ送信管理削除SQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makeDelAccountsDataSendSQL(string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        var sql = new StringBuilder();
        string SyukeiTani = "";
        paramClear();
        sql.AppendLine(" DELETE FROM T_ACCOUNTS_DATA_SEND ");
        sql.AppendLine(" WHERE ");
        sql.AppendLine("      TAISYO_YM = " + setParam("TAISYO_YM", TaisyoYM, OracleDbType.Char, 6));
        sql.AppendLine("  AND PROCESS_KBN = " + setParam("PROCESS_KBN", KaikeiRegistCommon.Kaikei_ProcessKbn.SSJSEND, OracleDbType.Char, 1));
        sql.AppendLine("  AND PROCESS_STATUS = " + setParam("PROCESS_STATUS", KaikeiRegistCommon.Kaikei_ProcessStatus.PROCESSING, OracleDbType.Char, 1));
        if (EigyosyoKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.EIGYOSYO + "'";
        }

        if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.COURSE_SSJSEND_TEIKI + "'";
        }

        if (CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if (!string.IsNullOrEmpty(SyukeiTani))
                SyukeiTani += ",";
            SyukeiTani += "'" + KaikeiRegistCommon.Kaikei_SyukeiTani.COURSE_SSJSEND_KIKAKU + "'";
        }

        sql.AppendLine("  AND MANAGEMENT_SEC IN (" + SyukeiTani + ")");
        return sql.ToString();
    }

    /// <summary>
    /// CORE連携テーブルロック
    /// </summary>
    /// <param name="tran"></param>
    /// <remarks></remarks>
    private void setLockCORERenkei(OracleTransaction tran)
    {
        var sql = new StringBuilder();
        try
        {
            paramClear();
            sql.AppendLine("LOCK TABLE CMSW2WRK IN EXCLUSIVE MODE");
            execNonQuery(tran, sql.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// システム設定取得
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="pCODE_VALE"></param>
    /// <param name="pNAIYO_NO"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string GetSetting(OracleTransaction tran, string pCODE_VALE, int pNAIYO_NO)
    {
        try
        {
            string sql = "";
            sql += " SELECT";
            sql += "   NAIYO_1, NAIYO_2, NAIYO_3";
            sql += " FROM M_CODE";
            sql += " WHERE";
            sql += "       CODE_BUNRUI = '400'";
            sql += "   AND CODE_VALUE = " + setParam("CODE_VALUE", pCODE_VALE, OracleDbType.Varchar2);
            DataTable dt;
            dt = this.getDataTable(tran, sql);
            if (dt.Rows.Count > 0)
            {
                return Conversions.ToString(dt.Rows(0).Item("NAIYO_" + pNAIYO_NO.ToString()));
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 送信結果リスト取得
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable getSendResultList(string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        OracleConnection connection = default;
        string sql = "";
        try
        {
            connection = openCon();
            sql = makegetSendResultListSQL(TaisyoYM, EigyosyoKbn, CourseTeikiKbn, CourseKikakuKbn);
            return this.getDataTable(connection, sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            closeCon(connection);
        }
    }

    /// <summary>
    /// 送信結果リスト取得SQL作成
    /// </summary>
    /// <param name="TaisyoYM"></param>
    /// <param name="EigyosyoKbn"></param>
    /// <param name="CourseTeikiKbn"></param>
    /// <param name="CourseKikakuKbn"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string makegetSendResultListSQL(string TaisyoYM, string EigyosyoKbn, string CourseTeikiKbn, string CourseKikakuKbn)
    {
        var sql = new StringBuilder();
        string DATA_KBN = string.Empty;
        if (EigyosyoKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if ((DATA_KBN ?? "") != (string.Empty ?? ""))
                DATA_KBN += ",";
            DATA_KBN += "'" + KaikeiRegistCommon.DATA_KBN_EIGYOSYO + "'";
        }

        if (CourseTeikiKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if ((DATA_KBN ?? "") != (string.Empty ?? ""))
                DATA_KBN += ",";
            DATA_KBN += "'" + KaikeiRegistCommon.DATA_KBN_CRS_URIAGE_TEIKI + "'";
        }

        if (CourseKikakuKbn == KaikeiRegistCommon.CHECK_ON)
        {
            if ((DATA_KBN ?? "") != (string.Empty ?? ""))
                DATA_KBN += ",";
            DATA_KBN += "'" + KaikeiRegistCommon.DATA_KBN_CRS_URIAGE_KIKAKU + "'";
        }

        paramClear();
        sql.AppendLine(" SELECT");
        sql.AppendLine("     T_SS_SIWAKE_INFO.SUM_YM");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DATA_KBN");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DATA_TYPE");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DENPYO_NO");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DENPYO_MEISAI_NO");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DENPYO_DATE");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.BUMON_CD");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.KAMOKU_CD");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.KAMOKU_H_CD");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '0', NVL(T_SS_SIWAKE_INFO.SIWAKE_GAKU, 0), NULL) AS KARI_SIWAKE_GAKU");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '0', NVL(T_SS_SIWAKE_INFO.TAX, 0), NULL) AS KARI_TAX");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '0', NVL(T_SS_SIWAKE_INFO.SIWAKE_GAKU, 0) + NVL(T_SS_SIWAKE_INFO.TAX, 0), NULL) AS KARI_KEI");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '1', NVL(T_SS_SIWAKE_INFO.SIWAKE_GAKU, 0), NULL) AS KASI_SIWAKE_GAKU");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '1', NVL(T_SS_SIWAKE_INFO.TAX, 0), NULL) AS KASI_TAX");
        sql.AppendLine("   , DECODE(T_SS_SIWAKE_INFO.TAISYAKU_KBN, '1', NVL(T_SS_SIWAKE_INFO.SIWAKE_GAKU, 0) + NVL(T_SS_SIWAKE_INFO.TAX, 0), NULL) AS KASI_KEI");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.TEKIYO_1");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.TEKIYO_2");
        sql.AppendLine("   , M_SS_BUMON.SS_BUMON_NAME");
        sql.AppendLine("   , WK_KAMOKU.KAMOKU_NAME");
        sql.AppendLine("   , WK_KAMOKU.KAMOKU_HOJO_NAME");
        sql.AppendLine(" FROM");
        sql.AppendLine("   T_SS_SIWAKE_INFO");
        sql.AppendLine("   LEFT JOIN M_SS_BUMON");
        sql.AppendLine("     ON TRIM(M_SS_BUMON.SS_BUMON_CD) = TRIM(T_SS_SIWAKE_INFO.BUMON_CD)");
        sql.AppendLine("   LEFT JOIN (");
        sql.AppendLine("     SELECT DISTINCT");
        sql.AppendLine("         SS_KAMOKU_CD");
        sql.AppendLine("       , SS_KAMOKU_H_CD");
        sql.AppendLine("       , KAMOKU_NAME");
        sql.AppendLine("       , KAMOKU_HOJO_NAME");
        sql.AppendLine("     FROM");
        sql.AppendLine("       M_SS_KAMOKU_HOJO_TORIHIKI");
        sql.AppendLine("   ) WK_KAMOKU");
        sql.AppendLine("     ON TRIM(WK_KAMOKU.SS_KAMOKU_CD) = TRIM(T_SS_SIWAKE_INFO.KAMOKU_CD)");
        sql.AppendLine("    AND TRIM(NVL(WK_KAMOKU.SS_KAMOKU_H_CD,'XXXXXXXXXXX')) = TRIM(NVL(T_SS_SIWAKE_INFO.KAMOKU_H_CD, 'XXXXXXXXXXX'))");
        sql.AppendLine(" WHERE");
        sql.AppendLine("       T_SS_SIWAKE_INFO.SUM_YM = " + setParam("SUM_YM", TaisyoYM, OracleDbType.Char, 6));
        if (!string.IsNullOrEmpty(DATA_KBN))
        {
            sql.AppendLine("   AND T_SS_SIWAKE_INFO.DATA_KBN IN (" + DATA_KBN + ")");
        }

        sql.AppendLine(" ORDER BY");
        sql.AppendLine("     T_SS_SIWAKE_INFO.DENPYO_NO");
        sql.AppendLine("   , T_SS_SIWAKE_INFO.DENPYO_MEISAI_NO");
        return sql.ToString();
    }

    /// <summary>
    /// 連携済仕入原価更新
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="TaisyoYM"></param>
    /// <param name="Flg"></param>
    /// <remarks></remarks>
    private void updRenSiireGenka(OracleTransaction tran, string TaisyoYM, int Flg)
    {
        var sql = new StringBuilder();
        try
        {
            paramClear();
            sql.AppendLine(" UPDATE T_REN_SIIRE_GENKA");
            sql.AppendLine(" SET");
            sql.AppendLine("   KAIKEI_REGIST_DATE = SYSDATE");
            sql.AppendLine(" WHERE");
            sql.AppendLine("      KAIKEI_YM = " + setParam("KAIKEI_YM", TaisyoYM, OracleDbType.Char, 6));
            sql.AppendLine("  AND KAIKEI_REGIST_DATE IS NULL");
            if (Flg == 1)
            {
                // 日本語定期・外国語
                sql.AppendLine("  AND (HEAD_ITEM_CD = '1' OR HOUJIN_KBN = 'G')");
            }
            else if (Flg == 2)
            {
                // 日本語企画
                sql.AppendLine("  AND (HEAD_ITEM_CD = '2' AND HOUJIN_KBN = 'H')");
            }

            execNonQuery(tran, sql.ToString());
        }
        catch (OracleException ex)
        {
            throw ex;
        }
    }

    #endregion

}