using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class CheckBox : IFormComponent
    {
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }
        
        /// <summary>
        /// Sprite when checkbox is true
        /// </summary>
        public Sprite SpriteTrue { get; set; }

        /// <summary>
        /// Sprite when checkbox is false
        /// </summary>
        public Sprite SpriteFalse { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// If the checkbox is true
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Creates a new CheckBox
        /// </summary>
        /// <param name="transform"> Tranform of the checkbox </param>
        /// <param name="text"> Text of the CheckBox </param>
        /// <param name="trueSprite"> Texture when the CheckBox is true </param>
        /// <param name="falseSprite"> Texture when the CheckBox is false </param>
        public CheckBox(Transform transform, string text, Sprite trueSprite = null, Sprite falseSprite = null)
        {
            if (trueSprite == null) trueSprite = checkBoxTrue;
            if (falseSprite == null) falseSprite = checkBoxFalse;

            Font = Font.SmallButton;
            Transform = transform;
            SpriteTrue = trueSprite;
            SpriteFalse = falseSprite;
            Text = text;
            Value = false;
        }

        /// <summary>
        /// Updates the CheckBox
        /// </summary>
        /// <param name="keyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            if (origin == null) origin = Vector2.Zero;

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            // check if it has been clicked
            if (InputUtil.WasLeftMouseButton &&
                Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                Value = !Value;

            if (Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                return true;
            return false;
        }

        /// <summary>
        /// Draws the check box
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the check box with </param>
        /// <param name="scale"> Scale to draw the check box by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the check box's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the check box with, defaults to white </param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Color.White;
            if (origin == null) origin = Vector2.Zero;

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            // draw the appropriate texture
            if (Value)
                SpriteTrue.Draw(sb, t, null, colour);
            else
                SpriteFalse.Draw(sb, t, null, colour);
            // draw the text
            sb.DrawString(fonts[(int)Font], Text, new Vector2(t.Translation.X + 70, t.Translation.Y + 10), Color.White);
        }
    }
}
