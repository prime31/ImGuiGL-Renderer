using System;
using System.Text;

namespace OpenGLLLLLLLL.Slim
{
	public static partial class GL
	{
		static uint[] uint1 = new uint[1];
		static int[] int1 = new int[1];
		static float[] matrix4Float = new float[16];
		static float[] matrix3Float = new float[9];

		public static uint GenBuffer()
		{
			uint1[0] = 0;
			glGenBuffers(1, uint1);
			return uint1[0];
		}

		public static string GetShaderInfoLog(UInt32 shader)
		{
			glGetShaderiv(shader, ShaderParameter.InfoLogLength, int1);
			if (int1[0] == 0)
				return string.Empty;

			var sb = new StringBuilder(int1[0]);
			glGetShaderInfoLog(shader, sb.Capacity, int1, sb);
			return sb.ToString();
		}

		public static void ShaderSource(uint shader, string source)
		{
			int1[0] = source.Length;
			glShaderSource(shader, 1, new string[] { source }, int1);
		}

		public static bool GetShaderCompileStatus(UInt32 shader)
		{
			glGetShaderiv(shader, ShaderParameter.CompileStatus, int1);
			return int1[0] == 1;
		}

		public static string GetProgramInfoLog(UInt32 program)
		{
			glGetProgramiv(program, ProgramParameter.InfoLogLength, int1);
			if (int1[0] == 0)
				return string.Empty;

			var sb = new StringBuilder(int1[0]);
			glGetProgramInfoLog(program, sb.Capacity, int1, sb);
			return sb.ToString();
		}

		public static bool GetProgramLinkStatus(UInt32 program)
		{
			glGetProgramiv(program, ProgramParameter.LinkStatus, int1);
			return int1[0] == 1;
		}

		public static void UniformMatrix4fv(int location, Matrix4 param)
		{
			// use the statically allocated float[] for setting the uniform
			matrix4Float[0] = param[0].X; matrix4Float[1] = param[0].Y; matrix4Float[2] = param[0].Z; matrix4Float[3] = param[0].W;
			matrix4Float[4] = param[1].X; matrix4Float[5] = param[1].Y; matrix4Float[6] = param[1].Z; matrix4Float[7] = param[1].W;
			matrix4Float[8] = param[2].X; matrix4Float[9] = param[2].Y; matrix4Float[10] = param[2].Z; matrix4Float[11] = param[2].W;
			matrix4Float[12] = param[3].X; matrix4Float[13] = param[3].Y; matrix4Float[14] = param[3].Z; matrix4Float[15] = param[3].W;

			glUniformMatrix4fv(location, 1, false, matrix4Float);
		}

		public static void UniformMatrix3fv(int location, Matrix3 param)
		{
			// use the statically allocated float[] for setting the uniform
			matrix3Float[0] = param[0].X; matrix3Float[1] = param[0].Y; matrix3Float[2] = param[0].Z;
			matrix3Float[3] = param[1].X; matrix3Float[4] = param[1].Y; matrix3Float[5] = param[1].Z;
			matrix3Float[6] = param[2].X; matrix3Float[7] = param[2].Y; matrix3Float[8] = param[2].Z;

			glUniformMatrix3fv(location, 1, false, matrix3Float);
		}

		public static void VertexAttribPointer(Int32 index, Int32 size, VertexAttribPointerType type, Boolean normalized, Int32 stride, IntPtr pointer)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));
			glVertexAttribPointer((UInt32)index, size, type, normalized, stride, pointer);
		}

		public static void EnableVertexAttribArray(Int32 index)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(nameof(index));
			glEnableVertexAttribArray((UInt32)index);
		}

		public static uint GenVertexArray()
		{
			uint1[0] = 0;
			glGenVertexArrays(1, uint1);
			return uint1[0];
		}

		public static void DeleteVertexArray(uint vao)
		{
			uint1[0] = vao;
			glDeleteVertexArrays(1, uint1);
		}

		public static uint GenTexture()
		{
			uint1[0] = 0;
			glGenTextures(1, uint1);
			return uint1[0];
		}

		public static void DeleteTexture(uint texture)
		{
			uint1[0] = texture;
			glDeleteTextures(1, uint1);
		}
	}
}
