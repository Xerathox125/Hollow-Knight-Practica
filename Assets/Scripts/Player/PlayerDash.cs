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
    public float timerDashDuration = 0f; //cooldown o timer que verifica el dashDuration
    public float timerCoolDownDash = 0f; //Cooldown del Dash

    private Vector2 dashDirection;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        if (timerCoolDownDash > 0f)
        {
            timerCoolDownDash -= Time.deltaTime;
        }

        if (isDash)
        {
            DashUpdate();
        }

    }

    //Verificamos los timers y dirección del dash
    void DashStart()
    {
        isDash = true;
        timerDashDuration = dashDuration;
        timerCoolDownDash = coolDownDash;

        playerController.rb.gravityScale = 0f;

        // Verificar la dirección del dash
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        if (move.x > 0.5f && move.y > 0.5f) //input arriba derecha
        {
            dashDirection = new Vector2(1, 1).normalized;
        }
        else if (move.x < -0.5f && move.y > 0.5f) //input arriba izquierda
        {
            dashDirection = new Vector2(-1, 1).normalized;
        }
        else if (move.x > 0.5f && move.y < -0.5f) // CORREGIDO: input abajo derecha (tenías > -0.5f)
        {
            dashDirection = new Vector2(1, -1).normalized;
        }
        else if (move.x < -0.5f && move.y < -0.5f) //input abajo izquierda
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
        else if (move.x < -0.5f) // CORREGIDO: antes evaluabas move.y < -0.5f para ir a la izquierda
        {
            dashDirection = Vector2.left;
        }
        else
        {
            // ¡NUEVO! Si hace el dash sin presionar direcciones, va hacia donde mira el sprite
            float lookDirection = Mathf.Sign(transform.localScale.x);
            dashDirection = new Vector2(lookDirection, 0f);
        }
    }


    void DashUpdate()
    {
        //Aplicamos la velocidad del dash al RB del player
        playerController.rb.linearVelocity = dashDirection * dashForce;

        //Actualizamos timers
        timerDashDuration -= Time.deltaTime;

        if (timerDashDuration <= 0)
        {
            DashEnd(); //Terminar dash
        }

    }

    //Este método actualiza isDash y reinicia la grabedad después del Dash
    void DashEnd()
    {
        isDash = false;
        playerController.rb.gravityScale = playerController.normalGravity;
    }

    public void DashHold() //El método se llama cuando el juego detecta que se presiona la acción de Dash
    {
        if (!isDash && timerCoolDownDash <= 0)
            DashStart();
    }


}
