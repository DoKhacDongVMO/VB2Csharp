using System.ComponentModel;


/// <summary>
/// S00_0202 造成メニュー
/// </summary>
public class S00_0202 : FormBase
{

	private DataTable dt;
	#region イベント

	/// <summary>
	/// ロード時イベント
	/// </summary>
	protected override void StartupOrgProc()
	{

		// 画面初期化
		setControlInitiarize();

		CommonDa.LogTable = "S_LOG";

	}
	#endregion

	#region フッタ
	private void ButtonEx2_Click(object sender, EventArgs e)
	{
		using (S01_0305 S01_0305 = new S01_0305())
		{
			S01_0305.ShowDialog();
		}

	}
	//TODO:遷移方法の反映
	/// <summary>
	/// 発券情報照会(予約照会)ボタン実行時のイベント
	/// </summary>
	private void btnHakkenInfoInquiryYoyakuInquiry_Click(object sender, EventArgs e)
	{
		using (S01_0301 S02_0601 = new S01_0301())
		{
			S02_0601.ShowDialog();
		}

	}
	#endregion

	#region メソッド

	/// <summary>
	/// 画面初期化
	/// </summary>
	private void setControlInitiarize()
	{

		//画面上ボタン制御
		this.setBtnControl();
		// フッタボタンの設定
		this.setButtonInitiarize();
		// ロード時にフォーカスを設定する
		this.ActiveControl = this.btnCrsSearch;
	}

	/// <summary>
	/// 画面上ボタン制御
	/// </summary>
	private void setBtnControl()
	{

		// 権限に従いボタンの活性/非活性を制御
		btnCrsSearch.Enabled = Common.AuthInfo.canUse(Common.FixedCd.SystemCode.M0101); // コース検索
		ButtonEx2.Enabled = Common.AuthInfo.canUse(Common.FixedCd.SystemCode.M0102); // 使用済みコース番号照会
		ButtonEx3.Enabled = Common.AuthInfo.canUse(Common.FixedCd.SystemCode.M0103); // ピックアップホテルマスタ
		ButtonEx4.Enabled = Common.AuthInfo.canUse(Common.FixedCd.SystemCode.M0104); // ピックアップマスタ

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		//TODO:ボタンの表示/非表示を変更
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

		//Text
		//TODO:ボタンのテキストを変更(必要に応じて)
		//Me.F1Key_Text = "F1:"
		this.F2Key_Text = "F2:戻る";
		//Me.F3Key_Text = "F3:終了"
		//Me.F4Key_Text = "F4:削除"
		//Me.F5Key_Text = "F5:参照"
		//Me.F6Key_Text = "F6:ﾌﾟﾚﾋﾞｭｰ"
		//Me.F7Key_Text = "F7:印刷" 'TODO: 紙出力の場合「F7:印刷」、DataStudio出力の場合「F7:出力」
		//Me.F8Key_Text = "F8:"
		//Me.F9Key_Text = "F9:CSV出力"
		//Me.F10Key_Text = "F10:登録"
		//Me.F11Key_Text = "F11:更新"
		//Me.F12Key_Text = "F12:"

		//TODO:ボタンの使用可/非を変更(必要に応じて)
		//Me.F1Key_Enabled = False
		//Me.F2Key_Enabled = False
		//Me.F3Key_Enabled = False
		//Me.F4Key_Enabled = False
		//Me.F5Key_Enabled = False
		//Me.F6Key_Enabled = False
		//Me.F7Key_Enabled = False
		//Me.F8Key_Enabled = False
		//Me.F9Key_Enabled = False
		//Me.F10Key_Enabled = False
		//Me.F11Key_Enabled = False
		//Me.F12Key_Enabled = False

	}

	private void ButtonEx1_Click(object sender, EventArgs e)
	{
		using (Test用 S02_0601 = new Test用())
		{
			S02_0601.ShowDialog();
		}

	}

	/// <summary>
	/// コンボボックスにDataTableの割付けを行う
	/// </summary>
	/// <param name="targetCtl"></param>
	/// <param name="dtList"></param>
	/// <remarks></remarks>
	private void setComboBoxToDataTable(ComboBoxEx targetCtl, DataTable dtList)
	{
		ComboBoxEx with_1 = targetCtl;
		with_1.DataSource = dtList;
		with_1.ValueSubItemIndex = 0;
		with_1.ListColumns(0).Visible = false;
		with_1.ListHeaderPane.Visible = false;
		with_1.TextSubItemIndex = 1;
		with_1.ListColumns(1).Width = with_1.Width;
		with_1.DropDown.AllowResize = false;
	}

	//ピックアップホテルマスタボタン押下
	private void ButtonEx3_Click(object sender, EventArgs e)
	{
		//ピックアップホテルマスタ管理表示
		using (S01_0502 form = new S01_0502())
		{
			form.ShowDialog();
		}

	}

	//ピックアップマスタボタン押下
	private void ButtonEx4_Click(object sender, EventArgs e)
	{
		//ピックアップマスタ照会管理表示
		using (S01_0501 form = new S01_0501())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// F2キー（戻る）押下時イベント
	/// </summary>
	/// <remarks></remarks>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();
	}

	#endregion

}