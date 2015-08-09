using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public struct int2
    {
        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int2(int value)
        {
            this.x = value;
            this.y = value;
        }

        public Vector2 toVector2()
        {
            return new Vector2(x, y);
        }

        public static bool operator ==(int2 a, int2 b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(int2 a, int2 b)
        {
            return (a.x != b.x || a.y != b.y);
        }

        public static int2 operator +(int2 c1, int2 c2)
        {
            return new int2(c1.x + c2.x, c1.y + c2.y);
        }

        public static int2 operator -(int2 c1, int2 c2)
        {
            return new int2(c1.x - c2.x, c1.y - c2.y);
        }

        public static int2 operator /(int2 c1, int2 c2)
        {
            return new int2(c1.x / c2.x, c1.y / c2.y);
        }

        public static int2 operator *(int2 c1, int2 c2)
        {
            return new int2(c1.x * c2.x, c1.y * c2.y);
        }

        public static int2 operator %(int2 c1, int2 c2)
        {
            return new int2(c1.x % c2.x, c1.y % c2.y);
        }

        public static int2 operator +(int2 c1, int c2)
        {
            return new int2(c1.x + c2, c1.y + c2);
        }

        public static int2 operator -(int2 c1, int c2)
        {
            return new int2(c1.x - c2, c1.y - c2);
        }

        public static int2 operator /(int2 c1, int c2)
        {
            return new int2(c1.x / c2, c1.y / c2);
        }

        public static int2 operator *(int2 c1, int c2)
        {
            return new int2(c1.x * c2, c1.y * c2);
        }

        public static int2 operator %(int2 c1, int c2)
        {
            return new int2(c1.x % c2, c1.y % c2);
        }

        public int x;
        public int y;
    }

    
     
}
