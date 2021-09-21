/// <summary>
/// 連携済仕入原価
/// </summary>
/// <remarks>
/// Author:2018/10/06//PhucNH
/// </remarks>
using System;
using System.Collections;
using System.Linq;

public partial class RenShiireGenka_DA : DataAccessorBase  // 連携済仕入原価_DA
{

    #region  定数／変数 
    private TRenSiireGenkaEntity rensiireGenkaEntity = new TRenSiireGenkaEntity();
    #endregion

    /// <summary>
    /// 連携済仕入原価テーブルにデータを数える
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public int countRenShiireGenkaByTargetDay(Hashtable paramList)
    {
        var resultdatatable = new DataTable();
        string strsql = "";
        try
        {
            strsql += "SELECT COUNT(1) ";
            strsql += "FROM T_REN_SIIRE_GENKA ";
            {
                var withBlock = rensiireGenkaEntity;
                strsql += "WHERE KAIKEI_YM = " + setParam("KaikeiYm", paramList["KaikeiYm"], withBlock.kaikeiYm.DBType, withBlock.kaikeiYm.IntegerBu, withBlock.kaikeiYm.DecimalBu);
            }

            resultdatatable = getDataTable(strsql);
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