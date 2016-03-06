using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Audio;

namespace Arwic.InfiniteFrontier
{
    public class Loader
    {
        public static string DefaultTexturePath = "Graphics/Util/Default";
        public static Texture2D LoadTexture(ContentManager cm, string path)
        {
            try
            {
                // check
                return cm.Load<Texture2D>(path);
            }
            catch (Exception)
            {
                try
                {
                    return cm.Load<Texture2D>(DefaultTexturePath);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
