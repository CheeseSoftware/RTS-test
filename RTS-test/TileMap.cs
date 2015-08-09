﻿using Graphics.Tools.Noise;
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
        private DisField disField;
        private TileManager tileManager;
        private int2 size;
        private PathGoal pathGoal = null;

        /***********************
         * Debug code!
         * // TODO: Remove debug code.
         * *********************/
        public void setPathGoal(PathGoal pathGoal)
        {
            this.pathGoal = pathGoal;
        }

		public TileMap(TileManager tileManager, int2 size)
		{
            this.tileManager = tileManager;
            this.size = size;
		}

        public void load()
        {
            tiles = new UInt16[size.x, size.y];//new Map<UInt16>(size.x, size.y);
            disField = new DisField(size);
        }

        public void update()
        {
            disField.update();
        }

		public void draw(SpriteBatch spriteBatch, TileManager tileManager)
		{
			Rectangle viewportWorldBoundry = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(viewportWorldBoundry.X / Global.tileSize, viewportWorldBoundry.Y / Global.tileSize, viewportWorldBoundry.Width / Global.tileSize, viewportWorldBoundry.Height / Global.tileSize);

			for (int x = tilesVisible.X; x <= tilesVisible.Right + 1; x++)
			{
				for (int y = tilesVisible.Y; y <= tilesVisible.Bottom + 1; y++)
				{

                    float dis = 3f;// getDis(new Vector2((float)x, (float)y)) / 4f;
					//Color color = new Color(dis, dis, dis);


					Color color = Color.White;

                    /***********************
                     * Debug code!
                     * // TODO: Remove debug code.
                     * *********************/
                    if (pathGoal != null)
                    {
                        Vector2 dir = pathGoal.getDirection(new Vector2((float)x+0.5f, (float)y+0.5f));
                        color = new Color(dir.Length(), 0.5f + 0.5f * dir.X, 0.5f + 0.5f * dir.Y);//new Color(0.5f + 0.5f * dir.Length(), 0.5f + 0.5f * dir.Length(), 0.5f + 0.5f * dir.Length());//
                    }

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
						int baseX = x * Global.tileSize % texture.Width;
						int baseY = y * Global.tileSize % texture.Height;

						Rectangle sourceRectangle = new Rectangle(baseX, baseY, Global.tileSize, Global.tileSize);

						spriteBatch.Draw(texture, new Vector2(x * Global.tileSize, y * Global.tileSize), null, sourceRectangle, null, 0f, null, color, SpriteEffects.None, 0f);
					}
					else // Draw normally
						spriteBatch.Draw(texture, new Vector2(x * Global.tileSize, y * Global.tileSize), color);
				}
			}
		}

        public TileData getTile(int x, int y)
        {
            UInt16 tileID = getTileID(x, y);
            return tileManager.getTile(tileID);
        }

        public UInt16 getTileID(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return 0;

            return tiles[x, y];
        }

		public void setTile(int x, int y, UInt16 tile)
		{
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return;

            if (tiles[x, y] == tile)
                return;

			tiles[x, y] = tile;

            // Notify disfield
            disField.setTile(x, y, tileManager.getTile(tile).IsSolid);
		}

        public DisField DisField
        {
            get { return disField; }
        }

        public TileManager TileManager
        {
            get { return tileManager; }
        }

		public int2 Size
        {
            get { return size; }
        }

	}
}
