using System.Collections.Generic;

namespace BattleStage.Domain
{
    public class WaveData
    {
        public List<SpawnData> SpawnData
        {
            get { return _spawnData; }
        }
        
        private List<SpawnData> _spawnData = new List<SpawnData>();

        public WaveData(List<SpawnData> spawnData)
        {
            spawnData.ForEach(x =>
            {
                _spawnData.Add(x);
            });
        }
    }
}
