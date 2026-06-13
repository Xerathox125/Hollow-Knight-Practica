using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Escaleras")]
    public float stairsSpeed; //Velocidad de movimiento mientras escalamos
    public bool rangeStairs; //Deecta si estamos en el rango de la escalera para escalarla
    private bool isStairs;  //Determina si estamos en una escalera o no

    //Getters
    public bool IsStairs { get => isStairs; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        DetectStairs();
    }

    public void OnFixedUpdate()
    {
        MoveStairs();
    }



    //Detecta si podemos iniciar la mecánica de escalado
    void DetectStairs()
    {
        if (rangeStairs && !isStairs)
        {
            Vector2 upArrow = playerController.controles.Player.Move.ReadValue<Vector2>();
            if (upArrow.y > 0.5f) //Verificar si se presiona la tecla hacia arriba
            {
                StartStairs();
            }
        }
    }

    //Inicia la mecánica de escalado
    void StartStairs()
    {
        isStairs = true;
        playerController.rb.gravityScale = 0f; //Si estamos escalando, gravedad = 0
        playerController.rb.linearVelocity = Vector2.zero; //Si estamos escalando, las velocidades se reinician
    }

    //Determina el movimiento del player cuando estamos en la escalera
    void MoveStairs()
    {
        if (isStairs)
        {
            //Nos aseguramos de que la gravedad sea 0 cuando isStairs = true
            playerController.rb.gravityScale = 0f;
            Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>(); //Vector donde se almacena el input del jugador que se recibe desde PlayerController
            playerController.rb.linearVelocity = new Vector2(move.x * stairsSpeed, move.y * stairsSpeed); //Aplicamos la velocidad en X y Y al rigidbody del PlayerController

        }
    }

    //Este método reinicia la gravedad del player y actualiza el bool isStairs a false
    public void ExitStairs()
    {
        isStairs = false;
        playerController.rb.gravityScale = playerController.normalGravity;
    }
}
