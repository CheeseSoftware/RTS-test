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
using System.Collections.Generic;
using System.IO;
using RectangleF = System.Drawing.RectangleF;

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
        private EntityTileMap entityTileMap;
        private DisFieldMixer disFieldMixer;
        private TileManager tileManager;
        private TextureManager textureManager;
        private AnimationManager animationManager;
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

        private bool isSelecting = false;
        private Vector2 firstCorner;
        private List<Entity> entitiesInSelection = new List<Entity>();
        private List<Entity> tempEntitiesInSelection = new List<Entity>();


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
            entityWorld = new EntityWorld();
            entityTileMap = new EntityTileMap(entityWorld, new int2(Global.mapWidth, Global.mapHeight));
            disFieldMixer = new DisFieldMixer();
            textureManager = new TextureManager();
            animationManager = new AnimationManager();
            unitController = new UnitController();
            world = new FarseerPhysics.Dynamics.World(new Vector2(0f, 0f));
            generator = new Generator(tileMap);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferWidth = 1000;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

            
            EntitySystem.BlackBoard.SetEntry<SpriteBatch>("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry<TextureManager>("TextureManager", textureManager);
            EntitySystem.BlackBoard.SetEntry<TileMap>("TileMap", tileMap);
            EntitySystem.BlackBoard.SetEntry<EntityTileMap>("EntityTileMap", entityTileMap);
            EntitySystem.BlackBoard.SetEntry<DisFieldMixer>("DisFieldMixer", disFieldMixer);
            EntitySystem.BlackBoard.SetEntry<FarseerPhysics.Dynamics.World>("PhysicsWorld", world);
            EntitySystem.BlackBoard.SetEntry<Generator>("Generator", generator);
            EntitySystem.BlackBoard.SetEntry<AnimationManager>("AnimationManager", animationManager);

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
            animationManager.load();

            TileData tileGrass = new TileData("grass", textureManager.getTexture(1), false);
            TileData tileSand = new TileData("sand", textureManager.getTexture(3), false);
            TileData tileWater = new TileData("water", textureManager.getTexture(4), true);
            TileData tileTree = new TileData("tree", textureManager.getTexture(5), true);
            TileData tileShallowWater = new TileData("shallowwater", textureManager.getTexture(15), false);

            tileManager.registerTile(tileGrass);
            tileManager.registerTile(tileSand);
            tileManager.registerTile(tileWater);
            tileManager.registerTile(tileTree);
            tileManager.registerTile(tileShallowWater); //5

            tileMap.load();
            entityTileMap.load();
            generator.generate(entityWorld);
            tileMap.update();
            entityTileMap.update();

            disFieldMixer.addDisField(tileMap.DisField);
            disFieldMixer.addDisField(entityTileMap.DisField);
            tileMap.setTreeDis(entityTileMap.DisField);

            for (int i = 0; i < 50; ++i)
                entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(17 + 0.2f*i, 17 + 4f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });

            /*for (int i = 0; i < 50; ++i)
                entityWorld.CreateEntityFromTemplate("Test2", new object[] {
                    new Vector2(0.2f*i, 4f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });*/


            /*Entity e = entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(10, 10),
                });
            Global.Camera.followEntity(e, 0.1f);*/

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
            entityTileMap.update();
            Global.Camera.HandleInput(_inputState, PlayerIndex.One);

            MouseState mouseState;
            if (entitiesInSelection.Count > 0)
            {
                if (_inputState.IsNewRightMouseClick(out mouseState))
                {
                    if (Global.Camera.Viewport.Contains(mouseState.Position))
                    {
                        Vector2 pos = Global.Camera.ScreenToWorld(mouseState.Position.ToVector2());
                        //Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal), typeof(component.Physics), typeof(component.Formation)));
                        int2 goalPos = new int2((int)pos.X / Global.tileSize, (int)pos.Y / Global.tileSize);
                        //Console.WriteLine(goalPos.x + " - " + goalPos.y);
                        Bag<Entity> bag = new Bag<Entity>();
                        PathGoal pathGoal = new PathGoal(entitiesInSelection, disFieldMixer, new int2(Global.mapWidth, Global.mapHeight), goalPos);
                        unitController.setPathGoal(pathGoal);
                        pathGoal.updatePath();
                        tileMap.setPathGoal(pathGoal);
                        

                        EntityFormation formation = new EntityFormation(entitiesInSelection, disFieldMixer, goalPos);
                        foreach (Entity e in entitiesInSelection)
                        {
                            e.GetComponent<component.Formation>().EntityFormation = formation;
                        }
                        formation.update();
                    }
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !isSelecting)
            {
                isSelecting = true;
                firstCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());

                foreach (Entity old in entitiesInSelection)
                {
                    old.GetComponent<component.HealthComponent>().Visible = false;
                }
                entitiesInSelection.Clear();
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released && isSelecting)
            {
                isSelecting = false;
                Vector2 secondCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                Vector2 topLeft = new Vector2(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y));
                Vector2 bottomRight = new Vector2(Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
                RectangleF rect = new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

                if (rect.Width > 0 && rect.Height > 0)
                {
                    Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Physics), typeof(component.Formation), typeof(component.HealthComponent)));
                    
                    foreach (Entity e in entities)
                    {
                        component.Physics phys = e.GetComponent<component.Physics>();
                        Vector2 newPos = new Vector2(phys.Position.X * Global.tileSize, phys.Position.Y * Global.tileSize);
                        RectangleF entityRect = new RectangleF(newPos.X - 0.5f * 32f, newPos.Y - 0.5f * 32f, 1f * 32f, 1f * 32f);
                        if (rect.IntersectsWith(entityRect))
                        {
                            e.GetComponent<component.HealthComponent>().Visible = true;
                            entitiesInSelection.Add(e);
                        }
                    }
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

            if (isSelecting)
            {
                Vector2 secondCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                Vector2 topLeft = new Vector2(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y));
                Vector2 bottomRight = new Vector2(Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
                RectangleF rect = new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

                foreach(Entity e in tempEntitiesInSelection)
                {
                    e.GetComponent<component.HealthComponent>().Visible = false;
                }
                tempEntitiesInSelection.Clear();

                if (rect.Width > 0 && rect.Height > 0)
                {
                    Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Physics), typeof(component.Formation), typeof(component.HealthComponent)));
                    foreach (Entity e in entities)
                    {
                        component.Physics phys = e.GetComponent<component.Physics>();
                        Vector2 newPos = new Vector2(phys.Position.X * Global.tileSize, phys.Position.Y * Global.tileSize);
                        RectangleF entityRect = new RectangleF(newPos.X - 0.5f * 32f, newPos.Y - 0.5f * 32f, 1f * 32f, 1f * 32f);
                        if (rect.IntersectsWith(entityRect))
                        {
                            e.GetComponent<component.HealthComponent>().Visible = true;
                            tempEntitiesInSelection.Add(e);
                        }
                    }
                }
            }

            Global.Camera.update();

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
    SamplerState.AnisotropicClamp, null, null, null, Global.Camera.TranslationMatrix);
            tileMap.draw(spriteBatch, tileManager);
            entityWorld.Draw();
            entityTileMap.draw(spriteBatch);

            if (isSelecting)
            {
                Vector2 secondCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                Vector2 topLeft = new Vector2(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y));
                Vector2 bottomRight = new Vector2(Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
                Rectangle rect = new Rectangle(topLeft.ToPoint(), (bottomRight - topLeft).ToPoint());

                DrawLine(spriteBatch,
                    new Vector2(topLeft.X, topLeft.Y), //start of line
                    new Vector2(bottomRight.X, topLeft.Y) //end of line
                );
                DrawLine(spriteBatch,
                    new Vector2(bottomRight.X, topLeft.Y), //start of line
                    new Vector2(bottomRight.X, bottomRight.Y) //end of line
                );
                DrawLine(spriteBatch,
                    new Vector2(topLeft.X, bottomRight.Y), //start of line
                    new Vector2(topLeft.X, topLeft.Y) //end of line
                );
                DrawLine(spriteBatch,
                    new Vector2(bottomRight.X, bottomRight.Y), //start of line
                    new Vector2(topLeft.X, bottomRight.Y) //end of line
                );

            }

            spriteBatch.End();


            spriteBatch.Begin();
            string fps = string.Format("fps: {0}", this.frameRate);
            spriteBatch.DrawString(font, fps, new Vector2(16, 16), Color.White);
            spriteBatch.DrawString(font, "Camera zoom: " + Global.Camera.Zoom, new Vector2(16, 32), Color.White);

            // GUI
            GUISystem.RenderAllGUIContexts();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);

            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white

            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.Black, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

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
            //AnimationManager.SetDefaultResourceGroup("animations");

            //// setup default group for validation schemas
            //CEGUI::XMLParser* parser = CEGUI::System::getSingleton().getXMLParser();
            //if (parser->isPropertyPresent("SchemaDefaultResourceGroup"))
            //    parser->setProperty("SchemaDefaultResourceGroup", "schemas");
        }
    }
}
