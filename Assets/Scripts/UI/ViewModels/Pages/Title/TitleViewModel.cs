using System.Collections.Generic;
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

		public List<LeaderboardRecord> _listLeaderboard { get; private set; }
		
		public TitleViewModel(GameUser gameUser, List<LeaderboardRecord> listLeaderboard)
		{
			GameUserData = gameUser;
			_listLeaderboard = listLeaderboard;
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
					UserComponents.Instance.GetLeaderboard().StartAsCoroutine(listLeaderboard =>
					{
						_listLeaderboard = listLeaderboard;
						_onCompleteSaveSetting.OnNext(newGameUser);			
					},  ex =>
					{
						Debug.LogError("Can't get Leaderboard");
					});
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

		public List<LeaderboardRecord> ListLeaderboard()
		{
			return _listLeaderboard;
		}
	}
}
