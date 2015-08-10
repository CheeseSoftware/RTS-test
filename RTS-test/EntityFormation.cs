using Artemis;
using Artemis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public class EntityFormation
    {
        private Bag<Entity> entities;
        private HashSet<int2> nodes = new HashSet<int2>();
        private List<int2> nodeList = new List<int2>();
        private Queue<int2> nodeExpandList = new Queue<int2>();
        private int2 goalPos;
        private DisFieldMixer disFieldMixer;
        private int2[] stepDirections = 
        {
            new int2(-1, 0),
            new int2(1, 0),
            new int2(0, -1),
            new int2(0, 1)
        };
        float radius = 0f;

        public EntityFormation(Bag<Entity> entities, DisFieldMixer disFieldMixer, int2 goalPos)
        {
            this.entities = entities;
            this.disFieldMixer = disFieldMixer;
            this.goalPos = goalPos;
        }

        public void update()
        {
            if (nodeList.Count == 0)
            {
                radius = 0f;

                nodeExpandList.Enqueue(goalPos);
                nodes.Add(goalPos);

                for (int i = 0; i < entities.Count; ++i)
                {
                    if (nodeExpandList.Count == 0)
                        break;

                    int2 nodePos = nodeExpandList.Dequeue();
                    nodeList.Add(nodePos);
                    radius = Math.Max(radius, (nodePos-goalPos).toVector2().Length()+1f);

                    for (int j = 0; j < stepDirections.Count(); ++j)
                    {
                        int2 newPos = nodePos + stepDirections[j];

                        if (disFieldMixer.getDis(newPos.toVector2()) < 0)
                            continue;

                        if (nodes.Contains(newPos))
                            continue;

                        nodeExpandList.Enqueue(newPos);
                        nodes.Add(newPos);
                    }

                }

                for (int i = 0; i < nodeList.Count; ++i)
                {
                    component.Formation formation = entities[i].GetComponent<component.Formation>();
                    formation.Pos = nodeList[i];
                }
            }
        }

    }
}
