using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash")]
    public float dashForce; //Variable que controla la fuerza del Dash
    public float dashDuration; //duración del dash
    public float coolDownDash; //Cooldown del Dash
    public bool isDash = false;

    //Timers
    private float timerDashDuration = 0f; //cooldown o timer que verifica el dashDuration
    private float timerCoolDownDash = 0f; //Cooldown del Dash

    private Vector2 dashDirection;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    //Verificamos los timers y dirección del dash
    void DashStart()
    {
        isDash = true;
        timerDashDuration = dashDuration;
        timerCoolDownDash = coolDownDash;

        playerController.rb.gravityScale = 0f;

        //Verificar la dirección del dash
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();
        if (move.x > 0.5f && move.y > 0.5f) //input arriba derecha
        {
            dashDirection = new Vector2(1, 1).normalized;
        }
        else if (move.x < -0.5f && move.y > 0.5f) //input arriba izquierda
        {
            dashDirection = new Vector2(-1, 1).normalized;
        }
        else if (move.x > -0.5f && move.y < -0.5f) //input abajo derecha
        {
            dashDirection = new Vector2(1, -1).normalized;
        }
        else if (move.x < -0.5f && move.y < -0.5f) //input abajo derecha
        {
            dashDirection = new Vector2(-1, -1).normalized;
        }
        else if (move.y > 0.5f) //input arriba
        {
            dashDirection = Vector2.up;
        }
        else if (move.y < -0.5f) //input abajo
        {
            dashDirection = Vector2.down;
        }
        else if (move.x > 0.5f) //input derecha
        {
            dashDirection = Vector2.right;
        }
        else if (move.y < -0.5f) //input izquierda
        {
            dashDirection = Vector2.left;
        }

        //Verificar si hace el dash sin presionar teclas de movimiento
    }


}
