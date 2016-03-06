using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Empire
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Adjective { get; private set; }
        public Race Race { get; private set; }

        // AI props
        public AI AI { get; set; }
        public float Agressiveness { get; set; }
        public float Warmonger { get; set; }
        public Atmosphere PreferredAtmosphere { get; set; }
        public Terrain PreferredTerrain { get; set; }

        // Income
        public float SciencePerTick { get; private set; }
        public float Energy { get; set; }
        public float EnergyPerTick { get; private set; }
        public float Credit { get; set; }
        public float CreditPerTick { get; private set; }

        // Property
        public Planet HomeWorld { get; set; }
        public List<Planet> Planets { get; set; }
        public List<Unit> Units { get; set; }
        public List<Fleet> Fleets { get; set; }
        public List<FleetTemplate> FleetTemplates { get; set; }

        // Progression
        public TechTree TechTree { get; set; }
        public TechNode CurrentTech { get; set; }

        // Selections
        //public Planet SelectedPlanet { get; set; }
        public List<Unit> SelectedUnits { get; set; }
        public Fleet SelectedFleet { get; set; }

        public Empire(int id, string name, string adjective, Race race, AI ai = null)
        {
            ID = id;
            Name = name;
            Adjective = adjective;
            if (id != 0)
            {
                AI = ai;
                SciencePerTick = 0;
                Energy = 100;
                EnergyPerTick = 0;
                Credit = 100;
                CreditPerTick = 0;
                HomeWorld = null;
                SelectedUnits = new List<Unit>();
                Planets = new List<Planet>();
                Units = new List<Unit>();
                Fleets = new List<Fleet>();
                TechTree = ObjectCopier.Clone(techTree);
                FleetTemplates = new List<FleetTemplate>();

                bool foundHome = false;
                while (!foundHome)
                {
                    int startingSystem = random.Next(0, planetarySystems.Count);
                    int startingPlanet = random.Next(0, planetarySystems[startingSystem].Planets.Count);
                    if (planetarySystems[startingSystem].Planets[startingPlanet].Owner == defaultEmpire)
                    {
                        foundHome = true;
                        planetarySystems[startingSystem].Planets[startingPlanet].Owner = this;
                        Planet home = planetarySystems[startingSystem].Planets[startingPlanet];
                        home.ColonisationStatus = ColonisationStatus.HomeWorld;
                        Planets.Add(home);
                        HomeWorld = home;
                    }
                }
                CurrentTech = TechTree.Nodes[0];

                if (AI != null) AI.Start(this);
            }
        }

        // Tick
        public void Tick(float millis)
        {
            CalculateHappiness();
            CalculateIncomeThisTick();
            TickIncome();
            TickScience();
            TickUnits(millis);
            if (AI != null) AI.Tick(millis);
        }
        private void TickUnits(float millis)
        {
            foreach (Unit u in Units)
            {
                u.Tick(millis);
            }
        }
        private void TickIncome()
        {
            Energy += EnergyPerTick;
            Credit += CreditPerTick;
        }
        private void TickScience()
        {
            if (CurrentTech != null && !CurrentTech.Unlocked)
            {
                CurrentTech.Progress += Convert.ToInt32(SciencePerTick);
                if (CurrentTech.Progress >= CurrentTech.Cost)
                {
                    CurrentTech.Progress = CurrentTech.Cost;
                    CurrentTech.Unlocked = true;
                    CurrentTech = null;
                }
            }
        }
        private void CalculateHappiness()
        {
            //ColonialHappiness = 0.5f;
            //NativeHappiness = 0.5f;
            //int colonies = 0;
            //int nativeGroups = 0;
            //foreach (Planet p in Planets)
            //{
            //    if (p.ColonisationStatus == ColonisationStatus.Natives)
            //    {
            //        ColonialHappiness += p.ColonialHappiness * p.ColonialPopulation;
            //        colonies += p.ColonialPopulation;
            //    }
            //    if (p.NativePopulation != 0)
            //    {
            //        NativeHappiness += p.NativeHappiness * p.NativePopulation;
            //        nativeGroups += p.NativePopulation;
            //    }
            //}
            //ColonialHappiness /= colonies;
            //NativeHappiness /= nativeGroups;
        }
        private void CalculateIncomeThisTick()
        {
            SciencePerTick = 0;
            EnergyPerTick = 0;
            CreditPerTick = 0;
            foreach (Planet p in Planets)
            {
                SciencePerTick += p.SciencePerTick;
                EnergyPerTick += p.EnergyPerTick;
                CreditPerTick += p.CreditPerTick;
            }
        }
    
        // Update
        public void Update(GameTime gameTime, bool guiInteracted)
        {
            foreach (Unit u in Units)
            {
                u.Update(gameTime, guiInteracted);
            }
        }

        // Draw
        public void Draw()
        {
            DrawUnits();
        }
        private void DrawUnits()
        {
            foreach (Unit u in Units)
            {
                u.Draw();
            }
        }
    }
}
