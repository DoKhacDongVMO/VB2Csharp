using hatobus.DevelopSystem.PrintSelect;
using hatobus.DevelopSystem.NewCommon;
using hatobus.DevelopSystem.PrintCommon;
//using System.Enum;
using Microsoft.VisualBasic.CompilerServices;


/// <summary>
/// パンフ依頼
/// </summary>
/// <remarks>
/// Author:2011/05/12//佐藤(宏)
/// </remarks>
public class S01_0303 //frmパンフ依頼
{
	public S01_0303()
	{
		// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
		crsMasterEntity = new EntityOperation[Of CrsMasterEntity + 1];

	}

	#region  定数／変数

	private const string ProcessResult_Normal = "正常終了"; //処理結果_正常
	private const string ProcessResult_Error = "エラー"; //処理結果_エラー
	private const string ProcessResult_LockChu = "ロック中"; //処理結果_ロック中
	private const string File_Teiki = "定期"; //ファイル名_定期
	private const string File_Kikaku = "企画"; //ファイル名_企画
	private const string Log_KeyCrsCd = "KeyCrsCd"; //ログ_キーコースコード
	private const string Log_CrsCd = "CrsCd"; //ログ_コースコード
	private const string Log_OutFile = "FileName"; //ログ_出力ファイル名

	private CrsMasterKeyKoumoku[] _taisyoCrsInfo; //_対象コース情報()
	private DataTable _logTable = null; //_ログテーブル

	//↓追加
	private EntityOperation[] crsMasterEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //コースマスタエンティティ

	#endregion

	#region  構造体／列挙型

	//所属が定期観光部の場合に出力する帳票
	private enum _TeikiKankoBuForm
	{
		[Value("たて表")] _tateTable = 0, //_たて表
		[Value("路線図")] _rosenzu //_路線図
								//_ルート表
	}

	// 所属が企画旅行部の場合に出力する帳票
	private enum _KikakuTravelBuForm
	{
		[Value("たて表")] _tateTable = 0 //_たて表
	}

	// グリッドに設定する物理名
	private enum _GridPhysicsName
	{
		NO = 0, //NO
		TEIKI_KIKAKU_KBN, //TEIKI_KIKAKU_KBN
		CRS_CD, //CRS_CD
		KAITEI_DATE, //KAITEI_DATE
		CRS_YEAR, //CRS_YEAR
		SEASON, //SEASON
		SEASON_DISPLAY, //SEASON_DISPLAY
		INVALID_FLG, //INVALID_FLG
		CRS_NAME, //CRS_NAME
		PAMPH_IRAI_DATE, //PAMPH_IRAI_DATE
		CRS_KIND_1, //CRS_KIND_1
		CRS_KIND_2, //CRS_KIND_2
		RESULT //RESULT
	}

	// グリッドに設定する列
	private enum _GridItem
	{
		[Value("No")] no = 0,
		[Value("定期企画区分")] teikiKikakuKbn,
		[Value("コースコード")] crsCd,
		[Value("改定日")] kaiteiDay,
		[Value("年")] year,
		[Value("季_値")] season_Value,
		[Value("季")] season,
		[Value("無効フラグ")] invalidFlg,
		[Value("コース名")] crsName,
		[Value("パンフ依頼日")] pamphIraiDay,
		[Value("コース種別１")] crsKind1,
		[Value("コース種別２")] crsKind2,
		[Value("結果")] result
	}

	#endregion

	#region  プロパティ

