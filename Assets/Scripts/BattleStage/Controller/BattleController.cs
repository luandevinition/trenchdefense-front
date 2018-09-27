using System;
using System.Collections.Generic;
using System.Linq;
using BattleStage.Domain;
using Domain.Wave;
using EazyTools.SoundManager;
using Facade;
using UniRx;
using UnityEngine;
using Utils;
using Vexe.Runtime.Extensions;
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
        private GameObject _gameObjetcManagerPool;

        private int currentWave = 1;
        private Wave _currentWave;
        
        public void InitData(List<Wave> waves)
        {
            // For Character
            // TODO : Later will get from backend.
            Unit unit = new Unit(new UnitID(1), "Character 1", 10 , 200, 750, new ResourceID(1), new WeaponID(1), null);
            Weapon weapon1= new Weapon(new WeaponID(1),"AR-25", "Pro" , 5 , 150, 450);
            Weapon weapon2= new Weapon(new WeaponID(2),"AK-47" , "Basic", 5 , 150, 450);
            Weapon weapon3= new Weapon(new WeaponID(3),"HellBlade","Pro" , 5 , 150, 450);

            if (!MyData.MyGameUser.GameSetting.MuteBGM)
            {
                //For Play Music
                SoundManager.PlayMusic(_backgroundMusic, 1f, true, false);
                SoundManager.globalVolume = (MyData.MyGameUser.GameSetting.VolumeValue / 100f);
            }
            
            _currentWave = waves.First();
            Initialize(new BattleInitializeData(unit, _currentWave, new List<Weapon>(){weapon1, weapon2 ,weapon3}));
            
        }
        
        private int currentSecondCounter = 0;
        public void Initialize(BattleInitializeData data)
        {
            _characterUnitStatus.InitCharacterData(data.Player , data.Weapons);
            _characterUnitStatus.ShowRetryUI.Subscribe(_ =>
            {
                _gameObjetcManagerPool.transform.DestroyChildren();
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
                            _positionsSpawn[(zombie.Position - 1)].position,
                            Quaternion.identity) as GameObject;
                        if (enemyObject != null)
                        {
                            enemyObject.transform.SetParent(_gameObjetcManagerPool.transform);
                            enemyObject.GetComponent<BaseUnitStatus>().SetBaseUnitStatus(zombie.HP, zombie.Attack,
                                zombie.Speed, zombie.ResourceID, null, null, zombie.GoldDropCount);
                        }
                    }
                }
                currentSecondCounter++;
            });

        }
    }
}

