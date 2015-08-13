using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.GUI
{
    abstract class GUIComponent
    {
        private Dictionary<string, GUIComponent> components;
        private GUIComponent father = null;
        private Rectangle box;
        private Alignment alignment;

        public GUIComponent(Rectangle box, Alignment alignment = Alignment.LeftTop)
        {
            components = new Dictionary<string, GUIComponent>();
            this.box = box;
            this.alignment = alignment;
        }

        public virtual void update(GameTime gameTime) // Child components must override and call base.draw()
        {
            foreach (GUIComponent containingComponent in components.Values)
            {
                containingComponent.update(gameTime);
            }
        }

        public virtual void draw(SpriteBatch spriteBatch) // Child components must override and call base.draw()
        {
            foreach (GUIComponent containingComponent in components.Values)
            {
                containingComponent.draw(spriteBatch);
            }
        }

        public void addComponent(string name, GUIComponent component) // example: ("buttonKillEverything", button)
        {
            component.father = this;
            components.Add(name, component);
        }

        public Point GlobalPosition
        {
            get
            {
                Point thisGlobalPos = box.Location;
                if (father == null)
                    return thisGlobalPos;
                thisGlobalPos += father.GlobalPosition;

                switch (alignment)
                {
                    case Alignment.LeftTop:
                        return thisGlobalPos;
                    case Alignment.Left:
                        return new Point(thisGlobalPos.X, thisGlobalPos.Y + father.Box.Height / 2 - box.Height / 2);
                    // faderns position + min relativa position till alignment
                    case Alignment.LeftBottom:
                        return new Point(thisGlobalPos.X, thisGlobalPos.Y + father.Box.Height - box.Height);
                    case Alignment.RightTop:
                        return new Point(thisGlobalPos.X + father.box.Width - box.Width, thisGlobalPos.Y);
                    case Alignment.Right:
                        return new Point(thisGlobalPos.X + father.box.Width - box.Width, thisGlobalPos.Y + father.Box.Height / 2 - box.Height / 2);
                    case Alignment.RightBottom:
                        return new Point(thisGlobalPos.X + father.box.Width - box.Width, thisGlobalPos.Y + father.Box.Height - box.Height);
                    case Alignment.Top:
                        return new Point(thisGlobalPos.X + father.box.Width / 2 - box.Width / 2, thisGlobalPos.Y);
                    case Alignment.Middle:
                        return new Point(thisGlobalPos.X + father.box.Width / 2 - box.Width / 2, thisGlobalPos.Y + father.Box.Height / 2 - box.Height / 2);
                    case Alignment.Bottom:
                        return new Point(thisGlobalPos.X + father.box.Width / 2 - box.Width / 2, thisGlobalPos.Y + father.Box.Height - box.Height);
                    default:
                        return thisGlobalPos;
                }
            }
        }

        public GUIComponent Father { get { return father; } set { father = value; } }

        public Point LocalPosition { get { return box.Location; } }

        public Point Size { get { return box.Size; } }

        public Rectangle Box { get { return this.box; } }

        public Rectangle GlobalBox { get { Point globalPosition = GlobalPosition;  return new Rectangle(globalPosition.X, globalPosition.Y, box.Width, box.Height); } }
    }
}
