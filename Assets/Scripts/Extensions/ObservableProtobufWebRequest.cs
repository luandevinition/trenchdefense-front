using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Exceptions;
using Program.Scripts.Components.Communication;
using ProtoBuf;
using UniRx;
using UnityEngine.Networking;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

namespace Extensions
{
    /// <summary>
    /// http://qiita.com/Marimoiro/items/7f007805ab8825c43a80 の記事をベースに作成
    /// </summary>
    public static class ObservableProtobufWebRequest
    {
        public static IObservable<UnityWebRequest> ToRequestObservable(this UnityWebRequest request, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) =>
                FetchRequest(request, null, observer, progress, cancellation));
        }

        public static IObservable<string> Get(string url, IDictionary<string, string> headers = null, UniRx.IProgress<float> progress = null)
        {
            return
                ObservableUnity.FromCoroutine<string>(
                    (observer, cancellation) =>
                        FetchText(RequestParameter.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<List<IExtensible>> GetAndGetProtobufs(string url,
            IDictionary<string, string> headers = null, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<List<IExtensible>>((observer, cancellation) =>
                FetchProtobuf(RequestParameter.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<List<IExtensible>> PostAndGetProtobufs(string url, byte[] postData,
            IDictionary<string, string> headers, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<List<IExtensible>>((observer, cancellation) => FetchProtobuf(
                RequestParameter.Post(url, postData), headers, observer, progress, cancellation));
        }

        public static IObservable<List<IExtensible>> PutAndGetProtobufs(string url, byte[] putData,
            IDictionary<string, string> headers, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<List<IExtensible>>((observer, cancellation) =>
                FetchProtobuf(RequestParameter.Put(url, putData), headers, observer, progress, cancellation));
        }

        public static IObservable<List<IExtensible>> DeleteAndGetProtobufs(string url,
            IDictionary<string, string> headers, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<List<IExtensible>>((observer, cancellation) =>
                FetchProtobuf(RequestParameter.Delete(url), headers, observer, progress, cancellation));
        }

        public static IObservable<List<IExtensible>> DeleteAndGetProtobufs(string url, byte[] deleteData,
            IDictionary<string, string> headers, UniRx.IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<List<IExtensible>>((observer, cancellation) => FetchProtobuf(
                RequestParameter.Delete(url,deleteData), headers, observer, progress, cancellation));
        }

        static IEnumerator Fetch<T>(UnityWebRequest request, IDictionary<string, string> headers, IObserver<T> observer, UniRx.IProgress<float> reportProgress, CancellationToken cancel)
        {

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }

            }

            if (reportProgress != null)
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone && !cancel.IsCancellationRequested)
                {
                    try
                    {
                        reportProgress.Report(operation.progress);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        yield break;
                    }
                    yield return null;
                }
            }
            else
            {
                yield return request.SendWebRequest();
            }



            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            if (reportProgress != null)
            {
                try
                {
                    reportProgress.Report(request.downloadProgress);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    yield break;
                }
            }
        }

        static IEnumerator FetchRequest(UnityWebRequest request, IDictionary<string, string> headers,
            IObserver<UnityWebRequest> observer, UniRx.IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (request)
            {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }

                if (!string.IsNullOrEmpty(request.error))
                {
                    observer.OnError(new UnityWebRequestErrorException(request));
                }
                else
                {
                    observer.OnNext(request);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchText(RequestParameter param, IDictionary<string, string> headers,
            IObserver<string> observer, UniRx.IProgress<float> reportProgress, CancellationToken cancel)
        {
            var request = param.CreateUnityWebRequest();

            using (request)
            {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }

                if (!string.IsNullOrEmpty(request.error))
                {
                    observer.OnError(new UnityWebRequestErrorException(request));
                }
                else
                {
                    var text = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                    observer.OnNext(text);
                    observer.OnCompleted();
                }
            }
        }


        static IEnumerator FetchProtobuf(RequestParameter param, IDictionary<string, string> headers,
            IObserver<List<IExtensible>> observer, UniRx.IProgress<float> reportProgress, CancellationToken cancel)
        {
            var request = param.CreateUnityWebRequest();

            using (request)
            {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }
                var responseCode = request.responseCode.ToString();
                
                if (HasError(request, responseCode))
                {
                    observer.OnError(new UnityWebRequestErrorException(request));
                }
                else
                {
                    var iExtensibleList = request.downloadHandler == null
                        ? new List<IExtensible>()
                        : ProtobufConverter.DeserializeResponseData(request.downloadHandler.data);
                    observer.OnNext(iExtensibleList);
                    observer.OnCompleted();
                }
            }
        }

        private static bool HasError(UnityWebRequest request, string responseCode)
        {
            return !string.IsNullOrEmpty(request.error) || responseCode.StartsWith("4") || responseCode.StartsWith("5");
        }
    }
}