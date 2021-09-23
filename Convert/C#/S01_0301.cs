using System.ComponentModel;
//using System.Enum;
using C1.Win.C1FlexGrid;
using Microsoft.VisualBasic.CompilerServices;

/// <summary>
/// コース検索
/// </summary>
/// <remarks></remarks>
public class S01_0301 //frmコース検索
{
	public S01_0301()
	{
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
											 //Private _検索条件_改定日From As String
											 //Private _検索条件_改定日To As String
	private string _searchConditions_Status; //_検索条件_ステータス
	private string _searchConditions_CrsKind1; //_検索条件_コース種別1
	private string _searchConditions_CrsKind2; //_検索条件_コース種別2
	private string _searchConditions_CrsKind3; //_検索条件_コース種別3
	private string _searchConditions_Theme; //_検索条件_テーマ
	private string _searchConditions_CrsName; //_検索条件_コース名
	private string _searchConditions_CrsNameKana; //_検索条件_コース名カナ
	private string _searchConditions_CrsKbn1; //_検索条件_コース区分1
	private string _searchConditions_CrsKbn2; //_検索条件_コース区分2
	private string _searchConditions_CrsKbn3; //_検索条件_コース区分3
	private bool _searchConditions_DeleteWith; //_検索条件_削除含む
	private string _searchConditions_CostCyosei; //_検索条件_原価調整
												 //Private _検索条件_WEB追加 As String
	private string _searchConditions_NaiyoKakunin; //_検索条件_内容確認
	private string _searchConditions_ToriatukaiAdd; //_検索条件_取扱追加
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
	public const int TopPnlList = 278;

	private List _chkKind2;
	private List _chkKbn1_Teiki;
	private List _chkKbn2_Teiki;
	private List _chkKbn1_Kikaku;
	#endregion

	#region 列挙 //#Region"列挙"
	/// <summary>
	/// 検索結果一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	public enum CrsList_Koumoku : int //コース一覧_項目
	{
		[Value("選択")] selection = 1,
		[Value("コースコード")] crsCd,
		[Value("年")] year,
		[Value("季")] season,
		[Value("季_表示用")] season_DisplayFor,
		[Value("無効フラグ")] invalidFlg,
		[Value("コース種別１")] crsKind1,
		[Value("コース種別２")] crsKind2,
		[Value("コース種別３")] crsKind3,
		[Value("コース名")] crsName,
		[Value("改定日")] kaiteiDay,
		[Value("ステータス")] status,
		[Value("開始日")] startDay,
		[Value("終了日")] endDay,
		[Value("受付開始日")] uketukeStartDay,
		[Value("更新日")] updateDay,
		[Value("更新者")] updatePerson,
		[Value("原価調整済日")] costCyoseiAlreadyDay,
		[Value("取扱確認日")] toriatukaiKakuninDay,
		[Value("コース内容確認日")] crsNaiyoKakuninDay,
		[Value("パンフ依頼日")] pamphIraiDay,
		[Value("台帳作成日")] ledgerCreateDay,
		[Value("最終作成月")] lastMonthTeiki,
		[Value("WEBシステム連動日付")] WEBSystemRendoYmd,
		[Value("一括公開日")] bulkOpenDay,
		[Value("コースメモ")] crsMemo,
		[Value("削除日付")] deleteYmd,
		[Value("削除理由")] deleteRiyuu,
		[Value("登録日")] entryDay,
		[Value("定期企画区分")] teikiKikakuKbn
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

		//権限によるファンクションボタン表示制御
		setFunctionKeyEnabled();

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
		//Me._chkKbn1_Teiki.Add(Me.chkOther1_Kbn)
		this._chkKbn2_Teiki.Add(this.chkTonai);
		this._chkKbn2_Teiki.Add(this.chkKogai);
		//Me._chkKbn2_Teiki.Add(Me.chkR_CrsKbn2)
		//Me._chkKbn2_Teiki.Add(Me.chkOther2_Kbn)
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
	/// 全てチェックONボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void btnCheckAll_Click(System.Object sender, System.EventArgs e)
	{
		setCrsListCheck(true);
	}

	/// <summary>
	/// 全てチェックOFFボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void btnUncheckALL_Click(System.Object sender, System.EventArgs e)
	{
		setCrsListCheck(false);
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
			this.btnCheckAll.Top = System.Convert.ToInt32(this.grdCrsList.Top + this.grdCrsList.Height) + MarginGbxCondition;
			this.btnUncheckALL.Top = System.Convert.ToInt32(this.grdCrsList.Top + this.grdCrsList.Height) + MarginGbxCondition;
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
			this.btnCheckAll.Top = System.Convert.ToInt32(this.grdCrsList.Top + this.grdCrsList.Height) + MarginGbxCondition;
			this.btnUncheckALL.Top = System.Convert.ToInt32(this.grdCrsList.Top + this.grdCrsList.Height) + MarginGbxCondition;
			this.lblCrsKind1.Visible = false;
			this.cmbCrsKind1.Visible = false;
		}
	}

	/// <summary>
	/// F8キーが押下された場合の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S01_0301_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			base.btnCom_Click(this.btnSearch, e);
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
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();
	}

