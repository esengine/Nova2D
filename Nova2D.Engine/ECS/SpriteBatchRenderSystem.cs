using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// A system that uses SpriteBatch2D to efficiently render all entities with both Transform and Sprite components.
    /// </summary>
    public class SpriteBatchRenderSystem : IRenderSystem
    {
        private readonly SpriteBatch2D _spriteBatch;
        private readonly Camera2D _camera;

        public SpriteBatchRenderSystem(SpriteBatch2D spriteBatch, Camera2D camera)
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        public void Render(Scene scene)
        {
            var viewMatrix = _camera.GetMatrix();

            _spriteBatch.Begin(viewMatrix);

            Texture? usedTexture = null;

            foreach (var entity in scene.Query<TransformComponent, SpriteComponent>())
            {
                var transform = entity.Get<TransformComponent>()!;
                var sprite = entity.Get<SpriteComponent>()!;

                if (usedTexture == null)
                    usedTexture = sprite.Texture;

                _spriteBatch.Draw(
                    sprite.Texture,
                    transform.Position,
                    sprite.Size * transform.Scale,
                    sprite.Color,
                    sprite.Origin,
                    transform.Rotation
                );
            }

            if (usedTexture != null)
                _spriteBatch.End(usedTexture);
        }
    }
}