using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StateWindow : MonoBehaviour
{
    // Things that in state window.
    public Text playerName;
    public Text level;
    public Text exp;
    public Text maxExp;
    public Text hp;
    public Text maxHp;
    public Text mp;
    public Text maxMp;
    public Text atk;
    public Text def;
    public Text mdef;
    public Text crihit;
    public Text criDamage;
    public Text speed;
    public Text weaponName;
    public Image weaponIcon;

    public Image weaponInfoBG;
    public Text weaponInfoName;
    public Text weaponAtkValue;
    public Text weaponCrihitValue;
    public Text weaponCriatkValue;
    public Text weaponDescription;
    public Image weaponImage;
    // Things that show on the top left.
    public RectTransform playerState;
    public Text sName;
    public Text sLevel;
    public Text hpText;
    public Text maxHpText;
    public Image HpBarBG;
    public Image HpBar;
    public Text mpText;
    public Text maxMpText;
    public Image MpBarBG;
    public Image MpBar;
    public Text expText;
    public Text maxExpText;
    public Image ExpBarBG;
    public Image ExpBar;

    public Image buffIconPrefab;

    private CanvasGroup canvas;
    private Dictionary<string, Image> buffIcons;
    private Dictionary<string, Coroutine> buffCoroutines;

    private void Awake()
    {
        buffIcons = new Dictionary<string, Image>();
        buffCoroutines = new Dictionary<string, Coroutine>();
        canvas = GetComponent<CanvasGroup>();
        Hide();
        HideItemInfo();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener(delegate { ShowItemInfo(Player.Instance.EquippedWeapon); });
        EventTrigger.Entry entry2 = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        entry2.callback.AddListener(delegate { HideItemInfo(); });
        weaponIcon.GetComponent<EventTrigger>().triggers.Add(entry);
        weaponIcon.GetComponent<EventTrigger>().triggers.Add(entry2);
    }

    // Start is called before the first frame update
    void Start()
    {
        Refrech();
    }

    // Update is called once per frame
    void Update()
    {
        if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.Status))
        {
            if (Input.GetKeyDown(KeyboardSetting.specialKeyTable[ValueType.Status].key.key))
            {
                if (canvas.alpha == 0)
                    Show();
                else
                    Hide();
            }
        }
    }

    public void Show()
    {
        Refrech();
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void ShowItemInfo(Weapon weapon)
    {
        weaponInfoBG.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);
        weaponInfoName.text = weapon.weaponName;
        weaponAtkValue.text = weapon.atkValue.ToString();
        weaponCriatkValue.text = weapon.criatkValue.ToString();
        weaponCrihitValue.text = weapon.crihitValue.ToString();
        weaponDescription.text = weapon.description;
        weaponImage.sprite = weapon.icon;
    }

    public void HideItemInfo()
    {
        weaponInfoBG.rectTransform.localScale = new Vector3(0, 0, 0);
    }

    public void Refrech()
    {
        playerName.text = GameManager.instance.PlayerName;
        level.text = Player.Instance.LV.ToString();
        exp.text = Player.Instance.Exp.ToString();
        maxExp.text = Player.Instance.nextExps[Player.Instance.LV - 1].ToString();
        hp.text = Player.Instance.HP.ToString();
        maxHp.text = Player.Instance.MaxHP.ToString();
        mp.text = Player.Instance.MP.ToString();
        maxMp.text = Player.Instance.MaxMP.ToString();
        atk.text = Player.Instance.Atk.ToString();
        def.text = Player.Instance.Def.ToString();
        mdef.text = Player.Instance.MDef.ToString();
        crihit.text = Player.Instance.Crihit.ToString();
        criDamage.text = Player.Instance.Criatk.ToString();
        speed.text = Player.Instance.Speed.ToString();
        weaponName.text = Player.Instance.EquippedWeapon.weaponName;
        weaponIcon.sprite = Player.Instance.EquippedWeapon.icon;

        sName.text = GameManager.instance.PlayerName;
        sLevel.text = Player.Instance.LV.ToString();
        //Vector2 size = HpBar.rectTransform.sizeDelta;
        //size.x = (float)Player.Instance.HP / Player.Instance.MaxHP * HpBarBG.rectTransform.sizeDelta.x;
        //HpBar.rectTransform.sizeDelta = size;
        HpBar.fillAmount = (float)Player.Instance.HP / Player.Instance.MaxHP;
        hpText.text = Player.Instance.HP.ToString();
        maxHpText.text = Player.Instance.MaxHP.ToString();
        //size = MpBar.rectTransform.sizeDelta;
        //size.x = (float)Player.Instance.MP / Player.Instance.MaxMP * MpBarBG.rectTransform.sizeDelta.x;
        MpBar.fillAmount = (float)Player.Instance.MP / Player.Instance.MaxMP;
        mpText.text = Player.Instance.MP.ToString();
        maxMpText.text = Player.Instance.MaxMP.ToString();
        //size = ExpBar.rectTransform.sizeDelta;
        //size.x = (float)Player.Instance.Exp / Player.Instance.nextExps[Player.Instance.LV - 1] * ExpBarBG.rectTransform.sizeDelta.x;
        //ExpBar.rectTransform.sizeDelta = size;
        ExpBar.fillAmount = (float)Player.Instance.Exp / Player.Instance.nextExps[Player.Instance.LV - 1];
        expText.text = Player.Instance.Exp.ToString();
        maxExpText.text = Player.Instance.nextExps[Player.Instance.LV - 1].ToString();
    }

    public void AddBuffIcon(string id, Sprite icon, float time)
    {
        if (buffIcons.ContainsKey(id))
        {
            Destroy(buffIcons[id].gameObject);
            StopCoroutine(buffCoroutines[id]);
            buffCoroutines.Remove(id);
            buffIcons.Remove(id);
            foreach (KeyValuePair<string, Image> buffImage in buffIcons)
                buffImage.Value.rectTransform.Translate(new Vector2(-0.55f, 0));
        }
        Image image = Instantiate(buffIconPrefab, playerState);
        image.rectTransform.localPosition = new Vector2(-5.049f + 0.55f * buffIcons.Count, 2.679f);
        image.sprite = icon;
        buffCoroutines.Add(id, StartCoroutine(ShowBuffIcon(id, image, time)));
    }

    IEnumerator ShowBuffIcon(string id, Image image, float time)
    {
        buffIcons.Add(id, image);
        Text text = image.transform.GetChild(0).GetComponent<Text>();
        while (time > 0)
        {
            text.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
        }
        float x = image.rectTransform.position.x;
        buffIcons.Remove(id);
        buffCoroutines.Remove(id);
        Destroy(image.gameObject);
        foreach (KeyValuePair<string, Image> buffImage in buffIcons)
        {
            if (buffImage.Value.rectTransform.position.x > x)
                buffImage.Value.rectTransform.Translate(new Vector2(-0.55f, 0));
        }
    }
}
