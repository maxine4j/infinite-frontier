using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class Transform
    {
        public Vector2 Translation { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 RotationOrigin { get; set; }
        public SpriteEffects Flip { get; set; }
        public Rectangle Bounds { get { return new Rectangle((int)Translation.X, (int)Translation.Y, (int)Size.X, (int)Size.Y); } }
         

        /// <summary>
        /// Creates a transform
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="flip"></param>
        public Transform(float x = 0, float y = 0, float w = 0, float h = 0, float rotation = 0f, Vector2? rotOrigin = null, Vector2? scale = null, SpriteEffects flip = SpriteEffects.None)
        {
            Translation = new Vector2(x, y);

            Size = new Vector2(w, h);

            Rotation = rotation;

            if (rotOrigin == null) RotationOrigin = new Vector2(0, 0); //new Vector2(result.Size.X / 2, result.Size.Y / 2);
            else RotationOrigin = rotOrigin.Value;

            if (scale == null) scale = new Vector2(1f, 1f);
            else Scale = scale.Value;

            Flip = flip;
        }

        public bool Contains(Vector2 point, float scale = 1f, Vector2? origin = null)
        {
            // TODO make this work with rotations
            if (origin == null) origin = Vector2.Zero;
            if (new Rectangle((int)((Bounds.X + origin.Value.X) * scale), (int)((Bounds.Y + origin.Value.Y) * scale), (int)(Bounds.Width * scale), (int)(Bounds.Height * scale)).Contains(point))
                return true;
            return false;
        }

        public bool Contains(Rectangle rect, float scale = 1f, Vector2? origin = null)
        {
            if (origin == null) origin = Vector2.Zero;
            if (new Rectangle((int)((Bounds.X + origin.Value.X) * scale), (int)((Bounds.Y + origin.Value.Y) * scale), (int)(Bounds.Width * scale), (int)(Bounds.Height * scale)).Contains(rect))
                return true;
            return false;
        }

        public bool Contains(Point point, float scale = 1f, Vector2? origin = null)
        {
            if (origin == null) origin = Vector2.Zero;
            if (new Rectangle((int)((Bounds.X + origin.Value.X) * scale), (int)((Bounds.Y + origin.Value.Y) * scale), (int)(Bounds.Width * scale), (int)(Bounds.Height * scale)).Contains(point))
                return true;
            return false;
        }
    }
}
