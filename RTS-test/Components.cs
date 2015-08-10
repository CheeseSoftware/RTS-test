using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Artemis.Attributes;
using Artemis.Interface;
using Artemis.System;
using Artemis.Blackboard;
using Jitter2D.Dynamics;

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
            private float additionalRotation;

            public Drawable(Texture2D texture, float additionalRotation = 0f)
            {
                this.texture = texture;
                this.additionalRotation = additionalRotation;
            }

            public float AdditionalRotation { get { return this.additionalRotation; } }
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

        public class TileEntity : IComponent
        {
            private Rectangle position;
            private Rectangle collision;
            private float rotation = 0f;

            public TileEntity(Rectangle position, Rectangle collision, float rotation = 0f)
            {
                this.position = position;
                this.collision = collision;
                this.rotation = rotation;

                DisField entityDisField = EntitySystem.BlackBoard.GetEntry<DisField>("EntityDisField");
                for (int i = collision.Left; i < collision.Right; i++)
                {
                    for (int j = collision.Top; j < collision.Bottom + collision.Height; j++)
                    {
                        entityDisField.setTile(i, j, true);
                    }
                }
            }

            ~TileEntity()
            {
                remove();
            }

            public Rectangle Position
            {
                get { return position; }
                set { position = value; }
            }

            public Rectangle Collision
            {
                get { return collision; }
                set { collision = value; }
            }

            public float Rotation
            {
                get { return rotation; }
                set { rotation = value; }
            }

            public void remove()
            {
                DisField entityDisField = EntitySystem.BlackBoard.GetEntry<DisField>("EntityDisField");
                for (int i = collision.Left; i < collision.Right; i++)
                {
                    for (int j = collision.Top; j < collision.Bottom + collision.Height; j++)
                    {
                        entityDisField.setTile(i, j, false);
                    }
                }
            }

            public bool isMultiTile()
            {
                return position.Width > 1 || position.Height > 1;
            }

            public IEnumerator<int2> getTiles()
            {
                for (int i = collision.Left; i < collision.Right; i++)
                {
                    for (int j = collision.Top; j < collision.Bottom + collision.Height; j++)
                    {
                        yield return new int2(i, j);
                    }
                }
            }
        }

        public class Physics : IComponent
        {
            private RigidBody body;
            private Vector2 oldPosition = new Vector2();

            public Physics(RigidBody body)
            {
                this.body = body;
            }

            public Vector2 Position
            {
                get { return new Vector2(body.Position.X, body.Position.Y); }
                set { oldPosition = new Vector2(body.Position.X, body.Position.Y); body.Position = new Jitter2D.LinearMath.JVector(value.X, value.Y); }
            }

            public Vector2 OldPosition
            {
                get { return oldPosition; }
            }

            public Vector2 Velocity
            {
                get { return new Vector2(body.LinearVelocity.X, body.LinearVelocity.Y); }
                set { body.LinearVelocity = new Jitter2D.LinearMath.JVector(value.X, value.Y); }
            }

            public float Rotation
            {
                get { return body.Orientation; }
                set { body.Orientation = value; }
            }

            public RigidBody Body
            {
                get { return body; }
            }
        }

        public class HealthComponent : IComponent
        {
            float health;
            float maxHealth;

            public HealthComponent(float health, float maxHealth = 100)
            {
                this.health = health;
                this.maxHealth = maxHealth;
            }

            public float Health { get { return health; } set { health = value; } }

            public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

        }

        public class AnimationComponent : IComponent
        {
            private Dictionary<int, Animation> animations;
            private Animation currentAnimation;
            private int currentTexture;

            public AnimationComponent()
            {
                animations = new Dictionary<int, Animation>();
            }

            public void addAnimation(int texture, Animation animation)
            {
                animations.Add(texture, animation);
            }

            public void startAnimation(int texture)
            {
                currentAnimation = animations[texture];
                currentTexture = texture;
                currentAnimation.start();
            }

            public void stopAnimation()
            {
                currentAnimation.stop();
            }

            public bool isAnimating()
            {
                return currentAnimation != null && currentAnimation.isAnimating();
            }

            public Rectangle getCurrentFrame()
            {
                return currentAnimation.getCurrentFrame();
            }

            public int getCurrentTexture()
            {
                return currentTexture;
            }

        }
    }

}
