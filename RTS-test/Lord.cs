using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTS_test.Types;
using Artemis;

namespace RTS_test
{
    public class Lord
    {
        private string name;
        private Color color;
        private int team;
        private PagedArray2D<bool> unitMap = new PagedArray2D<bool>(false, 8, 8);
        private PagedArray2D<List<Entity>> unitDetailMap = new PagedArray2D<List<Entity>>(null, 8, 8);


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
