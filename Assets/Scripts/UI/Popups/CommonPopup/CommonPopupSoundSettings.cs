using System.Collections.Generic;
using UI.Scripts.Settings.Sound;
using CommonPopupSelectType = UI.Scripts.Parts.CommonPopup.SelectType;

namespace UI.Scripts.Popup.CommonPopup
{
    /// <summary>
    /// 汎用ポップアップのサウンド設定
    /// </summary>
    public class CommonPopupSoundSettings
    {
        /// <summary>
        /// 選択結果に応じたボタンサウンド上書き指定ディクショナリ
        /// </summary>
        private Dictionary<CommonPopupSelectType, CommonSoundType?> _selectSoundTypeDictionary = new Dictionary<CommonPopupSelectType, CommonSoundType?>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommonPopupSoundSettings()
        {
        }

        /// <summary>
        /// 選択結果に応じたボタンサウンド設定
        /// </summary>
        /// <param name="selectType">選択結果</param>
        /// <param name="soundType">サウンドタイプ</param>
        public void SetSelectButtonSound(CommonPopupSelectType selectType, CommonSoundType? soundType)
        {
            _selectSoundTypeDictionary[selectType] = soundType;
        }

        /// <summary>
        /// 選択結果に応じたボタンサウンド取得
        /// </summary>
        /// <param name="selectType">選択結果</param>
        /// <returns>サウンドタイプ (設定されていない場合はnullを返却)</returns>
        public CommonSoundType? GetSelectButtonSound(CommonPopupSelectType selectType)
        {
            return _selectSoundTypeDictionary.ContainsKey(selectType) ? _selectSoundTypeDictionary[selectType] : null;
        }
    }
}
