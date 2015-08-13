using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.GUI
{
    class GUIWindow
    {
        Dictionary<string, GUILayout> layouts;

        public GUIWindow()
        {
            layouts = new Dictionary<string, GUILayout>();
        }

        public void addLayout(string name, GUILayout layout)
        {
            layouts.Add(name, layout);
        }

        public void removeLayout(string name)
        {
            layouts.Remove(name);
        }

        public void showLayout(string name)
        {
            layouts[name].Hidden = false;
        }

        public void hideLayout(string name)
        {
            layouts[name].Hidden = true;
        }

        public void update(GameTime gameTime)
        {
            foreach (GUILayout layout in layouts.Values)
                layout.update(gameTime);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            //TODO: FIX LAYOUT DRAW ORDER
            foreach (GUILayout layout in layouts.Values)
                layout.draw(spriteBatch);
        }
    }
}
