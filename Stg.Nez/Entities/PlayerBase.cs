using Nez;
using Nez.Sprites;

namespace Stg.Nez.Entities;

public class PlayerBase : Entity
{
    public float FireRate { get; set; }
    private float _lastFireTime;

    private SpriteAnimator spriteAnimator;

    public PlayerBase()
    {
        spriteAnimator = AddComponent<SpriteAnimator>();
    }

    public override void OnAddedToScene()
    {
        base.OnAddedToScene();
    }

    public override void Update()
    {
        base.Update();
    }
}
