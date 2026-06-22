using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [Header("Move")]
    public float moveSpeed; // Velocidad base de movimiento
    private bool isFacingRight = true; // Dirección actual del sprite
    private bool isMoving; // Estado de movimiento

    public bool IsMoving => isMoving; // Propiedad pública para acceder al estado
    public bool IsFacingRight => isFacingRight;


    private void Awake() => playerController = GetComponent<PlayerController>(); // Cache del controlador

    public void OnUpdate() // Actualiza lógica de movimiento
    {
        isMoving = playerController.moveInput.x != 0; // Verifica si hay input horizontal
    }

    public void Move() // Aplica la física de movimiento
    {

        // Si estamos en el periodo de bloqueo del WallJump, impedimos el movimiento horizontal
        if (playerController.wallJump != null && playerController.wallJump.isWallJumpActive)
        {
            return; // Salimos de la función sin aplicar el velocity.x del input
        }

        // Si estamoos en muro y no hemoe caído o saltado, bloquear movimiento
        if (playerController.wallJump != null && playerController.wallJump.IsWallJump)
        {
            return;
        }


        if (playerController.swim.IsSwim) // Ajusta velocidad si está nadando
        {
            playerController.currentSpeed = playerController.swim.speedSwim;
        }
        else // Velocidad normal terrestre
        {
            playerController.currentSpeed = moveSpeed;
        }

        if (playerController.dash?.isDash == true) return; // Cancela movimiento si está haciendo dash

        Vector2 move = playerController.moveInput; // Obtiene input del controlador
        bool pressInputX = Mathf.Abs(move.x) > 0.5f;


        float currentSpeed = playerController.crouch.isCrouching ? playerController.crouchSpeed : playerController.currentSpeed; // Elige velocidad según estado (agachado vs normal)

        //Verificamos el margen de tiempo cuando estamos en el muro y bloqueamos movimiento del jugador durante ese margen de tiempo
        if (playerController.wallJump != null && playerController.wallJump.IsWall)
        {
            //Verificar el input contrario
            int dirFacing;
            if (isFacingRight)
            {
                dirFacing = 1;
            }
            else
            {
                dirFacing = -1;
            }

            if (move.x * dirFacing < -0.2f)
            {
                //verificar el oppositeInputTime
                if (!playerController.wallJump.CanMoveOpposite())
                {
                    move.x = 0;
                }
            }
        }

        if (pressInputX || (playerController.jump != null && playerController.jump.IsGrounded)) //Si estamos oprimiendo el input X y estamos en el suelo
        {
            playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y); // Aplica velocidad en X al rigidbody del PlayerController
        }

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
        if (isFacingRight != right)
        {
            Flip();
        }
    }
}