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

            public TileEntity(Entity entity, Rectangle position, Rectangle collision, float rotation = 0f)
            {
                this.position = position;
                this.collision = collision;
                this.rotation = rotation;

                EntityTileMap entityTileMap = EntitySystem.BlackBoard.GetEntry<EntityTileMap>("EntityTileMap");

                IEnumerator<int2> i = this.getCollisonTiles();
                while (i.MoveNext())
                    entityTileMap.setTile(i.Current.x, i.Current.y, entity);
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
                EntityTileMap entityTileMap = EntitySystem.BlackBoard.GetEntry<EntityTileMap>("EntityTileMap");
                for (int i = collision.Left; i < collision.Right; i++)
                {
                    for (int j = collision.Top; j < collision.Bottom; j++)
                    {
                        entityTileMap.setTile(i, j, null);
                    }
                }
            }

            public bool isMultiTile()
            {
                return position.Width > 1 || position.Height > 1;
            }

            public IEnumerator<int2> getTiles()
            {
                for (int i = position.Left; i < position.Right; i++)
                {
                    for (int j = position.Top; j < position.Bottom; j++)
                    {
                        yield return new int2(i, j);
                    }
                }
            }

            public IEnumerator<int2> getCollisonTiles()
            {
                for (int i = collision.Left; i < collision.Right; i++)
                {
                    for (int j = collision.Top; j < collision.Bottom; j++)
                    {
                        yield return new int2(i, j);
                    }
                }
            }
        }

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

        public class HealthComponent : IComponent
        {
            float health;
            float maxHealth;
            bool visible = false;

            public HealthComponent(float health, float maxHealth = 100)
            {
                this.health = health;
                this.maxHealth = maxHealth;
            }

            public float Health { get { return health; } set { health = value; } }

            public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

            public bool Visible { get { return visible; } set { visible = value; } }
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

        public class Formation : IComponent
        {
            int2 pos;
            EntityFormation entityFormation = null;
            int formationIndex = 0;
            public float rot = (float)Math.PI;

            public Formation()
            {
                pos = new int2(-1, -1);
            }

            public EntityFormation EntityFormation
            {
                get { return entityFormation; }
                set { entityFormation = value; }
            }

            public int2 Pos
            {
                get { return pos; }
                set { pos = value; }
            }

            public int FormationIndex
            {
                get { return formationIndex; }
                set { formationIndex = value; }
            }

        }


    }

}
