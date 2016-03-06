using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arwic.InfiniteFrontier
{
    public class SceneManager
    {
        private IBaseScene currentScene;

        /// <summary>
        /// Creates a container that controlls a collection of scenes.
        /// </summary>
        public SceneManager()
        {

        }
        
        public void Update(GameTime gameTime)
        {
           currentScene.OnUpdate(gameTime);
        }

        /// <summary>
        /// Draws the current scene.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            currentScene.OnDraw(gameTime);
        }

        /// <summary>
        /// Changes to the given scene.
        /// </summary>
        /// <param name="scene"> The scene to change to. </param>
        public void ChageScene(IBaseScene scene)
        {
            if (currentScene != null)
            {
                currentScene.OnLeave();                
            }
            currentScene = scene;
            currentScene.OnEnter();
        }
    }
}
