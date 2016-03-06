
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    interface IDrawable
    {
        /// <summary>
        /// Draws the drawable oject
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw to </param>
        /// <param name="transform"> Where the object is to be drawn </param>
        /// <param name="source"> Portion of the object to be drawn, defaults to entire object </param>
        /// <param name="colour"> Tint to be applied to the object, defaults to white </param>
        /// <param name="origin"> Point to rotate about, defaults to centre of current object </param>
        /// <param name="rotation"> Angle to rotate object by (R), defaults to 0f </param>
        /// <param name="scale"> Scale to draw object by, defaults to (1f, 1f) </param>
        /// <param name="effects"> Effects to be applied to the object, defaults to None </param>
        /// <param name="depth"> Layer depth to draw object at, defaults to 0f </param>
        void Draw(SpriteBatch sb, Transform transform, Rectangle? source = null, Color? colour = null);
    }
}
