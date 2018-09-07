using System.Collections.Generic;
using UnityEngine;

namespace BattleStage.Domain
{
    public class BattleInitializeData
    {
        public List<WaveData> Waves
        {
            get { return _waves; }
        }

        private readonly List<WaveData> _waves = new List<WaveData>();

        public BattleInitializeData(List<WaveData> waves)
        {
            waves.ForEach(x =>
            {
                _waves.Add(x);
            });
        }

        public static BattleInitializeData CreateDummy()
        {
            var data = new BattleInitializeData(new List<WaveData>()
            {
                new WaveData(new List<SpawnData>()
                {
                    new SpawnData(
                        new Vector3(Random.Range(1000, 2000),
                            Random.Range(1000, 2000), 0f), 1),
                    new SpawnData(
                        new Vector3(Random.Range(1000, 2000),
                            Random.Range(1000, 2000), 0f), 1),
                    new SpawnData(
                        new Vector3(Random.Range(1000, 2000),
                            Random.Range(1000, 2000), 0f), 1),
                    new SpawnData(
                        new Vector3(Random.Range(1000, 2000),
                            Random.Range(1000, 2000), 0f), 1),
                    new SpawnData(
                        new Vector3(Random.Range(1000, 2000),
                            Random.Range(1000, 2000), 0f), 1)
                })
            });
            return data;
        }
    }
}
