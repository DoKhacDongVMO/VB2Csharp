Imports Oracle.ManagedDataAccess.Client


''' <summary>
''' コース台帳（基本）
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TCrsLedgerBasicEntity  ' コース台帳（基本）エンティティ


Private _crsCd As New EntityKoumoku_MojiType	' コースコード
Private _syuptDay As New EntityKoumoku_NumberType	' 出発日
Private _gousya As New EntityKoumoku_NumberType	' 号車
Private _accessCd As New EntityKoumoku_MojiType	' アクセスコード
Private _aibeyaUseFlg As New EntityKoumoku_MojiType	' 相部屋使用フラグ
Private _aibeyaYoyakuNinzuJyosei As New EntityKoumoku_NumberType	' 相部屋予約人数女性
Private _aibeyaYoyakuNinzuMale As New EntityKoumoku_NumberType	' 相部屋予約人数男性
Private _binName As New EntityKoumoku_MojiType	' 便名
Private _blockKakuhoNum As New EntityKoumoku_NumberType	' ブロック確保数
Private _busCompanyCd As New EntityKoumoku_MojiType	' バス会社コード
Private _busReserveCd As New EntityKoumoku_MojiType	' バス指定コード
Private _cancelNgFlg As New EntityKoumoku_MojiType	' キャンセル不可フラグ
Private _cancelRyouKbn As New EntityKoumoku_MojiType	' キャンセル料区分
Private _cancelWaitNinzu As New EntityKoumoku_NumberType	' キャンセル待ち人数
Private _capacityHo1kai As New EntityKoumoku_NumberType	' 定員補１階
Private _capacityRegular As New EntityKoumoku_NumberType	' 定員定
Private _carrierCd As New EntityKoumoku_MojiType	' キャリアコード
Private _carrierEdaban As New EntityKoumoku_MojiType	' キャリア枝番
Private _carNo As New EntityKoumoku_NumberType	' 車番
Private _carTypeCd As New EntityKoumoku_MojiType	' 車種コード
Private _carTypeCdYotei As New EntityKoumoku_MojiType	' 車種コード予定
Private _busCountFlg As New EntityKoumoku_MojiType	' 台数カウントフラグ
Private _categoryCd1 As New EntityKoumoku_MojiType	' カテゴリーコード１
Private _categoryCd2 As New EntityKoumoku_MojiType	' カテゴリーコード２
Private _categoryCd3 As New EntityKoumoku_MojiType	' カテゴリーコード３
Private _categoryCd4 As New EntityKoumoku_MojiType	' カテゴリーコード４
Private _costSetKbn As New EntityKoumoku_MojiType	' 原価設定区分
Private _crsBlockCapacity As New EntityKoumoku_NumberType	' コースブロック定員
Private _crsBlockOne1r As New EntityKoumoku_NumberType	' コースブロック１名１Ｒ
Private _crsBlockRoomNum As New EntityKoumoku_NumberType	' コースブロックルーム数
Private _crsBlockThree1r As New EntityKoumoku_NumberType	' コースブロック３名１Ｒ
Private _crsBlockTwo1r As New EntityKoumoku_NumberType	' コースブロック２名１Ｒ
Private _crsBlockFour1r As New EntityKoumoku_NumberType	' コースブロック４名１Ｒ
Private _crsBlockFive1r As New EntityKoumoku_NumberType	' コースブロック５名１Ｒ
Private _crsKbn1 As New EntityKoumoku_MojiType	' コース区分１
Private _crsKbn2 As New EntityKoumoku_MojiType	' コース区分２
Private _crsKind As New EntityKoumoku_MojiType	' コース種別
Private _managementSec As New EntityKoumoku_MojiType	' 取扱部署
Private _guideGengo As New EntityKoumoku_MojiType	' ガイド言語
Private _crsName As New EntityKoumoku_MojiType	' コース名
Private _crsNameKana As New EntityKoumoku_MojiType	' コース名カナ
Private _crsNameRk As New EntityKoumoku_MojiType	' コース名略称
Private _crsNameKanaRk As New EntityKoumoku_MojiType	' コース名カナ略称
Private _deleteDay As New EntityKoumoku_NumberType	' 削除日
Private _eiBlockHo As New EntityKoumoku_NumberType	' 営ブロック補
Private _eiBlockRegular As New EntityKoumoku_NumberType	' 営ブロック定
Private _endPlaceCd As New EntityKoumoku_MojiType	' 終了場所コード
Private _endTime As New EntityKoumoku_NumberType	' 終了時間
Private _haisyaKeiyuCd1 As New EntityKoumoku_MojiType	' 配車経由コード１
Private _haisyaKeiyuCd2 As New EntityKoumoku_MojiType	' 配車経由コード２
Private _haisyaKeiyuCd3 As New EntityKoumoku_MojiType	' 配車経由コード３
Private _haisyaKeiyuCd4 As New EntityKoumoku_MojiType	' 配車経由コード４
Private _haisyaKeiyuCd5 As New EntityKoumoku_MojiType	' 配車経由コード５
Private _homenCd As New EntityKoumoku_MojiType	' 方面コード
Private _houjinGaikyakuKbn As New EntityKoumoku_MojiType	' 邦人／外客区分
Private _hurikomiNgFlg As New EntityKoumoku_MojiType	' 振込不可フラグ
Private _itineraryTableCreateFlg As New EntityKoumoku_MojiType	' 行程表作成フラグ
Private _jyoseiSenyoSeatFlg As New EntityKoumoku_MojiType	' 女性専用席フラグ
Private _jyosyaCapacity As New EntityKoumoku_NumberType	' 乗車定員
Private _kaiteiDay As New EntityKoumoku_NumberType	' 改定日
Private _kusekiKakuhoNum As New EntityKoumoku_NumberType	' 空席確保数
Private _kusekiNumSubSeat As New EntityKoumoku_NumberType	' 空席数補助席
Private _kusekiNumTeiseki As New EntityKoumoku_NumberType	' 空席数定席
Private _kyosaiUnkouKbn As New EntityKoumoku_MojiType	' 共催運行区分
Private _maeuriKigen As New EntityKoumoku_MojiType	' 前売期限
Private _maruZouManagementKbn As New EntityKoumoku_MojiType	' ○増管理区分
Private _mealCountMorning As New EntityKoumoku_NumberType	' 食事回数朝
Private _mealCountNight As New EntityKoumoku_NumberType	' 食事回数夜
Private _mealCountNoon As New EntityKoumoku_NumberType	' 食事回数昼
Private _mediaCheckFlg As New EntityKoumoku_MojiType	' 媒体チェックフラグ
Private _meiboInputFlg As New EntityKoumoku_MojiType	' 名簿入力フラグ
Private _ninzuInputFlgKeiyu1 As New EntityKoumoku_MojiType	' 乗車人数入力済フラグ配車経由１
Private _ninzuInputFlgKeiyu2 As New EntityKoumoku_MojiType	' 乗車人数入力済フラグ配車経由２
Private _ninzuInputFlgKeiyu3 As New EntityKoumoku_MojiType	' 乗車人数入力済フラグ配車経由３
Private _ninzuInputFlgKeiyu4 As New EntityKoumoku_MojiType	' 乗車人数入力済フラグ配車経由４
Private _ninzuInputFlgKeiyu5 As New EntityKoumoku_MojiType	' 乗車人数入力済フラグ配車経由５
Private _ninzuKeiyu1Adult As New EntityKoumoku_NumberType	' 乗車人数配車経由１大人
Private _ninzuKeiyu1Child As New EntityKoumoku_NumberType	' 乗車人数配車経由１小人
Private _ninzuKeiyu1Junior As New EntityKoumoku_NumberType	' 乗車人数配車経由１中人
Private _ninzuKeiyu1S As New EntityKoumoku_NumberType	' 乗車人数配車経由１招
Private _ninzuKeiyu2Adult As New EntityKoumoku_NumberType	' 乗車人数配車経由２大人
Private _ninzuKeiyu2Child As New EntityKoumoku_NumberType	' 乗車人数配車経由２小人
Private _ninzuKeiyu2Junior As New EntityKoumoku_NumberType	' 乗車人数配車経由２中人
Private _ninzuKeiyu2S As New EntityKoumoku_NumberType	' 乗車人数配車経由２招
Private _ninzuKeiyu3Adult As New EntityKoumoku_NumberType	' 乗車人数配車経由３大人
Private _ninzuKeiyu3Child As New EntityKoumoku_NumberType	' 乗車人数配車経由３小人
Private _ninzuKeiyu3Junior As New EntityKoumoku_NumberType	' 乗車人数配車経由３中人
Private _ninzuKeiyu3S As New EntityKoumoku_NumberType	' 乗車人数配車経由３招
Private _ninzuKeiyu4Adult As New EntityKoumoku_NumberType	' 乗車人数配車経由４大人
Private _ninzuKeiyu4Child As New EntityKoumoku_NumberType	' 乗車人数配車経由４小人
Private _ninzuKeiyu4Junior As New EntityKoumoku_NumberType	' 乗車人数配車経由４中人
Private _ninzuKeiyu4S As New EntityKoumoku_NumberType	' 乗車人数配車経由４招
Private _ninzuKeiyu5Adult As New EntityKoumoku_NumberType	' 乗車人数配車経由５大人
Private _ninzuKeiyu5Child As New EntityKoumoku_NumberType	' 乗車人数配車経由５小人
Private _ninzuKeiyu5Junior As New EntityKoumoku_NumberType	' 乗車人数配車経由５中人
Private _ninzuKeiyu5S As New EntityKoumoku_NumberType	' 乗車人数配車経由５招
Private _oneSankaFlg As New EntityKoumoku_MojiType	' １名参加フラグ
Private _optionFlg As New EntityKoumoku_MojiType	' オプションフラグ
Private _pickupKbn1 As New EntityKoumoku_MojiType	' ピックアップ区分１
Private _pickupKbn2 As New EntityKoumoku_MojiType	' ピックアップ区分２
Private _pickupKbn3 As New EntityKoumoku_MojiType	' ピックアップ区分３
Private _pickupKbn4 As New EntityKoumoku_MojiType	' ピックアップ区分４
Private _pickupKbn5 As New EntityKoumoku_MojiType	' ピックアップ区分５
Private _returnDay As New EntityKoumoku_NumberType	' 帰着日
Private _roomZansuOneRoom As New EntityKoumoku_NumberType	' 部屋残数１人部屋
Private _roomZansuSokei As New EntityKoumoku_NumberType	' 部屋残数総計
Private _roomZansuThreeRoom As New EntityKoumoku_NumberType	' 部屋残数３人部屋
Private _roomZansuTwoRoom As New EntityKoumoku_NumberType	' 部屋残数２人部屋
Private _roomZansuFourRoom As New EntityKoumoku_NumberType	' 部屋残数４人部屋
Private _roomZansuFiveRoom As New EntityKoumoku_NumberType	' 部屋残数５人部屋
Private _minSaikouNinzu As New EntityKoumoku_NumberType	' 最小催行人数
Private _saikouDay As New EntityKoumoku_NumberType	' 催行日
Private _saikouKakuteiKbn As New EntityKoumoku_MojiType	' 催行確定区分
Private _seasonCd As New EntityKoumoku_MojiType	' 季コード
Private _senyoCrsKbn As New EntityKoumoku_MojiType	' 専用コース区分
Private _shanaiContactForMessage As New EntityKoumoku_MojiType	' 社内連絡用メッセージ
Private _shukujitsuFlg As New EntityKoumoku_MojiType	' 祝日フラグ
Private _sinsetuYm As New EntityKoumoku_NumberType	' 新設年月
Private _stayDay As New EntityKoumoku_NumberType	' 宿泊日
Private _stayStay As New EntityKoumoku_NumberType	' 宿泊泊
Private _subSeatOkKbn As New EntityKoumoku_MojiType	' 補助席可区分
Private _syoyoTime As New EntityKoumoku_NumberType	' 所要時間
Private _syugoPlaceCdCarrier As New EntityKoumoku_MojiType	' 集合場所コードキャリア
Private _syugoTime1 As New EntityKoumoku_NumberType	' 集合時間１
Private _syugoTime2 As New EntityKoumoku_NumberType	' 集合時間２
Private _syugoTime3 As New EntityKoumoku_NumberType	' 集合時間３
Private _syugoTime4 As New EntityKoumoku_NumberType	' 集合時間４
Private _syugoTime5 As New EntityKoumoku_NumberType	' 集合時間５
Private _syugoTimeCarrier As New EntityKoumoku_NumberType	' 集合時間キャリア
Private _syuptJiCarrierKbn As New EntityKoumoku_MojiType	' 出発時キャリア区分
Private _syuptPlaceCarrier As New EntityKoumoku_MojiType	' 出発場所キャリア
Private _syuptPlaceCdCarrier As New EntityKoumoku_MojiType	' 出発場所コードキャリア
Private _syuptTime1 As New EntityKoumoku_NumberType	' 出発時間１
Private _syuptTime2 As New EntityKoumoku_NumberType	' 出発時間２
Private _syuptTime3 As New EntityKoumoku_NumberType	' 出発時間３
Private _syuptTime4 As New EntityKoumoku_NumberType	' 出発時間４
Private _syuptTime5 As New EntityKoumoku_NumberType	' 出発時間５
Private _syuptTimeCarrier As New EntityKoumoku_NumberType	' 出発時間キャリア
Private _teiinseiFlg As New EntityKoumoku_MojiType	' 定員制フラグ
Private _teikiCrsKbn As New EntityKoumoku_MojiType	' 定期コース区分
Private _teikiKikakuKbn As New EntityKoumoku_MojiType	' 定期・企画区分
Private _tejimaiContactKbn As New EntityKoumoku_MojiType	' 手仕舞連絡区分
Private _tejimaiDay As New EntityKoumoku_NumberType	' 手仕舞日
Private _tejimaiKbn As New EntityKoumoku_MojiType	' 手仕舞区分
Private _tenjyoinCd As New EntityKoumoku_MojiType	' 添乗員コード
Private _tieTyakuyo As New EntityKoumoku_MojiType	' ネクタイ着用
Private _tokuteiChargeSet As New EntityKoumoku_MojiType	' 特定料金設定
Private _tokuteiDayFlg As New EntityKoumoku_MojiType	' 特定日フラグ
Private _ttyakPlaceCarrier As New EntityKoumoku_MojiType	' 到着場所キャリア
Private _ttyakPlaceCdCarrier As New EntityKoumoku_MojiType	' 到着場所コードキャリア
Private _ttyakTimeCarrier As New EntityKoumoku_NumberType	' 到着時間キャリア
Private _tyuijikou As New EntityKoumoku_MojiType	' 注意事項
Private _tyuijikouKbn As New EntityKoumoku_MojiType	' 注意事項区分
Private _uketukeGenteiNinzu As New EntityKoumoku_NumberType	' 受付限定人数
Private _uketukeStartBi As New EntityKoumoku_NumberType	' 受付開始日
Private _uketukeStartDay As New EntityKoumoku_NumberType	' 受付開始日
Private _uketukeStartKagetumae As New EntityKoumoku_MojiType	' 受付開始ヶ月前
Private _underKinsi18old As New EntityKoumoku_MojiType	' １８才未満禁
Private _unkyuContactDay As New EntityKoumoku_NumberType	' 運休連絡日
Private _unkyuContactDoneFlg As New EntityKoumoku_MojiType	' 運休連絡完了フラグ
Private _unkyuContactTime As New EntityKoumoku_NumberType	' 運休連絡時刻
Private _unkyuKbn As New EntityKoumoku_MojiType	' 運休区分
Private _tojituKokuchiFlg As New EntityKoumoku_NumberType	' 当日告知フラグ
Private _yusenYoyakuFlg As New EntityKoumoku_NumberType	' 優先予約フラグ
Private _pickupKbnFlg As New EntityKoumoku_NumberType	' ピックアップフラグ
Private _konjyoOkFlg As New EntityKoumoku_NumberType	' 混乗可フラグ
Private _tonariFlg As New EntityKoumoku_NumberType	' 隣席フラグ
Private _aheadZasekiFlg As New EntityKoumoku_NumberType	' 前方座席フラグ
Private _yoyakuMediaInputFlg As New EntityKoumoku_NumberType	' 予約媒体入力フラグ
Private _kokusekiFlg As New EntityKoumoku_NumberType	' 国籍入力フラグ
Private _sexBetuFlg As New EntityKoumoku_NumberType	' 性別入力フラグ
Private _ageFlg As New EntityKoumoku_NumberType	' 年齢フラグ
Private _birthdayFlg As New EntityKoumoku_NumberType	' 生年月日フラグ
Private _telFlg As New EntityKoumoku_NumberType	' 電話番号フラグ
Private _addressFlg As New EntityKoumoku_NumberType	' 住所フラグ
Private _usingFlg As New EntityKoumoku_MojiType	' 使用中フラグ
Private _uwagiTyakuyo As New EntityKoumoku_MojiType	' 上着着用
Private _year As New EntityKoumoku_NumberType	' 年
Private _yobiCd As New EntityKoumoku_MojiType	' 曜日コード
Private _yoyakuAlreadyRoomNum As New EntityKoumoku_NumberType	' 予約済ＲＯＯＭ数
Private _yoyakuKanouNum As New EntityKoumoku_NumberType	' 予約可能数
Private _yoyakuNgFlg As New EntityKoumoku_MojiType	' 予約不可フラグ
Private _yoyakuNumSubSeat As New EntityKoumoku_NumberType	' 予約数補助席
Private _yoyakuNumTeiseki As New EntityKoumoku_NumberType	' 予約数定席
Private _yoyakuStopFlg As New EntityKoumoku_MojiType	' 予約停止フラグ
Private _zouhatsumotogousya As New EntityKoumoku_NumberType	'増発元号車
Private _zouhatsuday As New EntityKoumoku_NumberType	'増発日
Private _zouhatsuentrypersoncd As New EntityKoumoku_MojiType	'増発実施者
Private _zasekiHihyojiFlg As New EntityKoumoku_MojiType	' 予約時座席非表示フラグ
Private _zasekiReserveKbn As New EntityKoumoku_MojiType	' 座席指定区分
Private _wtKakuhoSeatNum As New EntityKoumoku_NumberType	' ＷＴ確保席数
Private _systemEntryPgmid As New EntityKoumoku_MojiType	' システム登録ＰＧＭＩＤ
Private _systemEntryPersonCd As New EntityKoumoku_MojiType	' システム登録者コード
Private _systemEntryDay As New EntityKoumoku_YmdType	' システム登録日
Private _systemUpdatePgmid As New EntityKoumoku_MojiType	' システム更新ＰＧＭＩＤ
Private _systemUpdatePersonCd As New EntityKoumoku_MojiType	' システム更新者コード
Private _systemUpdateDay As New EntityKoumoku_YmdType	' システム更新日


