using System.Collections;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// 共通用のサブページTransition
    /// </summary>
    public class CommonSubPageTransition : PopupTransition
    {
        /// <summary>
        /// ページの表示に必要なモデルを読み込む
        /// </summary>
        /// <returns></returns>
        /// <remarks>ページの表示に必要なモデルを、サーバから取得したりMyDataクラスから取得したりして保持しておく。
        /// 保持されたモデルは、BindModelsToメソッド内でページビューモデルに渡されるようにする。</remarks>
        public override IEnumerator LoadAsync()
        {
            yield break;
        }

        /// <summary>
        /// 読み込んだモデルを遷移先ページに渡します。
        /// </summary>
        public override void BindLoadedModels()
        {
            // 特に何も処理しない
        }
    }
}
