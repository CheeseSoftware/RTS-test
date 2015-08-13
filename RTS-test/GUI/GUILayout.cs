using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.GUI
{
    class GUILayout : GUIComponent
    {
        private int order = 0; // 0 lowest order, higher on top
        private bool hidden = false;

        public GUILayout()
            : base(new Rectangle(0, 0, 0, 0), Alignment.LeftTop)
        {

        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (!hidden)
                base.draw(spriteBatch);
        }

        public bool Hidden { get { return hidden; } set { hidden = value; } }

        public int Order { get { return order; } set { order = value; } }
    }
}
