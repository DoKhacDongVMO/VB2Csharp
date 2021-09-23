using System.ComponentModel;
//using System.Enum;

/// <summary>
/// コース検索
/// </summary>
/// <remarks></remarks>
public class S01_0305 //frmコース検索
{
	public S01_0305()
	{
		// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
		_chkKind2 = new List(Of CheckBoxEx);
		_chkKbn1_Teiki = new List(Of CheckBoxEx);
		_chkKbn2_Teiki = new List(Of CheckBoxEx);
		_chkKbn1_Kikaku = new List(Of CheckBoxEx);

	}

	#region 定数/変数
	private string _searchConditions_CrsHeadBunrui; //_検索条件_コース大分類
	private string _searchConditions_CrsCd; //_検索条件_コースコード
	private string _searchConditions_Year; //_検索条件_年
	private string _searchConditions_Season; //_検索条件_季
	private string _searchConditions_Status; //_検索条件_ステータス
	private string _searchConditions_CrsKind1; //_検索条件_コース種別1
	private string _searchConditions_CrsKind2; //_検索条件_コース種別2
	private string _searchConditions_CrsKind3; //_検索条件_コース種別3
	private string _searchConditions_CrsName; //_検索条件_コース名
	private string _searchConditions_CrsKbn1; //_検索条件_コース区分1
	private string _searchConditions_CrsKbn2; //_検索条件_コース区分2
	private string _searchConditions_CrsKbn3; //_検索条件_コース区分3
	private string _searchConditions_ManagementSec; //_検索条件_取扱部署

	private CrsMasterKeyKoumoku[] _selectionCrsInfo;
	private CrsMasterKeyKoumoku[] _displayTaisyoCrsInfo;
	private const string DEL_CRS_STYLE = "DEL_CRS";

	private string _teikiKikakuKbn; //_定期企画区分

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 62;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;
	/// <summary>
	/// 一覧表示パネルのTop座標
	/// </summary>
	public const int TopPnlList = 229;

	private List _chkKind2; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
	private List _chkKbn1_Teiki; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
	private List _chkKbn2_Teiki; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
	private List _chkKbn1_Kikaku; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

	#endregion

	#region 列挙 //#Region"列挙"
	/// <summary>
	/// 検索結果一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	public enum CrsList_Koumoku : int //コース一覧_項目
	{
		[Value("運行年")] year = 1,
		[Value("季")] season,
		[Value("季_表示用")] season_DisplayFor,
		[Value("コース番号")] crsCd,
		[Value("コース名称")] crsName,
		[Value("削除日付")] deleteYmd
	}

	#endregion

	#region プロパティ

	/// <summary>
	/// 条件GroupBoxの高さ
	/// </summary>
	/// <returns></returns>
	public int HeightGbxCondition
	{
		get
		{
			return this.gbxCondition.Height;
		}
		set
		{
			this.gbxCondition.Height = value;
		}
	}

	/// <summary>
	/// 条件GroupBoxの表示/非表示
	/// </summary>
	/// <returns></returns>
	public bool VisibleGbxCondition
	{
		get
		{
			return this.gbxCondition.Visible;
		}
		set
		{
			this.gbxCondition.Visible = value;
		}
	}
	#endregion

	#region イベント

	/// <summary>
	/// コース検索_Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void CrsSearch_Load(object sender, System.EventArgs e) //コース検索_Load(sender,e)
	{

		//検索ボタンの関連付け
		btnSearch.Click += btnCom_Click;

		//タイトル表示

		//コンボボックス選択値設定（定期/企画共通）
		InitiarizeComboBox();

		//グリッド書式設定
		InitializeGrid();

		//クリア
		try
		{
			clearScreen(this);
			clearScreen(this.gbxCondition);
			clearScreen(this.pnlList);
		}
		catch (Exception)
		{

		}

		//コース種別１表示切替
		setCrsKind1();

		//コントロールの表示切替
		setControlInitialize();

		//'ADD-20120402-検索条件「年」「季」の前回情報を設定↓
		setLastSearchInfo();

		//コース種別チェックボックスのコントロール配列生成
		this._chkKind2.Add(this.chkHigaeri_Kind);
		this._chkKind2.Add(this.chkStay_Kind);
		this._chkKind2.Add(this.chkR_Kind);
		//コース区分チェックボックスのコントロール配列生成
		this._chkKbn1_Teiki.Add(this.chkNoon);
		this._chkKbn1_Teiki.Add(this.chkNight);
		this._chkKbn2_Teiki.Add(this.chkTonai);
		this._chkKbn2_Teiki.Add(this.chkKogai);
		this._chkKbn1_Kikaku.Add(this.chkHigaeri_Kbn);
		this._chkKbn1_Kikaku.Add(this.chkR_CrsKbn1);
		this._chkKbn1_Kikaku.Add(this.chkStay_Kbn);
		this._chkKbn1_Kikaku.Add(this.chk2StayUp);
		this._chkKbn1_Kikaku.Add(this.chkNightLine);
		this._chkKbn1_Kikaku.Add(this.chkShip);

		this.ActiveControl = this.cmbCrsKind1;

	}

