using System.Collections.Generic;
using System.Linq;
using BattleStage.Controller;
using Domain.Wave;
using UI.ViewModels.Pages.Battle;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pages.Battle
{
	public class BattlePageView : MonoBehaviour
	{
		[SerializeField]
		private BattleController BattleController;
		
		[SerializeField]
		private Button _pauseButton;
		
		[SerializeField]
		private GameObject _pauseSubPageGameObject;

		public void Bind(IBattlePageViewModel viewModel)
		{
			BattleController.InitData(viewModel.Waves.ToList());

			_pauseButton.OnClickAsObservable().Subscribe(_ =>
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}).AddTo(this);
		}
		
		void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus && ((int)Time.timeScale) != 0)
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}
		}
	}
}
