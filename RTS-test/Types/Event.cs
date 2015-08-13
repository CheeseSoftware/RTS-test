using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.Types
{
    public class VoidEvent
    {
        public delegate void VoidDelegate();
        event VoidDelegate theEvent;

        public void call()
        {
            theEvent();
        }

        public void subscribeEvent(VoidDelegate voidDelegate)
        {
            theEvent += voidDelegate;
        }

        public void unsubscribeEvent(VoidDelegate voidDelegate)
        {
            theEvent -= voidDelegate;
        }
    }
}
