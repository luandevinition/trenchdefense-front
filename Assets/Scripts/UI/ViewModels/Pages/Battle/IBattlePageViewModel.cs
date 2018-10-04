using System.Collections.Generic;
using BattleStage.Domain;
using Domain.Wave;
using UniRx;
using Unit = BattleStage.Domain.Unit;
using System.Collections;

namespace UI.ViewModels.Pages.Battle
{
    public interface IBattlePageViewModel
    {
        IReactiveCollection<Wave> Waves { get; }

        IReactiveProperty<Unit> Unit { get; }
        
        IReactiveCollection<Weapon> Weapons { get; }

        IEnumerator NextWave(int currentWave, int hp);

        IEnumerator LoseWave(int currentWave, int hp = 0);
        
        IObservable<Wave> NextWaveObservable { get; }
    }
}