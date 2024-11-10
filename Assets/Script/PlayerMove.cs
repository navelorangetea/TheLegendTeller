using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    CapsuleCollider2D capCollider;
    SpriteRenderer spriteRenderer;
    public GameManager gameManager;
    Animator anim;

    public float jumpForce = 5f;
    private int jumpCount = 0;
    private bool isGrounded;
    private bool isJumping;
    private bool isSliding;
    private float groundCheckDistance = 1.1f; //나중에 더 좋은 조작감으로 수정
    private float obstacleCheckDistance = 0.7f;
    private float v;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capCollider = GetComponent<CapsuleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColliderSize = capCollider.size;
        originalColliderOffset = capCollider.offset;

        isJumping = false;
        isSliding = false;

        Debug.Log("초기 playerHealth: " + gameManager.playerHealth);
    }

    void Update()
    {
        v = Input.GetAxisRaw("Vertical");
        bool jDown = Input.GetKeyDown(KeyCode.UpArrow);
        bool jUp = Input.GetKeyUp(KeyCode.UpArrow);
        bool sDown = Input.GetKeyDown(KeyCode.DownArrow);
        bool sUp = Input.GetKeyUp(KeyCode.DownArrow);

        anim.SetBool("IsSliding", false);
        anim.SetBool("IsSlidingStop", true);

        // Jump
        if (jDown && jumpCount < 2) // isGrounded를 추가하여 첫 점프만큼은 지면에서 가능하도록
        {
            if (!isJumping || jumpCount == 1)
            {
                isJumping = true;
                rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
                jumpCount++;
                if (jumpCount == 1) anim.SetBool("IsJumping1", true);
            }
        }

        // Sliding
        if (sDown && !sUp && !isSliding) StartSliding();
        else if (!sDown && isSliding && sUp) StopSliding();

        if (isSliding)
        {
            anim.SetBool("IsSliding", true);
            anim.SetBool("IsSlidingStop", false);
            capCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y / 3);
            capCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - 2);
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, Vector2.down * groundCheckDistance, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector2.up * groundCheckDistance, new Color(0, 1, 0));
        Debug.DrawRay(rigid.position, Vector2.right * obstacleCheckDistance, new Color(0, 1, 0));

        //check whether player is on ground
        RaycastHit2D hitDown = Physics2D.Raycast(rigid.position, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
        if (hitDown.collider != null)
        {
            if (!isGrounded)
            {
                isGrounded = true;
                jumpCount = 0;
            }
        }
        else
        {
            isGrounded = false;
        }

        //Lv1, Lv2 Vertical Obstacle Check
        RaycastHit2D hitRight = Physics2D.Raycast(rigid.position, Vector2.right, obstacleCheckDistance);
        RaycastHit2D hitUp = Physics2D.Raycast(rigid.position, Vector2.up, obstacleCheckDistance);
        if (!isSliding)
        {
            if ((hitRight.collider != null && hitRight.collider.CompareTag("Ground")) ||
                (hitUp.collider != null && hitUp.collider.CompareTag("Ground")))
            {
                OnDamaged();
            }
        }


    }

    private void StartSliding()
    {
        isSliding = true;
        anim.SetBool("IsSilding", true);
        anim.SetBool("IsSildingStop", false);
        capCollider.size = new Vector2(capCollider.size.x, capCollider.size.y / 3);
        capCollider.offset = new Vector2(capCollider.offset.x, capCollider.offset.y - 2);
        

        capCollider.enabled = false;
        capCollider.enabled = true;

        Debug.Log("슬라이딩 여부: " + isSliding);
        Debug.Log("\n" + originalColliderSize);
        Debug.Log("\n" + capCollider.size);
    }

    private void StopSliding()
    {
        isSliding = false;
        anim.SetBool("IsSilding", false);
        anim.SetBool("IsSildingStop", true);
        capCollider.size = originalColliderSize;
        capCollider.offset = originalColliderOffset;
    }

    private void OnDamaged()
    {
        anim.SetBool("IsHit", true);
        anim.SetBool("IsOkay", false);
        gameManager.DecreasePlayerHealth();
        Debug.Log("Player Health: " + gameManager.playerHealth);
        
        Debug.Log("공격받음");
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        gameObject.layer = 9;
        // 피격애니메이션
        Invoke("OffDamaged", 2); // 3초 후 무적 상태 해제
    }

    void OffDamaged()
    {
        gameObject.layer = 8; // 원래 레이어로 복구
        anim.SetBool("IsHit", false);
        anim.SetBool("IsOkay", true);
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("End"))
        {
            //gameManager.GameEnding();
            Debug.Log("end닿음");
        }

        if (collision.gameObject.CompareTag("Obstacle") && (gameObject.layer!=9))
        {
            OnDamaged();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false; // 바닥과의 충돌은 항상 허용
            anim.SetBool("IsJumping1", false);
        }
    }


    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        gameManager.GameOver();
    }
}