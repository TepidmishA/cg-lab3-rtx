using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

public class View
{
    private int vbo_position;
    private int BasicProgramID;
    private int BasicVertexShader;
    private int BasicFragmentShader;

    private int attribute_vpos;
    private Vector3 campos;
    private double aspect;

    private int uniform_pos;
    private int uniform_aspect;

    public void setAspect(double aspect)
    {
        this.aspect = aspect;
    }

    public void InitShaders()
    {
        // Создание шейдерной программы
        BasicProgramID = GL.CreateProgram();

        LoadShader("..\\..\\data\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
        LoadShader("..\\..\\data\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
        GL.LinkProgram(BasicProgramID);

        // Проверяем успех компоновки
        int status = 0;
        GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
        Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
    }

    private void LoadShader(string filename, ShaderType type, int program, out int address)
    {
        address = GL.CreateShader(type);
        using (System.IO.StreamReader sr = new StreamReader(filename))
        {
            GL.ShaderSource(address, sr.ReadToEnd());
        }
        GL.CompileShader(address);
        GL.AttachShader(program, address);
        Console.WriteLine(GL.GetShaderInfoLog(address));
    }

    public void SetupBuffers()
    {
        // Данные вершин квадрата
        Vector3[] vertdata = new Vector3[] {
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f)
        };

        GL.GenBuffers(1, out vbo_position);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.Uniform3(uniform_pos, campos);
        GL.Uniform1(uniform_aspect, aspect);
        GL.UseProgram(BasicProgramID);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetupScene(GLControl glControl)
    {
        campos = new Vector3(0, 0, 3);
        aspect = (float)glControl.Width / glControl.Height;

        attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
        uniform_pos = GL.GetUniformLocation(BasicProgramID, "campos");
        uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");

        SetupBuffers();
    }

    public void Render(GLControl glControl)
    {
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(BasicProgramID);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
        GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(attribute_vpos);

        GL.Uniform3(uniform_pos, campos);
        GL.Uniform1(uniform_aspect, aspect);

        GL.DrawArrays(PrimitiveType.Quads, 0, 4);
        GL.DisableVertexAttribArray(attribute_vpos);

        glControl.SwapBuffers();
    }
}

