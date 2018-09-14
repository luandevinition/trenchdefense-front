using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

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
		SceneManager.LoadScene(0);

	}
	
	
}
