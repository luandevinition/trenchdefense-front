using System;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;
using BattleStage.Domain;
using EazyTools.SoundManager;
using EZ_Pooling;
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

		public BaseUnitStatus GetEnemyStatus()
		{
			return _enemyStatus;
		}
		
		[SerializeField]
		private Damage _weaponDamage;
		
		[SerializeField]
		public Animator Animator;
		
		[SerializeField]
		private AudioClip _audioThrowPosion;
		
		private Transform _followedTarget;
		
		private Vector3 scaleVector3 = Vector3.one;

		[SerializeField]
		private BoxCollider _boxsuleCollider;
		
		[SerializeField]
		private CapsuleCollider _capsuleCollider;

		private bool _isReachedToTarget;

		private float _rangeAttack = 300f;

		private ISubject<EnemyController> _justKillOneZombie;

		public Zombie ZombieData;

		public Item ItemWillDrop;

		public void InitData(Transform followedTarget, ISubject<EnemyController> justKillOneZombie, Zombie zombie)
		{
			_followedTarget = followedTarget;

			ZombieData = zombie;
			
			_enemyStatus.SetBaseUnitStatus(zombie.HP, zombie.Attack,
				zombie.Speed, zombie.ResourceID, null, null, zombie.GoldDropCount);

			_rangeAttack = UnityEngine.Random.Range(40, 50);
			_justKillOneZombie = justKillOneZombie;
				 
			Observable.Interval(new TimeSpan(0, 0, 2)).Where(_ => _isReachedToTarget).Subscribe(_ =>
			{
				if(_enemyStatus.IsDie.Value) return;

				if(MyData.MyGameUser.GameSetting.EnableSFX)
					return;
				
				GetComponent<AudioSource>().PlayOneShot(_audioThrowPosion, MyData.MyGameUser.GameSetting.VolumeFloatValue*0.5f);
			
			}).AddTo(this);

			ItemWillDrop = zombie.GetDropItem();
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
				if (_weaponDamage != null)
				{
					_weaponDamage.DamageValue = _enemyStatus.Attack;
				}
				Animator.SetTrigger(Time.frameCount % 2 == 0 ? "Slash" : "Jab"); // Play animation randomly
				Animator.speed = 0.2f;
			}
			
		}
		
		public void OnTriggerEnter(Collider other)
		{
			if(!other.tag.Equals("Bullet"))
				return;

			var damgeComponent = other.gameObject.GetComponent<Damage>();
			if (damgeComponent == null)
			{
				return;
			}
			
			if(_enemyStatus.IsDie.Value || damgeComponent.IsEnemyDamage)
				return;
			
			var damgeValue = damgeComponent.DamageValue;
			_enemyStatus.GetDamage(other.gameObject.transform.position, damgeValue, Vector3.zero);
			
			if (_enemyStatus.IsDie.Value)
			{
				_justKillOneZombie.OnNext(this);
				
			    Animator.speed = 1f;
				Animator.SetBool("Run", false);
				Animator.SetBool("DieFront",true);
				_capsuleCollider.enabled = false;
				DestroyObject(this.gameObject,3f);
			}
		}
		
	}
}
