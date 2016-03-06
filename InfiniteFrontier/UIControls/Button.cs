using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Button : IFormComponent
    {
        public static readonly int DefaultBorder = 15;

        /// <summary>
        /// Key emulates a left clicka of the button
        /// </summary>
        public Keys? HotKey { get; set; }

        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Sprite
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// If the button is currently being left clicked
        /// </summary>
        public bool IsLeftClicked { get; private set; }

        /// <summary>
        /// If the button is currently being right clicked
        /// </summary>
        public bool IsRightClicked { get; private set; }

        /// <summary>
        /// If the button is currently being middle clicked
        /// </summary>
        public bool IsMiddleClicked { get; private set; }

        private int destBorderSize, sourceBorderSize;
        private bool nineSplit;
        
        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="transform"> tranformation ofthe button </param>
        /// <param name="text"> text to be rendered over the button sprite </param>
        /// <param name="tex"> sprite to be rendered </param>
        /// <param name="nineSplit"> whether the button should be rendered as is or using a nine split algorithm </param>
        /// <param name="destBorderSize"> nine split destination border </param>
        /// <param name="sourceBorderSize"> nine split source border, this should be a third of the width of the sprite </param>
        public Button(Transform transform, string text, Sprite sprite = null, bool nineSplit = false, int? destBorderSize = null, int? sourceBorderSize = null)
        {
            if (sprite == null) sprite = button;
            Font = Font.SmallButton;
            this.nineSplit = nineSplit;
            if (nineSplit)
            {
                if (destBorderSize == null) this.destBorderSize = DefaultBorder;
                else this.destBorderSize = destBorderSize.Value;
                if (sourceBorderSize == null) this.sourceBorderSize = sprite.Texture.Width / 3;
                else this.sourceBorderSize = sourceBorderSize.Value;
            }
            Transform = transform;
            Sprite = sprite;
            Text = text;
        }

        #region helpers
        private void CheckStatus(MouseState last, float scale, Vector2 origin)
        {
            if (last.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released && isOver(scale, origin))
                IsLeftClicked = true;
            else
                IsLeftClicked = false;

            if (last.RightButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Released && isOver(scale, origin))
                IsRightClicked = true;
            else
                IsRightClicked = false;

            if (last.MiddleButton == ButtonState.Pressed && Mouse.GetState().MiddleButton == ButtonState.Released && isOver(scale, origin))
                IsMiddleClicked = true;
            else
                IsMiddleClicked = false;
        }

        private bool isOver(float scale, Vector2 origin)
        {
            if (Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                return true;
            return false;
        }

        private bool isDown(float scale, Vector2 origin)
        {
            if (isOver(scale, origin) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                return true;
            return false;
        }
        #endregion

        /// <summary>
        /// Updates the state of the button
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            if (origin == null) origin = Vector2.Zero;

            CheckStatus(lastMouseState, scale, origin.Value);

            if (HotKey != null && Keyboard.GetState().IsKeyDown((Keys)HotKey))
                IsLeftClicked = true;

            if (Transform.Bounds.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }

        /// <summary>
        /// Draws the button
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the button with </param>
        /// <param name="scale"> Scale to draw the button by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the button's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the button with, defaults to white </param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (origin == null) origin = Vector2.Zero;
            if (colour == null) colour = Color.White;
            
            if (isDown(scale, origin.Value))
                colour = new Color(colour.Value.R - 80, colour.Value.G - 80, colour.Value.B - 80);
            else if (isOver(scale, origin.Value))
                colour = new Color(colour.Value.R - 40, colour.Value.G - 40, colour.Value.B - 40);

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            if (nineSplit)
                Sprite.DrawNineCut(sb, t, null, colour, destBorderSize, sourceBorderSize);
            else
                Sprite.Draw(sb, t, null, colour);

            // draw the text over the button texture
            sb.DrawString(fonts[(int)Font], Text, new Vector2(t.Translation.X + t.Bounds.Width / 2 - fonts[(int)Font].MeasureString(Text).X / 2, t.Translation.Y + t.Bounds.Height / 2 - fonts[(int)Font].MeasureString(Text).Y / 2), colour.Value);
        }
    }
}
