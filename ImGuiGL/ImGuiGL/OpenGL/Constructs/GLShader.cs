using System;
using static SDLImGuiGL.GL;

namespace SDLImGuiGL
{
	public sealed class GLShader : IDisposable
	{
		/// <summary>
		/// Specifies the OpenGL ShaderID.
		/// </summary>
		public uint ShaderID { get; private set; }

		/// <summary>
		/// Specifies the type of shader.
		/// </summary>
		public ShaderType ShaderType { get; private set; }

		/// <summary>
		/// Returns Gl.GetShaderInfoLog(ShaderID), which contains any compilation errors.
		/// </summary>
		public string ShaderLog => GetShaderInfoLog(ShaderID);

		public GLShader(string source, ShaderType type)
		{
			ShaderType = type;
			ShaderID = glCreateShader(type);

			ShaderSource(ShaderID, source);
			glCompileShader(ShaderID);

			if (!GetShaderCompileStatus(ShaderID))
				throw new Exception(ShaderLog);
		}

		~GLShader() => Dispose(false);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (ShaderID != 0)
			{
				glDeleteShader(ShaderID);
				ShaderID = 0;
			}
		}
	}
}
