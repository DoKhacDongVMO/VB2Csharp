//====================================================================================================
//The Free Edition of Instant C# limits conversion output to 100 lines per file.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================

//INSTANT C# NOTE: Formerly VB project-level imports:
using Hatobus.ReservationManagementSystem.ClientCommon;
using Hatobus.ReservationManagementSystem.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Text;


namespace Hatobus.ReservationManagementSystem.Tehai
{
	/// <summary>
	/// コース台帳一括修正（カテゴリ）のDAクラス
	/// </summary>
	public class Category_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getCategory, //一覧結果取得検索
			executeInsertCategory, //登録
			executeUpdateCategory //更新
			//executeReturnCategory         '戻し
		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessCategoryTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCategory:
					//一覧結果取得検索
					sqlString = getCategory(paramInfoList);
					break;
				default:
					//該当処理なし
					return returnValue;
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
		protected string getCategory(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();
			paramClear();

			//SELECT句
			sqlString.AppendLine(" SELECT ");
			sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY "); //出発日
			sqlString.AppendLine(",BASIC.CRS_CD "); //コースコード
			sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD "); //曜日コード
			sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 "); //乗車地コード
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME_1 "); //出発時間
			sqlString.AppendLine(",BASIC.GOUSYA "); //号車
			sqlString.AppendLine(",BASIC.UNKYU_KBN "); //運休区分
			sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN "); //催行確定区分
			sqlString.AppendLine(",BASIC.CATEGORY_CD_1 "); //カテゴリー1
			sqlString.AppendLine(",BASIC.CATEGORY_CD_2 "); //カテゴリー2
			sqlString.AppendLine(",BASIC.CATEGORY_CD_3 "); //カテゴリー3
			sqlString.AppendLine(",BASIC.CATEGORY_CD_4 "); //カテゴリー4
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY "); //システム更新日
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD "); //システム更新者コード
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID "); //システム更新ＰＧＭＩＤ
			sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG "); //使用中フラグ
			sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN "); //変更可否区分
			sqlString.AppendLine(",' ' AS UPDATE_KBN "); //更新区分
			//FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " + FixedCd.CodeBunrui.yobi + " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " + FixedCd.CodeBunrui.saikou + " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ");
			sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ");
			//WHERE句
			sqlString.AppendLine(" WHERE ");
			sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.HAISYA_KEIYU_CD_1 = " + setParam("HAISYA_KEIYU_CD_1", paramList["HAISYA_KEIYU_CD_1"], OracleDbType.Char));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.GOUSYA = " + setParam("GOUSYA", paramList["GOUSYA"], OracleDbType.Decimal, 3, 0));
			if (!string.IsNullOrEmpty(Convert.ToString(paramList["UNKYU_KBN"])))
			{
				sqlString.AppendLine(" AND ");
				sqlString.AppendLine(" BASIC.UNKYU_KBN = " + setParam("UNKYU_KBN", paramList["UNKYU_KBN"], OracleDbType.Char));
			}
			if (!string.IsNullOrEmpty(Convert.ToString(paramList["SAIKOU_KAKUTEI_KBN"])))
			{
				sqlString.AppendLine(" AND ");
				sqlString.AppendLine(" BASIC.SAIKOU_KAKUTEI_KBN = " + setParam("SAIKOU_KAKUTEI_KBN", paramList["SAIKOU_KAKUTEI_KBN"], OracleDbType.Char));
			}
			if (!string.IsNullOrEmpty(Convert.ToString(paramList["MARU_ZOU_MANAGEMENT_KBN"])))
			{
				sqlString.AppendLine(" AND ");
				sqlString.AppendLine(" BASIC.MARU_ZOU_MANAGEMENT_KBN = " + setParam("MARU_ZOU_MANAGEMENT_KBN", paramList["MARU_ZOU_MANAGEMENT_KBN"], OracleDbType.Char));
			}
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.YOBI_CD = " + setParam("YOBI_CD", paramList["YOBI_CD"], OracleDbType.Char));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ");
			//ORDER BY句
			sqlString.AppendLine(" ORDER BY ");
			sqlString.AppendLine(" BASIC.SYUPT_DAY ");
			sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ");
			sqlString.AppendLine(" ,BASIC.GOUSYA ");

			return sqlString.ToString();
		}
#endregion

#region  UPDATE処理 

		/// <summary>
		/// DB接続用
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public int executeCategoryTehai(accessType type, Hashtable paramInfoList)
		{
			OracleTransaction oracleTransaction = null;
			int returnValue = 0;
			string sqlString = string.Empty;

			try
			{

				//トランザクション開始
				oracleTransaction = callBeginTransaction();

				switch (type)
				{
					case accessType.executeUpdateCategory:
						sqlString = executeUpdateBasicData(paramInfoList);
						//Case accessType.executeReturnCategory
						//    sqlString = executeReturnBasicData(paramInfoList)

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
