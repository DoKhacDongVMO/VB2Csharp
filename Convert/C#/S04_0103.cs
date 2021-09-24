using System.ComponentModel;
using System.Drawing;

/// <summary>

/// S04_0103 船車券AGT乗車確認出力

/// </summary>
public class S04_0103 : PT21
{

    /// <summary>
    /// 画面PgmId
    /// </summary>
    private const string PgmId = "S04_0103";
    /// <summary>
    /// 画面名
    /// </summary>
    private const string ScreenName = "船車券AGT乗車確認出力";
    /// <summary>
    /// 条件GroupBoxのTop座標
    /// </summary>
    public const TopGbxCondition = 41;
    /// <summary>
    /// 条件GroupBoxのマージン
    /// </summary>
    public const MarginGbxCondition = 6;
    /// <summary>
    /// 条件GroupBox非表示時のGrouBoxAreaの高さ
    /// </summary>
    public const HeightGbxAreasOnNotVisibleCondition = 348;
    /// <summary>
    /// 件数 件
    /// </summary>
    private const Ken = "件";
    /// <summary>
    /// 件数の初期表示
    /// </summary>
    private const ZeroKen = "0件";
    /// <summary>
    /// 日付初期値
    /// </summary>
    private const DayInitialValue = "__/__/__";
    /// <summary>
    /// 時間初期値
    /// </summary>
    private const TimeInitialValue = "__:__";
    /// <summary>
    /// チェックインフラグ　乗車済み
    /// </summary>
    private const CheckinFlgJyosyaAlready = "1";
    /// <summary>
    /// チェックインフラグ　NOSHOW
    /// </summary>
    private const CheckinFlgNOSHOW = "2";
    /// <summary>
    /// NOSHOWフラグ　"Y"
    /// </summary>
    private const NOSHOWFlg = "Y";
    /// <summary>
    /// 列名 乗車済み
    /// </summary>
    private const NameColJyosyaAlready = "colJyosyaAlready";
    /// <summary>
    /// 列名 NOSHOW
    /// </summary>
    private const NameColNOSHOW = "colNOSHOW";
    /// <summary>
    /// 列名 請求対象
    /// </summary>
    private const NameColInquiry = "colInquiry";
    /// <summary>
    /// 状態　JTB発券
    /// </summary>
    private const StateJTB = "1";
    /// <summary>
    /// 状態　KNT発券
    /// </summary>
    private const StateKNT = "3";
    /// <summary>
    /// 前月
    /// </summary>
    private const BfMon = -1;
    /// <summary>
    /// 1日前
    /// </summary>
    private const OneDayBf = -1;
    /// <summary>
    /// 状態　「消」
    /// </summary>
    private const StateCancel = "消";
    /// <summary>
    /// 状態　「削」
    /// </summary>
    private const StateDelete = "削";
    /// <summary>
    /// 状態　「指」
    /// </summary>
    private const StateReserve = "指";
    /// <summary>
    /// 状態　「券」
    /// </summary>
    private const StateKen = "券";
    /// <summary>
    /// 検索結果最大表示件数件数
    /// </summary>
    public const int MaxKensu = 100;

    /// <summary>
    /// ハッシュキー　システム日付（日付型)
    /// </summary>
    private const KeyDtSysDate = "dtSysDate";
    /// <summary>
    /// ハッシュキー　システム日付（文字列型)
    /// </summary>
    private const KeyStrSysDate = "strSysDate";
    /// <summary>
    /// ハッシュキー　システム日付（数値型)
    /// </summary>
    private const KeyIntSysDate = "intSysDate";
    /// <summary>
    /// ハッシュキー　システム時刻（時分秒）（文字列型)
    /// </summary>
    private const KeyStrSysTimeHhMmSs = "strSysTimeHhMmSs";
    /// <summary>
    /// ハッシュキー　システム時刻（時分秒）（数値型)
    /// </summary>
    private const KeyIntSysTimeHhMmSs = "intSysTimeHhMmSs";
    /// <summary>
    /// ハッシュキー　システム時刻（時分）（文字列型)
    /// </summary>
    private const KeyStrSysTimeHhMm = "strSysTimeHhMm";
    /// <summary>
    /// ハッシュキー　システム時刻（時分）（数値型)
    /// </summary>
    private const KeyIntSysTimeHhMm = "intSysTimeHhMm";



