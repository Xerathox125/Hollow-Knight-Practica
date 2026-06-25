using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public class DamageAble : MonoBehaviour
{
    private Rigidbody2D rb;
    private HealthHandler healthHandler;

    //Knockback
    [Header("Knockback")]
    public bool activeKnockback = true; // Esta variable determina si el efecto de knockback aplica a este game object
    public float knockBackDuration; // Cuanto dura el efecto

    // Flash
    public bool activeFlash = true; // Esta variable determina si el efecto de Flash aplica a este game object
    public float flashDuration = 0.1f;
    public Material flashMaterial;
    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;

    // Freeze Time 

    // Invulnerability

    // Particles

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthHandler = GetComponent<HealthHandler>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    // Aplicamos toods los efectos de Damage
    public void ApplyDamage(int damageAmount, Vector2 sourcePosition, float sourceKnockbackForce)
    {
        //Efecto Knockback
        if (activeKnockback)
        {
            KnockBackApply(sourcePosition, sourceKnockbackForce);
        }

        //Efecto Flash
        if (activeFlash)
        {
            StartCoroutine(FlashEffect());
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

    IEnumerator FlashEffect()
    {
        if (spriteRenderer == null || flashMaterial == null)
        {
            yield break;
        }

        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
    }
}
