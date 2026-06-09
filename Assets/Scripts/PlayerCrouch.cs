using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{

    private PlayerController playerController;
    private Vector2 originalCollOffset; //almacena el valor original del offset del collider del player
    private Vector2 originalCollSize; //almacena el valor original del size del collider del player

    [Header("Crouch")]
    public float rayCheckOffset; //Distancia con el cual movemoos el punto desde donde se origina el raycast que detecta el techo sobre la cabeza del player 
    public float rayCheckDistance; //Detemina el tamaño del rayo
    public LayerMask headCollision;

    //Getters 
    public bool canStandUp
    {
        get { return CanStandUp(); }
    }

    public bool isCrouching { get; private set; }


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        originalCollOffset = playerController.collPlayer.offset;
        originalCollSize = playerController.collPlayer.size;
    }

    // Variable para saber si estamos agachados (se puede leer desde otros scripts, pero solo modificar aquí)

    public void OnUpdate()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // Si presionamos hacia abajo O NO podemos pararnos por el techo
        if (Mathf.RoundToInt(move.y) == -1 || !CanStandUp())
        {
            isCrouching = true; // Avisamos que estamos agachados

            // Disminuir tamaño y el offset del collider
            playerController.collPlayer.offset = new Vector2(playerController.collPlayer.offset.x, -0.35f);
            playerController.collPlayer.size = new Vector2(playerController.collPlayer.size.x, -0.80f);
        }
        else
        {
            isCrouching = false; // Avisamos que estamos de pie

            // Volver al tamaño y el offset original del collider
            playerController.collPlayer.offset = originalCollOffset;
            playerController.collPlayer.size = originalCollSize;
        }
    }

    //Método que determina si el personaje puede pparar se cuando está agachado pasando debajo de algo
    private bool CanStandUp(){
        Vector2 originPointRay = new Vector2(transform.position.x, transform.position.y) + Vector2.up * rayCheckOffset; //Punto desde donde se origina el raycast, se ajusta con el rayCheckOffset para que esté por encima de la cabeza del player
        RaycastHit2D hit = Physics2D.Raycast(originPointRay, Vector2.up, rayCheckDistance, headCollision);
        return hit.collider == null; //Si el raycast no colisiona con nada, entonces el player puede pararse
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 originPointRay = new Vector2(transform.position.x, transform.position.y) + Vector2.up * rayCheckOffset; //Punto desde donde se origina el raycast, se ajusta con el rayCheckOffset para que esté por encima de la cabeza del playerq
        Gizmos.DrawRay(originPointRay, Vector2.up * rayCheckDistance);
    }

}
