using System.Collections.Generic;
using System.Linq;

namespace Arwic.InfiniteFrontier
{
    public class FrameCounter
    {
        // total frames
        public long TotalFrames { get; private set; }
        // total seconds
        public float TotalSeconds { get; private set; }
        // average fps
        public float AverageFramesPerSecond { get; private set; }
        // current fps
        public float CurrentFramesPerSecond { get; private set; }
        // max samples to take for average
        public const int MAXIMUSAMPLES = 100;
        // sample buffer
        private Queue<float> SampleBuffer = new Queue<float>();

        // update
        public bool Update(float deltaTime)
        {
            // calculate current fps
            CurrentFramesPerSecond = 1.0f / deltaTime;

            // add current fps to buffer
            SampleBuffer.Enqueue(CurrentFramesPerSecond);
            // check if the buffer is too long
            if (SampleBuffer.Count > MAXIMUSAMPLES)
            {
                // remove the oldest entry
                SampleBuffer.Dequeue();
                // calculate average fps
                AverageFramesPerSecond = SampleBuffer.Average(i => i);
            }
            else
            {
                // calculate average fps
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }
            // update counters
            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }
    }
}
