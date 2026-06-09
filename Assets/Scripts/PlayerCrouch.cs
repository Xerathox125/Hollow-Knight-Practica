using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{

    private PlayerController playerController;
    private Vector2 originalCollOffset; //almacena el valor original del offset del collider del player
    private Vector2 originalCollSize; //almacena el valor original del size del collider del player

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        originalCollOffset = playerController.collPlayer.offset;
        originalCollSize = playerController.collPlayer.size;
    }

    public void OnUpdate()
    {
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>(); //con esto se sabe que tecla está presionando el jugador

        //Cmabiamos el tamaño del colisionador del personaje según si está agachado o no
        if (Mathf.RoundToInt(move.y) == -1) //se presionó la tecla hacia abajo ("S" o "flecha hacia abajo")
        {
            //Disminuir tamaño y el offset del collider
            playerController.collPlayer.offset = new Vector2(playerController.collPlayer.offset.x, -0.35f); //ajustar el offset del collider para que se mantenga en el suelo
            playerController.collPlayer.size = new Vector2(playerController.collPlayer.size.x, -0.80f); //ajustar el offset del collider para que se mantenga en el suelo
        }
        else
        {
            //Volver el tamaño y el offset del collider
            playerController.collPlayer.offset = originalCollOffset; //ajustar el offset del collider para que se mantenga en el suelo
            playerController.collPlayer.size = originalCollSize; //ajustar el offset del collider para que se mantenga en el suelo
        }
    }
  
}
