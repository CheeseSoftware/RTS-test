using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpCEGui.Base;

namespace RTS_test.GUI
{
    public class MonoGameRenderer : Renderer
    {
        private ContentManager _contentManager;

        public static MonoGameRenderer Create(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            return new MonoGameRenderer(graphicsDevice, contentManager);
        }

        private MonoGameRenderer(GraphicsDevice device, ContentManager contentManager)
        {
            _device = device;
            _contentManager = contentManager;
            _displaySize = GetViewportSize();
            _displayDotsPerInch = new Vector2f(96, 96);
            _defaultTarget = null;

            //var caps = device.Capabilities;

            //if ((caps.RasterCaps & RasterCaps.ScissorTest) != RasterCaps.ScissorTest)
            //    throw new System.Exception("Hardware does not support D3DPRASTERCAPS_SCISSORTEST. Unable to proceed.");

            _maxTextureSize = 1024;// Math.Min(caps.MaxTextureHeight, caps.MaxTextureWidth);
            //_supportNonSquareTex = (caps.TextureCaps & TextureCaps.SquareOnly) != TextureCaps.SquareOnly;

            //_supportNonPowerOdTwoTexture =
            //    (caps.TextureCaps & TextureCaps.Pow2) != TextureCaps.Pow2 ||
            //    ((caps.TextureCaps & TextureCaps.NonPow2Conditional) == TextureCaps.NonPow2Conditional);
            
            //var effect = contentManager.Load<Effect>("effect");
            _basicEffect = new BasicEffect(device);
            
            _defaultTarget = new MonoGameViewportTarget(this);
        }

        public BasicEffect _basicEffect;

        public override RenderTarget GetDefaultRenderTarget()
        {
            return _defaultTarget;
        }

        public override GeometryBuffer CreateGeometryBuffer()
        {
            var b = new MonoGameGeometryBuffer(this, _device);
            _geometryBuffers.Add(b);
            return b;
        }

        public override void DestroyGeometryBuffer(GeometryBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override void DestroyAllGeometryBuffers()
        {
            throw new System.NotImplementedException();
        }

        public override TextureTarget CreateTextureTarget()
        {
            var t = new MonoGameTextureTarget(this);
            _textureTargets.Add(t);
            return t;
        }

        public override void DestroyTextureTarget(TextureTarget target)
        {
            throw new System.NotImplementedException();
        }

        public override void DestroyAllTextureTargets()
        {
            throw new System.NotImplementedException();
        }

        public override SharpCEGui.Base.Texture CreateTexture(string name)
        {
            throw new System.NotImplementedException();
        }

        public override SharpCEGui.Base.Texture CreateTexture(string name, string filename, string resourceGroup)
        {
            ThrowIfNameExists(name);

            var tex = new MonoGameTexture(this, name, filename, resourceGroup);
            _textures[name] = tex;

            LogTextureCreation(name);

            return tex;
        }

        public override SharpCEGui.Base.Texture CreateTexture(string name, Sizef size)
        {
            ThrowIfNameExists(name);

            var tex = new MonoGameTexture(this, name, size);
            _textures[name] = tex;

            LogTextureCreation(name);

            return tex;
        }

        internal MonoGameTexture CreateTexture(string name, Texture2D texture)
        {
            ThrowIfNameExists(name);

            var tex = new MonoGameTexture(this, name, texture);
            _textures[name] = tex;

            LogTextureCreation(name);

            return tex;
        }

        public override void DestroyTexture(SharpCEGui.Base.Texture texture)
        {
            throw new System.NotImplementedException();
        }

        public override void DestroyTexture(string name)
        {
            throw new System.NotImplementedException();
        }

        public override void DestroyAllTextures()
        {
            throw new System.NotImplementedException();
        }

        public override SharpCEGui.Base.Texture GetTexture(string name)
        {
            MonoGameTexture result;
            if (!_textures.TryGetValue(name, out result))
                throw new UnknownObjectException("[MonoGameRenderer] Texture does not exist: " + name);

            return result;
        }

        public override bool IsTextureDefined(string name)
        {
            return _textures.ContainsKey(name);
        }

        public override void BeginRendering()
        {
            //_device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;

            // no shaders initially
            //_device.VertexShader = null;
            //_device.PixelShader = null;

            //_basicEffect.VertexColorEnabled = true;
            //_basicEffect.TextureEnabled = true;

            // set device states
            //_device.SamplerStates[0]=new SamplerState();
            
            //_device.SetRenderState(RenderState.Lighting, false);
            _basicEffect.LightingEnabled = false;
            //_device.SetRenderState(RenderState.FogEnable, false);
            //_basicEffect.FogEnabled = false;
            //_device.SetRenderState(RenderState.ZEnable, false);
            //_device.SetRenderState(RenderState.AlphaTestEnable, false);
            //_device.SetRenderState(RenderState.CullMode, Cull.None);
            //_device.RasterizerState.CullMode = CullMode.None;
            //_device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            //_device.RasterizerState.FillMode = FillMode.Solid;
            //_device.SetRenderState(RenderState.ScissorTestEnable, true);
            //_device.RasterizerState.ScissorTestEnable = true;
            //_device.SetRenderState(RenderState.ZWriteEnable, false);

            // setup texture addressing settings
            //_device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            //_device.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
            //_device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            //_device.SamplerStates[0].AddressV = TextureAddressMode.Clamp;

            // setup colour calculations
            //_device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            //_device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
            //_device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);

            // setup alpha calculations
            //_device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
            //_device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);
            //_device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);

            // setup filtering
            //_device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            //_device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            //_device.SamplerStates[0].Filter = TextureFilter.Linear;

            // disable texture stages we do not need.
            //_device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Disable);

            // setup scene alpha blending
            //_device.SetRenderState(RenderState.AlphaBlendEnable, true);

            _device.DepthStencilState = DepthStencilState.Default;
			BlendState blend = new BlendState();
			blend.ColorSourceBlend = Blend.SourceAlpha;
			blend.ColorDestinationBlend = Blend.InverseSourceAlpha;
			blend.AlphaSourceBlend = Blend.InverseDestinationAlpha;
			blend.AlphaDestinationBlend = Blend.One;
			_device.BlendState = blend;
            _device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.None };

            _basicEffect.TextureEnabled = true;
            
            // put alpha blend operations into a known state
            //SetupRenderingBlendMode(BlendMode.Normal, true);

            

            // set view matrix back to identity.
            //_device.SetTransform(TransformState.View, SharpDX.Matrix.Identity);
            _basicEffect.View = Matrix.Identity;
        }