	/// <summary>
	/// 遷移元から受け取るコース情報
	/// </summary>
	/// <value></value>
	/// <remarks></remarks>
	public CrsMasterKeyKoumoku[] TaisyoCrsInfo //対象コース情報()
	{
		set
		{
			_taisyoCrsInfo = value;
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
	private void frmPamphletIrai_Load(System.Object sender, System.EventArgs e)
	{

		// 初期化
		this.setControlInitialize();
		this.setBackColorInitialize();
		this.setGridInitialize();

		// コース検索で選択されたコースを一覧に表示
		this.grdList.DataSource = this.getDataSouce();

		try
		{
			// カーソルを砂時計に変更
			this.Cursor = Cursors.WaitCursor;

			// 出力先フォルダの初期値を設定
			this.setOutputFolderInit();

			//ADD-20130617-6月運用サポート-出力単位の変更↓
			this.rdoOutKeisikiPDF_CrsEvery.Checked = true;
			//ADD-20130617-6月運用サポート-出力単位の変更↑

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

		//DEL-20120224-参照によるフォルダ指定を可能とするため↓
		// 作成フォルダにフォーカスを設定
		//Me.ActiveControl = Me.txt作成フォルダ

	}

	/// <summary>
	/// 画面終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frmPamphletIrai_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
	{

		this.Owner.Show();
		this.Owner.Activate();

	}

	/// <summary>
	/// [参照]ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void btnReference_Click(System.Object sender, System.EventArgs e)
	{

		FolderBrowserDialog fdb = new FolderBrowserDialog(); //fdb

		//初期パス
		//ADD-20120224-入力されたフォルダパスを参照できるよう修正↓
		if (this.txtOutSakiFolder.Text == string.Empty || System.IO.Directory.Exists(this.txtOutSakiFolder.Text) == false)
		{
			fdb.SelectedPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		}
		else
		{
			fdb.SelectedPath = this.txtOutSakiFolder.Text;
		}
		fdb.ShowNewFolderButton = true;
		//ADD-20120224-入力されたフォルダパスを参照できるよう修正↑

		if (fdb.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
		{
			this.txtOutSakiFolder.Text = fdb.SelectedPath;
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
	/// [F10:実行]ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		CrsMasterKeyKoumoku[] lockInfo = null; //lockInfo()
		ArrayList idxRows = null; //idxRows
		int idx = 0; //idx

		// 背景色を初期化
		this.setBackColorInitialize();

		// 必須チェック
		if (this.isInputFolder() == false)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0013")
			createFactoryMsg.messageDisp("0013");
			return;
		}

		// 存在チェック
		if (System.IO.Directory.Exists(this.txtOutSakiFolder.Text) == false)
		{
			//UPD-20120224-メッセージ変更↓
			//メッセージ出力.messageDisp("0002", "フォルダ：" & Me.txt出力先フォルダ.Text)
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("0074", "フォルダ[" & Me.txt出力先フォルダ.Text & "]")
			createFactoryMsg.messageDisp("0074", "フォルダ[" + this.txtOutSakiFolder.Text + "]");
			this.txtOutSakiFolder.ExistError = true;
			this.txtOutSakiFolder.Focus();
			return;
		}

		try
		{
			// カーソルを砂時計に変更
			this.Cursor = Cursors.WaitCursor;

			// グリッドの結果列をクリア
			this.clearResultCol();

			// ロックをかける
			this.lockCourseMst(ref lockInfo);

			// 実行チェック
			for (idx = this.grdList.Rows.Fixed; idx <= this.grdList.Rows.Count - 1; idx++)
			{
				if (string.IsNullOrEmpty(System.Convert.ToString(this.grdList.Rows(idx).Item(_GridPhysicsName.RESULT.ToString()).ToString())) == true)
				{
					break;
				}
			}

			// 結果が全てエラーもしくはロック中だった場合のみメッセージを表示し処理を中断する
			if (idx == this.grdList.Rows.Count)
			{
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0033")
				createFactoryMsg.messageDisp("0033");
				return;
			}

			// 出力件数チェック
			if (this.getOutputCount() == 0)
			{
				//TODO:共通変更対応
				//Call メッセージ出力.messageDisp("0007")
				createFactoryMsg.messageDisp("0007");
				return;
			}

			// 確認
			//TODO:共通変更対応
			//If メッセージ出力.messageDisp("0031", "パンフ依頼") = MsgBoxResult.Cancel Then
			if (createFactoryMsg.messageDisp("0031", "パンフ依頼") == MsgBoxResult.Cancel)
			{
				return;
			}

			try
			{
				//*** 出力 ***'
				idxRows = this.instructOutput();

				if (ReferenceEquals(idxRows, null) == false)
				{

					// コースマスタ更新
					for (idx = 0; idx <= idxRows.Count - 1; idx++)
					{
						this.updateCourseMst(System.Convert.ToInt32(idxRows[idx].ToString()));
					}

				}
				else
				{
					return;
				}

			}
			catch (Exception ex)
			{
				if (idx < idxRows.Count)
				{
					while (idx < idxRows.Count)
					{
						this.grdList.Rows(System.Convert.ToInt32(idxRows[idx].ToString())).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_Error;
						//TODO:共通変更対応
						//MyBase.outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.更新, Me.setTitle & "_更新", ex.Message)
						createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, this.setTitle + "_更新", ex.Message);
						idx++;
					}
				}
				throw (ex);
			}
			finally
			{
				// ロックの解除
				this.relockCourseMst(lockInfo);
			}

			// 終了
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0035", "パンフ依頼")
			createFactoryMsg.messageDisp("0035", "パンフ依頼");
		}
		catch (Exception ex)
		{
			//TODO:共通変更対応
			//Call メッセージ出力.messageDisp("0033")
			//MyBase.outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.パンフ依頼, Me.setTitle & "_実行", ex.Message)
			createFactoryMsg.messageDisp("0033");
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.pamphIrai, this.setTitle + "_実行", ex.Message);

		}
		finally
		{
			// カーソルを元に戻す
			this.Cursor = Cursors.Default;
		}

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
		this.F4Key_Visible = false;
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
		this.txtOutSakiFolder.Text = string.Empty;

		//DEL-20120224-参照によるフォルダ指定を可能とするため↓
		//Me.txt作成フォルダ.Text = String.Empty

		// チェックボックスのクリア
		this.chkOutAlready.Checked = false;

	}

	/// <summary>
	/// コントロールの背景色の初期化
	/// </summary>
	/// <remarks></remarks>
	private void setBackColorInitialize()
	{

		// 必須/任意項目の設定
		this.txtOutSakiFolder.NotNull = true;

		//DEL-20120224-参照によるフォルダ指定を可能とするため↓
		//Me.txt作成フォルダ.必須項目 = True

		// エラー有無の初期化
		this.txtOutSakiFolder.ExistError = false;

		//'DEL-20120224-参照によるフォルダ指定を可能とするため↓
		//Me.txt作成フォルダ.エラー有無 = False

	}

	/// <summary>
	/// グリッドの初期化
	/// </summary>
	/// <remarks></remarks>
	private void setGridInitialize()
	{

		object with_1 = this.grdList;

		object with_2 = with_1.Cols(_GridItem.no);
		with_2.Name = _GridPhysicsName.NO.ToString();
		with_2.Caption = getEnumAttrValue(_GridItem.no);
		with_2.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_2.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_2.Width = 35;

		object with_3 = with_1.Cols(_GridItem.teikiKikakuKbn);
		with_3.Name = _GridPhysicsName.TEIKI_KIKAKU_KBN.ToString();
		with_3.Visible = false;

		object with_4 = with_1.Cols(_GridItem.crsCd);
		with_4.Name = _GridPhysicsName.CRS_CD.ToString();
		with_4.Caption = getEnumAttrValue(_GridItem.crsCd);
		with_4.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_4.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_4.Width = 110;

		object with_5 = with_1.Cols(_GridItem.kaiteiDay);
		with_5.Name = _GridPhysicsName.KAITEI_DATE.ToString();
		with_5.Visible = false;

		object with_6 = with_1.Cols(_GridItem.year);
		with_6.Name = _GridPhysicsName.CRS_YEAR.ToString();
		with_6.Caption = getEnumAttrValue(_GridItem.year);
		with_6.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_6.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_6.Width = 60;

		object with_7 = with_1.Cols(_GridItem.season_Value);
		with_7.Name = _GridPhysicsName.SEASON.ToString();
		with_7.Visible = false;

		object with_8 = with_1.Cols(_GridItem.season);
		with_8.Name = _GridPhysicsName.SEASON_DISPLAY.ToString();
		with_8.Caption = getEnumAttrValue(_GridItem.season);
		with_8.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_8.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_8.Width = 100;

		object with_9 = with_1.Cols(_GridItem.invalidFlg);
		with_9.Name = _GridPhysicsName.INVALID_FLG.ToString();
		with_9.Visible = false;

		object with_10 = with_1.Cols(_GridItem.crsName);
		with_10.Name = _GridPhysicsName.CRS_NAME.ToString();
		with_10.Caption = getEnumAttrValue(_GridItem.crsName);
		with_10.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_10.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;
		with_10.Width = 350;

		object with_11 = with_1.Cols(_GridItem.pamphIraiDay);
		with_11.Name = _GridPhysicsName.PAMPH_IRAI_DATE.ToString();
		with_11.Caption = getEnumAttrValue(_GridItem.pamphIraiDay);
		with_11.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_11.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_11.Width = 120;

		object with_12 = with_1.Cols(_GridItem.crsKind1);
		with_12.Name = _GridPhysicsName.CRS_KIND_1.ToString();
		with_12.Visible = false;

		object with_13 = with_1.Cols(_GridItem.crsKind2);
		with_13.Name = _GridPhysicsName.CRS_KIND_2.ToString();
		with_13.Visible = false;

		object with_14 = with_1.Cols(_GridItem.result);
		with_14.Name = _GridPhysicsName.RESULT.ToString();
		with_14.Caption = getEnumAttrValue(_GridItem.result);
		with_14.TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		with_14.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;
		with_14.Width = 100;


	}

	#endregion

	#region  データグリッド関連

	/// <summary>
	/// データテーブルを作成し、コース情報を設定して返す。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private DataTable getDataSouce() //getDataSouce
	{

		DataTable returnValue = new DataTable(); //returnValue
		DataColumn colNo = new DataColumn(); //colNo
		DataColumn colTeikiKikakuKbn = new DataColumn(); //colTeikiKikakuKbn
		DataColumn colCrsCd = new DataColumn(); //colCrsCd
		DataColumn colKaiteiDate = new DataColumn(); //colKaiteiDate
		DataColumn colCrsYear = new DataColumn(); //colCrsYear
		DataColumn colSeason = new DataColumn(); //colSeason
		DataColumn colSeasonDisplay = new DataColumn(); //colSeasonDisplay
		DataColumn colInvalidFlg = new DataColumn(); //colInvalidFlg
		DataColumn colCrsName = new DataColumn(); //colCrsName
		DataColumn colPnmphIraiDate = new DataColumn(); //colPnmphIraiDate
		DataColumn colCrsKind1 = new DataColumn(); //colCrsKind1
		DataColumn colCrsKind2 = new DataColumn(); //colCrsKind2
		DataColumn colResult = new DataColumn(); //colResult
		int idx = 0; //idx

		//列の設定
		colNo.ColumnName = _GridPhysicsName.NO.ToString();
		colNo.DataType = typeof(int);
		colTeikiKikakuKbn.ColumnName = _GridPhysicsName.TEIKI_KIKAKU_KBN.ToString();
		colTeikiKikakuKbn.DataType = typeof(string);
		colCrsCd.ColumnName = _GridPhysicsName.CRS_CD.ToString();
		colCrsCd.DataType = typeof(string);
		colKaiteiDate.ColumnName = _GridPhysicsName.KAITEI_DATE.ToString();
		colKaiteiDate.DataType = typeof(string);
		colCrsYear.ColumnName = _GridPhysicsName.CRS_YEAR.ToString();
		colCrsYear.DataType = typeof(int);
		colSeason.ColumnName = _GridPhysicsName.SEASON.ToString();
		colSeason.DataType = typeof(string);
		colSeasonDisplay.ColumnName = _GridPhysicsName.SEASON_DISPLAY.ToString();
		colSeasonDisplay.DataType = typeof(string);
		colInvalidFlg.ColumnName = _GridPhysicsName.INVALID_FLG.ToString();
		colInvalidFlg.DataType = typeof(int);
		colCrsName.ColumnName = _GridPhysicsName.CRS_NAME.ToString();
		colCrsName.DataType = typeof(string);
		colPnmphIraiDate.ColumnName = _GridPhysicsName.PAMPH_IRAI_DATE.ToString();
		colPnmphIraiDate.DataType = typeof(string);
		colCrsKind1.ColumnName = _GridPhysicsName.CRS_KIND_1.ToString();
		colCrsKind1.DataType = typeof(string);
		colCrsKind2.ColumnName = _GridPhysicsName.CRS_KIND_2.ToString();
		colCrsKind2.DataType = typeof(string);
		colResult.ColumnName = _GridPhysicsName.RESULT.ToString();
		colResult.DataType = typeof(string);

		//データテーブルに列を設定
		returnValue.Columns.Add(colNo);
		returnValue.Columns.Add(colTeikiKikakuKbn);
		returnValue.Columns.Add(colCrsCd);
		returnValue.Columns.Add(colKaiteiDate);
		returnValue.Columns.Add(colCrsYear);
		returnValue.Columns.Add(colSeason);
		returnValue.Columns.Add(colSeasonDisplay);
		returnValue.Columns.Add(colInvalidFlg);
		returnValue.Columns.Add(colCrsName);
		returnValue.Columns.Add(colPnmphIraiDate);
		returnValue.Columns.Add(colCrsKind1);
		returnValue.Columns.Add(colCrsKind2);
		returnValue.Columns.Add(colResult);

		//データテーブルにコース情報データを設定
		for (idx = 0; idx <= _taisyoCrsInfo.Length - 1; idx++)
		{
			returnValue.Rows.Add(idx + 1, _taisyoCrsInfo[idx].Teiki_KikakuKbn, _taisyoCrsInfo[idx].CrsCd, _taisyoCrsInfo[idx].KaiteiDay, _taisyoCrsInfo[idx].Year, _taisyoCrsInfo[idx].Season, _taisyoCrsInfo[idx].Season_DisplayFor, _taisyoCrsInfo[idx].InvalidFlg, _taisyoCrsInfo[idx].CrsName, _taisyoCrsInfo[idx].PamphIraiDay_DisplayFor, _taisyoCrsInfo[idx].CrsKind1, _taisyoCrsInfo[idx].CrsKind2, string.Empty);
		}

		return returnValue;

	}

	/// <summary>
	/// グリッドの結果列をクリアする。
	/// </summary>
	/// <remarks></remarks>
	private void clearResultCol() //PrivateSubclearResultCol()
	{

		int idx = 0;

		for (idx = this.grdList.Rows.Fixed; idx <= this.grdList.Rows.Count - 1; idx++)
		{
			this.grdList.Rows(idx).Item[_GridPhysicsName.RESULT.ToString()] = string.Empty;
		}

	}

	#endregion

	#region  データアクセス関連

	/// <summary>
	/// 出力先フォルダの初期値を設定する。
	/// </summary>
	/// <remarks></remarks>
	private void setOutputFolderInit() //PrivateSubsetOutputFolderInit()
	{

		PamphIrai_DA dataAccess = new PamphIrai_DA(); //dataAccess
		CdMasterEntity cdMstEntity = new CdMasterEntity(); //cdMstEntity
		DataTable returnTable = null; //returnTable

		//コードマスタデータ取得
		try
		{
			returnTable = dataAccess.getCodeMasterData(this.getCodeMstParameterList());

			if (returnTable.Rows.Count > 0)
			{
				this.txtOutSakiFolder.Text = returnTable.Rows(0).Item(cdMstEntity.Naiyo1.PhysicsName).ToString();
			}

		}
		catch (Exception ex)
		{
			throw (ex);
		}

	}

	/// <summary>
	/// DAクラスの「コードマスタSELECT」に渡すパラメーターを設定する。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private Hashtable getCodeMstParameterList()
	{

		Hashtable parameterList = new Hashtable(); //parameterList

		parameterList.Add("Bunrui", CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.systemSet));

		if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(FixedCd.Teiki_KikakuKbnType.teikiKanko))
		{
			parameterList.Add("Cd", CommonType_MojiColValue.SystemSetType_Value(SystemSetType.AGCShareFolderName_Teiki));
		}
		else if (_taisyoCrsInfo[0].Teiki_KikakuKbn == System.Convert.ToString(FixedCd.Teiki_KikakuKbnType.kikakuTravel))
		{
			parameterList.Add("Cd", CommonType_MojiColValue.SystemSetType_Value(SystemSetType.AGCShareFolderName_KikaKu));
		}
		else
		{
			parameterList.Add("Cd", string.Empty);
		}

		return parameterList;

	}

