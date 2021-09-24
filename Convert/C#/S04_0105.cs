using Hatobus.ReservationManagementSystem.Yoyaku;
using C1.Win.C1FlexGrid;

/// <summary>

/// S04_0105 売上確定入力

/// </summary>

/// <remarks>発券画面とほぼ共通</remarks>
public class S04_0105 : FormBase
{

    /// <summary>
    /// 画面ID
    /// </summary>
    private const PgmId = "S04_0105";
    /// <summary>
    /// コースコントロール情報　KEY1
    /// </summary>
    private const CrsControlInfoKey1_11 = "11";
    /// <summary>
    /// コースコントロール情報　KEY2
    /// </summary>
    private const CrsControlInfoKey2_01 = "01";
    /// <summary>
    /// 無効の貸借対応コード(貸借先がない)
    /// </summary>
    private const ErrorTaisyakuTaioCd = "000";
    /// <summary>
    /// 売上確定
    /// </summary>
    private const UriageKakutei = "売上確定";

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
    /// 列名　割引グリッド　割引名
    /// </summary>
    private const NameColGrdWaribikiWaribikiName = "WARIBIKI_NAME";
    /// <summary>
    /// 列名　割引グリッド　割引人数
    /// </summary>
    private const NameColGrdWaribikiWaribikiNinzu = "WARIBIKI_NINZU";
    /// <summary>
    /// 列名　割引グリッド　料金区分
    /// </summary>
    private const NameColGrdWaribikiChargeName = "CHARGE_NAME";
    /// <summary>
    /// 列名　割引グリッド　人員
    /// </summary>
    private const NameColGrdWaribikiJinin = "CHARGE_KBN_JININ_NAME";
    /// <summary>
    /// 列名　精算グリッド　精算項目名
    /// </summary>
    private const NameColGrdSeisanSeisanKoumokuName = "SEISAN_KOUMOKU_NAME";
    /// <summary>
    /// 列名　精算グリッド　補助券発行会社名
    /// </summary>
    private const NameColGrdSeisanIssueCompanyName = "ISSUE_COMPANY_NAME";
    /// <summary>
    /// 列名　精算グリッド　金額
    /// </summary>
    private const NameColGrdSeisanNyuukin = "KINGAKU";
    /// <summary>
    /// 列名　金額グリッド　金額
    /// </summary>
    private const NameColGrdKingaku = "colKingaku";
    /// <summary>
    /// 行番号　精算グリッド　現金
    /// </summary>
    private const NoRowGrdSeisanGenkin = 1;
    /// <summary>
    /// 行番号　精算グリッド　クレジット
    /// </summary>
    private const NoRowGrdSeisanCredit = 2;
    /// <summary>
    /// 行番号　精算グリッド　振込
    /// </summary>
    private const NoRowGrdSeisanHurikomi = 3;
    /// <summary>
    /// 行番号　金額グリッド　料金
    /// </summary>
    private const NoRowGrdKingakuCharge = 0;
    /// <summary>
    /// 行番号　金額グリッド　キャンセル料
    /// </summary>
    private const NoRowGrdKingakuCancel = 1;
    /// <summary>
    /// 行番号　金額グリッド　割引
    /// </summary>
    private const NoRowGrdKingakuWaribiki = 2;
    /// <summary>
    /// 行番号　金額グリッド　取扱手数料
    /// </summary>
    private const NoRowGrdKingakuToriatukaiFee = 3;
    /// <summary>
    /// 行番号　金額グリッド　請求
    /// </summary>
    private const NoRowGrdKingakuSeikyu = 4;
    /// <summary>
    /// 行番号　金額グリッド２　既入金額
    /// </summary>
    private const NoRowGrdKingaku2PreNyukinGaku = 0;
    /// <summary>
    /// 行番号　金額グリッド２　入金
    /// </summary>
    private const NoRowGrdKingaku2Nyukin = 1;
    // <summary>
    // 行番号　金額グリッド２　内金請求
    // </summary>
    // Private Const NoRowGrdKingaku2UchikinSeikyu = 2
    /// <summary>
    /// 行番号　金額グリッド２　残金
    /// </summary>
    private const NoRowGrdKingaku2Zankin = 2;
    /// <summary>
    /// 行番号　金額グリッド２　予約センター返金
    /// </summary>
    private const NoRowGrdKingaku2YoyakuCenterHenkin = 3;
    /// <summary>
    /// 行番号　金額グリッド２　おつり
    /// </summary>
    private const NoRowGrdKingaku2Otsuri = 4;
    /// <summary>
    /// 列名　入返金情報　予約センター振込金額
    /// </summary>
    private const NameColYoyakuCenterHurikomiKingaku = "NYUUKIN_GAKU_1";
    /// <summary>
    /// 列名　入返金情報　ｵﾝﾗｲﾝｸﾚｼﾞｯﾄ決済金額
    /// </summary>
    private const NameColOnlineCreditKingaku = "NYUUKIN_GAKU_2";

    /// <summary>
    /// 宿泊あり
    /// </summary>
    private bool _isStayAri = false;
    /// <summary>
    /// 振込入金あり（予約センター振込入金ありの場合、返金方法は予約センター返金）
    /// </summary>
    private bool _existsHurikomiNyuukin = false;
    /// <summary>
    /// 割引あり
    /// </summary>
    private bool _existsWaribiki = false;
    /// <summary>
    /// 予約番号単位の総予約人数
    /// </summary>
    private int _totalYoyakuNinzu = 0;
    /// <summary>
    /// 取扱手数料　売上
    /// </summary>
    private decimal _toriatukaiFeeUriage = 0;
    /// <summary>
    /// 取扱手数料　キャンセル
    /// </summary>
    private decimal _toriatukaiFeeCancel = 0;
    /// <summary>
    /// 割引コード、人員コード単位の情報
    /// </summary>
    private DataTable _infoByWaribikiCdJininCd = null;
    /// <summary>
    /// 割引種別単位の人数
    /// </summary>
    private Dictionary<string, int> _ninzuByWaribikiType = null;
    /// <summary>
    /// 割引種別単位の正規料金
    /// </summary>
    private Dictionary<string, int> _seikiChargeByWaribikiType = null;
    /// <summary>
    /// 割引種別単位の請求額
    /// </summary>
    private Dictionary<string, int> _seikyuByWaribikiType = null;
    /// <summary>
    /// 割引種別単位の割引額
    /// </summary>
    private Dictionary<string, int> _waribikiByWaribikiType = null;
    /// <summary>
    /// 精算項目単位の未按分金
    /// </summary>
    private Dictionary<string, int> _undistributedKinBySeisanKoumoku = null;
    /// <summary>
    /// 券番
    /// </summary>
    private string _newKenNo = "";
    /// <summary>
    /// ＳＥＱ１（発券情報）
    /// </summary>
    private string _hakkenInfoSeq1 = "";
    /// <summary>
    /// ＳＥＱ２（発券情報）
    /// </summary>
    private int _hakkenInfoSeq2 = 0;
    /// <summary>
    /// 発券情報の精算方法
    /// </summary>
    private string _SeisanHohoHakkenInfo = "";
    /// <summary>
    /// クローズ確認フラグ
    /// </summary>
    private bool _closeFormFlg = true;
    /// <summary>
    /// 使用中フラグ獲得状態（デフォルトFALSE）
    /// </summary>
    private bool _isUsingFlg = false;

    /// <summary>
    /// 予約情報（基本）テーブル
    /// </summary>
    private DataTable _yoyakuInfoBasicTable = null;
    /// <summary>
    /// 発券情報テーブル
    /// </summary>
    private DataTable _hakkenInfoTable = null;
    /// <summary>
    /// 代理店マスタテーブル
    /// </summary>
    private DataTable _agentMaster = null;
    /// <summary>
    /// 予約情報（コース料金_料金区分）テーブル
    /// </summary>
    private DataTable _yoyakuInfoCrsChargeChargeKbn = null;
    /// <summary>
    /// 割引コードマスタテーブル
    /// </summary>
    private DataTable _waribikiCdMaster = null;
    /// <summary>
    /// 精算項目マスタテーブル
    /// </summary>
    private DataTable _seisanKoumokuMaster = null;
    /// <summary>
    /// 補助券発行会社テーブル
    /// </summary>
    private DataTable _SubKenIssueCompany = null;

    /// <summary>
    /// パラメータクラス
    /// </summary>
    public S04_0105ParamData ParamData { get; set; }
    /// <summary>
    /// 当画面が更新されたかの判定
    /// </summary>
    /// <returns></returns>
    public bool IsUpdated { get; set; } = false;

    /// <summary>
    /// 定期
    /// </summary>
    /// <returns></returns>
    private bool IsTeiki { get; set; }
    /// <summary>
    /// 企画
    /// </summary>
    /// <returns></returns>
    private bool IsKikaku { get; set; }
    /// <summary>
    /// 宿泊あり
    /// </summary>
    /// <returns></returns>
    private bool IsStayAri
    {
        get
        {
            return this._isStayAri;
        }
        set
        {
            this._isStayAri = value;
            this.grdChargeKbnStay.Visible = value;
            this.grdChargeKbn.Visible = !value;
        }
    }
    /// <summary>
    /// 発券済み
    /// </summary>
    /// <returns></returns>
    private bool IsAlreadyHakken { get; set; }
    /// <summary>
    /// 料金
    /// </summary>
    /// <returns></returns>
    private int ChargeTotal { get; set; }
    /// <summary>
    /// キャンセル料
    /// </summary>
    /// <returns></returns>
    private int Cancel { get; set; }
    /// <summary>
    /// キャンセル料（予約情報（基本））
    /// </summary>
    /// <returns></returns>
    private int CancelYoyakuInfoBasic { get; set; }
    /// <summary>
    /// 割引
    /// </summary>
    /// <returns></returns>
    private int WaribikiTotal { get; set; }
    /// <summary>
    /// 取扱手数料
    /// </summary>
    private int ToriatukaiFee { get; set; }
    /// <summary>
    /// 請求
    /// </summary>
    /// <returns></returns>
    private int SeikyuTotal
    {
        get
        {
            return this.ChargeTotal + this.Cancel - this.WaribikiTotal - this.ToriatukaiFee;
        }
    }
    /// <summary>
    /// 既入金額
    /// </summary>
    /// <returns></returns>
    private int PreNyukinGaku { get; set; }
    /// <summary>
    /// 入金額
    /// </summary>
    /// <returns></returns>
    private int Nyuukin { get; set; }
    /// <summary>
    /// 残金
    /// </summary>
    /// <returns></returns>
    private int Zankin
    {
        get
        {
            return this.SeikyuTotal - this.PreNyukinGaku;
        }
    }
    /// <summary>
    /// 予約センター返金
    /// </summary>
    /// <returns></returns>
    private int YoyakuCenterHenkin { get; set; }
    /// <summary>
    /// おつり
    /// </summary>
    /// <returns></returns>
    private int Oturi
    {
        get
        {
            return this.Nyuukin - this.SeikyuTotal - this.YoyakuCenterHenkin;
        }
    }
    /// <summary>
    /// 予約センター入金額
    /// </summary>
    /// <returns></returns>
    private int YoyakuCenterNyuukin { get; set; }
    /// <summary>
    /// オンライン決済額
    /// </summary>
    /// <returns></returns>
    private int OnlineCredit { get; set; }


