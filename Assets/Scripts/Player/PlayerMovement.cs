using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private bool isFacingRight = true;
    private bool isMoving;

    public bool IsMoving => isMoving;

    private void Awake() => playerController = GetComponent<PlayerController>();

    // Cambiado de Move() a OnUpdate() para actualizar flags de movimiento en Update
    public void OnUpdate()
    {
        isMoving = playerController.moveInput.x != 0;
    }

    public void Move()
    {
        if (playerController.dash?.isDash == true) return;

        Vector2 move = playerController.moveInput; // Input optimizado

        float currentSpeed = playerController.crouch.isCrouching ? playerController.crouchSpeed : playerController.speed;
        playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y);

        if (move.x > 0 && !isFacingRight) Flip();
        else if (move.x < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f); // Simplificación visual de flip sin ifs anidados
    }
}