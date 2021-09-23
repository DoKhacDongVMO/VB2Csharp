using C1.Win.C1FlexGrid;
using GrapeCity.Win.Editors;


/// <summary>
/// コース情報管理ユーザーコントロール共通
/// </summary>
public class ucoS01_0307_Common
{

	#region 定数


	public Color FuzokuInfoAri_CellBackgroundColor;
	public Color FuzokuInfoNasi_CellBackgroundColor;

	public const string FuzokuInfoAri_CellBackgroundColor_StyleNm = "情報有";
	public const string FuzokuInfoNasi_CellBackgroundColor_StyleNm = "情報無";

	public const string JiBunLiteral = "__:__";
	public const string YmdLiteral = "____/__/__";

	public const string TimeMin = "00:01";
	public const string TimeMax = "27:59";
	public const string timeHour = "00:";

	#endregion

	#region 列挙

	#region 不要 → CrsKinds を参照
	//Public Enum HeadBunruiType As Integer
	//    <Value("定期")>
	//    teiki = 1
	//    <Value("企画")>
	//    kikaku
	//    <Value("外客")>
	//    gaikyaku
	//End Enum

	//Public Enum ChuBunruiType As Integer
	//    <Value("定期観光")>
	//    teikiKanko = 1
	//    <Value("日帰り")>
	//    higaeri
	//    <Value("宿泊")>
	//    stay
	//End Enum
	#endregion

	#endregion

	#region コンストラクタ
	public ucoS01_0307_Common()
	{
		FuzokuInfoAri_CellBackgroundColor = FixedCd.BackgroundColorType.Normal;
		FuzokuInfoNasi_CellBackgroundColor = FixedCd.BackgroundColorType.InfoNasi_BackgroundColor;



	}
	#endregion

