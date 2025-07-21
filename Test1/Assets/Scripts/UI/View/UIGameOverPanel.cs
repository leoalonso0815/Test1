using TMPro;
using UnityEngine;

public class UIGameOverPanel : UIBase
{
    public override UILayerEnum PanelLayer => UILayerEnum.Top;

    private TextMeshProUGUI textCoin;
    private TextMeshProUGUI textPork;

    protected override void Init()
    {
        textCoin = transform.Find("TextCoinMain/TextCoin").GetComponent<TextMeshProUGUI>();
        textPork = transform.Find("TextPorkMain/TextPork").GetComponent<TextMeshProUGUI>();
    }

    public override void Show()
    {
        var curCoin = CharacterManager.Instance.GetHeroCoin();
        textCoin.SetText($"{curCoin}");
        var curPorkSteak = CharacterManager.Instance.GetHeroPorkSteak();
        textPork.SetText($"{curPorkSteak}");
    }
}