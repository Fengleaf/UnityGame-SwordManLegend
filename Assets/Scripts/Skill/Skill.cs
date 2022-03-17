using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int id;
    public string skillName;
    public Skill[] preSkill;
    public int level;
    public int maxLevel;
    public int[] levelUpSp;
    public int[] mpCost;
    public float[] coolTime;
    public bool isPhysics;
    public bool isMagic;
    public float[] baseDamage;
    public float[] atkCoefficient;
    public float[] matkCoefficient;
    public float floatingPercent;
    public Animator animator;
    public string animationTriggerName;
    public float[] comboDelay;
    public int[] maxCombo = { 1 };
    public int[] maxTargetNumber = { 1 };

    public float positionXOffset;
    public float positionYOffset;
    public float rRotationZ;
    public float lRotationZ;

    public bool canUse = true;

    public AudioClip useAudios;
    public AudioClip[] hitAudios;
    private AudioSource audioSource;

    private bool shouldDestroy;

    private Character user;
    public Character User { get { return user; } }
    private int hitNumber;

    public void Use(Character user)
    {
        audioSource = GetComponent<AudioSource>();
        this.user = user;
        PlayAnimation();
        hitNumber = 0;
    }

    /// Start to judge if skill hit the target then compute damage.
    /// * reset: If 1, target number will compute from 0, otherwise, start from last judge.
    public void JudgeHit(int reset = 0)
    {
        Collider2D[] hits;
        if (reset == 1)
        {
            hits = new Collider2D[maxTargetNumber[level - 1]];
            hitNumber = 0;
        }
        else
            hits = new Collider2D[maxTargetNumber[level - 1] - hitNumber];
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        LayerMask layerMask;
        if (user.IsPlayer())
            layerMask = LayerMask.GetMask("Enemy");
        else
            layerMask = LayerMask.GetMask("Player");
        contactFilter2D.SetLayerMask(layerMask);
        int number = GetComponent<Collider2D>().OverlapCollider(contactFilter2D, hits);
        for (int i = 0; i < number; i++)
        {
            if (hits[i] != null)
            {
                if (hits[i].GetComponent<Character>().IsPlayer() != user.IsPlayer())
                {
                    hitNumber++;
                    StartCoroutine("Damage", hits[i].GetComponent<Character>());
                }
            }
        }
    }

    public void JudgeHit(Character[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null && targets[i].GetComponent<Character>().IsPlayer() != user.IsPlayer())
            {
                hitNumber++;
                StartCoroutine("Damage", targets[i].GetComponent<Character>());
            }
        }
    }

    private void PlayAnimation()
    {
        if (animationTriggerName != "")
            animator.SetTrigger(animationTriggerName);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    IEnumerator Damage(Character target)
    {
        float[] damages = new float[maxCombo[level - 1]];
        bool[] criticals = new bool[maxCombo[level - 1]];

        float totalDamage = 0;
        float totalDelay = 0;

        for (int combo = 0; combo < maxCombo[level - 1]; combo++)
        {
            damages[combo] = ComputeDagame(target, ref criticals[combo]);
            totalDamage += damages[combo];
            if (combo < comboDelay.Length)
                totalDelay += comboDelay[combo];
        }
        if (user.IsPlayer())
            StartCoroutine(DisplayDamage(damages, criticals, target.transform.position));
        if (target.IsPlayer() || target.CompareTag("Boss"))
            target.Damage((int)totalDamage);
        else
        {
            if (target.Damage((int)totalDamage, totalDelay + Time.deltaTime * maxCombo[level - 1]))
            {
                user.GetComponent<Player>().GetExp(target.GetComponent<Monster>().exp);
            }
        }
        yield return null;
    }

    IEnumerator DisplayDamage(float[] damages, bool[] criticals, Vector2 position)
    {
        float delay = 0;
        for (int combo = 0; combo < maxCombo[level - 1]; combo++)
        {
            if (combo >= comboDelay.Length)
                delay = 0;
            else
                delay = comboDelay[combo];
            yield return new WaitForSeconds(delay);
            // Do combo display.
            if (audioSource != null)
            {
                if (combo < hitAudios.Length)
                {
                    audioSource.clip = hitAudios[combo];
                    audioSource.Play();
                }
            }
            user.GetComponent<Player>().playerAttack.Combo++;
            user.GetComponent<Player>().damageDisplayer.DisplayDamage((int)damages[combo], position, criticals[combo]);
        }
    }

    private int ComputeDagame(Character target, ref bool critical)
    {
        // Get base damage.
        float damage = 0;
        if (baseDamage.Length >= level)
            damage = baseDamage[level - 1];
        // Plus attack.
        if (atkCoefficient.Length >= level)
            damage += user.Atk * atkCoefficient[level - 1];
        // Plus magic attack.
        if (matkCoefficient.Length >= level)
            damage += user.MAtk * matkCoefficient[level - 1];
        // Critical hit.
        if (Random.Range(0.0f, 100.0f) < user.Crihit)
        {
            damage *= user.Criatk / 100.0f;
            critical = true;
        }
        // If user is player, add damage according to player's combo.
        if (User.IsPlayer())
            damage += damage * User.GetComponent<Player>().playerAttack.Combo / 200;
        // Floating.
        damage *= Random.Range(1 - floatingPercent / 100.0f, 1 + floatingPercent / 100.0f);

        // Minus target's defence.
        if (isPhysics)
            damage -= target.Def * 0.5f;
        if (isMagic)
            damage -= target.MDef * 0.65f;
        if (damage < 0)
            damage = 0;
        return (int)damage;
    }

    public static void GetSkillDescription(Skill skill, ref string s1, ref string s2)
    {
        int baseDamage;
        float atkCoefficient;
        float matkCoefficient;
        int maxCombo;
        int maxTargetNumber;
        BuffSkill buff;
        float duration;
        float[] twoAtks;
        int[] twoMaxCombos;
        int[] twoMaxTargets;
        float[] thrAtks;
        int[] thrMaxCombos;
        int[] thrMaxTargets;

        switch (skill.id)
        {
            // Slash.
            case 1:
                // Information for two and three hit.
                twoAtks = new float[] { 2.2f, 2.4f, 2.6f, 2.8f, 3.0f };
                twoMaxCombos = new int[] { 1, 1, 1, 1, 1 };
                twoMaxTargets = new int[] { 1, 1, 1, 1, 1 };
                thrAtks = new float[] { 2.5f, 2.8f, 3.1f, 3.4f, 3.7f };
                thrMaxCombos = new int[] { 2, 2, 2, 2, 2 };
                thrMaxTargets = new int[] { 1, 1, 1, 1, 1 };
                if (skill.level == 0)
                {
                    atkCoefficient = skill.atkCoefficient[0];
                    maxCombo = skill.maxCombo[0];
                    maxTargetNumber = skill.maxTargetNumber[0];
                    s1 = "基本的攻擊，用劍對敵人造成傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[0] + "名敵人造成" + (twoAtks[0] * 100) + "%攻擊力傷害" + twoMaxCombos[0] + "次。\n三打: 對最多" + thrMaxTargets[0] + "名敵人造成" + (thrAtks[0] * 100) + "%攻擊力傷害" + thrMaxCombos[0] + "次。\n";
                }
                else
                {
                    atkCoefficient = skill.atkCoefficient[skill.level - 1];
                    maxCombo = skill.maxCombo[skill.level - 1];
                    maxTargetNumber = skill.maxTargetNumber[skill.level - 1];
                    s1 = "基本的攻擊，用劍對敵人造成傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[skill.level - 1] + "名敵人造成" + (twoAtks[skill.level - 1] * 100) + "%攻擊力傷害" + twoMaxCombos[skill.level - 1] + "次。\n三打: 對最多" + thrMaxTargets[skill.level - 1] + "名敵人造成" + (thrAtks[skill.level - 1] * 100) + "%攻擊力傷害" + thrMaxCombos[skill.level - 1] + "次。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkCoefficient = skill.atkCoefficient[skill.level];
                        maxCombo = skill.maxCombo[skill.level];
                        maxTargetNumber = skill.maxTargetNumber[skill.level];
                        s2 = "基本的攻擊，用劍對敵人造成傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[skill.level] + "名敵人造成" + (twoAtks[skill.level] * 100) + "%攻擊力傷害" + twoMaxCombos[skill.level] + "次。\n三打: 對最多" + thrMaxTargets[skill.level] + "名敵人造成" + (thrAtks[skill.level] * 100) + "%攻擊力傷害" + thrMaxCombos[skill.level] + "次。\n";
                    }
                }
                break;
            // Sword Air.
            case 2:
                if (skill.level == 0)
                {
                    atkCoefficient = skill.atkCoefficient[0];
                    maxCombo = skill.maxCombo[0];
                    maxTargetNumber = skill.maxTargetNumber[0];
                    s1 = "快速揮動劍以產生強大的劍氣攻擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                }
                else
                {
                    atkCoefficient = skill.atkCoefficient[skill.level - 1];
                    maxCombo = skill.maxCombo[skill.level - 1];
                    maxTargetNumber = skill.maxTargetNumber[skill.level - 1];
                    s1 = "快速揮動劍以產生強大的劍氣攻擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkCoefficient = skill.atkCoefficient[skill.level];
                        maxCombo = skill.maxCombo[skill.level];
                        maxTargetNumber = skill.maxTargetNumber[skill.level];
                        s2 = "快速揮動劍以產生強大的劍氣攻擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    }
                }
                break;
            // Instant Slash.
            case 3:
                if (skill.level == 0)
                {
                    atkCoefficient = skill.atkCoefficient[0];
                    maxCombo = skill.maxCombo[0];
                    maxTargetNumber = skill.maxTargetNumber[0];
                    s1 = "以迅雷不及掩耳的速度斬擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                }
                else
                {
                    atkCoefficient = skill.atkCoefficient[skill.level - 1];
                    maxCombo = skill.maxCombo[skill.level - 1];
                    maxTargetNumber = skill.maxTargetNumber[skill.level - 1];
                    s1 = "以迅雷不及掩耳的速度斬擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkCoefficient = skill.atkCoefficient[skill.level];
                        maxCombo = skill.maxCombo[skill.level];
                        maxTargetNumber = skill.maxTargetNumber[skill.level];
                        s2 = "以迅雷不及掩耳的速度斬擊敵人。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    }
                }
                break;
            // Shadow Slash.
            case 4:
                // Information for two and three hit.
                twoAtks = new float[] { 1.7f, 2.0f, 2.2f, 2.4f, 2.7f };
                twoMaxCombos = new int[] { 2, 2, 2, 2, 2 };
                twoMaxTargets = new int[] { 3, 3, 3, 3, 3 };
                thrAtks = new float[] { 2.5f, 2.7f, 2.9f, 3.2f, 3.5f };
                thrMaxCombos = new int[] { 3, 3, 3, 3, 3 };
                thrMaxTargets = new int[] { 4, 4, 4, 4, 4 };
                if (skill.level == 0)
                {
                    atkCoefficient = skill.atkCoefficient[0];
                    maxCombo = skill.maxCombo[0];
                    maxTargetNumber = skill.maxTargetNumber[0];
                    s1 = "附加幻影之力的斬擊，能對敵人造成更大的傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[0] + "名敵人造成" + (twoAtks[0] * 100) + "%攻擊力傷害" + twoMaxCombos[0] + "次。\n三打: 消耗MP" + (skill.mpCost[0] + 5) + "，對最多" + thrMaxTargets[0] + "名敵人造成" + (thrAtks[0] * 100) + "%攻擊力傷害" + thrMaxCombos[0] + "次。\n";
                }
                else
                {
                    atkCoefficient = skill.atkCoefficient[skill.level - 1];
                    maxCombo = skill.maxCombo[skill.level - 1];
                    maxTargetNumber = skill.maxTargetNumber[skill.level - 1];
                    s1 = "附加幻影之力的斬擊，能對敵人造成更大的傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[skill.level - 1] + "名敵人造成" + (twoAtks[skill.level - 1] * 100) + "%攻擊力傷害" + twoMaxCombos[skill.level - 1] + "次。\n三打: 消耗MP " + (skill.mpCost[skill.level - 1] + 5) + "，對最多" + thrMaxTargets[skill.level - 1] + "名敵人造成" + (thrAtks[skill.level - 1] * 100) + "%攻擊力傷害" + thrMaxCombos[skill.level - 1] + "次。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkCoefficient = skill.atkCoefficient[skill.level];
                        maxCombo = skill.maxCombo[skill.level];
                        maxTargetNumber = skill.maxTargetNumber[skill.level];
                        s2 = "附加幻影之力的斬擊，能對敵人造成更大的傷害，可以三連打。\n一打: 對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n二打: 對最多" + twoMaxTargets[skill.level] + "名敵人造成" + (twoAtks[skill.level] * 100) + "%攻擊力傷害" + twoMaxCombos[skill.level] + "次。\n三打: 消耗MP" + (skill.mpCost[skill.level] + 10) + "，對最多" + thrMaxTargets[skill.level] + "名敵人造成" + (thrAtks[skill.level] * 100) + "%攻擊力傷害" + thrMaxCombos[skill.level] + "次。\n";
                    }
                }
                break;
            // Speed
            case 7:
                buff = skill.GetComponent<BuffSkill>();
                float speedBuff;
                if (skill.level == 0)
                {
                    speedBuff = buff.speedValue[0];
                    duration = buff.duration[0];
                    s1 = "借助風的力量提升自己的移動速度。\n" + duration + "秒內提升移動速度" + speedBuff + "。\n";
                }
                else
                {
                    speedBuff = buff.speedValue[skill.level - 1];
                    duration = buff.duration[skill.level - 1];
                    s1 = "借助風的力量提升自己的移動速度。\n" + duration + "秒內提升移動速度" + speedBuff + "。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        speedBuff = buff.speedValue[skill.level];
                        duration = buff.duration[skill.level];
                        s2 = "借助風的力量提升自己的移動速度。\n" + duration + "秒內提升移動速度" + speedBuff + "。\n";
                    }
                }
                break;
            // Assault
            case 8:
                if (skill.level == 0)
                {
                    atkCoefficient = skill.atkCoefficient[0];
                    maxCombo = skill.maxCombo[0];
                    maxTargetNumber = skill.maxTargetNumber[0];
                    s1 = "快速向前方衝刺並對碰到的敵人造成傷害，同時按上或下方向鍵可以往該方向移動。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                }
                else
                {
                    atkCoefficient = skill.atkCoefficient[skill.level - 1];
                    maxCombo = skill.maxCombo[skill.level - 1];
                    maxTargetNumber = skill.maxTargetNumber[skill.level - 1];
                    s1 = "快速向前方衝刺並對碰到的敵人造成傷害，同時按上或下方向鍵可以往該方向移動。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkCoefficient = skill.atkCoefficient[skill.level];
                        maxCombo = skill.maxCombo[skill.level];
                        maxTargetNumber = skill.maxTargetNumber[skill.level];
                        s2 = "快速向前方衝刺並對碰到的敵人造成傷害，同時按上或下方向鍵可以往該方向移動。\n對最多" + maxTargetNumber + "名敵人造成" + (atkCoefficient * 100) + "%攻擊力傷害" + maxCombo + "次。\n";
                    }
                }
                break;
            // Sword Strengthen
            case 9:
                buff = skill.GetComponent<BuffSkill>();
                int atkBuff;
                if (skill.level == 0)
                {
                    atkBuff = buff.atkValue[0];
                    duration = buff.duration[0];
                    s1 = "強化自己的武器以增加攻擊力。\n" + duration + "秒內提升攻擊力" + atkBuff + "。\n";
                }
                else
                {
                    atkBuff = buff.atkValue[skill.level - 1];
                    duration = buff.duration[skill.level - 1];
                    s1 = "強化自己的武器以增加攻擊力。\n" + duration + "秒內提升攻擊力" + atkBuff + "。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        atkBuff = buff.atkValue[skill.level];
                        duration = buff.duration[skill.level];
                        s2 = "強化自己的武器以增加攻擊力。\n" + duration + "秒內提升攻擊力" + atkBuff + "。\n";
                    }
                }
                break;
            // Sword Guard
            case 10:
                buff = skill.GetComponent<BuffSkill>();
                int defBuff;
                int mdefBuff;
                if (skill.level == 0)
                {
                    defBuff = buff.defValue[0];
                    mdefBuff = buff.mdefValue[0];
                    duration = buff.duration[0];
                    s1 = "將劍之氣息圍繞自身以增加防禦力。\n" + duration + "秒內提升物理防禦力" + defBuff + "、魔法防禦力" + mdefBuff + "。\n";
                }
                else
                {
                    defBuff = buff.defValue[skill.level - 1];
                    mdefBuff = buff.mdefValue[skill.level - 1];
                    duration = buff.duration[skill.level - 1];
                    s1 = "將劍之氣息圍繞自身以增加防禦力。\n" + duration + "秒內提升物理防禦力" + defBuff + "、魔法防禦力" + mdefBuff + "。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        defBuff = buff.defValue[skill.level];
                        mdefBuff = buff.mdefValue[skill.level];
                        duration = buff.duration[skill.level];
                        s2 = "將劍之氣息圍繞自身以增加防禦力。\n" + duration + "秒內提升物理防禦力" + defBuff + "、魔法防禦力" + mdefBuff + "。\n";
                    }
                }
                break;
            // Violent Breath
            case 11:
                buff = skill.GetComponent<BuffSkill>();
                float crihitBuff;
                float criatkBuff;
                if (skill.level == 0)
                {
                    crihitBuff = buff.crihitValue[0];
                    criatkBuff = buff.criatkValue[0];
                    duration = buff.duration[0];
                    s1 = "激發潛在的強大力量增加爆擊機率與爆擊傷害。\n" + duration + "秒內提升爆擊率" + crihitBuff + "%、爆擊傷害" + criatkBuff + "。\n";
                }
                else
                {
                    crihitBuff = buff.crihitValue[skill.level - 1];
                    criatkBuff = buff.criatkValue[skill.level - 1];
                    duration = buff.duration[skill.level - 1];
                    s1 = "激發潛在的強大力量增加爆擊機率與爆擊傷害。\n" + duration + "秒內提升爆擊率" + crihitBuff + "%、爆擊傷害" + criatkBuff + "。\n";
                    if (skill.level < skill.maxLevel)
                    {
                        crihitBuff = buff.crihitValue[skill.level];
                        criatkBuff = buff.criatkValue[skill.level];
                        duration = buff.duration[skill.level];
                        s2 = "激發潛在的強大力量增加爆擊機率與爆擊傷害。\n" + duration + "秒內提升爆擊率" + crihitBuff + "%、爆擊傷害" + criatkBuff + "。\n";
                    }
                }
                break;
        }
    }
}
