using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class enemySystem : MonoBehaviour
{
    [Header("Prop")] public float maxHealth = 100f;
    public float Health;

    private void Start()
    {
        Health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        Health = Mathf.Max(Health, 0f);

        if (Health <= 0) Die();
    }

    public void Heal(float amount)
    {
        Health += amount;
        Health = Mathf.Min(Health, maxHealth);
    }

    private void Die()
    {
    }
}