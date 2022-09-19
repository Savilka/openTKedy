using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace TestApp;

public class Game : GameWindow {
    private int _vertexBufferObject;

    private int _elementBufferObject;

    private int _vertexArrayObject;

    private Shader _shader = null!;

    private readonly float[] _vertices = {
        //Position          Texture coordinates
        0.5f, 0.5f, 0.0f, 1.0f, 1.0f, // top right
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f, 0.5f, 0.0f, 0.0f, 1.0f // top left
    };

    private readonly uint[] _indices = {
        // note that we start from 0!
        0, 1, 3, // first triangle
        1, 2, 3 // second triangle
    };

    public Game(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings { Size = (width, height), Title = title }) {
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape)) {
            Close();
        }
    }

    protected override void OnLoad() {
        base.OnLoad();
        GL.ClearColor(0.2f,
            0.8f,
            0.8f,
            1.0f);

        _vertexBufferObject = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer,
            _vertices.Length * sizeof(float),
            _vertices,
            BufferUsageHint.StaticDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _shader = new Shader(@"../../../shader.vert", @"../../../shader.frag");
        // _shader.SetInt("texture0", 1);
        // _shader.SetInt("texture1", 2);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);


        var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation,
            2,
            VertexAttribPointerType.Float,
            false,
            5 * sizeof(float),
            3 * sizeof(float));


        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            _indices.Length * sizeof(uint),
            _indices,
            BufferUsageHint.StaticDraw);
    }

    protected override void OnRenderFrame(FrameEventArgs e) {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();

        var texture = Texture.LoadFromFile(@"../../../1.bmp");
        var texture1 = Texture.LoadFromFile(@"../../../2.bmp");
        GL.BindVertexArray(_vertexArrayObject);


        texture.Use(TextureUnit.Texture0);
        texture1.Use(TextureUnit.Texture1);
        GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "texture0"), 0);
        GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "texture1"), 1);

        _shader.Use();


        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        //GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 3);

        SwapBuffers();
        base.OnRenderFrame(e);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);

        GL.Viewport(0,
            0,
            e.Width,
            e.Height);
    }

    protected override void OnUnload() {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);

        GL.DeleteProgram(_shader.Handle);

        base.OnUnload();
    }
}