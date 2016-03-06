using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public interface IFormComponent
    {
        Transform Transform { get; set; }


        /// <summary>
        /// Updates the state of the component
        /// </summary>
        /// <param name="lastKeyboardState"></param>
        /// <param name="lastMouseState"></param>
        /// <param name="gameTime"></param>
        /// <param name="scale"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        bool Update(KeyboardState lastKeyboardState, MouseState lastMouseState, GameTime gameTime, float scale = 1f, Vector2? origin = null);

        /// <summary>
        /// Draws the component
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw the component with </param>
        /// <param name="font"> Font to draw the component's text with </param>
        /// <param name="scale"> Scale to draw the component by, defaults to (1f, 1f) </param>
        /// <param name="origin"> Origin of the component's parent, defaults to (0f, 0f) </param>
        /// <param name="colour"> Tint to draw the component with, defaults to white </param>
        void Draw(SpriteBatch sb, float scale = 1f, Color? colour = null, Vector2? origin = null);
    }
}
