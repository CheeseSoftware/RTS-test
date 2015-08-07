﻿using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
	[ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 1)]
	public class MovementSystem : EntityProcessingSystem<component.Position, component.Velocity>
	{

        private TileMap tileMap;

        public MovementSystem()
        {
            tileMap = EntitySystem.BlackBoard.GetEntry<TileMap>("TileMap");
        }

		/// <summary>Processes the specified entity.</summary>
		/// <param name="entity">The entity.</param>
		protected override void Process(Entity entity, component.Position transformComponent, component.Velocity velocityComponent)
		{
			if (velocityComponent != null)
			{
				if (transformComponent != null)
				{
					float ms = TimeSpan.FromTicks(this.EntityWorld.Delta).Milliseconds;

					transformComponent.X += (float)(velocityComponent.X * ms);
					transformComponent.Y += (float)(velocityComponent.Y * ms);

                    // TODO: Replace -1 with -radius.
                    float dis = tileMap.getDis(transformComponent.pos)-1;
                    Vector2 normal = tileMap.getNormal(transformComponent.pos);
                    if (dis < 0f)
                        transformComponent.pos += -normal * dis;
				}
			}


		}
	}
}
