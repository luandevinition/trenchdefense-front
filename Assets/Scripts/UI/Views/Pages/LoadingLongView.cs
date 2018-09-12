using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 長いバージョンのローディング画面を表示する
/// 仕様：【Page】ローディング_長
/// https://techcross.atlassian.net/wiki/spaces/FENCER/pages/85627142/Page
/// attached prefab：fencer/Assets/UI/Prefabs/Page/Loading_Long.prefab
/// </summary>
public class LoadingLongView : MonoBehaviour
{
    [SerializeField, Header("ローディンググループのNode")]
    private GameObject _loadingGroupAPartsNode;

    [SerializeField, Header("ローディンググループのprefab")]
    private GameObject _loadingGroupA;

    [SerializeField, Header("ローディングロング背景	")]
    private Image _backgroundImage;

    [SerializeField, Header("タイトルテキスト")]
    private Text _titleText;

    [SerializeField, Header("メインテキスト")]
    private Text _tipsText;
    
    /// <summary>
    /// ローディング画面の要素を置き換えます。
    /// </summary>
    /// <param name="config">差し替える設定内容</param>
    /// <param name="pageInstance"></param>
    public void Bind(LoadingLongConfigs config, GameObject pageInstance)
    {
        Instantiate(_loadingGroupA, _loadingGroupAPartsNode.transform, false);
        _titleText.text = config.TitleText;
        _tipsText.text = config.BodyText;
        _backgroundImage.sprite = config.BackgroundImage;
    }
}
