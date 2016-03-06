
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class Sprite : IDrawable
    {
        private enum SpriteType
        {
            Atlas,
            Texture
        }

        public Texture2D Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public SpriteAtlas Atlas { get; private set; }
        public int AtlasIndex { get; private set; }

        private SpriteType spriteType;

        /// <summary>
        /// Sprite
        /// </summary>
        /// <param name="cm"> Content Manager to load texture with </param>
        /// <param name="path"> Path to the required image file </param>
        public Sprite(ContentManager cm, string path)
        {
            spriteType = SpriteType.Texture;
            Texture = Loader.LoadTexture(cm, path);
            Width = Texture.Width;
            Height = Texture.Height;
        }

        /// <summary>
        /// Sprite
        /// </summary>
        /// <param name="atlas"> Atlas to render sprite from </param>
        /// <param name="index"> The atlas to index </param>
        public Sprite(SpriteAtlas atlas, int index)
        {
            spriteType = SpriteType.Atlas;
            Atlas = atlas;
            AtlasIndex = index;
            Width = Atlas.ItemDim;
            Height = atlas.ItemDim;
        }

        public void Draw(SpriteBatch sb, Transform transform, Rectangle? source = null, Color? colour = null)
        {
            // setup values
            if (colour == null) colour = Color.White;

            switch (spriteType)
            {
                case SpriteType.Atlas:
                    Atlas.Draw(sb, AtlasIndex, transform, source, colour);
                    break;
                case SpriteType.Texture:
                    if (source == null) source = Texture.Bounds;
                    sb.Draw(Texture, null, transform.Bounds, source, transform.RotationOrigin, transform.Rotation, transform.Scale, colour, transform.Flip, 0f);
                    break;
            }
        }

        public void DrawNineCut(SpriteBatch sb, Transform transform, Rectangle? source = null, Color? colour = null, int? destEdge = null, int? sourceEdge = null)
        {
            // setup values
            if (colour == null) colour = Color.White;

            switch (spriteType)
            {
                case SpriteType.Atlas:
                    if (destEdge == null) destEdge = transform.Bounds.Width / 3;
                    if (sourceEdge == null) sourceEdge = Atlas.ItemDim / 3;
                    if (source == null) source = new Rectangle(0, 0, Atlas.ItemDim, Atlas.ItemDim);
                    Atlas.DrawNineCut(sb, AtlasIndex, transform, source, colour, destEdge, sourceEdge);
                    break;
                case SpriteType.Texture:
                    if (destEdge == null) destEdge = 15;
                    if (sourceEdge == null) sourceEdge = Texture.Width / 3;
                    if (source == null) source = Texture.Bounds;
                    TextureUtil.DrawTextureNineCut(sb, Texture, colour.Value, destEdge.Value, sourceEdge.Value, transform.Bounds, source.Value);
                    break;
            }
        }
    }
}
