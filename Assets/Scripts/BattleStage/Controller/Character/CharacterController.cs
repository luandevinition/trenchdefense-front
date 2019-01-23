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
using UI.Views.Parts;
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

        public Rigidbody Rigidbody;
        
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
        
        //Store Ammo , Grenade
        public List<Item> ItemsCollection;

        private readonly Subject<int> _amountOfAmmoSubject = new Subject<int>();

        public UniRx.IObservable<int> AmountOfAmmoSubject
        {
            get { return _amountOfAmmoSubject.AsObservable(); }    
        }
        
        public void SetNewListWeapon(List<Weapon> weapons)
        {
            _weapons = weapons;
            Character.UnitStatus.SetGranade(weapons.FirstOrDefault(d=>d.ThrowAble));
        }

        public List<Weapon> Weapons
        {
            get { return _weapons; }
        }

        public bool StillCanFire()
        {
            var weapon = Character.UnitStatus.WeaponEquiped;
            var ammoCount = ItemsCollection.First(d => (int) d.Type == (int) weapon.Type).Count;
            if (ammoCount > 0)
                return true;
            return false;
        }

        public void InitCharacterData(Unit unit, List<Weapon> weapons, ISubject<int> indexOfButtonClick, List<Item> initItemCollection)
        {
            CurrentCamera = GameObject.Find("BattleCameraFollow").GetComponent<Camera>();

            ItemsCollection = initItemCollection;
            
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
            
            var itemD = ItemsCollection.FirstOrDefault(d => (int) d.Type == (int) weapon.Type);
            if (itemD != null)
            {
                _amountOfAmmoSubject.OnNext(itemD.Count);
            }
            
            SetFirearmParams(weapon.Name,  weapon.ShootSpeed, weapon.MagCapacity);
            EquipFirearms(weapon.Name, weapon.Collection);
            
            Weapon granade = weapons.FirstOrDefault(d=>d.ThrowAble);
            Character.UnitStatus.SetBaseUnitStatus(unit.HP, unit.Attack, unit.Speed, unit.ResourceID , weapon, granade, 0, 0, 0, 0,50,20,10);
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
                var itemZ = ItemsCollection.FirstOrDefault(d => (int) d.Type == (int) weaponChange.Type);
                if (itemZ != null)
                {
                    _amountOfAmmoSubject.OnNext(itemZ.Count);
                }
                
                SetFirearmParams(weaponChange.Name, weaponChange.ShootSpeed, weaponChange.MagCapacity);
                EquipFirearms(weaponChange.Name, weaponChange.Collection);
            }).AddTo(this);


            Character.Firearm.Fire.AmountOfAmmoDecreaseSubject.Subscribe(type =>
            {
                var item = ItemsCollection.FirstOrDefault(d => d.Type == type);
                if (item != null)
                {
                    item.Count--;
                }
                _amountOfAmmoSubject.OnNext(ItemsCollection.First(d=>(int)d.Type == (int) type ).Count);
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

        public void AddItem(ItemType itemType, int count)
        {
            var itemKeep = ItemsCollection.FirstOrDefault(d => d.Type == itemType);
            if (itemKeep != null)
            {
                itemKeep.Count+= count;
                
                var weaponEquiped = Character.UnitStatus.WeaponEquiped;
                if (weaponEquiped.Type == itemType)
                {
                    _amountOfAmmoSubject.OnNext(itemKeep.Count);
                }
            }
        }
        
        public void OnTriggerEnter(Collider other)
        {

            if (other.tag.Equals("Item"))
            {
                var item = other.gameObject.GetComponent<DropItemView>().DropItemData;
                switch (item.Type)
                {
                    case ItemType.HP:
                        Character.UnitStatus.Heal(item.Count);
                        break;
                    case ItemType.SPEED:
                        Character.UnitStatus.BuffSpeed(item.Count,10);
                        break;
                    default :
                        var type = item.Type;
                            
                        var itemKeep = ItemsCollection.FirstOrDefault(d => d.Type == type);
                        if (itemKeep != null)
                        {
                            itemKeep.Count+= item.Count;
                        }
                        
                        var weaponEquiped = Character.UnitStatus.WeaponEquiped;
                        if (weaponEquiped.Type == type)
                        {
                            _amountOfAmmoSubject.OnNext(itemKeep.Count);
                        }
                     
                        break;
                }
                
                
                EZ_PoolManager.Despawn(other.transform);
                return;
            }
            
            float damgeValue = 0;
            if (other.tag.Equals("Weapon") && _canGetDamageByHit)
            {
                var damgeComponent = other.gameObject.GetComponent<Damage>();
                if(Character.UnitStatus.IsDie.Value || !damgeComponent.IsEnemyDamage)
                    return;
                damgeValue = damgeComponent.DamageValue;
                
                Character.UnitStatus.GetDamage(transform.position, damgeValue, new Vector3(0,45,0));
            
                _canGetDamageByHit = false;
                if(getHitCoroutine != null)
                    StopCoroutine(getHitCoroutine);
                StartCoroutine(DelayForHit());
                
                if (Character.UnitStatus.IsDie.Value)
                {
                    Animator.SetBool("DieFront",true);
                    Animator.speed = 1f;
                }
                
                return;
            }

            if (other.tag.Equals("Enemy") && _canGetDamageByHit)
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero; 
                
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