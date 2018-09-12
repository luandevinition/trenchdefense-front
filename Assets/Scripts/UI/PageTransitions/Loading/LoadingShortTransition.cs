using System.Collections;
using UnityEngine;
using UI.Scripts.Route;
using Utils;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// 短いバージョンのローディング画面の遷移処理を司る
    /// </summary>
    public class LoadingShortTransition : LoadingTransition
    {
        /// <summary>
        /// ローディング実行数
        /// </summary>
        private int _loadingCount = 0;

        /// <summary>
        /// 現在の背景が半透明かどうか
        /// </summary>
        private bool _isTranslucentNow;

        /// <summary>
        /// 破棄しないかどうか
        /// </summary>
        private bool _isNoDestroy;

        /// <summary>
        /// ローディング画面を表示する
        /// </summary>
        /// <param name="pageTransition">遷移先ページ</param>
        /// <remarks>
        /// pageTransition.IsLoading()でオンオフ
        /// </remarks>
        public static IEnumerator DoLoading(PageTransition pageTransition)
        {
            StartLoading(false); // 画面遷移時は半透明にしない仕様

            while (pageTransition.IsLoading())
            {
                yield return new WaitForFixedUpdate();
            }

            EndLoading();
        }

        /// <summary>
        /// 短いバージョンのローディング画面を表示する
        /// </summary>
        /// <param name="isTranslucent">半透明にするかどうか</param>
        /// <remarks>
        /// 外から呼ぶときはこちら。終了させる時はEndLoading()でpageInstanceを破棄してください。
        /// </remarks>
        /// <returns>pageInstance</returns>
        public static GameObject StartLoading(bool isTranslucent = true)
        {
            // デバッグ用
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] StartLoading\t{0}", isTranslucent));

            return PageRouter.Instance.LoadingShort.Init(isTranslucent);
        }

        /// <summary>
        /// 短いバージョンのローディング画面の表示を終了する
        /// </summary>
        /// <param name="pageInstance">表示を終了させるGameObject</param>
        public static void EndLoading(GameObject pageInstance = null)
        {
            PageRouter.Instance.LoadingShort.Destroy();
        }

        /// <summary>
        /// ローディング中かの判定
        /// </summary>
        /// <returns></returns>
        public static bool IsLoading()
        {
            return PageRouter.Instance.LoadingShort.IsLoadingNow();
        }

        /// <summary>
        /// ローディング画面を表示を開始する
        /// </summary>
        /// <param name="isTranslucent">半透明にするかどうか</param>
        /// <returns>pageInstance</returns>
        private GameObject Init(bool isTranslucent)
        {
            // デバッグ用
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] pageInstance == null\t{0}", _pageInstance == null));

            // インスタンス作成時、ローディング状態を初期化する
            if (_pageInstance == null)
            {
                PageRouter.Instance.LoadingShort.ResetLoadingSetting();
            }

            // デバッグ用
            var lastCount = _loadingCount;
            _loadingCount++;
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] count\t{0} → {1}", lastCount, _loadingCount));

            if (_pageInstance != null)
            {
                // ローディング背景が半透明時にあとから黒背景に変更する処理
                //if (isTranslucent == false && _isTranslucentNow)
                //{
                //    PageRouter.Instance.LoadingShort.SetBackgroundColor(LoadingBackgroundColor.Black);
                //    _isTranslucentNow = false;
                //    PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] isTranslucentNow\t{0}", _isTranslucentNow));
                //}

                return _pageInstance;
            }

            _pageInstance = Instantiate(_pagePrefab);
            if (isTranslucent)
            {
                PageRouter.Instance.LoadingShort.SetBackgroundColor(LoadingBackgroundColor.Translucent);
            }

            return _pageInstance;
        }

        /// <summary>
        /// ローディング画面の表示を終了する
        /// </summary>
        private void Destroy()
        {
            // デバッグ用
            var lastCount = _loadingCount;
            if (_loadingCount > 1)
            {
                _loadingCount--;
                PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] count\t{0} → {1}", lastCount, _loadingCount));
                // カウント数で処理する場合、ここで終わる
                //return;
            }

            if (_pageInstance != null)
            {
                // デバッグ用
                PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] isNoDestroy\t{0}", _isNoDestroy));

                // 破棄しない設定の場合
                if (_isNoDestroy)
                {
                    return;
                }

                // デバッグ用
                PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] Destroy(pageInstance)"));

                Destroy(_pageInstance);
            }
            _pageInstance = null;

            lastCount = _loadingCount;

            // ローディング状態を初期化する
            PageRouter.Instance.LoadingShort.ResetLoadingSetting();

            // デバッグ用
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] count\t{0} → {1}", lastCount, _loadingCount));
        }

        /// <summary>
        /// ローディング中かの判定
        /// </summary>
        /// <returns></returns>
        private bool IsLoadingNow()
        {
            return _pageInstance != null;
        }

        /// <summary>
        /// 破棄の可否を設定する
        /// </summary>
        /// <param name="isDestroyable"></param>
        public static void SetDestroyable(bool isDestroyable)
        {
            PageRouter.Instance.LoadingShort.SetDestroyableState(isDestroyable);
        }

        /// <summary>
        /// 破棄可能かどうかを設定する
        /// </summary>
        /// <param name="isDestroyable"></param>
        private void SetDestroyableState(bool isDestroyable)
        {
            _isNoDestroy = !isDestroyable;

            // デバッグ用
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] SetNoDestroy\t{0}", _isNoDestroy));
        }

        /// <summary>
        /// ローディング状態を初期化する
        /// </summary>
        private void ResetLoadingSetting()
        {
            // デバッグ用
            PageRouter.Instance.LoadingShort.DisplayLog(string.Format("[loading] ResetLoadingSetting"));

            _loadingCount = 0;
            _isTranslucentNow = true;
            _isNoDestroy = false;
        }

        /// <summary>
        /// 背景の色を設定する
        /// </summary>
        public static void SetBackgroundBlack()
        {
            PageRouter.Instance.LoadingShort.SetBackgroundColor(LoadingBackgroundColor.Black);
        }

        private enum LoadingBackgroundColor
        {
            Translucent,
            Black
        }

        /// <summary>
        /// 背景の色を設定する
        /// </summary>
        /// <param name="color"></param>
        private void SetBackgroundColor(LoadingBackgroundColor color)
        {
            var viewModel = _pageInstance.GetComponent<LoadingShortView>();
            switch (color)
            {
                case LoadingBackgroundColor.Translucent:
                    viewModel.DoBackgroundTranslucent(_pageInstance);
                    break;
                case LoadingBackgroundColor.Black:
                    viewModel.DoBackgroundBlack(_pageInstance);
                    break;
                default:
                    viewModel.DoBackgroundTranslucent(_pageInstance);
                    break;
            }
        }

        /// <summary>
        /// ログに出力する（デバッグ用）
        /// </summary>
        /// <param name="message"></param>
        private void DisplayLog(string message)
        {
            if (_displayLog)
            {
                
            }
        }
    }
}