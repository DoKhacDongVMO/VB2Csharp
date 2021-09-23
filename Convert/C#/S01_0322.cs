/// <summary>
///コース一覧照会（増発）
/// </summary>
public class S01_0322 : PT99, iPT99
{

	#region 画面に実装(Interface)
	/// <summary>
	/// パターンの初期設定を行う
	/// (呼出し元から呼び出される想定)
	/// </summary>
	public void iPt99StartSetting()
	{
		//[ボタン制御用]
		//[CSV出力](F4)有無設定
		base.PtIsCsvOutFlg = false;
		//[ﾌﾟﾚﾋﾞｭｰ](F6)有無設定
		base.PtIsPrevFlg = false;
		//[印刷/出力](F7)有無設定
		base.PtIsPrintFlg = true;
		//[検索](F8)有無設定
		base.PtIsSearchFlg = false;
		//[登録](F10)有無設定
		base.PtIsRegFlg = false;
		//[更新](F11)有無設定
		base.PtIsUpdFlg = false;
		//[表示・非表示ボタン]
		base.PtDisplayBtn = null;

		//[検索領域]コントロール設定
		base.PtSearchControl = this.gbxSearch;
		//[検索結果領域]コントロール設定
		base.PtResultControl = null;
		//[検索詳細領域]コントロール設定
		base.PtDetailControl = null;
		//[件数表示ラベル]コントロール設定
		base.PtResultLblCnt = null;
		//[検索結果Max件数]コントロール設定
		base.PtMaxCount = 1000;
		//[検索結果Grid]コントロール設定
		base.PtResultGrid = null;

		//[帳票用]帳票タイプ
		base.PtPrintType = PRINTTYPE.DS;
		//[帳票用]DS用帳票ID
		base.PtDsPrintId = SystemSetCd.dsid_CrsMstTourokuList;

		//[変更チェック]カラム設定
		base.PtDiffChkColName = new List(Of string);

		//親画面に自分を設定
		base.PtMyForm = this;

	}

	/// <summary>
	/// データデフォルト値設定
	/// </summary>
	/// <param name="area"></param>
	void iPT99.iPt99SetDefValue(AREA area)
	{
		this.setDefValue(area);
	}

	public void setDefValue(AREA area)
	{
		if (area == area.FORM)
		{
			//初期化
			this.cmbCrsKind.Enabled = false;

			//コンボボックスに値を設定(S01_0301参照)
			//↑企画を選択
			InitiarizeComboBox();
			//季節
			SeasonType();
		}
		else if (area == area.SEARCH)
		{
			//年
			int SystemDate = 0;
			SystemDate = System.Convert.ToInt32(Strings.Format(CommonDateUtil.getSystemTime().Date, "yyyy"));
			nmbYear.Value = SystemDate;
		}
		else if (area == area.RESULT)
		{
		}
		else if (area == area.DETAIL)
		{
		}
		else if (area == area.BUTTON)
		{
			this.F7Key_Enabled = true;
			this.F7Key_Text = "F7:出力";
		}
	}

	/// <summary>
	/// フォームクローズ時処理
	/// (呼出し元から呼び出される想定)
	/// </summary>
	/// <returns></returns>
	public bool iPt99Closing()
	{
		return true;
	}

	#endregion

	#region 画面に実装(Overrides)
	protected override void OvPt99Init()
	{
		//初期化処理(PT99)
		base.OvPt99Init();
		this.ActiveControl = this.nmbYear;
	}
	#endregion

	#region 出力処理用
	/// <summary>
	/// 出力前(エラーチェック)処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99PrintBefore()
	{
		bool result = true;
		base.OvPt99PrintBefore();

		//必須チェック(共通ではエラーとならないため個別実装)
		if (this.nmbYear.Value == 0)
		{
			this.nmbYear.ExistError = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			this.nmbYear.Focus();
			return false;
		}

		return result;
	}

	/// <summary>
	/// DS出力時(パラメータ設定処理)処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99PrintSetDSParameter()
	{
		base.OvPt99PrintSetDSParameter();

		MakeDsParameter();
		return true;
	}

	#endregion

