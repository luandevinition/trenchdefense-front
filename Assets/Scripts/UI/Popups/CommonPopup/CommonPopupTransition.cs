using System.Collections;
using UI.Scripts.Interfaces.Popup.CommonPopup;
using UI.Scripts.Popup.CommonPopup;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// 汎用ポップアップの遷移処理
    /// 汎用ポップアップには読み込みは必要ない
    /// </summary>
    public class CommonPopupTransition : PopupTransition
    {
        /// <summary>
        /// ポップアップの設定
        /// </summary>
        public PopupConfigs Config {
            get {
                if (_runtimeConfig == null || _runtimeConfig.Id == 0) {
                    _runtimeConfig = _config.Clone();
                };
                return _runtimeConfig;
            }
        }

        /// <summary>
        /// ポップアップの設定（インスペクターで設定した状態）
        /// </summary>
        [FormerlySerializedAs("Config")] // TODO 削除するタイミングを検討する（マスターにマージした後）
        [SerializeField]
        private PopupConfigs _config;

        /// <summary>
        /// ポップアップの設定（ゲーム中に使われる）
        /// </summary>
        private PopupConfigs _runtimeConfig;

        /// <summary>
        /// ポップアップ文言のフォーマッター
        /// 設定しなければデフォルトフォーマッターを利用する
        /// </summary>
        private ICommonPopupFormatter _formatter;

        /// <summary>
        /// ポップアップのサウンド設定
        /// </summary>
        private CommonPopupSoundSettings _soundSettings;


        /// <summary>
        /// 汎用 Popup ID を取得する
        /// </summary>
        public int ID {
            get {
                return Config.Id;
            }
        }

        /// <summary>
        /// フォーマッターを設定する
        /// </summary>
        public ICommonPopupFormatter Formatter {
            private get {
                return _formatter;
            } 
            set {
                _formatter = value;
            }
        }

        /// <summary>
        /// サウンド設定
        /// </summary>
        public CommonPopupSoundSettings SoundSettings {
            get {
                return _soundSettings;
            }
        }

        /// <summary>
        /// 本文設定用
        /// </summary>
        private string _bodyText;

        /// <summary>
        /// ページの表示に必要なモデルを読み込みます
        /// </summary>
        /// <returns></returns>
        /// <remarks>ページの表示に必要なモデルを、サーバから取得したりMyDataクラスから取得したりして保持しておきます。
        /// 保持されたモデルは、BindModelsToメソッド内でページビューモデルに渡されるようにします。</remarks>
        public override IEnumerator LoadAsync()
        {
            yield break;
        }

        /// <summary>
        /// 読み込んだモデルを遷移先ページに渡します。
        /// </summary>
        public override void BindLoadedModels()
        {
            GetPopupInstance<Parts.CommonPopup>().BindConfig(Config, _formatter, _soundSettings, _bodyText);
        }

        private void OnEnable()
        {
            _formatter = new DefaultFormatter();
            _soundSettings = new CommonPopupSoundSettings();
        }
        
        /// <summary>
        /// ポップアップのボタンの数を取得
        /// </summary>
        /// <returns></returns>
        public int GetButtonNum()
        {
            var buttonNum = 0;
            if (Config.Button1.ButtonPrefab != null) buttonNum++;
            if (Config.Button2.ButtonPrefab != null) buttonNum++;
            return buttonNum;
        }

        /// <summary>
        /// 本文をフォーマットに応じて組み立てる
        /// </summary>
        /// <param name="replacements"></param>
        public void FormatBodyText(params string[] replacements)
        {
            Config.BodyText = string.Format(_config.BodyText, replacements);
        }

        /// <summary>
        /// タイトルをフォーマットに応じて組み立てる
        /// </summary>
        /// <param name="replacements"></param>
        public void FormatTitleText(params string[] replacements)
        {
            Config.TitleText = string.Format(_config.TitleText, replacements);
        }

        /// ポップアップの本文を設定
        /// </summary>
        /// <param name="textList"></param>
        public void SetBodyText(object[] textList)
        {
            _bodyText = string.Format(Config.BodyText, textList);
        }
    }
}
