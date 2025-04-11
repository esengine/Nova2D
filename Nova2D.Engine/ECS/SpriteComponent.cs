using System.Numerics;

namespace Nova2D.Engine.ECS
{
    /// <summary>
    /// Defines rendering properties for a 2D textured sprite.
    /// </summary>
    public class SpriteComponent
    {
        /// <summary>
        /// The texture used for rendering this sprite.
        /// </summary>
        public Graphics.Texture Texture { get; set; }
        
        /// <summary>
        /// The size (in pixels) the sprite should be rendered at.
        /// </summary>
        public Vector2 Size { get; set; }
        
        /// <summary>
        /// Tint color (including alpha). Defaults to opaque white.
        /// </summary>
        public Vector4 Color { get; set; } = Vector4.One;
        
        /// <summary>
        /// Origin offset in pixels. Controls the pivot point for rotation and scaling.
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        public SpriteComponent(Graphics.Texture texture)
        {
            Texture = texture;
        }
        
        /// <summary>
        /// Sets the origin to the center of the sprite.
        /// </summary>
        public void SetOriginToCenter()
        {
            Origin = Size * 0.5f;
        }

        /// <summary>
        /// Sets the origin using a normalized ratio (0-1).
        /// </summary>
        public void SetOriginByRatio(Vector2 ratio)
        {
            Origin = Size * ratio;
        }
    }
}