	/// <summary>
	/// cmbCrsKind1_SelectedValueChanged
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void cmbCrsKind1_SelectedValueChanged(System.Object sender, System.EventArgs e)
	{
		_teikiKikakuKbn = System.Convert.ToString(getTeikiKikaku(this.cmbCrsKind1.SelectedValue, string.Empty, string.Empty));
		setControlInitialize();
	}

	/// <summary>
	/// txtコース区分3_Validating
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void txtCrsKbn3_Validating(System.Object sender, System.ComponentModel.CancelEventArgs e) //txtコース区分3_Validating(sender,e)
	{

		string inputValue = ""; //inputValue
		string dispValue = ""; //dispValue

		inputValue = System.Convert.ToString(((TextBoxEx)sender).Text);

		if (inputValue == "")
		{
			dispValue = "";
		}
		else if (inputValue == System.Convert.ToString(CrsKbn3Type.tunen))
		{
			dispValue = System.Convert.ToString(getEnumAttrValue(CrsKbn3Type.tunen));
		}
		else if (inputValue == System.Convert.ToString(CrsKbn3Type.kisetu))
		{
			dispValue = System.Convert.ToString(getEnumAttrValue(CrsKbn3Type.kisetu));
		}
		else
		{
			dispValue = System.Convert.ToString(getEnumAttrValue(CrsKbn3Type.＿＿＿));
		}

		this.txtCrsKbn3Nm.Text = dispValue;
	}

	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.pnlList.Top = TopPnlList;
			this.pnlList.Height -= this.HeightGbxCondition + MarginGbxCondition + 11;
			this.grdCrsList.Height -= (this.HeightGbxCondition) + MarginGbxCondition + 11;
			this.lblCrsKind1.Visible = true;
			this.cmbCrsKind1.Visible = true;
		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.pnlList.Top = TopGbxCondition - 11;
			this.pnlList.Height += this.HeightGbxCondition + MarginGbxCondition + 11;
			this.grdCrsList.Height += (this.HeightGbxCondition) + MarginGbxCondition + 11;
			this.lblCrsKind1.Visible = false;
			this.cmbCrsKind1.Visible = false;
		}
	}

	/// <summary>
	/// F8キーが押下された場合の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S01_0305_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			btnCom_Click(this.btnSearch, e);
		}
		else
		{
			return;
		}

	}

	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnClear_Click(object sender, EventArgs e)
	{

		//検索条件クリアボタン押下時
		clearScreen(this);
		clearScreen(this.gbxCondition);
		clearScreen(this.pnlList);

		//コース種別１表示切替
		setCrsKind1();
		//コントロールの表示切替
		setControlInitialize();
		//'ADD-20120402-検索条件「年」「季」の前回情報を設定↓
		setLastSearchInfo();
		this.ActiveControl = this.cmbCrsKind1;

	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdCrsList_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdCrsList.Rows.Count - 1).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
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
	/// F8キー（検索）押下時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF8_ClickOrgProc()
	{

		//検索条件を退避
		_searchConditions_CrsHeadBunrui = _teikiKikakuKbn;
		_searchConditions_CrsKind1 = System.Convert.ToString(this.cmbCrsKind1.SelectedValue).Trim();
		_searchConditions_CrsCd = Strings.Trim(System.Convert.ToString(this.txtCrsCd.Text));
		if (txtYear.Value IsNot null && txtYear.Value != 0)
		{
			_searchConditions_Year = Strings.Trim(System.Convert.ToString(this.txtYear.Text));
		}
		else
		{
			_searchConditions_Year = "";
		}
		_searchConditions_Season = System.Convert.ToString(this.cmbSeason.SelectedValue).Trim();

		_searchConditions_Status = System.Convert.ToString(this.cmbStatus.SelectedValue).Trim();

		// コース種別２
		if (this.chkHigaeri_Kind.Enabled == true)
		{
			_searchConditions_CrsKind2 = "";
			if ((this.chkHigaeri_Kind.Checked == true && this.chkStay_Kind.Checked == true && this.chkR_Kind.Checked == true) ||)
			{
				(this.chkHigaeri_Kind.Checked == false && this.chkStay_Kind.Checked == false && this.chkR_Kind.Checked == false);
				_searchConditions_CrsKind2 = "";
			}
			else
			{
				for (int i = 0; i <= this._chkKind2.Count - 1; i++)
				{
					//TODO
					if (this._chkKind2[i].Checked == true)
					{
						if (this._chkKind2[i].Name == "chkHigaeri_Kind")
						{
							_searchConditions_CrsKind2 += System.Convert.ToString(CrsKind2.higaeri);
						}
						else if (this._chkKind2[i].Name == "chkStay_Kind")
						{
							_searchConditions_CrsKind2 += System.Convert.ToString(CrsKind2.stay);
						}
						else if (this._chkKind2[i].Name == "chkR_Kind")
						{
							_searchConditions_CrsKind2 += System.Convert.ToString(CrsKind2.rCrs);
						}
						_searchConditions_CrsKind2 += ",";
					}
				}
				_searchConditions_CrsKind2 = _searchConditions_CrsKind2.TrimEnd(',');
			}
		}
		else
		{
			_searchConditions_CrsKind2 = "";
		}

		// コース種別３
		if (this.chkJapanese.Enabled == true)
		{
			if ((this.chkJapanese.Checked == true && this.chkGaikokugo.Checked == true) ||)
			{
				(this.chkJapanese.Checked == false && this.chkGaikokugo.Checked == false);
				_searchConditions_CrsKind3 = "";
			}
			else
			{
				if (this.chkJapanese.Checked == true)
				{
					_searchConditions_CrsKind3 = System.Convert.ToString(CrsKind3Type.Japanese);
				}
				else
				{
					_searchConditions_CrsKind3 = System.Convert.ToString(CrsKind3Type.English);
				}
			}
		}
		else
		{
			_searchConditions_CrsKind3 = "";
		}

		_searchConditions_CrsName = System.Convert.ToString(this.txtCrsName.Text);

		// コース区分
		if (cmbCrsKind1.Text == "定期観光" || cmbCrsKind1.Text == "キャピタル")
		{
			//If (Me.chkNoon.Checked = True And Me.chkNight.Checked = True And Me.chkOther1_Kbn.Checked = True) OrElse
			//        (Me.chkNoon.Checked = False And Me.chkNight.Checked = False And Me.chkOther1_Kbn.Checked = False) Then
			if ((this.chkNoon.Checked == true && this.chkNight.Checked == true) ||)
			{
				(this.chkNoon.Checked == false && this.chkNight.Checked == false);
				_searchConditions_CrsKbn1 = "";
			}
			else
			{
				_searchConditions_CrsKbn1 = "";
				for (int i = 0; i <= this._chkKbn1_Teiki.Count - 1; i++)
				{
					//TODO
					if (this._chkKbn1_Teiki[i].Checked == true)
					{
						if (this._chkKbn1_Teiki[i].Name == "chkNoon")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.noon);
						}
						else if (this._chkKbn1_Teiki[i].Name == "chkNight")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.night);
						}
						_searchConditions_CrsKbn1 += ",";
					}
				}
				_searchConditions_CrsKbn1 = _searchConditions_CrsKbn1.TrimEnd(',');
			}
			//If (Me.chkTonai.Checked = True And Me.chkKogai.Checked = True And Me.chkR_CrsKbn2.Checked = True And Me.chkOther2_Kbn.Checked = True) OrElse
			//    (Me.chkTonai.Checked = False And Me.chkKogai.Checked = False And Me.chkR_CrsKbn2.Checked = False And Me.chkOther2_Kbn.Checked = False) Then
			if ((this.chkTonai.Checked == true && this.chkKogai.Checked == true) ||)
			{
				(this.chkTonai.Checked == false && this.chkKogai.Checked == false);
				_searchConditions_CrsKbn2 = "";
			}
			else
			{
				_searchConditions_CrsKbn2 = "";
				for (int i = 0; i <= this._chkKbn2_Teiki.Count - 1; i++)
				{
					//TODO
					if (this._chkKbn2_Teiki[i].Checked == true)
					{
						if (this._chkKbn2_Teiki[i].Name == "chkTonai")
						{
							_searchConditions_CrsKbn2 += System.Convert.ToString(crsKbn2Type.tonai);
						}
						else if (this._chkKbn2_Teiki[i].Name == "chkKogai")
						{
							_searchConditions_CrsKbn2 += System.Convert.ToString(crsKbn2Type.suburbs);
						}
						_searchConditions_CrsKbn2 += ",";
					}
				}
				_searchConditions_CrsKbn2 = _searchConditions_CrsKbn2.TrimEnd(',');
			}
			_searchConditions_CrsKbn3 = System.Convert.ToString(this.txtCrsKbn3.Text);
		}
		else if (cmbCrsKind1.Text == "企画旅行")
		{

			{
				this.chk2StayUp.Checked = true && this.chkNightLine.Checked == true && this.chkShip.Checked == true) ||;

				this.chk2StayUp.Checked = false && this.chkNightLine.Checked == false && this.chkShip.Checked == false);
				_searchConditions_CrsKbn1 = "";
			}
			else
			{
				_searchConditions_CrsKbn1 = "";
				for (int i = 0; i <= this._chkKbn1_Kikaku.Count - 1; i++)
				{
					if (this._chkKbn1_Kikaku[i].Checked == true)
					{
						//TODO
						if (this._chkKbn1_Kikaku[i].Name == "chkHigaeri_Kbn")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.higaeri);
						}
						else if (this._chkKbn1_Kikaku[i].Name == "chkStay_Kbn")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.stay);
						}
						else if (this._chkKbn1_Kikaku[i].Name == "chkNightLine")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.nightLine);
						}
						else if (this._chkKbn1_Kikaku[i].Name == "chkShip")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.ship);
						}
						else if (this._chkKbn1_Kikaku[i].Name == "chk2StayUp")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.Stay2Up);
						}
						else if (this._chkKbn1_Kikaku[i].Name == "chkR_CrsKbn1")
						{
							_searchConditions_CrsKbn1 += System.Convert.ToString(CrsKbn1Type.rCrs);
						}
						_searchConditions_CrsKbn1 += ",";
					}
				}
				_searchConditions_CrsKbn1 = _searchConditions_CrsKbn1.TrimEnd(',');
			}
			_searchConditions_CrsKbn2 = "";
			_searchConditions_CrsKbn3 = "";
		}

		_searchConditions_ManagementSec = System.Convert.ToString(UserInfoManagement.toriatukaiBusyo);

		//ADD-20120402-検索条件「年」「季」を取得↓
		getLastSearchInfo();

		dispCrsList();
		//対象データ未存在の場合
		if (grdCrsList.Rows.Count <= 1)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0007")
			createFactoryMsg.messageDisp("0007");
		}
		else
		{
			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();
		}

	}

	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// コンボボックス選択値の初期設定を行う（定期/企画共通）
	/// </summary>
	/// <remarks></remarks>
	private void InitiarizeComboBox()
	{

		//コース種別１
		DataTable dtCrsKind1 = new DataTable();
		dtCrsKind1.Columns.Add("CODE");
		dtCrsKind1.Columns.Add("VALUE");

		dtCrsKind1.Rows.Add(new object[] { System.Convert.ToInt32(CrsKind1Type.teikiKanko), getEnumAttrValue(CrsKind1Type.teikiKanko) });
		dtCrsKind1.Rows.Add(new object[] { System.Convert.ToInt32(CrsKind1Type.kikakuTravel), getEnumAttrValue(CrsKind1Type.kikakuTravel) });
		dtCrsKind1.Rows.Add(new object[] { System.Convert.ToInt32(CrsKind1Type.capital), getEnumAttrValue(CrsKind1Type.capital) });
		setComboBoxToDataTable(cmbCrsKind1, dtCrsKind1);

		//ステータス
		DataTable dtStatus = getComboboxDataOfDatatable(typeof(CrsStatusType), true); //dtステータス
		setComboBoxToDataTable(cmbStatus, dtStatus);

	}

	/// <summary>
	/// コンボボックスにDataTableの割付けを行う
	/// </summary>
	/// <param name="targetCtl"></param>
	/// <param name="dtList"></param>
	/// <remarks></remarks>
	private void setComboBoxToDataTable(ComboBoxEx targetCtl, DataTable dtList)
	{

		ComboBoxEx with_1 = targetCtl;
		with_1.DataSource = dtList;
		with_1.ValueSubItemIndex = 0;
		with_1.ListColumns(0).Visible = false;
		with_1.ListHeaderPane.Visible = false;
		with_1.TextSubItemIndex = 1;
		with_1.ListColumns(1).Width = with_1.Width;
		with_1.DropDown.AllowResize = false;
	}
	/// <summary>
	/// グリッドコントロールの初期化
	/// </summary>
	/// <remarks></remarks>
	private void InitializeGrid()
	{

		object with_1 = grdCrsList;

		with_1.Cols(CrsList_Koumoku.year).Name = "CRS_YEAR";
		with_1.Cols(CrsList_Koumoku.year).Caption = getEnumAttrValue(CrsList_Koumoku.year);
		with_1.Cols(CrsList_Koumoku.year).Width = 60;
		with_1.Cols(CrsList_Koumoku.year).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.year).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.year).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.season).Name = "SEASON";
		with_1.Cols(CrsList_Koumoku.season).Caption = getEnumAttrValue(CrsList_Koumoku.season);
		with_1.Cols(CrsList_Koumoku.season).Width = 110;
		with_1.Cols(CrsList_Koumoku.season).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.season).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.season).Visible = false;

		with_1.Cols(CrsList_Koumoku.season_DisplayFor).Name = "SEASON_DISP";
		with_1.Cols(CrsList_Koumoku.season_DisplayFor).Caption = getEnumAttrValue(CrsList_Koumoku.season);
		with_1.Cols(CrsList_Koumoku.season_DisplayFor).Width = 100;
		with_1.Cols(CrsList_Koumoku.season_DisplayFor).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.season_DisplayFor).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.season_DisplayFor).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.crsCd).Name = "CRS_CD";
		with_1.Cols(CrsList_Koumoku.crsCd).Caption = getEnumAttrValue(CrsList_Koumoku.crsCd);
		with_1.Cols(CrsList_Koumoku.crsCd).Width = 100;
		with_1.Cols(CrsList_Koumoku.crsCd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsCd).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.crsCd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.crsName).Name = "CRS_NAME";
		with_1.Cols(CrsList_Koumoku.crsName).Caption = getEnumAttrValue(CrsList_Koumoku.crsName);
		with_1.Cols(CrsList_Koumoku.crsName).Width = 330;
		with_1.Cols(CrsList_Koumoku.crsName).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsName).AllowEditing = false;

		with_1.Cols(CrsList_Koumoku.deleteYmd).Name = "DELETE_DATE";
		with_1.Cols(CrsList_Koumoku.deleteYmd).Caption = getEnumAttrValue(CrsList_Koumoku.deleteYmd);
		with_1.Cols(CrsList_Koumoku.deleteYmd).Width = 110;
		with_1.Cols(CrsList_Koumoku.deleteYmd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.deleteYmd).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.deleteYmd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.deleteYmd).Visible = false;


	}

	/// <summary>
	/// コントロールの表示切替
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitialize()
	{

		CdBunruiType cdBunrui = null; //コード分類

		if (cmbCrsKind1.Text == "定期観光")
		{
			chkHigaeri_Kind.Enabled = false;
			chkStay_Kind.Enabled = false;
			chkR_Kind.Enabled = false;
			chkJapanese.Enabled = true;
			chkGaikokugo.Enabled = true;
			pnlTeiki.Visible = true;
			pnlKikaku.Visible = false;

			// 部署で分岐
			if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
			{
				chkJapanese.Checked = false;
				chkGaikokugo.Checked = true;
			}
			else
			{
				chkJapanese.Checked = true;
				chkGaikokugo.Checked = false;
			}

		}
		else if (cmbCrsKind1.Text == "企画旅行")
		{
			chkHigaeri_Kind.Enabled = true;
			chkStay_Kind.Enabled = true;
			chkR_Kind.Enabled = true;
			chkJapanese.Enabled = true;
			chkGaikokugo.Enabled = true;
			pnlTeiki.Visible = false;
			pnlKikaku.Visible = true;

			// 部署で分岐
			if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
			{
				chkJapanese.Checked = false;
				chkGaikokugo.Checked = true;
			}
			else
			{
				chkJapanese.Checked = true;
				chkGaikokugo.Checked = false;
			}

		}
		else if (cmbCrsKind1.Text == "キャピタル")
		{
			chkHigaeri_Kind.Enabled = false;
			chkStay_Kind.Enabled = false;
			chkR_Kind.Enabled = false;
			chkJapanese.Enabled = false;
			chkGaikokugo.Enabled = false;
			pnlTeiki.Visible = true;
			pnlKikaku.Visible = false;
		}

		//定期観光/企画旅行
		if (_teikiKikakuKbn == Teiki_KikakuKbnType.teikiKanko)
		{
			cdBunrui = CdBunruiType.seasonMaster_Teiki;
		}
		else
		{
			cdBunrui = CdBunruiType.seasonMaster_Kikaku;
		}

		//「季」コンボの初期値設定  ''「季」コンボの初期値設定
		CdMasterGet_DA clsCdMasterGet_DA = new CdMasterGet_DA();
		DataTable dtSeasonMaster = clsCdMasterGet_DA.GetCodeMasterData(cdBunrui, true); //dt季マスタ

		this.cmbSeason.DataSource = dtSeasonMaster;
		cmbSeason.ValueSubItemIndex = 0;
		cmbSeason.ListColumns(0).Visible = false;
		cmbSeason.ListHeaderPane.Visible = false;
		cmbSeason.TextSubItemIndex = 1;
		cmbSeason.ListColumns(1).Width = cmbSeason.Width;

	}

	/// <summary>
	/// 画面表示内容のクリア
	/// </summary>
	/// <param name="targetCtr"></param>
	/// <remarks></remarks>
	private void clearScreen(Control targetCtr)
	{

		foreach (Control ctr in targetCtr.Controls)
		{
			if (ReferenceEquals(ctr.GetType, typeof(TextBoxEx)))
			{
				((TextBoxEx)ctr).Text = "";
			}
			else if (ReferenceEquals(ctr.GetType, typeof(NumberEx)))
			{
				((NumberEx)ctr).Text = "";
			}
			else if (ReferenceEquals(ctr.GetType, typeof(DateEx)))
			{
				((DateEx)ctr).Text = "";
			}
			else if (ReferenceEquals(ctr.GetType, typeof(ComboBoxEx)))
			{
				((ComboBoxEx)ctr).SelectedIndex = -1;
				((ComboBoxEx)ctr).ImeMode = System.Windows.Forms.ImeMode.Disable;
			}
			else if (ReferenceEquals(ctr.GetType, typeof(CheckBoxEx)))
			{
				((CheckBoxEx)ctr).Checked = false;
			}
			else if (ReferenceEquals(ctr.GetType, typeof(Panel)))
			{
				clearScreen(ctr);
			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBox)))
			{
				clearScreen(ctr);
			}
			else if (ReferenceEquals(ctr.GetType, typeof(FlexGridEx)))
			{
				if (ReferenceEquals(grdCrsList.DataSource, null))
				{
					((FlexGridEx)ctr).Rows.Count = 1;
				}
				else
				{
					DataTable dt = new DataTable();
					grdCrsList.DataSource = dt;
				}
			}
		}

	}

	/// <summary>
	/// コース種別１表示切替
	/// </summary>
	/// <remarks></remarks>
	private void setCrsKind1()
	{

		// 部署で分岐
		if ((UserInfoManagement.toriatukaiBusyo == Common.FixedCd.ToriatsukaiBusyo.teiki) || (UserInfoManagement.toriatukaiBusyo == Common.FixedCd.ToriatsukaiBusyo.world))
		{
			this.cmbCrsKind1.SelectedIndex = 0;
			this.cmbCrsKind1.SelectedValue = System.Convert.ToString(CrsKind1Type.teikiKanko);
		}
		else if (UserInfoManagement.toriatukaiBusyo == Common.FixedCd.ToriatsukaiBusyo.kikaku)
		{
			this.cmbCrsKind1.SelectedIndex = 1;
			this.cmbCrsKind1.SelectedValue = System.Convert.ToString(CrsKind1Type.kikakuTravel);
		}
		else
		{
			this.cmbCrsKind1.SelectedIndex = 0;
			this.cmbCrsKind1.SelectedValue = System.Convert.ToString(CrsKind1Type.teikiKanko);
		}

		_teikiKikakuKbn = System.Convert.ToString(getTeikiKikaku(this.cmbCrsKind1.SelectedValue, string.Empty, string.Empty));

	}

	/// <summary>
	/// コース一覧検索結果表示
	/// </summary>
	/// <remarks></remarks>
	private void dispCrsList() //Subdispコース一覧()
	{
		CrsSearch_DA clsCrsSearch_DA = new CrsSearch_DA(); //clsコース検索_DA

		clsCrsSearch_DA.CrsHeadBunrui = _teikiKikakuKbn;
		clsCrsSearch_DA.CrsCd = _searchConditions_CrsCd;
		clsCrsSearch_DA.Year = _searchConditions_Year;
		clsCrsSearch_DA.Season = _searchConditions_Season;
		clsCrsSearch_DA.CrsKind1 = _searchConditions_CrsKind1;
		clsCrsSearch_DA.CrsKind2 = _searchConditions_CrsKind2;
		clsCrsSearch_DA.CrsKind3 = _searchConditions_CrsKind3;
		clsCrsSearch_DA.Status = _searchConditions_Status;
		clsCrsSearch_DA.CrsName = _searchConditions_CrsName;
		clsCrsSearch_DA.CrsKbn1 = _searchConditions_CrsKbn1;
		clsCrsSearch_DA.CrsKbn2 = _searchConditions_CrsKbn2;
		clsCrsSearch_DA.CrsKbn3 = _searchConditions_CrsKbn3;
		clsCrsSearch_DA.ManagementSec = _searchConditions_ManagementSec;

		try
		{
			Cursor = Cursors.WaitCursor;

			DataTable dtcrsSearchResultList = new DataTable();

			dtcrsSearchResultList = clsCrsSearch_DA.getCourseList();
			if (dtcrsSearchResultList.Rows.Count > 1000)
			{
				//メッセージ出力
				//Call createFactoryMsg.messageDisp("W90_004", "1000")
				dtcrsSearchResultList.Rows.RemoveAt(1000);
			}
			grdCrsList.DataSource = dtcrsSearchResultList;
			grdCrsList.Styles.Add(DEL_CRS_STYLE);
			grdCrsList.Styles(DEL_CRS_STYLE).BackColor = Color.LightGray;
			for (int rwcnt = 1; rwcnt <= grdCrsList.Rows.Count - 1; rwcnt++)
			{
				if (!grdCrsList(rwcnt, CrsList_Koumoku.deleteYmd).Equals(DBNull.Value))
				{
					grdCrsList.Rows(rwcnt).Style = grdCrsList.Styles(DEL_CRS_STYLE);
				}
			}
		}
		catch (Exception ex)
		{
			throw (ex);
		}
		finally
		{
			Cursor = Cursors.Default;
		}

		//対象データ未存在の場合
		if (grdCrsList.Rows.Count <= 1)
		{
			//clearGuideMessage()
		}
		else
		{
			//setGuideMessage("G017", (grdCrsList.Rows.Count - 1).ToString)
		}
	}

	/// <summary>
	/// 前回検索条件を設定する
	/// </summary>
	/// <remarks></remarks>
	private void setLastSearchInfo()
	{
		this.txtYear.Text = My.Settings.OldYear2.ToString();
		//定⇔企　季節情報が異なるため、以下の条件↓
		if (this.cmbSeason.Items.Count - 1 >= My.Settings.OldSeason2)
		{
			this.cmbSeason.SelectedIndex = My.Settings.OldSeason2;
		}
	}

	/// <summary>
	/// 検索条件を取得する
	/// </summary>
	/// <remarks></remarks>
	private void getLastSearchInfo()
	{
		My.Settings.OldYear2 = this.txtYear.Text;
		My.Settings.OldSeason2 = this.cmbSeason.SelectedIndex;
		My.Settings.Default.Save();
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
		F10Key_Visible = false; // F10:未使用
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用
		F10Key_Text = "F10:新規";
		F12Key_Text = "F12:台帳作成";
		F3Key_Text = "F3:パンフ";
		F5Key_Text = "F5:一覧照会";
		F6Key_Text = "F6:参照";
		F9Key_Text = "F9:コピー";
	}
	#endregion

}