	/// <summary>
	/// 「コースマスタ」のパンフ依頼日付を更新する。
	/// </summary>
	/// <param name="idxRow">グリッド上の対象行番号</param>
	/// <remarks></remarks>
	private void updateCourseMst(int idxRow) //PrivateSubupdateCourseMst(idxRowAsInteger)
	{

		PamphIrai_DA dataAccess = new PamphIrai_DA(); //dataAccess
		Hashtable parameterList = null; //parameterList
		int idx = 0; //idx

		try //Try
		{
			//パラメーターの設定
			parameterList = this.getCourseMstParameterList(idxRow);

			//更新処理の実行
			if (dataAccess.executeUpdateCourseMaster(parameterList) == 1)
			{
				//正常終了の場合

				//ログ出力
				for (idx = 0; idx <= this._logTable.Rows.Count - 1; idx++)
				{
					if (this.grdList.Rows(idxRow).Item(_GridPhysicsName.CRS_CD.ToString()).ToString().Equals(this._logTable.Rows(idx).Item(Log_KeyCrsCd).ToString()) == true)
					{
						///TODO:共通変更対応
						///MyBase.outputLog(ログ種別タイプ.操作ログ,_
						///処理種別タイプ.パンフ依頼,_
						///Me.setTitle,_
						///Me._ログテーブル.Rows(idx).Item(ログ_コースコード).ToString(),_
						///Me._ログテーブル.Rows(idx).Item(ログ_出力ファイル名).ToString())
						createFactoryLog.logOutput(LogKindType.operationLog,;
						ProcessKindType.pamphIrai(,);
						Me.setTitle(,);
						this._logTable.Rows(idx).Item(Log_CrsCd).ToString(),;
						this._logTable.Rows(idx).Item(Log_OutFile).ToString());
			}
		}

				//グリッドに反映
		this.grdList.Rows(idxRow).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_Normal;
		this.grdList.Rows(idxRow).Item[_GridPhysicsName.PAMPH_IRAI_DATE.ToString()] = DateTime.Parse(System.Convert.ToString(parameterList.Item["PamphIraiDate"].ToString())).ToString("yyyy/MM/dd");

	}
			else
			{
				//エラーが発生した場合
				this.grdList.Rows(idxRow).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_Error;
				///TODO:共通変更対応
				///Callメッセージ出力.messageDisp("0002","データ")
				///MyBase.outputLog(ログ種別タイプ.エラーログ,処理種別タイプ.更新,Me.setTitle&"_更新","該当のデータが存在しません。")
				createFactoryMsg.messageDisp("0002", "データ");
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, this.setTitle + "_更新", "該当のデータが存在しません。");
	}

}
		catch (OracleException ex)
{
	this.grdList.Rows(idxRow).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_Error;
	///TODO:共通変更対応
	///Callメッセージ出力.messageDisp("0006",ex.Number.ToString())
	///MyBase.outputLog(ログ種別タイプ.エラーログ,処理種別タイプ.更新,Me.setTitle&"_更新",ex.Message)
	createFactoryMsg.messageDisp("0006", ex.Number.ToString());
	createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, this.setTitle + "_更新", ex.Message);
}
catch (Exception ex)
{
	throw (ex);
}
		
	}
	
	/// <summary>
	/// DAクラスの「コースマスタUPDATE」に渡すパラメーターを設定する。
	/// </summary>
	/// <param name="idxRow">対象のグリッド行インデックス</param>
	/// <returns></returns>
	/// <remarks></remarks>
	private Hashtable getCourseMstParameterList(int idxRow)
{

	Hashtable parameterList = new Hashtable(); //parameterList

	object with_1 = this.grdList.Rows(idxRow);

	parameterList.Add("TeikiKikakuKbn", with_1.Item(_GridPhysicsName.TEIKI_KIKAKU_KBN.ToString()).ToString());
	parameterList.Add("CrsCd", with_1.Item(_GridPhysicsName.CRS_CD.ToString()).ToString());
	parameterList.Add("KaiteiDate", with_1.Item(_GridPhysicsName.KAITEI_DATE.ToString()).ToString());
	parameterList.Add("CrsYear", with_1.Item(_GridPhysicsName.CRS_YEAR.ToString()));
	parameterList.Add("Season", with_1.Item(_GridPhysicsName.SEASON.ToString()).ToString());
	parameterList.Add("InvalidFlg", with_1.Item(_GridPhysicsName.INVALID_FLG.ToString()));
	parameterList.Add("PamphIraiDate", getDateTime);
	parameterList.Add("UpdateDate", createFactoryDA.getServerSysDate);
	parameterList.Add("UpdateClient", UserInfoManagement.client);
	parameterList.Add("UpdateUserId", UserInfoManagement.userId);


	return parameterList;

}

