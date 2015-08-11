using Artemis;
using Graphics.Tools.Noise;
using Graphics.Tools.Noise.Builder;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    
	class Generator
	{

		bool[,] occupied;
        TileMap tileMap;
        Random random = new Random();

		public Generator(TileMap tileMap)
		{
            this.tileMap = tileMap;
		}

		public void generate(EntityWorld entityWorld)
		{
            int seed = 1337; //random.Next(10000);

            occupied = new bool[tileMap.Size.x, tileMap.Size.y];
            Random random = new Random();

            // Generate tilemap
            Graphics.Tools.Noise.Primitive.BevinsGradient noise = new Graphics.Tools.Noise.Primitive.BevinsGradient(seed, NoiseQuality.Best);
            Graphics.Tools.Noise.Primitive.BevinsGradient noise2 = new Graphics.Tools.Noise.Primitive.BevinsGradient(seed, NoiseQuality.Best);

            for (int x = 0; x < tileMap.Size.x; x++)
            {
                for (int y = 0; y < tileMap.Size.y; y++)
                {

                    ushort id = 0;

                    float zoom = 0.01f;
                    float value = 0.67f * noise.GetValue(zoom * x, zoom * y, 0)
                        + 0.335f * noise2.GetValue(2 * zoom * x, 2 * zoom * y, 0);

                    if (value > 0.3f)
                        id = 3;

                    else if (value > 0.0001f)
                        id = 2;

                    else
                        id = 1;
                    tileMap.setTile(x, y, 1);
                }
            }

            //// Generate forest
            //Graphics.Tools.Noise.Primitive.SimplexPerlin forestNoise = new Graphics.Tools.Noise.Primitive.SimplexPerlin(seed, NoiseQuality.Best);
            //for (int x = 0; x < tileMap.Size.x; x++)
            //{
            //    for (int y = 0; y < tileMap.Size.y; y++)
            //    {
            //        float zoom = 0.05f;
            //        if (forestNoise.GetValue(zoom * x, zoom * y) > 0.2f)
            //        {
            //            int treeWidth = 2;
            //            int treeHeight = 2;

            //            bool canPlaceTree = true;
            //            for (int xt = 0; xt < treeWidth; xt++)
            //            {
            //                for (int yt = 0; yt < treeHeight; yt++)
            //                {
            //                    int checkX = xt + x;
            //                    int checkY = yt + y;

            //                    if (checkX >= tileMap.Size.x || checkY >= tileMap.Size.y || occupied[checkX, checkY] || !tileMap.getTile(checkX, checkY).Name.Equals("grass"))
            //                    {
            //                        canPlaceTree = false;
            //                        break;
            //                    }
            //                }
            //                if (!canPlaceTree)
            //                    break;
            //            }
            //            if (!canPlaceTree)
            //                continue;


            //            entityWorld.CreateEntityFromTemplate("Resource", new object[] {
            //                new int2(x, y),
            //                (float)random.Next(360),
            //                "wood",
            //                500,
            //                5
            //            });
            //            for (int xt = 0; xt < treeWidth; xt++)
            //            {
            //                for (int yt = 0; yt < treeHeight; yt++)
            //                {
            //                    int checkX = xt + x;
            //                    int checkY = yt + y;
            //                    occupied[checkX, checkY] = true;
            //                }
            //            }
            //        }
            //    }
            //}

            //// Generate stone resource 
            //for (int i = 0; i < Math.Sqrt(tileMap.Size.x * tileMap.Size.y) / 10; i++)
            //{
            //    int x = random.Next(tileMap.Size.x);
            //    int y = random.Next(tileMap.Size.y);

            //    int resourceWidth = 2;
            //    int resourceHeight = 2;
            //    bool canPlaceResource = true;

            //    for (int xt = 0; xt < resourceWidth; xt++)
            //    {
            //        for (int yt = 0; yt < resourceHeight; yt++)
            //        {
            //            int checkX = xt + x;
            //            int checkY = yt + y;

            //            if (checkX >= tileMap.Size.x || checkY >= tileMap.Size.y || occupied[checkX, checkY] || !tileMap.getTile(checkX, checkY).Name.Equals("grass"))
            //            {
            //                canPlaceResource = false;
            //                break;
            //            }
            //        }
            //        if (!canPlaceResource)
            //            break;
            //    }
            //    if (!canPlaceResource)
            //        continue;

            //    entityWorld.CreateEntityFromTemplate("Resource", new object[] {
            //                new int2(x, y),
            //                (float)0,
            //                "stone",
            //                500,
            //                8
            //            });

            //    for (int xt = 0; xt < resourceWidth; xt++)
            //    {
            //        for (int yt = 0; yt < resourceHeight; yt++)
            //        {
            //            int checkX = xt + x;
            //            int checkY = yt + y;
            //            occupied[checkX, checkY] = true;
            //        }
            //    }

            //}

            for (int i = 0; i < 32; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                createClump(pos, 1024, 4);
            }

            for (int i = 0; i < 1024; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                createClump(pos, 64, 4);
            }

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

        public void createClump(int2 pos, int size, UInt16 tileID)
        {
            HashSet<int2> positionSet = new HashSet<int2>();
            List<int2> expandPositions = new List<int2>();
            List<int2> expandDirections = new List<int2>();
            expandDirections.Add(new int2(-1, 0));
            expandDirections.Add(new int2(1, 0));
            expandDirections.Add(new int2(0, -1));
            expandDirections.Add(new int2(0, 1));
            //expandDirections.Add(new int2(-1, -1));
            //expandDirections.Add(new int2(1, 1));
            //expandDirections.Add(new int2(1, -1));
            //expandDirections.Add(new int2(-1, 1));

            expandPositions.Add(pos);

            for (int i = 0; i < size && expandPositions.Count > 0; ++i)
            {
                int index = random.Next(random.Next(expandPositions.Count)+8) % expandPositions.Count;
                int2 nodePos = expandPositions[index];
                expandPositions.RemoveAt(index);
                tileMap.setTile(nodePos.x, nodePos.y, tileID);

                for (int j = 0; j < expandDirections.Count; ++j)
                {
                    int2 newPos = nodePos + expandDirections[j];

                    if (newPos.x < 0 || newPos.y < 0 || newPos.x >= tileMap.Size.x || newPos.y >= tileMap.Size.y)
                        continue;

                    if (positionSet.Contains(newPos))
                        continue;

                    expandPositions.Add(newPos);
                    positionSet.Add(newPos);
                }
            }
        }
	}

}
