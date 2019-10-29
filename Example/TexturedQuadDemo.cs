using System;
using System.Numerics;
using OpenGL;

namespace Example
{
	public class TexturedQuadDemo : IDemo, IDisposable
	{
		private static int width = 800, height = 600;
		ShaderProgram program;
		VAO _quad;
		Texture crateTexture;

		public TexturedQuadDemo()
		{
			program = new ShaderProgram(new Shader(vertexShader2Source, ShaderType.VertexShader), new Shader(fragmentShader2Source, ShaderType.FragmentShader));
			program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)800 / 600, 0.1f, 1000f));
			program["modelview_matrix"].SetValue(Matrix4.CreateScaling(new Vector3(2)) * Matrix4.CreateTranslation(new Vector3(2, 2, -10)) * Matrix4.CreateRotation(new Vector3(1, -1, 0), 0.2f));

			Console.WriteLine(program.ProgramLog);

			crateTexture = new Texture("Assets/crate.jpg");

			// create the vertex data
			var vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0) };
			var vertexVBO = new VBO<Vector3>(vertices);

			// create the UV data
			var uvs = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
			var uvVBO = new VBO<Vector2>(uvs);

			// create the index data (the order in which the vertices should be drawn in groups of 3 to form triangles)
			var indices = new uint[] { 0, 1, 2, 2, 3, 0 };
			var indexVBO = new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead);

			// create a vertex array object (VAO) from the vertex, UV and index data
			_quad = new VAO(program, vertexVBO, uvVBO, indexVBO);
		}

		public void Render()
		{
			Gl.Viewport(0, 0, width, height);
			Gl.ClearColor(0.64f, 0.3f, 0.3f, 1);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			_quad.Program.Use();
			Gl.BindTexture(crateTexture);
			_quad.Draw();
		}


		static string vertexShader2Source = @"#version 330

layout(location = 0)in vec4 in_position;
layout(location = 1)in vec2 in_uv;

out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

void main()
{
	uv = in_uv;
    gl_Position = projection_matrix * modelview_matrix * in_position;
}";

		static string fragmentShader2Source = @"#version 330
in vec2 uv;
out vec4 fragColor;
uniform sampler2D tex;

void main()
{
    fragColor = texture(tex, uv);
}";

		public void Dispose()
		{
			program.Dispose();
			_quad.Dispose();
			crateTexture.Dispose();
		}
	}
}
