using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    class InputUtil
    {
        public static Vector2 MouseScreenPos
        {
            get
            {
                Point p = Mouse.GetState().Position;
                return new Vector2(p.X, p.Y);
            }
        }
        public static Vector2 MouseWorldPos
        {
            get
            {
                Vector2 pos = MouseScreenPos;
                pos = new Vector2(pos.X / scale, pos.Y / scale);
                pos = new Vector2(pos.X / camera.Transform.Scale.X, pos.Y / camera.Transform.Scale.Y);
                pos = new Vector2(pos.X - viewport.Width / 2 / camera.Transform.Scale.X, pos.Y - viewport.Height / 2 / camera.Transform.Scale.Y);
                pos = new Vector2(pos.X + camera.Transform.Translation.X, pos.Y + camera.Transform.Translation.Y);
                return pos;
            }
        }
        public static bool LeftMouseButton
        {
            get
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    return true;
                return false;
            }
        }
        public static bool RightMouseButton
        {
            get
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    return true;
                return false;
            }
        }
        public static bool MiddleMouseButton
        {
            get
            {
                if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
                    return true;
                return false;
            }
        }
        public static bool WasLeftMouseButton
        {
            get
            {
                if (lastMouseState.LeftButton == ButtonState.Pressed)
                    if (Mouse.GetState().LeftButton == ButtonState.Released)
                        return true;
                return false;
            }
        }
        public static bool WasRightMouseButton
        {
            get
            {
                if (lastMouseState.RightButton == ButtonState.Pressed)
                    if (Mouse.GetState().RightButton == ButtonState.Released)
                        return true;
                return false;
            }
        }
        public static bool WasMiddleMouseButton
        {
            get
            {
                if (lastMouseState.MiddleButton == ButtonState.Pressed)
                    if (Mouse.GetState().MiddleButton == ButtonState.Released)
                        return true;
                return false;
            }
        }
        public static bool IsKeyDown(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
                return true;
            return false;
        }

        public static bool WasKeyDown(Keys key)
        {
            if (lastKeyboardState.IsKeyDown(key))
                if (Keyboard.GetState().IsKeyUp(key))
                    return true;
            return false;
        }
    }
}
