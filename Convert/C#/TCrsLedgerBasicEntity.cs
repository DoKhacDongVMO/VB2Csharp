using System;
using Oracle.ManagedDataAccess.Client;


/// <summary>
/// コース台帳（基本）
/// </summary>
/// <remarks></remarks>
[Serializable()]
// コース台帳（基本）エンティティ
public partial class TCrsLedgerBasicEntity
{
    private EntityKoumoku_MojiType _crsCd = new EntityKoumoku_MojiType();	// コースコード
    private EntityKoumoku_NumberType _syuptDay = new EntityKoumoku_NumberType();	// 出発日
    private EntityKoumoku_NumberType _gousya = new EntityKoumoku_NumberType();	// 号車
    private EntityKoumoku_MojiType _accessCd = new EntityKoumoku_MojiType();	// アクセスコード
    private EntityKoumoku_MojiType _aibeyaUseFlg = new EntityKoumoku_MojiType();	// 相部屋使用フラグ
    private EntityKoumoku_NumberType _aibeyaYoyakuNinzuJyosei = new EntityKoumoku_NumberType();	// 相部屋予約人数女性
    private EntityKoumoku_NumberType _aibeyaYoyakuNinzuMale = new EntityKoumoku_NumberType();	// 相部屋予約人数男性
    private EntityKoumoku_MojiType _binName = new EntityKoumoku_MojiType();	// 便名
    private EntityKoumoku_NumberType _blockKakuhoNum = new EntityKoumoku_NumberType();	// ブロック確保数
    private EntityKoumoku_MojiType _busCompanyCd = new EntityKoumoku_MojiType();	// バス会社コード
    private EntityKoumoku_MojiType _busReserveCd = new EntityKoumoku_MojiType();	// バス指定コード
    private EntityKoumoku_MojiType _cancelNgFlg = new EntityKoumoku_MojiType();	// キャンセル不可フラグ
    private EntityKoumoku_MojiType _cancelRyouKbn = new EntityKoumoku_MojiType();	// キャンセル料区分
    private EntityKoumoku_NumberType _cancelWaitNinzu = new EntityKoumoku_NumberType();	// キャンセル待ち人数
    private EntityKoumoku_NumberType _capacityHo1kai = new EntityKoumoku_NumberType();	// 定員補１階
    private EntityKoumoku_NumberType _capacityRegular = new EntityKoumoku_NumberType();	// 定員定
    private EntityKoumoku_MojiType _carrierCd = new EntityKoumoku_MojiType();	// キャリアコード
    private EntityKoumoku_MojiType _carrierEdaban = new EntityKoumoku_MojiType();	// キャリア枝番
    private EntityKoumoku_NumberType _carNo = new EntityKoumoku_NumberType();	// 車番
    private EntityKoumoku_MojiType _carTypeCd = new EntityKoumoku_MojiType();	// 車種コード
    private EntityKoumoku_MojiType _carTypeCdYotei = new EntityKoumoku_MojiType();	// 車種コード予定
    private EntityKoumoku_MojiType _busCountFlg = new EntityKoumoku_MojiType();	// 台数カウントフラグ
    private EntityKoumoku_MojiType _categoryCd1 = new EntityKoumoku_MojiType();	// カテゴリーコード１
    private EntityKoumoku_MojiType _categoryCd2 = new EntityKoumoku_MojiType();	// カテゴリーコード２
    private EntityKoumoku_MojiType _categoryCd3 = new EntityKoumoku_MojiType();	// カテゴリーコード３
    private EntityKoumoku_MojiType _categoryCd4 = new EntityKoumoku_MojiType();	// カテゴリーコード４
    private EntityKoumoku_MojiType _costSetKbn = new EntityKoumoku_MojiType();	// 原価設定区分
    private EntityKoumoku_NumberType _crsBlockCapacity = new EntityKoumoku_NumberType();	// コースブロック定員
    private EntityKoumoku_NumberType _crsBlockOne1r = new EntityKoumoku_NumberType();	// コースブロック１名１Ｒ
    private EntityKoumoku_NumberType _crsBlockRoomNum = new EntityKoumoku_NumberType();	// コースブロックルーム数
    private EntityKoumoku_NumberType _crsBlockThree1r = new EntityKoumoku_NumberType();	// コースブロック３名１Ｒ
    private EntityKoumoku_NumberType _crsBlockTwo1r = new EntityKoumoku_NumberType();	// コースブロック２名１Ｒ
    private EntityKoumoku_NumberType _crsBlockFour1r = new EntityKoumoku_NumberType();	// コースブロック４名１Ｒ
    private EntityKoumoku_NumberType _crsBlockFive1r = new EntityKoumoku_NumberType();	// コースブロック５名１Ｒ
    private EntityKoumoku_MojiType _crsKbn1 = new EntityKoumoku_MojiType();	// コース区分１
    private EntityKoumoku_MojiType _crsKbn2 = new EntityKoumoku_MojiType();	// コース区分２
    private EntityKoumoku_MojiType _crsKind = new EntityKoumoku_MojiType();	// コース種別
    private EntityKoumoku_MojiType _managementSec = new EntityKoumoku_MojiType();	// 取扱部署
    private EntityKoumoku_MojiType _guideGengo = new EntityKoumoku_MojiType();	// ガイド言語
    private EntityKoumoku_MojiType _crsName = new EntityKoumoku_MojiType();	// コース名
    private EntityKoumoku_MojiType _crsNameKana = new EntityKoumoku_MojiType();	// コース名カナ
    private EntityKoumoku_MojiType _crsNameRk = new EntityKoumoku_MojiType();	// コース名略称
    private EntityKoumoku_MojiType _crsNameKanaRk = new EntityKoumoku_MojiType();	// コース名カナ略称
    private EntityKoumoku_NumberType _deleteDay = new EntityKoumoku_NumberType();	// 削除日
    private EntityKoumoku_NumberType _eiBlockHo = new EntityKoumoku_NumberType();	// 営ブロック補
    private EntityKoumoku_NumberType _eiBlockRegular = new EntityKoumoku_NumberType();	// 営ブロック定
    private EntityKoumoku_MojiType _endPlaceCd = new EntityKoumoku_MojiType();	// 終了場所コード
    private EntityKoumoku_NumberType _endTime = new EntityKoumoku_NumberType();	// 終了時間
    private EntityKoumoku_MojiType _haisyaKeiyuCd1 = new EntityKoumoku_MojiType();	// 配車経由コード１
    private EntityKoumoku_MojiType _haisyaKeiyuCd2 = new EntityKoumoku_MojiType();	// 配車経由コード２
    private EntityKoumoku_MojiType _haisyaKeiyuCd3 = new EntityKoumoku_MojiType();	// 配車経由コード３
    private EntityKoumoku_MojiType _haisyaKeiyuCd4 = new EntityKoumoku_MojiType();	// 配車経由コード４
    private EntityKoumoku_MojiType _haisyaKeiyuCd5 = new EntityKoumoku_MojiType();	// 配車経由コード５
    private EntityKoumoku_MojiType _homenCd = new EntityKoumoku_MojiType();	// 方面コード
    private EntityKoumoku_MojiType _houjinGaikyakuKbn = new EntityKoumoku_MojiType();	// 邦人／外客区分
    private EntityKoumoku_MojiType _hurikomiNgFlg = new EntityKoumoku_MojiType();	// 振込不可フラグ
    private EntityKoumoku_MojiType _itineraryTableCreateFlg = new EntityKoumoku_MojiType();	// 行程表作成フラグ
    private EntityKoumoku_MojiType _jyoseiSenyoSeatFlg = new EntityKoumoku_MojiType();	// 女性専用席フラグ
    private EntityKoumoku_NumberType _jyosyaCapacity = new EntityKoumoku_NumberType();	// 乗車定員
    private EntityKoumoku_NumberType _kaiteiDay = new EntityKoumoku_NumberType();	// 改定日
    private EntityKoumoku_NumberType _kusekiKakuhoNum = new EntityKoumoku_NumberType();	// 空席確保数
    private EntityKoumoku_NumberType _kusekiNumSubSeat = new EntityKoumoku_NumberType();	// 空席数補助席
    private EntityKoumoku_NumberType _kusekiNumTeiseki = new EntityKoumoku_NumberType();	// 空席数定席
    private EntityKoumoku_MojiType _kyosaiUnkouKbn = new EntityKoumoku_MojiType();	// 共催運行区分
    private EntityKoumoku_MojiType _maeuriKigen = new EntityKoumoku_MojiType();	// 前売期限
    private EntityKoumoku_MojiType _maruZouManagementKbn = new EntityKoumoku_MojiType();	// ○増管理区分
    private EntityKoumoku_NumberType _mealCountMorning = new EntityKoumoku_NumberType();	// 食事回数朝
    private EntityKoumoku_NumberType _mealCountNight = new EntityKoumoku_NumberType();	// 食事回数夜
    private EntityKoumoku_NumberType _mealCountNoon = new EntityKoumoku_NumberType();	// 食事回数昼
    private EntityKoumoku_MojiType _mediaCheckFlg = new EntityKoumoku_MojiType();	// 媒体チェックフラグ
    private EntityKoumoku_MojiType _meiboInputFlg = new EntityKoumoku_MojiType();	// 名簿入力フラグ
    private EntityKoumoku_MojiType _ninzuInputFlgKeiyu1 = new EntityKoumoku_MojiType();	// 乗車人数入力済フラグ配車経由１
    private EntityKoumoku_MojiType _ninzuInputFlgKeiyu2 = new EntityKoumoku_MojiType();	// 乗車人数入力済フラグ配車経由２
    private EntityKoumoku_MojiType _ninzuInputFlgKeiyu3 = new EntityKoumoku_MojiType();	// 乗車人数入力済フラグ配車経由３
    private EntityKoumoku_MojiType _ninzuInputFlgKeiyu4 = new EntityKoumoku_MojiType();	// 乗車人数入力済フラグ配車経由４
    private EntityKoumoku_MojiType _ninzuInputFlgKeiyu5 = new EntityKoumoku_MojiType();	// 乗車人数入力済フラグ配車経由５
    private EntityKoumoku_NumberType _ninzuKeiyu1Adult = new EntityKoumoku_NumberType();	// 乗車人数配車経由１大人
    private EntityKoumoku_NumberType _ninzuKeiyu1Child = new EntityKoumoku_NumberType();	// 乗車人数配車経由１小人
    private EntityKoumoku_NumberType _ninzuKeiyu1Junior = new EntityKoumoku_NumberType();	// 乗車人数配車経由１中人
    private EntityKoumoku_NumberType _ninzuKeiyu1S = new EntityKoumoku_NumberType();	// 乗車人数配車経由１招
    private EntityKoumoku_NumberType _ninzuKeiyu2Adult = new EntityKoumoku_NumberType();	// 乗車人数配車経由２大人
    private EntityKoumoku_NumberType _ninzuKeiyu2Child = new EntityKoumoku_NumberType();	// 乗車人数配車経由２小人
    private EntityKoumoku_NumberType _ninzuKeiyu2Junior = new EntityKoumoku_NumberType();	// 乗車人数配車経由２中人
    private EntityKoumoku_NumberType _ninzuKeiyu2S = new EntityKoumoku_NumberType();	// 乗車人数配車経由２招
    private EntityKoumoku_NumberType _ninzuKeiyu3Adult = new EntityKoumoku_NumberType();	// 乗車人数配車経由３大人
    private EntityKoumoku_NumberType _ninzuKeiyu3Child = new EntityKoumoku_NumberType();	// 乗車人数配車経由３小人
    private EntityKoumoku_NumberType _ninzuKeiyu3Junior = new EntityKoumoku_NumberType();	// 乗車人数配車経由３中人
    private EntityKoumoku_NumberType _ninzuKeiyu3S = new EntityKoumoku_NumberType();	// 乗車人数配車経由３招
    private EntityKoumoku_NumberType _ninzuKeiyu4Adult = new EntityKoumoku_NumberType();	// 乗車人数配車経由４大人
    private EntityKoumoku_NumberType _ninzuKeiyu4Child = new EntityKoumoku_NumberType();	// 乗車人数配車経由４小人
    private EntityKoumoku_NumberType _ninzuKeiyu4Junior = new EntityKoumoku_NumberType();	// 乗車人数配車経由４中人
    private EntityKoumoku_NumberType _ninzuKeiyu4S = new EntityKoumoku_NumberType();	// 乗車人数配車経由４招
    private EntityKoumoku_NumberType _ninzuKeiyu5Adult = new EntityKoumoku_NumberType();	// 乗車人数配車経由５大人
    private EntityKoumoku_NumberType _ninzuKeiyu5Child = new EntityKoumoku_NumberType();	// 乗車人数配車経由５小人
    private EntityKoumoku_NumberType _ninzuKeiyu5Junior = new EntityKoumoku_NumberType();	// 乗車人数配車経由５中人
    private EntityKoumoku_NumberType _ninzuKeiyu5S = new EntityKoumoku_NumberType();	// 乗車人数配車経由５招
    private EntityKoumoku_MojiType _oneSankaFlg = new EntityKoumoku_MojiType();	// １名参加フラグ
    private EntityKoumoku_MojiType _optionFlg = new EntityKoumoku_MojiType();	// オプションフラグ
    private EntityKoumoku_MojiType _pickupKbn1 = new EntityKoumoku_MojiType();	// ピックアップ区分１
    private EntityKoumoku_MojiType _pickupKbn2 = new EntityKoumoku_MojiType();	// ピックアップ区分２
    private EntityKoumoku_MojiType _pickupKbn3 = new EntityKoumoku_MojiType();	// ピックアップ区分３
    private EntityKoumoku_MojiType _pickupKbn4 = new EntityKoumoku_MojiType();	// ピックアップ区分４
    private EntityKoumoku_MojiType _pickupKbn5 = new EntityKoumoku_MojiType();	// ピックアップ区分５
    private EntityKoumoku_NumberType _returnDay = new EntityKoumoku_NumberType();	// 帰着日
    private EntityKoumoku_NumberType _roomZansuOneRoom = new EntityKoumoku_NumberType();	// 部屋残数１人部屋
    private EntityKoumoku_NumberType _roomZansuSokei = new EntityKoumoku_NumberType();	// 部屋残数総計
    private EntityKoumoku_NumberType _roomZansuThreeRoom = new EntityKoumoku_NumberType();	// 部屋残数３人部屋
    private EntityKoumoku_NumberType _roomZansuTwoRoom = new EntityKoumoku_NumberType();	// 部屋残数２人部屋
    private EntityKoumoku_NumberType _roomZansuFourRoom = new EntityKoumoku_NumberType();	// 部屋残数４人部屋
    private EntityKoumoku_NumberType _roomZansuFiveRoom = new EntityKoumoku_NumberType();	// 部屋残数５人部屋
    private EntityKoumoku_NumberType _minSaikouNinzu = new EntityKoumoku_NumberType();	// 最小催行人数
    private EntityKoumoku_NumberType _saikouDay = new EntityKoumoku_NumberType();	// 催行日
    private EntityKoumoku_MojiType _saikouKakuteiKbn = new EntityKoumoku_MojiType();	// 催行確定区分
    private EntityKoumoku_MojiType _seasonCd = new EntityKoumoku_MojiType();	// 季コード
    private EntityKoumoku_MojiType _senyoCrsKbn = new EntityKoumoku_MojiType();	// 専用コース区分
    private EntityKoumoku_MojiType _shanaiContactForMessage = new EntityKoumoku_MojiType();	// 社内連絡用メッセージ
    private EntityKoumoku_MojiType _shukujitsuFlg = new EntityKoumoku_MojiType();	// 祝日フラグ
    private EntityKoumoku_NumberType _sinsetuYm = new EntityKoumoku_NumberType();	// 新設年月
    private EntityKoumoku_NumberType _stayDay = new EntityKoumoku_NumberType();	// 宿泊日
    private EntityKoumoku_NumberType _stayStay = new EntityKoumoku_NumberType();	// 宿泊泊
    private EntityKoumoku_MojiType _subSeatOkKbn = new EntityKoumoku_MojiType();	// 補助席可区分
    private EntityKoumoku_NumberType _syoyoTime = new EntityKoumoku_NumberType();	// 所要時間
    private EntityKoumoku_MojiType _syugoPlaceCdCarrier = new EntityKoumoku_MojiType();	// 集合場所コードキャリア
    private EntityKoumoku_NumberType _syugoTime1 = new EntityKoumoku_NumberType();	// 集合時間１
    private EntityKoumoku_NumberType _syugoTime2 = new EntityKoumoku_NumberType();	// 集合時間２
    private EntityKoumoku_NumberType _syugoTime3 = new EntityKoumoku_NumberType();	// 集合時間３
    private EntityKoumoku_NumberType _syugoTime4 = new EntityKoumoku_NumberType();	// 集合時間４
    private EntityKoumoku_NumberType _syugoTime5 = new EntityKoumoku_NumberType();	// 集合時間５
    private EntityKoumoku_NumberType _syugoTimeCarrier = new EntityKoumoku_NumberType();	// 集合時間キャリア
    private EntityKoumoku_MojiType _syuptJiCarrierKbn = new EntityKoumoku_MojiType();	// 出発時キャリア区分
    private EntityKoumoku_MojiType _syuptPlaceCarrier = new EntityKoumoku_MojiType();	// 出発場所キャリア
    private EntityKoumoku_MojiType _syuptPlaceCdCarrier = new EntityKoumoku_MojiType();	// 出発場所コードキャリア
    private EntityKoumoku_NumberType _syuptTime1 = new EntityKoumoku_NumberType();	// 出発時間１
    private EntityKoumoku_NumberType _syuptTime2 = new EntityKoumoku_NumberType();	// 出発時間２
    private EntityKoumoku_NumberType _syuptTime3 = new EntityKoumoku_NumberType();	// 出発時間３
    private EntityKoumoku_NumberType _syuptTime4 = new EntityKoumoku_NumberType();	// 出発時間４
    private EntityKoumoku_NumberType _syuptTime5 = new EntityKoumoku_NumberType();	// 出発時間５
    private EntityKoumoku_NumberType _syuptTimeCarrier = new EntityKoumoku_NumberType();	// 出発時間キャリア
    private EntityKoumoku_MojiType _teiinseiFlg = new EntityKoumoku_MojiType();	// 定員制フラグ
    private EntityKoumoku_MojiType _teikiCrsKbn = new EntityKoumoku_MojiType();	// 定期コース区分
    private EntityKoumoku_MojiType _teikiKikakuKbn = new EntityKoumoku_MojiType();	// 定期・企画区分
    private EntityKoumoku_MojiType _tejimaiContactKbn = new EntityKoumoku_MojiType();	// 手仕舞連絡区分
    private EntityKoumoku_NumberType _tejimaiDay = new EntityKoumoku_NumberType();	// 手仕舞日
    private EntityKoumoku_MojiType _tejimaiKbn = new EntityKoumoku_MojiType();	// 手仕舞区分
    private EntityKoumoku_MojiType _tenjyoinCd = new EntityKoumoku_MojiType();	// 添乗員コード
    private EntityKoumoku_MojiType _tieTyakuyo = new EntityKoumoku_MojiType();	// ネクタイ着用
    private EntityKoumoku_MojiType _tokuteiChargeSet = new EntityKoumoku_MojiType();	// 特定料金設定
    private EntityKoumoku_MojiType _tokuteiDayFlg = new EntityKoumoku_MojiType();	// 特定日フラグ
    private EntityKoumoku_MojiType _ttyakPlaceCarrier = new EntityKoumoku_MojiType();	// 到着場所キャリア
    private EntityKoumoku_MojiType _ttyakPlaceCdCarrier = new EntityKoumoku_MojiType();	// 到着場所コードキャリア
    private EntityKoumoku_NumberType _ttyakTimeCarrier = new EntityKoumoku_NumberType();	// 到着時間キャリア
    private EntityKoumoku_MojiType _tyuijikou = new EntityKoumoku_MojiType();	// 注意事項
    private EntityKoumoku_MojiType _tyuijikouKbn = new EntityKoumoku_MojiType();	// 注意事項区分
    private EntityKoumoku_NumberType _uketukeGenteiNinzu = new EntityKoumoku_NumberType();	// 受付限定人数
    private EntityKoumoku_NumberType _uketukeStartBi = new EntityKoumoku_NumberType();	// 受付開始日
    private EntityKoumoku_NumberType _uketukeStartDay = new EntityKoumoku_NumberType();	// 受付開始日
    private EntityKoumoku_MojiType _uketukeStartKagetumae = new EntityKoumoku_MojiType();	// 受付開始ヶ月前
    private EntityKoumoku_MojiType _underKinsi18old = new EntityKoumoku_MojiType();	// １８才未満禁
    private EntityKoumoku_NumberType _unkyuContactDay = new EntityKoumoku_NumberType();	// 運休連絡日
    private EntityKoumoku_MojiType _unkyuContactDoneFlg = new EntityKoumoku_MojiType();	// 運休連絡完了フラグ
    private EntityKoumoku_NumberType _unkyuContactTime = new EntityKoumoku_NumberType();	// 運休連絡時刻
    private EntityKoumoku_MojiType _unkyuKbn = new EntityKoumoku_MojiType();	// 運休区分
    private EntityKoumoku_NumberType _tojituKokuchiFlg = new EntityKoumoku_NumberType();	// 当日告知フラグ
    private EntityKoumoku_NumberType _yusenYoyakuFlg = new EntityKoumoku_NumberType();	// 優先予約フラグ
    private EntityKoumoku_NumberType _pickupKbnFlg = new EntityKoumoku_NumberType();	// ピックアップフラグ
    private EntityKoumoku_NumberType _konjyoOkFlg = new EntityKoumoku_NumberType();	// 混乗可フラグ
    private EntityKoumoku_NumberType _tonariFlg = new EntityKoumoku_NumberType();	// 隣席フラグ
    private EntityKoumoku_NumberType _aheadZasekiFlg = new EntityKoumoku_NumberType();	// 前方座席フラグ
    private EntityKoumoku_NumberType _yoyakuMediaInputFlg = new EntityKoumoku_NumberType();	// 予約媒体入力フラグ
    private EntityKoumoku_NumberType _kokusekiFlg = new EntityKoumoku_NumberType();	// 国籍入力フラグ
    private EntityKoumoku_NumberType _sexBetuFlg = new EntityKoumoku_NumberType();	// 性別入力フラグ
    private EntityKoumoku_NumberType _ageFlg = new EntityKoumoku_NumberType();	// 年齢フラグ
    private EntityKoumoku_NumberType _birthdayFlg = new EntityKoumoku_NumberType();	// 生年月日フラグ
    private EntityKoumoku_NumberType _telFlg = new EntityKoumoku_NumberType();	// 電話番号フラグ
    private EntityKoumoku_NumberType _addressFlg = new EntityKoumoku_NumberType();	// 住所フラグ
    private EntityKoumoku_MojiType _usingFlg = new EntityKoumoku_MojiType();	// 使用中フラグ
    private EntityKoumoku_MojiType _uwagiTyakuyo = new EntityKoumoku_MojiType();	// 上着着用
    private EntityKoumoku_NumberType _year = new EntityKoumoku_NumberType();	// 年
    private EntityKoumoku_MojiType _yobiCd = new EntityKoumoku_MojiType();	// 曜日コード
    private EntityKoumoku_NumberType _yoyakuAlreadyRoomNum = new EntityKoumoku_NumberType();	// 予約済ＲＯＯＭ数
    private EntityKoumoku_NumberType _yoyakuKanouNum = new EntityKoumoku_NumberType();	// 予約可能数
    private EntityKoumoku_MojiType _yoyakuNgFlg = new EntityKoumoku_MojiType();	// 予約不可フラグ
    private EntityKoumoku_NumberType _yoyakuNumSubSeat = new EntityKoumoku_NumberType();	// 予約数補助席
    private EntityKoumoku_NumberType _yoyakuNumTeiseki = new EntityKoumoku_NumberType();	// 予約数定席
    private EntityKoumoku_MojiType _yoyakuStopFlg = new EntityKoumoku_MojiType();	// 予約停止フラグ
    private EntityKoumoku_NumberType _zouhatsumotogousya = new EntityKoumoku_NumberType();	// 増発元号車
    private EntityKoumoku_NumberType _zouhatsuday = new EntityKoumoku_NumberType();	// 増発日
    private EntityKoumoku_MojiType _zouhatsuentrypersoncd = new EntityKoumoku_MojiType();	// 増発実施者
    private EntityKoumoku_MojiType _zasekiHihyojiFlg = new EntityKoumoku_MojiType();	// 予約時座席非表示フラグ
    private EntityKoumoku_MojiType _zasekiReserveKbn = new EntityKoumoku_MojiType();	// 座席指定区分
    private EntityKoumoku_NumberType _wtKakuhoSeatNum = new EntityKoumoku_NumberType();	// ＷＴ確保席数
    private EntityKoumoku_MojiType _systemEntryPgmid = new EntityKoumoku_MojiType();	// システム登録ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemEntryPersonCd = new EntityKoumoku_MojiType();	// システム登録者コード
    private EntityKoumoku_YmdType _systemEntryDay = new EntityKoumoku_YmdType();	// システム登録日
    private EntityKoumoku_MojiType _systemUpdatePgmid = new EntityKoumoku_MojiType();	// システム更新ＰＧＭＩＤ
    private EntityKoumoku_MojiType _systemUpdatePersonCd = new EntityKoumoku_MojiType();	// システム更新者コード
    private EntityKoumoku_YmdType _systemUpdateDay = new EntityKoumoku_YmdType();	// システム更新日

