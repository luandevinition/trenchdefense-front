﻿using BattleStage.Domain;
using UnityEngine;

namespace BattleStage.Controller
{
    public class BattleController : MonoBehaviour
    {
        
        private const string ENEMY_RESOURCE_FOLDER = "Prefabs/Enemies/";
        
        private const string ENEMY_RESOURCE_PREFIX = "Enemy_{0:D4}";

        [SerializeField]
        private Transform[] _positionsSpawn;
        
        void Start()
        {
            Initialize(BattleInitializeData.CreateDummy());
        }
        
        public void Initialize(BattleInitializeData data)
        {
            data.Waves[0].SpawnData.ForEach(x =>
            {
                var path = string.Format(ENEMY_RESOURCE_FOLDER + ENEMY_RESOURCE_PREFIX,x.EnemyResourceId);
                var enemyPrefab = Resources.Load(path);

                Instantiate(enemyPrefab,_positionsSpawn[Random.Range(0,_positionsSpawn.Length)].position,Quaternion.identity);
            });
        }
    }
}
