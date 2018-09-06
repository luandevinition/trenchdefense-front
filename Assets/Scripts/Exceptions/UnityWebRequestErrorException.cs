using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace Exceptions
{
    /// <summary>
    /// Unityのリクエストエラーのラッピングクラス
    /// </summary>
    public class UnityWebRequestErrorException : Exception
    {

        public string RawErrorMessage { get; private set; }
        public bool HasResponse { get; private set; }
        public string Text { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Dictionary<string, string> ResponseHeaders { get; private set; }
        public UnityWebRequest Request { get; private set; }

        // cache the text because if www was disposed, can't access it.
        public UnityWebRequestErrorException(UnityWebRequest request)
        {
            Request = request;
            RawErrorMessage = request.error;
            ResponseHeaders = request.GetResponseHeaders();
            HasResponse = false;
            StatusCode = (HttpStatusCode) request.responseCode;

            if (request.downloadHandler != null)
            {
                Text = request.downloadHandler.text;
            }

            var isprotbuf = false;
            if (request.GetResponseHeaders() != null && request.GetResponseHeaders().Count > 0 &&
                request.GetResponseHeaders().ContainsKey("Content-Type"))
            {
                isprotbuf = (request.GetResponseHeaders()["Content-Type"] == "application/octet-stream");
            }
            if (StatusCode != HttpStatusCode.ServiceUnavailable && isprotbuf)
            {
                Debug.LogError("UnityWebRequestErrorException Error");
            }

            if (request.responseCode != 0)
            {
                HasResponse = true;
            }
        }

        /// <summary>
        /// 再接続ポップアップを行うべきWebRequestErrorかどうか
        /// 現状はバトルの終了
        /// </summary>
        /// <returns></returns>
        public bool IsReConnectError()
        {
            //ネットワーク接続できないもしくはステータスコードがタイムアウト
            return (Application.internetReachability == NetworkReachability.NotReachable);
        }

        /// <summary>
        /// チャット関連のAPIでのエラーかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsChatError()
        {
            try
            {
                return Request.url.Contains("/chat/");
            }
            catch (NullReferenceException ex)
            {
                Debug.LogWarning("UnityWebRequest has already been destroyed.");
                return false;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Text))
            {
                return "StatusCode:" + StatusCode + " " + RawErrorMessage;
            }

            return "StatusCode:" + StatusCode + " " + RawErrorMessage + " " + Text;
        }
    }
}