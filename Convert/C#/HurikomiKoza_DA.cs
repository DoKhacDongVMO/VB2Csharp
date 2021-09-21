/// <summary>
/// 振込口座マスタのDAクラス
/// </summary>
using System;
using System.Collections;

public partial class HurikomiKoza_DA : DataAccessorBase
{

    #region  定数／変数 

    private MHurikomiKozaEntity hurikomiKoza = new MHurikomiKozaEntity();

    public enum accessType : int
    {
        getHuriKomiKoza                      // 一覧結果取得検索
    }

    #endregion

    #region  SELECT処理 

    /// <summary>
    /// SELECT用DBアクセス
    /// </summary>
    /// <param name="type"></param>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable accessHorikomiKoza(accessType type, Hashtable paramInfoList = null)
    {
        // SQL文字列
        string sqlString = string.Empty;
        // 戻り値
        DataTable returnValue = default;
        switch (type)
        {
            case accessType.getHuriKomiKoza:
                {
                    // 一覧結果取得検索
                    sqlString = getHuriKomiKoza(paramInfoList);
                    break;
                }

            default:
                {
                    // 該当処理なし
                    return returnValue;
                }
        }

        try
        {
            returnValue = getDataTable(sqlString);
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramList"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected string getHuriKomiKoza(Hashtable paramList)
    {
        string strSQL = string.Empty;
        strSQL += " SELECT HURIKOMI_SAKI_BANK_NAME ";
        strSQL += "     ,HURIKOMI_SAKI_KOZA_NAME ";
        strSQL += "     ,HURIKOMI_SAKI_BRANCH_NAME ";
        strSQL += "     ,KOZA_NO ";
        strSQL += "     ,YOKIN_EVENT ";
        strSQL += " FROM M_HURIKOMI_KOZA ";
        strSQL += " WHERE HURIKOMI_SEIKYUSYO_FOR_FLG = 'S' ";
        return strSQL.ToString();
    }
    #endregion

}