using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private bool isFacingRight = true;
    private bool isMoving;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Move()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // Elegimos la velocidad dinámica: si está agachado usamos crouchSpeed, si no, usamos speed
        float currentSpeed = playerController.crouch.isCrouching ? playerController.crouchSpeed : playerController.speed;

        // Aplicamos la velocidad calculada al rigidbody
        playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y);

        isMoving = move.x != 0;

        if (move.x > 0 && !isFacingRight)
            Flip();
        else if (move.x < 0 && isFacingRight)
            Flip();
    }


    private void Flip() //Controla el cambio de vista del player según la dirección del movimiento
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            transform.localScale = new Vector3(1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1);
        }
    }

    public bool IsMoving //Getter de la variable isMoving para saber si nos estamos moviendo
    {
        get{ return isMoving; }
    }
}
