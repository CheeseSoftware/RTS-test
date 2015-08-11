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
            private TextureManager textureManager;

            public EntityRenderer()
                : base(Aspect.All(typeof(component.Physics), typeof(component.Drawable)))
            {
                spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
                textureManager = EntitySystem.BlackBoard.GetEntry<TextureManager>("TextureManager");
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

                if (!e.HasComponent<component.AnimationComponent>())
                {
                    Color color = new Color(dis, dis, dis);
                    spriteBatch.Draw(drawable.texture, null, rectangle, null, new Vector2(drawable.texture.Width / 2, drawable.texture.Height / 2), physics.Rotation + drawable.AdditionalRotation, null, color, SpriteEffects.None, 0);
                }
                else
                {
                    component.AnimationComponent animationComponent = e.GetComponent<component.AnimationComponent>();
                    if (!animationComponent.isAnimating())
                        animationComponent.startAnimation(14);
                    Texture2D currentTexture = textureManager.getTexture(animationComponent.getCurrentTexture());

                    spriteBatch.Draw(currentTexture, null, rectangle, animationComponent.getCurrentFrame(), new Vector2(rectangle.Width / 2, rectangle.Height / 2), physics.Rotation + drawable.AdditionalRotation, null, null, SpriteEffects.None, 0);
                }

                // Draw HP bar
                if (e.HasComponent<component.HealthComponent>())
                {
                    component.HealthComponent healthComponent = e.GetComponent<component.HealthComponent>();
                    if (healthComponent.Visible)
                    {
                        Texture2D hpbar = textureManager.getTexture(11);
                        Texture2D hp = textureManager.getTexture(12);

                        Vector2 pos = physics.Position * 32;
                        Vector2 offset = new Vector2(0, -12);
                        Rectangle hpbarRectangle = new Rectangle((int)(offset.X + pos.X), (int)(offset.Y + pos.Y), 52, 4);
                        Rectangle hpRectangle = new Rectangle((int)(offset.X + pos.X), (int)(offset.Y + pos.Y), (int)(healthComponent.Health / healthComponent.MaxHealth * 100 / 2), 2);

                        spriteBatch.Draw(hpbar, null, hpbarRectangle, null, new Vector2(hpbar.Width / 2, hpbar.Height / 2), 0f, null, null, SpriteEffects.None, 0);

                        spriteBatch.Draw(hp, null, hpRectangle, hpRectangle, new Vector2(hp.Width / 2, hp.Height / 2), 0f, null, null, SpriteEffects.None, 0);
                    }

                }
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
                Vector2 origin = new Vector2(drawable.texture.Width / 2, drawable.texture.Height / 2);
                Rectangle rectangle = new Rectangle(
                    new Point((int)(tileEntity.Position.X * Global.tileSize + origin.X), (int)(tileEntity.Position.Y * Global.tileSize + origin.Y)),
                    new Point(drawable.texture.Width, drawable.texture.Height)
                    );


                Color color = Color.White;
                color.A = 255;
                spriteBatch.Draw(drawable.texture, null, rectangle, null, origin, tileEntity.Rotation, null, color, SpriteEffects.None, 0);
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
                float newRotation = (float)Math.Atan2(dir.Y, dir.X);

                if (e.HasComponent<component.Formation>())
                {
                    component.Formation formation = e.GetComponent<component.Formation>();

                    Vector2 dir2 = (1.1f * (formation.Pos.toVector2()) + goal.pathGoal.GoalPos.toVector2()) - physics.Position;
                    if (dir2.Length() < 1.5f * formation.EntityFormation.Radius)
                    {
                        if (dir2.Length() > 0.25f)
                            dir2.Normalize();
                        else
                            dir2 = new Vector2(0f, 0f);

                        dir = dir2;
                    }
                    if (dir.Length() == 0f)
                        physics.rotateTo(formation.rot, 0.08f);
                }
                if (dir.Length() == 0f)
                    return;

                physics.rotateTo(newRotation, 0.08f);

                if (physics.Position.X < 0 || physics.Position.Y < 0 || physics.Position.X > Global.mapWidth * 32 || physics.Position.Y > Global.mapHeight * 32)
                {
                    physics.Position = physics.OldPosition;
                    physics.Velocity = new Vector2();
                }
                else if (Math.Abs(oldRotation - newRotation) <= 1)
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

                        if (dis < -0.25f)
                            physics.Position += -1.0f * normal * (dis + 0.25f);
                        //physics.Velocity += -0.5f * normal * dis;
                    }
                    else
                        break;
                }
            }

        }






    }
}
