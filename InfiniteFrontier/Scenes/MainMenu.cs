using Microsoft.Xna.Framework;
using System;

namespace Arwic.InfiniteFrontier
{

    public class Scene_MainMenu : IBaseScene
    {
        private Form form;
        private Button btnPlay, btnSettings, btnQuit;
        
        public void OnInit()
        {
        }

        public void OnFinal()
        {
        }

        public void OnEnter()
        {
            form = new Form(new Transform(1920 / 2 - 150, 1080 / 2 - 250, 300, 500), Main.formBack);


            // Initalises Buttons
            btnPlay = new Button(new Transform(form.Transform.Bounds.Width / 2 - Main.BUTTON_WIDTH / 2, 200, Main.BUTTON_WIDTH, Main.BUTTON_HEIGHT), "Play", Main.button, true);
            btnSettings = new Button(new Transform(form.Transform.Bounds.Width / 2 - Main.BUTTON_WIDTH / 2, 300, Main.BUTTON_WIDTH, Main.BUTTON_HEIGHT), "Settings", Main.button, true);
            btnQuit = new Button(new Transform(form.Transform.Bounds.Width / 2 - Main.BUTTON_WIDTH / 2, 400, Main.BUTTON_WIDTH, Main.BUTTON_HEIGHT), "Quit", Main.button, true);

            form.AddComponent(btnPlay);
            form.AddComponent(btnSettings);
            form.AddComponent(btnQuit);
        }

        public void OnLeave()
        {
        }

        private void UpdateButtons()
        {
            if (btnQuit.IsLeftClicked)
                Environment.Exit(0);
            if (btnSettings.IsLeftClicked)
                Main.settingsForm.SetUpForm();
            if (btnPlay.IsLeftClicked)
                Main.sceneManager.ChageScene(Main.play);
        }

        public void OnUpdate(GameTime gameTime)
        {
            form.Update(gameTime);
            if (!Main.settingsForm.Update(gameTime, false))
                UpdateButtons();
        }

        public void OnDraw(GameTime gameTime)
        {
            Main.sbBack.Draw(Main.mainMenuBack.Texture, new Rectangle(0, 0, 1920, 1080), Color.White);
            if (Main.settingsForm.Form.Hidden)
            {
                form.Draw(Main.sbGUI, Main.scale);
                Main.sbGUI.Draw(Main.logo.Texture, new Rectangle(1920 / 2 - Main.logo.Texture.Width / 2 + 10, 1080 / 2 - Main.logo.Texture.Height / 2 - 150, Main.logo.Texture.Width - 20, Main.logo.Texture.Height - 20), Color.White);
            }
        }
    }
}
