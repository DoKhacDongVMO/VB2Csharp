//// <summary>
//// S00_0318 運行サブメニュー（準備金管理）
//// </summary>
public class S00_0318 : FormBase
{
# Region イベント

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
    this.ActiveControl = this.btnS04_0501;
    }

    //// <summary>
    //// F2：戻るボタン押下イベント
    //// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }

# End Region

# Region フッタ

# End Region

# Region メソッド

    //// <summary>
    //// 画面初期化
    //// </summary> 
    private void setControlInitiarize()
    {
    //ベースフォームの設定
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
    // 現状処理なし
    }

    //// <summary>
    //// //起動画面 ： 『準備金基準情報設定』 （S04_0501）
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnS04_0501_Click(sender As Object, e As EventArgs) Handles btnS04_0501.Click
    {
        using (S04_0501 form = new S04_0501())
        {
            form.patternSettings();
            form.ShowDialog();
        }
    }

    //// <summary>
    //// 起動画面 ： 『準備金管理情報設定』 （S04_0502）
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnS04_0502_Click(sender As Object, e As EventArgs) Handles btnS04_0502.Click
    { 
        //準備金管理情報設定ボタン押下時処理
        using (S04_0502 form = new S04_0501S04_0502))
        {
            form.patternSettings();
            form.ShowDialog();
        }
    }
    //// <summary>
    //// 起動画面 ： 『準備金申請確定』 （S04_0503）
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnS04_0503_Click(sender As Object, e As EventArgs) Handles btnS04_0503.Click
    {
        //準備金申請確定ボタン押下時処理
        using (S04_0503 form = new S04_0503))
        {
            form.patternSettings();
            form.ShowDialog();
        }
    }
    //// <summary>
    //// 起動画面 ： 『準備金ラベル』 （S04_0504）
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    private void btnS04_0504_Click(sender As Object, e As EventArgs) Handles btnS04_0504.Click
    { 
        //準備金ラベルボタン押下時処理
        using (S04_0504 form = new S04_0504))
        {
            form.patternSettings();
            form.ShowDialog();
        }
    }
    //// <summary>
    //// 起動画面 ： 『準備金仮払精算明細書』 （S04_0505）
    //// </summary>
    //// <param name="sender"></param>
    //// <param name="e"></param>
    Private Sub btnS04_0505_Click(sender As Object, e As EventArgs) Handles btnS04_0505.Click
        //準備金仮払精算明細表ボタン押下時処理
        using (S04_0505 form = new S04_0505))
        {
            form.patternSettings();
            form.ShowDialog();
        }

#EndRegion

}