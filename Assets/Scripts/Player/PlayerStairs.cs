using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [Header("Escaleras")]
    public float stairsSpeed; // Velocidad de movimiento en escaleras
    public bool rangeStairs; // Indica si está en rango de colisión
    private bool isStairs; // Estado activo de uso de escaleras

    private float cooldownTimer = 0f; // Tiempo de espera para volver a subir

    public bool IsStairs => isStairs; // Propiedad pública de estado

    private void Start() => playerController = GetComponent<PlayerController>(); // Inicializa referencia

    public void OnUpdate() // Lógica de actualización de escaleras
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime; // Reduce cooldown
        DetectStairs(); // Comprueba si debe entrar a escaleras
    }

    public void OnFixedUpdate() => MoveStairs(); // Ejecuta movimiento físico

    void DetectStairs() // Lógica para entrar a las escaleras
    {
        if (rangeStairs && !isStairs && cooldownTimer <= 0f && playerController.moveInput.y > 0.5f) // Condición de entrada
        {
            StartStairs(); // Inicia estado
        }
    }

    void StartStairs() // Configura estado de escaleras
    {
        isStairs = true; // Activa modo escaleras
        playerController.rb.gravityScale = 0f; // Elimina gravedad
        playerController.rb.linearVelocity = Vector2.zero; // Detiene inercia
    }

    void MoveStairs() // Lógica de movimiento vectorial
    {
        if (!isStairs) return; // Sale si no está en escaleras

        playerController.rb.gravityScale = 0f; // Mantiene gravedad cero
        Vector2 move = playerController.moveInput; // Input actual
        playerController.rb.linearVelocity = move * stairsSpeed; // Aplica velocidad de movimiento
    }

    public void ExitStairs(float cooldown = 0f) // Salida del modo escaleras
    {
        isStairs = false; // Desactiva estado
        cooldownTimer = cooldown; // Aplica tiempo de espera si es necesario
        playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad normal
    }
}