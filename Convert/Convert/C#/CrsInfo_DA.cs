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
	/// コース台帳一括修正（コース情報）のDAクラス
	/// </summary>
	public class CrsInfo_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getCrsInfo, //一覧結果取得検索
			executeInsertCrsInfo, //登録
			executeUpdateCrsInfo, //更新
			executeDeleteCrsInfo //削除
		}

#endregion

#region  マスタテーブル読み込み 

		/// <summary>
		/// コードマスタより名称データを取得する
		/// </summary>
		/// <param name="cdBunrui"></param>
		/// <param name="cdValue"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string GetCdMaster(FixedCd.CdBunruiType cdBunrui, string cdValue)
		{

			DataTable resultDataTable = new DataTable(); //resultDataTable
			StringBuilder sqlString = new StringBuilder();
			string retStr = null; //retStr

			try
			{
				sqlString.AppendLine("SELECT CODE_NAME FROM M_CODE");
				sqlString.AppendLine(" WHERE");
				sqlString.AppendLine(" CODE_BUNRUI = " + setParam("CODE_BUNRUI", CommonType_MojiColValue.CdBunruiType_Value(cdBunrui), OracleDbType.Varchar2, 3));
				sqlString.AppendLine(" AND CODE_VALUE = " + setParam("CODE_VALUE", cdValue, OracleDbType.Varchar2, 15));
				sqlString.AppendLine(" AND DELETE_DATE IS NULL");

				resultDataTable = base.getDataTable(sqlString.ToString());
			}
			catch (Exception ex)
			{
				throw;
			}

			retStr = "";
			if (resultDataTable.Rows.Count > 0)
			{
				if (Convert.IsDBNull(resultDataTable.Rows[0][0]) == false)
				{
					retStr = (resultDataTable.Rows[0][0] == null ? null : Convert.ToString(resultDataTable.Rows[0][0]));
				}
			}

			return retStr;

		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessCrsInfoTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCrsInfo:
					//一覧結果取得検索
					sqlString = getCrsInfo(paramInfoList);
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
		protected string getCrsInfo(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();
			paramClear();

			//SELECT句
			sqlString.AppendLine(" SELECT ");
			sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY "); //出発日
			sqlString.AppendLine(",BASIC.CRS_CD "); //コースコード
			sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD "); //曜日コード
			sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 "); //乗車地コード
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME_1 "); //出発時間
			sqlString.AppendLine(",BASIC.GOUSYA "); //号車
			sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN "); //運休区分
			sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN "); //催行確定区分
			sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN "); //定期・企画区分
			sqlString.AppendLine(",NVL(INFO.LINE_NO,'1') AS LINE_NO "); //行ＮＯ
			sqlString.AppendLine(",'' AS CRS_INFO "); //コード
			sqlString.AppendLine(",NVL(INFO.CRS_JOKEN,'') AS CRS_JOKEN "); //コース条件
			sqlString.AppendLine(",NVL(INFO.DELETE_DAY,0) AS DELETE_DAY "); //削除日
			sqlString.AppendLine(",NVL(INFO.SYSTEM_ENTRY_DAY,'') AS SYSTEM_ENTRY_DAY "); //コース情報_システム登録日
			sqlString.AppendLine(",NVL(INFO.SYSTEM_ENTRY_PERSON_CD,'') AS SYSTEM_ENTRY_PERSON_CD "); //コース情報_システム登録者コード
			sqlString.AppendLine(",NVL(INFO.SYSTEM_ENTRY_PGMID,'') AS SYSTEM_ENTRY_PGMID "); //コース情報_システム登録ＰＧＭＩＤ
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY "); //システム更新日
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD "); //システム更新者コード
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID "); //システム更新ＰＧＭＩＤ
			sqlString.AppendLine(",TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY2 "); //出発日2
			sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_12 "); //乗車地コード2
			sqlString.AppendLine(",BASIC.GOUSYA AS GOUSYA2"); //号車2
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME_12 "); //出発時間2
			sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG "); //使用中フラグ
			sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN "); //変更可否区分
			sqlString.AppendLine(",' ' AS UPDATE_KBN "); //更新区分
			//FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_Y ON CODE_Y.CODE_BUNRUI = " + FixedCd.CodeBunrui.yobi + " AND BASIC.YOBI_CD = CODE_Y.CODE_VALUE ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_U ON CODE_U.CODE_BUNRUI = " + FixedCd.CodeBunrui.unkyu + " AND BASIC.UNKYU_KBN = CODE_U.CODE_VALUE ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = " + FixedCd.CodeBunrui.saikou + " AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ");
			sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ");
			sqlString.AppendLine("LEFT JOIN T_CRS_LEDGER_CRS_INFO INFO ON BASIC.CRS_CD = INFO.CRS_CD AND BASIC.GOUSYA = INFO.GOUSYA AND BASIC.SYUPT_DAY = INFO.SYUPT_DAY");
			//WHERE句
			sqlString.AppendLine(" WHERE ");
			sqlString.AppendLine(" BASIC.SYUPT_DAY = " + setParam("SYUPT_DAY", paramList["SYUPT_DAY"], OracleDbType.Decimal, 8, 0));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.CRS_CD = " + setParam("CRS_CD", paramList["CRS_CD"], OracleDbType.Char));
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.HAISYA_KEIYU_CD_1 = " + setParam("HAISYA_KEIYU_CD_1", paramList["HAISYA_KEIYU_CD_1"], OracleDbType.Char));

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
