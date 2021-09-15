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
	public class Cost_DA : DataAccessorBase
	{
#region  定数／変数 
		public enum accessType: int
		{
			getSearchData, //検索結果取得
			updateData //登録処理
		}

		/// <summary>
		/// パラメータキー
		/// </summary>
		/// <remarks></remarks>
		public sealed class ParamHashKeys
		{
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
		public DataTable getCostTable(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			base.paramClear();

			switch (type)
			{
				case accessType.getSearchData:
					//一覧結果取得検索
					sqlString = getChargeInfoHead(paramInfoList);
					break;
				default:
					//該当処理なし
					return returnValue;
			}

			try
			{
				returnValue = getDataTable(sqlString);
				foreach (DataColumn col in returnValue.Columns)
				{
					col.AllowDBNull = true;
				}
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

			sqlString.AppendLine("SELECT ");
			sqlString.AppendLine("  DISP_SYUPT_DAY ");
			sqlString.AppendLine("  , YOBI_NAME ");
			sqlString.AppendLine("  , PLACE_NAME ");
			sqlString.AppendLine("  , SYUPT_TIME ");
			sqlString.AppendLine("  , DISP_GOUSYA ");
			sqlString.AppendLine("  , UNKYU_NAME ");
			sqlString.AppendLine("  , UNKYU_KBN ");
			sqlString.AppendLine("  , CASE ");
			sqlString.AppendLine("    WHEN EDABAN = 1 ");
			sqlString.AppendLine("      THEN '変更' ");
			sqlString.AppendLine("    ELSE NULL ");
			sqlString.AppendLine("    END K_BUTTON                                  --降車ヶ所(ボタン) ");
			sqlString.AppendLine("  , DAILY ");
			sqlString.AppendLine("  , SIIRE_SAKI_KIND_CD ");
			sqlString.AppendLine("  , SIIRE_SAKI_KIND_NAME ");
			sqlString.AppendLine("  , CODE ");
			sqlString.AppendLine("  , SIIRE_SAKI_NAME ");
			sqlString.AppendLine("  , MOKUTEKI ");
			sqlString.AppendLine("  , '変更' AS G_BUTTON                              --原価(ボタン) ");
			sqlString.AppendLine("  , G_FLG ");
			sqlString.AppendLine("  , CASE ");
			sqlString.AppendLine("    WHEN EDABAN = 1 ");
			sqlString.AppendLine("      THEN '変更' ");
			sqlString.AppendLine("    ELSE NULL ");
			sqlString.AppendLine("    END S_BUTTON                                  --その他原価(ボタン) ");
			sqlString.AppendLine("  , LINE_NO ");
			sqlString.AppendLine("  , CRS_CD ");
			sqlString.AppendLine("  , GOUSYA ");
			sqlString.AppendLine("  , SYUPT_DAY ");
			sqlString.AppendLine("  , RNK ");
			sqlString.AppendLine("  , EDABAN ");
			sqlString.AppendLine("FROM ");
			sqlString.AppendLine("  ( ");
			sqlString.AppendLine("    SELECT ");
			sqlString.AppendLine("      TO_CHAR(TO_DATE(BASIC.SYUPT_DAY), 'yyyy/MM/dd') AS DISP_SYUPT_DAY --日付 ");
			sqlString.AppendLine("      , CODE_Y.CODE_NAME AS YOBI_NAME             --曜日 ");
			sqlString.AppendLine("      , PLACE.PLACE_NAME_1 AS PLACE_NAME          --乗車地 ");
			sqlString.AppendLine("      ," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + "AS SYUPT_TIME --出発時間 ");
			sqlString.AppendLine("      , BASIC.GOUSYA AS DISP_GOUSYA               --号車 ");
			sqlString.AppendLine("      , CASE BASIC.TEIKI_KIKAKU_KBN ");
			sqlString.AppendLine("        WHEN '1' THEN CODE_U_T.CODE_NAME          --定期の場合 ");
			sqlString.AppendLine("        ELSE CODE_U_K.CODE_NAME                   --企画の場合 ");
			sqlString.AppendLine("        END AS UNKYU_NAME                         --運休 ");
			sqlString.AppendLine("      , BASIC.UNKYU_KBN AS UNKYU_KBN              --運休区分 ");
			sqlString.AppendLine("      , KOUSHA.DAILY                              --日目 ");
			sqlString.AppendLine("      , SIIRE.SIIRE_SAKI_KIND_CD                  --降車ヶ所種別 ");
			sqlString.AppendLine("      , CODE_SK.CODE_NAME AS SIIRE_SAKI_KIND_NAME --降車ヶ所種別名 ");
			sqlString.AppendLine("      , KOUSHA.KOSHAKASHO_CD || KOUSHA.KOSHAKASHO_EDABAN AS CODE --コード ");
			sqlString.AppendLine("      , SIIRE.SIIRE_SAKI_NAME                     --降車ヶ所名 ");
			sqlString.AppendLine("      , CODE_S.CODE_NAME AS MOKUTEKI              --精算目的 ");
			sqlString.AppendLine("      , CASE ");
			sqlString.AppendLine("        WHEN GENKA.CNT > 0 ");
			sqlString.AppendLine("        OR GENKA_CARRIER.CNT > 0 ");
			sqlString.AppendLine("          THEN '済' ");
			sqlString.AppendLine("        ELSE '未' ");
			sqlString.AppendLine("        END AS G_FLG                              --原価設定 ");

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
