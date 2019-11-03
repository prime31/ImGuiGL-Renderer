using System;
using static SDL2.SDL;
using static SDLImGuiGL.GL;


namespace SDLImGuiGL
{
	public static class ImGuiGL
	{
		public static (IntPtr, IntPtr) CreateWindowAndGLContext(string title, int width, int height, bool fullscreen = false, bool highDpi = false)
		{
			// initialize SDL and set a few defaults for the OpenGL context
			SDL_Init(SDL_INIT_VIDEO);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_FLAGS, (int)SDL_GLcontext.SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);

			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);

			// create the window which should be able to have a valid OpenGL context and is resizable
			var flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
			if (fullscreen)
				flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
			if (highDpi)
				flags |= SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;

			var window = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, flags);
			var glContext = CreateGLContext(window);
			return (window, glContext);
		}

		static IntPtr CreateGLContext(IntPtr window)
		{
			var glContext = SDL_GL_CreateContext(window);
			if (glContext == IntPtr.Zero)
				throw new Exception("CouldNotCreateContext");

			SDL_GL_MakeCurrent(window, glContext);
			SDL_GL_SetSwapInterval(1);

			// initialize the screen to black as soon as possible
			glClearColor(0f, 0f, 0f, 1f);
			glClear(GL.ClearBufferMask.ColorBufferBit);
			SDL_GL_SwapWindow(window);

			Console.WriteLine($"GL Version: {glGetString(GL.StringName.Version)}");

			return glContext;
		}

		public static uint LoadTexture(IntPtr pixelData, int width, int height, GL.PixelFormat format = GL.PixelFormat.Rgba, GL.PixelInternalFormat internalFormat = GL.PixelInternalFormat.Rgba)
		{
			var textureId = GenTexture();

			glPixelStorei(GL.PixelStoreParameter.UnpackAlignment, 1);
			glBindTexture(GL.TextureTarget.Texture2D, textureId);

			glTexImage2D(GL.TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, GL.PixelType.UnsignedByte, pixelData);
			glTexParameteri(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMagFilter, GL.TextureParameter.Linear);
			glTexParameteri(GL.TextureTarget.Texture2D, GL.TextureParameterName.TextureMinFilter, GL.TextureParameter.Linear);

			glBindTexture(GL.TextureTarget.Texture2D, 0);
			return textureId;
		}
	}
}