using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_FleetList : IGUIElement
    {
        private class FleetListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public Fleet Fleet { get; set; }

            public FleetListItem(Fleet f)
            {
                Text = f.Name;
                Fleet = f;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                if (Button != null) Button.Draw(sb, scale, colour, origin);
            }
        }

        public Form Form { get; set; }

        // main form
        private ScrollBox sbFleetList;
        private Button btnTemplates;
        private int lastUnitCount = 0;

        public GUI_FleetList()
        {
            Form = new Form(new Transform(0, 1080 / 2 - 290, 300, 580), null, "Active Fleets", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Setup
        public void SetUpForm()
        {
            Form.RemoveAllComponents();

            sbFleetList = new ScrollBox(new Transform(10, 50, Form.Transform.Bounds.Width - 20, Form.Transform.Bounds.Height - 110), 40, GetFleetList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            Form.AddComponent(sbFleetList);

            btnTemplates = new Button(new Transform(10, Form.Transform.Bounds.Height - 50, Form.Transform.Bounds.Width - 20, 40), "Manage Fleets", button, true);
            Form.AddComponent(btnTemplates);

            Form.Hidden = false;
        }

        // Helpers
        private List<IListItem> GetFleetList()
        {
            List<IListItem> items = new List<IListItem>();
            foreach (Fleet f in player.Fleets)
            {
                items.Add(new FleetListItem(f));
            }
            return items;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            UpdateButtons();

            if (!Form.Hidden)
            {
                if (player.Units.Count != lastUnitCount)
                    SetUpForm();
            }

            lastUnitCount = player.Units.Count;

            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }
        private void UpdateButtons()
        {
            if (!Form.Hidden)
            {
                if (btnTemplates.IsLeftClicked)
                {
                    interfaceManager.GUIFleetTemplateList.SetUpForm();
                }
            }
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }

    public class GUI_FleetTemplateList : IGUIElement
    {
        private class FleetTemplateListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public FleetTemplate FleetTemplate { get; set; }

            public FleetTemplateListItem(FleetTemplate f)
            {
                Text = f.Name;
                FleetTemplate = f;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                if (Button != null) Button.Draw(sb, scale, colour, origin);
            }
        }


        public Form Form { get; set; }

        private Button btnProduceFromTemplate, btnNewFleetTemplate, btnDeleteFleetTemplate, btnEditFleetTemplate;
        private ScrollBox sbTemplateList;

        public GUI_FleetTemplateList()
        {
            Form = new Form(new Transform(300, 1080 / 2 - 290, 300, 580), formBack, "Fleet Templates", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Setup
        public void SetUpForm()
        {
            Form.RemoveAllComponents();

            sbTemplateList = new ScrollBox(new Transform(), 30, GetFleetTemplateList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            Form.AddComponent(sbTemplateList);

            int sep = 10;
            int height = 40;
            int index = 0;
            int xOffset = 10;
            int yOffset = 50;

            btnNewFleetTemplate = new Button(new Transform(xOffset, index++ * (height + sep) + yOffset, Form.Transform.Bounds.Width - 20, height), "New Template", button, true);
            Form.AddComponent(btnNewFleetTemplate);

            btnDeleteFleetTemplate = new Button(new Transform(xOffset, index++ * (height + sep) + yOffset, Form.Transform.Bounds.Width - 20, height), "Delete Template", button, true);
            Form.AddComponent(btnDeleteFleetTemplate);

            btnEditFleetTemplate = new Button(new Transform(xOffset, index++ * (height + sep) + yOffset, Form.Transform.Bounds.Width - 20, height), "Edit Template", button, true);
            Form.AddComponent(btnEditFleetTemplate);

            btnProduceFromTemplate = new Button(new Transform(10, Form.Transform.Bounds.Height - 10 - height, Form.Transform.Bounds.Width - 20, height), "Produce", button, true);
            Form.AddComponent(btnProduceFromTemplate);

            sbTemplateList = new ScrollBox(new Transform(10, index++ * (height + sep) + yOffset, Form.Transform.Bounds.Width - 20, Form.Transform.Bounds.Height - index * (height + sep) + yOffset - height - 70), 40, GetFleetTemplateList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            Form.AddComponent(sbTemplateList);
            
            Form.Hidden = false;
        }

        // Helpers
        private List<IListItem> GetFleetTemplateList()
        {
            List<IListItem> items = new List<IListItem>();
            foreach (FleetTemplate f in player.FleetTemplates)
            {
                items.Add(new FleetTemplateListItem(f));
            }
            return items;
        }
        
        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            UpdateButtons(interacted);

            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }
        private void UpdateButtons(bool interacted)
        {
            if (!Form.Hidden && !interacted)
            {
                if (btnNewFleetTemplate.IsLeftClicked)
                {
                    player.FleetTemplates.Add(new FleetTemplate("New Fleet", new List<UnitTemplate>(), new List<Vector2>()));
                    SetUpForm();
                }

                if (btnDeleteFleetTemplate.IsLeftClicked)
                {
                    if (sbTemplateList.Selected != -1)
                    {
                        FleetTemplateListItem item = (FleetTemplateListItem)sbTemplateList.SelectedItem;
                        player.FleetTemplates.Remove(item.FleetTemplate);
                        interfaceManager.GUIFleetDesigner.SetUpForm(item.FleetTemplate);
                        SetUpForm();
                    }
                }

                if (btnEditFleetTemplate.IsLeftClicked)
                {
                    if (sbTemplateList.Selected != -1)
                    {
                        Form.Hidden = true;
                        FleetTemplateListItem item = (FleetTemplateListItem)sbTemplateList.SelectedItem;
                        interfaceManager.GUIFleetDesigner.SetUpForm(item.FleetTemplate);
                    }
                }

                if (btnProduceFromTemplate.IsLeftClicked)
                {
                    FleetTemplateListItem item = (FleetTemplateListItem)sbTemplateList.SelectedItem;
                    if (item != null)
                        EngineCall.ProduceFleet(item.FleetTemplate);
                }
            }
        }
        private void UpdateFormDisplay()
        {
            if (interfaceManager.GUIFleetList.Form.Hidden)
                Form.Hidden = true;
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }

    public class GUI_FleetDesigner : IGUIElement
    {
        private class UnitListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            //public int IconIndex { get; set; }
            //public SpriteAtlas IconAtlas { get; set; }
            public UnitTemplate UnitTemplate { get; set; }

            public UnitListItem(UnitTemplate u)
            {
                Text = u.Name;
                //IconIndex = u.IconIndex;
                UnitTemplate = u;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                Button.Draw(sb, scale, colour, origin);
                //IconAtlas.Draw(sb, IconIndex, new Transform(origin.Value.X + Button.Transform.Translation.X + 5, origin.Value.Y + Button.Transform.Translation.Y + 5, Button.Transform.Bounds.Height - 5, Button.Transform.Bounds.Height - 5));
            }
        }

        public Form Form { get; set; }
        private ScrollBox sbUnitList;
        private Unit selectedUnit;
        private FleetTemplate template;
        private Image imgCanvas;
        private TextBox tbFleetTemplateName;

        // Ship info form
        private Form frmShipInfo;
        private Button btnRemoveShip;

        public GUI_FleetDesigner()
        {
            float formWidth = 1000;
            float formHeight = 600;
            Form = new Form(new Transform(1920 / 2 - formWidth / 2, 40, formWidth, formHeight), formBack, "Fleet Designer", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Set up
        public void SetUpForm(FleetTemplate ft)
        {
            Form.RemoveAllComponents();
            selectedUnit = null;

            template = ft;

            imgCanvas = new Image(formBack, new Transform(10, 50, Form.Transform.Bounds.Width - 180, Form.Transform.Bounds.Height - 60), null, null, true);
            Form.AddComponent(imgCanvas);

            sbUnitList = new ScrollBox(new Transform(), 30, GetPotentialShips(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            Form.AddComponent(sbUnitList);

            tbFleetTemplateName = new TextBox(new Transform(Form.Transform.Bounds.Width - 160, 50, 150, 40), "Fleet Name:", ft.Name, textBox, 20);
            Form.AddComponent(tbFleetTemplateName);

            Form.Hidden = false;
        }
        private void SetUpShipInfoForm(Vector2 pos)
        {
            frmShipInfo = new Form(new Transform(pos.X, pos.Y, 200, 150), formBack, "Unit Info", true, true, new Sprite(iconAtlas, (int)Icon.Cross));
            frmShipInfo.RemoveAllComponents();

            btnRemoveShip = new Button(new Transform(10, frmShipInfo.Transform.Bounds.Height - 50, frmShipInfo.Transform.Bounds.Width - 20, 40), "Remove Ship", button, true);
            frmShipInfo.AddComponent(btnRemoveShip);

            frmShipInfo.Hidden = false;
        }

        // Helpers
        private List<IListItem> GetPotentialShips()
        {
            List<IListItem> res = new List<IListItem>();

            // TODO make this a list of produible units
            foreach (UnitTemplate u in unitManager.Units)
            {
                UnitListItem item = new UnitListItem(u);
                res.Add(item);
            }

            return res;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);
            
            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }
}
