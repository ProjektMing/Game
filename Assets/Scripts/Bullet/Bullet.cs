using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Bullet
{
    /// <summary>
    /// 子弹基础类，处理子弹的基本行为、生命周期和碰撞检测
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [Header("子弹属性")] public float speed = 10f; // 子弹移动速度
        public Vector2 direction = Vector2.up; // 子弹移动方向
        private const float MaxSpeed = 20f; // 子弹最大速度限制
        public BulletTag target = BulletTag.Enemy; // 子弹目标类型

        /// <summary>
        /// 子弹标签枚举，用于区分子弹的目标类型
        /// </summary>
        public enum BulletTag
        {
            Player, // 针对玩家的子弹
            Enemy // 针对敌人的子弹
        }

        [Header("生命周期")] public float lifetime = 5f; // 子弹存在的最大时间（秒）

        // 对象池引用，用于子弹的回收和复用
        public IObjectPool<Bullet> Pool { get; set; }

        private Rigidbody2D _rb; // 刚体组件引用
        private float _lifeTimer; // 生命计时器

        /// <summary>
        /// 初始化时获取必要组件
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// 重置子弹状态，在从对象池获取时调用
        /// </summary>
        public void ResetState()
        {
            _lifeTimer = 0f;
            _rb.linearVelocity = direction * speed;
            _rb.angularVelocity = 0f;
        }

        /// <summary>
        /// 设置子弹的目标、方向和速度
        /// </summary>
        /// <param name="newTargets">子弹的目标类型</param>
        /// <param name="newDirection">子弹的移动方向</param>
        /// <param name="newSpeed">子弹的移动速度</param>
        public void SetState(BulletTag newTargets, Vector2 newDirection, float newSpeed)
        {
            target = newTargets;
            direction = newDirection.normalized;
            speed = newSpeed;
            _lifeTimer = 0f;
        }

        /// <summary>
        /// 每帧更新子弹的移动和生命周期
        /// </summary>
        private void Update()
        {
            // 更新子弹速度
            _rb.linearVelocity = direction * speed;

            // 限制最大速度
            if (_rb.linearVelocity.magnitude > MaxSpeed) _rb.linearVelocity = _rb.linearVelocity.normalized * MaxSpeed;

            // 更新生命计时器
            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= lifetime) Release(); // 超时销毁
        }

        /// <summary>
        /// 处理子弹碰撞
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 当碰撞目标与设定目标相同时，回收子弹
            if (other.CompareTag(target.ToString())) Release();
        }

        /// <summary>
        /// 将子弹返回对象池
        /// </summary>
        private void Release()
        {
            Pool.Release(this);
        }
    }
}