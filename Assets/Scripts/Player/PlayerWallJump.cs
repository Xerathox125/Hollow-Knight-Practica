using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWallJump : MonoBehaviour
{
    PlayerController playerController;

    [Header("Wall Jump")]
    public float rayCheckDistance;                     // Distancia del raycast para detectar paredes
    public LayerMask wallLayer;                        // Identifica que el layer será considerado una pared
    private float graceTimer = 0;                      // Asegúrate de tener esta variable definida en la clase

    [Header("Bloqueo de Input")]
    public float wallJumpDuration = 0.2f;              // El tiempo que el jugador no puede controlar el input
    private float wallJumpTimer;
    public bool isWallJumpActive { get; private set; } // Flag para saber si estamos en el "aire" del salto

    //Variables slide wall e impulso en muro
    public float slideWallSpeed;                       // Gravedad cuando estás en la pared
    public float wallJumpForceX;                       // Impulso de la fuerza en X cuando saltamos desde un muro
    public float wallJumpForceY;                       // Impulso de la fuerza en Y cuando saltamos desde un muro

    //Variables cooldown para saltar entre paredes
    public float wallJumpCoolDown;                     // Tiempo durante el cual no puede volver a sostenerse del muro después de hacer un salto
    private float coolDownWallTimer = 0;               // Contador para disminuir el cooldown
    private bool canAttach = true;                     // Cuando está en true significa que terminó el cooldown

    //Variables para saltar correctamente de pared en otra
    public float unStickCooldown;                      // Tiempo espera para volver a sostenerse en un muro luego de dejarnos caer

    private bool isWall;                               // Si estamos sostenidos en un muro
    private bool isWallJump;                           // Si estamos saltando de un muro

    // Dirección real de la pared, independiente del sprite
    private bool wallIsOnRight;

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

        //Verificar que el player no esté nadando
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

        // Durante un ataque de pared, el sprite está volteado pero la pared sigue en el mismo lado.
        // Usamos wallIsOnRight (guardado cuando se detectó la pared) para el raycast.
        bool isWallAttacking = playerController.attacks != null
                               && playerController.attacks.IsAttack
                               && playerController.attacks.WasOnWallAttack;

        Vector2 direction;
        if (isWallAttacking)
            // La pared está al lado opuesto de donde ahora mira el sprite
            direction = playerController.movement.IsFacingRight ? Vector2.left : Vector2.right;
        else
            direction = playerController.movement.IsFacingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayCheckDistance, wallLayer);

        if (hit.collider != null)
        {
            isWall = true;
            wallIsOnRight = direction == Vector2.right; // Guarda en qué lado está la pared real
        }
        else
        {
            isWall = false;
        }
    }

    public void UpdateWallJump()
    {
        if (isWall && !playerController.jump.IsGrounded)
        {
            Vector2 input = playerController.controles.Player.Move.ReadValue<Vector2>();

            // Durante ataque de pared usamos la dirección real de la pared, no IsFacingRight
            bool isWallAttacking = playerController.attacks != null
                                   && playerController.attacks.IsAttack
                                   && playerController.attacks.WasOnWallAttack;

            bool isPressingWall;
            if (isWallAttacking)
                isPressingWall = wallIsOnRight ? (input.x > 0.1f) : (input.x < -0.1f);
            else
                isPressingWall = playerController.movement.IsFacingRight ? (input.x > 0.1f) : (input.x < -0.1f);

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