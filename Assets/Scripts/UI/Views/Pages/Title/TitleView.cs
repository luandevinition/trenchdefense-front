using UI.ViewModels.Pages.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pages.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        
        public void Bind(ITitleViewModel viewModel)
        {
            _readyButton.OnClickAsObservable().Subscribe(_ =>
            {
                viewModel.OnClickReadyButton();
            }).AddTo(this);
        }
    }
}
