using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSpace
{
    // Item's icon.
    public Image image;
    // It's space index in item bag.
    public int index;
    public ItemSpace(Image image, int index)
    {
        this.image = image;
        this.index = index;
    }
}

public class ItemBag : MonoBehaviour
{
    public int maxColumn;
    public float spacing;
    public Image iconPrefab;

    public Image itemBagBG;

    public Image itemInfoBG;
    public Text itemName;
    public Text itemCoolTime;
    public Text itemDescription;

    public Image weaponInfoBG;
    public Text weaponName;
    public Text weaponAtkValue;
    public Text weaponCrihitValue;
    public Text weaponCriatkValue;
    public Text weaponDescription;
    public Image weaponImage;

    private CanvasGroup canvas;
    private Dictionary<int, ItemSpace> itemImages;
    // key: weapon's id, value: All weapons that have the same id.
    private Dictionary<int, List<ItemSpace>> weaponImages;
    private bool[] space;

    private ItemSpace selectedWeapon;

    // Start is called before the first frame update
    void Start()
    {
        itemImages = new Dictionary<int, ItemSpace>();
        weaponImages = new Dictionary<int, List<ItemSpace>>();
        canvas = GetComponent<CanvasGroup>();
        space = new bool[21];
        for (int i = 0; i < space.Length; i++)
            space[i] = false;
        HideItemInfo();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.ItemBag))
        {
            if (Input.GetKeyDown(KeyboardSetting.specialKeyTable[ValueType.ItemBag].key.key))
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
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        ShowItems();
        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        foreach (KeyValuePair<int, ItemSpace> pair in itemImages)
            Destroy(pair.Value.image.gameObject);
        foreach (KeyValuePair<int, List<ItemSpace>> pair in weaponImages)
        {
            for (int i = 0; i < pair.Value.Count; i++) 
                Destroy(pair.Value[i].image.gameObject);
        }
        for (int i = 0; i < space.Length; i++)
            space[i] = false;
        weaponImages.Clear();
        itemImages.Clear();
    }

    public void Refrech(bool isEquipped = false)
    {
        if (isEquipped)
        {
            space[selectedWeapon.index] = false;
            weaponImages[Player.Instance.EquippedWeapon.id].Remove(selectedWeapon);
            Destroy(selectedWeapon.image.gameObject);
        }
        if (canvas.alpha == 1)
            ShowItems();
    }

    public void ShowItemInfo(Item item)
    {
        itemInfoBG.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);
        itemName.text = item.itemName;
        itemCoolTime.text = item.coolTime.ToString();
        itemDescription.text = item.description;
    }

    public void ShowItemInfo(Weapon weapon)
    {
        weaponInfoBG.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);
        weaponName.text = weapon.weaponName;
        weaponAtkValue.text = weapon.atkValue.ToString();
        weaponCriatkValue.text = weapon.criatkValue.ToString();
        weaponCrihitValue.text = weapon.crihitValue.ToString();
        weaponDescription.text = weapon.description;
        weaponImage.sprite = weapon.icon;
    }

    public void HideItemInfo()
    {
        itemInfoBG.rectTransform.localScale = new Vector3(0, 0, 0);
        weaponInfoBG.rectTransform.localScale = new Vector3(0, 0, 0);
    }

    private void ShowItems()
    {
        int index = 0;
        foreach(KeyValuePair<int, int> pair in Player.Instance.ItemsNumber)
        {
            while (space[index])
                index++;
            if (itemImages.ContainsKey(pair.Key))
            {
                itemImages[pair.Key].image.transform.GetChild(0).GetComponent<Text>().text = pair.Value.ToString();
                if (pair.Value == 0)
                {
                    space[itemImages[pair.Key].index] = false;
                    Destroy(itemImages[pair.Key].image.gameObject);
                    itemImages.Remove(pair.Key);
                }
                continue;
            }
            Vector2 pos = new Vector2((index % maxColumn) * (1 + spacing) - 1, 3 - (index / maxColumn) * (1 + spacing));
            Image image = Instantiate(iconPrefab, pos, Quaternion.identity, itemBagBG.rectTransform);
            itemImages.Add(pair.Key, new ItemSpace(image, index));
            itemImages[pair.Key].image.rectTransform.localPosition = pos;
            itemImages[pair.Key].image.sprite = Player.Instance.Items[pair.Key].icon;
            itemImages[pair.Key].image.transform.GetChild(0).GetComponent<Text>().text = pair.Value.ToString();
            space[index] = true;
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener(delegate { ShowItemInfo(Player.Instance.Items[pair.Key]); });
            EventTrigger.Entry entry2 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entry2.callback.AddListener(delegate { HideItemInfo(); });
            EventTrigger.Entry entry3 = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry3.callback.AddListener(delegate { KeyboardSetting.Instance.SelectItem(itemImages[pair.Key].image.GetComponent<SettableItem>()); });
            itemImages[pair.Key].image.GetComponent<EventTrigger>().triggers.Add(entry);
            itemImages[pair.Key].image.GetComponent<EventTrigger>().triggers.Add(entry2);
            itemImages[pair.Key].image.GetComponent<EventTrigger>().triggers.Add(entry3);
            itemImages[pair.Key].image.GetComponent<SettableItem>().item = Player.Instance.Items[pair.Key].gameObject;
        }
        foreach (KeyValuePair<int, int> pair in Player.Instance.WeaponNumber)
        {
            while (space[index])
                index++;
            if (!weaponImages.ContainsKey(pair.Key))
                weaponImages[pair.Key] = new List<ItemSpace>();
            for (int i = weaponImages[pair.Key].Count; i < pair.Value; i++)
            {
                while (space[index])
                    index++;
                Vector2 pos = new Vector2((index % maxColumn) * (1 + spacing) - 1, 3 - (index / maxColumn) * (1 + spacing));
                Image image = Instantiate(iconPrefab, pos, Quaternion.identity, itemBagBG.rectTransform);
                weaponImages[pair.Key].Add(new ItemSpace(image, index));
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.rectTransform.localPosition = pos;
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.sprite = Player.Instance.Weapons[pair.Key].icon;
                space[index] = true;
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entry.callback.AddListener(delegate { ShowItemInfo(Player.Instance.Weapons[pair.Key]); });
                EventTrigger.Entry entry2 = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                entry2.callback.AddListener(delegate { HideItemInfo(); });
                EventTrigger.Entry entry3 = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entry3.callback.AddListener(delegate {
                    KeyboardSetting.Instance.SelectItem(weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.GetComponent<SettableItem>());
                    selectedWeapon = weaponImages[pair.Key][weaponImages[pair.Key].Count - 1];
                });
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.GetComponent<EventTrigger>().triggers.Add(entry);
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.GetComponent<EventTrigger>().triggers.Add(entry2);
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.GetComponent<EventTrigger>().triggers.Add(entry3);
                weaponImages[pair.Key][weaponImages[pair.Key].Count - 1].image.GetComponent<SettableItem>().item = Player.Instance.Weapons[pair.Key].gameObject;
            }

            //EventTrigger.Entry entry = new EventTrigger.Entry
            //{
            //    eventID = EventTriggerType.PointerEnter
            //};
            //entry.callback.AddListener(delegate { ShowItemInfo(Player.Instance.Items[pair.Key]); });
            //EventTrigger.Entry entry2 = new EventTrigger.Entry
            //{
            //    eventID = EventTriggerType.PointerExit
            //};
            //entry2.callback.AddListener(delegate { HideItemInfo(); });
            //EventTrigger.Entry entry3 = new EventTrigger.Entry
            //{
            //    eventID = EventTriggerType.PointerClick
            //};
            //entry3.callback.AddListener(delegate { KeyboardSetting.Instance.SelectItem(images[pair.Key].GetComponent<SettableItem>()); });
            //images[pair.Key].GetComponent<EventTrigger>().triggers.Add(entry);
            //images[pair.Key].GetComponent<EventTrigger>().triggers.Add(entry2);
            //images[pair.Key].GetComponent<EventTrigger>().triggers.Add(entry3);
            //images[pair.Key].GetComponent<SettableItem>().item = Player.Instance.Items[pair.Key].gameObject;
        }
    }
}
