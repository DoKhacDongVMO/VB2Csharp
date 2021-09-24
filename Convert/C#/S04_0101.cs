using System.ComponentModel;

/// <summary>
/// 利用人員確定・修正
/// 該当コースの予約情報を一覧で表示し、予約情報の修正（利用人員の修正）を行う（＝利用人員の補正）。補正の履歴管理を行う。
/// </summary>
/// <remarks>
///    Author：2019/02/07//DTS佐藤
/// </remarks>
public class S04_0101 : PT11
{
    /// <summary>
    /// 画面ID
    /// </summary>
    private const string ScreenId = "S04_0101";
    /// <summary>
    /// 画面名
    /// </summary>
    private const string ScreenName = "利用人員確定・修正";

    /// <summary>
    /// 条件GroupBoxのTop座標
    /// </summary>
    public const TopGbxCondition = 41;
    /// <summary>
    /// 条件GroupBoxのマージン
    /// </summary>
    public const MarginGbxCondition = 6;

    /// <summary>
    /// グリッド列タイトル_予約状況
    /// </summary>
    private const string grgColTitleYoyakuNinzu = "予約人数";
    /// <summary>
    /// グリッド列タイトル_入金済
    /// </summary>
    private const string grdColTitleNyuukinAlready = "入金済";
    /// <summary>
    /// グリッド列タイトル_未入金
    /// </summary>
    private const string grdColTitleMiNyuukin = "未入金";
    /// <summary>
    /// グリッド列タイトル_チェックイン状況
    /// </summary>
    private const string grdColTitleCheckinSituation = "チェックイン状況";
    /// <summary>
    /// グリッド列タイトル_済
    /// </summary>
    private const string grdColTitleAlready = "済";
    /// <summary>
    /// グリッド列タイトル_仮
    /// </summary>
    private const string grdColTitleKari = "仮";
    /// <summary>
    /// グリッド列タイトル_未
    /// </summary>
    private const string grdColTitleMi = "未";
    /// <summary>
    /// グリッド列タイトル_NoShow
    /// </summary>
    private const string grdColTitleNoShow = "ＮｏＳｈｏｗ";
    /// <summary>
    /// グリッド列タイトル_キャンセル
    /// </summary>
    private const string grdColTitleCancel = "キャンセル";
    /// <summary>
    /// グリッド列タイトル_インファント
    /// </summary>
    private const string grdColTitleInfant = "インファント";



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

        // エラー有無のクリア
        clearError();

