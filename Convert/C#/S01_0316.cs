using C1.Win.C1FlexGrid;
using GrapeCity.Win.Editors;


/// <summary>
/// 原価適用期間設定
/// </summary>
/// <remarks></remarks>
public class S01_0315 : FormBase
{
	public S01_0315()
	{
		crsMasterEntity = new EntityOperation[Of CrsMasterEntity + 1];
		_selectionJiFixedLine_BackgroundColor = FixedCd.BackgroundColorType.SelectionJiFixedLine_BackgroundColor;
		_fixedLine_BackgroundColor = Color.LightGray;
		_selectionLine_BackgroundColor = FixedCd.BackgroundColorType.SelectionLine_BackgroundColor;
		_selectionLine_MojiColor = SystemColors.HighlightText;
		koshakashoEntity = new EntityOperation(Of KoshakashoEntity);
		itinerary_KoshakashoEntity = new EntityOperation(Of KoshakashoEntity);
		sonotaItinerary_KoshakashoEntity = new EntityOperation(Of KoshakashoEntity);
		crsMasterEntity_CostSetting = new EntityOperation(Of CrsMasterEntity);

	}

	#region  プロパティ
	/// <summary>
	/// コース情報
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public CrsMasterKeyKoumoku TaisyoCrsInfo //対象コース情報()
	{
		set
		{
			_taisyoCrsInfo = value;
		}
	}

	/// <summary>
	/// 処理モード
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public ProcessMode ProcessMode //処理モード()
	{
		set
		{
			_processMode = value;
		}
	}

	/// <summary>
	/// 確定フラグ
	/// ADD-20120801-ご旅行案内機能追加
	/// </summary>
	/// <returns>True:確定／False：未確定</returns>
	/// <remarks></remarks>
	public bool KakuteiFlg //確定フラグ()
	{
		get
		{
			return _kakuteiFlg;
		}
	}
	#endregion

	#region  定数/変数
	private CrsMasterKeyKoumoku _taisyoCrsInfo;
	private ProcessMode _processMode;
	private bool _kakuteiFlg = false;
	private Teiki_KikakuKbnType _teiki_Kikaku;
	private CrsKind1Type _crsKind1;
	private CrsKind2 _crsKind2;
	private SyuptJiCarrierKbnType _syuptJiCarrier;
	private const string _YmdLiteral = "____/__/__";
	public EntityOperation[] crsMasterEntity;
	#endregion
	#region  色
	private Color _selectionJiFixedLine_BackgroundColor;
	private Color _fixedLine_BackgroundColor;
	private Color _selectionLine_BackgroundColor;
	private Color _selectionLine_MojiColor;
	#endregion

	#region  エンティティ変数
	private EntityOperation koshakashoEntity;
	private EntityOperation itinerary_KoshakashoEntity;
	private EntityOperation sonotaItinerary_KoshakashoEntity;
	private EntityOperation crsMasterEntity_CostSetting;
	#endregion

	#region  行関連変数
	private const int grdCostSelectionMaxRow = 5;
	#endregion

	#region  中間テーブル変数
	private DataTable tblCostSelection = new DataTable();
	#endregion

	#region Enum定義
	/// <summary>
	/// 原価選択グリッドカラム
	/// </summary>
	/// <remarks></remarks>
	private enum CostSelectionGridColType : int //原価選択GridColType
	{
		[Value("No")]
		No = 0, //No
		[Value("適用開始日")]
		applicationStartDay, //適用開始日
		[Value("削除日")]
		deleteDay, //削除日
		[Value("更新日")]
		updateDay, //更新日
		[Value("更新者")]
		updatePerson, //更新者
		[Value("列合計")]
		colTotal //列合計
	}
	#endregion

	/// <summary>
	/// ロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmItineraryInfo_Load(object sender, System.EventArgs e)
	{

		try
		{
			this.Cursor = Cursors.WaitCursor;

			InitializeSettingItem();

			//エンティティ初期化
			InitializeEntity();

			//グリッド列生成
			setGridSettingCostSelection();

			//データテーブル作成
			//列生成
			setTableColCostSelection();
			//データセット
			setTblCostSelection();

			//グリッドデータセット
			setGridDataCostSelection();

			//' 処理モードによるコントロールの設定
			//TODO:共通対応待
			//Call MyBase.setProcessMode(Me._processMode)
			if (this._processMode == FixedCd.ProcessMode.reference)
			{
				setProcessMode(this._processMode, this);
				this.setButtonEnabled(this);
			}

		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
			//_kakuteiFlg = True
			this.Close();
		}
		finally
		{
			this.Cursor = Cursors.Default;
		}

	}

	#region Private変数設定
	/// <summary>
	/// 行程情報設定項目の初期値を設定
	/// </summary>
	/// <remarks></remarks>
	private void InitializeSettingItem()
	{
		if (_taisyoCrsInfo.Teiki_KikakuKbn == string.Empty)
		{
			this._teiki_Kikaku = Teiki_KikakuKbnType.teikiKanko;
		}
		else
		{
			this._teiki_Kikaku = (Teiki_KikakuKbnType)_taisyoCrsInfo.Teiki_KikakuKbn;
		}

		if (crsMasterEntity.EntityData(0).CrsKind1.Value == string.Empty)
		{
			this._crsKind1 = CrsKind1Type.teikiKanko;
		}
		else
		{
			this._crsKind1 = (CrsKind1Type)(crsMasterEntity.EntityData(0).CrsKind1.Value);
		}

		if (crsMasterEntity.EntityData(0).CrsKind2.Value == string.Empty)
		{
			this._crsKind2 = CrsKind2.higaeri;
		}
		else
		{
			this._crsKind2 = (CrsKind2)(crsMasterEntity.EntityData(0).CrsKind2.Value);
		}

		if (crsMasterEntity.EntityData(0).SyuptJiCarrierKbn.Value == string.Empty)
		{
			this._syuptJiCarrier = SyuptJiCarrierKbnType.bus;
		}
		else
		{
			this._syuptJiCarrier = (SyuptJiCarrierKbnType)(crsMasterEntity.EntityData(0).SyuptJiCarrierKbn.Value);
		}

	}
	#endregion

