using Domain;
using Domain.User;
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
			SceneManager.LoadScene("BattleScene");
		}
	}
}
