using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int Health;
    [SerializeField] public int MaxHealth = 5;
    [SerializeField] public float MoveSpeed = 5f;

    [SerializeField] public int ScoreValue = 1;
    // [SerializeField]
    // public Emitter Shoot;

    [Header("Pooling Settings")] // 可选，为了在 Inspector 中分组
    [Tooltip("The tag used to identify this enemy type in the ObjectPooler.")]
    public string poolTag = "Enemy"; // <--- 在这里声明和初始化 poolTag

    public IObjectPool<Enemy> pool;


    // Start is called before the first frame update
    private void Start()
    {
        Health = MaxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        // Test();
        if (Health <= 0) Die();
    }


    private void Die()
    {
        // 播放死亡特效、掉落道具等
        Debug.Log(gameObject.name + " Died!");
        // Destroy(gameObject); // 返回对象池
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
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

    public void ResetState()
    {
        Health = MaxHealth;
        // 如果有其他需要重置的状态，也在这里进行
        // 例如，重置动画状态、清除已发射的子弹引用等
        // Debug.Log(gameObject.name + " state reset. Health: " + Health);
    }

    // private void Test()//一个测试掉血
    // {
    //     if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         Health--;
    //     }
    // }
}