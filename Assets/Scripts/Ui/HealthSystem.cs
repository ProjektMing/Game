using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("血量控制")] public float maxHealth = 5f; //最大血量
    public float Health;

    [Header("slider")] public Slider healthSlider;

    private void Start()
    {
        Health = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = Health;
        }
    }

    private void Update()
    {
        if (healthSlider != null) healthSlider.value = Health;

        if (Health <= 0) Die();
    }

    // public  void TakeDamage(float damage)
    // {
    //     Health -= damage;
    //     Health = Mathf.Max(Health, 0f); 
    //
    //     if (Health <= 0)
    //     {
    //         Die();
    //     }
    // }

    public void Heal(float amount)
    {
        Health += amount;
        Health = Mathf.Min(Health, maxHealth);
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("over");
    }

    private void Die()
    {
        StartCoroutine(Pause());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) Health--;
    }
}