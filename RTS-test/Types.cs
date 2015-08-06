using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    struct int2
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

        public static int2 operator +(int2 c1, int2 c2)
        {
            return new int2(c1.x + c2.x, c1.y + c2.y);
        }

        public int x;
        public int y;
    }

    
     
}
