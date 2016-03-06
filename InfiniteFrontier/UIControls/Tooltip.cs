using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    class Tooltip : IFormComponent
    {
        public Transform Transform { get; set; }
        public IFormComponent Host { get; set; }
        public Label Title { get; set; }
        public bool FollowCurosr { get; set; }
        public Vector2 CursorOffset { get; set; }

        /// <summary>
        /// Font used to render text
        /// </summary>
        public Font Font { get; set; }

        private Form form;
        
        public Tooltip(IFormComponent host)
        {
            Host = host;
            form = new Form(new Transform(host.Transform.Translation.X + host.Transform.Bounds.Width, host.Transform.Translation.Y + host.Transform.Bounds.Height), formBack);
            form.Hidden = true;
            FollowCurosr = false;
            CursorOffset = new Vector2(20, 30);
        }


        public void UpdateContent(Font font, string text, int textWidth, int linePadding = 5, int borderPaddingX = 10, int borderPaddingY = 10)
        {
            this.Font = font;
            List<string> lines = TextUtil.FlowString(text, textWidth, fonts[(int)Font]);
            form.RemoveAllComponents();
            if (lines.Count == 0)
                lines.Add("");
            float lineHeight = fonts[(int)Font].MeasureString(lines[0]).Y;
            float lineWidth = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                form.AddComponent(new Label(new Transform(borderPaddingX, i * (lineHeight + linePadding) + borderPaddingY), lines[i]) { Font = Font });
                float currentWidth = fonts[(int)Font].MeasureString(lines[i]).X;
                if (lineWidth < currentWidth)
                    lineWidth = currentWidth;
            }

            // Update form size
            form.Transform.Size = new Vector2(borderPaddingX * 2 + lineWidth, borderPaddingY * 2 + (lineHeight + linePadding) * lines.Count);
        }

        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            Transform t = new Transform(Host.Transform.Translation.X + origin.Value.X, Host.Transform.Translation.Y + origin.Value.Y, Host.Transform.Bounds.Width, Host.Transform.Bounds.Height, Host.Transform.Rotation, Host.Transform.RotationOrigin, Host.Transform.Scale, Host.Transform.Flip);

            if (t.Contains(InputUtil.MouseScreenPos))
            {
                form.Hidden = false;
                if (FollowCurosr)
                    form.Transform.Translation = InputUtil.MouseScreenPos + CursorOffset;
            }
            else
            {
                form.Hidden = true;
            }
            return false;
        }

        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            form.Draw(sb, scale);
        }
    }
}
