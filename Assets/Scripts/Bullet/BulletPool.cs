using UnityEngine;
using UnityEngine.Pool;

// 子弹对象池管理器
namespace Bullet
{
    public class BulletPool : MonoBehaviour
    {
        [Header("设置")] public Bullet bulletPrefab;
        public int defaultCapacity = 200;
        public int maxSize = 500;

        private IObjectPool<Bullet> _pool;

        public IObjectPool<Bullet> Pool
        {
            get
            {
                return _pool ??= new ObjectPool<Bullet>(
                    CreateBullet,
                    OnGetFromPool,
                    OnReleaseToPool,
                    OnDestroyBullet,
                    true,
                    defaultCapacity,
                    maxSize
                );
            }
        }

        // 创建新子弹（当池空时自动调用）
        private Bullet CreateBullet()
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.Pool = Pool;
            return bullet;
        }

        // 从池取出时的初始化
        private static void OnGetFromPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
            bullet.ResetState();
        }

        // 放回池时的清理
        private void OnReleaseToPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            bullet.transform.SetParent(transform); // 统一管理层级
        }

        // 当超过最大容量时的销毁处理
        private static void OnDestroyBullet(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        // 预加载对象池
        public void Prewarm(int count)
        {
            var bullets = new Bullet[count];
            for (var i = 0; i < count; i++) bullets[i] = Pool.Get();
            foreach (var bullet in bullets) Pool.Release(bullet);
        }
    }
}