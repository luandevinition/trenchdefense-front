using UI.Scripts.PageTransitions;
using UnityEngine;

namespace UI.Scripts.Route
{
    /// <summary>
    /// 表示中ポップアップの情報をまとめたクラス
    /// </summary>
    public class ShowingPopup
    {
        /// <summary>
        /// ポップアップ遷移
        /// </summary>
        public PopupTransition PopupTransition { get; }

        /// <summary>
        /// 背面のフィルターカラー
        /// </summary>
        public Color? FilterColor { get; set; }

        /// <summary>
        /// 背面のフィルタークリックでクローズするかどうか
        /// </summary>
        public bool IsFilterClickClose { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="popupTransition">ポップアップ遷移</param>
        /// <param name="filterColor">背面のフィルターカラー</param>
        /// <param name="isFilterClickClose">背面のフィルタークリックでクローズするかどうか</param>
        public ShowingPopup(PopupTransition popupTransition, Color? filterColor, bool isFilterClickClose)
        {
            PopupTransition = popupTransition;
            FilterColor = filterColor;
            IsFilterClickClose = isFilterClickClose;
        }
    }
}