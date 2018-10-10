using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Data;
using Assets.HeroEditor.Common.EditorScripts;
using BattleStage.Domain;
using EZ_Pooling;
using HeroEditor.Common;
using HeroEditor.Common.Enums;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unit = BattleStage.Domain.Unit;
using CharacterHM = Assets.HeroEditor.Common.CharacterScripts.Character;

namespace BattleStage.Controller.Character
{
    public class CharacterController : MonoBehaviour
    {
        private const float MAX_HORIZONTAL_VALUE = 5600f;
        private const float MAX_VERTICAL_VALUE = 3100f;
        
        public Animator Animator;
		
        public float MovingSpeed
        {
            get { return Character.UnitStatus.Speed; }
        }
	    
        [SerializeField]
        private Joystick _joystick;
        public Joystick Joystick
        {
            get { return _joystick; }
        }
        
        [SerializeField]
        private Joystick _joystickWeapon;
        public Joystick JoystickWeapon
        {
            get { return _joystickWeapon; }
        }

        [SerializeField]
        private Image _hpImage;

        [SerializeField]
        public SpriteCollection SpriteCollection;
        
        public FirearmCollection FirearmCollection;
        
        public CharacterHM Character;
        
        public Camera CurrentCamera;
        
        /// <summary>
        /// Local value for moving
        /// </summary>
        private Vector3 moveVector = Vector3.one;
        private Vector3 playerPosition = Vector3.one;

        public UniRx.IObservable<UniRx.Unit> ShowRetryUI
        {
            get { return _showRetryUI.AsObservable(); }    
        }

        private readonly Subject<UniRx.Unit> _showRetryUI = new Subject<UniRx.Unit>();


        private bool _isInit;
        private List<Weapon> _weapons;

        public void SetNewListWeapon(List<Weapon> weapons)
        {
            _weapons = weapons;
            Character.UnitStatus.SetGranade(weapons.FirstOrDefault(d=>d.ThrowAble));
            
        }

        public void InitCharacterData(Unit unit, List<Weapon> weapons, ISubject<int> indexOfButtonClick)
        {
            CurrentCamera = GameObject.Find("BattleCameraFollow").GetComponent<Camera>();
            _weapons = weapons;
            _joystick.SetCamera(CurrentCamera);
            _joystickWeapon.SetCamera(CurrentCamera);
            var weapon = weapons.FirstOrDefault(d => d.ID == unit.BaseWeaponID);
            if (weapon == null)
            {
                Debug.LogError("Problem of Data Weapon !");
                return;
            }
            Character.UnitStatus.SetWeapon(weapon);
            SetFirearmParams(weapon.Name,  weapon.ShootSpeed, weapon.MagCapacity);
            EquipFirearms(weapon.Name, weapon.Collection);
            
            Weapon granade = weapons.FirstOrDefault(d=>d.ThrowAble);
            Character.UnitStatus.SetBaseUnitStatus(unit.HP, unit.Attack, unit.Speed, unit.ResourceID , weapon, granade);
            Character.UnitStatus.CurrentHP.Subscribe(hpValue =>
            {
                _hpImage.fillAmount = hpValue/Character.UnitStatus.HP;
            }).AddTo(this);

            Character.UnitStatus.IsDie.Subscribe(isDie =>
            {
                if (isDie)
                {
                    Time.timeScale = 0f;
                    _showRetryUI.OnNext(UniRx.Unit.Default);
                }
            }).AddTo(this);

            indexOfButtonClick.Subscribe(currentButtonClick =>
            {
                var weaponChange = _weapons.FirstOrDefault(d=>d.ID.Value == currentButtonClick);
                if (weaponChange == null)
                {
                    Debug.LogError("Problem of Data Weapon !");
                    return;
                }
                
                Character.UnitStatus.SetWeapon(weaponChange);
                SetFirearmParams(weaponChange.Name, weaponChange.ShootSpeed, weaponChange.MagCapacity);
                EquipFirearms(weaponChange.Name, weaponChange.Collection);
            }).AddTo(this);
            
            _isInit = true;
        }
        
