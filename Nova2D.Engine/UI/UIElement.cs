using System.Numerics;

namespace Nova2D.Engine.UI
{
    /// <summary>
    /// The base class for all UI elements in NovaUI. Supports position, size, visibility and basic update/render lifecycle.
    /// This class is meant to be lightweight and render-only (no layout or parenting by default).
    /// </summary>
    public abstract class UIElement
    {
        /// <summary>
        /// Top-left position of the element in screen space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Size (width, height) of the element.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Whether the element is visible and should be rendered.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Whether the element is interactive and should receive input.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Called every frame with input state.
        /// </summary>
        /// <param name="mousePosition">The current mouse position in screen space.</param>
        /// <param name="isMousePressed">Whether the left mouse button is currently pressed.</param>
        public abstract void Update(Vector2 mousePosition, bool isMousePressed);

        /// <summary>
        /// Called during the render pass with the sprite batch.
        /// </summary>
        /// <param name="batch">The sprite batch used for rendering.</param>
        public abstract void Render(Graphics.SpriteBatch2D batch);
    }
}