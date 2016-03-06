using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public interface IBaseScene
    {
        /// <summary>
        /// Runs when the scene is registered.
        /// </summary>
        void OnInit();

        /// <summary>
        /// Runs when the scene is removed.
        /// </summary>
        void OnFinal();

        /// <summary>
        /// Runs every time the scene is changed to.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Runs every time the scene is changed from.
        /// </summary>
        void OnLeave();

        /// <summary>
        /// Runs every update call.
        /// </summary>
        /// <param name="gameTime"></param>
        void OnUpdate(GameTime gameTime);

        /// <summary>
        /// Runs every draw call.
        /// </summary>
        /// <param name="gameTime"></param>
        void OnDraw(GameTime gameTime);

    }
}
