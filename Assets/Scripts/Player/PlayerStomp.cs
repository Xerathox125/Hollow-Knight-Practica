using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Stomp")]
    public float stompForce = 20f;     // Fuerza con la que cae hacia abajo el jugador
    public float impulseUpForce = 5f;  // Fuerza del mini impulso hacia arriba
    public float timeImpuseUp = 0.5f;  // Tiempo en que hace el mini impulso
    private float counterImpulseUp = 0; // Contador que verifica el tiempo del mini impulso
    private bool isImpulse;
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

        if (isImpulse)
        {
            playerController.rb.linearVelocity = Vector2.up * impulseUpForce;

            if (counterImpulseUp > timeImpuseUp)
            {
                isImpulse = false;
                counterImpulseUp = 0f;
            }
        }
        else
        {
            // Esto se ejecutará siempre que el impulso inicial haya terminado
            // y el Stomp siga activo.
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
