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
	/// コース台帳一括修正（車種・台数カウント）のDAクラス
	/// </summary>
	public class CarType_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getCarType, //一覧結果取得検索
			executeCountUpdateCarType, //登録
			executeUpdateCarType, //更新
			executeReturnCarType, //戻し
			getZasekiImage, //座席イメージ（バス情報）取得
			getZasekiImageInfo, //座席イメージ（座席情報）取得
			getLegBasic, //コース台帳（基本）取得
			getBusReserveCrsLedger, //同一バス指定コードデータ取得
			getCarTypeMst //車種マスタ取得
		}

		/// <summary>
		/// パラメータキー
		/// </summary>
		/// <remarks></remarks>
		public sealed class ParamHashKeys
		{
			/// <summary>
			/// バス指定(KEY)
			/// </summary>
			public const string BASIC_KEYS = "BASIC_KEYS";
			/// <summary>
			/// コースコード(KEY)
			/// </summary>
			public const string CRS_CD = "CRS_CD";

		}

		private TehaiCommon comTehai = new TehaiCommon();
#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessCarTypeTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCarType:
					//一覧結果取得検索
					sqlString = getCarType(paramInfoList);
					break;
				case accessType.getZasekiImage:
					sqlString = getZasekiImage(paramInfoList);
					break;
				case accessType.getZasekiImageInfo:
					sqlString = getZasekiImageInfo(paramInfoList);
					break;
				case accessType.getLegBasic:
					sqlString = getLegBasic(paramInfoList);
					break;
				case accessType.getBusReserveCrsLedger:
					sqlString = getBusReserveCrsLedger(paramInfoList);
					break;
				case accessType.getCarTypeMst:
					//一覧結果取得検索
					sqlString = getMstCarType();
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
		protected string getCarType(Hashtable paramList)
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
			sqlString.AppendLine(",CODE_U.CODE_NAME AS UNKYU_KBN "); //運休区分
			sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN "); //催行確定区分
			sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN "); //定期・企画区分
			sqlString.AppendLine(",NVL(BASIC.BUS_RESERVE_CD,'') AS BUS_RESERVE_CD "); //バス指定コード
			sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,'0') AS JYOSYA_CAPACITY "); //乗車定員
			sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,'0') AS YOYAKU_NUM_TEISEKI "); //予約数定席
			sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,'0') AS YOYAKU_NUM_SUB_SEAT "); //予約数補助席
			sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_REGULAR,'0') AS EI_BLOCK_REGULAR "); //営ブロック定
			sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_HO,'0') AS EI_BLOCK_HO "); //営ブロック補
			sqlString.AppendLine(",NVL(BASIC.KUSEKI_KAKUHO_NUM,'0') AS KUSEKI_KAKUHO_NUM "); //空席確保数
			sqlString.AppendLine(",NVL(BASIC.BLOCK_KAKUHO_NUM,'0') AS BLOCK_KAKUHO_NUM "); //ブロック確保数
			sqlString.AppendLine(",NVL(BASIC.SUB_SEAT_OK_KBN,'') AS SUB_SEAT_OK_KBN "); //補助席可区分
			sqlString.AppendLine(",NVL(BASIC.CAR_NO,'') AS CAR_NO "); //車番
			sqlString.AppendLine(",NVL(BASIC.CAR_TYPE_CD,'') AS CAR_TYPE_CD "); //車種コード
			sqlString.AppendLine(",NVL(BASIC.BUS_COUNT_FLG,'0') AS BUS_COUNT_FLG "); //台数カウントフラグ
			sqlString.AppendLine(",NVL(ZASEKIIMAGE.KUSEKI_KAKUHO_NUM,0) AS ZASEKI_KUSEKI_KAKUHO_NUM "); //空席確保数
			sqlString.AppendLine(",NVL(ZASEKIIMAGE.BLOCK_KAKUHO_NUM,0) AS ZASEKI_BLOCK_KAKUHO_NUM "); //ブロック確保数
			sqlString.AppendLine(",NVL(ZASEKIIMAGE.SUB_SEAT_OK_KBN,'') AS ZASEKI_SUB_SEAT_OK_KBN "); //補助席可区分
			sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_REGULAR "); //配列状態定
			sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_HO "); //配列状態補
			sqlString.AppendLine(",'' AS ZASEKI_ARRAY_STATE_1KAI "); //配列状態1F
			sqlString.AppendLine(",NVL(CARKIND.CAR_CAPACITY,'') AS CAR_CAPACITY "); //定員（定）
			sqlString.AppendLine(",NVL(CARKIND.CAR_EMG_CAPACITY,'') AS CAR_EMG_CAPACITY "); //定員（補・１階）
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY "); //システム更新日
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD "); //システム更新者コード
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID "); //システム更新ＰＧＭＩＤ
			sqlString.AppendLine(",NVL(BASIC.CAPACITY_REGULAR,'') AS CAPACITY_REGULAR "); //定員定
			sqlString.AppendLine(",NVL(BASIC.CAPACITY_HO_1KAI,'') AS CAPACITY_HO_1KAI "); //定員補１階

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
