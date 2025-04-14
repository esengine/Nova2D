using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Nova2D.Engine.Graphics;
using Nova2D.Engine.UI;
using Nova2D.Engine.ECS;
using System.Collections.Generic;
using System.Numerics;
using Nova2D.Engine.NovaDebug;
using Shader = Nova2D.Engine.Graphics.Shader;
using Texture = Nova2D.Engine.Graphics.Texture;

namespace Nova2D.Engine.Core
{
    /// <summary>
    /// Central runtime context for Nova2D. Stores engine-wide resources.
    /// </summary>
    public static class NovaContext
    {
        public static GL? GL { get; private set; }
        public static IMouse? Mouse { get; private set; }
        public static IWindow? Window { get; private set; }

        public static Scene? Scene { get; private set; }
        public static UICanvas? UICanvas { get; private set; }
        public static SpriteBatch2D? SpriteBatch { get; private set; }

        public static Dictionary<string, Texture> Textures { get; } = new();
        public static Dictionary<string, BitmapFont> Fonts { get; } = new();
        /// <summary>
        /// Cached font renderer using the "main" font.
        /// </summary>
        public static BitmapFontRenderer? FontRenderer { get; private set; }
        
        public static Vector2 WindowSize => new(Window?.Size.X ?? 0, Window?.Size.Y ?? 0);

        /// <summary>
        /// Initializes the core systems. Called once at startup.
        /// </summary>
        public static void Initialize(GL gl, IMouse mouse, IWindow window)
        {
            GL = gl;
            Mouse = mouse;
            Window = window;

            Scene = new Scene();
            UICanvas = new UICanvas();
            var batchShader = new Shader(gl, "Shaders/spritebatch.vert", "Shaders/spritebatch.frag");
            SpriteBatch = new SpriteBatch2D(gl, batchShader);

            // Register ECS-based render system
            var camera = new Camera2D(window.Size.X, window.Size.Y);
            Scene.AddSystem(new SmartSpriteBatchRenderSystem(SpriteBatch, camera));
            Scene.AddSystem(new AnimationSystem());
            
            // Register debug stat collectors
            NovaStatsOverlay.GetDrawCallCount = () => SpriteBatch2D.TotalDrawCallsThisFrame;
            NovaStatsOverlay.GetEntityCount = () => Scene?.EntityCount ?? 0;
        }
        
        /// <summary>
        /// Begins the frame for timing stats.
        /// Should be called in NovaApp before any logic.
        /// </summary>
        public static void BeginFrame()
        {
            SpriteBatch2D.TotalDrawCallsThisFrame = 0;
            NovaStatsOverlay.BeginFrame();
        }

        /// <summary>
        /// Ends the frame timing.
        /// Should be called in NovaApp after render.
        /// </summary>
        public static void EndFrame()
        {
            NovaStatsOverlay.EndFrame();
        }

        /// <summary>
        /// Renders the debug overlay.
        /// Should be called after scene/UI rendering.
        /// </summary>
        public static void RenderOverlay()
        {
            if (!NovaStatsOverlay.Enabled || SpriteBatch == null) return;

            // Lazy-load FontRenderer from "main" font
            if (FontRenderer == null && Fonts.TryGetValue("main", out var font))
            {
                FontRenderer = new BitmapFontRenderer(font);
            }
            
            if (FontRenderer != null)
            {
                SpriteBatch.Begin();
                NovaStatsOverlay.Render(SpriteBatch, FontRenderer);
                SpriteBatch.End();
            }
        }
    }
}