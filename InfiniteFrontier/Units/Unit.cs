using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    [Serializable()]
    public class UnitTemplate : IProducible
    {
        public ProductionType ProductionType { get; set; }

        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Cost")]
        public int Cost { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("IconIndex")]
        public int IconIndex { get; set; }

        [XmlElement("GraphicIndex")]
        public int GraphicIndex { get; set; }

        [XmlElement("Size")]
        public int Size { get; set; }

        [XmlElement("PrerequisiteTech")]
        public List<int> PrerequisiteTech { get; set; }
        [XmlElement("PrerequisiteBuilding")]
        public List<int> PrerequisiteBuilding { get; set; }

        [XmlElement("Command")]
        public List<int> Commands { get; set; }

        [XmlElement("HitPoints")]
        public int HitPoints { get; set; }
        [XmlElement("Attack")]
        public int Attack { get; set; }
        [XmlElement("Defense")]
        public int Defense { get; set; }
        [XmlElement("Movement")]
        public int Movement { get; set; }

        public UnitTemplate()
        {
            ProductionType = ProductionType.Unit;
            
            ID = 0;
            Name = "DEFAULT_UNIT_NAME";
            Cost = 0;
            Description = "DEFAULT_UNIT_DESCRIPTION";

            IconIndex = 0;

            GraphicIndex = 0;

            Size = 0;

            PrerequisiteTech = new List<int>();
            PrerequisiteBuilding = new List<int>();

            Commands = new List<int>();

            HitPoints = 0;
            Attack = 0;
            Defense = 0;
            Movement = 0;
        }
    }

    [Serializable()]
    public class UnitManager
    {
        public static string FilePath = "Content/Data/UnitList.xml";

        [XmlElement("Unit")]
        public List<UnitTemplate> Units { get; set; }

        public static UnitManager Load()
        {
            console.WriteLine("Loading unit list", MsgType.Info);
            XmlSerializer xmls = new XmlSerializer(typeof(UnitManager));
            try
            {
                using (XmlReader reader = XmlReader.Create(File.OpenRead(UnitManager.FilePath), new XmlReaderSettings() { CloseInput = true }))
                {
                    UnitManager u = (UnitManager)xmls.Deserialize(reader);
                    reader.Close();
                    return u;
                }
            }
            catch (Exception)
            {
                console.WriteLine("Failed to load unit list", MsgType.Failed);
                return null;
            }
        }
    }

    public class Unit : IOwnable
    {
        public Empire Owner { get; set; }
        
        public int ID { get; set; }
        public string Name { get; set; }
        public int IconIndex { get; set; }
        public SpriteAtlas IconAtlas { get; set; }
        public int GraphicIndex { get; set; }
        public Transform Transform { get; set; }
        public List<int> Commands { get; set; }
        public List<IUnitAura> Auras { get; set; }

        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Movement { get; set; }

        private LinkedList<Vector2> moveList = new LinkedList<Vector2>();
        private Queue<Vector2> movesThisTick = new Queue<Vector2>();
        private Vector2? currentMove = null;

        public static Unit Create(Empire owner, UnitTemplate ut)
        {
            Unit u = new Unit();

            u.Owner = owner;

            u.ID = ut.ID;
            u.Name = ut.Name;

            u.IconIndex = ut.IconIndex;
            u.IconAtlas = iconAtlas;

            u.GraphicIndex = ut.GraphicIndex;

            u.Transform = new Transform(0, 0, ut.Size, ut.Size);
            u.Transform.RotationOrigin = new Vector2(ut.Size);
            u.Commands = ut.Commands;
            u.Auras = new List<IUnitAura>();
            u.HP = ut.HitPoints;
            u.MaxHP = ut.HitPoints;
            u.Attack = ut.Attack;
            u.Defense = ut.Defense;
            u.Movement = ut.Movement;

            return u;
        }

        // Accessors
        public void ClearMoves()
        {
            moveList.Clear();
        }
        public void QueuMove(Vector2 move)
        {
            moveList.AddLast(move);
        }

        // Tick
        public void Tick(float millis)
        {
            TickMovement(millis);
            foreach (IUnitCommand a in Auras)
            {
                a.Execute(this);
            }
        }
        private void TickMovement(float millis)
        {
            CalcMove(Movement * (millis / 1000), millis, Transform.Translation);
        }
        private void CalcMove(float dist, float millis, Vector2 last)
        {
            if (moveList.Count > 0)
            {
                Vector2 nextMove = moveList.First.Value;
                Vector2 sep = nextMove - last;
                float sepLength = sep.Length();
                if (sepLength < dist)
                {
                    float diff = dist - sepLength;
                    movesThisTick.Enqueue(nextMove);
                    moveList.RemoveFirst();
                    CalcMove(diff, millis, nextMove);
                }
                else
                {
                    sep.Normalize();
                    sep *= dist;
                    Vector2 newMove = last + sep;
                    movesThisTick.Enqueue(newMove);
                    dist = 0;
                }
            }
        }

        // Update
        public void Update(GameTime gameTime, bool guiInteracted)
        {
            UpdateMovement(gameTime);
            if (!guiInteracted) 
                UpdateSelection();
        }
        private void UpdateSelection()
        {
            if (Owner == player)
            {
                if (Transform.Contains(InputUtil.MouseWorldPos) && InputUtil.LeftMouseButton)
                {
                    if (Keyboard.GetState().IsKeyUp(config.inp_queueOrder))
                        player.SelectedUnits.Clear();
                    player.SelectedUnits.Add(this);
                }
            }
        }
        public void UpdateSelected(bool guiInteracted)
        {
            if (!guiInteracted)
            {
                if (InputUtil.WasRightMouseButton)
                {
                    Vector2 dest = InputUtil.MouseWorldPos;
                    if (!InputUtil.IsKeyDown(config.inp_queueOrder))
                    {
                        moveList.Clear();
                    }
                    moveList.AddLast(dest);
                }
            }
        }
        private void UpdateMovement(GameTime gameTime)
        {
            if (currentMove == null)
            {
                if (movesThisTick.Count > 0)
                {
                    currentMove = (Vector2?)movesThisTick.Dequeue();
                }
            }
            else
            {
                Vector2 sep = (Vector2)currentMove - Transform.Translation;
                float sepLen = sep.Length();
                sep.Normalize();
                Transform.Rotation = GraphicsUtil.Angle(-Vector2.UnitY, sep);
                float mag = Movement * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 inc = new Vector2(sep.X * mag, sep.Y * mag);
                if (inc.Length() >= sepLen)
                    inc = new Vector2(sep.X * sepLen, sep.Y * sepLen);
                Transform.Translation = new Vector2(Transform.Translation.X + inc.X, Transform.Translation.Y + inc.Y);
                if (Transform.Translation == currentMove)
                    currentMove = null;
            }
        }

        // Draw
        public void Draw()
        {
            unitAtlas.Draw(sbFore, GraphicIndex, Transform);
            if (drawLabels) sbFore.DrawString(fonts[(int)Font.InfoText], Name + " " + Transform.Translation, Transform.Translation, Color.White);
            DrawMovement();
            DrawBoundingBox(Color.Gray);
        }
        public void DrawBoundingBox(Color c)
        {
            GraphicsUtil.DrawRect(sbGUI, camera.ConvertWorldToGUI(Transform.Bounds), 2, c);
        }
        public void DrawMovement()
        {
            int thickness = 1;

            if (movesThisTick.Count > 0)
            {
                // draw the moves this tick
                for (int i = 0; i < movesThisTick.Count - 1; i++)
                {
                    GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(movesThisTick.ToArray()[i]), camera.ConvertWorldToGUI(movesThisTick.ToArray()[i + 1]), thickness, Color.Gray);
                }
            }
            else if (currentMove != null && moveList.Count > 0)
            {
                GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(moveList.ToArray()[0]), camera.ConvertWorldToGUI((Vector2)currentMove), thickness, Color.Gray);
            }
            else if (moveList.Count > 0)
            {
                GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(Transform.Translation), camera.ConvertWorldToGUI(moveList.ToArray()[0]), thickness, Color.Gray);
            }

            if (moveList.Count > 0)
            {
                if (movesThisTick.Count > 0)
                {
                    GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(movesThisTick.ToArray()[movesThisTick.Count - 1]), camera.ConvertWorldToGUI(moveList.ToArray()[0]), thickness, Color.Gray);
                }
                // draw the move queue
                for (int i = 0; i < moveList.Count - 1; i++)
                {
                    GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(moveList.ToArray()[i]), camera.ConvertWorldToGUI(moveList.ToArray()[i + 1]), thickness, Color.Gray);
                }
            }
            // draw the current part of the move this tick
            if (currentMove != null)
            {
                if (movesThisTick.Count > 0)
                    GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(movesThisTick.ToArray()[0]), camera.ConvertWorldToGUI((Vector2)currentMove), thickness, Color.Gray);
                GraphicsUtil.DrawLine(sbGUI, camera.ConvertWorldToGUI(Transform.Translation), camera.ConvertWorldToGUI((Vector2)currentMove), thickness, Color.Gold);
            }
        }
    }
}