	/// <summary>
	/// F3キー（パンフ）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF3_ClickOrgProc()
	{

		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		if (checkCrsListSelection(ref _selectionCrsInfo) == false)
		{
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0015", "コース")
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		// コースマスタ存在チェック (チェックONの行)
		if (checkExistsCrsMaster(_selectionCrsInfo) == false)
		{
			return;
		}

		//ADD-20130218-選択コース情報にフォーカスを戻す↓
		getSelectionCrsInfo();

		S01_0303 PamphletIrai = new S01_0303(); //パンフ依頼
		PamphletIrai.TaisyoCrsInfo = _selectionCrsInfo;

		try
		{
			//遷移先画面をモーダルで表示
			PamphletIrai.ShowDialog(this);
			//コース一覧を再表示
			dispCrsList();
			setSelectionCheck();

			//ADD-20130218-選択コース情報にフォーカスを戻す↓
			setSelectionLine();

			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();
		}
		catch (Exception)
		{
			//TODO
			PamphletIrai.Close();
		}
		finally
		{
			//オブジェクトの破棄
			PamphletIrai.Dispose();
		}
	}

	/// <summary>
	/// F5キー（一覧照会）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF5_ClickOrgProc()
	{

		// コースマスタ登録内容出力指示
		using (S01_0322 form = new S01_0322())
		{
			base.openWindow(form, true);
		}


	}

	/// <summary>
	/// F4キー（削除）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF4_ClickOrgProc()
	{

		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		if (checkCrsListSelection(ref _selectionCrsInfo) == false)
		{
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0015", "コース")
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		// コースマスタ存在チェック (チェックONの行)
		if (checkExistsCrsMaster(_selectionCrsInfo) == false)
		{
			return;
		}

		S01_0304 CrsDelete = new S01_0304(); //コース削除
		CrsDelete.TaisyoCrsInfo = _selectionCrsInfo;

		try
		{
			//遷移先画面をモーダルで表示
			CrsDelete.ShowDialog(this);
			//コース一覧を再表示
			dispCrsList();
			setSelectionCheck();
			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();
		}
		catch (Exception)
		{
			//TODO
			CrsDelete.Close();
		}
		finally
		{
			//オブジェクトの破棄
			CrsDelete.Dispose();
		}

	}

	/// <summary>
	/// F6キー（参照）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF6_ClickOrgProc()
	{

		//一覧にデータが表示されていない場合は何もしない
		if (this.grdCrsList.Rows.Count <= 1)
		{
			//TODO:共通変更対応
			//'メッセージ出力.messageDisp("0015", "コース")
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];

		int row = System.Convert.ToInt32(this.grdCrsList.Row); //row

		CrsMasterKeyKoumoku with_1 = _displayTaisyoCrsInfo[0];
		with_1.Teiki_KikakuKbn = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.teikiKikakuKbn));
		with_1.CrsKind1 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind1));
		with_1.CrsCd = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsCd));
		with_1.Year = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.year));
		with_1.Season = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.season));
		with_1.KaiteiDay = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "");
		with_1.InvalidFlg = System.Convert.ToInt32(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg));
		if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2)) == true)
		{
			with_1.CrsKind2 = "";
		}
		else
		{
			with_1.CrsKind2 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2));
		}
		if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind3)) == true)
		{
			with_1.CrsKind3 = "";
		}
		else
		{
			with_1.CrsKind3 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind3));
		}


		// コースマスタ存在チェック (選択されている行)
		if (checkExistsCrsMaster(_displayTaisyoCrsInfo) == false)
		{
			return;
		}

		//基本情報画面を表示
		S01_0307 BasicInfo = new S01_0307(); //基本情報
		BasicInfo.TaisyoCrsInfo = _displayTaisyoCrsInfo;
		BasicInfo.ProcessMode = ProcessMode.reference;

		try
		{
			//遷移先画面をモーダルで表示
			BasicInfo.ShowDialog(this);
			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();
		}
		catch (Exception)
		{
			//TODO
			BasicInfo.Close();
		}
		finally
		{
			//オブジェクトの破棄
			BasicInfo.Dispose();
		}

	}

	/// <summary>
	/// F7キー（印刷）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF7_ClickOrgProc()
	{

		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		if (checkCrsListSelection(ref _selectionCrsInfo) == false)
		{
			//TODO:共通変更対応
			createFactoryMsg.messageDisp("0015", "コース");
			//メッセージ出力.messageDisp("0015", "コース")
			return;
		}

		// コースマスタ存在チェック (チェックONの行)
		if (checkExistsCrsMaster(_selectionCrsInfo) == false)
		{
			return;
		}

		//基本情報画面を表示
		S01_0306 formSelection = new S01_0306(); //帳票選択
		formSelection.TaisyoCrsInfo = _selectionCrsInfo;
		formSelection.BfGamen = BfGamenType.crsSearch;

		try
		{
			//遷移先画面をモーダルで表示
			formSelection.ShowDialog(this);
			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();
		}
		catch (Exception)
		{
			//TODO
			formSelection.Close();
		}
		finally
		{
			//オブジェクトの破棄
			formSelection.Dispose();
		}
	}

	/// <summary>
	/// F8キー（検索）押下時イベント
	/// </summary>
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

		_searchConditions_Theme = System.Convert.ToString(this.txtTheme.Text);
		_searchConditions_CrsName = System.Convert.ToString(this.txtCrsName.Text);
		_searchConditions_CrsNameKana = System.Convert.ToString(this.txtCrsNameKana.Text);

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

		_searchConditions_DeleteWith = System.Convert.ToBoolean(this.chkDeleteWith.Checked);
		if ((this.chkCostCyosei_Mi.Checked == true && this.chkCostCyosei_Already.Checked == true) ||)
		{
			(this.chkCostCyosei_Mi.Checked == false && this.chkCostCyosei_Already.Checked == false);
			_searchConditions_CostCyosei = "";
		}
		else
		{
			if (this.chkCostCyosei_Mi.Checked == true)
			{
				_searchConditions_CostCyosei = "1";
			}
			else
			{
				_searchConditions_CostCyosei = "2";
			}
		}
		if ((this.chkNaiyoKakunin_Mi.Checked == true && this.chkNaiyoKakunin_Already.Checked == true) ||)
		{
			(this.chkNaiyoKakunin_Mi.Checked == false && this.chkNaiyoKakunin_Already.Checked == false);
			_searchConditions_NaiyoKakunin = "";
		}
		else
		{
			if (this.chkNaiyoKakunin_Mi.Checked == true)
			{
				_searchConditions_NaiyoKakunin = "1";
			}
			else
			{
				_searchConditions_NaiyoKakunin = "2";
			}
		}
		if ((this.chkToriatukaiAdd_Mi.Checked == true && this.chkToriatukaiAdd_Already.Checked == true) ||)
		{
			(this.chkToriatukaiAdd_Mi.Checked == false && this.chkToriatukaiAdd_Already.Checked == false);
			_searchConditions_ToriatukaiAdd = "";
		}
		else
		{
			if (this.chkToriatukaiAdd_Mi.Checked == true)
			{
				_searchConditions_ToriatukaiAdd = "1";
			}
			else
			{
				_searchConditions_ToriatukaiAdd = "2";
			}
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

	/// <summary>
	/// F9キー（コピー）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF9_ClickOrgProc()
	{

		//一覧にデータが表示されていない場合は何もしない
		if (this.grdCrsList.Rows.Count <= 1)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0015", "コース")
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		//チェックONの行情報を取得
		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		checkCrsListSelection(ref _selectionCrsInfo);

		//チェックONの行が無い場合、選択行を対象とする
		//DEL-20130218-選択コース情報にフォーカスを戻す↓
		//If _選択コース情報(0).コースコード = "" Then
		int row = System.Convert.ToInt32(this.grdCrsList.Row); //row
		_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];
		CrsMasterKeyKoumoku with_1 = _displayTaisyoCrsInfo[0];
		with_1.Teiki_KikakuKbn = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.teikiKikakuKbn));
		with_1.CrsCd = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsCd)).Trim();
		with_1.Year = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.year));
		with_1.Season = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.season));
		with_1.KaiteiDay = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "");
		with_1.InvalidFlg = System.Convert.ToInt32(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg));
		//End If

		//----------------------------------------------------------------
		// 挙動が異なるためコメント (現行踏襲)
		//   単一コピー時  ：チェックONが対象 (選択されている行は無視)
		//   複数コピー時  ：チェックONが対象 (選択されている行は無視)
		//   更新時、参照時：選択されている行が対象 (チェックON行は無視)
		//----------------------------------------------------------------

		if (_selectionCrsInfo.Count() == 1)
		{
			//選択行が１行の場合、基本情報画面を表示
			S01_0307 BasicInfo = new S01_0307(); //基本情報
			if (_selectionCrsInfo[0].CrsCd != "")
			{
				//ADD-20130218-選択コース情報にフォーカスを戻す↓
				//基本情報.対象コース情報 = _選択コース情報
				CrsMasterKeyKoumoku[] temp__toBasicInfo = BasicInfo.TaisyoCrsInfo;
				setBasicInfo_CrsInfo(ref temp__toBasicInfo, _selectionCrsInfo);
			}
			else
			{
				//ADD-20130218-選択コース情報にフォーカスを戻す↓
				//基本情報.対象コース情報 = _表示対象コース情報
				CrsMasterKeyKoumoku[] temp__toBasicInfo2 = BasicInfo.TaisyoCrsInfo;
				setBasicInfo_CrsInfo(ref temp__toBasicInfo2, _displayTaisyoCrsInfo);
			}
			BasicInfo.ProcessMode = ProcessMode.copy;

			try
			{

				// コースマスタ存在チェック (次画面へ渡すコースキー情報)
				if (checkExistsCrsMaster(BasicInfo.TaisyoCrsInfo) == false)
				{
					goto endOfTry;
				}

				//遷移先画面をモーダルで表示
				BasicInfo.ShowDialog(this);
				//ADD-20130218-選択コース情報にフォーカスを戻す↓
				if (BasicInfo.EntryFlg == true)
				{
					//コース一覧を再表示
					dispCrsList();
					setSelectionCheck();

					_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];
					_displayTaisyoCrsInfo[0] = BasicInfo.EntryCrsInfo;
					setSelectionLine();
				}
				//ADD-20130218-選択コース情報にフォーカスを戻す↑

			}
			catch (Exception)
			{
				//TODO
				BasicInfo.Close();
			}
			finally
			{
				//オブジェクトの破棄
				BasicInfo.Dispose();
			}
		endOfTry:
			1.GetHashCode();

		}
		else
		{
			//選択行が複数行の場合、コースコピー画面を表示
			S01_0302 CrsCopy = new S01_0302(); //コースコピー
			CrsCopy.TaisyoCrsInfo = _selectionCrsInfo;

			try
			{
				// コースマスタ存在チェック (チェックONの行)
				if (checkExistsCrsMaster(_selectionCrsInfo) == false)
				{
					goto endOfTry1;
				}

				//遷移先画面をモーダルで表示
				CrsCopy.ShowDialog(this);
				//コース一覧を再表示
				dispCrsList();
				setSelectionCheck();

				//ADD-20130218-選択コース情報にフォーカスを戻す↓
				setSelectionLine();
			}
			catch (Exception)
			{
				//TODO
				CrsCopy.Close();
			}
			finally
			{
				//オブジェクトの破棄
				CrsCopy.Dispose();
			}
		endOfTry1:
			1.GetHashCode();
		}

		//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
		this.grdCrsList.Focus();

	}

	/// <summary>
	/// F10キー（新規）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		//チェックONの行情報を取得
		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		checkCrsListSelection(ref _selectionCrsInfo);

		//選択行の情報を取得
		_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];
		CrsMasterKeyKoumoku with_1 = _displayTaisyoCrsInfo[0];
		//.Teiki_KikakuKbn = CStr(Me.cmbCrsHeadBunrui.SelectedValue)
		with_1.CrsCd = "";
		with_1.Year = "";
		with_1.Season = "";
		with_1.KaiteiDay = "";
		with_1.InvalidFlg = 0;
		with_1.CrsKind1 = System.Convert.ToString(this.cmbCrsKind1.SelectedValue);

		//基本情報画面を表示
		S01_0307 BasicInfo = new S01_0307(); //基本情報
		BasicInfo.TaisyoCrsInfo = _displayTaisyoCrsInfo;
		BasicInfo.ProcessMode = ProcessMode.shinki;

		try
		{
			//遷移先画面をモーダルで表示
			BasicInfo.ShowDialog(this);
			//ADD-20130218-選択コース情報にフォーカスを戻す↓
			if (BasicInfo.EntryFlg == true)
			{
				//コース一覧を再表示
				dispCrsList();
				setSelectionCheck();

				_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];
				_displayTaisyoCrsInfo[0] = BasicInfo.EntryCrsInfo;
				setSelectionLine();
			}
			//ADD-20130218-選択コース情報にフォーカスを戻す↑

			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();

		}
		catch (Exception)
		{
			//TODO
			BasicInfo.Close();
		}
		finally
		{
			//オブジェクトの破棄
			BasicInfo.Dispose();
		}
	}

	/// <summary>
	/// F11キー（更新）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF11_ClickOrgProc()
	{

		bool blnLock = false; //blnLock

		//一覧にデータが表示されていない場合は何もしない
		if (this.grdCrsList.Rows.Count <= 1)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0015", "コース")
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		//チェックONの行情報を取得
		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		checkCrsListSelection(ref _selectionCrsInfo);

		_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1];
		int row = System.Convert.ToInt32(this.grdCrsList.Row);

		CrsMasterKeyKoumoku with_1 = _displayTaisyoCrsInfo[0];
		with_1.Teiki_KikakuKbn = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.teikiKikakuKbn));
		with_1.CrsCd = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsCd)).Trim();
		with_1.Year = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.year));
		with_1.Season = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.season));
		with_1.KaiteiDay = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "");
		with_1.InvalidFlg = System.Convert.ToInt32(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg));
		if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind1)) == true)
		{
			with_1.CrsKind1 = "";
		}
		else
		{
			with_1.CrsKind1 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind1));
		}
		if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2)) == true)
		{
			with_1.CrsKind2 = "";
		}
		else
		{
			with_1.CrsKind2 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2));
		}


		// コースマスタ存在チェック (選択されている行)
		if (checkExistsCrsMaster(_displayTaisyoCrsInfo) == false)
		{
			return;
		}

		//該当コースのレコードをLOCK
		if (lockCrsMaster(_displayTaisyoCrsInfo[0]) == false)
		{
			return;
		}
		blnLock = true;

		//基本情報画面を表示
		S01_0307 basicInfo = new S01_0307(); //基本情報
		CrsMasterKeyKoumoku[] param = new CrsMasterKeyKoumoku[1]; //param(0)
		CrsMasterKeyKoumoku with_2 = param(0);
		with_2.Teiki_KikakuKbn = _displayTaisyoCrsInfo[0].Teiki_KikakuKbn;
		with_2.CrsCd = _displayTaisyoCrsInfo[0].CrsCd;
		with_2.Year = _displayTaisyoCrsInfo[0].Year;
		with_2.Season = _displayTaisyoCrsInfo[0].Season;
		with_2.KaiteiDay = _displayTaisyoCrsInfo[0].KaiteiDay;
		with_2.InvalidFlg = _displayTaisyoCrsInfo[0].InvalidFlg;
		with_2.CrsKind1 = _displayTaisyoCrsInfo[0].CrsKind1;
		with_2.CrsKind2 = _displayTaisyoCrsInfo[0].CrsKind2;
		basicInfo.TaisyoCrsInfo = param;
		basicInfo.ProcessMode = ProcessMode.update;

		try
		{
			//遷移先画面をモーダルで表示
			basicInfo.ShowDialog(this);
			//ロック解除
			relockCrsMaster();
			blnLock = false;
			if (basicInfo.EntryFlg == true)
			{
				//コース一覧を再表示
				dispCrsList();
				setSelectionCheck();
				setSelectionLine();
			}
			//ADD-20120927-検索後にグリッドへフォーカスをセット（マウススクロール関連）↓
			this.grdCrsList.Focus();

		}
		catch (Exception)
		{
			//ロック中の場合、解除
			if (blnLock == true)
			{
				relockCrsMaster();
			}
			//TODO
			basicInfo.Close();
		}
		finally
		{
			//オブジェクトの破棄
			basicInfo.Dispose();
		}
	}

	/// <summary>
	/// F12キー（台帳作成）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF12_ClickOrgProc()
	{
		//MyBase.btnF12_Click(sender, e)

		if (ReferenceEquals(this.grdCrsList.DataSource, null))
		{
			// 処理対象データがありません。
			createFactoryMsg.messageDisp("0036");
			return;
		}

		_selectionCrsInfo = new CrsMasterKeyKoumoku[1];
		if (checkCrsListSelection(ref _selectionCrsInfo) == false)
		{
			createFactoryMsg.messageDisp("0015", "コース");
			return;
		}

		// コースマスタ存在チェック (チェックONの行)
		if (checkExistsCrsMaster(_selectionCrsInfo) == false)
		{
			return;
		}

		// 台帳作成画面の一覧用(データテーブル)を生成する
		try
		{

			// 台帳作成に使用するチェックONのデータを取得
			DataTable selectDataTable = this.getListSelectionData();

			// 取得できない場合エラー (上記でチェック済のため不要だが念のため実装)
			if (selectDataTable.Rows.Count <= 0)
			{
				// {1}が選択されていません。
				createFactoryMsg.messageDisp("0015", "コース");
				return;
			}

			// 台帳作成
			bool ret = this.registCrsLedger(selectDataTable);

		}
		catch (Exception)
		{
			throw;

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
		DataTable dtCrsKind1 = new DataTable(); //dtコース種別1  1
		dtCrsKind1.Columns.Add("CODE");
		dtCrsKind1.Columns.Add("VALUE");
		//dtCrsKind1.Rows.Add(New Object() {"", ""})

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

		with_1.Cols(CrsList_Koumoku.selection).Name = "CHECK_FLG";
		with_1.Cols(CrsList_Koumoku.selection).Caption = getEnumAttrValue(CrsList_Koumoku.selection);
		with_1.Cols(CrsList_Koumoku.selection).Width = 50;
		with_1.Cols(CrsList_Koumoku.selection).DataType = typeof(bool);
		with_1.Cols(CrsList_Koumoku.selection).AllowEditing = true;
		with_1.Cols(CrsList_Koumoku.selection).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.crsCd).Name = "CRS_CD";
		with_1.Cols(CrsList_Koumoku.crsCd).Caption = getEnumAttrValue(CrsList_Koumoku.crsCd);
		with_1.Cols(CrsList_Koumoku.crsCd).Width = 100;
		with_1.Cols(CrsList_Koumoku.crsCd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsCd).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.crsCd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

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

		with_1.Cols(CrsList_Koumoku.invalidFlg).Name = "INVALID_FLG";
		with_1.Cols(CrsList_Koumoku.invalidFlg).Caption = getEnumAttrValue(CrsList_Koumoku.invalidFlg);
		with_1.Cols(CrsList_Koumoku.invalidFlg).Width = 30;
		with_1.Cols(CrsList_Koumoku.invalidFlg).DataType = typeof(int);
		with_1.Cols(CrsList_Koumoku.invalidFlg).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.invalidFlg).Visible = false;

		with_1.Cols(CrsList_Koumoku.crsKind1).Name = "CRS_KIND_1";
		with_1.Cols(CrsList_Koumoku.crsKind1).Caption = getEnumAttrValue(CrsList_Koumoku.crsKind1);
		with_1.Cols(CrsList_Koumoku.crsKind1).Width = 110;
		with_1.Cols(CrsList_Koumoku.crsKind1).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsKind1).AllowEditing = false;
		SortedList[,] hstCrsKind1 = getComboboxData(typeof(CrsKind1Type));
		with_1.Cols(CrsList_Koumoku.crsKind1).DataMap = hstCrsKind1;

		with_1.Cols(CrsList_Koumoku.crsKind2).Name = "CRS_KIND_2";
		with_1.Cols(CrsList_Koumoku.crsKind2).Caption = getEnumAttrValue(CrsList_Koumoku.crsKind2);
		with_1.Cols(CrsList_Koumoku.crsKind2).Width = 110;
		with_1.Cols(CrsList_Koumoku.crsKind2).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsKind2).AllowEditing = false;
		SortedList[,] hstCrsKind2 = getComboboxData(typeof(CrsKind2));
		with_1.Cols(CrsList_Koumoku.crsKind2).DataMap = hstCrsKind2;

		with_1.Cols(CrsList_Koumoku.crsKind3).Name = "CRS_KIND_3";
		with_1.Cols(CrsList_Koumoku.crsKind3).Caption = getEnumAttrValue(CrsList_Koumoku.crsKind3);
		with_1.Cols(CrsList_Koumoku.crsKind3).Width = 110;
		with_1.Cols(CrsList_Koumoku.crsKind3).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsKind3).AllowEditing = false;
		SortedList[,] hstCrsKind3 = getComboboxData(typeof(CrsKind3Type));
		with_1.Cols(CrsList_Koumoku.crsKind3).DataMap = hstCrsKind3;

		with_1.Cols(CrsList_Koumoku.crsName).Name = "CRS_NAME";
		with_1.Cols(CrsList_Koumoku.crsName).Caption = getEnumAttrValue(CrsList_Koumoku.crsName);
		with_1.Cols(CrsList_Koumoku.crsName).Width = 330;
		with_1.Cols(CrsList_Koumoku.crsName).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsName).AllowEditing = false;

		with_1.Cols(CrsList_Koumoku.kaiteiDay).Name = "KAITEI_DATE";
		with_1.Cols(CrsList_Koumoku.kaiteiDay).Caption = getEnumAttrValue(CrsList_Koumoku.kaiteiDay);
		with_1.Cols(CrsList_Koumoku.kaiteiDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.kaiteiDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.kaiteiDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.kaiteiDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.kaiteiDay).Visible = false;

		with_1.Cols(CrsList_Koumoku.status).Name = "CRS_STATUS";
		with_1.Cols(CrsList_Koumoku.status).Caption = getEnumAttrValue(CrsList_Koumoku.status);
		with_1.Cols(CrsList_Koumoku.status).Width = 130;
		with_1.Cols(CrsList_Koumoku.status).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.status).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.status).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		SortedList[,] hstStatus = getComboboxData(typeof(CrsStatusType));
		with_1.Cols(CrsList_Koumoku.status).DataMap = hstStatus;

		with_1.Cols(CrsList_Koumoku.startDay).Name = "YMD_FROM";
		with_1.Cols(CrsList_Koumoku.startDay).Caption = getEnumAttrValue(CrsList_Koumoku.startDay);
		with_1.Cols(CrsList_Koumoku.startDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.startDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.startDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.startDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.endDay).Name = "YMD_TO";
		with_1.Cols(CrsList_Koumoku.endDay).Caption = getEnumAttrValue(CrsList_Koumoku.endDay);
		with_1.Cols(CrsList_Koumoku.endDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.endDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.endDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.endDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.uketukeStartDay).Name = "UKETUKE_START_DATE";
		with_1.Cols(CrsList_Koumoku.uketukeStartDay).Caption = getEnumAttrValue(CrsList_Koumoku.uketukeStartDay);
		with_1.Cols(CrsList_Koumoku.uketukeStartDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.uketukeStartDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.uketukeStartDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.uketukeStartDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.updateDay).Name = "UPDATE_DATE";
		with_1.Cols(CrsList_Koumoku.updateDay).Caption = getEnumAttrValue(CrsList_Koumoku.updateDay);
		with_1.Cols(CrsList_Koumoku.updateDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.updateDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.updateDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.updateDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.updatePerson).Name = "UPDATE_USER_ID";
		with_1.Cols(CrsList_Koumoku.updatePerson).Caption = getEnumAttrValue(CrsList_Koumoku.updatePerson);
		with_1.Cols(CrsList_Koumoku.updatePerson).Width = 110;
		with_1.Cols(CrsList_Koumoku.updatePerson).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.updatePerson).AllowEditing = false;

		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).Name = "COST_TYOSEI_DATE";
		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).Caption = getEnumAttrValue(CrsList_Koumoku.costCyoseiAlreadyDay);
		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.costCyoseiAlreadyDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).Name = "TORIATUKAI_INFO_ADD_DATE";
		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).Caption = getEnumAttrValue(CrsList_Koumoku.toriatukaiKakuninDay);
		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.toriatukaiKakuninDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).Name = "CRS_NAIYO_KAKUNIN_DATE";
		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).Caption = getEnumAttrValue(CrsList_Koumoku.crsNaiyoKakuninDay);
		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).Width = 140;
		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.crsNaiyoKakuninDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.pamphIraiDay).Name = "PAMPH_IRAI_DATE";
		with_1.Cols(CrsList_Koumoku.pamphIraiDay).Caption = getEnumAttrValue(CrsList_Koumoku.pamphIraiDay);
		with_1.Cols(CrsList_Koumoku.pamphIraiDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.pamphIraiDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.pamphIraiDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.pamphIraiDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).Name = "LEDGER_CREATE_DATE";
		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).Caption = getEnumAttrValue(CrsList_Koumoku.ledgerCreateDay);
		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).Width = 170;
		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.ledgerCreateDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).Name = "LAST_MONTH_TEIKI";
		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).Caption = getEnumAttrValue(CrsList_Koumoku.lastMonthTeiki);
		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).Width = 110;
		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.lastMonthTeiki).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).Name = "WEB_YOYAKU_SYSTEM_RENDO_DATE";
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).Caption = getEnumAttrValue(CrsList_Koumoku.WEBSystemRendoYmd);
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).Width = 170;
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.WEBSystemRendoYmd).Visible = false;

		with_1.Cols(CrsList_Koumoku.bulkOpenDay).Name = "ALL_OPEN_DATE";
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).Caption = getEnumAttrValue(CrsList_Koumoku.bulkOpenDay);
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.bulkOpenDay).Visible = false;

		with_1.Cols(CrsList_Koumoku.crsMemo).Name = "CRS_MEMO";
		with_1.Cols(CrsList_Koumoku.crsMemo).Caption = getEnumAttrValue(CrsList_Koumoku.crsMemo);
		with_1.Cols(CrsList_Koumoku.crsMemo).Width = 110;
		with_1.Cols(CrsList_Koumoku.crsMemo).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.crsMemo).AllowEditing = false;

		with_1.Cols(CrsList_Koumoku.deleteYmd).Name = "DELETE_DATE";
		with_1.Cols(CrsList_Koumoku.deleteYmd).Caption = getEnumAttrValue(CrsList_Koumoku.deleteYmd);
		with_1.Cols(CrsList_Koumoku.deleteYmd).Width = 110;
		with_1.Cols(CrsList_Koumoku.deleteYmd).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.deleteYmd).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.deleteYmd).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		with_1.Cols(CrsList_Koumoku.deleteRiyuu).Name = "DELETE_REASON";
		with_1.Cols(CrsList_Koumoku.deleteRiyuu).Caption = getEnumAttrValue(CrsList_Koumoku.deleteRiyuu);
		with_1.Cols(CrsList_Koumoku.deleteRiyuu).Width = 250;
		with_1.Cols(CrsList_Koumoku.deleteRiyuu).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.deleteRiyuu).AllowEditing = false;

		with_1.Cols(CrsList_Koumoku.entryDay).Name = "ENTRY_DATE";
		with_1.Cols(CrsList_Koumoku.entryDay).Caption = getEnumAttrValue(CrsList_Koumoku.entryDay);
		with_1.Cols(CrsList_Koumoku.entryDay).Width = 110;
		with_1.Cols(CrsList_Koumoku.entryDay).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.entryDay).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.entryDay).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.entryDay).Visible = false;

		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).Name = "TEIKI_KIKAKU_KBN";
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).Caption = getEnumAttrValue(CrsList_Koumoku.teikiKikakuKbn);
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).Width = 110;
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).DataType = typeof(string);
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).AllowEditing = false;
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_1.Cols(CrsList_Koumoku.teikiKikakuKbn).Visible = false;

		with_1.Cols.Frozen = 5;

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
	/// コース一覧チェックボックス設定
	/// </summary>
	/// <param name="value"></param>
	/// <remarks></remarks>
	private void setCrsListCheck(bool value) //コース一覧Check(value)
	{

		this.Cursor = Cursors.WaitCursor;

		object with_1 = this.grdCrsList;
		for (int row = 1; row <= with_1.Rows.Count - 1; row++)
		{
			with_1.SetData(row, CrsList_Koumoku.selection, value);
		}

		this.Cursor = Cursors.Default;

	}

	/// <summary>
	/// コース一覧チェックボックス選択有無チェック
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool checkCrsListSelection(ref CrsMasterKeyKoumoku[] targetCourse) //checkコース一覧選択(targetCourse)
	{
		bool result = false;

		object with_1 = this.grdCrsList;
		for (int row = 1; row <= with_1.Rows.Count - 1; row++)
		{
			if (System.Convert.ToBoolean(with_1.GetData(row, CrsList_Koumoku.selection)) == true)
			{
				if (targetCourse[0].CrsCd != "")
				{
					Array.Resize(ref targetCourse, targetCourse.Length + 1);
				}
				targetCourse[targetCourse.Length - 1].Teiki_KikakuKbn = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.teikiKikakuKbn));
				targetCourse[targetCourse.Length - 1].CrsCd = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsCd)).Trim();
				targetCourse[targetCourse.Length - 1].Year = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.year));
				targetCourse[targetCourse.Length - 1].Season = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.season));
				targetCourse[targetCourse.Length - 1].Season_DisplayFor = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.season_DisplayFor));
				targetCourse[targetCourse.Length - 1].KaiteiDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "");
				targetCourse[targetCourse.Length - 1].InvalidFlg = System.Convert.ToInt32(with_1.GetData(row, CrsList_Koumoku.invalidFlg));
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.crsName)) == false)
				{
					targetCourse[targetCourse.Length - 1].CrsName = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsName));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].CrsName = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.pamphIraiDay)) == false)
				{
					targetCourse[targetCourse.Length - 1].PamphIraiDay_DisplayFor = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.pamphIraiDay));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].PamphIraiDay_DisplayFor = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.status)) == false)
				{
					targetCourse[targetCourse.Length - 1].CrsStatus = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.status));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].CrsStatus = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.entryDay)) == false)
				{
					targetCourse[targetCourse.Length - 1].EntryDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.entryDay));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].EntryDay = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.deleteYmd)) == false)
				{
					targetCourse[targetCourse.Length - 1].DeleteDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.deleteYmd));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].DeleteDay = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.crsKind1)) == false)
				{
					targetCourse[targetCourse.Length - 1].CrsKind1 = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsKind1));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].CrsKind1 = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.crsKind2)) == false)
				{
					targetCourse[targetCourse.Length - 1].CrsKind2 = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsKind2));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].CrsKind2 = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.crsKind3)) == false)
				{
					targetCourse[targetCourse.Length - 1].CrsKind3 = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsKind3));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].CrsKind3 = "";
				}
				//If IsDBNull(.GetData(row, CrsList_Koumoku.ledgerCreateDay)) = False Then
				//    targetCourse(targetCourse.Length - 1).ledgerCreateDay = CStr(.GetData(row, CrsList_Koumoku.ledgerCreateDay))
				//Else
				//    targetCourse(targetCourse.Length - 1).ledgerCreateDay = ""
				//End If
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.WEBSystemRendoYmd)) == false)
				{
					targetCourse[targetCourse.Length - 1].WebRendoDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.WEBSystemRendoYmd));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].WebRendoDay = "";
				}
				if (Information.IsDBNull(with_1.GetData(row, CrsList_Koumoku.bulkOpenDay)) == false)
				{
					targetCourse[targetCourse.Length - 1].BulkOpenDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.bulkOpenDay));
				}
				else
				{
					targetCourse[targetCourse.Length - 1].BulkOpenDay = "";
				}
				result = true;
			}
		}
		return result;
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
		clsCrsSearch_DA.Theme = _searchConditions_Theme;
		clsCrsSearch_DA.CrsName = _searchConditions_CrsName;
		clsCrsSearch_DA.CrsName_Kana = _searchConditions_CrsNameKana;

		clsCrsSearch_DA.CrsKbn1 = _searchConditions_CrsKbn1;
		clsCrsSearch_DA.CrsKbn2 = _searchConditions_CrsKbn2;
		clsCrsSearch_DA.CrsKbn3 = _searchConditions_CrsKbn3;

		clsCrsSearch_DA.DeleteWith = _searchConditions_DeleteWith;

		clsCrsSearch_DA.CostCyosei = _searchConditions_CostCyosei;
		clsCrsSearch_DA.NaiyoKakunin = _searchConditions_NaiyoKakunin;
		clsCrsSearch_DA.ToriatukaiAdd = _searchConditions_ToriatukaiAdd;
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
	/// コース選択チェック再設定
	/// </summary>
	/// <remarks></remarks>
	private void setSelectionCheck() //Subset選択チェック()
	{
		object with_1 = grdCrsList;
		for (int row = 1; row <= this.grdCrsList.Rows.Count - 1; row++)
		{
			if (IsExistsCourseInfo(System.Convert.ToString(this.cmbCrsKind1.SelectedValue),)
			{
				System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsCd)), ;
				System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.kaiteiDay)), ;
				System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.year)), ;
				System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.season)), ;
				System.Convert.ToInt32(with_1.GetData(row, CrsList_Koumoku.invalidFlg)))== true;
		with_1.SetData(row, CrsList_Koumoku.selection, true);
	}
}
	}
	
	/// <summary>
	/// 指定されたコース情報の退避選択コース情報存在チェック
	/// </summary>
	/// <param name="crsKind1"></param>
	/// <param name="crsCd"></param>
	/// <param name="kaiteiDay"></param>
	/// <param name="year"></param>
	/// <param name="season"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool IsExistsCourseInfo(string crsKind1, string crsCd, string kaiteiDay, string year, string season, int invalidFlg) //IsExistsCourseInfo(定期_企画区分,コースコード,改定日,年,季,無効フラグ)
{
	for (int cnt = 0; cnt <= this._selectionCrsInfo.Length - 1; cnt++)
	{
		if (_selectionCrsInfo[cnt].CrsKind1 == crsKind1 &&)
		{
			_selectionCrsInfo[cnt].CrsCd = crsCd.Trim() &&;
			_selectionCrsInfo[cnt].KaiteiDay = kaiteiDay.Replace("/", "") &&;
			_selectionCrsInfo[cnt].Year = year &&;
			_selectionCrsInfo[cnt].Season = season &&;
			_selectionCrsInfo[cnt].InvalidFlg = invalidFlg;
			return true;
		}
	}
	return false;
}

