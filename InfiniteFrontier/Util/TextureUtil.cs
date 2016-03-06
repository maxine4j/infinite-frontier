using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class TextureUtil
    {
        public static void DrawTextureNineCut(SpriteBatch s, Texture2D texture, Color c, int destEdge, int sourceEdge, Rectangle pos)
        {
            int destEdgeX = Math.Min(destEdge, pos.Width);
            int destEdgeY = Math.Min(destEdge, pos.Height);

            // top
            Rectangle r_topLeftDest = new Rectangle(pos.X, pos.Y, destEdgeX, destEdgeY);
            Rectangle r_topLeftSource = new Rectangle(0, 0, sourceEdge, sourceEdge);
            s.Draw(texture, r_topLeftDest, r_topLeftSource, c);
            Rectangle r_topRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y, destEdgeX, destEdgeY);
            Rectangle r_topRightSource = new Rectangle(2 * sourceEdge, 0, sourceEdge, sourceEdge);
            s.Draw(texture, r_topRightDest, r_topRightSource, c);
            Rectangle r_topMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y, pos.Width - destEdgeX * 2, destEdgeY);
            Rectangle r_topMiddleSource = new Rectangle(sourceEdge, 0, sourceEdge, sourceEdge);
            s.Draw(texture, r_topMiddleDest, r_topMiddleSource, c);

            // bottom
            Rectangle r_bottomLeftDest = new Rectangle(pos.X, pos.Y + pos.Height - destEdgeY, destEdgeX, destEdgeY);
            Rectangle r_bottomLeftSource = new Rectangle(0, 2 * sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_bottomLeftDest, r_bottomLeftSource, c);
            Rectangle r_bottomRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y + pos.Height - destEdgeY, destEdgeX, destEdgeY);
            Rectangle r_bottomRightSource = new Rectangle(2 * sourceEdge, 2 * sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_bottomRightDest, r_bottomRightSource, c);
            Rectangle r_bottomMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y + pos.Height - destEdgeY, pos.Width - destEdgeX * 2, destEdgeY);
            Rectangle r_bottomMiddleSource = new Rectangle(sourceEdge, 2 * sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_bottomMiddleDest, r_bottomMiddleSource, c);

            // middle
            Rectangle r_centreLeftDest = new Rectangle(pos.X, pos.Y + destEdgeY, destEdgeX, pos.Height - destEdgeY * 2);
            Rectangle r_centreLeftSource = new Rectangle(0, sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_centreLeftDest, r_centreLeftSource, c);
            Rectangle r_centreRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y + destEdgeY, destEdgeX, pos.Height - destEdgeY * 2);
            Rectangle r_centreRightSource = new Rectangle(2 * sourceEdge, sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_centreRightDest, r_centreRightSource, c);
            Rectangle r_centreMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y + destEdgeY, pos.Width - destEdgeX * 2, pos.Height - destEdgeY * 2);
            Rectangle r_centreMiddleSource = new Rectangle(sourceEdge, sourceEdge, sourceEdge, sourceEdge);
            s.Draw(texture, r_centreMiddleDest, r_centreMiddleSource, c);
        }

        public static void DrawTextureNineCut(SpriteBatch s, Texture2D texture, Color c, int destEdge, int sourceEdge, Rectangle pos, Rectangle src)
        {
            int destEdgeX = Math.Min(destEdge, pos.Width);
            int destEdgeY = Math.Min(destEdge, pos.Height);

            // top
            Rectangle r_topLeftDest = new Rectangle(pos.X, pos.Y, destEdgeX, destEdgeY);
            Rectangle r_topLeftSource = new Rectangle(src.X, src.Y, sourceEdge, sourceEdge);
            s.Draw(texture, r_topLeftDest, r_topLeftSource, c);
            Rectangle r_topRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y, destEdgeX, destEdgeY);
            Rectangle r_topRightSource = new Rectangle(src.X + 2 * sourceEdge, src.Y, sourceEdge , sourceEdge );
            s.Draw(texture, r_topRightDest, r_topRightSource, c);
            Rectangle r_topMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y, pos.Width - destEdgeX * 2, destEdgeY);
            Rectangle r_topMiddleSource = new Rectangle(src.X + sourceEdge, src.Y, sourceEdge , sourceEdge );
            s.Draw(texture, r_topMiddleDest, r_topMiddleSource, c);

            // bottom
            Rectangle r_bottomLeftDest = new Rectangle(pos.X, pos.Y + pos.Height - destEdgeY, destEdgeX, destEdgeY);
            Rectangle r_bottomLeftSource = new Rectangle(src.X, src.Y + 2 * sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_bottomLeftDest, r_bottomLeftSource, c);
            Rectangle r_bottomRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y + pos.Height - destEdgeY, destEdgeX, destEdgeY);
            Rectangle r_bottomRightSource = new Rectangle(src.X + 2 * sourceEdge, src.Y + 2 * sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_bottomRightDest, r_bottomRightSource, c);
            Rectangle r_bottomMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y + pos.Height - destEdgeY, pos.Width - destEdgeX * 2, destEdgeY);
            Rectangle r_bottomMiddleSource = new Rectangle(src.X + sourceEdge, src.Y + 2 * sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_bottomMiddleDest, r_bottomMiddleSource, c);

            // middle
            Rectangle r_centreLeftDest = new Rectangle(pos.X, pos.Y + destEdgeY, destEdgeX, pos.Height - destEdgeY * 2);
            Rectangle r_centreLeftSource = new Rectangle(src.X, src.Y + sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_centreLeftDest, r_centreLeftSource, c);
            Rectangle r_centreRightDest = new Rectangle(pos.X + pos.Width - destEdgeX, pos.Y + destEdgeY, destEdgeX, pos.Height - destEdgeY * 2);
            Rectangle r_centreRightSource = new Rectangle(src.X + 2 * sourceEdge, src.Y + sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_centreRightDest, r_centreRightSource, c);
            Rectangle r_centreMiddleDest = new Rectangle(pos.X + destEdgeX, pos.Y + destEdgeY, pos.Width - destEdgeX * 2, pos.Height - destEdgeY * 2);
            Rectangle r_centreMiddleSource = new Rectangle(src.X + sourceEdge, src.Y + sourceEdge, sourceEdge , sourceEdge);
            s.Draw(texture, r_centreMiddleDest, r_centreMiddleSource, c);
        }
    }
}
