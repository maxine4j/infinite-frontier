using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Arwic.InfiniteFrontier
{
    public class Animation : IDrawable
    {
        /// <summary>
        /// Width of the Animation
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of the Animation
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The Animations sprite sheet
        /// </summary>
        public Sprite SpriteSheet { get; private set; }

        /// <summary>
        /// Time per frame (ms)
        /// </summary>
        public double Time { get; set; }
        
        private int frameCount, sheetSize, frameIndex;
        private Rectangle[] frames;        
        private double timeElap;
        
        /// <summary>
        /// Frame by frame animation
        /// </summary>
        /// <param name="sheet"> Sprite sheet to draw from </param>
        /// <param name="time"> Time per frame </param>
        /// <param name="frames"> Number of frames on the sprite sheet </param>
        public Animation(Sprite sheet, int time, int frames)
        {
            SpriteSheet = sheet;
            frameCount = frames;
            sheetSize = SpriteSheet.Texture.Width;
            Time = time;
            Height = SpriteSheet.Texture.Height;
            GenerateAnimation();
        }

        private void GenerateAnimation()
        {
            Width = sheetSize / frameCount;
            frames = new Rectangle[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = new Rectangle(i * Width, 0, Width, SpriteSheet.Texture.Height);
            }
        }

        /// <summary>
        /// Updates the animation
        /// </summary>
        /// <param name="gameTime"> Xna GameTime </param>
        public void Update(GameTime gameTime)
        {
            timeElap += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeElap > Time)
            {
                timeElap -= Time;
                if (frameIndex < frames.Length - 1)
                    frameIndex++;
                else
                    frameIndex = 0;
            }
        }

        /// <summary>
        /// Draws the currently displayed frame of animation
        /// </summary>
        /// <param name="sb"> SpriteBatch to draw to </param>
        /// <param name="dest"> Where the animation is to be drawn </param>
        /// <param name="source"> Portion of the frame to be drawn, defaults to entire frame </param>
        /// <param name="colour"> Tint to be applied to the frame, defaults to white </param>
        /// <param name="rotOrigin"> Point to rotate about, defaults to centre of current frame </param>
        /// <param name="rotation"> Angle to rotate frame by (R), defaults to 0f </param>
        /// <param name="scale"> Scale to draw frame by, defaults to (1f, 1f) </param>
        /// <param name="effects"> Effects to be applied to the frame, defaults to None </param>
        /// <param name="depth"> Layer depth to draw frame at, defaults to 0f </param>
        public void Draw(SpriteBatch sb, Transform transform, Rectangle? source = null, Color? colour = null)
        {
            if (source == null) source = frames[frameIndex];
            else source = new Rectangle(frames[frameIndex].X + source.Value.X, frames[frameIndex].Y + source.Value.Y, source.Value.Width, source.Value.Height);
            if (colour == null) colour = Color.White;

            SpriteSheet.Draw(sb, transform, source, colour);
        }
    }
}
