using System.Collections.Generic;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a collection of entities in a scene and handles their rendering.
    /// </summary>
    public class Scene
    {
        private readonly List<Entity> _entities = new();

        /// <summary>
        /// Adds a new entity to the scene.
        /// </summary>
        public void AddEntity(Entity entity) => _entities.Add(entity);

        /// <summary>
        /// Renders all entities with Sprite and Transform components.
        /// </summary>
        public void Render(SpriteRenderer renderer, Camera2D camera)
        {
            foreach (var entity in _entities)
            {
                if (entity.Sprite is null) continue;

                var t = entity.Transform;
                var s = entity.Sprite;

                renderer.Draw(
                    s.Texture,
                    t.Position,
                    s.Size * t.Scale,
                    t.Rotation,
                    s.Origin,
                    s.Color,
                    camera.GetMatrix()
                );
            }
        }
        
        /// <summary>
        /// Returns all entities in the scene.
        /// </summary>
        public IEnumerable<Entity> GetAllEntities() => _entities;
    }
}