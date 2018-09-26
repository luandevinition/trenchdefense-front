using DG.Tweening;
using UI.ViewModels.Pages.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pages.Title
{
    public class TitlePageView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _accountButton;
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _multiplayButton;
        
        
        [SerializeField] private CanvasGroup _readyGroup;
        [SerializeField] private CanvasGroup _accountGroup;
        [SerializeField] private CanvasGroup _leaderboardGroup;
        [SerializeField] private CanvasGroup _settingGroup;
        
        [SerializeField] private Text _titleText;
        
        [SerializeField] private Button _backButton;
        
        [SerializeField]
        private ReactiveProperty<bool> _isShowMenu = new ReactiveProperty<bool>(true);

        private bool isCompleteMoveMenu = true;
        
        public void Bind(ITitleViewModel viewModel)
        {
            _isShowMenu.Value = true;
            _readyGroup.gameObject.SetActive(true);
            
            // For Display Animation
            _isShowMenu.Subscribe(isShow =>
            {
                if (isShow)
                {
                    isCompleteMoveMenu = false;
                    _titleText.DOFade(1f, 0.7f).SetRelative();
                    _readyButton.transform.DOMoveX(750f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _accountButton.transform.DOMoveX(750f, 0.55f).SetEase(Ease.InOutFlash).SetRelative();
                    _leaderboardButton.transform.DOMoveX(750f, 0.6f).SetEase(Ease.InOutFlash).SetRelative();
                    _settingButton.transform.DOMoveX(750f, 0.65f).SetEase(Ease.InOutFlash).SetRelative();
                    _multiplayButton.transform.DOMoveX(750f, 0.7f).SetEase(Ease.InOutFlash).SetRelative().OnComplete(() =>
                        {
                            isCompleteMoveMenu = true;
                        });
                }
                else
                {
                    _titleText.DOFade(0f, 0.7f).SetRelative();
                    _readyButton.transform.DOMoveX(-750f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _accountButton.transform.DOMoveX(-750f, 0.55f).SetEase(Ease.InOutFlash).SetRelative();
                    _leaderboardButton.transform.DOMoveX(-750f, 0.6f).SetEase(Ease.InOutFlash).SetRelative();
                    _settingButton.transform.DOMoveX(-750f, 0.65f).SetEase(Ease.InOutFlash).SetRelative();
                    _multiplayButton.transform.DOMoveX(-750f, 0.7f).SetEase(Ease.InOutFlash).SetRelative().OnComplete(() =>
                    {
                        isCompleteMoveMenu = true;
                    });
                }
            }).AddTo(this);
            
            // For Back Button
            _backButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = true;
                    _readyGroup.gameObject.SetActive(true);
                    _accountGroup.gameObject.SetActive(false);
                    _leaderboardGroup.gameObject.SetActive(false);
                    _settingGroup.gameObject.SetActive(false);
                }
            }).AddTo(this);
            
            // For Ready Button.
            _readyButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    viewModel.OnClickReadyButton();
                    _readyButton.transform.DOScale(1.2f, 0.7f).OnComplete(() =>
                    {
                        
                    });
                }
            }).AddTo(this);
            
            // For Setting Button.
            _settingButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    _settingButton.transform.DOScale(1.2f, 0.7f).OnComplete(() =>
                    {
                        _readyGroup.gameObject.SetActive(false);
                        _accountGroup.gameObject.SetActive(false);
                        _leaderboardGroup.gameObject.SetActive(true);
                        _settingGroup.gameObject.SetActive(false);
                    });
                }
            }).AddTo(this);
            
            // For Account Button.
            _accountButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    _accountButton.transform.DOScale(1.2f, 0.7f).OnComplete(() =>
                    {
                        _readyGroup.gameObject.SetActive(false);
                        _accountGroup.gameObject.SetActive(true);
                        _leaderboardGroup.gameObject.SetActive(false);
                        _settingGroup.gameObject.SetActive(false);
                    });
                }
            }).AddTo(this);
        }
    }
}
