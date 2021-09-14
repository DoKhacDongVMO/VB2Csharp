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
	/// コース一覧照会（増発）のDAクラス
	/// </summary>
	public class CrsItiranZouhatsu_DA : Hatobus.ReservationManagementSystem.Common.DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getCrsItiranZouhatsu //一覧結果取得検索
		}

		private const string YSet = "Y";

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessCrsItiranZouhatsu(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCrsItiranZouhatsu:
					//一覧結果取得検索
					sqlString = getCrsItiranZouhatsu(paramInfoList);
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
		protected string getCrsItiranZouhatsu(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();

			sqlString.AppendLine(" SELECT");
			sqlString.AppendLine(" " + CommonKyushuUtil.setSQLDateFormat("SYUPT_DAY", ((int)FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD).ToString(), "CRS") + " AS SYUPT_DAY"); // 出発日
			sqlString.AppendLine(" , CRS.CRS_CD "); // コースコード
			sqlString.AppendLine(" , CRS.CRS_NAME "); // コース名
			sqlString.AppendLine(" , CRS.GOUSYA "); // 号車
			sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_1 "); // 配車経由コード1
			sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_2 "); // 配車経由コード2
			sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_3 "); // 配車経由コード3
			sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_4 "); // 配車経由コード4
			sqlString.AppendLine(" , CRS.HAISYA_KEIYU_CD_5 "); // 配車経由コード5
			sqlString.AppendLine(" , PLC1.PLACE_NAME_1 "); // 場所名1
			sqlString.AppendLine(" , PLC2.PLACE_NAME_1 AS PLACE_NAME_2 "); // 場所名2
			sqlString.AppendLine(" , PLC3.PLACE_NAME_1 AS PLACE_NAME_3 "); // 場所名3
			sqlString.AppendLine(" , PLC4.PLACE_NAME_1 AS PLACE_NAME_4 "); // 場所名4
			sqlString.AppendLine(" , PLC5.PLACE_NAME_1 AS PLACE_NAME_5 "); // 場所名5
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_1") + " AS SYUPT_TIME_1 "); // 出発時間1
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_2") + " AS SYUPT_TIME_2 "); // 出発時間2
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_3") + " AS SYUPT_TIME_3 "); // 出発時間3
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_4") + " AS SYUPT_TIME_4 "); // 出発時間4
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLTime24Format("CRS.SYUPT_TIME_5") + " AS SYUPT_TIME_5 "); // 出発時間5
			sqlString.AppendLine(" , CRS.UNKYU_KBN "); // 運休区分
			sqlString.AppendLine(" , CD1.CODE_NAME AS UNKYU_KBN_NAME "); //運休区分(名称)
			sqlString.AppendLine(" , CD2.CODE_NAME AS SAIKOU_KAKUTEI_KBN_NAME "); //催行確定区分(名称)
			sqlString.AppendLine(" , CRS.TEJIMAI_KBN "); // 手仕舞区分
			sqlString.AppendLine(" , CRS.KUSEKI_NUM_TEISEKI + CRS.KUSEKI_NUM_SUB_SEAT AS KUSEKI_NUM_TEISEKI "); // 空席数
			sqlString.AppendLine(" , CRS.YOYAKU_NUM_TEISEKI + CRS.YOYAKU_NUM_SUB_SEAT AS YOYAKU_NUM_TEISEKI "); // 予約数定席
			sqlString.AppendLine(" , CRS.BLOCK_KAKUHO_NUM + CRS.EI_BLOCK_REGULAR + CRS.EI_BLOCK_HO AS BLOCK_KAKUHO_NUM "); // ブロック確保数
			sqlString.AppendLine(" , CRS.KUSEKI_KAKUHO_NUM "); // 空席確保数
			sqlString.AppendLine(" , CRS.CANCEL_WAIT_NINZU "); // キャンセル待ち人数
			sqlString.AppendLine(" , CRS.ROOM_ZANSU_SOKEI "); // 部屋残数総計
			sqlString.AppendLine(" , CRS.ROOM_ZANSU_ONE_ROOM "); // 部屋残数１人部屋
			sqlString.AppendLine(" , CRS.ROOM_ZANSU_TWO_ROOM "); // 部屋残数２人部屋
			sqlString.AppendLine(" , CRS.ROOM_ZANSU_THREE_ROOM "); // 部屋残数３人部屋
			sqlString.AppendLine(" , CRS.CRS_BLOCK_ONE_1R "); // コースブロック１名１Ｒ
			sqlString.AppendLine(" , CRS.CRS_BLOCK_TWO_1R "); // コースブロック２名１Ｒ
			sqlString.AppendLine(" , CRS.CRS_BLOCK_THREE_1R "); // コースブロック３名１Ｒ
			sqlString.AppendLine(" , CRS.CAR_NO "); // 車番
			sqlString.AppendLine(" , CRS.CAR_TYPE_CD "); // 車種コード
			sqlString.AppendLine(" , CRS.SUB_SEAT_OK_KBN "); // 補助席可区分
			sqlString.AppendLine(" , CRS.USING_FLG "); // 使用中フラグ
			sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_PGMID "); // システム更新PGM-ID
			sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_PERSON_CD "); // システム更新者コード
			sqlString.AppendLine(" , CRS.SYSTEM_UPDATE_DAY "); // システム更新日
			sqlString.AppendLine(" , 0 AS SOSU "); // 総数(加算用)
			sqlString.AppendLine(" , LPAD(' ',6) AS JYOUKYOU "); // 状況
			sqlString.AppendLine(" , CRS.TEIKI_KIKAKU_KBN "); // 定期＿企画区分
			sqlString.AppendLine(" , LPAD(' ',4) AS ZOUHATSU "); // 増発
			sqlString.AppendLine(" , CRS.YOYAKU_STOP_FLG "); // 予約停止フラグ
			sqlString.AppendLine(" , CRS.BUS_RESERVE_CD "); // バス指定コード
			sqlString.AppendLine(" , ZIMG.USING_FLG AS ZIMG_USING_FLG "); // 使用中フラグ
			sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_PGMID AS ZIMG_SYSTEM_UPDATE_PGMID "); // システム更新PGM-ID
			sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_PERSON_CD AS ZIMG_SYSTEM_UPDATE_PERSON_CD "); // システム更新者コード
			sqlString.AppendLine(" , ZIMG.SYSTEM_UPDATE_DAY AS ZIMG_SYSTEM_UPDATE_DAY "); // システム更新日
			sqlString.AppendLine(" , CRS.CAPACITY_REGULAR "); // 定員定
			sqlString.AppendLine(" , CRS.CAPACITY_HO_1KAI "); // 定員補
			sqlString.AppendLine(" , CRS.HOUJIN_GAIKYAKU_KBN "); // 邦人・外客区分
			sqlString.AppendLine(" , CRS.ZOUHATSUMOTO_GOUSYA "); // 増発元号車
			sqlString.AppendLine(" ," + CommonKyushuUtil.setSQLDateFormat("ZOUHATSU_DAY", ((int)FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD).ToString(), "CRS") + " AS ZOUHATSU_DAY "); // 増発日
			sqlString.AppendLine(" , USR.USER_NAME AS ZOUHATSU_ENTRY_PERSON_CD "); // 増発実施者

			// FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("T_CRS_LEDGER_BASIC CRS "); // コース台帳（基本）
			sqlString.AppendLine("INNER JOIN T_ZASEKI_IMAGE ZIMG ");
			sqlString.AppendLine("  ON     CRS.SYUPT_DAY = ZIMG.SYUPT_DAY ");

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
