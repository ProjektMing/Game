using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace Sample.Scenes;

internal class Menu : Scene
{
    UICanvas _uiCanvas;

    [MemberNotNull(nameof(_uiCanvas))]
    public override void Begin()
    {
        base.Begin();

        _uiCanvas = CreateEntity("MenuCanvas").AddComponent(new UICanvas());

        var table = _uiCanvas.Stage.AddElement(new Table());
        table.SetFillParent(true);
        var bar = new ProgressBar(
            0,
            1,
            0.1f,
            false,
            ProgressBarStyle.Create(Color.Black, Color.White)
        );
        table.Add(bar);
        table.Row();
        var slider = new Slider(
            0,
            1,
            0.1f,
            false,
            SliderStyle.Create(Color.DarkGray, Color.LightYellow)
        );
        table.Add(slider);
        table.Row();
        var button = new Button(ButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
        table.Add(button).SetMinWidth(100).SetMinHeight(30);
    }
}
