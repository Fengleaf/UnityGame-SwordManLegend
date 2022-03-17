using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Character
{
    public Queen queen;

    public Animator animator;
    public MonsterInfoShower infoShower;

    public Skill turnadoPrefab;
    public Skill kingSlashPrefab;
    public Skill kingFireGunPrefab;
    public Skill kingSlashMovePrefab;
    public Skill kingQueenExchangePrefab;

    public bool UsingFireGun { get; private set; }
    public bool CanProtectQueen { get; private set; }

    private Skill turnado;
    private Skill kingSlash;
    private Skill kingFireGun;
    private Skill kingSlashMove;
    private Skill kingQueenExchange;

    private bool isGround = true;
    private bool protectingQueen;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InitialSkill();
        StartCoroutine("Attack");
        UsingFireGun = false;
        CanProtectQueen = true;
    }

    private void InitialSkill()
    {
        turnado = Instantiate(turnadoPrefab);
        turnado.level = LV;
        turnado.gameObject.SetActive(false);
        kingSlash = Instantiate(kingSlashPrefab);
        kingSlash.level = LV;
        kingSlash.gameObject.SetActive(false);
        kingFireGun = Instantiate(kingFireGunPrefab);
        kingFireGun.level = LV;
        kingFireGun.gameObject.SetActive(false);
        kingSlashMove = Instantiate(kingSlashMovePrefab);
        kingSlashMove.level = LV;
        kingSlashMove.gameObject.SetActive(false);
        kingQueenExchange = Instantiate(kingQueenExchangePrefab);
        kingQueenExchange.level = LV;
        kingQueenExchange.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (transform.localScale.x * right < 0)
            infoShower.Flip(true);
        else
            infoShower.Flip(false);
    }

    public override bool IsPlayer()
    {
        return false;
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1);
        protectingQueen = false;
        // First use turnado once.
        StartCoroutine("UseTurnado");
        StartCoroutine("UseFireGun");
        yield return new WaitForSeconds(1);
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        //StartCoroutine("UseDemonSwordAir");
        while (true)
        {
            if (!protectingQueen && ! UsingFireGun && Time.timeScale != 0)
            {
                animator.SetBool("Walk", false);
                Vector2 v = Player.Instance.transform.position - transform.position;
                float d = v.magnitude;
                if (d > 5)
                {
                    if (kingSlashMove.canUse)
                    {
                        StartCoroutine(UseSlashMove());
                    }
                    else if (turnado.canUse)
                        StartCoroutine("UseTurnado");
                    else if (isGround)
                    {
                        Vector2 scale = transform.localScale;
                        animator.SetBool("Walk", true);
                        if (v.x > 0)
                        {
                            scale.x = right;
                            rigidbody2D.velocity = new Vector2(Speed, rigidbody2D.velocity.y);
                        }
                        else
                        {
                            scale.x = -right;
                            rigidbody2D.velocity = new Vector2(-Speed, rigidbody2D.velocity.y);
                        }
                        transform.localScale = scale;
                    }
                }
                else if (d >= 2.5)
                {
                    if (isGround)
                    {
                        Vector2 scale = transform.localScale;
                        animator.SetBool("Walk", true);
                        if (v.x > 0)
                        {
                            scale.x = right;
                            rigidbody2D.velocity = new Vector2(Speed, rigidbody2D.velocity.y);
                        }
                        else
                        {
                            scale.x = -right;
                            rigidbody2D.velocity = new Vector2(-Speed, rigidbody2D.velocity.y);
                        }
                        transform.localScale = scale;
                    }
                }
                else if (d < 2.5)
                {
                    if (d < 1.7f && kingSlashMove.canUse)
                    {
                        StartCoroutine("UseSlashMove");
                        yield return new WaitForSeconds(0.25f);
                        StartCoroutine("UseSlashMove");
                        yield return new WaitForSeconds(0.25f);
                    }
                    else if (kingSlash.canUse)
                    {
                        StartCoroutine("UseKingSlsh");
                    }
                }
            }
            yield return null;
        }
    }

    //IEnumerator DetectOutOfBound()
    //{
    //    while (true)
    //    {
    //        if (transform.position.y > 6)
    //        {
    //            transform.position = new Vector2(transform.position.x, -4.184577f);
    //            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    //            isGround = true;
    //            StartCoroutine("CoolDownSkill", shadowMove);
    //        }
    //        if (transform.position.y < -4.5)
    //        {
    //            transform.position = new Vector2(transform.position.x, -4.184577f);
    //            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    //            isGround = true;
    //            StartCoroutine("CoolDownSkill", shadowMove);
    //        }
    //        if (transform.position.x > 12)
    //        {
    //            transform.position = new Vector2(11.72f, transform.position.y);
    //            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    //            isGround = true;
    //            StartCoroutine("CoolDownSkill", shadowMove);
    //        }
    //        if (transform.position.x < -13)
    //        {
    //            transform.position = new Vector2(-12f, transform.position.y);
    //            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    //            isGround = true;
    //            StartCoroutine("CoolDownSkill", shadowMove);
    //        }
    //        yield return null;
    //    }
    //}

    IEnumerator UseSlashMove()
    {
        StartCoroutine("CoolDownSkill", kingSlashMove);
        Vector2 scale = transform.localScale;
        if (Player.Instance.transform.position.x > transform.position.x)
            scale.x = right;
        else
            scale.x = -right;
        transform.localScale = scale;
        Skill skill = Instantiate(kingSlashMove, transform.position, Quaternion.identity, transform);
        skill.GetComponent<DisplacementSkill>().xDirecition = true;
        skill.GetComponent<DisplacementSkill>().up = false;
        skill.Use(GetComponent<Character>());
        animator.SetTrigger("KingSlash");
        skill.gameObject.SetActive(true);
        yield return null;
    }

    IEnumerator UseKingSlsh()
    {
        StartCoroutine("CoolDownSkill", kingSlash);
        //animator.SetTrigger("explosion");
        Vector2 pos = transform.position;
        Vector2 scale = transform.localScale;
        if (Player.Instance.transform.position.x > transform.position.x)
            scale.x = right;
        else
            scale.x = -right;
        transform.localScale = scale;
        if (transform.localScale.x > 0)
        {
            pos.x -= kingSlash.positionXOffset;
            scale.x = -1;
        }
        else
        {
            pos.x += kingSlash.positionXOffset;
            scale.x = 1;
        }
        Skill skill = Instantiate(kingSlash, pos, Quaternion.identity);
        skill.transform.localScale = scale;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.35f);
        skill.gameObject.SetActive(true);
        skill.Use(GetComponent<Character>());
        yield return null;
    }

    IEnumerator UseTurnado()
    {
        StartCoroutine("CoolDownSkill", turnado);
        //animator.SetTrigger("explosion");
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2);
        //bool use = false;
        //while (!use)
        //{
        //    stateInfo = animator.GetCurrentAnimatorStateInfo(2);
        //    if (stateInfo.IsName("BossExplosion"))
        //    {
        //        while (true)
        //        {
        //            stateInfo = animator.GetCurrentAnimatorStateInfo(2);
        //            if (stateInfo.normalizedTime >= 0.5f)
        //            {
        //                use = true;
        //                break;
        //            }
        //            yield return null;
        //        }
        //    }
        //    yield return null;
        //}
        Vector2 position = transform.position;
        if (transform.localScale.x > 0)
            position.x -= turnado.positionXOffset;
        else
            position.x += turnado.positionXOffset;
        position.y += turnado.positionYOffset;
        Skill skill = Instantiate(turnado, position, Quaternion.identity);
        Vector2 scale = transform.localScale;
        scale.x = transform.localScale.x > 0 ? -1 : 1;
        skill.transform.localScale *= scale;
        skill.gameObject.SetActive(true);
        skill.Use(GetComponent<Character>());
        yield return null;
    }

    IEnumerator ProtectQueen()
    {
        CanProtectQueen = false;
        protectingQueen = true;
        Skill s1 = Instantiate(kingQueenExchange, transform.position - new Vector3(0, 0.25f, 0), Quaternion.identity);
        Skill s2 = Instantiate(kingQueenExchange, queen.transform.position - new Vector3(0, 0.25f, 0), Quaternion.identity);
        s1.gameObject.SetActive(true);
        s2.gameObject.SetActive(true);
        s1.Use(this);
        s2.Use(this);
        yield return new WaitForSeconds(0.25f);
        Vector3 oriPos = queen.transform.position;
        queen.transform.position = transform.position;
        transform.position = oriPos;
        StartCoroutine("UseSlashMove");
        yield return new WaitForSeconds(0.25f);
        StartCoroutine("UseSlashMove");
        yield return new WaitForSeconds(0.25f);
        StartCoroutine("UseSlashMove");
        yield return new WaitForSeconds(0.25f);
        StartCoroutine("UseTurnado");
        yield return new WaitForSeconds(0.75f);
        transform.position = queen.transform.position;
        queen.transform.position = oriPos;
        Destroy(s1.gameObject);
        Destroy(s2.gameObject);
        protectingQueen = false;
        yield return new WaitForSeconds(5);
        CanProtectQueen = true;
    }

    IEnumerator UseFireGun()
    {
        while (HP > MaxHP / 2)
            yield return null;
        GameManager.instance.StartCoroutine("ShowKingBattleStory");
        while (Time.timeScale == 0)
            yield return null;
        UsingFireGun = true;
        animator.SetBool("Walk", false);
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        Skill s = Instantiate(kingQueenExchange, transform.position - new Vector3(0, 0.25f, 0), Quaternion.identity);
        s.gameObject.SetActive(true);
        transform.position = new Vector3(5.35f, -2.723916f, 0);
        yield return new WaitForSeconds(0.5f);
        Destroy(s.gameObject);
        animator.SetBool("FireGun", true);
        for (int i = 0; i < 11; i++)
        {
            StartCoroutine("PrepareFireGun", i);
            yield return new WaitForSeconds(0.25f);
        }
        Skill fireGun = Instantiate(kingFireGun);
        fireGun.Use(this);
        fireGun.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        Destroy(fireGun.gameObject);
        animator.SetBool("FireGun", false);
        UsingFireGun = false;
        yield return null;
    }

    IEnumerator PrepareFireGun(int index)
    {
        Skill s = Instantiate(kingQueenExchange, new Vector3(3.84f, -2.24f, 0), Quaternion.AngleAxis(90, new Vector3(0, 0, 1)));
        s.transform.localScale *= 1 + index * (2.5f - 1.0f) / 10;
        s.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        Destroy(s.gameObject);
        yield return null;
    }

    IEnumerator CoolDownSkill(Skill skill)
    {
        skill.canUse = false;
        float coolTime;
        if (skill.coolTime.Length == 0)
            coolTime = 0;
        else if (skill.level > skill.coolTime.Length)
            coolTime = skill.coolTime[skill.coolTime.Length - 1];
        else
            coolTime = skill.coolTime[skill.level - 1];
        yield return new WaitForSeconds(coolTime);
        skill.canUse = true;
    }

    public override void Damage(int value)
    {
        if (HP > 0)
        {
            base.Damage(value);
            infoShower.UpdateHpStripe(HP, initialAbility.maxHp);
            Debug.Log(HP);
            if (queen.CanUseArea && ! UsingFireGun)
                queen.StartCoroutine("UseQueenArea", true);
            if (HP <= 0)
            {
                if (UsingFireGun)
                {
                    HP = 1;
                    return;
                }
                GameManager.instance.StartCoroutine("ShowKingBattleStory2");
                animator.SetBool("Walk", false);
                animator.SetBool("Jump", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && GetComponent<Rigidbody2D>().velocity.y <= 0)
        {
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    public IEnumerator Die()
    {
        animator.SetBool("Die", true);
        StopCoroutine("Attack");
        StopCoroutine("UseRedThunder");
        GetComponent<Character>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        //Instantiate(dropWeapon, new Vector3(transform.position.x, dropWeapon.transform.position.y, transform.position.z), Quaternion.identity);
        //GameObject g = Instantiate(dropEffect, transform.position, Quaternion.identity);
        //for (int i = 0; i < dropItems.Length; i++)
        //{
        //    for (int j = 0; j < 5; j++)
        //    {
        //        //Instantiate(dropItems[i], transform.position + new Vector3(-0.75f + (i * 5 + j) * (0.75f / dropItems.Length), 0.55f, 0), Quaternion.identity);
        //        Object item = GameManager.www.assetBundle.LoadAsset(dropItemNames[i]);
        //        Instantiate(item, transform.position + new Vector3(-0.75f + (i * 5 + j) * (0.75f / dropItems.Length), 0.55f, 0), Quaternion.identity);
        //    }
        //}
        //Player.Instance.GetExp(exp);
        //yield return new WaitForSeconds(0.25f);
        //Destroy(g);
        Destroy(gameObject);
    }
}
