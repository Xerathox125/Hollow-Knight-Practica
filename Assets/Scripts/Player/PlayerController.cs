using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animPlayer;
    [HideInInspector] public CapsuleCollider2D collPlayer;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool isJumpHeld = false;
    public Controles controles;

    [Header("Variables Movimiento")]
    public float currentSpeed;
    public float crouchSpeed;

    [Header("Variables de Salto")]
    public float normalGravity;
    public float fallGravity;

    //Referencias a scripts
    [HideInInspector] public UpdateAnimsPlayer updateAnimsPlayer;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerJump jump;
    [HideInInspector] public PlayerCrouch crouch;
    [HideInInspector] public PlayerDash dash;
    [HideInInspector] public PlayerStairs stairs;
    [HideInInspector] public PlayerSwim swim;
    [HideInInspector] public PlayerStomp stomp;
    [HideInInspector] public PlayerWallJump wallJump;
    [HideInInspector] public PlayerAttacks attacks;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>();
        collPlayer = GetComponent<CapsuleCollider2D>();
        controles = new Controles();

        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>();
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        crouch = GetComponent<PlayerCrouch>();
        dash = GetComponent<PlayerDash>();
        stairs = GetComponent<PlayerStairs>();
        swim = GetComponent<PlayerSwim>();
        stomp = GetComponent<PlayerStomp>();
        wallJump = GetComponent<PlayerWallJump>();
        attacks = GetComponent<PlayerAttacks>();
    }

    void Update()
    {
        moveInput = controles.Player.Move.ReadValue<Vector2>();

        updateAnimsPlayer.UpdateAnimation();
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
        stomp.OnUpdate();
        jump.OnUpdate();
        movement.Move();
        dash.OnFixedUpdate();
        stairs.OnFixedUpdate();
    }

    private void OnEnable()
    {
        controles.Enable();
        controles.Player.Jump.performed += OnJump;
        controles.Player.Jump.canceled += OnJumpRelease;
        controles.Player.Dash.performed += OnDash;
        controles.Player.Stomp.performed += OnStomp;
        controles.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Jump.performed -= OnJump;
        controles.Player.Jump.canceled -= OnJumpRelease;
        controles.Player.Dash.performed -= OnDash;
        controles.Player.Stomp.performed -= OnStomp;
        controles.Player.Attack.performed -= OnAttack;
    }

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
        if (!stairs.IsStairs) dash.DashHold();
    }

    private void OnStomp(InputAction.CallbackContext context)
    {
        stomp.StompHold();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        attacks.AttackHold();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairs")) stairs.rangeStairs = true;
        if (collision.CompareTag("Water")) swim.rangeSwim = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairs"))
        {
            stairs.rangeStairs = false;
            if (stairs.IsStairs) stairs.ExitStairs();
        }
        if (collision.CompareTag("Water"))
        {
            swim.rangeSwim = false;
            swim.ExitSwim();
        }
    }
}