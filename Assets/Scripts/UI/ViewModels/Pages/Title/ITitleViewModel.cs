using Domain.User;
using UniRx;
using UnityEngine;

namespace UI.ViewModels.Pages.Title
{
	public interface ITitleViewModel
	{
		GameUser GetGameUser();
		
		void OnClickReadyButton();
		
		void SaveGameSetting(GameUser newGameUser);

		IObservable<GameUser> OnCompleteSaveSetting();
		
	}
}
