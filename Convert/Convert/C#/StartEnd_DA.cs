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
	/// コース台帳一括修正（発着情報）のDAクラス
	/// </summary>
	public class StartEnd_DA : DataAccessorBase
	{
#region  定数／変数 

		public enum accessType: int
		{
			getPlaceMaster, //場所マスタ検索
			getStartEnd, //一覧結果取得検索
			executeUpdateBusCompanyCd, //更新
			executeReverseBusCompanyCd //戻し
		}

		private const string CodeValueSyugoTime = "106"; //コードマスタ.コード値（集合時間（出発日－？分））

		private TehaiCommon _comTehai = new TehaiCommon();
#endregion

#region  SELECT処理 

		/// <summary>
		/// SELECT用DBアクセス
		/// </summary>
		/// <param name="type"></param>
		/// <param name="paramInfoList"></param>
		/// <returns></returns>
		public DataTable accessStartEndTehai(accessType type, Hashtable paramInfoList = null)
		{
			//SQL文字列
			string sqlString = string.Empty;
			//戻り値
			DataTable returnValue = null;

			switch (type)
			{
				case accessType.getPlaceMaster:
					// 場所マスタ検索
					sqlString = getPlaceMaster();
					break;
				case accessType.getStartEnd:
					//一覧結果取得検索
					sqlString = getStartEnd(paramInfoList);
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
				throw ex;
			}

			return returnValue;

		}

		/// <summary>
		/// 場所マスタ検索
		/// </summary>
		public string getPlaceMaster()
		{

			DataTable resultDataTable = new DataTable();
			StringBuilder sqlString = new StringBuilder();

			//空レコード挿入要否に従い、空行挿入
			sqlString.AppendLine(" SELECT");
			sqlString.AppendLine(" ' ' AS CODE_VALUE ");
			sqlString.AppendLine(",'' AS CODE_NAME ");
			//FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("DUAL UNION ");
			sqlString.AppendLine(" SELECT");
			sqlString.AppendLine(" RTRIM(PLACE_CD) AS CODE_VALUE ");
			sqlString.AppendLine(",PLACE_NAME_1 AS CODE_NAME ");
			//FROM句
			sqlString.AppendLine(" FROM ");
			sqlString.AppendLine("M_PLACE ");
			//WHERE句
			sqlString.AppendLine(" WHERE NVL(DELETE_DATE,0) = 0 ");
			//ORDER BY句
			sqlString.AppendLine(" ORDER BY ");
			sqlString.AppendLine(" CODE_VALUE ASC ");

			return sqlString.ToString();
		}

		/// <summary>
		/// 検索用SELECT
		/// </summary>
		/// <param name="paramList"></param>
		/// <returns>SQL分</returns>
		/// <remarks></remarks>
		private string getStartEnd(Hashtable paramList)
		{

			CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();
			CdMasterEntity clsCdMasterEntity = new CdMasterEntity();

			StringBuilder sqlString = new StringBuilder();
			paramClear();


			//パラメータ設定
			setParam(clsCrsLedgerBasicEntity.syuptDay.PhysicsName, paramList[clsCrsLedgerBasicEntity.syuptDay.PhysicsName], clsCrsLedgerBasicEntity.syuptDay.DBType, clsCrsLedgerBasicEntity.syuptDay.IntegerBu, clsCrsLedgerBasicEntity.syuptDay.DecimalBu);
			setParam(clsCrsLedgerBasicEntity.crsCd.PhysicsName, paramList[clsCrsLedgerBasicEntity.crsCd.PhysicsName], clsCrsLedgerBasicEntity.crsCd.DBType, clsCrsLedgerBasicEntity.crsCd.IntegerBu, clsCrsLedgerBasicEntity.crsCd.DecimalBu);
			setParam(clsCrsLedgerBasicEntity.gousya.PhysicsName, paramList[clsCrsLedgerBasicEntity.gousya.PhysicsName], clsCrsLedgerBasicEntity.gousya.DBType, clsCrsLedgerBasicEntity.gousya.IntegerBu, clsCrsLedgerBasicEntity.gousya.DecimalBu);
			//setParam(.haisyaKeiyuCd1.PhysicsName, paramList.Item(.haisyaKeiyuCd1.PhysicsName), .haisyaKeiyuCd1.DBType, .haisyaKeiyuCd1.IntegerBu, .haisyaKeiyuCd1.DecimalBu)
			if (Convert.ToInt32(paramList[clsCrsLedgerBasicEntity.syuptJiCarrierKbn.PhysicsName]) == (int)FixedCd.SyuptJiCarrierKbnType.bus)
			{
				setParam(clsCrsLedgerBasicEntity.haisyaKeiyuCd1.PhysicsName, paramList[clsCrsLedgerBasicEntity.haisyaKeiyuCd1.PhysicsName], clsCrsLedgerBasicEntity.haisyaKeiyuCd1.DBType, clsCrsLedgerBasicEntity.haisyaKeiyuCd1.IntegerBu, clsCrsLedgerBasicEntity.haisyaKeiyuCd1.DecimalBu);
			}
			else
			{
				setParam(clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.PhysicsName, paramList[clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.PhysicsName], clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.DBType, clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.IntegerBu, clsCrsLedgerBasicEntity.syuptPlaceCdCarrier.DecimalBu);
			}
			setParam(clsCrsLedgerBasicEntity.FixedCdTehai.UnkyuKbn.PhysicsName, paramList[clsCrsLedgerBasicEntity.FixedCdTehai.UnkyuKbn.PhysicsName], clsCrsLedgerBasicEntity.FixedCdTehai.UnkyuKbn.DBType, clsCrsLedgerBasicEntity.FixedCdTehai.UnkyuKbn.IntegerBu, clsCrsLedgerBasicEntity.FixedCdTehai.UnkyuKbn.DecimalBu);
			setParam(clsCrsLedgerBasicEntity.FixedCdTehai.SaikouKakuteiKbn.PhysicsName, paramList[clsCrsLedgerBasicEntity.FixedCdTehai.SaikouKakuteiKbn.PhysicsName], clsCrsLedgerBasicEntity.FixedCdTehai.SaikouKakuteiKbn.DBType, clsCrsLedgerBasicEntity.FixedCdTehai.SaikouKakuteiKbn.IntegerBu, clsCrsLedgerBasicEntity.FixedCdTehai.SaikouKakuteiKbn.DecimalBu);
			setParam(clsCrsLedgerBasicEntity.maruZouManagementKbn.PhysicsName, paramList[clsCrsLedgerBasicEntity.maruZouManagementKbn.PhysicsName], clsCrsLedgerBasicEntity.maruZouManagementKbn.DBType, clsCrsLedgerBasicEntity.maruZouManagementKbn.IntegerBu, clsCrsLedgerBasicEntity.maruZouManagementKbn.DecimalBu);
			setParam(clsCrsLedgerBasicEntity.FixedCd.YobiCd.PhysicsName, paramList[clsCrsLedgerBasicEntity.FixedCd.YobiCd.PhysicsName], clsCrsLedgerBasicEntity.FixedCd.YobiCd.DBType, clsCrsLedgerBasicEntity.FixedCd.YobiCd.IntegerBu, clsCrsLedgerBasicEntity.FixedCd.YobiCd.DecimalBu);
			setParam(clsCdMasterEntity.CdValue.PhysicsName, CodeValueSyugoTime, clsCdMasterEntity.CdValue.DBType, clsCdMasterEntity.CdValue.IntegerBu, clsCdMasterEntity.CdValue.DecimalBu);

			//SELECT句
			sqlString.AppendLine(" SELECT ");
			sqlString.AppendLine(" TO_CHAR(TO_DATE(BASIC.SYUPT_DAY),'yyyy/MM/dd') AS SYUPT_DAY ");
			sqlString.AppendLine(",CODE_YOBI.CODE_NAME AS YOBI");
			sqlString.AppendLine(",TO_CHAR(BASIC.GOUSYA) AS GOUSYA ");
			sqlString.AppendLine(",CODE_UNKYU.CODE_NAME AS UNKYU_KBN");
			sqlString.AppendLine(",CODE_SAIKOU.CODE_NAME AS SAIKOU_KAKUTEI_KBN");
			sqlString.AppendLine(",BASIC.BUS_COMPANY_CD ");
			sqlString.AppendLine(",BUS_COMPANY.SIIRE_SAKI_NAME AS BUS_COMPANY_NAME ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_1 ");
			sqlString.AppendLine(",PLACE_1.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_1 ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_2 ");
			sqlString.AppendLine(",PLACE_2.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_2 ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_3 ");
			sqlString.AppendLine(",PLACE_3.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_3 ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_4 ");
			sqlString.AppendLine(",PLACE_4.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_4 ");
			sqlString.AppendLine(",BASIC.HAISYA_KEIYU_CD_5 ");
			sqlString.AppendLine(",PLACE_5.PLACE_NAME_1 AS HAISYA_KEIYU_NAME_5 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_1") + " AS SYUGO_TIME_1 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_2") + " AS SYUGO_TIME_2 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_3") + " AS SYUGO_TIME_3 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_4") + " AS SYUGO_TIME_4 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUGO_TIME_5") + " AS SYUGO_TIME_5 ");
			sqlString.AppendLine("," + CommonKyushuUtil.setSQLTime24Format("BASIC.SYUPT_TIME_1") + " AS SYUPT_TIME_1 ");

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
