using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Renders 2D textured quads using a simple shader and MVP transform.
    /// Supports color tinting, rotation, and origin offset.
    /// </summary>
    public unsafe class SpriteRenderer : IDisposable
    {
        private readonly GL _gl;
        private readonly Shader _shader;
        private readonly uint _vao;
        private readonly uint _vbo;
        
        private readonly int _mvpLocation;
        private readonly int _colorLocation;
        private readonly int _flipXLocation;
        private readonly int _flipYLocation;
        private readonly int _grayscaleLocation;
        private readonly int _discardTransparentLocation;

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
            
            _mvpLocation = _gl.GetUniformLocation(_shader.Handle, "uMVP");
            _colorLocation = _gl.GetUniformLocation(_shader.Handle, "uColor");
            _flipXLocation = _gl.GetUniformLocation(_shader.Handle, "uFlipX");
            _flipYLocation = _gl.GetUniformLocation(_shader.Handle, "uFlipY");
            _grayscaleLocation = _gl.GetUniformLocation(_shader.Handle, "uGrayscale");
            _discardTransparentLocation = _gl.GetUniformLocation(_shader.Handle, "uDiscardTransparent");
        }

        /// <summary>
        /// Renders a single textured quad at a given position and size.
        /// </summary>
        public void DrawSprite(
            Texture texture,
            Vector2 position,
            Vector2 size,
            float rotation = 0f,
            Vector2? origin = null,
            Vector4? color = null,
            Matrix4x4? cameraMatrix = null,
            bool flipX = false,
            bool flipY = false,
            bool grayscale = false,
            bool discardTransparent = false)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            
            Vector2 actualOrigin = origin ?? Vector2.Zero;
            Vector4 actualColor = color ?? Vector4.One;
            Matrix4x4 camMatrix = cameraMatrix ?? Matrix4x4.Identity;
            
            _shader.Use();

            var model =
                Matrix4x4.CreateTranslation(-actualOrigin.X, -actualOrigin.Y, 0f) *
                Matrix4x4.CreateScale(size.X, size.Y, 1f) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(position.X, position.Y, 0f);

            var mvp = model * camMatrix;
            
            float* m = (float*)Unsafe.AsPointer(ref mvp);
            _gl.UniformMatrix4(_mvpLocation, 1, false, m);
            _gl.Uniform4(_colorLocation, actualColor.X, actualColor.Y, actualColor.Z, actualColor.W);

            _gl.Uniform1(_flipXLocation, flipX ? 1 : 0);
            _gl.Uniform1(_flipYLocation, flipY ? 1 : 0);
            _gl.Uniform1(_grayscaleLocation, grayscale ? 1 : 0);
            _gl.Uniform1(_discardTransparentLocation, discardTransparent ? 1 : 0);
            
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