        public override void EndRendering()
        {
            
        }

        public override void SetDisplaySize(Sizef size)
        {
            throw new System.NotImplementedException();
        }

        public override Sizef GetDisplaySize()
        {
            return _displaySize;
        }

        public override Vector2f GetDisplayDPI()
        {
            return _displayDotsPerInch;
        }

        public override int GetMaxTextureSize()
        {
            return _maxTextureSize;
        }

        public override string GetIdentifierString()
        {
            return RendererId;
        }

        internal GraphicsDevice GetDevice()
        {
            return _device;
        }

        internal ContentManager Content
        {
            get { return _contentManager; }
        }

        private Sizef GetViewportSize()
        {
            var vp = _device.Viewport;
            return new Sizef(vp.Width, vp.Height);
        }

        /// <summary>
        /// helper to throw exception if name is already used.
        /// </summary>
        /// <param name="name"></param>
        private void ThrowIfNameExists(string name)
        {
            if (_textures.ContainsKey(name))
                throw new AlreadyExistsException("[MonoGameRenderer] Texture already exists: " + name);
        }

        /// <summary>
        /// helper to safely log the creation of a named texture
        /// </summary>
        /// <param name="name"></param>
        private static void LogTextureCreation(string name)
        {
            SharpCEGui.Base.System.GetSingleton().Logger
                .LogEvent("[MonoGameRenderer] Created texture: " + name);
        }

        /// <summary>
        /// helper to safely log the destruction of a named texture
        /// </summary>
        /// <param name="name"></param>
        private static void LogTextureDestruction(string name)
        {
            var logger = SharpCEGui.Base.System.GetSingleton().Logger;
            if (logger != null)
                logger.LogEvent("[MonoGameRenderer] Destroyed texture: " + name);
        }
        
        #region Fields

        //! Direct3DDevice9 interface we were given when constructed.
        private readonly GraphicsDevice _device;

        //! What the renderer considers to be the current display size.
        private Sizef _displaySize;

        /// <summary>
        /// What the renderer considers to be the current display DPI resolution.
        /// </summary>
        private readonly Vector2f _displayDotsPerInch;

        //! The default RenderTarget
        private readonly RenderTarget _defaultTarget;

        //! Container used to track texture targets.
        private readonly List<TextureTarget> _textureTargets = new List<TextureTarget>();

        //! Container used to track geometry buffers.
        private readonly List<MonoGameGeometryBuffer> _geometryBuffers = new List<MonoGameGeometryBuffer>();

        //! Container used to track textures.
        private readonly Dictionary<string, MonoGameTexture> _textures = new Dictionary<string, MonoGameTexture>();

        //! What the renderer thinks the max texture size is.
        private readonly int _maxTextureSize;

        //! whether the hardware supports non-power of two textures
        private readonly bool _supportNonPowerOdTwoTexture;

        //! whether the hardware supports non-square textures.
        private readonly bool _supportNonSquareTex;

        //! What we think is the active blendine mode
        private BlendMode _activeBlendMode;

        /// <summary>
        /// String holding the renderer identification text.
        /// </summary>
        private const string RendererId = "SharpCEGui.MonoGameRenderer";

        #endregion

        internal Sizef GetAdjustedSize(Sizef size)
        {
            var s = size;

            if (!_supportNonPowerOdTwoTexture)
            {
                s.d_width = GetSizeNextPowerOfTwo(size.d_width);
                s.d_height = GetSizeNextPowerOfTwo(size.d_height);
            }
            if (!_supportNonSquareTex)
                s.d_width = s.d_height = Math.Max(s.d_width, s.d_height);

            return s;
        }

        private static float GetSizeNextPowerOfTwo(float sz)
        {
            var size = (uint)sz;

            // if not power of 2
            if ((size & (size - 1)) == (size - 1) || size != 0)
            {
                int log = 0;

                // get integer log of 'size' to base 2
                while ((size >>= 1) != 0)
                    ++log;

                // use log to calculate value to use as size.
                size = (uint)(2 << log);
            }

            return size;
        }
    }
}