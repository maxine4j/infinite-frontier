using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class ProgressBar : IFormComponent
    {
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Maximum value of the progress bar
        /// </summary>
        public float Max { get; set; }

        /// <summary>
        /// The current value of the progress bar
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// The bar sprite
        /// </summary>
        public Sprite BarSprite { get; set; }

        /// <summary>
        /// The bar's backing sprite
        /// </summary>
        public Sprite BackSprite { get; set; }

        /// <summary>
        /// Creates a new progress bar
        /// </summary>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <param name="bartexture"></param>
        /// <param name="backtexture"></param>
        public ProgressBar(Transform transform, Sprite backSprite = null, Sprite barSprite = null, float max = 1f, float value = 0f)
        {
            if (backSprite == null) backSprite = progressBarBack;
            if (barSprite == null) barSprite = progressBarFill;

            Transform = transform;
            BackSprite = backSprite;
            BarSprite = barSprite;
            Max = max;
            Value = value;
        }

        /// <summary>
        /// Updates the state of the component
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            if (Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                return true;
            return false;
        }

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the component with </param>
        /// <param name="scale"> Scale to draw the component by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the component's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the component with, defaults to white </param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Color.DarkTurquoise;
            if (origin == null) origin = Vector2.Zero;

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            BackSprite.DrawNineCut(sb, t, null, colour);
            if (Max != 0 && Value != 0)
            {
                t = new Transform(Transform.Translation.X + origin.Value.X + 1,
                    Transform.Translation.Y + origin.Value.Y + 1, 
                    Value * Transform.Size.X / Max - 2, 
                    Transform.Size.Y - 3);
                BarSprite.DrawNineCut(sb, t, null, colour);
            }
        }
    }
}