	#region Privateメソッド(画面独自)
	#region コンボボックスの設定
	/// <summary>
	/// 季コンボボックス
	/// </summary>
	private void SeasonType()
	{
		CdBunruiType cdBunrui = null; //コード分類
		cdBunrui = CdBunruiType.seasonMaster_Kikaku;
		//「季」コンボの初期値設定  ''「季」コンボの初期値設定
		CdMasterGet_DA clsCdMasterGet_DA = new CdMasterGet_DA();
		DataTable dtSeasonMaster = clsCdMasterGet_DA.GetCodeMasterData(cdBunrui, true); //dt季マスタ

		this.cmbSeason.DataSource = dtSeasonMaster;

		cmbSeason.ListColumns(0).Visible = false;
		cmbSeason.ListHeaderPane.Visible = false;
		cmbSeason.TextSubItemIndex = 1;
		cmbSeason.ListColumns(1).Width = cmbSeason.Width;
	}

	/// <summary>
	/// コース種別1コンボボックス
	/// </summary>
	private void InitiarizeComboBox()
	{
		//コース種別１
		DataTable dtCrsKind1 = new DataTable(); //dtコース種別1  1
		dtCrsKind1.Columns.Add("CODE");
		dtCrsKind1.Columns.Add("VALUE");

		dtCrsKind1.Rows.Add(new object[] { System.Convert.ToInt32(CrsKind1Type.kikakuTravel), getEnumAttrValue(CrsKind1Type.kikakuTravel) });
		setComboBoxToDataTable(cmbCrsKind, dtCrsKind1);
	}

	#endregion

	#region DSパラメータ作成処理
	/// <summary>
	/// DSパラメータ作成処理
	/// </summary>
	/// <returns></returns>
	private string MakeDsParameter()
	{
		ClearDsParam();
		//年
		AddDsParam(ParamType.BASE, CmpType.NONE, "ARG_CRS_YEAR", this.nmbYear.Value.ToString(), CalType.EQ);
		//コース種別1
		AddDsParam(ParamType.BASE, CmpType.AND, "ARG_TEIKI_KIKAKU_KBN", System.Convert.ToString(FixedCd.Teiki_KikakuKbnType.kikakuTravel), CalType.EQ);
		//季
		if (Strings.Trim(System.Convert.ToString(this.cmbSeason.SelectedValue)).Equals(string.Empty) == false)
		{
			AddDsParam(ParamType.FREE, CmpType.AND, "ARG_SEASON", this.cmbSeason.SelectedValue.ToString(), CalType.EQ);
		}

		//コース種別2
		int chkNum = 0;
		chkNum = System.Convert.ToInt32(System.Convert.ToDouble(Int(this.chkDay.Checked)) + System.Convert.ToDouble(Int(this.chkStay.Checked)) + System.Convert.ToDouble(Int(this.chkRcouse.Checked)));
		if (chkNum != 0 && chkNum != 3)
		{
			List listKinds2 = new List(Of string);
			if (this.chkDay.Checked == true)
			{
				listKinds2.Add(System.Convert.ToString(FixedCd.CrsKind2.higaeri));
			}
			if (this.chkStay.Checked == true)
			{
				listKinds2.Add(System.Convert.ToString(FixedCd.CrsKind2.stay));
			}
			if (this.chkRcouse.Checked == true)
			{
				listKinds2.Add(System.Convert.ToString(FixedCd.CrsKind2.rCrs));
			}
			AddDsParamIn(ParamType.FREE, CmpType.AND, "ARG_CRS_KIND_2", listKinds2.ToArray());
		}
		//コース種別3
		if (this.ucoCrsKind.JapaneseState != this.ucoCrsKind.ForeignState)
		{
			List listKinds3 = new List(Of string);
			if (this.ucoCrsKind.JapaneseState == true)
			{
				listKinds3.Add("1");
			}
			if (this.ucoCrsKind.ForeignState == true)
			{
				listKinds3.Add("2");
			}
			AddDsParamIn(ParamType.FREE, CmpType.AND, "ARG_CRS_KIND_3", listKinds3.ToArray());
		}

		base.PtDsPostData = setParamTableId("P01_0314");
		return true;
	}
	#endregion



	#endregion
}