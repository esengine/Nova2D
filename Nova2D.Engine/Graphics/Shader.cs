using Silk.NET.OpenGL;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nova2D.Engine.Graphics
{
    /// <summary>
    /// Wraps an OpenGL shader program, provides uniform setters.
    /// </summary>
    public unsafe class Shader : IDisposable
    {
        private readonly GL _gl;
        public uint Handle { get; }

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
        
        public void Use() => _gl.UseProgram(Handle);

        public void Dispose() => _gl.DeleteProgram(Handle);

        // Uniform setters (overloads for various types)
        
        public void SetInt(string name, int value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            _gl.Uniform1(location, value);
        }
        
        public void SetFloat(string name, float value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            _gl.Uniform1(location, value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            _gl.Uniform2(location, value.X, value.Y);
        }

        public void SetVector3(string name, Vector3 value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            _gl.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void SetVector4(string name, Vector4 value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        public void SetMatrix4(string name, Matrix4x4 value)
        {
            int location = _gl.GetUniformLocation(Handle, name);
            float* ptr = (float*)Unsafe.AsPointer(ref value);
            _gl.UniformMatrix4(location, 1, transpose: false, ptr);
        }
    }
}