    /// <summary>
    /// コース情報テーブル
    /// </summary>
    private DataTable _crsInfoTable = null;
    /// <summary>
    /// 予約情報テーブル
    /// </summary>
    private DataTable _yoyakuInfoTable = null;
    /// <summary>
    /// 検索用テーブル
    /// </summary>
    private Hashtable _searchForTable = null;
    /// <summary>
    /// 予約情報（基本）エンティティ
    /// </summary>
    private YoyakuInfoBasicEntity _yoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
    /// <summary>
    /// 精算情報エンティティ
    /// </summary>
    private TSeisanInfoEntity _seisanInfoEntity = new TSeisanInfoEntity();
    /// <summary>
    /// 判定用リスト
    /// </summary>
    private Hashtable _judgmentForList = new Hashtable();
    /// <summary>
    /// チェックボックス解除フラグ
    /// </summary>
    private bool _releaseFlg = false;

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
    /// 画面起動時処理
    /// </summary>
    protected override void StartupOrgProc()
    {

        // 画面初期化
        this.setControlInitiarize();
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
        int harfOffset = System.Convert.ToInt32(offSet / (double)2);

        // GrpBoxArea1, 2の座標, サイズを表示/非表示に応じて変更
        if (this.VisibleGbxCondition)
        {
            // 表示状態
            this.btnVisiblerCondition.Text = "非表示 <<";

            this.setGrpLayout();
        }
        else
        {
            // 非表示状態
            this.btnVisiblerCondition.Text = "表示 >>";
            this.gbxArea1.Height = HeightGbxAreasOnNotVisibleCondition;
            this.gbxArea2.Height = HeightGbxAreasOnNotVisibleCondition;

            this.gbxArea1.Top = TopGbxCondition;
            this.gbxArea2.Top = TopGbxCondition + this.gbxArea1.Height + MarginGbxCondition;
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
    /// キーダウン
    /// </summary>
    /// <remarks></remarks>
    private void F8_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyData)
        {
            case Keys.F8:
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
    /// F8：検索ボタン押下イベント
    /// </summary>
    protected override void btnF8_ClickOrgProc()
    {

        // エラーのクリア
        this.errorClear();

        // 入力項目のチェック
        if (this.isInputCheck() == false)
            return;

        // コースグリッドの初期化
        this.setGrdCrsInitiarize();

        // 予約グリッドの初期化
        this.setGrdYoyakuInitiarize();

        // 検索用の値を格納
        this.setSerchForValue();

        // コース情報テーブルの取得
        this.getCrsInfoTable();

        // 取得したデータが0件の場合
        if (_crsInfoTable.Rows.Count <= 0)
        {
            // メッセージ出力（該当するデータがありません。）
            CommonProcess.createFactoryMsg().messageDisp("E90_019");

            return;
        }

        // 表示件数オーバーの場合
        if (_crsInfoTable.Rows.Count > MaxKensu)
            // メッセージ出力（検索結果が最大設定可能数を超えました。）
            CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");

        // コースグリッドに値を格納
        this.setGrdCrs();

        // 予約番号が入力されている場合
        Hashtable yoyakuKbnNo = this.getYoyakuNo();

        if (!string.IsNullOrEmpty(yoyakuKbnNo("yoyakuNo").ToString))
            // 全選択ボタンを押下
            this.btnAllSelectionCrs.PerformClick();

        // 検索ボタンにフォーカスを設定
        this.ActiveControl = this.btnSearch;
    }

    /// <summary>
    /// F8：検索ボタン押下イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void btnSearch_Click(object sender, EventArgs e)
    {

        // F8ボタン押下
        base.btnCom_Click(this.btnSearch, e);

        // log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");
    }

    /// <summary>
    /// F10：登録ボタン押下イベント
    /// </summary>
    protected override void btnF10_ClickOrgProc()
    {

        // エラーのクリア
        this.errorClear();

        // 必須項目のチェック
        this.isUpdateCheck();

        if (_yoyakuInfoTable == null)
            return;

        // 確認メッセージ（）
        if (createFactoryMsg.messageDisp("Q90_001", "登録") == MsgBoxResult.Cancel)
            return;

        foreach (DataRow dataRow in _yoyakuInfoTable.Rows)
        {

            // サーバー日付取得
            Hashtable sysdates = this.getSysDates();

            // 使用中フラグをUseに設定
            if (this.executeUsingFlgUse(sysdates, dataRow) == false)
                // メッセージ出力（"登録に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録");

            // エンティティに値を格納
            this.setEntityData(dataRow, sysdates);

            YoyakuInfoCrsChargeChargeKbnEntity yoyakuInfoCrsChargeChargeKbn = this.createYoyakuInfoCrsChargeChargeKbnEntityForHakken(dataRow("YOYAKU_KBN").ToString, System.Convert.ToInt32(dataRow("YOYAKU_NO")), sysdates);
            List<YoyakuInfoCrsChargeChargeKbnEntity> yoyakuInfoCrsChargeChargeKbnList = new List<YoyakuInfoCrsChargeChargeKbnEntity>();
            yoyakuInfoCrsChargeChargeKbnList.Add(yoyakuInfoCrsChargeChargeKbn);

            // 予約と精算の更新
            if (this.executeYoyakuSeisan(yoyakuInfoCrsChargeChargeKbnList) == false)
                // メッセージ出力（"更新に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録");

            // 使用中フラグをUnusedに設定
            if (this.executeUsingFlgUnused() == false)
                // メッセージ出力（"更新に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録");
        }

        // メッセージ出力（"登録が完了しました。")
        CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録");
    }

    /// <summary>
    /// 条件クリアボタン押下時
    /// </summary>
    protected override void btnCLEAR_ClickOrgProc()
    {

        // 初期表示と同一の処理を実行
        this.setControlInitiarize();
    }

    /// <summary>
    /// 条件クリアボタン押下時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCLEAR_Click(object sender, EventArgs e)
    {

        // CLEARボタン押下
        base.btnCom_Click(this.btnClear, e);
    }

    /// <summary>
    /// 代理店コードボタン押下イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void btnGridRow_Click(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // エラーのクリア
            this.errorClear();

            FlexGridEx grd = sender as FlexGridEx;
            if (grd.Row <= 0)
                return;
            int tableNum = grd.Row - 1;

            // 企画の場合、代理店コードボタンを押下できない。
            if (System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.higaeri) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.stay) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.rcourse))
            {

                // メッセージ出力（企画の場合は代理店コードを参照できません。）
                CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "代理店コードを参照");

                return;
            }

            // 画面間パラメータを用意
            MAgentEntity agentEntity = new MAgentEntity();

            // 代理店検索　画面展開
            using (Master.S90_0104 form = new Master.S90_0104(agentEntity))
            {
                form.ShowDialog();
                agentEntity = (MAgentEntity)form.getReturnEntity;
            }

            // エンティティに値がない時
            if (agentEntity == null)
                return;

            // グリッドに値を格納
            grd.Item(grd.Row, "AGENT_CD") = agentEntity.AgentCd.Value;
            grd.Item(grd.Row, "AGENT_NAME") = agentEntity.AgentName.Value;
            grd.Item(grd.Row, "colAgentCdChangeFlg") = true;
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// コースグリッドのチェックボックスの値が変更イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void grdCrs_AfterEdit(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // エラーのクリア
            this.errorClear();

            FlexGridEx grd = sender as FlexGridEx;
            string[] checkedAlreadyJyosyaYoyakuNoArray = null;
            string[] checkedNoShowYoyakuNoArray = null;
            string[,] inputAgentYoyakuNoArray = null;
            string[] checkedInquiryYoyakuNoArray = null;

            _releaseFlg = false;

            // チェックボックスが解除されていた場合、解除フラグをTrueに設定
            if (System.Convert.ToBoolean(grd.Item(grd.Row, "colSelection")) == false)
                _releaseFlg = true;

            // 変更があった行の予約番号を保持する
            checkedAlreadyJyosyaYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];   // 乗車済み
            checkedNoShowYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];          // NoShow
            inputAgentYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1, 3];          // 代理店コード、代理店名
            checkedInquiryYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];         // 請求対象
            if (grdYoyaku.Rows.Count - 1 > 0)
            {
                for (var ii = 1; ii <= grdYoyaku.Rows.Count - 1; ii += 1)
                {
                    // 乗車済み
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colJyosyaAlready")) == true)
                        checkedAlreadyJyosyaYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedAlreadyJyosyaYoyakuNoArray[ii] = " ";

                    // NO SHOW
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colNOSHOW")) == true)
                        checkedNoShowYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedNoShowYoyakuNoArray[ii] = " ";

                    // 代理店
                    if (!string.IsNullOrEmpty(grdYoyaku.Rows(ii).Item("AGENT_CD").ToString) == true)
                    {
                        inputAgentYoyakuNoArray[ii, 0] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                        inputAgentYoyakuNoArray[ii, 1] = grdYoyaku.Rows(ii).Item("AGENT_CD").ToString;
                        inputAgentYoyakuNoArray[ii, 2] = grdYoyaku.Rows(ii).Item("AGENT_NAME").ToString;
                    }
                    else
                    {
                        inputAgentYoyakuNoArray[ii, 0] = " ";
                        inputAgentYoyakuNoArray[ii, 1] = " ";
                        inputAgentYoyakuNoArray[ii, 2] = " ";
                    }

                    // 請求対象
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colInquiry")) == true)
                        checkedInquiryYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedInquiryYoyakuNoArray[ii] = " ";
                }
            }

            // 予約情報取得処理
            this.setYoyakuInfo();

            // 新しく表示された予約と前回チェックされていた予約が等しい場合、該当列のチェックボックスをONにする
            if (grdYoyaku.Rows.Count - 1 > 0)
            {
                for (var ii = 1; ii <= grdYoyaku.Rows.Count - 1; ii += 1)
                {
                    // 乗車済み
                    if (checkedAlreadyJyosyaYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum01 = 1; ArrayNum01 <= checkedAlreadyJyosyaYoyakuNoArray.Count() - 1; ArrayNum01 += 1)
                        {
                            // 乗車済みにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedAlreadyJyosyaYoyakuNoArray[ArrayNum01] != null && checkedAlreadyJyosyaYoyakuNoArray[ArrayNum01] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、乗車済みチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colJyosyaAlready") = true;
                                break;
                            }
                        }
                    }

                    // NOSHOW
                    if (checkedNoShowYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum02 = 1; ArrayNum02 <= checkedNoShowYoyakuNoArray.Count() - 1; ArrayNum02 += 1)
                        {
                            // NO SHOWにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedNoShowYoyakuNoArray[ArrayNum02] != null && checkedNoShowYoyakuNoArray[ArrayNum02] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、NO SHOWチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colNOSHOW") = true;
                                break;
                            }
                        }
                    }

                    // 代理店
                    if (inputAgentYoyakuNoArray.GetLength(0) > 0)
                    {
                        for (var ArrayNum03 = 1; ArrayNum03 <= inputAgentYoyakuNoArray.GetLength(0) - 1; ArrayNum03 += 1)
                        {
                            // 代理店に入力されていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (inputAgentYoyakuNoArray[ArrayNum03, 0] != null && inputAgentYoyakuNoArray[ArrayNum03, 0] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、代理店コードと代理店名に前回値を表示してループを抜ける
                                grdYoyaku.Rows(ii).Item("AGENT_CD") = inputAgentYoyakuNoArray[ArrayNum03, 1];
                                grdYoyaku.Rows(ii).Item("AGENT_NAME") = inputAgentYoyakuNoArray[ArrayNum03, 2];
                                break;
                            }
                        }
                    }

                    // 請求対象
                    if (checkedInquiryYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum04 = 1; ArrayNum04 <= checkedInquiryYoyakuNoArray.Count() - 1; ArrayNum04 += 1)
                        {
                            // 請求対象にチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedInquiryYoyakuNoArray[ArrayNum04] != null && checkedInquiryYoyakuNoArray[ArrayNum04] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、請求対象チェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colInquiry") = true;
                                break;
                            }
                        }
                    }
                }
            }

            // 予約グリッドのスクロールバー位置を左上に設定
            grdYoyaku.TopRow = 0;
            grdYoyaku.LeftCol = 0;

            // log出力
            createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// 予約グリッドのチェックボックスの値が変更イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void grdYoyaku_AfterEdit(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // エラーのクリア
            this.errorClear();

            FlexGridEx grd = sender as FlexGridEx;
            string selectedCd = grd.Item(grd.Row, grd.Col) as string;
            string selectedColumnName = grd.Cols(grd.Col).Name;

            switch (selectedColumnName)
            {
                case NameColJyosyaAlready:
                case NameColNOSHOW:
                    {
                        // 乗車済み、NOSHOWが変更された場合

                        S04_0103Da s04_0103Da = new S04_0103Da();
                        Hashtable yoyakuNo = new Hashtable();
                        DataTable usingFlgTable = new DataTable();
                        string selectedColumn = "";
                        string notSelectedColumn = "";
                        int tableNum = grd.Row - 1;

                        if (selectedColumnName.Equals(NameColJyosyaAlready))
                        {
                            selectedColumn = NameColJyosyaAlready;
                            notSelectedColumn = NameColNOSHOW;
                        }
                        else
                        {
                            selectedColumn = NameColNOSHOW;
                            notSelectedColumn = NameColJyosyaAlready;
                        }

                        // 企画の場合、「乗車済」チェックボックスは選択できない。
                        if (System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.higaeri) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.stay) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.rcourse))
                        {
                            if (selectedColumn.Equals(NameColJyosyaAlready))
                            {

                                // メッセージ出力（企画の場合は乗車済を選択できません。）
                                CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "乗車済を選択");

                                grd.Item(grd.Row, $"{selectedColumn}") = false;

                                return;
                            }
                        }

                        if (System.Convert.ToBoolean(grd.Item(grd.Row, $"{selectedColumn}")) == true)
                        {
                            // チェックボックスがONの場合

                            // 選択したカラムではないカラムのチェックボックスがONの場合
                            if (System.Convert.ToBoolean(grd.Item(grd.Row, $"{notSelectedColumn}")) == true)
                            {

                                // メッセージ出力（乗車済みとNOSHOWは同時選択不可です。）
                                CommonProcess.createFactoryMsg().messageDisp("E90_072", "乗車済みとNOSHOW");

                                grd.Item(grd.Row, $"{selectedColumn}") = false;

                                return;
                            }

                            string yoyakuKbnNo = grd.Item(grd.Row, "colYoyakuNo").ToString.Replace(",", "");

                            yoyakuNo("YoyakuKbn") = yoyakuKbnNo.Substring(0, 1);
                            yoyakuNo("YoyakuNo") = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1);

                            usingFlgTable = s04_0103Da.getUsingFlg(yoyakuNo);

                            // 予約情報が使用中の場合
                            if (usingFlgTable.Rows.Count > 0 && usingFlgTable.Rows(0)("USING_FLG").ToString.Trim.Equals(FixedCd.UsingFlg.Use))
                            {

                                // メッセージ出力（該当予約は予約で使用中です。確認後再度実行してください。）
                                CommonProcess.createFactoryMsg().messageDisp("E90_040");

                                grd.Item(grd.Row, $"{selectedColumn}") = false;

                                return;
                            }

                            // 発券済みチェック
                            if (System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.hatoBusTeiki) && !_yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateJTB) && !_yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateKNT))
                            {
                                if (string.IsNullOrEmpty(_yoyakuInfoTable.Rows(tableNum)("HAKKEN_NAIYO").ToString))
                                {

                                    // メッセージ出力（指定された予約は未だ発券されていません。）
                                    CommonProcess.createFactoryMsg().messageDisp("E02_051");

                                    grd.Item(grd.Row, $"{selectedColumn}") = false;

                                    return;
                                }
                            }

                            // サーバー日付取得
                            Hashtable sysdates = this.getSysDates();

                            if (System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.higaeri) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.stay) || System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.rcourse) && !_yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateJTB) && !_yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateKNT))
                            {

                                // TODO:場所はここで良いのか？（検索時か？）
                                // 企画日付チェック
                                string entryDate = _yoyakuInfoTable.Rows(tableNum)("ENTRY_DAY").ToString;
                                DateTime parseEntryDate = DateTime.ParseExact(entryDate, "yyyyMMdd", null);
                                if (CommonDateUtil.IsPastDate(parseEntryDate) == true)
                                {
                                    // メッセージ出力（前月１日より前の日付を指定することはできません。）
                                    CommonProcess.createFactoryMsg().messageDisp("E03_001", "前月１日");

                                    grd.Item(grd.Row, $"{selectedColumn}") = false;

                                    return;
                                }

                                // 企画旅行契約チェック
                                if (!_yoyakuInfoTable(tableNum)("KIKAKU_KEIYAKU_KBN") == DBNull.Value && System.Convert.ToInt32(_yoyakuInfoTable.Rows(tableNum)("KIKAKU_KEIYAKU_KBN")) != System.Convert.ToInt32(FixedCd.DairitenKeiyakuKbnType.Kekyaku))
                                {
                                    // メッセージ出力（企画旅行契約が結ばれていない業者です。よろしいですか？）
                                    if (CommonProcess.createFactoryMsg().messageDisp("Q02_002") == MsgBoxResult.Cancel)
                                        // キャンセルの場合、チェックOFF
                                        grd.Item(grd.Row, $"{selectedColumn}") = false;
                                    else
                                        // キャンセルの場合、チェックOFF
                                        grd.Item(grd.Row, $"{selectedColumn}") = true;
                                }
                            }
                        }

                        grd.Item(grd.Row, "colChangeFlg") = true;
                        break;
                    }

                case NameColInquiry:
                    {
                        // 請求対象が変更された場合

                        grd.Item(grd.Row, "colInquiryChangeFlg") = true;
                        break;
                    }
            }
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// コースグリッド全選択
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllSelectionCrs_Click(object sender, EventArgs e)
    {
        string[] checkedAlreadyJyosyaYoyakuNoArray = null;
        string[] checkedNoShowYoyakuNoArray = null;
        string[,] inputAgentYoyakuNoArray = null;
        string[] checkedInquiryYoyakuNoArray = null;

        try
        {
            // 前処理
            base.comPreEvent();

            // データテーブルがNothingだった場合
            if (_crsInfoTable == null)
                return;

            // エラーのクリア
            this.errorClear();

            _releaseFlg = false;

            // チェックボックスを全選択
            foreach (DataRow dataRow in _crsInfoTable.Rows)
                dataRow("colSelection") = true;

            // 変更があった行の予約番号を保持する
            checkedAlreadyJyosyaYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];   // 乗車済み
            checkedNoShowYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];          // NoShow
            inputAgentYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1, 3];          // 代理店コード、代理店名
            checkedInquiryYoyakuNoArray = new string[grdYoyaku.Rows.Count - 1 + 1];         // 請求対象
            if (grdYoyaku.Rows.Count - 1 > 0)
            {
                for (var ii = 1; ii <= grdYoyaku.Rows.Count - 1; ii += 1)
                {
                    // 乗車済み
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colJyosyaAlready")) == true)
                        checkedAlreadyJyosyaYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedAlreadyJyosyaYoyakuNoArray[ii] = " ";

                    // NO SHOW
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colNOSHOW")) == true)
                        checkedNoShowYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedNoShowYoyakuNoArray[ii] = " ";

                    // 代理店
                    if (!string.IsNullOrEmpty(grdYoyaku.Rows(ii).Item("AGENT_CD").ToString) == true)
                    {
                        inputAgentYoyakuNoArray[ii, 0] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                        inputAgentYoyakuNoArray[ii, 1] = grdYoyaku.Rows(ii).Item("AGENT_CD").ToString;
                        inputAgentYoyakuNoArray[ii, 2] = grdYoyaku.Rows(ii).Item("AGENT_NAME").ToString;
                    }
                    else
                    {
                        inputAgentYoyakuNoArray[ii, 0] = " ";
                        inputAgentYoyakuNoArray[ii, 1] = " ";
                        inputAgentYoyakuNoArray[ii, 2] = " ";
                    }

                    // 請求対象
                    if (System.Convert.ToBoolean(grdYoyaku.Rows(ii).Item("colInquiry")) == true)
                        checkedInquiryYoyakuNoArray[ii] = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString;
                    else
                        checkedInquiryYoyakuNoArray[ii] = " ";
                }
            }

            // 予約情報取得処理
            this.setYoyakuInfo();

            // 新しく表示された予約と前回チェックされていた予約が等しい場合、該当列のチェックボックスをONにする
            if (grdYoyaku.Rows.Count - 1 > 0)
            {
                for (var ii = 1; ii <= grdYoyaku.Rows.Count - 1; ii += 1)
                {
                    // 乗車済み
                    if (checkedAlreadyJyosyaYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum01 = 1; ArrayNum01 <= checkedAlreadyJyosyaYoyakuNoArray.Count() - 1; ArrayNum01 += 1)
                        {
                            // 乗車済みにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedAlreadyJyosyaYoyakuNoArray[ArrayNum01] != null && checkedAlreadyJyosyaYoyakuNoArray[ArrayNum01] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、乗車済みチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colJyosyaAlready") = true;
                                break;
                            }
                        }
                    }

                    // NOSHOW
                    if (checkedNoShowYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum02 = 1; ArrayNum02 <= checkedNoShowYoyakuNoArray.Count() - 1; ArrayNum02 += 1)
                        {
                            // NO SHOWにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedNoShowYoyakuNoArray[ArrayNum02] != null && checkedNoShowYoyakuNoArray[ArrayNum02] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、NO SHOWチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colNOSHOW") = true;
                                break;
                            }
                        }
                    }

                    // 代理店
                    if (inputAgentYoyakuNoArray.GetLength(0) > 0)
                    {
                        for (var ArrayNum03 = 1; ArrayNum03 <= inputAgentYoyakuNoArray.GetLength(0) - 1; ArrayNum03 += 1)
                        {
                            // 代理店に入力されていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (inputAgentYoyakuNoArray[ArrayNum03, 0] != null && inputAgentYoyakuNoArray[ArrayNum03, 0] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、代理店コードと代理店名に前回値を表示してループを抜ける
                                grdYoyaku.Rows(ii).Item("AGENT_CD") = inputAgentYoyakuNoArray[ArrayNum03, 1];
                                grdYoyaku.Rows(ii).Item("AGENT_NAME") = inputAgentYoyakuNoArray[ArrayNum03, 2];
                                break;
                            }
                        }
                    }

                    // 請求対象
                    if (checkedInquiryYoyakuNoArray.Count() > 0)
                    {
                        for (var ArrayNum04 = 1; ArrayNum04 <= checkedInquiryYoyakuNoArray.Count() - 1; ArrayNum04 += 1)
                        {
                            // 請求対象にチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            if (checkedInquiryYoyakuNoArray[ArrayNum04] != null && checkedInquiryYoyakuNoArray[ArrayNum04] == grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString)
                            {
                                // 等しい場合は、請求対象チェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colInquiry") = true;
                                break;
                            }
                        }
                    }
                }
            }

            // log出力
            createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// コースグリッド全解除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllReleaseCrs_Click(object sender, EventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // データテーブルがNothingだった場合
            if (_crsInfoTable == null)
                return;

            // エラーのクリア
            this.errorClear();

            // チェックボックスを全解除
            foreach (DataRow dataRow in _crsInfoTable.Rows)
                dataRow("colSelection") = false;

            _yoyakuInfoTable = new DataTable();

            grdYoyaku.DataSource = _yoyakuInfoTable;
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// 予約グリッド全選択
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllSelectionYoyaku_Click(object sender, EventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // データテーブルがNothingだった場合
            if (_yoyakuInfoTable == null)
                return;

            // エラーのクリア
            this.errorClear();

            // チェックボックスを全選択
            foreach (DataRow dataRow in _yoyakuInfoTable.Rows)
            {
                if (System.Convert.ToBoolean(dataRow("colJyosyaAlready")) == false)
                {

                    // 企画の場合、「乗車済」チェックボックスは選択できない。
                    if (System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.higaeri) || System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.stay) || System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.rcourse))
                    {

                        // 'メッセージ出力（企画の場合は乗車済を選択できません。）
                        // CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "乗車済を選択")

                        dataRow("colJyosyaAlready") = false;

                        continue;
                    }
                }

                // 選択したカラムではないカラムのチェックボックスがONの場合
                if (System.Convert.ToBoolean(dataRow("colNOSHOW")) == true)
                {

                    // 'メッセージ出力（乗車済みとNOSHOWは同時選択不可です。）
                    // CommonProcess.createFactoryMsg().messageDisp("E90_072", "乗車済みとNOSHOW")

                    dataRow("colJyosyaAlready") = false;

                    continue;
                }

                Hashtable yoyakuNo = new Hashtable();
                S04_0103Da s04_0103Da = new S04_0103Da();
                yoyakuNo("YoyakuKbn") = dataRow("YOYAKU_KBN");
                yoyakuNo("YoyakuNo") = dataRow("YOYAKU_NO");

                DataTable usingFlgTable = S04_0103Da.getUsingFlg(yoyakuNo);

                // 予約情報が使用中の場合
                if (usingFlgTable.Rows.Count > 0 && usingFlgTable.Rows(0)("USING_FLG").ToString.Trim.Equals(FixedCd.UsingFlg.Use))
                {

                    // 'メッセージ出力（該当予約は予約で使用中です。確認後再度実行してください。）
                    // CommonProcess.createFactoryMsg().messageDisp("E90_040")

                    dataRow("colJyosyaAlready") = false;

                    continue;
                }

                // 発券済みチェック
                if (System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.hatoBusTeiki) && !dataRow("STATE").ToString.Trim.Equals(StateJTB) && !dataRow("STATE").ToString.Trim.Equals(StateKNT))
                {
                    if (string.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString))
                    {

                        // 'メッセージ出力（指定された予約は未だ発券されていません。）
                        // CommonProcess.createFactoryMsg().messageDisp("E02_051")

                        dataRow("colJyosyaAlready") = false;

                        continue;
                    }
                }

                // サーバー日付取得
                Hashtable sysdates = this.getSysDates();

                if (System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.higaeri) || System.Convert.ToInt32(dataRow("CRS_KIND")) == System.Convert.ToInt32(FixedCd.CrsKindType.stay) && !dataRow("STATE").ToString.Trim.Equals(StateJTB) && !dataRow("STATE").ToString.Trim.Equals(StateKNT))
                {

                    // 企画日付チェック
                    if (System.Convert.ToInt32(dataRow("HAKKEN_DAY")) < System.Convert.ToInt32((DateTime)sysdates(KeyDtSysDate).AddMonths(BfMon).AddDays(OneDayBf).ToString("yyMMdd")))
                    {
                        // 'メッセージ出力（前月１日より前の日付を指定することはできません。）
                        // CommonProcess.createFactoryMsg().messageDisp("E03_001", "前月１日")

                        dataRow("colJyosyaAlready") = false;

                        continue;
                    }

                    // 企画旅行契約チェック
                    if (!dataRow("KIKAKU_ATO_SEISAN_KBN") == DBNull.Value && System.Convert.ToInt32(dataRow("KIKAKU_ATO_SEISAN_KBN")) != System.Convert.ToInt32(FixedCd.DairitenKeiyakuKbnType.Kekyaku))
                    {
                        // 'メッセージ出力（企画旅行契約が結ばれていない業者です。よろしいですか？）
                        // CommonProcess.createFactoryMsg().messageDisp("Q02_002")

                        dataRow("colJyosyaAlready") = false;

                        continue;
                    }
                }

                dataRow("colJyosyaAlready") = true;
                dataRow("colChangeFlg") = true;
            }
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// 予約グリッド全解除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllReleaseYoyaku_Click(object sender, EventArgs e)
    {
        try
        {
            // 前処理
            base.comPreEvent();

            // データテーブルがNothingだった場合
            if (_yoyakuInfoTable == null)
                return;

            // エラーのクリア
            this.errorClear();

            // チェックボックスを全解除
            foreach (DataRow dataRow in _yoyakuInfoTable.Rows)
            {
                if (System.Convert.ToBoolean(dataRow("colJyosyaAlready")) == true)
                {
                    dataRow("colJyosyaAlready") = false;
                    dataRow("colChangeFlg") = true;
                }
            }
        }
        catch (OracleException ex)
        {
            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString());
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            string[] strErr;
            strErr = new string[] { ex.Message, ex.Source, ex.StackTrace };
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, strErr);
            this.Close();
        }
        finally
        {

            // 後処理
            base.comPostEvent();
        }
    }

    /// <summary>
    /// グリッドのデータソース変更時イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void grdCrs_AfterDataRefresh(object sender, ListChangedEventArgs e)
    {
        // データ件数を表示(ヘッダー行分マイナス1)
        string formatedCount = (this.grdCrs.Rows.Count - 1).ToString.PadLeft(6);
        this.lblLengthGrd.Text = formatedCount + Ken;
    }


    /// <summary>
    /// 画面初期化
    /// </summary>
    private void setControlInitiarize()
    {

        // ベースフォームの設定
        this.setFormId = PgmId;
        this.setTitle = ScreenName;

        // フッタボタンの設定
        this.setButtonInitiarize();

        // 検索項目の設定
        this.setSearchKoumoku();

        // コースグリッドの初期化
        this.setGrdCrsInitiarize();

        // 予約グリッドの初期化
        this.setGrdYoyakuInitiarize();

        // エラーのクリア
        this.errorClear();

        // 変数の初期化
        this.setVariableInitial();

        // 検索条件を表示状態のGroupAreaのレイアウトを保存
        this.saveGrpLayout();

        // フォーカス設定
        this.ActiveControl = this.dtmSyuptDay;
    }

    /// <summary>
    /// フッタボタンの設定
    /// </summary>
    private void setButtonInitiarize()
    {

        // Visible
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
        this.F12Key_Visible = false;

        // Text
        this.F2Key_Text = "F2:戻る";
        this.F10Key_Text = "F10:登録";
    }

    /// <summary>
    /// コースグリッドの初期化
    /// </summary>
    private void setGrdCrsInitiarize()
    {
        // グリッド部の初期表示
        DataTable dt = new DataTable();

        this.grdCrs.DataSource = dt;
        this.grdCrs.DataMember = "";
        this.grdCrs.Refresh();

        this.lblLengthGrd.Text = ZeroKen.PadLeft(7);
    }

    /// <summary>
    /// 予約グリッドの初期化
    /// </summary>
    private void setGrdYoyakuInitiarize()
    {
        // グリッド部の初期表示
        DataTable dt = new DataTable();

        this.grdYoyaku.DataSource = dt;
        this.grdYoyaku.DataMember = "";
        this.grdYoyaku.Refresh();
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void setVariableInitial()
    {
        _searchForTable = new Hashtable();
        _yoyakuInfoTable = null;
        _crsInfoTable = null;
        _judgmentForList = new Hashtable();
        _seisanInfoEntity = new TSeisanInfoEntity();
        _yoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
        _releaseFlg = false;
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

    /// <summary>
    /// 入力項目のチェック
    /// </summary>
    private bool isInputCheck()
    {

        // 日付の入力チェック
        if (dtmSyuptDay.Text.Equals(DayInitialValue))
        {

            // メッセージ出力（日付を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付");

            // エラー項目を赤く表示
            this.dtmSyuptDay.ExistError = true;

            // フォーカス設定
            this.ActiveControl = this.dtmSyuptDay;

            return false;
        }

        DateTime checkDate = default(DateTime);

        // 日付の値チェック
        if (!DateTime.TryParse(dtmSyuptDay.Text, ref checkDate))
        {

            // メッセージ出力（日付の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "日付");

            // エラー項目を赤く表示
            this.dtmSyuptDay.ExistError = true;

            // フォーカス設定
            this.ActiveControl = this.dtmSyuptDay;

            return false;
        }

        // 乗車地の入力チェック
        if (string.IsNullOrEmpty(this.ucoNoribaCd.CodeText))
        {

            // メッセージ出力（乗車地を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地");

            // エラー項目を赤く表示
            this.ucoNoribaCd.ExistError = true;

            // フォーカス設定
            this.ActiveControl = this.ucoNoribaCd;

            return false;
        }

        // コース種別１の入力チェック
        if (this.chkCrsKind1Japanese.Checked == false && this.chkCrsKind1Gaikokugo.Checked == false)
        {

            // メッセージ出力（コース種別１を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "コース種別１");

            // エラー項目を赤く表示
            this.chkCrsKind1Japanese.ExistError = true;
            this.chkCrsKind1Gaikokugo.ExistError = true;

            // フォーカス設定
            this.ActiveControl = this.chkCrsKind1Japanese;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 入力項目のチェック(予約)
    /// </summary>
    private void isInputCheckYoyaku()
    {
        DateTime checkDate = default(DateTime);

        // 出発時刻の値が不正
        if (!this.ucoSyuptTimeFromTo.FromTimeText == null && DateTime.TryParse(this.ucoSyuptTimeFromTo.FromTimeText.ToString, ref checkDate) == false)
        {

            // メッセージ出力（出発時間の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "出発時間");

            // エラー項目を赤く表示
            this.ucoSyuptTimeFromTo.ExistErrorForFromTime = true;

            // フォーカス設定
            this.ActiveControl = this.ucoSyuptTimeFromTo;

            return;
        }

        if (!this.ucoSyuptTimeFromTo.ToTimeText == null && DateTime.TryParse(this.ucoSyuptTimeFromTo.ToTimeText.ToString, ref checkDate) == false)
        {

            // メッセージ出力（出発時間の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "出発時間");

            // エラー項目を赤く表示
            this.ucoSyuptTimeFromTo.ExistErrorForToTime = true;

            // フォーカス設定
            this.ActiveControl = this.ucoSyuptTimeFromTo;

            return;
        }
    }

    /// <summary>
    /// 更新時のチェック処理
    /// </summary>
    private void isUpdateCheck()
    {
    }

    /// <summary>
    /// 検索用の値を格納
    /// </summary>
    private void setSerchForValue()
    {
        DateTime checkDate = default(DateTime);        // チェック用
        Hashtable yoyakuNo = new Hashtable();     // 予約番号

        _searchForTable = new Hashtable();

        // 出発日
        _searchForTable("SyuptDay") = (DateTime)this.dtmSyuptDay.Text.ToString("yyyyMMdd");
        // 乗車地コード
        _searchForTable("JyochachiCd") = this.ucoNoribaCd.CodeText;
        // コース種別1
        if (chkCrsKind1Japanese.Checked == true)
            _searchForTable("Japanese") = true;
        else
            _searchForTable("Japanese") = false;
        if (chkCrsKind1Gaikokugo.Checked == true)
            _searchForTable("Gaikokugo") = true;
        else
            _searchForTable("Gaikokugo") = false;
        // コース種別2
        if (chkCrsKind2Teiki.Checked == true)
            _searchForTable("Teiki") = true;
        else
            _searchForTable("Teiki") = false;
        if (chkCrsKind2Kikaku.Checked == true)
            _searchForTable("Kikaku") = true;
        else
            _searchForTable("Kikaku") = false;
        // 出発時間
        if (DateTime.TryParse(ucoSyuptTimeFromTo.FromTimeText.ToString, ref checkDate))
            _searchForTable("SyuptTimeFrom") = (DateTime)ucoSyuptTimeFromTo.FromTimeText.ToString.ToString("HHmm");
        else
            _searchForTable("SyuptTimeFrom") = "";
        if (DateTime.TryParse(ucoSyuptTimeFromTo.ToTimeText.ToString, ref checkDate))
            _searchForTable("SyuptTimeTo") = (DateTime)ucoSyuptTimeFromTo.ToTimeText.ToString.ToString("HHmm");
        else
            _searchForTable("SyuptTimeTo") = "";
        // 予約区分と予約番号
        yoyakuNo = this.getYoyakuNo();
        if (!string.IsNullOrEmpty(yoyakuNo("yoyakuNo").ToString))
        {
            _searchForTable("YoyakuKbn") = yoyakuNo("yoyakuKbn");
            _searchForTable("YoyakuNo") = yoyakuNo("yoyakuNo");
        }
        else
        {
            _searchForTable("YoyakuKbn") = "";
            _searchForTable("YoyakuNo") = "";
        }
        // 予約者名
        _searchForTable("Surname") = this.txtYoyakuPersonSurname.Text + "%";
        _searchForTable("Name") = this.txtYoyakuPersonName.Text + "%";
        // 処理日
        if (DateTime.TryParse(dtmProcessDay.Text.ToString, ref checkDate))
            _searchForTable("ProcessDay") = (DateTime)dtmProcessDay.Text.ToString("yyyyMMdd");
        else
            _searchForTable("ProcessDay") = "";
        // ユーザーID
        _searchForTable("UserId") = this.txtUserId.Text;
        // 営業所コード
        _searchForTable("EigyosyoCd") = this.ucoEigyosyoCd.CodeText;
        // 代理店コード
        _searchForTable("DairitenCd") = this.ucoDairitenCd.CodeText;
        // コースコード
        _searchForTable("CrsCd") = this.ucoCrsCd.CodeText;
        // 乗車済み含む
        if (chkJyosyaAlreadyWith.Checked == true)
            _searchForTable("JyosyaAlready") = true;
        else
            _searchForTable("JyosyaAlready") = false;
        // 削除済み含む
        if (chkDeleteAlreadyWith.Checked == true)
            _searchForTable("DeleteAlreadyWith") = true;
        else
            _searchForTable("DeleteAlreadyWith") = false;
    }

    /// <summary>
    /// コース情報テーブルの取得
    /// </summary>
    private void getCrsInfoTable()
    {
        S04_0103Da s04_0103Da = new S04_0103Da();

        _crsInfoTable = s04_0103Da.getCrsInfoTable(_searchForTable);
    }

    /// <summary>
    /// 予約情報テーブルの取得
    /// </summary>
    private DataTable getYoyakuInfoTable()
    {
        S04_0103Da s04_0103Da = new S04_0103Da();

        return s04_0103Da.getYoyakuInfoTable(_searchForTable);
    }

    /// <summary>
    /// エンティティに値を格納
    /// </summary>
    private void setEntityData(DataRow dataRow, Hashtable sysdates)
    {

        // 変数の初期化
        _yoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
        _judgmentForList = new Hashtable();

        // エンティティに値を格納
        _yoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId;
        _yoyakuInfoBasicEntity.updatePgmid.Value = PgmId;
        _yoyakuInfoBasicEntity.updateDay.Value = (int?)sysdates(KeyIntSysDate);
        _yoyakuInfoBasicEntity.updateTime.Value = (int?)sysdates(KeyIntSysTimeHhMmSs);
        _yoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        _yoyakuInfoBasicEntity.systemUpdatePgmid.Value = PgmId;
        _yoyakuInfoBasicEntity.systemUpdateDay.Value = (DateTime)sysdates(KeyDtSysDate);
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Unused;
        _yoyakuInfoBasicEntity.yoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString;
        _yoyakuInfoBasicEntity.yoyakuNo.Value = System.Convert.ToInt32(dataRow("YOYAKU_NO"));
        _yoyakuInfoBasicEntity.crsCd.Value = dataRow("CRS_CD").ToString;
        _yoyakuInfoBasicEntity.syuptDay.Value = System.Convert.ToInt32(dataRow("SYUPT_DAY"));
        _yoyakuInfoBasicEntity.gousya.Value = System.Convert.ToInt32(dataRow("GOUSYA"));
        _yoyakuInfoBasicEntity.name.Value = dataRow("NAME").ToString;
        _yoyakuInfoBasicEntity.agentCd.Value = dataRow("AGENT_CD").ToString;
        _yoyakuInfoBasicEntity.agentNm.Value = dataRow("AGENT_NAME").ToString;
        _yoyakuInfoBasicEntity.cancelFlg.Value = dataRow("CANCEL_FLG").ToString;

        _judgmentForList("JyosyaAlreadyNOSHOW") = false;
        if (System.Convert.ToBoolean(dataRow("colJyosyaAlready")) == true)
        {
            // 乗車済みにチェックがある場合
            _yoyakuInfoBasicEntity.checkinFlg1.Value = CheckinFlgJyosyaAlready;
            _yoyakuInfoBasicEntity.noShowFlg.Value = "";
            _yoyakuInfoBasicEntity.usingFlg.Value = "";
            _judgmentForList("JyosyaAlreadyNOSHOW") = true;
        }
        else if (System.Convert.ToBoolean(dataRow("colNOSHOW")) == true)
        {
            // NOSHOWにチェックがある場合
            _yoyakuInfoBasicEntity.checkinFlg1.Value = CheckinFlgNOSHOW;
            _yoyakuInfoBasicEntity.noShowFlg.Value = NOSHOWFlg;
            _yoyakuInfoBasicEntity.usingFlg.Value = "";
            _judgmentForList("JyosyaAlreadyNOSHOW") = true;
        }

        // チェックボックスに変化がなかった場合
        if (System.Convert.ToBoolean(dataRow("colChangeFlg")) == false)
            _judgmentForList("JyosyaAlreadyNOSHOW") = false;

        _judgmentForList("Dairiten") = false;
        // 代理店に変更があった場合
        if (System.Convert.ToBoolean(dataRow("colAgentCdChangeFlg")) == true)
        {
            _yoyakuInfoBasicEntity.agentCd.Value = dataRow("AGENT_CD").ToString;
            _yoyakuInfoBasicEntity.agentNm.Value = dataRow("AGENT_NAME").ToString;
            // 業者名カナを取得
            Yoyaku.CheckAgentCdParam checkAgentCdParam = new Yoyaku.CheckAgentCdParam();

            checkAgentCdParam.agentCd = dataRow("AGENT_CD").ToString;
            checkAgentCdParam.crsCd = dataRow("CRS_CD").ToString;

            // 正常終了の場合
            if (Yoyaku.YoyakuBizCommon.checkAgentCd(checkAgentCdParam) == 0)
                _yoyakuInfoBasicEntity.agentNameKana.Value = checkAgentCdParam.agentNameKana;

            _seisanInfoEntity.UpdatePersonCd.Value = UserInfoManagement.userId;
            _seisanInfoEntity.UpdatePgmid.Value = PgmId;
            _seisanInfoEntity.UpdateDay.Value = (int?)sysdates(KeyIntSysDate);
            _seisanInfoEntity.UpdateTime.Value = (int?)sysdates(KeyIntSysTimeHhMmSs);
            _seisanInfoEntity.AgentCd.Value = dataRow("AGENT_CD").ToString;
            _seisanInfoEntity.SystemUpdatePersonCd.Value = UserInfoManagement.userId;
            _seisanInfoEntity.SystemUpdatePgmid.Value = PgmId;
            _seisanInfoEntity.SystemUpdateDay.Value = (DateTime)sysdates(KeyDtSysDate);
            _seisanInfoEntity.YoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString;
            _seisanInfoEntity.YoyakuNo.Value = System.Convert.ToInt32(dataRow("YOYAKU_NO"));

            _judgmentForList("Dairiten") = true;
        }

        _judgmentForList("Inquiry") = false;
        // 請求対象に変更があった場合
        if (System.Convert.ToBoolean(dataRow("colInquiryChangeFlg")) == true)
        {
            _seisanInfoEntity.UpdatePersonCd.Value = UserInfoManagement.userId;
            _seisanInfoEntity.UpdatePgmid.Value = PgmId;
            _seisanInfoEntity.UpdateDay.Value = (int?)sysdates(KeyIntSysDate);
            _seisanInfoEntity.UpdateTime.Value = (int?)sysdates(KeyIntSysTimeHhMmSs);
            _seisanInfoEntity.SystemUpdatePersonCd.Value = UserInfoManagement.userId;
            _seisanInfoEntity.SystemUpdatePgmid.Value = PgmId;
            _seisanInfoEntity.SystemUpdateDay.Value = (DateTime)sysdates(KeyDtSysDate);
            _seisanInfoEntity.YoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString;
            _seisanInfoEntity.YoyakuNo.Value = System.Convert.ToInt32(dataRow("YOYAKU_NO"));
            if (System.Convert.ToBoolean(dataRow("colInquiry")) == true)
                _seisanInfoEntity.AgtSeikyuTaisyoFlg.Value = FixedCd.AtoSeisanKbnType.ChakukenSeisan;
            else
                _seisanInfoEntity.AgtSeikyuTaisyoFlg.Value = FixedCd.AtoSeisanKbnType.HakkenSeisan;

            _judgmentForList("Inquiry") = true;
        }
    }

    /// <summary>
    /// 使用中フラグをUseに設定
    /// </summary>
    private bool executeUsingFlgUse(Hashtable sysdates, DataRow dataRow)
    {
        S04_0103Da s04_0103Da = new S04_0103Da();

        _yoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

        // エンティティに値を格納
        _yoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId;
        _yoyakuInfoBasicEntity.updatePgmid.Value = PgmId;
        _yoyakuInfoBasicEntity.updateDay.Value = (int?)sysdates(KeyIntSysDate);
        _yoyakuInfoBasicEntity.updateTime.Value = (int?)sysdates(KeyIntSysTimeHhMmSs);
        _yoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        _yoyakuInfoBasicEntity.systemUpdatePgmid.Value = PgmId;
        _yoyakuInfoBasicEntity.systemUpdateDay.Value = (DateTime)sysdates(KeyDtSysDate);
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Use;
        _yoyakuInfoBasicEntity.yoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString;
        _yoyakuInfoBasicEntity.yoyakuNo.Value = System.Convert.ToInt32(dataRow("YOYAKU_NO"));

        return s04_0103Da.executeUsingFlg(_yoyakuInfoBasicEntity);
    }

    /// <summary>
    /// 使用中フラグをUnusedに設定
    /// </summary>
    private bool executeUsingFlgUnused()
    {
        S04_0103Da s04_0103Da = new S04_0103Da();

        // エンティティに値を格納
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Unused;

        return s04_0103Da.executeUsingFlg(_yoyakuInfoBasicEntity);
    }

    /// <summary>
    /// 予約と精算の更新
    /// </summary>
    private bool executeYoyakuSeisan(List<YoyakuInfoCrsChargeChargeKbnEntity> yoyakuInfoCrsChargeChargeKbnList)
    {
        S04_0103Da s04_0103Da = new S04_0103Da();

        return s04_0103Da.executeYoyakuSeisan(_yoyakuInfoBasicEntity, _seisanInfoEntity, _judgmentForList, yoyakuInfoCrsChargeChargeKbnList);
    }

    // 検索項目の設定
    private void setSearchKoumoku()
    {
        // サーバー日付取得
        Hashtable sysdates = this.getSysDates();

        this.dtmSyuptDay.Text = (DateTime)sysdates(KeyDtSysDate).ToString("yy/MM/dd");
        // 日本語／外国語コース
        if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
        {
            // ユーザーが国際事業部の場合は外国語
            chkCrsKind1Gaikokugo.Checked = true;
            chkCrsKind1Japanese.Checked = false;
        }
        else
        {
            // それ以外の場合は日本語をONに設定
            chkCrsKind1Japanese.Checked = true;
            chkCrsKind1Gaikokugo.Checked = false;
        }

        // 初期化
        this.ucoNoribaCd.clear();
        this.chkCrsKind1Japanese.Checked = false;
        this.chkCrsKind2Teiki.Checked = false;
        this.chkCrsKind2Kikaku.Checked = false;
        this.ucoSyuptTimeFromTo.FromTimeText = null;
        this.ucoSyuptTimeFromTo.ToTimeText = null;
        this.ucoYoyakuNo.YoyakuText = "";
        this.ucoDairitenCd.clear();
        this.ucoCrsCd.clear();
        this.chkJyosyaAlreadyWith.Checked = false;
        this.chkDeleteAlreadyWith.Checked = false;
        this.dtmProcessDay.Clear();
        this.txtUserId.Text = "";
        this.ucoEigyosyoCd.clear();
        this.txtYoyakuPersonSurname.Text = "";
        this.txtYoyakuPersonName.Text = "";
    }

    /// <summary>
    /// エラーのクリア
    /// </summary>
    private void errorClear()
    {
        this.dtmSyuptDay.ExistError = false;
        this.ucoNoribaCd.ExistError = false;
        this.chkCrsKind1Japanese.ExistError = false;
        this.chkCrsKind1Gaikokugo.ExistError = false;
        this.ucoSyuptTimeFromTo.ExistErrorForFromTime = false;
        this.ucoSyuptTimeFromTo.ExistErrorForToTime = false;
    }



    /// <summary>
    /// コースグリッドに値を格納
    /// </summary>
    private void setGrdCrs()
    {

        // カラムの追加
        _crsInfoTable.Columns.Add("colTotalNinzu", Type.GetType("System.Int32"));
        _crsInfoTable.Columns.Add("colSelection", Type.GetType("System.Boolean"));
        _crsInfoTable.Columns.Add("colSyuptTime", Type.GetType("System.DateTime"));

        foreach (DataRow dataRow in _crsInfoTable.Rows)
        {
            dataRow("colTotalNinzu") = System.Convert.ToInt32(dataRow("ADULT_NINZU")) + System.Convert.ToInt32(dataRow("JUNIOR_NINZU")) + System.Convert.ToInt32(dataRow("CHILD_NINZU"));
            dataRow("colSelection") = false;

            if (dataRow("SYUPT_TIME").ToString.Length >= 3)
                dataRow("colSyuptTime") = dataRow("SYUPT_TIME").ToString.Insert(dataRow("SYUPT_TIME").ToString.Length - 2, ":");
        }

        int num = 0;
        for (int rowNum = 0; rowNum <= _crsInfoTable.Rows.Count - 1; rowNum++)
        {

            // 最大表示件数以上だった場合、行を削除
            if (rowNum >= MaxKensu)
            {
                _crsInfoTable.Rows(rowNum).Delete();

                num = num + 1;
            }
        }

        // 結果をコミット
        _crsInfoTable.AcceptChanges();

        this.grdCrs.DataSource = _crsInfoTable;
    }



    /// <summary>
    /// 予約情報取得処理
    /// </summary>
    private void setYoyakuInfo()
    {
        _yoyakuInfoTable = new DataTable();

        // 入力項目のチェック
        this.isInputCheckYoyaku();

        foreach (DataRow dataRow in _crsInfoTable.Rows)
        {
            // チェックボックスがONの場合
            if (System.Convert.ToBoolean(dataRow("colSelection")) == true)
            {
                _searchForTable("CrsCd") = dataRow("CRS_CD");
                _searchForTable("Gousya") = dataRow("GOUSYA");

                _yoyakuInfoTable.Merge(this.getYoyakuInfoTable(), true);
            }
        }

        // 解除フラグがFalseの場合
        if (_releaseFlg == false)
        {
            // 取得したデータが0件の場合
            if (_yoyakuInfoTable.Rows.Count <= 0)
            {
                // メッセージ出力（該当するデータがありません。）
                CommonProcess.createFactoryMsg().messageDisp("E90_019");

                return;
            }

            // DataViewを使用してDataTableの並び替えを行う
            // 並び替える
            var dv = new DataView(_yoyakuInfoTable);
            // 昇順「姓」「名」
            dv.Sort = "SURNAME, NAME";

            // 並び替え後のデータをDataTableに戻す
            _yoyakuInfoTable = dv.ToTable;

            // 表示件数オーバーの場合
            if (_yoyakuInfoTable.Rows.Count > MaxKensu)
                // メッセージ出力（検索結果が最大設定可能数を超えました。）
                CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");

            int num = 0;
            for (int rowNum = 0; rowNum <= _yoyakuInfoTable.Rows.Count - 1; rowNum++)
            {

                // 最大表示件数以上だった場合、行を削除
                if (rowNum >= MaxKensu)
                {
                    _yoyakuInfoTable.Rows(rowNum - num).Delete();

                    num = num + 1;
                }
            }
        }

        // 結果をコミット
        _yoyakuInfoTable.AcceptChanges();

        // 予約グリッドにセット
        this.setGrdYoyaku();
    }

    /// <summary>
    /// 予約グリッドの設定
    /// </summary>
    private void setGrdYoyaku()
    {

        // カラムを追加
        _yoyakuInfoTable.Columns.Add("colJyosyaAlready", Type.GetType("System.Boolean"));        // 乗車済み
        _yoyakuInfoTable.Columns.Add("colNOSHOW", Type.GetType("System.Boolean"));               // NOSHOW
        _yoyakuInfoTable.Columns.Add("colInquiry", Type.GetType("System.Boolean"));              // 請求対象
        _yoyakuInfoTable.Columns.Add("colSurnameName", Type.GetType("System.String"));           // 姓名
        _yoyakuInfoTable.Columns.Add("colState", Type.GetType("System.String"));                 // 状態
        _yoyakuInfoTable.Columns.Add("colYoyakuNo", Type.GetType("System.String"));              // 予約番号
        _yoyakuInfoTable.Columns.Add("colChangeFlg", Type.GetType("System.Boolean"));            // 変更フラグ
        _yoyakuInfoTable.Columns.Add("colAgentCdChangeFlg", Type.GetType("System.Boolean"));     // 代理店変更フラグ
        _yoyakuInfoTable.Columns.Add("colInquiryChangeFlg", Type.GetType("System.Boolean"));     // 請求対象変更フラグ
        _yoyakuInfoTable.Columns.Add("colTotalNinzu", Type.GetType("System.Int32"));             // 合計人数
        _yoyakuInfoTable.Columns.Add("colHakkenDay", Type.GetType("System.DateTime"));           // 発券日
        _yoyakuInfoTable.Columns.Add("colProcessDay", Type.GetType("System.DateTime"));          // 処理日

        // 値を代入
        foreach (DataRow dataRow in _yoyakuInfoTable.Rows)
        {
            dataRow("colJyosyaAlready") = false;
            dataRow("colNOSHOW") = false;
            dataRow("colInquiry") = false;
            dataRow("colChangeFlg") = false;
            dataRow("colAgentCdChangeFlg") = false;
            dataRow("colInquiryChangeFlg") = false;

            // 乗車済とNOSHOW
            if (dataRow("CHECKIN_FLG_1").ToString.Trim.Equals(CheckinFlgJyosyaAlready) || dataRow("CHECKIN_FLG_2").ToString.Trim.Equals(CheckinFlgJyosyaAlready) || dataRow("CHECKIN_FLG_3").ToString.Trim.Equals(CheckinFlgJyosyaAlready))
            {
                dataRow("colJyosyaAlready") = true;
                dataRow("colNOSHOW") = false;
            }
            else if (dataRow("CHECKIN_FLG_1").ToString.Trim.Equals(CheckinFlgNOSHOW) || dataRow("CHECKIN_FLG_2").ToString.Trim.Equals(CheckinFlgNOSHOW) || dataRow("CHECKIN_FLG_3").ToString.Trim.Equals(CheckinFlgNOSHOW))
            {
                dataRow("colJyosyaAlready") = false;
                dataRow("colNOSHOW") = true;
            }

            // 姓名
            dataRow("colSurnameName") = dataRow("SURNAME").ToString + Strings.Space(1) + dataRow("NAME").ToString;

            // 予約番号
            dataRow("colYoyakuNo") = dataRow("YOYAKU_KBN").ToString + FixedCd.CommonCharType.comma + System.Convert.ToInt32(dataRow("YOYAKU_NO")).ToString("#,0");

            // 状態
            if (dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.torikesi))
                dataRow("colState") = StateCancel;
            else if (dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.sakujo))
                dataRow("colState") = StateDelete;
            else if (dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.notCancel) && string.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString) && datarow("ZASEKI_RESERVE_YOYAKU_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.ZasekiSiteiFlg.sitei))
                dataRow("colState") = StateReserve;
            else if (dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.notCancel) && !string.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString) && string.IsNullOrEmpty(dataRow("STATE").ToString))
                dataRow("colState") = StateKen;
            else
                dataRow("colState") = "";

            string hakkenDay = dataRow("HAKKEN_DAY").ToString;

            // 発券日の形式変更
            if (hakkenDay.Length == 8)
                dataRow("colHakkenDay") = (DateTime)hakkenDay.Substring(0, 4) + FixedCd.CommonCharType.slash + hakkenDay.Substring(4, 2) + FixedCd.CommonCharType.slash + hakkenDay.Substring(6, 2);

            string processDay = dataRow("ENTRY_DAY").ToString;

            // 処理日の形式変更
            if (processDay.Length == 8)
                dataRow("colProcessDay") = (DateTime)processDay.Substring(0, 4) + FixedCd.CommonCharType.slash + processDay.Substring(4, 2) + FixedCd.CommonCharType.slash + processDay.Substring(6, 2);

            // 合計人数の計算
            dataRow("colTotalNinzu") = System.Convert.ToInt32(dataRow("ADULT_NINZU")) + System.Convert.ToInt32(dataRow("JUNIOR_NINZU")) + System.Convert.ToInt32(dataRow("CHILD_NINZU"));

            // TODO:営業所での入力・更新は不可、業務管理課のみ更新可能
            // 請求対象
            if (!dataRow("AGT_SEIKYU_TAISYO_FLG") == DBNull.Value)
            {
                if (System.Convert.ToInt32(dataRow("AGT_SEIKYU_TAISYO_FLG")) == FixedCd.AtoSeisanKbnType.HakkenSeisan)
                    dataRow("colInquiry") = false;
                else if (System.Convert.ToInt32(dataRow("AGT_SEIKYU_TAISYO_FLG")) == FixedCd.AtoSeisanKbnType.ChakukenSeisan)
                    dataRow("colInquiry") = true;
            }
            else
                dataRow("colInquiry") = false;
        }

        // 予約テーブルをグリッドへ設定
        this.grdYoyaku.DataSource = _yoyakuInfoTable;
    }


    /// <summary>
    /// 予約情報(コース料金_料金区分）エンティティの設定
    /// </summary>
    /// <returns></returns>
    private YoyakuInfoCrsChargeChargeKbnEntity createYoyakuInfoCrsChargeChargeKbnEntityForHakken(string yoyakuKbn, int yoyakuNo, Hashtable sysDates)
    {
        YoyakuInfoCrsChargeChargeKbnEntity ent = new YoyakuInfoCrsChargeChargeKbnEntity();

        ent.yoyakuKbn.Value = yoyakuKbn;
        ent.yoyakuNo.Value = yoyakuNo;
        ent.cancelNinzu1.Value = 0;
        ent.cancelNinzu2.Value = 0;
        ent.cancelNinzu3.Value = 0;
        ent.cancelNinzu4.Value = 0;
        ent.cancelNinzu5.Value = 0;
        ent.cancelNinzu.Value = 0;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        return ent;
    }

    /// <summary>
    /// 予約番号取得処理
    /// </summary>
    /// <returns></returns>
    private Hashtable getYoyakuNo()
    {
        Hashtable yoyakuNo = new Hashtable();

        // 予約区分+NO
        string yoyakuKbnNo = this.ucoYoyakuNo.YoyakuText;

        if (yoyakuKbnNo.Length == 10)
        {
            yoyakuNo("yoyakuKbn") = yoyakuKbnNo.Substring(0, 1);
            yoyakuNo("yoyakuNo") = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1);
        }
        else if (yoyakuKbnNo.Length < 10)
        {
            yoyakuNo("yoyakuKbn") = "0";
            yoyakuNo("yoyakuNo") = yoyakuKbnNo;
        }

        return yoyakuNo;
    }

    /// <summary>
    /// システム日付（各型）の取得
    /// </summary>
    /// <returns>各型のシステム日付を格納したHashTable</returns>
    private Hashtable getSysDates()
    {

        // サーバー日付を取得
        DateTime dtSysDate = createFactoryDA.getServerSysDate();

        // 各型へフォーマット
        string strSysDate = dtSysDate.ToString("yyyyMMdd");
        int intSysDate = 0;
        int.TryParse(strSysDate, ref intSysDate);
        string strSysTimeHhMmSs = dtSysDate.ToString("HHmmss");
        int intSysTimeHhMmSs = 0;
        int.TryParse(strSysTimeHhMmSs, ref intSysTimeHhMmSs);
        string strSysTimeHhMm = dtSysDate.ToString("HHmm");
        int intSysTimeHhMm = 0;
        int.TryParse(strSysTimeHhMm, ref intSysTimeHhMm);

        // ハッシュテーブルへ各型のサーバー日付を格納
        Hashtable sysDates = new Hashtable();
        sysDates.Add(KeyDtSysDate, dtSysDate);
        sysDates.Add(KeyStrSysDate, strSysDate);
        sysDates.Add(KeyIntSysDate, intSysDate);
        sysDates.Add(KeyStrSysTimeHhMmSs, strSysTimeHhMmSs);
        sysDates.Add(KeyIntSysTimeHhMmSs, intSysTimeHhMmSs);
        sysDates.Add(KeyStrSysTimeHhMm, strSysTimeHhMm);
        sysDates.Add(KeyIntSysTimeHhMm, intSysTimeHhMm);

        return sysDates;
    }
}
