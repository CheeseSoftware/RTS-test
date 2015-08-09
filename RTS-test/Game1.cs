using Artemis;
using Artemis.Manager;
using Artemis.System;
using Artemis.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RTS_test.component;
using RTS_test.GUI;
using RTS_test.system;
using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;
using System;
using System.IO;

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
        private DisField entityDisField;
        private DisFieldMixer disFieldMixer;
        private TileManager tileManager;
        private TextureManager textureManager;
        private EntityWorld entityWorld;
        private UnitController unitController;
        private FarseerPhysics.Dynamics.World world;
        private Generator generator;

        private int frameRate;
        private TimeSpan elapsedTime;
        private int frameCounter;
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private SpriteFont font; // Font for fps-counter

        private SharpCEGui.Base.System GUISystem;
        protected GUIContext GUIContext;
        private Window GUIWindow;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //this.IsFixedTimeStep = false; // Remove fps limit
            //graphics.SynchronizeWithVerticalRetrace = false;
            //graphics.ApplyChanges();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _inputState = new InputState();
            tileManager = new TileManager();
            tileMap = new TileMap(tileManager, new int2(Global.mapWidth, Global.mapHeight));
            entityDisField = new DisField(new int2(Global.mapWidth, Global.mapHeight));
            disFieldMixer = new DisFieldMixer();
            textureManager = new TextureManager();
            unitController = new UnitController();
            world = new FarseerPhysics.Dynamics.World(new Vector2(0f, 0f));
            generator = new Generator();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = 1000;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

            entityWorld = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry<SpriteBatch>("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry<TextureManager>("TextureManager", textureManager);
            EntitySystem.BlackBoard.SetEntry<TileMap>("TileMap", tileMap);
            EntitySystem.BlackBoard.SetEntry<DisField>("EntityDisField", entityDisField);
            EntitySystem.BlackBoard.SetEntry<DisFieldMixer>("DisFieldMixer", disFieldMixer);
            EntitySystem.BlackBoard.SetEntry<FarseerPhysics.Dynamics.World>("PhysicsWorld", world);
            EntitySystem.BlackBoard.SetEntry<Generator>("Generator", generator);
            this.entityWorld.InitializeAll(true);


            //GUI
            var renderer = MonoGameRenderer.Create(GraphicsDevice, Content);
            GUISystem = SharpCEGui.Base.System.Create(renderer, null, new DefaultXmlParser());

            InitialiseDefaultResourceGroups();
            InitialiseResourceGroupDirectories();

            GUIContext = GUISystem.GetDefaultGUIContext();

            //SchemeManager.GetSingleton().CreateFromFile("Generic.scheme");
            ////GUIContext.GetMouseCursor().SetDefaultImage("TaharezLook/MouseArrow");
            //GUIContext.GetMouseCursor().SetImage(GUIContext.GetMouseCursor().GetDefaultImage());

            //var winMgr = WindowManager.GetSingleton();
            //GUIWindow = winMgr.CreateWindow("DefaultWindow", "Root");

            //var defaultFont = FontManager.GetSingleton().CreateFromFile("1.font");
            //GUIContext.SetDefaultFont(defaultFont);
            //GUIContext.SetRootWindow(GUIWindow);

            //var label = winMgr.CreateWindow("Generic/Label", "Demo Window");
            //label.SetText("MOOOOOOO");
            //label.SetPosition(new UVector2(UDim.Absolute(100.0f), UDim.Absolute(100.0f)));
            //label.SetSize(new USize(UDim.Absolute(600.0f), UDim.Absolute(100.0f)));
            //GUIWindow.AddChild(label);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("SpriteFont1");
            textureManager.loadTextures(Content);

            TileData tileGrass = new TileData("grass", textureManager.getTexture(1), false);
            TileData tileSand = new TileData("sand", textureManager.getTexture(3), false);
            TileData tileWater = new TileData("water", textureManager.getTexture(4), true);
            TileData tileTree = new TileData("tree", textureManager.getTexture(5), true);
            tileManager.registerTile(tileGrass);
            tileManager.registerTile(tileSand);
            tileManager.registerTile(tileWater);
            tileManager.registerTile(tileTree);

            tileMap.load();
            generator.generate(tileMap, entityWorld);
            tileMap.update();
            entityDisField.update();

            disFieldMixer.addDisField(tileMap.DisField);
            disFieldMixer.addDisField(entityDisField);
            tileMap.setTreeDis(entityDisField);

            for (int i = 0; i < 50; ++i)
                entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(0.2f*i, 4f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });

            for (int i = 0; i < 50; ++i)
                entityWorld.CreateEntityFromTemplate("Test2", new object[] {
                    new Vector2(0.2f*i, 4f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });


            Entity e = entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(10, 10),
                });
            Global.Camera.followEntity(e, 0.1f);

            int2 a = new int2(0);
            int2 b = new int2(3);
            int2 c = a * b;


            //Generate resources and natural object entities
            /*Random r = new Random();
			for (int i = 0; i < tileMap.Size.x; i++)
			{
				entityWorld.CreateEntityFromTemplate("Tree", new object[] {
					new Vector2(r.Next(tileMap.Size.x * 32), r.Next(tileMap.Size.y() * 32)),
					});

				entityWorld.CreateEntityFromTemplate("Stone", new object[] {
					new Vector2(r.Next(tileMap.Size.x * 32), r.Next(tileMap.Size.y() * 32)),
					});

				entityWorld.CreateEntityFromTemplate("BerryBush", new object[] {
					new Vector2(r.Next(tileMap.Size.x * 32), r.Next(tileMap.Size.y() * 32)),
					});
			}*/


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
                if (Global.Camera.Viewport.Contains(mouseState.Position))
                {
                    //new System.Threading.Thread(() =>
                    //{
                        Vector2 pos = Global.Camera.ScreenToWorld(mouseState.Position.ToVector2());
                        Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal), typeof(component.Physics)));
                        PathGoal pathGoal = new PathGoal(entities, tileMap, new int2(Global.mapWidth, Global.mapHeight), new int2((int)pos.X / Global.tileSize, (int)pos.Y / Global.tileSize));
                        unitController.setPathGoal(pathGoal);
                        pathGoal.updatePath();
                        tileMap.setPathGoal(pathGoal);
                    //}).Start();
                }
            }


            unitController.updateGoal(entityWorld, Global.Camera);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
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

            //GUI
            var ms = Mouse.GetState();
            GUIContext.InjectMousePosition(ms.X, ms.Y);
            if (ms.LeftButton == ButtonState.Pressed)
                GUIContext.InjectMouseButtonDown(MouseButton.LeftButton);
            else if (ms.LeftButton == ButtonState.Released)
                GUIContext.InjectMouseButtonUp(MouseButton.LeftButton);
            GUIContext.InjectTimePulse((float)gameTime.ElapsedGameTime.TotalSeconds);
            GUISystem.InjectTimePulse((float)gameTime.ElapsedGameTime.TotalSeconds);

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

            // GUI
            GUISystem.RenderAllGUIContexts();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitialiseResourceGroupDirectories()
        {
            var resourceProvider = (DefaultResourceProvider)SharpCEGui.Base.System.GetSingleton().GetResourceProvider();
            var dataPathPrefix = Path.Combine(Environment.CurrentDirectory, "resources");

            // for each resource type, set a resource group directory
            resourceProvider.SetResourceGroupDirectory("schemes", Path.Combine(dataPathPrefix, "schemes"));
            resourceProvider.SetResourceGroupDirectory("imagesets", Path.Combine(dataPathPrefix, "imagesets"));
            resourceProvider.SetResourceGroupDirectory("fonts", Path.Combine(dataPathPrefix, "fonts"));
            resourceProvider.SetResourceGroupDirectory("layouts", Path.Combine(dataPathPrefix, "layouts"));
            resourceProvider.SetResourceGroupDirectory("looknfeels", Path.Combine(dataPathPrefix, "looknfeel"));
            resourceProvider.SetResourceGroupDirectory("lua_scripts", Path.Combine(dataPathPrefix, "lua_scripts"));
            resourceProvider.SetResourceGroupDirectory("schemas", Path.Combine(dataPathPrefix, "xml_schemas"));
            resourceProvider.SetResourceGroupDirectory("animations", Path.Combine(dataPathPrefix, "animations"));
        }

        private void InitialiseDefaultResourceGroups()
        {
            //// set the default resource groups to be used
            SharpCEGui.Base.ImageManager.SetImagesetDefaultResourceGroup("imagesets");
            SharpCEGui.Base.Font.SetDefaultResourceGroup("fonts");
            SharpCEGui.Base.Scheme.SetDefaultResourceGroup("schemes");
            SharpCEGui.Base.WidgetLookManager.SetDefaultResourceGroup("looknfeels");
            SharpCEGui.Base.WindowManager.SetDefaultResourceGroup("layouts");
            //CEGUI::ScriptModule::setDefaultResourceGroup("lua_scripts");
            AnimationManager.SetDefaultResourceGroup("animations");

            //// setup default group for validation schemas
            //CEGUI::XMLParser* parser = CEGUI::System::getSingleton().getXMLParser();
            //if (parser->isPropertyPresent("SchemaDefaultResourceGroup"))
            //    parser->setProperty("SchemaDefaultResourceGroup", "schemas");
        }
    }
}
