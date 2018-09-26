using System;
using System.Collections;
using Components;
using Components.Communication;
using Domain.User;
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
            if (string.IsNullOrEmpty(accessToken))
            {
                
            yield return UserComponents.Instance.CreateGameUser(SystemInfo.deviceUniqueIdentifier).StartAsCoroutine(baseGameUser =>
                {
                    accessToken = baseGameUser.Token;
                    PlayerPrefs.SetString("AccessToken",accessToken);
                },  ex =>
                {
                    Debug.LogError("Can't create access token");
                });
            }
            ApiClient.SetAccessTokenToHeader(accessToken);    
            
                        
            
            Debug.LogWarning("Access Token :" + accessToken);
            
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
