using System;
using System.Collections.Generic;
using System.Numerics;

namespace SDL.ImGuiRenderer
{
	public sealed class ShaderProgram : IDisposable
	{
		/// <summary>
		/// Specifies the OpenGL shader program ID.
		/// </summary>
		public uint ProgramID;

		/// <summary>
		/// Specifies the vertex shader used in this program.
		/// </summary>
		public Shader VertexShader;

		/// <summary>
		/// Specifies the fragment shader used in this program.
		/// </summary>
		public Shader FragmentShader;

		/// <summary>
		/// Specifies whether this program will dispose of the child
		/// vertex/fragment programs when the IDisposable method is called.
		/// </summary>
		public bool DisposeChildren;

		private Dictionary<string, ShaderProgramParam> shaderParams;

		/// <summary>
		/// Queries the shader parameter hashtable to find a matching attribute/uniform.
		/// </summary>
		/// <param name="name">Specifies the case-sensitive name of the shader attribute/uniform.</param>
		/// <returns>The requested attribute/uniform, or null on a failure.</returns>
		public ShaderProgramParam this[string name] => shaderParams.ContainsKey(name) ? shaderParams[name] : null;

		public string ProgramLog => GL.GetProgramInfoLog(ProgramID);

		/// <summary>
		/// Links a vertex and fragment shader together to create a shader program.
		/// </summary>
		/// <param name="vertexShader">Specifies the vertex shader.</param>
		/// <param name="fragmentShader">Specifies the fragment shader.</param>
		public ShaderProgram(Shader vertexShader, Shader fragmentShader)
		{
			VertexShader = vertexShader;
			FragmentShader = fragmentShader;
			ProgramID = GL.glCreateProgram();
			DisposeChildren = false;

			GL.glAttachShader(ProgramID, vertexShader.ShaderID);
			GL.glAttachShader(ProgramID, fragmentShader.ShaderID);
			GL.glLinkProgram(ProgramID);

			//Check whether the program linked successfully.
			//If not then throw an error with the linking error.
			if (!GL.GetProgramLinkStatus(ProgramID))
				throw new Exception(ProgramLog);

			GetParams();
		}

		/// <summary>
		/// Creates two shaders and then links them together to create a shader program.
		/// </summary>
		/// <param name="vertexShaderSource">Specifies the source code of the vertex shader.</param>
		/// <param name="fragmentShaderSource">Specifies the source code of the fragment shader.</param>
		public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
			: this(new Shader(vertexShaderSource, GL.ShaderType.VertexShader), new Shader(fragmentShaderSource, GL.ShaderType.FragmentShader))
		{
			DisposeChildren = true;
		}

		~ShaderProgram()
		{
			Dispose(false);
		}

		/// <summary>
		/// Parses all of the parameters (attributes/uniforms) from the two attached shaders
		/// and then loads their location by passing this shader program into the parameter object.
		/// </summary>
		private void GetParams()
		{
			shaderParams = new Dictionary<string, ShaderProgramParam>();

			var resources = new int[1];
			var actualLength = new int[1];
			var arraySize = new int[1];

			GL.glGetProgramiv(ProgramID, GL.ProgramParameter.ActiveAttributes, resources);

			for (uint i = 0; i < resources[0]; i++)
			{
				var type = new GL.ActiveAttribType[1];
				System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
				GL.glGetActiveAttrib(ProgramID, i, 256, actualLength, arraySize, type, sb);

				if (!shaderParams.ContainsKey(sb.ToString()))
				{
					var param = new ShaderProgramParam(TypeFromAttributeType(type[0]), ParamType.Attribute, sb.ToString());
					shaderParams.Add(param.Name, param);
					param.GetLocation(this);
				}
			}

			GL.glGetProgramiv(ProgramID, GL.ProgramParameter.ActiveUniforms, resources);

			for (uint i = 0; i < resources[0]; i++)
			{
				var type = new GL.ActiveUniformType[1];
				System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
				GL.glGetActiveUniform(ProgramID, i, 256, actualLength, arraySize, type, sb);

				if (!shaderParams.ContainsKey(sb.ToString()))
				{
					var param = new ShaderProgramParam(TypeFromUniformType(type[0]), ParamType.Uniform, sb.ToString());
					shaderParams.Add(param.Name, param);
					param.GetLocation(this);
				}
			}
		}

		private Type TypeFromAttributeType(GL.ActiveAttribType type)
		{
			switch (type)
			{
				case GL.ActiveAttribType.Float: return typeof(float);
				case GL.ActiveAttribType.FloatMat2: return typeof(float[]);
				case GL.ActiveAttribType.FloatMat3: throw new Exception();
				case GL.ActiveAttribType.FloatMat4: return typeof(Matrix4x4);
				case GL.ActiveAttribType.FloatVec2: return typeof(Vector2);
				case GL.ActiveAttribType.FloatVec3: return typeof(Vector3);
				case GL.ActiveAttribType.FloatVec4: return typeof(Vector4);
				default: return typeof(object);
			}
		}

