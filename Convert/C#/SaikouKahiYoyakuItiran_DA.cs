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
	/// 催行可否照会（予約情報一覧）のDAクラス
	/// </summary>
	public class SaikouKahiYoyakuItiran_DA : Hatobus.ReservationManagementSystem.Common.DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getSaikouKahiYoyakuItiran //一覧結果取得検索
		}

		//予約情報（基本）エンティティ
		private YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessSaikouKahiYoyakuItiran(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getSaikouKahiYoyakuItiran:
					//一覧結果取得検索
					sqlString = getSaikouKahiYoyakuItiran(paramInfoList);
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
		protected string getSaikouKahiYoyakuItiran(Hashtable paramList)
		{

			StringBuilder sqlString = new StringBuilder();

			sqlString.AppendLine(" SELECT");
			sqlString.AppendLine(" DISTINCT ");
			//sqlString.AppendLine("   YIB.YOYAKU_KBN || TO_CHAR(YIB.YOYAKU_NO) AS YOYAKU_NO_DISP")       ' 予約No(表示用)
			sqlString.AppendLine("   YIB.YOYAKU_KBN || ',' || TRIM(TO_CHAR(YIB.YOYAKU_NO,'000,000,000')) AS YOYAKU_NO_DISP"); // 予約No(表示用)
			sqlString.AppendLine(" , YIB.YOYAKU_KBN "); // 予約区分
			sqlString.AppendLine(" , YIB.YOYAKU_NO "); // 予約ＮＯ
			sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_1 "); // ＲＯＯＭＩＮＧ別人数１
			sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_2 "); // ＲＯＯＭＩＮＧ別人数２
			sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_3 "); // ＲＯＯＭＩＮＧ別人数３
			sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_4 "); // ＲＯＯＭＩＮＧ別人数４
			sqlString.AppendLine(" , YIB.ROOMING_BETU_NINZU_5 "); // ＲＯＯＭＩＮＧ別人数５
			sqlString.AppendLine(" , YIB.HAKKEN_NAIYO "); // 発券内容
			sqlString.AppendLine(" , YIB.SEISAN_HOHO "); // 精算方法
			sqlString.AppendLine(" , YIB.SURNAME || YIB.NAME AS YYKMKS "); // 姓+名
			sqlString.AppendLine(" , " + CommonKyushuUtil.setSQLDateFormat("ENTRY_DAY", ((int)FixedCdKyushu.Date_FormatType.formatSlashYYYYMMDD).ToString(), "YIB") + " AS ENTRY_DAY "); // 登録日
			sqlString.AppendLine(" , ( ");
			sqlString.AppendLine("     SELECT");
			sqlString.AppendLine("       SUM(ADT.CHARGE_APPLICATION_NINZU) ");
			sqlString.AppendLine("     FROM ");
			sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN ADT ");
			sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK4 ON ADT.CHARGE_KBN_JININ_CD = CJK4.CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("     WHERE ");
			sqlString.AppendLine("          ADT.YOYAKU_NO = YIB.YOYAKU_NO ");
			sqlString.AppendLine("      And ADT.YOYAKU_KBN = YIB.YOYAKU_KBN ");
			// かつ　予約情報（コース料金_料金区分）.料金区分（人員）コード　＝　大人
			sqlString.AppendLine("      And CJK4.SHUYAKU_CHARGE_KBN_CD = '" + FixedCd.ChargeKbnJininCd.adult + "'");
			sqlString.AppendLine("      GROUP BY CJK4.SHUYAKU_CHARGE_KBN_CD ");
			sqlString.AppendLine(" ) AS NINZU_ADULT "); // 大人
			sqlString.AppendLine(" , ( ");
			sqlString.AppendLine("     SELECT");
			sqlString.AppendLine("       SUM(MAN.CHARGE_APPLICATION_NINZU) ");
			sqlString.AppendLine("     FROM ");
			sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN MAN ");
			sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK5 ON MAN.CHARGE_KBN_JININ_CD = CJK5.CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("     WHERE ");
			sqlString.AppendLine("           MAN.YOYAKU_NO = YIB.YOYAKU_NO ");
			sqlString.AppendLine("       And MAN.YOYAKU_KBN = YIB.YOYAKU_KBN");
			sqlString.AppendLine("       And(");
			// 料金区分（人員）マスタ.集約料金区分コード＝’大人’
			sqlString.AppendLine("               CJK5.SHUYAKU_CHARGE_KBN_CD = '" + FixedCd.ShuyakuChargeKbnType.adult + "'");
			// かつ　料金区分（人員）マスタ.性別＝’男性’
			sqlString.AppendLine("           And CJK5.SEX_BETU = '" + (int)FixedCd.SexBetu.Man + "'");
			sqlString.AppendLine("       )");
			sqlString.AppendLine("     GROUP BY CJK5.SHUYAKU_CHARGE_KBN_CD");
			sqlString.AppendLine(" ) AS NINZU_MAN"); // 大人(男)(算出用)
			sqlString.AppendLine(" , ( ");
			sqlString.AppendLine("     SELECT");
			sqlString.AppendLine("       SUM(WMN.CHARGE_APPLICATION_NINZU) ");
			sqlString.AppendLine("     FROM ");
			sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN WMN ");
			sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK6 ON WMN.CHARGE_KBN_JININ_CD = CJK6.CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("     WHERE ");
			sqlString.AppendLine("           WMN.YOYAKU_NO = YIB.YOYAKU_NO ");
			sqlString.AppendLine("       And WMN.YOYAKU_KBN = YIB.YOYAKU_KBN");
			sqlString.AppendLine("       And(");
			// 料金区分（人員）マスタ.集約料金区分コード＝’大人’
			sqlString.AppendLine("               CJK6.SHUYAKU_CHARGE_KBN_CD = '" + FixedCd.ShuyakuChargeKbnType.adult + "'");
			// かつ　料金区分（人員）マスタ.性別＝’女性’
			sqlString.AppendLine("           And CJK6.SEX_BETU = '" + (int)FixedCd.SexBetu.Woman + "'");
			sqlString.AppendLine("       )");
			sqlString.AppendLine("     GROUP BY CJK6.SHUYAKU_CHARGE_KBN_CD");
			sqlString.AppendLine(" ) AS NINZU_WOMAN"); // 大人(女)(算出用)
			sqlString.AppendLine(" , ( ");
			sqlString.AppendLine("     SELECT");
			sqlString.AppendLine("       SUM(JNR.CHARGE_APPLICATION_NINZU) ");
			sqlString.AppendLine("     FROM ");
			sqlString.AppendLine("       T_YOYAKU_INFO_CRS_CHARGE_CHARGE_KBN JNR ");
			sqlString.AppendLine("     LEFT JOIN M_CHARGE_JININ_KBN CJK2 ON JNR.CHARGE_KBN_JININ_CD = CJK2.CHARGE_KBN_JININ_CD ");
			sqlString.AppendLine("     WHERE ");

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
