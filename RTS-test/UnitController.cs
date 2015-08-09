//using Artemis;
//using Artemis.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace RTS_test
//{
//    public class UnitController
//    {
//        private PathGoal pathGoal;

//        public UnitController()
//        {
//            pathGoal = null;
//        }

//		public void updateGoal(EntityWorld entityWorld, Camera camera)
//        {
//            if (pathGoal == null)
//                return;

//            Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal)));
//            foreach (Entity entity in entities)
//            {
//                entity.GetComponent<component.Goal>().pathGoal = pathGoal;
//            }
//        }

//		public void setPathGoal(PathGoal pathGoal)
//		{
//			this.pathGoal = pathGoal;
//            pathGoal.updatePath();
//        }
//    }
//}
