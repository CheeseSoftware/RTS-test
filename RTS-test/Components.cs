﻿using System;
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
		public class Size : IComponent
		{
			public Vector2 size;

			public float X { get { return size.X; } set { size.X = value; } }
			public float Y { get { return size.Y; } set { size.Y = value; } }

			public Size(Vector2 size)
			{
				this.size = size;
			}
			public Size(float x, float y)
			{
				this.size = new Vector2(x, y);
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

        public class Physics : IComponent
        {
            private FarseerPhysics.Dynamics.Body body;

            public Physics(FarseerPhysics.Dynamics.Body body)
            {
                this.body = body;
            }

            public Vector2 Position
            {
                get { return body.Position; }
                set { body.Position = value; }
            }

            public Vector2 Velocity
            {
                get { return body.LinearVelocity; }
                set { body.LinearVelocity = value; }
            }

            public float Rotation
            {
                get { return body.Rotation; }
                set { body.Rotation = value; }
            }
        }
	}

}
