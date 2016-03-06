using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_Tech : IGUIElement
    {
        private class Line
        {
            private Vector2 pos1, pos2;

            public Line(Vector2 p1, Vector2 p2)
            {
                pos1 = p1;
                pos2 = p2;
            }

            public void Draw(SpriteBatch sb, Vector2 origin, Color c)
            {
                GraphicsUtil.DrawLine(sb, pos1 + origin, pos2 + origin, 2, c);
            }
        }

        public Form Form { get; set; }

        private int scrollIndex, lastScrollIndex, itemHeight, itemWidth, sepX, sepY, offsetX, offsetY, selectedIndex, collumns, rows;
        private List<Button> buttons;
        private List<Tooltip> tooltips;
        private List<Line> lines;
        private TechNode lastTech;

        public GUI_Tech()
        {
            itemHeight = 50;
            itemWidth = 200;
            sepX = 50;
            sepY = 20;
            offsetX = 10;
            offsetY = 40;
            collumns = 6;
            rows = 10;
            float formWidth = offsetX * 3 + (itemWidth + sepX) * collumns - sepX;
            float formHeight = offsetY + offsetX + (itemHeight + sepY) * rows;
            Form = new Form(new Transform(1920 / 2 - formWidth / 2, 40, formWidth, formHeight), formBack, "Research");
            Form.Hidden = true;
            buttons = new List<Button>();
            lines = new List<Line>();
            tooltips = new List<Tooltip>();
            selectedIndex = 0;
            InitButtons();
        }
        
        // Set up
        private void InitButtons()
        {
            buttons.Clear();
            lines.Clear();
            tooltips.Clear();
            Form.RemoveAllComponents();
            for (int i = 0; i < player.TechTree.Nodes.Count; i++)
            {
                TechNode tn = player.TechTree.Nodes[i];
                Transform t = new Transform(offsetX + tn.GridX * (itemWidth + sepX) + scrollIndex * (itemWidth + tn.GridX + sepX), offsetY + tn.GridY * (itemHeight + sepY), itemWidth, itemHeight);
                if (Form.Transform.Contains(t.Translation + Form.Origin))
                {
                    Sprite s = techLocked;
                    if (tn.Unlocked) s = techUnlocked;
                    if (selectedIndex == tn.ID) s = techSelected;

                    Button b = new Button(t, tn.Name, s, true);
                    buttons.Add(b);
                    Form.AddComponent(b);
                    Tooltip tt = new Tooltip(b);
                    tt.FollowCurosr = true;
                    tt.UpdateContent(Font.InfoText, tn.Description, 400);
                    tooltips.Add(tt);

                    for (int j = 0; j < tn.Prerequisites.Count; j++)
                    {
                        TechNode prereq = player.TechTree.Nodes[tn.Prerequisites[j]];
                        Transform prereqt = new Transform(offsetX + prereq.GridX * (itemWidth + sepX) + scrollIndex * (itemWidth + prereq.GridX), offsetY + prereq.GridY * (itemHeight + sepY), itemWidth, itemHeight);
                        lines.Add(new Line(t.Translation + new Vector2(0, itemHeight / 2), prereqt.Translation + new Vector2(itemWidth + sepX * scrollIndex, itemHeight / 2)));
                    }
                }
            }
            Form.AddComponents(tooltips.ToArray());
        }
        

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            if (!Form.Hidden && !interacted)
            {
                UpdateScroll();
                UpdateButtons();

                if (player.CurrentTech != lastTech)
                    InitButtons();

                lastScrollIndex = scrollIndex;
                lastTech = player.CurrentTech;
            }

            if (!Form.Hidden && Form.Transform.Contains(Mouse.GetState().Position))
                return true;
            return false;
        }
        private void UpdateButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                TechNode tn = player.TechTree.Nodes[i];
                bool prereqMet = true;
                for (int j = 0; j < tn.Prerequisites.Count; j++)
                {
                    TechNode pn = player.TechTree.Nodes[tn.Prerequisites[j]];
                    if (!pn.Unlocked)
                        prereqMet = false;
                }
                if (buttons[i].IsLeftClicked && !tn.Unlocked && prereqMet)
                {
                    selectedIndex = i;
                    player.CurrentTech = player.TechTree.Nodes[selectedIndex];
                    InitButtons();
                }

                if (tn.Unlocked && selectedIndex == i)
                {
                    selectedIndex = -1;
                    InitButtons();
                }
            }
        }
        private void UpdateScroll()
        {
            if (Form.Transform.Contains(Mouse.GetState().Position, scale))
            {
                if (InputUtil.WasKeyDown(Keys.PageUp) || lastMouseState.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                    scrollIndex--;
                if (InputUtil.WasKeyDown(Keys.PageDown) || lastMouseState.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                    scrollIndex++;
                //if (scrollIndex >= maxX - (int)Math.Floor((float)(Form.Transform.Bounds.Width / itemWidth)))
                //    scrollIndex = maxX - (int)Math.Floor((float)(Form.Transform.Bounds.Height / itemWidth));
                if (scrollIndex < 0)
                    scrollIndex = 0;
                if (lastScrollIndex != scrollIndex)
                    InitButtons();
            }
        }

        // Draw
        public void Draw()
        {
            if (!Form.Hidden)
            {
                foreach (Line l in lines)
                {
                    l.Draw(sbGUI, Form.Transform.Translation, Color.Aqua);
                }
            }
            Form.Draw(sbGUI, scale);
        }
    }
}
