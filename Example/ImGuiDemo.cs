using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenGL;
using OpenGL.Platform;
using static SDL2.SDL;

namespace Example
{
	public class ImGuiDemo : IDemo, IDisposable
	{
		private int width = 800, height = 600;
		private ShaderProgram program;
		float g_Time;
		uint g_VboHandle, g_ElementsHandle;
		bool[] g_MousePressed = { false, false, false };

		public ImGuiDemo()
		{
			// compile the shader program
			program = new ShaderProgram(VertexShader, FragmentShader);

			ImGui.SetCurrentContext(ImGui.CreateContext());
			ImGui.GetIO().DisplaySize = new Vector2(width, height);
			RebuildFontAtlas();
			ImGui_ImplSDL2_Init();

			g_VboHandle = Gl.GenBuffer();
			g_ElementsHandle = Gl.GenBuffer();

			Window.OnEvent += ImGui_ImplSDL2_ProcessEvent;
		}

		public void Dispose()
		{
			Window.OnEvent -= ImGui_ImplSDL2_ProcessEvent;
		}

		unsafe void RebuildFontAtlas()
		{
			// Get font texture from ImGui
			var fonts = ImGui.GetIO().Fonts;

			fonts.AddFontDefault();
			fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

			// Copy the data to a managed array
			// var pixels = new byte[width * height * bytesPerPixel];
			// Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

			var tex = new Texture((IntPtr)pixelData, width, height, PixelFormat.Rgba, PixelInternalFormat.Rgba);

			// Store our identifier
			fonts.TexID = (IntPtr)tex.TextureID;


			// Should a texture already have been built previously, unbind it first so it can be deallocated
			// if (_fontTextureId.HasValue)
			// 	UnbindTexture(_fontTextureId.Value);

			// Bind the new texture to an ImGui-friendly id
			// _fontTextureId = BindTexture(tex2d);
			// var _fontTextureId = new IntPtr(666);

			// Let ImGui know where to find the texture
			// fonts.SetTexID(_fontTextureId);
			// fonts.SetTexID(_fontTextureId.Value);
			fonts.ClearTexData(); // Clears CPU side texture data
		}

		public void Render()
		{
			ImGui_ImplSDL2_NewFrame();
			ImGui.NewFrame();
			ImGui.ShowDemoWindow();
			ImGui.Render();

			var io = ImGui.GetIO();
			Gl.Viewport(0, 0, (int)io.DisplaySize.X, (int)io.DisplaySize.Y);
			Gl.ClearColor(0.7f, 0.6f, 0.9f, 1);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			ImGui_ImplOpenGL3_RenderDrawData();

			Gl.Disable(EnableCap.ScissorTest);
		}

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
			SDL_GetWindowSize(Window.window, out var w, out var h);
			SDL_GL_GetDrawableSize(Window.window, out var display_w, out var display_h);
			io.DisplaySize = new Vector2((float)w, (float)h);
			if (w > 0 && h > 0)
				io.DisplayFramebufferScale = new Vector2((float)display_w / w, (float)display_h / h);

			// Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
			var frequency = SDL_GetPerformanceFrequency();
			var current_time = SDL_GetPerformanceCounter();
			io.DeltaTime = g_Time > 0 ? (float)((double)(current_time - g_Time) / frequency) : (float)(1.0f / 60.0f);
			if (io.DeltaTime <= 0)
				io.DeltaTime = 0.016f;
			g_Time = current_time;

			ImGui_ImplSDL2_UpdateMousePosAndButtons();
			ImGui_ImplSDL2_UpdateGamepads();
		}

		void ImGui_ImplSDL2_ProcessEvent(SDL_Event evt)
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
						//io.AddInputCharactersUTF8(evt.text.text);
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
				SDL_WarpMouseInWindow(Window.window, (int)io.MousePos.X, (int)io.MousePos.Y);
			else
				io.MousePos = new Vector2(float.MinValue, float.MinValue);

