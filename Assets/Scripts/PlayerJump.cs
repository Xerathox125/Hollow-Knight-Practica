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

    public void JumpHold()
    {
        if (isGrounded)         
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce);               
    }

    public void JumpRelease()
    {
        // Si el jugador está subiendo (saltando) y suelta la tecla, reducir la fuerza de salto
        if (playerController.rb.linearVelocity.y > 0)
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, playerController.rb.linearVelocity.y * jumpRelease);
        }
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
