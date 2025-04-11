using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Nova2D.Engine.ECS;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// A smart sprite batch system that groups entities by texture and renders them efficiently using SpriteBatch2D.
    /// </summary>
    public class SmartSpriteBatchRenderSystem : IRenderSystem
    {
        private readonly SpriteBatch2D _spriteBatch;
        private readonly Camera2D _camera;

        public SmartSpriteBatchRenderSystem(SpriteBatch2D spriteBatch, Camera2D camera)
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        public void Render(Scene scene)
        {
            var viewMatrix = _camera.GetMatrix();

            // Group entities by texture
            var grouped = scene
                .Query<TransformComponent, SpriteComponent>()
                .GroupBy(e => e.Get<SpriteComponent>()!.Texture);

            foreach (var group in grouped)
            {
                var texture = group.Key;
                _spriteBatch.Begin(viewMatrix);

                foreach (var entity in group)
                {
                    var transform = entity.Get<TransformComponent>()!;
                    var sprite = entity.Get<SpriteComponent>()!;

                    _spriteBatch.Draw(
                        texture,
                        transform.Position,
                        sprite.Size * transform.Scale,
                        sprite.Color,
                        sprite.Origin,
                        transform.Rotation
                    );
                }

                _spriteBatch.End(texture);
            }
        }
    }
}