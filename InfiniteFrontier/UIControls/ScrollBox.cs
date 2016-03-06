using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class ScrollBox : IFormComponent
    {
        /// <summary>
        /// Transform
        /// </summary>
        private Transform _transform;
        public Transform Transform { get { return _transform; } set { _transform = value; InitItems(); } }

        /// <summary>
        /// List of items
        /// </summary>
        public List<IListItem> Items { get; set; }

        /// <summary>
        /// The index of the selected item
        /// </summary>
        public int Selected { get; set; }

        /// <summary>
        /// If the scroll box is selectable
        /// </summary>
        public bool Selectable { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The selected item
        /// </summary>
        public IListItem SelectedItem
        {
            get
            {
                if (Selected == -1) return null;
                if (Selected >= Items.Count) return null;
                return Items[Selected];
            }
        }

        /// <summary>
        /// The background sprite
        /// </summary>
        public Sprite BackgroundSprite { get; set; }

        /// <summary>
        /// The highlight sprite
        /// </summary>
        public Sprite HighLightSprite { get; set; }
        
        /// <summary>
        /// The sprite of each list item
        /// </summary>
        
        public Sprite ButtonSprite { get; set; }

        /// <summary>
        /// The sprite of each list item
        /// </summary>

        public Sprite ScrubberSprite { get; set; }

        /// <summary>
        /// The width of the scroll bar
        /// </summary>
        public int ScrollBarWidth { get; set; }

        /// <summary>
        /// The height of each item
        /// </summary>
        public int ItemHeight { get; set; }

        /// <summary>
        /// The index of the item that will appear at the top of the scroll box
        /// </summary>
        public int ScrollIndex { get; set; }

        private int lastScrollIndex;

        public ScrollBox(Transform transform, int itemHeight, List<IListItem> items, Sprite listButtonSprite = null, Sprite backgroundSprite = null, Sprite highlightSprite = null, Sprite scrubberSprite = null)
        {
            if (listButtonSprite == null) listButtonSprite = scrollBoxButton;
            if (backgroundSprite == null) backgroundSprite = scrollBoxBack;
            if (highlightSprite == null) highlightSprite = scrollBoxHighlight;
            if (scrubberSprite == null) scrubberSprite = scrollBoxScrubber;

            Font = Font.InfoText;
            Items = items;
            _transform = transform;
            ItemHeight = itemHeight;
            Selected = -1;
            BackgroundSprite = backgroundSprite;
            HighLightSprite = highlightSprite;
            ButtonSprite = listButtonSprite;
            ScrubberSprite = scrubberSprite;
            Selectable = true;
            ScrollIndex = 0;
            lastScrollIndex = ScrollIndex;
            ScrollBarWidth = 15;
            InitItems();
        }

        private void InitItems()
        {
            for (int i = ScrollIndex; i < Items.Count; i++)
			{
                Transform transform = new Transform(Transform.Translation.X, Transform.Translation.Y + (i - ScrollIndex) * ItemHeight, Transform.Bounds.Width - ScrollBarWidth, ItemHeight);
                Items[i].Button = new Button(transform, Items[i].Text, ButtonSprite, true);
                Items[i].Button.Font = Font;
			}
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
            {
                if (InputUtil.WasKeyDown(Keys.PageUp) || lastMouseState.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                    ScrollIndex--;
                if (InputUtil.WasKeyDown(Keys.PageDown) || lastMouseState.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                    ScrollIndex++;
                if (ScrollIndex >= Items.Count - (int)Math.Floor((float)(Transform.Bounds.Height / ItemHeight)))
                    ScrollIndex = Items.Count - (int)Math.Floor((float)(Transform.Bounds.Height / ItemHeight));
                if (ScrollIndex < 0)
                    ScrollIndex = 0;
                if (lastScrollIndex != ScrollIndex)
                    InitItems();
            }
            if (Selectable)
            {
                if (origin == null) origin = Vector2.Zero;
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Button.Update(lastKeyboardState, lastMouseState, gameTime, scale, origin.Value);
                    if (Items[i].Button.IsLeftClicked)
                    {
                        if (SelectedItem != null) SelectedItem.Button.Sprite = ButtonSprite;
                        Selected = i;
                    }
                    if (i == Selected)
                    {
                        SelectedItem.Button.Sprite = HighLightSprite;
                    }
                }
            }

            lastScrollIndex = ScrollIndex;

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
            if (colour == null) colour = Color.White;
            if (origin == null) origin = Vector2.Zero;

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            BackgroundSprite.DrawNineCut(sb, t, null, colour);

            float scrubberPos = ((float)ScrollIndex / (Items.Count == 0 ? 1f : (float)Items.Count)) * (float)t.Bounds.Height;
            float scrubberHeight = t.Bounds.Height * (t.Bounds.Height / (ItemHeight * (Items.Count == t.Bounds.Height ? float.MaxValue : (float)Items.Count)));
            if (scrubberHeight > t.Bounds.Height)
                scrubberHeight = t.Bounds.Height;
            Transform scrubberTransform = new Transform(
                t.Translation.X + t.Bounds.Width - ScrollBarWidth, t.Translation.Y + scrubberPos,
                ScrollBarWidth, scrubberHeight);
            ScrubberSprite.DrawNineCut(sb, scrubberTransform);

            for (int i = ScrollIndex; i < Math.Min(ScrollIndex + Math.Floor((float)(t.Bounds.Height / ItemHeight)), Items.Count); i++)            
            {
                Items[i].Draw(sb, fonts[(int)Font], scale, Color.White, origin);
            }
        }
    }
}
