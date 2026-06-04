using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //COMPONENTES
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animPlayer;
    public Controles controles;

    //VARIABLES
    public float speed;

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
    }    

    private void OnDisable()
    {
        controles.Player.Jump.performed -= OnJump;
        controles.Disable();    
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jump.OnUpdate();
    }

}
