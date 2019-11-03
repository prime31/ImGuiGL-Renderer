using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using static SDLImGuiGL.GL;

namespace SDLImGuiGL
{
	public partial class ImGuiGLRenderer : IDisposable
	{
		readonly IntPtr _window;
		GLShaderProgram _shader;
		uint _vboHandle, _elementsHandle, _vertexArrayObject, _fontTextureId;

		public ImGuiGLRenderer(IntPtr window)
		{
			_window = window;

			// compile the shader program
			_shader = new GLShaderProgram(VertexShader, FragmentShader);

			ImGui.SetCurrentContext(ImGui.CreateContext());
			RebuildFontAtlas();
			InitKeyMap();

			_vboHandle = GenBuffer();
			_elementsHandle = GenBuffer();
			_vertexArrayObject = GenVertexArray();
		}

		unsafe void RebuildFontAtlas()
		{
			var fonts = ImGui.GetIO().Fonts;

			fonts.AddFontDefault();
			fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int _);

			_fontTextureId = ImGuiGL.LoadTexture((IntPtr)pixelData, width, height);

			fonts.TexID = (IntPtr)_fontTextureId;
			fonts.ClearTexData();
		}

		public void Render()
		{
			ImGui.Render();

			var io = ImGui.GetIO();
			glViewport(0, 0, (int)io.DisplaySize.X, (int)io.DisplaySize.Y);
			glClearColor(0.45f, 0.55f, 0.60f, 1.00f);
			glClear(ClearBufferMask.ColorBufferBit);

			RenderDrawData();

			glDisable(EnableCap.ScissorTest);
		}

		void SetupRenderState(ImDrawDataPtr drawData, int fbWidth, int fbHeight)
		{
			glEnable(EnableCap.Blend);
			glBlendEquation(BlendEquationMode.FuncAdd);
			glBlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			glDisable(EnableCap.CullFace);
			glDisable(EnableCap.DepthTest);
			glEnable(EnableCap.ScissorTest);

			glUseProgram(_shader.ProgramID);

			var left = drawData.DisplayPos.X;
			var right = drawData.DisplayPos.X + drawData.DisplaySize.X;
			var top = drawData.DisplayPos.Y;
			var bottom = drawData.DisplayPos.Y + drawData.DisplaySize.Y;

			_shader["Texture"].SetValue(0);
			_shader["ProjMtx"].SetValue(Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1, 1));
			glBindSampler(0, 0);

			glBindVertexArray(_vertexArrayObject);

			// Bind vertex/index buffers and setup attributes for ImDrawVert
			glBindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
			glBindBuffer(BufferTarget.ElementArrayBuffer, _elementsHandle);

			EnableVertexAttribArray(_shader["Position"].Location);
			EnableVertexAttribArray(_shader["UV"].Location);
			EnableVertexAttribArray(_shader["Color"].Location);

			var drawVertSize = Marshal.SizeOf<ImDrawVert>();
			VertexAttribPointer(_shader["Position"].Location, 2, VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("pos"));
			VertexAttribPointer(_shader["UV"].Location, 2, VertexAttribPointerType.Float, false, drawVertSize, Marshal.OffsetOf<ImDrawVert>("uv"));
			VertexAttribPointer(_shader["Color"].Location, 4, VertexAttribPointerType.UnsignedByte, true, drawVertSize, Marshal.OffsetOf<ImDrawVert>("col"));
		}

		unsafe void RenderDrawData()
		{
			var drawData = ImGui.GetDrawData();

			// Avoid rendering when minimized, scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
			var fbWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
			var fbHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);
			if (fbWidth <= 0 || fbHeight <= 0)
				return;

			SetupRenderState(drawData, fbWidth, fbHeight);

			var clipOffset = drawData.DisplayPos;
			var clipScale = drawData.FramebufferScale;

			drawData.ScaleClipRects(clipScale);

			var lastTexId = ImGui.GetIO().Fonts.TexID;
			glBindTexture(TextureTarget.Texture2D, (uint)lastTexId);

			var drawVertSize = Marshal.SizeOf<ImDrawVert>();
			var drawIdxSize = sizeof(ushort);

			for (var n = 0; n < drawData.CmdListsCount; n++)
			{
				var cmdList = drawData.CmdListsRange[n];

				// Upload vertex/index buffers
				glBufferData(BufferTarget.ArrayBuffer, (IntPtr)(cmdList.VtxBuffer.Size * drawVertSize), cmdList.VtxBuffer.Data, BufferUsageHint.StreamDraw);
				glBufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(cmdList.IdxBuffer.Size * drawIdxSize), cmdList.IdxBuffer.Data, BufferUsageHint.StreamDraw);

				for (var cmd_i = 0; cmd_i < cmdList.CmdBuffer.Size; cmd_i++)
				{
					var pcmd = cmdList.CmdBuffer[cmd_i];
					if (pcmd.UserCallback != IntPtr.Zero)
					{
						Console.WriteLine("UserCallback not implemented");
					}
					else
					{

						// Project scissor/clipping rectangles into framebuffer space
						var clip_rect = pcmd.ClipRect;

						clip_rect.X = pcmd.ClipRect.X - clipOffset.X;
						clip_rect.Y = pcmd.ClipRect.Y - clipOffset.Y;
						clip_rect.Z = pcmd.ClipRect.Z - clipOffset.X;
						clip_rect.W = pcmd.ClipRect.W - clipOffset.Y;

						glScissor((int)clip_rect.X, (int)(fbHeight - clip_rect.W), (int)(clip_rect.Z - clip_rect.X), (int)(clip_rect.W - clip_rect.Y));

						// Bind texture, Draw
						if (pcmd.TextureId != IntPtr.Zero)
						{
							if (pcmd.TextureId != lastTexId)
							{
								lastTexId = pcmd.TextureId;
								glBindTexture(TextureTarget.Texture2D, (uint)pcmd.TextureId);
							}
						}

						glDrawElementsBaseVertex(BeginMode.Triangles, (int)pcmd.ElemCount, drawIdxSize == 2 ? DrawElementsType.UnsignedShort : DrawElementsType.UnsignedInt, (IntPtr)(pcmd.IdxOffset * drawIdxSize), (int)pcmd.VtxOffset);
					}
				}
			}
		}

		public void Dispose()
		{
			if (_shader != null)
			{
				_shader.Dispose();
				_shader = null;
				DeleteBuffer(_vboHandle);
				DeleteBuffer(_elementsHandle);
				DeleteVertexArray(_vertexArrayObject);
				DeleteTexture(_fontTextureId);
			}
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
