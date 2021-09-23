//using System.Enum;
using hatobus.DevelopSystem.NewCommon;
using Microsoft.VisualBasic.CompilerServices;


public class S01_0302 //frmコースコピー
{

	#region 定数/変数
	private CrsMasterKeyKoumoku[] _taisyoCrsInfo; //_対象コース情報()
	private string _kaiteiYmd_EvacuateArea; //_改定日付_退避域
	private string _kaiteiYear_EvacuateArea; //_改定年_退避域

	//処理結果
	private const string ProcessResult_Normal = "正常終了"; //処理結果_正常
	private const string ProcessResult_Overlap = "重複"; //処理結果_重複
	private const string ProcessResult_Existence = "存在"; //処理結果_存在
	private const string ProcessResult_Error = "エラー"; //処理結果_エラー
	private const string ProcessResult_LockChu = "ロック中"; //処理結果_ロック中
	private const string ProcessResult_MiProcess = "未"; //処理結果_未処理

	// ダイヤ最大数
	private const int DiaMaxCount = 100;

	#endregion

	#region 列挙
	/// <summary>
	/// 検索結果一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum CrsList_Koumoku : int //コース一覧_項目
	{
		[Value("No")] no = 1,
		[Value("コースコード")] crsCd,
		[Value("年")] year,
		[Value("季")] season,
		[Value("コース名")] crsName,
		[Value("設定有無")] setUmu,
		[Value("結果")] result
	}

	#endregion

	#region プロパティ
	public CrsMasterKeyKoumoku[] TaisyoCrsInfo //対象コース情報()
	{
		set
		{
			_taisyoCrsInfo = value;
		}
	}
	#endregion

	#region イベント

