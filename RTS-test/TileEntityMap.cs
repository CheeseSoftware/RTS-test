using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class TileEntityMap
    {
        private Map<bool> tiles;
        private float[,] disField;
        private int width;
        private int height;

        private Queue<int2> updateQueue = new Queue<int2>();
        private HashSet<int2> updateTiles = new HashSet<int2>();

        public TileEntityMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            tiles = new Map<bool>(width, height);
            disField = new float[width, height];
        }

        public void updateDisField()
        {
            while (updateQueue.Count > 0)
            {
                int2 pos = updateQueue.Dequeue();
                updateTiles.Remove(pos);

                float dis = 3f;

                bool isSolid = getTileSolid(pos.x, pos.y);

                if (isSolid)
                    dis = 1f;

                for (int xx = -2; xx <= 2; ++xx)
                {
                    for (int yy = -2; yy <= 2; ++yy)
                    {
                        if (xx == 0 && yy == 0)
                            continue;

                        bool tile = getTileSolid(pos.x + xx, pos.y + yy);
                        if (!(tile ^ tile))
                            continue;

                        Vector2 b = new Vector2(0.5f, 0.5f);
                        Vector2 p = new Vector2((float)(xx), (float)(yy));

                        Vector2 d = new Vector2(Math.Abs(p.X), Math.Abs(p.Y)) - b;
                        float dd = Math.Min(Math.Max(d.X, d.Y), 0f) +
                                 Vector2.Max(d, new Vector2(0f, 0f)).Length();

                        dis = Math.Min(dis, dd);
                    }
                }
                if (isSolid)
                    disField[pos.x, pos.y] = -dis;
                else
                    disField[pos.x, pos.y] = dis;
            }
        }

        public float getDis(Vector2 pos)
        {
            int2 floorPos = new int2((int)pos.X, (int)pos.Y);
            int2 ceilPos = new int2((int)pos.X + 1, (int)pos.Y + 1);

            if (floorPos.x < 0 || floorPos.y < 0 || ceilPos.x >= width || ceilPos.y >= height)
                return 5f;

            Vector2 localPos = new Vector2(pos.X % 1f, pos.Y % 1f);
            float a = 1f - localPos.X;
            float b = 1f - localPos.Y;
            float c = 1f - a;
            float d = 1f - b;
            float[] dis = {
                disField[floorPos.x, floorPos.y],
                disField[ceilPos.x, floorPos.y],
                disField[floorPos.x, ceilPos.y],
                disField[ceilPos.x, ceilPos.y]
            };


            float dis2 = a * b * dis[0] + c * b * dis[1] + a * d * dis[2] + c * d * dis[3];

            return dis2;
        }

        public Vector2 getNormal(Vector2 pos)
        {
            Vector2 v1 = pos + new Vector2(-0.1f, -0.1f);
            Vector2 v2 = Vector2.Normalize(new Vector2(-0.1f, -0.1f));
            float f1 = getDis(pos + new Vector2(-0.1f, -0.1f));
            Vector2 f2 = f1 * v2;

            Vector2 a = getDis(pos + new Vector2(-0.1f, -0.1f)) * Vector2.Normalize(new Vector2(-0.1f, -0.1f));
            Vector2 b = getDis(pos + new Vector2(+0.1f, -0.1f)) * Vector2.Normalize(new Vector2(+0.1f, -0.1f));
            Vector2 c = getDis(pos + new Vector2(-0.1f, +0.1f)) * Vector2.Normalize(new Vector2(-0.1f, +0.1f));
            Vector2 d = getDis(pos + new Vector2(+0.1f, +0.1f)) * Vector2.Normalize(new Vector2(+0.1f, +0.1f));

            Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            if (normal.Length() > 0f)
                normal.Normalize();
            return normal;
        }

        public bool getTileSolid(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return true;
            return tiles.getData(x, y);
        }

        public void setTileSolid(int x, int y, bool tile)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;


            tiles.setData(x, y, tile);

            for (int xx = -2; xx <= 2; ++xx)
            {
                for (int yy = -2; yy <= 2; ++yy)
                {
                    int2 pos = new int2(x + xx, y + yy);
                    if (pos.x < 0 || pos.y < 0 || pos.x >= width || pos.y >= height)
                        continue;

                    if (updateTiles.Contains(pos))
                        continue;

                    updateTiles.Add(pos);
                    updateQueue.Enqueue(pos);
                }
            }
        }

        public int getWidth()
        {
            return this.width;
        }

        public int getHeight()
        {
            return this.height;
        }
    }
}
