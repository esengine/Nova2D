using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Nova2D.Engine.Graphics
{
    public class BitmapFont
    {
        public Texture Texture { get; }
        public Dictionary<char, Glyph> Glyphs { get; } = new();

        public struct Glyph
        {
            public Rectangle Source;
            public Vector2 Offset;
            public float XAdvance;
        }

        public float LineHeight { get; private set; }

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
    }

    public struct Rectangle
    {
        public float X, Y, Width, Height;
        public Rectangle(float x, float y, float w, float h)
        {
            X = x; Y = y; Width = w; Height = h;
        }
    }
}
