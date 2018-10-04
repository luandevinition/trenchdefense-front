using System.Collections.Generic;
using System.Runtime.InteropServices;
using Domain;
using Domain.Wave;
using Infrastructures.Repository.Wave;
using Interface.Repository.Wave;
using UniRx;
using Unit = BattleStage.Domain.Unit;

namespace Components
{
    public class WavesComponents
    {
        private static WavesComponents _instance;

        public static WavesComponents Instance
        {
            get
            {
                if (_instance != null) return _instance;                
                _instance = new WavesComponents(WavesRepository.GetInstance());

                return _instance;
            }
        }

        private readonly IWavesRepository _repository;

        
        private WavesComponents(IWavesRepository repository)
        {
            _repository = repository;
        }
        
        public IObservable<List<Wave>> GetListWaves(int page)
        {
            return _repository.GetListWaves(page);
        }
        
        public IObservable<Unit> BeginWave(int wave)
        {
            return _repository.BeginWave(wave);
        }
        
        public IObservable<Unit> EndWave(int wave, int hp, int dropGoldOfWave)
        {
            return _repository.EndWave(wave,hp,dropGoldOfWave);
        }
        
    }
}