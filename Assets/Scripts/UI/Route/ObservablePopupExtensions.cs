using UI.Scripts.PageTransitions;
using UI.Scripts.Parts;
using UniRx;

namespace UI.Scripts.Route
{
    /// <summary>
    /// ポップアップ関連のUniRxのExtension関数
    /// </summary>
    public static class ObservablePopupExtensions
    {
        /// <summary>
        /// 一般ポップアップを開き、ボタンクリックで閉じる
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="popupID"></param>
        public static IObservable<CommonPopup.SelectType> OpenAndCloseCommonPopup<T>(this IObservable<T> stream, int popupID)
        {
            return stream
                .OpenCommonPopup(popupID)
                .CloseCommonPopup()
                ;
        }

        /// <summary>
        /// 一般ポップアップを開き、ボタンクリックで閉じる
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="popupID"></param>
        public static IObservable<CommonPopup.SelectType> OpenAndCloseCommonPopup<T>(
            this IObservable<T> stream,
            int popupID,
            System.Action<CommonPopupTransition> setupTransition
        )
        {
            return stream
                .OpenCommonPopup(popupID, setupTransition)
                .CloseCommonPopup()
                ;
        }

        /// <summary>
        /// 一般ポップアップを開く
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="popupID"></param>
        public static IObservable<CommonPopup.SelectType> OpenCommonPopup<T>(this IObservable<T> stream, int popupID)
        {
            return stream
                .SelectMany(_ => PopupController.Instance.ObservableCommonPopupSelectButton(popupID))
                ;
        }

        /// <summary>
        /// 一般ポップアップを開く
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="popupID"></param>
        /// <param name="setupTransition"></param>
        public static IObservable<CommonPopup.SelectType> OpenCommonPopup<T>(
            this IObservable<T> stream,
            int popupID,
            System.Action<CommonPopupTransition> setupTransition
        )
        {
            var transition = PopupController.Instance.GetCommonPopupTransition(popupID);
            setupTransition(transition);
            return OpenCommonPopup(stream, popupID);
        }

        /// <summary>
        /// 一般ポップアップを閉じる
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="popupID"></param>
        public static IObservable<T> CloseCommonPopup<T>(this IObservable<T> stream)
        {
            return stream
                .SelectMany(val => Observable.FromCoroutine(PopupController.Instance.Close)
                    .Select(_ => val)
                )
                ;
        }
    }
}
