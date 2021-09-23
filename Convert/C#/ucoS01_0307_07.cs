using C1.Win.C1FlexGrid;


/// <summary>
/// 媒体ユーザーコントロール
/// </summary>
public class ucoS01_0307_07
{

	#region 変数

	/// <summary>
	/// コース情報管理ユーザーコントロール共通
	/// </summary>
	private ucoS01_0307_Common _ucoCommon;

	// 媒体 最大行
	private int _mediaNameList_MaxRow = 10;

	// 掲載写真 最大行
	private int _keisaiImageList_MaxRow = 3;

	#endregion

	#region 列挙
	#region 媒体名グリッドカラム
	/// <summary>
	/// 媒体名一覧カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum MediaNameList_Koumoku : int
	{
		[Value("媒体名")]
		mediaName = 1,
		[Value("ページ")]
		page
	}
	#endregion

	/// <summary>
	/// 掲載写真一覧カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum KeisaiImageList_Koumoku : int
	{
		[Value("掲載写真")]
		keisaiImage_Left = 1,
		[Value("掲載写真")]
		keisaiImage_Right = 2
	}

	#endregion

	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ucoS01_0307_07()
	{

		// この呼び出しはデザイナーで必要です。
		InitializeComponent();

		// InitializeComponent() 呼び出しの後で初期化を追加します。

		// コース情報管理ユーザーコントロール共通
		_ucoCommon = new ucoS01_0307_Common();

		// コントロール値のクリア
		_ucoCommon.clearContorol(this);

	}
	#endregion

	#region イベント

