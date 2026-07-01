using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador principal

    [Header("Variables Salto")]
    public float jumpForce; // Fuerza aplicada al saltar
    public float groundRadius; // Radio del círculo para detectar suelo
    public float groundCheckDistance; // Distancia de detección del suelo
    public LayerMask groundMask; // Capas que cuentan como suelo
    public float jumpRelease; // Multiplicador para reducir la altura del salto al soltar el botón
    private bool isGrounded; // Estado que indica si está tocando el suelo
    public float waterJumpReduction = 2.5f; // Mientras más alto sea este valor, menos saltará el personaje en el agua

    [Header("Coyote Time")]
    public float coyoteTime; // Tiempo permitido para saltar tras dejar una plataforma
    public float coyoteCounter; // Contador descendente del coyote time
    private bool hasJumped = false; // Flag para evitar múltiples saltos

    [Header("Buffer Time")]
    public float bufferJumpTime; // Tiempo de espera para ejecutar un salto pre-presionado
    public float bufferJumpCounter; // Contador descendente del buffer de salto

    public bool IsGrounded => isGrounded; // Acceso público al estado de suelo

    public void Awake() => playerController = GetComponent<PlayerController>(); // Inicializa referencia al controlador

    public void OnUpdate() // Lógica de actualización de saltos
    {
        CheckGround(); // Verifica contacto con el suelo
        JumpUpdates(); // Procesa físicas y contadores de salto
    }

    public void CheckGround() // Ejecuta detección de suelo mediante CircleCast
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundRadius, Vector2.down, groundCheckDistance, groundMask); // Lanza círculo de detección
        isGrounded = hit.collider != null; // Actualiza el estado de suelo
    }

    public void JumpUpdates() // Gestión de estados, gravedades y buffers
    {
        // 1. Protección de Nado: Salimos inmediatamente si estamos en el agua
        if (playerController.swim != null && playerController.swim.IsSwim)
        {
            return;
        }
        Vector2 currentVel = playerController.rb.linearVelocity; // Captura velocidad actual

        if (isGrounded) // Si está tocando suelo
        {
            coyoteCounter = coyoteTime; // Resetea contador de coyote time
            hasJumped = false; // Permite saltar de nuevo
            playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad normal
        }
        else // Si está en el aire
        {
            coyoteCounter -= Time.deltaTime; // Reduce contador de coyote time
            if (currentVel.y < -0.1f) playerController.rb.gravityScale = playerController.fallGravity; // Aplica gravedad de caída al descender
        }

        if (bufferJumpCounter > 0) // Si hay un salto pendiente en buffer
        {
            bufferJumpCounter -= Time.deltaTime; // Reduce el contador

            if (isGrounded) // Si toca suelo mientras el buffer está activo
            {
                currentVel.y = jumpForce; // Aplica fuerza de salto
                coyoteCounter = 0; // Invalida coyote time
                bufferJumpCounter = 0; // Limpia el buffer

                if (!playerController.isJumpHeld) // Si el botón no está presionado al tocar suelo
                {
                    currentVel.y *= jumpRelease; // Reduce la fuerza del salto (salto corto)
                    playerController.rb.gravityScale = playerController.fallGravity; // Aplica gravedad de caída
                }
            }
        }

        float maxFallSpeed = -20f; // Límite de velocidad de caída
        if (currentVel.y < maxFallSpeed) currentVel.y = maxFallSpeed; // Clampeo de velocidad vertical para no atravesar suelos

        playerController.rb.linearVelocity = currentVel; // Aplica velocidad calculada al rigidbody
    }

    public void JumpHold() // Acción al presionar botón de salto
    {
        if (playerController.wallJump != null && playerController.wallJump.IsWall && playerController.jump.isGrounded) // si el player está en walljump, al preisonar salgo(jumphold), hace un impulso en diagonal hacia el lado contrario
        {
            playerController.wallJump.WallJump();
            return;
        }

        if (playerController.swim.IsSwim) // Si está en el agua
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce / waterJumpReduction); // Salto reducido
            return;
        }

        if (playerController.stairs.IsStairs) // Si está en escaleras
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce); // Salta desde escalera
            playerController.stairs.ExitStairs(0.2f); // Sale de la escalera con cooldown
            return; // Sale del método
        }

        if ((isGrounded || coyoteCounter > 0) && !hasJumped) // Si puede saltar (coyote o suelo)
        {
            playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, jumpForce); // Aplica fuerza
            playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad
            coyoteCounter = 0; // Limpia coyote
            hasJumped = true; // Marca que ha saltado
        }
        else // Si no puede saltar en el momento
        {
            bufferJumpCounter = bufferJumpTime; // Activa buffer de salto por si toca el suelo pronto
        }
    }

    public void JumpRelease() // Acción al soltar botón de salto
    {
        if (playerController.swim != null && playerController.swim.IsSwim) return; // En agua el salto es de fuerza fija, JumpRelease no debe interferir


        if (playerController.rb.linearVelocity.y > 0) // Si está subiendo
        {
            playerController.rb.linearVelocity = new Vector2(
                playerController.rb.linearVelocity.x,
                playerController.rb.linearVelocity.y * jumpRelease
            ); // Reduce velocidad vertical bruscamente
        }
        playerController.rb.gravityScale = playerController.fallGravity; // Aplica gravedad de caída
        hasJumped = true; // Marca salto como ejecutado
    }

    private void OnDrawGizmos() // Dibujo de debug para detección de suelo
    {
        Gizmos.color = isGrounded ? Color.green : Color.red; // Cambia color según estado
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, groundRadius); // Dibuja círculo de colisión
    }
}