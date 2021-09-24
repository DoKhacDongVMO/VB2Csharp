using System.ComponentModel;
/// <summary>

/// 売上確定照会

/// ダイレクトチェックインを行った予約について、売上の確定を行う。（システム内のデータを発券済み状態にする）

/// ''' </summary>

/// ''' <remarks>

/// '''    Author：2019/02/04//DTS佐藤

/// ''' </remarks>
public class S04_0104 : PT21, iPT21
{


    /// <summary>
    /// 画面ID
    /// </summary>
    private const string ScreenId = "S04_0104";
    /// <summary>
    /// 画面名
    /// </summary>
    private const string ScreenName = "売上確定照会";
    /// <summary>
    /// 乗車地コード (初期設定値)    浜松町バスターミナル(コード値:15))
    /// </summary>
    private const string _initPlaceCd = "15";
    // グリッド表示用
    /// <summary>
    /// 精算方法_振込
    /// </summary>
    private const strSeisanHohoHurikomi = "振込";
    /// <summary>
    /// 精算方法_営業所
    /// </summary>
    private const strSeisanHohoEigyosyo = "営業所";
    /// <summary>
    /// 精算方法_当日払い
    /// </summary>
    private const strSeisanHohoTojituPayment = "当日払い";
    /// <summary>
    /// 精算方法_ＡＧＴ
    /// </summary>
    private const strSeisanHohoAgt = "ＡＧＴ";
    /// <summary>
    /// 精算方法_カード
    /// </summary>
    private const strSeisanHohoCard = "カード";

    // 画面遷移用
    /// <summary>
    /// 列番号(グリッド) 予約区分
    /// </summary>
    private const NoColYoyakuKbn = 6;
    /// <summary>
    /// 列番号(グリッド) 予約NO
    /// </summary>
    private const NoColYoyakuNo = 7;

    /// <summary>
    /// 条件GroupBoxのTop座標
    /// </summary>
    public const TopGbxCondition = 41;

    /// <summary>
    /// 条件GroupBoxのマージン
    /// </summary>
    public const MarginGbxCondition = 6;

    /// <summary>
    /// 条件GroupBox非表示時のGroupBoxArea1の高さ
    /// </summary>
    public const HeightGbxAreas1OnNotVisibleCondition = 272;

    /// <summary>
    /// 条件GroupBox非表示時のGroupBoxArea2の高さ
    /// </summary>
    public const HeightGbxAreas2OnNotVisibleCondition = 429;

    /// <summary>
    /// 条件GroupBoxの高さ
    /// </summary>
    private int _heightGbxCondition;

    /// <summary>
    /// GroupBoxArea1の高さ
    /// </summary>
    private int _heightGbxArea1;

    /// <summary>
    /// GroupBoxArea2の高さ
    /// </summary>
    private int _heightGbxArea2;

    /// <summary>
    /// GroupBoxArea1のTop座標
    /// </summary>
    private int _topGbxArea1;

    /// <summary>
    /// GroupBoxArea2のTop座標
    /// </summary>
    private int _topGbxArea2;



    /// <summary>
    /// 条件GroupBoxの高さ
    /// </summary>
    /// <returns></returns>
    public int HeightGbxCondition
    {
        get
        {
            return this.gbxCondition.Height;
        }
        set
        {
            this.gbxCondition.Height = value;
        }
    }

    /// <summary>
    /// 条件GroupBoxの表示/非表示
    /// </summary>
    /// <returns></returns>
    public bool VisibleGbxCondition
    {
        get
        {
            return this.gbxCondition.Visible;
        }
        set
        {
            this.gbxCondition.Visible = value;
        }
    }



    /// <summary>
    /// 画面初期処理
    /// </summary>
    protected override void StartupOrgProc()
    {
        base.StartupOrgProc();

        // ベースフォームの設定
        this.setFormId = ScreenId;
        this.setTitle = ScreenName;

        // エラー有無のクリア
        clearError();
    }

