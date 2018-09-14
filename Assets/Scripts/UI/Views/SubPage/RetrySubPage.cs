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
                SceneManager.LoadScene(2);
            }).AddTo(this);
        }
    }
}