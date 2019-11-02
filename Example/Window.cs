using System;
using SDL.ImGuiRenderer;
using static SDL2.SDL;

namespace OpenGL.Platform
{
    public static class Window
    {
		public static event Action<SDL_Event> OnEvent;
		public static IntPtr window, glContext;
		public static uint windowID;

        public static void CreateWindow(string title, int width, int height, bool fullscreen = false, bool highDpi = false)
        {
            // check if a window already exists
            if (window != IntPtr.Zero || glContext != IntPtr.Zero)
				throw new Exception("AlreadyInitialized");

			// initialize SDL and set a few defaults for the OpenGL context
			SDL_Init(SDL_INIT_VIDEO);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_FLAGS, (int)SDL_GLcontext.SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG); // Always required on Mac
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);

			SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);
			SDL_GL_SetSwapInterval(1); // Enable vsync

			// create the window which should be able to have a valid OpenGL context and is resizable
			var flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            if (fullscreen)
				flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
			if (highDpi)
				flags |= SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI;

			window = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, flags);

			SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 300, 800, flags);

			if (window == IntPtr.Zero)
				throw new Exception("CouldNotCreateWindow");

			windowID = SDL_GetWindowID(window);

            CreateContextFromWindow(window, fullscreen);
        }

        public static void CreateContextFromWindow(IntPtr window, bool fullscreen = false)
        {
            if (window == IntPtr.Zero)
				throw new Exception("WindowWasNotInitialized");

            SDL_GetWindowSize(window, out int width, out int height);

            // create a valid OpenGL context within the newly created window
            glContext = SDL_GL_CreateContext(window);
            if (glContext == IntPtr.Zero)
				throw new Exception("CouldNotCreateContext");

            // initialize the screen to black as soon as possible
            GL.glClearColor(0f, 0f, 0f, 1f);
			GL.glClear(GL.ClearBufferMask.ColorBufferBit);
			SDL_GL_SwapWindow(window);

			Console.WriteLine($"GL Version: {GL.glGetString(GL.StringName.Version)}");
        }

        public static void HandleEvents()
        {
            while (SDL_PollEvent(out var sdlEvent) != 0 && window != IntPtr.Zero && sdlEvent.window.windowID == windowID)
            {
				OnEvent?.Invoke(sdlEvent);
            }
        }
    }
}
