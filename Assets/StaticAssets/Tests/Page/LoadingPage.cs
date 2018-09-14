using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StaticAssets.Tests.Page
{
	public class LoadingPage : MonoBehaviour {

		// Use this for initialization
		void Start ()
		{
			Time.timeScale = 1f;
			StartCoroutine(LoadYourAsyncScene());
		}
	
	
		static IEnumerator LoadYourAsyncScene()
		{
			yield return new WaitForSeconds(3f);
			SceneManager.LoadScene(1);

		}
	
	
	}
}
