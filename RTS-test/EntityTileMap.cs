using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    class EntityTileMap
    {
        private int[,] entityTiles;
        private DisField disField;
        private int2 size;
        private EntityWorld entityWorld;


        public EntityTileMap(EntityWorld entityWorld, int2 size)
		{
            this.entityWorld = entityWorld;
            this.size = size;
		}

        public void load()
        {
            entityTiles = new int[size.x, size.y];
            disField = new DisField(size);

            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    entityTiles[x, y] = -1;
                }
            }
        }

        public void update()
        {
            disField.update();
        }

		public void draw(SpriteBatch spriteBatch)
		{
            Rectangle viewportWorldBoundry = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(viewportWorldBoundry.X / Global.tileSize, viewportWorldBoundry.Y / Global.tileSize, viewportWorldBoundry.Width / Global.tileSize, viewportWorldBoundry.Height / Global.tileSize);

            HashSet<int> entitySet = new HashSet<int>();
            List<int> entitiesToDraw = new List<int>();

            for (int x = tilesVisible.X; x <= tilesVisible.Right + 1; x++)
            {
                for (int y = tilesVisible.Y; y <= tilesVisible.Bottom + 1; y++)
                {
                    if (x < 0 || y < 0 || x >= Global.mapWidth || y >= Global.mapHeight)
                        continue;

                    int id = entityTiles[x, y];
                    if (id == -1)
                        continue;

                    if (entitySet.Contains(id))
                        continue;

                    entitiesToDraw.Add(id);
                    entitySet.Add(id);
                }
            }

            foreach (int entityID in entitiesToDraw)
            {
                Entity entity = entityWorld.GetEntity(entityID);
                if (entity == null)
                    continue;

                if (!entity.HasComponent<component.TileEntity>() || !entity.HasComponent<component.Drawable>())
                    continue;

                component.TileEntity tileEntity = entity.GetComponent<component.TileEntity>();
                component.Drawable drawable = entity.GetComponent<component.Drawable>();


                Vector2 origin = new Vector2(drawable.texture.Width / 2, drawable.texture.Height / 2);
                Rectangle rectangle = new Rectangle(
                    new Point((int)(tileEntity.Position.X * Global.tileSize + origin.X), (int)(tileEntity.Position.Y * Global.tileSize + origin.Y)),
                    new Point(drawable.texture.Width, drawable.texture.Height)
                    );


                Color color = Color.White;
                color.A = 255;
                spriteBatch.Draw(drawable.texture, null, rectangle, null, origin, tileEntity.Rotation, null, color, SpriteEffects.None, 0);
            }
		}

        public Entity getTileEntity(int x, int y)
        {
            int tileEntityID = getTileEntityID(x, y);
            return entityWorld.GetEntity(tileEntityID);
        }

        public int getTileEntityID(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return -1;

            return entityTiles[x, y];
        }

        public bool getIsSolid(int x, int y)
        {
            return (getTileEntityID(x, y) != -1);
        }

		public void setTile(int x, int y, Entity tileEntity)
		{
            if (x < 0 || y < 0 || x >= size.x || y >= size.y)
                return;

            int id = (tileEntity == null) ? -1 : tileEntity.Id;

            if (entityTiles[x, y] == id)
                return;

            // Notify disfield
            if ((entityTiles[x, y] == -1) ^ (id == -1))
                disField.setTile(x, y, id != -1);

            entityTiles[x, y] = id;
		}

        public DisField DisField
        {
            get { return disField; }
        }

        public EntityWorld EntityWorld
        {
            get { return entityWorld; }
        }

		public int2 Size
        {
            get { return size; }
        }
    }
}
