using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Renders 2D textured quads using a simple shader and MVP transform.
    /// </summary>
    public unsafe class SpriteRenderer
    {
        private readonly GL _gl;
        private readonly Shader _shader;
        private readonly uint _vao;
        private readonly uint _vbo;

        public SpriteRenderer(GL gl, Shader shader)
        {
            _gl = gl;
            _shader = shader;

            float[] vertices =
            {
                // Position   // UV
                0f, 1f,       0f, 1f,
                1f, 0f,       1f, 0f,
                0f, 0f,       0f, 0f,

                0f, 1f,       0f, 1f,
                1f, 1f,       1f, 1f,
                1f, 0f,       1f, 0f
            };

            _vao = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();

            _gl.BindVertexArray(_vao);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

            ReadOnlySpan<float> vertexSpan = vertices;
            _gl.BufferData(BufferTargetARB.ArrayBuffer, vertexSpan, BufferUsageARB.StaticDraw);

            const int stride = 4 * sizeof(float);

            _gl.VertexAttribPointer(index: 0, 2, VertexAttribPointerType.Float, false, stride, 0);
            _gl.EnableVertexAttribArray(0);

            _gl.VertexAttribPointer(index: 1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));
            _gl.EnableVertexAttribArray(1);

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            _gl.BindVertexArray(0);
        }

        /// <summary>
        /// Renders a single textured quad at a given position and size.
        /// </summary>
        public void Draw(
            Texture texture,
            Vector2 position,
            Vector2 size,
            float rotation,
            Vector2 origin,
            Vector4 color,
            Matrix4x4 cameraMatrix)
        {
            _shader.Use();

            // Transform
            var model =
                Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix4x4.CreateScale(size.X, size.Y, 1f) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0f);

            var mvp = model * cameraMatrix;
            
            int mvpLoc = _gl.GetUniformLocation(_shader.Handle, "uMVP");
            float* m = (float*)Unsafe.AsPointer(ref mvp);
            _gl.UniformMatrix4(mvpLoc, 1, false, m);

            int colorLoc = _gl.GetUniformLocation(_shader.Handle, "uColor");
            _gl.Uniform4(colorLoc, color.X, color.Y, color.Z, color.W);
            
            texture.Bind();
            _gl.BindVertexArray(_vao);
            _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
        
        /// <summary>
        /// Releases GL resources associated with this renderer.
        /// </summary>
        public void Dispose()
        {
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteVertexArray(_vao);
        }
    }
}