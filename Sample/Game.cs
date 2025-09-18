using Microsoft.Xna.Framework;
using Nez;
using Nez.Console;
using Sample.Scenes;
using Sample.Scenes.CombatScenes;
using Sample.Systems; // updated namespace

namespace Sample;

internal class Game : Core
{
    public Game()
        : base(windowTitle: "东方测试乡") { }

    protected override void Initialize()
    {
        base.Initialize();
        Window.Title = "东方测试乡";
        Window.AllowUserResizing = false;
        IsMouseVisible = false; // hide cursor since we block mouse input
        ExitOnEscapeKeypress = false;

        // register a global mouse blocker to neutralize all mouse input
        RegisterGlobalManager(new MouseBlocker());

        Scene = new SplashScene();
    }

    [Command("to-combat", "toggle to Combat")]
    internal static void ToCombatScene()
    {
        Scene = new Scene1();
    }
}
