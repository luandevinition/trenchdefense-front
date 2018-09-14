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

        
        public List<Weapon> Weapons
        {
            get { return _weapons; }
        }

        private readonly List<Weapon> _weapons;

        
        /* Get Backend and include data.
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
        */
        #region For Testing Sprint 2

        public List<Zombie> ListZombies
        {
            get { return _zombies; }
        }

        private readonly List<Zombie> _zombies;

        private readonly Unit _player;

        public Unit Player
        {
            get { return _player; }
        }

        public BattleInitializeData(Unit unitSelectedData, List<Zombie> listZombieIds, List<Weapon> weapons)
        {
            _zombies = listZombieIds;
            _player = unitSelectedData;
            _weapons = weapons;
        }
        
        #endregion
      
    }
}
