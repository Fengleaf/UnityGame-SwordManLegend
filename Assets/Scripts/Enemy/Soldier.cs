using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public float attackFrequency;
    public Skill soldierAttaclPrefab;
    public Vector2 atk1Position;
    public float atk1Rotation;
    public Vector2 atk2Position;
    public float atk2Rotation;
    public Vector2 atk3Position;
    public float atk3Rotation;

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
            if (d <= monster.stopDistance)
            {
                Vector2 scale = transform.localScale;
                // Player is at left.
                if (v.x < 0)
                    scale.x = monster.right < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                // Player is at right.
                else
                    scale.x = monster.right < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
                StartCoroutine("UseSoldierAttack");
                yield return new WaitForSeconds(1 / attackFrequency);
            }
            yield return null;
        }
    }

    IEnumerator UseSoldierAttack()
    {
        monster.StartAttack();
        Skill skill;
        skill = Instantiate(soldierAttaclPrefab, atk1Position + (Vector2)transform.position, Quaternion.AngleAxis(atk1Rotation, new Vector3(0, 0, 1)), transform);
        skill.level = monster.LV;
        skill.Use(monster);
        yield return new WaitForSeconds(0.2f);
        skill = Instantiate(soldierAttaclPrefab, atk2Position + (Vector2)transform.position, Quaternion.AngleAxis(atk2Rotation, new Vector3(0, 0, 1)), transform);
        skill.level = monster.LV;
        skill.Use(monster);
        yield return new WaitForSeconds(0.2f);
        skill = Instantiate(soldierAttaclPrefab, atk3Position + (Vector2)transform.position, Quaternion.AngleAxis(atk3Rotation, new Vector3(0, 0, 1)), transform);
        skill.level = monster.LV;
        skill.Use(monster);
        monster.EndAttack();
    }
}