	/// <summary>
	/// 媒体名一覧
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void GrdMediaNameList_StartEdit(System.Object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{

		if (e.Col == MediaNameList_Koumoku.page)
		{
			TextBoxEx txtPage = new TextBoxEx();
			txtPage.Format = "9";
			txtPage.MaxLength = 3;
			txtPage.ContentAlignment = ContentAlignment.MiddleRight;
			grdMediaNameList.Editor = txtPage;
		}

	}

	/// <summary>
	/// 掲載写真一覧
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void GrdKeisaiImageList_StartEdit(System.Object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{

		if ((e.Col == KeisaiImageList_Koumoku.keisaiImage_Left) || (e.Col == KeisaiImageList_Koumoku.keisaiImage_Right))
		{
			TextBoxEx txtKeisaiImage = new TextBoxEx();
			txtKeisaiImage.MaxLength = 24;
			txtKeisaiImage.Format = "^Ｔ";
			grdKeisaiImageList.Editor = txtKeisaiImage;
		}

	}

	/// <summary>
	/// パンフフラグ（女性専用席）クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void chkSetNaiyo01_CheckedChanged(object sender, EventArgs e)
	{

		((S01_0307)ParentForm).ucoS01_0307_09.chkWomenSeat.Checked = this.chkSetNaiyo01.Checked;

	}

	/// <summary>
	/// パンフフラグ（１名参加ＯＫ）クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void chkSetNaiyo14_CheckedChanged(object sender, EventArgs e)
	{

		// [定期]の場合は処理を抜ける ※[お二人様より出発]
		if (((S01_0307)ParentForm).TeikiKikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			return;
		}

		// [企画]の場合は予約タブ[１名参加可]と連動 ※[１名参加可]
		((S01_0307)ParentForm).ucoS01_0307_09.chkOneSankaOk.Checked = this.chkSetNaiyo14.Checked;

	}

	#endregion

	#region メソッド

	/// <summary>
	/// コントロール初期化
	/// </summary>
	public void setControlInitialize()
	{

		// コントロール値のクリア
		_ucoCommon.clearContorol(this);

		// グリッド初期化
		this.SetGridInitialize();

		//パンフフラグの内容を元に、「設定内容」を表示
		this.setPamphletSetNaiyo();

	}

	/// <summary>
	/// 媒体情報の内容を画面に反映する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	private void setEntityDataToScreenMediaInfo(CrsMasterEntity crsMasterEntity)
	{

		object with_1 = crsMasterEntity.MediaInfoEntity;

		for (int row = 0; row <= with_1.EntityData.Length - 1; row++)
		{

			if ((this.grdMediaNameList.Rows.Count - 1) < (row + 1))
			{
				break;
			}

			this.grdMediaNameList.SetData(row + 1, MediaNameList_Koumoku.mediaName, with_1.EntityData(row).MediaKind.Value);

			if (ReferenceEquals(with_1.EntityData(row).KeisaiPage.Value, null) == false)
			{
				this.grdMediaNameList.SetData(row + 1, MediaNameList_Koumoku.page, with_1.EntityData(row).KeisaiPage.Value);
			}

		}


	}

	/// <summary>
	/// 掲載写真の内容を画面に反映する
	/// </summary>
	/// <param name="crsMasterEntity"></param>
	private void setEntityDataToScreenKeisaiImageList(CrsMasterEntity crsMasterEntity)
	{

		object with_1 = this.grdKeisaiImageList;
		with_1.SetData(1, KeisaiImageList_Koumoku.keisaiImage_Left, crsMasterEntity.Image1.Value);
		with_1.SetData(1, KeisaiImageList_Koumoku.keisaiImage_Right, crsMasterEntity.Image2.Value);

		with_1.SetData(2, KeisaiImageList_Koumoku.keisaiImage_Left, crsMasterEntity.Image3.Value);
		with_1.SetData(2, KeisaiImageList_Koumoku.keisaiImage_Right, crsMasterEntity.Image4.Value);

		with_1.SetData(3, KeisaiImageList_Koumoku.keisaiImage_Left, crsMasterEntity.Image5.Value);
		with_1.SetData(3, KeisaiImageList_Koumoku.keisaiImage_Right, crsMasterEntity.Image6.Value);

	}

	/// <summary>
	/// 必須入力チェック（媒体）
	/// </summary>
	/// <returns></returns>
	public bool checkRequiredKoumoku_grdMediaNameList()
	{

		bool ret = true;

		object with_1 = this.grdMediaNameList;

		for (int row = 1; row <= with_1.Rows.Count - 1; row++)
		{

			// 媒体名のみ入力されている場合
			if ((_ucoCommon.nnvl_str(with_1.GetData(row, MediaNameList_Koumoku.mediaName)).Trim() != "") &&)
			{
				(_ucoCommon.nnvl_str(with_1.GetData(row, MediaNameList_Koumoku.page)).Trim() == "");

				CellRange range = new CellRange();
				range = with_1.GetCellRange(row, MediaNameList_Koumoku.page);
				range.Style = with_1.Styles("入力エラー");
				with_1.Row = row;
				with_1.Col = MediaNameList_Koumoku.page;

				ret = false;

			}

			// ページのみ入力されている場合
			if ((_ucoCommon.nnvl_str(with_1.GetData(row, MediaNameList_Koumoku.mediaName)).Trim() == "") &&)
			{
				(_ucoCommon.nnvl_str(with_1.GetData(row, MediaNameList_Koumoku.page)).Trim() != "");

				CellRange range = new CellRange();
				range = with_1.GetCellRange(row, MediaNameList_Koumoku.mediaName);
				range.Style = with_1.Styles("入力エラー");
				with_1.Row = row;
				with_1.Col = MediaNameList_Koumoku.mediaName;

				ret = false;

			}

		}


		return ret;
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <returns></returns>
	public string checkInputNaiyo()
	{

		string errMsg = string.Empty;

		// 見所
		object with_1 = this.txtMidokoro;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_1.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_1.ExistError = true;
			with_1.Focus();
			return errMsg;
		}

		// パンフレット注意事項
		object with_2 = this.txtPamphletTyuijikou;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_2.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_2.ExistError = true;
			with_2.Focus();
			return errMsg;
		}
		// パンフレット作成指示
		object with_3 = this.txtPamphletCreateSiji;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_3.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_3.ExistError = true;
			with_3.Focus();
			return errMsg;
		}

		// 入力チェック（掲載写真一覧）
		errMsg = checkInputNaiyo_grdKeisaiImageList();

		return errMsg;
	}

