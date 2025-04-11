using System;
using System.Collections.Generic;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a game entity composed of components.
    /// </summary>
    public class Entity
    {
        private readonly Dictionary<Type, object> _components = new();

        /// <summary>
        /// Adds or replaces a component of a specific type.
        /// </summary>
        public void Add<T>(T component) where T : class
        {
            _components[typeof(T)] = component;
        }

        /// <summary>
        /// Gets a component of the specified type if present.
        /// </summary>
        public T? Get<T>() where T : class
        {
            return _components.TryGetValue(typeof(T), out var value) ? value as T : null;
        }

        /// <summary>
        /// Tries to retrieve a component.
        /// </summary>
        public bool TryGet<T>(out T? component) where T : class
        {
            if (_components.TryGetValue(typeof(T), out var value))
            {
                component = value as T;
                return true;
            }

            component = null;
            return false;
        }

        /// <summary>
        /// Checks whether a component of the specified type exists.
        /// </summary>
        public bool Has<T>() where T : class => _components.ContainsKey(typeof(T));
    }
}