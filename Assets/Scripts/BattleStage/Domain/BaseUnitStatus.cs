using System.Collections;
using EZ_Pooling;
using UniRx;
using UnityEngine;

namespace BattleStage.Domain
{
    public class BaseUnitStatus : MonoBehaviour
    {
        public float HP
        {
            get { return _baseHP  *  (1 + (_levelHP * 10) / 100f); }
        }
        
        public float Attack
        {
            get { return (_baseAttack * (1 + (_levelAttack * 10) / 100f) + (_weaponEquiped != null ? _weaponEquiped.Attack : 0)); }
        }
        
        public float Speed
        {
            get { return (_baseSpeed  * (1 + (_levelSpeed * 10) / 100f) + _tempSpeed ); }
        }

        public int GoldDroupCount
        {
            get { return _goldDropCount; }
        }
        
        public ResourceID ResourceId { get; private set; }
       
        /// <summary>
        /// Base status for each character will diff not the same.
        /// </summary>
        [SerializeField]
        private int _baseHP;
        [SerializeField]
        private int _baseAttack;
        [SerializeField]
        private int _baseSpeed;

        /// <summary>
        /// Level (This will using Point we get for 2 wave to increase it)
        /// </summary>
        [SerializeField]
        private int _levelHP;
        [SerializeField]
        private int _levelAttack;
        [SerializeField]
        private int _levelSpeed;
        
        /// <summary>
        /// Percent increase by level
        /// </summary>
        [SerializeField]
        private float _percentHPByLevel;
        [SerializeField]
        private float _percentAttackByLevel;
        [SerializeField]
        private float _percentSpeedByLevel;

        /// <summary>
        /// Just for Zombie
        /// </summary>
        private int _goldDropCount;

        private Weapon _weaponEquiped;
        
        public Weapon WeaponEquiped
        {
            get { return _weaponEquiped; }
        }

        private Weapon _grenadeEquiped;
        
        public Weapon GrenadeEquiped
        {
            get { return _grenadeEquiped; }
        }

        private ReactiveProperty<float> _currentHP;
        
        public int CurrentHPFloat
        {
            get { return (int)_currentHP.Value; }
        }

        public IObservable<float> CurrentHP
        {
            get { return _currentHP.AsObservable(); }
        }

        private float _tempSpeed = 0;
        
        [SerializeField]
        private GameObject _bloodGameObjectAnimation;

        public ReactiveProperty<bool> IsDie;


        private Coroutine _speedCoroutine;
        
        public void SetBaseUnitStatus(int baseHp, int baseAttack, int baseSpeed, ResourceID resourceId, Weapon weapon = null,
            Weapon granade = null, int goldDropCount = 0, int levelHp = 0, int levelAttack = 0, int levelSpeed = 0,
            float percentHpByLevel = 0, float percentAttackByLevel = 0, float percentSpeedByLevel = 0)
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
            
            _currentHP = new ReactiveProperty<float>(_baseHP  *  (1 + (_levelHP * 10) / 100f));
            IsDie = new ReactiveProperty<bool>(false);
        }

        public void GetDamage(Vector3 position, float damage , Vector3 offset)
        {
            EZ_PoolManager.Spawn(_bloodGameObjectAnimation.transform, position + offset,
                Quaternion.identity);
            
            _currentHP.Value -= damage;
            if (_currentHP.Value <= 0)
            {
                _currentHP.Value = 0;
                IsDie.Value = true;
            }
        }
        
        public void Heal(float damage)
        {
            _currentHP.Value += damage;
            if (_currentHP.Value >= HP)
            {
                _currentHP.Value = HP;
            }
        }

        public void BuffSpeed(int speedValue, float time)
        {
            _tempSpeed = speedValue;
            if (_speedCoroutine != null)
            {
                StopCoroutine(_speedCoroutine);
            }
            _speedCoroutine = StartCoroutine(ResetSpeedithTime(time));
        }

        IEnumerator ResetSpeedithTime(float time)
        {
            yield return new WaitForSeconds(time);
            _tempSpeed = 0;
        }
        
        public void SetWeapon(Weapon weapon)
        {
            _weaponEquiped = weapon;
        }
        
        public void SetGranade(Weapon granade)
        {
            _grenadeEquiped = granade;
        }

        public void IncreaseHPLevel(int plus = 1)
        {
            _levelHP = plus;
        }
        
        public void IncreaseAttackLevel(int plus = 1)
        {
            _levelAttack = plus;
        }
        
        public void IncreaseSpeedLevel(int plus = 1)
        {
            _levelSpeed = plus;
        }
        
        public int GetHPLevel()
        {
            return _levelHP;
        }
        
        public int GetttackLevel()
        {
            return _levelAttack;
        }
        
        public int GetSpeedLevel()
        {
            return _levelSpeed;
        }
    }
}