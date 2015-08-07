using Graphics.Tools.Noise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
	public class TileMap
	{
		private UInt16[,] tiles;
        private TileManager tileManager;
		private int width;
		private int height;

		public TileMap(TileManager tileManager, int width, int height)
		{
            this.tileManager = tileManager;
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
					tiles[x, y] = id;
				}
			}
		}

		public void draw(SpriteBatch spriteBatch, TileManager tileManager)
		{
			Rectangle viewportWorldBoundry = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(viewportWorldBoundry.X / 32, viewportWorldBoundry.Y / 32, viewportWorldBoundry.Width / 32, viewportWorldBoundry.Height / 32);

			//Draw tilemap
			for (int x = tilesVisible.X; x <= tilesVisible.Right + 1; x++)
			{
				for (int y = tilesVisible.Y; y <= tilesVisible.Bottom + 1; y++)
				{
					if (x < 0 || y < 0 || x >= Global.mapWidth || y >= Global.mapHeight)
						continue;
					UInt16 tile = getTileID(x, y);
					TileData tileData = tileManager.getTile(tile);
					Texture2D texture = tileData.Texture;

					if (texture == null)
						continue;

					if (texture.Width > Global.tileSize || texture.Height > Global.tileSize)
					{
						// Draw parts of a larger texture to look nice
						int baseX = x * 32 % texture.Width;
						int baseY = y * 32 % texture.Height;

						Rectangle sourceRectangle = new Rectangle(baseX, baseY, Global.tileSize, Global.tileSize);

						spriteBatch.Draw(texture, new Vector2(x * 32, y * 32), null, sourceRectangle);
					}
					else // Draw normally
						spriteBatch.Draw(texture, new Vector2(x * 32, y * 32));
				}
			}
		}

        public TileManager getTileManager()
        {
            return tileManager;
        }

        public TileData getTile(int x, int y)
        {
            UInt16 tileID = tiles[x, y];
            return tileManager.getTile(tileID);
        }

        public UInt16 getTileID(int x, int y)
        {
            return tiles[x, y];
        }

		public void setTile(int x, int y, UInt16 tile)
		{
			tiles[x, y] = tile;
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
