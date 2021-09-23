using Hatobus.ReservationManagementSystem.Common;
using System.ComponentModel;



/// <summary>
/// ピックアップホテルマスタ照会・管理
/// </summary>
public class S01_0502 : PT71, iPT71
{
	public S01_0502()
	{
		UpdateEntity = new EntityOperation(Of PickupHotelEntity);

	}

	#region  定数／変数宣言
	//更新対象エンティティ
	//TODO:Ofのあとのエンティティクラスとキー配列を変更(変更必須)
	private EntityOperation UpdateEntity;
	private string[] EntityKeys = new string[] { "PICKUP_HOTEL_CD" };

	//更新時のメッセージ
	private string UpdateMsg = "PUホテルコード";
	private string InsertMsg = "PUホテルコード";

	/// <summary>
	/// ＤＢ問合せ区分
	/// </summary>
	/// <remarks></remarks>
	protected enum DbTBLKbn : int
	{
		@Select,
		Key,
		HotelNameJyosyaTi,
		HotelNameJyosyaTi_Update
	}

	#endregion

	#region イベント
	//画面共通的なイベントはパターンに実装し、各画面はAddhandlerのみの方がすっきりする(※新規ボタンなど参照)
	#region グリッド関連
	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterSort(object sender, C1.Win.C1FlexGrid.SortColEventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// 行選択時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_RowColChange(object sender, System.EventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		base.displayMainGridCount();
	}

	#endregion

