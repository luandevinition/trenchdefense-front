using System;
using System.Collections.Generic;
using BattleStage.Domain;
using EazyTools.SoundManager;
using UniRx;
using UnityEngine;
using Utils;
using CharacterController = BattleStage.Controller.Character.CharacterController;
using Random = UnityEngine.Random;
using Unit = BattleStage.Domain.Unit;

namespace BattleStage.Controller
{
    public class BattleController : MonoBehaviour
    {
        
        private const string ENEMY_RESOURCE_FOLDER = "Prefabs/Enemies/";
        
        private const string ENEMY_RESOURCE_PREFIX = "Enemy_{0:D4}";
        
        private const string PLAYER_RESOURCE_FOLDER = "Prefabs/Characters/";
        
        private const string CHARACTER_RESOURCE_PREFIX = "Character_{0:D4}";
        
        [SerializeField]
        private Transform[] _positionsSpawn;
        
        [SerializeField]
        private CharacterController _characterUnitStatus;

        [SerializeField]
        private GameObject _uIRetry;

        [SerializeField]
        private AudioClip _backgroundMusic;

        private BattleInitializeData _data;
        private readonly List<List<Zombie>> _waveZombies = new List<List<Zombie>>();
        private int baseWavePower = 1000;
        private int currentWave = 1;
        private int waveTime = 20;
        
        void Start()
        {
            // Dummy Data for Sprint 2 
            // TODO : Later will get from backend.
            Unit unit = new Unit(new UnitID(1), "Character 1", 10 , 100, 700, new ResourceID(1), new WeaponID(1), null);
            Zombie zombie01= new Zombie(new ZombieID(1), "Zombie 1", 10 , 50, 500, 30, new ResourceID(1));
            //Zombie zombie02= new Zombie(new ZombieID(2), "Zombie 2", 5 , 150, 450, 30, new ResourceID(2));
            
            Weapon weapon= new Weapon(new WeaponID(1), 5 , 150, 450);
            
            for(int index = 0 ; index < waveTime ; index++)
            {
                _waveZombies.Add(new List<Zombie>());
            }
            
            Initialize(new BattleInitializeData(unit,new List<Zombie>(){zombie01}, new List<Weapon>(){weapon} ));
            
            SoundManager.PlayMusic(_backgroundMusic, 1f, true, false);
        }

        private int currentSecondCounter = 0;
        public void Initialize(BattleInitializeData data)
        {
            _data = data;
            CreateWaveData();
            
            _characterUnitStatus.InitCharacterData(data.Player , data.Weapons);
            _characterUnitStatus.ShowRetryUI.Subscribe(_ =>
            {
                _uIRetry.SetActive(true);
            }).AddTo(this);
            
            Observable.Interval(new TimeSpan(0, 0, 1)).TakeUntilDisable(this).Subscribe(_ =>
            {
                List<Zombie> listOfSecond = _waveZombies[currentSecondCounter];
                foreach (var zombie in listOfSecond)
                {
                    var path = string.Format(ENEMY_RESOURCE_FOLDER + ENEMY_RESOURCE_PREFIX,zombie.ResourceID.Value);
                    var enemyPrefab = Resources.Load(path);
                    var enemyObject = Instantiate(enemyPrefab,_positionsSpawn[Random.Range(0,_positionsSpawn.Length)].position,Quaternion.identity) as GameObject;
                    if(enemyObject != null)
                        enemyObject.GetComponent<BaseUnitStatus>().SetBaseUnitStatus(zombie.HP, zombie.Attack, zombie.Speed, zombie.ResourceID, null, null, zombie.GoldDropCount);
                }
             
                currentSecondCounter++;
                if (currentSecondCounter >= waveTime)
                {
                    currentSecondCounter = 0;
                    CreateWaveData();
                }
            });
            
        }

        private void CreateWaveData()
        {
            int zombieCount = (int)((baseWavePower * currentWave * 0.2f) / 100f);
            int currentZombieSpawn = 0;
            int currentIndex = 0;
            while (currentZombieSpawn < zombieCount)
            {
                var listOnIndex = _waveZombies[currentIndex];
                var result = Random.Range(1, 4) == 1;
                if (result)
                {
                    var randomZombie = _data.ListZombies.GetRandomZombie();
                    listOnIndex.Add(randomZombie);
                    currentZombieSpawn++;
                }
                
                currentIndex++;
                if (currentIndex >= waveTime)
                {
                    currentIndex = 0;
                }
            }
            currentWave++;

        }
        
        
    }
}

