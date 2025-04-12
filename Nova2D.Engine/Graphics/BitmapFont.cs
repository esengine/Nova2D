using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Represents a bitmap font loaded from a .fnt file and corresponding texture.
    /// Supports character lookup and string measurement.
    /// </summary>
    public class BitmapFont
    {
        /// <summary>
        /// The texture atlas containing the glyphs.
        /// </summary>
        public Texture Texture { get; }
        
        /// <summary>
        /// Dictionary mapping characters to glyph information.
        /// </summary>
        public Dictionary<char, Glyph> Glyphs { get; } = new();
        
        /// <summary>
        /// The height of a single line of text.
        /// </summary>
        public float LineHeight { get; private set; }

        /// <summary>
        /// Constructs a new bitmap font from a font texture and .fnt metadata file.
        /// </summary>
        /// <param name="texture">The texture containing the font glyphs.</param>
        /// <param name="fntPath">Path to the .fnt file exported by BMFont or similar tool.</param>
        public BitmapFont(Texture texture, string fntPath)
        {
            Texture = texture;

            foreach (var line in File.ReadAllLines(fntPath))
            {
                if (line.StartsWith("char id="))
                {
                    var parts = line.Split(' ');
                    int id = int.Parse(Get(parts, "id"));
                    float x = float.Parse(Get(parts, "x"));
                    float y = float.Parse(Get(parts, "y"));
                    float w = float.Parse(Get(parts, "width"));
                    float h = float.Parse(Get(parts, "height"));
                    float ox = float.Parse(Get(parts, "xoffset"));
                    float oy = float.Parse(Get(parts, "yoffset"));
                    float xa = float.Parse(Get(parts, "xadvance"));

                    Glyphs[(char)id] = new Glyph
                    {
                        Source = new Rectangle(x, y, w, h),
                        Offset = new Vector2(ox, oy),
                        XAdvance = xa
                    };
                }
                else if (line.StartsWith("common "))
                {
                    LineHeight = float.Parse(Get(line.Split(' '), "lineHeight"));
                }
            }

            static string Get(string[] parts, string key)
            {
                foreach (var part in parts)
                    if (part.StartsWith(key + "="))
                        return part[(key.Length + 1)..];
                return "0";
            }
        }
        
        /// <summary>
        /// Measures the total width and height of a string rendered with this font.
        /// </summary>
        /// <param name="text">The string to measure.</param>
        /// <returns>The size of the string in pixels.</returns>
        public Vector2 MeasureString(string text)
        {
            float width = 0f;
            float maxHeight = LineHeight;

            foreach (char c in text)
            {
                if (Glyphs.TryGetValue(c, out var glyph))
                {
                    width += glyph.XAdvance;
                    float glyphHeight = glyph.Source.Height + glyph.Offset.Y;
                    if (glyphHeight > maxHeight)
                        maxHeight = glyphHeight;
                }
            }

            return new Vector2(width, maxHeight);
        }
        
        /// <summary>
        /// Represents a single glyph/character inside the font.
        /// </summary>
        public struct Glyph
        {
            public Rectangle Source; // Texture rectangle
            public Vector2 Offset;   // Position offset when rendering
            public float XAdvance;   // Distance to advance to next glyph
        }
    }

    /// <summary>
    /// A rectangle in 2D space, typically used for texture regions.
    /// </summary>
    public struct Rectangle
    {
        public float X, Y, Width, Height;
        public Rectangle(float x, float y, float w, float h)
        {
            X = x; Y = y; Width = w; Height = h;
        }
    }
}