	/// <summary>
	/// コントロール値のクリア
	/// </summary>
	/// <param name="targetCtr"></param>
	public void clearContorol(Control targetCtr)
	{

		//For Each ctr As Control In targetCtr.Controls
		//    If ctr.GetType Is GetType(TextBoxEx) Then
		//        ' テキストボックス
		//        CType(ctr, TextBoxEx).Text = String.Empty

		//    ElseIf ctr.GetType Is GetType(NumberEx) Then
		//        ' ナンバーテキストボックス
		//        CType(ctr, NumberEx).Text = String.Empty

		//    ElseIf ctr.GetType Is GetType(DateEx) Then
		//        ' 日付テキストボックス
		//        CType(ctr, DateEx).Text = String.Empty

		//    ElseIf ctr.GetType Is GetType(ComboBoxEx) Then
		//        ' コンボボックス
		//        CType(ctr, ComboBoxEx).Clear()

		//    ElseIf ctr.GetType Is GetType(CheckBoxEx) Then
		//        ' チェックボックス
		//        CType(ctr, CheckBoxEx).Checked = False

		//    ElseIf ctr.GetType Is GetType(Panel) Then
		//        ' パネル
		//        clearContorol(ctr)

		//    ElseIf ctr.GetType Is GetType(GroupBox) Then
		//        ' グループボックス
		//        clearContorol(ctr)

		//    End If
		//Next

		foreach (Control ctr in targetCtr.Controls)
		{

			if (ctr.HasChildren)
			{
				clearContorol(ctr);
			}

			if (ReferenceEquals(ctr.GetType, typeof(TextBoxEx)))
			{
				((TextBoxEx)ctr).Text = string.Empty;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(NumberEx)))
			{
				((NumberEx)ctr).Text = string.Empty;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(DateEx)))
			{
				((DateEx)ctr).Value = null;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TimeEx)))
			{
				((TimeEx)ctr).Value = null;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(ComboBoxEx)))
			{
				((ComboBoxEx)ctr).Clear();

			}
			else if (ReferenceEquals(ctr.GetType, typeof(CheckBoxEx)))
			{
				((CheckBoxEx)ctr).Checked = false;

			}

			if (ReferenceEquals(ctr.GetType, typeof(Panel)))
			{
				clearContorol(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBoxEx)))
			{
				clearContorol(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBox)))
			{
				clearContorol(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TabControl)))
			{
				clearContorol(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TabPage)))
			{
				clearContorol(ctr);

				//ElseIf ctr.GetType Is GetType(FlexGridEx) Then
				//    CType(ctr, FlexGridEx).Rows.Count = CType(ctr, FlexGridEx).Rows.Fixed

			}
		}

		// グリッドのクリアは行わない（個別に行う）


	}

	/// <summary>
	/// エラー表示のクリアを行う
	/// </summary>
	/// <param name="targetCtr"></param>
	/// <remarks></remarks>
	public void clearErrinfo(Control targetCtr)
	{

		foreach (Control ctr in targetCtr.Controls)
		{

			if (ctr.HasChildren)
			{
				clearErrinfo(ctr);
			}

			if (ReferenceEquals(ctr.GetType, typeof(TextBoxEx)))
			{
				((TextBoxEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(NumberEx)))
			{
				((NumberEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(DateEx)))
			{
				((DateEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TimeEx)))
			{
				((TimeEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(ComboBoxEx)))
			{
				((ComboBoxEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(CheckBoxEx)))
			{
				((CheckBoxEx)ctr).ExistError = false;

			}
			else if (ReferenceEquals(ctr.GetType, typeof(Panel)))
			{
				clearErrinfo(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBox)))
			{
				clearErrinfo(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(GroupBoxEx)))
			{
				clearErrinfo(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TabControl)))
			{
				clearErrinfo(ctr);

			}
			else if (ReferenceEquals(ctr.GetType, typeof(TabPage)))
			{
				clearErrinfo(ctr);

				//ElseIf ctr.GetType Is GetType(FlexGridEx) Then
				//    CType(ctr, FlexGridEx).Rows.Count = 1
			}
		}

	}

	/// <summary>
	/// コントロール を参照モードに切り替える
	/// </summary>
	/// <param name="_processMode"></param>
	public void setProcessMode(Control targetCtr, FixedCd.ProcessMode _processMode)
	{

		if (_processMode == FixedCd.ProcessMode.reference)
		{
			foreach (Control _control in targetCtr.Controls)
			{

				if (_control.HasChildren)
				{
					setProcessMode(_control, _processMode);
				}

				if (_control is TextBoxEx)
				{
					((TextBoxEx)_control).ReadOnly = true;

				}
				else if (_control is NumberEx)
				{
					((NumberEx)_control).ReadOnly = true;

				}
				else if (_control is DateEx)
				{
					((DateEx)_control).ReadOnly = true;

				}
				else if (_control is TimeEx)
				{
					((TimeEx)_control).ReadOnly = true;

				}
				else if (_control is ComboBoxEx)
				{
					((ComboBoxEx)_control).ReadOnly = true;

				}
				else if (_control is CheckBoxEx)
				{
					((CheckBoxEx)_control).Enabled = false;

				}
				else if (_control is RadioButtonEx)
				{
					((RadioButtonEx)_control).Enabled = false;

				}
				else if (_control is FlexGridEx)
				{

					//If DirectCast(_control, FlexGridEx).AllowEditing = True Then
					//    For colIdx As Integer = DirectCast(_control, FlexGridEx).Cols.Fixed To DirectCast(_control, FlexGridEx).Cols.Count - 1
					//        If DirectCast(_control, FlexGridEx).Cols(colIdx).ComboList >= "|..." Then
					//            DirectCast(_control, FlexGridEx).Cols(colIdx).ComboList = "..."
					//        ElseIf DirectCast(_control, FlexGridEx).Cols(colIdx).ComboList <> "..." Then
					//            DirectCast(_control, FlexGridEx).Cols(colIdx).AllowEditing = False
					//        End If
					//    Next
					//End If

					for (int colIdx = ((FlexGridEx)_control).Cols.Fixed; colIdx <= ((FlexGridEx)_control).Cols.Count - 1; colIdx++)
					{
						if (((FlexGridEx)_control).Cols(colIdx).ComboList >= "|...")
						{
							((FlexGridEx)_control).Cols(colIdx).ComboList = "...";

						}
						else if (((FlexGridEx)_control).Cols(colIdx).ComboList != "...")
						{
							((FlexGridEx)_control).Cols(colIdx).AllowEditing = false;

							// 個別に設定が必要なためコメント（ボタン表示が必要なセルがあるため）
							//ElseIf DirectCast(_control, FlexGridEx).Cols(colIdx).ComboList = "..." Then
							//    DirectCast(_control, FlexGridEx).Cols(colIdx).ComboList = ""
							//    DirectCast(_control, FlexGridEx).Cols(colIdx).AllowEditing = False

						}
					}

				}
				else if (_control is CalendarControl)
				{
					((CalendarControl)_control).ReadingSenyo = true;

				}


				if (_control is GroupBoxEx)
				{
					setProcessMode(_control, _processMode);

				}
				else if (_control is GroupBox)
				{
					setProcessMode(_control, _processMode);

				}
				else if (_control is TabControl)
				{
					setProcessMode(_control, _processMode);

				}
				else if (_control is TabPage)
				{
					setProcessMode(_control, _processMode);

				}

			}
		}
		else
		{
		}

	}

	/// <summary>
	/// コントロール を使用不可に切り替える
	/// </summary>
	/// <param name="targetCtr"></param>
	public void setNotUseControls(Control targetCtr)
	{

		foreach (Control _control in targetCtr.Controls)
		{

			if (_control.HasChildren)
			{
				setNotUseControls(_control);
			}

			if (_control is LabelEx)
			{
				continue;
			}
			else
			{
				// 上記以外は全て使用不可にする
				changeEnableProperty(_control, false, false);
			}

		}

	}

	/// <summary>
	/// コンボボックスにDataTableの割付けを行う
	/// </summary>
	/// <param name="targetCtl"></param>
	/// <param name="dtList"></param>
	/// <remarks></remarks>
	public void setComboBoxToDataTable(ComboBoxEx targetCtl, DataTable dtList)
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


	/// <summary>
	/// グリッド共通設定
	/// </summary>
	public void setGridCommonInitialize(FlexGridEx targetCtl)
	{

		// ---------------------------------------------------------------
		// グリッドユーザーコントロールに実装すれば このメソッドは不要
		// ---------------------------------------------------------------

		// カラムの並び替えを可能にするか (初期値：True)
		targetCtl.AllowDragging = AllowDraggingEnum.None;

		// グリッドがデータソースに連結された時に列を自動的に作成するか (初期値：True)
		targetCtl.AutoGenerateColumns = false;

		// セルにいつコンボボタンを表示するか (初期値：Inherit)
		targetCtl.ShowButtons = ShowButtonsEnum.Always;

	}

	#region グリッドのセル項目型
	/// <summary>
	/// 文字項目セルのテキストを取得します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public TextBoxEx getTextEx(int _maxLen)
	{
		TextBoxEx _textEx = new TextBoxEx();

		_textEx.Text = string.Empty;
		_textEx.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
		_textEx.MaxLength = _maxLen;
		_textEx.MaxLengthUnit = GrapeCity.Win.Editors.LengthUnit.Byte;
		_textEx.Format = "ＺH^aＴ";

		return _textEx;
	}

	/// <summary>
	/// 文字項目セルのテキストを取得します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public TextBoxEx getTextEx(int _maxLen, string format)
	{
		TextBoxEx _textEx = new TextBoxEx();

		_textEx.Text = string.Empty;
		_textEx.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
		_textEx.MaxLength = _maxLen;
		_textEx.MaxLengthUnit = GrapeCity.Win.Editors.LengthUnit.Byte;
		_textEx.Format = format;

		return _textEx;
	}

	#region 不要
	///' <summary>
	///' 数値項目セルのテキストを取得します。
	///' </summary>
	///' <returns></returns>
	///' <remarks></remarks>
	//Public Function getNumberEx(ByVal _maxLen As Integer) As NumberEx
	//    Dim _numEx As New NumberEx

	//    _numEx.Fields.IntegerPart.MaxDigits = _maxLen
	//    _numEx.Fields.DecimalPart.MaxDigits = 0
	//    _numEx.Fields.IntegerPart.GroupSeparator = CChar("")
	//    _numEx.ImeMode = Windows.Forms.ImeMode.Disable
	//    _numEx.Spin.SpinOnKeys = False
	//    _numEx.AllowDeleteToNull = True

	//    Return _numEx
	//End Function
	#endregion

	/// <summary>
	/// 数値項目セルのテキストを取得します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public NumberEx getNumberEx(int _maxDigits, bool _commaSeparator, Nullable[] minValue)
	{
		NumberEx _numEx = new NumberEx();

		_numEx.Fields.IntegerPart.MaxDigits = _maxDigits;
		_numEx.Fields.DecimalPart.MaxDigits = 0;
		if (_commaSeparator == true)
		{
			_numEx.Fields.IntegerPart.GroupSeparator = ',';
		}
		else
		{
			_numEx.Fields.IntegerPart.GroupSeparator = char.Parse("");
		}
		_numEx.ImeMode = System.Windows.Forms.ImeMode.Disable;
		_numEx.Spin.SpinOnKeys = false;
		_numEx.AllowDeleteToNull = true;

		// 最小値の設定
		if (ReferenceEquals(minValue, null))
		{
			// 指定がない場合は、(桁数 * -1) を設定 (ex) 3桁の場合：-999
			minValue = (decimal.Parse(string.Concat(Enumerable.Repeat("9", _maxDigits)))) * -1;
		}
		_numEx.MinValue = minValue;


		return _numEx;
	}

	/// <summary>
	/// 時刻項目セル
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public TimeEx getTimeCell()
	{
		TimeEx _ymdCell = new TimeEx();

		GrapeCity.Win.Editors.Fields.DateMinuteField dateMinuteField = new GrapeCity.Win.Editors.Fields.DateMinuteField();
		GrapeCity.Win.Editors.Fields.DateLiteralField dateLiteralField = new GrapeCity.Win.Editors.Fields.DateLiteralField(":");
		GrapeCity.Win.Editors.Fields.DateSecondField dateSecondField = new GrapeCity.Win.Editors.Fields.DateSecondField();
		GcDateValidator.InvalidRange invalidRange1 = new GcDateValidator.InvalidRange();
		ValueProcess valueProcess1 = new ValueProcess();

		_ymdCell.SideButtons.Clear();
		_ymdCell.Fields.AddRange(new GrapeCity.Win.Editors.Fields.DateField[] { dateMinuteField, dateLiteralField, dateSecondField });
		_ymdCell.AlternateText.DisplayNull.Text = " ";
		_ymdCell.ImeMode = System.Windows.Forms.ImeMode.Disable;
		_ymdCell.MaxValue = System.TimeSpan.Parse("00:27:59");
		_ymdCell.MinValue = System.TimeSpan.Parse("00:00:01");
		//.HatoMode = True

		return _ymdCell;
	}

	/// <summary>
	/// 年月日項目セル
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public DateEx getYmdCell()
	{
		DateEx _ymdCell = new DateEx();
		GrapeCity.Win.Editors.Fields.DateEraYearField dateEraYearField = new GrapeCity.Win.Editors.Fields.DateEraYearField();
		GrapeCity.Win.Editors.Fields.DateLiteralField dateLiteralField1 = new GrapeCity.Win.Editors.Fields.DateLiteralField("/");
		GrapeCity.Win.Editors.Fields.DateMonthField dateMonthField = new GrapeCity.Win.Editors.Fields.DateMonthField();
		GrapeCity.Win.Editors.Fields.DateLiteralField dateLiteralField2 = new GrapeCity.Win.Editors.Fields.DateLiteralField("/");
		GrapeCity.Win.Editors.Fields.DateDayField dateDayField = new GrapeCity.Win.Editors.Fields.DateDayField();

		_ymdCell.SideButtons.Clear();
		_ymdCell.Fields.AddRange(new GrapeCity.Win.Editors.Fields.DateField[]
			{dateEraYearField, dateLiteralField1, dateMonthField, dateLiteralField2, dateDayField});
		_ymdCell.AlternateText.DisplayNull.Text = " ";
		_ymdCell.ImeMode = System.Windows.Forms.ImeMode.Disable;
		return _ymdCell;
	}

	#endregion

	/// <summary>
	/// 定義値を返す
	/// </summary>
	/// <param name="pObj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public Nullable[] nnvl_int(object pObj)
	{
		Nullable[] returnValue = null;
		if (!Information.IsDBNull(pObj) && !ReferenceEquals(pObj, null) && !(System.Convert.ToString(pObj) == string.Empty))
		{
			return System.Convert.ToInt32(pObj);
		}
		else
		{
			return returnValue;
		}
	}

	/// <summary>
	/// 定義値を返す
	/// </summary>
	/// <param name="pObj"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public string nnvl_str(object pObj)
	{
		string returnValue = string.Empty;
		if (!Information.IsDBNull(pObj) && !ReferenceEquals(pObj, null) && !(System.Convert.ToString(pObj).Trim() == string.Empty))
		{
			return System.Convert.ToString(pObj);
		}
		else
		{
			return returnValue;
		}
	}

	/// <summary>
	/// コンボボックスの選択内容を取得する
	/// </summary>
	/// <param name="ctl"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public string getItemValueForComboBox(ComboBoxEx ctl)
	{
		string retValue = "";

		if (ctl.SelectedValue IsNot null)
		{
			retValue = System.Convert.ToString(ctl.SelectedValue).Trim();
		}

		return retValue;
	}

	/// <summary>
	/// チェックボックスの選択内容を取得する
	/// </summary>
	/// <param name="ctl"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public int getItemValueForCheckBox(CheckBoxEx ctl)
	{
		int retValue = 0;

		if (ctl.Checked == true)
		{
			retValue = 1;
		}

		return retValue;
	}

	/// <summary>
	/// カンマ変換メソッド
	/// ※エンティティの金額の値をカンマ付の値に変換します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public string convertComma(Nullable[] kingaku)
	{
		if (ReferenceEquals(kingaku, null))
		{
			return string.Empty;
		}
		else
		{
			return kingaku.Value.ToString("#,0");
		}

	}

	/// <summary>
	/// コントロール使用切替（clearValue = true：値クリア）
	/// </summary>
	/// <param name="ctr"></param>
	/// <param name="enabled"></param>
	/// <param name="clearValue"></param>
	public void changeEnableProperty(Control ctr, bool enabled, bool clearValue)
	{

		if (ReferenceEquals(ctr.GetType, typeof(TextBoxEx)))
		{

			((TextBoxEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((TextBoxEx)ctr).Text = string.Empty;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(NumberEx)))
		{

			((NumberEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((NumberEx)ctr).Text = string.Empty;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(DateEx)))
		{

			((DateEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((DateEx)ctr).Text = string.Empty;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(TimeEx)))
		{

			((TimeEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((TimeEx)ctr).Text = string.Empty;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(ComboBoxEx)))
		{

			((ComboBoxEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((ComboBoxEx)ctr).SelectedValue = null;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(CheckBoxEx)))
		{

			((CheckBoxEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((CheckBoxEx)ctr).Checked = false;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(ButtonEx)))
		{

			((ButtonEx)ctr).Enabled = enabled;
			if (clearValue == true)
			{
				((ButtonEx)ctr).Text = string.Empty;
			}

		}
		else if (ReferenceEquals(ctr.GetType, typeof(FlexGridEx)))
		{

			// グリッドは使用不可設定のみ
			((FlexGridEx)ctr).Enabled = enabled;

		}

	}

	#region 行程情報の種別を取得する

	/// <summary>
	/// 行程情報の種別を取得する
	/// </summary>
	/// <param name="cdMaster_SuppliersKind"></param>
	/// <param name="crsKinds"></param>
	/// <returns></returns>
	public DataTable getTblSuppliersKind_Itinerary(CdMaster_SuppliersKindType cdMaster_SuppliersKind, CrsKinds crsKinds)
	{

		string kindJoken_Itinerary = string.Empty;
		string kindJoken_SonotaItinerary = string.Empty;

		kindJoken_Itinerary = getKind_ItineraryConditions(crsKinds);
		if (kindJoken_Itinerary != string.Empty)
		{
			kindJoken_Itinerary += " AND ";
		}

		//ADD-20121116-ﾎﾃﾙ(ｸｰﾎﾟﾝ)を追加（行程情報に表示されないよう制御）
		//_種別条件_行程 &= " NAIYO_1 <> " & CStr(行程種別タイプ.その他)
		kindJoken_Itinerary += " NAIYO_1 IN ('" + System.Convert.ToString(CdMaster_SuppliersKindType.all) + "', '" + System.Convert.ToString(cdMaster_SuppliersKind) + "') ";

		CdMasterGet_DA _cdMasterGet = new CdMasterGet_DA();
		return _cdMasterGet.GetCodeMasterData(CdBunruiType.suppliersKindMaster, false, kindJoken_Itinerary);

	}

	/// <summary>
	/// 行程情報の種別を取得する条件を取得
	/// </summary>
	/// <param name="crsKinds"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	private string getKind_ItineraryConditions(CrsKinds crsKinds)
	{

		string returnValue = string.Empty;

		//If _コース種別1 = コース種別1タイプ.企画旅行 Then
		//    If _コース種別2 = コース種別2タイプ.日帰り Then
		//        returnValue = "CODE_VALUE NOT IN (" & 仕入先種別_宿泊 & ")"
		//    End If

		//Else
		//    If _コース種別2 = コース種別2タイプ.日帰り Then
		//        returnValue = "CODE_VALUE NOT IN (" & 仕入先種別_宿泊 & "," & 仕入先種別_キャリア & ")"
		//    Else
		//        returnValue = "CODE_VALUE NOT IN (" & 仕入先種別_キャリア & ")"
		//    End If
		//End If

		if (crsKinds.kikaku == true) //If _crsKind1 = CrsKind1Type.kikakuTravel Then
		{

			if (crsKinds.stay == false)
			{
				// [宿泊] 以外
				return "CODE_VALUE NOT IN (" + SuppliersKind_Stay + ")";
			}

		}
		else
		{

			// [定期]時、コース種別2(crsKind2) が空白 Ｒコース の場合、[日帰り]と同様にする
			if (crsKinds.stay == false)
			{
				// [宿泊] 以外
				return "CODE_VALUE NOT IN (" + SuppliersKind_Stay + "," + SuppliersKind_Carrier + ")";
			}
			else
			{
				return "CODE_VALUE NOT IN (" + SuppliersKind_Carrier + ")";
			}
		}

		return returnValue;
	}
	#endregion

	/// <summary>
	/// timeExコントロールの値を返します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public DateTime dateExValue(object obj)
	{

		DateTime returnValue = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");

		if (Information.IsDBNull(obj) || ReferenceEquals(obj, null) || ReferenceEquals(obj, string.Empty) ||)
		{
			System.Convert.ToString(obj) = YmdLiteral || System.Convert.ToString(obj) == JiBunLiteral || Information.IsDate(obj) == false;
			return System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");

			// 注意！ Nothing ではなく、#1/1/0001 12:00:00 AM# が返却される
		}
		else
		{
			return System.Convert.ToDateTime(obj);
		}

		return returnValue;
	}

	/// <summary>
	/// dateExコントロールの値を返します。
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public string dateExText(object obj)
	{
		string returnText = string.Empty;

		if (Information.IsDBNull(obj) || ReferenceEquals(obj, null) || ReferenceEquals(obj, string.Empty) ||)
		{
			System.Convert.ToString(obj) = YmdLiteral || System.Convert.ToString(obj) == JiBunLiteral;
			returnText = string.Empty;
		}
		else
		{
			returnText = System.Convert.ToString(obj);
		}

		return returnText;
	}

	/// <summary>
	/// 時刻範囲チェック
	/// </summary>
	/// <param name="timeValue"></param>
	/// <returns></returns>
	public bool isTimeRangeCheck(string timeValue)
	{

		// timeValue 書式は [mm:ss] であること

		// 空文字の場合、処理を抜ける（チェックなしのため戻り値：True)
		if (string.IsNullOrEmpty(timeValue))
		{
			return true;
		}

		DateTime comparisonMoto = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		int comparisonErrorCd = 1;

		DateTime timeValueMin = dateExValue(timeHour + TimeMin); //00:00:01
		DateTime timeValueMax = dateExValue(timeHour + TimeMax); //00:27:59

		// 時:分:秒
		comparisonMoto = dateExValue(timeHour + timeValue);

		if ((DateTime.Compare(timeValueMin, comparisonMoto) == comparisonErrorCd) || (DateTime.Compare(comparisonMoto, timeValueMax) == comparisonErrorCd))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 時刻順チェック
	/// </summary>
	/// <param name="timeValueMoto"></param>
	/// <param name="timeValueSaki"></param>
	/// <returns></returns>
	public bool isTimeOrderCheck(string timeValueMoto, string timeValueSaki)
	{

		// timeValueMoto,timeValueSaki 書式は [mm:ss] であること

		// 空文字の場合、処理を抜ける（チェックなしのため戻り値：True)
		if (string.IsNullOrEmpty(timeValueMoto))
		{
			return true;
		}

		// 空文字の場合、処理を抜ける（チェックなしのため戻り値：True)
		if (string.IsNullOrEmpty(timeValueSaki))
		{
			return true;
		}

		DateTime comparisonMoto = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		DateTime comparisonSaki = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		int comparisonErrorCd = 0;

		// 時:分:秒
		comparisonMoto = dateExValue(timeHour + timeValueMoto);
		comparisonSaki = dateExValue(timeHour + timeValueSaki);

		// [時間元 < 時間先] 以外はエラー
		if (DateTime.Compare(comparisonMoto, comparisonSaki) >= comparisonErrorCd)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 指定時間が最大時間を超えている場合、最大時間を返す
	/// </summary>
	/// <param name="timeValue"></param>
	/// <returns></returns>
	public string getMaxTimeCheck(string timeValue)
	{

		string ret = timeValue;

		DateTime comparisonMoto = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		int comparisonErrorCd = 1;

		DateTime timeValueMax = dateExValue(timeHour + TimeMax); //00:27:59

		// 時:分:秒
		comparisonMoto = dateExValue(timeHour + timeValue);

		// 指定時間が最大時間を超えているか判定
		if (DateTime.Compare(comparisonMoto, timeValueMax) == comparisonErrorCd)
		{
			return TimeMax;
		}

		return ret;
	}

	/// <summary>
	/// 日次入力文字チェック（1～9と、バックスペース以外の時は、False を返す ※呼び出し元で入力不可制御する)
	/// </summary>
	/// <param name="keyChar"></param>
	/// <returns></returns>
	public bool isDailyInputCharCheck(char keyChar)
	{

		// 1～9と、バックスペース以外の時は、False を返す
		if ((keyChar < '1' || '9' < keyChar) &&)
		{
			keyChar(!= ControlChars.Back);
			return false;
		}

		return true;
	}

	#region [バス指定]コード取得 ([バス指定]が空白の場合、コースコードを返す)
	/// <summary>
	/// [バス指定]コード取得 ([バス指定]が空白の場合、コースコードを返す)
	/// </summary>
	/// <param name="busReserveCd"></param>
	/// <param name="crsCd"></param>
	/// <param name="crsKinds"></param>
	/// <param name="syuptJiCarrierKbn"></param>
	public string getEditAutoBusReserveCd(string busReserveCd, string crsCd, CrsKinds crsKinds, string syuptJiCarrierKbn)
	{

		// [バス指定]が空白 以外の場合、処理を抜ける
		if (string.IsNullOrWhiteSpace(busReserveCd) == false)
		{
			return busReserveCd;
		}

		// (定期観光 または 企画旅行) 以外の場合、処理を抜ける
		if (crsKinds.teikiKikaku == false)
		{
			return busReserveCd;
		}

		// [出発時キャリア=バス] 以外の場合、処理を抜ける
		if (syuptJiCarrierKbn.Equals(System.Convert.ToString(SyuptJiCarrierKbnType.bus)) == false)
		{
			return busReserveCd;
		}

		// コースコードを返す
		return crsCd;

	}
	#endregion

}


#region 列挙（工程情報）
/// <summary>
/// 行程情報グリッドカラム
/// </summary>
/// <remarks></remarks>
public enum ItineraryGridColType : int
{
	[Value("No")]
	colLine_no = 0,
	[Value("日")]
	colDaily,
	[Value("種別")]
	colKind,
	[Value("降車ヶ所")]
	colKousya_place,
	[Value("精算目的コード")]
	colSeisanMokutekiCd,
	[Value("精算目的")]
	colSeisanMokutekiName,
	[Value("文字色")]
	colMoji,
	[Value("着時刻")]
	colTyak_time,
	[Value("発時刻")]
	colHatu_time,
	[Value("滞在")]
	colTaizai_time,
	[Value("記号")]
	colMark,
	[Value("バウチャー要")]
	colVoucher,
	[Value("原価")]
	colCost_Umu,
	[Value("食事")]
	colMeal,
	[Value("宿泊")]
	colStay,
	[Value("備考")]
	colBiko,
	[Value("キャリア出発")]
	colSyuptPlaceCd_Carrier,
	[Value("キャリア到着")]
	colTtyakPlaceCd_Carrier,
	[Value("キャリア便名")]
	colBinName,
	[Value("目的")]
	colMokuteki,
	colTotal
}

/// <summary>
/// 子画面呼出元タイプ
/// </summary>
/// <remarks></remarks>
public enum KoGamenCallMotoType : int
{
	itinerary,
	sonota_Itinerary,
	carrier
}

/// <summary>
/// 仕入先種別＋α
/// </summary>
/// <remarks></remarks>
public enum SiireKindPlus : int
{
	[Value("車窓")]
	syaso = 990,
	[Value("場所")]
	place = 999
}

#endregion