Sub New()
_crsCd.PhysicsName = "CRS_CD"
_syuptDay.PhysicsName = "SYUPT_DAY"
_gousya.PhysicsName = "GOUSYA"
_accessCd.PhysicsName = "ACCESS_CD"
_aibeyaUseFlg.PhysicsName = "AIBEYA_USE_FLG"
_aibeyaYoyakuNinzuJyosei.PhysicsName = "AIBEYA_YOYAKU_NINZU_JYOSEI"
_aibeyaYoyakuNinzuMale.PhysicsName = "AIBEYA_YOYAKU_NINZU_MALE"
_binName.PhysicsName = "BIN_NAME"
_blockKakuhoNum.PhysicsName = "BLOCK_KAKUHO_NUM"
_busCompanyCd.PhysicsName = "BUS_COMPANY_CD"
_busReserveCd.PhysicsName = "BUS_RESERVE_CD"
_cancelNgFlg.PhysicsName = "CANCEL_NG_FLG"
_cancelRyouKbn.PhysicsName = "CANCEL_RYOU_KBN"
_cancelWaitNinzu.PhysicsName = "CANCEL_WAIT_NINZU"
_capacityHo1kai.PhysicsName = "CAPACITY_HO_1KAI"
_capacityRegular.PhysicsName = "CAPACITY_REGULAR"
_carrierCd.PhysicsName = "CARRIER_CD"
_carrierEdaban.PhysicsName = "CARRIER_EDABAN"
_carNo.PhysicsName = "CAR_NO"
_carTypeCd.PhysicsName = "CAR_TYPE_CD"
_carTypeCdYotei.PhysicsName = "CAR_TYPE_CD_YOTEI"
_busCountFlg.PhysicsName = "BUS_COUNT_FLG"
_categoryCd1.PhysicsName = "CATEGORY_CD_1"
_categoryCd2.PhysicsName = "CATEGORY_CD_2"
_categoryCd3.PhysicsName = "CATEGORY_CD_3"
_categoryCd4.PhysicsName = "CATEGORY_CD_4"
_costSetKbn.PhysicsName = "COST_SET_KBN"
_crsBlockCapacity.PhysicsName = "CRS_BLOCK_CAPACITY"
_crsBlockOne1r.PhysicsName = "CRS_BLOCK_ONE_1R"
_crsBlockRoomNum.PhysicsName = "CRS_BLOCK_ROOM_NUM"
_crsBlockThree1r.PhysicsName = "CRS_BLOCK_THREE_1R"
_crsBlockTwo1r.PhysicsName = "CRS_BLOCK_TWO_1R"
_crsBlockFour1r.PhysicsName = "CRS_BLOCK_FOUR_1R"
_crsBlockFive1r.PhysicsName = "CRS_BLOCK_FIVE_1R"
_crsKbn1.PhysicsName = "CRS_KBN_1"
_crsKbn2.PhysicsName = "CRS_KBN_2"
_crsKind.PhysicsName = "CRS_KIND"
_managementSec.PhysicsName = "MANAGEMENT_SEC"
_guideGengo.PhysicsName = "GUIDE_GENGO"
_crsName.PhysicsName = "CRS_NAME"
_crsNameKana.PhysicsName = "CRS_NAME_KANA"
_crsNameRk.PhysicsName = "CRS_NAME_RK"
_crsNameKanaRk.PhysicsName = "CRS_NAME_KANA_RK"
_deleteDay.PhysicsName = "DELETE_DAY"
_eiBlockHo.PhysicsName = "EI_BLOCK_HO"
_eiBlockRegular.PhysicsName = "EI_BLOCK_REGULAR"
_endPlaceCd.PhysicsName = "END_PLACE_CD"
_endTime.PhysicsName = "END_TIME"
_haisyaKeiyuCd1.PhysicsName = "HAISYA_KEIYU_CD_1"
_haisyaKeiyuCd2.PhysicsName = "HAISYA_KEIYU_CD_2"
_haisyaKeiyuCd3.PhysicsName = "HAISYA_KEIYU_CD_3"
_haisyaKeiyuCd4.PhysicsName = "HAISYA_KEIYU_CD_4"
_haisyaKeiyuCd5.PhysicsName = "HAISYA_KEIYU_CD_5"
_homenCd.PhysicsName = "HOMEN_CD"
_houjinGaikyakuKbn.PhysicsName = "HOUJIN_GAIKYAKU_KBN"
_hurikomiNgFlg.PhysicsName = "HURIKOMI_NG_FLG"
_itineraryTableCreateFlg.PhysicsName = "ITINERARY_TABLE_CREATE_FLG"
_jyoseiSenyoSeatFlg.PhysicsName = "JYOSEI_SENYO_SEAT_FLG"
_jyosyaCapacity.PhysicsName = "JYOSYA_CAPACITY"
_kaiteiDay.PhysicsName = "KAITEI_DAY"
_kusekiKakuhoNum.PhysicsName = "KUSEKI_KAKUHO_NUM"
_kusekiNumSubSeat.PhysicsName = "KUSEKI_NUM_SUB_SEAT"
_kusekiNumTeiseki.PhysicsName = "KUSEKI_NUM_TEISEKI"
_kyosaiUnkouKbn.PhysicsName = "KYOSAI_UNKOU_KBN"
_maeuriKigen.PhysicsName = "MAEURI_KIGEN"
_maruZouManagementKbn.PhysicsName = "MARU_ZOU_MANAGEMENT_KBN"
_mealCountMorning.PhysicsName = "MEAL_COUNT_MORNING"
_mealCountNight.PhysicsName = "MEAL_COUNT_NIGHT"
_mealCountNoon.PhysicsName = "MEAL_COUNT_NOON"
_mediaCheckFlg.PhysicsName = "MEDIA_CHECK_FLG"
_meiboInputFlg.PhysicsName = "MEIBO_INPUT_FLG"
_ninzuInputFlgKeiyu1.PhysicsName = "NINZU_INPUT_FLG_KEIYU_1"
_ninzuInputFlgKeiyu2.PhysicsName = "NINZU_INPUT_FLG_KEIYU_2"
_ninzuInputFlgKeiyu3.PhysicsName = "NINZU_INPUT_FLG_KEIYU_3"
_ninzuInputFlgKeiyu4.PhysicsName = "NINZU_INPUT_FLG_KEIYU_4"
_ninzuInputFlgKeiyu5.PhysicsName = "NINZU_INPUT_FLG_KEIYU_5"
_ninzuKeiyu1Adult.PhysicsName = "NINZU_KEIYU_1_ADULT"
_ninzuKeiyu1Child.PhysicsName = "NINZU_KEIYU_1_CHILD"
_ninzuKeiyu1Junior.PhysicsName = "NINZU_KEIYU_1_JUNIOR"
_ninzuKeiyu1S.PhysicsName = "NINZU_KEIYU_1_S"
_ninzuKeiyu2Adult.PhysicsName = "NINZU_KEIYU_2_ADULT"
_ninzuKeiyu2Child.PhysicsName = "NINZU_KEIYU_2_CHILD"
_ninzuKeiyu2Junior.PhysicsName = "NINZU_KEIYU_2_JUNIOR"
_ninzuKeiyu2S.PhysicsName = "NINZU_KEIYU_2_S"
_ninzuKeiyu3Adult.PhysicsName = "NINZU_KEIYU_3_ADULT"
_ninzuKeiyu3Child.PhysicsName = "NINZU_KEIYU_3_CHILD"
_ninzuKeiyu3Junior.PhysicsName = "NINZU_KEIYU_3_JUNIOR"
_ninzuKeiyu3S.PhysicsName = "NINZU_KEIYU_3_S"
_ninzuKeiyu4Adult.PhysicsName = "NINZU_KEIYU_4_ADULT"
_ninzuKeiyu4Child.PhysicsName = "NINZU_KEIYU_4_CHILD"
_ninzuKeiyu4Junior.PhysicsName = "NINZU_KEIYU_4_JUNIOR"
_ninzuKeiyu4S.PhysicsName = "NINZU_KEIYU_4_S"
_ninzuKeiyu5Adult.PhysicsName = "NINZU_KEIYU_5_ADULT"
_ninzuKeiyu5Child.PhysicsName = "NINZU_KEIYU_5_CHILD"
_ninzuKeiyu5Junior.PhysicsName = "NINZU_KEIYU_5_JUNIOR"
_ninzuKeiyu5S.PhysicsName = "NINZU_KEIYU_5_S"
_oneSankaFlg.PhysicsName = "ONE_SANKA_FLG"
_optionFlg.PhysicsName = "OPTION_FLG"
_pickupKbn1.PhysicsName = "PICKUP_KBN_1"
_pickupKbn2.PhysicsName = "PICKUP_KBN_2"
_pickupKbn3.PhysicsName = "PICKUP_KBN_3"
_pickupKbn4.PhysicsName = "PICKUP_KBN_4"
_pickupKbn5.PhysicsName = "PICKUP_KBN_5"
_returnDay.PhysicsName = "RETURN_DAY"
_roomZansuOneRoom.PhysicsName = "ROOM_ZANSU_ONE_ROOM"
_roomZansuSokei.PhysicsName = "ROOM_ZANSU_SOKEI"
_roomZansuThreeRoom.PhysicsName = "ROOM_ZANSU_THREE_ROOM"
_roomZansuTwoRoom.PhysicsName = "ROOM_ZANSU_TWO_ROOM"
_roomZansuFourRoom.PhysicsName = "ROOM_ZANSU_FOUR_ROOM"
_roomZansuFiveRoom.PhysicsName = "ROOM_ZANSU_FIVE_ROOM"
_minSaikouNinzu.PhysicsName = "MIN_SAIKOU_NINZU"
_saikouDay.PhysicsName = "SAIKOU_DAY"
_saikouKakuteiKbn.PhysicsName = "SAIKOU_KAKUTEI_KBN"
_seasonCd.PhysicsName = "SEASON_CD"
_senyoCrsKbn.PhysicsName = "SENYO_CRS_KBN"
_shanaiContactForMessage.PhysicsName = "SHANAI_CONTACT_FOR_MESSAGE"
_shukujitsuFlg.PhysicsName = "SHUKUJITSU_FLG"
_sinsetuYm.PhysicsName = "SINSETU_YM"
_stayDay.PhysicsName = "STAY_DAY"
_stayStay.PhysicsName = "STAY_STAY"
_subSeatOkKbn.PhysicsName = "SUB_SEAT_OK_KBN"
_syoyoTime.PhysicsName = "SYOYO_TIME"
_syugoPlaceCdCarrier.PhysicsName = "SYUGO_PLACE_CD_CARRIER"
_syugoTime1.PhysicsName = "SYUGO_TIME_1"
_syugoTime2.PhysicsName = "SYUGO_TIME_2"
_syugoTime3.PhysicsName = "SYUGO_TIME_3"
_syugoTime4.PhysicsName = "SYUGO_TIME_4"
_syugoTime5.PhysicsName = "SYUGO_TIME_5"
_syugoTimeCarrier.PhysicsName = "SYUGO_TIME_CARRIER"
_syuptJiCarrierKbn.PhysicsName = "SYUPT_JI_CARRIER_KBN"
_syuptPlaceCarrier.PhysicsName = "SYUPT_PLACE_CARRIER"
_syuptPlaceCdCarrier.PhysicsName = "SYUPT_PLACE_CD_CARRIER"
_syuptTime1.PhysicsName = "SYUPT_TIME_1"
_syuptTime2.PhysicsName = "SYUPT_TIME_2"
_syuptTime3.PhysicsName = "SYUPT_TIME_3"
_syuptTime4.PhysicsName = "SYUPT_TIME_4"
_syuptTime5.PhysicsName = "SYUPT_TIME_5"
_syuptTimeCarrier.PhysicsName = "SYUPT_TIME_CARRIER"
_teiinseiFlg.PhysicsName = "TEIINSEI_FLG"
_teikiCrsKbn.PhysicsName = "TEIKI_CRS_KBN"
_teikiKikakuKbn.PhysicsName = "TEIKI_KIKAKU_KBN"
_tejimaiContactKbn.PhysicsName = "TEJIMAI_CONTACT_KBN"
_tejimaiDay.PhysicsName = "TEJIMAI_DAY"
_tejimaiKbn.PhysicsName = "TEJIMAI_KBN"
_tenjyoinCd.PhysicsName = "TENJYOIN_CD"
_tieTyakuyo.PhysicsName = "TIE_TYAKUYO"
_tokuteiChargeSet.PhysicsName = "TOKUTEI_CHARGE_SET"
_tokuteiDayFlg.PhysicsName = "TOKUTEI_DAY_FLG"
_ttyakPlaceCarrier.PhysicsName = "TTYAK_PLACE_CARRIER"
_ttyakPlaceCdCarrier.PhysicsName = "TTYAK_PLACE_CD_CARRIER"
_ttyakTimeCarrier.PhysicsName = "TTYAK_TIME_CARRIER"
_tyuijikou.PhysicsName = "TYUIJIKOU"
_tyuijikouKbn.PhysicsName = "TYUIJIKOU_KBN"
_uketukeGenteiNinzu.PhysicsName = "UKETUKE_GENTEI_NINZU"
_uketukeStartBi.PhysicsName = "UKETUKE_START_BI"
_uketukeStartDay.PhysicsName = "UKETUKE_START_DAY"
_uketukeStartKagetumae.PhysicsName = "UKETUKE_START_KAGETUMAE"
_underKinsi18old.PhysicsName = "UNDER_KINSI_18OLD"
_unkyuContactDay.PhysicsName = "UNKYU_CONTACT_DAY"
_unkyuContactDoneFlg.PhysicsName = "UNKYU_CONTACT_DONE_FLG"
_unkyuContactTime.PhysicsName = "UNKYU_CONTACT_TIME"
_unkyuKbn.PhysicsName = "UNKYU_KBN"
_tojituKokuchiFlg.PhysicsName = "TOJITU_KOKUCHI_FLG"
_yusenYoyakuFlg.PhysicsName = "YUSEN_YOYAKU_FLG"
_pickupKbnFlg.PhysicsName = "PICKUP_KBN_FLG"
_konjyoOkFlg.PhysicsName = "KONJYO_OK_FLG"
_tonariFlg.PhysicsName = "TONARI_FLG"
_aheadZasekiFlg.PhysicsName = "AHEAD_ZASEKI_FLG"
_yoyakuMediaInputFlg.PhysicsName = "YOYAKU_MEDIA_INPUT_FLG"
_kokusekiFlg.PhysicsName = "KOKUSEKI_FLG"
_sexBetuFlg.PhysicsName = "SEX_BETU_FLG"
_ageFlg.PhysicsName = "AGE_FLG"
_birthdayFlg.PhysicsName = "BIRTHDAY_FLG"
_telFlg.PhysicsName = "TEL_FLG"
_addressFlg.PhysicsName = "ADDRESS_FLG"
_usingFlg.PhysicsName = "USING_FLG"
_uwagiTyakuyo.PhysicsName = "UWAGI_TYAKUYO"
_year.PhysicsName = "YEAR"
_yobiCd.PhysicsName = "YOBI_CD"
_yoyakuAlreadyRoomNum.PhysicsName = "YOYAKU_ALREADY_ROOM_NUM"
_yoyakuKanouNum.PhysicsName = "YOYAKU_KANOU_NUM"
_yoyakuNgFlg.PhysicsName = "YOYAKU_NG_FLG"
_yoyakuNumSubSeat.PhysicsName = "YOYAKU_NUM_SUB_SEAT"
_yoyakuNumTeiseki.PhysicsName = "YOYAKU_NUM_TEISEKI"
_yoyakuStopFlg.PhysicsName = "YOYAKU_STOP_FLG"
_zouhatsumotogousya.PhysicsName = "ZOUHATSUMOTO_GOUSYA"
_zouhatsuday.PhysicsName = "ZOUHATSU_DAY"
_zouhatsuentrypersoncd.PhysicsName = "ZOUHATSU_ENTRY_PERSON_CD"
_zasekiHihyojiFlg.PhysicsName = "ZASEKI_HIHYOJI_FLG"
_zasekiReserveKbn.PhysicsName = "ZASEKI_RESERVE_KBN"
_wtKakuhoSeatNum.PhysicsName = "WT_KAKUHO_SEAT_NUM"
_systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID"
_systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD"
_systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY"
_systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID"
_systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD"
_systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY"


