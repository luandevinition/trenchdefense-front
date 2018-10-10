using System.Collections;
using EZ_Pooling;
using UnityEngine;

namespace Utils
{
	public class DestroyAfterSecond : MonoBehaviour
	{
		public float SecondsForDestroy = 1f;

		public int DefaultScale = 1;
	
		void OnSpawned()
		{
			StartCoroutine(DelayBeforeDespawn());
		}
		
		void OnDespawned()
		{
			transform.localScale = (Vector3.one * 50);
		}

		IEnumerator DelayBeforeDespawn()
		{
			yield return new WaitForSeconds(SecondsForDestroy);
			EZ_PoolManager.Despawn(gameObject.transform);
		}
	}
}
