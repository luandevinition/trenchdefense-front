using Components.Communication;
using Domain.User;
using Infrastructures.Http.User;
using Interface.Http.User;
using Interface.Repository.User;
using UniRx;

namespace Infrastructures.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private static UserRepository _instance;

        public static UserRepository GetInstance()
        {
            if (_instance != null) return _instance;

            _instance = new UserRepository(new UserHttp(ApiClient.GetInstance()));
            return _instance;
        }

        private readonly IUserHttp _userHttp;

        private UserRepository(UserHttp userHttp)
        {
            _userHttp = userHttp;
        }


        public IObservable<BaseUserData> CreateGameUser(string imeiString)
        {
            return _userHttp.CreateGameUser(imeiString);
        }
        
        public IObservable<GameUser> GetGameUserData()
        {
            return _userHttp.GetGameUserData();
        }

        public IObservable<bool> UpdateGameSetting(GameUser gameUser)
        {
            return _userHttp.UpdateGameSetting(gameUser);
        }
    }
}