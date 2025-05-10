using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

public class View
{
    private int vbo_position;
    private int BasicProgramID;
    private int BasicVertexShader;
    private int BasicFragmentShader;

    public void InitShaders()
    {
        // Создание шейдерной программы
        BasicProgramID = GL.CreateProgram();

        // Загрузка и компиляция шейдеров
        LoadShader("..\\data\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
        LoadShader("..\\data\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);

        // Линковка программы
        GL.LinkProgram(BasicProgramID);

        // Проверка ошибок
        Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
    }

    private void LoadShader(string filename, ShaderType type, int program, out int address)
    {
        address = GL.CreateShader(type);
        string code = File.ReadAllText(filename);
        GL.ShaderSource(address, code);
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

        // Создание и привязка VBO
        GL.GenBuffers(1, out vbo_position);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);

        // Указание атрибутов
        int attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
        GL.EnableVertexAttribArray(attribute_vpos);
        GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
    }

    public void Draw()
    {
        GL.UseProgram(BasicProgramID);
        GL.DrawArrays(PrimitiveType.Quads, 0, 4);
    }
}