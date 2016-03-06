using Microsoft.Xna.Framework;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class SimulationManager
    {
        public bool Paused { get; set; }

        private int lastPlayerID;

        private void GenerateSystems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                planetarySystems.Add(new PlanetarySystem(mapBounds, i));
            }
        }

        public void Tick(float millis)
        {
            //console.Write("Simulation ticking " + millis + "ms forward");

            foreach (Empire e in empires)
            {
                e.Tick(millis);
            }
            foreach (PlanetarySystem ps in planetarySystems)
            {
                ps.Tick(millis);
            }

            tickTimer.Start();
        }

        public void StartGame()
        {
            console.WriteLine("Starting simulation");
            Paused = false;

            tickTimer = new Timer(tick);
            planetarySystems = new List<PlanetarySystem>();
            GenerateSystems(100);
            unitManager = UnitManager.Load();
            buildingManager = BuildingManager.Load();
            
            lastPlayerID = 0;
            empires = new List<Empire>();
            player = new Empire(++lastPlayerID, "tNet", " tNet", Race.Avian);
            empires.Add(player);
            GenerateAIs();
        }

        private void GenerateAIs()
        {
            for (int i = 2; i < 3; i++)
			{
                empires.Add(new Empire(i, "AI_" + i, "AI_" + i + "ADJ", Race.Gekkin, new AI()));
			}
        }

        public void Update(GameTime gameTime, bool guiInteracted)
        {
            if (!Paused)
            {
                tickTimer.Update(gameTime);
                if (tickTimer.isExpired())
                    Tick(tick);

                UpdateEmpires(gameTime, guiInteracted);
                UpdateSystems(gameTime, guiInteracted);
                UpdateSelectedUnits(guiInteracted);
            }
        }
        private void UpdateSystems(GameTime gameTime, bool guiInteracted)
        {
            foreach (PlanetarySystem ps in planetarySystems)
            {
                ps.Update(gameTime, guiInteracted);
            }
        }
        private void UpdateSelectedUnits(bool guiInteracted)
        {
            foreach (Unit u in player.SelectedUnits)
            {
                u.UpdateSelected(guiInteracted);
            }
        }
        private void UpdateEmpires(GameTime gameTime, bool guiInteracted)
        {
            foreach (Empire e in empires)
            {
                e.Update(gameTime, guiInteracted);
            }
        }

        public void Draw()
        {
            DrawBack();
            DrawSelectedUnitHighlight();
            DrawSystems();
            DrawEmpires();
            DrawBorder();
        }
        private void DrawEmpires()
        {
            foreach (Empire e in empires)
            {
                e.Draw();
            }
        }
        private void DrawBack()
        {
            sbBack.Draw(gameBack.Texture, new Rectangle(0, 0, 1920, 1080), Color.White);
        }
        private void DrawSystems()
        {
            foreach (PlanetarySystem ps in planetarySystems)
            {
                ps.Draw(sbFore, Font.InfoText);
            }
        }
        private void DrawSelectedUnitHighlight()
        {
            foreach (Unit u in player.SelectedUnits)
            {
                u.DrawBoundingBox(Color.Gray);
            }
        }
        private void DrawBorder()
        {
            Vector2 xy = camera.ConvertWorldToGUI(new Vector2(mapBounds.X, mapBounds.Y));
            Rectangle rect = new Rectangle((int)xy.X, (int)xy.Y, (int)(mapBounds.Width * camera.Transform.Scale.X), (int)(mapBounds.Height * camera.Transform.Scale.Y));
            GraphicsUtil.DrawRect(sbGUI, rect, 1, Color.Gray);
        }
    }
}
