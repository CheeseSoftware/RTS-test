using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpCEGui.Base;
using Texture = SharpCEGui.Base.Texture;

namespace RTS_test.GUI
{
    public class MonoGameGeometryBuffer : GeometryBuffer
    {
        public MonoGameGeometryBuffer(MonoGameRenderer owner, GraphicsDevice device)
        {
            _owner = owner;
            _activeTexture = null;
            _clipRect = Rectf.Zero;
            _clippingActive = true;
            _translation = Vector3f.Zero;
            _rotation = SharpCEGui.Base.Quaternion.Zero;
            _pivot = Vector3f.Zero;
            _effect = null;
            _device = device;
            _matrixValid = false;
        }

        public override void Draw()
        {
            var savedClip = _device.ScissorRectangle;

            // setup clip region
            var clip = new Rectangle((int) _clipRect.Left, (int) _clipRect.Top,
                                     (int) _clipRect.Width, (int) _clipRect.Height);

            // apply the transformations we need to use.
            if (!_matrixValid)
                UpdateMatrix();

            _owner._basicEffect.World = _matrix;
            //_device.SetTransform(SharpDX.Direct3D9.TransformState.World, ref _matrix);

            //_owner.SetupRenderingBlendMode(d_blendMode);

            var localVertices = _vertices.ToArray();
            var passCount = _effect != null ? _effect.GetPassCount() : 1;
            for (var pass = 0; pass < passCount; ++pass)
            {
                // set up RenderEffect
                if (_effect != null)
                    _effect.PerformPreRenderFunctions(pass);

                // draw the batches
                var pos = 0;
                foreach (var i in _batches)
                {
                    if (i.Clip)
                        _device.ScissorRectangle = clip;

                    //_device.Textures[0] = i.Texture;
                    _owner._basicEffect.Texture = i.Texture;
                    foreach (var effectPass in _owner._basicEffect.CurrentTechnique.Passes)
                    {
                        effectPass.Apply();
                        _device.DrawUserPrimitives(PrimitiveType.TriangleList, localVertices, pos, i.VertexCount / 3);
                    }
                    
                    pos += i.VertexCount;

                    if (i.Clip)
                        _device.ScissorRectangle = savedClip;
                }
            }

            // clean up RenderEffect
            if (_effect != null)
                _effect.PerformPostRenderFunctions();
        }

        protected void UpdateMatrix()
        {
            var r = new Microsoft.Xna.Framework.Quaternion(_rotation.d_w, _rotation.d_x, _rotation.d_y, _rotation.d_z);

            _matrix = Matrix.CreateTranslation(_translation.d_x + _pivot.d_x,
                                               _translation.d_y + _pivot.d_y,
                                               _translation.d_z + _pivot.d_z);
            var rotationMatrix = Matrix.CreateFromQuaternion(r);
            _matrix = _matrix*rotationMatrix;

            _matrix = _matrix*Matrix.CreateTranslation(-_pivot.d_x, -_pivot.d_y, -_pivot.d_z);

            //_matrix = SharpDX.Matrix.Transformation(new SharpDX.Vector3(1), SharpDX.Quaternion.Identity,
            //                                         new SharpDX.Vector3(1), p, r, t);
            _matrixValid = true;
        }

        public override void SetTranslation(Vector3f t)
        {
            _translation = t;
            _matrixValid = false;
        }

        public override void SetRotation(SharpCEGui.Base.Quaternion q)
        {
            _rotation = q;
            _matrixValid = false;
        }

        public override void SetPivot(Vector3f p)
        {
            _pivot = p;
            _matrixValid = false;
        }

        public override void SetClippingRegion(Rectf region)
        {
            _clipRect.Top = Math.Max(0.0f, region.Top);
            _clipRect.Bottom = Math.Max(0.0f, region.Bottom);
            _clipRect.Left = Math.Max(0.0f, region.Left);
            _clipRect.Right = Math.Max(0.0f, region.Right);
        }

        public override void AppendVertex(Vertex vertex)
        {
            throw new System.NotImplementedException();
        }

