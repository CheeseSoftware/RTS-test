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

        private FrameRate frameMeter = new FrameRate();
        private SpriteFont font; // Font for fps-counter

        private bool isSelecting = false;
        private Vector2 firstCorner;
        private List<Entity> entitiesInSelection = new List<Entity>();
        private List<Entity> tempEntitiesInSelection = new List<Entity>();

        //private static Button button;
        private GUIWindow GUIWindow;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //this.IsFixedTimeStep = false; // Remove fps limit
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(10);
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

            graphics.PreferredBackBufferWidth = Global.ViewportWidth;
            graphics.PreferredBackBufferHeight = Global.ViewportHeight;
            graphics.ApplyChanges();

            EntitySystem.BlackBoard.SetEntry<SpriteBatch>("SpriteBatch", spriteBatch);
            EntitySystem.BlackBoard.SetEntry<TextureManager>("TextureManager", textureManager);
            EntitySystem.BlackBoard.SetEntry<TileMap>("TileMap", tileMap);
            EntitySystem.BlackBoard.SetEntry<EntityTileMap>("EntityTileMap", entityTileMap);
            EntitySystem.BlackBoard.SetEntry<DisFieldMixer>("DisFieldMixer", disFieldMixer);
            EntitySystem.BlackBoard.SetEntry<FarseerPhysics.Dynamics.World>("PhysicsWorld", world);
            EntitySystem.BlackBoard.SetEntry<Generator>("Generator", generator);
            EntitySystem.BlackBoard.SetEntry<AnimationManager>("AnimationManager", animationManager);

            this.entityWorld.InitializeAll(true);

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

            GUIWindow = new GUIWindow();
            GUILayout layout = new GUILayout();
            //layout.addComponent("testComponent", new GUI.Components.TestComponent(textureManager.getTexture(1)));
            //GUI.Components.Label label = new GUI.Components.Label(new Point(), Color.Red, font);
            //label.Text = "banan";
            layout.addComponent("testLabel1", new GUI.Components.Button(new Rectangle(0, 0, 200, 50), "Button1", font, Color.White, Color.DarkGreen, 2, Color.Red));
            layout.addComponent("testLabel2", new GUI.Components.Button(new Rectangle(0, 100, 200, 50), "Button2", font, Color.White, Color.DarkGreen, 2, Color.Blue));
            layout.addComponent("testLabel3", new GUI.Components.Button(new Rectangle(0, 200, 200, 50), "Button3", font, Color.White, Color.DarkGreen, 2, Color.Red));
            layout.addComponent("testLabel4", new GUI.Components.Button(new Rectangle(0, 300, 200, 50), "Button2", font, Color.White, Color.DarkGreen, 2, Color.Red));
            layout.addComponent("testLabel5", new GUI.Components.Button(new Rectangle(0, 400, 200, 50), "Button3", font, Color.White, Color.DarkGreen, 2, Color.Red));

            GUIWindow.addLayout("testLayout", layout);

            for (int i = 0; i < 50; ++i)
                entityWorld.CreateEntityFromTemplate("Test", new object[] {
                    new Vector2(17 + 0.2f*i, 17 + 4f*(float)Math.Sin(0.5f*i)),
                    new Vector2(0.001f*i, 0.05f*(float)Math.Cos(0.5f*i))
                });
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            frameMeter.update(gameTime);

            Global.inputManager.update();

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
                    if (Global.Viewport.Contains(mouseState.Position))
                    {
                        Vector2 pos = Global.Camera.ScreenToWorld(mouseState.Position.ToVector2());
                        //Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal), typeof(component.Physics), typeof(component.Formation)));
                        int2 goalPos = new int2((int)pos.X / Global.tileSize, (int)pos.Y / Global.tileSize);
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

                Console.WriteLine(rect.X / 32 + " - " + rect.Y / 32);

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

            if (isSelecting)
            {
                Vector2 secondCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                Vector2 topLeft = new Vector2(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y));
                Vector2 bottomRight = new Vector2(Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
                RectangleF rect = new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

                foreach (Entity e in tempEntitiesInSelection)
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

            GUIWindow.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            frameMeter.draw(gameTime);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
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

            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[] { Color.Black });
            Color color = Color.Black;
            color.A = 128;
            spriteBatch.Draw(t, new Rectangle(0, 0, 400, 65), null, color, 0f, new Vector2(0, 0), SpriteEffects.None, 0);

            spriteBatch.DrawString(font, "FPS: " + frameMeter.FPS, new Vector2(16, 8), Color.White);
            spriteBatch.DrawString(font, "UPS: " + frameMeter.UPS, new Vector2(16, 24), Color.White);

            spriteBatch.DrawString(font, "frametime: " + frameMeter.FrameTime + "ms", new Vector2(164, 8), Color.White);
            spriteBatch.DrawString(font, "updatetime: " + frameMeter.UpdateTime + "ms", new Vector2(164, 24), Color.White);

            GUIWindow.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[] { Color.White });
            sb.Draw(t, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1), null, Color.Black, angle, new Vector2(0, 0), SpriteEffects.None, 0);

        }
    }
}
