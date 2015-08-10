using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace RTS_test
{
    public class Animation : ICloneable
    {
        private bool animating = false;
        private int startFrame;
        private int endFrame;
        private double msBetweenFrames;
        private Rectangle[] frames;

        private int currentFrame;
        private DateTime frameStarted;

        public Animation(int startFrame, int endFrame, double msBetweenFrames, params Rectangle[] frames)
        {
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.msBetweenFrames = msBetweenFrames;
            this.frames = frames;
        }

        public void start()
        {
            currentFrame = startFrame;
            frameStarted = DateTime.Now;
            animating = true;
        }

        public void stop()
        {
            animating = false;
        }

        public bool isAnimating()
        {
            return this.animating;
        }

        public Rectangle getCurrentFrame()
        {
            if ((DateTime.Now - frameStarted).TotalMilliseconds >= msBetweenFrames)
            {
                currentFrame++;
                frameStarted = DateTime.Now;
            }
            if (currentFrame > endFrame)
                currentFrame = startFrame;
            return frames[currentFrame];
        }

        public object Clone()
        {
            return new Animation(startFrame, endFrame, msBetweenFrames, frames);
        }
    }
}
