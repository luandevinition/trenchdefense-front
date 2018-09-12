using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using UI.Scripts.Interfaces.Popup.CommonPopup;
using UI.Scripts.Popup.CommonPopup;
using Vexe.Runtime.Extensions;
using Tuple = UniRx.Tuple;

namespace UI.Scripts.Parts
{
    /// <summary>
    /// 汎用ポップアップ
    /// PopupConfigs によって、表示内容を変更できる
    /// </summary>
    public class CommonPopup : Popup
    {
        private const string POP_UP_ANIMATION_OPEN_NAME = "PopupAnimation_Open";

        private const string POP_UP_ANIMATION_CLOSE_NAME = "PopupAnimation_Close";

        private const string INVALID_OPERATION_EXCEPTION_MESSAGE = "ノード表示が必須なポップアップに node を指定していません";

        private const string ARGUMENT_EXCEPTION_MESSAGE = "Button1の設定がされていません";

        /// <summary>
        /// 何を選択したか
        /// </summary>
        public enum SelectType
        {
            /// <summary>
            /// ボタン1が押された
            /// </summary>
            Button1,

            /// <summary>
            /// ボタン2が押された
            /// </summary>
            Button2,

            /// <summary>
            /// ボタン3が押された
            /// 2択の場合は選択されない
            /// </summary>
            Button3,

            /// <summary>
            /// 閉じるボタンが押された
            /// </summary>
            Close
        }

        /// <summary>
        /// ポップアップの開閉アニメーションを行うアニメーター
        /// </summary>
        [SerializeField]
        private Animator _openCloseAnimator;

        /// <summary>
        /// ポップアップのタイトル
        /// </summary>
        [SerializeField]
        private Text _title;

        /// <summary>
        /// ポップアップに表示するテキスト
        /// </summary>
        [SerializeField]
        private Text _content;

        /// <summary>
        /// ポップアップに表示するテキスト以外のオブジェクトの格納先
        /// </summary>
        [SerializeField]
        private GameObject _contentNodeHolder;

        /// <summary>
        /// 動的に生成するボタンのホルダー
        /// buttonSetting1, buttonSetting2 双方設定されていた場合は 0番目, 1番目を使用
        /// buttonSetting1 のみ設定されていた場合は、2番目を使用
        /// </summary>
        [SerializeField, Header("Button の生成場所")]
        private GameObject[] _buttonHolders;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        [SerializeField]
        private Button _closeButton;

        /// <summary>
        /// 選択したボタンの種類を通知する Subject
        /// </summary>
        /// <value>The select type subject.</value>
        private Subject<SelectType> _selectTypeSubject;

        /// <summary>
        /// ポップアップのボタンイベントのサブスクリプション
        /// </summary>
        private SerialDisposable _subscription;

        /// <summary>
        /// BindConfig で生成されたボタンのシーケンス
        /// </summary>
        private IEnumerable<UniRx.Tuple<SelectType, Button>> _buttonNodeSequence;

        /// <summary>
        /// PopupConfigs で View を初期化する
        /// </summary>
        /// <param name="config">Config.</param>
        /// <param name="formatter">ポップアップ文言のフォーマッター</param>
        /// <param name="soundSettings">ポップアップのサウンド設定</param>
        /// <param name="bodyText">本文</param>
        public void BindConfig(PopupConfigs config, ICommonPopupFormatter formatter, CommonPopupSoundSettings soundSettings, string bodyText = null)
        {
            ClearNodes();
            CreateNodePrefab(config);
            _buttonNodeSequence = CreateButtonNodes(config.Button1, config.Button2, formatter, soundSettings);

            _title.text = formatter.GetTitleText(config);
            // 本文の設定（本文の指定がなければポップアップのAssetに設定されている内容を取得）
            _content.text = string.IsNullOrEmpty(bodyText) ? formatter.GetBodyText(config) : bodyText;

            // CheckButtonSoundOverwritten(_closeButton.gameObject, SelectType.Close, soundSettings);
        }

