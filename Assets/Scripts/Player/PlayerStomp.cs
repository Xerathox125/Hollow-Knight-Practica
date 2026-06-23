using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [Header("Stomp")]
    public float stompForce;     // Fuerza con la que cae hacia abajo el jugador
    public float impulseUpForce;  // Fuerza del mini impulso hacia arriba
    public float timeImpuseUp;  // Tiempo en que hace el mini impulso
    public float hangTime;      // Tiempo en que el jugador se queda suspendido en el aire (Efecto Mario)
    private float counterImpulseUp = 0; // Contador que verifica el tiempo del mini impulso o suspensión
    private bool isImpulse;     // Indica si está en la fase de impulso
    private bool isHanging;     // Indica si está en la fase de suspensión en el aire
    private bool isStomp = false; // Variable que indica si se está haciendo el stomp en general

    void Start()
    {
        playerController = GetComponent<PlayerController>(); // Cache del controlador
    }

    public void OnUpdate()
    {
        UpdateStomp(); // Actualiza la lógica de stomp cada frame
    }

    public void StartStomp() // Inicia la secuencia de Stomp
    {
        isStomp = true; // Activa estado general
        isImpulse = true; // Inicia con el impulso
        counterImpulseUp = 0; // Resetea el contador

        playerController.rb.linearVelocity = Vector2.zero; // Resetear velocidades
        playerController.controles.Player.Move.Disable();  // Deshabilitar controles movimiento
    }

    public void UpdateStomp() // OPTIMIZADO: Flujo corregido para transición de estados
    {
        if (!isStomp) return; // Si no está haciendo stomp, ignora el resto

        counterImpulseUp += Time.deltaTime; // OPTIMIZADO: Cambiado a deltaTime porque se llama en Update

        // 1. Fase de Impulso Inicial
        if (isImpulse)
        {
            playerController.rb.linearVelocity = Vector2.up * impulseUpForce; // Aplica fuerza hacia arriba

            if (counterImpulseUp > timeImpuseUp) // Si termina el tiempo de impulso
            {
                isImpulse = false; // Termina impulso
                isHanging = true; // Iniciamos la suspensión (Mario style)
                counterImpulseUp = 0f; // Resetea contador para la siguiente fase
            }
        }
        // 2. Fase de Suspensión (el "momento en el aire")
        else if (isHanging)
        {
            playerController.rb.gravityScale = 0; // Pausamos la gravedad para flotar
            playerController.rb.linearVelocity = Vector2.zero; // Mantenemos posición frenando en seco

            if (counterImpulseUp > hangTime) // Esperamos el tiempo definido de suspensión
            {
                isHanging = false; // Termina suspensión
                counterImpulseUp = 0f; // Resetea contador
                playerController.rb.gravityScale = playerController.normalGravity; // Restauramos gravedad normal
            }
        }
        // 3. Fase de Caída (Stomp fuerte)
        else
        {
            playerController.rb.linearVelocity = Vector2.down * stompForce; // Aplica fuerza hacia abajo constantemente
            if (playerController.jump.IsGrounded) EndStomp(); // Si toca el suelo termina la acción
        }
    }

    public void EndStomp() // Lógica de finalización
    {
        playerController.controles.Player.Move.Enable(); // Habilitar controles movimiento
        // Resetea estados
        isStomp = false; 
        isImpulse = false;
        isHanging = false;
        counterImpulseUp = 0f;
    }

    public void StompHold() // Validación para iniciar
    {
        if (!isStomp && !playerController.jump.IsGrounded && !playerController.stairs.IsStairs) StartStomp(); // Inicia si cumple requisitos
    }
}