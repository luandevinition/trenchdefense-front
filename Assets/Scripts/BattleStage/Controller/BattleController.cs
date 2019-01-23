using System;
using System.Collections.Generic;
using System.Linq;
using BattleStage.Controller.Enemy;
using BattleStage.Domain;
using Domain.Wave;
using EazyTools.SoundManager;
using EZ_Pooling;
using Facade;
using UI.ViewModels.Pages.Battle;
using UI.Views.Parts;
using UI.Views.Parts.Buttons;
using UI.Views.SubPage;
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
        private Text[] _textsForZombieKilled;

        [SerializeField]
        private GameObject _gameObjetcManagerPool;
        
        [SerializeField]
        private GameObject _gameObjetcNewWaveUI;
        
        [SerializeField]
        private Text _newWaveReward1;
        
        [SerializeField]
        private Text _newWaveReward2;
        
        [SerializeField]
        private Text _newWaveReward3;
        
        [SerializeField]
        private NextWavePage _nextWavePage;
        
        [SerializeField]
        private FireButtonView _throwSuppliesButtonView;

        private int currentWave = 1;
        private Wave _currentWave;

        private ISubject<int> _selectWeaponIndex;

        public Text totalZombiesText;
        public Text killedZombiesText;

        public Text timePlayedText;

        public Text _ammoCountText;
        
        private int _totalGold = 0;

        [SerializeField]
        public ItemData[] ItemDatas;
        
        private readonly Subject<int> killedZombies = new Subject<int>();
        
        private readonly Subject<int> timePlayed = new Subject<int>();
        
        private Wave _currentWaveData;

        private int _killedZombies = 0;
        private int[] _killedZombiesCollect;
        
        private readonly Subject<EnemyController> justKillOneZombie = new Subject<EnemyController>();

        public void InitData(IBattlePageViewModel viewModel, ISubject<int> selectWeaponIndex)
        {
            timePlayedText.text = "00:00:00";
            killedZombiesText.text = "0";
            _ammoCountText.text = "0";
            _killedZombiesCollect = new []{0,0,0,0,0};
            _selectWeaponIndex = selectWeaponIndex;

            justKillOneZombie.AsObservable().Subscribe(enemy =>
            {
                var dropItemOfZombie = enemy.ItemWillDrop;
                if (dropItemOfZombie != null)
                {
                    var firstOrDefault = ItemDatas.FirstOrDefault(d => d.ItemType == dropItemOfZombie.Type);
                    if (firstOrDefault != null)
                    {
                        var dropItemPrefabs = firstOrDefault
                            .itemPrefabs;
                        var dropView = EZ_PoolManager.Spawn(dropItemPrefabs, enemy.transform.position, Quaternion.identity).GetComponent<DropItemView>();
                        dropView.Bind(dropItemOfZombie);
                    }
                }
                _totalGold += enemy.ZombieData.GoldDropCount;
                
                _killedZombies++;
                Debug.Log("Zombie ID : " + enemy.ZombieData.ID.Value);
                _killedZombiesCollect[(enemy.ZombieData.ID.Value - 1)]++;
                
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
                    _gameObjetcNewWaveUI.SetActive(true);
                    
                    _nextWavePage.SetSkillPoint(((_currentWaveData.WaveNumber + 1 ) % 2) == 0 ? 0 :1);
                    
                    _nextWavePage.OnClickButtonNextWave().Subscribe(_ =>
                    {
                        Time.timeScale = 0f;
                        StartCoroutine(viewModel.NextWave(currentWave, _characterUnitStatus.Character.UnitStatus.CurrentHPFloat));
                    }).AddTo(this);
                    
                    int numberForRw1 = _characterUnitStatus.Weapons.Count(d => d.Type == ItemType.AMMO308);
                    _newWaveReward1.text = "+" + numberForRw1;
                    _characterUnitStatus.AddItem(ItemType.AMMO308 , _characterUnitStatus.Weapons.Where(d=>d.Type == ItemType.AMMO308).Select(d=>d.MagCapacity).Sum());
                
                    int numberForRw2 = _characterUnitStatus.Weapons.Count(d => d.Type == ItemType.AMMO10MM);
                    _newWaveReward2.text = "+" + numberForRw2;
                    _characterUnitStatus.AddItem(ItemType.AMMO10MM , _characterUnitStatus.Weapons.Where(d=>d.Type == ItemType.AMMO10MM).Select(d=>d.MagCapacity).Sum());
              
                    int numberForRw3 = _characterUnitStatus.Weapons.Count(d => d.Type == ItemType.ROCKET);
                    _newWaveReward3.text = "+" + numberForRw3;
                    _characterUnitStatus.AddItem(ItemType.ROCKET , _characterUnitStatus.Weapons.Where(d=>d.Type == ItemType.ROCKET).Select(d=>d.MagCapacity).Sum());
              
                    
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
            
            List<Item> initItemsCollection = new List<Item>();
            initItemsCollection.Add(new Item(2,"AMMO308",ItemType.AMMO308, 20,"2"));
            initItemsCollection.Add(new Item(3,"AMMO10",ItemType.AMMO10MM, 60,"3"));
            initItemsCollection.Add(new Item(5,"ROCKET",ItemType.ROCKET, 3,"5"));
           
            _characterUnitStatus.AmountOfAmmoSubject.Subscribe(ammonCount =>
            {
                _ammoCountText.text = ammonCount.ToString();
            }).AddTo(this);
            
            _characterUnitStatus.InitCharacterData(data.Player , data.Weapons, _selectWeaponIndex,initItemsCollection);

            viewModel.Weapons.ObserveCountChanged().Subscribe(_ =>
            {
                _characterUnitStatus.SetNewListWeapon(viewModel.Weapons.ToList());
            }).AddTo(this);

            _characterUnitStatus.ShowRetryUI.Subscribe(_ =>
            {
                StartCoroutine(viewModel.LoseWave(currentWave, 0));
                _gameObjetcManagerPool.transform.DestroyChildren();
                
                for (int i = 0 ; i < _textsForZombieKilled.Length ; i++)
                {
                    var text = _textsForZombieKilled[i];
                    text.text = string.Format("{0:D6}", _killedZombiesCollect[i]);
                }
                
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
                            
                            enemyController.InitData(_characterUnitStatus.transform, justKillOneZombie, zombie);
                      
                        }
                    }
                }
                currentSecondCounter++;
                
                timePlayed.OnNext(currentSecondCounter);
            });

        }
    }

    [Serializable]
    public class ItemData
    {
        public ItemType ItemType;
        public Transform itemPrefabs;
    }
}

