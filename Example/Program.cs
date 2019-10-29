using System;
using OpenGL;
using OpenGL.Platform;
using static SDL2.SDL;

namespace Example
{
	class MainClass
	{
		static IDemo _demo;
		static IDemo demo
		{
			get => _demo;
			set
			{
				if (_demo is IDisposable disposable)
					disposable.Dispose();
				_demo = value;
			}
		}

		static bool quit;

		public static void Main(string[] args)
		{
			Window.CreateWindow("Poop", 800, 600);
			Window.OnEvent += HandleEvent;
			demo = new ImGuiDemo();

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
			Gl.Viewport(0, 0, 800, 600);
			Gl.ClearColor(0.4f, 0.4f, 0.4f, 1);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		static void HandleEvent(SDL_Event e)
		{
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
							case SDL_Keycode.SDLK_0:
								demo = null;
								break;
							case SDL_Keycode.SDLK_1:
								demo = new Primitives();
								break;
							case SDL_Keycode.SDLK_2:
								demo = new CubeDemo();
								break;
							case SDL_Keycode.SDLK_3:
								demo = new QuadDemo();
								break;
							case SDL_Keycode.SDLK_4:
								demo = new TexturedQuadDemo();
								break;
							case SDL_Keycode.SDLK_5:
								demo = new ImGuiDemo();
								break;
						}
						break;
					}
			}
		}
	}
}
