using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UI.Scripts.PageTransitions;
using UI.Scripts.Parts;

namespace UI.Scripts.Route
{
    /// <summary>
    /// ポップアップ表示の管理を行うクラス
    /// </summary>
    public class PopupController : MonoBehaviour
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        protected static PopupController _instance;

        public static PopupController Instance { get { return _instance; } }

        /// <summary>
        /// Notify when open any popup or sub-page
        /// </summary>
        public IObservable<bool> OnOpenAsObservable { get { return _onOpen; } }

        /// <summary>
        /// 黒フィルター
        /// </summary>
        [SerializeField]
        private Canvas BlackFilter;

        /// <summary>
        /// 黒フィルターのアニメーションフレーム数
        /// </summary>
        [SerializeField]
        private int _filterAnimationFrames;

        /// <summary>
        /// サブページへの遷移情報一覧
        /// </summary>
        /// <remarks>
        /// class 情報で管理されているので、
        /// 同じ class の設定は動作未定義である
        ///
        /// SubPageのTransitionのアセットは下記のフォルダーから読み込まれる
        /// fencer/Assets/UI/Scripts/PageTransitions/Resources/SubPage
        /// </remarks>
        private List<PopupTransition> SubPageTransitions;

        /// <summary>
        /// 汎用ポップアップの遷移情報一覧
        /// </summary>
        /// <remarks>
        /// PopupConfigs.Id で管理されているので
        /// PopupConfigsのId が重複しているものを設定するのは動作未定義である
        ///
        /// CommonPopupのTransitionのアセットは下記のフォルダーから読み込まれる
        /// fencer/Assets/UI/Scripts/PageTransitions/Resources/CommonPopup
        /// </remarks>
        private List<CommonPopupTransition> CommonPopupTransitions;

        /// <summary>
        /// CommonPopupのトランジションがのリソースフォルダーへのパス
        /// </summary>
        [SerializeField]
        private string _commonPopupResourcePath;

        /// <summary>
        /// SubPageのトランジションがのリソースフォルダーへのパス
        /// </summary>
        [SerializeField]
        protected string _subPageResourcePath;

        /// <summary>
        /// ポップアップのスタック
        /// </summary>
        private ReactiveCollection<ShowingPopup> _popupList;

        /// <summary>
        /// List of popup is processing to open or not
        /// </summary>
        private readonly List<PopupTransition> _isProcessingPopup = new List<PopupTransition>();

        /// <summary>
        /// フィルターマテリアル
        /// </summary>
        private Material _filterMaterial;

        /// <summary>
        /// デフォルトフィルターカラー
        /// </summary>
        private Color _defaultFilterColor;

        /// <summary>
        /// 黒フィルターのデフォルトブラー値
        /// </summary>
        private float _defaultFilterSize;

        /// <summary>
        /// フィルターカラーシェーダープロパティ名
        /// </summary>
        private const string FILTER_COLOR_SHADER_PROPERTY_NAME = "_TintColor";

        private IDisposable _blackFilterClickDisposable;
        
        /// <summary>
        /// Notify when open any popup or sub-page
        /// </summary>
        private readonly Subject<bool> _onOpen = new Subject<bool>();

        /// <summary>
        /// BackFilterが消える時の通知
        /// </summary>
        private Subject<ShowingPopup> _onClickBackFilterSubject = new Subject<ShowingPopup>();
        public UniRx.IObservable<CommonPopup.SelectType> OnClickBackFilter
        {
            get { return _onClickBackFilterSubject.Select(_=>CommonPopup.SelectType.Close); }
        }
        
        /// <summary>
        /// CloseAll のコルーチン
        /// </summary>
        private Coroutine _closeallCoroutine = null;
        
