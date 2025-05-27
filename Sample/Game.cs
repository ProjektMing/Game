using Nez;

namespace Sample;

internal class Game : Core
{
    protected override void Initialize()
    {
        base.Initialize();
        Window.Title = "游戏示例";
        Window.AllowUserResizing = false;
        Scene = new Scenes.SplashScene();

    }
}