        // 画面終了時確認不要
        base.closeFormFlg = false;
    }

    /// <summary>
    /// 条件GroupBox表示制御ボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnVisiblerCondition_Click(object sender, EventArgs e)
    {
        this.VisibleGbxCondition = !this.VisibleGbxCondition;

        // Panel, グリッドの座標, サイズを表示/非表示に応じて変更
        if (this.VisibleGbxCondition)
        {
            // 表示状態
            this.btnVisiblerCondition.Text = "非表示 <<";

            this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
            this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
            this.grdRiyouJininKakuteiRev.Height -= (this.HeightGbxCondition - 3);
        }
        else
        {
            // 非表示状態
            this.btnVisiblerCondition.Text = "表示 >>";

            this.PanelEx1.Top = TopGbxCondition;
            this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
            this.grdRiyouJininKakuteiRev.Height += (this.HeightGbxCondition - 3);
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
    /// 検索ボタン押下時処理
    /// </summary>
    protected override void btnF8_ClickOrgProc()
    {

        // MyBase.btnF8_ClickOrgProc()

        // エラー有無のクリア
        clearError();

        // 日付入力値の調整
        setYmdFromTo();

        // [詳細エリア]検索結果部の項目初期化
        initDetailAreaItems();

        // 検索条件項目入力チェック
        if (checkSearchItems() == true)
            // エラーがない場合、検索処理を実行

            // Gridへの表示(グリッドデータの取得とグリッド表示)
            reloadGrid();
    }

    /// <summary>
    /// F8キーダウンイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void S02_1501_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.F8)
        {
            e.Handled = true;
            this.btnSearch.Select();
            this.btnSearch_Click(sender, e);
        }
    }

    /// <summary>
    /// グリッドのデータソース変更時イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
    {
        // データ件数を表示(ヘッダー行分マイナス1)
        string formatedCount = (this.grdRiyouJininKakuteiRev.Rows.Count - 2).ToString.PadLeft(6);
        lblLengthGrd.Text = formatedCount + "件";
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
    private void S02_1501_FormClosing()
    {
        this.Dispose();
    }


    /// <summary>
    /// [検索エリア]検索条件部の項目初期化
    /// </summary>
    protected override void initSearchAreaItems()
    {
        base.initSearchAreaItems();

        // 乗車地コード
        ucoJyosyaTiCd.CodeText = UserInfoManagement.eigyosyoCd;
        ucoJyosyaTiCd.ValueText = null;

        // 出発日FromTo：システム日付を設定
        dtmSyuptDayFromTo.FromDateText = CommonDateUtil.getSystemTime();
        dtmSyuptDayFromTo.ToDateText = CommonDateUtil.getSystemTime();

        // コース種別
        if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
        {
            // ユーザーが国際事業部の場合は外国語
            chkGaikokugo.Checked = true;
            chkJapanese.Checked = false;
        }
        else
        {
            // それ以外の場合は日本語をONに設定
            chkJapanese.Checked = true;
            chkGaikokugo.Checked = false;
        }

        // 上記以外の項目は全て空欄/チェックOFF
        // コースコード
        ucoCrsCd.CodeText = "";
        ucoCrsCd.ValueText = null;

        // 号車
        txtGousya.Text = "";

        // 出発時間
        dtmSyuptTime.Text = "";

        // コース区分
        chkTeikiNoon.Checked = false;
        chkTeikiNight.Checked = false;
        chkKikakuDayTrip.Checked = false;
        chkKikakuStay.Checked = false;
        chkNightLine.Checked = false;
        chkBoat.Checked = false;
        chk2StayMore.Checked = false;
        chkRCrs.Checked = false;

        // ロード時にフォーカスを設定する
        this.ActiveControl = this.ucoJyosyaTiCd;
    }


    /// <summary>
    /// エラー有無のクリア
    /// </summary>
    private void clearError()
    {

        // ExistErrorプロパティのクリア
        ucoJyosyaTiCd.ExistError = false;
        dtmSyuptDayFromTo.ExistErrorForFromDate = false;
        dtmSyuptDayFromTo.ExistErrorForToDate = false;
        chkJapanese.ExistError = false;
        chkGaikokugo.ExistError = false;

        ucoCrsCd.ExistError = false;
        txtGousya.ExistError = false;
        dtmSyuptTime.ExistError = false;

        chkTeikiNoon.ExistError = false;
        chkTeikiNight.ExistError = false;
        chkKikakuDayTrip.ExistError = false;
        chkKikakuStay.ExistError = false;
        chkNightLine.ExistError = false;
        chkBoat.ExistError = false;
        chk2StayMore.ExistError = false;
        chkRCrs.ExistError = false;

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
        setgrdRiyouJininKakuteiRev();
        this.grdRiyouJininKakuteiRev.DataSource = dt;
        this.grdRiyouJininKakuteiRev.DataMember = "";
        this.grdRiyouJininKakuteiRev.Refresh();
        this.lblLengthGrd.Text = "     0件";
    }

    /// <summary>
    /// フッタボタンの制御(表示\[活性]／非表示[非活性])
    /// </summary>
    protected override void initFooterButtonControl()
    {
        base.initFooterButtonControl();

        this.F4Key_Visible = false;       // F4:非表示
        this.F10Key_Visible = false;       // F10:非表示
        this.F11Key_Visible = false;       // F11:非表示
    }

    // 利用人員確定・修正グリッドの設定
    private void setgrdRiyouJininKakuteiRev()
    {

        // 行ヘッダを作成
        grdRiyouJininKakuteiRev.AllowDragging = (AllowDraggingEnum)false;
        grdRiyouJininKakuteiRev.AllowAddNew = false;
        grdRiyouJininKakuteiRev.AutoGenerateColumns = false;
        grdRiyouJininKakuteiRev.AllowEditing = false;

        // 行ヘッダを作成
        grdRiyouJininKakuteiRev.Styles.Normal.WordWrap = true;
        grdRiyouJininKakuteiRev.Cols.Count = 16;     // gridの列数
        grdRiyouJininKakuteiRev.Rows.Fixed = 2;      // 行固定
        grdRiyouJininKakuteiRev.Cols.Frozen = 4;     // 列固定
        grdRiyouJininKakuteiRev.AllowMerging = AllowMergingEnum.Custom;
        grdRiyouJininKakuteiRev.Rows(0).AllowMerging = true;
        grdRiyouJininKakuteiRev.Cols(0).AllowMerging = true;

        CellRange cr;

        // 出発日
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 1, 1, 1);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // 乗車地
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 2, 1, 2);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // コースコード
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 3, 1, 3);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // コース名
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 4, 1, 4);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // 出発時間
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 5, 1, 5);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // 号車
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 6, 1, 6);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        // 予約人数
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 7, 0, 8);
        cr.Data = grgColTitleYoyakuNinzu;
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        grdRiyouJininKakuteiRev(1, 7) = grdColTitleNyuukinAlready;
        grdRiyouJininKakuteiRev(1, 8) = grdColTitleMiNyuukin;
        // チェックイン状況
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 9, 0, 14);
        cr.Data = grdColTitleCheckinSituation;
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
        grdRiyouJininKakuteiRev(1, 9) = grdColTitleAlready;
        grdRiyouJininKakuteiRev(1, 10) = grdColTitleKari;
        grdRiyouJininKakuteiRev(1, 11) = grdColTitleMi;
        grdRiyouJininKakuteiRev(1, 12) = grdColTitleNoShow;
        grdRiyouJininKakuteiRev(1, 13) = grdColTitleCancel;
        grdRiyouJininKakuteiRev(1, 14) = grdColTitleInfant;

        // 利用人員
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 15, 1, 15);
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr);
    }

    /// <summary>
    /// 出発日入力値の調整
    /// </summary>
    private void setYmdFromTo()
    {
        if (dtmSyuptDayFromTo.FromDateText != null & dtmSyuptDayFromTo.ToDateText == null)
            // 出発日From <> ブランク かつ 出発日To = ブランクの場合、出発日To に 出発日From の値をセット
            dtmSyuptDayFromTo.ToDateText = dtmSyuptDayFromTo.FromDateText;
        else if (dtmSyuptDayFromTo.ToDateText != null & dtmSyuptDayFromTo.FromDateText == null)
            // 出発日To <> ブランク かつ 出発日From = ブランクの場合、出発日From に 出発日To の値をセット
            dtmSyuptDayFromTo.FromDateText = dtmSyuptDayFromTo.ToDateText;
    }

    /// <summary>
    ///     '''検索条件項目入力チェック
    /// </summary>
    /// <returns>エラーがない場合：True、エラーの場合：False</returns>
    protected override bool checkSearchItems()
    {
        base.checkSearchItems();

        // 乗車地コード入力チェック
        if (string.IsNullOrEmpty(ucoJyosyaTiCd.CodeText) == true)
        {

            // 乗車地コードが入力されていない場合、エラー        
            // 乗車地コードを入力してください。
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地コード");

            // 背景色を赤にする
            ucoJyosyaTiCd.ExistError = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.ucoJyosyaTiCd;

            return false;
        }

        // コース種別入力チェック
        if (chkJapanese.Checked == false & chkGaikokugo.Checked == false)
        {

            // コース種別が選択されていません
            CommonProcess.createFactoryMsg().messageDisp("E90_024", "コース種別");

            // 背景色を赤にする
            chkJapanese.ExistError = true;
            chkGaikokugo.ExistError = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.chkJapanese;

            return false;
        }

        // 日付入力チェック
        if (dtmSyuptDayFromTo.FromDateText == null & dtmSyuptDayFromTo.ToDateText == null)
        {

            // 日付Fromと日付Toどちらも未入力の場合、エラー
            // 日付を入力してください。
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付");

            // 背景色を赤にする
            dtmSyuptDayFromTo.ExistErrorForToDate = true;
            dtmSyuptDayFromTo.ExistErrorForFromDate = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.dtmSyuptDayFromTo;

            return false;
        }

        // 日付入力値チェック
        if (CommonDateUtil.chkDayFromTo((DateTime)dtmSyuptDayFromTo.FromDateText, (DateTime)dtmSyuptDayFromTo.ToDateText) == false)
        {

            // 日付From＞日付Toの場合、エラー
            // 日付の設定が不正です。
            CommonProcess.createFactoryMsg().messageDisp("E90_017", "日付");

            // 背景色を赤にする
            dtmSyuptDayFromTo.ExistErrorForToDate = true;
            dtmSyuptDayFromTo.ExistErrorForFromDate = true;

            // 先頭のエラー項目にフォーカスを設定する
            this.ActiveControl = this.dtmSyuptDayFromTo;

            return false;
        }

        return true;
    }

    /// <summary>
    /// Gridへの表示(グリッドデータの取得とグリッド表示)
    /// </summary>
    protected override void reloadGrid()
    {
        base.reloadGrid();

        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();
        var dt = new DataTable();

        // 日付(パラメータ)編集用変数
        DateTime dteTmpDate = default(DateTime);

        // データアクセス
        S04_0101_DA dataAccess = new S04_0101_DA();
        DataTable dataRiyouJininList = new DataTable();

        // グリッド表示用dataRow
        DataRow dr = null/* TODO Change to default(_) if this is not a reference type */;
        // dt.NewRow
        DataRow drX = null/* TODO Change to default(_) if this is not a reference type */;

        TimeSpan SyuptTime = default(TimeSpan);

        // パラメータ設定
        // 乗車地コード
        paramInfoList.Add("JyosyaTiCd", Trim(this.ucoJyosyaTiCd.CodeText));

        // 出発日
        paramInfoList.Add("SyuptDayFrom", Format(this.dtmSyuptDayFromTo.FromDateText, "yyyyMMdd"));
        paramInfoList.Add("SyuptDayTo", Format(this.dtmSyuptDayFromTo.ToDateText, "yyyyMMdd"));

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

        // コース種別
        // 日本語
        paramInfoList.Add("Japanese", Trim(System.Convert.ToString(this.chkJapanese.Checked)));
        // 外国語
        paramInfoList.Add("Gaikokugo", Trim(System.Convert.ToString(this.chkGaikokugo.Checked)));

        // コース区分
        // 定期（昼）
        paramInfoList.Add("TeikiNoon", Trim(System.Convert.ToString(this.chkTeikiNoon.Checked)));
        // 定期（夜）
        paramInfoList.Add("TeikiNight", Trim(System.Convert.ToString(this.chkTeikiNight.Checked)));
        // 企画（日帰り）
        paramInfoList.Add("KikakuDayTrip", Trim(System.Convert.ToString(this.chkKikakuDayTrip.Checked)));
        // 企画（宿泊）
        paramInfoList.Add("KikakuStay", Trim(System.Convert.ToString(this.chkKikakuStay.Checked)));
        // 夜行
        paramInfoList.Add("NightLine", Trim(System.Convert.ToString(this.chkNightLine.Checked)));
        // 船舶
        paramInfoList.Add("Boat", Trim(System.Convert.ToString(this.chkBoat.Checked)));
        // ２泊以上
        paramInfoList.Add("2StayMore", Trim(System.Convert.ToString(this.chk2StayMore.Checked)));
        // Ｒコース
        paramInfoList.Add("RCrs", Trim(System.Convert.ToString(this.chkRCrs.Checked)));

        // SQLでデータを取得
        dataRiyouJininList = dataAccess.getRiyouJininList(paramInfoList);

        // データ取得件数チェック
        if (dataRiyouJininList.Rows.Count <= 0)
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
            dt.Columns.Add("colSyuptDay");           // 出発日
            dt.Columns.Add("colJyosyaTi");           // 乗車地
            dt.Columns.Add("colCrsCd");              // コースコード
            dt.Columns.Add("colCrsName");            // コース名
            dt.Columns.Add("colSyuptTime");          // 出発時間
            dt.Columns.Add("colGousya");             // 号車
            dt.Columns.Add("colNyuukinAlready");     // 入金済
            dt.Columns.Add("colMiNyuukin");          // 未入金
            dt.Columns.Add("colCheckinAlready");     // 済
            dt.Columns.Add("colKariCheckin");        // 仮
            dt.Columns.Add("colMiCheckin");          // 未
            dt.Columns.Add("colNoShow");             // ＮｏＳｈｏｗ
            dt.Columns.Add("colCancel");             // キャンセル
            dt.Columns.Add("colInfant");             // インファント
            dt.Columns.Add("colRiyouJinin");         // 利用人員

            // 取得した値を各列に設定
            foreach (var dr in dataRiyouJininList.Rows)
            {
                drX = dt.NewRow;

                drX("colSyuptDay") = Format(dr("SYUPT_DAY"), "yy/MM/dd");
                drX("colJyosyaTi") = dr("PLACE_NAME_SHORT");
                drX("colCrsCd") = dr("CRS_CD");
                drX("colCrsName") = dr("CRS_NAME");
                drX("colSyuptTime") = dr("SYUPT_TIME");
                drX("colGousya") = dr("GOUSYA");
                drX("colNyuukinAlready") = dr("NYUUKIN_ALREADY");
                drX("colMiNyuukin") = dr("MI_NYUUKIN");
                drX("colCheckinAlready") = 0;
                drX("colKariCheckin") = 0;
                drX("colMiCheckin") = dr("MI_CHECKIN");
                drX("colNoShow") = dr("NO_SHOW");
                drX("colCancel") = dr("CANCEL");
                drX("colInfant") = dr("INFANT");
                drX("colRiyouJinin") = dr("RIYOU_JININ");

                dt.Rows.Add(drX);
            }

            // グリッドに取得したデータを表示する
            grdRiyouJininKakuteiRev.DataSource = dt;
        }
    }
}
