using System;
using Assets.HeroEditor.Common.CharacterScripts;
using BattleStage.Domain;
using Facade;
using UniRx;
using UnityEngine;
using Utils;

namespace BattleStage.Controller.Enemy
{
	public class EnemyController : MonoBehaviour
	{
		[SerializeField]
		private BaseUnitStatus _enemyStatus;
		
		[SerializeField]
		public Animator Animator;
		
		[SerializeField]
		private GameObject _bulletGameObject;
		
		private Transform _followedTarget;
		
		private Vector3 scaleVector3 = Vector3.one;

		private bool _isReachedToTarget;
		
		void Start()
		{
			_followedTarget = GameObject.FindGameObjectWithTag(TagFacade.PLAYER_TAG).transform;
			
			Observable.Interval(new TimeSpan(0, 0, 2, 500)).Where(_ => _isReachedToTarget).Subscribe(_ =>
			{
				var pos = gameObject.transform.position;
				pos.y += 20;
				var bullet = Instantiate(_bulletGameObject,pos,Quaternion.identity) as GameObject;
				bullet.GetComponent<Damage>().DamageValue = _enemyStatus.Attack;
				
				var heading = _followedTarget.position - transform.position;
				var distance = heading.magnitude;
				var directionForBullet = heading / distance; // This is now the normalized direction.
				Debug.Log("directionForBullet : " + directionForBullet);
				bullet.GetComponent<MovingController>().Dicrection = directionForBullet;
			}).AddTo(this);
		}
		
		void Update()
		{
			if(_enemyStatus == null || _enemyStatus.IsDie.Value ) return;
			
			var direction = (_followedTarget.position - transform.position).normalized;

			var offset = _enemyStatus.Speed * direction * Time.deltaTime;

			var newPosition = transform.position + offset;

			var targetPosition = _followedTarget.position;

			_isReachedToTarget = Vector2.Distance(targetPosition, newPosition) <= 300f;
			Animator.SetBool("Run", !_isReachedToTarget);
			
			//Flip Character
			scaleVector3.x = targetPosition.IsLeft(transform.position) ? 1 : -1;
			transform.localScale = scaleVector3;
			
			if (!_isReachedToTarget)
			{
				// Play animation run when joystick pressing
				transform.position = newPosition;
				Animator.speed = 1f;
			}
			else
			{
				Animator.SetTrigger(Time.frameCount % 2 == 0 ? "Slash" : "Jab"); // Play animation randomly
				Animator.speed = 0.3f;
			}

			
		}
		
		public void OnTriggerEnter(Collider other)
		{
			var damgeComponent = other.gameObject.GetComponent<Damage>();
			if(_enemyStatus.IsDie.Value || damgeComponent.IsEnemyDamage)
				return;
			
			var damgeVlaue = damgeComponent.DamageValue;
			_enemyStatus.GetDamage(damgeVlaue);
			
			if (_enemyStatus.IsDie.Value)
			{
				Animator.SetBool("DieFront",true);
				Animator.speed = 1f;
				DestroyObject(this.gameObject,3f);
			}
		}
		
	}
}
