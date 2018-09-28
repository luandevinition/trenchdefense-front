using System;
using System.Collections;
using Components;
using Components.Communication;
using Domain.User;
using Facade;
using StaticAssets.Configs;
using UI.PageTransitions.Loading;
using UI.Scripts.PageTransitions;
using UI.Scripts.Route;
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
            
#if DEBUG_TD
            if(!settingData.keepUsingOldToken)
                accessToken = "";
#endif
            if (string.IsNullOrEmpty(accessToken))
            {
#if DEBUG_TD
                var imeiFromSetting = settingData.imei;
                yield return UserComponents.Instance
                    .CreateGameUser(string.IsNullOrEmpty(imeiFromSetting)
                        ? SystemInfo.deviceUniqueIdentifier
                        : imeiFromSetting).StartAsCoroutine(baseGameUser =>
                    {
                        accessToken = baseGameUser.Token;
                        PlayerPrefs.SetString("AccessToken", accessToken);
                        Debug.LogWarning("Create new Access Token :" + accessToken);
                    }, ex => { Debug.LogError("Can't create access token"); });
    
#endif
                yield return UserComponents.Instance
                    .CreateGameUser(SystemInfo.deviceUniqueIdentifier).StartAsCoroutine(baseGameUser =>
                    {
                        accessToken = baseGameUser.Token;
                        PlayerPrefs.SetString("AccessToken", accessToken);
#if DEBUG_TD_LOG  
                        Debug.LogWarning("Create new Access Token :" + accessToken);
#endif
                    }, ex => { Debug.LogError("Can't create access token"); });
            }
            ApiClient.SetAccessTokenToHeader(accessToken);    
                        
            yield return UserComponents.Instance.GetGameUserData().StartAsCoroutine(gameUser =>
            {
                _gameUser = gameUser;
                MyData.MyGameUser = _gameUser;
            },  ex =>
            {
                PlayerPrefs.SetString("AccessToken",String.Empty);
                Debug.LogError("Can't get Gameuser : " + ex.ToString());
                PageRouter.Instance.DoTransition<LoadingBundleTransition>();
            });
#if DEBUG_TD_LOG  
            Debug.LogWarning("Access Token :" + accessToken);
            Debug.LogWarning("Game User : " + _gameUser.Name);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.VolumeValue);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.MuteBGM);
            Debug.LogWarning("Game User : " + _gameUser.GameSetting.MuteSFX);
#endif
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
