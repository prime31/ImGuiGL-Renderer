using OpenGL.Platform;
using SDL.GL.ImGui;
using static SDL2.SDL;

namespace Example
{
	class MainClass
	{
		static ImGuiDemo demo;
		static bool quit;

		public static void Main(string[] args)
		{
			Window.CreateWindow("Poop", 800, 600);
			Window.OnEvent += HandleEvent;
			demo = new ImGuiDemo(Window.window);



			var flags = SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
			SDL_CreateWindow("fucker", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 300, 800, flags);


			while (!quit)
			{
				Window.HandleEvents();

				if (demo != null)
					demo.Render();
				else
					Render();
				SDL_GL_SwapWindow(Window.window);
			}

			SDL_Quit();
		}

		static void Render()
		{
			GL.glViewport(0, 0, 800, 600);
			GL.glClearColor(0.4f, 0.4f, 0.4f, 1);
			GL.glClear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit);
		}

		static void HandleEvent(SDL_Event e)
		{
			demo.ImGui_ImplSDL2_ProcessEvent(e);

			switch (e.type)
			{
				case SDL_EventType.SDL_QUIT:
					{
						quit = true;
						break;
					}
				case SDL_EventType.SDL_KEYDOWN:
					{
						switch (e.key.keysym.sym)
						{
							case SDL_Keycode.SDLK_q:
								quit = true;
								break;
						}
						break;
					}
			}
		}
	}
}
