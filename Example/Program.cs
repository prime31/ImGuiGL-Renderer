using System;
using ImGuiNET;
using SDLImGuiGL;
using static SDL2.SDL;

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
			(_window, _glContext) = ImGuiGL.CreateWindowAndGLContext("SDL GL ImGui Renderer", 800, 600);
			_renderer = new ImGuiGLRenderer(_window, _glContext);

			while (!_quit)
			{
				// send events to our window
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