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
	/// バス会社一括登録のDAクラス
	/// </summary>
	public class BusCompanyBulk_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getBusCompanyBulk, //一覧結果取得検索
			executeUpdateBusCompanyCd, //更新
			executeReverseBusCompanyCd //戻し
		}

		private const string paramSyuptDayFrom = "SYUPT_DAY_FROM";
		private const string paramSyuptDayTo = "SYUPT_DAY_TO";
		private const string paramBusCompanyCdOnlyBlank = "BUS_COMPANY_CD_ONLY_BLANK";

#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessBusCompanyBulkTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getBusCompanyBulk:
					//一覧結果取得検索
					sqlString = getBusCompanyBulk(paramInfoList);
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
		/// <returns>SQL分</returns>
		/// <remarks></remarks>
		private string getBusCompanyBulk(Hashtable paramList)
		{

			CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

			StringBuilder sqlString = new StringBuilder();
			paramClear();


			//パラメータ設定
			setParam(paramSyuptDayFrom, paramList[paramSyuptDayFrom], clsCrsLedgerBasicEntity.syuptDay.DBType, clsCrsLedgerBasicEntity.syuptDay.IntegerBu, clsCrsLedgerBasicEntity.syuptDay.DecimalBu);
			if (paramList[paramSyuptDayTo] != null)
			{
				setParam(paramSyuptDayTo, paramList[paramSyuptDayTo], clsCrsLedgerBasicEntity.syuptDay.DBType, clsCrsLedgerBasicEntity.syuptDay.IntegerBu, clsCrsLedgerBasicEntity.syuptDay.DecimalBu);
			}
			setParam(clsCrsLedgerBasicEntity.crsCd.PhysicsName, paramList[clsCrsLedgerBasicEntity.crsCd.PhysicsName], clsCrsLedgerBasicEntity.crsCd.DBType, clsCrsLedgerBasicEntity.crsCd.IntegerBu, clsCrsLedgerBasicEntity.crsCd.DecimalBu);

			//SELECT句
			sqlString.AppendLine(" SELECT ");
			sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') AS SYUPT_DAY ");
			sqlString.AppendLine(",BASIC.CRS_CD ");
			sqlString.AppendLine(",BASIC.CRS_NAME ");
			sqlString.AppendLine(",TO_CHAR(BASIC.GOUSYA) AS GOUSYA ");
			sqlString.AppendLine(",CODE_S.CODE_NAME AS SAIKOU_KAKUTEI_KBN");
			sqlString.AppendLine(",PLACE.PLACE_NAME_1 AS PLACE_NAME ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_1 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME ");
			sqlString.AppendLine(",BASIC.BUS_COMPANY_CD ");
			sqlString.AppendLine(",BUS_COMPANY.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ");
			sqlString.AppendLine(",BASIC.USING_FLG ");
			sqlString.AppendLine(",' ' AS DIFFERENCE_FLG ");
			sqlString.AppendLine(",' ' AS UPDATE_TARGET_FLG ");
			sqlString.AppendLine(",BASIC.TEIKI_KIKAKU_KBN ");
			//FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("T_CRS_LEDGER_BASIC BASIC ");
			sqlString.AppendLine("LEFT JOIN M_PLACE PLACE ON BASIC.HAISYA_KEIYU_CD_1 = PLACE.PLACE_CD ");
			sqlString.AppendLine("LEFT JOIN M_SIIRE_SAKI BUS_COMPANY ON BASIC.BUS_COMPANY_CD = BUS_COMPANY.SIIRE_SAKI_CD || BUS_COMPANY.SIIRE_SAKI_NO ");
			sqlString.AppendLine("AND RTRIM(BUS_COMPANY.DELETE_DATE) IS NULL ");
			sqlString.AppendLine("LEFT JOIN M_CODE CODE_S ON CODE_S.CODE_BUNRUI = '" + CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.saikouKakuteiKbn) + "' AND BASIC.SAIKOU_KAKUTEI_KBN = CODE_S.CODE_VALUE ");
			//WHERE句
			sqlString.AppendLine(" WHERE ");
			//出発日（To）が設定されていない場合
			if (paramList[paramSyuptDayTo] == null)
			{
				sqlString.AppendLine(" BASIC.SYUPT_DAY = :   " + paramSyuptDayFrom);
			}
			else
			{
				sqlString.AppendLine(" BASIC.SYUPT_DAY >= :" + paramSyuptDayFrom);
				sqlString.AppendLine(" AND ");
				sqlString.AppendLine(" BASIC.SYUPT_DAY <= :" + paramSyuptDayTo);
			}
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" BASIC.CRS_CD = :" + clsCrsLedgerBasicEntity.crsCd.PhysicsName);
			//バス会社コード未記入分のみにチェックが入っている場合
			if (paramList[paramBusCompanyCdOnlyBlank] != null)
			{
				sqlString.AppendLine(" AND");
				sqlString.AppendLine(" RTRIM(BASIC.BUS_COMPANY_CD) IS NULL");
			}
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" NVL(BASIC.SAIKOU_KAKUTEI_KBN,'*') NOT IN('" + FixedCd.SaikouKakuteiKbn.Tyushi + "','" + FixedCdTehai.SaikouKakuteiKbn.Haishi + "') ");
			sqlString.AppendLine("     AND NVL(BASIC.MARU_ZOU_MANAGEMENT_KBN,'*') <> '" + FixedCd.MaruzouKanriKbn.Maruzou + "' ");
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" NVL(BASIC.SYUPT_JI_CARRIER_KBN,'1') = '" + (int)FixedCd.SyuptJiCarrierKbnType.bus + "'");
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" NVL(BASIC.CAR_TYPE_CD,'*') <> '" + FixedCd.CarTypeCdKakuu + "'");
			sqlString.AppendLine(" AND ");
			sqlString.AppendLine(" NVL(BASIC.DELETE_DAY,0) = 0 ");
			//ORDER BY句
			sqlString.AppendLine(" ORDER BY ");
			sqlString.AppendLine(" BASIC.SYUPT_DAY ");
			sqlString.AppendLine(" ,BASIC.CRS_CD ");
			sqlString.AppendLine(" ,BASIC.SYUPT_TIME_1 ");
			sqlString.AppendLine(" ,BASIC.GOUSYA ");

			return sqlString.ToString();

		}

#endregion


#region  UPDATE処理 

		/// <summary>
		/// 使用中フラグ更新処理
		/// </summary>
		/// <param name="paramDatatable"></param>
		/// <param name="paramPgmId"></param>
		/// <returns>DataTable</returns>
		/// <remarks></remarks>
		public DataTable updateUsingFlag(DataTable paramDatatable, string paramPgmId)
		{
			OracleTransaction oracleTransaction = null;
			DataTable returnValue = new DataTable();

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