    /// <summary>
    /// 条件GroupBox表示制御ボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnVisiblerCondition_Click(object sender, EventArgs e)
    {
        this.VisibleGbxCondition = !this.VisibleGbxCondition;

        int offSet = this.HeightGbxCondition + MarginGbxCondition;
        int heightCondition = System.Convert.ToInt32(this.HeightGbxCondition / (double)2);

        // GrpBoxArea1, 2の座標, サイズを表示/非表示に応じて変更
        if (this.VisibleGbxCondition)
        {
            // 表示状態
            this.btnVisiblerCondition.Text = "非表示 <<";

            this.setGrpLayout();
            this.gbxArea1.Height -= heightCondition;
            this.gbxArea2.Height -= heightCondition;
            this.grdCrs.Height -= heightCondition;
            this.grdYoyaku.Height -= heightCondition;
        }
        else
        {
            // 非表示状態
            this.btnVisiblerCondition.Text = "表示 >>";
            this.gbxArea1.Height = HeightGbxAreas1OnNotVisibleCondition;
            this.gbxArea2.Height = HeightGbxAreas2OnNotVisibleCondition;

            this.gbxArea1.Top = TopGbxCondition;
            this.gbxArea2.Top = TopGbxCondition + this.gbxArea1.Height + MarginGbxCondition;
            this.grdCrs.Height += heightCondition;
            this.grdYoyaku.Height += heightCondition;
        }
    }

    /// <summary>
    /// 条件クリアボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {

