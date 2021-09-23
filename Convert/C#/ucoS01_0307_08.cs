using C1.Win.C1FlexGrid;


/// <summary>
/// 取扱ユーザーコントロール
/// </summary>
public class ucoS01_0307_08
{

	#region 変数

	/// <summary>
	/// コース情報管理ユーザーコントロール共通
	/// </summary>
	private ucoS01_0307_Common _ucoCommon;

	public EntityOperation[] Itinerary_KoshakashoEntity;
	public DataTable TblItineraryGridKoumoku;

	#endregion

	#region 列挙
	/// <summary>
	/// 行程情報グリッドカラム
	/// </summary>
	/// <remarks></remarks>
	private enum ItineraryGridColType : int
	{
		[Value("No")]
		colLine_no = 0,
		[Value("日")]
		colDaily,
		[Value("種別")]
		colKind,
		[Value("降車ヶ所")]
		colKousya_place,
		[Value("取扱")]
		colToriatukai
	}

	#endregion

	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ucoS01_0307_08()
	{
		Itinerary_KoshakashoEntity = new EntityOperation[Of KoshakashoEntity + 1];


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
	/// 確認済みチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkKakuninAlready_Toriatukai_CheckedChanged(System.Object sender, System.EventArgs e)
	{

		if (this.chkKakuninAlready_Toriatukai.Checked == true)
		{
			this.txtKakuninAlreadyYmd_Toriatukai.Value = getDateTime;
		}
		else
		{
			this.txtKakuninAlreadyYmd_Toriatukai.Value = null;
		}

	}

	/// <summary>
	/// 行程情報[取扱]セルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdItineraryInfo_CellButtonClick(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{

		FlexGridEx grid = (FlexGridEx)sender;

		// フィルタしているため、実際のラインNoを取得
		int lineNo = 0;
		lineNo = System.Convert.ToInt32(_ucoCommon.nnvl_int(grid.Rows(e.Row).Item(ItineraryGridColType.colLine_no.ToString())));

		showToriatukaiSet(grid.Rows(e.Row).Item(ItineraryGridColType.colKousya_place).ToString(), lineNo, KoGamenCallMotoType.itinerary);

		this.setGridInitialize_grdCellBackColor();

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
		this.setGridInitialize();

	}

	/// <summary>
	/// グリッド初期化
	/// </summary>
	private void setGridInitialize()
	{

		// -------------------------------------------------
		// グリッドユーザーコントロールに実装すれば不要
		// -------------------------------------------------
		// グリッド共通設定
		_ucoCommon.setGridCommonInitialize(this.grdItineraryInfo);

		// グリッド初期化（行程情報）
		setGridInitialize_grdItineraryInfo();

		// 取扱情報の設定有無をチェックし、背景色を設定
		setGridInitialize_grdCellBackColor();

	}

