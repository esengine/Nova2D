using Silk.NET.OpenGL;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

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
        private int _spriteCount = 0;

        private readonly uint[] _indices;

        private Matrix4x4 _mvp;
        private readonly int _uMvpLocation;
        
        public static int TotalDrawCallsThisFrame { get; set; } = 0;

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
            _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)0);
            _gl.EnableVertexAttribArray(0);

            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)(sizeof(float) * 2));
            _gl.EnableVertexAttribArray(1);

            _gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, (uint)stride, (void*)(sizeof(float) * 4));
            _gl.EnableVertexAttribArray(2);

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            _gl.BindVertexArray(0);

            _uMvpLocation = _gl.GetUniformLocation(_shader.Handle, "uMVP");
        }

        private void GenerateIndices()
        {
            for (int i = 0; i < MaxSprites; i++)
            {
                int offset = i * 4;
                int index = i * 6;

                _indices[index + 0] = (uint)(offset + 0);
                _indices[index + 1] = (uint)(offset + 1);
                _indices[index + 2] = (uint)(offset + 2);
                _indices[index + 3] = (uint)(offset + 2);
                _indices[index + 4] = (uint)(offset + 3);
                _indices[index + 5] = (uint)(offset + 0);
            }
        }

        /// <summary>
        /// Begins a new batch with a given MVP matrix (usually ViewProjection).
        /// </summary>
        public void Begin(Matrix4x4 mvp)
        {
            _spriteCount = 0;
            _mvp = mvp;
        }

        /// <summary>
        /// Queues a sprite to be rendered in this batch.
        /// </summary>
        public void Draw(Texture texture, Vector2 position, Vector2 size, Vector4 color, Vector2 origin = default, float rotation = 0f)
        {
            if (_spriteCount >= MaxSprites)
                Flush(texture); // auto flush if batch is full

            int baseIndex = _spriteCount * 4;

            // Compute model transform
            var model =
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                Matrix4x4.CreateScale(size.X, size.Y, 1) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0);

            // Quad corners
            Vector2[] quadPos = {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            };

            Vector2[] quadUV = {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            };

            for (int i = 0; i < 4; i++)
            {
                Vector3 world = Vector3.Transform(new Vector3(quadPos[i], 0), model);
                _vertices[baseIndex + i] = new Vertex
                {
                    Position = new Vector2(world.X, world.Y),
                    TexCoord = quadUV[i],
                    Color = color
                };
            }

            _spriteCount++;
        }
        
        public void Draw(Texture texture, Vector2 position, Vector2 size, Rectangle source, Vector4 color, Vector2 origin = default, float rotation = 0f)
        {
            if (_spriteCount >= MaxSprites)
                Flush(texture);

            int baseIndex = _spriteCount * VerticesPerSprite;

            Matrix4x4 model =
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4x4.CreateScale(size.X, size.Y, 1f) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0f);

            Vector2[] quadPos = {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1)
            };

            float texWidth = texture.Width;
            float texHeight = texture.Height;

            Vector2[] quadUV = {
                new(source.X / texWidth, source.Y / texHeight),
                new((source.X + source.Width) / texWidth, source.Y / texHeight),
                new((source.X + source.Width) / texWidth, (source.Y + source.Height) / texHeight),
                new(source.X / texWidth, (source.Y + source.Height) / texHeight),
            };

            for (int i = 0; i < 4; i++)
            {
                Vector3 world = Vector3.Transform(new Vector3(quadPos[i], 0f), model);
                _vertices[baseIndex + i] = new Vertex
                {
                    Position = new Vector2(world.X, world.Y),
                    TexCoord = quadUV[i],
                    Color = color
                };
            }

            _spriteCount++;
        }


        /// <summary>
        /// Ends the batch and submits all data to GPU.
        /// </summary>
        public void End(Texture texture)
        {
            if (_spriteCount == 0) return;
            Flush(texture);
            TotalDrawCallsThisFrame++;
        }

        private void Flush(Texture texture)
        {
            texture.Bind();
            _shader.Use();

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
                (uint)(_spriteCount * 6),
                (GLEnum)DrawElementsType.UnsignedInt,
                null
            );
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteBuffer(_ebo);
            _gl.DeleteVertexArray(_vao);
        }
    }
}
