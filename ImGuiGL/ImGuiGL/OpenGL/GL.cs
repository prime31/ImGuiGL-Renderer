using System;
using System.Runtime.InteropServices;
using System.Text;
using static SDL2.SDL;

namespace SDLImGuiGL
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
		delegate IntPtr GetString(StringName pname);
		static GetString _GetString = _<GetString>();
		public static unsafe string glGetString(StringName pname) => new string((sbyte*)_GetString(pname));

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GenBuffers(int n, [Out] uint[] buffers);
		public static GenBuffers glGenBuffers = _<GenBuffers>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeleteBuffers(Int32 n, UInt32[] buffers);
		public static DeleteBuffers glDeleteBuffers = _<DeleteBuffers>();

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
		public delegate void Enable(EnableCap cap);
		public static Enable glEnable = _<Enable>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Disable(EnableCap cap);
		public static Disable glDisable = _<Disable>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BlendEquation(BlendEquationMode mode);
		public static BlendEquation glBlendEquation = _<BlendEquation>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
		public static BlendFunc glBlendFunc = _<BlendFunc>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void UseProgram(uint program);
		public static UseProgram glUseProgram = _<UseProgram>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetShaderiv(UInt32 shader, ShaderParameter pname, [Out] int[] @params);
		public static GetShaderiv glGetShaderiv = _<GetShaderiv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetShaderInfoLogDel(UInt32 shader, Int32 maxLength, [Out] Int32[] length, [Out] StringBuilder infoLog);
		public static GetShaderInfoLogDel glGetShaderInfoLog = _Del<GetShaderInfoLogDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate uint CreateShader(ShaderType shaderType);
		public static CreateShader glCreateShader = _<CreateShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void ShaderSourceDel(UInt32 shader, Int32 count, String[] @string, Int32[] length);
		public static ShaderSourceDel glShaderSource = _Del<ShaderSourceDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void CompileShader(UInt32 shader);
		public static CompileShader glCompileShader = _<CompileShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeleteShader(UInt32 shader);
		public static DeleteShader glDeleteShader = _<DeleteShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetProgramiv(UInt32 program, ProgramParameter pname, [Out] Int32[] @params);
		public static GetProgramiv glGetProgramiv = _<GetProgramiv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetProgramInfoLogDel(uint program, Int32 maxLength, [OutAttribute] Int32[] length, [OutAttribute] StringBuilder infoLog);
		public static GetProgramInfoLogDel glGetProgramInfoLog = _Del<GetProgramInfoLogDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate UInt32 CreateProgram();
		public static CreateProgram glCreateProgram = _<CreateProgram>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void AttachShader(UInt32 program, UInt32 shader);
		public static AttachShader glAttachShader = _<AttachShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void LinkProgram(UInt32 program);
		public static LinkProgram glLinkProgram = _<LinkProgram>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate Int32 GetUniformLocation(UInt32 program, String name);
		public static GetUniformLocation glGetUniformLocation = _<GetUniformLocation>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate Int32 GetAttribLocation(UInt32 program, String name);
		public static GetAttribLocation glGetAttribLocation = _<GetAttribLocation>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DetachShader(UInt32 program, UInt32 shader);
		public static DetachShader glDetachShader = _<DetachShader>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeleteProgram(UInt32 program);
		public static DeleteProgram glDeleteProgram = _<DeleteProgram>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetActiveAttrib(UInt32 program, UInt32 index, Int32 bufSize, [Out] Int32[] length, [Out] Int32[] size, [Out] ActiveAttribType[] type, [Out] StringBuilder name);
		public static GetActiveAttrib glGetActiveAttrib = _<GetActiveAttrib>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GetActiveUniform(UInt32 program, UInt32 index, Int32 bufSize, [OutAttribute] Int32[] length, [OutAttribute] Int32[] size, [OutAttribute] ActiveUniformType[] type, [OutAttribute] StringBuilder name);
		public static GetActiveUniform glGetActiveUniform = _<GetActiveUniform>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform1f(Int32 location, Single v0);
		public static Uniform1f glUniform1f = _<Uniform1f>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform2f(Int32 location, Single v0, Single v1);
		public static Uniform2f glUniform2f = _<Uniform2f>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform3f(Int32 location, Single v0, Single v1, Single v2);
		public static Uniform3f glUniform3f = _<Uniform3f>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform4f(Int32 location, Single v0, Single v1, Single v2, Single v3);
		public static Uniform4f glUniform4f = _<Uniform4f>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform1i(Int32 location, Int32 v0);
		public static Uniform1i glUniform1i = _<Uniform1i>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform3fv(Int32 location, Int32 count, Single[] value);
		public static Uniform3fv glUniform3fv = _<Uniform3fv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Uniform4fv(Int32 location, Int32 count, Single[] value);
		public static Uniform4fv glUniform4fv = _<Uniform4fv>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void UniformMatrix3fvDel(Int32 location, Int32 count, Boolean transpose, Single[] value);
		public static UniformMatrix3fvDel glUniformMatrix3fv = _Del<UniformMatrix3fvDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void UniformMatrix4fvDel(Int32 location, Int32 count, Boolean transpose, Single[] value);
		public static UniformMatrix4fvDel glUniformMatrix4fv = _Del<UniformMatrix4fvDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BindSampler(UInt32 unit, UInt32 sampler);
		public static BindSampler glBindSampler = _<BindSampler>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BindVertexArray(UInt32 array);
		public static BindVertexArray glBindVertexArray = _<BindVertexArray>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BindBuffer(BufferTarget target, UInt32 buffer);
		public static BindBuffer glBindBuffer = _<BindBuffer>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void EnableVertexAttribArrayDel(UInt32 index);
		public static EnableVertexAttribArrayDel glEnableVertexAttribArray = _Del<EnableVertexAttribArrayDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DisableVertexAttribArray(UInt32 index);
		public static DisableVertexAttribArray glDisableVertexAttribArray = _<DisableVertexAttribArray>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void VertexAttribPointerDel(UInt32 index, Int32 size, VertexAttribPointerType type, Boolean normalized, Int32 stride, IntPtr pointer);
		public static VertexAttribPointerDel glVertexAttribPointer = _Del<VertexAttribPointerDel>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BindTexture(TextureTarget target, UInt32 texture);
		public static BindTexture glBindTexture = _<BindTexture>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void BufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage);
		public static BufferData glBufferData = _<BufferData>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void Scissor(Int32 x, Int32 y, Int32 width, Int32 height);
		public static Scissor glScissor = _<Scissor>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DrawElementsBaseVertex(BeginMode mode, Int32 count, DrawElementsType type, IntPtr indices, Int32 basevertex);
		public static DrawElementsBaseVertex glDrawElementsBaseVertex = _<DrawElementsBaseVertex>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeleteVertexArrays(Int32 n, UInt32[] arrays);
		public static DeleteVertexArrays glDeleteVertexArrays = _<DeleteVertexArrays>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GenVertexArrays(Int32 n, [Out] UInt32[] arrays);
		public static GenVertexArrays glGenVertexArrays = _<GenVertexArrays>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void GenTextures(Int32 n, [OutAttribute] UInt32[] textures);
		public static GenTextures glGenTextures = _<GenTextures>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void PixelStorei(PixelStoreParameter pname, Int32 param);
		public static PixelStorei glPixelStorei = _<PixelStorei>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TexImage2D(TextureTarget target, Int32 level, PixelInternalFormat internalFormat, Int32 width, Int32 height, Int32 border, PixelFormat format, PixelType type, IntPtr data);
		public static TexImage2D glTexImage2D = _<TexImage2D>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TexParameteri(TextureTarget target, TextureParameterName pname, TextureParameter param);
		public static TexParameteri glTexParameteri = _<TexParameteri>();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void DeleteTextures(Int32 n, UInt32[] textures);
		public static DeleteTextures glDeleteTextures = _<DeleteTextures>();
	}
}
