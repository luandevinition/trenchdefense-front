using System.Collections;
using EZ_Pooling;
using UnityEngine;

namespace Utils
{
	public class DestroyAfterSecond : MonoBehaviour
	{
		public float SecondsForDestroy = 1f;
	
		void OnSpawned()
		{
			StartCoroutine(DelayBeforeDespawn());
		}
		

		IEnumerator DelayBeforeDespawn()
		{
			yield return new WaitForSeconds(SecondsForDestroy);
			EZ_PoolManager.Despawn(gameObject.transform);
		}
	}
}