	/// <summary>
	/// グリッド初期化（行程情報）
	/// </summary>
	private void setGridInitialize_grdItineraryInfo()
	{


		Color _selectionLine_BackgroundColor = FixedCd.BackgroundColorType.SelectionLine_BackgroundColor;
		Color _selectionLine_MojiColor = SystemColors.HighlightText;


		SortedList siireKind = new SortedList(Of string, string);
		DataTable _tblSuppliersKind_Itinerary = null;

		//Dim _kindJoken_Itinerary As String = String.Empty
		//_kindJoken_Itinerary = getKind_ItineraryConditions()
		//If _kindJoken_Itinerary <> String.Empty Then
		//    _kindJoken_Itinerary &= " AND "
		//End If
		//'ADD-20121116-ﾎﾃﾙ(ｸｰﾎﾟﾝ)を追加（行程情報に表示されないよう制御）
		//'_種別条件_行程 &= " NAIYO_1 <> " & CStr(行程種別タイプ.その他)
		//_kindJoken_Itinerary &= " NAIYO_1 IN ('" & CStr(CdMaster_SuppliersKindType.all) & "', '" & CStr(CdMaster_SuppliersKindType.itinerary) & "') "
		//_tblSuppliersKind_Itinerary = _cdMasterGet.GetCodeMasterData(CdBunruiType.suppliersKindMaster, False, _kindJoken_Itinerary)
		//TODO:naga ↓ 修正
		// 行程情報 種別 （ユーザーコントロール共通 より取得 ※同期を取るため）
		_tblSuppliersKind_Itinerary = _ucoCommon.getTblSuppliersKind_Itinerary(CdMaster_SuppliersKindType.itinerary, ((S01_0307)ParentForm).CrsKinds);
		object with_1 = this.grdItineraryInfo;
		with_1.Clear(ClearFlags.All);
		with_1.Styles.Clear();
		with_1.Rows.Count = 1;
		with_1.Rows.Fixed = 1;
		with_1.Cols.Count = 5;
		with_1.Cols.Frozen = 1;
		with_1.AllowSorting = AllowSortingEnum.None;
		with_1.AllowDragging = AllowDraggingEnum.None;
		with_1.ShowButtons = ShowButtonsEnum.Always;
		with_1.KeyActionEnter = KeyActionEnum.MoveAcrossOut;
		with_1.KeyActionTab = KeyActionEnum.MoveAcrossOut;
		with_1.HighLight = HighLightEnum.WithFocus;
		with_1.SelectionMode = SelectionModeEnum.Row;
		with_1.Styles.Highlight.BackColor = _selectionLine_BackgroundColor;
		with_1.Styles.Highlight.ForeColor = _selectionLine_MojiColor;
		with_1.AutoSearch = AutoSearchEnum.None;

		//ツリー列：0
		with_1.Tree.Column = 0;
		//ツリースタイルを設定
		with_1.Tree.Style = C1.Win.C1FlexGrid.TreeStyleFlags.SimpleLeaf;

		//No
		object with_2 = with_1.Cols(ItineraryGridColType.colLine_no);
		with_2.Name = ItineraryGridColType.colLine_no.ToString();
		with_2.Caption = getEnumAttrValue(ItineraryGridColType.colLine_no);
		with_2.AllowEditing = false;
		with_2.AllowResizing = false;
		with_2.StyleNew.BackColor = Color.LightGray;
		with_2.Width = 40;
		with_2.TextAlign = TextAlignEnum.CenterCenter;
		with_2.TextAlignFixed = TextAlignEnum.CenterCenter;

		//日
		object with_3 = with_1.Cols(ItineraryGridColType.colDaily);
		with_3.Name = ItineraryGridColType.colDaily.ToString();
		with_3.Caption = getEnumAttrValue(ItineraryGridColType.colDaily);
		with_3.AllowEditing = false;
		with_3.AllowResizing = false;
		with_3.Width = 30;
		with_3.Editor = _ucoCommon.getNumberEx(1, false, 1);
		with_3.DataType = typeof(object);
		with_3.TextAlign = TextAlignEnum.CenterCenter;
		with_3.TextAlignFixed = TextAlignEnum.CenterCenter;

		//種別
		object with_4 = with_1.Cols(ItineraryGridColType.colKind);
		with_4.Name = ItineraryGridColType.colKind.ToString();
		with_4.Caption = getEnumAttrValue(ItineraryGridColType.colKind);
		with_4.AllowEditing = false;
		siireKind = CommonProcess.getComboboxData(_tblSuppliersKind_Itinerary);
		siireKind.Add(System.Convert.ToString(SiireKindPlus.syaso), getEnumAttrValue(SiireKindPlus.syaso));
		siireKind.Add(System.Convert.ToString(SiireKindPlus.place), getEnumAttrValue(SiireKindPlus.place));
		with_4.DataMap = siireKind;
		with_4.DataType = typeof(string);
		with_4.Width = 151;
		with_4.TextAlignFixed = TextAlignEnum.CenterCenter;

		//降車ヶ所
		object with_5 = with_1.Cols(ItineraryGridColType.colKousya_place);
		with_5.Name = ItineraryGridColType.colKousya_place.ToString();
		with_5.Caption = getEnumAttrValue(ItineraryGridColType.colKousya_place);
		with_5.AllowEditing = false;
		with_5.DataType = typeof(string);
		with_5.Width = 343;
		with_5.TextAlignFixed = TextAlignEnum.CenterCenter;

		//取扱
		object with_6 = with_1.Cols(ItineraryGridColType.colToriatukai);
		with_6.Name = ItineraryGridColType.colToriatukai.ToString();
		with_6.Caption = getEnumAttrValue(ItineraryGridColType.colToriatukai);
		with_6.AllowEditing = true;
		with_6.ComboList = "...";
		with_6.Width = 40;
		with_6.TextAlignFixed = TextAlignEnum.CenterCenter;
		with_6.AllowResizing = false;


		//セルスタイル設定↓
		with_1.Styles.Add(ucoS01_0307_Common.FuzokuInfoAri_CellBackgroundColor_StyleNm);
		with_1.Styles(ucoS01_0307_Common.FuzokuInfoAri_CellBackgroundColor_StyleNm).BackColor = _ucoCommon.FuzokuInfoAri_CellBackgroundColor;

		with_1.Styles.Add(ucoS01_0307_Common.FuzokuInfoNasi_CellBackgroundColor_StyleNm);
		with_1.Styles(ucoS01_0307_Common.FuzokuInfoNasi_CellBackgroundColor_StyleNm).BackColor = _ucoCommon.FuzokuInfoNasi_CellBackgroundColor;

		//.Styles.Add(_InputRequired_StyleNm)
		//.Styles(_InputRequired_StyleNm).BackColor = FixedCd.BackgroundColorType.InputRequired

		//.Styles.Add(_Error_StyleNm)
		//.Styles(_Error_StyleNm).BackColor = FixedCd.BackgroundColorType.Error

		//.Styles.Add(_CellStyle_ComboBox)
		//.Styles(_CellStyle_ComboBox).DataMap = getPlaceMultiCombo(_tblHaisyaKeiyuti, multiComboType.sonota)
		//.Styles(_CellStyle_ComboBox).DataType = GetType(String)

		const string CellStyle_Button = "Button";
		with_1.Styles.Add(CellStyle_Button);
		with_1.Styles(CellStyle_Button).ComboList = "...";
		with_1.Styles(CellStyle_Button).DataType = typeof(string);

		//定期観光設定↓
		if (((S01_0307)ParentForm).CrsKinds.teikiKankou == true)
		{
			// コース種別１=[定期観光]
			with_1.Cols(ItineraryGridColType.colDaily).Visible = false;
		}

		// [工程]タブ [工程情報]と同期
		//.DataSource = TblItineraryGridKoumoku

		DataTable myTb = TblItineraryGridKoumoku;
		DataView myVw = new DataView(myTb);
		myVw.RowFilter = "" + ItineraryGridColType.colLine_no.ToString() + " <> '' AND " + ItineraryGridColType.colKind.ToString() + " <> '" + SiireKindPlus.syaso + "' AND " + ItineraryGridColType.colKind.ToString() + " <> '" + SiireKindPlus.place + "'";
		with_1.DataSource = myVw;


	}

