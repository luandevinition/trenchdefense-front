using UnityEngine;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// ローディング画面の遷移処理を司る基底クラス
    /// </summary>
    public abstract class LoadingTransition : ScriptableObject
    {
        [SerializeField, Header("Page Prefab")]
        protected GameObject _pagePrefab;

        [SerializeField, Header("Display log or not !")]
        protected bool _displayLog;

        protected GameObject _pageInstance;

    }
  }