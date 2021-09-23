//using System.Enum;
using Microsoft.VisualBasic.CompilerServices;


/// <summary>
/// コース削除
/// 登録済みのコースの物理削除＿論理削除を行う
/// </summary>
/// <remarks>
/// Author:2011/05/16//藤本
/// </remarks>
public class S01_0304 //frmコース削除
{

	#region  定数/変数

	//処理結果
	private const string ProcessResult_Normal = "正常終了"; //処理結果_正常
	private const string ProcessResult_Error = "エラー"; //処理結果_エラー
	private const string ProcessResult_LockChu = "ロック中"; //処理結果_ロック中
	private const string ProcessResult_DeleteNg = "削除不可"; //処理結果_削除不可
	private const string ProcessResult_LogicDeleteAlready = "削除済み"; //処理結果_論理削除済
	private const string ProcessResult_MiProcess = "未"; //処理結果_未処理

	private CrsMasterKeyKoumoku[] _taisyoCrsInfo; //_対象コース情報()

	private CdMasterGet_DA _cdMasterGet = new CdMasterGet_DA(); //_コードマスタ取得
	private int _physicsDeleteKanouPassageYearNum = 0; //_物理削除可能経過年数

	#endregion

	#region  列挙

	/// <summary>
	/// コース削除一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum CrsDelete_Koumoku : int //コース削除_項目
	{
		[Value("No")] no = 0, //No
		[Value("コースコード")] crsCd, //
		[Value("年")] year, //
		[Value("季_検索用")] season_SearchFor, //
		[Value("季")] season_DisplayFor, //
		[Value("コース名")] crsName, //
		[Value("登録日付")] entryYmd, //
		[Value("削除日付")] deleteYmd, //
		[Value("結果")] result, //
		[Value("ステータス")] status //
	}

	#endregion

	#region  プロパティ

	public CrsMasterKeyKoumoku[] TaisyoCrsInfo //対象コース情報()
	{

		set
		{
			_taisyoCrsInfo = value;
		}

	}

	#endregion

	#region  イベント