    public TCrsLedgerBasicEntity()
    {
        _crsCd.PhysicsName = "CRS_CD";
        _syuptDay.PhysicsName = "SYUPT_DAY";
        _gousya.PhysicsName = "GOUSYA";
        _accessCd.PhysicsName = "ACCESS_CD";
        _aibeyaUseFlg.PhysicsName = "AIBEYA_USE_FLG";
        _aibeyaYoyakuNinzuJyosei.PhysicsName = "AIBEYA_YOYAKU_NINZU_JYOSEI";
        _aibeyaYoyakuNinzuMale.PhysicsName = "AIBEYA_YOYAKU_NINZU_MALE";
        _binName.PhysicsName = "BIN_NAME";
        _blockKakuhoNum.PhysicsName = "BLOCK_KAKUHO_NUM";
        _busCompanyCd.PhysicsName = "BUS_COMPANY_CD";
        _busReserveCd.PhysicsName = "BUS_RESERVE_CD";
        _cancelNgFlg.PhysicsName = "CANCEL_NG_FLG";
        _cancelRyouKbn.PhysicsName = "CANCEL_RYOU_KBN";
        _cancelWaitNinzu.PhysicsName = "CANCEL_WAIT_NINZU";
        _capacityHo1kai.PhysicsName = "CAPACITY_HO_1KAI";
        _capacityRegular.PhysicsName = "CAPACITY_REGULAR";
        _carrierCd.PhysicsName = "CARRIER_CD";
        _carrierEdaban.PhysicsName = "CARRIER_EDABAN";
        _carNo.PhysicsName = "CAR_NO";
        _carTypeCd.PhysicsName = "CAR_TYPE_CD";
        _carTypeCdYotei.PhysicsName = "CAR_TYPE_CD_YOTEI";
        _busCountFlg.PhysicsName = "BUS_COUNT_FLG";
        _categoryCd1.PhysicsName = "CATEGORY_CD_1";
        _categoryCd2.PhysicsName = "CATEGORY_CD_2";
        _categoryCd3.PhysicsName = "CATEGORY_CD_3";
        _categoryCd4.PhysicsName = "CATEGORY_CD_4";
        _costSetKbn.PhysicsName = "COST_SET_KBN";
        _crsBlockCapacity.PhysicsName = "CRS_BLOCK_CAPACITY";
        _crsBlockOne1r.PhysicsName = "CRS_BLOCK_ONE_1R";
        _crsBlockRoomNum.PhysicsName = "CRS_BLOCK_ROOM_NUM";
        _crsBlockThree1r.PhysicsName = "CRS_BLOCK_THREE_1R";
        _crsBlockTwo1r.PhysicsName = "CRS_BLOCK_TWO_1R";
        _crsBlockFour1r.PhysicsName = "CRS_BLOCK_FOUR_1R";
        _crsBlockFive1r.PhysicsName = "CRS_BLOCK_FIVE_1R";
        _crsKbn1.PhysicsName = "CRS_KBN_1";
        _crsKbn2.PhysicsName = "CRS_KBN_2";
        _crsKind.PhysicsName = "CRS_KIND";
        _managementSec.PhysicsName = "MANAGEMENT_SEC";
        _guideGengo.PhysicsName = "GUIDE_GENGO";
        _crsName.PhysicsName = "CRS_NAME";
        _crsNameKana.PhysicsName = "CRS_NAME_KANA";
        _crsNameRk.PhysicsName = "CRS_NAME_RK";
        _crsNameKanaRk.PhysicsName = "CRS_NAME_KANA_RK";
        _deleteDay.PhysicsName = "DELETE_DAY";
        _eiBlockHo.PhysicsName = "EI_BLOCK_HO";
        _eiBlockRegular.PhysicsName = "EI_BLOCK_REGULAR";
        _endPlaceCd.PhysicsName = "END_PLACE_CD";
        _endTime.PhysicsName = "END_TIME";
        _haisyaKeiyuCd1.PhysicsName = "HAISYA_KEIYU_CD_1";
        _haisyaKeiyuCd2.PhysicsName = "HAISYA_KEIYU_CD_2";
        _haisyaKeiyuCd3.PhysicsName = "HAISYA_KEIYU_CD_3";
        _haisyaKeiyuCd4.PhysicsName = "HAISYA_KEIYU_CD_4";
        _haisyaKeiyuCd5.PhysicsName = "HAISYA_KEIYU_CD_5";
        _homenCd.PhysicsName = "HOMEN_CD";
        _houjinGaikyakuKbn.PhysicsName = "HOUJIN_GAIKYAKU_KBN";
        _hurikomiNgFlg.PhysicsName = "HURIKOMI_NG_FLG";
        _itineraryTableCreateFlg.PhysicsName = "ITINERARY_TABLE_CREATE_FLG";
        _jyoseiSenyoSeatFlg.PhysicsName = "JYOSEI_SENYO_SEAT_FLG";
        _jyosyaCapacity.PhysicsName = "JYOSYA_CAPACITY";
        _kaiteiDay.PhysicsName = "KAITEI_DAY";
        _kusekiKakuhoNum.PhysicsName = "KUSEKI_KAKUHO_NUM";
        _kusekiNumSubSeat.PhysicsName = "KUSEKI_NUM_SUB_SEAT";
        _kusekiNumTeiseki.PhysicsName = "KUSEKI_NUM_TEISEKI";
        _kyosaiUnkouKbn.PhysicsName = "KYOSAI_UNKOU_KBN";
        _maeuriKigen.PhysicsName = "MAEURI_KIGEN";
        _maruZouManagementKbn.PhysicsName = "MARU_ZOU_MANAGEMENT_KBN";
        _mealCountMorning.PhysicsName = "MEAL_COUNT_MORNING";
        _mealCountNight.PhysicsName = "MEAL_COUNT_NIGHT";
        _mealCountNoon.PhysicsName = "MEAL_COUNT_NOON";
        _mediaCheckFlg.PhysicsName = "MEDIA_CHECK_FLG";
        _meiboInputFlg.PhysicsName = "MEIBO_INPUT_FLG";
        _ninzuInputFlgKeiyu1.PhysicsName = "NINZU_INPUT_FLG_KEIYU_1";
        _ninzuInputFlgKeiyu2.PhysicsName = "NINZU_INPUT_FLG_KEIYU_2";
        _ninzuInputFlgKeiyu3.PhysicsName = "NINZU_INPUT_FLG_KEIYU_3";
        _ninzuInputFlgKeiyu4.PhysicsName = "NINZU_INPUT_FLG_KEIYU_4";
        _ninzuInputFlgKeiyu5.PhysicsName = "NINZU_INPUT_FLG_KEIYU_5";
        _ninzuKeiyu1Adult.PhysicsName = "NINZU_KEIYU_1_ADULT";
        _ninzuKeiyu1Child.PhysicsName = "NINZU_KEIYU_1_CHILD";
        _ninzuKeiyu1Junior.PhysicsName = "NINZU_KEIYU_1_JUNIOR";
        _ninzuKeiyu1S.PhysicsName = "NINZU_KEIYU_1_S";
        _ninzuKeiyu2Adult.PhysicsName = "NINZU_KEIYU_2_ADULT";
        _ninzuKeiyu2Child.PhysicsName = "NINZU_KEIYU_2_CHILD";
        _ninzuKeiyu2Junior.PhysicsName = "NINZU_KEIYU_2_JUNIOR";
        _ninzuKeiyu2S.PhysicsName = "NINZU_KEIYU_2_S";
        _ninzuKeiyu3Adult.PhysicsName = "NINZU_KEIYU_3_ADULT";
        _ninzuKeiyu3Child.PhysicsName = "NINZU_KEIYU_3_CHILD";
        _ninzuKeiyu3Junior.PhysicsName = "NINZU_KEIYU_3_JUNIOR";
        _ninzuKeiyu3S.PhysicsName = "NINZU_KEIYU_3_S";
        _ninzuKeiyu4Adult.PhysicsName = "NINZU_KEIYU_4_ADULT";
        _ninzuKeiyu4Child.PhysicsName = "NINZU_KEIYU_4_CHILD";
        _ninzuKeiyu4Junior.PhysicsName = "NINZU_KEIYU_4_JUNIOR";
        _ninzuKeiyu4S.PhysicsName = "NINZU_KEIYU_4_S";
        _ninzuKeiyu5Adult.PhysicsName = "NINZU_KEIYU_5_ADULT";
        _ninzuKeiyu5Child.PhysicsName = "NINZU_KEIYU_5_CHILD";
        _ninzuKeiyu5Junior.PhysicsName = "NINZU_KEIYU_5_JUNIOR";
        _ninzuKeiyu5S.PhysicsName = "NINZU_KEIYU_5_S";
        _oneSankaFlg.PhysicsName = "ONE_SANKA_FLG";
        _optionFlg.PhysicsName = "OPTION_FLG";
        _pickupKbn1.PhysicsName = "PICKUP_KBN_1";
        _pickupKbn2.PhysicsName = "PICKUP_KBN_2";
        _pickupKbn3.PhysicsName = "PICKUP_KBN_3";
        _pickupKbn4.PhysicsName = "PICKUP_KBN_4";
        _pickupKbn5.PhysicsName = "PICKUP_KBN_5";
        _returnDay.PhysicsName = "RETURN_DAY";
        _roomZansuOneRoom.PhysicsName = "ROOM_ZANSU_ONE_ROOM";
        _roomZansuSokei.PhysicsName = "ROOM_ZANSU_SOKEI";
        _roomZansuThreeRoom.PhysicsName = "ROOM_ZANSU_THREE_ROOM";
        _roomZansuTwoRoom.PhysicsName = "ROOM_ZANSU_TWO_ROOM";
        _roomZansuFourRoom.PhysicsName = "ROOM_ZANSU_FOUR_ROOM";
        _roomZansuFiveRoom.PhysicsName = "ROOM_ZANSU_FIVE_ROOM";
        _minSaikouNinzu.PhysicsName = "MIN_SAIKOU_NINZU";
        _saikouDay.PhysicsName = "SAIKOU_DAY";
        _saikouKakuteiKbn.PhysicsName = "SAIKOU_KAKUTEI_KBN";
        _seasonCd.PhysicsName = "SEASON_CD";
        _senyoCrsKbn.PhysicsName = "SENYO_CRS_KBN";
        _shanaiContactForMessage.PhysicsName = "SHANAI_CONTACT_FOR_MESSAGE";
        _shukujitsuFlg.PhysicsName = "SHUKUJITSU_FLG";
        _sinsetuYm.PhysicsName = "SINSETU_YM";
        _stayDay.PhysicsName = "STAY_DAY";
        _stayStay.PhysicsName = "STAY_STAY";
        _subSeatOkKbn.PhysicsName = "SUB_SEAT_OK_KBN";
        _syoyoTime.PhysicsName = "SYOYO_TIME";
        _syugoPlaceCdCarrier.PhysicsName = "SYUGO_PLACE_CD_CARRIER";
        _syugoTime1.PhysicsName = "SYUGO_TIME_1";
        _syugoTime2.PhysicsName = "SYUGO_TIME_2";
        _syugoTime3.PhysicsName = "SYUGO_TIME_3";
        _syugoTime4.PhysicsName = "SYUGO_TIME_4";
        _syugoTime5.PhysicsName = "SYUGO_TIME_5";
        _syugoTimeCarrier.PhysicsName = "SYUGO_TIME_CARRIER";
        _syuptJiCarrierKbn.PhysicsName = "SYUPT_JI_CARRIER_KBN";
        _syuptPlaceCarrier.PhysicsName = "SYUPT_PLACE_CARRIER";
        _syuptPlaceCdCarrier.PhysicsName = "SYUPT_PLACE_CD_CARRIER";
        _syuptTime1.PhysicsName = "SYUPT_TIME_1";
        _syuptTime2.PhysicsName = "SYUPT_TIME_2";
        _syuptTime3.PhysicsName = "SYUPT_TIME_3";
        _syuptTime4.PhysicsName = "SYUPT_TIME_4";
        _syuptTime5.PhysicsName = "SYUPT_TIME_5";
        _syuptTimeCarrier.PhysicsName = "SYUPT_TIME_CARRIER";
        _teiinseiFlg.PhysicsName = "TEIINSEI_FLG";
        _teikiCrsKbn.PhysicsName = "TEIKI_CRS_KBN";
        _teikiKikakuKbn.PhysicsName = "TEIKI_KIKAKU_KBN";
        _tejimaiContactKbn.PhysicsName = "TEJIMAI_CONTACT_KBN";
        _tejimaiDay.PhysicsName = "TEJIMAI_DAY";
        _tejimaiKbn.PhysicsName = "TEJIMAI_KBN";
        _tenjyoinCd.PhysicsName = "TENJYOIN_CD";
        _tieTyakuyo.PhysicsName = "TIE_TYAKUYO";
        _tokuteiChargeSet.PhysicsName = "TOKUTEI_CHARGE_SET";
        _tokuteiDayFlg.PhysicsName = "TOKUTEI_DAY_FLG";
        _ttyakPlaceCarrier.PhysicsName = "TTYAK_PLACE_CARRIER";
        _ttyakPlaceCdCarrier.PhysicsName = "TTYAK_PLACE_CD_CARRIER";
        _ttyakTimeCarrier.PhysicsName = "TTYAK_TIME_CARRIER";
        _tyuijikou.PhysicsName = "TYUIJIKOU";
        _tyuijikouKbn.PhysicsName = "TYUIJIKOU_KBN";
        _uketukeGenteiNinzu.PhysicsName = "UKETUKE_GENTEI_NINZU";
        _uketukeStartBi.PhysicsName = "UKETUKE_START_BI";
        _uketukeStartDay.PhysicsName = "UKETUKE_START_DAY";
        _uketukeStartKagetumae.PhysicsName = "UKETUKE_START_KAGETUMAE";
        _underKinsi18old.PhysicsName = "UNDER_KINSI_18OLD";
        _unkyuContactDay.PhysicsName = "UNKYU_CONTACT_DAY";
        _unkyuContactDoneFlg.PhysicsName = "UNKYU_CONTACT_DONE_FLG";
        _unkyuContactTime.PhysicsName = "UNKYU_CONTACT_TIME";
        _unkyuKbn.PhysicsName = "UNKYU_KBN";
        _tojituKokuchiFlg.PhysicsName = "TOJITU_KOKUCHI_FLG";
        _yusenYoyakuFlg.PhysicsName = "YUSEN_YOYAKU_FLG";
        _pickupKbnFlg.PhysicsName = "PICKUP_KBN_FLG";
        _konjyoOkFlg.PhysicsName = "KONJYO_OK_FLG";
        _tonariFlg.PhysicsName = "TONARI_FLG";
        _aheadZasekiFlg.PhysicsName = "AHEAD_ZASEKI_FLG";
        _yoyakuMediaInputFlg.PhysicsName = "YOYAKU_MEDIA_INPUT_FLG";
        _kokusekiFlg.PhysicsName = "KOKUSEKI_FLG";
        _sexBetuFlg.PhysicsName = "SEX_BETU_FLG";
        _ageFlg.PhysicsName = "AGE_FLG";
        _birthdayFlg.PhysicsName = "BIRTHDAY_FLG";
        _telFlg.PhysicsName = "TEL_FLG";
        _addressFlg.PhysicsName = "ADDRESS_FLG";
        _usingFlg.PhysicsName = "USING_FLG";
        _uwagiTyakuyo.PhysicsName = "UWAGI_TYAKUYO";
        _year.PhysicsName = "YEAR";
        _yobiCd.PhysicsName = "YOBI_CD";
        _yoyakuAlreadyRoomNum.PhysicsName = "YOYAKU_ALREADY_ROOM_NUM";
        _yoyakuKanouNum.PhysicsName = "YOYAKU_KANOU_NUM";
        _yoyakuNgFlg.PhysicsName = "YOYAKU_NG_FLG";
        _yoyakuNumSubSeat.PhysicsName = "YOYAKU_NUM_SUB_SEAT";
        _yoyakuNumTeiseki.PhysicsName = "YOYAKU_NUM_TEISEKI";
        _yoyakuStopFlg.PhysicsName = "YOYAKU_STOP_FLG";
        _zouhatsumotogousya.PhysicsName = "ZOUHATSUMOTO_GOUSYA";
        _zouhatsuday.PhysicsName = "ZOUHATSU_DAY";
        _zouhatsuentrypersoncd.PhysicsName = "ZOUHATSU_ENTRY_PERSON_CD";
        _zasekiHihyojiFlg.PhysicsName = "ZASEKI_HIHYOJI_FLG";
        _zasekiReserveKbn.PhysicsName = "ZASEKI_RESERVE_KBN";
        _wtKakuhoSeatNum.PhysicsName = "WT_KAKUHO_SEAT_NUM";
        _systemEntryPgmid.PhysicsName = "SYSTEM_ENTRY_PGMID";
        _systemEntryPersonCd.PhysicsName = "SYSTEM_ENTRY_PERSON_CD";
        _systemEntryDay.PhysicsName = "SYSTEM_ENTRY_DAY";
        _systemUpdatePgmid.PhysicsName = "SYSTEM_UPDATE_PGMID";
        _systemUpdatePersonCd.PhysicsName = "SYSTEM_UPDATE_PERSON_CD";
        _systemUpdateDay.PhysicsName = "SYSTEM_UPDATE_DAY";
        _crsCd.Required = false;
        _syuptDay.Required = false;
        _gousya.Required = false;
        _accessCd.Required = false;
        _aibeyaUseFlg.Required = false;
        _aibeyaYoyakuNinzuJyosei.Required = false;
        _aibeyaYoyakuNinzuMale.Required = false;
        _binName.Required = false;
        _blockKakuhoNum.Required = false;
        _busCompanyCd.Required = false;
        _busReserveCd.Required = false;
        _cancelNgFlg.Required = false;
        _cancelRyouKbn.Required = false;
        _cancelWaitNinzu.Required = false;
        _capacityHo1kai.Required = false;
        _capacityRegular.Required = false;
        _carrierCd.Required = false;
        _carrierEdaban.Required = false;
        _carNo.Required = false;
        _carTypeCd.Required = false;
        _carTypeCdYotei.Required = false;
        _busCountFlg.Required = false;
        _categoryCd1.Required = false;
        _categoryCd2.Required = false;
        _categoryCd3.Required = false;
        _categoryCd4.Required = false;
        _costSetKbn.Required = false;
        _crsBlockCapacity.Required = false;
        _crsBlockOne1r.Required = false;
        _crsBlockRoomNum.Required = false;
        _crsBlockThree1r.Required = false;
        _crsBlockTwo1r.Required = false;
        _crsBlockFour1r.Required = false;
        _crsBlockFive1r.Required = false;
        _crsKbn1.Required = false;
        _crsKbn2.Required = false;
        _crsKind.Required = false;
        _managementSec.Required = false;
        _guideGengo.Required = false;
        _crsName.Required = false;
        _crsNameKana.Required = false;
        _crsNameRk.Required = false;
        _crsNameKanaRk.Required = false;
        _deleteDay.Required = false;
        _eiBlockHo.Required = false;
        _eiBlockRegular.Required = false;
        _endPlaceCd.Required = false;
        _endTime.Required = false;
        _haisyaKeiyuCd1.Required = false;
        _haisyaKeiyuCd2.Required = false;
        _haisyaKeiyuCd3.Required = false;
        _haisyaKeiyuCd4.Required = false;
        _haisyaKeiyuCd5.Required = false;
        _homenCd.Required = false;
        _houjinGaikyakuKbn.Required = false;
        _hurikomiNgFlg.Required = false;
        _itineraryTableCreateFlg.Required = false;
        _jyoseiSenyoSeatFlg.Required = false;
        _jyosyaCapacity.Required = false;
        _kaiteiDay.Required = false;
        _kusekiKakuhoNum.Required = false;
        _kusekiNumSubSeat.Required = false;
        _kusekiNumTeiseki.Required = false;
        _kyosaiUnkouKbn.Required = false;
        _maeuriKigen.Required = false;
        _maruZouManagementKbn.Required = false;
        _mealCountMorning.Required = false;
        _mealCountNight.Required = false;
        _mealCountNoon.Required = false;
        _mediaCheckFlg.Required = false;
        _meiboInputFlg.Required = false;
        _ninzuInputFlgKeiyu1.Required = false;
        _ninzuInputFlgKeiyu2.Required = false;
        _ninzuInputFlgKeiyu3.Required = false;
        _ninzuInputFlgKeiyu4.Required = false;
        _ninzuInputFlgKeiyu5.Required = false;
        _ninzuKeiyu1Adult.Required = false;
        _ninzuKeiyu1Child.Required = false;
        _ninzuKeiyu1Junior.Required = false;
        _ninzuKeiyu1S.Required = false;
        _ninzuKeiyu2Adult.Required = false;
        _ninzuKeiyu2Child.Required = false;
        _ninzuKeiyu2Junior.Required = false;
        _ninzuKeiyu2S.Required = false;
        _ninzuKeiyu3Adult.Required = false;
        _ninzuKeiyu3Child.Required = false;
        _ninzuKeiyu3Junior.Required = false;
        _ninzuKeiyu3S.Required = false;
        _ninzuKeiyu4Adult.Required = false;
        _ninzuKeiyu4Child.Required = false;
        _ninzuKeiyu4Junior.Required = false;
        _ninzuKeiyu4S.Required = false;
        _ninzuKeiyu5Adult.Required = false;
        _ninzuKeiyu5Child.Required = false;
        _ninzuKeiyu5Junior.Required = false;
        _ninzuKeiyu5S.Required = false;
        _oneSankaFlg.Required = false;
        _optionFlg.Required = false;
        _pickupKbn1.Required = false;
        _pickupKbn2.Required = false;
        _pickupKbn3.Required = false;
        _pickupKbn4.Required = false;
        _pickupKbn5.Required = false;
        _returnDay.Required = false;
        _roomZansuOneRoom.Required = false;
        _roomZansuSokei.Required = false;
        _roomZansuThreeRoom.Required = false;
        _roomZansuTwoRoom.Required = false;
        _roomZansuFourRoom.Required = false;
        _roomZansuFiveRoom.Required = false;
        _minSaikouNinzu.Required = false;
        _saikouDay.Required = false;
        _saikouKakuteiKbn.Required = false;
        _seasonCd.Required = false;
        _senyoCrsKbn.Required = false;
        _shanaiContactForMessage.Required = false;
        _shukujitsuFlg.Required = false;
        _sinsetuYm.Required = false;
        _stayDay.Required = false;
        _stayStay.Required = false;
        _subSeatOkKbn.Required = false;
        _syoyoTime.Required = false;
        _syugoPlaceCdCarrier.Required = false;
        _syugoTime1.Required = false;
        _syugoTime2.Required = false;
        _syugoTime3.Required = false;
        _syugoTime4.Required = false;
        _syugoTime5.Required = false;
        _syugoTimeCarrier.Required = false;
        _syuptJiCarrierKbn.Required = false;
        _syuptPlaceCarrier.Required = false;
        _syuptPlaceCdCarrier.Required = false;
        _syuptTime1.Required = false;
        _syuptTime2.Required = false;
        _syuptTime3.Required = false;
        _syuptTime4.Required = false;
        _syuptTime5.Required = false;
        _syuptTimeCarrier.Required = false;
        _teiinseiFlg.Required = false;
        _teikiCrsKbn.Required = false;
        _teikiKikakuKbn.Required = false;
        _tejimaiContactKbn.Required = false;
        _tejimaiDay.Required = false;
        _tejimaiKbn.Required = false;
        _tenjyoinCd.Required = false;
        _tieTyakuyo.Required = false;
        _tokuteiChargeSet.Required = false;
        _tokuteiDayFlg.Required = false;
        _ttyakPlaceCarrier.Required = false;
        _ttyakPlaceCdCarrier.Required = false;
        _ttyakTimeCarrier.Required = false;
        _tyuijikou.Required = false;
        _tyuijikouKbn.Required = false;
        _uketukeGenteiNinzu.Required = false;
        _uketukeStartBi.Required = false;
        _uketukeStartDay.Required = false;
        _uketukeStartKagetumae.Required = false;
        _underKinsi18old.Required = false;
        _unkyuContactDay.Required = false;
        _unkyuContactDoneFlg.Required = false;
        _unkyuContactTime.Required = false;
        _unkyuKbn.Required = false;
        _tojituKokuchiFlg.Required = false;
        _yusenYoyakuFlg.Required = false;
        _pickupKbnFlg.Required = false;
        _konjyoOkFlg.Required = false;
        _tonariFlg.Required = false;
        _aheadZasekiFlg.Required = false;
        _yoyakuMediaInputFlg.Required = false;
        _kokusekiFlg.Required = false;
        _sexBetuFlg.Required = false;
        _ageFlg.Required = false;
        _birthdayFlg.Required = false;
        _telFlg.Required = false;
        _addressFlg.Required = false;
        _usingFlg.Required = false;
        _uwagiTyakuyo.Required = false;
        _year.Required = false;
        _yobiCd.Required = false;
        _yoyakuAlreadyRoomNum.Required = false;
        _yoyakuKanouNum.Required = false;
        _yoyakuNgFlg.Required = false;
        _yoyakuNumSubSeat.Required = false;
        _yoyakuNumTeiseki.Required = false;
        _yoyakuStopFlg.Required = false;
        _zouhatsumotogousya.Required = false;
        _zouhatsuday.Required = false;
        _zouhatsuentrypersoncd.Required = false;
        _zasekiHihyojiFlg.Required = false;
        _zasekiReserveKbn.Required = false;
        _wtKakuhoSeatNum.Required = false;
        _systemEntryPgmid.Required = true;
        _systemEntryPersonCd.Required = true;
        _systemEntryDay.Required = true;
        _systemUpdatePgmid.Required = true;
        _systemUpdatePersonCd.Required = true;
        _systemUpdateDay.Required = true;
        _crsCd.DBType = OracleDbType.Char;
        _syuptDay.DBType = OracleDbType.Decimal;
        _gousya.DBType = OracleDbType.Decimal;
        _accessCd.DBType = OracleDbType.Char;
        _aibeyaUseFlg.DBType = OracleDbType.Char;
        _aibeyaYoyakuNinzuJyosei.DBType = OracleDbType.Decimal;
        _aibeyaYoyakuNinzuMale.DBType = OracleDbType.Decimal;
        _binName.DBType = OracleDbType.Varchar2;
        _blockKakuhoNum.DBType = OracleDbType.Decimal;
        _busCompanyCd.DBType = OracleDbType.Char;
        _busReserveCd.DBType = OracleDbType.Char;
        _cancelNgFlg.DBType = OracleDbType.Char;
        _cancelRyouKbn.DBType = OracleDbType.Char;
        _cancelWaitNinzu.DBType = OracleDbType.Decimal;
        _capacityHo1kai.DBType = OracleDbType.Decimal;
        _capacityRegular.DBType = OracleDbType.Decimal;
        _carrierCd.DBType = OracleDbType.Char;
        _carrierEdaban.DBType = OracleDbType.Char;
        _carNo.DBType = OracleDbType.Decimal;
        _carTypeCd.DBType = OracleDbType.Char;
        _carTypeCdYotei.DBType = OracleDbType.Char;
        _busCountFlg.DBType = OracleDbType.Char;
        _categoryCd1.DBType = OracleDbType.Char;
        _categoryCd2.DBType = OracleDbType.Char;
        _categoryCd3.DBType = OracleDbType.Char;
        _categoryCd4.DBType = OracleDbType.Char;
        _costSetKbn.DBType = OracleDbType.Char;
        _crsBlockCapacity.DBType = OracleDbType.Decimal;
        _crsBlockOne1r.DBType = OracleDbType.Decimal;
        _crsBlockRoomNum.DBType = OracleDbType.Decimal;
        _crsBlockThree1r.DBType = OracleDbType.Decimal;
        _crsBlockTwo1r.DBType = OracleDbType.Decimal;
        _crsBlockFour1r.DBType = OracleDbType.Decimal;
        _crsBlockFive1r.DBType = OracleDbType.Decimal;
        _crsKbn1.DBType = OracleDbType.Char;
        _crsKbn2.DBType = OracleDbType.Char;
        _crsKind.DBType = OracleDbType.Char;
        _managementSec.DBType = OracleDbType.Varchar2;
        _guideGengo.DBType = OracleDbType.Char;
        _crsName.DBType = OracleDbType.Varchar2;
        _crsNameKana.DBType = OracleDbType.Varchar2;
        _crsNameRk.DBType = OracleDbType.Varchar2;
        _crsNameKanaRk.DBType = OracleDbType.Varchar2;
        _deleteDay.DBType = OracleDbType.Decimal;
        _eiBlockHo.DBType = OracleDbType.Decimal;
        _eiBlockRegular.DBType = OracleDbType.Decimal;
        _endPlaceCd.DBType = OracleDbType.Char;
        _endTime.DBType = OracleDbType.Decimal;
        _haisyaKeiyuCd1.DBType = OracleDbType.Char;
        _haisyaKeiyuCd2.DBType = OracleDbType.Char;
        _haisyaKeiyuCd3.DBType = OracleDbType.Char;
        _haisyaKeiyuCd4.DBType = OracleDbType.Char;
        _haisyaKeiyuCd5.DBType = OracleDbType.Char;
        _homenCd.DBType = OracleDbType.Char;
        _houjinGaikyakuKbn.DBType = OracleDbType.Char;
        _hurikomiNgFlg.DBType = OracleDbType.Char;
        _itineraryTableCreateFlg.DBType = OracleDbType.Char;
        _jyoseiSenyoSeatFlg.DBType = OracleDbType.Char;
        _jyosyaCapacity.DBType = OracleDbType.Decimal;
        _kaiteiDay.DBType = OracleDbType.Decimal;
        _kusekiKakuhoNum.DBType = OracleDbType.Decimal;
        _kusekiNumSubSeat.DBType = OracleDbType.Decimal;
        _kusekiNumTeiseki.DBType = OracleDbType.Decimal;
        _kyosaiUnkouKbn.DBType = OracleDbType.Char;
        _maeuriKigen.DBType = OracleDbType.Char;
        _maruZouManagementKbn.DBType = OracleDbType.Char;
        _mealCountMorning.DBType = OracleDbType.Decimal;
        _mealCountNight.DBType = OracleDbType.Decimal;
        _mealCountNoon.DBType = OracleDbType.Decimal;
        _mediaCheckFlg.DBType = OracleDbType.Char;
        _meiboInputFlg.DBType = OracleDbType.Char;
        _ninzuInputFlgKeiyu1.DBType = OracleDbType.Char;
        _ninzuInputFlgKeiyu2.DBType = OracleDbType.Char;
        _ninzuInputFlgKeiyu3.DBType = OracleDbType.Char;
        _ninzuInputFlgKeiyu4.DBType = OracleDbType.Char;
        _ninzuInputFlgKeiyu5.DBType = OracleDbType.Char;
        _ninzuKeiyu1Adult.DBType = OracleDbType.Decimal;
        _ninzuKeiyu1Child.DBType = OracleDbType.Decimal;
        _ninzuKeiyu1Junior.DBType = OracleDbType.Decimal;
        _ninzuKeiyu1S.DBType = OracleDbType.Decimal;
        _ninzuKeiyu2Adult.DBType = OracleDbType.Decimal;
        _ninzuKeiyu2Child.DBType = OracleDbType.Decimal;
        _ninzuKeiyu2Junior.DBType = OracleDbType.Decimal;
        _ninzuKeiyu2S.DBType = OracleDbType.Decimal;
        _ninzuKeiyu3Adult.DBType = OracleDbType.Decimal;
        _ninzuKeiyu3Child.DBType = OracleDbType.Decimal;
        _ninzuKeiyu3Junior.DBType = OracleDbType.Decimal;
        _ninzuKeiyu3S.DBType = OracleDbType.Decimal;
        _ninzuKeiyu4Adult.DBType = OracleDbType.Decimal;
        _ninzuKeiyu4Child.DBType = OracleDbType.Decimal;
        _ninzuKeiyu4Junior.DBType = OracleDbType.Decimal;
        _ninzuKeiyu4S.DBType = OracleDbType.Decimal;
        _ninzuKeiyu5Adult.DBType = OracleDbType.Decimal;
        _ninzuKeiyu5Child.DBType = OracleDbType.Decimal;
        _ninzuKeiyu5Junior.DBType = OracleDbType.Decimal;
        _ninzuKeiyu5S.DBType = OracleDbType.Decimal;
        _oneSankaFlg.DBType = OracleDbType.Char;
        _optionFlg.DBType = OracleDbType.Char;
        _pickupKbn1.DBType = OracleDbType.Char;
        _pickupKbn2.DBType = OracleDbType.Char;
        _pickupKbn3.DBType = OracleDbType.Char;
        _pickupKbn4.DBType = OracleDbType.Char;
        _pickupKbn5.DBType = OracleDbType.Char;
        _returnDay.DBType = OracleDbType.Decimal;
        _roomZansuOneRoom.DBType = OracleDbType.Decimal;
        _roomZansuSokei.DBType = OracleDbType.Decimal;
        _roomZansuThreeRoom.DBType = OracleDbType.Decimal;
        _roomZansuTwoRoom.DBType = OracleDbType.Decimal;
        _roomZansuFourRoom.DBType = OracleDbType.Decimal;
        _roomZansuFiveRoom.DBType = OracleDbType.Decimal;
        _minSaikouNinzu.DBType = OracleDbType.Decimal;
        _saikouDay.DBType = OracleDbType.Decimal;
        _saikouKakuteiKbn.DBType = OracleDbType.Char;
        _seasonCd.DBType = OracleDbType.Char;
        _senyoCrsKbn.DBType = OracleDbType.Char;
        _shanaiContactForMessage.DBType = OracleDbType.Varchar2;
        _shukujitsuFlg.DBType = OracleDbType.Char;
        _sinsetuYm.DBType = OracleDbType.Decimal;
        _stayDay.DBType = OracleDbType.Decimal;
        _stayStay.DBType = OracleDbType.Decimal;
        _subSeatOkKbn.DBType = OracleDbType.Char;
        _syoyoTime.DBType = OracleDbType.Decimal;
        _syugoPlaceCdCarrier.DBType = OracleDbType.Char;
        _syugoTime1.DBType = OracleDbType.Decimal;
        _syugoTime2.DBType = OracleDbType.Decimal;
        _syugoTime3.DBType = OracleDbType.Decimal;
        _syugoTime4.DBType = OracleDbType.Decimal;
        _syugoTime5.DBType = OracleDbType.Decimal;
        _syugoTimeCarrier.DBType = OracleDbType.Decimal;
        _syuptJiCarrierKbn.DBType = OracleDbType.Char;
        _syuptPlaceCarrier.DBType = OracleDbType.Varchar2;
        _syuptPlaceCdCarrier.DBType = OracleDbType.Char;
        _syuptTime1.DBType = OracleDbType.Decimal;
        _syuptTime2.DBType = OracleDbType.Decimal;
        _syuptTime3.DBType = OracleDbType.Decimal;
        _syuptTime4.DBType = OracleDbType.Decimal;
        _syuptTime5.DBType = OracleDbType.Decimal;
        _syuptTimeCarrier.DBType = OracleDbType.Decimal;
        _teiinseiFlg.DBType = OracleDbType.Char;
        _teikiCrsKbn.DBType = OracleDbType.Char;
        _teikiKikakuKbn.DBType = OracleDbType.Char;
        _tejimaiContactKbn.DBType = OracleDbType.Char;
        _tejimaiDay.DBType = OracleDbType.Decimal;
        _tejimaiKbn.DBType = OracleDbType.Char;
        _tenjyoinCd.DBType = OracleDbType.Char;
        _tieTyakuyo.DBType = OracleDbType.Char;
        _tokuteiChargeSet.DBType = OracleDbType.Char;
        _tokuteiDayFlg.DBType = OracleDbType.Char;
        _ttyakPlaceCarrier.DBType = OracleDbType.Varchar2;
        _ttyakPlaceCdCarrier.DBType = OracleDbType.Char;
        _ttyakTimeCarrier.DBType = OracleDbType.Decimal;
        _tyuijikou.DBType = OracleDbType.Varchar2;
        _tyuijikouKbn.DBType = OracleDbType.Char;
        _uketukeGenteiNinzu.DBType = OracleDbType.Decimal;
        _uketukeStartBi.DBType = OracleDbType.Decimal;
        _uketukeStartDay.DBType = OracleDbType.Decimal;
        _uketukeStartKagetumae.DBType = OracleDbType.Char;
        _underKinsi18old.DBType = OracleDbType.Char;
        _unkyuContactDay.DBType = OracleDbType.Decimal;
        _unkyuContactDoneFlg.DBType = OracleDbType.Char;
        _unkyuContactTime.DBType = OracleDbType.Decimal;
        _unkyuKbn.DBType = OracleDbType.Char;
        _tojituKokuchiFlg.DBType = OracleDbType.Decimal;
        _yusenYoyakuFlg.DBType = OracleDbType.Decimal;
        _pickupKbnFlg.DBType = OracleDbType.Decimal;
        _konjyoOkFlg.DBType = OracleDbType.Decimal;
        _tonariFlg.DBType = OracleDbType.Decimal;
        _aheadZasekiFlg.DBType = OracleDbType.Decimal;
        _yoyakuMediaInputFlg.DBType = OracleDbType.Decimal;
        _kokusekiFlg.DBType = OracleDbType.Decimal;
        _sexBetuFlg.DBType = OracleDbType.Decimal;
        _ageFlg.DBType = OracleDbType.Decimal;
        _birthdayFlg.DBType = OracleDbType.Decimal;
        _telFlg.DBType = OracleDbType.Decimal;
        _addressFlg.DBType = OracleDbType.Decimal;
        _usingFlg.DBType = OracleDbType.Char;
        _uwagiTyakuyo.DBType = OracleDbType.Char;
        _year.DBType = OracleDbType.Decimal;
        _yobiCd.DBType = OracleDbType.Char;
        _yoyakuAlreadyRoomNum.DBType = OracleDbType.Decimal;
        _yoyakuKanouNum.DBType = OracleDbType.Decimal;
        _yoyakuNgFlg.DBType = OracleDbType.Char;
        _yoyakuNumSubSeat.DBType = OracleDbType.Decimal;
        _yoyakuNumTeiseki.DBType = OracleDbType.Decimal;
        _yoyakuStopFlg.DBType = OracleDbType.Char;
        _zouhatsumotogousya.DBType = OracleDbType.Decimal;
        _zouhatsuday.DBType = OracleDbType.Decimal;
        _zouhatsuentrypersoncd.DBType = OracleDbType.Char;
        _zasekiHihyojiFlg.DBType = OracleDbType.Char;
        _zasekiReserveKbn.DBType = OracleDbType.Char;
        _wtKakuhoSeatNum.DBType = OracleDbType.Decimal;
        _systemEntryPgmid.DBType = OracleDbType.Char;
        _systemEntryPersonCd.DBType = OracleDbType.Varchar2;
        _systemEntryDay.DBType = OracleDbType.Date;
        _systemUpdatePgmid.DBType = OracleDbType.Char;
        _systemUpdatePersonCd.DBType = OracleDbType.Varchar2;
        _systemUpdateDay.DBType = OracleDbType.Date;
        _crsCd.IntegerBu = 6;
        _syuptDay.IntegerBu = 8;
        _gousya.IntegerBu = 3;
        _accessCd.IntegerBu = 1;
        _aibeyaUseFlg.IntegerBu = 1;
        _aibeyaYoyakuNinzuJyosei.IntegerBu = 3;
        _aibeyaYoyakuNinzuMale.IntegerBu = 3;
        _binName.IntegerBu = 10;
        _blockKakuhoNum.IntegerBu = 3;
        _busCompanyCd.IntegerBu = 6;
        _busReserveCd.IntegerBu = 6;
        _cancelNgFlg.IntegerBu = 1;
        _cancelRyouKbn.IntegerBu = 1;
        _cancelWaitNinzu.IntegerBu = 2;
        _capacityHo1kai.IntegerBu = 3;
        _capacityRegular.IntegerBu = 3;
        _carrierCd.IntegerBu = 4;
        _carrierEdaban.IntegerBu = 2;
        _carNo.IntegerBu = 3;
        _carTypeCd.IntegerBu = 2;
        _carTypeCdYotei.IntegerBu = 2;
        _busCountFlg.IntegerBu = 1;
        _categoryCd1.IntegerBu = 1;
        _categoryCd2.IntegerBu = 1;
        _categoryCd3.IntegerBu = 1;
        _categoryCd4.IntegerBu = 1;
        _costSetKbn.IntegerBu = 1;
        _crsBlockCapacity.IntegerBu = 5;
        _crsBlockOne1r.IntegerBu = 3;
        _crsBlockRoomNum.IntegerBu = 3;
        _crsBlockThree1r.IntegerBu = 3;
        _crsBlockTwo1r.IntegerBu = 3;
        _crsBlockFour1r.IntegerBu = 3;
        _crsBlockFive1r.IntegerBu = 3;
        _crsKbn1.IntegerBu = 1;
        _crsKbn2.IntegerBu = 1;
        _crsKind.IntegerBu = 1;
        _managementSec.IntegerBu = 15;
        _guideGengo.IntegerBu = 2;
        _crsName.IntegerBu = 256;
        _crsNameKana.IntegerBu = 128;
        _crsNameRk.IntegerBu = 20;
        _crsNameKanaRk.IntegerBu = 20;
        _deleteDay.IntegerBu = 8;
        _eiBlockHo.IntegerBu = 3;
        _eiBlockRegular.IntegerBu = 3;
        _endPlaceCd.IntegerBu = 3;
        _endTime.IntegerBu = 4;
        _haisyaKeiyuCd1.IntegerBu = 3;
        _haisyaKeiyuCd2.IntegerBu = 3;
        _haisyaKeiyuCd3.IntegerBu = 3;
        _haisyaKeiyuCd4.IntegerBu = 3;
        _haisyaKeiyuCd5.IntegerBu = 3;
        _homenCd.IntegerBu = 2;
        _houjinGaikyakuKbn.IntegerBu = 1;
        _hurikomiNgFlg.IntegerBu = 1;
        _itineraryTableCreateFlg.IntegerBu = 1;
        _jyoseiSenyoSeatFlg.IntegerBu = 1;
        _jyosyaCapacity.IntegerBu = 3;
        _kaiteiDay.IntegerBu = 8;
        _kusekiKakuhoNum.IntegerBu = 3;
        _kusekiNumSubSeat.IntegerBu = 3;
        _kusekiNumTeiseki.IntegerBu = 3;
        _kyosaiUnkouKbn.IntegerBu = 1;
        _maeuriKigen.IntegerBu = 2;
        _maruZouManagementKbn.IntegerBu = 1;
        _mealCountMorning.IntegerBu = 2;
        _mealCountNight.IntegerBu = 2;
        _mealCountNoon.IntegerBu = 2;
        _mediaCheckFlg.IntegerBu = 1;
        _meiboInputFlg.IntegerBu = 1;
        _ninzuInputFlgKeiyu1.IntegerBu = 1;
        _ninzuInputFlgKeiyu2.IntegerBu = 1;
        _ninzuInputFlgKeiyu3.IntegerBu = 1;
        _ninzuInputFlgKeiyu4.IntegerBu = 1;
        _ninzuInputFlgKeiyu5.IntegerBu = 1;
        _ninzuKeiyu1Adult.IntegerBu = 3;
        _ninzuKeiyu1Child.IntegerBu = 3;
        _ninzuKeiyu1Junior.IntegerBu = 3;
        _ninzuKeiyu1S.IntegerBu = 3;
        _ninzuKeiyu2Adult.IntegerBu = 3;
        _ninzuKeiyu2Child.IntegerBu = 3;
        _ninzuKeiyu2Junior.IntegerBu = 3;
        _ninzuKeiyu2S.IntegerBu = 3;
        _ninzuKeiyu3Adult.IntegerBu = 3;
        _ninzuKeiyu3Child.IntegerBu = 3;
        _ninzuKeiyu3Junior.IntegerBu = 3;
        _ninzuKeiyu3S.IntegerBu = 3;
        _ninzuKeiyu4Adult.IntegerBu = 3;
        _ninzuKeiyu4Child.IntegerBu = 3;
        _ninzuKeiyu4Junior.IntegerBu = 3;
        _ninzuKeiyu4S.IntegerBu = 3;
        _ninzuKeiyu5Adult.IntegerBu = 3;
        _ninzuKeiyu5Child.IntegerBu = 3;
        _ninzuKeiyu5Junior.IntegerBu = 3;
        _ninzuKeiyu5S.IntegerBu = 3;
        _oneSankaFlg.IntegerBu = 1;
        _optionFlg.IntegerBu = 1;
        _pickupKbn1.IntegerBu = 1;
        _pickupKbn2.IntegerBu = 1;
        _pickupKbn3.IntegerBu = 1;
        _pickupKbn4.IntegerBu = 1;
        _pickupKbn5.IntegerBu = 1;
        _returnDay.IntegerBu = 8;
        _roomZansuOneRoom.IntegerBu = 3;
        _roomZansuSokei.IntegerBu = 3;
        _roomZansuThreeRoom.IntegerBu = 3;
        _roomZansuTwoRoom.IntegerBu = 3;
        _roomZansuFourRoom.IntegerBu = 3;
        _roomZansuFiveRoom.IntegerBu = 3;
        _minSaikouNinzu.IntegerBu = 2;
        _saikouDay.IntegerBu = 8;
        _saikouKakuteiKbn.IntegerBu = 1;
        _seasonCd.IntegerBu = 1;
        _senyoCrsKbn.IntegerBu = 1;
        _shanaiContactForMessage.IntegerBu = 32;
        _shukujitsuFlg.IntegerBu = 1;
        _sinsetuYm.IntegerBu = 6;
        _stayDay.IntegerBu = 2;
        _stayStay.IntegerBu = 2;
        _subSeatOkKbn.IntegerBu = 1;
        _syoyoTime.IntegerBu = 4;
        _syugoPlaceCdCarrier.IntegerBu = 3;
        _syugoTime1.IntegerBu = 4;
        _syugoTime2.IntegerBu = 4;
        _syugoTime3.IntegerBu = 4;
        _syugoTime4.IntegerBu = 4;
        _syugoTime5.IntegerBu = 4;
        _syugoTimeCarrier.IntegerBu = 4;
        _syuptJiCarrierKbn.IntegerBu = 1;
        _syuptPlaceCarrier.IntegerBu = 12;
        _syuptPlaceCdCarrier.IntegerBu = 3;
        _syuptTime1.IntegerBu = 4;
        _syuptTime2.IntegerBu = 4;
        _syuptTime3.IntegerBu = 4;
        _syuptTime4.IntegerBu = 4;
        _syuptTime5.IntegerBu = 4;
        _syuptTimeCarrier.IntegerBu = 4;
        _teiinseiFlg.IntegerBu = 1;
        _teikiCrsKbn.IntegerBu = 1;
        _teikiKikakuKbn.IntegerBu = 1;
        _tejimaiContactKbn.IntegerBu = 1;
        _tejimaiDay.IntegerBu = 8;
        _tejimaiKbn.IntegerBu = 1;
        _tenjyoinCd.IntegerBu = 5;
        _tieTyakuyo.IntegerBu = 1;
        _tokuteiChargeSet.IntegerBu = 1;
        _tokuteiDayFlg.IntegerBu = 1;
        _ttyakPlaceCarrier.IntegerBu = 12;
        _ttyakPlaceCdCarrier.IntegerBu = 3;
        _ttyakTimeCarrier.IntegerBu = 4;
        _tyuijikou.IntegerBu = 42;
        _tyuijikouKbn.IntegerBu = 1;
        _uketukeGenteiNinzu.IntegerBu = 1;
        _uketukeStartBi.IntegerBu = 2;
        _uketukeStartDay.IntegerBu = 8;
        _uketukeStartKagetumae.IntegerBu = 2;
        _underKinsi18old.IntegerBu = 1;
        _unkyuContactDay.IntegerBu = 8;
        _unkyuContactDoneFlg.IntegerBu = 1;
        _unkyuContactTime.IntegerBu = 6;
        _unkyuKbn.IntegerBu = 1;
        _tojituKokuchiFlg.IntegerBu = 1;
        _yusenYoyakuFlg.IntegerBu = 1;
        _pickupKbnFlg.IntegerBu = 1;
        _konjyoOkFlg.IntegerBu = 1;
        _tonariFlg.IntegerBu = 1;
        _aheadZasekiFlg.IntegerBu = 1;
        _yoyakuMediaInputFlg.IntegerBu = 1;
        _kokusekiFlg.IntegerBu = 1;
        _sexBetuFlg.IntegerBu = 1;
        _ageFlg.IntegerBu = 1;
        _birthdayFlg.IntegerBu = 1;
        _telFlg.IntegerBu = 1;
        _addressFlg.IntegerBu = 1;
        _usingFlg.IntegerBu = 1;
        _uwagiTyakuyo.IntegerBu = 1;
        _year.IntegerBu = 4;
        _yobiCd.IntegerBu = 1;
        _yoyakuAlreadyRoomNum.IntegerBu = 3;
        _yoyakuKanouNum.IntegerBu = 3;
        _yoyakuNgFlg.IntegerBu = 1;
        _yoyakuNumSubSeat.IntegerBu = 3;
        _yoyakuNumTeiseki.IntegerBu = 3;
        _yoyakuStopFlg.IntegerBu = 1;
        _zouhatsumotogousya.IntegerBu = 3;
        _zouhatsuday.IntegerBu = 8;
        _zouhatsuentrypersoncd.IntegerBu = 20;
        _zasekiHihyojiFlg.IntegerBu = 1;
        _zasekiReserveKbn.IntegerBu = 1;
        _wtKakuhoSeatNum.IntegerBu = 3;
        _systemEntryPgmid.IntegerBu = 8;
        _systemEntryPersonCd.IntegerBu = 20;
        _systemEntryDay.IntegerBu = 0;
        _systemUpdatePgmid.IntegerBu = 8;
        _systemUpdatePersonCd.IntegerBu = 20;
        _systemUpdateDay.IntegerBu = 0;
        _crsCd.DecimalBu = 0;
        _syuptDay.DecimalBu = 0;
        _gousya.DecimalBu = 0;
        _accessCd.DecimalBu = 0;
        _aibeyaUseFlg.DecimalBu = 0;
        _aibeyaYoyakuNinzuJyosei.DecimalBu = 0;
        _aibeyaYoyakuNinzuMale.DecimalBu = 0;
        _binName.DecimalBu = 0;
        _blockKakuhoNum.DecimalBu = 0;
        _busCompanyCd.DecimalBu = 0;
        _busReserveCd.DecimalBu = 0;
        _cancelNgFlg.DecimalBu = 0;
        _cancelRyouKbn.DecimalBu = 0;
        _cancelWaitNinzu.DecimalBu = 0;
        _capacityHo1kai.DecimalBu = 0;
        _capacityRegular.DecimalBu = 0;
        _carrierCd.DecimalBu = 0;
        _carrierEdaban.DecimalBu = 0;
        _carNo.DecimalBu = 0;
        _carTypeCd.DecimalBu = 0;
        _carTypeCdYotei.DecimalBu = 0;
        _busCountFlg.DecimalBu = 0;
        _categoryCd1.DecimalBu = 0;
        _categoryCd2.DecimalBu = 0;
        _categoryCd3.DecimalBu = 0;
        _categoryCd4.DecimalBu = 0;
        _costSetKbn.DecimalBu = 0;
        _crsBlockCapacity.DecimalBu = 0;
        _crsBlockOne1r.DecimalBu = 0;
        _crsBlockRoomNum.DecimalBu = 0;
        _crsBlockThree1r.DecimalBu = 0;
        _crsBlockTwo1r.DecimalBu = 0;
        _crsBlockFour1r.DecimalBu = 0;
        _crsBlockFive1r.DecimalBu = 0;
        _crsKbn1.DecimalBu = 0;
        _crsKbn2.DecimalBu = 0;
        _crsKind.DecimalBu = 0;
        _managementSec.DecimalBu = 0;
        _guideGengo.DecimalBu = 0;
        _crsName.DecimalBu = 0;
        _crsNameKana.DecimalBu = 0;
        _crsNameRk.DecimalBu = 0;
        _crsNameKanaRk.DecimalBu = 0;
        _deleteDay.DecimalBu = 0;
        _eiBlockHo.DecimalBu = 0;
        _eiBlockRegular.DecimalBu = 0;
        _endPlaceCd.DecimalBu = 0;
        _endTime.DecimalBu = 0;
        _haisyaKeiyuCd1.DecimalBu = 0;
        _haisyaKeiyuCd2.DecimalBu = 0;
        _haisyaKeiyuCd3.DecimalBu = 0;
        _haisyaKeiyuCd4.DecimalBu = 0;
        _haisyaKeiyuCd5.DecimalBu = 0;
        _homenCd.DecimalBu = 0;
        _houjinGaikyakuKbn.DecimalBu = 0;
        _hurikomiNgFlg.DecimalBu = 0;
        _itineraryTableCreateFlg.DecimalBu = 0;
        _jyoseiSenyoSeatFlg.DecimalBu = 0;
        _jyosyaCapacity.DecimalBu = 0;
        _kaiteiDay.DecimalBu = 0;
        _kusekiKakuhoNum.DecimalBu = 0;
        _kusekiNumSubSeat.DecimalBu = 0;
        _kusekiNumTeiseki.DecimalBu = 0;
        _kyosaiUnkouKbn.DecimalBu = 0;
        _maeuriKigen.DecimalBu = 0;
        _maruZouManagementKbn.DecimalBu = 0;
        _mealCountMorning.DecimalBu = 0;
        _mealCountNight.DecimalBu = 0;
        _mealCountNoon.DecimalBu = 0;
        _mediaCheckFlg.DecimalBu = 0;
        _meiboInputFlg.DecimalBu = 0;
        _ninzuInputFlgKeiyu1.DecimalBu = 0;
        _ninzuInputFlgKeiyu2.DecimalBu = 0;
        _ninzuInputFlgKeiyu3.DecimalBu = 0;
        _ninzuInputFlgKeiyu4.DecimalBu = 0;
        _ninzuInputFlgKeiyu5.DecimalBu = 0;
        _ninzuKeiyu1Adult.DecimalBu = 0;
        _ninzuKeiyu1Child.DecimalBu = 0;
        _ninzuKeiyu1Junior.DecimalBu = 0;
        _ninzuKeiyu1S.DecimalBu = 0;
        _ninzuKeiyu2Adult.DecimalBu = 0;
        _ninzuKeiyu2Child.DecimalBu = 0;
        _ninzuKeiyu2Junior.DecimalBu = 0;
        _ninzuKeiyu2S.DecimalBu = 0;
        _ninzuKeiyu3Adult.DecimalBu = 0;
        _ninzuKeiyu3Child.DecimalBu = 0;
        _ninzuKeiyu3Junior.DecimalBu = 0;
        _ninzuKeiyu3S.DecimalBu = 0;
        _ninzuKeiyu4Adult.DecimalBu = 0;
        _ninzuKeiyu4Child.DecimalBu = 0;
        _ninzuKeiyu4Junior.DecimalBu = 0;
        _ninzuKeiyu4S.DecimalBu = 0;
        _ninzuKeiyu5Adult.DecimalBu = 0;
        _ninzuKeiyu5Child.DecimalBu = 0;
        _ninzuKeiyu5Junior.DecimalBu = 0;
        _ninzuKeiyu5S.DecimalBu = 0;
        _oneSankaFlg.DecimalBu = 0;
        _optionFlg.DecimalBu = 0;
        _pickupKbn1.DecimalBu = 0;
        _pickupKbn2.DecimalBu = 0;
        _pickupKbn3.DecimalBu = 0;
        _pickupKbn4.DecimalBu = 0;
        _pickupKbn5.DecimalBu = 0;
        _returnDay.DecimalBu = 0;
        _roomZansuOneRoom.DecimalBu = 0;
        _roomZansuSokei.DecimalBu = 0;
        _roomZansuThreeRoom.DecimalBu = 0;
        _roomZansuTwoRoom.DecimalBu = 0;
        _roomZansuFourRoom.DecimalBu = 0;
        _roomZansuFiveRoom.DecimalBu = 0;
        _minSaikouNinzu.DecimalBu = 0;
        _saikouDay.DecimalBu = 0;
        _saikouKakuteiKbn.DecimalBu = 0;
        _seasonCd.DecimalBu = 0;
        _senyoCrsKbn.DecimalBu = 0;
        _shanaiContactForMessage.DecimalBu = 0;
        _shukujitsuFlg.DecimalBu = 0;
        _sinsetuYm.DecimalBu = 0;
        _stayDay.DecimalBu = 0;
        _stayStay.DecimalBu = 0;
        _subSeatOkKbn.DecimalBu = 0;
        _syoyoTime.DecimalBu = 0;
        _syugoPlaceCdCarrier.DecimalBu = 0;
        _syugoTime1.DecimalBu = 0;
        _syugoTime2.DecimalBu = 0;
        _syugoTime3.DecimalBu = 0;
        _syugoTime4.DecimalBu = 0;
        _syugoTime5.DecimalBu = 0;
        _syugoTimeCarrier.DecimalBu = 0;
        _syuptJiCarrierKbn.DecimalBu = 0;
        _syuptPlaceCarrier.DecimalBu = 0;
        _syuptPlaceCdCarrier.DecimalBu = 0;
        _syuptTime1.DecimalBu = 0;
        _syuptTime2.DecimalBu = 0;
        _syuptTime3.DecimalBu = 0;
        _syuptTime4.DecimalBu = 0;
        _syuptTime5.DecimalBu = 0;
        _syuptTimeCarrier.DecimalBu = 0;
        _teiinseiFlg.DecimalBu = 0;
        _teikiCrsKbn.DecimalBu = 0;
        _teikiKikakuKbn.DecimalBu = 0;
        _tejimaiContactKbn.DecimalBu = 0;
        _tejimaiDay.DecimalBu = 0;
        _tejimaiKbn.DecimalBu = 0;
        _tenjyoinCd.DecimalBu = 0;
        _tieTyakuyo.DecimalBu = 0;
        _tokuteiChargeSet.DecimalBu = 0;
        _tokuteiDayFlg.DecimalBu = 0;
        _ttyakPlaceCarrier.DecimalBu = 0;
        _ttyakPlaceCdCarrier.DecimalBu = 0;
        _ttyakTimeCarrier.DecimalBu = 0;
        _tyuijikou.DecimalBu = 0;
        _tyuijikouKbn.DecimalBu = 0;
        _uketukeGenteiNinzu.DecimalBu = 0;
        _uketukeStartBi.DecimalBu = 0;
        _uketukeStartDay.DecimalBu = 0;
        _uketukeStartKagetumae.DecimalBu = 0;
        _underKinsi18old.DecimalBu = 0;
        _unkyuContactDay.DecimalBu = 0;
        _unkyuContactDoneFlg.DecimalBu = 0;
        _unkyuContactTime.DecimalBu = 0;
        _unkyuKbn.DecimalBu = 0;
        _tojituKokuchiFlg.DecimalBu = 0;
        _yusenYoyakuFlg.DecimalBu = 0;
        _pickupKbnFlg.DecimalBu = 0;
        _konjyoOkFlg.DecimalBu = 0;
        _tonariFlg.DecimalBu = 0;
        _aheadZasekiFlg.DecimalBu = 0;
        _yoyakuMediaInputFlg.DecimalBu = 0;
        _kokusekiFlg.DecimalBu = 0;
        _sexBetuFlg.DecimalBu = 0;
        _ageFlg.DecimalBu = 0;
        _birthdayFlg.DecimalBu = 0;
        _telFlg.DecimalBu = 0;
        _addressFlg.DecimalBu = 0;
        _usingFlg.DecimalBu = 0;
        _uwagiTyakuyo.DecimalBu = 0;
        _year.DecimalBu = 0;
        _yobiCd.DecimalBu = 0;
        _yoyakuAlreadyRoomNum.DecimalBu = 0;
        _yoyakuKanouNum.DecimalBu = 0;
        _yoyakuNgFlg.DecimalBu = 0;
        _yoyakuNumSubSeat.DecimalBu = 0;
        _yoyakuNumTeiseki.DecimalBu = 0;
        _yoyakuStopFlg.DecimalBu = 0;
        _zouhatsumotogousya.DecimalBu = 0;
        _zouhatsuday.DecimalBu = 0;
        _zouhatsuentrypersoncd.DecimalBu = 0;
        _zasekiHihyojiFlg.DecimalBu = 0;
        _zasekiReserveKbn.DecimalBu = 0;
        _wtKakuhoSeatNum.DecimalBu = 0;
        _systemEntryPgmid.DecimalBu = 0;
        _systemEntryPersonCd.DecimalBu = 0;
        _systemEntryDay.DecimalBu = 0;
        _systemUpdatePgmid.DecimalBu = 0;
        _systemUpdatePersonCd.DecimalBu = 0;
        _systemUpdateDay.DecimalBu = 0;
    }


