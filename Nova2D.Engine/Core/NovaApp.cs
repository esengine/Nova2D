using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Nova2D.Engine.Graphics;
using Nova2D.Engine.ECS;
using Nova2D.Engine.UI;
using System.Numerics;
using Texture = Nova2D.Engine.Graphics.Texture;

namespace Nova2D.Engine.Core
{
    /// <summary>
    /// Static app launcher for Nova2D. Encapsulates window creation, input, and engine bootstrap.
    /// </summary>
    public static class NovaApp
    {
        private static IWindow? _window;
        private static GL? _gl;
        private static IInputContext? _input;
        private static IMouse? _mouse;
        private static IKeyboard? _keyboard;

        private static NovaGame? _game;
        private static UIButton? _statsToggleButton;

        public static void Run(NovaGame game, int width, int height, string title)
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(width, height);
            options.Title = title;
            options.VSync = false;

            _window = Window.Create(options);
            _game = game;

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;

            _window.Run();
        }

        private static void OnLoad()
        {
            _gl = GL.GetApi(_window);
            _input = _window!.CreateInput();
            _mouse = _input.Mice.Count > 0 ? _input.Mice[0] : null;
            _keyboard = _input.Keyboards.Count > 0 ? _input.Keyboards[0] : null;

            _gl.Enable(GLEnum.Blend);
            _gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

            Texture.CreateWhiteTexture(_gl);

            NovaContext.Initialize(_gl, _mouse!, _window);
            _game?.OnLoad();
        }

        private static void OnUpdate(double delta)
        {
            NovaContext.BeginFrame();
            
            _game?.OnUpdate((float)delta);
            NovaContext.Scene?.Update((float)delta);
            
            if (NovaContext.Mouse != null)
            {
                NovaContext.UICanvas?.Update(
                    NovaContext.Mouse.Position,
                    NovaContext.Mouse.IsButtonPressed(MouseButton.Left)
                );
            }
        }

        private static void OnRender(double delta)
        {
            _gl!.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            NovaContext.Scene?.Render();
            NovaContext.UICanvas?.Render(NovaContext.SpriteBatch!);
            NovaContext.RenderOverlay();
            NovaContext.EndFrame();
        }

        private static void OnClose()
        {
            _game?.OnClose();
        }
    }
}
