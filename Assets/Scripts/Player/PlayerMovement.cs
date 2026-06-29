using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [Header("Move")]
    public float moveSpeed;            // Velocidad base de movimiento
    private bool isFacingRight = true; // Dirección actual del sprite
    private bool isMoving;             // Estado de movimiento
    [HideInInspector] public bool onKnockBack;


    //Getters y Setters
    public bool IsMoving => isMoving;  // Propiedad pública para acceder al estado
    public bool IsFacingRight => isFacingRight;


    private void Awake() => playerController = GetComponent<PlayerController>(); // Cache del controlador

    public void OnUpdate() // Actualiza lógica de movimiento
    {
        isMoving = playerController.moveInput.x != 0; // Verifica si hay input horizontal
    }

    public void Move() // Aplica la física de movimiento
    {
        Vector2 move = playerController.moveInput; // Obtiene input del controlador
        bool pressInputX = Mathf.Abs(move.x) > 0.5f;
        float currentSpeed = playerController.crouch.isCrouching && playerController.jump.IsGrounded ? playerController.crouchSpeed : playerController.currentSpeed; // Elige velocidad según estado (agachado vs normal)

        if (onKnockBack) return; // Si ocurre knockback, return

        if (playerController.wallJump != null && playerController.wallJump.isWallJumpActive) return; // Si estamos haciendo haciendo WallJump, impedimos el movimiento horizontal y salimos de la función sin aplicar el velocity.x del input

        if (playerController.wallJump != null && playerController.wallJump.IsWallJump) return; // Si estamoos en muro y no hemoe caído o saltado, bloquear movimiento

        playerController.currentSpeed = playerController.swim.IsSwim ? playerController.swim.speedSwim : moveSpeed; // Ajusta la velocidad si está nadando, si está nadando le aplicamos la velocidad de nado, sino la velocidad normal

        if (playerController.dash?.isDash == true) return; // Cancela movimiento si está haciendo dash
            
        if (playerController.wallJump != null && playerController.wallJump.IsWall) // Si estamos en un muro pero no hemos saltado, bloqueamos movimiento del jugador
        {
            float inputX = playerController.moveInput.x;
            bool isPressingTowardsWall = isFacingRight ? (inputX > 0.1f) : (inputX < -0.1f); // Si el jugador intenta moverse en dirección opuesta a la pared, anulamos el input X
            if (!isPressingTowardsWall) move.x = 0;            
        }

        if (pressInputX || (playerController.jump != null && playerController.jump.IsGrounded)) //Si estamos oprimiendo el input X y estamos en el suelo
            playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y); // Aplica velocidad en X al rigidbody del PlayerController
        
        if (pressInputX)
        {
            if (move.x > 0 && !isFacingRight) Flip(); // Voltea sprite a la derecha
            else if (move.x < 0 && isFacingRight) Flip(); // Voltea sprite a la izquierda
        }

    }

    private void Flip() // Cambia la escala para voltear al jugador
    {
        isFacingRight = !isFacingRight; // Invierte el estado lógico
        transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f); // Aplica cambio visual en el transform
    }

    public void SetFacing(bool right)
    {
        if (isFacingRight != right) Flip();        
    }
}