			var mouse_buttons = SDL_GetMouseState(out var mx, out var my);
			io.MouseDown[0] = g_MousePressed[0] || (mouse_buttons & SDL_BUTTON(SDL_BUTTON_LEFT)) != 0;  // If a mouse press event came, always pass it as "mouse held this frame", so we don't miss click-release events that are shorter than 1 frame.
			io.MouseDown[1] = g_MousePressed[1] || (mouse_buttons & SDL_BUTTON(SDL_BUTTON_RIGHT)) != 0;
			io.MouseDown[2] = g_MousePressed[2] || (mouse_buttons & SDL_BUTTON(SDL_BUTTON_MIDDLE)) != 0;
			g_MousePressed[0] = g_MousePressed[1] = g_MousePressed[2] = false;

			var focused_window = SDL_GetKeyboardFocus();
			if (Window.window == focused_window)
			{
				// SDL_GetMouseState() gives mouse position seemingly based on the last window entered/focused(?)
				// The creation of a new windows at runtime and SDL_CaptureMouse both seems to severely mess up with that, so we retrieve that position globally.
				SDL_GetWindowPosition(focused_window, out var wx, out var wy);
				SDL_GetGlobalMouseState(out mx, out my);
				mx -= wx;
				my -= wy;
				io.MousePos = new Vector2((float)mx, (float)my);
			}

			// SDL_CaptureMouse() let the OS know e.g. that our imgui drag outside the SDL window boundaries shouldn't e.g. trigger the OS window resize cursor.
			// The function is only supported from SDL 2.0.4 (released Jan 2016)
			var any_mouse_button_down = ImGui.IsAnyMouseDown();
			SDL_CaptureMouse(any_mouse_button_down ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
		}

		void ImGui_ImplSDL2_UpdateGamepads()
		{ }

		void ImGui_ImplOpenGL3_SetupRenderState(ImDrawDataPtr draw_data, int fb_width, int fb_height, uint vertex_array_object)
		{
			Gl.Enable(EnableCap.Blend);
			Gl.BlendEquation(BlendEquationMode.FuncAdd);
			Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			Gl.Disable(EnableCap.CullFace);
			Gl.Disable(EnableCap.DepthTest);
			Gl.Enable(EnableCap.ScissorTest);

			//Gl.Viewport(0, 0, fb_width, fb_height);
			//Gl.ClearColor(0.7f, 0.6f, 0.9f, 1);
			//Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Gl.UseProgram(program);

			var L = draw_data.DisplayPos.X;
			var R = draw_data.DisplayPos.X + draw_data.DisplaySize.X;
			var T = draw_data.DisplayPos.Y;
			var B = draw_data.DisplayPos.Y + draw_data.DisplaySize.Y;

			program["Texture"].SetValue(0);
			program["ProjMtx"].SetValue(Matrix4.CreateOrthographicOffCenter(L, R, B, T, -1, 1));
			Gl.BindSampler(0, 0);

			Gl.BindVertexArray(vertex_array_object);
			//glBindVertexArray(vertex_array_object);

			// Bind vertex/index buffers and setup attributes for ImDrawVert
			Gl.BindBuffer(BufferTarget.ArrayBuffer, g_VboHandle);
			Gl.BindBuffer(BufferTarget.ArrayBuffer, g_ElementsHandle);

			Gl.EnableVertexAttribArray(program["Position"].Location);
			Gl.EnableVertexAttribArray(program["UV"].Location);
			Gl.EnableVertexAttribArray(program["Color"].Location);

			var drawVertSize = Marshal.SizeOf<ImDrawVert>();
			Gl.VertexAttribPointer(program["Position"].Location, 2, VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("pos"));
			//glVertexAttribPointer(g_AttribLocationVtxPos, 2, GL_FLOAT, GL_FALSE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, pos));
			Gl.VertexAttribPointer(program["UV"].Location, 2, VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("uv"));
			//glVertexAttribPointer(g_AttribLocationVtxUV, 2, GL_FLOAT, GL_FALSE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, uv));
			Gl.VertexAttribPointer(program["Color"].Location, 2, VertexAttribPointerType.Byte, true, drawVertSize, Marshal.OffsetOf<ImDrawVert>("col"));
			//glVertexAttribPointer(g_AttribLocationVtxColor, 4, GL_UNSIGNED_BYTE, GL_TRUE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, col));
		}

