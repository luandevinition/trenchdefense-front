using System.Collections;
using UnityEngine;
using UniRx;

namespace UI.Scripts.PageTransitions.Loading
{
    /// <summary>
    /// ローディング画面関連の処理をUniRxに紐づけるExtension関数
    /// </summary>
    public static class ObservableLoadTransitionExtensions
    {
        /// <summary>
        /// ローディングパネルを表示しながら非同期処理を行う
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="action"></param>
        public static IObservable<TResult> DoWithShortLoadingTransition<T, TResult>(
            this IObservable<T> stream,
            System.Func<T, IObservable<TResult>> action)
        {
            return stream
                .SelectMany(val => Observable.Return(LoadingShortTransition.StartLoading())
                    .SelectMany(loadingObject => action(val)
                        .Select(result => new {loadingObject, result})
                    )
                )
                .Do(obj => LoadingShortTransition.EndLoading(obj.loadingObject))
                .Select(obj => obj.result)
                ;
        }

    }
}
