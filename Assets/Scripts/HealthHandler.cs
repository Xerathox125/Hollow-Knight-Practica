using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    public int maxHealth;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        //Activamos partículas
        //generamos sonido

        Destroy(gameObject);
    }
   
}
