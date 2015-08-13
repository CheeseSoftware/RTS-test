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
        private PathGoal pathGoal;

        public UnitController()
        {
            pathGoal = null;
        }

		public void updateGoal(EntityWorld entityWorld, Camera camera)
        {
            if (pathGoal == null)
                return;

            pathGoal.updatePath(); 
        }

		public void setPathGoal(PathGoal pathGoal)
		{
			this.pathGoal = pathGoal;
            pathGoal.updatePath();

            List<Entity> entities = pathGoal.getEntities();
            foreach (Entity entity in entities)
            {
                entity.GetComponent<component.Unit>().pathGoal = pathGoal;
            }
		}
    }
}
