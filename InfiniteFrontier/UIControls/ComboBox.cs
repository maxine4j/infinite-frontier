
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class ComboBox : IFormComponent
    {
        /// <summary>
        /// Main button that opens the combo box
        /// </summary>
        public Button MainButton { get; set; }
        
        /// <summary>
        /// List of Items in the combo box
        /// </summary>
        public List<IListItem> Items { get; set; }

        private Transform _transform;
        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get { return _transform; } set { _transform = value; UpdateListTransforms(); } }
        
        private int _selected;
        /// <summary>
        /// The selected button index
        /// </summary>
        public int SelectedIndex { get { return _selected; } set { _selected = value; UpdateText(); } }
        
        /// <summary>
        /// The selected button
        /// </summary>
        public IListItem Selected { get { return Items[SelectedIndex]; } }
        
        /// <summary>
        /// If the combo box is open
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// The prefix of the text displayed on the combo box
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Font used to render main button text
        /// </summary>
        public Font HeaderFont { get; set; }


        /// <summary>
        /// Font used to render item text
        /// </summary>
        public Font ItemFont { get; set; }


        private Sprite mainButtonSprite, listButtonSprite;

        /// <summary>
        /// Creates a combo box
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="prefix"></param>
        /// <param name="items"></param>
        /// <param name="mainButtonSprite"></param>
        /// <param name="listButtonSprite"></param>
        public ComboBox(Transform transform, string prefix = "", List<IListItem> items = null, Sprite mainButtonSprite = null, Sprite listButtonSprite = null)
        {
            if (mainButtonSprite == null) mainButtonSprite = comboBoxButton;
            if (listButtonSprite == null) listButtonSprite = comboBoxButton;

            HeaderFont = Font.SmallButton;
            ItemFont = Font.SmallButton;
            this.mainButtonSprite = mainButtonSprite;
            this.listButtonSprite = listButtonSprite;
            Transform = transform;
            SelectedIndex = 0;
            Prefix = prefix;
            Items = items;
            MainButton = new Button(Transform, Prefix + items[SelectedIndex], mainButtonSprite, true);
            MainButton.Font = HeaderFont;
            UpdateListTransforms();
        }

        #region Helpers
        private void UpdateListTransforms()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Button = new Button(new Transform(MainButton.Transform.Translation.X, MainButton.Transform.Translation.Y + MainButton.Transform.Bounds.Height * (i + 1), MainButton.Transform.Bounds.Width, MainButton.Transform.Bounds.Height), Items[i].Text, listButtonSprite, true);
                    Items[i].Button.Font = ItemFont;
                }
            }
        }

        private void UpdateText()
        {
            if (Items != null)
                MainButton.Text = Prefix + Selected.Text;
        }
        #endregion

        /// <summary>
        /// Updates the combo box
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            MainButton.Update(lastKeyboardState, lastMouseState, gameTime, scale, origin);
            if (MainButton.IsLeftClicked)
                Open = !Open;
            if (Open)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Button.Update(lastKeyboardState, lastMouseState, gameTime, scale, origin);
                    if (Items[i].Button.IsLeftClicked)
                    {
                        SelectedIndex = i;
                        Open = false;
                        UpdateText();
                    }
                }
            }
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
            MainButton.Draw(sb, scale, colour, origin);
            if (Open)
            {
                foreach (IListItem l in Items)
                {
                    l.Button.Draw(sb, scale, colour, origin);
                }
            }
        }
    }
}
