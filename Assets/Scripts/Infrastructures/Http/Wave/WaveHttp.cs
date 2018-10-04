using System.Collections.Generic;
using System.Linq;
using App.Proto;
using Infrastructures.Factories.User;
using Infrastructures.Factories.Wave;
using Interface.Http.Wave;
using Program.Scripts.Components.Communication;
using ProtoBuf;
using UniRx;
using Unit = BattleStage.Domain.Unit;

namespace Infrastructures.Http.Wave
{
	public class WaveHttp : IWaveHttp
	{
		private const string API_WAVE_DATAS = "/waves/{0}";
		
		private const string API_BEGIN_WAVE = "/match/begin";
		
		private const string API_END_WAVE = "/match/end";
		
		
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
		
		public IObservable<Unit> BeginWave(int wave)
		{
			BeginMatchParameter request = new BeginMatchParameter
			{
				waveID = (uint) wave
			};
			var models = new List<IExtensible> {request};
            
			return _apiClient.Post(API_BEGIN_WAVE,models)
				.Select(MakeUnitByProtobufs)
				.First();
			
		}
		
		public IObservable<Unit> EndWave(int wave, int hp, int dropGoldOfWave)
		{
			EndMatchParameter request = new EndMatchParameter
			{
				waveID = (uint) wave,
				dropGold = (uint) dropGoldOfWave,
				hp = (uint) hp,
				matchResult = hp > 0 ? "complete" : "failure"  //complete, failure

			};
			var models = new List<IExtensible> {request};
            
			return _apiClient.Post(API_END_WAVE,models)
				.Select(MakeUnitByProtobufs)
				.First();
		}
		
		private Unit MakeUnitByProtobufs(IList<IExtensible> protobufs)
		{
			if (protobufs.Count < 1) {
				return null;
			}

			return protobufs.Cast<App.Proto.CharacterStatus>().Select(UserFactory.Make).FirstOrDefault();
		}
		
	}
}