	/// <summary>
	/// 取扱情報の設定有無をチェックし、背景色を設定
	/// </summary>
	public void setGridInitialize_grdCellBackColor()
	{

		//※ [取扱]タブ から参照するため Public としている

		if (ReferenceEquals(this.grdItineraryInfo.DataSource, null))
		{
			return;
		}

		for (int cnt = 1; cnt <= this.grdItineraryInfo.Rows.Count - 1; cnt++)
		{
			this.checkItineraryInfoUmu(cnt, (int)ItineraryGridColType.colToriatukai);
		}

	}

	/// <summary>
	/// 取扱情報の設定有無をチェック
	/// </summary>
	/// <param name="row"></param>
	/// <param name="col"></param>
	private void checkItineraryInfoUmu(int row, int col)
	{

		string _kind = System.Convert.ToString(_ucoCommon.nnvl_str(this.grdItineraryInfo.Rows(row).Item(ItineraryGridColType.colKind.ToString())));
		string _suppliersName = System.Convert.ToString(_ucoCommon.nnvl_str(this.grdItineraryInfo.Rows(row).Item(ItineraryGridColType.colKousya_place.ToString())));

		// フィルタしているため、実際のラインNoを取得
		int lineNo = 0;
		lineNo = System.Convert.ToInt32(_ucoCommon.nnvl_int(grdItineraryInfo.Rows(row).Item(ItineraryGridColType.colLine_no.ToString())));

		if ((_kind == string.Empty) ||)
		{
			(_suppliersName == string.Empty) ||;
			(_kind == System.Convert.ToString(SiireKindPlus.syaso)) ||;
			(_kind == System.Convert.ToString(SiireKindPlus.place)) ||;
			isItinerary_ToriatukaiInfoCheck(lineNo - 1) = System.Convert.ToBoolean(true);
			this.grdItineraryInfo.SetCellStyle(row, col, ucoS01_0307_Common.FuzokuInfoAri_CellBackgroundColor_StyleNm);
		}
		else
		{
			this.grdItineraryInfo.SetCellStyle(row, col, ucoS01_0307_Common.FuzokuInfoNasi_CellBackgroundColor_StyleNm);
		}

	}

