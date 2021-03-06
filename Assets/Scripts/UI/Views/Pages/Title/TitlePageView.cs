﻿using System;
using DG.Tweening;
using Domain.User;
using EazyTools.SoundManager;
using Facade;
using UI.ViewModels.Pages.Title;
using UI.Views.Parts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Vexe.Runtime.Extensions;

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
        
        [SerializeField] private Toggle _enableToggleSFX;
        [SerializeField] private Toggle _enableToggleBGM;

        [SerializeField] private InputField _accountNameSetting;
        
        [SerializeField] private Button _saveSettingButton;
        [SerializeField] private Button _saveAccountButton;
        
        [SerializeField] private Transform _girdLayoutTransform;
        [SerializeField] private GameObject leaderBoardItemPrefab;
        
        [SerializeField]
        private AudioClip _backgroundMusic;
        
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
                    new GameUserSetting(_enableToggleSFX.isOn, _enableToggleBGM.isOn,
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
                    _backButton.transform.DOLocalMoveX(-650f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _titleText.transform.DOScale(1f, 0.2f).SetRelative();
                    _readyButton.transform.DOLocalMoveX(750f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _accountButton.transform.DOLocalMoveX(750f, 0.55f).SetEase(Ease.InOutFlash).SetRelative();
                    _leaderboardButton.transform.DOLocalMoveX(750f, 0.6f).SetEase(Ease.InOutFlash).SetRelative();
                    _settingButton.transform.DOLocalMoveX(750f, 0.65f).SetEase(Ease.InOutFlash).SetRelative();
                    _multiplayButton.transform.DOLocalMoveX(750f, 0.7f).SetEase(Ease.InOutFlash).SetRelative().OnComplete(() =>
                        {
                            isCompleteMoveMenu = true;
                        });
                }
                else
                {
                    _titleText.transform.DOScale(-1f, 0.2f).SetRelative();
                    _backButton.transform.DOLocalMoveX(650f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _readyButton.transform.DOLocalMoveX(-750f, 0.5f).SetEase(Ease.InOutFlash).SetRelative();
                    _accountButton.transform.DOLocalMoveX(-750f, 0.55f).SetEase(Ease.InOutFlash).SetRelative();
                    _leaderboardButton.transform.DOLocalMoveX(-750f, 0.6f).SetEase(Ease.InOutFlash).SetRelative();
                    _settingButton.transform.DOLocalMoveX(-750f, 0.65f).SetEase(Ease.InOutFlash).SetRelative();
                    _multiplayButton.transform.DOLocalMoveX(-750f, 0.7f).SetEase(Ease.InOutFlash).SetRelative().OnComplete(() =>
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
            
            // For Leaderbaord Button.
            _leaderboardButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (isCompleteMoveMenu)
                {
                    _isShowMenu.Value = false;
                    isCompleteMoveMenu = false;
                    Sequence seqScaleButtonLeaderboard = DOTween.Sequence();
                    seqScaleButtonLeaderboard.Append(_leaderboardButton.transform.DOScale(1.2f, 0.02f));
                    seqScaleButtonLeaderboard.Append(_leaderboardButton.transform.DOScale(1f, 0.02f));
                    seqScaleButtonLeaderboard.OnComplete(() =>
                    {
                        _leaderboardGroup.gameObject.SetActive(true);
                        Sequence seqDisplayGroupSetting = DOTween.Sequence();
                        seqDisplayGroupSetting.Append(_currentCanvasGroup.transform.DOScale(0f, 0.1f));
                        seqDisplayGroupSetting.Append(_leaderboardGroup.transform.DOScale(1f, 0.1f));
                        seqDisplayGroupSetting.OnComplete(()=>{
                            _currentCanvasGroup.gameObject.SetActive(false);
                            _currentCanvasGroup = _leaderboardGroup;
                            _girdLayoutTransform.DestroyChildren();
                            for (int i = 0; i < 10; i++)
                            {
                                var list = viewModel.ListLeaderboard();
                                var gameObjectLeaderboard = Instantiate(leaderBoardItemPrefab, _girdLayoutTransform);
                                string username = "--";
                                string record = "--";
                                if (i < list.Count)
                                {
                                    username = list[i].UserName;
                                    record = list[i].result;
                                }
                                gameObjectLeaderboard.GetComponent<LeaderboardItem>().InitView((i + 1),record, username);
                            }
                            isCompleteMoveMenu = true;
                        });
                    });
                }
            }).AddTo(this);
        }

        private void UpdateUIforGameUser(GameUser gameUserData)
        {
            _titleText.text = string.Format(FORMAT_WELCOME_STRING, gameUserData.Name);
            _enableToggleSFX.isOn = gameUserData.GameSetting.EnableSFX;
            _enableToggleBGM.isOn = gameUserData.GameSetting.EnableBGM;
            _scollbarVolume.value = (gameUserData.GameSetting.VolumeValue/100f);
            _accountNameSetting.text = gameUserData.Name;
            SoundManager.globalMusicVolume = (MyData.MyGameUser.GameSetting.VolumeValue / 100f);
            SoundManager.globalSoundsVolume = (MyData.MyGameUser.GameSetting.VolumeValue / 100f);

            if (MyData.MyGameUser.GameSetting.EnableBGM)
            {
                //For Play Music
                SoundManager.StopAllMusic(0);
                SoundManager.PlayMusic(_backgroundMusic, 1f, true, false,0,0,0,null);
            }
            else
            {
                SoundManager.StopAllMusic(0);
            }
            
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
