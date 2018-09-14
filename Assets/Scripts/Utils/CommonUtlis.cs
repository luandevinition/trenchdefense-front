using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class CommonUtlis
    {

        public static void LoadingAndChangeScene(string sceneName)
        {
            Observable.FromCoroutine(_=>LoadYourAsyncScene(sceneName))
                .Subscribe(
                    _ => Debug.Log("Complete Load Scene"),
                    () => Debug.Log("OnCompleted")
                );
        }
        static IEnumerator LoadYourAsyncScene(string sceneName)
        {
            yield return new WaitForSeconds(3f);
            
            SceneManager.LoadScene(sceneName);

       
        }
    }
}