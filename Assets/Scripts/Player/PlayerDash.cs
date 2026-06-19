using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Config")]
    public float dashForce; // Potencia del impulso
    public float dashDuration; // Duración del dash
    public float coolDownDash; // Tiempo de enfriamiento
    public bool isDash = false; // Indica si el jugador está en estado dash

    [Header("Dash Aéreo (Token)")]
    [SerializeField] private bool _hasAirDashed = false; // Flag privado para controlar el dash aéreo
    public bool hasAirDashed // Propiedad pública para acceder al estado aéreo
    {
        get => _hasAirDashed; // Retorna el valor
        set => _hasAirDashed = value; // Permite modificar el valor
    }

    [Header("Timers")]
    public float timerDashDuration = 0f; // Tiempo restante del dash actual
    public float timerCoolDownDash = 0f; // Tiempo restante para enfriamiento

    private Vector2 dashDirection; // Dirección en la que se aplicará el dash
    private PlayerController playerController; // Referencia al controlador

    private void Start() => playerController = GetComponent<PlayerController>(); // Cachea controlador

    public void OnUpdate() // Actualiza contadores y resets
    {
        if (timerCoolDownDash > 0f) timerCoolDownDash -= Time.deltaTime; // Reduce enfriamiento

        if (playerController.jump.IsGrounded && !isDash) // Si toca suelo y no está haciendo dash
        {
            hasAirDashed = false; // Reinicia el token de dash aéreo
        }
    }

    public void OnFixedUpdate() // Ejecuta la lógica física durante el dash
    {
        if (isDash) DashUpdate(); // Llama a la actualización si está activo
    }

    public void DashHold() // Lógica de inicio de dash
    {
        if (isDash || playerController.stairs.IsStairs) return; // Cancela si ya está haciendo dash o en escaleras

        bool canGroundDash = playerController.jump.IsGrounded && timerCoolDownDash <= 0f; // Comprueba si puede hacer dash en suelo
        bool canAirDash = !playerController.jump.IsGrounded && !_hasAirDashed; // Comprueba si puede hacer dash aéreo

        if (canGroundDash || canAirDash) // Si se cumplen condiciones
        {
            DashStart(); // Inicia proceso
        }
    }

    void DashStart() // Configura el estado inicial del dash
    {
        isDash = true; // Activa estado
        timerDashDuration = dashDuration; // Resetea duración
        playerController.rb.gravityScale = 0f; // Elimina gravedad temporalmente

        Vector2 move = playerController.moveInput; // Lee input actual

        if (move.magnitude > 0.1f) // Si hay dirección de movimiento
        {
            dashDirection = move.normalized; // Usa la dirección del input
        }
        else // Si está quieto
        {
            dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0f); // Usa dirección donde mira el jugador
        }

        if (playerController.jump.IsGrounded) timerCoolDownDash = coolDownDash; // Si está en suelo, activa enfriamiento
        else hasAirDashed = true; // Si está en aire, consume el token

        if (dashDirection.y > 0.1f) hasAirDashed = true; // Si dash es vertical, consume token
    }

    void DashUpdate() // Aplica movimiento constante mientras dura el dash
    {
        playerController.rb.linearVelocity = dashDirection * dashForce; // Aplica velocidad
        timerDashDuration -= Time.deltaTime; // Reduce tiempo restante

        if (timerDashDuration <= 0) DashEnd(); // Finaliza si el tiempo termina
    }

    void DashEnd() // Finaliza el estado de dash
    {
        isDash = false; // Desactiva estado
        playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad
        playerController.rb.linearVelocity = Vector2.zero; // Detiene movimiento residual
    }
}