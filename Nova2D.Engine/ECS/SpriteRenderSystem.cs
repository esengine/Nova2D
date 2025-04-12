using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// A basic sprite render system that renders each sprite immediately using SpriteRenderer.
    /// Not batched – suitable for debug tools, gizmos, and editor visuals.
    /// </summary>
    public class SpriteRenderSystem : IRenderSystem
    {
        private readonly SpriteRenderer _renderer;
        private readonly Camera2D _camera;

        public SpriteRenderSystem(SpriteRenderer renderer, Camera2D camera)
        {
            _renderer = renderer;
            _camera = camera;
        }

        /// <summary>
        /// Renders all entities with TransformComponent and SpriteComponent.
        /// </summary>
        public void Render(Scene scene)
        {
            foreach (var entity in scene.Query<TransformComponent, SpriteComponent>())
            {
                var transform = entity.Get<TransformComponent>()!;
                var sprite = entity.Get<SpriteComponent>()!;

                _renderer.DrawSprite(
                    sprite.Texture,
                    transform.Position,
                    sprite.Size * transform.Scale,
                    transform.Rotation,
                    sprite.Origin,
                    sprite.Color,
                    _camera.GetMatrix()
                );
            }
        }
    }
}