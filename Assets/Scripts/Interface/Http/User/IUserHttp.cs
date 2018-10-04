using System.Collections.Generic;
using Domain.User;
using UniRx;
using Unit = BattleStage.Domain.Unit;

namespace Interface.Http.User
{
    public interface IUserHttp
    {
        IObservable<BaseUserData> CreateGameUser(string imeiString);

        IObservable<GameUser> GetGameUserData();

        IObservable<bool> UpdateGameSetting(GameUser gameUser);

        IObservable<Unit> GetCurrentCharacter();

        IObservable<List<LeaderboardRecord>> GetLeaderboard();
    }
}