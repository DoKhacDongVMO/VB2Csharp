public class S00_0317 : FormBase
{
    #Region イベント

    //// <summary>
    //// フォーム起動時の独自処理
    //// </summary>
    protected override void StartupOrgProc()
    {
        //画面上ボタン制御
        this.setBtnControl();

        //画面初期化
        this.setControlInitiarize();

        //フォーカス設定
        this.ActiveControl = this.btnMihakkenYoyakuListInquiry;

    }

    //// <summary>
    //// F2：戻るボタン押下イベント
    //// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }

#EndRegion

    #Region フッタ

    //// <summary>
    //// 乗車状況照会ボタン実行時のイベント
    //// </summary>
    private void btnMihakkenYoyakuListInquiry_Click(sender As Object, e As EventArgs) Handles btnMihakkenYoyakuListInquiry.Click
    {
        //乗車状況照会へ遷移
        using (S04_0301 form = new S04_0301())
        {     
            form.ShowDialog();
        }
    }

    //// <summary>
    //// NO SHOW一覧ボタン実行時のイベント
    //// </summary>
    private void btnNoShowList_Click(sender As Object, e As EventArgs) Handles btnNoShowList.Click
    { 
        //NO SHOW 一覧へ遷移
        using (S04_0303 form = new S04_0303())
        {
            form.ShowDialog();
        }
    }

    //// <summary>
    //// お客様状況一覧照会ボタン実行時のイベント
    //// </summary>
    private void btnCustomerSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnCustomerSituationListInquiry.Click
    {
    //お客様状況一覧照会へ遷移
    //Using form As New S04_0304
    //    form.ShowDialog()
    //End Using
    }

# EndRegion

# Region メソッド

    //// <summary>
    //// 画面初期化
    //// </summary>
    private void setControlInitiarize()
    {
    //ベースフォームの設定
    this.setFormId = "S00_0317";
    this.setTitle = "運行サブメニュー（受付状況確認）";

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

    private void setBtnControl()
    {
    //ボタンを無条件で非活性(Phase2で使用するボタンの為)
    this.btnCustomerSituationListInquiry.Enabled = False;

    }

#End Region

}