	/// <summary>
	/// 入力チェック（掲載写真一覧）
	/// </summary>
	/// <returns></returns>
	private string checkInputNaiyo_grdKeisaiImageList()
	{

		string errMsg = string.Empty;

		object with_1 = this.grdKeisaiImageList;

		for (int row = 1; row <= with_1.Rows.Count - 1; row++)
		{

			// 掲載写真（左）
			errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(Convert.ToString(this.grdKeisaiImageList.Item(row, KeisaiImageList_Koumoku.keisaiImage_Left))));
			if (!(string.IsNullOrEmpty(errMsg)))
			{

				CellRange range = new CellRange();
				range = with_1.GetCellRange(row, KeisaiImageList_Koumoku.keisaiImage_Left);
				range.Style = with_1.Styles("入力エラー");
				with_1.Select(row, KeisaiImageList_Koumoku.keisaiImage_Left);

				break;

			}

			// 掲載写真（右）
			errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(Convert.ToString(this.grdKeisaiImageList.Item(row, KeisaiImageList_Koumoku.keisaiImage_Right))));
			if (!(string.IsNullOrEmpty(errMsg)))
			{

				CellRange range = new CellRange();
				range = with_1.GetCellRange(row, KeisaiImageList_Koumoku.keisaiImage_Right);
				range.Style = with_1.Styles("入力エラー");
				with_1.Select(row, KeisaiImageList_Koumoku.keisaiImage_Right);

				break;

			}

		}


		return errMsg;
	}

	/// <summary>
	/// グリッドのエラー表示のクリアを行う
	/// </summary>
	public void clearErrinfoGrid()
	{

		object with_1 = grdMediaNameList;
		C1.Win.C1FlexGrid.CellRange rangeNaiyo = new C1.Win.C1FlexGrid.CellRange();
		rangeNaiyo = with_1.GetCellRange(1, MediaNameList_Koumoku.mediaName, with_1.Rows.Count - 1, MediaNameList_Koumoku.page);
		rangeNaiyo.Style = with_1.Styles("入力エラー解除");

		object with_2 = grdKeisaiImageList;
		C1.Win.C1FlexGrid.CellRange rangeNaiyo = new C1.Win.C1FlexGrid.CellRange();
		rangeNaiyo = with_2.GetCellRange(1, KeisaiImageList_Koumoku.keisaiImage_Left, with_2.Rows.Count - 1, KeisaiImageList_Koumoku.keisaiImage_Left);
		rangeNaiyo.Style = with_2.Styles("入力エラー解除");

		rangeNaiyo = with_2.GetCellRange(1, KeisaiImageList_Koumoku.keisaiImage_Right, with_2.Rows.Count - 1, KeisaiImageList_Koumoku.keisaiImage_Right);
		rangeNaiyo.Style = with_2.Styles("入力エラー解除");

	}

	/// <summary>
	/// グリッド初期化
	/// </summary>
	private void SetGridInitialize()
	{

		// -------------------------------------------------
		// グリッドユーザーコントロールに実装すれば不要
		// -------------------------------------------------
		// グリッド共通設定
		_ucoCommon.setGridCommonInitialize(this.grdMediaNameList);
		_ucoCommon.setGridCommonInitialize(this.grdKeisaiImageList);

		// 媒体
		setGridInitialize_grdMediaNameList();

		// 掲載写真
		setGridInitialize_grdKeisaiImageList();

	}

	/// <summary>
	/// グリッド初期化（媒体）
	/// </summary>
	private void setGridInitialize_grdMediaNameList()
	{

		MediaInfoEntity mediaInfoEntity = new MediaInfoEntity();

		object with_1 = grdMediaNameList;

		object with_2 = with_1.Cols(MediaNameList_Koumoku.mediaName);
		with_2.Name = mediaInfoEntity.MediaKind.PhysicsName; //"MEDIA_KIND"
		with_2.Caption = getEnumAttrValue(MediaNameList_Koumoku.mediaName);
		with_2.Width = 391;
		with_2.DataType = typeof(string);
		with_2.AllowEditing = true;

		object with_3 = with_1.Cols(MediaNameList_Koumoku.page);
		with_3.Name = mediaInfoEntity.KeisaiPage.PhysicsName; //"KEISAI_PAGE"
		with_3.Caption = getEnumAttrValue(MediaNameList_Koumoku.page);
		with_3.Width = 125;
		with_3.DataType = typeof(string);
		with_3.AllowEditing = true;
		with_3.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightCenter;

		// 明細９行
		with_1.Rows.Count = 1 + _mediaNameList_MaxRow;

		with_1.Styles.Add("入力エラー");
		with_1.Styles("入力エラー").BackColor = Color.Red;
		with_1.Styles.Add("入力エラー解除");

		// 媒体情報の媒体名コンボへ値をセット
		setMediaNameCmbData();

	}

	/// <summary>
	/// 媒体一覧のコンボボックスに値を設定
	/// </summary>
	private void setMediaNameCmbData()
	{

		if (ReferenceEquals(((S01_0307)ParentForm), null))
		{
			return;
		}

		//' コース種別１取得（現行SRC:コース種別１と同等レベル）
		//Dim oldCrsKinds As OldCrsKinds = DirectCast(ParentForm, S01_0307).OldCrsKinds
		//' コース種別２取得
		//Dim crsKind2 As String = _ucoCommon.getItemValueForComboBox(DirectCast(ParentForm, S01_0307).cmbCrsKind2)

		CdMasterGet_DA clsCdMasterGet_DA = new CdMasterGet_DA();
		DataTable dtMediaName = clsCdMasterGet_DA.GetCodeMasterData(CdBunruiType.mediaMaster, true);
		Dictionary dicMediaName = new Dictionary(Of string, string);

		if (((S01_0307)ParentForm).CrsKinds.kikaku == true)
		{
			//企画↓
			if (((S01_0307)ParentForm).CrsKinds.stay == true)
			{
				//宿泊↓
				dicMediaName = getSortMediaName(getEnumAttrValue(FixedCd.CrsKind2.stay), dtMediaName);
			}
			else
			{
				//日帰り↓
				dicMediaName = getSortMediaName(getEnumAttrValue(FixedCd.CrsKind2.higaeri), dtMediaName);
			}
		}
		else
		{
			//定期↓
			for (int idx = 0; idx <= dtMediaName.Rows.Count - 1; idx++)
			{
				dicMediaName.Add(dtMediaName[idx].Item("CODE_VALUE").ToString(), dtMediaName[idx].Item("CODE_NAME").ToString());
			}
		}

		this.grdMediaNameList.Cols(MediaNameList_Koumoku.mediaName).DataMap = dicMediaName;

	}

	/// <summary>
	/// グリッド初期化（掲載写真）
	/// </summary>
	private void setGridInitialize_grdKeisaiImageList()
	{

		object with_1 = grdKeisaiImageList;

		object with_2 = with_1.Cols(KeisaiImageList_Koumoku.keisaiImage_Left);
		with_2.Name = KeisaiImageList_Koumoku.keisaiImage_Left.ToString();
		with_2.Caption = getEnumAttrValue(KeisaiImageList_Koumoku.keisaiImage_Left);
		with_2.Width = 327;
		with_2.DataType = typeof(string);
		with_2.AllowEditing = true;

		object with_3 = with_1.Cols(KeisaiImageList_Koumoku.keisaiImage_Right);
		with_3.Name = KeisaiImageList_Koumoku.keisaiImage_Right.ToString();
		with_3.Caption = getEnumAttrValue(KeisaiImageList_Koumoku.keisaiImage_Right);
		with_3.Width = 327;
		with_3.DataType = typeof(string);
		with_3.AllowEditing = true;

		// ヘッダーマージ
		with_1.AllowMerging = AllowMergingEnum.FixedOnly;
		with_1.Rows(0).AllowMerging = true;
		CellRange rng = with_1.GetCellRange(0, KeisaiImageList_Koumoku.keisaiImage_Left, 0, KeisaiImageList_Koumoku.keisaiImage_Right);

		// 明細３行
		with_1.Rows.Count = 1 + _keisaiImageList_MaxRow;

		with_1.Styles.Add("入力エラー");
		with_1.Styles("入力エラー").BackColor = Color.Red;
		with_1.Styles.Add("入力エラー解除");


	}

	/// <summary>
	/// コース種別別に媒体名をソート
	/// </summary>
	/// <remarks></remarks>
	private object getSortMediaName()
	{
		ByVal data DataTable) Dictionary(Of string, string);
		Dictionary returnDic = new Dictionary(Of string, string);
		DataRow[] row0 = null;
		DataRow[] row1 = null;
		DataRow[] row2 = null;

		//NULLRecord判断↓
		row0 = data.Select("CODE_VALUE = ' '");
		for (int index = 0; index <= row0.Count() - 1; index++)
		{
			returnDic.Add(" ", string.Empty);
		}

		row1 = data.Select("CODE_NAME LIKE '%" + sortName + "%'");
		for (int index = 0; index <= row1.Count() - 1; index++)
		{
			returnDic.Add(row1[index][System.Convert.ToInt32("CODE_VALUE")].ToString(), row1[index][System.Convert.ToInt32("CODE_NAME")].ToString());
		}

		row2 = data.Select("CODE_NAME NOT LIKE '%" + sortName + "%'");
		for (int index = 0; index <= row2.Count() - 1; index++)
		{
			returnDic.Add(row2[index][System.Convert.ToInt32("CODE_VALUE")].ToString(), row2[index][System.Convert.ToInt32("CODE_NAME")].ToString());
		}

		return returnDic;
	}

	/// <summary>
	/// パンフ設定内容の表示を行う
	/// </summary>
	/// <remarks></remarks>
	private bool setPamphletSetNaiyo()
	{

		CdBunruiType cdBunrui = null;
		CdMasterGet_DA clsCdMasterGet_DA = new CdMasterGet_DA();
		CheckBoxEx setNaiyoCheckBox;

		//全て非表示とする
		foreach (Control ctl in this.flpSetNaiyo.Controls)
		{
			if (ReferenceEquals(ctl.GetType, typeof(CheckBoxEx)))
			{
				setNaiyoCheckBox = (CheckBoxEx)ctl;
				setNaiyoCheckBox.Visible = false;
			}
		}

		if (((S01_0307)ParentForm).TeikiKikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			cdBunrui = CdBunruiType.pamphFlg_Teiki;
		}
		else
		{
			cdBunrui = CdBunruiType.pamphFlg_Kikaku;
		}

		DataTable dtPamphletNaiyo = clsCdMasterGet_DA.GetCodeMasterData(cdBunrui);

		for (int idx = 0; idx <= dtPamphletNaiyo.Rows.Count - 1; idx++)
		{

			string setNaiyoNo = "";
			CheckBoxEx targetCheckBox = null;
			Control ctl = null;

			setNaiyoNo = System.Convert.ToString(dtPamphletNaiyo.Rows(idx).Item("CODE_VALUE"));

			foreach (Control tempLoopVar_ctl in this.flpSetNaiyo.Controls)
			{
				ctl = tempLoopVar_ctl;
				if (ctl.Name == "chkSetNaiyo" + setNaiyoNo)
				{
					targetCheckBox = (CheckBoxEx)ctl;
					targetCheckBox.Text = System.Convert.ToString(dtPamphletNaiyo.Rows(idx).Item("CODE_NAME")).Trim();
					targetCheckBox.Visible = true;
				}
			}

			if (ReferenceEquals(targetCheckBox, null) == true)
			{
				return false;
			}
		}

		return true;

	}


	/// <summary>
	/// 項目の表示＿非表示設定を行う
	/// </summary>
	/// <param name="crsKinds"></param>
	public void setVisibleControl(CrsKinds crsKinds)
	{

		//'「外客」の場合、設定内容01～04は表示しない
		//If crsKinds.english = True Then
		//    'chkSetNaiyo01.Visible = False
		//    'chkSetNaiyo02.Visible = False
		//    'chkSetNaiyo03.Visible = False
		//    'chkSetNaiyo04.Visible = False
		//    Call _ucoCommon.changeEnableProperty(Me.chkSetNaiyo01, False, True)
		//    Call _ucoCommon.changeEnableProperty(Me.chkSetNaiyo02, False, True)
		//    Call _ucoCommon.changeEnableProperty(Me.chkSetNaiyo03, False, True)
		//    Call _ucoCommon.changeEnableProperty(Me.chkSetNaiyo04, False, True)
		//End If

		//「外客」の場合、設定内容01～04は表示しない
		//   DQ-0254 対応 [企画・外国語]は使用可とする ([定期・外国語]の場合のみ使用不可とする)
		if (crsKinds.kikaku == false)
		{
			if (crsKinds.english == true)
			{
				_ucoCommon.changeEnableProperty(this.chkSetNaiyo01, false, true);
				_ucoCommon.changeEnableProperty(this.chkSetNaiyo02, false, true);
				_ucoCommon.changeEnableProperty(this.chkSetNaiyo03, false, true);
				_ucoCommon.changeEnableProperty(this.chkSetNaiyo04, false, true);
			}
		}

	}

	/// <summary>
	/// エンティティに格納されているデータを画面に反映する
	/// </summary>
	/// <param name="crsMasterEntity"></param>
	public void setEntityDataToScreen(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity with_1 = crsMasterEntity;

		//見所
		this.txtMidokoro.Text = with_1.ZentaiMidokoroInfo.Value;

		//パンフレット注意事項
		this.txtPamphletTyuijikou.Text = with_1.Tyuijikou.Value;

		//パンフレット作成指示
		this.txtPamphletCreateSiji.Text = with_1.CreateSiji.Value;


		// パンフフラグの内容を画面に反映する
		setEntityDataToScreenPamphletFlg(crsMasterEntity);

		// 媒体情報の媒体名コンボへ値をセット（コース種別により内容が異なるため再セットする）
		setMediaNameCmbData();

		// 媒体情報の内容を画面に反映する
		setEntityDataToScreenMediaInfo(crsMasterEntity);

		// 掲載写真一覧の内容を画面に反映する
		setEntityDataToScreenKeisaiImageList(crsMasterEntity);

	}

	/// <summary>
	/// エンティティに格納されているデータ(パンフフラグの内容)を画面に反映する
	/// </summary>
	/// <remarks></remarks>
	private void setEntityDataToScreenPamphletFlg(CrsMasterEntity crsMasterEntity)
	{

		string setNaiyoNo = "";

		CrsMasterEntity with_1 = crsMasterEntity;

		for (int idx = 0; idx <= with_1.PamphFlgEntity.EntityData.Length - 1; idx++)
		{
			setNaiyoNo = System.Convert.ToString(with_1.PamphFlgEntity.EntityData(idx).FlgCd.Value);

			foreach (Control ctl in this.flpSetNaiyo.Controls)
			{
				if (ctl.Name == "chkSetNaiyo" + setNaiyoNo)
				{
					((CheckBoxEx)ctl).Checked = true;
				}
			}
		}


	}

	/// <summary>
	/// 画面の内容をエンティティに格納する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	/// <returns>コースマスタエンティティ</returns>
	public CrsMasterEntity setScreenDataToEntity(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity retCrsMasterEntity = crsMasterEntity;


		//見所
		retCrsMasterEntity.ZentaiMidokoroInfo.Value = this.txtMidokoro.Text;


		//マーク
		if (crsMasterEntity.Teiki_KikakuKbn.Equals(Teiki_KikakuKbnType.teikiKanko))
		{
			//レディースシート
			retCrsMasterEntity.JyoseiSenyoSeatFlg.Value = _ucoCommon.getItemValueForCheckBox(this.chkSetNaiyo01);
			//18才未満禁止
			retCrsMasterEntity._18OldUnderKinsi.Value = _ucoCommon.getItemValueForCheckBox(this.chkSetNaiyo02);
			//上着着用
			retCrsMasterEntity.UwagiTyakuyo.Value = _ucoCommon.getItemValueForCheckBox(this.chkSetNaiyo03);
			//ネクタイ着用
			retCrsMasterEntity.TieTyakuyo.Value = _ucoCommon.getItemValueForCheckBox(this.chkSetNaiyo04);
		}
		else
		{
			retCrsMasterEntity.JyoseiSenyoSeatFlg.Value = _ucoCommon.getItemValueForCheckBox(this.chkSetNaiyo01);
			retCrsMasterEntity._18OldUnderKinsi.Value = 0;
			retCrsMasterEntity.UwagiTyakuyo.Value = 0;
			retCrsMasterEntity.TieTyakuyo.Value = 0;
		}

		//パンフレット注意事項
		retCrsMasterEntity.Tyuijikou.Value = this.txtPamphletTyuijikou.Text;

		//パンフレット作成指示
		retCrsMasterEntity.CreateSiji.Value = this.txtPamphletCreateSiji.Text;


		//媒体情報
		retCrsMasterEntity = setScreenToEntityDataMediaInfo(retCrsMasterEntity);

		//掲載写真
		retCrsMasterEntity = setScreenToEntityDataKeisaiImage(retCrsMasterEntity);

		//パンフフラグ取得
		retCrsMasterEntity = setScreenToEntityDataPamphletFlg(retCrsMasterEntity);

		return retCrsMasterEntity;
	}

	/// <summary>
	/// 画面内容を媒体エンティティに格納する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	/// <returns>コースマスタエンティティ</returns>
	private CrsMasterEntity setScreenToEntityDataMediaInfo(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity retCrsMasterEntity = crsMasterEntity;

		int idxMediaInfo = 0;

		object with_1 = retCrsMasterEntity.MediaInfoEntity;

		for (int idx = 1; idx <= _mediaNameList_MaxRow; idx++)
		{
			if (idx > with_1.EntityData.Length)
			{
				MediaInfoEntity entity = new MediaInfoEntity();
				with_1.add(entity);
			}
			with_1.EntityData(idx - 1).Teiki_KikakuKbn.Value = "";

			if (ReferenceEquals(grdMediaNameList.GetData(idx, MediaNameList_Koumoku.mediaName), null) == false && grdMediaNameList.GetData(idx, MediaNameList_Koumoku.mediaName).ToString().Trim() != "" &&)
			{
				ReferenceEquals(grdMediaNameList.GetData(idx, MediaNameList_Koumoku.page), null) = false && grdMediaNameList.GetData(idx, MediaNameList_Koumoku.page).ToString().Trim() != "";

				with_1.EntityData(idxMediaInfo).Teiki_KikakuKbn.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Teiki_KikakuKbn;
				with_1.EntityData(idxMediaInfo).CrsCd.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).CrsCd;
				with_1.EntityData(idxMediaInfo).Year.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Year;
				with_1.EntityData(idxMediaInfo).Season.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Season;
				with_1.EntityData(idxMediaInfo).KaiteiDay.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).KaiteiDay;
				with_1.EntityData(idxMediaInfo).InvalidFlg.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).InvalidFlg;
				with_1.EntityData(idxMediaInfo).LineNo.Value = idxMediaInfo + 1;

				with_1.EntityData(idxMediaInfo).MediaKind.Value = System.Convert.ToString(grdMediaNameList.GetData(idx, MediaNameList_Koumoku.mediaName));
				if (ReferenceEquals(grdMediaNameList.GetData(idx, MediaNameList_Koumoku.page), null) == false)
				{
					with_1.EntityData(idxMediaInfo).KeisaiPage.Value = System.Convert.ToString(grdMediaNameList.GetData(idx, MediaNameList_Koumoku.page));
				}
				else
				{
					with_1.EntityData(idxMediaInfo).KeisaiPage.Value = "";
				}

				idxMediaInfo++;
			}
		}

		return retCrsMasterEntity;
	}

	/// <summary>
	/// 画面内容をコースマスタエンティティに格納する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	/// <returns>コースマスタエンティティ</returns>
	private CrsMasterEntity setScreenToEntityDataKeisaiImage(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity retCrsMasterEntity = crsMasterEntity;


		//掲載写真一覧

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(1, KeisaiImageList_Koumoku.keisaiImage_Left), null) == false)
		{
			retCrsMasterEntity.Image1.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(1, KeisaiImageList_Koumoku.keisaiImage_Left));
		}
		else
		{
			retCrsMasterEntity.Image1.Value = "";
		}

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(1, KeisaiImageList_Koumoku.keisaiImage_Right), null) == false)
		{
			retCrsMasterEntity.Image2.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(1, KeisaiImageList_Koumoku.keisaiImage_Right));
		}
		else
		{
			retCrsMasterEntity.Image2.Value = "";
		}

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(2, KeisaiImageList_Koumoku.keisaiImage_Left), null) == false)
		{
			retCrsMasterEntity.Image3.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(2, KeisaiImageList_Koumoku.keisaiImage_Left));
		}
		else
		{
			retCrsMasterEntity.Image3.Value = "";
		}

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(2, KeisaiImageList_Koumoku.keisaiImage_Right), null) == false)
		{
			retCrsMasterEntity.Image4.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(2, KeisaiImageList_Koumoku.keisaiImage_Right));
		}
		else
		{
			retCrsMasterEntity.Image4.Value = "";
		}

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(3, KeisaiImageList_Koumoku.keisaiImage_Left), null) == false)
		{
			retCrsMasterEntity.Image5.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(3, KeisaiImageList_Koumoku.keisaiImage_Left));
		}
		else
		{
			retCrsMasterEntity.Image5.Value = "";
		}

		if (ReferenceEquals(this.grdKeisaiImageList.GetData(3, KeisaiImageList_Koumoku.keisaiImage_Right), null) == false)
		{
			retCrsMasterEntity.Image6.Value = System.Convert.ToString(this.grdKeisaiImageList.GetData(3, KeisaiImageList_Koumoku.keisaiImage_Right));
		}
		else
		{
			retCrsMasterEntity.Image6.Value = "";
		}


		return retCrsMasterEntity;
	}

	/// <summary>
	/// 画面内容をパンフフラグエンティティに格納する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	/// <returns>コースマスタエンティティ</returns>
	private CrsMasterEntity setScreenToEntityDataPamphletFlg(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity retCrsMasterEntity = crsMasterEntity;

		CheckBoxEx chkSetNaiyo;
		int idxPamphletFlg = 0;

		for (int idx = 1; idx <= 30; idx++)
		{

			chkSetNaiyo = (CheckBoxEx)(this.flpSetNaiyo.Controls("chkSetNaiyo" + idx.ToString("0#")));

			if (chkSetNaiyo.Checked == true)
			{

				object with_1 = retCrsMasterEntity.PamphFlgEntity;

				if (idxPamphletFlg >= with_1.EntityData.Length)
				{
					PamphFlgEntity entity = new PamphFlgEntity();
					with_1.add(entity);
				}

				with_1.EntityData(idxPamphletFlg).Teiki_KikakuKbn.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Teiki_KikakuKbn;
				with_1.EntityData(idxPamphletFlg).CrsCd.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).CrsCd;
				with_1.EntityData(idxPamphletFlg).Year.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Year;
				with_1.EntityData(idxPamphletFlg).Season.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).Season;
				with_1.EntityData(idxPamphletFlg).KaiteiDay.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).KaiteiDay;
				with_1.EntityData(idxPamphletFlg).InvalidFlg.Value = ((S01_0307)ParentForm).TaisyoCrsInfo(0).InvalidFlg;
				with_1.EntityData(idxPamphletFlg).FlgCd.Value = idx.ToString("0#");

				idxPamphletFlg++;
			}
		}

		//余分なレコードは削除しておく
		object with_2 = retCrsMasterEntity.PamphFlgEntity;
		for (int idxClear = idxPamphletFlg; idxClear <= with_2.EntityData.Length - 1; idxClear++)
		{
			with_2.clear(idxClear, clearType.value);
		}

		return retCrsMasterEntity;
	}

	#endregion

}