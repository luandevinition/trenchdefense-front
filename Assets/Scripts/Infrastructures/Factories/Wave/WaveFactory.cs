using System.Collections.Generic;
using System.Linq;
using Domain.Wave;
using Infrastructures.Factories.Zombie;

namespace Infrastructures.Factories.Wave
{
	public static class WaveFactory {

		public static Domain.Wave.Wave Make(App.Proto.Wave dto)
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
			return new Domain.Wave.Wave(new WaveID(0), 0, dto.name,dto.resourceID, WavezsZombies);
		}
	}
}
