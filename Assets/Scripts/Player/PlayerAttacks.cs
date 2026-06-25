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

    // Getters
    public bool IsAttack => isAttack;
    public Vector2 AttackDirection => attackDirection;
       

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
        if (isAttack || timerAttackCoolDown > 0)
        {
            return;
        }

        timerAttackCoolDown = attackCoolDown;
        isAttack = true;

        // Calcular el input del jugador
        Vector2 move = playerController.controles.Player.Move.ReadValue<Vector2>();

        // Calcular la dirección del ataque
        // Ataque hacia abajo
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
            if (playerController.movement.IsFacingRight)
            {
                attackDirection = Vector2.right;
            }
            else
            {
                attackDirection = Vector2.left;
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
            DamageAble damageable = enemy.GetComponent<DamageAble>();
            if (damageable != null)
            {
                damageable.ApplyDamage(hitPower, transform.position, knockBackForce);
            }

            /*
            //Health Handler
            HealthHandler healthEnemy = enemy.GetComponent<HealthHandler>();
            if (healthEnemy != null)            
                healthEnemy.TakeDamage(hitPower);
            */
            
            if (attackDirection == Vector2.down)            
                Pogo();            
        }
    }

    public void Pogo()
    {
        //Cancelamos las fuerzas y velocidades del RigidBody del player
        playerController.rb.linearVelocity = new Vector2(playerController.rb.linearVelocity.x, 0f);
        //Aplicamos un impulso hacia arriba a nuestro jugador
        playerController.rb.AddForce(Vector2.up * pogoForce,ForceMode2D.Impulse);
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 gizmosPos = (Vector2)transform.position + attackDirection * attackRange + Vector2.Scale(hitBoxOffset, attackDirection);
        Gizmos.DrawWireSphere(gizmosPos, attackRange);
    }


}
