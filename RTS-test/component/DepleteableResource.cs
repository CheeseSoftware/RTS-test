using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class DepletableResource : IComponent
    {
        public int resourceAmount;
        public String type;

        public DepletableResource(int resourceAmount, String type)
        {
            this.resourceAmount = resourceAmount;
            this.type = type;
        }
    }
}
