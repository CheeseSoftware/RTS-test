using Graphics.Tools.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
	class TileMap
	{
		private UInt16[,] tiles;
		private int width;
		private int height;

		public TileMap(int width, int height)
		{
			this.width = width;
			this.height = height;
            tiles = new UInt16[width, height];

			generate();
		}

		public void generate()
		{
			Random random = new Random();
			Graphics.Tools.Noise.Primitive.SimplexPerlin noise = new Graphics.Tools.Noise.Primitive.SimplexPerlin(random.Next(100000), NoiseQuality.Standard);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					ushort id = 0;

					float zoom = 0.05f;

					if (noise.GetValue(zoom * x, zoom * y) > 0.3f)
						id = 1;

					else if (noise.GetValue(zoom * x, zoom * y) > 0.0001f)
						id = 2;

					else
						id = 3;

					/*else if (distanceFromCenter + 0.33 * noise.GetValue(x * 0.015625F, y * 0.015625F, 48) > 0.5)
						blockMap.setBlock(x, y, new NormalBlock((int)Skylight.BlockIds.Blocks.Sand.GRAY, 0));

					else// if (noise.GetValue(x * 0.015625F, y * 0.015625F, 160) > 0)
						blockMap.setBlock(x, y, new NormalBlock(Skylight.BlockIds.Blocks.Sand.BROWN, 0));*/

					tiles[x, y] = id;
				}
			}
		}

        public UInt16 getTile(int x, int y)
		{
			return tiles[x, y];
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
