using System.Numerics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Represents spatial transformation data for a 2D entity.
    /// Includes position, rotation, and scale.
    /// </summary>
    public class TransformComponent
    {
        /// <summary>
        /// World-space position of the entity (in pixels).
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;
        
        /// <summary>
        /// Rotation in radians (clockwise).
        /// </summary>
        public float Rotation { get; set; } = 0f;
        
        /// <summary>
        /// Non-uniform scale along X and Y axes.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;
    }
}