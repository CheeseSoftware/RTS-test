using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public static class Global
    {
        public static readonly Camera Camera = new Camera();
        public static readonly int tileSize = 32;
        public static readonly int mapWidth = 256;
        public static readonly int mapHeight = 256;
        public static readonly Rectangle Viewport = new Rectangle(0, 0, 1000, 600);
        public static int ViewportWidth { get { return Viewport.Width; } }
        public static int ViewportHeight { get { return Viewport.Height; } }
        public static PathGoal globalGoal = null; //TEMP
        public static InputManager inputManager = new InputManager();
    }
}