	/// <summary>
	/// コースコピー画面_Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmCrsCopy_Load(object sender, System.EventArgs e) //frmコースコピー_Load(sender,e)
	{

		//タイトル表示

		try
		{
			//コンボボックス初期化
			InitiarizeComboBox();

			//グリッド初期化
			InitiarizeGrid();

			//運行日の表示非表示設定
			if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel))
			{
				this.txtUnkouDayFrom.Visible = false;
				this.txtUnkouDayTo.Visible = false;
				this.lblUnkouDay.Visible = false;
				this.lblFromTo.Visible = false;
			}
			//ダイヤ設定有無の表示非表示設定
			if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel))
			{
				grdCrsList.Cols(CrsList_Koumoku.setUmu).Visible = false;
			}

			//画面クリア
			this.txtYear.Value = DateAndTime.Today.Year;
			this.cmbSeason.SelectedIndex = 0;
			this.txtUnkouDayFrom.Value = null;
			this.txtUnkouDayTo.Value = null;
			setUnkouDay();

			//コース検索にて選択されたコース情報を一覧に表示
			setCourseList();

		}
		catch (OracleException ex)
		{
			//TODO:共通変更対応
			//'Call メッセージ出力.messageDisp("0006", ex.Number.ToString)
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());
			this.Close();
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//'Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
			this.Close();
		}

	}

	/// <summary>
	/// txt年_Validated
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void txtYear_Validated(System.Object sender, System.EventArgs e) //txt年_Validated(sendar,e)
	{
		setUnkouDay();
	}

	/// <summary>
	/// cmb季_SelectedValueChanged
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void cmbSeason_SelectedValueChanged(System.Object sender, System.EventArgs e) //cmb季_SelectedValueChanged(sender,e)
	{
		setUnkouDay();
	}

	#region フッタ

	/// <summary>
	/// F2キー（戻る）押下時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();
	}

	/// <summary>
	/// F10キー（実行）押下時イベント
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
			this.Cursor = Cursors.WaitCursor;
			//入力チェック
			if (checkInputData() == false)
			{
				return;
			}

			//実行時チェック
			if (checkExecData() == false)
			{
				return;
			}

			//レコードロック
			if (lockCourseMst(ref lockInfo) == false)
			{
				this.Cursor = Cursors.Default;
				//TODO:共通変更対応
				//'メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");
				return;
			}

			//実行確認メッセージ
			if (dispExecMessage() == false)
			{
				return;
			}

			//コピー処理
			if (courseCopyMain(ref normalKensu, ref errorKensu) == false)
			{
				this.Cursor = Cursors.Default;
				//TODO:共通変更対応
				//'メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");
				return;
			}

		}
		catch (OracleException ex)
		{
			//TODO:共通変更対応
			//'Call メッセージ出力.messageDisp("0006", ex.Number.ToString)
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//'Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
		}
		finally
		{
			//レコードロック解除
			relockCourseMst(lockInfo);
			this.Cursor = Cursors.Default;
		}

		//完了メッセージ
		if (normalKensu > 0 && errorKensu == 0)
		{
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0035", "コースコピー")
			createFactoryMsg.messageDisp("0035", "コースコピー");
		}
		else
		{
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0033")
			createFactoryMsg.messageDisp("0033");
		}

	}
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// コース一覧グリッドの初期設定
	/// </summary>
	/// <remarks></remarks>
	private void InitiarizeGrid()
	{
		object with_1 = grdCrsList;
		//№
		with_1.Cols(CrsList_Koumoku.no).Name = "NO";
		with_1.Cols(CrsList_Koumoku.no).Caption = getEnumAttrValue(CrsList_Koumoku.no);
		with_1.Cols(CrsList_Koumoku.no).Width = 40;
		with_1.Cols(CrsList_Koumoku.no).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.no).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		//コースコード
		with_1.Cols(CrsList_Koumoku.crsCd).Name = "CRS_CD";
		with_1.Cols(CrsList_Koumoku.crsCd).Caption = getEnumAttrValue(CrsList_Koumoku.crsCd);
		with_1.Cols(CrsList_Koumoku.crsCd).Width = 100;
		with_1.Cols(CrsList_Koumoku.crsCd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsCd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		//年
		with_1.Cols(CrsList_Koumoku.year).Name = "YMD";
		with_1.Cols(CrsList_Koumoku.year).Caption = getEnumAttrValue(CrsList_Koumoku.year);
		with_1.Cols(CrsList_Koumoku.year).Width = 50;
		with_1.Cols(CrsList_Koumoku.year).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.year).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		//季
		with_1.Cols(CrsList_Koumoku.season).Name = "SEASON_DISP";
		with_1.Cols(CrsList_Koumoku.season).Caption = getEnumAttrValue(CrsList_Koumoku.season);
		with_1.Cols(CrsList_Koumoku.season).Width = 100;
		with_1.Cols(CrsList_Koumoku.season).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.season).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		//コース名
		with_1.Cols(CrsList_Koumoku.crsName).Name = "CRS_NAME";
		with_1.Cols(CrsList_Koumoku.crsName).Caption = getEnumAttrValue(CrsList_Koumoku.crsName);
		with_1.Cols(CrsList_Koumoku.crsName).Width = 300;
		with_1.Cols(CrsList_Koumoku.crsName).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsName).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;
		//設定有無
		with_1.Cols(CrsList_Koumoku.setUmu).Name = "SETTING";
		with_1.Cols(CrsList_Koumoku.setUmu).Caption = getEnumAttrValue(CrsList_Koumoku.setUmu);
		with_1.Cols(CrsList_Koumoku.setUmu).Width = 80;
		with_1.Cols(CrsList_Koumoku.setUmu).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.setUmu).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		//結果
		with_1.Cols(CrsList_Koumoku.result).Name = "RESULT";
		with_1.Cols(CrsList_Koumoku.result).Caption = getEnumAttrValue(CrsList_Koumoku.result);
		with_1.Cols(CrsList_Koumoku.result).Width = 200;
		with_1.Cols(CrsList_Koumoku.result).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.result).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;


	}


	/// <summary>
	/// コース一覧初期表示
	/// </summary>
	/// <remarks></remarks>
	private void setCourseList()
	{

		DataTable dtCrsList = new DataTable(); //dtコース一覧

		dtCrsList.Columns.Add("NO");
		dtCrsList.Columns.Add("CRS_CD");
		dtCrsList.Columns.Add("YMD");
		dtCrsList.Columns.Add("SEASON_DISP");
		dtCrsList.Columns.Add("CRS_NAME");
		dtCrsList.Columns.Add("SETTING");
		dtCrsList.Columns.Add("RESULT");

		for (int row = 0; row <= _taisyoCrsInfo.Length - 1; row++)
		{

			string setUmu = ""; //設定有無

			try //Try
			{
				CrsCopy_DA clsCrsCopy_DA = new CrsCopy_DA(); //clsコースコピー_DA

				setUmu = ""; //設定有無
				if (clsCrsCopy_DA.isExistDiaSet(_taisyoCrsInfo[row]) == true)
				{
					setUmu = "有"; //設定有無
				} //EndIf

			}
			catch (OracleException ex)
			{
				throw (ex);
			}

			DataRow drCrsInfo = dtCrsList.NewRow; //drコース情報

			drCrsInfo["NO"] = row + 1;
			drCrsInfo["CRS_CD"] = _taisyoCrsInfo[row].CrsCd;
			drCrsInfo["YMD"] = _taisyoCrsInfo[row].Year;
			drCrsInfo["SEASON_DISP"] = _taisyoCrsInfo[row].Season_DisplayFor;
			drCrsInfo["CRS_NAME"] = _taisyoCrsInfo[row].CrsName;
			drCrsInfo["SETTING"] = setUmu;
			drCrsInfo["RESULT"] = "";

			dtCrsList.Rows.Add(drCrsInfo);
		} //Next

		grdCrsList.DataSource = dtCrsList;
	}

	/// <summary>
	/// コンボボックスの初期設定を行う
	/// </summary>
	/// <remarks></remarks>
	private void InitiarizeComboBox()
	{

		CdBunruiType cdBunrui = null; // コード分類

		//「季」コンボの初期値設定
		CdMasterGet_DA CdMasterGet_DA = new CdMasterGet_DA(); //clsコードマスタ取得_DA

		if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			cdBunrui = CdBunruiType.seasonMaster_Teiki;
		}
		else
		{
			cdBunrui = CdBunruiType.seasonMaster_Kikaku;
		}

		DataTable dtSeasonMaster = CdMasterGet_DA.GetCodeMasterData(cdBunrui); // dt季マスタ

		this.cmbSeason.DataSource = dtSeasonMaster;
		cmbSeason.ValueSubItemIndex = 0;
		cmbSeason.ListColumns(0).Visible = false;
		cmbSeason.ListHeaderPane.Visible = false;
		cmbSeason.TextSubItemIndex = 1;
		cmbSeason.ListColumns(1).Width = cmbSeason.Width;


	}

	/// <summary>
	/// 入力データのチェックを行う
	/// </summary>
	/// <remarks></remarks>
	private bool checkInputData()
	{

		string msgId = ""; //msgId
		string msgStr = ""; //msgStr
		Control errCtl = null; //errCtl

		//エラー内容クリア
		txtYear.ExistError = false;
		cmbSeason.ExistError = false;
		txtUnkouDayFrom.ExistError = false;
		txtUnkouDayTo.ExistError = false;

		//必須入力チェック
		if (ReferenceEquals(txtYear.Value, null) == true)
		{
			txtYear.ExistError = true;
			errCtl = txtYear;
			msgId = "0014";
			msgStr = "年";
		}

		if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			//運行日Toのみが入力されている場合
			if (ReferenceEquals(txtUnkouDayFrom.Value, null) == true && ReferenceEquals(txtUnkouDayTo.Value, null) == false)
			{
				txtUnkouDayFrom.ExistError = true;
				if (ReferenceEquals(errCtl, null) == true)
				{
					errCtl = txtUnkouDayFrom;
					msgId = "0014";
					msgStr = "運行日";
				}
			}
			//運行日Fromのみが入力されている場合
			if (ReferenceEquals(txtUnkouDayFrom.Value, null) == false && ReferenceEquals(txtUnkouDayTo.Value, null) == true)
			{
				txtUnkouDayTo.ExistError = true;
				if (ReferenceEquals(errCtl, null) == true)
				{
					errCtl = txtUnkouDayTo;
					msgId = "0014";
					msgStr = "運行日";
				}
			}
		}

		if (ReferenceEquals(errCtl, null) == false)
		{
			errCtl.Focus();
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp(msgId, msgStr)
			createFactoryMsg.messageDisp(msgId, msgStr);
			return false;
		}

		//入力値のチェック
		if (txtYear.Value < 2000M || txtYear.Value > 2099M)
		{
			txtYear.ExistError = true;
			txtYear.Focus();
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0004", "年")
			createFactoryMsg.messageDisp("0004", "年");
			return false;
		}

		if (ReferenceEquals(txtUnkouDayFrom.Value, null) == false)
		{
			if (txtUnkouDayFrom.Value > txtUnkouDayTo.Value)
			{
				txtUnkouDayFrom.ExistError = true;
				txtUnkouDayTo.ExistError = true;
				txtUnkouDayFrom.Focus();
				//TODO:共通変更対応
				//'メッセージ出力.messageDisp("0004", "運行日")
				createFactoryMsg.messageDisp("0004", "運行日");
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// 実行前チェックを行う
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool checkExecData()
	{

		CrsMasterKeyKoumoku taisyoCrsInfo = new CrsMasterKeyKoumoku(); //対象コース情報
		CrsCopy_DA clsCrsCopy_DA = new CrsCopy_DA(); //clsコースコピー_DA

		//処理結果「正常」以外のステータスをクリア
		for (int row = 1; row <= grdCrsList.Rows.Count - 1; row++)
		{
			//ステータスが設定されているデータはスキップ
			if (System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) != ProcessResult_Normal)
			{
				grdCrsList.SetData(row, CrsList_Koumoku.result, "");
			}
		}

		//一覧に表示されているコース内での重複チェックを行う
		//（更新後は年、季、改定日、無効フラグが同一となるため、コースコードが同一のデータは重複とする）
		for (int idx = 0; idx <= _taisyoCrsInfo.Length - 1; idx++)
		{
			//ステータスが設定されているデータはスキップ
			if (System.Convert.ToString(grdCrsList.GetData(idx + 1, CrsList_Koumoku.result)) != "")
			{
				continue;
			}

			for (int searchIdx = 0; searchIdx <= _taisyoCrsInfo.Length - 1; searchIdx++)
			{
				//同一行はスキップ
				if (idx == searchIdx)
				{
					continue;
				}
				//同一コースコードのデータが存在している場合
				if (_taisyoCrsInfo[idx].Teiki_KikakuKbn == _taisyoCrsInfo[searchIdx].Teiki_KikakuKbn &&)
				{
					_taisyoCrsInfo[idx].CrsCd = _taisyoCrsInfo[searchIdx].CrsCd;
					grdCrsList.SetData(idx + 1, CrsList_Koumoku.result, ProcessResult_Overlap);
				}
			}
		}

		DataTable dtKaiteiDay = clsCrsCopy_DA.GetKaiteiDayFromSeason(System.Convert.ToString(_taisyoCrsInfo[0].Teiki_KikakuKbn), System.Convert.ToString(cmbSeason.SelectedValue)); //dt改定日
		_kaiteiYmd_EvacuateArea = System.Convert.ToString(dtKaiteiDay[0].Item(0).ToString().Replace("/", ""));
		if (dtKaiteiDay[0].Item(1).ToString().Equals("1"))
		{
			_kaiteiYear_EvacuateArea = ((System.Convert.ToInt32(txtYear.Text)) - 1).ToString();
		}
		else
		{
			_kaiteiYear_EvacuateArea = System.Convert.ToString(txtYear.Text);
		}

		//実行後のキー項目と同一キーレコードの存在チェックを行う
		for (int idx = 0; idx <= _taisyoCrsInfo.Length - 1; idx++)
		{
			//ステータスが設定されているデータはスキップ
			if (System.Convert.ToString(grdCrsList.GetData(idx + 1, CrsList_Koumoku.result)) != "")
			{
				continue;
			}

			try
			{
				//選択された季コードより改定日付を取得
				taisyoCrsInfo.Teiki_KikakuKbn = _taisyoCrsInfo[idx].Teiki_KikakuKbn;
				taisyoCrsInfo.CrsCd = _taisyoCrsInfo[idx].CrsCd;
				taisyoCrsInfo.Year = txtYear.Text;
				taisyoCrsInfo.Season = System.Convert.ToString(cmbSeason.SelectedValue);
				taisyoCrsInfo.KaiteiDay = _kaiteiYear_EvacuateArea + _kaiteiYmd_EvacuateArea;

				if (clsCrsCopy_DA.isExistCourseMst(taisyoCrsInfo) == true)
				{
					grdCrsList.SetData(idx + 1, CrsList_Koumoku.result, ProcessResult_Existence);
				}
			}
			catch (OracleException ex)
			{
				throw (ex);
			}
		}

		return true;
	}

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

		for (int row = 1; row <= grdCrsList.Rows.Count - 1; row++)
		{

			if (System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) == "")
			{
				execKanouLineNum++;
			}
			else if (System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) == ProcessResult_Overlap ||)
			{
				System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) = ProcessResult_Existence;
				execNgLineNum++;
			}
		}

		if (execKanouLineNum == 0)
		{
			if (execNgLineNum == 0)
			{
				//TODO:共通変更対応
				//'メッセージ出力.messageDisp("0036")
				createFactoryMsg.messageDisp("0036");
			}
			else
			{
				//TODO:共通変更対応
				//'メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");
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
		//TODO:共通変更対応
		//'If メッセージ出力.messageDisp(msgId, "コースコピー") <> MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp(msgId, "コースコピー") != MsgBoxResult.Ok)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// コースコピー制御
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool courseCopyMain(ref int normalKensu, ref int errorKensu) //courseCopyMain(正常件数,エラー件数)
	{

		CrsMasterKeyKoumoku copyMotoCrsInfo = new CrsMasterKeyKoumoku(); //コピー元コース情報
		CrsMasterKeyKoumoku copySakiCrsInfo = new CrsMasterKeyKoumoku(); //コピー先コース情報
		string srvName = string.Empty; //srvName
		string[] logmsg = new string[3]; //logmsg(2)

		errorKensu = 0;
		normalKensu = 0;

		//処理対象行の処理結果に「未処理」を設定
		for (int row = 1; row <= grdCrsList.Rows.Count - 1; row++)
		{
			if (System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) == "")
			{
				grdCrsList.SetData(row, CrsList_Koumoku.result, ProcessResult_MiProcess);
			}
		}

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

		for (int copyMotoidx = 1; copyMotoidx <= grdCrsList.Rows.Count - 1; copyMotoidx++)
		{
			//処理対象外行はスキップ
			if (System.Convert.ToString(grdCrsList.GetData(copyMotoidx, CrsList_Koumoku.result)) != ProcessResult_MiProcess)
			{
				continue;
			}

			//コピー元コース情報取得
			copyMotoCrsInfo = _taisyoCrsInfo[copyMotoidx - 1]; //コピー元コース情報

			//コピー先コース情報取得
			copySakiCrsInfo.Teiki_KikakuKbn = _taisyoCrsInfo[0].Teiki_KikakuKbn;
			copySakiCrsInfo.CrsCd = System.Convert.ToString(grdCrsList.GetData(copyMotoidx, CrsList_Koumoku.crsCd));
			copySakiCrsInfo.Year = txtYear.Text;
			copySakiCrsInfo.Season = System.Convert.ToString(cmbSeason.SelectedValue);
			copySakiCrsInfo.KaiteiDay = _kaiteiYear_EvacuateArea + _kaiteiYmd_EvacuateArea;
			copySakiCrsInfo.InvalidFlg = 0;

			//コピー処理
			if (execCourseCopy(copyMotoCrsInfo, copySakiCrsInfo, srvName) == true)
			{

				//一覧に結果を表示
				grdCrsList.SetData(copyMotoidx, CrsList_Koumoku.result, ProcessResult_Normal);
				normalKensu++;
				logmsg[0] = System.Convert.ToString(this.setTitle);
				logmsg[1] = System.Convert.ToString(copyMotoCrsInfo.Teiki_KikakuKbn);
				logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.CrsCd);
				logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.Year);
				logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.Season);
				logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.InvalidFlg.ToString());
				logmsg[1] += "⇒";
				logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Teiki_KikakuKbn);
				logmsg[1] += System.Convert.ToString(copySakiCrsInfo.CrsCd);
				logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Year);
				logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Season);
				logmsg[1] += System.Convert.ToString(copySakiCrsInfo.InvalidFlg.ToString());
				//TODO:共通変更対応
				//'outputLog(ログ種別タイプ.操作ログ, 処理種別タイプ.コピー, logmsg)
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.copy, this.setTitle, logmsg);
			}
			else
			{
				grdCrsList.SetData(copyMotoidx, CrsList_Koumoku.result, ProcessResult_Error);
				errorKensu++;
				return false;
			}
		}

		return true;

	}

	private object execCourseCopy()
	{
		ByValcopySakiCrsInfoCrsMasterKeyKoumoku, ;
		ByValserverFolderNamestring)bool;

		CrsMasterOperation_DA clsCrsMasterOperation_DA = new CrsMasterOperation_DA(); //clsコースマスタ操作_DA
		OracleTransaction oraTran = null; //oraTran
		EntityOperation entity = new EntityOperation(Of CrsMasterEntity); //entity
		string[] logmsg = new string[3]; //logmsg(2)
		string fromName = string.Empty; //fromName
		string toName = string.Empty; //toName

		logmsg[0] = System.Convert.ToString(this.setTitle);
		logmsg[1] = System.Convert.ToString(copyMotoCrsInfo.Teiki_KikakuKbn);
		logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.CrsCd);
		logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.Year);
		logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.Season);
		logmsg[1] += System.Convert.ToString(copyMotoCrsInfo.InvalidFlg.ToString());
		logmsg[1] += "⇒";
		logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Teiki_KikakuKbn);
		logmsg[1] += System.Convert.ToString(copySakiCrsInfo.CrsCd);
		logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Year);
		logmsg[1] += System.Convert.ToString(copySakiCrsInfo.Season);
		logmsg[1] += System.Convert.ToString(copySakiCrsInfo.InvalidFlg.ToString());

		try
		{
			oraTran = clsCrsMasterOperation_DA.beginTransaction();
			//対象データをエンティティクラスに格納
			if (clsCrsMasterOperation_DA.getBasicInfoEntity(copyMotoCrsInfo, entity) == false)
			{
				return false;
			}

			if (!entity.EntityData(0).RosenzuFile.Value.Equals(string.Empty))
			{
				//コピー元ファイル名
				fromName = serverFolderName + copyMotoCrsInfo.Year + "_" + copyMotoCrsInfo.Season_DisplayFor + "_" + copyMotoCrsInfo.CrsCd + "_" + copyMotoCrsInfo.InvalidFlg.ToString() + "_" + entity.EntityData(0).RosenzuFile.Value;
			}

			//原価適用開始日の生成
			entity.EntityData(0).chgCostData(_kaiteiYear_EvacuateArea + _kaiteiYmd_EvacuateArea);

			//コピー時初期値の設定
			if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
			{
				//定期観光
				object with_1 = entity.EntityData(0);
				with_1.MotoCrsCd.Value = with_1.CrsCd.Value;
				with_1.CrsBunrui.Value = System.Convert.ToString(CrsBunruiType.continuation);
				with_1.YoyakuSystemRendoYmd.Value = null;
				with_1.WEBYoyakuSystemRendoYmd.Value = null;
				with_1.LastRendoMon_Teiki.Value = "";
				with_1.BulkOpenYmd.Value = null;
				with_1.DeleteDay.Value = "";
				with_1.DeleteRiyuu.Value = "";
			}
			else
			{
				//企画旅行
				object with_2 = entity.EntityData(0);
				with_2.MotoCrsCd.Value = with_2.CrsCd.Value;
				with_2.CrsStatus.Value = System.Convert.ToString(CrsStatusType.zouseiChu);
				with_2.CrsStatusUpdateDate.Value = null;
				with_2.CrsBunrui.Value = System.Convert.ToString(CrsBunruiType.continuation);
				with_2.CrsNaiyoKakuninFlg.Value = 0;
				with_2.CrsNaiyoKakuninYmd.Value = null;
				with_2.CostCyoseiFlg.Value = 0;
				with_2.CostCyoseiYmd.Value = null;
				with_2.ToriatukaiInfoAddFlg.Value = 0;
				with_2.ToriatukaiInfoAddYmd.Value = null;
				with_2.PamphIraiYmd.Value = null;
				with_2.YoyakuSystemRendoYmd.Value = null;
				with_2.WEBYoyakuSystemRendoYmd.Value = null;
				with_2.BulkOpenYmd.Value = null;
				with_2.LastRendoMon_Teiki.Value = "";
				with_2.DeleteDay.Value = "";
				with_2.DeleteRiyuu.Value = "";
			}

			//料金カレンダーのデータをクリア
			entity.EntityData(0).ChargeCalendarEntity.clear();

			//運行日情報の自動生成
			if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko) &&)
			{
				ReferenceEquals(txtUnkouDayFrom.Value, null) = System.Convert.ToBoolean(false &&);
				entity.EntityData(0).DiaSetEntity(0).Teiki_KikakuKbn.Value != "" &&;
				entity.EntityData(0).DiaEntity(0).Teiki_KikakuKbn.Value != "";
				setUnkouDayInfo(entity);
			}

			if (clsCrsMasterOperation_DA.InsertAllFromCrsMasterEntity(oraTran,)
			{
				CrsMasterOperation_DA.CrsMasterWritingMode.copy(,);
				entity(,);
				copySakiCrsInfo() == false);
				clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
				return false;
			}
			clsCrsMasterOperation_DA.commitTransaction(oraTran);

			if (!fromName.Equals(string.Empty))
			{
				//コピー先ファイル名
				toName = serverFolderName + copySakiCrsInfo.Year + "_" + this.cmbSeason.SelectedItem().SubItems[1].Value.ToString() + "_" + copySakiCrsInfo.CrsCd + "_" + copySakiCrsInfo.InvalidFlg.ToString() + "_" + entity.EntityData(0).RosenzuFile.Value;
				if (IO.File.Exists(toName))
				{
					IO.File.Delete(toName);
				}
				IO.File.Copy(fromName, toName);
			}
		}
		catch (OracleException ex)
		{
			logmsg[2] = System.Convert.ToString(ex.Message);
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			//TODO:共通変更対応
			//'outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.コピー, logmsg)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.copy, this.setTitle, logmsg);

			return false;
		}
		catch (Exception ex)
		{
			logmsg[2] = System.Convert.ToString(ex.Message);
			//TODO:共通変更対応
			//'outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.コピー, logmsg)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.copy, this.setTitle, logmsg);
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			return false;
		}

		return true;
	}

	/// <summary>
	/// コースマスタロック処理
	/// </summary>
	/// <param name="lockInfo"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool lockCourseMst(ref CrsMasterKeyKoumoku[] lockInfo) //lockCourseMst(ロック情報())
	{

		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		CrsState retValue = null; //retValue
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報
		string lockUser = ""; //lockUser

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		for (int row = 1; row <= grdCrsList.Rows.Count - 1; row++)
		{
			//処理対象（処理結果が空白）のレコード以外はスキップ
			if (System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.result)) != "")
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
				grdCrsList.SetData(row, CrsList_Koumoku.result, ProcessResult_Error);
			}
			else if (retValue == CrsState.lockChu)
			{
				grdCrsList.SetData(row, CrsList_Koumoku.result, System.Convert.ToString(ProcessResult_LockChu + "(" + lockUser + ")"));
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

	/// <summary>
	/// 運行日情報の自動設定を行う
	/// </summary>
	/// <remarks></remarks>
	private void setUnkouDayInfo(EntityOperation[] entity)
	{

		DataTable dtUnkouDayInfo = new DataTable(); //dt運行日情報
		DataTable dtShukujitsuList = null; //dt祝日リスト
		int diaMaxLine = 0; //ダイヤMax行
		CrsCopy_DA clsCrsCopy_DA = new CrsCopy_DA(); //clsコースコピー_DA
		DataRow[] selectRows = null; //selectRows()

		diaMaxLine = System.Convert.ToInt32(entity.EntityData(0).DiaEntity.EntityData.Length);

		//運行日編集用のワークカレンダーを作成
		object with_1 = dtUnkouDayInfo.Columns;
		with_1.Add("YMD", typeof(string));
		with_1.Add("WEEK", typeof(int));
		with_1.Add("HOLIDAY", typeof(int));
		with_1.Add("RANK", typeof(string));
		for (int idx = 1; idx <= DiaMaxCount; idx++)
		{
			with_1.Add("NO" + idx.ToString(), typeof(int));
		}

		DateTime taisyoYmd = System.Convert.ToDateTime(txtUnkouDayFrom.Value); //対象日付
		while (taisyoYmd <= txtUnkouDayTo.Value)
		{
			DataRow drUnkouDay = dtUnkouDayInfo.NewRow; // dr運行日
			drUnkouDay["YMD"] = taisyoYmd.ToString("yyyyMMdd");
			drUnkouDay["WEEK"] = taisyoYmd.DayOfWeek;
			drUnkouDay["HOLIDAY"] = 0;
			taisyoYmd = DateAndTime.DateAdd(DateInterval.Day, 1, taisyoYmd);
			dtUnkouDayInfo.Rows.Add(drUnkouDay);
		}

		//ワークカレンダーに祝日を設定
		dtShukujitsuList = clsCrsCopy_DA.getShukujitsuListFromCalendarMasterShukujitsu((System.Convert.ToDateTime(txtUnkouDayFrom.Value)).ToString("yyyyMMdd"), (System.Convert.ToDateTime(txtUnkouDayTo.Value)).ToString("yyyyMMdd"));

		for (int idxShukujitsu = 0; idxShukujitsu <= dtShukujitsuList.Rows.Count - 1; idxShukujitsu++)
		{
			for (int idxUnkouDay = 0; idxUnkouDay <= dtUnkouDayInfo.Rows.Count - 1; idxUnkouDay++)
			{
				if (dtUnkouDayInfo[idxUnkouDay].Item("YMD").Equals(dtShukujitsuList[idxShukujitsu].Item("YMD")))
				{
					dtUnkouDayInfo[idxUnkouDay].Item["HOLIDAY"] = 1;
				}
			}
		}

		//ダイヤ設定を元に料金ランクとダイヤ行№を設定
		for (int rowDiaSet = 0; rowDiaSet <= entity.EntityData(0).DiaSetEntity.EntityData.Length - 1; rowDiaSet++)
		{
			object with_2 = entity.EntityData(0).DiaSetEntity(rowDiaSet);
			for (int dayCnt = 0; dayCnt <= dtUnkouDayInfo.Rows.Count - 1; dayCnt++)
			{
				bool setNecessity = false; //設定要否

				if (System.Convert.ToInt32(dtUnkouDayInfo[dayCnt].Item("HOLIDAY")) == 1 && with_2.Unkou_SyukuWith.Value == 1)
				{
					setNecessity = true;
				}

				if (ReferenceEquals(with_2.DiaLineNo.Value, null) == false && System.Convert.ToInt32(with_2.DiaLineNo.Value) == 0)
				{
					setNecessity = true;
				}

				int yobi = System.Convert.ToInt32(dtUnkouDayInfo[dayCnt].Item("WEEK")); //曜日
				if (yobi == System.DayOfWeek.Sunday && with_2.Unkou_Sun.Value == 1 ||)
				{
					yobi = System.Convert.ToInt32(System.DayOfWeek.Monday && with_2.Unkou_Mon.Value == 1 ||);
					yobi = System.Convert.ToInt32(System.DayOfWeek.Tuesday && with_2.Unkou_Tue.Value == 1 ||);
					yobi = System.Convert.ToInt32(System.DayOfWeek.Wednesday && with_2.Unkou_Wed.Value == 1 ||);
					yobi = System.Convert.ToInt32(System.DayOfWeek.Thursday && with_2.Unkou_Thu.Value == 1 ||);
					yobi = System.Convert.ToInt32(System.DayOfWeek.Friday && with_2.Unkou_Fri.Value == 1 ||);
					yobi = System.Convert.ToInt32(System.DayOfWeek.Saturday && with_2.Unkou_Sat.Value == 1);

					if (!(System.Convert.ToInt32(dtUnkouDayInfo[dayCnt].Item("HOLIDAY")) == 1 && with_2.Unkou_SyukuWithout.Value == 1))
					{
						setNecessity = true;
					}
				}

				if (setNecessity == true)
				{
					//料金ランク設定
					if (Information.IsDBNull(dtUnkouDayInfo[dayCnt].Item("RANK")) == true && with_2.ChargeRank.Value != "")
					{
						dtUnkouDayInfo[dayCnt].Item["RANK"] = with_2.ChargeRank.Value;
					}
					//発着№設定
					if (ReferenceEquals(with_2.DiaLineNo.Value, null) == false)
					{
						if (with_2.DiaLineNo.Value == 0)
						{
							for (int idx = 1; idx <= diaMaxLine; idx++)
							{
								dtUnkouDayInfo[dayCnt].Item["NO" + idx.ToString()] = idx;
							}
						}
						else
						{
							for (int idx = 1; idx <= DiaMaxCount; idx++)
							{
								if (Information.IsDBNull(dtUnkouDayInfo[dayCnt].Item("NO" + idx.ToString())) == true)
								{
									dtUnkouDayInfo[dayCnt].Item["NO" + idx.ToString()] = with_2.DiaLineNo.Value;
									break;
								}
							}
						}
					}
				}
			}
		}

		//ワークカレンダーの有効行を元に料金カレンダーエンティティに書込み
		int idxChargeCalendar = 0; //idx料金カレンダ

		for (int idx = 0; idx <= dtUnkouDayInfo.Rows.Count - 1; idx++)
		{
			//料金ランク、発着№のいづれかが設定されているデータを出力対象とする
			if (Information.IsDBNull(dtUnkouDayInfo[idx].Item("RANK")) == false ||)
			{
				Information.IsDBNull(dtUnkouDayInfo[idx].Item("NO1")) = System.Convert.ToBoolean(false);
				if (idxChargeCalendar > 0)
				{
					ChargeCalendarEntity chargeCalendar = new ChargeCalendarEntity(); //料金カレンダ
					entity.EntityData(0).ChargeCalendarEntity.add(chargeCalendar);
				}

				object with_3 = entity.EntityData(0).ChargeCalendarEntity(idxChargeCalendar);
				with_3.Teiki_KikakuKbn.Value = _taisyoCrsInfo[0].Teiki_KikakuKbn;
				with_3.Ymd.Value = System.Convert.ToString(dtUnkouDayInfo[idx].Item("YMD"));
				if (Information.IsDBNull(dtUnkouDayInfo[idx].Item("RANK")) == false)
				{
					with_3.ChargeRank.Value = System.Convert.ToString(dtUnkouDayInfo[idx].Item("RANK"));
				}

				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO1")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO1"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO2")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO2"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO3")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO3"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO4")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO4"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO5")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO5"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO6")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO6"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO7")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO7"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO8")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO9"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO9")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO9"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO10")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO10"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO11")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO11"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO12")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO12"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO13")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO13"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO14")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO14"))
				//End If
				//If IsDBNull(dtUnkouDayInfo(idx).Item("NO15")) = False Then
				//    .DiaLineNo1.Value = CInt(dtUnkouDayInfo(idx).Item("NO15"))
				//End If
				// ↓ ダイヤ100件対応
				string itemId = string.Empty;
				for (int col = 1; col <= DiaMaxCount; col++)
				{

					itemId = string.Concat("NO", col.ToString());

					if (Information.IsDBNull(dtUnkouDayInfo[idx].Item(itemId)) == false)
					{
						with_3.DiaLineNo1.Value = System.Convert.ToInt32(dtUnkouDayInfo[idx].Item(itemId));
					}

				}

				idxChargeCalendar++;
			}
		}


	}

	/// <summary>
	/// 入力された年、季より運行日を自動設定する
	/// </summary>
	/// <remarks></remarks>
	private void setUnkouDay() //set運行日()
	{
		CrsCopy_DA clsCrsCopy_DA = new CrsCopy_DA(); //clsコースコピー_DA
		DateTime kaiteiDayFrom = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#"); //改定日From
		DateTime kaiteiDayTo = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#"); //改定日To

		//年、季が未入力の場合、なにもしない
		if (txtYear.Text == "" || cmbSeason.Text == "")
		{
			return;
		}

		if (System.Convert.ToInt32(txtYear.Text) < 1900 || System.Convert.ToInt32(txtYear.Text) > 2100)
		{
			return;
		}

		DataTable dtKaiteiDay = clsCrsCopy_DA.GetKaiteiDayFromSeason(_taisyoCrsInfo[0].Teiki_KikakuKbn, System.Convert.ToString(cmbSeason.SelectedValue)); //dt改定日

		if (dtKaiteiDay IsNot null && dtKaiteiDay.Rows.Count > 0)
		{
			string KaiteiDay = System.Convert.ToString(dtKaiteiDay[0].Item(0).ToString().Replace("/", "")); //改定日
			if (dtKaiteiDay[0].Item(1).ToString().Equals("1"))
			{
				kaiteiDayFrom = DateTime.Parse(((System.Convert.ToInt32(txtYear.Text)) - 1).ToString() + "/" + KaiteiDay.Substring(0, 2) + "/" + KaiteiDay.Substring(2, 2));
			}
			else
			{
				kaiteiDayFrom = DateTime.Parse(txtYear.Text + "/" + KaiteiDay.Substring(0, 2) + "/" + KaiteiDay.Substring(2, 2));
			}
			kaiteiDayTo = DateAndTime.DateAdd(DateInterval.Month, 3, kaiteiDayFrom);
		}

		txtUnkouDayFrom.Value = kaiteiDayFrom;
		txtUnkouDayTo.Value = kaiteiDayTo;

	}
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