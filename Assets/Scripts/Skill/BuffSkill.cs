using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Skill))]
public class BuffSkill : MonoBehaviour
{
    public int[] atkValue;
    public int[] defValue;
    public int[] matkValue;
    public int[] mdefValue;
    public float[] crihitValue;
    public float[] criatkValue;
    public float[] speedValue;
    public float[] duration;
    public Skill Skill { get; private set; }
    public Sprite icon;

    // Start is called before the first frame update
    void Start()
    {
        Skill = GetComponent<Skill>();
        Hashtable buffTable = new Hashtable()
        {
            { "name", Skill.skillName }
        };
        if (atkValue.Length >= Skill.level)
            buffTable.Add("atk", atkValue[Skill.level - 1]);
        if (defValue.Length >= Skill.level)
            buffTable.Add("def", defValue[Skill.level - 1]);
        if (matkValue.Length >= Skill.level)
            buffTable.Add("matk", matkValue[Skill.level - 1]);
        if (mdefValue.Length >= Skill.level)
            buffTable.Add("mdef", mdefValue[Skill.level - 1]);
        if (crihitValue.Length >= Skill.level)
            buffTable.Add("crihit", crihitValue[Skill.level - 1]);
        if (criatkValue.Length >= Skill.level)
            buffTable.Add("criatk", criatkValue[Skill.level - 1]);
        if (speedValue.Length >= Skill.level)
            buffTable.Add("speed", speedValue[Skill.level - 1]);
        if (duration.Length >= Skill.level)
            buffTable.Add("duration", duration[Skill.level - 1]);
        if (icon != null)
            buffTable.Add("icon", icon);
        Skill.User.AddBuff(buffTable);
    }
}
