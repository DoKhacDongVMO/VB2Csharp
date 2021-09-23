/// <summary>
/// 取扱情報管理
/// </summary>
/// <remarks></remarks>
public class S01_0319 //frm取扱設定
{
	public S01_0319()
	{
		_toriatukaiInfoEntityBase = new EntityOperation(Of ToriatukaiInfoEntity);
		_localToriatukaiEntity = new EntityOperation(Of ToriatukaiInfoEntity);

	}

	#region  定数／変数

	private EntityOperation _toriatukaiInfoEntityBase; 
	private ItineraryInfoKeyKoumoku _taisyoItineraryInfo = null; // 親画面から受け取る行程情報  '_対象行程情報
	private string _sisetuName; // 親画面から受け取る施設名(仕入先名)  '_施設名
	private string _suppliersCd = string.Empty; // 親画面から受け取る仕入先コード  '_仕入先コード
	private string _suppliersEdaban = string.Empty; // 親画面から受け取る仕入先枝番  '_仕入先枝番
	private ProcessMode _processMode; // 親画面から受け取る処理モード  '_処理モード
	private bool _kakuteiFlg = false; //_確定フラグ

	private EntityOperation _localToriatukaiEntity; 

	//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↓
	private KoshakashoEntity _koshakashoEntityBase = new KoshakashoEntity(); //_降車ヶ所エンティティBase
	#endregion

	#region  構造体／列挙型

	/// <summary>
	/// 取扱詳細エンティティ配列要素
	/// </summary>
	/// <remarks></remarks>
	private enum ToriatukaiDetailIndex : int //取扱詳細Index
	{
		line1 = 0, //行１
		line2 = 1, //行２
		line3 = 2, //行３
		line4 = 3, //行４
		line5 = 4 //行５
	}

	#endregion

	#region  プロパティ

	/// <summary>
	/// 親画面との受け渡しエンティティ
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public EntityOperation[] ToriatukaiInfoEntityBase //取扱情報エンティティBase()
	{
		get
		{
			return _toriatukaiInfoEntityBase;
		}
		set
		{
			_toriatukaiInfoEntityBase = value;
		}
	}

	//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↓
	/// <summary>
	/// 降車ヶ所エンティティ（参照のみ）
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public KoshakashoEntity KoshakashoEntityBase //降車ヶ所エンティティBase()
	{
		set
		{
			_koshakashoEntityBase = value;
		}
	}
	//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↑

	/// <summary>
	/// 親画面から受け取る行程情報キー項目
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public ItineraryInfoKeyKoumoku ItineraryInfo //行程情報()
	{
		set
		{
			_taisyoItineraryInfo = value;
		}
	}

	/// <summary>
	/// 親画面から受け取る仕入先名
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public string SuppliersName //仕入先名()
	{
		set
		{
			_sisetuName = value;
		}
	}

	/// <summary>
	/// 親画面から受け取る仕入先コード
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public string SuppliersCd //仕入先コード()
	{
		set
		{
			_suppliersCd = value;
		}
	}

	/// <summary>
	/// 親画面から受け取る仕入先枝番
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public string SuppliersEdaban //仕入先枝番()
	{
		set
		{
			_suppliersEdaban = value;
		}
	}

	/// <summary>
	/// 親画面から受け取る処理モード
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
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool KakuteiFlg //確定フラグ()
	{
		get
		{
			return _kakuteiFlg;
		}
		set
		{
			_kakuteiFlg = value;
		}
	}

	#endregion

	#region  イベント

	#region  画面

