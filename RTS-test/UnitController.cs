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
        bool first = true;

        public void update(EntityWorld entityWorld, Camera camera)
        {
            if (!first) return;
            first = false;
            Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal)));

            PathGoal pathGoal = new PathGoal(new int2(Global.mapWidth, Global.mapHeight), new int2(13, 37));
           
            pathGoal.updatePath();

            foreach(Entity entity in entities)
            {
                entity.GetComponent<component.Goal>().pathGoal = pathGoal;
            }
        }
    }
}
