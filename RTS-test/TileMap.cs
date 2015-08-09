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
		private Map<UInt16> tiles;
        private float[,] disField;
        private TileManager tileManager;
		private int width;
		private int height;
        //private PathGoal pathGoal = null;

        /***********************
         * Debug code!
         * // TODO: Remove debug code.
         * *********************/
        /*public void setPathGoal(PathGoal pathGoal)
        {
            this.pathGoal = pathGoal;
        }*/

        private Queue<int2> updateQueue = new Queue<int2>();
        private HashSet<int2> updateTiles = new HashSet<int2>();

		public TileMap(TileManager tileManager, int width, int height)
		{
            this.tileManager = tileManager;
			this.width = width;
			this.height = height;
		}

        public void load()
        {
            tiles = new Map<UInt16>(width, height);
            disField = new float[width, height];
        }

		public void draw(SpriteBatch spriteBatch, TileManager tileManager)
		{
			Rectangle viewportWorldBoundry = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(viewportWorldBoundry.X / Global.tileSize, viewportWorldBoundry.Y / Global.tileSize, viewportWorldBoundry.Width / Global.tileSize, viewportWorldBoundry.Height / Global.tileSize);

			for (int x = tilesVisible.X; x <= tilesVisible.Right + 1; x++)
			{
				for (int y = tilesVisible.Y; y <= tilesVisible.Bottom + 1; y++)
				{

                    float dis = getDis(new Vector2((float)x, (float)y)) / 4f;
					//Color color = new Color(dis, dis, dis);


					Color color = Color.White;

                    /***********************
                     * Debug code!
                     * // TODO: Remove debug code.
                     * *********************/
                    /*if (pathGoal != null)
                    {
                        Vector2 dir = pathGoal.getDirection(new Vector2((float)x+0.5f, (float)y+0.5f));
                        color = new Color(0.5f + 0.5f * dir.Length(), 0.5f + 0.5f * dir.Length(), 0.5f + 0.5f * dir.Length());//new Color(dir.Length(), 0.5f + 0.5f * dir.X, 0.5f + 0.5f * dir.Y);
                    }*/

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

        public void updateDisField()
        {
            while(updateQueue.Count > 0)
            {
                int2 pos = updateQueue.Dequeue();
                updateTiles.Remove(pos);

                float dis = 3f;

                bool isSolid = getTile(pos.x, pos.y).IsSolid;

                if (isSolid)
                    dis = 1f;

                for (int xx = -2; xx <= 2; ++xx)
                {
                    for (int yy = -2; yy <= 2; ++yy)
                    {
                        if (xx == 0 && yy == 0)
                            continue;

                        TileData tile = getTile(pos.x + xx, pos.y + yy);
                        if (!(tile.IsSolid^isSolid))
                            continue;

                        Vector2 b = new Vector2(0.5f, 0.5f);
                        Vector2 p = new Vector2((float)(xx), (float)(yy));
                        

                        Vector2 d = new Vector2(Math.Abs(p.X), Math.Abs(p.Y)) - b;
                        float dd = Math.Min(Math.Max(d.X, d.Y), 0f) +
                                 Vector2.Max(d, new Vector2(0f,0f)).Length();


                        dis = Math.Min(dis, dd);


                    }
                }

                if (isSolid)
                    disField[pos.x, pos.y] = -dis;
                else
                    disField[pos.x, pos.y] = dis;
            }
        }

        public TileManager getTileManager()
        {
            return tileManager;
        }

        public TileData getTile(int x, int y)
        {
            UInt16 tileID = getTileID(x, y);
            return tileManager.getTile(tileID);
        }

        public UInt16 getTileID(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return 0;

            return tiles.getData(x, y);
        }

        public float getDis(Vector2 pos)
        {
            int2 floorPos = new int2((int)pos.X, (int)pos.Y);
            int2 ceilPos = new int2((int)pos.X + 1, (int)pos.Y + 1);

            if (floorPos.x < 0 || floorPos.y < 0 || ceilPos.x >= width || ceilPos.y >= height)
                return 5f;

            Vector2 localPos = new Vector2(pos.X % 1f, pos.Y % 1f);
            float a = 1f-localPos.X;
            float b = 1f-localPos.Y;
            float c = 1f-a;
            float d = 1f-b;
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
            Vector2 f2 = f1*v2;

            Vector2 a = getDis(pos + new Vector2(-0.1f, -0.1f)) * Vector2.Normalize(new Vector2(-0.1f, -0.1f));
            Vector2 b = getDis(pos + new Vector2(+0.1f, -0.1f)) * Vector2.Normalize(new Vector2(+0.1f, -0.1f));
            Vector2 c = getDis(pos + new Vector2(-0.1f, +0.1f)) * Vector2.Normalize(new Vector2(-0.1f, +0.1f));
            Vector2 d = getDis(pos + new Vector2(+0.1f, +0.1f)) * Vector2.Normalize(new Vector2(+0.1f, +0.1f));

            Vector2 normal = new Vector2(a.X + b.X + c.X + d.X, a.Y + b.Y + c.Y + d.Y);
            if (normal.Length() > 0f)
                normal.Normalize();
             return normal;
        }

		public void setTile(int x, int y, UInt16 tile)
		{
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;


			tiles.setData(x, y, tile);

            for (int xx = -2; xx <= 2; ++xx)
            {
                for (int yy = -2; yy <= 2; ++yy)
                {
                    int2 pos = new int2(x+xx, y+yy);
                    if (pos.x < 0 || pos.y < 0 || pos.x >= width || pos.y >= height)
                        continue;

                    if (updateTiles.Contains(pos))
                        continue;

                    updateTiles.Add(pos);
                    updateQueue.Enqueue(pos);
                }
            }
		}

        public void setTileSolid(int x, int y, bool solid)
        {

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
