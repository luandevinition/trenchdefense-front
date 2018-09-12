namespace UI.Scripts.Interfaces.Popup.CommonPopup
{
    /// <summary>
    /// 汎用ポップアップのテキストをフォーマットに応じて変更できるベースクラス
    /// フォーマットが必要ないテキストの場合は各メソッドをオーバーライドしなければ OK です。
    /// </summary>
    public abstract class CommonPopupFormatterBase : ICommonPopupFormatter
    {
        /// <summary>
        /// フォーマットしたポップアップタイトルテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたポップアップタイトルテキスト</returns>
        /// <param name="config">ポップアップの設定</param>
        public virtual string GetTitleText(PopupConfigs config)
        {
            return config.TitleText;
        }

        /// <summary>
        /// フォーマットしたボディテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたポップアップボディテキスト</returns>
        /// <param name="config">ポップアップの設定</param>
        public virtual string GetBodyText(PopupConfigs config)
        {
            return config.BodyText;
        }

        /// <summary>
        /// フォーマットしたボタン１のテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたボタン１のテキスト</returns>
        public virtual string GetButtonText1(PopupButtonConfig config)
        {
            return config.Text;
        }

        /// <summary>
        /// フォーマットしたボタン２のテキストを取得する
        /// </summary>
        /// <returns>フォーマットしたボタン２のテキスト</returns>
        /// <param name="config">ポップアップボタン２の設定</param>
        public virtual string GetButtonText2(PopupButtonConfig config)
        {
            return config.Text;
        }
    }
}
