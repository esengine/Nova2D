using System.Numerics;
using Nova2D.Engine.ECS;
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

        private static Shader? _shader;
        private static Texture? _texture;
        private static SpriteRenderer? _renderer;
        private static Camera2D? _camera;

        private static Scene? _scene;
        private static Entity? _rotatingEntity;
        
        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "Nova2D Demo";

            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
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

            _texture = new Texture(_gl, Path.Combine("Assets", "Textures", "test.png"));
            _shader = new Shader(_gl, "Shaders/sprite.vert", "Shaders/sprite.frag");

            _renderer = new SpriteRenderer(_gl, _shader);
            _camera = new Camera2D(_window.Size.X, _window.Size.Y);
            _scene = new Scene();

            _rotatingEntity = new Entity
            {
                Sprite = new SpriteComponent(_texture!)
                {
                    Size = new Vector2(128, 128),
                    Color = Vector4.One
                }
            };
            _rotatingEntity.Transform.Position = new Vector2(400, 300);

            _scene.AddEntity(_rotatingEntity);
        }
        
        private static void OnUpdate(double delta)
        {
            if (_rotatingEntity != null)
                _rotatingEntity.Transform.Rotation += (float)delta;
        }

        private static void OnRender(double delta)
        {
            _gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _scene?.Render(_renderer!, _camera!);
        }

        private static void OnClose()
        {
            _texture?.Dispose();
            _shader?.Dispose();
        }
    }
}