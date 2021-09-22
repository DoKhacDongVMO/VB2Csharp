using System;
using System.Collections;
using System.Text;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

public partial class CrsMessageChangeAfter_DA : DataAccessorBase
{

    #region  定数／変数 
    private TYoyakuInfoBasicEntity yoyakuInfoBasicEntity = new TYoyakuInfoBasicEntity();
    private TYoyakuInfoPickupEntity yoyakuInfoPickUpEntity = new TYoyakuInfoPickupEntity();
    private const string notFlg = "";
    #endregion

    /// <summary>
    /// 更新対象の予約情報を取得する
    /// </summary>
    /// <param name="paramInfoList"></param>
    /// <returns></returns>
    public DataTable getChageYoyakuInfo(Hashtable paramInfoList)
    {
        // 戻り値
        DataTable returnValue = default;
        var sb = new StringBuilder();
        {
            var withBlock = yoyakuInfoBasicEntity;


            // SELECT句
            sb.AppendLine(" SELECT ");
            sb.AppendLine("     YIB.YOYAKU_KBN ");
            sb.AppendLine("   , YIB.YOYAKU_NO ");
            sb.AppendLine("   , YIB.USING_FLG");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     T_YOYAKU_INFO_BASIC YIB ");
            sb.AppendLine(" WHERE 1=1 ");
            sb.AppendLine("    AND NVL(YIB.DELETE_DAY, 0) = 0");
            // sb.AppendLine("    AND NVL(YIB.CANCEL_FLG, 0) = 0")
            sb.AppendLine("    AND YIB.SYSTEM_UPDATE_PERSON_CD != 'BAT_USER'");
            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["crsCd"])))
            {
                sb.AppendLine("    AND YIB.CRS_CD = " + setParam("crsCd", Conversions.ToString(paramInfoList["crsCd"]), withBlock.CrsCd.DBType, withBlock.CrsCd.IntegerBu, withBlock.CrsCd.DecimalBu));
            }

            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["gousya"])))
            {
                sb.AppendLine("    AND YIB.GOUSYA = " + setParam("gousya", Conversions.ToString(paramInfoList["gousya"]), withBlock.Gousya.DBType, withBlock.Gousya.IntegerBu, withBlock.Gousya.DecimalBu));
            }

            if (!string.IsNullOrEmpty(Conversions.ToString(paramInfoList["syuptDay"])))
            {
                sb.AppendLine("    AND YIB.SYUPT_DAY = " + setParam("syuptDay", Conversions.ToString(paramInfoList["syuptDay"]), withBlock.SyuptDay.DBType, withBlock.SyuptDay.IntegerBu, withBlock.SyuptDay.DecimalBu));
            }
        }

        try
        {
            returnValue = getDataTable(sb.ToString());
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// 予約情報(ピックアップ）更新
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約No</param>
    /// <returns></returns>
    public bool updateYoyakuPickUpInfo(string yoyakuKbn, string yoyakuNo, OracleTransaction oracleTransaction)
    {

        // Dim oracleTransaction As OracleTransaction = Nothing
        int updateCount = 0;

        // 戻り値
        DataTable returnValue = default;
        var sqlString = new StringBuilder();
        try
        {
            // トランザクション開始
            // oracleTransaction = MyBase.callBeginTransaction()

            // update
            {
                var withBlock = yoyakuInfoPickUpEntity;
                sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_PICKUP YIP");
                sqlString.AppendLine("    SET YIP.ZUMI_FLG = '' ");
                sqlString.AppendLine(" WHERE YIP.YOYAKU_KBN = " + setParam("yoyakuKbn", yoyakuKbn, withBlock.YoyakuKbn.DBType, withBlock.YoyakuKbn.IntegerBu, withBlock.YoyakuKbn.DecimalBu));
                sqlString.AppendLine(" AND YIP.YOYAKU_NO = " + setParam("yoyakuNo", yoyakuNo, withBlock.YoyakuNo.DBType, withBlock.YoyakuNo.IntegerBu, withBlock.YoyakuNo.DecimalBu));
            }

            updateCount = base.execNonQuery(oracleTransaction, sqlString.ToString());
        }

        // If updateCount <= 0 Then

        // ロールバック、コミットはコースメッセージの更新成功と同時に行う
        // ロールバック
        // MyBase.callRollbackTransaction(oracleTransaction)
        // Return False
        // End If
        // コミット
        // MyBase.callCommitTransaction(oracleTransaction)
        catch (Exception ex)
        {

            // ロールバック
            // MyBase.callRollbackTransaction(oracleTransaction)
            throw;
        }

        return true;
    }


    /// <summary>
    /// 予約情報(ピックアップ）更新
    /// </summary>
    /// <param name="yoyakuKbn">予約区分</param>
    /// <param name="yoyakuNo">予約No</param>
    /// <param name="gyouNo">行No</param>
    /// <returns></returns>
    public bool updateYoyakuBasicInfo(string yoyakuKbn, string yoyakuNo, int gyouNo, OracleTransaction oracleTransaction)
    {

        // Dim oracleTransaction As OracleTransaction = Nothing
        int updateCount = 0;
        var sqlString = new StringBuilder();
        try
        {
            // トランザクション開始
            // oracleTransaction = MyBase.callBeginTransaction()

            // update
            {
                var withBlock = yoyakuInfoBasicEntity;
                sqlString.AppendLine(" UPDATE T_YOYAKU_INFO_BASIC ");
                if (gyouNo == 1)
                {
                    // メッセージチェックフラグ１を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_1 = '' ");
                }
                else if (gyouNo == 2)
                {
                    // メッセージチェックフラグ２を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_2 = '' ");
                }
                else if (gyouNo == 3)
                {
                    // メッセージチェックフラグ３を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_3 = '' ");
                }
                else if (gyouNo == 4)
                {
                    // メッセージチェックフラグ４を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_4 = '' ");
                }
                else if (gyouNo == 5)
                {
                    // メッセージチェックフラグ５を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_5 = '' ");
                }
                else if (gyouNo == 6)
                {
                    // メッセージチェックフラグ６を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_6 = '' ");
                }
                else if (gyouNo == 7)
                {
                    // メッセージチェックフラグ７を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_7 = '' ");
                }
                else if (gyouNo == 8)
                {
                    // メッセージチェックフラグ８を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_8 = '' ");
                }
                else if (gyouNo == 9)
                {
                    // メッセージチェックフラグ９を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_9 = '' ");
                }
                else if (gyouNo == 10)
                {
                    // メッセージチェックフラグ10を更新
                    sqlString.AppendLine("    SET MESSAGE_CHECK_FLG_10 = '' ");
                }
                else
                {
                    // 失敗
                }

                sqlString.AppendLine(" WHERE YOYAKU_KBN = " + setParam("yoyakuKbn", yoyakuKbn, withBlock.YoyakuKbn.DBType, withBlock.YoyakuKbn.IntegerBu, withBlock.YoyakuKbn.DecimalBu));
                sqlString.AppendLine(" AND YOYAKU_NO = " + setParam("yoyakuNo", yoyakuNo, withBlock.YoyakuNo.DBType, withBlock.YoyakuNo.IntegerBu, withBlock.YoyakuNo.DecimalBu));
            }

            updateCount = base.execNonQuery(oracleTransaction, sqlString.ToString());


            // ロールバック、コミットはコースメッセージの更新成功と同時に行う
            if (updateCount <= 0)
            {

                // ロールバック
                // MyBase.callRollbackTransaction(oracleTransaction)
                return false;
            }
        }
        // コミット
        // MyBase.callCommitTransaction(oracleTransaction)
        catch (Exception ex)
        {

            // ロールバック
            // MyBase.callRollbackTransaction(oracleTransaction)
            throw;
        }

        return true;
    }
}