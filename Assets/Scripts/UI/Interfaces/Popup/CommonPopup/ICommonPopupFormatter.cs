namespace UI.Scripts.Interfaces.Popup.CommonPopup
{
    /// <summary>
    /// 汎用ポップアップのフォーマッタのインターフェイス
    /// </summary>
    public interface ICommonPopupFormatter
    {
        /// <summary>
        /// フォーマットしたポップアップタイトルテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたポップアップタイトルテキスト</returns>
        /// <param name="config">ポップアップの設定</param>
        string GetTitleText(PopupConfigs config);

        /// <summary>
        /// フォーマットしたボディテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたポップアップボディテキスト</returns>
        /// <param name="config">ポップアップの設定</param>
        string GetBodyText(PopupConfigs config);

        /// <summary>
        /// フォーマットしたボタン１のテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたボタン１のテキスト</returns>
        /// <param name="config">ポップアップボタン２の設定</param>
        string GetButtonText1(PopupButtonConfig config);

        /// <summary>
        /// フォーマットしたボタン２のテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたボタン２のテキスト</returns>
        /// <param name="config">ポップアップボタン２の設定</param>
        string GetButtonText2(PopupButtonConfig config);
    }
}