/// <summary>
/// コース選択チェック再設定
/// </summary>
/// <remarks></remarks>
private void setSelectionLine() //選択行()
{
	object with_1 = grdCrsList;
	for (int row = 1; row <= this.grdCrsList.Rows.Count - 1; row++)
	{
		if (_displayTaisyoCrsInfo[0].CrsKind1 == System.Convert.ToString(this.cmbCrsKind1.SelectedValue) &&)
		{
			_displayTaisyoCrsInfo[0].CrsCd.Trim = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.crsCd)).Trim() &&;
			_displayTaisyoCrsInfo[0].KaiteiDay = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "") &&;
			_displayTaisyoCrsInfo[0].Year = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.year)) &&;
			_displayTaisyoCrsInfo[0].Season = System.Convert.ToString(with_1.GetData(row, CrsList_Koumoku.season)) &&;
			_displayTaisyoCrsInfo[0].InvalidFlg = System.Convert.ToInt32(with_1.GetData(row, CrsList_Koumoku.invalidFlg));
			with_1.Row = row;
			return;
		}
	}
}

/// <summary>
/// 該当コースのレコードをLOCKする
/// </summary>
/// <returns></returns>
/// <remarks></remarks>
private bool lockCrsMaster(CrsMasterKeyKoumoku lockTaisyoCrsInfo) //lockコースマスタ(ロック対象コース情報)
{

	try
	{
		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報
		FixedCd.CrsState retValue = null; //retValue
		string lockUser = ""; //lockUser

		Cursor = Cursors.WaitCursor;

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		retValue = clsCrsLock.ExecuteCourseLockMain(lockTaisyoCrsInfo, userInfo, lockUser);

		if (retValue != CrsState.lockSuccess)
		{
			if (retValue == CrsState.lockChu)
			{


				createFactoryMsg.messageDisp("0037", lockUser);
			}
			else
			{
				//TODO:共通変更対応
				//メッセージ出力.messageDisp("9001", lockUser)
				createFactoryMsg.messageDisp("9001", lockUser);
			}
			return false;
		}
	}
	catch (OracleException ex)
	{
		//TODO:共通変更対応
		//メッセージ出力.messageDisp("0006", ex.Number.ToString)
		createFactoryMsg.messageDisp("0006", ex.Number.ToString());
		return false;
	}
	finally
	{
		Cursor = Cursors.Default;
	}

	return true;
}

