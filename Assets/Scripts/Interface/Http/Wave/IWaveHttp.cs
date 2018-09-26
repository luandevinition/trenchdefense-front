using System.Collections.Generic;
using UniRx;

namespace Interface.Http.Wave
{
    public interface IWaveHttp {
        IObservable<List<Domain.Wave>> GetListWaves(int page);
    }
}
