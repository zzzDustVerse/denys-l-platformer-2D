using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Movement : MonoBehaviour
{
    public int hp = 100;
    public GameObject gb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 35f;
    public float deceleration = 45f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpHoldForce = 25f;
    public float maxJumpHoldTime = 0.3f;
    public float fallMultiplier = 2.5f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Wall Jump")]
    public float wallJumpForce = 14f;
    public float wallJumpHorizontalForce = 10f;
    public float wallCheckDistance = 0.15f;
    public float wallJumpCooldown = 0.15f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.6f;

    [Header("Ground")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private Vector2 movementInput;

    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;

    private bool touchingLeftWall;
    private bool touchingRightWall;

    private bool isJumping;
    private float jumpHoldTimer;
    private float wallJumpTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (hp <= 0)
        {
            if (gb != null)
                Destroy(gb);

            return;
        }

        // WASD / Arrow input
        movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Prevent diagonal dash from being faster
        if (movementInput.magnitude > 1f)
        {
            movementInput.Normalize();
        }

        CheckGround();
        CheckWalls();

        // Start jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (touchingLeftWall || touchingRightWall)
            {
                WallJump();
            }
        }

        // Release jump to stop gaining height
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;

            // Cut upward velocity for a shorter jump
            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(
                    rb.velocity.x,
                    rb.velocity.y * jumpCutMultiplier
                );
            }
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) &&
            canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        // Wall jump timer
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
            ApplyGravity();

            // Apply extra upward force while holding jump
            if (isJumping &&
                Input.GetButton("Jump") &&
                jumpHoldTimer < maxJumpHoldTime &&
                rb.velocity.y > 0f)
            {
                rb.velocity += Vector2.up *
                    jumpHoldForce * Time.fixedDeltaTime;

                jumpHoldTimer += Time.fixedDeltaTime;
            }
        }
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Move()
    {
        // Don't immediately cancel a wall jump
        if (wallJumpTimer > 0f)
            return;

        float targetSpeed = movementInput.x * moveSpeed;
        float difference = targetSpeed - rb.velocity.x;

        float rate = Mathf.Abs(movementInput.x) > 0.01f
            ? acceleration
            : deceleration;

        float movement = difference * rate * Time.fixedDeltaTime;

        rb.velocity = new Vector2(
            rb.velocity.x + movement,
            rb.velocity.y
        );

        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed),
            rb.velocity.y
        );
    }

    // =========================================================
    // NORMAL JUMP
    // =========================================================

    private void Jump()
    {
        rb.velocity = new Vector2(
            rb.velocity.x,
            jumpForce
        );

        isJumping = true;
        jumpHoldTimer = 0f;
    }

    // =========================================================
    // WALL JUMP
    // =========================================================

    private void WallJump()
    {
        float direction;

        // Wall on left -> jump right
        if (touchingLeftWall)
        {
            direction = 1f;
        }
        // Wall on right -> jump left
        else
        {
            direction = -1f;
        }

        rb.velocity = new Vector2(
            direction * wallJumpHorizontalForce,
            wallJumpForce
        );

        // Allow variable height for wall jumps too
        isJumping = true;
        jumpHoldTimer = 0f;

        // Prevent normal movement from immediately cancelling
        wallJumpTimer = wallJumpCooldown;
    }

    // =========================================================
    // GROUND CHECK
    // =========================================================

    private void CheckGround()
    {
        Bounds bounds = playerCollider.bounds;

        Vector2 origin = new Vector2(
            bounds.center.x,
            bounds.min.y
        );

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    // =========================================================
    // WALL CHECK
    // =========================================================

    private void CheckWalls()
    {
        Bounds bounds = playerCollider.bounds;
        Vector2 center = bounds.center;

        RaycastHit2D leftHit = Physics2D.Raycast(
            center,
            Vector2.left,
            wallCheckDistance + bounds.extents.x,
            groundLayer
        );

        RaycastHit2D rightHit = Physics2D.Raycast(
            center,
            Vector2.right,
            wallCheckDistance + bounds.extents.x,
            groundLayer
        );

        touchingLeftWall = leftHit.collider != null;
        touchingRightWall = rightHit.collider != null;
    }

    // =========================================================
    // GRAVITY
    // =========================================================

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    // =========================================================
    // DASH
    // =========================================================

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Vector2 dashDirection = movementInput;

        // No direction = dash right
        if (dashDirection.magnitude < 0.1f)
        {
            dashDirection = Vector2.right;
        }

        dashDirection.Normalize();

        float oldGravity = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = oldGravity;
        rb.velocity *= 0.2f;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
            return;

        Bounds bounds = col.bounds;

        // Ground check
        Gizmos.color = Color.green;

        Vector3 groundStart = new Vector3(
            bounds.center.x,
            bounds.min.y,
            0f
        );

        Gizmos.DrawLine(
            groundStart,
            groundStart + Vector3.down * groundCheckDistance
        );

        // Wall checks
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            bounds.center,
            bounds.center + Vector3.left *
            (bounds.extents.x + wallCheckDistance)
        );

        Gizmos.DrawLine(
            bounds.center,
            bounds.center + Vector3.right *
            (bounds.extents.x + wallCheckDistance)
        );
    }
}