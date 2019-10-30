using System;

namespace SDL.ImGuiRenderer
{
	public sealed class Shader : IDisposable
	{
		/// <summary>
		/// Specifies the OpenGL ShaderID.
		/// </summary>
		public uint ShaderID { get; private set; }

		/// <summary>
		/// Specifies the type of shader.
		/// </summary>
		public GL.ShaderType ShaderType { get; private set; }

		/// <summary>
		/// Returns Gl.GetShaderInfoLog(ShaderID), which contains any compilation errors.
		/// </summary>
		public string ShaderLog => GL.GetShaderInfoLog(ShaderID);

		public Shader(string source, GL.ShaderType type)
		{
			ShaderType = type;
			ShaderID = GL.glCreateShader(type);

			GL.ShaderSource(ShaderID, source);
			GL.glCompileShader(ShaderID);

			if (!GL.GetShaderCompileStatus(ShaderID))
				throw new Exception(ShaderLog);
		}

		~Shader()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (ShaderID != 0)
			{
				GL.glDeleteShader(ShaderID);
				ShaderID = 0;
			}
		}
	}
}
