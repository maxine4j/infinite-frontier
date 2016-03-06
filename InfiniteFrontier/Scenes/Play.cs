using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Scene_Play : IBaseScene
    {
        public void OnInit()
        {
            
        }

        public void OnFinal()
        {

        }

        public void OnEnter()
        {
            simulationManager = new SimulationManager();
            simulationManager.StartGame();
            interfaceManager = new InterfaceManager();
            interfaceManager.GUIPlanetManagment.FollowPlanet();
        }

        public void OnLeave()
        {

        }

        private void UpdateCamera(GameTime gameTime, bool guiInteracted)
        {
            camera.Update(gameTime);
            
            if (Keyboard.GetState().IsKeyDown(config.inp_cameraUp))
                camera.TranslationTarget += new Vector2(0, -(float)(config.cameraScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds) / camera.Transform.Scale.Y / 10f);
            if (Keyboard.GetState().IsKeyDown(config.inp_cameraDown))
                camera.TranslationTarget += new Vector2(0, (float)(config.cameraScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds) / camera.Transform.Scale.Y / 10f);
            if (Keyboard.GetState().IsKeyDown(config.inp_cameraLeft))
                camera.TranslationTarget += new Vector2((float)(config.cameraScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds) / camera.Transform.Scale.X / 10f, 0);
            if (Keyboard.GetState().IsKeyDown(config.inp_cameraRight))
                camera.TranslationTarget += new Vector2(-(float)(config.cameraScrollSpeed * gameTime.ElapsedGameTime.TotalSeconds) / camera.Transform.Scale.X / 10f, 0);
            
            if (!guiInteracted)
            {
                if (Mouse.GetState().ScrollWheelValue != lastMouseState.ScrollWheelValue)
                {
                    float diff = (Mouse.GetState().ScrollWheelValue - lastMouseState.ScrollWheelValue) / 120.0f;
                    diff *= Math.Min(camera.ZoomTarget.X * camera.ZoomTarget.X, 0.01f);
                    diff *= 5;
                    camera.ZoomTarget += new Vector2(diff, diff);

                    float camMin = 0.01f;
                    float camMax = 1f;

                    if (camera.ZoomTarget.X < camMin)
                        camera.ZoomTarget = new Vector2(camMin, camera.ZoomTarget.Y);
                    if (camera.ZoomTarget.Y < camMin)
                        camera.ZoomTarget = new Vector2(camera.ZoomTarget.X, camMin);

                    if (camera.ZoomTarget.X > camMax)
                        camera.ZoomTarget = new Vector2(camMax, camera.ZoomTarget.Y);
                    if (camera.ZoomTarget.Y > camMax)
                        camera.ZoomTarget = new Vector2(camera.ZoomTarget.X, camMax);
                }
            }
        }

        public void OnUpdate(GameTime gameTime)
        {
            bool guiInteracted = interfaceManager.Update(gameTime);
            simulationManager.Update(gameTime, guiInteracted);
            unitCommandManager.Update(guiInteracted);

            UpdateCamera(gameTime, guiInteracted);

            if (InputUtil.IsKeyDown(Keys.Z))
                drawLabels = true;
            else
                drawLabels = false;
        }

        public void OnDraw(GameTime gameTime)
        {
            simulationManager.Draw();
            interfaceManager.Draw();

            if (InputUtil.IsKeyDown(Keys.Z)) 
                DrawDebug();
        }

        private void DrawDebug()
        {
            int spacing = 25;
            int xOffset = 10;
            int yOffset = 50;
            int index = 0;
            // draw some debug info, (TODO: move to a lua addon later) 
            sbGUI.DrawString(fonts[(int)Font.InfoText], "FPS: " + fpsCounter.CurrentFramesPerSecond, new Vector2(xOffset, index++ * spacing + yOffset), Color.White);
            sbGUI.DrawString(fonts[(int)Font.InfoText], "Camera Pos: " + camera.Transform.Translation, new Vector2(xOffset, index++ * spacing + yOffset), Color.White);
            sbGUI.DrawString(fonts[(int)Font.InfoText], "Camera Zoom: " + camera.Transform.Scale, new Vector2(xOffset, index++ * spacing + yOffset), Color.White);
            sbGUI.DrawString(fonts[(int)Font.InfoText], "Cursor GUI Pos: " + InputUtil.MouseScreenPos, new Vector2(xOffset, index++ * spacing + yOffset), Color.White);
            sbGUI.DrawString(fonts[(int)Font.InfoText], "Cursor Fore Pos: " + InputUtil.MouseWorldPos, new Vector2(xOffset, index++ * spacing + yOffset), Color.White);
        }
    }
}