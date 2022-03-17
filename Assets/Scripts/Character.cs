using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InitialAbility
{
    public int initialLevel;
    public int maxHp;
    public int maxMp;
    public int atk;
    public int def;
    public int matk;
    public int mdef;
    public float crihit;
    public float criatk;
    public float maxSpeed;
}

public abstract class Character : MonoBehaviour
{
    public InitialAbility initialAbility;
    public int LV { get; protected set; }
    public int HP { get; protected set; }
    public int MaxHP { get; protected set; }
    public int MP { get; protected set; }
    public int MaxMP { get; protected set; }
    public int Atk { get; protected set; }
    public int Def { get; protected set; }
    public int MAtk { get; protected set; }
    public int MDef { get; protected set; }
    public float Crihit { get; protected set; }
    public float Criatk { get; protected set; }
    public float Speed { get; protected set; }
    public int right = -1;
    public List<string> Buffs { get; protected set; }
    protected List<Hashtable> buffTables;
    private List<Coroutine> buffsCoroutine;

    protected virtual void Awake()
    {
        LV = initialAbility.initialLevel;
        HP = initialAbility.maxHp;
        MaxHP = initialAbility.maxHp;
        MP = initialAbility.maxMp;
        MaxMP = initialAbility.maxMp;
        Atk = initialAbility.atk;
        Def = initialAbility.def;
        MAtk = initialAbility.matk;
        MDef = initialAbility.mdef;
        Crihit = initialAbility.crihit;
        Criatk = initialAbility.criatk;
        Speed = initialAbility.maxSpeed;
        Buffs = new List<string>();
        buffTables = new List<Hashtable>();
        buffsCoroutine = new List<Coroutine>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public abstract bool IsPlayer();

    public virtual void Damage(int value)
    {
        HP -= value;
        if (HP < 0)
            HP = 0;
        else if (HP > MaxHP)
            HP = MaxHP;
    }

    public virtual bool Damage(int value, float delayToDie)
    {
        HP -= value;
        return false;
    }

    public virtual void CostMp(int value)
    {
        MP -= value;
        if (MP < 0)
            MP = 0;
        else if (MP > MaxMP)
            MP = MaxMP;
    }

    public virtual void AddBuff(Hashtable buffTable)
    {
        string name = (string)buffTable["name"];
        // Already has the same buff, override.
        if (Buffs.Contains(name))
        {
            DeleteBuff(buffTables[Buffs.IndexOf(name)]);
        }
        // New buff.
        if (buffTable.ContainsKey("atk"))
            Atk += (int)buffTable["atk"];
        if (buffTable.ContainsKey("def"))
            Def += (int)buffTable["def"];
        if (buffTable.ContainsKey("matk"))
            MAtk  += (int)buffTable["matk"];
        if (buffTable.ContainsKey("mdef"))
            MDef  += (int)buffTable["mdef"];
        if (buffTable.ContainsKey("crihit"))
            Crihit += (float)buffTable["crihit"];
        if (buffTable.ContainsKey("criatk"))
            Criatk += (float)buffTable["criatk"];
        if (buffTable.ContainsKey("speed"))
            Speed += (float)buffTable["speed"];                       
        Buffs.Add(name);
        buffTables.Add(buffTable);
        buffsCoroutine.Add(StartCoroutine("WaitDeleteBuff", buffTable));
    }                                                           

    public virtual void DeleteBuff(Hashtable buffTable)
    {
        string name = (string)buffTable["name"];
        if (Buffs.Contains(name))
        {
            int index = Buffs.IndexOf(name);
            Buffs.RemoveAt(index);
            StopCoroutine(buffsCoroutine[index]);
            buffsCoroutine.RemoveAt(index);
            if (buffTable.ContainsKey("atk"))
                Atk -= (int)buffTables[index]["atk"];
            if (buffTable.ContainsKey("def"))
                Def -= (int)buffTables[index]["def"];
            if (buffTable.ContainsKey("matk"))
                MAtk -= (int)buffTables[index]["matk"];
            if (buffTable.ContainsKey("mdef"))
                MDef -= (int)buffTables[index]["mdef"];
            if (buffTable.ContainsKey("crihit"))
                Crihit -= (float)buffTables[index]["crihit"];
            if (buffTable.ContainsKey("criatk"))
                Criatk -= (float)buffTables[index]["criatk"];
            if (buffTable.ContainsKey("speed"))
                Speed -= (float)buffTables[index]["speed"];
            buffTables.RemoveAt(index);
        }
    }

    IEnumerator WaitDeleteBuff(Hashtable buffTable)
    {
        yield return new WaitForSeconds((float)buffTable["duration"]);
        DeleteBuff(buffTable);
    }
}
