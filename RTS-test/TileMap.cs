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
        private DisField disField;
        private TileManager tileManager;
        private int2 size;
        private PathGoal pathGoal = null;
        private DisField treeDis = null;

        /***********************
         * Debug code!
         * // TODO: Remove debug code.
         * *********************/
        public void setPathGoal(PathGoal pathGoal)
        {
            this.pathGoal = pathGoal;
        }
        public void setTreeDis(DisField treeDis)
        {
            this.treeDis = treeDis;
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

            Rectangle sourceRectangle = new Rectangle(0, 0, Global.tileSize, Global.tileSize);
            Vector2 startPos = new Vector2();
            bool drawMultiTiles = false;
            UInt16 currentTile = UInt16.MaxValue;
            Random r = new Random();
            //bool timeToDraw = false;

            for (int x = tilesVisible.X - 1; x <= tilesVisible.Right + 1; x++)
            {
                //drawMultiTiles = false;
                for (int y = tilesVisible.Y - 1; y <= tilesVisible.Bottom + 1; y++)
                {
                    if (x < 0 || y < 0 || x >= Global.mapWidth || y >= Global.mapHeight)
                        continue;

                    Color color = Color.White;
                    //color = new Color(r.Next(256), r.Next(256), r.Next(256));

                    UInt16 tile = getTileID(x, y);
                    TileData tileData = tileManager.getTile(tile);
                    Texture2D texture = tileData.Texture;

                    if (texture.Width > Global.tileSize || texture.Height > Global.tileSize)
                    {
                        // Draw parts of a larger texture to look nice
                        int baseX = x * Global.tileSize % texture.Width;
                        int baseY = y * Global.tileSize % texture.Height;

                        int chunkY = y * Global.tileSize / texture.Height;
                        int newChunkY = (y + 1) * Global.tileSize / texture.Height;

                        if (y + 1 < tilesVisible.Bottom + 1 &&
                            chunkY == newChunkY &&
                            sourceRectangle.Bottom + Global.tileSize <= texture.Height && 
                            sourceRectangle.Height + Global.tileSize <= texture.Height && 
                            (getTileID(x, y + 1) == currentTile || currentTile == UInt16.MaxValue))
                        {
                            // tile under denna är samma

                            if (!drawMultiTiles)
                            {
                                sourceRectangle.Location = new Point(baseX, baseY);
                                startPos = new Vector2(x * Global.tileSize, y * Global.tileSize);
                                currentTile = tile;
                                drawMultiTiles = true;
                            }
                            sourceRectangle.Height += Global.tileSize;
                        }
                        else if (drawMultiTiles)
                        {
                            spriteBatch.Draw(texture, startPos, null, sourceRectangle, null, 0f, null, color, SpriteEffects.None, 0f);

                            drawMultiTiles = false;
                            sourceRectangle = new Rectangle(0, 0, Global.tileSize, Global.tileSize);
                            currentTile = UInt16.MaxValue;
                        }
                        else
                        {
                            sourceRectangle.X = baseX;
                            sourceRectangle.Y = baseY;

                            spriteBatch.Draw(texture, new Vector2(x * Global.tileSize, y * Global.tileSize), null, sourceRectangle, null, 0f, null, color, SpriteEffects.None, 0f);
                        }
                    }
                    else // Draw normally
                        spriteBatch.Draw(texture, new Vector2(x * Global.tileSize, y * Global.tileSize), color);
                }
            }
            return;

            VertexPositionColor[] vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(1, 0, 0), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(1, 1, 0), Color.Red);
            vertices[3] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            vertices[4] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            vertices[5] = new VertexPositionColor(new Vector3(1, 1, 0), Color.Red);

            BasicEffect basicEffect = new BasicEffect(spriteBatch.GraphicsDevice);
            //basicEffect.World = Global.Camera.TranslationMatrix;

            if (vertices.Length > 0)
            {
                var vertexBuffer = new VertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionColor>(vertices);
                //basicEffect.View = camera.ViewMatrix;
                //basicEffect.Projection = camera.ProjectionMatrix;
                //graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                // {
                //   pass.Apply();
                spriteBatch.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                spriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
                //}
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
