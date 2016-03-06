using System;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class EngineCall
    {
        /// <summary>
        /// Queues a production item at a planet
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="prod"></param>
        public static void QueueProduction(Planet planet, Production prod)
        {
            if (planet.ProductionQueue.Count == 0 && planet.CurrentProduction == null)
                planet.CurrentProduction = prod;
            else
                planet.ProductionQueue.Enqueue(prod);
        }

        /// <summary>
        /// Changes the current production item of a planet
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="prod"></param>
        public static void ChangeProduction(Planet planet, Production prod)
        {
            planet.CurrentProduction = prod;
        }

        /// <summary>
        /// Stops the current production of a planet
        /// </summary>
        public static void CancelProduction(Planet planet, bool clearQueue = false)
        {
            planet.CurrentProduction = null;
            if (clearQueue)
                planet.ProductionQueue.Clear();
        }

        public static void ProduceFleet(FleetTemplate ft)
        {
            // TODO DEBUG
            ft.Units.Add(unitManager.Units[0]);
            ft.Units.Add(unitManager.Units[1]);
            ft.Units.Add(unitManager.Units[2]);
            ft.Units.Add(unitManager.Units[3]);
            Fleet f = Fleet.Create(player, ft);
            player.Fleets.Add(f);
        }

        /// <summary>
        /// Selects the given unit
        /// </summary>
        /// <param name="u"></param>
        public static void SelectUnit(Unit u)
        {
            player.SelectedUnits.Clear();
            player.SelectedUnits.Add(u);
        }

        /// <summary>
        /// Selects the given units
        /// </summary>
        /// <param name="u"></param>
        public static void SelectUnit(List<Unit> u)
        {
            player.SelectedUnits.AddRange(u);
        }

        /// <summary>
        /// Issues a command to the given unit
        /// </summary>
        /// <param name="u"></param>
        /// <param name="cmd"></param>
        public static void CommandUnit(Unit u, int cmd)
        {
            unitCommandManager.CallCommand(u, cmd);
        }

        /// <summary>
        /// Exits the game
        /// </summary>
        public static void Exit()
        {
            config.SaveConfig();
            Environment.Exit(0);
        }

        /// <summary>
        /// Ticks the simulation forward
        /// </summary>
        public static void Tick()
        {
            simulationManager.Tick(tick);
        }

        /// <summary>
        /// Starts the game engine with initial values
        /// </summary>
        public static void StartGame()
        {
            simulationManager.StartGame();
        }

        /// <summary>
        /// Resets config file to defaults
        /// </summary>
        public static void ResetConfig()
        {
            config.RestoreDefaults();
        }

        //public static void 
    }
}
