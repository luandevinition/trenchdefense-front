using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UI.Scripts.Route;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// 長いバージョンのローディング画面の遷移処理を司る
    /// </summary>
    public class LoadingLongTransition : LoadingTransition
    {
        private static System.Random _random = new System.Random();

        /// <summary>
        /// ローディング画面の設定
        /// </summary>
        [SerializeField] private LoadingLongConfigs _config;

        /// <summary>
        /// 長いバージョンのローディング画面の種類の抽選を行う
        /// </summary>
        /// <param name="loadingLongs">抽選されるLoadingTransionのリスト</param>
        public static LoadingLongTransition GetLoadingLongByRandom(List<LoadingLongTransition> loadingLongs)
        {
            return loadingLongs.ElementAt(_random.Next(loadingLongs.Count()));
        }

        /// <summary>
        /// ローディング画面を表示する
        /// </summary>
        /// <param name="pageTransition">遷移先ページ</param>
        /// <remarks>
        /// pageTransition.IsLoading()でオンオフさせる
        /// </remarks>
        public static IEnumerator DoLoading(PageTransition pageTransition)
        {
            var pageInstance = StartLoading();

            while (pageTransition.IsLoading())
            {
                yield return new WaitForFixedUpdate();
            }

            EndLoading(pageInstance);
        }

        /// <summary>
        /// ローディング画面の要素を置き換えます。
        /// </summary>
        /// <param name="pageInstance"></param>
        private void BindConfig(GameObject pageInstance)
        {
            var viewModel = pageInstance.GetComponent<LoadingLongView>();

            viewModel.Bind(_config, pageInstance);
        }

        /// <summary>
        /// ローディング画面を表示を開始する
        /// </summary>
        /// <returns>pageInstance</returns>
        private GameObject Init()
        {
            var pageInstance = (GameObject) Instantiate(_pagePrefab);
            BindConfig(pageInstance);

            return pageInstance;
        }

        /// <summary>
        /// 長いバージョンのローディング画面を表示する
        /// </summary>
        /// <remarks>
        /// 外から呼ぶときはこちら。終了させる時はEndLoading()でpageInstanceを破棄してください。
        /// </remarks>
        /// <returns>pageInstance</returns>
        public static GameObject StartLoading()
        {
            var loadingLong = GetLoadingLongByRandom(PageRouter.Instance.LoadingLongs);

            return loadingLong.Init();
        }

        /// <summary>
        /// ローディング画面を表示を終了する
        /// </summary>
        /// <param name="pageInstance">表示を終了させるGameObject</param>
        public static void EndLoading(GameObject pageInstance)
        {
            Destroy(pageInstance);
        }

    }
}