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

        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "Nova2D Demo";

            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Closing += OnClose;

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
        }

        private static void OnRender(double delta)
        {
            _gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _renderer?.Draw(_texture!, new Vector2(0f, 0f), new Vector2(0.5f, 0.5f));
        }

        private static void OnClose()
        {
            _texture?.Dispose();
        }
    }
}