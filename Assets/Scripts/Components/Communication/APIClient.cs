using System;
using System.Collections.Generic;
using System.Net;
using Extensions;
using Program.Scripts.Components.Communication;
using ProtoBuf;
using StaticAssets.Configs;
using UniRx;
using UniRx.JsonWebRequest;
using UnityEngine;
using Utils;

namespace Components.Communication
{
    public class ApiClient : IApiClient
    {
        /// <summary>
        /// アクセストークンとか、デフォルトで付けたいヘッダー
        /// </summary>
        private static readonly IDictionary<string, string> _defaultRequestHeaders = new Dictionary<string, string>();

        /// <summary>
        /// バックエンドのベースURI
        /// </summary>
        private static string _baseUri = String.Empty;
        
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static IApiClient _instance;

        private static CompositeDisposable _requestDisposable = new CompositeDisposable();

        /// <summary>
        /// privateコンストラクタこのクラスはシングルトンですわよ
        /// </summary>
        private ApiClient(){}

        /// <summary>
        /// インスタンスを取得する
        /// </summary>
        /// <returns></returns>
        public static IApiClient GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new ApiClient();
            _baseUri = BaseURLFromScriptableObject();
            return _instance;
        }


        /// <summary>
        /// Get Setting Data from ScriptableObject in StaticAsset/Configs/Resources
        /// </summary>
        /// <returns></returns>
        private static string BaseURLFromScriptableObject()
        {
            return Resources.Load<SettingData>("SettingData").url;
        }

        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="throughCodes"></param>
        public UniRx.IObservable<List<IExtensible>> Get(string path, HttpStatusCode[] throughCodes = null)
        {
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.GetAndGetProtobufs(_baseUri + path, header));
        }                
        
        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        public UniRx.IObservable<string> GetJson(string path)
        {
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/json"}};
            return ObservableJsonWebRequest.Get(_baseUri + path, header);
        }

        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">POST時に渡すモデルの配列</param>
        /// <param name="throughCodes"></param>
        public UniRx.IObservable<List<IExtensible>> Get(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null)
        {
            var payload = ProtobufConverter.SerializeModels (models);
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.GetAndGetProtobufs(_baseUri + path + "?body=" + HEXUtil.HexStringFromBytes(payload), header));
        }

        /// <summary>
        /// POSTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">POST時に渡すモデルの配列</param>
        /// <param name="throughCodes"></param>
        public UniRx.IObservable<List<IExtensible>> Post(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null)
        {
            byte[] payload = null;
            if (models.Count > 0)
            {
                payload = ProtobufConverter.SerializeModels(models);
            }
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.PostAndGetProtobufs(_baseUri + path, payload, header));
        }

        /// <summary>
        /// POSTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="throughCodes"></param>
        public UniRx.IObservable<List<IExtensible>> Post(string path, HttpStatusCode[] throughCodes = null)
        {
            var models = new List<IExtensible>();
            return Post(path, models, throughCodes);
        }

        /// <summary>
        /// PUTメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">PUT時に渡すモデルの配列</param>
        /// <param name="throughCodes"></param>
        /// <returns></returns>
        public UniRx.IObservable<List<IExtensible>> Put(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null)
        {
            var payload = ProtobufConverter.SerializeModels (models);
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.PutAndGetProtobufs(_baseUri + path, payload, header));
        }

        /// <summary>
        /// POSTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="throughCodes"></param>
        public UniRx.IObservable<List<IExtensible>> Put(string path, HttpStatusCode[] throughCodes = null)
        {
            var models = new List<IExtensible>();
            return Put(path, models, throughCodes);
        }

        /// <summary>
        /// DELETEメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="throughCodes"></param>
        /// <returns></returns>
        public UniRx.IObservable<List<IExtensible>> Delete(string path, HttpStatusCode[] throughCodes = null)
        {
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.DeleteAndGetProtobufs(_baseUri + path, header));
        }

        /// <summary>
        /// DELETEメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">DELETE時に渡すモデル</param>
        /// <param name="throughCodes"></param>
        /// <returns></returns>
        public UniRx.IObservable<List<IExtensible>> Delete(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null)
        {
            var payload = ProtobufConverter.SerializeModels (models);
            var header = new Dictionary<string, string>(_defaultRequestHeaders) {{"Content-Type", "appliaction/octet-stream"}};
            return Wrap(ObservableProtobufWebRequest.DeleteAndGetProtobufs(_baseUri + path, payload, header));
        }

        /// <summary>
        /// 購読情報をこのクラスで管理するためのラップクラス
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private UniRx.IObservable<List<IExtensible>> Wrap(UniRx.IObservable<List<IExtensible>> source)
        {
            var subject = new Subject<List<IExtensible>>();

            var request = source.Subscribe(subject.OnNext, subject.OnError, subject.OnCompleted);

            _requestDisposable.Add(request);

            return subject;
        }

        /// <summary>
        /// Set Authorization To Header
        /// </summary>
        /// <param name="accessToken"></param>
        public static void SetAccessTokenToHeader(string accessToken)
        {
            RemoveAccessTokenToHeader();
            
            _defaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        }


        /// <summary>
        /// Check Authorization Exists or Not
        /// </summary>
        /// <returns></returns>
        public static bool IsExistAccessTokenToHeader()
        {
            return _defaultRequestHeaders.ContainsKey("Authorization");
        }
        
        /// <summary>
        /// Remove Authorization To Header
        /// </summary>
        public static void RemoveAccessTokenToHeader()
        { 
            if (_defaultRequestHeaders.ContainsKey("Authorization"))
            {
                _defaultRequestHeaders.Remove("Authorization");
            } 
        }

        /// <summary>
        /// Clear all pending request.
        /// </summary>
        public static void ClearPendingRequest()
        {
            _requestDisposable.Dispose();
            _requestDisposable = new CompositeDisposable();
        }

    }
}
