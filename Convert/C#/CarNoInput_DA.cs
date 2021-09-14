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
	/// 車番入力のDAクラス
	/// </summary>
	public class CarNoInput_DA : DataAccessorBase
	{
#region  定数／変数 
		// バス会社コード（＝はとバス）
		private const string SetHatobuscd = "400100";

		// サンライズピック
		private const string SetSunrize = "X";

		// コース台帳（基本）エンティティ
		private CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

		// 予約情報（基本）エンティティ
		private YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

		public enum accessType: int
		{
			getCarNoInput, // 一覧結果取得検索
			getYoyakuCount, // 予約情報（基本） 件数取得
			getBusReserveCount, // バス指定コード 件数取得
			getReturnCrs, // コース台帳（基本） 退避
			getReturnyoyaku, // 予約情報（基本） 退避
			getCarNoMaster, // 車番・車種取得検索
			getKyosaiUnkouKbn, // 共催運行区分検索
			getLegBasic // コース台帳（基本）取得
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
		public DataTable accessCarNoInput(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getCarNoInput:
					// 一覧結果取得検索
					sqlString = getCarNoInput(paramInfoList);
					break;
				case accessType.getYoyakuCount:
					// 予約情報（基本） 件数取得
					sqlString = getYoyakuCount(paramInfoList);
					break;
				case accessType.getBusReserveCount:
					// バス指定コード 件数取得
					sqlString = getBusReserveCount(paramInfoList);
					break;
				case accessType.getReturnCrs:
					// コース台帳（基本） 退避
					sqlString = getReturnCrs(paramInfoList);
					break;
				case accessType.getReturnyoyaku:
					// 予約情報（基本） 退避
					sqlString = getReturnyoyaku(paramInfoList);
					break;
				case accessType.getCarNoMaster:
					// 車番・車種取得検索
					sqlString = getCarNoMaster(paramInfoList);
					break;
				case accessType.getKyosaiUnkouKbn:
					// 共催運行区分検索
					sqlString = getKyosaiUnkouKbn(paramInfoList);
					break;
				case accessType.getLegBasic:
					// コース台帳（基本）取得
					sqlString = getLegBasic(paramInfoList);
					break;
				default:
					// 該当処理なし
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
		/// 検索用SELECT（一覧結果を取得する）
		/// </summary>
		/// <param name="paramList"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected string getCarNoInput(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();

			bool whereFlg = true;

			try
			{

				paramClear();


				sqlString.AppendLine(" SELECT ");
				sqlString.AppendLine(" crs.CRS_CD "); // コースコード
				sqlString.AppendLine(",crs.CRS_NAME "); // コース名
				sqlString.AppendLine(",place.PLACE_NAME_1 As PLACE_NAME_SHORT"); // 場所名1
				sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("crs.SYUPT_TIME_1") + " AS SYUPT_TIME_1 "); // 出発時間１
				sqlString.AppendLine(",crs.GOUSYA "); // 号車
				sqlString.AppendLine(",crs.CAR_NO "); // 車番
				sqlString.AppendLine(",crs.YOYAKU_NUM_TEISEKI "); // 予約数定席
				sqlString.AppendLine(",crs.YOYAKU_NUM_SUB_SEAT "); // 予約数補助席
				sqlString.AppendLine(",crs.CAPACITY_REGULAR "); // 定員定
				sqlString.AppendLine(",crs.CAPACITY_HO_1KAI "); // 定員補１階
				sqlString.AppendLine(",crs.CAR_TYPE_CD_YOTEI "); // 車種コード予定
				sqlString.AppendLine(",crs.CAR_TYPE_CD "); // 車種コード
				sqlString.AppendLine(",LPAD(' ', 256) AS MESSAGE "); // メッセージ
				sqlString.AppendLine(",crs.SYUPT_DAY "); // 出発日
				sqlString.AppendLine(",crs.BUS_RESERVE_CD "); // バス指定コード
				sqlString.AppendLine(",crs.ZASEKI_RESERVE_KBN "); // 座席指定区分
				sqlString.AppendLine(",crs.SUB_SEAT_OK_KBN "); // 補助席可区分
				sqlString.AppendLine(",crs.KYOSAI_UNKOU_KBN "); // 共催運行区分
				sqlString.AppendLine(",crs.EI_BLOCK_REGULAR "); // 営ブロック定
				sqlString.AppendLine(",crs.EI_BLOCK_HO "); // 営ブロック補
				sqlString.AppendLine(",crs.KUSEKI_KAKUHO_NUM "); // 空席確保数
				sqlString.AppendLine(",crs.BLOCK_KAKUHO_NUM "); // ブロック確保数
				sqlString.AppendLine(",crs.JYOSYA_CAPACITY "); // 乗車定員
				sqlString.AppendLine(",LPAD(' ', 1) AS USING_FLG "); // 使用中フラグ
				sqlString.AppendLine(",crs.KUSEKI_NUM_TEISEKI "); // 空席数定席
				sqlString.AppendLine(",crs.KUSEKI_NUM_SUB_SEAT "); // 空席数補助席
				sqlString.AppendLine(",crs.YOYAKU_KANOU_NUM "); // 予約可能数

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
