using Microsoft.Xna.Framework;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_Map : IGUIElement
    {
        public Form Form { get; set; }
        public Sprite Background { get; set; }
        public Transform Transform { get; set; }

        public GUI_Map()
        {
            Transform = new Transform(1496, 799, 407, 236);
            Background = Main.formBack;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            if (!interacted)
            {
                if (Transform.Contains(InputUtil.MouseScreenPos, Main.scale))
                {
                    if (InputUtil.LeftMouseButton)
                    {
                        Vector2 mousePos = InputUtil.MouseScreenPos;
                        Vector2 relMouse = new Vector2(mousePos.X / Main.scale - Transform.Translation.X, mousePos.Y / Main.scale - Transform.Translation.Y);
                        Vector2 pos = new Vector2((relMouse.X / (Transform.Bounds.Width / Main.scale) / Main.scale) * Main.mapBounds.Width + Transform.Bounds.Width / 2, (relMouse.Y / (Transform.Bounds.Height / scale) / scale) * mapBounds.Height + Transform.Bounds.Height / 2);
                        Main.camera.LookAt(pos);
                    }
                    return true;
                }
            }
            return false;
        }

        // Draw
        public void Draw()
        {
            DrawBackground();
            DrawSystems();
            DrawCameraRect();
        }
        private void DrawCameraRect()
        {
            Rectangle camRect = new Rectangle((int)((camera.Transform.Translation.X - viewport.Width / 2 / camera.Transform.Scale.X) / mapBounds.Width * Transform.Bounds.Width + Transform.Translation.X),
                                                (int)((camera.Transform.Translation.Y - viewport.Height / 2 / camera.Transform.Scale.Y) / mapBounds.Height * Transform.Bounds.Height + Transform.Translation.Y),
                                                (int)((1920.0f / mapBounds.Width) * (float)Transform.Bounds.Width / camera.Transform.Scale.X),
                                                (int)((1080.0f / mapBounds.Height) * (float)Transform.Bounds.Height / camera.Transform.Scale.Y));
            Vector2 vertA = new Vector2(camRect.X + camRect.Width / 2, Transform.Translation.Y);
            Vector2 vertB = new Vector2(camRect.X + camRect.Width / 2, Transform.Translation.Y + Transform.Bounds.Height);
            Vector2 horiA = new Vector2(Transform.Translation.X, camRect.Y + camRect.Height / 2);
            Vector2 horiB = new Vector2(Transform.Translation.X + Transform.Bounds.Width, camRect.Y + camRect.Height / 2);

            if (camRect.X < Transform.Translation.X)
            {
                camRect.Width -= (int)(Transform.Translation.X - camRect.X);
                camRect.X = (int)Transform.Translation.X;
            }
            if (camRect.Y < Transform.Translation.Y)
            {
                camRect.Height -= (int)(Transform.Translation.Y - camRect.Y);
                camRect.Y = (int)Transform.Translation.Y;
            }
            if (camRect.Height > Transform.Bounds.Bottom)
            {
                camRect.Height -= (int)Transform.Size.Y;
            }
            if (camRect.Width > Transform.Bounds.Right)
            {
                camRect.Width -= (int)Transform.Size.X;
            }

            GraphicsUtil.DrawRect(sbGUI, camRect, 2, Color.Turquoise);
            GraphicsUtil.DrawLine(sbGUI, vertA, vertB, 2, Color.Turquoise);
            GraphicsUtil.DrawLine(sbGUI, horiA, horiB, 2, Color.Turquoise);
        }
        private void DrawSystems()
        {
            foreach (PlanetarySystem ps in planetarySystems)
            {
                Vector2 systemPos = ps.GetMapPos();
                GraphicsUtil.DrawCircle(sbGUI, new Vector2(Transform.Translation.X + systemPos.X * Transform.Bounds.Width, Transform.Translation.Y + systemPos.Y * Transform.Bounds.Height), 1, 8, 3, Color.Turquoise);
            }
        }
        private void DrawBackground()
        {
            Transform t = new Transform(Transform.Translation.X - 4, Transform.Translation.Y - 4, Transform.Bounds.Width + 8, Transform.Bounds.Height + 8);
            Background.DrawNineCut(sbGUI, t);
        }
    }
}
