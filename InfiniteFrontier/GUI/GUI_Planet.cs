using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_PlanetList : IGUIElement
    {
        private class PlanetListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public int IconIndex { get; set; }
            public SpriteAtlas IconAtlas { get; set; }
            public Planet Planet { get; set; }

            public PlanetListItem(Planet p)
            {
                Text = p.Name;
                IconIndex = p.GraphicIndex;
                IconAtlas = p.GraphicAtlas;
                Planet = p;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                Button.Draw(sb, scale, colour, origin);
                IconAtlas.Draw(sb, IconIndex, new Transform(origin.Value.X + Button.Transform.Translation.X + 5, origin.Value.Y + Button.Transform.Translation.Y + 5, Button.Transform.Bounds.Height - 5, Button.Transform.Bounds.Height - 5));
            }
        }

        public Form Form { get; set; }

        private ScrollBox sbPlanetList;
        private List<IListItem> planetItems = new List<IListItem>();
        private int lastPlanetCount;

        public GUI_PlanetList()
        {
            Form = new Form(new Transform(0, 1080 / 2 - 290, 300, 580), formBack, "Planet List", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Set Up
        public void SetUpForm()
        {
            Form.RemoveAllComponents();
            GetPlanetList();
            Form.AddComponent(sbPlanetList);
        }
        private void GetPlanetList()
        {
            planetItems.Clear();
            foreach (Planet p in player.Planets)
            {
                planetItems.Add(new PlanetListItem(p));
            }
            sbPlanetList = new ScrollBox(new Transform(10, 50, Form.Transform.Bounds.Width - 20, Form.Transform.Bounds.Height - 60), 40, planetItems, scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            if (!Form.Hidden && !interacted)
            {
                if (player.Planets.Count != lastPlanetCount)
                    GetPlanetList();
            }
            UpatePlanetSelection(interacted);

            lastPlanetCount = player.Planets.Count;
            if (!Form.Hidden && Form.Transform.Contains(Mouse.GetState().Position))
                return true;
            return false;
        }
        private void UpatePlanetSelection(bool interacted)
        {
            if (!Form.Hidden && !interacted)
            {
                foreach (PlanetListItem pli in sbPlanetList.Items)
                {
                    if (pli.Button.IsLeftClicked)
                    {
                        interfaceManager.GUIPlanetManagment.SetUpForm(pli.Planet);
                        break;
                    }
                }
            }
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }

    public class GUI_PlanetManagment : IGUIElement
    {
        private class ScrollBoxProductionItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public int IconIndex { get; set; }
            public SpriteAtlas IconAtlas { get; set; }
            public Production Result { get; set; }

            public ScrollBoxProductionItem(Production p, SpriteAtlas iconAtlas, int iconIndex)
            {
                Button = null;
                Text = p.Result.Name;
                Result = p;
                IconAtlas = iconAtlas;
                IconIndex = iconIndex;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                Button.Draw(sb, scale, colour, origin);
                IconAtlas.Draw(sb, IconIndex, new Transform(origin.Value.X + Button.Transform.Translation.X + 5, origin.Value.Y + Button.Transform.Translation.Y + 5, Button.Transform.Bounds.Height - 5, Button.Transform.Bounds.Height - 5));
            }
        }

        private class ScrollBoxBuildingItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public int IconIndex { get; set; }
            public SpriteAtlas IconAtlas { get; set; }
            public Building Building { get; set; }

            public ScrollBoxBuildingItem(Building b, SpriteAtlas iconAtlas, int iconIndex)
            {
                Button = null;
                Text = b.Name;
                Building = b;
                IconAtlas = iconAtlas;
                IconIndex = iconIndex;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                Button.Draw(sb, scale, colour, origin);
                IconAtlas.Draw(sb, IconIndex, new Transform(origin.Value.X + Button.Transform.Translation.X + 5, origin.Value.Y + Button.Transform.Translation.Y + 5, Button.Transform.Bounds.Height - 5, Button.Transform.Bounds.Height - 5));
            }
        }

        public Form Form { get; set; }

        private Form frmProductionSelect, frmColonise, frmBuildingList;
        private Planet lastPlanetSelected;
        private int lastProductionQueueCount;
        private bool followingPlanet = false;
        private Planet selectedPlanet = null;

        // main form components
        private TextBox tbName;
        private Label lblTerrain, lblAtmosphere, lblResources, lblProductionProgress,
            lblNativeRace, lblNativePop, lblNativeHostility, lblNativeStrength, lblNativeHappiness,
            lblColonialRace, lblColonialPop, lblColonialStrength, lblColonialHappiness,
            lblSciencePT, lblEnergyPT, lblCreditPT, lblProductionPT;
        private ProgressBar pbProductionProgress;
        private Image imgPlanet, imgPlanetBack, imgNatives, imgNativesBack,
            imgCredit, imgScience, imgEnergy, imgColonialsBack, imgColonials, imgProduction;
        private Button btnProduction, btnColonise, btnLocatePlanet, btnBuildingList;
        private ScrollBox sbProductionQueue;

        // production form components
        private ScrollBox sbProductionList;
        private Button btnChangeProduction, btnAddProduction;
        private Production lastProduction;
        private int lastProductionIndex;

        // colonise form components
        private Button btnGenocide, btnAssimilate, btnIgnore;

        // buildinglist form components
        private ScrollBox sbBuildingList;
        private Button btnDemolish;
        private int lastBuildingCount;

        public GUI_PlanetManagment()
        {
            Form = new Form(new Transform(1920 / 2 - 400, 40, 800, 680), formBack, "Planet Managment", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
            frmProductionSelect = new Form(new Transform(1920 / 2 - 700, 40, 300, 600), formBack, "Production Selection", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            frmProductionSelect.Hidden = true;
            frmBuildingList = new Form(new Transform(1920 / 2 - 700, 40, 300, 600), formBack, "Building List", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            frmBuildingList.Hidden = true;
            frmColonise = new Form(new Transform(Form.Origin.X + 10, Form.Origin.Y + 150, 300, 200), formBack, "Colonisation Options", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            frmColonise.Hidden = true;
        }

        // Accessors
        public void CloseForm()
        {
            selectedPlanet = null;
            lastPlanetSelected = null;
            HideForms();
        }
        public void FollowPlanet()
        {
            followingPlanet = true;
        }

        // Helpers
        private List<IListItem> GetProductionList()
        {
            List<IListItem> items = new List<IListItem>();
            List<Production> prods = selectedPlanet.GetPossibleProductions();

            foreach (Production p in prods)
            {
                items.Add(new ScrollBoxProductionItem(p, iconAtlas, (int)Icon.Gear));
            }

            return items;
        }
        private List<IListItem> GetProductionQueue()
        {
            List<IListItem> items = new List<IListItem>();
            for (int i = 0; i < selectedPlanet.ProductionQueue.Count; i++)
            {
                items.Add(new ScrollBoxProductionItem(selectedPlanet.ProductionQueue.ToArray()[i], iconAtlas, 0));
            }
            return items;
        }
        private List<IListItem> GetBuildingList()
        {
            List<IListItem> items = new List<IListItem>();
            foreach (Building b in selectedPlanet.BuildingList)
            {
                items.Add(new ScrollBoxBuildingItem(b, iconAtlas, b.IconIndex));
            }
            return items;
        }
        private void HideSubForms()
        {
            frmProductionSelect.Hidden = true;
            frmBuildingList.Hidden = true;
            frmColonise.Hidden = true;
        }
        private void HideForms()
        {
            Form.Hidden = true;
            HideSubForms();
        }

        // Set up
        public void SetUpForm(Planet p)
        {
            console.WriteLine("Setting up PlanetManagment");

            selectedPlanet = p;

            Form.RemoveAllComponents();

            tbName = new TextBox(new Transform(10, 50, Form.Transform.Bounds.Width / 2 - 10 - 50, 40), "Name:", selectedPlanet.PlanetarySystem.ID + ":" + selectedPlanet.ID + ":" + selectedPlanet.Name, textBox, 20);
            Form.AddComponent(tbName);

            btnLocatePlanet = new Button(new Transform(Form.Transform.Size.X - 60, Form.Transform.Size.Y - 60, 50, 50), "", new Sprite(iconAtlas, (int)Icon.Reticle));
            Form.AddComponent(btnLocatePlanet);

            string prefix = "";
            int spacing = 30;
            int xOffset = Form.Transform.Bounds.Width / 2 + 10 - 50 + 200 + 10;
            int yOffset = 50;
            int index = 0;
            lblTerrain = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Terrain: " + selectedPlanet.Terrain);
            Form.AddComponent(lblTerrain);
            lblAtmosphere = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Atmosphere: " + selectedPlanet.Atmosphere);
            Form.AddComponent(lblAtmosphere);
            lblResources = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Resource: " + selectedPlanet.Resource);
            Form.AddComponent(lblResources);

            imgPlanetBack = new Image(formBack, new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), new Rectangle(10, 10, 20, 20), null, true);
            Form.AddComponent(imgPlanetBack);

            imgPlanet = new Image(new Sprite(planetAtlas, selectedPlanet.GraphicIndex),
                new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), null, null, true);
            Form.AddComponent(imgPlanet);

            // Native status
            switch (selectedPlanet.ColonisationStatus)
            {
                case ColonisationStatus.NativesAssimilated:
                case ColonisationStatus.NativesIgnored:
                case ColonisationStatus.Natives:
                    prefix = "Native ";
                    spacing = 30;
                    xOffset = Form.Transform.Bounds.Width / 2 + 10 - 50 + 200 + 10;
                    yOffset = 260;
                    index = 0;
                    lblNativeRace = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Race: " + selectedPlanet.NativeRace);
                    Form.AddComponent(lblNativeRace);
                    lblNativePop = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Population: " + selectedPlanet.NativePopulation);
                    Form.AddComponent(lblNativePop);
                    lblNativeHostility = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Hostility: " + selectedPlanet.NativeHostility * 100 + "%");
                    Form.AddComponent(lblNativeHostility);
                    lblNativeStrength = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Strength: " + selectedPlanet.NativeStrength);
                    Form.AddComponent(lblNativeStrength);
                    lblNativeHappiness = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Happiness: " + selectedPlanet.NativeHappiness);
                    Form.AddComponent(lblNativeHappiness);

                    imgNativesBack = new Image(formBack, new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), new Rectangle(10, 10, 20, 20), null, true);
                    Form.AddComponent(imgNativesBack);

                    imgNatives = new Image(new Sprite(racePortraitAtlas, (int)selectedPlanet.NativeRace - 1),
                        new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), null, null, true);
                    Form.AddComponent(imgNatives);
                    break;
            }

            // Colonial status
            switch (selectedPlanet.ColonisationStatus)
            {
                case ColonisationStatus.HomeWorld:
                case ColonisationStatus.NativesExterminated:
                case ColonisationStatus.NativesAssimilated:
                case ColonisationStatus.NativesIgnored:
                    if (selectedPlanet.ColonisationStatus != ColonisationStatus.HomeWorld)
                        prefix = "Colonial ";
                    else
                        prefix = "Home World ";
                    spacing = 30;
                    xOffset = Form.Transform.Bounds.Width / 2 + 10 - 50 + 200 + 10;
                    yOffset = 470;
                    index = 0;
                    lblColonialRace = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Race: " + selectedPlanet.ColonialRace);
                    Form.AddComponent(lblColonialRace);
                    lblColonialPop = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Population: " + selectedPlanet.ColonialPopulation);
                    Form.AddComponent(lblColonialPop);
                    lblColonialStrength = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Strength: " + selectedPlanet.ColonialStrength);
                    Form.AddComponent(lblColonialStrength);
                    lblColonialHappiness = new Label(new Transform(xOffset, index++ * spacing + yOffset), prefix + "Happiness: " + selectedPlanet.ColonialHappiness);
                    Form.AddComponent(lblColonialHappiness);

                    imgColonialsBack = new Image(formBack, new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), new Rectangle(10, 10, 20, 20), null, true);
                    Form.AddComponent(imgColonialsBack);

                    imgColonials = new Image(new Sprite(racePortraitAtlas, (int)selectedPlanet.ColonialRace - 1),
                        new Transform(Form.Transform.Bounds.Width / 2 + 10 - 50, yOffset, 200, 200), null, null, true);
                    Form.AddComponent(imgColonials);
                    break;
            }

            // Planet actions
            switch (selectedPlanet.ColonisationStatus)
            {
                case ColonisationStatus.Natives:
                case ColonisationStatus.NoNatives:
                    btnColonise = new Button(new Transform(10, 100, Form.Transform.Bounds.Width / 2 - 10 - 50, 50), "Colonise", button, true);
                    Form.AddComponent(btnColonise);
                    break;
                case ColonisationStatus.HomeWorld:
                case ColonisationStatus.NativesExterminated:
                case ColonisationStatus.NativesAssimilated:
                case ColonisationStatus.NativesIgnored:
                    btnBuildingList = new Button(new Transform(10, Form.Transform.Bounds.Height / 2 + 5 - 40 - 35 - 45, Form.Transform.Bounds.Width / 2 - 10 - 50, 40), "Building List", button, true);
                    Form.AddComponent(btnBuildingList);

                    btnProduction = new Button(new Transform(10, Form.Transform.Bounds.Height / 2 + 5 - 40 - 35, Form.Transform.Bounds.Width / 2 - 10 - 50, 40), "Production", button, true);
                    Form.AddComponent(btnProduction);

                    pbProductionProgress = new ProgressBar(new Transform(10, Form.Transform.Bounds.Height / 2 + 5 - 30, Form.Transform.Bounds.Width / 2 - 10 - 50, 30), progressBarBack, progressBarFill, 1, 0);
                    Form.AddComponent(pbProductionProgress);

                    string text = "";
                    if (selectedPlanet.CurrentProduction != null) text = "Building: " + selectedPlanet.CurrentProduction.Result.Name;
                    if (selectedPlanet.LastProduction != null) text = "Completed: " + selectedPlanet.LastProduction.Result.Name;
                    lblProductionProgress = new Label(new Transform(
                        pbProductionProgress.Transform.Translation.X + pbProductionProgress.Transform.Bounds.Width / 2 - fonts[(int)Font.InfoText].MeasureString(text).X / 2,
                        pbProductionProgress.Transform.Translation.Y + fonts[(int)Font.InfoText].MeasureString(text).Y / 4),
                        text);
                    Form.AddComponent(lblProductionProgress);

                    sbProductionQueue = new ScrollBox(new Transform(10, Form.Transform.Bounds.Height / 2 + 10, Form.Transform.Bounds.Width / 2 - 10 - 50, Form.Transform.Bounds.Height - Form.Transform.Bounds.Height / 2 - 20), 30, GetProductionQueue(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
                    sbProductionQueue.Selectable = false;
                    sbProductionQueue.Selected = -1;
                    Form.AddComponent(sbProductionQueue);

                    spacing = 30;
                    xOffset = 45;
                    yOffset = tbName.Transform.Bounds.Height + 60;
                    index = 0;
                    lblCreditPT = new Label(new Transform(xOffset, index++ * spacing + yOffset), "Credit: " + selectedPlanet.CreditPerTick + "/t");
                    Form.AddComponent(lblCreditPT);
                    lblSciencePT = new Label(new Transform(xOffset, index++ * spacing + yOffset), "Science: " + selectedPlanet.SciencePerTick + "/t");
                    Form.AddComponent(lblSciencePT);
                    lblEnergyPT = new Label(new Transform(xOffset, index++ * spacing + yOffset), "Energy: " + selectedPlanet.EnergyPerTick + "/t");
                    Form.AddComponent(lblEnergyPT);
                    lblProductionPT = new Label(new Transform(xOffset, index++ * spacing + yOffset), "Production: " + selectedPlanet.ProductionPerTick + "/t");
                    Form.AddComponent(lblProductionPT);

                    imgCredit = new Image(new Sprite(iconAtlas, (int)Icon.Dollar), new Transform(lblCreditPT.Transform.Translation.X - 30, lblCreditPT.Transform.Translation.Y, ICON_DIM, ICON_DIM));
                    Form.AddComponent(imgCredit);
                    imgScience = new Image(new Sprite(iconAtlas, (int)Icon.ConicalFlask), new Transform(lblSciencePT.Transform.Translation.X - 30, lblSciencePT.Transform.Translation.Y, ICON_DIM, ICON_DIM));
                    Form.AddComponent(imgScience);
                    imgEnergy = new Image(new Sprite(iconAtlas, (int)Icon.LightningBolt), new Transform(lblEnergyPT.Transform.Translation.X - 30, lblEnergyPT.Transform.Translation.Y, ICON_DIM, ICON_DIM));
                    Form.AddComponent(imgEnergy);
                    imgProduction = new Image(new Sprite(iconAtlas, (int)Icon.Hammer), new Transform(lblProductionPT.Transform.Translation.X - 30, lblProductionPT.Transform.Translation.Y, ICON_DIM, ICON_DIM));
                    Form.AddComponent(imgProduction);
                    break;
            }

            Form.Hidden = false;
        }
        private void SetUpProductionForm()
        {
            frmProductionSelect.RemoveAllComponents();

            HideSubForms();

            btnChangeProduction = new Button(new Transform(10, frmProductionSelect.Transform.Bounds.Height - 50, frmProductionSelect.Transform.Bounds.Width / 2 - 15, 40), "Change", button, true);
            btnAddProduction = new Button(new Transform(frmProductionSelect.Transform.Bounds.Width / 2 + 5, frmProductionSelect.Transform.Bounds.Height - 50, frmProductionSelect.Transform.Bounds.Width / 2 - 15, 40), "Queue", button, true);
            frmProductionSelect.AddComponent(btnChangeProduction);
            frmProductionSelect.AddComponent(btnAddProduction);

            sbProductionList = new ScrollBox(new Transform(10, 50, frmProductionSelect.Transform.Bounds.Width - 20, frmProductionSelect.Transform.Bounds.Height - 110), 30, GetProductionList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            sbProductionList.Selected = lastProductionIndex;

            frmProductionSelect.AddComponent(sbProductionList);

            frmProductionSelect.Hidden = false;
        }
        private void SetUpColoniseForm()
        {
            frmColonise.RemoveAllComponents();
            
            HideSubForms();

            btnGenocide = new Button(new Transform(10, 50, frmProductionSelect.Transform.Bounds.Width - 20, 40), "Genocide", button, true);
            frmColonise.AddComponent(btnGenocide);
            btnAssimilate = new Button(new Transform(10, 100, frmProductionSelect.Transform.Bounds.Width - 20, 40), "Assimilate", button, true);
            frmColonise.AddComponent(btnAssimilate);
            btnIgnore = new Button(new Transform(10, 150, frmProductionSelect.Transform.Bounds.Width - 20, 40), "Ignore", button, true);
            frmColonise.AddComponent(btnIgnore);

            frmColonise.Hidden = false;
        }
        private void SetUpBuildingListForm()
        {
            frmBuildingList.RemoveAllComponents();

            HideSubForms();

            btnDemolish = new Button(new Transform(10, frmBuildingList.Transform.Bounds.Height - 50, frmBuildingList.Transform.Bounds.Width - 20, 40), "Demolish", button, true);
            frmBuildingList.AddComponent(btnDemolish);

            sbBuildingList = new ScrollBox(new Transform(10, 50, frmBuildingList.Transform.Bounds.Width - 20, frmBuildingList.Transform.Bounds.Height - 110), 30, GetBuildingList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            frmBuildingList.AddComponent(sbBuildingList);

            frmBuildingList.Hidden = false;
        }
        
        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);
            frmProductionSelect.Update(gameTime);
            frmColonise.Update(gameTime);
            frmBuildingList.Update(gameTime);

            UpdateFormDisplay(interacted);

            if (selectedPlanet != null)
            {
                UpdateBuildingListForm(interacted);
                UpdateProductionForm(interacted);
                UpdateColoniseForm(interacted);
                
                UpdateName(interacted);
                UpdateFollow(interacted);
                UpdateLabels();
                UpdateProductionDisplay();

                lastPlanetSelected = selectedPlanet;
                lastBuildingCount = selectedPlanet.BuildingList.Count;
                lastProduction = selectedPlanet.CurrentProduction;
            }
            else
                HideForms();

            if (lastPlanetSelected != selectedPlanet)
                HideForms();

            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos) || !frmProductionSelect.Hidden && frmProductionSelect.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }
        private void UpdateName(bool interacted)
        {
            if (!interacted)
            {
                if (tbName.Selected && InputUtil.IsKeyDown(Keys.Enter))
                    if (tbName.Text.Length > 0)
                        selectedPlanet.Name = tbName.Text;
            }
        }
        private void UpdateFollow(bool interacted)
        {
            if (!interacted)
            {
                if (btnLocatePlanet.IsLeftClicked)
                    followingPlanet = !followingPlanet;
                if (followingPlanet)
                {
                    camera.LookAt(selectedPlanet.Transform.Translation);
                    camera.ZoomTarget = new Vector2(1.0f, 1.0f);
                }
            }
        }
        private void UpdateFormDisplay(bool interacted)
        {
            if (Form.Hidden)
                HideForms();

            if (!interacted)
            {
                if (Form.CloseButton.IsLeftClicked || InputUtil.WasKeyDown(Keys.Escape))
                {
                    CloseForm();
                }
            }

            if (selectedPlanet != null && selectedPlanet.ProductionQueue.Count != lastProductionQueueCount)
                SetUpForm(selectedPlanet);
        }
        private void UpdateColoniseForm(bool interacted)
        {
            if (!interacted)
            {
                if (btnColonise != null && btnColonise.IsLeftClicked)
                {
                    if (frmColonise.Hidden)
                        SetUpColoniseForm();
                    else
                        frmColonise.Hidden = true;
                }
                if (!frmColonise.Hidden)
                {
                    if (btnGenocide.IsLeftClicked)
                    {
                        selectedPlanet.NativeRace = Race.Null;
                        selectedPlanet.NativeHappiness = 0f;
                        selectedPlanet.NativeHostility = 1f;
                        selectedPlanet.NativePopulation = 0;
                        selectedPlanet.NativeStrength = 0;

                        selectedPlanet.ColonisationStatus = ColonisationStatus.NativesExterminated;
                        frmColonise.Hidden = true;
                        SetUpForm(selectedPlanet);
                    }
                    else if (btnAssimilate.IsLeftClicked)
                    {
                        selectedPlanet.NativeHappiness -= 0.3f;
                        selectedPlanet.NativeHostility += 0.2f;
                        selectedPlanet.NativeStrength -= 1;

                        selectedPlanet.ColonisationStatus = ColonisationStatus.NativesAssimilated;
                        frmColonise.Hidden = true;
                        SetUpForm(selectedPlanet);
                    }
                    else if (btnIgnore.IsLeftClicked)
                    {
                        selectedPlanet.NativeHappiness -= 0.05f;
                        selectedPlanet.NativeHostility += 0.05f;

                        selectedPlanet.ColonisationStatus = ColonisationStatus.NativesIgnored;
                        frmColonise.Hidden = true;
                        SetUpForm(selectedPlanet);
                    }
                }
            }
        }
        private void UpdateBuildingListForm(bool interacted)
        {
            if (!interacted)
            {
                if (btnBuildingList != null && btnBuildingList.IsLeftClicked)
                {
                    if (frmBuildingList.Hidden)
                        SetUpBuildingListForm();
                    else
                        frmBuildingList.Hidden = true;
                }
                if (!frmBuildingList.Hidden)
                {
                    if (lastBuildingCount != selectedPlanet.BuildingList.Count ||
                        lastProduction != selectedPlanet.CurrentProduction)
                    {
                        SetUpBuildingListForm();
                    }
                    if (btnDemolish.IsLeftClicked)
                    {
                        ScrollBoxBuildingItem item = (ScrollBoxBuildingItem)sbBuildingList.SelectedItem;
                        if (item != null)
                        {
                            selectedPlanet.BuildingList.Remove(item.Building);
                            SetUpBuildingListForm();
                        }
                    }
                }
            }
        }
        private void UpdateLabels()
        {
            lblTerrain.Text = "Terrain: " + selectedPlanet.Terrain;
            lblAtmosphere.Text = "Atmosphere: " + selectedPlanet.Atmosphere;
            lblResources.Text = "Resource: " + selectedPlanet.Resource;

            if (selectedPlanet.CurrentProduction != null)
            {
                lblProductionProgress.Text = "Building: " + selectedPlanet.CurrentProduction.Result.Name;
            }
            else if (selectedPlanet.LastProduction != null)
            {
                lblProductionProgress.Text = "Completed: " + selectedPlanet.LastProduction.Result.Name;
            }

            switch (selectedPlanet.ColonisationStatus)
            {
                case ColonisationStatus.NativesAssimilated:
                case ColonisationStatus.NativesIgnored:
                case ColonisationStatus.Natives:
                    lblNativePop.Text = "Native Population: " + selectedPlanet.NativePopulation;
                    lblNativeHostility.Text = "Native Hostility: " + selectedPlanet.NativeHostility * 100 + "%";
                    lblNativeStrength.Text = "Native Strength: " + selectedPlanet.NativeStrength;
                    lblNativeRace.Text = "Native Race: " + selectedPlanet.NativeRace;
                    break;
            }

            switch (selectedPlanet.ColonisationStatus)
            {
                case ColonisationStatus.HomeWorld:
                case ColonisationStatus.NativesExterminated:
                case ColonisationStatus.NativesAssimilated:
                case ColonisationStatus.NativesIgnored:
                    lblCreditPT.Text = "Credit:  " + selectedPlanet.CreditPerTick + "/t";
                    lblSciencePT.Text = "Science: " + selectedPlanet.SciencePerTick + "/t";
                    lblEnergyPT.Text = "Energy:   " + selectedPlanet.EnergyPerTick + "/t";
                    break;
            }
        }
        private void UpdateProductionForm(bool interacted)
        {
            if (!interacted)
            {
                if (btnProduction != null && btnProduction.IsLeftClicked)
                {
                    if (frmProductionSelect.Hidden)
                        SetUpProductionForm();
                    else
                        frmProductionSelect.Hidden = true;
                }
                if (!frmProductionSelect.Hidden)
                {
                    if (lastBuildingCount != selectedPlanet.BuildingList.Count ||
                       lastProduction != selectedPlanet.CurrentProduction)
                    {
                        SetUpProductionForm();
                    }
                    
                    switch (selectedPlanet.ColonisationStatus)
                    {
                        case ColonisationStatus.HomeWorld:
                        case ColonisationStatus.NativesExterminated:
                        case ColonisationStatus.NativesAssimilated:
                        case ColonisationStatus.NativesIgnored:
                            if (btnAddProduction.IsLeftClicked && sbProductionList.SelectedItem != null)
                            {
                                ScrollBoxProductionItem item = (ScrollBoxProductionItem)sbProductionList.SelectedItem;
                                EngineCall.QueueProduction(selectedPlanet, item.Result);
                                SetUpForm(selectedPlanet);
                                SetUpProductionForm();
                            }
                            if (btnChangeProduction.IsLeftClicked && sbProductionList.SelectedItem != null)
                            {
                                ScrollBoxProductionItem item = (ScrollBoxProductionItem)sbProductionList.SelectedItem;
                                EngineCall.ChangeProduction(selectedPlanet, item.Result);
                                SetUpForm(selectedPlanet);
                                SetUpProductionForm();
                            }
                            break;
                    }
                    lastProductionIndex = sbProductionList.Selected;
                }
            }
        }
        private void UpdateProductionDisplay()
        {
            if (selectedPlanet.CurrentProduction != null) // update the production progress bar
            {
                pbProductionProgress.Value = selectedPlanet.CurrentProduction.GetProgress();
            }
            else if (selectedPlanet.LastProduction != null)
            {
                pbProductionProgress.Value = 1.0f;
            }

            lastProductionQueueCount = selectedPlanet.ProductionQueue.Count;
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
            frmProductionSelect.Draw(sbGUI, scale);
            frmColonise.Draw(sbGUI, scale);
            frmBuildingList.Draw(sbGUI, scale);
        }
    }
}