    /// <summary>
/// crsCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsCd
    {
        get
        {
            return _crsCd;
        }

        set
        {
            _crsCd = value;
        }
    }


    /// <summary>
/// syuptDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptDay
    {
        get
        {
            return _syuptDay;
        }

        set
        {
            _syuptDay = value;
        }
    }


    /// <summary>
/// gousya
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType gousya
    {
        get
        {
            return _gousya;
        }

        set
        {
            _gousya = value;
        }
    }


    /// <summary>
/// accessCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType accessCd
    {
        get
        {
            return _accessCd;
        }

        set
        {
            _accessCd = value;
        }
    }


    /// <summary>
/// aibeyaUseFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType aibeyaUseFlg
    {
        get
        {
            return _aibeyaUseFlg;
        }

        set
        {
            _aibeyaUseFlg = value;
        }
    }


    /// <summary>
/// aibeyaYoyakuNinzuJyosei
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType aibeyaYoyakuNinzuJyosei
    {
        get
        {
            return _aibeyaYoyakuNinzuJyosei;
        }

        set
        {
            _aibeyaYoyakuNinzuJyosei = value;
        }
    }


    /// <summary>
/// aibeyaYoyakuNinzuMale
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType aibeyaYoyakuNinzuMale
    {
        get
        {
            return _aibeyaYoyakuNinzuMale;
        }

        set
        {
            _aibeyaYoyakuNinzuMale = value;
        }
    }


