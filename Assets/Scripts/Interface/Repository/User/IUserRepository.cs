using Domain.User;
using UniRx;

namespace Interface.Repository.User
{
    public interface IUserRepository
    {
        IObservable<BaseUserData> CreateGameUser(string devicesID);
    }
}