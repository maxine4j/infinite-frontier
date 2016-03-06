
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class Image : IFormComponent
    {
        /// <summary>
        /// The Sprite
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Colour of the image
        /// </summary>
        public Color? Colour { get; set; }

        /// <summary>
        /// The section of the image to draw
        /// </summary>
        public Rectangle? Source { get; set; }

        public bool NineCut { get; set; }
        public int? NineCutDestEdge { get; set; }
        public int? NineCutSourceEdge { get; set; }


        /// <summary>
        /// Creates a new Image
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="pos"></param>
        /// <param name="c"></param>
        /// <param name="source"></param>
        public Image(Sprite sprite, Transform transform, Rectangle? source = null, Color? colour = null, bool nineCut = false, int? nineCutDestEdge = null, int? nineCutSourceEdge = null)
        {
            Sprite = sprite;
            Transform = transform;
            if (source == null) Source = new Rectangle(0, 0, sprite.Width, sprite.Height);
            if (colour == null) Colour = Color.White;
            else Colour = colour.Value;
            NineCut = nineCut;
            NineCutDestEdge = nineCutDestEdge;
            NineCutSourceEdge = nineCutSourceEdge;
        }

        /// <summary>
        /// This method does nothing, it only exists for compatibility
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null)
        {
            return false;
        }

        /// <summary>
        /// Draws the Image
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="scale"></param>
        /// <param name="colour"></param>
        /// <param name="origin"></param>
        public void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null)
        {
            if (colour == null) colour = Colour;
            if (origin == null) origin = Vector2.Zero;

            Transform t = new Transform(Transform.Translation.X + origin.Value.X, Transform.Translation.Y + origin.Value.Y, Transform.Bounds.Width, Transform.Bounds.Height, Transform.Rotation, Transform.RotationOrigin, Transform.Scale, Transform.Flip);

            if (!NineCut) Sprite.Draw(sb, t, Source, colour);
            else Sprite.DrawNineCut(sb, t, Source, colour, NineCutDestEdge, NineCutSourceEdge);
        }
    }
}
