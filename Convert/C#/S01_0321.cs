//using System.Enum;
using Microsoft.VisualBasic.CompilerServices;


/// <summary>
/// コース台帳作成指示
/// </summary>
public class S01_0321
{

	#region  定数/変数

	//処理結果
	private const string ProcessResult_Normal = "正常終了";
	private const string ProcessResult_NoData = "台帳展開済";
	private const string ProcessResult_Error = "エラー";
	private const string ProcessResult_LockChu = "ロック中";
	private const string ProcessResult_NotCovered = "対象外";
	private const string ProcessResult_DeleteAlready = "論理削除済";

	private const string ProcessResult_NotProcMonth = "処理月対象外";

	private const string ProcessResult_NotComplete = "造成未完了";
	private const string ProcessResult_NotUketukeStartDay = "受付開始日無し";
	private const string ProcessResult_NotCalender = "運行日無し";
	private const string ProcessResult_NotCalenderDiaNg = "運行日設定不備";
	private const string ProcessResult_NotCost = "原価未有";
	private const string ProcessResult_NotCharge = "料金設定無し";
	private const string ProcessResult_NotHotel = "ホテル設定無し";


	// 台帳作成実行フラグ (True:実行済 / False:未実行)
	private bool _executeLeadger = false;

	// ダイヤ最大数
	private const int DiaMaxCount = 100;

	// from年月日 (初期値)
	private const string FromInitDate = "20000101";
	// to年月日 (初期値)
	private const string ToInitDate = "20991231";

	// コース種別
	private CrsKinds _crsKinds;

	#endregion

	#region  列挙

	/// <summary>
	/// コース一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum CrsGridCols : int
	{
		[Value("No")] no = 0,
		[Value("コースコード")] crs_Cd,
		[Value("年")] crs_year,
		[Value("季(非表示)")] season,
		[Value("季")] season_Disp,
		[Value("コース名")] crs_Name,
		[Value("台帳作成日")] ledger_Create_Date,
		[Value("最終作成月")] last_Month_Teiki,
		[Value("処理結果")] result,
		[Value("削除日(非表示)")] delete_date
	}

	// カレンダー日付取得区分
	private enum CalenderYmdGetKbn : int
	{
		min,
		max
	}

	#endregion

	#region  プロパティ

	// 選択されたデータ (コース検索一覧と同じ列)
	public DataTable SelectDataTable
	{

#endregion

		#region  イベント

		#region  画面

		/// <summary>
		/// 画面ロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
	private void frm_Load(object sender, System.EventArgs e)
	{

		try
		{

			// コース種別
			_crsKinds = getCrsKinds(SelectDataTable.Rows(0).Item(S01_0301.CrsList_Koumoku.crsKind1 - 1).ToString()
				, SelectDataTable.Rows(0).Item(S01_0301.CrsList_Koumoku.crsKind2 - 1).ToString()
				, SelectDataTable.Rows(0).Item(S01_0301.CrsList_Koumoku.crsKind3 - 1).ToString());

			//コントロールの初期化
			this.setControlInitialize();

			//グリッド初期化
			this.initializeGrid();

			//コース検索にて選択されたコース情報を一覧に表示
			this.setCourseList(SelectDataTable);

			this.grdCrsList.Focus();

		}
		catch (OracleException ex)
		{
			// Oracleエラー[{1}]が発生しました。
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());
		}
		catch (Exception)
		{
			// システムエラーが発生しました。
			createFactoryMsg.messageDisp("9001");
		}
	}

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void frm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
	{

		// 実行済か判定
		if (_executeLeadger == true)
		{
			this.DialogResult = DialogResult.OK;
		}
		else
		{
			this.DialogResult = DialogResult.Cancel;
		}

		this.Owner.Show();
		this.Owner.Activate();

	}

	#endregion

	#region  ボタン

	#region  F2:戻るボタン

	/// <summary>
	/// F2:戻るボタンクリック
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{

		this.Close();

	}

	#endregion

	#region  F10:実行ボタン

