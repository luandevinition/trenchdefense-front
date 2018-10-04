using System;
using HeroEditor.Common.Enums;
using UI.Views.Parts.Buttons;
using UnityEngine;
using CustomCharacterController = BattleStage.Controller.Character.CharacterController;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// Rotates arms and passes input events to child components like FirearmFire and BowShooting.
    /// </summary>
    public class WeaponControls : MonoBehaviour
    {
        private readonly Vector3 DEFAULT_VECTOR_ANGLE = new Vector3(0, 0, 20f);
        private const float MAX_ANGLE_ALLOW = 100f;
        
        public Character Character;
        public Transform ArmL;
        public Transform ArmR;
        public KeyCode ReloadButton;

        [SerializeField]
        private FireButtonView _fireButtonView;

        [SerializeField]
        private CustomCharacterController _characterController;
        
        [SerializeField]
        private bool _isPlayer = false;

        public bool IsPlayer
        {
            get { return _isPlayer; }
        }
	    
        /// <summary>
        /// Lock for Dead
        /// </summary>
        private bool _locked;

        public void Update()
        {
            _locked = !Character.Animator.GetBool("Ready") || Character.Animator.GetInteger("Dead") > 0;

            if (_locked || !_isPlayer) return;
            
            switch (Character.WeaponType)
            {
                case WeaponType.Melee1H:
                case WeaponType.Melee2H:
                case WeaponType.MeleePaired:
                    if (_fireButtonView)
                    {
                        Character.Animator.SetTrigger(Time.frameCount % 2 == 0 ? "Slash" : "Jab"); // Play animation randomly
                    }
                    break;
                case WeaponType.Bow:
                    Character.BowShooting.ChargeButtonDown = _fireButtonView.IsButtonDown;
                    Character.BowShooting.ChargeButtonUp = !_fireButtonView.IsButtonDown;
                    break;
                case WeaponType.Firearms1H:
                case WeaponType.Firearms2H:
                    Character.Firearm.Fire.FireButtonDown = _fireButtonView.IsButtonDown;
                    Character.Firearm.Fire.FireButtonPressed = _fireButtonView.IsButtonDown;
                    Character.Firearm.Fire.FireButtonUp = !_fireButtonView.IsButtonDown;
                    Character.Firearm.Reload.ReloadButtonDown = Input.GetKeyDown(ReloadButton);
                    break;
	            case WeaponType.Supplies:
		            if (_fireButtonView.IsButtonDown)
		            {
			            Character.Animator.Play(Time.frameCount % 2 == 0 ? "UseSupply" : "ThrowSupply", 0); // Play animation randomly
		            }
		            break;
			}
        }

        /// <summary>
        /// Called each frame update, weapon to mouse rotation example.
        /// </summary>
        public void LateUpdate()
        {
            if (_locked || !_isPlayer) return;

            Transform arm;
            Transform weapon;

            switch (Character.WeaponType)
            {
                case WeaponType.Bow:
                    arm = ArmL;
                    weapon = Character.BowRenderers[3].transform;
                    break;
                case WeaponType.Firearms1H:
                case WeaponType.Firearms2H:
                    arm = ArmR;
                    weapon = Character.Firearm.FireTransform;
                    break;
                default:
                    return;
            }
            
            RotateArm(arm, weapon, _isPlayer ? _characterController.JoystickWeapon.Vertical : DEFAULT_VECTOR_ANGLE.z, 0, MAX_ANGLE_ALLOW);
        }

        //[SerializeField] private Vector2 testPosition = Vector2.down;

        /// <summary>
        /// Selected arm to position (world space) rotation, with limits.
        /// </summary>
        public void RotateArm(Transform arm, Transform weapon, float weight, float angleMin, float angleMax) // TODO: Very hard to understand logic
        {
            arm.transform.localEulerAngles = new Vector3(0, 0, Mathf.Clamp(weight * angleMax , 0 , MAX_ANGLE_ALLOW) );
        }
    }
}