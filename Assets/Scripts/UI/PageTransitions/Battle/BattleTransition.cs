using System.Collections;
using System.Collections.Generic;
using BattleStage.Domain;
using Components;
using Domain.Wave;
using UI.Scripts.PageTransitions;
using UI.ViewModels.Pages.Battle;
using UI.Views.Pages.Battle;
using UniRx;
using UnityEngine;
using Unit = BattleStage.Domain.Unit;

namespace UI.PageTransitions.Battle
{
    public class BattleTransition : PageTransition
    {
        private const int THE_FIRST_PAGE = 1;
        
        private List<Wave> _waveList;

        private List<Weapon> _weaponList;
        
        private Unit _unit;
        
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
            
            yield return WavesComponents.Instance.BeginWave(THE_FIRST_PAGE).StartAsCoroutine(unit =>
            {
                _unit = unit;
            },  ex =>
            {
                Debug.LogError("Can't get Waves Data " + ex.ToString());
            });
            
            
            yield return null;
            _loading.Value = false;
        }

        public override void BindLoadedModels()
        {
            var viewModel = new BattlePageViewModel(_waveList,_unit);
            _pageInstance.GetComponent<BattlePageView>().Bind(viewModel);
        }
    }
}
