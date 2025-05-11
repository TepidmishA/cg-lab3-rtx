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

    private int uniform_maxRayDepth;
    private int maxRayDepth;

    private int[] uniform_materialColors;
    private int[] uniform_materialLightCoeffs;
    private int[] uniform_materialReflection;
    private int[] uniform_materialRefraction;
    private int[] uniform_materialType;

    private Vector3[] materialColors;
    private Vector4[] materialLightCoeffs;
    private float[] materialReflections;
    private float[] materialRefractions;
    private int[] materialTypes;

    private const int TOTAL_MATERIALS = 7;

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
        uniform_maxRayDepth = GL.GetUniformLocation(BasicProgramID, "maxRayDepth");

        uniform_materialColors = new int[TOTAL_MATERIALS];
        uniform_materialLightCoeffs = new int[TOTAL_MATERIALS];
        uniform_materialReflection = new int[TOTAL_MATERIALS];
        uniform_materialRefraction = new int[TOTAL_MATERIALS];
        uniform_materialType = new int[TOTAL_MATERIALS];

        materialColors = new Vector3[TOTAL_MATERIALS];
        materialLightCoeffs = new Vector4[TOTAL_MATERIALS];
        materialReflections = new float[TOTAL_MATERIALS];
        materialRefractions = new float[TOTAL_MATERIALS];
        materialTypes = new int[TOTAL_MATERIALS];

        for (int i = 0; i < TOTAL_MATERIALS; i++)
        {
            uniform_materialColors[i] = GL.GetUniformLocation(BasicProgramID, $"uMaterialColor[{i}]");
            uniform_materialLightCoeffs[i] = GL.GetUniformLocation(BasicProgramID, $"uMaterialLightCoeffs[{i}]");
            uniform_materialReflection[i] = GL.GetUniformLocation(BasicProgramID, $"uMaterialReflection[{i}]");
            uniform_materialRefraction[i] = GL.GetUniformLocation(BasicProgramID, $"uMaterialRefraction[{i}]");
            uniform_materialType[i] = GL.GetUniformLocation(BasicProgramID, $"uMaterialType[{i}]");
        }

        SetupMaterials();
        SetupBuffers();

        GL.Uniform1(uniform_maxRayDepth, 16);
        maxRayDepth = 16;
    }

    private void SetupMaterials()
    {
        GL.UseProgram(BasicProgramID);

        Vector3[] colors = new Vector3[]
        {
            new Vector3(0.0f, 1.0f, 0.0f), // Green
            new Vector3(0.0f, 0.0f, 1.0f), // Blue
            new Vector3(1.0f, 0.0f, 0.0f), // Red
            new Vector3(1.0f, 1.0f, 1.0f), // White
            new Vector3(0.9f, 0.9f, 1.0f), // Mirror
            new Vector3(0.9f, 0.9f, 1.0f), // Glass
            new Vector3(0.9f, 0.9f, 1.0f)  // Glass for cube
        };

        Vector4[] lightCoeffs = new Vector4[]
        {
            new Vector4(0.4f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.4f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.4f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.55f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.4f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.4f, 0.9f, 0.0f, 512.0f),
            new Vector4(0.4f, 0.9f, 0.8f, 256.0f)
        };

        float[] reflections = new float[]
        {
            0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.1f, 0.1f
        };

        float[] refractions = new float[]
        {
            1.0f, 1.0f, 1.0f, 1.0f, 1.5f, 2.5f, 1.3f
        };

        int[] types = new int[]
        {
            1, 1, 1, 1, 2, 3, 3 // DIFFUSE_REFLECTION, MIRROR_REFLECTION, REFRACTION
        };

        for (int i = 0; i < TOTAL_MATERIALS; i++)
        {
            materialColors[i] = colors[i];
            materialLightCoeffs[i] = lightCoeffs[i];
            materialReflections[i] = reflections[i];
            materialRefractions[i] = refractions[i];
            materialTypes[i] = types[i];

            GL.Uniform3(uniform_materialColors[i], colors[i]);
            GL.Uniform4(uniform_materialLightCoeffs[i], lightCoeffs[i]);
            GL.Uniform1(uniform_materialReflection[i], reflections[i]);
            GL.Uniform1(uniform_materialRefraction[i], refractions[i]);
            GL.Uniform1(uniform_materialType[i], types[i]);
        }
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

    public void UpdateMaterialColor(int index, Vector3 color)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform3(uniform_materialColors[index], color);
        if (index >= 0 && index < TOTAL_MATERIALS)
            materialColors[index] = color;
    }

    public void UpdateMaterialLightCoeffs(int index, Vector4 lightCoeffs)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform4(uniform_materialLightCoeffs[index], lightCoeffs);
        if (index >= 0 && index < TOTAL_MATERIALS)
            materialLightCoeffs[index] = lightCoeffs;
    }

    public void UpdateMaterialReflection(int index, float reflection)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform1(uniform_materialReflection[index], reflection);
        if (index >= 0 && index < TOTAL_MATERIALS)
            materialReflections[index] = reflection;
    }

    public void UpdateMaterialRefraction(int index, float refraction)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform1(uniform_materialRefraction[index], refraction);
        if (index >= 0 && index < TOTAL_MATERIALS)
            materialRefractions[index] = refraction;
    }

    public void UpdateMaterialType(int index, int type)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform1(uniform_materialType[index], type);
        if (index >= 0 && index < TOTAL_MATERIALS)
            materialTypes[index] = type;
    }

    public void UpdateMaxRayDepth(int _maxRayDepth)
    {
        GL.UseProgram(BasicProgramID);
        GL.Uniform1(uniform_maxRayDepth, _maxRayDepth);
        maxRayDepth = _maxRayDepth;
    }

    public int getMaxRayDepth()
    {
        return maxRayDepth;
    }

    public int getTotalMaterials()
    {
        return TOTAL_MATERIALS;
    }

    public Vector3 GetMaterialColor(int index)
    {
        if (index >= 0 && index < TOTAL_MATERIALS)
            return materialColors[index];
        throw new IndexOutOfRangeException("Index out of range for materialColors");
    }

    public Vector4 GetMaterialLightCoeffs(int index)
    {
        if (index >= 0 && index < TOTAL_MATERIALS)
            return materialLightCoeffs[index];
        throw new IndexOutOfRangeException("Index out of range for materialLightCoeffs");
    }

    public float GetMaterialReflection(int index)
    {
        if (index >= 0 && index < TOTAL_MATERIALS)
            return materialReflections[index];
        throw new IndexOutOfRangeException("Index out of range for materialReflections");
    }

    public float GetMaterialRefraction(int index)
    {
        if (index >= 0 && index < TOTAL_MATERIALS)
            return materialRefractions[index];
        throw new IndexOutOfRangeException("Index out of range for materialRefractions");
    }

    public int GetMaterialType(int index)
    {
        if (index >= 0 && index < TOTAL_MATERIALS)
            return materialTypes[index];
        throw new IndexOutOfRangeException("Index out of range for materialTypes");
    }
}