        // クリアボタン押下イベント実行
        base.btnCom_Click(this.btnClear, e);
    }

    /// <summary>
    /// 条件クリアボタン押下時
    /// </summary>
    protected override void btnCLEAR_ClickOrgProc()
    {

        // 初期処理と同じ処理を実行
        StartupOrgProc();
    }

    /// <summary>
    /// F8：検索ボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {

        // 検索ボタン押下イベント実行
        base.btnCom_Click(this.btnSearch, e);

        // log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");
    }

    /// <summary>
    /// F9：予約照会ボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnYoyakuInquiry_Click(object sender, EventArgs e)
    {

        // F9:予約照会ボタン押下イベント実行
        base.btnCom_Click(this.btnYoyakuInquiry, e);

        // log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");
    }

    /// <summary>
    /// キーダウンイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void S04_0104_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.F8)
        {
            e.Handled = true;
            this.btnSearch.Select();
            this.btnSearch_Click(sender, e);
        }
        else if (e.KeyData == Keys.F9)
        {
            e.Handled = true;
            this.btnYoyakuInquiry_Click(sender, e);
        }
    }

    /// <summary>
    /// F8(検索)ボタン押下イベント
    /// </summary>
    protected override void btnF8_ClickOrgProc()
    {

        // エラー有無のクリア
        clearError();

        // [詳細エリア]検索結果部の項目初期化
        initDetailAreaItems();

        // 検索条件項目入力チェック
        if (this.checkSearchItems() == true)
            // エラーがない場合、検索処理を実行

            // コース一覧の取得
            setDataCrsGrid();
    }

    /// <summary>
    /// F9(内訳)ボタン押下イベント
    /// </summary>
    protected override void btnF9_ClickOrgProc()
    {
        setDataYoyakuGrid();

        // 後処理
        base.comPostEvent();

        this.btnYoyakuInquiry.Focus();
    }


    /// <summary>
    /// グリッドのデータソース変更時イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
    {
        // データ件数を表示(ヘッダー行分マイナス1)
        string formatedCount = (this.grdCrs.Rows.Count - 1).ToString.PadLeft(6);
        this.lblLengthGrd.Text = formatedCount + "件";
    }

    /// <summary>
    /// 予約ボタン実行時の画面遷移イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void yoyakuUriageKakuteiBtnClick(object sender, C1.Win.C1FlexGrid.RowColEventArgs e
)
    {
        FlexGridEx grd;
        S04_0105ParamData prm = new S04_0105ParamData();
        string yoyakuKbn;             // 予約区分(画面遷移パラメータ)
        int yoyakuNo;             // 予約NO(画面遷移パラメータ)

        grd = sender as FlexGridEx;

        // 選択行が0以下の場合は処理をしない
        if (grd.Row <= 0)
            return;

        // 押下行の予約区分、予約NO
        yoyakuKbn = grd.GetData(e.Row, NoColYoyakuKbn) as string;

        if (grd.GetData(e.Row, NoColYoyakuNo) == null)
            return;

        yoyakuNo = int.Parse(grd.GetData(e.Row, NoColYoyakuNo).ToString());

        // 予約ボタン押下
        // 画面間パラメータを用意
        prm.YoyakuKbn = yoyakuKbn;                         // 予約区分
        prm.YoyakuNo = yoyakuNo;                           // 予約NO

        // 売上確定入力　画面展開
        using (S04_0105 form = new S04_0105())
        {
            form.ParamData = prm;
            form.ShowDialog();
        }
    }

    /// <summary>
    /// F2：戻るボタン押下イベント
    /// </summary>
    protected override void btnF2_ClickOrgProc()
    {

        // MyBase.btnF2_ClickOrgProc()

        closeCheckFlg = false;
        base.closeFormFlg = false;
        this.Close();
    }

    /// <summary>
    /// 画面終了時処理
    /// </summary>
    private void S04_0104_FormClosing()
    {
        this.Dispose();
    }


    /// <summary>
    /// コース一覧_全選択ボタン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAll_Click(object sender, EventArgs e)
    {
        for (int ii = 1; ii <= grdCrs.Rows.Count - 1; ii += 1)
            grdCrs.Rows(ii).Item("colSelection") = true;
    }

    /// <summary>
    /// コース一覧_全解除ボタン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear02_Click(object sender, EventArgs e)
    {
        for (int ii = 1; ii <= grdCrs.Rows.Count - 1; ii += 1)
            grdCrs.Rows(ii).Item("colSelection") = false;
    }



    /// <summary>
    /// [検索エリア]検索条件部の項目初期化
    /// </summary>
    protected override void initSearchAreaItems()
    {
        base.initSearchAreaItems();

        // 出発日
        dtmSyuptDay.Value = CommonDateUtil.getSystemTime();
        // 乗車地コード ←浜松町バスターミナル(コード値:15)を初期値でセット
        ucoNoribaCd.CodeText = _initPlaceCd;　　// UserInfoManagement.eigyosyoCd
        ucoNoribaCd.ValueText = null;
        // コード値のセットだけでは名称が表示されない (名称を取得)
        this.ucoNoribaCd.ValueText = getPlaceName(this.ucoNoribaCd.CodeText);

        // 上記以外の項目は全て空欄
        // コースコード
        ucoCrsCd.CodeText = null;
        ucoCrsCd.ValueText = null;
        // 出発時間
        dtmSyuptTime.Value = null;
        // 号車
        txtGousya.Text = null;

        // ロード時にフォーカスを設定する
        this.ActiveControl = this.dtmSyuptDay;
    }

    /// <summary>
    /// 場所マスタより名称取得
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private string getPlaceName(string code)
    {

        // 場所マスタ
        var clsPlace_DA = new Master.Place_DA();
        Hashtable paramInfoList = new Hashtable();
        paramInfoList.Add("placeCd", code);

        DataTable returnPalce = clsPlace_DA.getLocationCode(paramInfoList, string.Empty);
        if (returnPalce.Rows.Count == 1)
            return System.Convert.ToString(returnPalce.Rows(0)("PLACE_NAME_1"));

        return string.Empty;
    }

    /// <summary>
    /// エラー有無のクリア
    /// </summary>
    private void clearError()
    {

        // ExistErrorプロパティのクリア
        dtmSyuptDay.ExistError = false;
        ucoNoribaCd.ExistError = false;
        ucoCrsCd.ExistError = false;
        dtmSyuptTime.ExistError = false;
        txtGousya.ExistError = false;

        // Exceptionのクリア
        this.Exception = null;

        // エラーフラグの初期化
        this.ErrorFlg = false;
    }

    /// <summary>
    /// [詳細エリア]検索結果部の項目初期化
    /// </summary>
    protected override void initDetailAreaItems()
    {
        base.initDetailAreaItems();

        DataTable dt = new DataTable();
        setGridCrs();
        this.grdCrs.DataSource = dt;
        this.grdCrs.DataMember = "";
        this.grdCrs.Refresh();
        this.lblLengthGrd.Text = "     0件";

        setGridYoyaku();
        this.grdYoyaku.DataSource = dt;
        this.grdYoyaku.DataMember = "";
        this.grdYoyaku.Refresh();
        this.txtNinzuTotal.Text = "";

        // F9ボタンを非活性にする
        btnYoyakuInquiry.Enabled = false;

        // 検索条件を表示状態のGroupAreaのレイアウトを保存
        this.saveGrpLayout();
    }

    /// <summary>
    /// [詳細エリア]予約一覧の項目初期化
    /// </summary>
    protected void initDetailAreaItemsYoyaku()
    {
        DataTable dt = new DataTable();

        setGridYoyaku();
        this.grdYoyaku.DataSource = dt;
        this.grdYoyaku.DataMember = "";
        this.grdYoyaku.Refresh();

        this.txtNinzuTotal.Text = "";

        // 検索条件を表示状態のGroupAreaのレイアウトを保存
        this.saveGrpLayout();
    }

    /// <summary>
    /// フッタボタンの制御(表示\[活性]／非表示[非活性])
    /// </summary>
    protected override void initFooterButtonControl()
    {
        base.initFooterButtonControl();

        F8Key_Visible = false;
        F11Key_Visible = false;
    }

    /// <summary>
    /// GroupBoxのレイアウト保存
    /// </summary>
    private void saveGrpLayout()
    {
        this.gbxCondition.Height = this.gbxCondition.Height;
        this._heightGbxArea1 = this.gbxArea1.Height;
        this._heightGbxArea2 = this.gbxArea2.Height;
        this._topGbxArea1 = this.gbxArea1.Top;
        this._topGbxArea2 = this.gbxArea2.Top;
    }

    /// <summary>
    /// GroupBoxのレイアウト設定
    /// </summary>
    private void setGrpLayout()
    {
        this.gbxCondition.Height = this.gbxCondition.Height;
        this.gbxArea1.Height = this._heightGbxArea1;
        this.gbxArea2.Height = this._heightGbxArea2;
        this.gbxArea1.Top = this._topGbxArea1;
        this.gbxArea2.Top = this._topGbxArea2;
    }

    // コースグリッドの設定
    private void setGridCrs()
    {
        // グリッドの設定
        this.grdCrs.AllowDragging = (AllowDraggingEnum)false;
        this.grdCrs.AllowAddNew = false;
        this.grdCrs.AllowMerging = (AllowMergingEnum)8;
        this.grdCrs.AutoGenerateColumns = false;

        this.grdCrs.Cols(2).AllowEditing = false;
        this.grdCrs.Cols(3).AllowEditing = false;
        this.grdCrs.Cols(4).AllowEditing = false;
        this.grdCrs.Cols(5).AllowEditing = false;
        this.grdCrs.Cols(6).AllowEditing = false;
        this.grdCrs.Cols(7).AllowEditing = false;
    }

    // 予約グリッドの設定
    private void setGridYoyaku()
    {
        // グリッドの設定
        this.grdYoyaku.AllowDragging = (AllowDraggingEnum)false;
        this.grdYoyaku.AllowAddNew = false;
        this.grdYoyaku.AllowMerging = (AllowMergingEnum)8;
        this.grdYoyaku.AutoGenerateColumns = false;
        this.grdYoyaku.ShowButtons = (ShowButtonsEnum)2;

        this.grdYoyaku.Cols(2).AllowEditing = false;
        this.grdYoyaku.Cols(3).AllowEditing = false;
        this.grdYoyaku.Cols(4).AllowEditing = false;
        this.grdYoyaku.Cols(5).AllowEditing = false;
    }

    /// <summary>
    ///     '''検索条件項目入力チェック
    /// </summary>
    /// <returns>エラーがない場合：True、エラーの場合：False</returns>
    protected override bool checkSearchItems()
    {
        base.checkSearchItems();

        // 出発日入力チェック
        if (string.IsNullOrEmpty(dtmSyuptDay.Value.ToString) == true)
        {

            // 出発日が入力の場合、エラー
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "出発日");

            // 背景色を赤にする
            dtmSyuptDay.ExistError = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.dtmSyuptDay;

            return false;
        }

        // 乗車地コード入力チェック
        if (string.IsNullOrEmpty(ucoNoribaCd.CodeText) == true)
        {

            // 出発日が入力の場合、エラー
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地コード");

            // 背景色を赤にする
            ucoNoribaCd.ExistError = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.ucoNoribaCd;

            return false;
        }

        return true;
    }


    // コース一覧の取得
    private void setDataCrsGrid()
    {
        Hashtable paramInfoList = new Hashtable();
        var dtCrs = new DataTable();

        // データアクセス
        S04_0104_DA dataAccess = new S04_0104_DA();
        DataTable dataCrsInfo = new DataTable();

        // グリッド表示用dataRow
        DataRow drCrs = null/* TODO Change to default(_) if this is not a reference type */;
        // dt.NewRow
        DataRow drX = null/* TODO Change to default(_) if this is not a reference type */;

        TimeSpan SyuptTime = default(TimeSpan);

        // パラメータ設定
        // 出発日
        paramInfoList.Add("SyuptDay", Format(this.dtmSyuptDay.Value, "yyyyMMdd"));
        // 乗車地コード
        paramInfoList.Add("JyochachiCd", Trim(this.ucoNoribaCd.CodeText));

        // コースコード
        paramInfoList.Add("CrsCd", Trim(this.ucoCrsCd.CodeText));
        // 号車
        paramInfoList.Add("Gousya", Trim(this.txtGousya.Text));
        // 出発時間
        if (!string.IsNullOrEmpty(dtmSyuptTime.Value.ToString) == true)
        {
            SyuptTime = (TimeSpan)this.dtmSyuptTime.Value;
            paramInfoList.Add("SyuptTime", string.Concat(SyuptTime.Hours.ToString().PadLeft(2, '0'), SyuptTime.Minutes.ToString().PadLeft(2, '0')));
        }
        else
            paramInfoList.Add("SyuptTime", Trim(this.dtmSyuptTime.Value.ToString));

        // SQLでデータを取得
        dataCrsInfo = dataAccess.getCrsList(paramInfoList);

        // データ取得件数チェック
        if (dataCrsInfo.Rows.Count <= 0)
        {
            // 取得件数が0件の場合、エラー

            // [詳細エリア]検索結果部の項目初期化
            initDetailAreaItems();

            // 該当データが存在しません。
            CommonProcess.createFactoryMsg().messageDisp("E90_019");
        }
        else
        {
            // データが取得できた場合

            // 列作成
            dtCrs.Columns.Add("colSelection");          // 選択
            dtCrs.Columns.Add("colJyosyaTi");           // 乗車地
            dtCrs.Columns.Add("colCrsCd");              // コースコード
            dtCrs.Columns.Add("colCrsName");            // コース名
            dtCrs.Columns.Add("colSyuptTime");          // 出発時間
            dtCrs.Columns.Add("colGousya");             // 号車
            dtCrs.Columns.Add("colYoyakuNinzu");        // 予約人数
            dtCrs.Columns.Add("colSyuptDay");           // 出発日(非表示）

            // 取得した値を各列に設定
            foreach (var drCrs in dataCrsInfo.Rows)
            {
                drX = dtCrs.NewRow;

                drX("colSelection") = "";
                drX("colJyosyaTi") = drCrs("PLACE_NAME_SHORT");
                drX("colCrsCd") = drCrs("CRS_CD");
                drX("colCrsName") = drCrs("CRS_NAME");
                drX("colSyuptTime") = drCrs("SYUPT_TIME");
                drX("colGousya") = drCrs("GOUSYA");
                drX("colYoyakuNinzu") = drCrs("YOYAKU_NUM");
                drX("colSyuptDay") = drCrs("SYUPT_DAY");

                dtCrs.Rows.Add(drX);
            }

            // グリッドに取得したデータを表示する
            grdCrs.DataSource = dtCrs;

            // F9ボタンを活性化する
            btnYoyakuInquiry.Enabled = true;
        }
    }

    // 予約一覧の取得
    private void setDataYoyakuGrid()
    {
        // DBパラメータ
        List<int> paramSyuptDayList = new List<int>();       // 出発日
        List<string> paramCrsCdList = new List<string>();           // コースコード
        List<int> paramGousyaList = new List<int>();         // 号車

        var dtYoyaku = new DataTable();

        // データアクセス
        S04_0104_DA dataAccess = new S04_0104_DA();
        DataTable dataYoyakuInfo = new DataTable();

        // グリッド表示用dataRow
        DataRow drYoyaku = null/* TODO Change to default(_) if this is not a reference type */;
        // dt.NewRow
        DataRow drX = null/* TODO Change to default(_) if this is not a reference type */;

        // 精算方法表示用
        string strSeisanHoho = null;
        // 予約人数算出用
        int sumYoyakuNinzu = default(Integer);
        // 発券状態取得用
        string[] hakkenStateArry = null;
        for (int ii = 1; ii <= grdCrs.Rows.Count - 1; ii += 1)
        {

            // パラメータ設定
            if (grdCrs.Rows(ii).Item("colSelection").ToString == bool.TrueString)
            {

                // 出発日
                paramSyuptDayList.Add(System.Convert.ToInt32(grdCrs.Rows(ii).Item("colSyuptDay")));
                // コースコード
                paramCrsCdList.Add(grdCrs.Rows(ii).Item("colCrsCd").ToString);
                // 号車
                paramGousyaList.Add(System.Convert.ToInt32(grdCrs.Rows(ii).Item("colGousya")));
            }
        }

        // パラメータチェック
        if (paramSyuptDayList.Count == 0 && paramCrsCdList.Count == 0 && paramGousyaList.Count == 0)
        {

            // コースが選択されていません。
            CommonProcess.createFactoryMsg().messageDisp("E90_024", "コース");

            // 予約一覧の項目初期化
            initDetailAreaItemsYoyaku();

            return;
        }


        // SQLでデータを取得
        dataYoyakuInfo = dataAccess.getYoyakuList(paramSyuptDayList, paramCrsCdList, paramGousyaList);

        // データ取得件数チェック
        if (dataYoyakuInfo.Rows.Count <= 0)
        {
            // 取得件数が0件の場合、エラー

            // 予約一覧の項目初期化
            initDetailAreaItemsYoyaku();

            // 該当データが存在しません。
            CommonProcess.createFactoryMsg().messageDisp("E90_019");
        }
        else
        {
            // データが取得できた場合

            // 列作成
            dtYoyaku.Columns.Add("colUriageKakutei");          // 売上確定ボタン
            dtYoyaku.Columns.Add("colYoyakuNo");               // 予約番号(表示用)
            dtYoyaku.Columns.Add("colSurnameName");            // 姓名
            dtYoyaku.Columns.Add("colNinzu");                  // 人数
            dtYoyaku.Columns.Add("colSiharaiHoho");            // 支払方法
            dtYoyaku.Columns.Add("colYoyakuKbn");              // 予約区分
            dtYoyaku.Columns.Add("colYoyakuNo02");             // 予約番号
            dtYoyaku.Columns.Add("colCrsCd");                  // コースコード
            dtYoyaku.Columns.Add("colGousya");                 // 号車
            dtYoyaku.Columns.Add("colJotai");                  // 発券状態

            // 取得した値を各列に設定
            foreach (var drYoyaku in dataYoyakuInfo.Rows)
            {
                drX = dtYoyaku.NewRow;

                drX("colUriageKakutei") = "";
                drX("colYoyakuNo") = drYoyaku("YOYAKU_NO_DISP");
                drX("colSurnameName") = drYoyaku("SURNAME_NAME");
                drX("colNinzu") = drYoyaku("JYOSYA_NINZU");

                if (!string.IsNullOrEmpty(drYoyaku("SEISAN_HOHO").ToString) == true)
                {
                    if (drYoyaku("SEISAN_HOHO").ToString == FixedCd.SeisanHouhou.furikomi)
                        // 精算方法が「振込」の場合
                        strSeisanHoho = strSeisanHohoHurikomi;
                    else if (drYoyaku("SEISAN_HOHO").ToString == FixedCd.SeisanHouhou.eigyousyo)
                        // 精算方法が「営業所」の場合
                        strSeisanHoho = strSeisanHohoEigyosyo;
                    else if (drYoyaku("SEISAN_HOHO").ToString == FixedCd.SeisanHouhou.toujitsubarai)
                        // 精算方法が「当日払い」の場合
                        strSeisanHoho = strSeisanHohoTojituPayment;
                    else if (drYoyaku("SEISAN_HOHO").ToString == FixedCd.SeisanHouhou.agt)
                        // 精算方法が「ＡＧＴ」の場合
                        strSeisanHoho = strSeisanHohoAgt;
                    else
                        // 精算方法が「カード」の場合
                        strSeisanHoho = strSeisanHohoCard;
                }
                else
                    strSeisanHoho = "";
                // 発券状態
                // 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
                hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(drYoyaku("CANCEL_FLG").ToString, drYoyaku("ZASEKI_RESERVE_YOYAKU_FLG").ToString, drYoyaku("HAKKEN_NAIYO").ToString, drYoyaku("STATE").ToString);
                drX("colSiharaiHoho") = strSeisanHoho;
                drX("colYoyakuKbn") = drYoyaku("YOYAKU_KBN");
                drX("colYoyakuNo02") = drYoyaku("YOYAKU_NO");
                drX("colCrsCd") = drYoyaku("CRS_CD");
                drX("colGousya") = drYoyaku("GOUSYA");
                drX("colJotai") = hakkenStateArry[1];

                dtYoyaku.Rows.Add(drX);

                // 予約人数を算出
                sumYoyakuNinzu = sumYoyakuNinzu + System.Convert.ToInt32(drYoyaku("JYOSYA_NINZU"));
            }

            // グリッドに取得したデータを表示する
            grdYoyaku.DataSource = dtYoyaku;

            // 予約人数を表示
            txtNinzuTotal.Text = sumYoyakuNinzu.ToString();
        }
    }

    public void setSeFirsttDisplayData()
    {
        throw new NotImplementedException();
    }

    public void DisplayDataToEntity(ref iEntity ent)
    {
        throw new NotImplementedException();
    }

    public void EntityDataToDisplay(ref iEntity ent)
    {
        throw new NotImplementedException();
    }

    public void OldDataToEntity(DataRow pDataRow)
    {
        throw new NotImplementedException();
    }

    public bool CheckSearch()
    {
        throw new NotImplementedException();
    }

    public bool CheckInsert()
    {
        throw new NotImplementedException();
    }

    public bool CheckUpdate()
    {
        throw new NotImplementedException();
    }

    public bool isExistHissuError()
    {
        throw new NotImplementedException();
    }
}
