using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RTS_test.GUI.Components
{
    class TestComponent : GUIComponent
    {
        private Texture2D texture;

        public TestComponent(Texture2D texture)
            : base(new Rectangle(0, 0, 100, 100), Alignment.LeftTop)  // relative position to father, and the size of this component
        {
            this.texture = texture;
        }


        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, null, GlobalBox, Box, null, 0f, null, null, SpriteEffects.None, 0);
            base.draw(spriteBatch);
        }
    }
}
