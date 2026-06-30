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
    public bool IsMoving => isMoving;
    public bool IsFacingRight => isFacingRight;

    private void Awake() => playerController = GetComponent<PlayerController>();

    public void OnUpdate()
    {
        isMoving = playerController.moveInput.x != 0;
    }

    public void Move()
    {
        Vector2 move = playerController.moveInput;
        bool pressInputX = Mathf.Abs(move.x) > 0.5f;
        float currentSpeed = playerController.crouch.isCrouching && playerController.jump.IsGrounded
            ? playerController.crouchSpeed
            : playerController.currentSpeed;

        if (onKnockBack) return;
        if (playerController.wallJump != null && playerController.wallJump.isWallJumpActive) return;
        if (playerController.wallJump != null && playerController.wallJump.IsWallJump) return;

        playerController.currentSpeed = playerController.swim.IsSwim ? playerController.swim.speedSwim : moveSpeed;

        if (playerController.dash?.isDash == true) return;

        if (playerController.wallJump != null && playerController.wallJump.IsWall)
        {
            float inputX = playerController.moveInput.x;
            bool isPressingTowardsWall = isFacingRight ? (inputX > 0.1f) : (inputX < -0.1f);
            if (!isPressingTowardsWall) move.x = 0;
        }

        if (pressInputX || (playerController.jump != null && playerController.jump.IsGrounded))
            playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y);

        if (pressInputX)
        {
            if (move.x > 0 && !isFacingRight) Flip();
            else if (move.x < 0 && isFacingRight) Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector2(isFacingRight ? 1f : -1f, 1f);
    }

    public void SetFacing(bool right)
    {
        if (isFacingRight != right) Flip();
    }
}