/// <summary>
/// 該当コースのレコードLOCKを解除する
/// </summary>
/// <remarks></remarks>
private void relockCrsMaster() //コースマスタ()
{

	try
	{
		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;
		clsCrsLock.ExecuteCourseLockReleaseMain(_displayTaisyoCrsInfo[0], userInfo);
	}
	catch (OracleException)
	{
		return;
	}

	return;
}

/// <summary>
/// ファンクションキーの表示制御を行う
/// </summary>
/// <remarks></remarks>
private void setFunctionKeyEnabled()
{

	if (UserInfoManagement.addFlg != true)
	{
		base.F3Key_Visible = false;
		base.F4Key_Visible = false;
		base.F9Key_Visible = false;
		base.F10Key_Visible = false;
		base.F12Key_Visible = false;
	}

	if (UserInfoManagement.updateFlg != true)
	{
		base.F7Key_Visible = false;
		base.F11Key_Visible = false;
	}

	if (UserInfoManagement.referenceFlg != true)
	{
		base.F6Key_Visible = false;
		base.F7Key_Visible = false;
	}

}

//ADD-20120402-検索条件「年」「季」の前回情報を保持↓
/// <summary>
/// 前回検索条件を設定する
/// </summary>
/// <remarks></remarks>
private void setLastSearchInfo()
{
	this.txtYear.Text = My.Settings.OldYear.ToString();
	//定⇔企　季節情報が異なるため、以下の条件↓
	if (this.cmbSeason.Items.Count - 1 >= My.Settings.OldSeason)
	{
		this.cmbSeason.SelectedIndex = My.Settings.OldSeason;
	}
}
/// <summary>
/// 検索条件を取得する
/// </summary>
/// <remarks></remarks>
private void getLastSearchInfo()
{
	My.Settings.OldYear = this.txtYear.Text;
	My.Settings.OldSeason = this.cmbSeason.SelectedIndex;
	My.Settings.Default.Save();
}
//ADD-20120402-検索条件「年」「季」の前回情報を保持↑

