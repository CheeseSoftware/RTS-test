using Artemis;
using Graphics.Tools.Noise;
using Graphics.Tools.Noise.Builder;
using Microsoft.Xna.Framework;
using RTS_test.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class Land
    {
        private object o = new object();

        public Land()
        {

        }
    }


	class Generator
	{

		bool[,] occupied;
        Land[,] tileLands;
        List<Land> lands = new List<Land>();
        TileMap tileMap;
        Random random = new Random();

		public Generator(TileMap tileMap)
		{
            this.tileMap = tileMap;
		}

        public void generate(EntityWorld entityWorld)
        {
            occupied = new bool[tileMap.Size.x, tileMap.Size.y];


            // Create border forest
            fillArea(generateArea(tileMap.Size / 2, (float)tileMap.Size.x + 1, (float)tileMap.Size.x + 1), 4);
            fillArea(generateArea(tileMap.Size / 2, 0f, (float)tileMap.Size.x / 2), 1);

            // Forest
            for (int i = 0; i < 16; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                fillArea(generateArea(pos, 8f, 16f), 4);
            }

            // Small Forest
            for (int i = 0; i < 32; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                fillArea(generateArea(pos, 2f, 8f), 4);
            }



            // Create player land
            fillArea(generateArea(new int2(18, 18), 4, 16), 2);
            fillArea(createPath(new Point(18, 18), new Point(tileMap.Size.x/2, tileMap.Size.y/2), 2, 0.5f), 2);



            // Paths:
            //float curliness = 0.75f;
            //for (int i = 0; i < 3; ++i)
            //{
            //    fillArea(createPath(new Point(0, 0), new Point(tileMap.Size.x - 1, tileMap.Size.y - 1), i + 2, curliness), 2);
            //    fillArea(createPath(new Point(0, 0), new Point(tileMap.Size.x - 1, tileMap.Size.y - 1), i + 2, curliness), 2);
            //    fillArea(createPath(new Point(0, 0), new Point(tileMap.Size.x - 1, tileMap.Size.y - 1), i + 2, curliness), 2);

            //    curliness *= 0.85f;
            //}

            List<Point> players = new List<Point>();
            for (int i = 0; i < 8; ++i)
            {
                Point pos = new Point(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                players.Add(pos);
            }

            for (int i = 0; i < 8; ++i)
            {
                fillArea(createPath(players[i], players[(i+1)%8], 5, 0.25f), 2);
                fillArea(generateArea(new int2(players[i].X, players[i].Y), 2, 16), 2);
                fillArea(generateArea(new int2(players[i].X, players[i].Y), 2, 4), 3);
            }

            // River:
            fillArea(createPath(new Point(tileMap.Size.x - 1, 0), new Point(0, tileMap.Size.y - 1), 8, 0.5f), 3);

            for (int x = 0; x < tileMap.Size.x; x++)
            {
                for (int y = 0; y < tileMap.Size.y; y++)
                {
                    float zoom = 0.05f;
                    if (tileMap.getTileID(x, y) == 4)
                    {
                        int treeWidth = 2;
                        int treeHeight = 2;

                        bool canPlaceTree = true;
                        for (int xt = 0; xt < treeWidth; xt++)
                        {
                            for (int yt = 0; yt < treeHeight; yt++)
                            {
                                int checkX = xt + x;
                                int checkY = yt + y;

                                if (checkX >= tileMap.Size.x || checkY >= tileMap.Size.y || occupied[checkX, checkY] || !tileMap.getTile(checkX, checkY).Name.Equals("tree"))
                                {
                                    canPlaceTree = false;
                                    break;
                                }
                            }
                            if (!canPlaceTree)
                                break;
                        }
                        if (!canPlaceTree)
                            continue;


                        entityWorld.CreateEntityFromTemplate("Resource", new object[] {
                            new int2(x, y),
                            (float)random.Next(360),
                            "wood",
                            500,
                            5
                        });



                        for (int xt = 0; xt < treeWidth; xt++)
                        {
                            for (int yt = 0; yt < treeHeight; yt++)
                            {
                                int checkX = xt + x;
                                int checkY = yt + y;
                                occupied[checkX, checkY] = true;
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < tileMap.Size.x; x++)
            {
                for (int y = 0; y < tileMap.Size.y; y++)
                {
                    if (tileMap.getTile(x, y).Name.Equals("tree"))
                        tileMap.setTile(x, y, 1);
                }
            }



        }

        public Area generateArea(int2 pos, float minRadius, float maxRadius)
        {
            Area area = new Area();

            Rectangle rect = new Rectangle(pos.x - (int)maxRadius, pos.y - (int)maxRadius, (int)(2 * maxRadius), (int)(2 * maxRadius));
            Land land = new Land();

            float noiseRadius = maxRadius - minRadius;

            Graphics.Tools.Noise.Primitive.BevinsGradient noise = new Graphics.Tools.Noise.Primitive.BevinsGradient(random.Next(), NoiseQuality.Best);

            for (int y = rect.Y; y < rect.Y + rect.Height; ++y)
            {
                for (int x = rect.X; x < rect.X + rect.Width; ++x)
                {
                    if (x < 0 || y < 0 || x >= tileMap.Size.x || y >= tileMap.Size.y)
                        continue;

                    float dis = new Vector2(x - pos.x, y - pos.y).Length() / maxRadius;
                    float nvalue = Math.Abs(noise.GetValue((float)x / 64f, (float)y / 64f, 0f));

                    if (dis + noiseRadius / maxRadius * (nvalue) < 1f)
                    {
                        area[x, y] = true;
                    }


                }
            }

            return area;
        }

        public void createLine(Point a, Point b, int radius, ref Area area)
        {
            Queue<Point> points = new Queue<Point>();
            HashSet<Point> expandPoints = new HashSet<Point>();
            //PagedArray2D<bool> tiles = new PagedArray2D<bool>(false);

            Point[] expandDirections = new Point[]
            {
                new Point(1, 0),
                new Point(-1, 0),
                new Point(0, 1),
                new Point(0, -1)
            };

            bool steep = (Math.Abs(b.Y - a.Y) > Math.Abs(b.X - a.X));
            if (steep)
            {
                a = new Point(a.Y, a.X);
                b = new Point(b.Y, b.X);
            }
            if (a.X > b.X)
            {
                Point temp = a;
                a = b;
                b = temp;
            }

            Point delta = new Point(b.X - a.X, Math.Abs(b.Y - a.Y));

            float error = delta.X / 2f;
            int yStep = (a.Y < b.Y) ? 1 : -1;
            int y = a.Y;

            for (int x = a.X; x < b.X; ++x)
            {
                Point point;
                if (steep)
                    point = new Point(y, x);
                else
                    point = new Point(x, y);

                points.Enqueue(point);
                expandPoints.Add(point);

                error -= delta.Y;
                if (error < 0)
                {
                    y += yStep;
                    error += delta.X;
                }
            }

            for (int i = 0; i < radius; ++i)
            {
                int size = points.Count;
                for (int j = 0; j < size; ++j)
                {
                    Point point = points.Dequeue();
                    area[point.X, point.Y] = true;
                    expandPoints.Remove(point);

                    foreach(Point p in expandDirections)
                    {
                        Point newPoint = point + p;
                        if (p.X < 0 || p.Y < 0 || p.X >= tileMap.Size.x || p.Y >= tileMap.Size.y)
                            continue;

                        if (expandPoints.Contains(newPoint))
                            continue;

                        points.Enqueue(newPoint);
                        expandPoints.Add(newPoint);
                    }
                }
            }
        }

        public Area createPath(Point p1, Point p2, int radius, float curliness)
        {
            List<Point> points = new List<Point>();

            points.Add(p1);
            points.Add(p2);
            {
                int i = 0;
                while (i + 1 < points.Count)//for (int i = 0; i < points.Count-1; ++i)
                {
                    for (int j = 0; j < 100; ++j )
                    {
                        Point a = points[i];
                        Point b = points[i + 1];
                        float distance = (a - b).ToVector2().Length();
                        if (distance <= 8f)
                            break;

                        Point c = (a + b) / new Point(2, 2);
                        c.X += random.Next(-(int)(0.5f * distance * curliness), (int)(0.5f * distance * curliness));
                        c.Y += random.Next(-(int)(0.5f * distance * curliness), (int)(0.5f * distance * curliness));

                        c.X = Math.Min(Math.Max(c.X, 0), tileMap.Size.x - 1);
                        c.Y = Math.Min(Math.Max(c.Y, 0), tileMap.Size.y - 1);

                        points.Insert(i + 1, c);
                    }
                    ++i;
                }
            }

            Area area = new Area();
            for (int i = 0; i < points.Count - 1; ++i)
            {
                Point a = points[i];
                Point b = points[i + 1];
                createLine(a, b, radius, ref area);
            }

            return area;
        }

        public void fillArea(Area area, UInt16 tileID)
        {
            foreach (Point p in area)
                tileMap.setTile(p.X, p.Y, tileID);
        }

        public void replaceArea(Area area, UInt16 TileIDA, UInt16 TileIDB)
        {
            foreach (Point p in area)
            {
                if (tileMap.getTileID(p.X, p.Y) == TileIDA)
                    tileMap.setTile(p.X, p.Y, TileIDB);
            }
        }

	}

}
