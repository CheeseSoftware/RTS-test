using Artemis;
using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class Physics : IComponent
    {
        private FarseerPhysics.Dynamics.Body body;
        private Vector2 oldPosition = new Vector2();

        public Physics(Entity entity, FarseerPhysics.Dynamics.Body body)
        {
            this.body = body;
            body.UserData = entity;
        }

        public Vector2 Position
        {
            get { return body.Position; }
            set { oldPosition = body.Position; body.Position = value; }
        }

        public Vector2 OldPosition
        {
            get { return oldPosition; }
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

        public FarseerPhysics.Dynamics.Body Body
        {
            get { return body; }
        }

        public void rotateTo(float newRotation, float speed)
        {
            if (Rotation.Equals(newRotation))
                return;

            var newDir = new Vector2((float)Math.Cos(newRotation), (float)Math.Sin(newRotation));
            var oldDir = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            oldDir += (newDir - oldDir) * speed;
            Rotation = (float)Math.Atan2(oldDir.Y, oldDir.X);
        }
    }
}
