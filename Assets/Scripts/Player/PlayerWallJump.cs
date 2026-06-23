using UnityEngine;
using UnityEngine.Rendering;

public class PlayerWallJump : MonoBehaviour
{
    PlayerController playerController;

    [Header("Wall Jump")]
    //Variables para crear el raycast
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
    private bool canAttach = true;                     // Cuando esté en true significa que terminó el cooldown

    //Variables para saltar correctamente de pared en otra, congelando inputs por milésimas de segundo
    public float unStickCooldown;                      // Tiempo espera para volver a sostenerse en un muro luego de dejarnos caer cuando estábamos sostenidos

    private bool isWall;                               // Si estamos sostenidos en un muro
    private bool isWallJump;                           // Si estamos saltando de un muro



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
                isWallJumpActive = false; // Recuperamos el control            
        }

        //Verificar que el player no está nadando
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

    public void DetectWallJump() // Método para saber si estamos detectando una pared
    {
        if (!canAttach) //Si no podemos attacharnos a la pared, entonces no detectamos la pared
        {
            isWall = false;
            return;
        }

        if (playerController.jump.IsGrounded && playerController.jump != null) // Si estmamos en el suelo, no detectamos la pared
        {
            isWall = false;
            return;
        }

        //Creacion del raycast que sale frente al jugador para detectar muros
        Vector2 direction;
        direction = playerController.movement.IsFacingRight ? Vector2.right : Vector2.left; //Ver hacia donde apunta el jugador cuando detecta un raycast

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayCheckDistance, wallLayer);

        //Verificamos si estamos en pared
        isWall = (hit.collider != null) ? true : false;       
    }

    public void UpdateWallJump()
    {
        if (isWall && !playerController.jump.IsGrounded)
        {
            // 1. Obtenemos el input actual
            Vector2 input = playerController.controles.Player.Move.ReadValue<Vector2>();

            // 2. Determinamos si el jugador está presionando hacia el muro
            bool isPressingWall = playerController.movement.IsFacingRight ? (input.x > 0.1f) : (input.x < -0.1f);

            // 3. Lógica de despegue y ventana de salto
            if (!isPressingWall)
            {
                graceTimer += Time.deltaTime;

                if (graceTimer >= unStickCooldown)
                {
                    isWall = false;
                    canAttach = false;
                    coolDownWallTimer = unStickCooldown;
                    graceTimer = 0;
                    return; // Salimos: al no retornar, aplicaría slideWallSpeed, aquí se corta
                }
            }
            else
                graceTimer = 0; // Reseteamos si vuelve a presionar hacia el muro

            // 4. Aplicamos deslizamiento SOLO si estamos presionando hacia el muro (Esto evita que se quede "pegado" flotando si suelta la tecla)
            if (isPressingWall && !isWallJump && playerController.rb.linearVelocity.y < slideWallSpeed) playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, slideWallSpeed);

            // 5. Salto desde el muro
            if (playerController.controles.Player.Jump.triggered) WallJump();
        }
        else graceTimer = 0;
    }

    public void WallJump()
    {
        isWallJumpActive = true; // Iniciamos el estado de bloqueo
        wallJumpTimer = wallJumpDuration;
        isWallJump = true;
        coolDownWallTimer = wallJumpCoolDown;
        canAttach = true;

        // Aplicar fuerzas o velocidades en X y Y para el impulso del jugador cuando salta desde el muro
        Vector2 dirX = playerController.movement.IsFacingRight ? Vector2.left : Vector2.right;
        playerController.rb.linearVelocity = new Vector2(dirX.x * wallJumpForceX, wallJumpForceY);

        // Llamar a SetFacing desde PlayerMovement
        playerController.movement.SetFacing(dirX.x > 0f);
        isWall = false;
    }
}