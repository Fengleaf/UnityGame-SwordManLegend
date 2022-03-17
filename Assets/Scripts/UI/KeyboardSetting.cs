using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ValueType
{
    Item,
    Skill,
    SkillTree,
    Keyboard,
    Status,
    ItemBag,
    Setting,
    Pick,
    Jump
}

public class KeyValue
{
    public SettingKey key;
    public ValueType type;
    public GameObject obj;
    public KeyValue(SettingKey key, ValueType type, GameObject obj)
    {
        this.key = key;
        this.obj = obj;
        this.type = type;
    }
}

public class KeyboardSetting : MonoBehaviour
{
    public static Hashtable skillKeyTable = new Hashtable();
    public static Hashtable itemKeyTable = new Hashtable();
    public static Dictionary<ValueType, KeyValue> specialKeyTable = new Dictionary<ValueType, KeyValue>();
    public SettableItem nowSelected;
    public static bool isSetting;

    public SettableItem pickKey;
    public Vector2 pickKeyOriginPosition;
    public SettableItem jumpKey;
    public Vector2 jumpKeyOriginPosition;
    public SettableItem skillTreeKey;
    public Vector2 skillTreeKeyOriginPosition;
    public SettableItem statusKey;
    public Vector2 statusKeyOriginPosition;
    public SettableItem keyboardKey;
    public Vector2 keyboardKeyOriginPosition;
    public SettableItem itemBagKey;
    public Vector2 itemBagKeyOriginPosition;
    public SettableItem settingKey;
    public Vector2 settingKeyOriginPosition;

    private GameObject settingImage;
    private CanvasGroup canvas;

    private static KeyboardSetting instance;

