using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public class DamageAble : MonoBehaviour
{
    private Rigidbody2D rb;
    private HealthHandler healthHandler;

    //Knockback
    [Header("Knockback")]
    public bool activeKnockback = true; //Esta variable determina si el efecto de knockback aplica a este game object
    public float knockBackDuration; //Cuanto dura el efecto

    // Flash

    // Freeze Time 

    // Invulnerability

    // Particles

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthHandler = GetComponent<HealthHandler>();
    }

    // Aplicamos toods los efectos de Damage
    public void ApplyDamage(int damageAmount, Vector2 sourcePosition, float sourceKnockbackForce)
    {
        if (activeKnockback)
        {
            KnockBackApply(sourcePosition, sourceKnockbackForce);
        }

        healthHandler.TakeDamage(damageAmount);
        // Acceder a Health y quitar damageAmount
    }

    private void KnockBackApply(Vector2 sourcePosition, float sourceKnockbackForce)
    {
        // Calcular la dirección
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;

        // Resetear valores y fuerzas
        rb.linearVelocity = Vector2.zero;

        // Ejecutar la rutina del knockback
        StartCoroutine(KnockBackRoutine(direction, sourceKnockbackForce, knockBackDuration));
    }
    IEnumerator KnockBackRoutine(Vector2 direction, float force, float duration)
    {
        //Agregamos impulso
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        //Esperamos un tiempo minimo
        yield return new WaitForSeconds(duration);

        // Resetear valores y fuerzas
        rb.linearVelocity = Vector2.zero;
    }
}