	#region エンティティ初期化
	/// <summary>
	/// エンティティの初期化を行います。
	/// </summary>
	/// <remarks></remarks>
	private void InitializeEntity()
	{
		//降車ヶ所エンティティ
		koshakashoEntity.clear();
		koshakashoEntity = crsMasterEntity.EntityData(0).KoshakashoEntity.deepCopy;
		koshakashoEntity.copy_ValueFromZenkaiValue();
		for (int KoshakashoIndex = 0; KoshakashoIndex <= koshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			object with_1 = koshakashoEntity.EntityData(KoshakashoIndex);
			with_1.SiireInfoEntity.copy_ValueFromZenkaiValue();
			with_1.StayDetailEntity.copy_ValueFromZenkaiValue();
			with_1.MealDetailEntity.copy_ValueFromZenkaiValue();
			with_1.CostMaster_CarrierEntity.copy_ValueFromZenkaiValue();
			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_1.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				with_1.CostMaster_CarrierEntity(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.copy_ValueFromZenkaiValue();
			}
			with_1.CostMaster_KoshakashoEntity.copy_ValueFromZenkaiValue();
			for (int CostKoshaIndex = 0; CostKoshaIndex <= with_1.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshaIndex++)
			{
				with_1.CostMaster_KoshakashoEntity(CostKoshaIndex).CostMasterKoshakashoChargeKbnEntity.copy_ValueFromZenkaiValue();
			}
			with_1.ToriatukaiInfoEntity.copy_ValueFromZenkaiValue();
			for (int ToriatukaiIndex = 0; ToriatukaiIndex <= with_1.ToriatukaiInfoEntity.EntityData.Length - 1; ToriatukaiIndex++)
			{
				with_1.ToriatukaiInfoEntity(ToriatukaiIndex).ToriatukaiDetailEntity.copy_ValueFromZenkaiValue();
			}
		}
		InitializeItineraryEntity();
		//コースマスタエンティティ
		crsMasterEntity_CostSetting = crsMasterEntity.deepCopy;
		crsMasterEntity_CostSetting.copy_ValueFromZenkaiValue();
		//コースマスタエンティティ(キャリア)
		crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity.copy_ValueFromZenkaiValue();
		for (int CostCarrierIndex = 0; CostCarrierIndex <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
		{
			crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.copy_ValueFromZenkaiValue();
		}
		//コースマスタエンティティ(適用期間)
		crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.copy_ValueFromZenkaiValue();
		//コースマスタエンティティ(プレート)
		object with_2 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		for (int PlateIndex = 0; PlateIndex <= with_2.EntityData.Length - 1; PlateIndex++)
		{
			with_2.EntityData(PlateIndex).CostMaster_PlateEntity.copy_ValueFromZenkaiValue();
		}
	}

	/// <summary>
	/// 降車ヶ所エンティティから、「行程」「その他」に振り分ける
	/// </summary>
	/// <remarks></remarks>
	private void InitializeItineraryEntity() //Initialize行程Entity()
	{
		DataTable _tblInitialSuppliers = new DataTable(); //_tbl初期仕入先
		SuppliersMasterEntity _suppliersMasterE = new SuppliersMasterEntity(); //_仕入先マスタE

		itinerary_KoshakashoEntity.clear();
		sonotaItinerary_KoshakashoEntity.clear();

		for (int index = 0; index <= koshakashoEntity.EntityData.Length - 1; index++)
		{
			if (koshakashoEntity.EntityData(index).ItineraryKind.Value == System.Convert.ToString(ItineraryKindType.itinerary))
			{

				if (itinerary_KoshakashoEntity.EntityData(0).Teiki_KikakuKbn.Value == string.Empty &&)
				{
					itinerary_KoshakashoEntity.EntityData(0).Year.Value = string.Empty &&;
					itinerary_KoshakashoEntity.EntityData.Length = 1;
					itinerary_KoshakashoEntity.EntityData[0] = koshakashoEntity.EntityData[index];
				}
				else
				{
					itinerary_KoshakashoEntity.add(koshakashoEntity.EntityData(index));
				}
			}
			else
			{
				if (sonotaItinerary_KoshakashoEntity.EntityData(0).Teiki_KikakuKbn.Value == string.Empty &&)
				{
					sonotaItinerary_KoshakashoEntity.EntityData(0).Year.Value = string.Empty &&;
					sonotaItinerary_KoshakashoEntity.EntityData.Length = 1;
					sonotaItinerary_KoshakashoEntity.EntityData[0] = koshakashoEntity.EntityData[index];
				}
				else
				{
					sonotaItinerary_KoshakashoEntity.add(koshakashoEntity.EntityData(index));
				}
			}
		}
	}
	#endregion

	#region  グリッド設定
	#region 列設定
	/// <summary>
	/// 原価選択グリッド設定
	/// </summary>
	/// <remarks></remarks>
	private void setGridSettingCostSelection()
	{
		object with_1 = this.grdCostSelection;
		with_1.Clear(ClearFlags.All);
		with_1.Rows.Count = 1;
		with_1.Rows.Fixed = 1;
		with_1.Cols.Count = CostSelectionGridColType.colTotal;
		with_1.AllowSorting = AllowSortingEnum.None;
		with_1.AllowDragging = AllowDraggingEnum.None;
		with_1.ShowButtons = ShowButtonsEnum.Always;
		with_1.KeyActionEnter = KeyActionEnum.MoveAcross;
		with_1.KeyActionTab = KeyActionEnum.MoveAcrossOut;
		with_1.AllowEditing = false;
		with_1.SelectionMode = SelectionModeEnum.Row;
		with_1.AutoGenerateColumns = false;
		with_1.HighLight = HighLightEnum.Always;
		with_1.Styles.Highlight.BackColor = _selectionLine_BackgroundColor;
		with_1.Styles.Highlight.ForeColor = _selectionLine_MojiColor;
		with_1.HighLight = HighLightEnum.WithFocus;

		object with_2 = with_1.Cols(CostSelectionGridColType.No);
		with_2.Name = CostSelectionGridColType.No.ToString();
		with_2.Caption = getEnumAttrValue(CostSelectionGridColType.No);
		with_2.AllowResizing = false;
		with_2.DataType = typeof(int);
		with_2.TextAlign = TextAlignEnum.CenterCenter;
		with_2.TextAlignFixed = TextAlignEnum.CenterCenter;
		with_2.Style.BackColor = Color.LightGray;
		with_2.Width = 40;

		object with_3 = with_1.Cols(CostSelectionGridColType.applicationStartDay);
		with_3.Name = getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName;
		with_3.Caption = getEnumAttrValue(CostSelectionGridColType.applicationStartDay);
		with_3.Editor = getYmdCell();
		with_3.Width = 113;
		with_3.TextAlign = TextAlignEnum.CenterCenter;
		with_3.TextAlignFixed = TextAlignEnum.CenterCenter;

		object with_4 = with_1.Cols(CostSelectionGridColType.deleteDay);
		with_4.Name = getCostHeaderKoumokuType.DeleteDay.PhysicsName;
		with_4.Caption = getEnumAttrValue(CostSelectionGridColType.deleteDay);
		with_4.Editor = getYmdCell();
		with_4.Width = 110;
		with_4.TextAlign = TextAlignEnum.CenterCenter;
		with_4.TextAlignFixed = TextAlignEnum.CenterCenter;

		object with_5 = with_1.Cols(CostSelectionGridColType.updateDay);
		with_5.Name = getCostHeaderKoumokuType.UpdateDate.PhysicsName;
		with_5.Caption = getEnumAttrValue(CostSelectionGridColType.updateDay);
		with_5.Editor = getYmdCell();
		with_5.Width = 110;
		with_5.TextAlign = TextAlignEnum.CenterCenter;
		with_5.TextAlignFixed = TextAlignEnum.CenterCenter;

		object with_6 = with_1.Cols(CostSelectionGridColType.updatePerson);
		with_6.Name = getCostHeaderKoumokuType.UpdateUserID.PhysicsName;
		with_6.Caption = getEnumAttrValue(CostSelectionGridColType.updatePerson);
		with_6.Width = 110;
		with_6.TextAlign = TextAlignEnum.LeftCenter;
		with_6.TextAlignFixed = TextAlignEnum.CenterCenter;

	}
	#endregion
	#region データ設定
	/// <summary>
	/// 原価選択グリッドにデータを設定します。
	/// </summary>
	/// <remarks></remarks>
	private void setGridDataCostSelection()
	{
		this.grdCostSelection.DataSource = tblCostSelection;
		this.grdCostSelection.Refresh();
	}
	#endregion
	#endregion

	#region  データテーブル設定
	#region 列設定
	/// <summary>
	/// データテーブルに原価マスタ_ヘッダエンティティのカラムを設定する。
	/// </summary>
	/// <remarks></remarks>
	private void setTableColCostSelection()
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		tblCostSelection.Columns.Add(CostSelectionGridColType.No.ToString());
		for (int HeaderIndex = 0; HeaderIndex <= with_1.getPropertyDataLength - 1; HeaderIndex++)
		{
			tblCostSelection.Columns.Add(get_getCostHeaderKoumokuType(HeaderIndex, 0).PhysicsName);
		}
	}
	#endregion

