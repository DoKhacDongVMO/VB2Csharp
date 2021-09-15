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
	/// コース台帳一括修正（バス指定・乗車定員・受付限定人数）のDAクラス
	/// </summary>
	public class BusReserveCd_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getBusReserveCd, //一覧結果取得検索
			executeInsertBusReserveCd, //登録
			executeUpdateBusReserveCd, //更新（バス指定コード変更時）
			//executeReturnBusReserveCd               '戻し
			getCrsLedgerBasic2, //コース台帳（基本）件数検索 //乗せ合わせチェック
			getCrsLedgerBasic3, //コース台帳（基本）検索
			getCrsLedgerBasic4, //座席イメージ（座席情報）検索
			getNoseawaseZaseki, //座席イメージ（バス情報）検索
			//executeUpdateBusReserveCdZaseki        '更新(座席イメージ_使用フラグ:乗せ合わせ先)
			//executeUpdateBusReserveCdSaki           '更新(コース台帳（基本）:乗せ合わせ先)
			executeUpdateBusReserveCdSakiZaseki, //更新(座席イメージ（バス情報）:乗せ合わせ先)
			//getZasekiImage                         '座席イメージ（バス情報）取得
			getZasekiImageInfo, //座席イメージ（座席情報）取得
			executeUpdateJosyaChange, //乗車定員更新
			//executeUpdateBusReserveCdMotoZasekiData '座席イメージ（バス情報）元データ更新
			executeUpdateZasekiInfo, //座席イメージ（座席情報）
			executeUpdateZasekiMotoInfo //座席イメージ（座席情報）元データ
		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessBusReserveCdTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getBusReserveCd:
					//一覧結果取得検索
					sqlString = getBusReserveCd(paramInfoList);
					break;
				case accessType.getCrsLedgerBasic2:
					sqlString = getCrsLedgerBasic2(paramInfoList);
					break;
				case accessType.getCrsLedgerBasic3:
					sqlString = getCrsLedgerBasic3(paramInfoList);
					break;
				case accessType.getCrsLedgerBasic4:
					sqlString = getCrsLedgerBasic4(paramInfoList);
					break;
				case accessType.getNoseawaseZaseki:
					sqlString = getNoseawaseZaseki(paramInfoList);
				//Case accessType.getZasekiImage
				//    sqlString = getZasekiImage(paramInfoList)
					break;
				case accessType.getZasekiImageInfo:
					sqlString = getZasekiImageInfo(paramInfoList);
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
		protected string getBusReserveCd(Hashtable paramList)
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
			sqlString.AppendLine(",NVL(BASIC.UKETUKE_GENTEI_NINZU,'0') AS UKETUKE_GENTEI_NINZU "); //受付限定人数
			sqlString.AppendLine(",NVL(BASIC.JYOSYA_CAPACITY,'0') AS JYOSYA_CAPACITY "); //乗車定員
			sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_TEISEKI,'0') AS YOYAKU_NUM_TEISEKI "); //予約数定席
			sqlString.AppendLine(",NVL(BASIC.YOYAKU_NUM_SUB_SEAT,'0') AS YOYAKU_NUM_SUB_SEAT "); //予約数補助席
			sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_REGULAR,'0') AS EI_BLOCK_REGULAR "); //営ブロック定
			sqlString.AppendLine(",NVL(BASIC.EI_BLOCK_HO,'0') AS EI_BLOCK_HO "); //営ブロック補
			sqlString.AppendLine(",NVL(BASIC.KUSEKI_KAKUHO_NUM,'0') AS KUSEKI_KAKUHO_NUM "); //空席確保数
			sqlString.AppendLine(",NVL(BASIC.BLOCK_KAKUHO_NUM,'0') AS BLOCK_KAKUHO_NUM "); //ブロック確保数
			sqlString.AppendLine(",NVL(BASIC.SUB_SEAT_OK_KBN,'') AS SUB_SEAT_OK_KBN "); //補助席可区分
			sqlString.AppendLine(",0 AS TOTAL_SEKI "); //合計席数
			sqlString.AppendLine(",0 AS TOTAL_SUB_SEAT "); //合計補助席数
			sqlString.AppendLine(",NVL(BASIC.CRS_KIND,'') AS CRS_KIND "); //コース種別
			sqlString.AppendLine(",NVL(BASIC.TEIINSEI_FLG,'') AS TEIINSEI_FLG "); //定員制フラグ
			sqlString.AppendLine(",NVL(BASIC.CRS_BLOCK_CAPACITY,'0') AS CRS_BLOCK_CAPACITY "); //コースブロック定員
			sqlString.AppendLine(",NVL(BASIC.CAR_TYPE_CD,'') AS CAR_TYPE_CD "); //車種コード
			sqlString.AppendLine(",NVL(ZASEKI.BLOCK_KAKUHO_NUM,0) AS Z_BLOCK_KAKUHO_NUM "); //ブロック確保数
			sqlString.AppendLine(",NVL(ZASEKI.SUB_SEAT_OK_KBN,'') AS Z_SUB_SEAT_OK_KBN "); //補助席可区分
			sqlString.AppendLine(",'' AS Z_ARRAY_STATE_REGULAR "); //配列状態定
			sqlString.AppendLine(",'' AS Z_ARRAY_STATE_HO "); //配列状態補
			sqlString.AppendLine(",'' AS Z_ARRAY_STATE_1KAI "); //配列状態１Ｆ
			sqlString.AppendLine(",ZASEKI.SYSTEM_ENTRY_DAY AS Z_SYSTEM_ENTRY_DAY "); //座席イメージ_システム登録日
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_DAY "); //システム更新日
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PERSON_CD "); //システム更新者コード
			sqlString.AppendLine(",BASIC.SYSTEM_UPDATE_PGMID "); //システム更 新ＰＧＭＩＤ
			sqlString.AppendLine(",NVL(BASIC.USING_FLG,'') AS USING_FLG "); //使用中フラグ
			sqlString.AppendLine(",' ' AS HENKOU_KAHI_KBN "); //変更可否区分

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
