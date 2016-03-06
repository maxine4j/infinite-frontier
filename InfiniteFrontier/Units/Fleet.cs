using Microsoft.Xna.Framework;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class FleetTemplate
    {
        public List<UnitTemplate> Units { get; set; }
        public List<Vector2> Positions { get; set; }
        public string Name { get; set; }

        public FleetTemplate(string name, List<UnitTemplate> units, List<Vector2> positions)
        {
            Name = name;
            Units = units;
            Positions = positions;
        }
    }

    public class Fleet : IOwnable
    {
        public Empire Owner { get; set; }
        public List<Unit> Units { get; set; }
        public string Name { get; set; }

        public static Fleet Create(Empire e, FleetTemplate template)
        {
            Fleet res = new Fleet();
            res.Units = new List<Unit>();
            res.Name = template.Name;
            res.Owner = e;
            foreach (UnitTemplate ut in template.Units)
            {
                res.Units.Add(Unit.Create(player, ut));
            }
            return res;
        }

        // Generate
        private string GenerateName(int fleetCount)
        {
            string[] romanNumurals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI" };
            if (fleetCount < romanNumurals.Length)
                return Owner.Name + " " + romanNumurals[fleetCount];
            else
                return Owner.Name;
        }

        // Accessors
        public void Select()
        {
            player.SelectedUnits = Units;
        }
    }
}
