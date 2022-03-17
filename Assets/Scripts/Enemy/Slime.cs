using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class Slime : MonoBehaviour
{
    public float attackFrequency;
    public Skill slimeHit;

    private Monster monster;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<Monster>();
        player = FindObjectOfType<Player>();
        StartCoroutine("Attack");
    }

    IEnumerator Attack()
    {
        while (true)
        {
            Vector2 v = player.transform.position - transform.position;
            float d = v.magnitude;
            if (monster.HP < monster.MaxHP && d < 1f)
            {
                Vector2 scale = transform.localScale;
                // Player is at left.
                if (v.x < 0)
                    scale.x = monster.right < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                // Player is at right.
                else
                    scale.x = monster.right < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
                StartCoroutine("UseSlimeHit");
                yield return new WaitForSeconds(1 / attackFrequency);
            }
            yield return null;
        }
    }

    IEnumerator UseSlimeHit()
    {
        Skill skill = Instantiate(slimeHit, transform);
        skill.level = monster.LV;
        monster.StartAttack();
        skill.Use(monster);
        yield return new WaitForSeconds(0.6f);
        skill.JudgeHit(0);
        yield return new WaitForSeconds(1f);
        if (skill.gameObject != null)
            skill.DestroySelf();
    }
}