    /// <summary>
/// binName
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType binName
    {
        get
        {
            return _binName;
        }

        set
        {
            _binName = value;
        }
    }


    /// <summary>
/// blockKakuhoNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType blockKakuhoNum
    {
        get
        {
            return _blockKakuhoNum;
        }

        set
        {
            _blockKakuhoNum = value;
        }
    }


    /// <summary>
/// busCompanyCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType busCompanyCd
    {
        get
        {
            return _busCompanyCd;
        }

        set
        {
            _busCompanyCd = value;
        }
    }


    /// <summary>
/// busReserveCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType busReserveCd
    {
        get
        {
            return _busReserveCd;
        }

        set
        {
            _busReserveCd = value;
        }
    }


    /// <summary>
/// cancelNgFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType cancelNgFlg
    {
        get
        {
            return _cancelNgFlg;
        }

        set
        {
            _cancelNgFlg = value;
        }
    }


    /// <summary>
/// cancelRyouKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType cancelRyouKbn
    {
        get
        {
            return _cancelRyouKbn;
        }

        set
        {
            _cancelRyouKbn = value;
        }
    }


    /// <summary>
/// cancelWaitNinzu
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType cancelWaitNinzu
    {
        get
        {
            return _cancelWaitNinzu;
        }

        set
        {
            _cancelWaitNinzu = value;
        }
    }


