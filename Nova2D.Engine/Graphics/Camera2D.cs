using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// A simple 2D orthographic camera for screen-space to world-space transformations.
    /// Handles position, zoom, and viewport resizing.
    /// </summary>
    public class Camera2D
    {
        /// <summary>
        /// The current position of the camera in world space (top-left corner).
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;
        
        /// <summary>
        /// The zoom level of the camera on X and Y axes. Default is (1, 1).
        /// </summary>
        public Vector2 Zoom { get; set; } = new(1f, 1f);
        
        /// <summary>
        /// Current width of the viewport in pixels.
        /// </summary>
        public int ViewportWidth { get; private set; }
        
        /// <summary>
        /// Current height of the viewport in pixels.
        /// </summary>
        public int ViewportHeight { get; private set; }

        /// <summary>
        /// Constructs a new 2D camera with the given viewport dimensions.
        /// </summary>
        /// <param name="width">Initial width of the viewport.</param>
        /// <param name="height">Initial height of the viewport.</param>
        public Camera2D(int width, int height)
        {
            Resize(width, height);
        }

        /// <summary>
        /// Resizes the camera viewport. Should be called when the window is resized.
        /// </summary>
        /// <param name="width">New viewport width.</param>
        /// <param name="height">New viewport height.</param>
        public void Resize(int width, int height)
        {
            ViewportWidth = width;
            ViewportHeight = height;
        }

        /// <summary>
        /// Returns the combined view-projection matrix to be passed to shaders.
        /// </summary>
        public Matrix4x4 GetMatrix()
        {
            // Create an orthographic projection where Y increases downward
            var projection = Matrix4x4.CreateOrthographicOffCenter(
                0, ViewportWidth,
                ViewportHeight, 0, // Top-down Y axis
                -1f, 1f);   // Near/Far plane

            // Apply camera position and zoom
            var view = Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0f) *
                       Matrix4x4.CreateScale(Zoom.X, Zoom.Y, 1f);

            // Projection * View is the conventional order for transforming from world to clip space
            return projection * view;
        }
    }
}