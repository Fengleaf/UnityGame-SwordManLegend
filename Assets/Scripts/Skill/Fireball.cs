using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Skill))]
public class Fireball : MonoBehaviour
{
    public float speed;
    public float stayTime = 2.0f;
    private Skill skill;
    public bool directionByScale = true;

    // Start is called before the first frame update
    void Start()
    {
        skill = GetComponent<Skill>();
        StartCoroutine("Move");
        StartCoroutine("Stay");
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (skill.User == null)
            {
                StopAllCoroutines();
                Destroy(gameObject);
                break;
            }
            if (directionByScale)
            {
                Vector2 direction = Vector2.right;
                direction.x *= transform.localScale.x;
                transform.Translate(direction * speed * Time.deltaTime);
            }
            else
            {
                Vector2 direction = -Vector2.left * transform.localRotation.z;
                direction.Normalize();
                if (direction.magnitude == 0)
                    direction.x = transform.localScale.x > 0 ? 1 : -1;
                transform.Translate(direction * speed * Time.deltaTime);
            }
            yield return null;
        }
    }

    IEnumerator Stay()
    {
        yield return new WaitForSeconds(stayTime);
        if (skill != null)
            skill.DestroySelf();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponent<PointEffector2D>() != null)
            return;
        if (!skill.User.IsPlayer() && other.CompareTag("Player"))
        {
            skill.JudgeHit(new Character[] { other.GetComponent<Character>() });
            skill.DestroySelf();
        }
        else if (skill.User.CompareTag("Player") && (other.CompareTag("Enemy") || other.CompareTag("Boss")))
        {
            skill.JudgeHit(new Character[] { other.GetComponent<Character>() });
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
