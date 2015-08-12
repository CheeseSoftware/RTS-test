using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class FrameRate
    {
        float msPerUpdate = 50;

        DateTime updateLastCheck = DateTime.Now;
        DateTime updateLastReset = DateTime.Now;
        TimeSpan updateAverageTime = new TimeSpan();

        DateTime drawLastCheck = DateTime.Now;
        DateTime drawLastReset = DateTime.Now;
        TimeSpan drawAverageTime = new TimeSpan();

        public FrameRate()
        {

        }

        public void update(GameTime gameTime)
        {
            if ((DateTime.Now - updateLastReset).TotalMilliseconds >= msPerUpdate)
            {
                updateLastReset = DateTime.Now;
                updateAverageTime = (DateTime.Now - updateLastCheck);
            }
            updateLastCheck = DateTime.Now;
        }

        public void draw(GameTime gameTime)
        {
            if ((DateTime.Now - drawLastReset).TotalMilliseconds >= msPerUpdate)
            {
                drawLastReset = DateTime.Now;
                drawAverageTime = (DateTime.Now - drawLastCheck);
            }
            drawLastCheck = DateTime.Now;
        }

        public double UpdateTime { get { return updateAverageTime.TotalMilliseconds; } }

        public double FrameTime { get { return drawAverageTime.TotalMilliseconds; } }

        public double UPS { get { return Math.Round((double)(1000) / updateAverageTime.TotalMilliseconds, 2); } }

        public double FPS { get { return Math.Round((double)(1000) / drawAverageTime.TotalMilliseconds, 2); } }

    }
}
