using Microsoft.Xna.Framework;
using System;

namespace Arwic.InfiniteFrontier
{
    public class Timer
    {
        // time before expiring (ms)
        public double Delay { get; set; }
        // current time (ms)
        private double currentTime;

        // constructor
        public Timer(long d)
        {
            this.Delay = d;
        }

        // empty constructor
        public Timer() { }

        // start the timer
        public void Start()
        {
            // set the starting time
            currentTime = 0;
        }

        public float GetProgress()
        {
            float res = Convert.ToSingle(currentTime / Delay);
            if (res > 1.0f)
                res = 1.0f;
            return res;
        }

        // check if the timer has expired
        public Boolean isExpired()
        {
            // if it has expired return true 
            if (currentTime > Delay)
                return true;
            return false;
        }

        // update the timer
        public void Update(GameTime gameTime)
        {
            currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
