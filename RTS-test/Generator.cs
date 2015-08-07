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

		public Generator()
		{

		}

		public void generate(TileMap tileMap, EntityWorld entityWorld)
		{
			occupied = new bool[tileMap.getWidth(), tileMap.getHeight()];
			Random random = new Random();

			// Generate tilemap
			Graphics.Tools.Noise.Primitive.BevinsGradient noise = new Graphics.Tools.Noise.Primitive.BevinsGradient(random.Next(10000), NoiseQuality.Best);

			for (int x = 0; x < tileMap.getWidth(); x++)
			{
				for (int y = 0; y < tileMap.getHeight(); y++)
				{

					ushort id = 0;

					float zoom = 0.03f;

					if (noise.GetValue(zoom * x, zoom * y, 0) > 0.3f)
						id = 3;

					else if (noise.GetValue(zoom * x, zoom * y, 0) > 0.0001f)
						id = 2;

					else
						id = 1;
					tileMap.setTile(x, y, id);
				}
			}

			// Generate forest
			Graphics.Tools.Noise.Primitive.SimplexPerlin forestNoise = new Graphics.Tools.Noise.Primitive.SimplexPerlin(random.Next(10000), NoiseQuality.Best);
			for (int x = 0; x < tileMap.getWidth(); x++)
			{
				for (int y = 0; y < tileMap.getHeight(); y++)
				{
					float zoom = 0.05f;
					if (forestNoise.GetValue(zoom * x, zoom * y) > 0.5f)
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

								if (checkX >= tileMap.getWidth() || checkY >= tileMap.getHeight() || occupied[checkX, checkY] || !tileMap.getTile(checkX, checkY).Name.Equals("grass"))
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


						entityWorld.CreateEntityFromTemplate("Tree", new object[] {
							new Vector2(x, y),
							(float)random.Next(360),
							"wood",
							500,
							5
						});
						//tileMap.setTile(x, y, 0);

						/*for (int xt = 0; xt < treeWidth; xt++)
						{
							for (int yt = 0; yt < treeHeight; yt++)
							{
								int checkX = xt + x;
								int checkY = yt + y;
								occupied[checkX, checkY] = true;
							}
						}*/
					}
				}
			}

		}
	}
}
