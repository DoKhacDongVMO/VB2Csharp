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
	/// コース台帳一括修正（料金情報）のDAクラス
	/// </summary>
	public class ChargeInfo_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getChargeInfoHead, //ヘッダ情報取得
			getChargeInfo, //一覧結果取得検索(定期)
			getChargeInfoKikaku, //一覧結果取得検索(企画)
			executeUpdateChargeInfo, //更新(定期)
			executeUpdateChargeKikakuInfo, //更新(企画)
			executeReturnChargeInfo, //戻し
			getKbnNo, //区分No取得
			getJininCd //人員コード取得
		}

		/// <summary>
		/// パラメータキー
		/// </summary>
		/// <remarks></remarks>
		public sealed class ParamHashKeys
		{
			/// <summary>
			/// 定期用(KEY)
			/// </summary>
			public const string BASIC_KEYS_TEIKI = "BASIC_KEYS_TEIKI";
			/// <summary>
			/// 企画用(KEY)
			/// </summary>
			public const string BASIC_KEYS = "BASIC_KEYS";
		}

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessChargeInfoTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getChargeInfo:
					//一覧結果取得検索
					sqlString = getChargeInfo(paramInfoList);
					break;
				case accessType.getChargeInfoKikaku:
					//一覧結果取得検索
					sqlString = getChargeInfoKikaku(paramInfoList);
					break;
				case accessType.getChargeInfoHead:
					//ヘッダ情報検索
					sqlString = getChargeInfoHead(paramInfoList);
					break;
				case accessType.getKbnNo:
					//区分No取得
					sqlString = getKbnNo(paramInfoList);
					break;
				case accessType.getJininCd:
					//人員コード取得
					sqlString = getJininCd(paramInfoList);
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
		/// ヘッダ情報取得用SELECT
		/// </summary>
		/// <param name="paramList"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected string getChargeInfoHead(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();
			paramClear();

			//SELECT句
			sqlString.AppendLine("SELECT DISTINCT ");
			sqlString.AppendLine("  LINE_NO ");
			sqlString.AppendLine("  , CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("  , CHARGE_KBN_JININ_NAME ");
			sqlString.AppendLine("  , TEIKI_KIKAKU_KBN ");
			//FROM句
			sqlString.AppendLine("FROM ");
			sqlString.AppendLine("  ( ");
			sqlString.AppendLine("    SELECT ");
			sqlString.AppendLine("      CJK.CHARGE_KBN_JININ_CD AS CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("      , MCJK.CHARGE_KBN_JININ_NAME AS CHARGE_KBN_JININ_NAME ");
			sqlString.AppendLine("      , BASIC.TEIKI_KIKAKU_KBN AS TEIKI_KIKAKU_KBN ");
			sqlString.AppendLine("      , CJK.LINE_NO ");
			sqlString.AppendLine("      , RANK() OVER ( ");
			sqlString.AppendLine("        PARTITION BY ");
			sqlString.AppendLine("          CJK.LINE_NO ");
			sqlString.AppendLine("        ORDER BY ");
			sqlString.AppendLine("          CJK.CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("      ) RNK ");
			sqlString.AppendLine("    FROM ");
			sqlString.AppendLine("      T_CRS_LEDGER_BASIC BASIC ");
			sqlString.AppendLine("      INNER JOIN T_CRS_LEDGER_BASIC_CHARGE_KBN CJK ");
			sqlString.AppendLine("        ON BASIC.SYUPT_DAY = CJK.SYUPT_DAY ");
			sqlString.AppendLine("        AND BASIC.CRS_CD = CJK.CRS_CD ");
			sqlString.AppendLine("        AND BASIC.GOUSYA = CJK.GOUSYA ");
			sqlString.AppendLine("      LEFT JOIN M_CHARGE_JININ_KBN MCJK ");
			sqlString.AppendLine("        ON CJK.CHARGE_KBN_JININ_CD = MCJK.CHARGE_KBN_JININ_CD ");
			//WHERE句
			sqlString.AppendLine("      WHERE ");
			sqlString.AppendLine("        NVL(BASIC.DELETE_DAY,0) = 0 ");
			sqlString.AppendLine("        AND ");
			sqlString.AppendLine("        (BASIC.CRS_CD, BASIC.SYUPT_DAY) IN ( ");
			sqlString.AppendLine("        ").Append(paramList[ParamHashKeys.BASIC_KEYS_TEIKI]);
			sqlString.AppendLine("        ) ");

			//ORDER BY句
			sqlString.AppendLine("  ) ");
			sqlString.AppendLine("WHERE ");
			sqlString.AppendLine("  RNK = 1 ");
			sqlString.AppendLine("ORDER BY ");
			sqlString.AppendLine("  LINE_NO ");

			return sqlString.ToString();

		}

		/// <summary>
		/// 区分No取得用SELECT
		/// </summary>
		/// <param name="paramList"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected string getKbnNo(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
