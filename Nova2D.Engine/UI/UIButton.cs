using System;
using System.Numerics;
using Nova2D.Engine.Graphics;

namespace Nova2D.Engine.UI
{
    /// <summary>
    /// A basic clickable button element. Renders a background and text, and invokes a callback on click.
    /// </summary>
    public class UIButton : UIElement
    {
        public string Text { get; set; } = string.Empty;
        public BitmapFont Font { get; set; }
        public Vector4 BackgroundColor { get; set; } = new(0.2f, 0.2f, 0.2f, 1.0f);
        public Vector4 TextColor { get; set; } = Vector4.One;
        public Action? OnClick { get; set; }

        private bool _wasPressedLastFrame = false;

        public UIButton(string text, BitmapFont font)
        {
            Text = text;
            Font = font;
            Size = new Vector2(120, 40);
        }

        public override void Update(Vector2 mousePosition, bool isMousePressed)
        {
            if (!Enabled) return;

            bool isHovering = mousePosition.X >= Position.X && mousePosition.X <= Position.X + Size.X &&
                              mousePosition.Y >= Position.Y && mousePosition.Y <= Position.Y + Size.Y;

            if (isHovering && isMousePressed && !_wasPressedLastFrame)
            {
                OnClick?.Invoke();
            }

            _wasPressedLastFrame = isMousePressed;
        }

        public override void Render(SpriteBatch2D batch)
        {
            if (!Visible) return;

            // Draw button background (solid color quad using white texture)
            batch.Draw(Texture.WhiteTexture, Position, Size, BackgroundColor);

            // Center text inside button
            var textSize = Font.MeasureString(Text);
            var textPos = Position + (Size - textSize) * 0.5f;

            var fontRenderer = new BitmapFontRenderer(Font);
            fontRenderer.DrawText(batch, Text, textPos, TextColor);
        }
    }
}