using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float jumpPower;
    public Animator animator;
    public PlayerAttack playerAttack;

    private Rigidbody2D rigid;
    private Player character;
    private bool isGrounded;
    private Collider2D nowGround;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        character = GetComponent<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        isGrounded = true;
        StartCoroutine("FallDown");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!playerAttack.IsAttack)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    Move(Vector2.left);
                else if (Input.GetKey(KeyCode.RightArrow))
                    Move(Vector2.right);
                else
                    Move(Vector2.zero);

                if (KeyboardSetting.specialKeyTable.ContainsKey(ValueType.Jump))
                {
                    if (Input.GetKey(KeyboardSetting.specialKeyTable[ValueType.Jump].key.key))
                        Jump();
                }
                if (Input.GetKey(KeyCode.DownArrow))
                    JumpDown();
            }
            else
                Move(Vector2.zero);
        }
    }

    IEnumerator FallDown()
    {
        while (true)
        {
            rigid.AddForce(new Vector2(0, -9.81f * Time.deltaTime));
            yield return null;
        }
    }

    public void Move(Vector3 direction)
    {
        rigid.velocity = new Vector2(direction.x * character.Speed, rigid.velocity.y);
        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (direction.x != 0)
            animator.SetBool("Run", true);
        else
            animator.SetBool("Run", false);

    }

    public void Jump()
    {
        if (!isGrounded)
            return;
        if (rigid.velocity.y > 0)
            return;
        isGrounded = false;
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y + jumpPower);
        animator.SetBool("Jump", true);
    }

    public void JumpDown()
    {
        if (!isGrounded)
            return;
        animator.SetBool("Jump", true);
        StartCoroutine("DetectFallDown");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (rigid.velocity.y > 0.1f)
                return;
            nowGround = collision.collider;
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    IEnumerator DetectFallDown()
    {
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        PlatformEffector2D effector2D = nowGround.GetComponent<PlatformEffector2D>();
        if (effector2D != null)
        {
            isGrounded = false;
            effector2D.colliderMask -= LayerMask.GetMask("Player");
            RaycastHit2D hit2D = Physics2D.Raycast((Vector2)transform.position + collider.offset - new Vector2(0, collider.size.y / 2 + 0.1f), Vector2.down, 10, LayerMask.GetMask("Ground"));
            while (hit2D.distance > Mathf.Epsilon || hit2D.collider == nowGround)
            {
                hit2D = Physics2D.Raycast((Vector2)transform.position + collider.offset - new Vector2(0, collider.size.y / 2 + 0.1f), Vector2.down, 10, LayerMask.GetMask("Ground"));
                yield return null;
            }
            effector2D.colliderMask += LayerMask.GetMask("Player");
            nowGround = hit2D.collider;
            isGrounded = true;
        }
        animator.SetBool("Jump", false);
    }
}
