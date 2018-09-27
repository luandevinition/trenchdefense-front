using Domain.User;
using Infrastructures.Repository.User;
using Interface.Repository.User;
using UniRx;

namespace Components
{
    public class UserComponents
    {
        private static UserComponents _instance;

        public static UserComponents Instance
        {
            get
            {
                if (_instance != null) return _instance;                
                _instance = new UserComponents(UserRepository.GetInstance());

                return _instance;
            }
        }

        private readonly IUserRepository _repository;
        
        private UserComponents(IUserRepository repository)
        {
            _repository = repository;
        }

        public IObservable<BaseUserData> CreateGameUser(string imeiString)
        {
            return _repository.CreateGameUser(imeiString);
        }
        
        public IObservable<GameUser> GetGameUserData()
        {
            return _repository.GetGameUserData();
        }
    }
}