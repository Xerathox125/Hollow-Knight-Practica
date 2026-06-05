using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private bool isFacingRight = true;
    private bool isMoving;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Move() //Controla el movimiento del player
    {
        //Vector donde se almacena el input del jugador que se recibe desde el PlayerController
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        //Aplicamos la velocidad en X al rigidbody del player controller
        playerController.rb.linearVelocity = new Vector2(move.x * playerController.speed, playerController.rb.linearVelocity.y);

        // Dentro del bool isMoving guardamos si el player se mueve o no
        isMoving = move.x != 0;

        if (move.x > 0 && !isFacingRight)        
            Flip();        
        else if (move.x < 0 && isFacingRight)
            Flip();        
    }


    private void Flip() //Controla el cambio de vista del player según la dirección del movimiento
    {
        isFacingRight = !isFacingRight;

        if (isFacingRight)
        {
            transform.localScale = new Vector3(1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1);
        }
    }

    public bool IsMoving //Getter de la variable isMoving para saber si nos estamos moviendo
    {
        get{ return isMoving; }
    }
}