	#region データ設定
	/// <summary>
	/// データテーブルに原価マスタ_ヘッダエンティティのデータを設定する。
	/// </summary>
	/// <remarks></remarks>
	private void setTblCostSelection()
	{
		tblCostSelection.Clear();

		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		for (int HeaderIndex = 0; HeaderIndex <= with_1.EntityData.Length - 1; HeaderIndex++)
		{
			tblCostSelection.Rows.Add(1);
			tblCostSelection.Rows(HeaderIndex).Item[CostSelectionGridColType.No.ToString()] = HeaderIndex + 1;
			for (int HeaderptyIndex = 0; HeaderptyIndex <= with_1.getPropertyDataLength - 1; HeaderptyIndex++)
			{
				if (get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName == with_1.EntityData(0).ApplicationStartDay.PhysicsName)
				{
					tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
						= ;
					convertDate(get_getCostHeaderMojiType(HeaderptyIndex, HeaderIndex).Value);

				}
				else if (get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName == with_1.EntityData(0).DeleteDay.PhysicsName)
				{
					tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
						= ;
					convertDate(get_getCostHeaderMojiType(HeaderptyIndex, HeaderIndex).Value);

				}
				else if (get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName == with_1.EntityData(0).UpdateDate.PhysicsName)
				{
					tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
						= ;
					convertYmd(get_getCostHeaderYmdType(HeaderptyIndex, HeaderIndex).Value);

				}
				else
				{
					if (ReferenceEquals(with_1.getPtyType(HeaderptyIndex, with_1.EntityData(0)), typeof(EntityKoumoku_YmdType)))
					{
						tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
							= ;
						nnvl(get_getCostHeaderYmdType(HeaderptyIndex, HeaderIndex).Value, null);
					}
					else if (ReferenceEquals(with_1.getPtyType(HeaderptyIndex, with_1.EntityData(0)), typeof(EntityKoumoku_NumberType)))
					{
						tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
							= ;
						nnvl(get_getCostHeaderNumberType(HeaderptyIndex, HeaderIndex).Value, null);
					}
					else
					{
						tblCostSelection.Rows(HeaderIndex).Item[get_getCostHeaderKoumokuType(HeaderptyIndex, HeaderIndex).PhysicsName]
							= ;
						nnvl(get_getCostHeaderMojiType(HeaderptyIndex, HeaderIndex).Value, string.Empty);
					}
				}
			}
		}
	}
	#endregion
	#endregion

