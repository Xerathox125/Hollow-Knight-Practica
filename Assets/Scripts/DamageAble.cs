using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class Damageable : MonoBehaviour
{
    private Rigidbody2D rb;
    private HealthHandler healthHandler;

    //Knockback
    [Header("Knockback Effect")]
    public bool activeKnockback = true; // Esta variable determina si el efecto de knockback aplica a este game object
    public float knockBackDuration; // Cuanto dura el efecto de knockback

    // Flash
    [Header("Flash Effect")]
    public bool activeFlash = true; // Esta variable determina si el efecto de Flash aplica a este game object
    public float flashDuration;
    public Material flashMaterial;
    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;

    // Freeze Time 
    [Header("Freeze Time Effect")]
    public bool activeFreezeTime = true;
    public float freezeDuration;

    // Invulnerability
    [Header("Invulnerability Effect")]    
    private bool isInvulnerable = false;       // Verifica si estamos o no vulnerables
    public float timeInvulnerability;          // Tiempo de invulnerabilidad del personaje


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

    // Aplicamos todos los efectos de Damage
    public void ApplyDamage(int damageAmount, Vector2 sourcePosition, float sourceKnockbackForce)
    {
        //Efecto Invulnerable
        if (isInvulnerable)
        {
            return;
        }

        // Efecto Knockback
        if (activeKnockback)
        {
            KnockBackApply(sourcePosition, sourceKnockbackForce);
        }

        //Efecto Flash
        if (activeFlash)
        {
            StartCoroutine(FlashEffect());
        }

        //Efecto Freeze Time
        if (activeFreezeTime)
        {
            StartCoroutine(FreezeTimeEffect());
        }

        StartCoroutine(InvulnerabilityEffect());
        

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

    IEnumerator FreezeTimeEffect()
    {
        float originalTime = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeDuration);
        Time.timeScale = originalTime;
    }

    IEnumerator InvulnerabilityEffect()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(timeInvulnerability);
        isInvulnerable = false;
    }
}
