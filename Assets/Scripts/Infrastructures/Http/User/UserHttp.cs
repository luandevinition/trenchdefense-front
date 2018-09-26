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

namespace Infrastructures.Http.User
{
    public class UserHttp : IUserHttp
    {
        private const string API_CREATE_ACCESS_TOKEN = "/create-access-token";
		
        private readonly IApiClient _apiClient;
	
        public UserHttp(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
  
        private BaseUserData MakeUserDataByProtobufs(IList<IExtensible> protobufs)
        {
            if (protobufs.Count < 1) {
                return null;
            }

            return protobufs.Cast<App.Proto.AccessCode>().Select(UserFactory.Make).FirstOrDefault();
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
    }
}