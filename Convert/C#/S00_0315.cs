/// <summary>
/// S00_0315 運行サブメニュー(利用人員確定)
/// </summary>
using System;

public partial class S00_0315 : FormBase
{

    #region イベント

    /// <summary>
    /// フォーム起動時の独自処理
    /// </summary>
    protected override void StartupOrgProc()
    {

        // 画面上ボタン制御
        setBtnControl();

        // 画面初期化
        setControlInitiarize();

        // フォーカス設定
        this.ActiveControl = this.btnRiyouNinzuKakuteiRev;
    }

    #region フッタ

    /// <summary>
    /// F2：閉じるボタン押下イベント
    /// </summary>
    protected override void btnF2_ClickOrgProc()
    {
        this.Close();
    }

    #endregion

    #region ボタン

    /// <summary>
    /// 利用人員確定・修正ボタン実行時のイベント
    /// </summary>
    private void btnRiyouNinzuKakuteiRev_Click(object sender, EventArgs e)
    {
        // 利用人員確定・修正へ遷移
        using (var form = new S04_0101())
        {
            form.ShowDialog();
        }
    }

    /// <summary>
    /// 船車券ＡＧＴ乗車確認入力ボタン実行時のイベント
    /// </summary>
    private void btnSensyaKenAＧＴJyosyaKakuninInput_Click(object sender, EventArgs e)
    {
        // 船車券ＡＧＴ乗車確認入力へ遷移
        using (var form = new S04_0103())
        {
            form.ShowDialog();
        }
    }

    /// <summary>
    /// 売上確定照会（定期）ボタン実行時のイベント
    /// </summary>
    private void btnUriageKakuteiInquiryTeiki_Click(object sender, EventArgs e)
    {
        // 売上確定照会（定期）へ遷移
        using (var form = new S04_0104())
        {
            form.ShowDialog();
        }
    }

    #endregion

    #endregion

    #region メソッド

    /// <summary>
    /// 画面初期化
    /// </summary>
    private void setControlInitiarize()
    {

        // ベースフォームの設定
        this.setFormId = "S00_0315";
        this.setTitle = "運行サブメニュー（利用人員確定）";

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

        // 現状処理なし

    }

    #endregion

}