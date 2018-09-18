using UnityEngine;

namespace Utils
{
	public class DestroyAfterSecond : MonoBehaviour
	{
		public float SecondsForDestroy = 1f;
	
		// Use this for initialization
		void Start () {
			DestroyObject(gameObject,SecondsForDestroy);	
		}
	
	}
}
