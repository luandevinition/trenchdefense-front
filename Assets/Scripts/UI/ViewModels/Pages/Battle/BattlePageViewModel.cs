using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleStage.Domain;
using Components;
using Domain.Wave;
using UniRx;
using UnityEngine;
using Unit = BattleStage.Domain.Unit;

namespace UI.ViewModels.Pages.Battle
{
    public class BattlePageViewModel : IBattlePageViewModel
    {
        public IReactiveCollection<Wave> Waves
        {
            get { return _waves; }
        }
        
        private readonly IReactiveCollection<Wave> _waves;

        public IReactiveProperty<Unit> Unit
        {
            get { return _unit; }
        }

        private readonly IReactiveProperty<Unit> _unit;


        public IReactiveCollection<Weapon> Weapons
        {
            get { return _weapons; }
        }
        
        private readonly IReactiveCollection<Weapon> _weapons;


        public IObservable<Wave> NextWaveObservable
        {
            get { return _nextWaveObservable.AsObservable(); }
        }        

        private readonly Subject<Wave> _nextWaveObservable = new Subject<Wave>();
        
        
        public IObservable<Weapon> EnableGrenadeButtonObservable
        {
            get { return _enableGrenadeButtonObservable.AsObservable(); }
        }        

        private readonly Subject<Weapon> _enableGrenadeButtonObservable = new Subject<Weapon>();
        
        private int currentPage = 1;
        
        public BattlePageViewModel(List<Wave> waves, Unit unit)
        {
            _waves=new ReactiveCollection<Wave>(waves);
            _unit = new ReactiveProperty<Unit>(unit);
            _weapons = new ReactiveCollection<Weapon>(unit.Weapons);
            
            _enableGrenadeButtonObservable.OnNext(_weapons.FirstOrDefault(d=>d.ThrowAble));
        }

        public void AddMoreWeapon(List<Weapon> listNewWeapon)
        {
            foreach (var wp in listNewWeapon)
            {
                if (_weapons.All(d => d.ID != wp.ID))
                {
                    _weapons.Add(wp);
                }
            }
            
            _enableGrenadeButtonObservable.OnNext(_weapons.FirstOrDefault(d=>d.ThrowAble));
        }

        public IEnumerator LoseWave(int currentWave, int hp = 0)
        {
            yield return WavesComponents.Instance.EndWave(currentWave,hp,0).StartAsCoroutine(unit =>
            {
                Debug.LogWarning("Send Lose");
            },  ex =>
            {
                Debug.LogError("Can't End Waves " + currentWave + " with exception " +  ex.ToString());
            });
        }

        public IEnumerator NextWave(int currentWave, int hp)
        {
            int nextWave = (currentWave+1);

            if (nextWave > _waves.Count)
            {
                currentPage++;
                
                yield return WavesComponents.Instance.GetListWaves(currentPage).StartAsCoroutine(waveList =>
                {
                    foreach (var wave in waveList)
                    {
                        _waves.Add(wave);
                    }
                },  ex =>
                {
                    Debug.LogError("Can't get Waves Data " + ex.ToString());
                });
            }
            
            yield return WavesComponents.Instance.EndWave(currentWave,hp,0).StartAsCoroutine(unit =>
            {
                
            },  ex =>
            {
                Debug.LogError("Can't End Waves " + currentWave + " with exception " +  ex.ToString());
            });
            
            yield return WavesComponents.Instance.BeginWave(nextWave).StartAsCoroutine(unitNew =>
            {
                _unit.Value = unitNew;
                AddMoreWeapon(unitNew.Weapons.ToList());
            },  ex =>
            {
                Debug.LogError("Can't Begin Waves " + nextWave + " with exception " + ex.ToString());
            });

            Time.timeScale = 1f;
            yield return new WaitForSeconds(2f);
            
            _nextWaveObservable.OnNext(_waves[nextWave-1]);
        }
    }
}