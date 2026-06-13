using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Escaleras")]
    public float stairsSpeed; //Velocidad de movimiento mientras escalamos
    public bool rangeStairs; //Detecta si estamos en el rango de la escalera para escalarla
    private bool isStairs;  //Determina si estamos en una escalera o no

    private float cooldownTimer = 0f; // NUEVO: Temporizador para evitar reenganches inmediatos al saltar

    //Getters
    public bool IsStairs { get => isStairs; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        // Reducimos el cooldown frame a frame si está activo
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        DetectStairs();
    }

    public void OnFixedUpdate()
    {
        MoveStairs();
    }

    //Detecta si podemos iniciar la mecánica de escalado
    void DetectStairs()
    {
        // MODIFICACIÓN: Agregamos la condición de que el cooldown haya terminado (cooldownTimer <= 0f)
        if (rangeStairs && !isStairs && cooldownTimer <= 0f)
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

    // MODIFICACIÓN: Añadimos un parámetro opcional 'cooldown' que por defecto es 0
    public void ExitStairs(float cooldown = 0f)
    {
        isStairs = false;
        cooldownTimer = cooldown; // Asignamos el tiempo de espera para no volver a engancharnos inmediatamente
        playerController.rb.gravityScale = playerController.normalGravity;
    }
}