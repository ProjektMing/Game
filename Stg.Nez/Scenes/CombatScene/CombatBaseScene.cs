using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;

namespace Stg.Nez.Scenes.CombatScene;

public class CombatBaseScene(string name, BitmapFont? font = null) : Scene
{
    public UICanvas? Canvas;

    public override void Initialize()
    {
        base.Initialize();
        Canvas = CreateEntity("UI").AddComponent(new UICanvas());
        Canvas.IsFullScreen = true;
        Canvas.AddComponent(
            new TextComponent(
                font ?? Graphics.Instance.BitmapFont,
                name,
                new Vector2(0, 0),
                Color.Black
            )
        );
        CreateEntity("Player");
    }
}
