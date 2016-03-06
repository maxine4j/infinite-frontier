using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class Camera
    {
        public Vector2 TranslationTarget { get; set; }
        public Vector2 ZoomTarget { get; set; }
        public Vector2 FollowSpeed { get; set; }
        public Transform Transform { get; set; }
        
        private Vector2 origin;

        public Camera()
        {
            Transform = new Transform(0, 0, 1920, 1080, 0);
            TranslationTarget = Vector2.Zero;
            FollowSpeed = new Vector2(-1, -1);
            ZoomTarget = new Vector2(1, 1);
        }

        public Rectangle ConvertGUIToWorld(Rectangle r)
        {
            Vector2 xy = ConvertGUIToWorld(new Vector2(r.X, r.Y));
            Rectangle rect = new Rectangle((int)xy.X, (int)xy.Y, (int)(r.Width / Transform.Scale.X), (int)(r.Height / Transform.Scale.Y));
            return rect;
        }

        public Vector2 ConvertGUIToWorld(Vector2 p)
        {
            Vector2 pos = p;
            pos = new Vector2(pos.X / scale, pos.Y / scale);
            pos = new Vector2(pos.X / Transform.Scale.X, pos.Y / Transform.Scale.Y);
            pos = new Vector2(pos.X - viewport.Width / 2 / Transform.Scale.X, pos.Y - viewport.Height / 2 / Transform.Scale.Y);
            pos = new Vector2(pos.X + Transform.Translation.X, pos.Y + Transform.Translation.Y);
            return pos;
        }

        public Rectangle ConvertWorldToGUI(Rectangle r)
        {
            Vector2 xy = ConvertWorldToGUI(new Vector2(r.X, r.Y));
            Rectangle rect = new Rectangle((int)xy.X, (int)xy.Y, (int)(r.Width * Transform.Scale.X), (int)(r.Height * Transform.Scale.Y));
            return rect;
        }

        public Vector2 ConvertWorldToGUI(Vector2 p)
        {
            Vector2 pos = p;
            pos = new Vector2(pos.X - Transform.Translation.X, pos.Y - Transform.Translation.Y);
            pos = new Vector2(pos.X + viewport.Width / 2 / Transform.Scale.X, pos.Y + viewport.Height / 2 / Transform.Scale.Y);
            pos = new Vector2(pos.X * Transform.Scale.X, pos.Y * Transform.Scale.Y);
            return pos;
        }

        public void Update(GameTime gameTime)
        {
            if (ZoomTarget != Transform.Scale)
            {
                Vector2 sep = ZoomTarget - Transform.Scale;
                float autoZoomMod = 10.0f;
                Vector2 inc = new Vector2(sep.X * (float)gameTime.ElapsedGameTime.TotalSeconds * autoZoomMod, sep.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * autoZoomMod);
                if (inc.Length() >= sep.Length())
                    Transform.Scale = ZoomTarget;
                else
                    Transform.Scale += inc;
            }

            origin = new Vector2(viewport.Width / 2 / Transform.Scale.X, viewport.Height / 2 / Transform.Scale.Y);

            if (TranslationTarget != Transform.Translation)
            {
                Vector2 sep = TranslationTarget - Transform.Translation;
                float autoFollowMod = 10.0f;
                if (FollowSpeed.X == -1)
                {
                    float inc = sep.X * (float)gameTime.ElapsedGameTime.TotalSeconds * autoFollowMod;
                    Transform.Translation += new Vector2(inc, 0);
                }
                else
                {
                    float inc = FollowSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inc > sep.X)
                    {
                        inc = sep.X;
                    }
                    Transform.Translation += new Vector2(inc, 0);
                }

                if (FollowSpeed.Y == -1)
                {
                    float inc = sep.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * autoFollowMod;
                    Transform.Translation += new Vector2(0, inc);
                }
                else
                {
                    float inc = FollowSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inc > sep.Y)
                    {
                        inc = sep.Y;
                    }
                    Transform.Translation += new Vector2(0, inc);
                }
            }

            if (mapBounds.X != -1 && mapBounds.Y != -1 && mapBounds.Width != -1 && mapBounds.Height != -1)
            {
                if (Transform.Translation.X - viewport.Width / 2 < mapBounds.X)
                {
                    Transform.Translation = new Vector2(mapBounds.X + viewport.Width / 2, Transform.Translation.Y);
                    TranslationTarget = new Vector2(mapBounds.X + viewport.Width / 2, TranslationTarget.Y);
                }
                else if (Transform.Translation.X + viewport.Width / 2 > mapBounds.Width)
                {
                    Transform.Translation = new Vector2(mapBounds.Width - viewport.Width / 2, Transform.Translation.Y);
                    TranslationTarget = new Vector2(mapBounds.Width - viewport.Width / 2, TranslationTarget.Y);
                }

                if (Transform.Translation.Y - viewport.Height / 2 < mapBounds.Y)
                {
                    Transform.Translation = new Vector2(Transform.Translation.X, mapBounds.Y + viewport.Height / 2);
                    TranslationTarget = new Vector2(TranslationTarget.X, mapBounds.Y + viewport.Height / 2);
                }
                else if (Transform.Translation.Y + viewport.Height / 2 > mapBounds.Height)
                {
                    Transform.Translation = new Vector2(Transform.Translation.X, mapBounds.Height - viewport.Height / 2);
                    TranslationTarget = new Vector2(TranslationTarget.X, mapBounds.Height - viewport.Height / 2);
                }
            }
        }

        public void LookAt(Vector2 point)
        {
            TranslationTarget = point;
        }

        // get the transformation matrix
        public Matrix GetForegroundTransformation(GraphicsDevice graphicsDevice, float scale)
        {
            return Matrix.Identity *
                    Matrix.CreateTranslation(-Transform.Translation.X, -Transform.Translation.Y, 0) *
                    Matrix.CreateTranslation(origin.X, origin.Y, 0) *
                    Matrix.CreateScale(new Vector3(Transform.Scale.X, Transform.Scale.Y, 1.0f)) *
                    Matrix.CreateScale(new Vector3(scale, scale, 1.0f));
        }
    }
}