using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Skill))]
/// <summary>
/// The skill that can let user move forward instantly.
/// </summary>
public class DisplacementSkill : MonoBehaviour
{
    public float time;
    public float speed;
    // X direction or Y direction.
    public bool xDirecition;
    public bool up;

    private Skill skill;
    private float timer;
    private void Start()
    {
        skill = GetComponent<Skill>();
    }

    /// <summary>
    /// Call by animation event.
    /// </summary>
    public void MoveUser()
    {
        timer = Time.time;
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (Time.time - timer <= time)
        {
            Vector2 position = skill.User.transform.position;
            Vector2 distance;
            RaycastHit2D raycastHit2D;
            if (xDirecition)
            {
                // The same, face right
                if (skill.User.transform.localScale.x * skill.User.right > 0)
                {
                    distance = Vector2.right * speed * Time.deltaTime;
                    position = skill.User.transform.position;
                    position.x += skill.User.GetComponent<CapsuleCollider2D>().offset.x + skill.User.GetComponent<CapsuleCollider2D>().size.x / 2;
                    raycastHit2D = Physics2D.Raycast(position, Vector2.right, distance.x * 2, LayerMask.GetMask("Ground"));
                    if (raycastHit2D)
                    {
                        if (raycastHit2D.collider.CompareTag("Wall") && raycastHit2D.distance < distance.x)
                            distance.x = raycastHit2D.distance;
                    }
                    if (Mathf.Abs(distance.x) < 0.01f)
                        distance.x = 0;
                }
                // Face left.
                else
                {
                    distance = Vector2.left * speed * Time.deltaTime;
                    position = skill.User.transform.position;
                    position.x -= (skill.User.GetComponent<CapsuleCollider2D>().offset.x + skill.User.GetComponent<CapsuleCollider2D>().size.x / 2);
                    raycastHit2D = Physics2D.Raycast(position, Vector2.left, distance.x * 2, LayerMask.GetMask("Ground"));
                    if (raycastHit2D)
                    {
                        if (raycastHit2D.collider.CompareTag("Wall") && raycastHit2D.distance < Mathf.Abs(distance.x))
                            distance.x = raycastHit2D.distance;
                    }
                    if (Mathf.Abs(distance.x) < 0.01f)
                        distance.x = 0;
                }
            }
            else
            {
                if (up)
                {
                    distance = Vector2.up * speed * Time.deltaTime;
                    position = skill.User.transform.position;
                    position.y += skill.User.GetComponent<CapsuleCollider2D>().offset.y + skill.User.GetComponent<CapsuleCollider2D>().size.y / 2;
                    raycastHit2D = Physics2D.Raycast(position, Vector2.up, distance.y * 2, LayerMask.GetMask("Ground"));
                    if (raycastHit2D)
                    {
                        if (raycastHit2D.collider.CompareTag("Wall") && raycastHit2D.distance < distance.y)
                            distance.y = raycastHit2D.distance;
                    }
                }
                else
                {
                    distance = Vector2.down * speed * Time.deltaTime;
                    position = skill.User.transform.position;
                    position.y -= skill.User.GetComponent<CapsuleCollider2D>().offset.y - skill.User.GetComponent<CapsuleCollider2D>().size.y / 2;
                    raycastHit2D = Physics2D.Raycast(position, Vector2.down, distance.y * 2, LayerMask.GetMask("Ground"));
                    if (raycastHit2D)
                    {
                        if (raycastHit2D.collider.CompareTag("Wall") && raycastHit2D.distance < distance.y)
                            distance.y = raycastHit2D.distance;
                    }
                }
            }
            skill.User.transform.Translate(distance);
            yield return null;
        }
    }
}