    /// <summary>
    /// 画面ロード時のイベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void S04_0105_Load(object sender, System.EventArgs e)
    {
        try
        {
            // 画面表示時の初期設定
            this.setControlInitiarize();

            // 画面の初期表示
            this.setScreen();

            // 集計
            this.calculateEachKingaku();

            if (this._existsHurikomiNyuukin)
            {
                // 予約センター振込がある場合、予約センター返金とする
                this.YoyakuCenterHenkin = this.Nyuukin - this.SeikyuTotal;
                this.grdKingaku2.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku) = this.Nyuukin - this.SeikyuTotal;
            }
            else
            {
                // 予約センター返金がない場合、非活性
                CommonHakken.disableGrdCell(grdKingaku2, grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin));
                this.YoyakuCenterHenkin = 0;
                this.grdKingaku2.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku) = 0;
            }

            // 金額の表示
            this.setEachKingaku();

            // 表示チェック
            if (!this.isValidDisp())
                return;

            // 使用中フラグON
            bool isSuccess = this.updateUsingFlg(true);
            if (isSuccess == false)
            {
                CommonProcess.createFactoryMsg.messageDisp("E90_040");
                return;
            }
            else
                // 使用中フラグセットの状態を更新
                _isUsingFlg = true;
        }
        catch (Exception ex)
        {
            this._closeFormFlg = false;
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            this.Close();
        }

        finally
        {
            // 後処理
            comPostEvent();
        }
    }

    /// <summary>
    /// 画面終了時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void S04_0105_FormClosing(object sender, FormClosingEventArgs e)
    {
        // 更新完了時は確認不要
        if (this.IsUpdated == true)
            return;

        // 画面終了の確認
        if (_closeFormFlg == true)
        {
            if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "入力された内容") == MsgBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
        }

        this._closeFormFlg = true;
    }

    /// <summary>
    /// 画面終了後のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void S02_0602_Closed(object sender, EventArgs e)
    {
        try
        {
            // 使用中設定したら、終了時に解除
            if (_isUsingFlg == true)
            {
                bool isSuccess = this.updateUsingFlg(false);
                if (isSuccess == false)
                    CommonProcess.createFactoryMsg().messageDisp("E90_025", "更新処理", "使用中フラグ解除");
            }
        }
        catch (Exception ex)
        {
            createFactoryMsg().messageDisp("E90_046", ex.Message);
            this.Close();
        }

        finally
        {
            // 後処理
            comPostEvent();
        }
    }

    /// <summary>
    /// F2：戻るボタン押下イベント
    /// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        base.btnF2_ClickOrgProc();

        this.Close();
    }

    /// <summary>
    /// F10:登録ボタン押下イベント
    /// </summary>
    protected override void btnF10_ClickOrgProc()
    {
        // 確認メッセージ
        MsgBoxResult msgResult = CommonProcess.createFactoryMsg.messageDisp("Q90_001", UriageKakutei);
        if (msgResult == MsgBoxResult.Cancel)
            return;

        string hakkenNaiyo = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HAKKEN_NAIYO") ?? "";
        decimal dHakkenKingaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("HAKKEN_KINGAKU") ?? 0;
        int hakkenKingaku = Convert.ToInt32(dHakkenKingaku);

        if (this.IsAlreadyHakken == true)
            // 発券済みの場合、予約情報（基本）.NO SHOWフラグのみ更新を行う
            this.IsUpdated = this.updateYoyakuInfoBasicForNoShow();
        else
        {
            // エラー初期化
            CommonRegistYoyaku.removeGridBackColorStyle(this.grdWaribikiCharge);

            // 再計算
            this.calculateEachKingaku();

            // 入力チェック
            if (!this.isValidInputHakken())
                return;

            // 割引コード、種別単位の人数、請求額を集計
            this.setInfosByWaribikiType();
            // 精算項目単位の入金額を集計
            this._undistributedKinBySeisanKoumoku = this.setUndistributedKinBySeisanKoumoku();

            // 券番を採番
            this.numberingKenNo();

            // 発券登録処理、更新件数を取得
            this.IsUpdated = this.insertHakkenGroup();
        }

        if (this.IsUpdated == false)
        {
            // 異常終了
            CommonProcess.createFactoryMsg.messageDisp("E90_025", UriageKakutei); // TODO:オラクルエラー取り出し
            // log出力
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, UriageKakutei, "登録処理");

            return;
        }

        // log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, UriageKakutei, "登録処理");

        // 正常終了
        CommonProcess.createFactoryMsg.messageDisp("I90_002", UriageKakutei);

        this.Close();
    }

    /// <summary>
    /// 追加ボタン押下イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
        FlexGridEx grd = this.grdSeisan as FlexGridEx;
        grd.Rows.Add();
    }

    /// <summary>
    /// 削除ボタン押下イベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
        FlexGridEx grd = this.grdSeisan as FlexGridEx;

        if (grd.Row <= 0)
            return;

        // 支払済みなら削除失敗
        string shiharaizumi = grd.Item(grd.Row, "SHIHARAIZUMI") as string ?? "";
        if (shiharaizumi.Equals(FixedCd.CommonKigouType.maruMark))
            return;

        // 削除
        grd.RemoveItem(grd.Row);

        // 再計算
        this.calculateEachKingaku();
        this.setEachKingaku();
    }

    /// <summary>
    /// メモ分類コンボボックスチェンジイベント
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベントデータ</param>
    private void cmbBunrui_SelectedIndexChanged(object sender, EventArgs e)
    {
        string value = CommonRegistYoyaku.convertObjectToString(this.cmbBunrui.SelectedValue);

        if (string.IsNullOrWhiteSpace(value) == true)
        {
            // 空の場合は、フィルタをクリア
            this.grdMemoList.ClearFilter();
            return;
        }

        // グリッドのフィルタリングを有効にします
        this.grdMemoList.AllowFiltering = true;
        // 新しいValueFilterを作成します
        ValueFilter filter = new ValueFilter();
        filter.ShowValues = new object[] { value };
        // 新しいフィルタを1列目に割り当てます
        this.grdMemoList.Cols("MEMO_BUNRUI").Filter = filter;
        // フィルタ条件を適用します
        this.grdMemoList.ApplyFilters();
    }

    /// <summary>
    /// 割引料金グリッド編集後イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdWaribikiCharge_TextChanged(object sender, EventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        string selectedCd = grd.Item(grd.Row, grd.Col) as string;
        string selectedColumnName = grd.Cols(grd.Col).Name;

        switch (selectedColumnName)
        {
            case NameColGrdWaribikiWaribikiName:
                {
                    // 割引項目を変更の場合

                    // 選択コードを基に割引コードマスタより値を抽出
                    DataRow dr = this._waribikiCdMaster.AsEnumerable()
                                                 .FirstOrDefault(row => row.Field<string>("WARIBIKI_CD").Equals(selectedCd));

                    if (dr == null)
                        return;

                    string tani = dr.Field<string>("WARIBIKI_KBN") ?? "";

                    // 値のグリッドへの設定
                    grd(grd.Row, "WARIBIKI_CD") = dr.Field<string>("WARIBIKI_CD");
                    if (tani.Equals(FixedCdYoyaku.Tani.Per))
                        // 「率」の場合
                        grd(grd.Row, "WARIBIKI") = dr.Field<short?>("WARIBIKI_PER") ?? 0;
                    else if (tani.Equals(FixedCdYoyaku.Tani.Yen))
                        // 「額」の場合
                        grd(grd.Row, "WARIBIKI") = dr.Field<int?>("WARIBIKI_KINGAKU") ?? 0;
                    else
                        grd(grd.Row, "WARIBIKI") = "";
                    grd(grd.Row, "WARIBIKI_TYPE_KBN") = dr.Field<string>("WARIBIKI_TYPE_KBN");
                    grd(grd.Row, "WARIBIKI_KBN") = tani;
                    grd(grd.Row, "KBN") = dr.Field<string>("KBN");
                    grd(grd.Row, "CARRIAGE_WARIBIKI_FLG") = dr.Field<string>("CARRIAGE_WARIBIKI_FLG");
                    grd(grd.Row, "YOYAKU_WARIBIKI_FLG") = dr.Field<string>("YOYAKU_WARIBIKI_FLG");
                    grd(grd.Row, "WARIBIKI_APPLICATION_NINZU") = dr.Field<short?>("WARIBIKI_APPLICATION_NINZU");
                    break;
                }

            case NameColGrdWaribikiChargeName:
                {
                    // 料金区分を変更の場合

                    // 選択コードを基にコース料金より値を抽出
                    DataRow dr = this._yoyakuInfoCrsChargeChargeKbn.AsEnumerable()
                                          .FirstOrDefault(row => row.Field<short?>("KBN_NO").ToString().Equals(selectedCd));
                    grd(grd.Row, "KBN_NO") = dr == null ? null : dr.Field<short?>("KBN_NO");
                    grd(grd.Row, "CHARGE_KBN") = dr == null ? null : dr.Field<string>("CHARGE_KBN");
                    break;
                }

            case NameColGrdWaribikiJinin:
                {
                    grd.Item(grd.Row, "CHARGE_KBN_JININ_CD") = grd.Item(grd.Row, grd.Col);

                    // 人員を変更の場合
                    if (this.IsKikaku)
                    {
                        // 企画の場合、区分No、料金区分を設定
                        DataRow dr = this._yoyakuInfoCrsChargeChargeKbn.AsEnumerable()
                                              .FirstOrDefault();
                        grd(grd.Row, "KBN_NO") = dr == null ? null : dr.Field<short?>("KBN_NO");
                        grd(grd.Row, "CHARGE_KBN") = dr == null ? null : dr.Field<string>("CHARGE_KBN");
                    }

                    break;
                }

            case NameColGrdWaribikiWaribikiNinzu:
                {
                    // 割引人数を変更の場合

                    // エラー初期化
                    CommonRegistYoyaku.removeGridBackColorStyle(this.grdWaribikiCharge);

                    // 割引コードが空
                    if (CommonHakken.isNullOrEmpty(grd(grd.Row, "WARIBIKI_CD")))
                    {
                        CommonRegistYoyaku.changeGridBackColor(grd.Row, 6, this.grdWaribikiCharge);
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引コード");
                        grd(grd.Row, "WARIBIKI_NINZU") = "";
                        return;
                    }

                    // 料金区分が空
                    if (this.IsStayAri == false)
                    {
                        if (CommonHakken.isNullOrEmpty(grd(grd.Row, "CHARGE_NAME")))
                        {
                            CommonRegistYoyaku.changeGridBackColor(grd.Row, 12, this.grdWaribikiCharge);
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "料金区分");
                            grd(grd.Row, "WARIBIKI_NINZU") = "";
                            return;
                        }
                    }

                    // 割引適用者が空
                    if (CommonHakken.isNullOrEmpty(grd(grd.Row, "CHARGE_KBN_JININ_NAME")))
                    {
                        CommonRegistYoyaku.changeGridBackColor(grd.Row, 14, this.grdWaribikiCharge);
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引適用者");
                        grd(grd.Row, "WARIBIKI_NINZU") = "";
                        return;
                    }

                    this.calculateWaribiki();
                    this.setEachKingaku();
                    return;
                }
        }

        // 割引コードの予約人員との整合性を保つために、グリッドの変更時に以降の列を初期化
        for (var cntCol = 1; cntCol <= grd.Cols.Count - 1; cntCol++)
        {
            // 選択列以下の列番号なら次の列へ（初期化しない）
            if (cntCol <= grd.Col)
                continue;

            // 割引区分と区分と割引なら次の列へ（初期化しない）
            if (grd.Cols(cntCol).Name.Equals("WARIBIKI_KBN") || grd.Cols(cntCol).Name.Equals("WARIBIKI") || grd.Cols(cntCol).Name.Equals("KBN"))
                continue;

            // 以降の列へ空文字挿入
            grd.Item(grd.Row, cntCol) = "";
        }
    }

    /// <summary>
    /// 精算グリッド押下イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdSeisan_Click(object sender, EventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;

        // ヘッダー行選択時なら削除ボタンを非活性
        if (grd.Row == 0)
        {
            this.btnDelete.Enabled = false;
            return;
        }

        // 精算コードが空, 支払済みが空, 支払済みが○以外 のいずれかなら削除ボタンを活性化
        if (grd.GetData(grd.Row, "SEISAN_KOUMOKU_CD") == null || grd.GetData(grd.Row, "SHIHARAIZUMI") == null || !grd.GetData(grd.Row, "SHIHARAIZUMI").ToString().Equals(FixedCd.CommonKigouType.maruMark))
        {
            this.btnDelete.Enabled = true;
            return;
        }

        this.btnDelete.Enabled = false;
    }

    /// <summary>
    /// 精算グリッド編集後イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdSeisan_AfterEdit(object sender, RowColEventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        string selectedCd = grd.Item(grd.Row, grd.Col) as string ?? "";
        string selectedColumnName = grd.Cols(grd.Col).Name;


        switch (selectedColumnName)
        {
            case NameColGrdSeisanSeisanKoumokuName:
                {
                    // 精算項目名ドロップダウン変更時

                    // 空セル選択時、初期化
                    if (selectedCd.Equals(CommonHakken.EmptyCellKey))
                    {
                        for (var noCol = 1; noCol <= grd.Cols.Count - 1; noCol++)
                            grd.Item(grd.Row, noCol) = "";
                        return;
                    }

                    // 選択コードを基に割引コードマスタより値を抽出
                    DataRow dr = this._seisanKoumokuMaster.AsEnumerable()
                                       .FirstOrDefault(row => row.Field<string>("SEISAN_KOUMOKU_CD").Equals(selectedCd));

                    if (dr == null)
                        return;

                    // 値のグリッドへの設定
                    grd(grd.Row, "SEISAN_KOUMOKU_CD") = dr.Field<string>("SEISAN_KOUMOKU_CD");
                    grd(grd.Row, "TAISYAKU_KBN") = dr.Field<string>("TAISYAKU_KBN");

                    // 金額の計算
                    this.calculateEachKingaku();
                    // 金額の表示
                    this.setEachKingaku();
                    break;
                }

            case NameColGrdSeisanIssueCompanyName:
                {
                    // 補助券発行会社ドロップダウン変更時

                    // 選択コードを基に割引コードマスタより値を抽出
                    DataRow dr = this._SubKenIssueCompany.AsEnumerable()
                                       .FirstOrDefault(row => row.Field<string>("ISSUE_COMPANY_CD").Equals(selectedCd));

                    if (dr == null)
                        return;

                    // 値のグリッドへの設定
                    grd(grd.Row, "ISSUE_COMPANY_CD") = dr.Field<string>("ISSUE_COMPANY_CD");
                    break;
                }

            case NameColGrdSeisanNyuukin:
                {
                    // 入金額変更時

                    // 金額の計算
                    this.calculateEachKingaku();
                    // 金額の表示
                    this.setEachKingaku();
                    break;
                }
        }
    }

    /// <summary>
    /// 金額グリッド１編集後イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdKingaku1_TextChanged(object sender, EventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        if (grd.Rows.Count <= 0)
            return;

        // キャンセル料をプロパティへ
        if (!CommonHakken.isNull(grd.Item(NoRowGrdKingakuCancel, NameColGrdKingaku)))
        {
            string strCancel = grd.Item(NoRowGrdKingakuCancel, NameColGrdKingaku).ToString();
            strCancel = CommonHakken.replaceNotNumber(strCancel);
            int.TryParse(strCancel, ref this.Cancel);
        }

        // 取扱手数料をプロパティへ
        if (!CommonHakken.isNull(grd.Item(NoRowGrdKingakuToriatukaiFee, NameColGrdKingaku)))
        {
            string strToriatukaiFee = grd.Item(NoRowGrdKingakuToriatukaiFee, NameColGrdKingaku).ToString();
            strToriatukaiFee = CommonHakken.replaceNotNumber(strToriatukaiFee);
            int.TryParse(strToriatukaiFee, ref this.ToriatukaiFee);
        }

        // 金額の計算
        this.calculateEachKingaku();
        // 金額の表示
        this.setEachKingaku();
    }

    /// <summary>
    /// 金額グリッド２編集後イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void grdKingaku2_TextChanged(object sender, EventArgs e)
    {
        FlexGridEx grd = sender as FlexGridEx;
        if (grd.Rows.Count <= 0)
            return;

        // 予約センター返金をプロパティへ
        if (!CommonHakken.isNull(grd.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku)))
        {
            string strYoyakuCenterHenkin = grd.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku).ToString();
            strYoyakuCenterHenkin = CommonHakken.replaceNotNumber(strYoyakuCenterHenkin);
            int.TryParse(strYoyakuCenterHenkin, ref this.YoyakuCenterHenkin);
        }

        // 金額の計算
        this.calculateEachKingaku();
        // 金額の表示
        this.setEachKingaku();
    }

    /// <summary>
    /// 画面表示時の初期設定
    /// </summary>
    private void setControlInitiarize()
    {
        // ベースフォームの設定
        this.setFormId = PgmId;
        this.setTitle = UriageKakutei + "入力";

        // フッタボタンの設定
        this.setButtonInitiarize();

        // パラメータ確認
        if (string.IsNullOrWhiteSpace(this.ParamData.YoyakuKbn))
            return;
        if (this.ParamData.YoyakuNo == 0)
            return;

        // 予約情報（基本）の取得、存在確認
        this._yoyakuInfoBasicTable = CommonHakken.getYoyakuInfoBasic(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        if (_yoyakuInfoBasicTable == null)
            return;

        // 発券情報の取得
        this._hakkenInfoTable = CommonHakken.getHakkenInfo(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);

        // 各区分を設定
        string teikiKikakuKbn = _yoyakuInfoBasicTable.Rows(0).Field<string>("TEIKI_KIKAKU_KBN");
        string crsKind = _yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KIND");
        this.setEachKbn(teikiKikakuKbn, crsKind);

        // 代理店の設定
        this.setAgent(crsKind);
    }

    /// <summary>
    /// 各区分を設定
    /// </summary>
    private void setEachKbn(string teikiKikakuKbn, string crsKind)
    {
        // 定期/企画区分の設定
        int intTeikiKikakuKbn = 0;
        int.TryParse(teikiKikakuKbn, ref intTeikiKikakuKbn);
        if (intTeikiKikakuKbn == FixedCd.TeikiKikakuKbn.Teiki)
        {
            this.IsTeiki = true;
            this.IsKikaku = false;
        }
        if (intTeikiKikakuKbn == FixedCd.TeikiKikakuKbn.Kikaku)
        {
            this.IsTeiki = false;
            this.IsKikaku = true;
        }

        // 宿泊有無の設定
        this.IsStayAri = CommonCheckUtil.isStay(crsKind);
    }

    /// <summary>
    /// 代理店の設定
    /// </summary>
    /// <param name="crsKind"></param>
    private void setAgent(string crsKind)
    {
        // 取扱手数料の設定
        decimal dToriatukaiFeeCancel = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("TORIATUKAI_FEE_CANCEL") ?? 0;
        decimal dToriatukaiFeeUriage = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("TORIATUKAI_FEE_URIAGE") ?? 0;
        int toriatukaiFeeCancel = Convert.ToInt32(dToriatukaiFeeCancel);
        int toriatukaiFeeUriage = Convert.ToInt32(dToriatukaiFeeUriage);

        string agentCd = this._yoyakuInfoBasicTable.Rows(0).Field<string>("AGENT_CD");
        // 代理店ありの場合
        if (!string.IsNullOrWhiteSpace(agentCd))
            this._agentMaster = CommonHakken.getAgentMaster(agentCd);

        // 精算方法の確認
        string seisanHoho = this._yoyakuInfoBasicTable.Rows(0).Field<string>("SEISAN_HOHO") ?? "";

        string tojiTuPayment = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.tojituPayment);
        string agt = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.agt);
        if (seisanHoho.Equals(tojiTuPayment) || seisanHoho.Equals(agt))
        {
            // 精算方法が当日　または　代理店の場合

            this._toriatukaiFeeCancel = toriatukaiFeeCancel;
            this._toriatukaiFeeUriage = 0;
            this.ToriatukaiFee = Convert.ToInt32(this._toriatukaiFeeCancel + this._toriatukaiFeeUriage);
        }
        else
        {
            // 精算方法が当日でない　かつ　代理店でない場合

            if (string.IsNullOrWhiteSpace(agentCd) == true)
                // 代理店コードがない場合、処理終了
                return;

            // 未契約AGENTのコミッション率初期値取得
            GetMiKeiyakuAgentComPerParam prmAgentCom = new GetMiKeiyakuAgentComPerParam();
            prmAgentCom.crsKind = crsKind;
            YoyakuBizCommon.getMiKeiyakuAgentComPer(prmAgentCom);

            CheckAgentCdParam prmCheckAgentCd = new CheckAgentCdParam();
            prmCheckAgentCd.agentCd = agentCd;
            if (this.IsTeiki)
                prmCheckAgentCd.teikiSponsorshipKbn = CommonHakken.convertEnumToString(FixedCd.TeikiKikakuKbn.Teiki);
            else
                prmCheckAgentCd.teikiSponsorshipKbn = CommonHakken.convertEnumToString(FixedCd.TeikiKikakuKbn.Kikaku);

            prmCheckAgentCd.crsCd = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_CD");
            prmCheckAgentCd.houjinGaikyakuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HOUJIN_GAIKYAKU_KBN");

            // 業者コードチェック
            int returnCd = YoyakuBizCommon.checkAgentCd(prmCheckAgentCd);

            // 異常終了
            if (returnCd != CommonHakken.NormalEnd)
            {
                CommonProcess.createFactoryMsg.messageDisp("E90_014", "代理店コード");
                return;
            }

            // コミッションの設定
            int commission = CommonHakken.setCommission(prmAgentCom, prmCheckAgentCd);
            // 取扱手数料の設定
            this.setToriatukaiFee(commission);
        }
    }

    /// <summary>
    /// 取扱手数料の設定
    /// </summary>
    private void setToriatukaiFee(int commission)
    {
        // 正規料金総額
        decimal seikiChargeAllGaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("SEIKI_CHARGE_ALL_GAKU") ?? 0;
        // 割引総額
        decimal waribikiAllGaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("WARIBIKI_ALL_GAKU") ?? 0;
        CalcToriatukaiFeeParam prmCalcToriatukaiFee = new CalcToriatukaiFeeParam();
        prmCalcToriatukaiFee.com = commission;

        // 取扱手数料/売上の設定
        prmCalcToriatukaiFee.charge = seikiChargeAllGaku - waribikiAllGaku;
        int returnCd = YoyakuBizCommon.calcToriatukaiFee(prmCalcToriatukaiFee);
        if (returnCd == CommonHakken.NormalEnd)
        {
            // 正常終了
            this.ToriatukaiFee += Convert.ToInt32(prmCalcToriatukaiFee.comgaku);
            this._toriatukaiFeeUriage = prmCalcToriatukaiFee.comgaku;
        }

        // キャンセル料計
        decimal cancelRyoukei = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("CANCEL_RYOU_KEI") ?? 0;
        // 取扱手数料/ｷｬﾝｾﾙの設定
        prmCalcToriatukaiFee.charge = cancelRyoukei;
        returnCd = YoyakuBizCommon.calcToriatukaiFee(prmCalcToriatukaiFee);
        if (returnCd == CommonHakken.NormalEnd)
        {
            // 正常終了
            this.ToriatukaiFee += Convert.ToInt32(prmCalcToriatukaiFee.comgaku);
            this._toriatukaiFeeCancel = Convert.ToInt32(prmCalcToriatukaiFee.comgaku);
        }
    }

    /// <summary>
    /// フッタボタンの設定
    /// </summary>
    private void setButtonInitiarize()
    {

        // Visible
        this.F2Key_Visible = true;
        this.F10Key_Visible = true;

        this.F1Key_Visible = false;
        this.F3Key_Visible = false;
        this.F4Key_Visible = false;
        this.F5Key_Visible = false;
        this.F6Key_Visible = false;
        this.F7Key_Visible = false;
        this.F8Key_Visible = false;
        this.F9Key_Visible = false;
        this.F11Key_Visible = false;
        this.F12Key_Visible = false;

        // Text
        this.F2Key_Text = "F2:戻る";
        this.F10Key_Text = "F10:登録";
    }

    /// <summary>
    /// 画面の値設定
    /// </summary>
    private void setScreen()
    {
        // 予約情報（基本）の設定
        this.setYoyakuInfoBasic();

        // 発券情報の設定
        this.setHakkenInfo();

        // 予約情報（コース料金_料金区分）の設定
        this.setYoyakuInfoCrsChargeChargeKbn();

        // メモ情報の設定
        this.setMemoInfo();

        // 割引料金グリッドの設定
        this.setGrdWaribikiCharge();

        // 精算グリッドの設定
        this.setGrdSeisan();
    }

    /// <summary>
    /// 予約情報（基本）の設定
    /// </summary>
    private void setYoyakuInfoBasic()
    {
        // 値の取り出し
        DataTable dt = this._yoyakuInfoBasicTable;
        string yoyakuKbn = dt.Rows(0).Field<string>("YOYAKU_KBN") ?? "";
        string yoyakuNo = dt.Rows(0).Field<int?>("YOYAKU_NO") ?? 0.ToString();
        string yoyakuKbnNo = yoyakuKbn + yoyakuNo;
        int intSyuptDay = dt.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
        int intSyuptTime = dt.Rows(0).Field<short?>("SYUPT_TIME") ?? 0;
        string crsCd = dt.Rows(0).Field<string>("CRS_CD") ?? "";
        string crsNm = dt.Rows(0).Field<string>("CRS_NAME") ?? "";
        string jyoshaTi = dt.Rows(0).Field<string>("JYOSHATI") ?? "";
        string gousya = dt.Rows(0).Field<short?>("GOUSYA") ?? 0.ToString();
        string yoyakuName = dt.Rows(0).Field<string>("NAME") ?? "";
        string telNo = dt.Rows(0).Field<string>("TEL_NO_1") ?? "";
        string telNo2 = dt.Rows(0).Field<string>("TEL_NO_2") ?? "";
        string agentCd = dt.Rows(0).Field<string>("AGENT_CD") ?? "";
        string agentName = dt.Rows(0).Field<string>("AGENT_NM") ?? "";
        string noShowFlg = dt.Rows(0).Field<string>("NO_SHOW_FLG") ?? "";

        // 料金をプロパティへ設定
        decimal dSeikiChargeAllGaku = dt.Rows(0).Field<decimal?>("SEIKI_CHARGE_ALL_GAKU") ?? 0;
        decimal dAddChargeMaeBaraiKei = dt.Rows(0).Field<decimal?>("ADD_CHARGE_MAEBARAI_KEI") ?? 0;
        int charge = Convert.ToInt32(dSeikiChargeAllGaku)
                              + Convert.ToInt32(dAddChargeMaeBaraiKei);
        this.ChargeTotal = charge;

        // キャンセル料をプロパティへ設定
        this.Cancel = Convert.ToInt32(dt.Rows(0).Field<decimal?>("CANCEL_RYOU_KEI") ?? 0);
        this.CancelYoyakuInfoBasic = Convert.ToInt32(dt.Rows(0).Field<decimal?>("CANCEL_RYOU_KEI") ?? 0);

        // 入金額総計を設定
        this.PreNyukinGaku = Convert.ToInt32(dt.Rows(0).Field<decimal?>("NYUKINGAKU_SOKEI") ?? 0);

        // 値の設定
        this.ucoYoyakuNo.YoyakuText = yoyakuKbnNo;
        this.ucoSyuptDay.Value = CommonHakken.convertIntToDate(intSyuptDay);
        this.txtCourseCd.Text = crsCd;
        this.txtCourseNm.Text = crsNm;
        this.txtJyosyaTi.Text = jyoshaTi;
        this.ucoTime.Value = CommonHakken.convertIntToTime(intSyuptTime);
        this.txtGousya.Text = gousya;
        this.txtYoyakuPersonName.Text = yoyakuName;
        this.txtTel.Text = telNo;
        this.txtTel2.Text = telNo2;
        this.txtDairitencd.Text = agentCd;
        this.txtDairitenNm.Text = agentName;

        if (!string.IsNullOrWhiteSpace(noShowFlg) == true)
            // NoShowフラグ = 'Y'の場合、初期表示時チェックON
            this.chkNoShow.Checked = true;
    }

    /// <summary>
    /// 発券情報の設定
    /// </summary>
    private void setHakkenInfo()
    {
        if (this._hakkenInfoTable == null)
        {
            this.lblUriageKakuteiZumi.Text = "";
            return;
        }

        // 未VOIDの発券情報の存在確認
        bool isHakkend = this._hakkenInfoTable.AsEnumerable()
                                  .Any(row => string.IsNullOrWhiteSpace(row.Field<string>("VOID_KBN")));
        if (isHakkend)
        {
            this.lblUriageKakuteiZumi.Text = UriageKakutei + "済";
            this.lblUriageKakuteiZumi.ForeColor = Color.Red;
            string kenNo = CommonHakken.createKenNoFromHakkenInfoRow(this._hakkenInfoTable.Rows(0));
            this.txtKenNo.Text = kenNo;
            this.txtHakkenTime.Text = this._hakkenInfoTable.Rows(0).Field<DateTime>("SYSTEM_ENTRY_DAY").ToString();
        }
        else
            this.lblUriageKakuteiZumi.Text = "";
    }

    /// <summary>
    /// 予約情報（コース料金_料金区分）の設定
    /// </summary>
    private void setYoyakuInfoCrsChargeChargeKbn()
    {
        // グリッドの初期化
        CommonHakken.setInitiarizeGrid(this.grdChargeKbnStay);
        CommonHakken.setInitiarizeGrid(this.grdChargeKbn);

        if (!this.IsStayAri)
            // 宿泊なし
            this.setGrdChargeKbn();
        else
            // 宿泊あり
            this.setGrdChargeKbnStayAri();
    }

    /// <summary>
    /// 料金区分(宿泊なし)グリッドの設定
    /// </summary>
    private void setGrdChargeKbn()
    {
        // グリッドの設定
        this.grdChargeKbn.AllowDragging = AllowDraggingEnum.None;
        this.grdChargeKbn.AllowAddNew = false;
        this.grdChargeKbn.AutoGenerateColumns = false;
        this.grdChargeKbn.ShowButtons = ShowButtonsEnum.Always;

        // 予約情報（コース料金_料金区分）の取得
        DataTable yoyakuInfoCrsChargeChargeKbn = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        if (yoyakuInfoCrsChargeChargeKbn == null)
            return;

        // フィールドへ格納
        this._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn;

        // 予約総人数を集計
        this.calculateCharge();

        // グリッドへ設定
        DataTable formatedDt = CommonHakken.formatGrdChargeKbn(yoyakuInfoCrsChargeChargeKbn);
        this.grdChargeKbn.DataSource = formatedDt;
    }

    /// <summary>
    /// コース料金（宿泊なし）の集計（非活性なので初期表示時のみ）
    /// </summary>
    private void calculateCharge()
    {
        // 宿泊なし
        foreach (DataRow row in this._yoyakuInfoCrsChargeChargeKbn.AsEnumerable())
        {
            decimal dNinzu = row.Field<decimal?>("CHARGE_APPLICATION_NINZU_1") ?? 0;
            int ninzu = Convert.ToInt32(dNinzu);

            // null,0 なら次の行へ
            if (ninzu == 0)
                continue;

            decimal dTanka = row.Field<decimal?>("TANKA_1") ?? 0;
            int tanka = Convert.ToInt32(dTanka);

            // 予約総人数へ加算
            this._totalYoyakuNinzu += ninzu;
        }
    }

    /// <summary>
    /// 料金区分(宿泊あり)グリッドの設定
    /// </summary>
    private void setGrdChargeKbnStayAri()
    {
        // グリッドの設定
        this.grdChargeKbnStay.AllowDragging = AllowDraggingEnum.None;
        this.grdChargeKbnStay.AllowAddNew = false;
        this.grdChargeKbnStay.AllowMerging = AllowMergingEnum.Custom;
        this.grdChargeKbnStay.AutoGenerateColumns = false;
        this.grdChargeKbnStay.ShowButtons = ShowButtonsEnum.Always;

        // 予約情報（コース料金_料金区分）の取得
        DataTable yoyakuInfoCrsChargeChargeKbn = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        if (yoyakuInfoCrsChargeChargeKbn == null)
            return;

        // フィールドへ格納（※割引時に正規料金を確認するため。
        this._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn;

        // 部屋タイプごとのデータを生成
        DataTable dt = CommonHakken.formatGrdChargeKbnStayAri(yoyakuInfoCrsChargeChargeKbn);

        // 予約総人数を集計
        this._totalYoyakuNinzu = dt.AsEnumerable().Sum(dr => dr.Field<int>("CHARGE_APPLICATION_NINZU"));

        // 料金区分(宿泊あり)テーブルをグリッドへ設定
        this.grdChargeKbnStay.DataSource = dt.AsDataView();
    }

    /// <summary>
    /// メモ一覧設定
    /// </summary>
    private void setMemoInfo()
    {
        // メモ分類
        S02_0103Da s02_0103Da = new S02_0103Da();
        CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbBunrui, FixedCdYoyaku.CodeBunruiTypeMemoBunrui);

        DataTable yoyakuMemoTable = CommonRegistYoyaku.getYoyakuMemoList(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        this.grdMemoList.DataSource = yoyakuMemoTable;
    }

    /// <summary>
    /// 割引料金グリッドの設定
    /// </summary>
    private void setGrdWaribikiCharge()
    {
        // グリッドの設定
        this.grdWaribikiCharge.AllowDragging = AllowDraggingEnum.None;
        this.grdWaribikiCharge.AllowAddNew = true;
        this.grdWaribikiCharge.AutoGenerateColumns = false;
        this.grdWaribikiCharge.ShowButtons = ShowButtonsEnum.Always;

        // 入力MAX桁数設定
        CommonHakken.setGridLength(this.grdWaribikiCharge, "WARIBIKI", CommonHakken.KingakuMaxLength);
        CommonHakken.setGridLength(this.grdWaribikiCharge, "WARIBIKI_NINZU", CommonHakken.NinzuMaxLength);

        if (this.IsStayAri)
        {
            this.grdWaribikiCharge.Width -= this.grdWaribikiCharge.Cols("CHARGE_NAME").Width;
            this.grdWaribikiCharge.Cols("CHARGE_NAME").Visible = false;
        }

        // 割引コードの設定
        this.setWaribikiCd();

        // 料金区分、人員の設定
        this.setWaribikiChargeKbn();

        // 料金グリッドの行数になるまで、行を追加
        if (!this.IsStayAri)
        {
            // 宿泊なし
            while (this.grdWaribikiCharge.Rows.Count < this.grdChargeKbn.Rows.Count)
                this.grdWaribikiCharge.Rows.Add();
        }
        else
            // 宿泊あり
            while (this.grdWaribikiCharge.Rows.Count < this.grdChargeKbnStay.Rows.Count)
                this.grdWaribikiCharge.Rows.Add();

        string crsKind = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KIND");
        string houjinGaikyakuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HOUJIN_GAIKYAKU_KBN") ?? "";

        // 予約情報（割引）の取得
        DataTable yoyakuInfoWaribikiTable = CommonHakken.getYoyakuInfoWaribiki(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo, crsKind, houjinGaikyakuKbn);
        if (yoyakuInfoWaribikiTable == null)
            return;

        DataView dv = this.formatGrdWaribikiCharge(yoyakuInfoWaribikiTable);

        this.grdWaribikiCharge.DataSource = dv;
        this.grdWaribikiCharge.Rows.Add();
    }

    /// <summary>
    /// 割引コードの設定
    /// </summary>
    private void setWaribikiCd()
    {
        int syuptDay = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
        string crsKind = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KIND") ?? "";
        string houjinGaikyakuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HOUJIN_GAIKYAKU_KBN") ?? "";

        // 割引コードマスタの取得
        this._waribikiCdMaster = CommonHakken.getWaribikiCdMaster(syuptDay, crsKind, houjinGaikyakuKbn);

        if (this._waribikiCdMaster != null)
        {
            // 割引マスタ
            this.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataType = typeof(string);
            this.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataMap = CommonHakken.createCmbWaribikiCd(this._waribikiCdMaster);
        }
        else
            this.grdWaribikiCharge.Enabled = false;
    }

    /// <summary>
    /// 割引料金区分の設定
    /// </summary>
    private void setWaribikiChargeKbn()
    {
        if (IsNothing(_yoyakuInfoCrsChargeChargeKbn) == false)
        {
            // 料金区分
            this.grdWaribikiCharge.Cols("CHARGE_NAME").DataType = typeof(string);
            this.grdWaribikiCharge.Cols("CHARGE_NAME").DataMap = CommonHakken.createCmbChargeKbn(this._yoyakuInfoCrsChargeChargeKbn);

            // 人員コード
            this.grdWaribikiCharge.Cols("CHARGE_KBN_JININ_NAME").DataType = typeof(string);
            this.grdWaribikiCharge.Cols("CHARGE_KBN_JININ_NAME").DataMap = CommonHakken.createCmbChargeKbnJininCd(this._yoyakuInfoCrsChargeChargeKbn);
        }
    }

    /// <summary>
    /// 割引コード単位の割引のデータを生成（表示用）
    /// </summary>
    /// <returns>割引コード単位の割引グリッド表示用データ</returns>
    /// <remarks>
    /// 予約情報（割引）を部屋タイプ１～５を集計してデータ生成
    /// </remarks>
    private DataView formatGrdWaribikiCharge(DataTable yoyakuInfoWaribikiTable)
    {
        // 各行の値を取り出し
        // 予約情報（割引）を部屋タイプ１～５で集計してデータ生成
        foreach (DataRow row in yoyakuInfoWaribikiTable.AsEnumerable())
        {
            string tani = row.Field<string>("WARIBIKI_KBN") ?? "";
            string carriageWaribikiFlg = row.Field<string>("CARRIAGE_WARIBIKI_FLG") ?? "";
            decimal waribikiPer = row.Field<decimal?>("WARIBIKI_PER") ?? 0;
            decimal tanka = row.Field<decimal?>($"WARIBIKI_KINGAKU") ?? 0;

            // 割引 率/額によって計算を変更
            row.Item("WARIBIKI") = tani.Equals(FixedCdYoyaku.Tani.Per) ? waribikiPer : tanka;

            int sumWaribikiGakuByRow = 0;
            decimal sumNinzuByRow = 0;

            // 割適用人数１～５を集計
            for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
            {
                decimal ninzu = row.Field<decimal?>($"WARIBIKI_APPLICATION_NINZU_{roomType}") ?? 0;
                decimal waribikiTanka = row.Field<decimal?>($"WARIBIKI_TANKA_{roomType}") ?? 0;

                // null, 0なら次の人数
                if (ninzu == 0)
                    continue;

                // 行ごとの人数へ加算
                sumNinzuByRow += ninzu;

                // 行ごとの割引額へ加算
                sumWaribikiGakuByRow += Convert.ToInt32(waribikiTanka * ninzu); // 割引単価 * 割引人数
            }
            // 部屋タイプ１～５までのループ（1行単位）終了

            row.Item("WARIBIKI_NINZU") = sumNinzuByRow.ToString();
            row.Item("WARIBIKI_KINGAKU") = sumWaribikiGakuByRow.ToString();

            // 料金区分Null対応
            if (IsDBNull(row.Item("CHARGE_NAME")))
            {
                string kbnNo = row.Field<short?>("KBN_NO") ?? 0.ToString();
                row.Item("CHARGE_NAME") = kbnNo.ToString();
            }
        }
        // 予約情報（割引）データ１行ずつのループ終了

        return yoyakuInfoWaribikiTable.AsDataView();
    }

    // ' <summary>
    // ' 精算グリッドの設定
    // ' </summary>
    private void setGrdSeisan()
    {
        this.grdSeisan.AllowAddNew = false;
        this.grdSeisan.AutoGenerateColumns = false;
        this.grdSeisan.ShowButtons = ShowButtonsEnum.Always;

        // 入力MAX桁数設定
        CommonHakken.setGridLength(this.grdSeisan, "KINGAKU", CommonHakken.KingakuMaxLength);

        // 精算項目マスタの設定
        this.setSeisanKoumoku();
        // 補助券発行会社の設定
        this.setSubKenIssueCompany();
        // 内訳の設定
        this.setHurikomiKbn();

        // 固定行の用意（現金、クレジット）
        this.setFixRowToGrdSeisan(NoRowGrdSeisanGenkin, FixedCd.SeisanItemCd.genkin);
        this.setFixRowToGrdSeisan(NoRowGrdSeisanCredit, FixedCd.SeisanItemCd.credit_card);

        // 入返金情報の設定
        this.setNyuukinInfo();
    }

    /// <summary>
    /// 精算項目の設定
    /// </summary>
    private void setSeisanKoumoku()
    {
        // 非表示の精算項目リスト
        string[] hidedSeisanKomokuCds = CommonHakken.createHidedSeisanKomokuCds();
        // 精算項目マスタの取得
        this._seisanKoumokuMaster = CommonHakken.getSeisanKoumokuMaster(hidedSeisanKomokuCds);
        if (!CommonHakken.existsDatas(this._seisanKoumokuMaster))
            return;

        this.grdSeisan.Cols("SEISAN_KOUMOKU_NAME").DataType = typeof(string);
        this.grdSeisan.Cols("SEISAN_KOUMOKU_NAME").DataMap = CommonHakken.createCmbSeisanKoumoku(this._seisanKoumokuMaster);
    }

    /// <summary>
    /// 補助券発行会社の設定
    /// </summary>
    private void setSubKenIssueCompany()
    {
        // 補助券発行会社（マスタ）の取得
        this._SubKenIssueCompany = CommonHakken.getSubKenIssueCompany();
        if (!CommonHakken.existsDatas(this._SubKenIssueCompany))
            return;

        this.grdSeisan.Cols("ISSUE_COMPANY_NAME").DataType = typeof(string);
        this.grdSeisan.Cols("ISSUE_COMPANY_NAME").DataMap = CommonHakken.createCmbSubKenIssueCompany(this._SubKenIssueCompany);
    }

    /// <summary>
    /// 振込区分の設定
    /// </summary>
    private void setHurikomiKbn()
    {
        this.grdSeisan.Cols("HURIKOMI_KBN").DataType = typeof(string);
        this.grdSeisan.Cols("HURIKOMI_KBN").DataMap = CommonHakken.createCmbHurikomiKbn();
    }

    /// <summary>
    /// 入返金情報の設定
    /// </summary>
    private void setNyuukinInfo()
    {
        // 入返金情報の取得
        DataTable nyukinTable = this.getNyuukin(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        if (nyukinTable == null)
            return;
        bool IsOnline = false; // オンライン決済クレジットフラグ（True：オンライン決済の場合、False：オンライン決済以外）
        // 全行読出し
        for (var noTblRow = 0; noTblRow <= nyukinTable.Rows.Count - 1; noTblRow++)
        {
            // 発券振込区分に応じて処理を変更
            string hakkenHurikomiKbn = nyukinTable.Rows(noTblRow).Field<string>("HAKKEN_HURIKOMI_KBN");
            if (string.IsNullOrWhiteSpace(hakkenHurikomiKbn))
                continue;// nullなら次の行へ
            if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterNyuukin) || hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin))
            {
                // ■予約センター振込の場合

                // 振込予約センター入金の固定行を設定
                this.setFixRowToGrdSeisan(NoRowGrdSeisanHurikomi, FixedCd.SeisanItemCd.hurikomi_yoyaku_center);

                // 振込入金ありにする
                this._existsHurikomiNyuukin = true;

                // nullなら次の行へ
                if (nyukinTable.Rows(noTblRow).Field<int?>(NameColYoyakuCenterHurikomiKingaku) == null)
                    continue;

                // 合計へ追加
                int intKingaku = nyukinTable.Rows(noTblRow).Field<int?>(NameColYoyakuCenterHurikomiKingaku) ?? 0;
                if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterNyuukin))
                    this.YoyakuCenterNyuukin += intKingaku; // 入金
                else if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin))
                    this.YoyakuCenterNyuukin -= intKingaku;// 返金
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "KINGAKU") = this.YoyakuCenterNyuukin.ToString();

                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "SHIHARAIZUMI") = FixedCd.CommonKigouType.maruMark;
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "HURIKOMI_KBN") = nyukinTable.Rows(noTblRow).Item("HURIKOMI_KBN");
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "BIKO") = nyukinTable.Rows(noTblRow).Item("BIKO");
            }
            else if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditNyuukin)
                        || hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditHenkin))
            {
                // ■オンライン決済クレジットの場合

                // 振込予約ｾﾝﾀｰ入金の項目をｵﾝﾗｲﾝ決済に変更
                this.setFixRowToGrdSeisan(NoRowGrdSeisanHurikomi, FixedCd.SeisanItemCd.online_credit);

                // nullなら次の行へ
                if (nyukinTable.Rows(noTblRow).Field<int?>(NameColOnlineCreditKingaku) == null)
                    continue;

                // 合計へ追加
                int intKingaku = nyukinTable.Rows(noTblRow).Field<int?>(NameColOnlineCreditKingaku) ?? 0;
                if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditNyuukin))
                    this.OnlineCredit += intKingaku; // 入金
                else if (hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditHenkin))
                    this.OnlineCredit -= intKingaku;// 返金
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "KINGAKU") = this.OnlineCredit.ToString();

                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "SHIHARAIZUMI") = FixedCd.CommonKigouType.maruMark;
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "HURIKOMI_KBN") = nyukinTable.Rows(noTblRow).Item("HURIKOMI_KBN");
                this.grdSeisan.Item(NoRowGrdSeisanHurikomi, "BIKO") = nyukinTable.Rows(noTblRow).Item("BIKO");

                IsOnline = true;    // オンライン決済クレジットフラグ（オンライン決済：True）
            }
        } // 入金情報テーブル読出しのループ

        // 支払済みの行を非活性化
        foreach (Row grdRow in this.grdSeisan.Rows)
        {
            // "○"でなければ次の行へ
            if (grdRow.Item("SHIHARAIZUMI") as string == null || grdRow.Item("SHIHARAIZUMI").ToString() != FixedCd.CommonKigouType.maruMark)
                continue;

            // "○"の行を非活性
            CommonHakken.disableGrdCell(this.grdSeisan, grdRow);
        } // 精算グリッドのループ
        // オンライン決済時、デフォルト表示される「現金」「クレジット」の行は表示しない。
        if (IsOnline == true)
        {
            this.grdSeisan.RemoveItem(NoRowGrdSeisanCredit);
            this.grdSeisan.RemoveItem(NoRowGrdSeisanGenkin);
        }
    }

    /// <summary>
    /// 精算グリッドへの固定値の行の設定
    /// </summary>
    private void setFixRowToGrdSeisan(int noRowGrdSeisan, object enumSeisanItemCd)
    {
        // 精算項目コードの設定
        string seisanKoumokuCd = CommonHakken.convertEnumToString(enumSeisanItemCd);
        this.grdSeisan.Item(noRowGrdSeisan, "SEISAN_KOUMOKU_CD") = seisanKoumokuCd;

        // コードをキーに精算項目マスタから抽出
        DataRow dr = this._seisanKoumokuMaster.AsEnumerable()
                            .FirstOrDefault(x => x.Field<string>("SEISAN_KOUMOKU_CD").Equals(seisanKoumokuCd));
        this.grdSeisan.Item(noRowGrdSeisan, "SEISAN_KOUMOKU_NAME") = dr.Item("SEISAN_KOUMOKU_NAME").ToString();
    }

    /// <summary>
    /// 各金額の計算（各グリッドを集計）
    /// </summary>
    private void calculateEachKingaku()
    {
        // 割引の集計
        this.calculateWaribiki();

        // 入金額の集計
        this.calculateNyuukin();
    }

    /// <summary>
    /// 割引額の集計
    /// </summary>
    private void calculateWaribiki()
    {
        this.WaribikiTotal = 0;

        // 正規料金の人数を取得
        if (IsNothing(_yoyakuInfoCrsChargeChargeKbn) == false)
        {
            // キー：料金カラムセット、値：部屋タイプ１～５の予約人数を格納した配列
            Dictionary<string, int[]> yoyakuNinzusByChargeKbn = CommonHakken.setYoyakuNinzuByJininCd(this._yoyakuInfoCrsChargeChargeKbn);

            // 全行の割引人数（部屋タイプごと）を初期化
            this.initiarizeGrdWaribikiNinzu();

            // 割引人数（部屋タイプごと）を割り振り開始
            foreach (Row row in this.grdWaribikiCharge.Rows)
            {
                if (row.Index == 0)
                    continue;
                string waribikiCd = row.Item("WARIBIKI_CD") as string ?? "";
                if (string.IsNullOrWhiteSpace(waribikiCd))
                    continue;

                // 人数
                string strNinzu = "";
                if (!CommonHakken.isNull(row.Item("WARIBIKI_NINZU")))
                    strNinzu = row.Item("WARIBIKI_NINZU").ToString();
                int ninzu = 0;
                int.TryParse(strNinzu, ref ninzu);

                if (ninzu == 0)
                    continue;

                // 単位
                string tani = row.Item("WARIBIKI_KBN") as string ?? "";
                // 割引
                string strWaribiki = "";
                if (!CommonHakken.isNull(row.Item("WARIBIKI")))
                    strWaribiki = row.Item("WARIBIKI").ToString();
                decimal waribiki = 0;
                decimal.TryParse(strWaribiki, ref waribiki);

                // 区分No
                string KbnNo = "";
                if (!CommonHakken.isNull(row.Item("KBN_NO")))
                    KbnNo = row.Item("KBN_NO").ToString();

                // 料金区分
                string chargeKbn = row.Item("CHARGE_KBN") as string ?? "";
                // 人員コード
                string jininCd = row.Item("CHARGE_KBN_JININ_CD") as string ?? "";
                // 料金カラムセットの作成
                string chargeColumns = CommonHakken.createChargeColumnsSet(KbnNo, chargeKbn, jininCd);

                // 運賃割引フラグ
                string carriageWaribikiFlg = row.Item("CARRIAGE_WARIBIKI_FLG") as string ?? "";


                // 料金カラムセットをキーに部屋毎の人数を取り出し
                int[] yoyakuNinzusByRoomType = yoyakuNinzusByChargeKbn[chargeColumns];

                int tmpWaribikiNinzuByRow = ninzu; // 計算用の一時人数
                int waribikiKingakuByRow = 0; // 行単位の割引金額

                // 割引人数を予約のある、部屋タイプ１→５の人数へ振り分けていく
                // 原則、「1名1室タイプの1人当たり単価」 > ... > 「5名1室の1人当たり単価」の為、
                // 部屋タイプ1→５の順で割引金額を計算すると、割引額が最安値になる。
                for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
                {

                    // 残りの割引人数が0ならループ終了
                    if (tmpWaribikiNinzuByRow == 0)
                        break;

                    if (yoyakuNinzusByRoomType[roomType] == 0)
                    {
                        // 残りの予約人数が0
                        if (roomType < CommonHakken.FiveIjyou1R)
                            // １～４名1室なら次の部屋タイプへ
                            continue;
                        else
                        {
                            // ５名1室までループして減らせなかった場合、エラー
                            CommonRegistYoyaku.changeGridBackColor(row.Index, 25, this.grdWaribikiCharge);
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引人数", "予約人数以下");
                            waribikiKingakuByRow = 0;
                            break;
                        }
                    }

                    int cntAdd = 0;

                    // 部屋ごとの予約人数が0になるか、行ごとの人数が0になるまで減算
                    while (yoyakuNinzusByRoomType[roomType] > 0 && tmpWaribikiNinzuByRow > 0)
                    {
                        // 予約の人数を減算
                        yoyakuNinzusByRoomType[roomType] -= 1;
                        tmpWaribikiNinzuByRow -= 1;

                        cntAdd += 1;

                        // 部屋タイプ５が終わっても、割引人数が残っていればエラー
                        if (roomType == CommonHakken.FiveIjyou1R && yoyakuNinzusByRoomType[roomType] == 0 && tmpWaribikiNinzuByRow > 0)
                        {
                            CommonRegistYoyaku.changeGridBackColor(row.Index, 25, this.grdWaribikiCharge);
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引人数", "予約人数以下");
                            waribikiKingakuByRow = 0;
                            break;
                        }
                    }

                    // 割引の人数へ加算
                    int waribikiNinzu = int.Parse(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString());
                    waribikiNinzu += cntAdd;
                    row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}") = waribikiNinzu.ToString();

                    // 割引単価の取得
                    int waribikiTanka = this.setWaribikiTanka(tani, waribiki, jininCd, roomType, carriageWaribikiFlg);
                    row.Item($"WARIBIKI_TANKA_{roomType}") = waribikiTanka;

                    // 割引単価 * 割引適用人数を合計
                    waribikiKingakuByRow += waribikiTanka * waribikiNinzu;
                } // ここまで部屋タイプ1～5のループ


                // 割引金額の合計を表示
                row.Item("WARIBIKI_KINGAKU") = waribikiKingakuByRow == 0 ? "" : waribikiKingakuByRow.ToString();

                // 割引へ追加
                this.WaribikiTotal += waribikiKingakuByRow;
            }
        }
    }

    /// <summary>
    /// 割引人数の初期化
    /// </summary>
    private void initiarizeGrdWaribikiNinzu()
    {
        foreach (Row row in this.grdWaribikiCharge.Rows)
        {
            if (row.Index == 0)
                continue;

            for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
            {
                row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}") = "0";
                row.Item($"WARIBIKI_TANKA_{roomType}") = "0";
            }

            row.Item($"WARIBIKI_KINGAKU") = "";
        }
    }

    /// <summary>
    /// 割引単価の設定
    /// </summary>
    /// <returns></returns>
    private int setWaribikiTanka(string tani, decimal waribiki, string jinincd, int roomtype, string carriagewaribikiflg)
    {
        decimal dWaribikiTanka = 0;

        if (tani.Equals(FixedCdYoyaku.Tani.Yen))
            // 割引「額」の場合
            dWaribikiTanka = waribiki;
        else if (tani.Equals(FixedCdYoyaku.Tani.Per))
        {
            // 割引「率」の場合

            // 正規料金の取得（運賃割引フラグを考慮）
            decimal seikiCharge = CommonHakken.getSeikiChargeWithCarriage(jinincd, roomtype, carriagewaribikiflg, this._yoyakuInfoCrsChargeChargeKbn);
            // 端数処理前の割引金額
            decimal waribikiBeforeRound = seikiCharge * waribiki / (double)100M;

            // 四捨五入
            dWaribikiTanka = YoyakuBizCommon.roundWaribiki(waribikiBeforeRound, this.IsTeiki);
        }

        return Convert.ToInt32(dWaribikiTanka);
    }

    /// <summary>
    /// 入金額の集計
    /// </summary>
    private void calculateNyuukin()
    {
        this.Nyuukin = 0;

        FlexGridEx grd = this.grdSeisan;

        foreach (Row row in grd.Rows)
        {
            int nyuukin = 0;
            int henkin = 0;

            // ヘッダーなら次の行へ
            if (row.Index == 0)
                continue;

            // nullなら次の行へ
            if (CommonHakken.isNull(row.Item("KINGAKU")))
                continue;

            // 金額を取り出し
            int.TryParse(row.Item("KINGAKU").ToString(), ref nyuukin);
            row.Item("TAISYAKU_KINGAKU") = row.Item("KINGAKU");

            // 貸方の場合、符号反転
            string TaisyakuKbn = row.Item("TAISYAKU_KBN") as string ?? "";
            if (TaisyakuKbn.Equals(CommonHakken.convertEnumToString(FixedCd.TaisyakuKbn.Kasikata)))
            {
                henkin = nyuukin;
                nyuukin = 0;
                row.Item("TAISYAKU_KINGAKU") = (-henkin).ToString();
            }

            // 入金額へ追加
            this.Nyuukin += nyuukin;
            this.Nyuukin -= henkin;

            // 予約センター返金の入力があれば、貸方分を減算
            if (this.YoyakuCenterHenkin > 0)
                this.YoyakuCenterHenkin -= henkin;
        }
    }

    /// <summary>
    /// 各金額グリッドの設定(各集計金額を再表示）
    /// </summary>
    private void setEachKingaku()
    {
        this.setKingaku1();
        this.setKingaku2();
    }

    /// <summary>
    /// 金額グリッド１の設定
    /// </summary>
    private void setKingaku1()
    {
        grdKingaku.Rows.Fixed = 0;
        grdKingaku2.Rows.Fixed = 0;

        // 金額グリッド1の初期化
        CommonHakken.setInitiarizeGrid(this.grdKingaku);
        DataTable dt = new DataTable();
        dt.Columns.Add(NameColGrdKingaku);
        DataRow row = dt.NewRow;

        // 予約情報(基本）
        DataTable yoyakuTbl = this._yoyakuInfoBasicTable;

        // 料金
        this.setKingaku(ref dt, row, this.ChargeTotal);

        // キャンセル料
        this.setKingaku(ref dt, row, this.Cancel);

        // 割引額
        this.setKingaku(ref dt, row, this.WaribikiTotal);

        // 取扱手数料
        this.setKingaku(ref dt, row, this.ToriatukaiFee);

        // 請求額
        this.setKingaku(ref dt, row, this.SeikyuTotal);

        this.grdKingaku.DataSource = dt;

        // 行ヘッダを設定
        grdKingaku.Rows(NoRowGrdKingakuCharge).Caption = "料金";
        grdKingaku.Rows(NoRowGrdKingakuCancel).Caption = "キャンセル料";
        grdKingaku.Rows(NoRowGrdKingakuWaribiki).Caption = "割引額";
        grdKingaku.Rows(NoRowGrdKingakuToriatukaiFee).Caption = "取扱手数料";
        grdKingaku.Rows(NoRowGrdKingakuSeikyu).Caption = "請求額";

        // 割引行を赤文字
        grdKingaku.Styles.Add("redChar");
        grdKingaku.Styles("redChar").ForeColor = Color.Red;
        grdKingaku.Rows(NoRowGrdKingakuWaribiki).Style = grdKingaku.Styles("redChar");

        // 金額表示欄桁数付与
        CommonHakken.setGridLength(this.grdKingaku, "colKingaku", 7);

        // 料金、割引、請求を非活性
        CommonHakken.disableGrdCell(this.grdKingaku, grdKingaku.Rows(NoRowGrdKingakuCharge), grdKingaku.Rows(NoRowGrdKingakuWaribiki), grdKingaku.Rows(NoRowGrdKingakuSeikyu));
    }

    /// <summary>
    /// 金額グリッド２の設定
    /// </summary>
    private void setKingaku2()
    {
        DataTable dt2 = new DataTable();
        dt2.Columns.Add(NameColGrdKingaku);
        DataRow row = dt2.NewRow;

        // 既入金額
        this.setKingaku(ref dt2, row, this.PreNyukinGaku);

        // 入金額
        this.setKingaku(ref dt2, row, this.Nyuukin);

        // '内金請求額
        // Me.setKingaku(dt2, row, Me.UtikinSeikyu)

        // 残金
        this.setKingaku(ref dt2, row, this.Zankin);

        // 予約センター返金
        this.setKingaku(ref dt2, row, this.YoyakuCenterHenkin);

        // おつり
        this.setKingaku(ref dt2, row, this.Oturi);

        this.grdKingaku2.DataSource = dt2;

        // 行ヘッダを設定
        grdKingaku2.Rows(NoRowGrdKingaku2PreNyukinGaku).Caption = "既入金額";
        grdKingaku2.Rows(NoRowGrdKingaku2Nyukin).Caption = "入金額";
        grdKingaku2.Rows(NoRowGrdKingaku2Zankin).Caption = "残金";
        // grdKingaku2.Rows(NoRowGrdKingaku2UchikinSeikyu).Caption = "内金請求額"
        grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin).Caption = "予約センター返金";
        grdKingaku2.Rows(NoRowGrdKingaku2Otsuri).Caption = "おつり";

        // 既入金額、入金、残金、お釣を非活性
        CommonHakken.disableGrdCell(this.grdKingaku2, grdKingaku2.Rows(NoRowGrdKingaku2PreNyukinGaku), grdKingaku2.Rows(NoRowGrdKingaku2Nyukin), grdKingaku2.Rows(NoRowGrdKingaku2Zankin), grdKingaku2.Rows(NoRowGrdKingaku2Otsuri));

        if (!this._existsHurikomiNyuukin)
            // 予約センター返金がない場合、非活性
            CommonHakken.disableGrdCell(grdKingaku2, grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin));
    }

    /// <summary>
    /// 金額の設定
    /// </summary>
    private void setKingaku(ref DataTable dt, DataRow row, int value)
    {
        row = dt.NewRow();
        row(NameColGrdKingaku) = $"{value}";
        dt.Rows.Add(row);
    }

    /// <summary>
    /// 表示チェック
    /// </summary>
    /// <returns></returns>
    private bool isValidDisp()
    {
        // 予約キャンセルチェック
        string cancelFlg = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CANCEL_FLG") ?? "";
        if (!string.IsNullOrWhiteSpace(cancelFlg))
        {
            if (this.IsTeiki)
                // キャンセル済み　かつ　定期なら発券不可
                base.F10Key_Enabled = false;

            // キャンセル済み
            CommonProcess.createFactoryMsg.messageDisp("E90_034", "予約情報", "キャンセル");
            this.IsUpdated = true;
            this.Close();
            return false;
        }

        // 入金内容チェック
        string seisanHoho = this._yoyakuInfoBasicTable.Rows(0).Field<string>("SEISAN_HOHO") ?? "";
        // If seisanHoho.Equals(FixedCdYoyaku.PaymentHoho.agt) = False _
        // AndAlso Me.SeikyuTotal <> Me.PreNyukinGaku Then
        // '精算方法が代理店でない　かつ　請求額と入金額総計が異なる場合
        // 'TODO:メッセージ
        // CommonProcess.createFactoryMsg.messageDisp("E90_006", "請求額を超える入金", "決済")
        // Me.Close()
        // End If

        // 過剰オンライン決済チェック
        if (this.OnlineCredit > this.SeikyuTotal)
        {
            CommonProcess.createFactoryMsg.messageDisp("E90_006", "請求額を超える入金", "決済");
            this.Close();
        }

        this.IsAlreadyHakken = false;

        if (this.IsTeiki)
        {
            // 定期のチェック
            if (!this.isValidDispTeiki())
                return false;
        }
        else if (this.IsKikaku)
        {
            // 企画のチェック
            if (!this.isValidDispKikaku())
                return false;
        }

        // 使用中チェック
        string usingFlg = this._yoyakuInfoBasicTable.Rows(0).Field<string>("USING_FLG");
        if (!string.IsNullOrWhiteSpace(usingFlg))
        {
            CommonProcess.createFactoryMsg.messageDisp("E90_040");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 表示チェック（定期）
    /// </summary>
    /// <returns></returns>
    private bool isValidDispTeiki()
    {
        // 運休チェック
        string unkyuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("UNKYU_KBN");
        string saikouKakuteiKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("SAIKOU_KAKUTEI_KBN");
        saikouKakuteiKbn = saikouKakuteiKbn == null ? "" : saikouKakuteiKbn;
        if (!string.IsNullOrWhiteSpace(unkyuKbn))
        {
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "運休");

            // 発券ボタンを非活性
            base.F10Key_Enabled = false;
            return false;
        }

        // 催行未確定
        if (saikouKakuteiKbn.Equals(FixedCd.SaikouKakuteiKbn.Tyushi))
        {
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "催行未確定");

            // 発券ボタンを非活性
            base.F10Key_Enabled = false;
            return false;
        }

        // 発券済みチェック（全金発券または残金発券の場合エラー）
        string hakkenNaiyo = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HAKKEN_NAIYO") ?? "";
        if (hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)) || hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zankin)))
        {
            this.IsAlreadyHakken = true;

            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei);
        }
        else if (hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Uchikin)))
        {

            // 内金発券の場合、画面を閉じる
            CommonProcess.createFactoryMsg.messageDisp("E90_006", "内金発券済みの予約情報", UriageKakutei);

            this.IsUpdated = true;
            this.Close();
            return false;
        }

        // 補助席/飛び席チェック
        string yoyakuZasekiKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("YOYAKU_ZASEKI_KBN");
        yoyakuZasekiKbn = yoyakuZasekiKbn == null ? "" : yoyakuZasekiKbn;
        string tobiSeatFlg = this._yoyakuInfoBasicTable.Rows(0).Field<string>("TOBI_SEAT_FLG");
        tobiSeatFlg = tobiSeatFlg == null ? "" : tobiSeatFlg;


        if (yoyakuZasekiKbn.Equals(CommonHakken.YoyakuSeatKbnHojoSeat))
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "補助席");
        else if (yoyakuZasekiKbn.Equals(CommonHakken.YoyakuSeatKbn1FSeat))
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "１Ｆ席");

        if (tobiSeatFlg.Equals(CommonHakken.TobeSeatYes))
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "とび席");

        return true;
    }

    /// <summary>
    /// 表示チェック（企画）
    /// </summary>
    /// <returns></returns>
    private bool isValidDispKikaku()
    {
        // 催行中止チェック
        string saikouKakuteiKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("SAIKOU_KAKUTEI_KBN");
        saikouKakuteiKbn = saikouKakuteiKbn == null ? "" : saikouKakuteiKbn;
        if (saikouKakuteiKbn.Equals(FixedCd.SaikouKakuteiKbn.Tyushi))
        {
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "催行未確定");

            // 発券ボタンを非活性
            base.F10Key_Enabled = false;
            return false;
        }

        // 払戻チェック
        decimal dHakkenKingaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("HAKKEN_KINGAKU") ?? 0;
        int hakkenKingaku = Convert.ToInt32(dHakkenKingaku);
        string hakkenNaiyo = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HAKKEN_NAIYO") ?? "";
        if (hakkenKingaku != 0)
        {
            // 発券金額存在時
            if (this.ChargeTotal <= hakkenKingaku && hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)))
            {
                this.IsAlreadyHakken = true;

                CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei);
            }
            else if (hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Uchikin)))
            {

                // 内金発券の場合、画面を閉じる
                CommonProcess.createFactoryMsg.messageDisp("E90_006", "内金発券済みの予約情報", UriageKakutei);

                this.IsUpdated = true;
                this.Close();
                return false;
            }
        }

        // コースコントロールチェック
        DataTable crsConrolInfo = this.getKey3(CrsControlInfoKey1_11, CrsControlInfoKey2_01);
        if (crsConrolInfo != null)
        {
            string Key3CrsControl = crsConrolInfo.Rows(0).Field<string>("KEY_3");

            string last1DigitKey3CrsControl = Key3CrsControl.Substring(Key3CrsControl.Length - 1, 1);
            string last1DigitCrsCd = this.txtCourseCd.Text.Substring(this.txtCourseCd.Text.Length - 1, 1);
            // コースコード下1桁=コースコントロールのKEY3の下1桁
            if (last1DigitCrsCd.Equals(last1DigitKey3CrsControl))
            {
                CommonProcess.createFactoryMsg.messageDisp("E02_046", UriageKakutei + "不可");

                // 発券ボタンを非活性
                this.F10Key_Enabled = false;
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    private bool isValidInputHakken()
    {
        // 割引額チェック
        if (this.ChargeTotal < this.WaribikiTotal)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引合計", "コース料金未満");
            return false;
        }

        // 請求不正チェック
        if (this.SeikyuTotal < 0)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引合計", "請求額未満");
            return false;
        }

        // キャンセル料チェック
        if (this.Cancel < 0)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "キャンセル料", "0以上");
            return false;
        }

        // 取扱手数料チェック
        if (this.ToriatukaiFee < 0)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "取扱手数料", "0以上");
            return false;
        }

        // 予約センター返金チェック
        if (this.YoyakuCenterHenkin < 0)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "予約センター返金", "0以上");
            return false;
        }

        // 割引グリッドのチェック
        if (!this.isValidGrdWaribiki())
            return false;

        // 精算グリッドのチェック
        if (!this.isValidGrdSeisan())
            return false;

        if (this.IsTeiki)
        {
            if (!this.isValidInputHakkenTeiki())
                return false;
        }
        else if (this.IsKikaku)
        {
            if (!this.isValidInputHakkenKikaku())
                return false;
        }

        return true;
    }

    /// <summary>
    /// 入力チェック（定期）
    /// </summary>
    /// <returns></returns>
    private bool isValidInputHakkenTeiki()
    {
        // 入金なしチェック
        if (this._existsWaribiki == false && this.Nyuukin == 0)
        {
            CommonProcess.createFactoryMsg.messageDisp("E90_023", "入金額");
            return false;
        }

        // 入金不足チェック
        if (this.Nyuukin < this.SeikyuTotal)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金額", "請求額以上");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 入力チェック（企画）
    /// </summary>
    /// <returns></returns>
    private bool isValidInputHakkenKikaku()
    {
        // 2重全金発券チェック
        string hakkenNaiyo = this._yoyakuInfoBasicTable.Rows(0).Field<string>("HAKKEN_NAIYO") ?? "";
        if (hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)))
        {
            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei);
            return false;
        }

        // 発券額チェック
        decimal dHakkenKingaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("HAKKEN_KINGAKU") ?? 0;
        int hakkenKingaku = Convert.ToInt32(dHakkenKingaku);
        if (this.ChargeTotal == hakkenKingaku)
        {
            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei);
            return false;
        }

        // 入金不足（全金）チェック
        if (this.Nyuukin < this.SeikyuTotal)
        {
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金額", "請求額以上");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 割引グリッドチェック
    /// </summary>
    /// <returns></returns>
    private bool isValidGrdWaribiki()
    {
        this._existsWaribiki = false;

        string crsCd = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_CD") ?? "";
        int syuptDay = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;

        // 割引グリッド上のチェック
        foreach (Row row in this.grdWaribikiCharge.Rows)
        {
            // ヘッダー行飛ばす
            if (row.Index == 0)
                continue;

            // 値の取り出し
            string waribikiName = this.grdWaribikiCharge.Item(row.Index, "WARIBIKI_NAME") as string; // 割引名
            string waribikiCd = this.grdWaribikiCharge.Item(row.Index, "WARIBIKI_CD") as string; // 割引コード
            string tani = row.Item("WARIBIKI_KBN") as string ?? "";  // 単位
            string chargeName = this.grdWaribikiCharge.Item(row.Index, "CHARGE_NAME") as string; // 料金区分
            string jininName = this.grdWaribikiCharge.Item(row.Index, "CHARGE_KBN_JININ_NAME") as string; // 人員
            // 割引
            string strWaribiki = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI")))
                strWaribiki = row.Item("WARIBIKI").ToString();
            int waribiki = 0;
            int.TryParse(strWaribiki, ref waribiki);
            // 割引人数
            string strWaribikiNinzu = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI_NINZU")))
                strWaribikiNinzu = row.Item("WARIBIKI_NINZU").ToString();
            int waribikiNinzu = 0;
            int.TryParse(strWaribikiNinzu, ref waribikiNinzu);
            // 割引可能な人数の上限
            string strMaxWaribikiNinzu = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI_APPLICATION_NINZU")))
                strMaxWaribikiNinzu = row.Item("WARIBIKI_APPLICATION_NINZU").ToString();
            int maxWaribikiNinzu = 0;
            int.TryParse(strMaxWaribikiNinzu, ref maxWaribikiNinzu);
            string carriageWaribikiFlg = row.Item("CARRIAGE_WARIBIKI_FLG") as string ?? "";
            // 割引金額
            string strWaribikiKinagku = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI_KINGAKU")))
                strWaribikiKinagku = row.Item("WARIBIKI_KINGAKU").ToString();
            int waribikiKingaku = 0;
            int.TryParse(strWaribikiKinagku, ref waribikiKingaku);
            // 備考
            string biko = row.Item("WARIBIKI_BIKO") as string ?? "";

            if (string.IsNullOrWhiteSpace(waribikiName))
            {
                // 割引コードが空白アイテムの場合

                // 割引コード, 名称必須チェック
                // 全部空でなければエラー
                if (!(waribiki == 0 && string.IsNullOrWhiteSpace(chargeName) && string.IsNullOrWhiteSpace(jininName) && waribikiNinzu == 0 && string.IsNullOrWhiteSpace(biko)))
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引コード");
                    return false;
                }
            }
            else
            {
                // 割引コード選択の場合
                this._existsWaribiki = true;

                // 割引必須チェック
                if (waribiki <= 0)
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 7, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引");
                    return false;
                }

                if (tani.Equals(FixedCdYoyaku.Tani.Per))
                {
                    // 割引率不正チェック
                    if (waribiki > 100)
                    {
                        CommonRegistYoyaku.changeGridBackColor(row.Index, 7, this.grdWaribikiCharge);
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引率", "100未満の値");
                        return false;
                    }
                }
                else if (tani.Equals(FixedCdYoyaku.Tani.Yen))
                {
                    // 割引額不正チェック
                    for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
                    {
                        int seikiCharge = CommonHakken.getSeikiCharge(jininName, roomType, this._yoyakuInfoCrsChargeChargeKbn);
                        if (0 < seikiCharge && seikiCharge < waribiki)
                        {
                            CommonRegistYoyaku.changeGridBackColor(row.Index, 7, this.grdWaribikiCharge);
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引額", "コース料金以下");
                            return false;
                        }
                    }
                }
                else
                {
                    // 単位不正チェック
                    CommonProcess.createFactoryMsg.messageDisp("E90_016", "割引の単位");
                    return false;
                }

                // 料金区分必須チェック
                if (this.IsStayAri == false)
                {
                    if (string.IsNullOrWhiteSpace(chargeName))
                    {
                        CommonRegistYoyaku.changeGridBackColor(row.Index, 12, this.grdWaribikiCharge);
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "料金区分");
                        return false;
                    }
                }

                // 割引適用者必須チェック
                if (string.IsNullOrWhiteSpace(jininName))
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 14, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引適用者");
                    return false;
                }

                // 割引人数必須チェック
                if (waribikiNinzu == 0)
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 25, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引人数");
                    return false;
                }

                // 割引金額チェック
                if (waribikiKingaku <= 0)
                {
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引金額");
                    return false;
                }

                // 割引適用人数チェック
                if (maxWaribikiNinzu < waribikiNinzu)
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 25, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の人数", "割引適用人数内");
                    return false;
                }

                // ホリデーチケット適用コース確認
                if (this.isAppliedCourseForHolidayTicket(crsCd, waribikiCd) == false)
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg().messageDisp("E02_018");
                    return false;
                }

                // ホリデーチケット適用範囲確認
                if (this.isHolidayTicketScopeOfApplication(syuptDay, waribikiCd) == false)
                {
                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, this.grdWaribikiCharge);
                    CommonProcess.createFactoryMsg().messageDisp("E02_019");
                    return false;
                }
            }    // ここまで割引コード選択の場合
        } // ここまで割引グリッドのチェック

        return true;
    }

    /// <summary>
    /// ホリデーチケット適用コース確認
    /// </summary>
    /// <param name="crsCd">コースコード</param>
    /// <param name="waribikiCd">割引コード</param>
    /// <returns>検証結果</returns>
    private bool isAppliedCourseForHolidayTicket(string crsCd, string waribikiCd)
    {
        if (waribikiCd != "004")
            // 割引コードがホリデーチケット以外の場合、チェックなし
            return true;

        HolidayCrsCdEntity entity = new HolidayCrsCdEntity();
        entity.crsCd.Value = crsCd;

        S02_0103Da s02_0103Da = new S02_0103Da();
        DataTable holidayCrsInfo = s02_0103Da.getHolidayCrs(entity);

        if (holidayCrsInfo.Rows.Count < CommonRegistYoyaku.ZERO)
            return false;

        return true;
    }

    /// <summary>
    /// ホリデーチケット適用範囲確認
    /// </summary>
    /// <param name="syuptDay">出発日</param>
    /// <param name="waribikiCd">割引コード</param>
    /// <returns>検証結果</returns>
    private bool isHolidayTicketScopeOfApplication(int syuptDay, string waribikiCd)
    {
        if (waribikiCd != "004")
            // 割引コードがホリデーチケット以外の場合、チェックなし
            return true;

        HolidayApplicationDayEntity entity = new HolidayApplicationDayEntity();
        entity.applicationDayFrom.Value = syuptDay;
        entity.applicationDayTo.Value = syuptDay;

        S02_0103Da s02_0103Da = new S02_0103Da();
        DataTable holidayCrsInfo = s02_0103Da.getHolidayApplicationDay(entity);

        if (holidayCrsInfo.Rows.Count < CommonRegistYoyaku.ZERO)
            return false;

        return true;
    }

    /// <summary>
    /// 精算グリッドチェック
    /// </summary>
    /// <returns></returns>
    private bool isValidGrdSeisan()
    {
        // 精算グリッド上のチェック
        foreach (Row row in this.grdSeisan.Rows)
        {
            // ヘッダー行飛ばす
            if (row.Index == 0)
                continue;

            // 値の取り出し
            // 精算項目コード
            string seisanKoumokuCd = row.Item("SEISAN_KOUMOKU_CD") as string ?? "";
            string seisanKoumokuName = this.grdSeisan.Item(row.Index, "SEISAN_KOUMOKU_NAME") as string; // 精算項目名
            // 金額
            string strKingaku = "";
            if (!CommonHakken.isNull(row.Item("KINGAKU")))
                strKingaku = row.Item("KINGAKU").ToString();
            int kingaku = 0;
            int.TryParse(strKingaku, ref kingaku);
            string utiwake = row.Item("HURIKOMI_KBN") as string; // 内訳
            string biko = row.Item("BIKO") as string; // 備考


            if (string.IsNullOrWhiteSpace(seisanKoumokuName))
            {
                // 精算項目が空白アイテムの場合

                // 割引コード, 名称必須チェック
                // 全部空でなければエラー
                if (!(kingaku == 0 && string.IsNullOrWhiteSpace(utiwake) && string.IsNullOrWhiteSpace(biko)))
                {
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金の入力", "精算項目");
                    return false;
                }
            }
            else
            {
                // 精算項目が選択されている場合

                // 金額必須チェック
                // 現金、ｸﾚｼﾞｯﾄ、予約C振込、オンライン決済は初期表示の為、0でも可
                if (!seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) && !seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card)) && !seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.hurikomi_yoyaku_center)) && !seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit)))
                {
                    if (kingaku == 0)
                    {
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金の入力", "金額");
                        return false;
                    }
                }

                // 精算項目がクレジットで金額が請求金額を超えておつりが発生する場合、処理中断
                if ((seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card)) | seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit))) && kingaku > this.SeikyuTotal)
                {
                    CommonProcess.createFactoryMsg.messageDisp("E90_006", "精算項目がクレジットの場合", "返金"); // ("E02_054")
                    return false;
                }

                // 船車券チェック
                if (seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)) || seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)))
                {
                    // 船車券の入金の場合、出発日が未来ならエラーとする
                    DateTime syuptDay = DateTime.Parse(this.ucoSyuptDay.Value.ToString());
                    bool isMiraiSyuptDay = CommonConvertUtil.ChkWhenDay(syuptDay, CommonConvertUtil.WhenMirai);
                    if (isMiraiSyuptDay)
                    {
                        CommonProcess.createFactoryMsg().messageDisp("E02_022");
                        return false;
                    }
                }
            } // ここまで精算項目コード選択の場合
        } // ここまで精算グリッドのチェック

        return true;
    }

    /// <summary>
    /// 券番の採番
    /// </summary>
    private void numberingKenNo()
    {
        int syuptDay = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
        this._newKenNo = CommonHakken.numberingKenNo(this.IsTeiki, syuptDay);

        this._hakkenInfoSeq1 = this._newKenNo.PadLeft(10, '0').Substring(5, 1);
        this._hakkenInfoSeq2 = int.Parse(this._newKenNo.PadLeft(10, '0').Substring(6, 4));
    }

    /// <summary>
    /// 割引種別単位の情報を設定
    /// </summary>
    private void setInfosByWaribikiType()
    {
        // フィールドへインスタンスを設定
        // 割引コード種別、人員コード単位の参加人数
        this._infoByWaribikiCdJininCd = CommonHakken.createInfoWaribikiCdJininCd;

        // 割引種別単位の参加人数
        this._ninzuByWaribikiType = new Dictionary<string, int>();

        // 割引種別ごとに各精算項目へ按分をかける請求金額
        this._seikyuByWaribikiType = new Dictionary<string, int>();

        // 割引種別ごとに各精算項目へ按分をかける割引金額
        this._waribikiByWaribikiType = new Dictionary<string, int>();

        // 割引種別ごとの正規料金 
        this._seikiChargeByWaribikiType = new Dictionary<string, int>();


        // 割引グリッドの値を集計
        foreach (Row row in this.grdWaribikiCharge.Rows)
        {
            // ヘッダー行なら次の行へ
            if (row.Index() == 0)
                continue;
            // 空なら次の行へ
            string waribikiCd = row.Item("WARIBIKI_CD") as string ?? "";
            if (waribikiCd.Equals(""))
                continue;

            // グリッドの値を取り出し
            // 人数
            string strNinzuByGrdRow = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI_NINZU")))
                strNinzuByGrdRow = row.Item("WARIBIKI_NINZU").ToString();
            int ninzuByGrdRow = 0;
            int.TryParse(strNinzuByGrdRow, ref ninzuByGrdRow);
            // 割引金額
            string strWaribikiKingakuByGrdRow = "";
            if (!CommonHakken.isNull(row.Item("WARIBIKI_KINGAKU")))
                strWaribikiKingakuByGrdRow = row.Item("WARIBIKI_KINGAKU").ToString();
            int waribikiKingakuByGrdRow = 0;
            int.TryParse(strWaribikiKingakuByGrdRow, ref waribikiKingakuByGrdRow);


            // 割引種別単位でデータを集計する
            string waribikiType = row.Item("WARIBIKI_TYPE_KBN") as string ?? "";

            // 割引コード種別、人員コード単位の人数にデータを分解
            CommonHakken.setInfoByWaribikiCdJininCd(row, waribikiCd, waribikiType, this._infoByWaribikiCdJininCd);

            // 割引種別単位の人数
            CommonHakken.setNinzuByWaribikiType(waribikiType, ninzuByGrdRow, this._ninzuByWaribikiType);

            // 割引種別ごとに各精算項目へ按分をかける請求金額
            this.setSeikyuByWaribikiType(row, waribikiType, waribikiKingakuByGrdRow, ninzuByGrdRow);
        } // ここまで割引グリッドのループ

        // 割引付きの請求額の合計
        int sumWaribikiSeikyu = this._seikyuByWaribikiType.Sum(seikyu => seikyu.Value);
        int sumWaribiki = this._waribikiByWaribikiType.Sum(waribiki => waribiki.Value);

        // 残額を[割引種別：正規料金]として割引種別ごとの請求へ追加
        int seikyuSeikiCharge = this.ChargeTotal - sumWaribikiSeikyu - sumWaribiki;

        if (seikyuSeikiCharge != 0)
        {
            this._seikyuByWaribikiType.Add(CommonHakken.WaribikiTypeSeikiCharge, seikyuSeikiCharge);
            this._seikiChargeByWaribikiType.Add(CommonHakken.WaribikiTypeSeikiCharge, seikyuSeikiCharge);
        }
        // 正規料金の人数を集計
        CommonHakken.setNinzuSeikiChargeAfterWaribiki(this._yoyakuInfoCrsChargeChargeKbn, this._infoByWaribikiCdJininCd);
    }

    /// <summary>
    /// 割引種別ごとに各精算項目へ按分をかける請求金額
    /// </summary>
    /// <param name="row"></param>
    /// <param name="waribikiType"></param>
    /// <param name="waribikiKingakuByGrdRow"></param>
    private void setSeikyuByWaribikiType(Row row, string waribikiType, int waribikiKingakuByGrdRow, int waribikiNinzuByGrdRow)
    {

        // 人員
        string jininCd = row.Item("CHARGE_KBN_JININ_CD") as string ?? "";
        // 運賃割引フラグ
        string carriageWaribikiFlg = row.Item("CARRIAGE_WARIBIKI_FLG") as string ?? "";

        // 室タイプ１～５の請求額を集計
        for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
        {

            // 割引人数
            string strWaribikiApplicationNinzuN = "";
            int waribikiApplicationNinzuN = 0;
            if (!CommonHakken.isNull(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}")))
                strWaribikiApplicationNinzuN = row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString();
            int.TryParse(strWaribikiApplicationNinzuN, ref waribikiApplicationNinzuN);

            // 正規料金、人数を取得
            int seikiCharge = CommonHakken.getSeikiCharge(jininCd, roomType, this._yoyakuInfoCrsChargeChargeKbn);
            int seikiNinzu = CommonHakken.getSeikiNinzu(jininCd, roomType, this._yoyakuInfoCrsChargeChargeKbn);

            // 割引種別単位の正規料金へ加算
            if (!this._seikiChargeByWaribikiType.ContainsKey(waribikiType))
                this._seikiChargeByWaribikiType.Add(waribikiType, 0);
            this._seikiChargeByWaribikiType[waribikiType] += seikiCharge * waribikiApplicationNinzuN;

            // 割引種別単位の請求額へ加算
            if (!this._seikyuByWaribikiType.ContainsKey(waribikiType))
                this._seikyuByWaribikiType.Add(waribikiType, 0);
            this._seikyuByWaribikiType[waribikiType] += seikiCharge * waribikiApplicationNinzuN;
        } // ここまで室タイプ１～５のループ

        // 割引種別単位の請求額から割引金額を減算
        this._seikyuByWaribikiType[waribikiType] -= waribikiKingakuByGrdRow;

        // 割引種別単位の割引額へ割引金額を加算
        if (!this._waribikiByWaribikiType.ContainsKey(waribikiType))
            this._waribikiByWaribikiType.Add(waribikiType, 0);
        this._waribikiByWaribikiType[waribikiType] += waribikiKingakuByGrdRow;
    }

    /// <summary>
    /// 精算項目単位の未按分金（入金額）を集計
    /// </summary>
    private Dictionary<string, int> setUndistributedKinBySeisanKoumoku()
    {
        Dictionary<string, int> nyuukinBySeisanKoumoku = new Dictionary<string, int>();
        bool _GenkinSeisan = false;    // 精算項目「現金」での精算処理済みフラグ（False：未処理、True：処理済み）
        // 精算グリッドの値を取り出し
        foreach (Row row in this.grdSeisan.Rows)
        {
            // ヘッダー行なら次の行へ
            if (row.Index() == 0)
                continue;
            // 空なら次の行へ
            string seisanKoumokuCd = row.Item("SEISAN_KOUMOKU_CD") as string ?? "";
            if (seisanKoumokuCd.Equals(""))
                continue;

            // グリッドの値を取り出し
            // 金額
            string strKingakuByGrdRow = "";
            if (!CommonHakken.isNull(row.Item("TAISYAKU_KINGAKU")))
                strKingakuByGrdRow = row.Item("TAISYAKU_KINGAKU").ToString();
            int kingakuByGrdRow = 0;
            int.TryParse(strKingakuByGrdRow, ref kingakuByGrdRow);

            // 0なら次の行へ
            if (kingakuByGrdRow == 0)
                continue;

            // 精算項目単位の入金
            if (!nyuukinBySeisanKoumoku.ContainsKey(seisanKoumokuCd))
                nyuukinBySeisanKoumoku.Add(seisanKoumokuCd, 0);

            // 精算項目コードに応じて、返金額を減算
            if (seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) & _GenkinSeisan == false)
            {
                // 現金の場合、請求額からおつりを減算
                if (kingakuByGrdRow >= this.Oturi)
                {
                    kingakuByGrdRow -= this.Oturi;
                    nyuukinBySeisanKoumoku[seisanKoumokuCd] += kingakuByGrdRow;
                    _GenkinSeisan = true;    // 精算項目「現金」での精算処理完了
                }
                else
                {
                    // 減算するとマイナスになる場合は、精算項目「現金戻」におつりを計上する
                    if (!nyuukinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)))
                        nyuukinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi), this.Oturi);
                    else
                        nyuukinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)] = this.Oturi;
                    // 精算項目「現金戻」におつりを計上したため、精算項目「現金」におつりは減算せずそのまま計上
                    nyuukinBySeisanKoumoku[seisanKoumokuCd] += kingakuByGrdRow;
                }
            }
            else
                nyuukinBySeisanKoumoku[seisanKoumokuCd] += kingakuByGrdRow;
        } // ここまで精算グリッドのループ

        // 現金精算なし　かつ　おつりが発生時
        if (!nyuukinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)))
            // 貸借先の精算項目へおつりを追加
            this.setTaisyakuTaioHenkin(ref nyuukinBySeisanKoumoku);

        return nyuukinBySeisanKoumoku;
    }

    /// <summary>
    /// 貸借対応先（「現金戻」）の返金の設定
    /// </summary>
    private void setTaisyakuTaioHenkin(ref Dictionary<string, int> nyuukinBySeisanKoumoku)
    {
        // 振込時は追加しないで終了
        if (this._existsHurikomiNyuukin)
            return;

        // 精算グリッドに精算項目がない場合、終了
        if (nyuukinBySeisanKoumoku.Any() == false)
            return;

        // '精算項目ごとの入金から最大金額の精算項目コードを取得
        // Dim maxKingakuSeisanItem As String = nyuukinBySeisanKoumoku.OrderByDescending(Function(item) item.Value).First.Key
        // '最大金額の貸借対応コードを取得
        // Dim taisyakuTaioCd As String = Me.getTaisyakuTaioCd(maxKingakuSeisanItem)

        // If taisyakuTaioCd.Equals(ErrorTaisyakuTaioCd) Then
        // '貸借先がない精算項目で返金が発生時には、オペレートミスとして例外スロー
        // CommonProcess.createFactoryMsg.messageDisp("E02_054")
        // Throw New Exception
        // Else
        // '貸借先へおつり金額を格納
        // nyuukinBySeisanKoumoku.Add(taisyakuTaioCd, Me.Oturi)
        // End If

        // 貸借先「現金戻」へおつり金額を格納
        if (this.Oturi > 0)
        {
            if (!nyuukinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)))
                nyuukinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi), this.Oturi);
            else
                nyuukinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)] += this.Oturi;
        }
    }

    /// <summary>
    /// コースコントロール情報の取得
    /// </summary>
    /// <param name="key1">KEY1</param>
    /// <param name="key2">KEY2</param>
    /// <returns></returns>
    private DataTable getKey3(string key1, string key2)
    {
        S02_0602Da da = new S02_0602Da();
        DataTable dt = da.getKey3CrsControlInfo(key1, key2);

        return CommonHakken.existsDatas(dt) ? dt : null;
    }

    /// <summary>
    /// 入返金情報の取得
    /// </summary>
    /// <param name="yoyakuKbn"></param>
    /// <param name="yoyakuNo"></param>
    /// <returns></returns>
    private DataTable getNyuukin(string yoyakuKbn, int yoyakuNo)
    {
        S02_0602Da da = new S02_0602Da();
        DataTable dt = da.getNyuukin(yoyakuKbn, yoyakuNo);

        return CommonHakken.existsDatas(dt) ? dt : null;
    }

    /// <summary>
    /// 貸借対応コードの取得
    /// </summary>
    /// <param name="seisanKoumokuCd"></param>
    /// <returns></returns>
    private string getTaisyakuTaioCd(string seisanKoumokuCd)
    {
        S02_0602Da da = new S02_0602Da();
        DataTable dt = da.getTaisyakuTaioCd(seisanKoumokuCd);

        var taisyakuTaioCd = dt.Rows(0).Field<string>("TAISYAKU_TAIO_CD") ?? "";

        return taisyakuTaioCd;
    }

    /// <summary>
    /// 使用中フラグの更新
    /// </summary>
    private bool updateUsingFlg(bool toRaising)
    {
        // サーバー日付取得
        Hashtable sysdates = this.getSysDates();

        // エンティティの設定
        YoyakuInfoBasicEntity ent = new YoyakuInfoBasicEntity();
        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
        ent.usingFlg.Value = toRaising ? FixedCd.UsingFlg.Use : FixedCd.UsingFlg.Unused;
        ent.updatePersonCd.Value = UserInfoManagement.userId;
        ent.updatePgmid.Value = PgmId;
        ent.updateDay.Value = (int)sysdates(KeyIntSysDate);
        ent.updateTime.Value = (int)sysdates(KeyIntSysTimeHhMmSs);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;
        ent.systemUpdateDay.Value = (DateTime)sysdates(KeyDtSysDate);

        S02_0602Da da = new S02_0602Da();
        return da.updateUsingFlg(ent);
    }

    /// <summary>
    /// 発券情報グループの登録
    /// </summary>
    /// <returns></returns>
    private bool insertHakkenGroup()
    {

        // 座席状態更新
        string busReserveCd = string.Empty;
        int beforeZasekiStateCd = 0;
        // 共通処理実行
        if (setZasekiState(ref busReserveCd, ref beforeZasekiStateCd, false) == false)
            return false;

        try
        {
            // エンティティの設定
            HakkenGroupEntity hakkenGroupEntity = this.setHakkenGroup();

            S02_0602Da da = new S02_0602Da();
            if (da.insertHakkenGroup(hakkenGroupEntity, false) == true)
                // 更新成功
                return true;
            else
            {
                // 更新失敗(exception以外) → 座席ステータスを戻す
                resetZasekiStateCd(beforeZasekiStateCd, busReserveCd);
                return false;
            }
        }
        catch (Exception ex)
        {
            // ここのcatchでは、座席のステータスを戻す必要がある
            resetZasekiStateCd(beforeZasekiStateCd, busReserveCd);
            throw;
        }
    }

    /// <summary>
    /// 予約情報（基本）の更新（ノーショウのみ）
    /// </summary>
    /// <returns></returns>
    private bool updateYoyakuInfoBasicForNoShow()
    {
        try
        {
            // エンティティの設定
            HakkenGroupEntity YoyakuInfoBasicForNoShowEntity = this.setRegistInfoForNoShow();

            S02_0602Da da = new S02_0602Da();
            if (da.updateYoyakuInfoBaiscForNoShow(YoyakuInfoBasicForNoShowEntity) == true)
                // 更新成功
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// 共通処理を使用し、座席ステータスを更新する
    ///  ※F10⇒発券済み（処理区分10）を実行
    /// </summary>
    /// <param name="busReserveCd">byref。後続処理で失敗した場合に使用すること</param>
    /// <param name="beforeZasekiStateCd">byref。後続処理で失敗した場合に使用すること</param>
    /// <param name="voidFlg"></param>
    /// <returns></returns>
    private bool setZasekiState(ref string busReserveCd, ref int beforeZasekiStateCd, bool voidFlg)
    {
        S02_0602Da da = new S02_0602Da();

        // ------------------------------
        // 処理実行前準備
        // ------------------------------
        // バス指定コードの取得
        string tmpBusReserveCd = da.getBusReserveCd(this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_CD"), System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY")), System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA")));
        // 座席イメージ（バス）の取得
        DataTable zasekiDt = da.getZasekiImage(tmpBusReserveCd, System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY")), System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA")));
        // 座席イメージ（座席）の取得
        DataTable zasekiImageDt = da.getZasekiImageInfo(tmpBusReserveCd, System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY")), System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA")));
        // データ取得NGは処理終了
        if (zasekiDt == null || zasekiDt.Rows.Count == 0 || zasekiImageDt == null || zasekiImageDt.Rows.Count == 0)
            return false;
        // エンティティへ格納
        EntityOperation<TZasekiImageEntity> zasekiImage = YoyakuBizCommon.setEntityFromDataTable<TZasekiImageEntity>(zasekiDt);
        EntityOperation<TZasekiImageInfoEntity> zasekiImageInfo = YoyakuBizCommon.setEntityFromDataTable<TZasekiImageInfoEntity>(zasekiImageDt);
        // 座席イメージ（座席）の対象データをセット
        var query = (from d in zasekiImageInfo.EntityData
                     where d.YoyakuKbn.Value == this._yoyakuInfoBasicTable.Rows(0).Field<string>("YOYAKU_KBN") && d.YoyakuNo.Value == System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("YOYAKU_NO"))
                     select d).First();

        // ------------------------------
        // 引数のパラーメタに値をセットする
        busReserveCd = tmpBusReserveCd;
        beforeZasekiStateCd = System.Convert.ToInt32(query.ZasekiState.Value);

        // ------------------------------
        // 共通処理（座席自動配置Z0008）実行
        // ------------------------------
        Zaseki.Z0008 z0008 = new Zaseki.Z0008();
        Zaseki.Z0008_Param z0008Param = new Zaseki.Z0008_Param();
        Zaseki.Z0008_Result z0008Result = new Zaseki.Z0008_Result();

        // パラメータセット
        if (voidFlg == false)
            // 発券処理時
            z0008Param.ProcessKbn = Zaseki.Z0008_Param.Z0008_Param_ProcessKbn.ProcessKbn_10;
        else
            // VOID処理時
            z0008Param.ProcessKbn = Zaseki.Z0008_Param.Z0008_Param_ProcessKbn.ProcessKbn_30;
        z0008Param.CrsCd = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_CD");                      // コースコード
        z0008Param.SyuptDay = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY"));        // 出発日
        z0008Param.Gousya = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA"));               // 号車
        z0008Param.BusReserveCd = tmpBusReserveCd;                                                           // バス指定コード
        z0008Param.YoyakuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("YOYAKU_KBN");              // 予約区分
        z0008Param.YoyakuNo = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("YOYAKU_NO"));        // 予約No
        z0008Param.GroupNo = System.Convert.ToInt32(query.GroupNo.Value);                                                     // グループNO
        z0008Param.TobiSeatKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("TOBI_SEAT_FLG");         // とび席区分
        z0008Param.Ninzu = this._totalYoyakuNinzu;                                                             // 人数
        z0008Param.BusInfo = zasekiImage.EntityData(0);                                                      // 座席イメージ（バス情報）
        List<TZasekiImageInfoEntity> zasekiImageInfoList = new List<TZasekiImageInfoEntity>();
        foreach (var tmp in zasekiImageInfo.EntityData)                                                          // 座席イメージ（座席情報）
            zasekiImageInfoList.Add(tmp);
        z0008Param.ZasekiInfo = zasekiImageInfoList;

        // 共通処理の実行
        z0008Result = z0008.Execute(z0008Param);

        // 処理結果判定
        if (z0008Result.Status != Zaseki.Z0008_Result.Z0008_Result_Status.OK && z0008Result.Status != Zaseki.Z0008_Result.Z0008_Result_Status.Kaku)
        {
            // 正常終了(00)、架空車種(10)以外はエラー
            createFactoryLog.logOutput(LogKindType.debugLog, ProcessKindType.sonota, setFormId, setTitle, "座席共通処理実行(Z0008):処理NG（" + z0008Result.Status.ToString + "）");
            return false;
        }
        else
        {
            createFactoryLog.logOutput(LogKindType.debugLog, ProcessKindType.sonota, setFormId, setTitle, "座席共通処理実行(Z0008):処理OK（" + z0008Result.Status.ToString + "）");
            return true;
        }
    }


    /// <summary>
    /// 座席イメージの状態コードを元に戻す
    /// </summary>
    /// <param name="zasekiStateCd"></param>
    /// <param name="busReserveCd"></param>
    /// <returns></returns>
    private bool resetZasekiStateCd(int zasekiStateCd, string busReserveCd)
    {
        if (zasekiStateCd == 0 || busReserveCd == string.Empty)
            return false;

        S02_0602Da Da = new S02_0602Da();

        int syuptDay = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY"));
        int gousya = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA"));
        string yoyakuKbn = this._yoyakuInfoBasicTable.Rows(0).Field<string>("YOYAKU_KBN");
        int yoyakuNo = System.Convert.ToInt32(this._yoyakuInfoBasicTable.Rows(0).Field<int?>("YOYAKU_NO"));

        string sysUpdatePgmid = PgmId;
        string sysUpdatePersonCd = UserInfoManagement.userId;
        DateTime sysUpdateDate = CommonDateUtil.getSystemTime();

        return Da.updateZasekiState(zasekiStateCd, busReserveCd, syuptDay, gousya, yoyakuKbn, yoyakuNo, sysUpdatePgmid, sysUpdatePersonCd, sysUpdateDate);
    }


    /// <summary>
    /// 発券情報グループの設定
    /// </summary>
    /// <returns></returns>
    private HakkenGroupEntity setHakkenGroup()
    {
        // サーバー日付を取得
        Hashtable sysDates = this.getSysDates();

        // 予約情報（割引）エンティティ
        List<YoyakuInfoWaribikiEntity> yoyakuInfoWaribikiList = null;
        if (this._existsWaribiki)
            // 割引あり
            yoyakuInfoWaribikiList = this.createYoyakuInfoWaribikiEntityForHakken(sysDates);

        // 予約情報（基本）エンティティ
        List<string> yoyakuInfoBasicPhysicsNameList = new List<string>();
        YoyakuInfoBasicEntity yoyakuInfoBasic = this.createYoyakuInfoBasicEntityForHakken(ref yoyakuInfoBasicPhysicsNameList, sysDates);

        // 予約情報２エンティティ
        bool existsYoyakuInfo2 = CommonHakken.extistsYoyakuInfo2(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        YoyakuInfo2Entity yoyakuInfo2 = this.createYoyakuInfo2(sysDates, existsYoyakuInfo2);

        // 予約情報（コース料金）エンティティ
        YoyakuInfoCrsChargeEntity yoyakuInfoCrsCharge = this.createYoyakuInfoCrsChargeEntityForHakken(sysDates);

        // 予約情報（コース料金_料金区分）エンティティ
        YoyakuInfoCrsChargeChargeKbnEntity yoyakuInfoCrsChargeChargeKbn = this.createYoyakuInfoCrsChargeChargeKbnEntityForHakken(sysDates);
        List<YoyakuInfoCrsChargeChargeKbnEntity> yoyakuInfoCrsChargeChargeKbnList = new List<YoyakuInfoCrsChargeChargeKbnEntity>();
        yoyakuInfoCrsChargeChargeKbnList.Add(yoyakuInfoCrsChargeChargeKbn);


        // 発券情報エンティティ
        HakkenInfoEntity hakkenInfo = this.createHakkenInfoEntityForHakken(sysDates);

        // 発券情報（料金）エンティティ
        List<HakkenInfoChargeEntity> hakkenInfoCharge = this.createHakkenInfoChargeEntityListForHakken(sysDates);

        // 精算情報セットエンティティ　※割引種別ごとに各精算項目へ按分をかける
        SeisanInfoListSetEntity seisanInfoSetEntity = this.distributeSeisanInfo(sysDates);

        // 入返金情報エンティティ
        NyuukinInfoEntity nyuukinInfoEntity = this.createNyuukinInfoEntity(false, sysDates);

        // 入返金情報エンティティ（予約センター返金あり
        List<string> nyuukinInfoEntityHenkinAriPhysicsNameList = null;
        NyuukinInfoEntity nyuukinInfoEntityHenkinAri = null;
        if (this.YoyakuCenterHenkin > 0)
        {
            nyuukinInfoEntityHenkinAriPhysicsNameList = new List<string>();
            nyuukinInfoEntityHenkinAri = this.createNyuukinInfoEntityHenkinAri(ref nyuukinInfoEntityHenkinAriPhysicsNameList, sysDates);
        }

        // 発券情報グループへエンティティを格納
        HakkenGroupEntity hakkenGroupEntity = new HakkenGroupEntity();
        hakkenGroupEntity.YoyakuInfoWaribikiEntityList = yoyakuInfoWaribikiList;
        hakkenGroupEntity.YoyakuInfoBasicEntity = yoyakuInfoBasic;
        hakkenGroupEntity.YoyakuInfoBasicPhysicsNameList = yoyakuInfoBasicPhysicsNameList;
        hakkenGroupEntity.YoyakuInfo2Entity = yoyakuInfo2;
        hakkenGroupEntity.YoyakuInfoCrsChargeEntity = yoyakuInfoCrsCharge;
        hakkenGroupEntity.YoyakuInfoCrsChargeChargeKbnEntityList = yoyakuInfoCrsChargeChargeKbnList;
        hakkenGroupEntity.HakkenInfoEntity = hakkenInfo;
        hakkenGroupEntity.HakkenInfoChargeEntityList = hakkenInfoCharge;
        hakkenGroupEntity.SeisanInfoListSetEntity = seisanInfoSetEntity;
        hakkenGroupEntity.NyuukinInfoEntity = nyuukinInfoEntity;
        hakkenGroupEntity.NyuukinInfoEntityHenkinAri = nyuukinInfoEntityHenkinAri;

        return hakkenGroupEntity;
    }

    /// <summary>
    /// 登録情報の設定(ノーショウのみ登録用)
    /// </summary>
    /// <returns></returns>
    private HakkenGroupEntity setRegistInfoForNoShow()
    {
        // サーバー日付を取得
        Hashtable sysDates = this.getSysDates();

        // 予約情報（基本）エンティティ
        List<string> yoyakuInfoBasicPhysicsNameList = new List<string>();
        YoyakuInfoBasicEntity yoyakuInfoBasic = this.createYoyakuInfoBasicEntityForNoShow(ref yoyakuInfoBasicPhysicsNameList, sysDates);

        // 発券情報グループへエンティティを格納
        HakkenGroupEntity hakkenGroupEntity = new HakkenGroupEntity();
        hakkenGroupEntity.YoyakuInfoBasicEntity = yoyakuInfoBasic;
        hakkenGroupEntity.YoyakuInfoBasicPhysicsNameList = yoyakuInfoBasicPhysicsNameList;

        return hakkenGroupEntity;
    }

    /// <summary>
    /// 予約情報（割引）エンティティの設定（発券）
    /// </summary>
    /// <returns></returns>
    private List<YoyakuInfoWaribikiEntity> createYoyakuInfoWaribikiEntityForHakken(Hashtable sysDates)
    {
        List<YoyakuInfoWaribikiEntity> list = new List<YoyakuInfoWaribikiEntity>();

        foreach (Row row in this.grdWaribikiCharge.Rows)
        {
            if (row.Index == 0)
                continue;
            // 割引コード
            string waribikiCd = row.Item("WARIBIKI_CD") as string;
            string strKbnNo = "";
            // 区分No（区分No→料金区分コード→料金区分名と紐づく）
            if (!CommonHakken.isNull(row.Item("KBN_NO")))
                strKbnNo = row.Item("KBN_NO").ToString();
            int kbnNo = 0;
            int.TryParse(strKbnNo, ref kbnNo);
            // 人員コード
            string chargeKbnJininCd = row.Item("CHARGE_KBN_JININ_CD") as string;

            // nullなら次の行へ
            if (string.IsNullOrWhiteSpace(waribikiCd) || kbnNo == 0 || string.IsNullOrWhiteSpace(chargeKbnJininCd))

                continue;

            // グリッドからの値の取り出し
            // 割引
            string strWaribiki = "";
            int waribiki = 0;
            if (!CommonHakken.isNull(row.Item("WARIBIKI")))
                strWaribiki = row.Item("WARIBIKI").ToString();
            int.TryParse(strWaribiki, ref waribiki);
            // 割引区分（単位）
            string tani = row.Item("WARIBIKI_KBN") as string ?? "";
            // 割引人数
            string strWaribikiNinzu = "";
            int waribikiNinzu = 0;
            if (!CommonHakken.isNull(row.Item("WARIBIKI_NINZU")))
                strWaribikiNinzu = row.Item("WARIBIKI_NINZU").ToString();
            int.TryParse(strWaribikiNinzu, ref waribikiNinzu);
            // 割引適用人数
            string strWaribikiApplicationNinzu = "";
            int waribikiApplicationNinzu = 0;
            if (!CommonHakken.isNull(row.Item("WARIBIKI_APPLICATION_NINZU")))
                strWaribikiApplicationNinzu = row.Item("WARIBIKI_APPLICATION_NINZU").ToString();
            int.TryParse(strWaribikiApplicationNinzu, ref waribikiApplicationNinzu);
            // 運賃割引フラグ
            string carriageWaribikiFlg = row.Item("CARRIAGE_WARIBIKI_FLG") as string ?? "";

            // エンティティへの設定
            YoyakuInfoWaribikiEntity ent = new YoyakuInfoWaribikiEntity();
            ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
            ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
            ent.kbnNo.Value = kbnNo;
            ent.chargeKbnJininCd.Value = chargeKbnJininCd;
            ent.waribikiCd.Value = waribikiCd;
            int intYear = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
            string strYear = intYear.ToString().PadLeft(8, '0').Substring(0, 4);
            ent.year.Value = int.Parse(strYear);
            string waribikiName = this.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataMap(waribikiCd) as string;
            ent.waribikiRiyuu.Value = waribikiName ?? "";
            string waribikiKbn = "";
            if (tani.Equals(FixedCdYoyaku.Tani.Per))
                waribikiKbn = CommonHakken.convertEnumToString(FixedCd.WaribikiKubun.WaribikiRitsu);
            else if (tani.Equals(FixedCdYoyaku.Tani.Yen))
                waribikiKbn = CommonHakken.convertEnumToString(FixedCd.WaribikiKubun.WaribikiGaku);
            ent.waribikiKbn.Value = waribikiKbn;
            ent.carriageWaribikiFlg.Value = carriageWaribikiFlg;
            ent.yoyakuWaribikiFlg.Value = row.Item("YOYAKU_WARIBIKI_FLG") as string ?? "";

            for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
            {
                // 割引人数
                string strWaribikiApplicationNinzuN = "";
                int waribikiApplicationNinzuN = 0;
                if (!CommonHakken.isNull(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}")))
                    strWaribikiApplicationNinzuN = row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString();
                int.TryParse(strWaribikiApplicationNinzuN, ref waribikiApplicationNinzuN);

                // 割引単価
                string strWaribikiTankaN = "";
                int waribikiTankaN = 0;
                if (!CommonHakken.isNull(row.Item($"WARIBIKI_TANKA_{roomType}")))
                    strWaribikiTankaN = row.Item($"WARIBIKI_TANKA_{roomType}").ToString();
                int.TryParse(strWaribikiTankaN, ref waribikiTankaN);

                switch (roomType)
                {
                    case object _ when CommonHakken.One1R:
                        {
                            ent.waribikiApplicationNinzu1.Value = waribikiApplicationNinzuN;
                            ent.waribikiTanka1.Value = waribikiTankaN;
                            break;
                        }

                    case object _ when CommonHakken.Two1R:
                        {
                            ent.waribikiApplicationNinzu2.Value = waribikiApplicationNinzuN;
                            ent.waribikiTanka2.Value = waribikiTankaN;
                            break;
                        }

                    case object _ when CommonHakken.Three1R:
                        {
                            ent.waribikiApplicationNinzu3.Value = waribikiApplicationNinzuN;
                            ent.waribikiTanka3.Value = waribikiTankaN;
                            break;
                        }

                    case object _ when CommonHakken.Four1R:
                        {
                            ent.waribikiApplicationNinzu4.Value = waribikiApplicationNinzuN;
                            ent.waribikiTanka4.Value = waribikiTankaN;
                            break;
                        }

                    case object _ when CommonHakken.FiveIjyou1R:
                        {
                            ent.waribikiApplicationNinzu5.Value = waribikiApplicationNinzuN;
                            ent.waribikiTanka5.Value = waribikiTankaN;
                            break;
                        }
                }
            }

            ent.waribikiApplicationNinzu.Value = waribikiApplicationNinzu; // 割引適用人数（割引可能な上限の人数
            ent.waribikiBiko.Value = row.Item("WARIBIKI_BIKO") as string ?? "";

            if (tani.Equals(FixedCdYoyaku.Tani.Per))
            {
                // 割引「率」の場合
                ent.waribikiKingaku.Value = 0;
                ent.waribikiPer.Value = Convert.ToInt32(waribiki);
            }
            else if (tani.Equals(FixedCdYoyaku.Tani.Yen))
            {
                // 割引「額」の場合
                ent.waribikiKingaku.Value = Convert.ToInt32(waribiki);
                ent.waribikiPer.Value = 0;
            }

            ent.deleteDay.Value = 0;
            ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.entryPersonCd.Value = UserInfoManagement.userId;
            ent.entryPgmid.Value = PgmId;
            ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.updatePersonCd.Value = UserInfoManagement.userId;
            ent.updatePgmid.Value = PgmId;
            ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
            ent.systemUpdatePgmid.Value = PgmId;
            ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
            ent.systemEntryPgmid.Value = PgmId;

            list.Add(ent);
        }

        return list;
    }

    /// <summary>
    /// 予約情報（基本）エンティティの設定　（発券）
    /// </summary>
    private YoyakuInfoBasicEntity createYoyakuInfoBasicEntityForHakken(ref List<string> physicsNameList, Hashtable sysDates)
    {
        YoyakuInfoBasicEntity ent = new YoyakuInfoBasicEntity();
        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn; // 予約区分
        physicsNameList.Add(ent.yoyakuKbn.PhysicsName);
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
        physicsNameList.Add(ent.yoyakuNo.PhysicsName);
        ent.hakkenDay.Value = (int?)sysDates(KeyIntSysDate);
        physicsNameList.Add(ent.hakkenDay.PhysicsName);
        ent.hakkenTantosyaCd.Value = UserInfoManagement.userId;
        physicsNameList.Add(ent.hakkenTantosyaCd.PhysicsName);
        ent.hakkenEigyosyoCd.Value = UserInfoManagement.eigyosyoCd;
        physicsNameList.Add(ent.hakkenEigyosyoCd.PhysicsName);
        // 定期　または　企画かつ内金発券でない場合

        ent.hakkenNaiyo.Value = CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin); // 発券内容(全金発券
        physicsNameList.Add(ent.hakkenNaiyo.PhysicsName);
        ent.nyuukinSituationKbn.Value = CommonHakken.convertEnumToString(FixedCdYoyaku.NyuukinSituationKbn.NyuukinZumi); // 入金状況区分（入金済み
        physicsNameList.Add(ent.nyuukinSituationKbn.PhysicsName);
        decimal dHakkenKingaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("HAKKEN_KINGAKU") ?? 0;
        int hakkenKingaku = Convert.ToInt32(dHakkenKingaku);
        // 入金額総計 = 既入金額（入金額総計） + 請求 - 発券金額 - 予約センター振込 - オンライン決済
        ent.nyukingakuSokei.Value = this.PreNyukinGaku
                                  + this.SeikyuTotal
                                  - hakkenKingaku
                                  - this.YoyakuCenterNyuukin
                                  - this.OnlineCredit;
        physicsNameList.Add(ent.nyukingakuSokei.PhysicsName);
        decimal dSeikiCharge = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("SEIKI_CHARGE_ALL_GAKU") ?? 0;
        decimal dMaebaraiKei = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("ADD_CHARGE_MAEBARAI_KEI") ?? 0;
        int intSeikiCharge = Convert.ToInt32(dSeikiCharge);
        int intMaebaraiKei = Convert.ToInt32(dMaebaraiKei);
        ent.hakkenKingaku.Value = intSeikiCharge + intMaebaraiKei; // 発券金額 = 正規料金総額 + 追加料金前払計
        physicsNameList.Add(ent.hakkenKingaku.PhysicsName);
        ent.waribikiAllGaku.Value = this.WaribikiTotal; // 割引総額
        physicsNameList.Add(ent.waribikiAllGaku.PhysicsName);
        int intGousya = 0;
        int.TryParse(this.txtGousya.Text.Trim, ref intGousya);
        ent.oldGousya.Value = intGousya; // 旧号車
        physicsNameList.Add(ent.oldGousya.PhysicsName);
        ent.oldZaseki.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("ZASEKI") ?? ""; // 旧座席
        physicsNameList.Add(ent.oldZaseki.PhysicsName);

        string seisanHoho = this._yoyakuInfoBasicTable.Rows(0).Field<string>("SEISAN_HOHO") ?? "";
        if (this.IsKikaku && !seisanHoho.Equals(CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.agt)) && DateTime.Now < this.ucoSyuptDay.Value)
            // 企画　かつ　精算方法が代理店でない　かつ　出発日が過去　の場合
            ent.seisanHoho.Value = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.eigyosyo);

        ent.zasekiChangeUmu.Value = "";
        physicsNameList.Add(ent.zasekiChangeUmu.PhysicsName);
        ent.cancelRyouKei.Value = this.Cancel;
        physicsNameList.Add(ent.cancelRyouKei.PhysicsName);
        ent.toriatukaiFeeUriage.Value = 0;
        physicsNameList.Add(ent.toriatukaiFeeUriage.PhysicsName);
        ent.toriatukaiFeeCancel.Value = 0;
        physicsNameList.Add(ent.toriatukaiFeeCancel.PhysicsName);
        int tasyaKennoKingaku = 0; // 他社券
        int sonotaNyuukinHenkin = 0; // その他
        // 集計した精算項目から、船車券とその他の金額を取り出し
        if (this._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)) || this._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)))
        {
            if (this._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)))
                tasyaKennoKingaku = this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)];

            if (this._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)))
                tasyaKennoKingaku = tasyaKennoKingaku + this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)];
        }
        else if (this._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)))
            sonotaNyuukinHenkin = this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)];
        ent.tasyaKennoKingaku.Value = tasyaKennoKingaku;
        physicsNameList.Add(ent.tasyaKennoKingaku.PhysicsName);
        ent.sonotaNyuukinHenkin.Value = sonotaNyuukinHenkin;
        physicsNameList.Add(ent.sonotaNyuukinHenkin.PhysicsName);
        // TODO:座席
        // ent.zaseki.Value =
        // physicsNameList.Add(ent.zaseki.PhysicsName)
        // ent.yoyakuZasekiKbn.vaue    =
        // physicsNameList.Add(ent.yoyakuZasekiKbn.PhysicsName)
        // ent.changeHistoryLastDay.Value=
        // physicsNameList.Add(ent.changeHistoryLastDay.PhysicsName)
        // TODO:変更履歴から
        // ent.changeHistoryLastSeq.Value =
        // physicsNameList.Add(ent.changeHistoryLastSeq.PhysicsName)
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        physicsNameList.Add(ent.systemUpdateDay.PhysicsName);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        physicsNameList.Add(ent.systemUpdatePersonCd.PhysicsName);
        ent.systemUpdatePgmid.Value = PgmId;
        physicsNameList.Add(ent.systemUpdatePgmid.PhysicsName);
        if (this.OnlineCredit == 0)
        {
            // オンラインクレジット決済でない場合
            // 最終入金日更新
            ent.lastNyuukinDay.Value = (int?)sysDates(KeyIntSysDate);
            physicsNameList.Add(ent.lastNyuukinDay.PhysicsName);
        }
        else if (this.OnlineCredit != 0 && this.Oturi != 0)
        {
            // オンラインクレジット決済　かつ　お釣ありの場合
            // 最終返金日更新
            ent.lastHenkinDay.Value = (int?)sysDates(KeyIntSysDate);
            physicsNameList.Add(ent.lastHenkinDay.PhysicsName);
        }

        // ノーショー
        if (this.chkNoShow.Checked == true)
        {
            ent.noShowFlg.Value = "Y";
            physicsNameList.Add(ent.noShowFlg.PhysicsName);
        }

        return ent;
    }

    /// <summary>
    /// 予約情報２エンティティの設定
    /// </summary>
    /// <param name="sysDates"></param>
    /// <returns></returns>
    private YoyakuInfo2Entity createYoyakuInfo2(Hashtable sysDates, bool existsYoyakuInfo2)
    {
        YoyakuInfo2Entity ent = new YoyakuInfo2Entity();

        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
        int syuptDay = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
        string strYear = syuptDay.ToString().PadLeft(8, '0').Substring(0, 4);
        int year = int.Parse(strYear);
        ent.year.Value = year;

        ent.outDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.outTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.outPersonCd.Value = UserInfoManagement.userId;
        ent.outPgmid.Value = PgmId;

        ent.deleteDay.Value = 0;

        ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.updatePersonCd.Value = UserInfoManagement.userId;
        ent.updatePgmid.Value = PgmId;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        if (existsYoyakuInfo2 == false)
        {
            ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.entryPersonCd.Value = UserInfoManagement.userId;
            ent.entryPgmid.Value = PgmId;
            ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
            ent.systemEntryPgmid.Value = PgmId;
        }

        return ent;
    }

    /// <summary>
    /// 予約情報(コース料金）エンティティの設定
    /// </summary>
    /// <returns></returns>
    private YoyakuInfoCrsChargeEntity createYoyakuInfoCrsChargeEntityForHakken(Hashtable sysDates)
    {
        YoyakuInfoCrsChargeEntity ent = new YoyakuInfoCrsChargeEntity();

        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
        ent.cancelPer.Value = 0;
        if (this.Cancel == this.CancelYoyakuInfoBasic)
            ent.cancelRyou.Value = 0;
        else
            // キャンセル料金に変更あればコース料金へ残す
            ent.cancelRyou.Value = this.Cancel;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        return ent;
    }

    /// <summary>
    /// 予約情報(コース料金_料金区分）エンティティの設定
    /// </summary>
    /// <returns></returns>
    private YoyakuInfoCrsChargeChargeKbnEntity createYoyakuInfoCrsChargeChargeKbnEntityForHakken(Hashtable sysDates)
    {
        YoyakuInfoCrsChargeChargeKbnEntity ent = new YoyakuInfoCrsChargeChargeKbnEntity();

        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
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
    /// 予約情報（基本）エンティティの設定　（ノーショウのみ）
    /// </summary>
    private YoyakuInfoBasicEntity createYoyakuInfoBasicEntityForNoShow(ref List<string> physicsNameList, Hashtable sysDates)
    {
        YoyakuInfoBasicEntity ent = new YoyakuInfoBasicEntity();
        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn; // 予約区分
        physicsNameList.Add(ent.yoyakuKbn.PhysicsName);
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
        physicsNameList.Add(ent.yoyakuNo.PhysicsName);

        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        physicsNameList.Add(ent.systemUpdateDay.PhysicsName);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        physicsNameList.Add(ent.systemUpdatePersonCd.PhysicsName);
        ent.systemUpdatePgmid.Value = PgmId;
        physicsNameList.Add(ent.systemUpdatePgmid.PhysicsName);

        // ノーショー
        if (this.chkNoShow.Checked == true)
        {
            ent.noShowFlg.Value = "Y";
            physicsNameList.Add(ent.noShowFlg.PhysicsName);
        }

        return ent;
    }


    /// <summary>
    /// 発券情報エンティティの設定　(発券）
    /// </summary>
    /// <returns></returns>
    private HakkenInfoEntity createHakkenInfoEntityForHakken(Hashtable sysDates)
    {
        HakkenInfoEntity ent = new HakkenInfoEntity();

        ent.eigyosyoKbn.Value = this._newKenNo.Substring(0, 1);
        ent.tickettypeCd.Value = this._newKenNo.Substring(1, 1);
        ent.mokuteki.Value = this._newKenNo.Substring(2, 1);
        ent.issueYearly.Value = int.Parse(this._newKenNo.Substring(3, 2));
        ent.seq1.Value = this._hakkenInfoSeq1;
        ent.seq2.Value = this._hakkenInfoSeq2;

        ent.hakkenDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.hakkenTime.Value = (int?)sysDates(KeyIntSysTimeHhMm);
        ent.hakkenTantosyaCd.Value = UserInfoManagement.userId;
        ent.hakkenEigyosyoCd.Value = UserInfoManagement.eigyosyoCd;

        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

        // 全金
        ent.hakkenNaiyo.Value = CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin);

        decimal dMaebaraiKei = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("ADD_CHARGE_MAEBARAI_KEI") ?? 0;
        int intMaebaraiKei = Convert.ToInt32(dMaebaraiKei);
        ent.addChargeMaebaraiKei.Value = intMaebaraiKei; // 追加料金前払計
        ent.cancelRyou.Value = this.Cancel; // キャンセル料
        ent.toriatukaiFeeUriage.Value = Convert.ToInt32(this._toriatukaiFeeUriage); // 取扱手数料/ 売上
        ent.toriatukaiFeeCancel.Value = Convert.ToInt32(this._toriatukaiFeeCancel); // 取扱手数料/ ｷｬﾝｾﾙ

        // 正規料金 + 追加料金前払計
        decimal dSeikiCharge = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("SEIKI_CHARGE_ALL_GAKU") ?? 0;
        int intSeikiCharge = Convert.ToInt32(dSeikiCharge);
        int charge = intSeikiCharge + intMaebaraiKei;

        ent.hakkenKingaku.Value = charge;             // 発券金額
        ent.uriageKingaku.Value = charge;             // 売上金額
        ent.waribikiKingaku.Value = this.WaribikiTotal; // 割引金額

        bool existsGenkin = this._undistributedKinBySeisanKoumoku.Keys
                                     .Any(key => key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)));

        bool existsSensyaken = this._undistributedKinBySeisanKoumoku.Keys
                                     .Any(key => key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)));

        bool existsCredit = this._undistributedKinBySeisanKoumoku.Keys
                                     .Any(key => key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card))
                                                        || key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit)));

        bool existsSonota = this._undistributedKinBySeisanKoumoku.Keys
                                     .Any(key => key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)));


        if (existsGenkin)
            // 現金精算がある場合
            this._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoGenkin;
        else
        {
            // 現金精算がない場合
            if (existsSensyaken)
                this._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoSensyaKen;
            if (existsCredit)
                this._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoCredit;
            if (existsSonota)
                this._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoSonota;
        }

        ent.gousya.Value = this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA") ?? 0;
        ent.zaseki24Baito.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("ZASEKI") ?? "";
        ent.oldGousya.Value = this._yoyakuInfoBasicTable.Rows(0).Field<short?>("GOUSYA") ?? 0;
        ent.oldZaseki.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("ZASEKI") ?? "";
        ent.uriageKbn.Value = "";

        ent.deleteDay.Value = 0;
        ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.entryPersonCd.Value = UserInfoManagement.userId;
        ent.entryPgmid.Value = PgmId;
        ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.updatePersonCd.Value = UserInfoManagement.userId;
        ent.updatePgmid.Value = PgmId;
        ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
        ent.systemEntryPgmid.Value = PgmId;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        return ent;
    }

    /// <summary>
    /// 発券情報(料金）エンティティの設定　(発券）
    /// </summary>
    /// <returns></returns>
    private List<HakkenInfoChargeEntity> createHakkenInfoChargeEntityListForHakken(Hashtable sysDates)
    {

        // 人員SEQの採番用
        Dictionary<string, int> jininSeq = new Dictionary<string, int>();

        List<HakkenInfoChargeEntity> list = new List<HakkenInfoChargeEntity>();
        // 割引コード別に集計した行から値を取り出し
        foreach (DataRow waribikiCdRow in this._infoByWaribikiCdJininCd.AsEnumerable())
        {

            // 部屋タイプ1～5のループ
            for (var roomType = CommonHakken.One1R; roomType <= CommonHakken.FiveIjyou1R; roomType++)
            {
                int ninzu = waribikiCdRow.Field<int?>($"WARIBIKI_NINZU_{roomType}") ?? 0;

                // 0ならエンティティ作らない
                if (ninzu == 0)
                    continue;

                // 割引コード
                string waribikiCd = waribikiCdRow.Field<string>("WARIBIKI_CD") ?? "";
                // 割引種別
                string waribikiType = waribikiCdRow.Field<string>("WARIBIKI_TYPE_KBN") ?? "";
                if (waribikiType.Equals(CommonHakken.WaribikiTypeSeikiCharge))
                    waribikiType = "";// (割引種別：正規料金はダミー値なので初期化)
                // 割引金額
                int waribiki = waribikiCdRow.Field<int?>($"WARIBIKI_TANKA_{roomType}") ?? 0;

                // 料金カラムセット
                string chargeColumns = waribikiCdRow.Field<string>(CommonHakken.ChargeColumnsSet) ?? "";

                // 人員SEQ を採番
                jininSeq = CommonHakken.numberingJininSeq(jininSeq, chargeColumns);

                // 区分No
                string strKbnNo = "";
                int kbnNo = 0;
                // 料金区分
                string chargeKbn = "";
                // 料金区分（人員）コード
                string jininCd = "";

                // 料金区分のカラムを分解
                CommonHakken.separateChargeColumnsSet(chargeColumns, kbnNo, chargeKbn, jininCd, strKbnNo);

                // ■正規料金から登録値を取り出し
                // 料金
                int seikiCharge = CommonHakken.getSeikiCharge(jininCd, roomType, this._yoyakuInfoCrsChargeChargeKbn);
                // 運賃
                int carriage = CommonHakken.getCarriage(jininCd, this._yoyakuInfoCrsChargeChargeKbn);

                // エンティティへ値を設定
                HakkenInfoChargeEntity ent = new HakkenInfoChargeEntity();

                ent.eigyosyoKbn.Value = this._newKenNo.Substring(0, 1);
                ent.tickettypeCd.Value = this._newKenNo.Substring(1, 1);
                ent.mokuteki.Value = this._newKenNo.Substring(2, 1);
                ent.issueYearly.Value = int.Parse(this._newKenNo.Substring(3, 2));
                ent.seq1.Value = this._hakkenInfoSeq1;
                ent.seq2.Value = this._hakkenInfoSeq2;

                ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
                ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

                ent.kbnNo.Value = kbnNo;
                ent.chargeKbnJininCd.Value = jininCd;
                ent.jininSeq.Value = jininSeq[chargeColumns];
                ent.chargeKbn.Value = chargeKbn;

                ent.carriage1.Value = carriage;
                ent.charge.Value = seikiCharge;
                ent.ninzu.Value = ninzu;

                ent.waribikiCd.Value = waribikiCd;
                ent.waribikiTypeKbn.Value = waribikiType;
                ent.waribikiKingaku.Value = waribiki;

                ent.deleteDay.Value = 0;
                ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
                ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
                ent.systemEntryPgmid.Value = PgmId;
                ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
                ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
                ent.systemUpdatePgmid.Value = PgmId;

                list.Add(ent);
            } // ここまで部屋タイプ1～5のループ
        } // ここまで割引コード、人員コード単位のループ

        return list;
    }

    /// <summary>
    /// 精算情報の按分
    /// </summary>
    private SeisanInfoListSetEntity distributeSeisanInfo(Hashtable sysDates)
    {
        // SEQの取得（存在しない場合、１とする
        int seq = CommonHakken.createSeq(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);

        // パラメータ用の精算情報エンティティ
        SeisanInfoEntity prmSeisanInfo = new SeisanInfoEntity();
        prmSeisanInfo.seq.Value = seq;
        string crsKind = _yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KIND");
        prmSeisanInfo.seisanKbn.Value = CommonHakken.getSeisanKbn(crsKind, this.IsTeiki);

        // 精算情報セットエンティティ
        SeisanInfoListSetEntity seisanInfoSetEntity = new SeisanInfoListSetEntity();

        bool isFinishedDistibute = false;
        // 各割引種別から各精算項目への按分
        // 割引がある場合、按分を実行
        if (this._existsWaribiki)
            // 割引種別が一般でない按分
            isFinishedDistibute = this.distributeSeisanInfoNOTGeneralWaribikiType(prmSeisanInfo, sysDates, ref seisanInfoSetEntity);

        // 割引種別が一般、正規料金の按分
        if (isFinishedDistibute == false)
            this.distributeSeisanInfoGeneralWaribikiType(prmSeisanInfo, sysDates, ref seisanInfoSetEntity);

        return seisanInfoSetEntity;
    }

    /// <summary>
    /// 割引種別が一般でない精算情報の按分
    /// </summary>
    private bool distributeSeisanInfoNOTGeneralWaribikiType(SeisanInfoEntity prmSeisanInfo, Hashtable sysDates, ref SeisanInfoListSetEntity seisanInfoSetEntity
)
    {
        // 按分終了のフラグ
        bool isFinishedDistribute = false;

        // 按分後の精算内訳を格納するDictionary（キー：精算項目、値：精算内訳の金額）
        Dictionary<string, int> seisanUtiwakeNotIppan = new Dictionary<string, int>();

        // 割引種別単位の請求額ごとに按分を行う
        var seikyuByWaribikiType = new Dictionary<string, int>(this._seikyuByWaribikiType);
        foreach (var item in seikyuByWaribikiType.AsEnumerable())
        {
            // 精算内訳を初期化
            seisanUtiwakeNotIppan.Clear();

            // 割引種別
            string waribikiType = item.Key;
            prmSeisanInfo.waribikiType.Value = waribikiType;
            // 未按分金（割引種別単位の請求）
            int unDistributedKin = item.Value;

            // 割引種別が一般の場合、次の割引種別へ
            if (waribikiType.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) || waribikiType.Equals(CommonHakken.WaribikiTypeSeikiCharge))
                continue;

            // ■予約者が全員、同割引種別かつ一般割引でない場合、
            // 按分を行わず請求額そのままエンティティを作成
            bool isAllThisWaribikiType = (this._ninzuByWaribikiType[waribikiType] == this._totalYoyakuNinzu);
            if (isAllThisWaribikiType)
            {
                this.distributeSeisanInfoAllNOTGeneralWaribikiType(prmSeisanInfo, sysDates, ref seisanInfoSetEntity);

                // 按分不要 
                isFinishedDistribute = true;
                return isFinishedDistribute;
            }

            // ■その他の場合
            // ①現金へ按分
            if (unDistributedKin > 0 && this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) && this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)] > 0)

                // 按分処理
                this.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin), ref unDistributedKin, ref seisanUtiwakeNotIppan);

            // ②現金、その他、取扱手数料以外の按分
            Dictionary<string, int> undistributedKinBySeisanKoumoku = new Dictionary<string, int>(this._undistributedKinBySeisanKoumoku);
            foreach (var seisanKoumoku in undistributedKinBySeisanKoumoku)
            {
                // 現金、その他、取扱手数料の場合、次の精算項目へ
                if (seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) || seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) || seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
                    continue;

                if (unDistributedKin > 0 && this._undistributedKinBySeisanKoumoku[seisanKoumoku.Key] > 0)

                    // 按分処理
                    this.distribute(seisanKoumoku.Key, ref unDistributedKin, ref seisanUtiwakeNotIppan);
            } // ここまで精算項目ごとのループ

            // ③その他（精算項目）へ按分
            if (unDistributedKin > 0 && this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) && this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)] > 0)

                // 按分処理
                this.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota), ref unDistributedKin, ref seisanUtiwakeNotIppan);

            // 定期　または　企画かつ内金発券でない場合
            // ④取扱手数料の按分
            if (unDistributedKin > 0 && this.ToriatukaiFee > 0)
            {
                if (!this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
                    this._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), this.ToriatukaiFee);

                this.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), ref unDistributedKin, ref seisanUtiwakeNotIppan);
            }

            // 割引額を精算内訳に追加
            seisanUtiwakeNotIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku), this._waribikiByWaribikiType[waribikiType]);

            // 精算情報セットエンティティの作成
            this.setSeisanInfoSetEntity(ref seisanInfoSetEntity, prmSeisanInfo, seisanUtiwakeNotIppan, sysDates);

            // 精算情報のSEQをインクリメント
            prmSeisanInfo.seq.Value += 1;
        } // ここまで割引種別ごと（精算項目1レコード単位のループ

        // 按分終了
        return isFinishedDistribute;
    }

    /// <summary>
    /// 全員、割引種別が一般でない精算情報の按分
    /// </summary>
    private void distributeSeisanInfoAllNOTGeneralWaribikiType(SeisanInfoEntity prmSeisanInfo, Hashtable sysDates, ref SeisanInfoListSetEntity seisanInfoSetEntity
)
    {
        // 取扱手数料を精算内訳に追加
        if (this.ToriatukaiFee > 0)
        {
            if (!this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
                this._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), this.ToriatukaiFee);

            this._seikyuByWaribikiType.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)]);
        }

        // キャンセル料を精算内訳に追加
        if (this.Cancel > 0)
        {
            if (!this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo)))
                this._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo), this.Cancel);
        }

        // 割引金額を精算内訳に追加
        if (this.WaribikiTotal > 0)
        {
            if (!this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku)))
                this._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku), this.WaribikiTotal);
        }

        // 精算情報エンティティを作成
        this.setSeisanInfoSetEntity(ref seisanInfoSetEntity, prmSeisanInfo, this._undistributedKinBySeisanKoumoku, sysDates);
    }

    /// <summary>
    /// 割引種別が一般の精算情報の按分
    /// </summary>
    private void distributeSeisanInfoGeneralWaribikiType(SeisanInfoEntity prmSeisanInfo, Hashtable sysDates, ref SeisanInfoListSetEntity seisanInfoSetEntity
)
    {

        // 精算項目ごとの未按分金から全て、精算内訳を作成する
        var undistributedKinBySeisanKoumoku = this._undistributedKinBySeisanKoumoku
                                              .Where(undisributedKin => undisributedKin.Value != 0);

        // 精算内訳を格納するDictionary（キー：精算項目、値：精算内訳の金額）
        Dictionary<string, int> seisanUtiwakeIppan = new Dictionary<string, int>();
        foreach (var seisanKoumoku in undistributedKinBySeisanKoumoku)
            seisanUtiwakeIppan.Add(seisanKoumoku.Key, seisanKoumoku.Value); // ここまで精算項目ごとのループ

        // 予約センター返金があれば、精算内訳に追加
        if (this.YoyakuCenterHenkin != 0)
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.hurikomi_yoyaku_center_modosi), this.YoyakuCenterHenkin);

        // 取扱手数料を精算内訳に追加
        if (this.ToriatukaiFee > 0)
        {
            if (!this._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
                this._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), this.ToriatukaiFee);

            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), this._undistributedKinBySeisanKoumoku[CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)]);
        }

        // 一般割引の存在確認
        int seikiChargeByWaribikiTypeGeneral = 0;
        int waribikiByWaribikiTypeGeneral = 0;
        if (_seikyuByWaribikiType.ContainsKey(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)))
        {
            // 割引種別を一般割引へ変更
            prmSeisanInfo.waribikiType.Value = CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount);
            // 一般割引の正規料金、割引額
            seikiChargeByWaribikiTypeGeneral = this._seikiChargeByWaribikiType[CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)];
            waribikiByWaribikiTypeGeneral = this._waribikiByWaribikiType[CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)];
        }

        // キャンセル料を精算内訳に追加
        // 定期：     払戻手数料、企画かつ内金発券でない：キャンセル料
        if (this.IsTeiki)
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.haraimodosi_tesuryo), this.Cancel);
        else if (this.IsKikaku)
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo), this.Cancel);

        // 定期　または　企画かつ内金発券でない場合、
        // 割引額を精算内訳に追加
        if (waribikiByWaribikiTypeGeneral > 0)
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku), waribikiByWaribikiTypeGeneral);

        // 精算情報セットエンティティの作成
        this.setSeisanInfoSetEntity(ref seisanInfoSetEntity, prmSeisanInfo, seisanUtiwakeIppan, sysDates);
    }

    /// <summary>
    /// 按分処理
    /// </summary>
    /// <param name="seisanKoumokuA">精算項目A</param>
    /// <param name="unDistributedKin">未按分金</param>
    /// <param name="seisanUtiwake">精算内訳（キー：精算項目コード、値：金額</param>
    private void distribute(string seisanKoumokuA, ref int unDistributedKin, ref Dictionary<string, int> seisanUtiwake)
    {

        // 精算内訳のキーへ「精算項目A」を追加
        if (!seisanUtiwake.ContainsKey(seisanKoumokuA))
            seisanUtiwake.Add(seisanKoumokuA, 0);

        if (unDistributedKin <= _undistributedKinBySeisanKoumoku[seisanKoumokuA])
        {
            // 未按分金 <= 「精算項目A」の入金額の場合

            // 未按分金を全て按分し、内訳を生成　※複数の精算情報（割引種別）へ「精算項目A」の按分はまたがる
            this._undistributedKinBySeisanKoumoku[seisanKoumokuA] -= unDistributedKin;

            seisanUtiwake[seisanKoumokuA] += unDistributedKin;
            unDistributedKin = 0;
        }
        else
        {
            // 未按分金 > 「精算項目A」の入金額の場合

            // 「精算項目A」の入金分を按分し、内訳を生成　※当割引種別のみへ「精算項目A」を按分
            seisanUtiwake[seisanKoumokuA] += this._undistributedKinBySeisanKoumoku[seisanKoumokuA];

            unDistributedKin -= this._undistributedKinBySeisanKoumoku[seisanKoumokuA];
            this._undistributedKinBySeisanKoumoku[seisanKoumokuA] = 0;
        }
    }

    /// <summary>
    /// 精算情報セットエンティティの設定
    /// </summary>
    /// <param name="seisanUtiwake"></param>
    /// <param name="sysDates"></param>
    private void setSeisanInfoSetEntity(ref SeisanInfoListSetEntity seisanInfoSetEntity, SeisanInfoEntity prmSeisanInfo, Dictionary<string, int> seisanUtiwake, Hashtable sysDates)
    {
        if (seisanInfoSetEntity.SeisanInfoEntityList == null)
            seisanInfoSetEntity.SeisanInfoEntityList = new List<SeisanInfoEntity>();
        if (seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList == null)
            seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList = new List<SeisanInfoSankaNinzuEntity>();
        if (seisanInfoSetEntity.SeisanInfoUtiwakeEntityList == null)
            seisanInfoSetEntity.SeisanInfoUtiwakeEntityList = new List<SeisanInfoUtiwakeEntity>();

        // 精算情報エンティティリストの設定
        prmSeisanInfo.seisanInfoSeq.Value = CommonHakken.createSeisanInfoSeq(); // PK採番
        setSeisanInfoEntityList(ref seisanInfoSetEntity.SeisanInfoEntityList, prmSeisanInfo, sysDates);

        // 精算情報（参加人数）エンティティリストの設定
        setSeisanInfoSankaNinzuEntity(ref seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList, prmSeisanInfo, sysDates);

        // 精算情報内訳エンティティリストの設定
        setSeisanInfoUtiwakeEntity(ref seisanInfoSetEntity.SeisanInfoUtiwakeEntityList, prmSeisanInfo, seisanUtiwake, sysDates);
    }

    /// <summary>
    /// 精算情報エンティティの作成
    /// </summary>
    /// <param name="sysDates"></param>
    private void setSeisanInfoEntityList(ref List<SeisanInfoEntity> list, SeisanInfoEntity prmSeisanInfo, Hashtable sysDates)
    {



        // 割引種別単位でレコードを作成
        SeisanInfoEntity ent = new SeisanInfoEntity();
        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

        ent.kenno.Value = this._newKenNo;
        ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value;
        ent.seq.Value = prmSeisanInfo.seq.Value;

        ent.createDay.Value = (int)sysDates(KeyIntSysDate);
        ent.createTime.Value = (int)sysDates(KeyIntSysTimeHhMmSs);

        ent.companyCd.Value = UserInfoManagement.companyCd;
        ent.eigyosyoCd.Value = UserInfoManagement.eigyosyoCd;
        ent.tantosyaCd.Value = UserInfoManagement.userId;
        ent.signonTime.Value = CommonHakken.convertDateToIntTime(UserInfoManagement.signonDate);

        ent.crsCd.Value = this.txtCourseCd.Text.Trim();

        // 出発日
        ent.syuptDay.Value = CommonHakken.convertDateToInt(this.ucoSyuptDay.Value);
        // 号車
        int intGousya = 0;
        int.TryParse(this.txtGousya.Text.Trim(), ref intGousya);
        ent.gousya.Value = intGousya;

        ent.tokuteiDayFlg.Value = "";
        ent.teikiCrsKbn.Value = "";
        ent.crsKind.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KIND") ?? "";
        ent.crsKbn1.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KBN_1") ?? "";
        ent.crsKbn2.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("CRS_KBN_2") ?? "";
        ent.accessCd.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("ACCESS_CD") ?? "";

        ent.uriageKbn.Value = "";
        ent.hakkenKbn.Value = "";

        string agentCd = this._yoyakuInfoBasicTable.Rows(0).Field<string>("AGENT_CD") ?? "";
        ent.agentCd.Value = agentCd;
        ent.seatKbn.Value = this._yoyakuInfoBasicTable.Rows(0).Field<string>("YOYAKU_ZASEKI_KBN") ?? "";
        ent.tasyaKenno.Value = "";
        ent.tasyaKennoIssueDay.Value = 0;
        ent.tickettypeCd.Value = this.IsTeiki ? CommonHakken.TicketTypeCdTeiki : CommonHakken.TicketTypeCdKikaku;

        ent.otherUriageSyohinKbn.Value = "";
        ent.otherUriageSyohinCd1.Value = "";
        ent.otherUriageSyohinCd2.Value = "";
        ent.otherUriageSyohinQuantity.Value = 0;
        ent.otherUriageSyohinTanka.Value = 0;
        ent.otherUriageSyohinBiko.Value = "";

        ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value; // 精算区分

        // クーポン売上、払戻
        int couponRefund = prmSeisanInfo.couponRefund.Value ?? 0;
        int couponUriage = 0;

        if (prmSeisanInfo.waribikiType.Value.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) || string.IsNullOrEmpty(prmSeisanInfo.waribikiType.Value))
        {
            // 正規料金、一般割引の場合、合算
            if (this._seikiChargeByWaribikiType.Keys.Contains(CommonHakken.WaribikiTypeSeikiCharge))
                couponUriage += this._seikiChargeByWaribikiType[CommonHakken.WaribikiTypeSeikiCharge];
            if (this._seikiChargeByWaribikiType.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)))
                couponUriage += this._seikiChargeByWaribikiType[CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)];
        }
        else
            couponUriage = this._seikiChargeByWaribikiType[prmSeisanInfo.waribikiType.Value];

        // 発券金額
        decimal dHakkenKingaku = this._yoyakuInfoBasicTable.Rows(0).Field<decimal?>("HAKKEN_KINGAKU") ?? 0;
        int hakkenKingaku = Convert.ToInt32(dHakkenKingaku);

        ent.couponRefund.Value = couponRefund;
        ent.couponUriage.Value = couponUriage;

        // 割引種別
        if (prmSeisanInfo.waribikiType.Value.Equals(CommonHakken.WaribikiTypeSeikiCharge))
            // 正規料金のダミー値書換え
            ent.waribikiType.Value = "";
        else
            ent.waribikiType.Value = prmSeisanInfo.waribikiType.Value;

        // ノーサイン区分=オンラインクレジット決済
        if (this.OnlineCredit > 0)
            ent.nosignKbn.Value = "Y";

        // AGT請求対象フラグ
        int atoSeisanKbn = 0;
        if (this._agentMaster != null)
        {
            // 後払精算区分の確認
            string teikiKikaku = this.IsTeiki ? "TEIKI" : "KIKAKU";
            atoSeisanKbn = this._agentMaster.Rows(0).Field<short?>($"{teikiKikaku}_ATO_SEISAN_KBN") ?? 0;
        }
        if (atoSeisanKbn == Convert.ToInt32(FixedCd.AtoSeisanKbnType.HakkenSeisan))
            ent.agtSeikyuTaisyoFlg.Value = Convert.ToInt32(FixedCd.AtoSeisanKbnType.HakkenSeisan);
        else if (atoSeisanKbn == Convert.ToInt32(FixedCd.AtoSeisanKbnType.ChakukenSeisan))
            ent.agtSeikyuTaisyoFlg.Value = Convert.ToInt32(FixedCd.AtoSeisanKbnType.ChakukenSeisan);

        ent.deleteDay.Value = 0;
        ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.entryPersonCd.Value = UserInfoManagement.userId;
        ent.entryPgmid.Value = PgmId;
        ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.updatePersonCd.Value = UserInfoManagement.userId;
        ent.updatePgmid.Value = PgmId;
        ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
        ent.systemEntryPgmid.Value = PgmId;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        list.Add(ent);
    }

    /// <summary>
    /// 精算情報（参加人数）エンティティの作成
    /// </summary>
    private void setSeisanInfoSankaNinzuEntity(ref List<SeisanInfoSankaNinzuEntity> list, SeisanInfoEntity prmSeisanInfo, Hashtable sysDates)
    {
        string waribikiType = prmSeisanInfo.waribikiType.Value;

        EnumerableRowCollection<DataRow> rows = null;

        // 割引コードごとの集計から該当割引種別のみ抽出
        if (string.IsNullOrEmpty(waribikiType) || waribikiType.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)))
        {
            // 正規料金または割引ありの場合
            rows = this._infoByWaribikiCdJininCd.AsEnumerable()
                  .Where(row =>
                  {
                      return row.Field<string>("WARIBIKI_TYPE_KBN") ?? "".Equals(CommonHakken.WaribikiTypeSeikiCharge)
 || row.Field<string>("WARIBIKI_TYPE_KBN") ?? "".Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount));
                  });
        }
        else
            rows = this._infoByWaribikiCdJininCd.AsEnumerable()
              .Where(row => row.Field<string>("WARIBIKI_TYPE_KBN") ?? "".Equals(waribikiType));

        // キー：料金カラムセット（料金区分、料金区分（人員）コード、値：人数
        Dictionary<string, int> ninzuCnter = new Dictionary<string, int>();

        // 同一料金区分、料金区分（人員）を集計
        foreach (DataRow row in rows)
        {
            string chargeColumns = row.Field<string>(CommonHakken.ChargeColumnsSet) ?? "";
            int ninzu = row.Field<int?>("NINZU") ?? 0;

            // 一般割引、正規料金のPK競合対策
            if (!ninzuCnter.Keys.Contains(chargeColumns))
                ninzuCnter.Add(chargeColumns, ninzu);
            else
                ninzuCnter[chargeColumns] += ninzu;
        }

        // 集計した人数から値を取り出し
        foreach (var item in ninzuCnter)
        {
            // 精算情報（参加人数）エンティティを生成
            SeisanInfoSankaNinzuEntity ent = new SeisanInfoSankaNinzuEntity();

            ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
            ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;

            string chargeColumns = item.Key;
            int ninzu = item.Value;


            // 区分No
            int kbnNo = 0;
            // 料金区分
            string chargeKbn = "";
            // 料金区分（人員）コード
            string jininCd = "";
            CommonHakken.separateChargeColumnsSet(chargeColumns, kbnNo, chargeKbn, jininCd);

            ent.kbnNo.Value = kbnNo;
            ent.chargeKbn.Value = chargeKbn;
            ent.chargeKbnJininCd.Value = jininCd;

            ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value;
            ent.seq.Value = prmSeisanInfo.seq.Value;

            ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value;
            ent.kenno.Value = this._newKenNo;

            ent.sankaNinzu.Value = ninzuCnter[chargeColumns];

            ent.deleteDay.Value = 0;
            ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.entryPersonCd.Value = UserInfoManagement.userId;
            ent.entryPgmid.Value = PgmId;
            ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.updatePersonCd.Value = UserInfoManagement.userId;
            ent.updatePgmid.Value = PgmId;
            ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
            ent.systemEntryPgmid.Value = PgmId;
            ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
            ent.systemUpdatePgmid.Value = PgmId;

            list.Add(ent);
        } // ここまでパラメータのDictionaryのループ
    }

    /// <summary>
    /// 精算情報内訳エンティティの生成
    /// </summary>
    private void setSeisanInfoUtiwakeEntity(ref List<SeisanInfoUtiwakeEntity> list, SeisanInfoEntity prmSeisanInfo, Dictionary<string, int> seisanUtiwake, Hashtable sysDates)
    {

        // パラメータより値を取り出し
        foreach (var item in seisanUtiwake)
        {
            string prmCd = item.Key;
            int prmKingaku = item.Value;

            // 金額が0円ならエンティティを生成しない
            if (prmKingaku == 0)
                continue;

            bool isListAddedFromGrd = false;
            // グリッド上の項目からの精算情報内訳エンティティの作成
            isListAddedFromGrd = this.setSeisanInfoUtiwakeEntityWithGrd(ref list, prmSeisanInfo, seisanUtiwake, sysDates, item);

            // グリッドのループを抜けてもエンティティを追加できていない場合
            if (isListAddedFromGrd == false)
                // グリッドにない項目からの精算情報内訳エンティティの作成
                this.setSeisanInfoUtiwakeEntityWithoutGrd(ref list, prmSeisanInfo, seisanUtiwake, sysDates, item);
        } // ここまでパラメータのDictionaryのループ
    }

    /// <summary>
    /// 精算情報内訳エンティティの生成（グリッドに存在する場合
    /// </summary>
    /// <returns></returns>
    private bool setSeisanInfoUtiwakeEntityWithGrd(ref List<SeisanInfoUtiwakeEntity> list, SeisanInfoEntity prmSeisanInfo, Dictionary<string, int> seisanUtiwake, Hashtable sysDates, KeyValuePair<string, int> item)
    {
        string prmCd = item.Key;
        int prmKingaku = item.Value;

        var isListAdded = false;
        foreach (Row row in this.grdSeisan.Rows)
        {
            // ヘッダー行なら次の行へ
            if (row.Index == 0)
                continue;

            string grdCd = row.Item("SEISAN_KOUMOKU_CD") as string ?? "";

            // 空セル選択時は、次の行へ
            if (grdCd.Equals(CommonHakken.EmptyCellKey))
                continue;

            // パラメータの情報でなければ、次の行へ
            if (!prmCd.Equals(grdCd))
                continue;

            // 同精算項目コードのエンティティが作成済みなら、次の行へ
            bool isCreatedEntity = list.Any(entInList => entInList.seisanKoumokuCd.Value.Equals(prmCd));
            if (isCreatedEntity)
                continue;

            // 精算情報内訳のエンティティをグリッドを基に生成
            SeisanInfoUtiwakeEntity ent = new SeisanInfoUtiwakeEntity();
            ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
            ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

            ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value;
            ent.seq.Value = prmSeisanInfo.seq.Value;

            ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value;
            ent.kenno.Value = this._newKenNo;

            ent.seisanKoumokuCd.Value = prmCd;
            if (prmKingaku >= 0)
                ent.kingaku.Value = prmKingaku; // 引数で渡されたDictionaryの値
            else
                ent.kingaku.Value = -prmKingaku;// （貸方）

            ent.hurikomiKbn.Value = row.Item("HURIKOMI_KBN") as string ?? "";
            ent.issueCompanyCd.Value = row.Item("ISSUE_COMPANY_CD") as string ?? "";
            ent.biko.Value = row.Item("BIKO") as string ?? "";

            ent.deleteDay.Value = 0;
            ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.entryPersonCd.Value = UserInfoManagement.userId;
            ent.entryPgmid.Value = PgmId;
            ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
            ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
            ent.updatePersonCd.Value = UserInfoManagement.userId;
            ent.updatePgmid.Value = PgmId;
            ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
            ent.systemEntryPgmid.Value = PgmId;
            ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
            ent.systemUpdatePgmid.Value = PgmId;

            list.Add(ent);
            isListAdded = true;
        }

        return isListAdded;
    }

    /// <summary>
    /// 精算情報内訳エンティティの生成（グリッドに存在しない場合
    /// </summary>
    /// <returns></returns>
    private List<SeisanInfoUtiwakeEntity> setSeisanInfoUtiwakeEntityWithoutGrd(ref List<SeisanInfoUtiwakeEntity> list, SeisanInfoEntity prmSeisanInfo, Dictionary<string, int> seisanUtiwake, Hashtable sysDates, KeyValuePair<string, int> item)
    {
        string prmCd = item.Key;
        int prmKingaku = item.Value;

        SeisanInfoUtiwakeEntity ent = new SeisanInfoUtiwakeEntity();
        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

        ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value;
        ent.seq.Value = prmSeisanInfo.seq.Value;

        ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value;
        ent.kenno.Value = this._newKenNo;

        ent.seisanKoumokuCd.Value = prmCd;
        if (prmKingaku >= 0)
            ent.kingaku.Value = prmKingaku; // 引数で渡されたDictionaryの値
        else
            ent.kingaku.Value = -prmKingaku;// （貸方）

        ent.hurikomiKbn.Value = "";
        ent.issueCompanyCd.Value = "";
        ent.biko.Value = "";

        ent.deleteDay.Value = 0;
        ent.entryDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.entryTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.entryPersonCd.Value = UserInfoManagement.userId;
        ent.entryPgmid.Value = PgmId;
        ent.updateDay.Value = (int?)sysDates(KeyIntSysDate);
        ent.updateTime.Value = (int?)sysDates(KeyIntSysTimeHhMmSs);
        ent.updatePersonCd.Value = UserInfoManagement.userId;
        ent.updatePgmid.Value = PgmId;
        ent.systemEntryDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
        ent.systemEntryPgmid.Value = PgmId;
        ent.systemUpdateDay.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
        ent.systemUpdatePgmid.Value = PgmId;

        list.Add(ent);

        return list;
    }

    /// <summary>
    /// 入返金情報エンティティの作成
    /// </summary>
    /// <param name="isVoid"></param>
    /// <param name="sysDates"></param>
    /// <returns></returns>
    private NyuukinInfoEntity createNyuukinInfoEntity(bool isVoid, Hashtable sysDates)
    {
        NyuukinInfoEntity ent = new NyuukinInfoEntity();

        ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

        ent.hakkenFlg.Value = isVoid ? "" : "Y";

        ent.updateDate.Value = (DateTime)sysDates(KeyDtSysDate);
        ent.updateUserId.Value = UserInfoManagement.userId;
        ent.updateClient.Value = PgmId;

        return ent;
    }

    /// <summary>
    /// 入返金情報エンティティの作成（予約センターあり時）
    /// </summary>
    /// <param name="physicsNameList"></param>
    /// <param name="sysDates"></param>
    /// <returns></returns>
    private NyuukinInfoEntity createNyuukinInfoEntityHenkinAri(ref List<string> physicsNameList, Hashtable sysDates)
    {
        NyuukinInfoEntity entity = new NyuukinInfoEntity();

        // 予約区分
        entity.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
        physicsNameList.Add(entity.yoyakuKbn.PhysicsName);
        // 予約NO
        entity.yoyakuNo.Value = this.ParamData.YoyakuNo;
        physicsNameList.Add(entity.yoyakuNo.PhysicsName);
        // SEQ
        int seq = CommonHakken.createNyuukinInfoSeq(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
        entity.seq.Value = seq;
        physicsNameList.Add(entity.seq.PhysicsName);
        // 年
        int syuptDay = this._yoyakuInfoBasicTable.Rows(0).Field<int?>("SYUPT_DAY") ?? 0;
        string strYear = syuptDay.ToString().PadLeft(8, '0').Substring(0, 4);
        int year = int.Parse(strYear);
        entity.nyuukinYear.Value = year;
        physicsNameList.Add(entity.nyuukinYear.PhysicsName);
        // 会社コード
        entity.companyCd.Value = UserInfoManagement.companyCd;
        physicsNameList.Add(entity.companyCd.PhysicsName);
        // 営業所
        entity.eigyosyoCd.Value = UserInfoManagement.eigyosyoCd;
        physicsNameList.Add(entity.eigyosyoCd.PhysicsName);
        // 入金種別
        entity.nyuukinKind.Value = this.IsTeiki ? CommonHakken.NyuukinKindTeiki : CommonHakken.NyuukinKindKikaku;
        physicsNameList.Add(entity.nyuukinKind.PhysicsName);
        // 発券振込区分
        entity.hakkenHurikomiKbn.Value = CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin;
        physicsNameList.Add(entity.hakkenHurikomiKbn.PhysicsName);
        // 処理日
        entity.processDate.Value = (DateTime)sysDates(KeyDtSysDate);
        physicsNameList.Add(entity.processDate.PhysicsName);
        // 入金額１（振込）
        entity.nyuukinGaku1.Value = this.YoyakuCenterHenkin;
        physicsNameList.Add(entity.nyuukinGaku1.PhysicsName);
        // 入金額２（現金）
        entity.nyuukinGaku2.Value = 0;
        physicsNameList.Add(entity.nyuukinGaku2.PhysicsName);
        // 入金額３（その他）
        entity.nyuukinGaku3.Value = 0;
        physicsNameList.Add(entity.nyuukinGaku3.PhysicsName);
        // 入金額４（振込手数料）
        entity.nyuukinGaku4.Value = 0;
        physicsNameList.Add(entity.nyuukinGaku4.PhysicsName);
        // 入金額５
        entity.nyuukinGaku5.Value = 0;
        physicsNameList.Add(entity.nyuukinGaku5.PhysicsName);
        // 振込区分
        entity.hurikomiKbn.Value = "";
        physicsNameList.Add(entity.hurikomiKbn.PhysicsName);
        // 振込先口座名
        entity.hurikomiSakiKozaName.Value = "";
        physicsNameList.Add(entity.hurikomiSakiKozaName.PhysicsName);
        // 券番 
        entity.kenNo.Value = this._newKenNo;
        physicsNameList.Add(entity.kenNo.PhysicsName);
        // キャンセル区分
        entity.cancelKbn.Value = "";
        physicsNameList.Add(entity.cancelKbn.PhysicsName);
        // 発券フラグ
        entity.hakkenFlg.Value = "Y";
        physicsNameList.Add(entity.hakkenFlg.PhysicsName);
        // 振替フラグ 
        entity.hurikaeFlg.Value = 0;
        physicsNameList.Add(entity.hurikaeFlg.PhysicsName);
        // 連携結果
        entity.renkeiResult.Value = "0";
        physicsNameList.Add(entity.renkeiResult.PhysicsName);
        // 登録日時
        entity.entryDate.Value = (DateTime)sysDates(KeyDtSysDate);
        physicsNameList.Add(entity.entryDate.PhysicsName);
        // 登録ユーザーID
        entity.entryUserId.Value = UserInfoManagement.userId;
        physicsNameList.Add(entity.entryUserId.PhysicsName);
        // 登録クライアント名
        entity.entryClient.Value = PgmId;
        physicsNameList.Add(entity.entryClient.PhysicsName);
        // 更新日時
        entity.updateDate.Value = (DateTime)sysDates(KeyDtSysDate);
        physicsNameList.Add(entity.updateDate.PhysicsName);
        // 更新ユーザーID
        entity.updateUserId.Value = UserInfoManagement.userId;
        physicsNameList.Add(entity.updateUserId.PhysicsName);
        // 更新クライアント名
        entity.updateClient.Value = PgmId;
        physicsNameList.Add(entity.updateClient.PhysicsName);

        return entity;
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

    /// <summary>
    /// 各グリッドの活性化
    /// </summary>
    /// <param name="enable"></param>
    private void enableGrds(bool enable)
    {
        this.grdWaribikiCharge.Enabled = enable;
        this.grdSeisan.Enabled = enable;
        this.grdKingaku.Enabled = enable;
        this.grdKingaku2.Enabled = enable;
    }
}
