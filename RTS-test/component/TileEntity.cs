using Artemis;
using Artemis.Interface;
using Artemis.System;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class TileEntity : IComponent
    {
        private Rectangle position;
        private Rectangle collision;
        private float rotation = 0f;

        public TileEntity(Entity entity, Rectangle position, Rectangle collision, float rotation = 0f)
        {
            this.position = position;
            this.collision = collision;
            this.rotation = rotation;

            EntityTileMap entityTileMap = EntitySystem.BlackBoard.GetEntry<EntityTileMap>("EntityTileMap");

            IEnumerator<int2> i = this.getCollisonTiles();
            while (i.MoveNext())
                entityTileMap.setTile(i.Current.x, i.Current.y, entity);
        }

        ~TileEntity()
        {
            remove();
        }

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public Rectangle Collision
        {
            get { return collision; }
            set { collision = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public void remove()
        {
            EntityTileMap entityTileMap = EntitySystem.BlackBoard.GetEntry<EntityTileMap>("EntityTileMap");
            for (int i = collision.Left; i < collision.Right; i++)
            {
                for (int j = collision.Top; j < collision.Bottom; j++)
                {
                    entityTileMap.setTile(i, j, null);
                }
            }
        }

        public bool isMultiTile()
        {
            return position.Width > 1 || position.Height > 1;
        }

        public IEnumerator<int2> getTiles()
        {
            for (int i = position.Left; i < position.Right; i++)
            {
                for (int j = position.Top; j < position.Bottom; j++)
                {
                    yield return new int2(i, j);
                }
            }
        }

        public IEnumerator<int2> getCollisonTiles()
        {
            for (int i = collision.Left; i < collision.Right; i++)
            {
                for (int j = collision.Top; j < collision.Bottom; j++)
                {
                    yield return new int2(i, j);
                }
            }
        }
    }
}
