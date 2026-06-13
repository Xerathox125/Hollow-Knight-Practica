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

    private void Start() => playerController = GetComponent<PlayerController>();

    public void OnUpdate()
    {
        if (timerCoolDownDash > 0f) timerCoolDownDash -= Time.deltaTime;

        if (playerController.jump.IsGrounded && !isDash)
        {
            hasAirDashed = false;
        }
    }

    public void OnFixedUpdate()
    {
        if (isDash) DashUpdate();
    }

    public void DashHold()
    {
        if (isDash || playerController.stairs.IsStairs) return;

        bool canGroundDash = playerController.jump.IsGrounded && timerCoolDownDash <= 0f;
        bool canAirDash = !playerController.jump.IsGrounded && !_hasAirDashed;

        if (canGroundDash || canAirDash)
        {
            DashStart();
        }
    }

    void DashStart()
    {
        isDash = true;
        timerDashDuration = dashDuration;
        playerController.rb.gravityScale = 0f;

        Vector2 move = playerController.moveInput; // Input optimizado

        if (move.magnitude > 0.1f)
        {
            dashDirection = move.normalized;
        }
        else
        {
            dashDirection = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        }

        if (playerController.jump.IsGrounded) timerCoolDownDash = coolDownDash;
        else hasAirDashed = true;

        if (dashDirection.y > 0.1f) hasAirDashed = true;
    }

    void DashUpdate()
    {
        playerController.rb.linearVelocity = dashDirection * dashForce;
        timerDashDuration -= Time.deltaTime;

        if (timerDashDuration <= 0) DashEnd();
    }

    void DashEnd()
    {
        isDash = false;
        playerController.rb.gravityScale = playerController.normalGravity;
        playerController.rb.linearVelocity = Vector2.zero;
    }
}