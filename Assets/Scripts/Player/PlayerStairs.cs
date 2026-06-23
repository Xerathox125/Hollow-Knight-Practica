using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [HideInInspector] public bool rangeStairs; // Indica si está en rango de colisión de escalera

    [Header("Escaleras")]
    public float stairsSpeed; // Velocidad de movimiento en escaleras
    private bool isStairs; // Estado activo de uso de escaleras
    private float cooldownTimer = 0f; // Tiempo de espera para volver a subir tras salir

    //Getter
    public bool IsStairs => isStairs;

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); // Inicializa referencia
    } 

    public void OnUpdate() // Lógica de actualización de escaleras
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime; // Reduce cooldown de reingreso
        DetectStairs(); // Comprueba si debe entrar a escaleras
    }

    public void OnFixedUpdate()
    {
        MoveStairs(); // Ejecuta movimiento físico
    }

    void DetectStairs() // Lógica para entrar a las escaleras
    {
        if (rangeStairs && !isStairs && cooldownTimer <= 0f && playerController.moveInput.y > 0.5f) // Condición de entrada (input hacia arriba)
        {
            StartStairs(); // Inicia estado
        }
    }

    void StartStairs() // Configura estado de escaleras
    {
        isStairs = true; // Activa modo escaleras
        playerController.rb.gravityScale = 0f; // Elimina gravedad para no caer de la escalera
        playerController.rb.linearVelocity = Vector2.zero; // Detiene inercia previa
    }

    void MoveStairs() // Lógica de movimiento vectorial
    {
        if (!isStairs) return; // Sale si no está en escaleras

        playerController.rb.gravityScale = 0f; // Asegura mantener gravedad cero
        Vector2 move = playerController.moveInput; // Input actual (X e Y)
        playerController.rb.linearVelocity = move * stairsSpeed; // Aplica velocidad de movimiento libre en la escalera
    }

    public void ExitStairs(float cooldown = 0f) // Salida del modo escaleras
    {
        isStairs = false; // Desactiva estado
        cooldownTimer = cooldown; // Aplica tiempo de espera para evitar reenganche automático
        playerController.rb.gravityScale = playerController.normalGravity; // Restaura gravedad normal
    }
}