//using System.Enum;


/// <summary>
/// ピックアップ台帳作成指示
/// </summary>
public class S01_0503
{

	#region  定数/変数

	//処理結果
	private const string ProcessResult_Normal = "正常終了";
	private const string ProcessResult_NoData = "台帳展開済";
	private const string ProcessResult_Error = "エラー";
	private const string ProcessResult_DeleteAlready = "削除済";
	private const string ProcessResult_NotCovered = "作成対象外";

	// 台帳作成実行フラグ (True:実行済 / False:未実行)
	private bool _executeLeadger = false;

	// From年月日 (初期値)
	private const string FromInitDate = "20000101";
	// From年月日 (初期値)
	private const string ToInitDate = "20991231";

	#endregion

	#region  列挙

	/// <summary>
	/// ピックアップ一覧のカラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum PickupGridCols : int
	{
		[Value("No")] no = 0,
		[Value("PUﾙｰﾄｺｰﾄﾞ")] pickup_Route_Cd,
		[Value("開始日")] start_Day_Disp,
		[Value("開始日(非表示)")] start_Day,
		[Value("終了日")] end_Day_Disp,
		[Value("コース乗車地コード（非表示）")] crs_Jyosya_ti,
		[Value("コース乗車地")] crs_Jyosya_ti_Name,
		[Value("到着時間")] ttyak_Time_Disp,
		[Value("車種")] car_Cd,
		[Value("削除日(非表示)")] delete_date,
		[Value("台帳作成日")] ledger_Create_Day,
		[Value("最終作成月")] last_Create_Mon,
		[Value("処理結果")] result
	}

	/// <summary>
	/// SelectDataTableのカラム定義
	/// </summary>
	/// <remarks>参照に必要な項目のみ定義(必要に応じて追加する)</remarks>
	private enum PickupGridDataCols : int
	{
		CHECK = 0,
		PICKUP_ROUTE_CD,
		START_DAY,
		END_DAY
	}
	#endregion

	#region  プロパティ

	// 選択されたデータ (ピックアップルート一覧と同じ列)
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

			//コントロールの初期化
			this.setControlInitialize();

			//グリッド初期化
			this.initializeGrid();

			//ピックアップルートにて選択された情報を一覧に表示
			this.setPickupList(SelectDataTable);

			this.grdPickupList.Focus();

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
							//Dim lockInfo() As CrsMasterKeyKoumoku = Nothing  'ロック情報()

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
			string fromDate = FromInitDate;
			string toDate = ToInitDate;
			getDateBetween(nowDate, ref lastMonth, ref fromDate, ref toDate);

			// データチェック
			// ※処理結果に表示 (処理は続行する)
			checkExecData(SelectDataTable, toDate);

