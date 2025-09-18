using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace Stg.Nez.Scenes;

public class Interface : Scene
{
    public override void Initialize()
    {
        base.Initialize();
        var uiEntity = CreateEntity("UI").AddComponent(new UICanvas());
        var table = uiEntity.Stage.AddElement(new Table());

        // tell the table to fill all the available space. In this case that would be the entire screen.
        table.SetFillParent(true);

        // add a ProgressBar
        var bar = new ProgressBar(
            0,
            1,
            0.1f,
            false,
            ProgressBarStyle.Create(Color.Black, Color.White)
        );
        table.Add(bar);

        // this tells the table to move on to the next row
        table.Row();

        // add a Slider
        var slider = new Slider(
            0,
            1,
            0.1f,
            false,
            SliderStyle.Create(Color.DarkGray, Color.LightYellow)
        );
        table.Add(slider);
        table.Row();

        // if creating buttons with just colors (PrimitiveDrawables) it is important to explicitly set the minimum size since the colored textures created
        // are only 1x1 pixels
        var button = new Button(ButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
        table.Add(button).SetMinWidth(100).SetMinHeight(30);
    }
}