_crsCd.Required = FALSE
_syuptDay.Required = FALSE
_gousya.Required = FALSE
_accessCd.Required = FALSE
_aibeyaUseFlg.Required = FALSE
_aibeyaYoyakuNinzuJyosei.Required = FALSE
_aibeyaYoyakuNinzuMale.Required = FALSE
_binName.Required = FALSE
_blockKakuhoNum.Required = FALSE
_busCompanyCd.Required = FALSE
_busReserveCd.Required = FALSE
_cancelNgFlg.Required = FALSE
_cancelRyouKbn.Required = FALSE
_cancelWaitNinzu.Required = FALSE
_capacityHo1kai.Required = FALSE
_capacityRegular.Required = FALSE
_carrierCd.Required = FALSE
_carrierEdaban.Required = FALSE
_carNo.Required = FALSE
_carTypeCd.Required = FALSE
_carTypeCdYotei.Required = FALSE
_busCountFlg.Required = FALSE
_categoryCd1.Required = FALSE
_categoryCd2.Required = FALSE
_categoryCd3.Required = FALSE
_categoryCd4.Required = FALSE
_costSetKbn.Required = FALSE
_crsBlockCapacity.Required = FALSE
_crsBlockOne1r.Required = FALSE
_crsBlockRoomNum.Required = FALSE
_crsBlockThree1r.Required = FALSE
_crsBlockTwo1r.Required = FALSE
_crsBlockFour1r.Required = FALSE
_crsBlockFive1r.Required = FALSE
_crsKbn1.Required = FALSE
_crsKbn2.Required = FALSE
_crsKind.Required = FALSE
_managementSec.Required = FALSE
_guideGengo.Required = FALSE
_crsName.Required = FALSE
_crsNameKana.Required = FALSE
_crsNameRk.Required = FALSE
_crsNameKanaRk.Required = FALSE
_deleteDay.Required = FALSE
_eiBlockHo.Required = FALSE
_eiBlockRegular.Required = FALSE
_endPlaceCd.Required = FALSE
_endTime.Required = FALSE
_haisyaKeiyuCd1.Required = FALSE
_haisyaKeiyuCd2.Required = FALSE
_haisyaKeiyuCd3.Required = FALSE
_haisyaKeiyuCd4.Required = FALSE
_haisyaKeiyuCd5.Required = FALSE
_homenCd.Required = FALSE
_houjinGaikyakuKbn.Required = FALSE
_hurikomiNgFlg.Required = FALSE
_itineraryTableCreateFlg.Required = FALSE
_jyoseiSenyoSeatFlg.Required = FALSE
_jyosyaCapacity.Required = FALSE
_kaiteiDay.Required = FALSE
_kusekiKakuhoNum.Required = FALSE
_kusekiNumSubSeat.Required = FALSE
_kusekiNumTeiseki.Required = FALSE
_kyosaiUnkouKbn.Required = FALSE
_maeuriKigen.Required = FALSE
_maruZouManagementKbn.Required = FALSE
_mealCountMorning.Required = FALSE
_mealCountNight.Required = FALSE
_mealCountNoon.Required = FALSE
_mediaCheckFlg.Required = FALSE
_meiboInputFlg.Required = FALSE
_ninzuInputFlgKeiyu1.Required = FALSE
_ninzuInputFlgKeiyu2.Required = FALSE
_ninzuInputFlgKeiyu3.Required = FALSE
_ninzuInputFlgKeiyu4.Required = FALSE
_ninzuInputFlgKeiyu5.Required = FALSE
_ninzuKeiyu1Adult.Required = FALSE
_ninzuKeiyu1Child.Required = FALSE
_ninzuKeiyu1Junior.Required = FALSE
_ninzuKeiyu1S.Required = FALSE
_ninzuKeiyu2Adult.Required = FALSE
_ninzuKeiyu2Child.Required = FALSE
_ninzuKeiyu2Junior.Required = FALSE
_ninzuKeiyu2S.Required = FALSE
_ninzuKeiyu3Adult.Required = FALSE
_ninzuKeiyu3Child.Required = FALSE
_ninzuKeiyu3Junior.Required = FALSE
_ninzuKeiyu3S.Required = FALSE
_ninzuKeiyu4Adult.Required = FALSE
_ninzuKeiyu4Child.Required = FALSE
_ninzuKeiyu4Junior.Required = FALSE
_ninzuKeiyu4S.Required = FALSE
_ninzuKeiyu5Adult.Required = FALSE
_ninzuKeiyu5Child.Required = FALSE
_ninzuKeiyu5Junior.Required = FALSE
_ninzuKeiyu5S.Required = FALSE
_oneSankaFlg.Required = FALSE
_optionFlg.Required = FALSE
_pickupKbn1.Required = FALSE
_pickupKbn2.Required = FALSE
_pickupKbn3.Required = FALSE
_pickupKbn4.Required = FALSE
_pickupKbn5.Required = FALSE
_returnDay.Required = FALSE
_roomZansuOneRoom.Required = FALSE
_roomZansuSokei.Required = FALSE
_roomZansuThreeRoom.Required = FALSE
_roomZansuTwoRoom.Required = FALSE
_roomZansuFourRoom.Required = FALSE
_roomZansuFiveRoom.Required = FALSE
_minSaikouNinzu.Required = FALSE
_saikouDay.Required = FALSE
_saikouKakuteiKbn.Required = FALSE
_seasonCd.Required = FALSE
_senyoCrsKbn.Required = FALSE
_shanaiContactForMessage.Required = FALSE
_shukujitsuFlg.Required = FALSE
_sinsetuYm.Required = FALSE
_stayDay.Required = FALSE
_stayStay.Required = FALSE
_subSeatOkKbn.Required = FALSE
_syoyoTime.Required = FALSE
_syugoPlaceCdCarrier.Required = FALSE
_syugoTime1.Required = FALSE
_syugoTime2.Required = FALSE
_syugoTime3.Required = FALSE
_syugoTime4.Required = FALSE
_syugoTime5.Required = FALSE
_syugoTimeCarrier.Required = FALSE
_syuptJiCarrierKbn.Required = FALSE
_syuptPlaceCarrier.Required = FALSE
_syuptPlaceCdCarrier.Required = FALSE
_syuptTime1.Required = FALSE
_syuptTime2.Required = FALSE
_syuptTime3.Required = FALSE
_syuptTime4.Required = FALSE
_syuptTime5.Required = FALSE
_syuptTimeCarrier.Required = FALSE
_teiinseiFlg.Required = FALSE
_teikiCrsKbn.Required = FALSE
_teikiKikakuKbn.Required = FALSE
_tejimaiContactKbn.Required = FALSE
_tejimaiDay.Required = FALSE
_tejimaiKbn.Required = FALSE
_tenjyoinCd.Required = FALSE
_tieTyakuyo.Required = FALSE
_tokuteiChargeSet.Required = FALSE
_tokuteiDayFlg.Required = FALSE
_ttyakPlaceCarrier.Required = FALSE
_ttyakPlaceCdCarrier.Required = FALSE
_ttyakTimeCarrier.Required = FALSE
_tyuijikou.Required = FALSE
_tyuijikouKbn.Required = FALSE
_uketukeGenteiNinzu.Required = FALSE
_uketukeStartBi.Required = FALSE
_uketukeStartDay.Required = FALSE
_uketukeStartKagetumae.Required = FALSE
_underKinsi18old.Required = FALSE
_unkyuContactDay.Required = FALSE
_unkyuContactDoneFlg.Required = FALSE
_unkyuContactTime.Required = FALSE
_unkyuKbn.Required = FALSE
_tojituKokuchiFlg.Required = FALSE
_yusenYoyakuFlg.Required = FALSE
_pickupKbnFlg.Required = FALSE
_konjyoOkFlg.Required = FALSE
_tonariFlg.Required = FALSE
_aheadZasekiFlg.Required = FALSE
_yoyakuMediaInputFlg.Required = FALSE
_kokusekiFlg.Required = FALSE
_sexBetuFlg.Required = FALSE
_ageFlg.Required = FALSE
_birthdayFlg.Required = FALSE
_telFlg.Required = FALSE
_addressFlg.Required = FALSE
_usingFlg.Required = FALSE
_uwagiTyakuyo.Required = FALSE
_year.Required = FALSE
_yobiCd.Required = FALSE
_yoyakuAlreadyRoomNum.Required = FALSE
_yoyakuKanouNum.Required = FALSE
_yoyakuNgFlg.Required = FALSE
_yoyakuNumSubSeat.Required = FALSE
_yoyakuNumTeiseki.Required = FALSE
_yoyakuStopFlg.Required = FALSE
_zouhatsumotogousya.Required = FALSE
_zouhatsuday.Required = FALSE
_zouhatsuentrypersoncd.Required = FALSE
_zasekiHihyojiFlg.Required = FALSE
_zasekiReserveKbn.Required = FALSE
_wtKakuhoSeatNum.Required = FALSE
_systemEntryPgmid.Required = TRUE
_systemEntryPersonCd.Required = TRUE
_systemEntryDay.Required = TRUE
_systemUpdatePgmid.Required = TRUE
_systemUpdatePersonCd.Required = TRUE
_systemUpdateDay.Required = TRUE


