/// <summary>
/// 宿泊情報管理
/// 宿泊詳細エンティティを確定する。
/// </summary>
/// <remarks>
/// Author:2011/05/10//佐藤(宏)
/// </remarks>
public class S01_0317 //frm宿泊設定
{
	public S01_0317()
	{
		_stayDetailEntity = new EntityOperation(Of StayDetailEntity);
		_localStayEntity = new EntityOperation[Of StayDetailEntity + 1];

	}

	#region  定数／変数

	private const string CdValue_Bus = "01"; //コード値_バス
	private const string CdValue_Toilet = "02"; //コード値_トイレ
	private const string CdValue_ShowerTukiToilet = "03"; //コード値_シャワー付トイレ
	private const string CdValue_Yobi1 = "04"; //コード値_予備１
	private const string CdValue_Yobi2 = "05"; //コード値_予備２
	private const string CdValue_Yobi3 = "06"; //コード値_予備３
	private const int CheckBoxInterval = 19; //チェックボックス間隔

	private EntityOperation _stayDetailEntity;
	private string _sisetuName = string.Empty; // 親画面から受け取る施設名  '_施設名
	private ItineraryInfoKeyKoumoku _taisyoItineraryInfo = null; // 親画面から受け取る行程情報  '_対象行程情報
	private ProcessMode _processMode; // 親画面から受け取る処理モード  '_処理モード

	private EntityOperation[] _localStayEntity;
	private CdMasterEntity _cdMasterEntity = new CdMasterEntity(); // 項目名取得用  '_コードマスタエンティティ

	private bool _kakuteiFlg = false; // 確定した場合、Trueになるフラグ  '_確定フラグ

	#endregion

	#region  構造体／列挙型

	// 可変チェックボックスのID設定
	private enum _StaySetubiID
	{
		bus = 0, //バス
		toilet, //トイレ
		showerTukiToilet, //シャワー付トイレ
		yobi1, //予備１
		yobi2, //予備２
		yobi3 //予備３
	}

	#endregion

	#region  プロパティ

	/// <summary>
	/// 遷移元画面との受け渡しエンティティ
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public StayDetailEntity StayDetailEntity //宿泊詳細エンティティ()
	{
		get
		{
			return _stayDetailEntity.EntityData(0);
		}
		set
		{
			_stayDetailEntity.EntityData[0] = value;
		}
	}

	/// <summary>
	/// 遷移元画面から受け取る施設名(仕入先名)
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
	/// 遷移元画面から受け取る行程情報
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public ItineraryInfoKeyKoumoku TaisyoItineraryInfo //対象行程情報()
	{
		set
		{
			_taisyoItineraryInfo = value;
		}
	}

	/// <summary>
	/// 遷移元画面から受け取る処理モード
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
	/// 遷移元画面に返す確定フラグ
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
	private void frmStaySet_Load(System.Object sender, System.EventArgs e) //frm宿泊設定_Load(sender,e)
	{

		// 初期化
		this.setControlInitialize();

		// 宿泊設備の表示/非表示の設定
		try
		{
			// カーソルを砂時計に変更
			this.Cursor = Cursors.WaitCursor;

			this.setSetsubiInfoVisible();

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
			// カーソルを元に戻す
			this.Cursor = Cursors.Default;
		}

		// ベースエンティティをローカルエンティティにコピー
		this._localStayEntity = this._stayDetailEntity.deepCopy();

		// キー項目がNULLの場合は新規と判断しエンティティを初期化する
		if (string.IsNullOrEmpty(System.Convert.ToString(this._localStayEntity.EntityData(0).Teiki_KikakuKbn.Value)) == true)
		{
			this.setEntityInitialize();
		}
		else
		{
			// 各項目に値を設定
			this.setControlValue();
		}

		// 親画面から受け取った仕入先名を設定
		this.txtKoshakasho.Text = this._sisetuName;

		// 今回値を前回値に上書く
		this._localStayEntity.copy_ValueFromZenkaiValue();

		// 処理モードによるコントロールの設定
		//TODO:共通フォームの対応待ち(谷岡コメント化)
		//Call MyBase.setProcessMode(Me._processMode)
		if (this._processMode == FixedCd.ProcessMode.reference)
		{
			setProcessMode(this._processMode, this);
			this.setButtonEnabled(this);
		}

		// イベントの追加
		this.txtBiko.TextChanged += txtBiko_TextChanged;

		// 部屋情報_和室にフォーカスを設定
		this.ActiveControl = this.chkWasitu;

	}

