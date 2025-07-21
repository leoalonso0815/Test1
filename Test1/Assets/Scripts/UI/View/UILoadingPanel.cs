using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Tween;

public class UILoadingPanel : UIBase
{
    public override UILayerEnum PanelLayer => UILayerEnum.Top;

    private Image loadingImage;
    private TweenPlayer tweenPlayer;
    private TweenImageFillAmount tweenImageFillAmount;

    protected override void Init()
    {
        loadingImage = transform.Find("LoadingSliderBG/LoadingSlider").GetComponent<Image>();
        tweenPlayer = loadingImage.GetComponent<TweenPlayer>();
        tweenImageFillAmount =tweenPlayer.GetAnimation<TweenImageFillAmount>();
        loadingImage.fillAmount = 0f;
    }

    public override void Show()
    {
    }

    public void UpdateSlider(float progress)
    {
        tweenImageFillAmount.@from = tweenImageFillAmount.current;
        tweenImageFillAmount.@to = progress;
        tweenPlayer.Play();
    }
}