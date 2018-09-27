using System.Collections.Generic;
using UniRx;

namespace Interface.Repository.Wave
{
    public interface IWavesRepository
    {
        IObservable<List<Domain.Wave.Wave>> GetListWaves(int page);
    }
}
