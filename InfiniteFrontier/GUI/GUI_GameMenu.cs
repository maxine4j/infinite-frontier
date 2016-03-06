using Microsoft.Xna.Framework;
using System;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_GameMenu : IGUIElement
    {
        public Form Form { get; set; }

        private Button btnReturnToGame, btnSettings, btnHelp, btnMainMenu, btnQuit;

        public GUI_GameMenu()
        {
            Form = new Form(new Transform(1920 / 2 - 150, 1080 / 2 - 250, 300, 500), formBack, "Game Menu");
            Form.Hidden = true;

            int init = 100;
            int sep = 10;
            int height = BUTTON_HEIGHT;
            int index = 0;
            btnHelp = new Button(new Transform(10, index++ * (height + sep) + init, Form.Transform.Bounds.Width - 20, height), "Help", button, true);
            btnSettings = new Button(new Transform(10, index++ * (height + sep) + init, Form.Transform.Bounds.Width - 20, height), "Settings", button, true);
            btnMainMenu = new Button(new Transform(10, index++ * (height + sep) + init, Form.Transform.Bounds.Width - 20, height), "Main Menu", button, true);
            btnQuit = new Button(new Transform(10, index++ * (height + sep) + init, Form.Transform.Bounds.Width - 20, height), "Quit", button, true);
            btnReturnToGame = new Button(new Transform(10, Form.Transform.Bounds.Height - height - 10, Form.Transform.Bounds.Width - 20, height), "Return to Game", button, true);

            Form.AddComponent(btnHelp);
            Form.AddComponent(btnSettings);
            Form.AddComponent(btnMainMenu);
            Form.AddComponent(btnQuit);
            Form.AddComponent(btnReturnToGame);
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            UpdateButtons();
            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }
        private void UpdateButtons()
        {
            if (!Form.Hidden)
            {
                if (btnSettings.IsLeftClicked)
                    settingsForm.SetUpForm();
                if (btnMainMenu.IsLeftClicked)
                    sceneManager.ChageScene(mainMenu);
                if (btnQuit.IsLeftClicked)
                    Environment.Exit(0);
                if (btnReturnToGame.IsLeftClicked)
                    Form.Hidden = true;
            }
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }
}
