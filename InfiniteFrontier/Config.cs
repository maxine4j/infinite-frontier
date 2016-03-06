using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    [Serializable()]
    public class Config
    {
        [XmlElement("gfx_resX")]
        public int gfx_resX { get; set; }
        [XmlElement("gfx_resY")]
        public int gfx_resY { get; set; }
        [XmlElement("gfx_fullscreen")]
        public bool gfx_fullscreen { get; set; }
        [XmlElement("gfx_orbitSmoothness")]
        public int gfx_orbitSmoothness { get; set; }
        [XmlElement("gfx_showfps")]
        public byte gfx_showfps { get; set; }

        [XmlElement("gui_showSystemLabels")]
        public byte gui_showSystemLabels { get; set; }
        [XmlElement("gui_showPlanetLabels")]
        public byte gui_showPlanetLabels { get; set; }
        [XmlElement("gui_show")]
        public byte gui_show { get; set; }

        [XmlElement("aud_music")]
        public byte aud_music { get; set; }
        [XmlElement("aud_soundfx")]
        public byte aud_soundfx { get; set; }

        [XmlElement("cameraScrollSpeed")]
        public int cameraScrollSpeed { get; set; }
        [XmlElement("inp_cameraUp")]
        public Keys inp_cameraUp { get; set; }
        [XmlElement("inp_cameraDown")]
        public Keys inp_cameraDown { get; set; }
        [XmlElement("inp_cameraRight")]
        public Keys inp_cameraRight { get; set; }
        [XmlElement("inp_cameraLeft")]
        public Keys inp_cameraLeft { get; set; }
        [XmlElement("inp_openConsole")]
        public Keys inp_openConsole { get; set; }
        [XmlElement("inp_queueOrder")]
        public Keys inp_queueOrder { get; set; }

        [XmlElement("show_console")]
        public byte show_console { get; set; }

        public Config() { }

        // resets the config file to defaults
        public void RestoreDefaults()
        {
            if (console != null) console.WriteLine("resetting config file", MsgType.Info);
            gfx_resX = 1280;
            gfx_resY = 720;
            gfx_fullscreen = false;
            gfx_orbitSmoothness = 10000;

            aud_music = 1;
            aud_soundfx = 1;

            cameraScrollSpeed = 5000;
            inp_cameraUp = Keys.Up;
            inp_cameraDown = Keys.Down;
            inp_cameraRight = Keys.Left;
            inp_cameraLeft = Keys.Right;
            inp_openConsole = Keys.OemTilde;
            inp_queueOrder = Keys.LeftShift;

            show_console = 0;
        }

        // Saves the config to file
        public void SaveConfig()
        {
            XmlSerializer xmls = new XmlSerializer(typeof(Config));
            using (TextWriter writer = new StreamWriter("Config.xml"))
            {
                xmls.Serialize(writer, this);
                writer.Close();
            }
        }

        // Loads the config from file
        public static Config LoadConfig()
        {
            XmlSerializer xmls = new XmlSerializer(typeof(Config));
            try
            {
                using (XmlReader reader = XmlReader.Create(File.OpenRead("Config.xml"), new XmlReaderSettings() { CloseInput = true }))
                {
                    Config c = (Config)xmls.Deserialize(reader);
                    reader.Close();
                    return c;
                }
            }
            catch (Exception)
            {
                if (console != null) console.WriteLine("Unable to load config file, loading defaults", MsgType.Failed);
                Config new_config = new Config();
                new_config.RestoreDefaults();
                return new_config;
            }
        }
    }
}
