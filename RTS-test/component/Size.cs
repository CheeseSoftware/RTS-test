using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class Size : IComponent
    {
        public Vector2 size;

        public float X { get { return size.X; } set { size.X = value; } }
        public float Y { get { return size.Y; } set { size.Y = value; } }

        public Size(Vector2 size)
        {
            this.size = size;
        }
        public Size(float x, float y)
        {
            this.size = new Vector2(x, y);
        }
    }
}
