using System.Collections.Generic;
using Components.Communication;
using Infrastructures.Http.Wave;
using Interface.Http.Wave;
using Interface.Repository.Wave;
using UniRx;

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


        public IObservable<List<Domain.Wave>> GetListWaves(int page)
        {
            return _waveHttp.GetListWaves(page);
        }
    }
}