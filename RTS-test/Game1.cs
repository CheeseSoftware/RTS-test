﻿using Artemis;
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
        private EntityWorld entityWorld;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			_inputState = new InputState();
			tileMap = new TileMap(1280, 1280);
			textureManager = new TextureManager();
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
			for (int x = 0; x < tileMap.getWidth(); x++)
			{
				for (int y = 0; y < tileMap.getHeight(); y++)
				{
					if (x >= tilesVisible.X && y >= tilesVisible.Y && x <= tilesVisible.Right && y <= tilesVisible.Bottom)
					{
						int tile = tileMap.getTile(x, y);
						Texture2D texture = textureManager.getTexture(tile);

						if (texture.Width > Global.tileSize || texture.Height > Global.tileSize)
						{
							int baseX = x * 32 % texture.Width;
							//Console.WriteLine("x " + baseX);
							int baseY = y * 32 % texture.Height;

							Rectangle sourceRectangle = new Rectangle(baseX, baseY, Global.tileSize, Global.tileSize);

							spriteBatch.Draw(texture, new Vector2(x * 32, y * 32), null, sourceRectangle);
						}
						else
							spriteBatch.Draw(texture, new Vector2(x * 32, y * 32));
					}
				}
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
