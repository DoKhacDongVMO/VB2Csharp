﻿using System.Text;

/// <summary>
/// P02_0601 DA
/// </summary>
public partial class P02_0601Da : DataAccessorBase
{

    #region  定数／変数 

    #endregion

    #region メソッド

    /// <summary>
    /// ご乗車案内（予約確認書）用ピックアップ情報取得SQL
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約No</param>
    /// <returns></returns>
    public DataTable getP02_0601Info(string yoyakuKbn, int yoyakuNo)
    {
        base.paramClear();
        var ent = new YoyakuInfoBasicEntity();
        var sb = new StringBuilder();

        // パラメータの取得
        string prmYoyakuKbn = base.setParam(ent.yoyakuKbn.PhysicsName, yoyakuKbn, ent.yoyakuKbn.DBType, ent.yoyakuKbn.IntegerBu);
        string prmYoyakuNo = base.setParam(ent.yoyakuNo.PhysicsName, yoyakuNo, ent.yoyakuNo.DBType, ent.yoyakuNo.IntegerBu);
        try
        {
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     YB.YOYAKU_KBN ");
            sb.AppendLine("     , YB.YOYAKU_NO ");
            sb.AppendLine("     , NVL(PH.HOTEL_NAME_JYOSYA_TI_ALPH,' ') AS HOTEL_NAME_JYOSYA_TI_ALPH ");
            sb.AppendLine("     , NVL(PH.SYUGO_PLACE_ALPH,' ') AS SYUGO_PLACE_ALPH ");
            sb.AppendLine("     , NVL(TPRLH.SYUPT_TIME,0) AS PICKUP_SYUPT_TIME ");
            sb.AppendLine("     , MP.PLACE_NAME_1 ");
            sb.AppendLine("     , MP.PLACE_NAME_2 ");
            sb.AppendLine(" FROM  ");
            sb.AppendLine("     T_YOYAKU_INFO_BASIC YB ");
            sb.AppendLine("     LEFT OUTER JOIN T_YOYAKU_INFO_PICKUP YP ");                 // 予約情報（ピックアップ）
            sb.AppendLine("         ON  YP.YOYAKU_KBN = YB.YOYAKU_KBN ");
            sb.AppendLine("         AND YP.YOYAKU_NO = YB.YOYAKU_NO ");
            sb.AppendLine("     LEFT OUTER JOIN M_PICKUP_HOTEL PH ");                       // ピックアップホテルマスタ
            sb.AppendLine("         ON PH.PICKUP_HOTEL_CD = YP.PICKUP_HOTEL_CD ");
            sb.AppendLine("     LEFT OUTER JOIN T_PICKUP_ROUTE_LEDGER_HOTEL TPRLH ");       // ピックアップルート台帳（ホテル）
            sb.AppendLine("         ON   TPRLH.SYUPT_DAY = YB.SYUPT_DAY ");
            sb.AppendLine("         AND  TPRLH.PICKUP_ROUTE_CD = YP.PICKUP_ROUTE_CD ");
            sb.AppendLine("         AND  TPRLH.PICKUP_HOTEL_CD = YP.PICKUP_HOTEL_CD ");
            sb.AppendLine("     LEFT OUTER JOIN M_PLACE MP");                                // 場所マスタ
            sb.AppendLine("         ON  YB.JYOCHACHI_CD_1 = MP.PLACE_CD");
            sb.AppendLine(" WHERE 1=1");
            sb.AppendLine($"     AND YB.YOYAKU_KBN = {prmYoyakuKbn}");
            sb.AppendLine($"     AND YB.YOYAKU_NO  = {prmYoyakuNo}");
            return base.getDataTable(sb.ToString());
        }
        catch
        {
            throw;
        }
    }

    #endregion

}