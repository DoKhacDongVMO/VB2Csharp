public partial class S00_0316 : FormBase
{
# Region イベント

    protected override void StartupOrgProc()
    {

        // 画面上ボタン制御
        setBtnControl();

        // 画面初期化
        setControlInitiarize();

        // フォーカス設定
        this.ActiveControl = this.btnRiyouNinzuKakuteiRev;
    }

# Region フッタ
    //// <summary>
    //// F2：戻るボタン押下イベント
    //// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close()
    }

#endregion

    #Region ボタン

    //// <summary>
    //// 未チェックイン予約一覧照会ボタン実行時のイベント
    //// </summary>
    Private void btnMiCheckinYoyakuListInquiry_Click(sender As Object, e As EventArgs) Handles btnMiCheckinYoyakuListInquiry.Click
    {
        //未チェックイン予約一覧照会へ遷移
        //Using form As New S04_0201
        //    form.ShowDialog()
        //End Using
    }
        
    //// <summary>
    //// チェックイン状況一覧照会ボタン実行時のイベント
    //// </summary>
    Private void btnCheckinSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnCheckinSituationListInquiry.Click
    {
        //チェックイン状況一覧照会へ遷移
        //Using form As New S04_0203
        //    form.ShowDialog()
        //End Using
    }

#endregion
#endregion
    #Region "メソッド

    //// <summary>
    //// 画面初期化
    //// </summary>
    Private Sub setControlInitiarize()
    { 
        //ベースフォームの設定
        this.setFormId = "S00_0316"
        this.setTitle = "運行サブメニュー（チェックイン）"

        //Visible
        this.F1Key_Visible = False
        this.F2Key_Visible = True
        this.F3Key_Visible = False
        this.F4Key_Visible = False
        this.F5Key_Visible = False
        this.F6Key_Visible = False
        this.F7Key_Visible = False
        this.F8Key_Visible = False
        this.F9Key_Visible = False
        this.F10Key_Visible = False
        this.F11Key_Visible = False
        this.F12Key_Visible = False

        //Text
        this.F2Key_Text = "F2:戻る"

    }

    //// <summary>
    //// 画面上ボタン制御
    //// </summary>
    Private Sub setBtnControl()
    { 
        //ボタンを無条件で非活性(Phase2で使用するボタンの為)
        this.btnCheckinSituationListInquiry.Enabled = False
        this.btnMiCheckinYoyakuListInquiry.Enabled = False

    }

#End Region
}