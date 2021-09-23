/// <summary>
/// 帳票選択
/// 前画面からの提供情報を元に、出力可能な帳票と出力条件を提示する。
/// 画面から指定された条件の通りに帳票を表示＿出力する。
/// </summary>
/// <remarks>
/// Author:2011/04/27//misawa
/// </remarks>
public class S01_0306 : IFormCommonKoumokuType //frm帳票選択  'frm帳票選択
{

	#region 定数/変数
	private Form_RegularNum.Mode _processMode; //_処理モード
	private ArrayList _previewList = new ArrayList(); //_プレビューリスト
													  //ADD-20111121-パス初期値追加↓
													  // 未使用のためコメント (ph2) Private Const _OutSakiInitialValue As String = "C:\Next世代統合System\Form\"  '_出力先初期値  '_出力先初期値
													  //    '↓追加
	private EntityOperation[] _crsMasterEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //_コースマスタエンティティ
	#region プロパティ

	#region 帳票共通
	//※ここだけShadows※
	/// <summary>コースマスタエンティティ</summary>
	/// <remarks>画面連携用(TempTable作成)</remarks>
	public EntityOperation[] CrsMasterEntity //コースマスタエンティティ()
	{
		get
		{
			return _crsMasterEntity;
		}
		set
		{
			_crsMasterEntity = value;
		}
	}

	private string _outSakiFolder = string.Empty; //_出力先フォルダ
	/// <summary>ファイル出力先フォルダ</summary>
	/// <remarks>画面連携用</remarks>
	public string OutSakiFolder //出力先フォルダ()
	{
		get
		{
			return this._outSakiFolder;
		}
		set
		{
			this._outSakiFolder = value;
		}
	}

	private string _outFile; //_出力ファイル名
	/// <summary>出力ファイル名</summary>
	/// <remarks>画面連携用</remarks>
	public string OutFile //出力ファイル名()
	{
		get
		{
			return this._outFile;
		}
		set
		{
			this._outFile = value;
		}
	}

	private BfGamenType _bfGamen; //_前画面
	/// <summary>前画面情報</summary>
	/// <remarks>画面連携用</remarks>
	public Form_RegularNum.BfGamenType BfGamen //前画面()
	{
		get
		{
			return this._bfGamen;
		}
		set
		{
			this._bfGamen = value;
		}
	}

	private CrsMasterKeyKoumoku[] _taisyoCrsInfo; //_対象コース情報()
	///' <summary>対象コース情報</summary>
	///' <remarks>画面連携用</remarks>
	public FixedCd.CrsMasterKeyKoumoku[] TaisyoCrsInfo //対象コース情報()
	{
		get
		{
			return this._taisyoCrsInfo;
		}
		set
		{
			this._taisyoCrsInfo = value;
		}
	}

	private FormProcessType _formProcessKind; //_帳票処理種別
	/// <summary>帳票処理種別</summary>
	/// <remarks>画面連携用</remarks>
	public Form_RegularNum.FormProcessType FormProcessKind //帳票処理種別()
	{
		get
		{
			return this.getReportProcessType();
		}
		set
		{
			_formProcessKind = value;
		}
	}

	private bool _fileYmdUmu = true; //_ファイル名日付有無
	/// <summary>ファイル名日付有無</summary>
	/// <remarks>画面連携用</remarks>
	public bool FileYmdUmu //ファイル名日付有無()
	{
		get
		{
			return this._fileYmdUmu;
		}
		set
		{
			this._fileYmdUmu = value;
		}
	}

	#endregion

	#endregion

	#endregion

	#region イベント

	#region コンストラクタ
	public S01_0306()
	{
		// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
		_crsMasterEntity = new EntityOperation[Of CrsMasterEntity + 1];


		// この呼び出しは、Windows フォーム デザイナで必要です。
		InitializeComponent();

	}
	#endregion

	#region フォーム

	/// <summary>
	/// 画面終了時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmFormSelection_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
	{

		foreach (FormCommon Form in this._previewList)
		{
			if (Form.RosenzuViewerUse)
			{
				foreach (Rosenzu_Viewer Viewer in Form.Preview)
				{
					try
					{
						Viewer.Close();
					}
					catch (Exception)
					{
					}
					finally
					{
						Viewer.Dispose();
					}
				}

			}
			else
			{
				foreach (Form_Viewer Viewer in Form.Preview)
				{
					try
					{
						Viewer.Close();
					}
					catch (Exception)
					{
					}
					finally
					{
						Viewer.Dispose();
					}
				}
			}
		}

	}

	/// <summary>
	/// 画面ロード時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmFormSelection_Load(object sender, System.EventArgs e)
	{

		try
		{
			this.Cursor = Cursors.WaitCursor;

			//処理モードの設定
			this.setNowMode();

			//画面表示時の初期設定
			this.setControlInitiarize();

			//コース検索にて選択されたコース情報を一覧に表示
			this.setCourseList();

		}
		catch (Exception)
		{

		}
		finally
		{
			this.Cursor = Cursors.Default;
		}

	}

	#endregion

	#region ディティール

	/// <summary>
	/// 出力形式グループボックス内のラジオボタン変更時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void rdoOutKeisiki_CheckedChanged(System.Object sender, System.EventArgs e)
	{

		try
		{

			// Excelテンプレート (初期値：使用不可)
			this.lblExcelTemplateFolder.Enabled = false;
			this.txtExcelTemplateFolder.Enabled = false;
			this.btnExcelTemplateFolder.Enabled = false;

			//仕入れ情報からの呼び出し
			if (this._bfGamen == BfGamenType.siireInfo)
			{
				if (this.rdoOutKeisiki_Paper.Checked == true)
				{
					this.gbxOutKeisiki_PDF.Enabled = false;
					this.lblPDFOutSaki.Enabled = false;
					this.txtPDFOutSaki.Enabled = false;
					this.btnReference.Enabled = false;
					this.F6Key_Visible = true;

				}
				else if (this.rdoOutKeisiki_PDF.Checked == true)
				{
					this.gbxOutKeisiki_PDF.Enabled = true;
					this.lblPDFOutSaki.Enabled = true;
					this.lblPDFOutSaki.Text = "PDF出力先";
					this.txtPDFOutSaki.Enabled = true;
					this.btnReference.Enabled = true;
					this.F6Key_Visible = false;
				}
				goto endOfTry;
			}

			//仕入れ情報以外からの呼び出し
			if (rdoOutKeisiki_Paper.Checked == true)
			{
				this.gbxOutKeisiki_PDF.Enabled = false;
				this.lblPDFOutSaki.Enabled = false;
				this.txtPDFOutSaki.Enabled = false;
				this.btnReference.Enabled = false;
				base.F6Key_Visible = true;
				//定期
				this.chkTeiki_TateTable.Enabled = true;
				this.chkTeiki_Rosenzu.Enabled = true;
				this.chkTeiki_RootTable.Enabled = false;
				this.chkTeiki_RootTable.Checked = false;
				this.chkTeiki_SiireSetConfirmation.Enabled = true;
				this.chkTeiki_CostTable_ChargeConstitutionTable.Enabled = true; //
				this.chkTeiki_Toriatukai.Enabled = false;
				this.chkTeiki_Toriatukai.Checked = false;
				//コース内容確認リスト
				this.chkTeiki_CrsNaiyoKakuninList.Enabled = true;
				//ご旅行案内
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkTeiki_TravelGuide.Enabled = true;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---
				//企画
				this.chkKikaku_TateTable.Enabled = true;
				this.chkKikaku_SiireSetConfirmation.Enabled = true;
				this.chkKikaku_CostTable.Enabled = true;
				this.chkKikaku_Toriatukai.Enabled = false;
				this.chkKikaku_Toriatukai.Checked = false;
				//コース内容確認リスト
				this.chkKikaku_CrsNaiyoKakuninList.Enabled = true;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				//ご旅行案内
				this.chkKikaku_TravelGuide.Enabled = true;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---

			}
			else if (rdoOutKeisiki_PDF.Checked == true)
			{
				this.gbxOutKeisiki_PDF.Enabled = true;
				this.lblPDFOutSaki.Enabled = true;
				this.lblPDFOutSaki.Text = "PDF出力先";
				this.txtPDFOutSaki.Enabled = true;
				base.F6Key_Visible = false;
				this.btnReference.Enabled = true;
				//定期
				this.chkTeiki_TateTable.Enabled = true;
				this.chkTeiki_Rosenzu.Enabled = true;
				this.chkTeiki_RootTable.Enabled = false;
				this.chkTeiki_RootTable.Checked = false;
				this.chkTeiki_SiireSetConfirmation.Enabled = true;
				this.chkTeiki_CostTable_ChargeConstitutionTable.Enabled = true; //
				this.chkTeiki_Toriatukai.Enabled = false;
				this.chkTeiki_Toriatukai.Checked = false;
				//コース内容確認リスト
				this.chkTeiki_CrsNaiyoKakuninList.Enabled = true;
				//ご旅行案内
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkTeiki_TravelGuide.Enabled = true;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---
				//企画
				this.chkKikaku_TateTable.Enabled = true;
				this.chkKikaku_SiireSetConfirmation.Enabled = true;
				this.chkKikaku_CostTable.Enabled = true;
				this.chkKikaku_Toriatukai.Enabled = false;
				this.chkKikaku_Toriatukai.Checked = false;
				//コース内容確認リスト
				this.chkKikaku_CrsNaiyoKakuninList.Enabled = true;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				//ご旅行案内
				this.chkKikaku_TravelGuide.Enabled = true;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---

			}
			else if (rdoOutKeisiki_Excel.Checked == true)
			{
				this.gbxOutKeisiki_PDF.Enabled = true;
				this.lblPDFOutSaki.Enabled = true;
				this.lblPDFOutSaki.Text = "Excel出力先";
				this.txtPDFOutSaki.Enabled = true;
				this.btnReference.Enabled = true;
				base.F6Key_Visible = false;
				//定期
				this.chkTeiki_TateTable.Enabled = false;
				this.chkTeiki_TateTable.Checked = false;
				this.chkTeiki_Rosenzu.Enabled = false;
				this.chkTeiki_Rosenzu.Checked = false;
				this.chkTeiki_RootTable.Enabled = true;
				this.chkTeiki_SiireSetConfirmation.Enabled = false;
				this.chkTeiki_SiireSetConfirmation.Checked = false;
				this.chkTeiki_CostTable_ChargeConstitutionTable.Enabled = false;
				this.chkTeiki_CostTable_ChargeConstitutionTable.Checked = false;
				this.chkTeiki_Toriatukai.Enabled = true;
				this.chkTeiki_CrsNaiyoKakuninList.Enabled = false;
				this.chkTeiki_CrsNaiyoKakuninList.Checked = false;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkTeiki_TravelGuide.Enabled = false;
				this.chkTeiki_TravelGuide.Checked = false;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---
				//企画
				this.chkKikaku_TateTable.Enabled = false;
				this.chkKikaku_TateTable.Checked = false;
				this.chkKikaku_SiireSetConfirmation.Enabled = false;
				this.chkKikaku_SiireSetConfirmation.Checked = false;
				this.chkKikaku_CostTable.Enabled = true;
				this.chkKikaku_Toriatukai.Enabled = true;
				this.chkKikaku_CrsNaiyoKakuninList.Enabled = false;
				this.chkKikaku_CrsNaiyoKakuninList.Checked = false;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkKikaku_TravelGuide.Enabled = false;
				this.chkKikaku_TravelGuide.Checked = false;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---

				// Excelテンプレート
				this.lblExcelTemplateFolder.Enabled = true;
				this.txtExcelTemplateFolder.Enabled = true;
				this.btnExcelTemplateFolder.Enabled = true;

			}
			else
			{
				this.gbxOutKeisiki_PDF.Enabled = false;
				this.lblPDFOutSaki.Enabled = false;
				this.txtPDFOutSaki.Enabled = false;
				this.btnReference.Enabled = false;
			}

			// 初期表示用出力先フォルダ
			this.txtPDFOutSaki.Text = getInitOutputFoler();
			// 初期表示用Excelテンプレートフォルダ
			this.txtExcelTemplateFolder.Text = getInitExcelTemplateFolder();

		}
		catch (Exception)
		{
			this.gbxOutKeisiki_PDF.Enabled = false;
			this.lblPDFOutSaki.Enabled = false;
			this.txtPDFOutSaki.Enabled = false;
			this.btnReference.Enabled = false;
			return;
		}
	endOfTry:
		1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.

	}

