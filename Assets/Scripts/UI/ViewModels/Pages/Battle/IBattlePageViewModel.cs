using System.Collections.Generic;
using Domain.Wave;
using UniRx;

namespace UI.ViewModels.Pages.Battle
{
    public interface IBattlePageViewModel
    {
        IReactiveCollection<Wave> Waves { get; }
    }
}