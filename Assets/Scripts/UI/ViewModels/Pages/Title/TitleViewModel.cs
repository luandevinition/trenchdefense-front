using Domain;
using Domain.User;
using UI.PageTransitions.Battle;
using UI.Scripts.Route;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.ViewModels.Pages.Title
{
	public class TitleViewModel : ITitleViewModel
	{
		public GameUser GameUserData { get; private set; }

		public TitleViewModel(GameUser gameUser)
		{
			GameUserData = gameUser;
		}
		
		public void OnClickReadyButton()
		{
			PageRouter.Instance.DoTransition<BattleTransition>();
		}
	}
}
