using UnityEngine;
using System;

/// <summary>
/// 長いバージョンのローディング画面の設定情報を提供する
/// </summary>
[Serializable]
public class LoadingLongConfigs
{
    [Header("LoadingLong識別ID")] public int Id; // ID
    [Header("タイトル名")] public string TitleText; // タイトルテキスト
    [Header("本文")] [TextArea(1,200)] public string BodyText; // 本文
    [Header("壁紙画像")] public Sprite BackgroundImage; // 壁紙
}