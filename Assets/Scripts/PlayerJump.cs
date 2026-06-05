using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;
    [Header(("Variables Salto"))]
    public float jumpForce;
    public float groundRadius;
    public float groundCheckDistance;
    public LayerMask groundMask;
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
        //CheckGround();
        Jump();
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

    private void Jump()
    {
        if (isGrounded)         
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce);
               
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
