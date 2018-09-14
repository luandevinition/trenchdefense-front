using UniRx;
using UnityEngine;

namespace BattleStage.Domain
{
    public class BaseUnitStatus : MonoBehaviour
    {
        public float HP
        {
            get { return _baseHP  + (_levelHP * _percentHPByLevel * _baseHP); }
        }
        
        public float Attack
        {
            get { return (_baseAttack + (_weaponEquiped != null ? _weaponEquiped.Attack : 0)) * (1 + (_levelAttack * _percentAttackByLevel)); }
        }
        
        public float Speed
        {
            get { return _baseSpeed + (_levelSpeed * _percentSpeedByLevel * _baseSpeed) * Random.Range(0.85f, 1.15f); }
        }

        public int GoldDroupCount
        {
            get { return _goldDropCount; }
        }
        
        public ResourceID ResourceId { get; private set; }
       
        /// <summary>
        /// Base status for each character will diff not the same.
        /// </summary>
        private int _baseHP;
        private int _baseAttack;
        private int _baseSpeed;

        /// <summary>
        /// Level (This will using Point we get for 2 wave to increase it)
        /// </summary>
        private int _levelHP;
        private int _levelAttack;
        private int _levelSpeed;
        
        /// <summary>
        /// Percent increase by level
        /// </summary>
        private float _percentHPByLevel;
        private float _percentAttackByLevel;
        private float _percentSpeedByLevel;

        /// <summary>
        /// Just for Zombie
        /// </summary>
        private int _goldDropCount;

        private Weapon _weaponEquiped;
        
        private Weapon _grenadeEquiped;

        private ReactiveProperty<float> _currentHP;

        public IObservable<float> CurrentHP
        {
            get { return _currentHP.AsObservable(); }
        }
        
        
        public ReactiveProperty<bool> IsDie = new ReactiveProperty<bool>(false);
        
        /// <summary>
        /// Constructure
        /// </summary>
        /// <param name="baseHp"></param>
        /// <param name="baseAttack"></param>
        /// <param name="baseSpeed"></param>
        /// <param name="resourceId"></param>
        /// <param name="weapon"></param>
        /// <param name="granade"></param>
        /// <param name="levelHp"></param>
        /// <param name="levelAttack"></param>
        /// <param name="levelSpeed"></param>
        /// <param name="percentHpByLevel"></param>
        /// <param name="percentAttackByLevel"></param>
        /// <param name="percentSpeedByLevel"></param>
        /// <param name="goldDropCount"></param>
        public BaseUnitStatus(int baseHp, int baseAttack, int baseSpeed, ResourceID resourceId, Weapon weapon = null,
            Weapon granade = null, int goldDropCount = 0, int levelHp = 1, int levelAttack = 1, int levelSpeed = 1,
            float percentHpByLevel = 1, float percentAttackByLevel = 0, float percentSpeedByLevel = 0)
        {
            _baseHP = baseHp;
            _baseAttack = baseAttack;
            _baseSpeed = baseSpeed;

            ResourceId = resourceId;
            
            _levelHP = levelHp;
            _levelAttack = levelAttack;
            _levelSpeed = levelSpeed;
            _percentHPByLevel = percentHpByLevel;
            _percentAttackByLevel = percentAttackByLevel;
            _percentSpeedByLevel = percentSpeedByLevel;
            _goldDropCount = goldDropCount;
            
            _weaponEquiped = weapon;
            _grenadeEquiped = granade;
            
            _currentHP = new ReactiveProperty<float>(_baseHP  + (_levelHP * _percentHPByLevel * _baseHP));
        }

        public void SetBaseUnitStatus(BaseUnitStatus baseStatus)
        {
            _baseHP = baseStatus._baseHP;
            _baseAttack = baseStatus._baseAttack;
            _baseSpeed = baseStatus._baseSpeed;

            ResourceId = baseStatus.ResourceId;
            
            _levelHP = baseStatus._levelHP;
            _levelAttack = baseStatus._levelAttack;
            _levelSpeed = baseStatus._levelSpeed;
            _percentHPByLevel = baseStatus._percentHPByLevel;
            _percentAttackByLevel = baseStatus._percentAttackByLevel;
            _percentSpeedByLevel = baseStatus._percentSpeedByLevel;
            _goldDropCount = baseStatus._goldDropCount;
            
            _weaponEquiped = baseStatus._weaponEquiped;
            _grenadeEquiped = baseStatus._grenadeEquiped;
            
            _currentHP = new ReactiveProperty<float>(_baseHP  + (_levelHP * _percentHPByLevel * _baseHP));
        }

        public void GetDamage(float damage)
        {
            _currentHP.Value -= damage;
            if (_currentHP.Value <= 0)
            {
                _currentHP.Value = 0;
                IsDie.Value = true;
            }
        }
        
        public void SetWeapon(Weapon weapon)
        {
            _weaponEquiped = weapon;
        }
        
        public void SetGranade(Weapon granade)
        {
            _grenadeEquiped = granade;
        }

        public void IncreaseHPLevel()
        {
            _levelHP++;
        }
        
        public void IncreaseAttackLevel()
        {
            _levelAttack++;
        }
        
        public void IncreaseSpeedLevel()
        {
            _levelSpeed++;
        }
    }
}