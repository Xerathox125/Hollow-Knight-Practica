using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController; // Referencia al controlador
    private bool isFacingRight = true; // Dirección actual del sprite
    private bool isMoving; // Estado de movimiento

    public bool IsMoving => isMoving; // Propiedad pública para acceder al estado

    private void Awake() => playerController = GetComponent<PlayerController>(); // Cache del controlador

    public void OnUpdate() // Actualiza lógica de movimiento
    {
        isMoving = playerController.moveInput.x != 0; // Verifica si hay input horizontal
    }

    public void Move() // Aplica la física de movimiento
    {
        if (playerController.dash?.isDash == true) return; // Cancela movimiento si está haciendo dash

        Vector2 move = playerController.moveInput; // Obtiene input del controlador
        float currentSpeed = playerController.crouch.isCrouching ? playerController.crouchSpeed : playerController.speed; // Elige velocidad según estado
        playerController.rb.linearVelocity = new Vector2(move.x * currentSpeed, playerController.rb.linearVelocity.y); // Aplica velocidad física

        if (move.x > 0 && !isFacingRight) Flip(); // Voltea derecha
        else if (move.x < 0 && isFacingRight) Flip(); // Voltea izquierda
    }

    private void Flip() // Cambia la escala para voltear al jugador
    {
        isFacingRight = !isFacingRight; // Invierte el estado
        transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f); // Aplica cambio visual
    }
}