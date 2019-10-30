using System.Numerics;
using ImGuiNET;
using static SDL2.SDL;

namespace Example
{
	public partial class ImGuiDemo
	{
		float g_Time;
		bool[] g_MousePressed = { false, false, false };

		void ImGui_ImplSDL2_Init()
		{
			var io = ImGui.GetIO();

			io.KeyMap[(int)ImGuiKey.Tab] = (int)SDL_Scancode.SDL_SCANCODE_TAB;
			io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)SDL_Scancode.SDL_SCANCODE_LEFT;
			io.KeyMap[(int)ImGuiKey.RightArrow] = (int)SDL_Scancode.SDL_SCANCODE_RIGHT;
			io.KeyMap[(int)ImGuiKey.UpArrow] = (int)SDL_Scancode.SDL_SCANCODE_UP;
			io.KeyMap[(int)ImGuiKey.DownArrow] = (int)SDL_Scancode.SDL_SCANCODE_DOWN;
			io.KeyMap[(int)ImGuiKey.PageUp] = (int)SDL_Scancode.SDL_SCANCODE_PAGEUP;
			io.KeyMap[(int)ImGuiKey.PageDown] = (int)SDL_Scancode.SDL_SCANCODE_PAGEDOWN;
			io.KeyMap[(int)ImGuiKey.Home] = (int)SDL_Scancode.SDL_SCANCODE_HOME;
			io.KeyMap[(int)ImGuiKey.End] = (int)SDL_Scancode.SDL_SCANCODE_END;
			io.KeyMap[(int)ImGuiKey.Insert] = (int)SDL_Scancode.SDL_SCANCODE_INSERT;
			io.KeyMap[(int)ImGuiKey.Delete] = (int)SDL_Scancode.SDL_SCANCODE_DELETE;
			io.KeyMap[(int)ImGuiKey.Backspace] = (int)SDL_Scancode.SDL_SCANCODE_BACKSPACE;
			io.KeyMap[(int)ImGuiKey.Space] = (int)SDL_Scancode.SDL_SCANCODE_SPACE;
			io.KeyMap[(int)ImGuiKey.Enter] = (int)SDL_Scancode.SDL_SCANCODE_RETURN;
			io.KeyMap[(int)ImGuiKey.Escape] = (int)SDL_Scancode.SDL_SCANCODE_ESCAPE;
			io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)SDL_Scancode.SDL_SCANCODE_RETURN2;
			io.KeyMap[(int)ImGuiKey.A] = (int)SDL_Scancode.SDL_SCANCODE_A;
			io.KeyMap[(int)ImGuiKey.C] = (int)SDL_Scancode.SDL_SCANCODE_C;
			io.KeyMap[(int)ImGuiKey.V] = (int)SDL_Scancode.SDL_SCANCODE_V;
			io.KeyMap[(int)ImGuiKey.X] = (int)SDL_Scancode.SDL_SCANCODE_X;
			io.KeyMap[(int)ImGuiKey.Y] = (int)SDL_Scancode.SDL_SCANCODE_Y;
			io.KeyMap[(int)ImGuiKey.Z] = (int)SDL_Scancode.SDL_SCANCODE_Z;
		}

		void ImGui_ImplSDL2_NewFrame()
		{
			var io = ImGui.GetIO();

			// Setup display size (every frame to accommodate for window resizing)
			SDL_GetWindowSize(_window, out var w, out var h);
			SDL_GL_GetDrawableSize(_window, out var display_w, out var display_h);
			io.DisplaySize = new Vector2((float)w, (float)h);
			if (w > 0 && h > 0)
				io.DisplayFramebufferScale = new Vector2((float)display_w / w, (float)display_h / h);

			// Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
			var frequency = SDL_GetPerformanceFrequency();
			var current_time = SDL_GetPerformanceCounter();
			io.DeltaTime = g_Time > 0 ? (float)((double)(current_time - g_Time) / frequency) : 1.0f / 60.0f;
			if (io.DeltaTime <= 0)
				io.DeltaTime = 0.016f;
			g_Time = current_time;

			ImGui_ImplSDL2_UpdateMousePosAndButtons();
			ImGui_ImplSDL2_UpdateGamepads();
		}

		public unsafe void ImGui_ImplSDL2_ProcessEvent(SDL_Event evt)
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
						if (evt.button.button == SDL_BUTTON_LEFT) g_MousePressed[0] = true;
						if (evt.button.button == SDL_BUTTON_RIGHT) g_MousePressed[1] = true;
						if (evt.button.button == SDL_BUTTON_MIDDLE) g_MousePressed[2] = true;
						return;
					}
				case SDL_EventType.SDL_TEXTINPUT:
					{
						var str = new string((sbyte*)evt.text.text);
						io.AddInputCharactersUTF8(str);
						return;
					}
				case SDL_EventType.SDL_KEYDOWN:
				case SDL_EventType.SDL_KEYUP:
					{
						var key = evt.key.keysym.scancode;
						io.KeysDown[(int)key] = (evt.type == SDL_EventType.SDL_KEYDOWN);
						io.KeyShift = ((SDL_GetModState() & SDL_Keymod.KMOD_SHIFT) != 0);
						io.KeyCtrl = ((SDL_GetModState() & SDL_Keymod.KMOD_CTRL) != 0);
						io.KeyAlt = ((SDL_GetModState() & SDL_Keymod.KMOD_ALT) != 0);
						io.KeySuper = ((SDL_GetModState() & SDL_Keymod.KMOD_GUI) != 0);
						break;
					}
			}
		}

		void ImGui_ImplSDL2_UpdateMousePosAndButtons()
		{
			var io = ImGui.GetIO();

			// Set OS mouse position if requested (rarely used, only when ImGuiConfigFlags_NavEnableSetMousePos is enabled by user)
			if (io.WantSetMousePos)
				SDL_WarpMouseInWindow(_window, (int)io.MousePos.X, (int)io.MousePos.Y);
			else
				io.MousePos = new Vector2(float.MinValue, float.MinValue);

			var mouseButtons = SDL_GetMouseState(out var mx, out var my);
			io.MouseDown[0] = g_MousePressed[0] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_LEFT)) != 0;  // If a mouse press event came, always pass it as "mouse held this frame", so we don't miss click-release events that are shorter than 1 frame.
			io.MouseDown[1] = g_MousePressed[1] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_RIGHT)) != 0;
			io.MouseDown[2] = g_MousePressed[2] || (mouseButtons & SDL_BUTTON(SDL_BUTTON_MIDDLE)) != 0;
			g_MousePressed[0] = g_MousePressed[1] = g_MousePressed[2] = false;

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
			// The function is only supported from SDL 2.0.4 (released Jan 2016)
			var any_mouse_button_down = ImGui.IsAnyMouseDown();
			SDL_CaptureMouse(any_mouse_button_down ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
		}

		void ImGui_ImplSDL2_UpdateGamepads()
		{ }
	}
}
