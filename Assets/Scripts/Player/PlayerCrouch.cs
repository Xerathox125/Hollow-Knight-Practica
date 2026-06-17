using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador
    private Vector2 originalCollOffset; // Offset original del colisionador
    private Vector2 originalCollSize; // Tamaño original del colisionador

    [Header("Crouch")]
    public float rayCheckOffset; // Desplazamiento del rayo de verificación
    public float rayCheckDistance; // Distancia para comprobar techo
    public LayerMask headCollision; // Capas que bloquean levantarse

    public bool canStandUp => CanStandUp(); // Propiedad para verificar si puede levantarse
    public bool isCrouching { get; private set; } // Estado de agachado

    private void Start() // Inicializa parámetros al comenzar
    {
        playerController = GetComponent<PlayerController>(); // Cache del controlador
        originalCollOffset = playerController.collPlayer.offset; // Guarda offset original
        originalCollSize = playerController.collPlayer.size; // Guarda tamaño original
    }

    public void OnUpdate() // Actualiza lógica de agacharse por frame
    {
        if (Mathf.RoundToInt(playerController.moveInput.y) == -1 || !CanStandUp()) // Verifica input o techo
        {
            isCrouching = true; // Marca como agachado
            playerController.collPlayer.offset = new Vector2(playerController.collPlayer.offset.x, -0.35f); // Ajusta offset para colisionador pequeño
            playerController.collPlayer.size = new Vector2(playerController.collPlayer.size.x, 0.80f); // Ajusta tamaño para colisionador pequeño
        }
        else // Si no hay necesidad de agacharse
        {
            isCrouching = false; // Marca como parado
            playerController.collPlayer.offset = originalCollOffset; // Restaura offset original
            playerController.collPlayer.size = originalCollSize; // Restaura tamaño original
        }
    }

    private bool CanStandUp() // Verifica si hay espacio sobre la cabeza
    {
        Vector2 originPointRay = (Vector2)transform.position + Vector2.up * rayCheckOffset; // Define origen del rayo
        return Physics2D.Raycast(originPointRay, Vector2.up, rayCheckDistance, headCollision).collider == null; // Retorna true si no hay nada arriba
    }

    private void OnDrawGizmos() // Visualización del rayo de comprobación
    {
        Gizmos.color = Color.green; // Color del gizmo
        Vector2 originPointRay = (Vector2)transform.position + Vector2.up * rayCheckOffset; // Origen del rayo
        Gizmos.DrawRay(originPointRay, Vector2.up * rayCheckDistance); // Dibuja línea de debug
    }
}