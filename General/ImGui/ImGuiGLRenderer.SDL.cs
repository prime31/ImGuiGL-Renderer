using System.Numerics;
using ImGuiNET;
using static SDL2.SDL;


namespace ImGuiGeneral
{
	public partial class ImGuiGLRenderer
	{
		float _time;
		readonly bool[] _mousePressed = { false, false, false };

		public void NewFrame()
		{
			ImGui.NewFrame();
			var io = ImGui.GetIO();

			// Setup display size (every frame to accommodate for window resizing)
			SDL_GetWindowSize(_window, out var w, out var h);
			SDL_GL_GetDrawableSize(_window, out var displayW, out var displayH);
			io.DisplaySize = new Vector2(w, h);
			if (w > 0 && h > 0)
				io.DisplayFramebufferScale = new Vector2((float)displayW / w, (float)displayH / h);

			// Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
			var frequency = SDL_GetPerformanceFrequency();
			var currentTime = SDL_GetPerformanceCounter();
			io.DeltaTime = _time > 0 ? (float)((double)(currentTime - _time) / frequency) : 1.0f / 60.0f;
			if (io.DeltaTime <= 0)
				io.DeltaTime = 0.016f;
			_time = currentTime;

			UpdateMousePosAndButtons();
		}

		public unsafe void ProcessEvent(SDL_Event evt)
		{
			var io = ImGui.GetIO();
			switch (evt.type)
			{
				case SDL_EventType.SDL_MOUSEWHEEL:
					{
						if (evt.wheel.x > 0) io.MouseWheelH += 1;
						if (evt.wheel.x < 0) io.MouseWheelH -= 1;
						if (evt.wheel.y > 0) io.MouseWheel += 1;
						if (evt.wheel.y < 0) io.MouseWheel -= 1;
						return;
					}
				case SDL_EventType.SDL_MOUSEBUTTONDOWN:
					{
						if (evt.button.button == SDL_BUTTON_LEFT) _mousePressed[0] = true;
						if (evt.button.button == SDL_BUTTON_RIGHT) _mousePressed[1] = true;
						if (evt.button.button == SDL_BUTTON_MIDDLE) _mousePressed[2] = true;
						return;
					}
				case SDL_EventType.SDL_TEXTINPUT:
					{
						var str = new string((sbyte*)evt.text.text);
						io.AddInputCharactersUTF8(str);
						return;
					}
				case SDL_EventType.SDL_KEYDOWN:
					// Modifiers
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_LSHIFT)
						io.AddKeyEvent(ImGuiKey.LeftShift, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_RSHIFT)
						io.AddKeyEvent(ImGuiKey.RightShift, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_LALT)
						io.AddKeyEvent(ImGuiKey.LeftAlt, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_RALT)
						io.AddKeyEvent(ImGuiKey.RightAlt, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_LCTRL)
						io.AddKeyEvent(ImGuiKey.LeftCtrl, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_RCTRL)
						io.AddKeyEvent(ImGuiKey.RightCtrl, true);

					// Keys
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
						io.AddKeyEvent(ImGuiKey.Escape, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_TAB)
						io.AddKeyEvent(ImGuiKey.Tab, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_LEFT)
						io.AddKeyEvent(ImGuiKey.LeftArrow, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_RIGHT)
						io.AddKeyEvent(ImGuiKey.RightArrow, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_UP)
						io.AddKeyEvent(ImGuiKey.UpArrow, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_DOWN)
						io.AddKeyEvent(ImGuiKey.DownArrow, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_PAGEUP)
						io.AddKeyEvent(ImGuiKey.PageUp, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_PAGEDOWN)
						io.AddKeyEvent(ImGuiKey.PageDown, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_HOME)
						io.AddKeyEvent(ImGuiKey.Home, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_END)
						io.AddKeyEvent(ImGuiKey.End, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_INSERT)
						io.AddKeyEvent(ImGuiKey.Insert, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_DELETE)
						io.AddKeyEvent(ImGuiKey.Delete, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_BACKSPACE)
						io.AddKeyEvent(ImGuiKey.Backspace, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_SPACE)
						io.AddKeyEvent(ImGuiKey.Space, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_RETURN)
						io.AddKeyEvent(ImGuiKey.Enter, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_KP_ENTER)
						io.AddKeyEvent(ImGuiKey.KeypadEnter, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_a)
						io.AddKeyEvent(ImGuiKey.A, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_c)
						io.AddKeyEvent(ImGuiKey.C, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_v)
						io.AddKeyEvent(ImGuiKey.V, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_x)
						io.AddKeyEvent(ImGuiKey.X, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_y)
						io.AddKeyEvent(ImGuiKey.Y, true);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_z)
						io.AddKeyEvent(ImGuiKey.Z, true);
					break;

