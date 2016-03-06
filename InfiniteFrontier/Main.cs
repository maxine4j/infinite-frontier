using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Arwic.InfiniteFrontier
{
    public class Main : Game
    {
        public const int EXIT_SUCCESS = 0;
        public const int EXIT_FAILURE = 1;
        public const string CURRENT_VERSION = "0.0.2a";
        public const float ICON_DIM = 25;
        public const int BUTTON_WIDTH = 230;
        public const int BUTTON_HEIGHT = 60;

        public static float scale;
        public static Matrix scaleMatrix;
        public static MouseState lastMouseState;
        public static KeyboardState lastKeyboardState;
        public static GraphicsDeviceManager graphicsDM;
        public static GraphicsDevice graphics;
        public static SceneManager sceneManager;
        public static Viewport viewport;
        public static Random random;
        public static Config config;
        public static SpriteBatch sbGUI;
        public static SpriteBatch sbFore;
        public static SpriteBatch sbMid;
        public static SpriteBatch sbBack;
        public static Scene_MainMenu mainMenu;
        public static Scene_Play play;
        public static FrameCounter fpsCounter;
        public static List<SpriteFont> fonts;
        public static GUI_Settings settingsForm;
        public static ConsoleManager console;

        public static TechTree techTree;
        public static UnitManager unitManager;
        public static BuildingManager buildingManager;
        public static InterfaceManager interfaceManager;
        public static UnitCommandManager unitCommandManager;
        public static SimulationManager simulationManager;
        public static Camera camera;
        public static Timer tickTimer;
        public static int tick;
        public static Empire player;
        public static Empire defaultEmpire;
        public static Rectangle mapBounds;
        public static bool drawLabels;
        public static List<PlanetarySystem> planetarySystems;
        public static List<Empire> empires;
        public static List<Unit> Units
        {
            get
            {
                List<Unit> result = new List<Unit>();
                foreach (Empire e in empires)
                {
                    foreach (Unit u in e.Units)
                    {
                        result.Add(u);
                    }
                }
                return result;
            }
        }
        public static List<Planet> Planets
        {
            get
            {
                List<Planet> result = new List<Planet>();
                foreach (PlanetarySystem s in planetarySystems)
                {
                    foreach (Planet p in s.Planets)
                    {
                        result.Add(p);
                    }
                }
                return result;
            }
        }

        public static SpriteAtlas iconAtlas;
        public static SpriteAtlas planetAtlas;
        public static SpriteAtlas unitAtlas;
        public static SpriteAtlas racePortraitAtlas;
        public static Sprite mainMenuBack;
        public static Sprite gameBack;
        public static Sprite logo;
        public static Sprite checkBoxTrue;
        public static Sprite checkBoxFalse;
        public static Sprite textBox;
        public static Sprite formBack;
        public static Sprite progressBarFill;
        public static Sprite progressBarBack;
        public static Sprite scrollBoxHighlight;
        public static Sprite scrollBoxButton;
        public static Sprite scrollBoxBack;
        public static Sprite scrollBoxScrubber;
        public static Sprite comboBoxButton;
        public static Sprite planetLabelBack;
        public static Sprite button;
        public static Sprite techUnlocked;
        public static Sprite techLocked;
        public static Sprite techSelected;
        public static List<Sprite> planets;
        public static List<Sprite> stars;

        public Main()
            : base()
        {
            Init();
        }

        private void Init()
        {
            // Config
            config = Config.LoadConfig();

            // Graphics
            graphicsDM = new GraphicsDeviceManager(this);
            graphicsDM.CreateDevice();
            graphics = graphicsDM.GraphicsDevice;
            ApplyGraphics();
            sbFore = new SpriteBatch(graphics);
            sbBack = new SpriteBatch(graphics);
            sbGUI = new SpriteBatch(graphics);
            sbMid = new SpriteBatch(graphics);
            GraphicsUtil.Initialise(graphics);

            // Cursor
            IsMouseVisible = true;
            CursorUtil.Initialize(Window);
            CursorUtil.Set(Cursors.Main);

            // Content
            Content.RootDirectory = "Content";
            LoadTextures();
            LoadSounds();
            LoadFonts();

            // Props
            console = new ConsoleManager();
            Window.Title = "Infinite Frontier - " + CURRENT_VERSION;
            random = new Random();
            tick = 1000;
            defaultEmpire = new Empire(0, "DEFAULT_0", "DEFAULT_1", Race.Null);
            mapBounds = new Rectangle(0, 0, 16 * 40000, 9 * 40000);
            camera = new Camera();
            unitCommandManager = new UnitCommandManager();
            techTree = TechTree.Load();
            fpsCounter = new FrameCounter();
            settingsForm = new GUI_Settings();
            unitManager = UnitManager.Load();
            buildingManager = BuildingManager.Load();

            // Scenes
            sceneManager = new SceneManager();
            mainMenu = new Scene_MainMenu();
            play = new Scene_Play();
            sceneManager.ChageScene(mainMenu);
        }

        private void LoadTextures()
        {
            iconAtlas = new SpriteAtlas(Content, "Graphics/Atlas/Icons_1", 90);
            planetAtlas = new SpriteAtlas(Content, "Graphics/Atlas/Planets_1", 250);
            unitAtlas = new SpriteAtlas(Content, "Graphics/Atlas/Units_1", 250);
            racePortraitAtlas = new SpriteAtlas(Content, "Graphics/Atlas/Races_1", 250);
            mainMenuBack = new Sprite(Content, "Graphics/Backgrounds/MainMenu");
            gameBack = new Sprite(Content, "Graphics/Backgrounds/Game");
            logo = new Sprite(Content, "Graphics/Logo");
            checkBoxTrue = new Sprite(Content, "Graphics/Interface/Controls/CheckBox_True");
            checkBoxFalse = new Sprite(Content, "Graphics/Interface/Controls/CheckBox_False");
            textBox = new Sprite(Content, "Graphics/Interface/Controls/TextBox");
            formBack = new Sprite(Content, "Graphics/Interface/Controls/Form_Back");
            progressBarFill = new Sprite(Content, "Graphics/Interface/Controls/ProgressBar_Fill");
            progressBarBack = new Sprite(Content, "Graphics/Interface/Controls/ProgressBar_Back");
            scrollBoxHighlight = new Sprite(Content, "Graphics/Interface/Controls/ScrollBox_Highlight");
            scrollBoxButton = new Sprite(Content, "Graphics/Interface/Controls/ScrollBox_Button");
            scrollBoxBack = new Sprite(Content, "Graphics/Interface/Controls/ScrollBox_Back");
            scrollBoxScrubber = new Sprite(Content, "Graphics/Interface/Controls/ScrollBox_Scrubber");
            comboBoxButton = new Sprite(Content, "Graphics/Interface/Controls/ComboBox_Button");
            planetLabelBack = new Sprite(Content, "Graphics/Interface/PlanetLabelBack");
            button = new Sprite(Content, "Graphics/Interface/Controls/Button");
            techLocked = new Sprite(Content, "Graphics/Interface/Controls/Button");
            techUnlocked = new Sprite(Content, "Graphics/Interface/Controls/TechUnlocked");
            techSelected = new Sprite(Content, "Graphics/Interface/Controls/ScrollHighlight");

            planets = new List<Sprite>();
            for (int i = 0; i < 15; i++)
            {
                planets.Add(new Sprite(Content, "Graphics/Game/Planets/" + i));
            }

            stars = new List<Sprite>();
            for (int i = 0; i < 1; i++)
            {
                stars.Add(new Sprite(Content, "Graphics/Game/Stars/" + i));
            }
        }

        private void LoadSounds()
        {

        }

        private void LoadFonts()
        {
            fonts = new List<SpriteFont>();
            fonts.Add(Content.Load<SpriteFont>("Fonts/Crillee_20"));
            fonts.Add(Content.Load<SpriteFont>("Fonts/Crillee_12"));

            foreach (SpriteFont f in fonts)
            {
                f.DefaultCharacter = '*'/*'□'*/;
            }
        }

        public static void ApplyGraphics()
        {
            try
            {
                // Sets graphics values
                graphicsDM.PreferredBackBufferWidth = config.gfx_resX;
                graphicsDM.PreferredBackBufferHeight = config.gfx_resY;
                graphicsDM.IsFullScreen = config.gfx_fullscreen;
                graphicsDM.ApplyChanges();

                // Get graphics properties for display scaling
                viewport = new Viewport(0, 0, config.gfx_resX, config.gfx_resY);
                scale = viewport.Width / 1920f;
                scaleMatrix = Matrix.CreateScale(scale, scale, 1f);
            }
            catch (Exception) 
            {
                console.WriteLine("Error applying graphics settings", MsgType.Failed);
                Environment.Exit(1);
            }
        }

        
        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (!settingsForm.Update(gameTime, false))
                sceneManager.Update(gameTime);
            console.Update(gameTime);
            lastKeyboardState = Keyboard.GetState();
            lastMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Begins the SpriteBatches
            sbBack.Begin(0, null, null, null, null, null, scaleMatrix);
            if (camera != null) sbFore.Begin(0, null, null, null, null, null, camera.GetForegroundTransformation(GraphicsDevice, scale));
            sbGUI.Begin(0, null, null, null, null, null, scaleMatrix);
            // Clears the display for the new frame
            GraphicsDevice.Clear(Color.Black);

            // Draw the current scene
            sceneManager.Draw(gameTime);

            settingsForm.Draw();
            console.Draw();

            // Ends SpriteBatches
            sbBack.End();
            if (camera != null) sbFore.End();
            sbGUI.End();
            base.Draw(gameTime);
        }
    }
}
