using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.ViewModels.Pages.Title
{
	public class TitleViewModel : ITitleViewModel
	{
		public void OnClickReadyButton()
		{
			SceneManager.LoadScene("BattleScene");
		}
	}
}