#endregion

#region  チェック関連

/// <summary>
/// 必須チェック
/// 「出力先フォルダ」と「作成フォルダ」が指定されているかチェックする。
/// </summary>
/// <returns></returns>
/// <remarks></remarks>
private bool isInputFolder()
{

	bool returnValue = true;

	//DEL-20120224-参照によるフォルダ指定を可能とするため↓
	//IfString.IsNullOrEmpty(RTrim(Me.txt作成フォルダ.Text)) = True Then
	//Me.txt作成フォルダ.エラー有無 = True
	//Me.txt作成フォルダ.Focus()
	//returnValue = False
	//End If

	if (string.IsNullOrEmpty(Strings.RTrim(System.Convert.ToString(this.txtOutSakiFolder.Text))) == true)
	{
		this.txtOutSakiFolder.ExistError = true;
		this.txtOutSakiFolder.Focus();
		return false;
	}

	return returnValue;

}

/// <summary>
/// 出力対象の件数を返す。
/// </summary>
/// <returns></returns>
/// <remarks></remarks>
private int getOutputCount()
{

	int idx = 0;
	int countNum = 0;

	for (idx = this.grdList.Rows.Fixed; idx <= this.grdList.Rows.Count - 1; idx++)
	{

		object with_1 = this.grdList.Rows(idx);

		if (this.chkOutAlready.Checked == true)
		{
			if (string.IsNullOrEmpty(System.Convert.ToString(with_1.Item(_GridPhysicsName.RESULT.ToString()).ToString())) == true)
			{
				countNum++;
			}
		}
		else
		{

			{
				string.IsNullOrEmpty(System.Convert.ToString(with_1.Item(_GridPhysicsName.RESULT.ToString()).ToString())) = System.Convert.ToBoolean(true);
				countNum++;
			}
		}


	}

	return countNum;

}

