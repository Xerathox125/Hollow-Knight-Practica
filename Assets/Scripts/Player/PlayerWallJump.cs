using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWallJump : MonoBehaviour
{
    PlayerController playerController;

    [Header("Wall Jump")]
    public float rayCheckDistance;                     // Distancia del raycast para detectar paredes
    public LayerMask wallLayer;                        // Identifica que el layer será considerado una pared
    private float graceTimer = 0;

    [Header("Bloqueo de Input")]
    public float wallJumpDuration = 0.2f;
    private float wallJumpTimer;
    public bool isWallJumpActive { get; private set; }

    public float slideWallSpeed;
    public float wallJumpForceX;
    public float wallJumpForceY;

    public float wallJumpCoolDown;
    private float coolDownWallTimer = 0;
    private bool canAttach = true;

    public float unStickCooldown;

    private bool isWall;
    private bool isWallJump;

    //Getters & Setters
    public bool IsWall => isWall;
    public bool IsWallJump => isWallJump;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        if (isWallJumpActive)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
                isWallJumpActive = false;
        }

        if (playerController.swim != null && playerController.swim.IsSwim)
        {
            isWall = false;
            isWallJump = false;
            return;
        }

        DetectWallJump();
        UpdateTimers();
        UpdateWallJump();
    }

    public void UpdateTimers()
    {
        if (coolDownWallTimer > 0)
        {
            coolDownWallTimer -= Time.fixedDeltaTime;
            if (coolDownWallTimer <= 0f)
            {
                coolDownWallTimer = 0f;
                isWallJump = false;
                canAttach = true;
            }
        }
    }

    public void DetectWallJump()
    {
        if (!canAttach)
        {
            isWall = false;
            return;
        }

        if (playerController.jump.IsGrounded && playerController.jump != null)
        {
            isWall = false;
            return;
        }

        Vector2 direction = playerController.movement.IsFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayCheckDistance, wallLayer);
        isWall = hit.collider != null;
    }

    public void UpdateWallJump()
    {
        if (isWall && !playerController.jump.IsGrounded)
        {
            Vector2 input = playerController.controles.Player.Move.ReadValue<Vector2>();
            bool isPressingWall = playerController.movement.IsFacingRight ? (input.x > 0.1f) : (input.x < -0.1f);

            if (!isPressingWall)
            {
                graceTimer += Time.deltaTime;

                if (graceTimer >= unStickCooldown)
                {
                    isWall = false;
                    canAttach = false;
                    coolDownWallTimer = unStickCooldown;
                    graceTimer = 0;
                    return;
                }
            }
            else
                graceTimer = 0;

            if (isPressingWall && !isWallJump && playerController.rb.linearVelocity.y < slideWallSpeed)
                playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, slideWallSpeed);

            if (playerController.controles.Player.Jump.triggered) WallJump();
        }
        else graceTimer = 0;
    }

    public void WallJump()
    {
        isWallJumpActive = true;
        wallJumpTimer = wallJumpDuration;
        isWallJump = true;
        coolDownWallTimer = wallJumpCoolDown;
        canAttach = true;

        Vector2 dirX = playerController.movement.IsFacingRight ? Vector2.left : Vector2.right;
        playerController.rb.linearVelocity = new Vector2(dirX.x * wallJumpForceX, wallJumpForceY);

        playerController.movement.SetFacing(dirX.x > 0f);
        isWall = false;
    }
}