        /// <summary>
        /// Popup ID を指定して汎用ポップアップを開く
        /// </summary>
        /// <remarks>
        /// Popup ID とは PopupConfigs.Id の事
        /// </remarks>
        /// <exception cref="InvalidOperationException">指定された ID の PopupTransition が存在しない</exception>
        /// <returns>
        /// 汎用ポップアップを開くコルーチン
        /// 結果を取得すると、汎用ポップアップのボタンイベントストリームを得られる
        /// </returns>
        /// <param name="id">Popup ID</param>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        public IEnumerator OpenCommonPopup(int id, CancellationToken cancel = default(CancellationToken))
        {
            var transition = GetCommonPopupTransition(id);

            var error = default(Exception);
            yield return Open(transition, cancel);
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (error != null)
            {
                throw error;
            }

            CommonPopup popup;
            try
            {
                popup = transition.GetPopupInstance<CommonPopup>();
            }
            catch (Exception ex)
            {
                yield break;
            }

            yield return popup.OnSelectAsObservable();
        }

        /// <summary>
        /// 共通ポップアップのCommonPopupTransitionを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommonPopupTransition GetCommonPopupTransition(int id)
        {
            return TransitionLoader.GetCommonPopupTransition(id, CommonPopupTransitions, _commonPopupResourcePath);
        }

