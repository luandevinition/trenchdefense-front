using System.Collections.Generic;
using Components.Communication;
using Infrastructures.Http.Wave;
using Interface.Http.Wave;
using Interface.Repository.Wave;
using UniRx;
using Unit = BattleStage.Domain.Unit;

namespace Infrastructures.Repository.Wave
{
    public class WavesRepository : IWavesRepository
    {
        private static WavesRepository _instance;

        public static WavesRepository GetInstance()
        {
            if (_instance != null) return _instance;

            _instance = new WavesRepository(new WaveHttp(ApiClient.GetInstance()));
            return _instance;
        }

        private readonly IWaveHttp _waveHttp;

        private WavesRepository(WaveHttp waveHttp)
        {
            _waveHttp = waveHttp;
        }


        public IObservable<List<Domain.Wave.Wave>> GetListWaves(int page)
        {
            return _waveHttp.GetListWaves(page);
        }
        
        public IObservable<Unit> BeginWave(int wave)
        {
            return _waveHttp.BeginWave(wave);
        }
        
        public IObservable<Unit> EndWave(int wave, int hp, int dropGoldOfWave)
        {
            return _waveHttp.EndWave(wave,hp,dropGoldOfWave);
        }
    }
}