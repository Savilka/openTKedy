using System.Net.Mime;
using OpenTK.Graphics.OpenGL4;


namespace TestApp;

public sealed class Shader : IDisposable {
    public readonly int Handle;
    private bool _disposedValue;

    public Shader(string vertexPath, string fragmentPath) {
        var vertexShaderSource = File.ReadAllText(vertexPath);
        var fragmentShaderSource = File.ReadAllText(fragmentPath);

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var success);

        if (success == 0) {
            var infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);

        if (success == 0) {
            var infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog);
        }

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);

        if (success == 0) {
            var infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void Use() {
        GL.UseProgram(Handle);
    }
    
    public void SetInt(string name, int value)
    {
        var location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }

    private void Dispose(bool disposing) {
        if (_disposedValue) {
            return;
        }

        GL.DeleteProgram(Handle);
        _disposedValue = true;
    }

    ~Shader() {
        GL.DeleteProgram(Handle);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}