using System;
using System.Collections;
using Components;
using Components.Communication;
using Domain.User;
using Facade;
using StaticAssets.Configs;
using UI.Scripts.PageTransitions;
using UI.ViewModels.Pages.Title;
using UI.Views.Pages.Loading;
using UI.Views.Pages.Title;
using UniRx;
using UnityEngine;

namespace UI.PageTransitions.Loading
{
    /// <summary>
    /// マイページへ遷移するTransition
    /// </summary>
    public class LoadingBundleTransition : PageTransition
    {
        public override IEnumerator LoadAsync()
        {
            _loading.Value = true;
            
            yield return new WaitForSeconds(4f);
            
            _loading.Value = false;
        }

        public override void BindLoadedModels()
        {
            _pageInstance.GetComponent<LoadingBundleView>().Bind();
        }
    }
}
