using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Character
{
    public King king;

    public Animator animator;
    public MonsterInfoShower infoShower;

    public Skill queenThunderPrefab;
    public float queenThunderFrequency;
    public int[] queenThunderNumberPerTime;
    public float[] queenThunderXRange;
    public GameObject queenArrowPrefab;
    public Skill queenDarkBallPrefab;
    public float darkBallFrequency;
    public int[] darkBallNumberPerTime;

    public bool CanUseArea { get; private set; }

    private Skill queenThunder;
    private GameObject queenArrow;
    private Skill queenDarkBall;

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
        CanUseArea = true;
    }

    private void InitialSkill()
    {

        queenThunder = Instantiate(queenThunderPrefab);
        queenThunder.gameObject.SetActive(false);
        queenThunder.level = LV;
        queenArrow = Instantiate(queenArrowPrefab, new Vector3(-6.81f, -3.21f, 0), Quaternion.identity);
        queenArrow.gameObject.SetActive(false);
        queenDarkBall = Instantiate(queenDarkBallPrefab);
        queenDarkBall.gameObject.SetActive(false);
        queenDarkBall.level = LV;
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
        yield return new WaitForSeconds(2);
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine("UseDarkBall");
        StartCoroutine("UseQueenThunder");
        while (true)
        {
            yield return null;
        }
    }

    IEnumerator UseDarkBall()
    {
        while (true)
        {
            if (king.UsingFireGun && Time.timeScale != 0)
            {
                yield return null;
                continue;
            }
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

            int number = Random.Range(darkBallNumberPerTime[0], darkBallNumberPerTime[1] + 1);
            if (HP < MaxHP / 2)
                number += Random.Range(2, 4);
            float offset = Random.Range(-15.0f, 15.0f);
            for (int i = 0; i < number; i++)
            {
                Skill skill = Instantiate(queenDarkBall, (Vector2)transform.position, Quaternion.AngleAxis(135 + i * 90 / number + offset, new Vector3(0, 0, 1)));
                skill.gameObject.SetActive(true);
                skill.Use(GetComponent<Character>());
            }
            yield return new WaitForSeconds(1 / darkBallFrequency);
        }
    }

    IEnumerator UseQueenThunder()
    {
        while (true)
        {
            if (!king.UsingFireGun && Time.timeScale != 0)
            {
                int number = Random.Range(queenThunderNumberPerTime[0], queenThunderNumberPerTime[1] + 1);
                if (HP < MaxHP / 2)
                    number += Random.Range(1, 3);
                for (int i = 0; i < number; i++)
                {
                    float x = Random.Range(queenThunderXRange[0], queenThunderXRange[1]);
                    Skill thunder = Instantiate(queenThunder, new Vector2(x, queenThunder.transform.position.y), Quaternion.identity);
                    thunder.gameObject.SetActive(true);
                    thunder.Use(GetComponent<Character>());
                }
                yield return new WaitForSeconds(1 / queenThunderFrequency);
            }
            else
            {
                float x = Random.Range(5.466983f, 7.15f);
                Skill thunder = Instantiate(queenThunder, new Vector2(x, queenThunder.transform.position.y), Quaternion.identity);
                thunder.gameObject.SetActive(true);
                thunder.Use(GetComponent<Character>());
                yield return new WaitForSeconds(0.25f / queenThunderFrequency);
            }
        }
    }

    IEnumerator UseQueenArea(bool right)
    {
        CanUseArea = false;
        GameObject arrow = Instantiate(queenArrow);
        arrow.GetComponent<AreaEffector2D>().forceMagnitude = 0;
        float force;
        if (right)
        {
            arrow.transform.localScale = new Vector3(1, 1, 1);
            force = 275;
        }
        else
        {
            arrow.transform.localScale = new Vector3(-1, 1, 1);
            force = -275;
        }
        arrow.SetActive(true);
        for (int i = queenArrow.transform.childCount - 1; i >= 0; i--)
        {
            arrow.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.04f);
        }
        arrow.GetComponent<AreaEffector2D>().forceMagnitude = force;
        yield return new WaitForSeconds(4.0f);
        Destroy(arrow);
        yield return new WaitForSeconds(5);
        CanUseArea = true;
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
            float x = Player.Instance.transform.position.x - transform.position.x;
            if (!king.UsingFireGun)
            {
                Debug.Log(king.CanProtectQueen);
                if (Mathf.Abs(x) < 5f && king.CanProtectQueen)
                    king.StartCoroutine("ProtectQueen");
                else if (CanUseArea)
                    StartCoroutine("UseQueenArea", false);
            }
            if (HP <= 0)
            {
                if (king.UsingFireGun)
                    HP = 1;
                if (king.HP > 0)
                    HP = 1;
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
        Destroy(gameObject);
    }
}
