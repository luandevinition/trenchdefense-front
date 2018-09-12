using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGameObjects : MonoBehaviour
{

	[SerializeField]
	private SwitchCondition _switchCondition;

	[SerializeField]
	private GameObject[] _switchGameObjects;
	
	// Use this for initialization
	void Start () {
		switch (_switchCondition)
		{
				case SwitchCondition.Screen:
					
					break;
				case SwitchCondition.Platform:
					var switchResultP = SystemInfo.deviceModel.Contains("iPad");
					_switchGameObjects[0].SetActive(!switchResultP);
					_switchGameObjects[1].SetActive(switchResultP);
					break;
				case SwitchCondition.ScreenRatio:
					float heightScreen = Screen.height;
					float widthScreen = Screen.width;
					float result = heightScreen / widthScreen;
					var switchResultR = result >=0.75f;
					_switchGameObjects[0].SetActive(!switchResultR);
					_switchGameObjects[1].SetActive(switchResultR);
				break;
				default:
					break;
		}
	}
	
	public enum SwitchCondition {
		Screen,
		ScreenRatio,
		Platform
	}

}
