using System;
using System.Runtime.InteropServices;
using System.Text;
using static SDL2.SDL;

namespace OpenGLLLLLLLL.Slim
{
	public static partial class GL
	{
		static T _<T>() where T : class
		{
			var method = "gl" + typeof(T).Name;
			var ptr = SDL_GL_GetProcAddress(method);
			if (ptr == IntPtr.Zero)
				throw new Exception($"nogo: {method} from {typeof(T).Name}");
			return Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
		}

		/// <summary>
		/// Alternate delegate fetcher for when our delegate Type ends in "Del". These happen when the method needs a wrapper
		/// in GL.Utils.
		/// </summary>
		static T _Del<T>() where T : class
		{
			var method = "gl" + typeof(T).Name.Substring(0, typeof(T).Name.Length - 3);
			var ptr = SDL_GL_GetProcAddress(method);
			if (ptr == IntPtr.Zero)
				throw new Exception($"nogo: {method} from {typeof(T).Name}");
			return Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
		}


		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate IntPtr GetString(StringName pname);
		private static GetString _GetString = _<GetString>();
		public static unsafe string glGetString(StringName pname) => new string((sbyte*)_GetString(pname));

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GenBuffers(int n, [Out] uint[] buffers);
		public static GenBuffers glGenBuffers = _<GenBuffers>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Viewport(int x, int y, int width, int height);
		public static Viewport glViewport = _<Viewport>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ClearColor(float r, float g, float b, float a);
		public static ClearColor glClearColor = _<ClearColor>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Clear(ClearBufferMask mask);
		public static Clear glClear = _<Clear>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Enable(EnableCap cap);
		internal static Enable glEnable = _<Enable>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void Disable(EnableCap cap);
		internal static Disable glDisable = _<Disable>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendEquation(BlendEquationMode mode);
		internal static BlendEquation glBlendEquation = _<BlendEquation>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
		internal static BlendFunc glBlendFunc = _<BlendFunc>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void UseProgram(uint program);
		public static UseProgram glUseProgram = _<UseProgram>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderiv(UInt32 shader, ShaderParameter pname, [Out] int[] @params);
		internal static GetShaderiv glGetShaderiv = _<GetShaderiv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetShaderInfoLogDel(UInt32 shader, Int32 maxLength, [Out] Int32[] length, [Out] StringBuilder infoLog);
		internal static GetShaderInfoLogDel glGetShaderInfoLog = _Del<GetShaderInfoLogDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint CreateShader(ShaderType shaderType);
		internal static CreateShader glCreateShader = _<CreateShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void ShaderSourceDel(UInt32 shader, Int32 count, String[] @string, Int32[] length);
		internal static ShaderSourceDel glShaderSource = _Del<ShaderSourceDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void CompileShader(UInt32 shader);
		internal static CompileShader glCompileShader = _<CompileShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DeleteShader(UInt32 shader);
		internal static DeleteShader glDeleteShader = _<DeleteShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramiv(UInt32 program, ProgramParameter pname, [Out] Int32[] @params);
		internal static GetProgramiv glGetProgramiv = _<GetProgramiv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void GetProgramInfoLogDel(UInt32 program, Int32 maxLength, [OutAttribute] Int32[] length, [OutAttribute] System.Text.StringBuilder infoLog);
		internal static GetProgramInfoLogDel glGetProgramInfoLog = _Del<GetProgramInfoLogDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate UInt32 CreateProgram();
		internal static CreateProgram glCreateProgram = _<CreateProgram>();

		internal delegate void AttachShader(UInt32 program, UInt32 shader);
		internal static AttachShader glAttachShader = _<AttachShader>();

		internal delegate void LinkProgram(UInt32 program);
		internal static LinkProgram glLinkProgram = _<LinkProgram>();

		internal delegate Int32 GetUniformLocation(UInt32 program, String name);
		internal static GetUniformLocation glGetUniformLocation = _<GetUniformLocation>();

		internal delegate Int32 GetAttribLocation(UInt32 program, String name);
		internal static GetAttribLocation glGetAttribLocation = _<GetAttribLocation>();

		internal delegate void DetachShader(UInt32 program, UInt32 shader);
		internal static DetachShader glDetachShader = _<DetachShader>();

		internal delegate void DeleteProgram(UInt32 program);
		internal static DeleteProgram glDeleteProgram = _<DeleteProgram>();

		internal delegate void GetActiveAttrib(UInt32 program, UInt32 index, Int32 bufSize, [Out] Int32[] length, [Out] Int32[] size, [Out] GL.ActiveAttribType[] type, [Out] System.Text.StringBuilder name);
		internal static GetActiveAttrib glGetActiveAttrib = _<GetActiveAttrib>();