//ADD-20130218-選択コース情報にフォーカスを戻す↓
/// <summary>
/// 一覧から選択されているコース情報を取得します。
/// </summary>
/// <remarks></remarks>
private void getSelectionCrsInfo() //get選択コース情報()
{
	_displayTaisyoCrsInfo = new CrsMasterKeyKoumoku[1]; //_表示対象コース情報(0)
	int row = System.Convert.ToInt32(this.grdCrsList.Row); //row

	CrsMasterKeyKoumoku with_1 = _displayTaisyoCrsInfo[0];
	with_1.Teiki_KikakuKbn = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.teikiKikakuKbn));
	with_1.CrsCd = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsCd)).Trim();
	with_1.Year = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.year));
	with_1.Season = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.season));
	with_1.KaiteiDay = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "");
	with_1.InvalidFlg = System.Convert.ToInt32(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg));
	if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind1)) == true)
	{
		with_1.CrsKind1 = "";
	}
	else
	{
		with_1.CrsKind1 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind1));
	}
	if (Information.IsDBNull(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2)) == true)
	{
		with_1.CrsKind2 = "";
	}
	else
	{
		with_1.CrsKind2 = System.Convert.ToString(grdCrsList.GetData(row, CrsList_Koumoku.crsKind2));
	}
}
//ADD-20130218-選択コース情報にフォーカスを戻す↑

