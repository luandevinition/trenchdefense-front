using System.Collections;
using UnityEngine;

namespace UI.Scripts.Parts
{
    /// <summary>
    /// ポップアップ表示する UI Parts
    /// 開くアニメーションと閉じるアニメーションが実装されいれば良い。
    /// アニメーション方法は実装側に任せる
    /// </summary>
    public abstract class Popup : MonoBehaviour
    {
        /// <summary>
        /// ポップアップを描画する Canvas
        /// レイヤー順を変更するために使用
        /// </summary>
        [SerializeField]
        Canvas PopupCanvas;

        /// <summary>
        /// レイヤー順
        /// </summary>
        /// <value>The order in layer.</value>
        public int OrderInLayer
        { 
            set { PopupCanvas.sortingOrder = value; }
            get { return PopupCanvas.sortingOrder; }
        }

        /// <summary>
        /// 黒フィルターをUI（PagePrefab）よりも常に前面に配置するために前面にあるレイヤーを指定
        /// それによりサブページおよびポップアップも同レイヤーに配置する処理
        /// </summary>
        public void SetSortingLayerName()
        {
            PopupCanvas.sortingLayerName = "Overlay";
        }

        /// <summary>
        /// Popup を開く
        /// </summary>
        public abstract IEnumerator Open();

        /// <summary>
        /// Popup を閉じる
        /// </summary>
        public abstract IEnumerator Close();
    }
}