	/// <summary>
	/// 画面終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmStaySet_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) //frm宿泊設定_FormClosing(sender,e)
	{

		if (this._processMode != FixedCd.ProcessMode.reference && _kakuteiFlg == false)
		{
			// エンティティに入力値を設定
			this.setLocalEntity();

			if (this._localStayEntity.compare(0) == false)
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
	private void txtBiko_TextChanged(object sender, System.EventArgs e) //txt備考_TextChanged(sender,e)
	{

		this.txtBiko.Text = Strings.Replace(System.Convert.ToString(this.txtBiko.Text), "\r\n", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);

	}

	/// <summary>
	/// 設備チェックボックスのロケーションチェンジイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void chkSetubi_LocationChanged(object sender, System.EventArgs e)
	{
		chkYobi1.LocationChanged(, chkYobi2.LocationChanged, chkYobi3.LocationChanged);

		Padding margin = null; //margin
		margin.Left = 3;
		margin.Top = 3;
		margin.Bottom = 3;
		margin.Right = 3;

		if (this.chkToilet.Location.X < CheckBoxInterval && this.chkToilet.Location.X > 0)
		{
			this.chkToilet.Margin = margin;
		}
		if (this.chkShowerTukiToilet.Location.X < CheckBoxInterval && this.chkShowerTukiToilet.Location.X > 0)
		{
			this.chkShowerTukiToilet.Margin = margin;
		}
		if (this.chkYobi1.Location.X < CheckBoxInterval && this.chkYobi1.Location.X > 0)
		{
			this.chkYobi1.Margin = margin;
		}
		if (this.chkYobi2.Location.X < CheckBoxInterval && this.chkYobi2.Location.X > 0)
		{
			this.chkYobi2.Margin = margin;
		}
		if (this.chkYobi3.Location.X < CheckBoxInterval && this.chkYobi3.Location.X > 0)
		{
			this.chkYobi3.Margin = margin;
		}

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
		//If メッセージ出力.messageDisp("0071", "宿泊情報") = MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp("0071", "宿泊情報") == MsgBoxResult.Ok)
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

		//UNICODE変換可能チェック
		//TODO:共通フォームの対応待ち(谷岡コメント化)　Me->Mybaseにも変更
		//MyBase.clearErrorUmu()
		//If MyBase.Check_AllCode() = False Then
		//    Exit Sub
		//End If

		// エンティティに入力値を設定
		this.setLocalEntity();
		this._localStayEntity.copy_ValueFromZenkaiValue();

		// ベースエンティティにローカルエンティティの今回値を設定
		this._stayDetailEntity.deepCopy(this._localStayEntity, deepCopyType.value);
		this._kakuteiFlg = true;

		this.Close();

	}

	#endregion

	#endregion

	#region  メソッド

	#region  初期化

	/// <summary>
	/// コントロールの初期化処理
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitialize()
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

		// チェックボックスのクリア
		this.chkWasitu.Checked = false;
		this.chkYositu.Checked = false;
		this.chkWayositu.Checked = false;
		this.chkBus.Checked = false;
		this.chkToilet.Checked = false;
		this.chkShowerTukiToilet.Checked = false;
		this.chkYobi1.Checked = false;
		this.chkYobi2.Checked = false;
		this.chkYobi3.Checked = false;

		// 宿泊設備情報のテキストは「コードマスタ」から取得するためクリアしておく
		this.chkBus.Text = string.Empty;
		this.chkToilet.Text = string.Empty;
		this.chkShowerTukiToilet.Text = string.Empty;
		this.chkYobi1.Text = string.Empty;
		this.chkYobi2.Text = string.Empty;
		this.chkYobi3.Text = string.Empty;

	}

	/// <summary>
	/// 宿泊詳細エンティティの初期化を行う。
	/// </summary>
	/// <remarks></remarks>
	private void setEntityInitialize()
	{

		object with_1 = this._localStayEntity.EntityData(0);

		with_1.Teiki_KikakuKbn.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn;
		with_1.CrsCd.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd;
		with_1.KaiteiDay.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.KaiteiDay;
		with_1.Year.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Year;
		with_1.Season.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.Season;
		with_1.InvalidFlg.Value = this._taisyoItineraryInfo.CrsMasterKeyKoumoku.InvalidFlg;
		with_1.WorkKind.Value = System.Convert.ToString(this._taisyoItineraryInfo.ItineraryKind);
		with_1.LineNo.Value = this._taisyoItineraryInfo.LineNo;
		with_1.WasituUmu.Value = 0;
		with_1.YosituUmu.Value = 0;
		with_1.WayosituUmu.Value = 0;
		with_1.Setubi1.Value = 0;
		with_1.Setubi2.Value = 0;
		with_1.Setubi3.Value = 0;
		with_1.Setubi4.Value = 0;
		with_1.Setubi5.Value = 0;
		with_1.Setubi6.Value = 0;
		with_1.Biko.Value = string.Empty;


	}

	#endregion

	#region  エンティティ関連

	/// <summary>
	/// ローカル変数のエンティティに入力値を設定
	/// </summary>
	/// <remarks></remarks>
	private void setLocalEntity()
	{

		object with_1 = this._localStayEntity.EntityData(0);

		// テキストボックス
		with_1.Biko.Value = this.txtBiko.Text;

		// チェックボックス
		with_1.WasituUmu.Value = getCheckedValue(this.chkWasitu.Checked);
		with_1.YosituUmu.Value = getCheckedValue(this.chkYositu.Checked);
		with_1.WayosituUmu.Value = getCheckedValue(this.chkWayositu.Checked);
		with_1.Setubi1.Value = getCheckedValue(this.chkBus.Checked);
		with_1.Setubi2.Value = getCheckedValue(this.chkToilet.Checked);
		with_1.Setubi3.Value = getCheckedValue(this.chkShowerTukiToilet.Checked);
		with_1.Setubi4.Value = getCheckedValue(this.chkYobi1.Checked);
		with_1.Setubi5.Value = getCheckedValue(this.chkYobi2.Checked);
		with_1.Setubi6.Value = getCheckedValue(this.chkYobi3.Checked);


	}

	/// <summary>
	/// 宿泊エンティティから情報を削除します。
	/// （行程側のエンティティを操作しています。）
	/// </summary>
	/// <remarks></remarks>
	private void clearEntity()
	{
		this._stayDetailEntity.EntityData(0).Teiki_KikakuKbn.Value = string.Empty;
	}

	#endregion

	#region  入力項目関連

	/// <summary>
	/// 宿泊設備情報チェックボックスの設定を行う。
	/// </summary>
	/// <remarks></remarks>
	private void setSetsubiInfoVisible()
	{

		DataTable infoTable = null; //infoTable
		int idx = 0; //idx

		// 「コードマスタ」から設備情報の取得
		try
		{
			infoTable = this.getSetsubiInfo();


			for (idx = 0; idx <= infoTable.Rows.Count - 1; idx++)
			{
				if (infoTable.Rows(idx).Item(_cdMasterEntity.CdValue.PhysicsName).ToString().Equals(CdValue_Bus))
				{
					this.chkBus.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
				else if (infoTable.Rows(idx).Item(_cdMasterEntity.CdValue.PhysicsName).ToString().Equals(CdValue_Toilet))
				{
					this.chkToilet.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
				else if (infoTable.Rows(idx).Item(_cdMasterEntity.CdValue.PhysicsName).ToString().Equals(CdValue_ShowerTukiToilet))
				{
					this.chkShowerTukiToilet.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
				else if (infoTable.Rows(idx).Item(_cdMasterEntity.CdValue.PhysicsName).ToString().Equals(CdValue_Yobi1))
				{
					this.chkYobi1.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
				else if (infoTable.Rows(idx).Item(_cdMasterEntity.CdValue.PhysicsName).ToString().Equals(CdValue_Yobi2))
				{
					this.chkYobi2.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
				else
				{
					this.chkYobi3.Text = infoTable.Rows(idx).Item(_cdMasterEntity.CdNm.PhysicsName).ToString();
				}
			}


			// チェックボックスのVisible設定
			// ※初期設定で非表示にするとLocationが0になってしまうため、初期表示で非表示にはしない
			this.chkBus.Visible = this.getCheckboxVisible(this.chkBus);
			this.chkToilet.Visible = this.getCheckboxVisible(this.chkToilet);
			this.chkShowerTukiToilet.Visible = this.getCheckboxVisible(this.chkShowerTukiToilet);
			this.chkYobi1.Visible = this.getCheckboxVisible(this.chkYobi1);
			this.chkYobi2.Visible = this.getCheckboxVisible(this.chkYobi2);
			this.chkYobi3.Visible = this.getCheckboxVisible(this.chkYobi3);

		}
		catch (Exception ex)
		{
			throw (ex);
		}

	}

	/// <summary>
	/// チェックボックスのVisibleの値を返す。
	/// </summary>
	/// <param name="chkObj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool getCheckboxVisible(CheckBoxEx chkObj)
	{

		if (string.IsNullOrEmpty(System.Convert.ToString(chkObj.Text)) == true)
		{
			return false;
		}
		else
		{
			return true;
		}

	}

	/// <summary>
	/// 各項目に値を設定
	/// </summary>
	/// <remarks></remarks>
	private void setControlValue()
	{

		object with_1 = this._localStayEntity.EntityData(0);

		// テキストボックス
		this.txtBiko.Text = with_1.Biko.Value;

		// チェックボックス
		this.chkWasitu.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.WasituUmu.Value, 0)));
		this.chkYositu.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.YosituUmu.Value, 0)));
		this.chkWayositu.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.WayosituUmu.Value, 0)));
		this.chkBus.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi1.Value, 0)));
		this.chkToilet.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi2.Value, 0)));
		this.chkShowerTukiToilet.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi3.Value, 0)));
		this.chkYobi1.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi4.Value, 0)));
		this.chkYobi2.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi5.Value, 0)));
		this.chkYobi3.Checked = getCheckBoxValue(System.Convert.ToInt32(CommonProcess.Nvl(with_1.Setubi6.Value, 0)));


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

	#endregion

	#region  データアクセス関連

	/// <summary>
	/// 設備情報のデータテーブルを返す。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private DataTable getSetsubiInfo()
	{

		StaySet_DA dataAccess = new StaySet_DA(); //dataAccess
		DataTable returnValue = null; //returnValue
		Hashtable parameterList = null; //parameterList

		try
		{
			returnValue = dataAccess.getSetsubiData(parameterList);

		}
		catch (Exception ex)
		{
			throw (ex);
		}

		return returnValue;

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