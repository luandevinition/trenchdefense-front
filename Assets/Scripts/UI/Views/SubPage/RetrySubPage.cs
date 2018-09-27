using EZ_Pooling;
using UI.PageTransitions.Title;
using UI.Scripts.Route;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Views.SubPage
{
    public class RetrySubPage : MonoBehaviour
    {
        public Button ButtonRetry;

        void Start()
        {
            ButtonRetry.OnClickAsObservable().Subscribe(_ =>
            {
                Time.timeScale = 1f;
                PageRouter.Instance.DoTransition<TitleTransition>(true);
            }).AddTo(this);
        }
    }
}