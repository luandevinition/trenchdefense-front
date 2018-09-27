using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Components.Communication;
using Domain.User;
using Domain.Wave;
using UI.Scripts.PageTransitions;
using UI.ViewModels.Pages.Battle;
using UI.ViewModels.Pages.Title;
using UI.Views.Pages.Battle;
using UI.Views.Pages.Title;
using UniRx;
using UnityEngine;

namespace UI.PageTransitions.Battle
{
    public class BattleTransition : PageTransition
    {
        private List<Wave> _waveList;
        
        public override IEnumerator LoadAsync()
        {
            _loading.Value = true;
            
            yield return WavesComponents.Instance.GetListWaves(1).StartAsCoroutine(waveList =>
            {
                _waveList = waveList;
            },  ex =>
            {
                Debug.LogError("Can't create access token");
            });
            
            yield return null;
            _loading.Value = false;
        }

        public override void BindLoadedModels()
        {
            var viewModel = new BattlePageViewModel(_waveList);
            _pageInstance.GetComponent<BattlePageView>().Bind(viewModel);
        }
    }
}
