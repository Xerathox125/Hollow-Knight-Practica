using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash")]
    public float dashForce;
    public float dashDuration;
    public float coolDownDash;
    public bool isDash = false;

    [Header("Dash Aéreo (Token)")]
    public bool hasAirDashed = false; // Esta es la variable que actúa como "cargo" o "token"

    // Timers
    public float timerDashDuration = 0f;
    public float timerCoolDownDash = 0f;

    private Vector2 dashDirection;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        // 1. Manejo del cooldown (solo es relevante en el suelo)
        if (timerCoolDownDash > 0f)
        {
            timerCoolDownDash -= Time.deltaTime;
        }

        // 2. RECARGAR EL TOKEN AÉREO
        // Si tocamos el suelo, recuperamos la habilidad de hacer Dash en el aire
        if (playerController.jump.IsGrounded)
        {
            hasAirDashed = false;
        }

        if (isDash)
        {
            DashUpdate();
        }
    }

    void DashStart()
    {
        isDash = true;
        timerDashDuration = dashDuration;

        // Quitamos la gravedad para que no haga una parábola rara
        playerController.rb.gravityScale = 0f;

        // 3. GASTAR EL TOKEN O APLICAR COOLDOWN
        if (playerController.jump.IsGrounded)
        {
            // Si estamos en el suelo, aplicamos el cooldown de tiempo
            timerCoolDownDash = coolDownDash;
        }
        else
        {
            // Si estamos en el aire, gastamos el token
            hasAirDashed = true;
        }

        // Verificar la dirección del dash
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        if (move.x > 0.5f && move.y > 0.5f) dashDirection = new Vector2(1, 1).normalized;
        else if (move.x < -0.5f && move.y > 0.5f) dashDirection = new Vector2(-1, 1).normalized;
        else if (move.x > 0.5f && move.y < -0.5f) dashDirection = new Vector2(1, -1).normalized;
        else if (move.x < -0.5f && move.y < -0.5f) dashDirection = new Vector2(-1, -1).normalized;
        else if (move.y > 0.5f) dashDirection = Vector2.up;
        else if (move.y < -0.5f) dashDirection = Vector2.down;
        else if (move.x > 0.5f) dashDirection = Vector2.right;
        else if (move.x < -0.5f) dashDirection = Vector2.left;
        else
        {
            float lookDirection = Mathf.Sign(transform.localScale.x);
            dashDirection = new Vector2(lookDirection, 0f);
        }
    }

    void DashUpdate()
    {
        playerController.rb.linearVelocity = dashDirection * dashForce;

        timerDashDuration -= Time.deltaTime;

        if (timerDashDuration <= 0)
        {
            DashEnd();
        }
    }

    void DashEnd()
    {
        isDash = false;
        playerController.rb.gravityScale = playerController.normalGravity;

        // Frenado seco en ambos ejes (para que no salgas volando)
        playerController.rb.linearVelocity = Vector2.zero;
    }

    public void DashHold()
    {
        // 4. NUEVA LÓGICA DE VALIDACIÓN
        if (!isDash)
        {
            // Condición para dash en el suelo
            bool canGroundDash = playerController.jump.IsGrounded && timerCoolDownDash <= 0;

            // Condición para dash en el aire
            bool canAirDash = !playerController.jump.IsGrounded && !hasAirDashed;

            // Si cumplimos alguna de las dos, ejecutamos el Dash
            if (canGroundDash || canAirDash)
            {
                DashStart();
            }
        }
    }
}