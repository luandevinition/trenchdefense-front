using System;
using System.Collections.Generic;
using System.Linq;
using BattleStage.Controller.Enemy;
using BattleStage.Domain;
using Domain.Wave;
using EazyTools.SoundManager;
using Facade;
using UI.ViewModels.Pages.Battle;
using UI.Views.Parts.Buttons;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
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
        private GameObject _gameObjetcManagerPool;
        
        [SerializeField]
        private GameObject _gameObjetcNewWaveUI;
        
        [SerializeField]
        private FireButtonView _throwSuppliesButtonView;

        private int currentWave = 1;
        private Wave _currentWave;

        private ISubject<int> _selectWeaponIndex;


        public Text totalZombiesText;
        public Text killedZombiesText;

        public Text timePlayedText;
        
        private readonly Subject<int> killedZombies = new Subject<int>();
        
        private readonly Subject<int> timePlayed = new Subject<int>();
        
        private Wave _currentWaveData;

        private int _killedZombies = 0;
        
        private readonly Subject<int> justKillOneZombie = new Subject<int>();

        public void InitData(IBattlePageViewModel viewModel, ISubject<int> selectWeaponIndex)
        {
            timePlayedText.text = "00:00:00";
            killedZombiesText.text = "0";
            
            _selectWeaponIndex = selectWeaponIndex;

            justKillOneZombie.AsObservable().Subscribe(_ =>
            {
                _killedZombies++;
                killedZombies.OnNext(_killedZombies);
            }).AddTo(this);

            viewModel.EnableGrenadeButtonObservable.Subscribe(grenade =>
            {
                _throwSuppliesButtonView.gameObject.SetActive(grenade != null);
            }).AddTo(this);
            
            viewModel.NextWaveObservable.Subscribe(newWaveData =>
            {
                _gameObjetcNewWaveUI.SetActive(false);
                currentSecondCounter = 0;
                _killedZombies = 0;
                timePlayed.OnNext(currentSecondCounter);
                killedZombies.OnNext(_killedZombies);
                _currentWaveData = newWaveData;
                
                totalZombiesText.text = _currentWaveData.NumberZombiesOfWave.ToString();
            }).AddTo(this);
            
            Initialize(viewModel);
            
            timePlayed.AsObservable().Subscribe(num =>
            {
                var span = TimeSpan.FromSeconds(num);
                string displayString = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                timePlayedText.text = displayString;
            }).AddTo(this);
            
            killedZombies.AsObservable().Subscribe(num =>
            {
                killedZombiesText.text = num.ToString();
                if (num >= _currentWaveData.NumberZombiesOfWave)
                {
                    Time.timeScale = 0f;
                    _gameObjetcNewWaveUI.SetActive(true);
                    StartCoroutine(viewModel.NextWave(currentWave, _characterUnitStatus.Character.UnitStatus.CurrentHPFloat));
                }
            }).AddTo(this);

            
        }
        
        private int currentSecondCounter = 0;
        public void Initialize(IBattlePageViewModel viewModel)
        {
            var waves = viewModel.Waves.ToList();
            var unit = viewModel.Unit.Value;
            _currentWave = waves.First();
            var data = new BattleInitializeData(unit, _currentWave, unit.Weapons.ToList());
            _characterUnitStatus.InitCharacterData(data.Player , data.Weapons, _selectWeaponIndex);

            viewModel.Weapons.ObserveCountChanged().Subscribe(_ =>
            {
                _characterUnitStatus.SetNewListWeapon(viewModel.Weapons.ToList());
            }).AddTo(this);
           
            _characterUnitStatus.ShowRetryUI.Subscribe(_ =>
            {
                StartCoroutine(viewModel.LoseWave(currentWave, 0));
                _gameObjetcManagerPool.transform.DestroyChildren();
                _uIRetry.SetActive(true);
            }).AddTo(this);

            _currentWaveData = data.Wave;
            totalZombiesText.text = data.Wave.NumberZombiesOfWave.ToString();
            
            Observable.Interval(new TimeSpan(0, 0, 1)).TakeUntilDisable(this).Where(d=>((int)Time.timeScale) == 1).Subscribe(_ =>
            {
                Wave wave = _currentWaveData;
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
                            var enemyController = enemyObject.GetComponent<EnemyController>();
                            
                            enemyController.InitData(_characterUnitStatus.transform, justKillOneZombie);
                            enemyController.GetEnemyStatus().SetBaseUnitStatus(zombie.HP, zombie.Attack,
                                zombie.Speed, zombie.ResourceID, null, null, zombie.GoldDropCount);
                        }
                    }
                }
                currentSecondCounter++;
                
                timePlayed.OnNext(currentSecondCounter);
            });

        }
    }
}

