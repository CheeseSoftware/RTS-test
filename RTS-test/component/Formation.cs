using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class Formation : IComponent
    {
        int2 pos;
        EntityFormation entityFormation = null;
        int formationIndex = 0;
        public float rot = (float)Math.PI;

        public Formation()
        {
            pos = new int2(-1, -1);
        }

        public EntityFormation EntityFormation
        {
            get { return entityFormation; }
            set { entityFormation = value; }
        }

        public int2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public int FormationIndex
        {
            get { return formationIndex; }
            set { formationIndex = value; }
        }

    }
}
