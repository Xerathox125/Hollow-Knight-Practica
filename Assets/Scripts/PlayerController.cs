using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //COMPONENTES
    [HideInInspector] public Rigidbody2D rb;
    public Animator animPlayer;
    public Controles controles;

    //VARIABLES
    public float speed;

    //MECÁNICAS
    [HideInInspector] public PlayerMovement playerMovement;

    void Awake()
    {
        //COMPONENTES
        rb = GetComponent<Rigidbody2D>();
        animPlayer = GetComponentInChildren<Animator>();
        controles = new Controles();

        //CONECTAR MECÁNICAS
        playerMovement = GetComponent<PlayerMovement>();      
    }

    void FixedUpdate()
    {
        playerMovement.Move(); //MOVIMIENDO
        //SALTO
        //AGACHARSE
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
