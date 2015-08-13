using Microsoft.Xna.Framework.Input;
using RTS_test.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public struct KeyState
    {
         bool down;
         bool toggled;

        public KeyState(bool down, bool toggled)
        {
            this.down = down;
            this.toggled = toggled;
        }

        public void setDown(bool down)
        {
            this.down = down;
        }

        public void setToggled(bool toggled)
        {
            this.toggled = toggled;
        }

        public bool Down { get { return down; } }
        public bool Up { get { return !down; } }
        public bool Toggled { get { return toggled; }}
        public bool Pressed { get { return (down && toggled); } }
        public bool Released { get { return (!down && toggled); } }
    }

    public class InputManager
    {
        public delegate void MouseDelegate(bool isDown);

        private Dictionary<string, Keys> bindings = new Dictionary<string, Keys>();
        private Dictionary<Keys, KeyState> keyStates = new Dictionary<Keys, KeyState>();
        private Dictionary<Keys, VoidEvent> onPressEvents = new Dictionary<Keys, VoidEvent>();
        private List<Keys> toggledKeys = new List<Keys>();
        private List<Keys> boundKeys = new List<Keys>();
        public event MouseDelegate lmbEvent;
        public event MouseDelegate mmbEvent;
        public event MouseDelegate rmbEvent;
        public KeyState lmb = new KeyState(false, false);
        public KeyState rmb = new KeyState(false, false);
        public KeyState mmb = new KeyState(false, false);

        public InputManager()
        {
        }

        public void update()
        {
            //Set toggled to false
            foreach(Keys key in toggledKeys)
            {
                if (!keyStates.ContainsKey(key))
                    continue;

                keyStates[key].setToggled(false);
            }
            

            foreach(Keys key in boundKeys)
            {
                bool isDown = Keyboard.GetState().IsKeyDown(key);
                if (keyStates[key].Down == isDown)
                    continue;

                keyStates[key].setDown(isDown);
                keyStates[key].setToggled(true);
                toggledKeys.Add(key);
            }

            // Update mouse buttons
            lmb.setToggled(false);
            rmb.setToggled(false);
            mmb.setToggled(false);

            bool isLmbDown = (Mouse.GetState().LeftButton == ButtonState.Pressed);
            bool isRmbDown = (Mouse.GetState().RightButton == ButtonState.Pressed);
            bool isMmbDown = (Mouse.GetState().MiddleButton == ButtonState.Pressed);

            if (lmb.Down != isLmbDown) 
            {
                lmb.setDown(isLmbDown);
                lmb.setToggled(true);
                if (lmbEvent != null)
                    lmbEvent(lmb.Down);
            }
            if (rmb.Down != isRmbDown)
            {
                rmb.setDown(isRmbDown);
                rmb.setToggled(true);
                if (rmbEvent != null)
                    rmbEvent(rmb.Down);
            }
            if (mmb.Down != isMmbDown)
            {
                mmb.setDown(isMmbDown);
                mmb.setToggled(true);
                if (mmbEvent != null)
                    mmbEvent(lmb.Down);
            }
        }

        public void bindKey(string binding, Keys key)
        {
            if (bindings.ContainsKey(binding))
                bindings[binding] = key;
            else
                bindings.Add(binding, key);
            
            if (keyStates.ContainsKey(key))
                return;

            keyStates.Add(key, new KeyState(false, false));
            boundKeys.Add(key);
        }

        public KeyState this[string keyBinding]
        {
            get
            {
                if (!bindings.ContainsKey(keyBinding))
                    throw new Exception("Could not find Keybinding: '" + keyBinding + "'! Make sure you have bound your key and spelled right.");

                Keys key = bindings[keyBinding];

                if (!keyStates.ContainsKey(key))
                    return new KeyState(false, false);

                return keyStates[key];
            }
        }

        public void subscribeOnPress(string keyBinding, VoidEvent.VoidDelegate voidDelegate)
        {
            if (!bindings.ContainsKey(keyBinding))
                return;

            Keys key = bindings[keyBinding];

            if (!onPressEvents.ContainsKey(key))
                onPressEvents.Add(key, new VoidEvent());

            onPressEvents[key].subscribeEvent(voidDelegate);
        }

        public void unsubscribeOnPress(string keyBinding, VoidEvent.VoidDelegate voidDelegate)
        {
            if (!bindings.ContainsKey(keyBinding))
                return;

            Keys key = bindings[keyBinding];

            if (!onPressEvents.ContainsKey(key))
                onPressEvents.Add(key, new VoidEvent());

            onPressEvents[key].unsubscribeEvent(voidDelegate);
        }

    }
}
