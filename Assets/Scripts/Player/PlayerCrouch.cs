using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private PlayerController playerController;
    private Vector2 originalCollOffset;
    private Vector2 originalCollSize;

    [Header("Crouch")]
    public float rayCheckOffset;
    public float rayCheckDistance;
    public LayerMask headCollision;

    public bool canStandUp => CanStandUp();
    public bool isCrouching { get; private set; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        originalCollOffset = playerController.collPlayer.offset;
        originalCollSize = playerController.collPlayer.size;
    }

    public void OnUpdate()
    {
        // Si presionamos hacia abajo O NO podemos pararnos por el techo
        if (Mathf.RoundToInt(playerController.moveInput.y) == -1 || !CanStandUp())
        {
            isCrouching = true;
            playerController.collPlayer.offset = new Vector2(playerController.collPlayer.offset.x, -0.35f);
            playerController.collPlayer.size = new Vector2(playerController.collPlayer.size.x, 0.80f); // Se quitó el valor negativo de altura
        }
        else
        {
            isCrouching = false;
            playerController.collPlayer.offset = originalCollOffset;
            playerController.collPlayer.size = originalCollSize;
        }
    }

    private bool CanStandUp()
    {
        Vector2 originPointRay = (Vector2)transform.position + Vector2.up * rayCheckOffset;
        return Physics2D.Raycast(originPointRay, Vector2.up, rayCheckDistance, headCollision).collider == null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 originPointRay = (Vector2)transform.position + Vector2.up * rayCheckOffset;
        Gizmos.DrawRay(originPointRay, Vector2.up * rayCheckDistance);
    }
}