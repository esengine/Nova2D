using Silk.NET.OpenGL;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Nova2D.Engine.Core;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// A high-performance 2D sprite batch renderer that reduces draw calls by submitting many quads in a single draw.
    /// </summary>
    public unsafe class SpriteBatch2D : IDisposable
    {
        private const int MaxSprites = 1000;
        private const int VerticesPerSprite = 4;
        private const int IndicesPerSprite = 6;

        private readonly GL _gl;
        private readonly Shader _shader;
        private readonly uint _vao;
        private readonly uint _vbo;
        private readonly uint _ebo;

        private readonly Vertex[] _vertices;
        private readonly uint[] _indices;
        
        private int _spriteCount = 0;
        private Texture? _currentTexture;
        private Matrix4x4 _mvp;
        
        private bool _hasBegun = false;
        
        private readonly int _uMvpLocation;
        /// <summary>
        /// Total number of draw calls issued this frame.
        /// </summary>
        public static int TotalDrawCallsThisFrame { get; set; } = 0;
        
        private static readonly Vector2[] _unitQuad = new Vector2[]
        {
            new(0, 0), new(1, 0), new(1, 1), new(0, 1)
        };

        /// <summary>
        /// Defines the layout of a vertex used in the batch (position, texture coordinate, color).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex
        {
            public Vector2 Position;
            public Vector2 TexCoord;
            public Vector4 Color;
        }

        public SpriteBatch2D(GL gl, Shader shader)
        {
            _gl = gl;
            _shader = shader;

            _vertices = new Vertex[MaxSprites * VerticesPerSprite];
            _indices = new uint[MaxSprites * IndicesPerSprite];

            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _ebo = _gl.GenBuffer();

            GenerateIndices();

            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(Vertex) * _vertices.Length), null, BufferUsageARB.DynamicDraw);

            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            fixed (uint* i = _indices)
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(_indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);

            int stride = sizeof(Vertex);
            _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)Marshal.OffsetOf<Vertex>("Position"));
            _gl.EnableVertexAttribArray(0);

            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)Marshal.OffsetOf<Vertex>("TexCoord"));
            _gl.EnableVertexAttribArray(1);

            _gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, (uint)stride, (void*)Marshal.OffsetOf<Vertex>("Color"));
            _gl.EnableVertexAttribArray(2);

            _gl.BindVertexArray(0);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

            _uMvpLocation = _gl.GetUniformLocation(shader.Handle, "uMVP");
        }

        private void GenerateIndices()
        {
            for (int i = 0; i < MaxSprites; i++)
            {
                int offset = i * VerticesPerSprite;
                int index = i * IndicesPerSprite;

                _indices[index + 0] = (uint)(offset + 0);
                _indices[index + 1] = (uint)(offset + 1);
                _indices[index + 2] = (uint)(offset + 2);
                _indices[index + 3] = (uint)(offset + 2);
                _indices[index + 4] = (uint)(offset + 3);
                _indices[index + 5] = (uint)(offset + 0);
            }
        }

        /// <summary>
        /// Begins a new batch using an orthographic projection based on the current window size.
        /// Use this for rendering UI elements.
        /// </summary>
        public void Begin()
        {
            var projection = Matrix4x4.CreateOrthographicOffCenter(
                0f, NovaContext.WindowSize.X,
                NovaContext.WindowSize.Y, 0f,
                -1f, 1f
            );
            Begin(projection);
        }
        
        /// <summary>
        /// Begins a new batch with a given MVP matrix (usually ViewProjection).
        /// </summary>
        public void Begin(Matrix4x4 mvp)
        {
            if (_hasBegun)
                throw new InvalidOperationException("SpriteBatch already begun!");

            _spriteCount = 0;
            _currentTexture = null;
            _mvp = mvp;
            _hasBegun = true;
        }

        /// <summary>
        /// Ends the batch and flushes all pending draw calls.
        /// </summary>
        public void End()
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before End");

            Flush();
            _hasBegun = false;
        }
        
        /// <summary>
        /// Draws a textured sprite quad with optional transform, color and origin.
        /// </summary>
        public void Draw(Texture texture, Vector2 position, Vector2 size, Vector4 color, Vector2 origin = default, float rotation = 0f)
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Call Begin() before Draw()");

            EnsureTexture(texture);
            
            var model = GetModelMatrix(position, size, origin, rotation);
            int baseIndex = _spriteCount * VerticesPerSprite;

            for (int i = 0; i < 4; i++)
            {
                Vector3 world = Vector3.Transform(new Vector3(_unitQuad[i], 0f), model);
                _vertices[baseIndex + i] = new Vertex
                {
                    Position = new Vector2(world.X, world.Y),
                    TexCoord = _unitQuad[i],
                    Color = color
                };
            }

            _spriteCount++;
        }
        
        /// <summary>
        /// Draws a sprite using a source rectangle from a texture atlas.
        /// </summary>
        public void Draw(Texture texture, Vector2 position, Vector2 size, Rectangle source, Vector4 color, Vector2 origin = default, float rotation = 0f)
        {
            EnsureTexture(texture);
            
            int baseIndex = _spriteCount * VerticesPerSprite;

            Matrix4x4 model =
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4x4.CreateScale(size.X, size.Y, 1f) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0f);

            float texW = texture.Width;
            float texH = texture.Height;

            Vector2 uv0 = new(source.X / texW, source.Y / texH);
            Vector2 uv1 = new((source.X + source.Width) / texW, source.Y / texH);
            Vector2 uv2 = new((source.X + source.Width) / texW, (source.Y + source.Height) / texH);
            Vector2 uv3 = new(source.X / texW, (source.Y + source.Height) / texH);

            Vector2[] uvs = { uv0, uv1, uv2, uv3 };
            
            for (int i = 0; i < 4; i++)
            {
                Vector3 world = Vector3.Transform(new Vector3(_unitQuad[i], 0f), model);
                _vertices[baseIndex + i] = new Vertex
                {
                    Position = new Vector2(world.X, world.Y),
                    TexCoord = uvs[i],
                    Color = color
                };
            }

            _spriteCount++;
        }
        
        private static Matrix4x4 GetModelMatrix(Vector2 position, Vector2 size, Vector2 origin, float rotation)
        {
            return
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4x4.CreateScale(size.X, size.Y, 1f) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0f);
        }

        private void EnsureTexture(Texture texture)
        {
            if (_spriteCount >= MaxSprites)
            {
                Flush();
                _currentTexture = texture;
            }

            if (_currentTexture != null && _currentTexture != texture)
            {
                Flush();
                _currentTexture = texture;
            }

            _currentTexture ??= texture;
        }
        
        private void Flush()
        {
            if (_spriteCount == 0 || _currentTexture == null) return;
            
            _shader.Use();
            _currentTexture.Bind();

            fixed (Vertex* v = _vertices)
            {
                _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
                _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint)(_spriteCount * 4 * sizeof(Vertex)), v);
            }

            fixed (Matrix4x4* mvpPtr = &_mvp)
            {
                _gl.UniformMatrix4(_uMvpLocation, 1, false, (float*)mvpPtr);
            }

            _gl.BindVertexArray(_vao);
            _gl.DrawElements(
                (GLEnum)PrimitiveType.Triangles,
                (uint)(_spriteCount * IndicesPerSprite),
                (GLEnum)DrawElementsType.UnsignedInt,
                null
            );
            
            TotalDrawCallsThisFrame++;
            _spriteCount = 0;
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteBuffer(_ebo);
            _gl.DeleteVertexArray(_vao);
        }
    }
}