/// <summary>
/// 「コースマスタ」ロック処理
/// </summary>
/// <param name="lockInfo"></param>
/// <remarks></remarks>
private void lockCourseMst(ref CrsMasterKeyKoumoku[] lockInfo)
{

	CrsLock clsCrsLock = new CrsLock(); //clsコースロック
	UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //userInfo
	CrsState returnLockValue = null; //returnLockValue
	CrsMasterKeyKoumoku lockRowKeys = null; //lockRowKeys
	string lockUser = string.Empty; //lockUser
	int idx = 0; //idx
	bool returnValue = false; //returnValue

	userInfo.UserId = UserInfoManagement.userId;
	userInfo.Client = UserInfoManagement.client;
	userInfo.ProcessId = UserInfoManagement.processId;

	try
	{

		for (idx = this.grdList.Rows.Fixed; idx <= this.grdList.Rows.Count - 1; idx++)
		{

			//結果列が空白のレコードのみ処理対象とする
			if (string.IsNullOrEmpty(System.Convert.ToString(this.grdList.Rows(idx).Item(_GridPhysicsName.RESULT.ToString()).ToString())) == true)
			{
				if (!(this.chkOutAlready.Checked == false && string.IsNullOrEmpty(System.Convert.ToString(this.grdList.Rows(idx).Item(_GridPhysicsName.PAMPH_IRAI_DATE.ToString()).ToString())) == false))
				{

					lockRowKeys = new CrsMasterKeyKoumoku();
					lockRowKeys.Teiki_KikakuKbn = this._taisyoCrsInfo[idx - 1].Teiki_KikakuKbn;
					lockRowKeys.CrsCd = this._taisyoCrsInfo[idx - 1].CrsCd;
					lockRowKeys.KaiteiDay = this._taisyoCrsInfo[idx - 1].KaiteiDay.Replace("/", "");
					lockRowKeys.Year = this._taisyoCrsInfo[idx - 1].Year;
					lockRowKeys.Season = this._taisyoCrsInfo[idx - 1].Season;
					lockRowKeys.InvalidFlg = this._taisyoCrsInfo[idx - 1].InvalidFlg;

					returnLockValue = clsCrsLock.ExecuteCourseLockMain(lockRowKeys, userInfo, lockUser);

					if (returnLockValue == CrsState.lockSuccess)
					{
						if (ReferenceEquals(lockInfo, null) == true)
						{
							lockInfo = new CrsMasterKeyKoumoku[1];
						}
						else
						{
							Array.Resize(ref lockInfo, lockInfo.Length + 1);
						}
						lockInfo[lockInfo.Length - 1] = lockRowKeys;
					}
					else if (returnLockValue == CrsState.lockChu)
					{
						this.grdList.Rows(idx).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_LockChu + "[" + lockUser + "]";
					}
					else if (returnLockValue == CrsState.lockFailure)
					{
						this.grdList.Rows(idx).Item[_GridPhysicsName.RESULT.ToString()] = ProcessResult_Error;
					}

				}
			}

		}

	}
	catch (Exception ex)
	{
		throw (ex);
	}

}

/// <summary>
/// 「コースマスタ」ロック解除処理
/// </summary>
/// <param name="lockInfo"></param>
/// <remarks></remarks>
private void relockCourseMst(CrsMasterKeyKoumoku[] lockInfo)
{

	CrsLock clsCrsLock = new CrsLock(); //clsコースロック
	UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //userInfo
	int idx = 0; //idx

	if (ReferenceEquals(lockInfo, null) == true)
	{
		return;
	}

	userInfo.UserId = UserInfoManagement.userId;
	userInfo.Client = UserInfoManagement.client;
	userInfo.ProcessId = UserInfoManagement.processId;

	for (idx = 0; idx <= lockInfo.Length - 1; idx++)
	{
		clsCrsLock.ExecuteCourseLockReleaseMain(lockInfo[idx], userInfo);
	}

}

#endregion

#region  ファイル出力

/// <summary>
/// ファイルの出力指示を出す。
/// </summary>
/// <remarks></remarks>
private ArrayList instructOutput()
{

	CrsMasterKeyKoumoku[] courseInfo = null;
	string newFolderPath = string.Empty;
	int idx = 0;
	int idxArray = 0;
	ArrayList returnValue = new ArrayList();

	//フォルダの作成
	try
	{
		//UPD-20120224-参照によるフォルダ指定を可能とするため変更↓
		//newFolderPath = System.IO.Path.Combine(Me.txt出力先フォルダ.Text, Me.txt作成フォルダ.Text)
		newFolderPath = System.Convert.ToString(this.txtOutSakiFolder.Text);
		System.IO.Directory.CreateDirectory(newFolderPath);
	}
	catch (Exception)
	{
		//TODO:共通変更対応
		//Call メッセージ出力.messageDisp("0040")
		createFactoryMsg.messageDisp("0040");
		return null;
	}

	try
	{

		//コース情報の設定
		for (idx = this.grdList.Rows.Fixed; idx <= this.grdList.Rows.Count - 1; idx++)
		{
			if (string.IsNullOrEmpty(System.Convert.ToString(this.grdList.Rows(idx).Item(_GridPhysicsName.RESULT.ToString()).ToString())) == true)
			{

				if (this.chkOutAlready.Checked == false)
				{
					// パンフ依頼が既にされているコースを含まない
					if (string.IsNullOrEmpty(System.Convert.ToString(this.grdList.Rows(idx).Item(_GridPhysicsName.PAMPH_IRAI_DATE.ToString()).ToString())) == true)
					{
						Array.Resize(ref courseInfo, idxArray + 1);
						idxArray++;
						this.setCourseInfo(ref courseInfo, idx);
						returnValue.Add(idx);
					}
				}
				else
				{
					Array.Resize(ref courseInfo, idxArray + 1);
					idxArray++;
					this.setCourseInfo(ref courseInfo, idx);
					returnValue.Add(idx);
				}

			}
		}

		this.outputPDF(courseInfo, newFolderPath);

	}
	catch (Exception ex)
	{
		throw (ex);
	}

	return returnValue;

}

