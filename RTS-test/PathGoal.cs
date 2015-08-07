using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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

    public class PathGoal
    {
        private float[,] flowfield;
        private int2 size;
        private int2 goalPos;
        private TileMap tileMap;
        //private List<Unit> units;
        //private List<uint2> unitPositions;

        private List<StepDirection> stepDirections = new List<StepDirection>();

        struct Node
        {
            public Node(int2 pos)
            {
                this.pos = pos;
            }
            public int2 pos;
        }

        public PathGoal(TileMap tileMap, int2 size, int2 goalPos)
        {
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
            Queue<Node> nodesToExplore = new Queue<Node>();
            //Dictionary<int2, float> exploredNodes = new Dictionary<int2, float>();
            HashSet<int2> nodesToExploreSet = new HashSet<int2>();

            

            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    flowfield[x, y] = 1000000;
                }
            }

            nodesToExplore.Enqueue(new Node(goalPos));
            flowfield[goalPos.x, goalPos.y] = 0f;

            while(nodesToExplore.Count() > 0)
            {
                Node node = nodesToExplore.Dequeue();
                float nodeDis = flowfield[node.pos.x, node.pos.y];

                nodesToExploreSet.Remove(node.pos);

                float dis = tileMap.getDis(new Vector2(node.pos.x, node.pos.y));
          

                for (int i = 0; i < stepDirections.Count; ++i )
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

                    if (!nodesToExploreSet.Contains(newNodePos))
                    {
                        nodesToExplore.Enqueue(new Node(newNodePos));
                        nodesToExploreSet.Add(newNodePos);
                    }
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
