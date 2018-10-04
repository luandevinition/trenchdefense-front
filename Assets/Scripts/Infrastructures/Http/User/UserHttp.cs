using System.Collections.Generic;
using System.Linq;
using App.Proto;
using Domain;
using Domain.User;
using Infrastructures.Factories.User;
using Interface.Http.User;
using Program.Scripts.Components.Communication;
using ProtoBuf;
using UniRx;
using BattleStage.Domain;
using Unit = BattleStage.Domain.Unit;

namespace Infrastructures.Http.User
{
    public class UserHttp : IUserHttp
    {
        private const string API_CREATE_ACCESS_TOKEN = "/create-access-token";
		
        private const string API_GET_GAME_USER = "/user";
		
        private const string API_UPDATE_GAME_USER_SETTING = "/user/setting";
		
        private const string API_CURRENT_CHARACTER = "/characters/profile";
        
        private readonly IApiClient _apiClient;
	
        public UserHttp(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
  
        public IObservable<BaseUserData> CreateGameUser(string imeiString)
        {
            RequestAccessTokenParameter request = new RequestAccessTokenParameter
            {
                imei = imeiString
            };
            var models = new List<IExtensible> {request};
            
            return _apiClient.Post(API_CREATE_ACCESS_TOKEN,models)
                .Select(MakeUserDataByProtobufs)
                .First();
        }
        
        private BaseUserData MakeUserDataByProtobufs(IList<IExtensible> protobufs)
        {
            if (protobufs.Count < 1) {
                return null;
            }

            return protobufs.Cast<App.Proto.AccessCode>().Select(UserFactory.Make).FirstOrDefault();
        }
        
        public IObservable<GameUser> GetGameUserData()
        {
             return _apiClient.Get(API_GET_GAME_USER)
                .Select(MakeGameUserDataByProtobufs)
                .First();
        }
        
        private GameUser MakeGameUserDataByProtobufs(IList<IExtensible> protobufs)
        {
            if (protobufs.Count < 1) {
                return null;
            }

            return protobufs.Cast<App.Proto.User>().Select(UserFactory.Make).FirstOrDefault();
        }
        
        public IObservable<bool> UpdateGameSetting(GameUser gameUser)
        {
            UpdateSettingParameter request = new UpdateSettingParameter
            {
                name = gameUser.Name,
                sfx = gameUser.GameSetting.EnableSFX,
                bgm = gameUser.GameSetting.EnableBGM,
                volume = (uint) gameUser.GameSetting.VolumeValue
            };
            var models = new List<IExtensible> {request};
            
            return _apiClient.Post(API_UPDATE_GAME_USER_SETTING,models)
                .Select(_=>true)
                .First();
        }

        public IObservable<Unit> GetCurrentCharacter()
        {
            return _apiClient.Get(API_CURRENT_CHARACTER)
                .Select(MakeCharacterByProtobufs)
                .First();
        }
        
        private Unit MakeCharacterByProtobufs(IList<IExtensible> protobufs)
        {
            if (protobufs.Count < 1) {
                return null;
            }

            return protobufs.Cast<App.Proto.CharacterStatus>().Select(UserFactory.Make).FirstOrDefault();
        }
    }
}