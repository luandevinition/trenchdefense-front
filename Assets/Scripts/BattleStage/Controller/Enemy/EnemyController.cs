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

		[SerializeField]
		private AudioClip _audioThrowPosion;
		
		private Transform _followedTarget;
		
		private Vector3 scaleVector3 = Vector3.one;

		[SerializeField]
		private CapsuleCollider _capsuleCollider;

		private bool _isReachedToTarget;

		private float _rangeAttack = 300f;
		
		void Start()
		{
			_followedTarget = GameObject.FindGameObjectWithTag(TagFacade.PLAYER_TAG).transform;

			_rangeAttack = UnityEngine.Random.Range(300, 500);
				 
			Observable.Interval(new TimeSpan(0, 0, 2)).Where(_ => _isReachedToTarget).Subscribe(_ =>
			{
				if(_enemyStatus.IsDie.Value) return;
				
				var pos = gameObject.transform.position;
				pos.y += 150;
				var bullet = Instantiate(_bulletGameObject,pos,Quaternion.identity) as GameObject;
				bullet.GetComponent<Damage>().DamageValue = _enemyStatus.Attack;
				
				var heading = _followedTarget.position - transform.position;
				var distance = heading.magnitude;
				var directionForBullet = heading / distance; // This is now the normalized direction.
				bullet.GetComponent<MovingController>().Dicrection = directionForBullet;
				
				GetComponent<AudioSource>().PlayOneShot(_audioThrowPosion, 0.5f);
			}).AddTo(this);
		}
		
		void Update()
		{
			if(_enemyStatus == null || _enemyStatus.IsDie.Value ) return;
			
			var direction = (_followedTarget.position - transform.position).normalized;

			var offset = _enemyStatus.Speed * direction * Time.deltaTime;

			var newPosition = transform.position + offset;
			newPosition.z = 0;
			var targetPosition = _followedTarget.position;

			_isReachedToTarget = Vector2.Distance(targetPosition, newPosition) <= _rangeAttack;
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
			    Animator.speed = 1f;
				Animator.SetBool("Run", false);
				Animator.SetBool("DieFront",true);
				_capsuleCollider.enabled = false;
				DestroyObject(this.gameObject,3f);
			}
		}
		
	}
}
