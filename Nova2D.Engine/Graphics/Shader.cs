using Silk.NET.OpenGL;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Wraps an OpenGL shader program and provides utility functions for uniform management.
    /// </summary>
    public unsafe class Shader : IDisposable
    {
        private readonly GL _gl;
        
        /// <summary>
        /// The OpenGL handle for the linked shader program.
        /// </summary>
        public uint Handle { get; }
        
        
        /// <summary>
        /// Optional uniform location cache to avoid repeated lookups.
        /// </summary>
        private readonly Dictionary<string, int> _uniformLocationCache = new();

        /// <summary>
        /// Creates a shader program from vertex and fragment shader source files.
        /// </summary>
        /// <param name="gl">OpenGL instance.</param>
        /// <param name="vertexPath">Path to the vertex shader file.</param>
        /// <param name="fragmentPath">Path to the fragment shader file.</param>
        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            _gl = gl;

            var vertexSource = File.ReadAllText(vertexPath);
            var fragmentSource = File.ReadAllText(fragmentPath);

            uint vertex = CompileShader(ShaderType.VertexShader, vertexSource);
            uint fragment = CompileShader(ShaderType.FragmentShader, fragmentSource);

            Handle = _gl.CreateProgram();
            _gl.AttachShader(Handle, vertex);
            _gl.AttachShader(Handle, fragment);
            _gl.LinkProgram(Handle);

            _gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                string info = _gl.GetProgramInfoLog(Handle);
                throw new Exception($"Shader link error:\n{info}");
            }

            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        /// <summary>
        /// Compiles a single shader (vertex or fragment) from source.
        /// </summary>
        private uint CompileShader(ShaderType type, string source)
        {
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);

            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
            if (status == 0)
            {
                string info = _gl.GetShaderInfoLog(shader);
                throw new Exception($"{type} compile error:\n{info}");
            }

            return shader;
        }
        
        /// <summary>
        /// Activates the shader program for rendering.
        /// </summary>
        public void Use() => _gl.UseProgram(Handle);

        /// <summary>
        /// Disposes of the shader and deletes it from the GPU.
        /// </summary>
        public void Dispose() => _gl.DeleteProgram(Handle);

        /// <summary>
        /// Gets (or caches) a uniform location by name.
        /// </summary>
        private int GetUniformLocation(string name)
        {
            if (_uniformLocationCache.TryGetValue(name, out int cached))
                return cached;

            int location = _gl.GetUniformLocation(Handle, name);
            if (location == -1)
                Console.WriteLine($"[Shader] Warning: Uniform '{name}' not found.");

            _uniformLocationCache[name] = location;
            return location;
        }
        
        // Uniform setters for common types
        
        public void SetInt(string name, int value)
        {
            _gl.Uniform1(GetUniformLocation(name), value);
        }

        public void SetFloat(string name, float value)
        {
            _gl.Uniform1(GetUniformLocation(name), value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            _gl.Uniform2(GetUniformLocation(name), value.X, value.Y);
        }

        public void SetVector3(string name, Vector3 value)
        {
            _gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }

        public void SetVector4(string name, Vector4 value)
        {
            _gl.Uniform4(GetUniformLocation(name), value.X, value.Y, value.Z, value.W);
        }

        public void SetMatrix4(string name, Matrix4x4 value, bool transpose = false)
        {
            float* ptr = (float*)Unsafe.AsPointer(ref value);
            _gl.UniformMatrix4(GetUniformLocation(name), 1, transpose, ptr);
        }
    }
}