    /// <summary>
/// capacityHo1kai
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType capacityHo1kai
    {
        get
        {
            return _capacityHo1kai;
        }

        set
        {
            _capacityHo1kai = value;
        }
    }


    /// <summary>
/// capacityRegular
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType capacityRegular
    {
        get
        {
            return _capacityRegular;
        }

        set
        {
            _capacityRegular = value;
        }
    }


    /// <summary>
/// carrierCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carrierCd
    {
        get
        {
            return _carrierCd;
        }

        set
        {
            _carrierCd = value;
        }
    }


    /// <summary>
/// carrierEdaban
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carrierEdaban
    {
        get
        {
            return _carrierEdaban;
        }

        set
        {
            _carrierEdaban = value;
        }
    }


    /// <summary>
/// carNo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType carNo
    {
        get
        {
            return _carNo;
        }

        set
        {
            _carNo = value;
        }
    }


    /// <summary>
/// carTypeCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carTypeCd
    {
        get
        {
            return _carTypeCd;
        }

        set
        {
            _carTypeCd = value;
        }
    }


    /// <summary>
/// carTypeCdYotei
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType carTypeCdYotei
    {
        get
        {
            return _carTypeCdYotei;
        }

        set
        {
            _carTypeCdYotei = value;
        }
    }


