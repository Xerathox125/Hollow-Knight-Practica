using System.Collections;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Header("HurtBox")]
    public int damageEnemy; //Cantida de daño del hurtbox en cada golpe
    public float knockBackHurtBox; //Fuerza de retroceso que aplica
    public float timeKnockBack = 0.15f; //Tiempo que dura el knockback
    public Vector2 hurtBoxSize; //Tamaño del HurtBox
    public Vector2 hurtBoxOffSet; //Posición del HurtBox
    public LayerMask targetLayer; //Layre de los gameObjects afectados por el hurtbox

    // CoolDown
    public float damageCoolDown;
    private float coolDownTimer;

    void Update()
    {
        if(coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
            return;
        }

        CheckHurtBox();
    }

    private void CheckHurtBox()
    {
        Vector2 center = (Vector2)transform.position + hurtBoxOffSet;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, hurtBoxSize, 0f, targetLayer);


        foreach (Collider2D hit in hits)
        {
            Damageable damage = hit.GetComponent<Damageable>();
            if (damage != null)
            {
                //Punto de contacto entre el hurtbox y el collider del hit
                Vector2 contactPoint = hit.ClosestPoint(center);

                //Si el centro del hurtbox es muy cercano a el contact point, entonces forzamos dirección desde el center point
                if ((contactPoint - center).sqrMagnitude > 0.0001f)
                {
                    contactPoint = center;
                }
                damage.ApplyDamage(damageEnemy, contactPoint, knockBackHurtBox);
            }

            //En caso de que el hitbox detectado sea el player movement
            PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();
            if (playerMovement != null) 
            {
                playerMovement.onKnockBack = true;
                //Llamar a la corrutina
                StartCoroutine(ResetKnockBack(playerMovement, timeKnockBack));
            }

            coolDownTimer = damageCoolDown;

        }

    }

    IEnumerator ResetKnockBack(PlayerMovement playerMovement, float timeKnockBack)
    {
        yield return new WaitForSeconds(timeKnockBack);
        if (playerMovement != null) 
        {
            playerMovement.onKnockBack = false;
        }         
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + hurtBoxOffSet;
        Gizmos.DrawWireCube(center, hurtBoxSize);
    }

}