    public static KeyboardSetting Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(KeyboardSetting)) as KeyboardSetting;
                if (instance == null)
                {
                    GameObject go = new GameObject("KeyboardSetting");
                    instance = go.AddComponent<KeyboardSetting>();
                }
            }
            return instance;
        }
    }

    private KeySettingShower keySettingShower;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this) Destroy(gameObject);
        canvas = GetComponent<CanvasGroup>();
        Hide();
        keySettingShower = FindObjectOfType<KeySettingShower>();
    }

    private void Update()
    {
        if (specialKeyTable.ContainsKey(ValueType.Keyboard))
        {
            if (Input.GetKeyDown(specialKeyTable[ValueType.Keyboard].key.key))
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
        if (canvas.alpha == 1)
            return;
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        isSetting = false;
        StopCoroutine("SetItem");
        if (nowSelected != null)
            Destroy(nowSelected.gameObject);
        nowSelected = null;
        if (settingImage != null)
            Destroy(settingImage.gameObject);
        settingImage = null;
    }

    public void SelectItem(SettableItem item)
    {
        // Is setting key, player's input will be invalid.
        isSetting = true;
        // Create an image that follow the player's mouse.
        //nowSelected = new GameObject(item.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(SettableItem)).GetComponent<SettableItem>();
        //nowSelected.transform.SetParent(transform);
        //nowSelected.GetComponent<Image>().sprite = item.GetComponent<Image>().sprite;
        //nowSelected.GetComponent<RectTransform>().sizeDelta = item.GetComponent<RectTransform>().sizeDelta;
        //nowSelected.GetComponent<RectTransform>().localScale = item.GetComponent<RectTransform>().localScale;
        //nowSelected.transform.SetParent(transform.parent);
        //nowSelected.type = item.type;
        //nowSelected.item = item.item;
        nowSelected = Instantiate(item, transform.parent);
        // It can't be clicked.
        nowSelected.GetComponent<Image>().raycastTarget = false;
        if (nowSelected.transform.childCount == 1)
        {
            Destroy(nowSelected.transform.GetChild(0).gameObject);
        }
        settingImage = nowSelected.gameObject;
        StartCoroutine("SetItem");
    }

    public void SelectSpecialItem(SettableItem item)
    {
        // Is setting key, player's input will be invalid.
        isSetting = true;
        // Create an image that follow the player's mouse.
        nowSelected = Instantiate(item, transform.parent);
        nowSelected.type = item.type;
        nowSelected.item = item.item;
        // It can't be clicked.
        nowSelected.GetComponent<Image>().raycastTarget = false;
        nowSelected.transform.GetChild(0).GetComponent<Text>().raycastTarget = false;
        settingImage = nowSelected.gameObject;
        StartCoroutine("SetItem");
    }

    public void RegisterKey(SettingKey key)
    {
        StopCoroutine("SetItem");
        // Player drag other item to this key, override.
        if (nowSelected != null)
        {
            StopCoroutine("SetItem");
            if (key.settingItem != null)
            {
                if (skillKeyTable.Contains(key.key))
                    skillKeyTable.Remove(key.key);
                if (itemKeyTable.Contains(key.key))
                    itemKeyTable.Remove(key.key);
                if (specialKeyTable.ContainsKey(key.settingItem.GetComponent<SettableItem>().type))
                    specialKeyTable.Remove(key.settingItem.GetComponent<SettableItem>().type);
                Destroy(key.settingItem);
            }
            //key.settingItem = new GameObject(nowSelected.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(SettableItem));
            //key.settingItem.transform.SetParent(transform);
            //key.settingItem.GetComponent<Image>().sprite = nowSelected.GetComponent<Image>().sprite;
            //key.settingItem.GetComponent<Image>().color = nowSelected.GetComponent<Image>().color;
            //key.settingItem.GetComponent<Image>().raycastTarget = false;
            //key.settingItem.GetComponent<RectTransform>().sizeDelta = nowSelected.GetComponent<RectTransform>().sizeDelta;
            //key.settingItem.GetComponent<RectTransform>().localScale = nowSelected.GetComponent<RectTransform>().localScale;
            //key.settingItem.transform.position = key.transform.position;
            //key.settingItem.GetComponent<SettableItem>().type = nowSelected.type;
            //key.settingItem.GetComponent<SettableItem>().item = nowSelected.item;

            if (nowSelected.type == ValueType.Skill)
            {
                foreach (string s in skillKeyTable.Keys)
                {
                    KeyValue value = (KeyValue)skillKeyTable[s];
                    if (value.obj.GetComponent<Skill>().id == nowSelected.item.GetComponent<Skill>().id)
                    {
                        keySettingShower.HideKey(s);
                        Destroy(value.key.settingItem.gameObject);
                        skillKeyTable.Remove(s);
                        break;
                    }
                }
                KeyValue keyValue = new KeyValue(key, nowSelected.type, nowSelected.item);
                skillKeyTable[key.key] = keyValue;
            }
            else if(nowSelected.type == ValueType.Item)
            {
                foreach (string s in itemKeyTable.Keys)
                {
                    KeyValue value = (KeyValue)itemKeyTable[s];
                    if (value.obj.GetComponent<Item>().id == nowSelected.item.GetComponent<Item>().id)
                    {
                        keySettingShower.HideKey(s);
                        Destroy(value.key.settingItem.gameObject);
                        itemKeyTable.Remove(s);
                        break;
                    }
                }
                KeyValue keyValue = new KeyValue(key, nowSelected.type, nowSelected.item);
                itemKeyTable[key.key] = keyValue;
            }
            else
            {
                KeyValue value;
                if (specialKeyTable.ContainsKey(nowSelected.type))
                {
                    value = specialKeyTable[nowSelected.type];
                    keySettingShower.HideKey(value.key.key);
                    Destroy(value.key.settingItem.gameObject);
                }

                value = new KeyValue(key, nowSelected.type, nowSelected.item);
                specialKeyTable[nowSelected.type] = value;
            }
            key.settingItem = Instantiate(nowSelected, transform).gameObject;
            key.settingItem.transform.position = key.transform.position;
            keySettingShower.ShowKey(key.key, nowSelected.GetComponent<Image>().sprite);
            settingImage = null;
            Destroy(nowSelected.gameObject);
            nowSelected = null;
            isSetting = false;
        }
        // Player click the key directly, edit item on key, if any. 
        else if (key.settingItem != null)
        {
            SettableItem settableItem = key.settingItem.GetComponent<SettableItem>();
            if (settableItem.type == ValueType.Item || settableItem.type == ValueType.Skill)
                SelectItem(settableItem);
            else
                SelectSpecialItem(settableItem);
            if (skillKeyTable.Contains(key.key))
            {
                skillKeyTable.Remove(key.key);
            }
            if (itemKeyTable.Contains(key.key))
                itemKeyTable.Remove(key.key);
            if (specialKeyTable.ContainsKey(key.settingItem.GetComponent<SettableItem>().type))
                specialKeyTable.Remove(key.settingItem.GetComponent<SettableItem>().type);
            keySettingShower.HideKey(key.key);
            Destroy(key.settingItem.gameObject);
        }
    }

    public void EquipWeapon(SettingKey key)
    {
        if (nowSelected != null)
        {
            StopCoroutine("SetItem");
            if (nowSelected.item.GetComponent<Weapon>() != null)
            {
                Player.Instance.EquipWeapon(nowSelected.item.GetComponent<Weapon>());
                Destroy(settingImage.gameObject);
                settingImage = null;
                Destroy(nowSelected.gameObject);
                nowSelected = null;
                isSetting = false;
            }
        }
        StopCoroutine("SetItem");
    }

    IEnumerator SetItem()
    {
        // To prevent get mouse down when it just starts.
        yield return null;
        while (true)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (settingImage == null)
            {
                break;
            }
            try
            {
                settingImage.transform.position = new Vector2(position.x, position.y);
            }
            catch (System.Exception)
            {
                break;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    GameObject g = EventSystem.current.currentSelectedGameObject;
                    if (g == null || g.GetComponent<SettingKey>() == null)
                    {
                        Destroy(settingImage);
                        settingImage = null;
                        isSetting = false;
                        break;
                    }
                }
                else
                {
                    Destroy(settingImage);
                    settingImage = null;
                    isSetting = false;
                    break;
                }
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator SetSpecialItem(ValueType type)
    {
        while (true)
        {
            yield return null;
        }
    }
}
