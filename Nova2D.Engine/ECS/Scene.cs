using System;
using System.Collections.Generic;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a scene containing entities and systems.
    /// Responsible for updating all systems and providing entity queries.
    /// </summary>
    public class Scene
    {
        private readonly List<Entity> _entities = new();
        private readonly List<ISystem> _systems = new();
        private readonly List<IRenderSystem> _renderSystems = new();

        /// <summary>
        /// Adds an entity to the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity) => _entities.Add(entity);

        /// <summary>
        /// Adds a logic or render system to the scene.
        /// </summary>
        public void AddSystem(object system)
        {
            if (system is ISystem s)
                _systems.Add(s);
            if (system is IRenderSystem rs)
                _renderSystems.Add(rs);
        }

        /// <summary>
        /// Updates all registered systems in the scene.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds since last update.</param>
        public void Update(float deltaTime)
        {
            foreach (var system in _systems)
            {
                system.Update(deltaTime, this);
            }
        }
        
        /// <summary>
        /// Renders all render systems.
        /// </summary>
        public void Render()
        {
            foreach (var system in _renderSystems)
            {
                system.Render(this);
            }
        }

        /// <summary>
        /// Gets all entities currently in the scene.
        /// </summary>
        public IEnumerable<Entity> GetAllEntities() => _entities;

        /// <summary>
        /// Queries all entities containing specific component types.
        /// </summary>
        public IEnumerable<Entity> Query<T1, T2>() where T1 : class where T2 : class
        {
            foreach (var entity in _entities)
            {
                if (entity.TryGet(out T1 _) && entity.TryGet(out T2 _))
                    yield return entity;
            }
        }
    }
}