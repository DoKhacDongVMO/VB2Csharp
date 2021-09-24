using Microsoft.VisualBasic;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Yoyaku;
using System.ComponentModel;

/// <summary>

/// S04_0301 乗車状況照会

/// </summary>
public class S04_0301 : FormBase
{
    private DataTable searchResultGridData = new DataTable();
    private DataSet searchBusUnitData = new DataSet();
    private DataTable searchResultHakkenSituationGridData = new DataTable();
    private DataTable searchResultMihakkenYoyakuGridData = new DataTable();
    private DateTime operationDate; 
    private bool checkDisplayFlg = true; 

    // 場所マスタエンティティ
    private PlaceMasterEntity clsPlaceMasterEntity = new PlaceMasterEntity();

    // 選択行取得キー
    private string[] EntityKeys = new string[] { "CRS_CD", "GOUSYA" };
    private string selectRowSyuptTime;
    private string selectRowCrsCd;
    private int selectRowGousya;


    // 発券状況グリッドの最小行数
    private const int HakkenSituationGridMinRow = 5;
    // 発券状況グリッドの最大行数
    private const int HakkenSituationGridMaxRow = 10;
    // 発券状況（明細）グリッドの最大列数
    private const int HakkenSituationDetailGridMaxCol = 31;
    // 発券状況（横計）グリッドの最大列数
    private const int HakkenSituationYokoKeiGridMaxCol = 11;
    // 配車経由地保持の最大列数
    private const int HaisyaKeiyuMaxCount = 5;
    // Top座標
    private const int TopLblLengthGrd01 = 165;
    private const int TopGrdCrs = 186;
    private const int TopGrdHakkenSituationDetail = 186;
    private const int TopGrdHakkenSituationYokoKei = 186;
    private const int TopGrdHakkenSituationTotal = 186;
    private const int TopGrdHakkenSituationBlock = 186;
    private const int TopLblLengthGrd02 = 426;
    private const int TopGrdMihakkenYoyaku = 447;
    private const int TopLblLengthGrd02BusUnit = 165;
    private const int TopGrdMihakkenYoyakuBusUnit = 186;
    // グリッド高さ
    private const int HeightGrdCrs = 234;
    private const int HeightGrdHakkenSituationDetail = 234;
    private const int HeightGrdHakkenSituationYokoKei = 234;
    private const int HeightGrdHakkenSituationTotal = 234;
    private const int HeightGrdHakkenSituationBlock = 234;
    private const int HeightGrdMihakkenYoyaku = 301;
    private const int HeightGrdMihakkenYoyakuBusUnit = 560;
    private const int HeightCyoseiGrdCrs = 40;
    private const int HeightCyoseiGrdMihakkenYoyaku = 82;

    // 最大取得件数
    // TODO:最大取得件数指針決まり次第修正(仮対応)
    private const int limitMaxData = 10000;

    // チェックエラー表示項目
    private const string ErrorDisplaySyuptDay = "出発日";
    private const string ErrorDisplayNoribaCd = "乗車地";
    private const string ErrorDisplaySyuptTime = "出発時間";


    /// <summary>
    /// 列定義
    /// </summary>
    /// <remarks></remarks>
    private enum GrdMihakkenYoyakuColumn : int
    {
        [Value("遷移ボタン")]
        TransitionButton = 1,
        [Value("コースコード")]
        CrsCd,
        [Value("コース名")]
        CrsNm,
        [Value("出発時間")]
        SyuptTime,
        [Value("号車")]
        Gousya,
        [Value("予約番号")]
        YoyakuNumber,
        [Value("姓名")]
        SeiMei,
        [Value("予約人数")]
        YoyakuNinzu,
        [Value("PUルート")]
        PURoot,
        [Value("PU")]
        PU,
        [Value("PU出発時間")]
        PUSyuptTime,
        [Value("乗車人数")]
        JyosyaNinzu,
        [Value("状態")]
        State,
        [Value("代理店名")]
        AgentNm,
        [Value("予約区分")]
        YoyakuKbn,
        [Value("予約NO")]
        YoyakuNo,
        [Value("コース種別")]
        CrsKind,
        [Value("バス指定コード")]
        BusReserveCd
    }

    /// <summary>
    /// 行定義
    /// </summary>
    /// <remarks></remarks>
    private enum GrdHakkenSituationRow : int
    {
        [Value("料金区分名")]
        ChargeKbnNm,
        [Value("料金区分（人員）名")]
        ChargeKbnJininNm,
        [Value("未発券")]
        Mihakken,
        [Value("発券済")]
        HakkenZumi,
        [Value("縦計")]
        TateKei
    }

    /// <summary>
    /// 表示単位
    /// </summary>
    /// <remarks></remarks>
    public enum DisplayUnit : int
    {
        [Value("予約単位")]
        YoyakuUnit,
        [Value("バス単位")]
        BusUnit
    }

    public struct MihakkenYoyakuParameter
    {
        private DateTime _syuptDay;
        private string _crsCd;
        private string _crsNm;
        private int _gousya;
        private string _yoyakuNumber;
        private string _seimei;
        private string _syuptTime;
        private string _agentNm;
        private string _yoyakuKbn;
        private int _yoyakuNo;
        private string _crsKind;
        private string _busReserveCd;

        /// <summary>
        ///     出発日
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public DateTime SyuptDay
        {
            get
            {
                return _syuptDay;
            }
            set
            {
                _syuptDay = value;
            }
        }

        /// <summary>
        ///     コースコード
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string CrsCd
        {
            get
            {
                return _crsCd;
            }
            set
            {
                _crsCd = value;
            }
        }

        /// <summary>
        ///     コース名
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string CrsNm
        {
            get
            {
                return _crsNm;
            }
            set
            {
                _crsNm = value;
            }
        }

        /// <summary>
        ///     号車
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public int Gousya
        {
            get
            {
                return _gousya;
            }
            set
            {
                _gousya = value;
            }
        }

        /// <summary>
        ///     予約番号
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string YoyakuNumber
        {
            get
            {
                return _yoyakuNumber;
            }
            set
            {
                _yoyakuNumber = value;
            }
        }

        /// <summary>
        ///     姓名
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string Seimei
        {
            get
            {
                return _seimei;
            }
            set
            {
                _seimei = value;
            }
        }

        /// <summary>
        ///     出発時間
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string SyuptTime
        {
            get
            {
                return _syuptTime;
            }
            set
            {
                _syuptTime = value;
            }
        }

        /// <summary>
        ///     代理店名
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string AgentNm
        {
            get
            {
                return _agentNm;
            }
            set
            {
                _agentNm = value;
            }
        }

        /// <summary>
        ///     予約区分
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string YoyakuKbn
        {
            get
            {
                return _yoyakuKbn;
            }
            set
            {
                _yoyakuKbn = value;
            }
        }

        /// <summary>
        ///     予約NO
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public int YoyakuNo
        {
            get
            {
                return _yoyakuNo;
            }
            set
            {
                _yoyakuNo = value;
            }
        }

        /// <summary>
        ///     コース種別
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string CrsKind
        {
            get
            {
                return _crsKind;
            }
            set
            {
                _crsKind = value;
            }
        }

        /// <summary>
        ///     バス指定コード
        ///     </summary>
        ///     <value></value>
        ///     <returns></returns>
        ///     <remarks></remarks>
        public string BusReserveCd
        {
            get
            {
                return _busReserveCd;
            }
            set
            {
                _busReserveCd = value;
            }
        }
    }



