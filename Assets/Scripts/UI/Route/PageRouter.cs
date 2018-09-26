using System.Collections;
using System.Collections.Generic;
using UI.PageTransitions.Loading;
using UI.Scripts.PageTransitions;
using UnityEngine;
using UniRx;

namespace UI.Scripts.Route
{
    /// <summary>
    /// 現在のシーンでページ遷移させる
    /// </summary>
    /// <remarks>シーンの移動は行わない。現在のシーンのページPrefabを変更する</remarks>
    public class PageRouter : SingletonMonoBehaviour<PageRouter>
    {        
        private List<PageTransition> _transitions;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public List<PageTransition> PageTransitions { get { return _transitions; } }
#endif

        /// <summary>
        /// トランジション完了時に発火されるイベントサブジェクト
        /// </summary>
        private Subject<Unit> _onTransitionCompleted = new Subject<Unit>();
        public IObservable<Unit> OnTransitionCompleted { get { return _onTransitionCompleted; } }

        [SerializeField, Header("長いバージョンのローディング画面のScriptableObjectのリスト。ここで設定したものから一つ抽選されます。")]
        private List<LoadingLongTransition> _loadingLongs;

        public List<LoadingLongTransition> LoadingLongs { get { return _loadingLongs; } }

        [SerializeField, Header("短いバージョンのローディング画面のScriptableObject")]
        private LoadingShortTransition _loadingShort;

        public LoadingShortTransition LoadingShort { get { return _loadingShort; } }

        [SerializeField, Header("UI確認環境として使用する場合はここにチェックを入れてください")]
        private bool _uiDebug;

        /// <summary>
        /// ページトランジションのリソースパス
        /// </summary>
        [SerializeField]
        private string _pageTransitionResourcePath;

        /// <summary>
        /// 現在のページ遷移
        /// </summary>
        private static PageTransition _currentPageTransition;

        /// <summary>
        /// ゲームが起動されたかどうか
        /// </summary>
        private static bool _gameBooted = false;

        [SerializeField] private PageTransition _initializePage;

    #if UNITY_EDITOR
        /// <summary>
        /// ゲームが起動したつもりになる。
        /// バトルのテストで初期ローディング画面を飛ばすために使用する。
        /// </summary>
        public static void SetGameBooted()
        {
            _gameBooted = true;
        }
    #endif

        public PageTransition CurrentPageTransition { get { return _currentPageTransition; } }

        /// <summary>
        /// 次のページ遷移
        /// </summary>
        private static PageTransition _nextPageTransition;

        public PageTransition NextPageTransition { get { return _nextPageTransition; } }

        /// <summary>
        /// ページ遷移する
        /// </summary>
        /// <param name="pageTransition">遷移先ページ</param>
        /// <param name="isForce"></param>
        public IEnumerator Transition(PageTransition pageTransition, bool isForce = false)
        {
            _onTransitionCompleted.OnCompleted();
            _onTransitionCompleted = new Subject<Unit>();

            yield return StartCoroutine(TransitionCoroutine(pageTransition, isForce));
        }

        /// <summary>
        /// ページ遷移処理を行うcoroutine
        /// </summary>
        /// <param name="pageTransition">遷移先ページ</param>
        /// <param name="isForce"></param>
        private IEnumerator TransitionCoroutine(PageTransition pageTransition, bool isForce)
        {
            _nextPageTransition = pageTransition;
            if (!isForce && _currentPageTransition != null &&
                _currentPageTransition.name.Equals(_nextPageTransition.name) &&
                !_nextPageTransition.ForceTransition)
            {
                yield break; //if open the same pageTransition return early
            }

            LoadingShortTransition.StartLoading(false);

            StartCoroutine(DelayedLoadASync());

            if (_currentPageTransition != null)
            {
                _currentPageTransition.DestroyPageInstance();
            }

            // 画面遷移時にローディング画面表示させるため
            if (_nextPageTransition.IsLoading())
            {
                yield return Loading(_nextPageTransition);
            }

            _nextPageTransition.InstantiatePage(transform);
            _currentPageTransition = _nextPageTransition;
            yield return null; // ページにアタッチされているスクリプトのStart()メソッドを実行させるために必要

            // 未使用アセットを破棄
            yield return Resources.UnloadUnusedAssets();
            
            _nextPageTransition.BindLoadedModels();

            LoadingShortTransition.EndLoading();

            _onTransitionCompleted.OnNext(Unit.Default);
        }

        private IEnumerator DelayedLoadASync()
        {
            var orig = _nextPageTransition.LoadAsync();
            while (orig.MoveNext())
            {
                yield return new WaitForFixedUpdate();
                yield return orig.Current;
            }
        }

        /// <summary>
        /// ページ遷移する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DoTransition<T>(bool isForce = false) where T : PageTransition
        {
            var pageTransition = GetTransition<T>();
            StartCoroutine(Transition(pageTransition, isForce));
        }

        /// <summary>
        /// Tranasition一覧に登録されているPageTransitionを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTransition<T>() where T : PageTransition
        {
            return TransitionLoader.GetPageTransition<T>(_transitions, _pageTransitionResourcePath);
        }

        /// <summary>
        /// ローディング画面を表示する
        /// </summary>
        /// <param name="pageTransition">遷移先ページ</param>
        private IEnumerator Loading(PageTransition pageTransition)
        {
            if (pageTransition.IsLongVersionLoading())
            {
                yield return LoadingLongTransition.DoLoading(pageTransition);
            }
            else
            {
                yield return LoadingShortTransition.DoLoading(pageTransition);
            }
        }

        /// <summary>
        /// 初期化を行う
        /// </summary>
        private void Awake()
        {
            _transitions = new List<PageTransition>();
        }

        /// <summary>
        /// 進行中のトランジションを停止する
        /// </summary>
        public void StopTransition()
        {
            StopAllCoroutines();

            // MEMO: ロード中のページをインスタンス化しないと、ポップアップを見るカメラがない
            if (_nextPageTransition != null && _nextPageTransition.IsLoading()) {
                _nextPageTransition.InstantiatePage(transform);
                _currentPageTransition = _nextPageTransition;
            }
        }

        // Use this for initialization
        private void Start()
        {
            if (_uiDebug || _currentPageTransition != null || _gameBooted) return;

            if (_initializePage != null)
            {
                StartCoroutine(Transition(_initializePage));
            }

            _gameBooted = true;
        }

        /// <summary>
        /// TitlePageへ遷移（現在のSceneも考慮する）
        /// </summary>
        public void TransitionToTitlePage()
        {
            PopupController.Instance.CloseAllImmediately();
        }

        private void OnDestroy()
        {
            if (_currentPageTransition)
            {
                _currentPageTransition.DestroyPageInstance();
            }
        }

        /// <summary>
        /// 現在ロードされているページを破棄する
        /// </summary>
        public void ClearCurrentPage()
        {
            if (_currentPageTransition)
            {
                _currentPageTransition.DestroyPageInstance();
            }
        }
    }
}
