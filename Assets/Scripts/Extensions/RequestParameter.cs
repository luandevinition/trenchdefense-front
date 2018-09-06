using UnityEngine.Networking;

namespace Program.Scripts.Components.Communication
{
    /// <summary>
    /// UnityWebRequestを行うのに必要なパラメーター
    /// </summary>
    public class RequestParameter
    {
        /// <summary>
        /// TimeOutになるまでの秒数
        /// </summary>
        private const int TIME_OUT_SECOND = 60;

        /// <summary>
        /// URL
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// HTTP メソッド
        /// </summary>
        private readonly string _method;

        /// <summary>
        /// post / put / delete 時のデータ
        /// </summary>
        private readonly byte[] _payload;

        private RequestParameter(string url, string method, byte[] payload)
        {
            _url = url;
            _method = method;
            _payload = payload;
        }

        /// <summary>
        /// 保存データからUnityWebRequestを作成する
        /// </summary>
        /// <returns></returns>
        public UnityWebRequest CreateUnityWebRequest()
        {
            // MEMO: falseにしないとexpectヘッダが送られてしまう
            // 参考: https://docs.unity3d.com/jp/530/ScriptReference/Experimental.Networking.UnityWebRequest-useHttpContinue.html

            var dowload = new DownloadHandlerBuffer();
            var upload = new UploadHandlerRaw(_payload);
            var request = new UnityWebRequest(_url, _method, dowload, upload)
            {
                useHttpContinue = false,
                timeout = TIME_OUT_SECOND
            };

            return request;
        }


        public static RequestParameter Get(string url)
        {
            return new RequestParameter(url, UnityWebRequest.kHttpVerbGET, null);
        }

        public static RequestParameter Post(string url, byte[] payload)
        {
            return new RequestParameter(url, UnityWebRequest.kHttpVerbPOST, payload);
        }

        public static RequestParameter Put(string url, byte[] payload)
        {
            return new RequestParameter(url, UnityWebRequest.kHttpVerbPUT, payload);
        }

        public static RequestParameter Delete(string url)
        {
            return new RequestParameter(url, UnityWebRequest.kHttpVerbDELETE, null);
        }

        public static RequestParameter Delete(string url, byte[] payload)
        {
            return new RequestParameter(url, UnityWebRequest.kHttpVerbDELETE, payload);
        }
    }
}