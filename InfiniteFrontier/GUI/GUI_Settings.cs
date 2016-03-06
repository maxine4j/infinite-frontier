using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_Settings : IGUIElement
    {
        private class ResolutionListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public object Data { get; set; }

            public Vector2 Resolution { get; set; }

            public ResolutionListItem(string text, int w, int h)
            {
                Resolution = new Vector2(w, h);
                Text = text;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                Button.Draw(sb, scale, colour, origin);
            }
        }

        public Form Form { get; set; }

        private ComboBox cbResolution;
        private Button btnApply, btnResetConfig;
        private CheckBox cbFullscreen, cbMusic, cbSound;

        private List<IListItem> resolutions = new List<IListItem>() {
                                           new ResolutionListItem("3840x2160", 3840, 2160),
                                           //"3200x1800",
                                           //"2880x1620",
                                           new ResolutionListItem("2560x1440", 2560, 1440),
                                           //"2048x1152",
                                           new ResolutionListItem("1920x1080", 1920, 1080),
                                           new ResolutionListItem("1600x900", 1600, 900),
                                           new ResolutionListItem("1366x768", 1366, 768),
                                           new ResolutionListItem("1280x720", 1280, 720),
                                           //"1024x576",
                                           //"960x540",
                                           //"864x486",
                                           //"720x705",
                                           new ResolutionListItem("640x360", 640, 360),
                                       };

        public GUI_Settings()
        {
            Form = new Form(new Transform(1920 / 8, 1080 / 8, 1920 * 3 / 4, 1080 * 3 / 4), formBack, "Settings", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Set up
        public void SetUpForm()
        {
            console.WriteLine("Setting up settings");
            Form.RemoveAllComponents();

            // Initializes Buttons
            btnApply = new Button(new Transform(100, 100, BUTTON_WIDTH, BUTTON_HEIGHT), "Apply Changes", button, true);
            btnResetConfig = new Button(new Transform(100 + 10 + BUTTON_WIDTH, 100, BUTTON_WIDTH, BUTTON_HEIGHT), "Reset Config", button, true);
            Form.AddComponent(btnResetConfig);
            Form.AddComponent(btnApply);

            // Initializes drop downs
            cbResolution = new ComboBox(new Transform(100, 200, BUTTON_WIDTH + 50, BUTTON_HEIGHT), "Resolution: ", resolutions, comboBoxButton, comboBoxButton);
            Form.AddComponent(cbResolution);

            // Initializes CheckBoxes
            cbFullscreen = new CheckBox(new Transform(BUTTON_WIDTH + 200, 200, checkBoxFalse.Texture.Width, checkBoxFalse.Texture.Height), "Full Screen", checkBoxTrue, checkBoxFalse);
            cbMusic = new CheckBox(new Transform(BUTTON_WIDTH + 200, 300, checkBoxFalse.Texture.Width, checkBoxFalse.Texture.Height), "Music (NYI)", checkBoxTrue, checkBoxFalse);
            cbSound = new CheckBox(new Transform(BUTTON_WIDTH + 200, 400, checkBoxFalse.Texture.Width, checkBoxFalse.Texture.Height), "Sound (NYI)", checkBoxTrue, checkBoxFalse);
            Form.AddComponent(cbFullscreen);
            Form.AddComponent(cbMusic);
            Form.AddComponent(cbSound);

            // set values from file to controlls
            cbFullscreen.Value = config.gfx_fullscreen;
            cbMusic.Value = Convert.ToBoolean(config.aud_music);
            cbSound.Value = Convert.ToBoolean(config.aud_soundfx);
            for (int i = 0; i < cbResolution.Items.Count; i++)
            {
                string[] res = cbResolution.Items[i].Text.Split('x');
                if (Convert.ToInt32(res[0]) == config.gfx_resX && Convert.ToInt32(res[1]) == config.gfx_resY)
                {
                    cbResolution.SelectedIndex = i;
                    break;
                }
            }

            Form.Hidden = false;
        }

        // Helpers
        private void ResetSettings()
        {
            config.RestoreDefaults();
            config.SaveConfig();
            config = Config.LoadConfig();
            SetUpForm();
        }
        private void ApplySettings()
        {
            // Set the config data
            config.gfx_fullscreen = cbFullscreen.Value;
            config.aud_music = Convert.ToByte(cbMusic.Value);
            config.aud_soundfx = Convert.ToByte(cbSound.Value);

            ResolutionListItem selectedItem = (ResolutionListItem)cbResolution.Selected;
            config.gfx_resX = (int)selectedItem.Resolution.X;
            config.gfx_resY = (int)selectedItem.Resolution.Y;

            // Apply the changes
            config.SaveConfig();
            ApplyGraphics();
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);
            if (!Form.Hidden)
            {
                UpdateButtons();
                return true;
            }
            return false;
        }
        private void UpdateButtons()
        {
            if (btnApply.IsLeftClicked)
                ApplySettings();
            if (btnResetConfig.IsLeftClicked)
                ResetSettings();
        }
        
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }
}
