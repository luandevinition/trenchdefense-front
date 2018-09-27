using System;
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
		
        [SerializeField]
        private BaseUnitStatus _playerUnitStatus;

        public float MovingSpeed
        {
            get { return _playerUnitStatus.Speed; }
        }
	    
        [SerializeField]
        private Joystick _joystick;
        public Joystick Joystick
        {
            get { return _joystick; }
        }

        [SerializeField]
        private Image _hpImage;

        [SerializeField]
        public SpriteCollection SpriteCollection;

        public FirearmCollection FirearmCollection;
        
        
        private CharacterHM Character;
        
        public Vector3 ClickPosition = Vector3.zero;

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


        private float _offsetValue = 200f;
        private float _offsetratio = 0.225f;

        private bool _isInit = false;
        
        public void InitCharacterData(Unit unit, List<Weapon> weapons)
        {
            CurrentCamera = GameObject.Find("BattleCameraFollow").GetComponent<Camera>();
            
            var weapon = weapons.FirstOrDefault(d => d.ID == unit.BaseWeaponID);
            Weapon granade = null;
            if(unit.BaseGranedaID != null)
                granade = weapons.FirstOrDefault(d => d.ID == unit.BaseGranedaID);
            _playerUnitStatus.SetBaseUnitStatus(unit.HP, unit.Attack, unit.Speed, unit.ResourceID , weapon, granade);
            _playerUnitStatus.CurrentHP.Subscribe(hpValue =>
            {
                _hpImage.fillAmount = hpValue/_playerUnitStatus.HP;
            }).AddTo(this);

            _offsetValue = Screen.height * 0.225f;
            Debug.Log("offset : " + _offsetValue);
            _playerUnitStatus.IsDie.Subscribe(isDie =>
            {
                if (isDie)
                {
                    Time.timeScale = 0f;
                    _showRetryUI.OnNext(UniRx.Unit.Default);
                }
            }).AddTo(this);
            _isInit = true;
        }
        
        public void Update () 
        {
            if(_joystick == null || _playerUnitStatus == null || !_isInit)
                return;

            if (_playerUnitStatus.IsDie.Value)
                return;
            
            playerPosition = transform.position;

            Touch[] myTouches = Input.touches;  
            
            if(myTouches.Length > 1 && myTouches.Last().position.y > _offsetValue)
                ClickPosition = CurrentCamera.ScreenToViewportPoint(myTouches.Last().position);
            
            //Flip Character
            transform.localScale = new Vector3(ClickPosition.x >= 0.5 ? 1 : -1, 1, 1);
        
            // Play animation run when joystick pressing
            Animator.SetBool("Run", _joystick.Horizontal != 0 || _joystick.Vertical != 0);

            moveVector.x = Mathf.Clamp(playerPosition.x + (_joystick.Horizontal * _playerUnitStatus.Speed * Time.deltaTime), -MAX_HORIZONTAL_VALUE, MAX_HORIZONTAL_VALUE);
            moveVector.y = Mathf.Clamp(playerPosition.y + (_joystick.Vertical * _playerUnitStatus.Speed * Time.deltaTime), -MAX_VERTICAL_VALUE, MAX_VERTICAL_VALUE);
            moveVector.z = 0;
            
            // For Moving the character
            transform.position = moveVector;
        }
		
       
        
        
        public void OnTriggerEnter(Collider other)
        {
            var damgeComponent = other.gameObject.GetComponent<Damage>();
            if(_playerUnitStatus.IsDie.Value || !damgeComponent.IsEnemyDamage)
                return;
            
            var damgeValue = damgeComponent.DamageValue;
            _playerUnitStatus.GetDamage(other.gameObject.transform.position, damgeValue);
            EZ_PoolManager.Despawn(other.transform);
            
            if (_playerUnitStatus.IsDie.Value)
            {
                Animator.SetBool("DieFront",true);
                Animator.speed = 1f;
            }
        }
        
        protected void SetFirearmParams(string weaponName)
        {
            if (FirearmCollection.Firearms.Count(i => i.Name == weaponName) != 1) throw new Exception("Please check firearms params for: " + weaponName);

            ((CharacterHM) Character).Firearm.Params = FirearmCollection.Firearms.Single(i => i.Name == weaponName);
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