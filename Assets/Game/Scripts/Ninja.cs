using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ninja : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 5f;
    public float staminaDrainPerHit = 10f;
    public Image staminaBar;

    [Header("Attack")]
    public GameObject attackHitbox;
    public bool canAttackInAir = true;
    public float attackCooldown = 0.5f;

    [Header("Hit Response")]
    public float knockbackForce = 5f;

    [Header("Jumping")]
    public int maxJumps = 2;
    private int jumpCount;

    [Header("SFX")]
    public AudioSource[] firstJumpSFX;
    public AudioSource[] secondJumpSFX;
    public AudioSource[] attacksSFX;
    public AudioSource[] deathSFX;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public TrailRenderer dashTrail;
    public TrailRenderer moveTrail;

    public PlayerHitGlow PlayerHitGlow;

    private float lastDashTime;
    private bool isDashing;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private float lastAttackTime;
    private float currentStamina;
    private bool isGrounded;
    private bool isJumping;
    private bool isDead;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        currentStamina = maxStamina;
        InvokeRepeating(nameof(DrainStaminaOverTime), 1f, 1f);
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        HandleAnimations();
        CheckGroundStatus();
        UpdateStaminaUI();
        UpdateMoveTrail();
    }

    void FixedUpdate()
    {
        if (isDead || isDashing) return;
        Move();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        UpdateFacingDirection();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            var jumpForceToUse = (jumpCount == 1) ? jumpForce : jumpForce * 0.8f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForceToUse);
            jumpCount++;
            isJumping = true;
            animator.SetBool("Jump", true);

            int jumpSFXIndex = Random.Range(0, (jumpCount == 1 ? firstJumpSFX : secondJumpSFX).Length);
            (jumpCount == 1 ? firstJumpSFX : secondJumpSFX)[jumpSFXIndex].Play();
        }

        // Attack
        bool canAttack = Time.time - lastAttackTime > attackCooldown;
        if (Input.GetKeyDown(KeyCode.K) && canAttack)
        {
            if (isGrounded || canAttackInAir)
            {
                lastAttackTime = Time.time;
                int attackIndex = Random.Range(1, 3);
                int attackSFXIndex = Random.Range(0, attacksSFX.Length);
                attacksSFX[attackSFXIndex].Play();
                animator.SetInteger("Attack", attackIndex);
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - lastDashTime > dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        
        int jumpSFXIndex = Random.Range(0, (jumpCount == 1 ? firstJumpSFX : secondJumpSFX).Length);
        (jumpCount == 1 ? firstJumpSFX : secondJumpSFX)[jumpSFXIndex].Play();

        if (dashTrail != null) dashTrail.emitting = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2((sr.flipX ? -1 : 1) * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        if (dashTrail != null) dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetBool("Run", Mathf.Abs(horizontalInput) > 0.1f);

        // Flip direction
        if (sr.flipX && horizontalInput == 0)
        {
            return;
        }
        
        sr.flipX = horizontalInput < 0;

        // Run animation
    }

    private void UpdateFacingDirection()
    {
        SetHitboxDirection(!sr.flipX); // true = right, false = left
    }

    private void SetHitboxDirection(bool facingRight)
    {
        if (attackHitbox != null)
        {
            Vector3 localPos = attackHitbox.transform.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (facingRight ? 1 : -1);
            attackHitbox.transform.localPosition = localPos;
        }
    }

    private void HandleAnimations()
    {
        animator.SetBool("Jump", isJumping && rb.velocity.y > 0);
        animator.SetBool("Fall", rb.velocity.y < 0 && !isGrounded);
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
            isJumping = false;
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
        }
    }

    private void DrainStaminaOverTime()
    {
        if (isDead) return;

        currentStamina -= staminaDrainPerSecond;

        if (currentStamina <= 0)
        {
            Die();
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }

    private void UpdateMoveTrail()
    {
        if (moveTrail == null) return;

        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f && isGrounded && !isDashing;
        moveTrail.emitting = isMoving;
    }

    public void TakeHit(Vector2 attackerPosition)
    {
        if (isDead) return;

        animator.SetInteger("TakeHit", Random.Range(0, 2));

        Vector2 direction = (transform.position.x < attackerPosition.x) ? Vector2.left : Vector2.right;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        currentStamina -= staminaDrainPerHit;

        if (currentStamina <= 0)
        {
            Die();
        }
    }

    public void TakeFruitHit()
    {
        if (isDead) return;

        currentStamina -= staminaDrainPerHit;
        PlayerHitGlow.OnHit();

        if (currentStamina <= 0)
        {
            Die();
        }
    }

    public void AddStamina(int toAdd)
    {
        currentStamina += toAdd;
    }

    public void ResetAttack()
    {
        animator.SetInteger("Attack", 0);
    }

    private void Die()
    {
        deathSFX[Random.Range(0, deathSFX.Length)].Play();

        isDead = true;
        animator.SetBool("Dead", true);
        rb.velocity = Vector2.zero;
        
        GameManager.Instance.GameOver();
    }

    public void EnableHitbox() => attackHitbox.SetActive(true);
    public void DisableHitbox() => attackHitbox.SetActive(false);

    public void ResetPlayer()
    {
        currentStamina = maxStamina;
        isDead = false;
        gameObject.SetActive(true);
    }
}
