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
        private List<Entity> entities;
        private HashSet<int2> nodes = new HashSet<int2>();
        private List<int2> nodeList = new List<int2>();
        private Dictionary<int2, Entity> entityPositions = new Dictionary<int2, Entity>();
        private Queue<int2> nodeExpandList = new Queue<int2>();
        private int2 goalPos;
        private DisFieldMixer disFieldMixer;
        private int2[] stepDirections = 
        {
            new int2(-1, 0),
            new int2(1, 0),
            new int2(0, -1),
            new int2(0, 1),
            new int2(-1, -1),
            new int2(1, 1),
            new int2(1, -1),
            new int2(-1, 1)
            
        };
        float radius = 0f;
        private int updateIndex = 0;

        public EntityFormation(List<Entity> entities, DisFieldMixer disFieldMixer, int2 goalPos)
        {
            this.entities = entities;
            this.disFieldMixer = disFieldMixer;
            this.goalPos = goalPos;

            int formationIndex = 0;

            entities.Sort(delegate(Entity a, Entity b)
            {
                // sort by formationIndex
                return a.GetComponent<component.Formation>().FormationIndex.CompareTo(b.GetComponent<component.Formation>().FormationIndex);
            });

            foreach(Entity e in entities)
            {
                if (!e.HasComponent<component.Physics>())
                    continue;

                e.GetComponent<component.Physics>().Body.OnCollision += this.onEntityCollision;

                e.GetComponent<component.Formation>().FormationIndex = formationIndex;
                formationIndex++;
            }
        }

        ~EntityFormation()
        {
            foreach (Entity e in entities)
            {
                if (!e.HasComponent<component.Physics>())
                    continue;

                e.GetComponent<component.Physics>().Body.OnCollision -= this.onEntityCollision;
            }
        }

        public void update()
        {
            if (nodeList.Count == 0)
            {
                entities.Sort(delegate(Entity a, Entity b)
                {
                    // sort by formationIndex
                    return a.GetComponent<component.Formation>().FormationIndex.CompareTo(b.GetComponent<component.Formation>().FormationIndex);
                });

                radius = 0f;

                nodeExpandList.Enqueue(new int2(0));
                nodes.Add(new int2(0));

                for (int i = 0; i < entities.Count; ++i)
                {
                    if (nodeExpandList.Count == 0)
                        break;

                    int2 nodePos = nodeExpandList.Dequeue();
                    nodeList.Add(nodePos);
                    radius = Math.Max(radius, nodePos.toVector2().Length()+1f);

                    for (int j = 0; j < stepDirections.Count(); ++j)
                    {
                        int2 newPos = nodePos + stepDirections[j];

                        if (disFieldMixer.getDis(1.1f*newPos.toVector2()+goalPos.toVector2()) < 0)
                            continue;

                        if (nodes.Contains(newPos))
                            continue;

                        nodeExpandList.Enqueue(newPos);
                        nodes.Add(newPos);
                    }

                }

                entityPositions.Clear();
                for (int i = 0; i < nodeList.Count; ++i)
                {
                    component.Formation formation = entities[i].GetComponent<component.Formation>();
                    formation.Pos = nodeList[i];
                    entityPositions.Add(nodeList[i], entities[i]);
                }
            }

            {
                int2 posA = entities[updateIndex].GetComponent<component.Formation>().Pos;

                // Get every neighbor
                for (int i = 0; i < stepDirections.Length; ++i)
                {
                    int2 posB = posA + stepDirections[i];

                    if (!entityPositions.ContainsKey(posB))
                        continue;

                    Entity e1 = entityPositions[posA];
                    Entity e2 = entityPositions[posB];

                    checkSwapEntities(e1, e2);
                }


                updateIndex++;
            }
        }

        public bool onEntityCollision(FarseerPhysics.Dynamics.Fixture f1, FarseerPhysics.Dynamics.Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (f1.Body.UserData is Entity && f1.Body.UserData is Entity)
            {
                Entity e1 = f1.Body.UserData as Entity;
                Entity e2 = f2.Body.UserData as Entity;

                checkSwapEntities(e1, e2);
            }
            return true;
        }

        private void checkSwapEntities(Entity e1, Entity e2)
        {
            if (!entities.Contains(e1) || !entities.Contains(e2))
                return;

            if (!e1.HasComponent<component.Physics>() || !e2.HasComponent<component.Physics>())
                return;

            component.Formation fo1 = e1.GetComponent<component.Formation>();
            component.Formation fo2 = e2.GetComponent<component.Formation>();
            component.Physics p1 = e1.GetComponent<component.Physics>();
            component.Physics p2 = e2.GetComponent<component.Physics>();

            float disA = (p1.Position - fo1.Pos.toVector2()).Length() + (p2.Position - fo2.Pos.toVector2()).Length();
            float disB = (p1.Position - fo2.Pos.toVector2()).Length() + (p2.Position - fo1.Pos.toVector2()).Length();

            if (disA <= disB)
                return;

            // Swap
            int2 temp = fo1.Pos;
            fo1.Pos = fo2.Pos;
            fo2.Pos = temp;

            // Swap
            int temp2 = fo1.FormationIndex;
            fo1.FormationIndex = fo2.FormationIndex;
            fo2.FormationIndex = temp2;

            // Swap
            entityPositions[fo1.Pos] = e1;
            entityPositions[fo2.Pos] = e2;
        }

        public float Radius
        {
            get { return radius; }
        }

    }
}
