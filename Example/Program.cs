using System;
using ImGuiNET;
using static SDL2.SDL;
using ImGuiGeneral;

namespace Example
{
	internal class MainClass
	{
		static ImGuiGLRenderer _renderer;
		static bool _quit;
		static IntPtr _window;
		static IntPtr _glContext;

		public static void Main(string[] args)
		{
			// create a window, GL context and our ImGui renderer
			// this is fast solution for create SDL_Window and SDL_Render
			(_window, _glContext) = ImGuiGL.CreateWindowAndGLContext("SDL Window (OpenGL)", 800, 600);
			_renderer = new ImGuiGLRenderer(_window, _glContext);

			while (!_quit)
			{
				while (SDL_PollEvent(out var e) != 0)
				{
					_renderer.ProcessEvent(e);
					switch (e.type)
					{
						case SDL_EventType.SDL_QUIT:
						{
							_quit = true;
							break;
						}
						case SDL_EventType.SDL_KEYDOWN:
						{
							switch (e.key.keysym.sym)
							{
								case SDL_Keycode.SDLK_ESCAPE:
								case SDL_Keycode.SDLK_q:
									_quit = true;
									break;
							}

							break;
						}
					}
				}

				_renderer.ClearColor(0.05f, 0.05f, 0.05f, 1.00f);
				_renderer.NewFrame();
				ImGui.ShowDemoWindow();
				_renderer.Render();
				SDL_GL_SwapWindow(_window);
			}

			SDL_GL_DeleteContext(_glContext);
			SDL_DestroyWindow(_window);
			SDL_Quit();
		}
	}
}