_crsCd.DBType = OracleDbType.Char
_syuptDay.DBType = OracleDbType.Decimal
_gousya.DBType = OracleDbType.Decimal
_accessCd.DBType = OracleDbType.Char
_aibeyaUseFlg.DBType = OracleDbType.Char
_aibeyaYoyakuNinzuJyosei.DBType = OracleDbType.Decimal
_aibeyaYoyakuNinzuMale.DBType = OracleDbType.Decimal
_binName.DBType = OracleDbType.Varchar2
_blockKakuhoNum.DBType = OracleDbType.Decimal
_busCompanyCd.DBType = OracleDbType.Char
_busReserveCd.DBType = OracleDbType.Char
_cancelNgFlg.DBType = OracleDbType.Char
_cancelRyouKbn.DBType = OracleDbType.Char
_cancelWaitNinzu.DBType = OracleDbType.Decimal
_capacityHo1kai.DBType = OracleDbType.Decimal
_capacityRegular.DBType = OracleDbType.Decimal
_carrierCd.DBType = OracleDbType.Char
_carrierEdaban.DBType = OracleDbType.Char
_carNo.DBType = OracleDbType.Decimal
_carTypeCd.DBType = OracleDbType.Char
_carTypeCdYotei.DBType = OracleDbType.Char
_busCountFlg.DBType = OracleDbType.Char
_categoryCd1.DBType = OracleDbType.Char
_categoryCd2.DBType = OracleDbType.Char
_categoryCd3.DBType = OracleDbType.Char
_categoryCd4.DBType = OracleDbType.Char
_costSetKbn.DBType = OracleDbType.Char
_crsBlockCapacity.DBType = OracleDbType.Decimal
_crsBlockOne1r.DBType = OracleDbType.Decimal
_crsBlockRoomNum.DBType = OracleDbType.Decimal
_crsBlockThree1r.DBType = OracleDbType.Decimal
_crsBlockTwo1r.DBType = OracleDbType.Decimal
_crsBlockFour1r.DBType = OracleDbType.Decimal
_crsBlockFive1r.DBType = OracleDbType.Decimal
_crsKbn1.DBType = OracleDbType.Char
_crsKbn2.DBType = OracleDbType.Char
_crsKind.DBType = OracleDbType.Char
_managementSec.DBType = OracleDbType.Varchar2
_guideGengo.DBType = OracleDbType.Char
_crsName.DBType = OracleDbType.Varchar2
_crsNameKana.DBType = OracleDbType.Varchar2
_crsNameRk.DBType = OracleDbType.Varchar2
_crsNameKanaRk.DBType = OracleDbType.Varchar2
_deleteDay.DBType = OracleDbType.Decimal
_eiBlockHo.DBType = OracleDbType.Decimal
_eiBlockRegular.DBType = OracleDbType.Decimal
_endPlaceCd.DBType = OracleDbType.Char
_endTime.DBType = OracleDbType.Decimal
_haisyaKeiyuCd1.DBType = OracleDbType.Char
_haisyaKeiyuCd2.DBType = OracleDbType.Char
_haisyaKeiyuCd3.DBType = OracleDbType.Char
_haisyaKeiyuCd4.DBType = OracleDbType.Char
_haisyaKeiyuCd5.DBType = OracleDbType.Char
_homenCd.DBType = OracleDbType.Char
_houjinGaikyakuKbn.DBType = OracleDbType.Char
_hurikomiNgFlg.DBType = OracleDbType.Char
_itineraryTableCreateFlg.DBType = OracleDbType.Char
_jyoseiSenyoSeatFlg.DBType = OracleDbType.Char
_jyosyaCapacity.DBType = OracleDbType.Decimal
_kaiteiDay.DBType = OracleDbType.Decimal
_kusekiKakuhoNum.DBType = OracleDbType.Decimal
_kusekiNumSubSeat.DBType = OracleDbType.Decimal
_kusekiNumTeiseki.DBType = OracleDbType.Decimal
_kyosaiUnkouKbn.DBType = OracleDbType.Char
_maeuriKigen.DBType = OracleDbType.Char
_maruZouManagementKbn.DBType = OracleDbType.Char
_mealCountMorning.DBType = OracleDbType.Decimal
_mealCountNight.DBType = OracleDbType.Decimal
_mealCountNoon.DBType = OracleDbType.Decimal
_mediaCheckFlg.DBType = OracleDbType.Char
_meiboInputFlg.DBType = OracleDbType.Char
_ninzuInputFlgKeiyu1.DBType = OracleDbType.Char
_ninzuInputFlgKeiyu2.DBType = OracleDbType.Char
_ninzuInputFlgKeiyu3.DBType = OracleDbType.Char
_ninzuInputFlgKeiyu4.DBType = OracleDbType.Char
_ninzuInputFlgKeiyu5.DBType = OracleDbType.Char
_ninzuKeiyu1Adult.DBType = OracleDbType.Decimal
_ninzuKeiyu1Child.DBType = OracleDbType.Decimal
_ninzuKeiyu1Junior.DBType = OracleDbType.Decimal
_ninzuKeiyu1S.DBType = OracleDbType.Decimal
_ninzuKeiyu2Adult.DBType = OracleDbType.Decimal
_ninzuKeiyu2Child.DBType = OracleDbType.Decimal
_ninzuKeiyu2Junior.DBType = OracleDbType.Decimal
_ninzuKeiyu2S.DBType = OracleDbType.Decimal
_ninzuKeiyu3Adult.DBType = OracleDbType.Decimal
_ninzuKeiyu3Child.DBType = OracleDbType.Decimal
_ninzuKeiyu3Junior.DBType = OracleDbType.Decimal
_ninzuKeiyu3S.DBType = OracleDbType.Decimal
_ninzuKeiyu4Adult.DBType = OracleDbType.Decimal
_ninzuKeiyu4Child.DBType = OracleDbType.Decimal
_ninzuKeiyu4Junior.DBType = OracleDbType.Decimal
_ninzuKeiyu4S.DBType = OracleDbType.Decimal
_ninzuKeiyu5Adult.DBType = OracleDbType.Decimal
_ninzuKeiyu5Child.DBType = OracleDbType.Decimal
_ninzuKeiyu5Junior.DBType = OracleDbType.Decimal
_ninzuKeiyu5S.DBType = OracleDbType.Decimal
_oneSankaFlg.DBType = OracleDbType.Char
_optionFlg.DBType = OracleDbType.Char
_pickupKbn1.DBType = OracleDbType.Char
_pickupKbn2.DBType = OracleDbType.Char
_pickupKbn3.DBType = OracleDbType.Char
_pickupKbn4.DBType = OracleDbType.Char
_pickupKbn5.DBType = OracleDbType.Char
_returnDay.DBType = OracleDbType.Decimal
_roomZansuOneRoom.DBType = OracleDbType.Decimal
_roomZansuSokei.DBType = OracleDbType.Decimal
_roomZansuThreeRoom.DBType = OracleDbType.Decimal
_roomZansuTwoRoom.DBType = OracleDbType.Decimal
_roomZansuFourRoom.DBType = OracleDbType.Decimal
_roomZansuFiveRoom.DBType = OracleDbType.Decimal
_minSaikouNinzu.DBType = OracleDbType.Decimal
_saikouDay.DBType = OracleDbType.Decimal
_saikouKakuteiKbn.DBType = OracleDbType.Char
_seasonCd.DBType = OracleDbType.Char
_senyoCrsKbn.DBType = OracleDbType.Char
_shanaiContactForMessage.DBType = OracleDbType.Varchar2
_shukujitsuFlg.DBType = OracleDbType.Char
_sinsetuYm.DBType = OracleDbType.Decimal
_stayDay.DBType = OracleDbType.Decimal
_stayStay.DBType = OracleDbType.Decimal
_subSeatOkKbn.DBType = OracleDbType.Char
_syoyoTime.DBType = OracleDbType.Decimal
_syugoPlaceCdCarrier.DBType = OracleDbType.Char
_syugoTime1.DBType = OracleDbType.Decimal
_syugoTime2.DBType = OracleDbType.Decimal
_syugoTime3.DBType = OracleDbType.Decimal
_syugoTime4.DBType = OracleDbType.Decimal
_syugoTime5.DBType = OracleDbType.Decimal
_syugoTimeCarrier.DBType = OracleDbType.Decimal
_syuptJiCarrierKbn.DBType = OracleDbType.Char
_syuptPlaceCarrier.DBType = OracleDbType.Varchar2
_syuptPlaceCdCarrier.DBType = OracleDbType.Char
_syuptTime1.DBType = OracleDbType.Decimal
_syuptTime2.DBType = OracleDbType.Decimal
_syuptTime3.DBType = OracleDbType.Decimal
_syuptTime4.DBType = OracleDbType.Decimal
_syuptTime5.DBType = OracleDbType.Decimal
_syuptTimeCarrier.DBType = OracleDbType.Decimal
_teiinseiFlg.DBType = OracleDbType.Char
_teikiCrsKbn.DBType = OracleDbType.Char
_teikiKikakuKbn.DBType = OracleDbType.Char
_tejimaiContactKbn.DBType = OracleDbType.Char
_tejimaiDay.DBType = OracleDbType.Decimal
_tejimaiKbn.DBType = OracleDbType.Char
_tenjyoinCd.DBType = OracleDbType.Char
_tieTyakuyo.DBType = OracleDbType.Char
_tokuteiChargeSet.DBType = OracleDbType.Char
_tokuteiDayFlg.DBType = OracleDbType.Char
_ttyakPlaceCarrier.DBType = OracleDbType.Varchar2
_ttyakPlaceCdCarrier.DBType = OracleDbType.Char
_ttyakTimeCarrier.DBType = OracleDbType.Decimal
_tyuijikou.DBType = OracleDbType.Varchar2
_tyuijikouKbn.DBType = OracleDbType.Char
_uketukeGenteiNinzu.DBType = OracleDbType.Decimal
_uketukeStartBi.DBType = OracleDbType.Decimal
_uketukeStartDay.DBType = OracleDbType.Decimal
_uketukeStartKagetumae.DBType = OracleDbType.Char
_underKinsi18old.DBType = OracleDbType.Char
_unkyuContactDay.DBType = OracleDbType.Decimal
_unkyuContactDoneFlg.DBType = OracleDbType.Char
_unkyuContactTime.DBType = OracleDbType.Decimal
_unkyuKbn.DBType = OracleDbType.Char
_tojituKokuchiFlg.DBType = OracleDbType.Decimal
_yusenYoyakuFlg.DBType = OracleDbType.Decimal
_pickupKbnFlg.DBType = OracleDbType.Decimal
_konjyoOkFlg.DBType = OracleDbType.Decimal
_tonariFlg.DBType = OracleDbType.Decimal
_aheadZasekiFlg.DBType = OracleDbType.Decimal
_yoyakuMediaInputFlg.DBType = OracleDbType.Decimal
_kokusekiFlg.DBType = OracleDbType.Decimal
_sexBetuFlg.DBType = OracleDbType.Decimal
_ageFlg.DBType = OracleDbType.Decimal
_birthdayFlg.DBType = OracleDbType.Decimal
_telFlg.DBType = OracleDbType.Decimal
_addressFlg.DBType = OracleDbType.Decimal
_usingFlg.DBType = OracleDbType.Char
_uwagiTyakuyo.DBType = OracleDbType.Char
_year.DBType = OracleDbType.Decimal
_yobiCd.DBType = OracleDbType.Char
_yoyakuAlreadyRoomNum.DBType = OracleDbType.Decimal
_yoyakuKanouNum.DBType = OracleDbType.Decimal
_yoyakuNgFlg.DBType = OracleDbType.Char
_yoyakuNumSubSeat.DBType = OracleDbType.Decimal
_yoyakuNumTeiseki.DBType = OracleDbType.Decimal
_yoyakuStopFlg.DBType = OracleDbType.Char
_zouhatsumotogousya.DBType = OracleDbType.Decimal
_zouhatsuday.DBType = OracleDbType.Decimal
_zouhatsuentrypersoncd.DBType = OracleDbType.Char
_zasekiHihyojiFlg.DBType = OracleDbType.Char
_zasekiReserveKbn.DBType = OracleDbType.Char
_wtKakuhoSeatNum.DBType = OracleDbType.Decimal
_systemEntryPgmid.DBType = OracleDbType.Char
_systemEntryPersonCd.DBType = OracleDbType.Varchar2
_systemEntryDay.DBType = OracleDbType.Date
_systemUpdatePgmid.DBType = OracleDbType.Char
_systemUpdatePersonCd.DBType = OracleDbType.Varchar2
_systemUpdateDay.DBType = OracleDbType.Date


