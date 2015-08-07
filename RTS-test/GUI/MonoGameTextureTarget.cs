using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpCEGui.Base;

namespace RTS_test.GUI
{
    public class MonoGameTextureTarget : MonoGameRenderTarget, TextureTarget
    {
        public MonoGameTextureTarget(MonoGameRenderer owner) : base(owner)
        {
            _texture = null;
            //_surface = null;

            // this essentially creates a 'null' CEGUI::Texture
            _ceguiTexture = (MonoGameTexture)Owner.CreateTexture(GenerateTextureName(), null);

            // setup area and cause the initial texture to be generated.
            DeclareRenderSize(new Sizef(DefaultSize, DefaultSize));
        }

        public void Clear()
        {
            // switch to targetting our texture
            EnableRenderTexture();
            // Clear it.
            Device.Clear(ClearOptions.Target, new Color(0, 0, 0, 0), 1.0f, 0);
            // switch back to rendering to previous target
            DisableRenderTexture();
        }

        private void EnableRenderTexture()
        {
            var oldSurface = Device.GetRenderTargets();

            if (oldSurface.Length == 0)
            {
                _prevColourSurface = null;
                Device.SetRenderTarget(_texture);
            }
            else if (oldSurface.Length!=0 && oldSurface[0].RenderTarget != null && oldSurface[0].RenderTarget != _texture)
            {
                _prevColourSurface = (RenderTarget2D)oldSurface[0].RenderTarget;
                Device.SetRenderTarget(_texture);
            }
            else if (oldSurface.Length!=0 && oldSurface[0].RenderTarget != null)
                oldSurface[0].RenderTarget.Dispose();
        }

        private void DisableRenderTexture()
        {
            Device.SetRenderTarget(_prevColourSurface);

            if (_prevColourSurface != null)
            {
                _prevColourSurface.Dispose();
                _prevColourSurface = null;
            }
        }

        public override void Activate()
        {
            EnableRenderTexture();
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            DisableRenderTexture();
        }

        public SharpCEGui.Base.Texture GetTexture()
        {
            return _ceguiTexture;
        }

        public void DeclareRenderSize(Sizef sz)
        {
            // comment it out due directx debug runtime assertion
            // exit if current size is enough
            //if ((d_area.Width >= sz.d_width) && (d_area.Height >= sz.d_height))
            //    return;

            SetArea(new Rectf(Area.Position, sz));
            ResizeRenderTexture();
            Clear();
        }

        private void ResizeRenderTexture()
        {
            CleanupRenderTexture();
            InitialiseRenderTexture();
        }

        private void CleanupRenderTexture()
        {
            //if (_surface != null)
            //{
            //    _surface.Dispose();
            //    _surface = null;
            //}
            if (_texture != null)
            {
                _ceguiTexture.SetDirect3D9Texture(null);
                _texture.Dispose();
                _texture = null;
            }
        }

        private void InitialiseRenderTexture()
        {
            var textureSize = (Owner.GetAdjustedSize(Area.Size));

            _texture = new RenderTarget2D(Device, (int) textureSize.d_width, (int) textureSize.d_height, false,
                                          SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);

            //_surface = _texture.GetSurfaceLevel(0);

            // wrap the created texture with the CEGUI::Texture
            _ceguiTexture.SetDirect3D9Texture(_texture);
            _ceguiTexture.SetOriginalDataSize(Area.Size);
        }

        public override bool IsImageryCache()
        {
            return true;
        }

        public bool IsRenderingInverted()
        {
            return false;
        }

        private static string GenerateTextureName()
        {
            return String.Format(CultureInfo.InvariantCulture, "_mg_tt_tex_{0}", _textureNumber++);
        }

        #region Fields

        /// <summary>
        /// Direct3D9 texture that's rendered to.
        /// </summary>
        private RenderTarget2D _texture;

        /// <summary>
        /// Direct3D9 surface for the texture
        /// </summary>
        //private SharpDX.Direct3D9.Surface _surface;

        /// <summary>
        /// we use this to wrap d_texture so it can be used by the core CEGUI lib.
        /// </summary>
        private readonly MonoGameTexture _ceguiTexture;

        /// <summary>
        /// colour surface that was in use before this target was activated.
        /// </summary>
        //private SharpDX.Direct3D9.Surface _prevColourSurface;
        private RenderTarget2D _prevColourSurface;

        /// <summary>
        /// static data used for creating texture names
        /// </summary>
        private static uint _textureNumber;

        /// <summary>
        /// default size of created texture objects
        /// </summary>
        private const float DefaultSize = 128.0f;

        #endregion
    }
}