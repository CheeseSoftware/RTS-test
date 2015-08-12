using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class FrameRate
    {
        float msPerUpdate = 100;

        DateTime updateLastReset = DateTime.Now;
        double updateAverageTime = 0;

        DateTime drawLastReset = DateTime.Now;
        double drawAverageTime = 0;

        public FrameRate()
        {

        }

        public void update(GameTime gameTime)
        {
            if ((DateTime.Now - updateLastReset).TotalMilliseconds >= msPerUpdate)
            {
                updateLastReset = DateTime.Now;
                updateAverageTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public void draw(GameTime gameTime)
        {
            if ((DateTime.Now - drawLastReset).TotalMilliseconds >= msPerUpdate)
            {
                drawLastReset = DateTime.Now;
                drawAverageTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public double UpdateTime { get { return updateAverageTime; } }

        public double FrameTime { get { return drawAverageTime; } }

        public double UPS { get { return Math.Round(1000 / updateAverageTime, 2); } }

        public double FPS { get { return Math.Round(1000 / drawAverageTime, 2, MidpointRounding.AwayFromZero); } }

    }
}
