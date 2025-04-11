using System.Numerics;
using Nova2D.Engine.Graphics;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Shader = Nova2D.Engine.Graphics.Shader;
using Texture = Nova2D.Engine.Graphics.Texture;

namespace Nova2D.Demo
{
    internal static class Program
    {
        private static IWindow _window;
        private static GL _gl;
        private static Texture? _texture;
        
        private static SpriteRenderer? _renderer;
        private static Shader? _shader;
        private static Camera2D? _camera;
        
        private static float _angle = 0f;
        
        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "Nova2D Demo";

            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Closing += OnClose;
            _window.FramebufferResize += size =>
            {
                _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
                _camera?.Resize(size.X, size.Y);
            };

            _window.Run();
        }

        private static void OnLoad()
        {
            _gl = GL.GetApi(_window);

            var texPath = Path.Combine("Assets", "Textures", "test.png");
            _texture = new Texture(_gl, texPath);

            var vertPath = Path.Combine("Shaders", "sprite.vert");
            var fragPath = Path.Combine("Shaders", "sprite.frag");
            _shader = new Shader(_gl, vertPath, fragPath);

            _renderer = new SpriteRenderer(_gl, _shader);
            _camera = new Camera2D(_window.Size.X, _window.Size.Y);
        }

        private static void OnRender(double delta)
        {
            _gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _angle += (float)delta; // 旋转

            _renderer?.Draw(
                _texture!,
                new Vector2(0f, 0f),                 // 左上角
                new Vector2(128f, 128f),            // 大小
                0f,                                 // 不旋转
                Vector2.Zero,                       // 原点在左上角
                new Vector4(1f, 1f, 1f, 1f),        // 白色
                _camera!.GetMatrix()
            );
        }

        private static void OnClose()
        {
            _texture?.Dispose();
        }
    }
}