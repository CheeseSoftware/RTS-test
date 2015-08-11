using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public class DisField
    {
        private int2 size;
        private float[,] disField;
        private bool[,] tiles;
        private bool[,] updateTiles;
        private Queue<int2> updateQueue = new Queue<int2>();

        public DisField(int2 size)
        {
            this.size = size;
            disField = new float[size.x, size.y];
            tiles = new bool[size.x, size.y];
            updateTiles = new bool[size.x, size.y];

            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    disField[x, y] = 3f;
                    tiles[x, y] = false;
                    updateTiles[x, y] = false;
                }
            }
        }

        public void update()
        {
            while (updateQueue.Count > 0)
            {
                int2 pos = updateQueue.Dequeue();
                updateTiles[pos.x, pos.y] = false;

                float dis = 3f;

                bool isSolid = getTile(pos.x, pos.y);

                if (isSolid)
                    dis = 1f;

                for (int xx = -2; xx <= 2; ++xx)
                {
                    for (int yy = -2; yy <= 2; ++yy)
                    {
                        if (xx == 0 && yy == 0)
                            continue;

                        bool tileIsSolid = getTile(pos.x + xx, pos.y + yy);
                        if (!(tileIsSolid ^ isSolid))
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


        public bool getTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return true;

            return tiles[x, y];
        }

        public float getDis(Vector2 pos)
        {
            int2 floorPos = new int2((int)pos.X, (int)pos.Y);
            int2 ceilPos = new int2((int)pos.X + 1, (int)pos.Y + 1);

            if (floorPos.x < 0 || floorPos.y < 0 || ceilPos.x >= size.x || ceilPos.y >= size.y)
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

        public void setTile(int x, int y, bool isSolid)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return;

            if (tiles[x, y] == isSolid)
                return;

            tiles[x, y] = isSolid;

            for (int xx = -2; xx <= 2; ++xx)
            {
                for (int yy = -2; yy <= 2; ++yy)
                {

                    int2 pos = new int2(x + xx, y + yy);
                    if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
                        continue;

                    if (updateTiles[pos.x, pos.y])
                        continue;

                    updateTiles[pos.x, pos.y] = true;
                    updateQueue.Enqueue(pos);
                }
            }
        }

        public int2 Size
        {
            get { return size; }
        }

    }
}
