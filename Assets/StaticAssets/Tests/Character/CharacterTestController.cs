using System.Collections;
using System.Collections.Generic;
using BattleStage.Controller;
using Facade;
using UnityEngine;

public class CharacterTestController : MonoBehaviour {

	// TODO : [Refractor] Should contain _speed in another domain such as CharacterStatus etc
	[SerializeField]
	private float _speed = 50f;

	[SerializeField]
	private AvatarController _avatar;
	
	private Vector3 _targetPosition = Vector3.zero;

	[SerializeField]
	private Vector3[] movingPositions;
	
	private int currentIndex = 0;
	void Start()
	{
		_targetPosition = movingPositions[currentIndex];
	}
		
	void Update()
	{
		
		var direction = (_targetPosition - transform.position).normalized;

		var offset = _speed * direction * Time.deltaTime;

		var newPosition = transform.position + offset;

		var isReachedToTarget =
			Vector3.Dot((_targetPosition - newPosition).normalized, (_targetPosition - transform.position).normalized) <= 0f;

		transform.position = isReachedToTarget ? _targetPosition : newPosition;
		if (isReachedToTarget)
		{
			currentIndex++;
			if (currentIndex % 2 == 0)
			{
				_speed = 600f;
			}
			else
			{
				_speed = 500f;
			}
			if (currentIndex >= movingPositions.Length)
			{
				currentIndex = 0;
			}
			_targetPosition = movingPositions[currentIndex];
		}
	}
}
