using Artemis;
using Artemis.Manager;
using Artemis.System;
using Artemis.Utils;
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
        private UnitController unitController;

		private int frameRate;
		private TimeSpan elapsedTime;
		private int frameCounter;
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
		private SpriteFont font; // Font for fps-counter

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			//this.IsFixedTimeStep = false; // Remove fps limit
			//graphics.SynchronizeWithVerticalRetrace = false;
			//graphics.ApplyChanges();
		}

		protected override void Initialize()
		{
			_inputState = new InputState();
			tileMap = new TileMap(Global.mapWidth, Global.mapHeight);
			tileManager = new TileManager();
			textureManager = new TextureManager();
            unitController = new UnitController();

			spriteBatch = new SpriteBatch(GraphicsDevice);
			this.IsMouseVisible = true;

			graphics.PreferredBackBufferWidth = 1000;  // set this value to the desired width of your window
			graphics.PreferredBackBufferHeight = 600;	// set this value to the desired height of your window
			graphics.ApplyChanges();

			Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
			Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

			entityWorld = new EntityWorld();
			EntitySystem.BlackBoard.SetEntry<SpriteBatch>("SpriteBatch", spriteBatch);
			EntitySystem.BlackBoard.SetEntry<TextureManager>("TextureManager", textureManager);
			EntitySystem.BlackBoard.SetEntry<TileMap>("TileMap", tileMap);
			this.entityWorld.InitializeAll(true);

			base.Initialize();
		}

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


			//Generate resources and natural object entities
			Random r = new Random();
			for (int i = 0; i < tileMap.getWidth(); i++)
			{
				entityWorld.CreateEntityFromTemplate("Tree", new object[] {
					new Vector2(r.Next(tileMap.getWidth() * 32), r.Next(tileMap.getHeight() * 32)),
					});

				entityWorld.CreateEntityFromTemplate("Stone", new object[] {
					new Vector2(r.Next(tileMap.getWidth() * 32), r.Next(tileMap.getHeight() * 32)),
					});

				entityWorld.CreateEntityFromTemplate("BerryBush", new object[] {
					new Vector2(r.Next(tileMap.getWidth() * 32), r.Next(tileMap.getHeight() * 32)),
					});
			}

            
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (_inputState.IsExitGame(PlayerIndex.One))
				Exit();

			_inputState.Update();
			Global.Camera.HandleInput(_inputState, PlayerIndex.One);

			MouseState mouseState;
			if (_inputState.IsNewLeftMouseClick(out mouseState))
			{
				Vector2 pos = Global.Camera.ScreenToWorld(mouseState.Position.ToVector2());
				unitController.setPathGoal(new PathGoal(new int2(Global.mapWidth, Global.mapHeight), new int2((int)pos.X/32, (int)pos.Y/32)));
			}

			unitController.update(entityWorld, Global.Camera);
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

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
	null, null, null, null, Global.Camera.TranslationMatrix);
			tileMap.draw(spriteBatch, tileManager);
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