_crsCd.IntegerBu = 6
_syuptDay.IntegerBu = 8
_gousya.IntegerBu = 3
_accessCd.IntegerBu = 1
_aibeyaUseFlg.IntegerBu = 1
_aibeyaYoyakuNinzuJyosei.IntegerBu = 3
_aibeyaYoyakuNinzuMale.IntegerBu = 3
_binName.IntegerBu = 10
_blockKakuhoNum.IntegerBu = 3
_busCompanyCd.IntegerBu = 6
_busReserveCd.IntegerBu = 6
_cancelNgFlg.IntegerBu = 1
_cancelRyouKbn.IntegerBu = 1
_cancelWaitNinzu.IntegerBu = 2
_capacityHo1kai.IntegerBu = 3
_capacityRegular.IntegerBu = 3
_carrierCd.IntegerBu = 4
_carrierEdaban.IntegerBu = 2
_carNo.IntegerBu = 3
_carTypeCd.IntegerBu = 2
_carTypeCdYotei.IntegerBu = 2
_busCountFlg.IntegerBu = 1
_categoryCd1.IntegerBu = 1
_categoryCd2.IntegerBu = 1
_categoryCd3.IntegerBu = 1
_categoryCd4.IntegerBu = 1
_costSetKbn.IntegerBu = 1
_crsBlockCapacity.IntegerBu = 5
_crsBlockOne1r.IntegerBu = 3
_crsBlockRoomNum.IntegerBu = 3
_crsBlockThree1r.IntegerBu = 3
_crsBlockTwo1r.IntegerBu = 3
_crsBlockFour1r.IntegerBu = 3
_crsBlockFive1r.IntegerBu = 3
_crsKbn1.IntegerBu = 1
_crsKbn2.IntegerBu = 1
_crsKind.IntegerBu = 1
_managementSec.IntegerBu = 15
_guideGengo.IntegerBu = 2
_crsName.IntegerBu = 256
_crsNameKana.IntegerBu = 128
_crsNameRk.IntegerBu = 20
_crsNameKanaRk.IntegerBu = 20
_deleteDay.IntegerBu = 8
_eiBlockHo.IntegerBu = 3
_eiBlockRegular.IntegerBu = 3
_endPlaceCd.IntegerBu = 3
_endTime.IntegerBu = 4
_haisyaKeiyuCd1.IntegerBu = 3
_haisyaKeiyuCd2.IntegerBu = 3
_haisyaKeiyuCd3.IntegerBu = 3
_haisyaKeiyuCd4.IntegerBu = 3
_haisyaKeiyuCd5.IntegerBu = 3
_homenCd.IntegerBu = 2
_houjinGaikyakuKbn.IntegerBu = 1
_hurikomiNgFlg.IntegerBu = 1
_itineraryTableCreateFlg.IntegerBu = 1
_jyoseiSenyoSeatFlg.IntegerBu = 1
_jyosyaCapacity.IntegerBu = 3
_kaiteiDay.IntegerBu = 8
_kusekiKakuhoNum.IntegerBu = 3
_kusekiNumSubSeat.IntegerBu = 3
_kusekiNumTeiseki.IntegerBu = 3
_kyosaiUnkouKbn.IntegerBu = 1
_maeuriKigen.IntegerBu = 2
_maruZouManagementKbn.IntegerBu = 1
_mealCountMorning.IntegerBu = 2
_mealCountNight.IntegerBu = 2
_mealCountNoon.IntegerBu = 2
_mediaCheckFlg.IntegerBu = 1
_meiboInputFlg.IntegerBu = 1
_ninzuInputFlgKeiyu1.IntegerBu = 1
_ninzuInputFlgKeiyu2.IntegerBu = 1
_ninzuInputFlgKeiyu3.IntegerBu = 1
_ninzuInputFlgKeiyu4.IntegerBu = 1
_ninzuInputFlgKeiyu5.IntegerBu = 1
_ninzuKeiyu1Adult.IntegerBu = 3
_ninzuKeiyu1Child.IntegerBu = 3
_ninzuKeiyu1Junior.IntegerBu = 3
_ninzuKeiyu1S.IntegerBu = 3
_ninzuKeiyu2Adult.IntegerBu = 3
_ninzuKeiyu2Child.IntegerBu = 3
_ninzuKeiyu2Junior.IntegerBu = 3
_ninzuKeiyu2S.IntegerBu = 3
_ninzuKeiyu3Adult.IntegerBu = 3
_ninzuKeiyu3Child.IntegerBu = 3
_ninzuKeiyu3Junior.IntegerBu = 3
_ninzuKeiyu3S.IntegerBu = 3
_ninzuKeiyu4Adult.IntegerBu = 3
_ninzuKeiyu4Child.IntegerBu = 3
_ninzuKeiyu4Junior.IntegerBu = 3
_ninzuKeiyu4S.IntegerBu = 3
_ninzuKeiyu5Adult.IntegerBu = 3
_ninzuKeiyu5Child.IntegerBu = 3
_ninzuKeiyu5Junior.IntegerBu = 3
_ninzuKeiyu5S.IntegerBu = 3
_oneSankaFlg.IntegerBu = 1
_optionFlg.IntegerBu = 1
_pickupKbn1.IntegerBu = 1
_pickupKbn2.IntegerBu = 1
_pickupKbn3.IntegerBu = 1
_pickupKbn4.IntegerBu = 1
_pickupKbn5.IntegerBu = 1
_returnDay.IntegerBu = 8
_roomZansuOneRoom.IntegerBu = 3
_roomZansuSokei.IntegerBu = 3
_roomZansuThreeRoom.IntegerBu = 3
_roomZansuTwoRoom.IntegerBu = 3
_roomZansuFourRoom.IntegerBu = 3
_roomZansuFiveRoom.IntegerBu = 3
_minSaikouNinzu.IntegerBu = 2
_saikouDay.IntegerBu = 8
_saikouKakuteiKbn.IntegerBu = 1
_seasonCd.IntegerBu = 1
_senyoCrsKbn.IntegerBu = 1
_shanaiContactForMessage.IntegerBu = 32
_shukujitsuFlg.IntegerBu = 1
_sinsetuYm.IntegerBu = 6
_stayDay.IntegerBu = 2
_stayStay.IntegerBu = 2
_subSeatOkKbn.IntegerBu = 1
_syoyoTime.IntegerBu = 4
_syugoPlaceCdCarrier.IntegerBu = 3
_syugoTime1.IntegerBu = 4
_syugoTime2.IntegerBu = 4
_syugoTime3.IntegerBu = 4
_syugoTime4.IntegerBu = 4
_syugoTime5.IntegerBu = 4
_syugoTimeCarrier.IntegerBu = 4
_syuptJiCarrierKbn.IntegerBu = 1
_syuptPlaceCarrier.IntegerBu = 12
_syuptPlaceCdCarrier.IntegerBu = 3
_syuptTime1.IntegerBu = 4
_syuptTime2.IntegerBu = 4
_syuptTime3.IntegerBu = 4
_syuptTime4.IntegerBu = 4
_syuptTime5.IntegerBu = 4
_syuptTimeCarrier.IntegerBu = 4
_teiinseiFlg.IntegerBu = 1
_teikiCrsKbn.IntegerBu = 1
_teikiKikakuKbn.IntegerBu = 1
_tejimaiContactKbn.IntegerBu = 1
_tejimaiDay.IntegerBu = 8
_tejimaiKbn.IntegerBu = 1
_tenjyoinCd.IntegerBu = 5
_tieTyakuyo.IntegerBu = 1
_tokuteiChargeSet.IntegerBu = 1
_tokuteiDayFlg.IntegerBu = 1
_ttyakPlaceCarrier.IntegerBu = 12
_ttyakPlaceCdCarrier.IntegerBu = 3
_ttyakTimeCarrier.IntegerBu = 4
_tyuijikou.IntegerBu = 42
_tyuijikouKbn.IntegerBu = 1
_uketukeGenteiNinzu.IntegerBu = 1
_uketukeStartBi.IntegerBu = 2
_uketukeStartDay.IntegerBu = 8
_uketukeStartKagetumae.IntegerBu = 2
_underKinsi18old.IntegerBu = 1
_unkyuContactDay.IntegerBu = 8
_unkyuContactDoneFlg.IntegerBu = 1
_unkyuContactTime.IntegerBu = 6
_unkyuKbn.IntegerBu = 1
_tojituKokuchiFlg.IntegerBu = 1
_yusenYoyakuFlg.IntegerBu = 1
_pickupKbnFlg.IntegerBu = 1
_konjyoOkFlg.IntegerBu = 1
_tonariFlg.IntegerBu = 1
_aheadZasekiFlg.IntegerBu = 1
_yoyakuMediaInputFlg.IntegerBu = 1
_kokusekiFlg.IntegerBu = 1
_sexBetuFlg.IntegerBu = 1
_ageFlg.IntegerBu = 1
_birthdayFlg.IntegerBu = 1
_telFlg.IntegerBu = 1
_addressFlg.IntegerBu = 1
_usingFlg.IntegerBu = 1
_uwagiTyakuyo.IntegerBu = 1
_year.IntegerBu = 4
_yobiCd.IntegerBu = 1
_yoyakuAlreadyRoomNum.IntegerBu = 3
_yoyakuKanouNum.IntegerBu = 3
_yoyakuNgFlg.IntegerBu = 1
_yoyakuNumSubSeat.IntegerBu = 3
_yoyakuNumTeiseki.IntegerBu = 3
_yoyakuStopFlg.IntegerBu = 1
_zouhatsumotogousya.IntegerBu = 3
        _zouhatsuday.IntegerBu = 8
        _zouhatsuentrypersoncd.IntegerBu = 20
_zasekiHihyojiFlg.IntegerBu = 1
_zasekiReserveKbn.IntegerBu = 1
_wtKakuhoSeatNum.IntegerBu = 3
_systemEntryPgmid.IntegerBu = 8
_systemEntryPersonCd.IntegerBu = 20
_systemEntryDay.IntegerBu = 0
_systemUpdatePgmid.IntegerBu = 8
_systemUpdatePersonCd.IntegerBu = 20
_systemUpdateDay.IntegerBu = 0