	#region  参照関連
	/// <summary>
	///　ボタンの使用不可設定
	/// </summary>
	/// <param name="targetCtr"></param>
	/// <remarks></remarks>
	private void setButtonEnabled(Control targetCtr)
	{

		foreach (Control ctr in targetCtr.Controls)
		{
			if (ReferenceEquals(ctr.GetType, typeof(ButtonEx)))
			{
				if (((ButtonEx)ctr).Name != "btnF2")
				{
					((ButtonEx)ctr).Enabled = false;
				}
			}
			else if (ReferenceEquals(ctr.GetType, typeof(Panel)))
			{
				this.setButtonEnabled(ctr);
			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBox)))
			{
				this.setButtonEnabled(ctr);
			}
		}
	}

	#endregion

	#region イベント
	#region Grid
	/// <summary>
	/// 選択行が変更されたときの処理
	/// 選択されている行の「No」の背景色を変更
	/// ※フォーカスが離れた時、どの行が選択されているか明示する。
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdCostSelection_RowColChange(object sender, System.EventArgs e) //grd原価選択_RowColChange
	{
		CellStyle _cellstyleNormalLine = this.grdCostSelection.Styles.Add("通常行"); //_cellstyle通常行
		CellStyle _cellStyleSelectionLine = this.grdCostSelection.Styles.Add("選択行"); //_cellStyle選択行

		object with_1 = this.grdCostSelection;
		if (this.grdCostSelection.Row > 0)
		{
			if (ReferenceEquals(nnvl(with_1.Rows(with_1.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName), string.Empty), string.Empty))
			{
				this.txtApplicationStartDay.Value = null;
			}
			else
			{
				this.txtApplicationStartDay.Value = nnvl_date(with_1.Rows(with_1.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName));
			}
			if (ReferenceEquals(nnvl(with_1.Rows(with_1.Row).Item(getCostHeaderKoumokuType.DeleteDay.PhysicsName), string.Empty), string.Empty))
			{
				this.chkCostDelete.Checked = false;
			}
			else
			{
				this.chkCostDelete.Checked = true;
			}

			_cellstyleNormalLine.BackColor = _fixedLine_BackgroundColor;
			for (int gridRow = 1; gridRow <= with_1.Rows.Count - 1; gridRow++)
			{
				with_1.SetCellStyle(gridRow, CostSelectionGridColType.No, _cellstyleNormalLine);
			}

			_cellStyleSelectionLine.BackColor = _selectionJiFixedLine_BackgroundColor;
			with_1.SetCellStyle(with_1.Row, CostSelectionGridColType.No, _cellStyleSelectionLine);
		}
	}

	#endregion

	#region Button
	/// <summary>
	/// [F2:戻る]ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();
	}

	/// <summary>
	/// [F10:確定]ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		try
		{
			//カーソルを処理中に変更
			this.Cursor = Cursors.WaitCursor;

			//UNICODE変換可能チェック
			//TODO:共通フォームの対応待ち(谷岡コメント化)　Me->Mybaseにも変更
			//MyBase.clearErrorUmu()
			//If MyBase.Check_AllCode() = False Then
			//    Exit Sub
			//End If

			//'ベースのエンティティにコピーを行う(行程側で、最後の「Backupから前回値」を実行して終了）↓
			returnKoshakashoEntity();

			//ベースのエンティティにコピーを行う
			returnCrsMasterEntity();

			this._kakuteiFlg = true;
			this.Close();

		}
		catch (Exception ex)
		{
			//メッセージ出力
			//Call メッセージ出力.messageDisp("9001")
			//MyBase.outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.その他, Me.setTitle & "_確定", ex.Message)
			createFactoryMsg.messageDisp("9001");
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, this.setTitle + "_確定", ex.Message);
		}
		finally
		{
			//カーソルを戻す
			this.Cursor = Cursors.Default;
		}

	}

	///  <summary>
	/// 複写ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void btnCostSelection_Copy_Click(System.Object sender, System.EventArgs e)
	{
		int _focusRow = 0;

		if (this.grdCostSelection.Row > 0)
		{
			if (ReferenceEquals(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)), null) ||)
			{
				this.txtApplicationStartDay.Text = _YmdLiteral;
				createFactoryMsg.messageDisp("0007");

			}
			else if (checkApplicationStartDayExistenceUmu(System.Convert.ToString(this.txtApplicationStartDay.Text)) == false)
			{
				createFactoryMsg.messageDisp("0008", "適用開始日");

			}
			else if (this.grdCostSelection.Rows.Count > grdCostSelectionMaxRow)
			{
				createFactoryMsg.messageDisp("0044", "適用開始日");

			}
			else
			{
				copyApplicableApplicationCostHeaderRecord(System.Convert.ToString(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)).Replace("/", "")));
				copyApplicableApplicationCostKoshakashoRecord(System.Convert.ToString(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)).Replace("/", "")));
				_focusRow = System.Convert.ToInt32(this.grdCostSelection.Row);
				setTblCostSelection();
				setGridDataCostSelection();
				this.grdCostSelection.Select(_focusRow, CostSelectionGridColType.applicationStartDay, true);
			}
		}
	}

	/// <summary>
	/// 変更ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void btnCostSelection_Change_Click(System.Object sender, System.EventArgs e)
	{
		int _focusRow = 0;

		if (this.grdCostSelection.Row > 0)
		{
			if (ReferenceEquals(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)), null) ||)
			{
				this.txtApplicationStartDay.Text = _YmdLiteral;
				createFactoryMsg.messageDisp("0007");

			}
			else if (checkApplicationStartDayExistenceUmu(System.Convert.ToString(this.txtApplicationStartDay.Text)) == false &&)
			{
				nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)) != this.txtApplicationStartDay.Text;
				createFactoryMsg.messageDisp("0008", "適用開始日");

			}
			else if (this.chkCostDelete.Checked == true && checkDeleteDayUmu() == false)
			{
				createFactoryMsg.messageDisp("0057");

			}
			else
			{
				changeApplicableApplicationCostHeaderRecord(System.Convert.ToString(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)).Replace("/", "")));
				changeApplicableApplicationCostKoshakashoRecord(System.Convert.ToString(nnvl_str(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)).Replace("/", "")));
				_focusRow = System.Convert.ToInt32(this.grdCostSelection.Row);
				setTblCostSelection();
				setGridDataCostSelection();
				this.grdCostSelection.Select(_focusRow, CostSelectionGridColType.applicationStartDay, true);
			}
		}
	}
	#endregion

	#region 適用開始日
	/// <summary>
	/// 適用開始日からフォーカスが抜ける時の検証イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void txtApplicationStartDay_Validated(System.Object sender, System.EventArgs e) //txt適用開始日_Validated
	{
		if (this.txtApplicationStartDay.Text == _YmdLiteral)
		{
			this.txtApplicationStartDay.Value = nnvl_date(this.grdCostSelection.Rows(this.grdCostSelection.Row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName));
		}
	}
	#endregion

	#endregion










































	#region 必要関数
	/// <summary>
	///
	/// </summary>
	/// <param name="_costHeaderptyIndex"></param>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	private EntityKoumoku_NumberType get_getCostHeaderNumberType(int _costHeaderptyIndex, int _entityIndex)
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		return ((EntityKoumoku_NumberType)(with_1.getPtyValue(_costHeaderptyIndex, with_1.EntityData(_entityIndex))));
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="_costHeaderptyIndex"></param>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	private IEntityKoumokuType get_getCostHeaderKoumokuType(int _costHeaderptyIndex, int _entityIndex)
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		return ((IEntityKoumokuType)(with_1.getPtyValue(_costHeaderptyIndex, with_1.EntityData(_entityIndex))));
	}

	/// <summary>
	/// 主に物理名を取得
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	private CostMaster_HeaderEntity getCostHeaderKoumokuType
	{
		get
		{
			return crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData(0);
		}
	}

	#region 値変換(Null系)
	/// <summary>
	/// DBNullかチェックし、DBNullだったらdefaultValueの値を返す
	/// </summary>
	/// <param name="pObj"></param>
	/// <param name="defaultValue">pObjがDBNullだった場合に返す値</param>
	/// <returns></returns>
	/// <remarks></remarks>
	private object nnvl(object pObj, object defaultValue)
	{
		object returnValue = defaultValue;
		if (!Information.IsDBNull(pObj) && !ReferenceEquals(pObj, null) && !(System.Convert.ToString(pObj).Trim() == string.Empty))
		{
			return pObj;
		}
		return returnValue;
	}

	/// <summary>
	/// 定義値を返す
	/// </summary>
	/// <param name="pObj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private string nnvl_str(object pObj)
	{
		string returnValue = string.Empty;
		if (!Information.IsDBNull(pObj) && !ReferenceEquals(pObj, null) && !(System.Convert.ToString(pObj).Trim() == string.Empty))
		{
			return System.Convert.ToString(pObj);
		}
		else
		{
			return returnValue;
		}
	}

	/// <summary>
	/// 定義値を返す
	/// </summary>
	/// <param name="pObj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private Nullable[] nnvl_date(object pObj)
	{
		Nullable[] returnValue = null;
		if (!Information.IsDBNull(pObj) && !ReferenceEquals(pObj, null) && !(System.Convert.ToString(pObj) == string.Empty))
		{
			return System.Convert.ToDateTime(pObj);
		}
		else
		{
			return returnValue;
		}
	}

	/// <summary>
	/// 行程_降車ヶ所エンティティ＆その他行程_降車ヶ所エンティティから降車ヶ所エンティティへマージ
	/// ※コースマスタエンティティ→降車ヶ所マスタエンティティ→行程_降車ヶ所エンティティ→tbl行程情報→grd行程情報
	/// </summary>
	/// <remarks></remarks>
	private void returnKoshakashoEntity() //return降車ヶ所エンティティ()
	{
		EntityOperation _copyEntity = new EntityOperation(Of KoshakashoEntity); //_copyエンティティ

		for (int index = 0; index <= itinerary_KoshakashoEntity.EntityData.Length - 1; index++)
		{
			if (itinerary_KoshakashoEntity.EntityData(index).Teiki_KikakuKbn.Value != string.Empty &&)
			{
				itinerary_KoshakashoEntity.EntityData(index).Year.Value != string.Empty &&;
				itinerary_KoshakashoEntity.EntityData(index).LineNo.Value IsNot null;
				_copyEntity.add(itinerary_KoshakashoEntity.EntityData(index));
			}
		}
		for (int index = 0; index <= sonotaItinerary_KoshakashoEntity.EntityData.Length - 1; index++)
		{
			if (sonotaItinerary_KoshakashoEntity.EntityData(index).Teiki_KikakuKbn.Value != string.Empty &&)
			{
				sonotaItinerary_KoshakashoEntity.EntityData(index).Year.Value != string.Empty &&;
				sonotaItinerary_KoshakashoEntity.EntityData(index).LineNo.Value IsNot null;
				_copyEntity.add(sonotaItinerary_KoshakashoEntity.EntityData(index));
			}
		}
		if (_copyEntity.EntityData(0).Teiki_KikakuKbn.Value == string.Empty &&)
		{
			_copyEntity.EntityData(0).Year.Value = string.Empty &&;
			ReferenceEquals(_copyEntity.EntityData(0).LineNo.Value, null) &&;
			_copyEntity.EntityData.Length(!= 1);
			_copyEntity.remove(0);
		}
		koshakashoEntity = _copyEntity.deepCopy;

	}

	/// <summary>
	/// 最終処理
	/// </summary>
	/// <remarks></remarks>
	private void returnCrsMasterEntity() //returnコースマスタエンティティ()
	{
		//②降車ヶ所エンティティ↓
		object with_1 = crsMasterEntity.EntityData(0);
		//親エンティティへデータを戻す↓
		with_1.KoshakashoEntity.clear();
		for (int KoshakashoIndex = 0; KoshakashoIndex <= koshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			with_1.KoshakashoEntity.add(koshakashoEntity.EntityData(KoshakashoIndex));
		}
		with_1.KoshakashoEntity.remove(0);

		//エンティティの最終処理↓
		for (int KoshakashoIndex = 0; KoshakashoIndex <= with_1.KoshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			with_1.KoshakashoEntity.copy_BackupFromZenkaiValue();

			//降車ヶ所エンティティに紐付く全てのエンティティの最終処理↓
			object with_2 = with_1.KoshakashoEntity.EntityData(KoshakashoIndex);
			with_2.StayDetailEntity.copy_BackupFromZenkaiValue();
			with_2.MealDetailEntity.copy_BackupFromZenkaiValue();
			with_2.SiireInfoEntity.copy_BackupFromZenkaiValue();
			with_2.CostMaster_CarrierEntity.copy_BackupFromZenkaiValue();
			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_2.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				with_2.CostMaster_CarrierEntity(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.copy_BackupFromZenkaiValue();
			}
			with_2.CostMaster_KoshakashoEntity.copy_BackupFromZenkaiValue();
			for (int CostKoshaIndex = 0; CostKoshaIndex <= with_2.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshaIndex++)
			{
				with_2.CostMaster_KoshakashoEntity(CostKoshaIndex).CostMasterKoshakashoChargeKbnEntity.copy_BackupFromZenkaiValue();
			}
			with_2.ToriatukaiInfoEntity.copy_BackupFromZenkaiValue();
			for (int ToriatukaiDetailIndex = 0; ToriatukaiDetailIndex <= with_2.ToriatukaiInfoEntity.EntityData.Length - 1; ToriatukaiDetailIndex++)
			{
				with_2.ToriatukaiInfoEntity.EntityData(ToriatukaiDetailIndex).ToriatukaiDetailEntity.copy_BackupFromZenkaiValue();
			}

		}


		if (_crsKind1 == CrsKind1Type.kikakuTravel)
		{
			//③企画旅行の場合↓
			object with_3 = crsMasterEntity.EntityData(0);

			with_3.CostMaster_CarrierEntity.clear();
			for (int CarrierIndex = 0; CarrierIndex <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity.EntityData.Length - 1; CarrierIndex++)
			{
				with_3.CostMaster_CarrierEntity.add(crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity.EntityData(CarrierIndex));
			}
			with_3.CostMaster_CarrierEntity.remove(0);

			with_3.CostMaster_HeaderEntity.clear();
			for (int HeaderIndex = 0; HeaderIndex <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData.Length - 1; HeaderIndex++)
			{
				with_3.CostMaster_HeaderEntity.add(crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData(HeaderIndex));
			}
			with_3.CostMaster_HeaderEntity.remove(0);

			//エンティティの最終処理↓
			with_3.CostMaster_HeaderEntity.copy_BackupFromZenkaiValue();
			with_3.CostMaster_CarrierEntity.copy_BackupFromZenkaiValue();
			for (int CostCarrierIndex = 0; CostCarrierIndex <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				crsMasterEntity_CostSetting.EntityData(0).CostMaster_CarrierEntity(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.copy_BackupFromZenkaiValue();
			}
			for (int CarrierIndex = 0; CarrierIndex <= with_3.CostMaster_HeaderEntity.EntityData.Length - 1; CarrierIndex++)
			{
				with_3.CostMaster_HeaderEntity.EntityData(CarrierIndex).CostMaster_PlateEntity.copy_BackupFromZenkaiValue();
			}
		}
		else
		{
			//③定期観光の場合↓
			object with_4 = crsMasterEntity.EntityData(0);
			with_4.CostMaster_HeaderEntity.clear();
			for (int HeaderIndex = 0; HeaderIndex <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData.Length - 1; HeaderIndex++)
			{
				with_4.CostMaster_HeaderEntity.add(crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData(HeaderIndex));
			}
			with_4.CostMaster_HeaderEntity.remove(0);
			//エンティティの最終処理↓
			with_4.CostMaster_HeaderEntity.copy_BackupFromZenkaiValue();
		}
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 複写したい適用開始日が既に存在しているかチェックします。
	/// </summary>
	/// <param name="_applicationStartDay">yyyy/MM/dd</param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool checkApplicationStartDayExistenceUmu(string _applicationStartDay)
	{
		bool returnValue = true;

		for (int row = 1; row <= this.grdCostSelection.Rows.Count - 1; row++)
		{
			if (nnvl_str(this.grdCostSelection.Rows(row).Item(getCostHeaderKoumokuType.ApplicationStartDay.PhysicsName)) == _applicationStartDay)
			{
				returnValue = false;
				break;
			}
		}

		return returnValue;
	}

	/// <summary>
	/// 適用開始日有効レコードを1レコード残すためのチェック
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool checkDeleteDayUmu()
	{
		bool returnValue = true;
		int _deleteDayNasicnt = 0;

		for (int row = 1; row <= this.grdCostSelection.Rows.Count - 1; row++)
		{
			if (nnvl_str(this.grdCostSelection.Rows(row).Item(getCostHeaderKoumokuType.DeleteDay.PhysicsName)) == string.Empty)
			{
				_deleteDayNasicnt++;
			}
		}

		if (_deleteDayNasicnt == 1)
		{
			return false;
		}

		return returnValue;
	}






	#endregion

	#region 処理系

	#region 複写
	/// <summary>
	/// 原価_適用開始日「複写」処理時のメソッド
	/// ※全レコードをなめ、変更前に該当する全ての適用開始日のコピーを行う。
	/// ※Removeはプレート＆キャリアの0行目で適用開始日の無いレコードを対象（ヘッダの0行目は必ず入っている前提）
	/// </summary>
	/// <remarks></remarks>
	private void copyApplicableApplicationCostHeaderRecord(string _applicationStartDay_ChangeBf)
	{
		EntityOperation[] _headerOperationcopy = null;
		EntityOperation[] _carrierOperationcopy = null;
		CostMaster_HeaderEntity _headercopy = null;
		CostMaster_CarrierEntity _carriercopy = null;

		object with_1 = crsMasterEntity_CostSetting.EntityData(0);

		//ヘッダ↓
		_headerOperationcopy = with_1.CostMaster_HeaderEntity.deepCopy;
		for (int Headerindex = 0; Headerindex <= with_1.CostMaster_HeaderEntity.EntityData.Length - 1; Headerindex++)
		{
			if (with_1.CostMaster_HeaderEntity.EntityData(Headerindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
			{
				_headercopy = _headerOperationcopy.EntityData(Headerindex);
				_headercopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
				_headercopy.UpdateDate.Value = null;
				_headercopy.UpdateUserID.Value = string.Empty;
				if (this.chkCostDelete.Checked == true)
				{
					_headercopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
				}
				else
				{
					_headercopy.DeleteDay.Value = string.Empty;
				}

				//プレート↓
				for (int PlateIndex = 0; PlateIndex <= _headercopy.CostMaster_PlateEntity.EntityData.Length - 1; PlateIndex++)
				{
					object with_2 = _headercopy.CostMaster_PlateEntity.EntityData(PlateIndex);
					if (with_2.ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
					{
						with_2.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						with_2.UpdateDate.Value = null;
						with_2.UpdateUserID.Value = string.Empty;
						if (this.chkCostDelete.Checked == true)
						{
							with_2.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							with_2.DeleteDay.Value = string.Empty;
						}
					}
				}

				with_1.CostMaster_HeaderEntity.add(_headercopy);

			}

		}

		//キャリア（行No＝0）↓
		_carrierOperationcopy = with_1.CostMaster_CarrierEntity.deepCopy;
		for (int CarrierIndex = 0; CarrierIndex <= with_1.CostMaster_CarrierEntity.EntityData.Length - 1; CarrierIndex++)
		{
			if (with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
			{
				_carriercopy = _carrierOperationcopy.EntityData(CarrierIndex);
				_carriercopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
				_carriercopy.UpdateDate.Value = null;
				_carriercopy.UpdateUserID.Value = string.Empty;
				if (this.chkCostDelete.Checked == true)
				{
					_carriercopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
				}
				else
				{
					_carriercopy.DeleteDay.Value = string.Empty;
				}

				//料金区分対応
				for (int idxChgKbn = 0; idxChgKbn <= _carriercopy.CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
				{
					_carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					_carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateDate.Value = null;
					_carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateUserID.Value = string.Empty;
					if (this.chkCostDelete.Checked == true)
					{
						_carriercopy.CostMasterCarrierChargeKbnEntity.EntityData(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						_carriercopy.CostMasterCarrierChargeKbnEntity.EntityData(idxChgKbn).DeleteDay.Value = string.Empty;
					}
				}
				with_1.CostMaster_CarrierEntity.add(_carriercopy);
				if (with_1.CostMaster_CarrierEntity.EntityData(0).ApplicationStartDay.Value == string.Empty)
				{
					with_1.CostMaster_CarrierEntity.remove(0);
				}
			}
		}
	}

	/// <summary>
	/// 原価_適用開始日「複写」処理時のメソッド
	/// ※全レコードをなめ、変更前に該当する全ての適用開始日変更する。
	/// ※Removeはプレート＆キャリアの0行目で適用開始日の無いレコードを対象（ヘッダの0行目は必ず入っている前提）
	/// </summary>
	/// <remarks></remarks>
	private void copyApplicableApplicationCostKoshakashoRecord(string _applicationStartDay_ChangeBf)
	{
		EntityOperation[] _cost_KoshakashoOperationcopy = null;
		EntityOperation[] _cost_CarrierOperationcopy = null;
		CostMaster_KoshakashoEntity _cost_Koshakashocopy = null;
		CostMaster_CarrierEntity _cost_Carriercopy = null;

		//行程↓
		for (int KoshakashoIndex = 0; KoshakashoIndex <= itinerary_KoshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			object with_1 = itinerary_KoshakashoEntity.EntityData(KoshakashoIndex);

			_cost_KoshakashoOperationcopy = with_1.CostMaster_KoshakashoEntity.deepCopy;
			for (int CostKoshakashoindex = 0; CostKoshakashoindex <= with_1.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshakashoindex++)
			{
				if (with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					_cost_Koshakashocopy = _cost_KoshakashoOperationcopy.EntityData(CostKoshakashoindex);
					_cost_Koshakashocopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					_cost_Koshakashocopy.UpdateDate.Value = null;
					_cost_Koshakashocopy.UpdateUserID.Value = string.Empty;
					if (this.chkCostDelete.Checked == true)
					{
						_cost_Koshakashocopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						_cost_Koshakashocopy.DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (int idxChgKbn = 0; idxChgKbn <= _cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).UpdateDate.Value = null;
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).UpdateUserID.Value = string.Empty;
						if (this.chkCostDelete.Checked == true)
						{
							_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity.EntityData(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity.EntityData(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
					with_1.CostMaster_KoshakashoEntity.add(_cost_Koshakashocopy);
					if (with_1.CostMaster_KoshakashoEntity.EntityData(0).ApplicationStartDay.Value == string.Empty)
					{
						with_1.CostMaster_KoshakashoEntity.remove(0);
					}
				}
			}

			_cost_CarrierOperationcopy = with_1.CostMaster_CarrierEntity.deepCopy;
			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_1.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				if (with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					_cost_Carriercopy = _cost_CarrierOperationcopy.EntityData(CostCarrierIndex);
					_cost_Carriercopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					_cost_Carriercopy.UpdateDate.Value = null;
					_cost_Carriercopy.UpdateUserID.Value = string.Empty;
					if (this.chkCostDelete.Checked == true)
					{
						_cost_Carriercopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						_cost_Carriercopy.DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (int idxChgKbn = 0; idxChgKbn <= _cost_Carriercopy.CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateDate.Value = null;
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateUserID.Value = string.Empty;
						if (this.chkCostDelete.Checked == true)
						{
							_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
					with_1.CostMaster_CarrierEntity.add(_cost_Carriercopy);
					if (with_1.CostMaster_CarrierEntity.EntityData(0).ApplicationStartDay.Value == string.Empty)
					{
						with_1.CostMaster_CarrierEntity.remove(0);
					}
				}
			}
		}

		//その他行程↓
		for (int KoshakashoIndex = 0; KoshakashoIndex <= sonotaItinerary_KoshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			object with_2 = sonotaItinerary_KoshakashoEntity.EntityData(KoshakashoIndex);

			_cost_KoshakashoOperationcopy = with_2.CostMaster_KoshakashoEntity.deepCopy;
			for (int CostKoshakashoindex = 0; CostKoshakashoindex <= with_2.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshakashoindex++)
			{
				if (with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					_cost_Koshakashocopy = _cost_KoshakashoOperationcopy.EntityData(CostKoshakashoindex);
					_cost_Koshakashocopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					_cost_Koshakashocopy.UpdateDate.Value = null;
					_cost_Koshakashocopy.UpdateUserID.Value = string.Empty;
					if (this.chkCostDelete.Checked == true)
					{
						_cost_Koshakashocopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						_cost_Koshakashocopy.DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (int idxChgKbn = 0; idxChgKbn <= _cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).UpdateDate.Value = null;
						_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).UpdateUserID.Value = string.Empty;
						if (this.chkCostDelete.Checked == true)
						{
							_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							_cost_Koshakashocopy.CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
					with_2.CostMaster_KoshakashoEntity.add(_cost_Koshakashocopy);
					if (with_2.CostMaster_KoshakashoEntity.EntityData(0).ApplicationStartDay.Value == string.Empty)
					{
						with_2.CostMaster_KoshakashoEntity.remove(0);
					}
				}
			}

			_cost_CarrierOperationcopy = with_2.CostMaster_CarrierEntity.deepCopy;
			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_2.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				if (with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					_cost_Carriercopy = _cost_CarrierOperationcopy.EntityData(CostCarrierIndex);
					_cost_Carriercopy.ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					_cost_Carriercopy.UpdateDate.Value = null;
					_cost_Carriercopy.UpdateUserID.Value = string.Empty;
					if (this.chkCostDelete.Checked == true)
					{
						_cost_Carriercopy.DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						_cost_Carriercopy.DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (int idxChgKbn = 0; idxChgKbn <= _cost_Carriercopy.CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateDate.Value = null;
						_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).UpdateUserID.Value = string.Empty;
						if (this.chkCostDelete.Checked == true)
						{
							_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							_cost_Carriercopy.CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
					with_2.CostMaster_CarrierEntity.add(_cost_Carriercopy);
					if (with_2.CostMaster_CarrierEntity.EntityData(0).ApplicationStartDay.Value == string.Empty)
					{
						with_2.CostMaster_CarrierEntity.remove(0);
					}
				}
			}
		}
	}
	#endregion

	#region 変更
	/// <summary>
	/// 原価_適用開始日「変更」処理時のメソッド
	/// ※全レコードをなめ、変更前に該当する全ての適用開始日変更する。
	/// </summary>
	/// <remarks></remarks>
	private void changeApplicableApplicationCostHeaderRecord(string _applicationStartDay_ChangeBf)
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0);
		//ヘッダ↓
		for (int Headerindex = 0; Headerindex <= with_1.CostMaster_HeaderEntity.EntityData.Length - 1; Headerindex++)
		{
			if (with_1.CostMaster_HeaderEntity.EntityData(Headerindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
			{
				if (this.txtApplicationStartDay.Text != _YmdLiteral)
				{
					with_1.CostMaster_HeaderEntity.EntityData(Headerindex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
				}
				else
				{
					with_1.CostMaster_HeaderEntity.EntityData(Headerindex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
				}
				if (this.chkCostDelete.Checked == true)
				{
					with_1.CostMaster_HeaderEntity.EntityData(Headerindex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
				}
				else
				{
					with_1.CostMaster_HeaderEntity.EntityData(Headerindex).DeleteDay.Value = string.Empty;
				}
			}
			//プレート↓
			object with_2 = with_1.CostMaster_HeaderEntity.EntityData(Headerindex);
			for (int Plateindex = 0; Plateindex <= with_2.CostMaster_PlateEntity.EntityData.Length - 1; Plateindex++)
			{
				if (with_2.CostMaster_PlateEntity.EntityData(Plateindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_2.CostMaster_PlateEntity.EntityData(Plateindex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_2.CostMaster_PlateEntity.EntityData(Plateindex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_2.CostMaster_PlateEntity.EntityData(Plateindex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_2.CostMaster_PlateEntity.EntityData(Plateindex).DeleteDay.Value = string.Empty;
					}
				}
			}
		}
		//キャリア（行No＝0）↓
		for (int CarrierIndex = 0; CarrierIndex <= with_1.CostMaster_CarrierEntity.EntityData.Length - 1; CarrierIndex++)
		{
			if (with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
			{
				if (this.txtApplicationStartDay.Text != _YmdLiteral)
				{
					with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
				}
				else
				{
					with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
				}
				if (this.chkCostDelete.Checked == true)
				{
					with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
				}
				else
				{
					with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).DeleteDay.Value = string.Empty;
				}
				//料金区分対応
				for (idxChgKbn = 0; idxChgKbn <= with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_1.CostMaster_CarrierEntity.EntityData(CarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
					}
				}
			}
		}
	}

	/// <summary>
	/// 原価_適用開始日「変更」処理時のメソッド
	/// ※全レコードをなめ、変更前に該当する全ての適用開始日変更する。
	/// </summary>
	/// <remarks></remarks>
	private void changeApplicableApplicationCostKoshakashoRecord(string _applicationStartDay_ChangeBf)
	{
		//行程↓
		for (int KoshakashoIndex = 0; KoshakashoIndex <= itinerary_KoshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			object with_1 = itinerary_KoshakashoEntity.EntityData(KoshakashoIndex);

			for (int CostKoshakashoindex = 0; CostKoshakashoindex <= with_1.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshakashoindex++)
			{
				if (with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (idxChgKbn = 0; idxChgKbn <= with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						if (this.txtApplicationStartDay.Text != _YmdLiteral)
						{
							with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						}
						else
						{
							with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
						}
						if (this.chkCostDelete.Checked == true)
						{
							with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							with_1.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
				}
			}

			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_1.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				if (with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (idxChgKbn = 0; idxChgKbn <= with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						if (this.txtApplicationStartDay.Text != _YmdLiteral)
						{
							with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						}
						else
						{
							with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
						}
						if (this.chkCostDelete.Checked == true)
						{
							with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							with_1.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
				}
			}
		}

		//その他行程↓
		for (int KoshakashoIndex = 0; KoshakashoIndex <= sonotaItinerary_KoshakashoEntity.EntityData.Length - 1; KoshakashoIndex++)
		{
			object with_2 = sonotaItinerary_KoshakashoEntity.EntityData(KoshakashoIndex);

			for (int CostKoshakashoindex = 0; CostKoshakashoindex <= with_2.CostMaster_KoshakashoEntity.EntityData.Length - 1; CostKoshakashoindex++)
			{
				if (with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (idxChgKbn = 0; idxChgKbn <= with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						if (this.txtApplicationStartDay.Text != _YmdLiteral)
						{
							with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						}
						else
						{
							with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
						}
						if (this.chkCostDelete.Checked == true)
						{
							with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							with_2.CostMaster_KoshakashoEntity.EntityData(CostKoshakashoindex).CostMasterKoshakashoChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
				}
			}

			for (int CostCarrierIndex = 0; CostCarrierIndex <= with_2.CostMaster_CarrierEntity.EntityData.Length - 1; CostCarrierIndex++)
			{
				if (with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value.Equals(_applicationStartDay_ChangeBf))
				{
					if (this.txtApplicationStartDay.Text != _YmdLiteral)
					{
						with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
					}
					else
					{
						with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
					}
					if (this.chkCostDelete.Checked == true)
					{
						with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
					}
					else
					{
						with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).DeleteDay.Value = string.Empty;
					}
					//料金区分対応
					for (idxChgKbn = 0; idxChgKbn <= with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity.EntityData.Length - 1; idxChgKbn++)
					{
						if (this.txtApplicationStartDay.Text != _YmdLiteral)
						{
							with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = this.txtApplicationStartDay.Text.Replace("/", "");
						}
						else
						{
							with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).ApplicationStartDay.Value = _applicationStartDay_ChangeBf;
						}
						if (this.chkCostDelete.Checked == true)
						{
							with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = createFactoryDA.getServerSysDate.ToString("yyyyMMdd");
						}
						else
						{
							with_2.CostMaster_CarrierEntity.EntityData(CostCarrierIndex).CostMasterCarrierChargeKbnEntity(idxChgKbn).DeleteDay.Value = string.Empty;
						}
					}
				}
			}
		}
	}


	#endregion


	#endregion
	#endregion


























	/// <summary>
	/// 日付に変換（またはNothing)
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private string convertDate(object obj)
	{
		string returnValue = null;
		if (Information.IsDBNull(obj) || ReferenceEquals(obj, null) || ReferenceEquals(obj, string.Empty))
		{
			return null;
		}
		else
		{
			return System.Convert.ToString(obj).Substring(0, 4) + "/" + System.Convert.ToString(obj).Substring(4, 2) + "/" + System.Convert.ToString(obj).Substring(6, 2);
		}
		return returnValue;
	}

	/// <summary>
	/// 日付書式に変換
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private string convertYmd(object obj)
	{
		string returnValue = null;
		if (Information.IsDBNull(obj) || ReferenceEquals(obj, null) || ReferenceEquals(obj, string.Empty))
		{
			return null;
		}
		else
		{
			return System.Convert.ToString((System.Convert.ToDateTime(obj)).ToString("yyyy/MM/dd"));
		}
		return returnValue;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="_costHeaderptyIndex"></param>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	private EntityKoumoku_MojiType get_getCostHeaderMojiType(int _costHeaderptyIndex, int _entityIndex)
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		return ((EntityKoumoku_MojiType)(with_1.getPtyValue(_costHeaderptyIndex, with_1.EntityData(_entityIndex))));
	}

	/// <summary>
	/// 年月日項目セル
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private DateEx getYmdCell()
	{
		DateEx _ymdCell = new DateEx();
		GrapeCity.Win.Editors.Fields.DateEraYearField dateEraYearField = new GrapeCity.Win.Editors.Fields.DateEraYearField();
		GrapeCity.Win.Editors.Fields.DateLiteralField dateLiteralField1 = new GrapeCity.Win.Editors.Fields.DateLiteralField("/");
		GrapeCity.Win.Editors.Fields.DateMonthField dateMonthField = new GrapeCity.Win.Editors.Fields.DateMonthField();
		GrapeCity.Win.Editors.Fields.DateLiteralField dateLiteralField2 = new GrapeCity.Win.Editors.Fields.DateLiteralField("/");
		GrapeCity.Win.Editors.Fields.DateDayField dateDayField = new GrapeCity.Win.Editors.Fields.DateDayField();

		_ymdCell.SideButtons.Clear();
		_ymdCell.Fields.AddRange(new GrapeCity.Win.Editors.Fields.DateField[]
			{dateEraYearField, dateLiteralField1, dateMonthField, dateLiteralField2, dateDayField});
		_ymdCell.AlternateText.DisplayNull.Text = " ";
		_ymdCell.ImeMode = System.Windows.Forms.ImeMode.Disable;
		return _ymdCell;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="_costHeaderptyIndex"></param>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	private EntityKoumoku_YmdType get_getCostHeaderYmdType(int _costHeaderptyIndex, int _entityIndex)
	{
		object with_1 = crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity;
		return ((EntityKoumoku_YmdType)(with_1.getPtyValue(_costHeaderptyIndex, with_1.EntityData(_entityIndex))));
	}

	/// <summary>
	/// 画面終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void S01_0315_FormClosing(object sender, FormClosingEventArgs e)
	{
		int rowIdx = 0; //rowIdx

		if (this._processMode != FixedCd.ProcessMode.reference && this._kakuteiFlg == false)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				for (rowIdx = 0; rowIdx <= crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.EntityData.Length - 1; rowIdx++)
				{
					if (crsMasterEntity_CostSetting.EntityData(0).CostMaster_HeaderEntity.compare(rowIdx) == false)
					{
						if (createFactoryMsg.messageDisp("0011") == MsgBoxResult.Cancel)
						{
							e.Cancel = true;
							return;
						}
					}
				}
			}
			catch (Exception)
			{
				createFactoryMsg.messageDisp("9001");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		this.Owner.Show();
		this.Owner.Activate();
	}

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
		F10Key_Visible = true; // F10:確定
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用
		F10Key_Text = "F10:確定";
		F2Key_Text = "F2:戻る";
	}
	#endregion

}