		private Type TypeFromUniformType(GL.ActiveUniformType type)
		{
			switch (type)
			{
				case GL.ActiveUniformType.Int: return typeof(int);
				case GL.ActiveUniformType.Float: return typeof(float);
				case GL.ActiveUniformType.FloatVec2: return typeof(Vector2);
				case GL.ActiveUniformType.FloatVec3: return typeof(Vector3);
				case GL.ActiveUniformType.FloatVec4: return typeof(Vector4);
				case GL.ActiveUniformType.IntVec2: return typeof(int[]);
				case GL.ActiveUniformType.IntVec3: return typeof(int[]);
				case GL.ActiveUniformType.IntVec4: return typeof(int[]);
				case GL.ActiveUniformType.Bool: return typeof(bool);
				case GL.ActiveUniformType.BoolVec2: return typeof(bool[]);
				case GL.ActiveUniformType.BoolVec3: return typeof(bool[]);
				case GL.ActiveUniformType.BoolVec4: return typeof(bool[]);
				case GL.ActiveUniformType.FloatMat2: return typeof(float[]);
				case GL.ActiveUniformType.FloatMat3: throw new Exception();
				case GL.ActiveUniformType.FloatMat4: return typeof(Matrix4x4);
				case GL.ActiveUniformType.Sampler1D:
				case GL.ActiveUniformType.Sampler2D:
				case GL.ActiveUniformType.Sampler3D:
				case GL.ActiveUniformType.SamplerCube:
				case GL.ActiveUniformType.Sampler1DShadow:
				case GL.ActiveUniformType.Sampler2DShadow:
				case GL.ActiveUniformType.Sampler2DRect:
				case GL.ActiveUniformType.Sampler2DRectShadow: return typeof(int);
				case GL.ActiveUniformType.FloatMat2x3:
				case GL.ActiveUniformType.FloatMat2x4:
				case GL.ActiveUniformType.FloatMat3x2:
				case GL.ActiveUniformType.FloatMat3x4:
				case GL.ActiveUniformType.FloatMat4x2:
				case GL.ActiveUniformType.FloatMat4x3: return typeof(float[]);
				case GL.ActiveUniformType.Sampler1DArray:
				case GL.ActiveUniformType.Sampler2DArray:
				case GL.ActiveUniformType.SamplerBuffer:
				case GL.ActiveUniformType.Sampler1DArrayShadow:
				case GL.ActiveUniformType.Sampler2DArrayShadow:
				case GL.ActiveUniformType.SamplerCubeShadow: return typeof(int);
				case GL.ActiveUniformType.UnsignedIntVec2: return typeof(uint[]);
				case GL.ActiveUniformType.UnsignedIntVec3: return typeof(uint[]);
				case GL.ActiveUniformType.UnsignedIntVec4: return typeof(uint[]);
				case GL.ActiveUniformType.IntSampler1D:
				case GL.ActiveUniformType.IntSampler2D:
				case GL.ActiveUniformType.IntSampler3D:
				case GL.ActiveUniformType.IntSamplerCube:
				case GL.ActiveUniformType.IntSampler2DRect:
				case GL.ActiveUniformType.IntSampler1DArray:
				case GL.ActiveUniformType.IntSampler2DArray:
				case GL.ActiveUniformType.IntSamplerBuffer: return typeof(int);
				case GL.ActiveUniformType.UnsignedIntSampler1D:
				case GL.ActiveUniformType.UnsignedIntSampler2D:
				case GL.ActiveUniformType.UnsignedIntSampler3D:
				case GL.ActiveUniformType.UnsignedIntSamplerCube:
				case GL.ActiveUniformType.UnsignedIntSampler2DRect:
				case GL.ActiveUniformType.UnsignedIntSampler1DArray:
				case GL.ActiveUniformType.UnsignedIntSampler2DArray:
				case GL.ActiveUniformType.UnsignedIntSamplerBuffer: return typeof(uint);
				case GL.ActiveUniformType.Sampler2DMultisample: return typeof(int);
				case GL.ActiveUniformType.IntSampler2DMultisample: return typeof(int);
				case GL.ActiveUniformType.UnsignedIntSampler2DMultisample: return typeof(uint);
				case GL.ActiveUniformType.Sampler2DMultisampleArray: return typeof(int);
				case GL.ActiveUniformType.IntSampler2DMultisampleArray: return typeof(int);
				case GL.ActiveUniformType.UnsignedIntSampler2DMultisampleArray: return typeof(uint);
				default: return typeof(object);
			}
		}

		public void Use() => GL.glUseProgram(ProgramID);

		public int GetUniformLocation(string Name)
		{
			Use();
			return GL.glGetUniformLocation(ProgramID, Name);
		}

		public int GetAttributeLocation(string Name)
		{
			Use();
			return GL.glGetAttribLocation(ProgramID, Name);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (ProgramID != 0)
			{
				GL.glUseProgram(0);

				GL.glDetachShader(ProgramID, VertexShader.ShaderID);
				GL.glDetachShader(ProgramID, FragmentShader.ShaderID);
				GL.glDeleteProgram(ProgramID);

				if (DisposeChildren)
				{
					VertexShader.Dispose();
					FragmentShader.Dispose();
				}

				ProgramID = 0;
			}
		}
	}
}
