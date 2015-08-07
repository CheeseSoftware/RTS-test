using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Artemis.Attributes;
using Artemis.Interface;

namespace RTS_test
{
    namespace component
    {
		[ArtemisComponentPool(InitialSize = 5, IsResizable = true, ResizeSize = 20, IsSupportMultiThread = false)]
		public class Position : IComponent
		{
			public Vector2 pos;

			public float X { get { return pos.X; } set { pos.X = value; } }
			public float Y { get { return pos.Y; } set { pos.Y = value; } }

			public Position(Vector2 pos)
            {
                this.pos = pos;
            }
            public Position(float x, float y)
            {
                this.pos = new Vector2(x, y);
            }
        }

		public class Velocity : IComponent
		{
            public Vector2 velocity;

			public float X { get { return velocity.X; } set { velocity.X = value; } }
			public float Y { get { return velocity.Y; } set { velocity.Y = value; } }

			public Velocity(Vector2 velocity)
            {
                this.velocity = velocity;
            }
            public Velocity()
            {
                this.velocity = new Vector2(0f, 0f);
            }
        }

        public class MaxVelocity : IComponent
        {
            public float maxVelocity;

            public MaxVelocity(float maxVelocity)
            {
                this.maxVelocity = maxVelocity;
            }
        }

		public class Thrust : IComponent
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

		public class Unit : IComponent
		{
            private PathGoal pathGoal = null;
            private Vector2 direction;

            public Unit()
            {
                pathGoal = null;
                direction = new Vector2(0f);
            }
        }

		public class Drawable : IComponent
		{
            public Texture2D texture;

            public Drawable(Texture2D texture)
            {
                this.texture = texture;
            }
        }

        public class Goal : IComponent
        {
            public PathGoal pathGoal;

            public Goal()
            {
                this.pathGoal = null;
            }
        }
    }
}
