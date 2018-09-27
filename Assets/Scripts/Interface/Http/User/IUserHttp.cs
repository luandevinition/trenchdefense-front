using Domain.User;
using UniRx;

namespace Interface.Http.User
{
    public interface IUserHttp
    {
        IObservable<BaseUserData> CreateGameUser(string imeiString);

        IObservable<GameUser> GetGameUserData();
    }
}