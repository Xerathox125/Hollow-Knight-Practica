using UnityEngine;

public class PlayerStairs : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Escaleras")]
    public float stairsSpeed;
    public bool rangeStairs;
    private bool isStairs;

    private float cooldownTimer = 0f;

    // Expresión de cuerpo para simplificar sintaxis
    public bool IsStairs => isStairs;

    private void Start() => playerController = GetComponent<PlayerController>();

    public void OnUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        DetectStairs();
    }

    public void OnFixedUpdate() => MoveStairs();

    void DetectStairs()
    {
        // Optimizamos usando el moveInput pre-calculado
        if (rangeStairs && !isStairs && cooldownTimer <= 0f && playerController.moveInput.y > 0.5f)
        {
            StartStairs();
        }
    }

    void StartStairs()
    {
        isStairs = true;
        playerController.rb.gravityScale = 0f;
        playerController.rb.linearVelocity = Vector2.zero;
    }

    void MoveStairs()
    {
        if (!isStairs) return;

        playerController.rb.gravityScale = 0f;
        Vector2 move = playerController.moveInput; // Input optimizado
        playerController.rb.linearVelocity = move * stairsSpeed; // Multiplicación directa vectorial (más rápido que hacer new Vector2)
    }

    public void ExitStairs(float cooldown = 0f)
    {
        isStairs = false;
        cooldownTimer = cooldown;
        playerController.rb.gravityScale = playerController.normalGravity;
    }
}