using System;
using System.Collections;
using System.Linq;
using System.Text;
/// <summary>
/// 会計データ送信管理
/// </summary>
/// <remarks>
/// Author:2018/10/06//PhucNH
/// </remarks>
public partial class AccountDataSend_DA : DataAccessorBase  // 会計データ送信管理_DA
{

    #region  定数／変数 
    private TAccountsDataSendEntity accountDataSendEntity = new TAccountsDataSendEntity();
    #endregion

    /// <summary>
    /// 指定した年月で売掛情報テーブルにデータを取得する
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public int countAccountDataSendByTaisoYm(Hashtable paramList)
    {
        var resultdatatable = new DataTable();
        // sql生成
        var sql = new StringBuilder();
        try
        {
            sql.AppendLine("SELECT COUNT(1) ");
            sql.AppendLine("FROM T_ACCOUNTS_DATA_SEND TADS ");
            sql.AppendLine("INNER JOIN ( ");
            sql.AppendLine("        SELECT TAISYO_YM ");
            sql.AppendLine("                ,PROCESS_TANI ");
            sql.AppendLine("                ,MANAGEMENT_SEC ");
            sql.AppendLine("                ,PROCESS_KBN ");
            sql.AppendLine("                ,MAX(SEQ) AS SEQ ");
            sql.AppendLine("        FROM T_ACCOUNTS_DATA_SEND ");
            {
                var withBlock = accountDataSendEntity;
                sql.AppendLine("        WHERE PROCESS_TANI = '0' ");
                sql.AppendLine("            AND MANAGEMENT_SEC IN ('1', '2', '3') ");
                sql.AppendLine("            AND PROCESS_KBN = '1' ");
                sql.AppendLine("            AND TAISYO_YM = " + setParam("TaisoYm", paramList["TaisoYm"], withBlock.taisyoYm.DBType, withBlock.taisyoYm.IntegerBu, withBlock.taisyoYm.DecimalBu));
            }

            sql.AppendLine("        GROUP BY TAISYO_YM ");
            sql.AppendLine("                ,PROCESS_TANI ");
            sql.AppendLine("                ,MANAGEMENT_SEC ");
            sql.AppendLine("                ,PROCESS_KBN ");
            sql.AppendLine("        ) M ON TADS.TAISYO_YM = M.TAISYO_YM ");
            sql.AppendLine("        AND TADS.PROCESS_TANI = M.PROCESS_TANI ");
            sql.AppendLine("        AND TADS.MANAGEMENT_SEC = M.MANAGEMENT_SEC ");
            sql.AppendLine("        AND TADS.PROCESS_KBN = M.PROCESS_KBN ");
            sql.AppendLine("        AND TADS.SEQ = M.SEQ ");
            sql.AppendLine("        AND TADS.PROCESS_STATUS = '1' ");
            resultdatatable = getDataTable(sql.ToString());
        }
        catch (Exception ex)
        {
            throw;
        }

        if (resultdatatable.Rows.Count > 0)
        {
            return resultdatatable.Rows.Item(0).Item(0);
        }
        else
        {
            return 0;
        }
    }
}