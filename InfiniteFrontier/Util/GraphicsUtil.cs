using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class GraphicsUtil
    {
        public static Texture2D UtilPixel { get; set; }

        public static void Initialise(GraphicsDevice gd)
        {
            UtilPixel = new Texture2D(gd, 1, 1); // make a new 1x1 texture for our line
            Color[] pixels = { Color.White };
            UtilPixel.SetData(pixels); // set its colour to white
        }

        public static float Angle(Vector2 v1, Vector2 v2)
        {
            double dot = v1.X * v2.X + v1.Y * v2.Y;
            double det = v1.X * v2.Y - v1.Y * v2.X;
            double angle = Math.Atan2(det, dot);
            // Math.Acos(Vector2.Dot(v1, v2) / (v1.Length() * v2.Length()))
            return Convert.ToSingle(angle);
        }

        public static void DrawLine(SpriteBatch sb, Vector2 p1, Vector2 p2, int thickness, Color c)
        {
            float length = Vector2.Distance(p1, p2); // calc length of line
            length += 1;
            float rotation = GetRotation(p1, p2); // calc rotation of line
            sb.Draw(UtilPixel, new Rectangle((int)p1.X - thickness / 2, (int)p1.Y - thickness / 2, (int)length, thickness), new Rectangle(0, 0, 1, 1), c, rotation, Vector2.Zero, SpriteEffects.None, 0.0f); // draw line
        }

        private static float GetRotation(Vector2 p1, Vector2 p2)
        {
            float adj = p1.X - p2.X;
            float opp = p1.Y - p2.Y;
            float tan = opp / adj;
            float res = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            res = (res - 180) % 360;
            if (res < 0) { res += 360; }
            res = MathHelper.ToRadians(res);
            return res;
        }

        public static void DrawCircle(SpriteBatch sb, Vector2 centre, int radius, int smoothness, int thickness, Color c)
        {
            double segmentAngle = 2 * Math.PI / smoothness;
            Vector2 last = centre + new Vector2(0, radius);
            for (int i = 0; i < smoothness + 1; i++)
            {
                Vector2 next = centre + new Vector2((float)Math.Sin(segmentAngle * i) * radius, (float)Math.Cos(segmentAngle * i) * radius);
                DrawLine(sb, last, next, thickness, c);
                last = next;
            }
        }

        public static void DrawRect(SpriteBatch sb, Rectangle rect, int thickness, Color c)
        {
            GraphicsUtil.DrawLine(sb, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), thickness, c);
            GraphicsUtil.DrawLine(sb, new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height), thickness, c);
            GraphicsUtil.DrawLine(sb, new Vector2(rect.X - thickness, rect.Y + rect.Height + 1), new Vector2(rect.X + rect.Width - thickness, rect.Y + rect.Height + 1), thickness, c);
            GraphicsUtil.DrawLine(sb, new Vector2(rect.X + rect.Width + 1, rect.Y + thickness), new Vector2(rect.X + rect.Width + 1, rect.Y + rect.Height + thickness), thickness, c);
        }
    }
}
