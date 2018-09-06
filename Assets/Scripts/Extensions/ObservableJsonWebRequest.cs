using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Exceptions;
using ProtoBuf;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;

#endif

namespace UniRx.JsonWebRequest
{
    /// <summary>
    /// http://qiita.com/Marimoiro/items/7f007805ab8825c43a80 の記事をベースに作成
    /// </summary>    
    public static class ObservableJsonWebRequest
    {
        public static IObservable<UnityWebRequest> ToRequestObservable(this UnityWebRequest request,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) =>
                FetchRequest(request, null, observer, progress, cancellation));
        }

        public static IObservable<string> ToObservable(this UnityWebRequest request, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
                FetchText(request, null, observer, progress, cancellation));
        }

        public static IObservable<byte[]> ToBytesObservable(this UnityWebRequest request,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
                Fetch(request, null, observer, progress, cancellation));
        }

        public static IObservable<string> Get(string url, IDictionary<string, string> headers = null,
            IProgress<float> progress = null)
        {
            return
                ObservableUnity.FromCoroutine<string>(
                    (observer, cancellation) =>
                        FetchText(UnityWebRequest.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<byte[]> GetAndGetBytes(string url, IDictionary<string, string> headers = null,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
                FetchBytes(UnityWebRequest.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> GetRequest(string url, IDictionary<string, string> headers = null,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) =>
                Fetch(UnityWebRequest.Get(url), headers, observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, Dictionary<string, string> postData,
            IDictionary<string, string> headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
                FetchText(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, Dictionary<string, string> postData,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
                FetchBytes(UnityWebRequest.Post(url, postData), null, observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, Dictionary<string, string> postData,
            IDictionary<string, string> headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
                FetchBytes(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, Dictionary<string, string> postData,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) =>
                Fetch(UnityWebRequest.Post(url, postData), null, observer, progress, cancellation));
        }

        public static IObservable<UnityWebRequest> PostRequest(string url, Dictionary<string, string> postData,
            IDictionary<string, string> headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<UnityWebRequest>((observer, cancellation) =>
                Fetch(UnityWebRequest.Post(url, postData), headers, observer, progress, cancellation));
        }


        public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, uint version, uint crc,
            IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) =>
                FetchAssetBundle(UnityWebRequest.GetAssetBundle(url, version, crc), null, observer, progress,
                    cancellation));
        }


        static IEnumerator Fetch<T>(UnityWebRequest request, IDictionary<string, string> headers, IObserver<T> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
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
                var operation = request.Send();
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
                yield return request.Send();
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
            IObserver<UnityWebRequest> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
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

        static IEnumerator FetchText(UnityWebRequest request, IDictionary<string, string> headers,
            IObserver<string> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (request)
            {
                yield return Fetch(request, headers, observer, reportProgress, cancel);

                if (cancel.IsCancellationRequested)
                {
                    yield break;
                }
                
                var responseCode = request.responseCode.ToString();

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

        static IEnumerator FetchAssetBundle(UnityWebRequest request, IDictionary<string, string> headers,
            IObserver<AssetBundle> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
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
                    var handler = request.downloadHandler as DownloadHandlerAssetBundle;
                    var assetBundle = (handler != null) ? handler.assetBundle : null;

                    observer.OnNext(assetBundle);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchBytes(UnityWebRequest request, IDictionary<string, string> headers,
            IObserver<byte[]> observer,
            IProgress<float> reportProgress, CancellationToken cancel)
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
                    observer.OnNext(request.downloadHandler.data);
                    observer.OnCompleted();
                }
            }
        }
    }
}