using Microsoft.Xna.Framework;
using Nez;

namespace Stg.Nez.Entities;

public class Bullet : Entity
{
    public float Speed { get; set; } = 10f; // 子弹移动速度
    public Vector2 Direction { get; set; } = Vector2.Zero; // 子弹移动方向
    private const float MaxSpeed = 20f; // 子弹最大速度限制
    public BulletTag Target { get; set; } = BulletTag.Enemy; // 子弹目标类型

    private readonly BulletPool _pool;

    private readonly Collider _collider;

    /// <summary>
    /// 子弹标签枚举，用于区分子弹的目标类型
    /// </summary>
    public enum BulletTag
    {
        Player, // 针对玩家的子弹
        Enemy // 针对敌人的子弹
    }

    public float Lifetime { get; init; } = 5f;
    private float elapsedTime = 0f; // 子弹已存在的时间

    public Bullet(BulletPool pool)
    {
        _pool = pool;
        _collider = AddComponent<CircleCollider>();
    }

    public override void Update()
    {
        elapsedTime += Time.DeltaTime;

        if (elapsedTime >= Lifetime)
        {
            _pool.Return(this);
        }
        base.Update();
    }

    public void Reset()
    {
        Enabled = false;
        Position = Vector2.Zero; // 重置位置
        Direction = Vector2.Zero; // 重置方向
        Speed = 10f; // 重置速度
        Target = BulletTag.Enemy; // 重置目标类型
        elapsedTime = 0f; // 重置已存在时间
        _collider.Enabled = true; // 确保碰撞器启用
    }
}
