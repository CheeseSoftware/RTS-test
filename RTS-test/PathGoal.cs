using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Artemis;
using Artemis.Utils;

namespace RTS_test
{
    struct StepDirection
    {
        public int2 pos;
        public float dis;
        public StepDirection(int2 pos)
        {
            this.pos = pos;
            dis = new Vector2((float)pos.x, (float)pos.y).Length();
        }
    }

   class PagedMap<T>
    {
        struct Page<T>
        {
            public T[,] nodes;
        }

        private Dictionary<int2, Page<T>> pages = new Dictionary<int2,Page<T>>();
        private T nullNode;

        public PagedMap(T nullNode)
        {
            this.nullNode = nullNode;
        }

        T getNode(int2 pos)
        {
            int2 pagePos = pos/16;
            int2 localPos = pos - pagePos;

            if (!pages.ContainsKey(pagePos))
                return nullNode;

            return pages[pagePos].nodes[localPos.x, localPos.y];
        }

        void setNode(int2 pos, T node)
        {
            int2 pagePos = pos/16;
            int2 localPos = pos - pagePos;

            if (!pages.ContainsKey(pagePos))
            {
                Page<T> page = new Page<T>();
                page.nodes = new T[8, 8];
                page.nodes[localPos.x, localPos.y] = node;
                pages.Add(pagePos, new Page<T>());

            }
            else
                pages[pagePos].nodes[localPos.x, localPos.y] = node;
        }
    }

    public class PathGoal
    {
        private float[,] flowfield;
        private int2 size;
        private int2 goalPos;
        private TileMap tileMap;
        private Bag<Entity> units;
        //private List<uint2> unitPositions;

        private List<StepDirection> stepDirections = new List<StepDirection>();

        class Node : IComparable
        {
            public int2 pos;
            public float dis;
            public float f;

            public Node(int2 pos, float dis, float f)
            {
                this.pos = pos;
                this.dis = dis;
                this.f = f;
            }

            public int CompareTo(object obj)
            {
                return this.f.CompareTo((obj as Node).f);
            }
        }

        private float calcF(float dis, int2 a, int2 b)
        {
            Vector2 delta = new Vector2(a.x - b.x, a.y - b.y);
            return dis + delta.Length();
        }

        public PathGoal(Bag<Entity> units, TileMap tileMap, int2 size, int2 goalPos)
        {
            this.units = units;
            this.tileMap = tileMap;
            this.size = size;
            this.flowfield = new float[size.x,size.y];
            this.goalPos = goalPos;

            //stepDirections = new int2[] { new int2(-1,0), new int2(1,0), new int2(0,-1), new int2(0,1) };

            for (int x = -3; x <= 3; ++x)
            {
                for (int y = -3; y <= 3; ++y)
                {
                    if (x != 0 || y != 0)
                    stepDirections.Add(new StepDirection(new int2(x, y)));
                }
            }

            stepDirections.Sort(delegate(StepDirection x, StepDirection y)
            {
                return x.dis.CompareTo(y.dis);
            });
        }

        public void updatePath()
        {
            List<Node> nodesToExplore = new List<Node>();
            //Dictionary<int2, float> exploredNodes = new Dictionary<int2, float>();
            Dictionary<int2, Node> nodesToExploreMap = new Dictionary<int2, Node>();


            float maxValue = 1000000f;
            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    flowfield[x, y] = maxValue;
                }
            }

            nodesToExplore.Add(new Node(goalPos, 0f, 0f));
            flowfield[goalPos.x, goalPos.y] = 0f;

            foreach (Entity entity in units)
            {
                component.Physics physics = entity.GetComponent<component.Physics>();

                int2 entityPos = new int2((int)(physics.Position.X+0.5f), (int)(physics.Position.Y+0.5f));

                if (entityPos.x < 0 || entityPos.y < 0 || entityPos.x >= size.x || entityPos.y >= size.y)
                    continue;
                if (flowfield[entityPos.x, entityPos.y] < maxValue)
                    continue;

                foreach (Node node in nodesToExplore)
                {
                    node.f = calcF(node.dis, entityPos, node.pos);
                }
                nodesToExplore.Sort();

                while (nodesToExplore.Count() > 0)
                {
                    Node node = nodesToExplore[0];
                    nodesToExplore.RemoveAt(0);
                    float nodeDis = flowfield[node.pos.x, node.pos.y];

                    nodesToExploreMap.Remove(node.pos);

                    float dis = tileMap.getDis(new Vector2(node.pos.x, node.pos.y));


                    for (int i = 0; i < stepDirections.Count; ++i)
                    {
                        StepDirection stepDirection = stepDirections[i];
                        if (dis < stepDirection.dis)
                            break;

                        int2 newNodePos = node.pos + stepDirection.pos;
                        float newNodeDis = nodeDis + stepDirection.dis;

                        if (newNodePos.x < 0 || newNodePos.y < 0 || newNodePos.x >= size.x || newNodePos.y >= size.y)
                            continue;

                        TileData tile = tileMap.getTile(newNodePos.x, newNodePos.y);
                        if (tile.IsSolid)
                            continue;

                        //if (exploredNodes.ContainsKey(newNodePos))
                        //{
                        //    float oldDis = exploredNodes[newNodePos];
                        //    if (newNodeDis >= oldDis)
                        //        continue;


                        //    exploredNodes[newNodePos] = newNodeDis;
                        //}
                        //else
                        //    exploredNodes.Add(newNodePos, newNodeDis);

                        if (newNodeDis >= flowfield[newNodePos.x, newNodePos.y])
                            continue;

                        flowfield[newNodePos.x, newNodePos.y] = newNodeDis;

                        if (nodesToExploreMap.ContainsKey(newNodePos))
                        {
                            nodesToExploreMap.Remove(newNodePos);
                        }

                        Node newNode = new Node(newNodePos, newNodeDis, calcF(newNodeDis, entityPos, node.pos));
                        int index = nodesToExplore.BinarySearch(newNode);
                        if (index < 0)
                            index = ~index;
                        nodesToExplore.Insert(index, newNode);
                        nodesToExploreMap.Add(newNodePos, newNode);
                    }

                    if (node.pos.x == entityPos.x && node.pos.y == entityPos.y)
                        break;
                }
            }

        }

        //void unregisterUnit(Unit unit)
        //{
        //    int index = units.Search(unit);
        //    units.RemoveAt(index);
        //    unitPositions.RemoveAt(index);
        //}

        public Vector2 getDirection(Vector2 pos)
        {
            int2 floorPos = new int2((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
            if (floorPos.x < 0 || floorPos.y < 0 || floorPos.x + 1 >= size.x || floorPos.y + 1 >= size.y)
                return new Vector2(goalPos.x, goalPos.y) - pos;


            Vector2 direction =
                (float)(flowfield[floorPos.x, floorPos.y]) * new Vector2(-1f, -1f)
                + (float)(flowfield[floorPos.x + 1, floorPos.y]) * new Vector2(1f, -1f)
                + (float)(flowfield[floorPos.x, floorPos.y + 1]) * new Vector2(-1f, 1f)
                + (float)(flowfield[floorPos.x + 1, floorPos.y + 1]) * new Vector2(1f, 1f);

            if (direction.Length() > 0f)
                direction.Normalize();
            return -direction;
        }


    }
}
