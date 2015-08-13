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

        public Unit()
        {
        }
    }
}
