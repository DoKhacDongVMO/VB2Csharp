/// <summary>
/// ''' S00_0321 運行サブメニュー（帳票）
/// ''' </summary>
public class S00_0321 : FormBase
{


    /// <summary>
    /// フォーム起動時の独自処理
    /// </summary>
    protected override void StartupOrgProc()
    {

        // 画面上ボタン制御
        this.setBtnControl();

        // 画面表示時の初期設定
        this.setControlInitiarize();

        // フォーカス設定
        this.btnYoyakuPersonList.Focus();
    }


    /// <summary>
    /// F2：戻るボタン押下イベント
    /// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }



    /// <summary>
    /// 予約者リストボタン実行時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnYoyakuPersonList_Click(object sender, EventArgs e)
    {
        // 予約者リストへ遷移
        using (S04_1001 form = new S04_1001())
        {
            form.ShowDialog();
        }
    }

    /// <summary>
    /// 人員表／日別運行表ボタン実行時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnJininTableDayBetuUnkouTable_Click(object sender, EventArgs e)
    {
        // 人員表／日別運行表へ遷移
        using (S04_1002 form = new S04_1002())
        {
            form.ShowDialog();
        }
    }

    /// <summary>
    /// 乗客名簿ボタン実行時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnJyokyakuMeibo_Click(object sender, EventArgs e)
    {
        // 乗客名簿へ遷移
        using (S04_1003 form = new S04_1003())
        {
            form.ShowDialog();
        }
    }

    /// <summary>
    /// 配車表ボタン実行時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnHaisyaTable_Click(object sender, EventArgs e)
    {
    }




    /// <summary>
    /// 画面表示時の初期設定
    /// </summary>
    private void setControlInitiarize()
    {

        // ベースフォームの設定
        this.setFormId = "S00_0321";
        this.setTitle = "運行サブメニュー（帳票）";

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
        this.F10Key_Visible = false;
        this.F11Key_Visible = false;
        this.F12Key_Visible = false;

        // Text
        this.F2Key_Text = "F2:戻る";
    }

    /// <summary>
    /// 画面上ボタン制御
    /// </summary>
    private void setBtnControl()
    {

        // ボタンを無条件で非活性(Phase2で使用するボタンの為)
        this.btnHaisyaTable.Enabled = false;
    }
}
