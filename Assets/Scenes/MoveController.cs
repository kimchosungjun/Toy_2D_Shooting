using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D coll;

    bool isJump = false; // 점프 버그를 막기 위함

    [SerializeField] bool isOnGround = false; 
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask groundLayer;

    Vector3 moveDir;
    float verticalVelocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        CheckGravity();
        Move();
        Jump();

    }

    private void CheckGround()
    {
        isOnGround = false;
        if (verticalVelocity > 0f)
            return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer) ;
        if (hit)
            isOnGround = true;
        //isJump = !isOnGround;
    }   
    
    private void Move()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
        moveDir.y = rb.velocity.y;
        rb.velocity = moveDir;
    }

    private void Jump()
    {
        if (!isOnGround)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            isJump = true;

        }

        //if (isOnGround && isJump && Input.GetKeyDown(KeyCode.Space))
        //{
        //    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // 지긋이 미는 힘, 벡터만 넣었을 때
        //    isJump = true;                                // : 이단 점프는 addforce로 구현 불가
        //}
    }  

    private void CheckGravity()
    {
        if (!isOnGround)
        {
            verticalVelocity += Physics2D.gravity.y * Time.deltaTime;
            // 물리와 관련된(Rigidbody) 것에는 Time.deltaTime을 사용하지 않아도 된다.
            if (verticalVelocity < -10f)
            {
                verticalVelocity = -10f;
            }
        }
        else if(isJump)
        {
            isJump = false;
            verticalVelocity = jumpForce;
        }
        else if (isOnGround)
        {
            verticalVelocity = 0f;
        }
        rb.velocity = new Vector2(rb.velocity.x, verticalVelocity);
    }

    private void DoAnimation()
    {
        //anim.SetInteger("Horizontal",(int)moveDir.x);
        //anim.SetBool("isOnGround",isOnGround);
    }
}
