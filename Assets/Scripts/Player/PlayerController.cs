using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.LookDev;

public class PlayerController : MonoBehaviour
{
    // COMPONENTES
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animPlayer;
    [HideInInspector] public CapsuleCollider2D collPlayer;
    public Controles controles;

    [Header("Variables Movimiento")]
    public float speed;
    public float crouchSpeed;

    [Header("Variables de Salto")]
    public float normalGravity = 2f;
    public float fallGravity = 4f;

    // MECÁNICAS
    [HideInInspector] public UpdateAnimsPlayer updateAnimsPlayer;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerJump jump;
    [HideInInspector] public PlayerCrouch crouch;
    [HideInInspector] public PlayerDash dash;
    [HideInInspector] public PlayerStairs stairs;

    [HideInInspector] public bool isJumpHeld = false;

    void Awake()
    {   //Componentes
        rb = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>();
        collPlayer = GetComponent<CapsuleCollider2D>();
        controles = new Controles();

        //Conectar mecánicas
        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>();
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        crouch = GetComponent<PlayerCrouch>();
        dash = GetComponent<PlayerDash>();
        stairs = GetComponent<PlayerStairs>();
    }

    void FixedUpdate()
    {
        if(!stairs.isStairs)
            movement.Move();
        stairs.OnFixedUpdate();
    }

    private void Update()
    {

        jump.OnUpdate(); //saltar
        crouch.OnUpdate(); //agacharse
        dash.OnUpdate(); //dash
        stairs.OnUpdate();
        updateAnimsPlayer.UpdateAnimation();        
    }

    private void OnEnable()
    {
        controles.Enable();
        controles.Player.Jump.performed += OnJump;
        controles.Player.Jump.canceled += OnJumpRelease;
        controles.Player.Dash.performed += OnDash;
    }  

    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Jump.performed -= OnJump;
        controles.Player.Jump.canceled -= OnJumpRelease;
        controles.Player.Dash.performed -= OnDash;
    }

    //Métodos Inputs
    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpHeld = true;
        jump.JumpHold();
    }

    private void OnJumpRelease(InputAction.CallbackContext context)
    {
        isJumpHeld = false;
        jump.JumpRelease();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if(stairs.isStairs) // Evitamos que el jugador pueda hacer dash mientras está en las escaleras
            return; 

        dash.DashHold();        
    }

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Stairs"))
        {
            stairs.rangeStairs = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairs"))
        {
            stairs.rangeStairs = false;
            stairs.ExitStairs();
        }
    }

}