    /// <summary>
    /// 条件GroupBox表示制御ボタン押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnVisiblerCondition_Click(object sender, EventArgs e)
    {

        // グリッドの座標, サイズを表示/非表示に応じて変更
        if (checkDisplayFlg == true)
        {
            // 検索条件を表示状態→非表示状態に変更
            this.btnVisiblerCondition.Text = "表示 >>";                          // ボタンのキャプション切り替え

            if (rdoDisplayTaniYoyaku.Checked == true)
            {
                // 検索条件の表示単位が「予約」の場合
                this.grdMihakkenYoyaku.Height += this.gbxCondition.Height;         // 未発券予約情報グリッドの高さを変更
                this.lblLengthGrd02.Top -= this.gbxCondition.Height;               // 未発券予約情報の件数表示ラベルの位置を変更
                this.grdMihakkenYoyaku.Top -= this.gbxCondition.Height;            // 未発券予約情報グリッドの位置を変更
            }
            else
            {
                // 検索条件の表示単位が「バス」の場合
                this.grdCrs.Height += HeightCyoseiGrdCrs;                        // コース情報グリッドの高さを変更
                this.grdHakkenSituationDetail.Height += HeightCyoseiGrdCrs;      // 発券状況（詳細）グリッドの高さを変更
                this.grdHakkenSituationYokoKei.Height += HeightCyoseiGrdCrs;     // 発券状況（横計）グリッドの高さを変更
                this.grdHakkenSituationTotal.Height += HeightCyoseiGrdCrs;       // 発券状況（合計）グリッドの高さを変更
                this.grdHakkenSituationBlock.Height += HeightCyoseiGrdCrs;       // 発券状況（ブロック）グリッドの高さを変更
                this.grdMihakkenYoyaku.Height += HeightCyoseiGrdMihakkenYoyaku;  // 未発券予約情報グリッドの高さを変更

                this.lblLengthGrd01.Top -= this.gbxCondition.Height;               // コース情報の件数表示ラベルの位置を変更
                this.grdCrs.Top -= this.gbxCondition.Height;                       // コース情報グリッドの位置を変更
                this.grdHakkenSituationDetail.Top -= this.gbxCondition.Height;     // 発券状況（詳細）グリッドの位置を変更
                this.grdHakkenSituationYokoKei.Top -= this.gbxCondition.Height;    // 発券状況（横計）グリッドの位置を変更
                this.grdHakkenSituationTotal.Top -= this.gbxCondition.Height;      // 発券状況（合計）グリッドの位置を変更
                this.grdHakkenSituationBlock.Top -= this.gbxCondition.Height;      // 発券状況（ブロック）グリッドの位置を変更
                this.lblLengthGrd02.Top -= this.gbxCondition.Height - HeightCyoseiGrdCrs;    // 未発券予約情報の件数表示ラベルの位置を変更
                this.grdMihakkenYoyaku.Top -= this.gbxCondition.Height - HeightCyoseiGrdCrs; // 未発券予約情報グリッドの位置を変更
            }

            this.gbxCondition.Visible = false;
            checkDisplayFlg = false;
        }
        else
        {
            // 検索条件を非表示状態→表示状態に変更
            this.btnVisiblerCondition.Text = "非表示 <<";

            if (rdoDisplayTaniYoyaku.Checked == true)
            {
                // 検索条件の表示単位が「予約」の場合
                this.lblLengthGrd02.Top = TopLblLengthGrd02BusUnit;              // 未発券予約情報の件数表示ラベルを元の位置に戻す
                this.grdMihakkenYoyaku.Top = TopGrdMihakkenYoyakuBusUnit;        // 未発券予約情報グリッドを元の位置に戻す
                this.grdMihakkenYoyaku.Height = HeightGrdMihakkenYoyakuBusUnit;  // 未発券予約情報グリッドの高さを元に戻す
            }
            else
            {
                // 検索条件の表示単位が「バス」の場合
                this.lblLengthGrd01.Top = TopLblLengthGrd01;                       // コース情報の件数表示ラベルを元の位置に戻す
                this.grdCrs.Top = TopGrdCrs;                                       // コース情報グリッドを元の位置に戻す
                this.grdHakkenSituationDetail.Top = TopGrdHakkenSituationDetail;   // 発券状況（詳細）グリッドを元の位置に戻す
                this.grdHakkenSituationYokoKei.Top = TopGrdHakkenSituationYokoKei; // 発券状況（横計）グリッドを元の位置に戻す
                this.grdHakkenSituationTotal.Top = TopGrdHakkenSituationTotal;     // 発券状況（合計）グリッドを元の位置に戻す
                this.grdHakkenSituationBlock.Top = TopGrdHakkenSituationBlock;     // 発券状況（ブロック）グリッドを元の位置に戻す
                this.lblLengthGrd02.Top = TopLblLengthGrd02;                       // 未発券予約情報の件数表示ラベルを元の位置に戻す
                this.grdMihakkenYoyaku.Top = TopGrdMihakkenYoyaku;                 // 未発券予約情報グリッドを元の位置に戻す
                this.grdCrs.Height = HeightGrdCrs;                                         // コース情報グリッドの高さを元に戻す
                this.grdHakkenSituationDetail.Height = HeightGrdHakkenSituationDetail;     // 発券状況（詳細）グリッドの高さを元に戻す
                this.grdHakkenSituationYokoKei.Height = HeightGrdHakkenSituationYokoKei;   // 発券状況（横計）グリッドの高さを元に戻す
                this.grdHakkenSituationTotal.Height = HeightGrdHakkenSituationTotal;       // 発券状況（合計）グリッドの高さを元に戻す
                this.grdHakkenSituationBlock.Height = HeightGrdHakkenSituationBlock;       // 発券状況（ブロック）グリッドの高さを元に戻す
                this.grdMihakkenYoyaku.Height = HeightGrdMihakkenYoyaku;                   // 未発券予約情報グリッドの高さを元に戻す
            }
            this.gbxCondition.Visible = true;
            checkDisplayFlg = true;
        }
    }


    /// <summary>
    /// 条件クリアボタン押下時
    /// </summary>
    protected override void btnCLEAR_ClickOrgProc()
    {
        // 検索条件部の項目初期化
        initSearchAreaItems();
    }


    /// <summary>
    /// F8：検索ボタン押下時の独自処理
    /// </summary>
    protected override void btnF8_ClickOrgProc()
    {
        // Dim logmsg(0) As String
        // 選択された行データ
        DataRow[] selectData = null;
        // 問合せ文字列（明細グリッド用）
        string whereString = string.Empty;

        // データテーブルの初期化
        searchResultGridData = null;
        // チェックして検索
        if (checkSearchItems() == false)
            return;

        // 対象データ取得
        searchResultGridData = getMstData();

        if (searchResultGridData == null || searchResultGridData.Rows.Count <= 0)
        {
            // 取得件数0件の場合、メッセージを表示
            CommonProcess.createFactoryMsg().messageDisp("E90_019");

            // グリッドの設定
            initGrid();
        }
        else
        {
            // 最大取得件数で絞込み
            searchResultGridData = CommonDAUtil.checkLimitData(searchResultGridData, limitMaxData);
            // 検索後処理
            reloadGridSearch();
        }
    }


