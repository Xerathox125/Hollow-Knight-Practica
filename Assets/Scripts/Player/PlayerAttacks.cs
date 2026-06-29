using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    PlayerController playerController;

    [Header("Ataque Player")]
    public int hitPower; //cantidad de daño hecho por el jugador
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
    private bool wasOnWallAttack; // Flag para saber si atacó desde la pared

    // Getters
    public bool IsAttack => isAttack;
    public Vector2 AttackDirection => attackDirection;
    public bool WasOnWallAttack => wasOnWallAttack; // Expuesto para que otros scripts lo lean


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnUpdate()
    {
        if (timerAttackCoolDown > 0)
        {
            timerAttackCoolDown -= Time.fixedDeltaTime;
        }
    }

    //Este metodo se llama al apretar la tecla de attack del input
    public void AttackHold()
    {
        if (isAttack || timerAttackCoolDown > 0) return;

        timerAttackCoolDown = attackCoolDown;
        isAttack = true;

        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // Verifica si el jugador está sostenido en una pared (Megaman/Hollow Knight style)
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
            // En pared: ataca hacia afuera (opuesto al sprite), como Megaman/Hollow Knight
            // Sin pared: ataca hacia donde mira
            bool facingRight = playerController.movement.IsFacingRight;
            bool attackRight = isOnWall ? !facingRight : facingRight;
            attackDirection = attackRight ? Vector2.right : Vector2.left;

            // Si está en pared, voltea el sprite hacia donde ataca
            if (isOnWall)
            {
                wasOnWallAttack = true;
                playerController.movement.SetFacing(attackRight);
            }
        }
    }

    public void ActiveHitbox()
    {
        Vector2 attackPos = (Vector2)transform.position + attackDirection * attackRange + Vector2.Scale(hitBoxOffset, attackDirection);

        Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);

        foreach (Collider2D enemy in hurtEnemies)
        {
            //Aplicamos el daño
            Damageable damageable = enemy.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(hitPower, transform.position, knockBackForce);
            }

            if (attackDirection == Vector2.down)
                Pogo();
        }
    }

    public void Pogo()
    {
        //Cancelamos las fuerzas y velocidades del RigidBody del player
        playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, 0f);
        //Aplicamos un impulso hacia arriba a nuestro jugador
        playerController.rb.AddForce(Vector2.up * pogoForce, ForceMode2D.Impulse);
    }

    public void EndAttack()
    {
        isAttack = false;

        // Restaura la dirección del sprite si atacó desde la pared
        if (wasOnWallAttack)
        {
            playerController.movement.SetFacing(!playerController.movement.IsFacingRight);
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