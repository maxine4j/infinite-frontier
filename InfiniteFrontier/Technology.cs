using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Arwic.InfiniteFrontier
{
    [Serializable()]
    public class TechNode
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Cost")]
        public int Cost { get; set; }
        public int Progress { get; set; }
        public bool Unlocked { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("GridX")]
        public int GridX { get; set; }
        [XmlElement("GridY")]
        public int GridY { get; set; }
        [XmlElement("Prerequisite")]
        public List<int> Prerequisites { get; set; }
        [XmlElement("Mod_Production")]
        public float Mod_Production { get; set; }
        [XmlElement("Mod_Science")]
        public float Mod_Science { get; set; }
        [XmlElement("Mod_Credit")]
        public float Mod_Credit { get; set; }
        [XmlElement("Mod_Energy")]
        public float Mod_Energy { get; set; }

        public TechNode()
        {
            Prerequisites = new List<int>();
            Name = "DEFAULT_NAME";
            Cost = 0;
            Progress = 0;
            Unlocked = false;
            Description = "DEFAULT_DESCRIPTION";
            Mod_Credit = 1f;
            Mod_Energy = 1f;
            Mod_Production = 1f;
            Mod_Science = 1f;
        }
    }

    [Serializable()]
    public class TechTree
    {
        public static string FilePath = "Content/Data/TechnologyList.xml";

        [XmlElement("Node")]
        public List<TechNode> Nodes { get; set; }

        public TechTree()
        {
            Nodes = new List<TechNode>();
        }

        public static TechTree Load()
        {
            Main.console.WriteLine("Loading technologies", MsgType.Info);
            XmlSerializer xmls = new XmlSerializer(typeof(TechTree));
            try
            {
                using (XmlReader reader = XmlReader.Create(File.OpenRead(FilePath), new XmlReaderSettings() { CloseInput = true }))
                {
                    TechTree t = (TechTree)xmls.Deserialize(reader);
                    reader.Close();
                    return t;
                }
            }
            catch (Exception)
            {
                Main.console.WriteLine("Failed to load tech tree", MsgType.Failed);
                return null;
            }
        }
    }
}
