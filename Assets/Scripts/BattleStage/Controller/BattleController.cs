using System;
using System.Collections.Generic;
using System.Linq;
using BattleStage.Domain;
using Domain.Wave;
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
        
        [SerializeField]
        private GameObject _darknessUIGameObject;

        private int currentWave = 1;
        private Wave _currentWave;
        
        public void InitData(List<Wave> waves)
        {
            // For Character
            // TODO : Later will get from backend.
            Unit unit = new Unit(new UnitID(1), "Character 1", 10 , 100, 700, new ResourceID(1), new WeaponID(1), null);
            Weapon weapon= new Weapon(new WeaponID(1), 5 , 150, 450);
            
            //For Play Music
            SoundManager.PlayMusic(_backgroundMusic, 1f, true, false);
            
            _currentWave = waves.First();
            Initialize(new BattleInitializeData(unit, _currentWave, new List<Weapon>(){weapon} ));
            
        }

        private int currentSecondCounter = 0;
        public void Initialize(BattleInitializeData data)
        {
            _characterUnitStatus.InitCharacterData(data.Player , data.Weapons);
            _characterUnitStatus.ShowRetryUI.Subscribe(_ =>
            {
                _uIRetry.SetActive(true);
            }).AddTo(this);

            Observable.Interval(new TimeSpan(0, 0, 1)).TakeUntilDisable(this).Subscribe(_ =>
            {
                Wave wave = data.Wave;
                if (wave.WavezsZombies.ContainsKey(currentSecondCounter))
                {
                    List<Zombie> listOfSecond = wave.WavezsZombies[currentSecondCounter];
                    foreach (var zombie in listOfSecond)
                    {
                        var path = string.Format(ENEMY_RESOURCE_FOLDER + ENEMY_RESOURCE_PREFIX, zombie.ResourceID.Value);
                        var enemyPrefab = Resources.Load(path);
                        var enemyObject = Instantiate(enemyPrefab,
                            _positionsSpawn[zombie.Position].position,
                            Quaternion.identity) as GameObject;
                        if (enemyObject != null)
                            enemyObject.GetComponent<BaseUnitStatus>().SetBaseUnitStatus(zombie.HP, zombie.Attack,
                                zombie.Speed, zombie.ResourceID, null, null, zombie.GoldDropCount);
                    }
                }
                currentSecondCounter++;
            });

        }
    }
}

