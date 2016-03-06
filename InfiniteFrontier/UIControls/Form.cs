using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Form
    {
        /// <summary>
        /// The origin of the form
        /// </summary>
        public Vector2 Origin { get { return new Vector2(Transform.Translation.X, Transform.Translation.Y); } }

        /// <summary>
        /// Transform of the form
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// The title of the form
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Font used to render title text
        /// </summary>
        public Font TitleFont { get; set; }

        /// <summary>
        /// If the form is currently hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Background of the form
        /// </summary>
        public Sprite Background { get; set; }

        /// <summary>
        /// The forms close button
        /// </summary>
        public Button CloseButton { get; set; }

        /// <summary>
        /// If the form's close button is enabled
        /// </summary>
        public bool CloseButtonEnabled { get; private set; }

        /// <summary>
        /// If the form is draggable
        /// </summary>
        public bool Draggable { get; set; }

        /// <summary>
        /// The forms hotkey
        /// </summary>
        public Keys? HotKey { get; set; }

        private List<IFormComponent> children;
        private bool held;
        private Vector2 dragOffset;

        /// <summary>
        /// Creates a new form
        /// </summary>
        /// <param name="title"></param>
        /// <param name="hotKey"></param>
        /// <param name="pos"></param>
        /// <param name="closeButton"></param>
        /// <param name="dragable"></param>
        /// <param name="background"></param>
        /// <param name="closeButtonTex"></param>
        public Form(Transform transform, Sprite backgroundSprite = null, string title = "", bool dragable = false, bool closeButton = false, Sprite closeButtonSprite = null, Keys? hotKey = null)
        {
            if (backgroundSprite == null) backgroundSprite = formBack;
            TitleFont = Font.InfoHeader;
            children = new List<IFormComponent>();
            Background = backgroundSprite;
            HotKey = hotKey;
            Transform = transform;
            Title = title;
            CloseButtonEnabled = closeButton;
            Draggable = dragable;
            if (CloseButtonEnabled)
            {
                CloseButton = new Button(new Transform(Transform.Bounds.Width - 35, 5, 30, 30), "", closeButtonSprite);
                AddComponent(CloseButton);
            }
        }

        /// <summary>
        /// Updates the form and all of its children
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        public void Update(GameTime gameTime)
        {
            if (HotKey != null && lastKeyboardState.IsKeyDown(HotKey.Value) && Keyboard.GetState().IsKeyUp(HotKey.Value))
                Hidden = !Hidden;

            foreach (IFormComponent fc in children)
                if (fc != null && fc.Update(lastKeyboardState, lastMouseState, gameTime, scale, Origin))
                    break;

            if (Draggable)
            {
                if (Transform.Contains(InputUtil.MouseScreenPos, scale, null))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && held)
                    {
                        held = true;
                        dragOffset = new Vector2(InputUtil.MouseScreenPos.X / scale - Transform.Translation.X, InputUtil.MouseScreenPos.Y / scale - Transform.Translation.Y);
                    }
                }
                if (held)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                        held = false;
                    Transform.Translation = new Vector2(Mouse.GetState().X / scale - dragOffset.X, Mouse.GetState().Y / scale - dragOffset.Y);
                }
            }
            
            if (CloseButtonEnabled)
            {
                CloseButton.Update(lastKeyboardState, lastMouseState, gameTime, scale, Origin);
                if (CloseButton.IsLeftClicked)
                    Hidden = true;
            }
        }

        /// <summary>
        /// Draws the form and all of its children
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        public void Draw(SpriteBatch sb, float scale, bool drawChildren = true)
        {
            if (!Hidden)
            {
                Background.DrawNineCut(sb, Transform);
                sb.DrawString(fonts[(int)TitleFont], Title, new Vector2(Transform.Translation.X + Transform.Size.X / 2 - fonts[(int)TitleFont].MeasureString(Title).X / 2, Transform.Translation.Y), Color.White);
                foreach (IFormComponent c in children)
                {
                    if (c != null) c.Draw(sb, scale, null, Origin);
                }
            }
        }

        public void RemoveAllComponents()
        {
            children.Clear();
            AddComponent(CloseButton);
        }

        public void AddComponent(IFormComponent c)
        {
            children.Add(c);
        }

        public void AddComponents(IFormComponent[] c)
        {
            children.AddRange(c);
        }
    }
}
