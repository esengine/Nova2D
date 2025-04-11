using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.IO;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Represents a 2D texture loaded from file and uploaded to the GPU.
    /// </summary>
    public unsafe class Texture : IDisposable
    {
        private readonly GL _gl;
        
        /// <summary>
        /// The OpenGL handle of this texture.
        /// </summary>
        public uint Handle { get; }

        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Loads a texture from disk and uploads it to the GPU.
        /// </summary>
        public Texture(GL gl, string path)
        {
            _gl = gl;

            if (!File.Exists(path))
                throw new FileNotFoundException($"Texture file not found: {path}");

            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            Width = image.Width;
            Height = image.Height;

            Handle = _gl.GenTexture();
            _gl.BindTexture(TextureTarget.Texture2D, Handle);

            fixed (byte* dataPtr = image.Data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0,
                    (int)InternalFormat.Rgba,
                    (uint)image.Width,
                    (uint)image.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    dataPtr);
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