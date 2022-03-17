using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character
{
    public int exp;

    public float moveFrequncy = 1.0f;
    public bool isAutoAttack;
    public float autoAttackDistance;
    public bool moveToPlayerWhenDamaged;
    public float stopDistance;
    public float moveFrequncyWhenDamaged;
    public float speedWhenDamages;
    public Skill[] skills;

    public Weapon dropWeapon;
    public float dropWeaponProbibility;
    public string[] dropItems;
    public float[] dropItemProbibilities;

    public MonsterInfoShower infoShower;

    private Rigidbody2D rigid;
    private Animator animator;
    private bool beAttacked;
    private bool isAttacking;

    public readonly int animWalk = Animator.StringToHash("Walk");
    public readonly int animAttack = Animator.StringToHash("Attack");

    protected override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = transform.GetChild(0).GetComponent<Animator>();
        beAttacked = false;
        StartCoroutine("Move");
        for (int i = 0; i < skills.Length; i++)
            skills[i].level = LV;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.localScale.x > 0) && (right < 0 || transform.localScale.x > 0 && right > 0))
            infoShower.Flip(true);
        else
            infoShower.Flip(false);
    }

    public override bool IsPlayer()
    {
        return false;
    }

    IEnumerator Move()
    {
        Player player = FindObjectOfType<Player>();
        Vector2 v;
        float d;
        Vector2 scale;
        while (true) 
        {
            v = player.transform.position - transform.position;
            d = v.magnitude;
            if (isAttacking)
            {
                animator.SetBool(animWalk, false);
                yield return null;
            }
            else if (isAutoAttack && d <= autoAttackDistance && d > stopDistance && !isAttacking)
            {
                animator.SetBool(animWalk, true);
                if (v.x > 0)
                {
                    rigid.velocity = new Vector2(Speed, rigid.velocity.y);
                    scale = transform.localScale;
                    scale.x = right * Mathf.Abs(scale.x);
                }
                else
                {
                    rigid.velocity = new Vector2(-Speed, rigid.velocity.y);
                    scale = transform.localScale;
                    scale.x = -right * Mathf.Abs(scale.x);
                }
                transform.localScale = scale;
                if (transform.localScale.x * right < 0)
                    infoShower.Flip(true);
                else
                    infoShower.Flip(false);
                yield return new WaitForSeconds(1 / moveFrequncy);
            }
            else
            {
                if (!(beAttacked || isAttacking) && HP == MaxHP)
                {
                    int type = Random.Range(0, 3);
                    // Idle, no move.
                    if (type == 0)
                    {
                        animator.SetBool(animWalk, false);
                        rigid.velocity = Vector2.zero;
                    }
                    // Move left.
                    else if (type == 1)
                    {
                        rigid.velocity = new Vector2(-Speed, rigid.velocity.y);
                        animator.SetBool(animWalk, true);
                        scale = transform.localScale;
                        scale.x = -right * Mathf.Abs(scale.x);
                        transform.localScale = scale;
                    }
                    // Move right.
                    else if (type == 2)
                    {
                        rigid.velocity = new Vector2(Speed, rigid.velocity.y);
                        animator.SetBool(animWalk, true);
                        scale = transform.localScale;
                        scale.x = right * Mathf.Abs(scale.x);
                        transform.localScale = scale;
                    }
                    if (transform.localScale.x * right < 0)
                        infoShower.Flip(true);
                    else
                        infoShower.Flip(false);
                    yield return new WaitForSeconds(1 / moveFrequncy);
                }
                else if (HP < MaxHP && moveToPlayerWhenDamaged && d > stopDistance && !(beAttacked || isAttacking))
                {
                    animator.SetBool(animWalk, true);
                    if (v.x > 0)
                    {
                        rigid.velocity = new Vector2(speedWhenDamages, rigid.velocity.y);
                        scale = transform.localScale;
                        scale.x = right * Mathf.Abs(scale.x);
                    }
                    else
                    {
                        rigid.velocity = new Vector2(-speedWhenDamages, rigid.velocity.y);
                        scale = transform.localScale;
                        scale.x = -right * Mathf.Abs(scale.x);
                    }
                    transform.localScale = scale;
                    if (transform.localScale.x * right < 0)
                        infoShower.Flip(true);
                    else
                        infoShower.Flip(false);
                    yield return new WaitForSeconds(1 / moveFrequncyWhenDamaged);
                }
            }
            yield return null;
        }
    }

    public override bool Damage(int value, float delayToDie)
    {
        base.Damage(value);
        beAttacked = true;
        StartCoroutine("WaitToIdle", 2);
        infoShower.UpdateHpStripe(HP, initialAbility.maxHp);
        if (HP <= 0)
        {
            StartCoroutine("WaitToDestroy", delayToDie);
            return true;
        }
        return false;
    }

    IEnumerator WaitToDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Random.Range(0, 100) < dropWeaponProbibility)
            Instantiate(dropWeapon, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        for (int i = 0; i < dropItems.Length; i++)
        {
            float probility = i < dropItemProbibilities.Length? dropItemProbibilities[i] : 100;
            if (Random.Range(0, 100) < probility)
            {
                Object item = GameManager.instance.www.assetBundle.LoadAsset(dropItems[i]);
                Instantiate(item, transform.position + new Vector3(-0.75f + i * (0.75f / dropItems.Length), 0.55f, 0), Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator WaitToIdle(float delay)
    {
        if (this != null)
        {
            animator.SetBool(animWalk, false);
            yield return new WaitForSeconds(delay);
            beAttacked = false;
        }
        yield return null;
    }

    public void StartAttack()
    {
        isAttacking = true;
        //animator.SetBool(animWalk, false);
        animator.SetTrigger(animAttack);
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnDestroy()
    {
        transform.parent.GetComponent<EnemySpawner>().DeleteEnemy();
    }
}