	/// <summary>
	/// 行程情報の取扱情報設定有無
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool isItinerary_ToriatukaiInfoCheck(int koshakashoIndex)
	{

		bool returnValue = false;

		//'With 行程_降車ヶ所エンティティ.EntityData(降車ヶ所Index)
		//'    If .取扱情報エンティティ.EntityData.Length = 1 AndAlso _
		//'       .取扱情報エンティティ.EntityData(0).定期_企画区分.値 = String.Empty Then
		//'        '取扱設定されてません↓
		//'        returnValue = False
		//'    Else
		//'        '取扱設定されてます↓
		//'        returnValue = True
		//'    End If
		//'End With

		object with_1 = Itinerary_KoshakashoEntity.EntityData(koshakashoIndex);
		if (with_1.ToriatukaiInfoEntity.EntityData(0).Teiki_KikakuKbn.Value == string.Empty)
		{
			//取扱設定されてません↓
			returnValue = false;
		}
		else
		{
			//取扱設定されてます↓
			returnValue = true;
		}

		return returnValue;
	}

	public void checkItineraryInfoUmuBind(int row)
	{

		if (ReferenceEquals(this.grdItineraryInfo, null))
		{
			return;
		}

		this.checkItineraryInfoUmu(row, (int)ItineraryGridColType.colToriatukai);

	}

	/// <summary>
	/// 取扱設定画面呼び出し
	/// </summary>
	/// <remarks></remarks>
	private void showToriatukaiSet(string _suppliersName, int _lineNo, KoGamenCallMotoType _type)
	{

		S01_0319 _frmToriatukaiSet = null;
		ItineraryInfoKeyKoumoku _itineraryKey = null;
		string _suppliersCd = string.Empty;
		string _suppliersEdaban = string.Empty;


		try
		{

			if (string.IsNullOrEmpty(System.Convert.ToString(Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).Koshakasho.Value)))
			{
				return;
			}

			_frmToriatukaiSet = new S01_0319();
			_frmToriatukaiSet.ToriatukaiInfoEntityBase = Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).ToriatukaiInfoEntity;

