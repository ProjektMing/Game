using Microsoft.Xna.Framework;
using Nez;

namespace Sample.Scenes;

public class SplashScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();

        var splashEntity = CreateEntity("Splash");
        splashEntity.Transform.Scale = new Vector2(8f, 8f);
        splashEntity.Transform.Position = Screen.Center + new Vector2(100, -200);

        var text = new TextComponent(
            Graphics.Instance.BitmapFont,
            "Touhou\nProject",
            new Vector2(0, 0),
            Color.Red
        );
        splashEntity.AddComponent(text);
        text.HorizontalOrigin = HorizontalAlign.Center;
        text.VerticalOrigin = VerticalAlign.Center;
    }
}
