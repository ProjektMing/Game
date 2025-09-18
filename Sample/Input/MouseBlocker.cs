using Microsoft.Xna.Framework.Input;
using Nez;

namespace Sample.Systems
{
    /// <summary>
    /// Global manager that blocks all mouse input by overwriting Nez.Input mouse states every frame.
    /// </summary>
    public sealed class MouseBlocker : GlobalManager
    {
        private static MouseState _neutralState =
            new(
                0,
                0,
                0,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released
            );

        public override void Update()
        {
            // Force both previous and current mouse states to a neutral, non-pressed state
            global::Nez.Input.SetPreviousMouseState(_neutralState);
            global::Nez.Input.SetCurrentMouseState(_neutralState);
        }
    }
}
