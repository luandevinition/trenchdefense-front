using UnityEngine;
using System;

[Serializable]
public class PopupConfigs
{
    [Header("Popup識別ID")] public int Id; // ID
    [Header("ポップアップ名")] public string Name; // 名前
    [Header("タイトル名")] public string TitleText; // タイトルテキスト
    [Header("その他要素のPrefab")] public GameObject OtherPrefab; // その他要素
    [Header("本文")] [TextArea(1,20)] public string BodyText; // 本文
    [Header("ボタン設定1")] public PopupButtonConfig Button1; // ボタン
    [Header("ボタン設定2")] public PopupButtonConfig Button2; // ボタン

    public Action<GameObject> BindForOtherPrefab; //CommonPopup内で生成するPrefabに対する動作を登録する

    public PopupConfigs Clone()
    {
        return new PopupConfigs {
            Id = Id,
            Name = Name,
            TitleText = TitleText,
            OtherPrefab = OtherPrefab,
            BodyText = BodyText,
            Button1 = Button1,
            Button2 = Button2,
            BindForOtherPrefab = BindForOtherPrefab,
        };
    }
}

[Serializable]
public class PopupButtonConfig
{
    public GameObject ButtonPrefab;
    public string Text;
}
