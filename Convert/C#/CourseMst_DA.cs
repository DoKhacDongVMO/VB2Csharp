using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; 
/// <summary>
/// インターネット予約トランザクション管理）のDAクラス
/// <remarks>
/// Author:2018/10/26//QuangTD
/// </remarks>
/// </summary>
public partial class CourseMst_DA : DataAccessorBase
{

    #region  定数／変数 
    private TCourseMstEntity clsTCourseMstEntity = new TCourseMstEntity();
    #endregion

    /// <summary>
    /// 検索用SELECT
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <remarks></remarks>
    public DataTable getCrsCdExist(Hashtable paramInfoList)
    {
        // 戻り値
        DataTable returnValue = default;
        var sqlString = new StringBuilder();
        {
            var withBlock = clsTCourseMstEntity;
            // SELECT
            sqlString.AppendLine("SELECT");
            sqlString.AppendLine("    CRS_CD ");
            sqlString.AppendLine("FROM");
            sqlString.AppendLine("    T_COURSE_MST ");
            sqlString.AppendLine("WHERE");
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["crsCd"])))
            {
                sqlString.AppendLine("    T_COURSE_MST.CRS_CD = " + setParam("crsCd", Conversions.ToString(paramInfoList["crsCd"]), withBlock.crsCd.DBType, withBlock.crsCd.IntegerBu, withBlock.crsCd.DecimalBu));
            }
        }

        try
        {
            returnValue = getDataTable(sqlString.ToString());
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }
}