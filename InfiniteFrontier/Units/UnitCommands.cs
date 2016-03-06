using Microsoft.Xna.Framework;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public interface IUnitCommand
    {
        Cursors Cursor { get; }
        string Name { get; }
        string Description { get; }
        Icon Icon { get; }
        UnitCommandType CommandType { get; }

        void Execute(Unit me, Unit u = null, Planet p = null, Vector2? pos = null);
    }

    public interface IUnitAura
    {
        Unit unit { get; }

        void Tick(float millis);
        void Update(GameTime gameTime);
        void Draw();
    }

    public class UnitCommandManager
    {
        private List<IUnitCommand> commands = new List<IUnitCommand>();
        private IUnitCommand selectedCommand;
        private IUnitCommand lastSelectedCommand;
        private Unit selectedUnit;
        private bool recentlyCalled = false;

        public UnitCommandManager()
        {
            commands.Add(new UnitCommand_Move());
            commands.Add(new UnitCommand_Disband());
            commands.Add(new UnitCommand_Colonise());
        }

        public Icon GetCommandIcon(int cmd)
        {
            if (cmd >= 0 && cmd < commands.Count)
                return commands[cmd].Icon;
            return Icon.Gear;
        }

        public void CallCommand(Unit selected, int index)
        {
            if (index >= 0 && index < commands.Count)
            {
                IUnitCommand cmd = commands[index];
                selectedUnit = selected;
                switch (cmd.CommandType)
                {
                    case UnitCommandType.TargetLocation:
                    case UnitCommandType.TargetUnit:
                    case UnitCommandType.TargetPlanet:
                        CursorUtil.Set(commands[index].Cursor);
                        selectedCommand = cmd;
                        selectedUnit = player.SelectedUnits[0];
                        break;
                    case UnitCommandType.Aura:
                    case UnitCommandType.Instant:
                        cmd.Execute(selectedUnit, null, null);
                        break;
                }
            }
            recentlyCalled = true;
        }

        public void Update(bool guiInteracted)
        {
            if (InputUtil.WasLeftMouseButton && selectedCommand != null)
            {
                if (recentlyCalled)
                {
                    recentlyCalled = false;
                }
                else if (!guiInteracted)
                {
                    switch (selectedCommand.CommandType)
                    {
                        case UnitCommandType.TargetLocation:
                            selectedCommand.Execute(selectedUnit, null, null, InputUtil.MouseWorldPos);
                            console.WriteLine($"UNITCOMMAND:{selectedCommand.Name} UNIT:{selectedUnit.Name} TARGET:{InputUtil.MouseWorldPos}");
                            break;
                        case UnitCommandType.TargetUnit:
                            foreach (Unit u in Units)
                            {
                                if (u.Transform.Contains(InputUtil.MouseWorldPos))
                                {
                                    selectedCommand.Execute(selectedUnit, u);
                                    console.WriteLine($"UNITCOMMAND:{selectedCommand.Name} UNIT:{selectedUnit.Name} TARGET:{u.Name}");
                                    break;
                                }
                            }
                            break;
                        case UnitCommandType.TargetPlanet:
                            foreach (Planet p in Planets)
                            {
                                if (p.Transform.Contains(InputUtil.MouseWorldPos))
                                {
                                    selectedCommand.Execute(selectedUnit, null, p);
                                    console.WriteLine($"UNITCOMMAND:{selectedCommand.Name} UNIT:{selectedUnit.Name} TARGET:{p.Name}");
                                    break;
                                }
                            }
                            break;
                    }
                    CursorUtil.Set(Cursors.Main);
                    selectedCommand = null;
                    selectedUnit = null;
                }
                else if (InputUtil.WasRightMouseButton)
                {
                    CursorUtil.Set(Cursors.Main);
                    selectedCommand = null;
                    selectedUnit = null;
                }
            }
            lastSelectedCommand = selectedCommand;
        }
    }

    public class UnitCommand_Move : IUnitCommand
    {
        public UnitCommandType CommandType { get; }
        public Cursors Cursor { get; }
        public string Name { get; }
        public string Description { get; }
        public Icon Icon { get; }

        public UnitCommand_Move()
        {
            Name = "Move";
            Description = "Moves unit to target location.";
            Icon = Icon.Arrow;
            CommandType = UnitCommandType.TargetLocation;
            Cursor = Cursors.Crosshairs;
        }

        public void Execute(Unit me, Unit u = null, Planet p = null, Vector2? pos = null)
        {
            me.ClearMoves();
            me.QueuMove(pos.Value);
        }
    }

    public class UnitCommand_Disband : IUnitCommand
    {
        public UnitCommandType CommandType { get; }
        public Cursors Cursor { get; }
        public string Name { get; }
        public string Description { get; }
        public Icon Icon { get; }

        public UnitCommand_Disband()
        {
            Name = "Disband";
            Description = "Dsibands the unit, destroying it.";
            Icon = Icon.Cross;
            CommandType = UnitCommandType.Instant;
            Cursor = Cursors.Main;
        }

        public void Execute(Unit me, Unit u = null, Planet p = null, Vector2? pos = null)
        {
            player.SelectedUnits.Clear();
            me.Owner.Units.Remove(me);
        }
    }

    public class UnitCommand_Colonise : IUnitCommand
    {
        public UnitCommandType CommandType { get; }
        public Cursors Cursor { get; }
        public string Name { get; }
        public string Description { get; }
        public Icon Icon { get; }

        public UnitCommand_Colonise()
        {
            Name = "Colonise Planet";
            Description = "Colonises the target planet for your empire.";
            Icon = Icon.Flag;
            CommandType = UnitCommandType.TargetPlanet;
            Cursor = Cursors.Crosshairs;
        }

        public void Execute(Unit me, Unit u = null, Planet p = null, Vector2? pos = null)
        {
            if (p.Owner == defaultEmpire && (me.Transform.Translation - p.Transform.Translation).Length() <= p.Transform.Bounds.Width)
            {
                p.Owner = me.Owner;
                p.ColonialPopulation = 1;
                me.Owner.Planets.Add(p);
                me.Owner.Units.Remove(me);
                player.SelectedUnits.Clear();
                CursorUtil.Set(Cursors.Main);
                console.WriteLine(me.Name + " cast ability settle");
            }
        }
    }

}