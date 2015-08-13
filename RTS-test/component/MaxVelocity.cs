using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class MaxVelocity : IComponent
    {
        public float maxVelocity;

        public MaxVelocity(float maxVelocity)
        {
            this.maxVelocity = maxVelocity;
        }
    }
}
