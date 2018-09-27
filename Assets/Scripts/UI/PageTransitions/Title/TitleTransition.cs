using System;
using System.Collections;
using Components;
using Components.Communication;
using Domain.User;
using StaticAssets.Configs;
using UI.Scripts.PageTransitions;
using UI.ViewModels.Pages.Title;
using UI.Views.Pages.Title;
using UniRx;
using UnityEngine;

namespace UI.PageTransitions.Title
{
    /// <summary>
    /// マイページへ遷移するTransition
    /// </summary>
    public class TitleTransition : PageTransition
    {
        private GameUser _gameUser;
        
        public override IEnumerator LoadAsync()
        {
            _loading.Value = true;
            
            var accessToken = PlayerPrefs.GetString("AccessToken",String.Empty);
            var settingData = Resources.Load<SettingData>("SettingData");
            // Begin For Testing
            if(!settingData.keepUsingOldToken)
                accessToken = "";
            // End For Testing
            if (string.IsNullOrEmpty(accessToken))
            {
                // Begin For Testing
                var imeiFromSetting = settingData.imei;
                // End For Testing
                yield return UserComponents.Instance
                    .CreateGameUser(string.IsNullOrEmpty(imeiFromSetting)
                        ? SystemInfo.deviceUniqueIdentifier
                        : imeiFromSetting).StartAsCoroutine(baseGameUser =>
                    {
                        accessToken = baseGameUser.Token;
                        PlayerPrefs.SetString("AccessToken", accessToken);
                        Debug.LogWarning("Create new Access Token :" + accessToken);
                    }, ex => { Debug.LogError("Can't create access token"); });
            }
            ApiClient.SetAccessTokenToHeader(accessToken);    
            
                        
            yield return UserComponents.Instance.GetGameUserData().StartAsCoroutine(gameUser =>
            {
                _gameUser = gameUser;
            },  ex =>
            {
                Debug.LogError("Can't get ");
            });
            
            Debug.LogWarning("Access Token :" + accessToken);
            Debug.LogWarning("Game User : " + _gameUser.Name);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.VolumeValue);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.MuteBGM);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.MuteSFX);
      
            yield return null;
            _loading.Value = false;
        }

        public override void BindLoadedModels()
        {
            var viewModel = new TitleViewModel(_gameUser);
            _pageInstance.GetComponent<TitlePageView>().Bind(viewModel);
        }
    }
}
