using System.Collections.Generic;
using System.Net;
using ProtoBuf;
using UniRx;

namespace Program.Scripts.Components.Communication
{
    public interface IApiClient
    {
        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするURI</param>
        IObservable<List<IExtensible>> Get(string path, HttpStatusCode[] throughCodes = null);
        
        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするURI</param>
        IObservable<string> GetJson(string path);

        /// <summary>
        /// GETメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするURI</param>
        /// <param name="models">POST時に渡すモデルの配列</param>
        IObservable<List<IExtensible>> Get(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null);

        /// <summary>
        /// POSTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするURI</param>
        /// <param name="models">POST時に渡すモデルの配列</param>
        IObservable<List<IExtensible>> Post(string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null);

        /// <summary>
        /// POSTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        IObservable<List<IExtensible>> Post(string path, HttpStatusCode[] throughCodes = null);

        /// <summary>
        /// PUTメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">PUT時に渡すモデルの配列</param>
        /// <returns></returns>
        IObservable<List<IExtensible>> Put (string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null);
        
        /// <summary>
        /// PUTメソッドのでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        IObservable<List<IExtensible>> Put (string path, HttpStatusCode[] throughCodes = null);

        /// <summary>
        /// DELETEメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <returns></returns>
        IObservable<List<IExtensible>> Delete (string path, HttpStatusCode[] throughCodes = null);

        /// <summary>
        /// DELETEメソッドでリクエストする
        /// </summary>
        /// <param name="path">アクセスするpath</param>
        /// <param name="models">DELETE時に渡すモデル</param>
        /// <returns></returns>
        IObservable<List<IExtensible>> Delete (string path, List<IExtensible> models, HttpStatusCode[] throughCodes = null);
    }
}