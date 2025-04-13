using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Nova2D.Engine.Core;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Window = Silk.NET.Windowing.Window;

namespace NovaStudio.Controls
{
    /// <summary>
    /// A native control that hosts a Silk.NET OpenGL rendering context.
    /// Used in the editor to render 2D scenes using the Nova2D engine.
    /// </summary>
    public class GLSceneView : NativeControlHost
    {
        private IWindow? _silkWindow;
        private GL? _gl;

        private bool _initialized;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (!_initialized)
            {
                _initialized = true;
                Task.Run(InitializeSilkWindow);
            }
        }

        /// <summary>
        /// Initializes the Silk.NET rendering window and Nova2D runtime.
        /// </summary>
        private void InitializeSilkWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.IsVisible = false;
            options.WindowBorder = WindowBorder.Hidden;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 1));
            options.IsContextControlDisabled = false;

            _silkWindow = Window.Create(options);

            _silkWindow.Load += OnLoad;
            _silkWindow.Render += OnRender;
            _silkWindow.Run(); // Non-blocking in Silk.NET 2+
        }

        /// <summary>
        /// Called once when the Silk.NET window has loaded.
        /// Initializes OpenGL and Nova2D.
        /// </summary>
        private void OnLoad()
        {
            _gl = _silkWindow?.CreateOpenGL();
            var input = _silkWindow?.CreateInput().Mice[0];

            if (_gl == null || input == null || _silkWindow == null)
                throw new InvalidOperationException("Failed to initialize Silk.NET rendering context.");

            NovaContext.Initialize(_gl, input, _silkWindow);
        }

        /// <summary>
        /// Called once per frame to update and render the Nova2D scene.
        /// </summary>
        private void OnRender(double delta)
        {
            if (_gl == null || NovaContext.Scene == null) return;

            NovaContext.BeginFrame();

            _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            _gl.ClearColor(0.12f, 0.16f, 0.18f, 1f);

            // Update ECS logic systems
            NovaContext.Scene.Update((float)delta);

            // Render all render systems (sprite batch, overlays, etc.)
            NovaContext.SpriteBatch?.Begin();
            NovaContext.Scene.Render();
            NovaContext.SpriteBatch?.End();

            // Optional debug overlays
            NovaContext.RenderOverlay();

            NovaContext.EndFrame();
        }
    }
}
