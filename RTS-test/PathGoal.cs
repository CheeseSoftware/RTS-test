﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RTS_test
{
    class PathGoal
    {
        private int[,] flowfield;
        private int2 size;
        private int2 goalPos;
        //private List<Unit> units;
        //private List<uint2> unitPositions;

        private int2[] stepDirections;

        struct Node
        {
            public Node(int2 pos, int distance)
            {
                this.pos = pos;
                this.distance = distance;
            }
            public int2 pos;
            public int distance;
        }

        public PathGoal(int2 size)
        {
            this.size = size;
            this.flowfield = new int[size.x,size.y];

            stepDirections = new int2[] { new int2(-1,0), new int2(1,0), new int2(0,-1), new int2(0,1) };
        }

        public void updatePath()
        {
            Queue<Node> nodesToExplore = new Queue<Node>();
            HashSet<int2> exploredNodes = new HashSet<int2>();

            nodesToExplore.Enqueue(new Node(goalPos, 0));

            while(nodesToExplore.Count() > 0)
            {
                Node node = nodesToExplore.Dequeue();
                exploredNodes.Add(node.pos);
                flowfield[node.pos.x, node.pos.y] = node.distance;

                for (int i = 0; i < 4; ++i)
                {
                    int2 newNodePos = node.pos + stepDirections[i];

                    if (newNodePos.x < 0 || newNodePos.y < 0 || newNodePos.x >= size.x || newNodePos.y >= size.y)
                        continue;
                    if (exploredNodes.Contains(newNodePos))
                        continue;

                    nodesToExplore.Enqueue(new Node(newNodePos, node.distance+1));
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

            Vector2 direction =
                (float)(flowfield[floorPos.x, floorPos.y]) * new Vector2(-1f, -1f)
                + (float)(flowfield[floorPos.x + 1, floorPos.y]) * new Vector2(1f, -1f)
                + (float)(flowfield[floorPos.x, floorPos.y + 1]) * new Vector2(-1f, 1f)
                + (float)(flowfield[floorPos.x + 1, floorPos.y + 1]) * new Vector2(1f, 1f);

            direction.Normalize();
            return direction;
        }


    }
}