	/// <summary>
	/// 画面起動時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmToriatukaiSet_Load(System.Object sender, System.EventArgs e) //frm取扱設定_Load
	{

		// 初期化
		this.InitializeControl();

		this._localToriatukaiEntity = this.ToriatukaiInfoEntityBase.deepCopy();
		this._localToriatukaiEntity.copy_ValueFromZenkaiValue();

		if (string.IsNullOrEmpty(System.Convert.ToString(this._localToriatukaiEntity.EntityData(0).Teiki_KikakuKbn.Value)) == true && this._processMode != FixedCd.ProcessMode.reference)
		{
			// 新規の場合、「仕入先マスタ」のデータを取得して設定
			try
			{
				// カーソルを砂時計に変更
				this.Cursor = Cursors.WaitCursor;

				this.InitializeEntity();

				this.setShiireSakiData();

				//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↓
				this.setDefaultPrice();

			}
			catch (Exception ex)
			{
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("9001")
				//MyBase.outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.その他, Me.setTitle & "_起動", ex.Message)
				createFactoryMsg.messageDisp("9001");
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, this.setTitle + "_起動", ex.Message);

			}
			finally
			{
				this.Cursor = Cursors.Default;
			}

		}
		else
		{
			this.InitializeEntity();
		}

		this.InitialilzeComboBox();
		this.setDataToControl();

		// 処理モードによるコントロールの設定
		//TODO:共通フォームの対応待ち(谷岡コメント化)
		//Call MyBase.setProcessMode(Me._processMode)
		if (this._processMode == FixedCd.ProcessMode.reference)
		{
			setProcessMode(this._processMode, this);
			this.setButtonEnabled(this);
		}

