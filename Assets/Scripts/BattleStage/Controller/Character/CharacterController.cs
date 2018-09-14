using System.Collections.Generic;
using BattleStage.Domain;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Unit = BattleStage.Domain.Unit;

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
        private Text _hpText;
        
        /// <summary>
        /// Local value for moving
        /// </summary>
        private Vector3 moveVector = Vector3.one;
        private Vector3 playerPosition = Vector3.one;

        public IObservable<UniRx.Unit> ShowRetryUI
        {
            get { return _showRetryUI.AsObservable(); }    
        }

        private readonly Subject<UniRx.Unit> _showRetryUI = new Subject<UniRx.Unit>();


        public void InitCharacterData(Unit unit, List<Weapon> weapons)
        {
            _playerUnitStatus.SetBaseUnitStatus(unit.ToPlayerStatus(weapons));
            _playerUnitStatus.CurrentHP.Subscribe(hpValue =>
            {
                _hpText.text = hpValue.ToString();
            }).AddTo(this);

            _playerUnitStatus.IsDie.Subscribe(isDie =>
            {
                if (isDie)
                {
                    Time.timeScale = 0f;
                    _showRetryUI.OnNext(UniRx.Unit.Default);
                }
            }).AddTo(this);
        }
        
        public void Update () 
        {
            if(_joystick == null || _playerUnitStatus == null || _playerUnitStatus.IsDie.Value)
                return;
            
            playerPosition = transform.position;
            
            //Flip Character
            transform.localScale = new Vector3(_joystick.Horizontal >= 0 ? 1 : -1, 1, 1);
        
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
            _playerUnitStatus.GetDamage(damgeValue);
			
            if (_playerUnitStatus.IsDie.Value)
            {
                Animator.SetBool("DieFront",true);
                Animator.speed = 1f;
                DestroyObject(other.gameObject);
            }
        }
        
        
    }
}