        public override void AppendGeometry(IEnumerable<Vertex> vbuff, int vertexCount)
        {
            PerformBatchManagement();

            // update size of current batch
            _batches[_batches.Count - 1].VertexCount += vertexCount;

            // buffer these vertices
            foreach (var vertex in vbuff)
            {
                // copy vertex info the buffer, converting from CEGUI::Vertex to
                // something directly usable by D3D as needed.
                //_vertices.Add(new D3DVertex
                //{
                //    x = vertex.Position.d_x - 0.5f,
                //    y = vertex.Position.d_y - 0.5f,
                //    z = vertex.Position.d_z,
                //    diffuse = vertex.Colour.ToAlphaRGB(),
                //    tu = vertex.TextureCoordinates.d_x,
                //    tv = vertex.TextureCoordinates.d_y
                //});
                _vertices.Add(new Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture
                                  {
                                      Position = new Vector3(vertex.Position.d_x - 0.5f, vertex.Position.d_y - 0.5f, vertex.Position.d_z),
                                      Color = new Color(vertex.Colour.Red, vertex.Colour.Green, vertex.Colour.Blue, vertex.Colour.Alpha),
                                      TextureCoordinate = new Vector2(vertex.TextureCoordinates.d_x, vertex.TextureCoordinates.d_y)
                                  });
            }
        }

        protected void PerformBatchManagement()
        {
            var t = _activeTexture != null ? _activeTexture.Texture : null;

            // create a new batch if there are no batches yet, or if the active texture
            // differs from that used by the current batch.
            if (_batches.Count == 0 || t != _batches[_batches.Count - 1].Texture ||
                _clippingActive != _batches[_batches.Count - 1].Clip)
            {
                _batches.Add(new BatchInfo
                {
                    Texture = t,
                    VertexCount = 0,
                    Clip = _clippingActive
                });
            }
        }

        public override void SetActiveTexture(Texture texture)
        {
            _activeTexture = (MonoGameTexture)texture;
        }

        public override void Reset()
        {
            _batches.Clear();
            _vertices.Clear();
            _activeTexture = null;
        }

        public override Texture GetActiveTexture()
        {
            throw new System.NotImplementedException();
        }

        public override int GetVertexCount()
        {
            throw new System.NotImplementedException();
        }

        public override int GetBatchCount()
        {
            throw new System.NotImplementedException();
        }

        public override void SetRenderEffect(RenderEffect effect)
        {
            throw new System.NotImplementedException();
        }

        public override RenderEffect GetRenderEffect()
        {
            return _effect;
        }

        public override void SetClippingActive(bool value)
        {
            _clippingActive = value;
        }

        public override bool IsClippingActive()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// internal Vertex structure used for Direct3D based geometry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct D3DVertex : IVertexType
        {
            //! The transformed position for the vertex.
            public float x, y, z;

            //! colour of the vertex.
            public int diffuse;
            
            //! texture coordinates.
            public float tu, tv;

            public VertexDeclaration VertexDeclaration
            {
                get
                {
                    return
                        new VertexDeclaration(
                            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
                }
            }
        }

        /// <summary>
        /// type to track info for per-texture sub batches of geometry
        /// </summary>
        protected class BatchInfo
        {
            public Texture2D Texture;
            public int VertexCount;
            public bool Clip;
        }

        #region Fields

        /// <summary>
        /// last texture that was set as active
        /// </summary>
        private MonoGameTexture _activeTexture;
        
        /// <summary>
        /// rectangular clip region
        /// </summary>
        private Rectf _clipRect;

        /// <summary>
        /// whether clipping will be active for the current batch
        /// </summary>
        private bool _clippingActive;

        /// <summary>
        /// translation vector
        /// </summary>
        private Vector3f _translation;

        /// <summary>
        /// rotation vector
        /// </summary>
        private SharpCEGui.Base.Quaternion _rotation;

        /// <summary>
        /// pivot point for rotation
        /// </summary>
        private Vector3f _pivot;

        /// <summary>
        /// RenderEffect that will be used by the GeometryBuffer
        /// </summary>
        private RenderEffect _effect;

        /// <summary>
        /// model matrix cache
        /// </summary>
        private Matrix _matrix;

        /// <summary>
        /// true when d_matrix is valid and up to date
        /// </summary>
        private bool _matrixValid;

        /// <summary>
        /// Owning Direct3D9Renderer object
        /// </summary>
        private readonly MonoGameRenderer _owner;

        /// <summary>
        /// The D3D Device
        /// </summary>
        private readonly GraphicsDevice _device;

        //! list of texture batches added to the geometry buffer
        private readonly List<BatchInfo> _batches = new List<BatchInfo>();

        //! container where added geometry is stored.
        //private readonly List<D3DVertex> _vertices = new List<D3DVertex>();
        private readonly List<Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture> _vertices =
            new List<Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture>();

        #endregion
    }
}