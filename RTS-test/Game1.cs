using Artemis;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RTS_test.component;
using RTS_test.system;
using System;

namespace RTS_test
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		private InputState _inputState;
		private TileMap tileMap;
        private TileManager tileManager;
		private TextureManager textureManager;
        private EntityWorld entityWorld;

		private int frameRate;
		private TimeSpan elapsedTime;
		private int frameCounter;
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
		private SpriteFont font; // Font for fps-counter

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			_inputState = new InputState();
			tileMap = new TileMap(Global.mapWidth, Global.mapHeight);
            tileManager = new TileManager();
			textureManager = new TextureManager();

			

			//this.IsFixedTimeStep = false; // Remove fps limit
			//graphics.SynchronizeWithVerticalRetrace = false;
			//graphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			graphics.PreferredBackBufferWidth = 1000;  // set this value to the desired width of your window
			graphics.PreferredBackBufferHeight = 600;	// set this value to the desired height of your window
			graphics.ApplyChanges();

			Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
			Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

			entityWorld = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry<SpriteBatch>("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry<TextureManager>("TextureManager", textureManager);
			this.entityWorld.InitializeAll(true);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			font = Content.Load<SpriteFont>("SpriteFont1");
			textureManager.loadTextures(Content);

            TileData tileGrass = new TileData("grass", textureManager.getTexture(1), false);
            TileData tileSand = new TileData("sand", textureManager.getTexture(3), false);
            TileData tileWater = new TileData("water", textureManager.getTexture(4), true);
            tileManager.registerTile(tileGrass);
            tileManager.registerTile(tileSand);
            tileManager.registerTile(tileWater);

            for (int i = 0; i < 100; ++i)
                entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(16.0f*i, 40f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });


		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (_inputState.IsExitGame(PlayerIndex.One))
				Exit();

			_inputState.Update();
			Global.Camera.HandleInput(_inputState, PlayerIndex.One);

            entityWorld.Update();
			// FPS-counter stuff
			++this.frameCounter;
			this.elapsedTime += gameTime.ElapsedGameTime;
			if (this.elapsedTime > OneSecond)
			{
				this.elapsedTime -= OneSecond;
				this.frameRate = this.frameCounter;
				this.frameCounter = 0;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
	null, null, null, null, Global.Camera.TranslationMatrix);

			Rectangle viewportWorldBoundry = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(viewportWorldBoundry.X / 32, viewportWorldBoundry.Y / 32, viewportWorldBoundry.Width / 32, viewportWorldBoundry.Height / 32);

			//Draw tilemap
			for (int x = tilesVisible.X; x <= tilesVisible.Right + 1; x++)
			{
				for (int y = tilesVisible.Y; y <= tilesVisible.Bottom + 1; y++)
				{
					if (x < 0 || y < 0 || x >= Global.mapWidth ||y >= Global.mapHeight)
						continue;
					UInt16 tile = tileMap.getTile(x, y);
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
			entityWorld.Draw();
			spriteBatch.End();


			spriteBatch.Begin();
			string fps = string.Format("fps: {0}", this.frameRate);
			spriteBatch.DrawString(font, fps, new Vector2(16, 16), Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