        /// <summary>
        /// ポップアップを開く
        /// </param>
        /// <returns>ポップアップを開くコルーチン</returns>
        public override IEnumerator Open()
        {
            gameObject.SetActive(true);
            yield return _openCloseAnimator.PlayAsync(POP_UP_ANIMATION_OPEN_NAME);

            _subscription.Disposable = _buttonNodeSequence.Concat(new [] { Tuple.Create(SelectType.Close, _closeButton) })
                .Select(x => x.Item2.OnClickAsObservable().Select(_ => x.Item1))
                .Merge()
                .Subscribe(_selectTypeSubject.OnNext);
        }

        /// <summary>
        /// ポップアップを閉じる
        /// </summary>
        /// <returns>ポップアップを閉じるコルーチン</returns>
        public override IEnumerator Close()
        {
            _subscription.Disposable = Disposable.Empty;
            yield return _openCloseAnimator.PlayAsync(POP_UP_ANIMATION_CLOSE_NAME);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ボタンを選択した時に、選択したボタンの種類を流すストリームを取得する
        /// ポップアップが破棄された時、 OnCompleted が流れる
        /// </summary>
        /// <returns>
        /// ボタンを選択した時に、選択したボタンの種類を流すストリーム
        /// </returns>
        public IObservable<SelectType> OnSelectAsObservable()
        {
            return _selectTypeSubject.TakeUntilDestroy(this);
        }

        /// <summary>
        /// 指定したボタンタイプのストリームを流す
        /// </summary>
        /// <param name="selectType"></param>
        /// <returns></returns>
        public IObservable<SelectType> OnSelectButtonObservable(CommonPopup.SelectType selectType)
        {
            return OnSelectAsObservable().Where(buttonType => buttonType == selectType);
        }

        /// <summary>
        /// 指定したプレハブを ContentNodeHolder に生成する
        /// </summary>
        /// <param name="configs">config</param>
        private void CreateNodePrefab(PopupConfigs configs)
        {
            if (_contentNodeHolder == null)
            {
                return;
            }

            if (_contentNodeHolder != null && configs.OtherPrefab == null)
            {
                throw new InvalidOperationException(INVALID_OPERATION_EXCEPTION_MESSAGE);
            }

            var node = Instantiate(configs.OtherPrefab, _contentNodeHolder.transform, false);

            if (configs.BindForOtherPrefab != null)
            {
                configs.BindForOtherPrefab(node);
            }
        }

        /// <summary>
        /// ポップアップの設定に従ったボタンノードを作成する
        /// </summary>
        /// <returns>The button nodes.</returns>
        /// <param name="buttonConfig1">Button1 の設定</param>
        /// <param name="buttonConfig2">Button2 の設定(ButtonPrefab が null なら Button1 の情報を用いて Button3 が生成される)</param>
        /// <param name="formatter">ボタンテキストのフォーマッター</param>
        /// <param name="soundSettings">ポップアップのサウンド設定</param>
        private IEnumerable<UniRx.Tuple<SelectType, Button>> CreateButtonNodes(PopupButtonConfig buttonConfig1, PopupButtonConfig buttonConfig2, ICommonPopupFormatter formatter, CommonPopupSoundSettings soundSettings)
        {
            if (buttonConfig1.ButtonPrefab == null)
            {
                throw new ArgumentException(ARGUMENT_EXCEPTION_MESSAGE);
            }

            if (buttonConfig2.ButtonPrefab != null)
            {
                // Button2 の設定があるので Button1, Button2 を生成
                return new [] { 
                    Tuple.Create(SelectType.Button1, CreateButtonNode(SelectType.Button1, buttonConfig1, formatter.GetButtonText1, soundSettings)),
                    Tuple.Create(SelectType.Button2, CreateButtonNode(SelectType.Button2, buttonConfig2, formatter.GetButtonText2, soundSettings))
                };
            } 
            else
            {
                // Button2 の設定がないので Button3 を生成
                return new [] { 
                    Tuple.Create(SelectType.Button3, CreateButtonNode(SelectType.Button3, buttonConfig1, formatter.GetButtonText1, soundSettings))
                };
            }
        }

        /// <summary>
        /// ボタンのノードを生成する
        /// </summary>
        /// <returns>生成したノード</returns>
        /// <param name="type">ボタンの種類</param>
        /// <param name="buttonConfig">ボタンの設定</param>
        /// <param name="formatter">ボタンテキストのフォーマットを行うメソッド</param>
        /// <param name="soundSettings">ポップアップのサウンド設定</param>
        private Button CreateButtonNode(SelectType type, PopupButtonConfig buttonConfig, System.Func<PopupButtonConfig, string> formatter, CommonPopupSoundSettings soundSettings)
        {
            var button = Instantiate(buttonConfig.ButtonPrefab);
            button.transform.SetParent(_buttonHolders[(int)type].transform, false);

            var buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = formatter(buttonConfig);

            // CheckButtonSoundOverwritten(button, type, soundSettings);

            return button.GetComponent<Button>();
        }

        /// <summary>
        /// 動的に生成されるオブジェクトを削除する
        /// </summary>
        private void ClearNodes()
        {
            if (_contentNodeHolder != null)
            {
                _contentNodeHolder.transform.DestroyChildren();
            }
            foreach (var holder in _buttonHolders)
            {
                holder.transform.DestroyChildren();
            }
        }

        /// <summary>
        /// ボタンサウンド上書き指定チェック
        /// </summary>
        /// <param name="buttonGameObject">ボタンGameObject</param>
        /// <param name="selectType">ボタンの種類</param>
        /// <param name="soundSettings">ポップアップのサウンド設定</param>
        /* private void CheckButtonSoundOverwritten(GameObject buttonGameObject, SelectType selectType, CommonPopupSoundSettings soundSettings)
        {
            var buttonSoundType = soundSettings.GetSelectButtonSound(selectType);
            if (buttonSoundType == null)
            {
                return;
            }

            var buttonController = buttonGameObject.GetComponent<ButtonController>();
            if (buttonController == null)
            {
                return;
            }

            buttonController.SetSoundType(buttonSoundType.Value);
        }*/

        private void Awake()
        {
            _selectTypeSubject = new Subject<SelectType>();
            _selectTypeSubject.AddTo(this);
            _subscription = new SerialDisposable();
            _subscription.AddTo(this);
            _buttonNodeSequence = Enumerable.Empty<UniRx.Tuple<SelectType, Button>>();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// アニメータの拡張機能
    /// </summary>
    internal static class AnimatorExtention
    {
        private const int LAYER = 0;

        private const float NORMALIZED_TIME = 0f;

        private const float FINISH_TIME = 1f;

        /// <summary>
        /// 指定したステートのアニメーションを再生する
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="stateName">State name.</param>
        /// <param name="cancel">コルーチン終了を検知するトークン</param> 
        public static IEnumerator PlayAsync(this Animator source, string stateName, CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            source.Play(stateName, LAYER, NORMALIZED_TIME);
            yield return null;
            if (cancel.IsCancellationRequested)
            {
                yield break;
            }

            yield return new WaitWhile(() => !cancel.IsCancellationRequested && source.GetCurrentAnimatorStateInfo(0).IsName(stateName));
        }

        /// <summary>
        /// 指定したステートのアニメーションを再生する
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="stateName">State name.</param>
        public static IEnumerator PlayAsync(this Animator source, string stateName)
        {
            // Need play animation at end of an frame when animator finished initialize to avoid we play animation before animator initialized
            yield return null;
            if (source == null)
            {
                // FencerLog.Log(String.Format("CommonPopup PlayAsync source is null - stateName {0}", stateName));
                yield break;
            }
            source.Play(stateName, LAYER, NORMALIZED_TIME);
            // yield return new WaitForAnimation(source, LAYER);
        }


        

        /// <summary>
        /// 指定したステートのアニメーションを再生する
        /// </summary>
        /// <param name="source">コンテキスト</param>
        /// <param name="stateName">ステート名</param>
        /// <param name="cancel">コルーチン終了を検知するトークン</param>
        /// <returns>コルーチン</returns>
        public static IEnumerator PlayAsyncByNormalizedTime(this Animator source, string stateName, CancellationToken cancel = default(CancellationToken))
        {
            if (cancel.IsCancellationRequested) {
                yield break;
            }

            source.Play(stateName, LAYER, NORMALIZED_TIME);
            yield return null;
            if (cancel.IsCancellationRequested) {
                yield break;
            }

            yield return new WaitWhile(() => {
                return !cancel.IsCancellationRequested && source.GetCurrentAnimatorStateInfo(LAYER).normalizedTime < FINISH_TIME;
            });
        }
    }
}
