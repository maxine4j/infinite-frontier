using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Arwic.InfiniteFrontier
{
    public class TextUtil
    {
        public static List<string> FlowString(string s, float width, SpriteFont font)
        {
            List<string> lines = new List<string>();

            float stringLen = font.MeasureString(s).X;
            float numLines = stringLen / width;

            if (numLines < 1f)
            {
                lines.Add(s);
                return lines;
            }

            float avgCharWidth = stringLen / s.Length;
            float charPerLine = width / avgCharWidth;

            for (int i = 0; i < numLines; i++)
            {
                string line = "";
                for (int j = 0; j < charPerLine - 1; j++)
                {
                    int index = j + Convert.ToInt32(Math.Floor(charPerLine)) * i;
                    if (index < s.Length) line += s[index];
                }
                lines.Add(line);
            }
            return lines;
        }

    }
}
