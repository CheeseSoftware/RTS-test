using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpCEGui.Base;

namespace RTS_test.GUI
{
    public abstract class MonoGameRenderTarget : RenderTarget
    {
        protected MonoGameRenderTarget(MonoGameRenderer owner)
        {
            Owner = owner;
            Device = owner.GetDevice();
            Area = Rectf.Zero;
            _viewDistance = 0f;
            _matrixValid = false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<RenderTargetEventArgs> AreaChanged;

        public void Draw(GeometryBuffer buffer)
        {
            buffer.Draw();
        }

        public void Draw(RenderQueue queue)
        {
            queue.Draw();
        }

        public void SetArea(Rectf area)
        {
            Area = area;
            _matrixValid = false;

            var handler = AreaChanged;
            if (handler != null)
                handler(this, new RenderTargetEventArgs(this));
        }

        public Rectf GetArea()
        {
            return Area;
        }

        public abstract bool IsImageryCache();

        public virtual void Activate()
        {
            if (!_matrixValid)
                UpdateMatrix();

            var vp = new Viewport();
            SetupViewport(ref vp);
            Device.Viewport = vp;

            Owner._basicEffect.Projection = _matrix;
            //Owner.GetDevice().SetTransform(SharpDX.Direct3D9.TransformState.Projection, ref _matrix);
        }

        public virtual void Deactivate()
        {
        }


        protected void SetupViewport(ref Viewport vp)
        {
            vp.X = (int)Area.Left;
            vp.Y = (int)Area.Top;
            vp.Width = (int)Area.Width;
            vp.Height = (int)Area.Height;
            vp.MinDepth = 0.0f;
            vp.MaxDepth = 1.0f;
        }

        protected void UpdateMatrix()
        {
            const float fov = 0.523598776f;
            var w = Area.Width;
            var h = Area.Height;
            var aspect = w / h;
            var midx = w * 0.5f;
            var midy = h * 0.5f;
            _viewDistance = midx / (aspect * 0.267949192431123f);

            var eye = new Vector3(midx, midy, -_viewDistance);
            var at = new Vector3(midx, midy, 1);
            var up = new Vector3(0, -1, 0);

            _matrix = Matrix.Multiply(
                Matrix.CreateLookAt(eye, at, up),
                Matrix.CreatePerspectiveFieldOfView(fov, aspect, _viewDistance * 0.5f, _viewDistance * 2.0f));

            _matrixValid = true;
        }

        public void UnprojectPoint(GeometryBuffer buff, Vector2f pointIn, out Vector2f pointOut)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Direct3D9Renderer that created this object
        /// </summary>
        protected readonly MonoGameRenderer Owner;

        /// <summary>
        /// Direct3DDevice9 interface obtained from our owner.
        /// </summary>
        protected readonly GraphicsDevice Device;

        /// <summary>
        /// holds defined area for the RenderTarget
        /// </summary>
        protected Rectf Area;

        /// <summary>
        /// projection / view matrix cache
        /// </summary>
        private Matrix _matrix;

        /// <summary>
        /// true when d_matrix is valid and up to date
        /// </summary>
        private bool _matrixValid;

        /// <summary>
        /// tracks viewing distance (this is set up at the same time as d_matrix)
        /// </summary>
        private float _viewDistance;
    }
}