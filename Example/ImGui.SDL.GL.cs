using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using SDL.GL.ImGui;

namespace Example
{
	public partial class ImGuiDemo
	{
		int width = 800, height = 600;
		IntPtr _window;
		ShaderProgram program;
		uint g_VboHandle, g_ElementsHandle;

		public ImGuiDemo(IntPtr window)
		{
			_window = window;

			// compile the shader program
			program = new ShaderProgram(VertexShader, FragmentShader);

			ImGui.SetCurrentContext(ImGui.CreateContext());
			ImGui.GetIO().DisplaySize = new Vector2(width, height);
			RebuildFontAtlas();
			ImGui_ImplSDL2_Init();

			g_VboHandle = GL.GenBuffer();
			g_ElementsHandle = GL.GenBuffer();
		}

		unsafe void RebuildFontAtlas()
		{
			var fonts = ImGui.GetIO().Fonts;

			fonts.AddFontDefault();
			fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int _);

			var tex = new Texture((IntPtr)pixelData, width, height, GL.PixelFormat.Rgba, GL.PixelInternalFormat.Rgba);

			fonts.TexID = (IntPtr)tex.TextureID;
			fonts.ClearTexData();
		}

		public void Render()
		{
			ImGui_ImplSDL2_NewFrame();
			ImGui.NewFrame();
			ImGui.ShowDemoWindow();
			ImGui.Render();

			var io = ImGui.GetIO();
			GL.glViewport(0, 0, (int)io.DisplaySize.X, (int)io.DisplaySize.Y);
			GL.glClearColor(0.8f, 0.8f, 0.8f, 1);
			GL.glClear(GL.ClearBufferMask.ColorBufferBit | GL.ClearBufferMask.DepthBufferBit);

			ImGui_ImplOpenGL3_RenderDrawData();

			GL.glDisable(GL.EnableCap.ScissorTest);
		}

		void ImGui_ImplOpenGL3_SetupRenderState(ImDrawDataPtr draw_data, int fb_width, int fb_height, uint vertex_array_object)
		{
			GL.glEnable(GL.EnableCap.Blend);
			GL.glBlendEquation(GL.BlendEquationMode.FuncAdd);
			GL.glBlendFunc(GL.BlendingFactorSrc.SrcAlpha, GL.BlendingFactorDest.OneMinusSrcAlpha);
			GL.glDisable(GL.EnableCap.CullFace);
			GL.glDisable(GL.EnableCap.DepthTest);
			GL.glEnable(GL.EnableCap.ScissorTest);

			GL.glUseProgram(program.ProgramID);

			var L = draw_data.DisplayPos.X;
			var R = draw_data.DisplayPos.X + draw_data.DisplaySize.X;
			var T = draw_data.DisplayPos.Y;
			var B = draw_data.DisplayPos.Y + draw_data.DisplaySize.Y;

			program["Texture"].SetValue(0);
			program["ProjMtx"].SetValue(Matrix4x4.CreateOrthographicOffCenter(L, R, B, T, -1, 1));
			GL.glBindSampler(0, 0);

			GL.glBindVertexArray(vertex_array_object);

			// Bind vertex/index buffers and setup attributes for ImDrawVert
			GL.glBindBuffer(GL.BufferTarget.ArrayBuffer, g_VboHandle);
			GL.glBindBuffer(GL.BufferTarget.ElementArrayBuffer, g_ElementsHandle);

			GL.EnableVertexAttribArray(program["Position"].Location);
			GL.EnableVertexAttribArray(program["UV"].Location);
			GL.EnableVertexAttribArray(program["Color"].Location);

			var drawVertSize = Marshal.SizeOf<ImDrawVert>();
			GL.VertexAttribPointer(program["Position"].Location, 2, GL.VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("pos"));
			//glVertexAttribPointer(g_AttribLocationVtxPos, 2, GL_FLOAT, GL_FALSE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, pos));
			GL.VertexAttribPointer(program["UV"].Location, 2, GL.VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("uv"));
			//glVertexAttribPointer(g_AttribLocationVtxUV, 2, GL_FLOAT, GL_FALSE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, uv));
			GL.VertexAttribPointer(program["Color"].Location, 2, GL.VertexAttribPointerType.Byte, true, drawVertSize, Marshal.OffsetOf<ImDrawVert>("col"));
			//glVertexAttribPointer(g_AttribLocationVtxColor, 4, GL_UNSIGNED_BYTE, GL_TRUE, sizeof(ImDrawVert), (GLvoid*)IM_OFFSETOF(ImDrawVert, col));
		}

