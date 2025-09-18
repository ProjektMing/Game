using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Stg.Nez.Entities;

namespace Stg.Nez.Scenes.CombatScene;

public class CombatBaseScene : Scene
{
    public UICanvas? Canvas { get; private set; }
    private BulletPool? _bulletPool;

    [MemberNotNull(nameof(Canvas))]
    [MemberNotNull(nameof(_bulletPool))]
    public override void Initialize()
    {
        base.Initialize();
        Canvas = CreateEntity("UI").AddComponent(new UICanvas());
        Canvas.IsFullScreen = true;
        Canvas.AddComponent(
            new TextComponent(
                Graphics.Instance.BitmapFont,
                "Combat",
                new Vector2(10, 10),
                Color.Black
            )
        );
        CreateEntity("Player");
        _bulletPool = new BulletPool();
        _bulletPool.Get();
    }
}
