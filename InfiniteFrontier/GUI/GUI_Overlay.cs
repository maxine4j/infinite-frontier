using Microsoft.Xna.Framework;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_Overlay : IGUIElement
    {
        public Form Form { get; set; }

        private Form form;
        private Button btnTechTree, btnSocialPolicy, btnOpenMenu, btnUnitList, btnPlanetList, btnFleetList;
        private ProgressBar tickTimerBar;
        private Label lblCredit, lblScience, lblEnergy;
        private Image imgCredit, imgScience, imgEnergy;
        private Transform statusBarTransform = new Transform(0, 0, 1920, 40);

        // Constructor
        public GUI_Overlay()
        {
            form = new Form(new Transform(0, 0, 1920, 40), formBack);

            int dim = 30;
            int spacing = 10;
            int xOffset = 1920 - 6 * (dim + spacing); // (dim + spacing)'s coeff = num of buttons
            int yOffset = 5;
            int index = 0;
            btnFleetList = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.Rocket));
            form.AddComponent(btnFleetList);
            btnPlanetList = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.Planet));
            form.AddComponent(btnPlanetList);
            btnUnitList = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.Rocket));
            form.AddComponent(btnUnitList);
            btnSocialPolicy = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.Scroll));
            form.AddComponent(btnSocialPolicy);
            btnTechTree = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.ConicalFlask));
            form.AddComponent(btnTechTree);
            btnOpenMenu = new Button(new Transform(index++ * (spacing + dim) + xOffset, yOffset, dim, dim), "", new Sprite(iconAtlas, (int)Icon.Gear));
            form.AddComponent(btnOpenMenu);

            lblCredit = new Label(new Transform(300, 10), "Credits: ", Color.White);
            form.AddComponent(lblCredit);
            lblScience = new Label(new Transform(600, 10), "Science: ", Color.White);
            form.AddComponent(lblScience);
            lblEnergy = new Label(new Transform(900, 10), "Energy: ", Color.White);
            form.AddComponent(lblEnergy);

            imgCredit = new Image(new Sprite(iconAtlas, (int)Icon.Dollar), new Transform(lblCredit.Transform.Translation.X - 30, lblCredit.Transform.Translation.Y, ICON_DIM));
            form.AddComponent(imgCredit);
            imgScience = new Image(new Sprite(iconAtlas, (int)Icon.ConicalFlask), new Transform(lblScience.Transform.Translation.X - 30, lblCredit.Transform.Translation.Y, ICON_DIM));
            form.AddComponent(imgScience);
            imgEnergy = new Image(new Sprite(iconAtlas, (int)Icon.LightningBolt), new Transform(lblEnergy.Transform.Translation.X - 30, lblCredit.Transform.Translation.Y, ICON_DIM));
            form.AddComponent(imgEnergy);

            tickTimerBar = new ProgressBar(new Transform(5, 5, 250, 30), progressBarBack, progressBarFill, 1, 0);
            form.AddComponent(tickTimerBar);
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            form.Update(gameTime);

            tickTimerBar.Value = tickTimer.GetProgress();

            UpdateButtons(interacted);
            UpdateLabels();

            if (statusBarTransform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }
        private void UpdateButtons(bool interacted)
        {
            if (!interacted)
            {
                if (btnFleetList.IsLeftClicked)
                {
                    interfaceManager.GUIPlanetList.Form.Hidden = true;
                    interfaceManager.GUIUnitList.Form.Hidden = true;

                    interfaceManager.GUIFleetList.Form.Hidden = !interfaceManager.GUIFleetList.Form.Hidden;
                    if (interfaceManager.GUIFleetList.Form.Hidden == false)
                        interfaceManager.GUIFleetList.SetUpForm();
                }
                if (btnPlanetList.IsLeftClicked)
                {
                    interfaceManager.GUIFleetList.Form.Hidden = true; ;
                    interfaceManager.GUIUnitList.Form.Hidden = true;

                    interfaceManager.GUIPlanetList.Form.Hidden = !interfaceManager.GUIPlanetList.Form.Hidden;
                    if (interfaceManager.GUIPlanetList.Form.Hidden == false)
                        interfaceManager.GUIPlanetList.SetUpForm();
                }
                if (btnUnitList.IsLeftClicked)
                {
                    interfaceManager.GUIPlanetList.Form.Hidden = true;
                    interfaceManager.GUIFleetList.Form.Hidden = true;

                    interfaceManager.GUIUnitList.Form.Hidden = !interfaceManager.GUIUnitList.Form.Hidden;
                    if (interfaceManager.GUIUnitList.Form.Hidden == false)
                        interfaceManager.GUIUnitList.SetUpForm();
                }
                if (btnOpenMenu.IsLeftClicked)
                {
                    interfaceManager.CloseForms();
                    interfaceManager.GUIPlanetManagment.CloseForm();
                    interfaceManager.GUIGameMenu.Form.Hidden = !interfaceManager.GUIGameMenu.Form.Hidden;
                }
                if (btnTechTree.IsLeftClicked)
                {
                    interfaceManager.GUISocialPolicy.Form.Hidden = true;
                    interfaceManager.GUIPlanetManagment.CloseForm();
                    interfaceManager.GUITech.Form.Hidden = !interfaceManager.GUITech.Form.Hidden;
                }
                if (btnSocialPolicy.IsLeftClicked)
                {
                    interfaceManager.GUITech.Form.Hidden = true;
                    interfaceManager.GUIPlanetManagment.CloseForm();
                    interfaceManager.GUISocialPolicy.Form.Hidden = !interfaceManager.GUISocialPolicy.Form.Hidden;
                }
                
            }
        }
        private void CloseInfoPanels()
        {
            interfaceManager.GUISocialPolicy.Form.Hidden = true;
            interfaceManager.GUITech.Form.Hidden = true;
            interfaceManager.GUIPlanetManagment.Form.Hidden = true;
        }
        private void CloseListForms()
        {
            interfaceManager.GUIFleetList.Form.Hidden = true;
            interfaceManager.GUIPlanetList.Form.Hidden = true;
            interfaceManager.GUIUnitList.Form.Hidden = true;
        }
        private void UpdateLabels()
        {
            int scienceProgress = 0;
            int scienceCost = 0;
            if (player.CurrentTech != null)
            {
                scienceProgress = player.CurrentTech.Progress;
                scienceCost = player.CurrentTech.Cost;
            }
            lblScience.Text = "Science: " + scienceProgress + "/" + scienceCost + " (" + player.SciencePerTick + "/t)";
            lblCredit.Text = "Credits: " + player.Credit + " (" + player.CreditPerTick + "/t)";
            lblEnergy.Text = "Energy: " + player.Energy + " (" + player.EnergyPerTick + "/t)";
        }

        // Draw
        public void Draw()
        {
            form.Draw(sbGUI, scale); 
        }
    }
}
