using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    public class BitmapFontRenderer
    {
        private readonly BitmapFont _font;

        public BitmapFontRenderer(BitmapFont font)
        {
            _font = font;
        }

        public void DrawText(SpriteBatch2D spriteBatch, string text, Vector2 position, Vector4 color, float scale = 1f)
        {
            Vector2 pos = position;

            foreach (char c in text)
            {
                if (!_font.Glyphs.TryGetValue(c, out var glyph)) continue;

                Vector2 charPos = pos + glyph.Offset * scale;
                Vector2 size = new(glyph.Source.Width * scale, glyph.Source.Height * scale);

                spriteBatch.Draw(
                    _font.Texture,
                    charPos,
                    size,
                    glyph.Source,
                    color,
                    Vector2.Zero,
                    0f
                );

                pos.X += glyph.XAdvance * scale;
            }
        }
    }
}