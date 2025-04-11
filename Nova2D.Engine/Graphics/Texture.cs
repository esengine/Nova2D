using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.IO;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Represents an OpenGL texture, loaded from file via StbImage.
    /// </summary>
    public unsafe class Texture : IDisposable
    {
        private readonly GL _gl;
        public uint Handle { get; }

        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Loads a texture from disk and uploads it to GPU.
        /// </summary>
        /// <param name="gl">The OpenGL context.</param>
        /// <param name="path">Relative or absolute file path to image.</param>
        public Texture(GL gl, string path)
        {
            _gl = gl;

            if (!File.Exists(path))
                throw new FileNotFoundException($"Texture file not found: {path}");

            // Flip image vertically to match OpenGL's bottom-left origin
            StbImage.stbi_set_flip_vertically_on_load(1);
            
            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            Width = image.Width;
            Height = image.Height;

            Handle = _gl.GenTexture();
            _gl.BindTexture(TextureTarget.Texture2D, Handle);

            fixed (byte* dataPtr = image.Data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, level: 0,
                    internalformat: (int)InternalFormat.Rgba,
                    width: (uint)image.Width,
                    height: (uint)image.Height,
                    border: 0,
                    format: PixelFormat.Rgba,
                    type: PixelType.UnsignedByte,
                    pixels: dataPtr);
            }

            // Set default filtering and wrapping
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Binds this texture to the specified texture unit.
        /// </summary>
        public void Bind(TextureUnit unit = TextureUnit.Texture0)
        {
            _gl.ActiveTexture(unit);
            _gl.BindTexture(TextureTarget.Texture2D, Handle);
        }

        /// <summary>
        /// Frees the GPU texture resource.
        /// </summary>
        public void Dispose()
        {
            _gl.DeleteTexture(Handle);
        }
    }
}