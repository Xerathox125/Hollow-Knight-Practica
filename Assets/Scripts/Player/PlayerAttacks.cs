using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    PlayerController playerController;

    [Header("Ataque Player")]
    public int hitPower; //cantidad de daño hecho por el jugador
    public float attackRange = 0.6f;
    public float attackDistance = 0.6f;   // Esto será la distancia desde el jugador (qué tan lejos "sale" el ataque)
    public LayerMask enemyLayer;

    //Cooldowns
    public float attackCoolDown = 0.3f;
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
        Vector2 attackPos = (Vector2)transform.position + (attackDirection * attackDistance);

        Collider2D[] hurtEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);

        foreach (Collider2D enemy in hurtEnemies)
        {
            //Aplicamos el daño
            //Health Handler
            HealthHandler healthEnemy = enemy.GetComponent<HealthHandler>();
            if(healthEnemy != null)
            {
                healthEnemy.TakeDamage(hitPower);
            }
        }
    }

    public void EndAttack()
    {
        isAttack = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 gizmosPos = (Vector2)transform.position + (attackDirection * attackDistance);
        Gizmos.DrawWireSphere(gizmosPos, attackRange);
    }


}
