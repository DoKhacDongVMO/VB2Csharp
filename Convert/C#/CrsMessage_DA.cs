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
	/// コース台帳一括修正（メッセージ）のDAクラス
	/// </summary>
	public class CrsMessage_DA : DataAccessorBase
	{
#region  定数／変数 

		private string Edaban = "※"; //枝番判定

		public enum accessType: int
		{
			getCrsMessage, //一覧結果取得検索
			executeInsertCrsMessage, //登録
			executeUpdateCrsMessage, //更新
			executeDeleteCrsMessage //削除
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
		public DataTable accessCrsMessageTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCrsMessage:
					//一覧結果取得検索
					sqlString = getCrsMessage(paramInfoList);
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
		protected string getCrsMessage(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();
			paramClear();

			//SELECT句
			sqlString.AppendLine("SELECT ");
			sqlString.AppendLine("SYUPT_DAY "); //出発日
			sqlString.AppendLine(",CRS_CD "); //コースコード
			sqlString.AppendLine(",YOBI_CD "); //曜日コード
			sqlString.AppendLine(",HAISYA_KEIYU_CD_1 "); //乗車地コード
			sqlString.AppendLine(",SYUPT_TIME_1 "); //出発時間
			sqlString.AppendLine(",GOUSYA "); //号車
			sqlString.AppendLine(",UNKYU_KBN "); //運休区分
			sqlString.AppendLine(",SAIKOU_KAKUTEI_KBN "); //催行確定区分
			sqlString.AppendLine(",TEIKI_KIKAKU_KBN "); //定期・企画区分
			sqlString.AppendLine(",TO_CHAR(LINE_NO) AS LINE_NO "); //行ＮＯ
			sqlString.AppendLine(",CRS_MESSAGE "); //コード
			sqlString.AppendLine(",MESSAGE_KBN "); //注意事項区分/メッセージ区分
			sqlString.AppendLine(",MESSAGE "); //注意事項/メッセージ
			sqlString.AppendLine(",EIBUN_MESSAGE "); //英文メッセージ
			sqlString.AppendLine(",DELETE_DAY "); //削除日
			sqlString.AppendLine(",SYSTEM_ENTRY_DAY "); //システム登録日
			sqlString.AppendLine(",SYSTEM_ENTRY_PERSON_CD "); //システム登録者コード
			sqlString.AppendLine(",SYSTEM_ENTRY_PGMID "); //システム登録ＰＧＭＩＤ
			sqlString.AppendLine(",SYSTEM_UPDATE_DAY "); //システム更新日
			sqlString.AppendLine(",SYSTEM_UPDATE_PERSON_CD "); //システム更新者コード
			sqlString.AppendLine(",SYSTEM_UPDATE_PGMID "); //システム更新ＰＧＭＩＤ
			sqlString.AppendLine(",SYUPT_DAY2 "); //出発日2
			sqlString.AppendLine(",HAISYA_KEIYU_CD_12 "); //乗車地コード2
			sqlString.AppendLine(",GOUSYA2 "); //号車2
			sqlString.AppendLine(",SYUPT_TIME_12 "); //出発時間2
			sqlString.AppendLine(",USING_FLG "); //使用中フラグ
			sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN "); //変更可否区分
			sqlString.AppendLine(",' ' AS UPDATE_KBN "); //更新区分
			sqlString.AppendLine("FROM( ");
			sqlString.AppendLine("SELECT TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') SYUPT_DAY "); //出発日
			sqlString.AppendLine(",BASIC.CRS_CD "); //コースコード
			sqlString.AppendLine(",CODE_Y.CODE_NAME AS YOBI_CD "); //曜日コード
			sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS HAISYA_KEIYU_CD_1 "); //乗車地コード
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME_1 "); //出発時間
			sqlString.AppendLine(",BASIC.GOUSYA "); //号車
			sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN "); //運休区分
			sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN "); //催行確定区分
			sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN "); //定期・企画区分

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
