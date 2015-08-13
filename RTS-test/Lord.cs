using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public class Lord
    {
        private string name;
        private Color color;
        private int team;


        public Lord(string name, Color color, int team)
        {
            this.name = name;
            this.color = color;
            this.team = team;
        }

        public string Name
        {
            get { return name; }
        }

        public Color Color
        {
            get { return color; }
        }

        public int Team
        {
            get { return team; }
        }

    }
}
