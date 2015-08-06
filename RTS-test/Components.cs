using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Artemis.Attributes;

namespace RTS_test
{
    namespace component
    {
        [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
        class Position : ComponentPoolable
        {
            public Vector2 pos;

            public Position(Vector2 pos)
            {
                this.pos = pos;
            }
            public Position(float x, float y)
            {
                this.pos = new Vector2(x, y);
            }
        }

        [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
        class Velocity : ComponentPoolable
        {
            public Vector2 velocity;

            public Velocity(Vector2 velocity)
            {
                this.velocity = velocity;
            }
            public Velocity()
            {
                this.velocity = new Vector2(0f, 0f);
            }
        }

        [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
        class Thrust : ComponentPoolable
        {
            public Vector2 thrust;

            public Thrust(Vector2 thrust)
            {
                this.thrust = thrust;
            }
            public Thrust()
            {
                this.thrust = new Vector2(0f, 0f);
            }
        }

        [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
        class Unit : ComponentPoolable
        {
            private PathGoal pathGoal = null;
            private Vector2 direction;

            public Unit()
            {
                pathGoal = null;
                direction = new Vector2(0f);
            }
        }

        [ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
        class Drawable : ComponentPoolable
        {
            public Texture2D texture;

            public Drawable(Texture2D texture)
            {
                this.texture = texture;
            }
        }
    }
}
