using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_UnitList : IGUIElement
    {
        private class UnitListItem : IListItem
        {
            public Button Button { get; set; }
            public string Text { get; set; }
            public int IconIndex { get; set; }
            public SpriteAtlas IconAtlas { get; set; }
            public Unit Unit { get; set; }

            public UnitListItem(Unit u)
            {
                Text = u.Name;
                IconIndex = u.IconIndex;
                IconAtlas = u.IconAtlas;
                Unit = u;
            }

            public void Draw(SpriteBatch sb, SpriteFont font, float scale = 1f, Color? colour = null, Vector2? origin = null)
            {
                if (origin == null) origin = Vector2.Zero;
                Button.Draw(sb, scale, colour, origin);
                IconAtlas.Draw(sb, IconIndex, new Transform(origin.Value.X + Button.Transform.Translation.X + 5, origin.Value.Y + Button.Transform.Translation.Y + 5, Button.Transform.Bounds.Height - 5, Button.Transform.Bounds.Height - 5));
            }
        }

        public Form Form { get; set; }

        private ScrollBox sbUnitList;
        private int lastUnitCount;

        public GUI_UnitList()
        {
            Form = new Form(new Transform(0, 1080 / 2 - 290, 300, 580), formBack, "Unit List", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
        }

        // Set up
        public void SetUpForm()
        {
            Form.RemoveAllComponents();

            sbUnitList = new ScrollBox(new Transform(10, 50, Form.Transform.Bounds.Width - 20, Form.Transform.Bounds.Height - 60), 40, GetUnitList(), scrollBoxButton, scrollBoxBack, scrollBoxHighlight, scrollBoxScrubber);
            Form.AddComponent(sbUnitList);
        }

        // Helpers
        private List<IListItem> GetUnitList()
        {
            List<IListItem> items = new List<IListItem>();
            foreach (Unit u in player.Units)
            {
                items.Add(new UnitListItem(u));
            }
            return items;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            if (!Form.Hidden)
            {
                if (player.Units.Count != lastUnitCount)
                    SetUpForm();
                UpdateButtons(interacted);
            }

            lastUnitCount = player.Units.Count;
            if (!Form.Hidden && Form.Transform.Contains(Mouse.GetState().Position))
                return true;
            return false;
        }
        private void UpdateButtons(bool interacted)
        {
            if (!interacted)
            {
                foreach (UnitListItem u in sbUnitList.Items)
                {
                    if (u.Button.IsLeftClicked)
                    {
                        player.SelectedUnits.Clear();
                        player.SelectedUnits.Add(u.Unit);
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

    public class GUI_UnitManagment : IGUIElement
    {
        public Form Form { get; set; }

        // main form components
        private TextBox tbName;
        private Label lblAttack, lblDefense, lblMoveSpeed, lblHealth;
        private Image imgAttack, imgDefense, imgMoveSpeed, imgHealth;
        private List<Button> commands;
        private Button btnLocateUnit;

        private List<Unit> lastUnitsSelected;
        private bool followingUnit = false;

        public GUI_UnitManagment()
        {
            Form = new Form(new Transform(0, 1080 - 250, 500, 250), formBack, "Unit Info", false, true, new Sprite(iconAtlas, (int)Icon.Cross));
            Form.Hidden = true;
            lastUnitsSelected = new List<Unit>();
        }

        // Set up
        private void SetUpForm()
        {
            console.WriteLine("Setting up UnitManagment");

            Form.RemoveAllComponents();

            if (player.SelectedUnits.Count == 1)
                SetUpSingleUnit();
            else
                SetUpFleet();

            Form.Hidden = false;
        }
        private void SetUpSingleUnit()
        {
            Form.Title = "Unit Info";

            tbName = new TextBox(new Transform(10, 95, Form.Transform.Bounds.Width - 20, 40), "Name:", player.SelectedUnits[0].Name, textBox, 24);
            Form.AddComponent(tbName);

            lblAttack = new Label(new Transform(40, 150),
                "Attack: " + player.SelectedUnits[0].Attack);
            Form.AddComponent(lblAttack);
            lblDefense = new Label(new Transform(40, 180),
                "Defense: " + player.SelectedUnits[0].Defense);
            Form.AddComponent(lblDefense);
            lblMoveSpeed = new Label(new Transform(40, 210),
                "Movement: " + player.SelectedUnits[0].Movement);
            Form.AddComponent(lblMoveSpeed);
            lblHealth = new Label(new Transform(Form.Transform.Size.X - 200, 150),
                "Health: " + player.SelectedUnits[0].HP + "/" + player.SelectedUnits[0].MaxHP + "(" + ((float)player.SelectedUnits[0].HP / (float)player.SelectedUnits[0].MaxHP * 100f) + "%)");
            Form.AddComponent(lblHealth);

            imgAttack = new Image(new Sprite(iconAtlas, (int)Icon.Cannon), new Transform(lblAttack.Transform.Translation.X - 30, lblAttack.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgAttack);
            imgDefense = new Image(new Sprite(iconAtlas, (int)Icon.Shield), new Transform(lblDefense.Transform.Translation.X - 30, lblDefense.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgDefense);
            imgMoveSpeed = new Image(new Sprite(iconAtlas, (int)Icon.Reticle), new Transform(lblMoveSpeed.Transform.Translation.X - 30, lblMoveSpeed.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgMoveSpeed);
            imgHealth = new Image(new Sprite(iconAtlas, (int)Icon.Heart), new Transform(lblHealth.Transform.Translation.X - 30, lblHealth.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgHealth);

            btnLocateUnit = new Button(new Transform(Form.Transform.Size.X - 60, Form.Transform.Size.Y - 60, 50, 50), "", new Sprite(iconAtlas, (int)Icon.Reticle));
            Form.AddComponent(btnLocateUnit);

            commands = GetUnitCommands();
            foreach (Button a in commands)
            {
                Form.AddComponent(a);
            }
        }
        private void SetUpFleet()
        {
            Form.Title = "Fleet Info";

            tbName = new TextBox(new Transform(10, 95, Form.Transform.Bounds.Width - 20, 40), "Name:", player.SelectedUnits[0].Name, textBox, 24);
            Form.AddComponent(tbName);

            int selectedCount = player.SelectedUnits.Count;
            int avgAttack = 0;
            int avgDefense = 0;
            int avgMovement = 0;
            float avgHP = 0f;
            int totalHP = 0;
            int totalMaxHP = 0;
            foreach (Unit u in player.SelectedUnits)
            {
                totalHP += u.HP;
                totalMaxHP += u.MaxHP;
                avgAttack += u.Attack;
                avgDefense += u.Defense;
                avgMovement += u.Movement;
                avgHP += (u.HP / u.MaxHP);
            }
            avgAttack /= selectedCount;
            avgDefense /= selectedCount;
            avgMovement /= selectedCount;
            avgHP /= selectedCount;
            avgHP *= 100f;

            lblAttack = new Label(new Transform(40, 150),
                "Average Attack: " + avgAttack);
            Form.AddComponent(lblAttack);
            lblDefense = new Label(new Transform(40, 180),
                "Average Defense: " + avgDefense);
            Form.AddComponent(lblDefense);
            lblMoveSpeed = new Label(new Transform(40, 210),
                "Average Movement: " + avgMovement);
            Form.AddComponent(lblMoveSpeed);
            lblHealth = new Label(new Transform(Form.Transform.Size.X - 200, 150),
                "Average Health: " + totalHP + "/" + totalMaxHP + "(" + avgHP + "%)");
            Form.AddComponent(lblHealth);

            imgAttack = new Image(new Sprite(iconAtlas, (int)Icon.Cannon), new Transform(lblAttack.Transform.Translation.X - 30, lblAttack.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgAttack);
            imgDefense = new Image(new Sprite(iconAtlas, (int)Icon.Shield), new Transform(lblDefense.Transform.Translation.X - 30, lblDefense.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgDefense);
            imgMoveSpeed = new Image(new Sprite(iconAtlas, (int)Icon.Reticle), new Transform(lblMoveSpeed.Transform.Translation.X - 30, lblMoveSpeed.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgMoveSpeed);
            imgHealth = new Image(new Sprite(iconAtlas, (int)Icon.Heart), new Transform(lblHealth.Transform.Translation.X - 30, lblHealth.Transform.Translation.Y, ICON_DIM, ICON_DIM));
            Form.AddComponent(imgHealth);

            btnLocateUnit = new Button(new Transform(Form.Transform.Size.X - 60, Form.Transform.Size.Y - 60, 50, 50), "", new Sprite(iconAtlas, (int)Icon.Reticle));
            Form.AddComponent(btnLocateUnit);

            //abilities = GetUnitAbilities();
            //foreach (Button a in abilities)
            //{
            //    Form.AddComponent(a);
            //}
        }

        // Helpers
        private List<Button> GetUnitCommands()
        {
            List<Button> buttons = new List<Button>();
            if (player.SelectedUnits[0].Commands != null)
            {
                for (int i = 0; i < player.SelectedUnits[0].Commands.Count; i++)
                {
                    buttons.Add(new Button(new Transform(i * 50 + 10, 20, 40, 40), "", new Sprite(iconAtlas, (int)unitCommandManager.GetCommandIcon(player.SelectedUnits[0].Commands[i]))));
                }
            }
            return buttons;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            UpdateCloseButton();
            UpdateCommands(interacted);
            UpdateName(interacted);
            UpdateLocate(interacted);
            UpdateFormSetUp();

            if (player.SelectedUnits.Count == 0)
                Form.Hidden = true;

            UpdateLastUnitsCache();

            if (!Form.Hidden && Form.Transform.Contains(Mouse.GetState().Position))
                return true;
            return false;
        }
        private void UpdateLastUnitsCache()
        {
            lastUnitsSelected.Clear();
            foreach (Unit u in player.SelectedUnits)
            {
                lastUnitsSelected.Add(u);
            }
        }
        private void UpdateFormSetUp()
        {
            bool selectedChanged = false;
            if (player.SelectedUnits.Count != lastUnitsSelected.Count)
                selectedChanged = true;
            else
                foreach (Unit u in player.SelectedUnits)
                    foreach (Unit lu in lastUnitsSelected)
                        if (lu != u)
                            selectedChanged = true;

            if (player.SelectedUnits.Count > 0 && selectedChanged)
                SetUpForm();
        }
        private void UpdateName(bool interacted)
        {
            if (player.SelectedUnits.Count > 0 && !interacted)
            {
                if (tbName != null && tbName.Selected && InputUtil.IsKeyDown(Keys.Enter))
                {
                    if (player.SelectedUnits.Count == 1)
                    {
                        if (tbName.Text.Length > 0)
                            player.SelectedUnits[0].Name = tbName.Text; // set unit name based on what is in the text box
                    }
                    else
                    {
                        if (tbName.Text.Length > 0)
                            player.SelectedFleet.Name = tbName.Text; // set unit name based on what is in the text box
                    }
                }
            }
        }
        private void UpdateLocate(bool interacted)
        {
            if (player.SelectedUnits.Count > 0 && !interacted)
            {
                if (btnLocateUnit != null && btnLocateUnit.IsLeftClicked)
                    followingUnit = !followingUnit;
                if (followingUnit)
                {
                    camera.LookAt(player.SelectedUnits[0].Transform.Translation);
                    camera.ZoomTarget = new Vector2(1.0f, 1.0f);
                }
            }
        }
        private void UpdateCloseButton()
        {
            if (Form.CloseButton.IsLeftClicked)
            {
                player.SelectedUnits.Clear();
                Form.Hidden = true;
            }
        }
        private void UpdateCommands(bool interacted)
        {
            if (player.SelectedUnits != null && player.SelectedUnits.Count > 0 && commands != null)
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].IsLeftClicked)
                        unitCommandManager.CallCommand(player.SelectedUnits[0], player.SelectedUnits[0].Commands[i]);
                }
            }
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }
}
