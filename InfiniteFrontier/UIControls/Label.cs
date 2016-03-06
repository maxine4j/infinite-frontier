using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Label : IFormComponent
    {
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// The text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The colour of the label
        /// </summary>
        public Color Colour { get; set; }

        /// <summary>
        /// Creates a new label
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="text"></param>
        /// <param name="colour"></param>
        public Label(Transform transform, string text, Color? colour = null)
        {
            this.Font = Font.InfoLabel;
            Transform = transform;
            Text = text;
            if (colour == null) Colour = Color.White;
            else Colour = colour.Value;
        }

        /// <summary>
        /// Draws the label
        /// </summary>
        /// <param name="s"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Color.White;
            if (origin == null) origin = Vector2.Zero;
            sb.DrawString(fonts[(int)Font], Text, new Vector2(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y), Colour);
        }

        /// <summary>
        /// This method does nothing, it only exists for compatibility
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            return false;
        }
    }
}
