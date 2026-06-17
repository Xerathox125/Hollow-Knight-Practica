using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb; // Referencia al cuerpo físico
    [HideInInspector] public Animator animPlayer; // Referencia al componente de animaciones
    [HideInInspector] public CapsuleCollider2D collPlayer; // Referencia al colisionador
    public Controles controles; // Objeto de control de InputSystem

    [Header("Variables Movimiento")]
    public float currentSpeed; // Velocidad de movimiento normal
    public float crouchSpeed; // Velocidad mientras se está agachado

    [Header("Variables de Salto")]
    public float normalGravity; // Gravedad normal del jugador
    public float fallGravity; // Gravedad aplicada al caer

    [HideInInspector] public UpdateAnimsPlayer updateAnimsPlayer; // Referencia al gestor de animaciones
    [HideInInspector] public PlayerMovement movement; // Referencia al script de movimiento
    [HideInInspector] public PlayerJump jump; // Referencia al script de salto
    [HideInInspector] public PlayerCrouch crouch; // Referencia al script de agacharse
    [HideInInspector] public PlayerDash dash; // Referencia al script de dash
    [HideInInspector] public PlayerStairs stairs; // Referencia al script de escaleras
    [HideInInspector] public PlayerSwim swim; // Referencia al script de escaleras

    [HideInInspector] public bool isJumpHeld = false; // Estado para saber si el salto sigue presionado

    [HideInInspector] public Vector2 moveInput; // Cache del input de movimiento para evitar lecturas constantes

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Inicializa el componente Rigidbody
        animPlayer = GetComponentInChildren<Animator>(); // Busca el animador en hijos
        collPlayer = GetComponent<CapsuleCollider2D>(); // Inicializa el colisionador
        controles = new Controles(); // Crea instancia del input

        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>(); // Obtiene script de animaciones
        movement = GetComponent<PlayerMovement>(); // Obtiene script de movimiento
        jump = GetComponent<PlayerJump>(); // Obtiene script de salto
        crouch = GetComponent<PlayerCrouch>(); // Obtiene script de agachado
        dash = GetComponent<PlayerDash>(); // Obtiene script de dash
        stairs = GetComponent<PlayerStairs>(); // Obtiene script de escaleras
        swim = GetComponent<PlayerSwim>();
    }

    void Update()
    {
        moveInput = controles.Player.Move.ReadValue<Vector2>(); // Lee el vector de movimiento una vez por frame

        movement.OnUpdate(); // Llama al update de movimiento
        jump.OnUpdate(); // Llama al update de salto
        crouch.OnUpdate(); // Llama al update de agacharse
        dash.OnUpdate(); // Llama al update de dash
        stairs.OnUpdate(); // Llama al update de escaleras
        swim.OnUpdate(); //Llama al update de nado
        updateAnimsPlayer.UpdateAnimation(); // Actualiza el estado visual
    }

    private void FixedUpdate()
    {
        movement.Move(); // Ejecuta física de movimiento
        dash.OnFixedUpdate(); // Ejecuta física de dash
        stairs.OnFixedUpdate(); // Ejecuta física de escaleras
    }

    private void OnEnable()
    {
        controles.Enable(); // Activa el mapa de controles
        controles.Player.Jump.performed += OnJump; // Suscribe evento de presionar salto
        controles.Player.Jump.canceled += OnJumpRelease; // Suscribe evento de soltar salto
        controles.Player.Dash.performed += OnDash; // Suscribe evento de presionar dash
    }

    private void OnDisable()
    {
        controles.Disable(); // Desactiva el mapa de controles
        controles.Player.Jump.performed -= OnJump; // Desuscribe salto
        controles.Player.Jump.canceled -= OnJumpRelease; // Desuscribe soltar salto
        controles.Player.Dash.performed -= OnDash; // Desuscribe dash
    }

    private void OnJump(InputAction.CallbackContext context) // Método llamado al saltar
    {
        isJumpHeld = true; // Activa flag de salto presionado
        jump.JumpHold(); // Llama a la lógica de inicio de salto
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

    private void OnTriggerStay2D(Collider2D collision) // Detecta permanencia en triggers
    {
        if (collision.CompareTag("Stairs")) stairs.rangeStairs = true; // Marca que está cerca de escalera
        if (collision.CompareTag("Water"))
        {
            swim.rangeSwim = true;
        }
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
            swim.rangeSwim = false;
            swim.ExitSwim();
        }
    }
}