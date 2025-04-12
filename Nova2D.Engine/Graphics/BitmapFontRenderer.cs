using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
    
    /// <summary>
    /// A utility renderer for drawing bitmap font text using SpriteBatch2D.
    /// This class assumes the caller manages SpriteBatch.Begin/End and texture consistency.
    /// </summary>
    public class BitmapFontRenderer
    {
        public BitmapFont Font => _font;
        
        private readonly BitmapFont _font;

        public BitmapFontRenderer(BitmapFont font)
        {
            _font = font ?? throw new ArgumentNullException(nameof(font));
        }

        /// <summary>
        /// Draws multi-line, optionally wrapped text.
        /// </summary>
        public void DrawText(
            SpriteBatch2D? spriteBatch,
            string text,
            Vector2 position,
            Vector4 color,
            float scale = 1f,
            float maxWidth = float.MaxValue,
            TextAlignment alignment = TextAlignment.Left)
        {
            if (spriteBatch == null || string.IsNullOrEmpty(text)) return;
            
            var lines = WrapText(text, maxWidth, scale);
            float lineHeight = _font.LineHeight * scale;
            Vector2 drawPos = position;

            foreach (var line in lines)
            {
                float lineWidth = MeasureLineWidth(line, scale);
                float offsetX = alignment switch
                {
                    TextAlignment.Center => (maxWidth - lineWidth) * 0.5f,
                    TextAlignment.Right => (maxWidth - lineWidth),
                    _ => 0f
                };

                Vector2 pos = drawPos + new Vector2(offsetX, 0);

                foreach (char c in line)
                {
                    if (_font.Glyphs.TryGetValue(c, out var glyph))
                    {
                        DrawGlyph(spriteBatch, glyph, pos, color, scale);
                        pos.X += glyph.XAdvance * scale;
                    }
                }

                drawPos.Y += lineHeight;
            }
        }
        
        /// <summary>
        /// Renders a single glyph.
        /// </summary>
        private void DrawGlyph(SpriteBatch2D spriteBatch, BitmapFont.Glyph glyph, Vector2 position, Vector4 color, float scale)
        {
            Vector2 size = new(glyph.Source.Width * scale, glyph.Source.Height * scale);
            Vector2 offset = glyph.Offset * scale;

            spriteBatch.Draw(
                _font.Texture,
                position + offset,
                size,
                glyph.Source,
                color,
                Vector2.Zero,
                0f
            );
        }
        
        /// <summary>
        /// Measures the full size of wrapped text.
        /// </summary>
        public Vector2 MeasureText(string text, float scale = 1f, float maxWidth = float.MaxValue)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;
            
            var lines = WrapText(text, maxWidth, scale);
            float width = lines.Max(line => MeasureLineWidth(line, scale));
            float height = lines.Count * _font.LineHeight * scale;

            return new Vector2(width, height);
        }

        /// <summary>
        /// Measures the width of a single line of text.
        /// </summary>
        public float MeasureLineWidth(string line, float scale = 1f)
        {
            if (string.IsNullOrEmpty(line)) return 0f;

            float width = 0f;
            foreach (char c in line)
            {
                if (_font.Glyphs.TryGetValue(c, out var glyph))
                    width += glyph.XAdvance * scale;
            }
            return width;
        }

        /// <summary>
        /// Wraps raw text into lines based on maxWidth and font metrics.
        /// </summary>
        private List<string> WrapText(string text, float maxWidth, float scale)
        {
            var lines = new List<string>();

            foreach (var rawLine in text.Split('\n'))
            {
                var words = rawLine.Split(' ');
                string currentLine = string.Empty;
                float currentWidth = 0f;

                foreach (var word in words)
                {
                    var testWord = word + " ";
                    float wordWidth = MeasureLineWidth(testWord, scale);

                    if (currentWidth + wordWidth > maxWidth && currentLine.Length > 0)
                    {
                        lines.Add(currentLine.TrimEnd());
                        currentLine = string.Empty;
                        currentWidth = 0f;
                    }

                    currentLine += testWord;
                    currentWidth += wordWidth;
                }

                if (!string.IsNullOrWhiteSpace(currentLine))
                    lines.Add(currentLine.TrimEnd());
            }

            return lines;
        }
    }
}