    /// <summary>
/// busCountFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType busCountFlg
    {
        get
        {
            return _busCountFlg;
        }

        set
        {
            _busCountFlg = value;
        }
    }


    /// <summary>
/// categoryCd1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType categoryCd1
    {
        get
        {
            return _categoryCd1;
        }

        set
        {
            _categoryCd1 = value;
        }
    }


    /// <summary>
/// categoryCd2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType categoryCd2
    {
        get
        {
            return _categoryCd2;
        }

        set
        {
            _categoryCd2 = value;
        }
    }


    /// <summary>
/// categoryCd3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType categoryCd3
    {
        get
        {
            return _categoryCd3;
        }

        set
        {
            _categoryCd3 = value;
        }
    }


    /// <summary>
/// categoryCd4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType categoryCd4
    {
        get
        {
            return _categoryCd4;
        }

        set
        {
            _categoryCd4 = value;
        }
    }


    /// <summary>
/// costSetKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType costSetKbn
    {
        get
        {
            return _costSetKbn;
        }

        set
        {
            _costSetKbn = value;
        }
    }


    /// <summary>
/// crsBlockCapacity
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockCapacity
    {
        get
        {
            return _crsBlockCapacity;
        }

        set
        {
            _crsBlockCapacity = value;
        }
    }


    /// <summary>
/// crsBlockOne1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockOne1r
    {
        get
        {
            return _crsBlockOne1r;
        }

        set
        {
            _crsBlockOne1r = value;
        }
    }


    /// <summary>
/// crsBlockRoomNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockRoomNum
    {
        get
        {
            return _crsBlockRoomNum;
        }

        set
        {
            _crsBlockRoomNum = value;
        }
    }


    /// <summary>
/// crsBlockThree1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockThree1r
    {
        get
        {
            return _crsBlockThree1r;
        }

        set
        {
            _crsBlockThree1r = value;
        }
    }


    /// <summary>
/// crsBlockTwo1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockTwo1r
    {
        get
        {
            return _crsBlockTwo1r;
        }

        set
        {
            _crsBlockTwo1r = value;
        }
    }


    /// <summary>
/// crsBlockFour1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockFour1r
    {
        get
        {
            return _crsBlockFour1r;
        }

        set
        {
            _crsBlockFour1r = value;
        }
    }


    /// <summary>
/// crsBlockFive1r
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType crsBlockFive1r
    {
        get
        {
            return _crsBlockFive1r;
        }

        set
        {
            _crsBlockFive1r = value;
        }
    }


    /// <summary>
/// crsKbn1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsKbn1
    {
        get
        {
            return _crsKbn1;
        }

        set
        {
            _crsKbn1 = value;
        }
    }


    /// <summary>
/// crsKbn2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsKbn2
    {
        get
        {
            return _crsKbn2;
        }

        set
        {
            _crsKbn2 = value;
        }
    }


    /// <summary>
/// crsKind
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsKind
    {
        get
        {
            return _crsKind;
        }

        set
        {
            _crsKind = value;
        }
    }


    /// <summary>
/// managementSec
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType managementSec
    {
        get
        {
            return _managementSec;
        }

        set
        {
            _managementSec = value;
        }
    }


    /// <summary>
/// guideGengo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType guideGengo
    {
        get
        {
            return _guideGengo;
        }

        set
        {
            _guideGengo = value;
        }
    }


    /// <summary>
/// crsName
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsName
    {
        get
        {
            return _crsName;
        }

        set
        {
            _crsName = value;
        }
    }


    /// <summary>
/// crsNameKana
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsNameKana
    {
        get
        {
            return _crsNameKana;
        }

        set
        {
            _crsNameKana = value;
        }
    }


    /// <summary>
/// crsNameRk
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsNameRk
    {
        get
        {
            return _crsNameRk;
        }

        set
        {
            _crsNameRk = value;
        }
    }


    /// <summary>
/// crsNameKanaRk
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType crsNameKanaRk
    {
        get
        {
            return _crsNameKanaRk;
        }

        set
        {
            _crsNameKanaRk = value;
        }
    }


