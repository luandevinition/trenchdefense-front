using System;
using System.Collections.Generic;
using BattleStage.Domain;

namespace Domain.Wave
{
    public class Wave
    {
        public WaveID ID;

        public int WaveNumber;

        public string Name;

        public string ResourceID;
        
        public Dictionary<int, List<Zombie>> WavezsZombies;

        public int NumberZombiesOfWave
        {
            get
            {
                if (WavezsZombies == null)
                    return 0;

                int countZombie = 0;
                foreach (var zombiesOfTime in WavezsZombies)
                {
                    countZombie += zombiesOfTime.Value.Count;
                }
                return countZombie;
            }
        }

        public Wave(WaveID id, int waveNumber, string name, string resourceId, Dictionary<int, List<Zombie>> wavezsZombies)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (resourceId == null) throw new ArgumentNullException("resourceId");
            if (wavezsZombies == null) throw new ArgumentNullException("wavezsZombies");
            
            ID = id;
            WaveNumber = waveNumber;
            Name = name;
            ResourceID = resourceId;
            WavezsZombies = wavezsZombies;
        }
    }
}