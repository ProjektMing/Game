// Filepath: Stg.Nez/Scenes/StgBaseScene.cs

// For List<Entity>
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts; // For BitmapFont

namespace Stg.Nez.Scenes;

public class StgBaseScene : Scene
{
    public required Entity Player { get; set; }
    public List<Entity> Enemies { get; protected set; } = [];

    public RectangleF ScreenBounds { get; protected set; }
    public bool DestroyEnemiesOnExitScreen { get; set; } = true;

    protected BitmapFont? SceneFont = Graphics.Instance.BitmapFont;

    public override void Initialize()
    {
        base.Initialize();

        // 设置设计分辨率。对于STG，通常使用固定的垂直或水平分辨率。
        // 示例：384x448 是一个常见的纵向STG分辨率。
        SetDesignResolution(384, 448, SceneResolutionPolicy.ShowAllPixelPerfect);
        ScreenBounds = new RectangleF(0, 0, DesignWidth, DesignHeight);

        // 玩家实体
        Player = CreateEntity("player");
        // 玩家实体应有其自身的组件用于渲染、输入和移动。
        // 例如: Player.AddComponent(new SpriteRenderer(playerTexture));
        // Player.Position = new Vector2(DesignWidth / 2f, DesignHeight * 0.8f);
        // 边界限制最好通过一个专用组件实现:
        // Player.AddComponent(new PlayerBoundaryComponent(ScreenBounds));


        // 用于显示场景名称的基础UI (可选, 用于调试或演示)
        var canvas = CreateEntity("ui-canvas").AddComponent(new UICanvas());
        // canvas.IsFullScreen = true; // 使画布匹配最终渲染目标大小。
        // 如果使用DesignResolution，UI元素可能需要缩放，
        // 或者画布的渲染层需要与场景的渲染层匹配。

        // 添加场景标题文本
        canvas.Entity.AddComponent(
            new TextComponent(
                SceneFont,
                this.GetType().Name, // 显示实际的场景类名
                new Vector2(10, 10),
                Color.White
            )
        );
    }

    public float DesignHeight { get; set; }

    public float DesignWidth { get; set; }

    public override void Update()
    {
        base.Update();
        UpdatePlayerConfinement();
        UpdateEnemiesOffScreenBehavior();
    }

    protected virtual void UpdatePlayerConfinement()
    {
        if (Player == null || !Player.Enabled)
            return;

        // 此逻辑理想情况下应位于PlayerMovementComponent或专用的BoundaryComponent中。
        // 此处为简化示例，直接在场景中实现。
        // 假设 Player.Position 是玩家的中心点。
        // 需要知道玩家的大小。这里使用占位符值。
        float playerHalfWidth = 16f; // 示例: 玩家宽度为32px
        float playerHalfHeight = 16f; // 示例: 玩家高度为32px
        // 更好的方式: var collider = Player.GetComponent<Collider>();
        // if(collider != null) { playerHalfWidth = collider.Bounds.Width / 2f; playerHalfHeight = collider.Bounds.Height / 2f; }


        var posX = MathHelper.Clamp(
            Player.Position.X,
            ScreenBounds.Left + playerHalfWidth,
            ScreenBounds.Right - playerHalfWidth
        );
        var posY = MathHelper.Clamp(
            Player.Position.Y,
            ScreenBounds.Top + playerHalfHeight,
            ScreenBounds.Bottom - playerHalfHeight
        );
        Player.Position = new Vector2(posX, posY);
    }

    protected virtual void UpdateEnemiesOffScreenBehavior()
    {
        for (int i = Enemies.Count - 1; i >= 0; i--)
        {
            var enemy = Enemies[i];
            if (enemy == null || !enemy.Enabled) // 检查敌人是否仍然有效或已在别处销毁
            {
                Enemies.RemoveAt(i);
                continue;
            }

            // 判断敌人是否在屏幕外。这需要知道敌人的边界。
            // 这是一个简化的检查，假设 enemy.Position 是其中心。
            // 更健壮的检查会使用敌人的 Collider 或 RenderableComponent 的边界。
            float enemyHalfWidth = 16f; // 示例值
            float enemyHalfHeight = 16f; // 示例值
            // var collider = enemy.GetComponent<Collider>();
            // if (collider != null) {
            //    enemyHalfWidth = collider.Bounds.Width / 2f;
            //    enemyHalfHeight = collider.Bounds.Height / 2f;
            // }

            var enemyRect = new RectangleF(
                enemy.Position.X - enemyHalfWidth,
                enemy.Position.Y - enemyHalfHeight,
                enemyHalfWidth * 2,
                enemyHalfHeight * 2
            );

            // ScreenBounds 是可见的游戏区域。
            // 检查敌人是否与屏幕区域不相交。
            if (!ScreenBounds.Intersects(enemyRect))
            {
                // 为了确保敌机是真正飞出屏幕（而不是从屏幕外进入时短暂接触边界），
                // 可能需要更复杂的判断，例如检查其是否已远离屏幕中心一段距离。
                bool trulyOffScreen = false;
                if (
                    enemy.Position.X < ScreenBounds.Left - enemyHalfWidth * 2
                    || // 远在左侧
                    enemy.Position.X > ScreenBounds.Right + enemyHalfWidth * 2
                    || // 远在右侧
                    enemy.Position.Y < ScreenBounds.Top - enemyHalfHeight * 2
                    || // 远在上方
                    enemy.Position.Y > ScreenBounds.Bottom + enemyHalfHeight * 2
                ) // 远在下方
                {
                    trulyOffScreen = true;
                }

                if (trulyOffScreen)
                {
                    OnEnemyExitedScreen(enemy);
                    if (DestroyEnemiesOnExitScreen)
                    {
                        enemy.Destroy(); // 从场景中移除并销毁实体
                        Enemies.RemoveAt(i); // 从我们的管理列表中移除
                    }
                }
            }
        }
    }

    // 当确定敌人离开屏幕时调用 (在可能销毁之前)
    protected virtual void OnEnemyExitedScreen(Entity enemy)
    {
        // Nez.Debug.Log($"Enemy {enemy.Name ?? "Unnamed"} exited screen.");
        // 可在此处添加自定义逻辑 (例如计分、触发事件等)
    }

    // 用于从其他系统 (如敌机生成器) 管理敌机的公共方法
    public virtual Entity AddEnemy(string name = "enemy")
    {
        var enemy = CreateEntity(name); // 在场景中创建实体
        Enemies.Add(enemy); // 添加到我们的管理列表
        // 敌人实体应有其自身的组件用于AI、渲染、碰撞等。
        // 例如: enemy.AddComponent(new EnemyAI());
        //       enemy.AddComponent(new SpriteRenderer(enemyTexture));
        //       enemy.AddComponent(new BoxCollider());
        return enemy;
    }

    public virtual Entity AddEnemy(Entity enemyInstance)
    {
        // 如果实体不是通过此处的CreateEntity创建的，确保它已添加到场景中
        if (enemyInstance.Scene == null)
            AddEntity(enemyInstance);
        if (!Enemies.Contains(enemyInstance))
            Enemies.Add(enemyInstance);
        return enemyInstance;
    }

    public virtual void RemoveEnemy(Entity enemy)
    {
        if (Enemies.Remove(enemy))
        {
            enemy.Destroy(); // 从场景中移除并销毁实体
        }
    }
}
