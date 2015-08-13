using Artemis;
using Artemis.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RectangleF = System.Drawing.RectangleF;

namespace RTS_test
{
    public class UnitController
    {
        public delegate void OnSelectUnitDelegate();
        public delegate void OnSelectBuildingDelegate();

        private PathGoal pathGoal;
        private InputState inputState;
        private InputManager inputManager;
        private EntityWorld entityWorld;
        private DisFieldMixer disFieldMixer;
        public event OnSelectUnitDelegate onSelectUnit;
        public event OnSelectBuildingDelegate onSelectBuilding;

        private bool isSelecting = false;
        private Vector2 firstCorner;
        private List<Entity> entitiesInSelection = new List<Entity>();
        private List<Entity> tempEntitiesInSelection = new List<Entity>();

        public UnitController(InputManager inputManager, InputState inputState, EntityWorld entityWorld, DisFieldMixer disFieldMixer)
        {
            this.inputManager = inputManager;
            this.inputState = inputState;
            this.entityWorld = entityWorld;
            this.disFieldMixer = disFieldMixer;
            pathGoal = null;
        }

        public void update()
        {
            MouseState mouseState;
            if (entitiesInSelection.Count > 0)
            {
                if (inputState.IsNewRightMouseClick(out mouseState))
                {
                    if (Global.Viewport.Contains(mouseState.Position))
                    {
                        Vector2 pos = Global.Camera.ScreenToWorld(mouseState.Position.ToVector2());
                        //Bag<Entity> entities = entityWorld.EntityManager.GetEntities(Aspect.All(typeof(component.Goal), typeof(component.Physics), typeof(component.Formation)));
                        int2 goalPos = new int2((int)pos.X / Global.tileSize, (int)pos.Y / Global.tileSize);
                        Bag<Entity> bag = new Bag<Entity>();
                        PathGoal pathGoal = new PathGoal(entitiesInSelection, disFieldMixer, new int2(Global.mapWidth, Global.mapHeight), goalPos);
                        setPathGoal(pathGoal);
                        pathGoal.updatePath();


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

                    if (onSelectUnit != null)
                        onSelectUnit();
                }
            }

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
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (isSelecting)
            {
                Vector2 secondCorner = Global.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
                Vector2 topLeft = new Vector2(Math.Min(firstCorner.X, secondCorner.X), Math.Min(firstCorner.Y, secondCorner.Y));
                Vector2 bottomRight = new Vector2(Math.Max(firstCorner.X, secondCorner.X), Math.Max(firstCorner.Y, secondCorner.Y));
                Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(topLeft.ToPoint(), (bottomRight - topLeft).ToPoint());

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
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            Texture2D t = new Texture2D(sb.GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[] { Color.White });
            sb.Draw(t, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1), null, Color.Black, angle, new Vector2(0, 0), SpriteEffects.None, 0);

        }

		public void updateGoal(EntityWorld entityWorld, Camera camera)
        {
            if (pathGoal == null)
                return;

            pathGoal.updatePath(); 
        }

		public void setPathGoal(PathGoal pathGoal)
		{
			this.pathGoal = pathGoal;
            pathGoal.updatePath();

            List<Entity> entities = pathGoal.getEntities();
            foreach (Entity entity in entities)
            {
                entity.GetComponent<component.Unit>().pathGoal = pathGoal;
            }
		}

    }
}
