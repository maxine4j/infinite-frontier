using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class ConsoleManager
    {
        private int f_change_scene(List<string> args)
        {
            switch (args[0])
            {
                case "0":
                case "mainmenu":
                    sceneManager.ChageScene(mainMenu);
                    break;
                case "1":
                case "game":
                    sceneManager.ChageScene(play);
                    break;
                default:
                    break;
            }
            return EXIT_SUCCESS;
        }
        private int f_config_show(List<string> args)
        {
            settingsForm.SetUpForm();
            return EXIT_SUCCESS;
        }
        private int f_set(List<string> args)
        {
            Type type = typeof(Config);
            PropertyInfo[] props = type.GetProperties();
            bool propExists = false;
            foreach (PropertyInfo p in props)
            {
                if (p.Name.ToLower() == args[0].ToLower())
                {
                    try
                    {
                        object value;
                        if (p.PropertyType == typeof(Keys))
                            value = Enum.Parse(typeof(Keys), args[1], true);
                        else if (p.PropertyType == typeof(byte))
                        {
                            value = Convert.ToByte(args[1]);
                            if ((byte)value > 1 || (byte)value < 0)
                                value = Convert.ChangeType(value, p.PropertyType);
                        }
                        else
                            value = Convert.ChangeType(args[1], p.PropertyType);

                        p.SetValue(config, value, null);
                        propExists = true;
                    }
                    catch (Exception)
                    {
                        console.WriteLine("Unexpected data type for var:" + args[0], MsgType.Failed);
                        return EXIT_FAILURE;
                    }
                    break;
                }
            }
            if (!propExists)
            {
                console.WriteLine("Var does not exist", MsgType.Failed);
                return EXIT_FAILURE;
            }
            return EXIT_SUCCESS;
        }
        private int f_get(List<string> args)
        {
            if (args.Count == 1)
            {
                Type type = typeof(Config);
                PropertyInfo[] props = type.GetProperties();
                bool propExists = false;
                foreach (PropertyInfo p in props)
                {
                    if (p.Name.ToLower() == args[0].ToLower())
                    {
                        propExists = true;
                        console.WriteLine($"{p.Name}={p.GetValue(config).ToString()}", MsgType.Return);
                        break;
                    }
                }
                if (!propExists)
                {
                    console.WriteLine("Variable does not exist", MsgType.Failed);
                    return EXIT_FAILURE;
                }
            }
            else
            {
                Type type = typeof(Config);
                PropertyInfo[] props = type.GetProperties();
                foreach (PropertyInfo p in props)
                {
                    console.WriteLine($"{p.Name}={p.GetValue(config).ToString()}", MsgType.Return);
                }
            }
            return EXIT_SUCCESS;
        }
        private int f_gfx_apply(List<string> args)
        {
            ApplyGraphics();
            return EXIT_SUCCESS;
        }
        private int f_quit(List<string> args)
        {
            int exitstatus = EXIT_SUCCESS;
            if (args.Count != 0)
                exitstatus = Convert.ToInt32(args[0]);
            Environment.Exit(exitstatus);
            return exitstatus;
        }
        private int f_config_defaults(List<string> args)
        {
            EngineCall.ResetConfig();
            return EXIT_SUCCESS;
        }
        private int f_queue_p(List<string> args)
        {
            int systemID = 0;
            int planetID = 0;
            int typeID = 0;
            int productionID = 0;
            try
            {
                systemID = Convert.ToInt32(args[0]);
                planetID = Convert.ToInt32(args[1]);
                typeID = Convert.ToInt32(args[2]);
                productionID = Convert.ToInt32(args[3]);
            }
            catch (Exception)
            {
                console.WriteLine("Expected integers", MsgType.Failed);
            }

            if (planetarySystems[systemID]?.Planets[planetID]?.Owner != player)
                return EXIT_FAILURE;

            switch (typeID)
            {
                case 0:
                    foreach (var u in unitManager.Units)
                        if (u.ID == productionID)
                        {
                            Planet p = planetarySystems[systemID].Planets[planetID];
                            EngineCall.QueueProduction(p, new Production(u));
                            console.WriteLine($"PRODUCTION unit='{u.Name}' planet='{p.Name}' owner= {p.Owner.Name}", MsgType.Done);
                            break;
                        }
                    break;
                case 1:
                    foreach (var b in buildingManager.Buildings)
                        if (b.ID == productionID)
                        {
                            Planet p = planetarySystems[systemID].Planets[planetID];
                            EngineCall.QueueProduction(p, new Production(b));
                            console.WriteLine($"PRODUCTION building='{b.Name}' planet='{p.Name}' owner= {p.Owner.Name}", MsgType.Done);
                            break;
                        }
                    break;
            }
            return EXIT_SUCCESS;
        }
        private int f_cam_look(List<string> args)
        {
            if (args.Count < 2)
                return EXIT_FAILURE;
            try
            {
                Vector2 pos = new Vector2(Convert.ToSingle(args[0]), Convert.ToSingle(args[1]));
                camera.LookAt(pos);
            }
            catch (Exception e)
            {
                console.WriteLine(e.Message, MsgType.Failed);
            }
            return EXIT_SUCCESS;
        }
        private int f_clear(List<string> args)
        {
            messageLog.Lines.Clear();
            return EXIT_SUCCESS;    
        }
        private int f_new_game(List<string> args)
        {
            simulationManager.StartGame();
            return EXIT_SUCCESS;
        }

        private Dictionary<string, Func<List<string>, int>> commands = new Dictionary<string, Func<List<string>, int>>();
        private TextLog messageLog;
        private TextBox commandBox;
        private Form form;
        private List<string> commandHistory = new List<string>();
        private int historyIndex = 0;

        public ConsoleManager()
        {
            form = new Form(new Transform(0, 0, viewport.Width, 500), null, $"InfiniteFrontier - {CURRENT_VERSION}", false, false, null, Keys.OemTilde);
            form.Hidden = true;

            commandBox = new TextBox(new Transform(0, form.Transform.Bounds.Height - 30, form.Transform.Bounds.Width, 30));
            commandBox.ContentFont = Font.ConsoleText;
            form.AddComponent(commandBox);
            messageLog = new TextLog(new Transform(form.Transform.Translation.X, form.Transform.Translation.Y, form.Transform.Bounds.Width, form.Transform.Bounds.Height - commandBox.Transform.Bounds.Height), -1);
            messageLog.Font = Font.ConsoleText;
            form.AddComponent(messageLog);

            commands.Add("change_scene", f_change_scene);
            commands.Add("config_show", f_config_show);
            commands.Add("set", f_set);
            commands.Add("get", f_set);
            commands.Add("gfx_apply", f_gfx_apply);
            commands.Add("quit", f_quit);
            commands.Add("config_defaults", f_config_defaults);
            commands.Add("queue_p", f_queue_p);
            commands.Add("cam_look", f_cam_look);
            commands.Add("clear", f_clear);
            commands.Add("new_game", f_new_game);

        }

        public void Update(GameTime gameTime)
        {
            form.Update(gameTime);
            UpdateTextBox();
        }
        private void UpdateTextBox()
        {
            if (!form.Hidden)
            {
                commandBox.Selected = true;
                if (InputUtil.IsKeyDown(Keys.Enter) && commandBox.Text.Trim() != "")
                {
                    commandHistory.Add(commandBox.Text);
                    RunCommand(commandBox.Text);
                    commandBox.Text = "";
                    historyIndex = 0;
                }
                if (InputUtil.WasKeyDown(Keys.Down))
                {
                    if (commandHistory.Count > 0)
                    {
                        historyIndex++;
                        if (historyIndex >= commandHistory.Count)
                            historyIndex = 0;
                        commandBox.Text = commandHistory[historyIndex];
                    }
                }
                else if (InputUtil.WasKeyDown(Keys.Up))
                {
                    historyIndex--;
                    if (historyIndex < 0)
                    {
                        historyIndex = commandHistory.Count - 1;
                        if (historyIndex == -1)
                            historyIndex = 0;
                    }
                    try
                    {
                        commandBox.Text = commandHistory[historyIndex];
                    }
                    catch { }
                }
            }
            else
            {
                commandBox.Selected = false;
                commandBox.Text = "";
            }
        }

        public void WriteLine(string msg, MsgType type = MsgType.Info)
        {
            // create a prefix string
            string prefix = "";
            // create a time string
            string time = "[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + "] ";
            // determine what prefix is needed
            switch (type)
            {
                case MsgType.Info:
                    prefix = "INFO: ";
                    break;
                case MsgType.Warning:
                    prefix = "WARNING: ";
                    break;
                case MsgType.Failed:
                    prefix = "FAILED: ";
                    break;
                case MsgType.Done:
                    prefix = "DONE: ";
                    break;
                case MsgType.Input:
                    prefix = "> ";
                    break;
                case MsgType.Debug:
                    prefix = "DEBUG: ";
                    break;
                case MsgType.Return:
                    //prefix = "";
                    break;
                default:
                    break;
            }
            // write the fully formatted string to console
            string[] msgSplit = msg.Split('\n');
            foreach (string s in msgSplit)
            {
                string final = $"{time}{prefix}{s}";
                Console.Out.WriteLine(final);
                messageLog.WriteLine(final);
            }
        }
        public void RunCommand(string input)
        {
            string[] split = input.Split(' ');
            string cmd = split[0];
            List<string> args = new List<string>();
            for (int i = 1; i < split.Length; i++)
            {
                args.Add(split[i]);
            }
            WriteLine(input, MsgType.Input);
            bool commandExists = false;

            foreach (var c in commands)
            {
                if (c.Key == cmd)
                {
                    commandExists = true;
                    c.Value(args);
                }
            }
            if (!commandExists)
                WriteLine("Command does not exist", MsgType.Failed);
        }

        public void Draw()
        {
            form.Draw(sbGUI, scale);
        }
    }
}
