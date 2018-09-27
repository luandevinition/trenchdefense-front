using System.Collections.Generic;
using Domain.Wave;
using UnityEngine;

namespace BattleStage.Domain
{
    public class BattleInitializeData
    {
        public List<Weapon> Weapons
        {
            get { return _weapons; }
        }

        private readonly List<Weapon> _weapons;

        public Wave Wave
        {
            get { return _wave; }
        }

        private readonly Wave _wave;

        private readonly Unit _player;

        public Unit Player
        {
            get { return _player; }
        }

        public BattleInitializeData(Unit unitSelectedData, Wave wave, List<Weapon> weapons)
        {
            _wave = wave;
            _player = unitSelectedData;
            _weapons = weapons;
        }
    }
}
