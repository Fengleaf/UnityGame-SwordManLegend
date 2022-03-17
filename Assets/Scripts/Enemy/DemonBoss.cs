using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DemonBoss : Character
{
    public int exp;
    public Animator animator;
    public Skill explosionPrefab;
    public Vector2[] explosionPositions;
    public Skill swordSpinAirSlashPrefab;
    public Skill shadowMovePrefab;
    public Skill demonSwordAirPrefab;
    public int demonSwordAirNumber;
    public float demonSwordAirFrequency;
    public Skill redThunderPrefab;
    public int[] thunderNumberPerTime;
    public float thunderFrequency;
    public float[] thunderXRange;
    public MonsterInfoShower infoShower;

    public Item[] dropItems;
    public string[] dropItemNames;

    public Weapon dropWeapon;
    public GameObject dropEffect;

    private Skill explosion;
    private Skill swordSpinAirSlash;
    private Skill demonSwordAir;
    private Skill redThunder;
    private Skill shadowMove;

    private bool usingDemonSwordAir;

    private bool isGround = true;

    //private WWW www;
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
    }

    private void InitialSkill()
    {
        explosion = Instantiate(explosionPrefab);
        explosion.gameObject.SetActive(false);
        explosion.level = LV;
        swordSpinAirSlash = Instantiate(swordSpinAirSlashPrefab);
        swordSpinAirSlash.gameObject.SetActive(false);
        swordSpinAirSlash.level = LV;
        demonSwordAir = Instantiate(demonSwordAirPrefab);
        demonSwordAir.gameObject.SetActive(false);
        demonSwordAir.level = LV;
        redThunder = Instantiate(redThunderPrefab);
        redThunder.gameObject.SetActive(false);
        redThunder.level = LV;
        shadowMove = Instantiate(shadowMovePrefab);
        shadowMove.gameObject.SetActive(false);
        shadowMove.level = LV;
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
        //while (GameManager.instance.AllStop)
        //    yield return null;
        // First use explosion once.
        StartCoroutine("UseExplosion");
        yield return new WaitForSeconds(1);
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine("UseDemonSwordAir");
        usingDemonSwordAir = false;
        demonSwordAir.canUse = false;
        StartCoroutine("UseRedThunder");
        StartCoroutine("DetectOutOfBound");
        while (true)
        {
            if (!usingDemonSwordAir)
            {
                animator.SetBool("Run", false);
                Vector2 v = Player.Instance.transform.position - transform.position;
                float d = v.magnitude;/*Vector2.Distance(Player.Instance.transform.position, transform.position);*/
                if (d > 5)
                {
                    if (shadowMove.canUse)
                    {
                        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
                            StartCoroutine(UseShadowMove(false));
                        else
                            StartCoroutine(UseShadowMove(true));
                    }
                    else if (explosion.canUse)
                        StartCoroutine("UseExplosion");
                    else if (demonSwordAir.canUse)
                        StartCoroutine("UseDemonSwordAir2");
                    else if (isGround)
                    {
                        Vector2 scale = transform.localScale;
                        animator.SetBool("Run", true);
                        if (Player.Instance.transform.position.x > transform.position.x)
                        {
                            scale.x = right;
                            rigidbody2D.velocity = new Vector2(Speed, rigidbody2D.velocity.y);
                        }
                        else
                        {
                            scale.x = -right;
                            rigidbody2D.velocity = new Vector2(-Speed, rigidbody2D.velocity.y);
                        }
                    }
                }
                else if (d >= 2.5)
                {
                    if (isGround)
                    {
                        if (Mathf.Abs(v.x) < 0.1f && rigidbody2D.velocity.y <= 0 && v.y > 0)
                        {
                            animator.SetBool("Jump", true);
                            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -9.81f * Time.deltaTime + 7.5f);
                        }
                        else if (Mathf.Abs(v.x)  < 0.1f && v.y < 0)
                        {
                            if (shadowMove.canUse)
                            {
                                StartCoroutine(UseShadowMove(true));
                            }
                            else
                            {
                                while (Mathf.Abs(v.y) > 0.8f && Mathf.Abs(v.x) < 4.5f)
                                {
                                    if (shadowMove.canUse)
                                    {
                                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -9.81f * Time.deltaTime + 7.5f);
                                        StartCoroutine(UseShadowMove(true));
                                    }
                                    else
                                    {
                                        Vector2 scale = transform.localScale;
                                        animator.SetBool("Run", true);
                                        scale.x = right;
                                        rigidbody2D.velocity = new Vector2(Speed, rigidbody2D.velocity.y);
                                        transform.localScale = scale;
                                    }
                                    yield return null;
                                    v = Player.Instance.transform.position - transform.position;
                                }
                            }
                        }
                        else
                        {
                            if (v.y > 1)
                                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -9.81f * Time.deltaTime + 7.5f);
                            else
                            {
                                Vector2 scale = transform.localScale;
                                animator.SetBool("Run", true);
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
                    }
                }
                else if (d < 2.5)
                {
                    if (d < 1.6f && shadowMove.canUse)
                    {
                        if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
                            StartCoroutine(UseShadowMove(false));
                        else
                            StartCoroutine(UseShadowMove(true));
                    }
                    else if (swordSpinAirSlash.canUse)
                    {
                        if (Mathf.Abs(v.y) > 1 && v.y > 1)
                        {
                            while (Mathf.Abs(v.y) > 0.7f && Mathf.Abs(v.x) < 4.5f)
                            {
                                if (shadowMove.canUse)
                                {
                                    if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
                                        StartCoroutine(UseShadowMove(false));
                                    else
                                        StartCoroutine(UseShadowMove(true));
                                }
                                else
                                {
                                    if (Mathf.Abs(v.y) > Mathf.Abs(v.x))
                                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -9.81f * Time.deltaTime + 7.5f);
                                    else
                                    {
                                        Vector2 scale = transform.localScale;
                                        animator.SetBool("Run", true);
                                        scale.x = right;
                                        rigidbody2D.velocity = new Vector2(Speed, rigidbody2D.velocity.y);
                                        transform.localScale = scale;
                                        yield return null;
                                    }
                                }
                                yield return null;
                                v = Player.Instance.transform.position - transform.position;
                            }
                        }
                        else
                        {
                            StartCoroutine("UseSwordSpinAirSlash");
                        }
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator DetectOutOfBound()
    {
        while (true)
        {
            if (transform.position.y > 6)
            {
                transform.position = new Vector2(transform.position.x, -4.184577f);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                isGround = true;
                StartCoroutine("CoolDownSkill", shadowMove);
            }
            if (transform.position.y < -4.5)
            {
                transform.position = new Vector2(transform.position.x, -4.184577f);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                isGround = true;
                StartCoroutine("CoolDownSkill", shadowMove);
            }
            if (transform.position.x > 12)
            {
                transform.position = new Vector2(11.72f, transform.position.y);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                isGround = true;
                StartCoroutine("CoolDownSkill", shadowMove);
            }
            if (transform.position.x < -13)
            {
                transform.position = new Vector2(-12f, transform.position.y);
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                isGround = true;
                StartCoroutine("CoolDownSkill", shadowMove);
            }
            yield return null;
        }
    }

    IEnumerator UseShadowMove(bool horizontal, bool up = true)
    {
        StartCoroutine("CoolDownSkill", shadowMove);
        Vector2 scale = transform.localScale;
        if (Player.Instance.transform.position.x > transform.position.x)
            scale.x = right;
        else
            scale.x = -right;
        transform.localScale = scale;
        Skill skill = Instantiate(shadowMove, transform.position, Quaternion.identity);
        skill.GetComponent<DisplacementSkill>().xDirecition = horizontal;
        skill.GetComponent<DisplacementSkill>().up = up;
        skill.gameObject.SetActive(true);
        skill.Use(GetComponent<Character>());
        yield return null;
    }

    IEnumerator UseSwordSpinAirSlash()
    {
        StartCoroutine("CoolDownSkill", swordSpinAirSlash);
        animator.SetTrigger("explosion");
        Vector2 pos = transform.position;
        //pos.y += swordSpinAirSlash.positionYOffset;
        if (Player.Instance.transform.position.y - transform.position.y < 0)
           pos.y -= swordSpinAirSlash.positionYOffset;
        else
            pos.y += swordSpinAirSlash.positionYOffset;
        Skill skill = Instantiate(swordSpinAirSlash, pos, Quaternion.identity);
        skill.gameObject.SetActive(true);
        skill.Use(GetComponent<Character>());
        yield return null;
    }

    IEnumerator UseExplosion()
    {
        StartCoroutine("CoolDownSkill", explosion);
        animator.SetTrigger("explosion");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2);
        bool use = false;
        while (!use)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(2);
            if (stateInfo.IsName("BossExplosion"))
            {
                while (true)
                {
                    stateInfo = animator.GetCurrentAnimatorStateInfo(2);
                    if (stateInfo.normalizedTime >= 0.5f)
                    {
                        use = true;
                        break;
                    }
                    yield return null;
                }
            }
            yield return null;
        }

        for (int i = 0; i < explosionPositions.Length; i++)
        {
            Skill skill = Instantiate(explosion, (Vector2)transform.position + explosionPositions[i], Quaternion.AngleAxis(i * 45, new Vector3(0, 0, 1)));
            skill.gameObject.SetActive(true);
            skill.Use(GetComponent<Character>());
        }
    }

    IEnumerator UseDemonSwordAir()
    {
        while (HP > initialAbility.maxHp / 2.0f)
            yield return null;
        usingDemonSwordAir = true;
        StartCoroutine(GameManager.instance.ShowBossStory2());
        StartCoroutine("CoolDownSkill", demonSwordAir);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 500));
        isGround = false;
        yield return new WaitForSeconds(1);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        for (int i = 0; i < demonSwordAirNumber; i++)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.45f / 2.0f);
            Vector2 dir = Player.Instance.transform.position - transform.position;
            Skill skill = Instantiate(demonSwordAir, transform.position, Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) - 180, new Vector3(0, 0, 1)));
            skill.gameObject.SetActive(true);
            skill.Use(GetComponent<Character>());
            yield return new WaitForSeconds(1 / demonSwordAirFrequency);
        }
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        usingDemonSwordAir = false;
        StartCoroutine("CoolDownSkill", demonSwordAir);
    }

    IEnumerator UseDemonSwordAir2()
    {
        StartCoroutine("CoolDownSkill", demonSwordAir);
        animator.SetTrigger("Attack");
        Vector2 dir = Player.Instance.transform.position - transform.position;
        Skill skill = Instantiate(demonSwordAir, transform.position, Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x) - 180, new Vector3(0, 0, 1)));
        skill.gameObject.SetActive(true);
        skill.Use(GetComponent<Character>());
        yield return null;
    }

    IEnumerator UseRedThunder()
    {
        while (HP > initialAbility.maxHp / 4.0f)
            yield return null;
        StartCoroutine(GameManager.instance.ShowBossStory3());
        while (true)
        {
            int number = Random.Range(thunderNumberPerTime[0], thunderNumberPerTime[1] + 1);
            for (int i = 0; i < number; i++)
            {
                float x = Random.Range(thunderXRange[0], thunderXRange[1]);
                float y = redThunder.transform.position.y;
                Skill thunder = Instantiate(redThunder, new Vector2(x, 5.0f), Quaternion.identity);
                thunder.gameObject.SetActive(true);
                thunder.Use(GetComponent<Character>());
                StartCoroutine("MoveRedThunder", thunder);
            }
            yield return new WaitForSeconds(1 / thunderFrequency);
        }
    }

    /// <summary>
    /// Because two animation have different y position.
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveRedThunder(Skill redThunder)
    {
        while (true)
        {
            if (redThunder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("RedThunder"))
            {
                Vector2 p = redThunder.transform.position;
                p.y = 0.1f;
                redThunder.transform.position = p;
                break;
            }
            yield return null;
        }
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
            if (shadowMove.canUse)
                StartCoroutine(UseShadowMove(true));
            if (HP <= 0)
            {
                //StartCoroutine("LoadWWW");
                GameManager.instance.StartCoroutine("ShowBossStory4");
                animator.SetBool("Run", false);
                animator.SetBool("Jump", false);
                //StartCoroutine("Die");
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
        Instantiate(dropWeapon, new Vector3(transform.position.x, dropWeapon.transform.position.y, transform.position.z), Quaternion.identity);
        GameObject g = Instantiate(dropEffect, transform.position, Quaternion.identity);
        for (int i = 0; i < dropItems.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                //Instantiate(dropItems[i], transform.position + new Vector3(-0.75f + (i * 5 + j) * (0.75f / dropItems.Length), 0.55f, 0), Quaternion.identity);
                //Object item = GameManager.instance.www.LoadAsset(dropItemNames[i]);
                //GameManager.instance.www.
                Object item = GameManager.instance.www.assetBundle.LoadAsset(dropItemNames[i]);
                Instantiate(item, transform.position + new Vector3(-0.75f + (i * 5 + j) * (0.75f / dropItems.Length), 0.55f, 0), Quaternion.identity);
            }
        }
        Player.Instance.GetExp(exp);
        yield return new WaitForSeconds(0.25f);
        Destroy(g);
        Destroy(gameObject);
    }
}
