using UnityEngine;

public class PlayerSwim : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador

    [Header("Nado")]
    public float gravitySwim; // Gravedad específica al estar en el agua
    public float speedSwim; // Velocidad de movimiento nadando
    public float salidaDelAguaSalto; // Fuerza de impulso al salir del agua saltando
    [HideInInspector] public bool rangeSwim; // Indica si está colisionando con el trigger de agua
    private bool isSwim; // Estado activo de nado

    [Header("Configuración Mario-Style")]
    public float gravityScaleSwim; // Gravedad más baja (flotante)
    public float jumpForceSwim;      // Fuerza fija del salto en agua

    public bool IsSwim => isSwim; // Getter público

    private void Start()
    {
        playerController = GetComponent<PlayerController>(); // Cache del controlador
    }

    public void OnUpdate()
    {
        DetectSwim(); // Revisa si debe entrar al estado de nado
        MoveSwim(); // Aplica físicas de nado
    }

    void DetectSwim() // Lógica para entrar al agua
    {
        if (rangeSwim && !isSwim) // Si toca el agua y no estaba nadando
        {
            StartSwim(); // Inicia nado
        }
    }

    void StartSwim() // Lógica inicial al tocar el agua
    {
        isSwim = true; // Activa estado
        playerController.rb.linearVelocity = Vector2.zero; // Al entrar al agua se resetean las velocidades
    }

    void MoveSwim()
    {
        if (isSwim)
        {
            // 1. Aplicamos gravedad reducida
            playerController.rb.gravityScale = gravityScaleSwim;

            // 2. Lógica de Movimiento
            Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

            if (move.magnitude > 0.1f)
            {
                // Movemos al personaje manteniendo su velocidad en Y (para no afectar la gravedad/salto)
                playerController.rb.linearVelocity = new Vector2(move.x * speedSwim, playerController.rb.linearVelocity.y);
            }
            else
            {
                // Si no hay input, detenemos solo la velocidad horizontal
                playerController.rb.linearVelocity = new Vector2(0, playerController.rb.linearVelocity.y);
            }

            // 3. Input de salto (predeterminado)
            if (playerController.controles.Player.Jump.triggered)
            {
                // Reseteamos velocidad Y para que el salto siempre sea igual de alto
                playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, 0);
                playerController.rb.AddForce(Vector2.up * jumpForceSwim, ForceMode2D.Impulse);
            }
        }
    }

    public void ExitSwim() // Lógica de salida del agua
    {
        isSwim = false; // Desactiva estado
        playerController.rb.gravityScale = playerController.normalGravity; // Devuelve la gravedad a la normalidad
        playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, salidaDelAguaSalto); // Aplica impulso de salida
    }
}