        /// <summary>
        /// 共通サブページを開く
        /// </summary>
        /// <returns>ポップアップを開くコルーチン</returns>
        /// <param name="transitionName">CommonSubPageTransitionから作成したScriptableObject名</param>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        public IEnumerator OpenCommonSubPage(string transitionName,
            CancellationToken cancel = default(CancellationToken))
        {
            var transition = TransitionLoader.GetCommonSubPageTransition(transitionName, SubPageTransitions, _subPageResourcePath);
            var error = default(Exception);
            yield return Open(transition, cancel);
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (error != null)
            {
                throw error;
            }
        }

        /// <summary>
        /// ポップアップを開く
        /// </summary>
        /// <returns>ポップアップを開くコルーチン</returns>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        /// <typeparam name="T">ポップアップの種類</typeparam>
        public IEnumerator OpenPopup<T>(CancellationToken cancel = default(CancellationToken)) where T : PopupTransition
        {
            var transition = GetTransition<T>();

            yield return OpenPopup(transition, cancel);
        }

        /// <summary>
        /// ポップアップを開く
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerator OpenPopup(PopupTransition transition, CancellationToken cancel = default(CancellationToken))
        {
            var error = default(Exception);
            yield return Open(transition, cancel);
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (error != null)
            {
                throw error;
            }
        }

        /// <summary>
        /// ポップアップトランジションを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// 遷移前にトランジションに対してパラメータを渡す時に利用する
        /// </remarks>
        public T GetTransition<T>() where T : PopupTransition
        {
            return TransitionLoader.GetSubPageTransition<T>(SubPageTransitions, _subPageResourcePath);
        }

        /// <summary>
        /// 一番上のポップアップを取得
        /// </summary>
        /// <returns></returns>
        public PopupTransition GetTopTransition()
        {
            var showingPopup = _popupList.FirstOrDefault();

            if (showingPopup == null)
            {
                return null;
            }

            return showingPopup.PopupTransition;
        }

        /// <summary>
        /// 指定したポップアップの親のポップアップを取得
        /// </summary>
        /// <returns></returns>
        public PopupTransition GetParentTransitionOf(PopupTransition popupTransition)
        {
            var showingPopup = _popupList.First(x => x.PopupTransition == popupTransition);
            if (showingPopup == null)
            {
                return null;
            }

            var index = _popupList.IndexOf(showingPopup);

            if (_popupList.Count <= index + 1)
            {
                return null;
            }
            
            return _popupList[index].PopupTransition;
        }

        /// <summary>
        /// ポップアップを開く
        /// </summary>
        /// <returns>
        /// ポップアップを開くコルーチン
        /// </returns>
        /// <param name="transition">遷移情報</param>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        private IEnumerator Open(PopupTransition transition, CancellationToken cancel)
        {
            yield return StartCoroutine(OpenCoroutine(transition, cancel));
        }

        /// <summary>
        /// ポップアップを開く
        /// </summary>
        /// <returns>
        /// ポップアップを開くコルーチン
        /// </returns>
        /// <param name="transition">遷移情報</param>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        protected IEnumerator OpenCoroutine(PopupTransition transition, CancellationToken cancel)
        {
            OpenPreprocessing(transition);
            
            
            if (transition is CommonPopupTransition)
            {
                var isExisted = _isProcessingPopup.Where(x => x is CommonPopupTransition).Any(y =>
                    ((CommonPopupTransition) y).ID == ((CommonPopupTransition) transition).ID);

                if (isExisted)
                {
                    EndLoading(transition);
                    transition.EndLoading();
                    yield break;
                }
            }
            else if (_isProcessingPopup.Any(popup => popup.name.Equals(transition.name)))
            {
                EndLoading(transition);
                transition.EndLoading();
                yield break;
            }

            var error = default(Exception);
            yield return Load(transition, cancel);
            if (cancel.IsCancellationRequested)
            {
                EndLoading(transition);
                transition.EndLoading();
                yield break;
            }

            if (error != null)
            {
                throw error;
            }

            transition.EndLoading();
            // ポップアップを開く
            yield return transition.Open();
            if (error != null)
            {
                yield break;
            }

            // 演出中にキャンセルした場合はポップアップを閉じる
            if (cancel.IsCancellationRequested)
            {
                yield return Close();
                if (error != null)
                {
                    yield break;
                }
            }
            
            _onOpen.OnNext(true);
        }


        /// <summary>
        /// オープン処理の前処理
        /// </summary>
        protected virtual void OpenPreprocessing(PopupTransition transition)
        {
            transition.BeginLoading();
            StartLoading(transition);
        }

        /// <summary>
        /// 進行中のトランジションを停止する
        /// </summary>
        public void StopTransition()
        {
            StopAllCoroutines();
        }

        private void StartLoading(PopupTransition transition)
        {
            if (transition.IsShortVersionLoading())
            {
                LoadingShortTransition.StartLoading();
            }
        }

        private void EndLoading(PopupTransition transition)
        {
            if (transition.IsShortVersionLoading())
            {
                LoadingShortTransition.EndLoading();
            }
        }

        /// <summary>
        /// ポップアップをロードする
        /// ロードされるとポップアップのインスタンスが作成され、スタックに積まれる
        /// </summary>
        /// <param name="transition">遷移情報</param>
        /// <param name="cancel">コルーチン終了を検知するためのトークン</param>
        private IEnumerator Load(PopupTransition transition, CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            // 読み込み開始
            //HACK: できれば通常の async await 使いたい
            var error = default(Exception);

            yield return transition.LoadAsync();
            if (cancel.IsCancellationRequested || error != null)
            {
                if (error != null)
                {
                    throw error;
                }

                yield break;
            }

            // ポップアップのインスタンス準備
            transition.InstantiatePage(transform);

            try
            {
                transition.BindLoadedModels();
            }
            catch (Exception ex)
            {
                transition.DestroyPageInstance();
                throw;
            }

            // 準備完了したので、スタックに積む
            _popupList.Add(new ShowingPopup(transition, null, true));
            _isProcessingPopup.Add(transition);
        }

        /// <summary>
        /// 一番手前のポップアップを閉じる
        /// </summary>
        /// <remarks>
        /// Cancel しても正常な状態に戻せないため、Cancel はサポートしない
        ///
        /// 閉じるポップアップ自身が、自身のStartCoroutineでクローズすると黒フィルターが消えない。
        /// そんな時はOvservable.FromCoroutine()を使いましょう
        /// </remarks>
        /// <exception cref="InvalidOperationException">スタックに何も積まれていない状態で呼び出された</exception>
        public IEnumerator Close()
        {
            var last = _popupList.LastOrDefault();
            if (last == null)
            {
                yield break;
            }

            var error = default(Exception);
            yield return last.PopupTransition.Close();
            if (error != null)
            {
                yield break;
            }

            // スタックから削除
            _popupList.Remove(last);
            _isProcessingPopup.Remove(last.PopupTransition);
            last.PopupTransition.DestroyPageInstance();
            _onOpen.OnNext(false);
        }

        /// <summary>
        /// If you want to close sub-page with closing sub-page animation,use Close instead
        /// This method will close sub-page immediately without waiting closing sub-page animation completed 
        /// </summary>
        public void CloseImmediately()
        {
            var last = _popupList.LastOrDefault();
            if (last == null)
            {
                return;
            }

            last.PopupTransition.TransitionEvent.OnNext(PopupTransition.Event.Close);
            _popupList.Remove(last);
            _isProcessingPopup.Remove(last.PopupTransition);
            last.PopupTransition.DestroyPageInstance();
        }

        /// <summary>
        /// ポップアップコントローラーがコルーチン開始者として最前面にあるポップアップを閉じる
        /// </summary>
        /// <remarks>
        /// CloseAllしたくないことがあるので単体版を用意
        /// </remarks>
        public void CloseStartAsCoroutine()
        {
            StartCoroutine(Close());
        }
        
        /// <summary>
        /// 選択したページまでのポップアップを一番上のポップアップ以外はImmediateで閉じる
        /// </summary>
        /// <returns></returns>
        public IEnumerator CloseToSelectedPageQuickly(PopupTransition transition, bool mySelfToo)
        {
            yield return Close();
            
            CloseImmediateUntilSelectedPage(transition);
            
            if (mySelfToo)
            {
                CloseImmediately();
            }
        }

        /// <summary>
        /// Current PopupTransition
        /// </summary>
        public PopupTransition CurrentPopupTransition
        {
            get { return _popupList.Count > 0 ? _popupList.Last().PopupTransition : null; }
        }

        /// <summary>
        /// 選択したサブページやポップアップまでのポップアップを閉じる
        /// transitionが違うとCloseAllと同じ処理になります
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="mySelfToo"></param>
        public void CloseSelectStartAsCoroutine(PopupTransition transition, bool mySelfToo)
        {
            StartCoroutine(CloseToSelectedPageQuickly(transition, mySelfToo));
        }

        /// <summary>
        /// CloseImmediateUntilSelectedPageStartAsCoroutine
        /// </summary>
        /// <param name="transition">PageTransition</param>
        public void CloseImmediateUntilSelectedPage(PopupTransition transition)
        {
            var count = _popupList.Count;
            for (var i = 0; i < count; i++)
            {
                if (_popupList.Last().PopupTransition == transition)
                {
                    break;
                }

                CloseImmediately();
            }
        }

        /// <summary>
        /// ポップアップを全て閉じる
        /// </summary>
        /// <returns></returns>
        public IEnumerator CloseAll()
        {
            yield return Close();
            
            CloseAllImmediatelyCore();
            
            _closeallCoroutine = null;
        }

        /// <summary>
        /// If you want to close all sub-page with closing sub-page animation one by one,use CloseAll instead
        /// This method will close all sub-page immediately without waiting closing sub-page animation completed 
        /// </summary>
        private void CloseAllImmediatelyCore()
        {
            var count = _popupList.Count;
            for (var i = 0; i < count; i++)
            {
                CloseImmediately();
            }
        }

        public void CloseAllImmediately()
        {
            CloseAllImmediatelyCore();

            StopCloseAllCoroutine();
        }

        /// <summary>
        /// ポップアップコントローラーがコルーチン開始者としてポップアップを全て閉じる
        /// </summary>
        /// <remarks>
        /// ポップアップ自身が、自身を閉じる場合、コルーチンの実行が途中で止まってしまう。
        /// そのため、コルーチンの開始をポップアップコントローラーに任せてコルーチンが止まらないようするためのメソッド
        /// </remarks>
        public void CloseAllStartAsCoroutine()
        {
            _closeallCoroutine = StartCoroutine(CloseAll());
        }

        /// <summary>
        /// CloseAllのコルーチンを止める、主にエラー表示用
        /// </summary>
        public void StopCloseAllCoroutine()
        {
            if (_closeallCoroutine != null)
            {
                StopCoroutine(_closeallCoroutine);
            }
            _closeallCoroutine = null;
        }

        /// <summary>
        /// 指定したIDの共通ポップアップを表示し、選択されたボタンを通知するObservable
        /// </summary>
        /// <param name="popupID"></param>
        /// <returns></returns>
        public UniRx.IObservable<CommonPopup.SelectType> ObservableCommonPopupSelectButton(int popupID)
        {
            return Observable
                .FromCoroutineValue<IObservable<CommonPopup.SelectType>>(() => OpenCommonPopup(popupID))
                .SelectMany(x => x)
                .Merge(OnClickBackFilter)
                .First();
        }

        /// <summary>
        /// 黒フィルターのフェードインアニメーション
        /// </summary>
        /// <returns></returns>
        private IEnumerator FilterFadeIn()
        {
            BlackFilter.gameObject.SetActive(true);
            var red = _defaultFilterColor.r;
            var green = _defaultFilterColor.g;
            var blue = _defaultFilterColor.b;
            var alpha = _defaultFilterColor.a;
            var size = _defaultFilterSize;
            var variableAlpha = 0.0f;
            var variableSize = 0.0f;
            var frames = _filterAnimationFrames;
            while (frames > 0)
            {
                variableAlpha += alpha / _filterAnimationFrames;
                variableSize += size / _filterAnimationFrames;
                _filterMaterial.SetColor(FILTER_COLOR_SHADER_PROPERTY_NAME, new Color(red, green, blue, variableAlpha));
                _filterMaterial.SetFloat("_Size", variableSize);
                yield return null;
                frames--;
            }
        }

        /// <summary>
        /// 黒フィルターのフェードアウトアニメーション
        /// </summary>
        /// <returns></returns>
        private IEnumerator FilterFadeOut()
        {
            var red = _defaultFilterColor.r;
            var green = _defaultFilterColor.g;
            var blue = _defaultFilterColor.b;
            var alpha = _defaultFilterColor.a;
            var size = _defaultFilterSize;
            var variableAlpha = alpha;
            var variableSize = size;
            var frames = _filterAnimationFrames;

            while (frames > 0)
            {
                variableAlpha -= alpha / _filterAnimationFrames;
                variableSize -= size / _filterAnimationFrames;
                _filterMaterial.SetColor(FILTER_COLOR_SHADER_PROPERTY_NAME, new Color(red, green, blue, variableAlpha));
                _filterMaterial.SetFloat("_Size", variableSize);
                yield return null;
                frames--;
            }

            BlackFilter.gameObject.SetActive(false);
        }

        /// <summary>
        /// 初期化を行う
        /// </summary>
        private void Awake()
        {
            _instance = this;
            Initialize();
        }


        protected virtual void Initialize()
        {
            // TODO preload transitions in background
            CommonPopupTransitions = new List<CommonPopupTransition>();
            SubPageTransitions = new List<PopupTransition>();

            _popupList = new ReactiveCollection<ShowingPopup>();
            _popupList.AddTo(this);

            _onClickBackFilterSubject = new Subject<ShowingPopup>();
            // フィルターカラー変更時にオリジナルのシェーダーのプロパティを書き潰さないようにマテリアルごと複製
            var filterImage = BlackFilter.gameObject.GetComponentInChildren<Image>();
            _filterMaterial = new Material(filterImage.material);
            filterImage.material = _filterMaterial;

            // オリジナルのシェーダーに設定されているカラーをデフォルトのフィルターカラーとして設定
            _defaultFilterColor = _filterMaterial.GetColor(FILTER_COLOR_SHADER_PROPERTY_NAME);
            _defaultFilterSize = _filterMaterial.GetFloat("_Size");

            // 最新の表示ページ総数
            var lastPageCount = 0;
            _popupList.ObserveCountChanged(true)
                .Subscribe(count =>
                {
                    var last = _popupList.LastOrDefault();
                    if (last != null)
                    {
                        var top = last.PopupTransition.GetPopupInstance<Parts.Popup>();
                        top.OrderInLayer = count + 1;
                        ApplyFilterColor(last.FilterColor);
                        // サブページ、ポップアップを配置するレイヤーを設定
                        top.SetSortingLayerName();
                    }

                    BlackFilter.sortingOrder = count;
                    // 黒フィルターを配置するレイヤーを設定
                    BlackFilter.sortingLayerName = "Overlay";

                    // サブページまたはポップアップ表示が1ページの場合のみ黒フィルターアニメーションを行う
                    switch (count)
                    {
                        case 0:
                            // 表示するサブページまたはポップアップがない場合、フェードアウトアニメーション
                            StartCoroutine(FilterFadeOut());
                            break;
                        case 1:
                            // 表示するサブページまたはポップアップの総数が1ページの場合、フェードインアニメーション
                            if (lastPageCount < count)
                            {
                                StartCoroutine(FilterFadeIn());
                            }

                            break;
                    }

                    lastPageCount = count;

                    if (_blackFilterClickDisposable != null)
                    {
                        _blackFilterClickDisposable.Dispose();
                    }

                    var clickObservable = BlackFilter.gameObject.GetComponent<ObservablePointerClickTrigger>();
                    _blackFilterClickDisposable = clickObservable.OnPointerClickAsObservable().Subscribe(_ =>
                    {
                        if (last == null || !last.IsFilterClickClose)
                        {
                            return;
                        }

                        _onClickBackFilterSubject.OnNext(_popupList.LastOrDefault());
                    }).AddTo(this);

                    for (var i = 0; i < _popupList.Count - 1; i++)
                    {
                        _popupList[i].PopupTransition.GetPopupInstance<Parts.Popup>().OrderInLayer = i + 1;
                    }
                })
                .AddTo(this);
            
        }
        
        

        /// <summary>
        /// 一番手前のポップアップのフィルターカラーを変更
        /// </summary>
        /// <param name="filterColor">背面のフィルターカラー</param>
        public void ChangeFilterColor(Color? filterColor)
        {
            if (_popupList.Count <= 0)
            {
                return;
            }

            _popupList.Last().FilterColor = filterColor;

            ApplyFilterColor(filterColor);
        }

        /// <summary>
        /// フィルターカラー反映
        /// </summary>
        /// <param name="filterColor">背面のフィルターカラー</param>
        private void ApplyFilterColor(Color? filterColor)
        {
            _filterMaterial.SetColor(FILTER_COLOR_SHADER_PROPERTY_NAME,
                filterColor != null ? (Color) filterColor : _defaultFilterColor);
        }

        /// <summary>
        /// 一番手前のポップアップに対する背面のフィルタークリックでクローズするかどうか設定
        /// </summary>
        /// <param name="isFilterClickClose">背面のフィルタークリックでクローズするかどうか</param>
        public void SetFilterClickClose(bool isFilterClickClose)
        {
            if (_popupList.Count <= 0)
            {
                return;
            }

            _popupList.Last().IsFilterClickClose = isFilterClickClose;
        }

        /// <summary>
        /// １つでも開いているポップアップが存在するか
        /// </summary>
        /// <returns></returns>
        public bool IsExistOpen()
        {
            return _isProcessingPopup.Count != 0;
        }

        /// <summary>
        /// 解除処理を行う
        /// </summary>
        private void OnDestroy()
        {
            //CommonPopupTransitions.ForEach(Resources.UnloadAsset);
            //SubPageTransitions.ForEach(Resources.UnloadAsset);
        }
    }
}
