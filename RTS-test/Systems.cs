using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    namespace system
    {
        [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
        public class EntityRenderer : EntityProcessingSystem<component.Physics, component.Drawable>
        {
            private SpriteBatch spriteBatch;

            public EntityRenderer()
                : base(Aspect.All(typeof(component.Physics), typeof(component.Drawable)))
            {
                spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            public override void LoadContent()
            {
                this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            protected override void Process(Entity e, component.Physics physics, component.Drawable drawable)
            {
                Rectangle rectangle = new Rectangle(
                    new Point((int)(physics.Position.X * Global.tileSize), (int)(physics.Position.Y * Global.tileSize)),
                    new Point(drawable.texture.Width, drawable.texture.Height)
                    );

                TileMap tileMap = EntitySystem.BlackBoard.GetEntry<TileMap>("TileMap");
                float dis = tileMap.DisField.getDis(physics.Position * Global.tileSize) / 4f;
                Color color = new Color(dis, dis, dis);



                spriteBatch.Draw(drawable.texture, null, rectangle, null, new Vector2(drawable.texture.Width / 2, drawable.texture.Height / 2), physics.Rotation, null, color, SpriteEffects.None, 0);
            }
        }

        [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 1)]
        public class TileEntityRenderer : EntityProcessingSystem<component.TileEntity, component.Drawable>
        {
            private SpriteBatch spriteBatch;

            public TileEntityRenderer()
                : base(Aspect.All(typeof(component.TileEntity), typeof(component.Drawable)))
            {
                spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            public override void LoadContent()
            {
                this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            protected override void Process(Entity e, component.TileEntity tileEntity, component.Drawable drawable)
            {
                Rectangle rectangle = new Rectangle(
                    new Point((int)(tileEntity.Position.X * Global.tileSize), (int)(tileEntity.Position.Y * Global.tileSize)),
                    new Point(drawable.texture.Width, drawable.texture.Height)
                    );

                TileMap tileMap = EntitySystem.BlackBoard.GetEntry<TileMap>("TileMap");
                float dis = tileMap.DisField.getDis(tileEntity.Position.Location.ToVector2() * Global.tileSize) / 4f;
                Color color = new Color(dis, dis, dis);

                //spriteBatch.Draw(drawable.texture, null, rectangle, null, new Vector2(drawable.texture.Width / 2, drawable.texture.Height / 2), tileEntity.Rotation, null, color, SpriteEffects.None, 0);
            }
        }

        [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
        public class UnitWalker : EntityProcessingSystem<component.Goal, component.Physics>
        {
            public UnitWalker()
                : base(Aspect.All(typeof(component.Goal), typeof(component.Physics)))
            {
            }

            public override void LoadContent()
            {
            }

            protected override void Process(Entity e, component.Goal goal, component.Physics physics)
            {
                if (goal.pathGoal == null)
                    return;

                Vector2 dir = goal.pathGoal.getDirection(physics.Position);
                float oldRotation = physics.Rotation;
                float rotation = (float)Math.Atan2(dir.Y, dir.X);

                var newDir = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                var oldDir = new Vector2((float)Math.Cos(oldRotation), (float)Math.Sin(oldRotation));
                oldDir += (newDir - oldDir) * 0.08f;

                physics.Rotation = (float)Math.Atan2(oldDir.Y, oldDir.X);

                if (physics.Position.X < 0 || physics.Position.Y < 0 || physics.Position.X > Global.mapWidth * 32 || physics.Position.Y > Global.mapHeight * 32)
                {
                    physics.Position = physics.OldPosition;
                    physics.Velocity = new Vector2();
                }
                else if (Math.Abs(oldRotation - rotation) <= 1)
                    physics.Body.ApplyLinearImpulse(1.1f * dir);
            }
        }

        [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
        public class MaxVelocity : EntityProcessingSystem<component.MaxVelocity, component.Physics>
        {
            public MaxVelocity()
                : base(Aspect.All(typeof(component.MaxVelocity), typeof(component.Physics)))
            {
            }

            public override void LoadContent()
            {
            }

            protected override void Process(Entity e, component.MaxVelocity maxVelocity, component.Physics physics)
            {
                if (physics.Velocity.Length() > maxVelocity.maxVelocity)
                {
                    physics.Velocity.Normalize();
                    physics.Velocity *= maxVelocity.maxVelocity;
                }
            }
        }


        [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
        public class TerrainPhysics : EntityProcessingSystem<component.Physics>
        {
            private DisFieldMixer disFieldMixer;

            public TerrainPhysics()
                : base(Aspect.All(typeof(component.Physics)))
            {

            }


            public override void LoadContent()
            {
                disFieldMixer = EntitySystem.BlackBoard.GetEntry<DisFieldMixer>("DisFieldMixer");
            }

            protected override void Process(Entity e, component.Physics physics)
            {
                // Raymarched collision to terrain
                // TODO: Replace -1.0f with -1.0f*radius.
                float dis;
                for (int i = 0; i < 8; ++i)
                {
                    dis = disFieldMixer.getDis(physics.Position - new Vector2(0.5f, 0.5f)) - 1.0f + 0.5f;
                    Vector2 normal = disFieldMixer.getNormal(physics.Position);
                    if (dis < 0f && normal.Length() > 0f)
                    {
                        physics.Body.ApplyLinearImpulse(-1.0f * normal * dis);
                        //physics.Body.ApplyForce(-100.0f * normal * (dis));
                        //physics.Position += -1f * normal * (dis);
                        //physics.Velocity *= 0.5f;

                        if (dis < -0.5f)
                            physics.Position += -1.0f * normal * (dis + 0.5f);
                        //physics.Velocity += -0.5f * normal * dis;
                    }
                    else
                        break;
                }
            }

        }






    }
}