_crsCd.DecimalBu = 0
_syuptDay.DecimalBu = 0
_gousya.DecimalBu = 0
_accessCd.DecimalBu = 0
_aibeyaUseFlg.DecimalBu = 0
_aibeyaYoyakuNinzuJyosei.DecimalBu = 0
_aibeyaYoyakuNinzuMale.DecimalBu = 0
_binName.DecimalBu = 0
_blockKakuhoNum.DecimalBu = 0
_busCompanyCd.DecimalBu = 0
_busReserveCd.DecimalBu = 0
_cancelNgFlg.DecimalBu = 0
_cancelRyouKbn.DecimalBu = 0
_cancelWaitNinzu.DecimalBu = 0
_capacityHo1kai.DecimalBu = 0
_capacityRegular.DecimalBu = 0
_carrierCd.DecimalBu = 0
_carrierEdaban.DecimalBu = 0
_carNo.DecimalBu = 0
_carTypeCd.DecimalBu = 0
_carTypeCdYotei.DecimalBu = 0
_busCountFlg.DecimalBu = 0
_categoryCd1.DecimalBu = 0
_categoryCd2.DecimalBu = 0
_categoryCd3.DecimalBu = 0
_categoryCd4.DecimalBu = 0
_costSetKbn.DecimalBu = 0
_crsBlockCapacity.DecimalBu = 0
_crsBlockOne1r.DecimalBu = 0
_crsBlockRoomNum.DecimalBu = 0
_crsBlockThree1r.DecimalBu = 0
_crsBlockTwo1r.DecimalBu = 0
_crsBlockFour1r.DecimalBu = 0
_crsBlockFive1r.DecimalBu = 0
_crsKbn1.DecimalBu = 0
_crsKbn2.DecimalBu = 0
_crsKind.DecimalBu = 0
_managementSec.DecimalBu = 0
_guideGengo.DecimalBu = 0
_crsName.DecimalBu = 0
_crsNameKana.DecimalBu = 0
_crsNameRk.DecimalBu = 0
_crsNameKanaRk.DecimalBu = 0
_deleteDay.DecimalBu = 0
_eiBlockHo.DecimalBu = 0
_eiBlockRegular.DecimalBu = 0
_endPlaceCd.DecimalBu = 0
_endTime.DecimalBu = 0
_haisyaKeiyuCd1.DecimalBu = 0
_haisyaKeiyuCd2.DecimalBu = 0
_haisyaKeiyuCd3.DecimalBu = 0
_haisyaKeiyuCd4.DecimalBu = 0
_haisyaKeiyuCd5.DecimalBu = 0
_homenCd.DecimalBu = 0
_houjinGaikyakuKbn.DecimalBu = 0
_hurikomiNgFlg.DecimalBu = 0
_itineraryTableCreateFlg.DecimalBu = 0
_jyoseiSenyoSeatFlg.DecimalBu = 0
_jyosyaCapacity.DecimalBu = 0
_kaiteiDay.DecimalBu = 0
_kusekiKakuhoNum.DecimalBu = 0
_kusekiNumSubSeat.DecimalBu = 0
_kusekiNumTeiseki.DecimalBu = 0
_kyosaiUnkouKbn.DecimalBu = 0
_maeuriKigen.DecimalBu = 0
_maruZouManagementKbn.DecimalBu = 0
_mealCountMorning.DecimalBu = 0
_mealCountNight.DecimalBu = 0
_mealCountNoon.DecimalBu = 0
_mediaCheckFlg.DecimalBu = 0
_meiboInputFlg.DecimalBu = 0
_ninzuInputFlgKeiyu1.DecimalBu = 0
_ninzuInputFlgKeiyu2.DecimalBu = 0
_ninzuInputFlgKeiyu3.DecimalBu = 0
_ninzuInputFlgKeiyu4.DecimalBu = 0
_ninzuInputFlgKeiyu5.DecimalBu = 0
_ninzuKeiyu1Adult.DecimalBu = 0
_ninzuKeiyu1Child.DecimalBu = 0
_ninzuKeiyu1Junior.DecimalBu = 0
_ninzuKeiyu1S.DecimalBu = 0
_ninzuKeiyu2Adult.DecimalBu = 0
_ninzuKeiyu2Child.DecimalBu = 0
_ninzuKeiyu2Junior.DecimalBu = 0
_ninzuKeiyu2S.DecimalBu = 0
_ninzuKeiyu3Adult.DecimalBu = 0
_ninzuKeiyu3Child.DecimalBu = 0
_ninzuKeiyu3Junior.DecimalBu = 0
_ninzuKeiyu3S.DecimalBu = 0
_ninzuKeiyu4Adult.DecimalBu = 0
_ninzuKeiyu4Child.DecimalBu = 0
_ninzuKeiyu4Junior.DecimalBu = 0
_ninzuKeiyu4S.DecimalBu = 0
_ninzuKeiyu5Adult.DecimalBu = 0
_ninzuKeiyu5Child.DecimalBu = 0
_ninzuKeiyu5Junior.DecimalBu = 0
_ninzuKeiyu5S.DecimalBu = 0
_oneSankaFlg.DecimalBu = 0
_optionFlg.DecimalBu = 0
_pickupKbn1.DecimalBu = 0
_pickupKbn2.DecimalBu = 0
_pickupKbn3.DecimalBu = 0
_pickupKbn4.DecimalBu = 0
_pickupKbn5.DecimalBu = 0
_returnDay.DecimalBu = 0
_roomZansuOneRoom.DecimalBu = 0
_roomZansuSokei.DecimalBu = 0
_roomZansuThreeRoom.DecimalBu = 0
_roomZansuTwoRoom.DecimalBu = 0
_roomZansuFourRoom.DecimalBu = 0
_roomZansuFiveRoom.DecimalBu = 0
_minSaikouNinzu.DecimalBu = 0
_saikouDay.DecimalBu = 0
_saikouKakuteiKbn.DecimalBu = 0
_seasonCd.DecimalBu = 0
_senyoCrsKbn.DecimalBu = 0
_shanaiContactForMessage.DecimalBu = 0
_shukujitsuFlg.DecimalBu = 0
_sinsetuYm.DecimalBu = 0
_stayDay.DecimalBu = 0
_stayStay.DecimalBu = 0
_subSeatOkKbn.DecimalBu = 0
_syoyoTime.DecimalBu = 0
_syugoPlaceCdCarrier.DecimalBu = 0
_syugoTime1.DecimalBu = 0
_syugoTime2.DecimalBu = 0
_syugoTime3.DecimalBu = 0
_syugoTime4.DecimalBu = 0
_syugoTime5.DecimalBu = 0
_syugoTimeCarrier.DecimalBu = 0
_syuptJiCarrierKbn.DecimalBu = 0
_syuptPlaceCarrier.DecimalBu = 0
_syuptPlaceCdCarrier.DecimalBu = 0
_syuptTime1.DecimalBu = 0
_syuptTime2.DecimalBu = 0
_syuptTime3.DecimalBu = 0
_syuptTime4.DecimalBu = 0
_syuptTime5.DecimalBu = 0
_syuptTimeCarrier.DecimalBu = 0
_teiinseiFlg.DecimalBu = 0
_teikiCrsKbn.DecimalBu = 0
_teikiKikakuKbn.DecimalBu = 0
_tejimaiContactKbn.DecimalBu = 0
_tejimaiDay.DecimalBu = 0
_tejimaiKbn.DecimalBu = 0
_tenjyoinCd.DecimalBu = 0
_tieTyakuyo.DecimalBu = 0
_tokuteiChargeSet.DecimalBu = 0
_tokuteiDayFlg.DecimalBu = 0
_ttyakPlaceCarrier.DecimalBu = 0
_ttyakPlaceCdCarrier.DecimalBu = 0
_ttyakTimeCarrier.DecimalBu = 0
_tyuijikou.DecimalBu = 0
_tyuijikouKbn.DecimalBu = 0
_uketukeGenteiNinzu.DecimalBu = 0
_uketukeStartBi.DecimalBu = 0
_uketukeStartDay.DecimalBu = 0
_uketukeStartKagetumae.DecimalBu = 0
_underKinsi18old.DecimalBu = 0
_unkyuContactDay.DecimalBu = 0
_unkyuContactDoneFlg.DecimalBu = 0
_unkyuContactTime.DecimalBu = 0
_unkyuKbn.DecimalBu = 0
_tojituKokuchiFlg.DecimalBu = 0
_yusenYoyakuFlg.DecimalBu = 0
_pickupKbnFlg.DecimalBu = 0
_konjyoOkFlg.DecimalBu = 0
_tonariFlg.DecimalBu = 0
_aheadZasekiFlg.DecimalBu = 0
_yoyakuMediaInputFlg.DecimalBu = 0
_kokusekiFlg.DecimalBu = 0
_sexBetuFlg.DecimalBu = 0
_ageFlg.DecimalBu = 0
_birthdayFlg.DecimalBu = 0
_telFlg.DecimalBu = 0
_addressFlg.DecimalBu = 0
_usingFlg.DecimalBu = 0
_uwagiTyakuyo.DecimalBu = 0
_year.DecimalBu = 0
_yobiCd.DecimalBu = 0
_yoyakuAlreadyRoomNum.DecimalBu = 0
_yoyakuKanouNum.DecimalBu = 0
_yoyakuNgFlg.DecimalBu = 0
_yoyakuNumSubSeat.DecimalBu = 0
_yoyakuNumTeiseki.DecimalBu = 0
_yoyakuStopFlg.DecimalBu = 0
_zouhatsumotogousya.DecimalBu = 0
_zouhatsuday.DecimalBu = 0
_zouhatsuentrypersoncd.DecimalBu = 0
_zasekiHihyojiFlg.DecimalBu = 0
_zasekiReserveKbn.DecimalBu = 0
_wtKakuhoSeatNum.DecimalBu = 0
_systemEntryPgmid.DecimalBu = 0
_systemEntryPersonCd.DecimalBu = 0
_systemEntryDay.DecimalBu = 0
_systemUpdatePgmid.DecimalBu = 0
_systemUpdatePersonCd.DecimalBu = 0
_systemUpdateDay.DecimalBu = 0
End Sub


''' <summary>
''' crsCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsCd() As EntityKoumoku_MojiType
Get
    Return _crsCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsCd = value
End Set
End Property


''' <summary>
''' syuptDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptDay() As EntityKoumoku_NumberType
Get
    Return _syuptDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptDay = value
End Set
End Property


''' <summary>
''' gousya
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property gousya() As EntityKoumoku_NumberType
Get
    Return _gousya
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _gousya = value
End Set
End Property


''' <summary>
''' accessCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property accessCd() As EntityKoumoku_MojiType
Get
    Return _accessCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _accessCd = value
End Set
End Property


''' <summary>
''' aibeyaUseFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property aibeyaUseFlg() As EntityKoumoku_MojiType
Get
    Return _aibeyaUseFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _aibeyaUseFlg = value
End Set
End Property


''' <summary>
''' aibeyaYoyakuNinzuJyosei
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property aibeyaYoyakuNinzuJyosei() As EntityKoumoku_NumberType
Get
    Return _aibeyaYoyakuNinzuJyosei
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _aibeyaYoyakuNinzuJyosei = value
End Set
End Property


''' <summary>
''' aibeyaYoyakuNinzuMale
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property aibeyaYoyakuNinzuMale() As EntityKoumoku_NumberType
Get
    Return _aibeyaYoyakuNinzuMale
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _aibeyaYoyakuNinzuMale = value
End Set
End Property


''' <summary>
''' binName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property binName() As EntityKoumoku_MojiType
Get
    Return _binName
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _binName = value
End Set
End Property


''' <summary>
''' blockKakuhoNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property blockKakuhoNum() As EntityKoumoku_NumberType
Get
    Return _blockKakuhoNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _blockKakuhoNum = value
End Set
End Property


''' <summary>
''' busCompanyCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busCompanyCd() As EntityKoumoku_MojiType
Get
    Return _busCompanyCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _busCompanyCd = value
End Set
End Property


''' <summary>
''' busReserveCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busReserveCd() As EntityKoumoku_MojiType
Get
    Return _busReserveCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _busReserveCd = value
End Set
End Property


''' <summary>
''' cancelNgFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property cancelNgFlg() As EntityKoumoku_MojiType
Get
    Return _cancelNgFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _cancelNgFlg = value
End Set
End Property


''' <summary>
''' cancelRyouKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property cancelRyouKbn() As EntityKoumoku_MojiType
Get
    Return _cancelRyouKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _cancelRyouKbn = value
End Set
End Property


''' <summary>
''' cancelWaitNinzu
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property cancelWaitNinzu() As EntityKoumoku_NumberType
Get
    Return _cancelWaitNinzu
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _cancelWaitNinzu = value
End Set
End Property


''' <summary>
''' capacityHo1kai
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property capacityHo1kai() As EntityKoumoku_NumberType
Get
    Return _capacityHo1kai
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _capacityHo1kai = value
End Set
End Property


''' <summary>
''' capacityRegular
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property capacityRegular() As EntityKoumoku_NumberType
Get
    Return _capacityRegular
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _capacityRegular = value
End Set
End Property


''' <summary>
''' carrierCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carrierCd() As EntityKoumoku_MojiType
Get
    Return _carrierCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carrierCd = value
End Set
End Property


''' <summary>
''' carrierEdaban
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carrierEdaban() As EntityKoumoku_MojiType
Get
    Return _carrierEdaban
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carrierEdaban = value
End Set
End Property


''' <summary>
''' carNo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carNo() As EntityKoumoku_NumberType
Get
    Return _carNo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _carNo = value
End Set
End Property


''' <summary>
''' carTypeCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carTypeCd() As EntityKoumoku_MojiType
Get
    Return _carTypeCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carTypeCd = value
End Set
End Property


''' <summary>
''' carTypeCdYotei
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property carTypeCdYotei() As EntityKoumoku_MojiType
Get
    Return _carTypeCdYotei
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _carTypeCdYotei = value
End Set
End Property


''' <summary>
''' busCountFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property busCountFlg() As EntityKoumoku_MojiType
Get
    Return _busCountFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _busCountFlg = value
End Set
End Property


''' <summary>
''' categoryCd1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property categoryCd1() As EntityKoumoku_MojiType
Get
    Return _categoryCd1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _categoryCd1 = value
End Set
End Property


''' <summary>
''' categoryCd2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property categoryCd2() As EntityKoumoku_MojiType
Get
    Return _categoryCd2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _categoryCd2 = value
End Set
End Property


''' <summary>
''' categoryCd3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property categoryCd3() As EntityKoumoku_MojiType
Get
    Return _categoryCd3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _categoryCd3 = value
End Set
End Property


''' <summary>
''' categoryCd4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property categoryCd4() As EntityKoumoku_MojiType
Get
    Return _categoryCd4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _categoryCd4 = value
End Set
End Property


''' <summary>
''' costSetKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property costSetKbn() As EntityKoumoku_MojiType
Get
    Return _costSetKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _costSetKbn = value
End Set
End Property


''' <summary>
''' crsBlockCapacity
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockCapacity() As EntityKoumoku_NumberType
Get
    Return _crsBlockCapacity
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockCapacity = value
End Set
End Property


''' <summary>
''' crsBlockOne1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockOne1r() As EntityKoumoku_NumberType
Get
    Return _crsBlockOne1r
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockOne1r = value
End Set
End Property


''' <summary>
''' crsBlockRoomNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockRoomNum() As EntityKoumoku_NumberType
Get
    Return _crsBlockRoomNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockRoomNum = value
End Set
End Property


''' <summary>
''' crsBlockThree1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockThree1r() As EntityKoumoku_NumberType
Get
    Return _crsBlockThree1r
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockThree1r = value
End Set
End Property


''' <summary>
''' crsBlockTwo1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockTwo1r() As EntityKoumoku_NumberType
Get
    Return _crsBlockTwo1r
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockTwo1r = value
End Set
End Property


''' <summary>
''' crsBlockFour1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockFour1r() As EntityKoumoku_NumberType
Get
    Return _crsBlockFour1r
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockFour1r = value
End Set
End Property


''' <summary>
''' crsBlockFive1r
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsBlockFive1r() As EntityKoumoku_NumberType
Get
    Return _crsBlockFive1r
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _crsBlockFive1r = value
End Set
End Property


''' <summary>
''' crsKbn1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsKbn1() As EntityKoumoku_MojiType
Get
    Return _crsKbn1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsKbn1 = value
End Set
End Property


''' <summary>
''' crsKbn2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsKbn2() As EntityKoumoku_MojiType
Get
    Return _crsKbn2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsKbn2 = value
End Set
End Property


''' <summary>
''' crsKind
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsKind() As EntityKoumoku_MojiType
Get
    Return _crsKind
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsKind = value
End Set
End Property


''' <summary>
''' managementSec
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property managementSec() As EntityKoumoku_MojiType
Get
    Return _managementSec
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _managementSec = value
End Set
End Property


''' <summary>
''' guideGengo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property guideGengo() As EntityKoumoku_MojiType
Get
    Return _guideGengo
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _guideGengo = value
End Set
End Property


''' <summary>
''' crsName
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsName() As EntityKoumoku_MojiType
Get
    Return _crsName
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsName = value
End Set
End Property


''' <summary>
''' crsNameKana
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsNameKana() As EntityKoumoku_MojiType
Get
    Return _crsNameKana
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsNameKana = value
End Set
End Property


''' <summary>
''' crsNameRk
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsNameRk() As EntityKoumoku_MojiType
Get
    Return _crsNameRk
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsNameRk = value
End Set
End Property


''' <summary>
''' crsNameKanaRk
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property crsNameKanaRk() As EntityKoumoku_MojiType
Get
    Return _crsNameKanaRk
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _crsNameKanaRk = value
End Set
End Property


''' <summary>
''' deleteDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property deleteDay() As EntityKoumoku_NumberType
Get
    Return _deleteDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _deleteDay = value
End Set
End Property


''' <summary>
''' eiBlockHo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property eiBlockHo() As EntityKoumoku_NumberType
Get
    Return _eiBlockHo
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _eiBlockHo = value
End Set
End Property


''' <summary>
''' eiBlockRegular
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property eiBlockRegular() As EntityKoumoku_NumberType
Get
    Return _eiBlockRegular
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _eiBlockRegular = value
End Set
End Property


''' <summary>
''' endPlaceCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property endPlaceCd() As EntityKoumoku_MojiType
Get
    Return _endPlaceCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _endPlaceCd = value
