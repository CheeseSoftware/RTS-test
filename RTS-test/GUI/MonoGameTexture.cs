using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpCEGui.Base;
using Texture = SharpCEGui.Base.Texture;

namespace RTS_test.GUI
{
    public sealed class MonoGameTexture : SharpCEGui.Base.Texture, IDisposable
    {
        internal MonoGameTexture(MonoGameRenderer owner, string name, string filename, string resourceGroup)
        {
            _owner = owner;
            _texture = null;
            _size = Sizef.Zero;
            _dataSize = Sizef.Zero;
            _texelScaling = Vector2f.Zero;
            _savedSurfaceDescValid = false;
            _name = name;

            LoadFromFile(filename, resourceGroup);
        }

        internal MonoGameTexture(MonoGameRenderer owner, string name, Texture2D tex)
        {
            _owner = owner;
            _texture = null;
            _size = Sizef.Zero;
            _dataSize = Sizef.Zero;
            _texelScaling = Vector2f.Zero;
            _savedSurfaceDescValid = false;
            _name = name;

            SetDirect3D9Texture(tex);
        }

        internal Texture2D Texture
        {
            get { return _texture; }
        }

        internal void SetDirect3D9Texture(Texture2D tex)
        {
            if (_texture != tex)
            {
                CleanupDirect3D9Texture();
                _dataSize.d_width = _dataSize.d_height = 0;

                _texture = tex;
                // TODO: ...
                //if (d_texture!=null)
                //    d_texture.AddRef();
            }

            UpdateTextureSize();
            _dataSize = _size;
            UpdateCachedScaleValues();
        }

        private void CleanupDirect3D9Texture()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }

        private void UpdateTextureSize()
        {
            // obtain details of the size of the texture
            if (_texture != null)
            {
                _size.d_width = _texture.Width;
                _size.d_height = _texture.Height;
            }
            // use the original size if query failed.
            // NB: This should probably be an exception.
            else
                _size = _dataSize;
        }

        private void UpdateCachedScaleValues()
        {
            //
            // calculate what to use for x scale
            //
            var orgW = _dataSize.d_width;
            var texW = _size.d_width;

            // if texture and original data width are the same, scale is based
            // on the original size.
            // if texture is wider (and source data was not stretched), scale
            // is based on the size of the resulting texture.
            _texelScaling.d_x = 1.0f / ((orgW == texW) ? orgW : texW);

            //
            // calculate what to use for y scale
            //
            var orgH = _dataSize.d_height;
            var texH = _size.d_height;

            // if texture and original data height are the same, scale is based
            // on the original size.
            // if texture is taller (and source data was not stretched), scale
            // is based on the size of the resulting texture.
            _texelScaling.d_y = 1.0f / ((orgH == texH) ? orgH : texH);
        }

        public override string GetName()
        {
            return _name;
        }

        public override Sizef GetSize()
        {
            return _size;
        }

        public override Sizef GetOriginalDataSize()
        {
            return _dataSize;
        }

        public override Vector2f GetTexelScaling()
        {
            return _texelScaling;
        }

        public override void LoadFromFile(string filename, string resourceGroup)
        {
            // get and check existence of CEGUI::System object
            var sys = SharpCEGui.Base.System.GetSingleton();
            if (sys == null)
                throw new System.Exception("CEGUI::System object has not been created!");

            // load file to memory via resource provider
            var texFile = new RawDataContainer();
            sys.GetResourceProvider().LoadRawDataContainer(filename, texFile, resourceGroup);

            // TODO: Texture res = sys.GetImageCodec().Load(texFile, this);
            _texture = Texture2D.FromStream(_owner.GetDevice(), texFile.Stream());
            
            _dataSize = new Sizef(_texture.Width,_texture.Height);
            UpdateTextureSize();
            UpdateCachedScaleValues();

            //var img = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(texFile.Stream());
            //var bmpData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height),
            //                           System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //var dest = new byte[bmpData.Stride * bmpData.Height];
            //System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dest, 0, bmpData.Stride * bmpData.Height);
            //LoadFromMemory(dest, new Sizef(img.Width, img.Height), PixelFormat.RGBA);
            //img.UnlockBits(bmpData);

            // unload file data buffer
            sys.GetResourceProvider().UnloadRawDataContainer(texFile);

            // It's an error
            // TODO: ...
            //if (res == null)
            //    throw new global::System.Exception(sys.GetImageCodec().GetIdentifierString() + " failed to load image '" + filename + "'.");
        }