			//実行確認メッセージ
			if (dispExecMessage() == false)
			{
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

		object with_1 = this.grdPickupList;
		//№
		with_1.Cols(PickupGridCols.no).Name = PickupGridCols.no.ToString().ToUpper();
		with_1.Cols(PickupGridCols.no).Caption = getEnumAttrValue(PickupGridCols.no);
		with_1.Cols(PickupGridCols.no).Width = 40;
		//PUﾙｰﾄｺｰﾄﾞ
		with_1.Cols(PickupGridCols.pickup_Route_Cd).Name = PickupGridCols.pickup_Route_Cd.ToString().ToUpper();
		with_1.Cols(PickupGridCols.pickup_Route_Cd).Caption = getEnumAttrValue(PickupGridCols.pickup_Route_Cd);
		with_1.Cols(PickupGridCols.pickup_Route_Cd).Width = 90;
		//開始日
		with_1.Cols(PickupGridCols.start_Day_Disp).Name = PickupGridCols.start_Day_Disp.ToString().ToUpper();
		with_1.Cols(PickupGridCols.start_Day_Disp).Caption = getEnumAttrValue(PickupGridCols.start_Day_Disp);
		with_1.Cols(PickupGridCols.start_Day_Disp).Width = 90;
		//開始日(非表示)
		with_1.Cols(PickupGridCols.start_Day).Name = PickupGridCols.start_Day.ToString().ToUpper();
		with_1.Cols(PickupGridCols.start_Day).Caption = getEnumAttrValue(PickupGridCols.start_Day);
		with_1.Cols(PickupGridCols.start_Day).Visible = false;
		//終了日
		with_1.Cols(PickupGridCols.end_Day_Disp).Name = PickupGridCols.end_Day_Disp.ToString().ToUpper();
		with_1.Cols(PickupGridCols.end_Day_Disp).Caption = getEnumAttrValue(PickupGridCols.end_Day_Disp);
		with_1.Cols(PickupGridCols.end_Day_Disp).Width = 90;
		//コース乗車地コード（非表示）
		with_1.Cols(PickupGridCols.crs_Jyosya_ti).Name = PickupGridCols.crs_Jyosya_ti.ToString().ToUpper();
		with_1.Cols(PickupGridCols.crs_Jyosya_ti).Caption = getEnumAttrValue(PickupGridCols.crs_Jyosya_ti);
		with_1.Cols(PickupGridCols.crs_Jyosya_ti).Visible = false;
		//コース乗車地
		with_1.Cols(PickupGridCols.crs_Jyosya_ti_Name).Name = PickupGridCols.crs_Jyosya_ti_Name.ToString().ToUpper();
		with_1.Cols(PickupGridCols.crs_Jyosya_ti_Name).Caption = getEnumAttrValue(PickupGridCols.crs_Jyosya_ti_Name);
		with_1.Cols(PickupGridCols.crs_Jyosya_ti_Name).Width = 189;
		//到着時間
		with_1.Cols(PickupGridCols.ttyak_Time_Disp).Name = PickupGridCols.ttyak_Time_Disp.ToString().ToUpper();
		with_1.Cols(PickupGridCols.ttyak_Time_Disp).Caption = getEnumAttrValue(PickupGridCols.ttyak_Time_Disp);
		with_1.Cols(PickupGridCols.ttyak_Time_Disp).Width = 71;
		//車種
		with_1.Cols(PickupGridCols.car_Cd).Name = PickupGridCols.car_Cd.ToString().ToUpper();
		with_1.Cols(PickupGridCols.car_Cd).Caption = getEnumAttrValue(PickupGridCols.car_Cd);
		with_1.Cols(PickupGridCols.car_Cd).Width = 39;
		//削除日 (非表示)
		with_1.Cols(PickupGridCols.delete_date).Name = PickupGridCols.delete_date.ToString().ToUpper();
		with_1.Cols(PickupGridCols.delete_date).Caption = getEnumAttrValue(PickupGridCols.delete_date);
		with_1.Cols(PickupGridCols.delete_date).Visible = false;
		//台帳作成日
		with_1.Cols(PickupGridCols.ledger_Create_Day).Name = PickupGridCols.ledger_Create_Day.ToString().ToUpper();
		with_1.Cols(PickupGridCols.ledger_Create_Day).Caption = getEnumAttrValue(PickupGridCols.ledger_Create_Day);
		with_1.Cols(PickupGridCols.ledger_Create_Day).Width = 90;
		//最終作成月
		with_1.Cols(PickupGridCols.last_Create_Mon).Name = PickupGridCols.last_Create_Mon.ToString().ToUpper();
		with_1.Cols(PickupGridCols.last_Create_Mon).Caption = getEnumAttrValue(PickupGridCols.last_Create_Mon);
		with_1.Cols(PickupGridCols.last_Create_Mon).Width = 90;
		//結果
		with_1.Cols(PickupGridCols.result).Name = PickupGridCols.result.ToString().ToUpper();
		with_1.Cols(PickupGridCols.result).Caption = getEnumAttrValue(PickupGridCols.result);
		with_1.Cols(PickupGridCols.result).Width = 200;

	}

