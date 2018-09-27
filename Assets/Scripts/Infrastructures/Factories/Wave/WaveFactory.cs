using System.Collections.Generic;
using System.Linq;
using Domain.Wave;
using Infrastructures.Factories.Zombie;
using UnityEngine.Video;

namespace Infrastructures.Factories.Wave
{
	public static class WaveFactory {

		public static List<Domain.Wave.Wave> Make(App.Proto.WaveListResult proto)
		{
			List<Domain.Wave.Wave> result = new List<Domain.Wave.Wave>();
			foreach (var dto in proto.waves)
			{
				var listZombie = dto.zombiePositions.Select(ZombieFactory.Make).ToList();

				Dictionary<int,List<BattleStage.Domain.Zombie>> WavezsZombies = new Dictionary<int, List<BattleStage.Domain.Zombie>>();
				foreach (var zombie in listZombie)
				{
					if (WavezsZombies.ContainsKey(zombie.TimeSpawn))
					{
						WavezsZombies[zombie.TimeSpawn].Add(zombie);
					}
					else
					{
						WavezsZombies.Add(zombie.TimeSpawn,new List<BattleStage.Domain.Zombie>(){zombie});
					}
				}
				var wave = new Domain.Wave.Wave(new WaveID(0), 0, dto.name,dto.resourceID, WavezsZombies);
				result.Add(wave);
			}

			return result;
		}
	}
}
