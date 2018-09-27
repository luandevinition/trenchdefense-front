using System.Collections.Generic;
using System.Runtime.InteropServices;
using Domain;
using Domain.Wave;
using Infrastructures.Repository.Wave;
using Interface.Repository.Wave;
using UniRx;

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
        
    }
}