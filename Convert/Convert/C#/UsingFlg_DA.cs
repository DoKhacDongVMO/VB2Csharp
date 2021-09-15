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
	public class UsingFlg_DA : DataAccessorBase
	{
#region 変数／定数(Public)
		public struct LOCK_TARGET
		{
			public bool basic;
			//Public zaseki As Boolean
			//Public yoyaku As Boolean
		}

		public struct USING_CHECK
		{
			public bool basic;
			public bool busreserve;
		}


		public enum LOCK_MODE
		{
			USE,
			REJECT
		}

#endregion

#region 変数／定数(Private)
		private DataTable baseTbl = null; //対象データテーブル(コンストラクタで作成)
		private DataTable lockTbl = null; //対象データテーブル(ロック)

		private string pUserId = UserInfoManagement.userId;
		private string pPgmId;
		private DateTime pSysDate;
#endregion

#region 列挙体
		/// <summary>
		/// テーブルモード
		/// </summary>
		public enum MODE_TYPE
		{
			BASIC,
			BUS
		}

		private enum accessType: int
		{
			getUsingFlgList //使用中フラグリスト取得
		}

		/// <summary>
		/// 取得モード
		/// </summary>
		private enum SELECT_MODE
		{
			STANDARD,
			UPDATEERROR
		}

		/// <summary>
		/// 更新モード
		/// </summary>
		private enum UPDATE_MODE
		{
			USE,
			UNUSE_NOUPDATE,
			UNUSE_UPDATE
		}
#endregion

#region 構造体
		/// <summary>
		/// カラムキー
		/// </summary>
		private struct ColumnKeys
		{
			public string key1;
			public string key2;
			public string key3;
		}

		/// <summary>
		/// カラムID
		/// </summary>
		private struct KEY_COLUMNID
		{
			public const string CRS_CD = "CRS_CD";
			public const string GOUSYA = "GOUSYA";
			public const string SYUPT_DAY = "SYUPT_DAY";
			public const string BUS_RESERVE_CD = "BUS_RESERVE_CD";
			public const string USING_FLG = "USING_FLG";
		}
#endregion

#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dt">対象のDataTable</param>
		public UsingFlg_DA(DataTable dt, string pgmId)
		{
			baseTbl = dt.Copy();
			pPgmId = pgmId;
		}
#endregion

#region 公開メソッド
		public int executeUsingFlg(LOCK_TARGET locktype, USING_CHECK chkFlg, LOCK_MODE lockMode)
		{
			OracleTransaction tran = null;
			int retValue = 0;

			if (LOCK_MODE.USE.Equals(lockMode))
			{
				//ロック処理
				if (locktype.basic == true)
				{
					//[条件作成]
					List<string> listWhere = getWhereStr(baseTbl, MODE_TYPE.BASIC);
					if (listWhere.Count == 0)
					{
						//条件無しは失敗と見なす
						return -1;
					}
					else
					{
						try
						{
							//トランザクション開始
							tran = callBeginTransaction();
							//対象データ取得(状況取得)
							lockTbl = getUsingListData(listWhere, tran, SELECT_MODE.STANDARD, chkFlg);
							//ロック処理
							retValue = setUsingFlg(lockTbl, locktype, chkFlg, lockMode, tran);
							//コミット
							callCommitTransaction(tran);
						}
						catch (Exception ex)
						{
							//ロールバック
							callRollbackTransaction(tran);
							throw;
						}
						finally
						{
							tran.Dispose();
						}
					}
				}
			}
			else
			{
				//ロック解除処理
				if (locktype.basic == true)
				{
					if (lockTbl == null)
					{
						//ロック対象無しはエラー
						return -1;
					}
					else
					{
						try
						{
							//トランザクション開始
							tran = callBeginTransaction();
							//ロック解除処理
							retValue = setUsingFlg(lockTbl, locktype, chkFlg, lockMode, tran);
							//コミット
							callCommitTransaction(tran);
							lockTbl = null;
						}
						catch (Exception ex)
						{
							//ロールバック
							callRollbackTransaction(tran);
							throw;
						}
						finally
						{
							tran.Dispose();
						}
					}
				}
			}
			return retValue;
		}

		/// <summary>
		/// 使用中フラグの項目を更新する
		/// </summary>
		/// <param name="prmDt"></param>
		/// <returns></returns>

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To purchase the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