        public override void LoadFromMemory(byte[] buffer, Sizef bufferSize, PixelFormat pixelFormat)
        {
            //if (!IsPixelFormatSupported(pixelFormat))
            //    throw new InvalidRequestException("Data was supplied in an unsupported pixel format.");

            var pixfmt = ToD3DPixelFormat(pixelFormat);
            CreateDirect3D9Texture(bufferSize, pixfmt);

            //var surface = GetTextureSurface();
            //var pixelBuffer = new PixelBuffer(buffer, bufferSize, pixelFormat);

            //var srcRect = new SharpDX.Rectangle(0, 0, (int)bufferSize.d_width, (int)bufferSize.d_height);

            //SharpDX.Direct3D9.Surface.FromMemory(surface, pixelBuffer.GetPixelDataPtr(),
            //                                     SharpDX.Direct3D9.Filter.None, 0,
            //                                     pixfmt == SharpDX.Direct3D9.Format.X8R8G8B8
            //                                         ? SharpDX.Direct3D9.Format.R8G8B8
            //                                         : pixfmt,
            //                                     pixelBuffer.GetPitch(), srcRect);
            //surface.Dispose();

            _texture.SetData(buffer);
        }

        private SurfaceFormat ToD3DPixelFormat(PixelFormat pixelFormat)
        {
            return SurfaceFormat.Color;
        }

        public override void BlitFromMemory(byte[] sourceData, Rectf area)
        {
            throw new NotImplementedException();
        }

        public override void BlitToMemory(byte[] targetData)
        {
            throw new NotImplementedException();
        }

        public override bool IsPixelFormatSupported(PixelFormat fmt)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region Fields

        /// <summary>
        /// The D3D9 texture we're wrapping.
        /// </summary>
        private Texture2D _texture;

        //! Size of the texture.
        private Sizef _size;

        //! original pixel of size data loaded into texture
        private Sizef _dataSize;

        //! cached pixel to texel mapping scale values.
        private Vector2f _texelScaling;

        //! holds info about the texture surface before we released it for reset.
        //private SurfaceDescription _savedSurfaceDesc;

        //! true when d_savedSurfaceDesc is valid and texture can be restored.
        private bool _savedSurfaceDescValid;

        /// <summary>
        /// Name the texture was created with.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Direct3D9Renderer object that created and owns this texture.
        /// </summary>
        private readonly MonoGameRenderer _owner;

        public MonoGameTexture(MonoGameRenderer owner, string name, Sizef size)
        {
            _owner = owner;
            _texture = null;
            _size = size;
            _dataSize = Sizef.Zero;
            _texelScaling = Vector2f.Zero;
            _savedSurfaceDescValid = false;
            _name = name;

            CreateDirect3D9Texture(size, SurfaceFormat.Color);
        }

        private void CreateDirect3D9Texture(Sizef sz, SurfaceFormat format)
        {
            CleanupDirect3D9Texture();

            var textureSize = sz; _owner.GetAdjustedSize(sz);

            _texture = new Texture2D(_owner.GetDevice(),
                                                     (int)textureSize.d_width,
                                                     (int)textureSize.d_height,
                                                     false,
                                                     format);

            _dataSize = sz;
            UpdateTextureSize();
            UpdateCachedScaleValues();
        }

        #endregion

        public void SetOriginalDataSize(Sizef size)
        {
            _dataSize = size;
            UpdateCachedScaleValues();
        }

		public override void BlitFromMemory(IntPtr sourcePtr, Rectf area)
		{
			throw new NotImplementedException();
		}
	}
}