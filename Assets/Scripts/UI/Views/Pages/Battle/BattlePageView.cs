using System.Collections.Generic;
using System.Linq;
using BattleStage.Controller;
using Domain.Wave;
using UI.ViewModels.Pages.Battle;
using UnityEngine;

namespace UI.Views.Pages.Battle
{
	public class BattlePageView : MonoBehaviour
	{
		[SerializeField]
		private BattleController BattleController;

		public void Bind(IBattlePageViewModel viewModel)
		{
			BattleController.InitData(viewModel.Waves.ToList());
		}
	}
}
