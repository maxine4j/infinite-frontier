using Microsoft.Xna.Framework;
using System.Collections.Generic;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class AI
    {
        private interface IAction
        {
            int Priority { get; set; }
            AI AI { get; set; }

            void Tick();
        }

        private class AIProduce : IAction
        {
            public int Priority { get; set; }
            public AI AI { get; set; }

            public AIProduce(AI ai, int priority)
            {
                AI = ai;
                Priority = priority;
            }

            public void Tick()
            {
                CheckPlanets();
            }

            private Production GetProduction(Planet p)
            {
                return null;
            }

            private void CheckPlanets()
            {
                foreach (Planet p in AI.Empire.Planets)
                {
                    if (p.CurrentProduction == null)
                    {
                        p.ProductionQueue.Enqueue(GetProduction(p));
                    }
                }
            }
        }

        private class AIExpand : IAction
        {
            private enum State
            {
                Null,
                PickPlanet,
                WaitProduction,
                MoveToPlanet,
                Colonise
            }

            public int Priority { get; set; }
            public AI AI { get; set; }

            private State state;
            private PlanetarySystem closestSystem;
            private Planet destPlanet, sourcePlanet;
            private Unit coloniser;

            public AIExpand(AI ai, int priority)
            {
                AI = ai;
                Priority = priority;
            }

            public void Tick()
            {
                switch (state)
                {
                    case State.Null:
                        if (AI.desiredPlanetCount > AI.Empire.Planets.Count)
                            state = State.PickPlanet;
                        break;
                    case State.PickPlanet:      PickPlanet(); break;
                    case State.WaitProduction:  WaitForProduction(); break;
                    case State.MoveToPlanet:    MoveToPlanet(); break;
                    case State.Colonise:        Colonise(); break;
                }
            }

            private void PickPlanet()
            {
                float minDist = float.MaxValue;
                foreach (PlanetarySystem ps in planetarySystems)
                {
                    foreach (Planet psp in ps.Planets)
                    {
                        foreach (Planet myp in AI.Empire.Planets)
                        {
                            if (psp != myp && psp.Owner != AI.Empire)
                            {
                                Vector2 sep = psp.Transform.Translation - myp.Transform.Translation;
                                float seplen = sep.Length();
                                if (seplen < minDist)
                                {
                                    minDist = seplen;
                                    destPlanet = psp;
                                    sourcePlanet = myp;
                                }
                            }
                        }
                    }
                }

                if (coloniser == null)
                {
                    //camera.LookAt(sourcePlanet.Transform.Translation);
                    sourcePlanet.ProductionQueue.Enqueue(new Production(unitManager.Units[(int)UnitAtlas.Coloniser]));
                    state = State.WaitProduction;
                }
            }
            private void WaitForProduction()
            {
                foreach (Unit u in AI.Empire.Units)
                {
                    if (u.ID == (int)UnitAtlas.Coloniser)
                    {
                        coloniser = u;
                        state = State.MoveToPlanet;
                    }
                }
            }
            private void MoveToPlanet()
            {
                if (destPlanet.Owner == Main.defaultEmpire)
                {
                    coloniser.ClearMoves();
                    coloniser.QueuMove(destPlanet.Transform.Translation);
                    Vector2 sep = coloniser.Transform.Translation - destPlanet.Transform.Translation;
                    float length = sep.Length();
                    if (length <= destPlanet.Transform.Bounds.Width)
                    {
                        state = State.Colonise;
                    }
                }
                else
                {
                    state = State.PickPlanet;
                }
            }
            private void Colonise()
            {
                if (destPlanet.Owner == defaultEmpire)
                {
                    destPlanet.Owner = AI.Empire;
                    destPlanet.ColonialPopulation = 1;
                    AI.Empire.Planets.Add(destPlanet);
                    AI.Empire.Units.Remove(coloniser);
                    state = State.Null;
                    coloniser = null;
                }
                else
                {
                    state = State.PickPlanet;
                }
            }
        }

        public Empire Empire;

        private Resource desiredResource;
        private int desiredPlanetCount = 3;
        private List<IAction> actions;

        //private float d_war, d_economy, d_expansion,
        //    p_reasonable, p_;

        public AI()
        {
            desiredResource = Resource.Food;
            actions = new List<IAction>();
            actions.Add(new AIExpand(this, 0));
            actions.Add(new AIProduce(this, 0));
        }

        public void Start(Empire e)
        {
            Empire = e;
        }

        public void Tick(float millis)
        {
            foreach (IAction a in actions)
            {
                a.Tick();
            }
        }
    }
}
