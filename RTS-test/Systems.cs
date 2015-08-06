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

			public EntityRenderer() : base(Aspect.All(typeof(component.Drawable), typeof(component.Position)))
			{
				spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            }

			public override void LoadContent()
			{
				this.spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
			}

			protected override void Process(Entity e, component.Position position, component.Drawable drawable)
			{
				Rectangle rectangle = new Rectangle(new Point((int)position.pos.X, (int)position.pos.Y), new Point(32, 32));

				spriteBatch.Draw(drawable.texture, rectangle, Color.White);
			}

			protected override void ProcessEntities(IDictionary<int, Entity> entities)
			{
				
			}
		}


	}
}
