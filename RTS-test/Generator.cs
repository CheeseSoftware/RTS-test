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
            createLand(tileMap.Size / 2, (float)tileMap.Size.x+1, (float)tileMap.Size.x+1, 4);
            createLand(tileMap.Size / 2, 0f, (float)tileMap.Size.x / 2, 1);

            // Forest
            for (int i = 0; i < 16; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                createClump(pos, 256, 4);
            }

            // Small Forest
            for (int i = 0; i < 32; ++i)
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                createClump(pos, 64, 4);
            }

            // Create lakes
            for (int i = 0; i < 16; ++i )
            {
                int2 pos = new int2(random.Next(tileMap.Size.x), random.Next(tileMap.Size.y));
                createLand(pos, 4, 16, 3);
            }

            // Create player land
            createLand(new int2(18, 18), 32, 64, 2);

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

        public Land createLand(int2 pos, float minRadius, float maxRadius, UInt16 tileID)
        {
            Rectangle rect = new Rectangle(pos.x - (int)maxRadius, pos.y - (int)maxRadius, (int)(2 * maxRadius), (int)(2 * maxRadius));
            Land land = new Land();

            float noiseRadius = maxRadius - minRadius;

            Graphics.Tools.Noise.Primitive.BevinsGradient noise = new Graphics.Tools.Noise.Primitive.BevinsGradient(random.Next(10000), NoiseQuality.Best);

            for (int y = rect.Y; y < rect.Y + rect.Height; ++y)
            {
                for (int x = rect.X; x < rect.X + rect.Width; ++x)
                {
                    if (x < 0 || y < 0 || x >= tileMap.Size.x || y >= tileMap.Size.y)
                        continue;

                    float dis = new Vector2(x - pos.x, y - pos.y).Length()/maxRadius;
                    float nvalue = Math.Abs(noise.GetValue((float)x / 64f, (float)y / 64f, 0f));

                    if (dis+noiseRadius/maxRadius*(nvalue) < 1f)
                    {
                        //tileLands[x, y] = land;
                        tileMap.setTile(x, y, tileID);
                    }


                }
            }

            return land;
        }

	}

}
