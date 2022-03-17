using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    public PlayerMove playerMove;
    public PlayerAttack playerAttack;
    public DamageDisplayer damageDisplayer;
    public LevelUpDisplayer levelUpDisplayer;
    public int[] nextExps;
    public int SkillPoint { get; set; }
    public int Exp { get; private set; }
    private Rigidbody2D rigid;

    private static Player instance;

    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(Player)) as Player;
                if (instance == null)
                {
                    GameObject go = new GameObject("Displayer");
                    instance = Instantiate(GameManager.instance.player);
                }
            }
            return instance;
        }
    }

    public Dictionary<int, int> ItemsNumber { get; private set; }
    public Dictionary<int, Item> Items { get; private set; }
    public Dictionary<int, int> WeaponNumber { get; private set; }
    public Dictionary<int, Weapon> Weapons { get; private set; }
    public Weapon EquippedWeapon { get; private set; }

    [SerializeField]
    private Weapon initialWeapon;
    [SerializeField]
    private SpriteRenderer weaponImageOnHand = new SpriteRenderer();

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        ItemsNumber = new Dictionary<int, int>();
        Items = new Dictionary<int, Item>();
        WeaponNumber = new Dictionary<int, int>();
        Weapons = new Dictionary<int, Weapon>();
        rigid = GetComponent<Rigidbody2D>();
        Exp = 0;
        SkillPoint = 4;
        WeaponNumber[initialWeapon.id] = 0;
        Weapons[initialWeapon.id] = initialWeapon;
        EquippedWeapon = initialWeapon;
        Atk += initialWeapon.atkValue;
        Crihit += initialWeapon.crihitValue;
        Criatk += initialWeapon.criatkValue;
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        StartCoroutine("DetecetInput");
        StartCoroutine("PickUpItemInput");
    }

    IEnumerator DetecetInput()
    {
        while (true)
        {
            if (!KeyboardSetting.isSetting && Time.timeScale != 0)
            {
                foreach (string s in KeyboardSetting.itemKeyTable.Keys)
                {
                    
                    if (Input.GetKey(s))
                    {
                        KeyValue value = (KeyValue)KeyboardSetting.itemKeyTable[s];
                        UseItem(value.obj.GetComponent<Item>(), s);
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator PickUpItemInput()
    {
        while (true)
        {
            if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.Pick) && Time.timeScale != 0)
            {
                if (Input.GetKey(KeyboardSetting.specialKeyTable[ValueType.Pick].key.key))
                {
                    ContactFilter2D contactFilter2D = new ContactFilter2D();
                    LayerMask layerMask = LayerMask.GetMask("Item");
                    contactFilter2D.SetLayerMask(layerMask);
                    Collider2D[] collider2Ds = new Collider2D[1];
                    if (GetComponent<CapsuleCollider2D>().OverlapCollider(contactFilter2D, collider2Ds) == 1)
                    {
                        if (collider2Ds[0].CompareTag("Item"))
                            GetItem(collider2Ds[0].gameObject.GetComponent<Item>());
                        else if (collider2Ds[0].CompareTag("Weapon"))
                            GetItem(collider2Ds[0].gameObject.GetComponent<Weapon>());
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return null;
        }
    }

    public override bool IsPlayer()
    {
        return true;
    }

    public override void Damage(int value)
    {
        base.Damage(value);
        FindObjectOfType<StateWindow>().Refrech();
        if (value > 0 )
            playerAttack.Combo = 0;
        if (HP <= 0)
            GameManager.instance.StartCoroutine("HandlePlayerDie");
    }

    public override void CostMp(int value)
    {
        base.CostMp(value);
        FindObjectOfType<StateWindow>().Refrech();
    }

    public void GetExp(int value)
    {
        if (LV == nextExps.Length)
            return;
        Exp += value;
        while (Exp >= nextExps[LV - 1])
        {
            Exp -= nextExps[LV - 1];
            LV += 1;
            MaxHP += (int)(MaxHP * 0.05f + LV * 50.0f);
            HP = MaxHP;
            MaxMP += 50;
            MP = MaxMP;
            SkillPoint += 5;
            FindObjectOfType<SkillTree>().skillPoint.text = SkillPoint.ToString();
            levelUpDisplayer.StartShow();
            if (LV == nextExps.Length)
                return;
        }
        FindObjectOfType<StateWindow>().Refrech();
    }

    public void LearnSkill(Skill skill)
    {
        playerAttack.skills[skill.id] = skill;
    }

    public void GetItem(Item item)
    {
        if (Items.Count + Weapons.Count == 21)
            return;
        if (ItemsNumber.ContainsKey(item.id))
        {
            ItemsNumber[item.id] = ItemsNumber[item.id] + 1;
            Destroy(item.gameObject);
        }
        else
        {
            ItemsNumber[item.id] = 1;
            item.gameObject.SetActive(false);
            Items.Add(item.id, item);
        }
        item.transform.SetParent(transform);
        FindObjectOfType<ItemBag>().Refrech();
    }

    public void UseItem(Item item, string key)
    {
        if (!Items.ContainsKey(item.id))
            return;
        if (!item.CanUse)
            return;
        StartCoroutine(CoolDownItem(item, key));
        item.Use(GetComponent<Character>());
        ItemsNumber[item.id]--;
        FindObjectOfType<ItemBag>().Refrech();
        if (ItemsNumber[item.id] == 0)
        {
            Items.Remove(item.id);
            ItemsNumber.Remove(item.id);
        }
    }

    public void GetItem(Weapon weapon)
    {
        if (Items.Count + Weapons.Count == 21)
            return;
        if (Weapons.ContainsKey(weapon.id))
        {
            WeaponNumber[weapon.id] = WeaponNumber[weapon.id] + 1;
            Destroy(weapon.gameObject);
        }
        else
        {
            WeaponNumber[weapon.id] = 1;
            weapon.gameObject.SetActive(false);
            Weapons.Add(weapon.id, weapon);
        }
        weapon.transform.SetParent(transform);
        FindObjectOfType<ItemBag>().Refrech();
    }

    public void EquipWeapon(Weapon weapon)
    {
        // Already has a weapon equipped, substitute.
        if (EquippedWeapon != null)
        {
            WeaponNumber[EquippedWeapon.id] = WeaponNumber[EquippedWeapon.id] + 1;
            Atk -= EquippedWeapon.atkValue;
            Crihit -= EquippedWeapon.crihitValue;
            Criatk -= EquippedWeapon.criatkValue;
        }
        EquippedWeapon = weapon;
        WeaponNumber[weapon.id]--;
        Atk += weapon.atkValue;
        Crihit += weapon.crihitValue;
        Criatk += weapon.criatkValue;
        FindObjectOfType<ItemBag>().Refrech(true);
        FindObjectOfType<StateWindow>().Refrech();
        weaponImageOnHand.sprite = weapon.icon;
    }

    public override void AddBuff(Hashtable buffTable)
    {
        base.AddBuff(buffTable);
        StateWindow stateWindow = FindObjectOfType<StateWindow>();
        stateWindow.Refrech();
        stateWindow.AddBuffIcon((string)buffTable["name"], (Sprite)buffTable["icon"], (float)buffTable["duration"]);
    }

    public override void DeleteBuff(Hashtable buffTable)
    {
        base.DeleteBuff(buffTable);
        FindObjectOfType<StateWindow>().Refrech();
    }

    IEnumerator CoolDownItem(Item item, string key)
    {
        item.CanUse = false;
        float coolTime = item.coolTime;
        FindObjectOfType<KeySettingShower>().ShowCoolDownEffect(key, coolTime);
        yield return new WaitForSeconds(coolTime);
        if (item != null)
            item.CanUse = true;
    }
}
