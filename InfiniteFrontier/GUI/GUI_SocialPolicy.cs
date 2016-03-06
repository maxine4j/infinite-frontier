using Microsoft.Xna.Framework;

using static Arwic.InfiniteFrontier.Main;

namespace Arwic.InfiniteFrontier
{
    public class GUI_SocialPolicy : IGUIElement
    {
        public Form Form { get; set; }

        public GUI_SocialPolicy()
        {
            Form = new Form(new Transform(1920 / 2 - 500, 40, 1000, 600), formBack, "Social Policies");
            Form.Hidden = true;
        }

        // Update
        public bool Update(GameTime gameTime, bool interacted)
        {
            Form.Update(gameTime);

            if (!Form.Hidden && Form.Transform.Contains(InputUtil.MouseScreenPos))
                return true;
            return false;
        }

        // Draw
        public void Draw()
        {
            Form.Draw(sbGUI, scale);
        }
    }
}