	/// <summary>
	/// F10:実行ボタンクリック
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF10_ClickOrgProc()
	{

		int normalKensu = 0; //正常件数
		int errorKensu = 0; //エラー件数
		CrsMasterKeyKoumoku[] lockInfo = null; //ロック情報()

		try
		{
			//カーソルを処理中に変更
			this.Cursor = Cursors.WaitCursor;

			// 処理日を取得
			DateTime nowDate = System.Convert.ToDateTime(getDateTime());

			// エラークリア
			clearErrItem();

			//入力チェック
			if (checkInputData(nowDate) == false)
			{
				goto endOfTry;
			}

			// 最終作成月, from日付,to日付を求める
			string lastMonth = string.Empty;
			string fromDate = string.Empty;
			string toDate = string.Empty;
			getDateBetween(nowDate, ref lastMonth, ref fromDate, ref toDate);

			// データチェック ※エラーでも処理は続行 (処理結果に表示)
			checkExecData(_crsKinds, SelectDataTable, nowDate, fromDate, toDate);

			//コースロック
			if (this.lockCourseMst(ref lockInfo) == false)
			{
				this.relockCourseMst(lockInfo);

				// エラーが発生しています。結果を確認してください。
				createFactoryMsg.messageDisp("0033");

				goto endOfTry;
			}

			//実行確認メッセージ
			if (dispExecMessage() == false)
			{
				relockCourseMst(lockInfo);
				goto endOfTry;
			}

			//台帳作成処理
			bool ret = executeToLeadgerMaster(SelectDataTable, nowDate, lastMonth, fromDate, toDate);
			if (ret == true)
			{
				// 台帳作成 (一回でも正常終了であれば True をセット)
				_executeLeadger = ret;
			}

		}
		catch (OracleException ex)
		{
			// Oracleエラー[{1}]が発生しました。
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());

		}
		catch (Exception)
		{
			// システムエラーが発生しました。
			createFactoryMsg.messageDisp("9001");

		}
		finally
		{
			//レコードロック解除
			this.relockCourseMst(lockInfo);

			//カーソルを戻す
			this.Cursor = Cursors.Default;
		}
	endOfTry:
		1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.

	}

	#endregion

	#endregion

	#endregion

	#region  メソッド

	#region  初期化

	/// <summary>
	/// コースグリッドの初期設定
	/// </summary>
	/// <remarks></remarks>
	private void initializeGrid()
	{

		object with_1 = this.grdCrsList;
		//№
		with_1.Cols(CrsGridCols.no).Name = CrsGridCols.no.ToString().ToUpper();
		with_1.Cols(CrsGridCols.no).Caption = getEnumAttrValue(CrsGridCols.no);
		with_1.Cols(CrsGridCols.no).Width = 40;
		//コースコード
		with_1.Cols(CrsGridCols.crs_Cd).Name = CrsGridCols.crs_Cd.ToString().ToUpper();
		with_1.Cols(CrsGridCols.crs_Cd).Caption = getEnumAttrValue(CrsGridCols.crs_Cd);
		with_1.Cols(CrsGridCols.crs_Cd).Width = 120;
		//年
		with_1.Cols(CrsGridCols.crs_year).Name = CrsGridCols.crs_year.ToString().ToUpper();
		with_1.Cols(CrsGridCols.crs_year).Caption = getEnumAttrValue(CrsGridCols.crs_year);
		with_1.Cols(CrsGridCols.crs_year).Width = 50;
		//季（非表示）
		with_1.Cols(CrsGridCols.season).Name = CrsGridCols.season.ToString().ToUpper();
		with_1.Cols(CrsGridCols.season).Caption = getEnumAttrValue(CrsGridCols.season);
		with_1.Cols(CrsGridCols.season).Visible = false;
		//季[名称]（表示用）
		with_1.Cols(CrsGridCols.season_Disp).Name = CrsGridCols.season_Disp.ToString().ToUpper();
		with_1.Cols(CrsGridCols.season_Disp).Caption = getEnumAttrValue(CrsGridCols.season_Disp);
		with_1.Cols(CrsGridCols.season_Disp).Width = 100;
		//コース名
		with_1.Cols(CrsGridCols.crs_Name).Name = CrsGridCols.crs_Name.ToString().ToUpper();
		with_1.Cols(CrsGridCols.crs_Name).Caption = getEnumAttrValue(CrsGridCols.crs_Name);
		with_1.Cols(CrsGridCols.crs_Name).Width = 300;
		//台帳作成日
		with_1.Cols(CrsGridCols.ledger_Create_Date).Name = CrsGridCols.ledger_Create_Date.ToString().ToUpper();
		with_1.Cols(CrsGridCols.ledger_Create_Date).Caption = getEnumAttrValue(CrsGridCols.ledger_Create_Date);
		with_1.Cols(CrsGridCols.ledger_Create_Date).Width = 90;
		//最終作成月(定期)
		with_1.Cols(CrsGridCols.last_Month_Teiki).Name = CrsGridCols.last_Month_Teiki.ToString().ToUpper();
		with_1.Cols(CrsGridCols.last_Month_Teiki).Caption = getEnumAttrValue(CrsGridCols.last_Month_Teiki);
		with_1.Cols(CrsGridCols.last_Month_Teiki).Width = 90;
		//結果
		with_1.Cols(CrsGridCols.result).Name = CrsGridCols.result.ToString().ToUpper();
		with_1.Cols(CrsGridCols.result).Caption = getEnumAttrValue(CrsGridCols.result);
		with_1.Cols(CrsGridCols.result).Width = 200;
		//削除日 (非表示)
		with_1.Cols(CrsGridCols.delete_date).Name = CrsGridCols.delete_date.ToString().ToUpper();
		with_1.Cols(CrsGridCols.delete_date).Caption = getEnumAttrValue(CrsGridCols.delete_date);
		with_1.Cols(CrsGridCols.delete_date).Width = 90;
		with_1.Cols(CrsGridCols.delete_date).Visible = false;

	}

	/// <summary>
	/// コース一覧表示
	/// </summary>
	/// <remarks></remarks>
	private void setCourseList(DataTable selectData)
	{

		// データコピー
		DataTable grdData = new DataTable();
		grdData = selectData.Copy;

		// カラム追加
		grdData.Columns.Add(CrsGridCols.no.ToString().ToUpper());
		grdData.Columns.Add(CrsGridCols.result.ToString().ToUpper());

		// 追加した情報の初期設定
		for (int row = 0; row <= grdData.Rows.Count - 1; row++)
		{
			// NO
			grdData.Rows(row).Item[CrsGridCols.no.ToString().ToUpper()] = (row + 1).ToString();
			// 処理結果
			grdData.Rows(row).Item[CrsGridCols.result.ToString().ToUpper()] = string.Empty;
		}

		// データを一覧へ反映
		this.grdCrsList.DataSource = grdData;

	}

	/// <summary>
	/// コントロールの初期化
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitialize()
	{

		// 作成月(定期)
		if (_crsKinds.teikiKankou == true)
		{
			// [定期観光]の場合

			// システム処理月を初期表示
			CommonDaProcess commonDaProcess = new CommonDaProcess();
			this.datLastMonth.Value = commonDaProcess.getServerSysDate();
			datLastMonth.Enabled = true;

		}
		else
		{
			// [定期観光]以外の場合

			// 使用不可
			this.datLastMonth.Value = null;
			datLastMonth.Enabled = false;
		}

	}

	#endregion

	#region 最終作成月,from日付,to日付を求める
	/// <summary>
	/// 最終作成月,from日付,to日付を求める
	/// </summary>
	/// <param name="nowDate"></param>
	/// <param name="lastMonth"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	private void getDateBetween(DateTime nowDate, ref string lastMonth, ref string fromDate, ref string toDate)
	{

		// [定期観光] 以外
		if (_crsKinds.teikiKankou == false)
		{
			fromDate = FromInitDate;
			toDate = ToInitDate;

			//処理を抜ける
			return;

		}

		// [定期観光]の場合は[画面.作成月]から[最終作成月][開始日、終了日]を求める

		//作成年月 (YYYMM)
		DateTime contorlVallue = System.Convert.ToDateTime(this.datLastMonth.Value);
		lastMonth = string.Concat(System.Convert.ToString(contorlVallue.Year), contorlVallue.Month.ToString().PadLeft(2, '0'));

		if (lastMonth.Equals(nowDate.ToString("yyyyMM")))
		{
			// 当月の場合
			// 当日～当月末

			// from年月日 (当日)
			fromDate = nowDate.ToString("yyyyMMdd");

			// to年月日 (YYYYMM & 月末)
			toDate = System.Convert.ToString(new DateTime(nowDate.Year, nowDate.Month, DateTime.DaysInMonth(nowDate.Year, nowDate.Month)).ToString("yyyyMMdd"));

		}
		else
		{
			// 未来月の場合 (過去日はなし ※エラーとするため)
			// 未来月/01～未来月/末

			// from年月日 (画面.年月 & 月初)
			fromDate = lastMonth + "01";

			// to年月日 (YYYYMM & 月末)
			toDate = lastMonth + DateTime.DaysInMonth(contorlVallue.Year, contorlVallue.Month).ToString("00");

		}

	}
	#endregion

	#region エラークリア
	/// <summary>
	/// エラークリア
	/// </summary>
	private void clearErrItem()
	{

		// 作成月
		this.datLastMonth.ExistError = false;

		// 一覧.処理結果をクリアする
		for (int row = this.grdCrsList.Rows.Fixed; row <= this.grdCrsList.Rows.Count - 1; row++)
		{

			// "正常終了" 以外をクリアする。正常終了はクリアしない (再実行不可とするため)
			if (this.grdCrsList.GetData(row, CrsGridCols.result).ToString().Equals(ProcessResult_Normal) == false)
			{
				this.grdCrsList.SetData(row, CrsGridCols.result, string.Empty);
			}

		}

	}
	#endregion

	#region  入力チェック

	/// <summary>
	/// 入力データのチェックを行う
	/// </summary>
	/// <param name="nowDate"></param>
	/// <remarks></remarks>
	private bool checkInputData(DateTime nowDate)
	{

		// 定期観光 以外は処理を抜ける
		if (_crsKinds.teikiKankou == false)
		{
			return true;
		}

		//必須入力チェック
		if (ReferenceEquals(this.datLastMonth.Value, null))
		{
			this.datLastMonth.ExistError = true;
			this.datLastMonth.Focus();
			createFactoryMsg.messageDisp("0014", "作成月");
			return false;
		}

		// 作成月チェック
		// 画面.作成月 < 処理月(システム日付) はエラー
		DateTime contorlVallue = System.Convert.ToDateTime(this.datLastMonth.Value);

		if (string.Compare(contorlVallue.ToString("yyyyMM"), nowDate.ToString("yyyyMM")) < 0)
		{
			this.datLastMonth.ExistError = true;
			this.datLastMonth.Focus();
			createFactoryMsg.messageDisp("E90_006", "過去月", "指定");
			return false;
		}

		return true;
	}

	#endregion

	#region  データチェック

	/// <summary>
	/// データチェックを行う
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="selectData"></param>
	/// <param name="nowDate"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <remarks></remarks>
	private void checkExecData(CrsKinds crsKinds, DataTable selectData, DateTime nowDate, string fromDate, string toDate)
	{

		string resultDisp = string.Empty;
		CrsMasterKeyKoumoku crsKeyInfo = null;
		EntityOperation[] crsMasterEntity = new EntityOperation[Of CrsMasterEntity + 1];

		// 削除データチェック
		this.checkDeleteData();

		// データチェック（各種チェックを行う）
		for (int row = this.grdCrsList.Rows.Fixed; row <= this.grdCrsList.Rows.Count - 1; row++)
		{

			// 処理結果が 空白でない場合はスキップ
			if (string.IsNullOrWhiteSpace(this.grdCrsList.GetData(row, CrsGridCols.result).ToString()) == false)
			{
				continue;
			}

			resultDisp = string.Empty;

			// コースマスタ取得
			crsKeyInfo = getCrsMasterKey(selectData.Rows(row - 1));
			crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA();
			crsMasterEntity = new EntityOperation(Of crsMasterEntity);
			if (clsCrsMasterOperation_DA.getBasicInfoEntity(crsKeyInfo, crsMasterEntity) == false)
			{
				resultDisp = ProcessResult_Error + " (コースマスタなし) ";
			}

			if (string.IsNullOrWhiteSpace(resultDisp))
			{
				// データなし
				if (crsMasterEntity.EntityData.Length < 1)
				{
					resultDisp = ProcessResult_Error + " (コースマスタなし) ";
				}
			}

			if (string.IsNullOrWhiteSpace(resultDisp))
			{
				// 定期企画区分が 空白の場合
				if (string.IsNullOrWhiteSpace(System.Convert.ToString(crsMasterEntity.EntityData(0).Teiki_KikakuKbn.Value)))
				{
					resultDisp = ProcessResult_Error + " (コースマスタなし) ";
				}
			}


			if (string.IsNullOrWhiteSpace(resultDisp))
			{
				// データチェック（各種チェックを行う）
				resultDisp = checkExecDataItem(crsKinds, selectData.Rows(row - 1), crsMasterEntity.EntityData(0), nowDate, fromDate, toDate);
			}

			// 処理結果へ表示
			this.grdCrsList.SetData(row, CrsGridCols.result, resultDisp);

		}

	}

	#endregion

	#region データチェック（各種チェックを行う）
	/// <summary>
	/// データチェック（各種チェックを行う）
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="dr"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="nowDate"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private string checkExecDataItem(CrsKinds crsKinds, DataRow dr, CrsMasterEntity crsMasterEntity, DateTime nowDate, string fromDate, string toDate)
	{

		// コース種別チェック
		//  [キャピタル]はエラー
		if (checkCourseType(crsKinds) == false)
		{
			return ProcessResult_NotCovered;
		}

		// ステータスチェック
		if (checkCourseStatus(crsMasterEntity) == false)
		{
			return ProcessResult_NotComplete;
		}

		// 受付開始日チェック
		if (checkUketukeStartDay(crsKinds, crsMasterEntity) == false)
		{
			return ProcessResult_NotUketukeStartDay;
		}

		// 運行日チェック
		if (checkCalender(crsKinds, crsMasterEntity, fromDate, toDate) == false)
		{
			return ProcessResult_NotCalender;
		}

		// 運行日最大日チェック [企画のみ]
		if (checkCalenderMaxYmd(crsKinds, crsMasterEntity, nowDate) == false)
		{
			return ProcessResult_NotCalender;
		}

		// 運行日設定不備チェック
		if (checkCalenderDia(crsKinds, crsMasterEntity, fromDate, toDate) == false)
		{
			return ProcessResult_NotCalenderDiaNg;
		}

		// 原価有無チェック
		if (checkCost(crsKinds, crsMasterEntity, fromDate) == false)
		{
			return ProcessResult_NotCost;
		}

		// 原価有無チェック (出発時キャリア)
		if (checkCostCarrier(crsKinds, crsMasterEntity, fromDate) == false)
		{
			return ProcessResult_NotCost;
		}

		// 料金チェック
		if (checkCharge(crsKinds, crsMasterEntity) == false)
		{
			return ProcessResult_NotCharge;
		}

		// ホテルチェック
		if (checkHotel(crsKinds, crsMasterEntity) == false)
		{
			return ProcessResult_NotHotel;
		}

		return string.Empty;
	}
	#endregion

	#region コース種別チェック
	/// <summary>
	/// コース種別チェック
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <returns></returns>
	private bool checkCourseType(CrsKinds crsKinds)
	{

		// [定期観光 または 企画旅行] は エラー ※[定期観光 または 企画旅行] 以外 は Falseを返す
		return crsKinds.teikiKikaku;

	}
	#endregion

	#region ステータスチェック
	/// <summary>
	/// ステータスチェック [executeRendoProcess]
	/// </summary>
	/// <param name="crsMasterEntity"></param>
	/// <returns></returns>
	private bool checkCourseStatus(CrsMasterEntity crsMasterEntity)
	{

		// パンフ作成完了 以外はエラー
		return crsMasterEntity.CrsStatus.Value.Equals(System.Convert.ToString(CrsStatusType.pamphCreateDone));
	}
	#endregion

	#region 受付開始日チェック
	/// <summary>
	/// 受付開始日チェック [chkOther]
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <returns></returns>
	private bool checkUketukeStartDay(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity)
	{

		// [定期観光]
		if (crsKinds.teikiKankou == true)
		{

			// 受付開始 (ヶ月前)  Nullの場合はエラー
			if (ReferenceEquals(crsMasterEntity.UketukeStart_Kagetumae.Value, null))
			{
				return false;
			}

			return string.IsNullOrWhiteSpace(System.Convert.ToString(crsMasterEntity.UketukeStart_Kagetumae.Value)) == false;

		}

		// [企画旅行]
		if (crsKinds.kikakuTravel == true)
		{

			// 受付開始日  Nullの場合はエラー
			if (ReferenceEquals(crsMasterEntity.UketukeStartDay.Value, null))
			{
				return false;
			}

			return string.IsNullOrWhiteSpace(System.Convert.ToString(crsMasterEntity.UketukeStartDay.Value)) == false;
		}

		return true;
	}
	#endregion

	#region 運行日チェック（料金カレンダー存在チェック）
	/// <summary>
	/// 運行日チェック（料金カレンダー存在チェック）[checkCalendar]
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private bool checkCalender(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity, string fromDate, string toDate)
	{

		// 料金カレンダーが存在するかチェック
		foreach (ChargeCalendarEntity chargeCalendarEntity in crsMasterEntity.ChargeCalendarEntity.EntityData)
		{

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeCalendarEntity.Teiki_KikakuKbn.Value)))
			{
				continue;
			}

			// [定期観光]の場合、日付範囲外はスキップ
			if (crsKinds.teikiKankou == true)
			{

				// 指定した日付が日付範囲か
				if (isRangeCreateDate(fromDate, toDate, System.Convert.ToString(chargeCalendarEntity.Ymd.Value)) == false)
				{
					continue;
				}

			}

			// データあり
			return true;

		}

		return false;
	}
	#endregion

	#region 運行日最大日チェック [企画]
	/// <summary>
	/// 運行日最大日チェック [企画][chkCost]
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="nowDate"></param>
	/// <returns></returns>
	private bool checkCalenderMaxYmd(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity, DateTime nowDate)
	{

		// [企画]以外は処理を抜ける
		if (crsKinds.kikakuTravel == false)
		{
			return true;
		}

		// 運行最大日を取得
		string maxYmd = getCalenderYmd(CalenderYmdGetKbn.max, crsMasterEntity);

		// 処理日以降の運行日あり
		if (string.Compare(nowDate.ToString("yyyyMMdd"), maxYmd) <= 0)
		{
			return true;
		}

		return false;
	}
	#endregion

	#region 運行日設定不備チェック（料金カレンダーダイヤ設定チェック）
	/// <summary>
	/// 運行日設定不備チェック（料金カレンダーダイヤ設定チェック）[chkSetRankAndDia][chkSetRankAndDia]
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private bool checkCalenderDia(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity, string fromDate, string toDate)
	{

		const int chkOkCount = 2;
		int chkCount = 0;

		DataTable chargeCalendar = entityToDataTable(crsMasterEntity.ChargeCalendarEntity);

		// 料金カレンダーにランク or ダイヤが設定されているか
		foreach (ChargeCalendarEntity chargeCalendarEntity in crsMasterEntity.ChargeCalendarEntity.EntityData)
		{

			// チェックカウント初期化
			chkCount = 0;

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeCalendarEntity.Teiki_KikakuKbn.Value)))
			{
				continue;
			}

			// [定期観光]の場合、日付範囲外はスキップ
			if (crsKinds.teikiKankou == true)
			{

				// 指定した日付が日付範囲か
				if (isRangeCreateDate(fromDate, toDate, System.Convert.ToString(chargeCalendarEntity.Ymd.Value)) == false)
				{
					continue;
				}

			}

			// ランク設定あり
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeCalendarEntity.ChargeRank.Value)) == false)
			{
				chkCount++;
			}

			// キーに該当するデータを抽出
			string whereStr = string.Empty;
			DataRow[] dr;
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.Teiki_KikakuKbn.PhysicsName, chargeCalendarEntity.Teiki_KikakuKbn.Value);
			whereStr += " AND ";
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.CrsCd.PhysicsName, chargeCalendarEntity.CrsCd.Value);
			whereStr += " AND ";
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.KaiteiDay.PhysicsName, chargeCalendarEntity.KaiteiDay.Value);
			whereStr += " AND ";
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.Year.PhysicsName, chargeCalendarEntity.Year.Value);
			whereStr += " AND ";
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.Season.PhysicsName, chargeCalendarEntity.Season.Value);
			whereStr += " AND ";
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeCalendarEntity.InvalidFlg.Value.ToString())))
			{
				whereStr += string.Format("{0}= {1} ", chargeCalendarEntity.InvalidFlg.PhysicsName, 0);
			}
			else
			{
				whereStr += string.Format("{0}= {1} ", chargeCalendarEntity.InvalidFlg.PhysicsName, chargeCalendarEntity.InvalidFlg.Value);
			}
			whereStr += " AND ";
			whereStr += string.Format("{0}='{1}'", chargeCalendarEntity.Ymd.PhysicsName, chargeCalendarEntity.Ymd.Value);
			dr = chargeCalendar.Select(whereStr);
			if ((ReferenceEquals(dr, null)) ||)
			{
				dr.Length(< 1);
				continue;
			}

			// ダイヤ設定あり
			string colId = System.Convert.ToString(chargeCalendarEntity.DiaLineNo1.PhysicsName.TrimEnd);
			colId = colId.Substring(0, colId.Length - 1);
			for (int col = 1; col <= DiaMaxCount; col++)
			{

				string colName = colId + col.ToString();
				if (string.IsNullOrWhiteSpace(dr[0][System.Convert.ToInt32(colName)].ToString()) == false)
				{
					chkCount++;
					break;
				}
			}

			// ランク 及び、ダイヤ設定あり
			if (chkCount.Equals(chkOkCount))
			{
				return true;
			}
		}

		return false;
	}
	#endregion

	#region 原価有無チェック
	/// <summary>
	/// 原価有無チェック
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="fromDate"></param>
	/// <returns></returns>
	private bool checkCost(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity, string fromDate)
	{

		bool costUmFlg = false;
		bool costflg = false;

		string minYmd = fromDate;

		if (crsKinds.kikakuTravel == true)
		{
			//[企画]の場合、料金カレンダー.最小日を取得 (from日)
			minYmd = getCalenderYmd(CalenderYmdGetKbn.min, crsMasterEntity);
		}

		// 降車ヶ所
		foreach (KoshakashoEntity koshakashoEntity in crsMasterEntity.KoshakashoEntity.EntityData)
		{

			costUmFlg = false;
			costflg = false;

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(koshakashoEntity.Teiki_KikakuKbn.Value)))
			{
				continue;
			}

			// 原価有無が "1" 以外の場合はスキップ
			if (nnvl_int(koshakashoEntity.CostUmu.Value).Equals(1) == false)
			{
				continue;
			}

			// 降車ヶ所原価
			costflg = false;
			foreach (CostMaster_KoshakashoEntity costEnt in koshakashoEntity.CostMaster_KoshakashoEntity.EntityData)
			{

				// 定期企画区分が 空白の場合はスキップ
				if (string.IsNullOrWhiteSpace(System.Convert.ToString(costEnt.Teiki_KikakuKbn.Value)))
				{
					continue;
				}

				// 適用日原価あり
				if (costEnt.ApplicationStartDay.Value <= minYmd)
				{
					costflg = true;
				}

			}

			// 降車ヶ所キャリア原価
			if (costflg == false)
			{

				foreach (CostMaster_CarrierEntity costCarrierEnt in koshakashoEntity.CostMaster_CarrierEntity.EntityData)
				{

					// 定期企画区分が 空白の場合はスキップ
					if (string.IsNullOrWhiteSpace(System.Convert.ToString(costCarrierEnt.Teiki_KikakuKbn.Value)))
					{
						continue;
					}

					// 適用日原価あり
					if (costCarrierEnt.ApplicationStartDay.Value <= minYmd)
					{
						costflg = true;
					}

				}
			}

			// 適用日原価なし
			if (costflg == false)
			{
				return false;
			}

		}

		return true;
	}
	#endregion


	#region 原価有無チェック（出発時キャリア）
	/// <summary>
	/// 原価有無チェック（出発時キャリア）
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <param name="fromDate"></param>
	/// <returns></returns>
	private bool checkCostCarrier(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity, string fromDate)
	{

		bool costUmFlg = false;
		bool costflg = false;

		string minYmd = fromDate;

		if (crsKinds.kikakuTravel == true)
		{
			//[企画]の場合、料金カレンダー.最小日を取得 (from日)
			minYmd = getCalenderYmd(CalenderYmdGetKbn.min, crsMasterEntity);
		}

		// 出発時キャリア
		foreach (CostMaster_CarrierEntity costCarrierEnt in crsMasterEntity.CostMaster_CarrierEntity.EntityData)
		{

			costflg = false;

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(costCarrierEnt.Teiki_KikakuKbn.Value)))
			{
				continue;
			}

			// 適用日原価あり
			if (costCarrierEnt.ApplicationStartDay.Value <= minYmd)
			{
				costflg = true;
			}

			// 適用日原価なし
			if (costflg == false)
			{
				return false;
			}

		}

		return true;
	}
	#endregion

	#region 料金有無チェック
	/// <summary>
	/// 料金有無チェック
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <returns></returns>
	private bool checkCharge(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity)
	{

		bool chargeUmFlg = false;
		bool chargeflg = false;

		// 料金
		foreach (ChargeRankEntity chargeEntity in crsMasterEntity.ChargeRankEntity.EntityData)
		{

			chargeUmFlg = false;
			chargeflg = false;

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeEntity.Teiki_KikakuKbn.Value)))
			{
				continue;
			}
			else
			{
				foreach (ChargeRank_ChargeKbnEntity chargeKbn in chargeEntity.ChargeRankChargeKbnEntity.EntityData)
				{
					if (crsKinds.teiki == true)
					{
						if (nnvl_int(chargeKbn.Charge.Value) > 0)
						{
							chargeflg = true;
						}
					}
					else
					{
						if (crsKinds.stay == true)
						{
							if (nnvl_int(chargeKbn.Charge1.Value) > 0)
							{
								chargeflg = true;
							}
							if (nnvl_int(chargeKbn.Charge2.Value) > 0)
							{
								chargeflg = true;
							}
							if (nnvl_int(chargeKbn.Charge3.Value) > 0)
							{
								chargeflg = true;
							}
							if (nnvl_int(chargeKbn.Charge4.Value) > 0)
							{
								chargeflg = true;
							}
							if (nnvl_int(chargeKbn.Charge5.Value) > 0)
							{
								chargeflg = true;
							}
						}
						else
						{
							if (nnvl_int(chargeKbn.Charge1.Value) > 0)
							{
								chargeflg = true;
							}
						}
					}
				}
			}

			if (chargeflg == false)
			{
				return false;
			}
		}

		return true;
	}
	#endregion

	#region ホテルチェック
	/// <summary>
	/// ホテルチェック
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <param name="crsMasterEntity"></param>
	/// <returns></returns>
	private bool checkHotel(CrsKinds crsKinds, CrsMasterEntity crsMasterEntity)
	{

		bool hotelFlg = false;
		CrsMstToLedger_DA crsMstToLedger_DA = new CrsMstToLedger_DA();

		if (crsKinds.stay == true)
		{
			if (crsMstToLedger_DA.checkExistsHotel(crsMasterEntity) == true)
			{
				hotelFlg = true;
			}
		}
		else
		{
			hotelFlg = true;
		}

		return hotelFlg;

	}
	#endregion

	#region 削除データチェック
	/// <summary>
	/// 削除データチェック
	/// </summary>
	private void checkDeleteData()
	{

		string resultDisp = string.Empty;

		// 一覧を参照
		for (int row = this.grdCrsList.Rows.Fixed; row <= this.grdCrsList.Rows.Count - 1; row++)
		{

			// 処理結果が 空白でない場合はスキップ
			if (string.IsNullOrWhiteSpace(this.grdCrsList.GetData(row, CrsGridCols.result).ToString()) == false)
			{
				continue;
			}

			resultDisp = string.Empty;

			// 削除データ
			if (string.IsNullOrWhiteSpace(this.grdCrsList.GetData(row, CrsGridCols.delete_date).ToString()) == false)
			{
				// 削除済み
				string deleteYmd = this.grdCrsList.GetData(row, CrsGridCols.delete_date).ToString();
				resultDisp = string.Format("{0} ({1})", ProcessResult_DeleteAlready, deleteYmd);
			}

			// 処理結果
			this.grdCrsList.SetData(row, CrsGridCols.result, resultDisp);

		}

	}
	#endregion

	#region 指定した日付が日付範囲か
	/// <summary>
	/// 指定した日付が日付範囲か (True:範囲内 / False 範囲外)
	/// </summary>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <param name="tgtDate"></param>
	/// <returns></returns>
	private bool isRangeCreateDate(string fromDate, string toDate, string tgtDate)
	{


		// from日より過去日
		if (string.Compare(tgtDate, fromDate) < 0)
		{
			return false;
		}

		// to日より未来日
		if (string.Compare(toDate, tgtDate) < 0)
		{
			return false;
		}

		// 範囲内
		return true;
	}
	#endregion

	#region 料金カレンダーより指定日を取得する
	/// <summary>
	/// 料金カレンダーより指定日を取得する
	/// </summary>
	/// <param name="calenderYmdGetKbn"></param>
	/// <param name="crsMasterEntity"></param>
	/// <returns></returns>
	private string getCalenderYmd(CalenderYmdGetKbn calenderYmdGetKbn, CrsMasterEntity crsMasterEntity)
	{

		string returnYmd = string.Empty;
		switch (calenderYmdGetKbn)
		{
			case calenderYmdGetKbn.min:
				// 最小日を求めるため、最大値を設定
				returnYmd = ToInitDate;
				break;

			case calenderYmdGetKbn.max:
				// 最大日を求めるため、最小値を設定
				returnYmd = FromInitDate;
				break;

		}

		foreach (ChargeCalendarEntity chargeCalendarEntity in crsMasterEntity.ChargeCalendarEntity.EntityData)
		{

			// 定期企画区分が 空白の場合はスキップ
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(chargeCalendarEntity.Teiki_KikakuKbn.Value)))
			{
				continue;
			}

			switch (calenderYmdGetKbn)
			{
				case calenderYmdGetKbn.min:
					// 最小日を求める
					if (chargeCalendarEntity.Ymd.Value < returnYmd)
					{
						returnYmd = System.Convert.ToString(chargeCalendarEntity.Ymd.Value);
					}
					break;

				case calenderYmdGetKbn.max:
					// 最大日を求める
					if (returnYmd < chargeCalendarEntity.Ymd.Value)
					{
						returnYmd = System.Convert.ToString(chargeCalendarEntity.Ymd.Value);
					}
					break;

			}

		}

		return returnYmd;
	}
	#endregion

	#region  確認メッセージ

	/// <summary>
	/// 実行確認メッセージを表示する
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool dispExecMessage()
	{

		int execKanouLineNum = 0; //実行可能行数
		int execNgLineNum = 0; //実行不可行数
		string msgId = ""; //msgId

		for (int row = this.grdCrsList.Rows.Fixed; row <= this.grdCrsList.Rows.Count - 1; row++)
		{

			if ((string)(getEmptyValue(this.grdCrsList.GetData(row, CrsGridCols.result), string.Empty)) == "")
			{
				// 未処理
				execKanouLineNum++;
			}
			else if (getEmptyValue(this.grdCrsList.GetData(row, CrsGridCols.result), string.Empty) == ProcessResult_Normal)
			{
				// 正常終了
				//  何もしない (カウントアップしない)
			}
			else
			{
				// エラーあり
				execNgLineNum++;
			}

		}

		if (execKanouLineNum == 0 & execNgLineNum == 0)
		{
			// 実行可能なコースがありません。
			createFactoryMsg.messageDisp("0046");
			return false;
		}
		else if (execKanouLineNum == 0 & 0 < execNgLineNum)
		{
			// エラーが発生しています。結果を確認してください。
			createFactoryMsg.messageDisp("0033");
			return false;
		}

		if (execNgLineNum == 0)
		{
			// {1}処理を開始します。よろしいですか？
			msgId = "0031";
		}
		else
		{
			// 一部実行不可ですが、{1}を開始しますか？
			msgId = "0045";
		}

		if (createFactoryMsg.messageDisp(msgId, "台帳作成") != MsgBoxResult.Ok)
		{
			return false;
		}

		return true;

	}

	#endregion

	#region  コースマスタロック

	/// <summary>
	/// コースマスタロック処理
	/// </summary>
	/// <param name="lockInfo"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private bool lockCourseMst(ref CrsMasterKeyKoumoku[] lockInfo)
	{

		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		CrsState retValue = null; //retValue
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報
		string lockUser = ""; //lockUser

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		for (int row = 1; row <= this.grdCrsList.Rows.Count - 1; row++)
		{
			//処理対象（処理結果が空白）のレコード以外はスキップ
			if (System.Convert.ToString(this.grdCrsList.GetData(row, CrsGridCols.result)) != "")
			{
				continue;
			}

			// コースマスタ検索用キー取得
			CrsMasterKeyKoumoku crsKeyInfo = getCrsMasterKey(SelectDataTable.Rows(row - 1));
			// コースマスタ(基本)検索
			retValue = clsCrsLock.ExecuteCourseLockMain(crsKeyInfo, userInfo, lockUser);

			if (retValue == CrsState.lockFailure)
			{
				this.grdCrsList.SetData(row, CrsGridCols.result, ProcessResult_Error);
			}
			else if (retValue == CrsState.lockChu)
			{
				this.grdCrsList.SetData(row, CrsGridCols.result, System.Convert.ToString(ProcessResult_LockChu + "[" + lockUser + "]"));
			}
			else if (retValue == CrsState.lockSuccess)
			{
				if (ReferenceEquals(lockInfo, null) == true)
				{
					lockInfo = new CrsMasterKeyKoumoku[1];
				}
				else
				{
					Array.Resize(ref lockInfo, lockInfo.Length + 1);
				}
				lockInfo[lockInfo.Length - 1] = crsKeyInfo;
			}
		}
		return true;
	}

	#endregion

	#region  コースマスタロック解除

	/// <summary>
	/// コースマスタロック解除処理
	/// </summary>
	/// <param name="lockInfo"></param>
	/// <remarks></remarks>
	private void relockCourseMst(CrsMasterKeyKoumoku[] lockInfo)
	{

		CrsLock clsCrsLock = new CrsLock(); //clsコースロック
		UserInfoManagementKoumoku userInfo = new UserInfoManagementKoumoku(); //ユーザー情報

		if (ReferenceEquals(lockInfo, null) == true)
		{
			return;
		}

		userInfo.UserId = UserInfoManagement.userId;
		userInfo.Client = UserInfoManagement.client;
		userInfo.ProcessId = UserInfoManagement.processId;

		for (int idx = 0; idx <= lockInfo.Length - 1; idx++)
		{
			clsCrsLock.ExecuteCourseLockReleaseMain(lockInfo[idx], userInfo);
		}

	}

	#endregion

	#region 台帳作成
	/// <summary>
	/// 台帳作成
	/// </summary>
	/// <param name="selectData"></param>
	/// <param name="nowDate"></param>
	/// <param name="lastMonth"></param>
	/// <param name="fromDate"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private bool executeToLeadgerMaster(DataTable selectData, DateTime nowDate, string lastMonth, string fromDate, string toDate)
	{
		bool returnValue = false;

		try
		{

			// 台帳作成するコース情報 を コースマスタ（基本）エンティティにセットする
			EntityOperation[] tCourseMstEntity = setSelectDataTableToTCourseMstEntity(selectData, nowDate, lastMonth);

			//----------------------------------------------
			// 台帳作成
			//----------------------------------------------
			CrsMstToLedger_DA crsMstToLedger_DA = new CrsMstToLedger_DA();
			int ret = System.Convert.ToInt32(crsMstToLedger_DA.executeToLeadgerMaster(this.Name, tCourseMstEntity, fromDate, toDate));

			//----------------------------------------------
			// 台帳作成後処理
			//----------------------------------------------
			int registCount = setResultToLeadger(tCourseMstEntity);

			// {1}が正常終了しました。
			int ledgerCount = System.Convert.ToInt32(tCourseMstEntity.EntityData.Length);
			int selectCount = System.Convert.ToInt32(selectData.Rows.Count);

			string dispCount = string.Format(" (作成={0} / 対象={1} / 選択={2}) ", registCount, ledgerCount, selectCount);
			createFactoryMsg.messageDisp("0001", "台帳作成" + dispCount);
			returnValue = true;

		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.ASRendo, this.setTitle, ex.Message);
			// {1}に失敗しました。[{2}]
			createFactoryMsg.messageDisp("0016", "台帳作成", ex.Message);
			return returnValue;

		}

		return returnValue;
	}
	#endregion

	#region 台帳作成するコース情報 を コースマスタ（基本）エンティティにセットする
	/// <summary>
	/// 台帳作成するコース情報 を コースマスタ（基本）エンティティにセットする
	/// </summary>
	/// <param name="selectData"></param>
	/// <param name="nowDate"></param>
	/// <param name="lastMonth"></param>
	/// <returns></returns>
	private EntityOperation[] setSelectDataTableToTCourseMstEntity(DataTable selectData, DateTime nowDate, string lastMonth)
	{
		EntityOperation tCourseMstEntity = new EntityOperation(Of TCourseMstEntity);
		int setRow = 0;

		crsMasterOperation_DA crsMasterOperation_DA = new crsMasterOperation_DA();
		DataTable crsDt = null;

		tCourseMstEntity.clear();

		for (int row = this.grdCrsList.Rows.Fixed; row <= this.grdCrsList.Rows.Count - 1; row++)
		{

			// 結果が空白でない場合はスキップ (何かしらエラーあり)
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.grdCrsList.GetData(row, CrsGridCols.result))) == false)
			{
				continue;
			}

			// コースマスタ(基本)取得
			CrsMasterKeyKoumoku crsKeyInfo = getCrsMasterKey(selectData.Rows(row - 1));
			// コースマスタ(基本)検索
			crsDt = crsMasterOperation_DA.GetDatacrsMaster(crsKeyInfo);

			// コースマスタ(基本)が存在しない場合はスキップ (台帳作成不可)
			if (ReferenceEquals(crsDt, null) ||)
			{
				crsDt.Rows.Count(<= 0);
				this.grdCrsList.SetData(row, CrsGridCols.result, ProcessResult_Error + " (データ不明)");
				continue;
			}

			// エンティティ行追加
			if (tCourseMstEntity.EntityData.Length - 1 < setRow)
			{
				TCourseMstEntity entity = new TCourseMstEntity();
				tCourseMstEntity.add(entity);
			}

			// 受け渡しに必要な値のみ編集している (必要に応じて項目追加可)
			object with_1 = tCourseMstEntity.EntityData(setRow);

			//キー項目
			CrsMasterEntity crsMasterCol = new CrsMasterEntity();
			with_1.TeikiKikakuKbn.Value = crsDt.Rows(0).Item(crsMasterCol.Teiki_KikakuKbn.PhysicsName).ToString();
			with_1.CrsCd.Value = crsDt.Rows(0).Item(crsMasterCol.CrsCd.PhysicsName).ToString();
			with_1.KaiteiDate.Value = crsDt.Rows(0).Item(crsMasterCol.KaiteiDay.PhysicsName).ToString();
			with_1.CrsYear.Value = crsDt.Rows(0).Item(crsMasterCol.Year.PhysicsName).ToString();
			with_1.Season.Value = crsDt.Rows(0).Item(crsMasterCol.Season.PhysicsName).ToString();
			with_1.InvalidFlg.Value = crsDt.Rows(0).Item(crsMasterCol.InvalidFlg.PhysicsName).ToString();

			// 予約システム連動日付
			with_1.YoyakuSystemRendoDate.Value = nowDate;

			// 最終作成月（定期）※定期：画面.作成年月(YYYYMM), 企画：String.Empty
			with_1.LastRendoMonthTeiki.Value = lastMonth;

			// 登録ID(画面ID) ※造成は PC名が編集されている。画面IDをセットする
			with_1.EntryClient.Value = this.Name; // UserInfoManagement.client
												  // 登録ユーザーID(ユーザーID)
			with_1.EntryUserId.Value = UserInfoManagement.userId;
			// 更新ID(画面ID) ※造成は PC名が編集されている。画面IDをセットする
			with_1.UpdateClient.Value = this.Name; // UserInfoManagement.client
												   // 更新ユーザーID(ユーザーID)
			with_1.UpdateUserId.Value = UserInfoManagement.userId;


			// カウントアップ
			setRow++;

		}

		return tCourseMstEntity;
	}
	#endregion

	#region 台帳作成後処理
	/// <summary>
	/// 台帳作成後処理
	/// </summary>
	/// <param name="tCourseMstEntity"></param>
	/// <returns></returns>
	private int setResultToLeadger(EntityOperation[] tCourseMstEntity)
	{
		int returnValue = 0;

		string resultDisp = string.Empty;

		//台帳作成用エンティティ
		foreach (TCourseMstEntity courseMst in tCourseMstEntity.EntityData)
		{

			resultDisp = string.Empty;

			// エンティティと一致するデータテーブル行
			int dtIdx = this.getMatchCrsDataRow(courseMst, SelectDataTable);
			// 表示する一覧の行 (取得後、固定行分加算)
			int grdRow = dtIdx + this.grdCrsList.Rows.Fixed;

			// 表示行が取得できなかった場合はスキップ
			if (grdRow < this.grdCrsList.Rows.Fixed)
			{
				continue;
			}

			// 処理結果の表示
			if (ReferenceEquals(courseMst.YoyakuSystemRendoDate.Value, null))
			{
				resultDisp = ProcessResult_Error;
			}
			else
			{
				resultDisp = ProcessResult_Normal;
				if (courseMst.LastRendoMonthTeiki.Value.Equals(CrsMstToLedger_DA.noUpdateValue))
				{
					resultDisp = ProcessResult_NoData;
				}
			}
			this.grdCrsList.SetData(grdRow, CrsGridCols.result, resultDisp);

			// 正常終了時
			if (resultDisp.Equals(ProcessResult_Normal))
			{

				// 台帳作成日
				this.grdCrsList.SetData(grdRow, CrsGridCols.ledger_Create_Date, courseMst.YoyakuSystemRendoDate.Value.ToString().Substring(0, 10));

				// 最終作成月(定期)
				if ((ReferenceEquals(courseMst.LastRendoMonthTeiki.Value, null)) == false &&)
				{
					string.IsNullOrWhiteSpace(System.Convert.ToString(courseMst.LastRendoMonthTeiki.Value)) = System.Convert.ToBoolean(false);
					string lastDay = System.Convert.ToString(courseMst.LastRendoMonthTeiki.Value.ToString().PadLeft(6, " "));
					string dispLastDay = string.Format("{0}/{1}", lastDay.Substring(0, 4), lastDay.Substring(4, 2));
					this.grdCrsList.SetData(grdRow, CrsGridCols.last_Month_Teiki, dispLastDay);
				}
				else
				{
					this.grdCrsList.SetData(grdRow, CrsGridCols.last_Month_Teiki, string.Empty);
				}


				// 正常終了した件数
				returnValue++;
			}

		}

		return returnValue;
	}
	#endregion

	#region エンティティと一致するDataTable行を取得
	/// <summary>
	/// エンティティと一致するDataTable行を取得
	/// </summary>
	/// <param name="courseMst"></param>
	/// <param name="grdDataTable"></param>
	/// <returns></returns>
	private int getMatchCrsDataRow(TCourseMstEntity courseMst, DataTable grdDataTable)
	{
		int returnValue = -1;

		// 選択されたコース情報
		for (int row = 0; row <= grdDataTable.Rows.Count - 1; row++)
		{

			// 同一キーか
			if (isMatchCrsKey(courseMst, grdDataTable[row]) == false)
			{
				continue;
			}

			// 同一キーが見つかった場合、データテーブル行を返す
			returnValue = row;
			break;

		}

		return returnValue;
	}
	#endregion

	#region エンティティと同一キーか
	/// <summary>
	/// エンティティと同一キーか
	/// </summary>
	/// <param name="courseMst"></param>
	/// <param name="grdDataRow"></param>
	/// <returns></returns>
	private bool isMatchCrsKey(TCourseMstEntity courseMst, DataRow grdDataRow)
	{

		// 定期企画区分
		if (courseMst.TeikiKikakuKbn.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.teikiKikakuKbn - 1).ToString()) == false)
		{
			return false;
		}

		// コースコード
		if (courseMst.CrsCd.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.crsCd - 1).ToString()) == false)
		{
			return false;
		}

		// 改定日
		if (courseMst.KaiteiDate.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.kaiteiDay - 1).ToString().Replace("/", "")) == false)
		{
			return false;
		}

		// 年
		if (courseMst.CrsYear.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.year - 1).ToString()) == false)
		{
			return false;
		}

		// 季
		if (courseMst.Season.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.season - 1).ToString()) == false)
		{
			return false;
		}

		// 無効フラグ
		if (courseMst.InvalidFlg.Value.ToString().Equals(grdDataRow.Item(S01_0301.CrsList_Koumoku.invalidFlg - 1).ToString()) == false)
		{
			return false;
		}

		return true;
	}

	#endregion

	#region コースマスタ検索用キー設定
	/// <summary>
	/// コースマスタ検索用キー設定
	/// </summary>
	/// <param name="dr"></param>
	/// <returns></returns>
	private CrsMasterKeyKoumoku getCrsMasterKey(DataRow dr)
	{
		CrsMasterKeyKoumoku crsKeyInfo = new CrsMasterKeyKoumoku();

		crsKeyInfo.Teiki_KikakuKbn = dr.Item(S01_0301.CrsList_Koumoku.teikiKikakuKbn - 1).ToString();
		crsKeyInfo.CrsCd = dr.Item(S01_0301.CrsList_Koumoku.crsCd - 1).ToString();
		crsKeyInfo.KaiteiDay = dr.Item(S01_0301.CrsList_Koumoku.kaiteiDay - 1).ToString().Replace("/", "");
		crsKeyInfo.Year = dr.Item(S01_0301.CrsList_Koumoku.year - 1).ToString();
		crsKeyInfo.Season = dr.Item(S01_0301.CrsList_Koumoku.season - 1).ToString();
		crsKeyInfo.InvalidFlg = dr.Item(S01_0301.CrsList_Koumoku.invalidFlg - 1).ToString();

		return crsKeyInfo;
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
		F10Key_Visible = true; // F10:実行
		F11Key_Visible = false; // F11:未使用
		F12Key_Visible = false; // F12:未使用
		F10Key_Text = "F10:実行";
	}
	#endregion

}