	/// <summary>
	/// 一覧表示
	/// </summary>
	/// <remarks></remarks>
	private void setPickupList(DataTable selectData)
	{

		// データコピー
		DataTable grdData = new DataTable();
		grdData = selectData.Copy;

		// カラム追加
		grdData.Columns.Add(PickupGridCols.no.ToString().ToUpper());
		grdData.Columns.Add(PickupGridCols.result.ToString().ToUpper());
		//grdData.Columns.Add(PickupGridCols.ledger_Create_Day.ToString.ToUpper)
		//grdData.Columns.Add(PickupGridCols.last_Create_Mon.ToString.ToUpper)

		// 追加した情報の初期設定
		for (int row = 0; row <= grdData.Rows.Count - 1; row++)
		{
			// NO
			grdData.Rows(row).Item[PickupGridCols.no.ToString().ToUpper()] = (row + 1).ToString();
			// 処理結果
			grdData.Rows(row).Item[PickupGridCols.result.ToString().ToUpper()] = string.Empty;

			//Dim pickupRouteCol As New MPickupRouteEntity
			//Dim pickupRoute As DataTable = Nothing
			//' 台帳作成日
			//grdData.Rows(row).Item(PickupGridCols.ledger_Create_Day.ToString.ToUpper) = String.Empty
			//' 最終作成月
			//grdData.Rows(row).Item(PickupGridCols.last_Create_Mon.ToString.ToUpper) = String.Empty
		}

		// データを一覧へ反映
		this.grdPickupList.DataSource = grdData;

	}

	/// <summary>
	/// コントロールの初期化
	/// </summary>
	/// <remarks></remarks>
	private void setControlInitialize()
	{

		// システム処理月を初期表示
		CommonDaProcess commonDaProcess = new CommonDaProcess();
		this.datLastCreateMonth.Value = commonDaProcess.getServerSysDate();
		datLastCreateMonth.Enabled = true;

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


		// [画面.作成月]から[最終作成月][開始日、終了日]を求める

		//作成年月 (YYYMM)
		DateTime contorlVallue = System.Convert.ToDateTime(this.datLastCreateMonth.Value);
		lastMonth = string.Concat(System.Convert.ToString(contorlVallue.Year), contorlVallue.Month.ToString().PadLeft(2, '0'));

		// from年月日 (From年月日 (初期値))
		fromDate = FromInitDate;

		// to年月日 (YYYYMM & 月末)
		toDate = lastMonth + DateTime.DaysInMonth(contorlVallue.Year, contorlVallue.Month).ToString("00");

	}
	#endregion

