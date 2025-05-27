using UnityEngine;
using System.Collections;

namespace Bullet
{
    public class EnemyBulletEmitter : BulletEmitter
    {
        [Header("弹幕设置")] [SerializeField] private float spreadAngle = 45f; // 扇形弹幕的扩散角度
        [SerializeField] private int bulletsPerSpread = 5; // 扇形弹幕的子弹数量

        private void Start()
        {
            // 设置基础属性
            baseFireRate = 1f;
            baseBulletSpeed = 5f;
            StartCoroutine(AutoFireCoroutine());
        }

        // 自动射击协程
        private IEnumerator AutoFireCoroutine()
        {
            while (true)
            {
                if (canFire)
                {
                    FireSpreadPattern();
                    yield return new WaitForSeconds(baseFireRate);
                }

                yield return null;
            }
        }

        // 扇形弹幕模式
        private void FireSpreadPattern()
        {
            if (!canFire || Time.time < nextFireTime) return;

            // 向下发射扇形弹幕
            var targetDirection = Vector2.down;
            FireMultiple(targetDirection, baseBulletSpeed, Bullet.BulletTag.Player,
                bulletsPerSpread, spreadAngle);
        }

        // 设置扇形弹幕的参数
        public void SetSpreadParameters(float angle, int count)
        {
            spreadAngle = angle;
            bulletsPerSpread = count;
        }
    }
}