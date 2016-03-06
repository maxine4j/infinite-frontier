using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class PlanetarySystem : IOwnable
    {
        public Empire Owner { get; set; }
        public int ID { get; }
        public String Name { get; set; }
        public Transform Transform { get; set; }
        public int GraphicIndex { get; set; }
        public List<Planet> Planets { get; set; }
        public bool OrbitCW { get; private set; }
        
        private int lastOrbit;
        private bool drawPlanetOrbits;
        private bool drawPlanetInfo;

        public PlanetarySystem(Rectangle sectorBounds, int id)
        {
            ID = id;
            Planets = new List<Planet>();
            Owner = defaultEmpire;
            GenerateName();
            Vector2 centre = new Vector2(random.Next(sectorBounds.X, sectorBounds.Width), random.Next(sectorBounds.Y, sectorBounds.Height));
            float radius = random.Next(400, 700);
            Transform = new Transform(centre.X - radius, centre.Y - radius, radius * 2, radius * 2);
            GeneratePlanets(random.Next(2, 5));
            GraphicIndex = random.Next(0, 1);
            drawPlanetOrbits = false;
        }

        // Generate
        private void GenerateName()
        {
            switch (random.Next(0, 32))
            {
                case 0: Name += "Acamar"; break;
                case 1: Name += "Caph"; break;
                case 2: Name += "Gacrux"; break;
                case 3: Name += "Menkib"; break;
                case 4: Name += "Pollux"; break;
                case 5: Name += "Situla"; break;
                case 6: Name += "Vega"; break;
                case 7: Name += "Tureis"; break;
                case 8: Name += "Tarazet"; break;
                case 9: Name += "Tania"; break;
                case 10: Name += "Aldebaran"; break;
                case 11: Name += "Chara"; break;
                case 12: Name += "Giedi"; break;
                case 13: Name += "Heka"; break;
                case 14: Name += "Hadar"; break;
                case 15: Name += "Kraz"; break;
                case 16: Name += "Lesath"; break;
                case 17: Name += "Megrez"; break;
                case 18: Name += "Gatria"; break;
                case 19: Name += "Markab"; break;
                case 20: Name += "Mirfak"; break;
                case 21: Name += "Pherkad"; break;
                case 22: Name += "Phact"; break;
                case 23: Name += "Okul"; break;
                case 24: Name += "Rana"; break;
                case 25: Name += "Saiph"; break;
                case 26: Name += "Sham"; break;
                case 27: Name += "Thabit"; break;
                case 28: Name += "Tyl"; break;
                case 29: Name += "Thuban"; break;
                case 30: Name += "Zaniah"; break;
                case 31: Name += "Betria"; break;

                default: Name += "Betria"; break;
            }
            //Name += " " + RandomUtil.NextRandomInt(1000, 9999);
        }
        private void GeneratePlanets(int count)
        {
            lastOrbit = random.Next(Transform.Bounds.Width / 2 + 150, Transform.Bounds.Width / 2 + 350);
            OrbitCW = Convert.ToBoolean(random.Next(0, 2));
            for (int i = 0; i < count; i++)
            {
                Planets.Add(new Planet(this, random.Next(40, 200), lastOrbit, random.Next(5, 10), i));
                lastOrbit += random.Next(120, 400);
            }
        }

        // Accessors
        private int GetPopulation()
        {
            int pop = 0;
            foreach (Planet p in Planets)
            {
                pop += p.ColonialPopulation;
            }
            return pop;
        }
        public Vector2 GetMapPos()
        {
            return new Vector2(Transform.Translation.X / mapBounds.Width, Transform.Translation.Y / mapBounds.Height);
        }

        // Tick
        public void Tick(float millis)
        {
            foreach (Planet p in Planets)
            {
                p.Tick(millis);
            }
        }

        // Update
        public void Update(GameTime gameTime, bool guiInteracted)
        {
            UpdatePlanets(gameTime, guiInteracted);
            UpdateDrawPlanetLabels();
            UpdateDrawOrbits(guiInteracted);
        }
        private void UpdateDrawOrbits(bool guiInteracted)
        {
            if (!guiInteracted)
                drawPlanetOrbits = Transform.Contains(InputUtil.MouseWorldPos);
            else
                drawPlanetOrbits = false;
        }
        private void UpdateDrawPlanetLabels()
        {
            if (camera.Transform.Scale.X > 0.14f && camera.Transform.Scale.Y > 0.14f)
                drawPlanetInfo = true;
            else
                drawPlanetInfo = false;
        }
        private void UpdatePlanets(GameTime gameTime, bool guiInteracted)
        {
            foreach (Planet p in Planets)
            {
                p.Update(gameTime, guiInteracted);
            }
        }
        
        // Draw
        public void Draw(SpriteBatch sb, Font font)
        {
            stars[0].Draw(sb, Transform);
            foreach (Planet p in Planets)
            {
                p.Draw();
                if (drawPlanetOrbits)
                    p.DrawOrbit(Color.CornflowerBlue);
                if (drawPlanetInfo)
                    p.DrawInfo();
            }
            DrawInfo();

            if (drawLabels)
                sb.DrawString(fonts[(int)font], Name + " " + Transform.Translation, Transform.Translation, Color.White);
        }
        private void DrawInfo()
        {
            Vector2 translation = camera.ConvertWorldToGUI(Transform.Translation);
            planetLabelBack.DrawNineCut(sbGUI, new Transform(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 100, translation.Y - 40, 200, 40));
            sbGUI.DrawString(fonts[(int)Font.InfoText], Name, new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 55, translation.Y - 35), Color.White); // name
            sbGUI.DrawString(fonts[(int)Font.InfoText], GetPopulation().ToString() + " - ", new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 95, translation.Y - 35), Color.White); // population
        }
    }
}