		// フォーカスを施設名に設定する
		this.ActiveControl = this.txtTEL;

	}

	/// <summary>
	/// 画面終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmToriatukaiSet_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) //frm取扱設定_FormClosing
	{

		bool diffFlg = false; //diffFlg
		int index = 0; //index

		//*** 変更チェック ***'
		if (this._processMode != FixedCd.ProcessMode.reference && _kakuteiFlg == false)
		{

			this.getControlData();

			if (this._localToriatukaiEntity.compare(0) == false)
			{
				diffFlg = true;
			}

			object with_1 = _localToriatukaiEntity.EntityData(0);
			for (index = 0; index <= with_1.ToriatukaiDetailEntity.EntityData.Length - 1; index++)
			{
				if (with_1.ToriatukaiDetailEntity.compare(index) == false)
				{
					diffFlg = true;
				}
			}

			if (diffFlg == true)
			{
				//TODO:共通変更対応
				//If メッセージ出力.messageDisp("0011") = MsgBoxResult.Cancel Then
				if (createFactoryMsg.messageDisp("0011") == MsgBoxResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}

		}

		this.Owner.Show();
		this.Owner.Activate();

	}

	#endregion

	#region  フッター

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
	/// [F4:削除]ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF4_ClickOrgProc()
	{
		//TODO:共通変更対応
		//If メッセージ出力.messageDisp("0071", "取扱情報") = MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp("0071", "取扱情報") == MsgBoxResult.Ok)
		{

			clearEntity();

			this._kakuteiFlg = true;
			this.Close();
		}

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
			//TODO:共通フォームの対応待ち(谷岡コメント化) Me->MyBaseに変更
			//MyBase.clearErrorUmu()
			//If MyBase.Check_AllCode() = False Then
			//    Exit Sub
			//End If

			if (checkScreenData() == true)
			{

				getControlData();

				//ベースのエンティティにコピーを行う(行程側で、最後の「Backupから前回値」を実行して終了）↓
				this.ToriatukaiInfoEntityBase = _localToriatukaiEntity.deepCopy;
				this.ToriatukaiInfoEntityBase.copy_BackupFromZenkaiValue();
				this.ToriatukaiInfoEntityBase.EntityData(0).ToriatukaiDetailEntity.copy_ZenkaiValueFromBackup();

			}
			else
			{
				clearEntity();

			}

			this._kakuteiFlg = true;
			this.Close();

		}
		catch (Exception)
		{
			//メッセージ出力
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");

		}
		finally
		{
			//カーソルを戻す
			this.Cursor = Cursors.Default;
		}
	}

	#endregion

	#endregion

	#region  メソッド

	/// <summary>
	/// コントロールの初期化
	/// </summary>
	/// <remarks></remarks>
	private void InitializeControl()
	{

		// フッターボタンの初期化
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = true;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = true;
		this.F11Key_Visible = false;
		this.F11Key_Enabled = false;
		this.F12Key_Visible = false;

		// テキストエリアのクリア
		this.txtKoshakasho.Text = string.Empty;
		this.txtTEL.Text = string.Empty;
		this.txtFAX.Text = string.Empty;
		this.txtTantosya.Text = string.Empty;
		this.txtToriatukaiYouryo.Text = string.Empty;

		this.txtKingaku1.Text = string.Empty;
		this.txtKingaku2.Text = string.Empty;
		this.txtKingaku3.Text = string.Empty;
		this.txtKingaku4.Text = string.Empty;
		this.txtKingaku5.Text = string.Empty;

		this.txtPercent1.Text = string.Empty;
		this.txtPercent2.Text = string.Empty;
		this.txtPercent3.Text = string.Empty;
		this.txtPercent4.Text = string.Empty;
		this.txtPercent5.Text = string.Empty;

		this.txtBiko1.Text = string.Empty;
		this.txtBiko2.Text = string.Empty;
		this.txtBiko3.Text = string.Empty;
		this.txtBiko4.Text = string.Empty;
		this.txtBiko5.Text = string.Empty;

		this.txtToriatukaiBiko.Text = string.Empty;

	}

	/// <summary>
	/// エンティティ初期化処理
	/// ※降車ヶ所Noに紐付く取扱情報エンティティを対象とする。（取扱情報は1レコード、取扱詳細は複数レコード）
	/// ※配列０番目は必ず存在していること、取扱詳細を５レコード作成することを前提とする
	/// </summary>
	/// <remarks></remarks>
	private void InitializeEntity()
	{

		this._localToriatukaiEntity.EntityData(0).Teiki_KikakuKbn.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn;
		this._localToriatukaiEntity.EntityData(0).CrsCd.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd;
		this._localToriatukaiEntity.EntityData(0).Year.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Year;
		this._localToriatukaiEntity.EntityData(0).Season.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Season;
		this._localToriatukaiEntity.EntityData(0).InvalidFlg.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.InvalidFlg;
		this._localToriatukaiEntity.EntityData(0).ItineraryKind.Value = System.Convert.ToString(this._taisyoItineraryInfo.ItineraryKind);
		this._localToriatukaiEntity.EntityData(0).LineNo.Value = this._taisyoItineraryInfo.LineNo;
		this._localToriatukaiEntity.EntityData(0).SisetuName.Value = this._sisetuName;

		this._localToriatukaiEntity.copy_ValueFromZenkaiValue();

		object with_1 = this._localToriatukaiEntity.EntityData(0);
		this.setKeyItem(ToriatukaiDetailIndex.line1);
		if (with_1.ToriatukaiDetailEntity.EntityData.Length == 1)
		{
			with_1.ToriatukaiDetailEntity.add(4);
			setKeyItem(ToriatukaiDetailIndex.line2);
			setKeyItem(ToriatukaiDetailIndex.line3);
			setKeyItem(ToriatukaiDetailIndex.line4);
			setKeyItem(ToriatukaiDetailIndex.line5);
		}
		with_1.ToriatukaiDetailEntity.copy_ValueFromZenkaiValue();

	}

	/// <summary>
	/// エンティティへキー項目を挿入します。
	/// </summary>
	/// <remarks></remarks>
	private void setKeyItem(ToriatukaiDetailIndex index)
	{

		object with_1 = this._localToriatukaiEntity.EntityData(0);
		with_1.ToriatukaiDetailEntity.EntityData(index).Teiki_KikakuKbn.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn;
		with_1.ToriatukaiDetailEntity.EntityData(index).CrsCd.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd;
		with_1.ToriatukaiDetailEntity.EntityData(index).Year.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Year;
		with_1.ToriatukaiDetailEntity.EntityData(index).Season.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Season;
		with_1.ToriatukaiDetailEntity.EntityData(index).InvalidFlg.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.InvalidFlg;
		with_1.ToriatukaiDetailEntity.EntityData(index).ItineraryKind.Value = System.Convert.ToString(this._taisyoItineraryInfo.ItineraryKind);
		with_1.ToriatukaiDetailEntity.EntityData(index).LineNo.Value = this._taisyoItineraryInfo.LineNo;
		with_1.ToriatukaiDetailEntity.EntityData(index).DetailLineNo.Value = (int)index + 1;

	}

	/// <summary>
	/// コンボボックスの初期化
	/// </summary>
	/// <remarks></remarks>
	private void InitialilzeComboBox()
	{

		object with_1 = this.cmbTetuduki1;
		with_1.DataSource = getComboboxDataOfDatatable(typeof(TetudukiType), true);
		with_1.DropDown.AllowResize = false;
		with_1.ListHeaderPane.Visible = false;
		with_1.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_1.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_1.Width;
		with_1.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		object with_2 = this.cmbTetuduki2;
		with_2.DataSource = getComboboxDataOfDatatable(typeof(TetudukiType), true);
		with_2.DropDown.AllowResize = false;
		with_2.ListHeaderPane.Visible = false;
		with_2.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_2.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_2.Width;
		with_2.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		object with_3 = this.cmbTetuduki3;
		with_3.DataSource = getComboboxDataOfDatatable(typeof(TetudukiType), true);
		with_3.DropDown.AllowResize = false;
		with_3.ListHeaderPane.Visible = false;
		with_3.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_3.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_3.Width;
		with_3.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		object with_4 = this.cmbTetuduki4;
		with_4.DataSource = getComboboxDataOfDatatable(typeof(TetudukiType), true);
		with_4.DropDown.AllowResize = false;
		with_4.ListHeaderPane.Visible = false;
		with_4.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_4.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_4.Width;
		with_4.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		object with_5 = this.cmbTetuduki5;
		with_5.DataSource = getComboboxDataOfDatatable(typeof(TetudukiType), true);
		with_5.DropDown.AllowResize = false;
		with_5.ListHeaderPane.Visible = false;
		with_5.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_5.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_5.Width;
		with_5.TextSubItemIndex = ComboBoxCdType.CODE_NAME;

	}

	/// <summary>
	/// エンティティデータをコントロールへ設定します。
	/// </summary>
	/// <remarks></remarks>
	private void setDataToControl()
	{

		this.txtKoshakasho.Text = _sisetuName;

		this.txtTEL.Text = _localToriatukaiEntity.EntityData(0).ContactTEL.Value;
		this.txtFAX.Text = _localToriatukaiEntity.EntityData(0).ContactFAX.Value;
		this.txtTantosya.Text = _localToriatukaiEntity.EntityData(0).SisetuTantosya.Value;
		this.txtToriatukaiYouryo.Text = _localToriatukaiEntity.EntityData(0).ToriatukaiYouryo.Value;
		this.txtToriatukaiBiko.Text = _localToriatukaiEntity.EntityData(0).ToriatukaiBiko.Value;

		object with_1 = this._localToriatukaiEntity.EntityData(0);
		this.cmbTetuduki1.SelectedValue = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value;
		this.txtKingaku1.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Kingaku.Value;
		this.txtPercent1.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Rate.Value;
		this.txtBiko1.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Biko.Value;

		this.cmbTetuduki2.SelectedValue = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).TetudukiHoho.Value;
		this.txtKingaku2.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Kingaku.Value;
		this.txtPercent2.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Rate.Value;
		this.txtBiko2.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Biko.Value;

		this.cmbTetuduki3.SelectedValue = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).TetudukiHoho.Value;
		this.txtKingaku3.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Kingaku.Value;
		this.txtPercent3.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Rate.Value;
		this.txtBiko3.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Biko.Value;

		this.cmbTetuduki4.SelectedValue = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).TetudukiHoho.Value;
		this.txtKingaku4.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Kingaku.Value;
		this.txtPercent4.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Rate.Value;
		this.txtBiko4.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Biko.Value;

		this.cmbTetuduki5.SelectedValue = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).TetudukiHoho.Value;
		this.txtKingaku5.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Kingaku.Value;
		this.txtPercent5.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Rate.Value;
		this.txtBiko5.Text = with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Biko.Value;

	}

	/// <summary>
	/// コントロールデータを取得取得します。
	/// </summary>
	/// <remarks></remarks>
	private void getControlData()
	{

		this._localToriatukaiEntity.EntityData(0).ContactTEL.Value = this.txtTEL.Text;
		this._localToriatukaiEntity.EntityData(0).ContactFAX.Value = this.txtFAX.Text;
		this._localToriatukaiEntity.EntityData(0).SisetuTantosya.Value = this.txtTantosya.Text;
		this._localToriatukaiEntity.EntityData(0).ToriatukaiYouryo.Value = this.txtToriatukaiYouryo.Text;
		this._localToriatukaiEntity.EntityData(0).ToriatukaiBiko.Value = this.txtToriatukaiBiko.Text;

		object with_1 = this._localToriatukaiEntity.EntityData(0);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value = System.Convert.ToString(this.cmbTetuduki1.SelectedValue);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Kingaku.Value = this.txtKingaku1.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Rate.Value = this.txtPercent1.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Biko.Value = this.txtBiko1.Text;

		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).TetudukiHoho.Value = System.Convert.ToString(this.cmbTetuduki2.SelectedValue);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Kingaku.Value = this.txtKingaku2.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Rate.Value = this.txtPercent2.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line2).Biko.Value = this.txtBiko2.Text;

		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).TetudukiHoho.Value = System.Convert.ToString(this.cmbTetuduki3.SelectedValue);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Kingaku.Value = this.txtKingaku3.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Rate.Value = this.txtPercent3.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line3).Biko.Value = this.txtBiko3.Text;

		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).TetudukiHoho.Value = System.Convert.ToString(this.cmbTetuduki4.SelectedValue);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Kingaku.Value = this.txtKingaku4.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Rate.Value = this.txtPercent4.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line4).Biko.Value = this.txtBiko4.Text;

		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).TetudukiHoho.Value = System.Convert.ToString(this.cmbTetuduki5.SelectedValue);
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Kingaku.Value = this.txtKingaku5.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Rate.Value = this.txtPercent5.Text;
		with_1.ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line5).Biko.Value = this.txtBiko5.Text;

	}

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

	/// <summary>
	/// 「仕入先マスタ」からデータを取得してエンティティに設定する。
	/// </summary>
	/// <remarks></remarks>
	private void setShiireSakiData()
	{

		ToriatukaiSet_DA dataAccess = new ToriatukaiSet_DA(); //dataAccess
		Hashtable parameterList = new Hashtable(); //parameterList
		DataTable recieveTable = null; //recieveTable
		int idx = 0; //idx
		SuppliersMasterEntity shiireSakiEntity = new SuppliersMasterEntity(); // 型指定用

		parameterList.Add("SuppliersCd", this._suppliersCd);
		parameterList.Add("SuppliersEdaban", this._suppliersEdaban);

		try
		{
			recieveTable = dataAccess.getShiireSakiData(parameterList);


			if (recieveTable.Rows.Count == 1)
			{
				//UPD-20120423-TEL、FAXをハイフン付きで挿入（4月運用サポート）↓
				if (recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_1.PhysicsName).ToString() != string.Empty ||
					recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_2.PhysicsName).ToString() != string.Empty ||
					recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_3.PhysicsName).ToString() != string.Empty)
				{
					this._localToriatukaiEntity.EntityData(0).ContactTEL.Value = recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_1.PhysicsName).ToString() +
						"-" + recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_2.PhysicsName).ToString() +
						"-" + recieveTable.Rows(0).Item(shiireSakiEntity.TelNo3_3.PhysicsName).ToString();
				}
				if (recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_1.PhysicsName).ToString() != string.Empty ||
					recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_2.PhysicsName).ToString() != string.Empty ||
					recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_3.PhysicsName).ToString() != string.Empty)
				{
					this._localToriatukaiEntity.EntityData(0).ContactFAX.Value = recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_1.PhysicsName).ToString() +
						"-" + recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_2.PhysicsName).ToString() +
						"-" + recieveTable.Rows(0).Item(shiireSakiEntity.FAX3_3.PhysicsName).ToString();
				}

				//Me._ローカル取扱エンティティ.EntityData(0).連絡TEL.値 = recieveTable.Rows(0).Item(.電話番号３.物理名).ToString()
				//Me._ローカル取扱エンティティ.EntityData(0).連絡FAX.値 = recieveTable.Rows(0).Item(.ＦＡＸ３.物理名).ToString()
				//UPD-20120423-TEL、FAXをハイフン付きで挿入（4月運用サポート）↑

				this._localToriatukaiEntity.EntityData(0).ToriatukaiYouryo.Value = recieveTable.Rows(0).Item(shiireSakiEntity.ToriatukaiYouryo.PhysicsName).ToString();
				this._localToriatukaiEntity.EntityData(0).ToriatukaiBiko.Value = recieveTable.Rows(0).Item(shiireSakiEntity.Remarks1.PhysicsName).ToString()
					+ recieveTable.Rows(0).Item(shiireSakiEntity.Remarks2.PhysicsName).ToString()
					+ recieveTable.Rows(0).Item(shiireSakiEntity.Remarks3.PhysicsName).ToString()
					+ recieveTable.Rows(0).Item(shiireSakiEntity.Remarks4.PhysicsName).ToString();
			}


		}
		catch (Exception ex)
		{
			throw (ex);
		}

	}

	/// <summary>
	/// 取扱エンティティから情報を削除します。
	/// </summary>
	/// <remarks></remarks>
	private void clearEntity()
	{
		this._localToriatukaiEntity.EntityData(0).Teiki_KikakuKbn.Value = string.Empty;
		this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.clear();
	}

	/// <summary>
	/// 取扱情報を設定しているかチェック
	/// </summary>
	/// <remarks></remarks>
	private bool checkScreenData()
	{

		if (this.txtTEL.Text != string.Empty)
		{
			return true;
		}
		if (this.txtFAX.Text != string.Empty)
		{
			return true;
		}
		if (this.txtTantosya.Text != string.Empty)
		{
			return true;
		}
		if (this.txtToriatukaiYouryo.Text != string.Empty)
		{
			return true;
		}

		if (System.Convert.ToString(this.cmbTetuduki1.SelectedValue) != string.Empty)
		{
			return true;
		}
		if (System.Convert.ToString(this.cmbTetuduki2.SelectedValue) != string.Empty)
		{
			return true;
		}
		if (System.Convert.ToString(this.cmbTetuduki3.SelectedValue) != string.Empty)
		{
			return true;
		}
		if (System.Convert.ToString(this.cmbTetuduki4.SelectedValue) != string.Empty)
		{
			return true;
		}
		if (System.Convert.ToString(this.cmbTetuduki5.SelectedValue) != string.Empty)
		{
			return true;
		}

		if (this.txtKingaku1.Text != string.Empty)
		{
			return true;
		}
		if (this.txtKingaku2.Text != string.Empty)
		{
			return true;
		}
		if (this.txtKingaku3.Text != string.Empty)
		{
			return true;
		}
		if (this.txtKingaku4.Text != string.Empty)
		{
			return true;
		}
		if (this.txtKingaku5.Text != string.Empty)
		{
			return true;
		}

		if (this.txtPercent1.Text != string.Empty)
		{
			return true;
		}
		if (this.txtPercent2.Text != string.Empty)
		{
			return true;
		}
		if (this.txtPercent3.Text != string.Empty)
		{
			return true;
		}
		if (this.txtPercent4.Text != string.Empty)
		{
			return true;
		}
		if (this.txtPercent5.Text != string.Empty)
		{
			return true;
		}

		if (this.txtBiko1.Text != string.Empty)
		{
			return true;
		}
		if (this.txtBiko2.Text != string.Empty)
		{
			return true;
		}
		if (this.txtBiko3.Text != string.Empty)
		{
			return true;
		}
		if (this.txtBiko4.Text != string.Empty)
		{
			return true;
		}
		if (this.txtBiko5.Text != string.Empty)
		{
			return true;
		}

		if (this.txtToriatukaiBiko.Text != string.Empty)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 原価設定されている場合、金額に初期値として挿入
	/// </summary>
	/// <remarks></remarks>
	private void setDefaultPrice()
	{
		//原価設定無い場合は、終了↓
		if (ReferenceEquals(_koshakashoEntityBase.CostMaster_KoshakashoEntity, null))
		{
			return;
		}
		if (ReferenceEquals(_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).Teiki_KikakuKbn.Value, null))
		{
			return;
		}

		//UPD-20120821-原価情報から原価、コミッション、精算方法を取得する（8月運用サポート）↓
		//TODO:Entity変更に伴う一旦コメント化(谷岡)
		//If .CostMaster_KoshakashoEntity.EntityData(0).Adult1.Value IsNot Nothing Then
		//    Me.txtKingaku1.Text = String.Format("{0:#,0}", .CostMaster_KoshakashoEntity.EntityData(0).Adult1.Value)
		//    Me._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Kingaku.Value = String.Format("{0:#,0}", .CostMaster_KoshakashoEntity.EntityData(0).Adult1.Value)
		//End If
		if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).Com.Value IsNot null)
		{
			this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).Rate.Value = System.Convert.ToString(_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).Com.Value);
		}
		if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value IsNot null)
		{
			if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value == System.Convert.ToString(SeisanHohoType.coupon))
			{
				//クーポンにあたる手続きが無い
			}
			else if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value == System.Convert.ToString(SeisanHohoType.aoden))
			{
				this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value = System.Convert.ToString(TetudukiType.aoden);
			}
			else if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value == System.Convert.ToString(SeisanHohoType.seikyusyo))
			{
				this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value = System.Convert.ToString(TetudukiType.seikyusyo);
			}
			else if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value == System.Convert.ToString(SeisanHohoType.genkin))
			{
				this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value = System.Convert.ToString(TetudukiType.genkin);
			}
			else if (_koshakashoEntityBase.CostMaster_KoshakashoEntity.EntityData(0).SeisanHoho.Value == System.Convert.ToString(SeisanHohoType.akaden))
			{
				this._localToriatukaiEntity.EntityData(0).ToriatukaiDetailEntity.EntityData(ToriatukaiDetailIndex.line1).TetudukiHoho.Value = System.Convert.ToString(TetudukiType.akaden);
			}
			else
			{
				//該当無
			}
		}

	}
	#endregion

	#region 共通対応
	protected override void StartupOrgProc()
	{
		//フォーム起動時のフッタの各ボタンの設定
		F1Key_Visible = false; // F1:未使用
		F2Key_Visible = true; // F2:戻る
		F3Key_Visible = false; // F3:未使用
		F4Key_Visible = true; // F4:削除
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
		F4Key_Text = "F4:削除";
	}
	#endregion

}