End Set
End Property


''' <summary>
''' endTime
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property endTime() As EntityKoumoku_NumberType
Get
    Return _endTime
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _endTime = value
End Set
End Property


''' <summary>
''' haisyaKeiyuCd1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property haisyaKeiyuCd1() As EntityKoumoku_MojiType
Get
    Return _haisyaKeiyuCd1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _haisyaKeiyuCd1 = value
End Set
End Property


''' <summary>
''' haisyaKeiyuCd2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property haisyaKeiyuCd2() As EntityKoumoku_MojiType
Get
    Return _haisyaKeiyuCd2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _haisyaKeiyuCd2 = value
End Set
End Property


''' <summary>
''' haisyaKeiyuCd3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property haisyaKeiyuCd3() As EntityKoumoku_MojiType
Get
    Return _haisyaKeiyuCd3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _haisyaKeiyuCd3 = value
End Set
End Property


''' <summary>
''' haisyaKeiyuCd4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property haisyaKeiyuCd4() As EntityKoumoku_MojiType
Get
    Return _haisyaKeiyuCd4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _haisyaKeiyuCd4 = value
End Set
End Property


''' <summary>
''' haisyaKeiyuCd5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property haisyaKeiyuCd5() As EntityKoumoku_MojiType
Get
    Return _haisyaKeiyuCd5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _haisyaKeiyuCd5 = value
End Set
End Property


''' <summary>
''' homenCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property homenCd() As EntityKoumoku_MojiType
Get
    Return _homenCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _homenCd = value
End Set
End Property


''' <summary>
''' houjinGaikyakuKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property houjinGaikyakuKbn() As EntityKoumoku_MojiType
Get
    Return _houjinGaikyakuKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _houjinGaikyakuKbn = value
End Set
End Property


''' <summary>
''' hurikomiNgFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property hurikomiNgFlg() As EntityKoumoku_MojiType
Get
    Return _hurikomiNgFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _hurikomiNgFlg = value
End Set
End Property


''' <summary>
''' itineraryTableCreateFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property itineraryTableCreateFlg() As EntityKoumoku_MojiType
Get
    Return _itineraryTableCreateFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _itineraryTableCreateFlg = value
End Set
End Property


''' <summary>
''' jyoseiSenyoSeatFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property jyoseiSenyoSeatFlg() As EntityKoumoku_MojiType
Get
    Return _jyoseiSenyoSeatFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _jyoseiSenyoSeatFlg = value
End Set
End Property


''' <summary>
''' jyosyaCapacity
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property jyosyaCapacity() As EntityKoumoku_NumberType
Get
    Return _jyosyaCapacity
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _jyosyaCapacity = value
End Set
End Property


''' <summary>
''' kaiteiDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kaiteiDay() As EntityKoumoku_NumberType
Get
    Return _kaiteiDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kaiteiDay = value
End Set
End Property


''' <summary>
''' kusekiKakuhoNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kusekiKakuhoNum() As EntityKoumoku_NumberType
Get
    Return _kusekiKakuhoNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kusekiKakuhoNum = value
End Set
End Property


''' <summary>
''' kusekiNumSubSeat
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kusekiNumSubSeat() As EntityKoumoku_NumberType
Get
    Return _kusekiNumSubSeat
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kusekiNumSubSeat = value
End Set
End Property


''' <summary>
''' kusekiNumTeiseki
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kusekiNumTeiseki() As EntityKoumoku_NumberType
Get
    Return _kusekiNumTeiseki
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kusekiNumTeiseki = value
End Set
End Property


''' <summary>
''' kyosaiUnkouKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kyosaiUnkouKbn() As EntityKoumoku_MojiType
Get
    Return _kyosaiUnkouKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _kyosaiUnkouKbn = value
End Set
End Property


''' <summary>
''' maeuriKigen
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property maeuriKigen() As EntityKoumoku_MojiType
Get
    Return _maeuriKigen
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _maeuriKigen = value
End Set
End Property


''' <summary>
''' maruZouManagementKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property maruZouManagementKbn() As EntityKoumoku_MojiType
Get
    Return _maruZouManagementKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _maruZouManagementKbn = value
End Set
End Property


''' <summary>
''' mealCountMorning
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property mealCountMorning() As EntityKoumoku_NumberType
Get
    Return _mealCountMorning
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _mealCountMorning = value
End Set
End Property


''' <summary>
''' mealCountNight
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property mealCountNight() As EntityKoumoku_NumberType
Get
    Return _mealCountNight
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _mealCountNight = value
End Set
End Property


''' <summary>
''' mealCountNoon
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property mealCountNoon() As EntityKoumoku_NumberType
Get
    Return _mealCountNoon
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _mealCountNoon = value
End Set
End Property


''' <summary>
''' mediaCheckFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property mediaCheckFlg() As EntityKoumoku_MojiType
Get
    Return _mediaCheckFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _mediaCheckFlg = value
End Set
End Property


''' <summary>
''' meiboInputFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property meiboInputFlg() As EntityKoumoku_MojiType
Get
    Return _meiboInputFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _meiboInputFlg = value
End Set
End Property


''' <summary>
''' ninzuInputFlgKeiyu1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuInputFlgKeiyu1() As EntityKoumoku_MojiType
Get
    Return _ninzuInputFlgKeiyu1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ninzuInputFlgKeiyu1 = value
End Set
End Property


''' <summary>
''' ninzuInputFlgKeiyu2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuInputFlgKeiyu2() As EntityKoumoku_MojiType
Get
    Return _ninzuInputFlgKeiyu2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ninzuInputFlgKeiyu2 = value
End Set
End Property


''' <summary>
''' ninzuInputFlgKeiyu3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuInputFlgKeiyu3() As EntityKoumoku_MojiType
Get
    Return _ninzuInputFlgKeiyu3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ninzuInputFlgKeiyu3 = value
End Set
End Property


''' <summary>
''' ninzuInputFlgKeiyu4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuInputFlgKeiyu4() As EntityKoumoku_MojiType
Get
    Return _ninzuInputFlgKeiyu4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ninzuInputFlgKeiyu4 = value
End Set
End Property


''' <summary>
''' ninzuInputFlgKeiyu5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuInputFlgKeiyu5() As EntityKoumoku_MojiType
Get
    Return _ninzuInputFlgKeiyu5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ninzuInputFlgKeiyu5 = value
End Set
End Property


''' <summary>
''' ninzuKeiyu1Adult
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu1Adult() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu1Adult
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu1Adult = value
End Set
End Property


''' <summary>
''' ninzuKeiyu1Child
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu1Child() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu1Child
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu1Child = value
End Set
End Property


''' <summary>
''' ninzuKeiyu1Junior
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu1Junior() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu1Junior
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu1Junior = value
End Set
End Property


''' <summary>
''' ninzuKeiyu1S
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu1S() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu1S
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu1S = value
End Set
End Property


''' <summary>
''' ninzuKeiyu2Adult
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu2Adult() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu2Adult
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu2Adult = value
End Set
End Property


''' <summary>
''' ninzuKeiyu2Child
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu2Child() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu2Child
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu2Child = value
End Set
End Property


''' <summary>
''' ninzuKeiyu2Junior
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu2Junior() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu2Junior
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu2Junior = value
End Set
End Property


''' <summary>
''' ninzuKeiyu2S
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu2S() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu2S
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu2S = value
End Set
End Property


''' <summary>
''' ninzuKeiyu3Adult
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu3Adult() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu3Adult
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu3Adult = value
End Set
End Property


''' <summary>
''' ninzuKeiyu3Child
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu3Child() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu3Child
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu3Child = value
End Set
End Property


''' <summary>
''' ninzuKeiyu3Junior
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu3Junior() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu3Junior
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu3Junior = value
End Set
End Property


''' <summary>
''' ninzuKeiyu3S
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu3S() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu3S
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu3S = value
End Set
End Property


''' <summary>
''' ninzuKeiyu4Adult
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu4Adult() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu4Adult
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu4Adult = value
End Set
End Property


''' <summary>
''' ninzuKeiyu4Child
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu4Child() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu4Child
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu4Child = value
End Set
End Property


''' <summary>
''' ninzuKeiyu4Junior
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu4Junior() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu4Junior
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu4Junior = value
End Set
End Property


''' <summary>
''' ninzuKeiyu4S
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu4S() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu4S
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu4S = value
End Set
End Property


''' <summary>
''' ninzuKeiyu5Adult
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu5Adult() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu5Adult
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu5Adult = value
End Set
End Property


''' <summary>
''' ninzuKeiyu5Child
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu5Child() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu5Child
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu5Child = value
End Set
End Property


''' <summary>
''' ninzuKeiyu5Junior
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu5Junior() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu5Junior
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu5Junior = value
End Set
End Property


''' <summary>
''' ninzuKeiyu5S
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ninzuKeiyu5S() As EntityKoumoku_NumberType
Get
    Return _ninzuKeiyu5S
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ninzuKeiyu5S = value
End Set
End Property


''' <summary>
''' oneSankaFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property oneSankaFlg() As EntityKoumoku_MojiType
Get
    Return _oneSankaFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _oneSankaFlg = value
End Set
End Property


''' <summary>
''' optionFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property optionFlg() As EntityKoumoku_MojiType
Get
    Return _optionFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _optionFlg = value
End Set
End Property


''' <summary>
''' pickupKbn1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbn1() As EntityKoumoku_MojiType
Get
    Return _pickupKbn1
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _pickupKbn1 = value
End Set
End Property


''' <summary>
''' pickupKbn2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbn2() As EntityKoumoku_MojiType
Get
    Return _pickupKbn2
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _pickupKbn2 = value
End Set
End Property


''' <summary>
''' pickupKbn3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbn3() As EntityKoumoku_MojiType
Get
    Return _pickupKbn3
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _pickupKbn3 = value
End Set
End Property


''' <summary>
''' pickupKbn4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbn4() As EntityKoumoku_MojiType
Get
    Return _pickupKbn4
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _pickupKbn4 = value
End Set
End Property


''' <summary>
''' pickupKbn5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbn5() As EntityKoumoku_MojiType
Get
    Return _pickupKbn5
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _pickupKbn5 = value
End Set
End Property


''' <summary>
''' returnDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property returnDay() As EntityKoumoku_NumberType
Get
    Return _returnDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _returnDay = value
End Set
End Property


''' <summary>
''' roomZansuOneRoom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuOneRoom() As EntityKoumoku_NumberType
Get
    Return _roomZansuOneRoom
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuOneRoom = value
End Set
End Property


''' <summary>
''' roomZansuSokei
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuSokei() As EntityKoumoku_NumberType
Get
    Return _roomZansuSokei
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuSokei = value
End Set
End Property


''' <summary>
''' roomZansuThreeRoom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuThreeRoom() As EntityKoumoku_NumberType
Get
    Return _roomZansuThreeRoom
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuThreeRoom = value
End Set
End Property


''' <summary>
''' roomZansuTwoRoom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuTwoRoom() As EntityKoumoku_NumberType
Get
    Return _roomZansuTwoRoom
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuTwoRoom = value
End Set
End Property


''' <summary>
''' roomZansuFourRoom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuFourRoom() As EntityKoumoku_NumberType
Get
    Return _roomZansuFourRoom
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuFourRoom = value
End Set
End Property


''' <summary>
''' roomZansuFiveRoom
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property roomZansuFiveRoom() As EntityKoumoku_NumberType
Get
    Return _roomZansuFiveRoom
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _roomZansuFiveRoom = value
End Set
End Property


''' <summary>
''' minSaikouNinzu
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property minSaikouNinzu() As EntityKoumoku_NumberType
Get
    Return _minSaikouNinzu
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _minSaikouNinzu = value
End Set
End Property


''' <summary>
''' saikouDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property saikouDay() As EntityKoumoku_NumberType
Get
    Return _saikouDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _saikouDay = value
End Set
End Property


''' <summary>
''' saikouKakuteiKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property saikouKakuteiKbn() As EntityKoumoku_MojiType
Get
    Return _saikouKakuteiKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _saikouKakuteiKbn = value
End Set
End Property


''' <summary>
''' seasonCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property seasonCd() As EntityKoumoku_MojiType
Get
    Return _seasonCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _seasonCd = value
End Set
End Property


''' <summary>
''' senyoCrsKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property senyoCrsKbn() As EntityKoumoku_MojiType
Get
    Return _senyoCrsKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _senyoCrsKbn = value
End Set
End Property


''' <summary>
''' shanaiContactForMessage
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property shanaiContactForMessage() As EntityKoumoku_MojiType
Get
    Return _shanaiContactForMessage
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _shanaiContactForMessage = value
End Set
End Property


''' <summary>
''' shukujitsuFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property shukujitsuFlg() As EntityKoumoku_MojiType
Get
    Return _shukujitsuFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _shukujitsuFlg = value
End Set
End Property


''' <summary>
''' sinsetuYm
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property sinsetuYm() As EntityKoumoku_NumberType
Get
    Return _sinsetuYm
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _sinsetuYm = value
End Set
End Property


''' <summary>
''' stayDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property stayDay() As EntityKoumoku_NumberType
Get
    Return _stayDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _stayDay = value
End Set
End Property


''' <summary>
''' stayStay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property stayStay() As EntityKoumoku_NumberType
Get
    Return _stayStay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _stayStay = value
End Set
End Property


''' <summary>
''' subSeatOkKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property subSeatOkKbn() As EntityKoumoku_MojiType
Get
    Return _subSeatOkKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _subSeatOkKbn = value
End Set
End Property


''' <summary>
''' syoyoTime
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syoyoTime() As EntityKoumoku_NumberType
Get
    Return _syoyoTime
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syoyoTime = value
End Set
End Property


''' <summary>
''' syugoPlaceCdCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoPlaceCdCarrier() As EntityKoumoku_MojiType
Get
    Return _syugoPlaceCdCarrier
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _syugoPlaceCdCarrier = value
End Set
End Property


''' <summary>
''' syugoTime1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTime1() As EntityKoumoku_NumberType
Get
    Return _syugoTime1
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTime1 = value
End Set
End Property


''' <summary>
''' syugoTime2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTime2() As EntityKoumoku_NumberType
Get
    Return _syugoTime2
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTime2 = value
End Set
End Property


