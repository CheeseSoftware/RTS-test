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
        private const float maxValue = 1000000f;

        private float[,] flowfield;
        private int2 size;
        private int2 goalPos;
        private TileMap tileMap;
        private Bag<Entity> units;
        //private List<uint2> unitPositions;

        private List<StepDirection> stepDirections = new List<StepDirection>();

        List<Node> nodesToExplore = new List<Node>();
        Dictionary<int2, Node> nodesToExploreMap = new Dictionary<int2, Node>();

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

            HashSet<Vector2> dirSet = new HashSet<Vector2>();

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (x == 0 && y == 0)
                        continue;

                    //if (x != 0 && y != 0)
                    //    continue;

                    Vector2 dir = new Vector2(x, y);
                    dir.Normalize();


                    if (dirSet.Contains(dir))
                        continue;

                    stepDirections.Add(new StepDirection(new int2(x, y)));
                    dirSet.Add(dir);
                }
            }

            stepDirections.Sort(delegate(StepDirection x, StepDirection y)
            {
                return x.dis.CompareTo(y.dis);
            });

            
            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    flowfield[x, y] = maxValue;
                }
            }

            nodesToExplore.Add(new Node(goalPos, 0f, 0f));
            flowfield[goalPos.x, goalPos.y] = 0f;
        }

        public void updatePath()
        {
            foreach (Entity entity in units)
            {
                component.Physics physics = entity.GetComponent<component.Physics>();

                int2 entityPos = new int2((int)(physics.Position.X+0.5f), (int)(physics.Position.Y+0.5f));
                int2 floorPos = new int2((int)(physics.Position.X), (int)(physics.Position.Y));

                if (entityPos.x < 0 || entityPos.y < 0 || entityPos.x >= size.x || entityPos.y >= size.y)
                    continue;
                if (flowfield[entityPos.x, entityPos.y] < maxValue)
                    continue;
                if (tileMap.getTile(entityPos.x, entityPos.y).IsSolid)
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

                    float dis = 2f;// 0.55f + tileMap.DisField.getDis(new Vector2((float)node.pos.x - 0.5f, (float)node.pos.y - 0.5f));

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
                        {
                            flowfield[newNodePos.x, newNodePos.y] = newNodeDis+1;
                            continue;
                        }

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

        public float getDis(Vector2 pos)
        {
            int2 floorPos = new int2((int)pos.X, (int)pos.Y);
            int2 ceilPos = new int2((int)pos.X + 1, (int)pos.Y + 1);

            if (floorPos.x < 0 || floorPos.y < 0 || ceilPos.x >= size.x || ceilPos.y >= size.y)
                return maxValue;

            Vector2 localPos = new Vector2(pos.X % 1f, pos.Y % 1f);
            float a = 1f - localPos.X;
            float b = 1f - localPos.Y;
            float c = 1f - a;
            float d = 1f - b;
            float[] dis = { 
                flowfield[floorPos.x, floorPos.y],
                flowfield[ceilPos.x, floorPos.y],
                flowfield[floorPos.x, ceilPos.y],
                flowfield[ceilPos.x, ceilPos.y]
            };


            float dis2 = a * b * dis[0] + c * b * dis[1] + a * d * dis[2] + c * d * dis[3];

            return dis2;
        }

        public Vector2 getDirection(Vector2 pos)
        {
            //int2 floorPos = new int2((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
            //if (floorPos.x < 0 || floorPos.y < 0 || floorPos.x + 1 >= size.x || floorPos.y + 1 >= size.y)
            //    return new Vector2(goalPos.x, goalPos.y) - pos;


            //Vector2 direction =
            //    (float)(flowfield[floorPos.x, floorPos.y]) * new Vector2(-1f, -1f)
            //    + (float)(flowfield[floorPos.x + 1, floorPos.y]) * new Vector2(1f, -1f)
            //    + (float)(flowfield[floorPos.x, floorPos.y + 1]) * new Vector2(-1f, 1f)
            //    + (float)(flowfield[floorPos.x + 1, floorPos.y + 1]) * new Vector2(1f, 1f);

            //if (direction.Length() > 0f)
            //    direction.Normalize();
            //return -direction;


            //Vector2 a = (float)(flowfield[floorPos.x, floorPos.y]) * Vector2.Normalize(new Vector2(-1f, -1f));
            //Vector2 b = (float)(flowfield[floorPos.x + 1, floorPos.y]) * Vector2.Normalize(new Vector2(+1f, -1f));
            //Vector2 c = (float)(flowfield[floorPos.x, floorPos.y + 1]) * Vector2.Normalize(new Vector2(-1f, +1f));
            //Vector2 d = (float)(flowfield[floorPos.x + 1, floorPos.y + 1]) * Vector2.Normalize(new Vector2(+1f, +1f));

            //Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            //if (normal.Length() > 0f)
            //    normal.Normalize();
            //return -normal;


            //////////////
            //int2 floorPos = new int2((int)Math.Round(pos.X) - 1, (int)Math.Round(pos.Y) - 1);
            //if (floorPos.x < 0 || floorPos.y < 0 || floorPos.x + 2 >= size.x || floorPos.y + 2 >= size.y)
            //    return new Vector2(goalPos.x, goalPos.y) - pos;

            //Vector2 a = (float)(flowfield[floorPos.x, floorPos.y]) * Vector2.Normalize(new Vector2(-1f, -1f));
            //Vector2 b = (float)(flowfield[floorPos.x + 2, floorPos.y]) * Vector2.Normalize(new Vector2(+1f, -1f));
            //Vector2 c = (float)(flowfield[floorPos.x, floorPos.y + 2]) * Vector2.Normalize(new Vector2(-1f, +1f));
            //Vector2 d = (float)(flowfield[floorPos.x + 2, floorPos.y + 2]) * Vector2.Normalize(new Vector2(+1f, +1f));

            //Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            //if (normal.Length() > 0f)
            //    normal.Normalize();
            //return -normal;

            ////pos = new Vector2((int)Math.Round(pos.X), (int)Math.Round(pos.Y));
            //////////////
            ////// Epsilon: 0.01f
            Vector2 a = getDis(pos + new Vector2(-0.51f, +0.00f)) * Vector2.Normalize(new Vector2(-1f, +0f));
            Vector2 b = getDis(pos + new Vector2(+0.51f, -0.00f)) * Vector2.Normalize(new Vector2(+1f, -0f));
            Vector2 c = getDis(pos + new Vector2(-0.00f, +0.51f)) * Vector2.Normalize(new Vector2(-0f, +1f));
            Vector2 d = getDis(pos + new Vector2(+0.00f, -0.51f)) * Vector2.Normalize(new Vector2(+0f, -1f));

            Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            if (normal.Length() > 0f)
                normal.Normalize();
            return -normal;
        }

        public Bag<Entity> getEntities()
        {
            return units;
        }

    }
}
