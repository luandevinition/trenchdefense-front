using UnityEngine;

namespace BattleStage.Controller.Character
{
    public class CharacterController : MonoBehaviour
    {
        public Animator Animator;
		
        [SerializeField]
        private float _moveSpeed;

        public float MovingSpeed
        {
            get { return _moveSpeed; }
        }
	    
        [SerializeField]
        private Joystick _joystick;
        public Joystick Joystick
        {
            get { return _joystick; }
        }

        /// <summary>
        /// Local value for moving
        /// </summary>
        private Vector3 moveVector = Vector3.one;
        private Vector3 playerPosition = Vector3.one;

        public void Update () 
        {
            if(_joystick == null)
                return;
            
            playerPosition = transform.position;
            
            //Flip Character
            transform.localScale = new Vector3(_joystick.Horizontal >= 0 ? 1 : -1, 1, 1);
        
            // Play animation run when joystick pressing
            Animator.SetBool("Run", _joystick.Horizontal != 0 || _joystick.Vertical != 0);

            moveVector.x = Mathf.Clamp(playerPosition.x + (_joystick.Horizontal * _moveSpeed * Time.deltaTime), -5500, 5500);
            moveVector.y = Mathf.Clamp(playerPosition.y + (_joystick.Vertical * _moveSpeed * Time.deltaTime), -3000, 3000);
            moveVector.z = 0;
            
            // For Moving the character
            transform.position = moveVector;

        }
		
    }
}