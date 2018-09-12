using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 短いバージョンのローディング画面を表示する
/// 仕様：【Page】ローディング_短
/// https://techcross.atlassian.net/wiki/spaces/FENCER/pages/85627180/Page
/// attached prefab：fencer/Assets/UI/Prefabs/Page/Loading_Short.prefab
/// </summary>
public class LoadingShortView : MonoBehaviour
{
    [SerializeField, Header("半透明の透明度（0~1）")] protected float _alpha = 0.3f;

    [SerializeField, Header("ローディンググループのNode")]
    private GameObject _loadingGroupAPartsNode;

    [SerializeField, Header("ローディンググループのprefab")]
    private GameObject _loadingGroupA;

    /// <summary>
    /// 壁紙を半透明にする
    /// </summary>
    /// <param name="pageInstance"></param>
    public void DoBackgroundTranslucent(GameObject pageInstance)
    {
        var children = pageInstance.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name == "BackgroundImage")
            {
                var image = child.GetComponent<Image>();
                SetTransparency(image);
            }
        }
    }

    /// <summary>
    /// 背景を黒にする
    /// </summary>
    /// <param name="pageInstance"></param>
    public void DoBackgroundBlack(GameObject pageInstance)
    {
        var children = pageInstance.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name == "BackgroundImage")
            {
                var image = child.GetComponent<Image>();
                SetOpaque(image);
            }
        }
    }

    /// <summary>
    /// 画像を不透明にする
    /// </summary>
    /// <param name="image"></param>
    private void SetOpaque(Image image)
    {
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;
    }

    /// <summary>
    /// ローディングのビジュアルを非表示し、
    /// タッチシールドを残しておく
    /// </summary>
    public void Hide()
    {
        _loadingGroupAPartsNode.SetActive(false);
        _alpha = 0.0f;
        DoBackgroundTranslucent(gameObject);
    }

    /// <summary>
    /// 画像を半透明にする
    /// </summary>
    /// <param name="image">変更対象のImage</param>
    /// <param name="alpha">透明度（0~1）</param>
    private void SetTransparency(Image image)
    {
        Color color = image.color;
        color.a = _alpha;
        image.color = color;
    }

    private void Start()
    {
        Instantiate(_loadingGroupA, _loadingGroupAPartsNode.transform, false);
    }
}
