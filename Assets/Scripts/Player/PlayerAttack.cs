using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float normalAttackSpeed;
    public List<Skill> normalAttack;
    public List<Skill> shadowSlash;
    public Dictionary<int, Skill> skills;
    public Animator animator;
    public SkillEffect skillEffect;

    private int combo;
    public int Combo
    {
        get
        {
            return combo;
        }
        set
        {
            if (value > 999)
                value = 999;
            combo = value;
            ComboDisplayer.Instance.UpdateCombo(combo);
        }
    }

    public bool IsAttack { get; private set; }

    private Player character;
    //private Rigidbody2D rigid;
    private float normalAttackTimer;
    private int normalAttackCounter;
    private AnimatorStateInfo stateInfo;

    private bool isUseShadowSlash = false;

    private void Awake()
    {
        character = transform.parent.GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        normalAttackTimer = -100;
        normalAttackCounter = 0;
        combo = 0;
        IsAttack = false;
        StartCoroutine("DetecetInput");
        StartCoroutine("RecoverMp");
        skills = new Dictionary<int, Skill>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(2);
        if (stateInfo.IsName("None"))
            IsAttack = false;
        if (!stateInfo.IsName("None"))
            IsAttack = true;
        if (!stateInfo.IsName("None") && stateInfo.normalizedTime > 0.99f)
        {
            normalAttackCounter = 0;
            animator.SetInteger("Attack", normalAttackCounter);
            IsAttack = false;
        }
    }

    IEnumerator RecoverMp()
    {
        while (true)
        {
            if (character.MP < character.MaxMP)
            {
                yield return new WaitForSeconds(1.5f);
                int r = character.MaxMP / 60 > 1 ? -character.MaxMP / 60 : -1;
                if (character.MP - r > character.MaxMP)
                    r = -(character.MaxMP - character.MP);
                character.CostMp(r);
            }
            yield return null;
        }
    }

    IEnumerator DetecetInput()
    {
        while (true)
        {
            if (!KeyboardSetting.isSetting && Time.timeScale != 0)
            {
                foreach (string s in KeyboardSetting.skillKeyTable.Keys)
                {
                    if (Input.GetKey(s))
                    {
                        KeyValue value = (KeyValue)KeyboardSetting.skillKeyTable[s];
                        int id = value.obj.GetComponent<Skill>().id;
                        isUseShadowSlash = false;
                        // Slash
                        if (id == 1)
                        {
                            AttackNormal();
                            yield return null;
                        }
                        // Shadow slash.
                        else if (id == 4)
                        {
                            isUseShadowSlash = true;
                            AttackNormal();
                            yield return null;
                        }
                        else if (id == 8)
                        {
                            UseSkill(8, s, Input.GetKey(KeyCode.UpArrow));
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            UseSkill(value.obj.GetComponent<Skill>().id, s, Input.GetKey(KeyCode.UpArrow));
                            yield return new WaitForSeconds(1 / normalAttackSpeed);
                        }
                    }
                }
                //}
            }
            yield return null;
        }
    }

    public void AttackNormal()
    {
        // Phase 1.
        if (normalAttackCounter == 0)
        {
            if (stateInfo.IsName("None") && Time.time - normalAttackTimer >= 1.0f / normalAttackSpeed)
            {
                if (isUseShadowSlash)
                {
                    Skill skill = shadowSlash[0];
                    if (!skills.ContainsKey(shadowSlash[0].id))
                        return;
                    skill.level = skills[shadowSlash[0].id].level;
                    if (character.MP < skill.mpCost[skill.level - 1])
                        return;
                }
                else
                {
                    if (!skills.ContainsKey(normalAttack[0].id))
                        return;
                }
                normalAttackTimer = Time.time;
                normalAttackCounter = 1;
                animator.SetInteger("Attack", normalAttackCounter);
            }
        }
        // Phase 2.
        else if (stateInfo.IsName("Attack") && normalAttackCounter == 1)
        {
            if (isUseShadowSlash)
            {
                Skill skill = shadowSlash[1];
                if (!skills.ContainsKey(shadowSlash[0].id))
                    return;
                skill.level = skills[shadowSlash[0].id].level;
                if (character.MP < skill.mpCost[skill.level - 1])
                    return;
            }
            else
            {
                if (!skills.ContainsKey(normalAttack[0].id))
                    return;
            }
            normalAttackCounter = 2;
        }
        // Phase 3.
        else if (stateInfo.IsName("Attack2") && normalAttackCounter == 2)
        {
            if (isUseShadowSlash)
            {
                Skill skill = shadowSlash[2];
                if (!skills.ContainsKey(shadowSlash[0].id))
                    return;
                skill.level = skills[shadowSlash[0].id].level;
                if (character.MP < skill.mpCost[skill.level - 1])
                    return;
            }
            else
            {
                if (!skills.ContainsKey(normalAttack[0].id))
                    return;
            }
            normalAttackCounter = 3;
        }
    }

    public void AdvancedNormalAttack()
    {
        animator.SetInteger("Attack", normalAttackCounter);
    }

    public void PlayNormalAttackAnimation(int index)
    {
        Skill skill;
        if (isUseShadowSlash)
        {
            skill = Instantiate(shadowSlash[index]);
            skill.level = skills[shadowSlash[0].id].level;
            character.CostMp(skill.mpCost[skill.level - 1]);
        }
        else
        {
            skill = Instantiate(normalAttack[index]);
            skill.level = skills[normalAttack[0].id].level;
        }
        float x = transform.parent.localScale.x;
        // Player face right.
        if (x < 0)
        {
            skill.transform.position = (Vector2)transform.position + new Vector2(skill.positionXOffset, skill.positionYOffset);
            skill.transform.localRotation = Quaternion.Euler(0, 0, skill.rRotationZ);
            skill.transform.localScale = new Vector2(1, 1);
        }
        // Player face left.
        else if (x > 0)
        {
            skill.transform.position = (Vector2)transform.position + new Vector2(-skill.positionXOffset, skill.positionYOffset);
            skill.transform.localRotation = Quaternion.Euler(0, 0, skill.lRotationZ);
            skill.transform.localScale = new Vector2(-1, 1);
        }
        skill.Use(character);
    }

    public void UseSkill(int index, string key, bool inputUp = false)
    {
        if (skills.ContainsKey(index))
        {
            if (character.MP < skills[index].mpCost[skills[index].level - 1])
                return;
            if (!skills[index].canUse)
                return;
            character.CostMp(skills[index].mpCost[skills[index].level - 1]);
            Skill skill = Instantiate(skills[index]);
            StartCoroutine(CoolDownSkill(skills[index], key) );
            float x = transform.parent.localScale.x;
            // Player face right.
            if (x < 0)
            {
                skill.transform.position = (Vector2)transform.position + new Vector2(skill.positionXOffset, skill.positionYOffset);
                skill.transform.localRotation = Quaternion.Euler(0, 0, skill.rRotationZ);
                skill.transform.localScale = new Vector2(Mathf.Abs(skill.transform.localScale.x), skill.transform.localScale.y);
            }
            // Player face left.
            else if (x > 0)
            {
                skill.transform.position = (Vector2)transform.position + new Vector2(-skill.positionXOffset, skill.positionYOffset);
                skill.transform.localRotation = Quaternion.Euler(0, 0, skill.lRotationZ);
                skill.transform.localScale = new Vector2(-1 * Mathf.Abs(skill.transform.localScale.x), skill.transform.localScale.y);
            }
            animator.SetTrigger("SwordAir");
            skill.gameObject.SetActive(true);
            skill.Use(character);
            if (skill.GetComponent<DisplacementSkill>() != null)
            {
                skill.transform.SetParent(transform);
                if (inputUp)
                    skill.GetComponent<DisplacementSkill>().up = true;
                else
                    skill.GetComponent<DisplacementSkill>().xDirecition = true;
            }
        }
    }

    IEnumerator CoolDownSkill(Skill skill, string key)
    {
        skill.canUse = false;
        float coolTime;
        if (skill.coolTime.Length == 0)
            coolTime = 0;
        else if (skill.level > skill.coolTime.Length)
            coolTime = skill.coolTime[skill.coolTime.Length - 1];
        else
            coolTime = skill.coolTime[skill.level - 1];
        FindObjectOfType<KeySettingShower>().ShowCoolDownEffect(key, coolTime);
        yield return new WaitForSeconds(coolTime);
        if (skill != null)
            skill.canUse = true;
    }
}
