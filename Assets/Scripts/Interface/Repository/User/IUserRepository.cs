using Domain.User;
using UniRx;
using BattleStage.Domain;
using Unit = BattleStage.Domain.Unit;

namespace Interface.Repository.User
{
    public interface IUserRepository
    {
        IObservable<BaseUserData> CreateGameUser(string devicesID);

        IObservable<GameUser> GetGameUserData();

        IObservable<bool> UpdateGameSetting(GameUser gameUser);

        IObservable<Unit> GetCurrentCharacter();
    }
}