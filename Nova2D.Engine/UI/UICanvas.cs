using System.Collections.Generic;
using System.Numerics;
using Nova2D.Engine.Core;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.UI
{
    /// <summary>
    /// A UI canvas holds and manages a collection of UI elements.
    /// It provides centralized update and render calls.
    /// </summary>
    public class UICanvas
    {
        private readonly List<UIElement> _elements = new();

        /// <summary>
        /// Adds a UI element to the canvas.
        /// </summary>
        public void Add(UIElement element)
        {
            _elements.Add(element);
        }

        /// <summary>
        /// Updates all UI elements based on input state.
        /// </summary>
        public void Update(Vector2 mousePosition, bool isMousePressed)
        {
            foreach (var element in _elements)
            {
                if (element.Enabled)
                    element.Update(mousePosition, isMousePressed);
            }
        }

        /// <summary>
        /// Renders all visible UI elements.
        /// </summary>
        public void Render(SpriteBatch2D batch)
        {
            batch.Begin();
            
            foreach (var element in _elements)
            {
                if (element.Visible)
                    element.Render(batch);
            }
            
            batch.End();
        }
    }
}