using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
		private TextureManager textureManager;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			_inputState = new InputState();
			tileMap = new TileMap(2000, 2000);
			textureManager = new TextureManager();

			this.IsFixedTimeStep = false; // Remove fps limit
			graphics.SynchronizeWithVerticalRetrace = false;
			graphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
			Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			textureManager.loadTextures(Content);
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

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,
	null, null, null, null, Global.Camera.TranslationMatrix);

			Rectangle blabla = Global.Camera.ViewportWorldBoundry();
			Rectangle tilesVisible = new Rectangle(blabla.X / 32, blabla.Y / 32, blabla.Width / 32, blabla.Height / 32);

			//Draw tilemap
			for (int x = tilesVisible.X; x < tilesVisible.Right; x++)
			{
				for (int y = tilesVisible.Y; y < tilesVisible.Bottom; y++)
				{
					if (x < 0 || y < 0)
						continue;
					int tile = tileMap.getTile(x, y);
					Texture2D texture = textureManager.getTexture(tile);

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

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
