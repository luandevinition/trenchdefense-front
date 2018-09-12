using UnityEngine;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// ローディング画面の遷移処理を司る基底クラス
    /// </summary>
    public abstract class LoadingTransition : ScriptableObject
    {
        [SerializeField, Header("ローディング画面のprefab")]
        protected GameObject _pagePrefab;

        [SerializeField, Header("ローディング状態のログの出力")]
        protected bool _displayLog;

        protected GameObject _pageInstance;

    }
  }