//ADD-20130218-選択コース情報にフォーカスを戻す↓
/// <summary>
/// コースマスタキー項目DeepCopy
/// 直接渡すと参照渡しとなるため
/// </summary>
/// <param name="_toBasicInfo"></param>
/// <param name="_fromCrsSearch"></param>
/// <remarks></remarks>
private void setBasicInfo_CrsInfo(ref CrsMasterKeyKoumoku[] _toBasicInfo, CrsMasterKeyKoumoku[] _fromCrsSearch) //set基本情報_コース情報(_To基本情報,_Fromコース検索)
{

	_toBasicInfo = new CrsMasterKeyKoumoku[1]; //_To基本情報(0)

	for (int cnt = 0; cnt <= (_fromCrsSearch.Length - 1); cnt++)
	{
		Array.Resize(ref _toBasicInfo, cnt + 1); //Preserve_To基本情報(cnt)

		CrsMasterKeyKoumoku with_1 = _toBasicInfo[cnt];
		with_1.Teiki_KikakuKbn = _fromCrsSearch[cnt].Teiki_KikakuKbn;
		with_1.CrsCd = _fromCrsSearch[cnt].CrsCd;
		with_1.Year = _fromCrsSearch[cnt].Year;
		with_1.Season = _fromCrsSearch[cnt].Season;
		with_1.Season_DisplayFor = _fromCrsSearch[cnt].Season_DisplayFor;
		with_1.KaiteiDay = _fromCrsSearch[cnt].KaiteiDay;
		with_1.InvalidFlg = _fromCrsSearch[cnt].InvalidFlg;
		with_1.WebRendoDay = _fromCrsSearch[cnt].WebRendoDay;
		with_1.CrsStatus = _fromCrsSearch[cnt].CrsStatus;
		with_1.CrsKind1 = _fromCrsSearch[cnt].CrsKind1;
		with_1.CrsKind2 = _fromCrsSearch[cnt].CrsKind2;
		with_1.CrsName = _fromCrsSearch[cnt].CrsName;
		with_1.PamphIraiDay_DisplayFor = _fromCrsSearch[cnt].PamphIraiDay_DisplayFor;
		with_1.BulkOpenDay = _fromCrsSearch[cnt].BulkOpenDay;
		with_1.DeleteDay = _fromCrsSearch[cnt].DeleteDay;
		with_1.ApplicationStartDay = _fromCrsSearch[cnt].ApplicationStartDay;
		with_1.EntryDay = _fromCrsSearch[cnt].EntryDay;
		with_1.YoyakuRendoDay = _fromCrsSearch[cnt].YoyakuRendoDay;
		with_1.RendoMon = _fromCrsSearch[cnt].RendoMon;
		with_1.RosenzuFile = _fromCrsSearch[cnt].RosenzuFile;

	}

}
//ADD-20130218-選択コース情報にフォーカスを戻す↑  ''ADD-20130218-選択コース情報にフォーカスを戻す↑

