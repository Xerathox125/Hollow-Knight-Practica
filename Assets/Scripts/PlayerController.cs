using UnityEngine;

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

    void Awake()
    {
        //COMPONENTES
        rb = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>();
        controles = new Controles();

        //CONECTAR MECÁNICAS
        updateAnimsPlayer = GetComponent<UpdateAnimsPlayer>();
        movement = GetComponent<PlayerMovement>();      
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
    }

    private void OnDisable()
    {
        controles.Disable();    
    }

    
}
