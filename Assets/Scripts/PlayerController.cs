using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : LivingObject
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private BoxCollider2D collider;
    private LayerMask jumpableMask;

    private Vector2 startPosition;
    private float directionX = 0f;
    private bool jump = false;
    private int jumpCount = 0;
    private int maxJumpCount = 2;
    private bool eyeBlink = false;
    private float eyeBlinkInterval = 5f;
    private float eyeBlinkCountdown = 0f;
    private bool teleport = false;
    private bool teleporting = false;
    private RigidbodyConstraints2D savedConstraints;

    public float runSpeed = 7f;
    public float jumpForce = 12f;
    public float teleportDistance = 5f;
    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsFacingRight
    {
        get
        {
            return !sprite.flipX;
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        jumpableMask = LayerMask.GetMask("Jumpable");

        startPosition = transform.position;
    }

    private void Update()
    {
        // Out of map check
        OutOfMapCheck();

        // Moving Check
        MovingCheck();

        // Ground check
        GroundCheck();

        // Move left, right
        directionX = Input.GetAxis("Horizontal");
        if (directionX > 0.01f)
        {
            rb.velocity = new Vector2(directionX * runSpeed, rb.velocity.y);
        }
        else if (directionX < -0.01f)
        {
            rb.velocity = new Vector2(directionX * runSpeed, rb.velocity.y);
        }

        // Jump & double jump
        bool jumpPressed = Input.GetButtonDown("Jump");
        if (jumpPressed && jumpCount < maxJumpCount && !jump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jump = true;
            jumpCount++;
        }
        if (!jump && IsGrounded)
        {
            jumpCount = 0;
        }

        // Idle eye blink
        if (IsMoving)
        {
            eyeBlinkCountdown = eyeBlinkInterval;
            eyeBlink = false;
        }
        else
        {
            eyeBlinkCountdown -= Time.deltaTime;
            if (eyeBlinkCountdown <= 0)
            {
                eyeBlink = true;
                eyeBlinkCountdown = eyeBlinkInterval;
            }
        }

        // Teleport
        bool teleportPressed = Input.GetButtonDown("Fire1");
        if (teleportPressed && !teleporting)
        {
            teleport = true;
            teleporting = true;
        }
    }

    private void FixedUpdate()
    {
        Draw();
    }

    private void Draw()
    {
        // Set grounded
        anim.SetBool("isGrounded", IsGrounded);

        // Animation left, right
        anim.SetFloat("speedX", Math.Abs(rb.velocity.x));
        if (rb.velocity.x > 0.01f)
        {
            sprite.flipX = false;
        }
        else if (rb.velocity.x < -0.01f)
        {
            sprite.flipX = true;
        }

        // Animation jump & double jump
        anim.SetFloat("speedY", rb.velocity.y);
        if (jump)
        {
            anim.SetTrigger("jump");
            jump = false;
        }

        // Animation teleport
        if (teleport)
        {
            anim.SetTrigger("teleport");
            teleport = false;
        }
    }

    private void IdleAnim_Completed()
    {
        // Animation idle eye blink
        if (eyeBlink)
        {
            anim.SetTrigger("eyeBlink");
            eyeBlink = false;
        }
    }

    public override void Kill()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        transform.position = startPosition;
        rb.bodyType = RigidbodyType2D.Dynamic;
        sprite.flipX = false;
    }

    public override void TakeDamage(float damage)
    {
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0, Vector2.down, 0.1f, jumpableMask);
        IsGrounded = hit.collider != null;
    }

    private void MovingCheck()
    {
        IsMoving = Math.Abs(rb.velocity.x) > 0.01f || Math.Abs(rb.velocity.y) > 0.01f;
    }

    private void OutOfMapCheck()
    {
        if (transform.position.y < -15f)
        {
            rb.bodyType = RigidbodyType2D.Static;
            Kill();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            rb.bodyType = RigidbodyType2D.Static;
            Invoke("Kill", 0.5f);
        }
    }

    private void TeleAnim_Start()
    {
        savedConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
    }

    private void TeleAnim_DoTele()
    {
        bool facingRight = sprite.flipX == false;
        transform.position = new Vector2(transform.position.x + teleportDistance * (facingRight ? 1 : -1), transform.position.y);
    }

    private void TeleAnim_End()
    {
        rb.constraints = savedConstraints;
        teleporting = false;
    }
}