	#region エラークリア
	/// <summary>
	/// エラークリア
	/// </summary>
	private void clearErrItem()
	{

		// 作成月
		this.datLastCreateMonth.ExistError = false;

		// 一覧.処理結果をクリアする
		for (int row = this.grdPickupList.Rows.Fixed; row <= this.grdPickupList.Rows.Count - 1; row++)
		{

			// "正常終了" 以外をクリアする。正常終了はクリアしない (再実行不可とするため)
			if (this.grdPickupList.GetData(row, PickupGridCols.result).ToString().Equals(ProcessResult_Normal) == false)
			{
				this.grdPickupList.SetData(row, PickupGridCols.result, string.Empty);
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

		//必須入力チェック
		if (ReferenceEquals(this.datLastCreateMonth.Value, null))
		{
			this.datLastCreateMonth.ExistError = true;
			this.datLastCreateMonth.Focus();
			createFactoryMsg.messageDisp("0014", "作成月");
			return false;
		}

		// 作成月チェック
		// 画面.作成月 < 処理月(システム日付) はエラー
		DateTime contorlVallue = System.Convert.ToDateTime(this.datLastCreateMonth.Value);

		if (string.Compare(contorlVallue.ToString("yyyyMM"), nowDate.ToString("yyyyMM")) < 0)
		{
			this.datLastCreateMonth.ExistError = true;
			this.datLastCreateMonth.Focus();
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
	/// <param name="selectData"></param>
	/// <param name="toDate"></param>
	/// <remarks></remarks>
	private void checkExecData(DataTable selectData, string toDate)
	{

		string resultDisp = string.Empty;

		// 削除データチェック
		this.checkDeleteData();

		// データチェック（各種チェックを行う）
		for (int row = this.grdPickupList.Rows.Fixed; row <= this.grdPickupList.Rows.Count - 1; row++)
		{

			// 処理結果が 空白でない場合はスキップ
			if (string.IsNullOrWhiteSpace(this.grdPickupList.GetData(row, PickupGridCols.result).ToString()) == false)
			{
				continue;
			}

			// データチェック（各種チェックを行う）
			resultDisp = checkExecDataItem(selectData.Rows(row - 1), toDate);

			// 処理結果
			this.grdPickupList.SetData(row, PickupGridCols.result, resultDisp);

		}

	}

	#endregion

	#region データチェック（各種チェックを行う）
	/// <summary>
	/// データチェック（各種チェックを行う）
	/// </summary>
	/// <param name="dr"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private string checkExecDataItem(DataRow dr, string toDate)
	{

		// 開始日チェック
		if (checkStartDate(dr, toDate) == false)
		{
			return ProcessResult_NotCovered;
		}

		return string.Empty;
	}
	#endregion

	#region 開始日チェック
	/// <summary>
	/// 開始日チェック
	/// </summary>
	/// <param name="dr"></param>
	/// <param name="toDate"></param>
	/// <returns></returns>
	private bool checkStartDate(DataRow dr, string toDate)
	{

		// 開始日を取得
		string startDate = System.Convert.ToString(System.Convert.ToDateTime(dr.Item(PickupGridCols.start_Day)).ToString("yyyyMMdd"));

		// 作成月(月末)が  開始日より過去の場合、作成対象外 (False)
		if (string.Compare(toDate, startDate) <= 0)
		{
			return false;
		}

		return true;
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
		for (int row = this.grdPickupList.Rows.Fixed; row <= this.grdPickupList.Rows.Count - 1; row++)
		{

			// 処理結果が 空白でない場合はスキップ
			if (string.IsNullOrWhiteSpace(this.grdPickupList.GetData(row, PickupGridCols.result).ToString()) == false)
			{
				continue;
			}

			resultDisp = string.Empty;

			// 削除データ
			if (string.IsNullOrWhiteSpace(this.grdPickupList.GetData(row, PickupGridCols.delete_date).ToString()) == false)
			{
				// 削除済み

				CommonDateUtil commonDateUtil = new CommonDateUtil();
				string deleteYmd = System.Convert.ToString(commonDateUtil.convertDateFormat(this.grdPickupList.GetData(row, PickupGridCols.delete_date).ToString(), commonDateUtil.NormalTime_JP));
				resultDisp = string.Format("{0} ({1})", ProcessResult_DeleteAlready, deleteYmd);
			}

			// 処理結果
			this.grdPickupList.SetData(row, PickupGridCols.result, resultDisp);

		}

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

		for (int row = this.grdPickupList.Rows.Fixed; row <= this.grdPickupList.Rows.Count - 1; row++)
		{

			if ((string)(getEmptyValue(this.grdPickupList.GetData(row, PickupGridCols.result), string.Empty)) == "")
			{
				// 未処理
				execKanouLineNum++;
			}
			else if (getEmptyValue(this.grdPickupList.GetData(row, PickupGridCols.result), string.Empty) == ProcessResult_Normal)
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
			// 処理対象データがありません。
			createFactoryMsg.messageDisp("0036");
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

			// 台帳作成するピックアップ情報を ピックアップルートマスタエンティティにセットする
			EntityOperation[] mPickupRouteEntity = this.setSelectDataToMPickupRouteEntity(nowDate, lastMonth);

			//----------------------------------------------
			// 台帳作成
			//----------------------------------------------
			PickupToLedger_DA pickupToLedger_DA = new PickupToLedger_DA();
			int ret = System.Convert.ToInt32(pickupToLedger_DA.executePickupHotelMaster(this.Name, mPickupRouteEntity, fromDate, toDate));

			//----------------------------------------------
			// 台帳作成後処理
			//----------------------------------------------
			int registCount = setResultToLeadger(mPickupRouteEntity);


			// {1}が正常終了しました。
			int ledgerCount = System.Convert.ToInt32(mPickupRouteEntity.EntityData.Length);
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

	#region 台帳作成するピックアップ情報 を ックアップルートマスタエンティティにセットする
	/// <summary>
	/// 台帳作成するピックアップ情報 を ピックアップルートマスタエンティティにセットする
	/// </summary>
	/// <param name="nowDate"></param>
	/// <param name="lastMonth"></param>
	/// <returns></returns>
	private EntityOperation[] setSelectDataToMPickupRouteEntity(DateTime nowDate, string lastMonth)
	{
		EntityOperation mPickupRouteEntity = new EntityOperation(Of MPickupRouteEntity);
		int setRow = 0;

		PickupRoute_DA pickupRoute_DA = new PickupRoute_DA();
		MPickupRouteEntity routeCol = new MPickupRouteEntity(); // ピックアップルートマスタ カラム使用
		DataTable routeDt = null;

		mPickupRouteEntity.clear();

		for (int row = this.grdPickupList.Rows.Fixed; row <= this.grdPickupList.Rows.Count - 1; row++)
		{

			// 結果が空白でない場合はスキップ (何かしらエラーあり)
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.grdPickupList.GetData(row, PickupGridCols.result))) == false)
			{
				continue;
			}

			// ピックアップルートマスタ取得
			string pickupHotelCd = this.grdPickupList.GetData(row, PickupGridCols.pickup_Route_Cd).ToString();
			Date? startDay = (Date?)(this.grdPickupList.GetData(row, PickupGridCols.start_Day).ToString());

			Hashtable paramInfoList = new Hashtable();
			paramInfoList.Add(routeCol.PickupRouteCd.PhysicsName, pickupHotelCd);
			paramInfoList.Add(routeCol.StartDay.PhysicsName, startDay);
			routeDt = pickupRoute_DA.accessPickupRouteMaster(pickupRoute_DA.accessType.getPickupRouteMaster, paramInfoList);

			// ピックアップルートマスタが存在しない場合はスキップ (台帳展開不可)
			if (ReferenceEquals(routeDt, null) ||)
			{
				routeDt.Rows.Count(<= 0);
				this.grdPickupList.SetData(row, PickupGridCols.result, ProcessResult_Error + " (データ不明)");
				continue;
			}

			// エンティティ行追加
			if (mPickupRouteEntity.EntityData.Length - 1 < setRow)
			{
				MPickupRouteEntity entity = new MPickupRouteEntity();
				mPickupRouteEntity.add(entity);
			}

			// 受け渡しに必要な値のみ編集している (必要に応じて項目追加可)
			object with_1 = mPickupRouteEntity.EntityData(setRow);

			// キー項目
			with_1.PickupRouteCd.Value = routeDt.Rows(0).Item(routeCol.PickupRouteCd.PhysicsName).ToString();
			with_1.StartDay.Value = (Date?)(routeDt.Rows(0).Item(routeCol.StartDay.PhysicsName));

			// 台帳作成日
			with_1.LedgerCreateDay.Value = nowDate;

			// 最終作成月
			with_1.LastCreateMon.Value = lastMonth;

			// システム登録ＰＧＭＩＤ
			with_1.SystemEntryPgmid.Value = this.Name;
			// システム登録者コード
			with_1.SystemEntryPersonCd.Value = UserInfoManagement.userId;

			// システム更新ＰＧＭＩＤ
			with_1.SystemUpdatePgmid.Value = this.Name;
			// システム更新者コード
			with_1.SystemUpdatePersonCd.Value = UserInfoManagement.userId;


			// カウントアップ
			setRow++;

		}

		return mPickupRouteEntity;
	}
	#endregion

	#region 台帳作成後処理
	/// <summary>
	/// 台帳作成後処理
	/// </summary>
	/// <param name="mPickupRouteEntity"></param>
	/// <returns></returns>
	private int setResultToLeadger(EntityOperation[] mPickupRouteEntity)
	{
		int returnValue = 0;

		string resultDisp = string.Empty;

		//台帳作成用エンティティ
		foreach (MPickupRouteEntity pickupMst in mPickupRouteEntity.EntityData)
		{

			resultDisp = string.Empty;

			// エンティティと一致するデータテーブル行
			int dtIdx = this.getMatchPickupDataRow(pickupMst, SelectDataTable);
			// 表示する一覧の行 (取得後、固定行分加算)
			int grdRow = dtIdx + this.grdPickupList.Rows.Fixed;

			// 表示行が取得できなかった場合はスキップ
			if (grdRow < this.grdPickupList.Rows.Fixed)
			{
				continue;
			}

			// 処理結果の表示
			if (ReferenceEquals(pickupMst.ledgerCreateDay.Value, null))
			{
				resultDisp = ProcessResult_Error;
			}
			else
			{
				resultDisp = ProcessResult_Normal;
				if (pickupMst.LastCreateMon.Value.Equals(CrsMstToLedger_DA.noUpdateValue))
				{
					resultDisp = ProcessResult_NoData;
				}
			}
			this.grdPickupList.SetData(grdRow, PickupGridCols.result, resultDisp);

			// 正常終了時
			if (resultDisp.Equals(ProcessResult_Normal))
			{

				// 台帳作成日
				this.grdPickupList.SetData(grdRow, PickupGridCols.ledger_Create_Day, pickupMst.LedgerCreateDay.Value.ToString().Substring(0, 10));

				// 最終作成月
				if (string.IsNullOrWhiteSpace(System.Convert.ToString(pickupMst.lastCreateMon.Value)) == false)
				{
					string lastDay = System.Convert.ToString(pickupMst.lastCreateMon.Value.ToString().PadLeft(6, " "));
					string dispLastDay = string.Format("{0}/{1}", lastDay.Substring(0, 4), lastDay.Substring(4, 2));
					this.grdPickupList.SetData(grdRow, PickupGridCols.last_Create_Mon, dispLastDay);
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
	/// <param name="pickupMst"></param>
	/// <param name="grdDataTable"></param>
	/// <returns></returns>
	private int getMatchPickupDataRow(MPickupRouteEntity pickupMst, DataTable grdDataTable)
	{
		int returnValue = -1;

		// 選択されたコース情報
		for (int row = 0; row <= grdDataTable.Rows.Count - 1; row++)
		{

			// 同一キーか
			if (isMatchCrsKey(pickupMst, grdDataTable[row]) == false)
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
	/// <param name="pickupMst"></param>
	/// <param name="grdDataRow"></param>
	/// <returns></returns>
	private bool isMatchCrsKey(MPickupRouteEntity pickupMst, DataRow grdDataRow)
	{

		// ピックアップルートコード
		if (pickupMst.PickupRouteCd.Value.ToString().Equals(grdDataRow.Item(PickupGridDataCols.PICKUP_ROUTE_CD).ToString()) == false)
		{
			return false;
		}

		// 開始日
		if (pickupMst.StartDay.Value.ToString().Equals(grdDataRow.Item(PickupGridDataCols.START_DAY).ToString()) == false)
		{
			return false;
		}


		return true;
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