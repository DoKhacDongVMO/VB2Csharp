using hatobus.DevelopSystem.PrintSelect;


public class S01_0320 //frm仕入情報
{
	public S01_0320()
	{
		_siireInfoEntity = new EntityOperation(Of SiireInfoEntity);
		_printForEntity = new EntityOperation(Of CrsMasterEntity);
		_localSiireInfoEntity = new EntityOperation(Of SiireInfoEntity);
		CrsMasterEntity = new EntityOperation[Of CrsMasterEntity + 1];

	}

	#region  定数／変数

	private ProcessMode _processMode; //_処理モード
	private EntityOperation _siireInfoEntity;
	private EntityOperation _printForEntity;
	private ItineraryInfoKeyKoumoku _taisyoItineraryInfo = null; // 親画面から受け取る行程情報  '_対象行程情報
	private string _suppliersCd; // 親画面から受け取る仕入先コード  '_仕入先コード
	private string _suppliersEdaban; // 親画面から受け取る仕入先枝番  '_仕入先枝番
	private string _tyakTime = string.Empty; //_着時間
	private string _hatuTime = string.Empty; //_発時間
	private int _taisyoEntityIndex = 0; // 対象の降車ヶ所かオプションレコードインデックス  '_対象EntityIndex

	private bool _deleteProcessFlg = false; // 削除処理フラグ

	private EntityOperation _localSiireInfoEntity;

	private const string SiireRelationCdValue_SofuSakiTantosyaName = "02"; //仕入関連コード値_送付先担当者名
	private const string SiireRelationCdValue_RecruitmentJinin＿Daisu = "06"; //仕入関連コード値_募集人員＿台数

	//↓追加
	public EntityOperation[] CrsMasterEntity;
	#endregion

	#region プロパティ

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
	/// 親画面との受け渡しエンティティ
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public EntityOperation[] SiireInfoEntity //仕入情報エンティティ()
	{
		get
		{
			return _siireInfoEntity;
		}
		set
		{
			_siireInfoEntity = value;
		}
	}

