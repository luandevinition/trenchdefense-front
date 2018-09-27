using System.Collections.Generic;
using Domain.Wave;
using UniRx;

namespace UI.ViewModels.Pages.Battle
{
    public class BattlePageViewModel : IBattlePageViewModel
    {
        public IReactiveCollection<Wave> Waves
        {
            get { return _waves; }
        }

        private readonly IReactiveCollection<Wave> _waves;

        public BattlePageViewModel(List<Wave> waves)
        {
            _waves=new ReactiveCollection<Wave>(waves);
        }
    }
}