using Nez;
using Nez.Console;
using Sample.Scenes;
using Sample.Scenes.CombatScenes;

namespace Sample;

internal class Game : Core
{
    protected override void Initialize()
    {
        base.Initialize();
        Window.Title = "游戏示例";
        Window.AllowUserResizing = false;
        IsMouseVisible = false;
        ExitOnEscapeKeypress = false;

        Scene = new SplashScene();
    }

    [Command("to-combat", "toggle to Combat")]
    internal static void ToCombatScene()
    {
        Scene = new Scene1("东方弹幕秀");
    }
}
