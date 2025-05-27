using UnityEngine;

namespace Bullet
{
    public class PlayerBulletEmitter : BulletEmitter
    {
        [Header("玩家发射器设置")] [SerializeField] private float spreadAngle = 15f; // 散射角度
        [SerializeField] private int bulletPerShot = 1; // 每次发射的子弹数量
        [SerializeField] private bool autoFire = false; // 是否自动发射

        private void Update()
        {
            // 检测Z键输入
            if (autoFire || Input.GetKey(KeyCode.Z)) HandleShooting();
        }

        private void HandleShooting()
        {
            // 固定向上发射
            var direction = Vector2.up;

            // 根据子弹数量选择发射方式
            if (bulletPerShot > 1)
                FireMultiple(direction, baseBulletSpeed, Bullet.BulletTag.Enemy,
                    bulletPerShot, spreadAngle);
            else
                Fire(direction, baseBulletSpeed, Bullet.BulletTag.Enemy);
        }
    }
}