using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float vel;
    private Controles controles;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controles = new Controles();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        controles.Enable();
    }

    private void OnDisable()
    {
        controles.Disable();    
    }


    private void Move()
    {
        Vector2 move = controles.Player.Move.ReadValue<Vector2>();

        rb.linearVelocity = new Vector2(move.x * vel, rb.linearVelocity.y);
    }
}