/// <summary>
/// PDF出力処理
/// </summary>
/// <param name="courseInfo"></param>
/// <param name="folderPath"></param>
/// <remarks></remarks>
private void outputPDF(CrsMasterKeyKoumoku[] courseInfo, string folderPath)
{

	string fullFileName = string.Empty;
	string fileName = string.Empty;
	int idx = 0;
	int idxCourse = 0;
	int idxArray = 0;
	int idxTable = 0;
	DateTime outputTime = new DateTime();
	CrsMasterKeyKoumoku[] tateInfo = null;
	DataColumn colKeyCrsCd = new DataColumn();
	DataColumn colCrsCd = new DataColumn();
	DataColumn colFileName = new DataColumn();

	TateTable_Stay clsTateTable_Stay = null; //clsたて表_宿泊
	TateTable_Higaeri clsTateTable_Higaeri = null; //clsたて表_日帰り
	Rosenzu clsRosenzu = null; //cls路線図

	try
	{
		this._logTable = new DataTable();
		colKeyCrsCd.ColumnName = Log_KeyCrsCd;
		colKeyCrsCd.DataType = typeof(string);
		colCrsCd.ColumnName = Log_CrsCd;
		colCrsCd.DataType = typeof(string);
		colFileName.ColumnName = Log_OutFile;
		colFileName.DataType = typeof(string);
		this._logTable.Columns.Add(colKeyCrsCd);
		this._logTable.Columns.Add(colCrsCd);
		this._logTable.Columns.Add(colFileName);

		outputTime = DateTime.Now;

		if (courseInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{

			for (idx = 0; idx <= GetValues(typeof(_TeikiKankoBuForm)).Length - 1; idx++)
			{
				switch (idx)
				{

					case (int)_TeikiKankoBuForm._tateTable:
						//*** たて表(宿泊) ***'
						fileName = File_Teiki + "_" + getEnumAttrValue(_TeikiKankoBuForm._tateTable) + "_宿泊_" + outputTime.ToString("yyyyMMddHHmm") + ".pdf";
						fullFileName = System.Convert.ToString(System.IO.Path.Combine(folderPath, fileName));

						idxArray = 0;
						for (idxCourse = 0; idxCourse <= courseInfo.Length - 1; idxCourse++)
						{
							if (courseInfo[idxCourse].CrsKind2.Equals(System.Convert.ToString(FixedCd.CrsKind2.stay)) == true)
							{
								Array.Resize(ref tateInfo, idxArray + 1);
								tateInfo[idxArray] = courseInfo[idxCourse];
								idxArray++;
							}
						}

						if (ReferenceEquals(tateInfo, null) == false)
						{

							clsTateTable_Stay = new TateTable_Stay();
							clsTateTable_Stay.CrsMasterEntity = crsMasterEntity;
							clsTateTable_Stay.OutFile = fileName;
							clsTateTable_Stay.OutSakiFolder = folderPath;
							clsTateTable_Stay.BfGamen = BfGamenType.crsSearch;
							clsTateTable_Stay.TaisyoCrsInfo = tateInfo;
							//UPD-20130617-6月運用サポート-出力単位の変更↓
							if (rdoOutKeisikiPDF_CrsEvery.Checked == true)
							{
								clsTateTable_Stay.FormProcessKind = FormProcessType.pDF_CrsEvery;
							}
							else
							{
								clsTateTable_Stay.FormProcessKind = FormProcessType.pDF_AllCrs;
							}
							//UPD-20130617-6月運用サポート-出力単位の変更↑

							if (System.IO.File.Exists(fullFileName) == true)
							{
								//TODO:共通変更対応
								//If メッセージ出力.messageDisp("0038",fileName) = MsgBoxResult.Ok Then
								if (createFactoryMsg.messageDisp("0038", fileName) == MsgBoxResult.Ok)
								{
									//PDF出力
									clsTateTable_Stay.outputReport();
									for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
									{
										this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
										tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
										tateInfo[idxTable].Year.ToString() +;
										tateInfo[idxTable].Season.ToString() +;
										tateInfo[idxTable].CrsCd.ToString() +;
										tateInfo[idxTable].InvalidFlg.ToString(),;
										fileName());
									}
								}
							}
							else
{
	//PDF出力
	clsTateTable_Stay.outputReport();
	for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
	{
		this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
		tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
		tateInfo[idxTable].Year.ToString() +;
		tateInfo[idxTable].Season.ToString() +;
		tateInfo[idxTable].CrsCd.ToString() +;
		tateInfo[idxTable].InvalidFlg.ToString(),;
		fileName());
}
							}
							
						}
						
						//*** たて表(日帰り) ***
						fileName = File_Teiki + "_" + getEnumAttrValue(_TeikiKankoBuForm._tateTable) + "_日帰り_" + outputTime.ToString("yyyyMMddHHmm") + ".pdf";
fullFileName = System.Convert.ToString(System.IO.Path.Combine(folderPath, fileName));

tateInfo = null;
idxArray = 0;
for (idxCourse = 0; idxCourse <= courseInfo.Length - 1; idxCourse++)
{
	if (courseInfo[idxCourse].CrsKind2.Equals(System.Convert.ToString(FixedCd.CrsKind2.stay)) == false)
	{
		Array.Resize(ref tateInfo, idxArray + 1);
		tateInfo[idxArray] = courseInfo[idxCourse];
		idxArray++;
	}
}

if (ReferenceEquals(tateInfo, null) == false)
{

	clsTateTable_Higaeri = new TateTable_Higaeri();
	clsTateTable_Higaeri.CrsMasterEntity = crsMasterEntity;
	clsTateTable_Higaeri.OutFile = fileName;
	clsTateTable_Higaeri.OutSakiFolder = folderPath;
	clsTateTable_Higaeri.BfGamen = BfGamenType.crsSearch;
	clsTateTable_Higaeri.TaisyoCrsInfo = tateInfo;
	//UPD-20130617-6月運用サポート-出力単位の変更↓
	if (rdoOutKeisikiPDF_CrsEvery.Checked == true)
	{
		clsTateTable_Higaeri.FormProcessKind = FormProcessType.pDF_CrsEvery;
	}
	else
	{
		clsTateTable_Higaeri.FormProcessKind = FormProcessType.pDF_AllCrs;
	}
	//UPD-20130617-6月運用サポート-出力単位の変更↑

	if (System.IO.File.Exists(fullFileName) == true)
	{
		//TODO:共通変更対応
		//Ifメッセージ出力.messageDisp("0038",fileName)
		if (createFactoryMsg.messageDisp("0038", fileName) == MsgBoxResult.Ok)
		{
			//PDF出力
			clsTateTable_Higaeri.outputReport(Form_RegularNum.Mode.teiki);
			for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
			{
				this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
				tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
				tateInfo[idxTable].Year.ToString() +;
				tateInfo[idxTable].Season.ToString() +;
				tateInfo[idxTable].CrsCd.ToString() +;
				tateInfo[idxTable].InvalidFlg.ToString(),;
				fileName());
		}
	}
}
else
{
	//PDF出力
	clsTateTable_Higaeri.outputReport(Form_RegularNum.Mode.teiki);
	for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
	{
		this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
		tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
		tateInfo[idxTable].Year.ToString() +;
		tateInfo[idxTable].Season.ToString() +;
		tateInfo[idxTable].CrsCd.ToString() +;
		tateInfo[idxTable].InvalidFlg.ToString(),;
		fileName());
}
							}
							
						}
						break;
						
					case (int)_TeikiKankoBuForm._rosenzu:
						fileName = File_Teiki + "_" + getEnumAttrValue(_TeikiKankoBuForm._rosenzu) + "_" + outputTime.ToString("yyyyMMddHHmm") + ".pdf";
fullFileName = System.Convert.ToString(System.IO.Path.Combine(folderPath, fileName));

clsRosenzu = new Rosenzu();
clsRosenzu.CrsMasterEntity = crsMasterEntity;
clsRosenzu.OutFile = fileName;
clsRosenzu.OutSakiFolder = folderPath;
clsRosenzu.BfGamen = BfGamenType.crsSearch;
clsRosenzu.TaisyoCrsInfo = courseInfo;
//UPD-20130617-6月運用サポート-出力単位の変更↓
if (rdoOutKeisikiPDF_CrsEvery.Checked == true)
{
	clsRosenzu.FormProcessKind = FormProcessType.pDF_CrsEvery;
}
else
{
	clsRosenzu.FormProcessKind = FormProcessType.pDF_AllCrs;
}
//UPD-20130617-6月運用サポート-出力単位の変更↑

if (System.IO.File.Exists(fullFileName) == true)
{
	//TODO:共通変更対応
	//Ifメッセージ出力.messageDisp("0038",fileName) = MsgBoxResult.Ok Then
	if (createFactoryMsg.messageDisp("0038", fileName) == MsgBoxResult.Ok)
	{
		clsRosenzu.outputReport();
		for (idxTable = 0; idxTable <= courseInfo.Count() - 1; idxTable++)
		{
			this._logTable.Rows.Add(courseInfo[idxTable].CrsCd.ToString(),;
			courseInfo[idxTable].Teiki_KikakuKbn.ToString() +;
			courseInfo[idxTable].Year.ToString() +;
			courseInfo[idxTable].Season.ToString() +;
			courseInfo[idxTable].CrsCd.ToString() +;
			courseInfo[idxTable].InvalidFlg.ToString(),;
			fileName());
	}
}
						}
						else
{
	//PDF出力
	clsRosenzu.outputReport();
	for (idxTable = 0; idxTable <= courseInfo.Count() - 1; idxTable++)
	{
		this._logTable.Rows.Add(courseInfo[idxTable].CrsCd.ToString(),;
		courseInfo[idxTable].Teiki_KikakuKbn.ToString() +;
		courseInfo[idxTable].Year.ToString() +;
		courseInfo[idxTable].Season.ToString() +;
		courseInfo[idxTable].CrsCd.ToString() +;
		courseInfo[idxTable].InvalidFlg.ToString(),;
		fileName());
}
						}
						break;
						
				}
			}
			
		}
		else if (courseInfo[0].Teiki_KikakuKbn == System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel))
{

	for (idx = 0; idx <= GetValues(typeof(_KikakuTravelBuForm)).Length - 1; idx++)
	{
		//※拡張性を考慮し定期観光部と同じ手法を用いる
		switch (idx)
		{

			case (int)_KikakuTravelBuForm._tateTable:
				fileName = File_Kikaku + "_" + getEnumAttrValue(_KikakuTravelBuForm._tateTable) + "_宿泊_" + outputTime.ToString("yyyyMMddHHmm") + ".pdf";
				fullFileName = System.Convert.ToString(System.IO.Path.Combine(folderPath, fileName));

				idxArray = 0;
				for (idxCourse = 0; idxCourse <= courseInfo.Length - 1; idxCourse++)
				{
					if (courseInfo[idxCourse].CrsKind2.Equals(System.Convert.ToString(FixedCd.CrsKind2.stay)) == true)
					{
						Array.Resize(ref tateInfo, idxArray + 1);
						tateInfo[idxArray] = courseInfo[idxCourse];
						idxArray++;
					}
				}

				if (ReferenceEquals(tateInfo, null) == false)
				{

					clsTateTable_Stay = new TateTable_Stay();
					clsTateTable_Stay.CrsMasterEntity = crsMasterEntity;
					clsTateTable_Stay.OutFile = fileName;
					clsTateTable_Stay.OutSakiFolder = folderPath;
					clsTateTable_Stay.BfGamen = BfGamenType.crsSearch;
					clsTateTable_Stay.TaisyoCrsInfo = tateInfo;
					//UPD-20130617-6月運用サポート-出力単位の変更↓
					if (rdoOutKeisikiPDF_CrsEvery.Checked == true)
					{
						clsTateTable_Stay.FormProcessKind = FormProcessType.pDF_CrsEvery;
					}
					else
					{
						clsTateTable_Stay.FormProcessKind = FormProcessType.pDF_AllCrs;
					}
					//UPD-20130617-6月運用サポート-出力単位の変更↑

					if (System.IO.File.Exists(fullFileName) == true)
					{
						//TODO:共通変更対応
						//Ifメッセージ出力.messageDisp("0038",fileName)= MsgBoxResult.Ok Then
						if (createFactoryMsg.messageDisp("0038", fileName) == MsgBoxResult.Ok)
						{
							//PDF出力
							clsTateTable_Stay.outputReport();
							for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
							{
								this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
								tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
								tateInfo[idxTable].Year.ToString() +;
								tateInfo[idxTable].Season.ToString() +;
								tateInfo[idxTable].CrsCd.ToString() +;
								tateInfo[idxTable].InvalidFlg.ToString(),;
								fileName());
}
							}
						}
						else
{
	//PDF出力
	clsTateTable_Stay.outputReport();
	for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
	{
		this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
		tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
		tateInfo[idxTable].Year.ToString() +;
		tateInfo[idxTable].Season.ToString() +;
		tateInfo[idxTable].CrsCd.ToString() +;
		tateInfo[idxTable].InvalidFlg.ToString(),;
		fileName());
}
						}
						
					}
					
					//*** たて表(日帰り) ***
					fileName = File_Kikaku + "_" + getEnumAttrValue(_TeikiKankoBuForm._tateTable) + "_" + outputTime.ToString("yyyyMMddHHmm") + ".pdf";
fullFileName = System.Convert.ToString(System.IO.Path.Combine(folderPath, fileName));

tateInfo = null;
idxArray = 0;
for (idxCourse = 0; idxCourse <= courseInfo.Length - 1; idxCourse++)
{
	if (courseInfo[idxCourse].CrsKind2.Equals(System.Convert.ToString(FixedCd.CrsKind2.stay)) == false)
	{
		Array.Resize(ref tateInfo, idxArray + 1);
		tateInfo[idxArray] = courseInfo[idxCourse];
		idxArray++;
	}
}

if (ReferenceEquals(tateInfo, null) == false)
{

	clsTateTable_Higaeri = new TateTable_Higaeri();
	clsTateTable_Higaeri.CrsMasterEntity = crsMasterEntity;
	clsTateTable_Higaeri.OutFile = fileName;
	clsTateTable_Higaeri.OutSakiFolder = folderPath;
	clsTateTable_Higaeri.BfGamen = BfGamenType.crsSearch;
	clsTateTable_Higaeri.TaisyoCrsInfo = tateInfo;
	//UPD-20130617-6月運用サポート-出力単位の変更↓
	if (rdoOutKeisikiPDF_CrsEvery.Checked == true)
	{
		clsTateTable_Higaeri.FormProcessKind = FormProcessType.pDF_CrsEvery;
	}
	else
	{
		clsTateTable_Higaeri.FormProcessKind = FormProcessType.pDF_AllCrs;
	}


	if (System.IO.File.Exists(fullFileName) == true)
	{
		//TODO:共通変更対応
		//Ifメッセージ出力.messageDisp("0038",fileName)= MsgBoxResult.Ok Then
		if (createFactoryMsg.messageDisp("0038", fileName) == MsgBoxResult.Ok)
		{
			//PDF出力
			clsTateTable_Higaeri.outputReport(Form_RegularNum.Mode.kikaku);
			for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
			{
				this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
				tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
				tateInfo[idxTable].Year.ToString() +;
				tateInfo[idxTable].Season.ToString() +;
				tateInfo[idxTable].CrsCd.ToString() +;
				tateInfo[idxTable].InvalidFlg.ToString(),;
				fileName());
		}
	}
}
else
{
	//PDF出力
	clsTateTable_Higaeri.outputReport(Form_RegularNum.Mode.kikaku);
	for (idxTable = 0; idxTable <= tateInfo.Count() - 1; idxTable++)
	{
		this._logTable.Rows.Add(tateInfo[idxTable].CrsCd.ToString(),;
		tateInfo[idxTable].Teiki_KikakuKbn.ToString() +;
		tateInfo[idxTable].Year.ToString() +;
		tateInfo[idxTable].Season.ToString() +;
		tateInfo[idxTable].CrsCd.ToString() +;
		tateInfo[idxTable].InvalidFlg.ToString(),;
		fileName());
}
						}
						
					}
					break;
					
			}
		}
		
	}
	else
{
	return;
}
	
}
catch (Exception ex)
{
	throw (ex);
}

}

