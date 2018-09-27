using System.Collections.Generic;
using System.Linq;
using Infrastructures.Factories.Wave;
using Interface.Http;
using Interface.Http.Wave;
using Program.Scripts.Components.Communication;
using ProtoBuf;
using UniRx;

namespace Infrastructures.Http.Wave
{
	public class WaveHttp : IWaveHttp
	{
		private const string API_WAVE_DATAS = "/waves/{0}";
		
		
		private readonly IApiClient _apiClient;
	
		public WaveHttp(IApiClient apiClient)
		{
			_apiClient = apiClient;
		}
	
		public IObservable<List<Domain.Wave.Wave>> GetListWaves(int page)
		{
			return _apiClient.Get(string.Format(API_WAVE_DATAS, page))
				.Select(MakeWavesByProtobufs)
				.First();
		}
		
		private List<Domain.Wave.Wave> MakeWavesByProtobufs(IList<IExtensible> protobufs)
		{
			if (protobufs.Count < 1) {
				return new List<Domain.Wave.Wave>();
			}

			return protobufs.Cast<App.Proto.WaveListResult>().Select(WaveFactory.Make).First();
		}
		
	}
}
