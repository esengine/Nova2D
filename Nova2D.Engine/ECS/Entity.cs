namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents a game object with optional components such as Transform and Sprite.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Spatial transform component (always present).
        /// </summary>
        public TransformComponent Transform { get; } = new();
        
        /// <summary>
        /// Optional sprite component for rendering.
        /// </summary>
        public SpriteComponent? Sprite { get; set; }
    }
}