		unsafe void ImGui_ImplOpenGL3_RenderDrawData()
		{
			var draw_data = ImGui.GetDrawData();

			// Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
			int fb_width = (int)(draw_data.DisplaySize.X * draw_data.FramebufferScale.X);
			int fb_height = (int)(draw_data.DisplaySize.Y * draw_data.FramebufferScale.Y);
			if (fb_width <= 0 || fb_height <= 0)
				return;

			var vertex_array_object = Gl.GenVertexArray();
			//glGenVertexArrays(1, &vertex_array_object);
			ImGui_ImplOpenGL3_SetupRenderState(draw_data, fb_width, fb_height, vertex_array_object);

			var clip_off = draw_data.DisplayPos;
			var clip_scale = draw_data.FramebufferScale;

			draw_data.ScaleClipRects(clip_scale);

			var lastTexId = ImGui.GetIO().Fonts.TexID;
			Gl.BindTexture(TextureTarget.Texture2D, (uint)lastTexId);

			for (var n = 0; n < draw_data.CmdListsCount; n++)
			{
				var cmd_list = draw_data.CmdListsRange[n];

				// Upload vertex/index buffers
				var drawVertSize = Marshal.SizeOf<ImDrawVert>();
				var drawIdxSize = 2;

				Gl.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cmd_list.VtxBuffer.Size * drawVertSize), cmd_list.VtxBuffer.Data, BufferUsageHint.StreamDraw);
				//glBufferData(GL_ARRAY_BUFFER, (GLsizeiptr)cmd_list->VtxBuffer.Size * sizeof(ImDrawVert), (const GLvoid*)cmd_list->VtxBuffer.Data, GL_STREAM_DRAW);
				Gl.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(cmd_list.IdxBuffer.Size * drawIdxSize), cmd_list.IdxBuffer.Data, BufferUsageHint.StreamDraw);
				//glBufferData(GL_ELEMENT_ARRAY_BUFFER, (GLsizeiptr)cmd_list->IdxBuffer.Size * sizeof(ImDrawIdx), (const GLvoid*)cmd_list->IdxBuffer.Data, GL_STREAM_DRAW);

				for (var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
				{
					var pcmd = cmd_list.CmdBuffer[cmd_i];
					if (pcmd.UserCallback != IntPtr.Zero)
					{
						Console.WriteLine("UserCallback not implemented");
					}
					else
					{
						// Project scissor/clipping rectangles into framebuffer space
						var clip_rect = pcmd.ClipRect;
						Gl.Scissor((int)clip_rect.X, (int)clip_rect.Y, (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));
						//Gl.Scissor((int)clip_rect.X, (int)(fb_height - clip_rect.X), (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));
						//glScissor((int)clip_rect.x, (int)(fb_height - clip_rect.w), (int)(clip_rect.z - clip_rect.x), (int)(clip_rect.w - clip_rect.y));

						// Bind texture, Draw
						if (pcmd.TextureId != IntPtr.Zero)
						{
							if (pcmd.TextureId != lastTexId)
							{
								lastTexId = pcmd.TextureId;
								Gl.BindTexture(TextureTarget.Texture2D, (uint)pcmd.TextureId);
							}
						}

						Gl.DrawElementsBaseVertex(BeginMode.Triangles, (int)pcmd.ElemCount, drawIdxSize == 2 ? DrawElementsType.UnsignedShort : DrawElementsType.UnsignedInt, (IntPtr)(pcmd.IdxOffset * drawIdxSize), (int)pcmd.VtxOffset);
						//glDrawElementsBaseVertex(GL_TRIANGLES, (GLsizei)pcmd->ElemCount, sizeof(ImDrawIdx) == 2 ? GL_UNSIGNED_SHORT : GL_UNSIGNED_INT, (void*)(intptr_t)(pcmd->IdxOffset * sizeof(ImDrawIdx)), (GLint)pcmd->VtxOffset);
					}
				}
			}

			Gl.DeleteVertexArray(vertex_array_object);
			//glDeleteVertexArrays(1, &vertex_array_object);
		}

		public static string VertexShader = @"
#version 330

precision mediump float;
layout (location = 0) in vec2 Position;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec4 Color;
uniform mat4 ProjMtx;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
    Frag_UV = UV;
    Frag_Color = Color;
    gl_Position = ProjMtx * vec4(Position.xy, 0, 1);
}";

		public static string FragmentShader = @"
#version 330

precision mediump float;
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
layout (location = 0) out vec4 Out_Color;

void main()
{
    Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
	Out_Color = vec4(1, 1, 1, 1);
}";

	}
}