''' <summary>
''' syugoTime3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTime3() As EntityKoumoku_NumberType
Get
    Return _syugoTime3
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTime3 = value
End Set
End Property


''' <summary>
''' syugoTime4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTime4() As EntityKoumoku_NumberType
Get
    Return _syugoTime4
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTime4 = value
End Set
End Property


''' <summary>
''' syugoTime5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTime5() As EntityKoumoku_NumberType
Get
    Return _syugoTime5
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTime5 = value
End Set
End Property


''' <summary>
''' syugoTimeCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syugoTimeCarrier() As EntityKoumoku_NumberType
Get
    Return _syugoTimeCarrier
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syugoTimeCarrier = value
End Set
End Property


''' <summary>
''' syuptJiCarrierKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptJiCarrierKbn() As EntityKoumoku_MojiType
Get
    Return _syuptJiCarrierKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _syuptJiCarrierKbn = value
End Set
End Property


''' <summary>
''' syuptPlaceCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptPlaceCarrier() As EntityKoumoku_MojiType
Get
    Return _syuptPlaceCarrier
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _syuptPlaceCarrier = value
End Set
End Property


''' <summary>
''' syuptPlaceCdCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptPlaceCdCarrier() As EntityKoumoku_MojiType
Get
    Return _syuptPlaceCdCarrier
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _syuptPlaceCdCarrier = value
End Set
End Property


''' <summary>
''' syuptTime1
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTime1() As EntityKoumoku_NumberType
Get
    Return _syuptTime1
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTime1 = value
End Set
End Property


''' <summary>
''' syuptTime2
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTime2() As EntityKoumoku_NumberType
Get
    Return _syuptTime2
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTime2 = value
End Set
End Property


''' <summary>
''' syuptTime3
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTime3() As EntityKoumoku_NumberType
Get
    Return _syuptTime3
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTime3 = value
End Set
End Property


''' <summary>
''' syuptTime4
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTime4() As EntityKoumoku_NumberType
Get
    Return _syuptTime4
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTime4 = value
End Set
End Property


''' <summary>
''' syuptTime5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTime5() As EntityKoumoku_NumberType
Get
    Return _syuptTime5
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTime5 = value
End Set
End Property


''' <summary>
''' syuptTimeCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property syuptTimeCarrier() As EntityKoumoku_NumberType
Get
    Return _syuptTimeCarrier
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _syuptTimeCarrier = value
End Set
End Property


''' <summary>
''' teiinseiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property teiinseiFlg() As EntityKoumoku_MojiType
Get
    Return _teiinseiFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _teiinseiFlg = value
End Set
End Property


''' <summary>
''' teikiCrsKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property teikiCrsKbn() As EntityKoumoku_MojiType
Get
    Return _teikiCrsKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _teikiCrsKbn = value
End Set
End Property


''' <summary>
''' teikiKikakuKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property teikiKikakuKbn() As EntityKoumoku_MojiType
Get
    Return _teikiKikakuKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _teikiKikakuKbn = value
End Set
End Property


''' <summary>
''' tejimaiContactKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tejimaiContactKbn() As EntityKoumoku_MojiType
Get
    Return _tejimaiContactKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tejimaiContactKbn = value
End Set
End Property


''' <summary>
''' tejimaiDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tejimaiDay() As EntityKoumoku_NumberType
Get
    Return _tejimaiDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _tejimaiDay = value
End Set
End Property


''' <summary>
''' tejimaiKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tejimaiKbn() As EntityKoumoku_MojiType
Get
    Return _tejimaiKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tejimaiKbn = value
End Set
End Property


''' <summary>
''' tenjyoinCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tenjyoinCd() As EntityKoumoku_MojiType
Get
    Return _tenjyoinCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tenjyoinCd = value
End Set
End Property


''' <summary>
''' tieTyakuyo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tieTyakuyo() As EntityKoumoku_MojiType
Get
    Return _tieTyakuyo
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tieTyakuyo = value
End Set
End Property


''' <summary>
''' tokuteiChargeSet
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tokuteiChargeSet() As EntityKoumoku_MojiType
Get
    Return _tokuteiChargeSet
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tokuteiChargeSet = value
End Set
End Property


''' <summary>
''' tokuteiDayFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tokuteiDayFlg() As EntityKoumoku_MojiType
Get
    Return _tokuteiDayFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tokuteiDayFlg = value
End Set
End Property


''' <summary>
''' ttyakPlaceCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ttyakPlaceCarrier() As EntityKoumoku_MojiType
Get
    Return _ttyakPlaceCarrier
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ttyakPlaceCarrier = value
End Set
End Property


''' <summary>
''' ttyakPlaceCdCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ttyakPlaceCdCarrier() As EntityKoumoku_MojiType
Get
    Return _ttyakPlaceCdCarrier
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _ttyakPlaceCdCarrier = value
End Set
End Property


''' <summary>
''' ttyakTimeCarrier
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ttyakTimeCarrier() As EntityKoumoku_NumberType
Get
    Return _ttyakTimeCarrier
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ttyakTimeCarrier = value
End Set
End Property


''' <summary>
''' tyuijikou
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tyuijikou() As EntityKoumoku_MojiType
Get
    Return _tyuijikou
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tyuijikou = value
End Set
End Property


''' <summary>
''' tyuijikouKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tyuijikouKbn() As EntityKoumoku_MojiType
Get
    Return _tyuijikouKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _tyuijikouKbn = value
End Set
End Property


''' <summary>
''' uketukeGenteiNinzu
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketukeGenteiNinzu() As EntityKoumoku_NumberType
Get
    Return _uketukeGenteiNinzu
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _uketukeGenteiNinzu = value
End Set
End Property


''' <summary>
''' uketukeStartBi
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketukeStartBi() As EntityKoumoku_NumberType
Get
    Return _uketukeStartBi
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _uketukeStartBi = value
End Set
End Property


''' <summary>
''' uketukeStartDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketukeStartDay() As EntityKoumoku_NumberType
Get
    Return _uketukeStartDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _uketukeStartDay = value
End Set
End Property


''' <summary>
''' uketukeStartKagetumae
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uketukeStartKagetumae() As EntityKoumoku_MojiType
Get
    Return _uketukeStartKagetumae
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uketukeStartKagetumae = value
End Set
End Property


''' <summary>
''' underKinsi18old
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property underKinsi18old() As EntityKoumoku_MojiType
Get
    Return _underKinsi18old
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _underKinsi18old = value
End Set
End Property


''' <summary>
''' unkyuContactDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property unkyuContactDay() As EntityKoumoku_NumberType
Get
    Return _unkyuContactDay
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _unkyuContactDay = value
End Set
End Property


''' <summary>
''' unkyuContactDoneFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property unkyuContactDoneFlg() As EntityKoumoku_MojiType
Get
    Return _unkyuContactDoneFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _unkyuContactDoneFlg = value
End Set
End Property


''' <summary>
''' unkyuContactTime
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property unkyuContactTime() As EntityKoumoku_NumberType
Get
    Return _unkyuContactTime
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _unkyuContactTime = value
End Set
End Property


''' <summary>
''' unkyuKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property unkyuKbn() As EntityKoumoku_MojiType
Get
    Return _unkyuKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _unkyuKbn = value
End Set
End Property


''' <summary>
''' tojituKokuchiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tojituKokuchiFlg() As EntityKoumoku_NumberType
Get
    Return _tojituKokuchiFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _tojituKokuchiFlg = value
End Set
End Property


''' <summary>
''' yusenYoyakuFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yusenYoyakuFlg() As EntityKoumoku_NumberType
Get
    Return _yusenYoyakuFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yusenYoyakuFlg = value
End Set
End Property


''' <summary>
''' pickupKbnFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property pickupKbnFlg() As EntityKoumoku_NumberType
Get
    Return _pickupKbnFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _pickupKbnFlg = value
End Set
End Property


''' <summary>
''' konjyoOkFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property konjyoOkFlg() As EntityKoumoku_NumberType
Get
    Return _konjyoOkFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _konjyoOkFlg = value
End Set
End Property


''' <summary>
''' tonariFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property tonariFlg() As EntityKoumoku_NumberType
Get
    Return _tonariFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _tonariFlg = value
End Set
End Property


''' <summary>
''' aheadZasekiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property aheadZasekiFlg() As EntityKoumoku_NumberType
Get
    Return _aheadZasekiFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _aheadZasekiFlg = value
End Set
End Property


''' <summary>
''' yoyakuMediaInputFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuMediaInputFlg() As EntityKoumoku_NumberType
Get
    Return _yoyakuMediaInputFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yoyakuMediaInputFlg = value
End Set
End Property


''' <summary>
''' kokusekiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property kokusekiFlg() As EntityKoumoku_NumberType
Get
    Return _kokusekiFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _kokusekiFlg = value
End Set
End Property


''' <summary>
''' sexBetuFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property sexBetuFlg() As EntityKoumoku_NumberType
Get
    Return _sexBetuFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _sexBetuFlg = value
End Set
End Property


''' <summary>
''' ageFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property ageFlg() As EntityKoumoku_NumberType
Get
    Return _ageFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _ageFlg = value
End Set
End Property


''' <summary>
''' birthdayFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property birthdayFlg() As EntityKoumoku_NumberType
Get
    Return _birthdayFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _birthdayFlg = value
End Set
End Property


''' <summary>
''' telFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property telFlg() As EntityKoumoku_NumberType
Get
    Return _telFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _telFlg = value
End Set
End Property


''' <summary>
''' addressFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property addressFlg() As EntityKoumoku_NumberType
Get
    Return _addressFlg
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _addressFlg = value
End Set
End Property


''' <summary>
''' usingFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property usingFlg() As EntityKoumoku_MojiType
Get
    Return _usingFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _usingFlg = value
End Set
End Property


''' <summary>
''' uwagiTyakuyo
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property uwagiTyakuyo() As EntityKoumoku_MojiType
Get
    Return _uwagiTyakuyo
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _uwagiTyakuyo = value
End Set
End Property


''' <summary>
''' year
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property year() As EntityKoumoku_NumberType
Get
    Return _year
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _year = value
End Set
End Property


''' <summary>
''' yobiCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yobiCd() As EntityKoumoku_MojiType
Get
    Return _yobiCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _yobiCd = value
End Set
End Property


''' <summary>
''' yoyakuAlreadyRoomNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuAlreadyRoomNum() As EntityKoumoku_NumberType
Get
    Return _yoyakuAlreadyRoomNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yoyakuAlreadyRoomNum = value
End Set
End Property


''' <summary>
''' yoyakuKanouNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuKanouNum() As EntityKoumoku_NumberType
Get
    Return _yoyakuKanouNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yoyakuKanouNum = value
End Set
End Property


''' <summary>
''' yoyakuNgFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuNgFlg() As EntityKoumoku_MojiType
Get
    Return _yoyakuNgFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _yoyakuNgFlg = value
End Set
End Property


''' <summary>
''' yoyakuNumSubSeat
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuNumSubSeat() As EntityKoumoku_NumberType
Get
    Return _yoyakuNumSubSeat
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yoyakuNumSubSeat = value
End Set
End Property


''' <summary>
''' yoyakuNumTeiseki
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuNumTeiseki() As EntityKoumoku_NumberType
Get
    Return _yoyakuNumTeiseki
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _yoyakuNumTeiseki = value
End Set
End Property


''' <summary>
''' yoyakuStopFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property yoyakuStopFlg() As EntityKoumoku_MojiType
Get
    Return _yoyakuStopFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _yoyakuStopFlg = value
End Set
End Property


''' <summary>
''' zouhatsumotogousya
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zouhatsumotogousya() As EntityKoumoku_NumberType
Get
    Return _zouhatsumotogousya
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _zouhatsumotogousya = value
End Set
End Property


''' <summary>
''' zouhatsuday
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zouhatsuday() As EntityKoumoku_NumberType
Get
    Return _zouhatsuday
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _zouhatsuday = value
End Set
End Property


''' <summary>
''' zouhatsuentrypersoncd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zouhatsuentrypersoncd() As EntityKoumoku_MojiType
Get
    Return _zouhatsuentrypersoncd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _zouhatsuentrypersoncd = value
End Set
End Property


''' <summary>
''' zasekiHihyojiFlg
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiHihyojiFlg() As EntityKoumoku_MojiType
Get
    Return _zasekiHihyojiFlg
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _zasekiHihyojiFlg = value
End Set
End Property


''' <summary>
''' zasekiReserveKbn
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property zasekiReserveKbn() As EntityKoumoku_MojiType
Get
    Return _zasekiReserveKbn
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _zasekiReserveKbn = value
End Set
End Property


''' <summary>
''' wtKakuhoSeatNum
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property wtKakuhoSeatNum() As EntityKoumoku_NumberType
Get
    Return _wtKakuhoSeatNum
End Get
Set(ByVal value As EntityKoumoku_NumberType)
    _wtKakuhoSeatNum = value
End Set
End Property


''' <summary>
''' systemEntryPgmid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryPgmid() As EntityKoumoku_MojiType
Get
    Return _systemEntryPgmid
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemEntryPgmid = value
End Set
End Property


''' <summary>
''' systemEntryPersonCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryPersonCd() As EntityKoumoku_MojiType
Get
    Return _systemEntryPersonCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemEntryPersonCd = value
End Set
End Property


''' <summary>
''' systemEntryDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemEntryDay() As EntityKoumoku_YmdType
Get
    Return _systemEntryDay
End Get
Set(ByVal value As EntityKoumoku_YmdType)
    _systemEntryDay = value
End Set
End Property


''' <summary>
''' systemUpdatePgmid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdatePgmid() As EntityKoumoku_MojiType
Get
    Return _systemUpdatePgmid
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemUpdatePgmid = value
End Set
End Property


''' <summary>
''' systemUpdatePersonCd
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdatePersonCd() As EntityKoumoku_MojiType
Get
    Return _systemUpdatePersonCd
End Get
Set(ByVal value As EntityKoumoku_MojiType)
    _systemUpdatePersonCd = value
End Set
End Property


''' <summary>
''' systemUpdateDay
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
Public Property systemUpdateDay() As EntityKoumoku_YmdType
Get
    Return _systemUpdateDay
End Get
Set(ByVal value As EntityKoumoku_YmdType)
    _systemUpdateDay = value
End Set
End Property


End Class

