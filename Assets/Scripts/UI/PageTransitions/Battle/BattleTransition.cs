using System.Collections;
using System.Collections.Generic;
using Components;
using Domain.Wave;
using UI.Scripts.PageTransitions;
using UI.ViewModels.Pages.Battle;
using UI.Views.Pages.Battle;
using UniRx;
using UnityEngine;

namespace UI.PageTransitions.Battle
{
    public class BattleTransition : PageTransition
    {
        private const int THE_FIRST_PAGE = 1;
        
        private List<Wave> _waveList;
        
        public override IEnumerator LoadAsync()
        {
            _loading.Value = true;
            
            yield return WavesComponents.Instance.GetListWaves(THE_FIRST_PAGE).StartAsCoroutine(waveList =>
            {
                _waveList = waveList;
            },  ex =>
            {
                Debug.LogError("Can't get Waves Data " + ex.ToString());
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
