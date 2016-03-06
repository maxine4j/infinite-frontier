using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Arwic.InfiniteFrontier
{
    public class TextLog : IFormComponent
    {
        public List<string> Lines { get; set; }
        public Transform Transform { get; set; }
        public int LinesToDraw { get; set; }
        public Font Font { get; set; }
        public int LinesToKeep { get; set; }
        public int LineSpacing { get; set; }

        public TextLog(Transform transform, int linesToDraw = -1, int linesToKeep = 500)
        {
            LineSpacing = 2;
            Font = Font.InfoText;
            Lines = new List<string>();
            LinesToDraw = linesToDraw;
            for (int i = 0; i < (linesToDraw == -1 ? 10 : linesToDraw); i++)
            {
                Lines.Add(" ");
            }
            Transform = transform;
            LinesToKeep = linesToKeep;
        }

        public void Write(string s)
        {
            Lines[Lines.Count - 1] += s;
        }

        public void WriteLine(string line)
        {
            Lines.Add(line);
            if (Lines.Count > LinesToKeep)
            {
                Lines.RemoveAt(0);
            }
        }

        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            return false;
        }

        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Color.White;
            if (origin == null) origin = Vector2.Zero;

            // if in dynamic mode, calculate lines to draw based on font size and transform
            int linesToDraw = LinesToDraw;
            float lineHeight = Main.fonts[(int)Font].MeasureString("|").Y;
            if (linesToDraw == -1)
                linesToDraw = Convert.ToInt32(Math.Floor(Transform.Bounds.Height / (lineHeight + LineSpacing)));
            //Main.console.Write($"lineHeight={lineHeight}; linesToDraw={linesToDraw}, Transform.Bounds.Height={Transform.Bounds.Height}", MsgType.Debug);

            // draws the text
            for (int i = 0; i < linesToDraw; i++)
            {
                int index = Lines.Count - 1 - i;
                if (index < Lines.Count && index >= 0)
                    sb.DrawString(Main.fonts[(int)Font], Lines[index], new Vector2(Transform.Translation.X, Transform.Translation.Y + (linesToDraw - i - 1) * (lineHeight + LineSpacing)), Color.White);
                else
                    break;
            }
        }
    }
}
