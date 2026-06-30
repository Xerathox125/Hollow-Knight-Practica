using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    PlayerController playerController;

    [Header("Ataque Player")]
    public int hitPower;
    public int knockBackForce;
    public float attackRange;
    public Vector2 hitBoxOffset;
    public float pogoForce;
    public LayerMask enemyLayer;

    //Cooldowns
    public float attackCoolDown;
    private float timerAttackCoolDown = 0f;
    private bool isAttack;
    private Vector2 attackDirection;
    private bool wasOnWallAttack; // Flag: atacó desde pared

    // Getters
    public bool IsAttack => isAttack;
    public Vector2 AttackDirection => attackDirection;
    public bool WasOnWallAttack => wasOnWallAttack;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        if (timerAttackCoolDown > 0)
            timerAttackCoolDown -= Time.fixedDeltaTime;
    }

    public void AttackHold()
    {
        if (isAttack || timerAttackCoolDown > 0) return;

        timerAttackCoolDown = attackCoolDown;
        isAttack = true;

        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        bool isOnWall = playerController.wallJump != null
                        && playerController.wallJump.IsWall
                        && !playerController.jump.IsGrounded;

        if (move.y < -0.2f && !playerController.jump.IsGrounded)
        {
            attackDirection = Vector2.down;
        }
        else if (move.y > 0.2f)
        {
            attackDirection = Vector2.up;
        }
        else
        {
            bool facingRight = playerController.movement.IsFacingRight;
            bool attackRight = isOnWall ? !facingRight : facingRight;
            attackDirection = attackRight ? Vector2.right : Vector2.left;

            if (isOnWall)
            {
                wasOnWallAttack = true;
                // Voltea localScale visualmente sin cambiar IsFacingRight
                // para no romper la lógica de detección de pared
                Vector3 s = transform.localScale;
                transform.localScale = new Vector3(-s.x, s.y, s.z);
            }
        }
    }

    public void ActiveHitbox()
    {
        Vector2 attackPos = (Vector2)transform.position + attackDirection * attackRange + Vector2.Scale(hitBoxOffset, attackDirection);
        Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);

        foreach (Collider2D enemy in hurtEnemies)
        {
            Damageable damageable = enemy.GetComponent<Damageable>();
            if (damageable != null)
                damageable.ApplyDamage(hitPower, transform.position, knockBackForce);

            if (attackDirection == Vector2.down)
                Pogo();
        }
    }

    public void Pogo()
    {
        playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, 0f);
        playerController.rb.AddForce(Vector2.up * pogoForce, ForceMode2D.Impulse);
    }

    public void EndAttack()
    {
        isAttack = false;
        // El scale se restaura desde UpdateAnimsPlayer cuando corresponde,
        // para sincronizarlo con el cambio de animación y evitar el parpadeo de un frame
    }

    public void RestoreWallAttackScale()
    {
        if (wasOnWallAttack)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(-s.x, s.y, s.z);
            wasOnWallAttack = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 gizmosPos = (Vector2)transform.position + attackDirection * attackRange + Vector2.Scale(hitBoxOffset, attackDirection);
        Gizmos.DrawWireSphere(gizmosPos, attackRange);
    }
}