    private void F8_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyData)
        {
            case object _ when Keys.F8:
                {
                    this.btnSearch.Select();
                    base.btnCom_Click(this.btnSearch, e);
                    break;
                }

            default:
                {
                    return;
                }
        }
    }


    /// <summary>
    /// グリッドのデータソース変更時イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdMihakkenYoyaku_AfterDataRefresh(object sender, ListChangedEventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        if (grd == null)
            return;

        // データ件数を表示
        ClientCommonKyushuUtil.setGridCount(grdMihakkenYoyaku, lblLengthGrd02);
    }

    /// <summary>
    /// グリッドのデータソース変更時イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdCrs_AfterDataRefresh(object sender, ListChangedEventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        if (grd == null)
            return;

        // データ件数を表示
        ClientCommonKyushuUtil.setGridCount(grdCrs, lblLengthGrd01);
    }
    /// <summary>
    /// 行選択時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks></remarks>
    private void grdCrs_Click(object sender, System.EventArgs e)
    {

        // DBパラメータ
        Hashtable paramInfoListHakkenSituation = new Hashtable();
        Hashtable paramInfoListMihakkenYoyakuInfo = new Hashtable();
        // DataAccessクラス生成
        JyosyaSituationInquiry_DA dataAccess = new JyosyaSituationInquiry_DA();

        searchBusUnitData = null;
        searchResultHakkenSituationGridData = null;
        searchResultMihakkenYoyakuGridData = null;

        // 非表示化
        grdMihakkenYoyaku.Visible = false;
        grdHakkenSituationDetail.Visible = false;
        grdHakkenSituationYokoKei.Visible = false;
        grdHakkenSituationTotal.Visible = false;
        grdHakkenSituationBlock.Visible = false;
        // lblLengthGrd01.Visible = False
        lblLengthGrd02.Visible = false;

        // 選択行データ取得
        getSelectedRowData();

        // 発券状況検索用パラメータ設定処理
        paramInfoListHakkenSituation = setParameterHakkenSituation();
        // 未発券予約情報検索用パラメータ設定処理
        paramInfoListMihakkenYoyakuInfo = setParameterMihakkenYoyaku();
        // バス単位検索処理
        searchBusUnitData = dataAccess.getBusUnitSearch(paramInfoListHakkenSituation, paramInfoListMihakkenYoyakuInfo);
        // データセットから各データテーブルへ設定
        searchResultHakkenSituationGridData = searchBusUnitData.Tables(0);
        searchResultMihakkenYoyakuGridData = searchBusUnitData.Tables(1);

        if (searchResultHakkenSituationGridData.Rows.Count <= 0 || (searchResultHakkenSituationGridData.Rows.Count > 0 && string.IsNullOrEmpty(searchResultHakkenSituationGridData.Rows(0)("JYOCHACHI_CD_1").ToString()) == true))
            // 発券状況0件の場合、メッセージを表示
            // 「該当データが存在しません。」のエラーを表示
            CommonProcess.createFactoryMsg().messageDisp("E90_019");
        else
        {
            if (searchResultMihakkenYoyakuGridData.Rows.Count <= 0)
                // 未発券予約情報0件の場合、メッセージを表示
                // 「未発券の予約はありません。」のエラーを表示
                CommonProcess.createFactoryMsg().messageDisp("E04_002");
            else
                // 最大取得件数で絞込み
                searchResultMihakkenYoyakuGridData = CommonDAUtil.checkLimitData(searchResultMihakkenYoyakuGridData, limitMaxData);
            // コース情報グリッド選択時グリッド表示
            reloadGridCourseInfoGridSelect();
        }
    }
    /// <summary>
    /// 詳細ボタン押下イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void btnGridRow_Click(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
    {
        MihakkenYoyakuParameter transferParameter = new MihakkenYoyakuParameter();
        int idx = 0;

        // 引渡パラメータ設定
        transferParameter.SyuptDay = System.Convert.ToDateTime(dtmSyuptDay.Value);
        // コースコード
        transferParameter.CrsCd = System.Convert.ToString(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsCd));
        // コース名
        transferParameter.CrsNm = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsNm).ToString();
        // 出発時間
        transferParameter.SyuptTime = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.SyuptTime).ToString();
        // 号車
        transferParameter.Gousya = System.Convert.ToInt32(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.Gousya));
        // 予約番号
        transferParameter.YoyakuNumber = System.Convert.ToString(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuNumber));
        // 姓名
        transferParameter.Seimei = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.SeiMei).ToString();
        // 代理店名
        transferParameter.AgentNm = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.AgentNm).ToString();
        // 予約区分
        transferParameter.YoyakuKbn = System.Convert.ToString(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuKbn));
        // 予約NO
        transferParameter.YoyakuNo = System.Convert.ToInt32(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuNo));
        // コース種別
        transferParameter.CrsKind = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsKind).ToString();
        // バス指定コード
        transferParameter.BusReserveCd = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.BusReserveCd).ToString();

        using (S04_0302 form = new S04_0302())
        {
            // 引渡パラメータ設定
            form.setTransferParameter(transferParameter);
            openWindow(form, true);
        }
    }



    /// <summary>
    /// F2：戻るボタン押下イベント
    /// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }





    /// <summary>
    /// フォーム起動時の独自処理
    /// </summary>
    protected override void StartupOrgProc()
    {
        // オペレーション日取得
        operationDate = CommonDateUtil.getSystemTime();
        // 検索条件部の設定
        initSearchAreaItems();
        // グリッドの設定
        initGrid();
        // フッタボタンの設定
        setButtonInitialize();

        // 検索・条件クリアボタンの関連付け
        btnSearch.Click += base.btnCom_Click;
        btnClear.Click += base.btnCom_Click;
    }

    /// <summary>
    /// 検索条件部の項目初期化
    /// </summary>
    private void initSearchAreaItems()
    {

        // 戻り値
        DataTable returnValue = null;

        // コントロール初期化
        CommonUtil.Control_Init(this.gbxCondition.Controls);
        base.clearExistErrorProperty(this.gbxCondition.Controls);

        // 出発日の表示
        dtmSyuptDay.Value = CommonDateUtil.getSystemTime();

        // 場所コード・名称の表示
        returnValue = GetDbTablePlaceMaster();
        if (returnValue.Rows.Count > 0)
        {
            {
                var withBlock = clsPlaceMasterEntity;
                ucoNoribaCd.CodeText = System.Convert.ToString(returnValue.Rows(0)(withBlock.PlaceCd.PhysicsName));
                ucoNoribaCd.ValueText = System.Convert.ToString(returnValue.Rows(0)(withBlock.PlaceName1.PhysicsName));
            }
        }

        this.dtmSyuptTimeFromTo.FromTimeValue24 = null;  // 出発時間From
        this.dtmSyuptTimeFromTo.ToTimeValue24 = null;    // 出発時間To

        // 初期フォーカスのコントロールを設定を実装
        this.dtmSyuptDay.Select();
    }

    /// <summary>
    /// フッタボタンの設定
    /// </summary>
    private void setButtonInitialize()
    {

        // Visible
        this.F2Key_Visible = true;

        this.F1Key_Visible = false;
        this.F3Key_Visible = false;
        this.F4Key_Visible = false;
        this.F5Key_Visible = false;
        this.F6Key_Visible = false;
        this.F7Key_Visible = false;
        this.F8Key_Visible = false;
        this.F9Key_Visible = false;

        this.F10Key_Visible = false;
        this.F11Key_Visible = false;
        this.F12Key_Visible = false;

        // Enabled
        this.F2Key_Enabled = true;

        // Text
        this.F2Key_Text = "F2:戻る";
    }



    /// <summary>
    /// 検索条件項目チェック
    /// </summary>
    private bool checkSearchItems()
    {
        // 判定結果
        bool returnResult = true;
        // フォーカスセットフラグ
        bool focusSetFlg = false;
        // エラー表示項目
        string errorDisplay = string.Empty;

        // 戻り値
        DataTable returnValue = null;

        // searchGridData = Nothing

        // エラーの初期化
        base.clearExistErrorProperty(this.gbxCondition.Controls);

        // 必須チェック
        if (CommonUtil.checkHissuError(this.gbxCondition.Controls) == true)
        {
            CommonProcess.createFactoryMsg().messageDisp("E90_022");
            // 必須エラーフォーカス設定（エラーが発生した先頭にフォーカスを当てる）
            if (dtmSyuptDay.ExistError == true)
            {
                focusSetFlg = true;
                dtmSyuptDay.Focus();
            }

            if (focusSetFlg == false)
            {
                if (ucoNoribaCd.ExistError == true)
                    ucoNoribaCd.Focus();
            }
            return false;
        }

        // 出発時間の大小チェック
        if (IsNothing(this.dtmSyuptTimeFromTo.FromTimeValue24) == false && IsNothing(this.dtmSyuptTimeFromTo.ToTimeValue24) == false)
        {
            if (dtmSyuptTimeFromTo.FromTimeValue24Int > dtmSyuptTimeFromTo.ToTimeValue24Int)
            {
                dtmSyuptTimeFromTo.ExistErrorForFromTime = true;
                dtmSyuptTimeFromTo.ExistErrorForToTime = true;
                dtmSyuptTimeFromTo.Focus();
                // 「{1}の設定が不正です。」のエラーを表示
                CommonProcess.createFactoryMsg().messageDisp("E90_017", ErrorDisplaySyuptTime);
                return false;
            }
        }

        return returnResult;
    }


    /// <summary>
    /// 対象マスタのデータ取得
    /// </summary>
    private DataTable getMstData()
    {
        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();
        // DataAccessクラス生成
        JyosyaSituationInquiry_DA dataAccess = new JyosyaSituationInquiry_DA();

        if (rdoDisplayTaniYoyaku.Checked == true)
        {
            // 検索条件部の表示単位で「予約」を選択時
            // 未発券予約情報検索
            paramInfoList = setParameterMihakkenYoyaku();
            return dataAccess.getMihakkenYoyaku(paramInfoList);
        }
        else
            // 検索条件部の表示単位で「バス」を選択時
            // コース情報検索
            return getDbTableCourseInformation();
    }


    /// <summary>
    /// 場所コード・名称検索処理
    /// </summary>
    /// <returns>取得データ(DataTable)</returns>
    private DataTable GetDbTablePlaceMaster()
    {
        // 戻り値
        DataTable returnValue = null;
        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();

        // DataAccessクラス生成
        Place_DA dataAccess = new Place_DA();

        // パラメータ設定
        {
            var withBlock = clsPlaceMasterEntity;
            // 会社コード
            paramInfoList.Add(withBlock.CompanyCd.PhysicsName, UserInfoManagement.companyCd);
            // 営業所コード
            paramInfoList.Add(withBlock.EigyosyoCd.PhysicsName, UserInfoManagement.eigyosyoCd);
        }


        try
        {

            // 場所マスタ検索
            returnValue = dataAccess.GetPlaceMasterDataLoginUser(paramInfoList);
        }
        catch (OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// コース情報検索処理
    /// </summary>
    /// <returns>取得データ(DataTable)</returns>
    private DataTable getDbTableCourseInformation()
    {
        // 戻り値
        DataTable returnValue = null;
        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();
        // DataAccessクラス生成
        JyosyaSituationInquiry_DA dataAccess = new JyosyaSituationInquiry_DA();

        // コース台帳（基本）エンティティ
        CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

        // パラメータ設定
        {
            var withBlock = clsCrsLedgerBasicEntity;
            // 出発日
            paramInfoList.Add(withBlock.syuptDay.PhysicsName, System.Convert.ToInt32(System.Convert.ToString(dtmSyuptDay.Value).Replace("/", "")));
            // 乗車地コード
            paramInfoList.Add(withBlock.haisyaKeiyuCd1.PhysicsName, ucoNoribaCd.CodeText);
        }
        // 出発時間（From）
        if (!dtmSyuptTimeFromTo.FromTimeValue24 == null)
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeFrom, dtmSyuptTimeFromTo.FromTimeValue24Int.ToString());
        // 出発時間（To）
        if (!dtmSyuptTimeFromTo.ToTimeValue24 == null)
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeTo, dtmSyuptTimeFromTo.ToTimeValue24Int.ToString());

        // 共通パラメータ設定
        setCommonParameter(ref paramInfoList);

        try
        {
            // コース情報検索
            returnValue = dataAccess.getCourseInformation(paramInfoList);
        }
        catch (OracleException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }

        return returnValue;
    }

    /// <summary>
    /// 未発券予約情報検索用パラメータ設定処理
    /// </summary>
    /// <returns>検索用パラメータ</returns>
    private Hashtable setParameterMihakkenYoyaku()
    {
        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();

        // コース台帳（基本）エンティティ
        CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();
        // コース台帳（ダイヤ）エンティティ
        CrsLedgerDiaEntity clsCrsLedgerDiaEntity = new CrsLedgerDiaEntity();
        // ピックアップルート台帳 （ホテル）エンティティ
        PickupRouteLedgerHotelEntity clsPickupRouteLedgerHotelEntity = new PickupRouteLedgerHotelEntity();

        // パラメータ設定
        // 検索条件部の項目より設定
        {
            var withBlock = clsCrsLedgerDiaEntity;
            // 出発日
            paramInfoList.Add(withBlock.syuptDay.PhysicsName, System.Convert.ToInt32(System.Convert.ToString(dtmSyuptDay.Value).Replace("/", "")));
            // 乗車地コード
            paramInfoList.Add(withBlock.jyochachiCd.PhysicsName, ucoNoribaCd.CodeText);
        }
        // 出発時間（From）
        if (!dtmSyuptTimeFromTo.FromTimeValue24 == null)
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeFrom, dtmSyuptTimeFromTo.FromTimeValue24Int.ToString());
        // 出発時間（To）
        if (!dtmSyuptTimeFromTo.ToTimeValue24 == null)
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeTo, dtmSyuptTimeFromTo.ToTimeValue24Int.ToString());

        // 共通パラメータ設定
        setCommonParameter(ref paramInfoList);

        // オペレーション日
        paramInfoList.Add(clsPickupRouteLedgerHotelEntity.startDay.PhysicsName, operationDate);

        // 検索条件の表示単位が「バス」の場合はグリッド選択行の値をパラメータに設定
        if (rdoDisplayTaniYoyaku.Checked == true)
            paramInfoList.Add("DisplayUnit", DisplayUnit.YoyakuUnit);
        else
        {
            paramInfoList.Add("DisplayUnit", DisplayUnit.BusUnit);

            // コース情報グリッドの項目より設定
            // 出発時間
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeGrid, selectRowSyuptTime);
            // コースコード
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamCrsCdGrid, selectRowCrsCd);
            // 号車
            paramInfoList.Add(clsCrsLedgerBasicEntity.gousya.PhysicsName, selectRowGousya);
        }

        return paramInfoList;
    }

    /// <summary>
    /// 共通パラメータ設定処理
    /// </summary>
    /// <param name="paramList">パラメータリスト</param>
    private void setCommonParameter(ref Hashtable paramList)
    {

        // コース台帳（基本）エンティティ
        CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

        // パラメータ設定
        {
            var withBlock = clsCrsLedgerBasicEntity;
            // コースコード
            paramList.Add(withBlock.crsCd.PhysicsName, ucoCrsCd.CodeText);
            // コース種別
            if (chkCrsKindJapanese.Checked == true && chkCrsKindGaikokugo.Checked == true)
                // 日本語、外国語両方選択されている場合はパラメータ未設定
                paramList.Add(withBlock.houjinGaikyakuKbn.PhysicsName, string.Empty);
            else if (chkCrsKindJapanese.Checked == true)
                paramList.Add(withBlock.houjinGaikyakuKbn.PhysicsName, HoujinGaikyakuKbnType.Houjin);
            else if (chkCrsKindGaikokugo.Checked == true)
                paramList.Add(withBlock.houjinGaikyakuKbn.PhysicsName, HoujinGaikyakuKbnType.Gaikyaku);
            else
                paramList.Add(withBlock.houjinGaikyakuKbn.PhysicsName, string.Empty);
            // 定期企画区分
            if (chkTeikiKikakuKbnTeiki.Checked == true && chkTeikiKikakuKbnKikaku.Checked == true)
                // 定期、企画両方選択されている場合はパラメータ未設定
                paramList.Add(withBlock.teikiKikakuKbn.PhysicsName, string.Empty);
            else if (chkTeikiKikakuKbnTeiki.Checked == true)
                paramList.Add(withBlock.teikiKikakuKbn.PhysicsName, System.Convert.ToInt32(FixedCd.TeikiKikakuKbn.Teiki));
            else if (chkTeikiKikakuKbnKikaku.Checked == true)
                paramList.Add(withBlock.teikiKikakuKbn.PhysicsName, System.Convert.ToInt32(FixedCd.TeikiKikakuKbn.Kikaku));
            else
                paramList.Add(withBlock.teikiKikakuKbn.PhysicsName, string.Empty);
        }
    }

    /// <summary>
    /// 発券状況検索用パラメータ設定処理
    /// </summary>
    /// <returns>検索用パラメータ</returns>
    private Hashtable setParameterHakkenSituation()
    {

        // DBパラメータ
        Hashtable paramInfoList = new Hashtable();

        // DataAccessクラス生成
        JyosyaSituationInquiry_DA dataAccess = new JyosyaSituationInquiry_DA();

        // コース台帳（基本）エンティティ
        CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

        // パラメータ設定
        // 出発日
        // 検索条件部の項目より設定
        paramInfoList.Add(clsCrsLedgerBasicEntity.syuptDay.PhysicsName, System.Convert.ToInt32(System.Convert.ToString(dtmSyuptDay.Value).Replace("/", "")));

        // コース情報グリッドの項目より設定
        // コースコード
        paramInfoList.Add(JyosyaSituationInquiry_DA.ParamCrsCdGrid, selectRowCrsCd);
        // 号車
        paramInfoList.Add(clsCrsLedgerBasicEntity.gousya.PhysicsName, selectRowGousya);

        return paramInfoList;
    }


    /// <summary>
    /// グリッド初期設定
    /// </summary>
    private void initGrid()
    {

        // 非表示化
        grdMihakkenYoyaku.Visible = false;
        grdCrs.Visible = false;
        grdHakkenSituationDetail.Visible = false;
        grdHakkenSituationYokoKei.Visible = false;
        grdHakkenSituationTotal.Visible = false;
        grdHakkenSituationBlock.Visible = false;
        lblLengthGrd01.Visible = false;
        lblLengthGrd02.Visible = false;
    }

    /// <summary>
    /// 選択行のデータを取得
    /// </summary>
    /// <remarks></remarks>
    private void getSelectedRowData()
    {
        // 選択された行データ
        DataRow[] selectData = null;
        // 問合せ文字列
        string whereString = string.Empty;

        // コース台帳（基本）エンティティ
        CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

        // 引数の設定
        whereString = CommonUnkouUtil.MakeWhere(grdCrs, EntityKeys);

        // 問合せ対象データ取得
        selectData = searchResultGridData.Select(whereString);

        if (selectData.Length > 0)
        {
            {
                var withBlock = clsCrsLedgerBasicEntity;
                selectRowSyuptTime = Replace(selectData[0].Item("SYUPT_TIME").ToString, ":", "");
                selectRowCrsCd = selectData[0].Item(withBlock.crsCd.PhysicsName).ToString;
                selectRowGousya = System.Convert.ToInt32(selectData[0].Item(withBlock.gousya.PhysicsName));
            }
        }
    }

    /// <summary>
    /// 検索ボタン押下時グリッド表示
    /// </summary>
    private void reloadGridSearch()
    {
        if (rdoDisplayTaniYoyaku.Checked == true)
            // 検索条件部の表示単位で「予約」を選択時
            // 未発券予約情報グリッドへの設定
            setMihakkenYoyakuInfo(searchResultGridData);
        else
            // 検索条件部の表示単位で「バス」を選択時
            // コース情報グリッドへの設定
            this.grdCrs.DataSource = searchResultGridData;

        // グリッド初期設定
        initGrid();

        // 検索ボタン押下時グリッド初期設定
        initGridSearch();
    }

    /// <summary>
    /// コース情報グリッド選択時グリッド表示
    /// </summary>
    private void reloadGridCourseInfoGridSelect()
    {
        if (searchResultMihakkenYoyakuGridData.Rows.Count > 0)
            // 未発券予約情報グリッドへの設定
            setMihakkenYoyakuInfo(searchResultMihakkenYoyakuGridData);
        else
        {
        }

        // 発券状況グリッドへの設定
        setHakkenSituationDetailGrid();

        // コース情報グリッド選択時グリッド初期設定
        initGridCourseInfoGridSelect();
    }

    /// <summary>
    /// 検索ボタン押下時グリッド初期設定
    /// </summary>
    private void initGridSearch()
    {
        int saveIdx = 0;
        int count = 1;

        if (rdoDisplayTaniYoyaku.Checked == true)
        {
            // 検索条件部の表示単位で「予約」を選択時

            // 未発券予約情報グリッド初期設定
            initMihakkenYoyakuGrid(165, 186, 560);

            // セルボタン配置（同一の予約番号が続いた場合は1行目のみにボタンを表示）
            ClientCommonKyushuUtil.setGridCellButton(grdMihakkenYoyaku, 6, 1);
        }
        else
        {
            // 検索条件部の表示単位で「バス」を選択時

            // コース情報グリッド初期設定
            // ヘッダ設定
            grdCrs.SetData(grdCrs.GetCellRange(0, 1), "コース" + Constants.vbCrLf + "コード");
            grdCrs.Rows(0).Height = 37;

            // 行選択モード
            grdCrs.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
            // 並び替え不可
            grdCrs.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            // 編集不可
            grdCrs.AllowEditing = false;
            // 自動列生成なし
            grdCrs.AutoGenerateColumns = false;
            // グリッド表示
            grdCrs.Visible = true;
            // 件数ラベル表示
            lblLengthGrd01.Visible = true;
        }
    }

    /// <summary>
    /// コース情報グリッド選択時グリッド初期設定
    /// </summary>
    private void initGridCourseInfoGridSelect()
    {

        // '発券状況グリッド
        // 行選択モード
        grdHakkenSituationDetail.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
        grdHakkenSituationYokoKei.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
        grdHakkenSituationTotal.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
        grdHakkenSituationBlock.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
        // 並び替え不可
        grdHakkenSituationDetail.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        grdHakkenSituationYokoKei.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        grdHakkenSituationTotal.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        grdHakkenSituationBlock.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        // 編集不可
        grdHakkenSituationDetail.AllowEditing = false;
        grdHakkenSituationYokoKei.AllowEditing = false;
        grdHakkenSituationTotal.AllowEditing = false;
        grdHakkenSituationBlock.AllowEditing = false;
        // 自動列生成なし
        grdHakkenSituationDetail.AutoGenerateColumns = false;
        grdHakkenSituationYokoKei.AutoGenerateColumns = false;
        grdHakkenSituationTotal.AutoGenerateColumns = false;
        grdHakkenSituationBlock.AutoGenerateColumns = false;
        // ソート不可
        grdHakkenSituationDetail.AllowSorting = (C1.Win.C1FlexGrid.AllowSortingEnum)false;
        grdHakkenSituationYokoKei.AllowSorting = (C1.Win.C1FlexGrid.AllowSortingEnum)false;
        grdHakkenSituationTotal.AllowSorting = (C1.Win.C1FlexGrid.AllowSortingEnum)false;
        grdHakkenSituationBlock.AllowSorting = (C1.Win.C1FlexGrid.AllowSortingEnum)false;
        // グリッド表示
        grdHakkenSituationDetail.Visible = true;
        grdHakkenSituationYokoKei.Visible = true;
        grdHakkenSituationTotal.Visible = true;
        grdHakkenSituationBlock.Visible = true;

        // '固定ヘッダ設定
        // 明細グリッド
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(0, 0), "料金区分");
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(1, 0), "料金区分（人員）");
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(2, 0), "未発券");
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(3, 0), "発券済");
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(4, 0), "縦計");

        // 横計グリッド
        for (int idx = 1; idx <= grdHakkenSituationYokoKei.Cols.Count - 1; idx++)
            grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(0, idx), "横計");

        // '文字位置設定
        // 行ヘッダ（左寄せ）
        grdHakkenSituationDetail.Cols(0).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;

        // 明細グリッド
        for (int colIdx = 1; colIdx <= grdHakkenSituationDetail.Cols.Count - 1; colIdx++)
        {
            // ヘッダ（中央寄せ）
            grdHakkenSituationDetail.Cols(colIdx).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
            // データセル（右寄せ）
            grdHakkenSituationDetail.Cols(colIdx).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightCenter;
        }
        // 横計グリッド
        for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
        {
            // ヘッダ（中央寄せ）
            grdHakkenSituationYokoKei.Cols(colIdx).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
            // データセル（右寄せ）
            grdHakkenSituationYokoKei.Cols(colIdx).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightCenter;
        }

        // 'セル結合設定
        // 料金区分の行で隣接するセルが同一の値の場合にマージする
        grdHakkenSituationDetail.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.FixedOnly;
        grdHakkenSituationDetail.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Free;
        grdHakkenSituationDetail.Rows(0).AllowMerging = true;

        // 発券状況グリッド（横計）のヘッダセル結合
        grdHakkenSituationYokoKei.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Free;
        grdHakkenSituationYokoKei.Rows(0).AllowMerging = true;

        // 発券状況グリッド（合計）のヘッダセル結合
        grdHakkenSituationTotal.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
        grdHakkenSituationTotal.Cols(1).AllowMerging = true;
        grdHakkenSituationTotal.MergedRanges.Add(grdHakkenSituationTotal.GetCellRange(0, 1, 1, 1));

        // 発券状況グリッド（営業所ブロック）のヘッダセル結合
        grdHakkenSituationBlock.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
        grdHakkenSituationBlock.Cols(1).AllowMerging = true;
        grdHakkenSituationBlock.MergedRanges.Add(grdHakkenSituationBlock.GetCellRange(0, 1, 1, 1));

        // 背景色設定
        grdHakkenSituationBlock.Styles.Add("BackColorStyle");
        grdHakkenSituationBlock.Styles("BackColorStyle").BackColor = System.Drawing.SystemColors.ControlLight;
        for (int i = 2; i <= 3; i++)
            grdHakkenSituationBlock.Rows(i).Style = grdHakkenSituationBlock.Styles("BackColorStyle");
        for (int i = 5; i <= grdHakkenSituationBlock.Rows.Count - 1; i++)
            grdHakkenSituationBlock.Rows(i).Style = grdHakkenSituationBlock.Styles("BackColorStyle");

        // '未発券予約情報グリッド
        // 'コントロール配置設定
        if (searchResultMihakkenYoyakuGridData.Rows.Count > 0)
        {
            // 検索条件の表示状態により未発券予約情報の件数ラベル、グリッドの座標, サイズを表示/非表示に応じて変更
            if (checkDisplayFlg == true)
                // 検索条件表示状態
                initMihakkenYoyakuGrid(TopLblLengthGrd02, TopGrdMihakkenYoyaku, HeightGrdMihakkenYoyaku);
            else
                // 検索条件非表示状態
                initMihakkenYoyakuGrid(TopLblLengthGrd02 - this.gbxCondition.Height + HeightCyoseiGrdCrs, TopGrdMihakkenYoyaku - this.gbxCondition.Height + HeightCyoseiGrdCrs, HeightGrdMihakkenYoyaku + HeightCyoseiGrdMihakkenYoyaku);

            // セルボタン配置（同一の予約番号が続いた場合は1行目のみにボタンを表示）
            ClientCommonKyushuUtil.setGridCellButton(grdMihakkenYoyaku, GrdMihakkenYoyakuColumn.YoyakuNumber, GrdMihakkenYoyakuColumn.TransitionButton);
        }
    }

    /// <summary>
    /// 未発券予約情報表示
    /// </summary>
    /// <param name="paramGridData">データテーブル</param>
    private void setMihakkenYoyakuInfo(DataTable paramGridData)
    {
        string editYoyakuNumber = string.Empty;
        int idxRow = 1;
        string[] hakkenStateArry = null;

        // 予約情報（基本）エンティティ
        YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

        foreach (DataRow row in paramGridData.Rows)
        {
            // 予約番号をカンマ編集後設定
            {
                var withBlock = clsYoyakuInfoBasicEntity;
                if (!string.IsNullOrEmpty(row(withBlock.yoyakuKbn.PhysicsName).ToString()))
                {
                    // 共通処理にてカンマ編集（パラメータ：予約区分、予約NOを文字列結合した値）
                    // editYoyakuNumber = CommonCheckUtil.editYoyakuNo(row(.yoyakuKbn.PhysicsName).ToString() & CType(String.Format("{0:D9}", CType(row(.yoyakuNo.PhysicsName).ToString(), Integer)), String))
                    editYoyakuNumber = CommonRegistYoyaku.createManagementNumber(row(withBlock.yoyakuKbn.PhysicsName).ToString(), System.Convert.ToInt32(row(withBlock.yoyakuNo.PhysicsName).ToString()));
                    row("YOYAKU_NUMBER") = editYoyakuNumber;
                }
                // 状態に表示する文言を取得後設定
                // 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
                hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(row(withBlock.cancelFlg.PhysicsName).ToString(), row(withBlock.zasekiReserveYoyakuFlg.PhysicsName).ToString(), row(withBlock.hakkenNaiyo.PhysicsName).ToString(), row(withBlock.state.PhysicsName).ToString());
                row("STATE_NAME") = hakkenStateArry[1];

                idxRow += 1;
            }
        }

        // 未発券予約情報グリッドへのデータバインド
        this.grdMihakkenYoyaku.DataSource = paramGridData;
    }

    /// <summary>
    /// 未発券予約情報グリッド初期設定
    /// </summary>
    /// <param name="pramLabelYLocation">件数ラベルY座標</param>
    /// <param name="pramGridYLocation">グリッドY座標</param>
    /// <param name="pramGridHeight">グリッド高さ</param>
    private void initMihakkenYoyakuGrid(int pramLabelYLocation, int pramGridYLocation, int pramGridHeight)
    {

        // 行選択モード
        grdMihakkenYoyaku.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
        // 並び替え不可
        grdMihakkenYoyaku.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        // 編集不可
        for (int i = GrdMihakkenYoyakuColumn.CrsCd; i <= GrdMihakkenYoyakuColumn.AgentNm; i++)
            grdMihakkenYoyaku.Cols(i).AllowEditing = false;
        // 自動列生成なし
        grdMihakkenYoyaku.AutoGenerateColumns = false;
        // グリッド表示
        grdMihakkenYoyaku.Visible = true;
        // 件数ラベル表示
        lblLengthGrd02.Visible = true;

        // 未発券予約情報の件数ラベルの位置を移動
        this.lblLengthGrd02.Location = new System.Drawing.Point(1185, pramLabelYLocation);
        // 未発券予約情報グリッドの位置を移動
        this.grdMihakkenYoyaku.Location = new System.Drawing.Point(8, pramGridYLocation);
        // 未発券予約情報グリッドの高さを変更
        this.grdMihakkenYoyaku.Height = pramGridHeight;

        // ヘッダ設定
        var range = grdMihakkenYoyaku.GetCellRange(0, 1);
        grdMihakkenYoyaku.Rows(0).Height = 37;
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.CrsCd), "コース" + Constants.vbCrLf + "コード");
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.SyuptTime), "出発" + Constants.vbCrLf + "時間");
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.PURoot), "PU" + Constants.vbCrLf + "ルート");
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.PUSyuptTime), "PU" + Constants.vbCrLf + "出発時間");
    }

    /// <summary>
    /// 発券状況グリッド設定
    /// </summary>
    private void setHakkenSituationDetailGrid()
    {
        // 選択された行データ
        DataRow[] selectData = null;
        // 問合せ文字列（明細グリッド用）
        string whereString = string.Empty;
        // グリッド列数
        int colCount = 0;
        // 縦計
        int tateKei = 0;
        // 発券計
        int hakkenKei = 0;
        // 配車経由地表示フラグ
        bool displayHaisyaKeiyuchiFlag = false;
        // 発券状況データ件数
        int hakkenDetailCount = 0;
        // 未発券人数
        int miHakkenNinzuCount = 0;
        // 発券済人数
        int hakkenZumiNinzuCount = 0;
        // キー比較結果フラグ
        bool sameValueFlag = false;
        // 発券状況データインデックス
        int gridRowIdx = 0;
        // 合計グリッド集計用
        int totalCount = 0;

        try
        {
            // グリッド初期化
            // 明細グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationDetail, 0);
            // 横計グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationYokoKei, 1);
            // 合計グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationTotal, 1);
            // 営業所ブロックグリッド初期化
            grdHakkenSituationBlock.SetData(grdHakkenSituationBlock.GetCellRange(4, 1), string.Empty);

            // 各グリッドの行数初期設定（固定行5行＋配車経由地：最大4行＋発券計行1行＝最大10行）
            grdHakkenSituationDetail.Rows.Count = HakkenSituationGridMaxRow;
            grdHakkenSituationYokoKei.Rows.Count = HakkenSituationGridMaxRow;
            grdHakkenSituationTotal.Rows.Count = HakkenSituationGridMaxRow;
            grdHakkenSituationBlock.Rows.Count = HakkenSituationGridMaxRow;
            // 明細グリッドの列数初期設定（料金区分：最大3個×料金区分（人員）：最大10個＋行ヘッダ＝最大31個）
            grdHakkenSituationDetail.Cols.Count = HakkenSituationDetailGridMaxCol;
            // 横計グリッドの列数初期設定（料金区分（人員）：最大10個＋行ヘッダ＝最大11個）
            grdHakkenSituationYokoKei.Cols.Count = HakkenSituationYokoKeiGridMaxCol;

            // '明細グリッド（料金区分名、料金区分（人員）名）の設定
            foreach (DataRow row in searchResultHakkenSituationGridData.Rows)
            {
                sameValueFlag = false;
                for (int colIdx = 1; colIdx <= grdHakkenSituationDetail.Cols.Count - 1; colIdx++)
                {
                    // 料金区分、料金区分（人員）が既に設定されている場合は読み飛ばす
                    if (System.Convert.ToString(grdHakkenSituationDetail.GetData(0, colIdx)) == (row("CHARGE_NAME").ToString()) & System.Convert.ToString(grdHakkenSituationDetail.GetData(1, colIdx)) == (row("CHARGE_KBN_JININ_NAME").ToString()))
                    {
                        sameValueFlag = true;
                        break;
                    }
                }

                if (sameValueFlag == false)
                {
                    colCount += 1;
                    // 料金区分名
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.ChargeKbnNm, colCount), row("CHARGE_NAME").ToString());
                    // 料金区分（人員）名
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.ChargeKbnJininNm, colCount), row("CHARGE_KBN_JININ_NAME").ToString());
                }
            }
            // 明細グリッドの列数設定
            grdHakkenSituationDetail.Cols.Count = colCount + 1;

            // '横計グリッド（料金区分（人員）名）の設定
            colCount = 0;
            foreach (DataRow row in searchResultHakkenSituationGridData.Rows)
            {
                sameValueFlag = false;
                for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
                {
                    // 料金区分（人員）が既に設定されている場合は読み飛ばす
                    if (System.Convert.ToString(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx)) == (row("CHARGE_KBN_JININ_NAME").ToString()))
                    {
                        sameValueFlag = true;
                        break;
                    }
                }

                if (sameValueFlag == false)
                {
                    colCount += 1;
                    // 料金区分（人員）名
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.ChargeKbnJininNm, colCount), row("CHARGE_KBN_JININ_NAME").ToString());
                }
            }
            // 横計グリッドの列数設定
            grdHakkenSituationYokoKei.Cols.Count = colCount + 1;

            // 検索条件部に指定された乗車地の人数設定
            // 条件の設定
            whereString = "JYOCHACHI_CD_1 = '" + ucoNoribaCd.CodeText + "'";
            // 問合せ対象データ取得
            selectData = searchResultHakkenSituationGridData.Select(whereString);

            // '明細グリッド（未発券人数、発券済人数、縦計）の設定
            for (int colIdx = 1; colIdx <= grdHakkenSituationDetail.Cols.Count - 1; colIdx++)
            {
                miHakkenNinzuCount = 0;
                hakkenZumiNinzuCount = 0;
                foreach (DataRow row in selectData)
                {
                    // 料金区分、料金区分（人員）単位に未発券人数と発券済人数を集計
                    if (System.Convert.ToString(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnNm, colIdx)) == (row("CHARGE_NAME").ToString()) && System.Convert.ToString(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx)) == (row("CHARGE_KBN_JININ_NAME").ToString()))
                    {
                        miHakkenNinzuCount += System.Convert.ToInt32(row("MIHAKKEN_NINZU").ToString());
                        hakkenZumiNinzuCount += System.Convert.ToInt32(row("HAKKEN_NINZU").ToString());
                    }
                }
                // 未発券人数
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.Mihakken, colIdx), miHakkenNinzuCount);
                // 発券済人数
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.HakkenZumi, colIdx), hakkenZumiNinzuCount);
                // 縦計
                tateKei = miHakkenNinzuCount + hakkenZumiNinzuCount;
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.TateKei, colIdx), tateKei);
            }

            // '横計グリッド（未発券人数、発券済人数、縦計）の設定
            for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
            {
                miHakkenNinzuCount = 0;
                hakkenZumiNinzuCount = 0;
                foreach (DataRow row in selectData)
                {
                    // 料金区分（人員）単位に未発券人数と発券済人数を集計
                    if (System.Convert.ToString(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx)) == (row("CHARGE_KBN_JININ_NAME").ToString()))
                    {
                        miHakkenNinzuCount += System.Convert.ToInt32(row("MIHAKKEN_NINZU").ToString());
                        hakkenZumiNinzuCount += System.Convert.ToInt32(row("HAKKEN_NINZU").ToString());
                    }
                }

                // 未発券人数
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.Mihakken, colIdx), miHakkenNinzuCount);
                // 発券済人数
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.HakkenZumi, colIdx), hakkenZumiNinzuCount);
                // 縦計
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.TateKei, colIdx), miHakkenNinzuCount + hakkenZumiNinzuCount);
            }

            // 配車経由地の人数設定（配車経由地が存在する場合のみ）
            for (int idx = 1; idx <= HaisyaKeiyuMaxCount; idx++)
            {
                // 条件の設定
                whereString = "JYOCHACHI_CD_1 = HAISYA_KEIYU_CD_" + idx + " And JYOCHACHI_CD_1 <> '" + ucoNoribaCd.CodeText + "'";
                // 問合せ対象データ取得
                selectData = searchResultHakkenSituationGridData.Select(whereString);

                if (selectData.Count <= 0)
                    // 条件に合致しない場合は次のループ処理を行う
                    continue;

                // 配車経由地の表示ありのためフラグをONにする
                displayHaisyaKeiyuchiFlag = true;

                // 設定行を決定
                for (int rowIdx = HakkenSituationGridMinRow; rowIdx <= grdHakkenSituationDetail.Rows.Count - 1; rowIdx++)
                {
                    if (grdHakkenSituationDetail.GetData(rowIdx, 0) == null || string.IsNullOrEmpty(System.Convert.ToString(grdHakkenSituationDetail.GetData(rowIdx, 0))))
                    {
                        gridRowIdx = rowIdx;
                        break;
                    }
                }

                // 明細グリッド（発券済人数）の設定
                hakkenDetailCount = 0;
                for (int colIdx = 1; colIdx <= grdHakkenSituationDetail.Cols.Count - 1; colIdx++)
                {
                    hakkenZumiNinzuCount = 0;
                    foreach (DataRow row in selectData)
                    {
                        hakkenDetailCount += 1;

                        if (hakkenDetailCount == 1)
                            // 配車経由地名
                            grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx, 0), row("PLACE_NAME_SHORT").ToString());

                        // 料金区分、料金区分（人員）単位に発券済人数を集計
                        if (System.Convert.ToString(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnNm, colIdx)) == row("CHARGE_NAME").ToString() && System.Convert.ToString(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx)) == row("CHARGE_KBN_JININ_NAME").ToString())
                            hakkenZumiNinzuCount += System.Convert.ToInt32(row("HAKKEN_NINZU").ToString());
                    }

                    // 発券済人数
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx, colIdx), hakkenZumiNinzuCount);
                }

                // 横計グリッド（発券済人数）の設定
                for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
                {
                    hakkenZumiNinzuCount = 0;
                    foreach (DataRow row in selectData)
                    {

                        // 料金区分（人員）単位に発券済人数を集計
                        if (System.Convert.ToString(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx)) == row("CHARGE_KBN_JININ_NAME").ToString())
                            hakkenZumiNinzuCount += System.Convert.ToInt32(row("HAKKEN_NINZU").ToString());
                    }

                    // 発券済人数
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(gridRowIdx, colIdx), hakkenZumiNinzuCount);
                }
            }

            if (displayHaisyaKeiyuchiFlag == true)
            {
                // 配車経由地の表示ありの場合

                // 行数の設定（データ件数＋ヘッダ行（1行）＋発券計行（1行））
                grdHakkenSituationDetail.Rows.Count = gridRowIdx + 2;
                grdHakkenSituationYokoKei.Rows.Count = gridRowIdx + 2;
                grdHakkenSituationTotal.Rows.Count = gridRowIdx + 2;
                grdHakkenSituationBlock.Rows.Count = gridRowIdx + 2;

                // 発券計行の設定
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx + 1, 0), "発券計");

                // 明細グリッド（発券計）の設定
                for (int colIdx = 1; colIdx <= grdHakkenSituationDetail.Cols.Count - 1; colIdx++)
                {
                    hakkenKei = 0;
                    // 検索条件部に指定された乗車地の発券済人数を加算
                    hakkenKei += System.Convert.ToInt32(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.HakkenZumi, colIdx));
                    // 配車経由地の発券済人数を加算
                    for (int rowIdx = 5; rowIdx <= gridRowIdx; rowIdx++)
                        hakkenKei += System.Convert.ToInt32(grdHakkenSituationDetail.GetData(rowIdx, colIdx));

                    // 発券計
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx + 1, colIdx), hakkenKei);
                }

                // 横計グリッド（発券計）の設定
                for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
                {
                    hakkenKei = 0;
                    // 検索条件部に指定された乗車地の発券済人数を加算
                    hakkenKei += System.Convert.ToInt32(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.HakkenZumi, colIdx));
                    // 配車経由地の発券済人数を加算
                    for (int rowIdx = 5; rowIdx <= gridRowIdx; rowIdx++)
                        hakkenKei += System.Convert.ToInt32(grdHakkenSituationYokoKei.GetData(rowIdx, colIdx));

                    // 発券計
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(gridRowIdx + 1, colIdx), hakkenKei);
                }
            }
            else
            {
                // 配車経由地の表示なしの場合

                // 行数の設定（固定行5行のみを表示）
                grdHakkenSituationDetail.Rows.Count = HakkenSituationGridMinRow;
                grdHakkenSituationYokoKei.Rows.Count = HakkenSituationGridMinRow;
                grdHakkenSituationTotal.Rows.Count = HakkenSituationGridMinRow;
                grdHakkenSituationBlock.Rows.Count = HakkenSituationGridMinRow;
            }

            // '合計グリッドの設定
            for (int rowIdx = GrdHakkenSituationRow.Mihakken; rowIdx <= grdHakkenSituationYokoKei.Rows.Count - 1; rowIdx++)
            {
                totalCount = 0;
                // 横計グリッドの1列ごとの人数を集計
                for (int colIdx = 1; colIdx <= grdHakkenSituationYokoKei.Cols.Count - 1; colIdx++)
                    totalCount += System.Convert.ToInt32(grdHakkenSituationYokoKei.GetData(rowIdx, colIdx));

                // 合計
                grdHakkenSituationTotal.SetData(grdHakkenSituationTotal.GetCellRange(rowIdx, 1), totalCount);
            }

            // '営業所ブロックグリッドの設定
            grdHakkenSituationBlock.SetData(grdHakkenSituationBlock.GetCellRange(4, 1), searchResultHakkenSituationGridData(0)("EI_BLOCK").ToString());
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// 発券状況グリッド初期化
    /// </summary>
    /// <param name="grid">グリッドコントロール名</param>
    /// <param name="idxStart">インデックス開始位置</param>
    private void initHakkenSituationGrid(C1.Win.C1FlexGrid.C1FlexGrid grid, int idxStart)
    {
        for (int colIdx = idxStart; colIdx <= grid.Cols.Count - 1; colIdx++)
        {
            for (int rowIdx = idxStart; rowIdx <= grid.Rows.Count - 1; rowIdx++)
                grid.SetData(grid.GetCellRange(rowIdx, colIdx), string.Empty);
        }
    }
}
