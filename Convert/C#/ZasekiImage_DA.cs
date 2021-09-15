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
	/// 座席イメージのDAクラス
	/// </summary>
	public class ZasekiImage_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getZasekiBlockInfo, //座席ブロック情報結果取得検索
			getZasekiBlockInfoRjc, //座席ブロック情報結果取得検索
			getCrsLegBasic, //コース台帳(基本)取得
			getCrsLegReserveBasic, //コース台帳(基本)取得（乗せ合わせ）
			getZasekiImage, //座席イメージ（バス情報）取得
			updateZasekiBlockInfo, //座席ブロック情報更新
			updateZasekiBlockInfoRjc //座席ブロック情報更新
		}

		public enum updateType: int
		{
			regist, //登録
			reject //解除
		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessZasekiBlockInfo(accessType selectType, Hashtable paramInfoList)
		{

			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			base.paramClear();

			switch (selectType)
			{
				case accessType.getZasekiBlockInfo:
					sqlString = getZasekiBlockInfo(updateType.regist, paramInfoList);
					break;
				case accessType.getZasekiBlockInfoRjc:
					sqlString = getZasekiBlockInfo(updateType.reject, paramInfoList);
					break;
				case accessType.getCrsLegBasic:
					sqlString = getCrsLegBasic(paramInfoList);
					break;
				case accessType.getZasekiImage:
					sqlString = getZasekiImage(paramInfoList);
					break;
				case accessType.getCrsLegReserveBasic:
					sqlString = getCrsLegReserveBasic(paramInfoList);
					break;
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
		protected string getZasekiBlockInfo(updateType type, Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();
			TZasekiImageEntity zasekiEnt = new TZasekiImageEntity();
			paramClear();

			//SELECT句
			sqlString.AppendLine("SELECT ");
			sqlString.AppendLine("  INFO.ZASEKI_KAI ");
			sqlString.AppendLine("  , INFO.ZASEKI_LINE ");
			sqlString.AppendLine("  , INFO.ZASEKI_COL ");
			sqlString.AppendLine("  , INFO.ZASEKI_KIND ");
			sqlString.AppendLine("  , INFO.ZASEKI_KBN ");
			sqlString.AppendLine("  , INFO.BLOCK_KBN ");
			sqlString.AppendLine("  , INFO.EIGYOSYO_BLOCK_KBN ");
			sqlString.AppendLine("  , INFO.EIGYOSYO_CD ");
			sqlString.AppendLine("  , EIG.EIGYOSYO_NAME_PRINT_2 AS EIGYOSYO_NM ");
			sqlString.AppendLine("  , INFO.YOYAKU_FLG ");
			sqlString.AppendLine("  , INFO.LADIES_SEAT_FLG ");
			if (type == updateType.regist)
			{
				sqlString.AppendLine("  , CASE ");
				sqlString.AppendLine("    WHEN INFO.ZASEKI_KIND = 'X' ");
				sqlString.AppendLine("    OR INFO.BLOCK_KBN = '2' ");
				//sqlString.AppendLine("    OR ( ")
				//sqlString.AppendLine("      IMAGE.SUB_SEAT_OK_KBN IS NULL ")
				//sqlString.AppendLine("      AND INFO.ZASEKI_KIND = '2' ")
				//sqlString.AppendLine("    ) ")
				sqlString.AppendLine("      THEN 1 ");
				sqlString.AppendLine("    WHEN INFO.BLOCK_KBN = '1' AND INFO.EIGYOSYO_BLOCK_KBN = '1' ");
				sqlString.AppendLine("      THEN 2 ");
				sqlString.AppendLine("    WHEN INFO.YOYAKU_FLG = '1' ");
				sqlString.AppendLine("      THEN 3 ");
				sqlString.AppendLine("    WHEN INFO.LADIES_SEAT_FLG = '1' ");
				sqlString.AppendLine("      THEN 4 ");
				sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' ");
				sqlString.AppendLine("      THEN 5 ");
				sqlString.AppendLine("    ELSE 0 ");
				sqlString.AppendLine("    END EDIT_FLG ");
			}
			else
			{
				sqlString.AppendLine("  , CASE ");
				sqlString.AppendLine("    WHEN INFO.BLOCK_KBN = '2' ");
				sqlString.AppendLine("      THEN 1 ");
				sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' AND INFO.EIGYOSYO_CD = '" + UserInfoManagement.eigyosyoCd + "' ");
				sqlString.AppendLine("      THEN 0 ");
				sqlString.AppendLine("    WHEN INFO.EIGYOSYO_BLOCK_KBN = '1' AND INFO.EIGYOSYO_CD <> '" + UserInfoManagement.eigyosyoCd + "' ");
				sqlString.AppendLine("      THEN 1 ");
				sqlString.AppendLine("    ELSE 2 ");
				sqlString.AppendLine("    END EDIT_FLG ");
			}
			sqlString.AppendLine("  , INFO.ZASEKI_STATE");
			sqlString.AppendLine("FROM ");
			sqlString.AppendLine("  T_ZASEKI_IMAGE IMAGE ");
			sqlString.AppendLine("  INNER JOIN T_ZASEKI_IMAGE_INFO INFO ");
			sqlString.AppendLine("    ON IMAGE.BUS_RESERVE_CD = INFO.BUS_RESERVE_CD ");

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