	/// <summary>
	/// 親画面から受け取るコースマスタエンティティ
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public EntityOperation[] PrintForEntity //印刷用エンティティ()
	{
		set
		{
			_printForEntity = value;
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
	/// 親画面から受け取る着時間
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public string TyakTime //着時間()
	{
		set
		{
			_tyakTime = value;
		}
	}

	/// <summary>
	/// 親画面から受け取る発時間
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public string HatuTime //発時間()
	{
		set
		{
			_hatuTime = value;
		}
	}

	#endregion

	#region イベント

	/// <summary>
	/// frm仕入情報_Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmSiireInfo_Load(System.Object sender, System.EventArgs e)
	{

		try
		{
			this.Cursor = Cursors.WaitCursor;

			_localSiireInfoEntity = _siireInfoEntity.deepCopy;
			_localSiireInfoEntity.copy_ValueFromZenkaiValue();

			clearScreen(this);

			//初期値を表示する
			setInitialValue();

			//引き渡された仕入情報にデータが設定されている場合
			if (_localSiireInfoEntity.EntityData(0).Teiki_KikakuKbn.Value != "")
			{
				setEntityDataToScreen();
			}
			//TODO:共通フォームの対応待ち(谷岡コメント化)
			//Call MyBase.setProcessMode(_processMode)
			if (_processMode == FixedCd.ProcessMode.reference)
			{
				setProcessMode(this._processMode, this);
				base.F4Key_Enabled = false;
				base.F10Key_Enabled = false;
			}

			this.ActiveControl = this.txtHatoBusTantosya;
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");

		}
		finally
		{
			this.Cursor = Cursors.Default;
		}


	}

	/// <summary>
	/// frm仕入情報_FormClosing
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmSiireInfo_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
	{

		if (this._processMode != FixedCd.ProcessMode.reference && _deleteProcessFlg == false)
		{
			//ローカルエンティティに画面値を格納
			setScreenToEntityData();

			//画面値が修正されている場合
			if (_localSiireInfoEntity.compare(0) == false)
			{
				//破棄確認メッセージ
				//TODO:共通変更対応
				//If メッセージ出力.messageDisp("0011") = MsgBoxResult.Cancel Then
				if (createFactoryMsg.messageDisp("0011") == MsgBoxResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}

		}

	}

	#region フッタ
	/// <summary>
	///  F2キー（戻る）押下時イベント
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
		//If メッセージ出力.messageDisp("0071", "仕入情報") = MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp("0071", "仕入情報") == MsgBoxResult.Ok)
		{

			clearEntity();
			_deleteProcessFlg = true;
			this.Close();
		}

	}

	/// <summary>
	/// F7キー（印刷）押下時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF7_ClickOrgProc()
	{

		//コースコード未入力時は印刷不可
		if (string.IsNullOrWhiteSpace(System.Convert.ToString(_taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd.ToString())))
		{
			createFactoryMsg.messageDisp("0014", "コースコード");
			return;
		}

		S01_0306 formSelection = new S01_0306(); //帳票選択

		//UNICODE変換可能チェック
		//TODO:共通フォームの対応待ち(谷岡コメント化)※削除してよいはず Me->MyBaseに変更
		//If MyBase.Check_UNICODE() = False Then
		//    Exit Sub
		//End If

		//画面値をローカルエンティティへ格納↓
		setScreenToEntityData();
		//ローカルエンティティにキー項目を設定
		object with_1 = _localSiireInfoEntity.EntityData(0);
		with_1.Teiki_KikakuKbn.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn;
		with_1.Year.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Year;
		with_1.Season.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Season;
		with_1.CrsCd.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd;
		with_1.KaiteiDay.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.KaiteiDay;
		with_1.InvalidFlg.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.InvalidFlg;
		with_1.ItineraryKind.Value = System.Convert.ToString(_taisyoItineraryInfo.ItineraryKind);
		with_1.LineNo.Value = _taisyoItineraryInfo.LineNo;

		if (_taisyoItineraryInfo.ItineraryKind == ItineraryKindType.option)
		{
			//オプション↓

			//オプションエンティティの対象となるレコードのインデックスを設定します↓
			_taisyoEntityIndex = System.Convert.ToInt32(_taisyoItineraryInfo.LineNo - 1);

			object with_2 = _printForEntity.EntityData(0).OptionEntity(_taisyoEntityIndex).SiireInfoEntity;
			with_2.add(_localSiireInfoEntity.EntityData(0));
			with_2.remove(0);
			setTaisyoSiireInfoRecord();
		}
		else
		{
			//行程＆その他行程↓

			//降車ヶ所エンティティの対象となるレコードのインデックスを設定します↓
			_taisyoEntityIndex = getApplicableKoshakashoRecordIndex();

			object with_3 = _printForEntity.EntityData(0).KoshakashoEntity(_taisyoEntityIndex).SiireInfoEntity;
			with_3.add(_localSiireInfoEntity.EntityData(0));
			with_3.remove(0);
			setTaisyoSiireInfoRecord();
		}

		formSelection.CrsMasterEntity = _printForEntity;
		formSelection.TaisyoCrsInfo = new object[1];
		formSelection.TaisyoCrsInfo[0] = _taisyoItineraryInfo.CrsMasterKeyKoumoku;
		formSelection.BfGamen = BfGamenType.siireInfo;

		try
		{
			formSelection.ShowDialog(this);
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
		}

	}

	/// <summary>
	///
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		//UNICODE変換可能チェック
		//TODO:共通フォームの対応待ち(谷岡コメント化) Me->MyBaseにも変更
		//MyBase.clearErrorUmu()
		//If MyBase.Check_AllCode() = False Then
		//    Exit Sub
		//End If

		if (checkScreenData() == true)
		{

			//入力チェック
			if (InputDataCheck() == false)
			{
				return;
			}

			//ローカルエンティティに画面値を格納
			setScreenToEntityData();

			//ローカルエンティティにキー項目を設定
			object with_1 = _localSiireInfoEntity.EntityData(0);
			with_1.Teiki_KikakuKbn.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn;
			with_1.Year.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Year;
			with_1.Season.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Season;
			with_1.CrsCd.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsCd;
			with_1.KaiteiDay.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.KaiteiDay;
			with_1.InvalidFlg.Value = _taisyoItineraryInfo.CrsMasterKeyKoumoku.InvalidFlg;
			with_1.ItineraryKind.Value = System.Convert.ToString(_taisyoItineraryInfo.ItineraryKind);
			with_1.LineNo.Value = _taisyoItineraryInfo.LineNo;

			this._siireInfoEntity.deepCopy(_localSiireInfoEntity, deepCopyType.value);
			_localSiireInfoEntity.copy_ValueFromZenkaiValue();

		}
		else
		{
			clearEntity();
			_deleteProcessFlg = true;

		}

		this.Close();
	}

	#endregion

	#endregion

	#region メソッド
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
				((DateEx)ctr).Text = null;
			}
			else if (ReferenceEquals(ctr.GetType, typeof(ComboBoxEx)))
			{
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
			else if (ReferenceEquals(ctr.GetType, typeof(TabControl)))
			{
				clearScreen(ctr);
			}
			else if (ReferenceEquals(ctr.GetType, typeof(TabPage)))
			{
				clearScreen(ctr);
			}
		}

	}

	/// <summary>
	/// 画面初期値の設定を行う
	/// </summary>
	/// <remarks></remarks>
	private void setInitialValue()
	{

		SiireInfo_DA clsSiireInfo_DA = new SiireInfo_DA(); //cls仕入情報_DA
		DataTable dtSuppliers = null; //dt仕入先
		DataTable dtSuppliers_Tantosya = null; //dt仕入先_担当者
		DataTable dtSuppliers_RecruitmentJininDaisu = null; //dt仕入先_募集人員台数
		string suppliersName = ""; //仕入先名
		string TEL = ""; //TEL
		string FAX = ""; //FAX
		string companyName = ""; //会社名
		string notificationHoho = ""; //通知方法
		string mail = ""; //メールアドレス

		try
		{
			dtSuppliers = clsSiireInfo_DA.GetDataSuppliersMaster(_suppliersCd, _suppliersEdaban);
		}
		catch (Exception)
		{

		}

		if (ReferenceEquals(dtSuppliers, null) == false && dtSuppliers.Rows.Count > 0)
		{
			suppliersName = getDBValueForString(dtSuppliers[0].Item("SIIRE_SAKI_NAME"));
			TEL = getDBValueForString(dtSuppliers[0].Item("TELNO_1_1")) + "-" +
				getDBValueForString(dtSuppliers[0].Item("TELNO_1_2")) + "-" +
				getDBValueForString(dtSuppliers[0].Item("TELNO_1_3"));
			TEL = TEL.Replace("--", "");
			FAX = getDBValueForString(dtSuppliers[0].Item("FAX_2_1")) + "-" +
				getDBValueForString(dtSuppliers[0].Item("FAX_2_2")) + "-" +
				getDBValueForString(dtSuppliers[0].Item("FAX_2_3"));
			FAX = FAX.Replace("--", "");

			companyName = getDBValueForString(dtSuppliers[0].Item("COMPANY_NAME_KJ"));
			notificationHoho = getDBValueForString(dtSuppliers[0].Item("NOTIFICATION_HOHO"));
			mail = getDBValueForString(dtSuppliers[0].Item("MAIL"));
		}

		//タイトル部
		txtKoshakasho.Text = suppliersName;

		//乗務員料金
		if (CrsMasterEntity.EntityData(0).JyosyaKind_Jyo1_Jyo2.Value == System.Convert.ToString(JyosyaKindType.jyo2))
		{
			lblZyo2.Visible = true;
		}
		else
		{
			lblZyo2.Visible = false;
		}

		//出力設定
		chkChargeInfoNoOut.Checked = true;
		chkTatiyoriWakuNoOut.Checked = false;
		chkChargeInfoNoOut.Enabled = true;
		if (CrsMasterEntity.EntityData(0).CrsKind1.Value != System.Convert.ToString(CrsKind1Type.kikakuTravel))
		{
			chkChargeInfoNoOut.Enabled = false;
		}

		chkTatiyoriWakuNoOut.Enabled = false;
		//If CrsMasterEntity.EntityData(0).CrsKind1.Value = CStr(CrsKind1Type.kikakuTravel) OrElse
		//(CrsMasterEntity.EntityData(0).CrsKind1.Value = CStr(CrsKind1Type.teikiRCrs) And CrsMasterEntity.EntityData(0).CrsKind2.Value = CStr(CrsKind2.stay)) OrElse
		//(CrsMasterEntity.EntityData(0).CrsKind1.Value = CStr(CrsKind1Type.gaikyakuRCrs) And CrsMasterEntity.EntityData(0).CrsKind2.Value = CStr(CrsKind2.stay)) Then
		//    chkTatiyoriWakuNoOut.Enabled = True
		//End If
		//TODO:naga ↓ 修正
		if ((CrsMasterEntity.EntityData(0).CrsKind1.Value == System.Convert.ToString(CrsKind1Type.kikakuTravel)) ||)
		{
			(CrsMasterEntity.EntityData(0).CrsKind2.Value == System.Convert.ToString(CrsKind2.stay));
			chkTatiyoriWakuNoOut.Enabled = true;
		}

		//仕入回答
		rdoUnkyuContactSaki_Local.Checked = true;

		//手仕舞連絡・連絡方法
		//手仕舞連絡・宛先
		if (notificationHoho == System.Convert.ToString(0))
		{
			txtContactHoho.Text = "メール";
			if (mail != "")
			{
				txtDestination.Text = mail;
			}
			else
			{
				txtDestination.Text = "";
			}
		}
		else if (notificationHoho == System.Convert.ToString(1))
		{
			txtContactHoho.Text = "FAX";
			if (FAX != "")
			{
				txtDestination.Text = FAX;
			}
			else
			{
				txtDestination.Text = "";
			}
		}
		else
		{
			txtContactHoho.Text = "";
			txtDestination.Text = "";
		}
		txtContactHoho.Enabled = false;
		txtDestination.Enabled = false;

		// 利用時間初期化（2018/11/13 初期化するタイミングを下記から移動）
		dtmRiyouTimeFrom.Text = string.Empty;
		dtmRiyouTimeTo.Text = string.Empty;

		//権限によるファンクションキー表示制御
		if (UserInfoManagement.updateFlg == false && UserInfoManagement.referenceFlg == true)
		{
			base.F7Key_Visible = false;
		}

		if (_siireInfoEntity.EntityData(0).Teiki_KikakuKbn.Value != "")
		{
			return;
		}

		CdBunruiType cdBunrui = null; //コード分類

		//HACK:naga (DQ-0093 待ち) → 現行通り
		if (_taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsKind3 == System.Convert.ToString(CrsKind3Type.English))
		{
			cdBunrui = CdBunruiType.siireRelation_Gaikyaku;

		}
		else if (_taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsKind1 == System.Convert.ToString(CrsKind1Type.kikakuTravel))
		{
			if (_taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsKind2 == System.Convert.ToString(CrsKind2.stay))
			{
				cdBunrui = CdBunruiType.siireRelation_Kikaku_Stay;
			}
			else
			{
				cdBunrui = CdBunruiType.siireRelation_Kikaku_Higaeri;
			}

		}
		else
		{
			cdBunrui = CdBunruiType.siireRelation_Teiki;
		}


		//仕入依頼
		try
		{
			txtHatoBusTantosya.Text = clsSiireInfo_DA.GetCdMaster(cdBunrui, SiireRelationCdValue_SofuSakiTantosyaName);
			if (companyName != string.Empty)
			{
				txtSofuSaki.Text = companyName;
			}
			else
			{
				txtSofuSaki.Text = suppliersName;
			}
			txtSofuSakiTEL.Text = TEL;
			txtSofuSakiFAX.Text = FAX;
			txtRiyouSisetu.Text = suppliersName;

			// 2018/11/13 初期化するタイミングを上記へ移動
			//dtmRiyouTimeFrom.Text = String.Empty
			//dtmRiyouTimeTo.Text = String.Empty

			if (_tyakTime != string.Empty)
			{
				dtmRiyouTimeFrom.Text = _tyakTime;
			}
			if (_hatuTime != string.Empty)
			{
				dtmRiyouTimeTo.Text = _hatuTime;
			}

			txtRecruitmentJininDaisu.Text = clsSiireInfo_DA.GetCdMaster(cdBunrui, SiireRelationCdValue_RecruitmentJinin＿Daisu);

			//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↓
			if (_taisyoItineraryInfo.CrsMasterKeyKoumoku.Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
			{
				this.txtKisetuTitle.Visible = true;
				this.lblKisetuTitle.Visible = true;
				if (_taisyoItineraryInfo.CrsMasterKeyKoumoku.CrsKbn3 == System.Convert.ToString(0))
				{
					//通年コースの場合
					txtKisetuTitle.Text = "通年";
				}
				else
				{
					//通年以外の場合
					txtKisetuTitle.Text = _taisyoItineraryInfo.CrsMasterKeyKoumoku.Season_DisplayFor;
				}

			}
			else
			{
				this.txtKisetuTitle.Visible = false;
				this.lblKisetuTitle.Visible = false;
			}

			//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↑

		}
		catch (Exception)
		{
		}

	}

	/// <summary>
	/// エンティティに格納されているデータを画面に反映する
	/// </summary>
	/// <remarks></remarks>
	private void setEntityDataToScreen()
	{

		object with_1 = _localSiireInfoEntity.EntityData(0);

		//仕入依頼
		txtHatoBusTantosya.Text = with_1.HatoBusTantosya.Value;
		txtSofuSaki.Text = with_1.SofuSaki.Value;
		txtSofuSakiTantosya.Text = with_1.SofuSakiTantosya.Value;
		txtSofuSakiTEL.Text = with_1.SofuSakiTEL.Value;
		txtSofuSakiFAX.Text = with_1.SofuSakiFAX.Value;
		txtRiyouSisetu.Text = with_1.RiyouSisetuName.Value;
		if (with_1.RiyouTimeFrom.Value != "")
		{
			//TODO:時刻変換処理を追加
			dtmRiyouTimeFrom.Value = chgStringToTimeSpan(with_1.RiyouTimeFrom.Value);
		}
		if (with_1.RiyouTimeTo.Value != "")
		{
			//TODO:時刻変換処理を追加
			dtmRiyouTimeTo.Value = chgStringToTimeSpan(with_1.RiyouTimeTo.Value);
		}
		txtTehaiNaiyo.Text = with_1.TehaiNaiyo.Value;
		txtRecruitmentJininDaisu.Text = with_1.JininDaisuSet.Value;
		//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↓
		txtKisetuTitle.Text = with_1.KisetuTitle.Value;

		//乗務員料金
		if (ReferenceEquals(with_1.UntensiCharge.Value, null) == false)
		{
			numUntensiCharge.Value = System.Convert.ToDecimal(with_1.UntensiCharge.Value);
		}
		if (ReferenceEquals(with_1.GuideCharge.Value, null) == false)
		{
			numGuideCharge.Value = System.Convert.ToDecimal(with_1.GuideCharge.Value);
		}
		if (ReferenceEquals(with_1.TenjyoinCharge.Value, null) == false)
		{
			numTenjyoinCharge.Value = System.Convert.ToDecimal(with_1.TenjyoinCharge.Value);
		}
		txtTusinran.Text = with_1.Tusinran.Value;

		//出力設定
		if (with_1.ChargeOutFlg.Value == 1)
		{
			chkChargeInfoNoOut.Checked = true;
		}
		else
		{
			chkChargeInfoNoOut.Checked = false;
		}
		if (with_1.TatiyoriWakuOutFlg.Value == 1)
		{
			chkTatiyoriWakuNoOut.Checked = true;
		}
		else
		{
			chkTatiyoriWakuNoOut.Checked = false;
		}

		//仕入回答
		if (with_1.UnkyuContactSaki.Value == 0)
		{
			rdoUnkyuContactSaki_Local.Checked = true;
		}
		else
		{
			rdoUnkyuContactSaki_Toan.Checked = true;
		}
		if (ReferenceEquals(with_1.KaitoJuryoDay.Value, null) == false)
		{
			dtmKaitoJuryoDay.Value = with_1.KaitoJuryoDay.Value;
		}
		txtKaitoTantosya.Text = with_1.KaitoTantosya.Value;
		txtKaitoTantosyaTEL.Text = with_1.KaitoContactTEL.Value;
		txtKaitoTantosyaFAX.Text = with_1.KaitoContactFAX.Value;
		txtTojituTantosya.Text = with_1.TojituTantosya.Value;
		txtTojituTantosyaTEL.Text = with_1.TojituContactTEL.Value;
		txtTojituTantosyaFAX.Text = with_1.TojituContactFAX.Value;

		//手仕舞い連絡
		if (with_1.TejimaiContactKbn.Value == 0)
		{
			txtContactHoho.Text = "メール";
		}
		else
		{
			txtContactHoho.Text = "FAX";
		}
		txtDestination.Text = System.Convert.ToString(with_1.TejimaiContactSaki.Value);


	}


	/// <summary>
	/// 画面の内容をエンティティに格納する
	/// </summary>
	/// <remarks></remarks>
	private void setScreenToEntityData()
	{

		object with_1 = _localSiireInfoEntity.EntityData(0);
		//仕入依頼
		with_1.HatoBusTantosya.Value = txtHatoBusTantosya.Text;
		with_1.SofuSaki.Value = txtSofuSaki.Text;
		with_1.SofuSakiTantosya.Value = txtSofuSakiTantosya.Text;
		with_1.SofuSakiTEL.Value = txtSofuSakiTEL.Text;
		with_1.SofuSakiFAX.Value = txtSofuSakiFAX.Text;
		with_1.RiyouSisetuName.Value = txtRiyouSisetu.Text;
		if (ReferenceEquals(dtmRiyouTimeFrom.Value, null) == false)
		{
			with_1.RiyouTimeFrom.Value = dtmRiyouTimeFrom.Text;
		}
		else
		{
			with_1.RiyouTimeFrom.Value = "";
		}
		if (ReferenceEquals(dtmRiyouTimeTo.Value, null) == false)
		{
			with_1.RiyouTimeTo.Value = dtmRiyouTimeTo.Text;
		}
		else
		{
			with_1.RiyouTimeTo.Value = "";
		}
		with_1.TehaiNaiyo.Value = txtTehaiNaiyo.Text;
		with_1.JininDaisuSet.Value = txtRecruitmentJininDaisu.Text;
		//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↓
		with_1.KisetuTitle.Value = txtKisetuTitle.Text;
		//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↑

		//乗務員料金
		if (ReferenceEquals(numUntensiCharge.Value, null) == false)
		{
			with_1.UntensiCharge.Value = System.Convert.ToInt32(numUntensiCharge.Value);
		}
		else
		{
			with_1.UntensiCharge.Value = null;
		}
		if (ReferenceEquals(numGuideCharge.Value, null) == false)
		{
			with_1.GuideCharge.Value = System.Convert.ToInt32(numGuideCharge.Value);
		}
		else
		{
			with_1.GuideCharge.Value = null;
		}
		if (ReferenceEquals(numTenjyoinCharge.Value, null) == false)
		{
			with_1.TenjyoinCharge.Value = System.Convert.ToInt32(numTenjyoinCharge.Value);
		}
		else
		{
			with_1.TenjyoinCharge.Value = null;
		}
		with_1.Tusinran.Value = txtTusinran.Text;

		//出力設定
		if (chkChargeInfoNoOut.Checked == true)
		{
			with_1.ChargeOutFlg.Value = 1;
		}
		else
		{
			with_1.ChargeOutFlg.Value = 0;
		}
		if (chkTatiyoriWakuNoOut.Checked == true)
		{
			with_1.TatiyoriWakuOutFlg.Value = 1;
		}
		else
		{
			with_1.TatiyoriWakuOutFlg.Value = 0;
		}

		//仕入回答
		if (rdoUnkyuContactSaki_Local.Checked == true)
		{
			with_1.UnkyuContactSaki.Value = 0;
		}
		else
		{
			with_1.UnkyuContactSaki.Value = 1;
		}

		if (ReferenceEquals(dtmKaitoJuryoDay.Value, null) == false)
		{
			with_1.KaitoJuryoDay.Value = dtmKaitoJuryoDay.Value;
		}
		else
		{
			with_1.KaitoJuryoDay.Value = null;
		}

		with_1.KaitoTantosya.Value = txtKaitoTantosya.Text;
		with_1.KaitoContactTEL.Value = txtKaitoTantosyaTEL.Text;
		with_1.KaitoContactFAX.Value = txtKaitoTantosyaFAX.Text;
		with_1.TojituTantosya.Value = txtTojituTantosya.Text;
		with_1.TojituContactTEL.Value = txtTojituTantosyaTEL.Text;
		with_1.TojituContactFAX.Value = txtTojituTantosyaFAX.Text;
		if (txtContactHoho.Text == "メール")
		{
			with_1.TejimaiContactKbn.Value = 0;
		}
		else
		{
			with_1.TejimaiContactKbn.Value = 1;
		}
		with_1.TejimaiContactSaki.Value = txtDestination.Text;


	}

	/// <summary>
	/// 入力データのチェックを行う
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool InputDataCheck()
	{

		dtmRiyouTimeFrom.ExistError = false;
		dtmRiyouTimeTo.ExistError = false;

		//利用時間FromToが両方入力されている場合
		if (ReferenceEquals(dtmRiyouTimeFrom.Value, null) == false && ReferenceEquals(dtmRiyouTimeTo.Value, null) == false)
		{
			//TODO:時刻変換処理を追加する
			//dtmRiyouTimeFrom.Value = CDate("2000/01/01 00:" & dtmRiyouTimeFrom.Text)
			//dtmRiyouTimeTo.Value = CDate("2000/01/01 00:" & dtmRiyouTimeTo.Text)

			if (dtmRiyouTimeFrom.Value > dtmRiyouTimeTo.Value)
			{
				dtmRiyouTimeFrom.ExistError = true;
				dtmRiyouTimeTo.ExistError = true;
				//TODO:共通変更対応
				//メッセージ出力.messageDisp("0004", "利用時間")
				createFactoryMsg.messageDisp("0004", "利用時間");
				dtmRiyouTimeFrom.Focus();
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 印刷用エンティティのレコードから印刷対象の仕入情報レコードのみを設定します。
	/// </summary>
	/// <remarks></remarks>
	private void setTaisyoSiireInfoRecord()
	{

		object with_1 = _printForEntity.EntityData(0);

		if (_taisyoItineraryInfo.ItineraryKind == ItineraryKindType.option)
		{
			for (int _OptionIndex = 0; _OptionIndex <= with_1.OptionEntity.EntityData.Length - 1; _OptionIndex++)
			{
				if (_OptionIndex != _taisyoEntityIndex)
				{
					with_1.OptionEntity(_OptionIndex).SiireInfoEntity.clear();
				}
			}
			for (int _KoshakashoIndex = 0; _KoshakashoIndex <= with_1.KoshakashoEntity.EntityData.Length - 1; _KoshakashoIndex++)
			{
				with_1.KoshakashoEntity.EntityData(_KoshakashoIndex).SiireInfoEntity.clear();
			}
		}
		else
		{
			for (int _OptionIndex = 0; _OptionIndex <= with_1.OptionEntity.EntityData.Length - 1; _OptionIndex++)
			{
				with_1.OptionEntity(_OptionIndex).SiireInfoEntity.clear();
			}
			for (int _KoshakashoIndex = 0; _KoshakashoIndex <= with_1.KoshakashoEntity.EntityData.Length - 1; _KoshakashoIndex++)
			{
				if (_KoshakashoIndex != _taisyoEntityIndex)
				{
					with_1.KoshakashoEntity.EntityData(_KoshakashoIndex).SiireInfoEntity.clear();
				}
			}
		}


	}

	/// <summary>
	/// 該当の適用開始日の原価降車ヶ所レコードのIndexを返します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private int getApplicableKoshakashoRecordIndex()
	{
		int returnValue = 0; //returnValue

		for (int index = 0; index <= _printForEntity.EntityData(0).KoshakashoEntity.EntityData.Length - 1; index++)
		{
			if (_printForEntity.EntityData(0).KoshakashoEntity.EntityData(index).LineNo.Value.Equals(_taisyoItineraryInfo.LineNo) &&
				_printForEntity.EntityData(0).KoshakashoEntity.EntityData(index).ItineraryKind.Value.Equals(System.Convert.ToString(_taisyoItineraryInfo.ItineraryKind)))
			{
				return index;
				//				break;
			}
		}

		return returnValue;
	}

	/// <summary>
	/// 仕入情報を削除します。
	/// （行程側のエンティティを操作しています。）
	/// </summary>
	/// <remarks></remarks>
	private void clearEntity()
	{
		this._siireInfoEntity.EntityData(0).Teiki_KikakuKbn.Value = string.Empty;
		setClearEntityData();
	}

	/// <summary>
	/// 画面の内容をエンティティに格納する
	/// </summary>
	/// <remarks></remarks>
	private void setClearEntityData()
	{

		object with_1 = _siireInfoEntity.EntityData(0);
		with_1.HatoBusTantosya.Value = string.Empty;
		with_1.SofuSaki.Value = string.Empty;
		with_1.SofuSakiTantosya.Value = string.Empty;
		with_1.SofuSakiTEL.Value = string.Empty;
		with_1.SofuSakiFAX.Value = string.Empty;
		with_1.RiyouSisetuName.Value = string.Empty;
		with_1.RiyouTimeFrom.Value = string.Empty;
		with_1.RiyouTimeTo.Value = string.Empty;
		with_1.TehaiNaiyo.Value = string.Empty;
		with_1.JininDaisuSet.Value = string.Empty;
		with_1.UntensiCharge.Value = null;
		with_1.GuideCharge.Value = null;
		with_1.TenjyoinCharge.Value = null;
		with_1.Tusinran.Value = string.Empty;
		with_1.ChargeOutFlg.Value = 0;
		with_1.TatiyoriWakuOutFlg.Value = 0;
		with_1.UnkyuContactSaki.Value = 0;
		with_1.TejimaiContactSaki.Value = 0;
		with_1.KaitoJuryoDay.Value = null;
		with_1.KaitoTantosya.Value = string.Empty;
		with_1.KaitoContactTEL.Value = string.Empty;
		with_1.KaitoContactFAX.Value = string.Empty;
		with_1.TojituTantosya.Value = string.Empty;
		with_1.TojituContactTEL.Value = string.Empty;
		with_1.TojituContactFAX.Value = string.Empty;
	}

	/// <summary>
	/// 仕入情報を設定しているかチェック
	/// </summary>
	/// <remarks></remarks>
	private bool checkScreenData()
	{
		//仕入依頼
		if (txtHatoBusTantosya.Text != string.Empty)
		{
			return true;
		}
		if (txtSofuSaki.Text != string.Empty)
		{
			return true;
		}
		if (txtSofuSakiTantosya.Text != string.Empty)
		{
			return true;
		}
		if (txtSofuSakiTEL.Text != string.Empty)
		{
			return true;
		}
		if (txtSofuSakiFAX.Text != string.Empty)
		{
			return true;
		}
		if (txtRiyouSisetu.Text != string.Empty)
		{
			return true;
		}
		if (dtmRiyouTimeFrom.Value IsNot null)
		{
			return true;
		}
		if (dtmRiyouTimeTo.Value IsNot null)
		{
			return true;
		}
		if (txtTehaiNaiyo.Text != string.Empty)
		{
			return true;
		}
		if (txtRecruitmentJininDaisu.Text != string.Empty)
		{
			return true;
		}
		//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↓
		if (txtKisetuTitle.Text != string.Empty)
		{
			return true;
		}
		//UPD-20130617-6月運用サポート-タイトルの出力内容の自由化↑

		//乗務員料金
		if (numUntensiCharge.Value IsNot null)
		{
			return true;
		}
		if (numGuideCharge.Value IsNot null)
		{
			return true;
		}
		if (numTenjyoinCharge.Value IsNot null)
		{
			return true;
		}
		if (txtTusinran.Text != string.Empty)
		{
			return true;
		}

		//出力設定
		if (chkChargeInfoNoOut.Enabled == true)
		{
			if (chkChargeInfoNoOut.Checked == true)
			{
				return true;
			}
		}
		if (chkTatiyoriWakuNoOut.Checked == true)
		{
			return true;
		}

		if (dtmKaitoJuryoDay.Value IsNot null)
		{
			return true;
		}
		if (txtKaitoTantosya.Text != string.Empty)
		{
			return true;
		}
		if (txtKaitoTantosyaTEL.Text != string.Empty)
		{
			return true;
		}
		if (txtKaitoTantosyaFAX.Text != string.Empty)
		{
			return true;
		}
		if (txtTojituTantosya.Text != string.Empty)
		{
			return true;
		}
		if (txtTojituTantosyaTEL.Text != string.Empty)
		{
			return true;
		}
		if (txtTojituTantosyaFAX.Text != string.Empty)
		{
			return true;
		}

		return false;
	}

	#region 部品
	/// <summary>
	/// DBより取得した値をString型に変換する
	/// ※DBNullは空文字に変換
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private string getDBValueForString(object targetObj)
	{

		string retValue = ""; //retValue

		if (Information.IsDBNull(targetObj) == false)
		{
			retValue = targetObj.ToString().TrimEnd();
		}
		return retValue;
	}

	private TimeSpan RiyouTimeFrom = new TimeSpan(null); //利用時間From
	private void dtmRiyouTimeFrom_Validated(System.Object sender, System.EventArgs e)
	{
		if (RiyouTimeFrom != TimeSpan.MinValue)
		{
			dtmRiyouTimeFrom.Value = RiyouTimeFrom;
		}
	}
	private void dtmRiyouTimeFrom_Validating(object sender, System.ComponentModel.CancelEventArgs e)
	{
		RiyouTimeFrom = TimeSpan.MinValue;
		if (dtmRiyouTimeFrom.Value IsNot null)
		{
			RiyouTimeFrom = dtmRiyouTimeFrom.Value;
		}
	}

	private TimeSpan RiyouTimeTo = new TimeSpan(null); //利用時間To
	private void dtmRiyouTimeTo_Validated(System.Object sender, System.EventArgs e)
	{
		if (RiyouTimeTo != TimeSpan.MinValue)
		{
			dtmRiyouTimeTo.Value = RiyouTimeTo;
		}
	}
	private void dtmRiyouTimeTo_Validating(object sender, System.ComponentModel.CancelEventArgs e)
	{
		RiyouTimeTo = TimeSpan.MinValue;
		if (dtmRiyouTimeTo.Value IsNot null)
		{
			RiyouTimeTo = dtmRiyouTimeTo.Value;
		}
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
		F7Key_Visible = true; // F7:印刷
		F8Key_Visible = false; // F8:未使用
		F9Key_Visible = false; // F9:未使用
		F10Key_Visible = true; // F10:確定
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用
		F10Key_Text = "F10:確定";
		F2Key_Text = "F2:戻る";
		F4Key_Text = "F4:削除";
		F7Key_Text = "F7:印刷";
	}
	#endregion

}