using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    [Serializable()]
    public class BuildingTemplate : IProducible
    {
        public ProductionType ProductionType { get; set; }

        [XmlElementAttribute("ID")]
        public int ID { get; set; }
        [XmlElementAttribute("Name")]
        public string Name { get; set; }
        [XmlElementAttribute("Cost")]
        public int Cost { get; set; }
        [XmlElementAttribute("Description")]
        public string Description { get; set; }

        [XmlElementAttribute("IconIndex")]
        public int IconIndex { get; set; }

        [XmlElementAttribute("UniquePerPlanet")]
        public bool UniquePerPlanet { get; set; }
        
        [XmlElementAttribute("UniquePerSystem")]
        public bool UniquePerSystem { get; set; }

        [XmlElementAttribute("UniquePerGame")]
        public bool UniquePerGame { get; set; }

        [XmlElementAttribute("PrerequisiteTech")]
        public List<int> PrerequisiteTech { get; set; }
        [XmlElementAttribute("PrerequisiteBuilding")]
        public List<int> PrerequisiteBuilding { get; set; }

        [XmlElementAttribute("ProductionPerTick")]
        public int ProductionPerTick { get; set; }
        [XmlElementAttribute("CreditPerTick")]
        public int CreditPerTick { get; set; }
        [XmlElementAttribute("SciencePerTick")]
        public int SciencePerTick { get; set; }
        [XmlElementAttribute("EnergyPerTick")]
        public int EnergyPerTick { get; set; }

        public BuildingTemplate()
        {
            ProductionType = ProductionType.Building;

            ID = 0;
            Name = "DEFAULT_NAME";
            Cost = 0;
            Description = "DEFAULT_DESCRIPTION";

            IconIndex = 0;

            UniquePerPlanet = true;
            UniquePerSystem = false;
            UniquePerGame = false;

            PrerequisiteTech = new List<int>();
            PrerequisiteBuilding = new List<int>();

            CreditPerTick = 0;
            SciencePerTick = 0;
            EnergyPerTick = 0;
        }
    }

    [Serializable()]
    public class BuildingManager
    {
        public static string FilePath = "Content/Data/BuildingList.xml";

        [XmlElementAttribute("Building")]
        public List<BuildingTemplate> Buildings { get; set; }

        public static BuildingManager Load()
        {
            console.WriteLine("Loading building list", MsgType.Info);
            XmlSerializer xmls = new XmlSerializer(typeof(BuildingManager));
            try
            {
                using (XmlReader reader = XmlReader.Create(File.OpenRead(BuildingManager.FilePath), new XmlReaderSettings() { CloseInput = true }))
                {
                    BuildingManager b = (BuildingManager)xmls.Deserialize(reader);
                    reader.Close();
                    return b;
                }
            }
            catch (Exception)
            {
                console.WriteLine("Failed to load building list", MsgType.Failed);
                return null;
            }
        }
    }

    public class Building : IOwnable
    {
        public Empire Owner { get; set; }

        public int ID { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Description { get; set; }

        public int IconIndex { get; set; }

        public bool UniquePerPlanet { get; set; }
        public bool UniquePerSystem { get; set; }
        public bool UniquePerGame { get; set; }

        public List<int> PrerequisiteTech { get; set; }
        public List<int> PrerequisiteBuilding { get; set; }

        public int ProductionPerTick { get; set; }
        public int CreditPerTick { get; set; }
        public int SciencePerTick { get; set; }
        public int EnergyPerTick { get; set; }

        public Building(Empire owner)
        {
            if (owner == null) Owner = defaultEmpire;
            else Owner = owner;
        }

        public static Building Create(Empire owner, BuildingTemplate ut)
        {
            Building b = new Building(owner);
            b.ID = ut.ID;
            b.Name = owner.Adjective + ut.Name;

            b.IconIndex = ut.IconIndex;

            b.UniquePerGame = ut.UniquePerGame;
            b.UniquePerPlanet = ut.UniquePerPlanet;
            b.UniquePerSystem = ut.UniquePerSystem;

            b.CreditPerTick = ut.CreditPerTick;
            b.SciencePerTick = ut.SciencePerTick;
            b.EnergyPerTick = ut.EnergyPerTick;
            return b;
        }
    }
}
