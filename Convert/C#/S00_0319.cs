//// <summary>
//// S00_0319 運行サブメニュー（ピックアップ管理）
//// </summary>
public class S00_0319 : FormBase
{ 
#Region イベント

    //// <summary>
    //// フォーム起動時の独自処理
    //// </summary>
    protected override void StartupOrgProc()
    { 
        //画面上ボタン制御
        this.setBtnControl();

        //画面表示時の初期設定
        this.setControlInitiarize();

        //フォーカス設定
        this.ActiveControl = this.btnYoyakuListInquiryPickup

    }

#Region フッタ

    //// <summary>
    //// F2：戻るボタン押下イベント
    //// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }

#EndRegion

#Region ボタン

    //// <summary>
    //// 予約一覧照会(ピックアップ) ボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnYoyakuListInquiryPickup_Click(sender As Object, e As EventArgs) Handles btnYoyakuListInquiryPickup.Click
    {
        //予約一覧照会（ピックアップ）へ遷移
        using (S04_0601 form = new S04_0601())
        {
            form.ShowDialog();
        }
    }

    //// <summary>
    //// ピックアップ用リストボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnPickupForList_Click(sender As Object, e As EventArgs) Handles btnPickupForList.Click
    {
        //ピックアップ用リストへ遷移
        using (S04_0602 form = new S04_0602())
        {
            form.ShowDialog();
        }
    }

    //// <summary>
    //// ピックアップ予約・乗車状況一覧照会ボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnPickupYoyakuJyosyaSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnPickupYoyakuJyosyaSituationListInquiry.Click
    {
    //ピックアップ予約・乗車状況一覧照会へ遷移
    //Using form As New S04_0603()
    //    form.ShowDialog()
    //End Using
    }

    //// <summary>
    //// ピックアップ時刻修正ボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnPickupTimeRev_Click(sender As Object, e As EventArgs) Handles btnPickupTimeRev.Click
    {     
        //ピックアップ時刻修正へ遷移
        using (S04_0604 form = new S04_0604())
        {
            form.ShowDialog();
        }
    }
    
    //// <summary>
    //// ピックアップ在庫調整ボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnPickupStockCyosei_Click(sender As Object, e As EventArgs) Handles btnPickupStockCyosei.Click
    {
        //ピックアップ在庫調整へ遷移
        using (S04_0605 form = new S04_0605())
        {
            form.ShowDialog();
        }
    }

    //// <summary>
    //// ピックアップ台帳修正（予約停止）ボタン実行時のイベント
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnPickupLedgerRevYoyakuStop_Click(sender As Object, e As EventArgs) Handles btnPickupLedgerRevYoyakuStop.Click
    {
        //ピックアップ台帳修正（予約停止）へ遷移
        using (S04_0606 form = new S04_0606())
        {
            form.ShowDialog();
        }
    }

# EndRegion

# EndRegion

# Region メソッド

        //// <summary>
        //// 画面表示時の初期設定
        //// </summary>
        private void setControlInitiarize()
        { 
        //ベースフォームの設定
            this.setFormId = "S00_0319";
            this.setTitle = "運行サブメニュー（ピックアップ管理）";

            //Visible
            this.F1Key_Visible = False;
            this.F2Key_Visible = True;
            this.F3Key_Visible = False;
            this.F4Key_Visible = False;
            this.F5Key_Visible = False;
            this.F6Key_Visible = False;
            this.F7Key_Visible = False;
            this.F8Key_Visible = False;
            this.F9Key_Visible = False;
            this.F10Key_Visible = False;
            this.F11Key_Visible = False;
            this.F12Key_Visible = False;

            //Text
            this.F2Key_Text = "F2:戻る";

        }

        //// <summary>
        //// 画面上ボタン制御
        //// </summary>
        private void setBtnControl()
        {
            //ボタンを無条件で非活性(Phase2で使用するボタンの為)
            this.btnPickupYoyakuJyosyaSituationListInquiry.Enabled = False;
        }
    }

#EndRegion

}