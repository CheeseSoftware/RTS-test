using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
	class TileMap
	{
		private int[,] tiles;
		private int width;
		private int height;

		public TileMap(int width, int height)
		{
			this.width = width;
			this.height = height;
			tiles = new int[width, height];

			generate();
		}

		public void generate()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					tiles[x, y] = 1; // grass
				}
			}
		}

		public int getTile(int x, int y)
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
