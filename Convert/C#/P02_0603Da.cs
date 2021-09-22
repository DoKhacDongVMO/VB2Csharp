using System.Text;

/// <summary>
/// P02_0603 DA
/// </summary>
public partial class P02_0603Da : DataAccessorBase
{

    #region  定数／変数 

    #endregion

    #region メソッド

    /// <summary>
    /// 領収書(営業所名)取得用SQL
    /// </summary>
    /// <param name="companyCd">会社コード</param>
    /// <param name="eigyosyoCd">営業所コード</param>
    /// <returns></returns>
    public DataTable getP02_0601EigyosyoName(string companyCd, string eigyosyoCd)
    {
        base.paramClear();

        // 必須条件チェック
        if (string.IsNullOrWhiteSpace(companyCd))
        {
            return default;
        }

        if (string.IsNullOrWhiteSpace(eigyosyoCd))
        {
            return default;
        }

        var ent = new EigyosyoMasterEntity();
        var sb = new StringBuilder();

        // パラメータの取得
        string paramCompanyCd = base.setParam(ent.CompanyCd.PhysicsName, companyCd, ent.CompanyCd.DBType, ent.CompanyCd.IntegerBu);
        string paramEigyosyoCd = base.setParam(ent.EigyosyoCd.PhysicsName, eigyosyoCd, ent.EigyosyoCd.DBType, ent.EigyosyoCd.IntegerBu);
        try
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     EIGYOSYO_NAME_1 ");
            sb.AppendLine(" FROM  ");
            sb.AppendLine("     M_EIGYOSYO ");
            sb.AppendLine(" WHERE 1=1");
            sb.AppendLine($"     AND COMPANY_CD = {paramCompanyCd} ");
            sb.AppendLine($"     AND EIGYOSYO_CD  = {paramEigyosyoCd} ");
            return base.getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }
    #endregion

}