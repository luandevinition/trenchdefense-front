using System.Collections.Generic;
using UniRx;
using Unit = BattleStage.Domain.Unit;

namespace Interface.Repository.Wave
{
    public interface IWavesRepository
    {
        IObservable<List<Domain.Wave.Wave>> GetListWaves(int page);

        IObservable<Unit> BeginWave(int wave);

        IObservable<Unit> EndWave(int wave, int hp, int dropGoldOfWave);
    }
}
