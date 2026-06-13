using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Config")]
    public float dashForce;
    public float dashDuration;
    public float coolDownDash;
    public bool isDash = false;

    [Header("Dash Aéreo (Token)")]
    [SerializeField] private bool _hasAirDashed = false;
    public bool hasAirDashed
    {
        get => _hasAirDashed;
        set => _hasAirDashed = value;
    }

    [Header("Timers")]
    public float timerDashDuration = 0f;
    public float timerCoolDownDash = 0f;

    private Vector2 dashDirection;
    private PlayerController playerController;

    private void Start()
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

        // 2. Recargar el token aéreo (Solo si está en el suelo y NO está en medio de un dash)
        if (playerController.jump.IsGrounded && !isDash)
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

        // Quitamos la gravedad para evitar parábolas extrañas durante el trayecto
        playerController.rb.gravityScale = 0f;

        // 1. Verificar la dirección elegida mediante el input
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

        // 2. Evaluar si gasta el token del aire o aplica cooldown del suelo
        if (playerController.jump.IsGrounded)
        {
            timerCoolDownDash = coolDownDash;
        }
        else
        {
            hasAirDashed = true;
        }

        // 3. Regala de seguridad: Si el dash va hacia arriba, el token se consume
        if (dashDirection.y > 0.1f)
        {
            hasAirDashed = true;
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

        // Frenado seco en ambos ejes al terminar
        playerController.rb.linearVelocity = Vector2.zero;
    }

    public void DashHold()
    {
        if (!isDash && !playerController.stairs.IsStairs)
        {
            bool canGroundDash = playerController.jump.IsGrounded && timerCoolDownDash <= 0;
            bool canAirDash = !playerController.jump.IsGrounded && !hasAirDashed;

            if (canGroundDash || canAirDash)
            {
                DashStart();
            }
        }
    }
}