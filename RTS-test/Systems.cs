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
        public class EntityRenderer : EntityProcessingSystem<component.Position, component.Drawable>
        {
            private SpriteBatch spriteBatch;

            public EntityRenderer()
                : base(Aspect.All(typeof(component.Drawable), typeof(component.Position)))
            {
                spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            public override void LoadContent()
            {
                this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

            protected override void Process(Entity e, component.Position position, component.Drawable drawable)
			{
				Rectangle rectangle = new Rectangle(
                    new Point((int)position.pos.X, (int)position.pos.Y), 
                    new Point(drawable.texture.Width, drawable.texture.Height)
                    );

                spriteBatch.Draw(drawable.texture, rectangle, Color.White);
            }
        }

        [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
        public class UnitWalker : EntityProcessingSystem<component.Goal, component.Position, component.Velocity>
        {
            public UnitWalker()
                : base(Aspect.All(typeof(component.Goal), typeof(component.Position), typeof(component.Velocity)))
            {
            }

            public override void LoadContent()
            {
            }

            protected override void Process(Entity e, component.Goal goal, component.Position position, component.Velocity velocity)
            {
                if (goal.pathGoal == null)
                    return;

                Vector2 dir = goal.pathGoal.getDirection(position.pos);

                velocity.velocity += 0.1f * dir;
            }
        }

        [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
        public class MaxVelocity : EntityProcessingSystem<component.MaxVelocity, component.Velocity>
        {
            public MaxVelocity()
                : base(Aspect.All(typeof(component.MaxVelocity), typeof(component.Velocity)))
            {
            }

            public override void LoadContent()
            {
            }

            protected override void Process(Entity e, component.MaxVelocity maxVelocity, component.Velocity velocity)
            {
                if (velocity.velocity.Length() > maxVelocity.maxVelocity)
                {
                    velocity.velocity.Normalize();
                    velocity.velocity *= maxVelocity.maxVelocity;
                }
            }
        }





	}
}
