using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Stomp")]
    public float stompForce;     // Fuerza con la que cae hacia abajo el jugador
    public float impulseUpForce;  // Fuerza del mini impulso hacia arriba
    public float timeImpuseUp;  // Tiempo en que hace el mini impulso
    public float hangTime;
    private float counterImpulseUp = 0; // Contador que verifica el tiempo del mini impulso
    private bool isImpulse;
    private bool isHanging;
    private bool isStomp = false;       // Variable que indica si se está haciendo el stomp

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        UpdateStomp();
    }

    public void StartStomp()
    {
        isStomp = true;
        isImpulse = true;
        counterImpulseUp = 0;

        playerController.rb.linearVelocity = Vector2.zero; // Resetear velocidades
        playerController.controles.Player.Move.Disable();  // Deshabilitar controles movimiento
    }

    public void UpdateStomp()
    {
        if (!isStomp) return;

        counterImpulseUp += Time.fixedDeltaTime;

        // 1. Fase de Impulso Inicial
        if (isImpulse)
        {
            playerController.rb.linearVelocity = Vector2.up * impulseUpForce;

            if (counterImpulseUp > timeImpuseUp)
            {
                isImpulse = false;
                isHanging = true; // Iniciamos la suspensión
                counterImpulseUp = 0f;
                playerController.rb.linearVelocity = Vector2.zero; // Frenamos en seco
            }
        }
        // 2. Fase de Suspensión (el "momento en el aire")
        else if (isHanging)
        {
            playerController.rb.gravityScale = 0; // Pausamos la gravedad
            playerController.rb.linearVelocity = Vector2.zero; // Mantenemos posición

            if (counterImpulseUp > hangTime) // Esperamos el tiempo definido
            {
                isHanging = false;
                counterImpulseUp = 0f;
                playerController.rb.gravityScale = 1; // Restauramos gravedad si es necesario
            }
        }
        // 3. Fase de Caída (Stomp fuerte)
        else
        {
            playerController.rb.linearVelocity = Vector2.down * stompForce;

            if (playerController.jump.IsGrounded)
            {
                EndStomp();
            }
        }
    }

    public void EndStomp()
    {
        playerController.controles.Player.Move.Enable(); // Habilitar controles movimiento
        isStomp = false;
        isImpulse = false;
        counterImpulseUp = 0f;
    }

    public void StompHold()
    {
        if (!isStomp && !playerController.jump.IsGrounded && !playerController.stairs.IsStairs) //Condiciones para iniciar el Stomp
        {
            StartStomp();
        }
    }

}
