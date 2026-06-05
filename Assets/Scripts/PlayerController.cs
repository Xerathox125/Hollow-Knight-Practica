using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //COMPONENTES
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animPlayer;
    public Controles controles;


    [Header("Variables Movimiento")]    
    public float speed;

    [Header("Variables de Salto")]
    public float normalGravity = 2f;
    public float fallGravity = 4f;

    //MECÁNICAS
    [HideInInspector] public UpdateAnimsPlayer updateAnimsPlayer;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerJump jump;

    void Awake()
    {
        //COMPONENTES
        rb = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>();
        controles = new Controles();

        //CONECTAR MECÁNICAS
        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>();
        movement = GetComponent<PlayerMovement>();
        jump = GetComponentInChildren<PlayerJump>();
    }

    void FixedUpdate()
    {
        movement.Move(); //MOVIMIENDO
        jump.OnUpdate();
        //SALTO
        //AGACHARSE
    }

    private void Update()
    {
        updateAnimsPlayer.UpdateAnimation();
    }

    private void OnEnable()
    {
        controles.Enable();

        controles.Player.Jump.performed += OnJump;
        controles.Player.Jump.canceled += OnJumpRelease;
    }

   
    private void OnDisable()
    {
        controles.Player.Jump.performed -= OnJump;
        controles.Disable();
        controles.Player.Jump.canceled -= OnJumpRelease;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jump.JumpHold();
    }

    private void OnJumpRelease(InputAction.CallbackContext context)
    {
        jump.JumpRelease(); //posible cambio
    }


}
