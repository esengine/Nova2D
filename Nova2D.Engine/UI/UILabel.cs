using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.UI
{
    /// <summary>
    /// A static text label for UI, rendered using a bitmap font.
    /// </summary>
    public class UILabel : UIElement
    {
        public string Text { get; set; } = string.Empty;
        public Vector4 Color { get; set; } = Vector4.One;
        public BitmapFont Font { get; }

        public UILabel(string text, BitmapFont font)
        {
            Text = text;
            Font = font;
        }

        public override void Update(Vector2 mousePosition, bool isMousePressed)
        {
            // UILabel is not interactive.
        }

        public override void Render(SpriteBatch2D batch)
        {
            if (!Visible) return;

            var renderer = new BitmapFontRenderer(Font);
            renderer.DrawText(batch, Text, Position, Color);
        }
    }
}