		unsafe void ImGui_ImplOpenGL3_RenderDrawData()
		{
			var draw_data = ImGui.GetDrawData();

			// Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
			var fb_width = (int)(draw_data.DisplaySize.X * draw_data.FramebufferScale.X);
			var fb_height = (int)(draw_data.DisplaySize.Y * draw_data.FramebufferScale.Y);
			if (fb_width <= 0 || fb_height <= 0)
				return;

			var vertex_array_object = GL.GenVertexArray();
			//glGenVertexArrays(1, &vertex_array_object);
			ImGui_ImplOpenGL3_SetupRenderState(draw_data, fb_width, fb_height, vertex_array_object);

			var clip_off = draw_data.DisplayPos;
			var clip_scale = draw_data.FramebufferScale;

			draw_data.ScaleClipRects(clip_scale);

			var lastTexId = ImGui.GetIO().Fonts.TexID;
			GL.glBindTexture(GL.TextureTarget.Texture2D, (uint)lastTexId);

			for (var n = 0; n < draw_data.CmdListsCount; n++)
			{
				var cmd_list = draw_data.CmdListsRange[n];

				// Upload vertex/index buffers
				var drawVertSize = Marshal.SizeOf<ImDrawVert>();
				var drawIdxSize = 2;

				GL.glBufferData(GL.BufferTarget.ArrayBuffer, (IntPtr)(cmd_list.VtxBuffer.Size * drawVertSize), cmd_list.VtxBuffer.Data, GL.BufferUsageHint.StreamDraw);
				//glBufferData(GL_ARRAY_BUFFER, (GLsizeiptr)cmd_list->VtxBuffer.Size * sizeof(ImDrawVert), (const GLvoid*)cmd_list->VtxBuffer.Data, GL_STREAM_DRAW);
				GL.glBufferData(GL.BufferTarget.ElementArrayBuffer, (IntPtr)(cmd_list.IdxBuffer.Size * drawIdxSize), cmd_list.IdxBuffer.Data, GL.BufferUsageHint.StreamDraw);
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
						GL.glScissor((int)clip_rect.X, (int)clip_rect.Y, (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));
						//Gl.Scissor((int)clip_rect.X, (int)(fb_height - clip_rect.X), (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));
						//glScissor((int)clip_rect.x, (int)(fb_height - clip_rect.w), (int)(clip_rect.z - clip_rect.x), (int)(clip_rect.w - clip_rect.y));

						// Bind texture, Draw
						if (pcmd.TextureId != IntPtr.Zero)
						{
							if (pcmd.TextureId != lastTexId)
							{
								lastTexId = pcmd.TextureId;
								GL.glBindTexture(GL.TextureTarget.Texture2D, (uint)pcmd.TextureId);
							}
						}

						GL.glDrawElementsBaseVertex(GL.BeginMode.Triangles, (int)pcmd.ElemCount, drawIdxSize == 2 ? GL.DrawElementsType.UnsignedShort : GL.DrawElementsType.UnsignedInt, (IntPtr)(pcmd.IdxOffset * drawIdxSize), (int)pcmd.VtxOffset);
						//glDrawElementsBaseVertex(GL_TRIANGLES, (GLsizei)pcmd->ElemCount, sizeof(ImDrawIdx) == 2 ? GL_UNSIGNED_SHORT : GL_UNSIGNED_INT, (void*)(intptr_t)(pcmd->IdxOffset * sizeof(ImDrawIdx)), (GLint)pcmd->VtxOffset);
					}
				}
			}

			GL.DeleteVertexArray(vertex_array_object);
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
}";

	}
}
