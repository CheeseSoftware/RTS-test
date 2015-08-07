using Artemis;
using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public class UnitController
    {
        public void update(EntityWorld entityWorld, Camera camera)
        {
            Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal)));

            PathGoal pathGoal = new PathGoal(new int2(0), new int2(13, 37));

            foreach(Entity entity in entities)
            {
                entity.GetComponent<component.Goal>().pathGoal = pathGoal;
            }
        }
    }
}
