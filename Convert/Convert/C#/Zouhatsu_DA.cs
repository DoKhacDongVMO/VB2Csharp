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
	/// 増発DAクラス
	/// </summary>
	public class Zouhatsu_DA : DataAccessorBase
	{
#region  定数／変数 
		public enum accessType: int
		{
			getPlaceMaster, // 場所マスタ検索
			getCarNoMaster, // 車番マスタ検索
			getSameCrsLedgerBasic, // コース台帳（基本）重複データ検索
			getGousya, // 空き番検索
			getZasekiImageCnt // 座席イメージ作成済検索
		}

		private const string maruZou = "○増";
		private const int ERRCNT = -1;

		//コース台帳（基本）エンティティ
		private TCrsLedgerBasicEntity clsCrsLedgerBasicEntity = new TCrsLedgerBasicEntity();

		/// <summary>
		/// 共通設定カラム名
		/// </summary>
		/// <remarks></remarks>
		public sealed class CommonColName
		{
			public const string SystemEntryPgmid = "SYSTEM_ENTRY_PGMID";
			public const string SystemEntryPersonCd = "SYSTEM_ENTRY_PERSON_CD";
			public const string SystemEntryDay = "SYSTEM_ENTRY_DAY";
			public const string SystemUpdatePgmid = "SYSTEM_UPDATE_PGMID";
			public const string SystemUpdatePersonCd = "SYSTEM_UPDATE_PERSON_CD";
			public const string SystemUpdateDay = "SYSTEM_UPDATE_DAY";
			public const string DeleteDay = "DELETE_DAY";
			public const string DeleteDate = "DELETE_DATE";
			public const string Gousya = "GOUSYA";
		}

		/// <summary>
		/// パラメータキー
		/// </summary>
		/// <remarks></remarks>
		public sealed class UpdateParamKeys
		{
			/// <summary>
			/// 定員（定）
			/// </summary>
			public const string CAR_CAPACITY = "CAR_CAPACITY";
			/// <summary>
			/// 定員（補・１階）
			/// </summary>
			public const string CAR_EMG_CAPACITY = "CAR_EMG_CAPACITY";
			/// <summary>
			/// 車種コード
			/// </summary>
			public const string CAR_NO = "CAR_NO";
			/// <summary>
			/// 号車
			/// </summary>
			public const string GOUSYA = "GOUSYA";
			/// <summary>
			/// 車種(入力)
			/// </summary>
			public const string CAR_TYPE_CD = "CAR_TYPE_CD";
			/// <summary>
			/// 車種(基本)
			/// </summary>
			public const string CAR_TYPE_CD_BEF = "CAR_TYPE_CD_BEF";
			/// <summary>
			/// 配車経由地1
			/// </summary>
			public const string HAISYA_KEIYU_CD_1 = "HAISYA_KEIYU_CD_1";
			/// <summary>
			/// 配車経由地2
			/// </summary>
			public const string HAISYA_KEIYU_CD_2 = "HAISYA_KEIYU_CD_2";
			/// <summary>
			/// 配車経由地3
			/// </summary>
			public const string HAISYA_KEIYU_CD_3 = "HAISYA_KEIYU_CD_3";
			/// <summary>
			/// 配車経由地4
			/// </summary>
			public const string HAISYA_KEIYU_CD_4 = "HAISYA_KEIYU_CD_4";
			/// <summary>
			/// 配車経由地5
			/// </summary>
			public const string HAISYA_KEIYU_CD_5 = "HAISYA_KEIYU_CD_5";
			/// <summary>
			/// 出発日
			/// </summary>
			public const string SYUPT_DAY = "SYUPT_DAY";
			/// <summary>
			/// バス指定コード
			/// </summary>
			public const string BUS_RESERVE_CD = "BUS_RESERVE_CD";
			/// <summary>
			/// 出発時間１
			/// </summary>
			public const string SYUPT_TIME_1 = "SYUPT_TIME_1";
			/// <summary>
			/// 出発時間２
			/// </summary>
			public const string SYUPT_TIME_2 = "SYUPT_TIME_2";
			/// <summary>
			/// 出発時間３
			/// </summary>
			public const string SYUPT_TIME_3 = "SYUPT_TIME_3";
			/// <summary>
			/// 出発時間４
			/// </summary>
			public const string SYUPT_TIME_4 = "SYUPT_TIME_4";
			/// <summary>
			/// 出発時間５
			/// </summary>
			public const string SYUPT_TIME_5 = "SYUPT_TIME_5";
			/// <summary>
			/// 集合時間１
			/// </summary>
			public const string SYUGO_TIME_1 = "SYUGO_TIME_1";
			/// <summary>
			/// 集合時間２
			/// </summary>
			public const string SYUGO_TIME_2 = "SYUGO_TIME_2";
			/// <summary>
			/// 集合時間３
			/// </summary>
			public const string SYUGO_TIME_3 = "SYUGO_TIME_3";
			/// <summary>
			/// 集合時間４
			/// </summary>
			public const string SYUGO_TIME_4 = "SYUGO_TIME_4";
			/// <summary>
			/// 集合時間５
			/// </summary>
			public const string SYUGO_TIME_5 = "SYUGO_TIME_5";

		}

		/// <summary>
		/// DataSetテーブルNo
		/// </summary>
		/// <remarks></remarks>
		public enum crsLedgerTblId
		{
			crsLeaderBasic = 0, //コース台帳（基本
			//コース台帳（降車ヶ所）
			crsLeaderKoshakasho,
			//コース台帳（コース情報）
			crsLeaderCrsInfo,
			//コース台帳（メッセージ）
			crsLeaderMessage,
			//コース台帳（料金）
			crsLeaderCharge,
			//コース台帳（ホテル）
			crsLeaderHotel,
			//コース台帳（付加情報）
			crsLeaderAddInfo,
			//コース台帳（バス紐づけ）
			crsLeaderBusHimoduke,
			//コース台帳（オプション）
			crsLeaderOption,
			//コース台帳（ダイヤ）
			crsLeaderDia,
			//コース台帳（基本_料金区分）
			crsLeaderBasicChargeKbn,
			//コース台帳（基本_料金区分）
			crsLeaderChargeChargeKbn,
			//コース台帳（オプショングループ）
			crsLeaderOptionGroup,
			//コース台帳（販売課所）
			crsLeaderKasho,
			//コース台帳（リマークス）
			crsLeaderRemarks,
			//コース台帳原価（基本）
			crsLeaderCostBasic,
			//コース台帳原価（降車ヶ所）
			crsLeaderCostKoshakasho,
			//コース台帳原価（キャリア）
			crsLeaderCostCarrier,
			//コース台帳原価（プレート）
			crsLeaderCostPlate,
			//コース台帳原価（キャリア_料金区分）
			crsLeaderCostCarrierChargeKbn,
			//コース台帳原価（降車ヶ所_料金区分）
			crsLeaderCostKoshakashoChargeKbn,
			//座席イメージ（バス情報）
			zasekiImage,
			//座席イメージ（座席情報）
			zasekiImageInfo,
			//座席番号マスタ
			zasekiNoMst
		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessZouhatsuTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getPlaceMaster:
					// 場所マスタ検索
					sqlString = GetPlaceMaster(paramInfoList);
					break;
				case accessType.getCarNoMaster:
					// 車番マスタ検索
					sqlString = GetCarNoMaster(paramInfoList);
					break;
				case accessType.getSameCrsLedgerBasic:
					// コース台帳（基本）重複データ検索
					sqlString = getSameCrsLedgerBasic(paramInfoList);

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