			_itineraryKey.ItineraryKind = (ItineraryKindType)(Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).ItineraryKind.Value);
			_suppliersCd = System.Convert.ToString(Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).Koshakasho.Value);
			_suppliersEdaban = System.Convert.ToString(Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).KoshakashoEdaban.Value);

			//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↓
			_frmToriatukaiSet.KoshakashoEntityBase = Itinerary_KoshakashoEntity.EntityData(_lineNo - 1);


			_frmToriatukaiSet.SuppliersName = _suppliersName;
			_itineraryKey.CrsMasterKeyKoumoku = ((S01_0307)ParentForm).TaisyoCrsInfo(0);
			_itineraryKey.LineNo = _lineNo;
			_frmToriatukaiSet.ItineraryInfo = _itineraryKey;
			_frmToriatukaiSet.ProcessMode = ((S01_0307)ParentForm).ProcessMode;
			_frmToriatukaiSet.SuppliersCd = _suppliersCd;
			_frmToriatukaiSet.SuppliersEdaban = _suppliersEdaban;
			_frmToriatukaiSet.ShowDialog(this);

			if (_frmToriatukaiSet.KakuteiFlg == true)
			{
				Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).ToriatukaiInfoEntity.clear();
				for (int index = 0; index <= _frmToriatukaiSet.ToriatukaiInfoEntityBase.EntityData.Length - 1; index++)
				{
					Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).ToriatukaiInfoEntity.add(_frmToriatukaiSet.ToriatukaiInfoEntityBase.EntityData(index));
				}
				Itinerary_KoshakashoEntity.EntityData(_lineNo - 1).ToriatukaiInfoEntity.remove(0);
				//行程_降車ヶ所エンティティ.EntityData(_gridRow - 1).取扱情報エンティティ.copy_Backupから前回値()
				//行程_降車ヶ所エンティティ.EntityData(_gridRow - 1).取扱情報エンティティ.EntityData(0).取扱詳細エンティティ.copy_Backupから前回値()

			}

		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <returns></returns>
	public string checkInputNaiyo()
	{

		string errMsg = string.Empty;

		// 注釈
		object with_1 = this.txtNote;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_1.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_1.ExistError = true;
			with_1.Focus();
			return errMsg;
		}

		// 通行料
		object with_2 = this.txtTuukoryo;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_2.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_2.ExistError = true;
			with_2.Focus();
			return errMsg;
		}

		// 積込品
		object with_3 = this.txtTumikomihin;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_3.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_3.ExistError = true;
			with_3.Focus();
			return errMsg;
		}

		// 乗務員食事
		object with_4 = this.txtJomuinMeal;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_4.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_4.ExistError = true;
			with_4.Focus();
			return errMsg;
		}

		// メモ
		object with_5 = this.txtMemo_Toriatukai;
		errMsg = System.Convert.ToString(CommonProcess.unicodeCheck(with_5.Text));
		if (!errMsg.Equals(string.Empty))
		{
			with_5.ExistError = true;
			with_5.Focus();
			return errMsg;
		}

		return errMsg;
	}

	/// <summary>
	/// エンティティに格納されているデータを画面に反映する
	/// </summary>
	/// <param name="crsMasterEntity"></param>
	public void setEntityDataToScreen(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity with_1 = crsMasterEntity;

		//注釈
		this.txtNote.Text = with_1.Note.Value;

		//通行料
		this.txtTuukoryo.Text = with_1.Tuukoryo.Value;

		//積込品
		this.txtTumikomihin.Text = with_1.Tumikomihin.Value;

		//乗務員食事
		this.txtJomuinMeal.Text = with_1.JomuinMeal.Value;

		//メモ
		this.txtMemo_Toriatukai.Text = with_1.ToriatukaiMemo.Value;

		//確認済みフラグ
		if (with_1.ToriatukaiInfoAddFlg.Value == 1)
		{
			this.chkKakuninAlready_Toriatukai.Checked = true;
		}

		//確認日
		if (ReferenceEquals(with_1.ToriatukaiInfoAddYmd.Value, null) == false)
		{
			this.txtKakuninAlreadyYmd_Toriatukai.Value = with_1.ToriatukaiInfoAddYmd.Value;
		}


		//工程情報とリンクするため不要
		//行程（取扱）
		//Call Me.setEntityDataToScreenItineraryInfo(crsMasterEntity.KoshakashoEntity)

	}

	#region 工程情報とリンクするため不要
	///' <summary>
	///' 行程情報を画面に反映する
	///' </summary>
	///' <param name="koshakashoEntity">降車ヶ所エンティティ</param>
	//Private Sub setEntityDataToScreenItineraryInfo(ByVal koshakashoEntity As EntityOperation(Of KoshakashoEntity))

	//    Me.grdItineraryInfo.Rows.Count = 1
	//    Me.grdItineraryInfo.Rows.Count = 1 + koshakashoEntity.EntityData.Length
	//    Me.grdItineraryInfo.Refresh()


	//    With Me.grdItineraryInfo

	//        For row As Integer = 0 To koshakashoEntity.EntityData.Length - 1

	//            If koshakashoEntity.EntityData(row).Daily.Value Is Nothing Then
	//                Continue For
	//            End If

	//            ' No
	//            .SetData(row + 1, ItineraryGridColType.No, (row + 1).ToString())

	//            ' 日
	//            .SetData(row + 1, ItineraryGridColType.day, koshakashoEntity.EntityData(row).Daily.Value)

	//            ' 種別
	//            'TODO:naga 仮
	//            If IsNothing(koshakashoEntity.EntityData(row).Kind.Value) = False Then
	//                .SetData(row + 1, ItineraryGridColType.kind, koshakashoEntity.EntityData(row).Kind.Value)
	//            End If

	//            ' 名称
	//            'TODO:naga 仮
	//            .SetData(row + 1, ItineraryGridColType.koshakasho, koshakashoEntity.EntityData(row).Koshakasho)

	//            ' 取扱[ボタン]
	//            .SetData(row + 1, ItineraryGridColType.toriatukai, String.Empty)

	//        Next

	//    End With

	//End Sub
	#endregion

	/// <summary>
	/// 画面の内容をエンティティに格納する
	/// </summary>
	/// <param name="crsMasterEntity">コースマスタエンティティ</param>
	/// <returns>コースマスタエンティティ</returns>
	public CrsMasterEntity setScreenDataToEntity(CrsMasterEntity crsMasterEntity)
	{

		CrsMasterEntity retCrsMasterEntity = crsMasterEntity;


		//注釈
		retCrsMasterEntity.Note.Value = this.txtNote.Text;
		//通行料
		retCrsMasterEntity.Tuukoryo.Value = this.txtTuukoryo.Text;
		//積込品
		retCrsMasterEntity.Tumikomihin.Value = this.txtTumikomihin.Text;
		//乗務員食事
		retCrsMasterEntity.JomuinMeal.Value = this.txtJomuinMeal.Text;
		//メモ
		retCrsMasterEntity.ToriatukaiMemo.Value = this.txtMemo_Toriatukai.Text;

		//確認済みフラグ、確認日
		retCrsMasterEntity.ToriatukaiInfoAddFlg.Value = _ucoCommon.getItemValueForCheckBox(this.chkKakuninAlready_Toriatukai);
		if (ReferenceEquals(this.txtKakuninAlreadyYmd_Toriatukai.Value, null) == true)
		{
			retCrsMasterEntity.ToriatukaiInfoAddYmd.Value = null;
		}
		else
		{
			retCrsMasterEntity.ToriatukaiInfoAddYmd.Value = System.Convert.ToDateTime(this.txtKakuninAlreadyYmd_Toriatukai.Value);
		}


		return retCrsMasterEntity;
	}

	private void grdItineraryInfo_BeforeEdit(object sender, RowColEventArgs e)
	{

		// デザインモード時は処理を抜ける
		if (this.DesignMode == true)
		{
			return;
		}

		FlexGridEx grid = (FlexGridEx)sender;
		string _kind = System.Convert.ToString(_ucoCommon.nnvl_str(grid.Rows(e.Row).Item(ItineraryGridColType.colKind.ToString())));
		string _suppliersName = System.Convert.ToString(_ucoCommon.nnvl_str(grid.Rows(e.Row).Item(ItineraryGridColType.colKousya_place.ToString())));

		if (grid.Cols(e.Col).Name == ItineraryGridColType.colToriatukai.ToString())
		{
			// 取扱
			if (_kind == string.Empty || _suppliersName == string.Empty)
			{
				e.Cancel = true;
			}
		}
	}

	#endregion

}