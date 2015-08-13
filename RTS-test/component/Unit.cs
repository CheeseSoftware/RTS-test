using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class Unit : IComponent
    {
        public PathGoal pathGoal = null;
        private Lord lord = null;

        public Unit(Lord lord)
        {
            this.lord = lord;
        }

        public Lord Lord {
            get { return lord; }
        }
    }
}
