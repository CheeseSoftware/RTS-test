using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RTS_test.GUI.Components
{
    class Label : GUIComponent
    {
        private Color textColor;
        private SpriteFont font;
        private string text = "";

        public Label(Point position, Color textColor, SpriteFont font)
            : base(new Rectangle(position.X, position.Y, 0, 0), Alignment.LeftTop)  // relative position to father, and the size of this component
        {
            this.textColor = textColor;
            this.font = font;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, GlobalPosition.ToVector2(), textColor);
            base.draw(spriteBatch);
        }

        public string Text { get { return text; } set { text = value; } }
    }
}
