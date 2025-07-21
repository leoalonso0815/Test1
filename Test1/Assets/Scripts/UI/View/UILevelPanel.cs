using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityExtensions.Tween;

public class UILevelPanel : UIBase
{
    private TextMeshProUGUI textCoin;
    private TextMeshProUGUI textSteak;
    private TextMeshProUGUI textHeroHp;
    private Image heroHpBar;
    private UIParticle uiParticleCoin;
    private UIParticle uiParticleSteak;
    private TweenPlayer textCoinTween;
    private TweenPlayer textSteakTween;

    protected override void Init()
    {
        textCoin = transform.Find("ResourceCoin/TextCoin").GetComponent<TextMeshProUGUI>();
        textCoinTween = transform.Find("ResourceCoin/TextCoin").GetComponent<TweenPlayer>();
        uiParticleCoin = transform.Find("ResourceCoin/UIParticleCoin").GetComponent<UIParticle>();
        textSteak = transform.Find("ResourceSteak/TextSteak").GetComponent<TextMeshProUGUI>();
        textSteakTween = transform.Find("ResourceSteak/TextSteak").GetComponent<TweenPlayer>();
        uiParticleSteak = transform.Find("ResourceSteak/UIParticleSteak").GetComponent<UIParticle>();
        textHeroHp = transform.Find("HeroHpBar/TextHp").GetComponent<TextMeshProUGUI>();
        heroHpBar = transform.Find("HeroHpBar/HpBar").GetComponent<Image>();
        EventManager.Instance.AddListener<HeroInitEvent>(OnHeroInit);
        EventManager.Instance.AddListener<MonsterDeathAddCoinEvent>(OnCoinAdded);
        EventManager.Instance.AddListener<MonsterDeathAddSteakEvent>(OnSteakAdded);
        EventManager.Instance.AddListener<HeroHurtEvent>(OnHeroHurt);
    }

    public override void Show()
    {
        textCoin.SetText($"0");
        textCoin.SetText($"0");
        heroHpBar.color = Color.green;
    }

    private void OnHeroInit(HeroInitEvent e)
    {
        var heroData = CharacterManager.Instance.GetHeroCharacterData();
        textHeroHp.SetText($"{heroData.CurHp}/{heroData.MaxHp}");
    }

    private void OnCoinAdded(MonsterDeathAddCoinEvent e)
    {
        textCoinTween.Play();
        uiParticleCoin.Play();
        var curCoin = CharacterManager.Instance.GetHeroCoin();
        textCoin.SetText($"{curCoin}");
    }

    private void OnSteakAdded(MonsterDeathAddSteakEvent e)
    {
        textSteakTween.Play();
        uiParticleSteak.Play();
        var curSteak = CharacterManager.Instance.GetHeroPorkSteak();
        textSteak.SetText($"{curSteak}");
    }

    private void OnHeroHurt(HeroHurtEvent e)
    {
        textHeroHp.SetText($"{e.curHp}/{e.maxHp}");
        var ratio = e.HeroHpRatio;
        heroHpBar.fillAmount = ratio;
        if (ratio is >= 0.3f and < 0.5f)
        {
            heroHpBar.color = Color.yellow;
        }
        else if (ratio < 0.3f)
        {
            heroHpBar.color = Color.red;
        }
    }
}