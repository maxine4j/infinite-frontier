using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class SpinButton : IFormComponent
    {
        /// <summary>
        /// List of Items
        /// </summary>
        public List<IListItem> Items { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The selected item
        /// </summary>
        public IListItem SelectedItem { get { return Items[SelectedIndex]; } }

        private Transform _transform;
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get { return _transform; } set { _transform = value; UpdateButtons(); } }

        /// <summary>
        /// The selected index
        /// </summary>
        public int SelectedIndex { get; set; }

        private Button btnForward, btnBackward;

        /// <summary>
        /// Creates a new spin button
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="items"></param>
        /// <param name="buttonTexture"></param>
        public SpinButton(Transform transform, List<IListItem> items, Sprite buttonTexture = null)
        {
            if (buttonTexture == null) buttonTexture = button;

            Font = Font.InfoText;
            Transform = transform;
            btnBackward = new Button(new Transform(Transform.Translation.X - 50, Transform.Translation.Y - 10, 50, 50), "<", buttonTexture, true);
            btnForward = new Button(new Transform(Transform.Translation.X + 150 + 50, Transform.Translation.Y - 10, 50, 50), ">", buttonTexture, true);
            Items = items;
        }

        #region helpers
        private void UpdateButtons()
        {
            btnBackward.Transform.Translation = new Vector2(Transform.Translation.X - 50, Transform.Translation.Y - 10);
            btnForward.Transform.Translation = new Vector2(Transform.Translation.X + 150 + 50, Transform.Translation.Y - 10);
        }
        #endregion

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
            btnBackward.Update(lastKeyboardState, lastMouseState, gameTime, scale, origin);
            btnForward.Update(lastKeyboardState, lastMouseState, gameTime, scale, origin);

            if (btnBackward.IsLeftClicked)
            {
                SelectedIndex--;
                if (SelectedIndex == -1)
                {
                    SelectedIndex = Items.Count - 1;
                }
            }
            if (btnForward.IsLeftClicked)
            {
                SelectedIndex++;
                if (SelectedIndex == Items.Count)
                {
                    SelectedIndex = 0;
                }
            }
            if (btnForward.Transform.Contains(InputUtil.MouseScreenPos, scale, origin) || btnBackward.Transform.Contains(InputUtil.MouseScreenPos, scale, origin))
                return true;
            return false;
        }

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the component with </param>
        /// <param name="font"> Font to draw the component's text with </param>
        /// <param name="scale"> Scale to draw the component by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the component's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the component with, defaults to white </param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            btnBackward.Draw(sb, scale, colour, origin);
            btnForward.Draw(sb, scale, colour, origin);
            sb.DrawString(fonts[(int)Font], SelectedItem.Text, new Vector2(Transform.Translation.X + 20 + origin.Value.X, Transform.Translation.Y + origin.Value.Y), Color.White);
        }
    }
}
