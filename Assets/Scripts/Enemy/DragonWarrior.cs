using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWarrior : MonoBehaviour
{
    public Monster monster;
    public GameObject eye;
    public float fireBallFrequency = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Detect");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Detect()
    {
        while (true)
        {
            if (monster.HP != monster.initialAbility.maxHp)
            {
                // Charcter layer.
                int layer = 1 << 8;
                RaycastHit2D[] hits = Physics2D.RaycastAll(eye.transform.position, Vector2.right * transform.localScale.x, 6, layer);
                if (hits.Length > 0)
                {
                    for (int i = 0;i < hits.Length;i++)
                    {
                        if (hits[i].collider.CompareTag("Player"))
                        {
                            monster.StartAttack();
                            UseFireball();
                        }
                    }
                }
            }
            monster.EndAttack();
            yield return new WaitForSeconds(1 / fireBallFrequency);
        }
    }

    void UseFireball()
    {
        Vector2 scale = monster.skills[0].transform.localScale;
        scale.x = transform.localScale.x;
        monster.skills[0].transform.localScale = scale;
        Vector3 position = eye.transform.position;
        position.y += -0.1670498f;
        Skill skill = Instantiate(monster.skills[0], position, Quaternion.identity);
        skill.Use(monster);
    }
}
