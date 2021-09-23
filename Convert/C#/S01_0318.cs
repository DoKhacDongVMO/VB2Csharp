/// <summary>
/// 備考情報設定
/// </summary>
/// <remarks></remarks>
public class S01_0318 //frm備考設定
{
	public S01_0318()
	{
		_koshakashoEntityBase = new EntityOperation(Of KoshakashoEntity);
		_localKoshakashoEntity = new EntityOperation(Of KoshakashoEntity);

	}

	#region  定数／変数

	private const string Guidance = "GUIDANCE"; // 仕入先案内文の列名  '案内文  '案内文

	private EntityOperation _koshakashoEntityBase;
	private ItineraryInfoKeyKoumoku _taisyoItineraryInfo = null; // 親画面から受け取る行程情報  '_対象行程情報
	private string _sisetuName; // 親画面から受け取る施設名(仕入先名)  '_施設名
	private string _suppliersCd; // 親画面から受け取る仕入先コード  '_仕入先コード
	private string _suppliersEdaban; // 親画面から受け取る仕入先枝番  '_仕入先枝番
	private ProcessMode _processMode; // 親画面から受け取る処理モード  '_処理モード
	private int _lineNo = 0; // 親画面から受け取る行No  '_行No
	private bool _kakuteiFlg = false; //_確定フラグ

	private EntityOperation _localKoshakashoEntity;

	#endregion

	#region  プロパティ

	/// <summary>
	/// 親画面との受け渡しエンティティ
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public EntityOperation[] KoshakashoEntityBase //降車ヶ所エンティティBase()
	{
		get
		{
			return _koshakashoEntityBase;
		}
		set
		{
			_koshakashoEntityBase = value;
		}
	}

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
	/// 親画面から受け取る行No
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public int LineNo //行No()
	{
		set
		{
			_lineNo = value;
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
	private void frmBikoSet_Load(System.Object sender, System.EventArgs e)
	{

		// 初期化
		this.InitializeControl();
		this.InitializeEntity();
		this.setDataToControl();

		// 処理モードによるコントロールの設定
		//TODO:共通フォームの対応待ち(谷岡コメント化)
		//Call MyBase.setProcessMode(Me._processMode)
		if (this._processMode == FixedCd.ProcessMode.reference)
		{
			setProcessMode(this._processMode, this);
			this.setButtonEnabled(this);
		}

		// フォーカスを備考に設定
		this.ActiveControl = this.txtBiko;

	}

	/// <summary>
	/// 画面終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmBikoSet_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
	{

		//*** 変更チェック ***'
		if (this._processMode != FixedCd.ProcessMode.reference && this._kakuteiFlg == false)
		{
			// 入力値をエンティティに設定
			this.getControlData();

			if (this._localKoshakashoEntity.compare(this._lineNo) == false)
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

	/// <summary>
	/// 備考テキスト変更時
	/// 改行文字を削除する。
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void txtBiko_TextChanged(object sender, System.EventArgs e)
	{

		this.txtBiko.Text = Strings.Replace(System.Convert.ToString(this.txtBiko.Text), "\r\n", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);

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
		//If メッセージ出力.messageDisp("0071", "備考情報") = MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp("0071", "備考情報") == MsgBoxResult.Ok)
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
			//TODO:共通フォームの対応待ち(谷岡コメント化)　Me->Mybaseにも変更
			//MyBase.clearErrorUmu()
			//If MyBase.Check_AllCode() = False Then
			//    Exit Sub
			//End If

			//入力値をエンティティに設定
			this.getControlData();

			//ベースのエンティティにコピーを行う(行程側で、最後の「Backupから前回値」を実行して終了）↓
			this.KoshakashoEntityBase = this._localKoshakashoEntity.deepCopy;
			this.KoshakashoEntityBase.copy_BackupFromZenkaiValue();

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
		this.txtBiko.Text = string.Empty;
		this.txtMidokoro.Text = string.Empty;

	}

	/// <summary>
	/// エンティティ初期化処理
	/// </summary>
	/// <remarks></remarks>
	private void InitializeEntity()
	{

		this._localKoshakashoEntity = this.KoshakashoEntityBase.deepCopy;
		this._localKoshakashoEntity.copy_ValueFromZenkaiValue();

	}

	/// <summary>
	/// エンティティデータをコントロールへ設定します。
	/// </summary>
	/// <remarks></remarks>
	private void setDataToControl()
	{

		this.txtKoshakasho.Text = this._sisetuName;

		this.txtBiko.Text = this._localKoshakashoEntity.EntityData(this._lineNo).Biko.Value;
		this.txtMidokoro.Text = this._localKoshakashoEntity.EntityData(this._lineNo).Midokoro.Value;

	}

	/// <summary>
	/// コントロールデータを取得取得します。
	/// </summary>
	/// <remarks></remarks>
	private void getControlData()
	{

		this._localKoshakashoEntity.EntityData(this._lineNo).Biko.Value = this.txtBiko.Text;
		this._localKoshakashoEntity.EntityData(this._lineNo).Midokoro.Value = this.txtMidokoro.Text;

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
	/// 備考情報を削除します。
	/// （行程側のエンティティを操作しています。）
	/// </summary>
	/// <remarks></remarks>
	private void clearEntity()
	{
		this.KoshakashoEntityBase.EntityData(this._lineNo).Biko.Value = string.Empty;
		this.KoshakashoEntityBase.EntityData(this._lineNo).Midokoro.Value = string.Empty;
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