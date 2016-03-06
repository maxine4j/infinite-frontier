
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    interface IGUIElement
    {
        Form Form { get; set; }
        bool Update(GameTime gameTime, bool interacted);
        void Draw();
    }
}
