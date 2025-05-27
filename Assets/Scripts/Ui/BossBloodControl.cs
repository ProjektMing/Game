using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyHealthSystem : MonoBehaviour
{
    [Header("Prop")] public float maxHealth = 100f;
    public float Health;

    [Header("BloodUI")] public Slider healthSlider;

    private void Start()
    {
        Health = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = Health;
            //healthSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (healthSlider != null) healthSlider.value = Health;

        if (Health <= 0) Die();
    }

    public void BossAppear()
    {
        if (healthSlider != null) healthSlider.gameObject.SetActive(true);
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


    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("win");
    }

    // ����
    private void Die()
    {
        StartCoroutine(Pause());
        Debug.Log("已死亡");
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // 如果设置正确，这条日志现在应该会出现
        Debug.Log(
            $"敌人 OnTriggerEnter2D: '{gameObject.name}' 与 '{otherCollider.gameObject.name}' (标签: '{otherCollider.tag}') 发生碰撞");

        // 你的子弹预制体的标签是什么？
        // 假设你的子弹预制体标签是 "Bullet" (或者更具体地说是 "PlayerBullet")
        if (otherCollider.CompareTag("PlayerBullet")) //  <--- 如果你的子弹预制体有不同的标签，请更改此处的 "Bullet"
        {
            Debug.Log($"敌人: '{gameObject.name}' 确认被子弹击中。之前生命值: {Health}");
            Health--;
            Debug.Log($"敌人: '{gameObject.name}' 之后生命值: {Health}");

            // Bullet.cs 在击中标签为 "Enemy" 的对象时已经处理了自身的禁用/回收。
            // 所以，Enemy 脚本不需要在这里销毁或释放子弹。

            if (Health <= 0) Debug.Log($"敌人: '{gameObject.name}' 因触发器碰撞导致生命值耗尽。");
            // Die 会在 Update 中被调用，或者你可以在这里直接调用以立即生效
        }
        else
        {
            Debug.Log(
                $"敌人: '{gameObject.name}' 与 '{otherCollider.gameObject.name}' (标签: {otherCollider.tag}) 碰撞, 但它不是 'Bullet' 标签。");
        }
    }
}