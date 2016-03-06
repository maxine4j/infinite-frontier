using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public enum Terrain
    {
        Terrestrial,
        Aqueous,
        Gaseous
    }

    public enum Atmosphere
    {
        Oxygen,
        Sulphor,
        Nitrogen,
        Chlorine
    }

    public enum Resource
    {
        Food,
        Water,
        Alloy,
        Fuel
    }

    public enum ColonisationStatus
    {
        Natives,
        HomeWorld,
        NoNatives,
        NativesExterminated,
        NativesAssimilated,
        NativesIgnored
    }

    public class Planet : IOwnable
    {
        public Empire Owner { get; set; }
        public int ID { get; }
        public ColonisationStatus ColonisationStatus { get; set; }
        public PlanetarySystem PlanetarySystem { get; private set; }
        public int GraphicIndex { get; set; }
        public SpriteAtlas GraphicAtlas { get; set; }

        public Transform Transform { get; private set; }
        public string Name { get; set; }
        
        // orbit properties
        private double orbitSegmentAngle;
        private Vector2[] orbitPath;
        private int orbitPathIndex;
        private Timer orbitTimer;
        private bool showOrbit = false;
        private int orbitRadius;
        private bool orbitCW;
        private Vector2 orbitCentre;

        // Planetary properties
        public Terrain Terrain { get; set; }
        public Atmosphere Atmosphere { get; set; }
        public Resource Resource { get; set; }

        // Colonial properties
        public int ColonialPopulation { get; set; }
        public int ColonialHappiness { get; set; }
        public int ColonialStrength { get; set; }
        public Race ColonialRace { get; set; }

        // Native properties
        public int NativePopulation { get; set; }
        public float NativeHostility { get; set; }
        public int NativeStrength { get; set; }
        public Race NativeRace { get; set; }
        public float NativeHappiness { get; set; }

        // Income
        public int ProductionPerTick { get; set; }
        public int EnergyPerTick { get; set; }
        public int SciencePerTick { get; set; }
        public int CreditPerTick { get; set; }

        // Production
        public List<Building> BuildingList { get; set; }
        public Queue<Production> ProductionQueue { get; set; }
        public Production CurrentProduction { get; set; }
        public Production LastProduction { get; set; }

        public Planet(PlanetarySystem solarSystem, int planetRad, int orbitRad, double orbitSpeed, int planetNumber)
        {
            ID = planetNumber;
            PlanetarySystem = solarSystem;
            GenerateName(PlanetarySystem.Name, planetNumber);

            GenerateOrbit(orbitRad, orbitSpeed);
            GenerateNativeProps();
            GeneratePlanetProps();

            GraphicIndex = random.Next(0, 15);
            GraphicAtlas = planetAtlas;
            Transform = new Transform((orbitCentre.X + orbitPath[orbitPathIndex].X) - planetRad, (orbitCentre.Y + orbitPath[orbitPathIndex].Y) - planetRad, planetRad * 2, planetRad * 2);
            ColonialPopulation = random.Next(0, 4);
            BuildingList = new List<Building>();
            ProductionQueue = new Queue<Production>();
            Owner = defaultEmpire;

            ProductionPerTick = 10;
        }

        // Accessors
        public List<Production> GetPossibleProductions()
        {
            List<Production> items = new List<Production>();
            // add units to production list
            foreach (UnitTemplate ut in unitManager.Units)
            {
                int buildingsRequired = ut.PrerequisiteBuilding.Count;
                int buildingsUnlocked = 0;
                foreach (Building b in BuildingList)
                {
                    if (ut.PrerequisiteBuilding.Contains(b.ID))
                        buildingsUnlocked++;
                }
                int techRequired = ut.PrerequisiteTech.Count;
                int techUnlocked = 0;
                foreach (TechNode t in Owner.TechTree.Nodes)
                {
                    if (ut.PrerequisiteTech.Contains(t.ID))
                        techUnlocked++;
                }
                if (buildingsRequired == buildingsUnlocked && techRequired == techUnlocked)
                {
                    items.Add(new Production(ut));
                }
            }
            // add buildings to production list
            foreach (BuildingTemplate bt in buildingManager.Buildings)
            {
                bool built = false;
                if (bt.UniquePerPlanet)
                {
                    // Check if its already being built
                    if (CurrentProduction != null && CurrentProduction.ProductionType == ProductionType.Building)
                    {
                        BuildingTemplate res = (BuildingTemplate)CurrentProduction.Result;
                        if (res.ID == bt.ID)
                            built = true;
                    }
                    for (int i = 0; i < ProductionQueue.ToArray().Length; i++)
                    {
                        switch (ProductionQueue.ToArray()[i].Result.ProductionType)
                        {
                            case ProductionType.Building:
                                BuildingTemplate resB = (BuildingTemplate)ProductionQueue.ToArray()[i].Result;
                                if (resB.ID == bt.ID)
                                    built = true;
                                break;
                            case ProductionType.Unit:
                                UnitTemplate resU = (UnitTemplate)ProductionQueue.ToArray()[i].Result;
                                if (resU.ID == bt.ID)
                                    built = true;
                                break;
                        }
                    }

                    // Check if its already been built
                    foreach (Building b in BuildingList)
                    {
                        if (b.ID == bt.ID)
                        {
                            built = true;
                            break;
                        }
                    }
                }
                // TODO take into account game and planetary system uniqueness
                if (!built)
                {
                    int buildingsRequired = bt.PrerequisiteBuilding.Count;
                    int buildingsUnlocked = 0;
                    foreach (Building b in BuildingList)
                    {
                        if (bt.PrerequisiteBuilding.Contains(b.ID))
                            buildingsUnlocked++;
                    }
                    int techRequired = bt.PrerequisiteTech.Count;
                    int techUnlocked = 0;
                    foreach (TechNode t in player.TechTree.Nodes)
                    {
                        if (bt.PrerequisiteTech.Contains(t.ID))
                            techUnlocked++;
                    }
                    if (buildingsRequired == buildingsUnlocked && techRequired == techUnlocked)
                    {
                        items.Add(new Production(bt));
                    }
                }
            }
            return items;
        }
        public bool Colonise(Empire owner)
        {
            if (Owner == defaultEmpire)
            {
                Owner = owner;
                ColonialPopulation = 1;
                ColonialHappiness = 1;
                ColonialRace = Owner.Race;
                return true;
            }
            return false;
        }

        // Generate
        private void GeneratePlanetProps()
        {
            Terrain = (Terrain)random.Next(0, Enum.GetNames(typeof(Terrain)).Length);
            Atmosphere = (Atmosphere)random.Next(0, Enum.GetNames(typeof(Atmosphere)).Length);
            Resource = (Resource)random.Next(0, Enum.GetNames(typeof(Resource)).Length);
        }
        private void GenerateNativeProps()
        {
            NativePopulation = random.Next(-1, 5);
            if (NativePopulation < 0) NativePopulation = 0;

            if (NativePopulation != 0)
                ColonisationStatus = ColonisationStatus.Natives;
            else
                ColonisationStatus = ColonisationStatus.NoNatives;

            if (NativePopulation == 0) NativeHostility = 0;
            else NativeHostility = random.Next(0, 100) / 100f;
            
            if (NativePopulation == 0) NativeStrength = 0;
            else NativeStrength = random.Next(0, 100);
            
            if (NativePopulation == 0) NativeRace = Race.Null;
            else NativeRace = (Race)random.Next(1, Enum.GetNames(typeof(Race)).Length);

            if (NativePopulation == 0) NativeHappiness = 0;
            NativeHappiness = (float)random.Next(0, 100) / 100f;
        }
        private void GenerateOrbit(int orbitRad, double orbitSpeed)
        {
            orbitCW = PlanetarySystem.OrbitCW;
            orbitCentre = PlanetarySystem.Transform.Translation + PlanetarySystem.Transform.Size / 2;
            orbitTimer = new Timer((long)orbitSpeed);
            orbitTimer.Start();
            orbitRadius = orbitRad;
            orbitSegmentAngle = 2 * Math.PI / config.gfx_orbitSmoothness;
            orbitPathIndex = random.Next(0, config.gfx_orbitSmoothness);
            orbitPath = new Vector2[config.gfx_orbitSmoothness];
            for (int i = 0; i < config.gfx_orbitSmoothness; i++)
            {
                orbitPath[i] = new Vector2((float)Math.Sin(orbitSegmentAngle * i) * orbitRadius, (float)Math.Cos(orbitSegmentAngle * i) * orbitRadius);
            }
        }
        private void GenerateName(string systemName, int planetNumber)
        {
            //string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string[] romanNumurals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI" };
            Name = systemName + " " + romanNumurals[planetNumber];
        }

        // Tick
        public void Tick(float millis)
        {
            if (Owner.ID != 0)
            {
                TickProduction(millis);
                TickIncome(millis);
            }
        }
        private void TickProduction(float millis)
        {
            // check if we should start a new production item
            if (CurrentProduction == null)
            {
                if (ProductionQueue.Count > 0)
                    CurrentProduction = ProductionQueue.Dequeue();
            }
            else // update the current production item
            {
                CurrentProduction.Progress += ProductionPerTick;
                if (CurrentProduction.Progress >= CurrentProduction.Cost)
                {
                    switch (CurrentProduction.ProductionType)
                    {
                        case ProductionType.Building:
                            Building b = Building.Create(Owner, (BuildingTemplate)CurrentProduction.Result);
                            BuildingList.Add(b);
                            break;
                        case ProductionType.Unit:
                            Unit u = Unit.Create(Owner, (UnitTemplate)CurrentProduction.Result);
                            u.Transform.Translation = Transform.Translation;
                            Owner.Units.Add(u);
                            break;
                    }
                    LastProduction = CurrentProduction;
                    CurrentProduction = null;
                }
            }
        }
        private void TickIncome(float millis)
        {
            ProductionPerTick = 1;
            EnergyPerTick = 0;
            SciencePerTick = 0;
            CreditPerTick = 0;

            foreach (Building b in BuildingList)
            {
                ProductionPerTick += b.ProductionPerTick;
                EnergyPerTick += b.EnergyPerTick;
                SciencePerTick += b.SciencePerTick;
                CreditPerTick += b.CreditPerTick;
            }
        }

        // Update
        public void Update(GameTime gameTime, bool guiInteracted)
        {
            UpdateOrbit(gameTime);

            if (!guiInteracted)
            {
                if (Transform.Contains(InputUtil.MouseWorldPos))
                {
                    showOrbit = true;
                    if (InputUtil.LeftMouseButton && Owner == player)
                        interfaceManager.GUIPlanetManagment.SetUpForm(this);
                }
                else
                    showOrbit = false;
            }
            else
                showOrbit = false;
        }
        private void UpdateOrbit(GameTime gameTime)
        {
            orbitTimer.Update(gameTime);
            if (orbitTimer.isExpired())
            {
                Transform.Translation = orbitCentre + orbitPath[orbitPathIndex] - Transform.Size / 2;

                if (!orbitCW)
                {
                    orbitPathIndex++;
                    if (orbitPathIndex > orbitPath.Length - 1)
                        orbitPathIndex = 0;
                }
                else
                {
                    orbitPathIndex--;
                    if (orbitPathIndex < 0)
                        orbitPathIndex = orbitPath.Length - 1;
                }
                orbitTimer.Start();
            }
        }

        // Draw
        public void Draw()
        {
            if (showOrbit)
                DrawOrbit(Color.CornflowerBlue);
            planetAtlas.Draw(sbFore, GraphicIndex, Transform);
            if (Owner != defaultEmpire)
            {
                Vector2 translation = camera.ConvertWorldToGUI(Transform.Translation);
                sbGUI.DrawString(fonts[(int)Font.InfoText], $"Owned by: {Owner.Name}", new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 55, translation.Y - 35), Color.White);
            }
        }
        public void DrawInfo()
        {
            Vector2 translation = camera.ConvertWorldToGUI(Transform.Translation);

            planetLabelBack.DrawNineCut(sbGUI, new Transform(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 100, translation.Y - 40, 200, 40));
            sbGUI.DrawString(fonts[(int)Font.InfoText], Name, new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 55, translation.Y - 35), Color.White); // name
            sbGUI.DrawString(fonts[(int)Font.InfoText], ColonialPopulation.ToString() + " - ", new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 95, translation.Y - 35), Color.White); // population
            if (CurrentProduction != null)
                sbGUI.DrawString(fonts[(int)Font.InfoText], "Producing: " + CurrentProduction.Result.Name, new Vector2(translation.X + Transform.Size.X / 2 * camera.Transform.Scale.X - 95, translation.Y - 15), Color.White, 0.0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0.0f); // production
        }
        public void DrawOrbit(Color c)
        {
            //Transform t = Transform.CreateCircle(Transform.Translation + Transform.Size / 2, Transform.Radius);
            GraphicsUtil.DrawCircle(sbGUI, camera.ConvertWorldToGUI(orbitCentre), (int)(orbitRadius * camera.Transform.Scale.X), 100, 1, c);
            //GraphicsUtil.DrawCircle(sbGUI, camera.ConvertWorldToGUI(t.Translation), (int)(t.Radius * camera.Transform.Scale.X), 100, 1, c);
        }
    }
}