				case SDL_EventType.SDL_KEYUP:
					{
						// Modifiers
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_LSHIFT)
							io.AddKeyEvent(ImGuiKey.LeftShift, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_RSHIFT)
							io.AddKeyEvent(ImGuiKey.RightShift, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_LALT)
							io.AddKeyEvent(ImGuiKey.LeftAlt, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_RALT)
							io.AddKeyEvent(ImGuiKey.RightAlt, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_LCTRL)
							io.AddKeyEvent(ImGuiKey.LeftCtrl, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_RCTRL)
							io.AddKeyEvent(ImGuiKey.RightCtrl, false);

						// Keys
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
							io.AddKeyEvent(ImGuiKey.Escape, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_TAB)
							io.AddKeyEvent(ImGuiKey.Tab, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_LEFT)
							io.AddKeyEvent(ImGuiKey.LeftArrow, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_RIGHT)
							io.AddKeyEvent(ImGuiKey.RightArrow, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_UP)
							io.AddKeyEvent(ImGuiKey.UpArrow, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_DOWN)
							io.AddKeyEvent(ImGuiKey.DownArrow, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_PAGEUP)
							io.AddKeyEvent(ImGuiKey.PageUp, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_PAGEDOWN)
							io.AddKeyEvent(ImGuiKey.PageDown, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_HOME)
							io.AddKeyEvent(ImGuiKey.Home, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_END)
							io.AddKeyEvent(ImGuiKey.End, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_INSERT)
							io.AddKeyEvent(ImGuiKey.Insert, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_DELETE)
							io.AddKeyEvent(ImGuiKey.Delete, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_BACKSPACE)
							io.AddKeyEvent(ImGuiKey.Backspace, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_SPACE)
							io.AddKeyEvent(ImGuiKey.Space, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_RETURN)
							io.AddKeyEvent(ImGuiKey.Enter, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_KP_ENTER)
							io.AddKeyEvent(ImGuiKey.KeypadEnter, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_a)
							io.AddKeyEvent(ImGuiKey.A, false);
					if (evt.key.keysym.sym == SDL_Keycode.SDLK_c)
							io.AddKeyEvent(ImGuiKey.C, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_v)
							io.AddKeyEvent(ImGuiKey.V, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_x)
							io.AddKeyEvent(ImGuiKey.X, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_y)
							io.AddKeyEvent(ImGuiKey.Y, false);
						if (evt.key.keysym.sym == SDL_Keycode.SDLK_z)
							io.AddKeyEvent(ImGuiKey.Z, false);
						break;
					}
			}
		}

		void UpdateMousePosAndButtons()
		{
			var io = ImGui.GetIO();

			// Set OS mouse position if requested (rarely used, only when ImGuiConfigFlags_NavEnableSetMousePos is enabled by user)
			if (io.WantSetMousePos)
				SDL_WarpMouseInWindow(_window, (int)io.MousePos.X, (int)io.MousePos.Y);
			else
				io.MousePos = new Vector2(float.MinValue, float.MinValue);

			var mouseButtons = SDL_GetMouseState(out var mx, out var my);
			io.MouseDown[0] =
				_mousePressed[0] ||
				(mouseButtons & SDL_BUTTON(SDL_BUTTON_LEFT)) !=
				0; // If a mouse press event came, always pass it as "mouse held this frame", so we don't miss click-release events that are shorter than 1 frame.
			io.MouseDown[1] = _mousePressed[1] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_RIGHT)) != 0;
			io.MouseDown[2] = _mousePressed[2] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_MIDDLE)) != 0;
			_mousePressed[0] = _mousePressed[1] = _mousePressed[2] = false;

			var focusedWindow = SDL_GetKeyboardFocus();
			if (_window == focusedWindow)
			{
				// SDL_GetMouseState() gives mouse position seemingly based on the last window entered/focused(?)
				// The creation of a new windows at runtime and SDL_CaptureMouse both seems to severely mess up with that, so we retrieve that position globally.
				SDL_GetWindowPosition(focusedWindow, out var wx, out var wy);
				SDL_GetGlobalMouseState(out mx, out my);
				mx -= wx;
				my -= wy;
				io.MousePos = new Vector2(mx, my);
			}

			// SDL_CaptureMouse() let the OS know e.g. that our imgui drag outside the SDL window boundaries shouldn't e.g. trigger the OS window resize cursor.
			var any_mouse_button_down = ImGui.IsAnyMouseDown();
			SDL_CaptureMouse(any_mouse_button_down ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
		}

		void PrepareGLContext() => SDL_GL_MakeCurrent(_window, _glContext);
	}
}