#region コースマスタ存在チェック
/// <summary>
/// コースマスタ存在チェック
/// </summary>
/// <param name="selectCrsInfo"></param>
/// <returns></returns>
private bool checkExistsCrsMaster(CrsMasterKeyKoumoku[] selectCrsInfo)
{

	crsMasterOperation_DA crsMasterOperation_DA = new crsMasterOperation_DA();
	DataTable crsDt = null;

	for (int row = 0; row <= selectCrsInfo.Length - 1; row++)
	{

		// コースマスタ(基本)検索
		crsDt = crsMasterOperation_DA.GetDatacrsMaster(selectCrsInfo[row]);

		if (ReferenceEquals(crsDt, null) ||)
		{
			crsDt.Rows.Count(<= 0);
			// 指定した{1}が存在しません。
			createFactoryMsg.messageDisp("0074", "コースコード (" + selectCrsInfo[row].CrsCd.ToString().TrimEnd + ") ");
			// エラー発生で抜ける
			return false;
		}
	}
	return true;
}
#endregion

#region 台帳作成

#region 選択データ取得
/// <summary>
/// 選択データ取得
/// </summary>
private DataTable getListSelectionData()
{

	DataTable grdData = this.grdCrsList.DataSource;

	DataTable selectDataTable = grdData.Clone;

	//2019/2/20 tani
	DataRow[] drRow = grdData.Select("CHECK_FLG = 1");
	if (drRow.Count() > 0)
	{
		foreach (DataRow dr in drRow)
		{
			selectDataTable.ImportRow(dr);
		}
	}
	return selectDataTable;
}
#endregion