/// <summary>
/// 出力対象のコース情報設定処理
/// </summary>
/// <param name="courseInfo"></param>
/// <param name="idxRow"></param>
/// <remarks></remarks>
private void setCourseInfo(ref CrsMasterKeyKoumoku[] courseInfo, int idxRow)
{

	int idxArray = 0;

	idxArray = courseInfo.Length - 1;

	object with_1 = this.grdList.Rows(idxRow);
	courseInfo[idxArray].Teiki_KikakuKbn = with_1.Item(_GridPhysicsName.TEIKI_KIKAKU_KBN.ToString()).ToString();
	courseInfo[idxArray].CrsCd = with_1.Item(_GridPhysicsName.CRS_CD.ToString()).ToString();
	courseInfo[idxArray].KaiteiDay = with_1.Item(_GridPhysicsName.KAITEI_DATE.ToString()).ToString();
	courseInfo[idxArray].Year = with_1.Item(_GridPhysicsName.CRS_YEAR.ToString()).ToString();
	courseInfo[idxArray].Season = with_1.Item(_GridPhysicsName.SEASON.ToString()).ToString();
	courseInfo[idxArray].InvalidFlg = System.Convert.ToInt32(CommonProcess.Nvl(with_1.Item(_GridPhysicsName.INVALID_FLG.ToString()), 0));
	courseInfo[idxArray].CrsName = with_1.Item(_GridPhysicsName.CRS_NAME.ToString()).ToString();
	courseInfo[idxArray].Season_DisplayFor = with_1.Item(_GridPhysicsName.SEASON_DISPLAY.ToString()).ToString();
	courseInfo[idxArray].PamphIraiDay_DisplayFor = with_1.Item(_GridPhysicsName.PAMPH_IRAI_DATE.ToString()).ToString();
	courseInfo[idxArray].CrsKind1 = with_1.Item(_GridPhysicsName.CRS_KIND_1.ToString()).ToString();
	courseInfo[idxArray].CrsKind2 = with_1.Item(_GridPhysicsName.CRS_KIND_2.ToString()).ToString();

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
	F6Key_Visible = false; // F6:未使用
	F7Key_Visible = false; // F7:未使用
	F8Key_Visible = false; // F8:未使用
	F9Key_Visible = false; // F9:未使用
	F10Key_Visible = true; // F10:登録
	F11Key_Visible = false; // F11:未使用
	F12Key_Visible = false; // F12:未使用
}
#endregion

}