		internal delegate void GetActiveUniform(UInt32 program, UInt32 index, Int32 bufSize, [OutAttribute] Int32[] length, [OutAttribute] Int32[] size, [OutAttribute] GL.ActiveUniformType[] type, [OutAttribute] StringBuilder name);
		internal static GetActiveUniform glGetActiveUniform = _<GetActiveUniform>();

		internal delegate void Uniform1f(Int32 location, Single v0);
		internal static Uniform1f glUniform1f = _<Uniform1f>();

		internal delegate void Uniform2f(Int32 location, Single v0, Single v1);
		internal static Uniform2f glUniform2f = _<Uniform2f>();

		internal delegate void Uniform3f(Int32 location, Single v0, Single v1, Single v2);
		internal static Uniform3f glUniform3f = _<Uniform3f>();

		internal delegate void Uniform4f(Int32 location, Single v0, Single v1, Single v2, Single v3);
		internal static Uniform4f glUniform4f = _<Uniform4f>();

		internal delegate void Uniform1i(Int32 location, Int32 v0);
		internal static Uniform1i glUniform1i = _<Uniform1i>();

		internal delegate void Uniform3fv(Int32 location, Int32 count, Single[] value);
		internal static Uniform3fv glUniform3fv = _<Uniform3fv>();

		internal delegate void Uniform4fv(Int32 location, Int32 count, Single[] value);
		internal static Uniform4fv glUniform4fv = _<Uniform4fv>();

		internal delegate void UniformMatrix3fvDel(Int32 location, Int32 count, Boolean transpose, Single[] value);
		internal static UniformMatrix3fvDel glUniformMatrix3fv = _Del<UniformMatrix3fvDel>();

		internal delegate void UniformMatrix4fvDel(Int32 location, Int32 count, Boolean transpose, Single[] value);
		internal static UniformMatrix4fvDel glUniformMatrix4fv = _Del<UniformMatrix4fvDel>();

		internal delegate void BindSampler(UInt32 unit, UInt32 sampler);
		internal static BindSampler glBindSampler = _<BindSampler>();

		internal delegate void BindVertexArray(UInt32 array);
		internal static BindVertexArray glBindVertexArray = _<BindVertexArray>();

		internal delegate void BindBuffer(GL.BufferTarget target, UInt32 buffer);
		internal static BindBuffer glBindBuffer = _<BindBuffer>();

		internal delegate void EnableVertexAttribArrayDel(UInt32 index);
		internal static EnableVertexAttribArrayDel glEnableVertexAttribArray = _Del<EnableVertexAttribArrayDel>();

		internal delegate void DisableVertexAttribArray(UInt32 index);
		internal static DisableVertexAttribArray glDisableVertexAttribArray = _<DisableVertexAttribArray>();

		internal delegate void VertexAttribPointerDel(UInt32 index, Int32 size, VertexAttribPointerType type, Boolean normalized, Int32 stride, IntPtr pointer);
		internal static VertexAttribPointerDel glVertexAttribPointer = _Del<VertexAttribPointerDel>();

		internal delegate void BindTexture(TextureTarget target, UInt32 texture);
		internal static BindTexture glBindTexture = _<BindTexture>();

		internal delegate void BufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage);
		internal static BufferData glBufferData = _<BufferData>();

		internal delegate void Scissor(Int32 x, Int32 y, Int32 width, Int32 height);
		internal static Scissor glScissor = _<Scissor>();

		internal delegate void DrawElementsBaseVertex(BeginMode mode, Int32 count, DrawElementsType type, IntPtr indices, Int32 basevertex);
		internal static DrawElementsBaseVertex glDrawElementsBaseVertex = _<DrawElementsBaseVertex>();

		internal delegate void DeleteVertexArrays(Int32 n, UInt32[] arrays);
		internal static DeleteVertexArrays glDeleteVertexArrays = _<DeleteVertexArrays>();

		internal delegate void GenVertexArrays(Int32 n, [Out] UInt32[] arrays);
		internal static GenVertexArrays glGenVertexArrays = _<GenVertexArrays>();

		internal delegate void GenTextures(Int32 n, [OutAttribute] UInt32[] textures);
		internal static GenTextures glGenTextures = _<GenTextures>();

		internal delegate void PixelStorei(PixelStoreParameter pname, Int32 param);
		internal static PixelStorei glPixelStorei = _<PixelStorei>();

		internal delegate void TexImage2D(TextureTarget target, Int32 level, PixelInternalFormat internalFormat, Int32 width, Int32 height, Int32 border, PixelFormat format, PixelType type, IntPtr data);
		internal static TexImage2D glTexImage2D = _<TexImage2D>();

		internal delegate void TexParameteri(TextureTarget target, TextureParameterName pname, TextureParameter param);
		internal static TexParameteri glTexParameteri = _<TexParameteri>();

		internal delegate void DeleteTextures(Int32 n, UInt32[] textures);
		internal static DeleteTextures glDeleteTextures = _<DeleteTextures>();
	}
}
