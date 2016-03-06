using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public interface IListItem
    {
        Button Button { get; set; }
        string Text { get; set; }
        void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null);
    }
}
