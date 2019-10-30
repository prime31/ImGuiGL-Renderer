using System;
using System.Numerics;

namespace OpenGLLLLLLLL.Slim
{
	public enum ParamType
	{
		Uniform,
		Attribute
	}

	public class ShaderProgramParam
	{
		/// <summary>
		/// Specifies the C# equivalent of the GLSL data type.
		/// </summary>
		public Type Type;

		/// <summary>
		/// Specifies the location of the parameter in the OpenGL program.
		/// </summary>
		public int Location;

		/// <summary>
		/// Specifies the OpenGL program ID.
		/// </summary>
		public uint Program;

		/// <summary>
		/// Specifies the parameter type (either attribute or uniform).
		/// </summary>
		public ParamType ParamType;

		/// <summary>
		/// Specifies the case-sensitive name of the parameter.
		/// </summary>
		public string Name;

		public uint ProgramId;

		public ShaderProgramParam(Type type, ParamType paramType, string name)
		{
			Type = type;
			ParamType = paramType;
			Name = name;
		}

		public ShaderProgramParam(Type type, ParamType paramType, string name, uint program, int location) : this(type, paramType, name)
		{
			ProgramId = Program;
			Location = location;
		}

		/// <summary>
		/// Gets the location of the parameter in a compiled OpenGL program.
		/// </summary>
		/// <param name="Program">Specifies the shader program that contains this parameter.</param>
		public void GetLocation(ShaderProgram Program)
		{
			Program.Use();
			if (ProgramId == 0)
			{
				ProgramId = Program.ProgramID;
				Location = (ParamType == ParamType.Uniform ? Program.GetUniformLocation(Name) : Program.GetAttributeLocation(Name));
			}
		}

		public void SetValue(bool param)
		{
			if (Type != typeof(bool))
				throw new Exception(string.Format("SetValue({0}) was given a bool.", Type));
			GL.glUniform1i(Location, (param) ? 1 : 0);
		}

		public void SetValue(int param)
		{
			GL.glUniform1i(Location, param);
		}

		public void SetValue(float param)
		{
			if (Type != typeof(float))
				throw new Exception(string.Format("SetValue({0}) was given a float.", Type));
			GL.glUniform1f(Location, param);
		}

		public void SetValue(Vector2 param)
		{
			if (Type != typeof(Vector2))
				throw new Exception(string.Format("SetValue({0}) was given a Vector2.", Type));
			GL.glUniform2f(Location, param.X, param.Y);
		}

		public void SetValue(Vector3 param)
		{
			if (Type != typeof(Vector3))
				throw new Exception(string.Format("SetValue({0}) was given a Vector3.", Type));
			GL.glUniform3f(Location, param.X, param.Y, param.Z);
		}

		public void SetValue(Vector4 param)
		{
			if (Type != typeof(Vector4))
				throw new Exception(string.Format("SetValue({0}) was given a Vector4.", Type));
			GL.glUniform4f(Location, param.X, param.Y, param.Z, param.W);
		}

		public void SetValue(Matrix3 param)
		{
			if (Type != typeof(Matrix3))
				throw new Exception(string.Format("SetValue({0}) was given a Matrix3.", Type));

			GL.UniformMatrix3fv(Location, param);
		}

		public void SetValue(Matrix4 param)
		{
			if (Type != typeof(Matrix4))
				throw new Exception(string.Format("SetValue({0}) was given a Matrix4.", Type));

			GL.UniformMatrix4fv(Location, param);
		}

		public void SetValue(float[] param)
		{
			if (param.Length == 16)
			{
				if (Type != typeof(Matrix4))
					throw new Exception(string.Format("SetValue({0}) was given a Matrix4.", Type));
				GL.glUniformMatrix4fv(Location, 1, false, param);
			}
			else if (param.Length == 9)
			{
				if (Type != typeof(Matrix3))
					throw new Exception(string.Format("SetValue({0}) was given a Matrix3.", Type));
				GL.glUniformMatrix3fv(Location, 1, false, param);
			}
			else if (param.Length == 4)
			{
				if (Type != typeof(Vector4))
					throw new Exception(string.Format("SetValue({0}) was given a Vector4.", Type));
				GL.glUniform4f(Location, param[0], param[1], param[2], param[3]);
			}
			else if (param.Length == 3)
			{
				if (Type != typeof(Vector3))
					throw new Exception(string.Format("SetValue({0}) was given a Vector3.", Type));
				GL.glUniform3f(Location, param[0], param[1], param[2]);
			}
			else if (param.Length == 2)
			{
				if (Type != typeof(Vector2))
					throw new Exception(string.Format("SetValue({0}) was given a Vector2.", Type));
				GL.glUniform2f(Location, param[0], param[1]);
			}
			else if (param.Length == 1)
			{
				if (Type != typeof(float))
					throw new Exception(string.Format("SetValue({0}) was given a float.", Type));
				GL.glUniform1f(Location, param[0]);
			}
			else
			{
				throw new ArgumentException("param was an unexpected length.", nameof(param));
			}
		}
	}
}