    /// <summary>
/// deleteDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType deleteDay
    {
        get
        {
            return _deleteDay;
        }

        set
        {
            _deleteDay = value;
        }
    }


    /// <summary>
/// eiBlockHo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType eiBlockHo
    {
        get
        {
            return _eiBlockHo;
        }

        set
        {
            _eiBlockHo = value;
        }
    }


    /// <summary>
/// eiBlockRegular
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType eiBlockRegular
    {
        get
        {
            return _eiBlockRegular;
        }

        set
        {
            _eiBlockRegular = value;
        }
    }


    /// <summary>
/// endPlaceCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType endPlaceCd
    {
        get
        {
            return _endPlaceCd;
        }

        set
        {
            _endPlaceCd = value;
        }
    }


    /// <summary>
/// endTime
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType endTime
    {
        get
        {
            return _endTime;
        }

        set
        {
            _endTime = value;
        }
    }


    /// <summary>
/// haisyaKeiyuCd1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType haisyaKeiyuCd1
    {
        get
        {
            return _haisyaKeiyuCd1;
        }

        set
        {
            _haisyaKeiyuCd1 = value;
        }
    }


    /// <summary>
/// haisyaKeiyuCd2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType haisyaKeiyuCd2
    {
        get
        {
            return _haisyaKeiyuCd2;
        }

        set
        {
            _haisyaKeiyuCd2 = value;
        }
    }


    /// <summary>
/// haisyaKeiyuCd3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType haisyaKeiyuCd3
    {
        get
        {
            return _haisyaKeiyuCd3;
        }

        set
        {
            _haisyaKeiyuCd3 = value;
        }
    }


    /// <summary>
/// haisyaKeiyuCd4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType haisyaKeiyuCd4
    {
        get
        {
            return _haisyaKeiyuCd4;
        }

        set
        {
            _haisyaKeiyuCd4 = value;
        }
    }


    /// <summary>
/// haisyaKeiyuCd5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType haisyaKeiyuCd5
    {
        get
        {
            return _haisyaKeiyuCd5;
        }

        set
        {
            _haisyaKeiyuCd5 = value;
        }
    }


    /// <summary>
/// homenCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType homenCd
    {
        get
        {
            return _homenCd;
        }

        set
        {
            _homenCd = value;
        }
    }


    /// <summary>
/// houjinGaikyakuKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType houjinGaikyakuKbn
    {
        get
        {
            return _houjinGaikyakuKbn;
        }

        set
        {
            _houjinGaikyakuKbn = value;
        }
    }
}


internal partial class SurroundingClass
{

    /// <summary>
/// hurikomiNgFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType hurikomiNgFlg
    {
        get
        {
            return _hurikomiNgFlg;
        }

        set
        {
            _hurikomiNgFlg = value;
        }
    }


    /// <summary>
/// itineraryTableCreateFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType itineraryTableCreateFlg
    {
        get
        {
            return _itineraryTableCreateFlg;
        }

        set
        {
            _itineraryTableCreateFlg = value;
        }
    }


    /// <summary>
/// jyoseiSenyoSeatFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType jyoseiSenyoSeatFlg
    {
        get
        {
            return _jyoseiSenyoSeatFlg;
        }

        set
        {
            _jyoseiSenyoSeatFlg = value;
        }
    }


    /// <summary>
/// jyosyaCapacity
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType jyosyaCapacity
    {
        get
        {
            return _jyosyaCapacity;
        }

        set
        {
            _jyosyaCapacity = value;
        }
    }


    /// <summary>
/// kaiteiDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kaiteiDay
    {
        get
        {
            return _kaiteiDay;
        }

        set
        {
            _kaiteiDay = value;
        }
    }


    /// <summary>
/// kusekiKakuhoNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kusekiKakuhoNum
    {
        get
        {
            return _kusekiKakuhoNum;
        }

        set
        {
            _kusekiKakuhoNum = value;
        }
    }


    /// <summary>
/// kusekiNumSubSeat
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kusekiNumSubSeat
    {
        get
        {
            return _kusekiNumSubSeat;
        }

        set
        {
            _kusekiNumSubSeat = value;
        }
    }


    /// <summary>
/// kusekiNumTeiseki
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kusekiNumTeiseki
    {
        get
        {
            return _kusekiNumTeiseki;
        }

        set
        {
            _kusekiNumTeiseki = value;
        }
    }


    /// <summary>
/// kyosaiUnkouKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType kyosaiUnkouKbn
    {
        get
        {
            return _kyosaiUnkouKbn;
        }

        set
        {
            _kyosaiUnkouKbn = value;
        }
    }


    /// <summary>
/// maeuriKigen
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType maeuriKigen
    {
        get
        {
            return _maeuriKigen;
        }

        set
        {
            _maeuriKigen = value;
        }
    }


    /// <summary>
/// maruZouManagementKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType maruZouManagementKbn
    {
        get
        {
            return _maruZouManagementKbn;
        }

        set
        {
            _maruZouManagementKbn = value;
        }
    }


    /// <summary>
/// mealCountMorning
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType mealCountMorning
    {
        get
        {
            return _mealCountMorning;
        }

        set
        {
            _mealCountMorning = value;
        }
    }


    /// <summary>
/// mealCountNight
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType mealCountNight
    {
        get
        {
            return _mealCountNight;
        }

        set
        {
            _mealCountNight = value;
        }
    }


    /// <summary>
/// mealCountNoon
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType mealCountNoon
    {
        get
        {
            return _mealCountNoon;
        }

        set
        {
            _mealCountNoon = value;
        }
    }


    /// <summary>
/// mediaCheckFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType mediaCheckFlg
    {
        get
        {
            return _mediaCheckFlg;
        }

        set
        {
            _mediaCheckFlg = value;
        }
    }


    /// <summary>
/// meiboInputFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType meiboInputFlg
    {
        get
        {
            return _meiboInputFlg;
        }

        set
        {
            _meiboInputFlg = value;
        }
    }


    /// <summary>
/// ninzuInputFlgKeiyu1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ninzuInputFlgKeiyu1
    {
        get
        {
            return _ninzuInputFlgKeiyu1;
        }

        set
        {
            _ninzuInputFlgKeiyu1 = value;
        }
    }


    /// <summary>
/// ninzuInputFlgKeiyu2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ninzuInputFlgKeiyu2
    {
        get
        {
            return _ninzuInputFlgKeiyu2;
        }

        set
        {
            _ninzuInputFlgKeiyu2 = value;
        }
    }


    /// <summary>
/// ninzuInputFlgKeiyu3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ninzuInputFlgKeiyu3
    {
        get
        {
            return _ninzuInputFlgKeiyu3;
        }

        set
        {
            _ninzuInputFlgKeiyu3 = value;
        }
    }


    /// <summary>
/// ninzuInputFlgKeiyu4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ninzuInputFlgKeiyu4
    {
        get
        {
            return _ninzuInputFlgKeiyu4;
        }

        set
        {
            _ninzuInputFlgKeiyu4 = value;
        }
    }


    /// <summary>
/// ninzuInputFlgKeiyu5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ninzuInputFlgKeiyu5
    {
        get
        {
            return _ninzuInputFlgKeiyu5;
        }

        set
        {
            _ninzuInputFlgKeiyu5 = value;
        }
    }


    /// <summary>
/// ninzuKeiyu1Adult
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu1Adult
    {
        get
        {
            return _ninzuKeiyu1Adult;
        }

        set
        {
            _ninzuKeiyu1Adult = value;
        }
    }


    /// <summary>
/// ninzuKeiyu1Child
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu1Child
    {
        get
        {
            return _ninzuKeiyu1Child;
        }

        set
        {
            _ninzuKeiyu1Child = value;
        }
    }


    /// <summary>
/// ninzuKeiyu1Junior
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu1Junior
    {
        get
        {
            return _ninzuKeiyu1Junior;
        }

        set
        {
            _ninzuKeiyu1Junior = value;
        }
    }


    /// <summary>
/// ninzuKeiyu1S
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu1S
    {
        get
        {
            return _ninzuKeiyu1S;
        }

        set
        {
            _ninzuKeiyu1S = value;
        }
    }


    /// <summary>
/// ninzuKeiyu2Adult
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu2Adult
    {
        get
        {
            return _ninzuKeiyu2Adult;
        }

        set
        {
            _ninzuKeiyu2Adult = value;
        }
    }


    /// <summary>
/// ninzuKeiyu2Child
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu2Child
    {
        get
        {
            return _ninzuKeiyu2Child;
        }

        set
        {
            _ninzuKeiyu2Child = value;
        }
    }


    /// <summary>
/// ninzuKeiyu2Junior
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu2Junior
    {
        get
        {
            return _ninzuKeiyu2Junior;
        }

        set
        {
            _ninzuKeiyu2Junior = value;
        }
    }


    /// <summary>
/// ninzuKeiyu2S
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu2S
    {
        get
        {
            return _ninzuKeiyu2S;
        }

        set
        {
            _ninzuKeiyu2S = value;
        }
    }


    /// <summary>
/// ninzuKeiyu3Adult
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu3Adult
    {
        get
        {
            return _ninzuKeiyu3Adult;
        }

        set
        {
            _ninzuKeiyu3Adult = value;
        }
    }


    /// <summary>
/// ninzuKeiyu3Child
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu3Child
    {
        get
        {
            return _ninzuKeiyu3Child;
        }

        set
        {
            _ninzuKeiyu3Child = value;
        }
    }


    /// <summary>
/// ninzuKeiyu3Junior
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu3Junior
    {
        get
        {
            return _ninzuKeiyu3Junior;
        }

        set
        {
            _ninzuKeiyu3Junior = value;
        }
    }


    /// <summary>
/// ninzuKeiyu3S
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu3S
    {
        get
        {
            return _ninzuKeiyu3S;
        }

        set
        {
            _ninzuKeiyu3S = value;
        }
    }


    /// <summary>
/// ninzuKeiyu4Adult
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu4Adult
    {
        get
        {
            return _ninzuKeiyu4Adult;
        }

        set
        {
            _ninzuKeiyu4Adult = value;
        }
    }


    /// <summary>
/// ninzuKeiyu4Child
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu4Child
    {
        get
        {
            return _ninzuKeiyu4Child;
        }

        set
        {
            _ninzuKeiyu4Child = value;
        }
    }


    /// <summary>
/// ninzuKeiyu4Junior
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu4Junior
    {
        get
        {
            return _ninzuKeiyu4Junior;
        }

        set
        {
            _ninzuKeiyu4Junior = value;
        }
    }


    /// <summary>
/// ninzuKeiyu4S
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu4S
    {
        get
        {
            return _ninzuKeiyu4S;
        }

        set
        {
            _ninzuKeiyu4S = value;
        }
    }


    /// <summary>
/// ninzuKeiyu5Adult
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu5Adult
    {
        get
        {
            return _ninzuKeiyu5Adult;
        }

        set
        {
            _ninzuKeiyu5Adult = value;
        }
    }


    /// <summary>
/// ninzuKeiyu5Child
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu5Child
    {
        get
        {
            return _ninzuKeiyu5Child;
        }

        set
        {
            _ninzuKeiyu5Child = value;
        }
    }


    /// <summary>
/// ninzuKeiyu5Junior
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu5Junior
    {
        get
        {
            return _ninzuKeiyu5Junior;
        }

        set
        {
            _ninzuKeiyu5Junior = value;
        }
    }


    /// <summary>
/// ninzuKeiyu5S
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ninzuKeiyu5S
    {
        get
        {
            return _ninzuKeiyu5S;
        }

        set
        {
            _ninzuKeiyu5S = value;
        }
    }


    /// <summary>
/// oneSankaFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType oneSankaFlg
    {
        get
        {
            return _oneSankaFlg;
        }

        set
        {
            _oneSankaFlg = value;
        }
    }


    /// <summary>
/// optionFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType optionFlg
    {
        get
        {
            return _optionFlg;
        }

        set
        {
            _optionFlg = value;
        }
    }


    /// <summary>
/// pickupKbn1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType pickupKbn1
    {
        get
        {
            return _pickupKbn1;
        }

        set
        {
            _pickupKbn1 = value;
        }
    }


    /// <summary>
/// pickupKbn2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType pickupKbn2
    {
        get
        {
            return _pickupKbn2;
        }

        set
        {
            _pickupKbn2 = value;
        }
    }


    /// <summary>
/// pickupKbn3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType pickupKbn3
    {
        get
        {
            return _pickupKbn3;
        }

        set
        {
            _pickupKbn3 = value;
        }
    }


    /// <summary>
/// pickupKbn4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType pickupKbn4
    {
        get
        {
            return _pickupKbn4;
        }

        set
        {
            _pickupKbn4 = value;
        }
    }


    /// <summary>
/// pickupKbn5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType pickupKbn5
    {
        get
        {
            return _pickupKbn5;
        }

        set
        {
            _pickupKbn5 = value;
        }
    }


    /// <summary>
/// returnDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType returnDay
    {
        get
        {
            return _returnDay;
        }

        set
        {
            _returnDay = value;
        }
    }


    /// <summary>
/// roomZansuOneRoom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuOneRoom
    {
        get
        {
            return _roomZansuOneRoom;
        }

        set
        {
            _roomZansuOneRoom = value;
        }
    }


    /// <summary>
/// roomZansuSokei
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuSokei
    {
        get
        {
            return _roomZansuSokei;
        }

        set
        {
            _roomZansuSokei = value;
        }
    }


    /// <summary>
/// roomZansuThreeRoom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuThreeRoom
    {
        get
        {
            return _roomZansuThreeRoom;
        }

        set
        {
            _roomZansuThreeRoom = value;
        }
    }


    /// <summary>
/// roomZansuTwoRoom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuTwoRoom
    {
        get
        {
            return _roomZansuTwoRoom;
        }

        set
        {
            _roomZansuTwoRoom = value;
        }
    }


    /// <summary>
/// roomZansuFourRoom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuFourRoom
    {
        get
        {
            return _roomZansuFourRoom;
        }

        set
        {
            _roomZansuFourRoom = value;
        }
    }


    /// <summary>
/// roomZansuFiveRoom
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType roomZansuFiveRoom
    {
        get
        {
            return _roomZansuFiveRoom;
        }

        set
        {
            _roomZansuFiveRoom = value;
        }
    }


    /// <summary>
/// minSaikouNinzu
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType minSaikouNinzu
    {
        get
        {
            return _minSaikouNinzu;
        }

        set
        {
            _minSaikouNinzu = value;
        }
    }


    /// <summary>
/// saikouDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType saikouDay
    {
        get
        {
            return _saikouDay;
        }

        set
        {
            _saikouDay = value;
        }
    }


    /// <summary>
/// saikouKakuteiKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType saikouKakuteiKbn
    {
        get
        {
            return _saikouKakuteiKbn;
        }

        set
        {
            _saikouKakuteiKbn = value;
        }
    }


    /// <summary>
/// seasonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType seasonCd
    {
        get
        {
            return _seasonCd;
        }

        set
        {
            _seasonCd = value;
        }
    }


    /// <summary>
/// senyoCrsKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType senyoCrsKbn
    {
        get
        {
            return _senyoCrsKbn;
        }

        set
        {
            _senyoCrsKbn = value;
        }
    }


    /// <summary>
/// shanaiContactForMessage
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType shanaiContactForMessage
    {
        get
        {
            return _shanaiContactForMessage;
        }

        set
        {
            _shanaiContactForMessage = value;
        }
    }


    /// <summary>
/// shukujitsuFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType shukujitsuFlg
    {
        get
        {
            return _shukujitsuFlg;
        }

        set
        {
            _shukujitsuFlg = value;
        }
    }


    /// <summary>
/// sinsetuYm
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType sinsetuYm
    {
        get
        {
            return _sinsetuYm;
        }

        set
        {
            _sinsetuYm = value;
        }
    }


