using UnityEngine;
using UniRx;
using System.Collections;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// ページ遷移を表すクラス
    /// </summary>
    /// <remarks>
    /// ページ1つにつきこのクラスを継承した具象クラスを1つ作成し、ScriptableObjectでアセット化します。
    /// それにページPrefabをアタッチし、PageRouterのインスペクターで設定すれば遷移できるようになります。
    /// このクラスの役割は、ページ表示に必要なモデルを取得し、それらモデルを遷移先ページに渡してあげることです。
    /// </remarks>
    public abstract class PageTransition : ScriptableObject
    {
        [SerializeField] protected GameObject _pagePrefab;

        protected GameObject _pageInstance;
        public GameObject PageInstance {
            get { return _pageInstance; }
        }

        protected PageTransition()
        {
            _loading = new BoolReactiveProperty(false);
            _isLongVersionLoading = new BoolReactiveProperty(false);
        }

        protected BoolReactiveProperty _loading;

        [SerializeField, Header("ローディング画面を長いバージョンにするかどうか")]
        protected BoolReactiveProperty _isLongVersionLoading;

        /// <summary>
        /// Transition可能かの判定
        /// </summary>
        public virtual bool ForceTransition {
            get {return false;}
        }
        
        /// <summary>
        /// ページの表示に必要なモデルを読み込みます
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// ページの表示に必要なモデルを、サーバから取得したりMyDataクラスから取得したりして保持しておきます。
        /// 保持されたモデルは、BindModelsToメソッド内でページビューモデルに渡されるようにします。
        /// </remarks>
        public abstract IEnumerator LoadAsync();

        /// <summary>
        /// デフォルトのフレームレート数
        /// </summary>
        private int _defaultFrameRate = 30;

        private void Awake()
        {
            Application.targetFrameRate = _defaultFrameRate;
        }
    
        
        /// <summary>
        /// ページPrefabをインスタンス化します
        /// </summary>
        /// <param name="parent"></param>
        public virtual void InstantiatePage(Transform parent)
        {
            
            _pageInstance = (GameObject) Instantiate(_pagePrefab, parent);
        }

        /// <summary>
        /// 読み込んだモデルを遷移先ページに渡します。
        /// </summary>
        public abstract void BindLoadedModels();

        /// <summary>
        /// このクラスによってインスタンス化されたページを削除します
        /// </summary>
        public void DestroyPageInstance()
        {
            Destroy(_pageInstance);
        }
        
        /// <summary>
        /// このクラスによってインスタンス化されたページを削除します
        /// </summary>
        public void DestroyPageInstanceImmediate()
        {
            DestroyImmediate(_pageInstance);
        }

        /// <summary>
        /// なうろーでぃんぐ確認
        /// </summary>
        /// <returns></returns>
        public bool IsLoading()
        {
            return _loading.Value;
        }

        /// <summary>
        /// ローディングが長いバージョンかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsLongVersionLoading()
        {
            return _isLongVersionLoading.Value;
        }

        /// <summary>
        /// Stop loading immediately
        /// </summary>
        public void StopLoading()
        {
            _loading.Value = false;
            _isLongVersionLoading.Value = false;
        }

    }

}

