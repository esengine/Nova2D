using System.Diagnostics;
using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.NovaDebug
{
    /// <summary>
    /// A runtime performance stats overlay that displays FPS, draw calls, and entity count.
    /// </summary>
    public static class NovaStatsOverlay
    {
        private static readonly Stopwatch _frameTimer = new();
        private static int _frameCount = 0;
        private static float _fps = 0f;
        private static float _timeAccumulator = 0f;

        /// <summary>
        /// Whether the overlay is enabled and should be rendered.
        /// </summary>
        public static bool Enabled { get; set; } = false;

        /// <summary>
        /// Optional callback to retrieve draw call count.
        /// </summary>
        public static Func<int>? GetDrawCallCount;

        /// <summary>
        /// Optional callback to retrieve active entity count.
        /// </summary>
        public static Func<int>? GetEntityCount;

        /// <summary>
        /// Call at the start of the frame to begin timing.
        /// </summary>
        public static void BeginFrame()
        {
            if (!Enabled) return;
            _frameTimer.Start();
        }

        /// <summary>
        /// Call at the end of the frame to update stats.
        /// </summary>
        public static void EndFrame()
        {
            if (!Enabled) return;

            _frameCount++;
            _timeAccumulator += (float)_frameTimer.Elapsed.TotalSeconds;
            _frameTimer.Restart();

            if (_timeAccumulator >= 1.0f)
            {
                _fps = _frameCount / _timeAccumulator;
                _frameCount = 0;
                _timeAccumulator = 0f;
            }
        }

        /// <summary>
        /// Renders the debug overlay with stats.
        /// </summary>
        public static void Render(SpriteBatch2D spriteBatch, BitmapFontRenderer fontRenderer)
        {
            if (!Enabled) return;

            int drawCalls = GetDrawCallCount?.Invoke() ?? 0;
            int entities = GetEntityCount?.Invoke() ?? 0;

            fontRenderer.DrawText(spriteBatch, $"FPS: {_fps:F0}", new Vector2(10, 10), Vector4.One);
            fontRenderer.DrawText(spriteBatch, $"Draw Calls: {drawCalls}", new Vector2(10, 30), Vector4.One);
            fontRenderer.DrawText(spriteBatch, $"Entities: {entities}", new Vector2(10, 50), Vector4.One);
        }
    }
}