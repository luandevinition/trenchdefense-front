using System;
using DG.Tweening;
using Domain.User;
using Facade;
using UI.ViewModels.Pages.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pages.Title
{
    public class TitlePageView : MonoBehaviour
    {
        private const string FORMAT_WELCOME_STRING = "Welcome \n       {0}";
        
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
        
        [SerializeField] private Scrollbar _scollbarVolume;
        
        [SerializeField] private Toggle _muteToggleSFX;
        [SerializeField] private Toggle _muteToggleBGM;

        [SerializeField] private InputField _accountNameSetting;
        
        [SerializeField] private Button _saveSettingButton;
        [SerializeField] private Button _saveAccountButton;

        
        [SerializeField]
        private ReactiveProperty<bool> _isShowMenu = new ReactiveProperty<bool>(true);

        private CanvasGroup _currentCanvasGroup;
        
        private bool isCompleteMoveMenu = true;
        
        public void Bind(ITitleViewModel viewModel)
        {
            _isShowMenu.Value = true;
            _readyGroup.gameObject.SetActive(true);

            var gameUserData = viewModel.GetGameUser();


            UpdateUIforGameUser(gameUserData);

            viewModel.OnCompleteSaveSetting().Subscribe(newGameUser =>
            {
                MyData.MyGameUser = newGameUser;
                ClickBackButtonFunction();
            }).AddTo(this);

            _saveAccountButton.OnClickAsObservable().Subscribe(_ =>
            {
                var newGameUserData = new GameUser(_accountNameSetting.text, MyData.MyGameUser.GameSetting);
                viewModel.SaveGameSetting(newGameUserData);
            }).AddTo(this);

            _saveSettingButton.OnClickAsObservable().Subscribe(_ =>
            {
                var newGameUserData = new GameUser(MyData.MyGameUser.Name,
                    new GameUserSetting(_muteToggleSFX.isOn, _muteToggleBGM.isOn,
                        (int) (_scollbarVolume.value * 100f)));
                viewModel.SaveGameSetting(newGameUserData);
            }).AddTo(this);
            
            _currentCanvasGroup = _readyGroup;
            // For Display Animation
            _isShowMenu.Subscribe(isShow =>
            {
                if (isShow)
                {
                    isCompleteMoveMenu = false;
                    _backButton.transform.DOMoveX(-650f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _titleText.transform.DOScale(1f, 0.2f).SetRelative();
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
                    _titleText.transform.DOScale(-1f, 0.2f).SetRelative();
                    _backButton.transform.DOMoveX(650f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
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
                ClickBackButtonFunction();
            }).AddTo(this);
            
            // For Ready Button.
            _readyButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    viewModel.OnClickReadyButton();
                    Sequence seqScaleButtonReady = DOTween.Sequence();
                    seqScaleButtonReady.Append(_readyButton.transform.DOScale(1.2f, 0.5f));
                    seqScaleButtonReady.Append(_readyButton.transform.DOScale(1f, 0.25f));
                }
            }).AddTo(this);
            
            // For Setting Button.
            _settingButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    isCompleteMoveMenu = false;
                    Sequence seqScaleButtonSetting = DOTween.Sequence();
                    seqScaleButtonSetting.Append(_settingButton.transform.DOScale(1.2f, 0.02f));
                    seqScaleButtonSetting.Append(_settingButton.transform.DOScale(1f, 0.02f));
                    seqScaleButtonSetting.OnComplete(() =>
                    {
                        _settingGroup.gameObject.SetActive(true);
                        Sequence seqDisplayGroupSetting = DOTween.Sequence();
                        seqDisplayGroupSetting.Append(_currentCanvasGroup.transform.DOScale(0f, 0.1f));
                        seqDisplayGroupSetting.Append(_settingGroup.transform.DOScale(1f, 0.1f));
                        seqDisplayGroupSetting.OnComplete(()=>{
                            _currentCanvasGroup.gameObject.SetActive(false);
                            _currentCanvasGroup = _settingGroup;
                            isCompleteMoveMenu = true;
                        });
                    });
                }
            }).AddTo(this);
            
            // For Account Button.
            _accountButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    isCompleteMoveMenu = false;
                    Sequence seqScaleButtonAccount = DOTween.Sequence();
                    seqScaleButtonAccount.Append(_accountButton.transform.DOScale(1.2f, 0.02f));
                    seqScaleButtonAccount.Append(_accountButton.transform.DOScale(1f, 0.02f));
                    seqScaleButtonAccount.OnComplete(() =>
                    {
                        _accountGroup.gameObject.SetActive(true);
                        Sequence seqDisplayGroupSetting = DOTween.Sequence();
                        seqDisplayGroupSetting.Append(_currentCanvasGroup.transform.DOScale(0f, 0.1f));
                        seqDisplayGroupSetting.Append(_accountGroup.transform.DOScale(1f, 0.1f));
                        seqDisplayGroupSetting.OnComplete(()=>{
                            _currentCanvasGroup.gameObject.SetActive(false);
                            _currentCanvasGroup = _accountGroup;
                            isCompleteMoveMenu = true;
                        });
                    });
                }
            }).AddTo(this);
        }

        private void UpdateUIforGameUser(GameUser gameUserData)
        {
            _titleText.text = string.Format(FORMAT_WELCOME_STRING, gameUserData.Name);
            _muteToggleSFX.isOn = !gameUserData.GameSetting.MuteSFX;
            _muteToggleBGM.isOn = !gameUserData.GameSetting.MuteBGM;
            _scollbarVolume.value = (gameUserData.GameSetting.VolumeValue/100f);
            _accountNameSetting.text = gameUserData.Name;
        }

        private void ClickBackButtonFunction()
        {
            UpdateUIforGameUser(MyData.MyGameUser);
            if (isCompleteMoveMenu)
            {
                isCompleteMoveMenu = false;
                Sequence seqScaleButtonBack = DOTween.Sequence();
                seqScaleButtonBack.Append(_backButton.transform.DOScale(1.2f, 0.02f));
                seqScaleButtonBack.Append(_backButton.transform.DOScale(1f, 0.02f));
                seqScaleButtonBack.OnComplete(() =>
                {
                    _readyGroup.gameObject.SetActive(true);
                    Sequence seqDisplayGroupBack = DOTween.Sequence();
                    seqDisplayGroupBack.Append(_currentCanvasGroup.transform.DOScale(0f, 0.1f));
                    seqDisplayGroupBack.Append(_readyGroup.transform.DOScale(1f, 0.1f));
                    seqDisplayGroupBack.OnComplete(()=>{
                        _currentCanvasGroup.gameObject.SetActive(false);
                        _currentCanvasGroup = _readyGroup;
                        isCompleteMoveMenu = true;
                        _isShowMenu.Value = true;
                    });
                });
            }
        }
    }
}