	#region 参照ボタン (出力先)(Excelテンプレート)
	/// <summary>
	/// 参照ボタン (出力先)(Excelテンプレート)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnReference_Click(System.Object sender, System.EventArgs e)
	{

		TextBoxEx textBoxEx = null;

		if ((string)(((ButtonEx)sender).Name) == "btnReference")
		{
			textBoxEx = this.txtPDFOutSaki;
		}
		else if ((string)(((ButtonEx)sender).Name) == "btnExcelTemplateFolder")
		{
			textBoxEx = this.txtExcelTemplateFolder;
		}
		else
		{
			return;
		}

		// ダイアログを表示し、戻り値が [OK] の場合は、選択したディレクトリを表示する
		string selectedPath = string.Empty;
		if (dialogFolderBrowser(System.Convert.ToString(textBoxEx.Text), ref selectedPath) == DialogResult.OK)
		{
			textBoxEx.Text = selectedPath;
		}

	}
	#endregion

	#endregion

	#region フッタ

	/// <summary>
	/// F2ボタン実行時のイベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{

		this.Owner.Show();
		this.Owner.Activate();
		this.Close();
	}

	/// <summary>
	/// F6ボタン実行時のイベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF6_ClickOrgProc()
	{
		//プレビュー処理

		//■共通処理
		//出力対象帳票をタブインデックス順にリストアップ
		SortedList targetReportList = new SortedList(); //targetReportList  'targetReportList
		targetReportList = this.getTargetReportName();

		//＿選択チェック
		if (targetReportList.Count <= 0)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0030")
			// 出力帳票を選択してください。
			createFactoryMsg.messageDisp("0030");

			if (this._processMode == Form_RegularNum.Mode.teiki)
			{
				this.chkTeiki_TateTable.Focus();
			}
			else if (this._processMode == Form_RegularNum.Mode.kikaku)
			{
				this.chkKikaku_TateTable.Focus();
			}
			return;
		}

		try
		{
			this.Cursor = Cursors.WaitCursor;
			this.F2Key_Enabled = false;
			this.F6Key_Enabled = false;
			this.F10Key_Enabled = false;

			//■個別処理
			if (this._processMode == Form_RegularNum.Mode.teiki)
			{
				this.showPreviewTeiki();
			}
			else if (this._processMode == Form_RegularNum.Mode.kikaku)
			{
				this.showPreviewKikaku();
			}

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			this.Cursor = Cursors.Default;

			this.F2Key_Enabled = true;
			this.F6Key_Enabled = true;
			this.F10Key_Enabled = true;

		}
	}

	/// <summary>
	/// F10ボタン実行時のイベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{
		//出力処理

		//■共通処理
		//エラー情報のクリア
		//TODO:共通変更対応  Call Me.clearErrorUmu()

		//出力対象帳票をタブインデックス順にリストアップ
		SortedList targetReportList = new SortedList(); //targetReportList  'targetReportList
		targetReportList = this.getTargetReportName();

		//＿選択チェック
		if (targetReportList.Count <= 0)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0030")
			// 出力帳票を選択してください。
			createFactoryMsg.messageDisp("0030");
			if (this._processMode == Form_RegularNum.Mode.teiki)
			{
				this.chkTeiki_TateTable.Focus();
			}
			else if (this._processMode == Form_RegularNum.Mode.kikaku)
			{
				this.chkKikaku_TateTable.Focus();
			}
			return;
		}

		//＿出力先チェック
		if ((this.FormProcessKind == FormProcessType.pDF_CrsEvery) || (this.FormProcessKind == FormProcessType.pDF_AllCrs))
		{
			//入力有無チェック
			if (this.txtPDFOutSaki.Text.Trim().Length <= 0)
			{
				this.txtPDFOutSaki.ExistError = true;
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0013")
				// 未入力項目があります。
				createFactoryMsg.messageDisp("0013");
				this.txtPDFOutSaki.Focus();
				return;
			}
			//フォルダ存在チェック
			if (System.IO.Directory.Exists(this.txtPDFOutSaki.Text))
			{
				this.OutSakiFolder = System.Convert.ToString(this.txtPDFOutSaki.Text);
			}
			else
			{
				this.txtPDFOutSaki.ExistError = true;
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0002", "フォルダ")
				// 該当の{1}が存在しません。
				createFactoryMsg.messageDisp("0002", "フォルダ");
				this.txtPDFOutSaki.Focus();
				return;
			}
		}

		// Excelテンプレートファイルの存在チェック（処理の前にチェックする）
		string excelTemplateFolder = System.Convert.ToString(this.txtExcelTemplateFolder.Text);
		string notTemplateFile = string.Empty;
		if (checkExistsTemplateExcelFile(excelTemplateFolder, ref notTemplateFile) == false)
		{
			// 該当の{1}が存在しません。
			createFactoryMsg.messageDisp("0002", string.Format("テンプレートファイル({0})", notTemplateFile));
			this.txtExcelTemplateFolder.Focus();
			return;
		}

		try
		{
			this.Cursor = Cursors.WaitCursor;
			this.F2Key_Enabled = false;
			this.F6Key_Enabled = false;
			this.F10Key_Enabled = false;

			//■個別処理
			if (this._processMode == Form_RegularNum.Mode.teiki)
			{
				this.outputReportTeiki();
			}
			else if (this._processMode == Form_RegularNum.Mode.kikaku)
			{
				this.outputReportKikaku();
			}

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			this.Cursor = Cursors.Default;
			this.F2Key_Enabled = true;
			this.F6Key_Enabled = true;
			this.F10Key_Enabled = true;
			this.Activate();
		}

	}

	#endregion

	#endregion

	#region メソッド

	#region 初期化

	/// <summary>
	/// 処理モードの設定
	/// </summary>
	/// <remarks></remarks>
	private void setNowMode()
	{

		if (this._taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			this._processMode = Form_RegularNum.Mode.teiki;
		}
		else if (this._taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel))
		{
			this._processMode = Form_RegularNum.Mode.kikaku;
		}
		else
		{
			this._processMode = Form_RegularNum.Mode.processAuthorityNone;
		}

	}

	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitiarize()
	{
		int bufferFromLightEnd = 10; //ベースフッタボタン群のLeftStartLocationXとLightEndLocationXとの差分  'bufferFromLightEnd  'bufferFromLightEnd
		int intervalBetweenButton = 8; //ベースフッタボタン群の配置間隔  'intervalBetweenButton  'intervalBetweenButton
		Point locationSelectReport = new Point(12, 418); //グループボックス表示開始位置[出力帳票]  'LocationSelectReport  'LocationSelectReport
		Size sizeSelectReportTeiki = new Size(386, 226); //グループボックス表示サイズ[出力帳票]定期観光  'SizeSelectReportTeiki  'SizeSelectReportTeiki
		Size sizeSelectReportKikaku = new Size(386, 173); //グループボックス表示サイズ[出力帳票]企画旅行  'SizeSelectReportKikaku  'SizeSelectReportKikaku
		Point locationOutputFormat = new Point(413, 418); //グループボックス表示開始位置[出力形式]  'LocationOutputFormat  'LocationOutputFormat
		Point locationOutputSingleOrAll = new Point(413, 524); //グループボックス表示開始位置[出力単位(PDF)]  'LocationOutputSingleOrAll  'LocationOutputSingleOrAll
															   //--- 2012/08/03 MOD END BY.FUJIMOTO ---

		//■ベースフォームの設定
		//HeaderTitle

		#region [StartupOrgProc]メソッド使用のためコメント
		#endregion

		//Location_X
		this.F10Key_Location_X = this.Size.Width - (this.F2Key_Location_X + bufferFromLightEnd + this.F10Key_Width);
		this.F6Key_Location_X = this.F10Key_Location_X - (this.F6Key_Width + intervalBetweenButton);


		//■配置コントロールの設定
		//初期値
		this.rdoOutKeisiki_Excel.Enabled = true;
		this.rdoOutKeisiki_Paper.Checked = true;
		this.rdoOutKeisikiPDF_CrsEvery.Checked = true;

		//所属別の表示項目
		if (this._processMode == Form_RegularNum.Mode.teiki)
		{
			this.gbxOutForm_KikakuTravel.Visible = false;
			this.gbxOutForm_TeikiKanko.Visible = true;
			this.gbxOutForm_TeikiKanko.Location = new System.Drawing.Point(12, 342);

			//Me.gbxOutForm_TeikiKanko.Location = locationSelectReport
			//Me.gbxOutForm_TeikiKanko.Size = sizeSelectReportTeiki
			this.gbxOutKeisiki.Visible = true;
			//Me.gbxOutKeisiki.Location = locationOutputFormat
			this.gbxOutKeisiki_PDF.Visible = true;
			//Me.gbxOutKeisiki_PDF.Location = locationOutputSingleOrAll

			if (this._bfGamen == BfGamenType.siireInfo)
			{
				this.chkTeiki_TateTable.Enabled = false;
				this.chkTeiki_Rosenzu.Enabled = false;
				this.chkTeiki_RootTable.Enabled = false;
				this.chkTeiki_SiireSetConfirmation.Enabled = true;
				this.chkTeiki_CostTable_ChargeConstitutionTable.Enabled = false; //
				this.chkTeiki_Toriatukai.Enabled = false;
				this.chkTeiki_CrsNaiyoKakuninList.Enabled = false;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkTeiki_TravelGuide.Enabled = false;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---
				this.rdoOutKeisiki_Excel.Enabled = false;
			}
		}
		else if (this._processMode == Form_RegularNum.Mode.kikaku)
		{
			this.gbxOutForm_TeikiKanko.Visible = false;
			this.gbxOutForm_KikakuTravel.Visible = true;
			this.gbxOutForm_KikakuTravel.Location = new System.Drawing.Point(12, 342);

			//Me.gbxOutForm_KikakuTravel.Location = locationSelectReport
			//Me.gbxOutForm_KikakuTravel.Size = sizeSelectReportKikaku
			this.gbxOutKeisiki.Visible = true;
			//Me.gbxOutKeisiki.Location = locationOutputFormat
			this.gbxOutKeisiki_PDF.Visible = true;
			//Me.gbxOutKeisiki_PDF.Location = locationOutputSingleOrAll

			if (this._bfGamen == BfGamenType.siireInfo)
			{
				this.chkKikaku_TateTable.Enabled = false;
				this.chkKikaku_SiireSetConfirmation.Enabled = true;
				this.chkKikaku_CostTable.Enabled = false;
				this.chkKikaku_Toriatukai.Enabled = false;
				this.chkKikaku_CrsNaiyoKakuninList.Enabled = false;
				//--- 2012/08/03 ADD START BY.FUJIMOTO ---
				this.chkKikaku_TravelGuide.Enabled = false;
				//--- 2012/08/03 ADD END BY.FUJIMOTO ---
				this.rdoOutKeisiki_Excel.Enabled = false;
			}
		}
		else if (this._processMode == Form_RegularNum.Mode.processAuthorityNone)
		{
			//使用権限がない場合
			this.gbxOutForm_TeikiKanko.Visible = false;
			this.gbxOutForm_KikakuTravel.Visible = false;
			this.gbxOutKeisiki.Visible = false;
			this.gbxOutKeisiki_PDF.Visible = false;
			this.F6Key_Enabled = false;
			this.F7Key_Enabled = false;
		}

	}

	/// <summary>
	/// コース一覧初期表示
	/// </summary>
	/// <remarks></remarks>
	private void setCourseList()
	{

		DataTable dtFormSelection = new DataTable(); //dt帳票選択  'dt帳票選択

		dtFormSelection.Columns.Add("ROW_NO");
		dtFormSelection.Columns.Add("CRS_CD");
		dtFormSelection.Columns.Add("CRS_YEAR");
		dtFormSelection.Columns.Add("SEASON_DISP");
		dtFormSelection.Columns.Add("CRS_NAME");


		for (int row = 0; row <= (_taisyoCrsInfo.Length - 1); row++)
		{
			DataRow drCrsInfo = dtFormSelection.NewRow; //drコース情報  'drコース情報

			drCrsInfo["ROW_NO"] = (row + 1).ToString();
			drCrsInfo["CRS_CD"] = _taisyoCrsInfo[row].CrsCd;
			drCrsInfo["CRS_YEAR"] = _taisyoCrsInfo[row].Year;
			drCrsInfo["SEASON_DISP"] = _taisyoCrsInfo[row].Season_DisplayFor;
			drCrsInfo["CRS_NAME"] = _taisyoCrsInfo[row].CrsName;

			dtFormSelection.Rows.Add(drCrsInfo);
		}

		this.grdFormSelection.DataSource = dtFormSelection;
	}

	#endregion

	#region 共通処理

	/// <summary>
	/// ラジオボタンの選択状態を取得
	/// </summary>
	/// <remarks></remarks>
	private FormProcessType getReportProcessType()
	{
		FormProcessType returnValue = FormProcessType.paper; //returnValue  'returnValue

		//Select Case True
		//    Case Me.rdoOutKeisiki_Paper.Checked
		//        returnValue = FormProcessType.paper
		//    Case Me.rdoOutKeisikiPDF_CrsEvery.Checked
		//        returnValue = FormProcessType.pDF_CrsEvery
		//    Case Me.rdoOutKeisikiPDF_AllCrs.Checked
		//        returnValue = FormProcessType.pDF_AllCrs
		//End Select
		// ↓ (ph2) 判定項目見直し
		if (this.rdoOutKeisiki_Paper.Checked)
		{
			return FormProcessType.paper;
		}
		else
		{
			// [紙]以外
			if (this.rdoOutKeisikiPDF_CrsEvery.Checked)
			{
				return FormProcessType.pDF_CrsEvery;
			}
			else if (this.rdoOutKeisikiPDF_AllCrs.Checked)
			{
				return FormProcessType.pDF_AllCrs;
			}
		}

		return returnValue;
	}

	/// <summary>
	/// 出力対象の帳票を配列に取得
	/// </summary>
	/// <returns>出力対象の帳票</returns>
	/// <remarks></remarks>
	private SortedList getTargetReportName()
	{
		SortedList returnValue = new SortedList(); //returnValue  'returnValue
		GroupBox targetGroupBox = this.gbxOutForm_TeikiKanko; //targetGroupBox
		int idx = 0; //idx  'idx
		CheckBoxEx targetControl = null; //targetControl  'targetControl

		if (this.gbxOutForm_TeikiKanko.Visible == true)
		{
			targetGroupBox = this.gbxOutForm_TeikiKanko;
		}
		else if (this.gbxOutForm_KikakuTravel.Visible == true)
		{
			targetGroupBox = this.gbxOutForm_KikakuTravel;
		}

		//チェックされているコントロール名をタブインデックス順に取得
		while (idx < targetGroupBox.Controls.Count)
		{
			//If targetGroupBox.Controls(idx).GetType().ToString() = "hatobus.NextGenerationSystem.Common_UI.CheckBoxEx" Then
			targetControl = (CheckBoxEx)(targetGroupBox.Controls(idx));

			if (targetControl.Checked == true)
			{
				returnValue.Add(targetControl.TabIndex, targetControl.Name);
			}
			//End If
			idx++;
		}

		return returnValue;
	}

	#endregion

	#region プレビュー処理

	/// <summary>
	/// 定期観光時のプレビュー処理
	/// </summary>
	/// <remarks></remarks>
	private void showPreviewTeiki()
	{

		TateTable_Higaeri tateTable_Higaeri = new TateTable_Higaeri(); //たて表_日帰り  'たて表_日帰り
																	   // 企画のみ Dim tateTable_Stay As New TateTable_Stay  'たて表_宿泊  'たて表_宿泊
		Rosenzu rosenzu = new Rosenzu(); //路線図  '路線図
		SiireSetConfirmation siireSetConfirmation = new SiireSetConfirmation(); //仕入設定確認書  '仕入設定確認書
		ChargeConstitutionTable chargeConstitutionTable = new ChargeConstitutionTable(); //料金構成表  '  '料金構成表
																						 // 企画のみ Dim costTable_Higaeri As New CostTable_Higaeri  '原価表_日帰り  '原価表_日帰り

		P01_0312 crsNaiyoKakuninList = new P01_0312(); //コース内容確認リスト
													   //--- 2012/08/03 ADD START BY.FUJIMOTO ---
		TravelGuide travelGuide = new TravelGuide(); //ご旅行案内  'ご旅行案内
													 //--- 2012/08/03 ADD END BY.FUJIMOTO ---

		bool outputPreview = false; //True:出力有り / False:出力なし  'outputPreview  'outputPreview

		//たて表の出力
		if (this.chkTeiki_TateTable.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			tateTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Higaeri.BfGamen = this.BfGamen;
			tateTable_Higaeri.FormProcessKind = this.FormProcessKind;
			tateTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			tateTable_Higaeri.showPreview(this._processMode);
			this._previewList.Add(tateTable_Higaeri);

			if (tateTable_Higaeri.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}

			// 企画のみ
			//With TateTable_Stay
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    '対象帳票の呼出
			//    .showPreview()
			//    Me._previewList.Add(TateTable_Stay)

			//    If .ProcessResult <> ProcessResultType.taisyoDataNasi Then
			//        outputPreview = True
			//    End If
			//End With
		}

		//路線図の出力
		if (this.chkTeiki_Rosenzu.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			rosenzu.TaisyoCrsInfo = this.TaisyoCrsInfo;
			rosenzu.BfGamen = this.BfGamen;
			rosenzu.FormProcessKind = this.FormProcessKind;
			rosenzu.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			rosenzu.showPreview();
			this._previewList.Add(rosenzu);

			if (rosenzu.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//仕入設定確認書の出力
		if (this.chkTeiki_SiireSetConfirmation.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			siireSetConfirmation.TaisyoCrsInfo = this.TaisyoCrsInfo;
			siireSetConfirmation.BfGamen = this.BfGamen;
			siireSetConfirmation.FormProcessKind = this.FormProcessKind;
			siireSetConfirmation.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			siireSetConfirmation.showPreview();
			this._previewList.Add(siireSetConfirmation);

			if (siireSetConfirmation.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//料金構成表の出力
		if (this.chkTeiki_CostTable_ChargeConstitutionTable.Checked == true) //
		{
			//呼出先に画面連携情報を受渡
			chargeConstitutionTable.TaisyoCrsInfo = this.TaisyoCrsInfo;
			chargeConstitutionTable.BfGamen = this.BfGamen;
			chargeConstitutionTable.FormProcessKind = this.FormProcessKind;
			chargeConstitutionTable.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			chargeConstitutionTable.showPreview();
			this._previewList.Add(chargeConstitutionTable); //

			if (chargeConstitutionTable.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}

			// 企画のみ
			//With costTable_Higaeri
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    '対象帳票の呼出
			//    .showPreview()
			//    Me._previewList.Add(costTable_Higaeri)

			//    If .ProcessResult <> ProcessResultType.taisyoDataNasi Then
			//        outputPreview = True
			//    End If
			//End With
		}

		//コース内容確認リストの出力
		if (this.chkTeiki_CrsNaiyoKakuninList.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			crsNaiyoKakuninList.TaisyoCrsInfo = this.TaisyoCrsInfo;
			crsNaiyoKakuninList.BfGamen = this.BfGamen;
			crsNaiyoKakuninList.FormProcessKind = this.FormProcessKind;
			crsNaiyoKakuninList.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			crsNaiyoKakuninList.showPreview();
			this._previewList.Add(crsNaiyoKakuninList);

			if (crsNaiyoKakuninList.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//--- 2012/08/03 ADD START BY.FUJIMOTO ---
		//ご旅行案内の出力
		if (this.chkTeiki_TravelGuide.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			travelGuide.TaisyoCrsInfo = this.TaisyoCrsInfo;
			travelGuide.BfGamen = this.BfGamen;
			travelGuide.FormProcessKind = this.FormProcessKind;
			travelGuide.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			travelGuide.showPreview();
			this._previewList.Add(travelGuide);

			if (travelGuide.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}
		//--- 2012/08/03 ADD END BY.FUJIMOTO ---

		if (outputPreview == false)
		{
			//完了メッセージ
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0007")
			createFactoryMsg.messageDisp("0007");
		}

	}

	/// <summary>
	/// 企画旅行時のプレビュー処理
	/// </summary>
	/// <remarks></remarks>
	private void showPreviewKikaku()
	{

		TateTable_Higaeri tateTable_Higaeri = new TateTable_Higaeri(); //たて表_日帰り  'たて表_日帰り
		TateTable_Stay tateTable_Stay = new TateTable_Stay(); //たて表_宿泊  'たて表_宿泊
		SiireSetConfirmation siireSetConfirmation = new SiireSetConfirmation(); //仕入設定確認書  '仕入設定確認書
																				//Dim costTable_Higaeri As New CostTable_Higaeri  '原価表_日帰り  '原価表_日帰り
		CostList_Higaeri costTable_Higaeri = new CostList_Higaeri(); //原価表_日帰り  '原価表_日帰り


		P01_0312 crsNaiyoKakuninList = new P01_0312(); //コース内容確認リスト
													   //'--- 2012/08/03 ADD START BY.FUJIMOTO ---
		TravelGuide travelGuide = new TravelGuide(); //ご旅行案内  'ご旅行案内
													 //'--- 2012/08/03 ADD END BY.FUJIMOTO ---

		bool outputPreview = false; //True:出力有り / False:出力なし  'outputPreview  'outputPreview

		//たて表の出力
		if (this.chkKikaku_TateTable.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			tateTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Higaeri.BfGamen = this.BfGamen;
			tateTable_Higaeri.FormProcessKind = this.FormProcessKind;
			tateTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			tateTable_Higaeri.showPreview(this._processMode);
			this._previewList.Add(tateTable_Higaeri);

			if (tateTable_Higaeri.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}

			//呼出先に画面連携情報を受渡
			tateTable_Stay.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Stay.BfGamen = this.BfGamen;
			tateTable_Stay.FormProcessKind = this.FormProcessKind;
			tateTable_Stay.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			tateTable_Stay.showPreview();
			this._previewList.Add(tateTable_Stay);

			if (tateTable_Stay.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//仕入設定確認書の出力
		if (this.chkKikaku_SiireSetConfirmation.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			siireSetConfirmation.TaisyoCrsInfo = this.TaisyoCrsInfo;
			siireSetConfirmation.BfGamen = this.BfGamen;
			siireSetConfirmation.FormProcessKind = this.FormProcessKind;
			siireSetConfirmation.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			siireSetConfirmation.showPreview();
			this._previewList.Add(siireSetConfirmation);

			if (siireSetConfirmation.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//原価表の出力
		if (this.chkKikaku_CostTable.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			costTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
			costTable_Higaeri.BfGamen = this.BfGamen;
			costTable_Higaeri.FormProcessKind = this.FormProcessKind;
			costTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			costTable_Higaeri.showPreview();
			this._previewList.Add(costTable_Higaeri);

			if (costTable_Higaeri.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//コース内容確認リストの出力
		if (this.chkKikaku_CrsNaiyoKakuninList.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			crsNaiyoKakuninList.TaisyoCrsInfo = this.TaisyoCrsInfo;
			crsNaiyoKakuninList.BfGamen = this.BfGamen;
			crsNaiyoKakuninList.FormProcessKind = this.FormProcessKind;
			crsNaiyoKakuninList.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			crsNaiyoKakuninList.showPreview();
			this._previewList.Add(crsNaiyoKakuninList);

			if (crsNaiyoKakuninList.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}

		//--- 2012/08/03 ADD START BY.FUJIMOTO ---
		//ご旅行案内の出力
		if (this.chkKikaku_TravelGuide.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			travelGuide.TaisyoCrsInfo = this.TaisyoCrsInfo;
			travelGuide.BfGamen = this.BfGamen;
			travelGuide.FormProcessKind = this.FormProcessKind;
			travelGuide.CrsMasterEntity = this.CrsMasterEntity;
			//対象帳票の呼出
			travelGuide.showPreview();
			this._previewList.Add(travelGuide);

			if (travelGuide.ProcessResult != ProcessResultType.taisyoDataNasi)
			{
				outputPreview = true;
			}
		}
		//--- 2012/08/03 ADD END BY.FUJIMOTO ---

		if (outputPreview == false)
		{
			//完了メッセージ
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0007")
			// 該当データが存在しません。
			createFactoryMsg.messageDisp("0007");
		}

	}

	#endregion

	#region 出力処理
	/// <summary>
	/// 定期観光時の出力処理
	/// </summary>
	/// <remarks></remarks>
	private void outputReportTeiki()
	{
		TateTable_Higaeri tateTable_Higaeri = new TateTable_Higaeri(); //たて表_日帰り  'たて表_日帰り
																	   // 企画のみ Dim tateTable_Stay As New TateTable_Stay  'たて表_宿泊  'たて表_宿泊
		Rosenzu rosenzu = new Rosenzu(); //路線図  '路線図
										 // (ph2) Excel化のためコメント Dim clsRootTable_CSVOut As New RootTable_CSVOut  'clsルート表_CSV出力  'clsルート表_CSV出力
		SiireSetConfirmation siireSetConfirmation = new SiireSetConfirmation(); //仕入設定確認書  '仕入設定確認書
		ChargeConstitutionTable chargeConstitutionTable = new ChargeConstitutionTable(); //料金構成表  '  '料金構成表
																						 // 企画のみ Dim costTable_Higaeri As New CostTable_Higaeri  '原価表_日帰り  '原価表_日帰り
																						 // (ph2) Excel化のためコメント Dim clsCostTable_Stay_CSVOut As New CostTable_Stay_CSVOut  'cls原価表_宿泊_CSV出力  'cls原価表_宿泊_CSV出力
																						 // (ph2) Excel化のためコメント Dim clsToriatukai_Higaeri_CSVOut As New Toriatukai_Higaeri_CSVOut  'cls取扱_日帰り_CSV出力  'cls取扱_日帰り_CSV出力
																						 // (ph2) Excel化のためコメント Dim clsToriatukai_Stay_CSVOut As New Toriatukai_Stay_CSVOut  'cls取扱_宿泊_CSV出力  'cls取扱_宿泊_CSV出力
		P01_0312 clsCrsNaiyoKakuninList = new P01_0312(); //コース内容確認リスト
														  //'--- 2012/08/03 ADD START BY.FUJIMOTO ---
		TravelGuide travelGuide = new TravelGuide(); //ご旅行案内  'ご旅行案内
													 //'--- 2012/08/03 ADD END BY.FUJIMOTO ---

		// Excelテンプレートフォルダ
		string excelTemplateFolder = System.Convert.ToString(this.txtExcelTemplateFolder.Text);

		string crsKind2Name = System.Convert.ToString(getEnumAttrValue(CrsKind2.higaeri));
		if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.TaisyoCrsInfo[0].CrsKind2)) == false)
		{
			getEnumAttrValue(Enum.Parse(typeof(CrsKind2), System.Convert.ToInt32(this.TaisyoCrsInfo[0].CrsKind2)));
		}

		string msgBuf = ""; //msgBuf  'msgBuf

		if (this.FormProcessKind != FormProcessType.paper)
		{
			this.OutSakiFolder = System.Convert.ToString(this.txtPDFOutSaki.Text);
			if (this.OutSakiFolder.Substring(this.OutSakiFolder.Length - 1, 1) != "\\")
			{
				this.OutSakiFolder += "\\";
			}
		}

		//たて表の出力
		if (this.chkTeiki_TateTable.Checked == true)
		{
			ProcessResultType tateTableOutResult = null; //たて表出力結果  'たて表出力結果

			//呼出先に画面連携情報を受渡
			tateTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Higaeri.BfGamen = this.BfGamen;
			tateTable_Higaeri.FormProcessKind = this.FormProcessKind;
			tateTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
			tateTable_Higaeri.OutSakiFolder = this.OutSakiFolder;
			tateTable_Higaeri.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.TateTable_TeikiKanko_RHigaeri)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			tateTable_Higaeri.outputReport(this._processMode);

			// 企画のみ
			//'呼出先に画面連携情報を受渡
			//With tateTable_Stay
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    .OutFile = String.Format(Form_RegularNum.File_AllCrs.TateTable_RStay_KikakuStay _
			//                                        , getEnumAttrValue(Me._processMode) _
			//                                        , DateTime.Now.ToString("yyyyMMddHHmm"))
			//    '対象帳票の呼出
			//    .outputReport()
			//End With
			//If tateTable_Higaeri.ProcessResult = ProcessResultType.outError OrElse tateTable_Stay.ProcessResult = ProcessResultType.outError Then
			//    tateTableOutResult = ProcessResultType.outError
			//ElseIf tateTable_Higaeri.ProcessResult = ProcessResultType.taisyoDataNasi AndAlso tateTable_Stay.ProcessResult = ProcessResultType.taisyoDataNasi Then
			//    tateTableOutResult = ProcessResultType.taisyoDataNasi
			//ElseIf tateTable_Higaeri.ProcessResult = ProcessResultType.outCancel AndAlso tateTable_Stay.ProcessResult = ProcessResultType.outCancel Then
			//    tateTableOutResult = ProcessResultType.outCancel
			//ElseIf tateTable_Higaeri.ProcessResult = ProcessResultType.outCancel AndAlso tateTable_Stay.ProcessResult = ProcessResultType.taisyoDataNasi Then
			//    tateTableOutResult = ProcessResultType.outCancel
			//ElseIf tateTable_Higaeri.ProcessResult = ProcessResultType.taisyoDataNasi AndAlso tateTable_Stay.ProcessResult = ProcessResultType.outCancel Then
			//    tateTableOutResult = ProcessResultType.outCancel
			//Else
			//    tateTableOutResult = ProcessResultType.outOK
			//End If

			tateTableOutResult = tateTable_Higaeri.ProcessResult;
			msgBuf += "\r\n" + "たて表　－－－＞　" + getEnumAttrValue(tateTableOutResult);
		}

		//路線図の出力
		if (this.chkTeiki_Rosenzu.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			rosenzu.TaisyoCrsInfo = this.TaisyoCrsInfo;
			rosenzu.BfGamen = this.BfGamen;
			rosenzu.FormProcessKind = this.FormProcessKind;
			rosenzu.CrsMasterEntity = this.CrsMasterEntity;
			rosenzu.OutSakiFolder = this.OutSakiFolder;
			rosenzu.OutFile = string.Format(Form_RegularNum.File_AllCrs.Rosenzu_TeikiKanko_RHigaeri, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			rosenzu.outputReport();

			msgBuf += "\r\n" + "路線図　－－－＞　" + getEnumAttrValue(rosenzu.ProcessResult);
		}

		//ルート表の出力
		if (this.chkTeiki_RootTable.Checked == true)
		{
			//With clsRootTable_CSVOut
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    '対象帳票の呼出
			//    .OutputCSV()
			//End With
			//msgBuf &= vbCrLf & "ルート表　－－－＞　" & getEnumAttrValue(clsRootTable_CSVOut.ProcessResult)

			// ↓ (ph2) Excel化
			//--------------------------------------------------
			// 選択されているコースに[年・季]が混在していることを考慮
			// 出力に合わせて[年]・[季] で並び替えする
			//--------------------------------------------------
			// 一旦、選択されている全コースを退避
			FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfo_Org = this.TaisyoCrsInfo;

			FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfoRouteTable = null;

			// [年]・[季]・[コースコード]で昇順に並び替えする
			this.TaisyoCrsInfo = (From n in taisyoCrsInfo_Org;
			Order(By n.Year Ascending, n.Season Ascending, n.CrsCd Ascending).ToArray());

			ProcessResultType routeTableOutResult = null;
			makeExcelFile(PrintExcelKinds.RouteTable, excelTemplateFolder, ref routeTableOutResult);

			msgBuf += "\r\n" + "ルート表　－－－＞　" + getEnumAttrValue(routeTableOutResult);


			// 選択されている全コースに戻す
			this.TaisyoCrsInfo = taisyoCrsInfo_Org;

		}

		//仕入設定確認書の出力
		//※出力ファイル名は仕入れの内部で設定する
		if (this.chkTeiki_SiireSetConfirmation.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			siireSetConfirmation.TaisyoCrsInfo = this.TaisyoCrsInfo;
			siireSetConfirmation.BfGamen = this.BfGamen;
			siireSetConfirmation.FormProcessKind = this.FormProcessKind;
			siireSetConfirmation.CrsMasterEntity = this.CrsMasterEntity;
			siireSetConfirmation.OutSakiFolder = this.OutSakiFolder;
			//ファイル名は指定がなければ基底クラスで設定

			//対象帳票の呼出
			siireSetConfirmation.outputReport();

			msgBuf += "\r\n" + "仕入設定確認書　－－－＞　" + getEnumAttrValue(siireSetConfirmation.ProcessResult);
		}

		//料金構成表の出力
		if (this.chkTeiki_CostTable_ChargeConstitutionTable.Checked == true) //
		{
			if (rdoOutKeisiki_Paper.Checked == true || rdoOutKeisiki_PDF.Checked == true)
			{
				//******************************************
				// 料金構成表印刷
				//******************************************
				//呼出先に画面連携情報を受渡
				chargeConstitutionTable.TaisyoCrsInfo = this.TaisyoCrsInfo;
				chargeConstitutionTable.BfGamen = this.BfGamen;
				chargeConstitutionTable.FormProcessKind = this.FormProcessKind;
				chargeConstitutionTable.CrsMasterEntity = this.CrsMasterEntity;
				chargeConstitutionTable.OutSakiFolder = this.OutSakiFolder;
				chargeConstitutionTable.OutFile = string.Format(Form_RegularNum.File_AllCrs.CostTable_TeikiKanko, DateTime.Now.ToString("yyyyMMddHHmm"));
				//対象帳票の呼出
				chargeConstitutionTable.outputReport();
				msgBuf += "\r\n" + "料金構成表　－－－＞　" + getEnumAttrValue(chargeConstitutionTable.ProcessResult);

				//'******************************************
				//' 原価表（日帰り）印刷
				//'******************************************
				//With costTable_Higaeri
				//    '呼出先に画面連携情報を受渡
				//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
				//    .BfGamen = Me.BfGamen
				//    .FormProcessKind = Me.FormProcessKind
				//    .CrsMasterEntity = Me.CrsMasterEntity
				//    .OutSakiFolder = Me.OutSakiFolder
				//    .OutFile = String.Format(Form_RegularNum.File_AllCrs.CostTable_RCrsHigaeri_ikakuHigaeri _
				//                                        , getEnumAttrValue(Me._processMode) _
				//                                        , crsKind2Name _
				//                                        , DateTime.Now.ToString("yyyyMMddHHmm"))
				//    '対象帳票の呼出
				//    .outputReport()
				//    msgBuf &= vbCrLf & "原価表　－－－＞　" & getEnumAttrValue(.ProcessResult)
				//End With
			}
			else
			{
				// TODO:naga 定期不要
				//'******************************************
				//' 原価表（宿泊）CSV出力
				//'******************************************
				//With clsCostTable_Stay_CSVOut
				//    '呼出先に画面連携情報を受渡
				//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
				//    .BfGamen = Me.BfGamen
				//    .FormProcessKind = Me.FormProcessKind
				//    .CrsMasterEntity = Me.CrsMasterEntity
				//    .OutSakiFolder = Me.OutSakiFolder
				//    '対象帳票の呼出
				//    .OutputCSV()
				//    msgBuf &= vbCrLf & "原価表　－－－＞　" & getEnumAttrValue(.ProcessResult)
				//End With
			}
		}

		//取扱の出力
		if (this.chkTeiki_Toriatukai.Checked == true)
		{
			ProcessResultType toriatukaiOutResult = null; //取扱出力結果  '取扱出力結果

			//With clsToriatukai_Higaeri_CSVOut
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    '対象帳票の呼出
			//    .OutputCSV()
			//End With

			//With clsToriatukai_Stay_CSVOut
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    '対象帳票の呼出
			//    .OutputCSV()
			//End With

			//If clsToriatukai_Higaeri_CSVOut.ProcessResult = ProcessResultType.outError OrElse clsToriatukai_Stay_CSVOut.ProcessResult = ProcessResultType.outError Then
			//    toriatukaiOutResult = ProcessResultType.outError
			//ElseIf clsToriatukai_Higaeri_CSVOut.ProcessResult = ProcessResultType.taisyoDataNasi AndAlso clsToriatukai_Stay_CSVOut.ProcessResult = ProcessResultType.taisyoDataNasi Then
			//    toriatukaiOutResult = ProcessResultType.taisyoDataNasi
			//Else
			//    toriatukaiOutResult = ProcessResultType.outOK
			//End If
			//msgBuf &= vbCrLf & "取扱　－－－＞　" & getEnumAttrValue(toriatukaiOutResult)
			// ↓ (ph2) Excel化
			makeExcelFile(PrintExcelKinds.Toriatukai_Higaeri_Teiki, excelTemplateFolder, ref toriatukaiOutResult);

			msgBuf += "\r\n" + "取扱　－－－＞　" + getEnumAttrValue(toriatukaiOutResult);

		}

		//コース内容確認リストの出力
		if (this.chkTeiki_CrsNaiyoKakuninList.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			clsCrsNaiyoKakuninList.TaisyoCrsInfo = this.TaisyoCrsInfo;
			clsCrsNaiyoKakuninList.BfGamen = this.BfGamen;
			clsCrsNaiyoKakuninList.FormProcessKind = this.FormProcessKind;
			clsCrsNaiyoKakuninList.CrsMasterEntity = this.CrsMasterEntity;
			clsCrsNaiyoKakuninList.OutSakiFolder = this.OutSakiFolder;
			clsCrsNaiyoKakuninList.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.CrsNaiyoKakuninList)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			clsCrsNaiyoKakuninList.outputReport();
			msgBuf += "\r\n" + "コース内容確認リスト　－－－＞　" + getEnumAttrValue(clsCrsNaiyoKakuninList.ProcessResult);
		}

		//--- 2012/08/03 ADD START BY.FUJIMOTO ---
		//ご旅行案内の出力
		if (this.chkTeiki_TravelGuide.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			travelGuide.TaisyoCrsInfo = this.TaisyoCrsInfo;
			travelGuide.BfGamen = this.BfGamen;
			travelGuide.FormProcessKind = this.FormProcessKind;
			travelGuide.CrsMasterEntity = this.CrsMasterEntity;
			travelGuide.OutSakiFolder = this.OutSakiFolder;
			travelGuide.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.TravelGuide)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			travelGuide.outputReport();
			msgBuf += "\r\n" + "ご旅行案内　－－－＞　" + getEnumAttrValue(travelGuide.ProcessResult);
		}
		//--- 2012/08/03 ADD END BY.FUJIMOTO ---

		// 出力先フォルダ保存 (Settings使用)
		this.saveOutputFoler(System.Convert.ToString(this.txtPDFOutSaki.Text));
		// Excelテンプレートフォルダ保存 (Settings使用)
		this.saveExcelTemplateFolder(System.Convert.ToString(this.txtExcelTemplateFolder.Text));

		//完了メッセージ
		//TODO:共通変更対応
		//メッセージ出力.messageDisp("0062", msgBuf)
		createFactoryMsg.messageDisp("0062", msgBuf);

	}

	/// <summary>
	/// 企画旅行時の出力処理
	/// </summary>
	/// <remarks></remarks>
	private void outputReportKikaku()
	{

		TateTable_Higaeri tateTable_Higaeri = new TateTable_Higaeri(); //たて表_日帰り  'たて表_日帰り
		TateTable_Stay tateTable_Stay = new TateTable_Stay(); //たて表_宿泊  'たて表_宿泊
		SiireSetConfirmation siireSetConfirmation = new SiireSetConfirmation(); //仕入設定確認書  '仕入設定確認書
																				//Dim costTable_Higaeri As New CostTable_Higaeri  '原価表_日帰り  '原価表_日帰り
		CostList_Higaeri costTable_Higaeri = new CostList_Higaeri(); //原価表_日帰り  '原価表_日帰り
																	 // (ph2) Excel化のためコメント Dim clsCostTable_Stay_CSVOut As New CostTable_Stay_CSVOut  'cls原価表_宿泊_CSV出力  'cls原価表_宿泊_CSV出力
																	 // (ph2) Excel化のためコメント Dim clsToriatukai_Higaeri_CSVOut As New Toriatukai_Higaeri_CSVOut  'cls取扱_日帰り_CSV出力  'cls取扱_日帰り_CSV出力
																	 // (ph2) Excel化のためコメント Dim clsToriatukai_Stay_CSVOut As New Toriatukai_Stay_CSVOut  'cls取扱_宿泊_CSV出力  'cls取扱_宿泊_CSV出力
		P01_0312 clsCrsNaiyoKakuninList = new P01_0312(); //コース内容確認リスト
														  //'--- 2012/08/03 ADD START BY.FUJIMOTO ---
		TravelGuide travelGuide = new TravelGuide(); //ご旅行案内  'ご旅行案内
													 //'--- 2012/08/03 ADD END BY.FUJIMOTO ---

		// Excelテンプレートフォルダ
		string excelTemplateFolder = System.Convert.ToString(this.txtExcelTemplateFolder.Text);

		string msgBuf = ""; //msgBuf  'msgBuf

		string crsKind2Name = System.Convert.ToString(getEnumAttrValue(Enum.Parse(typeof(CrsKind2), System.Convert.ToInt32(this.TaisyoCrsInfo[0].CrsKind2))));

		if (this.FormProcessKind != FormProcessType.paper)
		{
			if (this.OutSakiFolder.Substring(this.OutSakiFolder.Length - 1, 1) != "\\")
			{
				this.OutSakiFolder += "\\";
			}
		}

		//たて表の出力
		if (this.chkKikaku_TateTable.Checked == true)
		{
			ProcessResultType tateTableOutResult = null; //たて表出力結果  'たて表出力結果

			//呼出先に画面連携情報を受渡
			tateTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Higaeri.BfGamen = this.BfGamen;
			tateTable_Higaeri.FormProcessKind = this.FormProcessKind;
			tateTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
			tateTable_Higaeri.OutSakiFolder = this.OutSakiFolder;
			tateTable_Higaeri.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.TateTable_KikakuHigaeri)
				, getEnumAttrValue(this._processMode)
				, crsKind2Name, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			tateTable_Higaeri.outputReport(this._processMode);

			//呼出先に画面連携情報を受渡
			tateTable_Stay.TaisyoCrsInfo = this.TaisyoCrsInfo;
			tateTable_Stay.BfGamen = this.BfGamen;
			tateTable_Stay.FormProcessKind = this.FormProcessKind;
			tateTable_Stay.CrsMasterEntity = this.CrsMasterEntity;
			tateTable_Stay.OutSakiFolder = this.OutSakiFolder;
			tateTable_Stay.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.TateTable_RStay_KikakuStay)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			tateTable_Stay.outputReport();

			if (tateTable_Higaeri.ProcessResult == ProcessResultType.outError || tateTable_Stay.ProcessResult == ProcessResultType.outError)
			{
				tateTableOutResult = ProcessResultType.outError;
			}
			else if (tateTable_Higaeri.ProcessResult == ProcessResultType.taisyoDataNasi && tateTable_Stay.ProcessResult == ProcessResultType.taisyoDataNasi)
			{
				tateTableOutResult = ProcessResultType.taisyoDataNasi;
			}
			else if (tateTable_Higaeri.ProcessResult == ProcessResultType.outCancel && tateTable_Stay.ProcessResult == ProcessResultType.outCancel)
			{
				tateTableOutResult = ProcessResultType.outCancel;
			}
			else if (tateTable_Higaeri.ProcessResult == ProcessResultType.outCancel && tateTable_Stay.ProcessResult == ProcessResultType.taisyoDataNasi)
			{
				tateTableOutResult = ProcessResultType.outCancel;
			}
			else if (tateTable_Higaeri.ProcessResult == ProcessResultType.taisyoDataNasi && tateTable_Stay.ProcessResult == ProcessResultType.outCancel)
			{
				tateTableOutResult = ProcessResultType.outCancel;
			}
			else
			{
				tateTableOutResult = ProcessResultType.outOK;
			}
			msgBuf += "\r\n" + "たて表　－－－＞　" + getEnumAttrValue(tateTableOutResult);
		}

		//仕入設定確認書の出力
		if (this.chkKikaku_SiireSetConfirmation.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			siireSetConfirmation.TaisyoCrsInfo = this.TaisyoCrsInfo;
			siireSetConfirmation.BfGamen = this.BfGamen;
			siireSetConfirmation.FormProcessKind = this.FormProcessKind;
			siireSetConfirmation.CrsMasterEntity = this.CrsMasterEntity;
			siireSetConfirmation.OutSakiFolder = this.OutSakiFolder;
			//ファイル名は指定がなければ基底クラスで設定

			//対象帳票の呼出
			siireSetConfirmation.outputReport();

			msgBuf += "\r\n" + "仕入設定確認書　－－－＞　" + getEnumAttrValue(siireSetConfirmation.ProcessResult);
		}

		//原価表の出力
		if (this.chkKikaku_CostTable.Checked == true)
		{
			if (rdoOutKeisiki_Paper.Checked == true || rdoOutKeisiki_PDF.Checked == true)
			{
				//******************************************
				// 原価表（日帰り）印刷
				//******************************************
				//呼出先に画面連携情報を受渡
				costTable_Higaeri.TaisyoCrsInfo = this.TaisyoCrsInfo;
				costTable_Higaeri.BfGamen = this.BfGamen;
				costTable_Higaeri.FormProcessKind = this.FormProcessKind;
				costTable_Higaeri.CrsMasterEntity = this.CrsMasterEntity;
				costTable_Higaeri.OutSakiFolder = this.OutSakiFolder;
				costTable_Higaeri.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.CostTable_RCrsHigaeri_ikakuHigaeri)
					, getEnumAttrValue(this._processMode)
					, crsKind2Name, DateTime.Now.ToString("yyyyMMddHHmm"));
				//対象帳票の呼出
				costTable_Higaeri.outputReport();
				msgBuf += "\r\n" + "原価表　－－－＞　" + getEnumAttrValue(costTable_Higaeri.ProcessResult);
			}
			else
			{
				//TODO:naga ph1 は不要 (原価表（宿泊）CSV出力)
				//'******************************************
				//' 原価表（宿泊）CSV出力
				//'******************************************
				//With clsCostTable_Stay_CSVOut
				//    '呼出先に画面連携情報を受渡
				//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
				//    .BfGamen = Me.BfGamen
				//    .FormProcessKind = Me.FormProcessKind
				//    .CrsMasterEntity = Me.CrsMasterEntity
				//    .OutSakiFolder = Me.OutSakiFolder
				//    '対象帳票の呼出
				//    .OutputCSV()
				//    msgBuf &= vbCrLf & "原価表　－－－＞　" & getEnumAttrValue(.ProcessResult)
				//End With

				// ↓ (ph2) Excel化
				//--------------------------------------------------
				// 選択されているコースに[日帰り][宿泊][Rコース]が混在していることを考慮
				// テンプレートに合わせて[宿泊]のみ処理する
				//--------------------------------------------------
				ProcessResultType costTableOutResult = null;

				// 一旦、選択されている全コースを退避
				FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfo_Org = this.TaisyoCrsInfo;
				// [宿泊]のみ抽出する
				FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfoStay = null;
				taisyoCrsInfoStay = taisyoCrsInfo_Org.Where(Function(str) str.CrsKind2.Equals(System.Convert.ToString(CrsKind2.stay))).ToArray();

				if (taisyoCrsInfoStay IsNot null && 0 < taisyoCrsInfoStay.Length)
				{
					// 選択されているコースに[宿泊]あり
					this.TaisyoCrsInfo = taisyoCrsInfoStay;
					makeExcelFile(PrintExcelKinds.CostTable_Stay_kikaku, excelTemplateFolder, ref costTableOutResult);

					// 選択されている全コースに戻す
					this.TaisyoCrsInfo = taisyoCrsInfo_Org;

				}
				else
				{
					// 選択されているコースに[宿泊]なし
					costTableOutResult = ProcessResultType.taisyoDataNasi;
				}

				msgBuf += "\r\n" + "原価表（宿泊）　－－－＞　" + getEnumAttrValue(costTableOutResult);

			}

		}

		//取扱の出力
		if (this.chkKikaku_Toriatukai.Checked == true)
		{
			//Dim toriatukaiOutResult As ProcessResultType  '取扱出力結果  '取扱出力結果
			//With clsToriatukai_Higaeri_CSVOut
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    '対象帳票の呼出
			//    .OutputCSV()
			//End With
			//With clsToriatukai_Stay_CSVOut
			//    '呼出先に画面連携情報を受渡
			//    .TaisyoCrsInfo = Me.TaisyoCrsInfo
			//    .BfGamen = Me.BfGamen
			//    .FormProcessKind = Me.FormProcessKind
			//    .CrsMasterEntity = Me.CrsMasterEntity
			//    .OutSakiFolder = Me.OutSakiFolder
			//    '対象帳票の呼出
			//    .OutputCSV()
			//End With
			//If clsToriatukai_Higaeri_CSVOut.ProcessResult = ProcessResultType.outError OrElse clsToriatukai_Stay_CSVOut.ProcessResult = ProcessResultType.outError Then
			//    toriatukaiOutResult = ProcessResultType.outError
			//ElseIf clsToriatukai_Higaeri_CSVOut.ProcessResult = ProcessResultType.taisyoDataNasi AndAlso clsToriatukai_Stay_CSVOut.ProcessResult = ProcessResultType.taisyoDataNasi Then
			//    toriatukaiOutResult = ProcessResultType.taisyoDataNasi
			//Else
			//    toriatukaiOutResult = ProcessResultType.outOK
			//End If
			//msgBuf &= vbCrLf & "取扱　－－－＞　" & getEnumAttrValue(toriatukaiOutResult)

			// ↓ (ph2) Excel化
			//--------------------------------------------------
			// 選択されているコースに[日帰り][宿泊][Rコース]が混在していることを考慮
			// テンプレートに合わせて[日帰り][Rコース]、[宿泊]を分けて処理する
			//--------------------------------------------------

			// 一旦、選択されている全コースを退避
			FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfo_Org = this.TaisyoCrsInfo;

			//--------------------------------------------------
			// [日帰り][Rコース]を抽出する ※[宿泊]以外を抽出
			//--------------------------------------------------
			ProcessResultType toriatukaiHigaeriOutResult = ProcessResultType.taisyoDataNasi;
			FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfoHigaeri = null;
			taisyoCrsInfoHigaeri = taisyoCrsInfo_Org.Where(Function(str) str.CrsKind2.Equals(System.Convert.ToString(CrsKind2.stay)) == false).ToArray();

			if (taisyoCrsInfoHigaeri IsNot null && 0 < taisyoCrsInfoHigaeri.Length)
			{
				// 選択されているコースに[日帰り][Rコース]あり
				this.TaisyoCrsInfo = taisyoCrsInfoHigaeri;
				makeExcelFile(PrintExcelKinds.Toriatukai_Higaeri_kikaku, excelTemplateFolder, ref toriatukaiHigaeriOutResult);

				// 選択されている全コースに戻す
				this.TaisyoCrsInfo = taisyoCrsInfo_Org;

				msgBuf += "\r\n" + "取扱（日帰り）　－－－＞　" + getEnumAttrValue(toriatukaiHigaeriOutResult);
			}

			//--------------------------------------------------
			// [宿泊]を抽出する
			//--------------------------------------------------
			ProcessResultType toriatukaiStayOutResult = ProcessResultType.taisyoDataNasi;
			FixedCd.CrsMasterKeyKoumoku[] taisyoCrsInfoStay = null;
			taisyoCrsInfoStay = taisyoCrsInfo_Org.Where(Function(str) str.CrsKind2.Equals(System.Convert.ToString(CrsKind2.stay))).ToArray();

			if (taisyoCrsInfoStay IsNot null && 0 < taisyoCrsInfoStay.Length)
			{
				// 選択されているコースに[宿泊]あり
				this.TaisyoCrsInfo = taisyoCrsInfoStay;
				makeExcelFile(PrintExcelKinds.Toriatukai_Stay_kikaku, excelTemplateFolder, ref toriatukaiStayOutResult);

				// 選択されている全コースに戻す
				this.TaisyoCrsInfo = taisyoCrsInfo_Org;

				msgBuf += "\r\n" + "取扱（宿泊）　－－－＞　" + getEnumAttrValue(toriatukaiStayOutResult);
			}

		}

		//コース内容確認リストの出力
		if (this.chkKikaku_CrsNaiyoKakuninList.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			clsCrsNaiyoKakuninList.TaisyoCrsInfo = this.TaisyoCrsInfo;
			clsCrsNaiyoKakuninList.BfGamen = this.BfGamen;
			clsCrsNaiyoKakuninList.FormProcessKind = this.FormProcessKind;
			clsCrsNaiyoKakuninList.CrsMasterEntity = this.CrsMasterEntity;
			clsCrsNaiyoKakuninList.OutSakiFolder = this.OutSakiFolder;
			clsCrsNaiyoKakuninList.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.CrsNaiyoKakuninList)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			clsCrsNaiyoKakuninList.outputReport();
			msgBuf += "\r\n" + "コース内容確認リスト　－－－＞　" + getEnumAttrValue(clsCrsNaiyoKakuninList.ProcessResult);
		}

		//--- 2012/08/03 ADD START BY.FUJIMOTO ---
		//ご旅行案内の出力
		if (this.chkKikaku_TravelGuide.Checked == true)
		{
			//呼出先に画面連携情報を受渡
			travelGuide.TaisyoCrsInfo = this.TaisyoCrsInfo;
			travelGuide.BfGamen = this.BfGamen;
			travelGuide.FormProcessKind = this.FormProcessKind;
			travelGuide.CrsMasterEntity = this.CrsMasterEntity;
			travelGuide.OutSakiFolder = this.OutSakiFolder;
			travelGuide.OutFile = string.Format(System.Convert.ToString(Form_RegularNum.File_AllCrs.TravelGuide)
				, getEnumAttrValue(this._processMode)
				, DateTime.Now.ToString("yyyyMMddHHmm"));
			//対象帳票の呼出
			travelGuide.outputReport();
			msgBuf += "\r\n" + "ご旅行案内　－－－＞　" + getEnumAttrValue(travelGuide.ProcessResult);
		}
		//--- 2012/08/03 ADD END BY.FUJIMOTO ---

		// 出力先フォルダ保存 (Settings使用)
		this.saveOutputFoler(System.Convert.ToString(this.txtPDFOutSaki.Text));
		// Excelテンプレートフォルダ保存 (Settings使用)
		this.saveExcelTemplateFolder(System.Convert.ToString(this.txtExcelTemplateFolder.Text));

		//完了メッセージ
		//TODO:共通変更対応
		//メッセージ出力.messageDisp("0062", msgBuf)
		createFactoryMsg.messageDisp("0062", msgBuf);

	}

	#endregion

	#region 出力先フォルダ
	/// <summary>
	/// 初期表示用出力先フォルダ取得
	/// </summary>
	/// <returns></returns>
	private string getInitOutputFoler()
	{
		string ret = string.Empty;
		SystemSetType systemSetType = null;

		if (rdoOutKeisiki_PDF.Checked)
		{
			ret = System.Convert.ToString(My.Settings.OutputFolderPDF);
			systemSetType = systemSetType.PDFOutputFolderName;
		}
		else if (rdoOutKeisiki_Excel.Checked)
		{
			ret = System.Convert.ToString(My.Settings.OutputFolderExcel);
			systemSetType = systemSetType.ExcelOutputFolderName;
		}
		else
		{
			//処理を抜ける
			return ret;
		}

		if (string.IsNullOrWhiteSpace(ret))
		{
			// コードマスタより[内容１](フォルダ)を取得
			ret = getInitMasterFolder(systemSetType);
		}

		return ret;
	}

	/// <summary>
	/// 出力先フォルダ保存 (Settings使用)
	/// </summary>
	private void saveOutputFoler(string outputFoler)
	{

		if (rdoOutKeisiki_PDF.Checked)
		{
			My.Settings.OutputFolderPDF = outputFoler;
		}
		else if (rdoOutKeisiki_Excel.Checked)
		{
			My.Settings.OutputFolderExcel = outputFoler;
		}
		else
		{
			// 保存するデータがない。処理を抜ける
			return;
		}

		My.Settings.Default.Save();

	}

	/// <summary>
	/// 初期表示用Excelテンプレートフォルダ取得
	/// </summary>
	/// <returns></returns>
	private string getInitExcelTemplateFolder()
	{
		string ret = string.Empty;

		if (rdoOutKeisiki_Excel.Checked == false)
		{
			return ret;
		}

		ret = System.Convert.ToString(My.Settings.ExcelTemplateFolder);

		if (string.IsNullOrWhiteSpace(ret))
		{
			// コードマスタより[内容１](フォルダ)を取得
			ret = getInitMasterFolder(SystemSetType.ExcelTemplateFolderName);
		}

		return ret;
	}

	/// <summary>
	/// Excelテンプレートフォルダ保存 (Settings使用)
	/// </summary>
	/// <param name="excelTemplateFolder"></param>
	private void saveExcelTemplateFolder(string excelTemplateFolder)
	{

		if (rdoOutKeisiki_Excel.Checked == false)
		{
			return;
		}

		My.Settings.ExcelTemplateFolder = excelTemplateFolder;
		My.Settings.Default.Save();

	}

	/// <summary>
	/// コードマスタより[内容１](フォルダ)を取得
	/// </summary>
	/// <param name="systemSetType"></param>
	/// <returns></returns>
	private string getInitMasterFolder(SystemSetType systemSetType)
	{
		string ret = string.Empty;

		CdMasterGet_DA cdMaster = new CdMasterGet_DA();
		CdMasterEntity cdMasterItem = new CdMasterEntity();

		DataTable dt = cdMaster.GetCodeMasterDataSystem(systemSetType);
		if (dt.Rows.Count > 0)
		{
			ret = System.Convert.ToString(dt.Rows(0).Item(cdMasterItem.Naiyo1.PhysicsName).ToString());
		}

		return ret;
	}
	#endregion

	#region フォルダ検索（ダイアログ表示あり）
	/// <summary>
	/// フォルダ検索（ダイアログ表示あり）
	/// </summary>
	/// <param name="text"></param>
	/// <param name="selectedPath"></param>
	/// <returns></returns>
	private DialogResult dialogFolderBrowser(string text, ref string selectedPath)
	{

		FolderBrowserDialog dialogFolder = new FolderBrowserDialog();
		DialogResult dialogResult = dialogResult.Cancel;


		if (string.IsNullOrWhiteSpace(text) || IO.Directory.Exists(text) == false)
		{
			dialogFolder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}
		else
		{
			dialogFolder.SelectedPath = text;
		}

		// [新しいフォルダ]ボタンの表示(初期値 True)
		dialogFolder.ShowNewFolderButton = true;

		// ダイアログを表示し、戻り値が [OK] の場合は、選択したディレクトリを返す
		dialogResult = dialogFolder.ShowDialog();
		if (dialogResult.Equals(dialogResult.OK))
		{
			selectedPath = System.Convert.ToString(dialogFolder.SelectedPath);
		}

		//オブジェクトの破棄
		dialogFolder.Dispose();

		return dialogResult;
	}
	#endregion

	#region Excelテンプレートファイルの存在チェック（処理の前にチェックする）
	/// <summary>
	/// Excelテンプレートファイルの存在チェック（処理の前にチェックする）
	/// </summary>
	/// <param name="excelTemplateFolder"></param>
	/// <param name="notTemplateFile"></param>
	/// <returns></returns>
	private bool checkExistsTemplateExcelFile(string excelTemplateFolder, ref string notTemplateFile)
	{
		bool ret = false;

		// 出力形式がExcel以外は処理を抜ける
		if (rdoOutKeisiki_Excel.Checked == false)
		{
			return true;
		}

		//選択されている帳票テンプレート
		ExcelBase excelBase = new ExcelBase();
		string templateExcelFile = string.Empty;

		if (this._processMode == Form_RegularNum.Mode.teiki)
		{

			// ルート表
			if (chkTeiki_RootTable.Checked == true)
			{
				templateExcelFile = System.Convert.ToString(excelBase.getTemplateExcelFile(PrintExcelKinds.RouteTable, excelTemplateFolder));
				if (IO.File.Exists(templateExcelFile) == false)
				{
					notTemplateFile = templateExcelFile;
					return ret;
				}
			}

			// 取扱(日帰り) 定期
			if (chkTeiki_Toriatukai.Checked == true)
			{
				templateExcelFile = System.Convert.ToString(excelBase.getTemplateExcelFile(PrintExcelKinds.Toriatukai_Higaeri_Teiki, excelTemplateFolder));
				if (IO.File.Exists(templateExcelFile) == false)
				{
					notTemplateFile = templateExcelFile;
					return ret;
				}
			}
		}
		else if (this._processMode == Form_RegularNum.Mode.kikaku)
		{

			// 取扱(日帰り) 企画
			if (chkKikaku_Toriatukai.Checked == true)
			{
				templateExcelFile = System.Convert.ToString(excelBase.getTemplateExcelFile(PrintExcelKinds.Toriatukai_Higaeri_kikaku, excelTemplateFolder));
				if (IO.File.Exists(templateExcelFile) == false)
				{
					notTemplateFile = templateExcelFile;
					return ret;
				}
			}

			// 取扱(宿泊) 企画
			if (chkKikaku_Toriatukai.Checked == true)
			{
				templateExcelFile = System.Convert.ToString(excelBase.getTemplateExcelFile(PrintExcelKinds.Toriatukai_Stay_kikaku, excelTemplateFolder));
				if (IO.File.Exists(templateExcelFile) == false)
				{
					notTemplateFile = templateExcelFile;
					return ret;
				}
			}

			// 原価表(企画_宿泊)
			if (chkKikaku_CostTable.Checked == true)
			{
				templateExcelFile = System.Convert.ToString(excelBase.getTemplateExcelFile(PrintExcelKinds.CostTable_Stay_kikaku, excelTemplateFolder));
				if (IO.File.Exists(templateExcelFile) == false)
				{
					notTemplateFile = templateExcelFile;
					return ret;
				}
			}
		}

		return true;
	}

	#endregion

	#region Excelファイル作成処理
	/// <summary>
	/// Excelファイル作成処理
	/// </summary>
	/// <param name="printExcelKinds"></param>
	/// <param name="excelTemplateFolder"></param>
	/// <param name="processResultType"></param>
	/// <returns></returns>
	private bool makeExcelFile(PrintExcelKinds printExcelKinds, string excelTemplateFolder, ref ProcessResultType processResultType)
	{
		bool ret = false;

		try
		{

			// 出力単位
			FormProcessType outputUnit = FormProcessType.pDF_CrsEvery;
			if (printExcelKinds == printExcelKinds.RouteTable)
			{
				// ルート表
				//  １シートに作成するため全コースとする
				outputUnit = FormProcessType.pDF_AllCrs;
			}
			else
			{
				// 画面.出力単位
				outputUnit = getReportProcessType();
			}


			// Excelファイル作成クラス 生成
			ExcelBase excBase = new ExcelBase();

			// テンプレートファイルの取得
			// ※テンプレートファイルの存在チェックは実施済（ここでは必ず存在する前提）
			string templateExcelFile = System.Convert.ToString(excBase.getTemplateExcelFile(printExcelKinds, excelTemplateFolder));

			//呼出先に画面連携情報を受渡
			excBase.TaisyoCrsInfo = this.TaisyoCrsInfo;
			excBase.BfGamen = this.BfGamen;
			excBase.FormProcessKind = this.FormProcessKind;
			excBase.CrsMasterEntity = this.CrsMasterEntity;
			excBase.OutSakiFolder = this.OutSakiFolder;

			bool retExcel = System.Convert.ToBoolean(excBase.printExcelFile(outputUnit, printExcelKinds, templateExcelFile));

			// 戻り値
			processResultType = excBase.ProcessResult;

			ret = true;

		}
		catch (Exception)
		{
			throw;

		}

		return ret;
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
		F4Key_Visible = false; // F4:未使用
		F5Key_Visible = false; // F5:未使用
		F6Key_Visible = true; // F6:プレビュー
		F7Key_Visible = false; // F7:未使用
		F8Key_Visible = false; // F8:未使用
		F9Key_Visible = false; // F9:未使用
		F10Key_Visible = true; // F10:実行
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用

		//Text
		this.F6Key_Text = "F6:プレビュー";
		this.F10Key_Text = "F10:実行";

	}
	#endregion

}