    /// <summary>
/// stayDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType stayDay
    {
        get
        {
            return _stayDay;
        }

        set
        {
            _stayDay = value;
        }
    }


    /// <summary>
/// stayStay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType stayStay
    {
        get
        {
            return _stayStay;
        }

        set
        {
            _stayStay = value;
        }
    }


    /// <summary>
/// subSeatOkKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType subSeatOkKbn
    {
        get
        {
            return _subSeatOkKbn;
        }

        set
        {
            _subSeatOkKbn = value;
        }
    }


    /// <summary>
/// syoyoTime
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syoyoTime
    {
        get
        {
            return _syoyoTime;
        }

        set
        {
            _syoyoTime = value;
        }
    }


    /// <summary>
/// syugoPlaceCdCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType syugoPlaceCdCarrier
    {
        get
        {
            return _syugoPlaceCdCarrier;
        }

        set
        {
            _syugoPlaceCdCarrier = value;
        }
    }


    /// <summary>
/// syugoTime1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTime1
    {
        get
        {
            return _syugoTime1;
        }

        set
        {
            _syugoTime1 = value;
        }
    }


    /// <summary>
/// syugoTime2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTime2
    {
        get
        {
            return _syugoTime2;
        }

        set
        {
            _syugoTime2 = value;
        }
    }


    /// <summary>
/// syugoTime3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTime3
    {
        get
        {
            return _syugoTime3;
        }

        set
        {
            _syugoTime3 = value;
        }
    }


    /// <summary>
/// syugoTime4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTime4
    {
        get
        {
            return _syugoTime4;
        }

        set
        {
            _syugoTime4 = value;
        }
    }


    /// <summary>
/// syugoTime5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTime5
    {
        get
        {
            return _syugoTime5;
        }

        set
        {
            _syugoTime5 = value;
        }
    }


    /// <summary>
/// syugoTimeCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syugoTimeCarrier
    {
        get
        {
            return _syugoTimeCarrier;
        }

        set
        {
            _syugoTimeCarrier = value;
        }
    }


    /// <summary>
/// syuptJiCarrierKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType syuptJiCarrierKbn
    {
        get
        {
            return _syuptJiCarrierKbn;
        }

        set
        {
            _syuptJiCarrierKbn = value;
        }
    }


    /// <summary>
/// syuptPlaceCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType syuptPlaceCarrier
    {
        get
        {
            return _syuptPlaceCarrier;
        }

        set
        {
            _syuptPlaceCarrier = value;
        }
    }


    /// <summary>
/// syuptPlaceCdCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType syuptPlaceCdCarrier
    {
        get
        {
            return _syuptPlaceCdCarrier;
        }

        set
        {
            _syuptPlaceCdCarrier = value;
        }
    }


    /// <summary>
/// syuptTime1
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTime1
    {
        get
        {
            return _syuptTime1;
        }

        set
        {
            _syuptTime1 = value;
        }
    }


    /// <summary>
/// syuptTime2
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTime2
    {
        get
        {
            return _syuptTime2;
        }

        set
        {
            _syuptTime2 = value;
        }
    }


    /// <summary>
/// syuptTime3
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTime3
    {
        get
        {
            return _syuptTime3;
        }

        set
        {
            _syuptTime3 = value;
        }
    }


    /// <summary>
/// syuptTime4
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTime4
    {
        get
        {
            return _syuptTime4;
        }

        set
        {
            _syuptTime4 = value;
        }
    }


    /// <summary>
/// syuptTime5
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTime5
    {
        get
        {
            return _syuptTime5;
        }

        set
        {
            _syuptTime5 = value;
        }
    }


    /// <summary>
/// syuptTimeCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType syuptTimeCarrier
    {
        get
        {
            return _syuptTimeCarrier;
        }

        set
        {
            _syuptTimeCarrier = value;
        }
    }


    /// <summary>
/// teiinseiFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType teiinseiFlg
    {
        get
        {
            return _teiinseiFlg;
        }

        set
        {
            _teiinseiFlg = value;
        }
    }


    /// <summary>
/// teikiCrsKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType teikiCrsKbn
    {
        get
        {
            return _teikiCrsKbn;
        }

        set
        {
            _teikiCrsKbn = value;
        }
    }


    /// <summary>
/// teikiKikakuKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType teikiKikakuKbn
    {
        get
        {
            return _teikiKikakuKbn;
        }

        set
        {
            _teikiKikakuKbn = value;
        }
    }


    /// <summary>
/// tejimaiContactKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tejimaiContactKbn
    {
        get
        {
            return _tejimaiContactKbn;
        }

        set
        {
            _tejimaiContactKbn = value;
        }
    }


    /// <summary>
/// tejimaiDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType tejimaiDay
    {
        get
        {
            return _tejimaiDay;
        }

        set
        {
            _tejimaiDay = value;
        }
    }


    /// <summary>
/// tejimaiKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tejimaiKbn
    {
        get
        {
            return _tejimaiKbn;
        }

        set
        {
            _tejimaiKbn = value;
        }
    }


    /// <summary>
/// tenjyoinCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tenjyoinCd
    {
        get
        {
            return _tenjyoinCd;
        }

        set
        {
            _tenjyoinCd = value;
        }
    }


    /// <summary>
/// tieTyakuyo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tieTyakuyo
    {
        get
        {
            return _tieTyakuyo;
        }

        set
        {
            _tieTyakuyo = value;
        }
    }


    /// <summary>
/// tokuteiChargeSet
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tokuteiChargeSet
    {
        get
        {
            return _tokuteiChargeSet;
        }

        set
        {
            _tokuteiChargeSet = value;
        }
    }


    /// <summary>
/// tokuteiDayFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tokuteiDayFlg
    {
        get
        {
            return _tokuteiDayFlg;
        }

        set
        {
            _tokuteiDayFlg = value;
        }
    }


    /// <summary>
/// ttyakPlaceCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ttyakPlaceCarrier
    {
        get
        {
            return _ttyakPlaceCarrier;
        }

        set
        {
            _ttyakPlaceCarrier = value;
        }
    }


    /// <summary>
/// ttyakPlaceCdCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType ttyakPlaceCdCarrier
    {
        get
        {
            return _ttyakPlaceCdCarrier;
        }

        set
        {
            _ttyakPlaceCdCarrier = value;
        }
    }


    /// <summary>
/// ttyakTimeCarrier
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ttyakTimeCarrier
    {
        get
        {
            return _ttyakTimeCarrier;
        }

        set
        {
            _ttyakTimeCarrier = value;
        }
    }


    /// <summary>
/// tyuijikou
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tyuijikou
    {
        get
        {
            return _tyuijikou;
        }

        set
        {
            _tyuijikou = value;
        }
    }


    /// <summary>
/// tyuijikouKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType tyuijikouKbn
    {
        get
        {
            return _tyuijikouKbn;
        }

        set
        {
            _tyuijikouKbn = value;
        }
    }


    /// <summary>
/// uketukeGenteiNinzu
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType uketukeGenteiNinzu
    {
        get
        {
            return _uketukeGenteiNinzu;
        }

        set
        {
            _uketukeGenteiNinzu = value;
        }
    }


    /// <summary>
/// uketukeStartBi
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType uketukeStartBi
    {
        get
        {
            return _uketukeStartBi;
        }

        set
        {
            _uketukeStartBi = value;
        }
    }


    /// <summary>
/// uketukeStartDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType uketukeStartDay
    {
        get
        {
            return _uketukeStartDay;
        }

        set
        {
            _uketukeStartDay = value;
        }
    }


    /// <summary>
/// uketukeStartKagetumae
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uketukeStartKagetumae
    {
        get
        {
            return _uketukeStartKagetumae;
        }

        set
        {
            _uketukeStartKagetumae = value;
        }
    }


    /// <summary>
/// underKinsi18old
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType underKinsi18old
    {
        get
        {
            return _underKinsi18old;
        }

        set
        {
            _underKinsi18old = value;
        }
    }


    /// <summary>
/// unkyuContactDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType unkyuContactDay
    {
        get
        {
            return _unkyuContactDay;
        }

        set
        {
            _unkyuContactDay = value;
        }
    }


    /// <summary>
/// unkyuContactDoneFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType unkyuContactDoneFlg
    {
        get
        {
            return _unkyuContactDoneFlg;
        }

        set
        {
            _unkyuContactDoneFlg = value;
        }
    }


    /// <summary>
/// unkyuContactTime
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType unkyuContactTime
    {
        get
        {
            return _unkyuContactTime;
        }

        set
        {
            _unkyuContactTime = value;
        }
    }


    /// <summary>
/// unkyuKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType unkyuKbn
    {
        get
        {
            return _unkyuKbn;
        }

        set
        {
            _unkyuKbn = value;
        }
    }


    /// <summary>
/// tojituKokuchiFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType tojituKokuchiFlg
    {
        get
        {
            return _tojituKokuchiFlg;
        }

        set
        {
            _tojituKokuchiFlg = value;
        }
    }


    /// <summary>
/// yusenYoyakuFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yusenYoyakuFlg
    {
        get
        {
            return _yusenYoyakuFlg;
        }

        set
        {
            _yusenYoyakuFlg = value;
        }
    }


    /// <summary>
/// pickupKbnFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType pickupKbnFlg
    {
        get
        {
            return _pickupKbnFlg;
        }

        set
        {
            _pickupKbnFlg = value;
        }
    }


    /// <summary>
/// konjyoOkFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType konjyoOkFlg
    {
        get
        {
            return _konjyoOkFlg;
        }

        set
        {
            _konjyoOkFlg = value;
        }
    }


    /// <summary>
/// tonariFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType tonariFlg
    {
        get
        {
            return _tonariFlg;
        }

        set
        {
            _tonariFlg = value;
        }
    }


    /// <summary>
/// aheadZasekiFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType aheadZasekiFlg
    {
        get
        {
            return _aheadZasekiFlg;
        }

        set
        {
            _aheadZasekiFlg = value;
        }
    }


    /// <summary>
/// yoyakuMediaInputFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yoyakuMediaInputFlg
    {
        get
        {
            return _yoyakuMediaInputFlg;
        }

        set
        {
            _yoyakuMediaInputFlg = value;
        }
    }


    /// <summary>
/// kokusekiFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType kokusekiFlg
    {
        get
        {
            return _kokusekiFlg;
        }

        set
        {
            _kokusekiFlg = value;
        }
    }


    /// <summary>
/// sexBetuFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType sexBetuFlg
    {
        get
        {
            return _sexBetuFlg;
        }

        set
        {
            _sexBetuFlg = value;
        }
    }


    /// <summary>
/// ageFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType ageFlg
    {
        get
        {
            return _ageFlg;
        }

        set
        {
            _ageFlg = value;
        }
    }


    /// <summary>
/// birthdayFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType birthdayFlg
    {
        get
        {
            return _birthdayFlg;
        }

        set
        {
            _birthdayFlg = value;
        }
    }


    /// <summary>
/// telFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType telFlg
    {
        get
        {
            return _telFlg;
        }

        set
        {
            _telFlg = value;
        }
    }


    /// <summary>
/// addressFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType addressFlg
    {
        get
        {
            return _addressFlg;
        }

        set
        {
            _addressFlg = value;
        }
    }


    /// <summary>
/// usingFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType usingFlg
    {
        get
        {
            return _usingFlg;
        }

        set
        {
            _usingFlg = value;
        }
    }


    /// <summary>
/// uwagiTyakuyo
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType uwagiTyakuyo
    {
        get
        {
            return _uwagiTyakuyo;
        }

        set
        {
            _uwagiTyakuyo = value;
        }
    }


    /// <summary>
/// year
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType year
    {
        get
        {
            return _year;
        }

        set
        {
            _year = value;
        }
    }


    /// <summary>
/// yobiCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType yobiCd
    {
        get
        {
            return _yobiCd;
        }

        set
        {
            _yobiCd = value;
        }
    }


    /// <summary>
/// yoyakuAlreadyRoomNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yoyakuAlreadyRoomNum
    {
        get
        {
            return _yoyakuAlreadyRoomNum;
        }

        set
        {
            _yoyakuAlreadyRoomNum = value;
        }
    }


    /// <summary>
/// yoyakuKanouNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yoyakuKanouNum
    {
        get
        {
            return _yoyakuKanouNum;
        }

        set
        {
            _yoyakuKanouNum = value;
        }
    }


    /// <summary>
/// yoyakuNgFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType yoyakuNgFlg
    {
        get
        {
            return _yoyakuNgFlg;
        }

        set
        {
            _yoyakuNgFlg = value;
        }
    }


    /// <summary>
/// yoyakuNumSubSeat
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yoyakuNumSubSeat
    {
        get
        {
            return _yoyakuNumSubSeat;
        }

        set
        {
            _yoyakuNumSubSeat = value;
        }
    }


    /// <summary>
/// yoyakuNumTeiseki
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType yoyakuNumTeiseki
    {
        get
        {
            return _yoyakuNumTeiseki;
        }

        set
        {
            _yoyakuNumTeiseki = value;
        }
    }


    /// <summary>
/// yoyakuStopFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType yoyakuStopFlg
    {
        get
        {
            return _yoyakuStopFlg;
        }

        set
        {
            _yoyakuStopFlg = value;
        }
    }


    /// <summary>
/// zouhatsumotogousya
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType zouhatsumotogousya
    {
        get
        {
            return _zouhatsumotogousya;
        }

        set
        {
            _zouhatsumotogousya = value;
        }
    }


    /// <summary>
/// zouhatsuday
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType zouhatsuday
    {
        get
        {
            return _zouhatsuday;
        }

        set
        {
            _zouhatsuday = value;
        }
    }


    /// <summary>
/// zouhatsuentrypersoncd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType zouhatsuentrypersoncd
    {
        get
        {
            return _zouhatsuentrypersoncd;
        }

        set
        {
            _zouhatsuentrypersoncd = value;
        }
    }


    /// <summary>
/// zasekiHihyojiFlg
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType zasekiHihyojiFlg
    {
        get
        {
            return _zasekiHihyojiFlg;
        }

        set
        {
            _zasekiHihyojiFlg = value;
        }
    }


    /// <summary>
/// zasekiReserveKbn
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType zasekiReserveKbn
    {
        get
        {
            return _zasekiReserveKbn;
        }

        set
        {
            _zasekiReserveKbn = value;
        }
    }


    /// <summary>
/// wtKakuhoSeatNum
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_NumberType wtKakuhoSeatNum
    {
        get
        {
            return _wtKakuhoSeatNum;
        }

        set
        {
            _wtKakuhoSeatNum = value;
        }
    }


    /// <summary>
/// systemEntryPgmid
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemEntryPgmid
    {
        get
        {
            return _systemEntryPgmid;
        }

        set
        {
            _systemEntryPgmid = value;
        }
    }


    /// <summary>
/// systemEntryPersonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemEntryPersonCd
    {
        get
        {
            return _systemEntryPersonCd;
        }

        set
        {
            _systemEntryPersonCd = value;
        }
    }


    /// <summary>
/// systemEntryDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_YmdType systemEntryDay
    {
        get
        {
            return _systemEntryDay;
        }

        set
        {
            _systemEntryDay = value;
        }
    }


    /// <summary>
/// systemUpdatePgmid
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemUpdatePgmid
    {
        get
        {
            return _systemUpdatePgmid;
        }

        set
        {
            _systemUpdatePgmid = value;
        }
    }


    /// <summary>
/// systemUpdatePersonCd
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_MojiType systemUpdatePersonCd
    {
        get
        {
            return _systemUpdatePersonCd;
        }

        set
        {
            _systemUpdatePersonCd = value;
        }
    }


    /// <summary>
/// systemUpdateDay
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
    public EntityKoumoku_YmdType systemUpdateDay
    {
        get
        {
            return _systemUpdateDay;
        }

        set
        {
            _systemUpdateDay = value;
        }
    }
}
