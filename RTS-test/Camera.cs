using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test
{
    public class Camera
    {
        private Entity entityFollowing = null;
        private float cameraSpeed = 0.01f;
        private Vector2 position;
        private float toZoom = float.NaN;
        private float zoomSpeed;

        public Camera()
        {
            Zoom = 1.0f;
        }

        public Vector2 Position
        {
            get { return this.position; }
            private set
            {
                position = value;
                int borderWidth = Global.tileSize;
                if (position.X < -borderWidth + ViewportCenter.X)
                    position.X = -borderWidth + ViewportCenter.X;
                if (position.X > borderWidth - ViewportCenter.X + Global.mapWidth * Global.tileSize)
                    position.X = borderWidth - ViewportCenter.X + Global.mapWidth * Global.tileSize;

                if (position.Y < -borderWidth + ViewportCenter.Y)
                    position.Y = -borderWidth + ViewportCenter.Y;
                if (position.Y > borderWidth - ViewportCenter.Y + Global.mapHeight * Global.tileSize)
                    position.Y = borderWidth - ViewportCenter.Y + Global.mapHeight * Global.tileSize;
            }
        }
        public float Zoom { get; private set; }

        public float Rotation { get; private set; }

        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(Global.ViewportWidth * 0.5f, Global.ViewportHeight * 0.5f);
            }
        }

        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)Position.X,
                   -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }

        public void AdjustZoom(float amount, float speed = 0.15f)
        {
            ZoomTo(Zoom + amount, speed);
        }

        public void ZoomTo(float zoom, float speed = 0.15f)
        {
            toZoom = zoom;
            zoomSpeed = speed;
        }

        public void update()
        {
            if (!float.IsNaN(toZoom) && toZoom != Zoom)
            {
                Zoom = MathHelper.Lerp(Zoom, toZoom, zoomSpeed);
                if (Zoom < 0.19f)
                {
                    Zoom = 0.19f;
                    toZoom = float.NaN;
                }

                if (Zoom == toZoom)
                    toZoom = float.NaN;
            }
        }

        public void MoveCamera(Vector2 cameraMovement)
        {
            Vector2 newPosition = Position + cameraMovement;
            Position = newPosition;
        }

        public Rectangle ViewportWorldBoundry()
        {
            Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
            Vector2 viewPortBottomCorner =
               ScreenToWorld(new Vector2(Global.ViewportWidth, Global.ViewportHeight));

            return new Rectangle((int)viewPortCorner.X,
               (int)viewPortCorner.Y,
               (int)(viewPortBottomCorner.X - viewPortCorner.X),
               (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
        }

        // Center the camera on specific pixel coordinates
        public void CenterOn(Vector2 position)
        {
            Position = position;
        }

        public void followEntity(Entity entity, float cameraSpeed = 0.01f)
        {
            this.entityFollowing = entity;
            this.cameraSpeed = cameraSpeed;
        }

        public void stopFollwingEntity()
        {
            this.entityFollowing = null;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer)
        {
            if (entityFollowing != null)
            {
                component.Physics physics = entityFollowing.GetComponent<component.Physics>();
                if (physics != null)
                {
                    Vector2 newPos = new Vector2();
                    newPos.X = MathHelper.Lerp(Position.X, physics.Position.X * 32, cameraSpeed);
                    newPos.Y = MathHelper.Lerp(Position.Y, physics.Position.Y * 32, cameraSpeed);
                    Position = newPos;
                }
            }
            else
            {
                Vector2 cameraMovement = Vector2.Zero;

                if (inputState.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Left))
                {
                    cameraMovement.X = -1;
                }
                else if (inputState.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Right))
                {
                    cameraMovement.X = 1;
                }
                if (inputState.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Up))
                {
                    cameraMovement.Y = -1;
                }
                else if (inputState.CurrentKeyboardStates[(int)PlayerIndex.One].IsKeyDown(Keys.Down))
                {
                    cameraMovement.Y = 1;
                }
                if (inputState.IsScrollDown(controllingPlayer))
                {
                    AdjustZoom(-0.1f);
                }
                else if (inputState.IsScrollUp(controllingPlayer))
                {
                    AdjustZoom(0.1f);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                {
                    AdjustZoom(-0.05f);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Add))
                {
                    AdjustZoom(0.05f);
                }

                if (cameraMovement != Vector2.Zero)
                {
                    cameraMovement.Normalize();
                }

                cameraMovement *= Global.tileSize;

                MoveCamera(cameraMovement);
            }
        }
    }
}
