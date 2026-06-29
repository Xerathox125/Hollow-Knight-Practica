using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb; // Referencia al cuerpo físico
    [HideInInspector] public Animator animPlayer; // Referencia al componente de animaciones
    [HideInInspector] public CapsuleCollider2D collPlayer; // Referencia al colisionador
    [HideInInspector] public Vector2 moveInput; // Cache del input de movimiento para evitar lecturas constantes
    [HideInInspector] public bool isJumpHeld = false; // Estado para saber si el salto sigue presionado
    public Controles controles; // Objeto de control de InputSystem

    [Header("Variables Movimiento")]
    public float currentSpeed; // Velocidad de movimiento normal
    public float crouchSpeed; // Velocidad mientras se está agachado

    [Header("Variables de Salto")]
    public float normalGravity; // Gravedad normal del jugador
    public float fallGravity; // Gravedad aplicada al caer

    //Referencias a scripts
    [HideInInspector] public UpdateAnimsPlayer updateAnimsPlayer;
    [HideInInspector] public PlayerMovement    movement;
    [HideInInspector] public PlayerJump        jump;
    [HideInInspector] public PlayerCrouch      crouch;
    [HideInInspector] public PlayerDash        dash;
    [HideInInspector] public PlayerStairs      stairs;
    [HideInInspector] public PlayerSwim        swim;
    [HideInInspector] public PlayerStomp       stomp;
    [HideInInspector] public PlayerWallJump    wallJump;
    [HideInInspector] public PlayerAttacks     attacks;


    void Awake()
    {
        //Inicializador
        rb         = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>(); // Busca el animador en hijos
        collPlayer = GetComponent<CapsuleCollider2D>();
        controles  = new Controles(); // Crea instancia del input

        //Obtiene scripts 
        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>(); 
        movement          = GetComponent<PlayerMovement>();             
        jump              = GetComponent<PlayerJump>();                     
        crouch            = GetComponent<PlayerCrouch>();                 
        dash              = GetComponent<PlayerDash>();                     
        stairs            = GetComponent<PlayerStairs>();                 
        swim              = GetComponent<PlayerSwim>();                     
        stomp             = GetComponent<PlayerStomp>();                   
        wallJump          = GetComponent<PlayerWallJump>();             
        attacks           = GetComponent<PlayerAttacks>();
    }

    void Update()
    {
        moveInput = controles.Player.Move.ReadValue<Vector2>(); // Lee el vector de movimiento una vez por frame

        //Llamada a los updates de cada script
        updateAnimsPlayer.UpdateAnimation(); // Actualiza el estado visual
        movement.OnUpdate();
        crouch.OnUpdate();
        dash.OnUpdate();
        stairs.OnUpdate();
        swim.OnUpdate();        
        wallJump.OnUpdate();
        attacks.OnUpdate();
    }

    private void FixedUpdate()
    {
        //Ejecución de físicas
        stomp.OnUpdate();
        jump.OnUpdate();
        movement.Move();
        dash.OnFixedUpdate();
        stairs.OnFixedUpdate();
    }

    private void OnEnable()
    {
        controles.Enable(); // Activa el mapa de controles

        // Suscripción de eventos
        controles.Player.Jump.performed  += OnJump;        // Presionar salto
        controles.Player.Jump.canceled   += OnJumpRelease; // Soltar salto
        controles.Player.Dash.performed  += OnDash;        // Presionar dash
        controles.Player.Stomp.performed += OnStomp;       // Presionar stomp
        controles.Player.Attack.performed += OnAttack;       // Presionar ataque
    }

    

    private void OnDisable()
    {
        controles.Disable(); // Desactiva el mapa de controles

        // Descucripción de eventos
        controles.Player.Jump.performed  -= OnJump;        // Presionar salto
        controles.Player.Jump.canceled   -= OnJumpRelease; // Soltar salto
        controles.Player.Dash.performed  -= OnDash;        // Presionar dash
        controles.Player.Stomp.performed -= OnStomp;       // Presionar stomp
        controles.Player.Attack.performed -= OnAttack;       // Presionar ataque
    }

    // Métodos inputs
    private void OnJump(InputAction.CallbackContext context) // Método llamado al saltar
    {
        isJumpHeld = true; // Activa flag de salto presionado
        jump.JumpHold();   // Llama a la lógica de inicio de salto
    }

    private void OnJumpRelease(InputAction.CallbackContext context) // Método llamado al soltar salto
    {
        isJumpHeld = false; // Desactiva flag de salto
        jump.JumpRelease(); // Llama a la lógica de fin de salto
    }

    private void OnDash(InputAction.CallbackContext context) // Método llamado al hacer dash
    {
        if (!stairs.IsStairs) dash.DashHold(); // Ejecuta dash si no está en escaleras
    }

    private void OnStomp(InputAction.CallbackContext context) // Método llamado al hacer stomp
    {
        stomp.StompHold(); // Llama a la lógica de validación de stomp
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        attacks.AttackHold();
    }



    private void OnTriggerStay2D(Collider2D collision) // Detecta permanencia en triggers
    {
        if (collision.CompareTag("Stairs")) stairs.rangeStairs = true; // Marca que está cerca de escalera
        if (collision.CompareTag("Water"))  swim.rangeSwim     = true; // Marca que está dentro de zona de agua        
    }

    private void OnTriggerExit2D(Collider2D collision) // Detecta salida de triggers
    {
        if (collision.CompareTag("Stairs"))
        {
            stairs.rangeStairs = false; // Desmarca proximidad a escalera
            if (stairs.IsStairs) stairs.ExitStairs(); // Sale de escaleras si estaba en ellas
        }

        if (collision.CompareTag("Water"))
        {
            swim.rangeSwim = false; // Desmarca proximidad de agua
            swim.ExitSwim(); // Ejecuta la salida del agua
        }
    }
}