	#region  画面

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmCrsDelete_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) //frmコース削除_FormClosing(sender,e)
	{

		this.Owner.Show();
		this.Owner.Activate();

	}

	/// <summary>
	/// 画面ロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmCrsDelete_Load(object sender, System.EventArgs e) //frmコース削除_Load(sender,e)
	{

		DataTable systemCodeTable = new DataTable(); //systemCodeTable
		try
		{
			//コントロールの初期化
			this.setControlInitialize();

			//グリッド初期化
			this.initializeGrid();

			//コース検索にて選択されたコース情報を一覧に表示
			this.setCourseList();

			//物理削除可能経過年数の取得
			systemCodeTable = this._cdMasterGet.GetCodeMasterDataSystem(SystemSetType.physicsDeleteKanouPassageYearNum);
			if (systemCodeTable.Rows.Count == 1)
			{
				this._physicsDeleteKanouPassageYearNum = System.Convert.ToInt32(systemCodeTable.Rows(0).Item(ComboBoxCdType.NAIYO_1));
			}

			//論理削除を選択
			this.rdoLogic.Checked = true;
			this.rdoLogic.Focus();

		}
		catch (OracleException ex)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0006", ex.Number.ToString)
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
		}
	}

	#endregion

	#region  ラジオボタン

	/// <summary>
	/// 論理削除ボタン変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void rdoLogic_CheckedChanged(System.Object sender, System.EventArgs e) //rdo論理_CheckedChanged(sender,e)
	{

		this.lblDeleteRiyuu.Visible = this.rdoLogic.Checked;
		this.txtDeleteRiyuu.Visible = this.rdoLogic.Checked;

	}

	#endregion

	#region  ボタン

	#region  F2:戻るボタン

	/// <summary>
	/// F2:戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();

	}

	#endregion

	#region  F10:実行ボタン

	/// <summary>
	/// F10:実行ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		int normalKensu = 0; //正常件数
		int errorKensu = 0; //エラー件数
		CrsMasterKeyKoumoku[] lockInfo = null; //ロック情報()

		try
		{
			//カーソルを処理中に変更
			this.Cursor = Cursors.WaitCursor;

			//背景色の初期化
			//TODO:共通フォームの対応待ち(谷岡コメント化)
			//Call MyBase.clearErrorUmu()

			//入力チェック
			if (this.rdoLogic.Checked)
			{
				if (this.checkInputData() == false)
				{
					goto endOfTry;
				}
			}

			//削除チェック()
			this.checkExecData();

			//ロックチェック
			if (this.lockCourseMst(ref lockInfo) == false)
			{
				this.relockCourseMst(lockInfo);
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");

				goto endOfTry;
			}

			//実行確認メッセージ
			if (dispExecMessage() == false)
			{
				relockCourseMst(lockInfo);
				goto endOfTry;
			}

			//削除処理
			courseDeleteMain(ref normalKensu, ref errorKensu);

			//完了メッセージ
			if (normalKensu > 0 && errorKensu == 0)
			{
				//TODO:共通変更対応
				//メッセージ出力.messageDisp("0018")
				createFactoryMsg.messageDisp("0018");
			}
			else
			{
				//TODO:共通変更対応
				//メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");
			}

		}
		catch (OracleException ex)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0006", ex.Number.ToString)
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());

		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");

		}
		finally
		{
			//レコードロック解除
			this.relockCourseMst(lockInfo);

			//カーソルを戻す
			this.Cursor = Cursors.Default;
		}
	endOfTry:
		1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.

	}

	#endregion

	#endregion

	#endregion

	#region  メソッド

	#region  初期化

	/// <summary>
	/// コース削除グリッドの初期設定
	/// </summary>
	/// <remarks></remarks>
	private void initializeGrid()
	{

		object with_1 = this.grdCrsDelete;
		//№
		with_1.Cols(CrsDelete_Koumoku.no).Name = "NO";
		with_1.Cols(CrsDelete_Koumoku.no).Caption = getEnumAttrValue(CrsDelete_Koumoku.no);
		with_1.Cols(CrsDelete_Koumoku.no).Width = 40;
		//コースコード
		with_1.Cols(CrsDelete_Koumoku.crsCd).Name = "CRS_CD";
		with_1.Cols(CrsDelete_Koumoku.crsCd).Caption = getEnumAttrValue(CrsDelete_Koumoku.crsCd);
		with_1.Cols(CrsDelete_Koumoku.crsCd).Width = 120;
		//年
		with_1.Cols(CrsDelete_Koumoku.year).Name = "YMD";
		with_1.Cols(CrsDelete_Koumoku.year).Caption = getEnumAttrValue(CrsDelete_Koumoku.year);
		with_1.Cols(CrsDelete_Koumoku.year).Width = 50;
		//季
		with_1.Cols(CrsDelete_Koumoku.season_SearchFor).Name = "SEASON";
		with_1.Cols(CrsDelete_Koumoku.season_SearchFor).Caption = getEnumAttrValue(CrsDelete_Koumoku.season_SearchFor);
		with_1.Cols(CrsDelete_Koumoku.season_SearchFor).Visible = false;
		//季（表示用）
		with_1.Cols(CrsDelete_Koumoku.season_DisplayFor).Name = "SEASON_DISP";
		with_1.Cols(CrsDelete_Koumoku.season_DisplayFor).Caption = "季";
		with_1.Cols(CrsDelete_Koumoku.season_DisplayFor).Width = 100;
		//コース名
		with_1.Cols(CrsDelete_Koumoku.crsName).Name = "CRS_NAME";
		with_1.Cols(CrsDelete_Koumoku.crsName).Caption = getEnumAttrValue(CrsDelete_Koumoku.crsName);
		with_1.Cols(CrsDelete_Koumoku.crsName).Width = 300;
		//登録日
		with_1.Cols(CrsDelete_Koumoku.entryYmd).Name = "ENTRY_DATE";
		with_1.Cols(CrsDelete_Koumoku.entryYmd).Caption = getEnumAttrValue(CrsDelete_Koumoku.entryYmd);
		with_1.Cols(CrsDelete_Koumoku.entryYmd).Width = 90;
		//削除日
		with_1.Cols(CrsDelete_Koumoku.deleteYmd).Name = "DELETE_DATE";
		with_1.Cols(CrsDelete_Koumoku.deleteYmd).Caption = getEnumAttrValue(CrsDelete_Koumoku.deleteYmd);
		with_1.Cols(CrsDelete_Koumoku.deleteYmd).Width = 90;
		//結果
		with_1.Cols(CrsDelete_Koumoku.result).Name = "RESULT";
		with_1.Cols(CrsDelete_Koumoku.result).Caption = getEnumAttrValue(CrsDelete_Koumoku.result);
		with_1.Cols(CrsDelete_Koumoku.result).Width = 100;
		//ステータス
		with_1.Cols(CrsDelete_Koumoku.status).Name = "CRS_STATUS";
		with_1.Cols(CrsDelete_Koumoku.status).Caption = getEnumAttrValue(CrsDelete_Koumoku.status);
		with_1.Cols(CrsDelete_Koumoku.status).Visible = false;

	}

	/// <summary>
	/// コース削除初期表示
	/// </summary>
	/// <remarks></remarks>
	private void setCourseList()
	{

		DataTable dtCrsDelete = new DataTable(); //dtコース削除

		dtCrsDelete.Columns.Add("NO");
		dtCrsDelete.Columns.Add("CRS_CD");
		dtCrsDelete.Columns.Add("YMD");
		dtCrsDelete.Columns.Add("SEASON");
		dtCrsDelete.Columns.Add("SEASON_DISP");
		dtCrsDelete.Columns.Add("CRS_NAME");
		dtCrsDelete.Columns.Add("ENTRY_DATE");
		dtCrsDelete.Columns.Add("DELETE_DATE");
		dtCrsDelete.Columns.Add("RESULT");
		dtCrsDelete.Columns.Add("CRS_STATUS");

		for (int row = 0; row <= this._taisyoCrsInfo.Length - 1; row++)
		{
			DataRow drCrsInfo = dtCrsDelete.NewRow; //drコース情報

			CrsMasterKeyKoumoku with_1 = this._taisyoCrsInfo[row];
			drCrsInfo["NO"] = row + 1;
			drCrsInfo["CRS_CD"] = with_1.CrsCd;
			drCrsInfo["YMD"] = with_1.Year;
			drCrsInfo["SEASON"] = with_1.Season;
			drCrsInfo["SEASON_DISP"] = with_1.Season_DisplayFor;
			drCrsInfo["CRS_NAME"] = with_1.CrsName;
			drCrsInfo["ENTRY_DATE"] = with_1.EntryDay;
			drCrsInfo["DELETE_DATE"] = with_1.DeleteDay;
			drCrsInfo["RESULT"] = "";
			drCrsInfo["CRS_STATUS"] = with_1.CrsStatus;

			dtCrsDelete.Rows.Add(drCrsInfo);
		}

		this.grdCrsDelete.DataSource = dtCrsDelete;

	}

	/// <summary>
	/// コントロールの初期化
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitialize()
	{

		this.txtDeleteRiyuu.Text = string.Empty;

	}

	#endregion

	#region  入力チェック

	/// <summary>
	/// 入力データのチェックを行う
	/// </summary>
	/// <remarks></remarks>
	private bool checkInputData()
	{

		string msgId = ""; //msgId
		string msgStr = ""; //msgStr
		Control errCtl = null; //errCtl

		//論理削除のみチェック
		if (this.rdoPhysics.Checked == true)
		{
			return true;
		}

		//必須入力チェック
		if (Strings.Trim(System.Convert.ToString(this.txtDeleteRiyuu.Text)) == string.Empty)
		{
			this.txtDeleteRiyuu.ExistError = true;
			errCtl = txtDeleteRiyuu;
			msgId = "0014";
			msgStr = System.Convert.ToString(this.lblDeleteRiyuu.Text);
		}

		if (ReferenceEquals(errCtl, null) == false)
		{
			errCtl.Focus();
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp(msgId, msgStr)
			createFactoryMsg.messageDisp(msgId, msgStr);
			return false;
		}

		return true;

	}

	#endregion

	#region  削除チェック

	/// <summary>
	/// 削除チェックを行う
	/// </summary>
	/// <remarks></remarks>
	private void checkExecData()
	{

		int rowIdx = 0; //rowIdx

		object with_1 = this.grdCrsDelete;
		//処理結果「正常」以外のステータスをクリア
		for (rowIdx = with_1.Rows.Fixed; rowIdx <= with_1.Rows.Count - 1; rowIdx++)
		{
			//結果が設定されているデータはスキップ
			if (System.Convert.ToString(with_1.GetData(rowIdx, CrsDelete_Koumoku.result)) != ProcessResult_Normal)
			{
				with_1.SetData(rowIdx, CrsDelete_Koumoku.result, "");

				//論理削除のみチェック
				if (this.rdoLogic.Checked == true)
				{
					//削除日付が入っていたら、対象外
					if (with_1.GetData(rowIdx, CrsDelete_Koumoku.deleteYmd).ToString() != string.Empty)
					{
						with_1.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_LogicDeleteAlready);
					}
				}
				//物理削除のみチェック
				if (this.rdoPhysics.Checked == true)
				{
					//ステータスが『パンフ作成完了』かつ登録日から指定年未満の場合削除不可
					if (System.Convert.ToInt32(with_1.GetData(rowIdx, CrsDelete_Koumoku.status)) == CrsStatusType.pamphCreateDone &&)
					{
						System.Convert.ToDateTime(with_1.GetData(rowIdx, CrsDelete_Koumoku.entryYmd)).AddYears(this._physicsDeleteKanouPassageYearNum) >= DateAndTime.Today;
						with_1.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_DeleteNg);
					}
				}
			}
		}

	}

	#endregion

	#region  確認メッセージ

	/// <summary>
	/// 実行確認メッセージを表示する
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool dispExecMessage()
	{

		int execKanouLineNum = 0; //実行可能行数
		int execNgLineNum = 0; //実行不可行数
		string msgId = ""; //msgId
		string deleteType = string.Empty; //deleteType

		for (int row = 1; row <= this.grdCrsDelete.Rows.Count - 1; row++)
		{

			if (System.Convert.ToString(this.grdCrsDelete.GetData(row, CrsDelete_Koumoku.result)) == "")
			{
				execKanouLineNum++;
			}
			else if (System.Convert.ToString(this.grdCrsDelete.GetData(row, CrsDelete_Koumoku.result)) == ProcessResult_DeleteNg)
			{
				execNgLineNum++;
			}
		}

		if (execKanouLineNum == 0)
		{
			if (execNgLineNum == 0)
			{
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0036")
				createFactoryMsg.messageDisp("0036");
			}
			else
			{
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0036");
			}
			return false;
		}

		if (execNgLineNum == 0)
		{
			msgId = "0031";
		}
		else
		{
			msgId = "0034";
		}

		//削除方法の取得
		if (this.rdoLogic.Checked)
		{
			deleteType = System.Convert.ToString(this.rdoLogic.Text);
		}
		else if (this.rdoPhysics.Checked)
		{
			deleteType = System.Convert.ToString(this.rdoPhysics.Text);
		}

		//If メッセージ出力.messageDisp(msgId, deleteType & "削除") <> MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp(msgId, deleteType + "削除") != MsgBoxResult.Ok)
		{
			return false;
		}

		return true;

	}

	#endregion

	#region  コースマスタロック

	/// <summary>
	/// コースマスタロック処理
	/// </summary>
	/// <param name="lockInfo"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool lockCourseMst(ref CrsMasterKeyKoumoku[] lockInfo)
	{

		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		CrsState retValue = null; //retValue
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報
		string lockUser = ""; //lockUser

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		for (int row = 1; row <= this.grdCrsDelete.Rows.Count - 1; row++)
		{
			//処理対象（処理結果が空白）のレコード以外はスキップ
			if (System.Convert.ToString(this.grdCrsDelete.GetData(row, CrsDelete_Koumoku.result)) != "")
			{
				continue;
			}

			CrsMasterKeyKoumoku lockTaisyoLineKeyKoumoku = new CrsMasterKeyKoumoku(); //ロック対象行キー項目
			lockTaisyoLineKeyKoumoku.Teiki_KikakuKbn = _taisyoCrsInfo[row - 1].Teiki_KikakuKbn;
			lockTaisyoLineKeyKoumoku.CrsCd = _taisyoCrsInfo[row - 1].CrsCd;
			lockTaisyoLineKeyKoumoku.Year = _taisyoCrsInfo[row - 1].Year;
			lockTaisyoLineKeyKoumoku.Season = _taisyoCrsInfo[row - 1].Season;
			lockTaisyoLineKeyKoumoku.KaiteiDay = _taisyoCrsInfo[row - 1].KaiteiDay.Replace("/", "");
			lockTaisyoLineKeyKoumoku.InvalidFlg = _taisyoCrsInfo[row - 1].InvalidFlg;

			retValue = clsCrsLock.ExecuteCourseLockMain(lockTaisyoLineKeyKoumoku, userInfo, lockUser);

			if (retValue == CrsState.lockFailure)
			{
				this.grdCrsDelete.SetData(row, CrsDelete_Koumoku.result, ProcessResult_Error);
			}
			else if (retValue == CrsState.lockChu)
			{
				this.grdCrsDelete.SetData(row, CrsDelete_Koumoku.result, System.Convert.ToString(ProcessResult_LockChu + "[" + lockUser + "]"));
			}
			else if (retValue == CrsState.lockSuccess)
			{
				if (ReferenceEquals(lockInfo, null) == true)
				{
					lockInfo = new CrsMasterKeyKoumoku[1];
				}
				else
				{
					Array.Resize(ref lockInfo, lockInfo.Length + 1);
				}
				lockInfo[lockInfo.Length - 1] = lockTaisyoLineKeyKoumoku;
			}
		}
		return true;
	}

	#endregion

	#region  コースマスタロック解除

	/// <summary>
	/// コースマスタロック解除処理
	/// </summary>
	/// <param name="lockInfo"></param>
	/// <remarks></remarks>
	private void relockCourseMst(CrsMasterKeyKoumoku[] lockInfo)
	{

		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報

		if (ReferenceEquals(lockInfo, null) == true)
		{
			return;
		}

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		for (int idx = 0; idx <= lockInfo.Length - 1; idx++)
		{
			clsCrsLock.ExecuteCourseLockReleaseMain(lockInfo[idx], userInfo);
		}

	}

	#endregion

	#region  削除処理

	/// <summary>
	/// コース削除
	/// </summary>
	/// <remarks></remarks>
	private void courseDeleteMain(ref int normalKensu, ref int errorKensu)
	{

		string execReturnValue = string.Empty; //execReturnValue
		string srvName = string.Empty; //srvName
		string rName = string.Empty; //rName
		string[] logMsg = new string[4]; //logMsg(3)

		errorKensu = 0;
		normalKensu = 0;
		logMsg[0] = System.Convert.ToString(this.setTitle);

		CdMasterGet_DA cmDA = new CdMasterGet_DA(); //cmDA
		DataTable dt = null; //dt
		dt = cmDA.GetCodeMasterAllData(CdBunruiType.systemSet, false, true, false, false, "CODE_VALUE='" + CommonType_MojiColValue.SystemSetType_Value(FixedCd.SystemSetType.rosenzuImageFolderName) + "'");
		if (dt.Rows.Count > 0)
		{
			srvName = dt.Rows(0).Item(2).ToString();
			if (!srvName[srvName.Length - 1].Equals("\\"))
			{
				srvName += "\\";
			}
		}

		for (int rowIdx = 1; rowIdx <= this.grdCrsDelete.Rows.Count - this.grdCrsDelete.Rows.Fixed; rowIdx++)
		{
			//処理対象外行はスキップ
			if (System.Convert.ToString(this.grdCrsDelete.GetData(rowIdx, CrsDelete_Koumoku.result)) != string.Empty)
			{
				continue;
			}

			logMsg[1] = System.Convert.ToString(this._taisyoCrsInfo[rowIdx - 1].Teiki_KikakuKbn);
			logMsg[1] += System.Convert.ToString(this._taisyoCrsInfo[rowIdx - 1].CrsCd);
			logMsg[1] += System.Convert.ToString(this._taisyoCrsInfo[rowIdx - 1].Year);
			logMsg[1] += System.Convert.ToString(this._taisyoCrsInfo[rowIdx - 1].Season);
			logMsg[1] += System.Convert.ToString(this._taisyoCrsInfo[rowIdx - 1].InvalidFlg.ToString());

			try
			{
				//削除処理
				if (this.rdoLogic.Checked)
				{
					execReturnValue = this.execCourseUpdate(rowIdx - 1);
				}
				else if (this.rdoPhysics.Checked)
				{
					execReturnValue = this.execCourseDelete(rowIdx - 1, ref rName);
					if (!rName.Equals(string.Empty))
					{
						IO.File.Delete(srvName + this._taisyoCrsInfo[rowIdx - 1].Year + "_" + this._taisyoCrsInfo[rowIdx - 1].Season_DisplayFor + "_" + this._taisyoCrsInfo[rowIdx - 1].CrsCd + "_" + this._taisyoCrsInfo[rowIdx - 1].InvalidFlg.ToString() + "_" + rName);
					}
				}

				if (execReturnValue == string.Empty)
				{
					//一覧に結果を表示
					this.grdCrsDelete.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_Normal);
					normalKensu++;
					logMsg[2] = string.Empty;
					if (this.rdoLogic.Checked)
					{
						//Call outputLog(ログ種別タイプ.操作ログ, 処理種別タイプ.論理削除, logMsg)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.logicDelete, "コース削除", logMsg);
					}
					else if (this.rdoPhysics.Checked)
					{
						//Call outputLog(ログ種別タイプ.操作ログ, 処理種別タイプ.物理削除, logMsg)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.physicsDelete, "コース削除", logMsg);
					}

				}
				else
				{
					this.grdCrsDelete.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_Error);
					errorKensu++;
					logMsg[2] = execReturnValue;
					if (this.rdoLogic.Checked)
					{
						//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.論理削除, logMsg)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.logicDelete, "コース削除", logMsg);
					}
					else if (this.rdoPhysics.Checked)
					{
						//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.物理削除, logMsg)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.physicsDelete, "コース削除", logMsg);
					}
				}

			}
			catch (OracleException ex)
			{
				this.grdCrsDelete.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_Error);
				logMsg[2] = System.Convert.ToString(ex.Message);
				if (this.rdoLogic.Checked)
				{
					//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.論理削除, logMsg)
					createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.logicDelete, "コース削除", logMsg);
				}
				else if (this.rdoPhysics.Checked)
				{
					//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.物理削除, logMsg)
					createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.physicsDelete, "コース削除", logMsg);
				}
				errorKensu++;

			}
			catch (Exception)
			{
				this.grdCrsDelete.SetData(rowIdx, CrsDelete_Koumoku.result, ProcessResult_Error);
				if (this.rdoLogic.Checked)
				{
					//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.論理削除, logMsg)
					createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.logicDelete, "コース削除", logMsg);
				}
				else if (this.rdoPhysics.Checked)
				{
					//Call outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.物理削除, logMsg)
					createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.physicsDelete, "コース削除", logMsg);
				}
				errorKensu++;
			}

		}

	}

	/// <summary>
	/// 論理削除
	/// </summary>
	/// <param name="taishoIdx"></param>
	/// <returns>ログの内容３に出力する値を返す</returns>
	/// <remarks></remarks>
	private string execCourseUpdate(int taishoIdx)
	{

		CrsMasterKeyKoumoku crsMasterInfo = new CrsMasterKeyKoumoku(); //コースマスタ情報
		crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA(); //clsコースマスタ操作_DA
		OracleTransaction oraTran = null; //oraTran
		int maxInvalidFlg = 0; //maxInvalidFlg
		Hashtable paramInfo = new Hashtable(); //paramInfo


		crsMasterInfo = this._taisyoCrsInfo[taishoIdx];

		try
		{
			oraTran = clsCrsMasterOperation_DA.beginTransaction;

			//無効フラグが99を超えるとエラー
			maxInvalidFlg = System.Convert.ToInt32(clsCrsMasterOperation_DA.selectMaxInvalidFlg(crsMasterInfo));
			if (maxInvalidFlg >= 99)
			{
				return "InvalidFlgが99までExistenceしています。";
			}

			paramInfo.Add("NewInvalidFlg", maxInvalidFlg + 1);
			paramInfo.Add("DeleteRiyuu", this.txtDeleteRiyuu.Text);

			//論理削除処理
			if (clsCrsMasterOperation_DA.updateTable_deleteDate(oraTran, crsMasterInfo, paramInfo) == false)
			{
				clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
				return ProcessKindType.logicDelete.ToString() + "に失敗しました。";
			}

			clsCrsMasterOperation_DA.commitTransaction(oraTran);

		}
		catch (OracleException ex)
		{
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			throw (ex);

		}
		catch (Exception ex)
		{
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			throw (ex);

		}
		finally
		{
			oraTran.Dispose();
		}

		return string.Empty;

	}

	/// <summary>
	/// 物理削除
	/// </summary>
	/// <param name="taishoIdx"></param>
	/// <returns>ログの内容３に出力する値を返す</returns>
	/// <remarks></remarks>
	private string execCourseDelete(int taishoIdx, ref string file)
	{

		CrsMasterKeyKoumoku crsMasterInfo = new CrsMasterKeyKoumoku(); //コースマスタ情報
		crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA(); //clsコースマスタ操作_DA
		OracleTransaction oraTran = null; //oraTran
		CrsDelete_DA clsCrsDelete_DA = new CrsDelete_DA(); //clsコース削除_DA

		crsMasterInfo = this._taisyoCrsInfo[taishoIdx];

		try
		{
			file = System.Convert.ToString(clsCrsDelete_DA.getRosenzuFile(crsMasterInfo));

			oraTran = clsCrsMasterOperation_DA.beginTransaction;

			if (clsCrsMasterOperation_DA.deleteTableBeforeInsert(oraTran, crsMasterInfo) == false)
			{
				clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
				return ProcessKindType.physicsDelete.ToString() + "に失敗しました。";
			}

			clsCrsMasterOperation_DA.commitTransaction(oraTran);

		}
		catch (OracleException ex)
		{
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			throw (ex);

		}
		catch (Exception ex)
		{
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			throw (ex);

		}
		finally
		{
			oraTran.Dispose();
		}

		return string.Empty;

	}


	#endregion

	#endregion

	#region 共通対応
	protected override void StartupOrgProc()
	{
		//フォーム起動時のフッタの各ボタンの設定
		F1Key_Visible = false; // F1:未使用
		F2Key_Visible = true; // F2:戻る
		F3Key_Visible = false; // F3:未使用
		F4Key_Visible = false; // F4:未使用
		F5Key_Visible = false; // F5:未使用
		F6Key_Visible = false; // F6:未使用
		F7Key_Visible = false; // F7:未使用
		F8Key_Visible = false; // F8:未使用
		F9Key_Visible = false; // F9:未使用
		F10Key_Visible = true; // F10:実行
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用
		F10Key_Text = "F10:実行";
	}
	#endregion

}