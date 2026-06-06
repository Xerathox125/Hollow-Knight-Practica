using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Variables Salto")]
    public float jumpForce;
    public float groundRadius;
    public float groundCheckDistance;
    public LayerMask groundMask;
    public float jumpRelease;
    private bool isGrounded;

    [Header("Coyote Time")]
    public float coyoteTime;
    public float coyoteCounter;
    private bool hasJumped = false;

    [Header("Buffer Time")]
    public float bufferJumpTime;
    public float bufferJumpCounter;

    public bool IsGrounded => isGrounded;

    public void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        CheckGround();
        JumpUpdates();
    }

    public void CheckGround()
    {
        // OPTIMIZACIÓN: Asignación directa sin if/else
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundRadius, Vector2.down, groundCheckDistance, groundMask);
        isGrounded = hit.collider != null;
    }

    public void JumpUpdates()
    {
        // OPTIMIZACIÓN: Cachear la velocidad para evitar múltiples llamadas al motor de físicas
        Vector2 currentVel = playerController.rb.linearVelocity;

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            hasJumped = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (bufferJumpCounter > 0)
        {
            bufferJumpCounter -= Time.deltaTime;

            if (isGrounded)
            {
                currentVel.y = jumpForce;
                playerController.rb.gravityScale = playerController.normalGravity;
                coyoteCounter = 0;
                bufferJumpCounter = 0;

                if (!playerController.isJumpHeld)
                {
                    currentVel.y *= jumpRelease;
                    playerController.rb.gravityScale = playerController.fallGravity;
                }
            }
        }

        if (isGrounded)
        {
            playerController.rb.gravityScale = playerController.normalGravity;
        }
        else if (currentVel.y < -0.1f) // Usa la variable cacheada
        {
            playerController.rb.gravityScale = playerController.fallGravity;
        }

        // Aplicar la velocidad final una única vez
        playerController.rb.linearVelocity = currentVel;
    }

    public void JumpHold()
    {
        if ((isGrounded || coyoteCounter > 0) && !hasJumped)
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce);
            playerController.rb.gravityScale = playerController.normalGravity;
            coyoteCounter = 0;
            hasJumped = true;
        }
        else
        {
            bufferJumpCounter = bufferJumpTime;
        }
    }

    public void JumpRelease()
    {
        if (playerController.rb.linearVelocity.y > 0)
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, playerController.rb.linearVelocity.y * jumpRelease);
        }
        playerController.rb.gravityScale = playerController.fallGravity;
        hasJumped = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 checkPosition = transform.position + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireSphere(checkPosition, groundRadius);
    }
}