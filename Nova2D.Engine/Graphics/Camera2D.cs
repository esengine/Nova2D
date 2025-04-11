using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    public class Camera2D
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Zoom { get; set; } = new(1f, 1f);
        public int ViewportWidth { get; private set; }
        public int ViewportHeight { get; private set; }

        public Camera2D(int width, int height)
        {
            Resize(width, height);
        }

        public void Resize(int width, int height)
        {
            ViewportWidth = width;
            ViewportHeight = height;
        }

        public Matrix4x4 GetMatrix()
        {
            var projection = Matrix4x4.CreateOrthographicOffCenter(
                0, ViewportWidth,
                ViewportHeight, 0,
                0f, 1f);

            var view = Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0f) *
                       Matrix4x4.CreateScale(Zoom.X, Zoom.Y, 1f);

            return view * projection;
        }
    }
}