	#region フォーム
	private void PT71_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			base.btnCom_Click(this.btnSearch, e);
		}
		else
		{
			return;
		}
	}
	#endregion

	#endregion

	#region PT71オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		CommonUtil.Control_Init(this.gbxInquiryItem.Controls);
		//背景色初期化はここ
		//フォーマット設定
		//削除を除くをON
		this.chkDeleteWithoutSearch.Checked = true;
		this.txtInqPuHotelCd.Format = CmmFormatType.HALF_ALPNUM_B;
		this.txtInqHotelNameJyosyaTi.Format = CmmFormatType.NONE;
	}

	/// <summary>
	/// 更新対象エリアの項目初期化
	/// </summary>
	protected override void initUpdateAreaItems()
	{
		CommonUtil.Control_Init(this.gbxUpdateItem.Controls);
		//背景色初期化はここ
		base.clearExistErrorProperty(this.gbxUpdateItem.Controls);

		grdList.Cols.Frozen = 4;

		this.txtUpdPuHotelCd.Format = CmmFormatType.HALF_ALPNUM_B;
		this.txtUpdHotelNameJyosyaTi.Format = CmmFormatType.FULL;
		this.txtUpdHotelNameJyosyaTiAlph.Format = CmmFormatType.HALF_ALPNUM_A;
		this.txtRk.Format = CmmFormatType.HALF_ALPNUM_B;
		this.txtPrint.Format = CmmFormatType.FULL;
		this.txtSyugoPlace.Format = CmmFormatType.FULL;
		this.txtSyugoPlaceAlph.Format = CmmFormatType.HALF_ALPNUM_A;

	}

	/// <summary>
	/// エンティティ初期化
	/// </summary>
	protected override void initEntityData()
	{
		UpdateEntity.clear();
	}
	#endregion

	#region 変更確認
	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		//差分チェック
		return UpdateEntity.compare(0);
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{
		return CheckSearch();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkUpdateItems()
	{
		return CheckUpdate();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		return CheckInsert();
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//フォーカスセット
		setSeFirsttDisplayData();

		//ベースフォームの初期化処理
		base.mainGrid = this.grdList;
		base.mainGridDataCntDsp = this.lblLengthGrd;

		//ベースフォームの初期化処理
		base.initScreenPerttern();

		//検索ボタンの関連付け
		btnSearch.Click += base.btnCom_Click;
		btnClear.Click += base.btnCom_Click;
		btnNewAdd.Click += PT71_btnNewAdd_Click;

		base.UpdateMsgParam = this.UpdateMsg;
		base.InsertMsgParam = this.InsertMsg;
	}

	/// <summary>
	/// 初期処理
	/// </summary>
	protected override void initDetailAreaItems()
	{
		//行選択モード
		this.grdList.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
		this.grdList.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.None;
	}

	#endregion

	#region 画面->エンティティ
	/// <summary>
	/// 更新対象項目をエンティティにセット
	/// </summary>
	protected override void setEntityDataValue()
	{


		//エンティティの初期化
		this.UpdateEntity.clear(0, Common.clearType.value);
		this.UpdateEntity.clear(0, Common.clearType.errorInfo);

		object temp_ent = TryCast(this.UpdateEntity.EntityData(0), PickupHotelEntity);
		DisplayDataToEntity(ref temp_ent);
	}
	#endregion

	#region Grid、データ関連
	/// <summary>
	/// 選択行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override void getSelectedRowData()
	{
		//選択された行データ
		DataRow[] selectData = null;
		//問合せ文字列
		string whereString = string.Empty;

		//更新対象項目の初期化
		initUpdateAreaItems();

		//引数の設定
		whereString = System.Convert.ToString(MakeWhere(grdList, EntityKeys));

		//問合せ対象データ取得
		selectData = base.SearchResultGridData.Select(whereString);

		if (selectData.Length > 0)
		{
			//取得データをEntityの前回値にセット
			OldDataToEntity(selectData[0]);
			object temp_ent = TryCast(this.UpdateEntity.EntityData(0), PickupHotelEntity);
			EntityDataToDisplay(ref temp_ent);
			//更新ボタンを使用可に設定
			this.F11Key_Enabled = true;
		}
		else
		{
			//Entityの初期化
			initEntityData();
			//更新対象エリアの項目初期化
			initUpdateAreaItems();
			//更新ボタンを使用不可に設定
			this.F11Key_Enabled = false;
		}
	}

	/// <summary>
	/// 更新エンティティ初期化(処理的には不明)
	/// </summary>
	protected override void initUpdateEntity()
	{
		UpdateEntity.clear(0, Common.clearType.value);
		UpdateEntity.clear(0, Common.clearType.errorInfo);
	}
	#endregion

	#region DB関連
	/// <summary>
	/// マスタデータ登録
	/// </summary>
	protected override int ExecuteInsertMaster()
	{
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		PickupHotel_DA dataAccess = new PickupHotel_DA();

		//パラメータ作成(共通部)
		CommonLogic.setTableCommonInfo(DbShoriKbn.Insert, this.Name, TryCast(UpdateEntity.EntityData(0), PickupHotelEntity));
		paramInfoList = entityToHash(UpdateEntity);

		try
		{
			//Insertの実施
			returnValue = System.Convert.ToInt32(dataAccess.executePickupHotelMaster(PickupHotel_DA.accessType.executeInsertPickupHotel, paramInfoList));
		}
		catch (OracleException ex)
		{
			throw (ex);
		}
		catch (Exception ex)
		{
			throw (ex);
		}

		return returnValue;
	}

	/// <summary>
	/// マスタデータ更新
	/// </summary>
	protected override int ExecuteUpdateMaster()
	{
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//TODO:DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//呼ばれる時点で、UpdateEntit.EntityData(0)には画面の値は設定されている
		//DataAccessクラス生成
		PickupHotel_DA dataAccess = new PickupHotel_DA();

		//パラメータ作成(共通部)
		CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, TryCast(UpdateEntity.EntityData(0), PickupHotelEntity));
		paramInfoList = entityToHash(UpdateEntity);

		try
		{
			//Updateの実施
			returnValue = System.Convert.ToInt32(dataAccess.executePickupHotelMaster(PickupHotel_DA.accessType.executeUpdatePickupHotel, paramInfoList));
		}
		catch (OracleException ex)
		{
			throw (ex);
		}
		catch (Exception ex)
		{
			throw (ex);
		}

		return returnValue;
	}

	/// <summary>
	/// キー値での問合せ
	/// </summary>
	protected override DataTable getMstDataByPrimaryKey()
	{
		return GetDbTable(DbTBLKbn.Key);
	}

	/// <summary>
	/// ホテル名・乗車地での問合せ
	/// </summary>
	protected DataTable getMstDataByHotelNameJyosyaTi()
	{
		return GetDbTable(DbTBLKbn.HotelNameJyosyaTi);
	}

	/// <summary>
	/// ホテル名・乗車地での問合せ（更新時）※自データは除く
	/// </summary>
	protected DataTable getMstDataByHotelNameJyosyaTi_Update()
	{
		return GetDbTable(DbTBLKbn.HotelNameJyosyaTi_Update);
	}


	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getMstData()
	{
		return GetDbTable(DbTBLKbn.Select);
	}
	#endregion
	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//TODO:初期値設定処理(コンボボックスの値設定等も含む)

		//TODO:初期フォーカスのコントロールを設定を実装
		this.txtInqPuHotelCd.Select();
	}
	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	public void DisplayDataToEntity(ref object ent)
	{
		if (typeof(PickupHotelEntity).Equals(ent.GetType()))
		{
			PickupHotelEntity with_1 = ((PickupHotelEntity)ent);
			//TODO:画面のデータをエンティティに設定する処理を実装
			with_1.PickupHotelCd.Value = this.txtUpdPuHotelCd.Text;
			with_1.HotelNameJyosyaTi.Value = this.txtUpdHotelNameJyosyaTi.Text;
			with_1.Rk.Value = this.txtRk.Text;
			with_1.Print.Value = this.txtPrint.Text;
			with_1.SyugoPlace.Value = this.txtSyugoPlace.Text;
			with_1.HotelNameJyosyaTiAlph.Value = this.txtUpdHotelNameJyosyaTiAlph.Text;
			with_1.SyugoPlaceAlph.Value = this.txtSyugoPlaceAlph.Text;
			//削除フラグ
			if (this.chbDelete.Checked == true)
			{
				//ONの場合
				with_1.DeleteDate.Value = getDateTime().ToString("yyyyMMdd"); //＊＊＊サーバ日付に変更の可能性あり＊＊＊
			}
			else
			{
				//OFFの場合
				with_1.DeleteDate.Value = string.Empty;
			}
		}
	}

	/// <summary>
	/// エンティティから画面に設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	public void EntityDataToDisplay(ref object ent)
	{
		//それぞれの画面エンティティにキャスト
		if (typeof(PickupHotelEntity).Equals(ent.GetType()))
		{
			PickupHotelEntity with_1 = ((PickupHotelEntity)ent);
			//TODO:エンティティのデータを画面に設定する処理を実装
			this.txtUpdPuHotelCd.Text = with_1.PickupHotelCd.ZenkaiValue;
			this.txtUpdHotelNameJyosyaTi.Text = with_1.HotelNameJyosyaTi.ZenkaiValue;
			this.txtRk.Text = with_1.Rk.ZenkaiValue;
			this.txtPrint.Text = with_1.Print.ZenkaiValue;
			this.txtSyugoPlace.Text = with_1.SyugoPlace.ZenkaiValue;
			this.txtUpdHotelNameJyosyaTiAlph.Text = with_1.HotelNameJyosyaTiAlph.ZenkaiValue;
			this.txtSyugoPlaceAlph.Text = with_1.SyugoPlaceAlph.ZenkaiValue;
			//削除チェックボックス
			bool isDeleted = false;
			if (!string.IsNullOrEmpty(System.Convert.ToString(with_1.DeleteDate.ZenkaiValue)))
			{
				//削除日が設定されている場合、ONにする
				isDeleted = true;
			}
			this.chbDelete.Checked = isDeleted;
		}
	}

	/// <summary>
	/// DataGridからエンティティ(前回値)に設定する処理(必須画面個別実装)
	/// ※DataGrid上の1レコードから関連するデータも取得する。Keyがない場合などは未対応
	/// </summary>
	/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
	/// <remarks></remarks>
	public void OldDataToEntity(DataRow pDataRow)
	{
		//TODO:Grid上の選択データ(pDataRow)をエンティティに設定する処理を実装
		//エンティティの初期化
		this.UpdateEntity.clear();

		//取得データが設定されている
		object with_2 = this.UpdateEntity.EntityData(0);
		with_2.PickupHotelCd.ZenkaiValue = pDataRow.Item(with_2.PickupHotelCd.PhysicsName).ToString();
		with_2.HotelNameJyosyaTi.ZenkaiValue = pDataRow.Item(with_2.HotelNameJyosyaTi.PhysicsName).ToString();
		with_2.Rk.ZenkaiValue = pDataRow.Item(with_2.Rk.PhysicsName).ToString();
		with_2.Print.ZenkaiValue = pDataRow.Item(with_2.Print.PhysicsName).ToString();
		with_2.SyugoPlace.ZenkaiValue = pDataRow.Item(with_2.SyugoPlace.PhysicsName).ToString();
		with_2.HotelNameJyosyaTiAlph.ZenkaiValue = pDataRow.Item(with_2.HotelNameJyosyaTiAlph.PhysicsName).ToString();
		with_2.SyugoPlaceAlph.ZenkaiValue = pDataRow.Item(with_2.SyugoPlaceAlph.PhysicsName).ToString();
		//削除日
		with_2.DeleteDate.ZenkaiValue = pDataRow.Item(with_2.DeleteDate.PhysicsName).ToString();
		// 前回値設定実績あり
		base.setZenkaiValueFlg = true;
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		//TODO:検索処理前のチェック処理を実装(メッセージは画面側でしてください)
		return true;
	}

	/// <summary>
	/// 登録処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckInsert()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxUpdateItem.Controls);

		//必須項目のチェック
		if (this.isExistHissuError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}
		//重複キーチェック
		return this.checkOverlappingData();

	}

	/// <summary>
	/// 更新処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckUpdate()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxUpdateItem.Controls);

		//必須項目のチェック
		if (this.isExistHissuError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		// [削除]チェックボックスが OFF の場合のみ
		if (this.chbDelete.Checked == false)
		{
			//重複チェック（更新時）
			if (this.checkOverlappingData_Update() == false)
			{
				return false;
			}
		}

		//プライマリーキー項目の変更確認
		return this.checkChangePk();

	}

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	public bool isExistHissuError()
	{

		bool returnValue = false;


		//エラー情報のクリア
		UpdateEntity.clear(0, Common.clearType.errorInfo);

		//テキストボックスの色を初期化
		base.clearExistError();

		//必須プロパティを付与
		return CommonUtil.checkHissuError(this.gbxUpdateItem.Controls);


		return returnValue;

	}

	/// <summary>
	/// 重複チェック
	/// </summary>
	private bool checkOverlappingData()
	{

		bool returnValue = true;

		//テキストボックスの色を初期化
		base.clearExistError();

		//既存データとのキー重複チェック
		if (getMstDataByPrimaryKey().Rows.Count > 0)
		{
			txtUpdPuHotelCd.ExistError = true;
			txtUpdPuHotelCd.Select();

			CommonProcess.createFactoryMsg().messageDisp("E90_020", "PUホテルコード");
			return false;
			return returnValue;
		}

		//既存データとのホテル名・乗車地重複チェック
		if (getMstDataByHotelNameJyosyaTi().Rows.Count > 0)
		{
			txtUpdHotelNameJyosyaTi.ExistError = true;
			txtUpdHotelNameJyosyaTi.Select();

			CommonProcess.createFactoryMsg().messageDisp("E90_020", "ホテル名・乗車地");
			return false;
			return returnValue;
		}

		return returnValue;

	}

	/// <summary>
	/// 重複チェック（更新時）
	/// </summary>
	/// <returns></returns>
	private bool checkOverlappingData_Update()
	{

		bool returnValue = true;

		//テキストボックスの色を初期化
		base.clearExistError();

		//既存データとのホテル名・乗車地重複チェック ※自データは除く
		if (getMstDataByHotelNameJyosyaTi_Update().Rows.Count > 0)
		{
			txtUpdHotelNameJyosyaTi.ExistError = true;
			txtUpdHotelNameJyosyaTi.Select();

			CommonProcess.createFactoryMsg().messageDisp("E90_020", "ホテル名・乗車地");
			return false;
		}

		return returnValue;

	}

	/// <summary>
	/// プライマリーキー項目の変更確認
	/// </summary>
	private bool checkChangePk()
	{

		bool returnValue = true;

		object with_1 = UpdateEntity.EntityData(0);
		if (with_1.PickupHotelCd.ZenkaiValue != with_1.PickupHotelCd.Value)
		{
			this.txtUpdPuHotelCd.ExistError = true;
			this.txtUpdPuHotelCd.Select();
			CommonProcess.createFactoryMsg().messageDisp("E90_015", "PUホテルコード");
			returnValue = false;
		}

		return returnValue;
	}
	#endregion
	#endregion

	#region DB取得処理

	/// <summary>
	/// データ取得処理(必須画面個別実装)
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTable(DbTBLKbn kbn)
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//TODO:DAクラス作成、パラメータ設定、選択処理実施までを実装
		//DataAccessクラス生成
		PickupHotel_DA dataAccess = new PickupHotel_DA();

		//パラメータ設定
		string pickupHotelCd = string.Empty;
		string hotelNameJyosyaTi = string.Empty;
		if (DbTBLKbn.Select.Equals(kbn))
		{
			pickupHotelCd = System.Convert.ToString(this.txtInqPuHotelCd.Text);
			hotelNameJyosyaTi = System.Convert.ToString(this.txtInqHotelNameJyosyaTi.Text);
		}
		else
		{
			pickupHotelCd = System.Convert.ToString(this.txtUpdPuHotelCd.Text);
			hotelNameJyosyaTi = System.Convert.ToString(this.txtUpdHotelNameJyosyaTi.Text);
		}
		if (!string.IsNullOrEmpty(pickupHotelCd))
		{
			paramInfoList.Add(UpdateEntity.EntityData(0).PickupHotelCd.PhysicsName, pickupHotelCd);
		}
		if (!string.IsNullOrEmpty(hotelNameJyosyaTi))
		{
			paramInfoList.Add(UpdateEntity.EntityData(0).HotelNameJyosyaTi.PhysicsName, hotelNameJyosyaTi);
		}
		if (this.chkDeleteWithoutSearch.Checked == true)
		{
			//削除を除くチェックボックス
			paramInfoList.Add(PickupHotel_DA.ParamKeys.deleteChk, CommonProcess.getCheckedValue(this.chkDeleteWithoutSearch.Checked));
		}
		try
		{
			if (DbTBLKbn.Select.Equals(kbn))
			{
				returnValue = dataAccess.accessPickupHotelMaster(PickupHotel_DA.accessType.getPickupHotel, paramInfoList);
			}
			else if (DbTBLKbn.Key.Equals(kbn))
			{
				returnValue = dataAccess.accessPickupHotelMaster(PickupHotel_DA.accessType.getPickupHotelDataByPrimaryKey, paramInfoList);
			}
			else if (DbTBLKbn.HotelNameJyosyaTi.Equals(kbn))
			{
				returnValue = dataAccess.accessPickupHotelMaster(PickupHotel_DA.accessType.getPickupHotelDataByHotelNameJyosyaTi, paramInfoList);
			}
			else if (DbTBLKbn.HotelNameJyosyaTi_Update.Equals(kbn))
			{
				returnValue = dataAccess.accessPickupHotelMaster(PickupHotel_DA.accessType.getPickupHotelDataByHotelNameJyosyaTi_Update, paramInfoList);
			}
		}
		catch (OracleException ex)
		{
			throw (ex);
		}
		catch (Exception ex)
		{
			throw (ex);
		}

		return returnValue;

	}

	#endregion

	/// <summary>
	/// エンティティデータをハッシュテーブルテーブル に変換
	/// </summary>
	/// <param name="headEnt"></param>
	/// <returns></returns>
	public Hashtable entityToHash(object headEnt)
	{
		Hashtable hash = new Hashtable();
		EntityOperation[] ent = new EntityOperation[Of PickupHotelEntity + 1];
		ent = (EntityOperation[OfPickupHotelEntity]) headEnt;
		//カラム情報作成
		for (int idx = 0; idx <= ent.getPropertyDataLength - 1; idx++)
		{
			if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).GetType, typeof(EntityKoumoku_MojiType)))
			{
				hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).PhysicsName, ((EntityKoumoku_MojiType)(ent.getPtyValue(idx, ent.EntityData(0)))).Value);
			}
			else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).GetType, typeof(EntityKoumoku_NumberType)))
			{
				hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).PhysicsName, ((EntityKoumoku_NumberType)(ent.getPtyValue(idx, ent.EntityData(0)))).Value);
			}
			else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).GetType, typeof(EntityKoumoku_YmdType)))
			{
				hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).PhysicsName, ((EntityKoumoku_YmdType)(ent.getPtyValue(idx, ent.EntityData(0)))).Value);
			}
			else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).GetType, typeof(EntityKoumoku_Number_DecimalType)))
			{
				hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(0)))).PhysicsName, ((EntityKoumoku_Number_DecimalType)(ent.getPtyValue(idx, ent.EntityData(0)))).Value);
			}
		}
		return hash;
	}
	#endregion

}