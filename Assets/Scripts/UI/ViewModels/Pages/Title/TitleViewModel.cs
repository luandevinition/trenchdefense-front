using Components;
using Domain;
using Domain.User;
using Facade;
using UI.PageTransitions.Battle;
using UI.Scripts.Route;
using UniRx;
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

		public GameUser GetGameUser()
		{
			return GameUserData;
		}

		public void OnClickReadyButton()
		{
			PageRouter.Instance.DoTransition<BattleTransition>();
		}

		public void SaveGameSetting(GameUser newGameUser)
		{
			UserComponents.Instance.UpdateGameSetting(newGameUser).StartAsCoroutine(result =>
			{
				if (result)
				{
					_onCompleteSaveSetting.OnNext(newGameUser);				
					MyData.MyGameUser = newGameUser;
				}
			},  ex =>
			{
				Debug.LogError("Can't update game setting");
			});
		}

		private readonly Subject<GameUser> _onCompleteSaveSetting = new Subject<GameUser>();
		
		public IObservable<GameUser> OnCompleteSaveSetting()
		{
			return _onCompleteSaveSetting.AsObservable();
		}
	}
}
