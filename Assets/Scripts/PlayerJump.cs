using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;
    [Header(("Variables Salto"))]
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

    //Getters
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

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
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundRadius, Vector2.down, groundCheckDistance, groundMask);

        // Verificar si el circleCast está en contacto con el layer Ground
        if (hit.collider != null) 
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    //Se encarga de actualizar las mejoras del salto; gravedad dinámica, coyote time y buffer jump
    public void JumpUpdates()
    {
        //Este if else verifica el coyote time 
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            hasJumped = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        //Este if verifica el buffer jump time 
        if (bufferJumpCounter > 0)
        {
            bufferJumpCounter -= Time.deltaTime;

            //Si tocamos el suelo y hay un buffer mayor que 0, saltamos automáticamente
            if (isGrounded)
            {
                playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce);
                //Ajustar la gravedad normal para el personaje
                playerController.rb.gravityScale = playerController.normalGravity;
                coyoteCounter = 0; // Reiniciar el contador de coyote time al saltar
                bufferJumpCounter = 0; // Reiniciar el contador de buffer jump al saltar

                // NUEVO: Si el jugador ya no está presionando el botón en el frame de aterrizaje, cortamos el salto inmediatamente
                if (!playerController.isJumpHeld)
                {
                    playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, playerController.rb.linearVelocity.y * jumpRelease);
                    playerController.rb.gravityScale = playerController.fallGravity;
                }

            }
        }
     


        //Este if else actualiza  la gravedad dinámica del personaje
        if (isGrounded)
        {
            playerController.rb.gravityScale = playerController.normalGravity;
        }
        else if (playerController.rb.linearVelocity.y < -0.1f)
        {
            playerController.rb.gravityScale = playerController.fallGravity;
        }
    }

    public void JumpHold()
    {
        if ((isGrounded || coyoteCounter > 0) && !hasJumped) {  
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce);
            //Ajustar la gravedad normal para el personaje
            playerController.rb.gravityScale = playerController.normalGravity;
            coyoteCounter = 0; // Reiniciar el contador de coyote time al saltar
            hasJumped = true;
        }
        else
        {
            bufferJumpCounter = bufferJumpTime; // Damos margen de tiempo para el buffer jump
        }
    }

    public void JumpRelease()
    {
        // Si el jugador está subiendo (saltando) y suelta la tecla, reducir la fuerza de salto
        if (playerController.rb.linearVelocity.y > 0)
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, playerController.rb.linearVelocity.y * jumpRelease);
        }
        playerController.rb.gravityScale = playerController.fallGravity;
        hasJumped = true;
    }

    private void OnDrawGizmos()
    {
        if(isGrounded)        
            Gizmos.color = Color.green;        
        else        
            Gizmos.color = Color.red;
        

        Vector3 checkPosition = transform.position + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireSphere(checkPosition, groundRadius);

    }
}