#region 台帳作成
/// <summary>
/// 台帳作成
/// </summary>
/// <param name="selectDataTable"></param>
/// <returns></returns>
private bool registCrsLedger(DataTable selectDataTable)
{
	bool returnValue = false;
	returnValue = showRegistCrsLedger(selectDataTable);

	if (returnValue == true)
	{

		// コース一覧を再表示
		dispCrsList();

		this.grdCrsList.Focus();

	}

	return returnValue;
}
#endregion

#region 台帳作成画面表示
/// <summary>
/// 台帳作成画面表示
/// </summary>
/// <param name="selectDataTable"></param>
/// <returns></returns>
private bool showRegistCrsLedger(DataTable selectDataTable)
{
	bool returnValue = false;

	try
	{
		S01_0321 crsLedger = new S01_0321();
		crsLedger.SelectDataTable = selectDataTable;
		DialogResult ret = crsLedger.ShowDialog(this);
		return ret.Equals(DialogResult.OK);
	}
	catch (Exception)
	{
		throw;
	}
	return returnValue;
}
#endregion

#endregion

#endregion

#region 共通対応
protected override void StartupOrgProc()
{
	F1Key_Visible = false; // F1:未使用
	F2Key_Visible = true; // F2:戻る
	F3Key_Visible = true; // F3:パンフ
	F4Key_Visible = true; // F4:削除
	F5Key_Visible = true; // F5:一覧照会
	F6Key_Visible = true; // F6:参照
	F7Key_Visible = true; // F7:印刷
	F8Key_Visible = false; // F8:未使用
	F9Key_Visible = true; // F9:コピー
	F10Key_Visible = true; // F10:新規
	F11Key_Visible = true; // F11:更新
	F12Key_Visible = true; // F12:台帳作成
	F10Key_Text = "F10:新規";
	F12Key_Text = "F12:台帳作成";
	F3Key_Text = "F3:パンフ";
	F5Key_Text = "F5:一覧照会";
	F6Key_Text = "F6:参照";
	F9Key_Text = "F9:コピー";
}
#endregion
	
}