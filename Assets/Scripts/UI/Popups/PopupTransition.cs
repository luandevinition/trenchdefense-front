using System;
using System.Collections;
using UnityEngine;
using UniRx;

namespace UI.Scripts.PageTransitions
{
    /// <summary>
    /// ポップアップの遷移処理
    /// 画面にオーバーラップ表示される
    /// </summary>
    public abstract class PopupTransition : PageTransition
    {
        private const string INVALID_OPERATION_EXCEPTION_MESSAGE = "まだ読み込みが完了していません";

        private const string ARGUMENT_EXCEPTION_MESSAGE = "指定したポップアップの型が間違っています";

        [SerializeField, Header("ローディング画面を短いバージョンにするかどうか")] 
        protected BoolReactiveProperty _isShortVersionLoading;
        
        public enum Event
        {
            Open,
            Close
        };

        /// <summary>
        /// Observable event of popup, ex. close and open
        /// </summary>
        public Subject<Event> TransitionEvent = new Subject<Event>();

        protected PopupTransition()
        {
            _isShortVersionLoading = new BoolReactiveProperty(false);
            _isLongVersionLoading.Subscribe(isOn =>
            {
                if (isOn && _isShortVersionLoading.Value)
                {
                    _isShortVersionLoading.Value = false;
                }
            });
            _isShortVersionLoading.Subscribe(isOn =>
            {
                if (isOn && _isLongVersionLoading.Value)
                {
                    _isLongVersionLoading.Value = false;
                }
            });
        }
        
        /// <summary>
        /// ページの表示に必要なモデルを読み込みます
        /// </summary>
        /// <returns></returns>
        /// <remarks>ページの表示に必要なモデルを、サーバから取得したりMyDataクラスから取得したりして保持しておきます。
        /// 保持されたモデルは、BindModelsToメソッド内でページビューモデルに渡されるようにします。</remarks>
        public abstract override IEnumerator LoadAsync();

        /// <summary>
        /// 読み込んだモデルを遷移先ページに渡します。
        /// </summary>
        public abstract override void BindLoadedModels();

        /// <summary>
        /// ポップアップのインスタンスを取得する
        /// </summary>
        /// <returns>インスタンス</returns>
        /// <exception cref="InvalidOperationException">
        /// InstantiatePage が呼び出されていない状態で呼び出された
        /// InstantiatePage は LoadAsync 等が完了して初めて呼び出されるので、読み込み処理が完了していないということ
        /// </exception>
        /// <exception cref="ArgumentException">
        /// 指定した Generic の型がポップアップのインスタンスの型と違う
        /// PopupTransition に設定しているプレハブと、呼び出し側の Generic 型が意図したものになっているか確認してください
        /// </exception>
        /// <typeparam name="T">Popup の型</typeparam>
        public T GetPopupInstance<T>() where T : Parts.Popup
        {
            if (_pageInstance == null)
            {
                throw new InvalidOperationException(INVALID_OPERATION_EXCEPTION_MESSAGE);
            }
            var result = _pageInstance.GetComponent<T>();
            if (result == null)
            {
                throw new ArgumentException(ARGUMENT_EXCEPTION_MESSAGE, typeof(T).Name);
            }
            return result;
        }

        /// <summary>
        /// Check is open short version or not
        /// </summary>
        /// <returns></returns>
        public bool IsShortVersionLoading()
        {
            return _isShortVersionLoading.Value;
        }

        /// <summary>
        /// Begin Loading
        /// </summary>
        public void BeginLoading()
        {
            _loading.Value = true;
        }
        
        /// <summary>
        /// End Loading
        /// </summary>
        public void EndLoading()
        {
            _loading.Value = false;
        }
        
        /// <summary>
        /// Popup を開く
        /// </summary>
        /// <remarks>演出処理のため、例外を発生させないように実装する必要がある</remarks>
        public IEnumerator Open()
        {
            LoadingShortTransition.StartLoading();
            var popup = default(Parts.Popup);
            try
            {
                popup = GetPopupInstance<Parts.Popup>();
            }
            catch(Exception ex)
            {
                LoadingShortTransition.EndLoading();
                yield break;
            }

            var error = default(Exception);
            
            // Open前にNOW LOADINGの表示を止める
            LoadingShortTransition.EndLoading();
            yield return popup.Open();
            if(error != null)
            {
                yield break;
            }

            TransitionEvent.OnNext(Event.Open);
            LoadingShortTransition.EndLoading();
        }

        /// <summary>
        /// Popup を閉じる
        /// </summary>
        public IEnumerator Close()
        {
            var popup = default(Parts.Popup);
            try
            {
                popup = GetPopupInstance<Parts.Popup>();
            }
            catch(Exception ex)
            {
                yield break;
            }

            var error = default(Exception);
            yield return popup.Close();
            if(error != null)
            {
                yield break;
            }

            TransitionEvent.OnNext(Event.Close);
        }

        /// <summary>
        /// Close as observable
        /// </summary>
        /// <returns></returns>
        public IObservable<Event> CloseAsObservable()
        {
            return TransitionEvent
                .Where(e => e == Event.Close)
                .First();
        }
    }
}
