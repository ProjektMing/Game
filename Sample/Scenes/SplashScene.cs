using Microsoft.Xna.Framework;
using Nez;
using Sample.Scenes.CombatScenes;

namespace Sample.Scenes;

public class SplashScene : Scene
{
    bool isTransitioning = false;

    public override void Begin()
    {
        base.Begin();

        var splashEntity = CreateEntity("Splash!");
        splashEntity.Transform.Scale = new Vector2(9f, 9f);
        splashEntity.Transform.Position = Screen.Center;

        var text = new TextComponent(
            Graphics.Instance.BitmapFont,
            "Touhou\nProject",
            Vector2.Zero,
            Color.Red
        )
        {
            HorizontalOrigin = HorizontalAlign.Center,
            VerticalOrigin = VerticalAlign.Center,
        };
        splashEntity.AddComponent(text);

        // 等待2秒后切换到下一个场景
        Core.Schedule(
            2f,
            timer =>
            {
                if (isTransitioning)
                {
                    return;
                }
                // 切换到下一个场景
                Core.StartSceneTransition(new WindTransition(() => new Menu()));
            }
        );
    }

    public override void Update()
    {
        // 如果按下空格键，则切换到下一个场景
        if (Input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
        {
            isTransitioning = true;
            Core.StartSceneTransition(new WindTransition(() => new Menu()));
            return;
        }
        base.Update();
    }
}
