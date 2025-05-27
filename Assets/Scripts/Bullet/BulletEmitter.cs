using UnityEngine;
using System.Collections;

namespace Bullet
{
    /// <summary>
    /// 子弹发射器的抽象基类，提供基础的子弹发射功能
    /// 所有具体的发射器类都应该继承自此类
    /// </summary>
    public abstract class BulletEmitter : MonoBehaviour
    {
        [Header("基础设置")] [SerializeField] protected Transform firePoint; // 子弹发射点
        [SerializeField] protected BulletPool bulletPool; // 子弹对象池引用
        [SerializeField] protected float baseFireRate = 0.5f; // 基础射击频率（秒）
        [SerializeField] protected float baseBulletSpeed = 10f; // 基础子弹速度

        protected bool canFire = true; // 是否可以发射
        protected float nextFireTime; // 下一次可发射的时间点

        /// <summary>
        /// 基础发射方法，发射单发子弹
        /// </summary>
        /// <param name="direction">发射方向</param>
        /// <param name="speed">子弹速度</param>
        /// <param name="target">目标类型</param>
        protected virtual void Fire(Vector2 direction, float speed, Bullet.BulletTag target)
        {
            if (!canFire || Time.time < nextFireTime) return;

            // 从对象池获取子弹并初始化
            var bullet = bulletPool.Pool.Get();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0,
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            bullet.SetState(target, direction, speed);
            nextFireTime = Time.time + baseFireRate;
        }

        /// <summary>
        /// 多发射击方法，可以发射扇形弹幕
        /// </summary>
        /// <param name="baseDirection">基准发射方向</param>
        /// <param name="speed">子弹速度</param>
        /// <param name="target">目标类型</param>
        /// <param name="bulletCount">子弹数量</param>
        /// <param name="spreadAngle">扩散角度</param>
        protected virtual void FireMultiple(Vector2 baseDirection, float speed, Bullet.BulletTag target,
            int bulletCount, float spreadAngle)
        {
            if (!canFire || Time.time < nextFireTime) return;

            // 计算每颗子弹之间的角度间隔
            for (var i = 0; i < bulletCount; i++)
            {
                var angle = -spreadAngle / 2 + spreadAngle / (bulletCount - 1) * i;
                var direction = RotateVector(baseDirection, angle);

                // 从对象池获取子弹并初始化
                var bullet = bulletPool.Pool.Get();
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0,
                    Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

                bullet.SetState(target, direction, speed);
            }

            nextFireTime = Time.time + baseFireRate;
        }

        /// <summary>
        /// 辅助方法：将向量按指定角度旋转
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="degrees">旋转角度（度）</param>
        /// <returns>旋转后的向量</returns>
        protected Vector2 RotateVector(Vector2 vector, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }

        /// <summary>
        /// 启用发射功能
        /// </summary>
        public virtual void EnableFiring()
        {
            canFire = true;
        }

        /// <summary>
        /// 禁用发射功能
        /// </summary>
        public virtual void DisableFiring()
        {
            canFire = false;
        }

        /// <summary>
        /// 设置发射频率
        /// </summary>
        /// <param name="rate">发射间隔（秒）</param>
        public virtual void SetFireRate(float rate)
        {
            baseFireRate = rate;
        }

        /// <summary>
        /// 设置子弹速度
        /// </summary>
        /// <param name="speed">子弹速度</param>
        public virtual void SetBulletSpeed(float speed)
        {
            baseBulletSpeed = speed;
        }
    }
}