﻿using System.Collections;
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
        
        private int currentPage = 1;
        
        public BattlePageViewModel(List<Wave> waves, Unit unit)
        {
            _waves=new ReactiveCollection<Wave>(waves);
            _unit = new ReactiveProperty<Unit>(unit);
            _weapons = new ReactiveCollection<Weapon>(unit.Weapons);
        }

        public void AddMoreWeapon(List<Weapon> listNewWeapon)
        {
            foreach (var wp in listNewWeapon)
            {
                if (Weapons.All(d => d.ID != wp.ID))
                {
                    Weapons.Add(wp);
                }
            }
        }

        public IEnumerator NextWave(int currentWave)
        {
            int nextWave = currentWave++;

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
            
            yield return WavesComponents.Instance.EndWave(currentWave,0,0).StartAsCoroutine(unit =>
            {
                
            },  ex =>
            {
                Debug.LogError("Can't End Waves " + currentWave + " with exception " +  ex.ToString());
            });
            
            yield return WavesComponents.Instance.BeginWave(nextWave).StartAsCoroutine(unitNew =>
            {
                _unit.Value = unitNew;
                AddMoreWeapon(unitNew.Weapons.ToList());
                _nextWaveObservable.OnNext(_waves[nextWave]);
            },  ex =>
            {
                Debug.LogError("Can't Begin Waves " + nextWave + " with exception " + ex.ToString());
            });
        }
    }
}