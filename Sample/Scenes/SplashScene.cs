using Microsoft.Xna.Framework;
using Nez;

namespace Sample.Scenes;

public class SplashScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();

        var textEntity = CreateEntity("SplashText");
        textEntity.Transform.Scale = new Vector2(8f, 8f);
        textEntity.Transform.Position = Screen.Center;

        var text = new TextComponent(Graphics.Instance.BitmapFont, "Splash!", new Vector2(0, 0), Color.Red);
        textEntity.AddComponent(text);
        text.HorizontalOrigin = HorizontalAlign.Center;
        text.VerticalOrigin = VerticalAlign.Center;
    }
}