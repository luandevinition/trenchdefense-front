using Assets.HeroEditor.Common.CharacterScripts;
using Facade;
using UnityEngine;

namespace BattleStage.Controller.Enemy
{
	public class EnemyController : MonoBehaviour
	{
		// TODO : [Refractor] Should contain _speed in another domain such as CharacterStatus etc
		[SerializeField]
		private float _speed = 50f;
		
		[SerializeField]
		public Animator Animator;
		
		[SerializeField]
		private AvatarController _avatar;
		
		private Transform _followedTarget;
		
		void Start()
		{
			_followedTarget = GameObject.FindGameObjectWithTag(TagFacade.PLAYER_TAG).transform;
		}
		
		void Update()
		{
			var direction = (_followedTarget.position - transform.position).normalized;

			var offset = _speed * direction * Time.deltaTime;

			var newPosition = transform.position + offset;

			var targetPosition = _followedTarget.position;

			var isReachedToTarget =
				Vector3.Dot((targetPosition - newPosition).normalized, (targetPosition - transform.position).normalized) <= 0f;

			// Play animation run when joystick pressing
			Animator.SetBool("Run", !isReachedToTarget);

			transform.position = isReachedToTarget ? targetPosition : newPosition;
		}
	}
}
