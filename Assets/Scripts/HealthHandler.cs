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
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        //Activamos partículas
        //generamos sonido

        Destroy(gameObject);
    }
   
}