        public void Update () 
        {
            if(_joystick == null || Character.UnitStatus == null || !_isInit)
                return;

            if (Character.UnitStatus.IsDie.Value)
                return;
            
            playerPosition = transform.position;

            //Flip Character
            transform.localScale = new Vector3(_joystickWeapon.Horizontal >= 0 ? 1 : -1, 1, 1);
        
            // Play animation run when joystick pressing
            Animator.SetBool("Run", _joystick.Horizontal != 0 || _joystick.Vertical != 0);

            moveVector.x = Mathf.Clamp(playerPosition.x + (_joystick.Horizontal * MovingSpeed * Time.deltaTime), -MAX_HORIZONTAL_VALUE, MAX_HORIZONTAL_VALUE);
            moveVector.y = Mathf.Clamp(playerPosition.y + (_joystick.Vertical * MovingSpeed * Time.deltaTime), -MAX_VERTICAL_VALUE, MAX_VERTICAL_VALUE);
            moveVector.z = 0;
            
            // For Moving the character
            transform.position = moveVector;
        }


        private bool _canGetDamageByHit = true;
        private Coroutine getHitCoroutine;
        
        public void OnTriggerEnter(Collider other)
        {
            float damgeValue = 0;
            if (other.tag.Equals("Weapon"))
            {
                var damgeComponent = other.gameObject.GetComponent<Damage>();
                if(Character.UnitStatus.IsDie.Value || !damgeComponent.IsEnemyDamage)
                    return;
                damgeValue = damgeComponent.DamageValue;
            }

            if (other.tag.Equals("Enemy") && _canGetDamageByHit)
            {
                _canGetDamageByHit = false;
                if(getHitCoroutine != null)
                    StopCoroutine(getHitCoroutine);
                StartCoroutine(DelayForHit());
                damgeValue = other.gameObject.GetComponent<BaseUnitStatus>().Attack;
                if(Character.UnitStatus.IsDie.Value)
                    return;
            }
          
            Character.UnitStatus.GetDamage(transform.position, damgeValue, new Vector3(0,45,0));
            
            if (Character.UnitStatus.IsDie.Value)
            {
                Animator.SetBool("DieFront",true);
                Animator.speed = 1f;
            }
        }

        IEnumerator DelayForHit()
        {
            yield return new WaitForSeconds(0.8f);
            _canGetDamageByHit = true;
        }
        
        protected void SetFirearmParams(string weaponName, int speed, int capicity)
        {
            if (FirearmCollection.Firearms.Count(i => i.Name == weaponName) != 1) throw new Exception("Please check firearms params for: " + weaponName);

            var weaponParams = FirearmCollection.Firearms.Single(i => i.Name == weaponName);
            Debug.Log("Weapon name : " + weaponParams.Name);
            
            
            ((CharacterHM) Character).Firearm.Params = FirearmCollection.Firearms.Single(i => i.Name == weaponName);

            ((CharacterHM) Character).Firearm.Params.FireRateInMinute = speed;
            ((CharacterHM) Character).Firearm.Params.MagazineCapacity = capicity;
            
        }
        
        public void EquipFirearms(string sname, string collection)
        {
            Character.Firearms = SpriteCollection.Firearms2H.Single(i => i.Name == sname && i.Collection == collection).Sprites;
            Character.Initialize();
        }
        
        public void EquipMeleeWeapon1H(string sname, string collection)
        {
            Character.PrimaryMeleeWeapon = SpriteCollection.MeleeWeapon1H.Single(i => i.Name == sname && i.Collection == collection).Sprite;
            Character.WeaponType = WeaponType.Melee1H;
            Character.Initialize();
        }
        
        public void EquipShield(string sname, string collection)
        {
            Character.Shield = SpriteCollection.Shield.Single(i => i.Name == sname && i.Collection == collection).Sprite;
            Character.Initialize();
        }

        public void EquipBow(string sname, string collection)
        {
            Character.Bow = SpriteCollection.Bow.Single(i => i.Name == sname && i.Collection == collection).Sprites;
            Character.Initialize();
        }

        public void EquipArmor(string sname, string collection)
        {
            Character.Armor = SpriteCollection.Armor.Single(i => i.Name == sname && i.Collection == collection).Sprites;
            Character.Initialize();
        }

        public void EquipHelmet(string sname, string collection)
        {
            Character.Helmet = SpriteCollection.Helmet.Single(i => i.Name == sname && i.Collection == collection).Sprite;
            Character.HairMaskType = HairMaskType.HelmetMask; // None, HelmetMask, HeadMask
            Character.HelmetMask = SpriteCollection.HelmetMask.Single(i => i.Name == sname && i.Collection == collection).Sprite;
            Character.Initialize();
        }
        
    }
}