using Facade;
using UnityEngine;

namespace BattleStage.Controller.Enemy
{
	public class EnemyController : MonoBehaviour
	{
		[SerializeField]
		private float _speed = 50f;
		
		private Transform _target;
		
		void Start()
		{
			_target = GameObject.FindGameObjectWithTag(TagFacade.PLAYER_TAG).transform;
		}
		
		void Update()
		{
			var direction = (_target.position - transform.position).normalized;

			transform.position += _speed * direction * Time.deltaTime;
		}
	}
}
